Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' SSS引落結果合計表印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KF3SP001
    Inherits CAstReports.ClsReportBase

    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KF3SP001"

        '定義体名セット
        ReportBaseName = "KF3SP001_SSS引落結果合計表.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "クラス変数"

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

        Try
            If oraReader.DataReader(CreateListSQL) = True Then
                While oraReader.EOF = False
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
                        '税率取得失敗
                        BatchLog.Write("レコード作成", "失敗", "税率の取得に失敗しました。基準日：" & strKijunDate)
                        Return False
                    End If

                    If SetListData(oraReader) = False Then
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                        Return False
                    End If
                    RecordCnt += 1
                    oraReader.NextRead()
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
            If Not oraReader Is Nothing Then oraReader.Close()
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
            .Append("  TORIS_CODE_S")
            .Append(", TORIF_CODE_S")
            .Append(", ITAKU_NNAME_T")
            .Append(", ITAKU_CODE_T")
            .Append(", FURI_DATE_S")
            .Append(", SYORI_KEN_S")
            .Append(", SYORI_KIN_S")
            .Append(", FURI_KEN_S")
            .Append(", FURI_KIN_S")
            .Append(", FUNOU_KEN_S")
            .Append(", FUNOU_KIN_S")
            .Append(", KIHTESUU_T")
            .Append(", SYOUHI_KBN_T")
            .Append(", TSIT_NO_T")
            .Append(", SIT_NNAME_N")
            .Append(", KESSAI_YDATE_S")
            .Append(" from TORIMAST, SCHMAST, TENMAST")
            .Append(" where FMT_KBN_T in ('20', '21')")
            .Append(" and TAKOU_FLG_S = '1'")
            .Append(" and FUNOU_FLG_S = '1'")
            .Append(" and TYUUDAN_FLG_S = '0'")
            .Append(" and FURI_DATE_S = " & SQ(FURI_DATE))
            If TORI_CODE.Equals("999999999999") = False Then
                .Append(" and TORIS_CODE_S = " & SQ(TORIS_CODE))
                .Append(" and TORIF_CODE_S = " & SQ(TORIF_CODE))
            End If
            .Append(" and TORIS_CODE_S = TORIS_CODE_T")
            .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            .Append(" and FSYORI_KBN_S = FSYORI_KBN_T")
            .Append(" and TKIN_NO_T = KIN_NO_N(+)")
            .Append(" and TSIT_NO_T = SIT_NO_N(+)")
            .Append(" order by TORIS_CODE_S, TORIF_CODE_S")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 印刷データをCSVに設定します。
    ''' </summary>
    ''' <param name="oraReader">オラクルリーダー</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function SetListData(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Try
            OutputCsvData(mMatchingDate, True)          '処理日
            OutputCsvData(oraReader.GetString("FURI_DATE_S"), True)     '振替日
            OutputCsvData(oraReader.GetString("TORIS_CODE_S"), True)    '取引先主コード
            OutputCsvData(oraReader.GetString("TORIF_CODE_S"), True)    '取引先副コード
            OutputCsvData(oraReader.GetString("ITAKU_NNAME_T"), True)   '委託者名
            OutputCsvData(oraReader.GetString("ITAKU_CODE_T"), True)    '委託者コード
            OutputCsvData(oraReader.GetString("TSIT_NO_T"), True)       '支店コード
            OutputCsvData(oraReader.GetString("SIT_NNAME_N"), True)     '支店名
            OutputCsvData(oraReader.GetInt("SYORI_KEN_S"))              '処理件数
            OutputCsvData(oraReader.GetInt64("SYORI_KIN_S"))            '処理金額
            OutputCsvData(oraReader.GetInt("FURI_KEN_S"))               '振替件数
            OutputCsvData(oraReader.GetInt64("FURI_KIN_S"))             '振替金額
            OutputCsvData(oraReader.GetInt("FUNOU_KEN_S"))              '不能件数
            OutputCsvData(oraReader.GetInt64("FUNOU_KIN_S"))            '不能金額
            OutputCsvData(oraReader.GetInt("KIHTESUU_T"))               '基本手数料

            Dim lngTesuu1 As Long = oraReader.GetInt("KIHTESUU_T") * oraReader.GetInt("SYORI_KEN_S")
            OutputCsvData(lngTesuu1.ToString)                           '手数料計1


            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "1" : OutputCsvData("0")                           '手数料計2(内税時)
                Case Else : OutputCsvData(lngTesuu1.ToString)           '手数料計2(外税時)
            End Select

            Dim strSyouhiPer As String = CStr(CDbl(TAX.ZEIRITSU) * 100 - 100)
            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "1" : OutputCsvData("", True)                      '消費税パーセント(内税時は印字しない)
                Case Else : OutputCsvData(strSyouhiPer & "％", True)    '消費税パーセント(外税時)
            End Select

            Dim lngSyohi As Long = 0
            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "1"
                    '内税時は税率に金額を税率に100かけたもので割って、パーセントをかける
                    Dim intTemp1 As Integer = CInt(CDbl(TAX.ZEIRITSU) * 100)
                    Dim intTemp2 As Integer = intTemp1 - 100
                    lngSyohi = lngTesuu1 / intTemp1 * intTemp2
                Case Else
                    '外税時は税率から1引いて、金額にかける
                    Dim dblTemp1 As Double = CDbl(TAX.ZEIRITSU) - 1.0
                    lngSyohi = Math.Floor(lngTesuu1 * dblTemp1)
            End Select
            OutputCsvData(lngSyohi.ToString)                            '消費税計


            OutputCsvData(oraReader.GetString("SYOUHI_KBN_T"), True)    '税区分
            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "1" : OutputCsvData("(内税)", True)                '税区分名称(内税時)
                Case Else : OutputCsvData("", True)                     '税区分名称(外税時は印字しない)
            End Select


            Dim lngShiharai As Long = 0
            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "1"
                    lngShiharai = lngTesuu1
                Case Else
                    lngShiharai = lngTesuu1 + lngSyohi
            End Select
            OutputCsvData(lngShiharai.ToString)                         '支払金額


            Dim lngNyukin As Long = oraReader.GetInt64("FURI_KIN_S") - lngShiharai
            OutputCsvData(lngNyukin.ToString, False, True)              '口座入金額

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
