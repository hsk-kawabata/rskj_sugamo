Option Explicit On
Option Strict Off
Imports CASTCommon
Public Class KFGOTHR010

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 口座振替データ作成取消
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Dim STR請求年月 As String
    Dim STR選択振替日 As String
    Dim STR編集振替日 As String
    Dim STR編集再振替日 As String
    Dim STR再振替日 As String
    Dim LNG削除件数 As Long
    Dim LNG読込件数 As Long
    Dim STR前回振替日 As String
    Dim STR再振種別 As String
    Dim STR処理名 As String

    Private STR選択振替区分 As String

    Public iGakunen_Flag() As Integer
    Public strSCH_KBN As String 'スケジュール区分

    Dim LNG企業削除件数 As Long '追加 2007/09/05
    Private MainLOG As New CASTCommon.BatchLOG("KFGOTHR010", "口座振替データ作成取消画面")
    Private Const msgTitle As String = "口座振替データ作成取消画面(KFGOTHR010)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

    '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
    'システムで設定できる最小の学年コードと最大の学年コード
    Private Const MIN_SYS_GAKUNEN_CODE As Integer = 1
    Private Const MAX_SYS_GAKUNEN_CODE As Integer = 9
    '使用学年
    Private INT使用学年 As Integer
    '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGOTHR010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "口座振替データ作成取消画面"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            'テキストファイルからコンボボックス設定
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbFURIKUBUN)")
                Exit Sub
            End If

            'Oracle 接続(Read専用)
            OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
            'Oracle OPEN(Read専用)
            OBJ_CONNECTION_DREAD.Open()

            '入力ボタン制御
            btnSEARCH.Enabled = True
            btnAction.Enabled = False
            btnEraser.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")

        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnSEARCH_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSEARCH.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)開始", "成功", "")
            '検索ボタン
            '入力項目の共通チェック
            If PFUNC_COMMON_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            If PFUNC_SCHMAST_GET() = False Then
                Exit Sub
            End If

            '入力項目制御
            Call PSUB_INPUTEnabled(False)

            '入力ボタン制御
            btnSEARCH.Enabled = False
            btnAction.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        '実行ボタン
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            '入力項目の共通チェック
            If PFUNC_COMMON_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            'チェックボックス状態チェック
            '空リストの場合は終了
            If chkLIST.Items.Count = 0 Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "口座振替データ作成取消"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Exit Sub
            End If

            'トランザクション開始
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
                Exit Sub
            End If

            LNG削除件数 = 0

            'チェックボックスがonのものを探す
            For iCount As Integer = 0 To (chkLIST.Items.Count - 1)
                'If chkLIST.GetItemChecked(iCount) = True Then
                STR選択振替日 = chkLIST.Items(iCount).ToString

                '請求年月の獲得のため、スケジュールマスタの再GET
                If PFUNC_SEIKYUNENTUKI_GET() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "請求年月の獲得", "失敗", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)

                    Exit Sub
                End If

                '口座振替明細データの削除（初振、再振）
                If PFUNC_MEISAI_DEL() = False Then
                    '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "明細マスタの削除（初振、再振）", "失敗", Err.Description)
                    'MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタの削除（初振、再振）", "失敗", Err.Description)
                    '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If

                'スケジュールマスタの更新（初振、再振）
                If PFUNC_SCHMAST_UPD() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタの更新（初振、再振）", "失敗", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If

                '前回振替明細データの更新
                '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
                '振替区分が随時処理（入金・出金）の場合は、前回振替明細のデータ更新は行わない。
                Select Case Me.STR選択振替区分
                    Case "2", "3"
                        '随時処理（入金・出金）
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "前回振替明細データの更新", "成功", "随時処理の取消のため処理不要")

                    Case Else
                        '随時処理以外
                        Select Case (STR再振種別)
                            Case "1", "2", "3"   '再振あり、繰越あり
                                If PFUNC_ZENMEISAI_UPD() = False Then
                                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "前回振替明細データの更新", "失敗", Err.Description)
                                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                                    Exit Sub
                                End If
                        End Select
                End Select
                'Select Case (STR再振種別)
                '    Case "1", "2", "3"   '再振あり、繰越あり
                '        If PFUNC_ZENMEISAI_UPD() = False Then
                '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "前回振替明細データの更新", "失敗", Err.Description)
                '            Call GFUNC_EXECUTESQL_TRANS("", 3)
                '            Exit Sub
                '        End If
                'End Select
                '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
                'End If
            Next iCount

            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            If CASTCommon.GetFSKJIni("COMMON", "KIGYO_JIFURI") = "1" Then
                '明細マスタの削除（企業）
                If Me.PFUNC_KIGYO_MEISAI_DEL() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "明細マスタの削除（企業）", "失敗", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If

                'スケジュールマスタの削除（企業）
                If Me.PFUNC_KIGYO_SCHMAST_UPD() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタの更新（企業）", "失敗", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If
            End If
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

            'リストボックスでチェックONのものを後ろから削除する
            For iCount As Integer = (chkLIST.Items.Count - 1) To 0 Step -1
                chkLIST.Items.RemoveAt(iCount)
            Next

            'リストボックスの有無
            If chkLIST.Items.Count = 0 Then
                'リストボックスの無の場合
                '入力項目制御
                Call PSUB_INPUTEnabled(True)

                '学校コードにカーソル設定
                txtGAKKOU_CODE.Focus()

                '入力ボタン制御
                btnSEARCH.Enabled = True
                btnAction.Enabled = False
                btnEraser.Enabled = True
                btnEnd.Enabled = True
            Else
                'リストボックスの有の場合
                '入力項目制御
                Call PSUB_INPUTEnabled(False)

                '入力ボタン制御
                btnSEARCH.Enabled = False
                btnAction.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
            End If

            'トランザクション終了（ＣＯＭＭＩＴ）
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                Exit Sub
            End If

            MessageBox.Show(String.Format(G_MSG0001I, LNG削除件数, LNG企業削除件数), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '取消ボタン
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            '入力項目　抑止の解除
            Call PSUB_INPUTEnabled(True)

            '口座振替データ作成一覧
            chkLIST.Items.Clear()

            '入力項目制御
            '学校コードにカーソル設定
            txtGAKKOU_CODE.Focus()

            '入力ボタン制御
            btnSEARCH.Enabled = True
            btnAction.Enabled = False
            btnEraser.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '********************************************
        '終了ボタン
        '********************************************
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD.Close()
            OBJ_CONNECTION_DREAD = Nothing
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_INPUTEnabled(ByVal pValue As Boolean)

        '入力項目の抑止
        cmbKana.Enabled = pValue
        cmbGakkouName.Enabled = pValue

        txtGAKKOU_CODE.Enabled = pValue
        txtFURINEN.Enabled = pValue
        txtFURITUKI.Enabled = pValue
        txtFURIHI.Enabled = pValue

        cmbFURIKUBUN.Enabled = pValue

    End Sub
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校検索
        Call GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName)

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        '学校名の取得
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '学校名検索
            STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            OBJ_DATAREADER.Read()
            If OBJ_DATAREADER.HasRows = True Then
                lab学校名.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
            Else
                lab学校名.Text = ""
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If
        End If

    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_COMMON_CHECK() As Boolean

        Dim bFlg As Boolean

        PFUNC_COMMON_CHECK = False

        STR選択振替区分 = ""

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '学校マスタ存在チェック
            STR_SQL = "SELECT SFURI_SYUBETU_T"
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            '使用学年も取得
            STR_SQL += ", SIYOU_GAKUNEN_T"
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END
            STR_SQL += " FROM GAKMAST2"
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            STR再振種別 = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SFURI_SYUBETU_T")
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            INT使用学年 = GCom.NzInt(GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SIYOU_GAKUNEN_T"))
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

            If Trim(STR再振種別) = "" Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If
        End If

        '振替日
        If Trim(txtFURINEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURINEN.Focus()

            Exit Function

        End If

        If Trim(txtFURITUKI.Text) = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURITUKI.Focus()

            Exit Function
        Else
            Select Case (CInt(txtFURITUKI.Text))
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFURITUKI.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFURIHI.Text) = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURIHI.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtFURINEN.Text) & "/" & Trim(txtFURITUKI.Text) & "/" & Trim(txtFURIHI.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFURINEN.Text) & Format(CInt(txtFURITUKI.Text), "00") & Format(CInt(txtFURIHI.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then

            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            txtFURINEN.Focus()

            Exit Function
        End If

        '振替区分
        If cmbFURIKUBUN.SelectedIndex < 0 Then

            MessageBox.Show(G_MSG0006W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbFURIKUBUN.Focus()
            Exit Function
        End If

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        bFlg = False

        With OBJ_DATAREADER
            .Read()

            If Trim(.Item("FUNOU_FLG_S")) = "1" Then
                bFlg = True
            End If
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If bFlg = True Then
            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        STR選択振替区分 = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)

        PFUNC_COMMON_CHECK = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False

        'スケジュールから対象データ検索する

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '画面の振替区分は変数に格納済みなので、そちらを使用する。
        STR_SQL += " AND FURI_KBN_S ='" & Me.STR選択振替区分 & "'"
        'STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        LNG読込件数 = 0

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        'リストボックスのクリア
        chkLIST.Items.Clear()

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                '金額確認済フラグ、データ作成済フラグが１の物のみ
                If .Item("CHECK_FLG_S") = "1" And .Item("DATA_FLG_S") = "1" Then
                    '振替日の編集
                    STR編集振替日 = Mid(.Item("FURI_DATE_S"), 1, 4) & "/" & Mid(.Item("FURI_DATE_S"), 5, 2) & "/" & Mid(.Item("FURI_DATE_S"), 7, 2)
                    '再振替日の編集
                    If .Item("SFURI_DATE_S") <> "00000000" Then
                        STR編集再振替日 = Mid(.Item("SFURI_DATE_S"), 1, 4) & "/" & Mid(.Item("SFURI_DATE_S"), 5, 2) & "/" & Mid(.Item("SFURI_DATE_S"), 7, 2)
                    Else
                        STR編集再振替日 = ""
                    End If

                    '企業自振スケジュールマスタのフラグチェック
                    If PFUNC_KSCHMAST_GET() = False Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If

                        Exit Function
                    End If
                    'リストボックスへ追加
                    chkLIST.Items.Add("振替日：" & STR編集振替日 & "  " & "再振替日：" & STR編集再振替日)
                    LNG読込件数 += 1
                End If
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If LNG読込件数 = 0 Then
            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_KSCHMAST_GET() As Boolean

        PFUNC_KSCHMAST_GET = False

        '企業自振スケジュールマスタのフラグチェック
        STR_SQL = " SELECT * FROM SCHMAST"
        STR_SQL += " WHERE TORIS_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '画面の振替区分は変数に格納済みなので、そちらを使用する。
        STR_SQL += " AND TORIF_CODE_S ='0" & (CInt(Me.STR選択振替区分) + 1) & "'"
        'STR_SQL += " AND TORIF_CODE_S ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If

            PFUNC_KSCHMAST_GET = True

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                If .Item("HAISIN_FLG_S") <> "0" Then
                    If GFUNC_SELECT_SQL3("", 1) = False Then
                        Exit Function
                    End If

                    MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    Exit Function
                End If
            End With
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        PFUNC_KSCHMAST_GET = True

    End Function
    Private Function PFUNC_SEIKYUNENTUKI_GET() As Boolean

        PFUNC_SEIKYUNENTUKI_GET = False

        'スケジュールから対象データの請求年月を獲得する

        'リストボックスから選択したものから、再振替日を抽出する
        STR再振替日 = Mid(STR選択振替日, 22, 4) & Mid(STR選択振替日, 27, 2) & Mid(STR選択振替日, 30, 2)
        If STR再振替日 = "" Then
            STR再振替日 = "00000000"
        End If

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '画面の振替区分は変数に格納済みなので、そちらを使用する。
        STR_SQL += " AND FURI_KBN_S ='" & Me.STR選択振替区分 & "'"
        'STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND SFURI_DATE_S ='" & STR再振替日 & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If

            STR請求年月 = ""

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function

        End If

        OBJ_DATAREADER_DREAD.Read()

        STR請求年月 = OBJ_DATAREADER_DREAD.Item("NENGETUDO_S")

        'スケジュールが「特別」取消対象学年取得 2005/12/09
        strSCH_KBN = OBJ_DATAREADER_DREAD.Item("SCH_KBN_S")
        With OBJ_DATAREADER_DREAD
            'SCH_KBN_S=1:特別振替日 SCH_KBN_S=2:随時振替日(学年指定)
            If strSCH_KBN <> "0" Then

                ReDim iGakunen_Flag(0)  '初期化
                '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
                For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    'For i As Integer = 1 To 9
                    '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
                    ReDim Preserve iGakunen_Flag(i)
                    If .Item("GAKUNEN" & i & "_FLG_S") = "1" Then
                        iGakunen_Flag(i) = 1
                    Else
                        iGakunen_Flag(i) = 0
                    End If
                Next
            End If
        End With

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        PFUNC_SEIKYUNENTUKI_GET = True

    End Function
    Private Function PFUNC_MEISAI_DEL() As Boolean

        PFUNC_MEISAI_DEL = False

        '口座振替明細データの削除

        '初振 振替データ削除処理
        '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        STR_SQL = " SELECT COUNT(*) AS CNT FROM G_MEIMAST"
        'STR_SQL = " SELECT * FROM G_MEIMAST"
        '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR請求年月 & "'"
        STR_SQL += " AND FURI_DATE_M ='" & STR_FURIKAE_DATE(1) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '画面の振替区分は変数に格納済みなので、そちらを使用する。
        STR_SQL += " AND FURI_KBN_M ='" & Me.STR選択振替区分 & "'"
        'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
        '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '特別振替日、随時スケジュール時は処理対象の学年を条件に含める。
        If Me.strSCH_KBN <> "0" Then
            '特別振替日、随時スケジュール
            STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
            For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                If iGakunen_Flag(i) = 1 Then
                    STR_SQL += ","
                    STR_SQL += i.ToString
                End If
            Next
            STR_SQL += ")"
        End If
        'IN句のダミー部を削除
        STR_SQL = STR_SQL.Replace("XXX,", "")
        '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        '件数カウント
        '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        If OBJ_DATAREADER_DREAD.Read = True Then
            LNG削除件数 += GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))
        End If
        'While (OBJ_DATAREADER_DREAD.Read = True)
        '    LNG削除件数 += 1
        'End While
        '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '削除処理
        STR_SQL = " DELETE  FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR請求年月 & "'"
        STR_SQL += " AND FURI_DATE_M ='" & STR_FURIKAE_DATE(1) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '画面の振替区分は変数に格納済みなので、そちらを使用する。
        STR_SQL += " AND FURI_KBN_M ='" & Me.STR選択振替区分 & "'"
        'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
        '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '特別振替日、随時スケジュール時は処理対象の学年を条件に含める。
        If Me.strSCH_KBN <> "0" Then
            '特別振替日、随時スケジュール
            STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
            For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                If iGakunen_Flag(i) = 1 Then
                    STR_SQL += ","
                    STR_SQL += i.ToString
                End If
            Next
            STR_SQL += ")"
        End If
        'IN句のダミー部を削除
        STR_SQL = STR_SQL.Replace("XXX,", "")
        '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        '再振　振替データ削除処理
        '2020/02/06 タスク）斎藤 NOTE 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '下記のIf文処理は、現在の仕様に沿っていない。（再振まで処理が進んでいる初振を取消したいのか？）
        '初振の取消　　　　　→　再振の明細は存在しない
        '再振の取消　　　　　→　再振日は"00000000"
        '持越有の再振の取消　→　再振日には次回初振日が入るが、処理していないため明細は無し
        '2020/02/06 タスク）斎藤 NOTE 標準版対応（口座振替データ作成取消画面改修） -------------------- END
        If STR再振替日 <> "00000000" Then
            '再振スケジュールありの場合
            '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            STR_SQL = " SELECT COUNT(*) AS CNT FROM G_MEIMAST"
            'STR_SQL = " SELECT * FROM G_MEIMAST"
            '2020/02/06 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
            STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR請求年月 & "'"
            STR_SQL += " AND FURI_DATE_M ='" & STR再振替日 & "'"
            '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            '画面の振替区分は変数に格納済みなので、そちらを使用する。
            STR_SQL += " AND FURI_KBN_M ='" & Me.STR選択振替区分 & "'"
            'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
            '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            '特別振替日、随時スケジュール時は処理対象の学年を条件に含める。
            If Me.strSCH_KBN <> "0" Then
                '特別振替日、随時スケジュール
                STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
                For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    If iGakunen_Flag(i) = 1 Then
                        STR_SQL += ","
                        STR_SQL += i.ToString
                    End If
                Next
                STR_SQL += ")"
            End If
            'IN句のダミー部を削除
            STR_SQL = STR_SQL.Replace("XXX,", "")
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

            If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
                Exit Function
            End If

            '件数カウント
            '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            If OBJ_DATAREADER_DREAD.Read = True Then
                LNG削除件数 += GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))
            End If
            'While (OBJ_DATAREADER_DREAD.Read = True)
            '    LNG削除件数 += 1
            'End While
            '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END

            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If

            '削除処理
            STR_SQL = " DELETE  FROM G_MEIMAST"
            STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR請求年月 & "'"
            STR_SQL += " AND FURI_DATE_M ='" & STR再振替日 & "'"
            '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            '画面の振替区分は変数に格納済みなので、そちらを使用する。
            STR_SQL += " AND FURI_KBN_M ='" & Me.STR選択振替区分 & "'"
            'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
            '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
            '特別振替日、随時スケジュール時は処理対象の学年を条件に含める。
            If Me.strSCH_KBN <> "0" Then
                '特別振替日、随時スケジュール
                STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
                For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    If iGakunen_Flag(i) = 1 Then
                        STR_SQL += ","
                        STR_SQL += i.ToString
                    End If
                Next
                STR_SQL += ")"
            End If
            'IN句のダミー部を削除
            STR_SQL = STR_SQL.Replace("XXX,", "")
            '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '削除処理エラー
                MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        '2020/02/06 タスク）斎藤 DEL 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '特別振替日で初振日が同一日、再振日が別日の場合に、初振の取消でその請求月に登録されている特別振替日数回
        '明細マスタの削除を行うため、別途明細マスタの削除処理を実装する。
        'STR_SQL = " SELECT * FROM MEIMAST"
        'STR_SQL += " WHERE TORIS_CODE_K ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND TORIF_CODE_K ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        'STR_SQL += " AND FURI_DATE_K ='" & STR_FURIKAE_DATE(1) & "'"

        'If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
        '    Exit Function
        'End If

        'LNG企業削除件数 = 0
        ''件数カウント
        'While (OBJ_DATAREADER_DREAD.Read = True)
        '    LNG企業削除件数 += 1
        'End While

        'If GFUNC_SELECT_SQL3("", 1) = False Then
        '    Exit Function
        'End If

        ''削除処理
        'STR_SQL = " DELETE FROM MEIMAST"
        'STR_SQL += " WHERE TORIS_CODE_K ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND TORIF_CODE_K ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        'STR_SQL += " AND FURI_DATE_K ='" & STR_FURIKAE_DATE(1) & "'"

        'If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '    '削除処理エラー
        '    MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    Exit Function
        'End If
        '2020/02/06 タスク）斎藤 DEL 標準版対応（口座振替データ作成取消画面改修） -------------------- END

        PFUNC_MEISAI_DEL = True

    End Function
    Private Function PFUNC_SCHMAST_UPD() As Boolean

        Dim sSyori_Date As String

        PFUNC_SCHMAST_UPD = False

        'スケジュールマスタの更新
        sSyori_Date = Format(Now, "yyyyMMddHHmmss")

        '初振スケジュールマスタ更新
        STR_SQL = " UPDATE  G_SCHMAST SET "
        '金額確認フラグ
        STR_SQL += " CHECK_FLG_S ='0'"
        '振替データ作成フラグ
        STR_SQL += ",DATA_FLG_S ='0'"
        STR_SQL += " WHERE GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '画面の振替区分は変数に格納済みなので、そちらを使用する。
        STR_SQL += " AND FURI_KBN_S ='" & Me.STR選択振替区分 & "'"
        'STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '更新処理エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        '再振スケジュールマスタ処理
        If STR再振替日 <> "00000000" Then
            '再振スケジュールありの場合
            STR_SQL = " UPDATE  G_SCHMAST SET "
            If STR再振替日 <> "00000000" Then
                '再振データ作成フラグ
                STR_SQL += " SAIFURI_FLG_S ='0'"
            End If

            STR_SQL += " WHERE GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND FURI_DATE_S ='" & STR再振替日 & "'"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '更新処理エラー
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        '2020/02/06 タスク）斎藤 DEL 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '特別振替日で初振日が同一日、再振日が別日の場合に、初振の取消でその請求月に登録されている特別振替日数回
        'スケジュールマスタの削除を行うため、別途スケジュールマスタの削除処理を実装する。
        ''企業自振連携
        'STR_SQL = " UPDATE  SCHMAST SET "
        ''受付フラグ
        'STR_SQL += " UKETUKE_FLG_S ='0'"
        ''登録フラグ
        'STR_SQL += ",TOUROKU_FLG_S ='0'"
        '' 2016/04/25 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
        ''スケジュールの更新項目の追加（企業自振の落し込み取消に合わせる）
        'STR_SQL += ",SYORI_KEN_S = 0"
        'STR_SQL += ",SYORI_KIN_S = 0"
        'STR_SQL += ",ERR_KEN_S = 0"
        'STR_SQL += ",ERR_KIN_S = 0"
        'STR_SQL += ",TESUU_KIN_S = 0"
        'STR_SQL += ",TESUU_KIN1_S = 0"
        'STR_SQL += ",TESUU_KIN2_S = 0"
        'STR_SQL += ",TESUU_KIN3_S = 0"
        'STR_SQL += ",FURI_KEN_S = 0"
        'STR_SQL += ",FURI_KIN_S = 0"
        'STR_SQL += ",FUNOU_KEN_S = 0"
        'STR_SQL += ",FUNOU_KIN_S = 0"
        'STR_SQL += ",UKETUKE_DATE_S = '" & New String("0"c, 8) & "'"
        'STR_SQL += ",TOUROKU_DATE_S = '" & New String("0"c, 8) & "'"
        'STR_SQL += ",UFILE_NAME_S = NULL"
        'STR_SQL += ",ERROR_INF_S = NULL"
        '' 2016/04/25 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
        'STR_SQL += " WHERE TORIS_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND TORIF_CODE_S ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        'STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        'If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '    '更新処理エラー
        '    MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    Exit Function
        'End If

        ''2018/10/23 saitou 広島信金(RSV2対応) ADD 大規模機能追加 START
        'If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
        '    STR_SQL = " UPDATE SCHMAST_SUB SET "
        '    STR_SQL &= "  SYOUGOU_FLG_S = '0' "
        '    STR_SQL &= " ,SYOUGOU_DATE_S = '00000000' "
        '    STR_SQL &= " ,UKETUKE_TIME_STAMP_S = '00000000000000' "
        '    STR_SQL &= " WHERE TORIS_CODE_SSUB  ='" & txtGAKKOU_CODE.Text & "'"
        '    STR_SQL &= " AND TORIF_CODE_SSUB ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        '    STR_SQL &= " AND FURI_DATE_SSUB ='" & STR_FURIKAE_DATE(1) & "'"

        '    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '        '更新処理エラー
        '        MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        Exit Function
        '    End If
        'End If
        ''2018/10/23 saitou 広島信金(RSV2対応) ADD 大規模機能追加 END
        '2020/02/06 タスク）斎藤 DEL 標準版対応（口座振替データ作成取消画面改修） -------------------- END

        PFUNC_SCHMAST_UPD = True

    End Function
    Private Function PFUNC_ZENMEISAI_UPD() As Boolean
        Dim bLoopFlg As Boolean = False '指定学年を条件に追加したかチェック
        Dim iLcount As Integer '指定学年ループ数

        PFUNC_ZENMEISAI_UPD = False

        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- START
        '再振種別と初振・再振に不能が存在したか、現在取消対象のスケジュールの振替区分は何かで条件が変更になるため、
        '処理の見直しを実施する。

        '学校マスタの再振種別と画面選択されている振替区分によって、前回明細の更新を行うか判断
        Select Case Me.STR再振種別
            Case "0"
                '再振種別「無」・持込種別「無」
                PFUNC_ZENMEISAI_UPD = True
                Exit Function
            Case "1"
                '再振種別「有」・持込種別「無」
                If Me.STR選択振替区分 = "0" Then
                    '振替区分「初振」
                    PFUNC_ZENMEISAI_UPD = True
                    Exit Function
                End If
        End Select

        Dim ZenFuriDateArray(INT使用学年) As String

        '前回明細マスタ取得
        '検索キーは、学校コード、振替結果コード、再振済フラグで、各学年の最大の振替日
        Dim SQL As New System.Text.StringBuilder
        With SQL
            .Length = 0
            .Append("SELECT")
            .Append("     GAKUNEN_CODE_M")
            .Append("   , MAX(FURI_DATE_M) AS FURI_DATE_M")
            .Append(" FROM")
            .Append("     G_MEIMAST")
            .Append(" WHERE")
            .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
            .Append(" AND SAIFURI_SUMI_M = '1'")
            .Append(" AND FURI_KBN_M IN ('0', '1')")
            If Me.strSCH_KBN = "0" Then
                '年間スケジュールの場合
                .Append(" AND GAKUNEN_CODE_M IN (XXX")
                For iLcount = MIN_SYS_GAKUNEN_CODE To INT使用学年
                    .Append(",").Append(iLcount.ToString)
                Next
                .Append(")")
            Else
                '特別振替日の場合
                .Append(" AND GAKUNEN_CODE_M IN (XXX")
                For iLcount = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    If iGakunen_Flag(iLcount) = 1 Then
                        .Append(",").Append(iLcount.ToString)
                    End If
                Next
                .Append(")")
            End If
            .Append(" GROUP BY")
            .Append("     GAKUNEN_CODE_M")
            .Append(" ORDER BY")
            .Append("     GAKUNEN_CODE_M")
            'IN句のダミー部を削除
            .Replace("XXX,", "")
        End With

        If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If
            PFUNC_ZENMEISAI_UPD = True

            Exit Function
        End If

        For i As Integer = 0 To ZenFuriDateArray.Length - 1
            ZenFuriDateArray(i) = String.Empty
        Next

        While (OBJ_DATAREADER_DREAD.Read = True)
            '前回振替日の取得
            Dim GetGakunenCode As Integer = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("GAKUNEN_CODE_M"))
            ZenFuriDateArray(GetGakunenCode) = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M").ToString
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '前回振替日と同一日付の明細マスタを更新する前に、本当に更新してよいかチェック
        If Me.STR選択振替区分 = "0" Then
            '振替区分が「初振」の場合、前回振替日より未来で不能件数が存在しないスケジュールがあるかチェックする
            For iLcount = MIN_SYS_GAKUNEN_CODE To ZenFuriDateArray.Length - 1
                If ZenFuriDateArray(iLcount) <> String.Empty Then

                    Me.STR前回振替日 = ZenFuriDateArray(iLcount)

                    With SQL
                        .Length = 0
                        .Append("SELECT")
                        .Append("     *")
                        .Append(" FROM")
                        .Append("     G_SCHMAST")
                        .Append(" WHERE")
                        .Append("     GAKKOU_CODE_S = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                        .Append(" AND FURI_DATE_S > '").Append(Me.STR前回振替日).Append("'")
                        .Append(" AND FURI_KBN_S IN ('0', '1')")
                        .Append(" AND FUNOU_FLG_S = '1'")
                        .Append(" AND GAKUNEN").Append(iLcount.ToString).Append("_FLG_S = '1'")
                        .Append(" ORDER BY")
                        .Append("     FURI_DATE_S")
                    End With

                    If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
                        Exit Function
                    End If

                    If OBJ_DATAREADER_DREAD.Read() = True Then
                        '不能件数の取得
                        Dim FunouKen As Integer = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("FUNOU_KEN_S"))

                        Dim FuriDate As String = OBJ_DATAREADER_DREAD.Item("FURI_DATE_S").ToString
                        Dim FuriKbn As String = OBJ_DATAREADER_DREAD.Item("FURI_KBN_S").ToString

                        '明細マスタ更新対象かチェック
                        '前回振替日より未来で不能件数が存在しない場合、
                        '前回振替日から今回取消振替日間で資金が引き落とされているため、
                        '明細マスタの更新は不要
                        If FunouKen = 0 Then
                            If GFUNC_SELECT_SQL3("", 1) = False Then
                                Exit Function
                            End If

                            Continue For
                            'PFUNC_ZENMEISAI_UPD = True
                            'Exit Function
                        Else
                            '再度ここで明細マスタから件数の取得を行う
                            If GFUNC_SELECT_SQL3("", 1) = False Then
                                Exit Function
                            End If

                            With SQL
                                .Length = 0
                                .Append("SELECT")
                                .Append("     COUNT(*) AS CNT")
                                .Append(" FROM")
                                .Append("     G_MEIMAST")
                                .Append(" WHERE")
                                .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                                .Append(" AND FURI_DATE_M = '").Append(FuriDate).Append("'")
                                .Append(" AND FURI_KBN_M = '").Append(FuriKbn).Append("'")
                                .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
                                .Append(" AND GAKUNEN_CODE_M = ").Append(iLcount.ToString)
                            End With

                            If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
                                Exit Function
                            End If

                            If OBJ_DATAREADER_DREAD.Read() = True Then
                                '不能件数の取得
                                FunouKen = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))

                                If FunouKen = 0 Then
                                    If GFUNC_SELECT_SQL3("", 1) = False Then
                                        Exit Function
                                    End If

                                    Continue For
                                End If
                            End If

                            If GFUNC_SELECT_SQL3("", 1) = False Then
                                Exit Function
                            End If
                        End If
                    Else
                        'レコードが存在しない、すなわち取得した振替日が前回振替日なので、明細マスタを更新する
                        If GFUNC_SELECT_SQL3("", 1) = False Then
                            Exit Function
                        End If
                    End If

                    '前回振替日と同一日付の明細マスタを更新する
                    With SQL
                        .Length = 0
                        .Append("UPDATE")
                        .Append("     G_MEIMAST")
                        .Append(" SET SAIFURI_SUMI_M = '0'")
                        .Append(" WHERE")
                        .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                        .Append(" AND FURI_DATE_M = '").Append(Me.STR前回振替日).Append("'")
                        .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
                        .Append(" AND SAIFURI_SUMI_M = '1'")
                        .Append(" AND GAKUNEN_CODE_M = ").Append(iLcount.ToString)
                    End With

                    If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                        MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Function
                    End If
                End If
            Next
        Else
            '振替区分が「再振」の場合
            Me.STR前回振替日 = String.Empty

            '再振の場合、取得した学年ごとの最大振替日からさらに最大の振替日が対象
            For iLcount = MIN_SYS_GAKUNEN_CODE To ZenFuriDateArray.Length - 1
                If ZenFuriDateArray(iLcount) >= Me.STR前回振替日 Then
                    Me.STR前回振替日 = ZenFuriDateArray(iLcount)
                End If
            Next

            '前回振替日と同一日付の明細マスタを更新する
            With SQL
                .Length = 0
                .Append("UPDATE")
                .Append("     G_MEIMAST")
                .Append(" SET SAIFURI_SUMI_M = '0'")
                .Append(" WHERE")
                .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                .Append(" AND FURI_DATE_M = '").Append(Me.STR前回振替日).Append("'")
                .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
                .Append(" AND SAIFURI_SUMI_M = '1'")
                .Append(" AND GAKUNEN_CODE_M IN (XXX")
                For iLcount = MIN_SYS_GAKUNEN_CODE To ZenFuriDateArray.Length - 1
                    If ZenFuriDateArray(iLcount).Trim <> String.Empty Then
                        .Append(",").Append(iLcount.ToString)
                    End If
                Next
                .Append(")")
                'IN句のダミー部を削除
                .Replace("XXX,", "")
            End With

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        PFUNC_ZENMEISAI_UPD = True

        ''前回明細マスタ更新（再振済フラグ）
        ''検索キーは、学校コード、振替区分、振替結果コード、再振済フラグで、振替日の降順

        'STR_SQL = " SELECT * FROM G_MEIMAST"
        'STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        ''STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
        ''STR_SQL += " AND FURIKETU_CODE_M <> 0"
        'STR_SQL += " AND FURIKETU_CODE_M IN (" & SFuriCode & ")"
        '' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
        'STR_SQL += " AND SAIFURI_SUMI_M ='1'"
        'STR_SQL += " ORDER BY FURI_DATE_M desc"

        'If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
        '    Exit Function
        'End If

        'If OBJ_DATAREADER_DREAD.Read() = True Then

        '    '一番近い振替日を前回振替日として再振済フラグを0にする 2007/01/10
        '    '初振の取消
        '    'Select case STR選択振替区分
        '    '    Case "0"
        '    'Select case STR再振種別
        '    '    Case "1", "2"
        '    '        '再振ありの場合で取得した明細が初振の明細の場合
        '    '        '前回の再振で振込が成功しているので明細マスタの更新は行わない
        '    '        If OBJ_DATAREADER_DREAD.Item("FURI_KBN_M") = 0 Then
        '    '            If GFUNC_SELECT_SQL3("", 1) = False Then
        '    '                Exit Function
        '    '            End If
        '    '            PFUNC_ZENMEISAI_UPD = True

        '    '            Exit Function
        '    '        End If
        '    'End Select
        '    'End Select

        '    STR前回振替日 = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M")
        'Else
        '    If GFUNC_SELECT_SQL3("", 1) = False Then
        '        Exit Function
        '    End If
        '    PFUNC_ZENMEISAI_UPD = True

        '    Exit Function
        'End If

        'If GFUNC_SELECT_SQL3("", 1) = False Then
        '    Exit Function
        'End If

        ''前回振替日と同一日付の明細マスタを更新する
        'STR_SQL = " UPDATE  G_MEIMAST SET "
        ''再振済フラグ
        'STR_SQL += " SAIFURI_SUMI_M ='0'"
        'STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND FURI_DATE_M ='" & STR前回振替日 & "' "
        ''STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
        ''STR_SQL += " AND FURIKETU_CODE_M <> 0"
        'STR_SQL += " AND FURIKETU_CODE_M IN (" & SFuriCode & ")"
        '' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
        ' STR_SQL += " AND SAIFURI_SUMI_M ='1'"
        'If strSCH_KBN <> "0" Then '通常以外のスケジュール時は学年指定 2005/12/09
        '    STR_SQL += " AND ("
        '    For iLcount = 1 To 9
        '        If iGakunen_Flag(iLcount) = 1 Then
        '            If bLoopFlg = True Then
        '                STR_SQL += " OR "
        '            End If
        '            STR_SQL += " GAKUNEN_CODE_M = " & iLcount
        '            bLoopFlg = True
        '        End If
        '    Next iLcount
        '    STR_SQL += " )"
        'End If

        'If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '    '更新処理エラー
        '    MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Exit Function
        'End If

        'PFUNC_ZENMEISAI_UPD = True
        '2020/02/05 タスク）斎藤 CHG 標準版対応（口座振替データ作成取消画面改修） -------------------- END

    End Function

    '2020/02/05 タスク）斎藤 DEL 標準版対応（口座振替データ作成取消画面改修） -------------------- START
    '未使用ロジックのためコメントアウト
    'Private Function PFUNC_MEISAI_SAIFURI_UPD() As Boolean

    '    PFUNC_MEISAI_SAIFURI_UPD = False

    '    '入力した振替日が再振日に該当する初振スケジュールを参照
    '    STR_SQL = " SELECT * FROM G_SCHMAST"
    '    STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
    '    STR_SQL += " AND FURI_KBN_S ='0'"
    '    STR_SQL += " AND SFURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

    '    If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
    '        Exit Function
    '    End If

    '    '取得したスケジュールより振替日(再振が発生した初振日を取得)
    '    While (OBJ_DATAREADER_DREAD.Read = True)
    '        With OBJ_DATAREADER_DREAD
    '            STR_SQL = " UPDATE  G_MEIMAST SET "
    '            STR_SQL += " SAIFURI_SUMI_M ='0'"
    '            STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
    '            STR_SQL += " AND FURI_DATE_M ='" & .Item("FURI_DATE_S") & "'"
    '            STR_SQL += " AND FURI_KBN_M ='0'"

    '            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
    '                Exit Function
    '            End If
    '        End With
    '    End While

    '    If GFUNC_SELECT_SQL3("", 1) = False Then
    '        Exit Function
    '    End If

    '    PFUNC_MEISAI_SAIFURI_UPD = True

    'End Function
    '2020/02/05 タスク）斎藤 DEL 標準版対応（口座振替データ作成取消画面改修） -------------------- END

    '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- START
    Private Function PFUNC_KIGYO_MEISAI_DEL() As Boolean

        PFUNC_KIGYO_MEISAI_DEL = False

        Dim SQL As System.Text.StringBuilder

        '削除件数カウント
        SQL = New System.Text.StringBuilder
        With SQL
            .Length = 0
            .Append("SELECT")
            .Append("     COUNT(*) AS CNT")
            .Append(" FROM")
            .Append("     MEIMAST")
            .Append(" WHERE")
            .Append("     TORIS_CODE_K = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND TORIF_CODE_K = '").Append(CStr(CInt(Me.STR選択振替区分) + 1).PadLeft(2, "0"c)).Append("'")
            .Append(" AND FURI_DATE_K = '").Append(STR_FURIKAE_DATE(1)).Append("'")
        End With

        If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.Read = True Then
            LNG企業削除件数 = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '削除処理
        With SQL
            .Length = 0
            .Append("DELETE FROM")
            .Append("    MEIMAST")
            .Append(" WHERE")
            .Append("     TORIS_CODE_K = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND TORIF_CODE_K = '").Append(CStr(CInt(Me.STR選択振替区分) + 1).PadLeft(2, "0"c)).Append("'")
            .Append(" AND FURI_DATE_K = '").Append(STR_FURIKAE_DATE(1)).Append("'")
        End With

        If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        PFUNC_KIGYO_MEISAI_DEL = True

    End Function

    Private Function PFUNC_KIGYO_SCHMAST_UPD() As Boolean

        PFUNC_KIGYO_SCHMAST_UPD = False

        Dim SQL As System.Text.StringBuilder

        SQL = New System.Text.StringBuilder

        With SQL
            .Length = 0
            .Append("UPDATE")
            .Append("     SCHMAST")
            .Append(" SET UKETUKE_FLG_S = '0'")
            .Append("   , TOUROKU_FLG_S = '0'")
            .Append("   , SYORI_KEN_S = 0")
            .Append("   , SYORI_KIN_S = 0")
            .Append("   , ERR_KEN_S = 0")
            .Append("   , ERR_KIN_S = 0")
            .Append("   , TESUU_KIN_S = 0")
            .Append("   , TESUU_KIN1_S = 0")
            .Append("   , TESUU_KIN2_S = 0")
            .Append("   , TESUU_KIN3_S = 0")
            .Append("   , FURI_KEN_S = 0")
            .Append("   , FURI_KIN_S = 0")
            .Append("   , FUNOU_KEN_S = 0")
            .Append("   , FUNOU_KIN_S = 0")
            .Append("   , UKETUKE_DATE_S = '00000000'")
            .Append("   , TOUROKU_DATE_S = '00000000'")
            .Append("   , UFILE_NAME_S = NULL")
            .Append("   , ERROR_INF_S = NULL")
            .Append(" WHERE")
            .Append("     TORIS_CODE_S = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND TORIF_CODE_S = '").Append(CStr(CInt(Me.STR選択振替区分) + 1).PadLeft(2, "0"c)).Append("'")
            .Append(" AND FURI_DATE_S = '").Append(STR_FURIKAE_DATE(1)).Append("'")
        End With

        If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
            '更新処理エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            With SQL
                .Length = 0
                .Append("UPDATE")
                .Append("     SCHMAST_SUB")
                .Append(" SET SYOUGOU_FLG_S = '0'")
                .Append("   , SYOUGOU_DATE_S = '00000000'")
                .Append("   , UKETUKE_TIME_STAMP_S = '00000000000000'")
                .Append(" WHERE")
                .Append("     TORIS_CODE_SSUB = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                .Append(" AND TORIF_CODE_SSUB = '").Append(CStr(CInt(Me.STR選択振替区分) + 1).PadLeft(2, "0"c)).Append("'")
                .Append(" AND FURI_DATE_SSUB = '").Append(STR_FURIKAE_DATE(1)).Append("'")
            End With

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                '更新処理エラー
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        PFUNC_KIGYO_SCHMAST_UPD = True

    End Function
    '2020/02/06 タスク）斎藤 ADD 標準版対応（口座振替データ作成取消画面改修） -------------------- END

#End Region

End Class
