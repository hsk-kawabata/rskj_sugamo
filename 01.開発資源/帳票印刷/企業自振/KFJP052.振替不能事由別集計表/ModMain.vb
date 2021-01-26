Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' 振替不能事由別集計表処理 メインモジュール
Module ModMain

    ' ログ処理クラス
    Private MainLOG As New CASTCommon.BatchLOG("KFJP052", "振替不能事由別集計表")

    '
    ' 機能　 ： 振替不能事由別集計表 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ：
    '
    Function Main(ByVal arg() As String) As Integer

        Try
            ' 振替不能事由別集計表出力処理
            Dim FurikaeFunouSyuukei As New ClsFurikaeFnouSyuukei

            ' 戻り値
            Dim ret As Integer

            If arg.Length = 0 Then
                MainLOG.Write("開始", "失敗", "引数不足")
                Return 1
            End If

            '2012/06/30 標準版　WEB伝送対応----------------------------------------------->
            If arg(0).Split(","c).Length <> 2 And arg(0).Split(","c).Length <> 3 And arg(0).Split(","c).Length <> 4 Then
                'If arg(0).Split(","c).Length <> 2 And arg(0).Split(","c).Length <> 3 Then
                '-------------------------------------------------------------------------<
                MainLOG.Write("開始", "失敗", "引数まちがい")
                Return 1
            End If

            Dim Cmd() As String = arg(0).Split(","c)

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("処理開始")
            MainLOG.Write("処理開始", "成功", arg(0))

            Try
                ' 起動パラメータ格納
                FurikaeFunouSyuukei.TORIS_CODE = Cmd(0).PadRight(10).Substring(0, 10)
                FurikaeFunouSyuukei.TORIF_CODE = Cmd(0).PadRight(2).Substring(10)
                FurikaeFunouSyuukei.FURI_DATE = Cmd(1)
                If Cmd.Length = 3 Then
                    FurikaeFunouSyuukei.PRINTERNAME = Cmd(2)
                End If
                '2012/06/30 標準版　WEB伝送対応----------->
                If Cmd.Length = 4 Then
                    FurikaeFunouSyuukei.INVOKE_KBN = Cmd(3)
                End If
                '-----------------------------------------<
                MainLOG.ToriCode = Cmd(0)
                MainLOG.FuriDate = Cmd(1)
                MainLOG.Write("パラメータ取得", "成功", arg(0))
            Catch ex As Exception
                MainLOG.Write("パラメータ取得", "失敗", ex.Message)
                Return -1
            End Try

            ' メイン処理
            ret = FurikaeFunouSyuukei.Main()
            MainLOG.Write("STEP出力", "", "ret = Frikaekekkamei...") '2009/04/10
            ELog.Write("処理終了")
            MainLOG.Write("処理終了", "成功")

            Return ret

        Catch ex As Exception
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace)
            Return -1
        End Try
    End Function

End Module
