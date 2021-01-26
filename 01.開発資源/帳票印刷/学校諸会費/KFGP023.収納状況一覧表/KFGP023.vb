' ============================================================================
'  HISTORY
'   No  Ver     Date          Name              Comment
'   01  V01L01  2020/06/16    FJH)AMANO         ＰＫＧ修正(PKG_2020_0012_000)
' ============================================================================
Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP023

    Inherits CAstReports.ClsReportBase
    Sub New()

        ' CSVファイルセット
        InfoReport.ReportName = "KFGP023"
        ' 定義体名セット
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        If STR_HIMOKU_PTN = "1" Then
            ReportBaseName = "KFGP023_収納状況一覧表(費目15).rpd"
        Else
            ReportBaseName = "KFGP023_収納状況一覧表.rpd"
        End If
        'ReportBaseName = "KFGP023_収納状況一覧表.rpd"
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

            SQL.Append("SELECT ")
            SQL.Append(" G_PRTWORK.GAKKOU_CODE_P")
            SQL.Append(",G_PRTWORK.GAKUNEN_CODE_P")
            SQL.Append(",G_PRTWORK.CLASS_CODE_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM01_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM02_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM03_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM04_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM05_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM06_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM07_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM08_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM09_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM10_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM11_P")
            SQL.Append(",G_PRTWORK.KEKKA_MM12_P")
            SQL.Append(",G_PRTWORK.SEITO_KNAME_P")
            SQL.Append(",G_PRTWORK.SEITO_NNAME_P")
            SQL.Append(",G_PRTWORK.NENDO_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME01_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME02_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME03_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME04_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME05_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME06_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME07_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME08_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME09_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME10_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME11_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME12_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME13_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME14_P")
            SQL.Append(",G_PRTWORK.HIMOKU_NAME15_P")
            SQL.Append(",G_PRTWORK.KINGAKU_P")
            SQL.Append(",G_PRTWORK.KINGAKU01_P")
            SQL.Append(",G_PRTWORK.KINGAKU02_P")
            SQL.Append(",G_PRTWORK.KINGAKU03_P")
            SQL.Append(",G_PRTWORK.KINGAKU04_P")
            SQL.Append(",G_PRTWORK.KINGAKU05_P")
            SQL.Append(",G_PRTWORK.KINGAKU06_P")
            SQL.Append(",G_PRTWORK.KINGAKU07_P")
            SQL.Append(",G_PRTWORK.KINGAKU08_P")
            SQL.Append(",G_PRTWORK.KINGAKU09_P")
            SQL.Append(",G_PRTWORK.KINGAKU10_P")
            SQL.Append(",G_PRTWORK.KINGAKU11_P")
            SQL.Append(",G_PRTWORK.KINGAKU12_P")
            SQL.Append(",G_PRTWORK.KINGAKU13_P")
            SQL.Append(",G_PRTWORK.KINGAKU14_P")
            SQL.Append(",G_PRTWORK.KINGAKU15_P")
            SQL.Append(",G_PRTWORK.TUUBAN_P")
            SQL.Append(",G_PRTWORK.GAKKOU_NNAME_P")
            SQL.Append(",G_PRTWORK.SEITO_NO_P")
            SQL.Append(",G_PRTWORK.HIMOKU_ID_P")
            SQL.Append(",G_PRTWORK.TKIN_NO_P")
            SQL.Append(",G_PRTWORK.TSIT_NO_P")
            SQL.Append(",G_PRTWORK.KAMOKU_P")
            SQL.Append(",G_PRTWORK.KOUZA_P")
            SQL.Append(",G_PRTWORK.GAKUNEN_NAME_P")
            SQL.Append(",G_PRTWORK.CLASS_NAME_P")
            SQL.Append(",G_PRTWORK.FURI_DATE_P")
            SQL.Append(",G_PRTWORK.MEIGI_KNAME_P")
            SQL.Append(",GAKMAST2.JIFURI_KBN_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            SQL.Append(",GAKMAST2.YOBI1_T")             'ユーザID
            SQL.Append(",GAKMAST2.BAITAI_CODE_T")       '媒体コード 
            SQL.Append(",GAKMAST2.ITAKU_CODE_T")        '委託者コード
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
            SQL.Append(" FROM")
            SQL.Append(" KZFMAST.GAKMAST2")
            SQL.Append(",KZFMAST.G_PRTWORK")
            SQL.Append(" WHERE GAKMAST2.GAKKOU_CODE_T = G_PRTWORK.GAKKOU_CODE_P")
            SQL.Append(" AND   G_PRTWORK.GAKKOU_CODE_P = " & SQ(GakkouCode))
            SQL.Append(" ORDER BY G_PRTWORK.GAKKOU_CODE_P, G_PRTWORK.GAKUNEN_CODE_P, G_PRTWORK.CLASS_CODE_P")
            Select Case PrintSort
                Case "0"
                    '学年、クラス
                    SQL.Append(", G_PRTWORK.GAKUNEN_CODE_P ,  G_PRTWORK.CLASS_CODE_P ,  G_PRTWORK.SEITO_NO_P,G_PRTWORK.NENDO_P ASC,  G_PRTWORK.TUUBAN_P ASC")
                Case "1"
                    '年度、通番
                    SQL.Append(", G_PRTWORK.GAKUNEN_CODE_P ASC ,G_PRTWORK.CLASS_CODE_P ,G_PRTWORK.NENDO_P ASC,  G_PRTWORK.TUUBAN_P ASC")
                Case Else
                    'あいうえお順
                    SQL.Append(", G_PRTWORK.GAKUNEN_CODE_P ASC , G_PRTWORK.CLASS_CODE_P ,G_PRTWORK.SEITO_KNAME_P , G_PRTWORK.NENDO_P ,  G_PRTWORK.TUUBAN_P")
            End Select

            If oraReader.DataReader(SQL) = True Then
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
                User_ID = oraReader.GetString("YOBI1_T")
                BaitaiCode = oraReader.GetString("BAITAI_CODE_T")
                ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END
                While oraReader.EOF = False
                    OutputCsvData(mMatchingDate)                                                '処理日
                    OutputCsvData(mMatchingTime)                                                'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_P")))             '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_P")))            '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_P")))            '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_P")))            '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_P")))              'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_NAME_P")))              'クラス名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_P")))               '振替日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_P")))                '生徒番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_P")))                   '年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_P")))                  '通番
                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_P")).Trim = "" Then          '生徒名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_P")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_P")), True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_P")))               '費目ID
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_P")))                 '金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_P")))                 '支店コード
                    Select Case oraReader.GetString("KAMOKU_P")                                 '科目
                        Case "02"
                            OutputCsvData("普通")
                        Case "01"
                            OutputCsvData("当座")
                        Case "09"
                            OutputCsvData("その他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_P")))                   '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_P")), True)             '名義人カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KINGAKU_P")))                 '振替金額

                    For No As Integer = 1 To 15                                                 '費目名1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_P")))
                    Next
                    For No As Integer = 1 To 15                                                 '費目金額1～15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KINGAKU" & No.ToString("00") & "_P")))
                    Next

                    For No As Integer = 1 To 12                                                 '1～12月収納状況
                        Dim Jyokyo As String
                        If oraReader.GetString("KEKKA_MM" & No.ToString("00") & "_P") = "0" Then
                            Jyokyo = "○"   '今回収納

                            ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- START
                            '振替が無かった場合または未来分は×ではなく空白とする
                        ElseIf oraReader.GetString("KEKKA_MM" & No.ToString("00") & "_P").Trim = "" Then
                            Jyokyo = ""   '今回依頼無／未来分
                            ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- END

                        Else
                            Jyokyo = "×"   '今回不能
                        End If

                        Select Case No
                            Case 1 To 11
                                OutputCsvData(Jyokyo)
                            Case 12
                                ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                                ' 12月収納状況は最終項目でなくなったため改行を削除
                                'OutputCsvData(Jyokyo, False, True)
                                OutputCsvData(Jyokyo)
                                ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
                        End Select
                    Next

                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))                   '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), False, True)     '企業コード
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

                    RecordCnt += 1
                    oraReader.NextRead()
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
