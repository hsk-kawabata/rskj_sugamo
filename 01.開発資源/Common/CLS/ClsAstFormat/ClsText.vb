Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Collections.Specialized

Public Class ClsText
    Private TextData As StringDictionary
    Private TextPath As String = CASTCommon.GetFSKJIni("COMMON", "TXT")

    ' 何も指定しないでＮＥＷした場合は，媒体コードを取得する
    Public Sub New()
        MyClass.New("Common_媒体コード.TXT")
    End Sub

    ' 汎用
    ' 指定がある場合は，指定のテキストデータを取得する
    Public Sub New(ByVal text As String)
        Try
            TextData = GetTextData(text)
        Catch
        End Try
    End Sub

    Private Function GetTextData(ByVal name As String) As StringDictionary
        If File.Exists(Path.Combine(TextPath, name)) = False Then
            Return Nothing
        End If

        Using rstm As New StreamReader(Path.Combine(TextPath, name), Encoding.GetEncoding("SHIFT-JIS"))
            Dim textdata As New StringDictionary

            While rstm.Peek > -1
                Dim line() As String = rstm.ReadLine().Split(","c)

                If line.Length >= 2 Then
                    Dim no As String
                    Try
                        no = Long.Parse(line(0).Trim).ToString
                    Catch
                        no = line(0).Trim
                    End Try

                    If textdata.ContainsKey(no.Trim) = False Then
                        textdata.Add(no.Trim, line(1).Trim)
                    End If
                End If

            End While

            Return textdata
        End Using
    End Function

    Public Function GetBaitaiCode(ByVal key As String) As String
        Return GetText(key)
    End Function

    Public Function GetBaitaiCode(ByVal key As Long) As String
        Return GetText(key)
    End Function

    Public Function GetText(ByVal key As String) As String
        If TextData Is Nothing Then
            Return ""
        End If

        Dim no As String
        Try
            no = Long.Parse(key).ToString
        Catch
            no = key
        End Try

        If TextData.ContainsKey(no) Then
            Return TextData.Item(no)
        End If

        Return ""
    End Function

    Public Function GetText(ByVal key As Long) As String
        If TextData Is Nothing Then
            Return ""
        End If

        Dim no As String = key.ToString

        If TextData.ContainsKey(no) Then
            Return TextData.Item(no)
        End If

        Return ""
    End Function
End Class
