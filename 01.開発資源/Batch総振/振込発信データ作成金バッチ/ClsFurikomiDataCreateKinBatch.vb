Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstFormKes
Imports System.Collections.Generic
Imports System.Collections
'*** Str Add 2016/01/11 タスク)斎藤 for 多重実行対応 ***
Imports Microsoft.VisualBasic
'*** End Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***

'振込発信データ作成処理
Public Class ClsFurikomiDataCreateKinBatch

    '***************************************************************************************************
    '必読！
    '為替請求に関する修正は為替請求リエンタ作成側も修正すること
    '***************************************************************************************************

    Private Const FD_COUNT_LIMIT As Integer = 5000 'リエンタＦＤ１枚あたりの最大件数をセットする

    Public MainLOG As New CASTCommon.BatchLOG("KFS045", "振込発信データ作成")

    Private MainDB As CASTCommon.MyOracle

    ' パブリックフォーマット
    Private FmtComm As New CAstFormat.CFormat

    Private strKEKKA As String              ' データ作成結果

    Private jobMessage As String = ""          ' ジョブ監視メッセージ

    ' 処理日付
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ''' <summary>
    ''' iniファイル情報
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni
        Dim KAWASE_CENTER As String          '発信センタ名
        Dim JIKINKO_CODE As String           '自金庫コード
        Dim JIKINKO_NAME As String           '自金庫名
        Dim HONBU_CODE As String             '本部コード
        Dim TESUU_KOUZA1 As String           '手数料入金口座１
        Dim UCHIWAKE1 As String              '内訳１
        Dim TESUU_KOUZA2 As String           '手数料入金口座２
        Dim UCHIWAKE2 As String              '内訳２
        Dim TESUU_KOUZA3 As String           '手数料入金口座３
        Dim UCHIWAKE3 As String              '内訳３
        Dim TESUU_KOUZA4 As String           '手数料入金口座４
        Dim UCHIWAKE4 As String              '内訳４
        Dim TESUU_KOUZA5 As String           '手数料入金口座５
        Dim UCHIWAKE5 As String              '内訳５
        Dim KAWASE_IRAININ As String         '為替依頼人名
        Dim RIENTA_PATH As String            'リエンタファイル作成先
        Dim DAT_PATH As String               'DATのパス
        Dim CSVPATH As String                'CSVファイル作成先
        Dim TESUU_OPEKBN As String           '手数料オペ区分
        Dim RIENTA_FILENAME As String        'リエンタファイル名
        Dim SIKINTEKIYOU As String           '摘要名の一部をINIファイルより取得するため
        Dim SEIKYU As String                 '為替請求設定
        '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
        Dim ZEIKIJUN As String               '消費税基準
        '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
        ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
        Dim RSV2_S_KAWASE_HONSITEN As String ' 為替振込明細票(本支店為替)印刷要否
        Dim RSV2_S_KAWASE_TAKOU As String    ' 為替振込明細票(他行為替)印刷要否
        Dim RSV2_S_KAWASE_LOGGING As String  ' 為替振込明細票(ロギング)印刷要否
        ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END
    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' 発信マスタのキー項目＋リエンタファイル名
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure hasmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' 初期化
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private haskey As hasmastKey

    ''' <summary>
    ''' 総振決済マスタのキー項目＋リエンタファイル名
    ''' </summary>
    ''' <remarks></remarks>
    Structure kesmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' 初期化
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private keskey As kesmastKey

    Private ParaHassinDate As String     'パラメータから引き継いだ発信日
    Private ParaKyuDate As String        'パラメータから引き継いだ給振可能日
    Private ParaSouDate As String        'パラメータから引き継いだ総振可能日

    Private HassinList As New List(Of CAstFormKes.ClsFormKes.KessaiData) '振込発信データ格納用
    Private HONBU_KNAME As String = ""

    Private TesuuList As New System.Collections.Hashtable
    Private Structure TesuuTable
        Dim TESUU_A1 As Integer
        Dim TESUU_A2 As Integer
        Dim TESUU_A3 As Integer
        Dim TESUU_B1 As Integer
        Dim TESUU_B2 As Integer
        Dim TESUU_B3 As Integer
        Dim TESUU_C1 As Integer
        Dim TESUU_C2 As Integer
        Dim TESUU_C3 As Integer
    End Structure

    Private JIKOU_KEN As Long = 0         '自行件数
    Private TAKOU_KEN As Long = 0         '他行分件数
    Private JIFURI_KEN As Long = 0        '自振ロギング件数

    Private Structure FKeyInfo
        '発信データ作成＆帳票ＣＳＶデータ作成兼用
        Dim TORIS_CODE As String            ' 取引先主コード
        Dim TORIF_CODE As String            ' 取引先副コード
        Dim BAITAI_CODE As String           ' 媒体コード
        Dim FURI_DATE As String             ' 振込日
        Dim MOTIKOMI_SEQ As Integer         ' 持込SEQ
        Dim SOUSIN_KBN As String            ' 送信区分
        Dim SYUBETU As String               ' 種別
        Dim SYUMOKU_CODE As String          ' 種目コード
        Dim FUKA_CODE As String             ' 付加コード
        Dim FURI_CODE As String             ' 振替コード
        Dim KIGYO_CODE As String            ' 企業コード
        Dim ITAKU_CODE As String            ' 委託者コード
        Dim ITAKU_KNAME As String           ' 委託者名カナ
        Dim ITAKU_NNAME As String           ' 委託者名漢字
        Dim TORIMATOME_SIT As String        ' 取りまとめ店
        Dim TUKEKIN_KNAME As String         ' 金融機関名
        Dim TUKESIT_KNAME As String         ' 支店名
        Dim BIKOU1 As String                ' 備考１
        Dim BIKOU2 As String                ' 備考２
        '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
        Dim TSIT_NO As String               ' 取扱支店コード
        '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

        Dim TEKIYOU_SYUBETU As String       ' 適用種別
        Dim FURI_NAME As String             ' 振替名称

        Dim TotalKen As Long                ' 明細の合計件数
        Dim TotalKin As Long                ' 明細の合計金額
        Dim JikouKen As Long                ' 明細の自行合計件数
        Dim JikouKin As Long                ' 明細の自行合計金額
        Dim TakouKen As Long                ' 明細の他行合計件数
        Dim TakouKin As Long                ' 明細の他行合計金額
        Dim JifuriKen As Long               ' 明細の自振ロギング合計件数
        Dim JifuriKin As Long               ' 明細の自振ロギング合計金額
        '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
        Dim TesuuKin As Long                ' 明細の手数料合計
        '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END

        Dim MESSAGE As String

        Dim TOUROKU_DATE As String          ' 受付日
        Dim TUKEKIN_NO As String            ' 決済金融機関コード
        Dim TUKESIT_NO As String            ' 決済支店コード
        Dim TUKEKAMOKU As String            ' 決済科目
        Dim TUKEKOUZA As String             ' 決済口座番号
        Dim KESSAI_PATN As String           ' 決済パターン
        Dim TESUUTYO_KBN As String          ' 手数料徴求区分

        Dim FURIKIN_KEI As Long
        Dim FURIKIN_KEIJ As Long
        Dim FURIKIN_KEIH As Long
        Dim FURIKIN_KEIT As Long
        Dim TESUU_KIN_KEI As Long

        Dim TESUU_TABLE_ID As String        ' 振込手数料基準ID

        ' 初期化
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            BAITAI_CODE = ""
            FURI_DATE = ""
            MOTIKOMI_SEQ = 0
            SOUSIN_KBN = ""
            SYUBETU = ""
            SYUMOKU_CODE = ""
            FUKA_CODE = ""
            FURI_CODE = ""
            KIGYO_CODE = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            ITAKU_NNAME = ""
            TORIMATOME_SIT = ""
            TUKEKIN_KNAME = ""
            TUKESIT_KNAME = ""
            BIKOU1 = ""
            BIKOU2 = ""
            '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            TSIT_NO = ""
            '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

            TEKIYOU_SYUBETU = ""
            FURI_NAME = ""

            TotalKen = 0
            TotalKin = 0
            JikouKen = 0
            JikouKin = 0
            TakouKen = 0
            TakouKin = 0
            JifuriKen = 0
            JifuriKin = 0
            '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
            TesuuKin = 0
            '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END

            MESSAGE = ""

            TOUROKU_DATE = ""
            TUKEKIN_NO = ""
            TUKESIT_NO = ""
            TUKEKAMOKU = ""
            TUKEKOUZA = ""
            KESSAI_PATN = ""
            TESUUTYO_KBN = ""

            FURIKIN_KEI = 0
            FURIKIN_KEIJ = 0
            FURIKIN_KEIH = 0
            FURIKIN_KEIT = 0
            TESUU_KIN_KEI = 0

            TESUU_TABLE_ID = ""
        End Sub

        ' ＤＢからの値を設定（振込発信リエンタ作成用）
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            MOTIKOMI_SEQ = oraReader.GetInt("MOTIKOMI_SEQ_S")
            SOUSIN_KBN = oraReader.GetString("SOUSIN_KBN_S")
            SYUBETU = oraReader.GetString("SYUBETU_S")
            SYUMOKU_CODE = oraReader.GetString("SYUMOKU_CODE_T")
            FUKA_CODE = oraReader.GetString("FUKA_CODE_T")
            FURI_CODE = oraReader.GetString("FURI_CODE_S")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_S")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_S")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            TUKEKIN_KNAME = oraReader.GetString("KIN_KNAME_N")
            TUKESIT_KNAME = oraReader.GetString("SIT_KNAME_N")
            BIKOU1 = oraReader.GetString("BIKOU1_T")
            BIKOU2 = oraReader.GetString("BIKOU2_T")
            '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            TSIT_NO = oraReader.GetString("TSIT_NO_T")
            '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

            TOUROKU_DATE = oraReader.GetString("TOUROKU_DATE_S")
            TUKEKIN_NO = oraReader.GetString("TUKEKIN_NO_T")
            TUKESIT_NO = oraReader.GetString("TUKESIT_NO_T")
            TUKEKAMOKU = oraReader.GetString("TUKEKAMOKU_T")
            TUKEKOUZA = oraReader.GetString("TUKEKOUZA_T")
            KESSAI_PATN = oraReader.GetString("KESSAI_PATN_T")
            TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_T")

            TESUU_TABLE_ID = oraReader.GetString("TESUU_TABLE_ID_T")
        End Sub

    End Structure

    Private Structure KKeyInfo
        '発信データ作成＆帳票ＣＳＶデータ作成兼用
        Dim TORIS_CODE As String            ' 取引先主コード
        Dim TORIF_CODE As String            ' 取引先副コード
        Dim FURI_DATE As String             ' 振込日
        Dim ITAKU_CODE As String            ' 委託者コード
        Dim ITAKU_KNAME As String           ' 委託者名カナ
        Dim TORIMATOME_SIT As String        ' 取りまとめ店
        Dim SIT_KNAME As String             ' 取りまとめ店名
        Dim KESSAI_KBN As String            ' 決済区分
        Dim KESSAI_PATN As String           ' 資金確保方法
        Dim BAITAI_CODE As String           ' 媒体コード

        Dim FURI_KEN As String              ' 振込済件数
        Dim FURI_KIN As String              ' 振込済金額
        Dim MESSAGE As String

        ' 初期化
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            FURI_DATE = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            TORIMATOME_SIT = ""
            SIT_KNAME = ""
            KESSAI_KBN = ""
            KESSAI_PATN = ""
            BAITAI_CODE = ""

            FURI_KEN = ""
            FURI_KIN = ""

            MESSAGE = ""
        End Sub

        ' ＤＢからの値を設定（為替請求リエンタ作成用）
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_S")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            SIT_KNAME = oraReader.GetString("SIT_KNAME_N")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_T")
            KESSAI_PATN = oraReader.GetString("KESSAI_PATN_T")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")

            FURI_KEN = oraReader.GetString("FURI_KEN_S")
            FURI_KIN = oraReader.GetString("FURI_KIN_S")
        End Sub

    End Structure

    '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    Private htFuriTesuuID As Hashtable
    Private Structure strcFuriTesuuInfo
        Dim TESUU_A1 As String
        Dim TESUU_A2 As String
        Dim TESUU_A3 As String
        Dim TESUU_B1 As String
        Dim TESUU_B2 As String
        Dim TESUU_B3 As String
        Dim TESUU_C1 As String
        Dim TESUU_C2 As String
        Dim TESUU_C3 As String
    End Structure
    '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 振込発信初期処理
    ''' </summary>
    ''' <returns>正常:True 異常:False</returns>
    ''' <remarks></remarks>
    Public Function Init(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try

            'パラメータの読込
            param = CmdArgs(0).Split(","c)
            If param.Length = 4 Then

                'ログ書込み情報の設定
                MainLOG.FuriDate = "00000000"
                MainLOG.JobTuuban = CInt(param(3))
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(初期処理)開始", "成功")

                ParaHassinDate = param(0)                       '発信日をセット
                ParaKyuDate = param(1)                       '給振可能日をセット
                ParaSouDate = param(2)                       '総振可能日をセット

            Else
                MainLOG.Write("(初期処理)開始", "失敗", "コマンドライン引数のパラメータが不正です")
                Return False
            End If

            'iniファイルの読込
            If IniRead() = False Then
                Return False
            End If

            '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            '消費税管理クラスインスタンス生成
            Me.TAX = New CASTCommon.ClsTAX
            '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

            Return True

        Catch ex As Exception
            MainLOG.Write("(初期処理)開始", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write("(初期処理)終了", "成功")
        End Try

    End Function

    Private Function IniRead() As Boolean

        ini_info.KAWASE_CENTER = CASTCommon.GetFSKJIni("KAWASE", "KAWASECENTER")      '発信センタ名
        If ini_info.KAWASE_CENTER = "err" OrElse ini_info.KAWASE_CENTER = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:発信センタ名 分類:KAWASE 項目:KAWASECENTER")
            jobMessage = "設定ファイル取得失敗 項目名:発信センタ名 分類:KAWASE 項目:KAWASECENTER"
            Return False
        End If

        ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")           '自金庫コード
        If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
            jobMessage = "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD"
            Return False
        End If

        ini_info.JIKINKO_NAME = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")       '自金庫名
        If ini_info.JIKINKO_NAME = "err" OrElse ini_info.JIKINKO_NAME = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME")
            jobMessage = "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME"
            Return False
        End If

        ini_info.HONBU_CODE = CASTCommon.GetFSKJIni("COMMON", "HONBUCD")         '本部コード
        If ini_info.HONBU_CODE = "err" OrElse ini_info.HONBU_CODE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD")
            jobMessage = "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD"
            Return False
        End If

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        'リエンタファイル作成先
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR")
            jobMessage = "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DATのパス
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            jobMessage = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
            Return False
        End If

        ini_info.CSVPATH = CASTCommon.GetFSKJIni("COMMON", "CSV")           'CSVファイル作成先
        If ini_info.CSVPATH = "err" OrElse ini_info.CSVPATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:CSVファイル保存先 分類:COMMON 項目:CSV")
            jobMessage = "設定ファイル取得失敗 項目名:CSVファイル保存先 分類:COMMON 項目:CSV"
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KAWASE", "RIENTANAME")       'リエンタファイル名
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル名 分類:KAWASE 項目:RIENTANAME")
            jobMessage = "設定ファイル取得失敗 項目名:リエンタファイル名 分類:KAWASE 項目:RIENTANAME"
            Return False
        End If

        ini_info.SEIKYU = CASTCommon.GetFSKJIni("KAWASE", "SEIKYU")             '為替請求設定
        If ini_info.SEIKYU = "err" OrElse ini_info.SEIKYU = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替請求設定 分類:KAWASE 項目:SEIKYU")
            jobMessage = "設定ファイル取得失敗 項目名:為替請求設定 分類:KAWASE 項目:SEIKYU"
            Return False
        End If

        '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
        ini_info.ZEIKIJUN = CASTCommon.GetFSKJIni("KAWASE", "ZEIKIJUN")
        If ini_info.ZEIKIJUN = "err" OrElse ini_info.ZEIKIJUN = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:税基準 分類:KAWASE 項目:ZEIKIJUN")
            jobMessage = "設定ファイル取得失敗 項目名:税基準 分類:KAWASE 項目:ZEIKIJUN"
            Return False
        End If
        '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

        ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
        ini_info.RSV2_S_KAWASE_HONSITEN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_HONSITEN")
        If ini_info.RSV2_S_KAWASE_HONSITEN = "err" OrElse ini_info.RSV2_S_KAWASE_HONSITEN = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替振込明細表(本支店為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_HONSITEN")
            jobMessage = "設定ファイル取得失敗 項目名:為替振込明細表(本支店為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_HONSITEN"
            Return False
        End If

        ini_info.RSV2_S_KAWASE_TAKOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_TAKOU")
        If ini_info.RSV2_S_KAWASE_TAKOU = "err" OrElse ini_info.RSV2_S_KAWASE_TAKOU = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替振込明細表(他行為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_TAKOU")
            jobMessage = "設定ファイル取得失敗 項目名:為替振込明細表(他行為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_TAKOU"
            Return False
        End If

        ini_info.RSV2_S_KAWASE_LOGGING = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_LOGGING")
        If ini_info.RSV2_S_KAWASE_LOGGING = "err" OrElse ini_info.RSV2_S_KAWASE_LOGGING = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替振込明細表(ロギング)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_LOGGING")
            jobMessage = "設定ファイル取得失敗 項目名:為替振込明細表(ロギング)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_LOGGING"
            Return False
        End If
        ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END

        Return True

    End Function

    ' 機能　 ： 振込発信リエンタ作成処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Public Function Main(ByVal command As String) As Integer

        '*** Str Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If
        '*** End Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***

        MainDB = New CASTCommon.MyOracle
        FmtComm.Oracle = MainDB

        Dim bRet As Boolean = True
        Dim iRet As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "振込発信データ作成金バッチ(開始)", "成功")
            'MainLOG.Write("(主処理)開始", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


            MainDB.BeginTrans()     ' トランザクション開始

            '*** Str Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write("(主処理)", "失敗", "振込発信リエンタ作成処理で実行待ちタイムアウト")
                MainLOG.UpdateJOBMASTbyErr("振込発信リエンタ作成処理で実行待ちタイムアウト")
                Return -1
            End If
            '*** End Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***

            '*******************************
            ' 回次を取得
            '*******************************
            If GetKaiji() = False Then
                MainLOG.Write("(主処理)", "失敗", "回次の取得に失敗しました")
                Return -1
            End If

            '*****************************************
            ' 振込発信データの格納とスケジュールの更新
            '*****************************************
            iRet = MakeFurikomiData()
            Select Case iRet
                Case 0          ' データ格納成功
                    bRet = True

                Case 1          ' 対象データ０件
                    bRet = True
                Case Else       ' データ格納失敗
                    bRet = False
            End Select

            '*******************************
            ' 帳票出力
            '*******************************
            ' 振込発信データが１件以上存在する場合、帳票出力
            If iRet = 0 Then
                Dim ExeRepo As New CAstReports.ClsExecute
                bRet = True

                '為替振込明細表(本支店為替)
                If bRet = True AndAlso JIKOU_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP010", "為替振込明細表(本支店為替)")
                End If

                '為替振込明細表(他行為替)
                If bRet = True AndAlso TAKOU_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP011", "為替振込明細表(他行為替)")
                End If

                '為替振込明細表(自振ロギング登録)
                If bRet = True AndAlso JIFURI_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP012", "為替振込明細表(自振ロギング登録)")
                End If

            End If

            If bRet = False Then
                If jobMessage = "" Then
                    Call MainLOG.UpdateJOBMASTbyErr("ログ参照")
                Else
                    Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                End If

                '*** Str Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                '*** End Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***

                ' ロールバック
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "対象データ０件"
                End If

                Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                '*** Str Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                '*** End Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***

                ' コミット
                MainDB.Commit()
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            MainLOG.Write("(主処理)", "失敗", ex.ToString)
            Return 1
        Finally
            '*** Str Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***
            If Not MainDB Is Nothing Then
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

                ' ロールバック
                MainDB.Rollback()
            End If
            '*** End Add 2016/01/11 タスク）斎藤 for 多重実行対応 ***

            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "振込発信データ作成金バッチ(終了)", "成功")
            'MainLOG.Write("(主処理)終了", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try

        Return 0

    End Function

    ' 機能　 ： 振込発信データ作成処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function MakeFurikomiData() As Integer

        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = New StringBuilder(256)

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim tbl As New TesuuTable

        Try
            MainLOG.Write("(振込発信データ格納)開始", "成功")

            '--------------------------------------------------
            '振込手数料マスタから読込
            '--------------------------------------------------
            With SQL
                .Append("select * from TESUUMAST, TAXMAST")
                .Append(" where TAX_ID_C = TAX_ID_Z")
                .Append(" and FSYORI_KBN_C = '3'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With


            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    tbl.TESUU_A1 = Integer.Parse(OraReader.GetString("TESUU_A1_C"))
                    tbl.TESUU_A2 = Integer.Parse(OraReader.GetString("TESUU_A2_C"))
                    tbl.TESUU_A3 = Integer.Parse(OraReader.GetString("TESUU_A3_C"))
                    tbl.TESUU_B1 = Integer.Parse(OraReader.GetString("TESUU_B1_C"))
                    tbl.TESUU_B2 = Integer.Parse(OraReader.GetString("TESUU_B2_C"))
                    tbl.TESUU_B3 = Integer.Parse(OraReader.GetString("TESUU_B3_C"))
                    tbl.TESUU_C1 = Integer.Parse(OraReader.GetString("TESUU_C1_C"))
                    tbl.TESUU_C2 = Integer.Parse(OraReader.GetString("TESUU_C2_C"))
                    tbl.TESUU_C3 = Integer.Parse(OraReader.GetString("TESUU_C3_C"))

                    '税率ID、振込手数料基準ID、種別をキーにハッシュテーブルに格納する
                    TesuuList.Add(OraReader.GetString("TAX_ID_C") & OraReader.GetString("TESUU_TABLE_ID_C") & OraReader.GetString("SYUBETU_C"), tbl)

                    OraReader.NextRead()
                End While
            Else
                MainLOG.Write("振込手数料マスタ取得", "失敗", "手数料テーブル取得失敗")
                jobMessage = "振込手数料マスタ取得 失敗 "
                Return -1
            End If

        Catch ex As Exception
            MainLOG.Write("振込手数料マスタ取得", "失敗", ex.Message)
            jobMessage = "振込手数料マスタ取得 失敗 例外が発生しました。"
            Return -1
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Try

            OraSchReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Length = 0
            SQL.AppendLine("SELECT")
            SQL.AppendLine(" TORIS_CODE_S")
            SQL.AppendLine(",TORIF_CODE_S")
            SQL.AppendLine(",BAITAI_CODE_S")
            SQL.AppendLine(",FURI_DATE_S")
            SQL.AppendLine(",MOTIKOMI_SEQ_S")
            SQL.AppendLine(",SYUBETU_S")
            SQL.AppendLine(",FURI_CODE_S")
            SQL.AppendLine(",KIGYO_CODE_S")
            SQL.AppendLine(",ITAKU_CODE_S")
            SQL.AppendLine(",SOUSIN_KBN_S")

            SQL.AppendLine(",ITAKU_KNAME_T")
            SQL.AppendLine(",ITAKU_NNAME_T")
            SQL.AppendLine(",SYUMOKU_CODE_T")
            SQL.AppendLine(",FUKA_CODE_T")
            SQL.AppendLine(",TORIMATOME_SIT_T")
            SQL.AppendLine(",BIKOU1_T")
            SQL.AppendLine(",BIKOU2_T")
            '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            SQL.AppendLine(",TSIT_NO_T")
            '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

            SQL.AppendLine(",TOUROKU_DATE_S")
            SQL.AppendLine(",TUKEKIN_NO_T")
            SQL.AppendLine(",TUKESIT_NO_T")
            SQL.AppendLine(",TUKEKAMOKU_T")
            SQL.AppendLine(",TUKEKOUZA_T")
            SQL.AppendLine(",KESSAI_PATN_T")
            SQL.AppendLine(",TESUUTYO_KBN_T")
            SQL.AppendLine(",TESUU_TABLE_ID_T")

            SQL.AppendLine(",KIN_KNAME_N")
            SQL.AppendLine(",SIT_KNAME_N")

            SQL.AppendLine(" FROM S_TORIMAST")
            SQL.AppendLine("     ,S_SCHMAST")
            SQL.AppendLine("     ,TENMAST")

            SQL.AppendLine(" WHERE TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine("   AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.AppendLine(" AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
            SQL.AppendLine(" AND TORIMATOME_SIT_T = SIT_NO_N(+)")
            SQL.AppendLine("   AND FURI_DATE_S >= " & SQ(ParaHassinDate))

            ' 総合振込
            SQL.AppendLine("   AND ((SYUBETU_T = '21' AND FURI_DATE_S BETWEEN " & SQ(ParaHassinDate) & " AND " & SQ(ParaSouDate) & ")")
            ' 給与振込，賞与振込
            SQL.AppendLine("    OR (SYUBETU_T <> '21' AND FURI_DATE_S BETWEEN " & SQ(ParaHassinDate) & " AND " & SQ(ParaKyuDate) & "))")

            SQL.AppendLine("   AND TOUROKU_FLG_S = '1'")
            SQL.AppendLine("   AND HASSIN_FLG_S = '2'")
            SQL.AppendLine("   AND TYUUDAN_FLG_S = '0'")

            SQL.AppendLine(" ORDER BY FURI_DATE_S, SOUSIN_KBN_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")


            Dim Key As FKeyInfo = Nothing
            Dim test As String = SQL.ToString

            If OraSchReader.DataReader(SQL) = True Then
                ' キー初期化
                Key.Init()

                ' 最初のキー設定
                Call Key.SetOraDataKessai(OraSchReader)

                Do While OraSchReader.EOF = False
                    MainLOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                    MainLOG.FuriDate = Key.FURI_DATE

                    '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
                    '基準日から税率を取得する
                    Dim strKijunDate As String = String.Empty
                    If ini_info.ZEIKIJUN.Equals("0") = True Then
                        '振込日基準
                        strKijunDate = Key.FURI_DATE
                    Else
                        '発信日基準
                        strKijunDate = ParaHassinDate
                    End If

                    Me.TAX.GetZeiritsu(strKijunDate)
                    If Me.TAX.ZEIRITSU.Equals("err") = True Then
                        MainLOG.Write("税率取得", "失敗", "基準日：" & strKijunDate)
                        Return -1
                    End If

                    '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
                    Me.TAX.GetInshizei(strKijunDate)
                    If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                        MainLOG.Write("印紙税取得", "失敗", "基準日：" & strKijunDate)
                        Return -1
                    End If
                    '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

                    '振込手数料の再計算を行う
                    If Me.CalcFurikomiTesuu(Key) = False Then
                        Return -1
                    End If
                    '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<

                    ' 明細マスタから，発信マスタを作成する
                    If GetFurikomiData(Key) = False Then
                        Return -1
                    End If

                    ' スケジュールマスタの更新処理 
                    If UpdateSchMast(Key) = False Then
                        Return -1
                    End If

                    JIKOU_KEN += Key.JikouKen
                    TAKOU_KEN += Key.TakouKen
                    JIFURI_KEN += Key.JifuriKen

                    ' 対象データの次レコードを読込む
                    OraSchReader.NextRead()

                    If OraSchReader.EOF = False Then
                        ' キー初期化
                        Key.Init()

                        ' キーの再設定
                        Call Key.SetOraDataKessai(OraSchReader)

                    End If
                Loop

            End If

            If haskey.RecordNo = 0 Then
                MainLOG.Write("(振込発信データ格納)", "失敗", "件数０件")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(振込発信データ格納)", "失敗", ex.Message)
            Return -1
        Finally
            If Not OraSchReader Is Nothing Then OraSchReader.Close()
            MainLOG.Write("(振込発信データ格納)終了", "成功")
        End Try

        Return 0

    End Function

    ' 機能　 ： 為替振込データ作成処理
    '
    ' 引数　 ： ARG1 - キー情報
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function GetFurikomiData(ByRef Key As FKeyInfo) As Boolean
        Dim SQL As StringBuilder = New StringBuilder(256)
        '*** Str Upd 2016/01/11 タスク）斎藤 for MyOracleReaderクローズ忘れを修正（潜在） ***
        'Dim OraMeiReader As CASTCommon.MyOracleReader       ' 明細マスタ
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing       ' 明細マスタ
        '*** End Upd 2016/01/11 タスク）斎藤 for MyOracleReaderクローズ忘れを修正（潜在） ***

        ' 振込日
        Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

        ' 振込日の１営業日前，２営業日前
        Dim Zen1Day As String = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

        '*** Str Add 2016/01/11 タスク）斎藤 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Try
            '*** End Add 2016/01/11 タスク）斎藤 for MyOracleReaderクローズ忘れを修正（潜在） ***

            SQL.AppendLine("SELECT")
            SQL.AppendLine("      FURIKIN_K")
            SQL.AppendLine("    , TESUU_KIN_K")
            SQL.AppendLine("    , KEIYAKU_KIN_K")
            SQL.AppendLine("    , KEIYAKU_SIT_K")
            SQL.AppendLine("    , SUBSTRB(FURI_DATA_K, 43, 1)   KEIYAKU_KAMOKU_K")  ' 科目はFURI_DATA_Kの値を設定する
            SQL.AppendLine("    , KEIYAKU_KOUZA_K")
            SQL.AppendLine("    , KEIYAKU_KNAME_K")
            SQL.AppendLine("    , SINKI_CODE_K")
            SQL.AppendLine("    , TEKIYO_KBN_K")
            SQL.AppendLine("    , KTEKIYO_K")
            SQL.AppendLine("    , NTEKIYO_K")
            SQL.AppendLine("    , JYUYOUKA_NO_K")
            SQL.AppendLine("    , SUBSTRB(FURI_DATA_K, 113, 1)  SIKIBETU")          ' EDI情報用 識別表示
            SQL.AppendLine("    , KIN_KNAME_N")
            SQL.AppendLine("    , SIT_KNAME_N")
            SQL.AppendLine(" FROM ")
            SQL.AppendLine("      S_MEIMAST")
            SQL.AppendLine("    , TENMAST")
            SQL.AppendLine(" WHERE ")
            SQL.AppendLine("      TORIS_CODE_K     = " & SQ(Key.TORIS_CODE))
            SQL.AppendLine("  AND TORIF_CODE_K     = " & SQ(Key.TORIF_CODE))
            SQL.AppendLine("  AND FURI_DATE_K      = " & SQ(Key.FURI_DATE))
            SQL.AppendLine("  AND MOTIKOMI_SEQ_K   = " & Key.MOTIKOMI_SEQ)
            SQL.AppendLine("  AND DATA_KBN_K       = '2'")
            SQL.AppendLine("  AND FURIKETU_CODE_K  = 0")
            SQL.AppendLine("  AND FURIKIN_K       >= 0")
            SQL.AppendLine("  AND KEIYAKU_KIN_K    = KIN_NO_N(+)")
            SQL.AppendLine("  AND KEIYAKU_SIT_K    = SIT_NO_N(+)")
            SQL.AppendLine(" ORDER BY ")
            SQL.AppendLine("       RECORD_NO_K")

            OraMeiReader = New CASTCommon.MyOracleReader(MainDB)

            If OraMeiReader.DataReader(SQL) = True Then

                Dim KData As New ClsFormKes.KessaiData

                Do While OraMeiReader.EOF = False
                    haskey.RecordNo += 1
                    Key.TotalKen += 1
                    Key.TotalKin += OraMeiReader.GetInt64("FURIKIN_K")
                    '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
                    Key.TesuuKin += OraMeiReader.GetInt64("TESUU_KIN_K")
                    '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END

                    '適用種別設定
                    Select Case ParaHassinDate
                        Case Key.FURI_DATE
                            ' 当日
                            Key.TEKIYOU_SYUBETU = "ﾌﾘｺﾐ"
                        Case Zen1Day
                            ' １営業日前
                            Key.TEKIYOU_SYUBETU = "ｻｷﾌﾘ"
                        Case Is <= Zen2Day
                            ' ２営業日前以前
                            Select Case Key.SYUBETU
                                Case "21"
                                    ' 総振
                                    Key.TEKIYOU_SYUBETU = "ｻｷﾌﾘ"
                                Case "11"
                                    ' 給与
                                    Key.TEKIYOU_SYUBETU = "ｷﾕｳﾖ"
                                Case "12"
                                    ' 賞与
                                    Key.TEKIYOU_SYUBETU = "ｼﾖｳﾖ"
                            End Select
                    End Select

                    Select Case Key.SOUSIN_KBN
                        Case "0"
                            '為替振込
                            If OraMeiReader.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
                                Key.JikouKen += 1
                                Key.JikouKin += OraMeiReader.GetInt64("FURIKIN_K")
                            Else
                                Key.TakouKen += 1
                                Key.TakouKin += OraMeiReader.GetInt64("FURIKIN_K")
                            End If

                            KData = fn_KAWASE_FURIKOMI(Key, OraMeiReader, Zen1Day, Zen2Day)
                        Case "1"
                            'ロギング
                            Key.JifuriKen += 1
                            Key.JifuriKin += OraMeiReader.GetInt64("FURIKIN_K")

                        Case "2"
                            'CSVリエンタ(信組用)
                        Case Else
                            '送信区分異常
                            jobMessage = "送信区分異常：" & Key.SOUSIN_KBN
                            Return False
                    End Select

                    '総給振明細連携データテーブルに挿入
                    If Me.InsertSMRenkeimast(Key, KData, OraMeiReader) = False Then
                        Return False
                    End If

                    ' 振込発信データのリスト作成
                    HassinList.Add(KData)

                    OraMeiReader.NextRead()
                Loop

                '振込金額の合計を計算
                If Me.CalcFurikinKei(Key) = False Then
                    Return False
                End If

                '総給振依頼人連携データテーブルに挿入
                If Me.InsertSIRenkeimast(Key, KData) = False Then
                    Return False
                End If

            End If
            OraMeiReader.Close()

            Return True

            '*** Str Add 2016/01/11 タスク）斎藤 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Finally
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
        End Try
        '*** End Add 2016/01/11 タスク）斎藤 for MyOracleReaderクローズ忘れを修正（潜在） ***

    End Function

    ' 機能　 ： 為替振込データ作成処理
    '
    ' 引数　 ： ARG1 - キー情報
    '           ARG1 - オラクルリーダ
    '
    ' 戻り値 ： １レコードデータ
    '
    ' 備考　 ： 
    '
    Private Function fn_KAWASE_FURIKOMI(ByVal Key As FKeyInfo, ByVal oraMei As CASTCommon.MyOracleReader, ByVal zen1Day As String, ByVal zen2Day As String) As CAstFormKes.ClsFormKes.KessaiData
        Dim Data As New CAstFormKes.ClsFormSikinFuri.T_48100

        Data.KAMOKU_CODE = "48"
        Data.OPE_CODE = "100"
        Data.TORIATUKAI = Key.FURI_DATE

        If Key.SYUBETU = "11" AndAlso Key.TEKIYOU_SYUBETU <> "ｷﾕｳﾖ" Then
            '契約種別が給与で種別コードが給与にならない場合
            Data.BIKOU2 = "ｷﾕｳﾖ"
        ElseIf Key.SYUBETU = "12" AndAlso Key.TEKIYOU_SYUBETU <> "ｼﾖｳﾖ" Then
            '契約種別が賞与で種別コードが賞与にならない場合
            Data.BIKOU2 = "ｼﾖｳﾖ"
        Else
            Data.BIKOU2 = ""
        End If

        '種目コード、備考２設定
        Select Case Key.TEKIYOU_SYUBETU
            Case "ﾌﾘｺﾐ"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '振込公金
                        Data.SYUMOKU = "1074"
                        Data.BIKOU1 = "ﾟｺｳｷﾝﾟ "

                    Case "01"
                        '振込国庫金
                        Data.SYUMOKU = "1054"
                        Data.BIKOU1 = "ﾟｺｸﾌﾘﾟ "

                    Case Else
                        '振込一般
                        Data.SYUMOKU = "1022"
                        Data.BIKOU1 = ""

                End Select

            Case "ｻｷﾌﾘ"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '先振公金
                        Data.SYUMOKU = "1174"
                        Data.BIKOU1 = "ﾟｺｳｷﾝﾟ "

                    Case "01"
                        '先振国庫金
                        Data.SYUMOKU = "1154"
                        Data.BIKOU1 = "ﾟｺｸﾌﾘﾟ "

                    Case Else
                        '先振一般
                        Data.SYUMOKU = "1122"
                        Data.BIKOU1 = ""

                End Select

            Case "ｷﾕｳﾖ"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '給与公金
                        Data.SYUMOKU = "1271"
                        Data.BIKOU1 = "ﾟｺｳｷﾝﾟ "

                    Case "01"
                        '給与国庫金
                        Data.SYUMOKU = "1251"
                        Data.BIKOU1 = "ﾟｺｸﾌﾘﾟ "

                    Case Else
                        '給与一般
                        Data.SYUMOKU = "1211"
                        Data.BIKOU1 = ""

                End Select

            Case "ｼﾖｳﾖ"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '賞与公金
                        Data.SYUMOKU = "1272"
                        Data.BIKOU1 = "ﾟｺｳｷﾝﾟ "

                    Case "01"
                        ''賞与国庫金
                        Data.SYUMOKU = "1252"
                        Data.BIKOU1 = "ﾟｺｸﾌﾘﾟ "

                    Case Else
                        '賞与一般
                        Data.SYUMOKU = "1212"
                        Data.BIKOU1 = ""

                End Select
        End Select

        '20131111 金バッチ連携時は不要
        'Data.BIKOU1 = Data.BIKOU1 & Key.TUKESIT_KNAME & " ｱﾂｶｲ"
        '20131111 金バッチ連携時は不要

        If oraMei.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
            Data.JUSIN_TEN = "ﾟ " & oraMei.GetString("SIT_KNAME_N")
            Data.HASSIN_TEN = "ﾟ ｾﾝﾀ-"
        Else
            Data.JUSIN_TEN = oraMei.GetString("KIN_KNAME_N") & " " & oraMei.GetString("SIT_KNAME_N")
            Data.HASSIN_TEN = ini_info.KAWASE_CENTER
        End If

        Data.FUKA_CODE = Key.FUKA_CODE '000固定
        Data.KINGAKU = oraMei.GetInt64("FURIKIN_K").ToString.PadLeft(10)
        '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ---------------------------------------->>>>
        'Data.KESSAI_CNT = " "
        '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ----------------------------------------<<<<
        Call fn_FUGO_SETTEI(oraMei.GetInt64("FURIKIN_K").ToString("#,##0"), Data.KINGAKU_FUGOU)
        Data.KINGAKU_FUGOU = Data.KINGAKU_FUGOU.Trim.PadRight(15, " "c)
        Data.KOKYAKU_TESUU = ""
        Data.UKETORI_KAMOKU = oraMei.GetItem("KEIYAKU_KAMOKU_K")
        Data.UKETORI_KOUZA = oraMei.GetString("KEIYAKU_KOUZA_K").Trim.PadLeft(7, "0"c).PadRight(15)
        Data.UKETORI_NAME = oraMei.GetString("KEIYAKU_KNAME_K").PadRight(29).Substring(0, 29).Trim
        Data.IRAI_NAME = Key.ITAKU_KNAME.PadRight(48).Substring(0, 48).Trim

        If Key.SYUBETU = "21" AndAlso oraMei.GetString("SIKIBETU") = "Y" Then
            '総振で識別表示がYの場合はEDI情報設定
            Data.EDI_INFO = oraMei.GetString("JYUYOUKA_NO_K")
        Else
            Data.EDI_INFO = ""
        End If

        '備考が設定されている場合
        If Key.BIKOU1 <> "" Then
            Data.BIKOU1 = Key.BIKOU1
        End If
        If Key.BIKOU2 <> "" Then
            Data.BIKOU2 = Key.BIKOU2
        End If

        Data.YOBI1 = ""

        Dim Kdata As New CAstFormKes.ClsFormKes.KessaiData
        Kdata.record320 = Data.Data
        Kdata.OpeCode = String.Concat(Data.KAMOKU_CODE, Data.OPE_CODE)
        Kdata.TorisCode = Key.TORIS_CODE
        Kdata.TorifCode = Key.TORIF_CODE
        Kdata.ToriNName = Key.ITAKU_NNAME
        Kdata.TorimatomeSit = Key.TORIMATOME_SIT
        Kdata.KesKinCode = oraMei.GetString("KEIYAKU_KIN_K")
        Kdata.KesSitCode = oraMei.GetString("KEIYAKU_SIT_K")
        Kdata.FurikomiTesuukin = oraMei.GetString("TESUU_KIN_K")

        Return Kdata
    End Function

#Region "決済用"
    ' 機能　 ： 資金決済データ作成処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '

    Private Function MakeKessaiData() As Integer

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = New StringBuilder(256)

        Try
            MainLOG.Write("(資金決済データ格納)開始", "成功")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.AppendLine("SELECT")
            SQL.AppendLine(" MAX(TORIS_CODE_S) TORIS_CODE_S")
            SQL.AppendLine(",MAX(TORIF_CODE_S) TORIF_CODE_S")
            SQL.AppendLine(",MAX(FURI_DATE_S) FURI_DATE_S")
            SQL.AppendLine(",MAX(SYUBETU_S) SYUBETU_S")
            SQL.AppendLine(",MAX(ITAKU_CODE_S) ITAKU_CODE_S")
            SQL.AppendLine(",SUM(FURI_KEN_S) FURI_KEN_S")
            SQL.AppendLine(",SUM(FURI_KIN_S) FURI_KIN_S")

            SQL.AppendLine(",MAX(ITAKU_KNAME_T) ITAKU_KNAME_T")
            SQL.AppendLine(",MAX(TORIMATOME_SIT_T) TORIMATOME_SIT_T")
            SQL.AppendLine(",MAX(KESSAI_KBN_T) KESSAI_KBN_T")
            SQL.AppendLine(",MAX(KESSAI_PATN_T) KESSAI_PATN_T")

            SQL.AppendLine(",MAX(KIN_KNAME_N) KIN_KNAME_N")
            SQL.AppendLine(",MAX(SIT_KNAME_N) SIT_KNAME_N")
            SQL.AppendLine(",MAX(BAITAI_CODE_S) BAITAI_CODE_S")
            SQL.AppendLine(" FROM S_TORIMAST")
            SQL.AppendLine("     ,S_SCHMAST")
            SQL.AppendLine("     ,TENMAST")

            SQL.AppendLine(" WHERE KESSAI_YDATE_S = " & SQ(ParaHassinDate))
            SQL.AppendLine(" AND KESSAI_FLG_S = '0'")
            SQL.AppendLine(" AND HASSIN_FLG_S = '1'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            SQL.AppendLine(" AND FURI_KIN_S > 0")
            SQL.AppendLine(" AND KESSAI_KBN_T <> '99'")
            SQL.AppendLine(" AND TORIS_CODE_S   = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S   = TORIF_CODE_T")
            SQL.AppendLine(" AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
            SQL.AppendLine(" AND TORIMATOME_SIT_T = SIT_NO_N(+)")
            SQL.AppendLine(" GROUP BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")
            SQL.AppendLine(" ORDER BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")

            Dim Key As KKeyInfo = Nothing
            Dim test As String = SQL.ToString

            If OraReader.DataReader(SQL) = True Then

                Dim lstKessaiData As New List(Of CAstFormKes.ClsFormKes.KessaiData)

                ' キー初期化
                Key.Init()

                ' 本部店名取得
                HONBU_KNAME = GetTenmast()

                ' 最初のキー設定
                Call Key.SetOraDataKessai(OraReader)

                keskey.RecordNo = haskey.RecordNo

                Do While OraReader.EOF = False
                    lstKessaiData.Clear()

                    ' 資金決済データ取得処理(総合振込用)
                    If fn_GetKessaiData(Key, lstKessaiData) = False Then
                        Return -1
                    End If

                    If Not (lstKessaiData Is Nothing OrElse lstKessaiData.Count = 0) Then
                        ' 取得した資金決済データを基に、総振決済マスタ登録を行う
                        For i As Integer = 0 To lstKessaiData.Count - 1
                            Dim KData As CAstFormKes.ClsFormKes.KessaiData = lstKessaiData.Item(i)

                            ' 総振決済マスタの登録処理
                            If InsertKessaiMast(Key, KData) = False Then
                                jobMessage = "総振決済マスタ登録失敗 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & _
                                             " 振込日：" & Key.FURI_DATE
                                Return -1
                            End If
                        Next
                    End If

                    ' スケジュールマスタの更新処理 
                    If UpdateSchMast(Key) = False Then
                        jobMessage = "スケジュールマスタ更新失敗 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & _
                                     " 振込日：" & Key.FURI_DATE
                        Return -1
                    End If

                    ' 対象データの次レコードを読込む
                    OraReader.NextRead()

                    If OraReader.EOF = False Then
                        ' キー設定
                        Call Key.SetOraDataKessai(OraReader)
                    End If

                    HassinList.AddRange(lstKessaiData)
                Loop
            End If

            If HassinList.Count = 0 Then
                MainLOG.Write("(資金決済データ格納)", "失敗", "件数０件")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(資金決済データ格納)", "失敗", ex.Message)
            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            MainLOG.Write("(資金決済データ格納)終了", "成功")
        End Try

        Return 0

    End Function


    ' 機能　 ： 資金決済データ取得処理(総合振込用)
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_GetKessaiData(ByRef Key As KKeyInfo, ByRef lstKessaiData As List(Of CAstFormKes.ClsFormKes.KessaiData)) As Boolean

        Dim errFlg As Boolean = False
        Dim errMsg As String = "決済情報に誤りがあります。"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing

        strKEKKA = ""

        Try
            Select Case Key.KESSAI_KBN
                Case "00"
                    ' 為替請求のデータ作成
                    If errFlg = False AndAlso fn_KAWASE_SEIKYU(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                        ' 資金決済データのリスト作成
                        lstKessaiData.Add(KData)
                    Else
                        errFlg = True
                    End If
                Case "99"

                Case Else
                    errFlg = True
                    errMsg &= "(決済区分)"
            End Select

            If errFlg = True Then
                jobMessage = errMsg & " 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振込日：" & Key.FURI_DATE & _
                    " 決済区分：" & Key.KESSAI_KBN
                MainLOG.Write("資金決済データ取得処理(総合振込用)", "失敗", jobMessage)
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("資金決済データ取得処理(総合振込用)", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 資金決済データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_KessaiData(ByRef Key As KKeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean

        Try
            ' 通番のカウントアップ
            keskey.RecordNo += 1

        Catch ex As Exception
            MainLOG.Write("資金決済データ作成", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 為替請求データ作成処理
    '
    ' 引数　 ：
    '
    ' 戻り値 ： 0 - 正常，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_KAWASE_SEIKYU(ByRef key As KKeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseSeikyuInFmt As New CAstFormKes.ClsFormSikinFuri.T_48600
        Dim strKINFUKKI_FUGOU As String = ""    ' 金額複記符号

        Try
            ' 初期化
            KawaseSeikyuInFmt.Init()

            ' 金額複記符号の取得
            If fn_FUGO_SETTEI(CASTCommon.CADec(key.FURI_KIN).ToString("#,##0"), strKINFUKKI_FUGOU) = False Then
                MainLOG.Write("為替請求データ作成", "失敗", "複記符号設定処理エラー。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE)
                Return -1
            End If

            'データ設定
            With KawaseSeikyuInFmt
                .TORIATUKAI = ParaHassinDate
                .SYUMOKU = "4701"                                   ' 種目コード
                .JUSIN_TEN = "ﾟ " & key.SIT_KNAME                   ' 受信店名
                .FUKA_CODE = "000"                                  ' 付加コード
                .HASSIN_TEN = "ﾟ " & HONBU_KNAME                    ' 発信店名
                .KINGAKU = key.FURI_KIN.ToString.PadLeft(10)        ' 金額
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = " "                                   ' 決済回数
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.PadRight(15, " "c) ' 金額複記符号
                .BANGOU = ""                                        ' 番号
                .SIKIN_JIYUU1 = "ｲﾗｲﾆﾝ" & key.ITAKU_KNAME.Trim & "ﾌﾞﾝ"  ' 資金付替理由
                .SIKIN_JIYUU2 = key.FURI_KEN.Trim & "ｹﾝ"            ' 資金付替理由２
                .BIKOU1 = ""                                        ' 備考１
                .BIKOU2 = ""                                        ' 備考２
                .SYOKAI_NO = ""                                     ' 照会番号
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = KawaseSeikyuInFmt.Data
            KData.OpeCode = String.Concat(KawaseSeikyuInFmt.KAMOKU_CODE, KawaseSeikyuInFmt.OPE_CODE)
            KData.TorimatomeSit = key.TORIMATOME_SIT

        Catch ex As Exception
            MainLOG.Write("為替請求データ作成", "失敗", ex.Message)
            Return -1
        End Try

        Return 0

    End Function

    ' 機能　 ： 総振決済マスタ登録
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertKessaiMast(ByVal Key As KKeyInfo, ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("INSERT INTO S_KESSAIMAST(")
            SQL.AppendLine(" SYORI_DATE_KR")
            SQL.AppendLine(",TIME_STAMP_KR")
            SQL.AppendLine(",KAIJI_KR")
            SQL.AppendLine(",RECORD_NO_KR")
            SQL.AppendLine(",FILE_NAME_KR")
            SQL.AppendLine(",TORIS_CODE_KR")
            SQL.AppendLine(",TORIF_CODE_KR")
            SQL.AppendLine(",FURI_DATE_KR")
            SQL.AppendLine(",MOTIKOMI_SEQ_KR")
            SQL.AppendLine(",KAMOKU_CODE_KR")
            SQL.AppendLine(",OPE_CODE_KR")
            SQL.AppendLine(",DENBUN_ALL_KR")
            SQL.AppendLine(",ERR_CODE_KR")
            SQL.AppendLine(",ERR_MSG_KR")
            SQL.AppendLine(",SAKUSEI_DATE_KR")
            SQL.AppendLine(",KOUSIN_DATE_KR")
            SQL.AppendLine(") VALUES (")
            SQL.AppendLine(" " & SQ(strDate))                                   ' 処理日
            SQL.AppendLine("," & SQ(String.Concat(strDate, strTime)))           ' タイムスタンプ
            SQL.AppendLine("," & SQ(keskey.Kaiji))                              ' 回次
            SQL.AppendLine("," & SQ(keskey.RecordNo))                           ' 通番
            SQL.AppendLine("," & SQ(ini_info.RIENTA_FILENAME))                  ' リエンタファイル名
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' 取引先主コード
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' 取引先副コード
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' 振込日
            SQL.AppendLine(",1")                                                ' 持込SEQ
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(0, 2)))             ' 科目コード
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(2, 3)))             ' オペコード
            SQL.AppendLine("," & SQ(KData.record320))                           ' 個別データ
            SQL.AppendLine("," & SQ(""))                                        ' 結果コード
            SQL.AppendLine("," & SQ(""))                                        ' エラーメッセージ
            SQL.AppendLine("," & SQ(strDate))                                   ' 作成日
            SQL.AppendLine("," & SQ("00000000"))                                ' 更新日
            SQL.AppendLine(")")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(総振決済マスタ登録)", "失敗", ex.Message)
            Return False
        Finally
        End Try

        Return True

    End Function

    ' 機能　 ： スケジュールマスタ更新
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateSchMast(ByVal key As KKeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("UPDATE S_SCHMAST")
            SQL.AppendLine(" SET")
            SQL.AppendLine(" KESSAI_FLG_S = '1'")
            SQL.AppendLine(",KESSAI_DATE_S = " & SQ(ParaHassinDate))
            SQL.AppendLine(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))
            SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))
            SQL.AppendLine("   AND FURI_KIN_S > 0")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & _
              " 振込日：" & key.FURI_DATE & " " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function GetTenmast() As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append("SELECT SIT_KNAME_N FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(ini_info.JIKINKO_CODE))
            SQL.Append("   AND SIT_NO_N = " & SQ(ini_info.HONBU_CODE))
            If OraReader.DataReader(SQL) = True Then
                Return OraReader.GetString("SIT_KNAME_N")
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

#End Region

    ' 機能　 ： 複記符号設定処理
    '
    ' 引数　 ： astrKEY1:変換前金額（カンマ編集済み）
    '           astrKEY2
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： パラメタでわたされた金額をもとに１５ケタの複記符号を返す
    '
    Private Function fn_FUGO_SETTEI(ByVal astrKEY1 As String, ByRef astrKEY2 As String) As Boolean
        Dim intCount As Integer     '文字数
        Dim strASSYUKU As String    '圧縮
        Dim strFUGO(14) As String   '符号
        Dim I As Integer

        Try
            astrKEY2 = ""
            strASSYUKU = "Y"

            For intCount = 0 To astrKEY1.Length - 1

                strFUGO(intCount) = " "

                Select Case astrKEY1.Substring(intCount, 1)
                    Case "0"
                        If strASSYUKU = "Y" Then
                            strFUGO(intCount) = " "
                        Else
                            strFUGO(intCount) = "ﾄ"
                        End If
                    Case "1"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾋ"
                    Case "2"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾌ"
                    Case "3"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾐ"
                    Case "4"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾖ"
                    Case "5"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ｲ"
                    Case "6"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾙ"
                    Case "7"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾅ"
                    Case "8"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾔ"
                    Case "9"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ｺ"
                    Case ","
                        strFUGO(intCount) = " "
                End Select
            Next

            For I = 0 To strFUGO.Length - 1
                astrKEY2 = astrKEY2 & strFUGO(I)
            Next

            astrKEY2 = astrKEY2.Trim

        Catch ex As Exception
            MainLOG.Write("複記符号設定処理", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 振込発信マスタ登録
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertHassinMast(ByVal Key As FKeyInfo, ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("INSERT INTO HASSINMAST(")
            SQL.AppendLine(" SYORI_DATE_FH")
            SQL.AppendLine(",TIME_STAMP_FH")
            SQL.AppendLine(",KAIJI_FH")
            SQL.AppendLine(",RECORD_NO_FH")
            SQL.AppendLine(",FILE_NAME_FH")
            SQL.AppendLine(",TORIS_CODE_FH")
            SQL.AppendLine(",TORIF_CODE_FH")
            SQL.AppendLine(",FURI_DATE_FH")
            SQL.AppendLine(",MOTIKOMI_SEQ_FH")
            SQL.AppendLine(",KAMOKU_CODE_FH")
            SQL.AppendLine(",OPE_CODE_FH")
            SQL.AppendLine(",DENBUN_ALL_FH")
            SQL.AppendLine(",ERR_CODE_FH")
            SQL.AppendLine(",ERR_MSG_FH")
            SQL.AppendLine(",SAKUSEI_DATE_FH")
            SQL.AppendLine(",KOUSIN_DATE_FH")
            SQL.AppendLine(") VALUES (")
            SQL.AppendLine(" " & SQ(strDate))                                   ' 処理日
            SQL.AppendLine("," & SQ(String.Concat(strDate, strTime)))           ' タイムスタンプ
            SQL.AppendLine("," & SQ(haskey.Kaiji))                              ' 回次
            SQL.AppendLine("," & SQ(haskey.RecordNo))                           ' 通番
            SQL.AppendLine("," & SQ(ini_info.RIENTA_FILENAME))                  ' リエンタファイル名
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' 取引先主コード
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' 取引先副コード
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' 振込日
            SQL.AppendLine("," & SQ(Key.MOTIKOMI_SEQ))                          ' 持込SEQ
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(0, 2)))             ' 科目コード
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(2, 3)))             ' オペコード
            SQL.AppendLine("," & SQ(KData.record320))                           ' 個別データ
            SQL.AppendLine("," & SQ(""))                                        ' 結果コード
            SQL.AppendLine("," & SQ(""))                                        ' エラーメッセージ
            SQL.AppendLine("," & SQ(strDate))                                   ' 作成日
            SQL.AppendLine("," & SQ("00000000"))                                ' 更新日
            SQL.AppendLine(")")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(振込発信マスタ登録)", "失敗", ex.Message)
            Return False
        Finally
        End Try

        Return True

    End Function

    ' 機能　 ： スケジュールマスタ更新
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateSchMast(ByVal key As FKeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine(" UPDATE S_SCHMAST SET")
            SQL.AppendLine(" HASSIN_DATE_S = " & SQ(strDate))
            SQL.AppendLine(",HASSIN_FLG_S = '1'")
            SQL.AppendLine(",HASSIN_TIME_STAMP_S = " & SQ(strDate & strTime))
            Select Case Me.GetKessaiPatn(key)
                Case "0", "1"           '資金確保不要（為替自動発信対象外）、資金確保不要（為替自動発信対象）

                Case "2", "3"           '手数料別出金、手数料合算
                    SQL.AppendLine(",KAKUHO_FLG_S = '1'")
                    SQL.AppendLine(",KAKUHO_DATE_S = " & SQ(ParaHassinDate))
                    SQL.AppendLine(",KAKUHO_TIME_STAMP_S = " & SQ(strDate & strTime))
                    SQL.AppendLine(",TESUUTYO_FLG_S = '1'")
                    SQL.AppendLine(",TESUU_DATE_S = " & SQ(ParaHassinDate))
                    SQL.AppendLine(",TESUU_TIME_STAMP_S = " & SQ(strDate & strTime))

                Case "4", "5"           '手数料後収、手数料別収
                    SQL.AppendLine(",KAKUHO_FLG_S = '1'")
                    SQL.AppendLine(",KAKUHO_DATE_S = " & SQ(ParaHassinDate))
                    SQL.AppendLine(",KAKUHO_TIME_STAMP_S = " & SQ(strDate & strTime))

            End Select

            SQL.AppendLine(",FURI_KEN_S    = " & key.TotalKen)
            SQL.AppendLine(",FURI_KIN_S    = " & key.TotalKin)
            '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
            SQL.AppendLine(",TESUU_KIN_S   = " & key.TesuuKin)
            '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END
            SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))
            SQL.AppendLine("   AND MOTIKOMI_SEQ_S = " & SQ(key.MOTIKOMI_SEQ))

            If MainDB.ExecuteNonQuery(SQL) = 0 Then
                MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & _
                              " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE & " 持込回数：" & key.MOTIKOMI_SEQ)
            Else
                MainLOG.Write("スケジュールマスタ更新", "成功", "取引先主コード：" & key.TORIS_CODE & _
                              " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE & " 持込回数：" & key.MOTIKOMI_SEQ)
            End If

        Catch ex As Exception
            MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & _
                          " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE & " 持込回数：" & key.MOTIKOMI_SEQ & " " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function GetKaiji() As Boolean

        Dim SQL As New StringBuilder()
        Dim OraReader As MyOracleReader = Nothing

        Try
            MainLOG.Write("(回次取得)開始", "成功", "")

            SQL.Append("SELECT NVL(MAX(KAIJI_SR),0) AS MAX_KAIJI FROM S_I_RENKEIMAST")
            SQL.Append(" WHERE SAKUSEI_DATE_SR = " & SQ(strDate))

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                haskey.Kaiji = OraReader.GetInt("MAX_KAIJI") + 1
                keskey.Kaiji = OraReader.GetInt("MAX_KAIJI") + 1
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("(回次取得)", "失敗", ex.ToString)
            Return False
        Finally
            MainLOG.Write("(回次取得)終了", "成功", "")
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return True

    End Function


    Private Function GetTENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(MainDB)

        Try
            KIN_NNAME = ""
            SIT_NNAME = ""
            KIN_KNAME = ""
            SIT_KNAME = ""

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & KIN_NO.Trim & "' AND SIT_NO_N = '" & SIT_NO.Trim & "'")

            If orareader.DataReader(sql) = True Then
                KIN_NNAME = orareader.GetString("KIN_NNAME_N")
                SIT_NNAME = orareader.GetString("SIT_NNAME_N")
                KIN_KNAME = orareader.GetString("KIN_KNAME_N")
                SIT_KNAME = orareader.GetString("SIT_KNAME_N")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

    '振込発信実行待ちのフラグをすべて元に戻す
    Public Function ReturnFlg() As Integer
        Dim SQL As String
        Dim Ret As Integer = 0
        '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- START
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- END

        Try
            '念のため、一端クローズ
            'INC-No.ME-0276 Not追加
            If Not MainDB Is Nothing Then MainDB.Close()
            MainDB = New CASTCommon.MyOracle

            '2016/01/11 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- START
            'データ作成対象のスケジュール抽出条件に合わせる
            SQL = "SELECT "
            SQL &= " TORIS_CODE_S"
            SQL &= ",TORIF_CODE_S"
            SQL &= ",FURI_DATE_S"
            SQL &= ",MOTIKOMI_SEQ_S"
            SQL &= " FROM S_TORIMAST, S_SCHMAST"
            SQL &= " WHERE TORIS_CODE_T = TORIS_CODE_S"
            SQL &= " AND TORIF_CODE_T = TORIF_CODE_S"
            SQL &= " AND FURI_DATE_S >= '" & ParaHassinDate & "'"
            ' 総合振込
            SQL &= " AND ((SYUBETU_T = '21'  AND FURI_DATE_S BETWEEN '" & ParaHassinDate & "' AND '" & ParaSouDate & "')"
            ' 給与振込、賞与振込
            SQL &= "  OR  (SYUBETU_T <> '21' AND FURI_DATE_S BETWEEN '" & ParaHassinDate & "' AND '" & ParaKyuDate & "'))"
            SQL &= " AND TOUROKU_FLG_S = '1'"
            SQL &= " AND HASSIN_FLG_S = '2'"
            SQL &= " AND TYUUDAN_FLG_S = '0'"

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    SQL = "UPDATE S_SCHMAST SET"
                    SQL &= " HASSIN_FLG_S = '0'"
                    SQL &= " WHERE TORIS_CODE_S = '" & OraReader.GetString("TORIS_CODE_S") & "'"
                    SQL &= " AND TORIF_CODE_S = '" & OraReader.GetString("TORIF_CODE_S") & "'"
                    SQL &= " AND FURI_DATE_S = '" & OraReader.GetString("FURI_DATE_S") & "'"
                    SQL &= " AND MOTIKOMI_SEQ_S = " & OraReader.GetString("MOTIKOMI_SEQ_S")

                    Ret += MainDB.ExecuteNonQuery(SQL)

                    OraReader.NextRead()
                End While
            End If

            OraReader.Close()
            OraReader = Nothing

            'SQL = "UPDATE S_SCHMAST SET"
            'SQL &= " HASSIN_FLG_S = '0'"
            'SQL &= " WHERE HASSIN_YDATE_S = '" & ParaHassinDate & "'"
            ''2012/04/02 saitou 標準修正 MODIFY ---------------------------------------->>>>
            'SQL &= "   AND HASSIN_FLG_S = '2'"
            ''SQL &= "   AND KESSAI_FLG_S = '2'"
            ''2012/04/02 saitou 標準修正 MODIFY ----------------------------------------<<<<

            'Ret = MainDB.ExecuteNonQuery(SQL)
            '2016/01/11 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- END
            MainLOG.Write("発信待ち取消", "成功", Ret & "件")

            MainDB.Commit()
            Return 0
        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write("発信待ち取消", "失敗", ex.ToString)
            Return -1
        Finally
            '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- START
            If Not OraReader Is Nothing Then OraReader.Close()
            '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- END
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function

    Private Function PrintKawaseFurikomiMeisai(ByVal ExeRepo As CAstReports.ClsExecute, ByVal ReportID As String, ByVal ReportName As String) As Boolean
        Dim iRet As Integer
        Dim bRet As Boolean = True

        Dim PrnMeisai As New ClsPrnKawaseFurikomiMeisai(ReportID, ReportName)

        PrnMeisai.OraDB = MainDB
        PrnMeisai.CreateCsvFile()

        '明細行出力
        iRet = PrnMeisai.OutputCSVKekka(HassinList, ini_info.JIKINKO_CODE, ParaHassinDate, strDate, strTime)

        If iRet <> 0 Then
            bRet = False
            MainLOG.Write(ReportName & "出力", "失敗", ReportName & "ＣＳＶ出力に失敗しました。")
        End If

        If Not PrnMeisai Is Nothing And iRet = 0 Then
            PrnMeisai.CloseCsv()

            Select Case ReportID
                Case "KFSP010"
                    '金融機関コード・支店コード・振込日・取引先主コード・取引先副コード・レコード番号順でソート
                    PrnMeisai.SortFile("14.4sjia 15.3sjia 3.8sjia 4.10sjia 5.2sjia 0.6sjia")

                    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
                    If ini_info.RSV2_S_KAWASE_HONSITEN = "0" Then
                        MainLOG.Write(ReportName & "印刷", "終了", "印刷不要")
                        Return bRet
                    End If
                    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END

                Case "KFSP011"
                    '振込日・取引先主コード・取引先副コード・レコード番号順でソート
                    PrnMeisai.SortFile("3.8sjia 4.10sjia 5.2sjia 0.6sjia")

                    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
                    If ini_info.RSV2_S_KAWASE_TAKOU = "0" Then
                        MainLOG.Write(ReportName & "印刷", "終了", "印刷不要")
                        Return bRet
                    End If
                    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END
                Case "KFSP012"

                    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
                    If ini_info.RSV2_S_KAWASE_LOGGING = "0" Then
                        MainLOG.Write(ReportName & "印刷", "終了", "印刷不要")
                        Return bRet
                    End If
                    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END

            End Select

            '印刷バッチ呼び出し
            Dim param As String = ""

            'パラメータ設定：ログイン名、ＣＳＶファイル名
            param = MainLOG.UserID & "," & PrnMeisai.FileName

            iRet = ExeRepo.ExecReport(ReportID & ".EXE", param)

            If iRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case iRet
                    Case -1
                        jobMessage = ReportName & "印刷対象０件。"
                    Case Else
                        jobMessage = ReportName & "印刷失敗。エラーコード：" & iRet
                End Select

                MainLOG.Write(ReportName & "印刷", "失敗", jobMessage)
                bRet = False
            End If

        End If

        Return bRet
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' 振込手数料を計算します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/14 標準版 消費税対応</remarks>
    Private Function CalcFurikomiTesuu(ByVal Key As FKeyInfo) As Boolean
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        ' 振込日
        Dim dtFuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

        ' 振込日の１営業日前, ２営業日前
        Dim Zen1Day As String = CASTCommon.GetEigyobi(dtFuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(dtFuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

        Dim SQL As New StringBuilder

        Try
            '適用種別の判定
            Dim strTekiyoSyubetu As String = String.Empty
            Select Case ParaHassinDate
                Case Key.FURI_DATE
                    '振込
                    strTekiyoSyubetu = "10"
                Case Zen1Day
                    '先振
                    strTekiyoSyubetu = "11"
                Case Is <= Zen2Day
                    '２営業日以前
                    Select Case Key.SYUBETU
                        Case "21"
                            '先振
                            strTekiyoSyubetu = "11"
                        Case "11", "12"
                            '給振
                            strTekiyoSyubetu = "12"
                    End Select
            End Select

            '振込手数料マスタの取得
            If Me.SetFuriTesuu(Me.TAX.ZEIRITSU_ID, strTekiyoSyubetu) = False Then
                Return False
            End If

            '明細マスタを取得
            With SQL
                .Append("select * ")
                .Append(" from S_MEIMAST, S_TORIMAST")
                .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
                .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
                .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
                .Append(" and MOTIKOMI_SEQ_K = " & Key.MOTIKOMI_SEQ)
                .Append(" and DATA_KBN_K = '2'")
                .Append(" and FURIKETU_CODE_K = 0")
                .Append(" and TORIS_CODE_K = TORIS_CODE_T")
                .Append(" and TORIF_CODE_K = TORIF_CODE_T")
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Dim intTesuuKin As Integer = 0

                    '振込手数料を計算する
                    If oraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                        '振込手数料基準IDが設定されていない場合は振込手数料0円とする
                        intTesuuKin = 0
                    Else
                        '振込手数料基準IDが設定されている場合は、その基準IDに紐付く手数料を計算する
                        If Me.GetFurikomiTesuu(oraReader, Key, oraReader.GetString("TESUU_TABLE_ID_T"), intTesuuKin) = False Then
                            Return False
                        End If
                    End If

                    '明細マスタを更新
                    If Me.UpdTesuuKin(oraReader, intTesuuKin) = False Then
                        Return False
                    End If

                    oraReader.NextRead()
                End While
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("振込手数料計算", "失敗", ex.Message)
            jobMessage = "振込手数料計算 失敗 例外が発生しました。"
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 振込手数料マスタを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/27 標準版 消費税対応</remarks>
    Private Function SetFuriTesuu(ByVal TAX_ID As String, _
                                  ByVal SYUBETU As String) As Boolean

        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            Me.htFuriTesuuID = New Hashtable

            '--------------------------------------------------
            '振込手数料マスタから振込手数料を取得する
            '--------------------------------------------------
            With SQL
                .Append("select * from TESUUMAST, TAXMAST")
                .Append(" where TAX_ID_C = TAX_ID_Z")
                .Append(" and TAX_ID_C = " & SQ(TAX_ID))
                .Append(" and SYUBETU_C = " & SQ(SYUBETU))
                .Append(" and FSYORI_KBN_C = '3'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Dim tesuu As strcFuriTesuuInfo = New strcFuriTesuuInfo
                    tesuu.TESUU_A1 = oraReader.GetString("TESUU_A1_C")
                    tesuu.TESUU_A2 = oraReader.GetString("TESUU_A2_C")
                    tesuu.TESUU_A3 = oraReader.GetString("TESUU_A3_C")
                    tesuu.TESUU_B1 = oraReader.GetString("TESUU_B1_C")
                    tesuu.TESUU_B2 = oraReader.GetString("TESUU_B2_C")
                    tesuu.TESUU_B3 = oraReader.GetString("TESUU_B3_C")
                    tesuu.TESUU_C1 = oraReader.GetString("TESUU_C1_C")
                    tesuu.TESUU_C2 = oraReader.GetString("TESUU_C2_C")
                    tesuu.TESUU_C3 = oraReader.GetString("TESUU_C3_C")


                    '振込手数料基準IDをキーにハッシュテーブルに格納する
                    htFuriTesuuID.Add(oraReader.GetString("TESUU_TABLE_ID_C"), tesuu)

                    oraReader.NextRead()
                End While
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("振込手数料マスタ取得", "失敗", ex.Message)
            jobMessage = "振込手数料マスタ取得 失敗 例外が発生しました。"
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 振込金額に対する振込手数料を計算します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <param name="Key"></param>
    ''' <param name="strTesuuTableId"></param>
    ''' <param name="intTesuuKin"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/14 標準版 消費税対応</remarks>
    Private Function GetFurikomiTesuu(ByVal oraReader As CASTCommon.MyOracleReader, _
                                      ByVal Key As FKeyInfo, _
                                      ByVal strTesuuTableId As String, _
                                      ByRef intTesuuKin As Integer) As Boolean

        intTesuuKin = 0
        Dim tesuu As strcFuriTesuuInfo

        '振込手数料基準IDに紐付く手数料を設定
        If htFuriTesuuID.ContainsKey(strTesuuTableId) = True Then
            tesuu = New strcFuriTesuuInfo
            tesuu = DirectCast(htFuriTesuuID.Item(strTesuuTableId), strcFuriTesuuInfo)
        Else
            '振込手数料基準IDに紐付く手数料が設定されていない場合
            '2014/03/02 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
            '振込手数料=0にせず、異常終了させる
            MainLOG.Write("振込手数料計算", "失敗", "振込手数料マスタ設定なし 手数料ID：" & strTesuuTableId)
            Return False
            ''そのまま0を返す
            'intTesuuKin = 0
            'Return True
            '2014/03/02 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
        End If

        Try
            If oraReader.GetInt("FURIKETU_CODE_K") = 0 Then
                '2013/12/27 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                If oraReader.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
                    '振込金融機関が自金庫の場合
                    If oraReader.GetString("KEIYAKU_SIT_K") = Key.TSIT_NO Then
                        '振込支店がとりまとめ店と一致する場合、自店内
                        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI1 Then
                            intTesuuKin = CInt(tesuu.TESUU_A1)
                        ElseIf Me.TAX.INSHIZEI1 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI2 Then
                            intTesuuKin = CInt(tesuu.TESUU_A2)
                        ElseIf Me.TAX.INSHIZEI2 <= oraReader.GetInt64("FURIKIN_K") Then
                            intTesuuKin = CInt(tesuu.TESUU_A3)
                        End If
                    Else
                        '振込支店がとりまとめ店と一致しない場合、本支店
                        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI1 Then
                            intTesuuKin = CInt(tesuu.TESUU_B1)
                        ElseIf Me.TAX.INSHIZEI1 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI2 Then
                            intTesuuKin = CInt(tesuu.TESUU_B2)
                        ElseIf Me.TAX.INSHIZEI2 <= oraReader.GetInt64("FURIKIN_K") Then
                            intTesuuKin = CInt(tesuu.TESUU_B3)
                        End If
                    End If
                Else
                    '振込金融機関が他行の場合
                    If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI1 Then
                        intTesuuKin = CInt(tesuu.TESUU_C1)
                    ElseIf Me.TAX.INSHIZEI1 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI2 Then
                        intTesuuKin = CInt(tesuu.TESUU_C2)
                    ElseIf Me.TAX.INSHIZEI2 <= oraReader.GetInt64("FURIKIN_K") Then
                        intTesuuKin = CInt(tesuu.TESUU_C3)
                    End If
                End If

                'If oraReader.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
                '    '振込金融機関が自金庫の場合
                '    If oraReader.GetString("KEIYAKU_SIT_K") = Key.TSIT_NO Then
                '        '振込支店がとりまとめ店と一致する場合、自店内
                '        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 10000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_A1)
                '        ElseIf 10000 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 30000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_A2)
                '        ElseIf 30000 <= oraReader.GetInt64("FURIKIN_K") Then
                '            intTesuuKin = CInt(tesuu.TESUU_A3)
                '        End If
                '    Else
                '        '振込支店がとりまとめ店と一致しない場合、本支店
                '        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 10000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_B1)
                '        ElseIf 10000 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 30000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_B2)
                '        ElseIf 30000 <= oraReader.GetInt64("FURIKIN_K") Then
                '            intTesuuKin = CInt(tesuu.TESUU_B3)
                '        End If
                '    End If
                'Else
                '    '振込金融機関が他行の場合
                '    If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 10000 Then
                '        intTesuuKin = CInt(tesuu.TESUU_C1)
                '    ElseIf 10000 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 30000 Then
                '        intTesuuKin = CInt(tesuu.TESUU_C2)
                '    ElseIf 30000 <= oraReader.GetInt64("FURIKIN_K") Then
                '        intTesuuKin = CInt(tesuu.TESUU_C3)
                '    End If
                'End If
                '2013/12/27 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("振込手数料計算", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 明細マスタの振込手数料を更新します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <param name="intTesuuKin"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/14 標準版 消費税対応</remarks>
    Private Function UpdTesuuKin(ByVal oraReader As CASTCommon.MyOracleReader, _
                                 ByVal intTesuuKin As Integer) As Boolean
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("update S_MEIMAST set ")
                .Append(" TESUU_KIN_K = " & intTesuuKin.ToString)
                .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_K")))
                .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_K")))
                .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_K")))
                .Append(" and MOTIKOMI_SEQ_K = " & oraReader.GetString("MOTIKOMI_SEQ_K"))
                .Append(" and RECORD_NO_K = " & oraReader.GetString("RECORD_NO_K"))
            End With

            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                MainLOG.Write("振込手数料更新", "失敗", MainDB.Message)
                jobMessage = "振込手数料更新 失敗 ログを参照して下さい。"
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("振込手数料更新", "失敗", ex.Message)
            jobMessage = "振込手数料更新 失敗 例外が発生しました。"
            Return False
        End Try

    End Function


    ''' <summary>
    ''' 総給振依頼人連携データテーブルに挿入します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="KData"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function InsertSIRenkeimast(ByVal Key As FKeyInfo, _
                                        ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim Data As New CAstFormKes.ClsFormSikinFuri.T_48100
        Data.Data = KData.record320

        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("insert into S_I_RENKEIMAST (")
                .Append(" FSYORI_KBN_SR")
                .Append(",TORIS_CODE_SR")
                .Append(",TORIF_CODE_SR")
                .Append(",REC_RENKEI_KBN_SR")
                .Append(",SAKUSEI_DATE_SR")
                .Append(",SAKUSEI_TIME_SR")
                .Append(",KAIJI_SR")
                .Append(",UKETUKE_DATE_SR")
                .Append(",FURI_DATE_SR")
                .Append(",REN_TORIKOMI_DATE_SR")
                .Append(",REN_KEKKA_DATE_SR")
                .Append(",FURI_CODE_SR")
                .Append(",ITAKU_CODE_SR")
                .Append(",CYCLE_NO_SR")
                .Append(",TUKESIT_NO_SR")
                .Append(",TUKEKAMOKU_SR")
                .Append(",TUKEKOUZA_SR")
                .Append(",ITAKU_KNAME_SR")
                .Append(",ITAKU_NNAME_SR")
                .Append(",KYUUYO_KBN_SR")
                .Append(",SYUBETU_KBN_SR")
                .Append(",KESSAI_PATN_SR")
                .Append(",TIEN_KBN_SR")
                .Append(",HASSIN_NO_SR")
                .Append(",KESSAI_NO_SR")
                .Append(",FURIKIN_KEI_SR")
                .Append(",FURIKIN_KEIJ_SR")
                .Append(",FURIKIN_KEIH_SR")
                .Append(",FURIKIN_KEIT_SR")
                .Append(",TESUU_KIN_KEI_SR")
                .Append(",KIN_STS_SR")
                .Append(",SOU_SYUBETU_SR")
                .Append(",TESUUTYO_KBN_SR")
                .Append(",FURI_STS_SR")
                .Append(",TESUU_STS_SR")
                .Append(",KAWASE_STS_SR")
                .Append(",MODOSI_STS_SR")
                .Append(",TUKEKAE_STS_SR")
                .Append(",TEN_STS_SR")
                .Append(",ERR_MESSAGE_CODE_SR")
                .Append(",ERR_MESSAGE_SR")
                .Append(",TIME_STAMP_SR")
                .Append(",RIENTA_FD_SR")
                .Append(",SOUFURI_NO_SR")
                .Append(",HASSIN_DATE_SR")
                .Append(",KESSAI_KANRYO_DATE_SR")
                .Append(",HASSIN_FURI_CODE_SR")
                .Append(",HASSIN_KBN_SR")
                .Append(",TORIKESI_HORYU_KBN_SR")
                .Append(",HASSIN_FURIKIN_KEI_SR")
                .Append(",HASSIN_FURIKIN_KEIJ_SR")
                .Append(",HASSIN_FURIKIN_KEIH_SR")
                .Append(",HASSIN_FURIKIN_KEIT_SR")
                .Append(",HASSIN_TESUU_KIN_KEI_SR")
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- START
                .Append(",UKETUKE_KBN_SR")
                .Append(",KEKKA_HASSO_KBN_SR")
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- END
                .Append(" ) values (")
                .Append(" " & SQ("3"))                          '振替処理区分
                .Append("," & SQ(Key.TORIS_CODE))               '取引先主コード
                .Append("," & SQ(Key.TORIF_CODE))               '取引先副コード
                .Append("," & SQ("1"))                          'レコード連携区分
                .Append("," & SQ(strDate))                      '作成日
                .Append("," & SQ(strTime))                      '作成時刻
                .Append("," & SQ(haskey.Kaiji.ToString.PadLeft(2, "0"c)))                 '作成回次
                .Append("," & SQ(Key.TOUROKU_DATE))             '受付日
                .Append("," & SQ(Key.FURI_DATE))                '振込指定日
                .Append("," & SQ("00000000000000"))             '連携取組日
                .Append("," & SQ("00000000000000"))             '連携結果更新日
                .Append("," & SQ(Data.SYUMOKU))                 '振込種目
                .Append("," & SQ(Key.ITAKU_CODE))               '委託者コード
                .Append("," & Key.MOTIKOMI_SEQ)                 'サイクル番号
                .Append("," & SQ(Key.TUKESIT_NO))               '依頼人店舗コード
                .Append("," & SQ(Key.TUKEKAMOKU))               '依頼人科目コード
                .Append("," & SQ(Key.TUKEKOUZA))                '依頼人口座番号
                .Append("," & SQ(Key.ITAKU_KNAME))              '依頼人名カナ
                .Append("," & SQ(Key.ITAKU_NNAME))              '依頼人名漢字
                Select Case Key.SYUBETU
                    Case "21"
                        .Append("," & SQ("0"))
                    Case Else
                        Select Case Data.SYUMOKU.Substring(0, 2)
                            Case "10", "11"
                                .Append("," & SQ("1"))
                            Case "12"
                                .Append("," & SQ("2"))
                        End Select
                End Select
                Select Case Key.SYUBETU
                    Case "21" : .Append("," & SQ("0"))
                    Case "11" : .Append("," & SQ("1"))
                    Case "12" : .Append("," & SQ("2"))
                End Select

                .Append("," & SQ(Me.GetKessaiPatn(Key)))        '資金確保パターン
                .Append("," & CInt(Me.GetTienKbn(Key)))         '受入遅延区分
                Select Case Data.SYUMOKU.Substring(0, 2)        '発信日日数
                    Case "10" : .Append("," & "0")
                    Case "11" : .Append("," & "1")
                    Case "12" : .Append("," & "2")
                End Select
                Select Case Data.SYUMOKU.Substring(0, 2)        '資金確保日数
                    Case "10" : .Append("," & "0")
                    Case "11" : .Append("," & "1")
                    Case "12" : .Append("," & "2")
                End Select

                .Append("," & Key.FURIKIN_KEI)                  '振込金額合計
                .Append("," & Key.FURIKIN_KEIJ)                 '振込金額合計内訳自店内
                .Append("," & Key.FURIKIN_KEIH)                 '振込金額合計内訳本支店
                .Append("," & Key.FURIKIN_KEIT)                 '振込金額合計内訳他行宛
                .Append("," & Key.TESUU_KIN_KEI)                '手数料金額合計
                .Append("," & "0")                              '処理状況
                .Append("," & "0")                              '総振種別
                Select Case Key.TESUUTYO_KBN                    '手数料徴求区分
                    Case "0" : .Append("," & "1")
                    Case Else : .Append("," & "3")
                End Select

                '振込金額出金状況（初期値要確認）
                Select Case CInt(Me.GetKessaiPatn(Key))
                    Case 0, 1 '資金確保不要
                        .Append("," & "0")
                    Case Else
                        .Append("," & "1")
                End Select

                '手数料出金状況（初期値要確認）
                Select Case CInt(Me.GetKessaiPatn(Key))
                    Case 2, 3 '手数料別出金、合算
                        .Append("," & "1")
                    Case Else
                        .Append("," & "0")
                End Select
                .Append("," & "0")                              '為替発信状況（初期値要確認）
                .Append("," & "0")                              '組戻状況
                .Append("," & "0")                              '別段付替状況
                .Append("," & "0")                              '諸勘定入金状況
                .Append("," & SQ("0000"))                       'エラーコード
                .Append("," & SQ(""))                           'エラー内容
                .Append("," & SQ("00000000000000"))             'リエンタ作成タイムスタンプ
                .Append("," & SQ("0"))                          'リエンタFD作成フラグ
                .Append("," & "0")                              '総合振込通番
                .Append("," & "0")                              '発信日
                .Append("," & "0")                              '資金確保完了日
                .Append("," & "0")                              '発信振込種別
                .Append("," & "0")                              '発信区分
                .Append("," & "0")                              '取消保留区分
                .Append("," & "0")                              '発信時振込金額合計
                .Append("," & "0")                              '発信時振込金額合計内訳自店内
                .Append("," & "0")                              '発信時振込金額合計内訳本支店
                .Append("," & "0")                              '発信時振込金額合計内訳他行宛
                .Append("," & "0")                              '発信時振込手数料合計
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- START
                .Append("," & CInt(Me.GetUketukeKbn(Key)))      '受付形態区分
                .Append("," & "0")                              '振込サービス処理結果要否区分
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- END
                .Append(")")
            End With

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(総給振依頼人連携データテーブル登録)", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' 総給振明細連携データテーブルに挿入します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="KData"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function InsertSMRenkeimast(ByVal Key As FKeyInfo, _
                                        ByVal KData As CAstFormKes.ClsFormKes.KessaiData, _
                                        ByVal oraMei As CASTCommon.MyOracleReader) As Boolean
        Dim Data As New CAstFormKes.ClsFormSikinFuri.T_48100
        Data.Data = KData.record320

        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("insert into S_M_RENKEIMAST (")
                .Append(" FSYORI_KBN_SR")
                .Append(",TORIS_CODE_SR")
                .Append(",TORIF_CODE_SR")
                .Append(",REC_RENKEI_KBN_SR")
                .Append(",KAWASE_STS_SR")
                .Append(",SAKUSEI_DATE_SR")
                .Append(",SAKUSEI_TIME_SR")
                .Append(",KAIJI_SR")
                .Append(",UKETUKE_DATE_SR")
                .Append(",FURI_DATE_SR")
                .Append(",REN_TORIKOMI_DATE_SR")
                .Append(",REN_KEKKA_DATE_SR")
                .Append(",SYUMOKU_SR")
                .Append(",ITAKU_CODE_SR")
                .Append(",CYCLE_NO_SR")
                .Append(",RECORD_SR")
                .Append(",KEIYAKU_KIN_KNAME_SR")
                .Append(",KEIYAKU_KIN_SR")
                .Append(",KEIYAKU_SIT_KNAME_SR")
                .Append(",KEIYAKU_SIT_SR")
                .Append(",FUKA_CODE_SR")
                .Append(",FURIKIN_SR")
                .Append(",TESUU_KIN1_SR")
                .Append(",TESUU_KIN2_SR")
                .Append(",TESUU_KIN3_SR")
                .Append(",KEIYAKU_KAMOKU_SR")
                .Append(",KEIYAKU_KOUZA_SR")
                .Append(",KEIYAKU_KNAME_SR")
                .Append(",BIKOU1_SR")
                .Append(",BIKOU2_SR")
                .Append(",EDI_SR")
                .Append(",TIME_STAMP_SR")
                .Append(",TORIKESI_KBN_SR")
                .Append(",HASSIN_TESUU_KIN_SR")
                .Append(",KAWASE_NO_SR")
                .Append(",ERR_MESSAGE_CODE_SR")
                .Append(",ERR_MESSAGE_SR")
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- START
                .Append(",CC_KBN_SR")
                .Append(",SYAIN_NO_SR")
                .Append(",SYOZOKU_CODE_SR")
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- END
                .Append(" ) values ( ")
                .Append(" " & SQ("3"))                      '振替処理区分
                .Append("," & SQ(Key.TORIS_CODE))           '取引先主コード
                .Append("," & SQ(Key.TORIF_CODE))           '取引先副コード
                .Append("," & SQ("2"))                      'レコード連携区分
                .Append("," & "0")                          '為替発信状況
                .Append("," & SQ(strDate))                  '作成日
                .Append("," & SQ(strTime))                  '作成時刻
                .Append("," & SQ(haskey.Kaiji.ToString.PadLeft(2, "0"c)))             '作成回次
                .Append("," & SQ(Key.TOUROKU_DATE))         '受付日
                .Append("," & SQ(Key.FURI_DATE))            '振込指定日
                .Append("," & SQ("00000000000000"))         '連携取込日
                .Append("," & SQ("00000000000000"))         '連携結果更新日
                .Append("," & SQ(Data.SYUMOKU))             '振込種目
                .Append("," & SQ(Key.ITAKU_CODE))           '依頼人番号
                .Append("," & Key.MOTIKOMI_SEQ)             'サイクル番号
                .Append("," & haskey.RecordNo)              'レコード通番
                Dim KinCd As String = KData.KesKinCode
                Dim SitCd As String = KData.KesSitCode
                Dim KinName As String = String.Empty
                Dim SitName As String = String.Empty
                If GetTenmast(KinCd, SitCd, "", "", KinName, SitName) Then
                    If KinCd.Equals(ini_info.JIKINKO_CODE) = True Then
                        .Append("," & SQ(""))
                    Else
                        .Append("," & SQ(KinName))
                    End If
                    .Append("," & SQ(KinCd))                '受取人金融機関番号
                    .Append("," & SQ(SitName))              '受取人支店名カナ
                    .Append("," & SQ(SitCd))                '受取人支店番号
                Else
                    .Append("," & SQ(""))
                    .Append("," & SQ(""))
                    .Append("," & SQ(""))
                    .Append("," & SQ(""))
                End If
                .Append("," & SQ("000"))                    '受取人付加コード
                .Append("," & CLng(Data.KINGAKU.Trim))      '受取人振込金額
                '2016/01/20 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- START
                Dim FurikomiTesuuKin As Integer = 0

                Select Case Data.SYUMOKU.Substring(0, 2)
                    Case "12"
                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "12", "12")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "給振手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "11", "12")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "先振手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "10", "12")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "振込手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                    Case "11"
                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "12", "11")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "給振手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "11", "11")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "先振手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "10", "11")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "振込手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                    Case "10"
                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "12", "10")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "給振手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "11", "10")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "先振手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "10", "10")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("総給振明細連携データテーブル登録", "失敗", "振込手数料設定失敗")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                End Select

                'Select Case Data.SYUMOKU.Substring(0, 2)
                '    Case "12"
                '        .Append("," & KData.FurikomiTesuukin)
                '        .Append("," & KData.FurikomiTesuukin)
                '        .Append("," & KData.FurikomiTesuukin)
                '    Case "11"
                '        .Append("," & "0")
                '        .Append("," & KData.FurikomiTesuukin)
                '        .Append("," & KData.FurikomiTesuukin)
                '    Case "10"
                '        .Append("," & "0")
                '        .Append("," & "0")
                '        .Append("," & KData.FurikomiTesuukin)
                'End Select
                '2016/01/20 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- END
                .Append("," & SQ(Data.UKETORI_KAMOKU))      '受取人科目コード
                .Append("," & SQ(Data.UKETORI_KOUZA.Trim))  '受取人口座番号
                .Append("," & SQ(Data.UKETORI_NAME))        '受取人名カナ
                .Append("," & SQ(Data.BIKOU1))              '備考1
                .Append("," & SQ(Data.BIKOU2))              '備考2
                .Append("," & SQ(Data.EDI_INFO))            'EDI情報
                .Append("," & SQ("00000000000000"))         'タイムスタンプ
                .Append("," & "0")                          '取消区分
                .Append("," & "0")                          '発信時受取人手数料
                .Append("," & SQ(""))                       '為替通番
                .Append("," & SQ(""))                       'エラーメッセージコード
                .Append("," & SQ(""))                       'エラーメッセージ
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- START
                .Append("," & "0")                          'センタカット対象区分
                .Append("," & SQ(oraMei.GetString("JYUYOUKA_NO_K", False).PadRight(24).Substring(0, 10)))
                .Append("," & SQ(oraMei.GetString("JYUYOUKA_NO_K", False).PadRight(24).Substring(10, 10)))
                ' 2017/04/13 タスク）綾部 ADD 【ME】(SKForce連携テーブル項目追加(標準化)) -------------------- END
                .Append(" )")
            End With

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(総給振明細連携データテーブル登録)", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' 各種振込金額合計を計算します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function CalcFurikinKei(ByRef Key As FKeyInfo) As Boolean
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("select ")
                .Append(" sum(FURIKIN_K) as FURIKIN_KEI")
                .Append(",sum(case when KEIYAKU_KIN_K = " & SQ(Key.TUKEKIN_NO) & " and KEIYAKU_SIT_K = " & SQ(Key.TUKESIT_NO) & " then FURIKIN_K else 0 end) as FURIKIN_KEIJ")
                .Append(",sum(case when KEIYAKU_KIN_K = " & SQ(Key.TUKEKIN_NO) & " and KEIYAKU_SIT_K <> " & SQ(Key.TUKESIT_NO) & " then FURIKIN_K else 0 end) as FURIKIN_KEIH")
                .Append(",sum(case when KEIYAKU_KIN_K <> " & SQ(Key.TUKEKIN_NO) & " then FURIKIN_K else 0 end) as FURIKIN_KEIT")
                .Append(",sum(TESUU_KIN_K) as TESUU_KIN_KEI")
                .Append(" from S_MEIMAST")
                .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
                .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
                .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
                .Append(" and MOTIKOMI_SEQ_K = " & Key.MOTIKOMI_SEQ)
                .Append(" and DATA_KBN_K = '2'")
                .Append(" and FURIKETU_CODE_K = 0")
            End With

            If oraReader.DataReader(SQL) = True Then
                Key.FURIKIN_KEI = oraReader.GetInt64("FURIKIN_KEI")
                Key.FURIKIN_KEIJ = oraReader.GetInt64("FURIKIN_KEIJ")
                Key.FURIKIN_KEIH = oraReader.GetInt64("FURIKIN_KEIH")
                Key.FURIKIN_KEIT = oraReader.GetInt64("FURIKIN_KEIT")
                Key.TESUU_KIN_KEI = oraReader.GetInt64("TESUU_KIN_KEI")
            Else
                Key.FURIKIN_KEI = 0
                Key.FURIKIN_KEIJ = 0
                Key.FURIKIN_KEIH = 0
                Key.FURIKIN_KEIT = 0
                Key.TESUU_KIN_KEI = 0
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("(振込金額計算)", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 資金確保パターンを返します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetKessaiPatn(ByVal Key As FKeyInfo) As String

        ' 振込日
        Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)
        ' 振込日の１営業日前，２営業日前、３営業日前
        Dim Zen1Day As String = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen3Day As String = CASTCommon.GetEigyobi(FuriDate, -3, FmtComm.HolidayList).ToString("yyyyMMdd")

        '2016/01/11 タスク）斎藤 金バッチ対応 UPD ---------------------------------------- START
        Select Case Key.KESSAI_PATN
            Case "1"        '営業店確保
                Return "1"      '資金確保不要(為替自動発信対象)

            Case "0"        '事前確保
                Select Case Key.TESUUTYO_KBN
                    Case "1"        '一括徴求
                        Return "4"      '手数料後収
                    Case "3"        '別途徴求
                        Return "5"      '手数料別収
                    Case Else
                        Return "2"      '手数料別出金
                End Select

            Case Else       '確保対象外
                Return "1"      '資金確保不要(為替自動発信対象)

        End Select

        ''確保前に発信する条件のチェック
        'Dim bHassinBeforeKakuho As Boolean = False
        'Select Case Key.SYUBETU
        '    Case "21"       '振込
        '        If Key.KINSI.Equals("1") = False Then
        '            '禁止でない
        '            If Key.IRAI_DATE <= Zen2Day Then
        '                '依頼日が振込日の2営業日以前
        '                bHassinBeforeKakuho = True
        '            End If
        '        End If

        '    Case "11", "12"     '給与・賞与
        '        If Key.KINSI.Equals("1") = False Then
        '            '禁止でない
        '            If Key.IRAI_DATE <= Zen3Day Then
        '                '依頼日が振込日の3営業日以前
        '                bHassinBeforeKakuho = True
        '            End If
        '        End If
        'End Select

        'Select Case Key.KESSAI_PATN
        '    Case "1"        '営業店確保
        '        If bHassinBeforeKakuho = True Then
        '            '確保前に発信する
        '            Return "1"          '資金確保不要(為替自動発信対象)
        '        Else
        '            '確保前に発信しない
        '            Return "0"          '資金確保不要(為替自動発信対象外)
        '        End If

        '    Case "0"        '事前確保
        '        If bHassinBeforeKakuho = True Then
        '            Select Case Key.TESUUTYO_KBN
        '                Case "1"        '一括徴求
        '                    Return "4"      '手数料後収
        '                Case "3"        '別途徴求
        '                    Return "5"      '手数料別収
        '                Case Else
        '                    Return "2"      '手数料別出金
        '            End Select
        '        Else
        '            Select Case Key.TESUUTYO_KBN
        '                Case "1"        '一括徴求
        '                    Return "4"      '手数料後収
        '                Case "3"        '別途徴求
        '                    Return "5"      '手数料別収
        '                Case Else
        '                    Return "2"      '手数料別出金
        '            End Select
        '        End If

        '    Case Else
        '        Return "2"

        'End Select
        '2016/01/11 タスク）斎藤 金バッチ対応 UPD ---------------------------------------- END

    End Function

    ''' <summary>
    ''' 受入遅延区分を返します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetTienKbn(ByVal Key As FKeyInfo) As String

        '2016/01/11 タスク）斎藤 金バッチ対応 UPD ---------------------------------------- START
        '遅延判定なし
        Return "0"

        'If Key.SYUBETU = "11" OrElse Key.SYUBETU = "12" Then
        '    Dim TienDay As Integer

        '    If sdDelayList.ContainsKey(Key.BAITAI_CODE) = True Then
        '        TienDay = CInt(sdDelayList.Item(Key.BAITAI_CODE))
        '    Else
        '        TienDay = 3
        '    End If

        '    Dim FURI_DATE As Date = CASTCommon.ConvertDate(Key.FURI_DATE)
        '    Dim TienDate As String = CASTCommon.GetEigyobi(FURI_DATE, TienDay * (-1), FmtComm.HolidayList).ToString("yyyyMMdd")

        '    If Key.IRAI_DATE > TienDate Then
        '        Return "1"
        '    Else
        '        Return "0"
        '    End If

        'Else
        '    '総振は遅延判定無し
        '    Return "0"
        'End If
        '2016/01/11 タスク）斎藤 金バッチ対応 UPD ---------------------------------------- END

    End Function

    ''' <summary>
    ''' 総給振明細連携データテーブルの振込手数料を計算します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="KData"></param>
    ''' <param name="Data"></param>
    ''' <param name="GetTesuuSyubetu"></param>
    ''' <param name="HassinSyubetu"></param>
    ''' <returns>正常 - 振込手数料 , 異常 - -1</returns>
    ''' <remarks>2016/01/20 タスク）斎藤 RSV2 added for 金バッチ対応</remarks>
    Private Function GetFurikomiTesuuKinBatch(ByVal Key As FKeyInfo, _
                                              ByVal KData As CAstFormKes.ClsFormKes.KessaiData, _
                                              ByVal Data As CAstFormKes.ClsFormSikinFuri.T_48100, _
                                              ByVal GetTesuuSyubetu As String, _
                                              ByVal HassinSyubetu As String) As Integer

        Try
            Dim intTesuuKin As Integer = 0
            Dim tesuu As TesuuTable

            If Me.ini_info.ZEIKIJUN.Equals("0") = True Then
                '振込日基準

                '税率と課税範囲を取得する
                Me.TAX.GetZeiritsu(Key.FURI_DATE)
                Me.TAX.GetInshizei(Key.FURI_DATE)

                '振込手数料を取得
                If Me.TesuuList.ContainsKey(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu) = True Then
                    tesuu = New TesuuTable
                    tesuu = DirectCast(Me.TesuuList.Item(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu), TesuuTable)
                Else
                    '振込手数料が設定されていない場合
                    MainLOG.Write("振込手数料計算", "失敗", "振込手数料マスタ設定なし 手数料ID：" & Key.TESUU_TABLE_ID)
                    Return -1
                End If

            Else
                '発信日基準
                Dim KijunDate As String = String.Empty

                Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

                Select Case HassinSyubetu
                    Case "12"
                        '給振発信
                        If GetTesuuSyubetu = "12" Then
                            '手数料種別が給振の場合は、基準日に発信日を設定
                            KijunDate = ParaHassinDate
                        ElseIf GetTesuuSyubetu = "11" Then
                            '手数料種別が先振の場合は、基準日に振込日の1営業日前を設定
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
                        Else
                            '手数料種別が振込の場合は、基準日に振込日を設定
                            KijunDate = Key.FURI_DATE
                        End If

                    Case "11"
                        '先振発信
                        If GetTesuuSyubetu = "12" Then
                            '手数料種別が給振の場合は、基準日に振込日の2営業日前を設定
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")
                        ElseIf GetTesuuSyubetu = "11" Then
                            '手数料種別が先振の場合は、基準日に発信日を設定
                            KijunDate = ParaHassinDate
                        Else
                            '手数料種別が振込の場合は、基準日に振込日を設定
                            KijunDate = Key.FURI_DATE
                        End If
                    Case "10"
                        '振込発信
                        If GetTesuuSyubetu = "12" Then
                            '手数料種別が給振の場合は、基準日に振込日の2営業日前を設定
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")
                        ElseIf GetTesuuSyubetu = "11" Then
                            '手数料種別が先振の場合は、基準日に振込日の1営業日前を設定
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
                        Else
                            '手数料種別が振込の場合は、基準日に発信日を設定
                            KijunDate = ParaHassinDate
                        End If
                End Select

                '税率と課税範囲を取得する
                Me.TAX.GetZeiritsu(KijunDate)
                Me.TAX.GetInshizei(KijunDate)

                '振込手数料を取得
                If Me.TesuuList.ContainsKey(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu) = True Then
                    tesuu = New TesuuTable
                    tesuu = DirectCast(Me.TesuuList.Item(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu), TesuuTable)
                Else
                    '振込手数料が設定されていない場合
                    MainLOG.Write("振込手数料計算", "失敗", "振込手数料マスタ設定なし 手数料ID：" & Key.TESUU_TABLE_ID)
                    Return -1
                End If
            End If

            '振込手数料の決定
            If KData.KesKinCode = Me.ini_info.JIKINKO_CODE Then
                If KData.KesSitCode = Key.TSIT_NO Then
                    If 0 < CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI1 Then
                        intTesuuKin = CInt(tesuu.TESUU_A1)
                    ElseIf Me.TAX.INSHIZEI1 <= CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI2 Then
                        intTesuuKin = CInt(tesuu.TESUU_A2)
                    ElseIf Me.TAX.INSHIZEI2 <= CLng(Data.KINGAKU) Then
                        intTesuuKin = CInt(tesuu.TESUU_A3)
                    End If
                Else
                    If 0 < CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI1 Then
                        intTesuuKin = CInt(tesuu.TESUU_B1)
                    ElseIf Me.TAX.INSHIZEI1 <= CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI2 Then
                        intTesuuKin = CInt(tesuu.TESUU_B2)
                    ElseIf Me.TAX.INSHIZEI2 <= CLng(Data.KINGAKU) Then
                        intTesuuKin = CInt(tesuu.TESUU_B3)
                    End If
                End If

            Else
                If 0 < CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI1 Then
                    intTesuuKin = CInt(tesuu.TESUU_C1)
                ElseIf Me.TAX.INSHIZEI1 <= CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI2 Then
                    intTesuuKin = CInt(tesuu.TESUU_C2)
                ElseIf Me.TAX.INSHIZEI2 <= CLng(Data.KINGAKU) Then
                    intTesuuKin = CInt(tesuu.TESUU_C3)
                End If
            End If

            Return intTesuuKin

        Catch ex As Exception
            MainLOG.Write("振込手数料計算", "失敗", ex.Message)
            Return -1

        End Try
    End Function

    ''' <summary>
    ''' 受付形態区分を返します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetUketukeKbn(ByVal Key As FKeyInfo) As String

        Try
            Dim ReturnCode As String = "0"
            Dim TextFile As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "受付形態区分_変換テーブル.TXT")
            ReturnCode = CASTCommon.GetText_CodeToName(TextFile, Key.BAITAI_CODE)

            Select Case ReturnCode.Trim
                Case ""
                    Return "0"
                Case Else
                    Return ReturnCode
            End Select
        Catch ex As Exception
            Return "0"
        End Try

    End Function

End Class
