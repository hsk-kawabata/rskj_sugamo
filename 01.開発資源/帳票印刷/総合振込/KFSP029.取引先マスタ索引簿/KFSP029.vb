Option Strict On
Option Explicit On

Imports System
Imports System.Text

Public Class KFSP029

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFSP029"

        ' 定義体名セット
        ReportBaseName = "KFSP029_取引先マスタ索引簿.rpd"
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

            SQL.Append(" SELECT ")
            SQL.Append("TORIS_CODE_T")      '取引先主コード
            SQL.Append(",TORIF_CODE_T")     '取引先副コード
            SQL.Append(",ITAKU_KNAME_T")    '委託者カナ名
            SQL.Append(",ITAKU_NNAME_T")    '委託者漢字名
            SQL.Append(",ITAKU_CODE_T")     '委託者コード
            SQL.Append(",FURI_CODE_T")      '振替コード
            SQL.Append(",KIGYO_CODE_T")     '企業コード
            SQL.Append(",BAITAI_CODE_T")    '媒体コード
            SQL.Append(",CODE_KBN_T")       'コード区分
            SQL.Append(",TEKIYOU_KBN_T")    '摘要区分
            SQL.Append(",KTEKIYOU_T")       'カナ摘要
            SQL.Append(",NTEKIYOU_T")       '漢字摘要
            SQL.Append(",NS_KBN_T")         '入出金区分
            SQL.Append(",FMT_KBN_T")        'フォーマット区分
            SQL.Append(",SYUBETU_T")        '種別
            SQL.Append(",MULTI_KBN_T")      'マルチ区分
            SQL.Append(",FILE_NAME_T")      'ファイル名
            For No As Integer = 1 To 31     '指定振込日
                SQL.Append(",DATE" & No.ToString & "_T")
            Next
            SQL.Append(" FROM S_TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_T ='3'")
            Select Case SortCd
                Case "1" '取引先コード順
                    SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T")
                Case "2" '種別順
                    SQL.Append(" ORDER BY SYUBETU_T,TORIS_CODE_T,TORIF_CODE_T")
                Case "3" '委託者カナ名順
                    SQL.Append(" ORDER BY ITAKU_KNAME_T,TORIS_CODE_T,TORIF_CODE_T")
            End Select

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                      '処理日
                    OutputCsvData(mMatchingTime, True)                                      'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T")), True)    '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T")), True)    '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_KNAME_T")), True)   '委託者カナ名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)   '委託者漢字名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)    '委託者コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")), True)     '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)    '企業コード
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_総振_媒体コード.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("BAITAI_CODE_T"))), True)
                    'Select Case GCOM.NzStr(oraReader.GetString("BAITAI_CODE_T"))            '媒体コード
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
                    Select Case GCOM.NzStr(oraReader.GetString("CODE_KBN_T"))               'コード区分
                        Case "0"
                            OutputCsvData("JIS", True)
                        Case "1"
                            OutputCsvData("JIS改有(120)", True)
                        Case "2"
                            OutputCsvData("JIS改有(119)", True)
                        Case "3"
                            OutputCsvData("JIS改有(118)", True)
                        Case "4"
                            OutputCsvData("EBCDIC", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    Select Case GCOM.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))            '摘要
                        Case "0" 'カナ摘要
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("KTEKIYOU_T")), True)
                        Case "1" '漢字摘要
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("NTEKIYOU_T")), True)
                        Case Else '可変摘要
                            OutputCsvData("", True)
                    End Select
                    Select Case GCOM.NzStr(oraReader.GetString("NS_KBN_T"))                 '入出金区分
                        Case "1"
                            OutputCsvData("入金", True)
                        Case "9"
                            OutputCsvData("出金", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_総振_フォーマット区分.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("FMT_KBN_T"))), True)
                    'Select Case GCOM.NzStr(oraReader.GetString("FMT_KBN_T"))                'フォーマット区分
                    '    Case "00"
                    '        OutputCsvData("全銀", True)
                    '    Case "04"
                    '        OutputCsvData("依頼書", True)
                    '    Case "05"
                    '        OutputCsvData("伝票", True)
                    '    Case Else
                    '        OutputCsvData("", True)
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                    Select Case GCOM.NzStr(oraReader.GetString("SYUBETU_T"))                '種別
                        Case "21"
                            OutputCsvData("総振", True)
                        Case "11"
                            OutputCsvData("給与", True)
                        Case "12"
                            OutputCsvData("賞与", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    Select Case GCOM.NzStr(oraReader.GetString("MULTI_KBN_T"))              'マルチ区分
                        Case "0"
                            OutputCsvData("1ﾍｯﾀﾞ/1ﾌｧｲﾙ", True)
                        Case "1"
                            OutputCsvData("複数ﾍｯﾀﾞ/1ﾌｧｲﾙ", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FILE_NAME_T")), True)     'ファイル名

                    Dim FURI_DATE As String = ""
                    For No As Integer = 1 To 31
                        If GCOM.NzStr(oraReader.GetString("DATE" & No & "_T")) = "1" Then
                            FURI_DATE &= No & " "
                        End If
                    Next
                    OutputCsvData(FURI_DATE, True, True)                                    '契約振込日
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
