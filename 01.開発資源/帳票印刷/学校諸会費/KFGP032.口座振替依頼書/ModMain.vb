Imports System
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFGP032", "口座振替依頼書")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim GakkouCode As String        '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public GakkouCode As String = ""    '取引先主コード
    Public FuriDate As String = ""      '振替日
    Public NSKbn As String = ""         '入出金区分
    Public RecordCnt As Long = 0        '出力レコード数
    Public AllRecordCnt As Long = 0     '全出力レコード数
    Public TaisyouGakunen As String     '2010/10/12.Sakon　対象学年
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    Public Ret As Integer

    '
    ' 機能　 ：口座振替依頼書印刷 メイン処理
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
        Dim PrinterName As String       'プリンタ名
        Dim LoginID As String = ""      'ログイン名
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.GakkouCode = "000000000000"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "(印刷処理)開始", "成功", "")
            PrinterName = ""    '通常使うプリンタ

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 4 Then '2010/10/12.Sakon　パラメータに対象学年追加
                'If Cmd.Length <> 3 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)                  'ログイン名取得
            GakkouCode = Mid(Cmd(1), 1, 10)     '学校コード
            NSKbn = Cmd(2)                      '入出金区分
            TaisyouGakunen = Cmd(3)             '2010/10/12.Sakon　対象学年追加

            LW.GakkouCode = GakkouCode
            LW.FuriDate = FuriDate
            BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))

            '---------------------------------------------------------
            ' 口座振替依頼書印刷処理
            '---------------------------------------------------------
            Dim Iraisyo As New KFGP032()
            Iraisyo.CreateCsvFile()
            If Iraisyo.MakeRecord = False Then
                If RecordCnt = -1 Then   '印刷対象なし
                    Return -1
                Else
                    Return -300         '処理中のエラー
                End If
            End If

            '印刷処理実行
            If Iraisyo.G_ReportExecute("") = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "RepoAgent印刷", "失敗", Iraisyo.ReportMessage)
                Ret = -999     'レポエージェントからの戻り値を返す(暫定-999)
                Return False
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "例外", "失敗", ex.Message)
        Finally
            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "(印刷処理)終了", "成功", AllRecordCnt & "件")
            Else
                BatchLog.Write(LW.UserID, LW.GakkouCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
            End If
        End Try

    End Function

End Module
