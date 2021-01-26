Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFGP108

    Inherits CAstReports.ClsReportBase
    Sub New(ByVal CSVFileName As String)
        Try
            Dim strLSTPass As String = CASTCommon.GetFSKJIni("GCOMMON", "LST")
            ReportBaseName = strLSTPass & "KFGP106,108_生徒マスタメンテ.rpd"
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception

        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
