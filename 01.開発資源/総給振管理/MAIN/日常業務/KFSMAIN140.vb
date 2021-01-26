Imports System
Imports CASTCommon

''' <summary>
''' 送付状入力処理選択画面
''' </summary>
''' <remarks>2017/11/30 標準版 added for 照合対応</remarks>
Public Class KFSMAIN140

#Region "クラス定数"
    Private Const msgTitle As String = "送付状入力処理選択画面(KFSMAIN140)"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

#End Region

#Region "クラス変数"
    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN140", "送付状入力処理選択画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFSMAIN140_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '--------------------------------------------------
            'ログの書込に必要な情報の取得
            '--------------------------------------------------
            Me.LW.UserID = GCom.GetUserID
            Me.LW.ToriCode = "000000000000"
            Me.LW.FuriDate = "00000000"

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)開始", "成功", "")

            '--------------------------------------------------
            'システム日付とユーザ名を表示
            '--------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label2, Me.Label3, Me.lblUser, Me.lblDate)

            '--------------------------------------------------
            '入力日付にシステム日付を設定
            '--------------------------------------------------
            Dim strSysDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
            Me.txtNyuryokuDateY.Text = strSysDate.Substring(0, 4)
            Me.txtNyuryokuDateM.Text = strSysDate.Substring(4, 2)
            Me.txtNyuryokuDateD.Text = strSysDate.Substring(6, 2)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 実行ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(実行)開始", "成功", "")

            '--------------------------------------------------
            'テキストボックスの入力チェック
            '--------------------------------------------------
            If Me.rbUpd.Checked Then
                If Me.fn_check_text() = False Then
                    Return
                End If
            End If

            '--------------------------------------------------
            '画面遷移
            '--------------------------------------------------
            If Me.rbNew.Checked Then
                '新規
                Dim KFSMAIN141 As New KFSMAIN141
                CASTCommon.ShowFORM(GCom.GetUserID, CType(KFSMAIN141, Form), Me)
            ElseIf Me.rbUpd.Checked Then
                '変更
                Dim KFSMAIN142 As New KFSMAIN142
                KFSMAIN142.SELECT_DATE = Me.txtNyuryokuDateY.Text & Me.txtNyuryokuDateM.Text & Me.txtNyuryokuDateD.Text
                CASTCommon.ShowFORM(GCom.GetUserID, CType(KFSMAIN142, Form), Me)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' テキストボックスヴァリデイティングイベント（ゼロパディング）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtNyuryokuDateY.Validating, txtNyuryokuDateM.Validating, txtNyuryokuDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ゼロパディング", "失敗", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' ラジオボタンチェックチェンジイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbNew_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbNew.CheckedChanged
        Try
            If Me.rbNew.Checked Then
                Me.pnlNyuryokuDate.Visible = False
            Else
                Me.pnlNyuryokuDate.Visible = True
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "チェックチェンジ(新規)", "失敗", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' ラジオボタンチェックチェンジイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbUpd_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbUpd.CheckedChanged
        Try
            If Me.rbUpd.Checked Then
                Me.pnlNyuryokuDate.Visible = True
            Else
                Me.pnlNyuryokuDate.Visible = False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "チェックチェンジ(変更)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックをします。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_check_text() As Boolean
        Try
            '入力日(年)必須チェック
            If Me.txtNyuryokuDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNyuryokuDateY.Focus()
                Return False
            End If

            '入力日(月)必須チェック
            If Me.txtNyuryokuDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNyuryokuDateM.Focus()
                Return False
            End If

            '入力日(月)範囲チェック
            If GCom.NzInt(Me.txtNyuryokuDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtNyuryokuDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNyuryokuDateM.Focus()
                Return False
            End If

            '入力日(日)必須チェック
            If Me.txtNyuryokuDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNyuryokuDateD.Focus()
                Return False
            End If

            '入力日(日)範囲チェック
            If GCom.NzInt(Me.txtNyuryokuDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtNyuryokuDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNyuryokuDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = Me.txtNyuryokuDateY.Text & "/" & Me.txtNyuryokuDateM.Text & "/" & Me.txtNyuryokuDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNyuryokuDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "入力チェック", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Class
