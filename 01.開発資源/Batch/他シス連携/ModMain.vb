Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' 他シス連携処理 メインモジュール
Module ModMain
    ' ログ処理クラス
    Private MainLOG As New CASTCommon.BatchLOG("他シス連携", "OTHERSYS")

    '
    ' 機能　 ： 他シス連携 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 1,06,00,000007
    '           
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        '*** 修正 mitsu 2009/08/07 エラー検知機能強化3 ***
        Try
            CASTCommon.TraceLog.AddErrHandler()
            '*********************************************
            ' 戻り値
            Dim ret As Integer

            Console.WriteLine("処理開始")

            'Dim ELog As New CASTCommon.ClsEventLOG
            'ELog.Write("処理開始:" & CmdArgs(0))

            If CmdArgs.Length = 0 Then
                '*** 修正 mitsu 2009/08/07 PID追加 ***
                'MainLOG.Write("開始", "失敗", "引数なし")
                MainLOG.Write("開始 PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "失敗", "引数なし")
                '*************************************
                Return -100
            End If

            '*** 修正 mitsu 2009/08/07 開始ログ追加 ***
            MainLOG.Write("処理開始 PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "成功")
            '******************************************

            Select Case CmdArgs(0).Split(","c).Length
                Case 4
                    ' 一括連携
                    Dim IkkatuClass As New ClsIkkatuRenkei

                    ' メイン処理
                    ret = IkkatuClass.Main(CmdArgs(0))
                Case 1, 2
                    ' 伝送連携
                    Dim DensouClass As New ClsDensouRenkei
                    Dim Param() As String = CmdArgs(0).Split(","c)
                    If Param.Length = 2 Then
                        MainLOG.JobTuuban = CInt(Param(1))
                    End If

                    ret = DensouClass.Main(Param(0))
                    If ret <> 0 Then
                        Call DensouClass.InsertJOBMASTbyError(Param(0))
                    Else
                        If Param.Length = 2 Then
                            MainLOG.UpdateJOBMASTbyOK("再実行 成功")
                        End If
                    End If
                Case Else
                    '*** 修正 mitsu 2009/08/07 PID追加 ***
                    'MainLOG.Write("開始", "失敗", "パラメタ取得失敗[" & CmdArgs(0) & "]")
                    MainLOG.Write("開始 PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "失敗", "パラメタ取得失敗[" & CmdArgs(0) & "]")
                    '*************************************
                    '*** 修正 mitsu 2009/08/07 リターンコード追加 ***
                    ret = -100
                    '************************************************
            End Select

            Console.WriteLine("処理終了")
            'ELog.Write("処理終了:" & CmdArgs(0))
            '*** 修正 mitsu 2009/08/07 終了ログ追加 ***
            MainLOG.Write("処理終了 PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "成功")
            '******************************************

            Return ret
            '*** 修正 mitsu 2009/08/07 エラー検知機能強化3 ***
        Catch ex As Exception
            CASTCommon.TraceLog.ELogWrite(ex, Diagnostics.EventLogEntryType.Error)
            Return -99
        End Try
        '*****************************************************
    End Function

End Module
