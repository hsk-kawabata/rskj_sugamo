Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports CASTCommon.ModPublic

' 自振不能金庫返還ＭＴ東海センターフォーマット（標準）クラス
Public Class CFormatTokFunou
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 237

    '------------------------------
    '東海センター自振フォーマット定義
    '------------------------------
    '----------------
    'ヘッダーレコード
    '----------------
    Structure TOKAI_FUNOU_Record1
        Implements CFormat.IFormat

        Public JF1 As String        ' 企業識別
        Public JF2 As String        ' データ種別
        Public JF3 As String        ' サイクル
        Public JF4 As String        ' 予備
        Public JF5 As String        ' 企業区分
        Public JF6 As String        ' 金融機関コード
        Public JF7 As String        ' 支店コード
        Public JF8 As String        ' 予備
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8})
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 8)
                JF3 = CuttingData(Value, 2)
                JF4 = CuttingData(Value, 2)
                JF5 = CuttingData(Value, 1)
                JF6 = CuttingData(Value, 4)
                JF7 = CuttingData(Value, 3)
                JF8 = CuttingData(Value, 201)
            End Set
        End Property
    End Structure
    Public TOKAI_FUNOU_DATA1 As TOKAI_FUNOU_Record1

    '----------------
    'データレコード
    '----------------
    Structure TOKAI_FUNOU_Record2
        Implements CFormat.IFormat

        Public JF1 As String        ' 自振指定日
        Public JF2 As String        ' センター一括処理種別
        Public JF3 As String        ' 金融機関コード
        Public JF4 As String        ' 店舗コード
        Public JF5 As String        ' 統一優先コード
        Public JF6 As String        ' 企業コード
        Public JF7 As String        ' 科目
        Public JF8 As String        ' 口番
        Public JF9 As String        ' 口座内優先コード
        Public JF10 As String       ' シーケンスNO
        Public JF11 As String       ' 金額
        Public JF12 As String       ' 振替科目・口番
        Public JF13 As String       ' 登録区分
        Public JF14 As String       ' 結果不能STS
        Public JF15 As String       ' 結果自振明細STS
        Public JF16 As String       ' 不能事由コード
        Public JF17 As String       ' 振替コード
        Public JF18 As String       ' 予備
        Public JF19 As String       ' 電話区分コード
        Public JF20 As String       ' 不能連絡先コード
        Public JF21 As String       ' 担当者コード
        Public JF22 As String       ' 漢字摘要/カナ摘要
        Public JF23 As String       ' 取扱店番
        Public JF24 As String       ' 顧客番号
        Public JF25 As String       ' 氏名
        Public JF26 As String       ' 支払可能残高
        Public JF27 As String       ' 口番訂正用店番
        Public JF28 As String       ' 口番訂正用口番
        Public JF29 As String       ' 相手訂正用科目口番
        Public JF30 As String       ' 当初自振指定日
        Public JF31 As String       ' 需要家番号
        Public JF32 As String       ' 持込管理NO
        Public JF33 As String       ' 持込金融機関コード
        Public JF34 As String       ' 返還区分
        Public JF35 As String       ' 他店券資金化日
        Public JF36 As String       ' センターカット処理コード
        Public JF37 As String       ' 特定顧客番号
        Public JF38 As String       ' 電話番号
        Public JF39 As String       ' 予備
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8, JF9, JF10, JF11, JF12, JF13, JF14, _
                             JF15, JF16, JF17, JF18, JF19, JF20, JF21, _
                             JF22, JF23, JF24, JF25, JF26, JF27, JF28, _
                             JF29, JF30, JF31, JF32, JF33, JF34, JF35, _
                             JF36, JF37, JF38, JF39 _
                             })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 8)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 4)
                JF4 = CuttingData(Value, 3)
                JF5 = CuttingData(Value, 3)
                JF6 = CuttingData(Value, 5)
                JF7 = CuttingData(Value, 2)
                JF8 = CuttingData(Value, 7)
                JF9 = CuttingData(Value, 2)
                JF10 = CuttingData(Value, 8)
                JF11 = CuttingData(Value, 13)
                JF12 = CuttingData(Value, 9)
                JF13 = CuttingData(Value, 1)
                JF14 = CuttingData(Value, 4)
                JF15 = CuttingData(Value, 4)
                JF16 = CuttingData(Value, 2)
                JF17 = CuttingData(Value, 3)
                JF18 = CuttingData(Value, 5)
                JF19 = CuttingData(Value, 1)
                JF20 = CuttingData(Value, 1)
                JF21 = CuttingData(Value, 2)
                JF22 = CuttingData(Value, 13)
                JF23 = CuttingData(Value, 3)
                JF24 = CuttingData(Value, 7)
                JF25 = CuttingData(Value, 13)
                JF26 = CuttingData(Value, 13)
                JF27 = CuttingData(Value, 3)
                JF28 = CuttingData(Value, 9)
                JF29 = CuttingData(Value, 9)
                JF30 = CuttingData(Value, 8)
                JF31 = CuttingData(Value, 24)
                JF32 = CuttingData(Value, 2)
                JF33 = CuttingData(Value, 4)
                JF34 = CuttingData(Value, 1)
                JF35 = CuttingData(Value, 8)
                JF36 = CuttingData(Value, 3)
                JF37 = CuttingData(Value, 7)
                JF38 = CuttingData(Value, 13)
                JF39 = CuttingData(Value, 8)
            End Set
        End Property
    End Structure
    Public TOKAI_FUNOU_DATA2 As TOKAI_FUNOU_Record2

    Private KEKKATXT As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "振替結果コード.TXT")

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"H"}

        FtranPfile = "FUNOU_TOKAI.P"

        '*** 修正 mitsu 2008/07/14 不能ファイルはEBCDIC ***
        DataInfo.Encoding = EncodingType.EBCDIC
        '**************************************************

        HeaderKubun = New String() {"H"}
        DataKubun = New String() {"D"}
        TrailerKubun = New String() {""}
    End Sub

    '
    ' 機能　 ： レコードチェック
    '
    ' 戻り値 ： 不正文字の位置
    '
    ' 備考　 ：
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(RecordLen)

        Select Case RecordData.Substring(0, 1)
            Case "H"        ' ヘッダレコード
        End Select

        Return MyBase.CheckRegularString()
    End Function

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
            Case "H"
                sRet = CheckRecord1()
            Case Else
                sRet = CheckRecord2()
        End Select

        Return sRet
    End Function

    '
    ' 機能　 ： ヘッダレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Public Overrides Function CheckRecord1() As String
        TOKAI_FUNOU_DATA1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = "H"

        Return "H"
    End Function

    '
    ' 機能　 ： データレコードチェック
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー

    ' 備考　 ：
    '
    Protected Overridable Function CheckRecord2() As String
        TOKAI_FUNOU_DATA2.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "D"

        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(TOKAI_FUNOU_DATA2.JF1, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = TOKAI_FUNOU_DATA2.JF1
        InfoMeisaiMast.KIGYO_CODE = TOKAI_FUNOU_DATA2.JF6
        InfoMeisaiMast.KIGYO_SEQ = TOKAI_FUNOU_DATA2.JF10
        InfoMeisaiMast.KEIYAKU_KIN = TOKAI_FUNOU_DATA2.JF3
        InfoMeisaiMast.KEIYAKU_SIT = TOKAI_FUNOU_DATA2.JF4
        InfoMeisaiMast.KEIYAKU_KAMOKU = TOKAI_FUNOU_DATA2.JF7
        InfoMeisaiMast.KEIYAKU_KOUZA = TOKAI_FUNOU_DATA2.JF8
        InfoMeisaiMast.FURIKIN_MOTO = TOKAI_FUNOU_DATA2.JF11
        InfoMeisaiMast.FURIKIN = CaDecNormal(TOKAI_FUNOU_DATA2.JF11)
        InfoMeisaiMast.JYUYOKA_NO = TOKAI_FUNOU_DATA2.JF31
        InfoMeisaiMast.FURIKETU_CENTERCODE = TOKAI_FUNOU_DATA2.JF16
        InfoMeisaiMast.FURI_CODE = TOKAI_FUNOU_DATA2.JF17   '2010/04/28 振替コード取得
        ' 2008.04.02 取引先マスタの振替結果変換テーブルＩＤＴ にて判定するため
        '            実際にＵＰＤＡＴＥする手前で，振替結果返還を行う
        'InfoMeisaiMast.FURIKETU_CODE = fn_FUNOU_KEKKA_YOMIKAE_2TO1(CASTCommon.CAInt32(TOKAI_FUNOU_DATA2.JF16))

        Dim 結果自振明細STS As String
        Dim L_結果自振明細STS As Long
        Dim S_結果自振明細STS As String
        Dim 結果自振明細STS_1 As String
        Dim 結果自振明細STS_2 As String
        Dim 結果自振明細STS_3 As String
        Dim 結果自振明細STS_4 As String

        L_結果自振明細STS = 0
        S_結果自振明細STS = ""

        結果自振明細STS = TOKAI_FUNOU_DATA2.JF15            '16進数表記
        '16進数表記を2進数表記に変換する
        結果自振明細STS_1 = 結果自振明細STS.Substring(3, 1)
        結果自振明細STS_2 = 結果自振明細STS.Substring(2, 1)
        結果自振明細STS_3 = 結果自振明細STS.Substring(1, 1)
        結果自振明細STS_4 = 結果自振明細STS.Substring(0, 1)

        結果自振明細STS_1 = fn_Change_10(結果自振明細STS_1)
        結果自振明細STS_2 = fn_Change_10(結果自振明細STS_2)
        結果自振明細STS_3 = fn_Change_10(結果自振明細STS_3)
        結果自振明細STS_4 = fn_Change_10(結果自振明細STS_4)
        L_結果自振明細STS = CAInt32(結果自振明細STS_1) * 1 _
                        + CAInt32(結果自振明細STS_2) * 16 _
                        + CAInt32(結果自振明細STS_3) * 256 _
                        + CAInt32(結果自振明細STS_4) * 4096 '10進表記
        Do
            S_結果自振明細STS = CStr(L_結果自振明細STS Mod 2) & S_結果自振明細STS
            L_結果自振明細STS = CType((L_結果自振明細STS / 2), Integer)

        Loop While L_結果自振明細STS > 0

        S_結果自振明細STS = CALng(S_結果自振明細STS).ToString("0000000000000000")
        InfoMeisaiMast.MINASI = S_結果自振明細STS.Substring(7, 1)
        ''1998/04/01 みなし完了が "1" の場合振替結果コードを 00 に補正する
        If InfoMeisaiMast.MINASI = "1" Then
            InfoMeisaiMast.FURIKETU_CODE = 0
        End If

        InfoMeisaiMast.TEISEI_SIT = TOKAI_FUNOU_DATA2.JF27
        InfoMeisaiMast.TEISEI_KAMOKU = TOKAI_FUNOU_DATA2.JF28.Substring(0, 2)   ' 訂正用科目
        InfoMeisaiMast.TEISEI_KOUZA = TOKAI_FUNOU_DATA2.JF28.Substring(2, 7)    ' 訂正用口番
        InfoMeisaiMast.TEISEI_AKAMOKU = TOKAI_FUNOU_DATA2.JF29.Substring(0, 2)  ' 相手訂正用科目
        InfoMeisaiMast.TEISEI_AKOUZA = TOKAI_FUNOU_DATA2.JF29.Substring(2, 7)   ' 相手訂正用口番

        '*** 修正 mitsu 2008/11/07 訂正情報の取得 ***
        If InfoMeisaiMast.TEISEI_SIT <> "000" Then
            InfoMeisaiMast.TEISEI_SIT = TOKAI_FUNOU_DATA2.JF4       'JF27,28：読替前情報(依頼)、JF4,7,8：読替後情報
        End If
        If InfoMeisaiMast.TEISEI_KAMOKU <> "00" Then
            InfoMeisaiMast.TEISEI_KAMOKU = TOKAI_FUNOU_DATA2.JF7
        End If
        If InfoMeisaiMast.TEISEI_KOUZA <> "0000000" Then
            InfoMeisaiMast.TEISEI_KOUZA = TOKAI_FUNOU_DATA2.JF8
        End If
        '********************************************

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        '*** 修正 mitsu 2008/07/15 不能ファイルはデータチェックしない ***
        '' データチェック
        'If MyBase.CheckDataRecord() = False Then
        '    Return "IJO"
        'End If
        '****************************************************************

        Return "D"
    End Function

    Public Overrides Function IsDataRecord() As Boolean
        If RecordData.StartsWith(HeaderKubun(0)) = False Then
            ' ヘッダでなければ，明細レコード
            Return True
        End If

        Return False
    End Function

    '============================================================================
    'NAME           :fn_Change_10
    'Parameter      :
    'Description    :16進数を10進数に変換
    'Return         :16進数を10進数に変換した値
    'Create         :2004/08/23
    'Update         :
    '============================================================================
    Private Function fn_Change_10(ByVal In_DATA As String) As String
        Select Case In_DATA
            Case "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
                Return In_DATA
            Case "A", "a"
                Return "10"
            Case "B", "b"
                Return "11"
            Case "C", "c"
                Return "12"
            Case "D", "d"
                Return "13"
            Case "E", "e"
                Return "14"
            Case "F", "f"
                Return "15"
            Case Else
                Return ""
        End Select
    End Function

    ' 機能　 ： 振替結果コード返還
    '
    ' 引数   ： ARG1 - 振替結果変換テーブルＩＤＴ
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり，"IJO" - インプットエラー
    '
    ' 備考　 ：
    '
    Public Function SetFuriKetu(ByVal fKekkaTbl As String) As Integer
        If fKekkaTbl = "0" Then
            KEKKATXT = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "振替結果コード.TXT")
        Else
            KEKKATXT = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "振替結果コード_" & fKekkaTbl & ".TXT")
        End If

        Return fn_FUNOU_KEKKA_YOMIKAE_2TO1(CASTCommon.CAInt32(InfoMeisaiMast.FURIKETU_CENTERCODE))
    End Function

    ' 機能　 ： 結果コード返還
    '
    ' 引数   ： ARG1 - センター振替結果コード
    '
    ' 戻り値 ： 振替結果コード
    '
    ' 備考　 ：
    '
    Private Function fn_FUNOU_KEKKA_YOMIKAE_2TO1(ByVal aFURIKETU_CODE_2 As Integer) As Integer
        Dim sRet As String = CASTCommon.GetIni(KEKKATXT, "KEKKA_CODE", aFURIKETU_CODE_2.ToString.Trim)
        If sRet <> "err" Then
            Return CASTCommon.CAInt32(sRet)
        End If

        sRet = CASTCommon.GetIni(KEKKATXT, "KEKKA_CODE", "ELSE")
        If sRet = "err" Then
            ' エラーの場合，標準の振替結果コード.TXTで再チャレンジ
            Dim sKekka As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "振替結果コード.TXT")
            sRet = CASTCommon.GetIni(sKekka, "KEKKA_CODE", aFURIKETU_CODE_2.ToString.Trim)
            If sRet <> "err" Then
                Return CASTCommon.CAInt32(sRet)
            End If
            sRet = CASTCommon.GetIni(sKekka, "KEKKA_CODE", "ELSE")
            If sRet = "err" Then
                Return CASTCommon.CAInt32(sRet)
            End If

            Return 9
        End If

        Return CASTCommon.CAInt32(sRet)
    End Function
End Class
