Option Strict On
Option Explicit On

Imports System.IO

' �W���u�Ď����C�����W���[��
Public Module ModMain

    Public Const msgTitle As String = "�W���u�Ď����(KFUJOBW010)"

    Sub Main()
        Try
            '�d���N���`�F�b�N
            Dim FileName As String = Path.GetFileNameWithoutExtension(Diagnostics.Process.GetCurrentProcess.ProcessName).ToUpper

            Dim Counter As Integer = _
                Diagnostics.Process.GetProcessesByName(FileName).GetUpperBound(0)

            If Counter >= 0 Then

                Dim arrProcess() As Process = _
                    Diagnostics.Process.GetProcessesByName(FileName)

                '���������O�C����ʂ��A�N�e�B�u�ɂ���
                For Index As Integer = 0 To arrProcess.Length - 1

                    If Diagnostics.Process.GetCurrentProcess.Id <> arrProcess(Index).Id() Then

                        Microsoft.VisualBasic.AppActivate(arrProcess(Index).Id())

                        Return
                    End If
                Next Index
            End If

            '۸޲݉�ʕ\��
            Dim MainDisp As New FrmJOB
            MainDisp.Show()

            Application.Run(MainDisp)

        Catch ex As Exception

        End Try
    End Sub

End Module
