Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Public Class KFGP005

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP005"

        ' 定義体名セット
        ReportBaseName = "KFGP005_口座振替予定集計表.rpd"
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
            SQL.Append(" GAKKOU_CODE_G")
            SQL.Append(",GAKKOU_NNAME_G")
            SQL.Append(",GAKUNEN_CODE_G")
            SQL.Append(",GAKUNEN_NAME_G")
            SQL.Append(",FURI_DATE_M")
            SQL.Append(",SEIKYU_KIN_M")
            SQL.Append(" FROM GAKMAST1")
            SQL.Append(" ,GAKMAST2")
            SQL.Append(" ,G_MEIMAST")
            SQL.Append(" WHERE GAKKOU_CODE_T = GAKKOU_CODE_G")
            SQL.Append(" AND GAKKOU_CODE_G = GAKKOU_CODE_M")
            SQL.Append(" AND GAKUNEN_CODE_G = GAKUNEN_CODE_M")
            SQL.Append(" AND GAKKOU_CODE_M = " & SQ(GakkouCode))
            SQL.Append(" AND YOBI1_M = " & SQ(SyoriDate))
            SQL.Append(" ORDER BY GAKKOU_CODE_G,GAKUNEN_CODE_G,CLASS_CODE_M,SEITO_NO_M")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(Mid(SyoriDate, 1, 8))                                     '処理日
                    OutputCsvData(Mid(SyoriDate, 9))                                        'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_M")))           '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_G")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))        '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_KIN_M")), False, True)   '契約金額
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
