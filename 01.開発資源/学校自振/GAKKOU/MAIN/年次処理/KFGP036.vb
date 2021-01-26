Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFGP036
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal GAKKOU_CODE As String)
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP036"
        InfoReport.Itakusyamei = GAKKOU_CODE
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： 生徒マスタ整合性チェックリストをデータに書き込む
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
