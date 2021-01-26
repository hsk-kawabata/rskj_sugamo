Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP025

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP025"

        ' 定義体名セット
        ReportBaseName = "KFGP025_学校マスタ索引簿.rpd"
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
            SQL.Append(" GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKKOU_KNAME_G")
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")
            SQL.Append(",GAKMAST2.TEKIYOU_KBN_T")
            SQL.Append(",GAKMAST2.KTEKIYOU_T")
            SQL.Append(",GAKMAST2.NTEKIYOU_T")
            SQL.Append(",GAKMAST2.SKYU_CODE_T")
            SQL.Append(",GAKMAST2.NKYU_CODE_T")
            SQL.Append(",GAKMAST2.TAKO_KBN_T")
            SQL.Append(",GAKMAST2.SFURI_SYUBETU_T")
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")
            SQL.Append(",GAKMAST2.FURI_DATE_T")
            SQL.Append(",GAKMAST2.SFURI_DATE_T")
            SQL.Append(",GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(",1 SORT")
            SQL.Append(" FROM  KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.GAKMAST2")
            SQL.Append(" WHERE GAKMAST1.GAKKOU_CODE_G = GAKMAST2.GAKKOU_CODE_T")
            SQL.Append(" AND GAKMAST1.GAKUNEN_CODE_G = 1")
            SQL.Append(" ORDER BY SORT")

            If PrintSort_Gakkou = "1" Then
                SQL.Append(",GAKMAST1.GAKKOU_CODE_G")
            End If

            If PrintSort_Waon = "1" Then
                SQL.Append(",GAKMAST1.GAKKOU_KNAME_G")
            End If

            If PrintSort_Furi = "1" Then
                SQL.Append(",GAKMAST2.FURI_DATE_T")
            End If

            If PrintSort_Itaku = "1" Then
                SQL.Append(",GAKMAST2.ITAKU_CODE_T")
            End If



            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_KNAME_G")), True)        '学校名カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T")))          '委託者コード
                    Select Case GCOM.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))            '摘要
                        Case "0"
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("KTEKIYOU_T")))
                        Case "1"
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("NTEKIYOU_T")))
                        Case Else
                            OutputCsvData("")
                    End Select
                    
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_T")))           '契約振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SFURI_DATE_T")))          '契約再振日

                    Select Case GCOM.NzStr(oraReader.GetString("SKYU_CODE_T"))              '出金休日コード
                        Case "0"
                            OutputCsvData("翌営業日振替")
                        Case "1"
                            OutputCsvData("前営業日振替")
                        Case Else
                            OutputCsvData("")
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("NKYU_CODE_T"))              '入金休日コード
                        Case "0"
                            OutputCsvData("翌営業日振替")
                        Case "1"
                            OutputCsvData("前営業日振替")
                        Case Else
                            OutputCsvData("")
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("TAKO_KBN_T"))               '他行区分
                        Case "0"
                            OutputCsvData("他行なし")
                        Case "1"
                            OutputCsvData("他行対象")
                        Case Else
                            OutputCsvData("")
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("SFURI_SYUBETU_T"))               '再振種別
                        Case "0"
                            OutputCsvData("再振なし 繰越なし")
                        Case "1"
                            OutputCsvData("再振あり 繰越なし")
                        Case "2"
                            OutputCsvData("再振あり 繰越あり")
                        Case "3"
                            OutputCsvData("再振なし 繰越あり")
                        Case Else
                            OutputCsvData("")
                    End Select


                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("GCOMMON", "TXT"), "GFJ_媒体.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("BAITAI_CODE_T"))), False, True)
                    'Select Case GCOM.NzStr(oraReader.GetString("BAITAI_CODE_T"))                '媒体
                    '    Case "0"
                    '        OutputCsvData("伝送", False, True)
                    '    Case "1"
                    '        OutputCsvData("FD", False, True)
                    '    Case "2"
                    '        OutputCsvData("紙", False, True)
                    '    Case Else
                    '        OutputCsvData("", False, True)
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

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
