﻿Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFJP027

    Inherits CAstReports.ClsReportBase

    Sub New(ByVal CSVFileName As String)
        Try
            '定義体名セット
            Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
            ReportBaseName = strLSTPass & "KFJP027_スケジュール進捗管理表.rpd"
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
