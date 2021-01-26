Imports System
Imports System.IO

''' <summary>
''' 処理結果確認表(CSVリエンタ)メインモジュール
''' </summary>
''' <remarks>2016/10/20 saitou RSV2 added for 信組対応</remarks>
Module ModMain

#Region "モジュール定数"

#End Region

#Region "モジュール変数"

    Private BatchLog As New CASTCommon.BatchLOG("KFSP032", "処理結果確認表(CSVリエンタ)")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure
    Private LW As LogWrite

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 処理結果確認表(CSVリエンタ)出力メイン処理
    ''' </summary>
    ''' <param name="CmdArgs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       'プリンタ名
        Dim CSVFileName As String = ""  'ＣＳＶファイル名(フルパス)
        LW.UserID = ""      'ログイン名
        LW.ToriCode = "0000000000-00"
        LW.FuriDate = "00000000"
        Try
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
            If Cmd.Length <> 2 Then
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)        'ログイン名取得
            CSVFileName = Cmd(1)    'ＣＳＶファイル名取得

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            If Not File.Exists(CSVFileName) Then
                '対象CSVファイルなし
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "CSVファイル取得失敗 CSVファイル名:" & CSVFileName)
                Return -200
            End If
            '---------------------------------------------------------
            ' 処理結果確認表(CSVリエンタ)印刷処理
            '---------------------------------------------------------
            Dim List As New KFSP032(CSVFileName)

            '印刷処理実行
            If List.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", List.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If
            Return 0
        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            Return -999
        Finally
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
        End Try

    End Function

#End Region

#Region "プライベートメソッド"

#End Region

End Module
