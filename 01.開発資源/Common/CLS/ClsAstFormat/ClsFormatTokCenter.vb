Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' 金庫渡しＭＴ東海センターフォーマット（標準）クラス
Public Class CFormatTokCenter
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 640

    ' ------------------------------------------
    ' 金庫渡しＭＴ東海センターフォーマット
    ' ------------------------------------------
    ' 各レコード共通部
    Public Structure TCRECORD
        Implements CFormat.IFormat

        <VBFixedString(8)> Public TC1 As String     ' 振替日指定                
        <VBFixedString(4)> Public TC2 As String     ' 持込企業コード
        <VBFixedString(2)> Public TC3 As String     ' 持込管理番号
        <VBFixedString(8)> Public TC4 As String     ' シーケンスNO
        <VBFixedString(3)> Public TC5 As String     ' 振替コード
        <VBFixedString(5)> Public TC6 As String     ' 企業コード
        <VBFixedString(2)> Public TC7 As String     ' 企業識別 
        <VBFixedString(4)> Public TC8 As String     ' 金庫コード
        <VBFixedString(8)> Public TC9 As String     ' チェック年月日
        <VBFixedString(2)> Public TC10 As String    ' 持込チェック結果コード
        <VBFixedString(2)> Public TC11 As String    ' ヘッダ種別コード
        <VBFixedString(1)> Public TC12 As String    ' 金庫持帰りMT作成済み表示
        <VBFixedString(1)> Public TC13 As String    ' 翌営業日返還表示
        <VBFixedString(5)> Public TC14 As String    ' 初振企業コード
        <VBFixedString(5)> Public TC15 As String    ' 予備

        <VBFixedString(2)> Public CO1 As String     ' 振込科目
        <VBFixedString(2)> Public CO2 As String     ' 科目コード（センタ側）
        <VBFixedString(7)> Public CO3 As String     ' 口座番号　（センタ側）
        <VBFixedString(1)> Public CO4 As String     ' 科目名称コード
        <VBFixedString(3)> Public CO5 As String     ' 店番　　　（訂正後）
        <VBFixedString(9)> Public CO6 As String     ' 科目・口番（訂正後）
        <VBFixedString(4)> Public CO7 As String     ' 予備

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {TC1, TC2, TC3, TC4, TC5, TC6, TC7, _
                            TC8, TC9, TC10, TC11, TC12, TC13, TC14, TC15, _
                            CO1, CO2, CO3, CO4, CO5, CO6, CO7 _
                            })
            End Get
            Set(ByVal value As String)
                TC1 = CuttingData(value, 8)
                TC2 = CuttingData(value, 4)
                TC3 = CuttingData(value, 2)
                TC4 = CuttingData(value, 8)
                TC5 = CuttingData(value, 3)
                TC6 = CuttingData(value, 5)
                TC7 = CuttingData(value, 2)
                TC8 = CuttingData(value, 4)
                TC9 = CuttingData(value, 8)
                TC10 = CuttingData(value, 2)
                TC11 = CuttingData(value, 2)
                TC12 = CuttingData(value, 1)
                TC13 = CuttingData(value, 1)
                TC14 = CuttingData(value, 5)
                TC15 = CuttingData(value, 5)
                CO1 = CuttingData(value, 2)
                CO2 = CuttingData(value, 2)
                CO3 = CuttingData(value, 7)
                CO4 = CuttingData(value, 1)
                CO5 = CuttingData(value, 3)
                CO6 = CuttingData(value, 9)
                CO7 = CuttingData(value, 4)
            End Set
        End Property
    End Structure

    ' ------------------------------------------
    ' 金庫渡しＭＴ東海センターフォーマット
    ' ------------------------------------------
    ' ヘッダレコード
    Public Structure CORECORD1
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' 各レコード共通部

        Public R1 As CFormatZengin.ZGRECORD1        ' **ここから全銀データ形式となる**
        <VBFixedString(432)> Public CO1 As String   ' ダミー

        Public N1 As CFormatNTTCMT.NTRECORD1        ' **ここからＮＴＴ口座振替データ形式となる**
        <VBFixedString(372)> Public CO2 As String   ' ダミー

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' 特殊ＦＴ
                    Return String.Concat(New String() _
                                {TC.Data, N1.Data, CO2})
                End If
                ' 通常
                Return String.Concat(New String() _
                            {TC.Data, R1.Data, CO1})
            End Get
            Set(ByVal value As String)
                TC.Data = CuttingData(value, 88)
                If TC.TC5 = "014" And (TC.TC6 = "40140" Or TC.TC6 = "40141") Then
                    ' 特殊ＦＴ
                    NTTFlag = True
                    N1.Data = CuttingData(value, 180)
                    CO2 = CuttingData(value, 372)
                Else
                    ' 通常
                    NTTFlag = False
                    R1.Data = CuttingData(value, 120)
                    CO1 = CuttingData(value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC1 As CORECORD1

    ' データレコード
    Structure STRECORD2
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' 各レコード共通部

        Public R2 As CFormatZengin.ZGRECORD2        ' **ここから全銀データ形式となる**
        <VBFixedString(432)> Public CO1 As String   ' ダミー

        Public N2 As CFormatNTTCMT.NTRECORD2        ' **ここからＮＴＴ口座振替データ形式となる**
        <VBFixedString(372)> Public CO2 As String   ' ダミー

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' 特殊ＦＴ
                    Return String.Concat(New String() _
                                {TC.Data, N2.Data, CO2})
                End If
                ' 通常
                Return String.Concat(New String() _
                            {TC.Data, R2.Data, CO1})
            End Get
            Set(ByVal Value As String)
                TC.Data = CuttingData(Value, 88)
                If NTTFlag = True Then
                    ' 特殊ＦＴ
                    N2.Data = CuttingData(Value, 180)
                    CO2 = CuttingData(Value, 372)
                Else
                    ' 通常
                    R2.Data = CuttingData(Value, 120)
                    CO1 = CuttingData(Value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC2 As STRECORD2

    ' トレーラレコード
    Structure STRECORD8
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' 各レコード共通部
        Public R8 As CFormatZengin.ZGRECORD8        ' **ここから全銀データ形式となる**
        <VBFixedString(432)> Public CO1 As String   ' ダミー

        Public N8 As CFormatNTTCMT.NTRECORD8        ' **ここからＮＴＴ口座振替データ形式となる**
        <VBFixedString(372)> Public CO2 As String   ' ダミー


        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' 特殊ＦＴ
                    Return String.Concat(New String() _
                                {TC.Data, N8.Data, CO2})
                End If
                ' 通常
                Return String.Concat(New String() _
                            {TC.Data, R8.Data, CO1})
            End Get
            Set(ByVal Value As String)
                TC.Data = CuttingData(Value, 88)
                If NTTFlag = True Then
                    ' 特殊ＦＴ
                    N8.Data = CuttingData(Value, 180)
                    CO2 = CuttingData(Value, 372)
                Else
                    ' 通常
                    R8.Data = CuttingData(Value, 120)
                    CO1 = CuttingData(Value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC8 As STRECORD8

    ' エンドレコード
    Structure STRECORD9
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' 各レコード共通部
        Public R9 As CFormatZengin.ZGRECORD9        ' **ここから全銀データ形式となる**
        <VBFixedString(432)> Public CO1 As String   ' ダミー

        Public N9 As CFormatNTTCMT.NTRECORD9        ' **ここからＮＴＴ口座振替データ形式となる**
        <VBFixedString(372)> Public CO2 As String   ' ダミー

        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' 特殊ＦＴ
                    Return String.Concat(New String() _
                                {TC.Data, N9.Data, CO2})
                End If
                ' 通常
                Return String.Concat(New String() _
                            {TC.Data, R9.Data, CO1})
            End Get
            Set(ByVal Value As String)
                TC.Data = CuttingData(Value, 88)
                If NTTFlag = True Then
                    ' 特殊ＦＴ
                    N9.Data = CuttingData(Value, 180)
                    CO2 = CuttingData(Value, 372)
                Else
                    ' 通常
                    R9.Data = CuttingData(Value, 120)
                    CO1 = CuttingData(Value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC9 As STRECORD9

    ' 特殊フォーマット判定
    Private Shared NTTFlag As Boolean

    ' New
    Public Sub New()
        MyBase.New()

        NTTFlag = False

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "2", "8", "9"}
        
        FtranPfile = "640.P"

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' 機能　 ： 規定文字チェック ＆　文字置換処理
    '
    ' 戻り値 ： 不正文字の位置
    '
    ' 備考　 ： RepaceString()関数にて文字置換を実施
    '           置換対象文字は，不正文字にはならないはず
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(DataInfo.LenOfOneRec)

        ' 全角文字１文字を半角空白２バイトに変換
        For i As Integer = 0 To RecordData.Length - 1
            Dim s As String = RecordData.Substring(i, 1)
            If EncdJ.GetByteCount(s) = 2 Then
                buff.Append("  ")
            Else
                buff.Append(s)
            End If
        Next i
        If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
            mRecordData = buff.ToString(0, RecordLen)
        End If

        Select Case RecordData.Substring(88, 1)
            Case "1"        ' ヘッダレコード
            Case "2"        ' データレコード
            Case "8"        ' トレーラ
            Case "9"        ' エンド
        End Select

        ' 全角チェック
        Return GetZenkakuPos(RecordData)
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

        If RecordData.Length = 0 Then
            DataInfo.Message = "ファイル異常"
            mnErrorNumber = 1
            Return "ERR"
        End If

        Select Case RecordData.Substring(88, 1)
            Case "1"
                sRet = CheckRecord1()
            Case "2"
                sRet = CheckRecord2()
            Case "8"
                sRet = CheckRecord8()
            Case "9"
                sRet = CheckRecord9()
                '*** 修正 mitsu 2008/07/16 空白レコードを通す ***
            Case " "
                sRet = "E"
                '************************************************
            Case Else
                DataInfo.Message = "レコード区分異常（" & RecordData.Substring(0, 1) & "）異常"
                mnErrorNumber = 1
                Return "ERR"
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

        Dim str As String = ""

        TOKCENTER_REC1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC1.R1.ZG1
        InfoMeisaiMast.FURIKAE_DATE = TOKCENTER_REC1.TC.TC1
        InfoMeisaiMast.FURIKAE_DATE_MOTO = TOKCENTER_REC1.TC.TC1

        InfoMeisaiMast.FURI_CODE = TOKCENTER_REC1.TC.TC5
        InfoMeisaiMast.KIGYO_CODE = TOKCENTER_REC1.TC.TC6

        If NTTFlag = True Then                   '特殊ＦＴの判定
            'ＮＴＴデータ区分対応
            InfoMeisaiMast.DATA_KBN = TOKCENTER_REC1.N1.NT1
            'ＮＴＴデータ区分対応
            InfoMeisaiMast.SYUBETU_CODE = TOKCENTER_REC1.N1.NT2
            InfoMeisaiMast.ITAKU_CODE = "9999999999"          '委託者コード固定
            InfoMeisaiMast.ITAKU_KIN = TOKCENTER_REC1.N1.NT10
            InfoMeisaiMast.ITAKU_KAMOKU = TOKCENTER_REC1.N1.NT13
            InfoMeisaiMast.ITAKU_KOUZA = TOKCENTER_REC1.N1.NT14
            InfoMeisaiMast.ITAKU_KNAME = TOKCENTER_REC1.N1.NT9
        Else
            InfoMeisaiMast.SYUBETU_CODE = TOKCENTER_REC1.R1.ZG2
            InfoMeisaiMast.ITAKU_CODE = TOKCENTER_REC1.R1.ZG4
            InfoMeisaiMast.ITAKU_KIN = TOKCENTER_REC1.R1.ZG7
            InfoMeisaiMast.ITAKU_SIT = TOKCENTER_REC1.R1.ZG9
            InfoMeisaiMast.ITAKU_KAMOKU = TOKCENTER_REC1.R1.ZG11
            InfoMeisaiMast.ITAKU_KOUZA = TOKCENTER_REC1.R1.ZG12
            InfoMeisaiMast.ITAKU_KNAME = TOKCENTER_REC1.R1.ZG5
        End If

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        ' データチェック（金庫持ち込み東海センター用）
        str = CheckHeaderRecordC()
        If str <> "" Then
            Return str
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
        TOKCENTER_REC2.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC2.R2.ZG1
        If NTTFlag = True Then '特殊ＦＴの判定
            'ＮＴＴデータ区分対応
            InfoMeisaiMast.DATA_KBN = TOKCENTER_REC2.N2.NT1
            'ＮＴＴデータ区分対応
            InfoMeisaiMast.KEIYAKU_KIN = TOKCENTER_REC2.N2.NT10
            InfoMeisaiMast.KEIYAKU_SIT = TOKCENTER_REC2.N2.NT11
            InfoMeisaiMast.KEIYAKU_KAMOKU = TOKCENTER_REC2.N2.NT12.PadLeft(2, "0"c)
            ' 2008.01.18 Delete >>
            ''センター直接FMTは銀行科目で来るため、信金科目に返還）20050705
            ''Select Case InfoMeisaiMast.KEIYAKU_KAMOKU
            ''    Case "01"         '普通
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "02"
            ''    Case "02"         '当座
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "01"
            ''    Case "03"         '納税
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "05"
            ''    Case "04"         '職員
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "37"
            ''    Case Else
            ''End Select
            ' 2008.01.18 Delete <<
            InfoMeisaiMast.KEIYAKU_KOUZA = TOKCENTER_REC2.N2.NT13
            InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC2.TC.TC4
            InfoMeisaiMast.KEIYAKU_KNAME = Trim(TOKCENTER_REC2.N2.NT20)  '2007/10/12 20桁に変更 →2007/10/24　30桁に変更
            InfoMeisaiMast.FURIKIN = CaDecNormal(TOKCENTER_REC2.N2.NT18)
            InfoMeisaiMast.FURIKIN_MOTO = TOKCENTER_REC2.N2.NT18
            InfoMeisaiMast.SINKI_CODE = "0"
            InfoMeisaiMast.KEIYAKU_NO = ""
            '2007/05/29:KG 静清信金用　需要家番号(21ﾊﾞｲﾄ)
            InfoMeisaiMast.JYUYOKA_NO = TOKCENTER_REC2.N2.NT2 & TOKCENTER_REC2.N2.NT3 & TOKCENTER_REC2.N2.NT4 & TOKCENTER_REC2.N2.NT5 & TOKCENTER_REC2.N2.NT6
        Else
            InfoMeisaiMast.KEIYAKU_KIN = TOKCENTER_REC2.R2.ZG2
            InfoMeisaiMast.KEIYAKU_SIT = TOKCENTER_REC2.R2.ZG4
            InfoMeisaiMast.KEIYAKU_KAMOKU = TOKCENTER_REC2.R2.ZG7.PadLeft(2, "0"c) 'データの科目は１ｂなので、２ｂに変換
            ' 2008.01.18 Delete >>
            ''センター直接FMTは銀行科目で来るため、信金科目に返還）20050705
            ''Select Case InfoMeisaiMast.KEIYAKU_KAMOKU
            ''    Case "01"         '普通
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "02"
            ''    Case "02"         '当座
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "01"
            ''    Case "03"         '納税
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "05"
            ''    Case "04"         '職員
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "37"
            ''    Case Else
            ''End Select
            ' 2008.01.18 Delete <<
            InfoMeisaiMast.KEIYAKU_KOUZA = TOKCENTER_REC2.R2.ZG8
            InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC2.TC.TC4
            InfoMeisaiMast.KEIYAKU_KNAME = Trim(TOKCENTER_REC2.R2.ZG9)  '2007/10/12 20桁に変更  →2007/10/24 30桁に変更
            InfoMeisaiMast.FURIKIN = CaDecNormal(TOKCENTER_REC2.R2.ZG10)
            InfoMeisaiMast.FURIKIN_MOTO = TOKCENTER_REC2.R2.ZG10
            InfoMeisaiMast.SINKI_CODE = TOKCENTER_REC2.R2.ZG11
            InfoMeisaiMast.KEIYAKU_NO = TOKCENTER_REC2.R2.ZG12
            InfoMeisaiMast.JYUYOKA_NO = TOKCENTER_REC2.R2.ZG12 & TOKCENTER_REC2.R2.ZG13
        End If

        If Not mInfoComm Is Nothing AndAlso mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
            InfoMeisaiMast.KTEKIYO = TOKCENTER_REC2.R2.ZG5.PadRight(15, " "c).Substring(0, 13).Trim
            InfoMeisaiMast.NTEKIYO = ""
        End If


        InfoMeisaiMast.FURIKETU_CODE = 0
        InfoMeisaiMast.FURIKETU_MOTO = "0"

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1


        ' データチェック
        '金額チェック
        If IsDecimal(InfoMeisaiMast.FURIKIN_MOTO) = False Then
            Dim InError As INPUTERROR = Nothing

            InfoMeisaiMast.FURIKETU_CODE = 9
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
            InErrorArray.Add(InError)

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
        TOKCENTER_REC8.Data = RecordData

        ' 明細マスタ項目設定 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC8.R8.ZG1
        InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC8.TC.TC4

        If NTTFlag = True Then '特殊ＦＴの判定(種目コード”10”→初振、”91””21”→再振)20050413
            'ＮＴＴデータ区分対応
            InfoMeisaiMast.DATA_KBN = TOKCENTER_REC8.N8.NT1
            'ＮＴＴデータ区分対応
            Select Case InfoMeisaiMast.SYUBETU_CODE
                Case "10"         '初振
                    InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT6)
                    InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT7)

                    InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = TOKCENTER_REC8.N8.NT6
                    InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = TOKCENTER_REC8.N8.NT7
                Case Else         '再振
                    InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT12)
                    InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT13)

                    InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = TOKCENTER_REC8.N8.NT12
                    InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = TOKCENTER_REC8.N8.NT13
            End Select
        Else
            InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(TOKCENTER_REC8.R8.ZG2)
            InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(TOKCENTER_REC8.R8.ZG3)

            InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = TOKCENTER_REC8.R8.ZG2
            InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = TOKCENTER_REC8.R8.ZG3
        End If
        InfoMeisaiMast.TOTAL_ZUMI_KEN = 0
        InfoMeisaiMast.TOTAL_ZUMI_KIN = 0
        InfoMeisaiMast.TOTAL_FUNO_KEN = 0
        InfoMeisaiMast.TOTAL_FUNO_KIN = 0

        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = "0"

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
        TOKCENTER_REC9.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC9.R9.ZG1
        InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC9.TC.TC4

        Return "E"
    End Function

    '
    ' 機能　 ： ヘッダレコードを読み込んでチェック
    '
    ' 備考　 ： 各フォーマットチェックから，呼ばれる共通関数
    ' 　　　     ヘッダ情報から，取引先マスタを参照する
    '
    'Protected Overrides Function CheckHeaderRecord() As Boolean
    Private Function CheckHeaderRecordC() As String

        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader
        Dim ToriReader As CASTCommon.MyOracleReader
        Dim strRet As String = ""

        ' ＤＢ接続が存在しない場合，正常値を返す
        If OraDB Is Nothing Then
            Return ""
        End If

        ' 口振
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(",TYUUDAN_FLG_S")
        SQL.Append(",TOUROKU_FLG_S")
        SQL.Append(",UKETUKE_FLG_S")
        SQL.Append(",BAITAI_CODE_T")
        SQL.Append(" FROM TORIMAST,SCHMAST")
        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
        SQL.Append("   AND FURI_CODE_S =  " & SQ(TOKCENTER_REC1.TC.TC5))
        SQL.Append("   AND KIGYO_CODE_S = " & SQ(TOKCENTER_REC1.TC.TC6))
        If ",91,71,10".IndexOf(InfoMeisaiMast.SYUBETU_CODE) >= 1 Then    '入出金区分
            SQL.Append("   AND NS_KBN_T = '9'")
        Else
            SQL.Append("   AND NS_KBN_T = '1'")
        End If
        SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
        SQL.Append("   AND '1'          = FSYORI_KBN_T")
        SQL.Append("   AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append("   AND TORIF_CODE_S = TORIF_CODE_T")
        SQL.Append("   AND KIGYO_CODE_S = KIGYO_CODE_T")
        SQL.Append("   AND FURI_CODE_S  = FURI_CODE_T ")

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then

            '**************
            '取引先のみ検索
            '**************
            SQL = New StringBuilder(128)
            SQL.Append(" SELECT")
            SQL.Append(" TORIS_CODE_T,TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE FURI_CODE_T = " & SQ(TOKCENTER_REC1.TC.TC5))
            SQL.Append(" AND KIGYO_CODE_T = " & SQ(TOKCENTER_REC1.TC.TC6))
            ToriReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
            If ToriReader.DataReader(SQL) = False Then

                ' 取引先マスタ情報をクリアする
                Call mInfoComm.GetTORIMAST("", "")
                WriteBLog("ファイルヘッダ取引先検索", "失敗", "委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
                DataInfo.Message = "取引先検索失敗 委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " 振替コード：" & TOKCENTER_REC1.TC.TC5 & " 企業コード：" & TOKCENTER_REC1.TC.TC6 & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
                strRet = "NT"
            Else

                ' 取引先マスタを取得
                Call mInfoComm.GetTORIMAST(ToriReader.GetItem("TORIS_CODE_T"), ToriReader.GetItem("TORIF_CODE_T"))
                WriteBLog("スケジュール検索", "失敗", "委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
                DataInfo.Message = "スケジュール検索失敗 委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " 振替コード：" & TOKCENTER_REC1.TC.TC5 & " 企業コード：" & TOKCENTER_REC1.TC.TC6 & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
                strRet = "NS"
            End If

            ToriReader.Close()

        Else
            ' 取引先マスタを取得
            Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
        End If

        ' 明細マスタ項目設定
        InfoMeisaiMast.ITAKU_KIN = mInfoComm.INFOToriMast.TKIN_NO_T     ' 取扱金融機関コード
        InfoMeisaiMast.ITAKU_SIT = mInfoComm.INFOToriMast.TSIT_NO_T     ' 取扱支店コード
        InfoMeisaiMast.ITAKU_KAMOKU = mInfoComm.INFOToriMast.KAMOKU_T   ' 科目
        InfoMeisaiMast.ITAKU_KOUZA = mInfoComm.INFOToriMast.KOUZA_T     ' 口座番号

        BLOG.ToriCode = mInfoComm.INFOToriMast.TORIS_CODE_T & mInfoComm.INFOToriMast.TORIF_CODE_T

        ' 2007/07/04　フォーマット区分、媒体コード、中断フラグ、登録フラグチェック
        If OraReader.EOF = False AndAlso OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
            WriteBLog("スケジュール:中断フラグ設定済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"), "中断")
            DataInfo.Message = "スケジュール:中断フラグ設定済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            strRet = "TS"
        End If

        If OraReader.EOF = False AndAlso OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
            WriteBLog("スケジュール:落し込み処理済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"), "中断")
            DataInfo.Message = "スケジュール:落し込み処理済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            strRet = "SS"
        End If

        ' オラクルReaderクローズ
        OraReader.Close()
        WriteBLog("ファイルヘッダ取引先検索", "成功", "取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
        Return strRet
    End Function

    Public Overrides Function IsHeaderRecord() As Boolean
        For i As Integer = 0 To HeaderKubun.Length - 1
            If RecordData.Substring(88, 1) = HeaderKubun(i) Then
                Return True
            End If
        Next i
        Return False
    End Function

    Public Overrides Function IsDataRecord() As Boolean
        For i As Integer = 0 To DataKubun.Length - 1
            If RecordData.Substring(88, 1) = DataKubun(i) Then
                Return True
            End If
        Next i
        Return False
    End Function

    Public Overrides Function IsTrailerRecord() As Boolean
        For i As Integer = 0 To TrailerKubun.Length - 1
            If RecordData.Substring(88, 1) = TrailerKubun(i) Then
                Return True
            End If
        Next i
        Return False
    End Function

    Public Overrides Function IsEndRecord() As Boolean
        If RecordData.Substring(88, 1) = DataInfo.MinRecordCode(DataInfo.MinRecordCode.Length - 1) Then
            Return True
        End If
        Return False
    End Function

    Public Overrides Function UpdateSCHMAST() As Boolean

        Return MyBase.UpdateSCHMAST

    End Function

End Class
