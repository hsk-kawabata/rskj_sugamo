Option Explicit On 
Option Strict On

Imports System
Imports System.IO
Imports System.Diagnostics

'トレースログ出力用クラス ※シングルトンクラス注意
Public NotInheritable Class TraceLog

#Region "変数"
    Private Shared Singleton As TraceLog = New TraceLog     'シングルトン
    Private Shared FileName As String                       'ログファイル名
    Private Shared HostName As String                       'PC名
    Private Shared PID As Integer                           'プロセスＩＤ
    Private Shared Source As String                         'プロセス名
    Private Shared CRLF As String                           '改行コード
    Private Shared State As FileState                       'ファイル状態
    Private Shared Output As FileStream                     'ファイルストリーム
    Private Shared Writer As StreamWriter                   '書込ストリーム

    'ファイル状態用
    Private Enum FileState
        Close
        Open
        Err
    End Enum
#End Region

#Region "プロパティ"
    Public Shared ReadOnly Property Instance() As TraceLog
        Get
            Return Singleton
        End Get
    End Property
#End Region

#Region "コンストラクタ"
    Private Sub New()
        Try
            '各種情報設定
            Dim p As Process = Process.GetCurrentProcess
            HostName = Environment.MachineName
            PID = p.Id
            Source = p.ProcessName
            CRLF = Environment.NewLine
            State = FileState.Close

            'ディレクトリ名はLOG\TRACE\システム日付8桁
            FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "LOG"), "TRACE\" & DateTime.Today.ToString("yyyyMMdd"))
            If Directory.Exists(FileName) = False Then
                Directory.CreateDirectory(FileName)
            End If
            'ファイル名はシステム時刻6桁.プロセス名.プロセスID4桁.LOG
            FileName = Path.Combine(FileName, DateTime.Now.ToString("HHmmss") & "." & Source & "." & p.Id.ToString("0000") & ".LOG")
        Catch
        End Try
    End Sub
#End Region

#Region "メソッド"
    Private Shared Function FileOpen() As Boolean
        Try
            Select Case State
                Case FileState.Close
                    'ファイルを排他制御無しで開く
                    Output = New FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                    'ストリーム出力はS-JISとする
                    Writer = New StreamWriter(Output, Text.Encoding.GetEncoding(932))
                    Writer.AutoFlush = True
                    State = FileState.Open
                    Return True

                Case FileState.Open
                    Return True

                Case FileState.Err
                    Return False
            End Select
        Catch
            State = FileState.Err
            Return False
        End Try
    End Function

    'イベントログ出力関数(例外)
    Public Shared Sub ELogWrite(ByVal ex As Exception, Optional ByVal type As EventLogEntryType = EventLogEntryType.Information)
        Try
            EventLog.WriteEntry(Source, _
                "処理中にシステムエラーが発生しました。" & CRLF & _
                "タイムスタンプ： " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                "コンピュータ名： " & HostName & CRLF & _
                "プロセスＩＤ： " & PID & CRLF & _
                "プロセス名： " & Source & CRLF & _
                "例外の種類： " & ex.GetType.Name & CRLF & _
                "エラーメッセージ： " & ex.Message & CRLF & _
                ex.StackTrace & CRLF, _
                type)
            'ログファイルにも出力する
            LogWrite(ex)
        Catch ex2 As Exception
            EventLog.WriteEntry(Source, ex2.Message)
        End Try
    End Sub

    'イベントログ出力関数(外部プロセス)
    Public Shared Sub ELogWrite(ByVal ps As Process, Optional ByVal type As EventLogEntryType = EventLogEntryType.Information)
        Try
            EventLog.WriteEntry(Source, _
                "処理中にシステムエラーが発生しました。" & CRLF & _
                "タイムスタンプ： " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                "コンピュータ名： " & HostName & CRLF & _
                "プロセスＩＤ： " & PID & CRLF & _
                "プロセス名： " & Source & CRLF & _
                "被呼出プロセス名： " & ps.StartInfo.FileName & CRLF & _
                "パラメータ： " & ps.StartInfo.Arguments & CRLF & _
                "リターンコード： " & ps.ExitCode & CRLF, _
                type)
            'ログファイルにも出力する
            LogWrite(ps)
        Catch ex As Exception
            EventLog.WriteEntry(Source, ex.Message)
        End Try
    End Sub

    'イベントログ出力関数(メッセージ)
    Public Shared Sub ELogWrite(ByVal message As String, Optional ByVal type As EventLogEntryType = EventLogEntryType.Information)
        Try
            EventLog.WriteEntry(Source, _
                "処理中にシステムエラーが発生しました。" & CRLF & _
                "タイムスタンプ： " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                "コンピュータ名： " & HostName & CRLF & _
                "プロセスＩＤ： " & PID & CRLF & _
                "プロセス名： " & Source & CRLF & _
                "エラーメッセージ： " & message & CRLF, _
                type)
            'ログファイルにも出力する
            LogWrite(message)
        Catch ex As Exception
            EventLog.WriteEntry(Source, ex.Message)
        End Try
    End Sub

    'ログファイル出力関数(例外)
    Public Shared Sub LogWrite(ByVal ex As Exception)
        Try
            If FileOpen() = True Then
                Writer.WriteLine("タイムスタンプ： " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                    "コンピュータ名： " & HostName & CRLF & _
                    "プロセスＩＤ： " & PID & CRLF & _
                    "プロセス名： " & Source & CRLF & _
                    "例外の種類： " & ex.GetType.Name & CRLF & _
                    "エラーメッセージ： " & ex.Message & CRLF & _
                    ex.StackTrace & CRLF)
            End If
        Catch
            'イベントログでも出力する場合、無限ループになるのでコメントアウト
            ''ログファイルに出力出来ない場合はイベントログに出力する
            'ELogWrite(ex)
        End Try
    End Sub

    'ログファイル出力関数(外部プロセス)
    Public Shared Sub LogWrite(ByVal ps As Process)
        Try
            If FileOpen() = True Then
                Writer.WriteLine("タイムスタンプ： " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                    "コンピュータ名： " & HostName & CRLF & _
                    "プロセスＩＤ： " & PID & CRLF & _
                    "プロセス名： " & Source & CRLF & _
                    "被呼出プロセス名： " & ps.StartInfo.FileName & CRLF & _
                    "パラメータ： " & ps.StartInfo.Arguments & CRLF & _
                    "リターンコード： " & ps.ExitCode & CRLF)
            End If
        Catch
            'イベントログでも出力する場合、無限ループになるのでコメントアウト
            ''ログファイルに出力出来ない場合はイベントログに出力する
            'ELogWrite(ps)
        End Try
    End Sub

    'ログファイル出力関数(例外)
    Public Shared Sub LogWrite(ByVal message As String)
        Try
            If FileOpen() = True Then
                Writer.WriteLine("タイムスタンプ： " & DateTime.Now & "." & DateTime.Now.Millisecond & CRLF & _
                    "コンピュータ名： " & HostName & CRLF & _
                    "プロセスＩＤ： " & PID & CRLF & _
                    "プロセス名： " & Source & CRLF & _
                    "エラーメッセージ： " & message & CRLF)
            End If
        Catch
            'イベントログでも出力する場合、無限ループになるのでコメントアウト
            ''ログファイルに出力出来ない場合はイベントログに出力する
            'ELogWrite(message)
        End Try
    End Sub
#End Region

#Region "捕捉されなかった例外用"
    Public Shared Function AddErrHandler() As Boolean
        Try
            Dim currentDomain As AppDomain = AppDomain.CurrentDomain
            AddHandler currentDomain.UnhandledException, AddressOf Singleton.ErrHandler
        Catch
            Return False
        End Try

        Return True
    End Function

    Private Sub ErrHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Try
            Dim ex As Exception = DirectCast(e.ExceptionObject, Exception)
            LogWrite(ex)
            ELogWrite(ex, EventLogEntryType.Error)
        Catch
        End Try
    End Sub
#End Region

#Region "デストラクタ"
    Protected Overrides Sub Finalize()
        Try
            If Not Output Is Nothing Then
                Output.Close()
            End If
            If Not Writer Is Nothing Then
                Writer.Close()
            End If
        Catch
        End Try
        MyBase.Finalize()
    End Sub
#End Region

End Class
