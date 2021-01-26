Imports System
Imports System.IO
Imports System.Diagnostics

Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFJP003", "受付明細表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public SyoriDate As String = ""     '処理日
    Public RecordCnt As Long = 0     '出力レコード数
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    '
    ' 機能　 ：受付明細表印刷（画面から） メイン処理
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
        Dim Ret As Integer              'データ作成結果

        Try
            Dim UkeMeisai As New KFJP003()

            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "開始", "成功", "")
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
            If Cmd.Length <> 6 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            UkeMeisai.ToriSCode = Cmd(1)    '取引先主コード
            UkeMeisai.ToriFCode = Cmd(2)    '取引先副コード
            UkeMeisai.FuriDate = Cmd(3)     '振替日
            UkeMeisai.SyoriKbn = Cmd(4)     '処理区分
            UkeMeisai.SortNo = Cmd(5)       'ソートコード

            LW.UserID = Cmd(0)              'ログイン名取得
            LW.ToriCode = Cmd(1) & Cmd(2)   '取引先主副
            LW.FuriDate = Cmd(3)            '振替日

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))

            '---------------------------------------------------------
            ' 処理結果確認表(返還一覧)印刷処理
            '---------------------------------------------------------
            UkeMeisai.CreateCsvFile()

            '対象データ書き出し
            Ret = UkeMeisai.OutputCsvData()
            If Ret <> 0 Then
                Return Ret
            End If

            '印刷処理実行
            If UkeMeisai.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", UkeMeisai.ReportMessage)
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
