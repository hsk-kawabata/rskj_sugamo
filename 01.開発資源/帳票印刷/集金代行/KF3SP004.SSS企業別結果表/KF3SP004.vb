Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports CASTCommon

''' <summary>
''' SSS企業別結果表印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KF3SP004
    Inherits CAstReports.ClsReportBase

    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KF3SP004"
        '定義体名セット
        ReportBaseName = "KF3SP004_SSS企業別結果表.rpd"
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
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Try
            If oraReader.DataReader(CreateGetSchSQL) = True Then
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                While oraReader.EOF = False
                    If oraMeiReader.DataReader(CreateGetMeiSQL(oraReader)) = True Then
                        While oraMeiReader.EOF = False
                            If SetListData(oraReader, oraMeiReader) = False Then
                                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "レコード作成", "失敗")
                                Return False
                            End If
                            oraMeiReader.NextRead()
                        End While
                        oraMeiReader.Close()

                    Else
                        BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                        RecordCnt = -1
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
            If Not oraMeiReader Is Nothing Then oraMeiReader.Close()
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
    Private Function CreateGetSchSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from SCHMAST, TORIMAST")
            .Append(" where TORIS_CODE_S = TORIS_CODE_T")
            .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            .Append(" and FMT_KBN_T in ('20', '21')")
            .Append(" and FUNOU_FLG_S = '1'")
            .Append(" and TAKOU_FLG_S = '1'")
            .Append(" and TYUUDAN_FLG_S = '0'")
            .Append(" and FURI_DATE_S = " & SQ(FURI_DATE))
            If TORI_CODE.Equals("999999999999") = False Then
                .Append(" and TORIS_CODE_S = " & SQ(TORIS_CODE))
                .Append(" and TORIF_CODE_S = " & SQ(TORIF_CODE))
            End If
            .Append(" order by TORIS_CODE_S, TORIF_CODE_S")
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
            .Append(", KIN_NNAME_N")
            .Append(", SYUBETU_N")
            .Append(", count(FURIKIN_K) as SYORI_KEN")
            .Append(", sum(FURIKIN_K) as SYORI_KIN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURI_KEN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURI_KIN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOU_KEN")
            .Append(", sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOU_KIN")
            .Append(" from MEIMAST, ")
            .Append(" (select distinct KIN_NO_N, KIN_NNAME_N, SYUBETU_N from TENMAST) TENMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and DATA_KBN_K = '2'")
            .Append(" and KEIYAKU_KIN_K = KIN_NO_N")
            .Append(" group by KEIYAKU_KIN_K, KIN_NNAME_N, SYUBETU_N")
            .Append(" order by SYUBETU_N, KEIYAKU_KIN_K")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 印刷データをCSVに設定します。
    ''' </summary>
    ''' <param name="oraSchReader">スケジュールオラクルリーダー</param>
    ''' <param name="oraMeiReader">明細オラクルリーダー</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function SetListData(ByVal oraSchReader As CASTCommon.MyOracleReader, _
                                 ByVal oraMeiReader As CASTCommon.MyOracleReader) As Boolean
        Try
            OutputCsvData(mMatchingDate, True)      '処理日
            OutputCsvData(oraSchReader.GetString("FURI_DATE_S"), True)      '振替日
            OutputCsvData(oraSchReader.GetString("TORIS_CODE_S"), True)     '取引先主コード
            OutputCsvData(oraSchReader.GetString("TORIF_CODE_S"), True)     '取引先副コード
            OutputCsvData(oraSchReader.GetString("ITAKU_NNAME_T"), True)    '委託者名
            OutputCsvData(oraMeiReader.GetString("KEIYAKU_KIN_K"), True)    '金融機関コード
            If IniInfo.KINKOCD = oraMeiReader.GetString("KEIYAKU_KIN_K") Then
                OutputCsvData("当庫", True)                                 '自金庫
                OutputCsvData("0", True)                                    '自金庫フラグ(表示に使う)
            Else
                OutputCsvData(oraMeiReader.GetString("KIN_NNAME_N"), True)  '金融機関名
                OutputCsvData("1", True)                                    '自金庫フラグ(表示に使う)
            End If
            OutputCsvData(oraMeiReader.GetInt("SYORI_KEN"))                 '請求件数
            OutputCsvData(oraMeiReader.GetInt64("SYORI_KIN"))               '請求金額
            OutputCsvData(oraMeiReader.GetInt("FURI_KEN"))                  '完了件数
            OutputCsvData(oraMeiReader.GetInt64("FURI_KIN"))                '完了金額
            OutputCsvData(oraMeiReader.GetInt("FUNOU_KEN"))                 '不能件数
            OutputCsvData(oraMeiReader.GetInt64("FUNOU_KIN"), False, True)  '不能金額
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
