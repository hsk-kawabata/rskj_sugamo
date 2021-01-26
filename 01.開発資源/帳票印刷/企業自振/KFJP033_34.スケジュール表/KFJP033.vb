Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 預金口座振替変更通知書
Class KFJP033
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal PrintNo As Integer)
        If PrintNo = 1 Then
            ' CSVファイルセット
            InfoReport.ReportName = "KFJP033"

            ' 定義体名セット
            ReportBaseName = "KFJP033_月間スケジュール表.rpd"
        Else
            ' CSVファイルセット
            InfoReport.ReportName = "KFJP034"

            ' 定義体名セット
            ReportBaseName = "KFJP034_振替日指定スケジュール表.rpd"
        End If

        '2017/05/08 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ START
        SORT_KEY = GCOM.GetObjectParam(InfoReport.ReportName, "SORT", "1")
        '2017/05/08 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ END

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： CSVファイルに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '
    ' 機能　 ： 印刷データの作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        '2011/06/16 標準版修正 決済機能を考慮 ------------------START
        '決済使用区分
        Dim KESSAI As String
        '2011/06/16 標準版修正 決済機能を考慮 ------------------END
        Try
            '2011/06/16 標準版修正 決済機能を考慮 ------------------START
            KESSAI = CASTCommon.GetFSKJIni("OPTION", "KESSAI")
            If KESSAI.ToUpper = "ERR" OrElse KESSAI = Nothing Then
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:決済使用区分 分類:OPTION 項目:KESSAI")
                RecordCnt = -1
                Return False
            End If
            '2011/06/16 標準版修正 決済機能を考慮 ------------------END
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT TORIS_CODE_S")                   '取引先主コード
            SQL.Append(" ,TORIF_CODE_S")                        '取引先副コード
            SQL.Append(" ,ITAKU_NNAME_T")                       '委託者漢字名
            SQL.Append(" ,ITAKU_CODE_S")                        '委託者コード
            SQL.Append(" ,TOUROKU_FLG_S")                       '登録済フラグ
            SQL.Append(" ,HAISIN_FLG_S")                        '配信済フラグ
            SQL.Append(" ,HENKAN_FLG_S")                        '返還済フラグ
            SQL.Append(" ,KESSAI_FLG_S")                        '決済済フラグ
            SQL.Append(" ,TESUUTYO_FLG_S")                      '手数料徴求済フラグ
            SQL.Append(" ,FURI_CODE_S")                         '振替コード
            SQL.Append(" ,KIGYO_CODE_S")                        '企業コード
            SQL.Append(" ,BAITAI_CODE_S")                       '媒体コード
            SQL.Append(" ,FURI_DATE_S")                         '振替日
            SQL.Append(" ,MOTIKOMI_DATE_S")                     '持込期日
            SQL.Append(" ,HAISIN_YDATE_S")                      '配信予定日
            SQL.Append(" ,HENKAN_YDATE_S")                      '返還予定日
            SQL.Append(" ,KESSAI_YDATE_S")                      '決済予定日
            SQL.Append(" ,TESUU_YDATE_S")                       '手数料徴求予定日
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            Select Case PrintKbn
                Case 0  '金庫持込
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                Case 1  '伝送
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    'SQL.Append(" AND BAITAI_CODE_S = '00'")
                    SQL.Append(" AND BAITAI_CODE_S IN ('00','10')")    '2012/06/30 標準版　WEB伝送対応
                Case 2  '媒体
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    SQL.Append(" AND BAITAI_CODE_S IN ('01','05','06','11','12','13','14','15')") 'FD3.5/MT/CMT/DVD/その他     '20120705 mubuchi "11"DVD追加
                Case 3  '依頼書
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    SQL.Append(" AND BAITAI_CODE_S = '04'")
                Case 4  '伝票
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    SQL.Append(" AND BAITAI_CODE_S = '09'")
                Case 5  '学校
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    SQL.Append(" AND BAITAI_CODE_S = '07'")
                Case 6  'センター直接持込
                    SQL.Append(" AND MOTIKOMI_KBN_S = '1'")
            End Select
            If PrintNo = 2 Then '振替日指定
                SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            Else                '対象月指定
                Dim strKIJYUN_DATE1 As String = FuriDate & "01"
                Dim strKIJYUN_DATE2 As String = FuriDate & "31"
                SQL.Append("  AND (FURI_DATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR HAISIN_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                '2011/06/16 標準版修正 決済機能を考慮 ------------------START
                If KESSAI = "1" Then
                    SQL.Append(" OR KESSAI_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                    SQL.Append(" OR TESUU_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                End If
                '2011/06/16 標準版修正 決済機能を考慮 ------------------ENDf

                SQL.Append(" OR HENKAN_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" )")
            End If
            '2017/05/08 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ START
            If SORT_KEY = "" Then
                SQL.Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S")
            Else
                SQL.Append(" ORDER BY " & SORT_KEY)
            End If
            'SQL.Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S")
            '2017/05/08 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ END

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                      '処理日
                    OutputCsvData(mMatchingTime, True)                                      'タイムスタンプ
                    '2010/01/06 追加
                    Select Case PrintKbn    '印刷区分
                        Case 0
                            '2010/12/24 信組対応 start
                            If CENTER = "0" Then
                                OutputCsvData("組合", True)
                            Else
                                OutputCsvData("金庫持込", True)
                            End If
                            '2010/12/24 信組対応 end
                        Case 1
                            OutputCsvData("伝送", True)
                        Case 2
                            OutputCsvData("媒体", True)
                        Case 3
                            OutputCsvData("依頼書", True)
                        Case 4
                            OutputCsvData("伝票", True)
                        Case 5
                            OutputCsvData("学校諸会費", True)
                        Case 6
                            OutputCsvData("センター直接持込", True)
                    End Select
                    '=====================
                    If PrintNo = 1 Then                                                     '対象年月(月間のみ)
                        OutputCsvData(FuriDate, True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_S")), True)    '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_S")), True)    '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)   '委託者名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_S")), True)    '委託者コード
                    If GCOM.NzStr(oraReader.GetString("TOUROKU_FLG_S")) = 1 Then            '登録
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HAISIN_FLG_S")) = 1 Then             '配信
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HENKAN_FLG_S")) = 1 Then             '返還
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KESSAI_FLG_S")) = 1 Then             '決済
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("TESUUTYO_FLG_S")) = 1 Then           '手数料
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_S")), True)     '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_S")), True)    '企業コード
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("BAITAI_CODE_S"))), True)
                    'Select Case GCOM.NzStr(oraReader.GetString("BAITAI_CODE_S"))            '媒体コード
                    '    Case "00"
                    '        OutputCsvData("伝送", True)
                    '    Case "01"
                    '        OutputCsvData("FD3.5", True)
                    '    Case "04"
                    '        OutputCsvData("依頼書", True)
                    '    Case "05"
                    '        OutputCsvData("MT", True)
                    '    Case "06"
                    '        OutputCsvData("CMT", True)
                    '    Case "07"
                    '        OutputCsvData("学校自振", True)
                    '    Case "09"
                    '        OutputCsvData("伝票", True)
                    '        '2012/06/30 標準版　WEB伝送対応
                    '    Case "10"
                    '        OutputCsvData("WEB伝送", True)
                    '        '******20120705 mubuchi DVD追加対応***************
                    '    Case "11"
                    '        OutputCsvData("DVD-RAM", True)
                    '        '******20120705 mubuchi DVD追加対応***************
                    '    Case "12"
                    '        OutputCsvData("その他", True)
                    '    Case "13"
                    '        OutputCsvData("その他", True)
                    '    Case "14"
                    '        OutputCsvData("その他", True)
                    '    Case "15"
                    '        OutputCsvData("その他", True)
                    '    Case Else
                    '        OutputCsvData("", True)
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")), True)     '振替日
                    If GCOM.NzStr(oraReader.GetString("MOTIKOMI_DATE_S")) = "00000000" Then               '持込期日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("MOTIKOMI_DATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HAISIN_YDATE_S")) = "00000000" Then  '配信予定日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HAISIN_YDATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HENKAN_YDATE_S")) = "00000000" Then  '返還予定日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HENKAN_YDATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KESSAI_YDATE_S")) = "00000000" Then  '決済予定日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KESSAI_YDATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("TESUU_YDATE_S")) = "00000000" Then   '手数料徴求予定日
                        OutputCsvData("", True, True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("TESUU_YDATE_S")), True, True)
                    End If
             
                    oraReader.NextRead()
                    RecordCnt += 1
                End While
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function

End Class