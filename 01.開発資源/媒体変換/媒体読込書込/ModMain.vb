Option Explicit On 
Option Strict On

Imports CASTCommon
Imports MenteCommon
Imports System.Data.OracleClient
Imports CMT


Module ModMain
    Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" _
        (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Public GCom As MenteCommon.clsCommon

    Public CmtCom As clsCmtCommon       '当プロジェクト内の共通機能関数群
    Public DB As clsDataBase            '当プロジェクト内のテーブル関連共通関数群
    Public GOwnerForm As Form           '汎用

    ' 連装CMTの最大スタッカ数
    Public Const MAXSTACKER As Integer = 1         '***修正 瀬戸 10 → 1

    'INIファイル読み込み用変数
    Public gintTEMP_LEN As Integer    'API関数取得文字数
    Public gstrIFileName As String
    Public gstrIAppName As String
    Public gstrIKeyName As String
    Public gstrIDefault As String

    ' 格納フォルダ名
    Public gstrCMTServerRead As String                          ' サーバの読込用パス
    Public gstrCMTServerWrite As String                         ' サーバの書込用パス
    Public gstrCMTWriteFileName(MAXSTACKER) As String           ' ローカルPC上の出力ファイル名
    Public gstrCMTReadFileName(MAXSTACKER) As String            ' ローカルPC上の入力ファイル名
    Public gstrCMTHeadLabelFileName(MAXSTACKER) As String       ' ローカルPC上のラベルファイル名
    Public gstrCMTEndLabelFileName(MAXSTACKER) As String        ' ローカルPC上のEOFラベルファイル名

    Private Const ThisModuleName As String = "ModMain.vb"

    Public Const GErrorString As String = "予期せぬエラー"

    Public Sub Main()
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim MSG As String = String.Empty
        'Dim MSG As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        '共通的な関数を搭載
        GCom = New MenteCommon.clsCommon
        With GCom.GLog
            .Job = "CMT変換"
            .Job1 = "起動処理"
        End With
        Try
            '当Project専用クラス初期化
            CmtCom = New clsCmtCommon           '当プロジェクト内の共通機能関数群
            DB = New clsDataBase                '当プロジェクト内のテーブル関連共通関数群

            '二重起動チェック
            '*** 修正 mitsu 2008/09/01 処理高速化 ***
            'If UBound(Diagnostics.Process.GetProcessesByName( _
            '                   Diagnostics.Process.GetCurrentProcess.ProcessName)) > 0 Then
            '    With GCom.GLog
            '        .Job2 = "二重起動チェック"
            '        .Result = MenteCommon.clsCommon.NG
            '        .Discription = "強制終了"
            '    End With
            '    Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            '    Return
            'End If
            Dim mp As Process = Process.GetCurrentProcess
            Dim ps() As Process = Process.GetProcesses()
            For i As Integer = 0 To ps.Length - 1
                '同じプロセス名でプロセスIDが違う場合は二重起動検出
                If ps(i).ProcessName = mp.ProcessName AndAlso ps(i).Id <> mp.Id Then
                    With GCom.GLog
                        .Job2 = "二重起動チェック"
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "強制終了"
                    End With
                    Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return
                End If
            Next
            '****************************************

            'コマンドライン評価
            If Not GetEnvironment(MSG) Then
                With GCom.GLog
                    .Job2 = "コマンドライン評価"
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = MSG
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

                GCom.GetUserID = "NoCommandLine"
                GCom.GetSysDate = #10/20/2007#
            End If


            'iniファイル読み込み
            If fn_set_INIFILE() = False Then
                With GCom.GLog
                    .Job2 = "iniファイル取得"
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = ""
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("iniファイルの取得に失敗しました", "iniファイル取得", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' 機番チェック. 機番の取得に失敗した場合は終了
            If Not CmtCom.SetStationNo() Then
                Return
            End If

            '起動痕跡LOG出力
            With GCom.GLog
                .Job2 = "システム起動"
                .Result = MenteCommon.clsCommon.OK
                .Discription = ""
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))


        Catch ex As Exception

        Finally
            '画面起動
            Dim onForm As New KFCMENU010
            With onForm
                onForm.Show()
                '2011/06/23 標準版修正 画面をXPスタイルに変更 ------------------START
                Application.EnableVisualStyles()
                '2011/06/23 標準版修正 画面をXPスタイルに変更 ------------------END

                Application.Run(onForm)

                '終了痕跡LOG出力
                With GCom.GLog
                    .Job2 = "システム終了"
                    .Result = MenteCommon.clsCommon.OK
                    .Discription = ""
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

                GCom = Nothing
                .Dispose()
            End With
        End Try

    End Sub


    ' 機能　　　: コマンドラインを評価する
    '
    ' 戻り値　　: 正常 = True
    ' 　　　　　  異常 = False
    '
    ' 引き数　　: ARG1 - なし
    '
    ' 備考　　　: 流用する
    '
    Private Function GetEnvironment(ByRef MSG As String) As Boolean
        Dim Cnt As Integer
        Dim CmdLine() As String

        MSG = "パラメータ不正。"

        '起動パラメータ有無／内容検査
        If Not Environment.GetCommandLineArgs.Length = 2 Then

            For Cnt = 1 To Environment.GetCommandLineArgs.Length - 1 Step 1
                Select Case Cnt = 1
                    Case True : MSG &= "(" & Environment.GetCommandLineArgs(Cnt)
                    Case Else : MSG &= " " & Environment.GetCommandLineArgs(Cnt)
                End Select
            Next Cnt
            MSG &= ")"

            Return False
        Else
            CmdLine = Environment.GetCommandLineArgs(1).Split(","c)

            For Cnt = 0 To CmdLine.Length - 1 Step 1
                Select Case Cnt = 0
                    Case True : MSG &= "(" & CmdLine(Cnt)
                    Case Else : MSG &= "," & CmdLine(Cnt)
                End Select
            Next Cnt
            MSG &= ")"

            If Not CmdLine.Length = 2 Then Return False
        End If

        '連携ユーザＩＤ 格納
        GCom.GetUserID = CmdLine(0).Trim

        ' 連携日付桁数／属性チェック
        If Not CmdLine(1).Trim.Length = 8 OrElse Not GCom.IsNumber(CmdLine(1).Trim) Then

            Return False
        End If

        '連携日付歴日チェック & 格納
        Dim sDate(2) As Integer
        With CmdLine(1).Trim
            sDate(0) = GCom.NzInt(.Substring(0, 4))
            sDate(1) = GCom.NzInt(.Substring(4, 2))
            sDate(2) = GCom.NzInt(.Substring(6))
        End With

        ' 連携日付(yyyymmdd)をファイル名等で流用するためグローバル変数にコピー
        CmtCom.gstrSysDate = CmdLine(1)

        Return (GCom.SET_DATE(GCom.GetSysDate, sDate) < 0)
    End Function


    '
    ' 機能　 ： CMT変換関連のINIファイルの情報取得
    '           読み取り元: fskj.ini, cmt.ini
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： 正常終了 = True
    ' 　　　 　 異常終了 = False
    '
    ' 備考　 ： 2007.11.13 Update By Astar
    '           2007.11.21 Update By Astar ラベル対応
    Function fn_set_INIFILE() As Boolean

        Dim Ret As Boolean = False
        Try
            ' FSKJ.INIからの読込
            ' 読込ファイルを格納するサーバ上フォルダ名の取得
            'FSKJ.iniからとるかCMT.iniからとるか、とりあえずCMT.iniに
            gstrCMTServerRead = GetCMTIni("JIFURI", "DEN")

            ' 返却ファイル(結果ファイル)を格納するサーバ上のフォルダ名の取得
            gstrCMTServerWrite = GetCMTIni("JIFURI", "DEN")

            ' CMT.INIからの読込
            Dim strCMTWriteFileName As String = GetCMTIni("FILE-NAME", "WRITE")

            Dim strCMTReadFileName As String = GetCMTIni("FILE-NAME", "READ")
            Dim strCMTHeadLabelFileName As String = GetCMTIni("FILE-NAME", "HEAD")
            Dim strCMTEndLabelFileName As String = GetCMTIni("FILE-NAME", "END")

            For i As Integer = 1 To MAXSTACKER
                gstrCMTWriteFileName(i - 1) = GetCMTIni("WRITE-DIRECTORY", i.ToString) & "\" & strCMTWriteFileName
                gstrCMTReadFileName(i - 1) = GetCMTIni("READ-DIRECTORY", i.ToString) & "\" & strCMTReadFileName
                gstrCMTHeadLabelFileName(i - 1) = GetCMTIni("READ-DIRECTORY", i.ToString) & "\" & strCMTHeadLabelFileName
                gstrCMTEndLabelFileName(i - 1) = GetCMTIni("READ-DIRECTORY", i.ToString) & "\" & strCMTEndLabelFileName
                If ((gstrCMTWriteFileName(i - 1) = "err") Or (gstrCMTReadFileName(i) = "err")) Then
                    Throw New System.Exception("CMT.iniファイル読取失敗")
                End If
            Next i

            ' いずれかひとつでも取得失敗した場合はエラーとみなす
            If ((gstrCMTServerRead = "err") Or (gstrCMTServerWrite = "err")) Then
                Ret = False
            Else
                Ret = True
            End If
        Catch ex As Exception
            Ret = False
        End Try

        REM TODO iniファイル取得ファイル名の検証

        Return Ret
    End Function
End Module
