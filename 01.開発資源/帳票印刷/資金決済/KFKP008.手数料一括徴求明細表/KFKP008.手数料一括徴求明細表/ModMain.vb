Imports System
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFKP008", "手数料徴求一括明細表")
    Public Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Public LW As LogWrite
    Public TESUUTYO_YDATE As String = ""    '手数料徴求予定日(From)
    Public TESUUTYO_YDATE2 As String = ""    '手数料徴求予定日(To)  '2009/12/02 追加
    Public RecordCnt As Long = 0     '出力レコード数
    Public GCOM As New MenteCommon.clsCommon
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' 現在日付
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' 現在時刻

    '手数料計算用
    '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
    'Public Const strTESUU_TABLE_FILE_NAME As String = "KFJMAST010_振込手数料基準ID.TXT"
    '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
    Public TXT_FOLDER As String
    Public Jikinko As String
    '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
    'Public ZEI_RITU As String
    '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
    '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
    Public ZEIKIJUN As String
    Public TAX As CASTCommon.ClsTAX
    '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
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
        Dim lng30000UNDER_TAKOU As Long          '手数料（他行１万円以上３万円以上）
        Dim lng30000OVER_TAKOU As Long          '手数料（他行３万円以上）
    End Structure
    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
    ' 手数料テーブルの件数を100件までに変更
    'Public TESUU_TABLE_DATA(10) As TESUU_TABLE
    Public TESUU_TABLE_DATA(100) As TESUU_TABLE
    ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
    '
    ' 機能　 ：手数料徴求一括明細表印刷 メイン処理
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
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            PrinterName = ""    '通常使うプリンタ
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "成功", "")

            '---------------------------------------------------------
            'パラメータ取得
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                'パラメータ取得失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータチェック", "失敗", "コマンドライン引数なし")
                Return -100
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            '2009/12/02 パラメータ数変更
            'If Cmd.Length <> 2 Then
            If Cmd.Length <> 3 Then
                '=====================
                'パラメータ間違い
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷処理)開始", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return -100
            End If

            LW.UserID = Cmd(0)        'ログイン名取得
            TESUUTYO_YDATE = Cmd(1)     '手数料徴求予定日(From)
            TESUUTYO_YDATE2 = Cmd(2)   '2009/12/02 追加 手数料徴求予定日(To) 
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "パラメータ取得", "成功", "コマンドライン引数：" & CmdArgs(0))
            '---------------------------------------------------------
            ' 手数料徴求一括明細表印刷処理
            '---------------------------------------------------------
            TXT_FOLDER = CASTCommon.GetFSKJIni("COMMON", "TXT")
            If TXT_FOLDER = "err" Then
                BatchLog.Write(BatchLog.ToriCode, BatchLog.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - TXT)")
                Return 1
            End If

            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko = "err" Then
                BatchLog.Write(BatchLog.ToriCode, BatchLog.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - KINKOCD)")
                Return 1
            End If

            '2013/11/14 saitou 標準版 消費税対応 DEL -------------------------------------------------->>>>
            'ZEI_RITU = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
            'If ZEI_RITU = "err" Then
            '    BatchLog.Write(BatchLog.ToriCode, BatchLog.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(COMMON - ZEIRITU)")
            '    Return 1
            'ElseIf ZEI_RITU = "" Then
            '    ZEI_RITU = "1.05"
            'End If
            '2013/11/14 saitou 標準版 消費税対応 DEL --------------------------------------------------<<<<
            '2013/11/14 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            ZEIKIJUN = CASTCommon.GetFSKJIni("JIFURI", "ZEIKIJUN")
            If ZEIKIJUN = "err" Then
                BatchLog.Write(BatchLog.ToriCode, BatchLog.FuriDate, "情報設定ファイル", "失敗", "イニシャルファイルからの取得失敗(JIFURI - ZEIKIJUN)")
                Return 1
            End If
            TAX = New CASTCommon.ClsTAX
            '2013/11/14 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
            Dim MEISAI As New KFKP008()
            MEISAI.CreateCsvFile()
            If MEISAI.MakeRecord = False Then
                If RecordCnt = -1 Then   '印刷対象なし
                    Return -1
                Else
                    Return -300 '処理中のエラー
                End If
            End If

            '印刷処理実行
            If MEISAI.ReportExecute(PrinterName) = True Then
                '印刷成功
            Else
                '印刷失敗
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent印刷", "失敗", MEISAI.ReportMessage)
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

End Module
