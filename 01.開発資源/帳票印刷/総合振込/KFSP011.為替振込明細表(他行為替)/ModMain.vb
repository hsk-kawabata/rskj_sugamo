Option Strict On
Option Explicit On

Imports System
Imports System.IO

Module ModMain

    Private BatchLog As New CASTCommon.BatchLOG("KFSP011", "為替振込明細表(他行為替)")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite
    '
    ' 機能　 ：為替振込明細表印刷 メイン処理
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
        Dim CSVFileName As String = ""  'ＣＳＶファイル名(フルパス)
        LW.UserID = ""      'ログイン名
        LW.ToriCode = "0000000000-00"
        LW.FuriDate = "00000000"
        Try
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
            '2016/10/21 saitou RSV2 UPD 信組対応 ---------------------------------------- START
            'MT代行用の帳票定義を呼び出せるように処理を変更
            Dim strSOUSINKBN As String = String.Empty
            If Cmd.Length <> 2 AndAlso Cmd.Length <> 3 Then
                'If Cmd.Length <> 2 Then
                '2016/10/21 saitou RSV2 UPD ------------------------------------------------- END
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)        'ログイン名取得
            CSVFileName = Cmd(1)    'ＣＳＶファイル名取得
            '2016/10/21 saitou RSV2 ADD 信組対応 ---------------------------------------- START
            If Cmd.Length = 3 Then
                strSOUSINKBN = Cmd(2)
            End If
            '2016/10/21 saitou RSV2 ADD ------------------------------------------------- END

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            If Not File.Exists(CSVFileName) Then
                '対象CSVファイルなし
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "CSVファイル取得失敗 CSVファイル名:" & CSVFileName)
                Return -200
            End If
            '---------------------------------------------------------
            ' 為替振込明細表印刷処理
            '---------------------------------------------------------
            '2016/10/21 saitou RSV2 UPD 信組対応 ---------------------------------------- START
            Dim pt As New KFSP011(CSVFileName, strSOUSINKBN)
            'Dim pt As New KFSP011(CSVFileName)
            '2016/10/21 saitou RSV2 UPD ------------------------------------------------- END

            '印刷処理実行
            If pt.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", pt.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If
            Return 0
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
        Finally
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
        End Try

    End Function

End Module
