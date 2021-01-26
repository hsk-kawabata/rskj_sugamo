Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFJP037

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP037"

        ' 定義体名セット
        ReportBaseName = "KFJP037_未処理一覧表(落込).rpd"

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

            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")                     '取引先主コード
            SQL.Append(",TORIF_CODE_T")                     '取引先副コード
            SQL.Append(",ITAKU_NNAME_T")                    '委託者名
            SQL.Append(",ITAKU_CODE_T")                     '委託者コード
            SQL.Append(",TSIT_NO_T")                        '取扱支店
            SQL.Append(",SIT_NNAME_N")                      '取扱支店名
            SQL.Append(",BAITAI_CODE_S")                    '媒体コード
            SQL.Append(",FURI_DATE_S")                      '振替日
            SQL.Append(",MOTIKOMI_DATE_S")                  '持込期日
            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",FURI_CODE_T")                      '振替コード
            SQL.Append(",KIGYO_CODE_T")                     '企業コード
            ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 --------------------<
            SQL.Append(" FROM SCHMAST,TORIMAST,TENMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND FURI_DATE_S BETWEEN " & SQ(KaisiDate) & " AND " & SQ(SyuryoDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '0'")
            SQL.Append(" AND KIN_NO_N = TKIN_NO_T")
            SQL.Append(" AND SIT_NO_N = TSIT_NO_T")
            '2017/05/08 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ START
            If SORT_KEY = "" Then
                SQL.Append(" ORDER BY TORIS_CODE_S,TORIF_CODE_S,FURI_DATE_S")
            Else
                SQL.Append(" ORDER BY " & SORT_KEY)
            End If
            'SQL.Append(" ORDER BY TORIS_CODE_S,TORIF_CODE_S,FURI_DATE_S")
            '2017/05/08 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ END
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                      '処理日
                    OutputCsvData(mMatchingTime, True)                                      'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T")), True)    '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T")), True)    '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)   '委託者名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)    '委託者コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_T")), True)       '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")), True)     '支店名
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
                    '        '******20120705 mubuchi DVD追加対応*******
                    '    Case "11"
                    '        OutputCsvData("DVD-RAM", True)
                    '        '******20120705 mubuchi DVD追加対応*******
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
                    ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 -------------------->
                    'If GCOM.NzStr(oraReader.GetString("MOTIKOMI_DATE_S")) = "00000000" Then '持込期日(受付予定日)
                    '    OutputCsvData("", True, True)
                    'Else
                    '    OutputCsvData(GCOM.NzStr(oraReader.GetString("MOTIKOMI_DATE_S")), True, True)
                    'End If
                    If GCOM.NzStr(oraReader.GetString("MOTIKOMI_DATE_S")) = "00000000" Then '持込期日(受付予定日)
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("MOTIKOMI_DATE_S")), True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")), True)     '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), True, True)    '企業コード
                    ' 2010/09/13 TASK)saitou 振替/企業コード印字対応 --------------------<

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
