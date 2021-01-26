Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class ClsPrnKouzafurikaeiraisyo
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "Kouzafurikaeiraisyo"

        ' 定義体名セット
        reportBaseName = "口座振替依頼書.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("取りまとめ金融機関名")
        CSVObject.Output("取りまとめ支店名")
        CSVObject.Output("取扱金融機関コード")
        CSVObject.Output("取扱支店コード")
        CSVObject.Output("取扱金融機関名")
        CSVObject.Output("取扱支店名")
        CSVObject.Output("委託者コード")
        CSVObject.Output("委託者名")
        CSVObject.Output("振替日")
        CSVObject.Output("入出区分")
        CSVObject.Output("入出取引先主コード")
        CSVObject.Output("入出取引先副コード")
        CSVObject.Output("引落金融機関コード")
        CSVObject.Output("引落金融機関名")
        CSVObject.Output("引落支店コード")
        CSVObject.Output("引落支店名")
        CSVObject.Output("契約者名カナ")
        CSVObject.Output("契約者番号")
        CSVObject.Output("科目")
        CSVObject.Output("口座番号")
        CSVObject.Output("引落金額", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 口座振替依頼書をデータに書き込む
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
