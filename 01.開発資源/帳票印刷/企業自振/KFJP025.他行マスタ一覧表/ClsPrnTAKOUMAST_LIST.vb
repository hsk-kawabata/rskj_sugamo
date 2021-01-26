Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 他行マスタ一覧表表
Class ClsPrnTAKOUMAST_LIST
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP025"

        ' 定義体名セット
        ReportBaseName = "KFJP025_他行マスタ一覧表.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("取引先名")
        CSVObject.Output("金融機関コード")
        CSVObject.Output("金融機関名")
        CSVObject.Output("支店コード")
        CSVObject.Output("支店名")
        CSVObject.Output("科目")
        CSVObject.Output("口座番号")
        CSVObject.Output("委託者コード")
        CSVObject.Output("媒体コード")
        CSVObject.Output("コード区分")
        CSVObject.Output("送信ファイル名")
        CSVObject.Output("受信ファイル名", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 他行マスタ一覧表をデータに書き込む
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
