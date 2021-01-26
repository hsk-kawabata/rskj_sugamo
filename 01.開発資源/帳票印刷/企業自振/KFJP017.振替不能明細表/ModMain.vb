Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' 振替不能明細表処理 メインモジュール
Module ModMain
    '2009/12/29 手数料計算用 ==========
    '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
    'Public Const strTESUU_TABLE_FILE_NAME As String = "KFJMAST010_振込手数料基準ID.TXT"
    'Public TXT_FOLDER As String
    '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
    Structure TESUU_TABLE
        Dim strKIJYUN_ID_CODE As String         '手数料基準ID
        Dim strKIJYUN_ID_TEXT As String         '手数料基準ID名
        Dim lng10000UNDER_JITEN As Long         '手数料（自店１万円未満）
        Dim lng30000UNDER_JITEN As Long         '手数料（自店１万円以上３万円未満）
        Dim lng30000OVER_JITEN As Long          '手数料（自店３万円以上）
        Dim lng10000UNDER_HONSITEN As Long      '手数料（本支店１万円未満）
        Dim lng30000UNDER_HONSITEN As Long      '手数料（本支店１万円以上３万円未満）
        Dim lng30000OVER_HONSITEN As Long       '手数料（本支店３万円以上）
        Dim lng10000UNDER_TAKOU As Long         '手数料（他行３万円未満）
        Dim lng30000UNDER_TAKOU As Long         '手数料（他行１万円以上３万円以上）
        Dim lng30000OVER_TAKOU As Long          '手数料（他行３万円以上）
    End Structure
    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
    ' 手数料テーブルの件数を100件までに変更
    'Public TESUU_TABLE_DATA(10) As TESUU_TABLE
    Public TESUU_TABLE_DATA(100) As TESUU_TABLE
    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
    '=================================

    ' ログ処理クラス
    Public MainLOG As New CASTCommon.BatchLOG("KFJP017", "振替不能明細表")

    '
    ' 機能　 ： 振替不能明細表 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ：
    '
    Function Main(ByVal arg() As String) As Integer
        '*** 修正 mitsu 2009/04/10 ログ機能強化 ***
        Try
            '**************************************

            ' 振替不能明細表出力処理
            Dim FrikaekeFunoumeisai As New ClsFrikaeFnoumeisai

            ' 戻り値
            Dim ret As Integer

            If arg.Length = 0 Then
                MainLOG.Write("開始", "失敗", "引数不足")
                Return 1
            End If

            '2012/06/30 標準版　WEB伝送対応------------------------------------------------->
            If arg(0).Split(","c).Length <> 3 And arg(0).Split(","c).Length <> 4 And arg(0).Split(","c).Length <> 5 Then
                'If arg(0).Split(","c).Length <> 3 And arg(0).Split(","c).Length <> 4 Then
                '---------------------------------------------------------------------------<
                MainLOG.Write("開始", "失敗", "引数まちがい")
                Return 1
            End If

            Dim Cmd() As String = arg(0).Split(","c)

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("処理開始")
            '*** 修正 mitsu 2009/04/10 ログ機能強化 ***
            'MainLOG.Write("処理開始", "成功", arg(0))
            '******************************************
            Try
                ' 起動パラメータ格納
                FrikaekeFunoumeisai.TORIS_CODE = Cmd(0).PadRight(10).Substring(0, 10)
                FrikaekeFunoumeisai.TORIF_CODE = Cmd(0).PadRight(2).Substring(10)
                FrikaekeFunoumeisai.FURI_DATE = Cmd(1)
                FrikaekeFunoumeisai.FUNO_FLG = Cmd(2)
                If Cmd.Length = 4 Then
                    FrikaekeFunoumeisai.PRINTERNAME = Cmd(3)
                End If
                '2012/06/30 標準版　WEB伝送対応------------->
                If Cmd.Length = 5 Then
                    FrikaekeFunoumeisai.INVOKE_KBN = Cmd(4)
                End If
                '-------------------------------------------<
                MainLOG.ToriCode = Cmd(0)
                MainLOG.FuriDate = Cmd(1)
                MainLOG.Write("パラメータ取得", "成功")
            Catch ex As Exception
                MainLOG.Write("パラメータ取得", "失敗", ex.Message)
                Return -1
            End Try

            '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
            ''2009/12/29 追加 ===========================
            'TXT_FOLDER = CASTCommon.GetFSKJIni("COMMON", "TXT")
            'If TXT_FOLDER = "err" Then
            '    MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - TXT)")
            '    Return 1
            'End If
            ''===========================================
            '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<

            ' メイン処理
            ret = FrikaekeFunoumeisai.Main()
            ELog.Write("処理終了")
            '*** 修正 mitsu 2009/04/10 ログ機能強化 ***
            MainLOG.Write("処理終了", "成功")
            '******************************************

            Return ret

            '*** 修正 mitsu 2009/04/10 ログ機能強化 ***
        Catch ex As Exception
            MainLOG.Write("想定外のエラーが発生しました", "失敗", ex.Message & "：" & ex.StackTrace)
            Return -1
        End Try
        '**********************************************
    End Function

End Module
