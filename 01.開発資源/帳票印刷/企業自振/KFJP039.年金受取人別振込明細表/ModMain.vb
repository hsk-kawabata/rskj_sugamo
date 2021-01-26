Imports System
Imports System.IO
Imports System.Diagnostics

'=====================================================
'年金受取人別振込明細表　メインモジュール
'=====================================================
Module ModMain

    ' ログ処理クラス
    Public BatchLOG As New CASTCommon.BatchLOG("KFJP039", "年金受取人別振込明細表")

    '=======================================================================
    ' 機能　 ： 年金受取人別振込明細表　メイン処理
    ' 引数　 ： ARG1 - 起動パラメータ
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ' 備考　 ： 
    ' 作成日 ： 2009/10/02
    ' 更新日 ：
    '=======================================================================
    Function Main(ByVal arg() As String) As Integer
        Dim ret As Integer
        Dim NenUkeMei As New ClsNenUkeMei

        Try
            If arg.Length <> 1 Then
                BatchLOG.Write("(初期処理)", "失敗", "引数まちがい")
                Return 1
            End If

            '------------------------------------------
            '起動パラメータ取得／チェック
            '------------------------------------------
            Dim Param() As String = arg(0).Split(","c)

            If Param.Length = 3 Then
                BatchLOG.UserID = Param(0)                                          'ログイン名(ログ書込用)
                NenUkeMei.ToriSCode = Param(1).Substring(0, 10)                     '取引先主コード
                NenUkeMei.ToriFCode = Param(1).Substring(10, 2)                     '取引先副コード
                BatchLOG.ToriCode = NenUkeMei.ToriSCode & "-" & NenUkeMei.ToriFCode '取引先コード(ログ書込用)
                NenUkeMei.FuriDate = Param(2)                                       '振替日
                BatchLOG.FuriDate = Param(2)                                        '振替日(ログ書込用)
                BatchLOG.Write("(初期処理)", "成功", "パラメータ:" & arg(0))
            Else
                BatchLOG.Write("(初期処理)", "失敗", "パラメータが不正です。" & Param.ToString)
            End If

            '------------------------------------------
            'メイン処理
            '------------------------------------------
            ret = NenUkeMei.Main()
            Return ret

        Catch ex As Exception
            BatchLOG.Write("パラメータ取得", "失敗", ex.Message)
            Return -1

        End Try
    End Function

End Module
