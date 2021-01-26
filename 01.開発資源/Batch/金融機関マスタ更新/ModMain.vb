Imports System
Imports System.IO

' 金融機関マスタ更新 メインモジュール
Module ModMain
    '
    ' 機能　 ： 金融機関マスタ更新 メイン処理
    '
    ' 引数　 ： ARG1 - ファイル名（パス付き）
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        ' 戻り値
        Dim ret As Integer

        Dim ELog As New CASTCommon.ClsEventLOG
        If CmdArgs.Length = 0 Then
            ELog.Write("処理開始:引数なし")
        Else
            ELog.Write("処理開始:" & CmdArgs(0))
        End If

        If CmdArgs.Length = 0 Then
            ELog.Write("処理失敗：引数なし", Diagnostics.EventLogEntryType.Error)
            Return -100
        End If

        If CmdArgs(0).Split(","c).Length = 3 Then
            CmdArgs(0) = String.Join("", CmdArgs).Replace(" "c, "")
        Else
            '引数違い
            Return -1
        End If


        ' 金融機関マスタ更新
        Dim TenmastKousinClass As New ClsKousin

        ' メイン処理
        ret = TenmastKousinClass.Main(CmdArgs(0))


        If ret = -100 Then
            ELog.Write("処理失敗：パラメタ取得失敗[" & CmdArgs(0) & "]", Diagnostics.EventLogEntryType.Error)
        Else
            ELog.Write("処理終了:" & CmdArgs(0))
        End If

        Return ret
    End Function

End Module
