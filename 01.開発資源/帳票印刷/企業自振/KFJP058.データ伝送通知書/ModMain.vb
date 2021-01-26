Imports System
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFJP058", "データ伝送通知書")
    Public Structure LogWrite
        Dim UserID As String            ' ユーザID
        Dim ToriCode As String          ' 取引先主副コード
        Dim FuriDate As String          ' 振替日
    End Structure

    Public LW As LogWrite
    Public TORIS_CODE As String         ' 取引先主コード
    Public TORIF_CODE As String         ' 取引先副コード
    Public FURI_DATE As String          ' 振替日
    Public RecordCnt As Long = 0        ' 出力レコード数
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    '
    ' 機能　 ：データ伝送通知書 メイン処理
    '
    ' 引数　 ：ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0    － 正常
    '           -100 － 異常(パラメータなし)
    '           -200 － 異常(CSVファイル存在なし)
    '           -300 － 異常(処理中のエラー)
    '          -1    － 正常(印刷対象なし)
    '           他   － 異常(RepoAgentの戻り値)
    ' 備考　 ：
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       ' プリンタ名
        Dim LoginID As String = ""      ' ログイン名
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "開始", "成功")
            PrinterName = ""            '通常使うプリンタ

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                BatchLog.Write("開始", "失敗", "引数不足")
                Return 1
            End If

            If CmdArgs(0).Split(","c).Length <> 2 And CmdArgs(0).Split(","c).Length <> 3 Then
                BatchLog.Write("開始", "失敗", "引数まちがい")
                Return 1
            End If

            Dim Cmd() As String = CmdArgs(0).Split(","c)

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("処理開始")
            BatchLog.Write("処理開始", "成功", CmdArgs(0))
            Try
                ' 起動パラメータ格納
                TORIS_CODE = Cmd(0).PadRight(10).Substring(0, 10)
                TORIF_CODE = Cmd(0).PadRight(2).Substring(10)
                FURI_DATE = Cmd(1)
                If Cmd.Length = 3 Then
                    PrinterName = Cmd(2)
                End If
                BatchLog.ToriCode = Cmd(0)
                BatchLog.FuriDate = Cmd(1)
                BatchLog.Write("パラメータ取得", "成功")
            Catch ex As Exception
                BatchLog.Write("パラメータ取得", "失敗", ex.Message)
                Return -1
            End Try

            '---------------------------------------------------------
            ' データ伝送通知書印刷処理
            '---------------------------------------------------------
            Dim PrnClass As New KFJP058()
            PrnClass.CreateCsvFile()
            If PrnClass.MakeRecord = False Then
                If RecordCnt = -1 Then      '印刷対象なし
                    Return -1
                Else
                    Return -300             '処理中のエラー
                End If
            End If

            '印刷処理実行
            If PrnClass.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", PrnClass.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If
            Return 0
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
        End Try

    End Function

End Module
