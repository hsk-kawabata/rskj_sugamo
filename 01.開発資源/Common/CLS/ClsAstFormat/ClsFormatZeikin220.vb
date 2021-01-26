Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' 地公体 データフォーマットクラス
Public Class CFormatZeikin220
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 220

    '------------------------------------------
    '地公体フォーマット
    '------------------------------------------
    'ヘッダーレコード
    Structure ZEIKIN_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String    'データ区分(=1)
        <VBFixedString(2)> Public ZK2 As String    '種別コード
        <VBFixedString(1)> Public ZK3 As String    'コード区分
        <VBFixedString(10)> Public ZK4 As String   '委託者コード
        <VBFixedString(40)> Public ZK5 As String   '委託者名
        <VBFixedString(4)> Public ZK6 As String    '振替日
        <VBFixedString(4)> Public ZK7 As String    '取引銀行番号
        <VBFixedString(15)> Public ZK8 As String   '取引銀行名
        <VBFixedString(3)> Public ZK9 As String    '取引支店番号
        <VBFixedString(15)> Public ZK10 As String  '取引支店名
        <VBFixedString(1)> Public ZK11 As String   '預金種目
        <VBFixedString(7)> Public ZK12 As String   '口座番号
        <VBFixedString(17)> Public ZK13 As String  'ダミー
        <VBFixedString(20)> Public ZK14 As String   '収納種目
        <VBFixedString(80)> Public ZK15 As String  'ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2, ZK3, ZK4, ZK5, ZK6, ZK7, _
                             ZK8, ZK9, ZK10, ZK11, ZK12, ZK13, ZK14, _
                             ZK15})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 2)
                ZK3 = CuttingData(Value, 1)
                ZK4 = CuttingData(Value, 10)
                ZK5 = CuttingData(Value, 40)
                ZK6 = CuttingData(Value, 4)
                ZK7 = CuttingData(Value, 4)
                ZK8 = CuttingData(Value, 15)
                ZK9 = CuttingData(Value, 3)
                ZK10 = CuttingData(Value, 15)
                ZK11 = CuttingData(Value, 1)
                ZK12 = CuttingData(Value, 7)
                ZK13 = CuttingData(Value, 17)
                ZK14 = CuttingData(Value, 20)
                ZK15 = CuttingData(Value, 80)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC1 As ZEIKIN_RECORD1

    'データレコード
    Structure ZEIKIN_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   'データ区分(=2)
        <VBFixedString(4)> Public ZK2 As String   '引落銀行番号
        <VBFixedString(15)> Public ZK3 As String  '引落銀行名
        <VBFixedString(3)> Public ZK4 As String   '引落支店番号
        <VBFixedString(15)> Public ZK5 As String  '引落支店名
        <VBFixedString(1)> Public ZK6 As String   '預金種目
        <VBFixedString(7)> Public ZK7 As String   '口座番号
        <VBFixedString(30)> Public ZK8 As String  '口座名義
        <VBFixedString(10)> Public ZK9 As String  '引落金額
        <VBFixedString(10)> Public ZK10 As String '保険料額(定)
        <VBFixedString(10)> Public ZK11 As String '保険料額(付)
        <VBFixedString(20)> Public ZK12 As String 'ダミー
        <VBFixedString(10)> Public ZK13 As String '前納報奨金
        <VBFixedString(1)> Public ZK14 As String  '新規コード
        <VBFixedString(20)> Public ZK15 As String '顧客番号
        <VBFixedString(30)> Public ZK16 As String '被保険者名
        <VBFixedString(1)> Public ZK17 As String  '振替結果コード
        <VBFixedString(10)> Public ZK18 As String '通帳略字
        <VBFixedString(2)> Public ZK19 As String  '年度
        <VBFixedString(20)> Public ZK20 As String 'ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2, ZK3, ZK4, ZK5, ZK6, ZK7, _
                             ZK8, ZK9, ZK10, ZK11, ZK12, ZK13, ZK14, _
                             ZK15, ZK16, ZK17, ZK18, ZK19, ZK20})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 4)
                ZK3 = CuttingData(Value, 15)
                ZK4 = CuttingData(Value, 3)
                ZK5 = CuttingData(Value, 15)
                ZK6 = CuttingData(Value, 1)
                ZK7 = CuttingData(Value, 7)
                ZK8 = CuttingData(Value, 30)
                ZK9 = CuttingData(Value, 10)
                ZK10 = CuttingData(Value, 10)
                ZK11 = CuttingData(Value, 10)
                ZK12 = CuttingData(Value, 20)
                ZK13 = CuttingData(Value, 10)
                ZK14 = CuttingData(Value, 1)
                ZK15 = CuttingData(Value, 20)
                ZK16 = CuttingData(Value, 30)
                ZK17 = CuttingData(Value, 1)
                ZK18 = CuttingData(Value, 10)
                ZK19 = CuttingData(Value, 2)
                ZK20 = CuttingData(Value, 20)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC2 As ZEIKIN_RECORD2

    'トレーラレコード
    Structure ZEIKIN_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   'データ区分(=8)
        <VBFixedString(6)> Public ZK2 As String   '合計件数
        <VBFixedString(12)> Public ZK3 As String  '合計金額
        <VBFixedString(6)> Public ZK4 As String   '振替済件数
        <VBFixedString(12)> Public ZK5 As String  '振替済金額
        <VBFixedString(6)> Public ZK6 As String   '振替不能件数
        <VBFixedString(12)> Public ZK7 As String  '振替不能金額
        <VBFixedString(165)> Public ZK8 As String 'ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2, ZK3, ZK4, ZK5, ZK6, ZK7, _
                             ZK8})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 6)
                ZK3 = CuttingData(Value, 12)
                ZK4 = CuttingData(Value, 6)
                ZK5 = CuttingData(Value, 12)
                ZK6 = CuttingData(Value, 6)
                ZK7 = CuttingData(Value, 12)
                ZK8 = CuttingData(Value, 165)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC8 As ZEIKIN_RECORD8

    Structure ZEIKIN_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   'データ区分(=9)
        <VBFixedString(219)> Public ZK2 As String   'ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 219)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC9 As ZEIKIN_RECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "220.P"

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' 機能　 ： レコードチェック
    '
    ' 戻り値 ： 不正文字の位置
    '
    ' 備考　 ：
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(DataInfo.LenOfOneRec)
        Dim nRet As Long

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' ヘッダ
                buff.Append(RecordData, 0, 14)
                buff.Append(ReplaceString(RecordData.Substring(14, 40)))
                buff.Append(RecordData.Substring(54))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If
            Case "2"        ' データ
                buff.Append(RecordData, 0, 46)
                buff.Append(ReplaceString(RecordData.Substring(46, 30)))
                buff.Append(RecordData.Substring(76))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If

                ' 規定文字チェック
                nRet = CheckRegularStringVerB(RecordData, 0, 198)
                If nRet >= 0 Then
                    Return nRet
                End If
            Case "8"        ' トレーラ
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If
            Case "9"        ' エンド
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If
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
    Public Overrides Function CheckDataFormat() As String
        ' 基本クラス チェック
        Dim sRet As String = MyBase.CheckDataFormat()
        If sRet = "ERR" Then
            ' 規定外文字あり
            Return "ERR"
        End If

        Select Case RecordData.Substring(0, 1)
            Case "1"
                If BeforeRecKbn <> "" And BeforeRecKbn <> "8" And BeforeRecKbn <> "9" Then
                    DataInfo.Message = "ファイルレコード（ヘッダ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord1()
                End If
            Case "2"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
                    DataInfo.Message = "ファイルレコード（データ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord2()
                End If
            Case "8"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
                    DataInfo.Message = "ファイルレコード（トレーラ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord8()
                End If
            Case "9"
                If BeforeRecKbn <> "8" Then
                    DataInfo.Message = "ファイルレコード（エンド区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord9()
                End If
        End Select

        ' 各フォーマット　共通後処理
        MyBase.CheckDataFormatAfter()

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
        ZEIKIN_REC1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC1.ZK1
        InfoMeisaiMast.SYUBETU_CODE = ZEIKIN_REC1.ZK2
        InfoMeisaiMast.CODE_KBN = ZEIKIN_REC1.ZK3
        InfoMeisaiMast.ITAKU_CODE = ZEIKIN_REC1.ZK4
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZEIKIN_REC1.ZK6, "yyyyMMdd")
        InfoMeisaiMast.ITAKU_KIN = ZEIKIN_REC1.ZK7
        InfoMeisaiMast.ITAKU_SIT = ZEIKIN_REC1.ZK9
        InfoMeisaiMast.ITAKU_KAMOKU = ZEIKIN_REC1.ZK11
        InfoMeisaiMast.ITAKU_KOUZA = ZEIKIN_REC1.ZK12
        InfoMeisaiMast.ITAKU_KNAME = ZEIKIN_REC1.ZK5

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
        OraReader.Close()

        'データチェック
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

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
        ZEIKIN_REC2.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC2.ZK1
        InfoMeisaiMast.KEIYAKU_KIN = ZEIKIN_REC2.ZK2
        InfoMeisaiMast.KEIYAKU_SIT = ZEIKIN_REC2.ZK4
        InfoMeisaiMast.KEIYAKU_KAMOKU = ZEIKIN_REC2.ZK6
        InfoMeisaiMast.KEIYAKU_KOUZA = ZEIKIN_REC2.ZK7
        InfoMeisaiMast.KEIYAKU_KNAME = ZEIKIN_REC2.ZK8.Trim
        InfoMeisaiMast.FURIKIN = CASTCommon.CADec(ZEIKIN_REC2.ZK9)
        InfoMeisaiMast.FURIKIN_MOTO = ZEIKIN_REC2.ZK9
        InfoMeisaiMast.SINKI_CODE = ZEIKIN_REC2.ZK14
        InfoMeisaiMast.KEIYAKU_NO = ZEIKIN_REC2.ZK15.Substring(0, 10)
        InfoMeisaiMast.JYUYOKA_NO = ZEIKIN_REC2.ZK15
        If mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
            InfoMeisaiMast.KTEKIYO = ZEIKIN_REC2.ZK5.Trim
            InfoMeisaiMast.NTEKIYO = ""
        Else
            InfoMeisaiMast.KTEKIYO = ""
            InfoMeisaiMast.NTEKIYO = ""
        End If

        InfoMeisaiMast.FURIKETU_CODE = 0
        InfoMeisaiMast.FURIKETU_MOTO = "0"

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        'データチェック
        If MyBase.CheckDataRecord() = False Then
            Return "IJO"
        End If

        Return "D"
    End Function

    '
    ' 機能　 ： トレーラーレコードチェック
    '
    ' 戻り値 ： True - 成功， False - 失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckRecord8() As String
        ZEIKIN_REC8.Data = RecordData

        ' 明細マスタ項目設定 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC8.ZK1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CADec(ZEIKIN_REC8.ZK2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CADec(ZEIKIN_REC8.ZK3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CADec(ZEIKIN_REC8.ZK4)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CADec(ZEIKIN_REC8.ZK5)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CADec(ZEIKIN_REC8.ZK6)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CADec(ZEIKIN_REC8.ZK7)

        'データチェック
        If MyBase.CheckTrailerRecord() = False Then
            Return "ERR"
        End If

        Return "T"
    End Function

    '
    ' 機能　 ： エンドレコードチェック
    '
    ' 戻り値 ： True - 成功， False - 失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckRecord9() As String
        ZEIKIN_REC9.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC9.ZK1

        Return "E"
    End Function

End Class
