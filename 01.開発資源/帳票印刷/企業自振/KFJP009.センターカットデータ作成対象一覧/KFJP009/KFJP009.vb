Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFJP009

    Inherits CAstReports.ClsReportBase

    Sub New(ByVal CSVFileName As String)
        Try
            '定義体名セット
            Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
            '2010/12/24 信組対応 帳票定義変更
            If CASTCommon.GetFSKJIni("COMMON", "CENTER") = "0" Then
                ReportBaseName = strLSTPass & "KFJP009_センターカットデータ作成対象一覧(信組).rpd"
            Else
                ReportBaseName = strLSTPass & "KFJP009_センターカットデータ作成対象一覧.rpd"
            End If
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
