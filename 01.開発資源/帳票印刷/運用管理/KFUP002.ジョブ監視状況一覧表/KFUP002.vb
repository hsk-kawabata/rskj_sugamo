Imports System
Imports System.Text
Imports CASTCommon
Imports System.IO
Imports CAstExternal

''' <summary>
''' ジョブ監視状況確認一覧表印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KFUP002
    Inherits CAstReports.ClsReportBase

    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFUP002"
        '定義体名セット
        ReportBaseName = "KFUP002_ジョブ監視状況確認一覧表.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "クラス変数"

    Public ParamSyoriDate As String

    Private PrintDate_From As String = String.Empty
    Private PrintDate_To As String = String.Empty
    Private PrintTime_To As String = String.Empty

    Private PrintStatus_From As String = String.Empty
    Private PrintStatus_To As String = String.Empty

    Private PrintDB As New CASTCommon.MyOracle

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 印刷用レコードを作成します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function MakeRecord() As Boolean

        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write("印刷メイン処理", "開始", "")

            PrintDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(PrintDB)

            '--------------------------------------
            ' 印刷対象日付初期設定
            '--------------------------------------
            Dim Gcom As New MenteCommon.clsCommon
            Dim BRet As Boolean = Gcom.CheckDateModule(Nothing, 1)

            '--------------------------------------
            ' 印刷対象日付(From)　取得 ※前営業日
            '--------------------------------------
            BRet = Gcom.CheckDateModule(ParamSyoriDate, PrintDate_From, 1, 1)

            '--------------------------------------
            ' 印刷対象日付(To)    取得 ※画面日付
            '--------------------------------------
            PrintDate_To = ParamSyoriDate
            If PrintDate_To = mMatchingDate Then
                PrintTime_To = mMatchingTime
            Else
                PrintTime_To = "999999"
            End If

            '--------------------------------------
            ' 印刷対象ステータス取得
            '--------------------------------------
            PrintStatus_From = Gcom.GetObjectParam("KFUP002", "PBDAY", "1")
            Select Case PrintStatus_From.ToUpper
                Case "ERR", ""
                    PrintStatus_From = "'7','9'"
            End Select

            PrintStatus_To = Gcom.GetObjectParam("KFUP002", "TODAY", "1")
            Select Case PrintStatus_To.ToUpper
                Case "ERR", ""
                    PrintStatus_To = "'4','7','9'"

            End Select

            '--------------------------------------
            ' ＣＳＶファイル作成
            '--------------------------------------
            If oraReader.DataReader(CreateGetListSQL) = True Then
                While oraReader.EOF = False
                    Call SetListCsv(oraReader)
                    RecordCnt += 1
                    oraReader.NextRead()
                End While
                MainLOG.Write("印刷メイン処理", "成功", "印刷対象:" & RecordCnt & " 件")
            Else
                MainLOG.Write("印刷メイン処理", "成功", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("印刷メイン処理", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not PrintDB Is Nothing Then
                PrintDB.Close()
                PrintDB = Nothing
            End If
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 印刷対象となるジョブを検索するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetListSQL() As StringBuilder

        Dim SQL As New StringBuilder

        With SQL
            .Length = 0
            .Append("SELECT")
            .Append("     *")
            .Append(" FROM")
            .Append("     JOBMAST")
            .Append(" WHERE")
            .Append("    (")
            .Append("        ( TOUROKU_DATE_J   >= '" & PrintDate_From & "'  AND  TOUROKU_DATE_J < '" & PrintDate_To & "' )")
            .Append("      AND STATUS_J         IN ( " & PrintStatus_From & " )")
            .Append("    )")
            .Append(" OR")
            .Append("    (")
            .Append("          TOUROKU_DATE_J    = '" & PrintDate_To & "'")
            .Append("      AND STATUS_J         IN ( " & PrintStatus_To & " )")
            If PrintTime_To <> "999999" Then
                .Append("  AND TOUROKU_TIME_J   <= '" & PrintTime_To & "'")
            End If
            .Append("    )")
            .Append(" OR")
            .Append("    (")
            .Append("        ( TOUROKU_DATE_J   >= '" & PrintDate_From & "'  AND  TOUROKU_DATE_J <= '" & PrintDate_To & "' )")
            .Append("      AND ERRMSG_J       LIKE '%二重持ち込み%'")
            .Append("    )")
            .Append(" ORDER BY")
            .Append("     TOUROKU_DATE_J")
            .Append("   , TUUBAN_J")
        End With

        Return SQL

    End Function

    ''' <summary>
    ''' 帳票CSVファイルに内容を出力します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsv(ByVal oraReader As CASTCommon.MyOracleReader)

        Try

            '--------------------------------------
            ' ヘッダ情報
            '--------------------------------------
            OutputCsvData(mMatchingDate, True)
            OutputCsvData(mMatchingTime, True)
            OutputCsvData(ParamSyoriDate, True)

            '--------------------------------------
            ' 明細情報
            '--------------------------------------
            Dim JikkouJyoukyou As String = String.Empty
            Select Case oraReader.GetString("STATUS_J")
                Case "0" : JikkouJyoukyou = "起動待ち"
                Case "1" : JikkouJyoukyou = "処理中"
                Case "2" : JikkouJyoukyou = "正常終了"
                Case "3" : JikkouJyoukyou = "処理中"
                Case "4" : JikkouJyoukyou = "キャンセル"
                Case "7" : JikkouJyoukyou = "異常終了"
                Case "8" : JikkouJyoukyou = "ＡＢＥＮＤ"
                Case "9" : JikkouJyoukyou = "解除待ち"
                Case Else : JikkouJyoukyou = "その他"
            End Select
            OutputCsvData(JikkouJyoukyou, True)
            OutputCsvData(oraReader.GetString("TOUROKU_DATE_J"), True)
            OutputCsvData(oraReader.GetString("TUUBAN_J"), True)
            OutputCsvData(oraReader.GetString("JOBID_J"), True)

            Dim SyoriName As String = Ex_GetText_CodeToName("ジョブ名管理.TXT", oraReader.GetString("JOBID_J"))
            OutputCsvData(SyoriName, True)

            OutputCsvData(oraReader.GetString("STA_TIME_J"), True)
            OutputCsvData(oraReader.GetString("END_TIME_J"), True)
            OutputCsvData(oraReader.GetString("USERID_J").Trim, True)
            OutputCsvData(oraReader.GetString("PARAMETA_J").Trim, True)
            Select Case oraReader.GetString("JOBID_J")
                Case "J010", "S010"
                    Dim ItakuNName As String = GetToriName(oraReader.GetString("JOBID_J"), oraReader.GetString("PARAMETA_J").Trim)
                    OutputCsvData(oraReader.GetString("ERRMSG_J").Trim & " (取引先名:" & ItakuNName & ")", True, True)
                Case Else
                    OutputCsvData(oraReader.GetString("ERRMSG_J").Trim, True, True)
            End Select

        Catch ex As Exception
            MainLOG.Write("ＣＳＶファイル作成", "失敗", ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' 印刷対象となるジョブを検索するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetToriName(ByVal JOBID As String, ByVal PARAMETER As String) As String

        Dim SQL As New StringBuilder
        Dim ToriReader As CASTCommon.MyOracleReader = Nothing

        Try
            With SQL
                .Length = 0
                .Append("SELECT")
                .Append("     ITAKU_NNAME_T")
                .Append(" FROM")
                Select Case JOBID
                    Case "J010"
                        .Append(" TORIMAST")
                    Case "S010"
                        .Append(" S_TORIMAST")
                End Select
                .Append(" WHERE")
                .Append("     TORIS_CODE_T = '" & PARAMETER.Substring(0, 10) & "'")
                .Append(" AND TORIF_CODE_T = '" & PARAMETER.Substring(10, 2) & "'")
            End With

            ToriReader = New CASTCommon.MyOracleReader(PrintDB)
            If ToriReader.DataReader(SQL) = True Then
                Return ToriReader.GetString("ITAKU_NNAME_T").Trim
            Else
                MainLOG.Write("取引先該当なし", "失敗", "取引先コード:" & PARAMETER.Substring(0, 10) & "-" & PARAMETER.Substring(10, 2))
                Return "取引先該当なし"
            End If

        Catch ex As Exception
            MainLOG.Write("取引先名取得", "失敗", "取引先コード:" & PARAMETER.Substring(0, 10) & "-" & PARAMETER.Substring(10, 2) & " " & ex.Message)
            Return "取引先名取得失敗"
        Finally
            If Not ToriReader Is Nothing Then
                ToriReader.Close()
                ToriReader = Nothing
            End If
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
