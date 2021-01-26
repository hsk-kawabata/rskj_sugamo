Imports System
Imports System.IO
Imports System.Diagnostics
Imports System.Data.OracleClient
'*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
Imports System.Threading
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

' バッチログ クラス
Public Class BatchLOG

#Region "ログの出力について"
    '①処理時間(時分秒)
    '②プロセスID(OS付与番号)
    '③ジョブ通番(バッチのみ(ジョブマスタに登録した番号)
    '④処理端末(コンピュータ名)
    '⑤ログインユーザ名
    '⑥取引先コード(XXXXXXXXXX-XX形式)
    '⑦振替日(振込日/処理日/基準日)(yyyymmdd形式)
    '⑧処理モジュール(画面(画面ID),バッチ(Exe名))
    '⑨処理名(システム機能一覧(システム機能名(システム機能ID)))
    '⑩処理内容(処理内容+開始OR処理内容+終了)
    '⑪処理結果
    '  ・印刷/データ作成系については処理件数を付加する(成功+(******件))
    '⑫備考(バッチならパラメータなど?,失敗ならエラー内容)

    '※通番は廃止します
#End Region

    ' 公開変数
    Public PID As Integer                 ' プロセスID(OS付与番号)
    '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
    'Private mTuuban As Integer           ' ジョブ通番(バッチのみ(ジョブマスタに登録した番号)
    Private Shared mTuuban As Integer = 0 ' ジョブ通番(バッチのみ(ジョブマスタに登録した番号)
    '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
    Public HostName As String             ' 処理端末(コンピュータ名)
    Public UserID As String               ' ログインユーザ名
    Private mToriCode As String           ' 取引先コード
    Public Property ToriCode() As String
        Get
            Return mToriCode
        End Get
        Set(ByVal Value As String)
            If Value.TrimEnd.Length = 12 Then
                mToriCode = Value.Insert(10, "-")
            Else
                mToriCode = Value
            End If
        End Set
    End Property

    Public FuriDate As String             ' 振替日(振込日/処理日/基準日)
    Public ModuleName As String           ' 処理モジュール(画面(画面ID),バッチ(Exe名))
    Public SyoriName As String            ' 処理名(システム機能一覧(システム機能名(システム機能ID)))

    ' ログパス
    Private ReadOnly LOGPATH As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "LOG"), DateTime.Today.ToString("yyyyMMdd"))
    ' ログファイル名
    '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
    'Private LogName As String
    Private Shared LogName As String = ""
    '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

    Private LogNamePre As String = ""

    Private ReadOnly kai As String = System.Text.Encoding.ASCII.GetString(New Byte() {13, 10})

    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
    ' ログレベル定数
    Private ReadOnly LOG_LEVEL1 As Integer = 1    ' 業務ロジックログ
    Private ReadOnly LOG_LEVEL2 As Integer = 2    ' 業務ロジック詳細ログ
    Private ReadOnly LOG_LEVEL3 As Integer = 3    ' 共通モジュールログ
    Private ReadOnly LOG_LEVEL4 As Integer = 4    ' 共通モジュール詳細ログ、デバッグログレベル

    ' iniファイルで指定されたログレベル
    Private mLogLevel As Integer = 1      ' トレースログレベル
    Private mPfmLog As Integer = 1        ' 性能ログレベル
    Private mSqlLog As Integer = 0        ' SQLログレベル

    Private mRetryCnt As Integer = 10     ' ログ排他待ちリトライ回数
    Private mWaitTime As Integer = 10     ' ログ排他待ち時間（ミリ秒）

    ' マルチスレッドからの同時ログ出力の排他用
    Private Shared mWriteLock As New Object 
    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
    Private mLockWaitTime As Integer = 30
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***


    ' 通知メッセージ
    Public JobMessage As String = ""

    'ジョブマスタ用
    Public Enum JobStatus
        NormalEnd = 2                       ' 正常終了
        AbnormalEnd = 3                     ' 異常終了
    End Enum

    ' 通番
    Public Property JobTuuban() As Integer
        Get
            Return mTuuban
        End Get
        Set(ByVal Value As Integer)
            mTuuban = Value

            If mTuuban > 0 Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                ' ログファイル名
                LogName = Path.Combine(LOGPATH, LogNamePre & "." & mTuuban.ToString("000000") & ".LOG")

                Dim sw As System.Diagnostics.Stopwatch
                sw = Write_Enter3("ClsBatchLOG.JobTuuban")
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                ' 通番からJOBマスタのユーザIDを取得する
                Dim OraDB As New MyOracle

                Try
                    Dim SQL As String
                    OraDB.Connect()
                    SQL = "SELECT " _
                        & " USERID_J" _
                        & " FROM JOBMAST" _
                        & " WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE, 'YYYYMMDD')" _
                        & "   AND TUUBAN_J = " & mTuuban

                    Dim SQLReader As New CASTCommon.MyOracleReader(OraDB)
                    If SQLReader.DataReader(SQL) = True Then
                        UserID = SQLReader.GetString("USERID_J")
                    Else
                        Throw New Exception("JOBMASTが見つかりません TUUBAN_J = " & mTuuban)
                        UserID = ""
                    End If
                    SQLReader.Close()
                Catch ex As Exception
                    Throw New Exception("JOBMASTが見つかりません TUUBAN_J = " & mTuuban & " " & ex.Message)
                Finally
                    OraDB.Close()
                End Try

                OraDB = Nothing

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                Write_Exit3(sw, "ClsBatchLOG.JobTuuban")
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
                '' ログファイル名
                'LogName = Path.Combine(LOGPATH, LogNamePre & "." & mTuuban.ToString("000000") & ".LOG")
                '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***

            Else
                UserID = ""

                ' ログファイル名
                LogName = Path.Combine(LOGPATH, LogNamePre & ".000000.LOG")
            End If
        End Set
    End Property

    ' New
    Public Sub New(Optional ByVal ModuleName As String = "", Optional ByVal SyoriName As String = "")
        LogNamePre = Process.GetCurrentProcess.ProcessName

        ' ログファイル名
        '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
        'LogName = Path.Combine(LOGPATH, LogNamePre & ".000000.LOG")
        If LogName = "" Then
            LogName = Path.Combine(LOGPATH, LogNamePre & ".000000.LOG")
        End If
        '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        PID = Process.GetCurrentProcess.Id
        '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
        'mTuuban = 0
        '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***
        HostName = Environment.MachineName
        UserID = ""
        mToriCode = ""
        FuriDate = ""
        If ModuleName = "" Then
            Me.ModuleName = LogNamePre
        Else
            Me.ModuleName = ModuleName
        End If
        Me.SyoriName = SyoriName

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sWork As String
        ' SQLログ出力チェック
        If LogNamePre.ToUpper.StartsWith("JOBKANSHI") Then
            sWork = CASTCommon.GetFSKJIni("COMMON", "JOBKANSHI_SQLLOG")
        Else
            sWork = CASTCommon.GetFSKJIni("COMMON", "SQLLOG")
        End If
        sWork = sWork.Trim
        If sWork.ToUpper = "YES" Then
            mSqlLog = 1
        End If

        ' 性能ログ出力チェック
        If LogNamePre.ToUpper.StartsWith("JOBKANSHI") Then
            mPfmLog = 0
        Else
            sWork = CASTCommon.GetFSKJIni("COMMON", "PFMLOGLEVEL")
            sWork = sWork.Trim
            If IsNumeric(sWork) Then
                mPfmLog = CInt(sWork)
                If mPfmLog < 0 OrElse mPfmLog > 4 Then
                    mPfmLog = 1
                End If
            End If
        End If

        ' ログレベルチェック
        If LogNamePre.ToUpper.StartsWith("JOBKANSHI") Then
            sWork = CASTCommon.GetFSKJIni("COMMON", "JOBKANSHI_LOGLEVEL")
        Else
            sWork = CASTCommon.GetFSKJIni("COMMON", "LOGLEVEL")
        End If
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mLogLevel = CInt(sWork)
            If mLogLevel < 0 OrElse mLogLevel > 4 Then
                mLogLevel = 1
            End If
        End If

        ' ログ排他待ちリトライ回数
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOG_RETRY_COUNT")
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mRetryCnt = CInt(sWork)
            If mRetryCnt < 1 OrElse mRetryCnt > 100 Then
                mRetryCnt = 10
            End If
        End If

        ' ログ排他待ち時間（ミリ秒）
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOG_WAIT_TIME")
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mWaitTime = CInt(sWork)
            If mWaitTime < 1 OrElse mWaitTime > 1000 Then
                mWaitTime = 10
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            mLockWaitTime = CInt(sWork)
            If mLockWaitTime <= 0 Then
                mLockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***


        '*** Str Del 2015/12/01 SO)荒木 for ログ強化（冗長なログ削除） ***
        'Write("開始", "成功")
        '*** End Del 2015/12/01 SO)荒木 for ログ強化（冗長なログ削除） ***

    End Sub

    Public Sub New(ByVal ModuleName As String, ByVal SyoriName As String, ByVal LogFileName As String)

        If LogFileName = "" Then
            LogNamePre = Process.GetCurrentProcess.ProcessName
        Else
            LogNamePre = LogFileName
        End If

        ' ログファイル名
        If LogName = "" Then
            LogName = Path.Combine(LOGPATH, LogNamePre & ".000000.LOG")
        End If

        PID = Process.GetCurrentProcess.Id
        HostName = Environment.MachineName
        UserID = ""
        mToriCode = ""
        FuriDate = ""
        If ModuleName = "" Then
            Me.ModuleName = LogNamePre
        Else
            Me.ModuleName = ModuleName
        End If
        Me.SyoriName = SyoriName

        Dim sWork As String
        If LogNamePre.ToUpper.StartsWith("JOBKANSHI") Then
            sWork = CASTCommon.GetFSKJIni("COMMON", "JOBKANSHI_SQLLOG")
        Else
            sWork = CASTCommon.GetFSKJIni("COMMON", "SQLLOG")
        End If
        sWork = sWork.Trim
        If sWork.ToUpper = "YES" Then
            mSqlLog = 1
        End If

        ' 性能ログ出力チェック
        If LogNamePre.ToUpper.StartsWith("JOBKANSHI") Then
            mPfmLog = 0
        Else
            sWork = CASTCommon.GetFSKJIni("COMMON", "PFMLOGLEVEL")
            sWork = sWork.Trim
            If IsNumeric(sWork) Then
                mPfmLog = CInt(sWork)
                If mPfmLog < 0 OrElse mPfmLog > 4 Then
                    mPfmLog = 1
                End If
            End If
        End If

        ' ログレベルチェック
        If LogNamePre.ToUpper.StartsWith("JOBKANSHI") Then
            sWork = CASTCommon.GetFSKJIni("COMMON", "JOBKANSHI_LOGLEVEL")
        Else
            sWork = CASTCommon.GetFSKJIni("COMMON", "LOGLEVEL")
        End If
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mLogLevel = CInt(sWork)
            If mLogLevel < 0 OrElse mLogLevel > 4 Then
                mLogLevel = 1
            End If
        End If

        ' ログ排他待ちリトライ回数
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOG_RETRY_COUNT")
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mRetryCnt = CInt(sWork)
            If mRetryCnt < 1 OrElse mRetryCnt > 100 Then
                mRetryCnt = 10
            End If
        End If

        ' ログ排他待ち時間（ミリ秒）
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOG_WAIT_TIME")
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mWaitTime = CInt(sWork)
            If mWaitTime < 1 OrElse mWaitTime > 1000 Then
                mWaitTime = 10
            End If
        End If

        sWork = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            mLockWaitTime = CInt(sWork)
            If mLockWaitTime <= 0 Then
                mLockWaitTime = 30
            End If
        End If

    End Sub

    '
    ' 機能　 ： ログ出力
    '
    ' 引数　 ： Detail：処理内容／Result：結果／Message：備考
    '
    ' 備考　 ： 
    '
    Public Sub Write(ByVal Detail As String, ByVal Result As String, Optional ByVal Message As String = "")
        Call Write(UserID, mToriCode, FuriDate, Detail, Result, Message)
    End Sub

    '
    ' 機能　 ： ログ出力
    '
    ' 引数　 ： Detail：処理内容／Result：結果／Message：備考
    '
    ' 備考　 ： 
    '
    Public Sub Write(ByVal ToriCode As String, ByVal FuriDate As String, _
                     ByVal Detail As String, ByVal Result As String, Optional ByVal Message As String = "")
        Call Write(UserID, ToriCode, FuriDate, Detail, Result, Message)
    End Sub



    '
    ' 機能　 ： ログ出力
    '
    ' 引数　 ： aUserID：ユーザ名／aToriCode：取引先コード／aFuriDate：振替日
    '           ／Detail：処理内容／Result：結果／Message：備考
    '
    ' 備考　 ： 
    '
    Public Sub Write(ByVal UserID As String, ByVal ToriCode As String, _
                        ByVal FuriDate As String, _
                        ByVal Detail As String, ByVal Result As String, _
                        ByVal Message As String, Optional ByVal Tuuban As Integer = -1)

'*** Str Add 2016/01/20 SO)荒木 for ログ強化 ***

        If Not Tuuban = -1 Then
            JobTuuban = Tuuban
        End If

        If Result = "失敗" Then
            Call Write_Err(UserID, ToriCode, FuriDate, Detail, Result, Message)
        Else
            Call Write_LEVEL1(UserID, ToriCode, FuriDate, Detail, Result, Message)
        End If

    End Sub


    Public Sub Write(ByVal Detail As String, ByVal Result As String, ByVal ex As Exception)
        Call Write(UserID, mToriCode, FuriDate, Detail, Result, ex)
    End Sub

    Public Sub Write(ByVal ToriCode As String, ByVal FuriDate As String, _
                     ByVal Detail As String, ByVal Result As String, ByVal ex As Exception)
        Call Write(UserID, ToriCode, FuriDate, Detail, Result, ex)
    End Sub

    Public Sub Write(ByVal UserID As String, ByVal ToriCode As String, _
                        ByVal FuriDate As String, _
                        ByVal Detail As String, ByVal Result As String, _
                        ByVal ex As Exception, Optional ByVal Tuuban As Integer = -1)

        If Not Tuuban = -1 Then
            JobTuuban = Tuuban
        End If

        Call Write_Err(UserID, ToriCode, FuriDate, Detail, Result, ex)

    End Sub

    Private Sub WriteLog(ByVal UserID As String, ByVal ToriCode As String, _
                        ByVal FuriDate As String, _
                        ByVal Detail As String, ByVal Result As String, _
                        ByVal Message As String, Optional ByVal Tuuban As Integer = -1)
'*** End Add 2016/01/20 SO)荒木 for ログ強化 ***

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim oCSV As CSVBase = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Try
            ' 引数保存
            Me.UserID = UserID
            Me.ToriCode = ToriCode
            Me.FuriDate = FuriDate
            If Message Is Nothing Then
                Message = ""
            End If

            If Not Tuuban = -1 Then
                JobTuuban = Tuuban
            End If

            ' ディレクトリチェック
            If File.Exists(LOGPATH) = False Then
                Directory.CreateDirectory(LOGPATH)
            End If

            ' ログ出力
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim oCSV As New CSVBase(LogName)
            'Dim nRet As Integer = oCSV.Open(FileMode.Append)
            'If nRet = 0 Then
            '    ' １回だけリトライする
            '    nRet = oCSV.Open(FileMode.Append)
            'End If
            ' 排他オープンに変更（リトライはOpenLockメソッド内で行う）
            oCSV = New CSVBase(LogName)
            Dim nRet As Integer = oCSV.OpenLock(FileMode.Append, mRetryCnt, mWaitTime)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            oCSV.Output(DateTime.Now.ToString("HHmmss"), True)
            oCSV.Output(PID, True)
            If JobTuuban > 0 Then
                oCSV.Output(JobTuuban, True)
            Else
                oCSV.Output("", True)
            End If
            Dim WorkMessage As String = Message.Replace(Environment.NewLine, "→")
            WorkMessage = WorkMessage.Replace(kai.Substring(0, 1), "->")
            WorkMessage = WorkMessage.Replace(kai.Substring(1), "->")
            oCSV.OutputPlus(HostName, UserID, mToriCode, FuriDate, ModuleName, SyoriName, Detail, Result, WorkMessage)

            oCSV.Close()

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for クローズ忘れを修正（潜在） ***
            If Not oCSV Is Nothing Then
                oCSV.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for クローズ忘れを修正（潜在） ***

        End Try
    End Sub

    '
    ' 機能　 ： ジョブマスタ更新
    '
    ' 引数　 ： ARG1 - 通番タス
    '           ARG2 - ステータス
    '           ARG2 - メッセージ
    '
    ' 戻り値 ： True - 成功，False - 失敗
    '
    ' 備考　 ： 
    '
    Public Function UpdateJOBMASTbyOK(ByVal message As String) As Boolean
        Return UpdateJOBMAST(JobStatus.NormalEnd, message)
    End Function

    '
    ' 機能　 ： ジョブマスタ更新
    '
    ' 引数　 ： ARG1 - 通番タス
    '           ARG2 - ステータス
    '           ARG2 - メッセージ
    '
    ' 戻り値 ： True - 成功，False - 失敗
    '
    ' 備考　 ： 
    '
    Public Function UpdateJOBMASTbyErr(ByVal Message As String) As Boolean
        ' 2008.04.22 ADD イベントログ
        Try
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write(Message, Diagnostics.EventLogEntryType.Error)
        Catch ex As Exception

        End Try

        JobMessage = Message
        Return UpdateJOBMAST(JobStatus.AbnormalEnd, Message)
    End Function

    '
    ' 機能　 ： ジョブマスタ更新
    '
    ' 引数　 ： ARG1 - 通番タス
    '           ARG2 - ステータス
    '           ARG2 - メッセージ
    '
    ' 戻り値 ： True - 成功，False - 失敗
    '
    ' 備考　 ： 
    '
    Public Function UpdateJOBMASTbyErr(ByVal Message As String, ByVal Level As System.Diagnostics.EventLogEntryType) As Boolean
        ' 2008.04.22 ADD イベントログ
        Try
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write(Message, Level)
        Catch ex As Exception

        End Try

        JobMessage = Message
        Return UpdateJOBMAST(JobStatus.AbnormalEnd, Message)
    End Function

    '
    ' 機能　 ： ジョブマスタ更新
    '
    ' 引数　 ： ARG1 - 通番タス
    '           ARG2 - ステータス
    '           ARG2 - メッセージ
    '
    ' 戻り値 ： True - 成功，False - 失敗
    '
    ' 備考　 ： 
    '
    Public Function UpdateJOBMAST(ByVal Status As JobStatus, ByVal Message As String) As Boolean

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Dim oDB As New CASTCommon.MyOracle

        Dim SQL As String
        Try
            ' ジョブマスタ更新
            SQL = "UPDATE JOBMAST SET " _
                & " END_DATE_J ='" & DateTime.Now.ToString("yyyyMMdd") & "'" _
                & ",END_TIME_J ='" & DateTime.Now.ToString("HHmmss") & "'" _
                & ",STATUS_J ='" & Status & "'" _
                & ",ERRMSG_J ='" & Cutting(Message & New String(" "c, 200), 200) & "'" _
                & " WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE, 'YYYYMMDD')" _
                & "   AND TUUBAN_J = " & JobTuuban.ToString

            Dim nRet As Integer = oDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                Call Write("JOBMAST更新", "データなし", Message)
            ElseIf nRet < 0 Then
                Call Write("JOBMAST更新", "失敗", oDB.Message)
                Return False
            End If
            oDB.Commit()
        Catch ex As Exception
            oDB.Rollback()
            Call Write("JOBMAST更新", "失敗", Message & " " & ex.Message)
            Return False
        Finally
            oDB.Close()
        End Try

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Write_Exit1(sw, "ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Return True
    End Function

    '
    ' 機能　 ： ジョブマスタ更新
    '
    ' 引数　 ： ARG1 - メッセージ
    '
    ' 戻り値 ： True - 成功，False - 失敗
    '
    ' 備考　 ： 
    '
    Public Function UpdateJOBMAST(ByVal Message As String) As Boolean

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Dim oDB As New CASTCommon.MyOracle

        Dim SQL As String
        Try
            ' ジョブマスタ更新
            SQL = "UPDATE JOBMAST SET " _
                & "ERRMSG_J ='" & Cutting(Message & New String(" "c, 200), 200) & "'" _
                & " WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE, 'YYYYMMDD')" _
                & "   AND TUUBAN_J = " & JobTuuban.ToString

            Dim nRet As Integer = oDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                Call Write("JOBMAST更新", "データなし", Message)
            ElseIf nRet < 0 Then
                Call Write("JOBMAST更新", "失敗", oDB.Message)
                Return False
            End If
            oDB.Commit()
        Catch ex As Exception
            oDB.Rollback()
            Call Write("JOBMAST更新", "失敗", Message & " " & ex.Message)
            Return False
        Finally
            oDB.Close()
        End Try

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Write_Exit1(sw, "ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Return True
    End Function

    Public Function InsertJOBMAST(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle) As Boolean
        '=====================================================================================
        'NAME           :InsertJOBMAST
        'Parameter      :jobid：起動するジョブＩＤ／userid：ログインユーザ
        '　　　　　　　　:para：パラメータ
        'Description    :ジョブマスタにジョブを登録する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/07/14
        'Update         :
        '=====================================================================================

        'Dim oDB As New CASTCommon.MyOracle
        Dim sql As String

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.InsertJOBMAST")
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
        Dim dblock As CASTCommon.CDBLock = Nothing
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

        Try

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
            ' ジョブ登録ロック
            dblock = New CASTCommon.CDBLock
            If dblock.InsertJOBMAST_Lock(db, mLockWaitTime) = False Then
                Call Write_Err("JOBMAST登録", "失敗", "タイムアウト")
                Return False
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

            sql = "INSERT INTO JOBMAST"
            sql &= "("
            sql &= " TUUBAN_J      "
            sql &= ",TOUROKU_DATE_J"
            sql &= ",TOUROKU_TIME_J"
            sql &= ",STA_DATE_J    "
            sql &= ",STA_TIME_J    "
            sql &= ",END_DATE_J    "
            sql &= ",END_TIME_J    "
            sql &= ",JOBID_J       "
            sql &= ",STATUS_J      "
            sql &= ",USERID_J      "
            sql &= ",PARAMETA_J    "
            sql &= ",ERRMSG_J      "
            sql &= ")"

            sql &= " VALUES ('1'"
            sql &= ",TO_CHAR(SYSDATE,'YYYYMMDD')"
            sql &= ",'" & System.DateTime.Now.ToString("HHmmss") & "'"
            sql &= ",'00000000'"
            sql &= ",'000000'"
            sql &= ",'00000000'"
            sql &= ",'000000'"
            sql &= ",'" & jobid & "'"
            sql &= ",'0'"
            sql &= ",'" & userid & "'"
            sql &= ",'" & para & "'"
            sql &= ",' '"
            sql &= ")"

            Dim nRet As Integer = db.ExecuteNonQuery(sql)
            If nRet = 0 Then
                Call Write("JOBMAST登録", "データなし", "")
            ElseIf nRet < 0 Then
                Call Write("JOBMAST登録", "失敗", db.Message)
                Return False
            End If
            'oDB.Commit()

        Catch ex As Exception
            'oDB.Rollback()
            Call Write("JOBMAST登録", "失敗", "ジョブマスタの登録に失敗しました　" & ex.Message)
            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
            If Not dblock Is Nothing Then
                ' ジョブ登録アンロック
                dblock.InsertJOBMAST_UnLock(db)
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
            'oDB.Close()

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            Write_Exit1(sw, "ClsBatchLOG.InsertJOBMAST")
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
        End Try

        Return True

    End Function

    Public Function SearchJOBMAST(ByVal jobid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle) As Integer
        '=====================================================================================
        'NAME           :SearchJOBMAST
        'Parameter      :jobid：起動するジョブＩＤ
        '　　　　　　　　:para：パラメータ
        'Description    :ジョブマスタからジョブを検索する
        'Return         :１：登録有り　０：登録０件　－１：エラー
        'Create         :2004/07/14
        'Update         :
        '=====================================================================================

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.SearchJOBMAST")
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        'Dim oDB As New CASTCommon.MyOracle
        Dim sql As String
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Try

            sql = "SELECT COUNT(*) AS COUNTER FROM JOBMAST "
            sql = sql & " WHERE JOBID_J = '" & jobid.Trim & "' AND PARAMETA_J = '" & para.Trim & "' AND STATUS_J IN('0','1') AND TOUROKU_DATE_J = '" & DateTime.Today.ToString("yyyyMMdd") & "'"

            If orareader.DataReader(sql) = True Then
                If orareader.GetInt64("COUNTER") = 0 Then
                    Return 0
                Else
                    Write("ジョブマスタ検索", "失敗", "ジョブマスタに登録済みです")
                    Return 1
                End If
            Else
                Write("ジョブマスタ検索", "失敗", db.Message)
                Return -1
            End If

        Catch ex As Exception
            'oDB.Rollback()
            Call Write("ジョブマスタ検索", "失敗", ex.Message)
            Return -1
        Finally
            'oDB.Close()
            orareader.Close()

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            Write_Exit1(sw, "ClsBatchLOG.SearchJOBMAST")
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
        End Try

    End Function

    '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
    Public Function InsertJOBMAST(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, _
                                   ByVal status As String, Optional ByVal message As String = " ") As Boolean
        'Public Function InsertJOBMAST(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, ByVal status As String) As Boolean
        '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END
        '=====================================================================================
        'NAME           :InsertJOBMAST
        'Parameter      : jobid   : 起動するジョブＩＤ
        '　　　　　　　 : userid  : ログインユーザ
        '　　　　　　　 : para    : パラメータ
        '　　　　　　　 : db      : オラクルインスタンス
        '　　　　　　　 : status  : 登録するジョブのステータス
        '　　　　　　　 : message : 出力メッセージ（省略可）
        'Description    : ジョブマスタに、登録するステータスを指定しジョブを登録する
        'Return         : True=OK(成功),False=NG（失敗）
        'Create         : 2016/10/27
        'Update         :
        '=====================================================================================

        Dim sql As String
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.InsertJOBMAST")
        Dim dblock As CASTCommon.CDBLock = Nothing

        Try

            ' ジョブ登録ロック
            dblock = New CASTCommon.CDBLock
            If dblock.InsertJOBMAST_Lock(db, mLockWaitTime) = False Then
                Call Write_Err("JOBMAST登録", "失敗", "タイムアウト")
                Return False
            End If

            sql = "INSERT INTO JOBMAST"
            sql &= "("
            sql &= " TUUBAN_J      "
            sql &= ",TOUROKU_DATE_J"
            sql &= ",TOUROKU_TIME_J"
            sql &= ",STA_DATE_J    "
            sql &= ",STA_TIME_J    "
            sql &= ",END_DATE_J    "
            sql &= ",END_TIME_J    "
            sql &= ",JOBID_J       "
            sql &= ",STATUS_J      "
            sql &= ",USERID_J      "
            sql &= ",PARAMETA_J    "
            sql &= ",ERRMSG_J      "
            sql &= ")"

            sql &= " VALUES ('1'"
            sql &= ",TO_CHAR(SYSDATE,'YYYYMMDD')"
            sql &= ",'" & System.DateTime.Now.ToString("HHmmss") & "'"
            sql &= ",'00000000'"
            sql &= ",'000000'"
            sql &= ",'00000000'"
            sql &= ",'000000'"
            sql &= ",'" & jobid & "'"
            sql &= ",'" & status & "'"
            sql &= ",'" & userid & "'"
            sql &= ",'" & para & "'"
            '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
            sql &= ",'" & message & "'"
            'sql &= ",' '"
            '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END
            sql &= ")"

            Dim nRet As Integer = db.ExecuteNonQuery(sql)
            If nRet = 0 Then
                Call Write("JOBMAST登録", "データなし", "")
            ElseIf nRet < 0 Then
                Call Write("JOBMAST登録", "失敗", db.Message)
                Return False
            End If

        Catch ex As Exception
            Call Write("JOBMAST登録", "失敗", "ジョブマスタの登録に失敗しました　" & ex.Message)
            Return False
        Finally
            If Not dblock Is Nothing Then
                ' ジョブ登録アンロック
                dblock.InsertJOBMAST_UnLock(db)
            End If
            Write_Exit1(sw, "ClsBatchLOG.InsertJOBMAST")
        End Try

        Return True

    End Function

    '2017/04/12 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ START
    Public Function InsertJOBMASTbyError(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, Optional ByVal message As String = " ") As Boolean
        '=====================================================================================
        'NAME           :InsertJOBMASTbyError
        'Parameter      : jobid  : 起動するジョブＩＤ
        '　　　　　　　 : userid : ログインユーザ
        '　　　　　　　 : para   : パラメータ
        '　　　　　　　 : db     : オラクルインスタンス
        '　　　　　　　 : status : 登録するジョブのステータス
        'Description    : ジョブマスタに、異常終了(ｽﾃｰﾀｽ=7)としてジョブを登録する
        'Return         : True=OK(成功),False=NG（失敗）
        'Create         : 2017/03/23
        'Update         :
        '=====================================================================================

        Dim sql As String
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.InsertJOBMASTbyError")
        Dim dblock As CASTCommon.CDBLock = Nothing

        Try

            ' ジョブ登録ロック
            dblock = New CASTCommon.CDBLock
            If dblock.InsertJOBMAST_Lock(db, mLockWaitTime) = False Then
                Call Write_Err("JOBMAST登録(byError)", "失敗", "タイムアウト")
                Return False
            End If

            sql = "INSERT INTO JOBMAST"
            sql &= "("
            sql &= " TUUBAN_J      "
            sql &= ",TOUROKU_DATE_J"
            sql &= ",TOUROKU_TIME_J"
            sql &= ",STA_DATE_J    "
            sql &= ",STA_TIME_J    "
            sql &= ",END_DATE_J    "
            sql &= ",END_TIME_J    "
            sql &= ",JOBID_J       "
            sql &= ",STATUS_J      "
            sql &= ",USERID_J      "
            sql &= ",PARAMETA_J    "
            sql &= ",ERRMSG_J      "
            sql &= ")"

            sql &= " VALUES ('1'"
            sql &= ",TO_CHAR(SYSDATE,'YYYYMMDD')"
            sql &= ",'" & System.DateTime.Now.ToString("HHmmss") & "'"
            sql &= ",TO_CHAR(SYSDATE,'YYYYMMDD')"
            sql &= ",'" & System.DateTime.Now.ToString("HHmmss") & "'"
            sql &= ",TO_CHAR(SYSDATE,'YYYYMMDD')"
            sql &= ",'" & System.DateTime.Now.ToString("HHmmss") & "'"
            sql &= ",'" & jobid & "'"
            sql &= ",'7'"
            sql &= ",'" & userid & "'"
            sql &= ",'" & para & "'"
            sql &= ",'" & message & "'"
            sql &= ")"

            Dim nRet As Integer = db.ExecuteNonQuery(sql)
            If nRet = 0 Then
                Call Write("JOBMAST登録(byError)", "データなし", "")
            ElseIf nRet < 0 Then
                Call Write("JOBMAST登録(byError)", "失敗", db.Message)
                Return False
            End If

        Catch ex As Exception
            Call Write("JOBMAST登録(byError)", "失敗", "ジョブマスタの登録に失敗しました　" & ex.Message)
            Return False
        Finally
            If Not dblock Is Nothing Then
                ' ジョブ登録アンロック
                dblock.InsertJOBMAST_UnLock(db)
            End If
            Write_Exit1(sw, "ClsBatchLOG.InsertJOBMASTbyError")
        End Try

        Return True

    End Function
    '2017/04/12 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ END

    Public Function SearchJOBMAST(ByVal jobid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, ByVal status As String) As Integer
        '=====================================================================================
        'NAME           :SearchJOBMAST
        'Parameter      : jobid  : 検索するジョブＩＤ
        '　　　　　　　 : para   : パラメータ
        '　　　　　　　 : db     : オラクルインスタンス
        '　　　　　　　 : status : 検索する追加ステータス
        'Description    : ジョブマスタから、指定したステータスを追加しジョブを検索する
        'Return         : １：登録有り　０：登録０件　－１：エラー
        'Create         : 2016/10/27
        'Update         :
        '=====================================================================================

        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.SearchJOBMAST")

        Dim sql As String
        Dim orareader As New CASTCommon.MyOracleReader(db)

        Try

            Dim JobStatus() As String = Status.Split(","c)
            Dim SearchAddStatus As String = String.Empty
            For i As Integer = 0 To JobStatus.Length - 1 Step 1
                SearchAddStatus &= ",'" & JobStatus(i) & "'"
            Next

            sql = "SELECT COUNT(*) AS COUNTER FROM JOBMAST "
            sql &= " WHERE "
            sql &= "     JOBID_J = '" & jobid.Trim & "'"
            sql &= " AND PARAMETA_J = '" & para.Trim & "'"
            sql &= " AND STATUS_J IN('0','1'" & SearchAddStatus & ")"
            sql &= " AND TOUROKU_DATE_J = '" & DateTime.Today.ToString("yyyyMMdd") & "'"

            If orareader.DataReader(sql) = True Then
                If orareader.GetInt64("COUNTER") = 0 Then
                    Return 0
                Else
                    Write("ジョブマスタ検索", "失敗", "ジョブマスタに登録済みです")
                    Return 1
                End If
            Else
                Write("ジョブマスタ検索", "失敗", db.Message)
                Return -1
            End If

        Catch ex As Exception
            Call Write("ジョブマスタ検索", "失敗", ex.Message)
            Return -1
        Finally
            orareader.Close()
            Write_Exit1(sw, "ClsBatchLOG.SearchJOBMAST")
        End Try

    End Function

'*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***

    '
    ' 機能　 ： エラーログ出力
    ' 引数　 ： func    ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           errinfo ：ログの補足情報域に出力するエラー情報文字列
    ' 備考　 ： ログの結果域には”失敗”が出力される
    '
    Public Sub Write_Err(ByVal func As String, ByVal errinfo As String)

        If errinfo = "失敗" Then
            Write_Err(UserID, mToriCode, FuriDate, func, "失敗", "")
        Else
            Write_Err(UserID, mToriCode, FuriDate, func, "失敗", errinfo)
        End If

    End Sub


    '
    ' 機能　 ： エラーログ出力
    ' 引数　 ： func    ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           result  ：ログの結果域に出力する結果文字列
    '           errinfo ：ログの補足情報域に出力するエラー情報文字列
    '
    Public Sub Write_Err(ByVal func As String, ByVal result As String, ByVal errinfo As String)

        Write_Err(UserID, mToriCode, FuriDate, func, result, errinfo)

    End Sub


    '
    ' 機能　 ： エラーログ出力
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           errinfo  ：ログの補足情報域に出力するエラー情報文字列
    '
    Public Sub Write_Err(ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal errinfo As String)

        Write_Err(UserID, aToriCode, aFuriDate, func, result, errinfo)

    End Sub


    '
    ' 機能　 ： エラーログ出力
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           errinfo  ：ログの補足情報域に出力するエラー情報文字列
    '
    Public Sub Write_Err(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal errinfo As String)

        Write_Err(Thread.CurrentThread.ManagedThreadId, aUserID, aToriCode, aFuriDate, func, result, errinfo)

    End Sub


    '
    ' 機能　 ： 例外エラーログ出力
    ' 引数　 ： func：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           ex  ：ログの補足情報域に出力する例外
    ' 備考　 ： ログの結果域には”失敗”が出力される
    '
    Public Sub Write_Err(ByVal func As String, ByVal ex As Exception)

        Write_Err(UserID, mToriCode, FuriDate, func, "失敗", ex)

    End Sub


    '
    ' 機能　 ： 例外エラーログ出力
    ' 引数　 ： func   ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           result ：ログの結果域に出力する結果文字列
    '           ex     ：ログの補足情報域に出力する例外
    '
    Public Sub Write_Err(ByVal func As String, ByVal result As String, ByVal ex As Exception)

        Write_Err(UserID, mToriCode, FuriDate, func, result, ex)

    End Sub


    '
    ' 機能　 ： 例外エラーログ出力
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           ex       ：ログの補足情報域に出力する例外
    '
    Public Sub Write_Err(ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal ex As Exception)

        Write_Err(UserID, aToriCode, aFuriDate, func, result, ex)

    End Sub


    '
    ' 機能　 ： 例外エラーログ出力
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           ex       ：ログの補足情報域に出力する例外
    '
    Public Sub Write_Err(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal ex As Exception)

        Write_Err(Thread.CurrentThread.ManagedThreadId, aUserID, aToriCode, aFuriDate, func, result, ex.Message & ": " & ex.StackTrace)

    End Sub


    '
    ' 機能　 ： 例外エラーログ出力
    ' 引数　 ： threadID ：スレッドID
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           errinfo  ：ログの補足情報域に出力するエラー情報文字列
    '
    Private Sub Write_Err(ByVal threadID As String, _
                          ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                          ByVal func As String, ByVal result As String, ByVal errinfo As String)

        SyncLock mWriteLock
            Call WriteLog(aUserID, aToriCode, aFuriDate, "[ERR ][" & threadID & "] " & _
                       func, result, errinfo)
            Call WriteLog(aUserID, aToriCode, aFuriDate, "[ERR ][" & threadID & "] " & _
                       func, result, "呼出しスタック: " & Environment.StackTrace)
        End SyncLock

    End Sub


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル１）
    ' 引数　 ： func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter1(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル１）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter1(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL1, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル１）
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter1(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL1, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル２）
    ' 引数　 ： func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter2(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル２）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter2(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL2, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル２）
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter2(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL2, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル３）
    ' 引数　 ： func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter3(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル３）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter3(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL3, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル３）
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter3(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL3, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル４）
    ' 引数　 ： func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter4(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル４）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter4(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL4, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル４）
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Public Function Write_Enter4(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL4, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' 機能　 ： 処理開始ログ出力（ログレベル指定）
    ' 引数　 ： loglevel ：ログレベル
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（パラメタなど）
    ' 戻り値 ： Stopwatchオブジェクト
    '           性能ログを採取しない場合はNothingが返る
    ' 備考　 ： ログの結果域には”開始”が出力される
    '
    Private Function Write_Enter(ByVal loglevel As Integer, _
                                 ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        ' 処理開始ログ出力
        Write_LEVEL(loglevel, aUserID, aToriCode, aFuriDate, func, "開始", msg)

        ' 性能ログ出力時は、Stopwatchオブジェクトを生成して返す
        If loglevel <= mPfmLog Then
            sw = System.Diagnostics.Stopwatch.StartNew()
        End If

        return sw

    End Function


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル１）
    ' 引数　 ： sw  ：Stopwatchオブジェクト
    '           func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit1(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL1, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル１）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit1(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL1, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル１）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit1(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL1, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル２）
    ' 引数　 ： sw  ：Stopwatchオブジェクト
    '           func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit2(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL2, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル２）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit2(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL2, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル２）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit2(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL2, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル３）
    ' 引数　 ： sw  ：Stopwatchオブジェクト
    '           func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit3(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL3, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル３）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit3(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL3, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル３）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit3(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL3, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル４）
    ' 引数　 ： sw  ：Stopwatchオブジェクト
    '           func：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit4(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL4, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル４）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit4(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL4, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル４）
    ' 引数　 ： sw       ：Stopwatchオブジェクト
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '
    Public Sub Write_Exit4(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL4, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' 機能　 ： 処理終了ログ出力（ログレベル指定）
    ' 引数　 ： loglevel ：ログレベル
    '           sw       ：Stopwatchオブジェクト
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報（復帰コードなど）
    ' 備考　 ： ログの結果域には”終了”が出力される
    '           iniファイルで性能ログ出力が指定されている場合は、性能ログも出力される
    '
    Private Sub Write_Exit(ByVal loglevel As Integer, ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Dim sLevel As String = ""

        ' 性能ログ出力
        If loglevel <= mPfmLog Then
            If Not sw Is Nothing Then
                sw.Stop()

                Select Case loglevel
                    Case LOG_LEVEL1
                        sLevel = "[PFM1]"
                    Case LOG_LEVEL2
                        sLevel = "[PFM2]"
                    Case LOG_LEVEL3
                        sLevel = "[PFM3]"
                    Case LOG_LEVEL4
                        sLevel = "[PFM4]"
                    Case Else
                End Select

                SyncLock mWriteLock
                    Call WriteLog(aUserID, aToriCode, aFuriDate, sLevel & "[" & Thread.CurrentThread.ManagedThreadId & "] " & _
                               func, "時間", sw.Elapsed.toString)
                End SyncLock

                sw = Nothing
            End If
        End If

        ' 処理終了ログ出力
        Write_LEVEL(loglevel, aUserID, aToriCode, aFuriDate, func, "終了", msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル１）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL1(ByVal func As String, ByVal msg As String)

        If msg = "成功" Or msg = "失敗" Then
            Write_LEVEL(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル１）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL1(ByVal func As String, ByVal result As String, ByVal msg As String)

        Write_LEVEL(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル１）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL1(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL1, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル１）
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL1(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL1, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル２）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL2(ByVal func As String, ByVal msg As String)

        If msg = "成功" Or msg = "失敗" Then
            Write_LEVEL(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル２）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL2(ByVal func As String, ByVal result As String, ByVal msg As String)

        Write_LEVEL(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル２）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL2(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL2, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル２）
    ' 引数　 ： aUserID：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL2(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL2, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル３）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL3(ByVal func As String, ByVal msg As String)

        If msg = "成功" Or msg = "失敗" Then
            Write_LEVEL(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル３）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL3(ByVal func As String, ByVal result As String, ByVal msg As String)

        Write_LEVEL(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル３）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL3(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL3, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル３）
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL3(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL3, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル４）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL4(ByVal func As String, ByVal msg As String)

        If msg = "成功" Or msg = "失敗" Then
            Write_LEVEL(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル４）
    ' 引数　 ： func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL4(ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル４）
    ' 引数　 ： aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL4(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL4, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル４）
    ' 引数　 ： aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Public Sub Write_LEVEL4(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL4, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' 機能　 ： ログ出力（ログレベル指定）
    ' 引数　 ： loglevel ：ログレベル
    '           aUserID  ：ユーザ名
    '           aToriCode：取引先コード
    '           aFuriDate：振替日
    '           func     ：ログの処理詳細域に出力する処理内容（処理名、クラス.メソッド名など）
    '           result   ：ログの結果域に出力する結果文字列
    '           msg      ：ログの補足情報域に出力する情報
    '
    Private Sub Write_LEVEL(ByVal loglevel As Integer, _
                            ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        If loglevel > mLogLevel Then
            return
        End If

        Dim sLevel As String = ""
        Select Case loglevel
            Case LOG_LEVEL1
                sLevel = "[LEV1]"
            Case LOG_LEVEL2
                sLevel = "[LEV2]"
            Case LOG_LEVEL3
                sLevel = "[LEV3]"
            Case LOG_LEVEL4
                sLevel = "[LEV4]"
            Case Else
        End Select

        SyncLock mWriteLock
            Call WriteLog(aUserID, aToriCode, aFuriDate, sLevel & "[" & Thread.CurrentThread.ManagedThreadId & "] " & _
                       func, result, msg)
        End SyncLock

    End Sub


    '
    ' 機能　 ： ログレベル１が有効かを判定する
    ' 戻り値 ： True - 有効，False - 無効
    '
    Public Function IS_LEVEL1() As Boolean

        return IsLogLevel(LOG_LEVEL1)

    End Function


    '
    ' 機能　 ： ログレベル２が有効かを判定する
    ' 戻り値 ： True - 有効，False - 無効
    '
    Public Function IS_LEVEL2() As Boolean

        return IsLogLevel(LOG_LEVEL2)

    End Function


    '
    ' 機能　 ： ログレベル３が有効かを判定する
    ' 戻り値 ： True - 有効，False - 無効
    '
    Public Function IS_LEVEL3() As Boolean

        return IsLogLevel(LOG_LEVEL3)

    End Function


    '
    ' 機能　 ： ログレベル４が有効かを判定する
    ' 戻り値 ： True - 有効，False - 無効
    '
    Public Function IS_LEVEL4() As Boolean

        return IsLogLevel(LOG_LEVEL4)

    End Function


    '
    ' 機能　 ： SQLログレベルが有効かを判定する
    ' 戻り値 ： True - 有効，False - 無効
    '
    Public Function IS_SQLLOG() As Boolean

        If mSqlLog = 1 Then
            return True
        Else
            return False
        End If

    End Function


    '
    ' 機能　 ： 指定ログレベルが有効かを判定する
    ' 引数　 ： loglevel： ログレベル
    ' 戻り値 ： True - 有効，False - 無効
    '
    Private Function IsLogLevel(ByVal loglevel As Integer) As Boolean

        If (loglevel > mLogLevel) And (loglevel > mPfmLog) Then
            return False
        Else
            return True
        End If

    End Function


    '
    ' 機能　 ： SQLログ出力（DBアクセスクラス用）
    ' 引数　 ： func：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           sql ：SQL文
    ' 備考　 ： UIDMASTのINSERT/UPDATEの場合、SQLログ出力しない
    '
    Public Sub Write_SQL(ByVal func As String, ByVal sql As String)

        If mSqlLog = 1 Then
            ' ユーザパスワードがあるSQLログに出さない
            Dim sqlUpper As String = sql.ToUpper
            If sqlUpper.IndexOf("UIDMAST") >= 0 Then
                If sqlUpper.IndexOf("INSERT") >= 0 OrElse sqlUpper.IndexOf("UPDATE") >= 0 Then
                    Exit Sub
                End If
            End If

            SyncLock mWriteLock
                Call WriteLog(UserID, mToriCode, FuriDate, "[SQL ][" & Thread.CurrentThread.ManagedThreadId & "] " & _
                           func, "", "SQL=" & sql)
            End SyncLock
        End If

    End Sub


    '
    ' 機能　 ： SQLエラーログ出力（DBアクセスクラス用）
    ' 引数　 ： func：ログの処理詳細域に出力する処理内容（クラス.メソッド名など）
    '           sql ：SQL文
    ' 備考　 ： ログの結果域には”失敗”が出力される
    '           UIDMASTのINSERT/UPDATEの場合、SQLログ出力しない
    '
    Public Sub Write_SQL_Err(ByVal func As String, ByVal sql As String)

        ' ユーザパスワードがあるSQLログに出さない
        Dim sqlUpper As String = sql.ToUpper
        If sqlUpper.IndexOf("UIDMAST") >= 0 Then
            If sqlUpper.IndexOf("INSERT") >= 0 OrElse sqlUpper.IndexOf("UPDATE") >= 0 Then
                Exit Sub
            End If
        End If

        SyncLock mWriteLock
            Call WriteLog(UserID, mToriCode, FuriDate, "[ERR ][" & Thread.CurrentThread.ManagedThreadId & "] " & _
                       func, "失敗", "SQL=" & sql)
        End SyncLock

    End Sub

'*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

End Class
