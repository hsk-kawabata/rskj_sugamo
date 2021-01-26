Option Strict On
Option Explicit On

Imports System

' 為替振込データ作成処理 メインモジュール
Module ModMain
    '
    ' 機能　 ： 為替振込データ作成 メイン処理
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
        Dim FurikomiDataCreateClass As New ClsFurikomiDataCreate

        Try
            ELog.Write("開始")

            '初期処理
            If FurikomiDataCreateClass.Init(CmdArgs) = False Then
                Return -1
            End If

            ' 主処理
            ret = FurikomiDataCreateClass.Main(CmdArgs(0))
            'フラグ取消
            FurikomiDataCreateClass.ReturnFlg()
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
