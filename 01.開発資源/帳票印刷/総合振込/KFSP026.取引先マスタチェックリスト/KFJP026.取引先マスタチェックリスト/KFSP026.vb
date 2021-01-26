Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFSP026

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFSP026"

        ' 定義体名セット
        ReportBaseName = "KFSP026_取引先マスタチェックリスト.rpd"
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

            SQL.Append(" SELECT")
            SQL.Append(" TORIS_CODE_T")         '取引先主コード
            SQL.Append(",TORIF_CODE_T")         '取引先副コード
            SQL.Append(",ITAKU_CODE_T")         '委託者コード
            SQL.Append(",NS_KBN_T")             '入出金区分
            SQL.Append(",SYUBETU_T")            '種別
            SQL.Append(",FURI_CODE_T")          '振替コード
            SQL.Append(",KIGYO_CODE_T")         '企業コード
            SQL.Append(",TEKIYOU_KBN_T")        '摘要区分
            SQL.Append(",KTEKIYOU_T")           'カナ摘要
            SQL.Append(",NTEKIYOU_T")           '漢字摘要
            SQL.Append(",SOUSIN_KBN_T")         '送信区分
            SQL.Append(",SYUMOKU_CODE_T")       '種目コード
            SQL.Append(",ITAKU_NNAME_T")        '委託者漢字名
            SQL.Append(",ITAKU_KNAME_T")        '委託者カナ名
            SQL.Append(",BAITAI_CODE_T")        '媒体コード
            SQL.Append(",FMT_KBN_T")            'フォーマット区分
            SQL.Append(",LABEL_KBN_T")          'ラベル区分
            SQL.Append(",FILE_NAME_T")          'ファイル名
            SQL.Append(",CODE_KBN_T")           'コード区分
            SQL.Append(",FURI_KYU_CODE_T")      '振込休日シフト
            SQL.Append(",UMEISAI_KBN_T")        '受付明細表出力区分

            For No As Integer = 1 To 31         '指定振込日
                SQL.Append(",DATE" & No.ToString & "_T")
            Next
            SQL.Append(",MULTI_KBN_T")          'マルチ区分
            SQL.Append(",TESUUTYO_KBN_T")       '手数料徴求区分
            SQL.Append(",TESUUMAT_NO_T")        '手数料集計周期
            SQL.Append(",TESUUTYO_DAY_T")       '手数料徴求日数/基準日
            SQL.Append(",TESUUTYO_KIJITSU_T")   '手数料徴求期日区分
            SQL.Append(",TESUUTYO_PATN_T")      '手数料徴求方法
            SQL.Append(",KESSAI_KBN_T")         '決済区分
            SQL.Append(",KESSAI_PATN_T")        '資金確保方法
            SQL.Append(",HONBU_KOUZA_T")        '本部別段口座番号
            SQL.Append(",KESSAI_KYU_CODE_T")    '決済休日コード
            SQL.Append(",TUKEKIN_NO_T")         '決済金融機関コード
            SQL.Append(",TUKESIT_NO_T")         '決済支店コード
            SQL.Append(",TUKEKAMOKU_T")         '決済科目
            SQL.Append(",TUKEKOUZA_T")          '決済口座番号
            SQL.Append(",TUKEMEIGI_KNAME_T")    '決済名義人(カナ)
            SQL.Append(",BIKOU1_T")             '備考１
            SQL.Append(",BIKOU2_T")             '備考２

            SQL.Append(" FROM S_TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_T ='3'")
            SQL.Append(" AND TORIS_CODE_T >=" & SQ(Toris_Kaisi))
            SQL.Append(" AND TORIS_CODE_T <=" & SQ(Toris_Syuryo))
            SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                                  '処理日
                    OutputCsvData(mMatchingTime, True)                                                  'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T")), True)                '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T")), True)                '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)                '委託者コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SYUBETU_T")), True)                   '種別
                    Select Case GCOM.NzStr(oraReader.GetString("NS_KBN_T"))                             '入出金区分
                        Case "1"
                            OutputCsvData("入金", True)
                        Case "9"
                            OutputCsvData("出金", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")), True)                 '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)                '企業コード
                    Select Case GCOM.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))                        '摘要
                        Case "0" 'カナ摘要
                            OutputCsvData("カナ", True)
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("KTEKIYOU_T")), True)
                        Case "1" '漢字摘要
                            OutputCsvData("漢字", True)
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("NTEKIYOU_T")), True)
                        Case "2", "3"   '可変摘要
                            OutputCsvData("ﾃﾞｰﾀ", True)
                            OutputCsvData("", True)
                        Case Else
                            OutputCsvData("", True)
                            OutputCsvData("", True)
                    End Select
                    Select Case GCOM.NzStr(oraReader.GetString("SOUSIN_KBN_T"))                         '送信区分
                        Case "0"
                            OutputCsvData("為替振込", True)
                        Case "1"
                            OutputCsvData("ロギング", True)
                        Case "2"
                            OutputCsvData("CSVリエンタ", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_KNAME_T")), True)               '委託者カナ名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)               '委託者漢字名

                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_総振_媒体コード.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("BAITAI_CODE_T"))), True)
                    'フォーマット名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_総振_フォーマット区分.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("FMT_KBN_T"))), True)
                    'Select Case GCOM.NzStr(oraReader.GetString("BAITAI_CODE_T"))                        '媒体コード
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
                    '        OutputCsvData("学校諸会費", True)
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
                    'Select Case GCOM.NzStr(oraReader.GetString("FMT_KBN_T"))                            'フォーマット区分
                    '    Case "00"
                    '        OutputCsvData("全銀", True)
                    '    Case "01"
                    '        OutputCsvData("ＮＨＫ", True)
                    '    Case "02"
                    '        OutputCsvData("国税", True)
                    '    Case "03"
                    '        OutputCsvData("年金", True)
                    '    Case "04"
                    '        OutputCsvData("依頼書", True)
                    '    Case "05"
                    '        OutputCsvData("伝票", True)
                    '    Case "06"
                    '        OutputCsvData("地公体", True)
                    '    Case Else
                    '        OutputCsvData("", True)
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                    Select Case GCOM.NzStr(oraReader.GetString("LABEL_KBN_T"))                          'ラベル区分
                        Case "0"
                            OutputCsvData("ラベル有", True)
                        Case "1"
                            OutputCsvData("ラベル無", True)
                        Case "2"
                            OutputCsvData("ラベル無(TM有)", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FILE_NAME_T")), True)                 'ファイル名
                    Select Case GCOM.NzStr(oraReader.GetString("CODE_KBN_T"))                           'コード区分
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

                    Select Case GCOM.NzStr(oraReader.GetString("SYUMOKU_CODE_T"))                       '種目コード
                        Case "00"
                            OutputCsvData("一般", True)
                        Case "01"
                            OutputCsvData("国庫金", True)
                        Case "02"
                            OutputCsvData("公金", True)
                        Case "03"
                            OutputCsvData("公金(指定日決済)", True)
                        Case "04"
                            OutputCsvData("公金(指定日前営業日決済)", True)
                        Case "05"
                            OutputCsvData("株式配当金(一般)", True)
                        Case "06"
                            OutputCsvData("株式配当金(自行)", True)
                        Case "07"
                            OutputCsvData("貸付信託収益配当金", True)
                        Case "08"
                            OutputCsvData("年金給付金(年金信託)", True)
                        Case "09"
                            OutputCsvData("年金給付金(公的年金)", True)
                        Case "10"
                            OutputCsvData("年金給付金(医療保険)", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("FURI_KYU_CODE_T"))                      '振込休日コード
                        Case "0"
                            OutputCsvData("翌営業日振込", True)
                        Case "1"
                            OutputCsvData("前営業日振込", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("UMEISAI_KBN_T"))                        '受付明細表出力区分
                        Case "0"
                            OutputCsvData("未出力", True)
                        Case "1"
                            OutputCsvData("店番ｿｰﾄ", True)
                        Case "2"
                            OutputCsvData("非ｿｰﾄ", True)
                        Case "3"
                            OutputCsvData("ｴﾗｰ分", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    Dim FURI_DATE As String = ""
                    For No As Integer = 1 To 31
                        If GCOM.NzStr(oraReader.GetString("DATE" & No & "_T")) = "1" Then
                            FURI_DATE &= No & " "
                        End If
                    Next
                    OutputCsvData(FURI_DATE, True)                                                  '契約振込日

                    Select Case GCOM.NzStr(oraReader.GetString("MULTI_KBN_T"))                          'マルチ区分
                        Case "0"
                            OutputCsvData("ｼﾝｸﾞﾙ", True)
                        Case "1"
                            OutputCsvData("ﾏﾙﾁ", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    Select Case GCOM.NzStr(oraReader.GetString("TESUUTYO_KBN_T"))                       '手数料徴求区分
                        Case "0"
                            OutputCsvData("都度徴求", True)
                        Case "1"
                            OutputCsvData("一括徴求", True)
                        Case "2"
                            OutputCsvData("特別免除", True)
                        Case "3"
                            OutputCsvData("別途徴求", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TESUUMAT_NO_T")), True)               '手数料集計周期(手数料まとめ回数)
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TESUUTYO_DAY_T")).Trim, True)         '日数／基準日(手数料徴求)
                    Select Case GCOM.NzStr(oraReader.GetString("TESUUTYO_KIJITSU_T"))                   '日付区分(手数料徴求)(手数料徴求期日区分)
                        Case "0"
                            OutputCsvData("営業日数指定", True)
                        Case "1"
                            OutputCsvData("基準日指定", True)
                        Case "2"
                            OutputCsvData("翌月基準日指定", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("TESUUTYO_PATN_T"))                      '手数料徴求方法
                        Case "0"
                            OutputCsvData("連動入金", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("KESSAI_KBN_T"))                         '決済区分
                        Case "00"
                            OutputCsvData("為替請求", True)
                        Case "99"
                            OutputCsvData("決済対象外", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("KESSAI_PATN_T"))                         '資金確保方法
                        Case "0"
                            OutputCsvData("事前確保", True)
                        Case "1"
                            OutputCsvData("営業店確保", True)
                        Case "2"
                            OutputCsvData("確保対象外", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HONBU_KOUZA_T")), True)               '本部別段口座番号
                    Select Case GCOM.NzStr(oraReader.GetString("KESSAI_KYU_CODE_T"))                    '決済休日シフト
                        Case "0"
                            OutputCsvData("翌営業日振込", True)
                        Case "1"
                            OutputCsvData("前営業日振込", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUKEKIN_NO_T")), True)                '決済金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUKESIT_NO_T")), True)                '決済支店コード

                    Select Case GCOM.NzStr(oraReader.GetString("TUKEKAMOKU_T"))                         '決済科目
                        Case "01"
                            OutputCsvData("当座", True)
                        Case "02"
                            OutputCsvData("普通", True)
                        Case "04"
                            OutputCsvData("別段", True)
                        Case "99"
                            OutputCsvData("諸勘定", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUKEKOUZA_T")), True)                 '決済口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUKEMEIGI_KNAME_T")), True)           '決済名義人カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("BIKOU1_T")), True)                    '備考１
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("BIKOU2_T")), True, True)              '備考２

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
