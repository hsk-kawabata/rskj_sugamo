Imports System.Text

Module ModSubroutine

    Public Function GetPrivateProfileSectionNames( _
       ByVal lpFileName As String) As String()
        Dim str As String = New String(" ", 10240)
        Dim len As Integer = 10240
        Call GetIniSectionNames(str, len, lpFileName)

        Dim s As New StringBuilder

        Return str.Split(Chr(0))
    End Function

    <System.Runtime.InteropServices.DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileSectionNames", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
    Public Function GetIniSectionNames( _
            ByVal lpszReturnBuffer As String, _
            ByVal nSize As Integer, _
            ByVal lpFileName As String) As Integer
    End Function

End Module
