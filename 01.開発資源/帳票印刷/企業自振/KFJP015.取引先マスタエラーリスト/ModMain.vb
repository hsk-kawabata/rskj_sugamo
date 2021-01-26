Imports System
Imports System.IO
Imports System.Diagnostics

' 取引先エラーリスト メインモジュール
Module ModMain

    ' ログ処理クラス
    Private BatchLog As New CASTCommon.BatchLOG("KFJP015", "取引先マスタエラーリスト印刷バッチ")

    Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       'プリンタ名
        Dim LoginID As String = ""      'ログイン名
        Dim CSVFileName As String = ""  'ＣＳＶファイル名(フルパス)

        Try
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "(印刷処理)開始", "成功", "")
            PrinterName = ""    '通常使うプリンタ

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "パラメータ取得", "失敗", "コマンドライン引数なし")
                Return 1
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 2 Then
                'パラメータ間違い
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "パラメータ取得", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return 1
            End If

            LoginID = Cmd(0)        'ログイン名取得
            CSVFileName = Cmd(1)    'ＣＳＶファイル名取得
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            '---------------------------------------------------------
            ' 取引先エラーリスト印刷処理
            '---------------------------------------------------------
            Dim PrnSchErrList As New PrnSchErrList(CSVFileName)

            '印刷処理実行
            If PrnSchErrList.ReportExecute(PrinterName) = True Then
                '印刷成功
                'BatchLog.Write(LoginID, "0000000000-00", "00000000", "(メイン)終了", "成功","")
            Else
                '印刷失敗
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "(印刷)", "失敗", PrnSchErrList.ReportMessage)
                Return 2
            End If

            If Not PrnSchErrList.HostCsvName Is Nothing AndAlso PrnSchErrList.HostCsvName <> "" Then
                Try
                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    DestName &= PrnSchErrList.HostCsvName
                    File.Copy(PrnSchErrList.FileName, DestName, True)
                Catch ex As Exception
                    'ＣＳＶファイル後処理失敗
                    BatchLog.Write(LoginID, "0000000000-00", "00000000", "ＣＳＶファイル後処理", "失敗", "")
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
