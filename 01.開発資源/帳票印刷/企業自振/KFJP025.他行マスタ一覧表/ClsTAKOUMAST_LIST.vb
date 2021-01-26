Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic
' 
Public Class ClsTAKOUMAST_LIST

    ' 引数
    Public TORIS_CODE As String         ' 取引先主コード
    Public TORIF_CODE As String         ' 取引先副コード
    Public ALL_PRINT As String          ' 印刷対象フラグ

    ' ログ処理クラス

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    ' FSKJ.INI セクション名
    Private ReadOnly AppTOUROKU As String = "REPORTS"

    ' 機能   ： 振替結果明細表メイン処理
    '
    ' 引数   ： なし
    '
    ' 戻り値 ： 0 - 正常 0以外 - 異常
    '
    ' 備考   ： 
    '
    Function Main() As Integer
        ' オラクル
        MainDB = New CASTCommon.MyOracle

        Dim bRet As Boolean
        Try
            'MainLOG = New CASTCommon.BatchLOG("他行マスタ一覧表", AppTOUROKU)

            MainLOG.ToriCode = TORIS_CODE & TORIF_CODE

            ' 他行マスタ一覧表印刷処理
            bRet = PrintTAKOUMAST_LIST()

        Catch ex As Exception
            MainLOG.Write("他行マスタ一覧表", "失敗", ex.Message & ":" & ex.StackTrace)

            Return -1
        Finally
            MainDB.Close()
        End Try

        If bRet Then
            Return 0
        Else
            Return 2
        End If

    End Function

    ' 機能   ： 他行マスタ一覧表出力処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考   ： 
    '
    Private Function PrintTAKOUMAST_LIST() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim TenSQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim OraTenReader As New CASTCommon.MyOracleReader(MainDB)
        Dim sFMTKbn As String = ""

        Try
            MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "他行マスタ一覧表作成処理(開始)", "成功", "")

            Dim PrnTAKOUMAST_LIST As New ClsPrnTAKOUMAST_LIST
            Dim name As String = ""

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("  TORIS_CODE_V")
            SQL.Append(", TORIF_CODE_V")
            SQL.Append(", TKIN_NO_V")
            SQL.Append(", TSIT_NO_V")
            SQL.Append(", ITAKU_CODE_V")
            SQL.Append(", KAMOKU_V")
            SQL.Append(", KOUZA_V")
            SQL.Append(", BAITAI_CODE_V")
            SQL.Append(", CODE_KBN_V")
            SQL.Append(", SFILE_NAME_V")
            SQL.Append(", RFILE_NAME_V")
            SQL.Append(", ITAKU_NNAME_T")
            SQL.Append(" FROM TAKOUMAST, TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = '1'")
            SQL.Append(" AND TAKO_KBN_T = '1'")
            SQL.Append(" AND TORIS_CODE_V = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_V = TORIF_CODE_T")
            If ALL_PRINT = "0" Then
                SQL.Append(" AND TORIS_CODE_V = " & SQ(TORIS_CODE))
                SQL.Append(" AND TORIF_CODE_V = " & SQ(TORIF_CODE))
            End If
            SQL.Append(" ORDER BY TORIS_CODE_V")
            SQL.Append(", TORIF_CODE_V")
            SQL.Append(", TKIN_NO_V")

            'SQL.Append("SELECT")
            'SQL.Append(" TORIS_CODE_V")
            'SQL.Append(", TORIF_CODE_V")
            'SQL.Append(", TKIN_NO_V")
            'SQL.Append(", KIN_NNAME_N")
            'SQL.Append(", TSIT_NO_V")
            ''   SQL.Append(", SIT_NNAME_N")
            'SQL.Append(", ITAKU_CODE_V")
            'SQL.Append(", KAMOKU_V")
            'SQL.Append(", KOUZA_V")
            'SQL.Append(", BAITAI_CODE_V")
            'SQL.Append(", CODE_KBN_V")
            'SQL.Append(", SFILE_NAME_V")
            'SQL.Append(", RFILE_NAME_V")
            'SQL.Append(", ITAKU_NNAME_T")
            'SQL.Append(", KIN_NO_N")
            'SQL.Append(", SIT_NO_N")
            'SQL.Append(" FROM TAKOUMAST")
            'SQL.Append(", (SELECT MIN(KIN_NO_N) KIN_NO_N ")
            'SQL.Append(", MIN(SIT_NO_N) SIT_NO_N")
            'SQL.Append(", KIN_NNAME_N")
            ''   SQL.Append(", SIT_NNAME_N")
            'SQL.Append(" FROM TENMAST")
            'SQL.Append(" GROUP BY KIN_NO_N")
            'SQL.Append(", SIT_NO_N")
            ''   SQL.Append(", KIN_NNAME_N")
            ''   SQL.Append(", SIT_NNAME_N) TENMAST")
            'SQL.Append(", KIN_NNAME_N")
            'SQL.Append(" ORDER BY KIN_NO_N")
            'SQL.Append(", SIT_NO_N) TENMAST")
            'SQL.Append(", (SELECT FSYORI_KBN_T")
            'SQL.Append(", TORIS_CODE_T")
            'SQL.Append(", TORIF_CODE_T")
            'SQL.Append(", ITAKU_NNAME_T")
            'SQL.Append(" FROM TORIMAST")
            'SQL.Append(" WHERE FSYORI_KBN_T = '1'")
            'SQL.Append(" AND TAKO_KBN_T = '1')")
            'SQL.Append(" WHERE TKIN_NO_V = KIN_NO_N (+)")
            'SQL.Append(" AND TSIT_NO_V = SIT_NO_N (+) ")
            'SQL.Append(" AND TORIS_CODE_V = TORIS_CODE_T")
            'SQL.Append(" AND TORIF_CODE_V = TORIF_CODE_T")
            'If ALL_PRINT = "0" Then
            '    SQL.Append(" AND TORIS_CODE_V = " & SQ(TORIS_CODE))
            '    SQL.Append(" AND TORIF_CODE_V = " & SQ(TORIF_CODE))
            'End If
            'SQL.Append(" ORDER BY TORIS_CODE_V")
            'SQL.Append(", TORIF_CODE_V")
            'SQL.Append(", TKIN_NO_V")

            Dim strSystemDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
            Dim strTimeStamp As String = CASTCommon.Calendar.Now.ToString("HHmmss")

            If OraReader.DataReader(SQL) = True Then

                name = PrnTAKOUMAST_LIST.CreateCsvFile

                Do While OraReader.EOF = False

                    TenSQL = New StringBuilder(128)
                    TenSQL.Append("SELECT KIN_NNAME_N,SIT_NNAME_N")
                    TenSQL.Append(" FROM TENMAST")
                    TenSQL.Append(" WHERE KIN_NO_N = '" & OraReader.GetString("TKIN_NO_V") & "'")
                    TenSQL.Append(" AND SIT_NO_N ='" & OraReader.GetString("TSIT_NO_V") & "'")
                    TenSQL.Append(" ORDER BY KIN_NO_N")
                    TenSQL.Append(", SIT_NO_N")

                    Dim strKIN_NNAME_N As String = ""
                    Dim strSIT_NNAME_N As String = ""

                    If OraTenReader.DataReader(TenSQL) = True Then
                        strKIN_NNAME_N = OraTenReader.GetString("KIN_NNAME_N")
                        strSIT_NNAME_N = OraTenReader.GetString("SIT_NNAME_N")
                    Else
                        strKIN_NNAME_N = ""
                        strSIT_NNAME_N = ""
                    End If

                    PrnTAKOUMAST_LIST.OutputCsvData(strSystemDate)
                    PrnTAKOUMAST_LIST.OutputCsvData(strTimeStamp)
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("TORIS_CODE_V"))
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("TORIF_CODE_V"))
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("TKIN_NO_V"))
                    'PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))
                    PrnTAKOUMAST_LIST.OutputCsvData(strKIN_NNAME_N)
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("TSIT_NO_V"))
                    '   PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("SIT_NNAME_N"))
                    PrnTAKOUMAST_LIST.OutputCsvData(strSIT_NNAME_N)

                    Select Case OraReader.GetString("KAMOKU_V")
                        Case "01"
                            PrnTAKOUMAST_LIST.OutputCsvData("当座")
                        Case "02"
                            PrnTAKOUMAST_LIST.OutputCsvData("普通")
                        Case "05"
                            PrnTAKOUMAST_LIST.OutputCsvData("納税")
                        Case "37"
                            PrnTAKOUMAST_LIST.OutputCsvData("職員")
                        Case "04"
                            PrnTAKOUMAST_LIST.OutputCsvData("別段")
                        Case "99"
                            PrnTAKOUMAST_LIST.OutputCsvData("諸勘定")
                        Case Else
                            PrnTAKOUMAST_LIST.OutputCsvData("")
                    End Select

                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("KOUZA_V"))
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("ITAKU_CODE_V"))

                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    PrnTAKOUMAST_LIST.OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFJMAST041_媒体コード.TXT"), _
                                                                                  OraReader.GetString("BAITAI_CODE_V")))
                    'Select Case OraReader.GetString("BAITAI_CODE_V")
                    '    Case "00"
                    '        PrnTAKOUMAST_LIST.OutputCsvData("伝送")
                    '    Case "01", "02"
                    '        PrnTAKOUMAST_LIST.OutputCsvData("FD3.5")
                    '    Case "04"
                    '        PrnTAKOUMAST_LIST.OutputCsvData("依頼書")
                    '    Case "05"
                    '        PrnTAKOUMAST_LIST.OutputCsvData("MT")
                    '    Case "06"
                    '        PrnTAKOUMAST_LIST.OutputCsvData("CMT")
                    '    Case "07"
                    '        PrnTAKOUMAST_LIST.OutputCsvData("学校自振")
                    '    Case "09"
                    '        PrnTAKOUMAST_LIST.OutputCsvData("伝票")
                    '    Case Else
                    '        PrnTAKOUMAST_LIST.OutputCsvData("")
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                    Select Case OraReader.GetString("CODE_KBN_V")
                        Case "0"
                            PrnTAKOUMAST_LIST.OutputCsvData("JIS")
                        Case "1"
                            PrnTAKOUMAST_LIST.OutputCsvData("JIS改有(120)")
                        Case "2"
                            PrnTAKOUMAST_LIST.OutputCsvData("JIS改有(119)")
                        Case "3"
                            PrnTAKOUMAST_LIST.OutputCsvData("JIS改有(118)")
                        Case "4"
                            PrnTAKOUMAST_LIST.OutputCsvData("EBCDIC")
                        Case Else
                            PrnTAKOUMAST_LIST.OutputCsvData("")
                    End Select
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("SFILE_NAME_V"))
                    PrnTAKOUMAST_LIST.OutputCsvData(OraReader.GetString("RFILE_NAME_V"), False, True)

                    OraReader.NextRead()
                Loop
            End If

            OraReader.Close()
            OraTenReader.Close()

            If PrnTAKOUMAST_LIST.ReportExecute() = True Then
            Else

                MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "他行マスタ一覧表作成処理", "失敗", "")
                Return False
            End If

            MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "他行マスタ一覧表作成処理(終了)", "成功", "")
            Return True

        Catch ex As Exception
            MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "他行マスタ一覧表作成処理", "失敗", ex.Message)
            Return False
        End Try

    End Function

End Class
