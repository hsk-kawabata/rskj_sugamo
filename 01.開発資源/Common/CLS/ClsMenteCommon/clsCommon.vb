Option Explicit On
Option Strict On

Imports System.IO
Imports System.text
Imports System.Globalization
Imports CASTCommon
Imports System.IO.Directory
Imports System.Data.OracleClient
' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- START
Imports System.Text.RegularExpressions
' GENGOU 2019/04/19 ADD ITL)OOKUBO ------------------------------------------------- END

Public Class clsCommon
    Public MainLog As CASTCommon.BatchLOG   'ログ統一用
    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean
    Private IS_SQLLOG As Boolean
    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

    Public LogUserID As String
    Public LogToriCode As String
    Public LogFuriDate As String
    Private Structure DefineOnThisProject
        Dim UserID As String                'UserID(起動パラメータ連携)
        Dim SysDate As Date                 'SystemDate(起動パラメータ連携)
        Dim BinFolder As String             '実行モジュール格納パス
        Dim LogFolder As String             'ログ情報格納パス
        Dim TXTFolder As String             'テキストファイル格納パス
        Dim PRTFolder As String             '印刷連携用ファイル格納パス
        Dim LSTFolder As String             '帳表定義体格納パス
        Dim DLLFolder As String             'DLL格納パス
        Dim Item() As String                '(汎用)配列情報記憶
        Dim GLogFolder As String            '学校ログ情報格納パス
    End Structure
    Private GSYS As DefineOnThisProject

    Public Structure LogWriteStructure
        Dim ToriCode As String          '取引先コード(0000000-00)
        Dim FuriDate As String          '振替日(yyyymmdd)
        Dim Job As String               '(大分類ジョブ名) Log Fileの名称に使用
        Dim Job1 As String              '(中分類ジョブ名)
        Dim Job2 As String              '(小分類ジョブ名)
        Dim Result As String            '処理の結果等
        Dim Discription As String       '備考欄(格納ファイル名+実行関数名+エラー内容)
    End Structure
    Public GLog As LogWriteStructure

    '休日情報
    Private Data() As Integer

    Public Enum enDB
        DB_Connect = 1        'DB接続
        DB_Begin = 2          'トランザクション開始
        DB_Execute = 4        'SQL文の実行
        DB_Commit = 8         'コミット
        DB_Rollback = 16      'ロールバック
        DB_Terminate = 32     'DB切断
    End Enum

    Public Structure JIFURI_Session
        Dim FUNOU As Integer                'GIntFUNOU          不能区分取り込み
        Dim FUNOU_TAKOU_1 As Integer        'GIntFUNOU_TAKOU_1  不能区分取り込み※他行提携内分
        Dim FUNOU_TAKOU_2 As Integer        'GIntFUNOU_TAKOU_2  不能区分取り込み※他行提携外分
        Dim FUNOU_SSS_1 As Integer          'GIntFUNOU_SSS_1    不能区分取り込み※SSS他行提携内分
        Dim FUNOU_SSS_2 As Integer          'GIntFUNOU_SSS_2    不能区分取り込み※SSS他行提携外分
        Dim HAISIN As Integer               'GIntHAISIN         配信区分取り込み
        Dim HAISIN_TAKOU_1 As Integer       'GIntHAISIN_TAKOU_1 配信区分(他行提携内)取り込み
        Dim HAISIN_TAKOU_2 As Integer       'GIntHAISIN_TAKOU_2 配信区分(他行提携外)取り込み
        Dim HAISIN_SSS_1 As Integer         'GIntHAISIN_SSS_1   配信区分(SSS他行提携内)取り込み
        Dim HAISIN_SSS_2 As Integer         'GINtHAISIN_SSS_2   配信区分(SSS他行提携外)取り込み
        Dim KAISYU As Integer               'gintKAISYU_KBN     回収区分取り込み
        Dim HITS As Integer
        Dim HITQ As Integer
    End Structure
    Public DayINI As JIFURI_Session

    Private OraConnect As OracleClient.OracleConnection
    Private Tran As OracleTransaction
    Private Command As OracleCommand

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
    Private GCOM_MainDB As CASTCommon.MyOracle   'メインDBコネクション

    Public WriteOnly Property Oracle() As CASTCommon.MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            GCOM_MainDB = Value
        End Set
    End Property
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

    Public Const GAppName As String = "Main"
    Public Const NG As String = "失敗"
    Public Const OK As String = "成功"
    Public Const ErrorString As String = "予期せぬエラー"
    Public Const BadResultDate As Date = #1/1/1900#

    '*** 修正 mitsu 2008/09/01 秒数修正 ***
    Private Const IntWaitTimer As Integer = 15000
    '**************************************
    Private Const ThisModuleName As String = "clsCommon.vb"

    '*** 修正 mitsu 2008/05/27 自金庫コード追加 ***
    Public ReadOnly JIKINKOCD As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
    '**********************************************

    '*** 修正 mitsu 2008/09/01 エンコーディング追加 ***
    Public ReadOnly EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")
    '**************************************************

    '起動パラメータから取得したユーザID
    Public Property GetUserID() As String
        Get
            Return GSYS.UserID
        End Get
        Set(ByVal Value As String)
            GSYS.UserID = Value
        End Set
    End Property

    '起動パラメータから取得したシステム日付
    Public Property GetSysDate() As Date
        Get
            Return GSYS.SysDate
        End Get
        Set(ByVal Value As Date)
            GSYS.SysDate = Value
        End Set
    End Property

    'INIファイルから取得した実行モジュール格納パス
    Public ReadOnly Property GetBinFolder() As String
        Get
            Return GSYS.BinFolder
        End Get
    End Property

    'INIファイルから取得したログファイル格納パス
    Public Property GetLogFolder() As String
        Get
            Return GSYS.LogFolder
        End Get
        Set(ByVal Value As String)
            GSYS.LogFolder = Value
        End Set
    End Property

    'INIファイルから取得したログファイル格納パス
    Public Property GetGLogFolder() As String
        Get
            Return GSYS.GLogFolder
        End Get
        Set(ByVal Value As String)
            GSYS.GLogFolder = Value
        End Set
    End Property

    'INIファイルから取得したテキスト情報格納パス
    Public Property GetTXTFolder() As String
        Get
            Return GSYS.TXTFolder
        End Get
        Set(ByVal Value As String)
            GSYS.TXTFolder = Value
        End Set
    End Property

    'INIファイルから取得した帳表定義体格納パス
    Public Property GetLSTFolder() As String
        Get
            Return GSYS.LSTFolder
        End Get
        Set(ByVal Value As String)
            GSYS.LSTFolder = Value
        End Set
    End Property

    'INIファイルから取得したDLL格納パス
    Public ReadOnly Property GetDLLFolder() As String
        Get
            Return GSYS.DLLFolder
        End Get
    End Property

    'INIファイルから取得した印刷用CSV格納パス
    Public ReadOnly Property GetPRTFolder() As String
        Get
            Return GSYS.PRTFolder
        End Get
    End Property

    '起動パラメータから取得したユーザID
    Public Property UseItem() As String()
        Get
            Return GSYS.Item
        End Get
        Set(ByVal Value As String())
            GSYS.Item = Value
        End Set
    End Property

    '当モジュール格納ファイル名
    Public ReadOnly Property GetThisModuleName() As String
        Get
            Return ThisModuleName
        End Get
    End Property

    Public Sub New()

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        MainLog = New BatchLOG("clsCommon", "GCOM")
        IS_LEVEL3 = MainLog.IS_LEVEL3()
        IS_LEVEL4 = MainLog.IS_LEVEL4()
        IS_SQLLOG = MainLog.IS_SQLLOG()
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        '実行フォルダ設定
        GSYS.BinFolder = GetAppPath()

        'TXTファイル格納場所設定
        GSYS.TXTFolder = CASTCommon.GetFSKJIni("COMMON", "TXT")

        '定義体ファイル格納場所設定
        GSYS.LSTFolder = CASTCommon.GetFSKJIni("COMMON", "LST")

        'DLLファイル格納場所設定
        GSYS.DLLFolder = CASTCommon.GetFSKJIni("COMMON", "DLL")

        'LOGファイル格納場所設定
        GSYS.LogFolder = CASTCommon.GetFSKJIni("COMMON", "LOG")
        If Not GSYS.LogFolder.ToUpper = "ERR" Then

            'フォルダ有無確認(無ければ作成)
            If Not System.IO.Directory.Exists(GSYS.LogFolder) Then
                System.IO.Directory.CreateDirectory(GSYS.LogFolder)
            End If
        End If

        'GLOGファイル格納場所設定
        GSYS.GLogFolder = CASTCommon.GetFSKJIni("GCOMMON", "LOG")
        If Not GSYS.GLogFolder.ToUpper = "ERR" Then

            'フォルダ有無確認(無ければ作成)
            If Not System.IO.Directory.Exists(GSYS.GLogFolder) Then
                System.IO.Directory.CreateDirectory(GSYS.GLogFolder)
            End If
        End If

        'CSVファイル格納場所設定
        GSYS.PRTFolder = CASTCommon.GetFSKJIni("COMMON", "PRT")
        If Not GSYS.PRTFolder.ToUpper = "ERR" Then

            'フォルダ有無確認(無ければ作成)
            If Not System.IO.Directory.Exists(GSYS.PRTFolder) Then
                System.IO.Directory.CreateDirectory(GSYS.PRTFolder)
            End If
        End If

        '2007,12,12 追加 By Astar
        With DayINI
            '不能区分取り込み
            .FUNOU = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU"))

            '不能区分取り込み※他行提携内分
            .FUNOU_TAKOU_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_TAKOU_1"))
            If .FUNOU_TAKOU_1 = 0 Then
                .FUNOU_TAKOU_1 = .FUNOU
            End If

            '不能区分取り込み※他行提携外分
            .FUNOU_TAKOU_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_TAKOU_2"))
            If .FUNOU_TAKOU_2 = 0 Then
                .FUNOU_TAKOU_2 = .FUNOU_TAKOU_1
            End If

            '不能区分取り込み※SSS他行提携内分
            .FUNOU_SSS_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_1"))
            If .FUNOU_SSS_1 = 0 Then
                .FUNOU_SSS_1 = .FUNOU
            End If

            '不能区分取り込み※SSS他行提携外分
            .FUNOU_SSS_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_2"))
            If .FUNOU_SSS_2 = 0 Then
                .FUNOU_SSS_2 = .FUNOU_SSS_1
            End If

            '配信区分取り込み
            .HAISIN = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN"))

            '配信区分(他行提携内)取り込み
            .HAISIN_TAKOU_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_TAKOU_1"))
            If .HAISIN_TAKOU_1 = 0 Then
                .HAISIN_TAKOU_1 = .HAISIN
            End If

            '配信区分(他行提携外)取り込み
            .HAISIN_TAKOU_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_TAKOU_2"))
            If .HAISIN_TAKOU_2 = 0 Then
                .HAISIN_TAKOU_2 = .HAISIN_TAKOU_1
            End If

            '配信区分(SSS他行提携内)取り込み
            .HAISIN_SSS_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_SSS_1"))
            If .HAISIN_SSS_1 = 0 Then
                .HAISIN_SSS_1 = .HAISIN
            End If

            '配信区分(SSS他行提携外)取り込み
            .HAISIN_SSS_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_SSS_2"))
            If .HAISIN_SSS_2 = 0 Then
                .HAISIN_SSS_2 = .HAISIN_SSS_1
            End If

            '回収区分取り込み
            .KAISYU = NzInt(CASTCommon.GetFSKJIni("JIFURI", "KAISYU"))
        End With

        'ログ関連値の初期化
        With GLog
            .ToriCode = String.Format("{0:0000000-00}", 0)
            .FuriDate = New String("0"c, 8)
            .Job = GAppName
            .Job1 = ""
            .Job2 = ""
            .Result = ""
            .Discription = ""
        End With
    End Sub

    '
    ' 機　能 : 稼働場所の取得
    '
    ' 戻り値 : 稼働場所
    '
    ' 引き数 : ARG1 - なし
    '
    ' 備　考 : 汎用
    '    
    Public Function GetAppPath() As String
        Dim FL As New System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)
        Return FL.DirectoryName
    End Function

    '
    ' 機能　　　: ログ出力モジュール
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - リクエスト・モジュール格納ファイル名
    ' 　　　　　  ARG2 - リクエスト・モジュール
    '
    ' 備考　　　: 流用する
    '           : ログ出力に失敗した場合(通常ありえない)は通番をカウントアップする
    '
    Public Sub FN_LOG_WRITE(ByVal avOnCallFileName As String, ByVal avOnCallModule As StackTrace)
        Dim MSG As String
        Dim FS As FileStream
        Dim FW As StreamWriter = Nothing
        Const C34 As String = ControlChars.Quote
        Dim DRet As DialogResult
        '*** 修正 mitsu 2008/08/04 ログ出力失敗対応 ***
        Dim FileSeq As Integer = 0
        Dim ErrCnt As Integer = 0
        '**********************************************

        '*** 修正 mitsu 2008/08/04 ログ出力失敗対応 ***
        While True
            '******************************************
            Try
                Dim sFileName As String = SET_PATH(GSYS.LogFolder)
                sFileName &= GLog.Job
                '*** 修正 mitsu 2008/08/04 ファイル名にシーケンス追加 ***
                'sFileName &= String.Format("{0:yyyyMMdd}", Date.Now) & ".LOG"
                sFileName &= String.Format("{0:yyyyMMdd}", Date.Now) & "." & FileSeq.ToString("00000") & ".LOG"
                '********************************************************

                '*** 修正 mitsu 2008/09/01 正常の場合はスタックトレースを出力しない ***
                'MSG = avOnCallFileName & ": "
                'MSG &= avOnCallModule.GetFrame(0).GetMethod.Name
                If GLog.Result = OK Then
                    MSG = ""
                Else
                    MSG = avOnCallFileName & ": "
                    MSG &= avOnCallModule.GetFrame(0).GetMethod.Name
                End If
                '**********************************************************************

                If GLog.Discription.Trim.Length > 0 Then
                    '*** 修正 mitsu 2008/09/01 処理高速化 ***
                    'GLog.Discription = GLog.Discription.Replace(ControlChars.CrLf, "")
                    'GLog.Discription = GLog.Discription.Replace(ControlChars.Cr, "")
                    'GLog.Discription = GLog.Discription.Replace(ControlChars.Lf, "")
                    'MSG &= ": " & GLog.Discription.Trim
                    If MSG.Length > 0 Then
                        MSG &= ": "
                    End If
                    GLog.Discription = GLog.Discription.Trim.Replace(ControlChars.CrLf, " ").Replace(ControlChars.Cr, " ").Replace(ControlChars.Lf, " ")
                    MSG &= GLog.Discription
                    '****************************************
                End If

                If Not System.IO.File.Exists(sFileName) Then

                    'ファイルが無ければ作る
                    FS = New FileStream(sFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
                    FW = New StreamWriter(FS, EncdJ)
                Else
                    '*** 修正 mitsu 2008/08/04 ログ出力失敗対応 ***

                    '次シーケンスファイルが存在する間カウントアップ
                    While File.Exists(SET_PATH(GSYS.LogFolder) & GLog.Job & _
                        String.Format("{0:yyyyMMdd}", Date.Now) & "." & (FileSeq + 1).ToString("00000") & ".LOG")
                        FileSeq += 1

                        sFileName = SET_PATH(GSYS.LogFolder)
                        sFileName &= GLog.Job
                        sFileName &= String.Format("{0:yyyyMMdd}", Date.Now) & "." & FileSeq.ToString("00000") & ".LOG"
                    End While
                    '**********************************************

                    'ファイルを追加モードで開く
                    FS = New FileStream(sFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                    FW = New StreamWriter(FS, EncdJ)
                End If

                '*** 修正 mitsu 2008/09/08 User名にコンピュータ名を追加 ***
                '0 = ID番号(通番)
                '1 = 時間
                '2 = JOB通番(JOB管理Master)
                '3 = User名 + コンピュータ名
                '4 = 取引先コード（主／副）
                '5 = 振替日
                '6 = 処理内容
                '7 = ジョブ内容
                '8 = 結果
                '9 = 文言(FREE)
                Dim LineData As String = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", _
                        C34 & "" & C34, _
                        C34 & Date.Now.ToString("HHmmss") & C34, _
                        C34 & "Main" & C34, _
                        C34 & GSYS.UserID & " (" & Environment.MachineName & ")" & C34, _
                        C34 & GLog.ToriCode & C34, _
                        C34 & GLog.FuriDate & C34, _
                        C34 & GLog.Job1 & C34, _
                        C34 & GLog.Job2 & C34, _
                        C34 & GLog.Result & C34, _
                        C34 & MSG & C34)
                '**********************************************************

                FW.WriteLine(LineData)
                '*** 修正 mitsu 2008/08/04 ログ出力失敗対応 ***
                Exit While
                '**********************************************

            Catch ex As Exception
                '*** 修正 mitsu 2008/08/04 ログ出力失敗対応 ***
                'DRet = MessageBox.Show(ex.Message, New StackTrace(True).GetFrame(0).GetMethod.Name, _
                '       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FileSeq += 1
                ErrCnt += 1

                'エラー回数が3回を超えた場合は強制終了
                If ErrCnt >= 3 Then
                    DRet = MessageBox.Show(ex.Message, New StackTrace(True).GetFrame(0).GetMethod.Name, _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit While
                End If
                '**********************************************
            Finally
                If Not FW Is Nothing Then
                    FW.Close()
                    FW = Nothing
                End If
            End Try
            '*** 修正 mitsu 2008/08/04 ログ出力失敗対応 ***
        End While
        '**************************************************
    End Sub

'*** Str Add 2015/12/01 SO)荒木 for ログ強化（未使用メソッドなのでコメント化） ***
'    Public Sub LogWrite(ByVal Detail As String, ByVal Result As String, ByVal Message As String, Optional ByVal Log As BatchLOG = Nothing)
'        If MainLog Is Nothing AndAlso Log Is Nothing Then
'            Return
'        End If
'        If Not Log Is Nothing Then
'            MainLog = Log
'        End If
'        MainLog.Write(LogUserID, LogToriCode, LogFuriDate, Detail, Result, Message)
'    End Sub
'    Public Sub LogWrite(ByVal UserID As String, ByVal ToriCode As String, ByVal FuriDate As String, ByVal Detail As String, ByVal Result As String, ByVal Message As String, Optional ByVal Log As BatchLOG = Nothing)
'        If MainLog Is Nothing AndAlso Log Is Nothing Then
'            Return
'        End If
'        If Not Log Is Nothing Then
'            MainLog = Log
'        End If
'        MainLog.Write(UserID, ToriCode, FuriDate, Detail, Result, Message)
'    End Sub
'*** End Add 2015/12/01 SO)荒木 for ログ強化（未使用メソッドなのでコメント化） ***

    '
    ' 機能　　　: SQLエラーログ出力モジュール
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - リクエスト・モジュール格納ファイル名
    ' 　　　　　  ARG2 - リクエスト・モジュール
    ' 　　　　　  ARG3 - SQL ERROR ONLY
    '
    ' 備考　　　: 流用する
    '
    Public Sub FN_LOG_WRITE(ByVal avOnCallFileName As String, _
                    ByVal avOnCallModule As StackTrace, ByVal SEL As Short)
        Dim MSG As String
        Dim FS As FileStream
        Dim FW As StreamWriter = Nothing
        Const C34 As String = ControlChars.Quote
        Dim DRet As DialogResult
        Try
            Dim sFileName As String = SET_PATH(GSYS.LogFolder) & "SQLERROR" '& "."
            sFileName &= String.Format("{0:yyyyMMdd}", Date.Now) & ".LOG"

            '*** 修正 mitsu 2008/09/01 正常の場合はスタックトレースを出力しない ***
            'MSG = avOnCallFileName & ": "
            'MSG &= avOnCallModule.GetFrame(0).GetMethod.Name
            If GLog.Result = OK Then
                MSG = ""
            Else
                MSG = avOnCallFileName & ": "
                MSG &= avOnCallModule.GetFrame(0).GetMethod.Name
            End If
            '**********************************************************************

            If GLog.Discription.Trim.Length > 0 Then
                '*** 修正 mitsu 2008/09/01 処理高速化 ***
                'GLog.Discription = GLog.Discription.Replace(ControlChars.CrLf, "")
                'GLog.Discription = GLog.Discription.Replace(ControlChars.Cr, "")
                'GLog.Discription = GLog.Discription.Replace(ControlChars.Lf, "")
                'MSG &= ": " & GLog.Discription.Trim
                If MSG.Length > 0 Then
                    MSG &= ": "
                End If
                GLog.Discription = GLog.Discription.Trim.Replace(ControlChars.CrLf, " ").Replace(ControlChars.Cr, " ").Replace(ControlChars.Lf, " ")
                MSG &= GLog.Discription
                '****************************************
            End If

            If Not System.IO.File.Exists(sFileName) Then

                'ファイルが無ければ作る
                FS = New FileStream(sFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
                FW = New StreamWriter(FS, EncdJ)
            Else
                'ファイルを追加モードで開く
                FS = New FileStream(sFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                FW = New StreamWriter(FS, EncdJ)
            End If

            '*** 修正 mitsu 2008/09/08 User名にコンピュータ名を追加 ***
            '0 = ID番号(通番)
            '1 = 時間
            '2 = JOB通番(JOB管理Master)
            '3 = User名 + コンピュータ名
            '4 = 取引先コード（主／副）
            '5 = 振替日
            '6 = 処理内容
            '7 = ジョブ内容
            '8 = 結果
            '9 = 文言(FREE)
            Dim LineData As String = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", _
                    C34 & "" & C34, _
                    C34 & Date.Now.ToString("HHmmss") & C34, _
                    C34 & "Main" & C34, _
                    C34 & GSYS.UserID & " (" & Environment.MachineName & ")" & C34, _
                    C34 & GLog.ToriCode & C34, _
                    C34 & GLog.FuriDate & C34, _
                    C34 & GLog.Job1 & C34, _
                    C34 & GLog.Job2 & C34, _
                    C34 & GLog.Result & C34, _
                    C34 & MSG & C34)
            '**********************************************************

            FW.WriteLine(LineData)

        Catch ex As Exception
            DRet = MessageBox.Show(ex.Message, New StackTrace(True).GetFrame(0).GetMethod.Name, _
                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            If Not FW Is Nothing Then
                FW.Close()
                FW = Nothing
            End If
        End Try
    End Sub

    '
    ' 機　能 : パス調整
    '
    ' 戻り値 : 調整値
    '
    ' 引き数 : ARG1 - パス名
    ' 　　　   ARG2 - 調整文字
    ' 　　　   ARG3 - True = 付加, False = 削除
    '
    ' 備　考 : 汎用
    '    
    Public Function SET_PATH(ByVal ARG1 As String, _
        Optional ByVal ARG2 As String = "\", Optional ByVal ARG3 As Boolean = True) As String

        Dim Ret As String = NzStr(ARG1)
        Dim FLG As Boolean = Ret.EndsWith(ARG2)

        Select Case ARG3
            Case True
                If Not FLG Then
                    Ret &= ARG2
                End If
            Case Else
                If FLG Then
                    Ret = Ret.PadLeft(300).Substring(0, 299).Trim
                End If
        End Select

        Return Ret
    End Function

    '
    ' 機能　　　: テキストボックスで入力された日付データを評価する
    '
    ' 戻り値　　: 正常日付 = OK(-1)
    ' 　　　　　  異常日付 = SetFocusすべきTextBox(Index)値
    '
    ' 引き数　　: ARG1 - 日付
    ' 　　　　　  ARG2 - テキストボックス情報
    '
    ' 機能説明　: テキストボックス用のチェック関数
    '
    ' 備考　　　: 流用する
    '
    Public Function SET_DATE(ByRef onDate As Date, ByRef onText() As Integer) As Integer
        Try
            Select Case onText(0)
                Case 1900 To 2100
                    '敢えて範囲を指定する

                    Select Case onText(1)
                        Case 1 To 12
                            '日がエラー

                            onDate = DateSerial(onText(0), onText(1), onText(2))

                            If Not onDate.Year = onText(0) OrElse _
                                Not onDate.Month = onText(1) OrElse _
                                Not onDate.Day = onText(2) Then

                                Return 2
                            End If
                        Case Else
                            '月がエラー

                            Return 1
                    End Select
                Case Else
                    '年が対象外

                    Return 0
            End Select

            Return -1
        Catch ex As Exception
            With GLog
                .Job2 = "入力日付データの評価"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化

            '想定外エラー
            Return 9
        End Try
    End Function

    '
    ' 機能　　　: 日付データ文字列を評価して日付型に変換する
    '
    ' 戻り値　　: 正常日付 = OK(-1)
    ' 　　　　　  異常日付 = Nothing
    '
    ' 引き数　　: ARG1 - 日付
    ' 　　　　　  ARG2 - テキストボックス情報
    '
    ' 機能説明　: テキストボックス用のチェック関数
    '
    ' 備考　　　: 流用する
    '
    Public Function SET_DATE(ByRef onString As String) As DateTime
        Try
            Select Case NzDec(onString, "").Trim.Length
                Case 8
                    Dim onData() As Integer = {NzInt(onString.Substring(0, 4)), _
                        NzInt(onString.Substring(4, 2)), NzInt(onString.Substring(6)), 0, 0, 0}

                    If onData(0) + onData(1) + onData(2) > 0 Then
                        Return New DateTime(onData(0), onData(1), onData(2), onData(3), onData(4), onData(5))
                    End If
                Case 14
                    Dim onData() As Integer = {NzInt(onString.Substring(0, 4)), _
                        NzInt(onString.Substring(4, 2)), NzInt(onString.Substring(6, 2)), _
                        NzInt(onString.Substring(8, 2)), NzInt(onString.Substring(10, 2)), _
                        NzInt(onString.Substring(12))}

                    If onData(0) + onData(1) + onData(2) > 0 Then
                        Return New DateTime(onData(0), onData(1), onData(2), onData(3), onData(4), onData(5))
                    End If
            End Select
        Catch ex As Exception
            With GLog
                .Job2 = "日付データの日付型変換"
                .Result = NG
                .Discription = "(" & onString & ") " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Nothing
    End Function

    '
    ' 機能　 ： 営業日判定関数
    '
    ' 引数　 ： ARG1(onDate)    - 評価する日付
    ' 　　　 　 ARG2(SEL)       - 評価 = 0, 蓄積 = Else
    ' 　　　 　 ARG3(SubSQL)    - 蓄積時の条件文
    '
    ' 戻り値 ： 休日指定日  = False
    ' 　　　 　 営業日      = True
    '
    ' 備考　 ： なし
    '
    Public Function CheckDateModule(ByVal onDate As Date, _
                Optional ByVal SEL As Short = 0, Optional ByVal SubSQL As String = "") As Boolean
        If SEL = 0 Then
            '評価
            Try
                If onDate.DayOfWeek = System.DayOfWeek.Saturday OrElse _
                   onDate.DayOfWeek = System.DayOfWeek.Sunday Then
                    '土日の場合は非営業日
                    Return False
                Else
                    Dim Temp As Integer = NzInt(String.Format("{0:yyyyMMdd}", onDate))
                    '休日登録が検知できない場合は営業日とする
                    For Index As Integer = 1 To Data.GetUpperBound(0) Step 1
                        Select Case Data(Index)
                            Case Is = Temp
                                '同一値は休日の証
                                Return False
                            Case Is > Temp
                                '検査日より未来になれば終了
                                Exit For
                        End Select
                    Next Index

                    Return True
                End If
            Catch ex As Exception
                With GLog
                    .Job2 = "営業日判定関数(評価)"
                    .Result = NG
                    .Discription = ex.Message
                End With
                'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            End Try
        Else
            '蓄積
            Dim REC As OracleDataReader = Nothing
            Try
                Dim Ret As Boolean
                Dim SQL As String
                Dim Cnt As Integer = 0

                SQL = "SELECT COUNT(*) COUNTER"
                SQL &= " FROM YASUMIMAST"
                SQL &= SubSQL

                Ret = SetDynaset(SQL, REC)
                If Ret AndAlso REC.Read Then

                    Cnt = NzInt(REC.Item("COUNTER"), 0)
                End If
                If Not REC Is Nothing Then
                    REC.Close()
                    REC.Dispose()
                End If

                ReDim Data(Cnt)
                Data(0) = 0

                If Cnt = 0 Then
                    Return True
                Else
                    SQL = "SELECT YASUMI_DATE_Y"
                    SQL &= " FROM YASUMIMAST"
                    SQL &= SubSQL
                    SQL &= " ORDER BY YASUMI_DATE_Y ASC"

                    Ret = SetDynaset(SQL, REC)
                    If Ret Then
                        For Index As Integer = 1 To Cnt Step 1
                            If REC.Read Then
                                Data(Index) = NzInt(REC.Item("YASUMI_DATE_Y"), 0)
                            Else
                                Exit For
                            End If
                        Next Index
                        Return True
                    End If
                End If
            Catch ex As Exception
                With GLog
                    .Job2 = "営業日判定関数(蓄積)"
                    .Result = NG
                    .Discription = ex.Message
                End With
                'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            Finally
                If Not REC Is Nothing Then
                    REC.Close()
                    REC.Dispose()
                End If
            End Try

            Return False
        End If
    End Function

    '
    ' 機能　 ： 営業日判定関数
    '
    ' 引数　 ： ARG1(aonDate)        - 評価する日付
    ' 　　　 　 ARG2(aOpenDate)      - 評価後の日付
    ' 　　　 　 ARG3(aFrontBackType) - 0    = 前方へスライドする(時間が進む)
    ' 　　　 　                        Else = 後方へスライドする(時間が戻る)
    '
    ' 戻り値 ： aonDate<>歴日 = False
    ' 　　　 　 aonDate=歴日=営業日 = True
    ' 　　　 　 aonDate=歴日<>営業日 = False
    '
    ' 備考　 ： 引数Target日付は営業日判定され、非営業日の場合にはaOpenDateに前後の営業日をセットする
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aFrontBackType As Integer) As Boolean

        aOpenDate = aTgetDate               '引数初期化
        Dim ReturnValue As Boolean = False  'aonDateが営業日か否かの判定(戻り値)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '日付変数

            'そもそも日付評価できない場合にはダメ
            If Not onDate = Nothing Then

                Dim LoopCounter As Integer = 0              'ループ処理の回数
                Do
                    '２回目以降は日付をスライドさせる
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType
                            Case Is = 0
                                '前方へスライドする(時間が進む)
                                onDate = onDate.AddDays(1)
                            Case Else
                                '後方へスライドする(時間が戻る)
                                onDate = onDate.AddDays(-1)
                        End Select
                    End If

                    '営業日判定(営業日であればループ終了)
                    If CheckDateModule(onDate) = True Then

                        Exit Do
                    End If

                    LoopCounter += 1
                Loop While LoopCounter <= 366

                '引数の営業日日付を設定する
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)

                ReturnValue = (aOpenDate = aTgetDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "単純営業日判定関数"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
        Return ReturnValue
    End Function

    '
    ' 機能　 ： 営業日算出関数
    '
    ' 引数　 ： ARG1(aonDate)        - 評価する日付
    ' 　　　 　 ARG2(aOpenDate)      - 評価後の日付
    ' 　　　 　 ARG3(aDayTeam)       - スライドする日数(標準値=1営業日)
    ' 　　　 　 ARG4(aFrontBackType) - 0    = 前方へスライドする(時間が進む:標準値)
    ' 　　　 　                        Else = 後方へスライドする(時間が戻る)
    '
    ' 戻り値 ： aonDate<>歴日 = False
    ' 　　　 　 aonDate=歴日=営業日 = True
    ' 　　　 　 aonDate=歴日<>営業日 = False
    '
    ' 備考　 ： 引数Target日付は常に営業日判定するのだ
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aDayTeam As Integer, _
                                    ByVal aFrontBackType As Integer) As Boolean

        aOpenDate = aTgetDate               '引数初期化
        Dim ReturnValue As Boolean = False  'aonDateが営業日か否かの判定(戻り値)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '日付変数

            'そもそも日付評価できない場合にはダメ
            If Not onDate = Nothing Then

                Dim DayTeamCounter As Integer = 0           '営業日経過カウンター
                Dim LoopCounter As Integer = 0              'ループ処理の回数
                Do
                    '２回目以降は日付をスライドさせる
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType
                            Case Is = 0
                                '前方へスライドする(時間が進む)
                                onDate = onDate.AddDays(1)
                            Case Else
                                '後方へスライドする(時間が戻る)
                                onDate = onDate.AddDays(-1)
                        End Select
                    End If

                    '土日以外で休日登録が検知できない場合は営業日とする
                    If CheckDateModule(onDate) = True Then
                        '
                        If LoopCounter = 0 Then
                            '初回だけ設定する(初回は営業日を期待する)

                            ReturnValue = True
                        Else
                            '２回目以降は営業日日数カウンターを繰り上げる
                            DayTeamCounter += 1
                        End If
                    End If

                    LoopCounter += 1

                    'ＸＸ営業日数を充足すれば終了する
                Loop While DayTeamCounter < aDayTeam

                '引数のＸＸ営業日後日付を設定する
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "前後営業日算出関数"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
        Return ReturnValue
    End Function

    '
    ' 機能　 ： 営業日判定関数
    '
    ' 引数　 ： ARG1(aonDate)        - 評価する日付
    ' 　　　 　 ARG2(aOpenDate)      - 評価後の日付
    ' 　　　 　 ARG3(aFrontBackType) - 0    = 前方へスライドする(時間が進む)
    ' 　　　 　                        Else = 後方へスライドする(時間が戻る)
    '
    ' 戻り値 ： aonDate<>歴日 = False
    ' 　　　 　 aonDate=歴日=営業日 = True
    ' 　　　 　 aonDate=歴日<>営業日 = False
    '
    ' 備考　 ： 引数Target日付は営業日判定され、非営業日の場合にはaOpenDateに前後の営業日をセットする
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aFrontBackType As String) As Boolean

        aOpenDate = aTgetDate               '引数初期化
        Dim ReturnValue As Boolean = False  'aonDateが営業日か否かの判定(戻り値)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '日付変数

            'そもそも日付評価できない場合にはダメ
            If Not onDate = Nothing Then

                Dim LoopCounter As Integer = 0              'ループ処理の回数
                Do
                    '２回目以降は日付をスライドさせる
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType.ToUpper
                            Case Is = "BACK"
                                '後方へスライドする(時間が戻る)
                                onDate = onDate.AddDays(-1)
                            Case Else
                                '前方へスライドする(時間が進む)
                                onDate = onDate.AddDays(1)
                        End Select
                    End If

                    '営業日判定(営業日であればループ終了)
                    If CheckDateModule(onDate) = True Then

                        Exit Do
                    End If

                    LoopCounter += 1
                Loop While LoopCounter <= 366

                '引数の営業日日付を設定する
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)

                ReturnValue = (aOpenDate = aTgetDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "単純営業日判定関数"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
        Return ReturnValue
    End Function

    '
    ' 機能　 ： 営業日算出関数
    '
    ' 引数　 ： ARG1(aonDate)        - 評価する日付
    ' 　　　 　 ARG2(aOpenDate)      - 評価後の日付
    ' 　　　 　 ARG3(aDayTeam)       - スライドする日数(標準値=1営業日)
    ' 　　　 　 ARG4(aFrontBackType) - 0    = 前方へスライドする(時間が進む:標準値)
    ' 　　　 　                        BACK = 後方へスライドする(時間が戻る)
    '
    ' 戻り値 ： aonDate<>歴日 = False
    ' 　　　 　 aonDate=歴日=営業日 = True
    ' 　　　 　 aonDate=歴日<>営業日 = False
    '
    ' 備考　 ： 引数Target日付は常に営業日判定するのだ
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aDayTeam As Integer, _
                                    ByVal aFrontBackType As String) As Boolean

        aOpenDate = aTgetDate               '引数初期化
        Dim ReturnValue As Boolean = False  'aonDateが営業日か否かの判定(戻り値)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '日付変数

            'そもそも日付評価できない場合にはダメ
            If Not onDate = Nothing Then

                Dim DayTeamCounter As Integer = 0           '営業日経過カウンター
                Dim LoopCounter As Integer = 0              'ループ処理の回数
                Do
                    '２回目以降は日付をスライドさせる
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType.ToUpper
                            Case Is = "BACK"
                                '後方へスライドする(時間が戻る)
                                onDate = onDate.AddDays(-1)
                            Case Else
                                '前方へスライドする(時間が進む)
                                onDate = onDate.AddDays(1)
                        End Select
                    End If

                    '土日以外で休日登録が検知できない場合は営業日とする
                    If CheckDateModule(onDate) = True Then
                        '
                        If LoopCounter = 0 Then
                            '初回だけ設定する(初回は営業日を期待する)

                            ReturnValue = True
                        Else
                            '２回目以降は営業日日数カウンターを繰り上げる
                            DayTeamCounter += 1
                        End If
                    End If

                    LoopCounter += 1

                    'ＸＸ営業日数を充足すれば終了する
                Loop While DayTeamCounter < aDayTeam

                '引数のＸＸ営業日後日付を設定する
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "前後営業日算出関数"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
        Return ReturnValue
    End Function

    '
    ' 機　能 : 明細行の詳細表示
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 対象コントロール
    '
    ' 備　考 : 明細行参照時の汎用的な関数
    '    
    Public Sub MonitorCsvFile(ByVal ListView As ListView)

        '2008.05.01 By Astar 機能廃止
        '*** 2008/5/14 kakinoki 機能復帰 ***
        'Return

        Dim FL As FileStream
        Try
            Dim Arguments As String = SET_PATH(System.IO.Path.GetTempPath)
            Arguments &= String.Format("{0:yyyy.MM.dd.hh.mm.ss}", Date.Now) & ".csv"

            FL = New FileStream(Arguments, FileMode.CreateNew)

            Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")

            For Col As Integer = 0 To ListView.Columns.Count - 1 Step 1
                Dim onString As String = ""

                onString = ListView.Columns(Col).Text
                Dim onByte() As Byte = JISEncoding.GetBytes(onString)
                Do While onByte.Length <= 12
                    onString &= Space(1)
                    onByte = JISEncoding.GetBytes(onString)
                Loop
                onString &= ControlChars.Tab
                onString &= SelectedItem(ListView, Col).ToString
                onString &= ControlChars.CrLf

                onByte = JISEncoding.GetBytes(onString)
                FL.Write(onByte, 0, onByte.Length)
            Next Col

            FL.Close()
            FL = Nothing

            Dim PSI As New System.Diagnostics.ProcessStartInfo

            Dim FileName As String = SET_PATH(Environment.SystemDirectory)
            FileName &= "notepad.exe"

            With PSI
                .FileName = FileName
                .CreateNoWindow = True
                .Arguments = Arguments
            End With

            Dim PS As Process = Process.Start(PSI)
            With PS
                .WaitForExit(IntWaitTimer)

                If Not .HasExited Then
                    .Kill()
                End If
                .Dispose()
            End With
        Catch ex As Exception
            With GLog
                .Job2 = "明細行の詳細表示"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        Finally
            FL = Nothing
        End Try
    End Sub

    '
    ' 機　能 : 対象値の参照
    '
    ' 戻り値 : 参照値
    '
    ' 引き数 : ARG1 - 対象コントロール
    ' 　　　   ARG2 - 対象列
    '
    ' 備　考 : 明細行参照時の専用関数
    '    
    Public Function SelectedItem(ByVal ListView As ListView, ByVal avItemIndex As Integer) As Object
        SelectedItem = Nothing
        Try
            Dim BreakFast As ListView.SelectedListViewItemCollection = ListView.SelectedItems

            If BreakFast.Count = 0 Then
                SelectedItem = Nothing
                Exit Function
            End If

            If (avItemIndex = 0) Then
                SelectedItem = Trim(BreakFast.Item(avItemIndex).Text)
            Else
                Dim lsvItem As ListViewItem.ListViewSubItemCollection = ListView.SelectedItems.Item(0).SubItems
                SelectedItem = Trim(lsvItem.Item(avItemIndex).Text)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "対象値の参照"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Function

    '
    ' 機　能 : 対象値の設定
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 対象コントロール
    ' 　　　   ARG2 - 対象列
    ' 　　　   ARG3 - 対象列の値
    '
    ' 備　考 : 明細行参照時の専用関数
    '    
    Public Sub SelectedItem(ByVal ListView As ListView, _
                        ByVal avItemIndex As Integer, ByVal avItemValue As String)
        Try
            Dim BreakFast As ListView.SelectedListViewItemCollection = ListView.SelectedItems

            If BreakFast.Count = 0 Then

                Return
            End If

            If (avItemIndex = 0) Then

                BreakFast.Item(avItemIndex).Text = avItemValue
            Else
                Dim lsvItem As ListViewItem.ListViewSubItemCollection = ListView.SelectedItems.Item(0).SubItems

                lsvItem.Item(avItemIndex).Text = avItemValue
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "対象値の設定"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Sub

    '
    ' 機　能 : 表示領域の有効表示行数
    '
    ' 戻り値 : 行数
    '
    ' 引き数 : ARG1 - 対象コントロール
    '
    ' 備　考 : 明細行参照時の専用関数
    '    
    Public Function GetListViewHasRow(ByVal ListView As ListView) As Integer
        Try
            Dim BreakFast As ListView.SelectedListViewItemCollection = ListView.SelectedItems

            GetListViewHasRow = BreakFast.Count

        Catch ex As Exception
            With GLog
                .Job2 = "表示領域の有効表示行数"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Function

    '
    ' 機　能 : 画面無効化／有効化
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 対象画面
    ' 　　　   ARG2 - 有効／無効
    '
    ' 備　考 : 画面遷移時の制御関数
    '    
    Public Sub SetFormEnabled(ByVal onForm As Form, Optional ByVal SEL As Boolean = True)
        Try
            Dim CTL As Control
            For Each CTL In onForm.Controls
                CTL.Enabled = SEL
            Next
        Catch ex As Exception
            With GLog
                .Job2 = "画面無効化／有効化"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Sub

    '
    ' 機　能 : 該当ComboBoxの項目設定
    '
    ' 戻り値 : なし >結果(Integer 0:成功 1:ファイルなし 2:失敗)2009/09/10 T-Sakai 追加
    '
    ' 引き数 : ARG1 - ComboBox Object
    ' 　　　   ARG2 - ファイル名
    ' 　　　   ARG3 - インデックスを最初の位置に設定する
    ' 　　　   ARG4 - ファイルの読み方
    '
    ' 備　考 : コンボボックス共通関数
    '    
    Public Function SetComboBox(ByVal aComboBox As ComboBox, ByVal aFileName As String, ByVal aIndex As Boolean) As Integer
        Dim FL As StreamReader = Nothing
        Try
            aComboBox.Items.Clear()

            Dim FileName As String = SET_PATH(GSYS.TXTFolder) & aFileName
            If System.IO.File.Exists(FileName) Then

                FL = New StreamReader(FileName, EncdJ)

                Dim LineData As String = FL.ReadLine
                Do While Not LineData Is Nothing

                    Dim Data() As String = LineData.Split(","c)
                    If Data.Length >= 2 Then

                        Dim Item As New clsAddItem(Data(1).Trim, NzInt(Data(0)))
                        aComboBox.Items.Add(Item)
                    End If

                    LineData = FL.ReadLine
                Loop
                FL.Close()

                If aIndex AndAlso aComboBox.Items.Count > 0 Then
                    aComboBox.SelectedIndex = 0
                End If
            Else

                Return 1 'ファイルなし 2009/09/10 T-Sakai 追加
            End If
            Return 0 '正常終了 2009/09/10 T-Sakai追加
        Catch ex As Exception
            With GLog
                .Job2 = "該当ComboBoxの項目設定"
                .Result = NG
                .Discription = aComboBox.Name & " : " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            Return 2 '異常終了 2009/09/10 T-Sakai追加
        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Function

    '
    ' 機　能 : 該当ComboBoxの項目設定
    '
    ' 戻り値 : なし >結果(Integer 0:成功 1:ファイルなし 2:失敗)2009/09/10 T-Sakai 追加 
    '
    ' 引き数 : ARG1 - ComboBox Object
    ' 　　　   ARG2 - ファイル名
    ' 　　　   ARG3 - インデックスを最初の位置に設定する
    ' 　　　   ARG4 - ファイルの読み方
    '
    ' 備　考 : コンボボックス共通関数
    '    
    Public Function SetComboBox(ByVal aComboBox As ComboBox, ByVal aFileName As String, ByVal aItemData As Integer) As Integer
        Dim FL As StreamReader = Nothing
        Try
            aComboBox.Items.Clear()

            Dim FileName As String = SET_PATH(GSYS.TXTFolder) & aFileName
            If System.IO.File.Exists(FileName) Then

                FL = New StreamReader(FileName, EncdJ)

                Dim LineData As String = FL.ReadLine
                Do While Not LineData Is Nothing

                    Dim Data() As String = LineData.Split(","c)
                    If Data.Length >= 2 Then

                        Dim Item As New clsAddItem(Data(1).Trim, NzInt(Data(0)))
                        aComboBox.Items.Add(Item)
                    End If

                    LineData = FL.ReadLine
                Loop
                FL.Close()
                Application.DoEvents()

                Dim Cnt As Integer
                For Cnt = 0 To aComboBox.Items.Count - 1 Step 1

                    aComboBox.SelectedIndex = Cnt

                    If GetComboBox(aComboBox) = aItemData Then
                        Exit For
                    End If
                Next Cnt

                If Cnt >= aComboBox.Items.Count AndAlso aComboBox.Items.Count > 0 Then

                    aComboBox.SelectedIndex = -1
                End If
            Else

                Return 1 'ファイルなし 2009/09/10 T-Sakai 追加
            End If
            Return 0 '正常終了 2009/09/10 T-Sakai追加
        Catch ex As Exception
            With GLog
                .Job2 = "該当ComboBoxの項目設定"
                .Result = NG
                .Discription = aComboBox.Name & " : " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            Return 2 '異常終了 2009/09/10 T-Sakai追加
        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Function

    '
    ' 機　能 : 該当INDEXの項目参照
    '
    ' 戻り値 : 最初の数値データ
    '
    ' 引き数 : ARG1 - ComboBox Object
    '
    ' 備　考 : コンボボックス共通関数
    '    
    Public Function GetComboBox(ByVal aComboBox As ComboBox) As Integer
        Try
            If Not aComboBox.SelectedItem Is Nothing AndAlso aComboBox.Items.Count > 0 Then
                Dim SelItem As clsAddItem = CType(aComboBox.SelectedItem, clsAddItem)
                Return SelItem.Data1
            Else
                Return -1
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "該当INDEXの項目参照"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Function

    '
    ' 機　能 : コンボボックスから，必要のないアイテムを削除する
    '
    ' 引き数 : ARG1 - コンボボックス
    '          ARG2 - 比較配列（許可する番号を登録）
    '
    ' 備　考 : 比較配列に一致しないアイテムを削除する
    '   
    Public Sub RemoveComboItem(ByVal combo As ComboBox, ByVal dataarray As Array)
        Dim RemoveArray As New ArrayList        ' 削除リスト

        For Each oItem As Object In combo.Items
            Dim dat As MenteCommon.clsAddItem = CType(oItem, clsAddItem)
            If Array.IndexOf(dataarray, dat.Data1) = -1 Then
                RemoveArray.Add(oItem)
            End If
        Next oItem
        For i As Integer = 0 To RemoveArray.Count - 1
            combo.Items.Remove(RemoveArray.Item(i))
        Next i
    End Sub

    '
    ' 機　能 : 指定長内の文字列を返す
    '
    ' 戻り値 : 文字列データ
    '
    ' 引き数 : ARG1 - 評価対象値
    ' 　　　   ARG2 - 指定長(Byte)
    '
    ' 備　考 : 漢字交じりの文字列評価
    '    
    Public Function GetLimitString(ByVal avTargetData As String, ByVal avLength As Integer) As String
        Try
            Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")
            Dim onByte() As Byte = JISEncoding.GetBytes(avTargetData)

            Do While onByte.Length > avLength
                avTargetData = avTargetData.Substring(0, avTargetData.Length - 1)
                onByte = JISEncoding.GetBytes(avTargetData)
            Loop
            Return avTargetData
        Catch ex As Exception
            With GLog
                .Job2 = "指定長内の文字列を返す"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            Return ""
        End Try
    End Function

    '
    ' 機　能 : TextBoxの文字列値を評価する
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 評価対象テキストボックス
    '
    ' 備　考 : 半角入力領域に用いる
    '    
    Public Sub NzCheckString(ByVal avTextBox As TextBox)
        Dim Ret As String = ""
        Try
            With avTextBox
                If Not IsNothing(.Text) Then

                    Dim Temp As String = StrConv(.Text, VbStrConv.Narrow)

                    For Idx As Integer = 0 To Temp.Length - 1 Step 1

                        Ret &= GetLimitString(Temp.Substring(Idx, 1), 1)
                    Next Idx
                End If
            End With
        Catch ex As Exception
            With GLog
                .Job2 = "TextBoxの文字列値を評価する"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        Finally
            avTextBox.Text = Ret
        End Try
    End Sub

    '
    ' 機　能 : TextBoxの整数値を評価する
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 評価対象値
    ' 　　　   ARG2 - Format する／しない
    '
    ' 備　考 : 画面で使用
    '    
    Public Sub NzNumberString(ByVal avTextBox As TextBox, Optional ByVal avSEL As Boolean = False)
        Try
            With avTextBox
                If IsNothing(.Text) OrElse Not IsNumber(.Text) Then
                    .Text = ""
                Else
                    Select Case avSEL
                        Case True
                            Dim Temp As String = New String("0"c, .MaxLength)
                            .Text = String.Format("{0:" & Temp & "}", NzDec(.Text))
                        Case Else
                            .Text = NzDec(.Text).ToString
                    End Select
                End If
            End With
        Catch ex As Exception
            With GLog
                .Job2 = "TextBoxの整数値を評価する"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Sub

    '
    ' 機　能 : 整数値を評価する
    '
    ' 戻り値 : 整数値
    '
    ' 引き数 : ARG1 - 評価対象値
    '
    ' 備　考 : テキスト処理系関数
    '    
    Public Function NzInt(ByVal avValue As Object) As Integer
        Dim Ret As Integer = 0
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) AndAlso IsNumber(avValue) Then

                Ret = CType(avValue, Integer)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "整数値を評価する"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 整数値評価
    '
    ' 戻り値 : 評価後の整数値
    '
    ' 引き数 : ARG1 - 評価対象値
    ' 　　　   ARG2 - 桁数
    '
    ' 備　考 : 画面系専用関数
    '    
    Public Function NzInt(ByVal AnyObject As Object, ByRef afSEL As Integer) As Integer
        Dim Ret As Integer = 0
        afSEL = 0
        Try
            If Not IsDBNull(AnyObject) AndAlso Not IsNothing(AnyObject) Then

                Dim Temp As String = ""
                Dim CharArray() As Char = AnyObject.ToString.ToCharArray()
                For Each c As Char In CharArray
                    If Char.IsNumber(c) Then
                        afSEL += 1
                        Temp &= c.ToString
                    End If
                Next

                If Temp.Length = 0 Then
                    Return Nothing
                Else
                    Ret = CType(Temp, Integer)
                End If
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "整数値評価"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 長整数値を評価する
    '
    ' 戻り値 : 長整数値
    '
    ' 引き数 : ARG1 - 評価対象値
    '
    ' 備　考 : テキスト処理系関数
    '    
    Public Function NzLong(ByVal avValue As Object) As Long
        Dim Ret As Long = 0
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) AndAlso IsNumber(avValue) Then

                Ret = CType(avValue, Long)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "長整数値"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 長整数値評価
    '
    ' 戻り値 : 評価後の長整数値
    '
    ' 引き数 : ARG1 - 評価対象値
    ' 　　　   ARG2 - 桁数
    '
    ' 備　考 : 画面系専用関数
    '    
    Public Function NzLong(ByVal AnyObject As Object, ByRef afSEL As Integer) As Long
        Dim Ret As Long = 0
        afSEL = 0
        Try
            If Not IsDBNull(AnyObject) AndAlso Not IsNothing(AnyObject) Then

                Dim Temp As String = ""
                Dim CharArray() As Char = AnyObject.ToString.ToCharArray()
                For Each C As Char In CharArray
                    If Char.IsNumber(C) Then
                        afSEL += 1
                        Temp &= C.ToString
                    End If
                Next

                If Temp.Length = 0 Then
                    Return Nothing
                Else
                    Ret = CType(Temp, Long)
                End If
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "長整数値評価"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 数値を評価する
    '
    ' 戻り値 : 数値
    '
    ' 引き数 : ARG1 - 評価対象値
    '
    ' 備　考 : テキスト処理系関数
    '    
    Public Function NzDec(ByVal avValue As Object) As Decimal
        Dim Ret As Decimal = 0
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) AndAlso IsNumber(avValue) Then

                Ret = CType(avValue, Decimal)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "数値を評価する"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 数値評価
    '
    ' 戻り値 : 評価後の数値
    '
    ' 引き数 : ARG1 - 評価対象値
    ' 　　　   ARG2 - 桁数
    '
    ' 備　考 : 画面系専用関数
    '    
    Public Function NzDec(ByVal AnyObject As Object, ByRef afSEL As Integer) As Decimal
        Dim Ret As Decimal = 0
        afSEL = 0
        Try
            If Not IsDBNull(AnyObject) AndAlso Not IsNothing(AnyObject) Then

                Dim Temp As String = ""
                Dim CharArray() As Char = AnyObject.ToString.ToCharArray()
                For Each c As Char In CharArray
                    If Char.IsNumber(c) Then
                        afSEL += 1
                        Temp &= c.ToString
                    End If
                Next
                If Temp.Length = 0 Then
                    Ret = Nothing
                Else
                    Ret = CType(Temp, Decimal)
                End If
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "数値評価"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 数値評価
    '
    ' 戻り値 : 評価後の文字列型の数値情報
    '
    ' 引き数 : ARG1 - 評価対象値
    ' 　　　   ARG2 - 桁数
    '
    ' 備　考 : 画面系専用関数
    '    
    Public Function NzDec(ByVal AnyObject As Object, ByRef afSEL As String) As String
        Dim Ret As String = ""
        Try
            If Not IsDBNull(AnyObject) AndAlso Not IsNothing(AnyObject) Then

                Dim CharArray() As Char = AnyObject.ToString.ToCharArray()
                For Each c As Char In CharArray
                    If Char.IsNumber(c) Then

                        Ret &= StrConv(c.ToString, VbStrConv.Narrow)
                    End If
                Next
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "数値評価"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 文字列値を評価する
    '
    ' 戻り値 : 文字列値
    '
    ' 引き数 : ARG1 - 評価対象値
    '
    ' 備　考 : 汎用的に使う
    '    
    Public Function NzStr(ByVal avValue As Object) As String
        Dim Ret As String = Space$(0)
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) Then

                Ret = avValue.ToString
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "文字列値を評価する"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機　能 : 許可された文字列でフィルタをかける
    '
    ' 戻り値 : 文字列値
    '
    ' 引き数 : ARG1 - 許可対象文字情報
    ' 　　　   ARG2 - 評価対象値
    '
    ' 備　考 : 汎用的に使う(主に郵便番号)
    '    
    Public Function NzAny(ByVal ARG1 As Object, ByVal ARG2 As Object) As String
        Dim Ret As String = Space$(0)
        Try
            If Not IsDBNull(ARG2) AndAlso Not IsNothing(ARG2) AndAlso Not IsNothing(ARG1) Then
                Dim OKCharArray() As Char = ARG1.ToString.ToCharArray()
                Dim TargetCharArray() As Char = ARG2.ToString.ToCharArray()
                For Each TARGET As Char In TargetCharArray
                    For Each OK As Char In OKCharArray
                        If TARGET = OK Then
                            Ret &= TARGET.ToString
                        End If
                    Next OK
                Next TARGET
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "許可された文字列でフィルタをかける"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
        Return Ret
    End Function

    '
    ' 機能　 ： プリンタ存在確認
    '
    ' 引数　 ： ARG1 - モジュール標題
    '
    ' 戻り値 ： プリンタ有無
    '
    ' 備考　 ： 印刷系で必要となる関数（暫定的に格納する）2007.10.05 By K.Seto
    '
    Public Function GetPrinters(ByVal TopErr As String) As Integer
        Dim Ret As Integer = 0
        Dim Printers As String
        Try
            For Each Printers In Printing.PrinterSettings.InstalledPrinters
                Ret += 1
            Next
            Return Ret
        Catch ex As Exception
            Dim MSG As String = "利用可能なプリンターがありません。"
            MSG &= " ： " & ex.Message
            With GLog
                .Job2 = "プリンタ存在確認"
                .Result = NG
                .Discription = MSG
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            Return 0
        End Try
    End Function

    '
    ' 機能　 ： 桁数調整
    '
    ' 引数　 ： ARG1 - 対象値
    ' 　　　 　 ARG2 - 調整後桁数
    ' 　　　 　 ARG3 - スペース(半角／全角)
    ' 　　　 　 ARG4 - 数値識別(左右)
    '
    ' 戻り値 ： 抽出レコード数
    '
    ' 備考　 ： フォーム内変数へ設定
    ' 　　　 　 印刷系で必要となる関数（暫定的に格納する）2007.10.05 By K.Seto
    '
    Public Function SetCol(ByVal TargetData As String, ByVal MaxLength As Integer, _
            Optional ByVal onSpace As Integer = 0, Optional ByVal SEL As Integer = 0) As String
        SetCol = ""
        Try
            SetCol = Trim(TargetData)
            If Not SEL = 0 Then
                SetCol = Format(CDec(SetCol), "#,##0")
            End If

            If Len(SetCol) > MaxLength Then
                SetCol = Mid(TargetData, 1, MaxLength)
            Else
                Do While Len(SetCol) < MaxLength
                    If SEL = 0 Then
                        If onSpace = 0 Then
                            SetCol &= " "
                        Else
                            SetCol &= "　"
                        End If
                    Else
                        SetCol = " " & SetCol
                    End If
                Loop
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "桁数調整"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Function

    '
    ' 機能　 ： 数値評価
    '
    ' 引数　 ： ARG1 - 対象値
    '
    ' 戻り値 ： 数値 = True
    '
    ' 備考　 ： なし
    '
    Public Function IsNumber(ByVal avValue As Object) As Boolean
        Try
            Return (New System.Text.RegularExpressions.Regex("^[-]*\d+$")).IsMatch(avValue.ToString)

        Catch ex As Exception
            With GLog
                .Job2 = "数値評価"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
    End Function

    '
    ' 機　能 : 全銀指定文字チェック
    '
    ' 戻り値 : 全部規定文字の場合 　= True
    ' 　　　   規定外文字がある場合 = False
    '
    ' 引き数 : ARG1 - 評価対象文字列
    '
    ' 備　考 : 変換出来る文字は変換してしまう
    '    
    Public Function CheckZenginChar(ByVal avTextBox As TextBox) As Boolean
        Dim BRet As Boolean = True
        Try
            Dim Chars() As Char = StrConv(avTextBox.Text, VbStrConv.Narrow).ToUpper.ToCharArray()
            Dim GetString As String = ""
            For Each C As Char In Chars
                Select Case C.ToString
                    Case "ｧ"
                        C = "ｱ"c
                    Case "ｨ"
                        C = "ｲ"c
                    Case "ｩ"
                        C = "ｳ"c
                    Case "ｪ"
                        C = "ｴ"c
                    Case "ｫ"
                        C = "ｵ"c
                    Case "ｬ"
                        C = "ﾔ"c
                    Case "ｭ"
                        C = "ﾕ"c
                    Case "ｮ"
                        C = "ﾖ"c
                    Case "ｯ"
                        C = "ﾂ"c
                    Case "ｰ"
                        C = "-"c
                    Case "A" To "Z"
                    Case "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", _
                         "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", _
                         "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", _
                         "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", _
                         "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", _
                         "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", _
                         "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", _
                         "ﾔ", "ﾕ", "ﾖ", _
                         "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", _
                         "ﾜ", "ｦ", "ﾝ"
                    Case "ﾞ", "ﾟ", " "
                    Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                        '*** 修正 mitsu 2009/04/17 *&$は規定外文字 ***
                    Case "\", ",", ".", "｢", "｣", "-", "/", "(", ")"
                        '*********************************************
                    Case Else
                        'エラー文字
                        BRet = False
                End Select
                'チェック済文字列を蓄積
                GetString = GetString + C.ToString
            Next
            'TextBox値を上書きする
            avTextBox.Text = GetString
        Catch ex As Exception
            BRet = False
            With GLog
                .Job2 = "全銀指定文字チェック"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try
        '結果返却
        Return BRet
    End Function

    '--------------------------------------------------------------------------------------------------
    '' データベース制御関連関数群
    '--------------------------------------------------------------------------------------------------

    '
    ' 機能　　 : カーソルを形成する
    '
    ' 戻り値　 : ORACLE Reader Object
    '
    ' 引き数　 : ARG1 - SQL String
    '
    ' 備考　　 : 流用する
    '
    Public Function SetDynaset(ByVal SQL As String, ByRef oraReader As OracleDataReader) As Boolean
        Dim BRet As Boolean = False

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = MainLog.Write_Enter3("clsCommon.SetDynaset")
            If IS_LEVEL4 = True Then
                MainLog.Write_LEVEL4("clsCommon.SetDynaset", "呼出", "呼出しスタック: " & Environment.StackTrace)
            End If
        End If
        If IS_SQLLOG = True Then
            MainLog.Write_SQL("clsCommon.SetDynaset", SQL)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Try
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            If Not GCOM_MainDB Is Nothing Then
                oraReader = GCOM_MainDB.getOracleDataReader(SQL)
                BRet = oraReader.HasRows

            Else
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

                If ConnectDataBase() Then

                    If Tran Is Nothing Then
                        oraReader = New OracleClient.OracleCommand(SQL, OraConnect).ExecuteReader
                    Else
                        oraReader = New OracleClient.OracleCommand(SQL, OraConnect, Tran).ExecuteReader
                    End If

                    BRet = oraReader.HasRows
                End If

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

        Catch ex As OracleException
            '*** Str Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader.Dispose()
                oraReader = Nothing
            End If
            '*** End Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***

            With GLog
                .Job2 = ex.Message.Replace(",", " ")
                .Result = NG
                .Discription = SQL.Replace(",", ":")
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True), 0) '2010/01/13 コメント化

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            MainLog.Write_SQL_Err("clsCommon.SetDynaset", SQL)
            MainLog.Write_Err("clsCommon.SetDynaset", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader.Dispose()
                oraReader = Nothing
            End If
            '*** End Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***

            BRet = False
            With GLog
                .Job2 = "カーソル形成"
                .Result = NG
                .Discription = ex.Message.Replace(",", " ")
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            MainLog.Write_SQL_Err("clsCommon.SetDynaset", SQL)
            MainLog.Write_Err("clsCommon.SetDynaset", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        End Try

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        If IS_LEVEL3 = True Then
            MainLog.Write_Exit3(sw, "clsCommon.SetDynaset", "復帰値=" & BRet)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Return BRet
    End Function

    '
    ' 機　能 : データベース制御
    '
    ' 戻り値 : OK           正常終了
    '          NG           異常終了
    '          ※DB_Execute の場合は実効レコード数を返す
    '
    ' 引　数 : onProcess      制御識別
    ' 　　　   SQL            SQL文(Optional) 
    '          SQLCode        ステータス(Optional)
    '          LogOut         ログ出力可否(Optional)
    '
    ' 備　考 : onProcess = Public Enum OraModule
    '                         DB_Connect   = 1      DB接続
    '                         DB_Begin     = 2      トランザクション開始
    '                         DB_Execute   = 4      SQL文の実行
    '                         DB_Commit    = 8      コミット
    '                         DB_Rollback  = 16     ロールバック
    '                         DB_Terminate = 32     DB切断
    '                      End Enum
    '
    Public Function DBExecuteProcess(ByVal onProcess As Integer, Optional ByVal SQL As String = "", _
            Optional ByRef SQLCode As Integer = 0, Optional ByVal LogOut As Boolean = True) As Integer
        Dim Ret As Integer = 0

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = MainLog.Write_Enter3("clsCommon.DBExecuteProcess", "onProcess=" & onProcess)
            If IS_LEVEL4 = True Then
                MainLog.Write_LEVEL4("clsCommon.DBExecuteProcess", "呼出", "呼出しスタック: " & Environment.StackTrace)
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Try
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            If GCOM_MainDB Is Nothing Then
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                'ORACLE接続確認
                If Not ConnectDataBase() Then
                    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                    If IS_LEVEL3 = True Then
                        MainLog.Write_Exit3(sw, "clsCommon.DBExecuteProcess", "復帰値=-9")
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                    'ORACLE 接続不可
                    Return -9
                End If
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

            If (onProcess And enDB.DB_Begin) = enDB.DB_Begin Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Begin")
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                If Not GCOM_MainDB Is Nothing Then
                    GCOM_MainDB.BeginTrans()

                Else
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

                    'トランザクションの開始
                    Tran = OraConnect.BeginTransaction

                    Command.Transaction = Tran

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                End If
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            End If

            If (onProcess And enDB.DB_Execute) = enDB.DB_Execute Then

                Try
                    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                    If IS_LEVEL3 = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Execute")
                    End If
                    If IS_SQLLOG = True Then
                        MainLog.Write_SQL("clsCommon.DBExecuteProcess", SQL)
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    If Not GCOM_MainDB Is Nothing Then
                        Ret = GCOM_MainDB.ExecuteNonQuery(SQL)

                    Else
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

                        'SQL文の実行
                        Command.CommandText = SQL

                        Ret = Command.ExecuteNonQuery

                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

                    If LogOut AndAlso SQLCode = 0 Then
                        With GLog
                            .Job2 = SQL.Substring(0, 6).ToUpper
                            .Result = OK
                            .Discription = "影響件数=" & Ret & ": "
                        End With
                        'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
                    End If
                Catch ex As OracleException
                    SQLCode = ex.Code
                    If LogOut OrElse SQLCode > 1 Then
                        With GLog
                            .Job2 = ex.Message.Replace(",", "")
                            .Result = NG
                            .Discription = SQL.Replace(",", ":")
                        End With
                        'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True), 0) '2010/01/13 コメント化
                    End If

                    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                    MainLog.Write_SQL_Err("clsCommon.DBExecuteProcess", SQL)
                    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                    '*** Str Add 2016/01/21 SO)荒木 for 潜在障害（例外発生時に呼出し元で正常処理が行われる） ***
                    Throw
                    '*** End Add 2016/01/21 SO)荒木 for 潜在障害（例外発生時に呼出し元で正常処理が行われる） ***

                Catch ex As Exception
                    If LogOut Then
                        With GLog
                            .Job2 = SQL.Substring(0, 6).ToUpper
                            .Result = NG
                            .Discription = ex.Message.Replace(",", " ")
                        End With
                        'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
                    End If

                    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                    MainLog.Write_SQL_Err("clsCommon.DBExecuteProcess", SQL)
                    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                    '*** Str Add 2016/01/21 SO)荒木 for 潜在障害（例外発生時に呼出し元で正常処理が行われる） ***
                    Throw
                    '*** End Add 2016/01/21 SO)荒木 for 潜在障害（例外発生時に呼出し元で正常処理が行われる） ***

                End Try
            End If

            If (onProcess And enDB.DB_Commit) = enDB.DB_Commit Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Commit")
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                If Not GCOM_MainDB Is Nothing Then
                    GCOM_MainDB.Commit()

                Else
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

                    'トランザクションをコミット
                    '*** Str Upd 2016/01/21 SO)荒木 for 潜在障害（トラン終了後のRollback呼出しで例外発生） ***
                    'Tran.Commit()
                    If Not Tran Is Nothing Then
                        Tran.Commit()
                        Tran = Nothing
                    End If
                    '*** End Upd 2016/01/21 SO)荒木 for 潜在障害（トラン終了後のRollback呼出しで例外発生） ***

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                End If
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            End If

            If (onProcess And enDB.DB_Rollback) = enDB.DB_Rollback Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Rollback")
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                If Not GCOM_MainDB Is Nothing Then
                    GCOM_MainDB.Rollback()

                Else
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

                    'ロールバック
                    '*** Str Upd 2016/01/21 SO)荒木 for 潜在障害（トラン終了後のRollback呼出しで例外発生） ***
                    'Tran.Rollback()
                    If Not Tran Is Nothing Then
                        Tran.Rollback()
                        Tran = Nothing
                    End If
                    '*** End Upd 2016/01/21 SO)荒木 for 潜在障害（トラン終了後のRollback呼出しで例外発生） ***

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                End If
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            End If

            If (onProcess And enDB.DB_Terminate) = enDB.DB_Terminate Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Terminate")
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                If Not GCOM_MainDB Is Nothing Then
                    'GCOM_MainDB.Close()
                    'GCOM_MainDB = Nothing

                Else
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

                    'データベース接続を閉じる
                    OraConnect.Close()

                    Tran = Nothing
                    OraConnect.Dispose()
                    OraConnect = Nothing

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                End If
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            End If

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            If IS_LEVEL3 = True Then
                MainLog.Write_Exit3(sw, "clsCommon.DBExecuteProcess", "復帰値=" & Ret)
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Return Ret
        Catch ex As Exception
            With GLog
                .Job2 = "データベース制御"
                .Result = NG
                .Discription = ex.Message.Replace(",", " ")
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            MainLog.Write_Err("clsCommon.DBExecuteProcess", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            '*** Str Upd 2016/01/21 SO)荒木 for 潜在障害（例外発生時に呼出し元で正常処理が行われる） ***
            'Return -9
            Throw
            '*** End Upd 2016/01/21 SO)荒木 for 潜在障害（例外発生時に呼出し元で正常処理が行われる） ***

        End Try
    End Function

    '
    ' 機能　　 : ORACLEへ接続する
    '
    ' 戻り値　 : 成功 = True
    ' 　　　　   失敗 = False
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : なし
    '
    Private Function ConnectDataBase() As Boolean
        Try
            If OraConnect Is Nothing OrElse Not OraConnect.State = ConnectionState.Open Then

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL4 = True Then
                    sw = MainLog.Write_Enter4("clsCommon.ConnectDataBase")
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                OraConnect = New OracleClient.OracleConnection

                OraConnect.ConnectionString = CASTCommon.DB.CONNECT

                OraConnect.Open()

                Command = OraConnect.CreateCommand

                If OraConnect.State = ConnectionState.Open Then
                    'With GLog
                    '    .Job2 = "ORACLE接続"
                    '    .Result = OK
                    '    .Discription = ""
                    'End With
                    'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Else
                    With GLog
                        .Job2 = "ORACLE接続"
                        .Result = NG
                        .Discription = ""
                    End With
                    'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化

                    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                    If IS_LEVEL4 = True Then
                        MainLog.Write_Exit4(sw, "clsCommon.ConnectDataBase", "復帰値=False")
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                    Return False
                End If

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL4 = True Then
                    MainLog.Write_Exit4(sw, "clsCommon.ConnectDataBase", "復帰値=True")
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
            End If

            Return True
        Catch ex As OracleException
            With GLog
                .Job2 = ex.Message
                .Result = NG
                .Discription = "ORACLE.CONNECT"
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True), 0) '2010/01/13 コメント化

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            MainLog.Write_Err("clsCommon.ConnectDataBase",ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            OraConnect = Nothing
            Return False
        Catch ex As Exception
            With GLog
                .Job2 = "ORACLE接続"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            MainLog.Write_Err("clsCommon.ConnectDataBase", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            OraConnect = Nothing
            Return False
        End Try
    End Function

    '
    ' 機能　　　: 自身の端末番号を参照する
    '
    ' 戻り値　　: 端末番号
    '
    ' 引き数　　: ARG1 - なし
    '
    ' 備考　　　: なし
    '
    Public Function GetStationNo() As String
        Dim REC As OracleDataReader = Nothing
        Try
            Dim SQL As String = "SELECT STATION_NO"
            SQL &= " FROM STATION_TBL"
            SQL &= " WHERE UPPER(COMPUTER_NAME) = '" & System.Environment.MachineName.ToUpper & "'"

            If SetDynaset(SQL, REC) AndAlso REC.Read Then

                Return NzStr(REC.Item("STATION_NO")).Trim
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "自身の端末番号参照"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return "NULL"
    End Function

    '
    ' 機能　　　: 銀行情報を参照する
    '
    ' 戻り値　　: 0 = OK, 1 = Bank Error, 2 = Branch Error
    '
    ' 引き数　　: ARG1 - Bank Code
    ' 　　　 　　 ARG2 - Branch Code
    '
    ' 備考　　　: なし
    '
    Public Function CheckBankBranch(ByVal aBKCode As Object, ByVal aBRCode As Object) As Integer
        Dim Ret As Integer = 1
        Dim REC As OracleDataReader = Nothing
        Try
            Dim BRCode As Integer = NzInt(aBRCode)

            Dim SQL As String = "SELECT SIT_NO_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & aBKCode.ToString.PadLeft(4, "0"c) & "'"

            If SetDynaset(SQL, REC) Then

                Ret = 2
                Do While REC.Read

                    If NzStr(REC.Item(0)) = BRCode.ToString.PadLeft(3, "0"c) Then

                        Ret = 0
                        Exit Do
                    End If
                Loop
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "銀行情報参照"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function

    '
    ' 機能　 ： 金融機関情報検索
    '
    ' 引数　 ： ARG1 - 金融機関コード
    ' 　　　 　 ARG2 - 支店コード
    ' 　　　 　 ARG3 - 制限バイト数
    '
    ' 戻り値 ： 金融機関名／支店名
    '
    ' 備考　 ： なし
    '
    Public Function GetBKBRName(ByVal BKCode As String, ByVal BRCode As String, _
                    Optional ByVal avShort As Integer = 0) As String
        GetBKBRName = ""
        Dim REC As OracleDataReader = Nothing
        Try
            If BKCode.Trim.Length = 0 Then
                Return ""
            End If
            Dim BRFLG As Boolean = (BRCode.Trim.Length > 0)

            Dim SQL As String = "SELECT KIN_NNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & BKCode.PadLeft(4, "0"c) & "'"
            If BRFLG Then
                SQL &= " AND SIT_NO_N = '" & BRCode.PadLeft(3, "0"c) & "'"
            Else
                SQL &= " AND ROWNUM = 1"
            End If

            If SetDynaset(SQL, REC) AndAlso REC.Read Then
                If BRFLG Then
                    '支店名
                    If avShort = 0 Then
                        Return NzStr(REC.Item("SIT_NNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("SIT_NNAME_N").ToString, avShort)
                    End If
                Else
                    '金融機関名
                    If avShort = 0 Then
                        Return NzStr(REC.Item("KIN_NNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("KIN_NNAME_N").ToString, avShort)
                    End If
                End If
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "金融機関情報検索"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            Return ""
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Function

    Protected Overrides Sub Finalize()
        Try
            If Not OraConnect Is Nothing Then
                OraConnect.Close()
                OraConnect.Dispose()
            End If
        Catch ex As Exception
        End Try
        MyBase.Finalize()
    End Sub
    '
    ' 機能　 ： 金融機関情報検索(カナ名)
    '
    ' 引数　 ： ARG1 - 金融機関コード
    ' 　　　 　 ARG2 - 支店コード
    ' 　　　 　 ARG3 - 制限バイト数
    '
    ' 戻り値 ： 金融機関名／支店名
    '
    ' 備考　 ： 2009/09/29 追加
    '
    Public Function GetBKBRKName(ByVal BKCode As String, ByVal BRCode As String, _
                    Optional ByVal avShort As Integer = 0) As String
        GetBKBRKName = ""
        Dim REC As OracleDataReader = Nothing
        Try
            If BKCode.Trim.Length = 0 Then
                Return ""
            End If
            Dim BRFLG As Boolean = (BRCode.Trim.Length > 0)

            Dim SQL As String = "SELECT KIN_KNAME_N"
            SQL &= ", SIT_KNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & BKCode.PadLeft(4, "0"c) & "'"
            If BRFLG Then
                SQL &= " AND SIT_NO_N = '" & BRCode.PadLeft(3, "0"c) & "'"
            Else
                SQL &= " AND ROWNUM = 1"
            End If

            If SetDynaset(SQL, REC) AndAlso REC.Read Then
                If BRFLG Then
                    '支店名
                    If avShort = 0 Then
                        Return NzStr(REC.Item("SIT_KNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("SIT_KNAME_N").ToString, avShort)
                    End If
                Else
                    '金融機関名
                    If avShort = 0 Then
                        Return NzStr(REC.Item("KIN_KNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("KIN_KNAME_N").ToString, avShort)
                    End If
                End If
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "金融機関情報検索"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            Return ""
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Function
    '--------------------------------------------------------------------------------------------------
    '' 和暦変換関数 2007.10.29 By K.Seto
    '--------------------------------------------------------------------------------------------------
    '
    ' 機能　 ： 西暦→和暦化
    '
    ' 引数　 ： ARG1 - 年月日配列
    ' 　　　 　 ARG2 - 返還後文字列
    ' 　　　 　 ARG3 - Optional 曜日要否
    ' 　　　 　 ARG4 - Optional 西暦日付
    '
    ' 戻り値 ： -1 正常、0=年相違、1=月相違、2=日相違 
    '
    ' 備考　 ： なし
    '
    Public Function ChangeDate(ByVal aonText() As Integer, ByRef aonString As String, _
        Optional ByVal aWeekDay As Short = 0, Optional ByRef onDate As Date = BadResultDate) As Integer
        Dim Ret As Integer = -1
        aonString = ""
        Try
            If aonText.Length = 3 Then

                onDate = New DateTime(aonText(0), aonText(1), aonText(2))

                If aonText(1) < 1 OrElse aonText(1) > 12 Then
                    Return 1
                ElseIf Not aonText(0) = onDate.Year Then
                    Return 0
                ElseIf Not aonText(1) = onDate.Month OrElse _
                       Not aonText(2) = onDate.Day Then
                    Return 2
                End If

                Dim Culture As CultureInfo = New CultureInfo("ja-JP", True)
                Culture.DateTimeFormat.Calendar = New JapaneseCalendar

                Select Case aWeekDay
                    Case 0
                        aonString = onDate.ToString("ggy年MM月dd日", Culture)
                    Case Else
                        aonString = onDate.ToString("ggy年MM月dd日(dddd)", Culture)
                End Select
            Else
                Return 0
            End If
        Catch ex As Exception
            onDate = BadResultDate
            With GLog
                .Job2 = "西暦→和暦化"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機能　 ： 和暦→西暦化
    '
    ' 引数　 ： ARG1 - 和暦文字列
    '
    ' 戻り値 ： 西暦文字列 
    '
    ' 備考　 ： なし
    '
    Public Function ChangeDate(ByVal aonString As String) As Date
        Dim Tmp2 As String = ""
        Dim Ret As Date = Nothing
        Try
            ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- START
            'Dim Culture As CultureInfo = New CultureInfo("ja-JP", True)

            'Dim Format As System.Globalization.DateTimeFormatInfo = Culture.DateTimeFormat

            'Format.Calendar = New System.Globalization.JapaneseCalendar
            ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- END

            Dim Tmp1 As String = aonString.Replace("/"c, "").Replace("."c, "")

            Dim Counter As Integer

            For Cnt As Integer = Tmp1.Length - 1 To 0 Step -1
                Select Case Tmp1.Substring(Cnt, 1)
                    Case "年", "月", "日"
                        Counter = 0
                    Case Else
                        If Counter = 0 AndAlso Tmp2.Length Mod 2 = 1 Then
                            Tmp2 = "0" & Tmp2
                        End If
                        Tmp2 = Tmp1.Substring(Cnt, 1) & Tmp2
                        Counter += 1
                End Select
            Next Cnt

            ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
            'Ret = DateTime.ParseExact(Tmp2, "gyyMMdd", Format)

            Dim reg As New Regex("[^0-9]")
            Tmp2 = reg.Replace(Tmp2, "").PadLeft(6, "0"c)
            Ret = DateTime.ParseExact(ConvertYear(Tmp2.Substring(0, 2)) & Tmp2.Substring(2, 4), "yyyyMMdd", Nothing)
            ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END

        Catch ex As Exception
            With GLog
                .Job2 = "和暦→西暦化"
                .Result = NG
                .Discription = aonString & " → " & Tmp2 & " ： " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
        End Try

        Return Ret
    End Function

    '
    ' 機能　 ： 画面右上ラベル位置設定
    '
    ' 引数　 ： ARG1 - ログイン名 ：    (Label)
    ' 　　　 　 ARG2 - システム日付 ：  (Label)
    ' 　　　 　 ARG3 - ユーザＩＤ       (Label)
    ' 　　　 　 ARG4 - システム日付     (Label)
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： なし
    '
    Public Sub SetMonitorTopArea(ByVal Label2 As Label, ByVal Label3 As Label, _
            ByVal lblUser As Label, ByVal lblDate As Label, Optional ByVal AddLeng As Integer = 0)
        Try
            '2009/09/23 ===========================================
            'Label2.Location = New Point(580 + AddLeng, 8)
            'Label3.Location = New Point(566 + AddLeng, 28)
            'lblUser.Location = New Point(640 + AddLeng, 8)
            'lblDate.Location = New Point(640 + AddLeng, 28)
            'lblUser.Text = GetUserID
            'lblDate.Text = String.Format("{0:yyyy年MM月dd日}", GetSysDate)
            Label2.Location = New Point(580 + AddLeng, 8)
            Label3.Location = New Point(580 + AddLeng, 28)
            lblUser.Location = New Point(665 + AddLeng, 8)
            lblDate.Location = New Point(665 + AddLeng, 28)
            lblUser.Text = GetUserID
            lblDate.Text = String.Format("{0:yyyy年MM月dd日}", Date.Parse("2021/02/17"))
            '======================================================
        Catch ex As Exception
            '2009/09/23 =================================================
            'With GLog
            '    .Job2 = "画面右上ラベル位置設定"
            '    .Result = NG
            '    .Discription = ex.Message
            'End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            lblUser.Text = "SYSTEM ERROR"
            lblDate.Text = "SYSTEM ERROR"
            '============================================================
        End Try
    End Sub

    '*** 修正 mitsu 2008/05/23 スケジュールの件数、金額再計算 ***
    '
    ' 機　能 : 各スケジュールマスタの振替済件数・金額の再計算
    '
    ' 引数　 : ARG1 - 処理区分
    ' 　　　   ARG2 - 取引先主コード
    ' 　　　   ARG3 - 取引先副コード
    ' 　　　   ARG4 - 振替日or振込日
    ' 　　　   ARG5 - 持込SEQ(省略化)
    '
    ' 戻り値 : 0以上 = OK, -1 = NG
    '
    ' 備　考 : 現仕様では総振は処理しない
    '    
    Public Function ReCalcSchmastTotal(ByVal FSYORI_KBN As String, ByVal TORIS_CODE As String, _
        ByVal TORIF_CODE As String, ByVal FURI_DATE As String, Optional ByVal MOTIKOMI_SEQ As Integer = Nothing) As Integer
        Dim nRet As Integer = 0

        Dim MainDB As New MyOracle
        Dim OraReader As New MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Dim TORIMAST As String = ""
        Dim SCHMAST As String = ""
        Dim MEIMAST As String = ""
        Select Case FSYORI_KBN
            Case "1"
                TORIMAST = "TORIMAST"
                SCHMAST = "SCHMAST"
                MEIMAST = "MEIMAST"
            Case "3"
                TORIMAST = "S_TORIMAST"
                SCHMAST = "S_SCHMAST"
                MEIMAST = "S_MEIMAST"
        End Select

        SQL.Append("SELECT")
        SQL.Append(" *")
        SQL.Append(" FROM " & SCHMAST & "," & TORIMAST)
        SQL.Append(" WHERE ")
        SQL.Append("     FURI_DATE_S   = " & SQ(FURI_DATE))
        SQL.Append(" AND TORIS_CODE_S  = TORIS_CODE_T")
        SQL.Append(" AND TORIF_CODE_S  = TORIF_CODE_T")
        SQL.Append(" AND TORIS_CODE_S  = " & SQ(TORIS_CODE))
        SQL.Append(" AND TORIF_CODE_S  = " & SQ(TORIF_CODE))

        Select Case FSYORI_KBN
            Case "1"
                '自振は不能フラグが立っている場合のみ更新する
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            Case "3"
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
                '総振は更新しない(要仕様検討)
                SQL.Append(" AND FSYORI_KBN_S <> '3'")
        End Select

        Try
            If OraReader.DataReader(SQL) = True Then
                Dim BAITAI_CODE As String = OraReader.GetString("BAITAI_CODE_T")
                Dim FMT_KBN As String = OraReader.GetString("FMT_KBN_T")
                Dim TotalReader As New MyOracleReader(MainDB)

                Dim FunoKensu As Long = 0
                Dim FunoKingaku As Long = 0
                Dim ZumiKensu As Long = 0
                Dim ZumiKingaku As Long = 0

                '----------------------
                '不能件数、金額取得
                '----------------------
                SQL.Length = 0
                SQL.Append("SELECT ")
                SQL.Append(" COUNT(FURIKIN_K) KEN")
                SQL.Append(",SUM(FURIKIN_K)   KIN")
                SQL.Append(" FROM " & MEIMAST)
                SQL.Append(" WHERE ")
                SQL.Append("     FURI_DATE_K = " & SQ(FURI_DATE))
                If FMT_KBN = "02" Then
                    ' 国税は、データ区分が３のレコード
                    SQL.Append(" AND DATA_KBN_K = '3'")
                Else
                    SQL.Append(" AND DATA_KBN_K = '2'")
                End If

                SQL.Append(" AND FURIKETU_CODE_K <> '0'")
                ' 2008.03.14 振替金額が０円のものは含まない
                SQL.Append(" AND FURIKIN_K > 0")
                SQL.Append(" AND TORIS_CODE_K = " & SQ(TORIS_CODE))
                SQL.Append(" AND TORIF_CODE_K = " & SQ(TORIF_CODE))
                If FSYORI_KBN = "3" Then
                    SQL.Append(" AND CYCLE_NO_K = " & MOTIKOMI_SEQ)
                End If

                If TotalReader.DataReader(SQL) = True Then
                    FunoKensu = TotalReader.GetInt64("KEN")
                    FunoKingaku = TotalReader.GetInt64("KIN")
                End If
                TotalReader.Close()

                '----------------------
                '振替済件数、金額取得
                '----------------------
                SQL = SQL.Replace("FURIKETU_CODE_K <> '0'", "FURIKETU_CODE_K = '0'")

                If TotalReader.DataReader(SQL) = True Then
                    ZumiKensu = TotalReader.GetInt64("KEN")
                    ZumiKingaku = TotalReader.GetInt64("KIN")
                End If
                TotalReader.Close()

                '-------------------------------------------
                'スケジュールマスタの更新
                '-------------------------------------------
                SQL.Length = 0
                SQL.Append("UPDATE " & SCHMAST & " SET")
                SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                SQL.Append(",FURI_KIN_S = " & ZumiKingaku.ToString)
                SQL.Append(",FUNOU_KEN_S = " & FunoKensu.ToString)
                SQL.Append(",FUNOU_KIN_S =" & FunoKingaku.ToString)
                SQL.Append(" WHERE TORIS_CODE_S = " & SQ(TORIS_CODE))
                SQL.Append("   AND TORIF_CODE_S = " & SQ(TORIF_CODE))
                SQL.Append("   AND FURI_DATE_S = " & SQ(FURI_DATE))

                nRet = MainDB.ExecuteNonQuery(SQL)

                '-------------------------------------------
                '学校スケジュールマスタの更新
                '-------------------------------------------
                If nRet > -1 AndAlso BAITAI_CODE = "07" Then
                    SQL.Length = 0
                    SQL.Append("UPDATE G_SCHMAST SET")
                    SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                    SQL.Append(",FURI_KIN_S = " & ZumiKingaku.ToString)
                    SQL.Append(",FUNOU_KEN_S = " & FunoKensu.ToString)
                    SQL.Append(",FUNOU_KIN_S =" & FunoKingaku.ToString)
                    SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(TORIS_CODE))
                    SQL.Append("   AND FURI_KBN_S = " & SQ((CAInt32(TORIF_CODE) - 1).ToString))
                    SQL.Append("   AND FURI_DATE_S = " & SQ(FURI_DATE))

                    nRet = MainDB.ExecuteNonQuery(SQL)
                End If
            End If

            OraReader.Close()
            MainDB.Commit()

        Catch ex As Exception
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            MainDB.Rollback()
            nRet = -1
        End Try

        Return nRet
    End Function

    '
    ' 機　能 : 各他行スケジュールマスタの振替済件数・金額の再計算
    '
    ' 引数　 : ARG1 - 処理区分
    ' 　　　   ARG2 - 取引先主コード
    ' 　　　   ARG3 - 取引先副コード
    ' 　　　   ARG4 - 振替日or振込日
    ' 　　　   ARG5 - 金融機関コード(省略化)
    '
    ' 戻り値 : 0以上 = OK, -1 = NG
    '
    ' 備　考 : 現仕様では総振は処理しない
    '    
    Public Function ReCalcTakoSchmastTotal(ByVal FSYORI_KBN As String, ByVal TORIS_CODE As String, _
        ByVal TORIF_CODE As String, ByVal FURI_DATE As String, Optional ByVal TKIN_NO As String = "") As Integer
        Dim nRet As Integer = 0

        Dim MainDB As New MyOracle
        Dim OraReader As New MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        SQL.Append("SELECT")
        SQL.Append(" *")
        SQL.Append(" FROM TAKOSCHMAST,TORIMAST")
        SQL.Append(" WHERE ")
        SQL.Append("     FURI_DATE_U   = " & SQ(FURI_DATE))
        SQL.Append(" AND TORIS_CODE_U  = TORIS_CODE_T")
        SQL.Append(" AND TORIF_CODE_U  = TORIF_CODE_T")
        SQL.Append(" AND TORIS_CODE_U  = " & SQ(TORIS_CODE))
        SQL.Append(" AND TORIF_CODE_U  = " & SQ(TORIF_CODE))
        '不能フラグが立っている場合のみ更新する
        SQL.Append(" AND FUNOU_FLG_U = '1'")
        '金融機関コード指定時
        If TKIN_NO <> "" Then
            SQL.Append(" AND (TKIN_NO_U = " & SQ(TKIN_NO))
            'SSSの場合は金融機関コードを指定しない
            SQL.Append(" OR FMT_KBN_T IN ('20','21'))")
        End If

        Try
            If OraReader.DataReader(SQL) = True Then
                Dim BAITAI_CODE As String = OraReader.GetString("BAITAI_CODE_T")
                Dim FMT_KBN As String = OraReader.GetString("FMT_KBN_T")
                Dim TotalReader As New MyOracleReader(MainDB)
                '他行スケジュールマスタのレコード毎に更新
                While OraReader.EOF = False
                    TKIN_NO = OraReader.GetString("TKIN_NO_U")
                    'Dim TEIKEI_KBN As String = OraReader.GetString("TEIKEI_KBN_U") '2010/01/13 コメント化

                    Dim FunoKensu As Long = 0
                    Dim FunoKingaku As Long = 0
                    Dim ZumiKensu As Long = 0
                    Dim ZumiKingaku As Long = 0

                    '----------------------
                    '不能件数、金額取得
                    '----------------------
                    SQL.Length = 0
                    SQL.Append("SELECT ")
                    SQL.Append(" COUNT(FURIKIN_K) KEN")
                    SQL.Append(",SUM(FURIKIN_K)   KIN")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE ")
                    SQL.Append("     FURI_DATE_K = " & SQ(FURI_DATE))
                    If FMT_KBN = "02" Then
                        ' 国税は、データ区分が３のレコード
                        SQL.Append(" AND DATA_KBN_K = '3'")
                    Else
                        SQL.Append(" AND DATA_KBN_K = '2'")
                    End If

                    SQL.Append(" AND FURIKETU_CODE_K <> '0'")
                    ' 2008.03.14 振替金額が０円のものは含まない
                    SQL.Append(" AND FURIKIN_K > 0")
                    SQL.Append(" AND TORIS_CODE_K = " & SQ(TORIS_CODE))
                    SQL.Append(" AND TORIF_CODE_K = " & SQ(TORIF_CODE))
                    Select Case FMT_KBN
                        'SSSの場合
                        Case "20", "21"
                            '自行分は対象外とする
                            SQL.Append(" AND KEIYAKU_KIN_K <> " & SQ(JIKINKOCD))
                            '2010/01/13 コメント化
                            'If TEIKEI_KBN = "1" Then
                            '    ' 提携内
                            '    SQL.Append(" AND EXISTS (")
                            'Else
                            '    ' 提携外
                            '    SQL.Append(" AND NOT EXISTS (")
                            'End If
                            '======================
                            SQL.Append(" SELECT TEIKEI_KBN_N FROM TENMAST ")
                            SQL.Append(" WHERE KIN_NO_N = KEIYAKU_KIN_K")
                            SQL.Append("   AND SIT_NO_N = KEIYAKU_SIT_K")
                            SQL.Append("   AND EDA_N = '01'")
                            SQL.Append("   AND TEIKEI_KBN_N = '1'")
                            SQL.Append("   )")

                        Case Else
                            SQL.Append(" AND KEIYAKU_KIN_K = " & SQ(TKIN_NO))
                    End Select

                    If TotalReader.DataReader(SQL) = True Then
                        FunoKensu = TotalReader.GetInt64("KEN")
                        FunoKingaku = TotalReader.GetInt64("KIN")
                    End If
                    TotalReader.Close()

                    '----------------------
                    '振替済件数、金額取得
                    '----------------------
                    SQL = SQL.Replace("FURIKETU_CODE_K <> '0'", "FURIKETU_CODE_K = '0'")

                    If TotalReader.DataReader(SQL) = True Then
                        ZumiKensu = TotalReader.GetInt64("KEN")
                        ZumiKingaku = TotalReader.GetInt64("KIN")
                    End If
                    TotalReader.Close()

                    '-------------------------------------------
                    '他行スケジュールマスタの更新
                    '-------------------------------------------
                    SQL.Length = 0
                    SQL.Append("UPDATE TAKOSCHMAST SET")
                    SQL.Append(" FURI_KEN_U = " & ZumiKensu.ToString)
                    SQL.Append(",FURI_KIN_U = " & ZumiKingaku.ToString)
                    SQL.Append(",FUNOU_KEN_U = " & FunoKensu.ToString)
                    SQL.Append(",FUNOU_KIN_U =" & FunoKingaku.ToString)
                    SQL.Append(" WHERE TORIS_CODE_U = " & SQ(TORIS_CODE))
                    SQL.Append("   AND TORIF_CODE_U = " & SQ(TORIF_CODE))
                    SQL.Append("   AND FURI_DATE_U = " & SQ(FURI_DATE))
                    Select Case FMT_KBN
                        'SSSの場合
                        Case "20", "21"
                            'SQL.Append("   AND TEIKEI_KBN_U = " & SQ(TEIKEI_KBN))  '2010/01/13 コメント化
                        Case Else
                            SQL.Append("   AND TKIN_NO_U = " & SQ(TKIN_NO))
                    End Select

                    nRet = MainDB.ExecuteNonQuery(SQL)

                    '-------------------------------------------
                    '学校他行スケジュールマスタの更新
                    '-------------------------------------------
                    If nRet > -1 AndAlso BAITAI_CODE = "07" Then
                        SQL.Length = 0
                        SQL.Append("UPDATE G_TAKOUSCHMAST SET")
                        SQL.Append(" FURI_KEN_U = " & ZumiKensu.ToString)
                        SQL.Append(",FURI_KIN_U = " & ZumiKingaku.ToString)
                        SQL.Append(",FUNOU_KEN_U = " & FunoKensu.ToString)
                        SQL.Append(",FUNOU_KIN_U =" & FunoKingaku.ToString)
                        SQL.Append(" WHERE GAKKOU_CODE_U = " & SQ(TORIS_CODE))
                        SQL.Append("   AND FURI_KBN_U = " & SQ((CAInt32(TORIF_CODE) - 1).ToString))
                        SQL.Append("   AND FURI_DATE_U = " & SQ(FURI_DATE))
                        SQL.Append("   AND TKIN_NO_U = " & SQ(TKIN_NO))

                        nRet = MainDB.ExecuteNonQuery(SQL)
                    End If

                    OraReader.NextRead()
                End While
            End If

            OraReader.Close()
            MainDB.Commit()

        Catch ex As Exception
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 コメント化
            MainDB.Rollback()
            nRet = -1
        End Try

        Return nRet
    End Function
    '************************************************************

    '
    ' 機能　　　: 取引先コンボボックス設定関数
    '
    ' 戻り値　　: 0 = OK, -1 = Err
    '
    ' 引き数　　: ARG1 - 先頭カナ文字
    ' 　　　 　　 ARG2 - コンボボックス名
    '             ARG3 - 取引先主コードテキストボックス
    '             ARG4 - 取引先副コードテキストボックス
    '             ARG5 - 処理区分(1:自振 3:総振)
    '             ARG6 - 媒体コード
    ' 備考　　　: T-Sakai追加
    '
    Public Function SelectItakuName(ByVal KanaKey As String, ByVal Cmbbox As ComboBox, _
                                    ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_F As TextBox, _
                                    Optional ByVal FSYORI_KBN As String = "1", Optional ByVal Jyoken As String = "") As Integer

        '--------------------------------------------
        '委託者名がカナKEY値で始まる委託者名カナを検索
        '--------------------------------------------
        Dim Ret As Integer = -1
        Dim REC As OracleDataReader = Nothing
        Dim Toris_Code As String
        Dim Torif_Code As String
        Dim Itaku_NName As String
        Try
            '-----------------------------------------
            '現在のコンボボックスをクリアにする
            '-----------------------------------------
            Cmbbox.Text = ""
            TORIS_CODE_T.Text = ""
            TORIF_CODE_F.Text = ""
            Cmbbox.Items.Clear()

            If String.IsNullOrEmpty(KanaKey) Then
                KanaKey = ""
            End If

            '2014/02/18 saitou 標準版 操作性向上 UPD -------------------------------------------------->>>>
            '必要な項目のみ取得する。
            Dim SQL As String = "SELECT TORIS_CODE_T, TORIF_CODE_T, ITAKU_NNAME_T FROM"
            'Dim SQL As String = "SELECT * FROM"
            '2014/02/18 saitou 標準版 操作性向上 UPD --------------------------------------------------<<<<
            If FSYORI_KBN = "1" Then
                SQL &= " TORIMAST"
            Else
                SQL &= " S_TORIMAST"
            End If
            SQL &= " WHERE FSYORI_KBN_T = " & SQ(FSYORI_KBN)

            If Jyoken.Trim <> "" Then
                SQL &= Space(1) & Jyoken
            End If

            If KanaKey.Trim <> Nothing Then
                SQL &= " AND SUBSTR(ITAKU_KNAME_T,1,1) = " & SQ(Trim(KanaKey))
            End If

            SQL &= " ORDER BY TORIS_CODE_T,TORIF_CODE_T"
            Console.WriteLine(SQL)
            Cmbbox.Items.Add(Space(50))
            '-----------------------------------------
            'コンボボックスにリストを追加する
            '-----------------------------------------
            If SetDynaset(SQL, REC) Then
                Do While REC.Read
                    Toris_Code = NzStr(REC.Item("TORIS_CODE_T")).Trim
                    Torif_Code = NzStr(REC.Item("TORIF_CODE_T")).Trim
                    Itaku_NName = NzStr(REC.Item("ITAKU_NNAME_T")).Trim
                    '2013/12/27 saitou 西濃信金 MODIFY ----------------------------------------------->>>>
                    '15文字で切るのをやめる
                    Cmbbox.Items.Add(Toris_Code & " - " & Torif_Code & " " & Itaku_NName.Trim)
                    'Cmbbox.Items.Add(Toris_Code & " - " & Torif_Code & "  " & Mid(Itaku_NName.Trim, 1, 15))
                    '2013/12/27 saitou 西濃信金 MODIFY -----------------------------------------------<<<<
                Loop

            '*** Str Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***
            ElseIf REC Is Nothing Then
                Return -1
            '*** End Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***

            End If
            '-----------------------------------------
            'コンボボックスに1番最初のリストを表示する
            '-----------------------------------------
            If Cmbbox.Items.Count > 0 Then
                Cmbbox.Text = Cmbbox.Items.Item(0).ToString
            End If

            Ret = 0
        Catch ex As Exception
            Console.WriteLine(ex.Message)

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function

    '************************************************************

    '
    ' 機能　　　: 取引先コンボボックス(VIEW用)設定関数
    '
    ' 戻り値　　: 0 = OK, -1 = Err
    '
    ' 引き数　　: ARG1 - 先頭カナ文字
    ' 　　　 　　 ARG2 - コンボボックス名
    '             ARG3 - 取引先主コードテキストボックス
    '             ARG4 - 取引先副コードテキストボックス
    '             ARG5 - 処理区分(1:自振 3:総振)
    '             ARG6 - 媒体コード
    ' 備考　　　: T-Sakai追加
    '
    Public Function SelectItakuName_View(ByVal KanaKey As String, ByVal Cmbbox As ComboBox, _
                                    ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_F As TextBox, _
                                    Optional ByVal FSYORI_KBN As String = "1", Optional ByVal Jyoken As String = "") As Integer

        '--------------------------------------------
        '委託者名がカナKEY値で始まる委託者名カナを検索
        '--------------------------------------------
        Dim Ret As Integer = -1
        Dim REC As OracleDataReader = Nothing
        Dim Toris_Code As String
        Dim Torif_Code As String
        Dim Itaku_NName As String
        Try
            '-----------------------------------------
            '現在のコンボボックスをクリアにする
            '-----------------------------------------
            Cmbbox.Text = ""
            TORIS_CODE_T.Text = ""
            TORIF_CODE_F.Text = ""
            Cmbbox.Items.Clear()

            If String.IsNullOrEmpty(KanaKey) Then
                KanaKey = ""
            End If

            Dim SQL As String = "SELECT TORIS_CODE_T, TORIF_CODE_T, ITAKU_NNAME_T FROM"
            If FSYORI_KBN = "1" Then
                SQL &= " TORIMAST_VIEW"
            Else
                SQL &= " S_TORIMAST_VIEW"
            End If
            SQL &= " WHERE FSYORI_KBN_T = " & SQ(FSYORI_KBN)

            If Jyoken.Trim <> "" Then
                SQL &= Space(1) & Jyoken
            End If

            If KanaKey.Trim <> Nothing Then
                SQL &= " AND SUBSTR(ITAKU_KNAME_T,1,1) = " & SQ(Trim(KanaKey))
            End If

            SQL &= " ORDER BY TORIS_CODE_T,TORIF_CODE_T"
            Console.WriteLine(SQL)
            Cmbbox.Items.Add(Space(50))
            '-----------------------------------------
            'コンボボックスにリストを追加する
            '-----------------------------------------
            If SetDynaset(SQL, REC) Then
                Do While REC.Read
                    Toris_Code = NzStr(REC.Item("TORIS_CODE_T")).Trim
                    Torif_Code = NzStr(REC.Item("TORIF_CODE_T")).Trim
                    Itaku_NName = NzStr(REC.Item("ITAKU_NNAME_T")).Trim
                    Cmbbox.Items.Add(Toris_Code & " - " & Torif_Code & " " & Itaku_NName.Trim)
                Loop

            ElseIf REC Is Nothing Then
                Return -1
            End If

            '-----------------------------------------
            'コンボボックスに1番最初のリストを表示する
            '-----------------------------------------
            If Cmbbox.Items.Count > 0 Then
                Cmbbox.Text = Cmbbox.Items.Item(0).ToString
            End If

            Ret = 0
        Catch ex As Exception
            Console.WriteLine(ex.Message)

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function

    '2018/01/17 タスク）西野 ADD 標準版修正：広島信金対応（中国新聞対応）---------------------- START
    ''' <summary>
    ''' 取引先コンボボックスの設定を行う（テーブル名指定）
    ''' </summary>
    ''' <param name="KanaKey">先頭カナ文字</param>
    ''' <param name="Cmbbox">コンボボックス名</param>
    ''' <param name="TORIS_CODE_T">取引先主コードテキストボックス</param>
    ''' <param name="TORIF_CODE_F">取引先副コードテキストボックス</param>
    ''' <param name="TABLE_NAME">対象の取引先マスタ</param>
    ''' <param name="Jyoken">条件指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SelectItakuName_SelectTable(ByVal KanaKey As String, ByVal Cmbbox As ComboBox, _
                                    ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_F As TextBox, _
                                    ByVal TABLE_NAME As String, Optional ByVal Jyoken As String = "") As Integer

        '--------------------------------------------
        '委託者名がカナKEY値で始まる委託者名カナを検索
        '--------------------------------------------
        Dim Ret As Integer = -1
        Dim REC As OracleDataReader = Nothing
        Dim Toris_Code As String
        Dim Torif_Code As String
        Dim Itaku_NName As String
        Dim WhereFlg As Boolean = False     'SQLのWhere句が存在するか

        Try
            '-----------------------------------------
            '現在のコンボボックスをクリアにする
            '-----------------------------------------
            Cmbbox.Text = ""
            TORIS_CODE_T.Text = ""
            TORIF_CODE_F.Text = ""
            Cmbbox.Items.Clear()

            If String.IsNullOrEmpty(KanaKey) Then
                KanaKey = ""
            End If

            '必要な項目のみ取得する。
            Dim SQL As String = "SELECT TORIS_CODE_T, TORIF_CODE_T, ITAKU_NNAME_T FROM "
            SQL &= TABLE_NAME

            If Jyoken.Trim <> "" Then
                SQL &= " WHERE "
                SQL &= Space(1) & Jyoken
                WhereFlg = True
            End If

            If KanaKey.Trim <> "" Then
                If WhereFlg Then
                    SQL &= " AND "
                Else
                    SQL &= " WHERE "
                    WhereFlg = True
                End If
                SQL &= "SUBSTR(ITAKU_KNAME_T,1,1) = " & SQ(Trim(KanaKey))
            End If

            SQL &= " ORDER BY TORIS_CODE_T,TORIF_CODE_T"
            Console.WriteLine(SQL)
            Cmbbox.Items.Add(Space(50))
            '-----------------------------------------
            'コンボボックスにリストを追加する
            '-----------------------------------------
            If SetDynaset(SQL, REC) Then
                Do While REC.Read
                    Toris_Code = NzStr(REC.Item("TORIS_CODE_T")).Trim
                    Torif_Code = NzStr(REC.Item("TORIF_CODE_T")).Trim
                    Itaku_NName = NzStr(REC.Item("ITAKU_NNAME_T")).Trim
                    Cmbbox.Items.Add(Toris_Code & " - " & Torif_Code & " " & Itaku_NName.Trim)
                Loop

            ElseIf REC Is Nothing Then
                Return -1

            End If
            '-----------------------------------------
            'コンボボックスに1番最初のリストを表示する
            '-----------------------------------------
            If Cmbbox.Items.Count > 0 Then
                Cmbbox.Text = Cmbbox.Items.Item(0).ToString
            End If

            Ret = 0
        Catch ex As Exception
            Console.WriteLine(ex.Message)

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function
    '2018/01/17 タスク）西野 ADD 標準版修正：広島信金対応（中国新聞対応）---------------------- END

    '
    ' 機能　　　: 取引先コンボボックス設定関数
    '
    ' 戻り値　　: 0 = OK, -1 = Err
    '
    ' 引き数　　: ARG1 - 先頭カナ文字
    ' 　　　 　　 ARG2 - コンボボックス名
    '             ARG3 - 取引先主コードテキストボックス
    '             ARG4 - 取引先副コードテキストボックス
    '             ARG5 - 処理区分(1:自振 3:総振)
    '             ARG6 - 媒体コード
    ' 備考　　　: T-Sakai追加
    Public Function Set_TORI_CODE(ByVal Cmbbox As ComboBox, ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_T As TextBox) As Integer
        Dim Ret As Integer = -1
        Try
            '-----------------------------------------
            '現在のテキストボックスをクリアにする
            '-----------------------------------------
            TORIS_CODE_T.Text = ""
            TORIF_CODE_T.Text = ""

            '-----------------------------------------------------------------------
            'コンボボックスからリストを取得し、取得値をテキストボックスに設定する
            '-----------------------------------------------------------------------
            Dim strTORI_CODE As String
            strTORI_CODE = Mid(Cmbbox.SelectedItem.ToString, 1, 15)
            TORIS_CODE_T.Text = Mid(strTORI_CODE, 1, 10).Trim
            TORIF_CODE_T.Text = Mid(strTORI_CODE, 14, 2).Trim
            If strTORI_CODE.Trim = "" Then
                TORIS_CODE_T.Text = ""
                TORIF_CODE_T.Text = ""
            End If
            Ret = 0
        Catch ex As Exception

        End Try

    End Function


    '
    ' 機能　　　: 口座チェック関数(エントリで使用)
    '
    ' 戻り値　　: 0:異常なし 1:口座無し 2:自振契約無し 3:口座解約済 -1:異常 (必要であれば順次追加)
    '
    ' 引き数　　: ARG1 - 支店コード
    ' 　　　 　　 ARG2 - 科目コード（2桁）
    '             ARG3 - 口座番号
    '             ARG4 - 企業コード
    '             ARG5 - 振替コード
    '             ARG6 - 契約者カナ氏名
    '             ARG7 - エラーメッセージ
    ' 備考　　　: 
    '
    Public Function KouzaChk_ENTRY(ByVal SitCode As String, ByVal Kamoku As String, _
                             ByVal Kouza As String, ByVal KigyoCode As String, _
                             ByVal FuriCode As String, ByRef KokyakuName As String, _
                             ByRef ErrMsg As String, ByVal MainDB As MyOracle) As Integer
        Dim Ret As Integer = -1

        Dim OraReader As New MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Dim KatuKouzaD As String = "0"

        Try
            SQL.Append("SELECT *")
            SQL.Append(" FROM KDBMAST")
            SQL.Append(" WHERE TSIT_NO_D = " & SQ(SitCode))
            SQL.Append(" AND KAMOKU_D = " & SQ(Kamoku))
            SQL.Append(" AND KOUZA_D = " & SQ(Kouza))
            '20130712 maeda 活口座判定修正
            SQL.Append(" ORDER BY KATU_KOUZA_D ")
            '20130712 maeda 活口座判定修正

            If OraReader.DataReader(SQL) = True Then
                Do
                    KokyakuName = NzStr(OraReader.GetString("KOKYAKU_KNAME_D"))
                    KatuKouzaD = NzStr(OraReader.GetString("KATU_KOUZA_D"))

                    If FuriCode.Trim = NzStr(OraReader.GetString("FURI_CODE_D")).Trim AndAlso _
                       KigyoCode.Trim = NzStr(OraReader.GetString("KIGYOU_CODE_D")).Trim AndAlso _
                       NzStr(OraReader.GetString("KATU_KOUZA_D")) = "1" Then
                        ErrMsg = ""
                        Ret = 0    '口座あり
                        Exit Try
                    End If
                Loop Until OraReader.NextRead = False
                '2011/06/16 標準版修正 活口座チェック追加 ------------------START
                If KatuKouzaD = "0" Then
                    ErrMsg = "口座解約済"
                    Ret = 3 '口座解約済
                    Exit Try
                End If
                '2011/06/16 標準版修正 活口座チェック追加 ------------------END

                ErrMsg = "自振契約無し"
                Ret = 2 '自振契約無し
                Exit Try
            End If

            ErrMsg = "口座無し"
            Ret = 1 '口座無し
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        '20130717 移管チェック追加 maeda
        If Ret = 1 OrElse Ret = 3 Then
            OraReader = New MyOracleReader(MainDB)

            Try
                SQL.Length = 0
                SQL.Append("SELECT *")
                SQL.Append(" FROM KDBMAST")
                SQL.Append(" WHERE OLD_TSIT_NO_D = " & SQ(SitCode))
                SQL.Append(" AND KAMOKU_D = " & SQ(Kamoku))
                SQL.Append(" AND OLD_KOUZA_D = " & SQ(Kouza))
                '20130712 maeda 活口座判定修正
                SQL.Append(" ORDER BY KATU_KOUZA_D ")
                '20130712 maeda 活口座判定修正

                KatuKouzaD = "0"

                If OraReader.DataReader(SQL) = True Then
                    Do
                        KokyakuName = NzStr(OraReader.GetString("KOKYAKU_KNAME_D"))
                        KatuKouzaD = NzStr(OraReader.GetString("KATU_KOUZA_D"))

                        If FuriCode.Trim = NzStr(OraReader.GetString("FURI_CODE_D")).Trim AndAlso _
                           KigyoCode.Trim = NzStr(OraReader.GetString("KIGYOU_CODE_D")).Trim AndAlso _
                           NzStr(OraReader.GetString("KATU_KOUZA_D")) = "1" Then
                            ErrMsg = "移管済"
                            Ret = 6    '口座あり
                            Exit Try
                        End If
                    Loop Until OraReader.NextRead = False
                    '2011/06/16 標準版修正 活口座チェック追加 ------------------START
                    If KatuKouzaD = "0" Then
                        ErrMsg = "(移管)解約済"
                        Ret = 5 '口座解約済
                        Exit Try
                    End If
                    '2011/06/16 標準版修正 活口座チェック追加 ------------------END

                    ErrMsg = "(移管)自振契約無"
                    Ret = 4 '自振契約無し
                    Exit Try
                End If

            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return -1
            Finally
                If Not OraReader Is Nothing Then OraReader.Close()
            End Try
        End If
        '20130717 移管チェック追加 maeda

        Return Ret
    End Function

    ''' <summary>
    ''' チェックデジット関数(エントリで使用)
    ''' </summary>
    ''' <param name="KinCode">金庫コード</param>
    ''' <param name="SitCode">支店コード</param>
    ''' <param name="Kamoku">科目（2桁）</param>
    ''' <param name="Kouza">口座番号</param>
    ''' <returns>True or False</returns>
    ''' <remarks>ClsFUSIONより移行</remarks>
    Public Function ChkDejit_ENTRY(ByVal KinCode As String, _
                                   ByVal SitCode As String, _
                                   ByVal Kamoku As String, _
                                   ByVal Kouza As String) As Boolean
        Try
            Dim strKINKO_OMOMI As String = "9874"
            Dim strTENPO_OMOMI As String = "732"
            Dim strKAMOKU_OMOMI As String = "19"
            Dim strKOUZA_OMOMI As String = "387432"

            Dim intKINKO_ATAI(4) As Integer
            Dim intTENPO_ATAI(3) As Integer
            Dim intKAMOKU_ATAI(2) As Integer
            Dim intKOUZA_ATAI(6) As Integer

            Dim intGOUKEI As Integer
            Dim intCHK_DEJIT As Integer

            For i As Integer = 1 To 4
                intKINKO_ATAI(i) = NzInt(strKINKO_OMOMI.Substring(i - 1, 1)) * NzInt(KinCode.Substring(i - 1, 1))
            Next
            For i As Integer = 1 To 3
                intTENPO_ATAI(i) = NzInt(strTENPO_OMOMI.Substring(i - 1, 1)) * NzInt(SitCode.Substring(i - 1, 1))
            Next
            Select Case Kamoku
                Case "01"
                    intKAMOKU_ATAI(1) = 0
                    intKAMOKU_ATAI(2) = 9
                Case "02"
                    intKAMOKU_ATAI(1) = 0
                    intKAMOKU_ATAI(2) = 18
                Case "05", "37"
                    intKAMOKU_ATAI(1) = 0
                    intKAMOKU_ATAI(2) = 18
                Case Else
                    'チェックデジット対象外
                    Return True
            End Select
            For i As Integer = 1 To 6
                intKOUZA_ATAI(i) = NzInt(strKOUZA_OMOMI.Substring(i - 1, 1)) * NzInt(Kouza.Substring(i - 1, 1))
            Next

            intGOUKEI = intKINKO_ATAI(1) + intKINKO_ATAI(2) + intKINKO_ATAI(3) + intKINKO_ATAI(4) _
                      + intTENPO_ATAI(1) + intTENPO_ATAI(2) + intTENPO_ATAI(3) _
                      + intKAMOKU_ATAI(1) + intKAMOKU_ATAI(2) _
                      + intKOUZA_ATAI(1) + intKOUZA_ATAI(2) + intKOUZA_ATAI(3) + intKOUZA_ATAI(4) + intKOUZA_ATAI(5) + intKOUZA_ATAI(6)

            intCHK_DEJIT = 10 - (intGOUKEI Mod 10)

            If intCHK_DEJIT = 10 Then
                intCHK_DEJIT = 0
            End If
            If intCHK_DEJIT = NzInt(Kouza.Substring(6, 1)) Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 振込手数料基準IDコンボボックス設定
    ''' </summary>
    ''' <param name="aComboBox">コンボボックスオブジェクト</param>
    ''' <param name="aTaxID">税率ID</param>
    ''' <param name="aItemData">初期設定インデックス</param>
    ''' <param name="aFSyoriKbn">処理区分(自振:1(デフォルト), 総振:3)</param>
    ''' <returns>0 - 正常, 2 - 異常</returns>
    ''' <remarks>2013/11/27 標準版 消費税対応 ADD</remarks>
    Public Function SetComboBox_TESUU_TABLE_ID_T(ByVal aComboBox As ComboBox, _
                                                 ByVal aTaxID As String, _
                                                 ByVal aItemData As Integer, _
                                                 Optional ByVal aFSyoriKbn As String = "1") As Integer

        '--------------------------------------------------
        '税率IDに紐付く振込手数料を取得
        '--------------------------------------------------
        Dim REC As OracleDataReader = Nothing
        Dim TESUU_TABLE_ID As Integer = -1
        Dim TESUU_TABLE_NAME As String = String.Empty

        Try
            aComboBox.Items.Clear()
            Dim SQL As String = "SELECT * FROM TESUUMAST, TAXMAST"
            SQL &= " WHERE TAX_ID_C = TAX_ID_Z"
            SQL &= " AND TAX_ID_C = " & SQ(aTaxID)
            SQL &= " AND FSYORI_KBN_C = " & SQ(aFSyoriKbn)
            If aFSyoriKbn = "3" Then
                '総振の場合は振込手数料に限定
                SQL &= " AND SYUBETU_C = '10'"
            End If
            SQL &= " ORDER BY TESUU_TABLE_ID_C"

            '-----------------------------------------
            'コンボボックスにリストを追加する
            '-----------------------------------------

            '振込手数料を設定しない空白固定パターン
            Dim Item As New clsAddItem("", TESUU_TABLE_ID)
            aComboBox.Items.Add(Item)

            If SetDynaset(SQL, REC) Then
                Do While REC.Read
                    TESUU_TABLE_ID = NzInt(REC.Item("TESUU_TABLE_ID_C"))
                    TESUU_TABLE_NAME = NzStr(REC.Item("TESUU_TABLE_NAME_C")).Trim

                    Item = New clsAddItem(TESUU_TABLE_NAME, TESUU_TABLE_ID)
                    aComboBox.Items.Add(Item)
                Loop

                Dim Cnt As Integer
                For Cnt = 0 To aComboBox.Items.Count - 1 Step 1
                    aComboBox.SelectedIndex = Cnt
                    If GetComboBox(aComboBox) = aItemData Then
                        Exit For
                    End If
                Next Cnt

                If Cnt >= aComboBox.Items.Count AndAlso aComboBox.Items.Count > 0 Then
                    aComboBox.SelectedIndex = -1
                End If

            '*** Str Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***
            ElseIf REC Is Nothing Then
                Return 2
            '*** End Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***

            End If

            Return 0
        Catch ex As Exception
            With GLog
                .Job2 = "該当ComboBoxの項目設定"
                .Result = NG
                .Discription = aComboBox.Name & " : " & ex.Message
            End With
            Return 2
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Function

    ''' <summary>
    ''' 振込手数料検索コンボボックスを設定します。
    ''' </summary>
    ''' <param name="CmbBox">振込手数料検索コンボボックス</param>
    ''' <param name="txtTAX_ID">税率IDテキストボックス</param>
    ''' <param name="txtTESUU_TABLE_ID">手数料IDテキストボックス</param>
    ''' <param name="FSYORI_KBN">処理区分(1:自振 , 3:総振)</param>
    ''' <returns>0:正常 , -1:異常</returns>
    ''' <remarks>2013/12/02 消費税対応 ADD</remarks>
    Public Function SelectTesuuName(ByVal CmbBox As ComboBox, _
                                    ByVal txtTAX_ID As TextBox, _
                                    ByVal txtTESUU_TABLE_ID As TextBox, _
                                    Optional ByVal FSYORI_KBN As String = "1") As Integer
        Dim Ret As Integer = -1
        Dim REC As OracleDataReader = Nothing
        Dim strTaxID As String = String.Empty
        Dim strTesuuTableID As String = String.Empty
        Dim strTesuuTableName As String = String.Empty
        Try
            '--------------------------------------------------
            '現在のコンボボックスをクリアにする
            '--------------------------------------------------
            CmbBox.Text = ""
            txtTAX_ID.Text = ""
            txtTESUU_TABLE_ID.Text = ""
            CmbBox.Items.Clear()

            Dim SQL As String = "SELECT * FROM TAXMAST, TESUUMAST"
            SQL &= " WHERE TAX_ID_Z = TAX_ID_C"
            If FSYORI_KBN = "1" Then
                SQL &= " AND FSYORI_KBN_C = " & SQ(FSYORI_KBN)
            Else
                SQL &= " AND FSYORI_KBN_C = " & SQ(FSYORI_KBN)
                SQL &= " AND SYUBETU_C = '10'"      '総振は振込手数料限定
            End If
            SQL &= " ORDER BY TAX_ID_C, TESUU_TABLE_ID_C"

            Console.WriteLine(SQL)
            CmbBox.Items.Add(Space(50))

            '--------------------------------------------------
            'コンボボックスに追加する
            '--------------------------------------------------
            If SetDynaset(SQL, REC) Then
                Do While REC.Read
                    strTaxID = NzStr(REC.Item("TAX_ID_C")).Trim
                    strTesuuTableID = NzStr(REC.Item("TESUU_TABLE_ID_C")).Trim
                    strTesuuTableName = NzStr(REC.Item("TESUU_TABLE_NAME_C")).Trim
                    CmbBox.Items.Add(strTaxID & " - " & strTesuuTableID & "　" & Mid(strTesuuTableName.Trim, 1, 15))
                Loop

            '*** Str Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***
            ElseIf REC Is Nothing Then
                Return -1
            '*** End Add 2015/12/01 SO)荒木 for 潜在障害（SQLエラー時に画面側でエラーメッセージが出ない） ***

            End If

            '--------------------------------------------------
            'コンボボックスに1番最初のリストを表示する
            '--------------------------------------------------
            If CmbBox.Items.Count > 0 Then
                CmbBox.Text = CmbBox.Items.Item(0).ToString
            End If

            Ret = 0
        Catch ex As Exception
            Console.WriteLine(ex.Message)

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function

    ''' <summary>
    ''' 振込手数料コンボボックス設定関数
    ''' </summary>
    ''' <param name="CmbBox">振込手数料検索コンボボックス</param>
    ''' <param name="txtTAX_ID">税率IDテキストボックス</param>
    ''' <param name="txtTESUU_TABLE_ID">手数料IDテキストボックス</param>
    ''' <returns>0:正常, -1:異常</returns>
    ''' <remarks>2013/12/02 消費税対応 ADD</remarks>
    Public Function Set_TESUU_CODE(ByVal CmbBox As ComboBox, _
                                   ByVal txtTAX_ID As TextBox, _
                                   ByVal txtTESUU_TABLE_ID As TextBox) As Integer
        Dim Ret As Integer = -1
        Try
            '--------------------------------------------------
            '現在のテキストボックスをクリアにする
            '--------------------------------------------------
            txtTAX_ID.Text = ""
            txtTESUU_TABLE_ID.Text = ""

            '----------------------------------------------------------------------
            'コンボボックスからリストを取得し、取得値をテキストボックスに設定する
            '----------------------------------------------------------------------
            Dim strTesuuLine As String
            strTesuuLine = CmbBox.SelectedItem.ToString
            Dim strTesuuItem1() As String = strTesuuLine.Split("-"c)
            If strTesuuItem1.Length >= 2 Then
                txtTAX_ID.Text = strTesuuItem1(0).Trim
                Dim strTesuuItem2() As String = strTesuuItem1(1).Split("　"c)
                If strTesuuItem2.Length >= 2 Then
                    txtTESUU_TABLE_ID.Text = strTesuuItem2(0).Trim
                End If
            Else
                txtTAX_ID.Text = ""
                txtTESUU_TABLE_ID.Text = ""
            End If
            Ret = 0
        Catch ex As Exception

        End Try

    End Function

    '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ START
    ''' <summary>
    ''' 指定した画面／帳票の個別情報ををINIファイルより取得する
    ''' </summary>
    ''' <param name="OBJ_ID">画面／帳票ID</param>
    ''' <param name="KEY_NAME">項目名</param>
    ''' <param name="MODE">0:画面、1:帳票</param>
    ''' <returns>ソートキー</returns>
    ''' <remarks></remarks>
    Public Function GetObjectParam(ByVal OBJ_ID As String, ByVal KEY_NAME As String, ByVal MODE As String) As String
        Dim INI_KEY As String = ""

        Select Case MODE
            Case "0"    '画面
                INI_KEY = "FORM"
            Case "1"    '帳票
                INI_KEY = "PRINT"
            Case Else   '上記以外は抜ける
                Return ""
        End Select

        Dim wkStr As String = GetRSKJIni(INI_KEY, OBJ_ID & "_" & KEY_NAME)
        Select Case wkStr
            Case "err", ""
                Return ""
            Case Else
                Return wkStr
        End Select
    End Function
    '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ END
    '2017/04/26 タスク）西野 ADD 標準版修正（From-ToのINI化）------------------------------------ START
    ''' <summary>
    ''' 指定した画面で使用する期間のFrom-Toを取得する
    ''' </summary>
    ''' <param name="FORM_ID">画面ID</param>
    ''' <param name="RET_FROM">何営業日前かを返却する</param>
    ''' <param name="RET_TO">何営業日後かを返却する</param>
    ''' <param name="DEF_FROM">INIファイルからの取得エラー時の返却値</param>
    ''' <param name="DEF_TO">INIファイルからの取得エラー時の返却値</param>
    ''' <remarks>INIファイルから取得できない場合は、DEF_FROM/DEF_TOの値を返却します</remarks>
    Public Sub GetFromTo(ByVal FORM_ID As String, ByRef RET_FROM As Integer, ByRef RET_TO As Integer, ByVal DEF_FROM As Integer, ByVal DEF_TO As Integer)
        Dim wkFROM As String = GetRSKJIni("FROM_TO", FORM_ID & "_FROM")
        Dim wkTO As String = GetRSKJIni("FROM_TO", FORM_ID & "_TO")

        'FROMの設定
        Select Case wkFROM
            Case "err", ""
                RET_FROM = DEF_FROM
            Case Else
                RET_FROM = CInt(wkFROM)
        End Select

        'TOの設定
        Select Case wkTO
            Case "err", ""
                RET_TO = DEF_TO
            Case Else
                RET_TO = CInt(wkTO)
        End Select

    End Sub
    '2017/04/26 タスク）西野 ADD 標準版修正（From-ToのINI化）------------------------------------ END

End Class

'--------------------------------------------------------------------------------------------------
'' 各画面のコンボボックス格納値制御で必要となる関数 2007.10.05 By K.Seto
'--------------------------------------------------------------------------------------------------
Public Class clsAddItem
    '
    ' 機　能 : コンボボックス制御クラス
    '
    ' 備　考 : 設定／参照に使用する
    '    
    Public Item As String           '表示テキスト
    Public Data1 As Integer         '記憶値(Number)
    Public Data2 As Integer         '記憶値(Number)
    Public Data3 As String          '記憶値(String)

    Public Sub New(ByVal aItem As String, ByVal aData1 As Integer, _
        Optional ByVal aData2 As Integer = 0, Optional ByVal aData3 As String = "")
        Item = aItem
        Data1 = aData1
        Data2 = aData2
        Data3 = aData3
    End Sub

    '
    ' 機　能 : 該当INDEXの項目情報参照
    '
    ' 戻り値 : 該当INDEXの項目情報
    '
    ' 引き数 : ARG1 - なし
    '
    ' 備　考 : コンボボックス共通関数
    '    
    Public Overrides Function ToString() As String
        Return Item
    End Function


End Class
