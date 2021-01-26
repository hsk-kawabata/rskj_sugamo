Imports System
Imports System.IO
Imports System.Diagnostics

Module ModMain

    Private BatchLog As New CASTCommon.BatchLOG("ExtendPrint", "拡張印刷")

    '
    ' 機能　 ：拡張印刷 メイン処理
    '
    ' 引数　 ：CmdArgs(0) - 拡張印刷パラメタファイルパス名
    '
    ' 戻り値 ： 0     － 印刷対象なし
    '          -1     － 印刷失敗
    '          その他 － 印刷ページ数
    '
    Function Main(ByVal CmdArgs() As String) As Integer

        Dim ret As Integer = -1   ' 復帰値

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = BatchLog.Write_Enter1("印刷メイン処理")

            ' パラメタ取得
            If CmdArgs.Length = 0 Then
                BatchLog.Write_Err("印刷メイン処理", "失敗", "コマンドライン引数なし")
                Return ret
            End If

            BatchLog.Write_LEVEL1("印刷メイン処理", "パラメタファイル：" & CmdArgs(0))

            If File.Exists(CmdArgs(0)) = False Then
                Throw New Exception("帳票印刷パラメタファイルが見つかりませんでした。ファイル=" & CmdArgs(0))
            End If

            ' 帳票ID、帳票名取得
            Dim prtID As String = CASTCommon.GetIniFileValue(CmdArgs(0), "COMMON", "PRTID")
            Dim prtName As String = CASTCommon.GetIniFileValue(CmdArgs(0), "COMMON", "PRTNAME")

            ' ALL9指定取得
            Dim strAll9 As String = CASTCommon.GetIniFileValue(CmdArgs(0), "COMMON", "ISALL9")
            Dim isAll9 As Boolean = (strAll9 = "True")

            ' 置換文字列配列取得
            Dim replaceArray As String()
            replaceArray = CASTCommon.GetIniFileValues(CmdArgs(0), "REPLACEARRAYS", "REPLACE")
            If Not replaceArray Is Nothing Then
                For i As Integer = 0 To replaceArray.Length - 1
                    ' 2016/01/27 SO)荒木 For IT_D-05_002 SQL置換え番号が跳び番の場合、SQL定義置換でエラーとなる
                    ' 置換文字がない場合は、Nothingを特定する値（制御文字付き）でiniファイルを作成
                    If replaceArray(i) = "Nothing" & Chr(1) & "Nothing" Then
                        replaceArray(i) = Nothing
                    End If
                Next
            End If

            ' プリンタ名配列取得
            Dim printerArray As String()
            printerArray = CASTCommon.GetIniFileValues(CmdArgs(0), "PRINTERARRAYS", "PRINTER")

            ' 業務固有拡張印刷クラス情報取得
            Dim dllName As String = CASTCommon.GetIniFileValue(CmdArgs(0), "EXTERNAL", "DLLNAME")
            Dim className As String = CASTCommon.GetIniFileValue(CmdArgs(0), "EXTERNAL", "CLASSNAME")
            Dim methodName As String = CASTCommon.GetIniFileValue(CmdArgs(0), "EXTERNAL", "METHODNAME")

            ' 印刷処理呼び出し
            Dim clsExPrt As New CAstExtendPrint.CExtendPrint()
            If dllName <> "err" Then
                ' 業務固有印刷あり
                ret = clsExPrt.ExtendPrint4Exe(prtID, prtName, replaceArray, printerArray, isAll9, _
                                           dllName, className, methodName)
            Else
                ' 業務固有印刷なし
                ret = clsExPrt.ExtendPrint4Exe(prtID, prtName, replaceArray, printerArray, isAll9)
            End If

            Return ret

        Catch ex As Exception
            BatchLog.Write_Err("印刷メイン処理", ex)
            Return ret

        Finally
            BatchLog.Write_Exit1(sw, "印刷メイン処理", "復帰値=" & ret)

        End Try

    End Function

End Module
