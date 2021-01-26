Option Strict On
Option Explicit On

Imports System
Imports System.Text

Public Class KFULGIN010
    Inherits System.Windows.Forms.Form

#Region "宣言"

    'キーコントロールクラス
    Private CAST As New CASTCommon.Events
    Private CASTlimit As New CASTCommon.Events

    'ログ書込クラス
    Private BatchLog As New CASTCommon.BatchLOG("KFULGIN010", "ログイン")

    'ログインクラス
    Private mLogin As CASTCommon.ClsLogin = Nothing
    Private Const msgtitle As String = "ログイン(KFULGIN010)"
#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFULGIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Try
            sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "ロード(開始)", "成功")
        Catch ex As Exception
            BatchLog.Write_Err("0000000000-00", "00000000", "ロード", "失敗", ex)
        Finally
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "ロード(終了)", "成功")
        End Try
        'BatchLog.Write("0000000000-00", "00000000", "ロード(開始)", "成功")
        'BatchLog.Write("0000000000-00", "00000000", "ロード(終了)", "成功")
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
    End Sub

#End Region

#Region "ボタン"

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "クローズ(開始)", "成功")
            'BatchLog.Write("0000000000-00", "00000000", "クローズ(開始)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Err("0000000000-00", "00000000", "クローズ", "失敗",ex)
            'BatchLog.Write("0000000000-00", "00000000", "クローズ", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "クローズ(終了)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try
    End Sub

    Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "ログイン(開始)", "成功")
            'BatchLog.Write("0000000000-00", "00000000", "ログイン(開始)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            If mLogin Is Nothing Then
                mLogin = New CASTCommon.ClsLogin
            End If

            'ログイン名入力チェック
            If Trim(txtUser.Text) = "" Then
                MessageBox.Show(MSG0004W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtUser.Focus()
                Exit Sub
            End If

            'パスワード入力チェック
            If Trim(txtPassword.Text) = "" Then
                MessageBox.Show(MSG0005W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtPassword.Focus()
                Exit Sub
            End If

            ' ユーザ権限チェック
            Select Case mLogin.LoginCheck(txtUser.Text, txtPassword.Text)
                Case 0
                    mLogin = Nothing
                    ' 実行
                    Dim ExeName As String
                    ExeName = System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), "MENU.EXE")
                    Try
                        Dim ExecProc As Process = Process.Start(ExeName, txtUser.Text)
                        If ExecProc.Id <> 0 Then
                        Else
                            Throw New Exception(String.Format(MSG0024E, ExeName))
                        End If
                    Catch ex As Exception
                        Dim MessageText As String
                        MessageText = ex.Message & "ファイル名:" & ExeName
                        MessageText &= Environment.NewLine
                        MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                        BatchLog.Write_LEVEL1("0000000000-00", "00000000", "ログイン", "失敗", MessageText)
                        'BatchLog.Write("0000000000-00", "00000000", "ログイン", "失敗", MessageText)
                        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
                        Exit Sub
                    End Try

                    Me.DialogResult = System.Windows.Forms.DialogResult.OK
                    Me.Close()
                Case 2
                    txtUser.Focus()
                Case Else
                    txtUser.Focus()
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Err("0000000000-00", "00000000", "ログイン","失敗", ex)
            'BatchLog.Write("0000000000-00", "00000000", "ログイン", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "ログイン(終了)", "成功")
            'BatchLog.Write("0000000000-00", "00000000", "ログイン(終了)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try
    End Sub

#End Region

#Region "テキストボックス"

    '全角文字の排除
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtUser.Validating, txtPassword.Validating
        Dim Ret As String = ""
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            With CType(sender, TextBox)
                If Not .Text = Nothing AndAlso .Text.Length > 0 Then

                    For Idx As Integer = 0 To .Text.Length - 1 Step 1

                        Dim Temp As String = .Text.Substring(Idx, 1)

                        Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")
                        Dim onByte() As Byte = JISEncoding.GetBytes(Temp)

                        Do While onByte.Length > 1
                            Temp = Temp.Substring(0, Temp.Length - 1)
                            onByte = JISEncoding.GetBytes(Temp)
                        Loop

                        Ret &= Temp
                    Next Idx
                End If

                .Text = Ret
            End With

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Err("0000000000-00", "00000000", "文字チェック", "失敗", ex)
            'BatchLog.Write("0000000000-00", "00000000", "文字チェック", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Finally

        End Try
    End Sub

    'クリップボードの掃除
    Private Sub TextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtUser.GotFocus, txtPassword.GotFocus
        'Clipboard.SetDataObject("")
    End Sub

#End Region

End Class
