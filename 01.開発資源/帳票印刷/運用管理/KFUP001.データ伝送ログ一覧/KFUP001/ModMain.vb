Imports System
Imports System.IO
Imports System.Diagnostics

Module ModMain

    Private BatchLog As New CASTCommon.BatchLOG("KFUP001", "データ伝送ログ一覧印刷バッチ")
    
    '
    ' 機能　 ：振替不能明細表 メイン処理
    '
    ' 引数　 ：ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 － 正常
    '           1 － 異常(パラメータ異常)
    '           2 － 異常(印刷処理失敗)
    '          -1 － 異常(例外発生)
    ' 備考　 ：戻り値は暫定的に書いてます。
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       'プリンタ名
        Dim LoginID As String = ""      'ログイン名
        Dim CSVFileName As String = ""  'ＣＳＶファイル名(フルパス)

        Try
            BatchLog.Write("", "0000000000-00", "00000000", "(印刷処理)開始", "成功", "")
            PrinterName = ""    '通常使うプリンタ

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write("", "0000000000-00", "00000000", "パラメータ取得", "失敗", "パラメータなし")
                Return 1
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 2 Then
                'パラメータ間違い
                BatchLog.Write("", "0000000000-00", "00000000", "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return 1
            End If

            LoginID = Cmd(0)        'ログイン名取得
            CSVFileName = Cmd(1)    'ＣＳＶファイル名取得
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            '---------------------------------------------------------
            ' データ伝送ログ一覧印刷処理
            '---------------------------------------------------------
            Dim prtDensouLog As New prtDatadensoulog(CSVFileName)

            '印刷処理実行
            If prtDensouLog.ReportExecute(PrinterName) = True Then
                '印刷成功
                'BatchLog.Write(LoginID, "0000000000-00", "00000000", "RepoAgent印刷", "成功", "印刷に成功しました。")
            Else
                '印刷失敗
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "RepoAgent印刷", "失敗", prtDensouLog.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If

            If Not prtDensouLog.HostCsvName Is Nothing AndAlso prtDensouLog.HostCsvName <> "" Then
                Try
                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    DestName &= prtDensouLog.HostCsvName
                    File.Copy(prtDensouLog.FileName, DestName, True)
                Catch ex As Exception
                    'ＣＳＶファイル後処理失敗
                    BatchLog.Write(LoginID, "0000000000-00", "00000000", "ＣＳＶファイル後処理", "失敗", "CSVファイル後処理に失敗しました。")
                End Try
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "例外", "失敗", ex.Message)
            Return -1
        Finally
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "(印刷処理)終了", "成功", "")
        End Try

    End Function

End Module
