Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections
Imports System.Diagnostics
Imports CASTCommon
Imports Microsoft.VisualBasic


'資金決済データ作成処理
Public Class ClsKessaiDataCreate

    Public MainLOG As New CASTCommon.BatchLOG("KFK010", "資金決済リエンタ作成")

    Dim MainDB As CASTCommon.MyOracle

    Private strKEKKA As String              ' データ作成結果

    Private jobMessage As String = ""          ' ジョブ監視メッセージ
    Private CntDenbun As Integer = 0        '2010/02/03 追加 電文件数
    ' 処理日付
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
    Private KessaiKamokuCode_ETC As String = "xx"   '決済科目の「その他」の信金科目コード
    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END

    ''' <summary>
    ''' iniファイル情報
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni
        Dim KAWASE_CENTER As String         ' 発信センタ名
        Dim JIKINKO_CODE As String          ' 自金庫コード
        Dim JIKINKO_NAME As String          ' 自金庫名
        Dim HONBU_CODE As String            ' 本部コード
        Dim TESUU_KOUZA1 As String          ' 手数料入金口座１
        Dim UCHIWAKE1 As String             ' 内訳１
        Dim TESUU_KOUZA2 As String          ' 手数料入金口座２
        Dim UCHIWAKE2 As String             ' 内訳２
        Dim TESUU_KOUZA3 As String          ' 手数料入金口座３
        Dim UCHIWAKE3 As String             ' 内訳３
        Dim TESUU_KOUZA4 As String          ' 手数料入金口座４
        Dim UCHIWAKE4 As String             ' 内訳４
        Dim TESUU_KOUZA5 As String          ' 手数料入金口座５
        Dim UCHIWAKE5 As String             ' 内訳５
        Dim KAWASE_IRAININ As String        ' 為替依頼人名
        Dim RIENTA_PATH As String           ' リエンタファイル作成先
        Dim DAT_PATH As String              ' DATのパス
        Dim CSVPATH As String               ' CSVファイル作成先
        Dim TESUU_OPEKBN As String          ' 手数料オペ区分
        Dim RIENTA_FILENAME As String       ' リエンタファイル名
        Dim SIKINTEKIYOU As String          ' 摘要名の一部をINIファイルより取得するため 岡崎
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
        Dim RSV2_EDITION As String        ' RSV2機能設定
        Dim COMMON_BAITAIWRITE As String  ' 媒体書込用フォルダ
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' 決済マスタのキー項目＋リエンタファイル名
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

    Dim ParaKessaiDate As String            ' パラメータから引き継いだ決済日

    Structure KeyInfo
        '決済データ作成＆帳票ＣＳＶデータ作成兼用
        Dim TORIS_CODE As String            ' 取引先主コード
        Dim TORIF_CODE As String            ' 取引先副コード
        Dim FURI_DATE As String             ' 振替日
        Dim KESSAI_YDATE As String          ' 決済予定日
        Dim TESUU_YDATE As String           ' 手数料徴求予定日
        Dim TESUU_KIN As String             ' 手数料金額  ：手数料合計
        Dim TESUU_KIN1 As String            ' 手数料金額１：引落手数料
        Dim TESUU_KIN2 As String            ' 手数料金額２：送料
        Dim TESUU_KIN3 As String            ' 手数料金額３：振込手数料
        Dim FURI_KEN As String              ' 振替済件数
        Dim FURI_KIN As String              ' 振替済金額
        Dim KIGYO_CODE As String            ' 企業コード
        Dim BAITAI_CODE As String           ' 媒体コード
        Dim SYUBETU_CODE As String          ' 種別コード
        Dim ITAKU_CODE As String            ' 委託者コード
        Dim ITAKU_NNAME As String           ' 委託者名漢字
        Dim ITAKU_KNAME As String           ' 委託者名カナ
        Dim FURI_CODE As String             ' 振替コード
        Dim NS_KBN As String                ' 入出金区分
        Dim TESUUTYO_PATN As String         ' 手数料徴求方法
        Dim TESUUTYO_KBN As String          ' 手数料徴求区分
        Dim KESSAI_KBN As String            ' 決済区分
        Dim TORIMATOME_SIT_NO As String     ' とりまとめ店
        Dim HONBU_KOUZA As String           ' 本部別段口座番号
        Dim TUKEKIN_NO As String            ' 決済金融機関
        Dim TUKESIT_NO As String            ' 決済支店
        Dim TUKEKAMOKU As String            ' 決済科目
        Dim TUKEKOUZA As String             ' 決済口座番号
        Dim TUKEMEIGI As String             ' 決済名義人 （カナ）
        Dim BIKOU1 As String                ' 備考１
        Dim BIKOU2 As String                ' 備考２
        Dim TSUUTYOSIT_NO As String         ' 手数料徴求支店
        Dim TSUUTYOKAMOKU As String         ' 手数料徴求科目
        Dim TSUUTYOKOUZA As String          ' 手数料徴口座番号
        Dim KESSAI_TORI_KBN As String       ' 0：資金決済と手数料徴求の両方の先、1：資金決済のみ先、2：手数料徴求のみ先
        Dim TESUUTYO_FLG As String          ' 手数料徴求済フラグ
        Dim RECTUUBAN As Long               ' レコード番号
        Dim JIFURI_TESUU_KIN As Long        ' 自振手数料合計
        Dim FURIKOMI_TESUU_KIN As Long      ' 振込手数料合計

        Dim KESSAI_FLG As String            ' 決済フラグ
        Dim KESSAI_DATE As String           ' 決済日時
        Dim TESUU_DATE As String            ' 手数料徴求日時
        Dim SYORI_KEN As String             ' 処理件数
        Dim SYORI_KIN As String             ' 処理金額
        Dim FUNOU_KEN As String             ' 不能件数
        Dim FUNOU_KIN As String             ' 不能金額
        Dim KAWASE_FURI_KEN As String       ' 為替振込先数
        Dim KAWASE_SEIKYU_KEN As String     ' 為替請求先数

        ' 学校自振用
        Dim GAKUNEN_FLG As ArrayList        ' 学年フラグ
        Dim KESSAI_SYUBETU As String        ' 決済種別(0：委託者一括請求 / 1：費目口座単位請求)
        Dim KESSAI_KIN_CODE As String       ' 決済金融機関コード
        Dim KESSAI_TENPO As String          ' 決済支店コード
        Dim KESSAI_KAMOKU As String         ' 決済科目
        Dim KESSAI_MEIGI As String          ' 決済名義人
        Dim KESSAI_KOUZA As String          ' 決済口座番号
        Dim HIMOKU_KINGAKU As String        ' 費目金額
        Dim HIMOKU_FURI_KIN As String       ' 費目振替金額
        Dim HIMOKU_FUNOU_KIN As String      ' 費目不能金額
        Dim UpSchMastFLG As Boolean         ' スケジュールマスタ更新対象フラグ
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
        Dim HIMOKU_NAME As String           ' 費目名
        Dim HIMOKU_GASSAN As Long           ' 学年、費目番号別の合算数
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

        Dim MESSAGE As String

        '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------START
        Dim SEIKYU_KBN As String            '手数料請求区分
        '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------END
        ' 初期化
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            FURI_DATE = ""
            KIGYO_CODE = ""
            KESSAI_YDATE = ""
            TESUU_YDATE = ""
            TESUU_KIN = ""
            TESUU_KIN1 = ""
            TESUU_KIN2 = ""
            TESUU_KIN3 = ""
            FURI_KEN = ""
            FURI_KIN = ""
            BAITAI_CODE = ""
            SYUBETU_CODE = ""
            ITAKU_CODE = ""
            ITAKU_NNAME = ""
            ITAKU_KNAME = ""
            FURI_CODE = ""
            NS_KBN = ""
            TESUUTYO_PATN = ""
            TESUUTYO_KBN = ""
            KESSAI_KBN = ""
            TORIMATOME_SIT_NO = ""
            HONBU_KOUZA = ""
            TUKEKIN_NO = ""
            TUKESIT_NO = ""
            TUKEKAMOKU = ""
            TUKEKOUZA = ""
            TUKEMEIGI = ""
            BIKOU1 = ""
            BIKOU2 = ""
            TSUUTYOSIT_NO = ""
            TSUUTYOKAMOKU = ""
            TSUUTYOKOUZA = ""
            KESSAI_TORI_KBN = ""
            TESUUTYO_FLG = "0"

            KESSAI_FLG = ""
            KESSAI_DATE = ""
            TESUU_DATE = ""
            SYORI_KEN = ""
            SYORI_KIN = ""
            FUNOU_KEN = ""
            FUNOU_KIN = ""
            KAWASE_FURI_KEN = ""
            KAWASE_SEIKYU_KEN = ""

            KESSAI_SYUBETU = "0"
            KESSAI_KIN_CODE = ""
            KESSAI_TENPO = ""
            KESSAI_KAMOKU = ""
            KESSAI_MEIGI = ""
            KESSAI_KOUZA = ""
            HIMOKU_KINGAKU = ""

            '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------START
            SEIKYU_KBN = ""
            '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------END
            MESSAGE = ""
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            HIMOKU_NAME = ""
            HIMOKU_GASSAN = 0
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
        End Sub

        ' ＤＢからの値を設定（資金決済データ作成用）
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_SV1").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_SV1").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_SV1").PadRight(8)
            KESSAI_YDATE = oraReader.GetString("KESSAI_YDATE_SV1")
            TESUU_YDATE = oraReader.GetString("TESUU_YDATE_SV1")
            TESUU_KIN = oraReader.GetString("TESUU_KIN_SV1")
            TESUU_KIN1 = oraReader.GetString("TESUU_KIN1_SV1")
            TESUU_KIN2 = oraReader.GetString("TESUU_KIN2_SV1")
            TESUU_KIN3 = oraReader.GetString("TESUU_KIN3_SV1")
            FURI_KEN = oraReader.GetString("FURI_KEN_SV1")
            FURI_KIN = oraReader.GetString("FURI_KIN_SV1")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_TV1")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_TV1")
            SYUBETU_CODE = oraReader.GetString("SYUBETU_TV1")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_TV1")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_TV1")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_TV1")
            FURI_CODE = oraReader.GetString("FURI_CODE_TV1")
            NS_KBN = oraReader.GetString("NS_KBN_TV1")
            TESUUTYO_PATN = oraReader.GetString("TESUUTYO_PATN_TV1")
            TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_TV1")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_TV1")
            TORIMATOME_SIT_NO = oraReader.GetString("TORIMATOME_SIT_TV1")
            HONBU_KOUZA = oraReader.GetString("HONBU_KOUZA_TV1")
            TUKEKIN_NO = oraReader.GetString("TUKEKIN_NO_TV1")
            TUKESIT_NO = oraReader.GetString("TUKESIT_NO_TV1")
            TUKEKAMOKU = oraReader.GetString("TUKEKAMOKU_TV1")
            TUKEKOUZA = oraReader.GetString("TUKEKOUZA_TV1")
            TUKEMEIGI = oraReader.GetString("TUKEMEIGI_KNAME_TV1")
            BIKOU1 = oraReader.GetString("BIKOU1_TV1")
            BIKOU2 = oraReader.GetString("BIKOU2_TV1")
            TSUUTYOSIT_NO = oraReader.GetString("TESUUTYO_SIT_TV1")
            TSUUTYOKAMOKU = oraReader.GetString("TESUUTYO_KAMOKU_TV1")
            TSUUTYOKOUZA = oraReader.GetString("TESUUTYO_KOUZA_TV1")
            KESSAI_TORI_KBN = oraReader.GetString("KESSAI_TORI_KBN")
            SYORI_KEN = oraReader.GetString("SYORI_KEN_SV1")
            SYORI_KIN = oraReader.GetString("SYORI_KIN_SV1")
            FUNOU_KEN = oraReader.GetString("FUNOU_KEN_SV1")
            FUNOU_KIN = oraReader.GetString("FUNOU_KIN_SV1")
            '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------START
            SEIKYU_KBN = oraReader.GetString("SEIKYU_KBN_TV1")
            '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------END
        End Sub

        ' ＤＢからの値を設定（費目口座単位の決済情報用）
        Friend Sub SetOraDataHimo(ByVal oraReader As CASTCommon.MyOracleReader)
            KESSAI_KIN_CODE = oraReader.GetString("KESSAI_KIN_CODE_H")
            KESSAI_TENPO = oraReader.GetString("KESSAI_TENPO_H")
            KESSAI_KAMOKU = oraReader.GetString("KESSAI_KAMOKU_H")
            KESSAI_MEIGI = oraReader.GetString("KESSAI_MEIGI_H")
            KESSAI_KOUZA = oraReader.GetString("KESSAI_KOUZA_H")
            HIMOKU_KINGAKU = oraReader.GetString("HIMOKU_KINGAKU_H")
            HIMOKU_FURI_KIN = oraReader.GetString("HIMOKU_FURI_KIN_H")
            HIMOKU_FUNOU_KIN = oraReader.GetString("HIMOKU_FUNOU_KIN_H")
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            HIMOKU_NAME = oraReader.GetString("HIMOKU_NAME_H")
            HIMOKU_GASSAN = oraReader.GetInt64("YOBI1_H")
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
        End Sub

    End Structure

    Structure KeyGakInfo
        Dim GAKKOU_CODE As String           ' 学校コード
        Dim GAKUNEN_CODE As String          ' 学年コード
        Dim HIMOKU_NAME As String           ' 費目名
        Dim KESSAI_KIN_CODE As String       ' 取扱金融機関コード
        Dim KESSAI_TENPO As String          ' 取扱支店コード
        Dim KESSAI_KAMOKU As String         ' 科目
        Dim KESSAI_KOUZA As String          ' 口座番号
        Dim KESSAI_MEIGI As String          ' 口座名義人
        Dim HIMOKU_KINGAKU As Long          ' 費目金額
        Dim FURIKETU_CODE As String         ' 振替結果コード
        Dim HIMOKU_FURI_KIN As Long         ' 費目振替金額
        Dim HIMOKU_FUNOU_KIN As Long        ' 費目不能金額
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
        Dim HIMOKU_GASSAN As Long           ' 学年、費目番号別の合算件数
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END


        ' 初期化
        Public Sub Init()
            GAKKOU_CODE = ""
            GAKUNEN_CODE = ""
            HIMOKU_NAME = ""
            KESSAI_KIN_CODE = ""
            KESSAI_TENPO = ""
            KESSAI_KAMOKU = ""
            KESSAI_KOUZA = ""
            KESSAI_MEIGI = ""
            HIMOKU_KINGAKU = 0
            HIMOKU_FURI_KIN = 0
            HIMOKU_FUNOU_KIN = 0
            FURIKETU_CODE = ""
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            HIMOKU_GASSAN = 0               ' 学年、費目番号別の合算件数
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
        End Sub

        ' ＤＢからの値を設定（決済ワークマスタ作成用）
        Friend Sub SetOraDataGak(ByVal oraReader As CASTCommon.MyOracleReader, ByVal HimoNo As String)
            GAKKOU_CODE = oraReader.GetString("GAKKOU_CODE_H")
            GAKUNEN_CODE = oraReader.GetString("GAKUNEN_CODE_H")
            HIMOKU_NAME = oraReader.GetString("HIMOKU_NAME" & HimoNo & "_H")
            KESSAI_KIN_CODE = oraReader.GetString("KESSAI_KIN_CODE" & HimoNo & "_H")
            KESSAI_TENPO = oraReader.GetString("KESSAI_TENPO" & HimoNo & "_H")
            KESSAI_KAMOKU = oraReader.GetString("KESSAI_KAMOKU" & HimoNo & "_H")
            KESSAI_KOUZA = oraReader.GetString("KESSAI_KOUZA" & HimoNo & "_H")
            KESSAI_MEIGI = oraReader.GetString("KESSAI_MEIGI" & HimoNo & "_H")
            HIMOKU_KINGAKU = oraReader.GetInt64("HIMOKU" & CInt(HimoNo) & "_KIN_M")
            FURIKETU_CODE = oraReader.GetString("FURIKETU_CODE_M")

            ' 費目振替金額,費目不能金額の設定
            If FURIKETU_CODE = "0" Then             ' 0:振替済
                HIMOKU_FURI_KIN = HIMOKU_KINGAKU
                HIMOKU_FUNOU_KIN = 0
            Else                                    ' 1:不能
                HIMOKU_FURI_KIN = 0
                HIMOKU_FUNOU_KIN = HIMOKU_KINGAKU
            End If
        End Sub

    End Structure

    ' 2016/06/13 タスク）綾部 CHG 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
    Structure msgDATA
        Dim msg_DATA As String
        Public Sub Init()
            msg_DATA = ""
        End Sub
    End Structure
    ' 2016/06/13 タスク）綾部 CHG 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 資金決済初期処理
    ''' </summary>
    ''' <returns>正常:True 異常:False</returns>
    ''' <remarks></remarks>
    Public Function KessaiInit(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try

            'パラメータの読込
            param = CmdArgs(0).Split(","c)
            If param.Length = 2 Then

                'ログ書込み情報の設定
                MainLOG.FuriDate = param(0)                     '決済日をセット
                MainLOG.JobTuuban = CType(param(1), Integer)
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(初期処理)開始", "成功")


                ParaKessaiDate = param(0)                       '決済日をセット

            ElseIf param.Length = 1 Then    '2010.01.27 追加　start

                'ログ書込み情報の設定
                MainLOG.FuriDate = param(0)                     '決済日をセット
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(初期処理)開始", "成功")

                ParaKessaiDate = param(0)                       '決済日をセット
                '2010.01.27 追加　end
            Else
                MainLOG.Write("(初期処理)開始", "失敗", "コマンドライン引数のパラメータが不正です")
                Return False

            End If

            'iniファイルの読込
            If IniRead() = False Then
                Return False
            End If

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

        ini_info.TESUU_KOUZA1 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA1")       '手数料入金口座１
        If ini_info.TESUU_KOUZA1 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:手数料入金口座１ 分類:KESSAI 項目:TESUUKOUZA1")
            jobMessage = "設定ファイル取得失敗 項目名:手数料入金口座１ 分類:KESSAI 項目:TESUUKOUZA1"
            Return False
        End If

        ini_info.UCHIWAKE1 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE1")         '内訳１
        If ini_info.UCHIWAKE1 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:内訳１ 分類:KESSAI 項目:UTIWAKE1")
            jobMessage = "設定ファイル取得失敗 項目名:内訳１ 分類:KESSAI 項目:UTIWAKE1"
            Return False
        End If

        ini_info.TESUU_KOUZA2 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA2")       '手数料入金口座２
        If ini_info.TESUU_KOUZA2 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:手数料入金口座２ 分類:KESSAI 項目:TESUUKOUZA2")
            jobMessage = "設定ファイル取得失敗 項目名:手数料入金口座２ 分類:KESSAI 項目:TESUUKOUZA2"
            Return False
        End If

        ini_info.UCHIWAKE2 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE2")         '内訳２
        If ini_info.UCHIWAKE2 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:内訳２ 分類:KESSAI 項目:UTIWAKE2")
            jobMessage = "設定ファイル取得失敗 項目名:内訳２ 分類:KESSAI 項目:UTIWAKE2"
            Return False
        End If

        ini_info.TESUU_KOUZA3 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA3")       '手数料入金口座３
        If ini_info.TESUU_KOUZA3 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:手数料入金口座３ 分類:KESSAI 項目:TESUUKOUZA3")
            jobMessage = "設定ファイル取得失敗 項目名:手数料入金口座３ 分類:KESSAI 項目:TESUUKOUZA3"
            Return False
        End If

        ini_info.UCHIWAKE3 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE3")         '内訳３
        If ini_info.UCHIWAKE3 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:内訳３ 分類:KESSAI 項目:UTIWAKE3")
            jobMessage = "設定ファイル取得失敗 項目名:内訳３ 分類:KESSAI 項目:UTIWAKE3"
            Return False
        End If

        ini_info.TESUU_KOUZA4 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA4")       '手数料入金口座４
        If ini_info.TESUU_KOUZA4 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:手数料入金口座４ 分類:KESSAI 項目:TESUUKOUZA4")
            jobMessage = "設定ファイル取得失敗 項目名:手数料入金口座４ 分類:KESSAI 項目:TESUUKOUZA4"
            Return False
        End If

        ini_info.UCHIWAKE4 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE4")         '内訳４
        If ini_info.UCHIWAKE4 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:内訳４ 分類:KESSAI 項目:UTIWAKE4")
            jobMessage = "設定ファイル取得失敗 項目名:内訳４ 分類:KESSAI 項目:UTIWAKE4"
            Return False
        End If

        ini_info.TESUU_KOUZA5 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA5")       '手数料入金口座５
        If ini_info.TESUU_KOUZA5 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:手数料入金口座５ 分類:KESSAI 項目:TESUUKOUZA5")
            jobMessage = "設定ファイル取得失敗 項目名:手数料入金口座５ 分類:KESSAI 項目:TESUUKOUZA5"
            Return False
        End If

        ini_info.UCHIWAKE5 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE5")         '内訳５
        If ini_info.UCHIWAKE5 = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:内訳５ 分類:KESSAI 項目:UTIWAKE5")
            jobMessage = "設定ファイル取得失敗 項目名:内訳５ 分類:KESSAI 項目:UTIWAKE5"
            Return False
        End If

        ini_info.KAWASE_IRAININ = CASTCommon.GetFSKJIni("KESSAI", "IRAININ")     '為替依頼人名
        If ini_info.KAWASE_IRAININ = "err" OrElse ini_info.KAWASE_IRAININ = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替依頼人名 分類:KESSAI 項目:IRAININ")
            jobMessage = "設定ファイル取得失敗 項目名:為替依頼人名 分類:KESSAI 項目:IRAININ"
            Return False
        End If

        ini_info.SIKINTEKIYOU = CASTCommon.GetFSKJIni("KESSAI", "SIKINTEKIYOU")
        If ini_info.SIKINTEKIYOU = "err" OrElse ini_info.SIKINTEKIYOU = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:摘要 分類:KESSAI 項目:SIKINTEKIYOU")
            jobMessage = "設定ファイル取得失敗 項目名:摘要 分類:KESSAI 項目:SIKINTEKIYOU"
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

        ini_info.TESUU_OPEKBN = CASTCommon.GetFSKJIni("KESSAI", "OPE_TESUU")       '手数料オペ区分
        If ini_info.TESUU_OPEKBN = "err" OrElse ini_info.TESUU_OPEKBN = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:手数料オペ区分 分類:KESSAI 項目:OPE_TESUU")
            jobMessage = "設定ファイル取得失敗 項目名:手数料オペ区分 分類:KESSAI 項目:OPE_TESUU"
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")       'リエンタファイル名
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:手数料オペ区分 分類:KESSAI 項目:RIENTANAME")
            jobMessage = "設定ファイル取得失敗 項目名:手数料オペ区分 分類:KESSAI 項目:RIENTANAME"
            Return False
        End If

        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
        ini_info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If ini_info.RSV2_EDITION = "err" OrElse ini_info.RSV2_EDITION = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
            jobMessage = "設定ファイル取得失敗 項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION"
            Return False
        End If

        ini_info.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
        If ini_info.COMMON_BAITAIWRITE = "err" OrElse ini_info.COMMON_BAITAIWRITE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:媒体書込用フォルダ 分類:COMMON 項目:BAITAIWRITE")
            jobMessage = "設定ファイル取得失敗 項目名:媒体書込用フォルダ 分類:COMMON 項目:BAITAIWRITE"
            Return False
        End If
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

        Return True

    End Function

    ' 機能　 ： 資金決済データ作成処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Public Function Main(ByVal command As String) As Integer

        MainDB = New CASTCommon.MyOracle
        Dim bRet As Boolean = True
        Dim iRet As Integer
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 60
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME1")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 60
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

        ' パラメータチェック
        Try
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "資金決済データ作成処理(開始)", "成功")


            MainDB.BeginTrans()     ' トランザクション開始

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("(主処理)", "失敗", "資金決済リエンタ作成処理で実行待ちタイムアウト")
                MainLOG.UpdateJOBMASTbyErr("資金決済リエンタ作成処理で実行待ちタイムアウト")
                Return -1
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            '*******************************
            ' 回次を取得
            '*******************************
            If GetKaiji() = False Then
                MainLOG.Write("(主処理)", "失敗", "回次の取得に失敗しました")
                Return -1
            End If

            '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
            '********************************************
            ' 決済科目の「その他」の信金科目コードを取得
            '********************************************
            Call GetKessaiKamokuCode_ETC()
            '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END

            '*****************************************
            ' 資金決済データを格納とスケジュールの更新
            '*****************************************
            Dim aryKessaiData As New ArrayList      '資金決済データ格納用
            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
            Dim aryMsgData As New ArrayList
            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            Dim aryHimokuName As New ArrayList      '費目名保持用リスト
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
            iRet = MakeKessaiData(aryKessaiData, aryMsgData, aryHimokuName)
            'iRet = MakeKessaiData(aryKessaiData, aryMsgData)
            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END
            Select Case iRet
                Case 0          ' データ格納成功
                    bRet = True
                Case 1          ' 対象データ０件
                    bRet = True
                Case Else       ' データ格納失敗
                    bRet = False
            End Select

            '***********************
            ' リエンタFD作成
            '***********************
            Dim totalRow As Integer = aryKessaiData.Count()
            Dim msgtitle As String = "資金決済リエンタ作成(KFK010)"

            If iRet = 0 AndAlso aryKessaiData.Count() > 0 Then

                If MakeRientaFD(aryKessaiData) = False Then     '何行目から何行目のデータまで作成するか指定,FD何枚目かも指定
                    jobMessage = "資金決済リエンタ作成失敗"
                    iRet = -1
                Else
                    Select Case ini_info.RSV2_EDITION
                        Case "2"
                            '---------------------------------------------------------------
                            ' ファイル名構築
                            '  [ファイル名] RNT_KR_yyyyMMddHHmmss_1(1固定)
                            '---------------------------------------------------------------
                            Dim RientFileName As String = "RNT_KR_" & strDate & "_" & strTime & "_1"

                            '---------------------------------------------
                            ' ファイルコピー
                            '---------------------------------------------
                            If File.Exists(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName)) Then
                                File.Delete(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName))
                            End If
                            File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME), Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName), True)

                        Case Else
                            Do
                                Try

                                    Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                                    Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                                    iRet = 0
                                    Exit Do

                                Catch ex As Exception
                                    If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                        iRet = -1
                                        jobMessage = "媒体挿入キャンセル"
                                        MainLOG.Write_LEVEL1("媒体要求", "失敗", "媒体挿入キャンセル")
                                        Exit Do
                                    End If
                                End Try
                            Loop

                            Select Case iRet
                                Case 0          ' データ格納成功
                                    If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME)) Then
                                        If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                            jobMessage = "媒体内ファイル削除キャンセル"
                                            iRet = -1
                                        End If
                                    End If

                                    If iRet = 0 Then
                                        File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME), Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME), True)
                                    End If
                            End Select
                    End Select
                End If

            End If

            If iRet <> 0 Then
                bRet = False
            End If

            '*******************************
            ' 帳票出力
            '*******************************
            ' 資金決済データが１件以上存在する場合、帳票出力
            If iRet = 0 Then
                ' 帳票出力
                Dim PrnSyoKekka As ClsPrnKessaiSyorikekkaKakuninhyo = Nothing
                Dim intPrnRet As Integer

                PrnSyoKekka = New ClsPrnKessaiSyorikekkaKakuninhyo

                PrnSyoKekka.OraDB = MainDB
                PrnSyoKekka.Tenmei = ""
                PrnSyoKekka.Itakusyamei = ""

                ' 資金決済処理結果確認表　タイトル行出力
                PrnSyoKekka.CreateCsvFile()

                ' 資金決済処理結果確認表　明細行出力
                '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
                intPrnRet = PrnSyoKekka.OutputCSVKekka(aryKessaiData, ini_info.JIKINKO_CODE, strDate, strTime, ParaKessaiDate, MainDB, aryMsgData, aryHimokuName)
                'intPrnRet = PrnSyoKekka.OutputCSVKekka(aryKessaiData, ini_info.JIKINKO_CODE, strDate, strTime, ParaKessaiDate, MainDB, aryMsgData)
                '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END

                If intPrnRet <> 0 Then
                    bRet = False
                    MainLOG.Write("処理結果確認表(資金決済)出力", "失敗", "処理結果確認表(資金決済)ＣＳＶ出力に失敗しました。")

                End If

                ' 資金決済処理結果確認表
                If Not PrnSyoKekka Is Nothing And intPrnRet = 0 Then
                    PrnSyoKekka.CloseCsv()

                    '印刷バッチ呼び出し
                    Dim ExeRepo As New CAstReports.ClsExecute
                    Dim param As String = ""
                    Dim nret As Integer

                    'パラメータ設定：ログイン名、ＣＳＶファイル名
                    param = MainLOG.UserID & "," & PrnSyoKekka.FileName

                    nret = ExeRepo.ExecReport("KFKP001.EXE", param)

                    If nret <> 0 Then
                        '印刷失敗：戻り値に対応したエラーメッセージを表示する
                        Select Case nret
                            Case -1
                                jobMessage = "処理結果確認表(資金決済)印刷対象０件。"

                            Case Else

                                jobMessage = "処理結果確認表(資金決済)印刷失敗。エラーコード：" & nret
                        End Select
                        MainLOG.Write("処理結果確認表(資金決済)印刷", "失敗", jobMessage)

                        bRet = False
                    End If
                End If
            End If

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            If bRet = False Then
                If MainLOG.JobTuuban <> 0 Then

                    If jobMessage = "" Then
                        Call MainLOG.UpdateJOBMASTbyErr("ログ参照")
                    Else
                        Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                    End If

                End If
                ' ロールバック
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "対象データ０件"
                Else
                    jobMessage = "リエンタ作成件数:" & CntDenbun & "件"
                End If

                If MainLOG.JobTuuban <> 0 Then

                    Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                End If

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
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            If Not MainDB Is Nothing Then
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

                ' ロールバック
                MainDB.Rollback()
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "資金決済データ作成処理(終了)", "成功")

        End Try

        Return 0

    End Function

    ' 機能　 ： 資金決済データ作成処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    '*** 修正 mitsu 2008/09/09 0件データ対応のため大幅修正(不要行削除) ***
    '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
    Private Function MakeKessaiData(ByRef aryKes As ArrayList, ByRef aryMSG As ArrayList, ByRef aryHimokuName As ArrayList) As Integer
        'Private Function MakeKessaiData(ByRef aryKes As ArrayList, ByRef aryMSG As ArrayList) As Integer
        '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END

        Dim OraKesReader As CASTCommon.MyOracleReader       ' 決済ビュー

        Dim SQL As StringBuilder
        Dim CommonSQL As StringBuilder
        Dim TudoWhereSQL As StringBuilder
        Dim KessaiOnlyWhereSQL As StringBuilder
        Dim TesuuOnlyWhereSQL As StringBuilder

        OraKesReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)
        CommonSQL = New StringBuilder(128)
        TudoWhereSQL = New StringBuilder(128)
        KessaiOnlyWhereSQL = New StringBuilder(128)
        TesuuOnlyWhereSQL = New StringBuilder(128)

        Try
            MainLOG.Write("(資金決済データ格納)開始", "成功")

            CommonSQL.Append("SELECT")
            CommonSQL.Append(" TORIS_CODE_SV1")
            CommonSQL.Append(",TORIF_CODE_SV1")
            CommonSQL.Append(",FURI_DATE_SV1")
            CommonSQL.Append(",KESSAI_YDATE_SV1")
            CommonSQL.Append(",TESUU_YDATE_SV1")
            CommonSQL.Append(",TESUU_KIN_SV1")
            CommonSQL.Append(",TESUU_KIN1_SV1")
            CommonSQL.Append(",TESUU_KIN2_SV1")
            CommonSQL.Append(",TESUU_KIN3_SV1")
            CommonSQL.Append(",FURI_KEN_SV1")
            CommonSQL.Append(",FURI_KIN_SV1")
            CommonSQL.Append(",SYORI_KEN_SV1")
            CommonSQL.Append(",SYORI_KIN_SV1")
            CommonSQL.Append(",FUNOU_KEN_SV1")
            CommonSQL.Append(",FUNOU_KIN_SV1")
            CommonSQL.Append(",KIGYO_CODE_TV1")
            CommonSQL.Append(",BAITAI_CODE_TV1")
            CommonSQL.Append(",SYUBETU_TV1")
            CommonSQL.Append(",ITAKU_CODE_TV1")
            CommonSQL.Append(",ITAKU_NNAME_TV1")
            CommonSQL.Append(",ITAKU_KNAME_TV1")
            CommonSQL.Append(",FURI_CODE_TV1")
            CommonSQL.Append(",NS_KBN_TV1")
            CommonSQL.Append(",TESUUTYO_PATN_TV1")
            CommonSQL.Append(",TESUUTYO_KBN_TV1")
            CommonSQL.Append(",KESSAI_KBN_TV1")
            CommonSQL.Append(",TORIMATOME_SIT_TV1")
            CommonSQL.Append(",HONBU_KOUZA_TV1")
            CommonSQL.Append(",TUKEKIN_NO_TV1")
            CommonSQL.Append(",TUKESIT_NO_TV1")
            CommonSQL.Append(",TUKEKAMOKU_TV1")
            CommonSQL.Append(",TUKEKOUZA_TV1")
            CommonSQL.Append(",TUKEMEIGI_KNAME_TV1")
            CommonSQL.Append(",BIKOU1_TV1")
            CommonSQL.Append(",BIKOU2_TV1")
            CommonSQL.Append(",TESUUTYO_SIT_TV1")
            CommonSQL.Append(",TESUUTYO_KAMOKU_TV1")
            CommonSQL.Append(",TESUUTYO_KOUZA_TV1")
            CommonSQL.Append(",0 AS JYUNBAN")
            '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------START
            '別段出金件数対応のため、手数料請求区分を抽出
            CommonSQL.Append(",SEIKYU_KBN_TV1")
            '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------END

            ' 資金決済と手数料徴求を同時に行う先の条件ＳＱＬ文(都度)
            TudoWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(ParaKessaiDate))
            TudoWhereSQL.Append("   AND TESUU_YDATE_SV1  = " & SQ(ParaKessaiDate))
            TudoWhereSQL.Append("   AND TESUUKEI_FLG_SV1 = '1'")                                    ' 1:手数料計算済み
            TudoWhereSQL.Append("   AND KESSAI_FLG_SV1   = '2'")                                    ' 0:未決済
            TudoWhereSQL.Append("   AND TESUUTYO_FLG_SV1 = '2'")                                    ' 0:手数料未徴求
            TudoWhereSQL.Append("   AND TYUUDAN_FLG_SV1  = '0'")                                    ' 0:中断なし
            TudoWhereSQL.Append("   AND FURI_KIN_SV1     >  0")                                     ' 振替済金額あり

            ' 資金決済だけ行う先の条件ＳＱＬ文（都度以外）
            KessaiOnlyWhereSQL.Append(" WHERE KESSAI_YDATE_SV1  = " & SQ(ParaKessaiDate))
            'KessaiOnlyWhereSQL.Append("   AND TESUU_YDATE_SV1  != " & SQ(ParaKessaiDate))
            KessaiOnlyWhereSQL.Append("   AND TESUUKEI_FLG_SV1  = '1'")                             ' 1:手数料計算済
            KessaiOnlyWhereSQL.Append("   AND KESSAI_FLG_SV1    = '2'")                             ' 0:未決済
            KessaiOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '0'")                             ' 0:手数料未徴求
            KessaiOnlyWhereSQL.Append("   AND TYUUDAN_FLG_SV1   = '0'")                             ' 0:中断なし
            KessaiOnlyWhereSQL.Append("   AND FURI_KIN_SV1      >  0")                              ' 振替済金額あり

            '***手数料のみは抽出しない 2009.10.29 start
            ' 手数料徴求だけ行う先の条件ＳＱＬ文
            'TesuuOnlyWhereSQL.Append(" WHERE (KESSAI_YDATE_SV1 != " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUU_YDATE_SV1   = " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUUKEI_FLG_SV1  = '1'")                              ' 1:手数料計算済
            'TesuuOnlyWhereSQL.Append("   AND KESSAI_FLG_SV1    = '1'")                              ' 1:決済済み
            'TesuuOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '2'")                              ' 0:手数料未徴求
            'TesuuOnlyWhereSQL.Append("   AND TYUUDAN_FLG_SV1   = '0'")                              ' 0:中断なし
            'TesuuOnlyWhereSQL.Append("   AND FURI_KIN_SV1      >  0)")                              ' 振替済金額あり
            'TesuuOnlyWhereSQL.Append("   OR")

            ' 手数料徴求区分が 1:一括徴求 の場合、資金決済と手数料徴求を同時に行う先も含める
            'TesuuOnlyWhereSQL.Append("      (KESSAI_YDATE_SV1  = " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUU_YDATE_SV1   = " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUUKEI_FLG_SV1  = '1'")                              ' 1:手数料計算済
            'TesuuOnlyWhereSQL.Append("   AND KESSAI_FLG_SV1    = '2'")                              ' 2:決済待ち
            'TesuuOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '0'")                              ' 0:手数料未徴求
            ''TesuuOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '2'")                             ' 0:手数料未徴求
            'TesuuOnlyWhereSQL.Append("   AND TYUUDAN_FLG_SV1   = '0'")                              ' 0:中断なし
            'TesuuOnlyWhereSQL.Append("   AND FURI_KIN_SV1      >  0")                               ' 振替済金額あり
            'TesuuOnlyWhereSQL.Append("   AND TESUUTYO_KBN_TV1  = '1')")                             ' 1:一括徴求
            '***手数料のみは抽出しない 2009.10.29 end

            SQL.Append("(")
            SQL.Append(CommonSQL)
            SQL.Append(",0 AS KESSAI_TORI_KBN")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(TudoWhereSQL)
            SQL.Append(")")
            SQL.Append("UNION")
            SQL.Append("(")
            SQL.Append(CommonSQL)
            SQL.Append(",1 AS KESSAI_TORI_KBN")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(KessaiOnlyWhereSQL)
            SQL.Append(")")
            '***手数料のみは抽出しない 2009.10.29 start
            'SQL.Append("UNION")
            'SQL.Append("(")
            'SQL.Append("SELECT")
            'SQL.Append(" TORIS_CODE_SV1")
            'SQL.Append(",TORIF_CODE_SV1")
            'SQL.Append(",'00000000' AS FURI_DATE_SV1")
            'SQL.Append(",'00000000' AS KESSAI_YDATE_SV1")
            'SQL.Append(",MAX(TESUU_YDATE_SV1) AS TESUU_YDATE_SV1")
            'SQL.Append(",SUM(TESUU_KIN_SV1) AS TESUU_KIN_SV1")
            'SQL.Append(",SUM(TESUU_KIN1_SV1) AS TESUU_KIN1_SV1")
            'SQL.Append(",SUM(TESUU_KIN2_SV1) AS TESUU_KIN2_SV1")
            'SQL.Append(",SUM(TESUU_KIN3_SV1) AS TESUU_KIN3_SV1")
            'SQL.Append(",SUM(FURI_KEN_SV1) AS FURI_KEN_SV1")
            'SQL.Append(",SUM(FURI_KIN_SV1) AS FURI_KIN_SV1")
            'SQL.Append(",SUM(SYORI_KEN_SV1) AS SYORI_KEN_SV1")
            'SQL.Append(",SUM(SYORI_KIN_SV1) AS SYORI_KIN_SV1")
            'SQL.Append(",SUM(FUNOU_KEN_SV1) AS FUNOU_KEN_SV1")
            'SQL.Append(",SUM(FUNOU_KIN_SV1) AS FUNOU_KIN_SV1")
            'SQL.Append(",MAX(KIGYO_CODE_TV1) AS KIGYO_CODE_TV1")
            'SQL.Append(",MAX(BAITAI_CODE_TV1) AS BAITAI_CODE_TV1")
            'SQL.Append(",MAX(SYUBETU_TV1) AS SYUBETU_TV1")
            'SQL.Append(",MAX(ITAKU_CODE_TV1) AS ITAKU_CODE_TV1")
            'SQL.Append(",MAX(ITAKU_NNAME_TV1) AS ITAKU_NNAME_TV1")
            'SQL.Append(",MAX(ITAKU_KNAME_TV1) AS ITAKU_KNAME_TV1")
            'SQL.Append(",MAX(FURI_CODE_TV1) AS FURI_CODE_TV1")
            'SQL.Append(",MAX(NS_KBN_TV1) AS NS_KBN_TV1")
            'SQL.Append(",MAX(TESUUTYO_PATN_TV1) AS TESUUTYO_PATN_TV1")
            'SQL.Append(",MAX(TESUUTYO_KBN_TV1) AS TESUUTYO_KBN_TV1")
            'SQL.Append(",MAX(KESSAI_KBN_TV1) AS KESSAI_KBN_TV1")
            'SQL.Append(",MAX(TORIMATOME_SIT_TV1) AS TORIMATOME_SIT_TV1")
            'SQL.Append(",MAX(HONBU_KOUZA_TV1) AS HONBU_KOUZA_TV1")
            'SQL.Append(",MAX(TUKEKIN_NO_TV1) AS TUKEKIN_NO_TV1")
            'SQL.Append(",MAX(TUKESIT_NO_TV1) AS TUKESIT_NO_TV1")
            'SQL.Append(",MAX(TUKEKAMOKU_TV1) AS TUKEKAMOKU_TV1")
            'SQL.Append(",MAX(TUKEKOUZA_TV1) AS TUKEKOUZA_TV1")
            'SQL.Append(",MAX(TUKEMEIGI_KNAME_TV1) AS TUKEMEIGI_KNAME_TV1")
            'SQL.Append(",MAX(BIKOU1_TV1) AS BIKOU1_TV1")
            'SQL.Append(",MAX(BIKOU2_TV1) AS BIKOU2_TV1")
            'SQL.Append(",MAX(TESUUTYO_SIT_TV1) AS TESUUTYO_SIT_TV1")
            'SQL.Append(",MAX(TESUUTYO_KAMOKU_TV1) AS TESUUTYO_KAMOKU_TV1")
            'SQL.Append(",MAX(TESUUTYO_KOUZA_TV1) AS TESUUTYO_KOUZA_TV1")
            'SQL.Append(",1 AS JYUNBAN")
            'SQL.Append(",2 AS KESSAI_TORI_KBN")
            'SQL.Append(" FROM V1_KESMAST")
            'SQL.Append(TesuuOnlyWhereSQL)
            'SQL.Append(" GROUP BY TORIS_CODE_SV1, TORIF_CODE_SV1")
            'SQL.Append(")")
            '***手数料のみは抽出しない 2009.10.29 end 

            SQL.Append(" ORDER BY TORIS_CODE_SV1, TORIF_CODE_SV1, JYUNBAN, FURI_DATE_SV1")

            Dim Key As KeyInfo = Nothing
            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
            Dim MSG As msgDATA = Nothing
            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
            Dim test As String = SQL.ToString

            If OraKesReader.DataReader(SQL) = True Then

                Dim lstKessaiData As New ArrayList(128)
                Dim lstHimokuData As New ArrayList(128)
                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                Dim lstMsgData As New ArrayList(128)
                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                Dim lstHimokuName As New ArrayList(128)
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

                ' キー初期化
                Key.Init()
                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                MSG.Init()
                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END

                ' 最初のキー設定
                Call Key.SetOraDataKessai(OraKesReader)

                Do While OraKesReader.EOF = False

                    ' 媒体コードの判定
                    If Key.BAITAI_CODE = "07" Then
                        ' 媒体コードが07：学校自振の場合、決済種別(0：委託者一括請求 or 1：費目口座単位請求)を判定
                        If GetKessaiSyubetu(Key.TORIS_CODE, Key.KESSAI_SYUBETU) = False Then
                            Return -1
                        End If
                    Else
                        ' 媒体コードが07：学校自振以外の場合、決済種別は 0：委託者一括請求 のみ
                        Key.KESSAI_SYUBETU = "0"
                    End If

                    '*** 修正 mitsu 2008/10/15 手数料徴求だけ行う場合は費目単位処理に行かない ***
                    'If Key.KESSAI_SYUBETU = "0" Then
                    If Key.KESSAI_SYUBETU = "0" OrElse Key.KESSAI_TORI_KBN = "2" Then
                        '************************************************************************
                        ' 決済種別が 0：委託者一括請求 の場合

                        ' 資金決済データ取得処理(企業自振用)
                        '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
                        If fn_GetKessaiData(Key, lstKessaiData, lstMsgData, lstHimokuName) = False Then
                            'If fn_GetKessaiData(Key, lstKessaiData, lstMsgData) = False Then
                            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END
                            Return -1
                        End If

                        Key.UpSchMastFLG = True
                    Else
                        ' 決済種別が 1：費目口座単位請求 の場合（費目口座単位で集計を行う）

                        ' 決済ワークマスタ作成処理
                        If CreateMAIN0500G_WORK(Key) = False Then
                            Return -1
                        End If

                        ' 決済ワークマスタ検索処理
                        If SelectMAIN0500G_WORK(lstHimokuData) = False Then
                            Return -1
                        End If

                        ' 資金決済データ取得処理(学校自振用)
                        If Not (lstHimokuData Is Nothing OrElse lstHimokuData.Count = 0) Then
                            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
                            If fn_GetGakKessaiData(Key, lstKessaiData, lstHimokuData, lstMsgData, lstHimokuName) = False Then
                                'If fn_GetGakKessaiData(Key, lstKessaiData, lstHimokuData) = False Then
                                '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END
                                Return -1
                            End If

                            Key.UpSchMastFLG = True
                        Else
                            ' 費目口座単位の決済情報がゼロ件の場合
                            ' 資金決済データ作成、スケジュールマスタ更新を行わない
                            Key.UpSchMastFLG = False
                        End If
                    End If

                    ' スケジュールマスタの更新処理 
                    If Key.UpSchMastFLG = True AndAlso UpdateSchMast(Key) = False Then
                        MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                        Return -1
                    End If

                    ' 対象データの次レコードを読込む
                    OraKesReader.NextRead()

                    If OraKesReader.EOF = False Then
                        ' キーの再設定
                        Call Key.SetOraDataKessai(OraKesReader)

                    End If

                    aryKes.AddRange(lstKessaiData)

                    ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                    aryMSG.AddRange(lstMsgData)
                    ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                    aryHimokuName.AddRange(lstHimokuName)
                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

                Loop

            End If


            If aryKes.Count = 0 Then
                MainLOG.Write("(資金決済データ格納)", "失敗", "件数０件")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(資金決済データ格納)", "失敗", ex.Message)

            Return -1
        Finally
            If Not OraKesReader Is Nothing Then OraKesReader.Close()
            MainLOG.Write("(資金決済データ格納)終了", "成功")

        End Try

        Return 0

    End Function

    ' 機能　 ： 資金決済データ取得処理(企業自振用)
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
    Private Function fn_GetKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByRef lstMsgData As ArrayList, ByRef lstHimokuName As ArrayList) As Boolean
        'Private Function fn_GetKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByRef lstMsgData As ArrayList) As Boolean
        '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END

        Dim errFlg As Boolean = False
        Dim errMsg As String = "決済情報に誤りがあります。"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing
        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
        Dim MData As msgDATA = Nothing
        Dim OP_Count As Integer = 0
        lstMsgData = New ArrayList(128)
        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
        lstHimokuName = New ArrayList(128)
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

        strKEKKA = ""
        lstKessaiData = New ArrayList(128)

        Try

            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
            MData.Init()
            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END

            KData.Init()

            '手数料徴求済フラグを初期化する
            Key.TESUUTYO_FLG = "0"

            ' 手数料徴求区分が 0:都度徴求、かつ、決済予定日と手数料徴求予定日が処理対象日の場合
            If Key.TESUUTYO_KBN = "0" And Key.KESSAI_YDATE = ParaKessaiDate And Key.TESUU_YDATE = ParaKessaiDate Then
                ' 決済区分の判定
                Select Case Key.KESSAI_KBN
                    Case "00"    ' しんきん中金預け金

                        ' 手数料徴求方法の判定
                        Select Case Key.TESUUTYO_PATN
                            Case "0"    ' 差引入金

                                ' 決済科目の判定
                                Select Case Key.TUKEKAMOKU
                                    Case "99"   ' 諸勘定
                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 諸勘定入金のデータ作成
                                        If errFlg = False AndAlso fn_SYOKANJO_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 諸勘定入金額に手数料が差引かれている場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then

                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select

                                        End If

                                    Case Else   ' 上記以外の場合
                                        errFlg = True
                                        errMsg &= "(決済科目)"
                                End Select

                            Case "1"    ' 直接入金
                                errFlg = True
                                errMsg &= "(手数料徴求方法)"

                            Case Else   ' 上記以外の場合
                                errFlg = True
                                errMsg &= "(手数料徴求方法)"
                        End Select

                    Case "01"    ' 口座入金

                        ' 手数料徴求方法の判定
                        Select Case Key.TESUUTYO_PATN
                            Case "0"    ' 差引入金

                                ' 決済科目の判定
                                Select Case Key.TUKEKAMOKU
                                    Case "02"   ' 普通

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 普通入金のデータ作成
                                        If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 普通入金額に手数料が差引かれている場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select

                                        End If

                                    Case "01"   ' 当座

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 当座入金のデータ作成
                                        If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 当座入金額に手数料が差引かれている場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select
                                        End If

                                    Case "04"   ' 別段

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 別段入金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 別段入金額に手数料が差引かれている場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select
                                        End If

                                    Case Else   ' 上記以外の場合
                                        errFlg = True
                                        errMsg &= "(決済科目)"
                                End Select

                            Case "1"    ' 直接入金

                                ' 決済科目の判定
                                Select Case Key.TUKEKAMOKU
                                    Case "02"   ' 普通

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 普通入金のデータ作成
                                        If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 振替済金額と手数料金額の差額が０円超の場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select
                                        End If

                                    Case "01"   ' 当座

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 当座入金のデータ作成
                                        If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 振替済金額と手数料金額の差額が０円超の場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select
                                        End If

                                    Case "04"   ' 別段

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 別段入金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 振替済金額と手数料金額の差額が０円超の場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select
                                        End If

                                    Case Else   ' 上記以外の場合
                                        errFlg = True
                                        errMsg &= "(決済科目)"
                                End Select

                            Case Else   ' 上記以外の場合
                                errFlg = True
                                errMsg &= "(手数料徴求方法)"
                        End Select

                    Case "02"    ' 為替振込

                        ' 手数料徴求方法の判定
                        Select Case Key.TESUUTYO_PATN
                            Case "0"    ' 差引入金

                                ' 決済科目の判定
                                Select Case Key.TUKEKAMOKU
                                    '2017/05/29 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    Case "01", "02", KessaiKamokuCode_ETC  ' 当座、普通、その他"
                                        'Case "01", "02"  ' 当座、普通
                                        '2017/05/29 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 為替振込のデータ作成
                                        If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 諸勘定入金額に手数料が差引かれている場合のみ、手数料データ作成
                                        If strKEKKA <> "金額０" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                                Case "1"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                            End Select
                                        End If

                                    Case Else   ' 上記以外の場合
                                        errFlg = True
                                        errMsg &= "(決済科目)"
                                End Select

                            Case "1"    ' 直接入金
                                errFlg = True
                                errMsg &= "(手数料徴求方法)"

                            Case Else   ' 上記以外の場合
                                errFlg = True
                                errMsg &= "(手数料徴求方法)"
                        End Select

                    Case "03"    ' 為替付替
                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                        If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                            errFlg = True
                            errMsg &= "(決済科目)"
                        Else
                            '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                            ' 手数料徴求方法の判定
                            Select Case Key.TESUUTYO_PATN
                                Case "0"    ' 差引入金
                                    ' 別段出金のデータ作成
                                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' 資金決済データのリスト作成
                                        lstKessaiData.Add(KData)
                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                        OP_Count += 1
                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                    Else
                                        errFlg = True
                                    End If

                                    ' 為替付替のデータ作成
                                    If errFlg = False AndAlso fn_KAWASE_TUKEKAE(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' 資金決済データのリスト作成
                                        lstKessaiData.Add(KData)
                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                        OP_Count += 1
                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                    Else
                                        errFlg = True
                                    End If

                                    ' 為替付替金額に手数料が差引かれている場合のみ、手数料データ作成
                                    If strKEKKA <> "金額０" Then
                                        Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                            Case "1"
                                                ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' 資金決済データのリスト作成
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If

                                                ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                If CLng(Key.TESUU_KIN3) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' 資金決済データのリスト作成
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If

                                            Case "2"
                                                ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                                If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' 資金決済データのリスト作成
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If

                                                ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                                If CLng(Key.TESUU_KIN3) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' 資金決済データのリスト作成
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If
                                            Case Else
                                                errFlg = True
                                                errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                        End Select
                                    End If

                                '*** 修正 mitsu 2008/06/25 科目の判定はしない ***
                                '    Case Else   ' 上記以外の場合
                                '        errFlg = True
                                '        errMsg &= "(決済科目)"
                                'End Select
                                '************************************************

                                Case "1"    ' 直接入金
                                    errFlg = True
                                    errMsg &= "(手数料徴求方法)"

                                Case Else   ' 上記以外の場合
                                    errFlg = True
                                    errMsg &= "(手数料徴求方法)"
                            End Select
                            '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                        End If
                    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                    Case "04"    ' 別段出金のみ
                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                        If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                            errFlg = True
                            errMsg &= "(決済科目)"
                        Else
                            '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                            ' 別段出金のデータ作成
                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                ' 資金決済データのリスト作成
                                lstKessaiData.Add(KData)
                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                OP_Count += 1
                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                            Else
                                errFlg = True
                            End If
                            '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                        End If
                    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                    Case "05"    ' 特別企業外
                        ' 処理対象外

                    Case "99"
                        ' 処理対象外

                    Case Else   ' 上記以外の場合
                        errFlg = True
                        errMsg &= "(決済区分)"
                End Select

            ElseIf Key.TESUUTYO_KBN = "0" OrElse Key.TESUUTYO_KBN = "1" OrElse Key.TESUUTYO_KBN = "2" OrElse Key.TESUUTYO_KBN = "3" Then

                ' 決済予定日が処理対象日の場合
                If Key.KESSAI_YDATE = ParaKessaiDate Then

                    ' 手数料徴求方法の判定
                    Select Case Key.TESUUTYO_PATN
                        Case "0"    ' 差引入金

                            Select Case Key.KESSAI_KBN
                                Case "04"    ' 別段出金のみ

                                    ' 別段出金のデータ作成
                                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' 資金決済データのリスト作成
                                        lstKessaiData.Add(KData)
                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                        OP_Count += 1
                                        ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                    Else
                                        errFlg = True
                                    End If
                                Case Else
                                    errFlg = True
                                    errMsg &= "(決済区分)"
                            End Select

                        Case "1"    ' 直接入金
                            ' 決済区分の判定
                            Select Case Key.KESSAI_KBN
                                Case "00"    ' しんきん中金預け金

                                    ' 決済科目の判定
                                    Select Case Key.TUKEKAMOKU
                                        Case "99"   ' 諸勘定
                                            ' 別段出金のデータ作成
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' 諸勘定入金のデータ作成
                                            If errFlg = False AndAlso fn_SYOKANJO_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case Else   ' 上記以外の場合
                                            errFlg = True
                                            errMsg &= "(決済科目)"
                                    End Select

                                Case "01"    ' 口座入金

                                    ' 決済科目の判定
                                    Select Case Key.TUKEKAMOKU
                                        Case "02"   ' 普通

                                            ' 別段出金のデータ作成
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' 普通入金のデータ作成
                                            If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case "01"   ' 当座

                                            ' 別段出金のデータ作成
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' 当座入金のデータ作成
                                            If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case "04"   ' 別段

                                            ' 別段出金のデータ作成
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' 別段入金のデータ作成
                                            If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case Else   ' 上記以外の場合
                                            errFlg = True
                                            errMsg &= "(決済科目)"
                                    End Select

                                Case "02"    ' 為替振込
                                    ' 決済科目の判定
                                    Select Case Key.TUKEKAMOKU
                                        Case "01", "02"  ' 当座、普通

                                            ' 別段出金のデータ作成
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' 為替振込のデータ作成
                                            If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' 資金決済データのリスト作成
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                        Case KessaiKamokuCode_ETC   ' その他

                                            '手数料徴求区分=特別免除、別途徴求
                                            If Key.TESUUTYO_KBN = "2" OrElse Key.TESUUTYO_KBN = "3" Then
                                                ' 別段出金のデータ作成
                                                If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' 資金決済データのリスト作成
                                                    lstKessaiData.Add(KData)
                                                    OP_Count += 1
                                                Else
                                                    errFlg = True
                                                End If

                                                ' 為替振込のデータ作成
                                                If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' 資金決済データのリスト作成
                                                    lstKessaiData.Add(KData)
                                                    OP_Count += 1
                                                Else
                                                    errFlg = True
                                                End If
                                            Else
                                                errFlg = True
                                                errMsg &= "(手数料徴求区分)"
                                            End If
                                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                        Case Else   ' 上記以外の場合
                                            errFlg = True
                                            errMsg &= "(決済科目)"
                                    End Select

                                Case "03"    ' 為替付替
                                    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                                        errFlg = True
                                        errMsg &= "(決済科目)"
                                    Else
                                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' 為替付替のデータ作成
                                        If errFlg = False AndAlso fn_KAWASE_TUKEKAE(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    End If
                                '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                Case "04"    ' 別段出金のみ ※他の区分は気にせず作成？

                                    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                                        errFlg = True
                                        errMsg &= "(決済科目)"
                                    Else
                                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END


                                        ' 別段出金のデータ作成
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' 資金決済データのリスト作成
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- START
                                    End If
                                '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）----------------- END

                                Case "05"    ' 特別企業
                                    ' 処理対象外

                                Case "99"    ' 決済対象外
                                    ' 処理対象外

                                Case Else   ' 上記以外の場合
                                    errFlg = True
                                    errMsg &= "(決済区分)"
                            End Select

                        Case Else   ' 上記以外の場合
                            errFlg = True
                            errMsg &= "(手数料徴求方法)"
                    End Select

                End If

                '***手数料のみの電文は作成しない 2009.10.28 start
                '' 手数料徴求予定日が処理対象日の場合
                'If Key.TESUU_YDATE = ParaKessaiDate Then

                '    ' 一括徴求、かつ、資金決済と手数料徴求を同時に行う先の場合、資金決済データは作成しない
                '    ' （一括徴求の手数料は、手数料徴求だけ行う先にて集計済み）
                '    If Key.TESUUTYO_KBN = "1" And Key.KESSAI_TORI_KBN = "0" Then
                '        TesuuData_Flg = False
                '    Else
                '        TesuuData_Flg = True
                '    End If

                '    If TesuuData_Flg = True Then

                '        ' 決済区分の判定
                '        Select Case Key.KESSAI_KBN
                '            Case "00"    ' しんきん中金預け金
                '                errFlg = True
                '                errMsg &= "(決済区分)"

                '            Case "01"    ' 口座入金

                '                ' 手数料徴求方法の判定
                '                Select Case Key.TESUUTYO_PATN
                '                    Case "0"    ' 差引入金
                '                        errFlg = True
                '                        errMsg &= "(手数料徴求方法)"

                '                    Case "1"    ' 直接入金

                '                        ' 決済科目の判定
                '                        Select Case Key.TUKEKAMOKU
                '                            Case "02"   ' 普通

                '                                ' 振替済金額と手数料金額の差額が０円超の場合のみ、手数料データ作成
                '                                If strKEKKA <> "金額０" Then
                '                                    Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                '                                        Case "1"
                '                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                        Case "2"
                '                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If
                '                                        Case Else
                '                                            errFlg = True
                '                                            errMsg &= "(手数料徴求オペ区分(iniファイル))"
                '                                    End Select
                '                                End If

                '                            Case "01"   ' 当座

                '                                ' 振替済金額と手数料金額の差額が０円超の場合のみ、手数料データ作成
                '                                If strKEKKA <> "金額０" Then
                '                                    Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                '                                        Case "1"
                '                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                        Case "2"
                '                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If
                '                                        Case Else
                '                                            errFlg = True
                '                                            errMsg &= "(手数料徴求オペ区分(iniファイル))"
                '                                    End Select
                '                                End If

                '                            Case "04"   ' 別段

                '                                ' 振替済金額と手数料金額の差額が０円超の場合のみ、手数料データ作成
                '                                If strKEKKA <> "金額０" Then
                '                                    Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                '                                        Case "1"
                '                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                        Case "2"
                '                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' 資金決済データのリスト作成
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If
                '                                        Case Else
                '                                            errFlg = True
                '                                            errMsg &= "(手数料徴求オペ区分(iniファイル))"
                '                                    End Select
                '                                End If

                '                            Case Else ' 上記以外の場合
                '                                errFlg = True
                '                                errMsg &= "(決済科目)"
                '                        End Select

                '                    Case Else   ' 上記以外の場合
                '                        errFlg = True
                '                        errMsg &= "(手数料徴求方法)"
                '                End Select

                '            Case "02"    '為替振込
                '                errFlg = True
                '                errMsg &= "(決済区分)"

                '            Case "03"    ' 為替付替
                '                errFlg = True
                '                errMsg &= "(決済区分)"

                '            Case "04"    ' 別段出金のみ
                '                errFlg = True
                '                errMsg &= "(決済区分)"

                '            Case "05"    ' 特別企業
                '                ' 処理対象外

                '            Case "99"    ' 決済対象外
                '                ' 処理対象外

                '            Case Else   ' 上記以外の場合
                '                errFlg = True
                '                errMsg &= "(決済区分)"
                '        End Select

                '    End If
                'End If


                'ElseIf Key.TESUUTYO_KBN = "3" Then

                ''************************************************
                '' 決済予定日が処理対象日の場合
                'If Key.KESSAI_YDATE = ParaKessaiDate Then
                '    ' 決済区分の判定
                '    Select Case Key.KESSAI_KBN
                '        Case "00"    ' しんきん中金預け金

                '            '*** 修正 mitsu 2008/08/14 しんきん中金預け金対応 ***
                '            ' 決済科目の判定
                '            Select Case Key.TUKEKAMOKU
                '                Case "99"   ' 諸勘定
                '                    ' 別段出金のデータ作成
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' 諸勘定入金のデータ作成
                '                    If errFlg = False AndAlso fn_SYOKANJO_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case Else   ' 上記以外の場合
                '                    errFlg = True
                '                    errMsg &= "(決済科目)"
                '            End Select
                '            '****************************************************

                '        Case "01"    ' 口座入金

                '            ' 決済科目の判定
                '            Select Case Key.TUKEKAMOKU
                '                Case "02"   ' 普通

                '                    ' 別段出金のデータ作成
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' 普通入金のデータ作成
                '                    If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case "01"   ' 当座

                '                    ' 別段出金のデータ作成
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' 当座入金のデータ作成
                '                    If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case "04"   ' 別段

                '                    ' 別段出金のデータ作成
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' 別段入金のデータ作成
                '                    If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case Else   ' 上記以外の場合
                '                    errFlg = True
                '                    errMsg &= "(決済科目)"
                '            End Select

                '        Case "02"    ' 為替振込
                '            ' 決済科目の判定
                '            Select Case Key.TUKEKAMOKU
                '                Case "01", "02"  ' 当座、普通

                '                    ' 別段出金のデータ作成
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' 為替振込のデータ作成
                '                    If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' 資金決済データのリスト作成
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case Else   ' 上記以外の場合
                '                    errFlg = True
                '                    errMsg &= "(決済科目)"
                '            End Select

                '        Case "03"    ' 為替付替
                '            '*** 修正 mitsu 2008/06/25 科目の判定はしない ***
                '            '' 決済科目の判定
                '            'Select Case Key.TUKEKAMOKU
                '            '    Case "01", "02"  ' 当座、普通
                '            '************************************************
                '            ' 別段出金のデータ作成
                '            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                ' 資金決済データのリスト作成
                '                lstKessaiData.Add(KData)
                '            Else
                '                errFlg = True
                '            End If

                '            ' 為替付替のデータ作成
                '            If errFlg = False AndAlso fn_KAWASE_TUKEKAE(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                ' 資金決済データのリスト作成
                '                lstKessaiData.Add(KData)
                '            Else
                '                errFlg = True
                '            End If

                '        Case "04"    ' 別段出金のみ ※他の区分は気にせず作成？

                '            ' 別段出金のデータ作成
                '            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                ' 資金決済データのリスト作成
                '                lstKessaiData.Add(KData)
                '            Else
                '                errFlg = True
                '            End If

                '        Case "05"    ' 特別企業
                '            ' 処理対象外

                '        Case "99"    ' 決済対象外
                '            ' 処理対象外

                '        Case Else   ' 上記以外の場合
                '            errFlg = True
                '            errMsg &= "(決済区分)"
                '    End Select
                'End If
                ''********************************************************************************
                '***手数料のみの電文は作成しない 2009.10.28 end

            Else ' 上記以外の場合
                errFlg = True
                errMsg &= "(手数料徴求区分)"
            End If

            '*** 修正 mitsu 2008/05/30 手数料徴求区分判定コメントアウト ***
            '    Case "2"    ' 特別免除の場合
            '' 処理対象外

            '    Case "3"    ' 別途徴求の場合
            '' 処理対象外

            '    Case Else   ' 上記以外の場合
            'errFlg = True
            'errMsg &= "(手数料徴求区分)"
            'End Select
            '**************************************************************

            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- START
            If strKEKKA <> "金額０" Then
                MData.msg_DATA = ""
            Else
                MData.msg_DATA = "***手数料未徴求***"
            End If

            For cnt As Integer = 1 To OP_Count
                lstMsgData.Add(MData)
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                lstHimokuName.Add("")
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
            Next
            ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-12(RSV2対応<小浜信金>) -------------------- END

            If errFlg = True Then
                jobMessage = errMsg & " 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE &
                    " 手数料徴求区分：" & Key.TESUUTYO_KBN & " 決済区分：" & Key.KESSAI_KBN & " 手数料徴求方法：" & Key.TESUUTYO_PATN & " 決済科目：" & Key.TUKEKAMOKU
                MainLOG.Write("資金決済データ取得処理(企業自振用)", "失敗", jobMessage)
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("資金決済データ取得処理(企業自振用)", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 資金決済データ取得処理(学校自振用)
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
    Private Function fn_GetGakKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByVal lstHimokuData As ArrayList, _
                                        ByRef lstMsgData As ArrayList, ByRef lstHimokuName As ArrayList) As Boolean
        'Private Function fn_GetGakKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByVal lstHimokuData As ArrayList) As Boolean
        '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END

        Dim errFlg As Boolean = False
        Dim errMsg As String = "決済情報に誤りがあります。"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing
        Dim HimoKey As KeyInfo
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
        Dim MData As msgDATA = Nothing
        Dim OP_Count As Integer = 0
        lstMsgData = New ArrayList(128)
        lstHimokuName = New ArrayList(128)
        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

        strKEKKA = ""
        lstKessaiData = New ArrayList(128)

        Try
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            MData.Init()
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

            ' 手数料徴求済フラグを初期化する
            Key.TESUUTYO_FLG = "0"

            ' 手数料徴求区分が 0:都度徴求、かつ、決済予定日と手数料徴求予定日が処理対象日の場合
            If Key.TESUUTYO_KBN = "0" And Key.KESSAI_YDATE = ParaKessaiDate And Key.TESUU_YDATE = ParaKessaiDate Then

                ' 決済区分の判定
                Select Case Key.KESSAI_KBN
                    Case "00"    ' しんきん中金預け金
                        errFlg = True
                        errMsg &= "(決済区分)"

                    Case "01"    ' 口座入金

                        ' 手数料徴求方法の判定
                        Select Case Key.TESUUTYO_PATN
                            Case "1"    ' 直接入金

                                ' 別段出金のデータ作成
                                If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                    ' 資金決済データのリスト作成
                                    lstKessaiData.Add(KData)
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                    OP_Count += 1
                                    lstHimokuName.Add("")
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                Else
                                    errFlg = True
                                End If

                                For Count As Integer = 0 To lstHimokuData.Count - 1
                                    HimoKey = CType(lstHimokuData.Item(Count), KeyInfo)

                                    ' 費目口座単位の決済情報を取得
                                    Key.TUKEKIN_NO = HimoKey.KESSAI_KIN_CODE
                                    Key.TUKESIT_NO = HimoKey.KESSAI_TENPO
                                    Key.TUKEKAMOKU = HimoKey.KESSAI_KAMOKU
                                    Key.KESSAI_MEIGI = HimoKey.KESSAI_MEIGI
                                    Key.TUKEKOUZA = HimoKey.KESSAI_KOUZA
                                    Key.FURI_KIN = HimoKey.HIMOKU_FURI_KIN
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                    Key.HIMOKU_NAME = HimoKey.HIMOKU_NAME
                                    Key.HIMOKU_GASSAN = HimoKey.HIMOKU_GASSAN
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

                                    '金融機関コードで口座入金か為替振込か決定
                                    Select Case Key.TUKEKIN_NO
                                        Case ini_info.JIKINKO_CODE      '口座入金
                                            ' 決済科目の判定
                                            Select Case Key.TUKEKAMOKU
                                                Case "02"   ' 普通

                                                    ' 普通入金のデータ作成
                                                    If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' 資金決済データのリスト作成
                                                        lstKessaiData.Add(KData)
                                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                        OP_Count += 1
                                                        lstHimokuName.Add(Key.HIMOKU_NAME)
                                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                    Else
                                                        errFlg = True
                                                    End If

                                                Case "01"   ' 当座

                                                    ' 当座入金のデータ作成
                                                    If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' 資金決済データのリスト作成
                                                        lstKessaiData.Add(KData)
                                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                        OP_Count += 1
                                                        lstHimokuName.Add(Key.HIMOKU_NAME)
                                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                    Else
                                                        errFlg = True
                                                    End If

                                                Case Else   ' 上記以外の場合
                                                    errFlg = True
                                                    errMsg &= "(決済科目)"
                                            End Select
                                        Case Else                       '為替振込

                                            ' 決済科目の判定
                                            Select Case Key.TUKEKAMOKU
                                                Case "01", "02"  ' 当座、普通
                                                    ' 為替振込のデータ作成
                                                    If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' 資金決済データのリスト作成
                                                        lstKessaiData.Add(KData)
                                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                        OP_Count += 1
                                                        lstHimokuName.Add(Key.HIMOKU_NAME)
                                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                    Else
                                                        errFlg = True
                                                    End If

                                                Case Else   ' 上記以外の場合
                                                    errFlg = True
                                                    errMsg &= "(決済科目)"
                                            End Select

                                    End Select
                                Next

                                ' 振替済金額と手数料金額の差額が０円超の場合のみ、手数料データ作成
                                If strKEKKA <> "金額０" Then

                                    Select Case ini_info.TESUU_OPEKBN   '1:手数料徴求　2:諸勘定

                                        Case "1"
                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' 資金決済データのリスト作成
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If

                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                            If CLng(Key.TESUU_KIN3) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' 資金決済データのリスト作成
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If

                                        Case "2"
                                            ' 自振手数料が０円超の場合のみ、自振手数料徴求（連動）のデータ作成
                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' 資金決済データのリスト作成
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If

                                            ' 振込手数料が０円超の場合のみ、振込手数料徴求（連動）のデータ作成
                                            If CLng(Key.TESUU_KIN3) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' 資金決済データのリスト作成
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If
                                        Case Else
                                            errFlg = True
                                            errMsg &= "(手数料徴求オペ区分(iniファイル))"
                                    End Select
                                End If

                            Case Else   ' 上記以外の場合
                                errFlg = True
                                errMsg &= "(手数料徴求方法)"
                        End Select

                    Case "02"    ' 為替振込
                        errFlg = True
                        errMsg &= "(決済区分)"

                    Case "03"    ' 為替付替
                        errFlg = True
                        errMsg &= "(決済区分)"

                    Case "04"    ' 別段出金のみ 
                        errFlg = True
                        errMsg &= "(決済区分)"

                    Case "05"    ' 特別企業
                        ' 処理対象外

                    Case "99"    ' 決済対象外
                        ' 処理対象外

                    Case Else   ' 上記以外の場合
                        errFlg = True
                        errMsg &= "(決済区分)"
                End Select


            ElseIf Key.TESUUTYO_KBN = "0" OrElse Key.TESUUTYO_KBN = "1" OrElse Key.TESUUTYO_KBN = "2" OrElse Key.TESUUTYO_KBN = "3" Then

                ' 決済予定日が処理対象日の場合
                If Key.KESSAI_YDATE = ParaKessaiDate Then
                    ' 決済区分の判定
                    Select Case Key.KESSAI_KBN
                        Case "00"    ' しんきん中金預け金
                            errFlg = True
                            errMsg &= "(決済区分)"

                        Case "01"    ' 口座入金

                            ' 手数料徴求方法の判定
                            Select Case Key.TESUUTYO_PATN
                                Case "1"    ' 直接入金

                                    ' 別段出金のデータ作成
                                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' 資金決済データのリスト作成
                                        lstKessaiData.Add(KData)
                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                        OP_Count += 1
                                        lstHimokuName.Add("")
                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                    Else
                                        errFlg = True
                                    End If

                                    For Count As Integer = 0 To lstHimokuData.Count - 1
                                        HimoKey = CType(lstHimokuData.Item(Count), KeyInfo)

                                        ' 費目口座単位の決済情報を取得
                                        Key.TUKEKIN_NO = HimoKey.KESSAI_KIN_CODE
                                        Key.TUKESIT_NO = HimoKey.KESSAI_TENPO
                                        Key.TUKEKAMOKU = HimoKey.KESSAI_KAMOKU
                                        Key.KESSAI_MEIGI = HimoKey.KESSAI_MEIGI
                                        Key.TUKEKOUZA = HimoKey.KESSAI_KOUZA
                                        Key.FURI_KIN = HimoKey.HIMOKU_FURI_KIN
                                        '************************************************
                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                        Key.HIMOKU_NAME = HimoKey.HIMOKU_NAME
                                        Key.HIMOKU_GASSAN = HimoKey.HIMOKU_GASSAN
                                        '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

                                        '金融機関コードで口座入金か為替振込か決定
                                        Select Case Key.TUKEKIN_NO
                                            Case ini_info.JIKINKO_CODE      '口座入金
                                                ' 決済科目の判定
                                                Select Case Key.TUKEKAMOKU
                                                    Case "02"   ' 普通

                                                        ' 普通入金のデータ作成
                                                        If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                            OP_Count += 1
                                                            lstHimokuName.Add(Key.HIMOKU_NAME)
                                                            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                        Else
                                                            errFlg = True
                                                        End If

                                                    Case "01"   ' 当座

                                                        ' 当座入金のデータ作成
                                                        If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                            OP_Count += 1
                                                            lstHimokuName.Add(Key.HIMOKU_NAME)
                                                            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                        Else
                                                            errFlg = True
                                                        End If

                                                    Case Else   ' 上記以外の場合
                                                        errFlg = True
                                                        errMsg &= "(決済科目)"
                                                End Select
                                            Case Else                       '為替振込

                                                ' 決済科目の判定
                                                Select Case Key.TUKEKAMOKU
                                                    Case "01", "02"  ' 当座、普通
                                                        ' 為替振込のデータ作成
                                                        If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' 資金決済データのリスト作成
                                                            lstKessaiData.Add(KData)
                                                            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                                            OP_Count += 1
                                                            lstHimokuName.Add(Key.HIMOKU_NAME)
                                                            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
                                                        Else
                                                            errFlg = True
                                                        End If

                                                    Case Else   ' 上記以外の場合
                                                        errFlg = True
                                                        errMsg &= "(決済科目)"
                                                End Select

                                        End Select
                                    Next

                                Case Else   ' 上記以外の場合
                                    errFlg = True
                                    errMsg &= "(手数料徴求方法)"
                            End Select

                        Case "02"    ' 為替振込
                            errFlg = True
                            errMsg &= "(決済区分)"

                        Case "03"    ' 為替付替
                            errFlg = True
                            errMsg &= "(決済区分)"

                        Case "04"    ' 別段出金のみ
                            errFlg = True
                            errMsg &= "(決済区分)"

                        Case "05"    ' 特別企業
                            ' 処理対象外

                        Case "99"    ' 決済対象外
                            ' 処理対象外

                        Case Else   ' 上記以外の場合
                            errFlg = True
                            errMsg &= "(決済区分)"
                    End Select
                End If

            Else ' 上記以外の場合
                errFlg = True
                errMsg &= "(手数料徴求区分)"
            End If

            '*** 修正 mitsu 2008/05/30 手数料徴求区分判定コメントアウト ***
            '    Case "2"    ' 特別免除の場合
            '' 処理対象外

            '    Case "3"    ' 別途徴求の場合
            '' 処理対象外

            '    Case Else   ' 上記以外の場合
            'errFlg = True
            'errMsg &= "(手数料徴求区分)"
            'End Select
            '**************************************************************

            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            If strKEKKA <> "金額０" Then
                MData.msg_DATA = ""
            Else
                MData.msg_DATA = "***手数料未徴求***"
            End If

            For cnt As Integer = 1 To OP_Count
                lstMsgData.Add(MData)
            Next
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

            ' エラーフラグがＯＮの場合
            If errFlg = True Then
                jobMessage = errMsg & " 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE &
                    " 手数料徴求区分：" & Key.TESUUTYO_KBN & " 決済区分：" & Key.KESSAI_KBN & " 手数料徴求方法：" & Key.TESUUTYO_PATN & " 決済科目：" & Key.TUKEKAMOKU
                MainLOG.Write("資金決済データ取得処理(学校自振用)", "失敗", jobMessage)
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("資金決済データ取得処理(学校自振用)", "失敗", ex.Message)
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
    Private Function fn_KessaiData(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean

        Dim strKamokuOpeCode As String
        Dim strTUKEKIN_NO As String
        Dim strTUKESIT_NO As String
        Dim strHONBUCD As String

        Try
            strKamokuOpeCode = KData.OpeCode

            ' 科目が 48100:為替振替、又は、48500:為替付替の場合のみ設定（為替以外は初期値）
            If strKamokuOpeCode = "48100" Or strKamokuOpeCode = "48500" Then
                strTUKEKIN_NO = Key.TUKEKIN_NO          ' 受信金融機関コード
                strTUKESIT_NO = Key.TUKESIT_NO          ' 受信店番
                strHONBUCD = ini_info.HONBU_CODE              ' 発信店番
            Else
                strTUKEKIN_NO = "".PadLeft(4, "0"c)     ' 受信金融機関コード
                strTUKESIT_NO = "".PadLeft(3, "0"c)     ' 受信店番
                strHONBUCD = "".PadLeft(3, "0"c)        ' 発信店番
            End If

            ' データ設定
            With KData
                .KessaiDate = ParaKessaiDate                            '決済日
                .TorisCode = Key.TORIS_CODE                              '取引先主コード
                .TorifCode = Key.TORIF_CODE                              '取引先副コード
                .ToriKName = Key.ITAKU_KNAME
                .ToriNName = Key.ITAKU_NNAME
                .FuriCode = Key.FURI_CODE                                '振替コード
                .KigyoCode = Key.KIGYO_CODE                              '企業コード
                .FuriDate = Key.FURI_DATE                                '振替日
                .KessaiKbn = Key.KESSAI_KBN                              '決済区分
                .KesKinCode = Key.TUKEKIN_NO                        '決済金融機関コード
                .KesSitCode = Key.TUKESIT_NO                           '決済支店コード
                .KesKamoku = Key.TUKEKAMOKU                           '決済科目
                .KesKouza = Key.TUKEKOUZA                             '決済口座番号
                .TesTyoKbn = Key.TESUUTYO_KBN                            '手数料徴求区分
                .TesTyohh = Key.TESUUTYO_PATN                            '手数料徴求方法
                .TorimatomeSit = Key.TORIMATOME_SIT_NO                   'とりまとめ店コード
                .SyoriKen = Key.SYORI_KEN                                   '請求件数
                .Syorikin = Key.SYORI_KIN                                   '請求金額
                .FunouKen = Key.FUNOU_KEN                                   '不能件数
                .FunouKin = Key.FUNOU_KIN                                   '不能金額
                .FuriKen = Key.FURI_KEN                                     '振替件数
                .FuriKin = Key.FURI_KIN                                     '振替金額
                .TesuuKin = Key.TESUU_KIN                                   '手数料
                .JifutiTesuuKin = Key.TESUU_KIN1                            '自振手数料
                .FurikomiTesuukin = Key.TESUU_KIN3                          '振込手数料
                .SonotaTesuuKin = Key.TESUU_KIN2                            'その他手数料
                .NyukinKen = Key.FURI_KEN                               '入金件数
                .NyukinKin = ""                                         '入金金額
                .ToriKbn = Key.KESSAI_TORI_KBN
                .TesuuTyoFlg = Key.TESUUTYO_FLG
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                .HonbuKouza = Key.HONBU_KOUZA                               '本部別段口座番号
                '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
            End With

            ' 固定長に変換する
            KData.Data = KData.Data

        Catch ex As Exception
            MainLOG.Write("資金決済データ作成", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 別段出金データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： 0 - 金額０円超，1 - 金額０円，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_BETUDAN_OUT(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim BetudanOutFmt As New CAstFormKes.ClsFormSikinFuri.T_04099
        Dim strKin As String
        Dim strSagaku As String
        Dim strTEKIYOU As String

        Try
            ' 初期化
            BetudanOutFmt.Init()

            ' データチェック
            If Key.HONBU_KOUZA = "" OrElse CInt(Key.HONBU_KOUZA) = 0 Then
                '未設定の場合、又は、オール０の場合、エラーとする
                MainLOG.Write("別段出金データ作成", "失敗", "別段口座番号が設定されていません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 摘要の設定
            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
            '決済種別が 1：費目口座単位請求 の対応を追加
            If Key.KESSAI_SYUBETU = "0" Then
                ' 名義人カナが設定済みの場合、名義人カナを優先
                If Key.TUKEMEIGI <> "" Then
                    strTEKIYOU = Key.TUKEMEIGI
                Else
                    strTEKIYOU = Key.ITAKU_KNAME
                End If
            Else
                ' 決済種別が 1：費目口座単位請求 の場合文言は固定
                strTEKIYOU = Key.FURI_DATE.Substring(4, 2) & "/" & Key.FURI_DATE.Substring(6, 2) & "*****1ｻｷ"
            End If
            '' 名義人カナが設定済みの場合、名義人カナを優先
            'If Key.TUKEMEIGI <> "" Then
            '    strTEKIYOU = Key.TUKEMEIGI
            'Else
            '    strTEKIYOU = Key.ITAKU_KNAME
            'End If
            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END

            If strTEKIYOU = "" Then
                MainLOG.Write("別段出金データ作成", "失敗", "摘要がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            '振込済金額と手数料額の差額
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' 手数料差引金額が０円以下となる場合は手数料を差し引かない
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "金額０"
            End If

            ' 金額の設定
            ' 都度徴求、かつ、差引入金以外は出金額に手数料を差し引かない
            '99-019 の場合は差し引かない 2009.10.30
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" And ini_info.TESUU_OPEKBN = "1" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' データ設定
            With BetudanOutFmt
                .KOUZA_NO = Key.HONBU_KOUZA.PadLeft(7, "0"c)        ' 口座番号
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' 金額
                .FURI_CODE = ""                                     ' 振替コード
                .KIGYO_CODE = ""                                    ' 企業コード
                .TEKIYOU = strTEKIYOU                               ' 摘要
                .TORIATUKAI1 = ""                                   ' 取扱番号１
                '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------START
                '件数追加
                Select Case Key.SEIKYU_KBN
                    Case "0"
                        '請求分徴求
                        .KENSU = Key.SYORI_KEN.PadLeft(4, " "c)
                    Case "1"
                        '振替分徴求
                        .KENSU = Key.FURI_KEN.PadLeft(4, " "c)
                    Case Else
                        'その他
                        .KENSU = ""
                End Select
                '.KENSU = ""                                         ' 件数
                '2011/06/16 標準版修正 手数料請求区分から件数取得 ------------------END
                .TEGATA_NO = ""                                     ' 手形小切手番号
                .GENTEN_NO = ""                                     ' 原店番号
                .KISANBI = ""                                       ' 起算日
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = BetudanOutFmt.Data
            KData.OpeCode = String.Concat(BetudanOutFmt.KAMOKU_CODE, BetudanOutFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("別段出金データ作成", "失敗", ex.Message)

            Return -1
        End Try

        If strKEKKA = "金額０" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' 機能　 ： 当座入金データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： 0 - 金額０円超，1 - 金額０円，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_TOUZA_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim TouzaInFmt As New CAstFormKes.ClsFormSikinFuri.T_01010
        Dim strKin As String
        Dim strSagaku As String
        Dim strGENTEN_NO As String
        Dim strTEKIYOU As String
        Dim strFURIKOMI_IRAI As String

        Try
            ' 初期化
            TouzaInFmt.Init()

            ' データチェック
            If Key.TUKEKOUZA = "" OrElse CInt(Key.TUKEKOUZA) = 0 Then
                MainLOG.Write("当座入金データ作成", "失敗", "口座番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If Key.FURI_DATE.Length <> 8 Or CLng(Key.FURI_KEN) = 0 Then
                MainLOG.Write("当座入金データ作成", "失敗", "摘要がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If Key.TUKESIT_NO.Trim = "" OrElse CInt(Key.TUKESIT_NO) = 0 Then
                MainLOG.Write("当座入金データ作成", "失敗", "原店番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            '振込済金額と手数料額の差額
            ' 決済種別が 1：費目口座単位請求 の場合、手数料との差額は算出しない
            If Key.KESSAI_SYUBETU = "0" Then
                strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))
            Else
                strSagaku = Key.FURI_KIN
            End If

            ' 手数料差引金額が０円以下となる場合は手数料を差し引かない
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "金額０"
            End If

            ' 金額の設定
            ' 都度徴求、かつ、差引入金以外は入金額に手数料を差し引かない
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' 摘要の設定
            If Key.KESSAI_SYUBETU = "0" Then
                ' 決済種別が 0：委託者一括請求 の場合
                strTEKIYOU = "ｺｳﾌﾘ" & Key.FURI_KEN.PadRight(6, " "c).Trim & "ｹﾝ"
            Else

                ' 決済種別が 1：費目口座単位請求 の場合
                strTEKIYOU = Key.KESSAI_MEIGI
            End If

            ' 振込依頼人名の設定
            Select Case Key.FURI_CODE
                Case "040", "041", "042"
                    strFURIKOMI_IRAI = ""
                Case Else
                    strFURIKOMI_IRAI = Key.TUKEMEIGI
            End Select

            ' 原店番号の設定
            ' 決済支店とiniファイルの本部コードを比較
            '   一致する場合：空白
            '   異なる場合  ：決済支店を設定
            If Key.TUKESIT_NO = ini_info.HONBU_CODE Then
                strGENTEN_NO = ""
            Else
                strGENTEN_NO = Key.TUKESIT_NO.PadLeft(3, "0"c)
            End If

            ' データ設定
            With TouzaInFmt
                .KOUZA_NO = Key.TUKEKOUZA.PadLeft(7, "0"c)          ' 口座番号
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' 金額
                .SIKINKA_KBN = ""                                   ' 資金化区分コード
                .TATEN_TEKIYOU = ""                                 ' 他店券摘要
                .FURI_CODE = ""                                     ' 振替コード
                .TEKIYOU = strTEKIYOU                               ' 摘要
                .TESUU_KBN = ""                                     ' 手数料徴求区分
                .TESUU_KINGAKU = ""                                 ' 手数料額
                .KISANBI = ""                                       ' 起算日
                .SAKIHIDUKE_YOTEI = ""                              ' 先日付予定日
                .FURIKOMI_IRAI = ""                                 ' 振込依頼人名
                .GENTEN_NO = strGENTEN_NO                           ' 原店番号
                .KINGAKU1 = ""                                      ' 金額１
                .SIKINKA_KBN1 = ""                                  ' 資金化区分コード１
                .TATEN_TEKIYOU1 = ""                                ' 他店券摘要１
                .KINGAKU2 = ""                                      ' 金額２
                .SIKINKA_KBN2 = ""                                  ' 資金化区分コード２
                .TATEN_TEKIYO2 = ""                                 ' 他店券摘要２
                .KINGAKU3 = ""                                      ' 金額３
                .SIKINKA_KBN3 = ""                                  ' 資金化区分コード３
                .TATEN_TEKIYO3 = ""                                 ' 他店券摘要３
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = TouzaInFmt.Data
            KData.OpeCode = String.Concat(TouzaInFmt.KAMOKU_CODE, TouzaInFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("当座入金データ作成", "失敗", ex.Message)
            Return -1
        End Try

        If strKEKKA = "金額０" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' 機能　 ： 普通入金データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： 0 - 金額０円超，1 - 金額０円，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_FUTUU_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim FutuuInFmt As New CAstFormKes.ClsFormSikinFuri.T_02019
        Dim strKin As String
        Dim strSagaku As String
        Dim strGENTEN_NO As String
        Dim strTEKIYOU As String
        Dim strFURIKOMI_IRAI As String

        Try
            ' 初期化
            FutuuInFmt.Init()

            ' データチェック
            If Key.TUKEKOUZA = "" OrElse CInt(Key.TUKEKOUZA) = 0 Then
                MainLOG.Write("普通入金データ作成", "失敗", "口座番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If Key.FURI_DATE.Length <> 8 OrElse CLng(Key.FURI_KEN) = 0 Then
                MainLOG.Write("普通入金データ作成", "失敗", "摘要がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If Key.TUKESIT_NO.Trim = "" Then
                MainLOG.Write("普通入金データ作成", "失敗", "原店番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            '振込済金額と手数料額の差額
            ' 決済種別が 1：費目口座単位請求 の場合、手数料との差額は算出しない
            If Key.KESSAI_SYUBETU = "0" Then
                strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))
            Else
                strSagaku = Key.FURI_KIN
            End If

            ' 手数料差引金額が０円以下となる場合は手数料を差し引かない
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "金額０"
            End If

            ' 金額の設定
            ' 都度徴求、かつ、差引入金以外は入金額に手数料を差し引かない
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' 摘要の設定
            If Key.KESSAI_SYUBETU = "0" Then
                ' 決済種別が 0：委託者一括請求 の場合
                strTEKIYOU = "ｺｳﾌﾘ" & Key.FURI_KEN.PadRight(6, " "c).Trim & "ｹﾝ"
            Else

                ' 決済種別が 1：費目口座単位請求 の場合
                strTEKIYOU = Key.KESSAI_MEIGI
            End If

            ' 振込依頼人名の設定
            Select Case Key.FURI_CODE
                Case "040", "041", "042"
                    strFURIKOMI_IRAI = ""
                Case Else
                    strFURIKOMI_IRAI = Key.TUKEMEIGI
            End Select

            ' 原店番号の設定
            ' 決済支店とiniファイルの本部コードを比較
            '   一致する場合：空白
            '   異なる場合  ：決済支店を設定
            If Key.TUKESIT_NO = ini_info.HONBU_CODE Then
                strGENTEN_NO = ""
            Else
                strGENTEN_NO = Key.TUKESIT_NO.PadLeft(3, "0"c)
            End If

            With FutuuInFmt
                .KOUZA_NO = Key.TUKEKOUZA.PadLeft(7, "0"c)          ' 口座番号
                .GYO = "01"                                         ' 行
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' 金額
                .SIKINKA_KBN = ""                                   ' 資金化区分コード
                .TATEN_TEKIYOU = ""                                 ' 他店券摘要
                .FURI_CODE = ""                                     ' 振替コード
                .TEKIYOU = strTEKIYOU                               ' 摘要
                .TESUU_KBN = ""                                     ' 手数料徴求区分
                .TESUU_KINGAKU = ""                                 ' 手数料額
                .KISANBI = ""                                       ' 起算日
                .SAKIHIDUKE_YOTEI = ""                              ' 先日付予定日
                .FURIKOMI_IRAI = ""                                 ' 振込依頼人名
                .GENTEN_NO = strGENTEN_NO                           ' 原店番号
                .KINGAKU1 = ""                                      ' 金額１
                .SIKINKA_KBN1 = ""                                  ' 資金化区分コード１
                .TATEN_TEKIYOU1 = ""                                ' 他店券摘要１
                .KINGAKU2 = ""                                      ' 金額２
                .SIKINKA_KBN2 = ""                                  ' 資金化区分コード２
                .TATEN_TEKIYO2 = ""                                 ' 他店券摘要２
                .KINGAKU3 = ""                                      ' 金額３
                .SIKINKA_KBN3 = ""                                  ' 資金化区分コード３
                .TATEN_TEKIYO3 = ""                                 ' 他店券摘要３
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = FutuuInFmt.Data
            KData.OpeCode = String.Concat(FutuuInFmt.KAMOKU_CODE, FutuuInFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("普通入金データ作成", "失敗", ex.Message)
            Return -1
        End Try

        If strKEKKA = "金額０" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' 機能　 ： 別段入金データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： 0 - 金額０円超，1 - 金額０円，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_BETUDAN_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim BetudanInFmt As New CAstFormKes.ClsFormSikinFuri.T_04019
        Dim strKin As String
        Dim strSagaku As String
        Dim strTEKIYOU As String
        Dim strGENTEN_NO As String

        Try
            ' 初期化
            BetudanInFmt.Init()

            ' データチェック
            If Key.TUKEKOUZA = "" OrElse CInt(Key.TUKEKOUZA) = 0 Then
                ' 未設定の場合、又は、オール０の場合、エラーとする。
                MainLOG.Write("別段入金データ作成", "失敗", "口座番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 摘要の設定
            ' 名義人カナが設定済みの場合、名義人カナを優先
            If Key.TUKEMEIGI <> "" Then
                strTEKIYOU = Key.TUKEMEIGI
            Else
                strTEKIYOU = Key.ITAKU_KNAME
            End If

            If strTEKIYOU = "" Then
                MainLOG.Write("別段入金データ作成", "失敗", "摘要がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If Key.TUKESIT_NO.Trim = "" Then
                MainLOG.Write("別段入金データ作成", "失敗", "原店番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            '振込済金額と手数料額の差額
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' 手数料差引金額が０円以下となる場合は手数料を差し引かない
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "金額０"
            End If

            ' 金額の設定
            ' 都度徴求、かつ、差引入金以外は入金金額に手数料を差し引かない
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' 原店番号の設定
            ' 決済支店とiniファイルの本部コードを比較
            '   一致する場合：空白
            '   異なる場合  ：決済支店を設定
            If Key.TUKESIT_NO = ini_info.HONBU_CODE Then
                strGENTEN_NO = ""
            Else
                strGENTEN_NO = Key.TUKESIT_NO.PadLeft(3, "0"c)
            End If

            ' データ設定
            With BetudanInFmt
                .KOUZA_NO = Key.TUKEKOUZA.PadLeft(7, "0"c)          ' 口座番号
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' 金額
                .SIKINKA_KBN = ""                                   ' 資金化区分コード
                .TATEN_TEKIYOU = ""                                 ' 他店券摘要
                .FURI_CODE = ""                                     ' 振替コード
                .KIGYO_CODE = ""                                    ' 企業コード
                .TEKIYOU = strTEKIYOU                               ' 摘要
                .TORIATUKAI1 = ""                                   ' 取扱番号２
                .KENSU = ""                                         ' 件数
                .MADOGUTI_KBN = ""                                  ' 窓口収納区分
                .INSI_KEN = ""                                      ' 印紙件数
                .TEGATA_NO = ""                                     ' 手形小切手番号
                .HAKKOU_NO = ""                                     ' 発行先顧客番号
                .TESUU_KBN = ""                                     ' 手数料徴求区分
                .TESUU_KINGAKU = ""                                 ' 手数料額
                .KISANBI = ""                                       ' 起算日
                .SAKIHIDUKE_YOTEI = ""                              ' 先日付予定日
                .GENTEN_NO = strGENTEN_NO                           ' 原店番号
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = BetudanInFmt.Data
            KData.OpeCode = String.Concat(BetudanInFmt.KAMOKU_CODE, BetudanInFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("別段入金データ作成", "失敗", ex.Message)
            Return -1
        End Try

        If strKEKKA = "金額０" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' 機能　 ： 諸勘定入金データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： 0 - 金額０円超，1 - 金額０円，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_SYOKANJO_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer
        '*** 修正 mitsu 2008/10/10 99-019→99-011に変更 99-019に変更RSV1***
        Dim SyokanjyoInFmt As New CAstFormKes.ClsFormSikinFuri.T_99019
        'Dim SyokanjyoInFmt As CAstFormKes.ClsFormSikinFuri.T_99011
        '**************************************************
        Dim strKin As String
        Dim strSagaku As String
        Dim strKOUZA_NO As String
        Dim strUTIWAKE_CODE As String
        Dim strTEKIYOU As String

        Try
            ' 初期化
            SyokanjyoInFmt.Init()

            ' データチェック
            If Key.TUKEKOUZA = "" Then
                MainLOG.Write("諸勘定入金データ作成", "失敗", "口座番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 口座番号の設定（ "00"＋決済口座番号の上５桁）
            strKOUZA_NO = "00" & Key.TUKEKOUZA.Substring(0, 5)

            If CInt(strKOUZA_NO) = 0 Then
                MainLOG.Write("諸勘定入金データ作成", "失敗", "口座番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 内訳コードの設定（決済口座番号の下２桁）
            strUTIWAKE_CODE = Key.TUKEKOUZA.Substring(5, 2)

            If CInt(strUTIWAKE_CODE) = 0 Then
                MainLOG.Write("諸勘定入金データ作成", "失敗", "内訳コードが設定されていません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If
            '2011/06/24 標準版修正 名義人カナが設定済みの場合、名義人カナを優先 ------------------START
            ' 摘要の設定
            ' 名義人カナが設定済みの場合、名義人カナを優先
            ' *** 修正 nishida 2008/10/14 摘要は委託者名を設定
            If Key.TUKEMEIGI <> "" Then
                strTEKIYOU = Key.TUKEMEIGI
            Else
                strTEKIYOU = Key.ITAKU_KNAME
            End If
            '**************************************************
            '2011/06/24 標準版修正 名義人カナが設定済みの場合、名義人カナを優先 ------------------END

            If strTEKIYOU = "" Then
                MainLOG.Write("諸勘定入金データ作成", "失敗", "摘要がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            '振込済金額と手数料額の差額
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' 手数料差引金額が０円以下となる場合は手数料を差し引かない
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "金額０"
            End If

            ' 金額の設定
            ' 都度徴求、かつ、差引入金以外は入金額に手数料を差し引かない
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            With SyokanjyoInFmt
                .KOUZA_NO = strKOUZA_NO                             ' 口座番号
                .GYO = "01"                                         ' 行
                .UTIWAKE_CODE = strUTIWAKE_CODE                     ' 内訳コード
                .ZENZAN = "0".PadLeft(15, " "c)                      ' 前残
                .FUGOU_CODE = "1"                                   ' 符号コード
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' 金額
                .TATEN_TEKIYOU = ""                                 ' 他店券摘要
                .KENSU = "1".PadLeft(5, " "c)                       ' 件数
                .FURI_CODE = ""                                     ' 振替コード
                .TORIATUKAI1 = ""                                   ' 取扱番号１
                .JINKAKU_CODE = ""                                  ' 人格コード
                .KAZEI_CODE = ""                                    ' 課税コード
                .TEKIYOU = strTEKIYOU                               ' 摘要
                .KISANBI = ""                                       ' 起算日
                .GENTEN_NO = ""                                     ' 原店番号
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = SyokanjyoInFmt.Data
            KData.OpeCode = String.Concat(SyokanjyoInFmt.KAMOKU_CODE, SyokanjyoInFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("諸勘定入金データ作成", "失敗", ex.Message)
            Return -1
        End Try

        If strKEKKA = "金額０" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' 機能　 ： 為替振込データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： 0 - 金額０円超，1 - 金額０円，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_KAWASE_FURIKOMI(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseFurikomiInFmt As New CAstFormKes.ClsFormSikinFuri.T_48100
        Dim strTUKEKIN_NO As String
        Dim strTUKESIT_NO As String
        Dim strKIN_NNM As String = ""                ' 金融機関漢字名
        Dim strSIT_NNM As String = ""                ' 支店漢字名
        Dim strKIN_KNM As String = ""                ' 金融機関カナ名
        Dim strSIT_KNM As String = ""               ' 支店カナ名
        '*** 修正　備考２とりまとめ店を設定するためのワークエリア　2008/11/07 NISHIDA
        Dim strKIN_NNM2 As String = ""                ' 金融機関漢字名
        Dim strSIT_NNM2 As String = ""                ' 支店漢字名
        Dim strKIN_KNM2 As String = ""                ' 金融機関カナ名
        Dim strSIT_KNM2 As String = ""                ' 支店カナ名
        '*****************************************************************************
        Dim strSYUMOKU As String
        Dim strJUSIN_TEN As String
        Dim strHASSIN_TEN As String
        Dim strKin As String
        Dim strSagaku As String
        Dim strKINGAKU_LOCAL As String          ' 金額
        Dim strKINFUKKI_FUGOU As String = ""         ' 金額複記符号 
        Dim strUKETORI_KAMOKU As String
        Dim strBIKOU1 As String = "" '2011/06/17
        Dim strBIKOU2 As String = "" '2011/06/17
        Dim strTEKIYOU As String

        Try
            ' 初期化
            KawaseFurikomiInFmt.Init()

            ' 受信店名の取得
            strTUKEKIN_NO = Key.TUKEKIN_NO     ' 決済金融機関
            strTUKESIT_NO = Key.TUKESIT_NO     ' 決済支店

            If GetTENMAST(strTUKEKIN_NO, strTUKESIT_NO, strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = False Then
                MainLOG.Write("為替振込データ作成", "失敗", "金融機関コード取込エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE & " 金融機関：" & Key.TUKEKIN_NO & Key.TUKESIT_NO)
                Return -1
            End If

            If Key.TUKEMEIGI = "" Then
                MainLOG.Write("為替振込データ作成", "失敗", "受取人名が設定されていません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 種目コードの設定
            ' 備考１の上６桁が「ﾟｺｳｷﾝﾟ」、「ﾟｺｸﾌﾘﾟ」の場合、空白に再設定
            strSYUMOKU = "1022"

            If Key.BIKOU1.Length >= 6 Then
                If Key.BIKOU1.Substring(0, 6) = "ﾟｺｳｷﾝﾟ" Then
                    strSYUMOKU = "1074"
                    Key.BIKOU1 = " "
                ElseIf Key.BIKOU1.Substring(0, 6) = "ﾟｺｸﾌﾘﾟ" Then
                    strSYUMOKU = "1054"
                    Key.BIKOU1 = " "
                End If
            End If

            ' 受信店名、発信店名の設定
            If strTUKEKIN_NO = ini_info.JIKINKO_CODE Then         ' 本支店為替の場合（INIファイルの自金庫コードと同一）
                strJUSIN_TEN = "ﾟ " & strSIT_KNM.Trim
                strHASSIN_TEN = "ﾟ ｾﾝﾀ-"
                strTEKIYOU = "ｺｳﾌﾘ" & Key.FURI_KEN.PadRight(6, " "c).Trim & "ｹﾝ"
            Else                                            ' 他行為替の場合
                strJUSIN_TEN = strKIN_KNM.Trim & " " & strSIT_KNM.Trim
                strHASSIN_TEN = ini_info.KAWASE_CENTER
                strTEKIYOU = ini_info.SIKINTEKIYOU & " ｺｳﾌﾘ" & Key.FURI_KEN.PadRight(6, " "c).Trim & "ｹﾝ"
            End If

            '振込済金額と手数料額の差額
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' 手数料差引金額が０円以下となる場合は手数料を差し引かない
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "金額０"
            End If

            ' 金額の設定
            ' 都度徴求、かつ、差引入金以外は入金額に手数料を引かない。
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' 金額をカンマ編集する
            strKINGAKU_LOCAL = CASTCommon.CADec(strKin).ToString("###,###,###,##0").PadLeft(10, " "c)

            ' 金額復記記号の取得
            If fn_FUGO_SETTEI(strKINGAKU_LOCAL, strKINFUKKI_FUGOU) = False Then
                MainLOG.Write("為替振込データ作成", "失敗", "複記符号設定処理エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            '2017/05/29 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
            ' 受取人科目コードの設定(全銀フォーマットに合わせる)
            ' 02：普通 → 1
            ' 01：当座 → 2
            ' xx：その他→? (その他のコード値は、Common_決済科目.TXTと科目変換テーブル.iniより取得する)
            '2017/07/25 タスク）綾部 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
            'strUKETORI_KAMOKU = ConvertKamoku2TO1(KessaiKamokuCode_ETC)
            '----------------------------------------------------
            ' 為替振込の「受取人科目」編集仕様変更
            '  『為替振込』時は「その他」の取扱も可能とする
            '  ・科目="01"(信金 当座預金)の場合、"2"(全銀当座)
            '  ・科目="02"(信金 普通預金)の場合、"1"(全銀普通)
            '  ・科目="xx"(信金 その他)の場合、　"x"(全銀その他)
            '  ・上記以外の科目の場合、　　　　　"2"(全銀普通)
            '　※その他科目に「xx」が設定されている場合は、
            '　　その他科目未設定とし、標準ロジックで処理を行う。
            '----------------------------------------------------
            If KessaiKamokuCode_ETC = "xx" Then
                Select Case Key.TUKEKAMOKU.PadLeft(2, "0"c)
                    Case "02"
                        strUKETORI_KAMOKU = "1"
                    Case Else
                        strUKETORI_KAMOKU = "2"
                End Select
            Else
                Select Case Key.TUKEKAMOKU.PadLeft(2, "0"c)
                    Case "01"
                        strUKETORI_KAMOKU = "2"
                    Case "02"
                        strUKETORI_KAMOKU = "1"
                    Case KessaiKamokuCode_ETC
                        strUKETORI_KAMOKU = ConvertKamoku2TO1(KessaiKamokuCode_ETC)
                    Case Else
                        strUKETORI_KAMOKU = "2"
                End Select
            End If
            '2017/07/25 タスク）綾部 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END
            '' 受取人科目コードの設定(全銀フォーマットに合わせる)
            '' 02：普通 → 1
            '' 01：当座 → 2
            'Select Case Key.TUKEKAMOKU.PadLeft(2, "0"c)
            '    Case "02"
            '        strUKETORI_KAMOKU = "1"
            '    Case Else
            '        strUKETORI_KAMOKU = "2"
            'End Select
            '2017/05/29 タスク）西野 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END

            ' 備考１の設定
            If fn_BIKOU1_SETTEI(Key, strBIKOU1, 29) = False Then
                MainLOG.Write("為替振込データ作成", "失敗", "備考１編集処理エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If


            '*** 修正 NISHIDA 2008/11/07 備考２に設定する扱店は自金庫とする
            If GetTENMAST(ini_info.JIKINKO_CODE, Key.TORIMATOME_SIT_NO, strKIN_NNM2, strSIT_NNM2, strKIN_KNM2, strSIT_KNM2) = False Then
                MainLOG.Write("為替振込データ作成", "失敗", "金融機関コード取込エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE & " 金融機関：" & ini_info.JIKINKO_CODE & Key.TORIMATOME_SIT_NO)
                Return -1
            End If
            ' 備考２の設定
            If fn_BIKOU2_SETTEI(Key, strBIKOU2, 29, strSIT_KNM2) = False Then
                '**************************************************************
                MainLOG.Write("為替振込データ作成", "失敗", "備考２編集処理エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' データ設定
            With KawaseFurikomiInFmt
                .TORIATUKAI = Key.KESSAI_YDATE                      ' 取扱日
                .SYUMOKU = strSYUMOKU                               ' 種目コード
                .JUSIN_TEN = strJUSIN_TEN                           ' 受信店名
                .FUKA_CODE = "".PadLeft(3, "0"c)                    ' 付加コード
                .HASSIN_TEN = strHASSIN_TEN                         ' 発信店名
                .KINGAKU = strKin.PadLeft(10, " "c)
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = "".PadLeft(1, " "c)                                    ' 決済回数
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.Trim.PadRight(15, " "c)   ' 金額複記符号
                .KOKYAKU_TESUU = "0"                                 ' 顧客手数料
                .UKETORI_KAMOKU = strUKETORI_KAMOKU                 ' 受取人科目コード
                .UKETORI_KOUZA = Key.TUKEKOUZA.PadLeft(7, "0"c).PadRight(15)    ' 受取人口座番号 
                .UKETORI_NAME = Key.TUKEMEIGI.Trim                  ' 受取人名
                .IRAI_NAME = ini_info.KAWASE_IRAININ                          ' 依頼人名
                .EDI_INFO = ""                                      ' EDI情報
                .BIKOU1 = strBIKOU1                                 ' 備考１
                .BIKOU2 = strBIKOU2                                 ' 備考２
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = KawaseFurikomiInFmt.Data
            KData.OpeCode = String.Concat(KawaseFurikomiInFmt.KAMOKU_CODE, KawaseFurikomiInFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("為替振込データ作成", "失敗", ex.Message)
            Return -1
        End Try

        If strKEKKA = "金額０" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' 機能　 ： 為替付替データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： 0 - 金額０円超，1 - 金額０円，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_KAWASE_TUKEKAE(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseTukekaeInFmt As New CAstFormKes.ClsFormSikinFuri.T_48500
        Dim strJUSIN_TEN As String
        Dim strHASSIN_TEN As String
        '2011/06/17 警告なくす
        Dim strKIN_NNM As String = ""                ' 金融機関漢字名
        Dim strSIT_NNM As String = ""                ' 支店漢字名
        Dim strKIN_KNM As String = ""                ' 金融機関カナ名
        Dim strSIT_KNM As String = ""                ' 支店カナ名
        '*** 修正　備考２とりまとめ店を設定するためのワークエリア　2008/11/07 NISHIDA
        '2011/06/17 
        Dim strKIN_NNM2 As String = ""                ' 金融機関漢字名
        Dim strSIT_NNM2 As String = ""                ' 支店漢字名
        Dim strKIN_KNM2 As String = ""             ' 金融機関カナ名
        Dim strSIT_KNM2 As String = ""                ' 支店カナ名
        '*****************************************************************************
        Dim strTUKEKIN_NO As String
        Dim strTUKESIT_NO As String
        Dim strKin As String
        Dim strSagaku As String
        Dim strKINGAKU_LOCAL As String          ' 金額
        Dim strKINFUKKI_FUGOU As String = ""         ' 金額複記符号'2011/06/17
        Dim strBANGOU As String
        Dim strBIKOU1 As String = ""
        Dim strBIKOU2 As String = ""
        Dim strTEKIYOU As String

        Try
            ' 初期化
            KawaseTukekaeInFmt.Init()

            ' 受信店名の取得
            strTUKEKIN_NO = Key.TUKEKIN_NO     ' 決済金融機関
            strTUKESIT_NO = Key.TUKESIT_NO     ' 決済支店

            If GetTENMAST(strTUKEKIN_NO, strTUKESIT_NO, strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = False Then
                MainLOG.Write("為替付替データ作成", "失敗", "金融機関コード取込エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE & " 金融機関：" & Key.TUKEKIN_NO & Key.TUKESIT_NO)
                Return -1
            End If

            If Key.TUKEMEIGI = "" Then
                MainLOG.Write("為替付替データ作成", "失敗", "受取人名（口座名義人名）が設定されていません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 受信店名、発信店名の設定
            If strTUKEKIN_NO = ini_info.JIKINKO_CODE Then         ' 本支店為替の場合（INIファイルの自金庫コードと同一）
                strJUSIN_TEN = "ﾟ " & strSIT_KNM.Trim
                strHASSIN_TEN = "ﾟ ｾﾝﾀ-"
                strTEKIYOU = "ｺｳﾌﾘ" & Key.FURI_KEN.PadRight(6, " "c).Trim & "ｹﾝ"
            Else                                            ' 他行為替の場合
                strJUSIN_TEN = strKIN_KNM.Trim & " " & strSIT_KNM.Trim
                strHASSIN_TEN = ini_info.KAWASE_CENTER
                strTEKIYOU = ini_info.SIKINTEKIYOU & " ｺｳﾌﾘ" & Key.FURI_KEN.PadRight(6, " "c).Trim & "ｹﾝ"
            End If

            '振込済金額と手数料額の差額
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' 手数料差引金額が０円以下となる場合は手数料を差し引かない
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "金額０"
            End If

            ' 金額の設定
            ' 都度徴求、かつ、差引入金以外は入金額に手数料を差し引かない
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' 金額をカンマ編集する
            strKINGAKU_LOCAL = CASTCommon.CADec(strKin).ToString("###,###,###,##0").PadLeft(10, " "c)

            ' 金額複記符号の取得
            If fn_FUGO_SETTEI(strKINGAKU_LOCAL, strKINFUKKI_FUGOU) = False Then
                MainLOG.Write("為替付替データ作成", "失敗", "複記符号設定処理エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 番号の設定
            '2018/11/21 タスク）綾部 CHG 標準版バグ対応(信金中金先の為替付替の設定値変更(文字列設定不可))  -------------------- START
            '--------------------------------------------------------------------------------
            ' 為替付替の「番号欄」は数値項目のため「ﾁｳｹｲ-1」はエラー、
            ' 数値のみが設定可能(番号欄には"1"を設定する)
            '--------------------------------------------------------------------------------
            'If Key.TUKEKIN_NO = "1000" Then             ' 全信連の場合
            '    strBANGOU = "ﾁｳｹｲ-1"
            'Else
            '    strBANGOU = ""                          ' 全信連以外の場合
            'End If
            Select Case Key.TUKEKIN_NO
                Case "1000"
                    '----------------------------------------
                    ' 決済金融機関が、全信連(信金中金)の場合
                    ' 「番号欄」は、"1"を設定
                    '----------------------------------------
                    strBANGOU = "1"
                Case Else
                    '----------------------------------------
                    ' 決済金融機関が、全信連以外の場合
                    ' 「番号欄」は、空白を設定
                    '----------------------------------------
                    strBANGOU = ""
            End Select
            '2018/11/21 タスク）綾部 CHG 標準版バグ対応(信金中金先の為替付替の設定値変更(文字列設定不可))  -------------------- END

            ' 備考１の設定
            If fn_BIKOU1_SETTEI(Key, strBIKOU1, 48) = False Then
                MainLOG.Write("為替付替データ作成", "失敗", "備考１編集処理エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If GetTENMAST(ini_info.JIKINKO_CODE, Key.TORIMATOME_SIT_NO, strKIN_NNM2, strSIT_NNM2, strKIN_KNM2, strSIT_KNM2) = False Then
                MainLOG.Write("為替振込データ作成", "失敗", "金融機関コード取込エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE & " 金融機関：" & ini_info.JIKINKO_CODE & Key.TORIMATOME_SIT_NO)
                Return -1
            End If

            ' 備考２の設定
            If fn_BIKOU2_SETTEI(Key, strBIKOU2, 48, strSIT_KNM2) = False Then
                '**************************************************************
                MainLOG.Write("為替付替データ作成", "失敗", "備考２編集処理エラー。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' データ設定
            With KawaseTukekaeInFmt
                .TORIATUKAI = Key.KESSAI_YDATE                      ' 取扱日
                .SYUMOKU = "4301"                                   ' 種目コード
                .JUSIN_TEN = strJUSIN_TEN                           ' 受信店名
                .FUKA_CODE = "".PadLeft(3, "0"c)                    ' 付加コード
                .HASSIN_TEN = strHASSIN_TEN                         ' 発信店名
                .KINGAKU = strKin.PadLeft(10, " "c)                 ' 金額
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = "".PadLeft(1, " "c)                   ' 決済回数
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.Trim.PadRight(15, " "c)  ' 金額複記符号
                .BANGOU = strBANGOU                                 ' 番号
                '2018/11/21 タスク）綾部 CHG 標準版バグ対応(信金中金先の為替付替の設定値変更(文字列設定不可))  -------------------- START
                '--------------------------------------------------------------------------------
                ' 為替付替の「番号欄」は数値項目のため「ﾁｳｹｲ-1」はエラー、
                ' 数値のみが設定可能(番号欄には"1"を設定する)
                '--------------------------------------------------------------------------------
                '.SIKIN_JIYUU1 = Key.TUKEMEIGI                       ' 資金付替事由1：受取人名（口座名義人名）
                ''*** 修正 nishida 2008/11/07 依頼人名変更 ｺｳﾌﾘ#####9ｹﾝ
                ''もとに戻す 2009.10.29
                '.SIKIN_JIYUU2 = ini_info.KAWASE_IRAININ                         ' 資金付替事由2：依頼人名（INIファイル)
                ''.SIKIN_JIYUU2 = strTEKIYOU
                ''*****************************************************************
                '.SIKIN_JIYUU3 = strBIKOU1                           ' 資金付替事由3：備考１
                '.SIKIN_JIYUU4 = strBIKOU2                           ' 資金付替事由4：備考２
                .SIKIN_JIYUU1 = Key.TUKEMEIGI                       ' 資金付替事由1：取引先マスタ「決済名義人（カナ）」
                .SIKIN_JIYUU2 = ini_info.KAWASE_IRAININ             ' 資金付替事由2：INIファイル 「依頼人名」([KESSAI]-[IRAININ])
                .SIKIN_JIYUU3 = strBIKOU1                           ' 資金付替事由3：取引先マスタ「備考１」
                .SIKIN_JIYUU4 = strBIKOU2                           ' 資金付替事由4：取引先マスタ「備考２」
                '2018/11/21 タスク）綾部 CHG 標準版バグ対応(信金中金先の為替付替の設定値変更(文字列設定不可))  -------------------- END
                .SYOKAI_NO = ""                                     ' 照会番号
                '2011/08/24 saitou 第6次全銀対応 EDI情報追加 ADD ---------------------------------------->>>>
                .EDI_INFO = ""                                      ' EDI情報
                '2011/08/24 saitou 第6次全銀対応 EDI情報追加 ADD ----------------------------------------<<<<
                .YOBI1 = ""                                         ' 備考
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = KawaseTukekaeInFmt.Data
            KData.OpeCode = String.Concat(KawaseTukekaeInFmt.KAMOKU_CODE, KawaseTukekaeInFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("為替付替データ作成", "失敗", ex.Message)
            Return -1
        End Try

        If strKEKKA = "金額０" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' 機能　 ： 手数料徴求（連動）データ作成処理
    '
    ' 引数　 ： TesuuMode：0 - 自振手数料，1 - 振込手数料
    '
    ' 戻り値 ： 0 - 正常，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_TESUUIN_RENDO(ByRef key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal TesuuMode As Integer) As Integer

        Dim TesuuRendoFmt As New CAstFormKes.ClsFormSikinFuri.T_99418
        Dim strTESUU_SYUBETU As String
        Dim strUTIWAKE_CODE As String
        Dim strKAMOKU_KOBAN As String
        Dim strKin As String
        Dim strTEKIYO As String

        Try
            ' 初期化
            TesuuRendoFmt.Init()

            ' 手数料種別Ｃ、手数料額、諸勘定明細摘要の設定
            If TesuuMode = 0 Then   ' 0:自振手数料
                strTESUU_SYUBETU = "301"
                strKin = CStr(CLng(key.TESUU_KIN1) + CLng(key.TESUU_KIN2))
                strTEKIYO = "ｼﾞﾌﾘﾃｽｳﾘﾖｳ"
            Else                    ' 1:振込手数料
                strTESUU_SYUBETU = "200"
                strKin = key.TESUU_KIN3
                strTEKIYO = "ﾌﾘｺﾐﾃｽｳﾘﾖｳ"
            End If

            ' 徴求店番、徴求科目・口番の設定
            If key.TESUUTYO_PATN = "0" Then     ' 0:差引
                ' データチェック
                If ini_info.HONBU_CODE = "" OrElse CLng(ini_info.HONBU_CODE) = 0 Then '空白またはオール０の時
                    MainLOG.Write("手数料徴求（連動）データ作成", "失敗", "徴求店番がありません。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE)

                    Return -1
                End If

                If key.HONBU_KOUZA = "" OrElse CLng(key.HONBU_KOUZA) = 0 Then
                    MainLOG.Write("手数料徴求（連動）データ作成", "失敗", "徴求科目・口番がありません。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE)
                    Return -1
                End If

                '*** 修正 2008/11/25 nishida 本部コード＝とりまとめ店の場合空白、本部コード≠とりまとめ店の場合本部コードをセットする。START
                If ini_info.HONBU_CODE = key.TORIMATOME_SIT_NO Then
                    strUTIWAKE_CODE = ""
                Else
                    strUTIWAKE_CODE = ini_info.HONBU_CODE
                End If
                '*** 修正 2008/11/25 nishida 本部コード＝とりまとめ店の場合空白、本部コード≠とりまとめ店の場合本部コードをセットする。END
                strKAMOKU_KOBAN = "04" & key.HONBU_KOUZA.PadLeft(7, "0"c)

            Else                                ' 1:直接
                ' データチェック
                If key.TSUUTYOSIT_NO = "" OrElse CInt(key.TSUUTYOSIT_NO) = 0 Then
                    MainLOG.Write("手数料徴求（連動）データ作成", "失敗", "徴求店番がありません。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE)
                    Return -1
                End If

                If key.TSUUTYOKAMOKU = "" OrElse CLng(key.TSUUTYOKAMOKU) = 0 Then
                    MainLOG.Write("手数料徴求（連動）データ作成", "失敗", "徴求科目・口番がありません。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE)
                    Return -1
                End If

                If key.TSUUTYOKOUZA = "" OrElse CLng(key.TSUUTYOKOUZA) = 0 Then
                    MainLOG.Write("手数料徴求（連動）データ作成", "失敗", "徴求科目・口番がありません。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE)
                    Return -1
                End If

                ' 2008.04.18 MODIFY 徴求店番にて手数料徴求区分が直接の場合，付替支店コード固定ではなく付替支店ととりまとめ店を判定し同一店番の場合は空白，異なる場合は付替支店を設定する >>
                'strUTIWAKE_CODE = key.TSUUTYOSIT_NO
                If key.TSUUTYOSIT_NO = key.TORIMATOME_SIT_NO Then
                    strUTIWAKE_CODE = ""
                Else
                    strUTIWAKE_CODE = key.TSUUTYOSIT_NO
                End If
                ' 2008.04.18 MODIFY 徴求店番にて手数料徴求区分が直接の場合，付替支店コード固定ではなく付替支店ととりまとめ店を判定し同一店番の場合は空白，異なる場合は付替支店を設定する <<
                strKAMOKU_KOBAN = key.TSUUTYOKAMOKU.PadLeft(2, "0"c) & key.TSUUTYOKOUZA.PadLeft(7, "0"c)
            End If

            'データ設定
            With TesuuRendoFmt
                .TESUU_SYUBETU = strTESUU_SYUBETU                   ' 手数料種別Ｃ
                .TEUSU_UTIWAKE = ""                                 ' 手数料内訳Ｃ
                .UTIWAKE_CODE = strUTIWAKE_CODE                     ' 徴求店番
                .KAMOKU_KOBAN = strKAMOKU_KOBAN                     ' 徴求科目・口番
                .KOKYAKU_NO = ""                                    ' 顧客番号
                .KAIIN_CODE = ""                                    ' 会員コード
                .TESUUTYO_KBN = ""                                  ' 手数料徴求区分
                .TESUU_KINGAKU = strKin.PadLeft(10, " "c)           ' 手数料額
                .TESUU_KEN = ""                                     ' 手数料件数
                .CLKAMOKU_KOBAN = ""                                ' ＣＬ科目口番
                .TEKIYO = strTEKIYO                                 ' 諸勘定明細摘要
                .KOUSYU_NO = ""                                     ' 後収明細番号
                .KISANBI = ""                                       ' 起算日
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = TesuuRendoFmt.Data
            KData.OpeCode = String.Concat(TesuuRendoFmt.KAMOKU_CODE, TesuuRendoFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = "0".PadLeft(13, "0"c)
            KData.ope_tesuu = strKin.PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("手数料徴求（連動）データ作成", "失敗", ex.Message)

            Return -1
        End Try

        '手数料徴求済フラグをＯＮに設定
        key.TESUUTYO_FLG = "1"
        Return 0

    End Function


    ' 機能　　：諸勘定入金（手数料）99-019 データ作成処理
    '
    ' 引数　　：
    '
    ' 戻り値　： 0 - 金額 0 円超、1 - 金額 0 円、-1 - 異常
    '
    ' 備考      ※浜松に従います
    '
    '
    Private Function fn_TESUUIN_SYOKANJOIN(ByRef Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal TesuuMode As Integer) As Integer

        Dim TesuuInSyokanjoInFmt As New CAstFormKes.ClsFormSikinFuri.T_99019
        Dim strKOUZA_NO As String               '口座番号
        Dim strUTIWAKE_CODE As String           '内訳コード
        Dim strKin As String                    '金額
        Dim strTEKIYOU As String                '摘要

        Try
            '初期化
            TesuuInSyokanjoInFmt.Init()

            If TesuuMode = 0 Then       '0:自振手数料
                'データチェック
                '諸勘定科目コード(INIファイル)
                If ini_info.TESUU_KOUZA1 = "" OrElse CLng(ini_info.TESUU_KOUZA1) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "諸勘定科目コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If
                '内訳コード（INIファイル）
                If ini_info.UCHIWAKE1 = "" OrElse CLng(ini_info.UCHIWAKE1) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "内訳コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If

                '口座番号の設定（INIファイルの諸勘定科目コード）
                strKOUZA_NO = ini_info.TESUU_KOUZA1.Trim.PadLeft(7, "0"c)

                '内訳コードの設定（INIファイルの内訳コード）
                strUTIWAKE_CODE = ini_info.UCHIWAKE1.Trim.PadLeft(2, "0"c)

                '金額の設定（決済VIEWの 手数料金額１ + 手数料金額２）
                strKin = CStr(CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2))

            Else                        '1:振込手数料
                'データチェック
                '諸勘定科目コード(INIファイル)
                If ini_info.TESUU_KOUZA2 = "" OrElse CLng(ini_info.TESUU_KOUZA2) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "諸勘定科目コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If
                '内訳コード（INIファイル）
                If ini_info.UCHIWAKE2 = "" OrElse CLng(ini_info.UCHIWAKE2) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "内訳コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If

                If Key.KESSAI_SYUBETU = "0" Then
                    '口座番号の設定（INIファイルの諸勘定科目コード）
                    strKOUZA_NO = ini_info.TESUU_KOUZA2.Trim.PadLeft(7, "0"c)

                    '内訳コードの設定（INIファイルの内訳コード）
                    strUTIWAKE_CODE = ini_info.UCHIWAKE2.Trim.PadLeft(2, "0"c)
                Else
                    '口座番号の設定（INIファイルの諸勘定科目コード）
                    strKOUZA_NO = ini_info.TESUU_KOUZA4.Trim.PadLeft(7, "0"c)

                    '内訳コードの設定（INIファイルの内訳コード）
                    strUTIWAKE_CODE = ini_info.UCHIWAKE4.Trim.PadLeft(2, "0"c)
                End If

                '金額の設定（決済VIEWの 手数料金額３）
                strKin = CStr(CLng(Key.TESUU_KIN3))
            End If

            If CInt(strKOUZA_NO) = 0 Then
                MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "口座番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If CInt(strUTIWAKE_CODE) = 0 Then
                MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "内訳コードが設定されていません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            ' 摘要の設定
            ' 名義人カナが設定済みの場合、名義人カナを優先
            If Key.TUKEMEIGI <> "" Then
                strTEKIYOU = Key.TUKEMEIGI
            Else
                strTEKIYOU = Key.ITAKU_KNAME
            End If

            If strTEKIYOU = "" Then
                MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "摘要がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            With TesuuInSyokanjoInFmt
                .KOUZA_NO = strKOUZA_NO                 '口座番号
                .GYO = "01"                             '行
                .UTIWAKE_CODE = strUTIWAKE_CODE         '内訳コード
                .ZENZAN = "0".PadLeft(15, " "c)          '前残
                .FUGOU_CODE = "1"                       '符号コード
                .KINGAKU = strKin.PadLeft(13, " "c)     '金額
                .TATEN_TEKIYOU = ""                     '他店券摘要
                .KENSU = "1".PadLeft(5, " "c)           '件数
                .FURI_CODE = ""                         '振替コード
                .TORIATUKAI1 = ""                       '取扱番号１
                .JINKAKU_CODE = ""                      '人格コード
                .KAZEI_CODE = ""                        '課税コード
                .TEKIYOU = strTEKIYOU                   '摘要
                .KISANBI = ""                           '起算日
                .GENTEN_NO = ""                         '原店番号
                .YOBI1 = ""                             '予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = TesuuInSyokanjoInFmt.Data
            ' 科目コード、オペコードの設定
            KData.OpeCode = String.Concat(TesuuInSyokanjoInFmt.KAMOKU_CODE, TesuuInSyokanjoInFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = "0".PadLeft(13, "0"c)
            KData.ope_tesuu = strKin.PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", ex.Message)
            Return -1
        End Try

        '手数料徴求済フラグをＯＮに設定
        Key.TESUUTYO_FLG = "1"
        Return 0
    End Function

    ' 機能　　：諸勘定連動入金（手数料）99-419 データ作成処理
    '
    ' 引数　　：
    '
    ' 戻り値　： 0 - 金額 0 円超、1 - 金額 0 円、-1 - 異常
    '
    ' 備考
    '
    Private Function fn_TESUUIN_SYOKANJORENDO(ByRef Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal TesuuMode As Integer) As Integer

        Dim TesuuInSyokanjoRendoFmt As New CAstFormKes.ClsFormSikinFuri.T_99419
        Dim strKOUZA_NO As String               '口座番号
        Dim strUTIWAKE_CODE As String           '内訳コード
        Dim strKin As String                    '金額
        '2011/06/17
        Dim strTEKIYOU As String = ""                '摘要
        Dim strTUKEKIN_NO As String             '決済金融機関
        Dim strTUKESIT_NO As String             '決済支店
        Dim strTSUUTYOSIT_NO As String          '手数料徴求支店     '2008/07/22　浜松信金　手数料徴求支店追加

        Try
            TesuuInSyokanjoRendoFmt.Init()

            If TesuuMode = 0 Then       '自振手数料
                'データチェック
                '諸勘定科目コード(INIファイル)
                If ini_info.TESUU_KOUZA1 = "" OrElse CLng(ini_info.TESUU_KOUZA1) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "諸勘定科目コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If
                '内訳コード（INIファイル）
                If ini_info.UCHIWAKE1 = "" OrElse CLng(ini_info.UCHIWAKE1) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "内訳コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If

                '口座番号の設定（INIファイルの諸勘定科目コード）
                strKOUZA_NO = ini_info.TESUU_KOUZA1.Trim.PadLeft(7, "0"c)

                '内訳コードの設定（INIファイルの内訳コード）
                strUTIWAKE_CODE = ini_info.UCHIWAKE1.Trim.PadLeft(2, "0"c)

                '金額の設定（決済VIEWの 手数料金額１ + 手数料金額２）
                strKin = CStr(CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2))

            Else                        '振込手数料
                'データチェック
                '諸勘定科目コード(INIファイル)
                If ini_info.TESUU_KOUZA3 = "" OrElse CLng(ini_info.TESUU_KOUZA3) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "諸勘定科目コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If
                '内訳コード（INIファイル）
                If ini_info.UCHIWAKE3 = "" OrElse CLng(ini_info.UCHIWAKE3) = 0 Then '空白、またはオールゼロの時
                    MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "内訳コードがありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                    Return -1
                End If

                If Key.KESSAI_SYUBETU = "0" Then
                    '口座番号の設定（INIファイルの諸勘定科目コード）
                    strKOUZA_NO = ini_info.TESUU_KOUZA3.Trim.PadLeft(7, "0"c)

                    '内訳コードの設定（INIファイルの内訳コード）
                    strUTIWAKE_CODE = ini_info.UCHIWAKE3.Trim.PadLeft(2, "0"c)
                Else
                    '口座番号の設定（INIファイルの諸勘定科目コード）
                    strKOUZA_NO = ini_info.TESUU_KOUZA5.Trim.PadLeft(7, "0"c)

                    '内訳コードの設定（INIファイルの内訳コード）
                    strUTIWAKE_CODE = ini_info.UCHIWAKE5.Trim.PadLeft(2, "0"c)
                End If

                '金額の設定（決済VIEWの 手数料金額３）
                strKin = CStr(CLng(Key.TESUU_KIN3))
            End If

            If CInt(strKOUZA_NO) = 0 Then
                MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "口座番号がありません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            If CInt(strUTIWAKE_CODE) = 0 Then
                MainLOG.Write("諸勘定入金（手数料）データ作成", "失敗", "内訳コードが設定されていません。取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振替日：" & Key.FURI_DATE)
                Return -1
            End If

            strTUKEKIN_NO = Key.TUKEKIN_NO
            strTUKESIT_NO = Key.TUKESIT_NO
            strTSUUTYOSIT_NO = Key.TSUUTYOSIT_NO        '2008/07/22　浜松信金　手数料徴求支店追加

            With TesuuInSyokanjoRendoFmt
                .KOUZA_NO = strKOUZA_NO                 '口座番号
                .GYO = "01"                             '行
                .UTIWAKE_CODE = strUTIWAKE_CODE         '内訳コード
                .ZENZAN = "0".PadLeft(15, " "c)         '前残
                .FUGOU_CODE = "1"                       '符号コード
                .KINGAKU = strKin.PadLeft(13, " "c)     '金額
                .KENSU = "1".PadLeft(5, " "c)           '件数
                .FURI_CODE = ""                         '振替コード
                .TORIATUKAI1 = ""                       '取扱番号１
                .JINKAKU_CODE = ""                      '人格コード
                .KAZEI_CODE = ""                        '課税コード
                .TEKIYOU = "ﾃｽｳﾘﾖｳ"                     '摘要

                If strTUKEKIN_NO = ini_info.JIKINKO_CODE Then
                    .RENDO_TEN = strTSUUTYOSIT_NO.Trim.PadLeft(3, "0"c)        '連動店番
                    .RENDO_KAMOKU = (Key.TSUUTYOKAMOKU).Trim.PadLeft(2, "0"c) + (Key.TSUUTYOKOUZA).Trim.PadLeft(7, "0"c) '連動科目口番
                Else
                    .RENDO_TEN = ""
                    .RENDO_KAMOKU = ""
                End If
                .AITE_UTIWAKE = ""                      '相手内訳コード
                .SOFT_NO = ""                           'ソフト機番
                .TORIATUKAI2 = ""                       '取扱番号２
                .AITE_TEKIYOU = ""                      '相手摘要
                .KISANBI = ""                           '起算日
                .YOBI1 = ""                             '予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = TesuuInSyokanjoRendoFmt.Data
            ' 科目コード、オペコードの設定
            KData.OpeCode = String.Concat(TesuuInSyokanjoRendoFmt.KAMOKU_CODE, TesuuInSyokanjoRendoFmt.OPE_CODE)

            ' 発信金額の設定
            KData.ope_nyukin = "0".PadLeft(13, "0"c)
            KData.ope_tesuu = strKin.PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("諸勘定連動入金（手数料）データ作成", "失敗", ex.Message)
            Return -1
        End Try

        '手数料徴求済フラグをＯＮに設定
        Key.TESUUTYO_FLG = "1"
        Return 0
    End Function


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

    ' 機能　 ： 備考１編集処理
    '
    ' 引数　 ： Key:
    '           strBIKOU1:編集後の備考１
    '           strKeta  :備考１の桁数
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 備考１を下記のとおり編集して戻す
    '           「@MM-DD@」 → 振替日に編集
    '           「@KEN@」   → 振替済件数に編集
    '           （ 取引先マスタ登録時、「@MM-DD@」、「@TEN@」、「@KEN@」は同時に設定不可）
    '
    Private Function fn_BIKOU1_SETTEI(ByVal Key As KeyInfo, ByRef strBIKOU1 As String, ByVal intKeta As Integer) As Boolean

        Try
            If Key.BIKOU1.Length >= 7 Then
                If Key.BIKOU1.Substring(0, 7) = "@MM-DD@" Then
                    strBIKOU1 = Key.FURI_DATE.Substring(4, 2) & "-" & Key.FURI_DATE.Substring(6, 2) & Key.BIKOU1.Substring(7, Key.BIKOU1.Length - 7)
                Else
                    strBIKOU1 = Key.BIKOU1
                End If
            ElseIf Key.BIKOU1.Length >= 5 Then
                strBIKOU1 = Key.BIKOU1.Replace("@KEN@", Key.FURI_KEN)
            Else
                strBIKOU1 = Key.BIKOU1
            End If

            ' 設定桁数超過チェック
            If strBIKOU1.Length > intKeta Then
                strBIKOU1 = strBIKOU1.Substring(0, intKeta)
            End If

        Catch ex As Exception
            MainLOG.Write("備考１編集処理", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 備考２編集処理
    '
    ' 引数　 ： Key:
    '           strBIKOU1:編集後の備考２
    '           strKeta  :備考２の桁数
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 備考２を下記のとおり編集して戻す
    '           「@MM-DD@」 → 振替日に編集
    '           「@TEN@」   → 支店名カナ＋「ｱﾂｶｲ」に編集
    '           「@KEN@」   → 振替済件数に編集
    '           （ 取引先マスタ登録時、「@MM-DD@」、「@TEN@」、「@KEN@」は同時に設定不可）
    '
    Private Function fn_BIKOU2_SETTEI(ByVal Key As KeyInfo, ByRef strBIKOU2 As String, ByVal intKeta As Integer, ByVal strSIT_KNM As String) As Boolean

        Try
            If Key.BIKOU2.Trim.Length >= 7 Then
                If Key.BIKOU2.Substring(0, 7) = "@MM-DD@" Then
                    strBIKOU2 = Key.FURI_DATE.Substring(4, 2) & "-" & Key.FURI_DATE.Substring(6, 2) & Key.BIKOU2.Substring(7, Key.BIKOU2.Length - 7)
                ElseIf Key.BIKOU2.Substring(0, 5) = "@TEN@" Then
                    strBIKOU2 = strSIT_KNM.Trim & " ｱﾂｶｲ" & Key.BIKOU2.Substring(5, Key.BIKOU2.Length - 5)
                Else
                    strBIKOU2 = Key.BIKOU2
                End If
            ElseIf Key.BIKOU2.Length >= 5 Then
                If Key.BIKOU2.Substring(0, 5) = "@TEN@" Then
                    strBIKOU2 = strSIT_KNM.Trim & " ｱﾂｶｲ" & Key.BIKOU2.Substring(5, Key.BIKOU2.Length - 5)
                ElseIf Key.BIKOU2.Substring(0, 5) = "@KEN@" Then
                    strBIKOU2 = Key.BIKOU2.Replace("@KEN@", Key.FURI_KEN)
                Else
                    strBIKOU2 = Key.BIKOU2
                End If
            Else
                strBIKOU2 = Key.BIKOU2
            End If

            ' 設定桁数超過チェック
            If strBIKOU2.Length > intKeta Then
                strBIKOU2 = strBIKOU2.Substring(0, intKeta)
            End If

        Catch ex As Exception
            MainLOG.Write("備考２編集処理", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 資金決済マスタ登録
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertKesMast(ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            MainLOG.Write("(資金決済マスタ登録)開始", "成功")


            SQL.Append("INSERT INTO KESSAIMAST(")
            SQL.Append(" SYORI_DATE_KR")
            SQL.Append(",TIME_STAMP_KR")
            SQL.Append(",KAIJI_KR")
            SQL.Append(",RECORD_NO_KR")
            SQL.Append(",FILE_NAME_KR")
            SQL.Append(",TORIS_CODE_KR")
            SQL.Append(",TORIF_CODE_KR")
            SQL.Append(",FURI_DATE_KR")
            SQL.Append(",KAMOKU_CODE_KR")
            SQL.Append(",OPE_CODE_KR")
            SQL.Append(",DENBUN_ALL_KR")
            SQL.Append(",ERR_CODE_KR")
            SQL.Append(",ERR_MSG_KR")
            SQL.Append(",SAKUSEI_DATE_KR")
            SQL.Append(",KOUSIN_DATE_KR")
            SQL.Append(") VALUES (")
            SQL.Append(" " & SQ(strDate))                                   ' 処理日
            SQL.Append("," & SQ(String.Concat(strDate, strTime)))           ' タイムスタンプ
            SQL.Append("," & SQ(keskey.Kaiji))                              ' 回次
            SQL.Append("," & SQ(keskey.RecordNo))                           ' 通番
            SQL.Append("," & SQ(ini_info.RIENTA_FILENAME))                     ' リエンタファイル名
            SQL.Append("," & SQ(KData.TorisCode))                        ' 取引先主コード
            SQL.Append("," & SQ(KData.TorifCode))                        ' 取引先副コード
            SQL.Append("," & SQ(KData.FuriDate))                         ' 振替日
            SQL.Append("," & SQ(KData.OpeCode.Substring(0, 2)))           ' 科目コード
            SQL.Append("," & SQ(KData.OpeCode.Substring(2, 3)))           ' オペコード
            SQL.Append("," & SQ(KData.record320))                           ' 個別データ
            SQL.Append("," & SQ(""))                                        ' 結果コード
            SQL.Append("," & SQ(""))                                        ' エラーメッセージ
            SQL.Append("," & SQ(strDate))                                   ' 作成日
            SQL.Append("," & SQ(strDate))                                   ' 更新日
            SQL.Append(")")

            Call MainDB.ExecuteNonQuery(SQL)
            CntDenbun += 1
        Catch ex As Exception
            MainLOG.Write("(資金決済マスタ登録)", "失敗", ex.Message)

            Return False
        Finally
            MainLOG.Write("(資金決済マスタ登録)開始", "成功")

        End Try

        Return True

    End Function

    ' 機能　 ： スケジュールマスタ更新
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateSchMast(ByVal key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim Up_FLG As Boolean = False

        Try
            '*** 修正 mitsu 2008/07/31 別途徴求でも決済フラグを更新する ***
            '' 手数料徴求区分が 3：別途徴求 の場合、スケジュールマスタは更新しない
            'If key.TESUUTYO_KBN = "3" Then
            '    LOG.Write("スケジュールマスタ更新", "成功", "スケジュール更新対象外（手数料徴求区分：別途徴求）。" & "取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE)
            '    Return True
            'End If
            '**************************************************************

            SQL.Append("UPDATE SCHMAST")
            SQL.Append(" SET ")

            Select Case key.KESSAI_TORI_KBN

                Case "0"    ' 資金決済と手数料徴求を同時に行う先
                    Up_FLG = True

                    SQL.Append(" KESSAI_FLG_S = '1'")
                    SQL.Append(",KESSAI_DATE_S = " & SQ(strDate))
                    SQL.Append(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))

                    ' 手数料徴求済、又は、決済区分が 04：特別企業、05：決済対象外 の場合
                    If key.TESUUTYO_FLG = "1" Or _
                        (key.KESSAI_KBN = "04" Or key.KESSAI_KBN = "05") Then

                        SQL.Append(",TESUUTYO_FLG_S = '1'")
                        SQL.Append(",TESUU_DATE_S = " & SQ(strDate))
                        SQL.Append(",TESUU_TIME_STAMP_S = " & SQ(strDate & strTime))
                    End If

                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
                    SQL.Append("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
                    SQL.Append("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))

                Case "1"    ' 資金決済だけ行う先
                    Up_FLG = True

                    SQL.Append(" KESSAI_FLG_S = '1'")
                    SQL.Append(",KESSAI_DATE_S = " & SQ(strDate))
                    SQL.Append(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))
                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
                    SQL.Append("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
                    SQL.Append("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))

                Case "2"    ' 手数料徴求だけ行う先

                    '*** 手数料のみは動作させない 2009.10.29 start
                    ' 手数料徴求済、又は、決済区分が 04：特別企業、05：決済対象外の場合
                    'If key.TESUUTYO_FLG = "1" Or _
                    '    (key.KESSAI_KBN = "04" Or key.KESSAI_KBN = "05") Then

                    '    Up_FLG = True

                    '    SQL.Append(" TESUUTYO_FLG_S = '1'")
                    '    SQL.Append(",TESUU_DATE_S = " & SQ(strDate))
                    '    SQL.Append(",TESUU_TIME_STAMP_S = " & SQ(strDate & strTime))
                    '    SQL.Append(" WHERE TORIS_CODE_S    = " & SQ(key.TORIS_CODE))
                    '    SQL.Append("   AND TORIF_CODE_S    = " & SQ(key.TORIF_CODE))
                    '    SQL.Append("   AND (KESSAI_YDATE_S != " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUU_YDATE_S   = " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUUKEI_FLG_S  = '1'")                              ' 1:手数料計算済
                    '    SQL.Append("   AND KESSAI_FLG_S    = '1'")                              ' 1:決済済み
                    '    SQL.Append("   AND TESUUTYO_FLG_S  = '2'")                              ' 0:手数料未徴求
                    '    SQL.Append("   AND TYUUDAN_FLG_S   = '0'")                              ' 0:中断なし
                    '    SQL.Append("   AND FURI_KIN_S      >  0)")                              ' 振替済金額あり
                    '    SQL.Append("   OR")
                    '    ' 手数料徴求区分が 1:一括徴求 の場合、資金決済と手数料徴求を同時に行う先も含める
                    '    SQL.Append("      (KESSAI_YDATE_S  = " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUU_YDATE_S   = " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUUKEI_FLG_S  = '1'")                              ' 1:手数料計算済
                    '    SQL.Append("   AND KESSAI_FLG_S    = '1'")                              ' 1:決済済み  ← 決済データは既に完了済
                    '    SQL.Append("   AND TESUUTYO_FLG_S  = '2'")                              ' 0:手数料未徴求
                    '    SQL.Append("   AND TYUUDAN_FLG_S   = '0'")                              ' 0:中断なし
                    '    SQL.Append("   AND FURI_KIN_S      >  0")                               ' 振替済金額あり
                    '    SQL.Append("   AND EXISTS")                                             ' 1:一括徴求
                    '    SQL.Append("       (SELECT TORIS_CODE_T")
                    '    SQL.Append("        FROM TORIMAST")
                    '    SQL.Append("        WHERE TORIS_CODE_T    = TORIS_CODE_S")
                    '    SQL.Append("          AND TORIF_CODE_T    = TORIF_CODE_S")
                    '    SQL.Append("          AND TESUUTYO_KBN_T  = '1'))")
                    'End If
                    '*** 手数料のみは動作させない 2009.10.29 end 
            End Select

            If Up_FLG = True Then
                Call MainDB.ExecuteNonQuery(SQL)
            Else
                MainLOG.Write("スケジュールマスタ更新", "成功", "スケジュール更新対象外（手数料未徴求先）。" & "取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE)
            End If

        Catch ex As Exception
            MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振替日：" & key.FURI_DATE & " " & ex.Message)
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

    ' 機能　 ： スケジュールマスタ（学校自振）の学年フラグを取得
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function GetGakunenFLG(ByRef Key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraGSCHReader As New CASTCommon.MyOracleReader(MainDB)

        Key.GAKUNEN_FLG = New ArrayList(8)

        Try
            ' FURI_KBN_S =  0：初振/ 1：再振/ 2：入金/ 3：出金
            ' TORIF_CODE = 01：初振/02：再振/03：入金/04：出金

            SQL.Append("SELECT *")
            SQL.Append(" FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Key.TORIS_CODE))
            SQL.Append("   AND FURI_KBN_S = " & SQ(CInt(Key.TORIF_CODE) - 1))
            SQL.Append("   AND FURI_DATE_S = " & SQ(Key.FURI_DATE))
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            '特別振替日の場合、学校スケジュールマスタは同一振替日で複数レコード持つことができるので、
            '複数レコード存在する場合は学年フラグをマージする
            Dim GakunenFlg(8) As String
            '学年フラグはすべて0で初期化
            For i As Integer = 0 To GakunenFlg.Length - 1
                GakunenFlg(i) = "0"
            Next

            If OraGSCHReader.DataReader(SQL) = True Then
                While OraGSCHReader.EOF = False
                    For iCount As Integer = 1 To 9
                        If OraGSCHReader.GetString("GAKUNEN" & iCount.ToString & "_FLG_S").Equals("1") = True Then
                            GakunenFlg(iCount - 1) = OraGSCHReader.GetString("GAKUNEN" & iCount.ToString & "_FLG_S")
                        End If
                    Next

                    OraGSCHReader.NextRead()
                End While
            Else
                Throw New Exception("スケジュールマスタ（学校自振）に該当データが存在しません。")
            End If

            For i As Integer = 0 To GakunenFlg.Length - 1
                Key.GAKUNEN_FLG.Add(GakunenFlg(i))
            Next

            'If OraGSCHReader.DataReader(SQL) = True Then
            '    For iCount As Integer = 1 To 9
            '        Key.GAKUNEN_FLG.Add(OraGSCHReader.GetString("GAKUNEN" & iCount & "_FLG_S"))
            '    Next iCount
            'Else
            '    Throw New Exception("スケジュールマスタ（学校自振）に該当データが存在しません。")
            'End If
            '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END

            Return True
        Catch ex As Exception
            MainLOG.Write("学年フラグ取得処理", "失敗", ex.Message)
            Return False
        Finally
            OraGSCHReader.Close()
            OraGSCHReader = Nothing
        End Try
    End Function

    ' 機能　 ： 決済ワークマスタ作成処理
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function CreateMAIN0500G_WORK(ByVal Key As KeyInfo) As Boolean
        Dim HimoSQL As New StringBuilder(128)
        Dim OraHimoReader As New CASTCommon.MyOracleReader(MainDB)
        Dim GakunenFlg As String
        Dim KeyGak As KeyGakInfo = Nothing

        Try
            ' 決済ワークマスタ削除処理
            If DeleteMAIN0500G_WORK() = False Then
                Return False
            End If

            ' 学年フラグ取得処理
            If GetGakunenFLG(Key) = False Then
                Return False
            End If

            HimoSQL.Append("SELECT")
            HimoSQL.Append("  HIMOMAST.*")
            HimoSQL.Append("  ,G_MEIMAST.*")
            HimoSQL.Append(" FROM")
            HimoSQL.Append("  HIMOMAST")
            HimoSQL.Append(" ,G_MEIMAST")
            HimoSQL.Append(" WHERE")
            HimoSQL.Append("      GAKKOU_CODE_M = GAKKOU_CODE_H")
            HimoSQL.Append("  AND GAKUNEN_CODE_M = GAKUNEN_CODE_H")
            HimoSQL.Append("  AND GAKKOU_CODE_M = " & SQ(Key.TORIS_CODE))
            HimoSQL.Append("  AND FURI_DATE_M = " & SQ(Key.FURI_DATE))
            HimoSQL.Append("  AND FURI_KBN_M = " & SQ(CInt(Key.TORIF_CODE) - 1))
            HimoSQL.Append("  AND HIMOKU_ID_H ='000'")                      ' 000:決済口座
            HimoSQL.Append(" AND (")

            Dim bLoopFlg As Boolean = False

            ' 処理対象の学年のみ抽出
            For iCount As Integer = 1 To Key.GAKUNEN_FLG.Count
                GakunenFlg = CType(Key.GAKUNEN_FLG.Item(iCount - 1), String)

                If GakunenFlg = "1" Then
                    If bLoopFlg = True Then
                        HimoSQL.Append(" OR ")
                    End If
                    HimoSQL.Append(" GAKUNEN_CODE_H = " & iCount)
                    bLoopFlg = True
                End If
            Next iCount

            HimoSQL.Append(" )")
            HimoSQL.Append(" ORDER BY GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H,TUKI_NO_H")

            If OraHimoReader.DataReader(HimoSQL) = True Then
                Do While OraHimoReader.EOF = False

                    For iHimokuNo As Integer = 1 To 15
                        ' 初期化
                        KeyGak.Init()

                        ' 最初のキー設定
                        Call KeyGak.SetOraDataGak(OraHimoReader, iHimokuNo.ToString("00"))

                        ' 費目名が設定されているレコードを対象とする
                        If KeyGak.HIMOKU_NAME <> "" Then
                            Dim WorkSQL As New StringBuilder(128)
                            Dim OraWorkReader As New CASTCommon.MyOracleReader(MainDB)

                            Try
                                ' 同一の費目口座単位で決済ワークマスタを検索
                                WorkSQL.Append(" SELECT * FROM MAIN0500G_WORK")
                                WorkSQL.Append(" WHERE")
                                WorkSQL.Append("     GAKKOU_CODE_H = " & SQ(KeyGak.GAKKOU_CODE))
                                WorkSQL.Append(" AND KESSAI_KIN_CODE_H = " & SQ(KeyGak.KESSAI_KIN_CODE))
                                WorkSQL.Append(" AND KESSAI_TENPO_H = " & SQ(KeyGak.KESSAI_TENPO))
                                WorkSQL.Append(" AND KESSAI_KAMOKU_H = " & SQ(KeyGak.KESSAI_KAMOKU))
                                WorkSQL.Append(" AND KESSAI_KOUZA_H = " & SQ(KeyGak.KESSAI_KOUZA))
                                WorkSQL.Append(" AND HIMOKU_NO_H = " & iHimokuNo)

                                ' 費目口座単位で費目金額を合算する
                                If OraWorkReader.DataReader(WorkSQL) = True Then
                                    ' 同一の費目口座が存在する場合

                                    ' 費目金額を合算する
                                    KeyGak.HIMOKU_KINGAKU += OraWorkReader.GetInt64("HIMOKU_KINGAKU_H")
                                    KeyGak.HIMOKU_FURI_KIN += OraWorkReader.GetInt64("HIMOKU_FURI_KIN_H")
                                    KeyGak.HIMOKU_FUNOU_KIN += OraWorkReader.GetInt64("HIMOKU_FUNOU_KIN_H")
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                    '振替済みのみカウントアップする
                                    If KeyGak.FURIKETU_CODE = "0" Then
                                        KeyGak.HIMOKU_GASSAN = OraWorkReader.GetInt64("YOBI1_H") + 1
                                    Else
                                        KeyGak.HIMOKU_GASSAN = OraWorkReader.GetInt64("YOBI1_H")
                                    End If
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

                                    ' 決済ワークマスタ更新処理
                                    If UpdateMAIN0500G_WORK(KeyGak, iHimokuNo) = False Then
                                        Return False
                                    End If
                                Else
                                    ' 同一の費目口座が存在しない場合
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
                                    If KeyGak.FURIKETU_CODE = "0" Then
                                        KeyGak.HIMOKU_GASSAN = 1
                                    Else
                                        '不能の場合はレコードだけ作成する
                                        KeyGak.HIMOKU_GASSAN = 0
                                    End If
                                    '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END

                                    ' 決済ワークマスタ登録処理
                                    If InsertMAIN0500G_WORK(KeyGak, iHimokuNo, Key) = False Then
                                        Return False
                                    End If
                                End If
                            Catch ex As Exception
                                MainLOG.Write("決済ワークマスタ作成処理", "失敗", ex.Message)
                                Return False
                            Finally
                                If Not OraWorkReader Is Nothing Then
                                    OraWorkReader.Close()
                                    OraWorkReader = Nothing
                                End If
                            End Try

                        End If
                    Next iHimokuNo

                    ' 対象データの次レコードを読込む
                    OraHimoReader.NextRead()
                Loop
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("決済ワークマスタ作成処理", "失敗", ex.Message)
            Return False
        Finally
            If Not OraHimoReader Is Nothing Then
                OraHimoReader.Close()
                OraHimoReader = Nothing
            End If
        End Try

    End Function

    ' 機能　 ： 決済ワークマスタ削除処理
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function DeleteMAIN0500G_WORK() As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            '決済ワークマスタ削除
            '------------------------------------------------
            SQL.Append("DELETE FROM MAIN0500G_WORK")

            ' 決済ワークマスタがゼロ件の場合は、エラーとしない
            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                Throw New Exception(MainDB.Message)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("決済ワークマスタ削除処理", "失敗", ex.Message)
            Return False
        End Try

    End Function

    ' 機能　 ： 決済ワークマスタ登録処理
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertMAIN0500G_WORK(ByVal KeyGak As KeyGakInfo, ByVal iHimokuNo As Integer, ByVal Key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            '決済ワークマスタ登録
            '------------------------------------------------
            SQL.Append("INSERT INTO MAIN0500G_WORK(")
            SQL.Append(" GAKKOU_CODE_H")
            SQL.Append(",HIMOKU_NAME_H")
            SQL.Append(",KESSAI_KIN_CODE_H")
            SQL.Append(",KESSAI_TENPO_H")
            SQL.Append(",KESSAI_KAMOKU_H")
            SQL.Append(",KESSAI_MEIGI_H")
            SQL.Append(",KESSAI_KOUZA_H")
            SQL.Append(",HIMOKU_KINGAKU_H")
            SQL.Append(",HIMOKU_FURI_KIN_H")
            SQL.Append(",HIMOKU_FUNOU_KIN_H")
            SQL.Append(",GAKUNEN_H")
            SQL.Append(",HIMOKU_NO_H")
            SQL.Append(",FURI_KBN_H")
            SQL.Append(",YOBI1_H")
            SQL.Append(",YOBI2_H")
            SQL.Append(",YOBI3_H")
            SQL.Append(") VALUES (")
            SQL.Append(SQ(KeyGak.GAKKOU_CODE))                      ' 学校コード
            SQL.Append("," & SQ(KeyGak.HIMOKU_NAME))                ' 費目名
            SQL.Append("," & SQ(KeyGak.KESSAI_KIN_CODE))            ' 決済金融機関コード
            SQL.Append("," & SQ(KeyGak.KESSAI_TENPO))               ' 決済支店コード
            SQL.Append("," & SQ(KeyGak.KESSAI_KAMOKU))              ' 決済科目
            SQL.Append("," & SQ(KeyGak.KESSAI_MEIGI))               ' 決済名義人
            SQL.Append("," & SQ(KeyGak.KESSAI_KOUZA))               ' 決済口座番号
            SQL.Append("," & KeyGak.HIMOKU_KINGAKU)                 ' 費目金額
            SQL.Append("," & KeyGak.HIMOKU_FURI_KIN)                ' 費目振替金額
            SQL.Append("," & KeyGak.HIMOKU_FUNOU_KIN)               ' 費目不能金額
            SQL.Append(",0")                                        ' 学年コード←未使用のため初期値設定
            SQL.Append("," & iHimokuNo)                             ' 費目番号
            SQL.Append("," & CInt(Key.TORIF_CODE) - 1)              ' 振替区分
            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ START
            SQL.Append("," & SQ(KeyGak.HIMOKU_GASSAN.ToString))     ' 予備1(費目番号別の合算数)
            'SQL.Append(",''")                                       ' 予備1     ←未使用のため初期値設定
            '2017/03/16 タスク）西野 CHG 標準版修正（資金決済性能改善）------------------------------------ END
            SQL.Append(",''")                                       ' 予備2     ←未使用のため初期値設定
            SQL.Append(",''")                                       ' 予備3     ←未使用のため初期値設定
            SQL.Append(")")

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                Throw New Exception(MainDB.Message)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("決済ワークマスタ登録処理", "失敗", ex.Message)
            Return False
        End Try

    End Function

    ' 機能　 ： 決済ワークマスタ更新処理
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateMAIN0500G_WORK(ByVal KeyGak As KeyGakInfo, ByVal iHimokuNo As Integer) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            '決済ワークマスタ更新
            '------------------------------------------------
            SQL.Append(" UPDATE MAIN0500G_WORK SET")
            SQL.Append("  HIMOKU_KINGAKU_H = " & KeyGak.HIMOKU_KINGAKU)
            SQL.Append(" ,HIMOKU_FURI_KIN_H = " & KeyGak.HIMOKU_FURI_KIN)
            SQL.Append(" ,HIMOKU_FUNOU_KIN_H = " & KeyGak.HIMOKU_FUNOU_KIN)
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ START
            SQL.Append(" ,YOBI1_H = " & SQ(KeyGak.HIMOKU_GASSAN.ToString))
            '2017/03/16 タスク）西野 ADD 標準版修正（資金決済性能改善）------------------------------------ END
            SQL.Append(" WHERE")
            SQL.Append("     GAKKOU_CODE_H =" & SQ(KeyGak.GAKKOU_CODE))
            SQL.Append(" AND KESSAI_KIN_CODE_H =" & SQ(KeyGak.KESSAI_KIN_CODE))
            SQL.Append(" AND KESSAI_TENPO_H =" & SQ(KeyGak.KESSAI_TENPO))
            SQL.Append(" AND KESSAI_KAMOKU_H =" & SQ(KeyGak.KESSAI_KAMOKU))
            SQL.Append(" AND KESSAI_KOUZA_H =" & SQ(KeyGak.KESSAI_KOUZA))
            SQL.Append(" AND HIMOKU_NO_H =" & iHimokuNo)

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                Throw New Exception(MainDB.Message)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("決済ワークマスタ更新処理", "失敗", ex.Message)
            Return False
        End Try

    End Function

    ' 機能　 ： 決済ワークマスタ検索処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function SelectMAIN0500G_WORK(ByRef lstHimokuData As ArrayList) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraWorkReader As New CASTCommon.MyOracleReader(MainDB)
        Dim Key As KeyInfo = Nothing

        lstHimokuData = New ArrayList(128)

        Try
            SQL.Append("SELECT *")
            SQL.Append(" FROM MAIN0500G_WORK")
            SQL.Append(" WHERE HIMOKU_FURI_KIN_H > 0")          ' 費目振替金額あり
            SQL.Append(" ORDER BY")
            SQL.Append("   GAKKOU_CODE_H")
            SQL.Append("  ,KESSAI_KAMOKU_H DESC")
            SQL.Append("  ,KESSAI_KIN_CODE_H")
            SQL.Append("  ,KESSAI_TENPO_H")
            SQL.Append("  ,KESSAI_KOUZA_H")
            SQL.Append("  ,HIMOKU_NO_H")

            If OraWorkReader.DataReader(SQL) = True Then
                Do While OraWorkReader.EOF = False
                    ' キー初期化
                    Key.Init()

                    ' キー設定
                    Key.SetOraDataHimo(OraWorkReader)

                    ' 費目口座単位の決済情報をセット
                    lstHimokuData.Add(Key)

                    ' 対象データの次レコードを読込む
                    OraWorkReader.NextRead()
                Loop
            Else
                lstHimokuData = Nothing
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("決済ワークマスタ検索処理", "失敗", ex.Message)
            Return False
        Finally
            OraWorkReader.Close()
            OraWorkReader = Nothing
        End Try

    End Function

    Private Function MakeRientaFD(ByVal ary As ArrayList) As Boolean
        Dim T_RIENT77 As New CAstFormKes.ClsT_RIENT77
        Dim T_RIENT10 As New CAstFormKes.ClsT_RIENT10

        Dim Kdata As CAstFormKes.ClsFormKes.KessaiData
        Dim EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")

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

        Dim StrmWrite As FileStream = Nothing

        Try

            ' タンキングヘッダ  
            ' 初期データ設定
            Call T_RIENT77.TANKING_HEAD.Init()
            ' Ｔ日付
            T_RIENT77.TANKING_HEAD.strT_HIDUKE = CASTCommon.Calendar.Now.ToString("yyyyMMdd")

            ' 店舗情報レコード（１店舗情報）
            ' 初期データ設定
            Call T_RIENT77.TENPO_INFOREC(0).Init()
            ' 金庫コード
            T_RIENT77.TENPO_INFOREC(0).strKINKO_CD = ini_info.JIKINKO_CODE
            ' 店舗コード
            T_RIENT77.TENPO_INFOREC(0).strSIT_CD = ini_info.HONBU_CODE

            ' 店舗情報レコード（２～３２店舗情報）
            ' 初期データ設定
            For i As Integer = 1 To T_RIENT77.TENPO_INFOREC.Length - 1
                ' ２～３２の初期データ設定
                Call T_RIENT77.TENPO_INFOREC(i).Init2_32()
            Next i

            ' 予備３
            ' 初期化
            Call T_RIENT77.DATA_SIKIBETU.Init()

            ' リエンタファイル オープン

            If File.Exists(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME)) = True Then
                ' 既に存在する場合は，削除
                File.Delete(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME))
            End If

            StrmWrite = New FileStream(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME), FileMode.OpenOrCreate, FileAccess.ReadWrite)

            Dim Bytes As Byte() = EncdJ.GetBytes(ini_info.RIENTA_FILENAME.PadRight(12, Nothing).PadRight(16, " "c))
            StrmWrite.Write(Bytes, 0, 16)

            ' タンキングヘッダ書込
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.Data, 0, 28)

            ' 店舗情報レコード書込
            For i As Integer = 0 To T_RIENT77.TENPO_INFOREC.Length - 1
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(i).Data, 0, 28)
            Next i

            ' 予備３書込
            StrmWrite.Write(T_RIENT77.DATA_SIKIBETU.Data, 0, 84)

            Dim WriteCount As Integer = 0           ' 書込件数

            Dim RecLen As Integer

            ' タンキングデータ書込

            Dim cnt As Integer = ary.Count - 1 'ループ回数

            For i As Integer = 0 To cnt

                ' タンキングデータ
                Kdata = CType(ary.Item(i), CAstFormKes.ClsFormKes.KessaiData)

                Select Case Kdata.OpeCode
                    Case "01010"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_01010.DataSepaPlus = Kdata.record320
                        RecLen = T_01010.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_01010.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3               '(ジャーナル・伝票)
                        T_RIENT10.TANKING_DATA.bytNYURYOKU(0) = 24
                        T_01010 = Nothing
                    Case "02019"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_02019.DataSepaPlus = Kdata.record320
                        RecLen = T_02019.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_02019.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 11                '(ジャーナル・伝票・証書)
                        T_RIENT10.TANKING_DATA.bytNYURYOKU(0) = 24
                        T_02019 = Nothing
                    Case "04019"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_04019.DataSepaPlus = Kdata.record320
                        RecLen = T_04019.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_04019.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3                '(ジャーナル・伝票)
                        T_04099 = Nothing
                    Case "04099"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_04099.DataSepaPlus = Kdata.record320
                        RecLen = T_04099.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_04099.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3                '(ジャーナル・伝票)
                        T_04099 = Nothing
                    Case "04419"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_04419.DataSepaPlus = Kdata.record320
                        RecLen = T_04419.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_04419.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3                '(ジャーナル・伝票)
                        T_04419 = Nothing
                    Case "48100"
                        T_RIENT10.TANKING_DATA.Init_48()
                        T_48100.DataSepaPlus = Kdata.record320
                        RecLen = T_48100.DataSepaPlus.Length + 32
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48100.DataSepaPlus
                        T_48100 = Nothing
                    Case "48500"
                        T_RIENT10.TANKING_DATA.Init_48()
                        T_48500.DataSepaPlus = Kdata.record320
                        'RecLen = T_48500.DataSepaPlus.Replace(" ", "").Length + 32
                        RecLen = T_48500.DataSepaPlus.Length + 32
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48500.DataSepaPlus
                        T_48500 = Nothing
                    Case "48600"
                        T_RIENT10.TANKING_DATA.Init_48()
                        T_48600.DataSepaPlus = Kdata.record320
                        RecLen = T_48600.DataSepaPlus.Length + 32
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48600.DataSepaPlus
                        T_48600 = Nothing
                    Case "99019"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_99019.DataSepaPlus = Kdata.record320
                        RecLen = T_99019.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_99019.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 11           '(ジャーナル・伝票・証書)
                        T_99019 = Nothing
                    Case "99418"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_99418.DataSepaPlus = Kdata.record320
                        RecLen = T_99418.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_99418.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3           '(ジャーナル・伝票)
                        T_99418 = Nothing
                    Case "99419"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_99419.DataSepaPlus = Kdata.record320
                        RecLen = T_99419.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_99419.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 11           '(ジャーナル・伝票・証書)
                        T_99419 = Nothing
                    Case Else
                        'エラー
                End Select

                WriteCount += 1

                ' 金額セパレータ
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)

                ' タンキング連番
                T_RIENT10.TANKING_DATA.bytTANKING_NO(0) = CType(WriteCount \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytTANKING_NO(1) = CType(WriteCount Mod 256, Byte)

                ' 次データアドレス
                If cnt + 1 = WriteCount Then
                    ' 最終行
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = 255
                Else
                    Dim NextAddr As Integer = 1024 + (WriteCount * 256)

                    Dim NextAddr0 As Integer = CType(NextAddr \ 16777216, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = CType(NextAddr0, Byte)

                    Dim NextAddr1 As Integer = CType((NextAddr Mod 16777216) \ 65536, Integer)
                    Dim Amari1 As Integer = CType((NextAddr Mod 16777216) Mod 65536, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = CType(NextAddr1, Byte)

                    Dim NextAddr2 As Integer = CType(Amari1 \ 256, Integer)
                    Dim Amari2 As Integer = CType(Amari1 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = CType(NextAddr2, Byte)

                    Dim NextAddr3 As Integer = CType(Amari2 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = CType(NextAddr3, Byte)
                End If

                ' 金店コード
                T_RIENT10.TANKING_DATA.strKINTEN_CD = T_RIENT77.TENPO_INFOREC(0).strKINKO_CD & T_RIENT77.TENPO_INFOREC(0).strSIT_CD

                Select Case Kdata.OpeCode.Substring(0, 2)
                    Case "48"
                        StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_48, 0, 256)
                    Case Else
                        StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_10, 0, 256)
                End Select

                '*****************************************
                '決済マスタに登録
                '*****************************************
                keskey.RecordNo = WriteCount
                If InsertKesMast(Kdata) = False Then
                    MainLOG.Write("資金決済マスタ登録", "失敗", "")
                    Return False
                End If
            Next

            ' 最終レコード
            T_RIENT10.TANKING_LAST.Init()
            StrmWrite.Write(T_RIENT10.TANKING_LAST.Data, 0, 512)

            ' タンキングヘッダ 書込件数 再書込
            ' 全店舗Ｔ件数
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(20 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN, 0, 2)

            ' 店舗Ｔ件数
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(36 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN, 0, 2)

            ' 店舗Ｔ終了ＦＤアドレス
            '***修正　前田　20080930****************************************
            'WriteCountが1以外なら、終了アドレスを正しく設定する
            If WriteCount <> 1 Then

                '***修正 maeda 2001006*******************************************
                '書込件数-1の値で終了アドレスを計算する(正確には最終レコードの開始アドレスのため)
                Dim FinishAddr As Integer = 1024 + ((WriteCount - 1) * 256)
                'Dim FinishAddr As Integer = 1024 + (WriteCount * 256)
                '****************************************************************

                Dim FinishAddr0 As Integer = CType(FinishAddr \ 16777216, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(0) = CType(FinishAddr0, Byte)

                Dim FinishAddr1 As Integer = CType((FinishAddr Mod 16777216) \ 65536, Integer)
                Dim FinishAmari1 As Integer = CType((FinishAddr Mod 16777216) Mod 65536, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(1) = CType(FinishAddr1, Byte)

                Dim FinishAddr2 As Integer = CType(FinishAmari1 \ 256, Integer)
                Dim FinishAmari2 As Integer = CType(FinishAmari1 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(2) = CType(FinishAddr2, Byte)

                Dim FinishAddr3 As Integer = CType(FinishAmari2 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(3) = CType(FinishAddr3, Byte)
            End If

            StrmWrite.Seek(48 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD, 0, 4)

            StrmWrite.Close()

        Catch ex As Exception
            MainLOG.Write("リエンタファイル作成", "失敗", ex.Message)
            Return False
        Finally

        End Try
        Return True
    End Function

    Private Function GetKaiji() As Boolean

        Dim sql As New StringBuilder(64)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            MainLOG.Write("(回次取得)開始", "成功", "")

            sql.Append("SELECT NVL(MAX(KAIJI_KR),0) AS MAX_KAIJI FROM KESSAIMAST")
            sql.Append(" WHERE SYORI_DATE_KR = " & SQ(strDate))

            If OraReader.DataReader(sql) = True Then
                keskey.Kaiji = CType(OraReader.GetInt64("MAX_KAIJI"), Integer) + 1
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

    '資金決済・手数料徴求実行待ちのフラグをすべて元に戻す
    Public Function ReturnFlg() As Integer
        Dim SQL As String
        Dim Ret As Integer = 0
        Try
            '念のため、一端クローズ
            If Not MainDB Is Nothing Then MainDB.Close()
            MainDB = New CASTCommon.MyOracle

            SQL = "UPDATE SCHMAST SET"
            SQL &= " KESSAI_FLG_S = '0'"
            SQL &= " WHERE KESSAI_YDATE_S = '" & ParaKessaiDate & "'"
            SQL &= "   AND KESSAI_FLG_S = '2'"

            Ret = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("決済待ち取消", "成功", Ret & "件")

            SQL = "UPDATE SCHMAST SET"
            SQL &= " TESUUTYO_FLG_S = '0'"
            SQL &= " WHERE TESUU_YDATE_S = '" & ParaKessaiDate & "'"
            SQL &= "   AND TESUUTYO_FLG_S = '2'"

            Ret = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("手数料徴求待ち取消", "成功", Ret & "件")

            MainDB.Commit()
            Return 0
        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write("決済･手数料徴求待ち取消", "失敗", ex.ToString)
            Return -1
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function

    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
    ''' <summary>
    ''' 決済科目の「その他」の信金科目コードを取得する
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetKessaiKamokuCode_ETC()

        Dim TxtFile As String = Path.Combine(GetFSKJIni("COMMON", "TXT"), "Common_決済科目.TXT")

        'ファイルが無ければ抜ける
        '2017/07/25 タスク）綾部 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
        'If Not File.Exists(TxtFile) Then Return
        If Not File.Exists(TxtFile) Then
            KessaiKamokuCode_ETC = "xx"
            Return
        End If
        '2017/07/25 タスク）綾部 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END

        Try
            Using sr As New StreamReader(TxtFile, Encoding.GetEncoding("SHIFT-JIS"))
                While sr.Peek > -1
                    Dim strLineData() As String = sr.ReadLine().Split(","c)
                    If strLineData(1).Trim = "その他" Then
                        KessaiKamokuCode_ETC = strLineData(0).Trim
                        '2017/07/25 タスク）綾部 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
                        'Exit While
                        Return
                        '2017/07/25 タスク）綾部 CHG 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END
                    End If
                End While
            End Using

            '2017/07/25 タスク）綾部 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
            KessaiKamokuCode_ETC = "xx"
            '2017/07/25 タスク）綾部 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END

        Catch ex As Exception
            MainLOG.Write("「Common_決済科目.TXT」読込失敗", "失敗", ex.ToString)
            '2017/07/25 タスク）綾部 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- START
            KessaiKamokuCode_ETC = "xx"
            '2017/07/25 タスク）綾部 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END
        End Try

    End Sub
    '2017/05/29 タスク）西野 ADD 標準版修正（決済科目に「xx:その他」を追加）-------------------------- END

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
