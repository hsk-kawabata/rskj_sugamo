Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' 地公体（３５０） データフォーマットクラス
Public Class CFormatZeikin350
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 350

    ' ------------------------------------------
    ' 地公体フォーマット（３５０バイト）
    ' ------------------------------------------
    ' ヘッダーレコード
    Structure ZEIKIN350_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String    ' データ区分(=1)
        <VBFixedString(2)> Public ZK2 As String    ' 種別コード
        <VBFixedString(1)> Public ZK3 As String    ' コード区分
        <VBFixedString(8)> Public ZK4 As String    ' 委託者コード
        <VBFixedString(2)> Public ZK5 As String    ' 科目コード
        <VBFixedString(5)> Public ZK6 As String    ' 科目名
        <VBFixedString(35)> Public ZK7 As String   ' 委託者名
        <VBFixedString(2)> Public ZK8 As String    ' 振替年
        <VBFixedString(2)> Public ZK9 As String    ' 振替月
        <VBFixedString(2)> Public ZK10 As String   ' 振替日
        <VBFixedString(4)> Public ZK11 As String   ' 銀行コード
        <VBFixedString(15)> Public ZK12 As String  ' 銀行名
        <VBFixedString(3)> Public ZK13 As String   ' 支店コード
        <VBFixedString(15)> Public ZK14 As String  ' 支店名
        <VBFixedString(1)> Public ZK15 As String   ' 入金預金種目
        <VBFixedString(7)> Public ZK16 As String   ' 入金口座番号
        <VBFixedString(2)> Public ZK17 As String   ' 年度
        <VBFixedString(243)> Public ZK18 As String ' 予備
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 2), _
                            SubData(ZK3, 1), _
                            SubData(ZK4, 8), _
                            SubData(ZK5, 2), _
                            SubData(ZK6, 5), _
                            SubData(ZK7, 35), _
                            SubData(ZK8, 2), _
                            SubData(ZK9, 2), _
                            SubData(ZK10, 2), _
                            SubData(ZK11, 4), _
                            SubData(ZK12, 15), _
                            SubData(ZK13, 3), _
                            SubData(ZK14, 15), _
                            SubData(ZK15, 1), _
                            SubData(ZK16, 7), _
                            SubData(ZK17, 2), _
                            SubData(ZK18, 243) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 2)
                ZK3 = CuttingData(Value, 1)
                ZK4 = CuttingData(Value, 8)
                ZK5 = CuttingData(Value, 2)
                ZK6 = CuttingData(Value, 5)
                ZK7 = CuttingData(Value, 35)
                ZK8 = CuttingData(Value, 2)
                ZK9 = CuttingData(Value, 2)
                ZK10 = CuttingData(Value, 2)
                ZK11 = CuttingData(Value, 4)
                ZK12 = CuttingData(Value, 15)
                ZK13 = CuttingData(Value, 3)
                ZK14 = CuttingData(Value, 15)
                ZK15 = CuttingData(Value, 1)
                ZK16 = CuttingData(Value, 7)
                ZK17 = CuttingData(Value, 2)
                ZK18 = CuttingData(Value, 243)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC1 As ZEIKIN350_RECORD1

    ' データレコード
    Structure ZEIKIN350_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   ' データ区分(=2)
        <VBFixedString(4)> Public ZK2 As String   ' 引落銀行番号
        <VBFixedString(15)> Public ZK3 As String  ' 引落銀行名
        <VBFixedString(3)> Public ZK4 As String   ' 引落支店番号
        <VBFixedString(15)> Public ZK5 As String  ' 引落支店名
        <VBFixedString(4)> Public ZK6 As String   ' 予備
        <VBFixedString(1)> Public ZK7 As String   ' 預金種目
        <VBFixedString(7)> Public ZK8 As String   ' 口座番号
        <VBFixedString(30)> Public ZK9 As String  ' 口座名義人
        <VBFixedString(10)> Public ZK10 As String ' 引落金額
        <VBFixedString(1)> Public ZK11 As String  ' 新規コード
        <VBFixedString(1)> Public ZK12 As String  ' 振替結果コード
        <VBFixedString(10)> Public ZK13 As String ' 予備(スペース＋銀行使用欄)
        <VBFixedString(18)> Public ZK14 As String ' （新）義務者番号
        <VBFixedString(2)> Public ZK15 As String  ' 予備
        <VBFixedString(18)> Public ZK16 As String ' （旧）義務者番号
        <VBFixedString(2)> Public ZK17 As String  ' 種目コード
        <VBFixedString(5)> Public ZK18 As String  ' 種目名
        <VBFixedString(2)> Public ZK19 As String  ' 年度
        <VBFixedString(5)> Public ZK20 As String  ' 期別
        <VBFixedString(11)> Public ZK21 As String ' 整理番号
        <VBFixedString(26)> Public ZK22 As String ' 収納番号
        <VBFixedString(7)> Public ZK23 As String  ' 予備
        <VBFixedString(30)> Public ZK24 As String ' 納税（付）者名氏名
        <VBFixedString(5)> Public ZK25 As String  ' 郵便番号
        <VBFixedString(22)> Public ZK26 As String ' 住所（市町村）
        <VBFixedString(22)> Public ZK27 As String ' 住所（町名）
        <VBFixedString(22)> Public ZK28 As String ' 住所（番地）
        <VBFixedString(22)> Public ZK29 As String ' 住所（方書）
        <VBFixedString(15)> Public ZK30 As String ' 科目
        <VBFixedString(14)> Public ZK31 As String ' 口座番号
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 4), _
                            SubData(ZK3, 15), _
                            SubData(ZK4, 3), _
                            SubData(ZK5, 15), _
                            SubData(ZK6, 4), _
                            SubData(ZK7, 1), _
                            SubData(ZK8, 7), _
                            SubData(ZK9, 30), _
                            SubData(ZK10, 10), _
                            SubData(ZK11, 1), _
                            SubData(ZK12, 1), _
                            SubData(ZK13, 10), _
                            SubData(ZK14, 18), _
                            SubData(ZK15, 2), _
                            SubData(ZK16, 18), _
                            SubData(ZK17, 2), _
                            SubData(ZK18, 5), _
                            SubData(ZK19, 2), _
                            SubData(ZK20, 5), _
                            SubData(ZK21, 11), _
                            SubData(ZK22, 26), _
                            SubData(ZK23, 7), _
                            SubData(ZK24, 30), _
                            SubData(ZK25, 5), _
                            SubData(ZK26, 22), _
                            SubData(ZK27, 22), _
                            SubData(ZK28, 22), _
                            SubData(ZK29, 22), _
                            SubData(ZK30, 15), _
                            SubData(ZK31, 14) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 4)
                ZK3 = CuttingData(Value, 15)
                ZK4 = CuttingData(Value, 3)
                ZK5 = CuttingData(Value, 15)
                ZK6 = CuttingData(Value, 4)
                ZK7 = CuttingData(Value, 1)
                ZK8 = CuttingData(Value, 7)
                ZK9 = CuttingData(Value, 30)
                ZK10 = CuttingData(Value, 10)
                ZK11 = CuttingData(Value, 1)
                ZK12 = CuttingData(Value, 1)
                ZK13 = CuttingData(Value, 10)
                ZK14 = CuttingData(Value, 18)
                ZK15 = CuttingData(Value, 2)
                ZK16 = CuttingData(Value, 18)
                ZK17 = CuttingData(Value, 2)
                ZK18 = CuttingData(Value, 5)
                ZK19 = CuttingData(Value, 2)
                ZK20 = CuttingData(Value, 5)
                ZK21 = CuttingData(Value, 11)
                ZK22 = CuttingData(Value, 26)
                ZK23 = CuttingData(Value, 7)
                ZK24 = CuttingData(Value, 30)
                ZK25 = CuttingData(Value, 5)
                ZK26 = CuttingData(Value, 22)
                ZK27 = CuttingData(Value, 22)
                ZK28 = CuttingData(Value, 22)
                ZK29 = CuttingData(Value, 22)
                ZK30 = CuttingData(Value, 15)
                ZK31 = CuttingData(Value, 14)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC2 As ZEIKIN350_RECORD2

    ' トレーラレコード
    Structure ZEIKIN350_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   ' データ区分(=8)
        <VBFixedString(6)> Public ZK2 As String   ' 合計件数
        <VBFixedString(12)> Public ZK3 As String  ' 合計金額
        <VBFixedString(6)> Public ZK4 As String   ' 振替済件数
        <VBFixedString(12)> Public ZK5 As String  ' 振替済金額
        <VBFixedString(6)> Public ZK6 As String   ' 振替不能件数
        <VBFixedString(12)> Public ZK7 As String  ' 振替不能金額
        <VBFixedString(295)> Public ZK8 As String ' ダミー
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 6), _
                            SubData(ZK3, 12), _
                            SubData(ZK4, 6), _
                            SubData(ZK5, 12), _
                            SubData(ZK6, 6), _
                            SubData(ZK7, 12), _
                            SubData(ZK8, 295) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 6)
                ZK3 = CuttingData(Value, 12)
                ZK4 = CuttingData(Value, 6)
                ZK5 = CuttingData(Value, 12)
                ZK6 = CuttingData(Value, 6)
                ZK7 = CuttingData(Value, 12)
                ZK8 = CuttingData(Value, 295)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC8 As ZEIKIN350_RECORD8

    Structure ZEIKIN350_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String     ' データ区分(=9)
        <VBFixedString(349)> Public ZK2 As String   ' ダミー
        ' 固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 349) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 349)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC9 As ZEIKIN350_RECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "350.P"
        FtranIBMPfile = "350IBM.P"

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

        Dim RD() As Byte = EncdJ.GetBytes(RecordData)

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' ヘッダ
                buff.Append(EncdJ.GetString(RD, 0, 19))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 19, 35), -1)) '委託者名
                buff.Append(EncdJ.GetString(RD, 54, 10))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 64, 15), -1)) '銀行名
                buff.Append(EncdJ.GetString(RD, 79, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 82, 15), -1)) '支店名
                buff.Append(EncdJ.GetString(RD, 97, 253))

            Case "2"        ' データ
                buff.Append(EncdJ.GetString(RD, 0, 5))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 5, 15), -1)) '銀行名
                buff.Append(EncdJ.GetString(RD, 20, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 23, 15), -1)) '支店名
                buff.Append(EncdJ.GetString(RD, 38, 12))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30), -1)) '口座名義人
                buff.Append(EncdJ.GetString(RD, 80, 270))

            Case "8"        ' トレーラ
                buff.Append(ReplaceString(EncdJ.GetString(RD, 0, 350), -1))

            Case "9"        ' エンド
                buff.Append(ReplaceString(EncdJ.GetString(RD, 0, 350), -1))
        End Select

        mRecordData = buff.ToString

        Return -1
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

        Select Case RecordData.Substring(0, 1)
            Case "1"
                If BeforeRecKbn <> "" And BeforeRecKbn <> "8" And BeforeRecKbn <> "9" Then
                    DataInfo.Message = "ファイルレコード（ヘッダ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord1()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord1()
                    End If
                End If
            Case "2"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
                    DataInfo.Message = "ファイルレコード（データ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord2()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord2()
                    End If
                End If
            Case "8"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
                    DataInfo.Message = "ファイルレコード（トレーラ区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord8()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord8()
                    End If
                End If
            Case "9"
                If BeforeRecKbn <> "8" Then
                    DataInfo.Message = "ファイルレコード（エンド区分）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord9()
                End If
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
        ZEIKIN350_REC1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC1.ZK1
        InfoMeisaiMast.SYUBETU_CODE = ZEIKIN350_REC1.ZK2
        InfoMeisaiMast.ITAKU_CODE = ZEIKIN350_REC1.ZK4 & ZEIKIN350_REC1.ZK5
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZEIKIN350_REC1.ZK9 & ZEIKIN350_REC1.ZK10, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = ZEIKIN350_REC1.ZK9 & ZEIKIN350_REC1.ZK10
        InfoMeisaiMast.ITAKU_KIN = ZEIKIN350_REC1.ZK11
        InfoMeisaiMast.ITAKU_SIT = ZEIKIN350_REC1.ZK13
        InfoMeisaiMast.ITAKU_KAMOKU = ZEIKIN350_REC1.ZK15
        InfoMeisaiMast.ITAKU_KOUZA = ZEIKIN350_REC1.ZK16
        InfoMeisaiMast.ITAKU_KNAME = ZEIKIN350_REC1.ZK7

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        Return "H"
    End Function

    Private Function CheckDBRecord1() As String
        ' データチェック
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
        ZEIKIN350_REC2.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC2.ZK1
        InfoMeisaiMast.KEIYAKU_KIN = ZEIKIN350_REC2.ZK2
        InfoMeisaiMast.KEIYAKU_SIT = ZEIKIN350_REC2.ZK4
        InfoMeisaiMast.KEIYAKU_KAMOKU = ZEIKIN350_REC2.ZK7
        InfoMeisaiMast.KEIYAKU_KOUZA = ZEIKIN350_REC2.ZK8
        InfoMeisaiMast.KEIYAKU_KNAME = ZEIKIN350_REC2.ZK9.Trim
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(ZEIKIN350_REC2.ZK10)
        InfoMeisaiMast.FURIKIN_MOTO = ZEIKIN350_REC2.ZK10
        InfoMeisaiMast.SINKI_CODE = ZEIKIN350_REC2.ZK11
        '*** 修正 mitsu 2008/06/26 規定外文字対応 ***
        'InfoMeisaiMast.KEIYAKU_NO = ZEIKIN350_REC2.ZK21.Substring(0, 10)
        '*** 修正 mitsu 2008/08/05 ZK21の値を保持する ***
        'InfoMeisaiMast.KEIYAKU_NO = CASTCommon.Cutting(ZEIKIN350_REC2.ZK21, 10)
        Dim zk21 As String = ZEIKIN350_REC2.ZK21
        ZEIKIN350_REC2.ZK21 = CASTCommon.Cutting(zk21, 11)
        '*** 修正 mitsu 2008/08/26 10byteになるよう修正 ***
        zk21 = ZEIKIN350_REC2.ZK21
        InfoMeisaiMast.KEIYAKU_NO = CASTCommon.Cutting(zk21, 10)
        '**************************************************
        '************************************************
        '********************************************
        InfoMeisaiMast.YOBI1 = ZEIKIN350_REC2.ZK21
        InfoMeisaiMast.JYUYOKA_NO = ZEIKIN350_REC2.ZK16

        InfoMeisaiMast.KTEKIYO = ""
        InfoMeisaiMast.NTEKIYO = ""

        ' 摘要
        If (Not mInfoComm Is Nothing) Then
            Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                Case "0"
                    InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                    InfoMeisaiMast.NTEKIYO = ""
                Case "1"
                    InfoMeisaiMast.KTEKIYO = ""
                    InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                Case "2"
                    InfoMeisaiMast.KTEKIYO = ZEIKIN350_REC2.ZK5.PadRight(13, " "c).Substring(0, 13).Trim
                Case "3"
                    InfoMeisaiMast.KTEKIYO = ZEIKIN350_REC2.ZK5
            End Select
        End If

        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(ZEIKIN350_REC2.ZK12)
        InfoMeisaiMast.FURIKETU_MOTO = ZEIKIN350_REC2.ZK12

        '*** 修正 maeda 2008/05/12************************************************
        '帳票出力項目用に金融機関名、店舗名を追加
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ZEIKIN350_REC2.ZK3
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ZEIKIN350_REC2.ZK5
        '*************************************************************************

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        Return "D"
    End Function

    Private Function CheckDBRecord2() As String
        ' データチェック
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
        ZEIKIN350_REC8.Data = RecordData

        ' 明細マスタ項目設定 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC8.ZK1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK4)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK5)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK6)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK7)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = ZEIKIN350_REC8.ZK2
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = ZEIKIN350_REC8.ZK3
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = ZEIKIN350_REC8.ZK4
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = ZEIKIN350_REC8.ZK5
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = ZEIKIN350_REC8.ZK6
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = ZEIKIN350_REC8.ZK7

        Return "T"
    End Function

    Private Function CheckDBRecord8() As String

        ' データチェック
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
        ZEIKIN350_REC9.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC9.ZK1

        Return "E"
    End Function

    ' 機能　 ： 返還データレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overrides Sub GetHenkanDataRecord()
        If IsDataRecord() = False Then
            Return
        End If

        ZEIKIN350_REC2.Data = RecordData

        ' 振替結果をセット
        ZEIKIN350_REC2.ZK12 = InfoMeisaiMast.FURIKETU_KEKKA

        '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
        'バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(91, ZEIKIN350_REC2.ZK12)
        End If
        '**********************************************

        RecordData = ZEIKIN350_REC2.Data

        ' レコードデータを分析
        Call CheckRecord2()

        Call MyBase.GetHenkanDataRecord()
    End Sub

    ' 機能　 ： 返還トレーラレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overrides Sub GetHenkanTrailerRecord()
        If IsTrailerRecord() = False Then
            Return
        End If

        ' レコードデータを分析
        Call CheckRecord8()

        ' 振替済み件数をセット
        ZEIKIN350_REC8.ZK4 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, "0"c)
        ZEIKIN350_REC8.ZK5 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        ' 振替不能件数をセット
        ZEIKIN350_REC8.ZK6 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
        ZEIKIN350_REC8.ZK7 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)

        '*** 修正 mitsu 2008/09/30 EBCDICデータ対応 ***
        'バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(19, ZEIKIN350_REC8.ZK4)
            ReadByteBin.Insert(25, ZEIKIN350_REC8.ZK5)
            ReadByteBin.Insert(37, ZEIKIN350_REC8.ZK6)
            ReadByteBin.Insert(43, ZEIKIN350_REC8.ZK7)
        End If
        '**********************************************

        RecordData = ZEIKIN350_REC8.Data

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
End Class
