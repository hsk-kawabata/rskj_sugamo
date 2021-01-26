Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.IO
Imports System.Collections

Class KFKP002
    Inherits CAstReports.ClsReportBase

    Public CsvData(38) As String            '2010/02/02 36→38

    Private BasePath As String              ' INIファイルの資金決済ファイルパス
    Private KessaiFileName As String        ' 資金決済ファイル名

    Private strKESSAI_DATE As String                    ' 決済日
    Private Mainlog As CASTCommon.BatchLOG
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFKP002"

        ' 定義体名セット
        ReportBaseName = "KFKP002_処理結果確認表(資金決済取消).rpd"

        CsvData(0) = "00010101"                                     ' 処理日
        CsvData(1) = "00010101"                                     ' 決済日
        CsvData(2) = "00010101"                                     ' タイムスタンプ
        CsvData(3) = ""                                             ' 取引先主コード
        CsvData(4) = ""                                             ' 取引先副コード
        CsvData(5) = ""                                             ' 取引先名
        CsvData(6) = ""                                             ' 振替コード
        CsvData(7) = ""                                             ' 企業コード
        CsvData(8) = "00010101"                                     ' 振替日
        CsvData(9) = ""                                             ' 決済区分（漢字）
        CsvData(10) = ""                                            ' 決済金融機関コード
        CsvData(11) = ""                                            ' 決済金融機関支店コード
        CsvData(12) = ""                                            ' 決済科目
        CsvData(13) = ""                                            ' 決済口座番号
        CsvData(14) = ""                                            ' 手数料徴求区分
        CsvData(15) = ""                                            ' 手数料徴求方法
        CsvData(16) = ""                                            ' 取りまとめ店コード
        CsvData(17) = ""                                            ' 取りまとめ店名
        CsvData(18) = "0"                                            ' 請求件数
        CsvData(19) = "0"                                            ' 請求金額
        CsvData(20) = "0"                                            ' 不能件数
        CsvData(21) = "0"                                            ' 不能金額
        CsvData(22) = "0"                                            ' 振替件数
        CsvData(23) = "0"                                           ' 振替金額
        CsvData(24) = "0"                                           ' 手数料
        CsvData(25) = "0"                                           ' 手数料内訳－自振
        CsvData(26) = "0"                                           ' 手数料内訳－振込
        CsvData(27) = "0"                                           ' 手数料内訳－その他
        CsvData(28) = "0"                                           ' 入金分件数
        CsvData(29) = "0"                                           ' 入金分金額
        CsvData(30) = ""                                            ' 科目コード
        CsvData(31) = ""                                            ' オペコード
        CsvData(32) = ""                                            ' オペレーション名
        CsvData(33) = "0"                                           ' 入出金額（オペコード単位）
        CsvData(34) = "0"                                           ' 手数料額（オペコード単位）
        CsvData(35) = "0"                                           ' 集計フラグ
        CsvData(36) = "0"                                           ' リエンタ作成区分
        '2010/02/02 追加
        CsvData(37) = ""                                            ' 決済金融機関名
        CsvData(38) = ""                                            ' 決済支店名
        '===============
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' タイトル行
        CSVObject.Output("処理日")
        CSVObject.Output("決済日")
        CSVObject.Output("タイムスタンプ")
        CSVObject.Output("取引先主コード")
        CSVObject.Output("取引先副コード")
        CSVObject.Output("取引先名")
        CSVObject.Output("振替コード")
        CSVObject.Output("企業コード")
        CSVObject.Output("振替日")
        CSVObject.Output("決済区分")
        CSVObject.Output("決済金融機関コード")
        CSVObject.Output("決済支店コード")
        CSVObject.Output("決済科目")
        CSVObject.Output("決済口座番号")
        CSVObject.Output("手数料徴求区分")
        CSVObject.Output("手数料徴求方法")
        CSVObject.Output("とりまとめ店コード")
        CSVObject.Output("とりまとめ店名")
        CSVObject.Output("請求件数")
        CSVObject.Output("請求金額")
        CSVObject.Output("不能件数")
        CSVObject.Output("不能金額")
        CSVObject.Output("振替件数")
        CSVObject.Output("振替金額")
        CSVObject.Output("手数料")
        CSVObject.Output("自振手数料")
        CSVObject.Output("振込手数料")
        CSVObject.Output("その他手数料")
        CSVObject.Output("入金件数")
        CSVObject.Output("入金金額")
        CSVObject.Output("科目コード")
        CSVObject.Output("オペコード")
        CSVObject.Output("オペレーション名")
        CSVObject.Output("入出金額")
        CSVObject.Output("手数料額")
        CSVObject.Output("集計フラグ")
        '2010/02/02 項目追加
        'CSVObject.Output("作成区分", False, True)
        CSVObject.Output("作成区分")
        CSVObject.Output("決済金融機関名")
        CSVObject.Output("決済支店名", False, True)
        '====================

        Return file
    End Function

    '
    ' 機能　 ： 資金決済処理結果確認表(資金決済取消)ＣＳＶを出力する
    '
    ' 備考　 ： 
    '
    Public Function OutputCSVKekka(ByVal TimeStamp As String, ByVal Jikinko As String, ByVal db As CASTCommon.MyOracle, ByVal WriteLog As CASTCommon.BatchLOG) As Integer

        Mainlog = WriteLog
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing
        Dim newkey As String = ""
        Dim oldkey As String = ""
        Dim SQL As New StringBuilder(128)
        Dim strDate As String = Now.ToString("yyyyMMdd")
        Dim strTime As String = Now.ToString("HHmmss")
        Dim OraKesReader As CASTCommon.MyOracleReader = Nothing
        Dim CommonSQL As New StringBuilder(128)

        Try
            OraKesReader = New CASTCommon.MyOracleReader(db)
            SQL.Append("(")
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(",KAMOKU_CODE_KR")
            SQL.Append(",OPE_CODE_KR")
            SQL.Append(",DENBUN_ALL_KR")
            SQL.Append(",RECORD_NO_KR")
            SQL.Append(",KESSAI_YDATE_S")
            SQL.Append(",TESUU_YDATE_S")
            SQL.Append(",TESUU_KIN_S")
            SQL.Append(",TESUU_KIN1_S")
            SQL.Append(",TESUU_KIN2_S")
            SQL.Append(",TESUU_KIN3_S")
            SQL.Append(",FURI_KEN_S")
            SQL.Append(",FURI_KIN_S")
            SQL.Append(",SYORI_KEN_S")
            SQL.Append(",SYORI_KIN_S")
            SQL.Append(",FUNOU_KEN_S")
            SQL.Append(",FUNOU_KIN_S")
            SQL.Append(",KIGYO_CODE_T")
            SQL.Append(",BAITAI_CODE_T")
            SQL.Append(",SYUBETU_T")
            SQL.Append(",ITAKU_CODE_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",ITAKU_KNAME_T")
            SQL.Append(",FURI_CODE_T")
            SQL.Append(",NS_KBN_T")
            SQL.Append(",TESUUTYO_PATN_T")
            SQL.Append(",TESUUTYO_KBN_T")
            SQL.Append(",KESSAI_KBN_T")
            SQL.Append(",TORIMATOME_SIT_T")
            SQL.Append(",HONBU_KOUZA_T")
            SQL.Append(",TUKEKIN_NO_T")
            SQL.Append(",TUKESIT_NO_T")
            SQL.Append(",TUKEKAMOKU_T")
            SQL.Append(",TUKEKOUZA_T")
            SQL.Append(",TUKEMEIGI_KNAME_T")
            SQL.Append(",BIKOU1_T")
            SQL.Append(",BIKOU2_T")
            SQL.Append(",TESUUTYO_SIT_T")
            SQL.Append(",TESUUTYO_KAMOKU_T")
            SQL.Append(",TESUUTYO_KOUZA_T")
            SQL.Append(",TESUUTYO_FLG_S")
            SQL.Append(",TESUU_TIME_STAMP_S")
            SQL.Append(",KESSAI_TIME_STAMP_S")
            SQL.Append(",0 AS JYUNBAN")
            SQL.Append(" FROM KESSAIMAST,TORIMAST,SCHMAST")
            SQL.Append(" WHERE TIME_STAMP_KR = " & SQ(TimeStamp))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_KR")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_KR")
            SQL.Append(" AND FURI_DATE_S  = FURI_DATE_KR")
            SQL.Append(" AND (TESUU_TIME_STAMP_S  =" & SQ(TimeStamp))
            SQL.Append(" OR  KESSAI_TIME_STAMP_S  =" & SQ(TimeStamp) & ")")
            SQL.Append(")")

            SQL.Append("UNION")
            SQL.Append("(")
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",MAX(FURI_DATE_S) AS FURI_DATE_S")
            SQL.Append(",KAMOKU_CODE_KR")
            SQL.Append(",OPE_CODE_KR")
            SQL.Append(",MAX(DENBUN_ALL_KR) AS DENBUN_ALL_KR")
            SQL.Append(",MAX(RECORD_NO_KR) AS RECORD_NO_KR")
            SQL.Append(",'00000000' AS KESSAI_YDATE_S")
            SQL.Append(",MAX(TESUU_YDATE_S) AS TESUU_YDATE_S")
            SQL.Append(",SUM(TESUU_KIN_S) AS TESUU_KIN_S")
            SQL.Append(",SUM(TESUU_KIN1_S) AS TESUU_KIN1_S")
            SQL.Append(",SUM(TESUU_KIN2_S) AS TESUU_KIN2_S")
            SQL.Append(",SUM(TESUU_KIN3_S) AS TESUU_KIN3_S")
            SQL.Append(",SUM(FURI_KEN_S) AS FURI_KEN_S")
            SQL.Append(",SUM(FURI_KIN_S) AS FURI_KIN_SV1")
            SQL.Append(",SUM(SYORI_KEN_S) AS SYORI_KEN_S")
            SQL.Append(",SUM(SYORI_KIN_S) AS SYORI_KIN_S")
            SQL.Append(",SUM(FUNOU_KEN_S) AS FUNOU_KEN_S")
            SQL.Append(",SUM(FUNOU_KIN_S) AS FUNOU_KIN_S")
            SQL.Append(",MAX(KIGYO_CODE_T) AS KIGYO_CODE_T")
            SQL.Append(",MAX(BAITAI_CODE_T) AS BAITAI_CODE_T")
            SQL.Append(",MAX(SYUBETU_T) AS SYUBETU_T")
            SQL.Append(",MAX(ITAKU_CODE_T) AS ITAKU_CODE_T")
            SQL.Append(",MAX(ITAKU_NNAME_T) AS ITAKU_NNAME_T")
            SQL.Append(",MAX(ITAKU_KNAME_T) AS ITAKU_KNAME_T")
            SQL.Append(",MAX(FURI_CODE_T) AS FURI_CODE_T")
            SQL.Append(",MAX(NS_KBN_T) AS NS_KBN_T")
            SQL.Append(",MAX(TESUUTYO_PATN_T) AS TESUUTYO_PATN_T")
            SQL.Append(",MAX(TESUUTYO_KBN_T) AS TESUUTYO_KBN_T")
            SQL.Append(",MAX(KESSAI_KBN_T) AS KESSAI_KBN_T")
            SQL.Append(",MAX(TORIMATOME_SIT_T) AS TORIMATOME_SIT_T")
            SQL.Append(",MAX(HONBU_KOUZA_T) AS HONBU_KOUZA_T")
            SQL.Append(",MAX(TUKEKIN_NO_T) AS TUKEKIN_NO_T")
            SQL.Append(",MAX(TUKESIT_NO_T) AS TUKESIT_NO_T")
            SQL.Append(",MAX(TUKEKAMOKU_T) AS TUKEKAMOKU_T")
            SQL.Append(",MAX(TUKEKOUZA_T) AS TUKEKOUZA_T")
            SQL.Append(",MAX(TUKEMEIGI_KNAME_T) AS TUKEMEIGI_KNAME_T")
            SQL.Append(",MAX(BIKOU1_T) AS BIKOU1_T")
            SQL.Append(",MAX(BIKOU2_T) AS BIKOU2_T")
            SQL.Append(",MAX(TESUUTYO_SIT_T) AS TESUUTYO_SIT_T")
            SQL.Append(",MAX(TESUUTYO_KAMOKU_T) AS TESUUTYO_KAMOKU_T")
            SQL.Append(",MAX(TESUUTYO_KOUZA_T) AS TESUUTYO_KOUZA_T")
            SQL.Append(",MAX(TESUUTYO_FLG_S) AS TESUUTYO_FLG_S")
            SQL.Append(",MAX(TESUU_TIME_STAMP_S) AS TESUU_TIME_STAMP_S")
            SQL.Append(",MAX(KESSAI_TIME_STAMP_S) AS KESSAI_TIME_STAMP_S")
            SQL.Append(",1 AS JYUNBAN")
            SQL.Append(" FROM KESSAIMAST,TORIMAST,SCHMAST")
            SQL.Append(" WHERE TIME_STAMP_KR = " & SQ(TimeStamp))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_KR")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_KR")
            SQL.Append(" AND TRIM(FURI_DATE_KR) IS NULL ")
            SQL.Append(" AND (TESUU_TIME_STAMP_S  =" & SQ(TimeStamp))
            SQL.Append(" OR  KESSAI_TIME_STAMP_S  =" & SQ(TimeStamp) & ")")
            SQL.Append(" GROUP BY TORIS_CODE_T, TORIF_CODE_T,KAMOKU_CODE_KR,OPE_CODE_KR,FURI_DATE_KR")
            SQL.Append(")")
            SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T,JYUNBAN,RECORD_NO_KR,KAMOKU_CODE_KR,OPE_CODE_KR,FURI_DATE_S")

            If OraKesReader.DataReader(SQL) = True Then
                While OraKesReader.EOF = False
                    KData.Init()
                    fn_KessaiData(KData, OraKesReader, TimeStamp)

                    newkey = KData.TorisCode & KData.TorifCode & KData.FuriDate
                    ' ＣＳＶデータ設定
                    CsvData(0) = strDate                                    ' 処理日
                    CsvData(1) = Mid(TimeStamp, 1, 8)                       ' 基準日
                    CsvData(2) = strTime                                    ' タイムスタンプ
                    CsvData(3) = KData.TorisCode                             ' 取引先主コード
                    CsvData(4) = KData.TorifCode                             ' 取引先副コード
                    CsvData(5) = KData.ToriNName.Trim                        ' 取引先名
                    CsvData(6) = KData.FuriCode                              ' 振替コード
                    CsvData(7) = KData.KigyoCode                             ' 企業コード
                    CsvData(8) = KData.FuriDate                              ' 振替日

                    Select Case KData.KessaiKbn
                        Case "00"
                            CsvData(9) = "預け金"
                        Case "01"
                            CsvData(9) = "口座入金"                                            ' 決済区分（漢字）
                        Case "02"
                            CsvData(9) = "為替振込"
                        Case "03"
                            CsvData(9) = "為替付替"
                        Case "04"
                            CsvData(9) = "別段出金のみ"
                        Case "05"
                            CsvData(9) = "特別企業"
                        Case "99"
                            CsvData(9) = "決済対象外"
                    End Select

                    CsvData(10) = KData.KesKinCode                                            ' 決済金融機関コード
                    CsvData(11) = KData.KesSitCode                                            ' 決済金融機関支店コード
                    CsvData(12) = KData.KesKamoku                                            ' 決済科目
                    CsvData(13) = KData.KesKouza                                ' 決済口座番号

                    Select Case KData.TesTyoKbn
                        Case "0"    '都度徴求の場合
                            CsvData(14) = "都度徴求"                               ' 手数料徴求区分
                        Case "1"    '一括徴求の場合
                            CsvData(14) = "一括徴求"
                        Case "2"    '特別免除の場合
                            CsvData(14) = "特別免除"

                        Case "3"    '別途徴求の場合
                            CsvData(14) = "別途徴求"
                        Case Else
                            CsvData(14) = "手数料未徴求"
                    End Select

                    Select Case KData.TesTyohh
                        Case "0"
                            CsvData(15) = "差引"                               ' 手数料徴求方法
                        Case "1"
                            CsvData(15) = "直接"                               ' 手数料徴求方法
                        Case Else
                            CsvData(15) = ""                                    ' 手数料徴求方法
                    End Select

                    CsvData(16) = KData.TorimatomeSit                           ' 取りまとめ店コード
                    CsvData(17) = GetTenmast(Jikinko, KData.TorimatomeSit, db)                                            ' 取りまとめ店名
                    CsvData(18) = KData.SyoriKen.Trim                                            ' 請求件数
                    CsvData(19) = KData.Syorikin.Trim                                            ' 請求金額
                    CsvData(20) = KData.FunouKen.Trim                                            ' 不能件数
                    CsvData(21) = KData.FunouKin.Trim                                            ' 不能金額
                    CsvData(22) = KData.FuriKen.Trim                                             ' 振替件数
                    CsvData(23) = KData.FuriKin.Trim                                             ' 振替金額
                    CsvData(24) = KData.TesuuKin.Trim                                            ' 手数料
                    CsvData(25) = KData.JifutiTesuuKin.Trim                                           ' 手数料内訳－自振
                    CsvData(26) = KData.FurikomiTesuukin.Trim                                           ' 手数料内訳－振込
                    CsvData(27) = KData.SonotaTesuuKin.Trim                                           ' 手数料内訳－その他
                    CsvData(28) = KData.NyukinKen.Trim                                           ' 入金分件数
                    CsvData(29) = KData.NyukinKin.Trim                                           ' 入金分金額
                    CsvData(30) = KData.OpeCode.Substring(0, 2)                             ' 科目コード
                    CsvData(31) = KData.OpeCode.Substring(2, 3)                             ' オペコード
                    Select Case KData.OpeCode
                        Case "04099"
                            CsvData(32) = "別段支払"                                                        ' オペレーション名
                        Case "01010"
                            CsvData(32) = "当座入金"
                        Case "02019"
                            CsvData(32) = "普通入金(NB)"
                        Case "04019"
                            CsvData(32) = "別段入金"
                        Case "99019"
                            CsvData(32) = "諸勘定入金"
                        Case "48100"
                            CsvData(32) = "為替振込"
                        Case "48500"
                            CsvData(32) = "為替付替"
                        Case "48600"
                            CsvData(32) = "為替請求"
                        Case "99418"
                            CsvData(32) = "手数料徴求(連動)"
                        Case "99419"
                            CsvData(32) = "諸勘定連動入金"
                    End Select

                    CsvData(33) = KData.ope_nyukin.Trim                                          ' 入出金額（オペコード単位）
                    CsvData(34) = KData.ope_tesuu.Trim                                           ' 手数料額（オペコード単位）

                    If newkey = oldkey Then
                        CsvData(35) = "0"                                           ' 集計フラグ 集計しない
                    Else
                        CsvData(35) = "1"                                          ' 集計フラグ 集計する
                    End If

                    Select Case KData.ToriKbn                     ' 0：資金決済と手数料徴求の両方の先、1：資金決済のみ先、2：手数料徴求のみ先
                        Case "0"
                            CsvData(36) = "決済・手数"
                        Case "1"
                            CsvData(36) = "決済"
                        Case "2"
                            CsvData(36) = "手数"
                        Case Else
                            CsvData(36) = ""
                    End Select

                    '2010/02/02 追加
                    CsvData(37) = GetTenmast(KData.KesKinCode, "", db, True)                        '決済金融機関名
                    CsvData(38) = GetTenmast(KData.KesKinCode, KData.KesSitCode, db, False)         '決済支店名
                    '================

                    'ＣＳＶ出力処理
                    If CSVObject.Output(CsvData) = 0 Then
                        Return -1
                    End If

                    oldkey = KData.TorisCode & KData.TorifCode & KData.FuriDate
                    OraKesReader.NextRead()
                End While
            Else
                Return -1
            End If


        Catch ex As Exception
            Mainlog.Write("資金決済取消印刷データ作成", "失敗", ex.Message)
            Return -300
        Finally
            If Not OraKesReader Is Nothing Then OraKesReader.Close()
        End Try

        Return 0

    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strSitName As String = ""

        Try
            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")
            If orareader.DataReader(sql) = True Then
                strSitName = orareader.GetString("SIT_NNAME_N")
                Return strSitName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function
    '2010/02/02 追加
    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle, ByVal KIN As Boolean) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strKinName As String = ""

        Try
            If KIN = True Then
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' ORDER BY SIT_NO_N")
            Else
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")
            End If


            If orareader.DataReader(sql) = True Then
                If KIN = True Then
                    strKinName = orareader.GetString("KIN_KNAME_N")
                Else
                    strKinName = orareader.GetString("SIT_KNAME_N")
                End If
                Return strKinName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function
    '===============
    ' 機能　 ： 資金決済データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    ''
    Private Function fn_KessaiData(ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal OraReader As CASTCommon.MyOracleReader, ByVal TimeStamp As String) As Boolean

        Dim strKamokuOpeCode As String

        Try
            Dim T_01010 As New CAstFormKes.ClsFormSikinFuri.T_01010
            Dim T_02019 As New CAstFormKes.ClsFormSikinFuri.T_02019
            Dim T_04019 As New CAstFormKes.ClsFormSikinFuri.T_04019
            Dim T_04099 As New CAstFormKes.ClsFormSikinFuri.T_04099
            Dim T_04419 As New CAstFormKes.ClsFormSikinFuri.T_04419
            Dim T_48100 As New CAstFormKes.ClsFormSikinFuri.T_48100
            Dim T_48500 As New CAstFormKes.ClsFormSikinFuri.T_48500
            Dim T_48600 As New CAstFormKes.ClsFormSikinFuri.T_48600
            Dim T_99019 As New CAstFormKes.ClsFormSikinFuri.T_99019
            Dim T_99418 As New CAstFormKes.ClsFormSikinFuri.T_99418
            Dim T_99419 As New CAstFormKes.ClsFormSikinFuri.T_99419

            KData.OpeCode = OraReader.GetString("KAMOKU_CODE_KR") + OraReader.GetString("OPE_CODE_KR")
            strKamokuOpeCode = KData.OpeCode

            KData.record320 = OraReader.GetString("DENBUN_ALL_KR")
            KData.KessaiKbn = OraReader.GetString("KESSAI_KBN_T")                              '決済区分
            KData.KesKouza = OraReader.GetString("TUKEKOUZA_T")
            Select Case KData.OpeCode
                Case "01010" '当座入金
                    T_01010.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_01010.KINGAKU
                    KData.ope_tesuu = ""
                Case "02019" '普通入金(NB)
                    T_02019.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_02019.KINGAKU
                    KData.ope_tesuu = ""
                Case "04019" '別段入金(NB)
                    T_04019.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_04019.KINGAKU
                    KData.ope_tesuu = ""
                Case "04099" '別段支払(NB)
                    T_04099.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_04099.KINGAKU
                    KData.ope_tesuu = ""
                Case "48100" '振込関連
                    T_48100.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_48100.KINGAKU
                    KData.ope_tesuu = ""
                Case "48500" '雑付替
                    T_48500.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_48500.KINGAKU
                    KData.ope_tesuu = ""
                Case "48600" '雑請求
                    T_48600.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_48600.KINGAKU
                    KData.ope_tesuu = ""
                Case "99019" '諸勘定入金
                    T_99019.DataSepaPlus = KData.record320
                    If KData.KessaiKbn = "00" AndAlso _
                       T_99019.KOUZA_NO = "00" & Mid(KData.KesKouza, 1, 5) Then
                        KData.ope_tesuu = ""
                        KData.ope_nyukin = T_99019.KINGAKU
                    Else
                        KData.ope_tesuu = T_99019.KINGAKU
                        KData.ope_nyukin = ""
                    End If
                    'If T_99019.ZENZAN.Trim = "0" Then
                    '    KData.ope_tesuu = T_99019.KINGAKU
                    '    KData.ope_nyukin = ""
                    'Else
                    '    KData.ope_tesuu = ""
                    '    KData.ope_nyukin = T_99019.KINGAKU
                    'End If
                Case "99418" '手数料徴求(連動)
                    T_99418.DataSepaPlus = KData.record320
                    KData.ope_nyukin = ""
                    KData.ope_tesuu = T_99418.TESUU_KINGAKU
                Case "99419" '諸勘定連動入金
                    T_99419.DataSepaPlus = KData.record320
                    KData.ope_nyukin = ""
                    KData.ope_tesuu = T_99419.KINGAKU
            End Select
            ' データ設定
            With KData
                '.KessaiDate = ParaKessaiDate                            '決済日
                .TorisCode = OraReader.GetString("TORIS_CODE_T")                              '取引先主コード
                .TorifCode = OraReader.GetString("TORIF_CODE_T")                              '取引先副コード
                .ToriKName = OraReader.GetString("ITAKU_KNAME_T")
                .ToriNName = OraReader.GetString("ITAKU_NNAME_T")
                .FuriCode = OraReader.GetString("FURI_CODE_T")                                '振替コード
                .KigyoCode = OraReader.GetString("KIGYO_CODE_T")                              '企業コード
                .FuriDate = OraReader.GetString("FURI_DATE_S")                                '振替日
                .KessaiKbn = OraReader.GetString("KESSAI_KBN_T")                              '決済区分
                .KesKinCode = OraReader.GetString("TUKEKIN_NO_T")                             '決済金融機関コード
                .KesSitCode = OraReader.GetString("TUKESIT_NO_T")                             '決済支店コード
                .KesKamoku = OraReader.GetString("TUKEKAMOKU_T")                              '決済科目
                .KesKouza = OraReader.GetString("TUKEKOUZA_T")                                '決済口座番号
                .TesTyoKbn = OraReader.GetString("TESUUTYO_KBN_T")                            '手数料徴求区分
                .TesTyohh = OraReader.GetString("TESUUTYO_PATN_T")                            '手数料徴求方法
                .TorimatomeSit = OraReader.GetString("TORIMATOME_SIT_T")                      'とりまとめ店コード
                .SyoriKen = OraReader.GetString("SYORI_KEN_S")                                  '請求件数
                .Syorikin = OraReader.GetString("SYORI_KIN_S")                                   '請求金額
                .FunouKen = OraReader.GetString("FUNOU_KEN_S")                                   '不能件数
                .FunouKin = OraReader.GetString("FUNOU_KIN_S")                                   '不能金額
                .FuriKen = OraReader.GetString("FURI_KEN_S")                                     '振替件数
                .FuriKin = OraReader.GetString("FURI_KIN_S")                                     '振替金額
                .TesuuKin = OraReader.GetString("TESUU_KIN_S")                                   '手数料
                .JifutiTesuuKin = OraReader.GetString("TESUU_KIN1_S")                           '自振手数料
                .FurikomiTesuukin = OraReader.GetString("TESUU_KIN3_S")                          '振込手数料
                .SonotaTesuuKin = OraReader.GetString("TESUU_KIN2_S")                           'その他手数料
                .NyukinKen = OraReader.GetString("FURI_KEN_S")              '入金件数
                If .TesTyohh = "0" AndAlso .TesTyoKbn = "0" Then
                    If Long.Parse(.FuriKin) - Long.Parse(.TesuuKin) >= 0 Then
                        .NyukinKin = (Long.Parse(.FuriKin) - Long.Parse(.TesuuKin)).ToString              '入金金額
                    Else
                        .NyukinKin = .FuriKin
                    End If
                Else
                    .NyukinKin = .FuriKin
                End If
                If OraReader.GetString("TESUU_TIME_STAMP_S") = TimeStamp AndAlso OraReader.GetString("KESSAI_TIME_STAMP_S") = TimeStamp Then
                    .ToriKbn = "0"
                ElseIf OraReader.GetString("KESSAI_TIME_STAMP_S") = TimeStamp Then
                    .ToriKbn = "1"
                ElseIf OraReader.GetString("TESUU_TIME_STAMP_S") = TimeStamp Then
                    .ToriKbn = "2"
                End If
                .TesuuTyoFlg = OraReader.GetString("TESUUTYO_FLG_S")
            End With

            ' 固定長に変換する
            KData.Data = KData.Data

        Catch ex As Exception
            Mainlog.Write("資金決済データ作成", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function
    ' 機能　 ： 学校マスタ２の決済種別を取得
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 決済種別 = 0：委託者一括請求 or 1：費目口座単位請求
    '
    Private Function GetKessaiSyubetu(ByVal GakkouCode As String, ByRef KessaiSyubetu As String) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraGak2Reader As New CASTCommon.MyOracleReader(MainDB)
        Try
            SQL.Append("SELECT")
            SQL.Append(" KESSAI_SYUBETU_T")
            SQL.Append(" FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(GakkouCode))
            If OraGak2Reader.DataReader(SQL) = True Then
                KessaiSyubetu = OraGak2Reader.GetString("KESSAI_SYUBETU_T")
            Else
                Throw New Exception("学校マスタ２に該当データが存在しません。学校コード：" & GakkouCode)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("決済種別取得処理", "失敗", ex.Message)
            Return False
        Finally
            OraGak2Reader.Close()
            OraGak2Reader = Nothing
        End Try
    End Function
End Class
