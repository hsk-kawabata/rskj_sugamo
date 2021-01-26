Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Collections.Generic
' 処理結果確認表(再振データ作成)印刷クラス
Class clsKFJP021
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP021"

        ' 定義体名セット
        ReportBaseName = "KFJP021_処理結果確認表(再振データ作成).rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： 処理結果確認表(再振データ作成)をデータに書き込む
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
