Option Explicit On 
Option Strict Off
Imports System.Text
Imports CASTCommon
Public Class KFGMAST070

#Region " 共通変数定義 "
    Dim STR振替日 As String
    Dim STRエントリデータ予定日 As String
    Dim STRチェック予定日 As String
    Dim STR振替データ作成予定日 As String
    Dim STR不能結果更新予定日 As String
    Dim STR処理名 As String
    Dim LNG読込件数 As Long

    Private Str_TimeStamp As String
    Private Str_Nentuki As String
    Private Str_Kessai_Yotei As String

    Private Str_Sch_Kbn As String
    Private Str_Sch_Name As String

    Public intCHECK_FLG As Integer = 0
    Public intDATA_FLG As Integer = 0
    Public STR再振種別 As String = ""
    Public STR前回振替日 As String = ""
    Public STR選択振替区分 As String = ""
    Public iGakunen_Flag() As Integer
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST070", "スケジュールメンテナンス画面")
    Private Const msgTitle As String = "スケジュールメンテナンス画面(KFGMAST070)"

    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

#End Region

#Region " Form Load "
    Private Sub KFGMAST070_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            STR処理名 = "スケジュールマスタ更新"

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Call GCom.SetMonitorTopArea(Label95, Label94, lblUser, lblDate)

            'テキストファイルからコンボボックス設定
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbFURI_KBN)")
                MessageBox.Show(String.Format(MSG0013E, "振替区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '振替区分の初期値設定
            'cmbFURI_KBN.SelectedIndex = 0
            '明細作成フラグ
            txtENTRI_FLG.Text = "0"
            '金額確認済フラグ
            txtCHECK_FLG.Text = "0"
            '振替データ作成済
            txtDATA_FLG.Text = "0"
            '不能結果確認済
            txtFUNOU_FLG.Text = "0"
            '再振データ作成済
            txtSAIFURI_FLG.Text = "0"
            '決済済
            txtKESSAI_FLG.Text = "0"
            '中断フラグ
            txtTYUUDAN_FLG.Text = "0"

            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

            '入力ボタン制御
            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnSansyo.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True

            If OBJ_CONNECTION_DREAD Is Nothing Then
                'Oracle 接続(Read専用)
                OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
                'Oracle OPEN(Read専用)
                OBJ_CONNECTION_DREAD.Open()

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click

        '更新ボタン
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            '入力値チェック
            If PFUNC_Nyuryoku_Check(0) = False Then
                Exit Sub
            End If

            '2006/10/12　追加：確認メッセージを表示
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            STR_COMMAND = "更新"
            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '予定日再計算
            ' エントリデータ
            ' チェック
            ' データ作成
            ' 不能
            Call PSUB_Get_Yotei_Date()

            'トランザクション開始
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
                Exit Sub
            End If

            STR_SQL = " UPDATE  G_SCHMAST SET "
            'エントリデータ予定日
            STR_SQL += " ENTRI_YDATE_S ='" & STRエントリデータ予定日 & "'"
            'チェック予定日
            STR_SQL += ",CHECK_YDATE_S ='" & STRチェック予定日 & "'"
            'データ作成予定日
            STR_SQL += ",DATA_YDATE_S ='" & STR振替データ作成予定日 & "'"
            '不能予定日
            STR_SQL += ",FUNOU_YDATE_S ='" & STR不能結果更新予定日 & "'"
            '資金決済予定日
            STR_SQL += ",KESSAI_YDATE_S ='" & Str_Kessai_Yotei & "'"
            '明細作成フラグ
            STR_SQL += ",ENTRI_FLG_S ='" & txtENTRI_FLG.Text & "'"
            '金額チェックフラグ
            STR_SQL += ",CHECK_FLG_S ='" & txtCHECK_FLG.Text & "'"
            '振替データフラグ
            STR_SQL += ",DATA_FLG_S ='" & txtDATA_FLG.Text & "'"
            '不能チェックフラグ
            STR_SQL += ",FUNOU_FLG_S ='" & txtFUNOU_FLG.Text & "'"
            '再振フラグ
            STR_SQL += ",SAIFURI_FLG_S ='" & txtSAIFURI_FLG.Text & "'"
            '決済フラグ
            STR_SQL += ",KESSAI_FLG_S ='" & txtKESSAI_FLG.Text & "'"
            '中断フラグ
            STR_SQL += ",TYUUDAN_FLG_S ='" & txtTYUUDAN_FLG.Text & "'"
            'タイムスタンプ
            STR_SQL += ",TIME_STAMP_S ='" & Str_TimeStamp & "'"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S ='" & Str_Sch_Kbn & "'"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '更新処理エラー
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            'トランザクション終了（ＣＯＭＭＩＴ）
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            '他行分の前回不能分の再振フラグを"0"にする 2006/10/23
            '金額確認フラグのみ1⇒0に戻す時
            If (intCHECK_FLG = 1 And txtCHECK_FLG.Text = "0") And (intDATA_FLG = 0 And txtDATA_FLG.Text = "0") Then
                '前回振替明細データの更新
                Select Case STR再振種別
                    Case "1", "2", "3"   '再振あり、繰越あり
                        STR選択振替区分 = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN)

                        If PFUNC_ZENMEISAI_UPD() = False Then
                            MainLOG.Write("更新", "失敗", "前回振替明細データ更新失敗")
                            Exit Sub
                        End If
                End Select

            End If


            Call MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '入力項目制御
            'Call PSUB_KEY_Enabled(True)

            '学校コード入力にカーソル位置付け
            'txtGAKKOU_CODE.Focus()

            '入力ボタン制御
            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnSansyo.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        '削除ボタン
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")
            Dim int処理フラグ件数 As Integer

            int処理フラグ件数 = 0

            '入力値チェック
            If PFUNC_Nyuryoku_Check(0) = False Then
                Exit Sub
            End If

            '2006/10/12　追加：確認メッセージを表示
            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '処理フラグの立っているものを調べる
            STR_SQL = " SELECT * FROM G_SCHMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S ='" & Str_Sch_Kbn & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()

                Exit Sub
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            '処理フラグがONのレコードの件数を取得する
            While (OBJ_DATAREADER.Read = True)
                If OBJ_DATAREADER.Item("ENTRI_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("CHECK_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("DATA_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("FUNOU_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("SAIFURI_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("KESSAI_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("TYUUDAN_FLG_S") = "1" Then
                    int処理フラグ件数 = int処理フラグ件数 + 1
                End If
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            If int処理フラグ件数 <> 0 Then
                MessageBox.Show(MSG0290W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '全削除処理、キーは学校コード、対象年度、振替区分、振替日
            STR_SQL = " DELETE  FROM G_SCHMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S ='" & Str_Sch_Kbn & "'"
            'STR_SQL += " AND"
            'STR_SQL += " SCH_KBN_S =0"
            STR_SQL += " AND"
            STR_SQL += " ENTRI_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " CHECK_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " DATA_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " FUNOU_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " SAIFURI_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " KESSAI_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " TYUUDAN_FLG_S =0"

            'トランザクション開始
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '削除処理エラー
                MainLOG.Write("削除", "失敗", "スケジュールの削除処理でエラー")
                MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            'トランザクション終了（ＣＯＭＭＩＴ）
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            '表示項目のクリア
            Call PSUB_GAMEN_CLEAR()

            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Call PSUB_KEY_Enabled(True)

            '学校コード入力にカーソル位置付け
            txtGAKKOU_CODE.Focus()

            '入力ボタン制御
            btnKousin.Enabled = True
            btnDelete.Enabled = True
            btnSansyo.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True

            '追加 2007/02/15
            txtTAISYONEN.Text = ""
            txtTAISYOUTUKI.Text = ""
            txtFURI_DATENEN.Text = ""
            txtFURI_DATETUKI.Text = ""
            txtFURI_DATEHI.Text = ""
            cmbFURI_KBN.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnSansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyo.Click
        '参照ボタン
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")
            If PFUNC_Nyuryoku_Check(1) = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            'スケジュールマスタ存在チェック
            STR_SQL = " SELECT G_SCHMAST.* , FILE_NAME_T,SFURI_SYUBETU_T FROM G_SCHMAST , GAKMAST2"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
            STR_SQL += " AND"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MainLOG.Write("参照", "失敗", "スケジュールマスタ参照処理で０件")
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()

                Exit Sub
            End If

            '先に該当スケジュールマスタの件数を調べる
            LNG読込件数 = 0

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            While (OBJ_DATAREADER.Read = True)
                LNG読込件数 += 1
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            Select Case LNG読込件数
                Case Is = 1
                    If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                        Exit Sub
                    End If

                    While (OBJ_DATAREADER.Read = True)
                        Str_Sch_Kbn = OBJ_DATAREADER.Item("SCH_KBN_S")

                        '委託者コード
                        txtITAKU_CODE.Text = OBJ_DATAREADER.Item("ITAKU_CODE_S")
                        '再振替日
                        txtSFURI_DATENEN.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 1, 4)
                        txtSFURI_DATETUKI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 5, 2)
                        txtSFURI_DATEHI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 7, 2)

                        'エントリーデータ予定日
                        txtENTRI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 1, 4)
                        txtENTRI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 5, 2)
                        txtENTRI_YDATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 7, 2)
                        'エントリーデータ日
                        txtENTRI_DATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 1, 4)
                        txtENTRI_DATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 5, 2)
                        txtENTRI_DATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 7, 2)
                        'チェック予定日
                        txtCHECK_YDATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 1, 4)
                        txtCHECK_YDATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 5, 2)
                        txtCHECK_YDATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 7, 2)
                        'チェック日
                        txtCHECK_DATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 1, 4)
                        txtCHECK_DATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 5, 2)
                        txtCHECK_DATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 7, 2)
                        'データ作成予定日
                        txtDATA_YDATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 1, 4)
                        txtDATA_YDATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 5, 2)
                        txtDATA_YDATED.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 7, 2)
                        'データ作成日
                        txtDATA_DATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 1, 4)
                        txtDATA_DATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 5, 2)
                        txtDATA_DATED.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 7, 2)
                        '不能予定日
                        txtFUNOU_YDATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 1, 4)
                        txtFUNOU_YDATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 5, 2)
                        txtFUNOU_YDATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 7, 2)
                        '不能日
                        txtFUNOU_DATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 1, 4)
                        txtFUNOU_DATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 5, 2)
                        txtFUNOU_DATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 7, 2)
                        '資金決済予定日
                        txtKESSAI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 1, 4)
                        txtKESSAI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 5, 2)
                        txtKESSAI_YDATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 7, 2)
                        '資金決済日
                        txtKESSAI_DATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 1, 4)
                        txtKESSAI_DATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 5, 2)
                        txtKESSAI_DATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 7, 2)
                        '明細作成
                        txtENTRI_FLG.Text = OBJ_DATAREADER.Item("ENTRI_FLG_S")
                        '金額確認済
                        txtCHECK_FLG.Text = OBJ_DATAREADER.Item("CHECK_FLG_S")
                        '追加 2006/10/23
                        intCHECK_FLG = CInt(OBJ_DATAREADER.Item("CHECK_FLG_S"))
                        '振替データ作成済
                        txtDATA_FLG.Text = OBJ_DATAREADER.Item("DATA_FLG_S")
                        '追加 2006/10/23
                        intDATA_FLG = CInt(OBJ_DATAREADER.Item("DATA_FLG_S"))
                        '不能結果確認済
                        txtFUNOU_FLG.Text = OBJ_DATAREADER.Item("FUNOU_FLG_S")
                        '再振データ作成済
                        txtSAIFURI_FLG.Text = OBJ_DATAREADER.Item("SAIFURI_FLG_S")
                        '決済済
                        txtKESSAI_FLG.Text = OBJ_DATAREADER.Item("KESSAI_FLG_S")
                        '中断フラグ
                        txtTYUUDAN_FLG.Text = OBJ_DATAREADER.Item("TYUUDAN_FLG_S")
                        '処理件数
                        txtSYORI_KEN.Text = Format(OBJ_DATAREADER.Item("SYORI_KEN_S"), "#,##0")
                        '処理金額
                        txtSYORI_KIN.Text = Format(OBJ_DATAREADER.Item("SYORI_KIN_S"), "#,##0")
                        '手数料計算区分
                        txtTESUU_KBN.Text = OBJ_DATAREADER.Item("TESUU_KBN_S")
                        '振替済件数
                        txtFURI_KEN.Text = Format(OBJ_DATAREADER.Item("FURI_KEN_S"), "#,##0")
                        '振替済金額
                        txtFURI_KIN.Text = Format(OBJ_DATAREADER.Item("FURI_KIN_S"), "#,##0")
                        '手数料
                        txtTESUU_KIN.Text = Format(OBJ_DATAREADER.Item("TESUU_KIN_S"), "#,##0")
                        '不能件数
                        txtFUNOU_KEN.Text = Format(OBJ_DATAREADER.Item("FUNOU_KEN_S"), "#,##0")
                        '不能金額
                        txtFUNOU_KIN.Text = Format(OBJ_DATAREADER.Item("FUNOU_KIN_S"), "#,##0")

                        If IsDBNull(OBJ_DATAREADER.Item("FILE_NAME_T")) = False Then
                            '送信ファイル名
                            txtSousinFile.Text = OBJ_DATAREADER.Item("FILE_NAME_T")
                        Else
                            '送信ファイル名
                            txtSousinFile.Text = ""
                        End If

                        '作成日付
                        txtSAKUSEI_DATEY.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 1, 4)
                        txtSAKUSEI_DATEM.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 5, 2)
                        txtSAKUSEI_DATED.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 7, 2)

                        '再振種別設定 2006/10/23
                        STR再振種別 = OBJ_DATAREADER.Item("SFURI_SYUBETU_T")
                        ReDim iGakunen_Flag(0)  '初期化
                        For iGAK_CNT As Integer = 1 To 9
                            ReDim Preserve iGakunen_Flag(iGAK_CNT)
                            If OBJ_DATAREADER.Item("GAKUNEN" & iGAK_CNT & "_FLG_S") = "1" Then
                                iGakunen_Flag(iGAK_CNT) = 1
                            Else
                                iGakunen_Flag(iGAK_CNT) = 0
                            End If
                        Next
                    End While

                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Sub
                    End If

                Case Is > 1
                    If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                        Exit Sub
                    End If

                    Dim ICnt As Integer = 0

                    While (OBJ_DATAREADER.Read = True)

                        Dim str入出区分 As String = ""

                        Select Case OBJ_DATAREADER.Item("FURI_KBN_S")
                            Case "0"
                                str入出区分 = "初振"
                            Case "1"
                                str入出区分 = "再振"
                            Case "2"
                                str入出区分 = "入金"
                            Case "3"
                                str入出区分 = "出金"
                        End Select

                        'スケジュール区分
                        Str_Sch_Kbn = OBJ_DATAREADER.Item("SCH_KBN_S")

                        Select Case CInt(Str_Sch_Kbn)
                            Case 0
                                Str_Sch_Name = "年間スケジュール"
                            Case 1
                                Str_Sch_Name = "特別スケジュール"
                            Case 2
                                Str_Sch_Name = "随時スケジュール"
                        End Select

                        '確認メッセージ
                        'メッセージがYESの場合現在読み込み中のレコードを画面に設定する
                        'メッセージがNOの場合は次レコードを読み込む
                        If MessageBox.Show(String.Format(G_MSG0011I, Str_Sch_Name, _
                                                                     Mid(OBJ_DATAREADER.Item("NENGETUDO_S"), 1, 4) & "年" & Mid(OBJ_DATAREADER.Item("NENGETUDO_S"), 5, 2) & "月度", _
                                                                     str入出区分, _
                                                                     Mid(OBJ_DATAREADER.Item("FURI_DATE_S"), 1, 4) & "/" & Mid(OBJ_DATAREADER.Item("FURI_DATE_S"), 5, 2) & "/" & Mid(OBJ_DATAREADER.Item("FURI_DATE_S"), 7, 2), _
                                                                     Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 1, 4) & "/" & Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 5, 2) & "/" & Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 7, 2)), _
                                           msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                            Str_Sch_Kbn = OBJ_DATAREADER.Item("SCH_KBN_S")

                            '当該レコードの確定処理
                            '委託者コード
                            txtITAKU_CODE.Text = OBJ_DATAREADER.Item("ITAKU_CODE_S")
                            '再振替日
                            txtSFURI_DATENEN.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 1, 4)
                            txtSFURI_DATETUKI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 5, 2)
                            txtSFURI_DATEHI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 7, 2)

                            'エントリーデータ予定日
                            txtENTRI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 1, 4)
                            txtENTRI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 5, 2)
                            txtENTRI_YDATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 7, 2)
                            'エントリーデータ日
                            txtENTRI_DATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 1, 4)
                            txtENTRI_DATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 5, 2)
                            txtENTRI_DATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 7, 2)
                            'チェック予定日
                            txtCHECK_YDATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 1, 4)
                            txtCHECK_YDATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 5, 2)
                            txtCHECK_YDATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 7, 2)
                            'チェック日
                            txtCHECK_DATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 1, 4)
                            txtCHECK_DATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 5, 2)
                            txtCHECK_DATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 7, 2)
                            'データ作成予定日
                            txtDATA_YDATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 1, 4)
                            txtDATA_YDATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 5, 2)
                            txtDATA_YDATED.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 7, 2)
                            'データ作成日
                            txtDATA_DATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 1, 4)
                            txtDATA_DATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 5, 2)
                            txtDATA_DATED.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 7, 2)
                            '不能予定日
                            txtFUNOU_YDATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 1, 4)
                            txtFUNOU_YDATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 5, 2)
                            txtFUNOU_YDATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 7, 2)
                            '不能日
                            txtFUNOU_DATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 1, 4)
                            txtFUNOU_DATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 5, 2)
                            txtFUNOU_DATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 7, 2)
                            '資金決済予定日
                            txtKESSAI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 1, 4)
                            txtKESSAI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 5, 2)
                            txtKESSAI_YDATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 7, 2)
                            '資金決済日
                            txtKESSAI_DATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 1, 4)
                            txtKESSAI_DATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 5, 2)
                            txtKESSAI_DATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 7, 2)
                            '明細作成
                            txtENTRI_FLG.Text = OBJ_DATAREADER.Item("ENTRI_FLG_S")
                            '金額確認済
                            txtCHECK_FLG.Text = OBJ_DATAREADER.Item("CHECK_FLG_S")
                            '振替データ作成済
                            txtDATA_FLG.Text = OBJ_DATAREADER.Item("DATA_FLG_S")
                            '不能結果確認済
                            txtFUNOU_FLG.Text = OBJ_DATAREADER.Item("FUNOU_FLG_S")
                            '再振データ作成済
                            txtSAIFURI_FLG.Text = OBJ_DATAREADER.Item("SAIFURI_FLG_S")
                            '決済済
                            txtKESSAI_FLG.Text = OBJ_DATAREADER.Item("KESSAI_FLG_S")
                            '中断フラグ
                            txtTYUUDAN_FLG.Text = OBJ_DATAREADER.Item("TYUUDAN_FLG_S")
                            '処理件数
                            txtSYORI_KEN.Text = OBJ_DATAREADER.Item("SYORI_KEN_S")
                            '処理金額
                            txtSYORI_KIN.Text = OBJ_DATAREADER.Item("SYORI_KIN_S")
                            '手数料計算区分
                            txtTESUU_KBN.Text = OBJ_DATAREADER.Item("TESUU_KBN_S")
                            '振替済件数
                            txtFURI_KEN.Text = OBJ_DATAREADER.Item("FURI_KEN_S")
                            '振替済金額
                            txtFURI_KIN.Text = OBJ_DATAREADER.Item("FURI_KIN_S")
                            '手数料
                            txtTESUU_KIN.Text = OBJ_DATAREADER.Item("TESUU_KIN_S")
                            '不能件数
                            txtFUNOU_KEN.Text = OBJ_DATAREADER.Item("FUNOU_KEN_S")
                            '不能金額
                            txtFUNOU_KIN.Text = OBJ_DATAREADER.Item("FUNOU_KIN_S")

                            If IsDBNull(OBJ_DATAREADER.Item("FILE_NAME_T")) = False Then
                                '送信ファイル名
                                txtSousinFile.Text = OBJ_DATAREADER.Item("FILE_NAME_T")
                            Else
                                '送信ファイル名
                                txtSousinFile.Text = ""
                            End If
                            '作成日付
                            txtSAKUSEI_DATEY.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 1, 4)
                            txtSAKUSEI_DATEM.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 5, 2)
                            txtSAKUSEI_DATED.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 7, 2)

                            '再振種別設定 2006/10/23
                            STR再振種別 = OBJ_DATAREADER.Item("SFURI_SYUBETU_T")
                            Dim iGAK_CNT As Integer
                            ReDim iGakunen_Flag(0)  '初期化
                            For iGAK_CNT = 1 To 9
                                ReDim Preserve iGakunen_Flag(iGAK_CNT)
                                If OBJ_DATAREADER.Item("GAKUNEN" & iGAK_CNT & "_FLG_S") = "1" Then
                                    iGakunen_Flag(iGAK_CNT) = 1
                                Else
                                    iGakunen_Flag(iGAK_CNT) = 0
                                End If
                            Next
                            Exit While
                        End If

                        ICnt += 1

                    End While

                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Sub
                    End If
            End Select

            'キー項目の入力不可とする
            Call PSUB_KEY_Enabled(False)

            '振替日の年にフォーカス設定

            txtFURI_DATENEN.Focus()

            '入力ボタン制御
            btnKousin.Enabled = True
            btnDelete.Enabled = True
            btnSansyo.Enabled = False
            btnEraser.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '取消ボタン
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            Str_Sch_Kbn = ""
            '画面のクリア
            Call PSUB_GAMEN_CLEAR()

            'キー項目の入力可とする
            Call PSUB_KEY_Enabled(True)

            '学校コード入力にカーソル位置付け
            txtGAKKOU_CODE.Focus()

            '入力ボタン制御
            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnSansyo.Enabled = True
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
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        End Try
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_Get_Yotei_Date()

        'エントリーデータ予定日再計算
        '随時処理（入金、出金）は、振替日を設定
        Select Case GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN)
            Case "02", "03"
                STRエントリデータ予定日 = STR_FURIKAE_DATE(1)

                txtENTRI_YDATEY.Text = Mid(STRエントリデータ予定日, 1, 4)
                txtENTRI_YDATEM.Text = Mid(STRエントリデータ予定日, 5, 2)
                txtENTRI_YDATED.Text = Mid(STRエントリデータ予定日, 7, 2)
            Case Else
                STRエントリデータ予定日 = "00000000"

                txtENTRI_YDATEY.Text = "0000"
                txtENTRI_YDATEM.Text = "00"
                txtENTRI_YDATED.Text = "00"
        End Select

        'チェック予定日算出
        STRチェック予定日 = PFUNC_EIGYOUBI_GET(STR_FURIKAE_DATE(1), STR_JIFURI_CHECK, "-")
        If STRチェック予定日 <> "" Then
            txtCHECK_YDATEY.Text = Mid(STRチェック予定日, 1, 4)
            txtCHECK_YDATEM.Text = Mid(STRチェック予定日, 5, 2)
            txtCHECK_YDATED.Text = Mid(STRチェック予定日, 7, 2)
        Else
            txtCHECK_YDATEY.Text = ""
            txtCHECK_YDATEM.Text = ""
            txtCHECK_YDATED.Text = ""
        End If

        '振替データ作成予定日算出
        STR振替データ作成予定日 = PFUNC_EIGYOUBI_GET(STR_FURIKAE_DATE(1), STR_JIFURI_HAISIN, "-")
        If STR振替データ作成予定日 <> "" Then
            txtDATA_YDATEY.Text = Mid(STR振替データ作成予定日, 1, 4)
            txtDATA_YDATEM.Text = Mid(STR振替データ作成予定日, 5, 2)
            txtDATA_YDATED.Text = Mid(STR振替データ作成予定日, 7, 2)
        Else
            txtDATA_YDATEY.Text = ""
            txtDATA_YDATEM.Text = ""
            txtDATA_YDATED.Text = ""
        End If

        '不能結果更新予定日算出
        STR不能結果更新予定日 = PFUNC_EIGYOUBI_GET(STR_FURIKAE_DATE(1), STR_JIFURI_FUNOU, "+")
        If STR不能結果更新予定日 <> "" Then
            txtFUNOU_YDATEY.Text = Mid(STR不能結果更新予定日, 1, 4)
            txtFUNOU_YDATEM.Text = Mid(STR不能結果更新予定日, 5, 2)
            txtFUNOU_YDATED.Text = Mid(STR不能結果更新予定日, 7, 2)
        Else
            txtFUNOU_YDATEY.Text = ""
            txtFUNOU_YDATEM.Text = ""
            txtFUNOU_YDATED.Text = ""
        End If

        'タイムスタンプ
        Str_TimeStamp = Format(Now, "yyyyMMddHHmmss")

    End Sub
    Private Sub PSUB_GAMEN_CLEAR()

        '画面の表示項目クリア
        '委託者コード
        txtITAKU_CODE.Text = ""
        '再振替日
        txtSFURI_DATENEN.Text = ""
        txtSFURI_DATETUKI.Text = ""
        txtSFURI_DATEHI.Text = ""

        'エントリーデータ予定日
        txtENTRI_YDATEY.Text = ""
        txtENTRI_YDATEM.Text = ""
        txtENTRI_YDATED.Text = ""
        'エントリーデータ日
        txtENTRI_DATEY.Text = ""
        txtENTRI_DATEM.Text = ""
        txtENTRI_DATED.Text = ""
        'チェック予定日
        txtCHECK_YDATEY.Text = ""
        txtCHECK_YDATEM.Text = ""
        txtCHECK_YDATED.Text = ""
        'チェック日
        txtCHECK_DATEY.Text = ""
        txtCHECK_DATEM.Text = ""
        txtCHECK_DATED.Text = ""
        'データ作成予定日
        txtDATA_YDATEY.Text = ""
        txtDATA_YDATEM.Text = ""
        txtDATA_YDATED.Text = ""
        'データ作成日
        txtDATA_DATEY.Text = ""
        txtDATA_DATEM.Text = ""
        txtDATA_DATED.Text = ""
        '不能予定日
        txtFUNOU_YDATEY.Text = ""
        txtFUNOU_YDATEM.Text = ""
        txtFUNOU_YDATED.Text = ""
        '不能日
        txtFUNOU_DATEY.Text = ""
        txtFUNOU_DATEM.Text = ""
        txtFUNOU_DATED.Text = ""
        '資金決済予定日
        txtKESSAI_YDATEY.Text = ""
        txtKESSAI_YDATEM.Text = ""
        txtKESSAI_YDATED.Text = ""
        '資金決済日
        txtKESSAI_DATEY.Text = ""
        txtKESSAI_DATEM.Text = ""
        txtKESSAI_DATED.Text = ""
        '明細作成フラグ
        txtENTRI_FLG.Text = "0"
        '金額確認済フラグ
        txtCHECK_FLG.Text = "0"
        '振替データ作成済
        txtDATA_FLG.Text = "0"
        '不能結果確認済
        txtFUNOU_FLG.Text = "0"
        '再振データ作成済
        txtSAIFURI_FLG.Text = "0"
        '決済済
        txtKESSAI_FLG.Text = "0"
        '中断フラグ
        txtTYUUDAN_FLG.Text = "0"
        '処理件数
        txtSYORI_KEN.Text = ""
        '処理金額
        txtSYORI_KIN.Text = ""
        '手数料計算区分
        txtTESUU_KBN.Text = ""
        '振替済件数
        txtFURI_KEN.Text = ""
        '振替済金額
        txtFURI_KIN.Text = ""
        '手数料
        txtTESUU_KIN.Text = ""
        '不能件数
        txtFUNOU_KEN.Text = ""
        '不能金額
        txtFUNOU_KIN.Text = ""
        '送信ファイル名
        txtSousinFile.Text = ""
        '作成日付
        txtSAKUSEI_DATEY.Text = ""
        txtSAKUSEI_DATEM.Text = ""
        txtSAKUSEI_DATED.Text = ""

    End Sub
    Private Sub PSUB_KEY_Enabled(ByVal pValue As Boolean)

        'キー項目を使用可とする
        cmbKana.Enabled = pValue
        cmbGakkouName.Enabled = pValue

        txtFURI_DATENEN.Enabled = pValue
        txtFURI_DATETUKI.Enabled = pValue
        txtFURI_DATEHI.Enabled = pValue

        txtGAKKOU_CODE.Enabled = pValue
        txtTAISYONEN.Enabled = pValue
        txtTAISYOUTUKI.Enabled = pValue

        cmbFURI_KBN.Enabled = pValue

    End Sub
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If
        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text

        ' 2017/05/26 タスク）綾部 ADD 【ME】(RSV2対応 フォーカス設定) -------------------- START
        txtTAISYONEN.Focus()
        ' 2017/05/26 タスク）綾部 ADD 【ME】(RSV2対応 フォーカス設定) -------------------- END

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校名の取得
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 学校コードゼロ埋め
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '学校名の取得
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If

    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtENTRI_FLG.Validating, _
    txtCHECK_FLG.Validating, _
    txtDATA_FLG.Validating, _
    txtFUNOU_FLG.Validating, _
    txtSAIFURI_FLG.Validating, _
    txtKESSAI_FLG.Validating, _
    txtTYUUDAN_FLG.Validating

        If CType(sender, TextBox).Text.Trim = "" Then
            CType(sender, TextBox).Text = "0"
        End If

    End Sub
    Private Sub txtTYUUDAN_FLG_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTYUUDAN_FLG.Validating
        If txtTYUUDAN_FLG.Text.Length = 1 Then
            If btnSansyo.Enabled = True Then
                btnSansyo.Focus()
            ElseIf btnKousin.Enabled = True Then
                btnKousin.Focus()
            End If
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check(ByVal pIndex As Integer) As Boolean

        PFUNC_Nyuryoku_Check = False

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '学校マスタ存在チェック
            STR_SQL = "SELECT *"
            STR_SQL += " FROM GAKMAST2"
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()

                Exit Function
            End If
        End If

        '対象年
        If Trim(txtTAISYONEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYONEN.Focus()
            Exit Function
            'Else
            '    If CStr(CLng(txtTAISYONEN.Text)).Length <> 4 Then
            '        MessageBox.Show("指定した対象年が間違っています", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtTAISYONEN.Focus()

            '        Exit Function
            '    End If
        End If

        '対象月
        If Trim(txtTAISYOUTUKI.Text) = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYOUTUKI.Focus()
            Exit Function
        Else
            Select Case CInt(txtTAISYOUTUKI.Text)
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTAISYOUTUKI.Focus()

                    Exit Function
            End Select
        End If

        Str_Nentuki = txtTAISYONEN.Text & Format(CInt(txtTAISYOUTUKI.Text), "00")

        '振替日
        If Trim(txtFURI_DATENEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURI_DATENEN.Focus()

            Exit Function
            'Else
            '    If CStr(CLng(txtFURI_DATENEN.Text)).Trim.Length <> 4 Then
            '        MessageBox.Show("指定した振替日(年)が間違っています", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFURI_DATENEN.Focus()

            '        Exit Function
            '    End If
        End If

        If Trim(txtFURI_DATETUKI.Text) = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURI_DATETUKI.Focus()

            Exit Function
        Else
            Select Case CInt(txtFURI_DATETUKI.Text)
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFURI_DATETUKI.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFURI_DATEHI.Text) = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURI_DATEHI.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtFURI_DATENEN.Text) & "/" & Trim(txtFURI_DATETUKI.Text) & "/" & Trim(txtFURI_DATEHI.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFURI_DATENEN.Text) & Format(CInt(txtFURI_DATETUKI.Text), "00") & Format(CInt(txtFURI_DATEHI.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            txtFURI_DATENEN.Focus()

            Exit Function
        End If

        Select Case pIndex
            Case 0
                '資金決済予定日
                If Trim(txtKESSAI_YDATEY.Text) = "" Then
                    MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKESSAI_YDATEY.Focus()

                    Exit Function
                    'Else
                    '    If CStr(CLng(txtKESSAI_YDATEY.Text)).Trim.Length <> 4 Then
                    '        MessageBox.Show("指定した資金決済予定日(年)が間違っています", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '        txtKESSAI_YDATEY.Focus()

                    '        Exit Function
                    '    End If

                End If

                If Trim(txtKESSAI_YDATEM.Text) = "" Then
                    MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKESSAI_YDATEM.Focus()

                    Exit Function
                Else
                    Select Case CInt(txtKESSAI_YDATEM.Text)
                        Case 1 To 12
                        Case Else
                            MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtKESSAI_YDATEM.Focus()

                            Exit Function
                    End Select
                End If

                If Trim(txtKESSAI_YDATED.Text) = "" Then
                    MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKESSAI_YDATED.Focus()

                    Exit Function
                End If

                Str_Kessai_Yotei = Trim(txtKESSAI_YDATEY.Text) & "/" & Trim(txtKESSAI_YDATEM.Text) & "/" & Trim(txtKESSAI_YDATED.Text)

                If Not IsDate(Str_Kessai_Yotei) Then
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    txtKESSAI_YDATEY.Focus()

                    Exit Function
                End If

                Str_Kessai_Yotei = Trim(txtKESSAI_YDATEY.Text) & Format(CInt(txtKESSAI_YDATEM.Text), "00") & Format(CInt(txtKESSAI_YDATED.Text), "00")

                '明細作成フラグ
                Select Case Trim(txtENTRI_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "明細作成フラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '金額チェックフラグ
                Select Case Trim(txtCHECK_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "金額チェックフラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '振替データフラグ
                Select Case Trim(txtDATA_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "振替データフラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '不能フラグ
                Select Case Trim(txtFUNOU_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "不能フラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '再振フラグ
                Select Case Trim(txtSAIFURI_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "再振フラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '決済フラグ
                Select Case Trim(txtKESSAI_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "決済フラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '中断フラグ
                Select Case Trim(txtTYUUDAN_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "中断フラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select
        End Select

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_EIGYOUBI_GET(ByVal str年月日 As String, _
                                        ByVal str日数 As String, _
                                        ByVal str前後営業日区分 As String) As String

        '営業日算出
        Dim WORK_DATE As Date
        Dim YOUBI As Long
        Dim HOSEI As Integer
        'Dim FLG As Integer
        Dim int日数 As Integer



        PFUNC_EIGYOUBI_GET = ""

        int日数 = CInt(str日数)
        'Debug.WriteLine("int日数 =" & int日数)

        '-------------------------------------
        '月末補正（月末指定の場合実日に変換する）
        '-------------------------------------
        Select Case Mid(str年月日, 5, 2)
            Case "01", "03", "05", "07", "08", "10", "12"
                If Mid(str年月日, 7, 2) < "01" Then
                    Mid(str年月日, 7, 2) = "01"
                End If
                If Mid(str年月日, 7, 2) > "31" Then
                    Mid(str年月日, 7, 2) = "31"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str年月日, 1, 4)), CInt(Mid(str年月日, 5, 2)), CInt(Mid(str年月日, 7, 2)))
            Case "04", "06", "09", "11"
                If Mid(str年月日, 7, 2) < "01" Then
                    Mid(str年月日, 7, 2) = "01"
                End If
                If Mid(str年月日, 7, 2) > "30" Then
                    Mid(str年月日, 7, 2) = "30"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str年月日, 1, 4)), CInt(Mid(str年月日, 5, 2)), CInt(Mid(str年月日, 7, 2)))
            Case "02"
                If Mid(str年月日, 7, 2) < "01" Then
                    Mid(str年月日, 7, 2) = "01"
                End If
                '２月２９日,２月３０日,２月３１日は２月末日指定扱いで２月末日（実日に変換）
                If Mid(str年月日, 7, 2) > "28" Then
                    '１月末の実日で日付型データ変換
                    WORK_DATE = Mid(str年月日, 1, 4) & "/" & "01" & "/" & "31"
                    '２月末の実日を算出
                    WORK_DATE = DateAdd(DateInterval.Month, 1, WORK_DATE)
                Else
                    '２月末日以外の日付変換
                    WORK_DATE = DateSerial(CInt(Mid(str年月日, 1, 4)), CInt(Mid(str年月日, 5, 2)), CInt(Mid(str年月日, 7, 2)))
                End If
        End Select


        'Debug.WriteLine("WORK_DATE =" & WORK_DATE)



        '------------
        '０営業日対応
        '------------
        If int日数 = 0 Then
            YOUBI = Weekday(WORK_DATE)

            '曜日判定(Sun = 1:Sat = 7)
            If YOUBI = 1 Or YOUBI = 7 Or PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = False Then
                HOSEI = 1
            Else
                HOSEI = 0
            End If

            Do Until HOSEI = 0
                If str前後営業日区分 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str前後営業日区分 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If
                YOUBI = Weekday(WORK_DATE)

                '曜日判定(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        HOSEI = HOSEI - 1
                    End If
                End If
            Loop
        Else
            '-----------------
            '０営業日以外の処理
            '-----------------
            Do Until int日数 = 0
                If str前後営業日区分 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str前後営業日区分 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If

                'Debug.WriteLine("WORK_DATE3 =" & WORK_DATE)

                YOUBI = Weekday(WORK_DATE)

                'Debug.WriteLine("YOUBI =" & YOUBI)

                '曜日判定(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        int日数 = int日数 - 1
                    End If
                End If
            Loop
        End If

        PFUNC_EIGYOUBI_GET = Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")
        'PFUC_EIGYOUBI_GET = WORK_DATE


    End Function
    Private Function PFUNC_COMMON_YASUMIGET(ByVal str年月日 As String) As Boolean

        PFUNC_COMMON_YASUMIGET = False

        '休日マスタ存在チェック
        STR_SQL = " SELECT * FROM YASUMIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " YASUMI_DATE_Y ='" & str年月日 & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 5) = True Then
            Exit Function
        End If

        PFUNC_COMMON_YASUMIGET = True

    End Function
    Private Function PFUNC_GAKNAME_GET() As Boolean

        PFUNC_GAKNAME_GET = False

        '学校名の設定
        STR_SQL = " SELECT * FROM GAKMAST1"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            lab学校名.Text = ""
            txtGAKKOU_CODE.Focus()

            Exit Function
        End If

        OBJ_DATAREADER.Read()
        lab学校名.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G")

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_ZENMEISAI_UPD() As Boolean
        Dim bLoopFlg As Boolean = False '指定学年を条件に追加したかチェック
        Dim iLcount As Integer '指定学年ループ数

        PFUNC_ZENMEISAI_UPD = False

        '前回明細マスタ更新（再振済フラグ）

        '検索キーは、学校コード、振替区分、振替結果コード、再振済フラグで、振替日の降順

        STR_SQL = " SELECT * FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND"
        'STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
        'STR_SQL += " FURIKETU_CODE_M <> 0"
        STR_SQL += " FURIKETU_CODE_M IN (" & SFuriCode & ")"
        ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
        STR_SQL += " AND"
        STR_SQL += " SAIFURI_SUMI_M ='1'"
        STR_SQL += " ORDER BY FURI_DATE_M desc"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.Read() = True Then

            '初振の取消
            Select Case STR選択振替区分
                Case "0"
                    Select Case STR再振種別
                        Case "1", "2"
                            '再振ありの場合で取得した明細が初振の明細の場合
                            '前回の再振で振込が成功しているので明細マスタの更新は行わない
                            If OBJ_DATAREADER_DREAD.Item("FURI_KBN_M") = 0 Then
                                If GFUNC_SELECT_SQL3("", 1) = False Then
                                    Exit Function
                                End If
                                PFUNC_ZENMEISAI_UPD = True

                                Exit Function
                            End If
                    End Select
            End Select

            STR前回振替日 = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M")

            '追加 2006/10/06
            If STR_FURIKAE_DATE(1) = STR前回振替日 Then
                While OBJ_DATAREADER_DREAD.Read()
                    STR前回振替日 = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M")
                    If STR_FURIKAE_DATE(1) <> STR前回振替日 Then
                        Exit While
                    End If
                End While
            End If

        Else
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If
            PFUNC_ZENMEISAI_UPD = True

            Exit Function
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            '更新処理エラー
            MessageBox.Show(String.Format(MSG0002E, "参照"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If


        '前回振替日と同一日付の明細マスタを更新する
        STR_SQL = " UPDATE  G_MEIMAST SET "
        '再振済フラグ
        STR_SQL += " SAIFURI_SUMI_M ='0'"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & STR前回振替日 & "' "
        'STR_SQL += " AND"
        'STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
        'STR_SQL += " FURIKETU_CODE_M <> 0"
        STR_SQL += " FURIKETU_CODE_M IN (" & SFuriCode & ")"
        ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
        STR_SQL += " AND"
        STR_SQL += " SAIFURI_SUMI_M ='1'"
        'If strSCH_KBN <> "0" Then '通常以外のスケジュール時は学年指定 2005/12/09
        STR_SQL += " AND ("
        For iLcount = 1 To 9
            If iGakunen_Flag(iLcount) = 1 Then
                If bLoopFlg = True Then
                    STR_SQL += " OR "
                End If
                STR_SQL += " GAKUNEN_CODE_M = " & iLcount
                bLoopFlg = True
            End If
        Next iLcount
        STR_SQL += " )"
        'End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 3) = False Then
                '更新処理エラー
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Function
            End If
            '更新処理エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        Else
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                '更新処理エラー
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Function
            End If

        End If

        PFUNC_ZENMEISAI_UPD = True

    End Function


#End Region

End Class

