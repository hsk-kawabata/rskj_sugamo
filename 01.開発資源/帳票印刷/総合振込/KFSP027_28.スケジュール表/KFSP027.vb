Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic

Class KFSP027
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal PrintNo As Integer)
        If PrintNo = 1 Then
            ' CSVファイルセット
            InfoReport.ReportName = "KFSP027"

            ' 定義体名セット
            ReportBaseName = "KFSP027_月間スケジュール表.rpd"
        Else
            ' CSVファイルセット
            InfoReport.ReportName = "KFSP028"

            ' 定義体名セット
            ReportBaseName = "KFSP028_振込日指定スケジュール表.rpd"
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
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT TORIS_CODE_S")                   '取引先主コード
            SQL.Append(" ,TORIF_CODE_S")                        '取引先副コード
            SQL.Append(" ,ITAKU_NNAME_T")                       '委託者漢字名
            SQL.Append(" ,ITAKU_CODE_S")                        '委託者コード
            SQL.Append(" ,SYUBETU_S")                           '種別
            SQL.Append(" ,TOUROKU_FLG_S")                       '登録済フラグ
            SQL.Append(" ,HASSIN_FLG_S")                        '発信済フラグ
            SQL.Append(" ,KAKUHO_FLG_S")                        '確保済フラグ
            SQL.Append(" ,KESSAI_FLG_S")                        '決済済フラグ
            SQL.Append(" ,TESUUTYO_FLG_S")                      '手数料徴求済フラグ
            SQL.Append(" ,FURI_CODE_S")                         '振替コード
            SQL.Append(" ,KIGYO_CODE_S")                        '企業コード
            SQL.Append(" ,BAITAI_CODE_S")                       '媒体コード
            SQL.Append(" ,FURI_DATE_S")                         '振込日
            SQL.Append(" ,MOTIKOMI_SEQ_S")                      '持込回数
            SQL.Append(" ,KAKUHO_YDATE_S")                      '確保予定日
            SQL.Append(" ,KAKUHO_DATE_S")                       '確保日
            SQL.Append(" ,HASSIN_YDATE_S")                      '発信予定日
            SQL.Append(" ,HASSIN_DATE_S")                       '発信日
            SQL.Append(" ,KESSAI_KBN_T")                        '決済区分
            SQL.Append(" ,KESSAI_PATN_T")                       '資金確保方法

            SQL.Append(" FROM S_SCHMAST,S_TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='3'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            Select Case PrintKbn
                Case 1  '伝送
                    'SQL.Append(" AND BAITAI_CODE_S = '00'")
                    SQL.Append(" AND BAITAI_CODE_S IN ('00','10')")    '2012/06/30 標準版　WEB伝送対応
                Case 2  '媒体
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                    SQL.Append(" AND BAITAI_CODE_S IN ('01','05','06','11','12','13','14','15')") 'FD3.5/MT/CMT/DVD/その他
                    'SQL.Append(" AND BAITAI_CODE_S IN ('01','05', '06')") 'FD3.5/MT/CMT
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                Case 3  '依頼書
                    SQL.Append(" AND BAITAI_CODE_S = '04'")
                Case 4  '伝票
                    SQL.Append(" AND BAITAI_CODE_S = '09'")
            End Select
            If PrintNo = 2 Then '振込日指定
                SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            Else                '対象月指定
                Dim strKIJYUN_DATE1 As String = FuriDate & "01"
                Dim strKIJYUN_DATE2 As String = FuriDate & "31"
                SQL.Append("  AND (FURI_DATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR KAKUHO_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR HASSIN_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR KESSAI_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR TESUU_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" )")
            End If
            '2017/05/08 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ START
            If SORT_KEY = "" Then
                SQL.Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")
            Else
                SQL.Append(" ORDER BY " & SORT_KEY)
            End If
            'SQL.Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")
            '2017/05/08 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ END

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                      '処理日
                    OutputCsvData(mMatchingTime, True)                                      'タイムスタンプ
                    '2010/01/06 追加
                    Select Case PrintKbn    '印刷区分
                        Case 0
                            OutputCsvData("総振　全件", True)
                        Case 1
                            OutputCsvData("総振　伝送", True)
                        Case 2
                            OutputCsvData("総振　媒体", True)
                        Case 3
                            OutputCsvData("総振　依頼書", True)
                        Case 4
                            OutputCsvData("総振　伝票", True)
                    End Select
                    '=====================
                    If PrintNo = 1 Then                                                     '対象年月(月間のみ)
                        OutputCsvData(FuriDate, True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_S")), True)    '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_S")), True)    '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)   '委託者名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_S")), True)    '委託者コード
                    If GCOM.NzStr(oraReader.GetString("TOUROKU_FLG_S")) = "1" Then          '登録
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KAKUHO_FLG_S")) = "1" Then           '確保
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HASSIN_FLG_S")) = "1" Then           '発信
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KESSAI_FLG_S")) = "1" Then           '決済
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("TESUUTYO_FLG_S")) = "1" Then         '手数料
                        OutputCsvData("■", True)
                    Else
                        OutputCsvData("□", True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_S")), True)     '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_S")), True)    '企業コード
                    Select Case GCOM.NzStr(oraReader.GetString("SYUBETU_S"))
                        Case "21"
                            OutputCsvData("総振", True)
                        Case "11"
                            OutputCsvData("給与", True)
                        Case "12"
                            OutputCsvData("賞与", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_総振_媒体コード.TXT"), _
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
                    '    Case "09"
                    '        OutputCsvData("伝票", True)
                    '        '2012/06/30 標準版　WEB伝送対応
                    '    Case "10"
                    '        OutputCsvData("WEB伝送", True)
                    '        '2013/12/24 saitou 標準版 外部媒体対応 ADD -------------------------------------------------->>>>
                    '    Case "11"
                    '        OutputCsvData("DVD-RAM", True)
                    '    Case "12"
                    '        OutputCsvData("その他", True)
                    '    Case "13"
                    '        OutputCsvData("その他", True)
                    '    Case "14"
                    '        OutputCsvData("その他", True)
                    '    Case "15"
                    '        OutputCsvData("その他", True)
                    '        '2013/12/24 saitou 標準版 外部媒体対応 ADD --------------------------------------------------<<<<
                    '    Case Else
                    '        OutputCsvData("", True)
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")), True)     '振込日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MOTIKOMI_SEQ_S")), True)  '持込回数
                    If GCOM.NzStr(oraReader.GetString("KESSAI_PATN_T")) = "2" OrElse GCOM.NzStr(oraReader.GetString("KAKUHO_YDATE_S")) = "00000000" Then  '確保予定日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KAKUHO_YDATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KESSAI_PATN_T")) = "2" OrElse GCOM.NzStr(oraReader.GetString("KAKUHO_DATE_S")) = "00000000" Then   '確保日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KAKUHO_DATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HASSIN_YDATE_S")) = "00000000" Then  '発信予定日
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HASSIN_YDATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HASSIN_DATE_S")) = "00000000" Then   '発信日
                        OutputCsvData("", True, True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HASSIN_DATE_S")), True, True)
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