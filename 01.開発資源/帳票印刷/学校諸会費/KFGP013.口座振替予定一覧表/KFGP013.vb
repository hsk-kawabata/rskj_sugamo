Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP013

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP013"

        ' 定義体名セット
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        If STR_HIMOKU_PTN = "1" Then
            ReportBaseName = "KFGP013_口座振替予定一覧表(費目15).rpd"
        Else
            ReportBaseName = "KFGP013_口座振替予定一覧表.rpd"
        End If
        'ReportBaseName = "KFGP013_口座振替予定一覧表.rpd"
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
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
            SQL.Append(" GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G")
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
            SQL.Append(",SEITOMASTVIEW.SEITO_KNAME_O")
            SQL.Append(",SEITOMASTVIEW.SEITO_NNAME_O")
            SQL.Append(",SEITOMASTVIEW.MEIGI_KNAME_O")
            SQL.Append(",SEITOMASTVIEW.NENDO_O")
            SQL.Append(",SEITOMASTVIEW.TUUBAN_O")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(",SEITOMASTVIEW.TYOUSI_FLG_O")
            SQL.Append(",G_PRTWORK2.GAKKOU_CODE_P")
            SQL.Append(",G_PRTWORK2.KINGAKU01_P")
            SQL.Append(",G_PRTWORK2.KINGAKU02_P")
            SQL.Append(",G_PRTWORK2.KINGAKU03_P")
            SQL.Append(",G_PRTWORK2.KINGAKU04_P")
            SQL.Append(",G_PRTWORK2.KINGAKU05_P")
            SQL.Append(",G_PRTWORK2.KINGAKU06_P")
            SQL.Append(",G_PRTWORK2.KINGAKU07_P")
            SQL.Append(",G_PRTWORK2.KINGAKU08_P")
            SQL.Append(",G_PRTWORK2.KINGAKU09_P")
            SQL.Append(",G_PRTWORK2.KINGAKU10_P")
            SQL.Append(",G_PRTWORK2.KINGAKU11_P")
            SQL.Append(",G_PRTWORK2.KINGAKU12_P")
            SQL.Append(",G_PRTWORK2.KINGAKU13_P")
            SQL.Append(",G_PRTWORK2.KINGAKU14_P")
            SQL.Append(",G_PRTWORK2.KINGAKU15_P")
            SQL.Append(",G_PRTWORK2.FURI_DATE_P")
            SQL.Append(",G_PRTWORK2.GAKUNEN_CODE_P")
            SQL.Append(",G_PRTWORK2.CLASS_CODE_P")
            SQL.Append(",G_PRTWORK2.SEITO_NO_P")
            SQL.Append(",G_PRTWORK2.TKIN_NO_P")
            SQL.Append(",G_PRTWORK2.TSIT_NO_P")
            SQL.Append(",G_PRTWORK2.KAMOKU_P")
            SQL.Append(",G_PRTWORK2.KOUZA_P")
            SQL.Append(",G_PRTWORK2.KINGAKU_P")
            For No As Integer = 1 To 20
                SQL.Append(",GAKMAST1.CLASS_CODE1" & No.ToString("00") & "_G")
                SQL.Append(",GAKMAST1.CLASS_NAME1" & No.ToString("00") & "_G")
            Next
           '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
                       SQL.Append(",G_PRTWORK2.SEIKYU_TUKI_P")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.TORIMATOME_SIT_T")
            SQL.Append(",TENMAST_2.SIT_NNAME_N AS TORIMATOME_SIT_NAME")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
            '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
            '帳票出力内容に費目IDを追加
            SQL.Append(",G_PRTWORK2.HIMOKU_ID_P")
            '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
            '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
            '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.G_PRTWORK2")
            SQL.Append(",KZFMAST.SEITOMASTVIEW")
            SQL.Append(",KZFMAST.TENMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.HIMOMAST")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
            SQL.Append(",KZFMAST.TENMAST TENMAST_2")
            SQL.Append(",KZFMAST.GAKMAST2")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
            SQL.Append(" WHERE G_PRTWORK2.SEIKYU_TUKI_P=SEITOMASTVIEW.TUKI_NO_O (+)")
            SQL.Append(" AND G_PRTWORK2.GAKKOU_CODE_P=SEITOMASTVIEW.GAKKOU_CODE_O (+)")
            SQL.Append(" AND G_PRTWORK2.NENDO_P=SEITOMASTVIEW.NENDO_O (+)")
            SQL.Append(" AND G_PRTWORK2.TUUBAN_P=SEITOMASTVIEW.TUUBAN_O (+)")
            SQL.Append(" AND G_PRTWORK2.TKIN_NO_P=TENMAST.KIN_NO_N (+)")
            SQL.Append(" AND G_PRTWORK2.TSIT_NO_P=TENMAST.SIT_NO_N (+)")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
            SQL.Append(" AND  G_PRTWORK2.GAKKOU_CODE_P = GAKMAST2.GAKKOU_CODE_T")
            SQL.Append(" AND  GAKMAST2.TKIN_NO_T = TENMAST_2.KIN_NO_N (+)")
            SQL.Append(" AND  GAKMAST2.TORIMATOME_SIT_T = TENMAST_2.SIT_NO_N (+)")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END
            SQL.Append(" AND G_PRTWORK2.GAKKOU_CODE_P=GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND G_PRTWORK2.GAKUNEN_CODE_P=GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(" AND G_PRTWORK2.GAKKOU_CODE_P=HIMOMAST.GAKKOU_CODE_H")
            SQL.Append(" AND G_PRTWORK2.GAKUNEN_CODE_P=HIMOMAST.GAKUNEN_CODE_H")
            SQL.Append(" AND G_PRTWORK2.HIMOKU_ID_P=HIMOMAST.HIMOKU_ID_H")
            SQL.Append(" AND G_PRTWORK2.SEIKYU_TUKI_P=HIMOMAST.TUKI_NO_H")
            SQL.Append(" AND G_PRTWORK2.GAKKOU_CODE_P= " & SQ(GakkouCode))
            SQL.Append(" ORDER BY ")
            Select Case PrintSort
                Case "0"    '学年・クラス・生徒番号・年度・通番
                    SQL.Append(" G_PRTWORK2.GAKKOU_CODE_P")
                    SQL.Append(",G_PRTWORK2.GAKUNEN_CODE_P")
                    SQL.Append(",G_PRTWORK2.CLASS_CODE_P")
                    SQL.Append(",G_PRTWORK2.SEITO_NO_P")
                    SQL.Append(",G_PRTWORK2.NENDO_P")
                    SQL.Append(",G_PRTWORK2.TUUBAN_P")
                Case "1"    '年度・通番
                    SQL.Append(" G_PRTWORK2.GAKKOU_CODE_P")
                    SQL.Append(",G_PRTWORK2.GAKUNEN_CODE_P")
                    SQL.Append(",G_PRTWORK2.NENDO_P")
                    SQL.Append(",G_PRTWORK2.TUUBAN_P")
                Case Else   'あいうえお(生徒名(ｶﾅ))・年度・通番
                    SQL.Append(" G_PRTWORK2.GAKKOU_CODE_P")
                    SQL.Append(",G_PRTWORK2.GAKUNEN_CODE_P")
                    SQL.Append(",G_PRTWORK2.SEITO_KNAME_P")
                    SQL.Append(",G_PRTWORK2.NENDO_P")
                    SQL.Append(",G_PRTWORK2.TUUBAN_P")
            End Select
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
            SQL.Append(",G_PRTWORK2.SEIKYU_TUKI_P")
            '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END

            If oraReader.DataReader(SQL) = True Then
                '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                User_ID = oraReader.GetString("YOBI1_T")
                BaitaiCode = oraReader.GetString("BAITAI_CODE_T")
                ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                '2017/04/25 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_G")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_P")))           '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_P")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))        '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_P")))          'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_P")))            '生徒番号
                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")).Trim = "" Then      '生徒名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_P")))             '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_P")))             '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))           '支店名
                    Select Case oraReader.GetString("KAMOKU_P")                             '科目
                        Case "02"
                            OutputCsvData("普")
                        Case "01"
                            OutputCsvData("当")
                        Case "09"
                            OutputCsvData("他")
                        Case Else
                            OutputCsvData("")
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_P")))               '振替口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KINGAKU_P")))             '振替金額
                    For No As Integer = 1 To 15                                             '費目名1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H")))
                    Next
                    For No As Integer = 1 To 15                                             '費目金額1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KINGAKU" & No.ToString("00") & "_P")))
                    Next
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_O")), True)         '名義人カナ名
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEIKYU_TUKI_P")))
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END  
                    If GCOM.NzStr(oraReader.GetString("TYOUSI_FLG_O")) = "1" Then           '長子
                        OutputCsvData("※")
                    Else
                        OutputCsvData("")
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_O")))               '年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")))              '通番
                    For No As Integer = 1 To 20
                        If GCOM.NzStr(oraReader.GetInt("CLASS_CODE_P")) = GCOM.NzStr(oraReader.GetInt("CLASS_CODE1" & No.ToString("00") & "_G")) Then
                            OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_NAME1" & No.ToString("00") & "_G")))    'クラス名
                            Exit For
                        End If
                    Next
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------START
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")))               '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))               '企業コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIMATOME_SIT_T")))               '取りまとめ店
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIMATOME_SIT_NAME")))               '取りまとめ店名
                    '2011/06/16 標準版修正 請求月、取りまとめ店印字対応------------------END

                    '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_P")))                       '費目ID
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
