Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Collections.Generic
' 処理結果確認表(資金決済結果)印刷クラス
Class KFKP003
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFKP003"

        ' 定義体名セット
        ReportBaseName = "KFK003_処理結果確認表(資金決済結果).rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： 処理結果確認表(資金決済結果)をデータに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '更新項目構造体
    Public Structure UpdateInfo
        Public TorisCode As String
        Public TorifCode As String
        Public FuriDate As String
        Public RecordNo As Long
        Public ItakuNName As String
        Public Kamoku As String
        Public OpeCode As String
        Public ErrCode As String
        Public ErrMsg As String
        Public TimeStamp As String
        Public Bikou As String
    End Structure
    Public RecordCnt As Long
    '
    ' 機能　 ： 決済マスタから印刷情報の抽出を行う
    '
    ' 備考　 ： 
    '
    Public Function SetData(ByRef Item As UpdateInfo, ByVal MainDB As CASTCommon.MyOracle) As Boolean
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_KR")
            SQL.Append(",TORIF_CODE_KR")
            SQL.Append(",FURI_DATE_KR")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(" FROM KESSAIMAST,TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_KR ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_KR ")
            SQL.Append(" AND TIME_STAMP_KR  = " & SQ(Item.TimeStamp))
            SQL.Append(" AND KAMOKU_CODE_KR = " & SQ(Item.Kamoku))
            SQL.Append(" AND OPE_CODE_KR    = " & SQ(Item.OpeCode))
            SQL.Append(" AND RECORD_NO_KR   = " & Item.RecordNo)

            If oraReader.DataReader(SQL) = True Then
                Item.TorisCode = oraReader.GetString("TORIS_CODE_KR")
                Item.TorifCode = oraReader.GetString("TORIF_CODE_KR")
                Item.ItakuNName = oraReader.GetString("ITAKU_NNAME_T")
                Item.FuriDate = oraReader.GetString("FURI_DATE_KR")
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            MainLOG.Write("処理結果確認表(資金決済結果)印刷情報取得", "失敗", ex.Message)

        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
    '
    ' 機能　 ： 帳票印刷ＣＳＶファイルを作成する
    '
    ' 備考　 ： 
    '
    Public Function MakeRecord(ByVal DataList As List(Of UpdateInfo)) As Boolean
        Try
            If DataList.Count = 0 Then
                '印刷対象無し 
                MainLOG.Write("処理結果確認表(資金決済結果)印刷", "失敗", "印刷対象無し")

                Return False
            End If

            CreateCsvFile()
            Dim Today As String = Date.Now.ToString("yyyyMMdd")
            Dim Time As String = Date.Now.ToString("HHmmss")
            For No As Integer = 0 To DataList.Count - 1
                OutputCsvData(Today, True)                          '処理日
                OutputCsvData(Time, True)                           'タイムスタンプ
                OutputCsvData(DataList.Item(No).TorisCode, True)    '取引先主コード
                OutputCsvData(DataList.Item(No).TorifCode, True)    '取引先副コード
                OutputCsvData(DataList.Item(No).ItakuNName, True)   '委託者名
                OutputCsvData(DataList.Item(No).FuriDate, True)     '振替日
                OutputCsvData(DataList.Item(No).Kamoku, True)       '科目
                OutputCsvData(DataList.Item(No).OpeCode, True)      'オペコード
                OutputCsvData(DataList.Item(No).ErrCode, True)      'エラーコード
                OutputCsvData(DataList.Item(No).ErrMsg, True, True) 'エラーメッセージ
                RecordCnt += 1
            Next

            CloseCsv()
            Return True
        Catch ex As Exception
            MainLOG.Write("処理結果確認表(資金決済結果)印刷", "失敗", ex.ToString)

            Return False
        End Try
    End Function
End Class
