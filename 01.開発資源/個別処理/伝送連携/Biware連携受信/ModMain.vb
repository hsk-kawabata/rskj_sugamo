Module ModMain

    Public ELog As New CASTCommon.ClsEventLOG

    Public Function Main(ByVal Args() As String) As Integer

        Dim ret As Integer = -1

        ELog.Write("開始")

        '伝送連携受信メイン処理
        Dim BiwareRev As New ClsBiwareReceive

        Try
            If Args.Length <> 1 Then
                '引数不一致
                ELog.Write("コマンドライン引数のパラメータが不正です", EventLogEntryType.Error)
                Exit Try
            End If

            If Not BiwareRev.Init(Args) Then
                ELog.Write("初期化処理 失敗 " & Args(0), EventLogEntryType.Error)
                Exit Try
            End If

            If BiwareRev.RevMain = True Then
                '正常終了
                ret = 0
            Else
                ELog.Write("メイン処理 失敗")
            End If

        Catch ex As Exception
            ELog.Write(ex.Message, EventLogEntryType.Error)
        Finally
            ELog.Write("終了")
        End Try

        Return ret

    End Function

End Module
