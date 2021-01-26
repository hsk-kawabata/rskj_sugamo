Option Strict On
Option Explicit On

Imports System
Imports System.IO

' 手数料計算 メインモジュール
Module ModMain
    ' ログ処理クラス
    '       2009.08.27 コンパイルを通すためコメント
    '       Private MainLOG As New CASTCommon.BatchLOG("手数料計算", "TESUU")
    Private MainLOG As New CASTCommon.BatchLOG("手数料計算")
    '
    ' 機能　 ： 手数料計算 メイン処理
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    Function Main(ByVal cmdArgs() As String) As Integer
        ' 戻り値
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "手数料計算(開始)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            ELog.Write("処理開始 " & cmdArgs(0))


            Dim FuriDate As String              ' 振替日
            Dim ReCalcFlag As Integer = 0       ' 再計算フラグ
            Dim JobTuuban As Integer = 0        ' ジョブ通番

            If cmdArgs.Length = 1 Then
                Dim arr() As String = cmdArgs(0).Split(","c)
                '*** 修正 mitsu 2008/09/17 パラメータパターン追加 ***
                'If arr.Length = 2 Then
                '不能結果更新から呼び出される場合 
                If arr.Length = 1 Then
                    FuriDate = arr(0)
                    JobTuuban = 0
                    ReCalcFlag = 0
                ElseIf arr.Length = 2 Then
                    '************************************************
                    FuriDate = arr(0)
                    JobTuuban = CASTCommon.CAInt32(arr(1))
                    ReCalcFlag = 0
                ElseIf arr.Length = 3 Then
                    FuriDate = arr(0)
                    ReCalcFlag = CASTCommon.CAInt32(arr(1))
                    JobTuuban = CASTCommon.CAInt32(arr(2))
                Else
                    ELog.Write("処理開始:引数なし PIG:" & Process.GetCurrentProcess.Id)
                    Return 1
                End If
            Else
                ELog.Write("処理開始:引数なし PIG:" & Process.GetCurrentProcess.Id)
                Return 1
            End If

            ' 手数料計算処理
            Dim TesuuCalcClass As New ClsTesuu

            ' メイン処理
            ret = TesuuCalcClass.Main(FuriDate, ReCalcFlag, JobTuuban)
            ELog.Write("処理終了 " & cmdArgs(0))

            Return ret
        Catch ex As Exception
            ELog.Write("手数料計算に失敗しました。", EventLogEntryType.Warning)
            MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "手数料計算", "失敗", ex.Message)
            Return 1
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "手数料計算(終了)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try
    End Function
End Module
