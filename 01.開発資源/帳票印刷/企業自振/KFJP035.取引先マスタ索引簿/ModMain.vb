﻿Imports System
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFJP035", "取引先マスタ索引簿")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public SortCd As String = ""     'ソート順
    Public RecordCnt As Long = 0     '出力レコード数
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    Public CENTER As String = CASTCommon.GetFSKJIni("COMMON", "CENTER") '2010/12/24 信組対応

    '
    ' 機能　 ：取引先マスタ索引簿印刷 メイン処理
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

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 2 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)        'ログイン名取得
            SortCd = Cmd(1)    'ソート順(1:取引先コード順 2:振替/企業コード順 3:委託者カナ名順)
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            Select Case SortCd
                Case 1, 2, 3
                Case Else
                    'ソート区分異常
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "ソート区分：" & SortCd)
                    Return -100
            End Select

            '---------------------------------------------------------
            ' 取引先マスタ索引簿印刷処理
            '---------------------------------------------------------
            Dim SAKUINBO As New KFJP035()
            SAKUINBO.CreateCsvFile()
            If SAKUINBO.MakeRecord = False Then
                If RecordCnt = -1 Then   '印刷対象なし
                    Return -1
                Else
                    Return -300 '処理中のエラー
                End If
            End If

            '印刷処理実行
            If SAKUINBO.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", SAKUINBO.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If
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
