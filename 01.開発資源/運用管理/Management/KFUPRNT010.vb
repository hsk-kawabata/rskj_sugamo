Imports CASTCommon

Public Class KFUPRNT010

    Private MainLOG As New CASTCommon.BatchLOG("KFUPRNT010", "ジョブ監視状況確認一覧表印刷画面")
    Private Const msgTitle As String = "ジョブ監視状況確認一覧表印刷画面(KFUPRNT010)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

#Region " ロード"

    Private Sub KFUPRNT010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "開始", "")

            '-------------------------------------
            ' ログ情報設定
            '-------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            '-------------------------------------
            ' ユーザID／システム日付表示
            '-------------------------------------
            Call GCom.SetMonitorTopArea(Label3, Label2, lblUser, lblDate)

            Dim SyoriDate As String = Now.ToString("yyyyMMdd")
            txtSyoriDateY.Text = SyoriDate.Substring(0, 4)
            txtSyoriDateM.Text = SyoriDate.Substring(4, 2)
            txtSyoriDateD.Text = SyoriDate.Substring(6, 2)

            txtSyoriDateY.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "終了", "")
        End Try

    End Sub

#End Region

#Region " 終了ボタン"

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "開始", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "終了", "")
        End Try
    End Sub

#End Region

#Region " 印刷ボタン"

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "開始", "")

            '--------------------------------
            ' 入力値チェック
            '--------------------------------
            If Check_Input() = False Then
                Exit Sub
            End If

            '--------------------------------
            ' 印刷前確認メッセージ
            '--------------------------------
            If MessageBox.Show(String.Format(MSG0013I, "ジョブ監視状況確認一覧表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            
            '--------------------------------
            ' パラメータ設定
            '  ①ログイン名：ログインユーザ名
            '  ②処理日：画面入力した処理日(yyyyMMdd)
            '--------------------------------
            Dim param As String
            Dim SyoriDate As String = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text
            param = GCom.GetUserID & "," & SyoriDate
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "パラメータ構築", "パラメータ:" & param)

            '--------------------------------
            ' 印刷バッチ呼び出し
            '--------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim nRet As Integer
            ExeRepo.SetOwner = Me

            nRet = ExeRepo.ExecReport("KFUP002.EXE", param)
            Select Case nRet
                Case 0     '<メッセージ> 印刷完了
                    MessageBox.Show(String.Format(MSG0014I, "ジョブ監視状況確認一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1    '<メッセージ> 印刷対象なし
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else  '<メッセージ> 印刷失敗
                    MessageBox.Show(String.Format(MSG0004E, "ジョブ監視状況確認一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "失敗", "リターンコード:" & nRet)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "終了", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try
    End Sub

#End Region

#Region " 関数"

    Private Function Check_Input() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力値チェック", "開始", "")

            '年必須チェック(MSG0018W)
            If txtSyoriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

            '月必須チェック(MSG0020W)
            If txtSyoriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '月範囲チェック
            If GCom.NzInt(txtSyoriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '日必須チェック(MSG0023W)
            If txtSyoriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '日範囲チェック
            If GCom.NzInt(txtSyoriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                txtSyoriDateY.Focus()
                Return False
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力値チェック", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力値チェック", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力値チェック", "終了", "")
        End Try

    End Function

#End Region

#Region " イベント "

    '================================
    ' ゼロパディング
    '================================
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtSyoriDateY.Validating, txtSyoriDateM.Validating, txtSyoriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

End Class