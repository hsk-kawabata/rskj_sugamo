Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFJP053

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFJP053"

        ' 定義体名セット
        ReportBaseName = "KFJP053_自振管理リスト.rpd"
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

            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")                     '取引先主コード
            SQL.Append(",TORIF_CODE_T")                     '取引先副コード
            SQL.Append(",ITAKU_CODE_T")                     '委託者コード
            SQL.Append(",ITAKU_NNAME_T")                    '委託者名
            SQL.Append(",TEKIYOU_KBN_T")                    '摘要区分
            SQL.Append(",KTEKIYOU_T")                       'カナ摘要
            SQL.Append(",NTEKIYOU_T")                       '漢字摘要
            SQL.Append(",FURI_DATE_S")                      '振替日
            SQL.Append(",KIGYO_CODE_T")                     '企業コード
            SQL.Append(",FURI_CODE_T")                      '振替コード
            SQL.Append(",SYORI_KEN_S")                      '処理件数
            SQL.Append(",SYORI_KIN_S")                      '処理金額
            SQL.Append(",TOUROKU_DATE_S")                   '登録日
            SQL.Append(",HAISIN_DATE_S")                    '配信日
            SQL.Append(",HAISIN_FLG_S")                     '配信フラグ
            SQL.Append(",FUNOU_FLG_S")                      '不能フラグ
            SQL.Append(",HENKAN_FLG_S")                     '返還フラグ
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND MOTIKOMI_KBN_T = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND FURI_DATE_S >= " & SQ(FuriDate))
            SQL.Append(" ORDER BY FURI_DATE_S,TORIS_CODE_S,TORIF_CODE_S")

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False

                    OutputCsvData(mMatchingDate, True)                                          '処理日
                    OutputCsvData(mMatchingTime, True)                                          'タイムスタンプ
                    OutputCsvData(FuriDate, True)                                               '入力振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_T")), True)        '取引先主コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_T")), True)        '取引先副コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T")), True)        '委託者コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)       '委託者名

                    Select Case GCOM.NzStr(oraReader.GetString("TEKIYOU_KBN_T"))

                        Case "0"
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("KTEKIYOU_T")), True)           'カナ摘要

                        Case "1"
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("NTEKIYOU_T")), True)           '漢字摘要

                        Case "2", "3"
                            OutputCsvData("データ摘要", True)                                           'データ摘要

                        Case Else
                            OutputCsvData("", True)

                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")), True)         '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), True)        '企業コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")), True)         '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt64("SYORI_KEN_S").ToString), True) '処理件数
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt64("SYORI_KIN_S").ToString), True) '処理金額

                    If GCOM.NzStr(oraReader.GetString("TOUROKU_DATE_S")) = "00000000" Then       '登録日
                        OutputCsvData("00010101", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("TOUROKU_DATE_S")), True)
                    End If

                    If GCOM.NzStr(oraReader.GetString("HAISIN_DATE_S")) = "00000000" Then           '配信日
                        OutputCsvData("00010101", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HAISIN_DATE_S")), True)
                    End If

                    If oraReader.GetString("HAISIN_FLG_S") = "0" Then                           '備考
                        OutputCsvData("未配信", True, True)
                    ElseIf oraReader.GetString("HAISIN_FLG_S") = "1" And oraReader.GetString("FUNOU_FLG_S") = "0" Then
                        OutputCsvData("配信済", True, True)
                    ElseIf oraReader.GetString("FUNOU_FLG_S") = "1" And oraReader.GetString("HENKAN_FLG_S") = "0" Then
                        OutputCsvData("未返還", True, True)
                    ElseIf oraReader.GetString("HENKAN_FLG_S") = "1" Then
                        OutputCsvData("返還済", True, True)
                    Else
                        OutputCsvData("", True, True)
                    End If

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
