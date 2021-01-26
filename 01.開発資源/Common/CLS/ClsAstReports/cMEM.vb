Imports System
Imports System.IO
Imports System.Text

Public Class MEM
    ' ファイル
    Private IOFile As MemoryStream
    Private Reader As StreamReader
    Public Message As String
    Public FileName As String

    ' New
    Public Sub New()
    End Sub

    ' ファイルを開く
    Public Function Open() As Integer
        Try
            IOFile = New MemoryStream
            Reader = New StreamReader(IOFile, Encoding.GetEncoding("SHIFT-JIS"))
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' Output
    Public Function Output() As Integer
        Try
            Dim btCrLf() As Byte = {13, 10}
            IOFile.Write(btCrLf, 0, btCrLf.Length)
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' Output
    Public Function Output(ByVal data As Long, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False) As Integer
        Dim sData As String
        Try
            sData = data.ToString
        Catch ex As Exception
            sData = "0"
        End Try
        Return Output(sData, dq, crlf)
    End Function

    ' Output
    Public Function Output(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False) As Integer
        Dim e As Encoding = Encoding.GetEncoding("SHIFT-JIS")

        If data Is Nothing Then
            data = ""
        End If

        Try
            If dq = True Then
                data = CASTCommon.EncloseValue(data)
            End If
            Dim bt() As Byte = e.GetBytes(data)
            IOFile.Write(bt, 0, bt.Length)

            If crlf = True Then
                Dim btCrLf() As Byte = {13, 10}
                IOFile.Write(btCrLf, 0, btCrLf.Length)
            Else
                Dim btComma() As Byte = e.GetBytes(",")
                IOFile.Write(btComma, 0, btComma.Length)
            End If
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' Output
    Public Function OutputPlus(ByVal ParamArray data() As String) As Integer
        For i As Integer = 0 To data.Length - 1
            data(i) = CASTCommon.EncloseValue(data(i))
        Next i
        Return Output(data)
    End Function

    ' Output
    Public Function Output(ByVal ParamArray data() As String) As Integer
        Dim e As Encoding = Encoding.GetEncoding("SHIFT-JIS")

        Try
            Dim sJoin As String = String.Join(",", data)
            Dim bt() As Byte = e.GetBytes(sJoin)
            IOFile.Write(bt, 0, bt.Length)

            Dim btCrLf() As Byte = {13, 10}
            IOFile.Write(btCrLf, 0, 2)
        Catch ex As Exception
            Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    ' Close
    Public Function Close() As Integer
        Try
            If Not IOFile Is Nothing Then
                IOFile.Close()
            End If
        Catch ex As Exception

        End Try
    End Function

    Public Function Seek(ByVal offset As Long) As Long
        Return IOFile.Seek(0, SeekOrigin.Begin)
    End Function

    Public Function ReadLine() As String
        Return Reader.ReadLine()
    End Function

End Class
