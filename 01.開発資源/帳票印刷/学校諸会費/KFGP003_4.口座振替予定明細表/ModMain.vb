Imports System
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFGP003", "口座振替予定明細表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public SyoriDate As String = ""     '処理日
    Public FuriDate As String = ""      '振替日
    Public GakkouCode As String         '学校コード
    Public SortKey As String            '1:店番ソートなし 2:店番ソート
    Public SortKbn As String            ' 0:学年,クラス,生徒番号 1:入学年度,通番 2:あいうえお(生徒名(ｶﾅ))
    Public RecordCnt As Long = 0        '出力レコード数
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    '2017/03/02 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
    Public STR_HIMOKU_PTN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HIMOKU_PTN")
    '2017/03/02 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
    '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- START
    Public STR_TYOUSI_KBN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_TYOUSI_KBN")
    '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- END
    '
    ' 機能　 ：口座振替予定明細表印刷 メイン処理
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
            If Cmd.Length <> 6 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)          'ログイン名取得
            GakkouCode = Cmd(1)         '学校コード
            SyoriDate = Cmd(2)          '処理日
            FuriDate = Cmd(3)           '振替日
            SortKey = Cmd(4)            'ソートキー(帳票選択キー)
            SortKbn = Cmd(5)            '帳票ソート順

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            '---------------------------------------------------------
            ' 口座振替予定明細表印刷処理
            '---------------------------------------------------------
            Dim List As New KFGP003(SortKey)
            List.CreateCsvFile()
            If List.MakeRecord = False Then
                If RecordCnt = -1 Then   '印刷対象なし
                    Return -1
                Else
                    Return -300 '処理中のエラー
                End If
            End If

            '印刷処理実行
            If List.G_ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
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
