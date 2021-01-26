Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports MenteCommon.clsCommon
Public Class KFGP026

    Inherits CAstReports.ClsReportBase
    Sub New()
        ' CSVファイルセット
        InfoReport.ReportName = "KFGP026"

        ' 定義体名セット
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        If STR_HIMOKU_PTN = "1" Then
            ReportBaseName = "KFGP026_学校生徒名簿(費目15).rpd"
        Else
            ReportBaseName = "KFGP026_学校生徒名簿.rpd"
        End If
        'ReportBaseName = "KFGP026_学校生徒名簿.rpd"
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
            SQL.Append(" SEITOMAST.SEIBETU_O")
            SQL.Append(",SEITOMAST.GAKUNEN_CODE_O")
            SQL.Append(",SEITOMAST.FURIKAE_O")
            SQL.Append(",SEITOMAST.KAMOKU_O")
            SQL.Append(",SEITOMAST.GAKKOU_CODE_O")
            SQL.Append(",SEITOMAST.NENDO_O")
            SQL.Append(",SEITOMAST.TUUBAN_O")
            SQL.Append(",SEITOMAST.SEITO_NO_O")
            SQL.Append(",SEITOMAST.TUKI_NO_O")
            SQL.Append(",GAKMAST1.GAKKOU_NNAME_G")
            SQL.Append(",SEITOMAST.MEIGI_KNAME_O")
            SQL.Append(",SEITOMAST.TKIN_NO_O")
            SQL.Append(",SEITOMAST.TSIT_NO_O")
            SQL.Append(",TENMAST.KIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N")
            SQL.Append(",SEITOMAST.CLASS_CODE_O")
            SQL.Append(",SEITOMAST.KEIYAKU_DENWA_O")
            SQL.Append(",SEITOMAST.KOUZA_O")
            SQL.Append(",SEITOMAST.KEIYAKU_NJYU_O")
            SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            SQL.Append(",SEITOMAST.SEITO_NNAME_O")
            SQL.Append(",SEITOMAST.HIMOKU_ID_O")
            SQL.Append(",GAKMAST1.GAKUNEN_NAME_G")
            For No As Integer = 1 To 15
                SQL.Append(",SEITOMAST.SEIKYU" & No.ToString("00") & "_O")
                SQL.Append(",SEITOMAST.KINGAKU" & No.ToString("00") & "_O")
                SQL.Append(",HIMOMAST.HIMOKU_NAME" & No.ToString("00") & "_H")
                SQL.Append(",HIMOMAST.HIMOKU_KINGAKU" & No.ToString("00") & "_H")
            Next
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",GAKMAST2.FURI_CODE_T")
            SQL.Append(",GAKMAST2.KIGYO_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            SQL.Append(" FROM KZFMAST.SEITOMAST")
            SQL.Append(",KZFMAST.GAKMAST1")
            SQL.Append(",KZFMAST.TENMAST")
            SQL.Append(",KZFMAST.HIMOMAST")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            SQL.Append(",KZFMAST.GAKMAST2")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<
            SQL.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = GAKMAST1.GAKKOU_CODE_G")
            SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O = GAKMAST1.GAKUNEN_CODE_G")
            SQL.Append(" AND SEITOMAST.TKIN_NO_O = TENMAST.KIN_NO_N (+)")
            SQL.Append(" AND SEITOMAST.TSIT_NO_O = TENMAST.SIT_NO_N (+)")
            SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O = HIMOMAST.GAKKOU_CODE_H")
            SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H")
            SQL.Append(" AND SEITOMAST.TUKI_NO_O = HIMOMAST.TUKI_NO_H")
            SQL.Append(" AND SEITOMAST.HIMOKU_ID_O = HIMOMAST.HIMOKU_ID_H")

            SQL.Append(" AND SEITOMAST.GAKKOU_CODE_O  =" & SQ(GakkouCode))

            '学年未入力時は全学年印刷する
            If GAKUNEN.Trim <> "" Then
                SQL.Append(" AND SEITOMAST.GAKUNEN_CODE_O =" & SQ(GAKUNEN))
            End If

            SQL.Append(" AND SEITOMAST.TUKI_NO_O      =" & SQ(TUKI))

            '解約フラグも見る
            SQL.Append(" AND SEITOMAST.KAIYAKU_FLG_O ='0'")

            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 -------------------->
            ' 結合条件：GAKMAST1.GAKKOU_CODE_GとGAKMAST2.GAKKOU_CODE_T
            SQL.Append(" AND GAKMAST1.GAKKOU_CODE_G = GAKMAST2.GAKKOU_CODE_T")
            ' 2010/09/15 TASK)saitou 振替/企業コード印字対応 --------------------<

            SQL.Append(" ORDER BY GAKKOU_CODE_O,GAKUNEN_CODE_O,CLASS_CODE_O")

            If PrintSort_Seito = "1" Then
                SQL.Append(",SEITOMAST.SEITO_NO_O")
            End If

            If PrintSort_Tuuban = "1" Then
                SQL.Append(",SEITOMAST.TUUBAN_O")
            End If

            If PrintSort_Waon = "1" Then
                SQL.Append(",SEITOMAST.SEITO_KNAME_O")
            End If

            If PrintSort_Seibetu = "1" Then
                SQL.Append(",SEITOMAST.SEIBETU_O")
            End If



            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStrはNULL値対策
                    OutputCsvData(mMatchingDate)                                            '処理日
                    OutputCsvData(mMatchingTime)                                            'タイムスタンプ
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_CODE_O")))         '学校コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKKOU_NNAME_G")))        '学校名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_CODE_O")))        '学年
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("GAKUNEN_NAME_G")))       '学年名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("NENDO_O")))               '入学年度
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUUBAN_O")))              '通番
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("CLASS_CODE_O")))          'クラス
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NO_O")))            '生徒番号

                    If GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")).Trim = "" Then      '生徒名
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_KNAME_O")), True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("SEITO_NNAME_O")), True)
                    End If

                    Select Case oraReader.GetString("SEIBETU_O")                            '性別
                        Case "0"
                            OutputCsvData("男")
                        Case "1"
                            OutputCsvData("女")
                        Case "2"
                            OutputCsvData("-")
                        Case Else
                            OutputCsvData("")
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MEIGI_KNAME_O")), True)         '保護者名

                    Select Case oraReader.GetString("FURIKAE_O")                            '振替方法
                        Case "0"
                            OutputCsvData("口座振替")
                        Case "1"
                            OutputCsvData("集金扱い")
                        Case "2"
                            OutputCsvData("振替停止")
                        Case Else
                            OutputCsvData("")
                    End Select

                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_NJYU_O")))        '住所
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KEIYAKU_DENWA_O")))       '電話番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TKIN_NO_O")))             '取扱金融機関コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIN_NNAME_N")))           '取扱金融機関名
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TSIT_NO_O")))             '支店コード
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("SIT_NNAME_N")))           '取扱支店名
                    Select Case oraReader.GetString("KAMOKU_O")                             '科目
                        Case "02"
                            OutputCsvData("普通")
                        Case "01"
                            OutputCsvData("当座")
                        Case "09"
                            OutputCsvData("その他")
                        Case Else
                            OutputCsvData("")
                    End Select
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KOUZA_O")))               '口座番号
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TUKI_NO_O")))             '対象月
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_ID_O")))           '費目ID 

                    For No As Integer = 1 To 15
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HIMOKU_NAME" & No.ToString("00") & "_H"))) '費目名1～15
                    Next

                    For No As Integer = 1 To 15
                        Select Case oraReader.GetString("SEIKYU" & No.ToString("00") & "_O") '請求方法1～15
                            Case "0"
                                OutputCsvData("一律")
                            Case "1"
                                OutputCsvData("個別")
                            Case Else
                                OutputCsvData("")
                        End Select
                    Next

                    For No As Integer = 1 To 15

                        '2011/06/16 標準版修正 振替金額は請求方法から判断 ------------------START
                        Select Case oraReader.GetInt64("SEIKYU" & No.ToString("00") & "_O") '請求方法1～15
                            Case 0
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("HIMOKU_KINGAKU" & No.ToString("00") & "_H")))
                            Case Else
                                OutputCsvData(GCOM.NzStr(oraReader.GetInt64("KINGAKU" & No.ToString("00") & "_O")))
                        End Select
                        'Select Case oraReader.GetInt64("KINGAKU" & No.ToString("00") & "_O") '費目金額1～15
                        '    Case 0
                        '        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("HIMOKU_KINGAKU" & No.ToString("00") & "_H")))
                        '    Case Else
                        '        OutputCsvData(GCOM.NzStr(oraReader.GetInt64("KINGAKU" & No.ToString("00") & "_O")))
                        'End Select
                        '2011/06/16 標準版修正 振替金額は請求方法から判断 ------------------END
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
