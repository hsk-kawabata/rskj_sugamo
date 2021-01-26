Option Strict On
Option Explicit On 

Imports System
Imports System.IO
Imports System.Diagnostics
Imports System.Windows.Forms

Module M_FUSION
    Private Const msgTitle As String = "ログイン(KFULGIN010)"
    Public Sub Main()
        Try
            '重複起動許可モジュール
            Dim OK_File() As String = New String(1) {"JOBKANSHI", ""}

            '重複起動チェック
            For Each PROC As String In Directory.GetFiles(Application.StartupPath, "*.EXE")

                Dim FileName As String = Path.GetFileNameWithoutExtension(PROC).ToUpper

                Select Case FileName
                    Case OK_File(0), OK_File(1)
                    Case Else

                        Dim Counter As Integer = _
                            Diagnostics.Process.GetProcessesByName(FileName).GetUpperBound(0)

                        If Counter >= 0 Then

                            Dim arrProcess() As Process = _
                                Diagnostics.Process.GetProcessesByName(FileName)

                            '見つけたログイン画面をアクティブにする
                            For Index As Integer = 0 To arrProcess.Length - 1

                                If Diagnostics.Process.GetCurrentProcess.Id <> arrProcess(Index).Id() Then

                                    Microsoft.VisualBasic.AppActivate(arrProcess(Index).Id())
                                    If arrProcess(Index).ProcessName <> "LOGIN" Then
                                        Dim DRet As DialogResult
                                        DRet = MessageBox.Show(MSG0353W, msgTitle, _
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                                        MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification)
                                    End If
                                    Return
                                End If
                            Next Index
                        End If
                End Select
            Next

            'ﾛｸﾞｲﾝ画面表示
            Dim MainDisp As New KFULGIN010
            MainDisp.ShowDialog()

        Catch ex As Exception

        End Try
    End Sub

End Module
