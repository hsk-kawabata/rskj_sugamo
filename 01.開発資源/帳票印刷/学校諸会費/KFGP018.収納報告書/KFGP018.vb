Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP018

    Inherits CAstReports.ClsReportBase
    Sub New()

        ' CSVファイルセット
        InfoReport.ReportName = "KFGP018"
        ' 定義体名セット
        ReportBaseName = "KFGP018_収納報告書.rpd"

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
        Dim oraReader2 As CASTCommon.MyOracleReader = Nothing
        ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- START
        Dim GakkouKeiFlg As String = CASTCommon.GetRSKJIni("PRINT", "KFGP018_GOUKEIOUT")
        Dim oraReader3 As CASTCommon.MyOracleReader = Nothing
        ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- END
        Dim SQL As New StringBuilder(128)
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append(" SUM(1) SUM_KEN")                                                               '請求件数
            SQL.Append(",SUM(SEIKYU_KIN_M) SUM_KIN")                                                    '請求金額
            SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 1 ELSE 0 END) FURI_KEN")                  '振替済件数
            SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN SEIKYU_KIN_M ELSE 0 END) FURI_KIN")       '振替済金額
            SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 0 ELSE 1 END) FUNOU_KEN")                 '不能件数
            SQL.Append(",SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 0 ELSE SEIKYU_KIN_M END) FUNOU_KIN")      '不能金額
            SQL.Append(",GAKKOU_CODE_G")
            SQL.Append(",GAKUNEN_CODE_G")
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            SQL.Append(" FROM GAKMAST1,G_MEIMAST,")
            SQL.Append(" (SELECT GAKKOU_CODE_S,FURI_DATE_S,FURI_KBN_S FROM G_SCHMAST")
            SQL.Append("  GROUP BY GAKKOU_CODE_S,FURI_DATE_S,FURI_KBN_S)")
            'SQL.Append(" FROM GAKMAST1,G_MEIMAST,G_SCHMAST")
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_M")
            SQL.Append(" AND GAKUNEN_CODE_G = GAKUNEN_CODE_M")
            SQL.Append(" AND GAKKOU_CODE_G = " & SQ(GakkouCode))
            SQL.Append(" AND FURI_DATE_M = " & SQ(FuriDate))
            SQL.Append(" AND GAKKOU_CODE_S = GAKKOU_CODE_M(+)")
            SQL.Append(" AND FURI_DATE_S = FURI_DATE_M(+)")
            SQL.Append(" AND FURI_KBN_S = FURI_KBN_M(+)")
            SQL.Append(" AND (FURI_KBN_M= '0' OR FURI_KBN_M= '1')")
            SQL.Append(" GROUP BY GAKKOU_CODE_G,GAKUNEN_CODE_G")
            SQL.Append(" ORDER BY GAKKOU_CODE_G,GAKUNEN_CODE_G")


            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    SQL = New StringBuilder()
                    SQL.Append("SELECT DISTINCT")
                    SQL.Append(" GAKKOU_CODE_G")
                    SQL.Append(",GAKKOU_NNAME_G")
                    SQL.Append(",GAKUNEN_CODE_G")
                    SQL.Append(",GAKUNEN_NAME_G")
                    SQL.Append(",FURI_DATE_M")
                    SQL.Append(",JIFURI_KBN_T")
                    SQL.Append(",TKIN_NO_T")
                    SQL.Append(",KIN_NNAME_N")
                    SQL.Append(",TSIT_NO_T")
                    SQL.Append(",SIT_NNAME_N")
                    SQL.Append(",SEIKYU_TUKI_M")

                    For No As Integer = 1 To 15
                        SQL.Append(",HIMOKU_NAME" & No.ToString("00") & "_H")
                        SQL.Append(",KESSAI_KIN_CODE" & No.ToString("00") & "_H")
                        SQL.Append(",KESSAI_TENPO" & No.ToString("00") & "_H")
                        SQL.Append(",KESSAI_KAMOKU" & No.ToString("00") & "_H")
                        SQL.Append(",KESSAI_KOUZA" & No.ToString("00") & "_H")
                    Next

                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    SQL.Append(",FURI_CODE_T")
                    SQL.Append(",KIGYO_CODE_T")
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
                    '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                    SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
                    SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
                    SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
                    '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END

                    SQL.Append(" FROM GAKMAST1,GAKMAST2,G_MEIMAST,TENMAST,HIMOMAST,G_SCHMAST")
                    SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")

                    SQL.Append(" AND GAKKOU_CODE_M = GAKKOU_CODE_G")
                    SQL.Append(" AND GAKUNEN_CODE_M = GAKUNEN_CODE_G")

                    SQL.Append(" AND GAKKOU_CODE_G = " & SQ(oraReader.GetString("GAKKOU_CODE_G")))
                    SQL.Append(" AND GAKUNEN_CODE_G = " & SQ(oraReader.GetString("GAKUNEN_CODE_G")))

                    SQL.Append(" AND TKIN_NO_T = KIN_NO_N(+)")
                    SQL.Append(" AND TSIT_NO_T = SIT_NO_N(+)")

                    SQL.Append(" AND GAKKOU_CODE_M = GAKKOU_CODE_H")
                    SQL.Append(" AND GAKUNEN_CODE_M = GAKUNEN_CODE_H")
                    SQL.Append(" AND HIMOKU_ID_M = HIMOKU_ID_H")
                    SQL.Append(" AND SUBSTR(SEIKYU_TUKI_M,5,2) = TUKI_NO_H")
                    SQL.Append(" AND FURI_DATE_M = " & SQ(FuriDate))

                    SQL.Append(" AND GAKKOU_CODE_S = GAKKOU_CODE_M(+)")
                    SQL.Append(" AND FURI_DATE_S = FURI_DATE_M(+)")
                    SQL.Append(" AND FURI_KBN_S = FURI_KBN_M(+)")
                    SQL.Append(" AND (FURI_KBN_M= '0' OR FURI_KBN_M= '1')")
                    '2010/10/21 請求月毎に並べる----------
                    SQL.Append(" ORDER BY SEIKYU_TUKI_M")
                    '-------------------------------------

                    oraReader2 = New CASTCommon.MyOracleReader(oraDB)
                    If oraReader2.DataReader(SQL) = True Then
                        '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                        User_ID = oraReader2.GetString("YOBI1_T")
                        BaitaiCode = oraReader2.GetString("BAITAI_CODE_T")
                        ITAKU_CODE = oraReader2.GetString("ITAKU_CODE_T")
                        '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                        '2010/10/21 oraReader2.NextRead()追加
                        While oraReader2.EOF = False
                            '--------------------------------
                            For No As Integer = 1 To 15
                                Dim HimoKen As Long = 0
                                Dim HimoKin As Long = 0

                                OutputCsvData(mMatchingDate)                                            '処理日
                                OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("JIFURI_KBN_T")))         '自振区分
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("GAKKOU_CODE_G")))        '学校コード
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("GAKKOU_NNAME_G")))       '学校名
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("GAKUNEN_CODE_G")))       '学年
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("GAKUNEN_NAME_G")))       '学年名
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("FURI_DATE_M")))          '振替日
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("SEIKYU_TUKI_M")))        '対象年月
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("TKIN_NO_T")))            '取扱金融機関コード
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("KIN_NNAME_N")))          '取扱金融機関名
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("TSIT_NO_T")))            '取扱支店コード
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("SIT_NNAME_N")))          '取扱支店名
                                OutputCsvData(GCOM.NzStr(No))                                           '費目番号
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("HIMOKU_NAME" & No.ToString("00") & "_H"))) '費目名
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("KESSAI_TENPO" & No.ToString("00") & "_H"))) '決済支店コード

                                Select Case GCOM.NzStr(oraReader2.GetString("KESSAI_KAMOKU" & No.ToString("00") & "_H")) '科目
                                    Case "01"
                                        OutputCsvData("当座")
                                    Case "02"
                                        OutputCsvData("普通")
                                    Case "09"
                                        OutputCsvData("その他")
                                    Case Else
                                        OutputCsvData("")
                                End Select
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("KESSAI_KOUZA" & No.ToString("00") & "_H"))) '口座番号

                                '2010/10/21 費目、請求月ごと済件数・金額取得------------------------------------------------------
                                '費目ごと済件数・金額取得
                                'If GetHimoData(oraReader2.GetString("GAKKOU_CODE_G"), oraReader2.GetString("GAKUNEN_CODE_G"), _
                                '               True, No, oraDB, HimoKen, HimoKin) = False Then
                                '    Return False
                                'End If
                                If GetHimoData(oraReader2.GetString("GAKKOU_CODE_G"), oraReader2.GetString("GAKUNEN_CODE_G"), oraReader2.GetString("SEIKYU_TUKI_M"), _
                                               True, No, oraDB, HimoKen, HimoKin) = False Then
                                    Return False
                                End If
                                '--------------------------------------------------------------------------------------------------
                                OutputCsvData(GCOM.NzStr(HimoKen))                          '費目振替件数
                                OutputCsvData(GCOM.NzStr(HimoKin))                          '費目振替金額

                                '2010/10/21 費目、請求月ごと不能件数・金額取得
                                '費目ごと不能件数・金額取得
                                'If GetHimoData(oraReader2.GetString("GAKKOU_CODE_G"), oraReader2.GetString("GAKUNEN_CODE_G"), _
                                '               False, No, oraDB, HimoKen, HimoKin) = False Then
                                '    Return False
                                'End If
                                If GetHimoData(oraReader2.GetString("GAKKOU_CODE_G"), oraReader2.GetString("GAKUNEN_CODE_G"), oraReader2.GetString("SEIKYU_TUKI_M"), _
                                               False, No, oraDB, HimoKen, HimoKin) = False Then
                                    Return False
                                End If
                                '-----------------------------------------------------------------------------------------------------
                                OutputCsvData(GCOM.NzStr(HimoKen))                          '費目不能件数
                                OutputCsvData(GCOM.NzStr(HimoKin))                          '費目不能金額
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("SUM_KEN")))   '請求件数
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("SUM_KIN")))   '請求金額
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FURI_KEN")))  '振替済件数
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FURI_KIN")))  '振替済金額
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FUNOU_KEN"))) '振替不能件数
                                ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                                ' 振替コード、企業コードを追加（【振替不能金額】は最終項目でなくなったため改行を削除）
                                'OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FUNOU_KIN")), False, True) '振替不能金額
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FUNOU_KIN"))) '振替不能金額
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("FURI_CODE_T")))               '振替コード
                                OutputCsvData(GCOM.NzStr(oraReader2.GetString("KIGYO_CODE_T")), False, True) '企業コード
                                ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

                                RecordCnt += 1
                            Next
                            '2010/10/21 oraReader2.NextRead()追加
                            oraReader2.NextRead()
                        End While
                        oraReader2.Close()
                    End If
                    oraReader.NextRead()
                End While
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If

            ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- START
            If GakkouKeiFlg <> "0" Then
                '--------------------------------------------------------------
                ' 学年合計
                '--------------------------------------------------------------
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append("   SUM(1)                                                                   SUM_KEN")   '請求件数
                SQL.Append(" , SUM(SEIKYU_KIN_M)                                                        SUM_KIN")   '請求金額
                SQL.Append(" , SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 1            ELSE 0            END) FURI_KEN")  '振替済件数
                SQL.Append(" , SUM(CASE FURIKETU_CODE_M WHEN 0 THEN SEIKYU_KIN_M ELSE 0            END) FURI_KIN")  '振替済金額
                SQL.Append(" , SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 0            ELSE 1            END) FUNOU_KEN") '不能件数
                SQL.Append(" , SUM(CASE FURIKETU_CODE_M WHEN 0 THEN 0            ELSE SEIKYU_KIN_M END) FUNOU_KIN") '不能金額
                SQL.Append(" , GAKKOU_CODE_G")
                SQL.Append(" FROM  GAKMAST1 , G_MEIMAST , G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_G  = GAKKOU_CODE_M")
                SQL.Append(" AND   GAKUNEN_CODE_G = GAKUNEN_CODE_M")
                SQL.Append(" AND   GAKKOU_CODE_G  = " & SQ(GakkouCode))
                SQL.Append(" AND   FURI_DATE_M    = " & SQ(FuriDate))
                SQL.Append(" AND   GAKKOU_CODE_S  = GAKKOU_CODE_M(+)")
                SQL.Append(" AND   FURI_DATE_S    = FURI_DATE_M(+)")
                SQL.Append(" AND   FURI_KBN_S     = FURI_KBN_M(+)")
                SQL.Append(" AND ( FURI_KBN_M     = '0'")
                SQL.Append("  OR   FURI_KBN_M     = '1' )")
                SQL.Append(" GROUP BY GAKKOU_CODE_G")
                SQL.Append(" ORDER BY GAKKOU_CODE_G")

                If oraReader.DataReader(SQL) = True Then
                    While oraReader.EOF = False

                        SQL.Length = 0
                        SQL.Append("SELECT")
                        SQL.Append("  HIMOKU_ID_H")
                        For No As Integer = 1 To 15
                            SQL.Append(", HIMOKU_NAME" & No.ToString("00") & "_H")
                            SQL.Append(", KESSAI_KIN_CODE" & No.ToString("00") & "_H")
                            SQL.Append(", KESSAI_TENPO" & No.ToString("00") & "_H")
                            SQL.Append(", KESSAI_KAMOKU" & No.ToString("00") & "_H")
                            SQL.Append(", KESSAI_KOUZA" & No.ToString("00") & "_H")
                        Next
                        SQL.Append(" FROM  HIMOMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_H  = " & SQ(oraReader.GetString("GAKKOU_CODE_G")))
                        SQL.Append(" AND   HIMOKU_ID_H   <> '000'")
                        SQL.Append(" AND   ROWNUM         = 1")
                        SQL.Append(" ORDER BY GAKUNEN_CODE_H , HIMOKU_ID_H , TUKI_NO_H")

                        oraReader3 = New CASTCommon.MyOracleReader(oraDB)
                        If oraReader3.DataReader(SQL) = True Then

                            SQL.Length = 0
                            SQL.Append("SELECT DISTINCT")
                            SQL.Append(" GAKKOU_CODE_G")
                            SQL.Append(",GAKKOU_NNAME_G")
                            SQL.Append(",FURI_DATE_M")
                            SQL.Append(",JIFURI_KBN_T")
                            SQL.Append(",TKIN_NO_T")
                            SQL.Append(",KIN_NNAME_N")
                            SQL.Append(",TSIT_NO_T")
                            SQL.Append(",SIT_NNAME_N")
                            SQL.Append(",SEIKYU_TUKI_M")
                            SQL.Append(",FURI_CODE_T")
                            SQL.Append(",KIGYO_CODE_T")
                            SQL.Append(" FROM  GAKMAST1 , GAKMAST2 , G_MEIMAST , TENMAST , G_SCHMAST")
                            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
                            SQL.Append(" AND   GAKKOU_CODE_M = GAKKOU_CODE_G")
                            SQL.Append(" AND   GAKKOU_CODE_G = " & SQ(oraReader.GetString("GAKKOU_CODE_G")))
                            SQL.Append(" AND   TKIN_NO_T     = KIN_NO_N(+)")
                            SQL.Append(" AND   TSIT_NO_T     = SIT_NO_N(+)")
                            SQL.Append(" AND   FURI_DATE_M   = " & SQ(FuriDate))
                            SQL.Append(" AND   GAKKOU_CODE_S = GAKKOU_CODE_M(+)")
                            SQL.Append(" AND   FURI_DATE_S   = FURI_DATE_M(+)")
                            SQL.Append(" AND   FURI_KBN_S    = FURI_KBN_M(+)")
                            SQL.Append(" AND ( FURI_KBN_M    = '0'")
                            SQL.Append("  OR   FURI_KBN_M    = '1' )")
                            SQL.Append(" ORDER BY SEIKYU_TUKI_M")

                            oraReader2 = New CASTCommon.MyOracleReader(oraDB)
                            If oraReader2.DataReader(SQL) = True Then
                                While oraReader2.EOF = False
                                    For No As Integer = 1 To 15
                                        Dim HimoKen As Long = 0
                                        Dim HimoKin As Long = 0

                                        OutputCsvData(mMatchingDate)                                            '処理日
                                        OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("JIFURI_KBN_T")))         '自振区分
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("GAKKOU_CODE_G")))        '学校コード
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("GAKKOU_NNAME_G")))       '学校名
                                        OutputCsvData("0")                                                      '学年
                                        OutputCsvData("学校計")                                                 '学年名
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("FURI_DATE_M")))          '振替日
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("SEIKYU_TUKI_M")))        '対象年月
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("TKIN_NO_T")))            '取扱金融機関コード
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("KIN_NNAME_N")))          '取扱金融機関名
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("TSIT_NO_T")))            '取扱支店コード
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("SIT_NNAME_N")))          '取扱支店名
                                        OutputCsvData(GCOM.NzStr(No))                                           '費目番号
                                        OutputCsvData(GCOM.NzStr(oraReader3.GetString("HIMOKU_NAME" & No.ToString("00") & "_H"))) '費目名
                                        OutputCsvData(GCOM.NzStr(oraReader3.GetString("KESSAI_TENPO" & No.ToString("00") & "_H"))) '決済支店コード

                                        Select Case GCOM.NzStr(oraReader3.GetString("KESSAI_KAMOKU" & No.ToString("00") & "_H")) '科目
                                            Case "01"
                                                OutputCsvData("当座")
                                            Case "02"
                                                OutputCsvData("普通")
                                            Case "09"
                                                OutputCsvData("その他")
                                            Case Else
                                                OutputCsvData("")
                                        End Select
                                        OutputCsvData(GCOM.NzStr(oraReader3.GetString("KESSAI_KOUZA" & No.ToString("00") & "_H"))) '口座番号

                                        If GetHimoData_GakSum(oraReader2.GetString("GAKKOU_CODE_G"), oraReader2.GetString("SEIKYU_TUKI_M"), _
                                                       True, No, oraDB, HimoKen, HimoKin) = False Then
                                            Return False
                                        End If
                                        OutputCsvData(GCOM.NzStr(HimoKen))                          '費目振替件数
                                        OutputCsvData(GCOM.NzStr(HimoKin))                          '費目振替金額

                                        If GetHimoData_GakSum(oraReader2.GetString("GAKKOU_CODE_G"), oraReader2.GetString("SEIKYU_TUKI_M"), _
                                                       False, No, oraDB, HimoKen, HimoKin) = False Then
                                            Return False
                                        End If
                                        OutputCsvData(GCOM.NzStr(HimoKen))                         '費目不能件数
                                        OutputCsvData(GCOM.NzStr(HimoKin))                         '費目不能金額
                                        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("SUM_KEN")))   '請求件数
                                        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("SUM_KIN")))   '請求金額
                                        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FURI_KEN")))  '振替済件数
                                        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FURI_KIN")))  '振替済金額
                                        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FUNOU_KEN"))) '振替不能件数
                                        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("FUNOU_KIN"))) '振替不能金額
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("FURI_CODE_T")))               '振替コード
                                        OutputCsvData(GCOM.NzStr(oraReader2.GetString("KIGYO_CODE_T")), False, True) '企業コード

                                        RecordCnt += 1
                                    Next
                                    oraReader2.NextRead()
                                End While
                                oraReader2.Close()
                            End If
                        End If
                        oraReader.NextRead()
                    End While
                End If
            End If
            ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- END

            Return True

        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not oraReader2 Is Nothing Then oraReader.Close()
            ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- START
            If Not oraReader3 Is Nothing Then oraReader.Close()
            ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- END
        End Try
    End Function

    '2010/10/21 請求月を引数に追加
    'Private Function GetHimoData(ByVal Gakkou As String, ByVal Gakunen As Integer, _
    '                             ByVal Zumi As Boolean, ByVal HimokuNo As Integer, ByVal OraDB As CASTCommon.MyOracle, _
    '                             ByRef HimokuKen As Long, ByRef HimokuKin As Long) As Boolean
    Private Function GetHimoData(ByVal Gakkou As String, ByVal Gakunen As Integer, ByVal SEIKYU_TUKI As String, _
                             ByVal Zumi As Boolean, ByVal HimokuNo As Integer, ByVal OraDB As CASTCommon.MyOracle, _
                             ByRef HimokuKen As Long, ByRef HimokuKin As Long) As Boolean

        GetHimoData = False
        HimokuKen = 0
        HimokuKin = 0
        '振替済:結果コード=0かつ金額が0円以上 不能:結果コード<> 0
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            SQL.Append("SELECT")
            SQL.Append(" SUM(CASE HIMOKU" & HimokuNo.ToString & "_KIN_M WHEN 0 THEN 0 ELSE 1 END) SUM_KEN")     '請求件数
            SQL.Append(",SUM(HIMOKU" & HimokuNo.ToString & "_KIN_M) SUM_KIN")       '請求金額
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            SQL.Append(" FROM GAKMAST1,G_MEIMAST,")
            SQL.Append("(SELECT GAKKOU_CODE_S,FURI_DATE_S,FURI_KBN_S FROM G_SCHMAST")
            SQL.Append(" GROUP BY GAKKOU_CODE_S,FURI_DATE_S,FURI_KBN_S) G_SCHMAST")
            'SQL.Append(" FROM GAKMAST1,G_MEIMAST,G_SCHMAST")
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_M")
            SQL.Append(" AND GAKUNEN_CODE_G = GAKUNEN_CODE_M")
            SQL.Append(" AND GAKKOU_CODE_G = " & SQ(Gakkou))
            SQL.Append(" AND GAKUNEN_CODE_G = " & SQ(Gakunen))
            SQL.Append(" AND FURI_DATE_M = " & SQ(FuriDate))
            SQL.Append(" AND GAKKOU_CODE_S = GAKKOU_CODE_M(+)")
            SQL.Append(" AND FURI_DATE_S = FURI_DATE_M(+)")
            SQL.Append(" AND FURI_KBN_S = FURI_KBN_M(+)")
            If Zumi Then
                SQL.Append(" AND FURIKETU_CODE_M = 0")
            Else
                SQL.Append(" AND FURIKETU_CODE_M <> 0")
            End If
            SQL.Append(" AND SEIKYU_KIN_M > 0")
            '2010/10/21 請求月ごとの判定を追加-------------------
            SQL.Append(" AND SEIKYU_TUKI_M = " & SQ(SEIKYU_TUKI))
            '----------------------------------------------------
            SQL.Append(" GROUP BY GAKKOU_CODE_G,GAKKOU_CODE_G")
            SQL.Append(" ORDER BY GAKKOU_CODE_G,GAKKOU_CODE_G")
            If OraReader.DataReader(SQL) Then
                HimokuKen = OraReader.GetInt64("SUM_KEN")
                HimokuKin = OraReader.GetInt64("SUM_KIN")
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("費目別件数・金額取得", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function

    ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- START
    Private Function GetHimoData_GakSum(ByVal Gakkou As String, ByVal SEIKYU_TUKI As String, _
                             ByVal Zumi As Boolean, ByVal HimokuNo As Integer, ByVal OraDB As CASTCommon.MyOracle, _
                             ByRef HimokuKen As Long, ByRef HimokuKin As Long) As Boolean

        GetHimoData_GakSum = False
        HimokuKen = 0
        HimokuKin = 0
        '振替済:結果コード=0かつ金額が0円以上 不能:結果コード<> 0
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            SQL.Append("SELECT")
            SQL.Append(" SUM(CASE HIMOKU" & HimokuNo.ToString & "_KIN_M WHEN 0 THEN 0 ELSE 1 END) SUM_KEN")     '請求件数
            SQL.Append(",SUM(HIMOKU" & HimokuNo.ToString & "_KIN_M) SUM_KIN")       '請求金額
            SQL.Append(" FROM GAKMAST1,G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_M")
            SQL.Append(" AND GAKUNEN_CODE_G = GAKUNEN_CODE_M")
            SQL.Append(" AND GAKKOU_CODE_G = " & SQ(Gakkou))
            SQL.Append(" AND FURI_DATE_M = " & SQ(FuriDate))
            SQL.Append(" AND GAKKOU_CODE_S = GAKKOU_CODE_M(+)")
            SQL.Append(" AND FURI_DATE_S = FURI_DATE_M(+)")
            SQL.Append(" AND FURI_KBN_S = FURI_KBN_M(+)")
            If Zumi Then
                SQL.Append(" AND FURIKETU_CODE_M = 0")
            Else
                SQL.Append(" AND FURIKETU_CODE_M <> 0")
            End If
            SQL.Append(" AND SEIKYU_KIN_M > 0")
            SQL.Append(" AND SEIKYU_TUKI_M = " & SQ(SEIKYU_TUKI))
            SQL.Append(" GROUP BY GAKKOU_CODE_G,GAKKOU_CODE_G")
            SQL.Append(" ORDER BY GAKKOU_CODE_G,GAKKOU_CODE_G")
            If OraReader.DataReader(SQL) Then
                HimokuKen = OraReader.GetInt64("SUM_KEN")
                HimokuKin = OraReader.GetInt64("SUM_KIN")
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("費目別件数・金額取得", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function
    ' 2017/04/24 タスク）綾部 ADD【ME】UI_99-99(RSV2 収納報告書の学校計出力対応) -------------------- END

End Class
