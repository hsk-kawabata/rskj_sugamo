Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

' 
Public Class ClsNenSitCheck

    'タイムスタンプ取得
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '現在日付
    Private mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    '現在時刻

    'パラメータ
    Protected Friend FuriDate As String                 '振替日

    '共通処理項目
    Private MainDB As CASTCommon.MyOracle               'パブリックＤＢ
    Private ReadOnly AppTOUROKU As String = "REPORTS"   'FSKJ.INI セクション名
    
    '=======================================================================
    ' 機能   ： 口座振替明細表メイン処理
    ' 引数   ： なし
    ' 戻り値 ： 0 - 正常 0以外 - 異常
    ' 備考   ： 
    ' 作成日 ： 2009/09/29
    ' 更新日 ： 
    '=======================================================================
    Function Main() As Integer
        Dim nRet As Integer

        Try
            MainDB = New CASTCommon.MyOracle

            '--------------------------------------
            '印刷処理実行
            '--------------------------------------
            nRet = PrintKouzafurimei()

            If nRet < 0 Then
                Return 2
            Else
                Return nRet
            End If

        Catch ex As Exception
           BatchLOG.Write("(主処理)", "失敗", ex.Message & ":" & ex.StackTrace)
            Return -1

        End Try
    End Function

    '=======================================================================
    ' 機能   ： 口座振替明細表メイン処理
    ' 引数   ： なし
    ' 戻り値 ： 0 - 正常 ， -1 - 異常 , 100 - ０件
    ' 備考   ： 
    ' 作成日 ： 2009/09/29
    ' 更新日 ： 
    '=======================================================================
    Private Function PrintKouzafurimei() As Integer
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim PrnNenSitCheck As New ClsPrnNenSitCheck()
        Dim strNenkinSit As String = "000"
        Dim strPrinterName As String = ""
        Dim CSVName As String = ""
        Dim bSQL As Boolean
        Dim strKamoku As String = ""
        Dim strNenkinSyubetuCode As String = "00"
        Dim strNenkinSyubetuName As String = ""
        Dim strNenkinSyousyo As String = "000000000000"

        Try
            strNenkinSit = CASTCommon.GetFSKJIni("COMMON", "NENKIN_SIT")
            strPrinterName = CASTCommon.GetFSKJIni("COMMON", "PRINTER_1")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT TORIS_CODE_K, TORIF_CODE_K")
            SQL.Append(", FURI_DATE_K, JYUYOUKA_NO_K, YOBI2_K")
            SQL.Append(", KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
            SQL.Append(", KEIYAKU_KNAME_K, FURIKIN_K, KEIYAKU_KIN_K, YOBI1_K")
            SQL.Append(" FROM NENKINMAST, SCHMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & FuriDate & "'")
            SQL.Append(" AND DATA_KBN_K = '2'")
            SQL.Append(" AND KEIYAKU_SIT_K = '" & strNenkinSit & "'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_S")
            SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, YOBI2_K")

            bSQL = OraReader.DataReader(SQL)

            If bSQL = True Then

                CSVName = PrnNenSitCheck.CreateCsvFile()

                Do
                    '--------------------------------------
                    '年金種別設定
                    '--------------------------------------
                    If OraReader.GetString("JYUYOUKA_NO_K").Length >= 2 Then
                        strNenkinSyubetuCode = OraReader.GetString("JYUYOUKA_NO_K").Substring(0, 2)
                    End If

                    Select Case strNenkinSyubetuCode
                        Case "61"
                            strNenkinSyubetuName = "旧厚生"
                        Case "62"
                            strNenkinSyubetuName = "旧船員保険"
                        Case "63"
                            strNenkinSyubetuName = "旧国民"
                        Case "64"
                            strNenkinSyubetuName = "労災"
                        Case "65"
                            strNenkinSyubetuName = "新国民保険"
                        Case "66"
                            strNenkinSyubetuName = "新船員保険"
                        Case "67"
                            strNenkinSyubetuName = "旧国民短期"
                    End Select

                    '--------------------------------------
                    '年金証書番号設定
                    '--------------------------------------
                    If OraReader.GetString("JYUYOUKA_NO_K").Length >= 17 Then
                        strNenkinSyousyo = OraReader.GetString("JYUYOUKA_NO_K").Substring(2, 15)
                    End If

                    '--------------------------------------
                    '科目設定
                    '--------------------------------------
                    Select Case OraReader.GetString("KEIYAKU_KAMOKU_K")
                        Case "02"
                            strKamoku = "普通"
                        Case "01"
                            strKamoku = "当座"
                        Case "05"
                            strKamoku = "納税"
                        Case "37"
                            strKamoku = "職員"
                        Case Else
                            strKamoku = "そ他"
                    End Select

                    '--------------------------------------
                    '項目書出
                    '--------------------------------------
                    With PrnNenSitCheck
                        .OutputCsvData(OraReader.GetString("FURI_DATE_K"))              '振替日
                        .OutputCsvData(mMatchingDate)                                   '処理日
                        .OutputCsvData(mMatchingTime)                                   'タイムスタンプ
                        .OutputCsvData(OraReader.GetString("TORIS_CODE_K"))             '取引先主コード
                        .OutputCsvData(OraReader.GetString("TORIF_CODE_K"))             '取引先副コード
                        .OutputCsvData(strNenkinSyubetuCode)                            '年金種別コード
                        .OutputCsvData(strNenkinSyubetuName, True)                      '年金種別名
                        .OutputCsvData(OraReader.GetString("YOBI2_K"))                  '整理番号
                        .OutputCsvData(strNenkinSyousyo)                                '年金証書番号
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"), True)      '契約金融機関コード
                        .OutputCsvData(OraReader.GetString("YOBI1_K"), True)            '契約支店名
                        .OutputCsvData(strKamoku, True)                                 '契約科目
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))          '契約口座番号
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)    '契約者カナ名
                        .OutputCsvData(OraReader.GetString("FURIKIN_K"), False, True)   '振替金額
                    End With

                    OraReader.NextRead()
                Loop Until OraReader.EOF    ' EOFまで作業を繰り返す。

                OraReader.Close()
                PrnNenSitCheck.CloseCsv()

                If PrnNenSitCheck.ReportExecute(strPrinterName) = True Then
                    Return 0
                Else
                    BatchLOG.Write("印刷", "失敗", PrnNenSitCheck.ReportMessage)
                    Return -1
                End If
            Else
                BatchLOG.Write("印刷対象データ０件", "成功")
                Return 100
            End If

        Catch ex As Exception
            BatchLOG.Write("印刷", "失敗", ex.Message)
            Return -1
        End Try
    End Function

End Class
