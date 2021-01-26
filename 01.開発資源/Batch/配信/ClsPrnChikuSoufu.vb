Imports System.Globalization
Imports System.Text

' nๆ`f[^MA[
Class ClsPrnChikuSoufu
    Inherits CAstReports.ClsReportBase

    Public CsvData(16) As String

    Sub New()
        ' CSVt@CZbg
        InfoReport.ReportName = "KFJP051"

        ' ่`ฬผZbg
        ReportBaseName = "KFJP051_nๆ`f[^MA[.rpd"

        ' ๚
        CsvData(0) = ""                                 ' Sา
        CsvData(1) = ""                                 ' dbิ
        CsvData(2) = ""                                 ' เZ@ึผ
        CsvData(3) = "0"                                ' ๚ชP
        CsvData(4) = "0"                                ' ๚ชQ
        CsvData(5) = "0"                                ' ๚ชR
        CsvData(6) = "0"                                ' ๚ชS
        CsvData(7) = "0"                                ' ๚ชT
        CsvData(8) = "0"                                ' ๚ชU
        CsvData(9) = "0"                                ' ๚ชV
        CsvData(10) = "0"                               ' X๚ชP
        CsvData(11) = "0"                               ' X๚ชQ
        CsvData(12) = "0"                               ' X๚๚ชR
        CsvData(13) = "0"                               ' XชS
        CsvData(14) = "0"                               ' X๚ชT
        CsvData(15) = "0"                               ' X๚ชU
        CsvData(16) = "0"                               ' X๚ชV

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' ^Cgs
        CSVObject.Output("Sา")
        CSVObject.Output("dbิ")
        CSVObject.Output("เZ@ึผ")
        CSVObject.Output("๚ชP")
        CSVObject.Output("๚ชQ")
        CSVObject.Output("๚ชR")
        CSVObject.Output("๚ชS")
        CSVObject.Output("๚ชT")
        CSVObject.Output("๚ชU")
        CSVObject.Output("๚ชV")
        CSVObject.Output("X๚ชP")
        CSVObject.Output("X๚ชQ")
        CSVObject.Output("X๚ชR")
        CSVObject.Output("X๚ชS")
        CSVObject.Output("X๚ชT")
        CSVObject.Output("X๚ชU")
        CSVObject.Output("X๚ชV", False, True)

        Return file

    End Function

    '
    ' @\@ F nๆ`f[^MA[bru๐oอท้
    '
    ' ๕l@ F 
    '
    Public Function OutputCSVKekka(ByVal SyoriKen As String, ByVal Time As String, _
                                   ByVal KINKOBUSYO As String, ByVal KINKOTANTO As String, ByVal KINKOTEL As String, ByVal KINKONAME As String) As Boolean


        Dim SyoriAM As String = ""
        Dim SyoriPM As String = ""

        '2010.02.19 ซEิ๐iniๆ่ๆพ start
        Dim strBorder As String = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
        If strBorder = "err" OrElse strBorder = "" Then
            strBorder = "1200"
        End If

        If Time < strBorder Then
            SyoriAM = SyoriKen
        Else
            SyoriPM = SyoriKen
        End If
        '2010.02.19 ซEิ๐iniๆ่ๆพ end

        Try
            '2010/01/20 วม =====
            'CsvData(0) = KINKOTANTO
            CsvData(0) = (KINKOBUSYO & " " & KINKOTANTO).Trim
            '=========================
            CsvData(1) = KINKOTEL
            CsvData(2) = KINKONAME
            CsvData(3) = SyoriAM.PadLeft(7, "0"c).Substring(0, 1)
            CsvData(4) = SyoriAM.PadLeft(7, "0"c).Substring(1, 1)
            CsvData(5) = SyoriAM.PadLeft(7, "0"c).Substring(2, 1)
            CsvData(6) = SyoriAM.PadLeft(7, "0"c).Substring(3, 1)
            CsvData(7) = SyoriAM.PadLeft(7, "0"c).Substring(4, 1)
            CsvData(8) = SyoriAM.PadLeft(7, "0"c).Substring(5, 1)
            CsvData(9) = SyoriAM.PadLeft(7, "0"c).Substring(6, 1)
            CsvData(10) = SyoriPM.PadLeft(7, "0"c).Substring(0, 1)
            CsvData(11) = SyoriPM.PadLeft(7, "0"c).Substring(1, 1)
            CsvData(12) = SyoriPM.PadLeft(7, "0"c).Substring(2, 1)
            CsvData(13) = SyoriPM.PadLeft(7, "0"c).Substring(3, 1)
            CsvData(14) = SyoriPM.PadLeft(7, "0"c).Substring(4, 1)
            CsvData(15) = SyoriPM.PadLeft(7, "0"c).Substring(5, 1)
            CsvData(16) = SyoriPM.PadLeft(7, "0"c).Substring(6, 1)

            CSVObject.Output(CsvData)

            Return True

        Catch ex As Exception
            MainLOG.Write("nๆ`f[^MA[CSV์ฌ", "ธs", ex.Message)
            Return False
        Finally
        End Try
    End Function

    'Public Overloads Overrides Function ReportExecute() As Boolean
    '    Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
    'End Function
End Class
