Option Strict On
Option Explicit On

Imports System

Public Class KFSP019

    Inherits CAstReports.ClsReportBase

    Sub New(ByVal CSVFileName As String)
        Try
            '定義体名セット
            Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
            ReportBaseName = strLSTPass & "KFSP019_取引先マスタメンテ(削除).rpd"
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
