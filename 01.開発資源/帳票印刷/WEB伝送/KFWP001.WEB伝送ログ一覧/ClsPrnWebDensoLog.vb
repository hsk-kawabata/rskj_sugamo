Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' WEB伝送ログ一覧表
Class ClsPrnWebDensoLog
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFWP001"

        ' 定義体名セット
        ReportBaseName = "KFWP001_WEB伝送ログ一覧.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' 機能　 ： WEB伝送ログ一覧をデータに書き込む
    '
    ' 備考　 ： 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    ' 機能   ： WEB伝送ログ一覧帳票出力処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考   ： 
    '
    Public Function MakeRecord() As Boolean
        Dim SQL As New StringBuilder(128)
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        SQL = New StringBuilder(128)
        SQL.Append("SELECT *")
        SQL.Append(" FROM WEB_RIREKIMAST")
        SQL.Append(" WHERE SAKUSEI_DATE_W = '" & txtSYORI_DATE & "'")
        SQL.Append(" AND STATUS_KBN_W <> '2'")
        SQL.Append(" ORDER BY SAKUSEI_TIME_W")

        Dim bSQL As Boolean

        Try
            bSQL = OraReader.DataReader(SQL)
            If bSQL = True Then
                Do
                    '処理日
                    OutputCsvData(mMatchingDate, True)
                    'タイムスタンプ
                    OutputCsvData(mMatchingTime, True)
                    '処理区分
                    Select Case OraReader.GetString("FSYORI_KBN_W")
                        Case "1"
                            OutputCsvData("口振", True)
                        Case "3"
                            OutputCsvData("総振", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    '伝送日付
                    OutputCsvData(OraReader.GetString("SAKUSEI_DATE_W"), True)
                    '伝送時間
                    OutputCsvData(OraReader.GetString("SAKUSEI_TIME_W"), True)
                    'ユーザ名
                    OutputCsvData(OraReader.GetString("USER_ID_W"), True)
                    'ファイル名
                    OutputCsvData(OraReader.GetString("FILE_NAME_W"), True)

                    '2012/12/05 saitou WEB伝送 UPD -------------------------------------------------->>>>
                    '印字内容に件数と金額を追加

                    '送受信
                    Select Case OraReader.GetString("STATUS_KBN_W")
                        Case "0", "1"
                            OutputCsvData("受付", True)
                        Case "3"
                            OutputCsvData("返却", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    '件数
                    OutputCsvData(OraReader.GetInt("END_KEN_W"))
                    '金額
                    OutputCsvData(OraReader.GetInt64("END_KIN_W"), False, True)

                    ''送受信
                    'Select Case OraReader.GetString("STATUS_KBN_W")
                    '    Case "0", "1"
                    '        OutputCsvData("受付", True, True)
                    '    Case "3"
                    '        OutputCsvData("返却", True, True)
                    '    Case Else
                    '        OutputCsvData("", True, True)
                    'End Select
                    '2012/12/05 saitou WEB伝送 UPD --------------------------------------------------<<<<

                    OraReader.NextRead()
                Loop Until OraReader.EOF
                Return True
            Else
                BatchLog.Write("レコード作成", "失敗", "印刷対象なし")
                RecordCnt = -1
                Return False
            End If
        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.ToString)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

End Class
