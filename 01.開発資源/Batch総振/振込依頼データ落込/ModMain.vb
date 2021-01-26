Option Strict On
Option Explicit On

Imports System

' 落し込み処理 メインモジュール
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFS010", "振込依頼データ落込処理")
    Public Const msgTitle As String = "振込依頼データ落込処理(KFS010)"

    '=================================================================
    '機能　：落し込み メイン処理
    '引数　：ARG1 - 起動パラメータ
    '戻り値：0 - 正常 ， 0 以外 - 異常
    '備考　：
    '作成　：2009/09/07
    '更新　：
    '=================================================================
    Function Main(ByVal CmdArgs() As String) As Integer
        ' 戻り値
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "(メイン処理)開始", "成功")
            'BatchLog.Write("0000000000-00", "00000000", "(メイン処理)開始", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            '------------------------------------------------------------
            'パラメタチェック
            '------------------------------------------------------------
            If CmdArgs.Length = 0 Then
                ELog.Write("処理開始:引数なし")
            Else
                ELog.Write("処理開始:" & CmdArgs(0))
            End If

            Console.WriteLine("処理開始")

            If CmdArgs.Length = 0 Then
                Return -100
            End If

            Select Case CmdArgs(0).Split(","c).Length
                Case 7, 8, 9, 13
                Case Else
                    'パラメータ異常
                    ret = -100
            End Select

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            ''------------------------------------------------------------
            ''1秒間隔、10分間まで二重起動監視する
            ''------------------------------------------------------------
            'CASTCommon.ModPublic.WatchAndWaitProcess(1000, 600)
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

            '------------------------------------------------------------
            '落込主処理実行
            '------------------------------------------------------------
            Dim TourokuClass As New ClsTouroku

            ret = TourokuClass.Main(CmdArgs(0))
            Console.WriteLine("処理終了")
            ELog.Write("処理終了:" & CmdArgs(0))

            Return ret

        Catch ex As Exception
            BatchLog.Write("", "0000000000-00", "00000000", "(メイン処理)", "失敗", ex.Message)
            Return -999
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "(メイン処理)終了", "成功")
            'BatchLog.Write("", "0000000000-00", "00000000", "(メイン処理)終了", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try
    End Function



End Module
