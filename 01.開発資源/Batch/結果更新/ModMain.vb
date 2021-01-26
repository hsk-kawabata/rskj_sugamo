Imports System
Imports System.IO

' 結果更新 メインモジュール
Module ModMain
    ' ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFJ040", "不能結果更新バッチ")
    Public Const msgTitle As String = "不能結果更新バッチ(KFJ040)"
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    '
    ' 機能　 ： 結果更新 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    Function Main(ByVal CmdArgs() As String) As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1(LW.UserID, LW.ToriCode, LW.FuriDate, "結果更新(開始)", "成功")
            'MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "開始", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            

            ' 戻り値
            Dim ret As Integer

            Dim ELog As New CASTCommon.ClsEventLOG
            If CmdArgs.Length = 0 Then
                ELog.Write("処理開始:引数なし")
            Else
                ELog.Write("処理開始:" & CmdArgs(0))
            End If

            Console.WriteLine("処理開始")

            If CmdArgs.Length = 0 Then
                MainLOG.Write("開始", "失敗", "パラメタ取得失敗[" & CmdArgs(0) & "]")
                Return -100
            End If

            If CmdArgs.Length >= 2 Then
                CmdArgs(0) = String.Join("", CmdArgs).Replace(" "c, "")
            End If

            Dim KekkaClass As New ClsKekka

            ret = KekkaClass.Main(CmdArgs(0))

            Console.WriteLine("処理終了")
            ELog.Write("処理終了:" & CmdArgs(0))

            Return ret
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, LW.UserID, LW.UserID, LW.FuriDate, "結果更新(終了)", "成功")
            'MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "終了", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        End Try

    End Function

End Module
