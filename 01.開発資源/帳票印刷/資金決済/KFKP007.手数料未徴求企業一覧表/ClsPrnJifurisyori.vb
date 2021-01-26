Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 口座振替資金決済一覧表
Class ClsPrnJifurisyori
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFKP007"

        ' 定義体名セット
        ReportBaseName = "KFKP007_手数料未徴求企業一覧表.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： 口座振替資金決済一覧表をデータに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    ' 機能   ： 資金決済帳票出力処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考   ： 
    '
    Public Function MakeRecord() As Boolean
        Dim SQL As New StringBuilder(128)
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim TYO_KBN As String = ""
        Dim MISHUU_KBN As String = ""
        '2017/05/19 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
        Dim PATH_TXT As String = CASTCommon.GetFSKJIni("COMMON", "TXT")
        '2017/05/19 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

        'INIファイル取得
        '自金庫コード
        Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        If Jikinko.ToUpper = "ERR" Or Jikinko = "" Then
            BatchLog.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
            Return False
        End If

        SQL = New StringBuilder(128)
        SQL.Append("SELECT")
        SQL.Append(" TORIMAST.FURI_CODE_T")         '振替コード
        SQL.Append(",TORIMAST.KIGYO_CODE_T")        '企業コード
        SQL.Append(",TORIMAST.ITAKU_NNAME_T")       '委託者漢字名
        SQL.Append(",TORIMAST.HONBU_KOUZA_T")       '本部口座番号
        SQL.Append(",TORIMAST.TORIMATOME_SIT_T")    '取りまとめ店
        SQL.Append(",TORIMAST.TUKEKIN_NO_T")        '決済金融機関
        SQL.Append(",TORIMAST.TUKESIT_NO_T")        '決済支店
        SQL.Append(",TORIMAST.TUKEKAMOKU_T")        '決済科目
        SQL.Append(",TORIMAST.TUKEKOUZA_T")         '決済口座
        SQL.Append(",TORIMAST.TESUUTYO_KBN_T")      '手数料徴求区分
        SQL.Append(",SCHMAST.TORIS_CODE_S")         '取引先主コード
        SQL.Append(",SCHMAST.TORIF_CODE_S")         '取引先副コード
        SQL.Append(",SCHMAST.FURI_DATE_S")          '振替日
        SQL.Append(",SCHMAST.KESSAI_DATE_S")        '決済日
        SQL.Append(",SCHMAST.TESUU_KIN_S")          '手数料金額
        SQL.Append(",SCHMAST.TESUU_KIN1_S")         '手数料(自振)
        SQL.Append(",SCHMAST.TESUU_KIN2_S")         '手数料(その他)
        SQL.Append(",SCHMAST.TESUU_KIN3_S")         '手数料(振込)
        SQL.Append(",SCHMAST.SYORI_KEN_S")          '処理件数
        SQL.Append(",SCHMAST.SYORI_KIN_S")          '処理金額
        SQL.Append(",SCHMAST.FURI_KEN_S")           '振替件数
        SQL.Append(",SCHMAST.FURI_KIN_S")           '振替金額
        SQL.Append(",SCHMAST.FUNOU_KEN_S")          '不能件数
        SQL.Append(",SCHMAST.FUNOU_KIN_S")          '不能金額
        SQL.Append(",SCHMAST.KESSAI_YDATE_S")       '決済予定日
        SQL.Append(",TENMAST.SIT_NNAME_N")          '支店名
        SQL.Append(" FROM SCHMAST")
        SQL.Append(",TORIMAST")
        SQL.Append(",TENMAST")
        SQL.Append(" WHERE SCHMAST.FSYORI_KBN_S = TORIMAST.FSYORI_KBN_T")
        SQL.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
        SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
        SQL.Append(" AND SCHMAST.TESUU_YDATE_S <= " & SQ(txtTESUU_YDATE))
        SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND SCHMAST.TESUUTYO_FLG_S IN('0','2')")   '未徴求・徴求待ち
        SQL.Append(" AND SCHMAST.TESUUKEI_FLG_S = '1'")
        SQL.Append(" AND SCHMAST.TESUU_KIN_S > 0")
        SQL.Append(" AND TENMAST.KIN_NO_N = " & SQ(Jikinko))
        SQL.Append(" AND TENMAST.SIT_NO_N = TORIMAST.TORIMATOME_SIT_T")
        ''決済対象外以外(今は条件からはずす)
        'SQL.Append(" AND KESSAI_KBN_T <> '99'")
        SQL.Append(" ORDER BY TORIMATOME_SIT_T,FURI_DATE_S,TORIS_CODE_T,TORIF_CODE_T")

        Dim bSQL As Boolean
        Dim substrKESSAI_DATE As String = ""
        Dim substrTESUU_DATE As String = ""
        Try
            bSQL = OraReader.DataReader(SQL)
            If bSQL = True Then
                Do
                    OutputCsvData(mMatchingDate, True)                                  '処理日
                    OutputCsvData(mMatchingTime, True)                                  'タイムスタンプ
                    OutputCsvData(txtTESUU_YDATE, True)                                '基準日(手数料徴求予定日)
                    OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"), True)        '取りまとめ店コード
                    OutputCsvData(OraReader.GetString("SIT_NNAME_N"), True)             '取りまとめ店名
                    OutputCsvData(OraReader.GetString("TORIS_CODE_S"), True)            '取引先主コード
                    OutputCsvData(OraReader.GetString("TORIF_CODE_S"), True)            '取引先副コード
                    OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"), True)           '取引先名
                    OutputCsvData(OraReader.GetString("FURI_CODE_T"), True)             '振替コード
                    OutputCsvData(OraReader.GetString("KIGYO_CODE_T"), True)            '企業コード
                    OutputCsvData(OraReader.GetString("FURI_DATE_S"), True)             '振替日

                    substrKESSAI_DATE = OraReader.GetString("KESSAI_YDATE_S")
                    If substrKESSAI_DATE = "00000000" Then
                        substrKESSAI_DATE = ""
                    End If
                    OutputCsvData(substrKESSAI_DATE, True)                              '決済(予定)日

                    OutputCsvData(OraReader.GetString("HONBU_KOUZA_T"), True)           '本部別段口座番号
                    OutputCsvData(OraReader.GetInt64("SYORI_KEN_S"), True)              '請求件数
                    OutputCsvData(OraReader.GetInt64("SYORI_KIN_S"), True)              '請求金額
                    OutputCsvData(OraReader.GetInt64("FUNOU_KEN_S"), True)              '不能件数
                    OutputCsvData(OraReader.GetInt64("FUNOU_KIN_S"), True)              '不能金額
                    OutputCsvData(OraReader.GetInt64("FURI_KEN_S"), True)               '振替件数
                    OutputCsvData(OraReader.GetInt64("FURI_KIN_S"), True)               '振替金額
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN_S"), True)              '手数料
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN1_S"), True)             '手数料内訳-自振
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN3_S"), True)             '手数料内訳-振込
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN2_S"), True)             '手数料内訳-その他
                    Select Case OraReader.GetString("TESUUTYO_KBN_T")   '手数料徴求区分
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
                    OutputCsvData(OraReader.GetString("TUKEKIN_NO_T"), True)            '決済金融機関
                    OutputCsvData(OraReader.GetString("TUKESIT_NO_T"), True)            '決済支店
                    '2017/05/19 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                    '決済科目名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_決済科目.TXT"), _
                                                                OraReader.GetString("TUKEKAMOKU_T")), True)
                    'Select Case OraReader.GetString("TUKEKAMOKU_T")                     '決済科目
                    '    Case "01"
                    '        OutputCsvData("当座", True)
                    '    Case "02"
                    '        OutputCsvData("普通", True)
                    '    Case "04"
                    '        OutputCsvData("別段", True)
                    '    Case "99"
                    '        OutputCsvData("諸勘定", True)
                    '    Case Else
                    '        OutputCsvData("", True)
                    'End Select
                    '2017/05/19 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- END
                    OutputCsvData(OraReader.GetString("TUKEKOUZA_T"), True, True)       '決済口座番号
                    RecordCnt += 1
                    OraReader.NextRead()
                Loop Until OraReader.EOF
                Return True
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

End Class
