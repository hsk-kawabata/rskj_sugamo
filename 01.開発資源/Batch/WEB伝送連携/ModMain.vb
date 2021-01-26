Option Strict On
Option Explicit On

Imports System

' WEB伝送連携処理 メインモジュール
Module ModMain
    ' ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFW010", "WEB伝送連携")

    '
    ' 機能　 ： WEB伝送連携 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 1,06,00,000007
    '           
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            ' 戻り値
            Dim ret As Integer

            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "(WEB伝送メイン処理)開始", "成功")
            'MainLOG.Write("0000000000-00", "00000000", "(メイン処理)開始", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


            If CmdArgs.Length = 0 Then
                Return -100
            End If

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            ''------------------------------------------------------------
            ''1秒間隔、10分間まで二重起動監視する
            ''------------------------------------------------------------
            'CASTCommon.ModPublic.WatchAndWaitProcess(1000, 600)
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

            Select Case CmdArgs(0).Split(","c).Length
                Case 1, 2, 3            '2012/12/05 WEB伝送 UPD 件数と金額追加
                    ' 伝送連携
                    Dim DensouClass As New ClsDensouRenkei
                    Dim Param() As String = CmdArgs(0).Split(","c)

                    'ジョブ通番取得
                    If Param.Length = 2 Then
                        MainLOG.JobTuuban = CInt(Param(1))
                    Else
                        MainLOG.JobTuuban = 0
                    End If

                    '件数と金額取得
                    If Param.Length = 3 Then
                        DensouClass.END_KEN = Param(1)
                        DensouClass.END_KIN = Param(2)
                    Else
                        DensouClass.END_KEN = "0"
                        DensouClass.END_KIN = "0"
                    End If

                    DensouClass.JobTuuban = MainLOG.JobTuuban

                    ret = DensouClass.Main(Param(0))

                Case Else
                    ret = -100
            End Select

            Return ret

        Catch ex As Exception
            MainLOG.Write("", "0000000000-00", "00000000", "(メイン処理)", "失敗", ex.Message)

            Return -999
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "(WEB伝送メイン処理)終了", "成功")
            'MainLOG.Write("", "0000000000-00", "00000000", "(メイン処理)終了", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        End Try
    End Function

End Module
