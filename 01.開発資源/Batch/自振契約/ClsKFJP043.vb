Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.IO
Imports System.Collections
Imports CAstBatch

Class ClsKFJP043
    Inherits CAstReports.ClsReportBase

    Public CsvData(12) As String

    Private strKESSAI_DATE As String                    ' Ïú

    Sub New()
        ' CSVt@CZbg
        InfoReport.ReportName = "KFJP043"

        ' è`Ì¼Zbg
        ReportBaseName = "KFJP043_ÊmF\(©U_ñ).rpd"

        CsvData(0) = "00010101"                                     ' ú
        CsvData(1) = "00010101"                                     ' ^CX^v
        CsvData(2) = ""                                             ' UÖR[h
        CsvData(3) = ""                                             ' éÆR[h
        CsvData(4) = ""                                             ' æøæåR[h
        CsvData(5) = ""                                             ' æøæR[h
        CsvData(6) = ""                                             ' æøæ¼
        CsvData(7) = ""                                             ' _ñÒxXR[h
        CsvData(8) = ""                                             ' _ñÒxX¼
        CsvData(9) = ""                                             ' _ñÒÈÚ
        CsvData(10) = ""                                            ' _ñÒûÀÔ
        CsvData(11) = ""                                            ' _ñÒJi¼
        CsvData(12) = ""                                            ' õl

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' ^Cgs
        CSVObject.Output("ú")
        CSVObject.Output("^CX^v")
        CSVObject.Output("UÖR[h")
        CSVObject.Output("éÆR[h")
        CSVObject.Output("æøæåR[h")
        CSVObject.Output("æøæR[h")
        CSVObject.Output("æøæ¼")
        CSVObject.Output("_ñÒxXR[h")
        CSVObject.Output("_ñÒxX¼")
        CSVObject.Output("_ñÒÈÚ")
        CSVObject.Output("_ñÒûÀÔ")
        CSVObject.Output("_ñÒJi¼")
        CSVObject.Output("õl", False, True)

        '2012/01/13 saitou WC³ xñð MODIFY ---------------------------->>>>
        Return file
        '2012/01/13 saitou WC³ xñð MODIFY ----------------------------<<<<

    End Function

    '
    ' @\@ F ©U_ñÊmF\bruðoÍ·é
    '
    ' õl@ F 
    '
    Public Function OutputCSVKekka(ByVal ary As ArrayList, ByVal jikinko As String, ByVal strDate As String, ByVal strTime As String) As Integer


        Dim JData As New CAstFormKes.ClsFormKes.JifkeiData
        Dim fmt10004 As New CAstFormKes.ClsFormSikinFuri.T_10004

        Dim cnt As Integer = ary.Count - 1 '[vñ

        Try

            For i As Integer = 0 To cnt

                JData.Init()
                fmt10004.Init()

                JData = CType(ary.Item(i), CAstFormKes.ClsFormKes.JifkeiData)
                fmt10004.Data = JData.record320

                ' bruf[^Ýè
                CsvData(0) = strDate                                                ' ú
                CsvData(1) = strTime                                                ' ^CX^v
                CsvData(2) = JData.FuriCode                                         ' UÖR[h
                CsvData(3) = JData.KigyoCode                                        ' éÆR[h
                CsvData(4) = JData.TorisCode                                        ' æøæåR[h
                CsvData(5) = JData.TorifCode                                        ' æøæR[h
                CsvData(6) = JData.ToriNName.Trim                                   ' æøæ¼
                CsvData(7) = fmt10004.GENTEN_NO                                     ' _ñÒxXR[h
                CsvData(8) = GetTenmast(jikinko, fmt10004.GENTEN_NO, MainDB)        ' _ñÒxX¼
                CsvData(9) = fmt10004.KAMOKU_KOUZA_NO.Substring(0, 2)               ' _ñÒÈÚ
                CsvData(10) = fmt10004.KAMOKU_KOUZA_NO.Substring(2, 7)              ' _ñÒûÀÔ
                CsvData(11) = JData.KeiyakuKname                                    ' _ñÒJi¼
                CsvData(12) = ""                                                    ' õl

                'bruoÍ
                If CSVObject.Output(CsvData) = 0 Then
                    Return -1
                End If

            Next

        Catch ex As Exception

            'MainLOG.Write("ÊmF\(©U_ñ)bruoÍ", "¸s", ex.Message)
            Return -1
        Finally

        End Try

        Return 0

    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strSitName As String = ""

        Try

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")

            If orareader.DataReader(sql) = True Then
                strSitName = orareader.GetString("SIT_NNAME_N")
                Return strSitName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

End Class
