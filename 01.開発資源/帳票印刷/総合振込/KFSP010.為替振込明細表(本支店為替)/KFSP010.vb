Option Strict On
Option Explicit On

Imports System

Public Class KFSP010

    Inherits CAstReports.ClsReportBase

    '2016/10/21 saitou RSV2 UPD 信組対応 ---------------------------------------- START
    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <param name="CSVFileName">CSVファイル名</param>
    ''' <param name="SousinKbn">送信区分（2 - MT代行 , 2 以外 - MT代行以外)</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal CSVFileName As String, SousinKbn As String)
        Try
            '定義体名セット
            Dim strLSTPath As String = CASTCommon.GetFSKJIni("COMMON", "LST")

            If SousinKbn = "2" Then
                ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP010_為替振込明細表(本支店為替)(MT代行).rpd")
            Else
                ReportBaseName = System.IO.Path.Combine(strLSTPath, "KFSP010_為替振込明細表(本支店為替).rpd")
            End If

            'ＣＳＶファイル名セット(フルパス)
            MyBase.CsvName = CSVFileName

        Catch ex As Exception

        End Try
    End Sub

    'Sub New(ByVal CSVFileName As String)
    '    Try
    '        '定義体名セット
    '        Dim strLSTPass As String = CASTCommon.GetFSKJIni("COMMON", "LST")
    '        ReportBaseName = strLSTPass & "KFSP010_為替振込明細表(本支店為替).rpd"
    '        'ＣＳＶファイル名セット(フルパス)
    '        MyBase.CsvName = CSVFileName
    '    Catch ex As Exception
    '    End Try
    'End Sub
    '2016/10/21 saitou RSV2 UPD ------------------------------------------------- END

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
