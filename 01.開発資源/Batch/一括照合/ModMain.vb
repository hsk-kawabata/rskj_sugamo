Imports System

''' <summary>
''' 一括照合　メインモジュール
''' </summary>
''' <remarks>2017/11/30 saitou 広島信金(RSV2標準版) added for 大規模構築対応</remarks>
Module ModMain

#Region "モジュール変数"
    Public BatchLog As New CASTCommon.BatchLOG("KFJ110", "一括照合")

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 一括照合処理のメイン処理を行います。
    ''' </summary>
    ''' <param name="CmdArgs">コマンドライン引数</param>
    ''' <returns>0 - 正常 , 0以外 - 異常</returns>
    ''' <remarks></remarks>
    Public Function Main(CmdArgs() As String) As Integer
        Dim ret As Integer
        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Dim Syougou As New ClsSyougou

        Try
            sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "(メイン処理)開始", "成功")

            '----------------------------------------
            '初期処理
            '----------------------------------------
            If Syougou.Init(CmdArgs) = False Then
                Return -1
            End If

            '----------------------------------------
            'メイン処理
            '----------------------------------------
            ret = Syougou.Main()

            Return ret

        Catch ex As Exception
            BatchLog.Write("0000000000-00", "00000000", "(メイン処理)", "失敗", ex.ToString)
            Return -99

        Finally
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "(メイン処理)終了", "成功")
        End Try

    End Function

#End Region

End Module
