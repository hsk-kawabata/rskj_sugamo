Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Public Class KFGP003

    Inherits CAstReports.ClsReportBase
    Sub New(ByVal SortKey As String)
        Select Case SortKey
            Case "1"
                ' CSVファイルセット
                InfoReport.ReportName = "KFGP003"

                '2017/03/02 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                If STR_HIMOKU_PTN = "1" Then
                    ' 定義体名セット
                    ReportBaseName = "KFGP003_口座振替予定明細表(費目15).rpd"
                Else
                    ' 定義体名セット
                    ReportBaseName = "KFGP003_口座振替予定明細表.rpd"
                End If
                '' 定義体名セット
                'ReportBaseName = "KFGP003_口座振替予定明細表.rpd"
                '2017/03/02 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
            Case Else
                ' CSVファイルセット
                InfoReport.ReportName = "KFGP004"
                '2017/03/02 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                If STR_HIMOKU_PTN = "1" Then
                    ' 定義体名セット
                    ReportBaseName = "KFGP004_口座振替予定明細表(店番ソート)(費目15).rpd"
                Else
                    ' 定義体名セット
                    ReportBaseName = "KFGP004_口座振替予定明細表(店番ソート).rpd"
                End If
                '' 定義体名セット
                'ReportBaseName = "KFGP004_口座振替予定明細表(店番ソート).rpd"
                '2017/03/02 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
        End Select

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
        Dim HimokuName(14) As String

        '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
        '金庫名取得
        Dim strKINKONAME As String = ""
        Dim wk As String = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
        If wk.ToUpper = "ERR" OrElse wk = "" Then
            strKINKONAME = ""
        Else
            strKINKONAME = wk.Trim
        End If
        '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END

        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append(" SELECT TENMAST.SIT_NO_N")
            SQL.Append(",GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G")
            SQL.Append(",HIMOMAST.HIMOKU_NAME01_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME02_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME04_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME06_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME07_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME08_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME09_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME10_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME11_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME12_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME13_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME14_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME15_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME03_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME05_H")
            SQL.Append(",TENMAST.KIN_NO_N")
            SQL.Append(",G_SCHMAST.FURI_DATE_S")
            SQL.Append(",SEITOMASTVIEW.GAKUNEN_CODE_O")
            SQL.Append(",SEITOMASTVIEW.SEITO_NO_O")
            SQL.Append(",SEITOMASTVIEW.SEITO_KNAME_O")
            SQL.Append(",SEITOMASTVIEW.SEITO_NNAME_O")
            SQL.Append(",SEITOMASTVIEW.MEIGI_KNAME_O")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(",SEITOMASTVIEW.TYOUSI_FLG_O")
            SQL.Append(",G_SCHMAST.FUNOU_FLG_S")
            SQL.Append(",G_MEIMAST.TKAMOKU_M")
            SQL.Append(",G_MEIMAST.GAKKOU_CODE_M ")
            SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
            SQL.Append(",G_MEIMAST.HIMOKU1_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU2_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU3_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU4_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU5_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU6_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU7_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU8_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU9_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU10_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU11_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU12_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU13_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU14_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU15_KIN_M")
            SQL.Append(",G_MEIMAST.TUUBAN_M")
            SQL.Append(",G_MEIMAST.NENDO_M")
            SQL.Append(",G_MEIMAST.CLASS_CODE_M")
            SQL.Append(",G_MEIMAST.SEITO_NO_M")
            SQL.Append(",G_MEIMAST.TKOUZA_M")
            SQL.Append(",G_MEIMAST.FURI_KBN_M")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
            SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M")
            SQL.Append(",GAKMAST2.TORIMATOME_SIT_T")
            SQL.Append(",TENMAST_2.SIT_NNAME_N AS TORIMATOME_SIT_NNAME")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
            '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
            SQL.Append(",SEITOMASTVIEW.HIMOKU_ID_O")
            '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END
            SQL.Append(" FROM KZFMAST.G_MEIMAST G_MEIMAST ")
            SQL.Append(",KZFMAST.SEITOMASTVIEW SEITOMASTVIEW ")
            SQL.Append(",KZFMAST.G_SCHMAST G_SCHMAST ")
            SQL.Append(",KZFMAST.HIMOMAST HIMOMAST ")
            SQL.Append(",KZFMAST.GAKMAST1 GAKMAST1 ")
            SQL.Append(",KZFMAST.TENMAST TENMAST ")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",KZFMAST.GAKMAST2 GAKMAST2")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
            SQL.Append(",KZFMAST.TENMAST TENMAST_2")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
            SQL.Append(" WHERE ")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M=SEITOMASTVIEW.GAKKOU_CODE_O")
            SQL.Append(" AND G_MEIMAST.NENDO_M=SEITOMASTVIEW.NENDO_O")
            '2011/06/16 標準版修正 生徒番号変更対応------------------START
            'SQL.Append(" AND G_MEIMAST.GAKUNEN_CODE_M=SEITOMASTVIEW.GAKUNEN_CODE_O")
            'SQL.Append(" AND G_MEIMAST.CLASS_CODE_M=SEITOMASTVIEW.CLASS_CODE_O")
            'SQL.Append(" AND G_MEIMAST.SEITO_NO_M=SEITOMASTVIEW.SEITO_NO_O")
            '2011/06/16 標準版修正 生徒番号変更対応応------------------END
            SQL.Append(" AND G_MEIMAST.TUUBAN_M=SEITOMASTVIEW.TUUBAN_O")
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M=G_SCHMAST.GAKKOU_CODE_S")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S")
            SQL.Append(" AND G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S")

            SQL.Append(" AND  SEITOMASTVIEW.GAKKOU_CODE_O=HIMOMAST.GAKKOU_CODE_H (+)")
            SQL.Append(" AND  SEITOMASTVIEW.GAKUNEN_CODE_O=HIMOMAST.GAKUNEN_CODE_H (+)")
            SQL.Append(" AND  SEITOMASTVIEW.HIMOKU_ID_O=HIMOMAST.HIMOKU_ID_H (+)")
            SQL.Append(" AND  SEITOMASTVIEW.TUKI_NO_O=HIMOMAST.TUKI_NO_H (+)")
            SQL.Append(" AND  SEITOMASTVIEW.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G (+)")
            SQL.Append(" AND  SEITOMASTVIEW.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G (+)")
            SQL.Append(" AND  SEITOMASTVIEW.TKIN_NO_O=TENMAST.KIN_NO_N (+)")
            SQL.Append(" AND  SEITOMASTVIEW.TSIT_NO_O=TENMAST.SIT_NO_N (+)")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(" AND SEITOMASTVIEW.GAKKOU_CODE_O = GAKMAST2.GAKKOU_CODE_T (+)")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
            SQL.Append(" AND  GAKMAST2.TKIN_NO_T = TENMAST_2.KIN_NO_N (+)")
            SQL.Append(" AND  GAKMAST2.TORIMATOME_SIT_T = TENMAST_2.SIT_NO_N (+)")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M = " & SQ(GakkouCode))
            SQL.Append(" AND G_MEIMAST.YOBI1_M = " & SQ(SyoriDate))
            SQL.Append(" AND SEITOMASTVIEW.TUKI_NO_O = " & SQ(Mid(FuriDate, 5, 2)))

            SQL.Append(" AND ")
            SQL.Append(" ((G_MEIMAST.GAKUNEN_CODE_M=1 AND G_SCHMAST.GAKUNEN1_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=2 AND G_SCHMAST.GAKUNEN2_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=3 AND G_SCHMAST.GAKUNEN3_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=4 AND G_SCHMAST.GAKUNEN4_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=5 AND G_SCHMAST.GAKUNEN5_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=6 AND G_SCHMAST.GAKUNEN6_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=7 AND G_SCHMAST.GAKUNEN7_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=8 AND G_SCHMAST.GAKUNEN8_FLG_S='1') OR ")
            SQL.Append(" (G_MEIMAST.GAKUNEN_CODE_M=9 AND G_SCHMAST.GAKUNEN9_FLG_S='1')) ")
            SQL.Append(" ORDER BY ")

            If SortKey = "2" Then   '店番ソート
                SQL.Append(" G_MEIMAST.TKIN_NO_M , G_MEIMAST.TSIT_NO_M , ")
            End If

            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '画面でソート順を指定しても反映されないのを修正
            Select Case CInt(SortKbn)
                'Select Case CInt(SortKey)
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                Case 0
                    '学年,クラス,生徒番号
                    SQL.Append(" G_MEIMAST.GAKUNEN_CODE_M ASC ")
                    SQL.Append(",G_MEIMAST.CLASS_CODE_M ASC ")
                    SQL.Append(",G_MEIMAST.SEITO_NO_M ASC ")
                    SQL.Append(",G_MEIMAST.NENDO_M ASC ")
                    SQL.Append(",G_MEIMAST.TUUBAN_M ASC ")
                    SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M ASC ") '2010/10/13.Sakon　追加
                Case 1
                    '入学年度,通番
                    SQL.Append(" G_MEIMAST.GAKUNEN_CODE_M ASC ")
                    SQL.Append(",G_MEIMAST.CLASS_CODE_M ASC ")
                    SQL.Append(",G_MEIMAST.NENDO_M ASC ")
                    SQL.Append(",G_MEIMAST.TUUBAN_M ASC ")
                    SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M ASC ") '2010/10/13.Sakon　追加
                Case 2
                    'あいうえお(生徒名(ｶﾅ))
                    SQL.Append(" G_MEIMAST.GAKUNEN_CODE_M ASC ")
                    SQL.Append(",G_MEIMAST.CLASS_CODE_M ASC ")
                    SQL.Append(",SEITOMASTVIEW.SEITO_KNAME_O ASC ")
                    SQL.Append(",G_MEIMAST.NENDO_M ASC ")
                    SQL.Append(",G_MEIMAST.TUUBAN_M ASC ")
                    SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M ASC ") '2010/10/13.Sakon　追加
            End Select

            Dim FuriKin As Long
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    FuriKin = 0

                    OutputCsvData(Mid(SyoriDate, 1, 8))                                             '処理日
                    OutputCsvData(Mid(SyoriDate, 9))                                                'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))                '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))                 '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")))                   '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_O")))                '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))                '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_M")))                  'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_M")))                    '生徒番号
                    Select Case GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")).Trim               '生徒名
                        Case ""
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)
                        Case Else
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")))
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NO_N")))                      '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NO_N")))                      '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))                   '支店名
                    Select Case oraReader.GetString("TKAMOKU_M")                                    '科目
                        Case "02"
                            OutputCsvData("普")
                        Case "01"
                            OutputCsvData("当")
                        Case "09"
                            OutputCsvData("他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKOUZA_M")))                      '振替口座番号
                    Select Case GCOM.NzStr(oraReader.GetString("FURI_KBN_M"))
                        Case "0", "1"   '初振・再振
                            For No As Integer = 1 To 15                                             '費目金額1～15
                                FuriKin += GCOM.NzLong(oraReader.GetString("HIMOKU" & No.ToString & "_KIN_M"))
                            Next
                            OutputCsvData(FuriKin.ToString)                                         '振替金額
                        Case Else   '入金・出金
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_KIN_M")))          '振替金額
                    End Select

                    For No As Integer = 1 To 15                                                     '費目名1～15
                        Select Case GCOM.NzStr(oraReader.GetString("FURI_KBN_M"))
                            Case "0", "1"   '初振・再振
                                HimokuName(No - 1) = GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H"))
                            Case Else   '入金・出金
                                HimokuName(No - 1) = ""
                        End Select
                        OutputCsvData(HimokuName(No - 1))
                    Next
                    For No As Integer = 1 To 15                                                     '費目金額1～15
                        If HimokuName(No - 1) = "" Then
                            OutputCsvData("")
                        Else
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU" & No.ToString & "_KIN_M")))
                        End If
                    Next
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_O")), True)                 '名義人カナ名
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_TUKI_M")).Substring(4, 2))     '請求月
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
                    If GCOM.NzStr(oraReader.GetString("TYOUSI_FLG_O")) = "1" Then                   '長子
                        OutputCsvData("※")
                    Else
                        OutputCsvData("")
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_M")))                       '年度
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    ' 振替コード、企業コードを追加（【通番】は最終項目でなくなったため改行を削除）
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_M")), False, True)         '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_M")))                      '通番


                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))                   '振替コード
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), False, True)     '企業コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")))     '企業コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIMATOME_SIT_T")))      'とりまとめ店
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIMATOME_SIT_NNAME")))  'とりまとめ店名
                    '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_O")))           '費目ＩＤ
                    OutputCsvData(GCOM.NzStr(strKINKONAME))                                 '金庫名
                    '2016/10/18 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END
                    '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- START
                    '長子表示制御
                    If STR_TYOUSI_KBN = "0" Then
                        OutputCsvData("0")
                    Else
                        OutputCsvData("1")
                    End If
                    '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- END
                    OutputCsvData("", False, True)      '改行コード
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

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
