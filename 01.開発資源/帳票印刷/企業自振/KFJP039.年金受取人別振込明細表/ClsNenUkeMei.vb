Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

' 
Public Class ClsNenUkeMei

    'タイムスタンプ取得
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '現在日付
    
    'パラメータ
    Protected Friend ToriSCode As String                '取引先主コード
    Protected Friend ToriFCode As String                '取引先副コード
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
            nRet = PrintNenUkeMei()

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
    Private Function PrintNenUkeMei() As Integer
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim PrnNenSitCheck As New ClsPrnNenUkeMei()
        Dim strPrinterName As String = ""
        Dim CSVName As String = ""
        Dim bSQL As Boolean
        Dim strKamoku As String = ""
        
        Try
            strPrinterName = CASTCommon.GetFSKJIni("COMMON", "PRINTER_1")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT ITAKU_NNAME_T,KEIYAKU_SIT_K")
            SQL.Append(", YOBI3_K, KEIYAKU_KNAME_K")
            SQL.Append(", KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
            SQL.Append(", FURIKIN_K, YOBI1_K, YOBI2_K,TORIS_CODE_K,TORIF_CODE_K")
            SQL.Append(" FROM NENKINMAST, SCHMAST, TORIMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & FuriDate & "'")
            If Not (ToriSCode = "9999999999" And ToriFCode = "99") Then
                SQL.Append(" AND TORIS_CODE_K = '" & ToriSCode & "'")
                SQL.Append(" AND TORIF_CODE_K = '" & ToriFCode & "'")
            End If
            SQL.Append(" AND DATA_KBN_K = '2'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_T")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_S")
            SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, KEIYAKU_SIT_K")

            bSQL = OraReader.DataReader(SQL)

            If bSQL = True Then

                CSVName = PrnNenSitCheck.CreateCsvFile()

                Do
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
                            strKamoku = "その他"
                    End Select

                    '--------------------------------------
                    '項目書出
                    '--------------------------------------
                    With PrnNenSitCheck
                        .OutputCsvData(mMatchingDate)                                   '処理日
                        .OutputCsvData(OraReader.GetString("TORIS_CODE_K"))             '取引先主コード
                        .OutputCsvData(OraReader.GetString("TORIF_CODE_K"))             '取引先副コード
                        .OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))            '委託者名
                        .OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))            '契約支店コード
                        .OutputCsvData(FuriDate)                                        '振替日
                        .OutputCsvData(OraReader.GetString("YOBI3_K"))                  '年金証書番号
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)    '受取人名
                        .OutputCsvData(strKamoku, True)                                 '契約科目
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))          '契約口座番号
                        .OutputCsvData(OraReader.GetString("FURIKIN_K"))                '振込金額
                        .OutputCsvData(OraReader.GetString("YOBI2_K"))                  '整理番号
                        .OutputCsvData(OraReader.GetString("YOBI1_K"), True)            '登録店舗名
                        .OutputCsvData("ジフリ", True, True)                            '処理方法（ジフリ固定）
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
