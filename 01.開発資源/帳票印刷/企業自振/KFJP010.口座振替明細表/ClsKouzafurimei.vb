Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

Public Class ClsKouzafurimei

    '現在日付と時刻
    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    '引数
    Protected Friend TORI_CODE As String
    Protected Friend HAISINTIME As String
    Protected Friend PRINTERNAME As String

    Dim strJYUYOUKA As String = ""

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    ' FSKJ.INI セクション名
    Private ReadOnly AppTOUROKU As String = "REPORTS"

    ' 機能   ： 口座振替明細表メイン処理
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

    ' 機能   ： 口座振替明細表帳票出力処理
    '
    ' 戻り値 ： 0 - 正常 ， -1 - 異常 , 100 - ０件
    '
    ' 備考   ： 
    '
    Private Function PrintKouzafurimei() As Integer

        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim strJikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

        Dim strSort As String = CASTCommon.GetFSKJIni("PRINT", "HAISIN_FLG")
        Dim strSortMei As String = CASTCommon.GetFSKJIni("PRINT", "SORTMEI")

        Dim PrnFurimei As New ClsPrnKouzafurimei(strSort)

        '2013/10/21 saitou 標準修正 UPD -------------------------------------------------->>>>
        'パフォーマンス向上
        Dim strCenter As String = CASTCommon.GetFSKJIni("COMMON", "CENTER")
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing
        Dim name As String = ""

        Try
            'まず、取引先とスケジュールの限定
            With SQL
                .Length = 0
                .Append("SELECT * FROM SCHMAST, TORIMAST")
                .Append(" WHERE FSYORI_KBN_S = '1'")
                .Append(" AND JIFURI_TIME_STAMP_S = " & SQ(HAISINTIME))
                .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
                .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                .Append(" AND HAISIN_FLG_S = '1'")
                .Append(" AND TYUUDAN_FLG_S = '0'")
                If TORI_CODE <> "000000000000" Then
                    .Append(" AND TORIS_CODE_T = " & SQ(TORI_CODE.Substring(0, 10)))
                    .Append(" AND TORIF_CODE_T = " & SQ(TORI_CODE.Substring(10, 2)))
                End If
                .Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S")
            End With

            If OraReader.DataReader(SQL) = True Then

                If name = "" Then
                    ' ＣＳＶを作成する
                    name = PrnFurimei.CreateCsvFile()
                End If

                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While OraReader.EOF = False
                    '明細マスタの参照
                    With SQL
                        .Length = 0
                        .Append("SELECT * FROM MEIMAST, TENMAST")
                        .Append(" WHERE FSYORI_KBN_K = '1'")
                        .Append(" AND KEIYAKU_KIN_K = " & SQ(strJikinko))
                        .Append(" AND FURI_DATE_K = " & SQ(OraReader.GetString("FURI_DATE_S")))
                        .Append(" AND TORIS_CODE_K = " & SQ(OraReader.GetString("TORIS_CODE_S")))
                        .Append(" AND TORIF_CODE_K = " & SQ(OraReader.GetString("TORIF_CODE_S")))
                        If OraReader.GetString("FMT_KBN_T") = "02" Then
                            .Append(" AND DATA_KBN_K = '3'")
                        Else
                            .Append(" AND DATA_KBN_K = '2'")
                        End If
                        .Append(" AND FURIKETU_CODE_K = 0")
                        If strCenter <> "5" Then
                            .Append(" AND KEIYAKU_KOUZA_K <> '0000000'")
                        End If
                        .Append(" AND KEIYAKU_KIN_K = KIN_NO_N")
                        .Append(" AND KEIYAKU_SIT_K = SIT_NO_N")
                        Select Case String.Concat(strSort, strSortMei)
                            Case "11"    '取引先ソート　        明細部：企業ＳＥＱ
                                .Append(" ORDER BY KIGYO_SEQ_K")
                            Case "21"    '支店、取引先ソート　  明細部：企業ＳＥＱ   
                                .Append(" ORDER BY KEIYAKU_SIT_K, KIGYO_SEQ_K")
                            Case "12"    '取引先ソート　        明細部：店科目口番
                                .Append(" ORDER BY KEIYAKU_SIT_K, KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
                            Case "22"    '支店、取引先ソート　  明細部：店科目口番
                                .Append(" ORDER BY KEIYAKU_SIT_K, KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
                            Case Else   '取得できかったら"11"扱い
                                .Append(" ORDER BY KIGYO_SEQ_K")
                        End Select
                    End With

                    If oraMeiReader.DataReader(SQL) = True Then
                        While oraMeiReader.EOF = False
                            Dim strNS As String = ""
                            Dim strKAMOKU As String = ""
                            Dim strTEKIYOU As String = ""

                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("FURI_DATE_K"), True)
                            PrnFurimei.OutputCsvData(mMatchingDate, True)
                            PrnFurimei.OutputCsvData(mMatchingTime, True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("TORIS_CODE_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("TORIF_CODE_K"), True)
                            PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"), True)
                            PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_KIN_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KIN_NNAME_N"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_SIT_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("SIT_NNAME_N"), True)
                            Select Case oraMeiReader.GetString("KEIYAKU_KAMOKU_K")
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
                            PrnFurimei.OutputCsvData(strKAMOKU, True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_KOUZA_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_KNAME_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("FURIKIN_K"))

                            Select Case OraReader.GetString("NS_KBN_T")
                                Case "1"
                                    strNS = "入"
                                Case "9"
                                    strNS = "出"
                                Case Else
                                    strNS = "入"
                            End Select
                            PrnFurimei.OutputCsvData(strNS, True)

                            PrnFurimei.OutputCsvData(OraReader.GetString("FURI_CODE_T"), True)
                            PrnFurimei.OutputCsvData(OraReader.GetString("KIGYO_CODE_T"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KIGYO_SEQ_K"), True)
                            Select Case OraReader.GetString("TEKIYOU_KBN_T")
                                Case "0", "2", "3" 'カナorデータ
                                    strTEKIYOU = oraMeiReader.GetString("KTEKIYO_K")
                                Case "1" '漢字
                                    strTEKIYOU = oraMeiReader.GetString("NTEKIYO_K")
                            End Select
                            PrnFurimei.OutputCsvData(strTEKIYOU, True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("JYUYOUKA_NO_K"), True, True)

                            oraMeiReader.NextRead()
                        End While
                    Else
                        MainLOG.Write("印刷対象データ0件", "成功")
                        'Return 100
                    End If

                    oraMeiReader.Close()

                    OraReader.NextRead()
                End While
            Else
                MainLOG.Write("印刷対象データ0件", "成功")
                Return 100
            End If

            PrnFurimei.CloseCsv()

            If PrnFurimei.ReportExecute(PRINTERNAME) = True Then
                MainLOG.Write("印刷", "成功")
                Return 0
            Else
                MainLOG.Write("印刷", "失敗", PrnFurimei.ReportMessage)
                Return -1
            End If

        Catch ex As Exception
            MainLOG.Write("印刷", "失敗", ex.Message)
            Return -1
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not oraMeiReader Is Nothing Then
                oraMeiReader.Close()
                oraMeiReader = Nothing
            End If
        End Try

        'SQL = New StringBuilder(128)
        'SQL.Append("SELECT * FROM MEIMAST,TORIMAST,SCHMAST,TENMAST")
        'SQL.Append(" WHERE FSYORI_KBN_K = '1'")
        'SQL.Append(" AND JIFURI_TIME_STAMP_S = '" & HAISINTIME & "'")
        'SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S AND TORIF_CODE_K=TORIF_CODE_S AND FURI_DATE_K = FURI_DATE_S ")
        'SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_T AND TORIF_CODE_K=TORIF_CODE_T ")
        'SQL.Append(" AND KIN_NO_N = KEIYAKU_KIN_K AND KEIYAKU_SIT_K = SIT_NO_N ")
        'SQL.Append(" AND HAISIN_FLG_S = '1'")
        'SQL.Append(" AND TYUUDAN_FLG_S = '0'")

        'SQL.Append(" AND KEIYAKU_KIN_K = '" & strJikinko & "'")

        'If TORI_CODE <> "000000000000" Then
        '    SQL.Append(" AND TORIS_CODE_T = '" & TORI_CODE.Substring(0, 10) & "'")
        '    SQL.Append(" AND TORIF_CODE_T = '" & TORI_CODE.Substring(10, 2) & "'")
        'End If
        ''2010.02.01 start
        ''SQL.Append(" AND DATA_KBN_K = '2'")
        'SQL.Append(" AND ((DATA_KBN_K = '2' AND FMT_KBN_T<>'02')")
        'SQL.Append(" OR (DATA_KBN_K = '3' AND FMT_KBN_T='02'))")
        ''2010.02.01 end
        'SQL.Append(" AND FURIKETU_CODE_K = 0")
        'If CASTCommon.GetFSKJIni("COMMON", "CENTER") <> "5" Then
        '    SQL.Append(" AND KEIYAKU_KOUZA_K <> '0000000'")
        'End If
        'Select Case String.Concat(strSort, strSortMei)
        '    Case "11"    '取引先ソート　        明細部：企業ＳＥＱ
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KIGYO_SEQ_K ASC")
        '    Case "21"    '支店、取引先ソート　  明細部：企業ＳＥＱ   
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KEIYAKU_SIT_K ASC,KIGYO_SEQ_K ASC")
        '    Case "12"    '取引先ソート　        明細部：店科目口番
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KEIYAKU_SIT_K ASC,KEIYAKU_KAMOKU_K ASC,KEIYAKU_KOUZA_K ASC")
        '    Case "22"    '支店、取引先ソート　  明細部：店科目口番
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KEIYAKU_SIT_K ASC,KEIYAKU_KAMOKU_K ASC,KEIYAKU_KOUZA_K ASC")
        '    Case Else   '取得できかったら"11"扱い
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,KEIYAKU_SIT_K ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KIGYO_SEQ_K ASC")
        'End Select

        'Dim name As String = ""


        'Dim bSQL As Boolean
        'bSQL = OraReader.DataReader(SQL)

        'If bSQL = True Then

        '    name = PrnFurimei.CreateCsvFile

        '    If name = "" Then
        '        ' ＣＳＶを作成する
        '        name = PrnFurimei.CreateCsvFile()
        '    End If

        '    Do
        '        Dim strNS As String = ""
        '        Dim strKAMOKU As String = ""
        '        Dim strTEKIYOU As String = ""

        '        PrnFurimei.OutputCsvData(OraReader.GetString("FURI_DATE_K"))
        '        PrnFurimei.OutputCsvData(mMatchingDate)
        '        PrnFurimei.OutputCsvData(mMatchingTime)
        '        PrnFurimei.OutputCsvData(OraReader.GetString("TORIS_CODE_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("TORIF_CODE_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("SIT_NNAME_N"))
        '        Select Case OraReader.GetString("KEIYAKU_KAMOKU_K")
        '            Case "02"
        '                strKAMOKU = "普通"
        '            Case "01"
        '                strKAMOKU = "当座"
        '            Case "05"
        '                strKAMOKU = "納税"
        '            Case "37"
        '                strKAMOKU = "職員"
        '            Case Else
        '                strKAMOKU = "そ他"
        '        End Select
        '        PrnFurimei.OutputCsvData(strKAMOKU)
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("FURIKIN_K"))

        '        Select Case OraReader.GetString("NS_KBN_T")
        '            Case "1"
        '                strNS = "入"
        '            Case "9"
        '                strNS = "出"
        '            Case Else
        '                strNS = "入"
        '        End Select
        '        PrnFurimei.OutputCsvData(strNS)

        '        PrnFurimei.OutputCsvData(OraReader.GetString("FURI_CODE_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KIGYO_CODE_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KIGYO_SEQ_K"))
        '        Select Case OraReader.GetString("TEKIYOU_KBN_T")
        '            Case "0", "2", "3" 'カナorデータ
        '                strTEKIYOU = OraReader.GetString("KTEKIYO_K")
        '            Case "1" '漢字
        '                strTEKIYOU = OraReader.GetString("NTEKIYO_K")
        '        End Select
        '        PrnFurimei.OutputCsvData(strTEKIYOU)
        '        PrnFurimei.OutputCsvData(OraReader.GetString("JYUYOUKA_NO_K"), False, True)

        '        OraReader.NextRead()

        '    Loop Until OraReader.EOF    ' EOFまで作業を繰り返す。
        '    OraReader.Close()

        '    PrnFurimei.CloseCsv()

        '    If PrnFurimei.ReportExecute(PRINTERNAME) = True Then
        '        MainLOG.Write("印刷", "成功")
        '        Return 0
        '    Else
        '        MainLOG.Write("印刷", "失敗", PrnFurimei.ReportMessage)
        '        Return -1
        '    End If
        'Else
        '    MainLOG.Write("印刷対象データ０件", "成功")
        '    Return 100
        'End If
        '2013/10/21 saitou 標準修正 UPD --------------------------------------------------<<<<

    End Function

End Class
