﻿Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFJP008

    Inherits CAstReports.ClsReportBase

    Sub New(ByVal CSVFileName As String)
        Try
            '定義体名セット
            Dim strLSTPass As String = CASTCommon.GetFSKJIni("GCOMMON", "LST")
            ReportBaseName = strLSTPass & "KFGP008_学校マスタメンテ(削除).rpd"
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
