Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 口座振替請求データ送付票印刷
Public Class ClsPrnKouzafurikaeSeikyuDataInvoice

    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP007"

        ' 定義体名セット
        ReportBaseName = "KFJP007_口座振替請求データ送付票.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("相手金庫名")
        CSVObject.Output("相手電話番号")
        CSVObject.Output("相手ＦＡＸ番号")
        CSVObject.Output("自金庫名")
        CSVObject.Output("担当部署名")
        CSVObject.Output("担当者名")
        CSVObject.Output("電話番号")
        CSVObject.Output("ＦＡＸ番号")
        CSVObject.Output("委託者名")
        CSVObject.Output("振替日")
        CSVObject.Output("処理件数")
        CSVObject.Output("処理金額", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 口座振替請求データ送付票をデータに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
