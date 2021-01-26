Imports System

'処理結果確認表
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    '2010/10/04.Sakon　摘要追加のため、インデックス変更 +++++++++++++++++++++++++
    Public CsvData(15) As String
    '' 2010/09/13 TASK)saitou 振替/企業コード印字対応 -------------------->
    '' 振替コード、企業コード追加のため、CsvDataのインデックスを12→14へ変更
    ''Public CsvData(12) As String
    'Public CsvData(14) As String
    '' 2010/09/13 TASK)saitou 振替/企業コード印字対応 --------------------<
    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFJP001"

        '定義体名セット
        ReportBaseName = "KFJP001_処理結果確認表(落込).rpd"

        CsvData(0) = "00010101"                                     '振替日
        CsvData(1) = CASTCommon.Calendar.Now.ToString("yyyyMMdd")   '処理日
        CsvData(2) = CASTCommon.Calendar.Now.ToString("HHmmss")     'タイムスタンプ
        CsvData(3) = "0"                                            '取引先主コード
        CsvData(4) = "0"                                            '取引先副コード
        CsvData(5) = ""                                             '取引先名
        CsvData(6) = ""                                             '委託者コード
        CsvData(7) = ""                                             '媒体
        CsvData(8) = "0"                                            '依頼件数
        CsvData(9) = "0"                                            '依頼金額
        CsvData(10) = "0"                                           '処理件数
        CsvData(11) = "0"                                           '処理金額
        CsvData(12) = ""                                            '備考
        ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 -------------------->
        CsvData(13) = ""                                            '振替コード
        CsvData(14) = ""                                            '企業コード
        ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 --------------------<

        '2010/10/04.Sakon　摘要追加 ++++++++++++++++++++++++++++++++++++++++++
        CsvData(15) = ""
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        '' タイトル行
        'CSVObject.Output("振替日")
        'CSVObject.Output("処理日")
        'CSVObject.Output("タイムスタンプ")
        'CSVObject.Output("取引先主コード")
        'CSVObject.Output("取引先副コード")
        'CSVObject.Output("取引先名")
        'CSVObject.Output("委託者コード")
        'CSVObject.Output("媒体")
        'CSVObject.Output("依頼件数")
        'CSVObject.Output("依頼金額")
        'CSVObject.Output("処理件数")
        'CSVObject.Output("処理金額")
        'CSVObject.Output("備考", True, True)

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
            Call Comm.GetTORIMAST(aToriS, aToriF)

            'Itakusyamei = "ERROR"
            CreateCsvFile()

            CsvData(0) = ""
            CsvData(3) = aToriS
            CsvData(4) = aToriF
            
            '０行目
            If Not Comm.INFOToriMast.ITAKU_NNAME_T Is Nothing Then
                CsvData(5) = Comm.INFOToriMast.ITAKU_NNAME_T.Trim   '委託者名漢字
            Else
                CsvData(5) = ""                                     '取引先マスタに存在しない場合
            End If

            If Not Comm.INFOToriMast.ITAKU_CODE_T Is Nothing Then
                CsvData(6) = Comm.INFOToriMast.ITAKU_CODE_T         '委託者コード追加
            Else
                CsvData(6) = ""                                     '取引先マスタに存在しない場合
            End If

            ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
            '媒体名をテキストから取得する

            ' 2016/10/10 向井 ADD 【PG】青梅信金 カスタマイズ対応(UI_12-1-7) -------------- START
            '媒体がファイリングで、委託者コードがALL1の場合『ファイリング』、ALL2の場合『CP』を表示させる
            If Comm.INFOToriMast.BAITAI_CODE_T = "09" Then

                If Comm.INFOToriMast.ITAKU_CODE_T = "2222222222" Then
                    CsvData(7) = "CP"
                Else
                    CsvData(7) = "ﾌｧｲﾘﾝｸﾞ"
                End If
            Else
                CsvData(7) = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
                                           Comm.INFOToriMast.BAITAI_CODE_T)
            End If
            ' 2016/10/10 向井 ADD 【PG】青梅信金 カスタマイズ対応(UI_12-1-7) -------------- END

            'Select Case Comm.INFOToriMast.BAITAI_CODE_T             '媒体
            '    Case "00"
            '        CsvData(7) = "伝送"
            '    Case "01"
            '        CsvData(7) = "FD3.5"
            '    Case "04"
            '        CsvData(7) = "依頼書"
            '    Case "05"
            '        CsvData(7) = "MT"
            '    Case "06"
            '        CsvData(7) = "CMT"
            '    Case "07"
            '        CsvData(7) = "学校自振"
            '    Case "09"
            '        CsvData(7) = "伝票"
            '        '2012/06/30 標準版　WEB伝送対応
            '    Case "10"
            '        CsvData(7) = "WEB伝送"
            '        '2012/06/30 標準版　WEB伝送対応
            '    Case "11"
            '        CsvData(7) = "DVD-RAM"
            '    Case "12"
            '        CsvData(7) = "その他"
            '    Case "13"
            '        CsvData(7) = "その他"
            '    Case "14"
            '        CsvData(7) = "その他"
            '        '2013/12/24 saitou 標準版 外部媒体対応 ADD -------------------------------------------------->>>>
            '    Case "15"
            '        CsvData(7) = "その他"
            '        '2013/12/24 saitou 標準版 外部媒体対応 ADD --------------------------------------------------<<<<
            '    Case Else
            '        CsvData(7) = ""
            'End Select
            ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

            CsvData(8) = ""
            CsvData(9) = ""
            CsvData(10) = ""
            '2010/02/04 追加
            CsvData(11) = ""                                           '処理金額
            '===============

            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 -------------------->
            If Not Comm.INFOToriMast.FURI_CODE_T Is Nothing Then
                CsvData(13) = Comm.INFOToriMast.FURI_CODE_T
            Else
                CsvData(13) = ""
            End If
            If Not Comm.INFOToriMast.KIGYO_CODE_T Is Nothing Then
                CsvData(14) = Comm.INFOToriMast.KIGYO_CODE_T
            Else
                CsvData(14) = ""
            End If
            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 --------------------<

            '2010/10/04.Sakon　摘要追加 ++++++++++++++++++++++++++++++++++++++++++
            Select Case Comm.INFOToriMast.TEKIYOU_KBN_T
                Case "0"
                    CsvData(15) = Comm.INFOToriMast.KTEKIYOU_T
                Case "1"
                    CsvData(15) = Comm.INFOToriMast.NTEKIYOU_T
                Case "2"
                    '2010/10/19 摘要文言修正--------------------------------------
                    'CsvData(15) = "データ摘要"
                    CsvData(15) = "可変摘要(支店名)"
                Case Else
                    'CsvData(15) = ""
                    CsvData(15) = "可変摘要(予備)"
                    '-------------------------------------------------------------
            End Select
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            If CASTCommon.CAInt32(aToriS) <> 0 Then
                CSVObject.Output(CsvData)
            End If

            '１行目
            CsvData(3) = ""
            CsvData(4) = ""
            CsvData(5) = "処理できませんでした "
            CsvData(6) = ""
            CsvData(7) = ""
            'CsvData(11) = "1"
            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 -------------------->
            CsvData(13) = ""
            CsvData(14) = ""
            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 --------------------<

            '2010/10/04.Sakon　摘要追加 ++++++++++++++++++++++++++++++++++++++++++
            CsvData(15) = ""
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            CSVObject.Output(CsvData)

            '２行目
            'CsvData(2) = "0"
            'CsvData(3) = "0"
            CsvData(5) = "ジョブ通番：" & aTuuban.ToString
            CSVObject.Output(CsvData)

            '３行目
            CsvData(5) = "ファイル名：" & aInfile
            CSVObject.Output(CsvData)

            '４行目
            CsvData(5) = """内容:" & aMSG & """"
            CSVObject.Output(CsvData)

            If ReportExecute() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write("(処理結果確認表印刷)", "失敗", ex.Message)
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
            CsvData(0) = InfoPara.FURI_DATE                                     '振替日
            CsvData(3) = InfoTori.TORIS_CODE_T                                  '取引先主コード
            CsvData(4) = InfoTori.TORIF_CODE_T                                  '取引先副コード
            CsvData(5) = InfoTori.ITAKU_NNAME_T                                 '取引先名
            CsvData(6) = InfoMei.ITAKU_CODE                                     '委託者コード

            ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
            '媒体名をテキストから取得する

            If InfoTori.BAITAI_CODE_T = "09" Then

                If InfoTori.ITAKU_CODE_T = "2222222222" Then
                    CsvData(7) = "CP"
                Else
                    CsvData(7) = "ﾌｧｲﾘﾝｸﾞ"
                End If
            Else
                CsvData(7) = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
                                           InfoTori.BAITAI_CODE_T)
            End If

            'CsvData(7) = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
            '                                           InfoTori.BAITAI_CODE_T)
            'Select Case InfoTori.BAITAI_CODE_T                                  '媒体
            '    Case "00"
            '        CsvData(7) = "伝送"
            '    Case "01"
            '        CsvData(7) = "FD3.5"
            '    Case "04"
            '        CsvData(7) = "依頼書"
            '    Case "05"
            '        CsvData(7) = "MT"
            '    Case "06"
            '        CsvData(7) = "CMT"
            '    Case "07"
            '        CsvData(7) = "学校自振"
            '    Case "09"
            '        CsvData(7) = "伝票"
            '        '2012/06/30 標準版　WEB伝送対応
            '    Case "10"
            '        CsvData(7) = "WEB伝送"
            '    Case "11"
            '        CsvData(7) = "DVD-RAM"
            '    Case "12"
            '        CsvData(7) = "その他"
            '    Case "13"
            '        CsvData(7) = "その他"
            '    Case "14"
            '        CsvData(7) = "その他"
            '        '2013/12/24 saitou 標準版 外部媒体対応 ADD -------------------------------------------------->>>>
            '    Case "15"
            '        CsvData(7) = "その他"
            '        '2013/12/24 saitou 標準版 外部媒体対応 ADD --------------------------------------------------<<<<
            '    Case Else
            '        CsvData(7) = ""
            'End Select
            ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

            CsvData(8) = InfoMei.TOTAL_IRAI_KEN.ToString                        '依頼件数
            CsvData(9) = InfoMei.TOTAL_IRAI_KIN.ToString                        '依頼金額
            '2010.03.05 NHKは0円を抜く start
            If InfoTori.FMT_KBN_T = "01" Then
                CsvData(10) = (InfoMei.TOTAL_KEN2 - InfoMei.TOTAL_IJO_KEN).ToString  '処理件数
            Else
                CsvData(10) = (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN).ToString  '処理件数
            End If
            '2010.03.05 NHKは0円を抜く end
            CsvData(11) = (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN).ToString  '処理金額
            CsvData(12) = ""                                                    '備考

            'エラーメッセージは備考欄に出力する
            If aReadFmt.InfoMeisaiMast.DUPLICATE_KBN = "" Then
                '2010.03.05 NHKは0円を抜く start
                If InfoTori.FMT_KBN_T = "01" Then
                    If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN2 - InfoMei.TOTAL_IJO_KEN) Then
                        CsvData(12) = "件数異常"
                    ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                        CsvData(12) = "金額異常"
                    End If
                Else
                    If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN) Then
                        CsvData(12) = "件数異常"
                    ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                        CsvData(12) = "金額異常"
                    End If
                End If
                '2010.03.05 NHKは0円を抜く end
            Else
                CsvData(12) = "二重持ち込み"
            End If

            If aReadFmt.TAKOU_ON = True Then
                CsvData(12) += " 他行有"
            End If

            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 -------------------->
            CsvData(13) = InfoTori.FURI_CODE_T          '振替コード
            CsvData(14) = InfoTori.KIGYO_CODE_T         '企業コード
            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 --------------------<

            '2010/10/04.Sakon　摘要追加 ++++++++++++++++++++++++++++++++++++++++++
            Select Case InfoTori.TEKIYOU_KBN_T
                Case "0"
                    CsvData(15) = InfoTori.KTEKIYOU_T
                Case "1"
                    CsvData(15) = InfoTori.NTEKIYOU_T
                Case "2"
                    '2010/10/19 摘要文言修正--------------------------------------
                    'CsvData(15) = "データ摘要"
                    CsvData(15) = "可変摘要(支店名)"
                Case Else
                    'CsvData(15) = ""
                    CsvData(15) = "可変摘要(予備)"
                    '-------------------------------------------------------------
            End Select
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            CSVObject.Output(CsvData)

        Catch ex As Exception
            BatchLog.Write("(処理結果確認表印刷)", "失敗", ex.Message)
        End Try
    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean
        Try
            '2010/02/03 通常使用するプリンタにする ===
            'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
            Return MyBase.ReportExecute()
            '=======================================================
        Catch ex As Exception
            BatchLog.Write("(処理結果確認表印刷)", "印刷", ex.Message)
        End Try
    End Function
End Class
