Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports CASTCommon.ModPublic

'ＳＫＣ不能フォーマットクラス
Public Class CFormatSKCFunou
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 160

    '--------------------
    'ＳＫＣ自振フォーマット定義
    '--------------------
    '----------------
    'ヘッダーレコード
    '----------------
    Structure SKC_FUNOU_Record1
        Implements CFormat.IFormat

        Public JF1 As String     'データ区分
        Public JF2 As String     '組合コード
        Public JF3 As String     '店舗コード
        Public JF4 As String     'レコード種別
        Public JF5 As String     '持込組合コード
        Public JF6 As String     '返還区分
        Public JF7 As String     '職域信組区分
        Public JF8 As String     '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, JF8})
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 1)
                JF2 = CuttingData(Value, 4)
                JF3 = CuttingData(Value, 3)
                JF4 = CuttingData(Value, 1)
                JF5 = CuttingData(Value, 4)
                JF6 = CuttingData(Value, 1)
                JF7 = CuttingData(Value, 1)
                JF8 = CuttingData(Value, 145)
            End Set
        End Property
    End Structure
    Public SKC_FUNOU_DATA1 As SKC_FUNOU_Record1

    '--------------
    'データレコード
    '--------------
    Structure SKC_FUNOU_Record2
        Implements CFormat.IFormat

        Public JF1 As String     'データ区分
        Public JF2 As String     '組合コード
        Public JF3 As String     '店舗コード
        Public JF4 As String     'レコード種別
        Public JF5 As String     '科目コード
        Public JF6 As String     '口座番号
        Public JF7 As String     '自振指定日
        Public JF8 As String     '金額
        Public JF9 As String     '入出金区分
        Public JF10 As String    '企業コード
        Public JF11 As String    '企業シーケンス
        Public JF12 As String    '口座内優先コード
        Public JF13 As String    '振替コード
        Public JF14 As String    '振替相手科目
        Public JF15 As String    '振替相手口番
        Public JF16 As String    '開始年月
        Public JF17 As String    '終了年月
        Public JF18 As String    '摘要設定区分
        Public JF19 As String    'カナ摘要
        Public JF20 As String    '漢字摘要
        Public JF21 As String    '需要家番号
        Public JF22 As String    '特定顧客番号
        Public JF23 As String    '振替結果
        Public JF24 As String    '職員番号
        Public JF25 As String    '所属コード
        Public JF26 As String    '休日指定コード
        Public JF27 As String    '予備
        Public JF28 As String    '持込ＭＴチェックエラーコード

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8, JF9, JF10, JF11, JF12, JF13, JF14, _
                             JF15, JF16, JF17, JF18, JF19, JF20, JF21, _
                             JF22, JF23, JF24, JF25, JF26, JF27, JF28 _
                             })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 1)
                JF2 = CuttingData(Value, 4)
                JF3 = CuttingData(Value, 3)
                JF4 = CuttingData(Value, 1)
                JF5 = CuttingData(Value, 2)
                JF6 = CuttingData(Value, 7)
                JF7 = CuttingData(Value, 8)
                JF8 = CuttingData(Value, 10)
                JF9 = CuttingData(Value, 1)
                JF10 = CuttingData(Value, 4)
                JF11 = CuttingData(Value, 7)
                JF12 = CuttingData(Value, 2)
                JF13 = CuttingData(Value, 3)
                JF14 = CuttingData(Value, 2)
                JF15 = CuttingData(Value, 7)
                JF16 = CuttingData(Value, 6)
                JF17 = CuttingData(Value, 6)
                JF18 = CuttingData(Value, 1)
                JF19 = CuttingData(Value, 13)
                JF20 = CuttingData(Value, 12)
                JF21 = CuttingData(Value, 24)
                JF22 = CuttingData(Value, 7)
                JF23 = CuttingData(Value, 1)
                JF24 = CuttingData(Value, 11)
                JF25 = CuttingData(Value, 10)
                JF26 = CuttingData(Value, 1)
                JF27 = CuttingData(Value, 4)
                JF28 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public SKC_FUNOU_DATA2 As SKC_FUNOU_Record2

    '--------------
    'トレーラレコード
    '--------------
    Structure SKC_FUNOU_Record8
        Implements CFormat.IFormat

        Public JF1 As String     'データ区分
        Public JF2 As String     '組合コード
        Public JF3 As String     '店舗コード
        Public JF4 As String     'レコード種別
        Public JF5 As String     '合計件数
        Public JF6 As String     '合計金額
        Public JF7 As String     '振替済件数符号
        Public JF8 As String     '振替済件数
        Public JF9 As String     '振替済金額符号
        Public JF10 As String    '振替済金額
        Public JF11 As String    '不能件数
        Public JF12 As String    '不能金額
        Public JF13 As String    '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8, JF9, JF10, JF11, JF12, JF13 _
                             })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 1)
                JF2 = CuttingData(Value, 4)
                JF3 = CuttingData(Value, 3)
                JF4 = CuttingData(Value, 1)
                JF5 = CuttingData(Value, 6)
                JF6 = CuttingData(Value, 12)
                JF7 = CuttingData(Value, 1)
                JF8 = CuttingData(Value, 6)
                JF9 = CuttingData(Value, 1)
                JF10 = CuttingData(Value, 12)
                JF11 = CuttingData(Value, 6)
                JF12 = CuttingData(Value, 12)
                JF13 = CuttingData(Value, 95)
            End Set
        End Property
    End Structure
    Public SKC_FUNOU_DATA8 As SKC_FUNOU_Record8

    '--------------
    'エンドレコード
    '--------------
    Structure SKC_FUNOU_Record9
        Implements CFormat.IFormat

        Public JF1 As String     'データ区分
        Public JF2 As String     '組合コード
        Public JF3 As String     '店舗コード
        Public JF4 As String     'レコード種別
        Public JF5 As String     '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                               {JF1, JF2, JF3, JF4, JF5})
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 1)
                JF2 = CuttingData(Value, 4)
                JF3 = CuttingData(Value, 3)
                JF4 = CuttingData(Value, 1)
                JF5 = CuttingData(Value, 151)
            End Set
        End Property
    End Structure
    Public SKC_FUNOU_DATA9 As SKC_FUNOU_Record9

    Private KEKKATXT As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "振替結果コード.TXT")

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "2", "8", "9"}

        FtranPfile = "FUNOU_SKC.P"

        '不能ファイルはEBCDIC
        DataInfo.Encoding = EncodingType.EBCDIC

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' 機能　 ： レコードを読み込んでチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ：
    '
    Public Overrides Function CheckKekkaFormat() As String
        ' 基本クラス チェック
        Dim sRet As String = MyBase.CheckKekkaFormat()

        If RecordData.Length = 0 Then
            DataInfo.Message = "ファイル異常"
            mnErrorNumber = 1
            Return "ERR"
        End If

        Select Case RecordData.Substring(0, 1)
            Case "1"
                sRet = "H"
            Case "2"
                sRet = CheckRecord2()
            Case "8"
                sRet = "T"
            Case "9"
                sRet = "E"
            Case Else
                DataInfo.Message = "レコード区分異常（" & RecordData.Substring(0, 1) & "）異常"
                mnErrorNumber = 1
                Return "ERR"
        End Select

        Return sRet
    End Function

    '
    ' 機能　 ： データレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Protected Overridable Function CheckRecord2() As String
        SKC_FUNOU_DATA2.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "D"

        InfoMeisaiMast.FURIKAE_DATE = SKC_FUNOU_DATA2.JF7
        InfoMeisaiMast.FURIKAE_DATE_MOTO = SKC_FUNOU_DATA2.JF7
        InfoMeisaiMast.KIGYO_CODE = SKC_FUNOU_DATA2.JF10
        InfoMeisaiMast.KIGYO_SEQ = SKC_FUNOU_DATA2.JF11
        InfoMeisaiMast.KEIYAKU_KIN = SKC_FUNOU_DATA2.JF2
        InfoMeisaiMast.KEIYAKU_SIT = SKC_FUNOU_DATA2.JF3
        InfoMeisaiMast.KEIYAKU_KAMOKU = SKC_FUNOU_DATA2.JF5
        InfoMeisaiMast.KEIYAKU_KOUZA = SKC_FUNOU_DATA2.JF6
        InfoMeisaiMast.FURIKIN_MOTO = SKC_FUNOU_DATA2.JF8
        InfoMeisaiMast.FURIKIN = CaDecNormal(SKC_FUNOU_DATA2.JF8)
        InfoMeisaiMast.JYUYOKA_NO = SKC_FUNOU_DATA2.JF21
        InfoMeisaiMast.FURIKETU_CENTERCODE = SKC_FUNOU_DATA2.JF23
        InfoMeisaiMast.FURIKETU_CODE = CInt(SKC_FUNOU_DATA2.JF23)

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        '不能ファイルはデータチェックしない ***
        '' データチェック
        'If MyBase.CheckDataRecord() = False Then
        '    Return "IJO"
        'End If

        Return "D"
    End Function

End Class
