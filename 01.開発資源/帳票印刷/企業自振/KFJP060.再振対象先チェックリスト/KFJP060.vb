Imports System
Imports System.Text
Imports CASTCommon
Imports System.IO

''' <summary>
''' 再振対象先チェックリスト印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KFJP060
    Inherits CAstReports.ClsReportBase

    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFJP060"
        '定義体名セット
        ReportBaseName = "KFJP060_再振対象先チェックリスト.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "クラス変数"

    Public PARA_SYS_DATE As String

    Private SfuriCode As String
    Private TxtFolder As String

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
            MainLOG.Write("印刷メイン処理", "開始", "")

            TxtFolder = GetFSKJIni("COMMON", "TXT")
            SfuriCode = GetRSKJIni("RSV2_V1.0.0", "SFURI_CODE")
            If SfuriCode = "err" OrElse SfuriCode = "" Then
                SfuriCode = ""
                MainLOG.Write("印刷メイン処理", "成功", "[RSV2_V1.0.0]-[SFURI_CODE] 再振対象振替結果指定なし")
            Else
                MainLOG.Write("印刷メイン処理", "成功", "[RSV2_V1.0.0]-[SFURI_CODE] 再振対象振替結果指定:" & SfuriCode)
            End If

            If oraReader.DataReader(CreateGetListSQL) = True Then
                While oraReader.EOF = False
                    If Check_SaifuriMeisai(MainDB, oraReader) = True Then
                        Call SetListCsv(oraReader)
                        RecordCnt += 1
                    End If
                    oraReader.NextRead()
                End While
            Else
                Call SetListCsv_NoData()
                RecordCnt += 1
                MainLOG.Write("印刷メイン処理", "成功", "印刷対象なし")
            End If

            If RecordCnt = 0 Then
                Call SetListCsv_NoData()
                RecordCnt += 1
                MainLOG.Write("印刷メイン処理", "成功", "印刷対象なし(RecordCnt = 0)")
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
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 再振契約有りの結果更新済み初振スケジュールを検索するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetListSQL() As StringBuilder

        Dim SQL As New StringBuilder

        With SQL
            .Length = 0
            .Append(" SELECT")
            .Append("     TORIMAST.TORIS_CODE_T       TORIS_CODE")
            .Append("   , TORIMAST.TORIF_CODE_T       TORIF_CODE")
            .Append("   , TORIMAST.ITAKU_NNAME_T      ITAKU_NNAME")
            .Append("   , SCHMAST.FURI_DATE_S         FURI_DATE")
            .Append("   , SCHMAST.KSAIFURI_DATE_S     SAIFURI_DATE")
            .Append("   , TORIMAST.BAITAI_CODE_T      BAITAI_CODE")
            .Append(" FROM")
            .Append("     TORIMAST")
            .Append("   , SCHMAST")
            .Append(" WHERE")
            .Append("     SCHMAST.HENKAN_YDATE_S   = '" & PARA_SYS_DATE & "'")
            .Append(" AND SCHMAST.TOUROKU_FLG_S    = '1'")
            .Append(" AND SCHMAST.FUNOU_FLG_S      = '1'")
            .Append(" AND SCHMAST.TYUUDAN_FLG_S    = '0'")
            .Append(" AND SCHMAST.FILE_SEQ_S       = 1")
            .Append(" AND SCHMAST.TORIS_CODE_S     = TORIMAST.TORIS_CODE_T")
            .Append(" AND SCHMAST.TORIF_CODE_S     = TORIMAST.TORIF_CODE_T")
            .Append(" AND TORIMAST.SFURI_FLG_T     = '1'")
            .Append(" AND TORIMAST.BAITAI_CODE_T  <> '07'")
            .Append(" UNION ")
            .Append(" SELECT")
            .Append("     TORIMAST.TORIS_CODE_T       TORIS_CODE")
            .Append("   , TORIMAST.TORIF_CODE_T       TORIF_CODE")
            .Append("   , TORIMAST.ITAKU_NNAME_T      ITAKU_NNAME")
            .Append("   , SCHMAST.FURI_DATE_S         FURI_DATE")
            .Append("   , G_SCHMAST.SFURI_DATE_S      SAIFURI_DATE")
            .Append("   , TORIMAST.BAITAI_CODE_T      BAITAI_CODE")
            .Append(" FROM")
            .Append("     TORIMAST")
            .Append("   , SCHMAST")
            .Append("   , GAKMAST2")
            .Append("   , G_SCHMAST")
            .Append(" WHERE")
            .Append("     SCHMAST.HENKAN_YDATE_S   = '" & PARA_SYS_DATE & "'")
            .Append(" AND SCHMAST.TOUROKU_FLG_S    = '1'")
            .Append(" AND SCHMAST.FUNOU_FLG_S      = '1'")
            .Append(" AND SCHMAST.TYUUDAN_FLG_S    = '0'")
            .Append(" AND SCHMAST.FILE_SEQ_S       = 1")
            .Append(" AND SCHMAST.TORIF_CODE_S     = '01'")
            .Append(" AND SCHMAST.TORIS_CODE_S     = TORIMAST.TORIS_CODE_T")
            .Append(" AND SCHMAST.TORIF_CODE_S     = TORIMAST.TORIF_CODE_T")
            .Append(" AND TORIMAST.TORIF_CODE_T    = '01'")
            .Append(" AND TORIMAST.BAITAI_CODE_T   = '07'")
            .Append(" AND TORIMAST.TORIS_CODE_T    = GAKMAST2.GAKKOU_CODE_T")
            .Append(" AND GAKMAST2.SFURI_SYUBETU_T = '1'")
            .Append(" AND SCHMAST.TORIS_CODE_S     = G_SCHMAST.GAKKOU_CODE_S")
            .Append(" AND SCHMAST.FURI_DATE_S      = G_SCHMAST.FURI_DATE_S")
            .Append(" AND G_SCHMAST.SCH_KBN_S      = '0'")
            .Append(" AND G_SCHMAST.FURI_KBN_S     = '0'")
            .Append(" ORDER BY")
            .Append("     TORIS_CODE")
            .Append("   , TORIF_CODE")
        End With

        Return SQL

    End Function

    ''' <summary>
    ''' 検索した初振スケジュールから再振対象の明細有無をチェックします。
    ''' </summary>
    ''' <returns>再振明細チェック</returns>
    ''' <remarks></remarks>
    Private Function Check_SaifuriMeisai(ByVal MainDB As MyOracle, ByVal oraReader As MyOracleReader) As Boolean

        Dim MeiReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            Dim TorisCode As String = oraReader.GetString("TORIS_CODE")
            Dim TorifCode As String = oraReader.GetString("TORIF_CODE")
            Dim FuriDate As String = oraReader.GetString("FURI_DATE")

            MainLOG.Write("再振明細チェック", "開始", "取引先コード:" & TorisCode & "-" & TorifCode & " / 振替日:" & FuriDate)

            With SQL
                .Length = 0
                .Append(" SELECT")
                .Append("     COUNT(*) CNT")
                .Append(" FROM")
                .Append("     SCHMAST")
                .Append("   , MEIMAST")
                .Append(" WHERE")
                .Append("     SCHMAST.TORIS_CODE_S     = " & SQ(TorisCode))
                .Append(" AND SCHMAST.TORIF_CODE_S     = " & SQ(TorifCode))
                .Append(" AND SCHMAST.FURI_DATE_S      = " & SQ(FuriDate))
                .Append(" AND SCHMAST.TORIS_CODE_S     = MEIMAST.TORIS_CODE_K")
                .Append(" AND SCHMAST.TORIF_CODE_S     = MEIMAST.TORIF_CODE_K")
                .Append(" AND SCHMAST.FURI_DATE_S      = MEIMAST.FURI_DATE_K")
                .Append(" AND MEIMAST.DATA_KBN_K       = '2'")
                .Append(" AND MEIMAST.FURIKETU_CODE_K <> 0")
                If SfuriCode <> "" Then
                    .Append(" AND MEIMAST.FURIKETU_CODE_K IN (" & SfuriCode & ")")
                End If
            End With

            MeiReader = New CASTCommon.MyOracleReader(MainDB)
            If MeiReader.DataReader(SQL) = True Then
                Dim SfuriMeisaiKen As Integer = MeiReader.GetInt("CNT")
                If SfuriMeisaiKen > 0 Then
                    MainLOG.Write("再振明細チェック", "成功", "再振対象明細件数: " & SfuriMeisaiKen & " 件")
                    Return True
                Else
                    MainLOG.Write("再振明細チェック", "成功", "再振対象明細件数: 0 件")
                    Return False
                End If
            Else
                MainLOG.Write("再振明細チェック", "失敗", "明細検索失敗")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("再振明細チェック", "失敗", ex.Message)
            Return False
        Finally
            If Not MeiReader Is Nothing Then
                MeiReader.Close()
                MeiReader = Nothing
            End If
            MainLOG.Write("再振明細チェック", "終了", "")
        End Try

    End Function

    ''' <summary>
    ''' 帳票CSVファイルに内容を出力します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsv(ByVal oraReader As CASTCommon.MyOracleReader)

        OutputCsvData(mMatchingDate, True)
        OutputCsvData(mMatchingTime, True)

        OutputCsvData(oraReader.GetString("TORIS_CODE"), True)
        OutputCsvData(oraReader.GetString("TORIF_CODE"), True)
        OutputCsvData(oraReader.GetString("ITAKU_NNAME"), True)

        OutputCsvData(oraReader.GetString("FURI_DATE"), True)
        OutputCsvData(oraReader.GetString("SAIFURI_DATE"), True)

        Dim BaitaiName As String = CASTCommon.GetText_CodeToName(Path.Combine(TxtFolder, "Common_媒体コード.TXT"), _
                                                                 oraReader.GetString("BAITAI_CODE"))
        OutputCsvData(StrConv(BaitaiName.Trim, VbStrConv.Wide), True, True)

    End Sub

    ''' <summary>
    ''' 帳票CSVファイル(対象0件時)に内容を出力します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetListCsv_NoData()

        OutputCsvData(mMatchingDate, True)
        OutputCsvData(mMatchingTime, True)

        OutputCsvData("", True)
        OutputCsvData("", True)
        OutputCsvData("再振対象なし", True)

        OutputCsvData("", True)
        OutputCsvData("", True)

        OutputCsvData("", True, True)

    End Sub

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
