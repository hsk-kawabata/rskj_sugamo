Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class clsKFUP001CSV
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFUP001"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' タイトル行
        CSVObject.Output("処理日", True)
        CSVObject.Output("タイムスタンプ", True)
        CSVObject.Output("伝送日付", True)
        CSVObject.Output("項番", True)
        CSVObject.Output("開始時間", True)
        CSVObject.Output("終了時間", True)
        CSVObject.Output("センター名", True)
        CSVObject.Output("ファイル名", True)
        CSVObject.Output("送受信", True)
        CSVObject.Output("レコード件数", True, True)

        Return file
    End Function

    '
    ' 機能　 ： 振替結果明細表をデータに書き込む
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
