Imports System
Imports System.IO
Imports System.Diagnostics

Module ModMain

    Private BatchLog As New CASTCommon.BatchLOG("KFJP026", "処理結果確認表(落込取消)")
    
    '
    ' 機能　 ：処理結果確認表(落込取消)印刷 メイン処理
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
        Dim CSVFileName As String = ""  'ＣＳＶファイル名(フルパス)
        Dim Toris_Code As String = "0000000000"
        Dim Torif_Code As String = "00"
        Try
            BatchLog.Write(LoginID, Toris_Code & "-" & Torif_Code, "00000000", "(印刷処理)開始", "成功", "")
            PrinterName = ""    '通常使うプリンタ

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LoginID, Toris_Code & "-" & Torif_Code, "00000000", "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 2 Then
                'パラメータ間違い
               BatchLog.Write(LoginID, Toris_Code & "-" & Torif_Code, "00000000", "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LoginID = Cmd(0)        'ログイン名取得
            CSVFileName = Cmd(1)    'ＣＳＶファイル名取得
            BatchLog.Write(LoginID, Toris_Code & "-" & Torif_Code, "00000000", "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            If Not File.Exists(CSVFileName) Then
                '対象CSVファイルなし
                BatchLog.Write(LoginID, Toris_Code & "-" & Torif_Code, "00000000", "パラメータチェック", "失敗", "CSVファイル取得失敗 CSVファイル名:" & CSVFileName)
                Return -200
            End If
            '---------------------------------------------------------
            ' 処理結果確認表(落込取消)印刷処理
            '---------------------------------------------------------
            Dim Ot As New KFJP026(CSVFileName)

            '印刷処理実行
            If Ot.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "RepoAgent印刷", "失敗", Ot.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If
            Return 0
        Catch ex As Exception
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "例外", "失敗", ex.Message)
        Finally
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "(印刷処理)終了", "成功", "")
        End Try

    End Function

End Module
