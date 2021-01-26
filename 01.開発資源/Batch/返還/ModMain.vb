Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' 返還処理 メインモジュール
Module ModMain
    ' ログ処理クラス
    Private MainLOG As New CASTCommon.BatchLOG("返還データ作成")

    '
    ' 機能　 ： 返還 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '                  取引先コード（取引先主コード,取引先副コード,振替日,持込シーケンス,ジョブ通番
    '                  
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Function Main(ByVal CmdArgs() As String) As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "返還メイン処理(開始)", "成功")
            'BatchLog.Write("0000000000-00", "00000000", "ログイン(開始)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            CASTCommon.TraceLog.AddErrHandler()

            ' 戻り値
            Dim ret As Integer

            Dim ELog As New CASTCommon.ClsEventLOG
            If CmdArgs.Length = 0 Then
                ELog.Write("処理開始:引数なし PID:" & Process.GetCurrentProcess.Id)
            Else
                ELog.Write("処理開始:" & CmdArgs(0) & " PID:" & Process.GetCurrentProcess.Id)
            End If

            If CmdArgs.Length = 0 Then
                MainLOG.Write("開始 PID:" & Process.GetCurrentProcess.Id, "失敗", "引数なし")
                Return -100
            End If

            Console.WriteLine("処理開始")

            If CmdArgs.Length >= 2 Then
                CmdArgs(0) = String.Join(","c, CmdArgs).Replace(" "c, "")
            End If

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            ''* 二重起動を監視する ***
            ''1秒間隔、20分間まで二重起動監視する
            'Dim waitCnt As Integer = CASTCommon.ModPublic.WatchAndWaitProcess(1000, 1200)
            'If waitCnt > 0 Then
            '    ELog.Write("二重起動検知：" & CmdArgs(0) & "　" & waitCnt & "秒待機")
            'End If
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

            Select Case CmdArgs(0).Split(","c).Length
                Case 4
                    '一括返還処理はやらない
                    Dim KobetuClass As New ClsHenkan
                    'メイン処理
                    ret = KobetuClass.Main(CmdArgs(0))
                Case Else
                    MainLOG.Write("開始 PID:" & Process.GetCurrentProcess.Id, "失敗", "パラメタ取得失敗[" & CmdArgs(0) & "]")
                    ret = -100
            End Select

            Console.WriteLine("処理終了")
            ELog.Write("処理終了:" & CmdArgs(0) & " PID:" & Process.GetCurrentProcess.Id)
            Return ret
        Catch ex As Exception

            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write("0000000000-00", "00000000", "返還メイン処理", "失敗", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            CASTCommon.TraceLog.ELogWrite(ex, Diagnostics.EventLogEntryType.Error)
            
            Return -99
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "返還メイン処理(終了)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try
    End Function
End Module
