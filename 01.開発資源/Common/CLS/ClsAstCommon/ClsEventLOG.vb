Imports System
Imports System.Diagnostics

Public Class ClsEventLOG

    Private MyLog As EventLog

    Public Sub New()
        'If Not EventLog.SourceExists(Process.GetCurrentProcess.ProcessName) Then
        '    EventLog.CreateEventSource(Process.GetCurrentProcess.ProcessName, "")
        'End If

        MyLog = New EventLog
        MyLog.Source = Process.GetCurrentProcess.ProcessName
    End Sub

    Public Sub Write(ByVal message As String, Optional ByVal type As EventLogEntryType = EventLogEntryType.Information)
        Try
            MyLog.WriteEntry(message, type)
        Catch ex As Exception

        End Try
    End Sub

    Protected Overrides Sub Finalize()
        Try
            MyLog.Close()
        Catch ex As Exception

        End Try

        MyBase.Finalize()
    End Sub
End Class
