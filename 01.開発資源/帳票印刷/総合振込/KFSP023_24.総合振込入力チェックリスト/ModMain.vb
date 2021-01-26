Option Strict On
Option Explicit On

Imports System

Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFSP023", "総合振込入力チェックリスト")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Public LW As LogWrite
    Public TorisCode As String = ""     '取引先主コード
    Public TorifCode As String = ""     '取引先副コード
    Public FuriDate As String = ""      '振込日
    Public BaitaiCode As String         '媒体コード
    Public SortNo As String             'ソート区分(1:ソート無し 2:店番ソート)
    Public RecordCnt As Long = 0        '出力レコード数
    Public AllRecordCnt As Long = 0     '全出力レコード数
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    '2017/05/22 saitou RSV2 DEL 標準版修正（潜在バグ） ---------------------------------------- START
    '用途不明
    'Public Ret As Integer
    '2017/05/22 saitou RSV2 DEL --------------------------------------------------------------- END
    Public Jikinko As String
    '
    ' 機能　 ：総合振込入力チェックリスト印刷 メイン処理
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
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "成功", "")
            PrinterName = ""    '通常使うプリンタ

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 5 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0).ToString)
                Return -100
            End If

            LW.UserID = Cmd(0)                   'ログイン名取得
            TorisCode = Cmd(1).Substring(0, 10)  '取引先主コード
            TorifCode = Cmd(1).Substring(10, 2)  '取引先副コード
            FuriDate = Cmd(2)               '振込日
            BaitaiCode = Cmd(3)               '媒体コード
            SortNo = Cmd(4)                 'ソート区分

            LW.ToriCode = TorisCode + TorifCode
            LW.FuriDate = FuriDate
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0).ToString)

            '自金庫コード取得
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" Or Jikinko = "" Then
                BatchLog.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return -300
            End If
            '---------------------------------------------------------
            ' 総合振込入力チェックリスト印刷処理
            '---------------------------------------------------------
            Dim CheckList As New KFSP023(BaitaiCode, SortNo)
            CheckList.CreateCsvFile()
            If BaitaiCode = "04" Then
                If CheckList.MakeRecord_IRAISYO = False Then
                    If RecordCnt = -1 Then   '印刷対象なし
                        Return -1
                    Else
                        Return -300         '処理中のエラー
                    End If
                End If
            Else
                If CheckList.MakeRecord_DENPYO = False Then
                    If RecordCnt = -1 Then   '印刷対象なし
                        Return -1
                    Else
                        Return -300         '処理中のエラー
                    End If
                End If
            End If

            '印刷処理実行
            If CheckList.ReportExecute("") = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", CheckList.ReportMessage)
                '2017/05/22 saitou RSV2 UPD 標準版修正（潜在バグ） ---------------------------------------- START
                '口振に合わせる
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
                'Ret = -999     'レポエージェントからの戻り値を返す(暫定-999)
                'Return Ret
                '2017/05/22 saitou RSV2 UPD --------------------------------------------------------------- END
            End If
            Return 0
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            '2017/05/22 saitou RSV2 ADD 標準版修正（潜在バグ） ---------------------------------------- START
            '例外時もきちんと戻り値を返す
            Return -300
            '2017/05/22 saitou RSV2 ADD --------------------------------------------------------------- END
        Finally
            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", AllRecordCnt & "件")
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
            End If
        End Try

    End Function

End Module
