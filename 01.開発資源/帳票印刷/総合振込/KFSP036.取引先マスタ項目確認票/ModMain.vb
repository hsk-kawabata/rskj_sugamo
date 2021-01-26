'========================================================================
'ModMain
'取引先マスタ項目確認票出力（KFSP036）　メインモジュール
'
'作成日：2017/03/06
'作成者：
'
'備考：
'========================================================================
Imports System
Imports System.Text
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic

Module ModMain

#Region "クラス定数定義"
    Public BatchLog As New CASTCommon.BatchLOG("KFSP036", "取引先マスタ項目確認票")

    '帳票名
    Public Const strPrintName As String = "取引先マスタ項目確認票"
    'ログ出力用
    Public Const strLogWrite As String = "KFSP036（" & strPrintName & "）"
    'RSV2 EDITION
    Public ReadOnly RSV2_MASTPTN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN")
#End Region


#Region "クラス変数定義"

    'データベースインスタンス
    Dim orcl As CASTCommon.MyOracle

    'ログ情報構造体
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Dim stcLog As LogWrite

    Private PrinterName As String = ""  '通常使うプリンター

    '共通関数インスタンス
    Public GCom As New MenteCommon.clsCommon

    'システム日付
    Public StrSysDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    'システム時刻
    Public StrSysTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    'パラメータ用変数
    Public strUserId As String
    Public strToriSCd As String
    Public strToriFCd As String
    Public strKijyunDate As String

    Public RecordCnt As Long = 0        ' 出力レコード数
    Public LW As LogWrite

#End Region


#Region "メインメソッド"

    '=======================================================================
    'Main
    '
    '＜概要＞
    '　取引先マスタ項目確認票出力のメインメソッド。
    '
    '＜パラメータ＞
    '　Args()：起動パラメータ
    '　　(0)ユーザＩＤ
    '　　(1)取引先主コード
    '　　(2)取引先副コード
    '　　(3)基準日(yyyyMMdd)
    '
    '＜戻り値＞
    '　　0：正常終了
    '　 -1：出力データなしエラー
    '　100：パラーメータ異常（パラメータなし）
    '　101：iniファイル取得エラー
    '　　9：その他異常終了
    '=======================================================================
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
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "開始", "失敗", "引数不足")
                Return 100
            End If

            If CmdArgs(0).Split(","c).Length <> 4 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "開始", "失敗", "引数まちがい")
                Return 100
            End If

            Dim Cmd() As String = CmdArgs(0).Split(","c)
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理開始", "成功", CmdArgs(0))
            Try
                ' 起動パラメータ格納
                strUserId = Cmd(0)
                strToriSCd = Cmd(1)
                strToriFCd = Cmd(2)
                strKijyunDate = Cmd(3)

                With LW
                    .UserID = strUserId
                    .ToriCode = strToriSCd & strToriFCd
                End With
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功")
            Catch ex As Exception
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "失敗", ex.Message)
                Return 9
            End Try

            'KFSP036クラス生成
            Dim clsRep As New KFSP036
            clsRep.CreateCsvFile()
            If clsRep.MakeRecord = False Then
                If RecordCnt = -1 Then      '印刷対象なし
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "終了", "成功", "印刷対象なし")
                    Return -1
                Else
                    Return 9
                End If
            End If

            '印刷処理実行
            If clsRep.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", clsRep.ReportMessage)
                Return -999     'レポエージェントからの戻り値を返す(暫定-999)
            End If
            Return 0

        Catch ex As Exception
            '異常終了ログ
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            Return 9
        End Try

    End Function

#End Region

End Module
