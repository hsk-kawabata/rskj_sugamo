Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFJP031

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP031"

        ' 定義体名セット
        ReportBaseName = "KFJP031_契約者一覧表.rpd"
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

            SQL.Append(" SELECT ")
            SQL.Append(" TORIS_CODE_T")     '取引先主コード
            SQL.Append(",TORIF_CODE_T")     '取引先副コード
            SQL.Append(",ITAKU_NNAME_T")    '委託者漢字名
            SQL.Append(",KEIYAKU_NO_E")     '契約者番号
            SQL.Append(",TKIN_NO_E")        '金融機関コード
            SQL.Append(",KIN_NNAME_N")      '金融機関名
            SQL.Append(",TSIT_NO_E")        '支店コード
            SQL.Append(",SIT_NNAME_N")      '支店名
            SQL.Append(",KAMOKU_E")         '科目
            SQL.Append(",KOUZA_E")          '口座番号
            SQL.Append(",KEIYAKU_KNAME_E")  '契約者名カナ
            SQL.Append(",FURI_CODE_T")      '振替コード
            SQL.Append(",KIGYO_CODE_T")     '企業コード
            SQL.Append(" FROM TORIMAST,ENTMAST,TENMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = FSYORI_KBN_E")
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_E")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_E")
            If TorisCode + TorifCode <> "999999999999" Then
                SQL.Append(" AND TORIS_CODE_E = " & SQ(TorisCode))
                SQL.Append(" AND TORIF_CODE_E = " & SQ(TorifCode))
            End If
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '金融機関マスタに存在しない場合も出力する
            SQL.Append(" AND KIN_NO_N(+) = TKIN_NO_E")
            SQL.Append(" AND SIT_NO_N(+) = TSIT_NO_E")
            'SQL.Append(" AND KIN_NO_N = TKIN_NO_E")
            'SQL.Append(" AND SIT_NO_N = TSIT_NO_E")
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
            SQL.Append(" ORDER BY TORIS_CODE_E,TORIF_CODE_E,KEIYAKU_NO_E")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate, True)                                          '処理日
                    OutputCsvData(mMatchingTime, True)                                          'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T")), True)        '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T")), True)        '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)       '委託者漢字名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NO_E")), True)        '契約者番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_E")), True)           '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")), True)         '金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_E")), True)           '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")), True)         '支店名
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_E"))                     '科目
                        Case "02"
                            OutputCsvData("普通", True)
                        Case "01"
                            OutputCsvData("当座", True)
                        Case "37"
                            OutputCsvData("職員", True)
                        Case "05"
                            OutputCsvData("納税", True)
                        Case "04"
                            OutputCsvData("別段", True)
                        Case "99"
                            OutputCsvData("諸勘定", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_E")), True)             '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_KNAME_E")), True)     '契約者名カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")), True)         '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), True, True)  '企業コード

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
