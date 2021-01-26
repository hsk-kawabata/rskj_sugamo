Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 振替不能事由別集計表
Class ClsPrnFurikaeFunouSyuukei
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP052"

        ' 定義体名セット
        ReportBaseName = "KFJP052_振替不能事由別集計表.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("振替日")
        CSVObject.Output("委託者名漢字")
        CSVObject.Output("委託者コード")
        CSVObject.Output("自金庫名")
        CSVObject.Output("とりまとめ店コード")
        CSVObject.Output("とりまとめ店名漢字")
        CSVObject.Output("振替結果コード")
        CSVObject.Output("振替金額")
        CSVObject.Output("処理日", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 振替不能事由別集計表をデータに書き込む
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
