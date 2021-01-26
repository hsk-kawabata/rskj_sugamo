Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' 全銀 データフォーマットクラス
Public Class CFormatZengin
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 120

    '------------------------------------------
    '全銀フォーマット
    '------------------------------------------
    'ヘッダレコード
    Public Structure ZGRECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    ' データ区分(=1)
        <VBFixedString(2)> Public ZG2 As String    ' 種別コード
        <VBFixedString(1)> Public ZG3 As String    ' コード区分
        <VBFixedString(10)> Public ZG4 As String   ' 会社コード            振込依頼人コード
        <VBFixedString(40)> Public ZG5 As String   ' 依頼人名              振込依頼人名
        <VBFixedString(4)> Public ZG6 As String    ' 振替指定日(月日）     取扱日
        <VBFixedString(4)> Public ZG7 As String    ' 取引金融機関コード    仕向銀行ｺｰﾄﾞ
        <VBFixedString(15)> Public ZG8 As String   ' 取引金融機関名        仕向銀行名
        <VBFixedString(3)> Public ZG9 As String    ' 取引店舗コード        仕向支店ｺｰﾄﾞ
        <VBFixedString(15)> Public ZG10 As String  ' 取引店舗名            仕向支店名
        <VBFixedString(1)> Public ZG11 As String   ' 取引店舗名種目        預金種目
        <VBFixedString(7)> Public ZG12 As String   ' 依頼人口座番号        口座番号
        <VBFixedString(17)> Public ZG13 As String  ' ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 2),
                            SubData(ZG3, 1),
                            SubData(ZG4, 10),
                            SubData(ZG5, 40),
                            SubData(ZG6, 4),
                            SubData(ZG7, 4),
                            SubData(ZG8, 15),
                            SubData(ZG9, 3),
                            SubData(ZG10, 15),
                            SubData(ZG11, 1),
                            SubData(ZG12, 7),
                            SubData(ZG13, 17)
                            })
            End Get
            Set(ByVal value As String)
                ZG1 = CuttingData(value, 1)
                ZG2 = CuttingData(value, 2)
                '***修正 maeda 2008/05/21******************************
                '種別コードが41,43,44,45→21に変更する
                Select Case ZG2
                    Case "41", "43", "44", "45"
                        ZG2 = "21"
                    Case Else
                End Select
                '******************************************************
                ZG3 = CuttingData(value, 1)
                ZG4 = CuttingData(value, 10)
                ZG5 = CuttingData(value, 40)
                ZG6 = CuttingData(value, 4)
                ZG7 = CuttingData(value, 4)
                ZG8 = CuttingData(value, 15)
                ZG9 = CuttingData(value, 3)
                ZG10 = CuttingData(value, 15)
                ZG11 = CuttingData(value, 1)
                ZG12 = CuttingData(value, 7)
                ZG13 = CuttingData(value, 17)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC1 As ZGRECORD1

    'データレコード
    Structure ZGRECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    ' データ区分(=2)
        <VBFixedString(4)> Public ZG2 As String    ' 引落金融機関コード		被仕向銀行番号
        <VBFixedString(15)> Public ZG3 As String   ' 引落金融機関名         被仕向銀行名　
        <VBFixedString(3)> Public ZG4 As String    ' 引落店舗コード         被仕向支店番号
        <VBFixedString(15)> Public ZG5 As String   ' 引落店舗名             被仕向支店名
        <VBFixedString(4)> Public ZG6 As String    ' ダミーエリア           手形交換所番号
        <VBFixedString(1)> Public ZG7 As String    ' 預金種目               預金種目
        <VBFixedString(7)> Public ZG8 As String    ' 口座番号               口座番号
        <VBFixedString(30)> Public ZG9 As String   ' 預金者氏名             受取人
        <VBFixedString(10)> Public ZG10 As String  ' 引落金額               振込金額
        <VBFixedString(1)> Public ZG11 As String   ' 新規コード             新規コード
        <VBFixedString(10)> Public ZG12 As String  ' 顧客番号               顧客コード１
        <VBFixedString(10)> Public ZG13 As String  '				        顧客コード２
        <VBFixedString(1)> Public ZG14 As String   ' 振替結果コード         振込指定区分
        <VBFixedString(8)> Public ZG15 As String   ' ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 4),
                            SubData(ZG3, 15),
                            SubData(ZG4, 3),
                            SubData(ZG5, 15),
                            SubData(ZG6, 4),
                            SubData(ZG7, 1),
                            SubData(ZG8, 7),
                            SubData(ZG9, 30),
                            SubData(ZG10, 10),
                            SubData(ZG11, 1),
                            SubData(ZG12, 10),
                            SubData(ZG13, 10),
                            SubData(ZG14, 1),
                            SubData(ZG15, 8)
                            })
            End Get
            Set(ByVal Value As String)
                ZG1 = CuttingData(Value, 1)
                ZG2 = CuttingData(Value, 4)
                ZG3 = CuttingData(Value, 15)
                ZG4 = CuttingData(Value, 3)
                ZG5 = CuttingData(Value, 15)
                ZG6 = CuttingData(Value, 4)
                ZG7 = CuttingData(Value, 1)
                ZG8 = CuttingData(Value, 7)
                ZG9 = CuttingData(Value, 30)
                ZG10 = CuttingData(Value, 10)
                ZG11 = CuttingData(Value, 1)
                ZG12 = CuttingData(Value, 10)
                ZG13 = CuttingData(Value, 10)
                ZG14 = CuttingData(Value, 1)
                ZG15 = CuttingData(Value, 8)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC2 As ZGRECORD2

    'トレーラレコード
    Structure ZGRECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    'データ区分(=8)
        <VBFixedString(6)> Public ZG2 As String    ' 合計件数
        <VBFixedString(12)> Public ZG3 As String   ' 合計金額
        <VBFixedString(6)> Public ZG4 As String    ' 振替済件数
        <VBFixedString(12)> Public ZG5 As String   ' 振替済金額
        <VBFixedString(6)> Public ZG6 As String    ' 振替不能件数
        <VBFixedString(12)> Public ZG7 As String   ' 振替不能金額
        <VBFixedString(65)> Public ZG8 As String    'ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 6),
                            SubData(ZG3, 12),
                            SubData(ZG4, 6),
                            SubData(ZG5, 12),
                            SubData(ZG6, 6),
                            SubData(ZG7, 12),
                            SubData(ZG8, 65)
                            })
            End Get
            Set(ByVal Value As String)
                ZG1 = CuttingData(Value, 1)
                ZG2 = CuttingData(Value, 6)
                ZG3 = CuttingData(Value, 12)
                ZG4 = CuttingData(Value, 6)
                ZG5 = CuttingData(Value, 12)
                ZG6 = CuttingData(Value, 6)
                ZG7 = CuttingData(Value, 12)
                ZG8 = CuttingData(Value, 65)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC8 As ZGRECORD8

    'エンドレコード
    Structure ZGRECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    'データ区分(=9)
        <VBFixedString(119)> Public ZG2 As String  'ダミー数
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 119)
                            })
            End Get
            Set(ByVal Value As String)
                ZG1 = CuttingData(Value, 1)
                ZG2 = CuttingData(Value, 119)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC9 As ZGRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- START
        DataInfo.WorkLen = 120
        '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- END

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

    '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- START
    Public Sub New(ByVal len As Integer)
        '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ START
        Me.New()
        'MyBase.New()
        '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ END

        ' レコード長指定
        DataInfo.RecoedLen = len

    End Sub
    '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- END


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

        '2008/06/12 浜松信金用　TXT管理の文字以外は異常と判定する。（要検討）================================
        Select Case RecordData.Substring(0, 1)
            Case "1"        ' ヘッダレコード
                buff.Append(EncdJ.GetString(RD, 0, 14))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 14, 40), -1))
                buff.Append(EncdJ.GetString(RD, 54, 8))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 62, 15), -1))
                buff.Append(EncdJ.GetString(RD, 77, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 80, 15), -1))
                '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ START
                Dim intS As Integer = RecordLen - DataInfo.LenOfOneRec
                buff.Append(EncdJ.GetString(RD, 95, 25 - intS))
                'buff.Append(EncdJ.GetString(RD, 95, 25))
                '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ END
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ START
                    mRecordData = buff.ToString(0, DataInfo.LenOfOneRec)
                    'mRecordData = buff.ToString(0, RecordLen)
                    '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ END
                End If
            Case "2"        ' データレコード
                buff.Append(EncdJ.GetString(RD, 0, 5))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 5, 15), -1))
                buff.Append(EncdJ.GetString(RD, 20, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 23, 15), -1))
                buff.Append(EncdJ.GetString(RD, 38, 12))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30), -1))
                buff.Append(EncdJ.GetString(RD, 80, 11))    '引き落とし金額、新規コード
                buff.Append(ReplaceString(EncdJ.GetString(RD, 91, 20), -1)) '顧客コード
                '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ START
                Dim intS As Integer = RecordLen - DataInfo.LenOfOneRec
                buff.Append(EncdJ.GetString(RD, 111, 9 - intS)) '振替結果コード、ダミー
                'buff.Append(EncdJ.GetString(RD, 111, 9)) '振替結果コード、ダミー
                '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ END
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ START
                    mRecordData = buff.ToString(0, DataInfo.LenOfOneRec)
                    'mRecordData = buff.ToString(0, RecordLen)
                    '2018/03/05 タスク）西野 CHG 標準版修正：広島信金対応（不具合修正）------------------------ END
                End If

                nRet = CheckRegularStringVerA(RecordData, 5, 15)
                If nRet >= 0 Then
                    Return nRet
                End If
                nRet = CheckRegularStringVerA(RecordData, 23, 15)
                If nRet >= 0 Then
                    Return nRet
                End If
                nRet = CheckRegularStringVerA(RecordData, 50, 30)
                If nRet >= 0 Then
                    Return nRet
                End If
                nRet = CheckRegularStringVerA(RecordData, 91, 20)
                If nRet >= 0 Then
                    Return nRet
                End If
                If nRet >= 0 Then

                    If nRet = 118 AndAlso RecordLen = 120 Then
                        If mRecordData.Substring(118) = Environment.NewLine Then
                            Return -1
                        End If

                    ElseIf nRet = 119 AndAlso RecordLen = 120 Then
                        If mRecordData.Substring(119) = vbCr OrElse
                           mRecordData.Substring(119) = vbLf Then
                            Return -1
                        End If

                    End If
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
        If nRet = 118 AndAlso RecordLen = 120 Then
            If mRecordData.Substring(118) = Environment.NewLine Then
                Return -1
            End If
        ElseIf nRet = 119 AndAlso RecordLen = 120 Then
            If mRecordData.Substring(119) = vbCr OrElse
               mRecordData.Substring(119) = vbLf Then
                Return -1
            End If
        End If
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

            Case ChrW(&H1A) '2010.01.19　1A 追加 start
                If BeforeRecKbn <> "9" Then
                    DataInfo.Message = "レコード区分異常（1A）異常"
                    mnErrorNumber = 1
                    Return "ERR"
                Else
                    sRet = "1A"
                End If
            '2010.01.19　1A 追加 end
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
                    If sRet <> "ERR" Then
                        If CheckTrailerRecordFunou() = False Then
                            sRet = "ERR"
                        End If
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
        Call MyBase.CheckDataFormatAfterFunou()

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
        ZENGIN_REC1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC1.ZG1
        InfoMeisaiMast.SYUBETU_CODE = ZENGIN_REC1.ZG2
        InfoMeisaiMast.CODE_KBN = ZENGIN_REC1.ZG3
        InfoMeisaiMast.ITAKU_CODE = ZENGIN_REC1.ZG4
        InfoMeisaiMast.ITAKU_KNAME = ZENGIN_REC1.ZG5
        '2017/12/12 タスク）西野 CHG 標準版修正：広島信金対応（振替日休日補正対応）---------------- START
        If INI_IRAI_KYUJITU_HOSEI = "1" Then
            '休日補正を行う
            InfoMeisaiMast.FURIKAE_DATE = HoseiFurikaeDate(ZENGIN_REC1.ZG6)
        Else
            InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZENGIN_REC1.ZG6, "yyyyMMdd")
        End If
        'InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZENGIN_REC1.ZG6, "yyyyMMdd")
        '2017/12/12 タスク）西野 CHG 標準版修正：広島信金対応（振替日休日補正対応）---------------- END
        InfoMeisaiMast.FURIKAE_DATE_MOTO = ZENGIN_REC1.ZG6
        InfoMeisaiMast.ITAKU_KIN = ZENGIN_REC1.ZG7
        InfoMeisaiMast.ITAKU_SIT = ZENGIN_REC1.ZG9
        InfoMeisaiMast.ITAKU_KAMOKU = ZENGIN_REC1.ZG11
        InfoMeisaiMast.ITAKU_KOUZA = ZENGIN_REC1.ZG12

        Return "H"
    End Function

    Public Function CheckDBRecord1() As String
        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        '2018/10/07 saitou 広島信金(RSV2標準) ADD （ヘッダ口座情報チェック対応） ------------------ START
        'ヘッダチェックを行うと、ヘッダの明細情報が取引先情報で上書きされるので、あらかじめ変数に格納する
        Dim strTSIT_NO As String = InfoMeisaiMast.ITAKU_SIT
        Dim strKAMOKU As String = InfoMeisaiMast.ITAKU_KAMOKU
        Dim strKOUZA As String = InfoMeisaiMast.ITAKU_KOUZA
        '2018/10/07 saitou 広島信金(RSV2標準) ADD ------------------------------------------------- END

        'データチェック
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        '2018/10/07 saitou 広島信金(RSV2標準) ADD （ヘッダ口座情報チェック対応） ------------------ START
        If OraDB Is Nothing Then
        Else
            If INI_S_KDBMAST_CHK = "1" AndAlso mInfoComm.INFOToriMast.FSYORI_KBN_T = "3" Then
                '2018/10/07 saitou 広島信金(RSV2標準) UPD （ヘッダ口座情報チェック対応） ------------------ START
                Dim bRet As Boolean = ChkKDBMAST(strTSIT_NO, strKAMOKU, strKOUZA)
                If bRet = False Then
                    Dim MSG As String = String.Format("支店コード：{0} 科目：{1} 口座：{2}", strTSIT_NO, strKAMOKU, strKOUZA)
                    WriteBLog("ヘッダ口座情報チェック", "口座なし", MSG)
                    DataInfo.Message = "ヘッダ口座情報チェック不一致 " & MSG

                    Dim InError As INPUTERROR = Nothing
                    InError.ERRINFO = "口座情報なし(ヘッダー)"
                    InErrorArray.Add(InError)

                    Return "IJO"
                End If
                'Dim bRet As Boolean = ChkKDBMAST(InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
                'If bRet = False Then
                '    Dim MSG As String = String.Format("支店コード：{0} 科目：{1} 口座：{2}", InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
                '    WriteBLog("ヘッダ口座情報チェック", "口座なし", MSG)
                '    DataInfo.Message = "ヘッダ口座情報チェック不一致 " & MSG

                '    Dim InError As INPUTERROR = Nothing
                '    InError.ERRINFO = "口座情報なし(ヘッダー)"
                '    InErrorArray.Add(InError)

                '    Return "IJO"
                'End If
                '2018/10/07 saitou 広島信金(RSV2標準) UPD ------------------------------------------------- END
            End If
        End If
        '2018/10/07 saitou 広島信金(RSV2標準) ADD ------------------------------------------------- END

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
        ZENGIN_REC2.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC2.ZG1
        InfoMeisaiMast.KEIYAKU_KIN = ZENGIN_REC2.ZG2
        InfoMeisaiMast.KEIYAKU_SIT = ZENGIN_REC2.ZG4
        InfoMeisaiMast.KEIYAKU_KAMOKU = ZENGIN_REC2.ZG7


        '全銀フォーマットの契約口座番号は前後空白の場合、0埋めする
        InfoMeisaiMast.KEIYAKU_KOUZA = CStr(ZENGIN_REC2.ZG8).Trim.PadLeft(7, "0"c)
        InfoMeisaiMast.KEIYAKU_KNAME = ZENGIN_REC2.ZG9
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(ZENGIN_REC2.ZG10.TrimStart)
        InfoMeisaiMast.FURIKIN_MOTO = ZENGIN_REC2.ZG10.TrimStart
        InfoMeisaiMast.SINKI_CODE = ZENGIN_REC2.ZG11
        InfoMeisaiMast.KEIYAKU_NO = ZENGIN_REC2.ZG12
        InfoMeisaiMast.JYUYOKA_NO = ZENGIN_REC2.ZG12 & ZENGIN_REC2.ZG13

        '帳票出力項目用に金融機関名、店舗名を取得
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ZENGIN_REC2.ZG3
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ZENGIN_REC2.ZG5
        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(ZENGIN_REC2.ZG14)
        InfoMeisaiMast.FURIKETU_MOTO = ZENGIN_REC2.ZG14

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        Return "D"
    End Function

    Private Function CheckDBRecord2() As String
        Dim CheckRet As Boolean

        'データチェック
        CheckRet = CheckDataRecord()

        ' 摘要
        InfoMeisaiMast.NTEKIYO = ""
        InfoMeisaiMast.KTEKIYO = ""
        Try
            If (Not mInfoComm Is Nothing) Then
                Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                    Case "0"
                        '2017/01/16 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                        'スリーエスの場合は摘要を編集する。
                        Select Case mInfoComm.INFOToriMast.FMT_KBN_T
                            Case "20", "21"
                                If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Length > 10 Then
                                    InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Substring(0, 10)
                                Else
                                    InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim
                                End If
                                InfoMeisaiMast.NTEKIYO = ""
                            Case Else
                                InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                                InfoMeisaiMast.NTEKIYO = ""
                        End Select
                    'InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                    'InfoMeisaiMast.NTEKIYO = ""
                    '2017/01/16 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case "2"
                        '2017/01/16 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                        'スリーエスの場合は摘要を編集する。
                        Select Case mInfoComm.INFOToriMast.FMT_KBN_T
                            Case "20", "21"
                                '依頼データにSK(が付いているかわからないので、依頼データのSK(は取り除き、個別でSK(を付加する
                                InfoMeisaiMast.KTEKIYO = "SK(" & ZENGIN_REC2.ZG5.Replace("SK(", "").PadRight(10, " "c).Substring(0, 10).Trim
                            Case Else
                                InfoMeisaiMast.KTEKIYO = ZENGIN_REC2.ZG5.PadRight(13, " "c).Substring(0, 13).Trim
                        End Select
                    'InfoMeisaiMast.KTEKIYO = ZENGIN_REC2.ZG5.PadRight(13, " "c).Substring(0, 13).Trim
                    '2017/01/16 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    Case "3"
                        InfoMeisaiMast.KTEKIYO = ZENGIN_REC2.ZG15
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
        ZENGIN_REC8.Data = RecordData

        ' 明細マスタ項目設定 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC8.ZG1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG4)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG5)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG6)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG7)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = ZENGIN_REC8.ZG2
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = ZENGIN_REC8.ZG3
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = ZENGIN_REC8.ZG4
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = ZENGIN_REC8.ZG5
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = ZENGIN_REC8.ZG6
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = ZENGIN_REC8.ZG7

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
    ' 機能　 ： エンドレコードチェック
    '
    ' 戻り値 ： True - 成功， False - 失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckRecord9() As String
        ZENGIN_REC9.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC9.ZG1

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

        ZENGIN_REC2.Data = RecordData

        If Not ToriData Is Nothing Then
            Dim ToriCode As String = ToriData.INFOToriMast.TORIS_CODE_T & ToriData.INFOToriMast.TORIF_CODE_T

            '複数取引先対応
            If ("," & KokuminNenkinTori).IndexOf(ToriCode) >= 1 Then

                '国民年金保険料の場合
                '金融機関名に訂正用店番，訂正用口番を店番と訂正用店番が一致しない場合にのみセットする
                If InfoMeisaiMast.TEISEI_SIT <> "000" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> "" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> InfoMeisaiMast.KEIYAKU_SIT Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(4, 3).Insert(4, InfoMeisaiMast.TEISEI_SIT)

                    'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(9, InfoMeisaiMast.TEISEI_SIT)
                    End If
                End If

                If InfoMeisaiMast.TEISEI_KOUZA <> "0000000" AndAlso InfoMeisaiMast.TEISEI_KOUZA <> "" Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(8, 7).Insert(8, InfoMeisaiMast.TEISEI_KOUZA)

                    'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(13, InfoMeisaiMast.TEISEI_KOUZA)
                    End If
                End If
            End If
        End If

        '振替結果をセット
        ZENGIN_REC2.ZG14 = InfoMeisaiMast.FURIKETU_KEKKA

        'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, ZENGIN_REC2.ZG14)
        End If

        RecordData = ZENGIN_REC2.Data

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
        ZENGIN_REC8.ZG4 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, "0"c)
        ZENGIN_REC8.ZG5 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        ' 振替不能件数をセット
        ZENGIN_REC8.ZG6 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
        ZENGIN_REC8.ZG7 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)

        'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(19, ZENGIN_REC8.ZG4)
            ReadByteBin.Insert(25, ZENGIN_REC8.ZG5)
            ReadByteBin.Insert(37, ZENGIN_REC8.ZG6)
            ReadByteBin.Insert(43, ZENGIN_REC8.ZG7)
        End If

        RecordData = ZENGIN_REC8.Data

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
    ' 機能　 ： 再振ヘッダレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overrides Sub GetSaifuriHeaderRecord(ByVal SAIFURI_DATE As String)
        If IsHeaderRecord() = False Then
            Return
        End If

        ' レコードデータを分析
        Call CheckRecord1()

        '再振日をセット
        ZENGIN_REC1.ZG6 = SAIFURI_DATE
        'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(54, ZENGIN_REC1.ZG6)
        End If

        RecordData = ZENGIN_REC1.Data

        Call MyBase.GetSaifuriHeaderRecord(SAIFURI_DATE)
    End Sub
    ' 機能　 ： 再振データレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overrides Sub GetSaifuriDataRecord()
        If IsDataRecord() = False Then
            Return
        End If

        ZENGIN_REC2.Data = RecordData

        If Not ToriData Is Nothing Then
            Dim ToriCode As String = ToriData.INFOToriMast.TORIS_CODE_T & ToriData.INFOToriMast.TORIF_CODE_T

            '複数取引先対応
            If ("," & KokuminNenkinTori).IndexOf(ToriCode) >= 1 Then

                '国民年金保険料の場合
                '金融機関名に訂正用店番，訂正用口番を店番と訂正用店番が一致しない場合にのみセットする
                If InfoMeisaiMast.TEISEI_SIT <> "000" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> "" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> InfoMeisaiMast.KEIYAKU_SIT Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(4, 3).Insert(4, InfoMeisaiMast.TEISEI_SIT)

                    'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(9, InfoMeisaiMast.TEISEI_SIT)
                    End If
                End If

                If InfoMeisaiMast.TEISEI_KOUZA <> "0000000" AndAlso InfoMeisaiMast.TEISEI_KOUZA <> "" Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(8, 7).Insert(8, InfoMeisaiMast.TEISEI_KOUZA)

                    'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(13, InfoMeisaiMast.TEISEI_KOUZA)
                    End If
                End If
            End If
        End If

        '振替結果をセット
        ZENGIN_REC2.ZG14 = InfoMeisaiMast.FURIKETU_KEKKA

        'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, ZENGIN_REC2.ZG14)
        End If
        RecordData = ZENGIN_REC2.Data

        ' レコードデータを分析
        Call CheckRecord2()
        Call MyBase.GetSaifuriDataRecord()

        'データレコードの振替結果を0にする
        '振替結果をセット
        ZENGIN_REC2.ZG14 = "0"

        'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, ZENGIN_REC2.ZG14)
        End If
        RecordData = ZENGIN_REC2.Data
    End Sub

    ' 機能　 ： 再振トレーラレコードを取得する
    '
    ' 戻り値 ： １レコード分のデータ
    '
    ' 備考　 ：
    '
    Public Overrides Sub GetSaifuriTrailerRecord(Optional ByVal SyoriKen As Long = 0, Optional ByVal SyoriKin As Long = 0,
                                                       Optional ByVal Write As Boolean = False)
        If IsTrailerRecord() = False Then
            Return
        End If

        ' レコードデータを分析
        Call CheckRecord8()

        ' 振替不能件数をセット
        If Write = True Then
            ZENGIN_REC8.ZG2 = SyoriKen.ToString.PadLeft(6, "0"c)
            ZENGIN_REC8.ZG3 = SyoriKin.ToString.PadLeft(12, "0"c)
        Else
            ZENGIN_REC8.ZG2 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
            ZENGIN_REC8.ZG3 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)
        End If

        ' 0をセット
        ZENGIN_REC8.ZG4 = "0".PadLeft(6, "0"c)
        ZENGIN_REC8.ZG5 = "0".PadLeft(12, "0"c)
        ZENGIN_REC8.ZG6 = "0".PadLeft(6, "0"c)
        ZENGIN_REC8.ZG7 = "0".PadLeft(12, "0"c)

        'EBCDICデータ対応：バイナリデータが存在する場合は書き換える
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(1, ZENGIN_REC8.ZG2)
            ReadByteBin.Insert(7, ZENGIN_REC8.ZG3)
            ReadByteBin.Insert(19, ZENGIN_REC8.ZG4)
            ReadByteBin.Insert(25, ZENGIN_REC8.ZG5)
            ReadByteBin.Insert(37, ZENGIN_REC8.ZG6)
            ReadByteBin.Insert(43, ZENGIN_REC8.ZG7)
        End If

        RecordData = ZENGIN_REC8.Data

        Call MyBase.GetSaifuriTrailerRecord(SyoriKin, SyoriKin, Write)
    End Sub

    '
    ' 機能　 ： レコードを読み込んでチェック（スリーエス用）
    '
    ' 戻り値 ： "H" - ヘッダ，"D" - データ，"T" - トレーラ，"E" - エンド
    '           "ERR" - エラーあり
    '
    ' 備考　 ： 2017/01/16 saitou 東春信金(RSV2標準) added for スリーエス対応
    '
    Public Overrides Function CheckKekkaFormatSSS() As String
        ' 基本クラス チェック
        Dim sRet As String = MyBase.CheckKekkaFormatSSS()

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
                    If sRet <> "ERR" Then
                        'ＳＳＳの不能データは不能分しか返ってこないため、データとトレーラの件数は合わない
                        'If CheckTrailerRecordFunou() = False Then
                        '    sRet = "ERR"
                        'End If
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
        Call MyBase.CheckDataFormatAfterFunou()

        Return sRet
    End Function

    ''' <summary>
    ''' 口座情報マスタに指定口座が存在するかチェックする
    ''' </summary>
    ''' <param name="astrSIT_NO">支店コード</param>
    ''' <param name="astrKAMOKU">科目コード</param>
    ''' <param name="astrKOUZA">口座番号</param>
    ''' <returns></returns>
    ''' <remarks>2018/10/07 saitou 広島信金(RSV2標準) added for ヘッダ口座チェック対応</remarks>
    Private Function ChkKDBMAST(ByVal astrSIT_NO As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String) As Boolean
        Dim SQL As New System.Text.StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            astrKAMOKU = CASTCommon.ConvertKamoku1TO2(astrKAMOKU)

            SQL.Append("SELECT TSIT_NO_D, KOUZA_D, IDOU_DATE_D FROM KDBMAST")
            SQL.Append(" WHERE TSIT_NO_D = '" & astrSIT_NO & "'")
            SQL.Append(" AND KAMOKU_D = '" & astrKAMOKU & "'")
            SQL.Append(" AND KOUZA_D = '" & astrKOUZA & "'")

            Return OraReader.DataReader(SQL)

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function

End Class
