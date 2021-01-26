Option Strict On
Option Explicit On

Imports CASTCommon

Public Class KFJMAIN020

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MyOwnerForm As Form

    'ログ初期化
    Private BLOG As New CASTCommon.BatchLOG("KFJMAIN020", "他行データ作成")
    Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"
    Private Const msgTitle As String = "他行データ作成画面(KFJMAIN020)"

    '2013/12/24 saitou 標準版 性能向上 ADD -------------------------------------------------->>>>
    Private MainDB As CASTCommon.MyOracle
    '2013/12/24 saitou 標準版 性能向上 ADD --------------------------------------------------<<<<

    'フォームLOAD処理
    Private Sub KFJMAIN020_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            MyOwnerForm = GOwnerForm
            GOwnerForm = Me
            Me.btnEnd.DialogResult = Windows.Forms.DialogResult.None
             BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(ロード)開始", "成功", "")
            
            'ユーザＩＤ／システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Me.txtTorisCode.Clear()
            Me.txtTorifCode.Clear()

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            Dim strKey As String
            If cmbKana.SelectedItem Is Nothing Then
                strKey = ""
            Else
                strKey = cmbKana.SelectedItem.ToString
            End If
            Dim Jyoken As String = " AND TAKO_KBN_T = '1'"   '他行対象
            If GCom.SelectItakuName(strKey, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(String.Format(MSG0027E, "取引先コード", "検索"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(ロード)", "失敗", ex.Message)
            Me.Close()

        Finally
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(ロード)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BLOG.Write(gstrUSER_ID, "", "", "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write("(クローズ)", "失敗", ex.ToString)
        Finally
            BLOG.Write(gstrUSER_ID, "", "", "(クローズ)終了", "成功", "")
        End Try
    End Sub

    '作成ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
        '作り直し
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New System.Text.StringBuilder

        Try
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, BLOG.FuriDate, "(作成)開始", "成功", "")

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_Check_Text() = False Then
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            '--------------------------------
            '取引先存在チェック
            '--------------------------------
            Dim strTORIS_CODE As String
            Dim strTORIF_CODE As String
            Dim strFURI_DATE As String

            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            BLOG.ToriCode = strTORIS_CODE & "-" & strTORIF_CODE
            BLOG.FuriDate = strFURI_DATE

            SQL.Length = 0
            SQL.Append("SELECT * FROM TORIMAST WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    If oraReader.GetString("TAKO_KBN_T") = "0" Then
                        MessageBox.Show(MSG0069W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtTorisCode.Focus()
                        Return
                    End If

                    oraReader.NextRead()
                End While
            Else
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return
            End If

            oraReader.Close()

            '--------------------------------
            'スケジュール存在チェック
            '--------------------------------
            SQL.Length = 0
            SQL.Append("SELECT * FROM SCHMAST WHERE TORIS_CODE_S = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    If oraReader.GetString("TOUROKU_FLG_S") = "0" Then
                        MessageBox.Show(MSG0339W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtFuriDateY.Focus()
                        Return
                    End If

                    If oraReader.GetString("HAISIN_FLG_S") = "1" Then
                        MessageBox.Show(MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtFuriDateY.Focus()
                        Return
                    End If

                    oraReader.NextRead()
                End While
            Else
                MessageBox.Show(MSG0064W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return
            End If

            oraReader.Close()

            If MessageBox.Show(String.Format(MSG0023I, "他行データ作成処理"), _
                msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Exit Sub
            End If

            '------------------------------------------------
            'ジョブマスタに登録
            '------------------------------------------------
            Dim jobid As String = "J020"
            Dim para As String = strTORIS_CODE & strTORIF_CODE & "," & strFURI_DATE

            MainDB.BeginTrans()
            Dim iRet As Integer = BLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            ElseIf iRet = -1 Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            If BLOG.InsertJOBMAST(jobid, gstrUSER_ID, para, MainDB) = False Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                MainDB.Commit()
                MessageBox.Show(String.Format(MSG0021I, "他行データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, BLOG.FuriDate, "作成", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, BLOG.FuriDate, "(作成)終了", "成功", "")

            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

        End Try

        'Try
        '    BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(作成)開始", "成功", "")

        '    '--------------------------------
        '    'テキストボックスの入力チェック
        '    '--------------------------------
        '    If fn_Check_Text() = False Then
        '        Exit Sub
        '    End If

        '    gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        '    gdbcCONNECT.Open()
        '    '--------------------------------
        '    '取引先存在チェック
        '    '--------------------------------
        '    Dim strTORIS_CODE As String
        '    Dim strTORIF_CODE As String
        '    Dim strFURI_DATE As String

        '    strTORIS_CODE = txtTorisCode.Text
        '    strTORIF_CODE = txtTorifCode.Text
        '    strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
        '    BLOG.ToriCode = strTORIS_CODE & "-" & strTORIF_CODE
        '    BLOG.FuriDate = strFURI_DATE
        '    gstrSSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & strTORIS_CODE & "'"
        '    gstrSSQL = gstrSSQL & " AND TORIF_CODE_T = '" & strTORIF_CODE & "'"

        '    gdbCOMMAND = New OracleClient.OracleCommand()
        '    gdbCOMMAND.CommandText = gstrSSQL
        '    gdbCOMMAND.Connection = gdbcCONNECT

        '    gdbrREADER = gdbCOMMAND.ExecuteReader

        '    '読込のみ
        '    Dim intCOUNT As Integer
        '    intCOUNT = 0
        '    While (gdbrREADER.Read)
        '        intCOUNT += 1
        '        If clsFUSION.fn_chenge_null(gdbrREADER.Item("TAKO_KBN_T"), "0") = "0" Then
        '            MessageBox.Show(MSG0069W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            txtTorisCode.Focus()
        '            gdbcCONNECT.Close()
        '            Exit Sub
        '        End If
        '    End While

        '    If intCOUNT = 0 Then
        '        MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '        txtTorisCode.Focus()
        '        gdbcCONNECT.Close()
        '        Exit Sub
        '    End If

        '    '--------------------------------
        '    'スケジュール存在チェック
        '    '--------------------------------
        '    gstrSSQL = "SELECT * FROM SCHMAST WHERE TORIS_CODE_S = '" & strTORIS_CODE & "'"
        '    gstrSSQL = gstrSSQL & " AND TORIF_CODE_S = '" & strTORIF_CODE & "'"
        '    gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & strFURI_DATE & "'"

        '    gdbCOMMAND = New OracleClient.OracleCommand
        '    gdbCOMMAND.CommandText = gstrSSQL
        '    gdbCOMMAND.Connection = gdbcCONNECT

        '    gdbrREADER = gdbCOMMAND.ExecuteReader

        '    '読込のみ
        '    intCOUNT = 0
        '    While (gdbrREADER.Read)
        '        intCOUNT += 1

        '        If CDbl(clsFUSION.fn_chenge_null(gdbrREADER.Item("TOUROKU_FLG_S"), "0"c)) = 0 Then
        '            MessageBox.Show(MSG0339W, _
        '                            msgTitle, _
        '                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            txtFuriDateY.Focus()
        '            gdbcCONNECT.Close()
        '            Exit Sub
        '        End If
        '        If CDbl(clsFUSION.fn_chenge_null(gdbrREADER.Item("HAISIN_FLG_S"), "0")) = 1 Then
        '            MessageBox.Show(MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            txtFuriDateY.Focus()
        '            gdbcCONNECT.Close()
        '            Exit Sub
        '        End If
        '    End While

        '    If intCOUNT = 0 Then
        '        MessageBox.Show(MSG0064W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '        txtFuriDateY.Focus()
        '        gdbcCONNECT.Close()
        '        Exit Sub
        '    End If

        '    '2010/01/20 確認メッセージ追加
        '    If MessageBox.Show(String.Format(MSG0023I, "他行データ作成処理"), _
        '                    msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
        '        Exit Sub
        '    End If
        '    '==============================

        '    '------------------------------------------------
        '    'ジョブマスタに登録
        '    '------------------------------------------------
        '    '2013/10/24 saitou 標準修正 ADD -------------------------------------------------->>>>
        '    Dim jobid As String = String.Empty
        '    Dim para As String = String.Empty
        '    '2013/10/24 saitou 標準修正 ADD --------------------------------------------------<<<<
        '    jobid = "J020"
        '    para = strTORIS_CODE & strTORIF_CODE & "," & strFURI_DATE

        '    If clsFUSION.fn_JOBMAST_TOUROKU_CHECK(jobid, gstrUSER_ID, para) = False Then
        '        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '        gdbcCONNECT.Close()
        '        Exit Sub
        '    End If

        '    If clsFUSION.fn_INSERT_JOBMAST(jobid, gstrUSER_ID, para) = False Then
        '        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURI_DATE, "起動パラメータ登録", "失敗", "起動パラメータ登録に失敗しました")
        '    Else
        '        MessageBox.Show(String.Format(MSG0021I, "他行データ作成"), _
        '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        '        BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURI_DATE, "起動パラメータ登録", "成功", "起動パラメータを登録しました")
        '    End If

        '    gdbcCONNECT.Close()

        'Catch ex As Exception
        '    MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(作成)", "失敗", ex.Message)
        'Finally
        '    gdbcCONNECT.Close() '2010/03/26 必ず閉じるよう修正
        '    BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(作成)終了", "成功", "")
        'End Try
        '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<

    End Sub

    '取引先コードテキストボックスに取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged

        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先コードを取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取引先コード設定)", "失敗", ex.Message)

        Finally
        End Try

    End Sub

    Function fn_Check_Text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/07/14
        'Update         :
        '============================================================================
        fn_Check_Text = False

        '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
        '取引先主コード必須チェック
        If txtTorisCode.Text.Trim = "" Then
            MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTorisCode.Focus()
            Return False
        End If
        '取引先副コード必須チェック
        If txtTorifCode.Text.Trim = "" Then
            MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTorifCode.Focus()
            Return False
        End If

        '年必須チェック
        If txtFuriDateY.Text.Trim = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Return False
        End If
        '月必須チェック
        If txtFuriDateM.Text.Trim = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateM.Focus()
            Return False
        End If
        '月範囲チェック
        If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
            MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateM.Focus()
            Return False
        End If
        '日付必須チェック
        If txtFuriDateD.Text.Trim = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateD.Focus()
            Return False
        End If
        '日付範囲チェック
        If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
            MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateD.Focus()
            Return False
        End If

        '日付整合性チェック
        Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
        If Information.IsDate(WORK_DATE) = False Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Return False
        End If

        'If fn_CHECK_NUM_MSG(txtTorisCode.Text, "取引先主コード", msgTitle) = False Then
        '    txtTorisCode.Focus()
        '    Exit Function
        'End If
        'If fn_CHECK_NUM_MSG(txtTorifCode.Text, "取引先副コード", msgTitle) = False Then
        '    txtTorifCode.Focus()
        '    Exit Function
        'End If
        'If fn_CHECK_NUM_MSG(txtFuriDateY.Text, "振替年", msgTitle) = False Then
        '    txtFuriDateY.Focus()
        '    Exit Function
        'End If
        'If fn_CHECK_NUM_MSG(txtFuriDateM.Text, "振替月", msgTitle) = False Then
        '    txtFuriDateM.Focus()
        '    Exit Function
        'Else
        '    If CInt(txtFuriDateM.Text) < 1 Or CInt(txtFuriDateM.Text) > 12 Then
        '        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

        '        txtFuriDateM.Focus()
        '        Exit Function
        '    End If
        'End If
        'If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateD.Text, "振替日", msgTitle) = False Then
        '    txtFuriDateD.Focus()
        '    Exit Function
        'Else
        '    If CInt(txtFuriDateD.Text) < 1 Or CInt(txtFuriDateD.Text) > 31 Then
        '        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

        '        txtFuriDateD.Focus()
        '        Exit Function
        '    End If
        'End If
        '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<
        fn_Check_Text = True

    End Function

    '取引先主／副コード入力時ゼロ埋め
    Private Sub txtTorisCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, _
            txtTorifCode.Validating, _
            txtFuriDateY.Validating, _
            txtFuriDateM.Validating, _
            txtFuriDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, "00000000", "取引先主/副コード入力ゼロ埋め(終了)", "失敗", ex.Message)
        End Try
    End Sub

    '画面終了時処理
    Private Sub KFJMAIN020_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        End Try

    End Sub

    '2013/12/24 saitou 標準版 性能向上 DEL -------------------------------------------------->>>>
    'Function fn_CHECK_NUM_MSG(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
    '    '============================================================================
    '    'NAME           :fn_CHECK_NUM_MSG
    '    'Parameter      :objOBJ：チェック対象オブジェクト／strJNAME：オブジェクト名称
    '    '               :gstrTITLE：タイトル
    '    'Description    :数値チェック
    '    'Return         :True=OK,False=NG
    '    'Create         :2004/05/28
    '    'Update         :
    '    '============================================================================
    '    fn_CHECK_NUM_MSG = False
    '    If Trim(objOBJ).Length = 0 Then
    '        MessageBox.Show(String.Format(MSG0285W, strJNAME), _
    '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        fn_CHECK_NUM_MSG = False
    '        Exit Function
    '    End If
    '    '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
    '    'Dim i As Integer
    '    '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
    '    fn_CHECK_NUM_MSG = True
    'End Function
    '2013/12/24 saitou 標準版 性能向上 DEL --------------------------------------------------<<<<

    'カナコンボボックス設定変更時再読み込み
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '選択カナで始まる委託者名を取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim Jyoken As String = " AND TAKO_KBN_T = '1'"   '他行対象
            If GCom.SelectItakuName(CStr(cmbKana.SelectedItem), cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(String.Format(MSG0027E, "委託者名リスト", "コンボボックス設定"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            If Trim(cmbToriName.Text) = "" Then
                cmbToriName.Focus()
            Else
                txtTorisCode.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "(委託者名リストコンボボックス設定)", "失敗", ex.Message)

        Finally
        End Try

    End Sub
End Class