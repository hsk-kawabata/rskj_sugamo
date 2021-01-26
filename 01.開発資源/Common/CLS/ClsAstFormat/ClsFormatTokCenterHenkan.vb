Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' 金庫渡しＭＴ東海センターフォーマット（返還）クラス
Public Class CFormatTokCenterHenkan
    ' データフォーマット基本クラス
    Inherits CFormatZengin

    Public Overrides Function CheckRecord1() As String
        If RecordData.Length = 640 Then
            RecordData = RecordData.Substring(88, 120)

            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Buffer = ReadByteBin.GetBytes(88, 120)
            End If
        End If
        Return MyBase.CheckRecord1
    End Function

    Public Overrides Sub GetHenkanDataRecord()
        If RecordData.Length = 640 Then
            RecordData = RecordData.Substring(88, 120)

            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Buffer = ReadByteBin.GetBytes(88, 120)
            End If
        End If
        Call MyBase.GetHenkanDataRecord()
    End Sub

    Public Overrides Sub GetHenkanTrailerRecord()
        If RecordData.Length = 640 Then
            RecordData = RecordData.Substring(88, 120)

            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Buffer = ReadByteBin.GetBytes(88, 120)
            End If
        End If
        Call MyBase.GetHenkanTrailerRecord()
    End Sub

    Public Overrides Function IsHeaderRecord() As Boolean
        If RecordData.Length = 640 Then
            For i As Integer = 0 To HeaderKubun.Length - 1
                If RecordData.Substring(88, 1) = HeaderKubun(i) Then
                    Return True
                End If
            Next i
            Return False
        Else
            Return MyBase.IsHeaderRecord
        End If
    End Function

    Public Overrides Function IsDataRecord() As Boolean
        If RecordData.Length = 640 Then
            For i As Integer = 0 To DataKubun.Length - 1
                If RecordData.Substring(88, 1) = DataKubun(i) Then
                    Return True
                End If
            Next i
            Return False
        Else
            Return MyBase.IsDataRecord
        End If
    End Function

    Public Overrides Function IsTrailerRecord() As Boolean
        If RecordData.Length = 640 Then
            For i As Integer = 0 To TrailerKubun.Length - 1
                If RecordData.Substring(88, 1) = TrailerKubun(i) Then
                    Return True
                End If
            Next i
            Return False
        Else
            Return MyBase.IsTrailerRecord
        End If
    End Function

    Public Overrides Function IsEndRecord() As Boolean
        If RecordData.Length = 640 Then
            If RecordData.Substring(88, 1) = DataInfo.MinRecordCode(DataInfo.MinRecordCode.Length - 1) Then
                Return True
            End If
            Return False
        Else
            Return MyBase.IsEndRecord
        End If
    End Function

End Class
