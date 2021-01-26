Imports System
Imports System.IO
Imports System.Diagnostics
' 預金口座振替変更通知書印刷メインモジュール
Module ModMain

    ' ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFJP012", "預金口座振替変更通知書印刷バッチ")
    '
    ' 機能　 ： 預金口座振替変更通知書印刷 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ：
    '
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Function Main(ByVal arg() As String) As Integer
        Dim KFJP012 As New ClsHenkouTuuti    ' クラス宣言
        Dim nRtn As Integer    ' 復帰値
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "成功", "")

            If arg.Length <> 1 Then
                MainLOG.Write("開始", "失敗", "パラメータなし")
                Return -100
            End If

            Console.WriteLine("処理開始")
            Try
                ' 起動パラメータ格納
                Dim Param() As String = arg(0).Split(","c)
                LW.UserID = Param(0)
                KFJP012.FURI_DATE = Param(1)
                If Param.Length = 2 Then
                End If
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数:" & arg(0))
            Catch ex As Exception
                MainLOG.Write("パラメータ取得", "失敗", "コマンドライン引数:" & arg(0))
                Return -100
            End Try

            ' 帳票印刷メイン処理
            nRtn = KFJP012.PrintHenkouTuuti()
            If nRtn = 0 Then
                MainLOG.Write("処理終了", "成功")
            Else
                MainLOG.Write("処理終了", "失敗", "復帰値:" & nRtn)
            End If
            Return nRtn
        Catch ex As Exception
            MainLOG.Write("例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
        End Try
    End Function
End Module
