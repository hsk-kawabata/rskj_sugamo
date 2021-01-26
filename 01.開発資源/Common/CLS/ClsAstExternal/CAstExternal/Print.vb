' 2016/01/08 タスク）岩城 ADD 【PG】UI-05 --------------------START
Public Class Print
    Inherits CAstReports.ClsReportBase

    Public Sub New(ByVal PrtID As String)
        'CSVファイルセット
        InfoReport.ReportName = PrtID
    End Sub

    Public Sub New(ByVal PrtID As String, ByVal RpdBaseName As String, ByVal CSVFileName As String)
        'CSVファイルセット
        InfoReport.ReportName = PrtID
        MyBase.ReportBaseName = RpdBaseName
        MyBase.CsvName = CSVFileName
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '
    ' 機能　 ： CSVファイルに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub
End Class
' 2016/01/08 タスク）岩城 ADD 【PG】UI-05 --------------------END
