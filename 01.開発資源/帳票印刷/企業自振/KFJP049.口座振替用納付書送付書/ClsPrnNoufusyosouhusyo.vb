Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' ûÀUÖp[tt
Class ClsPrnNoufusyosouhusyo
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVt@CZbg
        InfoReport.ReportName = "KFJP049"

        ' è`Ì¼Zbg
        ReportBaseName = "KFJP049_ûÀUÖp[tt.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("ÇÔ")
        CSVObject.Output("àZ@ÖXÖÔ")
        CSVObject.Output("ÅÚR[h")
        CSVObject.Output("[úæª")
        CSVObject.Output("\¦[úæª")
        CSVObject.Output("ssæ¼")
        CSVObject.Output("Nx")
        CSVObject.Output("ÝnP")
        CSVObject.Output("ÝnQ")
        CSVObject.Output("¨")
        CSVObject.Output("àZ@Ö¼ÌP")
        CSVObject.Output("àZ@Ö¼ÌQ")
        CSVObject.Output("XÜ¼Ì")
        CSVObject.Output("tª")
        CSVObject.Output("tªvàz")
        CSVObject.Output("UÖ[ts\")
        CSVObject.Output("UÖ[ts\vàz")
        CSVObject.Output("UÖ[t")
        CSVObject.Output("UÖ[tvàz")
        CSVObject.Output("­Nú(N)")
        CSVObject.Output("­Nú()")
        CSVObject.Output("­Nú(ú)")
        CSVObject.Output("Å±¼")
        CSVObject.Output("úâR[h", False, True)

        Return file
    End Function

    '
    ' @\@ F ûÀUÖp[ttðf[^É«Þ
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
