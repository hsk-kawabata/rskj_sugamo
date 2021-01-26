Imports System.IO


Module ModMain

    Private Const KekkaOKCode As Integer = 0
    Private Const KekkaNGCode As Integer = -1

    ' ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFJP050", "口座振替処理結果件数表")

    Function Main(ByVal CmdArgs() As String) As Integer

        Dim ret As Integer = KekkaNGCode

        Try
            ' 口座振替処理結果件数表出力処理
            Dim Ryousyusyosyo As New ClsSyorikekkakensuhyou

            Dim ELog As New CASTCommon.ClsEventLOG

            '引数判定
            If CmdArgs.Length <> 1 Then
                MainLOG.Write("処理開始", "失敗", "引数まちがい")

                Exit Try
            Else
                MainLOG.Write("処理開始", "成功", CmdArgs(0))
            End If

            '帳票印刷処理
            If Not Ryousyusyosyo.Main(CmdArgs(0)) Then

                Exit Try
            End If

            ret = KekkaOKCode

        Catch ex As Exception
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace)
            ret = KekkaNGCode
        Finally
            MainLOG.Write("処理終了", "成功")
        End Try

        Return ret

    End Function


End Module
