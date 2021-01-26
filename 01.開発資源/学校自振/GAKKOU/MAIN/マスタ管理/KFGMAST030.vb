Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Public Class KFGMAST030

#Region " 共通変数定義 "
    Private Str_Spread1(14, 6) As String
    Private Str_Spread2(12, 14) As String

    '現在有効なタブ情報を設定
    Private Int_TabFlag As Integer
    Private tab_Ctrl(1) As TabPage

    Private Str_HName() As String

    Private Str_Work(,) As String
    Private blnEnter_Check As Boolean
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
    Private intMaxHimokuCnt As Integer = CInt(IIf(STR_HIMOKU_PTN = "1", 15, 10))
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST030", "費目マスタメンテナンス画面")
    Private Const msgTitle As String = "費目マスタメンテナンス画面(KFGMAST030)"
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "費目マスタメンテナンス画面"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            If fn_SetDataGridViewKamokuCmbFromText() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定")
                MessageBox.Show(String.Format(MSG0013E, "科目"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            TabControl1.TabPages(0).Enabled = False
            TabControl1.TabPages(1).Enabled = False

            '入力禁止制御
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")

                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Sub
            End If

            Int_TabFlag = 0

            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            DataGridView1.Rows.Add(intMaxHimokuCnt)
            'DataGridView1.Rows.Add(10)
            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
            DataGridView2.Rows.Add(12)
            '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
            If STR_HIMOKU_PTN = "1" Then
                '費目１１～１５が非表示のため表示する
                For i As Integer = 10 To intMaxHimokuCnt - 1
                    DataGridView2.Columns(i).Visible = True
                Next
            End If
            '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END

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
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")
            LW.ToriCode = txtGAKKOU_CODE.Text
            '基本項目入力値チェック
            '項目LostFocus時にもしているが
            '直接処理ボタンを押下したときの対処
            If PFUNC_Hisuu_Check() = False Then
                Exit Sub
            End If

            '重複チェック
            STR_SQL = " SELECT "
            STR_SQL += " GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H"
            STR_SQL += " FROM HIMOMAST"
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " AND HIMOKU_ID_H ='" & cmbHimoku.Text & "'"

            If GFUNC_ISEXIST(STR_SQL) = True Then
                MessageBox.Show(G_MSG0047W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If

            Select Case Int_TabFlag
                Case 0
                    '費目名決済口座タブ

                    '入力値チェック
                    If PFUNC_Nyuryoku_Check() = False Then
                        Exit Sub
                    End If

                    '確認メッセージ
                    If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If

                    'データ登録
                    Call PSUB_Spread1_Insert()

                    'データ登録後、一部入力内容をクリア
                    Call PSUB_Nyuuryoku_Clear(1)

                    'コンボ再作成
                    STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                    STR_SQL += " ORDER BY HIMOKU_ID_H "

                    Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)

                Case 1
                    '月別金額タブ
                    If Trim(txtHIMOKU_PATERN.Text) = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "費目名"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Focus()
                        Exit Sub
                    End If

                    '確認メッセージ
                    If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If

                    'データ登録
                    'トランザクションデータ操作開始
                    Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)
                    If PSUB_Spread2_Insert() = False Then
                        'ロールバック
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                    Else
                        'トランザクションデータ処理確定
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)
                        MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                    'データ登録後、一部入力内容をクリア
                    Call PSUB_Nyuuryoku_Clear(1)
                    Call PSUB_Nyuuryoku_Clear(4)

                    'コンボ再作成
                    STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                    STR_SQL += " ORDER BY HIMOKU_ID_H "

                    Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)
            End Select

            cmbHimoku.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            LW.ToriCode = txtGAKKOU_CODE.Text

            Select Case Int_TabFlag
                Case 0
                    '費目名決済口座タブ

                    '入力値チェック
                    If PFUNC_Nyuryoku_Check() = False Then
                        Exit Sub
                    End If

                    '確認メッセージ
                    If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If

                    'データ更新
                    Call PSUB_Spread_Update()
                Case 1
                    '月別金額タブ
                    If Trim(txtHIMOKU_PATERN.Text) = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "費目名"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Focus()
                        Exit Sub
                    End If
                    '確認メッセージ
                    If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If
                    If Trim(txtHIMOKU_PATERN.Text) = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "費目名"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Focus()
                        Exit Sub
                    End If

                    'トランザクションデータ操作開始
                    Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)
                    'データ削除
                    If PSUB_Spread_Delete() = False Then
                        'ロールバック
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                        Return
                    End If
                    'データ登録
                    If PSUB_Spread2_Insert() = False Then
                        'ロールバック
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                        Return
                    End If
                    'トランザクションデータ処理確定
                    Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    'コンボ再作成
                    STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                    STR_SQL += " ORDER BY HIMOKU_ID_H "

                    Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)
            End Select
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")
            LW.ToriCode = txtGAKKOU_CODE.Text

            If PFUNC_Hisuu_Check() = False Then
                Exit Sub
            End If

            '確認メッセージ
            Select Case Int_TabFlag
                Case 0
                    If MessageBox.Show(G_MSG0017I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If
                Case 1
                    If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If
            End Select

            '2007/02/12　必須項目チェック
            If cmbHimoku.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "費目ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbHimoku.Focus()
                Exit Sub
            End If

            'トランザクションデータ操作開始
            Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)
            'データ削除
            If PSUB_Spread_Delete() = False Then
                'ロールバック
                Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                Return
            End If
            'トランザクションデータ処理確定
            Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)

            'コンボ再作成
            STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
            STR_SQL += " ORDER BY HIMOKU_ID_H "

            Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)

            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            'データ削除後、入力内容をクリア
            Call PSUB_Nyuuryoku_Clear(0)
            txtGAKUNEN_CODE.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")
            LW.ToriCode = txtGAKKOU_CODE.Text

            Dim Str_Sql_local As String

            '入力値チェック
            If PFUNC_Hisuu_Check() = False Then

                Exit Sub
            End If

            '存在チェック
            STR_SQL = " SELECT *"
            STR_SQL += " FROM HIMOMAST"
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " AND HIMOKU_ID_H = '" & cmbHimoku.Text & "'"

            Select Case cmbHimoku.Text
                Case "000"
                    Int_TabFlag = 0
                    '費目名決済口座
                    Call PFUNC_SET_SPDDATA(STR_SQL)

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        'データ登録後、一部入力内容をクリア
                        Call PSUB_Nyuuryoku_Clear(2)
                    Else
                        Call PSUB_Nyuuryoku_Clear(3)
                    End If

                    Me.DataGridView1.Focus()
                Case Else
                    '月別金額
                    Int_TabFlag = 1

                    For intRow As Integer = 0 To 11
                        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                        For intCol As Integer = 0 To intMaxHimokuCnt - 1
                            'For intCol As Integer = 0 To 9 '費目は１０まで
                            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                            DataGridView2.Rows(intRow).Cells(intCol).Value = ""
                            DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                            DataGridView2.Columns(intCol).HeaderText = "費目" & StrConv(CStr(intCol + 1), VbStrConv.Wide)
                        Next intCol
                    Next intRow

                    Str_Sql_local = " SELECT * "
                    Str_Sql_local += " FROM HIMOMAST"
                    Str_Sql_local += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    Str_Sql_local += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    Str_Sql_local += " AND HIMOKU_ID_H = '000'"

                    Call PFUNC_SET_SPDDATA(Str_Sql_local)

                    Call PFUNC_SET_SPDDATA2(STR_SQL)

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        Call PSUB_Nyuuryoku_Clear(5)
                    Else
                        Call PSUB_Nyuuryoku_Clear(6)
                    End If

                    DataGridView1.Focus()
            End Select
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try

    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")
            Call PSUB_Nyuuryoku_Clear(0)
            '2007/02/12　画面クリア後、学校コードにフォーカスをあてる
            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)開始", "成功", "")

            Me.Close()
        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)例外エラー", "失敗", ex.Message)

        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim OraDB As New MyOracle
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '2012/04/13 saitou 標準修正 ADD ---------------------------------------->>>>
            '各項目の入力チェックを追加
            If Me.txtGAKKOU_CODE.Text.Trim = String.Empty Then
                '学校コードが空白ならば、学年コードと費目IDも空白であること
                If Me.txtGAKUNEN_CODE.Text.Trim <> String.Empty Then
                    MessageBox.Show(G_MSG0048W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtGAKKOU_CODE.Focus()
                    Return
                End If
                If Me.cmbHimoku.Text.Trim <> String.Empty Then
                    MessageBox.Show(G_MSG0049W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtGAKKOU_CODE.Focus()
                    Return
                End If
            End If
            '2012/04/13 saitou 標準修正 ADD ----------------------------------------<<<<

            '学校コード
            If Trim(txtGAKKOU_CODE.Text) <> "" Then

                '学校マスタ存在チェック
                STR_SQL = " SELECT GAKKOU_CODE_G "
                STR_SQL += " FROM GAKMAST1"
                STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

                If GFUNC_ISEXIST(STR_SQL) = False Then

                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    lblGAKKOU_NAME.Text = "" '2007/02/14
                    Exit Sub
                End If
            End If

            '学年
            If Trim(txtGAKUNEN_CODE.Text) <> "" Then

                Select Case CInt(txtGAKUNEN_CODE.Text)
                    Case 0
                        MessageBox.Show(G_MSG0050W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKUNEN_CODE.Focus()

                        Exit Sub
                End Select
                Dim sSiyou_Gakunen As String
                '使用学年チェック
                STR_SQL = " SELECT SIYOU_GAKUNEN_T"
                STR_SQL += " FROM GAKMAST2"
                STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

                sSiyou_Gakunen = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SIYOU_GAKUNEN_T")

                If Trim(sSiyou_Gakunen) = "" AndAlso Trim(txtGAKKOU_CODE.Text) <> "" Then
                    MessageBox.Show(G_MSG0051W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()
                    Exit Sub
                End If

                Select Case CInt(sSiyou_Gakunen)
                    Case Is < CInt(txtGAKUNEN_CODE.Text)
                        '使用最高学年以上の学年を入力
                        MessageBox.Show(G_MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKUNEN_CODE.Focus()
                        Exit Sub
                End Select
            End If

            '費目ID
            If Trim(cmbHimoku.Text) <> "" Then
                '費目ID指定時に学年が空白の場合はエラーとする 2007/02/14
                If Trim(txtGAKUNEN_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "学年"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()

                    Exit Sub
                End If
            End If

            '印刷用ワークテーブル作成
            If PFUNC_Print_Work() = False Then
                Exit Sub
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            Dim nRet As Integer
            Dim Param As String

            OraReader = New MyOracleReader(OraDB)

            SQL.Append("SELECT DISTINCT GAKKOU_CODE_H FROM MAST0300G_WORK")

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "参照"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            If MessageBox.Show(String.Format(MSG0013I, "費目マスタ一覧"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            While OraReader.EOF = False
                Param = GCom.GetUserID & "," & OraReader.GetString("GAKKOU_CODE_H")
                nRet = ExeRepo.ExecReport("KFGP010.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "費目マスタ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                OraReader.NextRead()
            End While

            MessageBox.Show(String.Format(MSG0014I, "費目マスタ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            btnPrint.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Sub
#End Region


    Private Sub PSUB_Spread1_Insert()

        With DataGridView1
            STR_SQL = " INSERT INTO HIMOMAST VALUES ("
            STR_SQL += "'" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += "," & txtGAKUNEN_CODE.Text
            '費目名・決済口座の登録は費目コード＝『000』、費目名『決済口座』固定
            STR_SQL += ",'" & cmbHimoku.Text & "'"
            STR_SQL += ",'" & txtHIMOKU_PATERN.Text & "'"
            '請求月は暫定で『  』を設定
            STR_SQL += ",'" & Space(2) & "'"

            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            For intCnt As Integer = 0 To intMaxHimokuCnt - 1
                'For intCnt As Integer = 0 To 9 '2007/04/04　費目は１０まで
                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                '費目名が入力されていなければ次の行の入力値チェックを行う
                If IsDBNull(Trim(.Rows(intCnt).Cells(0).Value)) = False And Trim(.Rows(intCnt).Cells(0).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(0).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(2).Value)) = False And Trim(.Rows(intCnt).Cells(2).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(2).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(3).Value)) = False And Trim(.Rows(intCnt).Cells(3).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(3).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If

                If IsDBNull(.Rows(intCnt).Cells(4).Value) = False And Trim(.Rows(intCnt).Cells(4).Value) <> "" Then
                    STR_SQL += ",'" & fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If

                If IsDBNull(Trim(.Rows(intCnt).Cells(5).Value)) = False And Trim(.Rows(intCnt).Cells(5).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(5).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(6).Value)) = False And Trim(.Rows(intCnt).Cells(6).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(6).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(1).Value)) = False And Trim(.Rows(intCnt).Cells(1).Value) <> "" Then
                    If Trim(.Rows(intCnt).Cells(1).Value) <> "" Then
                        STR_SQL += "," & Format(CLng(Trim(.Rows(intCnt).Cells(1).Value)), "###0")
                    Else
                        STR_SQL += ",0"
                    End If
                Else
                    STR_SQL += ",0"
                End If
            Next intCnt

            '2017/04/06 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            For intCnt As Integer = intMaxHimokuCnt To 14
                STR_SQL += ",'','','','','','',0"
            Next
            ''2007/04/04　費目11～15の分は空欄・0で入力する
            'For intCnt As Integer = 0 To 4
            '    STR_SQL += ",'','','','','','',0"
            'Next
            '2017/04/06 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END

            STR_SQL += ",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'"
            STR_SQL += ",'00000000'"
            '予備項目追加
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ")"
        End With

        'トランザクションデータ操作開始
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)

        'トランザクションデータ処理実行
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

            'データ処理中エラー
            MessageBox.Show(String.Format(MSG0002E, "挿入"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        'トランザクションデータ処理確定
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)

        MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)


    End Sub
    Private Function PSUB_Spread2_Insert() As Boolean
        PSUB_Spread2_Insert = False
        With DataGridView2
            For intRow As Integer = 0 To 11
                STR_SQL = " INSERT INTO HIMOMAST VALUES ("
                STR_SQL += "'" & txtGAKKOU_CODE.Text & "'"
                STR_SQL += "," & txtGAKUNEN_CODE.Text
                STR_SQL += ",'" & cmbHimoku.Text & "'"
                STR_SQL += ",'" & txtHIMOKU_PATERN.Text & "'"
                Select Case intRow
                    Case 0 To 8
                        STR_SQL += ",'" & Format((intRow + 4), "00") & "'"
                    Case 9 To 11
                        STR_SQL += ",'" & Format((intRow - 8), "00") & "'"
                End Select

                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                For intCol As Integer = 0 To intMaxHimokuCnt - 1
                    'For intCol As Integer = 0 To 9 '2007/04/04　費目は１０まで
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                    If IsDBNull(Trim(.Rows(intRow).Cells(intCol).Value)) = False Then
                        '費目『000』の情報を設定する（金額以外）
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 0)) & "'"
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 2)) & "'"
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 3)) & "'"
                        '2014/05/15 saitou 標準修正 MODIFY ----------------------------------------------->>>>
                        If Trim(Str_Spread1(intCol, 4)) = "0" Then
                            '科目0は格納しない
                            STR_SQL += ",''"
                        Else
                            STR_SQL += ",'" & Trim(Str_Spread1(intCol, 4)) & "'"
                        End If
                        'STR_SQL += ",'" & Trim(Str_Spread1(intCol, 4)) & "'"
                        '2014/05/15 saitou 標準修正 MODIFY -----------------------------------------------<<<<
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 5)) & "'"
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 6)) & "'"
                        '金額に値がない場合はゼロを設定する
                        If Trim(.Rows(intRow).Cells(intCol).Value) <> "" Then
                            STR_SQL += "," & Format(CLng(Trim(.Rows(intRow).Cells(intCol).Value)), "###0")
                        Else
                            STR_SQL += ",0"
                        End If
                    Else
                        STR_SQL += ",'','','','','','',0"
                    End If
                Next

                '2017/04/06 タスク）西野 UPD 標準版修正（費目数１０⇒１５）------------------------------------ START
                For intCol As Integer = intMaxHimokuCnt To 14
                    STR_SQL += ",'','','','','','',0"
                Next
                ''2007/04/04　費目11～15の分は空欄・0で入力する
                'For intCol As Integer = 0 To 4
                '    STR_SQL += ",'','','','','','',0"
                'Next
                '2017/04/06 タスク）西野 UPD 標準版修正（費目数１０⇒１５）------------------------------------ END

                STR_SQL += ",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'"
                STR_SQL += ",'00000000'"
                '予備項目追加
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ")"

                'トランザクションデータ処理実行
                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

                    'データ処理中エラー
                    MessageBox.Show(String.Format(MSG0002E, "挿入"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Exit Function
                End If
            Next
        End With
        PSUB_Spread2_Insert = True

    End Function
    Private Function PSUB_Spread_Delete() As Boolean
        PSUB_Spread_Delete = False
        '削除処理

        '確認メッセージ

        'SQL文作成
        '画面に設定されている学校コード、学年コード、費目IDを条件に費目マスタを削除する
        STR_SQL = " DELETE  FROM HIMOMAST"
        STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"

        '費目名・決済口座ｽﾌﾟﾚｯﾄの削除時は、費目『000』以外のﾚｺｰﾄﾞも削除対象とする為
        '学校ｺｰﾄﾞ、学年ｺｰﾄﾞのみで削除をかける
        Select Case Int_TabFlag
            Case 1
                STR_SQL += " AND HIMOKU_ID_H = '" & cmbHimoku.Text & "'"
        End Select

        'トランザクションデータ処理実行
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

            'データ処理中エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        '完了メッセージ

        PSUB_Spread_Delete = True
    End Function
    Private Sub PSUB_Spread_Update()

        Dim Str_Sql1 As String
        Dim Str_Sql2 As String

        With Me.DataGridView1
            Str_Sql1 = ""
            Str_Sql2 = ""

            STR_SQL = " UPDATE  HIMOMAST SET "
            STR_SQL += " KOUSIN_DATE_H='" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'"

            Str_Sql1 += STR_SQL
            Str_Sql2 += STR_SQL

            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            For intCnt As Integer = 0 To intMaxHimokuCnt - 1
                'For intCnt As Integer = 0 To 9 '2007/04/04　費目は１０まで
                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                '費目名が入力されていなければ次の行の入力値チェックを行う
                '修正前と修正後で変更がある場合にのみ修正
                If IsDBNull(Trim(.Rows(intCnt).Cells(0).Value)) = False Then
                    If Trim(.Rows(intCnt).Cells(0).Value) <> "" Then
                        STR_SQL = ""
                        If Trim(.Rows(intCnt).Cells(0).Value) <> Trim(Str_Spread1(intCnt, 0)) Then
                            STR_SQL += ",HIMOKU_NAME" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(0).Value) & "'"
                            Str_Spread1(intCnt, 0) = Trim(.Rows(intCnt).Cells(0).Value)
                        End If
                        If Trim(.Rows(intCnt).Cells(2).Value) <> Trim(Str_Spread1(intCnt, 2)) Then
                            STR_SQL += ",KESSAI_KIN_CODE" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(2).Value) & "'"
                            Str_Spread1(intCnt, 2) = Trim(.Rows(intCnt).Cells(2).Value)
                        End If
                        If Trim(.Rows(intCnt).Cells(3).Value) <> Trim(Str_Spread1(intCnt, 3)) Then
                            STR_SQL += ",KESSAI_TENPO" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(3).Value) & "'"
                            Str_Spread1(intCnt, 3) = Trim(.Rows(intCnt).Cells(3).Value)
                        End If

                        If fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value) <> Trim(Str_Spread1(intCnt, 4)) Then

                            STR_SQL += ",KESSAI_KAMOKU" & Format((intCnt + 1), "00") & "_H ='" & fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value) & "'"

                            Str_Spread1(intCnt, 4) = CStr(GFUNC_NAME_TO_CODE2(STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT, Trim(.Rows(intCnt).Cells(4).Value)))
                        End If
                        If Trim(.Rows(intCnt).Cells(5).Value) <> Trim(Str_Spread1(intCnt, 5)) Then
                            STR_SQL += ",KESSAI_KOUZA" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(5).Value) & "'"
                            Str_Spread1(intCnt, 5) = Trim(.Rows(intCnt).Cells(5).Value)
                        End If
                        If Trim(.Rows(intCnt).Cells(6).Value) <> Trim(Str_Spread1(intCnt, 6)) Then
                            STR_SQL += ",KESSAI_MEIGI" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(6).Value) & "'"
                            Str_Spread1(intCnt, 6) = Trim(.Rows(intCnt).Cells(6).Value)
                        End If

                        '2006/10/24　金額欄空白対策
                        If Trim(.Rows(intCnt).Cells(1).Value) <> "" Then
                            Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =" & Format(CLng(Trim(.Rows(intCnt).Cells(1).Value)), "###0")
                        Else
                            Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =" & "0"
                        End If

                        Str_Sql2 += STR_SQL
                    Else
                        If Trim(Str_Spread1(intCnt, 0)) <> "" Then
                            STR_SQL = ",HIMOKU_NAME" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_KIN_CODE" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_TENPO" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_KAMOKU" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_KOUZA" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_MEIGI" & Format((intCnt + 1), "00") & "_H =''"

                            Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =0"
                            Str_Sql2 += STR_SQL
                        End If
                    End If
                Else
                    If Trim(Str_Spread1(intCnt, 0)) <> "" Then
                        STR_SQL = ",HIMOKU_NAME" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_KIN_CODE" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_TENPO" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_KAMOKU" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_KOUZA" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_MEIGI" & Format((intCnt + 1), "00") & "_H =''"

                        Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =0"
                        Str_Sql2 += STR_SQL
                    End If
                End If
            Next intCnt
            STR_SQL = " WHERE "
            STR_SQL += " GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H =" & txtGAKUNEN_CODE.Text

            Str_Sql1 += STR_SQL & " AND HIMOKU_ID_H ='" & cmbHimoku.Text & "'"
            Str_Sql2 += STR_SQL
            '            STR_SQL += " AND TUKI_NO_H ='" & Space(2) & "'"
        End With

        'トランザクションデータ操作開始
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)

        'トランザクションデータ処理実行
        If GFUNC_EXECUTESQL_TRANS(Str_Sql1, 1) = False Then

            'データ処理中エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'ロールバック
            Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(Str_Sql2, 1) = False Then

            'データ処理中エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Sub
        End If

        'トランザクションデータ処理確定
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)

        MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)


    End Sub
    Private Sub PSUB_Nyuuryoku_Clear(ByVal pIndex As Integer)
        '検索用学校名ｺﾝﾎﾞﾎﾞｯｸｽ
        Select Case pIndex
            Case 0
                '未選択状態
                cmbGAKKOUNAME.SelectedIndex = -1
                cmbGAKKOUNAME.Enabled = True
                cmbKana.Enabled = True
            Case 5, 6
                cmbGAKKOUNAME.Enabled = False
                cmbKana.Enabled = False
        End Select

        '2007/02/12　学校コード・学年コードは初期化しない

        '学校コードﾃｷｽﾄﾎﾞｯｸｽ+学校名ラベル
        Select Case pIndex
            Case 0
                txtGAKKOU_CODE.Enabled = True
            Case 5, 6
                txtGAKKOU_CODE.Enabled = False
        End Select

        '学年ﾃｷｽﾄﾎﾞｯｸｽ
        Select Case pIndex
            Case 0
                txtGAKUNEN_CODE.Enabled = True
            Case 5, 6
                txtGAKUNEN_CODE.Enabled = False
        End Select

        '費目コードｺﾝﾎﾞﾎﾞｯｸｽ+費目名ﾃｷｽﾄﾎﾞｯｸｽ
        Select Case pIndex
            Case 0, 1, 4
                cmbHimoku.Enabled = True
                cmbHimoku.Text = ""
                cmbHimoku.Items.Clear()
                ReDim Str_HName(0)

                txtHIMOKU_PATERN.Enabled = True
                txtHIMOKU_PATERN.Text = ""
            Case 2, 3
                txtHIMOKU_PATERN.Enabled = False
            Case 5
                txtGAKUNEN_CODE.Enabled = False
                txtHIMOKU_PATERN.Enabled = True
        End Select

        '費目名・決済口座ｽﾌﾟﾚｯﾄ
        Select Case pIndex
            Case 0, 1
                TabControl1.SelectedIndex = 0
                TabControl1.TabPages(0).Enabled = False

                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '2007/04/04　費目は１０まで
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                    For intCol As Integer = 0 To 6
                        DataGridView1.Rows(intRow).Cells(intCol).Value = ""
                        DataGridView1.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow
            Case 2, 3
                TabControl1.SelectedIndex = 0

                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '2007/04/04　費目は１０まで
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                    For intCol As Integer = 0 To 6
                        DataGridView1.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow


                TabControl1.TabPages(0).Enabled = True
            Case 5, 6

                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '2007/04/04　費目は１０まで
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                    For intCol As Integer = 0 To 6
                        DataGridView1.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow

                TabControl1.TabPages(0).Enabled = False
        End Select

        '月別金額ｽﾌﾟﾚｯﾄ
        Select Case pIndex
            Case 0, 4
                TabControl1.TabPages(1).Enabled = False
                For intRow As Integer = 0 To 11
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                    For intCol As Integer = 0 To intMaxHimokuCnt - 1
                        'For intCol As Integer = 0 To 9 '2007/04/04　費目は１０まで
                        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                        DataGridView2.Rows(intRow).Cells(intCol).Value = ""
                        DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                        DataGridView2.Columns(intCol).HeaderText = "費目" & StrConv(CStr(intCol + 1), VbStrConv.Wide)

                    Next intCol
                Next intRow
            Case 2, 3
                For intRow As Integer = 0 To 11
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                    For intCol As Integer = 0 To intMaxHimokuCnt - 1
                        'For intCol As Integer = 0 To 9 '2007/04/04　費目は１０まで
                        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                        DataGridView2.Rows(intRow).Cells(intCol).Value = ""
                        DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                        DataGridView2.Columns(intCol).HeaderText = "費目" & StrConv(CStr(intCol + 1), VbStrConv.Wide)
                    Next intCol
                Next intRow

                TabControl1.TabPages(1).Enabled = False
            Case 5, 6
                TabControl1.SelectedIndex = 1

                For intRow As Integer = 0 To 11
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                    For intCol As Integer = 0 To intMaxHimokuCnt - 1
                        'For intCol As Integer = 0 To 9 '2007/04/04　費目は１０まで
                        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                        DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow

                TabControl1.TabPages(1).Enabled = True
        End Select

        '登録ボタン
        Select Case pIndex
            Case 0, 2, 5
                btnAdd.Enabled = True
            Case 3, 6
                btnAdd.Enabled = False
        End Select

        '修正・削除ボタン
        Select Case pIndex
            Case 0, 1, 2, 4, 5
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
            Case 3, 6
                btnUPDATE.Enabled = True
                btnDelete.Enabled = True
        End Select

    End Sub
    Private Function PFUNC_SET_SPDDATA(ByVal pSql As String) As Boolean

        PFUNC_SET_SPDDATA = False

        If Trim(pSql) = "" Then
            Exit Function
        End If

        If GFUNC_SELECT_SQL2(pSql, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        OBJ_DATAREADER.Read()

        ReDim Str_Spread1(14, 6)

        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        For intCnt As Integer = 1 To intMaxHimokuCnt
            'For intCnt As Integer = 1 To 10 '2007/04/04　費目は１０まで 
            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
            If IsDBNull(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCnt, "00") & "_H")) = True Then
                With DataGridView1
                    '費目名
                    .Rows(intCnt - 1).Cells(0).Value = ""
                    Str_Spread1((intCnt - 1), 0) = ""

                    '金額
                    .Rows(intCnt - 1).Cells(1).Value = ""
                    Str_Spread1((intCnt - 1), 1) = 0

                    '金融機関コード
                    .Rows(intCnt - 1).Cells(2).Value = ""
                    Str_Spread1((intCnt - 1), 2) = ""

                    '支店コード
                    .Rows(intCnt - 1).Cells(3).Value = ""
                    Str_Spread1((intCnt - 1), 3) = ""

                    '科目
                    .Rows(intCnt - 1).Cells(4).Value = ""
                    Str_Spread1((intCnt - 1), 4) = 0

                    '口座番号
                    .Rows(intCnt - 1).Cells(5).Value = ""
                    Str_Spread1((intCnt - 1), 5) = ""

                    '口座名義人(ｶﾅ)
                    .Rows(intCnt - 1).Cells(6).Value = ""
                    Str_Spread1((intCnt - 1), 6) = ""
                End With

            Else
                With DataGridView1
                    '費目名
                    Dim HimokaName As String = ConvNullToString(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCnt, "00") & "_H")).Trim
                    .Rows(intCnt - 1).Cells(0).Value = HimokaName
                    Str_Spread1((intCnt - 1), 0) = HimokaName
                    '金額
                    Dim Kingaku As String = ConvNullToString(OBJ_DATAREADER.Item("HIMOKU_KINGAKU" & Format(intCnt, "00") & "_H")).Trim
                    .Rows(intCnt - 1).Cells(1).Value = Format(CDbl(Kingaku), "#,##0")
                    Str_Spread1((intCnt - 1), 1) = Kingaku
                    Dim KinCode As String = ConvNullToString(OBJ_DATAREADER.Item("KESSAI_KIN_CODE" & Format(intCnt, "00") & "_H")).Trim
                    '金融機関コード
                    .Rows(intCnt - 1).Cells(2).Value = KinCode
                    Str_Spread1((intCnt - 1), 2) = KinCode
                    Dim SitCode As String = ConvNullToString(OBJ_DATAREADER.Item("KESSAI_TENPO" & Format(intCnt, "00") & "_H")).Trim
                    '支店コード
                    .Rows(intCnt - 1).Cells(3).Value = SitCode
                    Str_Spread1((intCnt - 1), 3) = SitCode

                    '科目
                    .Rows(intCnt - 1).Cells(4).Value = fn_GetKamokuNameFromKamokuCode(OBJ_DATAREADER.Item("KESSAI_KAMOKU" & Format(intCnt, "00") & "_H"))
                    Str_Spread1((intCnt - 1), 4) = CStr(OBJ_DATAREADER.Item("KESSAI_KAMOKU" & Format(intCnt, "00") & "_H"))
                    Dim Kouza As String = ConvNullToString(OBJ_DATAREADER.Item("KESSAI_KOUZA" & Format(intCnt, "00") & "_H")).Trim
                    '口座番号
                    .Rows(intCnt - 1).Cells(5).Value = Kouza
                    Str_Spread1((intCnt - 1), 5) = Kouza

                    '口座名義人(ｶﾅ)
                    If IsDBNull(OBJ_DATAREADER.Item("KESSAI_MEIGI" & Format(intCnt, "00") & "_H")) = False Then
                        .Rows(intCnt - 1).Cells(6).Value = Trim(CStr(OBJ_DATAREADER.Item("KESSAI_MEIGI" & Format(intCnt, "00") & "_H")))
                        Str_Spread1((intCnt - 1), 6) = Trim(CStr(OBJ_DATAREADER.Item("KESSAI_MEIGI" & Format(intCnt, "00") & "_H")))
                    Else
                        .Rows(intCnt - 1).Cells(6).Value = ""
                        Str_Spread1((intCnt - 1), 6) = ""
                    End If

                End With
            End If
        Next intCnt

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_SET_SPDDATA = True

    End Function
    Private Function PFUNC_SET_SPDDATA2(ByVal pSql As String) As Boolean
        Dim intCnt As Integer
        Dim intCol As Integer
        Dim intRow As Integer

        Dim intTuki As Integer

        PFUNC_SET_SPDDATA2 = False

        If Trim(pSql) = "" Then
            Exit Function
        End If

        If GFUNC_SELECT_SQL2(pSql, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            With DataGridView2
                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                For intCnt = 0 To intMaxHimokuCnt - 1
                    'For intCnt = 0 To 9 '2007/04/04　費目は１０まで
                    '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                    If Trim(Str_Spread1((intCnt), 0)) = "" Then
                        .Columns(intCnt).HeaderText = "費目" & StrConv(CStr(intCnt + 1), VbStrConv.Wide)
                        .Columns(intCnt).ReadOnly = True
                    Else
                        .Columns(intCnt).HeaderText = Trim(Str_Spread1((intCnt), 0))
                        .Columns(intCnt).ReadOnly = False
                        For intRow = 0 To 11
                            .Rows(intRow).Cells(intCnt).ReadOnly = False
                            If IsNumeric(CStr(Trim(Str_Spread1((intCnt), 1)))) Then
                                .Rows(intRow).Cells(intCnt).Value = Format(CDbl(Trim(Str_Spread1((intCnt), 1))), "#,##0")
                            Else
                                .Rows(intRow).Cells(intCnt).Value = CStr(Trim(Str_Spread1((intCnt), 1)))
                            End If
                        Next intRow
                    End If
                Next intCnt
            End With

            Exit Function
        End If

        With DataGridView2
            While (OBJ_DATAREADER.Read = True)
                If IsDBNull(OBJ_DATAREADER.Item("TUKI_NO_H")) = True Then
                    Exit While
                End If

                intTuki = CInt(OBJ_DATAREADER.Item("TUKI_NO_H"))

                Select Case intTuki
                    Case 1 To 3
                        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                        For intCol = 0 To intMaxHimokuCnt - 1
                            'For intCol = 0 To 9 '2007/04/04　費目は１０まで
                            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                            '費目名が入力されていれば費目名を列ヘッダ名称に設定し、
                            '対象請求月行の金額セルに金額を挿入
                            '入力されていなければデフォルト値を設定
                            If IsDBNull(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")) = True Then
                                .Columns(intCol).HeaderText = "費目" & StrConv(CStr(intCol + 1), VbStrConv.Wide)
                                .Rows(intTuki + 8).Cells(intCol).ReadOnly = True
                            Else
                                .Columns(intCol).HeaderText = Trim(CStr(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")))
                                .Rows(intTuki + 8).Cells(intCol).ReadOnly = False
                                .Rows(intTuki + 8).Cells(intCol).Value = Format(OBJ_DATAREADER.Item("HIMOKU_KINGAKU" & Format(intCol + 1, "00") & "_H"), "#,##0")
                            End If
                        Next intCol
                    Case 4 To 12
                        '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                        For intCol = 0 To intMaxHimokuCnt - 1
                            'For intCol = 0 To 9 '2007/04/04　費目は１０まで
                            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                            '費目名が入力されていれば費目名を列ヘッダ名称に設定し、
                            '対象請求月行の金額セルに金額を挿入
                            '入力されていなければデフォルト値を設定
                            If IsDBNull(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")) = True Then
                                .Columns(intCol).HeaderText = "費目" & CStr(StrConv(CStr(intCol + 1), VbStrConv.Wide))
                                .Rows(intTuki - 4).Cells(intCol).ReadOnly = True
                            Else
                                .Columns(intCol).HeaderText = Trim(CStr(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")))
                                .Rows(intTuki - 4).Cells(intCol).ReadOnly = False
                                .Rows(intTuki - 4).Cells(intCol).Value = Format(OBJ_DATAREADER.Item("HIMOKU_KINGAKU" & Format(intCol + 1, "00") & "_H"), "#,##0")
                            End If
                        Next intCol
                End Select
            End While
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_SET_SPDDATA2 = True

    End Function
    Private Function PFUNC_Hisuu_Check() As Boolean

        PFUNC_Hisuu_Check = False

        Dim sSiyou_Gakunen As String

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '学校マスタ存在チェック
            STR_SQL = " SELECT GAKKOU_CODE_G"
            STR_SQL += " FROM GAKMAST1"
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                lblGAKKOU_NAME.Text = "" '2007/02/14

                Exit Function
            End If
        End If

        '学年
        If Trim(txtGAKUNEN_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学年コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN_CODE.Focus()

            Exit Function
        Else

            Select Case CInt(txtGAKUNEN_CODE.Text)
                Case 0
                    MessageBox.Show(G_MSG0050W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()

                    Exit Function
            End Select

            '使用学年チェック
            STR_SQL = " SELECT SIYOU_GAKUNEN_T"
            STR_SQL += " FROM GAKMAST2"
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            sSiyou_Gakunen = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SIYOU_GAKUNEN_T")

            If Trim(sSiyou_Gakunen) = "" Then
                MessageBox.Show(G_MSG0051W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKUNEN_CODE.Focus()

                Exit Function
            End If

            Select Case CInt(sSiyou_Gakunen)
                Case Is < CInt(txtGAKUNEN_CODE.Text)
                    '使用最高学年以上の学年を入力
                    MessageBox.Show(G_MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()

                    Exit Function
            End Select
        End If

        '費目ID
        If Trim(cmbHimoku.Text) = "" Then
            MessageBox.Show(G_MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbHimoku.Focus()

            Exit Function
        End If

        PFUNC_Hisuu_Check = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        Dim bNyuryokuFlg As Boolean

        PFUNC_Nyuryoku_Check = False

        bNyuryokuFlg = False

        With DataGridView1
            '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            For intCnt As Integer = 0 To intMaxHimokuCnt - 1
                'For intCnt As Integer = 0 To 9
                '2017/02/22 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                '費目名が入力されていなければ次の行の入力値チェックを行う
                If IsDBNull(Trim(.Rows(intCnt).Cells(0).Value)) = False Then
                    If Trim(.Rows(intCnt).Cells(0).Value) <> "" Then
                        bNyuryokuFlg = True

                        '費目名 
                        If .Rows(intCnt).Cells(0).Value.Trim.Length > 10 Then
                            MessageBox.Show(G_MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function

                        End If

                        '金融機関コード
                        If IsDBNull(Trim(.Rows(intCnt).Cells(2).Value)) = True Then
                            MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                            'Else
                            '    '入力桁チェック
                            '    If Len(Trim(.Rows(intCnt).Cells(2).Value)) <> 4 Then
                            '        MessageBox.Show("金融機関コードの入力桁は４桁固定です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            '        Exit Function
                            '    End If
                        End If

                        '支店コード
                        If IsDBNull(Trim(.Rows(intCnt).Cells(3).Value)) = True Then
                            MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                            'Else
                            '    '入力桁チェック
                            '    If Len(Trim(.Rows(intCnt).Cells(3).Value)) <> 3 Then
                            '        MessageBox.Show("支店コードの入力桁は３桁固定です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            '        Exit Function
                            '    End If
                        End If

                        '入力金融機関マスタ存在チェック
                        STR_SQL = " SELECT * FROM TENMAST "
                        STR_SQL += " WHERE KIN_NO_N ='" & Trim(.Rows(intCnt).Cells(2).Value) & "'"
                        STR_SQL += " AND SIT_NO_N ='" & Trim(.Rows(intCnt).Cells(3).Value) & "'"

                        If GFUNC_ISEXIST(STR_SQL) = False Then
                            MessageBox.Show(MSG0096W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                        End If

                        '科目
                        If .Rows(intCnt).Cells(4).Value = Nothing OrElse IsNumeric(fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value)) = False Then
                            MessageBox.Show(G_MSG0055W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If

                        '口座番号
                        If .Rows(intCnt).Cells(5).Value = Nothing OrElse IsDBNull(Trim(.Rows(intCnt).Cells(5).Value)) = True Then
                            MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                            'Else
                            '    '入力桁チェック
                            '    If Len(Trim(.Rows(intCnt).Cells(5).Value)) <> 7 Then
                            '        MessageBox.Show("口座番号の入力桁は７桁固定です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            '        Exit Function
                            '    End If
                        End If

                        If Trim(.Rows(intCnt).Cells(6).Value) <> "" Then
                            '追加 
                            If .Rows(intCnt).Cells(6).Value.Trim.Length > 40 Then
                                MessageBox.Show(G_MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            Dim StrConvertRet As String = ""
                            If ConvKanaNGToKanaOK(Trim(.Rows(intCnt).Cells(6).Value), StrConvertRet) = False Then
                                MessageBox.Show(G_MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            Else
                                .Rows(intCnt).Cells(6).Value = StrConvertRet
                            End If
                        End If
                    End If
                End If
            Next intCnt
        End With

        If bNyuryokuFlg = False Then
            MessageBox.Show(G_MSG0058W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Print_Work() As Boolean

        Dim sESC_Gakkou_Code As String = ""
        Dim sESC_Himoku_Id As String = ""
        Dim sESC_Gakunen_Code As String = "" '追加 2005/10/20

        Dim iColumCount As Integer

        PFUNC_Print_Work = False

        ReDim Str_Work(277, 0)

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        STR_SQL = " DELETE  FROM MAST0300G_WORK"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        STR_SQL = " SELECT "
        STR_SQL += " HIMOMAST.*"
        STR_SQL += ", GAKKOU_NNAME_G , GAKUNEN_NAME_G"
        STR_SQL += " FROM "
        STR_SQL += " HIMOMAST , GAKMAST1"
        STR_SQL += " WHERE GAKKOU_CODE_H = GAKKOU_CODE_G"
        STR_SQL += " AND GAKUNEN_CODE_H = GAKUNEN_CODE_G"

        Select Case Trim(txtGAKKOU_CODE.Text)
            Case ""
                'Case "", "9999999999"
                '全学校指定
            Case Else
                STR_SQL += " AND GAKKOU_CODE_H = '" & Trim(txtGAKKOU_CODE.Text) & "'"
        End Select

        Select Case Trim(txtGAKUNEN_CODE.Text)
            Case ""
                'Case "", "9"
                '全学年指定
            Case Else
                STR_SQL += " AND GAKUNEN_CODE_H = " & Trim(txtGAKUNEN_CODE.Text)
        End Select

        Select Case Trim(cmbHimoku.Text)
            Case ""
                '全費目指定
            Case Else
                STR_SQL += " AND HIMOKU_ID_H = '" & Trim(cmbHimoku.Text) & "'"
        End Select
        STR_SQL += " ORDER BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , TUKI_NO_H"



        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        Dim lLoopCount As Long = 0

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                '学校コードが変わる度に追加
                If Trim(sESC_Gakkou_Code) <> Trim(.Item("GAKKOU_CODE_H")) Then
                    If Trim(sESC_Gakkou_Code) <> "" Then
                        lLoopCount += 1
                    End If

                    ReDim Preserve Str_Work(277, lLoopCount)
                Else '追加 2007/01/17
                    '学年コードが変わる度に追加
                    If sESC_Gakunen_Code <> Trim(.Item("GAKUNEN_CODE_H")) Then
                        If Trim(sESC_Gakunen_Code) <> "" Then
                            lLoopCount += 1
                        End If

                        ReDim Preserve Str_Work(277, lLoopCount)
                    End If
                End If

                Select Case .Item("HIMOKU_ID_H")
                    Case "000"
                        Str_Work(0, lLoopCount) = .Item("GAKKOU_CODE_H")
                        Str_Work(1, lLoopCount) = .Item("GAKKOU_NNAME_G")
                        Str_Work(2, lLoopCount) = .Item("GAKUNEN_CODE_H")
                        Str_Work(3, lLoopCount) = .Item("GAKUNEN_NAME_G")
                        Str_Work(4, lLoopCount) = .Item("HIMOKU_ID_H")
                        Str_Work(5, lLoopCount) = .Item("HIMOKU_ID_NAME_H")

                        Str_Work(276, lLoopCount) = .Item("SAKUSEI_DATE_H")
                        Str_Work(277, lLoopCount) = .Item("KOUSIN_DATE_H")

                        iColumCount = 6

                        For iHimokuCount As Integer = 1 To 15 '2007/04/04　費目は１０まで →　15まで
                            If IsDBNull(.Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H")) = False Then
                                Str_Work(iColumCount, lLoopCount) = Trim(.Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H"))

                                If IsDBNull(.Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 1, lLoopCount) = .Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 1, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 2, lLoopCount) = .Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 2, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 3, lLoopCount) = Trim(.Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 3, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 4, lLoopCount) = Trim(.Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 4, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 5, lLoopCount) = Trim(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 5, lLoopCount) = ""
                                End If
                            End If

                            iColumCount += 5

                            For iTukiCount As Integer = 1 To 12
                                If IsDBNull(.Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + iTukiCount, lLoopCount) = .Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + iTukiCount, lLoopCount) = "0"
                                End If
                            Next iTukiCount

                            iColumCount += 13
                        Next iHimokuCount
                    Case Else
                        If Trim(sESC_Himoku_Id) <> Trim(.Item("HIMOKU_ID_H")) Then
                            If Trim(sESC_Gakkou_Code) = Trim(.Item("GAKKOU_CODE_H")) Then

                                lLoopCount += 1
                                ReDim Preserve Str_Work(277, lLoopCount)
                            Else
                                '学年コードが変わる度に追加
                                If sESC_Gakunen_Code <> Trim(.Item("GAKUNEN_CODE_H")) Then
                                    If Trim(sESC_Gakunen_Code) <> "" Then
                                        lLoopCount += 1
                                    End If

                                    ReDim Preserve Str_Work(277, lLoopCount)
                                End If
                            End If

                            Str_Work(0, lLoopCount) = .Item("GAKKOU_CODE_H")
                            Str_Work(1, lLoopCount) = .Item("GAKKOU_NNAME_G")
                            Str_Work(2, lLoopCount) = .Item("GAKUNEN_CODE_H")
                            Str_Work(3, lLoopCount) = .Item("GAKUNEN_NAME_G")
                            Str_Work(4, lLoopCount) = .Item("HIMOKU_ID_H")
                            Str_Work(5, lLoopCount) = .Item("HIMOKU_ID_NAME_H")

                            Str_Work(276, lLoopCount) = .Item("SAKUSEI_DATE_H")
                            Str_Work(277, lLoopCount) = .Item("KOUSIN_DATE_H")
                        End If

                        iColumCount = 6

                        For iHimokuCount As Integer = 1 To 15
                            If IsDBNull(.Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H")) = False Then
                                Str_Work(iColumCount, lLoopCount) = .Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H")

                                If IsDBNull(.Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 1, lLoopCount) = .Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 1, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 2, lLoopCount) = .Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 2, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 3, lLoopCount) = .Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 3, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 4, lLoopCount) = .Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 4, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 5, lLoopCount) = Trim(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 5, lLoopCount) = ""
                                End If
                            End If

                            iColumCount += 5

                            '2005/10/06
                            If IsDBNull(.Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                Str_Work(iColumCount + CInt(.Item("TUKI_NO_H")), lLoopCount) = .Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")

                            Else
                                Str_Work(iColumCount + CInt(.Item("TUKI_NO_H")), lLoopCount) = "0"
                            End If

                            iColumCount += 13
                        Next iHimokuCount

                End Select
                sESC_Himoku_Id = Trim(.Item("HIMOKU_ID_H"))
                sESC_Gakunen_Code = Trim(.Item("GAKUNEN_CODE_H"))
                sESC_Gakkou_Code = Trim(.Item("GAKKOU_CODE_H"))
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        For l As Integer = 0 To lLoopCount
            STR_SQL = " INSERT INTO MAST0300G_WORK VALUES ("
            For l2 As Integer = 0 To 277
                Select Case l2
                    Case 0
                        STR_SQL += "'" & Str_Work(l2, l) & "'" & vbCrLf
                    Case 2, _
                        12 To 23, 30 To 41, 48 To 59, 66 To 77, 84 To 95, _
                        102 To 113, 120 To 131, 138 To 149, 156 To 167, 174 To 185, _
                        192 To 203, 210 To 221, 228 To 239, 246 To 257, 264 To 275
                        STR_SQL += "," & IIf(Trim(Str_Work(l2, l)) = "", 0, Str_Work(l2, l)) & vbCrLf
                    Case Else
                        STR_SQL += ",'" & IIf(Trim(Str_Work(l2, l)) = "", " ", Str_Work(l2, l)) & "'" & vbCrLf
                End Select
            Next

            STR_SQL += ")"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                Exit Function
            End If
        Next l

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Print_Work = True

    End Function
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbGAKKOUNAME_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        If cmbGAKKOUNAME.SelectedIndex = -1 Then

            Exit Sub
        End If


        'COMBOBOX選択時学校名,学校コード設定
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)
        '2007/02/14
        If Trim(txtGAKUNEN_CODE.Text) <> "" Then
            '情報抽出
            STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " group by GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
            STR_SQL += " ORDER BY HIMOKU_ID_H "

            '入力した学校ｺｰﾄﾞ,学年で費目マスタを抽出する
            If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then

            End If
        End If
        '学年テキストボックスにFOCUS
        txtGAKUNEN_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '学校名検索
            STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            If OBJ_DATAREADER.HasRows = False Then
                lblGAKKOU_NAME.Text = ""
            Else
                OBJ_DATAREADER.Read()
                lblGAKKOU_NAME.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If

            If Trim(txtGAKUNEN_CODE.Text) <> "" Then
                '情報抽出
                STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                STR_SQL += " group by GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                STR_SQL += " ORDER BY HIMOKU_ID_H "

                If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then

                End If
            End If
        End If

    End Sub
    Private Sub txtGAKUNEN_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN_CODE.Validating

        If Trim(txtGAKKOU_CODE.Text) <> "" Then

            If Trim(txtGAKUNEN_CODE.Text) <> "" Then
                '情報抽出
                STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                STR_SQL += " group by GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                STR_SQL += " ORDER BY HIMOKU_ID_H "

                '入力した学校ｺｰﾄﾞ,学年で費目マスタを抽出する
                If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then

                End If
            End If
        End If

    End Sub
    Private Sub cmbHimoku_Validating(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbHimoku.Validating

        If Trim(cmbHimoku.Text) <> "" Then
            If Not IsNumeric(cmbHimoku.Text) Then
                MessageBox.Show(G_MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbHimoku.Focus()
                Exit Sub
            End If
            If cmbHimoku.SelectedIndex <> -1 Then
                txtHIMOKU_PATERN.Text = Trim(Str_HName(cmbHimoku.SelectedIndex))
            Else
                txtHIMOKU_PATERN.Text = ""
            End If

            '2006/10/13　学校コード・学年コードの存在チェック
            If PFUNC_Hisuu_Check() = False Then
                Exit Sub
            End If

            '2010/09/10.Sakon　修正－０埋め後の値で判定する +++++++++++++++++++++++
            Select Case cmbHimoku.Text.Trim.PadLeft(cmbHimoku.MaxLength, "0"c)
                'Select Case Trim(cmbHimoku.Text)
                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                Case "000"
                    '情報抽出

                    Int_TabFlag = 0

                    STR_SQL = "SELECT GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H, HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " group by GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H,HIMOKU_ID_NAME_H "
                    '===2008/04/13 000参照可能対応========================================================
                    STR_SQL += " ORDER BY GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H,HIMOKU_ID_NAME_H "
                    '=====================================================================================

                    If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then
                        txtHIMOKU_PATERN.Text = "決済口座"

                        'ﾌｧｲﾙが存在しない場合は費目名にフォーカス移動
                        Call PSUB_Nyuuryoku_Clear(2)
                        DataGridView1.Focus()
                    Else
                        cmbHimoku.SelectedIndex = 0
                        txtHIMOKU_PATERN.Text = Trim(Str_HName(cmbHimoku.SelectedIndex))
                        'ﾌｧｲﾙが存在する場合は参照ボタンにフォーカス移動
                        btnFind.Focus()
                    End If
                Case Else
                    '情報抽出

                    Int_TabFlag = 1

                    '基本存在チェック
                    STR_SQL = " SELECT "
                    STR_SQL += " GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H"
                    STR_SQL += " FROM HIMOMAST"
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " AND HIMOKU_ID_H ='000'"

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        MessageBox.Show(G_MSG0060W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Text = ""
                        Exit Sub
                    End If

                    '費目項目存在チェック
                    STR_SQL = " SELECT "
                    STR_SQL += " GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H"
                    STR_SQL += " FROM HIMOMAST"
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " AND HIMOKU_ID_H ='" & cmbHimoku.Text.Trim.PadLeft(3, "0"c) & "'"

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        'ﾌｧｲﾙが存在しない場合は費目名にフォーカス移動
                        STR_SQL = "SELECT *"
                        STR_SQL += " FROM HIMOMAST"
                        STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                        STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                        STR_SQL += " AND HIMOKU_ID_H = '000'"

                        Call PFUNC_SET_SPDDATA(STR_SQL)

                        '存在チェック
                        STR_SQL = "SELECT *"
                        STR_SQL += " FROM HIMOMAST"
                        STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                        STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                        STR_SQL += " AND HIMOKU_ID_H = '" & cmbHimoku.Text & "'"

                        Call PFUNC_SET_SPDDATA2(STR_SQL)

                        Call PSUB_Nyuuryoku_Clear(5)

                        txtHIMOKU_PATERN.Focus()
                    Else
                        txtHIMOKU_PATERN.Text = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "HIMOKU_ID_NAME_H")
                        'ﾌｧｲﾙが存在する場合は参照ボタンにフォーカス移動
                        btnFind.Focus()
                    End If
            End Select
        End If

    End Sub
    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl
    Private ComboBoxEditCtrl As DataGridViewComboBoxEditingControl
    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                Select Case e.ColumnIndex
                    Case 0
                        CType(sender, DataGridView).ImeMode = ImeMode.Hiragana
                    Case 6
                        CType(sender, DataGridView).ImeMode = ImeMode.KatakanaHalf
                    Case 1, 2, 3, 4, 5
                        CType(sender, DataGridView).ImeMode = ImeMode.Disable
                End Select
            Case "DataGridView2"
                CType(sender, DataGridView).ImeMode = ImeMode.Disable
        End Select
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)


        'スプレット内項目前ZERO詰め
        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                With CType(sender, DataGridView)
                    Select Case colNo
                        Case 0 '2007/02/12　費目名チェック
                            If Not DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                                If DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.Length > 10 Then
                                    MessageBox.Show(G_MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    DataGridView1.Focus()
                                End If
                            End If
                        Case 1, 2, 3, 5, 6
                            If Not DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                                '費目名が入力されている行のみ有効
                                If Trim(.Rows(e.RowIndex).Cells(0).Value) <> "" Then
                                    If IsDBNull(.Rows(e.RowIndex).Cells(e.ColumnIndex).Value) = False Then
                                        Dim str_Value As String

                                        str_Value = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value

                                        If str_Value Is Nothing Then
                                            str_Value = ""
                                        End If

                                        Select Case e.ColumnIndex
                                            Case 1
                                                str_Value = Format(GCom.NzLong(str_Value.Replace(",", "")), "#,##0")
                                            Case 2
                                                str_Value = str_Value.PadLeft(4, "0"c)
                                            Case 3
                                                str_Value = str_Value.PadLeft(3, "0"c)
                                            Case 5
                                                str_Value = str_Value.PadLeft(7, "0"c)
                                            Case 6
                                                str_Value = StrConv(str_Value, VbStrConv.Uppercase)

                                                Dim StrConvertRet As String = ""
                                                str_Value = StrConv(StrConv(str_Value, VbStrConv.Katakana), VbStrConv.Narrow)
                                                If ConvKanaNGToKanaOK(str_Value, StrConvertRet) = True Then
                                                    str_Value = Replace(StrConvertRet, "ｰ", "-")
                                                End If
                                                If .Rows(e.RowIndex).Cells(e.ColumnIndex).Value.Length > 40 Then
                                                    MessageBox.Show(G_MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                    DataGridView1.Focus()
                                                End If
                                        End Select

                                        .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Trim(str_Value)
                                    End If
                                End If
                            End If
                    End Select
                End With
            Case "DataGridView2"
                Dim StrValue As String = DataGridView2.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                If IsNumeric(StrValue) Then
                    DataGridView2.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Format(CDbl(StrValue), "#,##0")
                End If
        End Select
    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                If colNo <> 4 Then
                    TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)
                Else
                    ComboBoxEditCtrl = CType(e.Control, DataGridViewComboBoxEditingControl)
                End If
                Select Case colNo
                    Case 0
                        AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 6
                        AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 2, 3, 5
                        AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                        AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
                    Case 1
                        AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                        AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
                    Case 4
                        AddHandler ComboBoxEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        AddHandler ComboBoxEditCtrl.KeyPress, AddressOf CAST.KeyPress
                End Select
            Case "DataGridView2"
                TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)
                AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                Select Case colNo
                    Case 0
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 6
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 2, 3, 5
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
                    Case 1
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
                    Case 4
                        RemoveHandler ComboBoxEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        RemoveHandler ComboBoxEditCtrl.KeyPress, AddressOf CAST.KeyPress
                End Select
            Case "DataGridView2"
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPressMoney
        End Select

        Call CellLeave(sender, e)
    End Sub
    Private Sub DataGridView1_DataError(ByVal sender As Object, _
        ByVal e As DataGridViewDataErrorEventArgs) _
        Handles DataGridView1.DataError
        e.Cancel = False
    End Sub
    Private Function fn_SetDataGridViewKamokuCmbFromText() As Boolean

        Dim cbstr As String() = Nothing
        Dim intCnt As Integer
        Dim Sp() As String

        Dim pTxtFile As String = STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT
        '*****************************************
        'テキストFILEからコンボボックス設定
        '*****************************************
        fn_SetDataGridViewKamokuCmbFromText = False

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":未登録", "GFUNC_TXT_TO_DBCOMBO", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, Encoding.GetEncoding(932))
        Dim Str_Line As String

        intCnt = 0
        'COMBOBOX ADD

        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Sp = Split(Str_Line, ",")

                ReDim Preserve cbstr(intCnt)
                cbstr(intCnt) = Sp(1)
                intCnt += 1
            End If
        Loop Until Str_Line Is Nothing


        Dim CmbCol As New DataGridViewComboBoxColumn

        For cnt As Integer = 0 To cbstr.Length - 1
            CmbCol.Items.Add(cbstr(cnt))
        Next

        CmbCol.DataPropertyName = "科目"
        DataGridView1.Columns.Insert(DataGridView1.Columns("科目").Index, CmbCol)
        DataGridView1.Columns.Remove("科目")
        CmbCol.Name = "科目"

        'FILE CLOSE
        Sr_File.Close()

        fn_SetDataGridViewKamokuCmbFromText = True
    End Function
    Private Function fn_GetKamokuCodeFromKamokuName(ByVal KamokuCode As String) As String

        Dim pTxtFile As String = STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":未登録", "GFUNC_TXT_TO_DBCOMBO", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return ""
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, Encoding.GetEncoding(932))
        Dim Str_Line As String

        Dim ArrayKamokuCode As String() = Nothing
        Dim ArrayKamokuName As String() = Nothing
        Dim intCnt As Integer
        Dim Sp() As String

        intCnt = 0
        'COMBOBOX ADD

        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Sp = Split(Str_Line, ",")

                ReDim Preserve ArrayKamokuCode(intCnt)
                ReDim Preserve ArrayKamokuName(intCnt)

                ArrayKamokuCode(intCnt) = Sp(0)
                ArrayKamokuName(intCnt) = Sp(1)

                intCnt += 1
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

        Dim CmbCol As New DataGridViewComboBoxColumn

        For cnt As Integer = 0 To ArrayKamokuName.Length - 1
            If ArrayKamokuName(cnt).Trim = KamokuCode.Trim Then
                Return ArrayKamokuCode(cnt)
            End If
        Next

        Return ""

    End Function
    Private Function fn_GetKamokuNameFromKamokuCode(ByVal KamokuCode As String) As String

        Dim pTxtFile As String = STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":未登録", "GFUNC_TXT_TO_DBCOMBO", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return ""
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, Encoding.GetEncoding(932))
        Dim Str_Line As String

        Dim ArrayKamokuCode As String() = Nothing
        Dim ArrayKamokuName As String() = Nothing
        Dim intCnt As Integer
        Dim Sp() As String

        intCnt = 0
        'COMBOBOX ADD

        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Sp = Split(Str_Line, ",")

                ReDim Preserve ArrayKamokuCode(intCnt)
                ReDim Preserve ArrayKamokuName(intCnt)

                ArrayKamokuCode(intCnt) = Sp(0)
                ArrayKamokuName(intCnt) = Sp(1)

                intCnt += 1
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()


        For cnt As Integer = 0 To ArrayKamokuCode.Length - 1
            If ArrayKamokuCode(cnt).Trim = KamokuCode.Trim Then
                Return ArrayKamokuName(cnt)
            End If
        Next

        Return ""

    End Function
    Public Function PFUNC_DB_COMBO_SET3(ByVal pSql As String, _
                   ByVal pCmb As ComboBox, _
                   ByRef pHName() As String) As Boolean
        '*****************************************
        '費目コンボボックス設定
        '*****************************************
        Dim iCounter As Integer

        PFUNC_DB_COMBO_SET3 = False

        'コンボクリア
        pCmb.Items.Clear()

        If GFUNC_SELECT_SQL2(pSql, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If
        iCounter = 0
        'コンボ設定
        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                pCmb.Items.Add(.Item("HIMOKU_ID_H"))
                ReDim Preserve pHName(iCounter)
                pHName(iCounter) = CStr(.Item("HIMOKU_ID_NAME_H"))

                iCounter += 1
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_DB_COMBO_SET3 = True
    End Function
    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView1.RowPostPaint

        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' 行ヘッダのセル領域を、行番号を描画する長方形とする
        ' （ただし右端に4ドットのすき間を空ける）
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' 上記の長方形内に行番号を縦方向中央＆右詰で描画する
        ' フォントや色は行ヘッダの既定値を使用する
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub
    Private Sub CustomDataGridView_RowPostPaint2(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView2.RowPostPaint

        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' 行ヘッダのセル領域を、行番号を描画する長方形とする
        ' （ただし右端に4ドットのすき間を空ける）
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth, dgv.Rows(e.RowIndex).Height)

        ' 上記の長方形内に行番号を縦方向中央＆右詰で描画する
        ' フォントや色は行ヘッダの既定値を使用する
        Dim strMonth As String

        Select Case e.RowIndex
            Case 11, 10, 9
                strMonth = e.RowIndex - 8
            Case Else
                strMonth = e.RowIndex + 4
        End Select

        TextRenderer.DrawText(e.Graphics, strMonth.ToString() + "月", dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub
    '++++++++++++++++++++++++++++++++++++++++++++++
    '2010/10/12.Sakon　セルのペースト対応
    '++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub CustomDataGridView_Past(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _
        DataGridView1.KeyDown, DataGridView2.KeyDown

        Dim dgv As DataGridView = CType(sender, DataGridView)
        Dim CellMaxLength As Integer = 0
        Dim pText As String

        If (e.Modifiers And Keys.Control) = Keys.Control And e.KeyCode = Keys.V Then

            '=====================================================
            '[Ctrl + V]キーが押されたときにペースト処理を実行
            '=====================================================
            pText = Clipboard.GetText           'クリップボードのテキストを取得
            pText = pText.Replace(vbCrLf, "")   '改行を取り除く

            '最大文字数取得・規定文字チェック（読替）
            If dgv.Name = "DataGridView1" Then
                Select Case dgv.CurrentCell.ColumnIndex
                    Case 0
                        CellMaxLength = Me.費目名.MaxInputLength
                    Case 1
                        CellMaxLength = Me.金額.MaxInputLength
                        pText = String.Format("{0:#,##0}", GCom.NzDec(pText, 1))
                    Case 2
                        CellMaxLength = Me.金融機関コード.MaxInputLength
                        pText = CStr(GCom.NzDec(pText, 1))
                    Case 3
                        CellMaxLength = Me.支店コード.MaxInputLength
                        pText = CStr(GCom.NzDec(pText, 1))
                    Case 5
                        CellMaxLength = Me.口座番号.MaxInputLength
                        pText = CStr(GCom.NzDec(pText, 1))
                    Case 6
                        CellMaxLength = Me.口座名義人カナ.MaxInputLength
                        If ConvKanaNGToKanaOK(Trim(pText), pText) = False Then
                            '半角カナ化できない場合はペーストを行わない
                            Exit Sub
                        End If
                    Case Else
                        Exit Sub
                End Select
            ElseIf dgv.Name = "DataGridView2" Then
                CellMaxLength = Me.費目１.MaxInputLength
                pText = String.Format("{0:#,##0}", GCom.NzDec(pText, 1))
            End If

            '最大文字数考慮
            If pText.Length > CellMaxLength Then
                pText = pText.Substring(0, CellMaxLength)
            End If

            dgv.CurrentCell.Value = pText   'テキストをセルに出力

        ElseIf e.KeyCode = Keys.Delete Or e.KeyCode = Keys.Back Then

            '==============================================================
            '[Delete]または[BackSpace]キーが押されたときにセルをクリア
            '==============================================================
            dgv.CurrentCell.Value = ""

        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
