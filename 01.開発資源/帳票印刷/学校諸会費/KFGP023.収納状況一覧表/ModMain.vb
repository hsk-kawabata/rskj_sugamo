Imports System
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFGP023", "収納状況一覧表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public RecordCnt As Long = 0        '出力レコード数
    Public GakkouCode As String         '学校コード
    Public PrintSort As String          'ソート順
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻

    ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-07(RSV2対応) -------------------- START
    Private INI_RSV2_G_SYUNOU As String = ""
    Private INI_COMMON_PRINT_NAME As String = ""
    ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-07(RSV2対応) -------------------- END
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
    Public STR_HIMOKU_PTN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HIMOKU_PTN")
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
    '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
    Public User_ID As String = ""       'ユーザＩＤ
    Public ITAKU_CODE As String = ""    '委託者コード
    Public BaitaiCode As String = ""    '媒体コード
    Public WEB_PRINTERNAME As String = CASTCommon.GetFSKJIni("WEB_DEN", "PRINTER")  'WEB伝送用プリンタ名
    Public WEB_PRINT As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_PRINT")      'WEB伝送プリント区分(0:PDFのみ、1:PDFと紙）
    '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END

    '
    ' 機能　 ：収納状況一覧表印刷 メイン処理
    '
    ' 引数　 ：ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0    － 正常
    '           -100 － 異常(パラメータなし)
    '           -200 － 異常(CSVファイル存在なし)
    '           -300 － 異常(処理中のエラー)
    '          -1    － 正常(印刷対象なし)
    '           他   － 異常(RepoAgentの戻り値)
    ' 備考　 ：
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       'プリンタ名
        Dim LoginID As String = ""      'ログイン名
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "成功", "")
            PrinterName = ""    '通常使うプリンタ

            ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-07(RSV2対応) -------------------- START
            INI_RSV2_G_SYUNOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SYUNOU")
            INI_COMMON_PRINT_NAME = CASTCommon.GetFSKJIni("COMMON", "PRINT_NAME")
            ' 2015/12/18 タスク）綾部 ADD 【PG】UI_B-14-07(RSV2対応) -------------------- END

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 3 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)          'ログイン名取得
            GakkouCode = Cmd(1)         '学校コード
            PrintSort = Cmd(2)         'ソート順
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            '---------------------------------------------------------
            ' 収納状況一覧表印刷処理
            '---------------------------------------------------------
            Dim List As New KFGP023()
            List.CreateCsvFile()
            If List.MakeRecord = False Then
                If RecordCnt = -1 Then   '印刷対象なし
                    Return -1
                Else
                    Return -300 '処理中のエラー
                End If
            End If

            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ START
            '---------------------------------------------------------
            ' WEB伝送用PDF作成処理
            '---------------------------------------------------------
            If BaitaiCode = "3" AndAlso User_ID <> "" Then
                If List.G_ReportExecute(WEB_PRINTERNAME, User_ID, ITAKU_CODE) = True Then
                    '印刷成功
                Else
                    '印刷失敗
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
                    Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                End If

                'PDF作成のみの場合はここで抜ける
                If WEB_PRINT = "0" Then
                    Return 0
                End If
            End If
            '2017/04/26 タスク）西野 ADD 標準版修正（学校WEB伝送対応）------------------------------------ END

            '印刷処理実行
            ' 2015/12/18 タスク）綾部 CHG 【PG】UI_B-14-07(RSV2対応) -------------------- START
            'If List.G_ReportExecute(PrinterName) = True Then
            '    '印刷成功
            'Else
            '    '印刷失敗
            '    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
            '    Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            'End If
            '=====================================================================================
            ' <INI_RSV2_G_SYUNOU>
            '  CASE "1"  : 通常プリンタ印刷           (PRINTERNAME)
            '  CASE "2"  : ListWorks印刷              (INI_COMMON_PRINT_NAME)
            '  CASE "3"  : 通常プリンタ,ListWorks印刷 (PRINTERNAME,INI_COMMON_PRINT_NAME)
            '  CASE ELSE : 通常プリンタ印刷           (PRINTERNAME)
            '=====================================================================================
            Select Case INI_RSV2_G_SYUNOU
                Case "1"
                    If List.G_ReportExecute(PrinterName) = True Then
                        '印刷成功
                    Else
                        '印刷失敗
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
                        Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                    End If
                Case "2"
                    If List.G_ReportExecute(INI_COMMON_PRINT_NAME) = True Then
                        '印刷成功
                    Else
                        '印刷失敗
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷(ListWorks)", "失敗", List.ReportMessage)
                        Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                    End If
                Case "3"
                    If List.G_ReportExecute(PrinterName) = True Then
                        '印刷成功
                    Else
                        '印刷失敗
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
                        Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                    End If

                    If List.G_ReportExecute(INI_COMMON_PRINT_NAME) = True Then
                        '印刷成功
                    Else
                        '印刷失敗
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷(ListWorks)", "失敗", List.ReportMessage)
                        Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                    End If
                Case Else
                    If List.G_ReportExecute(PrinterName) = True Then
                        '印刷成功
                    Else
                        '印刷失敗
                        BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
                        Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                    End If
            End Select
            ' 2015/12/18 タスク）綾部 CHG 【PG】UI_B-14-07(RSV2対応) -------------------- END

            Return 0
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
        Finally
            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", RecordCnt & "件")
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
            End If
        End Try

    End Function

End Module
