Imports System

Public Class Calendar
    'Private Shared DtY As String = GetIni("DT.INI", "DEBUG", "DATE-Y")
    'Private Shared DtM As String = GetIni("DT.INI", "DEBUG", "DATE-M")
    'Private Shared DtD As String = GetIni("DT.INI", "DEBUG", "DATE-D")

    Public Shared Function Now() As DateTime
        'If DtY = "err" Then
        '    Return DateTime.Now
        'End If
        'Return New DateTime(DtY, DtM, DtD, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond)
        Return DateTime.Now
    End Function
End Class
