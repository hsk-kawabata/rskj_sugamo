Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' SSS委託者別決済額一覧表印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KF3SP002
    Inherits CAstReports.ClsReportBase

    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KF3SP002"
        '定義体名セット
        ReportBaseName = "KF3SP002_SSS委託者別決済額一覧表.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "クラス変数"

    Private Structure strcListData
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim FURI_DATE As String
        Dim SIT_NO As String
        Dim KAMOKU As String
        Dim KOUZA As String
        Dim ITAKU_NAME As String
        Dim ITAKU_CODE As String
        Dim KANRYO_KIN As Long
        Dim KOFURI_TESUU As Long
        Dim SYOUHI1 As Integer
        Dim SYOUHI2 As String
        Dim NYUKIN As Long
        Dim SIHARAI_TESUU As Long
        Dim SYUEKI_TESUU As Long
        Dim KIHTESUU As Integer
        Dim SYOUHI_KBN As String

        Public Sub Init()
            TORIS_CODE = String.Empty
            TORIF_CODE = String.Empty
            FURI_DATE = String.Empty
            SIT_NO = String.Empty
            KAMOKU = String.Empty
            KOUZA = String.Empty
            ITAKU_NAME = String.Empty
            ITAKU_CODE = String.Empty
            KANRYO_KIN = 0
            KOFURI_TESUU = 0
            SYOUHI1 = 0
            SYOUHI2 = String.Empty
            NYUKIN = 0
            SIHARAI_TESUU = 0
            SYUEKI_TESUU = 0

            KIHTESUU = 0
            SYOUHI_KBN = String.Empty
        End Sub

    End Structure
    Private ListInfo As strcListData

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 印刷用レコードを作成します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function MakeRecord() As Boolean
        Dim MainDB As New CASTCommon.MyOracle

        Dim oraSchReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Try
            If oraSchReader.DataReader(CreateGetSchSQL) = True Then
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                While oraSchReader.EOF = False
                    '取得した項目を構造体に格納する
                    If SetSchData(oraSchReader) = True Then
                        If oraMeiReader.DataReader(CreateGetMeiSQL(oraSchReader)) = True Then
                            While oraMeiReader.EOF = False
                                '手数料計算
                                '支払手数料(D)(金融機関の件数×金融機関の手数料単価)
                                'ゆうちょは内税で計算する
                                If oraMeiReader.GetString("KIN_NO_N") = "9900" Then
                                    ListInfo.SIHARAI_TESUU += oraMeiReader.GetInt("KEN") * oraMeiReader.GetInt("TESUU_TANKA_N")
                                Else
                                    ListInfo.SIHARAI_TESUU += Math.Floor(oraMeiReader.GetInt64("KEN") * oraMeiReader.GetInt64("TESUU_TANKA_N") * CDbl(TAX.ZEIRITSU))
                                End If

                                oraMeiReader.NextRead()
                            End While
                        Else
                            BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                            RecordCnt = -1
                            Return False
                        End If

                        oraMeiReader.Close()

                        '収益手数料
                        ListInfo.SYUEKI_TESUU = ListInfo.KOFURI_TESUU - ListInfo.SYOUHI1 - ListInfo.SIHARAI_TESUU
                    Else
                        BatchLog.Write("構造体格納", "失敗")
                        Return False
                    End If

                    'CSV書き込み
                    If SetListData() = False Then
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                        Return False
                    End If
                    RecordCnt += 1

                    oraSchReader.NextRead()
                End While
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
            If Not oraSchReader Is Nothing Then oraSchReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 印刷データ取得用のSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks>大ループ用のSQL(これを基に明細を取得したりする)</remarks>
    Private Function CreateGetSchSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from SCHMAST, TORIMAST")
            .Append(" where FURI_DATE_S = " & SQ(FURI_DATE))
            .Append(" and FUNOU_FLG_S = '1'")
            .Append(" and TAKOU_FLG_S = '1'")
            .Append(" and TYUUDAN_FLG_S = '0'")
            .Append(" and FMT_KBN_T in ('20', '21')")
            .Append(" and TORIS_CODE_S = TORIS_CODE_T")
            .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            .Append(" order by TUKESIT_NO_T asc, TUKEKAMOKU_T desc, TUKEKOUZA_T asc")   '出力順は決済支店・科目(※科目のみ降順)・口座番号順
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 印刷データ取得用のSQLを作成します。
    ''' </summary>
    ''' <param name="oraReader">オラクルリーダー</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetMeiSQL(ByVal oraReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append("  KEIYAKU_KIN_K")
            .Append(", count(KEIYAKU_KIN_K) as KEN")
            .Append(", KIN_NO_N")
            .Append(", SYUBETU_N")
            .Append(", TESUU_TANKA_N")
            .Append(" from MEIMAST, ")
            .Append(" (select distinct KIN_NO_N, SYUBETU_N, TESUU_TANKA_N from TENMAST) TENMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and DATA_KBN_K = '2'")
            .Append(" and KEIYAKU_KIN_K = KIN_NO_N")
            .Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
            .Append(" group by KEIYAKU_KIN_K, KIN_NO_N, SYUBETU_N, TESUU_TANKA_N")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' スケジュールの情報を構造体に格納します。
    ''' </summary>
    ''' <param name="oraReader">オラクルリーダー</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function SetSchData(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        ListInfo.Init()         '構造体初期化

        Try
            With ListInfo
                .TORIS_CODE = oraReader.GetString("TORIS_CODE_S")
                .TORIF_CODE = oraReader.GetString("TORIF_CODE_S")
                .FURI_DATE = oraReader.GetString("FURI_DATE_S")
                .SIT_NO = oraReader.GetString("TUKESIT_NO_T")
                .KAMOKU = ConvertKamoku2TO1(oraReader.GetString("TUKEKAMOKU_T"))
                .KOUZA = oraReader.GetString("TUKEKOUZA_T")
                .ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                .ITAKU_NAME = oraReader.GetString("ITAKU_NNAME_T")
                .KIHTESUU = oraReader.GetInt("KIHTESUU_T")
                .SYOUHI_KBN = oraReader.GetString("SYOUHI_KBN_T")

                Dim strKijunDate As String = String.Empty
                If IniInfo.ZEIKIJUN.Equals("0") = True Then
                    '振替日基準
                    strKijunDate = oraReader.GetString("FURI_DATE_S")
                Else
                    '決済日基準
                    strKijunDate = oraReader.GetString("KESSAI_YDATE_S")
                End If

                TAX.GetZeiritsu(strKijunDate)

                If TAX.ZEIRITSU_ID.Equals("err") = True Then
                    BatchLog.Write("印字内容格納(スケジュール)", "失敗", "税率の取得に失敗しました。基準日：" & strKijunDate)
                    Return False
                End If

                .KANRYO_KIN = oraReader.GetInt64("FURI_KIN_S")      '完了金額(A)

                '口振手数料(B)
                Dim lngTesuu As Long = oraReader.GetInt64("TESUU_KIN1_S")
                Select Case oraReader.GetString("SYOUHI_KBN_T")
                    Case "0"    '外税
                        .KOFURI_TESUU = oraReader.GetInt64("TESUU_KIN1_S")
                    Case "1"    '内税
                        .KOFURI_TESUU = oraReader.GetInt64("TESUU_KIN1_S")
                End Select

                '消費税(C)
                Select Case oraReader.GetString("SYOUHI_KBN_T")
                    Case "0"    '外税
                        .SYOUHI1 = Math.Floor(lngTesuu * CDbl(CDbl(TAX.ZEIRITSU) - 1.0))
                    Case "1"    '内税
                        .SYOUHI1 = 0
                End Select

                '依頼者への入金額
                .NYUKIN = .KANRYO_KIN - oraReader.GetInt64("TESUU_KIN_S")

                Select Case oraReader.GetString("SYOUHI_KBN_T")
                    Case "0" : .SYOUHI2 = "*外税*"
                    Case "1" : .SYOUHI2 = "*内税*"
                End Select

            End With

            Return True
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印字内容格納(スケジュール)", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 印刷データをCSVに設定します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function SetListData() As Boolean
        Try
            OutputCsvData(mMatchingDate, True)      '処理日
            OutputCsvData(ListInfo.FURI_DATE, True)     '振替日
            OutputCsvData(ListInfo.TORIS_CODE, True)    '取引先主コード
            OutputCsvData(ListInfo.TORIF_CODE, True)    '取引先副コード
            OutputCsvData(ListInfo.SIT_NO, True)        '支店コード
            OutputCsvData(ListInfo.KAMOKU, True)        '科目コード
            OutputCsvData(ListInfo.KOUZA, True)         '口座番号
            OutputCsvData(ListInfo.ITAKU_NAME, True)    '委託者名
            OutputCsvData(ListInfo.ITAKU_CODE, True)    '委託者コード
            OutputCsvData(ListInfo.KANRYO_KIN)          '完了金額(振替金額)
            OutputCsvData(ListInfo.KOFURI_TESUU)        '口振手数料
            OutputCsvData(ListInfo.SYOUHI_KBN, True)    '消費税区分(表示の判断に使う)
            OutputCsvData(ListInfo.SYOUHI1)             '消費税1
            OutputCsvData(ListInfo.SYOUHI2, True)       '消費税2
            OutputCsvData(ListInfo.NYUKIN)              '依頼者への入金額
            OutputCsvData(ListInfo.SIHARAI_TESUU)       '支払手数料
            OutputCsvData(ListInfo.SYUEKI_TESUU)        '収益手数料
            Dim TyukinTesuu As Long = 30

            OutputCsvData(TyukinTesuu)                          '信金中金支払手数料(※)
            OutputCsvData(Math.Floor(TyukinTesuu * CDbl(CDbl(TAX.ZEIRITSU) - 1)), False, True)             '信金中金支払手数料消費税(※)

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
