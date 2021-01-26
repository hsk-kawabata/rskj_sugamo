Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

'年金振込支店コードチェックリスト
Class ClsPrnNenSitCheck
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP040"
        ' 定義体名セット
        ReportBaseName = "KFJP040_年金振込支店コードチェックリスト.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function

    '=======================================================================
    ' 機能　 ： 年金振込支店コードチェックリストをデータに書き込む
    ' 備考　 ： 
    ' 振替日 ： 2009/09/29
    ' 更新日 ：
    '=======================================================================
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
