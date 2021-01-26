Imports System
Imports System.IO
Imports System.Diagnostics

' 口座振替明細表 メインモジュール
Module ModMain

    ' ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFJP010", "口座振替明細表")

    '
    ' 機能　 ： 口座振替明細表 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ：
    '
    Function Main(ByVal arg() As String) As Integer
        ' 戻り値
        Dim ret As Integer

        Dim Kouzafurimei As New ClsKouzafurimei

        Try
            MainLOG.Write("(初期処理)開始", "成功")

            If arg.Length <> 1 Then
                MainLOG.Write("(初期処理)", "失敗", "引数まちがい")
                Return 1
            End If

            ' 起動パラメータ格納
            Dim Param() As String = arg(0).Split(","c)
            If Param.Length = 3 Then
                Kouzafurimei.TORI_CODE = Param(1)
                Kouzafurimei.HAISINTIME = Param(2)                ' 配信タイムスタンプ
            ElseIf Param.Length = 4 Then
                Kouzafurimei.TORI_CODE = Param(1)               ' 取引先コード
                Kouzafurimei.HAISINTIME = Param(2)                ' 配信タイムスタンプ
                Kouzafurimei.PRINTERNAME = Param(3)          ' 出力プリンタ
            Else
                MainLOG.Write("(初期処理)", "失敗", "パラメータが不正です。" & arg(0))
                Return -100
            End If
            MainLOG.ToriCode = Kouzafurimei.TORI_CODE
            MainLOG.FuriDate = Kouzafurimei.HAISINTIME
            MainLOG.Write("パラメータ取得", "成功", "コマンドライン引数:" & arg(0))
        Catch ex As Exception
            MainLOG.Write("パラメータ取得", "失敗", ex.Message)
            Return -100
        End Try

        Dim ELog As New CASTCommon.ClsEventLOG
        ELog.Write("処理開始")

        ' メイン処理
        ret = Kouzafurimei.Main()
        ELog.Write("処理終了")

        Return ret
    End Function

End Module
