Module ModMain

    Public ELog As New CASTCommon.ClsEventLOG

    Public Function Main(ByVal Args() As String) As Integer

        Dim ret As Integer = -1

        ELog.Write("開始")

        '伝送連携受信個別メイン処理
        Dim DensouRev As New ClsDensouReceive

        Try
            If Args.Length <> 1 Then
                '引数不一致
                ELog.Write("コマンドライン引数のパラメータが不正です", EventLogEntryType.Error)
                Exit Try
            End If

            If Not DensouRev.Init(Args) Then
                ELog.Write("初期化処理 失敗 " & Args(0), EventLogEntryType.Error)
                Exit Try
            End If

            If DensouRev.RevMain = True Then
                '正常終了
                ret = 0
            Else
                ' 2017/03/22 タスク）綾部 CHG 【ME】(ジョブメッセージ考慮不足修正) -------------------- START
                'ELog.Write("メイン処理 失敗", EventLogEntryType.Error)
                ELog.Write("メイン処理 失敗")
                ' 2017/03/22 タスク）綾部 CHG 【ME】(ジョブメッセージ考慮不足修正) -------------------- END
            End If

        Catch ex As Exception
            ELog.Write(ex.Message, EventLogEntryType.Error)
        Finally
            ELog.Write("終了")
        End Try

        Return ret

    End Function

End Module
