Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 口座振替用納付書送付書
Class ClsPrnNoufusyosouhusyo
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP049"

        ' 定義体名セット
        ReportBaseName = "KFJP049_口座振替用納付書送付書.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("局所番号")
        CSVObject.Output("金融機関郵便番号")
        CSVObject.Output("税目コード")
        CSVObject.Output("納期区分")
        CSVObject.Output("表示納期区分")
        CSVObject.Output("都市区名")
        CSVObject.Output("年度")
        CSVObject.Output("所在地１")
        CSVObject.Output("所在地２")
        CSVObject.Output("肩書")
        CSVObject.Output("金融機関名称１")
        CSVObject.Output("金融機関名称２")
        CSVObject.Output("店舗名称")
        CSVObject.Output("送付分件数")
        CSVObject.Output("送付分合計金額")
        CSVObject.Output("振替納付不能件数")
        CSVObject.Output("振替納付不能合計金額")
        CSVObject.Output("振替納付件数")
        CSVObject.Output("振替納付合計金額")
        CSVObject.Output("発送年月日(年)")
        CSVObject.Output("発送年月日(月)")
        CSVObject.Output("発送年月日(日)")
        CSVObject.Output("税務署名")
        CSVObject.Output("日銀コード", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 口座振替用納付書送付書をデータに書き込む
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
