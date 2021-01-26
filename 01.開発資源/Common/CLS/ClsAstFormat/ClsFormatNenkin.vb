Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' 年金振込 データフォーマットクラス
Public Class CFormatNenkin
    ' データフォーマット基本クラス
    Inherits CFormat

    ' データ長
    Private Shadows ReadOnly RecordLen As Integer = 130

    '振込支店変換用項目
    Private ReadOnly SIT_CODE As String = CASTCommon.GetFSKJIni("COMMON", "NENKIN_SIT") '読替失敗時のデフォルト支店
    Private ReadOnly SIT_CODE_TABLE As New CAstFormat.ClsTextEqual("支店コード.TXT")    '振込支店読替テーブル

    '------------------------------------------
    '年金振込フォーマット
    '------------------------------------------
    'ヘッダレコード
    '   Public Structure MKRECORD1  2009/09/30 kakiwaki
    Public Structure NKRECORD1
        Implements CFormat.IFormat

        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(2)> Public NK2 As String     '年金種別
        <VBFixedString(1)> Public NK3 As String     'コード区分
        <VBFixedString(10)> Public NK4 As String    '日本銀行コード
        <VBFixedString(20)> Public NK5 As String    '日本銀行名
        <VBFixedString(2)> Public NK6 As String     '日本銀行振込依頼店コード
        <VBFixedString(18)> Public NK7 As String    '日本銀行振込依頼店名
        <VBFixedString(6)> Public NK8 As String     '振込依頼日
        <VBFixedString(4)> Public NK9 As String     '依頼先金融機関コード
        <VBFixedString(14)> Public NK10 As String   '依頼先金融機関名
        <VBFixedString(14)> Public NK11 As String   'ダミー
        <VBFixedString(21)> Public NK12 As String   '依頼先金融機関店舗名
        <VBFixedString(10)> Public NK13 As String   '振込請求官庁名
        <VBFixedString(6)> Public NK14 As String    'ダミー

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 2), _
                            SubData(NK3, 1), _
                            SubData(NK4, 10), _
                            SubData(NK5, 20), _
                            SubData(NK6, 2), _
                            SubData(NK7, 18), _
                            SubData(NK8, 6), _
                            SubData(NK9, 4), _
                            SubData(NK10, 14), _
                            SubData(NK11, 14), _
                            SubData(NK12, 21), _
                            SubData(NK13, 10), _
                            SubData(NK14, 6) _
                            })
            End Get
            Set(ByVal value As String)
                NK1 = CuttingData(value, 2)
                NK2 = CuttingData(value, 2)
                NK3 = CuttingData(value, 1)
                NK4 = CuttingData(value, 10)
                NK5 = CuttingData(value, 20)
                NK6 = CuttingData(value, 2)
                NK7 = CuttingData(value, 18)
                NK8 = CuttingData(value, 6)
                NK9 = CuttingData(value, 4)
                NK10 = CuttingData(value, 14)
                NK11 = CuttingData(value, 14)
                NK12 = CuttingData(value, 21)
                NK13 = CuttingData(value, 10)
                NK14 = CuttingData(value, 6)
            End Set
        End Property
    End Structure
    '   Public NENKIN_REC1 As MKRECORD1     2009/09/30 kakiwaki
    Public NENKIN_REC1 As NKRECORD1

    'データレコード
    Structure NKRECORD2
        Implements CFormat.IFormat
        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(7)> Public NK2 As String     '整理番号
        <VBFixedString(6)> Public NK3 As String     'ダミー
        <VBFixedString(4)> Public NK4 As String     '振込先金融機関コード
        <VBFixedString(14)> Public NK5 As String    '振込先金融機関名
        <VBFixedString(11)> Public NK6 As String    'ダミー
        <VBFixedString(3)> Public NK7 As String     '振込店番
        <VBFixedString(21)> Public NK8 As String    '振込先店舗名
        <VBFixedString(1)> Public NK9 As String     '振込科目
        <VBFixedString(10)> Public NK10 As String   '振込先口座番号
        <VBFixedString(25)> Public NK11 As String   '受取人氏名
        <VBFixedString(8)> Public NK12 As String    '金額
        <VBFixedString(15)> Public NK13 As String   '年金証書番号
        <VBFixedString(3)> Public NK14 As String    'ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 7), _
                            SubData(NK3, 6), _
                            SubData(NK4, 4), _
                            SubData(NK5, 14), _
                            SubData(NK6, 11), _
                            SubData(NK7, 3), _
                            SubData(NK8, 21), _
                            SubData(NK9, 1), _
                            SubData(NK10, 010), _
                            SubData(NK11, 025), _
                            SubData(NK12, 08), _
                            SubData(NK13, 015), _
                            SubData(NK14, 03) _
                            })
            End Get
            Set(ByVal Value As String)
                NK1 = CuttingData(Value, 2)
                NK2 = CuttingData(Value, 7)
                NK3 = CuttingData(Value, 6)
                NK4 = CuttingData(Value, 4)
                NK5 = CuttingData(Value, 14)
                NK6 = CuttingData(Value, 11)
                NK7 = CuttingData(Value, 3)
                NK8 = CuttingData(Value, 21)
                NK9 = CuttingData(Value, 1)
                NK10 = CuttingData(Value, 10)
                NK11 = CuttingData(Value, 25)
                NK12 = CuttingData(Value, 8)
                NK13 = CuttingData(Value, 15)
                NK14 = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
    Public NENKIN_REC2 As NKRECORD2

    'トレーラレコード
    Structure NKRECORD8
        Implements CFormat.IFormat
        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(8)> Public NK2 As String     '合計件数
        <VBFixedString(13)> Public NK3 As String    '合計金額
        <VBFixedString(107)> Public NK4 As String   'ダミー
        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 8), _
                            SubData(NK3, 13), _
                            SubData(NK4, 107) _
                            })
            End Get
            Set(ByVal Value As String)
                NK1 = CuttingData(Value, 2)
                NK2 = CuttingData(Value, 8)
                NK3 = CuttingData(Value, 13)
                NK4 = CuttingData(Value, 107)
            End Set
        End Property
    End Structure
    Public NENKIN_REC8 As NKRECORD8

    'エンドレコード
    Structure NKRECORD9
        Implements CFormat.IFormat
        <VBFixedString(2)> Public NK1 As String     'データ区分
        <VBFixedString(8)> Public NK2 As String     '総合計件数
        <VBFixedString(13)> Public NK3 As String    '総合計金額
        <VBFixedString(107)> Public NK4 As String   'ダミー

        '固定長データ処理用プロパティ
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 8), _
                            SubData(NK3, 13), _
                            SubData(NK4, 107) _
                            })
            End Get
            Set(ByVal Value As String)
                NK1 = CuttingData(Value, 2)
                NK2 = CuttingData(Value, 8)
                NK3 = CuttingData(Value, 13)
                NK4 = CuttingData(Value, 107)
            End Set
        End Property
    End Structure
    Public NENKIN_REC9 As NKRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' レコード長指定
        DataInfo.RecoedLen = RecordLen

        ' レコード区分
        DataInfo.MinRecordCode = New String() {"1", "2", "8", "9"}

        FtranPfile = "130.P"
        FtranIBMPfile = "130IBM.P"
        FtranIBMBinaryPfile = "130READ.P"

        CMTBlockSize = 1300

        HeaderKubun = New String() {"10", "11"}
        DataKubun = New String() {"23"}
        TrailerKubun = New String() {"83"}
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
        Dim buff As New StringBuilder(RecordLen)

        Select Case RecordData.Substring(0, 1)
            Case "10", "11"         ' ヘッダレコード
            Case "23"               ' データレコード
            Case "81", "82", "83"   ' トレーラ
            Case "90"               ' エンド
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

        Select Case RecordData.Substring(0, 2)
            Case "10", "11"
                sRet = CheckRecord1()
            Case "23"
                sRet = CheckRecord2()
            Case "81", "82"
                sRet = CheckRecord81_82()
            Case "83"
                sRet = CheckRecord8()
            Case Else
                Select Case RecordData.Substring(0, 1)
                    Case "9"
                        sRet = CheckRecord9()
                    Case Else
                        DataInfo.Message = "レコード区分異常（" & RecordData.Substring(0, 2) & "）異常"
                        mnErrorNumber = 1
                        Return "ERR"
                End Select
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
        NENKIN_REC1.Data = RecordData

        ' 明細マスタ情報初期化
        Call InfoMeisaiMast.Init()

        ' 明細マスタ項目設定
        InfoMeisaiMast.DATA_KBN = "1"
        InfoMeisaiMast.SYUBETU_CODE = NENKIN_REC1.NK2
        InfoMeisaiMast.CODE_KBN = NENKIN_REC1.NK3
        InfoMeisaiMast.ITAKU_CODE = mInfoComm.INFOToriMast.ITAKU_CODE_T
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(NENKIN_REC1.NK8, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = NENKIN_REC1.NK8

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        Dim ToriCode As String = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & InfoMeisaiMast.SYUBETU_CODE)
        If ToriCode <> "err" Then
            Call GetTorimastFromToriCode(ToriCode, OraDB)
        End If

        'データチェック（年金振込み用）
        If CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        InfoMeisaiMast.ITAKU_KNAME = ""

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
        NENKIN_REC2.Data = RecordData

        '明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "2"
        InfoMeisaiMast.KEIYAKU_KIN = NENKIN_REC2.NK4
        InfoMeisaiMast.KEIYAKU_SIT = NENKIN_REC2.NK7
        InfoMeisaiMast.KEIYAKU_KAMOKU = NENKIN_REC2.NK9
        If InfoMeisaiMast.KEIYAKU_KAMOKU.Trim = "" Then
            InfoMeisaiMast.KEIYAKU_KAMOKU = "1"
        End If
        InfoMeisaiMast.KEIYAKU_KOUZA = NENKIN_REC2.NK10.Substring(3, 7)
        InfoMeisaiMast.KEIYAKU_KNAME = NENKIN_REC2.NK11
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(NENKIN_REC2.NK12)
        InfoMeisaiMast.FURIKIN_MOTO = NENKIN_REC2.NK12
        InfoMeisaiMast.SINKI_CODE = "0"
        InfoMeisaiMast.KEIYAKU_NO = New String("0"c, 10)
        InfoMeisaiMast.JYUYOKA_NO = (InfoMeisaiMast.SYUBETU_CODE & NENKIN_REC2.NK13).PadRight(20, " "c)

        InfoMeisaiMast.FURIKETU_CODE = 0
        InfoMeisaiMast.FURIKETU_MOTO = "0"

        InfoMeisaiMast.YOBI1 = Cutting(NENKIN_REC2.NK8, 15)
        NENKIN_REC2.NK8 = InfoMeisaiMast.YOBI1
        InfoMeisaiMast.YOBI2 = NENKIN_REC2.NK2
        InfoMeisaiMast.YOBI3 = NENKIN_REC2.NK13.Trim

        Try
            If (Not mInfoComm Is Nothing) Then
                Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                    Case "0"
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case Else
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                End Select
            End If
        Catch ex As Exception
        End Try

        '帳票出力項目用に金融機関名、店舗名を追加
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = NENKIN_REC2.NK5
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = NENKIN_REC2.NK8

        ' 依頼件数，依頼金額 カウント対象レコード
        InfoMeisaiMast.FURIKEN = 1

        '支店コードが無い場合は支店名から判別する
        If InfoMeisaiMast.KEIYAKU_SIT.Trim = "" Then
            InfoMeisaiMast.KEIYAKU_SIT = SIT_CODE_TABLE.GetText(InfoMeisaiMast.YOBI1.Trim)
            If InfoMeisaiMast.KEIYAKU_SIT.Trim = "" Then
                '変換できない場合
                InfoMeisaiMast.KEIYAKU_SIT = SIT_CODE
            End If
        End If

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
    Protected Function CheckRecord81_82() As String
        NENKIN_REC8.Data = RecordData

        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "8"

        Return "T0"
    End Function

    '
    ' 機能　 ： トレーラーレコードチェック
    '
    ' 戻り値 ： True - 成功， False - 失敗
    '
    ' 備考　 ：
    '
    Protected Function CheckRecord8() As String
        NENKIN_REC8.Data = RecordData

        ' 明細マスタ項目設定 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "8"
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(NENKIN_REC8.NK2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(NENKIN_REC8.NK3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = 0
        InfoMeisaiMast.TOTAL_ZUMI_KIN = 0
        InfoMeisaiMast.TOTAL_FUNO_KEN = 0
        InfoMeisaiMast.TOTAL_FUNO_KIN = 0

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = NENKIN_REC8.NK2
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = NENKIN_REC8.NK3
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = "0"

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
        NENKIN_REC9.Data = RecordData

        ' 明細マスタ項目設定
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "9"

        Return "E"
    End Function

    '
    ' 機能　 ： ヘッダレコードを読み込んでチェック
    '
    ' 備考　 ： 各フォーマットチェックから，呼ばれる共通関数
    '　　　     ヘッダ情報から，取引先マスタを参照する
    '
    Protected Overrides Function CheckHeaderRecord(Optional ByVal mode As Integer = 0) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        ' ＤＢ接続が存在しない場合，正常値を返す
        If OraDB Is Nothing Then
            Return True
        End If

        ' 口振
        '取引先コード検索（マルチの場合も考慮する）、委託者コードのチェック
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(",TYUUDAN_FLG_S")
        SQL.Append(",TOUROKU_FLG_S")
        SQL.Append(",UKETUKE_FLG_S")
        SQL.Append(",BAITAI_CODE_T")
        SQL.Append(" FROM TORIMAST,SCHMAST")
        SQL.Append(" WHERE ")

        Dim ToriCode As String
        ' 年金種別から判断 61:旧厚生年金,62:旧船員年金,63:旧国民年金,64:労災年金,65:新国民年金・厚生年金,66:新船員年金,67:旧国民年金短期
        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & NENKIN_REC1.NK2)
        If ToriCode <> "err" Then
            SQL.Append("     TORIS_CODE_T = " & SQ(ToriCode.PadRight(12).Substring(0, 10)))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(ToriCode.PadRight(12).Substring(10, 2)))
        Else
            WriteBLog("年金 取引先コード取得", "失敗", "種目コード：" & NENKIN_REC1.NK2 & " 取引先コード：" & ToriCode & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
            DataInfo.Message = "年金 取引先コード取得 種目コード：" & NENKIN_REC1.NK2 & " 取引先コード：" & ToriCode & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            ' 取引先マスタ情報をクリアする
            Call mInfoComm.GetTORIMAST("", "")
            Return False
        End If

        'SQL.Append("   AND ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))

        SQL.Append("   AND FURI_DATE_S = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append("   AND '1'          = FSYORI_KBN_T")
        SQL.Append("   AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append("   AND TORIF_CODE_S = TORIF_CODE_T")

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then
            WriteBLog("ファイルヘッダ取引先又はスケジュール検索", "失敗", "種目コード：" & NENKIN_REC1.NK2 & " 取引先コード：" & ToriCode & " 委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"))
            DataInfo.Message = "取引先又はスケジュール検索失敗 種目コード：" & NENKIN_REC1.NK2 & " 取引先コード：" & ToriCode & " 委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            ' 取引先マスタ情報をクリアする
            Call mInfoComm.GetTORIMAST("", "")
            OraReader.Close()
            Return False
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

        '2007/07/04　フォーマット区分、媒体コード、中断フラグ、登録フラグチェック
        'フォーマット区分のチェック
        If mInfoComm.INFOToriMast.FMT_KBN_T <> mInfoComm.INFOToriMast.FMT_KBN_T Then
            WriteBLog("フォーマット区分異常", "エラー発生")
            DataInfo.Message = " 取引先マスタ登録異常：フォーマット区分"
            Return False
        End If

        If OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
            WriteBLog("スケジュール:中断フラグ設定済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日"), "中断")
            DataInfo.Message = "スケジュール:中断フラグ設定済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            Return False
        End If
        If OraReader.GetItem("UKETUKE_FLG_S") <> "0" Then
            WriteBLog("スケジュール:落し込み処理済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日") & "種目コード：" & NENKIN_REC1.NK2 & " 取引先コード：" & ToriCode & " 委託者コード：" & mInfoComm.INFOToriMast.ITAKU_CODE_T, "中断")
            DataInfo.Message = "スケジュール:落し込み処理済 振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy年MM月dd日")
            Return False
        End If

        ' オラクルReaderクローズ
        OraReader.Close()

        WriteBLog("ファイルヘッダ取引先検索", "成功", "取引先コード：" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)

        If InfoMeisaiMast.FURIKAE_DATE <> mInfoComm.INFOParameter.FURI_DATE Then
            '振替日不一致
            '振替日が違っていると異常終了する場合
            WriteBLog("ファイルヘッダ振替日", "不一致", "振替日：" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "MM月dd日"))
            DataInfo.Message = "ファイルヘッダ振替日不一致:" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "MM月dd日")
            Return False
        End If

        Return True
    End Function
End Class
