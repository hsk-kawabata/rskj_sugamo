Imports System
Imports System.IO
Imports System.Diagnostics

' 資金決済企業一覧表印刷 メインモジュール
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFKP005", "資金決済企業一覧表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public RecordCnt As Long = 0     '出力レコード数
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    Public txtKESSAI_DATE As String
    Public txtcommand As String
    Public Jikinko As String
    '2017/05/16 タスク）西野 ADD 標準版修正（決済予定日での出力対応）----------------- START
    Public PRINT_MODE As String = CASTCommon.GetRSKJIni("PRINT", "KFKP005_MODE")   '0:予定帳票、0以外:結果帳票
    '2017/05/16 タスク）西野 ADD 標準版修正（決済予定日での出力対応）----------------- END
    Private PrinterName As String = ""
    ' パブリックＤＢ
    Public MainDB As CASTCommon.MyOracle
    '
    ' 機能　 ： 資金決済企業一覧表印刷 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ, ARG2 - 決済日
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ：
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        ' 戻り値
        Dim ret As Integer
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "成功", "")

            ' 資金決済企業一覧表印刷処理
            Dim ELog As New CASTCommon.ClsEventLOG

            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write("パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Param() As String = CmdArgs(0).Split(",")
            If Param.Length <> 3 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            ELog.Write("処理開始")
            LW.UserID = Param(0)                         ' ログイン名
            txtKESSAI_DATE = Param(1)        ' 決済日
            txtcommand = Param(2)            ' 印刷区分
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))

            '---------------------------------------------------------
            ' 資金決済企業一覧表印刷処理
            '---------------------------------------------------------
            Dim Itiranhyo As New ClsPrnSikinKessai(txtcommand)
            Itiranhyo.CreateCsvFile()
            If Itiranhyo.MakeRecord = False Then
                If RecordCnt = -1 Then   '印刷対象なし
                    Return -1
                Else
                    Return -300 '処理中のエラー
                End If
            End If

            '印刷処理実行
            If Itiranhyo.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", Itiranhyo.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If
            Return 0

            ELog.Write("処理終了:" & CmdArgs(0))

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
        Finally
            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", RecordCnt & "件")
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
            End If
        End Try


        Return ret
    End Function

End Module
