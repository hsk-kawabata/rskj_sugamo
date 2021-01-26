Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP009

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP009"

        ' 定義体名セット
        ReportBaseName = "KFGP009_学校他行マスタ一覧.rpd"
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
        Dim GakCode As String = ""
        Dim No As Integer = 0
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT")
            SQL.Append(" GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",TENMAST.KIN_NO_N")
            SQL.Append(",TENMAST.SIT_NO_N")
            SQL.Append(",TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(",G_TAKOUMAST.GAKKOU_CODE_V")
            SQL.Append(",G_TAKOUMAST.KOUZA_V")
            SQL.Append(",G_TAKOUMAST.ITAKU_CODE_V")
            SQL.Append(",G_TAKOUMAST.BAITAI_CODE_V")
            SQL.Append(",G_TAKOUMAST.CODE_KBN_V")
            SQL.Append(",G_TAKOUMAST.SFILE_NAME_V")
            SQL.Append(",G_TAKOUMAST.RFILE_NAME_V")
            SQL.Append(",G_TAKOUMAST.TKIN_NO_V")
            SQL.Append(",G_TAKOUMAST.TSIT_NO_V")
            SQL.Append(",G_TAKOUMAST.KAMOKU_V")
            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.G_TAKOUMAST")
            SQL.Append(",KZFMAST.TENMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(" WHERE G_TAKOUMAST.TKIN_NO_V = TENMAST.KIN_NO_N (+)")
            SQL.Append(" AND G_TAKOUMAST.TSIT_NO_V = TENMAST.SIT_NO_N (+) ")
            SQL.Append(" AND G_TAKOUMAST.GAKKOU_CODE_V = GAKMAST1.GAKKOU_CODE_G (+) ")
            If GakkouCode <> "9999999999" Then
                SQL.Append(" AND GAKMAST1.GAKKOU_CODE_G = " & GakkouCode)
            End If
            SQL.Append(" AND GAKMAST1.GAKUNEN_CODE_G = 1")
            SQL.Append(" ORDER BY GAKKOU_CODE_V ,TKIN_NO_V ,TSIT_NO_V")


            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_V")))         '学校コード
                    If oraReader.GetString("GAKKOU_CODE_V") <> GakCode Then
                        GakCode = oraReader.GetString("GAKKOU_CODE_V")
                        No += 1
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_V")))             '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_V")))             '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")))           '金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))           '支店名
                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_V"))                 '科目
                        Case "02"
                            OutputCsvData("普通")
                        Case "01"
                            OutputCsvData("当座")
                        Case "37"
                            OutputCsvData("職員")
                        Case "05"
                            OutputCsvData("納税")
                        Case "09"
                            OutputCsvData("その他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_V")))               '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_V")))          '委託者コード
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("GCOMMON", "TXT"), "KZFMAST021_他行媒体.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("BAITAI_CODE_V"))))
                    'Select Case GCOM.NzStr(oraReader.GetString("BAITAI_CODE_V"))            '媒体コード
                    '    Case "00"
                    '        OutputCsvData("伝送")
                    '    Case "01"
                    '        OutputCsvData("FD3.5")
                    '    Case "04"
                    '        OutputCsvData("依頼書")
                    '    Case "06"
                    '        OutputCsvData("CMT")
                    '    Case Else
                    '        OutputCsvData("")
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                    Select Case GCOM.NzStr(oraReader.GetString("CODE_KBN_V"))               'コード区分
                        Case "0"
                            OutputCsvData("JIS")
                        Case "1"
                            OutputCsvData("JIS改")
                        Case "4"
                            OutputCsvData("EBCDIC")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SFILE_NAME_V")))          '送信ファイル名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("RFILE_NAME_V"))) '受信ファイル名
                    If GakkouCode <> "9999999999" Then
                        OutputCsvData("", False, True)
                    Else
                        OutputCsvData(No.ToString, False, True)
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
