Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' SSS銀行別振替結果合計表印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KF3SP003
    Inherits CAstReports.ClsReportBase

    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KF3SP003"
        '定義体名セット
        ReportBaseName = "KF3SP003_SSS銀行別振替結果合計表.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "クラス変数"

    Private Structure strcListInfo
        Dim KIN_CD As String
        Dim KIN_NAME As String
        Dim SYUBETU As String
        Dim TANKA As Integer
        Dim TIKU_CD As String
        Dim SYORI_KEN As Integer
        Dim SYORI_KIN As Long
        Dim FURI_KEN As Integer
        Dim FURI_KIN As Long
        Dim FUNOU_KEN As Integer
        Dim FUNOU_KIN As Long

        Dim SIHARAI_TESUU As Long
        Dim SIHARAI_TAX As Integer

        Dim SINKIN_TOUKAI As Long               '信金東海地区支払手数料合計
        Dim SINKIN_TOUKAI_ZEI As Long           '信金東海地区支払手数料消費税合計
        Dim SINKIN_TOUKAI_GAI As Long           '信金東海地区以外支払手数料合計
        Dim SINKIN_TOUKAI_GAI_ZEI As Long       '信金東海地区以外支払手数料消費税合計

        Dim NOUKYO_AICHI As Long                '農協愛知支払手数料合計
        Dim NOUKYO_AICHI_ZEI As Long            '農協愛知支払手数料消費税合計
        Dim NOUKYO_GIFU As Long                 '農協岐阜支払手数料合計
        Dim NOUKYO_GIFU_ZEI As Long             '農協岐阜支払手数料消費税合計
        Dim NOUKYO_MIE As Long                  '農協三重支払手数料合計
        Dim NOUKYO_MIE_ZEI As Long              '農協三重支払手数料消費税合計
        Dim NOUKYO_SIZU As Long                 '農協静岡支払手数料合計
        Dim NOUKYO_SIZU_ZEI As Long             '農協静岡支払手数料消費税合計
        Dim NOUKYO_SONOTA As Long               '農協その他支払手数料合計
        Dim NOUKYO_SONOTA_ZEI As Long           '農協その他支払手数料消費税合計

        Dim TYUKIN_TESUU As Long                '信金中金手数料
        Dim TYUKIN_ZEI As Long                  '信金中金消費税

        Public Sub Init()
            KIN_CD = String.Empty
            KIN_NAME = String.Empty
            SYUBETU = String.Empty
            TANKA = 0
            TIKU_CD = String.Empty
            SYORI_KEN = 0
            SYORI_KIN = 0
            FURI_KEN = 0
            FURI_KIN = 0
            FUNOU_KEN = 0
            FUNOU_KIN = 0

            SIHARAI_TESUU = 0
            SIHARAI_TAX = 0

            SINKIN_TOUKAI = 0
            SINKIN_TOUKAI_ZEI = 0
            SINKIN_TOUKAI_GAI = 0
            SINKIN_TOUKAI_GAI_ZEI = 0

            NOUKYO_AICHI = 0
            NOUKYO_AICHI_ZEI = 0
            NOUKYO_GIFU = 0
            NOUKYO_GIFU_ZEI = 0
            NOUKYO_MIE = 0
            NOUKYO_MIE_ZEI = 0
            NOUKYO_SIZU = 0
            NOUKYO_SIZU_ZEI = 0
            NOUKYO_SONOTA = 0
            NOUKYO_SONOTA_ZEI = 0

            TYUKIN_TESUU = 0
            TYUKIN_ZEI = 0

        End Sub

        Public Sub InitData()
            KIN_CD = String.Empty
            KIN_NAME = String.Empty
            SYUBETU = String.Empty
            TANKA = 0
            TIKU_CD = String.Empty
            SYORI_KEN = 0
            SYORI_KIN = 0
            FURI_KEN = 0
            FURI_KIN = 0
            FUNOU_KEN = 0
            FUNOU_KIN = 0

            SIHARAI_TESUU = 0
            SIHARAI_TAX = 0
        End Sub

        Friend Sub SetOraDataList(ByVal oraReader As CASTCommon.MyOracleReader)
            KIN_CD = oraReader.GetString("KEIYAKU_KIN_K")
            KIN_NAME = oraReader.GetString("KIN_NNAME_N")
            SYUBETU = oraReader.GetString("SYUBETU_N")
            TANKA = oraReader.GetInt("TESUU_TANKA_N")
            TIKU_CD = oraReader.GetString("TIKU_CODE_N")
            SYORI_KEN = oraReader.GetInt("SYORI_KEN")
            SYORI_KIN = oraReader.GetInt64("SYORI_KIN")
            FURI_KEN = oraReader.GetInt("FURI_KEN")
            FURI_KIN = oraReader.GetInt64("FURI_KIN")
            FUNOU_KEN = oraReader.GetInt("FUNOU_KEN")
            FUNOU_KIN = oraReader.GetInt64("FUNOU_KIN")
        End Sub

    End Structure
    Private ListInfo As strcListInfo

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 印刷用レコードを作成します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function MakeRecord() As Boolean
        Dim MainDB As New CASTCommon.MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim strSyubetuBuff As String = String.Empty
        Dim strTikuCdBuff As String = String.Empty

        Try
            '構造体初期化
            ListInfo.Init()

            'この帳票は決済日基準で計算できない
            Dim strKijunDate As String = FURI_DATE
            TAX.GetZeiritsu(strKijunDate)
            If TAX.ZEIRITSU_ID.Equals("err") = True Then
                BatchLog.Write("税率取得", "失敗", "基準日：" & strKijunDate)
                Return False
            End If

            '---------------------------------------------------------
            '小計を計算
            '---------------------------------------------------------
            If oraReader.DataReader(CreateListSQL) = True Then
                While oraReader.EOF = False
                    If CalcSubTotal(oraReader, False) = False Then
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                        Return False
                    End If
                    oraReader.NextRead()
                End While
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If

            oraReader.Close()

            '税抜手数料小計が計算し終わったら、税を含める
            If CalcSubTotal(Nothing, True) = False Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                Return False
            End If

            '信金中金の手数料
            If oraReader.DataReader("select distinct TESUU_TANKA_N from TENMAST where KIN_NO_N = '1000'") = True Then
                ListInfo.TYUKIN_TESUU = Math.Floor(CDbl(oraReader.GetInt("TESUU_TANKA_N")) * CDbl(TAX.ZEIRITSU))
                ListInfo.TYUKIN_ZEI = Math.Floor(CDbl(oraReader.GetInt("TESUU_TANKA_N")) * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
            Else
                ListInfo.TYUKIN_TESUU = 0
                ListInfo.TYUKIN_ZEI = 0
            End If

            oraReader.Close()

            '---------------------------------------------------------
            '明細を作成
            '---------------------------------------------------------
            If oraReader.DataReader(CreateListSQL) = True Then
                While oraReader.EOF = False
                    '構造体初期化
                    ListInfo.InitData()
                    '取得レコードを構造体に格納
                    ListInfo.SetOraDataList(oraReader)

                    '支払手数料と消費税を計算
                    'ゆうちょは内税、それ以外は外税
                    If ListInfo.KIN_CD.Equals("9900") = False Then
                        ListInfo.SIHARAI_TESUU = Math.Floor(ListInfo.SYORI_KEN * ListInfo.TANKA * CDbl(TAX.ZEIRITSU))
                        ListInfo.SIHARAI_TAX = Math.Floor(ListInfo.SYORI_KEN * ListInfo.TANKA * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                    Else
                        'ゆうちょは内税かつ、振替件数で手数料を計算する
                        ListInfo.SIHARAI_TESUU = Math.Floor(ListInfo.FURI_KEN * ListInfo.TANKA)
                        ListInfo.SIHARAI_TAX = 0
                    End If

                    'CSV書き込み
                    '小計の書き込み(小計はループ内で種別が変わったときに印字する)
                    If strSyubetuBuff.Equals(String.Empty) = False Then
                        If strSyubetuBuff.Equals(ListInfo.SYUBETU) = False Then
                            Select Case strSyubetuBuff
                                Case "1"        '金庫
                                    If SetListData(1) = False Then
                                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                                        Return False
                                    End If

                                Case "2"        '農協
                                    If SetListData(2) = False Then
                                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                                        Return False
                                    End If
                            End Select
                        End If
                    End If

                    If SetListData(0) = False Then
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                        Return False
                    End If

                    strSyubetuBuff = oraReader.GetString("SYUBETU_N")

                    RecordCnt += 1

                    oraReader.NextRead()
                End While

                If ListInfo.SYUBETU.Equals("1") = True OrElse ListInfo.SYUBETU.Equals("2") = True Then
                    'ループを抜けた際の種別が1(農協が存在しなかった)か2(ゆうちょが存在しなかった)の場合
                    '小計の書込み
                    Select Case ListInfo.SYUBETU
                        Case "1"
                            If SetListData(1) = False Then
                                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                                Return False
                            End If
                        Case "2"
                            If SetListData(2) = False Then
                                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                                Return False
                            End If
                    End Select
                End If
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If

            Return True
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.Message)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 印刷データ取得用のSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateListSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append("  KEIYAKU_KIN_K")
            .Append(", KIN_NNAME_N")
            .Append(", SYUBETU_N")
            .Append(", TESUU_TANKA_N")
            .Append(", TIKU_CODE_N")
            .Append(", count(FURIKIN_K) as SYORI_KEN")
            .Append(", sum(FURIKIN_K) as SYORI_KIN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURI_KEN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURI_KIN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOU_KEN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOU_KIN")
            .Append(" from TORIMAST, SCHMAST, MEIMAST, ")
            .Append(" (select distinct KIN_NO_N, KIN_NNAME_N, SYUBETU_N, TESUU_TANKA_N, TIKU_CODE_N from TENMAST) TENMAST")
            .Append(" where TORIS_CODE_T = TORIS_CODE_S")
            .Append(" and TORIF_CODE_T = TORIF_CODE_S")
            .Append(" and TORIS_CODE_S = TORIS_CODE_K")
            .Append(" and TORIF_CODE_S = TORIF_CODE_K")
            .Append(" and FURI_DATE_S = FURI_DATE_K")
            .Append(" and KEIYAKU_KIN_K = KIN_NO_N")
            .Append(" and FURI_DATE_K = " & SQ(FURI_DATE))
            .Append(" and DATA_KBN_K = '2'")
            .Append(" and FMT_KBN_T in ('20','21')")
            .Append(" and FUNOU_FLG_S = '1'")
            .Append(" and TAKOU_FLG_S = '1'")
            .Append(" and TYUUDAN_FLG_S = '0'")
            .Append(" group by KEIYAKU_KIN_K, KIN_NNAME_N, SYUBETU_N, TESUU_TANKA_N, TIKU_CODE_N")
            .Append(" order by SYUBETU_N, KEIYAKU_KIN_K")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 小計を計算します。
    ''' </summary>
    ''' <param name="oraReader">オラクルリーダー</param>
    ''' <param name="bCalcTaxFlg">税計算フラグ(True:税計算 False:小計加算)</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function CalcSubTotal(ByVal oraReader As CASTCommon.MyOracleReader, _
                                  ByVal bCalcTaxFlg As Boolean) As Boolean
        Try
            '明細でまわして小計を加算する処理と消費税や税込の手数料を計算する処理とで分ける
            If bCalcTaxFlg = False Then
                '小計加算処理

                '税抜手数料計算
                'ゆうちょのみ、振替件数×単価で計算する
                Dim lngNoTaxTesuu As Long = 0

                If oraReader.GetString("KEIYAKU_KIN_K").Equals("9900") = True Then
                    lngNoTaxTesuu = oraReader.GetInt("FURI_KEN") * oraReader.GetInt("TESUU_TANKA_N")
                Else
                    lngNoTaxTesuu = oraReader.GetInt("SYORI_KEN") * oraReader.GetInt("TESUU_TANKA_N")
                End If

                Select Case oraReader.GetString("SYUBETU_N")
                    Case "0"        '銀行
                    Case "1"        '金庫
                        Select Case oraReader.GetString("TIKU_CODE_N")
                            Case "0"
                                '地区コードがその他は除く
                            Case Else
                                If oraReader.GetString("KEIYAKU_KIN_K") <> IniInfo.KINKOCD Then
                                    '自金庫を除く
                                    ListInfo.SINKIN_TOUKAI += lngNoTaxTesuu
                                End If
                        End Select

                    Case "2"        '農協
                        Select Case oraReader.GetString("TIKU_CODE_N")
                            Case "0"    'その他
                                ListInfo.NOUKYO_SONOTA += lngNoTaxTesuu
                            Case "1"    '愛知
                                ListInfo.NOUKYO_AICHI += lngNoTaxTesuu
                            Case "2"    '岐阜
                                ListInfo.NOUKYO_GIFU += lngNoTaxTesuu
                            Case "3"    '三重
                                ListInfo.NOUKYO_MIE += lngNoTaxTesuu
                            Case "4"    '静岡
                                ListInfo.NOUKYO_SIZU += lngNoTaxTesuu
                        End Select

                    Case "3"        'ゆうちょ
                End Select

            Else
                '消費税計算

                '信金東海地区
                ListInfo.SINKIN_TOUKAI_ZEI = Math.Floor(ListInfo.SINKIN_TOUKAI * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                ListInfo.SINKIN_TOUKAI = Math.Floor(ListInfo.SINKIN_TOUKAI * CDbl(TAX.ZEIRITSU))

                '信金東海地区以外
                ListInfo.SINKIN_TOUKAI_GAI_ZEI = Math.Floor(ListInfo.SINKIN_TOUKAI_GAI * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                ListInfo.SINKIN_TOUKAI_GAI = Math.Floor(ListInfo.SINKIN_TOUKAI_GAI * CDbl(TAX.ZEIRITSU))

                '農協愛知
                ListInfo.NOUKYO_AICHI_ZEI = Math.Floor(ListInfo.NOUKYO_AICHI * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                ListInfo.NOUKYO_AICHI = Math.Floor(ListInfo.NOUKYO_AICHI * CDbl(TAX.ZEIRITSU))

                '農協岐阜
                ListInfo.NOUKYO_GIFU_ZEI = Math.Floor(ListInfo.NOUKYO_GIFU * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                ListInfo.NOUKYO_GIFU = Math.Floor(ListInfo.NOUKYO_GIFU * CDbl(TAX.ZEIRITSU))

                '農協三重
                ListInfo.NOUKYO_MIE_ZEI = Math.Floor(ListInfo.NOUKYO_MIE * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                ListInfo.NOUKYO_MIE = Math.Floor(ListInfo.NOUKYO_MIE * CDbl(TAX.ZEIRITSU))

                '農協静岡
                ListInfo.NOUKYO_SIZU_ZEI = Math.Floor(ListInfo.NOUKYO_SIZU * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                ListInfo.NOUKYO_SIZU = Math.Floor(ListInfo.NOUKYO_SIZU * CDbl(TAX.ZEIRITSU))

                '農協その他
                ListInfo.NOUKYO_SONOTA_ZEI = Math.Floor(ListInfo.NOUKYO_SONOTA * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                ListInfo.NOUKYO_SONOTA = Math.Floor(ListInfo.NOUKYO_SONOTA * CDbl(TAX.ZEIRITSU))

            End If

            Return True
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "構造体設定(小計部)", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 印刷データをCSVに設定します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function SetListData(ByVal intCsvMode As Integer) As Boolean
        Try
            Select Case intCsvMode
                Case 0      '明細
                    OutputCsvData(mMatchingDate, True)      '処理日
                    OutputCsvData(FURI_DATE, True)          '振替日
                    OutputCsvData(ListInfo.KIN_CD, True)    '金融機関コード
                    If ListInfo.KIN_CD = IniInfo.KINKOCD Then
                        '自金庫の場合
                        OutputCsvData("当庫", True)
                    Else
                        '他金庫の場合
                        OutputCsvData(ListInfo.KIN_NAME, True)  '金融機関名 
                    End If
                    OutputCsvData(ListInfo.SYORI_KEN)       '請求件数
                    OutputCsvData(ListInfo.SYORI_KIN)       '請求金額
                    OutputCsvData(ListInfo.FURI_KEN)        '完了件数
                    OutputCsvData(ListInfo.FURI_KIN)        '完了金額
                    OutputCsvData(ListInfo.FUNOU_KEN)       '不能件数
                    OutputCsvData(ListInfo.FUNOU_KIN)       '不能金額
                    '支払手数料印字条件
                    '種別が0(銀行)か3(ゆうちょ)の場合か、種別が1(金庫)で地区コードが0(その他)
                    Select Case ListInfo.SYUBETU
                        Case "0", "3"
                            OutputCsvData(ListInfo.SIHARAI_TESUU)       '支払手数料
                            OutputCsvData(ListInfo.SIHARAI_TAX)         '支払手数料消費税
                            OutputCsvData("0", True)                    '支払手数料印字フラグ(印字する)
                        Case "1"
                            If ListInfo.TIKU_CD.Equals("0") = True Then
                                OutputCsvData(ListInfo.SIHARAI_TESUU)   '支払手数料
                                OutputCsvData(ListInfo.SIHARAI_TAX)     '支払手数料消費税
                                OutputCsvData("0", True)                '支払手数料印字フラグ(印字する)
                            Else
                                OutputCsvData("0")                      '支払手数料
                                OutputCsvData("0")                      '支払手数料消費税
                                OutputCsvData("1", True)                '支払手数料印字フラグ(印字しない)
                            End If
                        Case Else
                            OutputCsvData("0")                          '支払手数料
                            OutputCsvData("0")                          '支払手数料消費税
                            OutputCsvData("1", True)                    '支払手数料消費税フラグ(印字しない)
                    End Select

                    '自金庫フラグ
                    If ListInfo.KIN_CD = IniInfo.KINKOCD Then
                        OutputCsvData("0", True)
                    Else
                        OutputCsvData("1", True)
                    End If

                    OutputCsvData("0", True)                '明細表示フラグ(表示する)
                    OutputCsvData(ListInfo.TYUKIN_TESUU)                '信金中金手数料
                    OutputCsvData(ListInfo.TYUKIN_ZEI, False, True)     '信金中金消費税

                Case 1      '信金小計
                    For i As Integer = 0 To 1
                        OutputCsvData(mMatchingDate, True)  '処理日
                        OutputCsvData(FURI_DATE, True)      '振替日
                        OutputCsvData("****", True)         '金融機関コード
                        Select Case i
                            Case 0 : OutputCsvData("ｼﾝｷﾝﾄｳｶｲﾁｸ ｺﾞｳｹｲ", True)
                            Case 1 : OutputCsvData("ｼﾝｷﾝﾄｳｶｲｲｶﾞｲ ｺﾞｳｹｲ", True)
                        End Select
                        '小計系は件数・金額は0固定とする
                        OutputCsvData("0")                  '請求件数
                        OutputCsvData("0")                  '請求金額
                        OutputCsvData("0")                  '完了件数
                        OutputCsvData("0")                  '完了金額
                        OutputCsvData("0")                  '不能件数
                        OutputCsvData("0")                  '不能金額
                        Select Case i
                            Case 0
                                OutputCsvData(ListInfo.SINKIN_TOUKAI.ToString)
                                OutputCsvData(ListInfo.SINKIN_TOUKAI_ZEI.ToString)
                            Case 1
                                OutputCsvData(ListInfo.SINKIN_TOUKAI_GAI.ToString)
                                OutputCsvData(ListInfo.SINKIN_TOUKAI_GAI_ZEI.ToString)
                        End Select
                        OutputCsvData("0", True)            '支払手数料印字フラグ(印字する)
                        OutputCsvData("1", True)            '自金庫フラグ(他行)
                        OutputCsvData("1", True)            '明細表示フラグ(表示しない)
                        OutputCsvData(ListInfo.TYUKIN_TESUU)                '信金中金手数料
                        OutputCsvData(ListInfo.TYUKIN_ZEI, False, True)     '信金中金消費税
                    Next

                Case 2      '農協小計
                    For i As Integer = 0 To 4
                        OutputCsvData(mMatchingDate, True)  '処理日
                        OutputCsvData(FURI_DATE, True)      '振替日
                        OutputCsvData("****", True)         '金融機関コード
                        Select Case i
                            Case 0 : OutputCsvData("ﾉｳｷﾖｳｱｲﾁ ｺﾞｳｹｲ", True)
                            Case 1 : OutputCsvData("ﾉｳｷﾖｳｷﾞﾌ ｺﾞｳｹｲ", True)
                            Case 2 : OutputCsvData("ﾉｳｷﾖｳﾐｴ ｺﾞｳｹｲ", True)
                            Case 3 : OutputCsvData("ﾉｳｷﾖｳｼｽﾞｵｶ ｺﾞｳｹｲ", True)
                            Case 4 : OutputCsvData("ﾉｳｷﾖｳｿﾉﾀ ｺﾞｳｹｲ", True)
                        End Select
                        '小計系は件数・金額は0固定とする
                        OutputCsvData("0")                  '請求件数
                        OutputCsvData("0")                  '請求金額
                        OutputCsvData("0")                  '完了件数
                        OutputCsvData("0")                  '完了金額
                        OutputCsvData("0")                  '不能件数
                        OutputCsvData("0")                  '不能金額
                        Select Case i
                            Case 0
                                OutputCsvData(ListInfo.NOUKYO_AICHI.ToString)
                                OutputCsvData(ListInfo.NOUKYO_AICHI_ZEI.ToString)
                            Case 1
                                OutputCsvData(ListInfo.NOUKYO_GIFU.ToString)
                                OutputCsvData(ListInfo.NOUKYO_GIFU_ZEI.ToString)
                            Case 2
                                OutputCsvData(ListInfo.NOUKYO_MIE.ToString)
                                OutputCsvData(ListInfo.NOUKYO_MIE_ZEI.ToString)
                            Case 3
                                OutputCsvData(ListInfo.NOUKYO_SIZU.ToString)
                                OutputCsvData(ListInfo.NOUKYO_SIZU_ZEI.ToString)
                            Case 4
                                OutputCsvData(ListInfo.NOUKYO_SONOTA.ToString)
                                OutputCsvData(ListInfo.NOUKYO_SONOTA_ZEI.ToString)
                        End Select
                        OutputCsvData("0", True)            '支払手数料印字フラグ(印字する)
                        OutputCsvData("1", True)            '自金庫フラグ(他行)
                        OutputCsvData("1", True)            '明細表示フラグ(表示しない)
                        OutputCsvData(ListInfo.TYUKIN_TESUU)                '信金中金手数料
                        OutputCsvData(ListInfo.TYUKIN_ZEI, False, True)     '信金中金消費税
                    Next
            End Select


            Return True
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "CSVデータ設定", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' CSVファイルに書き込みます。
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="dq"></param>
    ''' <param name="crlf"></param>
    ''' <remarks></remarks>
    Private Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

#End Region

End Class
