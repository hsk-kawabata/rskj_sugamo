''' <summary>
''' 運用試験_不能結果自動　メインモジュール
''' </summary>
''' <remarks>2017/02/14 綾部 ADD FOR RSV2対応</remarks>
Module ModMain

    ''' <summary>
    ''' メイン処理
    ''' </summary>
    ''' <returns>0 - 正常 , 0 以外 - 異常</returns>
    ''' <remarks></remarks>
    Public Function Main() As Integer

        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG
        Dim FunouAutoClass As New ClsFunouAuto

        Try
            ELog.Write("運用試験_不能結果自動 開始 (受信日:" & Now.ToString("yyyyMMdd") & " / 開始時間:" & Now.ToString("HHmmss") & ")")

            '主処理
            ret = FunouAutoClass.Main()

            If ret <> 0 Then
                Return -1
            End If

        Catch ex As Exception
            ELog.Write(ex.Message)
            Return -1
        Finally
            ELog.Write("運用試験_不能結果自動 終了")
        End Try

        Return ret
    End Function

End Module
