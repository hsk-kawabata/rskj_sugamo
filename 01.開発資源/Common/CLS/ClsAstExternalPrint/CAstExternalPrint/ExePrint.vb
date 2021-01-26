' 2016/01/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) --------------------START
Public Class ExePrint

    ' 機能   ： 業務固有のCSVファイルを作成し拡張印刷を実行する
    ' 引数   ： prtID  帳票ID
    '           prtName  帳票名
    '           replaceArray  置換文字列配列（画面入力コードなど、SQL文の置換文字に対応する配列）
    '           printerArray  プリンタ名配列（通常使うプリンタのみの場合はNothing、配列中の通常使うプリンタ名は空文字）
    '           sql  印刷用SQL文（置換え後のSQL文。ALL9指定の場合はALL9指定用SQL文の置換え後）
    '           isAll9  ALL9指定か否か（True: ALL9）
    ' 戻り値 ： 印刷したレコード数（該当レコードがない場合は、0）
    '           異常時は、-1
    '
    Public Function Main(ByVal prtID As String, ByVal prtName As String, _
                               ByVal replaceArray As String(), ByVal printerArray As String(), _
                               ByVal isALL9 As Boolean) As Integer

        Dim ret As Integer = -1
        Dim nRet As Integer = 0
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Dim LOG As CASTCommon.BatchLOG = New CASTCommon.BatchLOG(prtID, prtName)

        Try
            ' 処理開始ログ出力
            sw = LOG.Write_Enter1("業務固有印刷処理")

            ' 任意の印刷名
            Dim prtDspName As String = prtID & "_" & prtName

            ' レポエージェント印刷
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim Param As String = String.Join(",", replaceArray)

            '[prtID].EXEを起動して印刷処理を実施する
            nRet = ExeRepo.ExecReport(prtID & ".EXE", Param)

            Select Case prtID
                Case "KFJP018", "KFJP017", "KFJP052", "KFJP019"
                    If nRet <> 0 Then
                        ret = -1    ' 異常終了： -1
                    Else
                        ret = 1     ' 正常終了
                    End If
                Case Else
                    Select Case nRet
                        Case 0
                            ret = 1     ' 正常終了
                        Case -1
                            ret = 0     ' 印刷対象なし
                        Case Else
                            ret = -1    ' 異常終了： -1
                    End Select
            End Select

            Return ret

        Catch ex As Exception
            LOG.Write_Err("業務固有印刷処理", ex)
            Return ret   ' 異常終了： -1

        Finally
            LOG.Write_Exit1(sw, "業務固有印刷処理", "復帰値=" & ret)
        End Try
    End Function
End Class
' 2016/01/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) --------------------END
