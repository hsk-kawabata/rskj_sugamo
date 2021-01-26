Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 領収控
Class ClsPrnRyousyuhikae
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP048"

        ' 定義体名セット
        ReportBaseName = "KFJP048_領収控.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("自金庫名")
        CSVObject.Output("税目コード")
        CSVObject.Output("納期区分カナ文字")
        CSVObject.Output("期")
        CSVObject.Output("年度")
        CSVObject.Output("税務署コード")
        CSVObject.Output("税務署名")
        CSVObject.Output("日銀コード")
        CSVObject.Output("振替日")
        CSVObject.Output("自課税期間(年)")
        CSVObject.Output("自課税期間(月)")
        CSVObject.Output("自課税期間(日)")
        CSVObject.Output("至課税期間(年)")
        CSVObject.Output("至課税期間(月)")
        CSVObject.Output("至課税期間(日)")
        CSVObject.Output("レコード番号")
        CSVObject.Output("都市区名")
        CSVObject.Output("住所１")
        CSVObject.Output("住所２")
        CSVObject.Output("住所３")
        CSVObject.Output("納税者名")
        CSVObject.Output("整理番号１")
        CSVObject.Output("整理番号２")
        CSVObject.Output("納付税額")
        CSVObject.Output("内利子税")
        CSVObject.Output("振替金融機関コード")
        CSVObject.Output("振替支店コード")
        CSVObject.Output("金融機関整理欄")
        CSVObject.Output("領収欄文言１")
        CSVObject.Output("領収欄文言２")
        CSVObject.Output("領収欄文言３")
        CSVObject.Output("領収欄文言４", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 領収控をデータに書き込む
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
