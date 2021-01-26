Option Strict On
Option Explicit On

Imports System

Public Class KFSP009

    Inherits CAstReports.ClsReportBase

    '2016/02/04 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- START
    '金バッチ使用フラグによって使用する帳票定義体を変更する
    Public Sub New(ByVal CSVFileName As String, _
                   ByVal KinBatchFlg As String)
        Try
            '定義体名セット
            Dim strLSTPath As String = CASTCommon.GetFSKJIni("COMMON", "LST")
            Select Case KinBatchFlg
                Case "1"
                    ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP009_振込発信データ対象一覧.rpd")
                Case "2"
                    ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP009_振込発信データ対象一覧(MT代行).rpd")
                Case "3"
                    ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP009_振込発信CSVリエンタ対象一覧.rpd")
                Case "4"
                    ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP009_振込発信データ対象一覧(通常定例).rpd")
                Case "5"
                    ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP009_振込発信データ対象一覧(当日異例).rpd")
                Case Else
                    ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP009_振込発信リエンタ対象一覧.rpd")
            End Select
            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName

        Catch ex As Exception

        End Try
    End Sub

    'Sub New(ByVal CSVFileName As String)
    '    Try
    '        '定義体名セット
    '        Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
    '        ReportBaseName = strLSTPass & "KFSP009_振込発信リエンタ対象一覧.rpd"
    '        'ＣＳＶファイル名セット(フルパス)
    '        MyBase.CsvName = CSVFileName
    '    Catch ex As Exception
    '    End Try
    'End Sub
    '2016/02/04 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- END

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
