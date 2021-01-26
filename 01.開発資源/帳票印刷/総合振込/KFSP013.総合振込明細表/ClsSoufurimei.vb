Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports System.Collections.Specialized
Imports CASTCommon.ModPublic

Public Class ClsSoufurimei

    '尰嵼擔晅偲帪崗
    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    '堷悢
    Protected Friend TORI_CODE As String
    Protected Friend FURI_DATE As String
    Protected Friend PRINTERNAME As String
    Private FmtComm As New CAstFormat.CFormat
    Private YokuDateList As New StringDictionary

    ' 僷僽儕僢僋俢俛
    Private MainDB As CASTCommon.MyOracle

    ' 婡擻   丗 憤崌怳崬柧嵶昞儊僀儞張棟
    '
    ' 堷悢   丗 側偟
    '
    ' 栠傝抣 丗 0 - 惓忢 0埲奜 - 堎忢
    '
    ' 旛峫   丗 
    '
    Function Main() As Integer
        ' 僆儔僋儖
        MainDB = New CASTCommon.MyOracle
        FmtComm.Oracle = MainDB

        Dim nRet As Integer
        Try

            MainLOG.Write("(庡張棟)奐巒", "惉岟")

            ' 報嶞張棟
            nRet = PrintKouzafurimei()

        Catch ex As Exception
            MainLOG.Write("(庡張棟)", "幐攕", ex.Message & ":" & ex.StackTrace)
            Return -1
        Finally
            MainDB.Close()
            MainLOG.Write("(庡張棟)廔椆", "惉岟")
        End Try

        If nRet < 0 Then
            Return 2
        End If

        Return nRet

    End Function

    ' 婡擻   丗 憤崌怳崬柧嵶昞挔昜弌椡張棟
    '
    ' 栠傝抣 丗 0 - 惓忢 丆 -1 - 堎忢 , 100 - 侽審
    '
    ' 旛峫   丗 
    '
    Private Function PrintKouzafurimei() As Integer

        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim PrnFurimei As New ClsPrnSoufurimei

        SQL = New StringBuilder(128)
        SQL.Append("SELECT * FROM S_MEIMAST,S_TORIMAST,S_SCHMAST,TENMAST")
        SQL.Append(" WHERE FSYORI_KBN_K = '3'")
        SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S AND TORIF_CODE_K=TORIF_CODE_S AND FURI_DATE_K = FURI_DATE_S")
        '2011/06/28 昗弨斉廋惓 帩崬SEQ傪峫椂偡傞 ------------------START
        SQL.Append(" AND MOTIKOMI_SEQ_K = MOTIKOMI_SEQ_S") '2011/06/13 帩崬SEQ傪峫椂偡傞
        '2011/06/28 昗弨斉廋惓 帩崬SEQ傪峫椂偡傞 ------------------END
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
        '2011/06/28 昗弨斉廋惓 帩崬SEQ傪峫椂偡傞 ------------------START
         SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, MOTIKOMI_SEQ_K, KEIYAKU_KIN_K, KEIYAKU_SIT_K,  RECORD_NO_K")
        'SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, KEIYAKU_KIN_K, KEIYAKU_SIT_K, MOTIKOMI_SEQ_K, RECORD_NO_K")
        '2011/06/28 昗弨斉廋惓 帩崬SEQ傪峫椂偡傞 ------------------END
        
        Dim name As String = ""

        Dim bSQL As Boolean
        bSQL = OraReader.DataReader(SQL)

        If bSQL = True Then

            name = PrnFurimei.CreateCsvFile

            If name = "" Then
                ' 俠俽倁傪嶌惉偡傞
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

                ' 2016/01/23 僞僗僋乯嵵摗 UPD 亂PG亃UI_B-14-99(RSV2懳墳) -------------------- START
                '攠懱柤傪僥僉僗僩偐傜庢摼偡傞
                PrnFurimei.OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_憤怳_攠懱僐乕僪.TXT"), _
                                                                       OraReader.GetString("BAITAI_CODE_T")))
                'Select Case OraReader.GetString("BAITAI_CODE_T")
                '    Case "00" : PrnFurimei.OutputCsvData("揱憲")
                '    Case "01" : PrnFurimei.OutputCsvData("FD3.5")
                '    Case "04" : PrnFurimei.OutputCsvData("埶棅彂")
                '    Case "05" : PrnFurimei.OutputCsvData("MT")
                '    Case "06" : PrnFurimei.OutputCsvData("CMT")
                '    Case "09" : PrnFurimei.OutputCsvData("揱昜")
                '    Case "10" : PrnFurimei.OutputCsvData("WEB揱憲")         '2012/06/30 昗弨斉丂WEB揱憲懳墳
                '    Case Else : PrnFurimei.OutputCsvData("")
                'End Select
                ' 2016/01/23 僞僗僋乯嵵摗 UPD 亂PG亃UI_B-14-99(RSV2懳墳) -------------------- END

                Select Case OraReader.GetString("SYUBETU_K")
                    Case "21" : PrnFurimei.OutputCsvData("憤怳")
                    Case "11" : PrnFurimei.OutputCsvData("媼梌")
                    Case "12" : PrnFurimei.OutputCsvData("徿梌")
                    Case Else : PrnFurimei.OutputCsvData("")
                End Select

                If OraReader.GetString("FURI_DATE_S") = OraReader.GetString("HASSIN_YDATE_S") Then
                    PrnFurimei.OutputCsvData("特盒")
                ElseIf OraReader.GetString("FURI_DATE_S") = YokuDate Then
                    PrnFurimei.OutputCsvData("环特")
                Else
                    Select Case OraReader.GetString("SYUBETU_K")
                        Case "11" : PrnFurimei.OutputCsvData("氛持")
                        Case "12" : PrnFurimei.OutputCsvData("贾持")
                        Case Else : PrnFurimei.OutputCsvData("环特")
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
                        strKAMOKU = "晛捠"
                    Case "01"
                        strKAMOKU = "摉嵗"
                    Case "05"
                        strKAMOKU = "擺惻"
                    Case "37"
                        strKAMOKU = "怑堳"
                    Case Else
                        strKAMOKU = "偦懠"
                End Select

                PrnFurimei.OutputCsvData(strKAMOKU)
                PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))
                PrnFurimei.OutputCsvData(OraReader.GetString("FURIKIN_K"))
                PrnFurimei.OutputCsvData("")
                PrnFurimei.OutputCsvData(OraReader.GetString("TESUU_KIN_K"))

                If OraReader.GetString("FURI_DATE_S") = OraReader.GetString("HASSIN_YDATE_S") Then
                    Select Case OraReader.GetString("SYUBETU_K")
                        Case "11" : PrnFurimei.OutputCsvData("氛持")
                        Case "12" : PrnFurimei.OutputCsvData("贾持")
                        Case Else : PrnFurimei.OutputCsvData("")
                    End Select
                ElseIf OraReader.GetString("FURI_DATE_S") = YokuDate Then
                    Select Case OraReader.GetString("SYUBETU_K")
                        Case "11" : PrnFurimei.OutputCsvData("氛持")
                        Case "12" : PrnFurimei.OutputCsvData("贾持")
                        Case Else : PrnFurimei.OutputCsvData("")
                    End Select
                Else
                    PrnFurimei.OutputCsvData("")
                End If

                '懠峴敾掕嬫暘
                If OraReader.GetString("KEIYAKU_KIN_K") = FmtComm.JIKINKO Then
                    PrnFurimei.OutputCsvData("0", False, True)
                Else
                    PrnFurimei.OutputCsvData("1", False, True)
                End If

                OraReader.NextRead()

            Loop Until OraReader.EOF    ' EOF傑偱嶌嬈傪孞傝曉偡丅
            OraReader.Close()

            PrnFurimei.CloseCsv()

            If PrnFurimei.ReportExecute(PRINTERNAME) = True Then
                MainLOG.Write("報嶞", "惉岟")
                Return 0
            Else
                MainLOG.Write("報嶞", "幐攕", PrnFurimei.ReportMessage)
                Return -1
            End If
        Else
            MainLOG.Write("報嶞懳徾僨乕僞侽審", "惉岟")
            Return 100
        End If

    End Function

    '梻塩嬈擔傪曉偡
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
