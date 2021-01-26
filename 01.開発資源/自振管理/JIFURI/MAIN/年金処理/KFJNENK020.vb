Imports System.Text
Imports CAstReports.ClsExecute
Imports CASTCommon

Public Class KFJNENK020

#Region "宣言"

    Public clsFUSION As New clsFUSION.clsMain
    Public GCom As New MenteCommon.clsCommon

    Private BatchLog As New CASTCommon.BatchLOG("KFJNENK020", "年金振込支店コードチェックリスト印刷画面")
    Private Const MsgTitle As String = "年金振込支店コードチェックリスト印刷(KFJNENK020)"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private NenkSitNo As String = ""            '年金振込支店コード
    Private LogFuriDate As String = "00000000"  '振替日

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFJNENK020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLog.Write("0000000000-00", "00000000", "(ロード)開始", "成功")

            '------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------
            GCom.GetUserID = gstrUSER_ID
            GCom.GetSysDate = String.Format("{0:yyyy年MM月dd日}", Date.Now)
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '年金振込支店コードを取得
            '------------------------------------------
            NenkSitNo = CASTCommon.GetFSKJIni("COMMON", "NENKIN_SIT")

            If NenkSitNo = "err" Then
                MessageBox.Show(String.Format(MSG0001E, "年金振込支店コード", "COMMON", "NENKIN_SIT"), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)

        Finally
            BatchLog.Write("0000000000-00", "00000000", "(ロード)終了", "成功")

        End Try
    End Sub

#End Region

#Region "ボタン"

    '印刷ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            BatchLog.Write("0000000000-00", LogFuriDate, "(印刷)開始", "成功")

            '------------------------------------------
            '入力項目チェック
            '------------------------------------------
            If fn_check_text() = True Then
                '振替日取得
                LogFuriDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            Else
                Exit Sub
            End If

            '------------------------------------------
            '対象データチェック
            '------------------------------------------
            If fn_check_data() = False Then
                Exit Sub
            End If

            '------------------------------------------
            '印刷確認／印刷実行
            '------------------------------------------
            '確認メッセージ表示
            If MessageBox.Show(MSG0013I.Replace("{0}", "年金振込支店コードチェックリスト"), MsgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                '印刷実行
                If fn_print_out() = False Then
                    Exit Sub
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("0000000000-00", LogFuriDate, "(印刷)", "失敗", ex.Message)

        Finally
            BatchLog.Write("0000000000-00", LogFuriDate, "(印刷)終了", "成功")

        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Me.Close()
    End Sub

#End Region

#Region "テキスト"

    'テキストボックスゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("0000000000-00", "00000000", gstrSYORI_R, "(テキスト制御)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "関数"

    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/28
        'Update         :
        '============================================================================
        Try
            '------------------------------------------
            '振替年
            '------------------------------------------
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateY.Text, "振替年", MsgTitle) = False Then
                txtFuriDateY.Focus()
                Return False
            End If

            '------------------------------------------
            '振替月
            '------------------------------------------
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateM.Text, "振替月", MsgTitle) = False Then
                txtFuriDateM.Focus()
                Return False
            ElseIf txtFuriDateM.Text < 1 Or txtFuriDateM.Text > 12 Then
                MessageBox.Show(MSG0022W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------
            '振替日
            '------------------------------------------
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateD.Text, "振替日", MsgTitle) = False Then
                txtFuriDateD.Focus()
                Return False
            ElseIf txtFuriDateD.Text < 1 Or txtFuriDateD.Text > 31 Then
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
            BatchLog.Write("0000000000-00", "00000000", "(入力チェック処理)", "失敗", ex.Message)
            Return False

        End Try
    End Function

    Function fn_check_data() As Boolean
        '============================================================================
        'NAME           :fn_check_data
        'Parameter      :
        'Description    :対象データの存在チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/28
        'Update         :
        '============================================================================
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------
            '件数取得ＳＱＬ実行
            '------------------------------------------
            SQL.Append("SELECT COUNT(*) AS COUNT")
            SQL.Append(" FROM NENKINMAST, SCHMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & LogFuriDate & "'")    '振替日
            SQL.Append(" AND DATA_KBN_K = '2'")                         'データ区分(データ部)
            SQL.Append(" AND KEIYAKU_SIT_K = '" & NenkSitNo & "'")      '契約支店コード(ini)
            SQL.Append(" AND TOUROKU_FLG_S = '1'")                      '登録フラグ(落し込み処理済)
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                      '中断フラグ(中断していない)
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_S")

            gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            gdbcCONNECT.Open()

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = SQL.ToString
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader

            '------------------------------------------
            '登録件数チェック
            '------------------------------------------
            If gdbrREADER.Read = True AndAlso CLng(gdbrREADER.Item(0)) > 0 Then
                Return True
            End If

            MessageBox.Show(MSG0112W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Return False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("0000000000-00", LogFuriDate, "(入力チェック処理)", "失敗", ex.Message)
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
            param &= "," & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '------------------------------------------
            'バッチ実行
            '------------------------------------------
            nret = ExeRepo.ExecReport("KFJP040.EXE", param)

            '------------------------------------------
            '戻り値チェック
            '------------------------------------------
            If nret = 0 Then
                MessageBox.Show(MSG0014I.Replace("{0}", "年金振込支店コードチェックリスト"), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return True
            Else
                MessageBox.Show(MSG0004E.Replace("{0}", "年金振込支店コードチェックリスト"), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("0000000000-00", LogFuriDate, "(印刷処理)", "失敗", ex.Message)
            Return False

        End Try

    End Function

#End Region

End Class
