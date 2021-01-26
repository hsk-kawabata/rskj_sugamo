Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP027

    Inherits CAstReports.ClsReportBase
    Sub New(ByVal Kbn As String)
        If Kbn = "1" Then
            ' CSVファイルセット
            InfoReport.ReportName = "KFGP027"
            ' 定義体名セット
            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            If STR_HIMOKU_PTN = "1" Then
                ReportBaseName = "KFGP027_28_生徒マスタ登録チェックリスト(費目15).rpd"
            Else
                ReportBaseName = "KFGP027_28_生徒マスタ登録チェックリスト.rpd"
            End If
            'ReportBaseName = "KFGP027_28_生徒マスタ登録チェックリスト.rpd"
            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
        Else
            ' CSVファイルセット
            InfoReport.ReportName = "KFGP028"
            ' 定義体名セット
            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            If STR_HIMOKU_PTN = "1" Then
                ReportBaseName = "KFGP027_28_生徒マスタ登録チェックリスト(費目15).rpd"
            Else
                ReportBaseName = "KFGP027_28_生徒マスタ登録チェックリスト.rpd"
            End If
            'ReportBaseName = "KFGP027_28_生徒マスタ登録チェックリスト.rpd"
            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
        End If

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
            SQL.Append(" SEITOMAST.SEIBETU_O")
            SQL.Append(",SEITOMAST.FURIKAE_O")
            SQL.Append(",SEITOMAST.KAMOKU_O")
            SQL.Append(",SEITOMAST.KAIYAKU_FLG_O")
            SQL.Append(",SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(",SEITOMAST.NENDO_O")
            SQL.Append(",SEITOMAST.GAKUNEN_CODE_O")
            SQL.Append(",SEITOMAST.TUUBAN_O")
            SQL.Append(",SEITOMAST.SEITO_NO_O")
            SQL.Append(",SEITOMAST.SEITO_NNAME_O")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            SQL.Append(",SEITOMAST.MEIGI_KNAME_O")
            SQL.Append(",SEITOMAST.MEIGI_NNAME_O")
            SQL.Append(",SEITOMAST.TKIN_NO_O")
            SQL.Append(",SEITOMAST.TSIT_NO_O")
            SQL.Append(",TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(",SEITOMAST.CLASS_CODE_O")
            SQL.Append(",SEITOMAST.KEIYAKU_DENWA_O")
            For No As Integer = 1 To 15
                SQL.Append(",HIMOMAST.HIMOKU_NAME" & No.ToString("00") & "_H")
                SQL.Append(",SEITOMAST.SEIKYU" & No.ToString("00") & "_O")
                SQL.Append(",HIMOMAST.HIMOKU_KINGAKU" & No.ToString("00") & "_H")
                SQL.Append(",SEITOMAST.KINGAKU" & No.ToString("00") & "_O")
            Next
            SQL.Append(",SEITOMAST.KOUZA_O")
            SQL.Append(",SEITOMAST.HIMOKU_ID_O")
            SQL.Append(",SEITOMAST.TUKI_NO_O")
            SQL.Append(",SEITOMAST.SAKUSEI_DATE_O")
            SQL.Append(",SEITOMAST.KOUSIN_DATE_O")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            SQL.Append(" FROM ")
            SQL.Append(" KZFMAST.SEITOMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.TENMAST")
            SQL.Append(",KZFMAST.HIMOMAST")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",KZFMAST.GAKMAST2")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            SQL.Append(" WHERE  SEITOMAST.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G (+)")
            If PrintKbn = "1" Then
                SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O = GAKMAST1.GAKUNEN_CODE_G (+)")
                SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H (+)")
            Else
                SQL.Append(" AND 1 = GAKMAST1.GAKUNEN_CODE_G (+)")
                SQL.Append(" AND 1 = HIMOMAST.GAKUNEN_CODE_H (+)")
                SQL.Append(" AND GAKMAST1.GAKUNEN_CODE_G = HIMOMAST.GAKUNEN_CODE_H")
            End If
            SQL.Append(" AND SEITOMAST.TKIN_NO_O = TENMAST.KIN_NO_N (+)")
            SQL.Append(" AND SEITOMAST.TSIT_NO_O = TENMAST.SIT_NO_N (+)")
            SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O = HIMOMAST.GAKKOU_CODE_H (+)")
            SQL.Append(" AND SEITOMAST.HIMOKU_ID_O = HIMOMAST.HIMOKU_ID_H (+)")
            SQL.Append(" AND SEITOMAST.TUKI_NO_O = HIMOMAST.TUKI_NO_H (+)")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(" AND GAKMAST1.GAKKOU_CODE_G = GAKMAST2.GAKKOU_CODE_T (+)")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

            '生徒マスタ（学校コード、作成日、更新日）
            'レコード抽出設定
            '指定学校コード
            SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O = " & SQ(GakkouCode))
         
            '作成日
            SQL.Append(" AND (SEITOMAST.SAKUSEI_DATE_O = " & SQ(KousinDate))
            '更新日
            SQL.Append(" OR SEITOMAST.KOUSIN_DATE_O = " & SQ(KousinDate) & ")")
            '請求月
            SQL.Append(" AND SEITOMAST.TUKI_NO_O = " & SQ(Tuki))

            SQL.Append(" ORDER BY SEITOMAST.GAKKOU_CODE_O")
            Select Case PrintSort
                Case "0"
                    SQL.Append(",SEITOMAST.GAKUNEN_CODE_O")
                    SQL.Append(",SEITOMAST.CLASS_CODE_O")
                    SQL.Append(",SEITOMAST.SEITO_NO_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case "1"
                    SQL.Append(",SEITOMAST.GAKUNEN_CODE_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
                Case Else
                    SQL.Append(",SEITOMAST.GAKUNEN_CODE_O")
                    SQL.Append(",SEITOMAST.SEITO_KNAME_O")
                    SQL.Append(",SEITOMAST.NENDO_O")
                    SQL.Append(",SEITOMAST.TUUBAN_O")
            End Select

            If PrintKbn = "2" Then
                SQL.Replace("SEITOMAST", "SEITOMAST2")
            End If

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    If PrintKbn = "1" Then                                                  'タイトル
                        OutputCsvData("生徒マスタ登録チェックリスト")
                    Else
                        OutputCsvData("新入生マスタ登録チェックリスト")
                    End If
                    OutputCsvData(KousinDate)                                               '更新日
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_O")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")), True)  '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_O")))               '入学年度
                    If PrintKbn = "1" Then                                                  '学年
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_O")))
                    Else
                        OutputCsvData("0")
                    End If

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")))              '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_O")))            '生徒番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)   '生徒氏名カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True)   '生徒氏名漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_O")), True)   '口座名義カナ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_NNAME_O")), True)   '口座名義漢字
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_O")))             '取扱金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")))           '取扱金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_O")))             '取扱支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))           '取扱支店名

                    Select Case GCOM.NzStr(oraReader.GetString("KAMOKU_O"))                 '科目
                        Case "01"
                            OutputCsvData("当座")
                        Case "02"
                            OutputCsvData("普通")
                        Case Else
                            OutputCsvData("")
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_O")))               '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_O")))          'クラス
                    Select Case GCOM.NzStr(oraReader.GetString("SEIBETU_O"))                '性別
                        Case "0"
                            OutputCsvData("男")
                        Case "1"
                            OutputCsvData("女")
                        Case "2"
                            OutputCsvData("-")
                        Case Else
                            OutputCsvData("")
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("FURIKAE_O"))                '振替方法
                        Case "0"
                            OutputCsvData("口座振替")
                        Case "1"
                            OutputCsvData("集金扱い")
                        Case "2"
                            OutputCsvData("振替停止")
                        Case Else
                            OutputCsvData("")
                    End Select

                    Select Case GCOM.NzStr(oraReader.GetString("KAIYAKU_FLG_O"))            '解約区分
                        Case "0"
                            OutputCsvData("通常")
                        Case "9"
                            OutputCsvData("解約")
                        Case Else
                            OutputCsvData("")
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_DENWA_O")))       '電話番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUKI_NO_O")))             '対象月
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_O")))           '費目ID
                    For No As Integer = 1 To 15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H"))) '費目名1～15
                    Next
                    For No As Integer = 1 To 15
                        Select Case GCOM.NzStr(oraReader.GetString("SEIKYU" & No.ToString("00") & "_O"))  '請求方法1～15
                            Case "0"
                                OutputCsvData("一律")
                            Case "1"
                                OutputCsvData("個別")
                            Case Else

                        End Select
                    Next
                    For No As Integer = 1 To 15
                        '2017/04/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                        '請求金額を請求方法で判定するように修正
                        Select Case GCOM.NzStr(oraReader.GetString("SEIKYU" & No.ToString("00") & "_O"))  '請求方法1～15
                            Case "1"
                                OutputCsvData(GCOM.NzLong(oraReader.GetString("KINGAKU" & No.ToString("00") & "_O")))
                            Case Else
                                OutputCsvData(GCOM.NzLong(oraReader.GetString("HIMOKU_KINGAKU" & No.ToString("00") & "_H")))
                        End Select
                        'Select Case GCOM.NzLong(oraReader.GetString("KINGAKU" & No.ToString("00") & "_O"))
                        '    Case Is > 0
                        '        OutputCsvData(GCOM.NzLong(oraReader.GetString("KINGAKU" & No.ToString("00") & "_O")))
                        '    Case Else
                        '        OutputCsvData(GCOM.NzLong(oraReader.GetString("HIMOKU_KINGAKU" & No.ToString("00") & "_H")))
                        'End Select
                        '2017/04/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
                    Next
                    ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
                    ' 振替コード、企業コードを追加（改行用ダミーは企業コードで改行するため削除）
                    'OutputCsvData("", False, True)                                          '改行用ダミー
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_T")))                   '振替コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_T")), False, True)     '企業コード
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
