Imports System.Text
Imports CASTCommon

Public Class KFJNENK040

#Region "宣言"

    Private BatchLog As New CASTCommon.BatchLOG("KFJNENK040", "年金受取人別振込明細表画面")
    Private Const MsgTitle As String = "年金受取人別振込明細表(KFJNENK040)"
    Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private LogToriCode As String = "0000000000-00"
    Private LogFuriDate As String = "00000000"

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFJNENK040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLog.Write(LogToriCode, LogFuriDate, "(ロード)開始", "成功")

            '------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------
            GCom.GetUserID = gstrUSER_ID
            GCom.GetSysDate = String.Format("{0:yyyy年MM月dd日}", Date.Now)
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            Dim Jyoken As String = "  AND FMT_KBN_T = '03'"   '年金フォーマット
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(ロード)", "失敗", ex.Message)

        Finally
            BatchLog.Write(LogToriCode, LogFuriDate, "(ロード)終了", "成功")
        End Try
    End Sub

#End Region

#Region "ボタン"

    '印刷ボタン
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Try
            BatchLog.Write(LogToriCode, LogFuriDate, "(印刷)開始", "成功")

            '------------------------------------------
            '入力チェック
            '------------------------------------------
            If fn_CheckText() = False Then
                Exit Sub
            End If

            '------------------------------------------
            'ログ書込用パラメータ設定
            '------------------------------------------
            LogToriCode = txtTorisCode.Text & "-" & txtTorifCode.Text
            LogFuriDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '------------------------------------------
            '取引先マスタチェック
            '------------------------------------------
            If Not (txtTorisCode.Text = "9999999999" And txtTorifCode.Text = "99") Then
                If fn_CheckTORIMAST() = False Then
                    Exit Sub
                End If
            End If
            
            '------------------------------------------
            '対象データ存在チェック
            '------------------------------------------
            If fn_CheckData() = False Then
                Exit Sub
            End If

            '------------------------------------------
            '印刷確認／印刷実行
            '------------------------------------------
            '確認メッセージ表示
            If MessageBox.Show(MSG0013I.Replace("{0}", "年金受取人別振込明細表"), MsgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                '印刷実行
                If fn_print_out() = False Then
                    Exit Sub
                End If
            End If


        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(印刷)", "失敗", ex.Message)

        Finally
            BatchLog.Write(LogToriCode, LogFuriDate, "(印刷)終了", "成功")
        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BatchLog.Write(LogToriCode, LogFuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            BatchLog.Write(LogToriCode, LogFuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "テキスト"

    'テキストボックスゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, txtTorifCode.Validating, _
            txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(テキスト制御)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "コンボボックス"

    '委託者名コンボボックスの設定
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '------------------------------------------
            '選択カナで始まる委託者名を取得
            '------------------------------------------
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                Dim Jyoken As String = "  AND FMT_KBN_T = '03'"   '年金フォーマット
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                cmbToriName.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(委託者名リストボックス設定)", "失敗", ex.Message)

        End Try
    End Sub

    '取引先コードテキストボックスに取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '------------------------------------------
            '取引先コードを取得
            '------------------------------------------
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(取引先コード設定)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "関数"

    Function fn_CheckText() As Boolean
        '=====================================================================================
        'NAME           :fn_CheckText
        'Parameter      :なし
        'Description    :入力項目のチェック処理(参照処理時)
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Try
            '------------------------------------------
            '取引先主コード
            '------------------------------------------
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            '------------------------------------------
            '取引先副コード
            '------------------------------------------
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If

            '------------------------------------------
            '振替日(年)
            '------------------------------------------
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '------------------------------------------
            '振替日(月)
            '------------------------------------------
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            ElseIf CInt(txtFuriDateM.Text) < 1 Or CInt(txtFuriDateM.Text) > 12 Then
                MessageBox.Show(MSG0022W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------
            '振替日(日)
            '------------------------------------------
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            ElseIf CInt(txtFuriDateD.Text) < 1 Or CInt(txtFuriDateD.Text) > 31 Then
                MessageBox.Show(MSG0025W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(参照)", "失敗", ex.Message)
            Return False
        End Try
    End Function

    Function fn_CheckTORIMAST() As Boolean
        '=====================================================================================
        'NAME           :fn_CheckTORIMAST
        'Parameter      :なし
        'Description    :取引先のチェック処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT FMT_KBN_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = '" & txtTorisCode.Text & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & txtTorifCode.Text & "'")

            gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            gdbcCONNECT.Open()

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = SQL.ToString
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader

            '------------------------------------------
            'フォーマット区分チェック
            '------------------------------------------
            If gdbrREADER.Read = False Then
                MessageBox.Show(MSG0063W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            Else
                If gdbrREADER.Item(0) <> "03" Then
                    MessageBox.Show(MSG0270W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorisCode.Focus()
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(取引先チェック)", "失敗", ex.Message)
            Return False

        Finally
            If gdbcCONNECT IsNot Nothing Then
                gdbcCONNECT.Close()
            End If
        End Try
    End Function

    Function fn_CheckData() As Boolean
        '=====================================================================================
        'NAME           :fn_CheckData
        'Parameter      :なし
        'Description    :対象データの検索
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT COUNT(*) AS COUNT")
            SQL.Append(" FROM NENKINMAST, SCHMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & LogFuriDate & "'")    '振替日
            SQL.Append(" AND DATA_KBN_K = '2'")                         'データ区分(データ部)
            SQL.Append(" AND TOUROKU_FLG_S = '1'")                      '登録フラグ(落し込み処理済)
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                      '中断フラグ(中断していない)
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_S")

            If Not (txtTorisCode.Text = "9999999999" And txtTorifCode.Text = "99") Then
                SQL.Append(" AND TORIS_CODE_S = '" & txtTorisCode.Text & "'")
                SQL.Append(" AND TORIF_CODE_S = '" & txtTorifCode.Text & "'")
            End If

            gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            gdbcCONNECT.Open()

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = SQL.ToString
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader

            '------------------------------------------
            '登録チェック
            '------------------------------------------
            If gdbrREADER.Read = False OrElse gdbrREADER.Item(0) < 1 Then
                MessageBox.Show(MSG0112W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(対象データ検索)", "失敗", ex.Message)
            Return False

        Finally
            If gdbcCONNECT IsNot Nothing Then
                gdbcCONNECT.Close()
            End If
        End Try
    End Function

    Function fn_print_out() As Boolean
        '============================================================================
        'NAME           :fn_print_out
        'Parameter      :
        'Description    :帳票印刷バッチ実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Dim param As String = ""
        Dim nret As Integer = 0

        Try
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '------------------------------------------
            'パラメータ設定
            '------------------------------------------
            param = GCom.GetUserID
            param &= "," & txtTorisCode.Text & txtTorifCode.Text
            param &= "," & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '------------------------------------------
            'バッチ実行
            '------------------------------------------
            nret = ExeRepo.ExecReport("KFJP039.EXE", param)

            '------------------------------------------
            '戻り値チェック
            '------------------------------------------
            If nret = 0 Then
                MessageBox.Show(MSG0014I.Replace("{0}", "年金受取人別振込明細表"), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return True
            Else
                MessageBox.Show(MSG0004E.Replace("{0}", "年金受取人別振込明細表"), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(印刷処理)", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Class
