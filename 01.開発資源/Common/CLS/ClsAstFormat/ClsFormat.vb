Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports CASTCommon.ModPublic

'*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
'*** Str Upd 2015/12/01 SO)荒木 for 登録出口対応 ***
Imports System.Configuration
Imports System.Reflection
Imports System.Xml
'*** End Upd 2015/12/01 SO)荒木 for 登録出口対応 ***

' データフォーマット基本クラス
Public Class CFormat
    Implements IDisposable

    Protected clsfusion As New clsFUSION.clsMain

    '*** Str Add 2015/12/01 SO)荒木 for 登録出口対応 ***
    Private FmtKubun As String = ""
    '*** End Add 2015/12/01 SO)荒木 for 登録出口対応 ***

    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
    Protected MOTIKOMI_DATE As String = ""  '持込期日
    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

    '2014/05/21 saitou 標準版 DEL -------------------------------------------------->>>>
    ''コムフロント振替日マルチ対応 >>
    'Private PreFURI_DATE As String = ""
    ''コムフロント振替日マルチ対応 <<
    '2014/05/21 saitou 標準版 DEL --------------------------------------------------<<<<

    ' 国民年金保険料 取引先コード
    '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応（別クラスの個別メソッドから参照可能とする） ***
    'Protected KokuminNenkinTori As String = CASTCommon.GetFSKJIni("KEKKA", "KOKUMINNENKIN")
    Public KokuminNenkinTori As String = CASTCommon.GetFSKJIni("KEKKA", "KOKUMINNENKIN")
    '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応（別クラスの個別メソッドから参照可能とする） ***

    '2010/10/07.Sakon　インプットエラーの配信対象可否
    Public SouInputErr As String = CASTCommon.GetFSKJIni("JIFURI", "SOU_INPUTERR")

    ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
    Private strUketukeDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private strUketukeTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END

    ' 固定長構造体用インターフェース
    Protected Friend Interface IFormat
        ' データ
        Property Data() As String
    End Interface

    ' SHIT-JISエンコーディング
    Protected Friend Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")
    ' EBCDIC(ｶﾀｶﾅ)エンコーディング
    Protected Friend Shared EncdE As System.Text.Encoding = System.Text.Encoding.GetEncoding("IBM290")

    ' 使用するエンコーディング
    Protected Friend CurrEncd As System.Text.Encoding

    ' 読み書きストリーム
    Protected IOfileStream As FileStream

    ' MEIMAST用OracleReader
    Protected IOreader As CASTCommon.MyOracleReader

    ' 1行データ読み込み用
    Protected ReadByte() As Byte
    Protected mRecordData As String         ' 文字変換あり
    Protected mRecordDataNoChange As String ' 文字変換前
    Public Property RecordData() As String
        Get
            Return mRecordData
        End Get
        Set(ByVal Value As String)
            '*** 修正 mitsu 2008/06/03 NULL文字置換(※不具合があれば外す！) ***
            mRecordData = Value
            'mRecordData = Value.Replace(Convert.ToChar(0), " ")
            '******************************************************************
            mRecordDataNoChange = Value
        End Set
    End Property
    Public ReadOnly Property RecordDataNoChange() As String
        Get
            Return mRecordDataNoChange
        End Get
    End Property

    '*** 修正 2008/09/30 EBCDICデータ対応 ***
    '返還書込み用 レコードをバイト配列で返す
    Public ReadOnly Property RecordDataToBytes() As Byte()
        Get
            Return EncdJ.GetBytes(mRecordData)
        End Get
    End Property

    'バイナリデータ(EBCDICデータ)使用フラグ
    Protected BinDataMode As Boolean = False
    Public ReadOnly Property BinMode() As Boolean
        Get
            Return BinDataMode
        End Get
    End Property

    'バイナリデータ(EBCDICデータ)格納用
    Public ReadByteBin As BinaryString
    Public Property RecordDataBin() As Byte()
        Get
            If ReadByteBin Is Nothing Then
                Return Nothing
            Else
                Return ReadByteBin.Buffer
            End If
        End Get
        Set(ByVal Value As Byte())
            If Not Value Is Nothing Then
                If ReadByteBin Is Nothing Then
                    ReadByteBin = New BinaryString(Value, EncdE)
                Else
                    ReadByteBin.Buffer = Value
                End If
            End If
        End Set
    End Property
    '****************************************

    Friend code_flg As Boolean = False '大阪センタ対応－コード変換フラグ

    ' ＦＴＲＡＮ＋ パラメータファイル
    Protected Friend FtranPfile As String
    Protected Friend FtranIBMPfile As String
    Protected Friend FtranIBMBinaryPfile As String

    Protected Friend CMTBlockSize As Integer

    ' データ情報
    Friend Structure DATAINFORMATION
        Dim FileName As String                      ' ファイル名
        Dim FileLen As Long                         ' ファイル長
        Dim Encoding As EncodingType                ' エンコーディング
        Dim NewLine As NewLineType                  ' 改行コード
        Dim Message As String                       ' 情報
        Dim RecoedLen As Integer                    ' データ長
        Dim LenOfOneRec As Integer                  ' １レコードの長さ
        Dim MinRecordCode() As String               ' 最低分のレコード区分
        '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- START
        Dim WorkLen As Integer                      '中間ファイルのレコードサイズ
        '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- END
    End Structure
    Friend DataInfo As DATAINFORMATION

    ' １レコード前のレコード区分
    Friend BeforeRecKbn As String

    ' ヘッダレコード区分
    Protected Friend HeaderKubun() As String
    ' データレコード区分
    Protected Friend DataKubun() As String
    ' トレーラレコード区分
    Protected Friend TrailerKubun() As String

    ' ファイル名プロパティ
    Public ReadOnly Property FileName() As String
        Get
            Return DataInfo.FileName
        End Get
    End Property

    ' データ長プロパティ
    Public Property RecordLen() As Integer
        Get
            Return DataInfo.RecoedLen
        End Get
        Set(ByVal Value As Integer)
            DataInfo.RecoedLen = Value
        End Set
    End Property

    ' CRLFプロパティ
    Public ReadOnly Property CRLF() As Boolean
        Get
            If DataInfo.NewLine = NewLineType.None Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    ' FTRAN パラメータファイルプロパティ
    Public ReadOnly Property FTRANP() As String
        Get
            Return FtranPfile
        End Get
    End Property

    ' FTRAN IBM パラメータファイルプロパティ
    Public ReadOnly Property FTRANIBMP() As String
        Get
            Return FtranIBMPfile
        End Get
    End Property

    ' FTRAN IBM パラメータファイルプロパティ
    Public ReadOnly Property FTRANIBMBINARYP() As String
        Get
            Return FtranIBMBinaryPfile
        End Get
    End Property

    ' CMTブロックサイズ パラメータファイルプロパティ
    Public ReadOnly Property BLOCKSIZE() As Integer
        Get
            Return CMTBlockSize
        End Get
    End Property

    'Public Shared ErrorNum As Err.InputErrorType    ' エラー番号

    ' 中点ＮＧ
    '*** 修正 mitsu 2009/04/17 &$*は規定外文字 ***
    Protected Shared ReadOnly RegularStringVerA As String =
                                            "\,.｢｣()-/" &
                                            "0123456789" &
                                            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" &
                                            "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝｦﾞﾟ" &
                                            " "
    '*********************************************
    ' 中点ＯＫ
    Protected Shared ReadOnly RegularStringVerB As String = RegularStringVerA

    ' ２つのパターンを持つ場合にチェックする
    '*** 修正 mitsu 2008/09/30 コンパイルオプションを使用する ***
    'Protected Shared ReadOnly RegularRegexVerA As New Regex("[^" & Regex.Escape(RegularStringVerA) & "]")
    'Protected Shared ReadOnly RegularRegexVerB As New Regex("[^" & Regex.Escape(RegularStringVerB) & "]")
    'Protected Shared ReadOnly RegularRegexVerPlus As New Regex("[\+]")
    Protected Shared ReadOnly RegularRegexVerA As New Regex("[^" & Regex.Escape(RegularStringVerA) & "]", RegexOptions.Compiled)
    Protected Shared ReadOnly RegularRegexVerB As New Regex("[^" & Regex.Escape(RegularStringVerB) & "]", RegexOptions.Compiled)
    Protected Shared ReadOnly RegularRegexVerPlus As New Regex("[\+]", RegexOptions.Compiled)
    '************************************************************

    Private ReplaceStringPattern As Hashtable   '規定外文字変換パターン

    '改行コード
    Friend Enum NewLineType As Integer
        None = 0                                ' なし
        CrLf                                    ' CRLF
        Cr                                      ' CR
        Lf                                      ' LF
    End Enum

    'エンコーディング
    Friend Enum EncodingType As Integer
        NONE                                    ' なし
        SJIS                                    ' SHIFT-JIS
        EBCDIC                                  ' EBCDIC
    End Enum

    Public SitenYomikae As String = CASTCommon.GetFSKJIni("YOMIKAE", "TENPO")  '2010/02/03 支店読替区分　追加
    Public KouzaYomikae As String = CASTCommon.GetFSKJIni("YOMIKAE", "KOUZA")  '2010/02/03 口座読替区分　追加

    '　明細データ
    Public Structure MEISAI
        ' 区分
        Dim DATA_KBN As String                  ' データ区分
        Dim CODE_KBN As String                  ' コード区分
        Dim RECORD_NO As Integer                ' レコード番号
        ' ヘッダ情報
        Dim SYUBETU_CODE As String              ' 種別コード
        Dim ITAKU_CODE As String                ' 委託者コード
        Dim FURIKAE_DATE As String              ' 振替日
        Dim FURIKAE_DATE_MOTO As String         ' 振替日（変換元データ）
        Dim KIGYO_SEQ As String                 ' 企業ＳＥＱ
        Dim ITAKU_KIN As String                 ' 委託者金融機関コード
        Dim ITAKU_SIT As String                 ' 委託者支店コード
        Dim I_SIT_NNAME As String               ' 委託者支店名
        Dim ITAKU_KAMOKU As String              ' 委託者科目
        Dim ITAKU_KOUZA As String               ' 委託者口座番号
        Dim ITAKU_KNAME As String               ' 委託者名カナ
        ' データ情報
        Dim KEIYAKU_KIN As String               ' 契約金融機関コード
        Dim KEIYAKU_SIT As String               ' 契約金融機関支店コード
        Dim KEIYAKU_NO As String                ' 契約者番号
        Dim KEIYAKU_KNAME As String             ' 契約者名カナ
        Dim KEIYAKU_KAMOKU As String            ' 契約者科目
        Dim KEIYAKU_BsKAMOKU As String          ' 契約者科目 変換前
        Dim KEIYAKU_KOUZA As String             ' 契約者口座番号
        Dim FURIKIN As Decimal                  ' 振替金額
        Dim FURIKIN_MOTO As String              ' 振替金額（変換元データ）
        Dim FURIKETU_CODE As Integer            ' 振替結果
        Dim FURIKETU_MOTO As String             ' 振替結果（変換元データ）
        Dim SINKI_CODE As String                ' 新規コード
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        Dim NS_KBN As String                    ' 入出金区分
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        Dim KTEKIYO As String                   ' カナ摘要
        Dim NTEKIYO As String                   ' 漢字摘要
        Dim JYUYOKA_NO As String                ' 需要家番号
        Dim TEISEI_SIT As String                ' 訂正後支店コード
        Dim TEISEI_KAMOKU As String             ' 訂正後科目
        Dim TEISEI_KOUZA As String              ' 訂正後口座
        Dim YOBI1 As String                     ' 予備１
        Dim YOBI2 As String                     ' 予備２
        Dim YOBI3 As String                     ' 予備３
        Dim YOBI4 As String                     ' 予備４
        Dim YOBI5 As String                     ' 予備５
        Dim YOBI6 As String                     ' 予備６
        Dim YOBI7 As String                     ' 予備７
        Dim YOBI8 As String                     ' 予備８
        Dim YOBI9 As String                     ' 予備９
        Dim YOBI10 As String                    ' 予備１０
        ' トレーラ情報
        Dim TOTAL_IRAI_KEN As Decimal           ' トレーラ依頼合計
        Dim TOTAL_IRAI_KEN_MOTO As String       ' トレーラ依頼合計
        Dim TOTAL_IRAI_KIN As Decimal           ' トレーラ金額合計
        Dim TOTAL_IRAI_KIN_MOTO As String       ' トレーラ金額合計
        Dim TOTAL_ZUMI_KEN As Decimal           ' トレーラ済件数合計
        Dim TOTAL_ZUMI_KEN_MOTO As String       ' トレーラ済件数合計
        Dim TOTAL_ZUMI_KIN As Decimal           ' トレーラ済金額合計
        Dim TOTAL_ZUMI_KIN_MOTO As String       ' トレーラ済金額合計
        Dim TOTAL_FUNO_KEN As Decimal           ' トレーラ不能件数合計
        Dim TOTAL_FUNO_KEN_MOTO As String       ' トレーラ不能件数合計
        Dim TOTAL_FUNO_KIN As Decimal           ' トレーラ不能金額合計
        Dim TOTAL_FUNO_KIN_MOTO As String       ' トレーラ不能金額合計

        ' 不能結果情報
        Dim FURI_CODE As String                 ' 振替コード
        Dim KIGYO_CODE As String                ' 企業コード
        Dim FURIKETU_CENTERCODE As String       ' 不能事由コード
        Dim MINASI As String                    ' みなし完了
        Dim TEISEI_AKAMOKU As String            ' 訂正後相手科目
        Dim TEISEI_AKOUZA As String             ' 訂正後相手口座

        ' 可変情報
        Dim MOTIKOMI_SEQ As Integer             ' 持ち込みＳＥＱ 同一取引先主コード・振替日内の順番
        Dim SCH_UPDATE_FLG As Boolean           ' スケジュールアップデートフラグ
        Dim FILE_SEQ As Integer                 ' ＦＩＬＥＳＥＱ 同一媒体内の順番

        Dim FURIKEN As Integer                  ' 計算対象件数（データレコード読み込み時は１，以外は０）
        Dim TOTAL_KEN As Decimal                ' 合計依頼件数 計算値
        '2009/12/03 追加 ============================================================
        Dim TOTAL_KEN2 As Decimal                ' 合計依頼件数 計算値(0円データ除く)
        '============================================================================
        Dim TOTAL_KIN As Decimal                ' 合計依頼金額 計算値
        Dim TOTAL_IJO_KEN As Decimal            ' 合計インプットエラー（不能）件数 計算値
        Dim TOTAL_IJO_KIN As Decimal            ' 合計インプットエラー（不能）金額 計算値
        Dim TOTAL_NORM_KEN As Decimal           ' 合計正常件数 計算値
        Dim TOTAL_NORM_KIN As Decimal           ' 合計正常金額 計算値
        Dim TOTAL_NORM_KEN2 As Decimal          ' 合計正常件数 計算値(0円データ除く)

        Dim DUPLICATE_KBN As String             ' 二重持込区分

        Dim FURIKETU_KEKKA As String            ' 明細マスタ振替結果

        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        Dim TimeOver As Boolean                 ' 時間外受付区分
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- START
        Dim KinTenSoui As Boolean               ' 金融機関名、支店名相違フラグ
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- END

        Dim OLD_KIN_NO As String                '2009/09/17　読替前金融機関コード
        Dim OLD_SIT_NO As String                '2009/09/17　読替前支店コード
        Dim OLD_KOUZA As String                 '2009/09/17　読替前口座番号
        Dim IDOU_DATE As String                 '2009/09/19　異動日
        Dim TESUU_KIN As Integer                '2010/01/22  手数料

        '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------START
        Dim HENKANKBN As String                 '返還区分
        '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------END
        Public Sub Init()
            SYUBETU_CODE = ""
            ITAKU_CODE = ""
            FURIKAE_DATE = ""
            FURIKAE_DATE_MOTO = ""
            KIGYO_SEQ = "00000000"
            ITAKU_KIN = ""
            ITAKU_SIT = ""
            ITAKU_KAMOKU = ""
            ITAKU_KOUZA = ""
            ITAKU_KNAME = ""
            KEIYAKU_KIN = ""
            KEIYAKU_SIT = ""
            KEIYAKU_NO = ""
            KEIYAKU_KNAME = ""
            KEIYAKU_KAMOKU = ""
            KEIYAKU_KOUZA = ""
            FURIKIN = 0
            FURIKETU_CODE = 0
            FURIKETU_CENTERCODE = "0"           ' 2008.04.22 ADD
            SINKI_CODE = ""
            '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
            NS_KBN = ""             ' 入出金区分
            '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            KTEKIYO = ""
            NTEKIYO = ""
            JYUYOKA_NO = ""
            TEISEI_SIT = ""
            TEISEI_KAMOKU = ""
            TEISEI_KOUZA = ""
            DATA_KBN = ""

            YOBI1 = ""
            YOBI2 = ""
            YOBI3 = ""
            YOBI4 = ""
            YOBI5 = ""
            YOBI6 = ""
            YOBI7 = ""
            YOBI8 = ""
            YOBI9 = ""
            YOBI10 = ""

            TOTAL_IRAI_KEN = 0
            TOTAL_IRAI_KIN = 0
            TOTAL_ZUMI_KEN = 0
            TOTAL_ZUMI_KIN = 0
            TOTAL_FUNO_KEN = 0
            TOTAL_FUNO_KIN = 0

            FURIKEN = 0
            TOTAL_KEN = 0
            TOTAL_KIN = 0
            TOTAL_IJO_KEN = 0
            TOTAL_IJO_KIN = 0
            TOTAL_NORM_KEN = 0
            TOTAL_NORM_KIN = 0

            FURIKETU_KEKKA = ""

            OLD_KIN_NO = ""
            OLD_SIT_NO = ""

            '2009/10/21 追加
            FURI_CODE = ""
            KIGYO_CODE = ""

            '2010/01/22 追加
            TESUU_KIN = 0

            '2010.03.02 start マルチ対応
            TOTAL_KEN2 = 0
            TOTAL_NORM_KEN2 = 0
            '2010.03.02 end

            '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------START
            HENKANKBN = ""
            '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------END
            '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
            TimeOver = True                ' 時間外受付区分
            '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END
        End Sub

        Public Sub InitData()
            KEIYAKU_KIN = ""
            KEIYAKU_SIT = ""
            KEIYAKU_NO = ""
            KEIYAKU_KNAME = ""
            KEIYAKU_KAMOKU = ""
            KEIYAKU_KOUZA = ""
            FURIKIN = 0
            FURIKETU_CODE = 0
            FURIKETU_CENTERCODE = "0"           ' 2008.04.22 ADD
            SINKI_CODE = ""
            '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
            NS_KBN = ""             ' 入出金区分
            '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
            KTEKIYO = ""
            NTEKIYO = ""
            JYUYOKA_NO = ""
            TEISEI_SIT = ""
            TEISEI_KAMOKU = ""
            TEISEI_KOUZA = ""
            DATA_KBN = ""
            YOBI1 = ""
            YOBI2 = ""
            YOBI3 = ""
            YOBI4 = ""
            YOBI5 = ""
            YOBI6 = ""
            YOBI7 = ""
            YOBI8 = ""
            YOBI9 = ""
            YOBI10 = ""

            TOTAL_IRAI_KEN = 0
            TOTAL_IRAI_KIN = 0
            TOTAL_ZUMI_KEN = 0
            TOTAL_ZUMI_KIN = 0
            TOTAL_FUNO_KEN = 0
            TOTAL_FUNO_KIN = 0

            FURIKETU_KEKKA = ""

            OLD_KIN_NO = ""
            OLD_SIT_NO = ""

            '2010/01/22 追加
            TESUU_KIN = 0
            '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------START
            HENKANKBN = ""
            '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------END
        End Sub
    End Structure
    Public InfoMeisaiMast As MEISAI

    '***修正 meada 2008/05/12*****************************************************
    'データレコードの金融機関名をセット
    '　明細データ
    Public Structure MEISAI2
        ' データ情報
        Dim KEIYAKU_KIN_KNAME As String '契約金融機関名
        Dim KEIYAKU_SIT_KNAME As String '契約店舗名

        '***修正 meada 20181001 maeda 標準修正*****************************************************
        Dim TENMAST_SIT_KNAME As String 'TENMAST店舗名
        '***修正 meada 20181001 maeda 標準修正*****************************************************

        Public Sub Init()
            KEIYAKU_KIN_KNAME = "" '契約金融機関名
            KEIYAKU_SIT_KNAME = "" '契約店舗名
            TENMAST_SIT_KNAME = ""
        End Sub

    End Structure
    Public InfoMeisaiMast2 As MEISAI2
    '************************************************************************

    'エラー定義
    Public Class Err
        ' 2008.04.08 ADD >> 新規コード異常追加
        '***修正 maeda 2008/05/13***************************************************************
        'エラー定義追加
        '金融機関コード異常
        '店舗統廃合
        '規定外文字あり
        '*** 修正 mitsu 2008/07/01 振替結果コード異常追加 ***
        '振替結果コード異常
        '****************************************************

        Private Shared ReadOnly InputErrorString() As String = {
                                                "",
                                                "店番数値異常",
                                                "科目異常",
                                                "口座番号異常",
                                                "カナ氏名異常",
                                                "金額異常",
                                                "自行店番異常",
                                                "他行異常",
                                                "他行店番異常",
                                                "金額0円",
                                                "振替日",
                                                "提携外",
                                                "伝送未送信",
                                                "CMTデータ作成",
                                                "新規コード異常",
                                                "金融機関コード異常",
                                                "店舗統廃合",
                                                "規定外文字あり",
                                                "振替結果コード異常"
                                                }
        'Private Shared ReadOnly InputErrorString() As String = { _
        '                        "", _
        '                        "店番数値異常", _
        '                        "科目異常", _
        '                        "口座番号異常", _
        '                        "カナ氏名異常", _
        '                        "金額異常", _
        '                        "自行店番異常", _
        '                        "他行異常", _
        '                        "他行店番異常", _
        '                        "金額0円", _
        '                        "振替日", _
        '                        "提携外", _
        '                        "伝送未送信", _
        '                        "CMTデータ作成", _
        '                        "新規コード異常" _
        '                        }


        '***************************************************************************************
        Public Enum InputErrorType As Integer
            ' 2008.04.08 ADD >> 新規コード異常追加
            Tenban = 1
            Kamoku
            Kouza
            Kana
            Kingaku
            JikouTenban
            Takou
            TakouTenban
            KingakuZero
            Furikaebi
            TeikeiGai
            DensoMi
            CmtMeked
            SinkiCode
            '***修正 maeda 2008/05/13*************************************************************
            GinkouCode
            TenpoTougou
            Kiteigaimoji
            '*************************************************************************************
            '*** 修正 mitsu 2008/07/01 振替結果コード異常追加 ***
            FuriketuCode
            '****************************************************
        End Enum
        Public Shared Function Name(ByVal num As InputErrorType) As String
            Try
                Return InputErrorString(num)
            Catch ex As Exception
                Return ""
            End Try
        End Function
    End Class

    ' インプットエラー構造体
    Public Structure INPUTERROR
        Dim RECNO As Long
        Dim KIN As String
        Dim SIT As String
        Dim KAMOKU As String
        Dim KOUZA As String
        Dim JYUYOKA_NO As String
        Dim KNAME As String
        Dim FURIKIN As String
        Dim ERRINFO As String
        Public WriteOnly Property DATA() As MEISAI
            Set(ByVal Value As MEISAI)
                RECNO = Value.RECORD_NO
                KIN = Value.KEIYAKU_KIN
                SIT = Value.KEIYAKU_SIT
                KAMOKU = Value.KEIYAKU_KAMOKU
                KOUZA = Value.KEIYAKU_KOUZA
                JYUYOKA_NO = Value.JYUYOKA_NO
                KNAME = Value.KEIYAKU_KNAME
                FURIKIN = Value.FURIKIN_MOTO
                ERRINFO = ""
            End Set
        End Property
    End Structure
    Public InErrorArray As New ArrayList

    ' エラー番号
    Protected mnErrorNumber As Long = 0
    Public ReadOnly Property ErrorNumber() As Long
        Get
            Return mnErrorNumber
        End Get
    End Property

    ' 摘要データ
    Protected mTekiyoData() As String

    ' ORACLE
    Protected Friend OraDB As CASTCommon.MyOracle
    ' LOG
    Protected Friend BLOG As CASTCommon.BatchLOG

    ' 休日テーブル
    Public HolidayList As New ArrayList(10)

    ' 各ヘッダごとの依頼データ情報
    Protected mInfoComm As CAstBatch.CommData
    Public Property ToriData() As CAstBatch.CommData
        Get
            Return mInfoComm
        End Get
        Set(ByVal Value As CAstBatch.CommData)
            mInfoComm = Value
            'mInfoComm = New CAstBatch.CommData(Value.INFOToriMast, Value.INFOParameter)
        End Set
    End Property

    ' 自金庫コード
    Public ReadOnly JIKINKO As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

    ' 不能予定日数
    Protected mnJifuriFunou As Long = CASTCommon.GetFSKJIniNum("JIFURI", "FUNOU")
    ' 配信予定日数
    Protected mnJifuriHaisin As Long = CASTCommon.GetFSKJIniNum("JIFURI", "HAISIN")
    ' 回収予定日数
    Protected mnJifuriKaisyu As Long = CASTCommon.GetFSKJIniNum("JIFURI", "KAISYU")

    ' チェックデジット
    Protected mCheckDigitFlag As String = CASTCommon.GetFSKJIni("COMMON", "CHKDJT")

    '2010/09/08.Sakon　新規コードチェックフラグ追加
    Protected mSinkiCheckFlag As String = CASTCommon.GetFSKJIni("COMMON", "SINKICHECK")

    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
    '受付期日チェック(0:チェックしない、1:チェックする)
    Protected INI_UKETUKE_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_KIJITU_CHK")
    '受付期日(何営業日前か )
    Protected INI_UKETUKE_KIJITU As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_KIJITU")
    '持込期日チェック(0:チェックしない、1:チェックする)
    Protected INI_MOTIKOMI_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MOTIKOMI_KIJITU_CHK")
    '受付時間チェック(0:チェックしない、1:チェックする)
    Protected INI_UKETUKE_JIKAN_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_JIKAN_CHK")
    '受付時間(受付可能時間 HHMMSS)
    Protected INI_UKETUKE_JIKAN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_JIKAN")
    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END
    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- START
    Protected INI_KINKO_SOUI_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KINKO_SOUI_CHK")
    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- END
    '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（種別コード追加(給与・賞与で同一委託者コードが存在するため)）----- START
    Protected INI_JIFURI_SYUBETU_KEY As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "JIFURI_SYUBETU_KEY")
    '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（種別コード追加(給与・賞与で同一委託者コードが存在するため)）----- END
    '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（振替日休日補正対応）---------------- START
    Protected INI_IRAI_KYUJITU_HOSEI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IRAI_KYUJITU_HOSEI")
    '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（振替日休日補正対応）---------------- END
    '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（口座情報チェック）------------------ START
    Protected INI_S_KDBMAST_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KDBMAST_CHK")
    '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（口座情報チェック）------------------ END

    '*** 修正 mitsu 2008/06/24 ヘッダ数カウント ***
    Protected HeaderCnt As Integer = 0
    '**********************************************

    Public TAKOU_ON As Boolean                  ' 他行あり

    '2018/02/14 saitou 広島信金(RSV2標準) ADD 金融機関名置換対応 ---------------------------------------- START
    Protected ReplaceKinNamePattern As Hashtable
    Protected ReplaceSitNamePattern As Hashtable
    '2018/02/14 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------------- END

    ' New
    Public Sub New()
        With DataInfo
            .Encoding = EncodingType.NONE
            .NewLine = NewLineType.None
            .RecoedLen = 0
            .LenOfOneRec = 0
            .MinRecordCode = New String() {}
            '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- START
            .WorkLen = 0
            '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- END
        End With

        ' デフォルトのエンコーディングをSHIFT-JISに設定する
        CurrEncd = EncdJ

        InfoMeisaiMast.MOTIKOMI_SEQ = 0
        InfoMeisaiMast.FILE_SEQ = 0

        InfoMeisaiMast.DUPLICATE_KBN = ""
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- START
        InfoMeisaiMast.KinTenSoui = False
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（金融機関名相違対応）---------------- END

        FtranPfile = ""
        FtranIBMPfile = ""

        ReplaceStringPattern = New Hashtable

        Try
            Dim sr As New StreamReader(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "規定外文字変換パターン.txt"), EncdJ)
            While sr.Peek > -1
                Dim s() As String = sr.ReadLine().Split(","c)
                ReplaceStringPattern.Add(s(0), s(1))
            End While
            sr.Close()

        Catch ex As Exception
            ReplaceStringPattern = Nothing
            Throw
        End Try

        '2018/02/14 saitou 広島信金(RSV2標準) ADD 金融機関名置換対応 ---------------------------------------- START
        '金融機関名置換パターン、支店名置換パターンを取り込む
        ReplaceKinNamePattern = New Hashtable
        ReplaceSitNamePattern = New Hashtable
        Dim RepKinNameTxt As StreamReader = Nothing

        Try
            Dim RepKinNameTxtName As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "金融機関名変換パターン.TXT")
            Dim SectionName As String = String.Empty

            If File.Exists(RepKinNameTxtName) Then
                RepKinNameTxt = New StreamReader(RepKinNameTxtName, EncdJ)
                While RepKinNameTxt.Peek > -1
                    Dim s() As String = RepKinNameTxt.ReadLine().Split("="c)
                    If s.Length = 1 Then
                        If s(0).Trim = "[KIN]" OrElse s(0).Trim = "[SIT]" Then
                            SectionName = s(0)
                        Else
                            SectionName = String.Empty
                        End If

                    ElseIf s.Length = 2 Then
                        Select Case SectionName
                            Case "[KIN]"
                                If ReplaceKinNamePattern.ContainsKey(s(0)) = False Then
                                    ReplaceKinNamePattern.Add(s(0), s(1))
                                End If

                            Case "[SIT]"
                                If ReplaceSitNamePattern.ContainsKey(s(0)) = False Then
                                    ReplaceSitNamePattern.Add(s(0), s(1))
                                End If
                        End Select
                    End If
                End While

                RepKinNameTxt.Close()
                RepKinNameTxt = Nothing

            End If

        Catch ex As Exception
            Throw
        Finally
            If Not RepKinNameTxt Is Nothing Then
                RepKinNameTxt.Close()
                RepKinNameTxt = Nothing
            End If
        End Try
        '2018/02/14 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------------- END

    End Sub

    ' New
    Public Sub New(ByVal filename As String)
        MyClass.New()

        DataInfo.FileName = filename
    End Sub

    ' New
    Public Sub New(ByVal filename As String, ByVal len As Integer)
        MyClass.New()

        DataInfo.FileName = filename
        DataInfo.RecoedLen = len
        '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- START
        DataInfo.WorkLen = 120
        '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- END
    End Sub

    Public Sub Dispose() Implements System.IDisposable.Dispose
        Try
            If Not IOfileStream Is Nothing Then
                IOfileStream.Close()
            End If
            IOfileStream = Nothing
        Catch ex As Exception

        End Try
    End Sub

#Region "オラクル"
    Public WriteOnly Property Oracle() As CASTCommon.MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            OraDB = Value

            ' 休日マスタを配列に読み込み
            HolidayList.Clear()
            Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
            Dim SQL As New StringBuilder("SELECT YASUMI_DATE_Y,YASUMI_NAME_Y FROM YASUMIMAST ORDER BY YASUMI_DATE_Y")
            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF = True
                    HolidayList.Add(OraReader.GetValue(0).ToString)

                    OraReader.NextRead()
                Loop
            End If
            OraReader.Close()
        End Set
    End Property
#End Region

#Region "ログ"
    Public WriteOnly Property LOG() As CASTCommon.BatchLOG
        Set(ByVal Value As CASTCommon.BatchLOG)
            BLOG = Value
        End Set
    End Property
#End Region

    '
    ' 機能　 ： フォーマット区分から，各フォーマットクラスを取得する
    '
    ' 引数   ： ARG1 - 起動パラメータ情報
    '
    ' 戻り値 ： 各フォーマットクラス
    '
    ' 備考　 ：
    '
    Public Shared Shadows Function GetFormat(ByVal para As CAstBatch.CommData.stPARAMETER) As CFormat
        Try
            Select Case para.FMT_KBN
                Case "00", "20", "21", "04", "05" '依頼書･伝票追加
                    If para.FSYORI_KBN = "3" Then
                        '全銀フォーマット（振込）
                        '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ START
                        '改行なしのレコード長を設定できるようにする
                        Dim fmt As CFormat
                        Select Case para.CODE_KBN
                            Case "2"
                                fmt = New CAstFormat.CFormatFurikomi(119)
                            Case "3"
                                fmt = New CAstFormat.CFormatFurikomi(118)
                            Case Else
                                fmt = New CAstFormat.CFormatFurikomi
                        End Select
                        Return fmt
                        'Return New CAstFormat.CFormatFurikomi
                        '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ END
                    Else
                        '全銀フォーマット（口振）,集金代行サービス
                        '*** Str Upd 2015/12/01 SO)荒木 for 登録出口対応 ***
                        'Return New CAstFormat.CFormatZengin
                        '2017/05/24 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- START
                        '改行なしのレコード長を設定できるようにする
                        Dim fmt As CFormat
                        Select Case para.CODE_KBN
                            Case "2"
                                fmt = New CAstFormat.CFormatZengin(119)
                            Case "3"
                                fmt = New CAstFormat.CFormatZengin(118)
                            Case Else
                                fmt = New CAstFormat.CFormatZengin
                        End Select
                        'Dim fmt As CFormat = New CAstFormat.CFormatZengin
                        '2017/05/24 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- END
                        fmt.setFmtKubun(para.FMT_KBN)

                        Return fmt
                        '*** End Upd 2015/12/01 SO)荒木 for 登録出口対応 ***
                    End If
                Case "01"
                    '***** 2009/10/02 kakiwaki *******
                    '       '地公体フォーマット350
                    '       Return New CAstFormat.CFormatZeikin350
                    'NHKフォーマットに変更
                    Return New CAstFormat.CFormatNHK
                    '**************** 2009/10/02 *****
                Case "02"
                    '国税フォーマット
                    Return New CAstFormat.CFormatKokuzei
                Case "03"
                    '年金振込フォーマット
                    Return New CAstFormat.CFormatNenkin
                Case "06"
                    '***** 2009/09/30 kakiwaki *******
                    '       '300バイトフォーマット(岡崎市税)
                    '       Return New CAstFormat.CFormatZeikin300
                    '**************** 2009/09/30 *****
                Case "TO"
                    ' センタ直接持ち込み
                    Return New CAstFormat.CFormatTokCenter
                Case "MT"
                    ' 自振不能金庫返還ＭＴ
                    Select Case CASTCommon.GetFSKJIni("COMMON", "CENTER")
                        Case "0" '2010/12/24 信組対応 SKC不能フォーマット追加
                            Return New CAstFormat.CFormatSKCFunou
                        Case "1" '北海道センター

                        Case "2", "3", "5", "6"
                            Return New CAstFormat.FUNOU_164_DATA
                        Case "4" '東海センター
                            Return New CAstFormat.CFormatTokFunou
                        Case "7"

                        Case Else
                            Return Nothing
                    End Select

                Case "SC"
                    ' ＳＳＣ結果
                    Return New CAstFormat.ClsFormatSSCKekka
                Case Else
                    '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                    'Return Nothing
                    If IsNumeric(para.FMT_KBN) Then
                        Dim nFmtKbn As Integer = CInt(para.FMT_KBN)
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            '*** Str Upd 2015/12/01 SO)荒木 for 登録出口対応 ***
                            'Return New CAstFormat.CFormatXML(para.FMT_KBN)
                            Dim fmt As CFormat = New CAstFormat.CFormatXML(para.FMT_KBN)
                            fmt.setFmtKubun(para.FMT_KBN)
                            Return fmt
                            '*** End Upd 2015/12/01 SO)荒木 for 登録出口対応 ***
                        Else
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                    '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

            End Select
        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Return Nothing
            Throw ex
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        Finally
        End Try

        Return Nothing
    End Function

    '
    ' 機能　 ： マスタから情報を読み取る（２重読み込み用）
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Public Overridable Function FirstReadMasterData() As Integer
        Dim SQL As New StringBuilder(128)

        SQL.Append("SELECT ")
        SQL.Append(" FURI_DATA_K")
        SQL.Append(",MOTIKOMI_SEQ_S")
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        SQL.Append(",MOTIKOMI_DATE_S")  '持込期日
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END
        If mInfoComm.INFOParameter.FSYORI_KBN = "1" Then
            SQL.Append(" FROM MEIMAST")
            SQL.Append("   ,  SCHMAST")
        Else
            SQL.Append(" FROM S_MEIMAST")
            SQL.Append("    , S_SCHMAST")
        End If
        SQL.Append(" WHERE FSYORI_KBN_S   = FSYORI_KBN_K")
        SQL.Append("   AND TORIS_CODE_S   = TORIS_CODE_K")
        SQL.Append("   AND TORIF_CODE_S   = TORIF_CODE_K")
        SQL.Append("   AND FURI_DATE_S    = FURI_DATE_K")
        If mInfoComm.INFOParameter.FSYORI_KBN <> "1" Then
            SQL.Append("   AND MOTIKOMI_SEQ_S = CYCLE_NO_K")
        End If
        SQL.Append("   AND FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append("   AND ERROR_INF_S    = '020'")
        SQL.Append("   AND MOTIKOMI_SEQ_S = ")
        SQL.Append("(SELECT MOTIKOMI_SEQ_S FROM")
        If mInfoComm.INFOParameter.FSYORI_KBN = "1" Then
            SQL.Append(" SCHMAST")
        Else
            SQL.Append(" S_SCHMAST")
        End If
        SQL.Append(" WHERE FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append("   AND ERROR_INF_S    = '020'")
        SQL.Append("   AND ROWNUM <= 1")
        SQL.Append(")")
        SQL.Append(" ORDER BY RECORD_NO_K")
        If Not IOreader Is Nothing Then
            IOreader.Close()
        End If

        IOreader = New CASTCommon.MyOracleReader(OraDB)
        If IOreader.DataReader(SQL) = False Then
            DataInfo.Message = "スケジュールなし " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & CASTCommon.ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "yyyy年MM月dd日") & " 委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T
            Return 0
        End If

        ' 更新用に持ち込みＳＥＱを保存する
        InfoMeisaiMast.MOTIKOMI_SEQ = IOreader.GetInt("MOTIKOMI_SEQ_S")
        InfoMeisaiMast.FURIKETU_CODE = IOreader.GetInt("FURIKETU_CODE_K")

        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        MOTIKOMI_DATE = IOreader.GetString("MOTIKOMI_DATE_S")   '持込期日
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

        Return 1
    End Function

    '
    ' 機能　 ： マスタから情報を読み取る（返還用）
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Public Overridable Function FirstReadMasterDataHenkan() As Integer
        '*** 修正 mitsu 2008/09/30 処理高速化 ***
        'Dim SQL As New StringBuilder(128)
        Dim SQL As New StringBuilder(512)
        '****************************************
        SQL.Append("SELECT ")
        SQL.Append(" FURI_DATA_K")
        SQL.Append(",MOTIKOMI_SEQ_S")
        SQL.Append(",FURIKETU_CODE_K")
        SQL.Append(",TEISEI_SIT_K")
        SQL.Append(",TEISEI_KAMOKU_K")
        SQL.Append(",TEISEI_KOUZA_K")
        '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
        'バイナリデータも取得する
        SQL.Append(",BIN_DATA_K")
        '**********************************************
        SQL.Append(" FROM MEIMAST")
        SQL.Append("   ,  SCHMAST")
        SQL.Append(" WHERE FSYORI_KBN_S   = FSYORI_KBN_K")
        SQL.Append("   AND TORIS_CODE_S   = TORIS_CODE_K")
        SQL.Append("   AND TORIF_CODE_S   = TORIF_CODE_K")
        SQL.Append("   AND FURI_DATE_S    = FURI_DATE_K")
        SQL.Append("   AND FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        '*** 修正 nishida 2008/10/29 不具合発生 コメント化 ***
        'If mInfoComm.INFOToriMast.KESSAI_KYU_CODE_T = "1" OrElse mInfoComm.INFOToriMast.KESSAI_KYU_CODE_T = "3" Then
        '    ' 不能明細のみ抽出
        '    SQL.Append(" AND FURIKETU_CODE_K <> 0")
        'End If
        '**********************************************
        SQL.Append(" ORDER BY RECORD_NO_K")
        If Not IOreader Is Nothing Then
            IOreader.Close()
        End If

        IOreader = New CASTCommon.MyOracleReader(OraDB)
        If IOreader.DataReader(SQL) = False Then
            DataInfo.Message = "スケジュールなし " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & CASTCommon.ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mInfoComm.INFOParameter.TORIS_CODE & "-" & mInfoComm.INFOParameter.TORIF_CODE
            Return 0
        End If

        ' 更新用に持ち込みＳＥＱを保存する
        InfoMeisaiMast.MOTIKOMI_SEQ = IOreader.GetInt("MOTIKOMI_SEQ_S")
        InfoMeisaiMast.FURIKETU_CODE = IOreader.GetInt("FURIKETU_CODE_K")

        Return 1
    End Function

    '
    ' 機能　 ： ファイルから情報を取得する
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Public Overridable Function FirstRead() As Integer

        '2009/09/08.暫定対応　連携区分とりのぞく +++++++
        'If Not mInfoComm Is Nothing AndAlso _
        '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
        '    ' 強制の場合は，そのままリターンする
        '    If Not IOreader Is Nothing Then
        '        IOreader.Close()
        '        IOreader = Nothing
        '    End If
        '    Return 1
        'End If
        ''If Not mInfoComm Is Nothing Then
        ''    If Not IOreader Is Nothing Then
        ''        IOreader.Close()
        ''        IOreader = Nothing
        ''    End If
        ''    Return 1
        ''End If
        '+++++++++++++++++++++++++++++++++++++++++++++++

        If DataInfo.FileName Is Nothing Then
            DataInfo.Message = "ファイルが指定されていません"
            Return 0
        Else
            Return FirstRead(DataInfo.FileName)
        End If
    End Function
    '
    ' 機能　 ： マスタ・ファイルから情報を取得する
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Public Overridable Function FirstReadMast() As Integer


        If Not mInfoComm Is Nothing Then
            If Not IOreader Is Nothing Then
                IOreader.Close()
                IOreader = Nothing
            End If
            Return 1
        End If

        If DataInfo.FileName Is Nothing Then
            DataInfo.Message = "ファイルが指定されていません"
            Return 0
        Else
            Return FirstRead(DataInfo.FileName)
        End If
    End Function
    '
    ' 機能　 ： マスタから情報を読み取る（再振用）
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Public Overridable Function FirstReadMasterDataSaifuri() As Integer
        Dim SQL As New StringBuilder(512)
        SQL.Append("SELECT ")
        SQL.Append(" FURI_DATA_K")
        SQL.Append(",MOTIKOMI_SEQ_S")
        SQL.Append(",FURIKETU_CODE_K")
        SQL.Append(",TEISEI_SIT_K")
        SQL.Append(",TEISEI_KAMOKU_K")
        SQL.Append(",TEISEI_KOUZA_K")
        'バイナリデータも取得する
        SQL.Append(",BIN_DATA_K")
        SQL.Append(" FROM MEIMAST")
        SQL.Append("   ,  SCHMAST")
        SQL.Append(" WHERE FSYORI_KBN_S   = FSYORI_KBN_K")
        SQL.Append("   AND TORIS_CODE_S   = TORIS_CODE_K")
        SQL.Append("   AND TORIF_CODE_S   = TORIF_CODE_K")
        SQL.Append("   AND FURI_DATE_S    = FURI_DATE_K")
        SQL.Append("   AND FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        '    ' 不能明細のみ抽出
        '    SQL.Append(" AND FURIKETU_CODE_K <> 0")
        SQL.Append(" ORDER BY RECORD_NO_K")
        If Not IOreader Is Nothing Then
            IOreader.Close()
        End If

        IOreader = New CASTCommon.MyOracleReader(OraDB)
        If IOreader.DataReader(SQL) = False Then
            DataInfo.Message = "スケジュールなし " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & CASTCommon.ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mInfoComm.INFOParameter.TORIS_CODE & "-" & mInfoComm.INFOParameter.TORIF_CODE
            Return 0
        End If

        ' 更新用に持ち込みＳＥＱを保存する
        InfoMeisaiMast.MOTIKOMI_SEQ = IOreader.GetInt("MOTIKOMI_SEQ_S")
        InfoMeisaiMast.FURIKETU_CODE = IOreader.GetInt("FURIKETU_CODE_K")

        Return 1
    End Function

    '
    ' 機能　 ： ファイルから情報を取得する(返還データ作成)
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Public Overridable Function FirstRead_Henkan() As Integer

        If Not mInfoComm Is Nothing Then
            If Not IOreader Is Nothing Then
                IOreader.Close()
                IOreader = Nothing
            End If
            Return 1
        End If

        If DataInfo.FileName Is Nothing Then
            DataInfo.Message = "ファイルが指定されていません"
            Return 0
        Else
            Return FirstRead(DataInfo.FileName)
        End If
    End Function

    ' 機能　 ： ファイルから情報を取得する
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ： 共有モード
    '
    Public Overridable Function FirstRead(ByVal filename As String) As Integer
        Return FirstReadMode(filename, FileShare.ReadWrite)
    End Function

    ' 機能　 ： ファイルから情報を取得する
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ： 排他モード
    '
    Public Overridable Function FirstReadShare(ByVal filename As String) As Integer
        Return FirstReadMode(filename, FileShare.None)
    End Function

    ' 機能　 ： ファイルから情報を取得する
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Protected Friend Function FirstReadMode(ByVal filename As String, ByVal share As System.IO.FileShare) As Integer

        ' ファイル情報初期化
        With DataInfo
            .FileName = filename
            .FileLen = -1
            .Encoding = EncodingType.NONE
            .NewLine = NewLineType.None
        End With

        InfoMeisaiMast.FILE_SEQ = 0

        Call Close()

        Try
            ' ファイル情報取得
            Dim InfileInfo As New FileInfo(filename)
            If InfileInfo.Exists() = False Then
                Throw New FileNotFoundException
            End If
            DataInfo.FileLen = InfileInfo.Length
        Catch ex As UnauthorizedAccessException
            DataInfo.Message = String.Format("ファイルにアクセスできません")
            Return 0
        Catch ex As FileNotFoundException
            DataInfo.Message = String.Format("ファイルが見つかりません")
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Try
            ' ファイルオープン
            IOfileStream = New FileStream(filename, FileMode.Open, FileAccess.Read, share)
        Catch ex As FileNotFoundException
            DataInfo.Message = String.Format("ファイルが見つかりません")
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Dim TwoByte(1) As Byte

        ' データ読み取り
        Try
            '改行コードのチェック
            DataInfo.LenOfOneRec = DataInfo.RecoedLen

            '*** 修正 mitsu 2008/12/05 東海センター不能フォーマットはチェックしない ***
            'If IOfileStream.Seek(DataInfo.RecoedLen, SeekOrigin.Begin) = DataInfo.RecoedLen Then
            If Not (DataInfo.RecoedLen = 237 OrElse DataInfo.RecoedLen = 164) AndAlso IOfileStream.Seek(DataInfo.RecoedLen, SeekOrigin.Begin) = DataInfo.RecoedLen Then
                '**********************************************************************
                If IOfileStream.Read(TwoByte, 0, 2) = 0 Then
                    Throw New System.Exception("ファイル形式が正しくありません。")
                End If

                Dim NewLineLen As Integer = 0   '　改行コードの長さ
                '*** 修正 mitsu 2008/09/30 処理高速化 ***
                'Select Case String.Format("{0:X2}", TwoByte(0))
                '    Case "0D"
                '        Select Case String.Format("{0:X2}", TwoByte(1))
                '            Case "0A", "25"
                Select Case TwoByte(0)
                    Case &HD  '0D
                        Select Case TwoByte(1)
                            Case &HA, &H25 '0A, 25
                                '************************
                                DataInfo.NewLine = NewLineType.CrLf
                                NewLineLen = 2
                            Case Else
                                DataInfo.NewLine = NewLineType.Cr
                                NewLineLen = 1
                        End Select
                        '*** 修正 mitsu 2008/09/30 処理高速化 ***
                        'Case "0A"
                    Case &HA  '0A
                        '****************************************
                        DataInfo.NewLine = NewLineType.Lf
                        NewLineLen = 1
                        '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
                    Case &HF1 'F1
                        '改行コードの位置にコードF1がある場合はEBCDICデータフラグを立てる
                        DataInfo.NewLine = NewLineType.None
                        BinDataMode = True
                        '**********************************************
                    Case Else
                        DataInfo.NewLine = NewLineType.None
                End Select
                DataInfo.LenOfOneRec += NewLineLen
            End If

            ' １レコード用のバイト配列を用意
            '*** 修正 mitsu 2008/09/30 処理高速化 ***
            'ReadByte = CType(Array.CreateInstance(GetType(Byte), DataInfo.LenOfOneRec), Byte())
            ReadByte = New Byte(DataInfo.LenOfOneRec - 1) {}
            '****************************************

            BeforeRecKbn = ""

            ' 最低限のレコード以下のバイト数の場合は，エラー
            If DataInfo.FileLen < (DataInfo.RecoedLen * DataInfo.MinRecordCode.Length) Then
                '*** 修正 mitsu 2008/12/05 東海センター不能フォーマットは1レコード165バイト ***
                'Throw New System.Exception("ファイル形式が正しくありません。")
                If Not DataInfo.RecoedLen = 225 OrElse DataInfo.FileLen < 165 Then
                    Throw New System.Exception("ファイル形式が正しくありません。")
                End If
                '******************************************************************************
            End If

            IOfileStream.Seek(0, SeekOrigin.Begin)
            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                Throw New System.Exception("ファイル形式が正しくありません。")
            End If

            ' 先頭が，ヘッダレコードではない場合は，エラー
            If DataInfo.MinRecordCode.Length > 0 Then
                Select Case TwoByte(0)
                    Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                        ' SHIFT-JIS かどうかを判定
                        DataInfo.Encoding = EncodingType.SJIS
                        CurrEncd = EncdJ
                        '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
                        If BinDataMode Then
                            'レコードサイズ * 2の領域を確保する
                            ReadByte = New Byte(DataInfo.LenOfOneRec * 2 - 1) {}
                            'EBCDICデータ格納用配列を用意する
                            RecordDataBin = New Byte(DataInfo.LenOfOneRec - 1) {}
                        End If
                        '**********************************************

                    Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                        ' EBCDIC かどうかを判定
                        DataInfo.Encoding = EncodingType.EBCDIC
                        CurrEncd = EncdE
                    Case Else
                        If DataInfo.RecoedLen = 640 Then
                            ' センター直接持ち込みフォーマット対応
                            IOfileStream.Seek(88, SeekOrigin.Begin)
                            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                                Throw New System.Exception("ファイル形式が正しくありません。")
                            End If
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' SHIFT-JIS かどうかを判定
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' EBCDIC かどうかを判定
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("ファイル形式が正しくありません。")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 180 Then
                            ' しんきんフォーマット（作業用）対応
                            IOfileStream.Seek(59, SeekOrigin.Begin)
                            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                                Throw New System.Exception("ファイル形式が正しくありません。")
                            End If
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' SHIFT-JIS かどうかを判定
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' EBCDIC かどうかを判定
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("ファイル形式が正しくありません。")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 120 Then
                            ' しんきんフォーマット対応
                            IOfileStream.Seek(7, SeekOrigin.Begin)
                            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                                Throw New System.Exception("ファイル形式が正しくありません。")
                            End If
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' SHIFT-JIS かどうかを判定
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' EBCDIC かどうかを判定
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("ファイル形式が正しくありません。")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 237 Then
                            ' 不能ＭＴ(東海センター)
                            'ヘッダのない不能ファイルは"2"
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes("2")(0)
                                    ' SHIFT-JIS かどうかを判定
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case 2
                                    ' EBCDIC かどうかを判定
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("ファイル形式が正しくありません。")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 164 Then
                            ' 不能ＭＴ(東北・東京・大阪・中国センター)
                            If code_flg = True Then
                                ' SHIFT-JIS かどうかを判定
                                DataInfo.Encoding = EncodingType.SJIS
                                CurrEncd = EncdJ
                            Else
                                ' EBCDIC かどうかを判定
                                DataInfo.Encoding = EncodingType.EBCDIC
                                CurrEncd = EncdE
                                code_flg = True
                            End If
                        ElseIf DataInfo.RecoedLen = 580 Then
                            ' 配信ワークファイル
                        Else
                            Throw New System.Exception("ファイル形式が正しくありません。")
                            DataInfo.Encoding = EncodingType.NONE
                        End If
                End Select
            End If

            ' 最初に位置づけ
            IOfileStream.Seek(0, SeekOrigin.Begin)
        Catch ex As IO.DirectoryNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As IO.FileNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As IO.IOException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
        End Try

        Return 1
    End Function

    '
    ' 機能　 ： ファイルから情報を取得する
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ：
    '
    Public Overridable Function OpenWriteFile(ByVal filename As String) As Integer
        Try
            ' 書き込みファイルオープン
            If File.Exists(filename) = True Then
                File.Delete(filename)
            End If
            IOfileStream = New FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)
        Catch ex As ArgumentNullException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As ArgumentOutOfRangeException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As ArgumentException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As FileNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As DirectoryNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As PathTooLongException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As IOException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As System.Security.SecurityException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As UnauthorizedAccessException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    '
    ' 機能　 ： １レコード読み込み
    '
    ' 引数　 ： ARG1 - 文字列（参照渡し）
    '
    ' 戻り値 ： 読み込んだレコード長
    '
    ' 備考　 ：
    '
    Public Overridable Function GetFileData() As Integer
        Return MyClass.GetFileData(RecordData)
    End Function

    '
    ' 機能　 ： １レコード読み込み（返還用）
    '
    ' 引数　 ： ARG1 - 文字列（参照渡し）
    '
    ' 戻り値 ： 読み込んだレコード長
    '
    ' 備考　 ：
    '
    Public Overridable Function GetFileDataHenkan() As Integer
        Dim len As Integer = 0          ' レコード長

        Try
            '2009/09/08.暫定対応　連携区分とりのぞく +++++++
            'If Not mInfoComm Is Nothing AndAlso _
            '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
            ''If Not mInfoComm Is Nothing Then


            ''    If IOreader Is Nothing Then
            ''        If FirstReadMasterDataHenkan() = 0 Then
            ''            mRecordData = New String(" "c, RecordLen)
            ''            InfoMeisaiMast.FURIKETU_KEKKA = ""
            ''            Return 0
            ''        End If
            ''    Else
            ''        If IOreader.NextRead() = False Then
            ''            mRecordData = New String(" "c, RecordLen)
            ''            InfoMeisaiMast.FURIKETU_KEKKA = ""
            ''            Return 0
            ''        End If
            ''    End If
            ''    mRecordData = IOreader.GetString("FURI_DATA_K", False)
            ''    InfoMeisaiMast.FURIKETU_KEKKA = IOreader.GetInt("FURIKETU_CODE_K").ToString
            ''    InfoMeisaiMast.TEISEI_SIT = IOreader.GetString("TEISEI_SIT_K")
            ''    InfoMeisaiMast.TEISEI_KAMOKU = IOreader.GetString("TEISEI_KAMOKU_K")
            ''    InfoMeisaiMast.TEISEI_KOUZA = IOreader.GetString("TEISEI_KOUZA_K")
            ''    '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
            ''    'バイナリデータも取得
            ''    RecordDataBin = IOreader.GetBytes("BIN_DATA_K")
            ''    '**********************************************
            ''    Return mRecordData.Length
            ''End If
            '++++++++++++++++++++++++++++++++++++++++++++++++

            '***** 2009/09/20 kakiwaki *******
            '↑連携区分の条件式以外を戻す
            If Not mInfoComm Is Nothing Then


                If IOreader Is Nothing Then
                    If FirstReadMasterDataHenkan() = 0 Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                Else
                    If IOreader.NextRead() = False Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                End If
                mRecordData = IOreader.GetString("FURI_DATA_K", False)
                InfoMeisaiMast.FURIKETU_KEKKA = IOreader.GetInt("FURIKETU_CODE_K").ToString
                InfoMeisaiMast.TEISEI_SIT = IOreader.GetString("TEISEI_SIT_K")
                InfoMeisaiMast.TEISEI_KAMOKU = IOreader.GetString("TEISEI_KAMOKU_K")
                InfoMeisaiMast.TEISEI_KOUZA = IOreader.GetString("TEISEI_KOUZA_K")
                '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
                'バイナリデータも取得
                RecordDataBin = IOreader.GetBytes("BIN_DATA_K")
                '**********************************************
                Return mRecordData.Length
            End If
            '**************** 2009/09/20 *****

            ' １行を読み込み（改行データを含む）
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' 読込データ（BYTE)を文字列に変換
            mRecordData = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = mRecordData
        End Try

        Return len
    End Function
    '
    ' 機能　 ： １レコード読み込み（不能用）
    '
    ' 引数　 ： ARG1 - 文字列（参照渡し）
    '
    ' 戻り値 ： 読み込んだレコード長
    '
    ' 備考　 ：
    '
    Public Overridable Function GetFileDataFunou() As Integer
        Dim len As Integer = 0          ' レコード長

        Try
            ' １行を読み込み（改行データを含む）
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' 読込データ（BYTE)を文字列に変換
            mRecordData = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = mRecordData
        End Try

        Return len
    End Function

    '
    ' 機能　 ： １レコード読み込み（再振用）
    '
    ' 引数　 ： ARG1 - 文字列（参照渡し）
    '
    ' 戻り値 ： 読み込んだレコード長
    '
    ' 備考　 ：
    '
    Public Overridable Function GetFileDataSaifuri() As Integer
        Dim len As Integer = 0          ' レコード長

        Try
            If Not mInfoComm Is Nothing Then


                If IOreader Is Nothing Then
                    If FirstReadMasterDataSaifuri() = 0 Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                Else
                    If IOreader.NextRead() = False Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                End If
                mRecordData = IOreader.GetString("FURI_DATA_K", False)
                InfoMeisaiMast.FURIKETU_KEKKA = IOreader.GetInt("FURIKETU_CODE_K").ToString
                InfoMeisaiMast.TEISEI_SIT = IOreader.GetString("TEISEI_SIT_K")
                InfoMeisaiMast.TEISEI_KAMOKU = IOreader.GetString("TEISEI_KAMOKU_K")
                InfoMeisaiMast.TEISEI_KOUZA = IOreader.GetString("TEISEI_KOUZA_K")
                RecordDataBin = IOreader.GetBytes("BIN_DATA_K")
                '**********************************************
                Return mRecordData.Length
            End If

            ' １行を読み込み（改行データを含む）
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' 読込データ（BYTE)を文字列に変換
            mRecordData = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = mRecordData
        End Try

        Return len
    End Function
    '
    ' 機能　 ： １レコード読み込み（落し込み時に使用する）
    '
    ' 引数　 ： ARG1 - 文字列（参照渡し）
    '
    ' 戻り値 ： 読み込んだレコード長
    '
    ' 備考　 ：
    '
    Public Overridable Function GetFileData(ByRef data As String, ByVal seekPos As Integer) As Integer
        Dim len As Integer = 0          ' レコード長

        Try
            IOfileStream.Seek((seekPos - 1) * DataInfo.LenOfOneRec, SeekOrigin.Begin)

            ' １行を読み込み（改行データを含む）
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' 読込データ（BYTE)を文字列に変換
            data = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = data
        End Try

        Return len
    End Function

    '
    ' 機能　 ： １レコード読み込み（落し込み時に使用する）
    '
    ' 引数　 ： ARG1 - 文字列（参照渡し）
    '
    ' 戻り値 ： 読み込んだレコード長
    '
    ' 備考　 ：
    '
    Public Overridable Function GetFileData(ByRef data As String) As Integer
        Dim len As Integer = 0          ' レコード長

        Try
            '2009/09/08.暫定対応　連携区分とりのぞく +++++++
            'If Not mInfoComm Is Nothing AndAlso _
            '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
            ''If Not mInfoComm Is Nothing Then
            '    If IOreader Is Nothing Then
            '        ' 重複データのみを抽出
            '        If FirstReadMasterData() = 0 Then
            '            data = New String(" "c, RecordLen)
            '            Return 0
            '        End If
            '    Else
            '        If IOreader.NextRead() = False Then
            '            data = New String(" "c, RecordLen)
            '            Return 0
            '        End If
            '    End If
            '    data = IOreader.GetString("FURI_DATA_K", False)
            '    Return data.Length
            'End If
            '+++++++++++++++++++++++++++++++++++++++++++++++

            ' １行を読み込み（改行データを含む）
            '*** 2008/09/30 EBCDICデータ対応 ***
            'len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)
            If BinDataMode = False Then
                len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)
            Else
                'EBCDICデータの場合は倍レコード長を取得する
                len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec * 2)
                'レコード長を本来のサイズに戻す
                len = Convert.ToInt32(len / 2)
                'レコードの後半部はEBCDICデータなのでReadByteBinに格納
                Array.Copy(ReadByte, len, RecordDataBin, 0, len)
            End If
            '***********************************

            ' 読込データ（BYTE)を文字列に変換
            data = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = data
        End Try

        Return len
    End Function

    '
    ' 機能　 ： １バイト読み込み
    '
    ' 引数　 ： ARG1 - バイト（参照渡し）
    '
    ' 戻り値 ： 読み込んだ長さ
    '
    ' 備考　 ：
    '
    Public Overridable Function GetFileData(ByRef data As Byte) As Integer
        Dim len As Integer = 0          ' レコード長
        Dim bt(0) As Byte

        Try
            ' １バイトを読み込み
            len = IOfileStream.Read(bt, 0, 1)
            If len = 1 Then
                data = bt(0)
            End If
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Return len
    End Function

    '
    ' 機能　 ： ファイルに書き込む
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ： 改行コードは含まない
    '
    Public Overridable Function WriteData(ByVal data As String) As Integer
        Try
            IOfileStream.Write(EncdJ.GetBytes(data), 0, DataInfo.RecoedLen)

            '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- START
            With DataInfo
                If .WorkLen > .RecoedLen Then
                    '改行分スペースで埋める
                    Dim b As String = Space(.WorkLen - .RecoedLen)
                    IOfileStream.Write(EncdJ.GetBytes(b), 0, b.Length)
                End If
            End With
            '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- END

            Return 1
        Catch ex As Exception
            Try
                If EncdJ.GetBytes(data).Length < DataInfo.RecoedLen Then
                    IOfileStream.Write(EncdJ.GetBytes(data), 0, EncdJ.GetBytes(data).Length)
                    Return 1
                Else
                    DataInfo.Message = ex.Message
                End If
            Catch exB As Exception
                DataInfo.Message = ex.Message
            End Try
        End Try
        Return 0
    End Function

    '
    ' 機能　 ： ファイルに書き込む
    '
    ' 戻り値 ： 0 - 失敗，1 - 成功
    '
    ' 備考　 ： 改行コードは含まない
    '
    Public Overridable Function WriteCrLf() As Integer
        Try
            IOfileStream.Write(New Byte() {13, 10}, 0, 2)
        Catch ex As Exception
            DataInfo.Message = ex.Message
        End Try
    End Function

    '
    ' 機能　 ： ＥＯＦ判定
    '
    ' 引数　 ： CheckFLG - チェック方法フラグ
    '
    ' 戻り値 ： ＥＯＦ
    '
    ' 備考　 ：2009/09/23.Sakon　CheckFLGによる、チェック方法変更を追加
    '
    Public Function EOF(Optional ByVal CheckFLG As Integer = 0) As Boolean
        Try
            '2009/09/08.暫定対応　連携区分とりのぞく +++++++
            'If Not mInfoComm Is Nothing AndAlso _
            '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
            ''If Not mInfoComm Is Nothing Then

            '    If IOreader Is Nothing Then
            '        Return False
            '    End If
            '    Return IOreader.EOF
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++

            ''***** 2009/09/20 kakiwaki *******
            ''↑連携区分の条件式以外を戻す
            'If Not mInfoComm Is Nothing Then

            '    If IOreader Is Nothing Then
            '        Return False
            '    End If
            '    Return IOreader.EOF
            'End If
            ''**************** 2009/09/20 *****

            'If IOfileStream.Position >= DataInfo.FileLen Then
            '    Return True
            'End If
            'Return False

            '-------------------------------------
            '引数によってチェック処理を変更する
            '-------------------------------------
            If CheckFLG = 0 Then
                If IOfileStream.Position >= DataInfo.FileLen Then
                    Return True
                End If
                Return False
            Else
                '***** 2009/09/20 kakiwaki *******
                '↑連携区分の条件式以外を戻す
                If Not mInfoComm Is Nothing Then

                    If IOreader Is Nothing Then
                        Return False
                    End If
                    Return IOreader.EOF
                End If
                '**************** 2009/09/20 *****
            End If

        Catch ex As Exception
            Return True
        End Try
    End Function

    '
    ' 機能　 ： ヘッダレコード判定
    '
    ' 戻り値 ： True - ヘッダレコード
    '
    ' 備考　 ： 
    '
    Public Overridable Function IsHeaderRecord() As Boolean
        For i As Integer = 0 To HeaderKubun.Length - 1
            If RecordData.StartsWith(HeaderKubun(i)) = True Then
                Return True
            End If
        Next i
        Return False
    End Function

    '
    ' 機能　 ： データレコード判定
    '
    ' 戻り値 ： True - データレコード
    '
    ' 備考　 ： 
    '
    Public Overridable Function IsDataRecord() As Boolean
        For i As Integer = 0 To DataKubun.Length - 1
            If RecordData.StartsWith(DataKubun(i)) = True Then
                Return True
            End If
        Next i
        Return False
    End Function

    '
    ' 機能　 ： トレーラレコード判定
    '
    ' 戻り値 ： True - トレーラーレコード
    '
    ' 備考　 ： 依頼データデータの終わりかどうかを判定
    '
    Public Overridable Function IsTrailerRecord() As Boolean
        For i As Integer = 0 To TrailerKubun.Length - 1
            If RecordData.StartsWith(TrailerKubun(i)) = True Then
                Return True
            End If
        Next i
        Return False
    End Function

    '
    ' 機能　 ： エンドレコード判定
    '
    ' 戻り値 ： True - エンドレコード
    '
    ' 備考　 ： 依頼データデータの終わりかどうかを判定
    '
    Public Overridable Function IsEndRecord() As Boolean
        If RecordData.StartsWith(DataInfo.MinRecordCode(DataInfo.MinRecordCode.Length - 1)) = True Then
            Return True
        End If
        Return False
    End Function

    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function CuttingData(ByRef value As String, ByVal len As Integer) As String
        Try
            ' 切り取る文字列
            'Dim ret As String = value.Substring(0, len)
            Dim ret As String
            Dim bt() As Byte = EncdJ.GetBytes(value)
            ret = EncdJ.GetString(bt, 0, len)
            ' 切り取った後の残りの文字列
            'value = value.Substring(len)
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' 機能　 ： 半角文字のみかチェックする
    '
    ' 引数　 ： ARG1 - 文字列
    '
    ' 戻り値 ： True：半角のみ，False：全角あり
    '
    ' 備考　 ：
    '
    Public Function CheckHankaku(ByVal value As String) As Boolean
        Return (value.Length = EncdJ.GetByteCount(value))
    End Function

    '
    ' 機能　 ： レコードチェック
    '
    ' 戻り値 ： 不正文字の位置
    '
    ' 備考　 ：
    '
    Public Overridable Function CheckRegularString() As Long
        Dim nRet As Long

        ' 各フォーマット 共通チェック
        Select Case RecordData.Substring(0, 1)
            Case "1"        ' ヘッダ
                ' 全角チェック
                nRet = GetZenkakuPos(RecordData)
                If nRet = -1 Then
                    '規定文字チェック ヘッダーレコードは中点ＯＫ
                    nRet = CheckRegularStringVerB(RecordData)
                End If
                Return nRet
            Case "8", "9"   ' トレーラレコード，エンドレコード

                nRet = GetZenkakuPos(RecordData)
                If nRet = -1 Then
                    '規定文字チェック ヘッダーレコードは中点ＮＧ
                    nRet = CheckRegularStringVerA(RecordData)
                End If
                Return nRet
        End Select

        ' 全角チェック
        Return GetZenkakuPos(RecordData)
    End Function

    '
    ' 機能　 ： バイト配列から，文字列を取得する
    '
    ' 引数　 ： ARG1 - バイト配列
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Public Function GetString(ByVal bt() As Byte) As String
        Return CurrEncd.GetString(bt)
    End Function

    '
    ' 機能　 ： ファイルを閉じる
    '
    ' 備考　 ：
    '
    Public Sub Close()
        '2009/09/08.暫定対応　連携区分とりのぞく +++++++
        'If Not mInfoComm Is Nothing AndAlso _
        '        (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
        ''If Not mInfoComm Is Nothing Then

        'If Not IOreader Is Nothing Then
        '    IOreader.Close()
        'End If

        'Return
        'End If
        '+++++++++++++++++++++++++++++++++++++++++++
        If Not IOfileStream Is Nothing Then
            IOfileStream.Close()
        End If
        IOfileStream = Nothing
    End Sub

    '
    ' 機能　 ： ＥＢＣＤＩＣの判定
    '
    ' 備考　 ：
    '
    Public ReadOnly Property IsEBCDIC() As Boolean
        Get
            If CurrEncd.Equals(EncdE) = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    '
    ' 機能　 ： メッセージ
    '
    ' 備考　 ：
    '
    Public ReadOnly Property Message() As String
        Get
            If DataInfo.Message Is Nothing Then
                Return ""
            End If
            Return DataInfo.Message
        End Get
    End Property

#Region "位置取得"

    '
    ' 機能　 ： 全角文字の位置を取得する
    '
    ' 引数　 ： ARG1 - 文字列
    '
    ' 戻り値 ： 全角文字の位置
    '
    ' 備考　 ：
    '
    Public Function GetZenkakuPos(ByVal value As String) As Long
        If CheckHankaku(value) = True Then
            Return -1
        End If
        For i As Integer = 0 To value.Length - 1
            Dim s As String = value.Substring(i, 1)
            If EncdJ.GetByteCount(s) = 2 Then
                Return i
            End If
        Next i
        Return -1
    End Function
#End Region

#Region "チェック関数"
    '
    ' 機能　 ： 規定外文字列置換
    '
    ' 引数　 ： ARG1 - 文字列
    '
    ' 戻り値 ： 置換後文字列
    '
    ' 備考　 ：空白以外に置換する場合はこの関数内に追加する
    '
    Public Function ReplaceString(ByVal Value As String) As String
        Dim work As String = Value

        'work = work.Replace("ｧ", "ｱ")       ' ｧ
        'work = work.Replace("ｨ", "ｲ")       ' ｨ
        'work = work.Replace("ｩ", "ｳ")       ' ｩ
        'work = work.Replace("ｪ", "ｴ")       ' ｪ
        'work = work.Replace("ｫ", "ｵ")       ' ｫ
        'work = work.Replace("ｬ", "ﾔ")       ' ｬ
        'work = work.Replace("ｭ", "ﾕ")       ' ｭ
        'work = work.Replace("ｮ", "ﾖ")       ' ｮ
        'work = work.Replace("ｯ", "ﾂ")       ' ｯ
        'work = work.Replace("ｰ", "-")       ' ｰ
        'work = work.Replace("･", ".")       ' ･（半角中点）
        'work = work.Replace("　", "  ")     ' 全角空白
        ''2008/04/30 置換文字追加
        'work = work.Replace("`", " ")       ' `
        'work = work.Replace("､", " ")       ' ､
        'work = work.Replace("<", " ")       ' <
        'work = work.Replace(">", " ")       ' >
        ''*** 修正 mitsu 2008/11/26 置換文字追加 ***
        'work = work.Replace("*", " ")       ' *
        ''******************************************

        For Each de As DictionaryEntry In ReplaceStringPattern
            work = work.Replace(de.Key.ToString, de.Value.ToString)
        Next
        work = work.Replace(Convert.ToChar(0), " ") 'NULL

        work = work.ToUpper()

        Return work
    End Function

    '
    ' 機能　 ： 規定外文字列置換
    '
    ' 引数　 ： ARG1 - 文字列 ARG2 - 全角文字位置インデックス(Long)
    '
    ' 戻り値 ： 置換後文字列
    '
    ' 備考　 ：トレーラレコード・エンドレコードに対する処置
    '
    Public Function ReplaceString(ByVal Value As String, ByVal nRet As Long) As String

        '2010/10/07.Sakon　コメント化：全角変換もファイルから取得する ****************************
        'If nRet < 0 Then 'nRetの値が不正なとき
        '    nRet = GetZenkakuPos(Value)
        'End If

        ''全角文字が存在しなくなるまでループ
        'While nRet >= 0
        '    '2009/09/17.Sakon　全角括弧、全角中点の読替考慮追加 ++++++++++++++++++++++++++++
        '    Select Case Value.Substring(Convert.ToInt32(nRet), 1)
        '        Case "（"
        '            '全角"（" ⇒ 半角ｽﾍﾟｰｽ & "(" 
        '            Value = Value.Replace("（", " (")
        '            nRet = GetZenkakuPos(Value)
        '        Case "）"
        '            '全角"）" ⇒ ")" & 半角ｽﾍﾟｰｽ
        '            Value = Value.Replace("）", ") ")
        '            nRet = GetZenkakuPos(Value)
        '        Case "・"
        '            '全角"・" ⇒ "･" & 半角ｽﾍﾟｰｽ
        '            Value = Value.Replace("・", "･ ")
        '            nRet = GetZenkakuPos(Value)
        '        Case Else
        '            '全角文字を切り出し空白*2に置換
        '            Value = Value.Replace(Value.Substring(Convert.ToInt32(nRet), 1), "  ")
        '            nRet = GetZenkakuPos(Value)
        '    End Select
        '    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        'End While
        '*****************************************************************************************

        '規定外文字置換
        Value = ReplaceString(Value)                    '空白以外の規定外文字置換
        'Value = RegularRegexVerA.Replace(Value, " ")    '規定外文字置換A
        'Value = RegularRegexVerB.Replace(Value, " ")    '規定外文字置換B
        'Value = RegularRegexVerPlus.Replace(Value, " ") '「+」に対する置換

        Return Value
    End Function

    '
    ' 機能　 ： 規定外文字判定を行う(中点ＮＧ)
    '
    ' 引数　 ： ARG1 - 文字コード
    '
    ' 戻り値 ： 文字位置
    '
    ' 備考　 ：
    '
    Protected Friend Function CheckRegularStringVerA(ByVal Value As String) As Integer
        Return CheckRegularStringVerA(Value, 0)
    End Function

    '
    ' 機能　 ： 規定外文字判定を行う(中点ＮＧ)
    '
    ' 引数　 ： ARG1 - 文字コード
    '
    ' 戻り値 ： 文字位置
    '
    ' 備考　 ：
    '
    Protected Friend Function CheckRegularStringVerA(ByVal Value As String, ByVal startat As Integer) As Integer
        If RegularRegexVerA.IsMatch(Value, startat) = True Then
            ' 規定外文字がある場合は，文字位置を検索する
            Return RegularRegexVerA.Match(Value, startat).Index
        Else
            If RegularRegexVerPlus.IsMatch(Value) = True Then
                ' 規定外文字がある場合は，文字位置を検索する
                Return RegularRegexVerPlus.Match(Value).Index
            End If
        End If

        Return -1
    End Function

    '
    ' 機能　 ： 規定外文字判定を行う(中点ＮＧ)
    '
    ' 引数　 ： ARG1 - 文字コード
    '
    ' 戻り値 ： 文字位置
    '
    ' 備考　 ：
    '
    Protected Friend Function CheckRegularStringVerA(ByVal Value As String, ByVal beginning As Integer, ByVal length As Integer) As Integer
        If RegularRegexVerA.IsMatch(Value, beginning) = True Then
            ' 規定外文字がある場合は，文字位置を検索する
            Dim mat As Match = RegularRegexVerA.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        If RegularRegexVerPlus.IsMatch(Value, beginning) = True Then
            ' 規定外文字がある場合は，文字位置を検索する
            Dim mat As Match = RegularRegexVerPlus.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        Return -1
    End Function

    '
    ' 機能　 ： 規定外文字判定を行う(中点ＯＫ)
    '
    ' 引数　 ： ARG1 - 文字コード
    '
    ' 戻り値 ： 文字位置
    '
    ' 備考　 ：
    '
    Protected Friend Function CheckRegularStringVerB(ByVal Value As String) As Integer
        Return CheckRegularStringVerB(Value, 0)
    End Function

    '
    ' 機能　 ： 規定外文字判定を行う(中点ＯＫ)
    '
    ' 引数　 ： ARG1 - 文字コード
    '
    ' 戻り値 ： 文字位置
    '
    ' 備考　 ：
    '
    Protected Friend Function CheckRegularStringVerB(ByVal Value As String, ByVal startat As Integer) As Integer
        If RegularRegexVerB.IsMatch(Value, startat) = True Then
            ' 規定外文字がある場合は，文字位置を検索する
            Return RegularRegexVerB.Match(Value, startat).Index
        End If

        Return -1
    End Function

    '
    ' 機能　 ： 規定外文字判定を行う(中点ＯＫ)
    '
    ' 引数　 ： ARG1 - 文字コード
    '
    ' 戻り値 ： 文字位置
    '
    ' 備考　 ：
    '
    Protected Friend Function CheckRegularStringVerB(ByVal Value As String, ByVal beginning As Integer, ByVal length As Integer) As Integer
        If RegularRegexVerB.IsMatch(Value, beginning) = True Then
            ' 規定外文字がある場合は，文字位置を検索する
            Dim mat As Match = RegularRegexVerB.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        If RegularRegexVerPlus.IsMatch(Value, beginning) = True Then
            ' 規定外文字がある場合は，文字位置を検索する
            Dim mat As Match = RegularRegexVerPlus.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        Return -1
    End Function
#End Region

    '
    ' 機能　 ： レコードを読み込んでチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ： 
    '
    Public Overridable Function CheckDataFormat() As String
        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1レコード読込
            If GetFileData() > 0 Then
                ' 文字置換処理
                If CheckRegularString() > 0 Then
                    '***修正 maeda 2008/05/14*************************************************
                    '振替処理区分が、3(総給振)の場合規定外文字が存在してもエラーとしない
                    'If mInfoComm.INFOParameter.FSYORI_KBN <> "3" Then
                    '    ' 規定外文字あり
                    '    Return "ERR"
                    'End If
                    '' 規定外文字あり
                    'Return "ERR"
                    '*************************************************************************
                End If
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try
    End Function

    '
    ' 機能　 ： レコードを読み込んでチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ： 不能結果データから情報を読み込む
    '
    Public Overridable Function CheckKekkaFormat() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1レコード読込
            'If GetFileDataHenkan() > 0 Then
            If GetFileDataFunou() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function

    '
    ' 機能　 ： レコードを読み込んでチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ： 返還時，明細マスタから情報を読み込む
    '
    Public Overridable Function CheckHenkanFormat() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1レコード読込
            If GetFileDataHenkan() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function
    '
    ' 機能　 ： レコードを読み込んでチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ： 返還時，明細マスタから情報を読み込む
    '
    Public Overridable Function CheckSaifuriFormat() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1レコード読込
            If GetFileDataSaifuri() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function
    '
    ' 機能　 ： ヘッダレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Public Overridable Function CheckRecord1() As String
        Return ""
    End Function

    '
    ' 機能　 ： レコードを読み込んでチェック 後処理
    '
    ' 備考　 ：
    '
    Protected Friend Sub CheckDataFormatAfter()
        Try
            ' 合計依頼件数，合計依頼金額
            InfoMeisaiMast.TOTAL_KEN += InfoMeisaiMast.FURIKEN
            '2009/12/03 0円データを除いた依頼件数 ===========
            If InfoMeisaiMast.FURIKIN > 0 Then
                InfoMeisaiMast.TOTAL_KEN2 += InfoMeisaiMast.FURIKEN
            End If
            '================================================
            If InfoMeisaiMast.FURIKEN = 1 Then
                InfoMeisaiMast.TOTAL_KIN += InfoMeisaiMast.FURIKIN
            End If

            '***修正 maeda 2008/05/15**********************************************************************
            '総振でも振替結果コードを見るように再修正(依頼データの振替結果コードが7,8以外の場合) 
            '***修正 2008/05/01 mitsu *********************************************************
            '総振の場合は振替結果コードを見ない
            If Not (mInfoComm Is Nothing) Then
                If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
                    ' 2008.04.22 MODIFY 落し込みチェック仕様変更 (FJH殿指示) >>
                    'If InfoMeisaiMast.FURIKETU_CODE = 9 Then
                    If InfoMeisaiMast.FURIKETU_CODE <> 0 Then
                        ' 2008.04.22 MODIFY 落し込みチェック仕様変更 (FJH殿指示) <<
                        ' 異常件数，異常金額
                        InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
                        If InfoMeisaiMast.FURIKEN = 1 Then
                            InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
                        End If
                    End If
                Else
                    Select Case InfoMeisaiMast.FURIKETU_CODE
                        Case 7, 8 '依頼データに入っているデータ
                            InfoMeisaiMast.FURIKETU_CODE = 0
                        Case 2, 9 'システムで異常時に使用するコード
                            ' 異常件数，異常金額
                            InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
                            If InfoMeisaiMast.FURIKEN = 1 Then
                                InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
                            End If
                        Case Else
                            InfoMeisaiMast.FURIKETU_CODE = 0
                    End Select
                End If
            End If
            '**********************************************************************************
            ''***修正 2008/05/01 mitsu *********************************************************
            ''総振の場合は振替結果コードを見ない
            'If Not (mInfoComm Is Nothing) Then
            '    If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
            '        ' 2008.04.22 MODIFY 落し込みチェック仕様変更 (FJH殿指示) >>
            '        'If InfoMeisaiMast.FURIKETU_CODE = 9 Then
            '        If InfoMeisaiMast.FURIKETU_CODE <> 0 Then
            '            ' 2008.04.22 MODIFY 落し込みチェック仕様変更 (FJH殿指示) <<
            '            ' 異常件数，異常金額
            '            InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
            '            If InfoMeisaiMast.FURIKEN = 1 Then
            '                InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
            '            End If
            '        End If
            '    End If
            'End If
            ''**********************************************************************************
            '*********************************************************************************************

            ' レコード区分を保存
            BeforeRecKbn = RecordData.Substring(0, 1)
        Catch ex As Exception
        End Try
    End Sub

    Private Function GetJifuriSQL() As System.Text.StringBuilder
        Dim SQL As New StringBuilder(128)

        '取引先コード検索（マルチの場合も考慮する）、委託者コードのチェック
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(",TYUUDAN_FLG_S")
        SQL.Append(",TOUROKU_FLG_S")
        SQL.Append(",UKETUKE_FLG_S")
        SQL.Append(",BAITAI_CODE_T")
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        SQL.Append(",MOTIKOMI_DATE_S")  '持込期日
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END
        SQL.Append(" FROM TORIMAST,SCHMAST")

        Select Case mInfoComm.INFOToriMast.MULTI_KBN_T
            Case "0"    'シングル (取引先主コード、副コードで検索)
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                SQL.Append("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            Case Else   'マルチ（代表委託者コード、委託者コードで検索）
                ' 2016/11/08 タスク）綾部 CHG 【ME】(RSV2対応 標準バグ修正) -------------------- START
                ''2011/06/16 標準版修正 マルチの場合は、伝票・依頼書は考慮しない ------------------START
                'If mInfoComm.INFOToriMast.BAITAI_CODE_T <> "09" OrElse mInfoComm.INFOToriMast.BAITAI_CODE_T <> "04" Then
                '    SQL.Append(" WHERE ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                '    SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mInfoComm.INFOToriMast.ITAKU_KANRI_CODE_T))
                'Else
                '    '媒体コード伝票の場合はシングル扱いとする。 2011.5.27
                '    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                '    SQL.Append("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                'End If
                ''2011/06/16 標準版修正 マルチの場合は、伝票・依頼書は考慮しない ------------------END
                Select Case mInfoComm.INFOToriMast.BAITAI_CODE_T
                    Case "04", "09"
                        '-----------------------------------------------
                        ' 媒体コードが、依頼書・伝票はシングル扱い
                        '-----------------------------------------------
                        SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                        SQL.Append("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                    Case Else
                        '-----------------------------------------------
                        '媒体コードが、依頼書・伝票以外はマルチ扱い
                        '-----------------------------------------------
                        SQL.Append(" WHERE ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mInfoComm.INFOToriMast.ITAKU_KANRI_CODE_T))
                End Select
                ' 2016/11/08 タスク）綾部 CHG 【ME】(RSV2対応 標準バグ修正) -------------------- END
                '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（種別コード追加(給与・賞与で同一委託者コードが存在するため)）----- START
                If INI_JIFURI_SYUBETU_KEY = "1" Then
                    SQL.Append(" AND SYUBETU_T = " & SQ(InfoMeisaiMast.SYUBETU_CODE))
                End If
                '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（種別コード追加(給与・賞与で同一委託者コードが存在するため)）----- END
        End Select
        SQL.Append(" AND FURI_DATE_S = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

        Return SQL
    End Function

    Private Function GetFurikomiSQL() As System.Text.StringBuilder
        Dim SQL As New StringBuilder(128)

        '取引先コード検索（マルチの場合も考慮する）、委託者コードのチェック
        SQL.AppendLine("SELECT")
        SQL.AppendLine(" TORIS_CODE_T")
        SQL.AppendLine(",TORIF_CODE_T")
        SQL.AppendLine(",TYUUDAN_FLG_S")
        SQL.AppendLine(",TOUROKU_FLG_S")
        SQL.AppendLine(",UKETUKE_FLG_S")
        SQL.AppendLine(",BAITAI_CODE_T")
        SQL.AppendLine(",CYCLE_T")
        SQL.AppendLine(",ERROR_INF_S")
        SQL.AppendLine(",MOTIKOMI_SEQ_S")
        SQL.AppendLine(",TUKI1_T")
        SQL.AppendLine(",TUKI2_T")
        SQL.AppendLine(",TUKI3_T")
        SQL.AppendLine(",TUKI4_T")
        SQL.AppendLine(",TUKI5_T")
        SQL.AppendLine(",TUKI6_T")
        SQL.AppendLine(",TUKI7_T")
        SQL.AppendLine(",TUKI8_T")
        SQL.AppendLine(",TUKI9_T")
        SQL.AppendLine(",TUKI10_T")
        SQL.AppendLine(",TUKI11_T")
        SQL.AppendLine(",TUKI12_T")
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        SQL.AppendLine(",MOTIKOMI_DATE_S")  '持込期日
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END
        SQL.AppendLine(" FROM S_TORIMAST,S_SCHMAST")

        Select Case mInfoComm.INFOToriMast.MULTI_KBN_T
            Case "0"    'シングル (取引先主コード、副コードで検索)
                SQL.AppendLine(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                SQL.AppendLine("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            Case Else   'マルチ（代表委託者コード、委託者コード、種別で検索）
                SQL.AppendLine(" WHERE ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                SQL.AppendLine("   AND SYUBETU_T =" & SQ(InfoMeisaiMast.SYUBETU_CODE))
                SQL.AppendLine("   AND ITAKU_KANRI_CODE_T = " & SQ(mInfoComm.INFOToriMast.ITAKU_KANRI_CODE_T))
        End Select

        If mInfoComm.INFOToriMast.KIJITU_KANRI_T = "1" Then
            ' 期日管理あり
            SQL.AppendLine(" AND FURI_DATE_S = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            SQL.AppendLine(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.AppendLine(" ORDER BY MOTIKOMI_SEQ_S")
        Else
            '期日管理なし
            SQL.AppendLine(" AND FURI_DATE_S(+)  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            SQL.AppendLine(" AND TORIS_CODE_S(+) = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S(+) = TORIF_CODE_T")
        End If

        Return SQL
    End Function

    Public Function GetTorimastFromItakuCode(ByVal db As CASTCommon.MyOracle) As Boolean
        Return GetTorimastFromItakuCode(InfoMeisaiMast.SYUBETU_CODE, InfoMeisaiMast.ITAKU_CODE, db)
    End Function

    Public Function GetTorimastFromItakuCodeSSS(ByVal db As CASTCommon.MyOracle) As Boolean
        'Return GetTorimastFromItakuCode(InfoMeisaiMast.ITAKU_CODE.Substring(5, 4).PadLeft(10, "0"c), db)
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)

        '2017/02/27 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
        Dim SSS_ITAKUCODE_PATN As String = String.Empty
        SSS_ITAKUCODE_PATN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_ITAKUCODE_PATN")
        If SSS_ITAKUCODE_PATN = "err" OrElse SSS_ITAKUCODE_PATN = "" Then
            SSS_ITAKUCODE_PATN = "0"
        End If
        '2017/02/27 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE ")
            '2017/02/27 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            Select Case SSS_ITAKUCODE_PATN
                Case "0"
                    SQL.Append(" ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                Case "1"
                    SQL.Append(" SUBSTR(ITAKU_CODE_T, 1, 9) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(0, 9)))
                Case "2"
                    SQL.Append(" SUBSTR(ITAKU_CODE_T, 7, 4) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(5, 4)))
            End Select
            ''*** 修正 START 2009/03/05 NISHIDA SSS委託者コードの取得箇所が誤り
            ''SQL.Append(" SUBSTR(ITAKU_CODE_T, 7, 4) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(5, 4)))
            'SQL.Append(" SUBSTR(ITAKU_CODE_T, 1, 9) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(0, 9)))
            ''*** 修正 END
            '2017/02/27 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
            '2008/04/17 種別コード追加
            SQL.Append("   AND SYUBETU_T =" & SQ(InfoMeisaiMast.SYUBETU_CODE))
            SQL.Append(" AND FMT_KBN_T IN ('20', '21')")
            If OraReader.DataReader(SQL) = True Then
                Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
                Return True
            Else
                Call mInfoComm.GetTORIMAST("", "")
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            OraReader.Close()
        End Try
    End Function

    Public Function GetTorimastFromItakuCodeTAKO(ByVal db As CASTCommon.MyOracle) As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)
        Dim ToriCode As String

        SQL.Append("SELECT TORIS_CODE_V || TORIF_CODE_V TORI_CODE")
        SQL.Append(" FROM TAKOUMAST, TAKOSCHMAST")
        SQL.Append(" WHERE TKIN_NO_V = " & CASTCommon.SQ(InfoMeisaiMast.ITAKU_KIN))
        SQL.Append("   AND ITAKU_CODE_V = " & CASTCommon.SQ(InfoMeisaiMast.ITAKU_CODE))
        SQL.Append("   AND TORIS_CODE_V = TORIS_CODE_U")
        SQL.Append("   AND TORIF_CODE_V = TORIF_CODE_U")
        SQL.Append("   AND TKIN_NO_V    = TKIN_NO_U")
        SQL.Append("   AND FURI_DATE_U  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
        If OraReader.DataReader(SQL) = False Then
            Return False
        End If
        ToriCode = OraReader.GetString("TORI_CODE")

        Call GetTorimastFromToriCode(ToriCode, db)
        If ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            Return False
        End If

        Return True
    End Function

    '2011/03/01 企業持込不能更新用の関数追加
    Public Function GetTorimastFromItakuCodeKigyo(ByVal db As CASTCommon.MyOracle) As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST, SCHMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
            SQL.Append(" AND FURI_DATE_S = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" ORDER BY FSYORI_KBN_T, TORIS_CODE_T, TORIF_CODE_T")

            If OraReader.DataReader(SQL) = True Then
                Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
                Return True
            Else
                Call mInfoComm.GetTORIMAST("", "")
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            OraReader.Close()
        End Try
    End Function

    Public Sub GetTorimastFromToriCode(ByVal toriCode As String, ByVal db As CASTCommon.MyOracle)
        Call mInfoComm.GetTORIMAST(toriCode.Substring(0, 10), toriCode.Substring(10))
    End Sub

    Public Function GetTorimastFromItakuCode(ByVal SyubetuCode As String, ByVal itakusyaCode As String, ByVal db As CASTCommon.MyOracle) As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" ITAKU_CODE_T = " & SQ(itakusyaCode))
            ' 2008/04/24 DELETE 種別コードをチェックするのは振込のみに 再修正 >>
            ''2008/04/17 種別コード追加
            'SQL.Append("   AND SYUBETU_T =" & SQ(SyubetuCode))
            ' 2008/04/24 DELETE 種別コードをチェックするのは振込のみに 再修正 <<
            SQL.Append(" ORDER BY FSYORI_KBN_T, TORIS_CODE_T, TORIF_CODE_T")
            If OraReader.DataReader(SQL) = False Then
                OraReader.Close()
                ' 2008/04/24 MODIFY 種別コードをチェックするのは振込のみに 再修正 >>
                'OraReader.DataReader(SQL.Replace("TORIMAST", "S_TORIMAST"))
                SQL = New StringBuilder
                SQL.Append("SELECT ")
                SQL.Append(" TORIS_CODE_T")
                SQL.Append(",TORIF_CODE_T")
                SQL.Append(" FROM S_TORIMAST")
                SQL.Append(" WHERE ")
                SQL.Append(" ITAKU_CODE_T = " & SQ(itakusyaCode))
                SQL.Append("   AND SYUBETU_T =" & SQ(SyubetuCode))
                SQL.Append(" ORDER BY FSYORI_KBN_T, TORIS_CODE_T, TORIF_CODE_T")
                ' 2008/04/24 MODIFY 種別コードをチェックするのは振込のみに 再修正 <<
            End If

            If OraReader.DataReader(SQL) = True Then
                Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
                Return True
            Else
                Call mInfoComm.GetTORIMAST("", "")
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            OraReader.Close()
        End Try
    End Function

    '
    ' 機能　 ： ヘッダレコードを読み込んでチェック
    '
    ' 引数   ： ARG1 - モード(0 - 通常，1 - 総給振落し込み時に仕様）
    '
    ' 備考　 ： 各フォーマットチェックから，呼ばれる共通関数
    '　　　     ヘッダ情報から，取引先マスタを参照する
    '
    Protected Overridable Function CheckHeaderRecord(Optional ByVal mode As Integer = 0) As Boolean

        ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-02(RSV2対応) -------------------- START
        Dim INI_RSV2_HEAD_BANKCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BANKCODE")
        '*** Str Add 2016/01/05 sys)mori for バグ修正対応 ***
        'Dim INI_RSV2_HEAD_S_SITENCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_SITENCOD")
        Dim INI_RSV2_HEAD_S_SITENCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_SITENCODE")
        '*** End Add 2016/01/05 sys)mori for バグ修正対応 ***
        Dim INI_RSV2_HEAD_S_KAMOKUCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAMOKUCODE")
        Dim INI_RSV2_HEAD_S_KOUZABANGO As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KOUZABANGO")
        ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-02(RSV2対応) -------------------- END
        '2018/09/24 saitou 広島信金(RSV2対応) ADD 【現行】（総振委託者コードチェック可否） ---------- START
        Dim INI_RSV2_HEAD_S_ITAKUCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_ITAKUCODE")
        '2018/09/24 saitou 広島信金(RSV2対応) ADD --------------------------------------------------- END

        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        ' ＤＢ接続が存在しない場合，正常値を返す
        If OraDB Is Nothing Then
            Return True
        End If

        'マルチ区分が0の場合、複数ヘッダならば異常終了とする
        HeaderCnt += 1

        If ToriData.INFOToriMast.MULTI_KBN_T = "0" AndAlso HeaderCnt > 2 Then '※シングルでも2回通る
            WriteBLog("マルチ区分異常", "エラー発生")
            DataInfo.Message = " 取引先マスタ登録異常：マルチ区分"
            Return False
        End If

        TAKOU_ON = False

        If CASTCommon.IsDecimal(InfoMeisaiMast.FURIKAE_DATE_MOTO) = False Then
            WriteBLog("ヘッダ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "異常", "異常", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
            DataInfo.Message = "ヘッダ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "異常 委託者コード：" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO
            Return False
        Else
            If InfoMeisaiMast.FURIKAE_DATE = "00010101" Then
                WriteBLog("ヘッダ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "異常", "異常", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
                DataInfo.Message = "ヘッダ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "異常 委託者コード：" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO
                Return False
            End If
        End If

        If mInfoComm.INFOParameter.FSYORI_KBN = "1" Then
            ' 口振
            SQL = GetJifuriSQL()
        Else
            ' 振込
            SQL = GetFurikomiSQL()
        End If

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then
            DataInfo.Message = "取引先/ｽｹｼﾞｭｰﾙ検索失敗"
            DataInfo.Message &= " 委託者" & mInfoComm.INFOToriMast.ITAKU_CODE_T

            Select Case mInfoComm.INFOToriMast.MULTI_KBN_T
                Case "0"    'シングル (取引先主コード、副コードで検索)
                    WriteBLog("ファイルヘッダ取引先検索", "失敗",
                        "取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "－" & mInfoComm.INFOToriMast.TORIS_CODE_T)
                Case Else   'マルチ（代表委託者コード、委託者コードで検索）
                    WriteBLog("取引先/ｽｹｼﾞｭｰﾙ検索", "失敗",
                        " 委託者ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_CODE &
                        " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
                    DataInfo.Message = "取引先/ｽｹｼﾞｭｰﾙ検索失敗"
                    DataInfo.Message &= " 委託者" & InfoMeisaiMast.ITAKU_CODE
            End Select
            ' 取引先マスタ情報をクリアする
            Call mInfoComm.ClearTORIMAST()
            Return False
        Else
            ' 取引先マスタを取得
            Call mInfoComm.SelectTORIMAST(mInfoComm.INFOParameter.FSYORI_KBN, OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
        End If

        ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-02(RSV2対応) -------------------- START
        '' 取引銀行番号チェック
        'If InfoMeisaiMast.ITAKU_KIN <> JIKINKO Then
        '    WriteBLog("ﾍｯﾀﾞ自金庫ﾁｪｯｸ", "不一致", "金融機関ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_KIN)
        '    DataInfo.Message = "ﾍｯﾀﾞ自金庫ﾁｪｯｸ不一致 金融機関ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_KIN
        '    Return False
        'End If

        ''総振の場合はヘッダレコード委託者情報チェック
        'If mode = 1 Then
        '    '口座番号ALL0以外を対象とする
        '    If mInfoComm.INFOToriMast.KOUZA_T <> "0000000" Then
        '        ' 支店番号チェック
        '        If InfoMeisaiMast.ITAKU_SIT <> mInfoComm.INFOToriMast.TSIT_NO_T Then
        '            WriteBLog("ﾍｯﾀﾞ支店番号ﾁｪｯｸ", "不一致", "支店ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_SIT)
        '            DataInfo.Message = "ﾍｯﾀﾞ支店番号不一致 支店ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_SIT
        '            Return False
        '        End If

        '        ' 預金種目チェック(普通・当座のみ対象とする)
        '        If (mInfoComm.INFOToriMast.KAMOKU_T = "01" OrElse mInfoComm.INFOToriMast.KAMOKU_T = "02") AndAlso _
        '            InfoMeisaiMast.ITAKU_KAMOKU <> ConvertKamoku2TO1(mInfoComm.INFOToriMast.KAMOKU_T) Then
        '            WriteBLog("ﾍｯﾀﾞ預金種目ﾁｪｯｸ", "不一致", "預金種目：" & InfoMeisaiMast.ITAKU_KAMOKU)
        '            DataInfo.Message = "ﾍｯﾀﾞ預金種目不一致 預金種目：" & InfoMeisaiMast.ITAKU_KAMOKU
        '            Return False
        '        End If

        '        ' 口座番号チェック
        '        If InfoMeisaiMast.ITAKU_KOUZA <> mInfoComm.INFOToriMast.KOUZA_T Then
        '            WriteBLog("ﾍｯﾀﾞ口座番号ﾁｪｯｸ", "不一致", "口座番号：" & InfoMeisaiMast.ITAKU_KOUZA)
        '            DataInfo.Message = "ﾍｯﾀﾞ口座番号不一致 口座番号：" & InfoMeisaiMast.ITAKU_KOUZA
        '            Return False
        '        End If
        '    End If

        '    Return True
        'End If
        ' 取引銀行番号チェック
        If INI_RSV2_HEAD_BANKCODE = "1" Then
            If InfoMeisaiMast.ITAKU_KIN <> JIKINKO Then
                WriteBLog("ﾍｯﾀﾞ自金庫ﾁｪｯｸ", "不一致", "金融機関ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_KIN)
                DataInfo.Message = "ﾍｯﾀﾞ自金庫ﾁｪｯｸ不一致 金融機関ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_KIN
                Return False
            End If
        End If

        '総振の場合はヘッダレコード委託者情報チェック
        If mode = 1 Then
            '口座番号ALL0以外を対象とする
            If mInfoComm.INFOToriMast.KOUZA_T <> "0000000" Then
                ' 支店番号チェック
                If INI_RSV2_HEAD_S_SITENCODE = "1" Then
                    If InfoMeisaiMast.ITAKU_SIT <> mInfoComm.INFOToriMast.TSIT_NO_T Then
                        WriteBLog("ﾍｯﾀﾞ支店番号ﾁｪｯｸ", "不一致", "支店ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_SIT)
                        DataInfo.Message = "ﾍｯﾀﾞ支店番号不一致 支店ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_SIT
                        Return False
                    End If
                End If

                ' 預金種目チェック(普通・当座のみ対象とする)
                If INI_RSV2_HEAD_S_KAMOKUCODE = "1" Then
                    If (mInfoComm.INFOToriMast.KAMOKU_T = "01" OrElse mInfoComm.INFOToriMast.KAMOKU_T = "02") AndAlso
                        InfoMeisaiMast.ITAKU_KAMOKU <> ConvertKamoku2TO1(mInfoComm.INFOToriMast.KAMOKU_T) Then
                        WriteBLog("ﾍｯﾀﾞ預金種目ﾁｪｯｸ", "不一致", "預金種目：" & InfoMeisaiMast.ITAKU_KAMOKU)
                        DataInfo.Message = "ﾍｯﾀﾞ預金種目不一致 預金種目：" & InfoMeisaiMast.ITAKU_KAMOKU
                        Return False
                    End If
                End If

                ' 口座番号チェック
                If INI_RSV2_HEAD_S_KOUZABANGO = "1" Then
                    If InfoMeisaiMast.ITAKU_KOUZA <> mInfoComm.INFOToriMast.KOUZA_T Then
                        WriteBLog("ﾍｯﾀﾞ口座番号ﾁｪｯｸ", "不一致", "口座番号：" & InfoMeisaiMast.ITAKU_KOUZA)
                        DataInfo.Message = "ﾍｯﾀﾞ口座番号不一致 口座番号：" & InfoMeisaiMast.ITAKU_KOUZA
                        Return False
                    End If
                End If
            End If

            Return True
        End If
        ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-02(RSV2対応) -------------------- END

        ' 明細マスタ項目設定
        InfoMeisaiMast.ITAKU_KIN = mInfoComm.INFOToriMast.TKIN_NO_T     ' 取扱金融機関コード
        InfoMeisaiMast.ITAKU_SIT = mInfoComm.INFOToriMast.TSIT_NO_T     ' 取扱支店コード
        InfoMeisaiMast.ITAKU_KAMOKU = mInfoComm.INFOToriMast.KAMOKU_T   ' 科目
        InfoMeisaiMast.ITAKU_KOUZA = mInfoComm.INFOToriMast.KOUZA_T     ' 口座番号

        BLOG.ToriCode = mInfoComm.INFOToriMast.TORIS_CODE_T & mInfoComm.INFOToriMast.TORIF_CODE_T

        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        MOTIKOMI_DATE = OraReader.GetString("MOTIKOMI_DATE_S")    '持込期日
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

        '媒体コードのチェック
        Select Case mInfoComm.INFOParameter.BAITAI_CODE
            Case "01"
                If mInfoComm.INFOToriMast.BAITAI_CODE_T <> "01" And mInfoComm.INFOToriMast.BAITAI_CODE_T <> "02" And mInfoComm.INFOToriMast.BAITAI_CODE_T <> "06" Then
                    WriteBLog("媒体コード異常", "エラー発生")
                    DataInfo.Message = "取引先マスタ登録異常：媒体コード"
                    Return False
                End If
            Case "00"
                If mInfoComm.INFOToriMast.BAITAI_CODE_T <> "00" Then
                    WriteBLog("媒体コード異常", "エラー発生")
                    DataInfo.Message = "取引先マスタ登録異常：媒体コード"
                    Return False
                End If
        End Select

        Dim CheckFlag As Boolean = False
        ' 複数回持込ありの場合，スケジュールを作成する

        If mInfoComm.INFOParameter.FSYORI_KBN = "3" Then

            If mInfoComm.INFOToriMast.KIJITU_KANRI_T = "1" Then
                ' 期日管理あり
                If OraReader.GetItem("TYUUDAN_FLG_S") = "0" And
                    (OraReader.GetItem("UKETUKE_FLG_S") = "1" Or OraReader.GetItem("ERROR_INF_S") = "020") Then
                    ' すでに，落とし込み済みの場合
                    If OraReader.GetItem("CYCLE_T") = "1" Then
                        Dim Tuki As String
                        Tuki = "TUKI" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE).ToString("M") & "_T"
                        If OraReader.GetItem(Tuki) = "0" Then
                            WriteBLog("ヘッダ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & " 処理対象月以外 ", "異常", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
                            DataInfo.Message = "ヘッダ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & " 処理対象月以外 委託者コード：" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO
                            Return False
                        End If

                        ' 複数回持込あり
                        If InsertSCHMAST() = False Then
                            Return False
                        End If
                    Else
                        ' 複数回持込なし
                        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetInt("MOTIKOMI_SEQ_S")
                        CheckFlag = True
                    End If
                Else
                    ' 持込SEQの設定
                    Dim SEQReader As New CASTCommon.MyOracleReader(OraDB)
                    Dim nGetSEQ As Integer
                    ' 持込SEQの最大値を取得する
                    Dim SQL_SEQ As New StringBuilder
                    SQL_SEQ.AppendLine("SELECT")
                    SQL_SEQ.AppendLine(" NVL(MAX(MOTIKOMI_SEQ_S), 0)  SEQ")
                    SQL_SEQ.AppendLine(" FROM S_SCHMAST")
                    SQL_SEQ.AppendLine(" WHERE TORIS_CODE_S  =" & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                    SQL_SEQ.AppendLine(" AND TORIF_CODE_S  =" & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                    SQL_SEQ.AppendLine(" AND FURI_DATE_S  =" & SQ(InfoMeisaiMast.FURIKAE_DATE))

                    Dim nRet As Long = 0
                    If InfoMeisaiMast.FILE_SEQ = 0 AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
                        ' 持込SEQカウントSQL実行
                        If SEQReader.DataReader(SQL_SEQ) = True Then
                            nGetSEQ = SEQReader.GetValueInt(0)
                        Else
                            WriteBLog("スケジュール作成", "失敗", OraDB.Message & ":" & SQL_SEQ.ToString)
                            Return False
                        End If
                        SEQReader.Close()
                    Else
                        nGetSEQ = InfoMeisaiMast.MOTIKOMI_SEQ
                    End If

                    InfoMeisaiMast.MOTIKOMI_SEQ = nGetSEQ

                    CheckFlag = True
                End If
            Else
                ' 期日管理なしの場合
                If OraReader.GetItem("CYCLE_T") = "1" Then
                    ' 複数回持込あり
                    If InsertSCHMAST() = False Then
                        'エラー内容が空の場合のみ
                        If DataInfo.Message = "" Then
                            WriteBLog("同一ﾌｧｲﾙ内に同じ委託者，" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "のデータが存在", "異常", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
                            DataInfo.Message = "同一ﾌｧｲﾙ内に同じ委託者" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "のﾃﾞｰﾀが存在 委託者ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & InfoMeisaiMast.FURIKAE_DATE_MOTO
                        End If

                        Return False
                    End If
                Else
                    ' 複数回持込なし
                    If OraReader.GetItem("TYUUDAN_FLG_S") = "" Then
                        ' スケジュールが無い場合に作成する
                        If InsertSCHMAST() = False Then
                            Return False
                        End If
                    Else
                        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetInt("MOTIKOMI_SEQ_S")
                        CheckFlag = True
                    End If
                End If
            End If

        Else
            CheckFlag = True
        End If

        If CheckFlag = True Then
            If OraReader.IsNull("TYUUDAN_FLG_S") = True Then
                ' 期日管理する場合で，スケジュールマスタが無い場合，エラー
                WriteBLog("ファイルヘッダスケジュール検索", "失敗", "委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
                DataInfo.Message = "ｽｹｼﾞｭｰﾙ検索失敗 委託者ｺｰﾄﾞ:" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
                Return False
            End If

            If OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
                WriteBLog("スケジュール:中断フラグ設定済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"), "中断")
                DataInfo.Message = "中断ﾌﾗｸﾞ設定済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
                Return False
            End If

            '2017/12/04 タスク）西野 CHG 標準版修正（照合機能追加）------------------------------------ START
            If mInfoComm.INFOToriMast.SYOUGOU_KBN_T = "1" Then
                ' 照合ありの場合、受付済みフラグにて判定
                If OraReader.GetItem("UKETUKE_FLG_S") <> "0" Then
                    WriteBLog("スケジュール:落し込み処理済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T, "中断")
                    DataInfo.Message = "落し込み処理済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日" & " 取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
                    Return False
                End If
            Else
                ' 伝送，ＦＤ，ＣＭＴの場合（照合なし） 登録済フラグにて判定
                If OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
                    WriteBLog("スケジュール:落し込み処理済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T, "中断")
                    DataInfo.Message = "落し込み処理済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日" & " 取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
                    Return False
                End If
            End If
            '' 伝送，ＦＤ，ＣＭＴの場合（照合なし） 登録済フラグにて判定
            'If OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
            '    WriteBLog("スケジュール:落し込み処理済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T, "中断")
            '    DataInfo.Message = "落し込み処理済 " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日" & " 取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
            '    Return False
            'End If
            '2017/12/04 タスク）西野 CHG 標準版修正（照合機能追加）------------------------------------ END

        End If

        ' オラクルReaderクローズ
        OraReader.Close()

        'フォーマット区分取得
        If mInfoComm.INFOToriMast.FMT_KBN_T = "20" Or mInfoComm.INFOToriMast.FMT_KBN_T = "21" Then  '集金代行サービスの時
            '2017/01/16 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            If mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "0" Then
                If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim = "" Then
                    'カナ摘要が設定されていないとエラー
                    WriteBLog("ｶﾅ摘要", "未設定", "委託者ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_CODE)
                    DataInfo.Message = "ｶﾅ摘要未設定　取引先ｺｰﾄﾞ:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
                    Return False
                End If

            ElseIf mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
                '支店摘要はOK

            Else
                '上記以外はNG
                WriteBLog("摘要区分", "設定ミス", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE)
                DataInfo.Message = "摘要区分を「ｶﾅ」「支店」に設定してください　取引先ｺｰﾄﾞ:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
                Return False
            End If

            'If mInfoComm.INFOToriMast.TEKIYOU_KBN_T <> "0" Then 'ｶﾅ摘要以外はエラーとする
            '    WriteBLog("摘要区分", "設定ミス", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE)
            '    DataInfo.Message = "摘要区分を「ｶﾅ」に設定してください　取引先ｺｰﾄﾞ:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
            '    Return False
            'End If
            ''ｶﾅ摘要が設定されていないとエラー
            'If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim = "" Then
            '    WriteBLog("ｶﾅ摘要", "未設定", "委託者ｺｰﾄﾞ:" & InfoMeisaiMast.ITAKU_CODE)
            '    DataInfo.Message = "ｶﾅ摘要未設定　取引先ｺｰﾄﾞ:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
            '    Return False
            'Else
            '    '集金代行サービス用の摘要に編集
            '    '' 明細マスタ項目設定 摘要
            '    If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Length > 10 Then
            '        InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Substring(0, 10)
            '    Else
            '        InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim
            '    End If
            'End If
            '2017/01/16 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
        End If
        WriteBLog("ヘッダ取引先検索", "成功", "取引先ｺｰﾄﾞ:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
        Select Case mInfoComm.INFOParameter.FMT_KBN
            Case "02"     '国税フォーマット
            Case Else     '国税フォーマット以外
                '2018/09/24 saitou 広島信金(RSV2対応) UPD 【現行】（総振委託者コードチェック可否） ---------- START
                If mInfoComm.INFOToriMast.FSYORI_KBN_T = "1" Then
                    If InfoMeisaiMast.ITAKU_CODE <> mInfoComm.INFOToriMast.ITAKU_CODE_T Then
                        ' 委託者コード不一致
                        WriteBLog("ヘッダ委託者コード", "不一致", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ委託者ｺｰﾄﾞ不一致:" & InfoMeisaiMast.ITAKU_CODE
                        Return False
                    End If
                Else
                    If INI_RSV2_HEAD_S_ITAKUCODE = "1" Then
                        If InfoMeisaiMast.ITAKU_CODE <> mInfoComm.INFOToriMast.ITAKU_CODE_T Then
                            ' 委託者コード不一致
                            WriteBLog("ヘッダ委託者コード", "不一致", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE)
                            DataInfo.Message = "ﾍｯﾀﾞ委託者ｺｰﾄﾞ不一致:" & InfoMeisaiMast.ITAKU_CODE
                            Return False
                        End If
                    End If
                End If
                'If InfoMeisaiMast.ITAKU_CODE <> mInfoComm.INFOToriMast.ITAKU_CODE_T Then
                '    ' 委託者コード不一致
                '    WriteBLog("ヘッダ委託者コード", "不一致", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE)
                '    DataInfo.Message = "ﾍｯﾀﾞ委託者ｺｰﾄﾞ不一致:" & InfoMeisaiMast.ITAKU_CODE
                '    Return False
                'End If
                '2018/09/24 saitou 広島信金(RSV2対応) UPD --------------------------------------------------- END
        End Select

        '明細の振替日とパラメータの振替日のチェック
        If InfoMeisaiMast.FURIKAE_DATE <> mInfoComm.INFOParameter.FURI_DATE Then
            '振替日不一致
            '振替日が違っていると異常終了する場合
            WriteBLog("ファイルヘッダ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "不一致", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
            DataInfo.Message = "ﾍｯﾀﾞ" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "不一致:" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyy年MM月dd日")
            Return False
        End If

        ' 入出金区分 種別コードチェック
        Select Case mInfoComm.INFOToriMast.NS_KBN_T
            Case "1"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "11", "12", "21", "41", "43", "44", "45", "71", "72"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ入出金区分、種別", "不一致", "種別：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ入出金区分、種別不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "9"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "91"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ入出金区分、種別", "不一致", "種別：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ入出金区分、種別不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
        End Select

        '  種別コードチェック
        Select Case mInfoComm.INFOToriMast.SYUBETU_T
            Case "91"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "91"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "21"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "21", "41", "43", "44", "45"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "12"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "12"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "11"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "11"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "71"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "71"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "72"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "72"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("ファイルヘッダ種別コード", "不一致", "種別コード：" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ﾍｯﾀﾞ種別ｺｰﾄﾞ不一致:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
        End Select

        Return True
    End Function

    '
    ' 機能　 ： データレコードを読み込んでチェック
    '
    ' 備考　 ：2010/10/07.Sakon　インプットエラー分に結果コードをセットするかをINIファイルより判定する
    '
    Protected Overridable Function CheckDataRecord() As Boolean
        Dim InError As INPUTERROR = Nothing

        InError.DATA = InfoMeisaiMast
        InErrorArray = New ArrayList

        If mInfoComm Is Nothing Then
            Return True
        End If

        '支店・口座読替対応(変更前情報の保持)
        InfoMeisaiMast.OLD_KIN_NO = InfoMeisaiMast.KEIYAKU_KIN
        InfoMeisaiMast.OLD_SIT_NO = InfoMeisaiMast.KEIYAKU_SIT
        InfoMeisaiMast.OLD_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA

        '2010.02.09 振替結果を見るのは種別９１のみとする
        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" AndAlso
            InfoMeisaiMast.FURIKETU_CODE <> 0 AndAlso mInfoComm.INFOToriMast.NS_KBN_T = "9" Then
            ' 振替結果コード異常
            InError.ERRINFO = Err.Name(Err.InputErrorType.FuriketuCode)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 振替結果コード異常：" & InfoMeisaiMast.FURIKETU_CODE
        ElseIf mInfoComm.INFOToriMast.NS_KBN_T = "1" Then   '振込指定区分対応 2010.02.09
            InfoMeisaiMast.FURIKETU_CODE = 0
        End If

        '規定外文字チェックをインプットエラーとして出力
        Dim kiteiRem As Long = CheckRegularString()

        If kiteiRem <> -1 Then
            ' 規定外文字異常
            If SouInputErr <> "0" Then
                InfoMeisaiMast.FURIKETU_CODE = 9
            End If
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kiteigaimoji)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 規定外文字：" & kiteiRem & "バイト目"
        End If

        '金融機関マスタ存在チェックフラグ追加
        '金融機関マスタ存在チェックを行うかどうかを判定するフラグ
        Dim TenMastExistCheck_Flg As Boolean = True

        '金融機関コード数値チェック
        If IsDecimal(InfoMeisaiMast.KEIYAKU_KIN) = False OrElse
            InfoMeisaiMast.KEIYAKU_KIN.Equals("0000") = True OrElse
            InfoMeisaiMast.KEIYAKU_KIN.Equals("9999") = True Then
            ' 銀行コード数値異常
            If SouInputErr <> "0" Then
                InfoMeisaiMast.FURIKETU_CODE = 9
            End If
            '銀行コード異常時のメッセージを変更(店番異常→銀行コード異常)
            InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN

            '数値異常の場合、フラグをFALSEにする処理を追加
            TenMastExistCheck_Flg = False

        ElseIf IsDecimal(InfoMeisaiMast.KEIYAKU_SIT) = False OrElse
            InfoMeisaiMast.KEIYAKU_SIT.Equals("999") = True Then

            'ゆうちょ銀行の場合は"000"と"999"を異常としない
            If InfoMeisaiMast.KEIYAKU_KIN.Equals("9900") = False Then
                ' 店番数値異常
                If SouInputErr <> "0" Then
                    InfoMeisaiMast.FURIKETU_CODE = 9
                End If
                InError.ERRINFO = Err.Name(Err.InputErrorType.Tenban)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 支店コード：" & InfoMeisaiMast.KEIYAKU_SIT

                '数値異常の場合、フラグをFALSEにする処理を追加
                TenMastExistCheck_Flg = False
            End If
        End If

        Select Case mInfoComm.INFOToriMast.SYUBETU_T
            Case "91"
                '科目チェックに9を許す
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 0
                        '国税フォーマットは0を許す
                        If mInfoComm.INFOToriMast.FMT_KBN_T <> "02" Then
                            ' 科目異常
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                    Case 1, 2, 3, 9
                    Case Else
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- START
                        '' 科目異常
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 9
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_91").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                            ' 科目異常
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- END
                End Select
            Case "11", "12"
                '科目チェックに9を許す
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 9
                    Case Else
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- START
                        '' 科目異常
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 9
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        Select Case mInfoComm.INFOToriMast.SYUBETU_T
                            Case "11"
                                If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_11").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' 科目異常
                                    If SouInputErr <> "0" Then
                                        InfoMeisaiMast.FURIKETU_CODE = 9
                                    End If
                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                            Case "12"
                                If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_12").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' 科目異常
                                    If SouInputErr <> "0" Then
                                        InfoMeisaiMast.FURIKETU_CODE = 9
                                    End If
                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                        End Select
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- END
                End Select
            Case "21"
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 4, 9
                    Case Else
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- START
                        '' 科目異常
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 9
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        '' 2008.04.22 ADD
                        'DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_21").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                            ' 科目異常
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            ' 2008.04.22 ADD
                            DataInfo.Message = InError.ERRINFO & " 科目：" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                        '2017/12/18 タスク）綾部 CHG 高崎信金機能標準適用(UI_5-01,5-11<PG>) -------------------- END
                End Select
        End Select

        ' 新規コードチェック
        '新規コードに空白を許す
        '2010/09/08.Sakon　新規コードチェックを行うか否かをＩＮＩファイルに指定する ++++++++++++++++++++++
        If mSinkiCheckFlag = "1" Then
            '***修正 maeda 2008/05/15****************************************************************
            '新規コードに空白を許す
            Select Case InfoMeisaiMast.SINKI_CODE
                Case " ", "0", "1", "2"
                Case Else
                    ' 新規コード異常
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.SinkiCode)
                    InErrorArray.Add(InError)
                    ' 2008.04.22 ADD
                    DataInfo.Message = InError.ERRINFO & " 新規コード：" & InfoMeisaiMast.SINKI_CODE
            End Select
            'Select Case InfoMeisaiMast.SINKI_CODE
            '    Case " ", "0", "1", "2"
            '    Case Else
            '        ' 新規コード異常
            '        InfoMeisaiMast.FURIKETU_CODE = 9
            '        InError.ERRINFO = Err.Name(Err.InputErrorType.SinkiCode)
            '        InErrorArray.Add(InError)
            '        ' 2008.04.22 ADD
            '        DataInfo.Message = InError.ERRINFO & " 新規コード：" & InfoMeisaiMast.SINKI_CODE
            'End Select
            '****************************************************************************************
        End If
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        Dim KouzaCheck As Boolean = True

        '口座番号内のハイフンは省き、頭0埋めする
        InfoMeisaiMast.KEIYAKU_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA.Replace("-"c, "").PadLeft(7, "0"c)
        '科目が9かつ口座番号がALL0又はALL9の時は口座チェックなし
        '国税で科目が0かつ口座番号がALL0の時は口座チェックなし
        Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
            Case 0
                If mInfoComm.INFOToriMast.FMT_KBN_T = "02" AndAlso InfoMeisaiMast.KEIYAKU_KOUZA.Trim = "0000000" Then
                    KouzaCheck = False
                End If

            Case 9
                '総振の場合はALL0の口座チェックをする
                Select Case InfoMeisaiMast.KEIYAKU_KOUZA.Trim
                    Case "0000000"
                        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
                            KouzaCheck = False
                        End If

                    Case "9999999"
                        KouzaCheck = False
                End Select
        End Select

        '口座番号チェックディジットチェック処理時も、KOUZACHECKフラグを参照するよう修正
        If KouzaCheck = True Then
            If mCheckDigitFlag = "1" Then
                '口座番号チェックデジットチェック
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    If CheckDigitCheck() = False Then
                        ' 口座番号異常
                        If SouInputErr <> "0" Then
                            InfoMeisaiMast.FURIKETU_CODE = 9
                        End If
                        InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                        KouzaCheck = False
                    End If
                End If
            End If
        End If

        If KouzaCheck = True Then
            If mInfoComm.INFOToriMast.FMT_KBN_T = "02" Then
                ' 国税 先頭スペースはOK
                If IsDecimal(InfoMeisaiMast.KEIYAKU_KOUZA.TrimStart) = False Then
                    ' 口座番号異常
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
                End If
            Else
                If IsDecimal(InfoMeisaiMast.KEIYAKU_KOUZA) = False Then
                    ' 口座番号異常
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
                End If
            End If
        End If

        '口座番号ALL0は振替結果コード2をセットする
        If KouzaCheck = True AndAlso (InfoMeisaiMast.KEIYAKU_KOUZA = "0000000" OrElse InfoMeisaiMast.KEIYAKU_KOUZA = "9999999") Then
            ' 口座番号異常
            Select Case InfoMeisaiMast.KEIYAKU_KOUZA
                Case "0000000"
                    If SouInputErr <> "0" Then
                        '大阪センタの場合は停止しない
                        If CASTCommon.GetFSKJIni("COMMON", "CENTER") <> "5" Then
                            InfoMeisaiMast.FURIKETU_CODE = 2
                            InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        End If
                    End If

                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
                Case "9999999"
                    '総振の場合はALL9をエラーとしない
                    If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
                        If SouInputErr <> "0" Then
                            '大阪センタの場合は停止しない
                            If CASTCommon.GetFSKJIni("COMMON", "CENTER") <> "5" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                        End If

                        InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " 口座番号：" & InfoMeisaiMast.KEIYAKU_KOUZA
                        KouzaCheck = False
                    End If
            End Select
        End If

        '支店読み替え
        With InfoMeisaiMast
            If SitenYomikae = "1" Then
                '支店読み替え対象
                Call fn_TENPO_YOMIKAE(.KEIYAKU_KIN, .KEIYAKU_SIT, .KEIYAKU_KIN, .KEIYAKU_SIT)
            End If
        End With

        '他行データ作成非対象の場合、自金庫チェック
        '120byte（集金代行対応:他行データも登録できるようにする）
        If mInfoComm.INFOToriMast.FMT_KBN_T <> "20" AndAlso mInfoComm.INFOToriMast.FMT_KBN_T <> "21" Then
            ' 集金代行フォーマットでは無い場合
            If mInfoComm.INFOToriMast.TAKO_KBN_T = "0" And
                InfoMeisaiMast.KEIYAKU_KIN <> "0000" And InfoMeisaiMast.KEIYAKU_KIN <> "9999" Then  '他行区分非対称 
                If InfoMeisaiMast.KEIYAKU_KIN <> JIKINKO Then
                    ' 他行異常
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.Takou)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN
                End If
            End If
        Else
            If mInfoComm.INFOToriMast.FMT_KBN_T = "20" Then
                ' ＳＳＳ提携内 他行異常
                Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT)
                If Not OraReader Is Nothing AndAlso OraReader.GetItem("TEIKEI_KBN_N") <> "1" Then
                    ' 提携外の金融機関の場合，ＳＳＳ提携外
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.TeikeiGai)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN
                    OraReader.Close()
                End If
            End If
        End If

        If mInfoComm.INFOToriMast.TAKO_KBN_T = "1" Then
            If InfoMeisaiMast.KEIYAKU_KIN <> JIKINKO Then
                TAKOU_ON = True             '他行有り
            End If
        End If


        '口座読み替え
        With InfoMeisaiMast
            If KouzaYomikae = "1" AndAlso JIKINKO = .KEIYAKU_KIN Then
                '支店読み替え対象
                Call fn_KOUZA_YOMIKAE(.KEIYAKU_SIT, .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA,
                                                .KEIYAKU_SIT, .KEIYAKU_KOUZA, .IDOU_DATE)
            End If
        End With

        '金融機関コード存在チェック
        Dim nRet As Integer
        If TenMastExistCheck_Flg = True Then
            nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
        Else '金融機関コード数値チェック失敗
            nRet = 9
        End If

        '金融機関取得処理時のエラー制御を変更(以下の通り)
        '===================================================================
        '0:金融機関取得失敗(GetTENMASTExistで例外発生)
        '1:金融機関あり支店なし
        '2:金融機関あり支店あり(正常終了)
        '3:振替日が削除日より後(店舗統廃合)
        '9:金融機関コード数値チェック失敗
        '===================================================================
        Select Case nRet
            Case 0 '金融機関なしの場合

                '金融機関なし
                '2018/10/16 saitou 広島信金(RSV2標準) UPD （金融機関マスタチェック） ------------------------------ START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "GINKOUCODE") = "1" Then
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 2
                        InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                End If
                'If SouInputErr <> "0" Then
                '    InfoMeisaiMast.FURIKETU_CODE = 2
                '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                'End If
                'InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
                'InErrorArray.Add(InError)
                'DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                '2018/10/16 saitou 広島信金(RSV2標準) UPD --------------------------------------------------------- END

            Case 1 '金融機関あり，支店なし
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    '自行で支店なしの場合は自行店番異常
                    '2018/10/05 saitou 広島信金(RSV2標準) UPD （金融機関マスタチェック） ------------------------------ START
                    If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "JIKOUTENBAN") = "1" Then
                        If SouInputErr <> "0" Then
                            InfoMeisaiMast.FURIKETU_CODE = 2
                            InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        End If
                        InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                    End If
                    'If SouInputErr <> "0" Then
                    '    InfoMeisaiMast.FURIKETU_CODE = 2
                    '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    'End If
                    'InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    'InErrorArray.Add(InError)
                    'DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                    '2018/10/05 saitou 広島信金(RSV2標準) UPD --------------------------------------------------------- END

                    '支店なしの場合は他行店番異常
                Else
                    '他行で支店なしの場合は他行店番異常
                    'SSSでゆうちょ銀行の場合は他行店番異常としない
                    If Not ((mInfoComm.INFOToriMast.FMT_KBN_T = "20" Or mInfoComm.INFOToriMast.FMT_KBN_T = "21") AndAlso InfoMeisaiMast.KEIYAKU_KIN = "9900") Then
                        '2018/10/15 saitou 広島信金(RSV2標準) UPD （金融機関マスタチェック） ------------------------------ START
                        If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TAKOUTENBAN") = "1" Then
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 2
                                InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.TakouTenban)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                        End If
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 2
                        '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.TakouTenban)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                        '2018/10/05 saitou 広島信金(RSV2標準) UPD --------------------------------------------------------- END
                    End If
                End If
            Case 2 '金融機関あり，支店あり
                '自行で店番000の場合は自行店番異常
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO AndAlso
                   InfoMeisaiMast.KEIYAKU_SIT = "000" Then

                    '2018/10/15 saitou 広島信金(RSV2標準) UPD （金融機関マスタチェック） ------------------------------ START
                    If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "JIKOUTENBAN") = "1" Then
                        If SouInputErr <> "0" Then
                            InfoMeisaiMast.FURIKETU_CODE = 2
                            InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        End If
                        InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                    End If
                    'If SouInputErr <> "0" Then
                    '    InfoMeisaiMast.FURIKETU_CODE = 2
                    '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    'End If
                    'InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    'InErrorArray.Add(InError)
                    'DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                    '2018/10/05 saitou 広島信金(RSV2標準) UPD --------------------------------------------------------- END
                End If

                '正常終了のため処理なし

            Case 3 '振替日が削除日より後(店舗統廃合)
                '2018/10/15 saitou 広島信金(RSV2標準) UPD （金融機関マスタチェック） ------------------------------ START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TENPOTOUGOU") = "1" Then
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 2
                        InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.TenpoTougou)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                End If
                'If SouInputErr <> "0" Then
                '    InfoMeisaiMast.FURIKETU_CODE = 2
                '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                'End If
                'InError.ERRINFO = Err.Name(Err.InputErrorType.TenpoTougou)
                'InErrorArray.Add(InError)
                'DataInfo.Message = InError.ERRINFO & " 金融機関コード：" & InfoMeisaiMast.KEIYAKU_KIN & "支店コード：" & InfoMeisaiMast.KEIYAKU_SIT
                '2018/10/05 saitou 広島信金(RSV2標準) UPD --------------------------------------------------------- END
            Case 9 '金融機関コード数値チェックで失敗した場合
            Case Else '例外

        End Select

        '総振で受取人名が空白の場合はエラーとする
        If mInfoComm.INFOToriMast.FSYORI_KBN_T = "3" AndAlso InfoMeisaiMast.KEIYAKU_KNAME.Trim = "" Then
            InfoMeisaiMast.FURIKETU_CODE = 9

            InError.ERRINFO = Err.Name(Err.InputErrorType.Kana)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 受取人名なし"
        End If

        '金額チェック
        If IsDecimal(InfoMeisaiMast.FURIKIN_MOTO) = False Then
            If SouInputErr <> "0" Then
                InfoMeisaiMast.FURIKETU_CODE = 9
            End If
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " 金額：" & InfoMeisaiMast.FURIKIN_MOTO
        Else
            If InfoMeisaiMast.FURIKIN < 0 Then
                ' マイナス金額
                If SouInputErr <> "0" Then
                    InfoMeisaiMast.FURIKETU_CODE = 9
                End If
                InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " 金額：" & InfoMeisaiMast.FURIKIN_MOTO
            End If

            If InfoMeisaiMast.FURIKIN = 0 Then
                InfoMeisaiMast.FURIKETU_CODE = 0
                'NHKはインプットエラーにのせない
                If mInfoComm.INFOToriMast.FMT_KBN_T <> "01" Then
                    InError.ERRINFO = Err.Name(Err.InputErrorType.KingakuZero)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " 金額：" & InfoMeisaiMast.FURIKIN_MOTO
                End If
            End If
        End If

        InErrorArray.TrimToSize()

        If InErrorArray.Count > 0 Then
            Return False
        End If
        Return True
    End Function
    '2010/02/03 処理高速化 ========================
    Function fn_TENPO_YOMIKAE(ByVal astrKIN_NO As String, ByVal astrSIT_NO As String, ByRef astrNEW_KIN_NO As String, ByRef astrNEW_SIT_NO As String) As Boolean
        '=====================================================================================
        'NAME           :fn_TENPO_YOMIKAE
        'Parameter      :astrKIN_NO：金融機関コード／astrSIT_NO：支店コード／
        '               :astrNEW_KIN_NO：読み替え後金融機関コード／astrNEW_SIT_NO：読み替え後支店コード
        'Description    :YOMIKAEMASTから店舗読み替えを行う
        'Return         :読み替え後の金融機関コード、支店コード、True=OK(読み替え済み),False=NG（未読み替え）
        'Create         :2010/02/03
        'Update         :
        '=====================================================================================
        fn_TENPO_YOMIKAE = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            SQL.Append("SELECT NEW_KIN_NO_S, NEW_SIT_NO_S FROM SITENYOMIKAE")
            SQL.Append(" WHERE OLD_KIN_NO_S = " & SQ(astrKIN_NO))
            SQL.Append(" AND OLD_SIT_NO_S = " & SQ(astrSIT_NO))

            If OraReader.DataReader(SQL) Then
                astrNEW_KIN_NO = OraReader.GetString("NEW_KIN_NO_S")
                astrNEW_SIT_NO = OraReader.GetString("NEW_SIT_NO_S")
                fn_TENPO_YOMIKAE = True
            Else
                astrNEW_KIN_NO = astrKIN_NO
                astrNEW_SIT_NO = astrSIT_NO
            End If

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function
    Function fn_KOUZA_YOMIKAE(ByVal astrSIT_NO As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String,
                          ByRef astrNEW_SIT_NO As String, ByRef astrNEW_KOUZA As String, ByRef astrIDOU_DATE As String) As Boolean
        '=====================================================================================
        'NAME           :fn_KOUZA_YOMIKAE
        'Parameter      :astrKIN_NO：金融機関コード／astrSIT_NO：支店コード／astrKAMOKU：科目
        '               :astrKOUZA：口座番号
        'Description    :KDBMASTから口座読み替えを行う
        'Return         :読み替え後の支店コード、口座番号、True=OK(読み替え済み),False=NG（未読み替え）
        'Create         :2010/02/03
        'Update         :
        '=====================================================================================
        fn_KOUZA_YOMIKAE = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            astrKAMOKU = CASTCommon.ConvertKamoku1TO2(astrKAMOKU)

            SQL.Append("SELECT TSIT_NO_D, KOUZA_D, IDOU_DATE_D FROM KDBMAST")
            SQL.Append(" WHERE OLD_TSIT_NO_D = '" & astrSIT_NO & "'")
            SQL.Append(" AND KAMOKU_D = '" & astrKAMOKU & "'")
            SQL.Append(" AND OLD_KOUZA_D = '" & astrKOUZA & "'")

            If OraReader.DataReader(SQL) Then
                astrNEW_SIT_NO = OraReader.GetString("TSIT_NO_D").PadLeft(3, "0"c)
                astrNEW_KOUZA = OraReader.GetString("KOUZA_D").PadLeft(7, "0"c)
                astrIDOU_DATE = OraReader.GetString("IDOU_DATE_D")
                fn_KOUZA_YOMIKAE = True
            Else
                astrNEW_SIT_NO = astrSIT_NO.PadLeft(3, "0"c)
                astrNEW_KOUZA = astrKOUZA.PadLeft(7, "0"c)
                astrIDOU_DATE = ""
            End If

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function
    '==================================================

    '
    ' 機能　 ： トレーラーレコードを読み込んでチェック
    '
    ' 備考　 ：
    '
    Protected Function CheckTrailerRecord() As Boolean
        '依頼件数合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO) = False Then
            WriteBLog("ファイルトレーラ件数", "異常", "件数：" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ件数異常 件数:" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO
            Return False
        End If

        '依頼金額合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO) = False Then
            WriteBLog("ファイルトレーラ金額", "異常", "金額：" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ金額異常 金額:" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO
            Return False
        End If

        '2009/12/03 トレーラチェックに、0円を除いた場合を追加 ==============
        'If InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN Then
        If InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN AndAlso
            InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN2 Then
            '===============================================================
            WriteBLog("ファイルトレーラ件数", "不一致", "件数：" & InfoMeisaiMast.TOTAL_KEN.ToString)
            '2010/10/20 トレーラ件数不一致時のメッセージを金額不一致の場合と同様に集計値、トレーラの文言を表示させる-------------------------
            'DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ件数不一致 件数:" & InfoMeisaiMast.TOTAL_KEN.ToString & "," & InfoMeisaiMast.TOTAL_IRAI_KEN.ToString
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ件数不一致 件数(集計値):" & InfoMeisaiMast.TOTAL_KEN.ToString & " 件数(トレーラ):" & InfoMeisaiMast.TOTAL_IRAI_KEN.ToString
            '--------------------------------------------------------------------------------------------------------------------------------
            Return False
        End If

        '***修正 maeda 2008/05/16********************************************************************************************************************************************
        'トレーラ金額不一致時のメッセージに集計金額と、実トレーラの金額を表示するように変更
        If InfoMeisaiMast.TOTAL_IRAI_KIN <> InfoMeisaiMast.TOTAL_KIN Then
            WriteBLog("ファイルトレーラ金額", "不一致", "金額：" & InfoMeisaiMast.TOTAL_KIN.ToString)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ金額不一致 金額(集計値):" & InfoMeisaiMast.TOTAL_KIN.ToString & "　金額(トレーラ):" & InfoMeisaiMast.TOTAL_IRAI_KIN.ToString
            Return False
        End If

        'If InfoMeisaiMast.TOTAL_IRAI_KIN <> InfoMeisaiMast.TOTAL_KIN Then
        '    WriteBLog("ファイルトレーラ金額", "不一致", "金額：" & InfoMeisaiMast.TOTAL_KIN.ToString)
        '    DataInfo.Message = "ファイルトレーラ金額不一致 金額:" & InfoMeisaiMast.TOTAL_KIN.ToString
        '    Return False
        'End If

        '********************************************************************************************************************************************************************

        If DataInfo.RecoedLen <> RecordData.Length Then
            WriteBLog("トレーラレコード長", "不一致", "レコード長：" & RecordData.Length.ToString)
            DataInfo.Message = "ﾄﾚｰﾗﾚｺｰﾄﾞ長不一致 ﾚｺｰﾄﾞ長:" & RecordData.Length.ToString
            Return False
        End If

        Return True
    End Function

    '
    ' 機能　 ： エンドレコードを読み込んでチェック
    '
    ' 備考　 ：
    '
    Protected Function CheckEndRecord() As Boolean

        '2014/05/21 saitou 標準版 DEL -------------------------------------------------->>>>
        '' 2008.04.23 ADD コムフロント振替日マルチ対応 >>
        'PreFURI_DATE = ""
        '' 2008.04.23 ADD コムフロント振替日マルチ対応 <<
        '2014/05/21 saitou 標準版 DEL --------------------------------------------------<<<<

        If DataInfo.RecoedLen <> RecordData.Length Then
            WriteBLog("エンドレコード長", "不一致", "レコード長：" & RecordData.Length.ToString)
            DataInfo.Message = "ｴﾝﾄﾞﾚｺｰﾄﾞ長不一致 ﾚｺｰﾄﾞ長:" & RecordData.Length.ToString
            Return False
        End If

        Return True
    End Function

    '
    ' 機能　 ： トレーラーレコードを読み込んでチェック
    '
    ' 備考　 ：
    '
    Protected Function CheckTrailerRecordFunou() As Boolean
        '依頼件数合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO) = False Then
            WriteBLog("ファイルトレーラ依頼件数", "異常", "件数：" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ依頼件数異常 件数:" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO
            Return False
        End If

        '依頼金額合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO) = False Then
            WriteBLog("ファイルトレーラ依頼金額", "異常", "金額：" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ依頼金額異常 金額:" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO
            Return False
        End If

        '振替済み件数合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO) = False Then
            WriteBLog("ファイルトレーラ振替済み件数", "異常", "件数：" & InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ振替済件数異常 件数:" & InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO
            Return False
        End If

        '振替済み金額合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO) = False Then
            WriteBLog("ファイルトレーラ振替済み金額", "異常", "金額：" & InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ振替済金額異常 金額:" & InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO
            Return False
        End If

        '不能件数合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO) = False Then
            WriteBLog("ファイルトレーラ不能件数", "異常", "件数：" & InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ不能件数異常 件数:" & InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO
            Return False
        End If

        '不能金額合計チェック
        If IsDecimal(InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO) = False Then
            WriteBLog("ファイルトレーラ不能金額", "異常", "金額：" & InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ不能金額異常 金額:" & InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO
            Return False
        End If


        If InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN Then
            WriteBLog("ファイルトレーラ依頼件数", "不一致", "件数：" & InfoMeisaiMast.TOTAL_KEN.ToString)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ依頼件数不一致 件数:" & InfoMeisaiMast.TOTAL_KEN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_IRAI_KIN <> InfoMeisaiMast.TOTAL_KIN Then
            WriteBLog("ファイルトレーラ依頼金額", "不一致", "金額：" & InfoMeisaiMast.TOTAL_KIN.ToString)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ依頼金額不一致 金額:" & InfoMeisaiMast.TOTAL_KIN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_ZUMI_KEN <> InfoMeisaiMast.TOTAL_NORM_KEN Then
            WriteBLog("ファイルトレーラ振替済み件数", "不一致", "件数：" & InfoMeisaiMast.TOTAL_NORM_KEN.ToString)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ振替済件数不一致 件数:" & InfoMeisaiMast.TOTAL_NORM_KEN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_ZUMI_KIN <> InfoMeisaiMast.TOTAL_NORM_KIN Then
            WriteBLog("ファイルトレーラ振替済み金額", "不一致", "金額：" & InfoMeisaiMast.TOTAL_NORM_KIN.ToString)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ振替済金額不一致 金額:" & InfoMeisaiMast.TOTAL_NORM_KIN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_FUNO_KEN <> InfoMeisaiMast.TOTAL_IJO_KEN Then
            WriteBLog("ファイルトレーラ不能件数", "不一致", "件数：" & InfoMeisaiMast.TOTAL_IJO_KEN.ToString)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ不能件数不一致 金額:" & InfoMeisaiMast.TOTAL_IJO_KEN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_FUNO_KIN <> InfoMeisaiMast.TOTAL_IJO_KIN Then
            WriteBLog("ファイルトレーラ不能金額", "不一致", "金額：" & InfoMeisaiMast.TOTAL_IJO_KIN.ToString)
            DataInfo.Message = "ﾌｧｲﾙﾄﾚｰﾗ不能金額不一致 金額:" & InfoMeisaiMast.TOTAL_IJO_KIN.ToString
            Return False
        End If

        Return True
    End Function

    ' 機能　 ： 店舗マスタの存在を確認する
    '
    ' 引数   ： ARG1 - 金融機関コード
    '        ： ARG2 - 支店コード
    '        ： ARG3 - 振替日
    '
    ' 戻り値 ： 0 - 金融機関なし，1 - 金融機関あり，支店なし，2 - 金融機関，支店あり
    '
    ' 備考　 ：
    '
    Public Function GetTENMASTExists(ByVal kinno As String, ByVal sitno As String, ByVal furiDate As String) As Integer

        ' ＤＢ接続が存在しない場合，正常値を返す
        If OraDB Is Nothing Then
            Return 2
        End If


        Dim nRet As Integer = 0
        Try
            Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(kinno, sitno)
            If Not OraReader Is Nothing Then
                If OraReader.EOF = False Then

                    '2009/09/10.Sakon　削除日チェックに支店削除日を追加 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '' 2008.04.22 ADD OraReader.GetItem("SIT_N") = "OK" 追加 
                    'If OraReader.GetItem("DEL_KIN_DATE_N") <> "" AndAlso OraReader.GetItem("DEL_KIN_DATE_N") < furiDate AndAlso OraReader.GetItem("SIT_N") = "OK" Then
                    'If ((OraReader.GetItem("KIN_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("KIN_DEL_DATE_N") < furiDate) Or _
                    '    (OraReader.GetItem("SIT_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("SIT_DEL_DATE_N") < furiDate)) AndAlso OraReader.GetItem("SIT_N") = "OK" Then
                    If ((OraReader.GetItem("KIN_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("KIN_DEL_DATE_N") <> "00000000" AndAlso OraReader.GetItem("KIN_DEL_DATE_N") < furiDate) Or
                        (OraReader.GetItem("SIT_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("SIT_DEL_DATE_N") <> "00000000" AndAlso OraReader.GetItem("SIT_DEL_DATE_N") < furiDate)) AndAlso OraReader.GetItem("SIT_N") = "OK" Then

                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        ' 振替日が削除日よりあとになります
                        nRet = 3
                    Else
                        If OraReader.GetItem("SIT_N") = "OK" Then
                            ' 金融機関，支店あり
                            nRet = 2
                        Else
                            ' 金融機関，支店なし
                            nRet = 1
                        End If
                    End If
                End If
                OraReader.Close()
            End If
            OraReader = Nothing
        Catch ex As Exception

        End Try

        Return nRet
    End Function

    ' 機能　 ： 店舗マスタを取得する
    '
    ' 引数   ： ARG1 - 金融機関コード
    '           ARG2 - 支店コード
    '
    ' 備考　 ： 2017/01/16 saitou 東春信金(RSV2標準) update for スリーエス対応
    '           ・TEIKEI_KBN_N(提携区分)の復活
    '
    Public Function GetTENMAST(ByVal kinno As String, ByVal sitno As String) As CASTCommon.MyOracleReader
        Dim SQL As New StringBuilder(256)
        Dim OraReader As CASTCommon.MyOracleReader

        Try

            ' 2016/01/27 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            ' ＤＢ接続が存在しない場合，処理を行わない
            If OraDB Is Nothing Then
                Return Nothing
            End If
            ' 2016/01/27 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            SQL.Append("SELECT")
            SQL.Append(" KIN_NNAME_N")
            SQL.Append(",SIT_NNAME_N")
            SQL.Append(",KIN_KNAME_N")
            SQL.Append(",SIT_KNAME_N")
            SQL.Append(",KIN_DEL_DATE_N")
            SQL.Append(",SIT_DEL_DATE_N")
            SQL.Append(",TEIKEI_KBN_N")
            SQL.Append(",'OK' SIT_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(kinno))
            SQL.Append("   AND SIT_NO_N = " & SQ(sitno))

            OraReader = New CASTCommon.MyOracleReader(OraDB)

            If OraReader.DataReader(SQL) = True Then
                Return OraReader
            Else
                OraReader.Close()

                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append(" KIN_NNAME_N")
                SQL.Append(",SIT_NNAME_N")
                SQL.Append(",KIN_KNAME_N")
                SQL.Append(",SIT_KNAME_N")
                SQL.Append(",KIN_DEL_DATE_N")
                SQL.Append(",SIT_DEL_DATE_N")
                SQL.Append(",TEIKEI_KBN_N")
                SQL.Append(",'NG' SIT_N")
                SQL.Append(" FROM TENMAST")
                SQL.Append(" WHERE KIN_NO_N = " & SQ(kinno))

                OraReader = New CASTCommon.MyOracleReader(OraDB)
                If OraReader.DataReader(SQL) = True Then
                    Return OraReader
                End If
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        Finally
        End Try
    End Function

    ' 機能　 ： スケジュールマスタを登録する
    '
    ' 備考　 ：
    '
    Private Function InsertSCHMAST() As Boolean
        Dim SQL As New StringBuilder(1024)

        ' ＤＢ接続が存在しない場合，正常値を返す
        If OraDB Is Nothing Then
            Return True
        End If

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If Not BLOG Is Nothing Then
            sw = BLOG.Write_Enter1("ClsFormat.InsertSCHMAST")
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Dim SCHData As CAstFormatMini.ClsSchduleMaintenanceClass.SCHMAST_Data

        ' 2016/04/25 タスク）綾部 DEL 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
        ''コムフロント振替日マルチ対応 >>
        'If Not InfoMeisaiMast.FURIKAE_DATE Is Nothing AndAlso InfoMeisaiMast.FURIKAE_DATE <> "" Then
        '    Dim IParam As CAstBatch.CommData.stPARAMETER
        '    IParam = mInfoComm.INFOParameter
        '    IParam.FURI_DATE = InfoMeisaiMast.FURIKAE_DATE
        '    mInfoComm.INFOParameter = IParam
        'End If
        ''コムフロント振替日マルチ対応 <<
        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

        ' スケジュール 予定日数算出
        Dim CLS As New CAstFormatMini.ClsMain(mInfoComm.INFOToriMast.TORIS_CODE_T, mInfoComm.INFOToriMast.TORIF_CODE_T, mInfoComm.INFOParameter.FURI_DATE)

        SCHData = CLS.SCHData

        CLS = Nothing

        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        '持込期日を退避
        MOTIKOMI_DATE = SCHData.MOTIKOMI_DATE
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

        ' 振込日
        Dim FurikaeDate As Date = ConvertDate(mInfoComm.INFOParameter.FURI_DATE)
        If FurikaeDate.ToString("yyyyMMdd") <> mInfoComm.INFOParameter.FURI_DATE Then
            WriteBLog("入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "異常", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日"))
            DataInfo.Message = "入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "異常　" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日")
            Return False
        End If

        ' 曜日判定
        Select Case FurikaeDate.DayOfWeek()
            Case DayOfWeek.Saturday
                WriteBLog("入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "土曜日", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日"))
                DataInfo.Message = "入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "（土曜日）　" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日")
                Return False
            Case DayOfWeek.Sunday
                WriteBLog("入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "日曜日", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日"))
                DataInfo.Message = "入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "（日曜日）　" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日")
                Return False
        End Select

        ' 休日判定
        If HolidayList.BinarySearch(FurikaeDate.ToString("yyyyMMdd")) >= 0 Then
            WriteBLog("入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "休日", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日"))
            DataInfo.Message = "入力" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "（休日）　" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "：" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM月dd日")
            Return False
        End If

        '------------------
        'マスタ登録項目設定
        '------------------
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = mInfoComm.INFOParameter
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = mInfoComm.INFOToriMast

        SQL = New StringBuilder(512)
        SQL.AppendLine("INSERT INTO S_SCHMAST(")
        SQL.AppendLine(" FSYORI_KBN_S")                     ' FSYORI_KBN_S
        SQL.AppendLine(",TORIS_CODE_S")                     ' TORIS_CODE_S
        SQL.AppendLine(",TORIF_CODE_S")                     ' TORIF_CODE_S
        SQL.AppendLine(",FURI_DATE_S")                      ' FURI_DATE_S
        SQL.AppendLine(",KFURI_DATE_S")                     ' KFURI_DATE_S
        SQL.AppendLine(",SYUBETU_S")                        ' SYUBETU_S
        SQL.AppendLine(",FURI_CODE_S")                      ' FURI_CODE_S
        SQL.AppendLine(",KIGYO_CODE_S")                     ' KIGYO_CODE_S
        SQL.AppendLine(",ITAKU_CODE_S")                     ' ITAKU_CODE_S
        SQL.AppendLine(",TKIN_NO_S")                        ' TKIN_NO_S
        SQL.AppendLine(",TSIT_NO_S")                        ' TSIT_NO_S
        SQL.AppendLine(",SOUSIN_KBN_S")                     ' SOUSIN_KBN_S
        SQL.AppendLine(",BAITAI_CODE_S")                    ' BAITAI_CODE_S
        SQL.AppendLine(",MOTIKOMI_SEQ_S")                   ' MOTIKOMI_SEQ_S
        SQL.AppendLine(",FILE_SEQ_S")                       ' FILE_SEQ_S
        SQL.AppendLine(",TESUU_KBN_S")                      ' TESUU_KBN_S
        SQL.AppendLine(",IRAISYO_DATE_S")                   ' IRAISYO_DATE_S
        SQL.AppendLine(",IRAISYOK_YDATE_S")                 ' IRAISYOK_YDATE_S
        SQL.AppendLine(",MOTIKOMI_DATE_S")                  ' MOTIKOMI_DATE_S
        SQL.AppendLine(",UKETUKE_DATE_S")                   ' UKETUKE_DATE_S
        SQL.AppendLine(",TOUROKU_DATE_S")                   ' TOUROKU_DATE_S
        SQL.AppendLine(",KAKUHO_YDATE_S")                   ' KAKUHO_YDATE_S
        SQL.AppendLine(",KAKUHO_DATE_S")                    ' KAKUHO_DATE_S
        SQL.AppendLine(",HASSIN_YDATE_S")                   ' HASSIN_YDATE_S
        SQL.AppendLine(",HASSIN_DATE_S")                    ' HASSIN_DATE_S
        SQL.AppendLine(",SOUSIN_YDATE_S")                   ' SOUSIN_YDATE_S
        SQL.AppendLine(",SOUSIN_DATE_S")                    ' SOUSIN_DATE_S
        SQL.AppendLine(",KESSAI_YDATE_S")                   ' KESSAI_YDATE_S  
        SQL.AppendLine(",KESSAI_DATE_S")                    ' KESSAI_DATE_S    
        SQL.AppendLine(",TESUU_YDATE_S")                    ' TESUU_YDATE_S      
        SQL.AppendLine(",TESUU_DATE_S")                     ' TESUU_DATE_S    
        SQL.AppendLine(",UKETORI_DATE_S")                   ' UKETORI_DATE_S
        SQL.AppendLine(",UKETUKE_FLG_S")                    ' UKETUKE_FLG_S
        SQL.AppendLine(",TOUROKU_FLG_S")                    ' TOUROKU_FLG_S
        SQL.AppendLine(",KAKUHO_FLG_S")                     ' KAKUHO_FLG_S
        SQL.AppendLine(",HASSIN_FLG_S")                     ' HASSIN_FLG_S
        SQL.AppendLine(",SOUSIN_FLG_S")                     ' SOUSIN_FLG_S
        SQL.AppendLine(",TESUUKEI_FLG_S")                   ' TESUUKEI_FLG_S
        SQL.AppendLine(",TESUUTYO_FLG_S")                   ' TESUUTYO_FLG_S
        SQL.AppendLine(",KESSAI_FLG_S")                     ' KESSAI_FLG_S
        SQL.AppendLine(",TYUUDAN_FLG_S")                    ' TYUUDAN_FLG_S
        SQL.AppendLine(",ERROR_INF_S")                      ' ERROR_INF_S
        SQL.AppendLine(",SYORI_KEN_S")                      ' SYORI_KEN_S
        SQL.AppendLine(",SYORI_KIN_S")                      ' SYORI_KIN_S
        SQL.AppendLine(",ERR_KEN_S")                        ' ERR_KEN_S
        SQL.AppendLine(",ERR_KIN_S")                        ' ERR_KIN_S
        SQL.AppendLine(",TESUU_KIN_S")                      ' TESUU_KIN_S
        SQL.AppendLine(",TESUU_KIN1_S")                     ' TESUU_KIN1_S
        SQL.AppendLine(",TESUU_KIN2_S")                     ' TESUU_KIN2_S
        SQL.AppendLine(",TESUU_KIN3_S")                     ' TESUU_KIN3_S
        SQL.AppendLine(",FURI_KEN_S")                       ' FURI_KEN_S
        SQL.AppendLine(",FURI_KIN_S")                       ' FURI_KIN_S
        SQL.AppendLine(",FUNOU_KEN_S")                      ' FUNOU_KEN_S
        SQL.AppendLine(",FUNOU_KIN_S")                      ' FUNOU_KIN_S
        SQL.AppendLine(",UFILE_NAME_S")                     ' UFILE_NAME_S
        SQL.AppendLine(",SFILE_NAME_S")                     ' SFILE_NAME_S
        SQL.AppendLine(",SAKUSEI_DATE_S")                   ' SAKUSEI_DATE_S
        SQL.AppendLine(",KAKUHO_TIME_STAMP_S")              ' KAKUHO_TIME_STAMP_S
        SQL.AppendLine(",HASSIN_TIME_STAMP_S")              ' HASSIN_TIME_STAMP_S
        SQL.AppendLine(",KESSAI_TIME_STAMP_S")              ' KESSAI_TIME_STAMP_S
        SQL.AppendLine(",TESUU_TIME_STAMP_S")               ' TESUU_TIME_STAMP_S
        SQL.AppendLine(",YOBI1_S")                          ' YOBI1_S
        SQL.AppendLine(",YOBI2_S")                          ' YOBI2_S
        SQL.AppendLine(",YOBI3_S")                          ' YOBI3_S
        SQL.AppendLine(",YOBI4_S")                          ' YOBI4_S
        SQL.AppendLine(",YOBI5_S")                          ' YOBI5_S
        SQL.AppendLine(",YOBI6_S")                          ' YOBI6_S
        SQL.AppendLine(",YOBI7_S")                          ' YOBI7_S
        SQL.AppendLine(",YOBI8_S")                          ' YOBI8_S
        SQL.AppendLine(",YOBI9_S")                          ' YOBI9_S
        SQL.AppendLine(",YOBI10_S")                         ' YOBI10_S
        SQL.AppendLine(") VALUES(")
        SQL.AppendLine(" " & SQ(InfoTori.FSYORI_KBN_T))             ' FSYORI_KBN_S
        SQL.AppendLine("," & SQ(InfoTori.TORIS_CODE_T))             ' TORIS_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.TORIF_CODE_T))             ' TORIF_CODE_S
        SQL.AppendLine("," & SQ(InfoMeisaiMast.FURIKAE_DATE))       ' FURI_DATE_S
        SQL.AppendLine("," & SQ(InfoMeisaiMast.FURIKAE_DATE))       ' KFURI_DATE_S
        SQL.AppendLine("," & SQ(InfoTori.SYUBETU_T))                ' SYUBETU_S
        SQL.AppendLine("," & SQ(InfoTori.FURI_CODE_T))              ' FURI_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.KIGYO_CODE_T))             ' KIGYO_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.ITAKU_CODE_T))             ' ITAKU_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.TKIN_NO_T))                ' TKIN_NO_S
        SQL.AppendLine("," & SQ(InfoTori.TSIT_NO_T))                ' TSIT_NO_S
        SQL.AppendLine("," & SQ(InfoTori.SOUSIN_KBN_T))             ' SOUSIN_KBN_S
        SQL.AppendLine("," & SQ(InfoTori.BAITAI_CODE_T))            ' BAITAI_CODE_S
        SQL.AppendLine(",{0}")                                      ' MOTIKOMI_SEQ_S
        SQL.AppendLine("," & (InfoMeisaiMast.FILE_SEQ + 1).ToString) ' FILE_SEQ_S

        '手数料計算区分の算出
        Select Case InfoTori.TESUUTYO_KBN_T
            Case "0"    ' 都度請求
                SQL.AppendLine(",1")                                ' TESUU_KBN_S
            Case "1"    ' 一括徴求
                Select Case InfoTori.TUKI_T(FurikaeDate.Month - 1)
                    Case "1", "3"
                        SQL.AppendLine(",'2'")                      ' TESUU_KBN_S
                    Case Else
                        SQL.AppendLine(",'3'")                      ' TESUU_KBN_S
                End Select
            Case "2"    ' 特別免除
                SQL.AppendLine(",'0'")                              ' TESUU_KBN_S
            Case Else   ' 別途徴求
                SQL.AppendLine(",'0'")                              ' TESUU_KBN_S
        End Select

        SQL.AppendLine(",'00000000'")                               ' IRAISYO_DATE_S        
        SQL.AppendLine("," & SQ(SCHData.IRAISYOK_YDATE))            ' IRAISYOK_YDATE_S
        '2017/12/07 タスク）西野 CHG 標準版修正：広島信金対応（持込期日対応）---------------------- START
        If INI_MOTIKOMI_KIJITU_CHK = "1" Then
            SQL.AppendLine("," & SQ(SCHData.MOTIKOMI_DATE))             ' MOTIKOMI_DATE_S
        Else
            SQL.AppendLine(",'00000000'")                               ' MOTIKOMI_DATE_S
        End If
        'SQL.AppendLine(",'00000000'")                               ' MOTIKOMI_DATE_S
        '2017/12/07 タスク）西野 CHG 標準版修正：広島信金対応（持込期日対応）---------------------- END
        SQL.AppendLine(",'00000000'")                               ' UKETUKE_DATE_S
        SQL.AppendLine(",'00000000'")                               ' TOUROKU_DATE_S
        SQL.AppendLine("," & SQ(SCHData.KAKUHO_YDATE))              ' KAKUHO_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' KAKUHO_DATE_S
        SQL.AppendLine("," & SQ(SCHData.HASSIN_YDATE))              ' HASSIN_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' HASSIN_DATE_S
        SQL.AppendLine("," & SQ(SCHData.HASSIN_YDATE))              ' SOUSIN_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' SOUSIN_DATE_S
        SQL.AppendLine("," & SQ(SCHData.KESSAI_YDATE))              ' KESSAI_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' KESSAI_DATE_S
        SQL.AppendLine("," & SQ(SCHData.TESUU_YDATE))               ' TESUU_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' TESUU_DATE_S
        SQL.AppendLine(",'00000000'")                               ' UKETORI_DATE_S
        SQL.AppendLine(",'0'")                                      ' UKETUKE_FLG_S
        SQL.AppendLine(",'0'")                                      ' TOUROKU_FLG_S
        SQL.AppendLine(",'0'")                                      ' KAKUHO_FLG_S
        SQL.AppendLine(",'0'")                                      ' HASSIN_FLG_S
        SQL.AppendLine(",'0'")                                      ' SOUSIN_FLG_S
        SQL.AppendLine(",'0'")                                      ' TESUUKEI_FLG_S
        SQL.AppendLine(",'0'")                                      ' TESUUTYO_FLG_S
        SQL.AppendLine(",'0'")                                      ' KESSAI_FLG_S
        SQL.AppendLine(",'0'")                                      ' TYUUDAN_FLG_S
        SQL.AppendLine(",' '")                                      ' ERROR_INF_S
        SQL.AppendLine(",0")                                        ' SYORI_KEN_S
        SQL.AppendLine(",0")                                        ' SYORI_KIN_S
        SQL.AppendLine(",0")                                        ' ERR_KEN_S
        SQL.AppendLine(",0")                                        ' ERR_KIN_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN1_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN2_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN3_S
        SQL.AppendLine(",0")                                        ' FURI_KEN_S
        SQL.AppendLine(",0")                                        ' FURI_KIN_S
        SQL.AppendLine(",0")                                        ' FUNOU_KEN_S
        SQL.AppendLine(",0")                                        ' FUNOU_KIN_S
        SQL.AppendLine(",' '")                                      ' UFILE_NAME_S
        SQL.AppendLine(",' '")                                      ' SFILE_NAME_S
        SQL.AppendLine("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' SAKUSEI_DATE_S
        SQL.AppendLine(",'00000000000000'")                         ' KAKUHO_TIME_STAMP_S
        SQL.AppendLine(",'00000000000000'")                         ' HASSIN_TIME_STAMP_S
        SQL.AppendLine(",'00000000000000'")                         ' KESSAI_TIME_STAMP_S
        SQL.AppendLine(",'00000000000000'")                         ' TESUU_TIME_STAMP_S
        SQL.AppendLine(",' '")                                      ' YOBI1_S
        SQL.AppendLine(",' '")                                      ' YOBI2_S
        SQL.AppendLine(",' '")                                      ' YOBI3_S
        SQL.AppendLine(",' '")                                      ' YOBI4_S
        SQL.AppendLine(",' '")                                      ' YOBI5_S
        SQL.AppendLine(",' '")                                      ' YOBI6_S
        SQL.AppendLine(",' '")                                      ' YOBI7_S
        SQL.AppendLine(",' '")                                      ' YOBI8_S
        SQL.AppendLine(",' '")                                      ' YOBI9_S
        SQL.AppendLine(",' '")                                      ' YOBI10_S
        SQL.AppendLine(")")

        ' 持込SEQの設定
        Dim SEQReader As New CASTCommon.MyOracleReader(OraDB)
        Dim nGetSEQ As Integer
        ' 持込SEQをカウントアップする
        Dim SQL_SEQ As New StringBuilder
        ' 2017/08/23 タスク）綾部 CHG【ME】(標準機能改善(総振マルチの持込ＳＥＱ取得改善)) -------------------- START
        'SQL_SEQ.AppendLine("SELECT")
        'SQL_SEQ.AppendLine(" NVL(MAX(MOTIKOMI_SEQ_S), 0) + 1 SEQ")
        'SQL_SEQ.AppendLine(" FROM S_SCHMAST")
        'SQL_SEQ.AppendLine(" WHERE TORIS_CODE_S  =" & SQ(InfoTori.TORIS_CODE_T))
        'SQL_SEQ.AppendLine(" AND TORIF_CODE_S  =" & SQ(InfoTori.TORIF_CODE_T))
        'SQL_SEQ.AppendLine(" AND FURI_DATE_S  =" & SQ(InfoMeisaiMast.FURIKAE_DATE))
        Select Case InfoTori.MULTI_KBN_T
            Case "1"
                '----------------------------------------
                ' マルチ(複数ﾍｯﾀﾞ/1ﾌｧｲﾙ) の取得SQL
                '----------------------------------------
                SQL_SEQ.Append("SELECT")
                SQL_SEQ.Append("     NVL(MAX(MOTIKOMI_SEQ_S), 0) + 1 SEQ")
                SQL_SEQ.Append(" FROM")
                SQL_SEQ.Append("     S_SCHMAST")
                SQL_SEQ.Append("   , S_TORIMAST")
                SQL_SEQ.Append(" WHERE ")
                SQL_SEQ.Append("     FURI_DATE_S        = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
                SQL_SEQ.Append(" AND TORIS_CODE_S       = TORIS_CODE_T")
                SQL_SEQ.Append(" AND TORIF_CODE_S       = TORIF_CODE_T")
                SQL_SEQ.Append(" AND MULTI_KBN_T        = '1'")
                SQL_SEQ.Append(" AND ITAKU_KANRI_CODE_T = ( SELECT ITAKU_KANRI_CODE_T")
                SQL_SEQ.Append("                             FROM  S_TORIMAST")
                SQL_SEQ.Append("                             WHERE")
                SQL_SEQ.Append("                                   TORIS_CODE_T = " & SQ(InfoTori.TORIS_CODE_T))
                SQL_SEQ.Append("                               AND TORIF_CODE_T = " & SQ(InfoTori.TORIF_CODE_T))
                SQL_SEQ.Append("                          )")
            Case Else
                '----------------------------------------
                ' シングル(1ﾍｯﾀﾞ/1ﾌｧｲﾙ) の取得SQL
                '----------------------------------------
                SQL_SEQ.Append("SELECT")
                SQL_SEQ.Append("     NVL(MAX(MOTIKOMI_SEQ_S), 0) + 1 SEQ")
                SQL_SEQ.Append(" FROM")
                SQL_SEQ.Append("     S_SCHMAST")
                SQL_SEQ.Append(" WHERE ")
                SQL_SEQ.Append("     TORIS_CODE_S =" & SQ(InfoTori.TORIS_CODE_T))
                SQL_SEQ.Append(" AND TORIF_CODE_S =" & SQ(InfoTori.TORIF_CODE_T))
                SQL_SEQ.Append(" AND FURI_DATE_S  =" & SQ(InfoMeisaiMast.FURIKAE_DATE))
        End Select
        ' 2017/08/23 タスク）綾部 CHG【ME】(標準機能改善(総振マルチの持込ＳＥＱ取得改善)) -------------------- END

        Dim nRet As Long = 0
        Do Until nRet = 1
            If InfoMeisaiMast.FILE_SEQ = 0 AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
                ' 持込SEQカウントアップSQL実行
                If SEQReader.DataReader(SQL_SEQ) = True Then
                    nGetSEQ = SEQReader.GetValueInt(0)
                Else
                    WriteBLog("スケジュール作成", "失敗", OraDB.Message & ":" & SQL_SEQ.ToString)
                    Return False
                End If
                SEQReader.Close()
            Else
                nGetSEQ = InfoMeisaiMast.MOTIKOMI_SEQ
            End If

            ' INSERT
            nRet = OraDB.ExecuteNonQuery(String.Format(SQL.ToString, nGetSEQ), True)

            If nRet <= 0 Then
                If OraDB.Code <> 1 OrElse InfoMeisaiMast.MOTIKOMI_SEQ <> 0 Then
                    WriteBLog("スケジュール作成", "失敗", OraDB.Message & ":" & nRet.ToString)
                    Return False
                End If

                ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
            Else
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Dim ReturnMessage As String = String.Empty
                    Dim SubMastInsert_Ret As Integer = 0
                    Call CAstExternal.ModExternal.Ex_InsertSchmastSub(InfoTori.FSYORI_KBN_T,
                                                                      InfoTori.TORIS_CODE_T,
                                                                      InfoTori.TORIF_CODE_T,
                                                                      InfoMeisaiMast.FURIKAE_DATE,
                                                                      nGetSEQ,
                                                                      ReturnMessage,
                                                                      OraDB)
                End If
                ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END

            End If
        Loop
        InfoMeisaiMast.MOTIKOMI_SEQ = nGetSEQ

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        If Not BLOG Is Nothing Then
            BLOG.Write_Exit1(sw, "ClsFormat.InsertSCHMAST")
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Return True

    End Function

    ' 機能　 ： スケジュールマスタ UPDATE
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Public Overridable Function UpdateSCHMAST() As Boolean
        Dim SQL As New StringBuilder(128)                        ' ＳＱＬ

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If Not BLOG Is Nothing Then
            sw = BLOG.Write_Enter1("ClsFormat.UpdateSCHMAST")
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Dim InfoTori As CAstBatch.CommData.stTORIMAST   ' 取引先情報
        InfoTori = mInfoComm.INFOToriMast
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = mInfoComm.INFOParameter

        '2010/10/04.Sakon　テストモード判定追加
        Dim TestMode As String

        ' 更新条件の持ち込みＳＥＱ
        Dim WhereMotikomiSEQ As Integer = -1
        If InfoTori.CYCLE_T = "1" Then
            ' 複数回持込ありの場合
            WhereMotikomiSEQ = InfoMeisaiMast.MOTIKOMI_SEQ
        End If

        ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
        Dim Schmast_Updateflg As String = ""
        Dim UkeTimeStamp_UpdateFlg As String = ""
        Dim Update_MotikomiSeq As Integer = 0
        ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END

        '2010/10/04.Sakon　テストモード判定追加 +++++++++++++++++++++++++
        Try
            'INIファイルよりテストモード判定を行う
            TestMode = CASTCommon.GetFSKJIni("COMMON", "TESTMODE")
            If TestMode = "err" Then
                TestMode = "0"
            End If

            '取引先副コードが99の場合のみテストモードとする
            If TestMode = "1" And InfoTori.TORIF_CODE_T <> "99" Then
                TestMode = "0"
            End If
        Catch ex As Exception
            TestMode = "0"
        End Try
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        Try
            InfoMeisaiMast.SCH_UPDATE_FLG = False
            ' 期日管理ありの場合

            '2017/04/27 saitou RSV2 UPD 持込ＳＥＱ不具合対応 ---------------------------------------- START
            If mInfoComm.INFOParameter.FMT_KBN <> "TO" Then
                'センター直接持込以外は既存の処理
                If InfoPara.RENKEI_KBN <> "88" AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
                    ' 強制以外， 複数回持ち込み
                    If InfoMeisaiMast.FILE_SEQ = 1 Then
                        '初めの1回だけ調べればよい（マルチのとき）
                        '持込ＳＥＱの取得
                        SQL = New StringBuilder(1024)
                        SQL.Append("SELECT")
                        SQL.Append(" NVL(MAX(MOTIKOMI_SEQ_S),0)+1 MOTIKOMI_MAX")
                        If InfoTori.FSYORI_KBN_T = "1" Then
                            SQL.Append(" FROM SCHMAST")
                        Else
                            SQL.Append(" FROM S_SCHMAST")
                        End If
                        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                        ' 決済のためコメント化
                        'SQL.Append("   AND BAITAI_CODE_S= " & SQ(InfoTori.BAITAI_CODE_T))

                        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
                        If OraReader.DataReader(SQL) = False Then
                            WriteBLog("持ち込みＳＥＱ取得", "失敗")
                            Return False
                        End If

                        ' 明細マスタ項目設定 持込ＳＥＱ
                        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetValueInt(0)
                        InfoMeisaiMast.SCH_UPDATE_FLG = True
                        WriteBLog("持ち込みＳＥＱ取得", "成功", "持ち込みＳＥＱ：" & OraReader.GetValueInt(0).ToString)

                        OraReader.Close()
                        OraReader = Nothing
                    End If
                End If
            Else
                'センター直接持込は毎回持込ＳＥＱの取得を行う
                If InfoPara.RENKEI_KBN <> "88" Then
                    ' 強制以外， 複数回持ち込み
                    '持込ＳＥＱの取得
                    SQL = New StringBuilder(1024)
                    SQL.Append("SELECT")
                    SQL.Append(" NVL(MAX(MOTIKOMI_SEQ_S),0)+1 MOTIKOMI_MAX")
                    If InfoTori.FSYORI_KBN_T = "1" Then
                        SQL.Append(" FROM SCHMAST")
                    Else
                        SQL.Append(" FROM S_SCHMAST")
                    End If
                    SQL.Append(" WHERE FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
                    ' 決済のためコメント化
                    'SQL.Append("   AND BAITAI_CODE_S= " & SQ(InfoTori.BAITAI_CODE_T))

                    Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
                    If OraReader.DataReader(SQL) = False Then
                        WriteBLog("持ち込みＳＥＱ取得", "失敗")
                        Return False
                    End If

                    ' 明細マスタ項目設定 持込ＳＥＱ
                    InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetValueInt(0)
                    InfoMeisaiMast.SCH_UPDATE_FLG = True
                    WriteBLog("持ち込みＳＥＱ取得", "成功", "持ち込みＳＥＱ：" & OraReader.GetValueInt(0).ToString)

                    OraReader.Close()
                    OraReader = Nothing
                End If
            End If

            'If InfoPara.RENKEI_KBN <> "88" AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
            '    ' 強制以外， 複数回持ち込み
            '    If InfoMeisaiMast.FILE_SEQ = 1 Then
            '        '初めの1回だけ調べればよい（マルチのとき）
            '        '持込ＳＥＱの取得
            '        SQL = New StringBuilder(1024)
            '        SQL.Append("SELECT")
            '        SQL.Append(" NVL(MAX(MOTIKOMI_SEQ_S),0)+1 MOTIKOMI_MAX")
            '        If InfoTori.FSYORI_KBN_T = "1" Then
            '            SQL.Append(" FROM SCHMAST")
            '        Else
            '            SQL.Append(" FROM S_SCHMAST")
            '        End If
            '        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            '        ' 決済のためコメント化
            '        'SQL.Append("   AND BAITAI_CODE_S= " & SQ(InfoTori.BAITAI_CODE_T))

            '        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
            '        If OraReader.DataReader(SQL) = False Then
            '            WriteBLog("持ち込みＳＥＱ取得", "失敗")
            '            Return False
            '        End If

            '        ' 明細マスタ項目設定 持込ＳＥＱ
            '        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetValueInt(0)
            '        InfoMeisaiMast.SCH_UPDATE_FLG = True
            '        WriteBLog("持ち込みＳＥＱ取得", "成功", "持ち込みＳＥＱ：" & OraReader.GetValueInt(0).ToString)

            '        OraReader.Close()
            '        OraReader = Nothing
            '    End If
            'End If
            '2017/04/27 saitou RSV2 UPD ------------------------------------------------------------- END

            SQL = New StringBuilder(1024)
            SQL.Append("UPDATE")

            If InfoTori.FSYORI_KBN_T = "1" Then
                SQL.Append(" SCHMAST")
            Else
                SQL.Append(" S_SCHMAST")
            End If

            SQL.Append(" SET ")
            SQL.Append(" SYORI_KEN_S = " & InfoMeisaiMast.TOTAL_IRAI_KEN.ToString)
            SQL.Append(",SYORI_KIN_S = " & InfoMeisaiMast.TOTAL_IRAI_KIN.ToString)
            SQL.Append(",ERR_KEN_S = " & InfoMeisaiMast.TOTAL_IJO_KEN.ToString)
            SQL.Append(",ERR_KIN_S = " & InfoMeisaiMast.TOTAL_IJO_KIN.ToString)

            If InfoMeisaiMast.DUPLICATE_KBN = "020" Then
                ' ２重の場合は，受付済にしない
                SQL.Append(",UKETUKE_FLG_S ='0'")       ' 受付済
                SQL.Append(",ERROR_INF_S = '020'")      ' ２重登録
                '2018/10/04 saitou 広島信金(RSV2標準) ADD ---------------------------------------- START
                '二重持込の場合も受付日時は更新する。
                UkeTimeStamp_UpdateFlg = "1"
                '2018/10/04 saitou 広島信金(RSV2標準) ADD ---------------------------------------- END
            Else
                SQL.Append(",ERROR_INF_S = ' '")        ' エラークリア
                SQL.Append(",UKETUKE_FLG_S ='1'")       ' 受付済
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
                UkeTimeStamp_UpdateFlg = "1"
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END
                '2018/03/16 saitou 広島信金(RSV2標準) ADD 照合機能追加 ---------------------------------------- START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    '依頼書、伝票以外は受付日も更新
                    If InfoTori.BAITAI_CODE_T <> "04" AndAlso InfoTori.BAITAI_CODE_T <> "09" Then
                        SQL.Append(",UKETUKE_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    End If
                End If
                '2018/03/16 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------- END
            End If

            If InfoMeisaiMast.TOTAL_IJO_KEN > 0 Then
                '2009/09/29.Sakon　ERROR_INF2_Sを取り除く ++++++++++++++++++++++++
                '                  (総振の場合の考慮必要かも？)
                'SQL.Append(",ERROR_INF2_S = 'ERR'")     ' インプットエラーあり
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            End If

            '2017/12/04 タスク）西野 CHG 標準版修正（照合機能追加、金融機関名相違対応）------------------------------------ START
            If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" AndAlso
                CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYOUGOU") = "1" Then
                '照合機能有りの場合
                If InfoTori.SYOUGOU_KBN_T = "1" OrElse InfoMeisaiMast.DUPLICATE_KBN = "020" OrElse
                   (INI_KINKO_SOUI_CHK = "1" AndAlso InfoMeisaiMast.KinTenSoui = True) OrElse
                   (InfoMeisaiMast.TOTAL_IJO_KEN > 0 AndAlso InfoMeisaiMast.DUPLICATE_KBN <> "020") Then
                    '照合要の場合
                    '２重持ち込みの場合、２重持ち込み以外のインプットエラーがある場合または
                    '金融機関名が相違している場合(総振の場合のみ判定している)
                    SQL.Append(",TOUROKU_FLG_S ='0'")       ' 未登録
                Else
                    SQL.Append(",TOUROKU_FLG_S ='1'")       ' 登録済
                    '2018/03/16 saitou 広島信金(RSV2標準) ADD 照合機能追加 ---------------------------------------- START
                    '登録日の更新
                    SQL.Append(",TOUROKU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    '2018/03/16 saitou 広島信金(RSV2標準) ADD ----------------------------------------------------- END
                End If
            Else
                SQL.Append(",TOUROKU_FLG_S ='1'")       ' 登録済
                SQL.Append(",TOUROKU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
            End If
            'SQL.Append(",TOUROKU_FLG_S ='1'")       ' 登録済
            'SQL.Append(",TOUROKU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
            '2017/12/04 タスク）西野 CHG 標準版修正（照合機能追加、金融機関名相違対応）------------------------------------ END

            '2010/10/04.Sakon　テストモード追加（配信もフラグを立てる） +++++++++++++++
            If TestMode = "1" Then
                Select Case InfoTori.FSYORI_KBN_T
                    Case "1"
                        SQL.Append(",HAISIN_FLG_S ='1'")
                        SQL.Append(",HAISIN_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    Case "3"
                        SQL.Append(",HASSIN_FLG_S ='1'")
                        SQL.Append(",HASSIN_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                End Select
            End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            If InfoPara.RENKEI_KBN <> "88" AndAlso (WhereMotikomiSEQ = -1) Then
                ' 複数回持ち込みありの場合
                SQL.Append(",MOTIKOMI_SEQ_S = " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
                Update_MotikomiSeq = InfoMeisaiMast.MOTIKOMI_SEQ
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END
            End If

            SQL.Append(",FILE_SEQ_S = " & InfoMeisaiMast.FILE_SEQ.ToString)

            ' センター直接持ち込みは，配信フラグに１をたてる
            If InfoTori.MOTIKOMI_KBN_T = "1" Then
                'センター直接持込設定を参照する
                If CASTCommon.GetFSKJIni("JIFURI", "CENTER_MOTIKOMI") = "1" Then
                    SQL.Append(", HAISIN_FLG_S = '1'")
                End If
            End If

            '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
            If INI_MOTIKOMI_KIJITU_CHK = "1" OrElse INI_UKETUKE_JIKAN_CHK = "1" Then
                '総振かつ時間外の場合、中断フラグを立てる
                If InfoTori.FSYORI_KBN_T = "3" AndAlso InfoMeisaiMast.TimeOver = False Then
                    SQL.Append(", TYUUDAN_FLG_S = '1'")
                End If
            End If
            '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

            ' SQL 更新条件
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(InfoTori.TORIS_CODE_T))
            SQL.Append("   AND TORIF_CODE_S = " & SQ(InfoTori.TORIF_CODE_T))
            'センター直接持込フォーマット複数振替日対応
            If mInfoComm.INFOParameter.FMT_KBN = "TO" Then
                SQL.Append("   AND FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
            Else
                SQL.Append("   AND FURI_DATE_S  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            End If

            SQL.Append("   AND TOUROKU_FLG_S = '0'")

            ' 期日管理なしの場合
            If WhereMotikomiSEQ <> -1 Then
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & WhereMotikomiSEQ)
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
                Update_MotikomiSeq = WhereMotikomiSEQ
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END
            End If

            SQL.Append(" AND ROWNUM <= 1")

            ' UPDATE 実行
            If OraDB.ExecuteNonQuery(SQL) <= 0 Then
                WriteBLog("UpdateSCHMAST", "失敗", OraDB.Message)
                Return False
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
            Else
                Schmast_Updateflg = "1"
                ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END
            End If

            If WhereMotikomiSEQ <> -1 Then
                '+++++++++++++++++++++++++++++++++++++++++++
                ' 期日管理なしの場合
                ' ２重持ち込みエラー時は，同じ持ち込み単位のファイルを全て２重エラーにする
                SQL = New StringBuilder("UPDATE S_SCHMAST SET ", 128)
                SQL.Append(" ERROR_INF_S = ")
                SQL.Append(" (SELECT MAX(ERROR_INF_S) FROM S_SCHMAST")
                SQL.Append("   WHERE TORIS_CODE_S  = " & SQ(InfoTori.TORIS_CODE_T))
                SQL.Append("     AND FURI_DATE_S   = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQL.Append("     AND MOTIKOMI_SEQ_S= " & WhereMotikomiSEQ)
                SQL.Append(" )")
                SQL.Append(",UKETUKE_FLG_S = ")
                SQL.Append(" (SELECT MIN(UKETUKE_FLG_S) FROM S_SCHMAST")
                SQL.Append("   WHERE TORIS_CODE_S  = " & SQ(InfoTori.TORIS_CODE_T))
                SQL.Append("     AND FURI_DATE_S   = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQL.Append("     AND MOTIKOMI_SEQ_S= " & WhereMotikomiSEQ)
                SQL.Append(" )")
                SQL.Append(" WHERE TORIS_CODE_S   = " & SQ(InfoTori.TORIS_CODE_T))
                SQL.Append("   AND FURI_DATE_S    = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQL.Append("   AND MOTIKOMI_SEQ_S = " & WhereMotikomiSEQ)
                ' UPDATE 実行
                Dim nRet As Integer = OraDB.ExecuteNonQuery(SQL)
                WriteBLog("UpdateSCHMAST", "成功", "重複登録件数：" & nRet.ToString)
                If nRet < 0 Then
                    WriteBLog("UpdateSCHMAST", "失敗", OraDB.Message)
                End If
            End If

            ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
            If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If UkeTimeStamp_UpdateFlg = "1" And
                   Schmast_Updateflg = "1" Then

                    SQL.Length = 0
                    Select Case InfoTori.FSYORI_KBN_T
                        Case "1"
                            SQL.Append("UPDATE   SCHMAST_SUB SET ")
                        Case Else
                            SQL.Append("UPDATE S_SCHMAST_SUB SET ")
                    End Select

                    SQL.Append("      UKETUKE_TIME_STAMP_S  = " & SQ(strUketukeDate & strUketukeTime))

                    SQL.Append(" WHERE ")
                    SQL.Append("      TORIS_CODE_SSUB       = " & SQ(InfoTori.TORIS_CODE_T))
                    SQL.Append("  AND TORIF_CODE_SSUB       = " & SQ(InfoTori.TORIF_CODE_T))

                    If mInfoComm.INFOParameter.FMT_KBN = "TO" Then
                        SQL.Append("  AND FURI_DATE_SSUB    = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
                    Else
                        SQL.Append("  AND FURI_DATE_SSUB    = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                    End If

                    Select Case InfoTori.FSYORI_KBN_T
                        Case "3"
                            SQL.Append("  AND MOTIKOMI_SEQ_SSUB = " & Update_MotikomiSeq)
                    End Select
                    SQL.Append("  AND ROWNUM               <= 1")

                    ' UPDATE 実行
                    Dim nRet As Integer = OraDB.ExecuteNonQuery(SQL)
                    If nRet < 0 Then
                        WriteBLog("UpdateSCHMAST(受付時間更新)", "失敗", OraDB.Message)
                    Else
                        WriteBLog("UpdateSCHMAST(受付時間更新)", "成功", "更新日時:" & strUketukeDate & strUketukeTime & "/結果:" & nRet.ToString)
                    End If
                End If
            End If
            ' 2016/10/17 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END

        Catch ex As Exception
            WriteBLog("UpdateSCHMAST", "失敗", ex.Message)
            Return False

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Finally
            If Not BLOG Is Nothing Then
                BLOG.Write_Exit1(sw, "ClsFormat.UpdateSCHMAST")
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        End Try

        Return True
    End Function

    ' 機能　 ： 二重持ち込みチェック
    '
    ' 引数   ： ARG1 - 明細データ（ヘッダ＋明細（ＭＡＸ５件）＋トレーラ）
    '
    ' 備考　 ：
    '
    Public Function CheckDuplicate(ByVal Arr As ArrayList) As Boolean
        Dim Ora1StMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora2ndMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora3rdMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQLBase As StringBuilder
        Dim SQL2Jyu As StringBuilder

        If Arr.Count = 0 Then
            Return False
        End If

        Try
            SQLBase = New StringBuilder("SELECT MOTIKOMI_SEQ_K , RECORD_NO_K,  FURI_DATA_K", 128)
            SQLBase.Append(" FROM S_MEIMAST")
            SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
            SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))

            SQL2Jyu = New StringBuilder(SQLBase.ToString, 128)
            SQL2Jyu.Append("   AND RECORD_NO_K  = 1")
            SQL2Jyu.Append("   AND MOTIKOMI_SEQ_K <> " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
            If Ora1StMeiReader.DataReader(SQL2Jyu) = True Then
                '規定外文字を全て置換して比較する
                If ReplaceString(Ora1StMeiReader.GetItem("FURI_DATA_K"), -1).Equals(ReplaceString(CType(Arr.Item(0), String), -1)) = False Then
                    ' 一致するレコードが無い場合，FALSEを返す
                    Return False
                End If

                ' 最初の１件が一致したので，次もサーチする  ' サイクル番号 でのループ
                Dim nDupli As Integer = 0
                Do Until Ora1StMeiReader.EOF = True
                    SQL2Jyu = New StringBuilder(SQLBase.ToString, 128)
                    SQL2Jyu.Append(" AND MOTIKOMI_SEQ_K = " & Ora1StMeiReader.GetItem("MOTIKOMI_SEQ_K"))
                    SQL2Jyu.Append(" ORDER BY MOTIKOMI_SEQ_K , RECORD_NO_K")
                    If Ora2ndMeiReader.DataReader(SQL2Jyu) = True Then
                        Do Until Ora2ndMeiReader.EOF = True
                            '規定外文字を全て置換して比較する
                            Dim MotoData As String = ReplaceString(Ora2ndMeiReader.GetItem("FURI_DATA_K"), -1)
                            If MotoData.Equals(ReplaceString(CType(Arr.Item(nDupli), String), -1)) = False Then
                                ' 一致しない場合は，次へ
                                Exit Do
                            End If

                            nDupli += 1
                            If (nDupli) = (Arr.Count - 1) Then
                                ' トレーラレコードチェック
                                SQL2Jyu = New StringBuilder(SQLBase.ToString, 128)
                                SQL2Jyu.Append(" AND MOTIKOMI_SEQ_K = " & Ora1StMeiReader.GetItem("MOTIKOMI_SEQ_K"))
                                SQL2Jyu.Append(" AND DATA_KBN_K = " & SQ(TrailerKubun(TrailerKubun.Length - 1)))

                                If Ora3rdMeiReader.DataReader(SQL2Jyu) = True Then
                                    '規定外文字を全て置換して比較する
                                    If ReplaceString(Ora3rdMeiReader.GetItem("FURI_DATA_K"), -1).Equals(ReplaceString(CType(Arr.Item(nDupli), String), -1)) = True Then
                                        ' ２重読み込み決定
                                        InfoMeisaiMast.DUPLICATE_KBN = "020"
                                        Return True
                                    End If
                                End If
                            End If

                            ' 次の明細を検索
                            Ora2ndMeiReader.NextRead()
                        Loop
                    End If

                    ' 次のサイクル番号を検索
                    Ora1StMeiReader.NextRead()
                    '*** 修正 mitsu 2009/07/09 ループ毎にカウンタのリセットを行う ***
                    nDupli = 0
                    '****************************************************************
                Loop
            End If
        Catch ex As Exception
            BLOG.Write("２重チェック処理", "失敗", ex.Message & ":" & ex.StackTrace)
        Finally
            Ora1StMeiReader.Close()
            Ora2ndMeiReader.Close()
            Ora3rdMeiReader.Close()
        End Try

        Return False
    End Function

    ''' <summary>
    ''' 総振複数依頼存在チェック
    ''' </summary>
    ''' <param name="Arr">明細データ</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/12/06 タスク）西野　広島信金(RSV2標準版) added for 大規模構築対応（重複レコードチェック）</remarks>
    Public Function fn_Meisai_FukusuuIrai_Check(ByVal Arr As ArrayList) As Boolean

        Dim Ora1stMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora2ndMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora3rdMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQLBase As StringBuilder

        If Arr.Count = 0 Then
            Return False
        End If

        Try

            SQLBase = New StringBuilder("SELECT * ", 128)
            SQLBase.Append(" FROM S_MEIMAST")
            SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
            SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            SQLBase.Append("   AND MOTIKOMI_SEQ_K <> " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)

            '同一取引先・同一振込指定日がなければ処理しない。
            If Ora1stMeiReader.DataReader(SQLBase) = True Then

                '今回落し込みの全明細行を読み取る。
                SQLBase = New StringBuilder("SELECT * ", 128)
                SQLBase.Append(" FROM S_MEIMAST")
                SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQLBase.Append("   AND MOTIKOMI_SEQ_K = " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
                SQLBase.Append("   AND DATA_KBN_K  = '2'")

                If Ora2ndMeiReader.DataReader(SQLBase) = True Then
                    BLOG.Write("データ読込", "成功", "振込依頼データ二重チェック開始")
                    Do Until Ora2ndMeiReader.EOF = True
                        '同一取引先・同一振込指定日・同一依頼人口座情報（金・店・科目・口番）・同一受取人口座情報（金・店・科目・口番・需要家番号）が他にあれば、
                        'この依頼全体を２重持込の恐れありと判断する。
                        SQLBase = New StringBuilder("SELECT * ", 128)
                        SQLBase.Append(" FROM S_MEIMAST")
                        SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                        SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                        SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                        SQLBase.Append("   AND MOTIKOMI_SEQ_K <> " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
                        SQLBase.Append("   AND ITAKU_KIN_K = " & SQ(Ora2ndMeiReader.GetString("ITAKU_KIN_K")))
                        SQLBase.Append("   AND ITAKU_SIT_K= " & SQ(Ora2ndMeiReader.GetString("ITAKU_SIT_K")))
                        SQLBase.Append("   AND ITAKU_KAMOKU_K= " & SQ(Ora2ndMeiReader.GetString("ITAKU_KAMOKU_K")))
                        SQLBase.Append("   AND ITAKU_KOUZA_K= " & SQ(Ora2ndMeiReader.GetString("ITAKU_KOUZA_K")))
                        SQLBase.Append("   AND KEIYAKU_KIN_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_KIN_K")))
                        SQLBase.Append("   AND KEIYAKU_SIT_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_SIT_K")))
                        If Ora2ndMeiReader.GetString("KEIYAKU_KNAME_K") = "" Then
                            SQLBase.Append("   AND KEIYAKU_KNAME_K= ' '")
                        Else
                            SQLBase.Append("   AND KEIYAKU_KNAME_K= " & SQ(Ora2ndMeiReader.GetItem("KEIYAKU_KNAME_K")))
                        End If
                        SQLBase.Append("   AND KEIYAKU_KAMOKU_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_KAMOKU_K")))
                        SQLBase.Append("   AND KEIYAKU_KOUZA_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_KOUZA_K")))
                        SQLBase.Append("   AND FURIKIN_K= " & Ora2ndMeiReader.GetInt64("FURIKIN_K"))
                        If Ora2ndMeiReader.GetString("JYUYOUKA_NO_K") = "" Then
                            SQLBase.Append("   AND JYUYOUKA_NO_K= ' '")
                        Else
                            SQLBase.Append("   AND JYUYOUKA_NO_K= " & SQ(Ora2ndMeiReader.GetItem("JYUYOUKA_NO_K")))
                        End If
                        If Ora3rdMeiReader.DataReader(SQLBase) = True Then
                            InfoMeisaiMast.DUPLICATE_KBN = "020" '二重持込扱いとする。
                            BLOG.Write("データ読込", "成功", "振込依頼データ二重チェック該当あり")
                            Return True
                        End If
                        Ora2ndMeiReader.NextRead()
                    Loop
                End If
            End If
        Catch ex As Exception
            BLOG.Write("振込依頼データ２重チェック処理", "失敗", ex.Message & ":" & ex.StackTrace)
        Finally
            Ora1stMeiReader.Close()
            Ora2ndMeiReader.Close()
            Ora3rdMeiReader.Close()
        End Try

        BLOG.Write("データ読込", "成功", "振込依頼データ二重チェック該当なし")
        Return False

    End Function

    ''' <summary>
    ''' 受付時間外チェック
    ''' 受付時間内の処理かチェックする
    ''' </summary>
    ''' <param name="FuriDate">振込日</param>
    ''' <returns>True,False</returns>
    ''' <remarks>2017/12/07 タスク）西野　広島信金(RSV2標準版) added for 大規模構築対応（持込期日対応）</remarks>
    Public Function CheckInDatetime(ByVal FuriDate As String) As Boolean
        Dim MSG As String = ""
        Dim strSYORI As String = "受付時間外チェック処理"

        BLOG.Write(strSYORI & "（開始）", "成功", "")

        Try
            Dim after1Date As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, 1, HolidayList)
            Dim strAfter1Date As String = String.Format("{0:yyyyMMdd}", after1Date)
            Dim SyoriDate As String = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)
            Dim SyoriTime As String = String.Format("{0:HHmmss}", CASTCommon.Calendar.Now)

            '振込日が処理日より後の受付は全部だめ
            If FuriDate < SyoriDate Then
                InfoMeisaiMast.TimeOver = False
                MSG = "振込日超"
                Return False
            End If

            '振込日が当日で、受付時間外はだめ
            If IsNumeric(INI_UKETUKE_JIKAN) Then
                If FuriDate = SyoriDate AndAlso SyoriTime >= INI_UKETUKE_JIKAN Then
                    InfoMeisaiMast.TimeOver = False
                    MSG = "受付時間外"
                    Return False
                End If
            End If

            Return True
        Catch ex As Exception
            BLOG.Write(strSYORI & "（終了）", "失敗", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
            BLOG.Write(strSYORI & "（終了）", "成功", MSG)
        End Try
    End Function

    ''' <summary>
    ''' 受付期日チェック
    ''' 処理日が受付開始日内かチェックする
    ''' </summary>
    ''' <param name="FuriDate">振替/振込日</param>
    ''' <returns>True,False</returns>
    ''' <remarks>2017/12/07 タスク）西野　広島信金(RSV2標準版) added for 大規模構築対応（持込期日対応）</remarks>
    Public Function CheckKaisiDate(ByVal FuriDate As String) As Boolean
        Dim MSG As String = ""
        Dim strSYORI As String = "受付期日チェック"

        BLOG.Write(strSYORI & "（開始）", "成功", "")

        Try
            Dim Kaisibi As Integer = 15     '基本は15日前

            'INIファイルの受付開始日が数値の場合、INIファイルの値を使用する
            If IsNumeric(INI_UKETUKE_KIJITU) Then
                Kaisibi = CInt(INI_UKETUKE_KIJITU)
            End If

            Dim beforeDate As Date = CASTCommon.GetEigyobi(ConvertDate(FuriDate), Kaisibi * -1, HolidayList)
            Dim strbeforeDate As String = String.Format("{0:yyyyMMdd}", beforeDate)
            Dim SyoriDate As String = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)

            '開始日前は受け付けない
            If strbeforeDate > SyoriDate Then
                InfoMeisaiMast.TimeOver = False
                MSG = "受付開始日前" & strbeforeDate
                Return False
            End If

            Return True
        Catch ex As Exception
            BLOG.Write(strSYORI & "（終了）", "失敗", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
            BLOG.Write(strSYORI & "（終了）", "成功", MSG)
        End Try
    End Function

    ''' <summary>
    ''' 持込期日チェック
    ''' 処理日がスケジュールマスタの持込期日内かチェックする
    ''' </summary>
    ''' <returns>True,False</returns>
    ''' <remarks>2017/12/07 タスク）西野　広島信金(RSV2標準版) added for 大規模構築対応（持込期日対応）</remarks>
    Public Function CheckMotikomiDate() As Boolean
        Dim MSG As String = ""
        Dim strSYORI As String = "持込期日チェック"

        BLOG.Write(strSYORI & "（開始）", "成功", "")

        Try
            Dim SyoriDate As String = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)

            '開始日前は受け付けない
            If MOTIKOMI_DATE < SyoriDate Then
                InfoMeisaiMast.TimeOver = False
                MSG = "持込期日超"
                Return False
            End If

            Return True
        Catch ex As Exception
            BLOG.Write(strSYORI & "（終了）", "失敗", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
            BLOG.Write(strSYORI & "（終了）", "成功", MSG)
        End Try
    End Function

    ' 機能　 ： 返還予定日を取得する
    '
    ' 戻り値 ： 
    '
    ' 備考　 ：
    '
    Protected Function GetHenkanYDate() As String
        Dim FurikaeDate As Date = ConvertDate(mInfoComm.INFOParameter.FURI_DATE)
        Dim OraInReader As New CASTCommon.MyOracleReader(OraDB)

        Return "00000000"
    End Function

    ' 機能　 ： ログ出力
    '
    ' 引数   ： ARG1 - ジョブ名
    '           ARG2 - 結果
    '
    ' 備考　 ：
    '
    Protected Sub WriteBLog(ByVal aJob As String, ByVal aKekka As String, Optional ByVal aErr As String = "")
        If BLOG Is Nothing Then
            Return
        End If

        BLOG.Write(aJob, aKekka, aErr)

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Function GetEigyobiFmt(ByVal base As Date, ByVal days As Long) As Date
        Return GetEigyobi(base, days, HolidayList)
    End Function

    '
    ' 機能　 ： 文字列から，指定の長さを切り取る
    '
    ' 引数　 ： ARG1 - 文字列
    ' 　　　 　 ARG2 - 長さ
    '           ARG3 - ０：左詰，１：右詰
    '           ARG4 - 埋め文字
    '
    ' 戻り値 ： 切り取った後の残りの文字列
    '
    ' 備考　 ：
    '
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer,
                    Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            If len = 0 Then
                Return ""
            End If

            ' 切り取る文字列
            If align = 0 Then
                ' 左詰
                value = value.PadRight(len, pad)
            Else
                ' 右詰
                value = value.PadLeft(len, pad)
            End If

            ' 切り取る文字列
            Dim bt() As Byte = EncdJ.GetBytes(value)
            Return EncdJ.GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

    '
    ' 機能　 ： 結果レコードを読み込んでチェック 後処理
    '
    ' 備考　 ：
    '
    Protected Friend Sub CheckDataFormatAfterFunou()
        Try
            ' 合計依頼件数，合計依頼金額
            InfoMeisaiMast.TOTAL_KEN += InfoMeisaiMast.FURIKEN
            If InfoMeisaiMast.FURIKEN = 1 Then
                InfoMeisaiMast.TOTAL_KIN += InfoMeisaiMast.FURIKIN
            End If

            '2009.12.05 ０円明細対応 start
            If InfoMeisaiMast.FURIKIN > 0 Then
                InfoMeisaiMast.TOTAL_KEN2 += InfoMeisaiMast.FURIKEN
            End If
            '2009.12.05 ０円明細対応 end

            If InfoMeisaiMast.FURIKETU_CODE = 0 Then
                ' 振替済み件数，振替済み金額 
                InfoMeisaiMast.TOTAL_NORM_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    InfoMeisaiMast.TOTAL_NORM_KIN += InfoMeisaiMast.FURIKIN
                End If

                '2010.03.02 start NHK対応
                If InfoMeisaiMast.FURIKIN > 0 Then
                    InfoMeisaiMast.TOTAL_NORM_KEN2 += InfoMeisaiMast.FURIKEN
                End If
                '2010.03.02 end

            Else
                ' 異常件数，異常金額
                InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
                End If
            End If

            ' レコード区分を保存
            BeforeRecKbn = RecordData.Substring(0, 1)
        Catch ex As Exception

        End Try
    End Sub

    '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
    ' 機能　 ： 返還ヘッダレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overridable Sub GetHenkanHeaderRecord()
        Call CheckRecord1()
    End Sub
    '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

    ' 機能　 ： 返還データレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overridable Sub GetHenkanDataRecord()
        Call CheckDataFormatAfterFunou()
    End Sub

    ' 機能　 ： 返還トレーラレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overridable Sub GetHenkanTrailerRecord()
    End Sub

    ' 機能　 ： 再振ヘッダーレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overridable Sub GetSaifuriHeaderRecord(ByVal SAIFURI_DATE As String)
    End Sub

    ' 機能　 ： 再振データレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overridable Sub GetSaifuriDataRecord()
        Call CheckDataFormatAfterFunou()
    End Sub

    ' 機能　 ： 返還トレーラレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overridable Sub GetSaifuriTrailerRecord(Optional ByVal SyoriKen As Long = 0, Optional ByVal SyoriKin As Long = 0,
                                                       Optional ByVal Write As Boolean = False)
    End Sub
    ' 機能　 ： チェックデジットチェック
    '
    ' 引数   ： ARG1 - 口座番号
    '
    ' 戻り値 ： TRUE-正常，FALSE-失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckDigitCheck() As Boolean
        Dim Omomi() As String = {"379187432",
                            "987473219387432",
                            "987453259587432",
                            "579587432"}

        Dim Value As String
        Dim Kouza As String = InfoMeisaiMast.KEIYAKU_KOUZA.Substring(0, 6)
        Dim Digit As Integer

        For nOmo As Integer = 0 To Omomi.Length - 1
            Digit = 0
            Select Case nOmo
                Case 0
                    ' Ⅰ顧客番号
                    Value = InfoMeisaiMast.KEIYAKU_SIT & Kouza
                Case 1
                    ' Ⅱ口座番号
                    Value = InfoMeisaiMast.KEIYAKU_KIN & InfoMeisaiMast.KEIYAKU_SIT & GetKamoku1(InfoMeisaiMast.KEIYAKU_KAMOKU) & Kouza
                Case 2
                    ' Ⅲ口座番号（その２）
                    Value = InfoMeisaiMast.KEIYAKU_KIN & InfoMeisaiMast.KEIYAKU_SIT & GetKamoku2(InfoMeisaiMast.KEIYAKU_KAMOKU) & Kouza
                Case Else
                    ' Ⅳ顧客番号
                    Value = InfoMeisaiMast.KEIYAKU_SIT & Kouza
            End Select
            For i As Integer = 0 To Value.Length - 1
                Digit += CASTCommon.CAInt32(Value.Substring(i, 1)) * CASTCommon.CAInt32(Omomi(nOmo).Substring(i, 1))
            Next i

            If (10 - (Digit Mod 10)).ToString = InfoMeisaiMast.KEIYAKU_KOUZA.Substring(6) Then
                Return True
            End If
        Next nOmo

        Return False
    End Function

    Private Function GetKamoku1(ByVal kamoku As String) As String
        Select Case kamoku
            Case "1"
                ' 普通預金
                Return "02"
            Case "2"
                ' 当座預金
                Return "01"
            Case "3", "4"
                ' 納税準備金，職員預り金
                Return "02"
        End Select

        Return "00"
    End Function

    Private Function GetKamoku2(ByVal kamoku As String) As String
        Select Case kamoku
            Case "1"
                ' 普通預金
                Return "02"
            Case "2"
                ' 当座預金
                Return "01"
            Case "3"
                ' 納税準備金
                Return "05"
            Case "4"
                ' 職員預り金
                Return "37"
        End Select

        Return "00"
    End Function

    '総給振の場合、振替日→振込日
    Private Function GetFuriDate(ByVal FSYORI_KBN As String) As String
        Try
            Select Case FSYORI_KBN
                Case "3"
                    Return "振込日"
                Case Else
                    Return "振替日"
            End Select
        Catch ex As Exception
            Return "振替日"
        End Try
    End Function

    '
    ' 機能　 ： レコードを読み込んでチェック（スリーエス用）
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ： 2017/01/16 saitou 東春信金(RSV2標準) added for スリーエス対応
    '
    Public Overridable Function CheckKekkaFormatSSS() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1レコード読込
            If GetFileDataFunou() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function

    '*** Str Upd 2015/12/01 SO)荒木 for 登録出口対応 ***
    '
    ' 機能   ： フォーマットを設定する
    ' 引数   ： フォーマット区分
    '
    Public Sub setFmtKubun(ByVal kubun As String)

        FmtKubun = kubun

    End Sub

    '
    ' 機能   ： 落し込み用登録出口メソッドを実行する
    ' 引数   ： 取引先コード配列
    '           振替日
    ' 戻り値 ： True - 正常 ， False - 異常
    '
    Public Function CallTourokuExit(ByVal toriCode As String(), ByVal furiDate As String) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        If BLOG Is Nothing Then
            BLOG = New CASTCommon.BatchLOG("ClsFormat", "CFormat")
        End If

        sw = BLOG.Write_Enter1("ClsFormat.CallTourokuExit", "FmtKubun=" & FmtKubun)

        Dim rtn As Boolean = CallExitMethod("落し込み用登録出口メソッド", toriCode, furiDate)

        BLOG.Write_Exit1(sw, "ClsFormat.CallTourokuExit", "rtn=" & rtn)

        Return rtn

    End Function


    '
    ' 機能   ： 返還用登録出口メソッドを実行する
    ' 引数   ： 取引先コード配列
    '           振替日
    ' 戻り値 ： True - 正常 ， False - 異常
    '
    Public Function CallHenkanExit(ByVal toriCode As String(), ByVal furiDate As String) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        If BLOG Is Nothing Then
            BLOG = New CASTCommon.BatchLOG("ClsFormat", "CFormat")
        End If

        sw = BLOG.Write_Enter1("ClsFormat.CallHenkanExit", "FmtKubun=" & FmtKubun)

        Dim rtn As Boolean = CallExitMethod("返還用登録出口メソッド", toriCode, furiDate)

        BLOG.Write_Exit1(sw, "ClsFormat.CallHenkanExit", "rtn=" & rtn)

        Return rtn

    End Function


    '
    ' 機能   ： 返還用登録出口メソッドを実行する
    ' 引数   ： 出口種別文字列
    '           取引先コード配列
    '           振替日
    ' 戻り値 ： True - 正常 ， False - 異常
    '
    Private Function CallExitMethod(ByVal kindStr As String, ByVal toriCode As String(), ByVal furiDate As String) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        sw = BLOG.Write_Enter1("ClsFormat.CallExitMethod")

        Dim xmlDoc As New ConfigXmlDocument
        Dim node As XmlNode
        Dim mXmlFile As String
        Dim mXmlRoot As XmlElement
        Dim mDllName As String = ""
        Dim mClassName As String = ""
        Dim mDllAsm As Assembly = Nothing
        Dim mClassInstance As Object = Nothing
        Dim methodname As String = ""


        Try
            ' フォーマット区分が設定されていない場合は、NOP
            If FmtKubun = "" Then
                Return True
            End If

            ' iniファイルにXML_FORMAT_FLDが定義されていない場合は、NOP
            Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
            If xmlFolderPath = "err" Or xmlFolderPath = "" Then
                BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", "XML_FORMAT_FLD定義なし")
                Return True
            End If

            'XMLパス作成
            If xmlFolderPath.EndsWith("\") = False Then
                xmlFolderPath &= "\"
            End If
            mXmlFile = "XML_FORMAT_" & FmtKubun & ".xml"

            ' XMLファイルが無い場合は、NOP
            If System.IO.File.Exists(xmlFolderPath & mXmlFile) = False Then
                BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", xmlFolderPath & mXmlFile & "ファイルなし")
                Return True
            End If

            ' XMLフォーマットのrootオブジェクト生成
            xmlDoc.Load(xmlFolderPath & mXmlFile)
            mXmlRoot = xmlDoc.DocumentElement


            ' 登録出口メソッド指定が無い場合は、NOP
            node = mXmlRoot.SelectSingleNode("登録出口/" & kindStr)
            If node Is Nothing OrElse node.InnerText.Trim = "" Then
                BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", "「登録出口/" & kindStr & "」指定なし")
                Return True
            End If

            methodname = node.InnerText.Trim


            ' 個別DLL名
            node = mXmlRoot.SelectSingleNode("共通/個別DLL名")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」タグが定義されていません。")
            End If
            mDllName = node.InnerText.Trim
            ' 個別DLLをロード
            If mDllName <> "" Then
                Try
                    mDllAsm = System.Reflection.Assembly.LoadFrom(mDllName & ".dll")
                Catch ex As Exception
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」タグで指定された" & mDllName & ".dll" &
                                        "が見つかりません。（" & ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End Try
            Else
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
            End If

            ' 個別クラス名
            node = mXmlRoot.SelectSingleNode("共通/個別クラス名")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別クラス名」タグが定義されていません。")
            End If
            mClassName = node.InnerText.Trim

            If mClassName <> "" Then
                ' 個別クラスをインスタンス化
                Try
                    mClassInstance = mDllAsm.CreateInstance(mDllName & "." & mClassName)
                    If mClassInstance Is Nothing Then
                        Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別クラス名」タグで指定されたクラスが" &
                                    mDllName & ".dll" & "にありません。（" & ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                    End If
                Catch ex As Exception
                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別クラス名」タグで指定されたクラスが" &
                                    mDllName & ".dll" & "にありません。（" & ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End Try
            Else
                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/個別DLL名」,「共通/個別クラス名」が定義されていません。")
            End If

            '----------------------------------------------------------------
            ' 登録出口個別メソッド呼出し
            '----------------------------------------------------------------
            BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", kindStr & "呼出し：" & methodname)

            Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
            If methodInfo Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「登録出口/" & kindStr & "」タグで指定された" &
                        methodname & "が見つかりません。（" & ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If

            Dim methodParams() As Object = {toriCode, furiDate, Me}
            If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = False Then
                BLOG.Write_Err("ClsFormat.CallExitMethod", kindStr & "エラー", methodname)
                Return False
            End If

            Return True

        Catch ex As Exception
            BLOG.Write_Err("ClsFormat.CallExitMethod", ex)
            Return False

        Finally
            BLOG.Write_Exit1(sw, "ClsFormat.CallExitMethod")

        End Try

    End Function

    '*** End Upd 2015/12/01 SO)荒木 for 登録出口対応 ***

    ''' <summary>
    ''' 振替日を休日補正した値を返す
    ''' </summary>
    ''' <param name="furiDate">補正元の振替日</param>
    ''' <returns>休日補正後の振替日</returns>
    ''' <remarks>2017/12/12 タスク）西野　広島信金(RSV2標準版) added for 大規模構築対応（振替日休日補正対応）</remarks>
    Public Function HoseiFurikaeDate(ByVal furiDate As String) As String
        Dim HoseiFlag As Boolean = False
        Dim dFuriDate As Date = CASTCommon.ConvertDate(furiDate)

        If mInfoComm Is Nothing OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "" OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T Is Nothing Then

            Return dFuriDate.ToString("yyyyMMdd")
        End If

        '総振の場合、対象外
        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "1" Then
            Return dFuriDate.ToString("yyyyMMdd")
        End If

        If IsEigyobi(dFuriDate, HolidayList) = False Then
            '休業日
            If mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "0" Then
                '翌営業日にスライド
                Return GetEigyobiFmt(dFuriDate, 1).ToString("yyyyMMdd")
            Else
                '前営業日にスライド
                Return GetEigyobiFmt(dFuriDate, -1).ToString("yyyyMMdd")
            End If
        End If

        Return dFuriDate.ToString("yyyyMMdd")

    End Function

    ''' <summary>
    ''' 振替日を休日補正した値を返す(休日リスト再取得)
    ''' </summary>
    ''' <param name="furiDate">補正元の振替日</param>
    ''' <param name="db">DB</param>
    ''' <returns>休日補正後の振替日</returns>
    ''' <remarks>2017/12/12 タスク）西野　広島信金(RSV2標準版) added for 大規模構築対応（振替日休日補正対応）</remarks>
    Public Function HoseiFurikaeDate2(ByVal furiDate As String, ByVal db As CASTCommon.MyOracle) As String
        Dim HoseiFlag As Boolean = False
        Dim dFuriDate As Date = CASTCommon.ConvertDate(furiDate)

        If mInfoComm Is Nothing OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "" OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T Is Nothing Then

            Return dFuriDate.ToString("yyyyMMdd")
        End If

        '休日リスト再取得
        HolidayList.Clear()
        Dim SQL As New StringBuilder("SELECT YASUMI_DATE_Y,YASUMI_NAME_Y FROM YASUMIMAST ORDER BY YASUMI_DATE_Y")
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        If OraReader.DataReader(SQL) = True Then
            Do Until OraReader.EOF = True
                HolidayList.Add(OraReader.GetValue(0).ToString)

                OraReader.NextRead()
            Loop
        End If
        OraReader.Close()

        '総振の場合、対象外
        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "1" Then
            Return dFuriDate.ToString("yyyyMMdd")
        End If

        If IsEigyobi(dFuriDate, HolidayList) = False Then
            '休業日
            If mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "0" Then
                '翌営業日にスライド
                Return GetEigyobiFmt(dFuriDate, 1).ToString("yyyyMMdd")
            Else
                '前営業日にスライド
                Return GetEigyobiFmt(dFuriDate, -1).ToString("yyyyMMdd")
            End If
        End If

        Return dFuriDate.ToString("yyyyMMdd")

    End Function

    ''' <summary>
    ''' 営業日かどうか判定する
    ''' </summary>
    ''' <param name="base">対象の振替日</param>
    ''' <param name="holiday">祝日リスト</param>
    ''' <returns>True:営業日／False:休業日</returns>
    ''' <remarks>2017/12/12 タスク）西野　広島信金(RSV2標準版) added for 大規模構築対応（振替日休日補正対応）</remarks>
    Public Function IsEigyobi(ByVal base As Date, ByRef holiday As System.Collections.ArrayList) As Boolean
        If base.DayOfWeek() = DayOfWeek.Saturday OrElse
           base.DayOfWeek() = DayOfWeek.Sunday Then

            Return False
        Else
            '休日判定
            If holiday.BinarySearch(base.ToString("yyyyMMdd")) >= 0 Then
                Return False
            End If
        End If

        Return True
    End Function

End Class
