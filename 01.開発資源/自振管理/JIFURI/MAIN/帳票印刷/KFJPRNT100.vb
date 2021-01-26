Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT100

    Private MainLOG As New CASTCommon.BatchLOG(Me.Name, "処理結果確認表一覧印刷画面")
    Private Const msgTitle As String = "処理結果確認表一覧印刷画面(KFJPRNT100)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle = Nothing

    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Sub KFJPRNT100_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not MainDB Is Nothing Then
            MainDB.Close()
            MainDB = Nothing
        End If
    End Sub

    Private Sub KFJPRNT100_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            Me.CheckBox1.Checked = True
            Me.CheckBox2.Checked = True

            MainDB = New CASTCommon.MyOracle

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :実行ボタン
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            Dim PrintName As String = "処理結果確認表"

            If Not fn_EntryCheck() Then
                Exit Sub
            End If

            Dim SyoriDate As String = Me.txtSyoriDateY.Text & Me.txtSyoriDateM.Text & Me.txtSyoriDateD.Text

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            If CheckBox1.Checked = True Then

                Dim param As String
                Dim nRet As Integer
                '印刷バッチ呼び出し
                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me

                param = GCom.GetUserID & "," & SyoriDate

                PrintName = "処理結果確認表(登録)"
                nRet = ExeRepo.ExecReport("KFJP054.EXE", param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                        '印刷後確認メッセージ
                        MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Case -1
                        '印刷対象なしメッセージ
                        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End If

            If CheckBox2.Checked = True Then

                Dim param As String
                Dim nRet As Integer
                '印刷バッチ呼び出し
                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me

                param = GCom.GetUserID & "," & SyoriDate
                PrintName = "処理結果確認表(返還)"
                nRet = ExeRepo.ExecReport("KFJP055.EXE", param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                        '印刷後確認メッセージ
                        MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Case -1
                        '印刷対象なしメッセージ
                        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End If


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Function fn_EntryCheck() As Boolean

        Dim ret As Boolean = False

        Try
            '年必須チェック
            If txtSyoriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If
            '月必須チェック
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
            '日付必須チェック
            If txtSyoriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtSyoriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

            '帳票選択なし
            If CheckBox1.Checked = False AndAlso CheckBox2.Checked = False Then
                MessageBox.Show(MSG0358W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CheckBox1.Focus()
                Return False
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

        Return ret

    End Function

End Class