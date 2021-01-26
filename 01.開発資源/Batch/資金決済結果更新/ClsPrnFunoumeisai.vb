Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 資金決済／手数料徴求不能一覧表
Class ClsPrnFunoumeisai
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "資金決済不能一覧"

        ' 定義体名セット
        reportBaseName = "資金決済手数料徴求不能一覧表.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("回次")
        CSVObject.Output("通番")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("委託者名（漢字）")
        CSVObject.Output("オペコード")
        CSVObject.Output("入出金区分")
        CSVObject.Output("データ区分")
        CSVObject.Output("金額")
        CSVObject.Output("結果")
        CSVObject.Output("エラー内容", False, True)

        Return file
    End Function

    '
    ' 機能　 ： 資金決済／手数料徴求不能一覧表をデータに書き込む
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
