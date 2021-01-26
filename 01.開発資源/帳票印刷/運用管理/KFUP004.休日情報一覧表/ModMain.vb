Imports System
Imports CASTCommon

''' <summary>
''' 休日情報一覧表印刷　メインモジュール
''' </summary>
''' <remarks></remarks>
Module ModMain

#Region "クラス定数"

#End Region

#Region "クラス変数"
    Public BatchLog As New CASTCommon.BatchLOG("KFUP004", "休日情報一覧表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Public LW As LogWrite

    Public RecordCnt As Long = 0        '出力レコード数
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻
    Public TargetNen As String = ""     '対象年

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 休日情報一覧表印刷　メイン処理
    ''' </summary>
    ''' <param name="CmdArgs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       'プリンタ名
        Dim LoginID As String = ""      'ログイン名
        Dim dbConn As CASTCommon.MyOracle = Nothing
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = ""
            LW.ToriCode = "000000000000"
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
            If Cmd.Length <> 2 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)              'ログイン名取得
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            TargetNen = Cmd(1)              '対象年
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))

            '----------------------------------------
            '休日情報一覧表印刷処理
            '----------------------------------------
            Dim List As New KFUP004
            dbConn = New CASTCommon.MyOracle
            List.OraDB = dbConn
            List.CreateCsvFile()
            If List.MakeRecord = False Then
                If RecordCnt = -1 Then
                    Return -1   '印刷対象なし
                Else
                    Return -300 '処理中のエラー
                End If
            End If

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
        Finally
            If Not dbConn Is Nothing Then
                dbConn.Close()
                dbConn = Nothing
            End If

            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", RecordCnt & "件")
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
            End If
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

#End Region

End Module
