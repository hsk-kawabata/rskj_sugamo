Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP016

    Inherits CAstReports.ClsReportBase
    Sub New(ByVal PrintKbn As String)
        Select Case PrintKbn
            Case "0"
                ' CSVファイルセット
                InfoReport.ReportName = "KFGP016"
                ' 定義体名セット
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                If STR_HIMOKU_PTN = "1" Then
                    ReportBaseName = "KFGP016_口座振替一覧表(振替結果)(費目15).rpd"
                Else
                    ReportBaseName = "KFGP016_口座振替一覧表(振替結果).rpd"
                End If
                'ReportBaseName = "KFGP016_口座振替一覧表(振替結果).rpd"
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END

                ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-06(RSV2対応) -------------------- START
                PRD_ID = "KFGP016"
                ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-06(RSV2対応) -------------------- END

            Case Else
                ' CSVファイルセット
                InfoReport.ReportName = "KFGP017"
                ' 定義体名セット
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                If STR_HIMOKU_PTN = "1" Then
                    ReportBaseName = "KFGP017_口座振替一覧表(振替結果不能結果)(費目15).rpd"
                Else
                    ReportBaseName = "KFGP017_口座振替一覧表(振替結果不能結果).rpd"
                End If
                'ReportBaseName = "KFGP017_口座振替一覧表(振替結果不能結果).rpd"
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END

                ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-06(RSV2対応) -------------------- START
                PRD_ID = "KFGP017"
                ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-06(RSV2対応) -------------------- END

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
        '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
        Dim HimoReader As CASTCommon.MyOracleReader = Nothing
        Dim Gakunen As Integer = 0
        '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
        Dim SQL As New StringBuilder(128)
        Dim FuriKin As Long = 0
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
            SQL.Append(",G_MEIMAST.NENDO_M")
            SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
            SQL.Append(",G_MEIMAST.CLASS_CODE_M")
            SQL.Append(",G_MEIMAST.SEITO_NO_M")
            SQL.Append(",G_MEIMAST.TUUBAN_M")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M")
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
            SQL.Append(",G_MEIMAST.FURI_DATE_M")
            SQL.Append(",G_MEIMAST.FURIKETU_CODE_M")
            SQL.Append(",GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G")
            For No As Integer = 1 To 20
                SQL.Append(",GAKMAST1.CLASS_CODE1" & No.ToString("00") & "_G")
                SQL.Append(",GAKMAST1.CLASS_NAME1" & No.ToString("00") & "_G")
            Next
            SQL.Append(",HIMOMAST.HIMOKU_NAME01_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME02_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME03_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME04_H")
            SQL.Append(",HIMOMAST.HIMOKU_NAME05_H")
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
            SQL.Append(",SEITOMASTVIEW.GAKUNEN_CODE_O")
            SQL.Append(",SEITOMASTVIEW.SEITO_KNAME_O")
            SQL.Append(",SEITOMASTVIEW.SEITO_NNAME_O")

            '2010/10/13.Sakon　明細マスタから取得 ++++++++++
            SQL.Append(",G_MEIMAST.TKAMOKU_M")
            SQL.Append(",G_MEIMAST.TKOUZA_M")
            SQL.Append(",G_MEIMAST.TMEIGI_KNM_M")
            'SQL.Append(",SEITOMASTVIEW.KAMOKU_O")
            'SQL.Append(",SEITOMASTVIEW.KOUZA_O")
            'SQL.Append(",SEITOMASTVIEW.MEIGI_KNAME_O")
            '+++++++++++++++++++++++++++++++++++++++++++++++
            SQL.Append(",SEITOMASTVIEW.TYOUSI_FLG_O")

            '2010/09/10.Sakon　取りまとめ店を追加 +++++
            SQL.Append(",TENMAST_T.SIT_NO_N AS GAK_SIT_CODE")
            SQL.Append(",TENMAST_T.SIT_NNAME_N AS GAK_SIT_NAME")
            SQL.Append(",TENMAST_S.KIN_NO_N AS SEITO_KIN_CODE")
            SQL.Append(",TENMAST_S.SIT_NO_N AS SEITO_SIT_CODE")
            SQL.Append(",TENMAST_S.SIT_NNAME_N AS SEITO_SIT_NAME")
            'SQL.Append(",TENMAST.KIN_NO_N ")
            'SQL.Append(",TENMAST.SIT_NO_N ")
            'SQL.Append(",TENMAST.SIT_NNAME_N ")
            '++++++++++++++++++++++++++++++++++++++++++

            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
            SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
            '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
            SQL.Append(",SEITOMASTVIEW.HIMOKU_ID_O")
            '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQL.Append(" FROM ")
            SQL.Append("  KZFMAST.G_MEIMAST")
            SQL.Append(" ,KZFMAST.GAKMAST1")
            SQL.Append(" ,KZFMAST.HIMOMAST")
            SQL.Append(" ,KZFMAST.SEITOMASTVIEW")
            SQL.Append(" ,KZFMAST.GAKMAST2")            '追加

            '2010/09/10.Sakon　取りまとめ店を追加 +++++
            SQL.Append(" ,KZFMAST.TENMAST TENMAST_T")
            SQL.Append(" ,KZFMAST.TENMAST TENMAST_S")
            'SQL.Append(" ,KZFMAST.TENMAST")
            '++++++++++++++++++++++++++++++++++++++++++

            SQL.Append(" WHERE ")
            SQL.Append(" G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)")
            SQL.Append(" AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)")
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)")
            SQL.Append(" AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)")
            SQL.Append(" AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)")
            SQL.Append(" AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)")
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)")
            SQL.Append(" AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)")
            SQL.Append(" AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)")
            SQL.Append(" AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)")

            '2010/09/10.Sakon　取りまとめ店を追加 +++++++++++++++++++++++++++++++++++
            SQL.Append(" AND GAKMAST1.GAKKOU_CODE_G   = GAKMAST2.GAKKOU_CODE_T(+)")
            SQL.Append(" AND GAKMAST2.TKIN_NO_T       = TENMAST_T.KIN_NO_N(+)")
            SQL.Append(" AND GAKMAST2.TSIT_NO_T       = TENMAST_T.SIT_NO_N(+)")
            SQL.Append(" AND G_MEIMAST.TKIN_NO_M      = TENMAST_S.KIN_NO_N(+)")
            SQL.Append(" AND G_MEIMAST.TSIT_NO_M      = TENMAST_S.SIT_NO_N(+)")
            'SQL.Append(" AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+)")
            'SQL.Append(" AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+)")
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M  =" & SQ(GakkouCode))
            '請求金額が0円のデータは出力しない
            SQL.Append(" AND G_MEIMAST.SEIKYU_KIN_M  > 0 ")

            If Gassan = "1" AndAlso Syofuri <> "" AndAlso PrintKbn = "0" _
                AndAlso FuriKbn <> "2" AndAlso FuriKbn <> "3" Then
                '再振日の入力で合算出力の場合かつ振替区分が初振・再振かつ、振替結果の印刷
                '入力した再振日の明細は全て対象だが
                '取得した初振日の明細は振替済のものが対象
                SQL.Append(" AND ((G_MEIMAST.FURI_DATE_M <= " & SQ(FuriDate))
                SQL.Append(" AND G_MEIMAST.SEIKYU_TAISYOU_M = " & SQ(TaisyoNengetu))
                SQL.Append(" AND G_MEIMAST.FURI_KBN_M = '1')")
                SQL.Append(" OR (G_MEIMAST.FURI_DATE_M = " & SQ(Syofuri))
                SQL.Append(" AND G_MEIMAST.FURIKETU_CODE_M = 0)")
                SQL.Append(" OR (G_MEIMAST.FURI_DATE_M = " & SQ(Syofuri))
                SQL.Append(" AND G_MEIMAST.FURIKETU_CODE_M <> 0")
                SQL.Append(" AND G_MEIMAST.SAIFURI_SUMI_M = 0))")
            ElseIf Gassan = "1" AndAlso Syofuri <> "" Then
                'そのほかの合算
                SQL.Append(" AND (G_MEIMAST.FURI_DATE_M = " & SQ(FuriDate))
                SQL.Append("  OR  G_MEIMAST.FURI_DATE_M = " & SQ(Syofuri) & ")")
            Else
                '初振日または再振日の入力で合算出力なしの場合
                SQL.Append(" AND G_MEIMAST.FURI_DATE_M =" & SQ(FuriDate))
            End If

            Select Case FuriKbn
                Case "2", "3"   '入金・出金
                    SQL.Append(" AND G_MEIMAST.FURI_KBN_M = " & SQ(FuriKbn))
                Case Else
                    SQL.Append(" AND (G_MEIMAST.FURI_KBN_M = '0' OR G_MEIMAST.FURI_KBN_M = '1') ")
            End Select


            SQL.Append(" ORDER BY ")
            Select Case PrintSort
                Case "0"
                    SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
                    SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
                    SQL.Append(",G_MEIMAST.CLASS_CODE_M")
                    SQL.Append(",G_MEIMAST.SEITO_NO_M")
                    SQL.Append(",G_MEIMAST.NENDO_M")
                    SQL.Append(",G_MEIMAST.TUUBAN_M")
                    SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M") '2010/10/13.Sakon　追加
                Case "1"
                    SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
                    SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
                    SQL.Append(",G_MEIMAST.NENDO_M")
                    SQL.Append(",G_MEIMAST.TUUBAN_M")
                    SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M") '2010/10/13.Sakon　追加
                Case Else
                    SQL.Append(" G_MEIMAST.GAKKOU_CODE_M")
                    SQL.Append(",G_MEIMAST.GAKUNEN_CODE_M")
                    SQL.Append(",SEITOMASTVIEW.SEITO_KNAME_O")
                    SQL.Append(",G_MEIMAST.NENDO_M")
                    SQL.Append(",G_MEIMAST.TUUBAN_M")
                    SQL.Append(",G_MEIMAST.SEIKYU_TUKI_M") '2010/10/13.Sakon　追加

            End Select

            If oraReader.DataReader(SQL) = True Then
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                User_ID = oraReader.GetString("YOBI1_T")
                BaitaiCode = oraReader.GetString("BAITAI_CODE_T")
                ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                While oraReader.EOF = False
                    FuriKin = 0
                    'GCOM.NzStrはNULL値対策
                    '2013/12/24 saitou 標準版 修正 DEL -------------------------------------------------->>>>
                    '印刷区分で帳票定義が分かれているため削除
                    'Select Case PrintKbn                                                    'タイトル
                    '    Case "0"
                    '        OutputCsvData("< 口座振替結果一覧表 >")
                    '    Case Else
                    '        OutputCsvData("< 口座振替不能明細一覧表 >")
                    'End Select
                    '2013/12/24 saitou 標準版 修正 DEL --------------------------------------------------<<<<
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_M")))           '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_M")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))        '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_M")))          'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_M")))            '生徒番号
                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")).Trim = "" Then      '生徒名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True)
                    End If

                    '2010/09/10.Sakon　取りまとめ店考慮 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KIN_CODE")))        '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_SIT_CODE")))        '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_SIT_NAME")))        '支店名
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NO_N")))              '金融機関コード
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NO_N")))              '支店コード
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))           '支店名
                    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    '2010/10/13.Sakon　明細マスタから取得 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    Select Case oraReader.GetString("TKAMOKU_M")                            '科目 
                        Case "02"
                            OutputCsvData("普")
                        Case "01"
                            OutputCsvData("当")
                        Case "09"
                            OutputCsvData("他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKOUZA_M")))              '振替口座番号
                    'Select Case oraReader.GetString("KAMOKU_O")                             '科目
                    '    Case "02"
                    '        OutputCsvData("普")
                    '    Case "01"
                    '        OutputCsvData("当")
                    '    Case "09"
                    '        OutputCsvData("他")
                    '    Case Else
                    '        OutputCsvData("")
                    'End Select
                    'OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_O")))               '振替口座番号
                    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    Select Case FuriKbn
                        Case "2", "3"   '入金・出金
                            OutputCsvData(GCOM.NzLong(oraReader.GetString("SEIKYU_KIN_M"))) '振替金額
                        Case Else
                            For No As Integer = 1 To 15                                     '費目金額1～15
                                FuriKin += oraReader.GetInt64("HIMOKU" & No.ToString & "_KIN_M")
                            Next
                            OutputCsvData(GCOM.NzStr(FuriKin))                              '振替金額
                    End Select


                    For No As Integer = 1 To 15                                             '費目名1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H")))
                    Next
                    For No As Integer = 1 To 15                                             '費目金額1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU" & No.ToString & "_KIN_M")))
                    Next
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TMEIGI_KNM_M")), True)         '名義人カナ名
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_TUKI_M")).Substring(4, 2))
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
                    If GCOM.NzStr(oraReader.GetString("TYOUSI_FLG_O")) = "1" Then           '長子
                        OutputCsvData("※")
                    Else
                        OutputCsvData("")
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_M")))               '年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_M")))              '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetInt("FURIKETU_CODE_M")))          '振替結果コード
                    Select Case oraReader.GetInt("FURIKETU_CODE_M")                         '振替結果
                        Case 0
                            OutputCsvData("")
                        Case 1
                            OutputCsvData("資金不足")
                        Case 2
                            OutputCsvData("取引なし")
                        Case 3
                            OutputCsvData("預金者都合")
                        Case 4
                            OutputCsvData("依頼書なし")
                        Case 8
                            OutputCsvData("委託者都合")
                        Case 9
                            OutputCsvData("その他")
                        Case Else
                            OutputCsvData("不明：" & oraReader.GetInt("FURIKETU_CODE_M"))
                    End Select
                    For No As Integer = 1 To 20
                        If GCOM.NzStr(oraReader.GetInt("CLASS_CODE_M")) = GCOM.NzStr(oraReader.GetInt("CLASS_CODE1" & No.ToString("00") & "_G")) Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_NAME1" & No.ToString("00") & "_G")))    'クラス名
                            Exit For
                        End If
                    Next

                    '2010/09/10.Sakon　取りまとめ店を追加 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAK_SIT_CODE")))          '取りまとめ店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAK_SIT_NAME")))          '取りまとめ店名
                    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))       '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")))      '企業コード
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
                    '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
                    '費目ID
                    OutputCsvData(oraReader.GetString("HIMOKU_ID_O"))
                    '金庫名
                    OutputCsvData(JIKINKO)
                    '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
                    '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- START
                    '長子表示制御
                    If STR_TYOUSI_KBN = "0" Then
                        OutputCsvData("0")
                    Else
                        OutputCsvData("1")
                    End If
                    '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- END

                    OutputCsvData("", False, True)                                          '改行用ダミー
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
