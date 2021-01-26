Option Strict Off
Option Explicit On
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT010
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 月間スケジュール表印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Dim Str_Report_Path As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT010", "月間スケジュール表印刷画面")
    Private Const msgTitle As String = "月間スケジュール表印刷画面(KFGPRNT010)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)


            '対象年月の初期値設定
            txtFuriDateY.Text = Mid(lblDate.Text, 1, 4)
            txtFuriDateM.Text = Mid(lblDate.Text, 6, 2)

            '入力ボタン制御
            btnPrnt.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        ''印刷ボタン

        Dim Param As String
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            MainDB = New MyOracle
            '入力値チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "月間スケジュール表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            Dim nRet As Integer
            '月間スケジュール表印刷 
            'パラメータ設定：ログイン名,対象年月,帳票ソート順(1:振替日順 2:学校コード順)
            Param = GCom.GetUserID & "," & txtFuriDateY.Text & txtFuriDateM.Text & "," & IIf(rdbFURI_DATE.Checked, "1", "2")

            nRet = ExeRepo.ExecReport("KFGP012.EXE", Param)
            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "月間スケジュール表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "月間スケジュール表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
            End Select

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_YASUMI_GET(ByVal str年月日 As String) As Boolean

        PFUNC_YASUMI_GET = False

        '休日判定チェック
        STR_SQL = " SELECT * FROM YASUMIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " YASUMI_DATE_Y  ='" & Mid(str年月日, 1, 4) & Mid(str年月日, 6, 2) & Mid(str年月日, 9, 2) & "'"

        If GFUNC_ISEXIST(STR_SQL) = True Then
            PFUNC_YASUMI_GET = True
        End If

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean
        PFUNC_Nyuryoku_Check = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
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

            '日付整合性チェック
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & "01"
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            OraReader = New MyOracleReader(MainDB)

            '印刷対象のスケジュールが存在するか検索
            SQL.Append(" SELECT * FROM G_SCHMAST,GAKMAST2")
            SQL.Append(" WHERE NENGETUDO_S = " & SQ(txtFuriDateY.Text & txtFuriDateM.Text))
            SQL.Append(" AND GAKMAST2.KAISI_DATE_T <= " & SQ(txtFuriDateY.Text & txtFuriDateM.Text))
            SQL.Append(" AND GAKMAST2.SYURYOU_DATE_T >= " & SQ(txtFuriDateY.Text & txtFuriDateM.Text))
            SQL.Append(" AND G_SCHMAST.GAKKOU_CODE_S = GAKMAST2.GAKKOU_CODE_T")

            'スケジュール存在チェック
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_Nyuryoku_Check = True
    End Function



#End Region

#Region " イベント"
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
      Handles txtFuriDateY.Validating, txtFuriDateM.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

End Class
