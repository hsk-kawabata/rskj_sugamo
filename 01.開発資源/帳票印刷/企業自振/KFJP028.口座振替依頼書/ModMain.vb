Imports System
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFJP028", "口座振替依頼書")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public TorisCode As String = ""     '取引先主コード
    Public TorifCode As String = ""     '取引先副コード
    Public FuriDate As String = ""      '振替日
    Public Syuturyoku As String = ""     '出力区分   '2009/12/17 追加
    Public RecordCnt As Long = 0        '出力レコード数
    Public AllRecordCnt As Long = 0     '全出力レコード数
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    '2017/05/22 saitou RSV2 DEL 標準版修正（潜在バグ） ---------------------------------------- START
    '用途不明
    'Public Ret As Integer
    '2017/05/22 saitou RSV2 DEL --------------------------------------------------------------- END
    '
    ' 機能　 ：口座振替依頼書印刷 メイン処理
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
            If Cmd.Length <> 4 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)              'ログイン名取得
            TorisCode = Mid(Cmd(1), 1, 10)  '取引先主コード
            TorifCode = Mid(Cmd(1), 11, 2)  '取引先副コード
            FuriDate = Cmd(2)               '振替日
            Syuturyoku = Cmd(3)
            LW.ToriCode = TorisCode + TorifCode
            LW.FuriDate = FuriDate
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            '---------------------------------------------------------
            ' 口座振替依頼書印刷処理
            '---------------------------------------------------------
            Dim Iraisyo As New KFJP028()
            Iraisyo.CreateCsvFile()
            If Iraisyo.MakeRecord = False Then
                If RecordCnt = -1 Then   '印刷対象なし
                    Return -1
                Else
                    Return -300         '処理中のエラー
                End If
            End If
            '印刷処理実行
            If Iraisyo.ReportExecute("") = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", Iraisyo.ReportMessage)
                '2017/05/22 saitou RSV2 MODIFY 標準版修正（潜在バグ） ------------------------------------- START
                '印刷に失敗しても成功になるのを修正
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                'Ret = -999     'レポエージェントからの戻り値を返す(暫定-999)
                'Return False
                '2017/05/22 saitou RSV2 MODIFY ------------------------------------------------------------ END
            End If
            Return 0
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            '2017/05/22 saitou RSV2 ADD 標準版修正（潜在バグ） ---------------------------------------- START
            '例外時もきちんと戻り値を返す
            Return -300
            '2017/05/22 saitou RSV2 ADD --------------------------------------------------------------- END
        Finally
            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", AllRecordCnt & "件")
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
            End If
        End Try

    End Function

End Module
