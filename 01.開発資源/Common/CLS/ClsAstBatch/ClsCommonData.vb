Option Strict On
Option Explicit On

Imports System

' バッチ処理専用クラス
Public Class CommData


    ' 取引先マスタ 取得用構造体
    Public Structure stTORIMAST
        '取引先マスタから取得
        Dim FSYORI_KBN_T As String              '振替処理区分
        Dim TORIS_CODE_T As String              '取引先主コード 
        Dim TORIF_CODE_T As String              '取引先副コード
        Dim BAITAI_CODE_T As String             '媒体コード
        Dim LABEL_KBN_T As String               'ラベル区分
        Dim CODE_KBN_T As String                'コード区分
        Dim ITAKU_KANRI_CODE_T As String        '代表委託者コード
        Dim FILE_NAME_T As String               'ファイル名
        Dim FMT_KBN_T As String                 'フォーマット区分 
        Dim MULTI_KBN_T As String               'マルチ区分
        Dim NS_KBN_T As String                  '入出金区分
        Dim SYUBETU_T As String                 '種別
        Dim ITAKU_CODE_T As String              '委託者コード
        Dim ITAKU_NNAME_T As String             '委託者名カナ
        Dim TKIN_NO_T As String                 '取扱金融機関コード    
        Dim TSIT_NO_T As String                 '取扱支店コード
        Dim KAMOKU_T As String                  '科目
        Dim KOUZA_T As String                   '口座番号
        Dim MOTIKOMI_KBN_T As String            '持込区分
        Dim SOUSIN_KBN_T As String              '送信区分
        Dim TAKO_KBN_T As String                '他行区分
        Dim JIFURICHK_KBN_T As String           '自振契約作成区分
        Dim TEKIYOU_KBN_T As String             '摘要区分
        Dim KTEKIYOU_T As String                'カナ摘要
        Dim NTEKIYOU_T As String                '漢字摘要
        Dim FURI_CODE_T As String               '振替コード
        Dim KIGYO_CODE_T As String              '企業コード
        Dim SYUMOKU_CODE_T As String            '種目コード
        Dim FUKA_CODE_T As String               '付加コード
        Dim ITAKU_KNAME_T As String             '委託者名漢字
        Dim YUUBIN_T As String                  '郵便番号
        Dim DENWA_T As String                   '電話番号
        Dim FAX As String                       'ＦＡＸ番号
        Dim KOKYAKU_NO_T As String              '顧客番号
        Dim KANREN_KIGYO_CODE_T As String       '関連企業コード
        Dim ITAKU_NJYU_T As String              '委託者住所漢字
        Dim YUUBIN_KNAME_T As String            '郵送先カナ
        Dim YUUBIN_NNAME_T As String            '郵送先漢字
        Dim FURI_KYU_CODE_T As String           '振替休日シフト
        Dim SFURI_FLG_T As String               '再振契約
        Dim SFURI_FCODE_T As String             '再振副コード
        Dim SFURI_DAY_T As String               '再振日
        Dim SFURI_KIJITSU_T As String           '日付区分(再振)
        Dim SFURI_KYU_CODE_T As String          '再振休日シフト
        Dim KEIYAKU_DATE_T As String            '契約日
        Dim KAISI_DATE_T As String              '開始年月日
        Dim SYURYOU_DATE_T As String            '終了年月日
        Dim MOTIKOMI_KIJITSU_T As String        '持込期日
        Dim IRAISYO_YDATE_T As String           '日数／基準日(依頼書)
        Dim IRAISYO_KIJITSU_T As String         '日付区分(依頼書)
        Dim IRAISYO_KYU_CODE_T As String        '依頼書休日シフト
        Dim IRAISYO_KBN_T As String             '依頼書種別
        Dim IRAISYO_SORT_T As String            '依頼書出力順
        Dim TEIGAKU_KBN_T As String             '定額区分
        Dim UMEISAI_KBN_T As String             '受付明細表 出力区分 0:非対象，1:店番ソート，2:非ソート,3:エラー分のみ
        Dim FUNOU_MEISAI_KBN_T As String        '不能結果明細表出力区分
        Dim KEKKA_HENKYAKU_KBN_T As String      '結果返却要否
        Dim KEKKA_MEISAI_KBN_T As String        '結果明細データ作成区分
        Dim FKEKKA_TBL_T As String              '振替結果変換テーブルＩＤＴ
        Dim KESSAI_KBN_T As String              '決済区分
        Dim TORIMATOME_SIT_NO_T As String       'とりまとめ店
        Dim HONBU_KOUZA_T As String             '本部別段口座番号
        Dim KESSAI_DAY_T As String              '日数／基準日(決済)
        Dim KESSAI_KIJITSU As String            '日付区分(決済)
        Dim KESSAI_KYU_CODE_T As String         '決済休日シフト
        Dim TUKEKIN_NO_T As String              '決済金融機関
        Dim TUKESIT_NO_T As String              '決済支店
        Dim TUKEKAMOKU_T As String              '決済科目
        Dim TUKE_KOUZA_T As String              '決済口座番号
        Dim TUKEMEIGI_KNAME_T As String         '決済名義人(カナ)
        Dim BIKOU1_T As String                  '備考１
        Dim BIKOU2_T As String                  '備考２
        Dim TESUUTYO_SIT_T As String            '手数料徴求支店
        Dim TESUUTYO_KAMOKU_T As String         '手数料徴求科目
        Dim TESUUTYO_KOUZA_T As String          '手数料徴求口座番号
        Dim TESUUTYO_KBN_T As String            '手数料徴求区分
        Dim TESUURYO_PATN_T As String           '手数料徴求方法
        Dim TESUUMAT_NO_T As String             '手数料集計周期
        Dim TESUUTYO_DAY_T As Integer           '日数／基準日(手数料徴求)
        Dim TESUUTYO_KIJITSU_T As String        '日付区分(手数料徴求)
        Dim TESUU_KYU_CODE_T As String          '手数料休日シフト
        Dim SEIKYU_KBN_T As String              '手数料請求区分
        Dim KIHTESUU_T As String                '振替手数料単価
        Dim SYOUHI_KBN_T As String              '消費税区分
        Dim SOURYO_T As String                  '送料
        Dim KOTEI_TESUU1_T As String            '固定手数料１
        Dim KOTEI_TESUU2_T As String            '固定手数料２
        Dim TESUUMAT_MONTH_T As String          '集計基準月
        Dim TESUUMAT_ENDDAY_T As String         '集計終了日
        Dim TESUUMAT_KIJYUN_T As String         '集計基準
        Dim TESUUMAT_PATN_T As String           '集計方法
        Dim TESUU_GRP_T As String               '集計企業ＧＲＰ
        Dim TESUU_TABLE_T As String             '振込手数料基準ＩＤ
        Dim TESUU_A1_T As Integer               '基準手数料A1
        Dim TESUU_A2_T As Integer               '基準手数料A2
        Dim TESUU_A3_T As Integer               '基準手数料A3
        Dim TESUU_B1_T As Integer               '基準手数料B1
        Dim TESUU_B2_T As Integer               '基準手数料B2
        Dim TESUU_B3_T As Integer               '基準手数料B3
        Dim TESUU_C1_T As Integer               '基準手数料C1
        Dim TESUU_C2_T As Integer               '基準手数料C2
        Dim TESUU_C3_T As Integer               '基準手数料C3
        Dim ENC_KBN_T As String                 '暗号化処理区分
        Dim ENC_OPT1_T As String                'ＡＥＳ
        Dim ENC_KEY1_T As String                '暗号化キー１
        Dim ENC_KEY2_T As String                '暗号化キー２
        Dim SAKUSEI_DATE_T As String            '作成日
        Dim KOUSIN_DATE_T As String             '更新日
        Dim YOBI1_T As String                   '予備１
        Dim YOBI2_T As String                   '予備２
        Dim YOBI3_T As String                   '予備３
        Dim YOBI4_T As String                   '予備４
        Dim YOBI5_T As String                   '予備５
        Dim YOBI6_T As String                   '予備６
        Dim YOBI7_T As String                   '予備７
        Dim YOBI8_T As String                   '予備８
        Dim YOBI9_T As String                   '予備９
        Dim YOBI10_T As String                  '予備１０

        '2017/12/04 タスク）西野 ADD 標準版修正（照合機能追加）------------------------------------ START
        'TORIMAST_SUB用の項目
        Dim AITE_CNT_CODE_T As String           '伝送相手センターコード
        Dim TOHO_CNT_CODE_T As String           '伝送当方センターコード
        Dim DENSO_FILE_ID_T As String           '伝送ファイルID
        Dim HANSOU_KBN_T As String              '搬送方法
        Dim HANSOU_ROOT1_T As String            '搬送ルート１
        Dim HANSOU_ROOT2_T As String            '搬送ルート２
        Dim HANSOU_ROOT3_T As String            '搬送ルート３
        Dim HENKYAKU_SIT_NO_T As String         '返却支店
        Dim SYOUGOU_KBN_T As String             '照合要否区分
        Dim KEIYAKU_NO_T As String              '契約番号
        Dim MAE_SYORI_T As String               '個別前処理
        Dim ATO_SYORI_T As String               '個別後処理  
        Dim TOKKIJIKOU1_T As String             '特記事項１
        Dim TOKKIJIKOU2_T As String             '特記事項２
        Dim TOKKIJIKOU3_T As String             '特記事項３
        Dim TOKKIJIKOU4_T As String             '特記事項４
        Dim KYUUYO_KBN_T As String              '給与適用区分（総振のみ）
        Dim CUSTOM_NUM01_T As Long              '予備数値０１
        Dim CUSTOM_NUM02_T As Long              '予備数値０２
        Dim CUSTOM_NUM03_T As Long              '予備数値０３
        Dim CUSTOM_NUM04_T As Long              '予備数値０４
        Dim CUSTOM_NUM05_T As Long              '予備数値０５
        Dim CUSTOM_NUM06_T As Long              '予備数値０６
        Dim CUSTOM_NUM07_T As Long              '予備数値０７
        Dim CUSTOM_NUM08_T As Long              '予備数値０８
        Dim CUSTOM_NUM09_T As Long              '予備数値０９
        Dim CUSTOM_NUM10_T As Long              '予備数値１０
        Dim CUSTOM_NUM11_T As Long              '予備数値１１
        Dim CUSTOM_NUM12_T As Long              '予備数値１２
        Dim CUSTOM_NUM13_T As Long              '予備数値１３
        Dim CUSTOM_NUM14_T As Long              '予備数値１４
        Dim CUSTOM_NUM15_T As Long              '予備数値１５
        Dim CUSTOM_NUM16_T As Long              '予備数値１６
        Dim CUSTOM_NUM17_T As Long              '予備数値１７
        Dim CUSTOM_NUM18_T As Long              '予備数値１８
        Dim CUSTOM_NUM19_T As Long              '予備数値１９
        Dim CUSTOM_NUM20_T As Long              '予備数値２０
        Dim CUSTOM_NUM21_T As Long              '予備数値２１
        Dim CUSTOM_NUM22_T As Long              '予備数値２２
        Dim CUSTOM_NUM23_T As Long              '予備数値２３
        Dim CUSTOM_NUM24_T As Long              '予備数値２４
        Dim CUSTOM_NUM25_T As Long              '予備数値２５
        Dim CUSTOM_NUM26_T As Long              '予備数値２６
        Dim CUSTOM_NUM27_T As Long              '予備数値２７
        Dim CUSTOM_NUM28_T As Long              '予備数値２８
        Dim CUSTOM_NUM29_T As Long              '予備数値２９
        Dim CUSTOM_NUM30_T As Long              '予備数値３０
        Dim CUSTOM_NUM31_T As Long              '予備数値３１
        Dim CUSTOM_NUM32_T As Long              '予備数値３２
        Dim CUSTOM_NUM33_T As Long              '予備数値３３
        Dim CUSTOM_NUM34_T As Long              '予備数値３４
        Dim CUSTOM_NUM35_T As Long              '予備数値３５
        Dim CUSTOM_NUM36_T As Long              '予備数値３６
        Dim CUSTOM_NUM37_T As Long              '予備数値３７
        Dim CUSTOM_NUM38_T As Long              '予備数値３８
        Dim CUSTOM_NUM39_T As Long              '予備数値３９
        Dim CUSTOM_NUM40_T As Long              '予備数値４０
        Dim CUSTOM_NUM41_T As Long              '予備数値４１
        Dim CUSTOM_NUM42_T As Long              '予備数値４２
        Dim CUSTOM_NUM43_T As Long              '予備数値４３
        Dim CUSTOM_NUM44_T As Long              '予備数値４４
        Dim CUSTOM_NUM45_T As Long              '予備数値４５
        Dim CUSTOM_NUM46_T As Long              '予備数値４６
        Dim CUSTOM_NUM47_T As Long              '予備数値４７
        Dim CUSTOM_NUM48_T As Long              '予備数値４８
        Dim CUSTOM_NUM49_T As Long              '予備数値４９
        Dim CUSTOM_NUM50_T As Long              '予備数値５０
        Dim CUSTOM_VCR01_T As String            '予備文字０１
        Dim CUSTOM_VCR02_T As String            '予備文字０２
        Dim CUSTOM_VCR03_T As String            '予備文字０３
        Dim CUSTOM_VCR04_T As String            '予備文字０４
        Dim CUSTOM_VCR05_T As String            '予備文字０５
        Dim CUSTOM_VCR06_T As String            '予備文字０６
        Dim CUSTOM_VCR07_T As String            '予備文字０７
        Dim CUSTOM_VCR08_T As String            '予備文字０８
        Dim CUSTOM_VCR09_T As String            '予備文字０９
        Dim CUSTOM_VCR10_T As String            '予備文字１０
        Dim CUSTOM_VCR11_T As String            '予備文字１１
        Dim CUSTOM_VCR12_T As String            '予備文字１２
        Dim CUSTOM_VCR13_T As String            '予備文字１３
        Dim CUSTOM_VCR14_T As String            '予備文字１４
        Dim CUSTOM_VCR15_T As String            '予備文字１５
        Dim CUSTOM_VCR16_T As String            '予備文字１６
        Dim CUSTOM_VCR17_T As String            '予備文字１７
        Dim CUSTOM_VCR18_T As String            '予備文字１８
        Dim CUSTOM_VCR19_T As String            '予備文字１９
        Dim CUSTOM_VCR20_T As String            '予備文字２０
        Dim CUSTOM_VCR21_T As String            '予備文字２１
        Dim CUSTOM_VCR22_T As String            '予備文字２２
        Dim CUSTOM_VCR23_T As String            '予備文字２３
        Dim CUSTOM_VCR24_T As String            '予備文字２４
        Dim CUSTOM_VCR25_T As String            '予備文字２５
        Dim CUSTOM_VCR26_T As String            '予備文字２６
        Dim CUSTOM_VCR27_T As String            '予備文字２７
        Dim CUSTOM_VCR28_T As String            '予備文字２８
        Dim CUSTOM_VCR29_T As String            '予備文字２９
        Dim CUSTOM_VCR30_T As String            '予備文字３０
        Dim CUSTOM_VCR31_T As String            '予備文字３１
        Dim CUSTOM_VCR32_T As String            '予備文字３２
        Dim CUSTOM_VCR33_T As String            '予備文字３３
        Dim CUSTOM_VCR34_T As String            '予備文字３４
        Dim CUSTOM_VCR35_T As String            '予備文字３５
        Dim CUSTOM_VCR36_T As String            '予備文字３６
        Dim CUSTOM_VCR37_T As String            '予備文字３７
        Dim CUSTOM_VCR38_T As String            '予備文字３８
        Dim CUSTOM_VCR39_T As String            '予備文字３９
        Dim CUSTOM_VCR40_T As String            '予備文字４０
        Dim CUSTOM_VCR41_T As String            '予備文字４１
        Dim CUSTOM_VCR42_T As String            '予備文字４２
        Dim CUSTOM_VCR43_T As String            '予備文字４３
        Dim CUSTOM_VCR44_T As String            '予備文字４４
        Dim CUSTOM_VCR45_T As String            '予備文字４５
        Dim CUSTOM_VCR46_T As String            '予備文字４６
        Dim CUSTOM_VCR47_T As String            '予備文字４７
        Dim CUSTOM_VCR48_T As String            '予備文字４８
        Dim CUSTOM_VCR49_T As String            '予備文字４９
        Dim CUSTOM_VCR50_T As String            '予備文字５０
        '2017/12/04 タスク）西野 ADD 標準版修正（照合機能追加）------------------------------------ END

        Dim TUKI_T() As String                  '月別処理フラグ

        '取引マスタに存在しない項目
        Dim TKIN_NNAME_N As String              '取扱金融機関名
        Dim TSIT_NNAME_N As String              '取扱支店名
        Dim TKIN_KNAME_N As String              '取扱金融機関名カナ
        Dim TSIT_KNAME_N As String              '取扱支店名カナ
        Dim TORIMATOME_SIT_NNAME_N As String    'とりまとめ店名
        Dim CYCLE_T As String                   'サイクル管理 0:複数回持込なし，1:複数回持込あり
        Dim KIJITU_KANRI_T As String            '期日管理要否 0:期日管理なし，1:期日管理あり
        Dim TESUUTYO_NO_T As Integer            '手数料徴求日数
        '2017/12/04 タスク）西野 DEL 標準版修正（照合機能追加）------------------------------------ START
        'TORIMAST_SUBで追加になったので、こっちは削除する
        'Dim DENSO_FILE_ID_T As String           '伝送ファイルID
        '2017/12/04 タスク）西野 DEL 標準版修正（照合機能追加）------------------------------------ END

        ReadOnly Property EOF() As Boolean
            Get
                If ITAKU_KNAME_T Is Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
    End Structure

    ' 連携情報用構造体（起動パラメータの値）
    Public Structure stPARAMETER
        Dim FURI_DATE As String             '振替日
        Dim CODE_KBN As String              'コード区分
        Dim FMT_KBN As String               'フォーマット区分
        Dim BAITAI_CODE As String           '媒体コード
        Dim LABEL_KBN As String             'ラベル区分

        Dim RENKEI_FILENAME As String       '連携ファイル名
        Dim ENC_KBN As String               '暗号化区分
        Dim ENC_KEY1 As String              '暗号化キー
        Dim ENC_KEY2 As String              '暗号化IVキー
        Dim ENC_OPT1 As String              'ＡＥＳ
        Dim CYCLENO As String               'サイクル№

        Dim JOBTUUBAN As Integer            'ジョブ通番

        Dim FSYORI_KBN As String            '振替処理区分
        Dim TORIS_CODE As String            '取引先主コード
        Dim TORIF_CODE As String            '取引先副コード

        Dim TIME_STAMP As String            'タイムスタンプ

        Dim MODE1 As String                 '処理モード

        Dim RENKEI_KBN As String            '連携区分

        ' 取引先コード
        Public Property TORI_CODE() As String
            Get
                Return TORIS_CODE & TORIF_CODE
            End Get
            Set(ByVal Value As String)
                If Value.Length >= 11 Then
                    TORIS_CODE = Value.Substring(0, 10) '取引先主コード10桁
                    TORIF_CODE = Value.Substring(10)    '取引先副コード2桁
                End If
            End Set
        End Property
    End Structure

    ' メッセージ
    Public Message As String

    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
    Public Syorikekka_Bikou As String = ""      '処理結果確認表の備考に出力するメッセージ
    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

    Public MainDB As CASTCommon.MyOracle

    ' 取引先マスタ 情報
    Private mInfoTorimast As stTORIMAST
    Public Property INFOToriMast() As stTORIMAST
        Get
            Return mInfoTorimast
        End Get
        Set(ByVal Value As stTORIMAST)
            mInfoTorimast = Value
        End Set
    End Property

    ' パラメータ 情報
    Private mInfoParam As stPARAMETER
    Public Property INFOParameter() As stPARAMETER
        Get
            Return mInfoParam
        End Get
        Set(ByVal Value As stPARAMETER)
            mInfoParam = Value
        End Set
    End Property

    Sub New(ByVal db As CASTCommon.MyOracle)
        MainDB = db
    End Sub

    ' New
    Sub New(ByVal torimast As stTORIMAST, ByVal param As stPARAMETER)
        Message = ""

        ' 取引先マスタ情報
        mInfoTorimast = torimast

        ' 連携情報
        mInfoParam = param
    End Sub

    '
    ' 機能　 ： 取引先マスタ情報 取得
    '
    ' 引数　 ： ARG1 - 取引先主コード
    '           ARG2 - 取引先副コード
    '
    ' 戻り値 ： 取引先マスタ情報
    '
    ' 備考　 ： 
    '
    Public Sub GetTORIMAST(ByVal toris As String, ByVal torif As String)
        '口振 取引先マスタを検索
        Call SelectTORIMAST("1", toris.Trim, torif.Trim)
        If mInfoTorimast.EOF = True Then
            '振込 取引先マスタを検索
            Call SelectTORIMAST("3", toris.Trim, torif.Trim)
        End If

        'Return mInfoTorimast
    End Sub

    '
    ' 機能　 ： 取引先マスタ情報 取得
    '
    ' 引数　 ： ARG1 - 振替処理区分
    '           ARG2 - 取引先主コード
    '           ARG3 - 取引先副コード
    '
    ' 戻り値 ： 取引先マスタ情報
    '
    ' 備考　 ： 
    '
    Public Function SelectTORIMAST(ByVal fsyorikbn As String, ByVal toris As String, ByVal torif As String) As stTORIMAST
        Dim SQL As New System.Text.StringBuilder(2048)
        Dim CloseFlag As Boolean = False

        If MainDB Is Nothing Then
            CloseFlag = True
            MainDB = New CASTCommon.MyOracle
        End If

        Try
            '取引先マスタ取得
            SQL.Append("SELECT")
            SQL.Append(" FSYORI_KBN_T")
            SQL.Append(",TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",ITAKU_KNAME_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",BAITAI_CODE_T")
            SQL.Append(",ITAKU_KANRI_CODE_T")
            SQL.Append(",FMT_KBN_T")
            SQL.Append(",CODE_KBN_T")
            SQL.Append(",SOUSIN_KBN_T")
            SQL.Append(",FILE_NAME_T")
            SQL.Append(",TKIN_NO_T")
            SQL.Append(",TSIT_NO_T")
            SQL.Append(",TORIMATOME_SIT_T")
            SQL.Append(",KAMOKU_T")
            SQL.Append(",KOUZA_T")
            SQL.Append(",MULTI_KBN_T")
            SQL.Append(",ITAKU_CODE_T")
            SQL.Append(",SYUBETU_T")
            SQL.Append(",TENMAST.KIN_NNAME_N TKIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N TSIT_NNAME_N")
            SQL.Append(",TENMAST.KIN_KNAME_N TKIN_KNAME_N")
            SQL.Append(",TENMAST.SIT_KNAME_N TSIT_KNAME_N")
            SQL.Append(",TORI_TENMAST.SIT_NNAME_N TORIMATOME_SIT_NNAME_N")
            SQL.Append(",UMEISAI_KBN_T")
            SQL.Append(",MOTIKOMI_KIJITSU_T")
            SQL.Append(",TESUUTYO_KIJITSU_T")
            SQL.Append(",KESSAI_KYU_CODE_T")
            SQL.Append(",TESUUTYO_DAY_T")
            SQL.Append(",TESUUTYO_KBN_T")
            SQL.Append(",IRAISYO_KIJITSU_T")
            SQL.Append(",IRAISYO_YDATE_T")
            SQL.Append(",IRAISYO_KYU_CODE_T")
            SQL.Append(",TEIGAKU_KBN_T")
            SQL.Append(",KOKYAKU_NO_T")
            For i As Integer = 1 To 12
                SQL.Append(",TUKI" & i & "_T")
            Next
            SQL.Append(" ,ENC_KBN_T ")
            SQL.Append(" ,ENC_KEY1_T")
            SQL.Append(" ,ENC_KEY2_T")
            SQL.Append(" ,ENC_OPT1_T")
            SQL.Append(",TESUU_A1_T")
            SQL.Append(",TESUU_A2_T")
            SQL.Append(",TESUU_A3_T")
            SQL.Append(",TESUU_B1_T")
            SQL.Append(",TESUU_B2_T")
            SQL.Append(",TESUU_B3_T")
            SQL.Append(",TESUU_C1_T")
            SQL.Append(",TESUU_C2_T")
            SQL.Append(",TESUU_C3_T")

            '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（振替日休日補正対応）---------------- START
            SQL.Append(",FURI_KYU_CODE_T")
            '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（振替日休日補正対応）---------------- END
            '2018/01/16 saitou 広島信金(RSV2標準) ADD 契約期間等取得 -------------------- START
            SQL.Append(",LABEL_KBN_T")
            SQL.Append(",KEIYAKU_DATE_T")
            SQL.Append(",KAISI_DATE_T")
            SQL.Append(",SYURYOU_DATE_T")
            '2018/01/16 saitou 広島信金(RSV2標準) ADD ----------------------------------- END

            If fsyorikbn = "1" Then
                ' 口振 取引先マスタ
                SQL.Append(",FURI_CODE_T")          '振替コード
                SQL.Append(",KIGYO_CODE_T")         '企業コード
                SQL.Append(",MOTIKOMI_KBN_T")       '持込区分
                SQL.Append(",TAKO_KBN_T")           '他行区分
                SQL.Append(",TEKIYOU_KBN_T")        '摘要区分
                SQL.Append(",KTEKIYOU_T")           'カナ摘要
                SQL.Append(",NTEKIYOU_T")           '漢字摘要
                SQL.Append(",NS_KBN_T")             '入出金区分
                SQL.Append(",'0' CYCLE_T")          '複数回持込なし
                SQL.Append(",'1' KIJITU_KANRI_T")   '期日管理あり
                SQL.Append(",SFURI_FLG_T")          '再振契約
                SQL.Append(",SFURI_DAY_T")          '再振基準日
                SQL.Append(",SFURI_KIJITSU_T")      '再振区分
                SQL.Append(",SFURI_KYU_CODE_T")     '再振休日コード
                SQL.Append(",FUNOU_MEISAI_KBN_T")   '結果明細データ作成区分
                SQL.Append(",FKEKKA_TBL_T")         '振替結果変換テーブルＩＤＴ
                SQL.Append(",SEIKYU_KBN_T")         '2009/09/12　追加
                SQL.Append(",'00' SYUMOKU_CODE_T")  '種目コード
                SQL.Append(",'000' FUKA_CODE_T")    '付加コード
                SQL.Append(",TESUU_TABLE_ID_T")

                SQL.Append(" FROM ")
                SQL.Append("  TORIMAST")
            Else
                ' 振込 取引先マスタ
                SQL.Append(",FURI_CODE_T")          '振替コード
                SQL.Append(",KIGYO_CODE_T")         '企業コード
                SQL.Append(",'0' MOTIKOMI_KBN_T")   '持込区分
                SQL.Append(",'1' TAKO_KBN_T")       '他行区分
                SQL.Append(",TEKIYOU_KBN_T")        '摘要区分
                SQL.Append(",KTEKIYOU_T")           'カナ摘要
                SQL.Append(",NTEKIYOU_T")           '漢字摘要
                SQL.Append(",NS_KBN_T")             '入出金区分
                SQL.Append(",CYCLE_T")              '複数回持込なし
                SQL.Append(",KIJITU_KANRI_T")       '期日管理あり
                SQL.Append(",'0' SFURI_FLG_T")      '再振フラグ
                SQL.Append(",NULL SFURI_DAY_T")     '再振基準日
                SQL.Append(",NULL SFURI_KIJITSU_T") '再振区分
                SQL.Append(",NULL SFURI_KYU_CODE_T") '再振休日コード
                SQL.Append(",'0' FUNOU_MEISAI_KBN_T") '結果明細データ作成区分
                SQL.Append(",'0' FKEKKA_TBL_T")     '振替結果変換テーブルＩＤＴ
                SQL.Append(",NULL SEIKYU_KBN_T")
                SQL.Append(",SYUMOKU_CODE_T")  '種目コード
                SQL.Append(",FUKA_CODE_T")    '付加コード
                SQL.Append(",TESUU_TABLE_ID_T")

                SQL.Append(" FROM ")
                SQL.Append("  S_TORIMAST")
            End If

            SQL.Append(" ,TENMAST TENMAST")
            SQL.Append(" ,TENMAST TORI_TENMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = '" & fsyorikbn & "'")
            SQL.Append(" AND TORIS_CODE_T = '" & toris & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & torif & "'")
            SQL.Append(" AND TKIN_NO_T = TENMAST.KIN_NO_N(+)")
            SQL.Append(" AND TSIT_NO_T = TENMAST.SIT_NO_N(+)")
            SQL.Append(" AND TKIN_NO_T = TORI_TENMAST.KIN_NO_N(+)")
            SQL.Append(" AND TORIMATOME_SIT_T = TORI_TENMAST.SIT_NO_N(+)")

            Dim SQLReader As New CASTCommon.MyOracleReader(MainDB)
            If SQLReader.DataReader(SQL) = True Then
                mInfoTorimast.FSYORI_KBN_T = SQLReader.GetString("FSYORI_KBN_T")
                mInfoTorimast.TORIS_CODE_T = SQLReader.GetString("TORIS_CODE_T")
                mInfoTorimast.TORIF_CODE_T = SQLReader.GetString("TORIF_CODE_T")
                mInfoTorimast.ITAKU_KNAME_T = SQLReader.GetString("ITAKU_KNAME_T")
                mInfoTorimast.ITAKU_NNAME_T = SQLReader.GetString("ITAKU_NNAME_T")
                mInfoTorimast.FURI_CODE_T = SQLReader.GetString("FURI_CODE_T")
                mInfoTorimast.KIGYO_CODE_T = SQLReader.GetString("KIGYO_CODE_T")
                mInfoTorimast.BAITAI_CODE_T = SQLReader.GetString("BAITAI_CODE_T").PadLeft(2, "0"c)
                mInfoTorimast.ITAKU_KANRI_CODE_T = SQLReader.GetString("ITAKU_KANRI_CODE_T")
                mInfoTorimast.FMT_KBN_T = SQLReader.GetString("FMT_KBN_T").PadLeft(2, "0"c)
                mInfoTorimast.MOTIKOMI_KBN_T = SQLReader.GetString("MOTIKOMI_KBN_T")
                mInfoTorimast.CODE_KBN_T = SQLReader.GetString("CODE_KBN_T")
                mInfoTorimast.SOUSIN_KBN_T = SQLReader.GetString("SOUSIN_KBN_T")
                mInfoTorimast.FILE_NAME_T = SQLReader.GetString("FILE_NAME_T")
                mInfoTorimast.TAKO_KBN_T = SQLReader.GetString("TAKO_KBN_T")
                mInfoTorimast.TKIN_NO_T = SQLReader.GetString("TKIN_NO_T")
                mInfoTorimast.TKIN_NNAME_N = SQLReader.GetString("TKIN_NNAME_N")
                mInfoTorimast.TSIT_NO_T = SQLReader.GetString("TSIT_NO_T")
                mInfoTorimast.TSIT_NNAME_N = SQLReader.GetString("TSIT_NNAME_N")
                mInfoTorimast.TKIN_KNAME_N = SQLReader.GetString("TKIN_KNAME_N")
                mInfoTorimast.TSIT_KNAME_N = SQLReader.GetString("TSIT_KNAME_N")
                mInfoTorimast.TORIMATOME_SIT_NO_T = SQLReader.GetString("TORIMATOME_SIT_T")
                mInfoTorimast.TORIMATOME_SIT_NNAME_N = SQLReader.GetString("TORIMATOME_SIT_NNAME_N")
                mInfoTorimast.KAMOKU_T = SQLReader.GetString("KAMOKU_T")
                mInfoTorimast.KOUZA_T = SQLReader.GetString("KOUZA_T")
                mInfoTorimast.MULTI_KBN_T = SQLReader.GetString("MULTI_KBN_T")
                mInfoTorimast.NS_KBN_T = SQLReader.GetString("NS_KBN_T")
                mInfoTorimast.ITAKU_CODE_T = SQLReader.GetString("ITAKU_CODE_T")
                mInfoTorimast.SYUBETU_T = SQLReader.GetString("SYUBETU_T")
                mInfoTorimast.TEKIYOU_KBN_T = SQLReader.GetString("TEKIYOU_KBN_T")
                mInfoTorimast.KTEKIYOU_T = SQLReader.GetString("KTEKIYOU_T")
                mInfoTorimast.NTEKIYOU_T = SQLReader.GetString("NTEKIYOU_T")
                mInfoTorimast.UMEISAI_KBN_T = SQLReader.GetString("UMEISAI_KBN_T")
                mInfoTorimast.MOTIKOMI_KIJITSU_T = SQLReader.GetString("MOTIKOMI_KIJITSU_T")
                mInfoTorimast.TESUUTYO_KIJITSU_T = SQLReader.GetString("TESUUTYO_KIJITSU_T")
                mInfoTorimast.KESSAI_KYU_CODE_T = SQLReader.GetString("KESSAI_KYU_CODE_T")
                mInfoTorimast.FUNOU_MEISAI_KBN_T = SQLReader.GetString("FUNOU_MEISAI_KBN_T")
                mInfoTorimast.TESUUTYO_DAY_T = SQLReader.GetInt("TESUUTYO_DAY_T")
                mInfoTorimast.TESUUTYO_KBN_T = SQLReader.GetString("TESUUTYO_KBN_T")
                mInfoTorimast.IRAISYO_KIJITSU_T = SQLReader.GetString("IRAISYO_KIJITSU_T")
                mInfoTorimast.IRAISYO_YDATE_T = SQLReader.GetString("IRAISYO_YDATE_T")
                mInfoTorimast.IRAISYO_KYU_CODE_T = SQLReader.GetString("IRAISYO_KYU_CODE_T")
                mInfoTorimast.SFURI_FLG_T = SQLReader.GetString("SFURI_FLG_T")
                mInfoTorimast.SFURI_DAY_T = SQLReader.GetString("SFURI_DAY_T")
                mInfoTorimast.SFURI_KIJITSU_T = SQLReader.GetString("SFURI_KIJITSU_T")
                mInfoTorimast.SFURI_KYU_CODE_T = SQLReader.GetString("SFURI_KYU_CODE_T")
                mInfoTorimast.SEIKYU_KBN_T = SQLReader.GetString("SEIKYU_KBN_T")
                mInfoTorimast.CYCLE_T = SQLReader.GetString("CYCLE_T")
                mInfoTorimast.KIJITU_KANRI_T = SQLReader.GetString("KIJITU_KANRI_T")
                mInfoTorimast.SYUMOKU_CODE_T = SQLReader.GetString("SYUMOKU_CODE_T")
                mInfoTorimast.FUKA_CODE_T = SQLReader.GetString("FUKA_CODE_T")
                mInfoTorimast.TEIGAKU_KBN_T = SQLReader.GetString("TEIGAKU_KBN_T")      '2010/02/08 項目追加
                mInfoTorimast.KOKYAKU_NO_T = SQLReader.GetString("KOKYAKU_NO_T")        '2010/02/11 項目追加

                mInfoTorimast.TUKI_T = New String(11) {}
                For i As Integer = 1 To 12
                    INFOToriMast.TUKI_T(i - 1) = SQLReader.GetString("TUKI" & i & "_T")
                Next i

                mInfoTorimast.ENC_KBN_T = SQLReader.GetString("ENC_KBN_T")
                mInfoTorimast.ENC_KEY1_T = SQLReader.GetString("ENC_KEY1_T")
                mInfoTorimast.ENC_KEY2_T = SQLReader.GetString("ENC_KEY2_T")
                mInfoTorimast.ENC_OPT1_T = SQLReader.GetString("ENC_OPT1_T")
                mInfoTorimast.FKEKKA_TBL_T = SQLReader.GetString("FKEKKA_TBL_T")
                mInfoTorimast.TESUU_A1_T = SQLReader.GetInt("TESUU_A1_T")
                mInfoTorimast.TESUU_A2_T = SQLReader.GetInt("TESUU_A2_T")
                mInfoTorimast.TESUU_A3_T = SQLReader.GetInt("TESUU_A3_T")
                mInfoTorimast.TESUU_B1_T = SQLReader.GetInt("TESUU_B1_T")
                mInfoTorimast.TESUU_B2_T = SQLReader.GetInt("TESUU_B2_T")
                mInfoTorimast.TESUU_B3_T = SQLReader.GetInt("TESUU_B3_T")
                mInfoTorimast.TESUU_C1_T = SQLReader.GetInt("TESUU_C1_T")
                mInfoTorimast.TESUU_C2_T = SQLReader.GetInt("TESUU_C2_T")
                mInfoTorimast.TESUU_C3_T = SQLReader.GetInt("TESUU_C3_T")
                mInfoTorimast.TESUU_TABLE_T = SQLReader.GetString("TESUU_TABLE_ID_T")

                '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（振替日休日補正対応）---------------- START
                mInfoTorimast.FURI_KYU_CODE_T = SQLReader.GetString("FURI_KYU_CODE_T")
                '2017/12/12 タスク）西野 ADD 標準版修正：広島信金対応（振替日休日補正対応）---------------- END
                '2018/01/16 saitou 広島信金(RSV2標準) ADD 契約期間等取得 -------------------- START
                mInfoTorimast.LABEL_KBN_T = SQLReader.GetString("LABEL_KBN_T")
                mInfoTorimast.KEIYAKU_DATE_T = SQLReader.GetString("KEIYAKU_DATE_T")
                mInfoTorimast.KAISI_DATE_T = SQLReader.GetString("KAISI_DATE_T")
                mInfoTorimast.SYURYOU_DATE_T = SQLReader.GetString("SYURYOU_DATE_T")
                '2018/01/16 saitou 広島信金(RSV2標準) ADD ----------------------------------- END
                '2017/12/04 タスク）西野 ADD 標準版修正（照合機能追加）------------------------------------ START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Call SelectTORIMAST_SUB(fsyorikbn, toris, torif)
                End If
                '2017/12/04 タスク）西野 ADD 標準版修正（照合機能追加）------------------------------------ END
            Else
                mInfoTorimast = New stTORIMAST
            End If

            SQLReader.Close()
            SQLReader = Nothing
        Catch ex As Exception
            Message = ex.Message
        Finally
        End Try

        If CloseFlag = True Then
            MainDB.Close()
            MainDB = Nothing
        End If

        Return mInfoTorimast
    End Function

    '2017/12/04 タスク）西野 ADD 標準版修正（照合機能追加）------------------------------------ START
    ''' <summary>
    ''' 取引先マスタ情報(固有情報) 取得
    ''' </summary>
    ''' <param name="fsyorikbn">処理区分</param>
    ''' <param name="toris">取引先主コード</param>
    ''' <param name="torif">取引先副コード</param>
    ''' <remarks></remarks>
    Public Sub SelectTORIMAST_SUB(ByVal fsyorikbn As String, ByVal toris As String, ByVal torif As String)
        Dim SQL As New System.Text.StringBuilder(2048)
        Dim SQLReader As CASTCommon.MyOracleReader = Nothing

        Try
            '取引先マスタ(固有情報)取得
            If fsyorikbn = "1" Then
                SQL.Append("SELECT * FROM TORIMAST_SUB")
            Else
                SQL.Append("SELECT * FROM S_TORIMAST_SUB")
            End If
            SQL.Append(" WHERE FSYORI_KBN_TSUB = '" & fsyorikbn & "'")
            SQL.Append(" AND TORIS_CODE_TSUB = '" & toris & "'")
            SQL.Append(" AND TORIF_CODE_TSUB = '" & torif & "'")

            SQLReader = New CASTCommon.MyOracleReader(MainDB)
            If SQLReader.DataReader(SQL) = True Then
                With mInfoTorimast
                    .AITE_CNT_CODE_T = SQLReader.GetString("AITE_CNT_CODE_T")
                    .TOHO_CNT_CODE_T = SQLReader.GetString("TOHO_CNT_CODE_T")
                    .DENSO_FILE_ID_T = SQLReader.GetString("DENSO_FILE_ID_T")
                    .HANSOU_KBN_T = SQLReader.GetString("HANSOU_KBN_T")
                    .HANSOU_ROOT1_T = SQLReader.GetString("HANSOU_ROOT1_T")
                    .HANSOU_ROOT2_T = SQLReader.GetString("HANSOU_ROOT2_T")
                    .HANSOU_ROOT3_T = SQLReader.GetString("HANSOU_ROOT3_T")
                    .HENKYAKU_SIT_NO_T = SQLReader.GetString("HENKYAKU_SIT_NO_T")
                    .SYOUGOU_KBN_T = SQLReader.GetString("SYOUGOU_KBN_T")
                    .KEIYAKU_NO_T = SQLReader.GetString("KEIYAKU_NO_T")
                    .MAE_SYORI_T = SQLReader.GetString("MAE_SYORI_T")
                    .ATO_SYORI_T = SQLReader.GetString("ATO_SYORI_T")
                    .TOKKIJIKOU1_T = SQLReader.GetString("TOKKIJIKOU1_T")
                    .TOKKIJIKOU2_T = SQLReader.GetString("TOKKIJIKOU2_T")
                    .TOKKIJIKOU3_T = SQLReader.GetString("TOKKIJIKOU3_T")
                    .TOKKIJIKOU4_T = SQLReader.GetString("TOKKIJIKOU4_T")
                    If fsyorikbn = "3" Then
                        .KYUUYO_KBN_T = SQLReader.GetString("KYUUYO_KBN_T")
                    End If
                    .CUSTOM_NUM01_T = SQLReader.GetInt64("CUSTOM_NUM01_T")
                    .CUSTOM_NUM02_T = SQLReader.GetInt64("CUSTOM_NUM02_T")
                    .CUSTOM_NUM03_T = SQLReader.GetInt64("CUSTOM_NUM03_T")
                    .CUSTOM_NUM04_T = SQLReader.GetInt64("CUSTOM_NUM04_T")
                    .CUSTOM_NUM05_T = SQLReader.GetInt64("CUSTOM_NUM05_T")
                    .CUSTOM_NUM06_T = SQLReader.GetInt64("CUSTOM_NUM06_T")
                    .CUSTOM_NUM07_T = SQLReader.GetInt64("CUSTOM_NUM07_T")
                    .CUSTOM_NUM08_T = SQLReader.GetInt64("CUSTOM_NUM08_T")
                    .CUSTOM_NUM09_T = SQLReader.GetInt64("CUSTOM_NUM09_T")
                    .CUSTOM_NUM10_T = SQLReader.GetInt64("CUSTOM_NUM10_T")
                    .CUSTOM_NUM11_T = SQLReader.GetInt64("CUSTOM_NUM11_T")
                    .CUSTOM_NUM12_T = SQLReader.GetInt64("CUSTOM_NUM12_T")
                    .CUSTOM_NUM13_T = SQLReader.GetInt64("CUSTOM_NUM13_T")
                    .CUSTOM_NUM14_T = SQLReader.GetInt64("CUSTOM_NUM14_T")
                    .CUSTOM_NUM15_T = SQLReader.GetInt64("CUSTOM_NUM15_T")
                    .CUSTOM_NUM16_T = SQLReader.GetInt64("CUSTOM_NUM16_T")
                    .CUSTOM_NUM17_T = SQLReader.GetInt64("CUSTOM_NUM17_T")
                    .CUSTOM_NUM18_T = SQLReader.GetInt64("CUSTOM_NUM18_T")
                    .CUSTOM_NUM19_T = SQLReader.GetInt64("CUSTOM_NUM19_T")
                    .CUSTOM_NUM20_T = SQLReader.GetInt64("CUSTOM_NUM20_T")
                    .CUSTOM_NUM21_T = SQLReader.GetInt64("CUSTOM_NUM21_T")
                    .CUSTOM_NUM22_T = SQLReader.GetInt64("CUSTOM_NUM22_T")
                    .CUSTOM_NUM23_T = SQLReader.GetInt64("CUSTOM_NUM23_T")
                    .CUSTOM_NUM24_T = SQLReader.GetInt64("CUSTOM_NUM24_T")
                    .CUSTOM_NUM25_T = SQLReader.GetInt64("CUSTOM_NUM25_T")
                    .CUSTOM_NUM26_T = SQLReader.GetInt64("CUSTOM_NUM26_T")
                    .CUSTOM_NUM27_T = SQLReader.GetInt64("CUSTOM_NUM27_T")
                    .CUSTOM_NUM28_T = SQLReader.GetInt64("CUSTOM_NUM28_T")
                    .CUSTOM_NUM29_T = SQLReader.GetInt64("CUSTOM_NUM29_T")
                    .CUSTOM_NUM30_T = SQLReader.GetInt64("CUSTOM_NUM30_T")
                    .CUSTOM_NUM31_T = SQLReader.GetInt64("CUSTOM_NUM31_T")
                    .CUSTOM_NUM32_T = SQLReader.GetInt64("CUSTOM_NUM32_T")
                    .CUSTOM_NUM33_T = SQLReader.GetInt64("CUSTOM_NUM33_T")
                    .CUSTOM_NUM34_T = SQLReader.GetInt64("CUSTOM_NUM34_T")
                    .CUSTOM_NUM35_T = SQLReader.GetInt64("CUSTOM_NUM35_T")
                    .CUSTOM_NUM36_T = SQLReader.GetInt64("CUSTOM_NUM36_T")
                    .CUSTOM_NUM37_T = SQLReader.GetInt64("CUSTOM_NUM37_T")
                    .CUSTOM_NUM38_T = SQLReader.GetInt64("CUSTOM_NUM38_T")
                    .CUSTOM_NUM39_T = SQLReader.GetInt64("CUSTOM_NUM39_T")
                    .CUSTOM_NUM40_T = SQLReader.GetInt64("CUSTOM_NUM40_T")
                    .CUSTOM_NUM41_T = SQLReader.GetInt64("CUSTOM_NUM41_T")
                    .CUSTOM_NUM42_T = SQLReader.GetInt64("CUSTOM_NUM42_T")
                    .CUSTOM_NUM43_T = SQLReader.GetInt64("CUSTOM_NUM43_T")
                    .CUSTOM_NUM44_T = SQLReader.GetInt64("CUSTOM_NUM44_T")
                    .CUSTOM_NUM45_T = SQLReader.GetInt64("CUSTOM_NUM45_T")
                    .CUSTOM_NUM46_T = SQLReader.GetInt64("CUSTOM_NUM46_T")
                    .CUSTOM_NUM47_T = SQLReader.GetInt64("CUSTOM_NUM47_T")
                    .CUSTOM_NUM48_T = SQLReader.GetInt64("CUSTOM_NUM48_T")
                    .CUSTOM_NUM49_T = SQLReader.GetInt64("CUSTOM_NUM49_T")
                    .CUSTOM_NUM50_T = SQLReader.GetInt64("CUSTOM_NUM50_T")
                    .CUSTOM_VCR01_T = SQLReader.GetString("CUSTOM_VCR01_T")
                    .CUSTOM_VCR02_T = SQLReader.GetString("CUSTOM_VCR02_T")
                    .CUSTOM_VCR03_T = SQLReader.GetString("CUSTOM_VCR03_T")
                    .CUSTOM_VCR04_T = SQLReader.GetString("CUSTOM_VCR04_T")
                    .CUSTOM_VCR05_T = SQLReader.GetString("CUSTOM_VCR05_T")
                    .CUSTOM_VCR06_T = SQLReader.GetString("CUSTOM_VCR06_T")
                    .CUSTOM_VCR07_T = SQLReader.GetString("CUSTOM_VCR07_T")
                    .CUSTOM_VCR08_T = SQLReader.GetString("CUSTOM_VCR08_T")
                    .CUSTOM_VCR09_T = SQLReader.GetString("CUSTOM_VCR09_T")
                    .CUSTOM_VCR10_T = SQLReader.GetString("CUSTOM_VCR10_T")
                    .CUSTOM_VCR11_T = SQLReader.GetString("CUSTOM_VCR11_T")
                    .CUSTOM_VCR12_T = SQLReader.GetString("CUSTOM_VCR12_T")
                    .CUSTOM_VCR13_T = SQLReader.GetString("CUSTOM_VCR13_T")
                    .CUSTOM_VCR14_T = SQLReader.GetString("CUSTOM_VCR14_T")
                    .CUSTOM_VCR15_T = SQLReader.GetString("CUSTOM_VCR15_T")
                    .CUSTOM_VCR16_T = SQLReader.GetString("CUSTOM_VCR16_T")
                    .CUSTOM_VCR17_T = SQLReader.GetString("CUSTOM_VCR17_T")
                    .CUSTOM_VCR18_T = SQLReader.GetString("CUSTOM_VCR18_T")
                    .CUSTOM_VCR19_T = SQLReader.GetString("CUSTOM_VCR19_T")
                    .CUSTOM_VCR20_T = SQLReader.GetString("CUSTOM_VCR20_T")
                    .CUSTOM_VCR21_T = SQLReader.GetString("CUSTOM_VCR21_T")
                    .CUSTOM_VCR22_T = SQLReader.GetString("CUSTOM_VCR22_T")
                    .CUSTOM_VCR23_T = SQLReader.GetString("CUSTOM_VCR23_T")
                    .CUSTOM_VCR24_T = SQLReader.GetString("CUSTOM_VCR24_T")
                    .CUSTOM_VCR25_T = SQLReader.GetString("CUSTOM_VCR25_T")
                    .CUSTOM_VCR26_T = SQLReader.GetString("CUSTOM_VCR26_T")
                    .CUSTOM_VCR27_T = SQLReader.GetString("CUSTOM_VCR27_T")
                    .CUSTOM_VCR28_T = SQLReader.GetString("CUSTOM_VCR28_T")
                    .CUSTOM_VCR29_T = SQLReader.GetString("CUSTOM_VCR29_T")
                    .CUSTOM_VCR30_T = SQLReader.GetString("CUSTOM_VCR30_T")
                    .CUSTOM_VCR31_T = SQLReader.GetString("CUSTOM_VCR31_T")
                    .CUSTOM_VCR32_T = SQLReader.GetString("CUSTOM_VCR32_T")
                    .CUSTOM_VCR33_T = SQLReader.GetString("CUSTOM_VCR33_T")
                    .CUSTOM_VCR34_T = SQLReader.GetString("CUSTOM_VCR34_T")
                    .CUSTOM_VCR35_T = SQLReader.GetString("CUSTOM_VCR35_T")
                    .CUSTOM_VCR36_T = SQLReader.GetString("CUSTOM_VCR36_T")
                    .CUSTOM_VCR37_T = SQLReader.GetString("CUSTOM_VCR37_T")
                    .CUSTOM_VCR38_T = SQLReader.GetString("CUSTOM_VCR38_T")
                    .CUSTOM_VCR39_T = SQLReader.GetString("CUSTOM_VCR39_T")
                    .CUSTOM_VCR40_T = SQLReader.GetString("CUSTOM_VCR40_T")
                    .CUSTOM_VCR41_T = SQLReader.GetString("CUSTOM_VCR41_T")
                    .CUSTOM_VCR42_T = SQLReader.GetString("CUSTOM_VCR42_T")
                    .CUSTOM_VCR43_T = SQLReader.GetString("CUSTOM_VCR43_T")
                    .CUSTOM_VCR44_T = SQLReader.GetString("CUSTOM_VCR44_T")
                    .CUSTOM_VCR45_T = SQLReader.GetString("CUSTOM_VCR45_T")
                    .CUSTOM_VCR46_T = SQLReader.GetString("CUSTOM_VCR46_T")
                    .CUSTOM_VCR47_T = SQLReader.GetString("CUSTOM_VCR47_T")
                    .CUSTOM_VCR48_T = SQLReader.GetString("CUSTOM_VCR48_T")
                    .CUSTOM_VCR49_T = SQLReader.GetString("CUSTOM_VCR49_T")
                    .CUSTOM_VCR50_T = SQLReader.GetString("CUSTOM_VCR50_T")
                End With
            End If
            Return
        Catch ex As Exception
            Message = ex.Message
            Return
        Finally
            If Not SQLReader Is Nothing Then
                SQLReader.Close()
                SQLReader = Nothing
            End If
        End Try

    End Sub
    '2017/12/04 タスク）西野 ADD 標準版修正（照合機能追加）------------------------------------ END

    ' 取引先マスタ情報をクリアする
    Public Sub ClearTORIMAST()
        mInfoTorimast = New stTORIMAST
    End Sub
End Class
