Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP031

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP031"

        ' 定義体名セット
        ReportBaseName = "KFGP031_学校自振口座チェックリスト.rpd"
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
            SQL.Append(" GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(",PR_KOUZAMAST.GAKKOU_CODE_P")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",PR_KOUZAMAST.NENDO_P")
            SQL.Append(",PR_KOUZAMAST.TUUBAN_P")
            SQL.Append(",PR_KOUZAMAST.GAKUNEN_CODE_P")
            SQL.Append(",PR_KOUZAMAST.CLASS_CODE_P")
            SQL.Append(",PR_KOUZAMAST.SEITO_NO_P")
            SQL.Append(",PR_KOUZAMAST.MEIGI_KNAME_P")
            SQL.Append(",PR_KOUZAMAST.KEKKA_P")
            SQL.Append(",PR_KOUZAMAST.RE_MEIGI_KNAME_P")
            SQL.Append(",PR_KOUZAMAST.TSIT_NO_P")
            SQL.Append(",PR_KOUZAMAST.KOUZA_P")
            SQL.Append(",PR_KOUZAMAST.KAMOKU_P")
            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.PR_KOUZAMAST")
            SQL.Append(",KZFMAST.GAKMAST1")

            SQL.Append(" WHERE PR_KOUZAMAST.GAKKOU_CODE_P = GAKMAST1.GAKKOU_CODE_G (+)")
            SQL.Append(" AND PR_KOUZAMAST.GAKUNEN_CODE_P = GAKMAST1.GAKUNEN_CODE_G (+)")
            If GakkouCode <> "9999999999" Then
                SQL.Append(" AND PR_KOUZAMAST.GAKKOU_CODE_P = " & SQ(GakkouCode))
            End If
            SQL.Append(" ORDER BY PR_KOUZAMAST.GAKKOU_CODE_P,PR_KOUZAMAST.GAKUNEN_CODE_P")
            Select Case PrintSort
                Case "0"
                    SQL.Append(",PR_KOUZAMAST.CLASS_CODE_P")
                    SQL.Append(",PR_KOUZAMAST.SEITO_NO_P")
                    SQL.Append(",PR_KOUZAMAST.NENDO_P")
                    SQL.Append(",PR_KOUZAMAST.TUUBAN_P")
                Case "1"
                    SQL.Append(",PR_KOUZAMAST.NENDO_P")
                    SQL.Append(",PR_KOUZAMAST.TUUBAN_P")
                Case Else
                    SQL.Append(",PR_KOUZAMAST.MEIGI_KNAME_P")
                    SQL.Append(",PR_KOUZAMAST.NENDO_P")
                    SQL.Append(",PR_KOUZAMAST.TUUBAN_P")
            End Select

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_P")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")), True)  '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_P")))               '入学年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_P")))              '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_P")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_P")))          'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_P")))            '生徒番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_P")))             '支店コード
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_P"))                 '科目
                        Case "01"
                            OutputCsvData("当")
                        Case "02"
                            OutputCsvData("普")
                        Case "09"
                            OutputCsvData("他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_P")))               '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_P")), True)         '名義人カナ名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("RE_MEIGI_KNAME_P")), True)      '元帳カナ名

                    ' 2017/05/09 タスク）綾部 CHG 【OM】(RSV2対応 機能追加) -------------------- START
                    'Select Case GCOM.NzStr(oraReader.GetString("KEKKA_P"))                  '備考
                    '    Case "1"
                    '        OutputCsvData("自振契約ありカナ不一致", False, True)
                    '    Case "2"
                    '        OutputCsvData("自振契約なし", False, True)
                    '    Case "3"
                    '        OutputCsvData("自振契約なしカナ不一致", False, True)
                    '    Case "4"
                    '        OutputCsvData("口座なし", False, True)
                    '    Case Else
                    '        OutputCsvData("", False, True)
                    'End Select
                    Select Case GCOM.NzStr(oraReader.GetString("KEKKA_P"))                  '備考
                        Case "1"
                            OutputCsvData("自振契約なし", False, True)
                        Case "2"
                            OutputCsvData("自振契約なしカナ不一致", False, True)
                        Case "3"
                            OutputCsvData("自振契約ありカナ不一致", False, True)
                        Case "4"
                            OutputCsvData("カナ不一致", False, True)
                        Case "9"
                            OutputCsvData("口座なし", False, True)
                            '2017/06/23 saitou ADD 標準版修正 ---------------------------------------- START
                        Case "8"
                            OutputCsvData("解約済", False, True)
                            '2017/06/23 saitou ADD 標準版修正 ---------------------------------------- END
                        Case Else
                            OutputCsvData("", False, True)
                    End Select
                    ' 2017/05/09 タスク）綾部 CHG 【OM】(RSV2対応 機能追加) -------------------- END

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
