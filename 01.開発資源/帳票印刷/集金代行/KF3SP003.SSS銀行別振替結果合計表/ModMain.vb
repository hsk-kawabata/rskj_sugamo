﻿Imports System
Imports System.Collections
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports CASTCommon

''' <summary>
''' SSS銀行別振替結果合計表印刷　メインモジュール
''' </summary>
''' <remarks></remarks>
Module ModMain

#Region "クラス定数"

#End Region

#Region "クラス変数"
    Public BatchLog As New CASTCommon.BatchLOG("KF3SP003", "SSS銀行別振替結果合計表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Public LW As LogWrite

    Public RecordCnt As Long = 0     '出力レコード数
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻

    Public Structure strcIniInfo
        Dim KINKOCD As String
        Dim ZEIKIJUN As String
    End Structure
    Public IniInfo As strcIniInfo

    Public Structure strcTesuuInfo
        Dim SYUBETU As String
        Dim TESUU_KIN As String
        Dim SYOUHI_KBN As String
    End Structure

    Public FURI_DATE As String

    Public TAX As CASTCommon.ClsTAX

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' SSS銀行別振替結果合計表印刷　メイン処理
    ''' </summary>
    ''' <param name="CmdArgs"></param>
    ''' <returns></returns>
    ''' <remarks>2013/12/02 消費税対応メモ この帳票は振替日ベースでしか消費税計算できない</remarks>
    Public Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       'プリンタ名
        Dim LoginID As String = ""      'ログイン名
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
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)          'ログイン名取得
            LW.FuriDate = Cmd(1)        '振替日取得
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))

            FURI_DATE = Cmd(1)

            '---------------------------------------------------------
            '設定ファイル読み込み
            '---------------------------------------------------------
            If GetIniInfo() = False Then
                Return -100
            End If

            TAX = New CASTCommon.ClsTAX

            '---------------------------------------------------------
            'SSS銀行別振替結果合計表印刷処理
            '---------------------------------------------------------
            Dim List As New KF3SP003
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
            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", RecordCnt & "件")
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)終了", "成功", "")
            End If
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 設定ファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function GetIniInfo() As Boolean
        Try
            IniInfo.KINKOCD = GetFSKJIni("COMMON", "KINKOCD")
            If IniInfo.KINKOCD.Equals("err") OrElse IniInfo.KINKOCD = Nothing Then
                BatchLog.Write("INIファイル取得", "失敗", "[COMMON].KINKOCD 取得失敗")
                Return False
            End If

            IniInfo.ZEIKIJUN = GetFSKJIni("JIFURI", "ZEIKIJUN")
            If IniInfo.ZEIKIJUN.Equals("err") OrElse IniInfo.ZEIKIJUN = Nothing Then
                BatchLog.Write("INIファイル取得", "失敗", "[JIFURI].ZEIKIJUN 取得失敗")
                Return False
            End If

            Return True
        Catch ex As Exception
            BatchLog.Write("INIファイル取得", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Module
