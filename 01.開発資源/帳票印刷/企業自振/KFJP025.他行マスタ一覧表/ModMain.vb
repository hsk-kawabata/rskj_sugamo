Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' 他行マスタ一覧表処理 メインモジュール
Module ModMain

    ' ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFJP025", "他行マスタ一覧表")

    '
    ' 機能　 ： 他行マスタ一覧表 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ：
    '
    Function Main(ByVal arg() As String) As Integer
        Try
            ' 他行マスタ一覧表出力処理
            Dim TAKOUMAST_LIST As New ClsTAKOUMAST_LIST

            ' 戻り値
            Dim ret As Integer

            If arg.Length = 0 Then
                MainLOG.Write("開始", "失敗", "引数不足")
                Return 1
            End If

            If arg(0).Split(","c).Length <> 2 And arg(0).Split(","c).Length <> 3 Then
                MainLOG.Write("開始", "失敗", "引数まちがい")
                Return 1
            End If

            Dim Cmd() As String = arg(0).Split(","c)

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("処理開始")
            'MainLOG.Write("処理開始", "成功", arg(0))
            Try
                '起動パラメータ格納
                TAKOUMAST_LIST.TORIS_CODE = Cmd(0).PadRight(10).Substring(0, 10)
                TAKOUMAST_LIST.TORIF_CODE = Cmd(1).PadRight(2).Substring(0, 2)
                TAKOUMAST_LIST.ALL_PRINT = Cmd(2)

                MainLOG.ToriCode = Cmd(0) & Cmd(1)
                MainLOG.Write("パラメータ取得", "成功", arg(0))
            Catch ex As Exception
                MainLOG.Write("パラメータ取得", "失敗", ex.Message)
                Return -1
            End Try

            'メイン処理
            ret = TAKOUMAST_LIST.Main()
            ELog.Write("処理終了")
            MainLOG.Write("処理終了", "成功")

            Return ret

        Catch ex As Exception
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace)
            Return -1
        End Try
    End Function

End Module
