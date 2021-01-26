Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' ＮＨＫ データフォーマットクラス
Public Class CFormatNHK
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 120

    '------------------------------------------
    'ＮＨＫフォーマット
    '------------------------------------------
    'ヘッダレコード
    Public Structure NHKRECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    ' データ区分(=1)
        <VBFixedString(2)> Public NH02 As String    ' 種別コード
        <VBFixedString(1)> Public NH03 As String    ' コード区分
        <VBFixedString(10)> Public NH04 As String   ' 委託者コード
        <VBFixedString(40)> Public NH05 As String   ' 委託者名
        <VBFixedString(4)> Public NH06 As String    ' 振替月日
        <VBFixedString(4)> Public NH07 As String    ' 金融機関コード
        <VBFixedString(15)> Public NH08 As String   ' ダミー
        <VBFixedString(3)> Public NH09 As String    ' ダミー
        <VBFixedString(15)> Public NH10 As String   ' ダミー
        <VBFixedString(8)> Public NH11 As String    ' ダミー
        <VBFixedString(9)> Public NH12 As String    ' ダミー
        <VBFixedString(3)> Public NH13 As String    ' ダミー
        <VBFixedString(4)> Public NH14 As String    ' 請求機関コード
        <VBFixedString(1)> Public NH15 As String    ' アスタリスク
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 2), _
                            SubData(NH03, 1), _
                            SubData(NH04, 10), _
                            SubData(NH05, 40), _
                            SubData(NH06, 4), _
                            SubData(NH07, 4), _
                            SubData(NH08, 15), _
                            SubData(NH09, 3), _
                            SubData(NH10, 15), _
                            SubData(NH11, 8), _
                            SubData(NH12, 9), _
                            SubData(NH13, 3), _
                            SubData(NH14, 4), _
                            SubData(NH15, 1) _
                            })
            End Get
            Set(ByVal value As String)
                NH01 = CuttingData(value, 1)
                NH02 = CuttingData(value, 2)
                NH03 = CuttingData(value, 1)
                NH04 = CuttingData(value, 10)
                NH05 = CuttingData(value, 40)
                NH06 = CuttingData(value, 4)
                NH07 = CuttingData(value, 4)
                NH08 = CuttingData(value, 15)
                NH09 = CuttingData(value, 3)
                NH10 = CuttingData(value, 15)
                NH11 = CuttingData(value, 8)
                NH12 = CuttingData(value, 9)
                NH13 = CuttingData(value, 3)
                NH14 = CuttingData(value, 4)
                NH15 = CuttingData(value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC1 As NHKRECORD1

    'データレコード
    Structure NHKRECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    ' データ区分(=2)
        <VBFixedString(4)> Public NH02 As String    ' 統一金融機関コード
        <VBFixedString(15)> Public NH03 As String   ' ダミーエリア
        <VBFixedString(3)> Public NH04 As String    ' 統一金融機関店舗コード
        <VBFixedString(1)> Public NH05 As String    ' 新口座番号    科目
        <VBFixedString(7)> Public NH06 As String    '               通帳番号
        <VBFixedString(11)> Public NH07 As String   ' ダミー
        <VBFixedString(1)> Public NH08 As String    ' 口座番号      科目
        <VBFixedString(7)> Public NH09 As String    '               通帳番号
        <VBFixedString(10)> Public NH10 As String   ' 預金者名      通帳名
        <VBFixedString(5)> Public NH11 As String    '               ダミー
        <VBFixedString(15)> Public NH12 As String   ' 契約者名
        <VBFixedString(10)> Public NH13 As String   ' 請求金額
        <VBFixedString(1)> Public NH14 As String    ' 変更識別
        <VBFixedString(4)> Public NH15 As String    ' 局・地域番号  局
        <VBFixedString(7)> Public NH16 As String    '               地域番号
        <VBFixedString(7)> Public NH17 As String    ' 発行番号
        <VBFixedString(2)> Public NH18 As String    ' ダミー
        <VBFixedString(1)> Public NH19 As String    ' 振替不能理由
        <VBFixedString(3)> Public NH20 As String    ' ダミー
        <VBFixedString(4)> Public NH21 As String    ' 請求機関コード
        <VBFixedString(1)> Public NH22 As String    ' アスタリスク
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 4), _
                            SubData(NH03, 15), _
                            SubData(NH04, 3), _
                            SubData(NH05, 1), _
                            SubData(NH06, 7), _
                            SubData(NH07, 11), _
                            SubData(NH08, 1), _
                            SubData(NH09, 7), _
                            SubData(NH10, 10), _
                            SubData(NH11, 5), _
                            SubData(NH12, 15), _
                            SubData(NH13, 10), _
                            SubData(NH14, 1), _
                            SubData(NH15, 4), _
                            SubData(NH16, 7), _
                            SubData(NH17, 7), _
                            SubData(NH18, 2), _
                            SubData(NH19, 1), _
                            SubData(NH20, 3), _
                            SubData(NH21, 4), _
                            SubData(NH22, 1) _
                            })
            End Get
            Set(ByVal Value As String)
                NH01 = CuttingData(Value, 1)
                NH02 = CuttingData(Value, 4)
                NH03 = CuttingData(Value, 15)
                NH04 = CuttingData(Value, 3)
                NH05 = CuttingData(Value, 1)
                NH06 = CuttingData(Value, 7)
                NH07 = CuttingData(Value, 11)
                NH08 = CuttingData(Value, 1)
                NH09 = CuttingData(Value, 7)
                NH10 = CuttingData(Value, 10)
                NH11 = CuttingData(Value, 5)
                NH12 = CuttingData(Value, 15)
                NH13 = CuttingData(Value, 10)
                NH14 = CuttingData(Value, 1)
                NH15 = CuttingData(Value, 4)
                NH16 = CuttingData(Value, 7)
                NH17 = CuttingData(Value, 7)
                NH18 = CuttingData(Value, 2)
                NH19 = CuttingData(Value, 1)
                NH20 = CuttingData(Value, 3)
                NH21 = CuttingData(Value, 4)
                NH22 = CuttingData(Value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC2 As NHKRECORD2

    'トレーラレコード
    Structure NHKRECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    ' データ区分(=8)
        <VBFixedString(6)> Public NH02 As String    ' 請求件数
        <VBFixedString(12)> Public NH03 As String   ' 請求金額
        <VBFixedString(6)> Public NH04 As String    ' 振替済件数
        <VBFixedString(12)> Public NH05 As String   ' 振替済金額
        <VBFixedString(6)> Public NH06 As String    ' 振替不能件数
        <VBFixedString(12)> Public NH07 As String   ' 振替不能金額
        <VBFixedString(12)> Public NH08 As String   ' 振替手数料
        <VBFixedString(12)> Public NH09 As String   ' 入金額
        <VBFixedString(6)> Public NH10 As String    ' レコード数
        <VBFixedString(6)> Public NH11 As String    ' 変更識別件数内訳  有金額
        <VBFixedString(6)> Public NH12 As String    '                   金額「0」
        <VBFixedString(15)> Public NH13 As String   ' ダミー
        <VBFixedString(3)> Public NH14 As String    ' トレーラ識別
        <VBFixedString(4)> Public NH15 As String    ' 予備
        <VBFixedString(1)> Public NH16 As String    ' アスタリスク
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 6), _
                            SubData(NH03, 12), _
                            SubData(NH04, 6), _
                            SubData(NH05, 12), _
                            SubData(NH06, 6), _
                            SubData(NH07, 12), _
                            SubData(NH08, 12), _
                            SubData(NH09, 12), _
                            SubData(NH10, 6), _
                            SubData(NH11, 6), _
                            SubData(NH12, 6), _
                            SubData(NH13, 15), _
                            SubData(NH14, 3), _
                            SubData(NH15, 4), _
                            SubData(NH16, 1) _
                            })
            End Get
            Set(ByVal Value As String)
                NH01 = CuttingData(Value, 1)
                NH02 = CuttingData(Value, 6)
                NH03 = CuttingData(Value, 12)
                NH04 = CuttingData(Value, 6)
                NH05 = CuttingData(Value, 12)
                NH06 = CuttingData(Value, 6)
                NH07 = CuttingData(Value, 12)
                NH08 = CuttingData(Value, 12)
                NH09 = CuttingData(Value, 12)
                NH10 = CuttingData(Value, 6)
                NH11 = CuttingData(Value, 6)
                NH12 = CuttingData(Value, 6)
                NH13 = CuttingData(Value, 15)
                NH14 = CuttingData(Value, 3)
                NH15 = CuttingData(Value, 4)
                NH16 = CuttingData(Value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC8 As NHKRECORD8

    'エンドレコード
    Structure NHKRECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    'データ区分(=9)
        <VBFixedString(111)> Public NH02 As String  'ダミーエリア
        <VBFixedString(3)> Public NH03 As String    'ダミーエリア
        <VBFixedString(4)> Public NH04 As String    '請求機関コード
        <VBFixedString(1)> Public NH05 As String    'アスタリスク
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 111), _
                            SubData(NH03, 3), _
                            SubData(NH04, 4), _
                            SubData(NH05, 1) _
                            })
            End Get
            Set(ByVal Value As String)
                NH01 = CuttingData(Value, 1)
                NH02 = CuttingData(Value, 111)
                NH03 = CuttingData(Value, 3)
                NH04 = CuttingData(Value, 4)
                NH05 = CuttingData(Value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC9 As NHKRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "120.P"
        FtranIBMPfile = "120IBM.P"
        FtranIBMBinaryPfile = "120READ.P"

        CMTBlockSize = 1800

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
    '           置換対象文字は，不正文字にはならない
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(DataInfo.LenOfOneRec)
        Dim nRet As Long
        Dim RD() As Byte = EncdJ.GetBytes(RecordData)

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' ヘッダレコード
                buff.Append(EncdJ.GetString(RD, 0, 14))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 14, 40), -1))
                buff.Append(EncdJ.GetString(RD, 54, 66))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If
            Case "2"        ' データレコード
                buff.Append(EncdJ.GetString(RD, 0, 50))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30), -1))
                buff.Append(EncdJ.GetString(RD, 80, 40))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If

                nRet = CheckRegularStringVerA(RecordData, 50, 30)
                If nRet >= 0 Then
                    Return nRet
                End If

            Case "8"        ' トレーラ
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If
            Case "9"        ' エンド
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If
        End Select

        nRet = MyBase.CheckRegularString()
        Return nRet

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
                'エンドレコードが複数あってもOK
                If BeforeRecKbn <> "8" Then
                    If BeforeRecKbn <> "9" Then
                        DataInfo.Message = "ファイルレコード（エンド区分）異常"
                        mnErrorNumber = 1
                        Return "ERR"
                    Else
                        sRet = CheckRecord9()
                        sRet = "99"
                    End If
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
        NHK_REC1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = NHK_REC1.NH01             'データ区分
        InfoMeisaiMast.SYUBETU_CODE = NHK_REC1.NH02         '種別コード
        InfoMeisaiMast.CODE_KBN = NHK_REC1.NH03             'コード区分
        InfoMeisaiMast.ITAKU_CODE = NHK_REC1.NH04           '委託者コード
        InfoMeisaiMast.ITAKU_KNAME = NHK_REC1.NH05          '委託者名
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(NHK_REC1.NH06, "yyyyMMdd")     '振替月日
        InfoMeisaiMast.FURIKAE_DATE_MOTO = NHK_REC1.NH06    '振替月日（元データ）
        InfoMeisaiMast.ITAKU_KIN = NHK_REC1.NH07            '金融機関コード

        Return "H"
    End Function

    Public Function CheckDBRecord1() As String
        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        'データチェック
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        '全銀フォーマットかつ出金の時、摘要マスタ存在チェック
        '摘要区分=データ摘要の場合のみ取得 2007/08/08
        'If Not mInfoComm Is Nothing AndAlso mInfoComm.INFOToriMast.FMT_KBN_T = "00" AndAlso _
        '   mInfoComm.INFOToriMast.NS_KBN_T = "9" AndAlso mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
        '    mTekiyoData = GetTEKIYOUMAST(mInfoComm.INFOToriMast.TORIS_CODE_T, mInfoComm.INFOToriMast.TORIF_CODE_T)

        '    If mTekiyoData.Length = 0 Then
        '        WriteBLog("摘要マスタ", "未登録", "委託者コード：" & InfoMeisaiMast.ITAKU_CODE)
        '        DataInfo.Message = "摘要マスタ未登録　取引先コード:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
        '        Return "ERR"
        '    End If
        'End If

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
        NHK_REC2.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = NHK_REC2.NH01             'データ区分
        InfoMeisaiMast.KEIYAKU_KIN = NHK_REC2.NH02          '金融機関コード
        InfoMeisaiMast.KEIYAKU_SIT = NHK_REC2.NH04          '支店コード
        InfoMeisaiMast.KEIYAKU_KAMOKU = NHK_REC2.NH08                               '口座番号 科目
        InfoMeisaiMast.KEIYAKU_KOUZA = CStr(NHK_REC2.NH09).Trim.PadLeft(7, "0"c)    '口座番号 通帳番号
        InfoMeisaiMast.KEIYAKU_KNAME = NHK_REC2.NH10                                '預金者名   通帳名
        InfoMeisaiMast.SINKI_CODE = NHK_REC2.NH14           '変更識別　新規コード？
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(NHK_REC2.NH13)              '請求金額
        InfoMeisaiMast.FURIKIN_MOTO = NHK_REC2.NH13
        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(NHK_REC2.NH19)
        InfoMeisaiMast.FURIKETU_MOTO = NHK_REC2.NH19
        InfoMeisaiMast.JYUYOKA_NO = String.Concat(New String() {NHK_REC2.NH15, NHK_REC2.NH16, NHK_REC2.NH17, NHK_REC2.NH18})    '需要家番号？

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        '帳票出力項目用に金融機関名、店舗名を追加
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""

        Return "D"
    End Function

    Private Function CheckDBRecord2() As String
        Dim CheckRet As Boolean

        'データチェック
        CheckRet = MyBase.CheckDataRecord()

        ' 摘要
        InfoMeisaiMast.NTEKIYO = ""
        InfoMeisaiMast.KTEKIYO = ""
        Try
            If (Not mInfoComm Is Nothing) Then
                Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                    Case "0"
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                        'Case "2"
                        '    InfoMeisaiMast.KTEKIYO = NHK_REC2.ZG5.PadRight(13, " "c).Substring(0, 13).Trim
                        'Case "3"
                        '    InfoMeisaiMast.KTEKIYO = NHK_REC2.ZG15
                End Select
            End If
        Catch ex As Exception

        End Try

        If CheckRet = False Then
            Return "IJO"
        End If

        If mInfoComm Is Nothing Then
            Return "D"
        End If

        'If mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
        '    ' 摘要区分が，２：可変摘要１（支店名）の場合
        '    Dim CheckTekiyou As String
        '    CheckTekiyou = ZENGIN_REC2.ZG5.TrimEnd

        '    ' 明細マスタ項目設定 カナ摘要
        '    If CheckTekiyou.Length > 13 Then
        '        InfoMeisaiMast.KTEKIYO = CheckTekiyou.Substring(0, 13)
        '    Else
        '        InfoMeisaiMast.KTEKIYO = CheckTekiyou
        '    End If
        '    Dim bFoundFlag As Boolean = False
        '    For i As Integer = 0 To mTekiyoData.Length - 1
        '        If mTekiyoData(i).Length > 0 AndAlso CheckTekiyou.IndexOf(mTekiyoData(i)) >= 0 Then
        '            bFoundFlag = True
        '            Exit For
        '        End If
        '    Next i

        '    ' 明細マスタ項目設定 漢字摘要
        '    InfoMeisaiMast.NTEKIYO = ""
        '    If bFoundFlag = False Then
        '        ' 店番数値異常
        '        Dim InError As INPUTERROR
        '        InError.ERRINFO = "摘要 " & InfoMeisaiMast.KTEKIYO
        '        InErrorArray.Add(InError)
        '        Return "IJO"
        '    End If
        'End If

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
        NHK_REC8.Data = RecordData

        ' 明細マスタ項目設定 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = NHK_REC8.NH01
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(NHK_REC8.NH02)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(NHK_REC8.NH03)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(NHK_REC8.NH04)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(NHK_REC8.NH05)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(NHK_REC8.NH06)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(NHK_REC8.NH07)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = NHK_REC8.NH02
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = NHK_REC8.NH03
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = NHK_REC8.NH04
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = NHK_REC8.NH05
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = NHK_REC8.NH06
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = NHK_REC8.NH07

        Return "T"
    End Function

    Private Function CheckDBRecord8() As String
        'データチェック
        If MyBase.CheckTrailerRecord() = False Then
            Return "ERR"
        End If

        Return "T"
    End Function
    '
    '
    ' 機能　 ： エンドレコードチェック
    '
    ' 戻り値 ： True - 成功， False - 失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckRecord9() As String
        NHK_REC9.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = NHK_REC9.NH01

        'データチェック
        If MyBase.CheckEndRecord() = False Then
            Return "ERR"
        End If

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

        NHK_REC2.Data = RecordData

        ' 変更識別をセット
        NHK_REC2.NH19 = InfoMeisaiMast.FURIKETU_KEKKA

        ' EBCDICデータ対応 
        'バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, NHK_REC2.NH19)
        End If

        RecordData = NHK_REC2.Data

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
        '2010.03.02 start   'NHK対応
        NHK_REC8.NH04 = InfoMeisaiMast.TOTAL_NORM_KEN2.ToString.PadLeft(6, "0"c)
        'NHK_REC8.NH04 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, "0"c)
        '2010.03.02 end
        NHK_REC8.NH05 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        ' 振替不能件数をセット
        NHK_REC8.NH06 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
        NHK_REC8.NH07 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)

        '2010/03/02 振替手数料・入金額を設定する
        Dim TESUU As Double = 0
        Dim TANKA As Double
        Dim NHK As String = CASTCommon.GetFSKJIni("COMMON", "NHKTESUU").ToUpper
        Select Case NHK
            Case "ERR", Nothing
                TANKA = 10.5
            Case Else
                TANKA = CDbl(NHK)
        End Select
        TESUU = Math.Floor(TANKA * InfoMeisaiMast.TOTAL_NORM_KEN2)
        ' 振替手数料をセット
        NHK_REC8.NH08 = TESUU.ToString.PadLeft(12, "0"c)
        '入金額をセット
        If InfoMeisaiMast.TOTAL_NORM_KIN - TESUU >= 0 Then
            NHK_REC8.NH09 = (InfoMeisaiMast.TOTAL_NORM_KIN - TESUU).ToString.PadLeft(12, "0"c)
        Else
            '手数料が振替金額よりも大きい場合の仮対応
            NHK_REC8.NH09 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        End If
        '=============================================

        ' EBCDICデータ対応
        'バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(19, NHK_REC8.NH04)
            ReadByteBin.Insert(25, NHK_REC8.NH05)
            ReadByteBin.Insert(37, NHK_REC8.NH06)
            ReadByteBin.Insert(43, NHK_REC8.NH07)
            '2010/03/02 振替手数料・入金額を設定する
            ReadByteBin.Insert(55, NHK_REC8.NH08)
            ReadByteBin.Insert(67, NHK_REC8.NH09)
            '======================================
        End If

        RecordData = NHK_REC8.Data

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
End Class
