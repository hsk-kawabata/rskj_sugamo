Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP011

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP011"

        ' 定義体名セット
        ReportBaseName = "KFGP011_進級リスト.rpd"
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

            SQL.Append(" SELECT")
            SQL.Append(" SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(",SEITOMAST.GAKUNEN_CODE_O")
            SQL.Append(",SEITOMAST.CLASS_CODE_O")
            SQL.Append(",SEITOMAST.SEITO_NO_O")
            SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            SQL.Append(",SEITOMAST.SEITO_NNAME_O")
            SQL.Append(",SEITOMAST.SEIBETU_O")
            SQL.Append(",SEITOMAST.TSIT_NO_O")
            SQL.Append(",SEITOMAST.KAMOKU_O")
            SQL.Append(",SEITOMAST.KOUZA_O")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G")
        
            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.SEITOMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = GAKMAST1.GAKKOU_CODE_G(+)")
            SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O = GAKMAST1.GAKUNEN_CODE_G(+) ")
            SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O =" & SQ(GakkouCode))
            SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O =" & Gakunen)
            '同一生徒マスタは各月で存在するため０４月の条件を設定
            SQL.Append(" AND SEITOMAST.TUKI_NO_O = '04'")
            SQL.Append(" ORDER BY SEITOMAST.GAKKOU_CODE_O,SEITOMAST.GAKUNEN_CODE_O")
            Select Case PrintSort
                Case "0"
                    SQL.Append(",SEITOMAST.CLASS_CODE_O")
                    SQL.Append(",SEITOMAST.SEITO_NO_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case "1"
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case Else
                    SQL.Append(",SEITOMAST.SEITO_KNAME_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
            End Select

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_O")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))        '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True) '生徒名カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O"))) '生徒名漢字
                    Select Case GCOM.NzStr(oraReader.GetString("SEIBETU_O"))   '性別
                        Case "0"
                            OutputCsvData("男")
                        Case "1"
                            OutputCsvData("女")
                        Case "2"
                            OutputCsvData("-")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_O"))) '店舗コード
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_O")) '科目
                        Case "01"
                            OutputCsvData("当座")
                        Case "02"
                            OutputCsvData("普通")
                        Case "09"
                            OutputCsvData("その他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_O"))) '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_O"))) 'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_O")), False, True) '生徒番号
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
