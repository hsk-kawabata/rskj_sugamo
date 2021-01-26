Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 口座振替処理結果件数表
Class ClsPrnSyorikekkakensuhyou
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP050"

        ' 定義体名セット
        ReportBaseName = "KFJP050_口座振替処理結果件数表.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("処理日")
        CSVObject.Output("相手先名")
        CSVObject.Output("相手先担当部署")
        CSVObject.Output("自金庫名")
        CSVObject.Output("年度")
        CSVObject.Output("期")
        CSVObject.Output("振替日")
        CSVObject.Output("税務署名")
        CSVObject.Output("局所番号等")
        CSVObject.Output("送付分件数")
        CSVObject.Output("送付分合計金額")
        CSVObject.Output("振替納付不能件数")
        CSVObject.Output("振替納付不能合計金額")
        CSVObject.Output("振替納付件数")
        CSVObject.Output("振替納付合計金額", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 口座振替処理結果件数表をデータに書き込む
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
