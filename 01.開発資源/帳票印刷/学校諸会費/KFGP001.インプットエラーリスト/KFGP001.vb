Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP001

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP001"

        ' 定義体名セット
        ReportBaseName = "KFGP001_インプットエラーリスト.rpd"
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
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append(" GAKKOU_CODE_L")
            SQL.Append(",GAKKOU_NNAME_G")
            SQL.Append(",GAKUNEN_CODE_L")
            SQL.Append(",CLASS_CODE_L")
            SQL.Append(",FURI_DATE_L")
            SQL.Append(",SEITO_NO_L")
            SQL.Append(",KINKO_NO_L")
            SQL.Append(",SITEN_NO_L")
            SQL.Append(",KAMOKU_L")
            SQL.Append(",KOUZA_L")
            SQL.Append(",KEIYAKU_KNAME_L")
            SQL.Append(",FKINGAKU_L")
            SQL.Append(",ERRMSG_L")
            SQL.Append(" FROM GAKMAST1")
            SQL.Append(" ,G_IJYOLIST")
            SQL.Append(" WHERE GAKKOU_CODE_L = GAKKOU_CODE_G")
            SQL.Append(" AND GAKUNEN_CODE_L = GAKUNEN_CODE_G")
       
            SQL.Append(" ORDER BY GAKKOU_CODE_L,KINKO_NO_L,RECORD_NO_L")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_L")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_L")))           '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_L")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_L")))        'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_L")))        '生徒番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KINKO_NO_L")))        '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SITEN_NO_L")))        '店番
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_L")) '科目
                        Case "01"
                            OutputCsvData("当座")
                        Case "02"
                            OutputCsvData("普通")
                        Case Else
                            OutputCsvData("その他")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_L")))        '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KNAME_L")), True)        '名義人名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FKINGAKU_L")))        '振替金額
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ERRMSG_L")), False, True)   'エラー情報
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
