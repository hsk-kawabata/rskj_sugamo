Imports System

'処理結果確認表
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase
    Public CsvData(10) As String

    Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFJP002"

        '定義体名セット
        ReportBaseName = "KFJP002_処理結果確認表（センター直接持込）.rpd"


        CsvData(0) = CASTCommon.Calendar.Now.ToString("yyyyMMdd")   '処理日
        CsvData(1) = CASTCommon.Calendar.Now.ToString("HHmmss")     'タイムスタンプ
        CsvData(2) = "0"                                            '取引先主コード
        CsvData(3) = "0"                                            '取引先副コード
        CsvData(4) = ""                                             '取引先名
        CsvData(5) = "00010101"                                     '振替日
        CsvData(6) = "0"                                            '依頼件数
        CsvData(7) = "0"                                            '依頼金額
        CsvData(8) = "0"                                           '処理件数
        CsvData(9) = "0"                                           '処理金額
        CsvData(10) = ""                                            '備考
        
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        Return file
    End Function

    Public Function OutputCSVKekkaSysError(ByVal fsyoriKbn As String, _
                        ByVal aToriS As String, ByVal aToriF As String, _
                        ByVal aTuuban As Integer, ByVal aInfile As String, _
                        ByVal aMSG As String, ByVal oraDB As CASTCommon.MyOracle) As Boolean
        '------------------------------------------------------
        ' ＣＳＶファイル作成(エラー分)
        '------------------------------------------------------
        Dim Comm As New CAstBatch.CommData(oraDB)

        Try
            'Call Comm.GetTORIMAST(aToriS, aToriF)

            'Itakusyamei = "ERROR"
            CreateCsvFile()

            CsvData(2) = ""
            CsvData(3) = ""
            
            '０行目
            If Not Comm.INFOToriMast.ITAKU_NNAME_T Is Nothing Then
                CsvData(4) = Comm.INFOToriMast.ITAKU_NNAME_T.Trim   '委託者名漢字
            Else
                CsvData(4) = ""                                     '取引先マスタに存在しない場合
            End If

            CsvData(5) = ""                                         '振替日
            CsvData(6) = ""
            CsvData(7) = ""
            CsvData(8) = ""
            CsvData(9) = ""
            CsvData(10) = ""

            If CASTCommon.CAInt32(aToriS) <> 0 Then
                CSVObject.Output(CsvData)
            End If

            '１行目
            CsvData(2) = ""
            CsvData(3) = ""
            CsvData(4) = "処理できませんでした "
            CsvData(5) = ""
            CsvData(6) = ""

            CSVObject.Output(CsvData)

            '２行目
            CsvData(4) = "ジョブ通番：" & aTuuban.ToString
            CSVObject.Output(CsvData)

            '３行目
            CsvData(4) = "ファイル名：" & aInfile
            CSVObject.Output(CsvData)

            '４行目
            CsvData(4) = """内容：" & aMSG & """"

            CSVObject.Output(CsvData)

            If ReportExecute() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write("(処理結果確認表(センター直接持込))CSVデータ出力", "失敗", ex.Message)
        End Try
    End Function

    '
    ' 機能　 ： 処理結果確認表ＣＳＶを出力する
    '
    ' 備考　 ： 
    '
    Public Sub OutputCSVKekka(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter    'パラメータ情報
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '取引先情報
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast          '依頼明細情報

        Try
            '------------------------------------------------------
            ' ＣＳＶファイル作成(正常終了時)
            '------------------------------------------------------
            CsvData(0) = InfoPara.FURI_DATE                                     '
            CsvData(2) = InfoTori.TORIS_CODE_T                                  '取引先主コード
            CsvData(3) = InfoTori.TORIF_CODE_T                                  '取引先副コード
            CsvData(4) = InfoTori.ITAKU_NNAME_T                                 '取引先名
            CsvData(5) = InfoMei.FURIKAE_DATE                                   '振替日
            CsvData(6) = InfoMei.TOTAL_IRAI_KEN.ToString                        '依頼件数
            CsvData(7) = InfoMei.TOTAL_IRAI_KIN.ToString                        '依頼金額
            CsvData(8) = (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN).ToString  '処理件数
            CsvData(9) = (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN).ToString  '処理金額
            CsvData(10) = ""                                                    '備考

            'エラーメッセージは備考欄に出力する
            If aReadFmt.InfoMeisaiMast.DUPLICATE_KBN = "" Then
                If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN) Then
                    CsvData(10) = "件数異常"
                ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                    CsvData(10) = "金額異常"
                End If
            End If

            CSVObject.Output(CsvData)

        Catch ex As Exception
            BatchLog.Write("(処理結果確認表(センター直接持込))CSVデータ出力", "失敗", ex.Message)
            End Try
    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean
        Try
            '2010/02/03 通常使用するプリンタにする ===
            'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
            Return MyBase.ReportExecute()
            '=======================================================
        Catch ex As Exception
            BatchLog.Write("(処理結果確認表(センター直接持込))印刷実行", "失敗", ex.Message)
            
            Return False
        End Try
    End Function
End Class
