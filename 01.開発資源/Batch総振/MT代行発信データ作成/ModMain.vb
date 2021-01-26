''' <summary>
''' MT代行発信データ作成　メインモジュール
''' </summary>
''' <remarks>2016/10/12 saitou added for RSV2対応</remarks>
Module ModMain

    ''' <summary>
    ''' メイン処理
    ''' </summary>
    ''' <param name="CmdArgs">コマンドライン引数</param>
    ''' <returns>0 - 正常 , 0 以外 - 異常</returns>
    ''' <remarks></remarks>
    Public Function Main(ByVal CmdArgs() As String) As Integer
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG
        Dim DensouDataCreateClass As New ClsDensouDataCreate

        Try
            ELog.Write("開始")

            '初期処理
            If DensouDataCreateClass.Init(CmdArgs) = False Then
                Return -1
            End If

            '主処理
            ret = DensouDataCreateClass.Main()
            'フラグ取消
            DensouDataCreateClass.ReturnFlg()
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
