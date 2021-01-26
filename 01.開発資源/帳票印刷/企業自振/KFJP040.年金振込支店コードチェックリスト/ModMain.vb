Imports System
Imports System.IO
Imports System.Diagnostics

'=====================================================
'年金振込支店コードチェックリスト　メインモジュール
'=====================================================
Module ModMain

    ' ログ処理クラス
    Public BatchLOG As New CASTCommon.BatchLOG("KFJP040", "年金振込支店コードチェックリスト")

    '=======================================================================
    ' 機能　 ： 年金振込支店コードチェックリスト　メイン処理
    ' 引数　 ： ARG1 - 起動パラメータ
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ' 備考　 ： 
    ' 作成日 ： 2009/09/29
    ' 更新日 ：
    '=======================================================================
    Function Main(ByVal arg() As String) As Integer
        Dim ret As Integer
        Dim NenSitCheck As New ClsNenSitCheck

        Try
            If arg.Length <> 1 Then
                BatchLOG.Write("(初期処理)", "失敗", "引数まちがい")
                Return 1
            End If

            '------------------------------------------
            '起動パラメータ取得／チェック
            '------------------------------------------
            Dim Param() As String = arg(0).Split(","c)

            If Param.Length = 2 Then
                BatchLOG.UserID = Param(0)              'ログイン名(ログ書込用)
                BatchLOG.FuriDate = Param(1)            '振替日(ログ書込用)
                NenSitCheck.FuriDate = Param(1)         '振替日
                BatchLOG.Write("(初期処理)", "成功", "パラメータ:" & arg(0))
            Else
                BatchLOG.Write("(初期処理)", "失敗", "パラメータが不正です。" & Param.ToString)
            End If

            '------------------------------------------
            'メイン処理
            '------------------------------------------
            ret = NenSitCheck.Main()
            Return ret

        Catch ex As Exception
            BatchLOG.Write("パラメータ取得", "失敗", ex.Message)
            Return -1

        End Try
    End Function

End Module
