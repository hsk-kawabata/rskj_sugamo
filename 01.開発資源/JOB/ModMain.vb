Option Strict On
Option Explicit On

Imports System.IO

' ジョブ監視メインモジュール
Public Module ModMain

    Public Const msgTitle As String = "ジョブ監視画面(KFUJOBW010)"

    Sub Main()
        Try
            '重複起動チェック
            Dim FileName As String = Path.GetFileNameWithoutExtension(Diagnostics.Process.GetCurrentProcess.ProcessName).ToUpper

            Dim Counter As Integer = _
                Diagnostics.Process.GetProcessesByName(FileName).GetUpperBound(0)

            If Counter >= 0 Then

                Dim arrProcess() As Process = _
                    Diagnostics.Process.GetProcessesByName(FileName)

                '見つけたログイン画面をアクティブにする
                For Index As Integer = 0 To arrProcess.Length - 1

                    If Diagnostics.Process.GetCurrentProcess.Id <> arrProcess(Index).Id() Then

                        Microsoft.VisualBasic.AppActivate(arrProcess(Index).Id())

                        Return
                    End If
                Next Index
            End If

            'ﾛｸﾞｲﾝ画面表示
            Dim MainDisp As New FrmJOB
            MainDisp.Show()

            Application.Run(MainDisp)

        Catch ex As Exception

        End Try
    End Sub

End Module
