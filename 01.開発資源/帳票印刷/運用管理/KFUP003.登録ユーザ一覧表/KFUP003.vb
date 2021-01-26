Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports CASTCommon

''' <summary>
''' 登録ユーザ一覧表印刷　メインクラス
''' </summary>
''' <remarks>2017/04/28 saitou RSV2 added for 標準機能追加(登録ユーザ一覧表)</remarks>
Public Class KFUP003
    Inherits CAstReports.ClsReportBase

#Region "クラス変数"

    '権限テキスト
    Private PowerText As CAstFormat.ClsText

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFUP003"
        '定義体名セット
        ReportBaseName = "KFUP003_登録ユーザ一覧表.rpd"
    End Sub

    ''' <summary>
    ''' 印刷用レコードを作成します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function MakeRecord() As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            '権限テキストの読込
            Me.PowerText = New CAstFormat.ClsText("KFUMAST040_ユーザ権限.TXT")

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
            .Append("select")
            .Append("     LOGINID_U")
            .Append("    ,KENGEN_U")
            .Append("    ,SAKUSEI_DATE_U")
            .Append("    ,KOUSIN_DATE_U")
            .Append(" from")
            .Append("     UIDMAST")
            .Append(" order by")
            .Append("     LOGINID_U")
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
        OutputCsvData(dbReader.GetString("LOGINID_U"), True)                            'ユーザID
        OutputCsvData(Me.PowerText.GetBaitaiCode(dbReader.GetString("KENGEN_U")), True) '権限
        OutputCsvData(dbReader.GetString("SAKUSEI_DATE_U"), True)                       '作成日
        'パスワードの更新等を行っていないユーザは更新日がALL0だったりするので、更新日のチェックをする
        Dim KousinDate As String = dbReader.GetString("KOUSIN_DATE_U").PadLeft(8, "0"c)
        Dim WorkDate As String = KousinDate.Substring(0, 4) & "/" & KousinDate.Substring(4, 2) & "/" & KousinDate.Substring(6, 2)
        If Information.IsDate(WorkDate) = False Then
            OutputCsvData("", True, True)
        Else
            OutputCsvData(dbReader.GetString("KOUSIN_DATE_U"), True, True)
        End If
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
