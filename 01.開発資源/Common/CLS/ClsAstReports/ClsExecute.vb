Imports System.IO

' 印刷中 制御 クラス
Public Class ClsExecute

    Private FrmProg As FrmProgress
    Private Owner As Form = Nothing

    Public WriteOnly Property SetOwner() As Form
        Set(ByVal Value As Form)
            Owner = Value
        End Set
    End Property

    ' レポートを出力する
    Public Function ExecReport(ByVal execName As String, Optional ByVal CmdArg As String = "") As Integer
        Try
            Dim ProcReport As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), execName)
            If CmdArg.Length > 0 Then
                ProcInfo.Arguments = CmdArg
            End If
            ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "EXE")
            ProcInfo.UseShellExecute = False
            ProcInfo.CreateNoWindow = True
            ProcInfo.RedirectStandardOutput = True
            ProcReport = Process.Start(ProcInfo)

            Call ShowProgress()

            Do While True
                If ProcReport.HasExited = True Then
                    Exit Do
                Else
                    Dim NowProc As Process
                    Try
                        NowProc = Diagnostics.Process.GetProcessById(ProcReport.Id)
                        If NowProc Is Nothing Then
                            Exit Do
                        End If
                    Catch ex As Exception
                        Exit Do
                    End Try
                End If

                Application.DoEvents()

                Threading.Thread.Sleep(100)

                FrmProg.Show()
            Loop

            If ProcReport.ExitCode <> 0 Then

            End If

            Call CloseProgress()

            Return ProcReport.ExitCode
        Catch ex As Exception
            Return -100
        End Try
    End Function

    '2010/02/25 学校のレポートを出力する
    Public Function G_ExecReport(ByVal execName As String, Optional ByVal CmdArg As String = "") As Integer
        Dim nRet As Integer = 0

        Try
            Dim ProcReport As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("GCOMMON", "EXE"), execName)
            If CmdArg.Length > 0 Then
                ProcInfo.Arguments = CmdArg
            End If
            ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("GCOMMON", "EXE")
            ProcInfo.UseShellExecute = False
            ProcInfo.CreateNoWindow = True
            ProcInfo.RedirectStandardOutput = True
            ProcReport = Process.Start(ProcInfo)

            Call ShowProgress()

            Do While True
                If ProcReport.HasExited = True Then
                    Exit Do
                Else
                    Dim NowProc As Process
                    Try
                        NowProc = Diagnostics.Process.GetProcessById(ProcReport.Id)
                        If NowProc Is Nothing Then
                            Exit Do
                        End If
                    Catch ex As Exception
                        Exit Do
                    End Try
                End If

                Application.DoEvents()

                Threading.Thread.Sleep(100)

                FrmProg.Show()
            Loop

            If ProcReport.ExitCode <> 0 Then

            End If

            Call CloseProgress()

            Return ProcReport.ExitCode
        Catch ex As Exception
            Return -100
        End Try
    End Function
    '============================================================
    ' 印刷中フォームを表示
    Private Sub ShowProgress()
        FrmProg = New FrmProgress

        If Not Owner Is Nothing Then
            Owner.AddOwnedForm(FrmProg)
        End If

        FrmProg.Show()
    End Sub

    ' プロセスをクローズする
    Private Sub CloseProgress()
        If Not FrmProg Is Nothing Then
            FrmProg.Close()
        End If

        FrmProg = Nothing
    End Sub
End Class
