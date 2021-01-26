Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFJP029
    Inherits CAstReports.ClsReportBase

    Private TORIS_CODE As String    '取引先主コード
    Private TORIF_CODE As String    '取引先副コード
    Private TKIN_NO As String       '取扱金融機関コード
    Private KIN_NAME As String      '取扱金融機関名
    Private TSIT_NO As String       '取扱支店名
    Private SIT_NAME As String     '取扱支店名
    Private IRAISYO_SORT As String  '依頼書ソート順

    Sub New(ByVal Baitai As String, ByVal Sort As String)
        If Sort = "1" Then
            ' CSVファイルセット
            InfoReport.ReportName = "KFJP029"
            ' 定義体名セット
            ReportBaseName = "KFJP029_口座振替入力チェックリスト.rpd"
        Else
            ' CSVファイルセット
            InfoReport.ReportName = "KFJP030"
            ' 定義体名セット
            ReportBaseName = "KFJP030_口座振替入力チェックリスト(店番ソート).rpd"
        End If
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
    ' 機能　 ： 印刷データ(依頼書)の作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord_IRAISYO() As Boolean
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append(" SELECT ")
            SQL.Append(" TORIS_CODE_T")         '取引先主コード
            SQL.Append(",TORIF_CODE_T")         '取引先副コード
            SQL.Append(",IRAISYO_SORT_T")       '依頼書出力順

            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = FSYORI_KBN_S")
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0' ") '継続
            If TorisCode + TorifCode <> "111111111111" Then
                SQL.Append(" AND TORIS_CODE_T = " & SQ(TorisCode))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(TorifCode))
            End If
            SQL.Append(" AND SYURYOU_DATE_T >= " & SQ(FuriDate))
            SQL.Append(" AND BAITAI_CODE_T = '04'")
            SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    TORIS_CODE = GCOM.NzStr(oraReader.GetString("TORIS_CODE_T"))        '取引先主コード
                    TORIF_CODE = GCOM.NzStr(oraReader.GetString("TORIF_CODE_T"))        '取引先副コード
                    IRAISYO_SORT = GCOM.NzStr(oraReader.GetString("IRAISYO_SORT_T"))    '依頼書出力順
                    If SetENTMAST() = False Then
                        'Return False
                    End If
                    oraReader.NextRead()
                End While
                If AllRecordCnt = 0 Then
                    BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                    RecordCnt = -1
                    Return False
                End If
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
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function
    '
    ' 機能　 ： 依頼書の内容を書き出す
    '
    ' 備考　 ： 
    '
    Private Function SetENTMAST() As Boolean

        RecordCnt = 0
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append(" SELECT")
            SQL.Append(" ITAKU_NNAME_T,FURI_CODE_T,KIGYO_CODE_T,SOUSIN_KBN_T,TOUROKU_FLG_S")
            SQL.Append(",TKIN_NO_T,TSIT_NO_T,KEIYAKU_NO_E,TKIN_NO_E,KIN_NNAME_N,TSIT_NO_E")
            SQL.Append(",SIT_NNAME_N,KAMOKU_E,KOUZA_E,KEIYAKU_KNAME_E,KEIYAKU_NNAME_E")
            SQL.Append(",FURIKIN_E,TESUU_E,ERR_MSG_E,SINKI_CODE_E,JYUYOUKA_NO_E")
            SQL.Append(",'0000000000' KOKYAKU_NO_E")
            SQL.Append(" FROM ENTMAST,TORIMAST,TENMAST,SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_E")
            SQL.Append(" AND   TORIF_CODE_T = TORIF_CODE_E")
            SQL.Append(" AND   TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND   TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" AND 　TYUUDAN_FLG_S = '0' ") '継続
            SQL.Append(" AND   FURI_DATE_E =  FURI_DATE_S")
            SQL.Append(" AND   KAIYAKU_E = '0'")    '解約済みでない
            SQL.Append(" AND   FURIKIN_E > 0")      '0円以上
            SQL.Append(" AND   SINKI_CODE_E = '0'")
            SQL.Append(" AND   KAISI_DATE_E <= " & SQ(FuriDate))
            SQL.Append(" AND   FURI_DATE_E = " & SQ(FuriDate))
            SQL.Append(" AND   TORIS_CODE_E = " & SQ(TORIS_CODE))
            SQL.Append(" AND   TORIF_CODE_E = " & SQ(TORIF_CODE))
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '金融機関マスタに存在しない場合も出力する
            SQL.Append(" AND   KIN_NO_N(+) = TKIN_NO_E")
            SQL.Append(" AND   SIT_NO_N(+) = TSIT_NO_E")
            'SQL.Append(" AND   KIN_NO_N = TKIN_NO_E")
            'SQL.Append(" AND   SIT_NO_N = TSIT_NO_E")
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
            SQL.Append(" UNION SELECT")
            SQL.Append(" ITAKU_NNAME_T,FURI_CODE_T,KIGYO_CODE_T,SOUSIN_KBN_T,TOUROKU_FLG_S")
            SQL.Append(",TKIN_NO_T,TSIT_NO_T,KEIYAKU_NO_E,TKIN_NO_E,KIN_NNAME_N,TSIT_NO_E")
            SQL.Append(",SIT_NNAME_N,KAMOKU_E,KOUZA_E,KEIYAKU_KNAME_E,KEIYAKU_NNAME_E")
            SQL.Append(",FURIKIN_E,TESUU_E,ERR_MSG_E,SINKI_CODE_E,JYUYOUKA_NO_E")
            SQL.Append(",KOKYAKU_NO_E")
            SQL.Append(" FROM ENTMAST,TORIMAST,TENMAST,SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_E")
            SQL.Append(" AND   TORIF_CODE_T = TORIF_CODE_E")
            SQL.Append(" AND   TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND   TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" AND 　TYUUDAN_FLG_S = '0' ") '継続
            SQL.Append(" AND   FURI_DATE_E =  FURI_DATE_S")
            SQL.Append(" AND   KAIYAKU_E = '0'")    '解約済みでない
            SQL.Append(" AND   FURIKIN_E > 0")      '0円以上
            SQL.Append(" AND   SINKI_CODE_E <> '0'")
            SQL.Append(" AND   KAISI_DATE_E <= " & SQ(FuriDate))
            SQL.Append(" AND   FURI_DATE_E = " & SQ(FuriDate))
            SQL.Append(" AND   TORIS_CODE_E = " & SQ(TORIS_CODE))
            SQL.Append(" AND   TORIF_CODE_E = " & SQ(TORIF_CODE))
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '金融機関マスタに存在しない場合も出力する
            SQL.Append(" AND   KIN_NO_N(+) = TKIN_NO_E")
            SQL.Append(" AND   SIT_NO_N(+) = TSIT_NO_E")
            'SQL.Append(" AND   KIN_NO_N = TKIN_NO_E")
            'SQL.Append(" AND   SIT_NO_N = TSIT_NO_E")
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
            If SortNo = "2" Then  '支店ソートあり
                SQL.Append(" ORDER BY TKIN_NO_E,TSIT_NO_E,")
            Else
                SQL.Append(" ORDER BY ")
            End If
            '2011/06/24 標準版修正 エントリ処理に新規コードは考慮しない ------------------START
            Select Case IRAISYO_SORT
                Case "1"
                    SQL.Append(" KEIYAKU_KNAME_E , KEIYAKU_NO_E")
                Case "2"
                    SQL.Append(" JYUYOUKA_NO_E , KEIYAKU_NO_E")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-07(RSV2対応<小浜信金>) -------------------- START
                Case "3"
                    SQL.Append(" TKIN_NO_E , TSIT_NO_E , KAMOKU_E , KOUZA_E ASC")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-07(RSV2対応<小浜信金>) -------------------- END
                Case Else
                    SQL.Append(" KEIYAKU_NO_E")
            End Select
            '2011/06/24 標準版修正 エントリ処理に新規コードは考慮しない ------------------END

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                              '処理日
                    OutputCsvData(mMatchingTime, True)                                              'タイムスタンプ
                    OutputCsvData("依頼書   口振", True)                                            'サブタイトル
                    OutputCsvData(TORIS_CODE, True)                                                 '取引先主コード
                    OutputCsvData(TORIF_CODE, True)                                                 '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T", True)))            '委託者名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T", True)))             '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T", True)))            '企業コード
                    Select Case GCOM.NzStr(oraReader.GetString("SOUSIN_KBN_T"))                     '送信区分
                        'Case 2
                        '    OutputCsvData("ロギング", True)
                        'Case 3
                        '    OutputCsvData("為替振込", True)
                        Case Else
                            OutputCsvData("口座振替", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TOUROKU_FLG_S", True)))           '登録フラグ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_T")), True)                                                    '取扱金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_T")), True)                                                   '取扱支店コード
                    OutputCsvData(FuriDate, True)                                                   '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NO_E")), True)            '契約者番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_E")), True)               '振替金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")), True)             '振替金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_E")), True)               '振替支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")), True)             '振替支店名
                    OutputCsvData(ConvertKamoku2TO1(oraReader.GetString("KAMOKU_E")), True)         '科目(1桁)
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_E"))                         '科目名
                        Case "02"
                            OutputCsvData("普", True)
                        Case "01"
                            OutputCsvData("当", True)
                        Case "37"
                            OutputCsvData("職", True)
                        Case "05"
                            OutputCsvData("納", True)
                        Case "04"
                            OutputCsvData("別", True)
                        Case "99"
                            OutputCsvData("諸", True)
                        Case Else
                            OutputCsvData("他", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_E")), True)                 '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KNAME_E")), True)         '契約者カナ名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NNAME_E")), True)         '契約者漢字名
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FURIKIN_E")), True)                '振替金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt64("TESUU_E")), True)                  '手数料
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ERR_MSG_E")), True)               '備考
                    OutputCsvData(Jikinko, True, True)                                              '自金庫コード

                    AllRecordCnt += 1
                    RecordCnt += 1

                    oraReader.NextRead()
                End While
                Return True
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = 0
                Return False
            End If
        Catch ex As Exception
            '2017/05/22 saitou RSV2 DEL 標準版修正（潜在バグ） ---------------------------------------- START
            '不要
            'Ret = -300
            '2017/05/22 saitou RSV2 DEL --------------------------------------------------------------- END
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        End Try
    End Function
    '
    ' 機能　 ：印刷データ(伝票)の作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord_DENPYO() As Boolean
        RecordCnt = 0
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            SQL.Append(" SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",FURI_CODE_T")
            SQL.Append(",KIGYO_CODE_T")
            SQL.Append(",SOUSIN_KBN_T")
            SQL.Append(",TOUROKU_FLG_S")
            SQL.Append(",TKIN_NO_T")
            SQL.Append(",TSIT_NO_T")
            SQL.Append(",KEIYAKU_NO_E")
            SQL.Append(",TKIN_NO_E")
            SQL.Append(",KIN_NNAME_N")
            SQL.Append(",FURIKIN_E")
            SQL.Append(",TSIT_NO_E")
            SQL.Append(",SIT_NNAME_N")
            SQL.Append(",KAMOKU_E")
            SQL.Append(",KOUZA_E")
            SQL.Append(",KEIYAKU_KNAME_E")
            SQL.Append(",KEIYAKU_NNAME_E")
            SQL.Append(",TESUU_E")
            SQL.Append(",ERR_MSG_E")
            SQL.Append(" FROM DENPYOMAST,TORIMAST,TENMAST,SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_E")
            SQL.Append(" AND   TORIF_CODE_T = TORIF_CODE_E")
            SQL.Append(" AND   TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND   TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" AND 　TYUUDAN_FLG_S = '0' ") '継続
            'SQL.Append(" AND 　UKETUKE_FLG_S = '1' ") '受付済
            SQL.Append(" AND   FURI_DATE_E =  FURI_DATE_S")
            SQL.Append(" AND   KAIYAKU_E = '0'")    '解約済みでない
            SQL.Append(" AND   FURIKIN_E > 0")      '0円以上
            SQL.Append(" AND   FURI_DATE_E = " & SQ(FuriDate))
            If TorisCode + TorifCode = "222222222222" Then
                SQL.Append(" AND   BAITAI_CODE_T = " & SQ(BaitaiCode))
            Else
                SQL.Append(" AND   TORIS_CODE_E = " & SQ(TorisCode))
                SQL.Append(" AND   TORIF_CODE_E = " & SQ(TorifCode))
            End If
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '金融機関マスタに存在しない場合も出力する
            SQL.Append(" AND   KIN_NO_N(+) = TKIN_NO_E")
            SQL.Append(" AND   SIT_NO_N(+) = TSIT_NO_E")
            'SQL.Append(" AND   KIN_NO_N = TKIN_NO_E")
            'SQL.Append(" AND   SIT_NO_N = TSIT_NO_E")
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            If SortNo = "2" Then
                SQL.Append(" ORDER BY TORIS_CODE_E,TORIF_CODE_E,TKIN_NO_E,TSIT_NO_E,KEIYAKU_NO_E")
            Else
                SQL.Append(" ORDER BY TORIS_CODE_E,TORIF_CODE_E,KEIYAKU_NO_E")
            End If


            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                                    '処理日
                    OutputCsvData(mMatchingTime, True)                                                    'タイムスタンプ
                    OutputCsvData("伝票   口振", True)                                                    'サブタイトル
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T", True)))                  '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T", True)))                  '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T", True)))                 '委託者名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T", True)))                   '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T", True)))                  '企業コード
                    Select Case GCOM.NzStr(oraReader.GetString("SOUSIN_KBN_T"))                           '送信区分
                        'Case 2
                        '    OutputCsvData("ロギング", True)
                        'Case 3
                        '    OutputCsvData("為替振込", True)
                        Case Else
                            OutputCsvData("口座振替", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TOUROKU_FLG_S", True)))           '登録フラグ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_T")), True)                                                    '取扱金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_T")), True)                                                   '取扱支店コード
                    OutputCsvData(FuriDate, True)                                                   '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NO_E")), True)            '契約者番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_E")), True)               '振替金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")), True)             '振替金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_E")), True)               '振替支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")), True)             '振替支店名
                    OutputCsvData(ConvertKamoku2TO1(oraReader.GetString("KAMOKU_E")), True)         '科目(1桁)
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_E"))                         '科目名
                        Case "02"
                            OutputCsvData("普", True)
                        Case "01"
                            OutputCsvData("当", True)
                        Case "37"
                            OutputCsvData("職", True)
                        Case "05"
                            OutputCsvData("納", True)
                        Case "04"
                            OutputCsvData("別", True)
                        Case "99"
                            OutputCsvData("諸", True)
                        Case Else
                            OutputCsvData("他", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_E")), True)                 '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KNAME_E")), True)         '契約者カナ名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NNAME_E")), True)         '契約者漢字名
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FURIKIN_E")), True)                '振替金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt64("TESUU_E")), True)                  '手数料
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ERR_MSG_E")), True)               '備考
                    OutputCsvData(Jikinko, True, True)                                              '自金庫コード

                    AllRecordCnt += 1
                    RecordCnt += 1

                    oraReader.NextRead()
                End While
                Return True
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = 0
                Return False
            End If
        Catch ex As Exception
            '2017/05/22 saitou RSV2 DEL 標準版修正（潜在バグ） ---------------------------------------- START
            '不要
            'Ret = -300
            '2017/05/22 saitou RSV2 DEL --------------------------------------------------------------- END
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        End Try
    End Function
End Class
