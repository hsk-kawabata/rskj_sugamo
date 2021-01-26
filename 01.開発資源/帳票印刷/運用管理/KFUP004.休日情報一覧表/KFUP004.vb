Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports CASTCommon

''' <summary>
''' 休日情報一覧表印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KFUP004
    Inherits CAstReports.ClsReportBase

#Region "クラス変数"

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFUP004"
        '定義体名セット
        ReportBaseName = "KFUP004_休日情報一覧表.rpd"
    End Sub

    ''' <summary>
    ''' 印刷用レコードを作成します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function MakeRecord() As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try

            '印刷対象の抽出
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(Me.CreateListSQL()) = True Then
                While OraReader.EOF = False
                    '印刷内容の設定
                    Me.SetListCsv(OraReader)
                    RecordCnt += 1
                    OraReader.NextRead()
                End While
            Else
                '印刷対象無し
                RecordCnt = -1
                BatchLog.Write("レコード作成", "失敗", "印刷対象無し")
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write("レコード作成", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 印刷対象を取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateListSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("SELECT ")
            .Append("     YASUMI_DATE_Y")
            .Append("    ,YASUMI_NAME_Y")
            .Append(" FROM ")
            .Append("     YASUMIMAST")
            .Append(" WHERE ")
            .Append("     SUBSTR(YASUMI_DATE_Y,1,4) = " & SQ(TargetNen))
            .Append(" ORDER BY")
            .Append("     YASUMI_DATE_Y")
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 印刷用CSVデータの出力を行います。
    ''' </summary>
    ''' <param name="dbReader"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsv(ByVal dbReader As CASTCommon.MyOracleReader)
        OutputCsvData(mMatchingDate, True)                                              '処理日
        OutputCsvData(mMatchingTime, True)                                              'タイムスタンプ
        OutputCsvData(dbReader.GetString("YASUMI_DATE_Y"), True)                        '登録日
        OutputCsvData(dbReader.GetString("YASUMI_NAME_Y"), True)                        '休日名称

        OutputCsvData("", , True)
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
