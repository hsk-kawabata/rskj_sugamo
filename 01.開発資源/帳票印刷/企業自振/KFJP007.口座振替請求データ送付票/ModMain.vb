Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' 口座振替請求データ送付票バッチ メインモジュール
Module ModMain

    ' ログ処理クラス
    '(1)ログ初期化、(2)ログ書込関数を呼び出す。
    Private MainLOG As New CASTCommon.BatchLOG("KFJP007", "口座振替請求データ送付票")

    '
    ' 機能　 ： 口座振替請求データ送付票 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ：
    '
    Function Main(ByVal arg() As String) As Integer
        Try
            ' 戻り値
            Dim ret As Integer = 0
            Dim KouzafurikaeSeikyuDataInvoice As New ClsKouzafurikaeSeikyuDataInvoice

            '受け取った値のパラメータ数をチェックする
            If arg.Length = 0 Then
               MainLOG.Write("開始", "失敗", "引数不足")
                Return 1
            End If

            If arg(0).Split(","c).Length <> 3 Then
                MainLOG.Write("開始", "失敗", "引数まちがい " & arg(0))
                Return 1
            End If

            Dim Param() As String = arg(0).Split(","c)

            Try
                If Param.Length = 3 Then
                    KouzafurikaeSeikyuDataInvoice.TORIS_CODE = Param(0).Substring(0, 10)
                    KouzafurikaeSeikyuDataInvoice.TORIF_CODE = Param(0).Substring(10, 2)
                    KouzafurikaeSeikyuDataInvoice.FURI_DATE = Param(1)
                    KouzafurikaeSeikyuDataInvoice.KEIYAKU_KIN = Param(2)

                    MainLOG.ToriCode = Param(0)
                    MainLOG.Write("パラメータ取得", "成功", "コマンドライン引数:" & arg(0))
                End If
            Catch ex As Exception
                MainLOG.Write("パラメータ取得", "失敗", ex.Message)
                Return -1
            End Try

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("処理開始")
            MainLOG.Write("処理開始", "成功", arg(0))

            'メイン処理 
            ret = KouzafurikaeSeikyuDataInvoice.Main
            MainLOG.Write("STEP出力", "", "ret = KouzafurikaeSeikyuDataInvoice.Ma...")

            If ret = 0 Then
                ELog.Write("処理成功")
                MainLOG.Write("口座振替請求データ送付票", "成功")
            Else
                ELog.Write("処理失敗")
                MainLOG.Write("口座振替請求データ送付票", "失敗")
            End If

            Return ret

        Catch ex As Exception
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace)
            Return -1
        End Try
    End Function
End Module
