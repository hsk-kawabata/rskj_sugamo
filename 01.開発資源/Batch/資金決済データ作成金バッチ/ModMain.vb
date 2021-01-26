' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
Imports System
Imports System.IO
Imports System.Diagnostics

' 資金決済データ作成処理 メインモジュール
Module ModMain
    ' ログ処理クラス



    '
    ' 機能　 ： 資金決済データ作成 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        ' 戻り値
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG
        Dim ClsKessaiDataCreateKinBatch As New ClsKessaiDataCreateKinBatch
        
        Try
            ELog.Write("開始")

            '初期処理
            If ClsKessaiDataCreateKinBatch.KessaiInit(CmdArgs) = False Then
                Return -1
            End If

            ' 主処理
            ret = ClsKessaiDataCreateKinBatch.Main(CmdArgs(0))
            'フラグ取消
            ClsKessaiDataCreateKinBatch.ReturnFlg()
            If ret <> 0 Then
                Return -1
            End If

        Catch ex As Exception
            ELog.Write(ex.Message)
            Return -1
        Finally
            ELog.Write("終了")
        End Try

        Return ret

    End Function

End Module
' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
