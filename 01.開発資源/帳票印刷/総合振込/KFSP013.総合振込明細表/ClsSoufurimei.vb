Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports System.Collections.Specialized
Imports CASTCommon.ModPublic

Public Class ClsSoufurimei

    '現在日付と時刻
    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    '引数
    Protected Friend TORI_CODE As String
    Protected Friend FURI_DATE As String
    Protected Friend PRINTERNAME As String
    Private FmtComm As New CAstFormat.CFormat
    Private YokuDateList As New StringDictionary

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    ' 機能   ： 総合振込明細表メイン処理
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
        FmtComm.Oracle = MainDB

        Dim nRet As Integer
        Try

            MainLOG.Write("(主処理)開始", "成功")

            ' 印刷処理
            nRet = PrintKouzafurimei()

        Catch ex As Exception
            MainLOG.Write("(主処理)", "失敗", ex.Message & ":" & ex.StackTrace)
            Return -1
        Finally
            MainDB.Close()
            MainLOG.Write("(主処理)終了", "成功")
        End Try

        If nRet < 0 Then
            Return 2
        End If

        Return nRet

    End Function

    ' 機能   ： 総合振込明細表帳票出力処理
    '
    ' 戻り値 ： 0 - 正常 ， -1 - 異常 , 100 - ０件
    '
    ' 備考   ： 
    '
    Private Function PrintKouzafurimei() As Integer

        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim PrnFurimei As New ClsPrnSoufurimei

        SQL = New StringBuilder(128)
        SQL.Append("SELECT * FROM S_MEIMAST,S_TORIMAST,S_SCHMAST,TENMAST")
        SQL.Append(" WHERE FSYORI_KBN_K = '3'")
        SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S AND TORIF_CODE_K=TORIF_CODE_S AND FURI_DATE_K = FURI_DATE_S")
        '2011/06/28 標準版修正 持込SEQを考慮する ------------------START
        SQL.Append(" AND MOTIKOMI_SEQ_K = MOTIKOMI_SEQ_S") '2011/06/13 持込SEQを考慮する
        '2011/06/28 標準版修正 持込SEQを考慮する ------------------END
        SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_T AND TORIF_CODE_K=TORIF_CODE_T")
        SQL.Append(" AND KEIYAKU_KIN_K = KIN_NO_N(+) AND KEIYAKU_SIT_K = SIT_NO_N(+)")
        SQL.Append(" AND TOUROKU_FLG_S = '1'")
        SQL.Append(" AND TYUUDAN_FLG_S = '0'")

        If TORI_CODE <> "999999999999" Then
            SQL.Append(" AND TORIS_CODE_T = '" & TORI_CODE.Substring(0, 10) & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & TORI_CODE.Substring(10, 2) & "'")
        End If

        SQL.Append(" AND FURI_DATE_K = '" & FURI_DATE & "'")
        SQL.Append(" AND DATA_KBN_K = '2'")
        SQL.Append(" AND FURIKETU_CODE_K = 0")
        '2011/06/28 標準版修正 持込SEQを考慮する ------------------START
         SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, MOTIKOMI_SEQ_K, KEIYAKU_KIN_K, KEIYAKU_SIT_K,  RECORD_NO_K")
        'SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, KEIYAKU_KIN_K, KEIYAKU_SIT_K, MOTIKOMI_SEQ_K, RECORD_NO_K")
        '2011/06/28 標準版修正 持込SEQを考慮する ------------------END
        
        Dim name As String = ""

        Dim bSQL As Boolean
        bSQL = OraReader.DataReader(SQL)

        If bSQL = True Then

            name = PrnFurimei.CreateCsvFile

            If name = "" Then
                ' ＣＳＶを作成する
                name = PrnFurimei.CreateCsvFile()
            End If

            Do
                Dim strKAMOKU As String = ""
                Dim YokuDate As String = OraReader.GetString("HASSIN_YDATE_S")

                PrnFurimei.OutputCsvData(OraReader.GetString("RECORD_NO_K"))
                PrnFurimei.OutputCsvData(mMatchingDate)
                PrnFurimei.OutputCsvData(mMatchingTime)
                PrnFurimei.OutputCsvData(OraReader.GetString("FURI_DATE_K"))
                PrnFurimei.OutputCsvData(OraReader.GetString("TORIS_CODE_K"))
                PrnFurimei.OutputCsvData(OraReader.GetString("TORIF_CODE_K"))
                PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))
                PrnFurimei.OutputCsvData(OraReader.GetString("HASSIN_DATE_S"))

                ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                '媒体名をテキストから取得する
                PrnFurimei.OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_総振_媒体コード.TXT"), _
                                                                       OraReader.GetString("BAITAI_CODE_T")))
                'Select Case OraReader.GetString("BAITAI_CODE_T")
                '    Case "00" : PrnFurimei.OutputCsvData("伝送")
                '    Case "01" : PrnFurimei.OutputCsvData("FD3.5")
                '    Case "04" : PrnFurimei.OutputCsvData("依頼書")
                '    Case "05" : PrnFurimei.OutputCsvData("MT")
                '    Case "06" : PrnFurimei.OutputCsvData("CMT")
                '    Case "09" : PrnFurimei.OutputCsvData("伝票")
                '    Case "10" : PrnFurimei.OutputCsvData("WEB伝送")         '2012/06/30 標準版　WEB伝送対応
                '    Case Else : PrnFurimei.OutputCsvData("")
                'End Select
                ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                Select Case OraReader.GetString("SYUBETU_K")
                    Case "21" : PrnFurimei.OutputCsvData("総振")
                    Case "11" : PrnFurimei.OutputCsvData("給与")
                    Case "12" : PrnFurimei.OutputCsvData("賞与")
                    Case Else : PrnFurimei.OutputCsvData("")
                End Select

                If OraReader.GetString("FURI_DATE_S") = OraReader.GetString("HASSIN_YDATE_S") Then
                    PrnFurimei.OutputCsvData("ﾌﾘｺﾐ")
                ElseIf OraReader.GetString("FURI_DATE_S") = YokuDate Then
                    PrnFurimei.OutputCsvData("ｻｷﾌﾘ")
                Else
                    Select Case OraReader.GetString("SYUBETU_K")
                        Case "11" : PrnFurimei.OutputCsvData("ｷﾕｳﾖ")
                        Case "12" : PrnFurimei.OutputCsvData("ｼﾖｳﾖ")
                        Case Else : PrnFurimei.OutputCsvData("ｻｷﾌﾘ")
                    End Select
                End If

                PrnFurimei.OutputCsvData("")
                PrnFurimei.OutputCsvData("")
                PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)
                PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"))
                PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))
                PrnFurimei.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))
                PrnFurimei.OutputCsvData(OraReader.GetString("SIT_NNAME_N"))

                Select Case OraReader.GetString("KEIYAKU_KAMOKU_K")
                    Case "02"
                        strKAMOKU = "普通"
                    Case "01"
                        strKAMOKU = "当座"
                    Case "05"
                        strKAMOKU = "納税"
                    Case "37"
                        strKAMOKU = "職員"
                    Case Else
                        strKAMOKU = "そ他"
                End Select

                PrnFurimei.OutputCsvData(strKAMOKU)
                PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))
                PrnFurimei.OutputCsvData(OraReader.GetString("FURIKIN_K"))
                PrnFurimei.OutputCsvData("")
                PrnFurimei.OutputCsvData(OraReader.GetString("TESUU_KIN_K"))

                If OraReader.GetString("FURI_DATE_S") = OraReader.GetString("HASSIN_YDATE_S") Then
                    Select Case OraReader.GetString("SYUBETU_K")
                        Case "11" : PrnFurimei.OutputCsvData("ｷﾕｳﾖ")
                        Case "12" : PrnFurimei.OutputCsvData("ｼﾖｳﾖ")
                        Case Else : PrnFurimei.OutputCsvData("")
                    End Select
                ElseIf OraReader.GetString("FURI_DATE_S") = YokuDate Then
                    Select Case OraReader.GetString("SYUBETU_K")
                        Case "11" : PrnFurimei.OutputCsvData("ｷﾕｳﾖ")
                        Case "12" : PrnFurimei.OutputCsvData("ｼﾖｳﾖ")
                        Case Else : PrnFurimei.OutputCsvData("")
                    End Select
                Else
                    PrnFurimei.OutputCsvData("")
                End If

                '他行判定区分
                If OraReader.GetString("KEIYAKU_KIN_K") = FmtComm.JIKINKO Then
                    PrnFurimei.OutputCsvData("0", False, True)
                Else
                    PrnFurimei.OutputCsvData("1", False, True)
                End If

                OraReader.NextRead()

            Loop Until OraReader.EOF    ' EOFまで作業を繰り返す。
            OraReader.Close()

            PrnFurimei.CloseCsv()

            If PrnFurimei.ReportExecute(PRINTERNAME) = True Then
                MainLOG.Write("印刷", "成功")
                Return 0
            Else
                MainLOG.Write("印刷", "失敗", PrnFurimei.ReportMessage)
                Return -1
            End If
        Else
            MainLOG.Write("印刷対象データ０件", "成功")
            Return 100
        End If

    End Function

    '翌営業日を返す
    Private Function GetYokuDate(ByVal aDate As String) As String
        If YokuDateList.ContainsKey(aDate) = True Then
            Return YokuDateList.Item(aDate)
        End If

        Dim YokuDate As String = ""
        YokuDate = CASTCommon.GetEigyobi(CASTCommon.ConvertDate(aDate), 1, FmtComm.HolidayList).ToString("yyyyMMdd")
        YokuDateList.Add(aDate, YokuDate)

        Return YokuDate
    End Function
End Class
