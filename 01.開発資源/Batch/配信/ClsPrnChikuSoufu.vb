Imports System.Globalization
Imports System.Text

' 地区伝送データ送信連絡票
Class ClsPrnChikuSoufu
    Inherits CAstReports.ClsReportBase

    Public CsvData(16) As String

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP051"

        ' 定義体名セット
        ReportBaseName = "KFJP051_地区伝送データ送信連絡票.rpd"

        ' 処理日
        CsvData(0) = ""                                 ' 担当者
        CsvData(1) = ""                                 ' 電話番号
        CsvData(2) = ""                                 ' 金融機関名
        CsvData(3) = "0"                                ' 翌日分件数１
        CsvData(4) = "0"                                ' 翌日分件数２
        CsvData(5) = "0"                                ' 翌日分件数３
        CsvData(6) = "0"                                ' 翌日分件数４
        CsvData(7) = "0"                                ' 翌日分件数５
        CsvData(8) = "0"                                ' 翌日分件数６
        CsvData(9) = "0"                                ' 翌日分件数７
        CsvData(10) = "0"                               ' 翌々日分件数１
        CsvData(11) = "0"                               ' 翌々日分件数２
        CsvData(12) = "0"                               ' 翌々日日分件数３
        CsvData(13) = "0"                               ' 翌々分件数４
        CsvData(14) = "0"                               ' 翌々日分件数５
        CsvData(15) = "0"                               ' 翌々日分件数６
        CsvData(16) = "0"                               ' 翌々日分件数７

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' タイトル行
        CSVObject.Output("担当者")
        CSVObject.Output("電話番号")
        CSVObject.Output("金融機関名")
        CSVObject.Output("翌日分件数１")
        CSVObject.Output("翌日分件数２")
        CSVObject.Output("翌日分件数３")
        CSVObject.Output("翌日分件数４")
        CSVObject.Output("翌日分件数５")
        CSVObject.Output("翌日分件数６")
        CSVObject.Output("翌日分件数７")
        CSVObject.Output("翌々日分件数１")
        CSVObject.Output("翌々日分件数２")
        CSVObject.Output("翌々日分件数３")
        CSVObject.Output("翌々日分件数４")
        CSVObject.Output("翌々日分件数５")
        CSVObject.Output("翌々日分件数６")
        CSVObject.Output("翌々日分件数７", False, True)

        Return file

    End Function

    '
    ' 機能　 ： 地区伝送データ送信連絡票ＣＳＶを出力する
    '
    ' 備考　 ： 
    '
    Public Function OutputCSVKekka(ByVal SyoriKen As String, ByVal Time As String, _
                                   ByVal KINKOBUSYO As String, ByVal KINKOTANTO As String, ByVal KINKOTEL As String, ByVal KINKONAME As String) As Boolean


        Dim SyoriAM As String = ""
        Dim SyoriPM As String = ""

        '2010.02.19 境界時間をiniより取得 start
        Dim strBorder As String = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
        If strBorder = "err" OrElse strBorder = "" Then
            strBorder = "1200"
        End If

        If Time < strBorder Then
            SyoriAM = SyoriKen
        Else
            SyoriPM = SyoriKen
        End If
        '2010.02.19 境界時間をiniより取得 end

        Try
            '2010/01/20 部署追加 =====
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
            MainLOG.Write("地区伝送データ送信連絡票CSV作成", "失敗", ex.Message)
            Return False
        Finally
        End Try
    End Function

    'Public Overloads Overrides Function ReportExecute() As Boolean
    '    Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
    'End Function
End Class
