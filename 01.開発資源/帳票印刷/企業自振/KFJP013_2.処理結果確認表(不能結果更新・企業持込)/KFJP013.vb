Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFJP013

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP013_2"

        ' 定義体名セット
        ReportBaseName = "KFJP013_2_処理結果確認表(不能結果更新・企業持込).rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： CSVファイルに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '
    ' 機能　 ： 印刷データの作成
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As CASTCommon.MyOracle = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try
            oraDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT * FROM TORIMAST T1,SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            If TORIS_CODE & TORIF_CODE <> "999999999999" AndAlso TORIS_CODE & TORIF_CODE <> "" Then
                SQL.Append(" AND EXISTS(")
                SQL.Append(" SELECT * FROM TORIMAST T2")
                SQL.Append(" WHERE T2.TORIS_CODE_T = " & SQ(TORIS_CODE))
                SQL.Append(" AND T2.TORIF_CODE_T = " & SQ(TORIF_CODE))
                SQL.Append(" AND T1.ITAKU_KANRI_CODE_T = T2.ITAKU_KANRI_CODE_T")
                SQL.Append(")")
            End If
            SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))
            SQL.Append(" AND SOUSIN_KBN_S IN ('1','2')")
            SQL.Append(" AND HAISIN_FLG_S = '1'")
            SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" ORDER BY FURI_DATE_S, SOUSIN_KBN_S, ITAKU_KANRI_CODE_T, TORIS_CODE_S, TORIF_CODE_S")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False

                    '帳票印刷用
                    OutputCsvData(mMatchingDate, True)                                        'システム日付
                    OutputCsvData(mMatchingTime, True)                                        'タイムスタンプ
                    OutputCsvData(FURI_DATE, True)                                            '振替日
                    OutputCsvData(oraReader.GetString("TORIS_CODE_S"), True)                  '取引先主コード
                    OutputCsvData(oraReader.GetString("TORIF_CODE_S"), True)                  '取引先副コード
                    OutputCsvData(oraReader.GetString("ITAKU_NNAME_T"), True)                 '取引先名
                    OutputCsvData(oraReader.GetString("ITAKU_CODE_T"), True)                  '委託者コード
                    OutputCsvData(oraReader.GetString("SYORI_KEN_S"), True)                   '依頼件数
                    OutputCsvData(oraReader.GetString("SYORI_KIN_S"), True)                   '依頼金額
                    OutputCsvData(oraReader.GetString("FUNOU_KEN_S"), True)                   '不能件数 '2011/01/27 KIN→KENに修正
                    OutputCsvData(oraReader.GetString("FUNOU_KIN_S"), True)                   '不能金額
                    Select Case oraReader.GetString("SOUSIN_KBN_S")                           '備考
                        Case "1"
                            OutputCsvData("全銀", True)
                        Case "2"
                            OutputCsvData("地公体", True)
                    End Select
                    OutputCsvData(oraReader.GetString("FURI_CODE_T"), True)                   '振替コード
                    OutputCsvData(oraReader.GetString("KIGYO_CODE_T"), True, True)            '企業コード

                    oraReader.NextRead()
                    RecordCnt += 1
                End While
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function

End Class
