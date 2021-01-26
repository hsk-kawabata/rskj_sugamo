Imports System
Imports System.IO

' 資金決済結果更新 メインモジュール
Module ModMain

    Public MainLOG As New CASTCommon.BatchLOG("KFK020", "資金決済結果更新")
    '
    ' 機能　 ： 資金決済結果更新 メイン処理
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    Function Main(ByVal cmdArgs() As String) As Integer
        ' 戻り値
        Dim ret As Integer

        Dim ELog As New CASTCommon.ClsEventLOG
        If cmdArgs.Length = 0 Then
            ELog.Write("処理開始:引数なし")
            MainLOG.Write("開始", "失敗", "引数なし")
            Return -100
        End If

        ELog.Write("処理開始:" & cmdArgs(0))
        
        ' 資金決済結果更新処理
        Dim KessaiKekkaClass As New ClsKessaiKekka

        ' メイン処理
        ret = KessaiKekkaClass.Main(cmdArgs(0))

        ELog.Write("処理終了 " & cmdArgs(0))

        Return ret
    End Function

End Module
