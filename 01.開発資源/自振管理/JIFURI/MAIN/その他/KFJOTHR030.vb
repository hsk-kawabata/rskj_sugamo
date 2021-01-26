Option Explicit On
Option Strict On

Imports System
Imports System.IO
'Imports System.Data.OracleClient
Imports CASTCommon

Public Class KFJOTHR030
#Region "宣言"
    Inherits System.Windows.Forms.Form

    Private BatchLog As New CASTCommon.BatchLOG("KFJOTHR030", "センター直接持込落込取消画面")
    'Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Const ThisModuleName As String = "KFJOTHR030.vb"
    Private GCom As New MenteCommon.clsCommon

    Private Const msgTitle As String = "センター直接持込落込取消画面(KFJOTHR030)"

#End Region

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)", "失敗", ex.Message)
            Me.Close()

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)終了", "成功", "")

        End Try

    End Sub

    Private Sub KFJOTHR030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")

            GCom.GetSysDate = Date.Now
            GCom.GetUserID = gstrUSER_ID

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '処理日にシステム日付を表示
            txtFuriDateY.Text = Format(System.DateTime.Today, "yyyy")
            txtFuriDateM.Text = Format(System.DateTime.Today, "MM")
            txtFuriDateD.Text = Format(System.DateTime.Today, "dd")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)
            Me.Close()

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)終了", "成功", "")

        End Try

    End Sub

    '実行ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim SQL As System.Text.StringBuilder
        Dim MainDB As MyOracle = Nothing
        Dim oraReader As MyOracleReader = Nothing

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消実行)開始", "成功", "")

            Cursor.Current = Cursors.WaitCursor()

            'テキストボックスの入力チェック
            Dim onDate As New Date
            Dim strDate As String = "00000000"
            Dim CTL As TextBox = Nothing

            If Not FN_CHECK_TEXT(strDate) Then
                Return
            End If

            If MessageBox.Show(MSG0015I.Replace("{0}", "センター直接持込落込取消"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Return
            End If

            MainDB = New MyOracle
            oraReader = New MyOracleReader(MainDB)

            SQL = New System.Text.StringBuilder(128)

            SQL.Append("SELECT FSYORI_KBN_S,TORIS_CODE_S,TORIF_CODE_S,FURI_DATE_S,FUNOU_FLG_S FROM SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            SQL.Append(" AND MOTIKOMI_KBN_S = '1'")
            SQL.Append(" AND TOUROKU_DATE_S = '" & strDate & "'")
            SQL.Append(" ORDER BY FUNOU_FLG_S DESC")

            If oraReader.DataReader(SQL) = True Then

                '見つかったスケジュールの先頭に不能が立っていたら
                '処理が進んでいるスケジュール有なので取消不可
                If oraReader.GetString("FUNOU_FLG_S") = "0" Then

                    Do Until oraReader.EOF

                        Dim delKen As Long = 0

                        'スケジュールの更新
                        SQL.Length = 0
                        SQL.Append("UPDATE SCHMAST")
                        SQL.Append(" SET UKETUKE_FLG_S ='0'")
                        SQL.Append(", TOUROKU_FLG_S = '0'")
                        SQL.Append(", HAISIN_FLG_S = '0'")
                        SQL.Append(", SYORI_KEN_S = 0")
                        SQL.Append(", SYORI_KIN_S = 0")
                        SQL.Append(", ERR_KEN_S = 0")
                        SQL.Append(", ERR_KIN_S = 0")
                        SQL.Append(", TESUU_KIN_S = 0")
                        SQL.Append(", TESUU_KIN1_S = 0")
                        SQL.Append(", TESUU_KIN2_S = 0")
                        SQL.Append(", TESUU_KIN3_S = 0")
                        SQL.Append(", FURI_KEN_S = 0")
                        SQL.Append(", FURI_KIN_S = 0")
                        SQL.Append(", FUNOU_KEN_S = 0")
                        SQL.Append(", FUNOU_KIN_S = 0")
                        SQL.Append(", UKETUKE_DATE_S = '" & New String("0"c, 8) & "'")
                        SQL.Append(", TOUROKU_DATE_S = '" & New String("0"c, 8) & "'")
                        SQL.Append(", UFILE_NAME_S = NULL")
                        SQL.Append(", ERROR_INF_S = NULL")
                        SQL.Append(" WHERE FSYORI_KBN_S = '1'")
                        SQL.Append(" AND TORIS_CODE_S = '" & oraReader.GetString("TORIS_CODE_S") & "'")
                        SQL.Append(" AND TORIF_CODE_S = '" & oraReader.GetString("TORIF_CODE_S") & "'")
                        SQL.Append(" AND FURI_DATE_S = '" & oraReader.GetString("FURI_DATE_S") & "'")

                        If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                            MessageBox.Show(String.Format(MSG0027E, "センター直接持込", "落込取消"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            MainDB.Rollback()
                            Return
                        End If

                        '明細の削除
                        SQL.Length = 0
                        SQL.Append("DELETE FROM MEIMAST")
                        SQL.Append(" WHERE FSYORI_KBN_K = '1'")
                        SQL.Append(" AND TORIS_CODE_K = '" & oraReader.GetString("TORIS_CODE_S") & "'")
                        SQL.Append(" AND TORIF_CODE_K = '" & oraReader.GetString("TORIF_CODE_S") & "'")
                        SQL.Append(" AND FURI_DATE_K = '" & oraReader.GetString("FURI_DATE_S") & "'")

                        delKen = MainDB.ExecuteNonQuery(SQL)

                        If delKen <= 0 Then
                            MessageBox.Show(String.Format(MSG0027E, "センター直接持込", "落込取消"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            MainDB.Rollback()
                            Return
                        End If

                        BatchLog.Write(gstrUSER_ID, oraReader.GetString("TORIS_CODE_S") & "-" & oraReader.GetString("TORIF_CODE_S"), _
                           oraReader.GetString("FURI_DATE_S"), "(取消実行)", "成功", "明細削除件数:" & delKen.ToString & "件")

                        oraReader.NextRead()

                    Loop

                    MainDB.Commit()

                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'MessageBox.Show(MSG0025I.Replace("{0", "センター直接持込落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MessageBox.Show(MSG0025I.Replace("{0}", "センター直接持込落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Else

                    '進行スケジュールあり、取り消し不可
                    MessageBox.Show("不能結果更新済スケジュールがあるため取消できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                End If
            Else

                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            End If

            oraReader.Close()

            Return

        Catch ex As Exception

            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消実行)", "失敗", ex.Message)

        Finally

            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消実行)終了", "成功", "")
            Cursor.Current = Cursors.Default

        End Try
    End Sub

    '
    ' 機能　 ： 入力日付チェック
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： True=OK,False=NG
    '
    ' 備考　 ： Create 2009.10.12

    Private Function FN_CHECK_TEXT(ByRef strDate As String) As Boolean
        Try
            '振替日(年)
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateY.Text, "処理年", msgTitle) = False Then
                txtFuriDateY.Focus()
                Return False
            End If

            '振替日(月)
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateM.Text, "処理月", msgTitle) = False Then
                txtFuriDateM.Focus()
                Return False
            Else
                If CInt(txtFuriDateM.Text) < 1 Or CInt(txtFuriDateM.Text) > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateM.Focus()
                    Return False
                End If
            End If

            '振替日(日)
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateD.Text, "処理日", msgTitle) = False Then
                txtFuriDateD.Focus()
                Return False
            Else
                If CInt(txtFuriDateD.Text) < 1 Or CInt(txtFuriDateD.Text) > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateD.Focus()
                    Return False
                End If
            End If

            '処理日(日付型)を返す
            strDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(入力チェック)", "失敗", ex.Message)
        End Try

        Return False

    End Function

    '日付関連項目

    Private Sub Date_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

End Class

