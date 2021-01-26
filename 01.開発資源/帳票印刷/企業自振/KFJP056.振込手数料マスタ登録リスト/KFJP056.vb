Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 振込手数料マスタ登録リスト印刷　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class KFJP056
    Inherits CAstReports.ClsReportBase

    Public Sub New()
        'CSVファイルセット
        InfoReport.ReportName = "KFJP056"
        '定義体名セット
        ReportBaseName = "KFJP056_振込手数料マスタ登録リスト.rpd"
    End Sub
    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "クラス変数"

    Public FSYORI_KBN As String
    '2014/01/15 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
    Public PARA_SYS_DATE As String
    Private Structure strcListInshizei
        Dim Inshizei1 As String
        Dim Inshizei2 As String
        Dim Inshizei3 As String
    End Structure
    Private ListInshizei As strcListInshizei
    '2014/01/15 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

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
            '2014/01/15 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
            '印紙税を取得する
            Dim TAX As New CASTCommon.ClsTAX
            TAX.GetInshizei(Me.PARA_SYS_DATE)
            If TAX.INSHIZEI_ID.Equals("err") = True Then
                '印紙税が設定されていない
                Me.ListInshizei.Inshizei1 = "1万円未満"
                Me.ListInshizei.Inshizei2 = "1万円以上3万円未満"
                Me.ListInshizei.Inshizei3 = "3万円以上"
            Else
                '金額によって表示形式を変える
                Dim strInshizei1 As String
                Dim strInshizei2 As String
                If TAX.INSHIZEI1 >= 10000 Then
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1 / 10000) & "万"
                ElseIf TAX.INSHIZEI1 >= 1000 Then
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1 / 1000) & "千"
                Else
                    strInshizei1 = String.Format("{0:#,##0}", TAX.INSHIZEI1)
                End If

                If TAX.INSHIZEI2 >= 10000 Then
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2 / 10000) & "万"
                ElseIf TAX.INSHIZEI1 >= 1000 Then
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2 / 1000) & "千"
                Else
                    strInshizei2 = String.Format("{0:#,##0}", TAX.INSHIZEI2)
                End If

                Me.ListInshizei.Inshizei1 = strInshizei1 & "円未満"
                Me.ListInshizei.Inshizei2 = strInshizei1 & "円以上" & strInshizei2 & "円未満"
                Me.ListInshizei.Inshizei3 = strInshizei2 & "円以上"
            End If
            '2014/01/15 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<


            If oraReader.DataReader(CreateGetListSQL) = True Then
                While oraReader.EOF = False
                    Call SetListCsv(oraReader)
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
    ''' 印刷対象を検索するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetListSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from TESUUMAST, TAXMAST")
            .Append(" where TAX_ID_C = TAX_ID_Z")
            .Append(" and FSYORI_KBN_C = " & SQ(Me.FSYORI_KBN))
            .Append(" order by TAX_ID_C, TESUU_TABLE_ID_C, SYUBETU_C")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 帳票CSVファイルに内容を出力します。
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <remarks></remarks>
    Private Sub SetListCsv(ByVal oraReader As CASTCommon.MyOracleReader)
        OutputCsvData(mMatchingDate, True)
        OutputCsvData(mMatchingTime, True)
        OutputCsvData(oraReader.GetString("TAX_ID_C"), True)
        OutputCsvData(oraReader.GetString("TAX_Z"), True)
        OutputCsvData(oraReader.GetString("TESUU_TABLE_ID_C"), True)
        OutputCsvData(oraReader.GetString("TESUU_TABLE_NAME_C"), True)
        Select Case oraReader.GetString("SYUBETU_C")
            Case "91", "10"
                OutputCsvData("振込手数料", True)
            Case "11"
                OutputCsvData("先振手数料", True)
            Case "12"
                OutputCsvData("給振手数料", True)
        End Select

        OutputCsvData(oraReader.GetInt("TESUU_A1_C"))
        OutputCsvData(oraReader.GetInt("TESUU_A2_C"))
        OutputCsvData(oraReader.GetInt("TESUU_A3_C"))
        OutputCsvData(oraReader.GetInt("TESUU_B1_C"))
        OutputCsvData(oraReader.GetInt("TESUU_B2_C"))
        OutputCsvData(oraReader.GetInt("TESUU_B3_C"))
        OutputCsvData(oraReader.GetInt("TESUU_C1_C"))
        OutputCsvData(oraReader.GetInt("TESUU_C2_C"))
        '2014/01/15 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
        '表示内容に印紙税を追加する
        'OutputCsvData(oraReader.GetInt("TESUU_C3_C"), False, True)
        OutputCsvData(oraReader.GetInt("TESUU_C3_C"))
        OutputCsvData(Me.ListInshizei.Inshizei1, True)
        OutputCsvData(Me.ListInshizei.Inshizei2, True)
        OutputCsvData(Me.ListInshizei.Inshizei3, True, True)
        '2014/01/15 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
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
