Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Public Class KFGP012

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP012"

        ' 定義体名セット
        ReportBaseName = "KFGP012_月間スケジュール表.rpd"
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
        Dim Youbi(31) As String
        Dim Yotei(31) As String
        Const Yasumi As String = "＊"
        Dim SetDate As Date
        Dim sTemp As String = Nothing
        Dim sTemp2 As String = Nothing
        Try
            Dim bRet As Boolean = GCOM.CheckDateModule(Nothing, 1)

            If bRet = False Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                Exit Function
            End If

            For No As Integer = 1 To 31
                If IsDate(Mid(Nentuki, 1, 4) & "/" & Mid(Nentuki, 5, 2) & "/" & No) = False Then
                    Youbi(No) = Yasumi '存在しない日付
                Else
                    SetDate = New Date(Mid(Nentuki, 1, 4), Mid(Nentuki, 5, 2), No)
                    sTemp = SetDate.ToString("yyyyMMdd")
                    If Not GCOM.CheckDateModule(sTemp, sTemp2, 0) Then
                        Youbi(No) = Yasumi '非営業日
                    Else
                        Select Case Weekday(SetDate)
                            Case 1  '日曜日
                                Youbi(No) = "日"
                            Case 2  '月曜日
                                Youbi(No) = "月"
                            Case 3  '火曜日
                                Youbi(No) = "火"
                            Case 4  '水曜日
                                Youbi(No) = "水"
                            Case 5  '木曜日
                                Youbi(No) = "木"
                            Case 6  '金曜日
                                Youbi(No) = "金"
                            Case 7  '土曜日
                                Youbi(No) = "土"
                        End Select
                    End If
                End If
            Next

            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append(" GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")
            SQL.Append(",G_SCHMAST.ENTRI_FLG_S")
            SQL.Append(",G_SCHMAST.CHECK_FLG_S")
            SQL.Append(",G_SCHMAST.DATA_FLG_S")
            SQL.Append(",G_SCHMAST.FUNOU_FLG_S")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")
            SQL.Append(",G_SCHMAST.DATA_YDATE_S")
            SQL.Append(",G_SCHMAST.FURI_DATE_S")
            SQL.Append(",G_SCHMAST.NENGETUDO_S")
            SQL.Append(",G_SCHMAST.SCH_KBN_S")
            SQL.Append(",G_SCHMAST.FURI_KBN_S")
            SQL.Append(",G_SCHMAST.HENKAN_YDATE_S")
            SQL.Append(",GAKMAST2.TAKO_KBN_T")
            SQL.Append(",SCHMAST.HAISIN_YDATE_S")
            SQL.Append(" FROM KZFMAST.G_SCHMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.GAKMAST2")
            SQL.Append(",KZFMAST.SCHMAST")
            SQL.Append(" WHERE G_SCHMAST.GAKKOU_CODE_S=SCHMAST.TORIS_CODE_S")
            SQL.Append(" AND ((G_SCHMAST.FURI_KBN_S = '0' AND SCHMAST.TORIF_CODE_S = '01')")
            SQL.Append(" OR (G_SCHMAST.FURI_KBN_S = '1' AND SCHMAST.TORIF_CODE_S = '02')")
            SQL.Append(" OR (G_SCHMAST.FURI_KBN_S = '2' AND SCHMAST.TORIF_CODE_S = '03')")
            SQL.Append(" OR (G_SCHMAST.FURI_KBN_S = '3' AND SCHMAST.TORIF_CODE_S = '04'))")
            SQL.Append(" AND G_SCHMAST.FURI_DATE_S=SCHMAST.FURI_DATE_S")
            SQL.Append(" AND G_SCHMAST.GAKKOU_CODE_S=GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND G_SCHMAST.GAKKOU_CODE_S=GAKMAST2.GAKKOU_CODE_T")
            SQL.Append(" AND GAKMAST1.GAKUNEN_CODE_G = 1")
            SQL.Append(" AND NENGETUDO_S = " & SQ(Nentuki))
            SQL.Append(" AND GAKMAST2.KAISI_DATE_T <= " & SQ(Nentuki & "01"))
            SQL.Append(" AND GAKMAST2.SYURYOU_DATE_T >= " & SQ(Nentuki & "31"))
            Select Case PrintSort
                Case "1"
                    SQL.Append(" ORDER BY G_SCHMAST.FURI_DATE_S, GAKMAST1.GAKKOU_CODE_G, G_SCHMAST.FURI_KBN_S")
                Case "2"
                    SQL.Append(" ORDER BY GAKMAST1.GAKKOU_CODE_G, G_SCHMAST.FURI_DATE_S, G_SCHMAST.FURI_KBN_S")
            End Select


            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False

                    For No As Integer = 1 To 31
                        If oraReader.GetString("FURI_DATE_S") = Nentuki & No.ToString("00") Then
                            Yotei(No) = "振"
                        ElseIf oraReader.GetString("HAISIN_YDATE_S") = Nentuki & No.ToString("00") Then
                            Yotei(No) = "配"
                        ElseIf oraReader.GetString("HENKAN_YDATE_S") = Nentuki & No.ToString("00") Then
                            Yotei(No) = "返"
                        ElseIf Youbi(No) = Yasumi Then
                            Yotei(No) = Yasumi
                        Else
                            Yotei(No) = ""
                        End If
                    Next

                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENGETUDO_S")))           '対象年月
                    For No As Integer = 1 To 31                                             '曜日1～31
                        OutputCsvData(GCOM.NzStr(Youbi(No)))
                    Next
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))         '学校コード

                    If oraReader.GetString("ENTRI_FLG_S") = "1" Then                        '入金
                        OutputCsvData("■")
                    Else
                        OutputCsvData("□")
                    End If

                    If oraReader.GetString("CHECK_FLG_S") = "1" Then                        'チェック
                        OutputCsvData("■")
                    Else
                        OutputCsvData("□")
                    End If

                    If oraReader.GetString("DATA_FLG_S") = "1" Then                         '作成
                        OutputCsvData("■")
                    Else
                        OutputCsvData("□")
                    End If

                    If oraReader.GetString("FUNOU_FLG_S") = "1" Then                        '不能
                        OutputCsvData("■")
                    Else
                        OutputCsvData("□")
                    End If

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_T")))          '委託者コード
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("GCOMMON", "TXT"), "GFJ_媒体.TXT"), _
                                                                oraReader.GetString("BAITAI_CODE_T")))
                    'Select Case oraReader.GetString("BAITAI_CODE_T")                        '媒体
                    '    Case "0"
                    '        OutputCsvData("伝送")
                    '    Case "1"
                    '        OutputCsvData("FD")
                    '    Case "2"
                    '        OutputCsvData("紙")
                    '    Case Else
                    '        OutputCsvData("")
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                    Select Case oraReader.GetString("SCH_KBN_S")                        'スケジュール区分
                        Case "0"
                            OutputCsvData("通")
                        Case "1"
                            OutputCsvData("特")
                        Case "2"
                            OutputCsvData("随")
                        Case Else
                            OutputCsvData("")
                    End Select
                    Select Case oraReader.GetString("FURI_KBN_S")                        '振替区分
                        Case "0"
                            OutputCsvData("初")
                        Case "1"
                            OutputCsvData("再")
                        Case "2"
                            OutputCsvData("入")
                        Case "3"
                            OutputCsvData("出")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")))          '振替日
                    For No As Integer = 1 To 31                                         '1～31日
                        OutputCsvData(GCOM.NzStr(Yotei(No)))
                    Next
                    OutputCsvData("", False, True)             'ダミー

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
