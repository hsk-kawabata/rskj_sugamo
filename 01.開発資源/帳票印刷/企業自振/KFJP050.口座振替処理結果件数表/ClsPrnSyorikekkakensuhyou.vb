Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' ûÀUÖÊ\
Class ClsPrnSyorikekkakensuhyou
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVt@CZbg
        InfoReport.ReportName = "KFJP050"

        ' è`Ì¼Zbg
        ReportBaseName = "KFJP050_ûÀUÖÊ\.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("ú")
        CSVObject.Output("èæ¼")
        CSVObject.Output("èæS")
        CSVObject.Output("©àÉ¼")
        CSVObject.Output("Nx")
        CSVObject.Output("ú")
        CSVObject.Output("UÖú")
        CSVObject.Output("Å±¼")
        CSVObject.Output("ÇÔ")
        CSVObject.Output("tª")
        CSVObject.Output("tªvàz")
        CSVObject.Output("UÖ[ts\")
        CSVObject.Output("UÖ[ts\vàz")
        CSVObject.Output("UÖ[t")
        CSVObject.Output("UÖ[tvàz", False, True)

        Return file
    End Function

    '
    ' @\@ F ûÀUÖÊ\ðf[^É«Þ
    '
    ' õl@ F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
