﻿Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFJP023

    Inherits CAstReports.ClsReportBase

    Sub New(ByVal CSVFileName As String)
        Try
            '定義体名セット
            Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
            ReportBaseName = strLSTPass & "KFJP023_取引先マスタメンテ(変更).rpd"
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
