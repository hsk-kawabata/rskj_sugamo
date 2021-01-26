' 処理結果確認表
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    Public CsvData(12) As String

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "他シス処理結果確認表"

        ' 定義体名セット
        ReportBaseName = "他シス処理結果確認表.rpd"

        ' 処理日
        CsvData(0) = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        CsvData(1) = "00010101"                         ' 振替日
        CsvData(2) = "0"                                ' 取引先主コード
        CsvData(3) = "0"                                ' 取引先副コード
        CsvData(4) = ""                                 ' 取引先名
        CsvData(5) = ""                                 ' 摘要
        CsvData(6) = "0"                                ' 依頼件数
        CsvData(7) = "0"                                ' 依頼金額
        CsvData(8) = "0"                                ' 処理件数
        CsvData(9) = "0"                                ' 処理金額
        CsvData(10) = ""                                ' 備考
        CsvData(11) = ""                                ' 区分
        CsvData(12) = ""                                ' 振替処理区分
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("振替日")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("取引先名")
        CSVObject.Output("摘要")
        CSVObject.Output("依頼件数")
        CSVObject.Output("依頼金額")
        CSVObject.Output("処理件数")
        CSVObject.Output("処理金額")
        CSVObject.Output("備考")
        CSVObject.Output("区分")
        CSVObject.Output("振替処理区分", False, True)
    End Function

    Public Function OutputCSVKekkaSysError(ByVal aItakuCode As String, ByVal fSyoriKbn As String, ByVal outFileName As String, ByVal oraDB As CASTCommon.MyOracle) As Boolean
        ' ＣＳＶファイル作成
        Itakusyamei = "ERROR"
        CreateCsvFile()

        CsvData(2) = aItakuCode
        CsvData(3) = ""
        CsvData(11) = "1"

        CsvData(12) = fSyoriKbn                 ' 振替処理区分

        ' ０行目
        CsvData(4) = ""
        CSVObject.Output(CsvData)

        ' １行目
        CsvData(2) = "0"
        CsvData(3) = "0"
        CsvData(4) = "連携処理できませんでした "
        CSVObject.Output(CsvData)

        ' ２行目
        CsvData(4) = "ファイル名：" & outFileName
        CSVObject.Output(CsvData)

        ' ３行目
        CsvData(4) = "取引先情報取得失敗"
        CSVObject.Output(CsvData)

        If ReportExecute() = False Then
            Return False
        End If

        Return True
    End Function

    '
    ' 機能　 ： 処理結果確認表ＣＳＶを出力する
    '
    ' 備考　 ： 
    '
    Public Sub OutputCSVKekka(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
        ' パラメータ情報
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter
        ' 取引先情報
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast
        ' 依頼明細情報
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast

        CsvData(1) = InfoPara.FURI_DATE        ' 振替日
        CsvData(2) = InfoTori.TORIS_CODE_T     ' 取引先主コード
        CsvData(3) = InfoTori.TORIF_CODE_T     ' 取引先副コード
        CsvData(4) = InfoTori.ITAKU_NNAME_T    ' 取引先名
        'Select Case InfoTori.TEKIYOU_KBN_T          ' 摘要
        '    Case "0"
        '        CsvData(5) = InfoMei.KTEKIYO
        '    Case "1"
        '        CsvData(5) = InfoMei.NTEKIYO
        '    Case "2"
        '        CsvData(5) = "データ摘要"
        'End Select
        CsvData(5) = InfoMei.ITAKU_CODE         ' 委託者コード
        CsvData(6) = InfoMei.TOTAL_IRAI_KEN.ToString                        ' 依頼件数
        CsvData(7) = InfoMei.TOTAL_IRAI_KIN.ToString                        ' 依頼金額
        CsvData(8) = (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN).ToString   ' 処理件数
        CsvData(9) = (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN).ToString   ' 処理金額
        CsvData(10) = ""                       ' 備考
        If aReadFmt.InfoMeisaiMast.DUPLICATE_KBN = "" Then
            If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN) Then
                CsvData(10) = "件数異常"
            ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                CsvData(10) = "金額異常"
            End If
        Else
            CsvData(10) = "二重持ち込み"
        End If
        CsvData(11) = "0"                       ' 区分
        CsvData(12) = InfoTori.FSYORI_KBN_T     ' 振替処理区分

        CSVObject.Output(CsvData)
    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean
        Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
    End Function
End Class
