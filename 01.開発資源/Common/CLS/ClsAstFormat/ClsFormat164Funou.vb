Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports CASTCommon.ModPublic

' 自振不能金庫返還ＭＴ 東北・東京・大阪・中国センターフォーマット（標準）クラス
Public Class FUNOU_164_DATA
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 164

    '------------------------------------------------------------
    '東北・東京・大阪・中国センター自振フォーマット定義
    '------------------------------------------------------------

    '----------------
    'データレコード
    '----------------
    Structure FUNOU_164_Record
        Implements CFormat.IFormat

        Public JF1 As String     '持込み金融機関コード
        Public JF2 As String     '返還区分
        Public JF3 As String     '登録区分
        Public JF4 As String     '金融機関コード
        Public JF5 As String     '店舗コード
        Public JF6 As String     'レコード種別
        Public JF7 As String     '科目コード
        Public JF8 As String     '口座番号
        Public JF9 As String     '振替日
        Public JF10 As String    '金額
        Public JF11 As String    '入出金区分
        Public JF12 As String    '企業コード
        Public JF13 As String    '企業シーケンス
        Public JF14 As String    '口座内優先
        Public JF15 As String    '振替コード
        Public JF16 As String    '振替相手科目
        Public JF17 As String    '振替相手口番
        Public JF18 As String    '摘要設定区分
        Public JF19 As String    'カナ摘要
        Public JF20 As String    '漢字摘要
        Public JF21 As String    '需要家番号
        Public JF22 As String    '振替結果コード
        Public JF23 As String    'みなし完了
        Public JF24 As String    '訂正後店番
        Public JF25 As String    '訂正後科目
        Public JF26 As String    '訂正後口番
        Public JF27 As String    '訂正後相手科目
        Public JF28 As String    '訂正後相手口番
        Public JF29 As String    '特定顧客番号
        Public JF30 As String    '予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8, JF9, JF10, JF11, JF12, JF13, JF14, _
                             JF15, JF16, JF17, JF18, JF19, JF20, JF21, _
                             JF22, JF23, JF24, JF25, JF26, JF27, JF28, _
                             JF29, JF30 _
                             })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 1)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 4)
                JF5 = CuttingData(Value, 3)
                JF6 = CuttingData(Value, 1)
                JF7 = CuttingData(Value, 2)
                JF8 = CuttingData(Value, 7)
                JF9 = CuttingData(Value, 6)
                JF10 = CuttingData(Value, 13)
                JF11 = CuttingData(Value, 1)
                JF12 = CuttingData(Value, 5)
                JF13 = CuttingData(Value, 8)
                JF14 = CuttingData(Value, 2)
                JF15 = CuttingData(Value, 3)
                JF16 = CuttingData(Value, 2)
                JF17 = CuttingData(Value, 7)
                JF18 = CuttingData(Value, 1)
                JF19 = CuttingData(Value, 13)
                JF20 = CuttingData(Value, 12)
                JF21 = CuttingData(Value, 24)
                JF22 = CuttingData(Value, 2)
                JF23 = CuttingData(Value, 1)
                JF24 = CuttingData(Value, 3)
                JF25 = CuttingData(Value, 2)
                JF26 = CuttingData(Value, 7)
                JF27 = CuttingData(Value, 2)
                JF28 = CuttingData(Value, 7)
                JF29 = CuttingData(Value, 7)
                JF30 = CuttingData(Value, 13)
            End Set
        End Property
    End Structure
    Public FUNOU_164_DATA As FUNOU_164_Record

    Private KEKKATXT As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "振替結果コード.TXT")

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"D"}

        FtranPfile = "FUNOU_164.P"

        '不能ファイルはEBCDIC
        DataInfo.Encoding = EncodingType.EBCDIC

        HeaderKubun = New String() {""}
        DataKubun = New String() {"D"}
        TrailerKubun = New String() {""}
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

        sRet = CheckRecord2()

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
        FUNOU_164_DATA.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "D"

        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(FUNOU_164_DATA.JF9, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = FUNOU_164_DATA.JF9
        InfoMeisaiMast.KIGYO_CODE = FUNOU_164_DATA.JF12
        InfoMeisaiMast.KIGYO_SEQ = FUNOU_164_DATA.JF13
        InfoMeisaiMast.KEIYAKU_KIN = FUNOU_164_DATA.JF4
        InfoMeisaiMast.KEIYAKU_SIT = FUNOU_164_DATA.JF5
        InfoMeisaiMast.KEIYAKU_KAMOKU = FUNOU_164_DATA.JF7
        InfoMeisaiMast.KEIYAKU_KOUZA = FUNOU_164_DATA.JF8
        InfoMeisaiMast.FURIKIN_MOTO = FUNOU_164_DATA.JF10
        InfoMeisaiMast.FURIKIN = CaDecNormal(FUNOU_164_DATA.JF10)
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- START
        InfoMeisaiMast.NS_KBN = FUNOU_164_DATA.JF11         ' 入出金区分
        '2018/02/01 タスク）西野 ADD 標準版修正：広島信金対応（自振ロギングリエンタ分の不能結果更新機能の追加）-- END
        InfoMeisaiMast.JYUYOKA_NO = FUNOU_164_DATA.JF21
        InfoMeisaiMast.FURIKETU_CENTERCODE = FUNOU_164_DATA.JF22
        ' 2008.04.02 取引先マスタの振替結果変換テーブルＩＤＴ にて判定するため
        '            実際にＵＰＤＡＴＥする手前で，振替結果返還を行う
        'InfoMeisaiMast.FURIKETU_CODE = fn_FUNOU_KEKKA_YOMIKAE_2TO1(CASTCommon.CAInt32(FUNOU_164_DATA.JF16))

        '2009/12/29 みなし完了フラグチェック追加 FJH Append Start -------------------------------------------------->
        InfoMeisaiMast.MINASI = FUNOU_164_DATA.JF23
        '2009/12/29 みなし完了フラグチェック追加 FJH Append Start --------------------------------------------------<

        InfoMeisaiMast.TEISEI_SIT = FUNOU_164_DATA.JF24      ' 訂正用支店コード
        InfoMeisaiMast.TEISEI_KAMOKU = FUNOU_164_DATA.JF25   ' 訂正用科目
        InfoMeisaiMast.TEISEI_KOUZA = FUNOU_164_DATA.JF26    ' 訂正用口番
        InfoMeisaiMast.TEISEI_AKAMOKU = FUNOU_164_DATA.JF27  ' 相手訂正用科目
        InfoMeisaiMast.TEISEI_AKOUZA = FUNOU_164_DATA.JF28   ' 相手訂正用口番

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        '不能ファイルはデータチェックしない ***
        '' データチェック
        'If MyBase.CheckDataRecord() = False Then
        '    Return "IJO"
        'End If
        '****************************************************************

        '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------START
        '返還区分追加
        InfoMeisaiMast.HENKANKBN = FUNOU_164_DATA.JF2           '返還区分
        InfoMeisaiMast.FURI_CODE = FUNOU_164_DATA.JF15          '振替コード
        '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------END
        Return "D"
    End Function

    Public Overrides Function IsDataRecord() As Boolean
        'If RecordData.StartsWith(HeaderKubun(0)) = False Then
        '    ' ヘッダでなければ，明細レコード
            Return True
        'End If

        'Return False
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
