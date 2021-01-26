Imports System
Imports System.Text
Imports System.Data.OracleClient

' オラクル接続クラス
Public Class MyOracle
    ' オラクルConnection
    Private OraConn As OracleConnection
    Private OraTran As OracleTransaction
    'バインド変数クエリ用
    Private OraComm As OracleCommand

    ' ログファイル
    '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
    'Private LOG As New BatchLOG("ClsOracle", "オラクル接続")
    Private LOG As BatchLOG
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean
    Private IS_SQLLOG As Boolean
    Private sHashCode As String = ""

    Public ReadOnly Property BatchLOG() As BatchLOG
        Get
            Return LOG
        End Get
    End Property
    '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

    '　メッセージ
    Public Message As String = ""

    ' コード
    Public Code As Integer

    ' オラクルConnectionプロパティ
    Public Property OracleConnection() As OracleConnection
        Get
            Return OraConn
        End Get
        Set(ByVal Value As OracleConnection)
            OraConn = Value
        End Set
    End Property

    ' オラクルTransactionプロパティ
    Public Property OracleTransaction() As OracleTransaction
        Get
            Return OraTran
        End Get
        Set(ByVal Value As OracleTransaction)
            OraTran = Value
        End Set
    End Property

    'OracleCommandプロパティ
    Public Property OracleCommand() As OracleCommand
        Get
            Return OraComm
        End Get
        Set(ByVal Value As OracleCommand)
            OraComm = Value
        End Set
    End Property

    'Parametersプロパティ
    Public ReadOnly Property Parameters() As OracleParameterCollection
        Get
            Return OraComm.Parameters
        End Get
    End Property

    'CommandTextプロパティ
    Public Property CommandText() As String
        Get
            Return OraComm.CommandText
        End Get
        Set(ByVal Value As String)
            If OraComm Is Nothing Then
                'OraCommが存在しない場合、新規作成
                If OraTran Is Nothing Then
                    OraComm = New OracleCommand(Value, OraConn)
                Else
                    OraComm = New OracleCommand(Value, OraConn, OraTran)
                End If
            Else
                '存在する場合はクエリのみ
                OraComm.CommandText = Value
            End If
        End Set
    End Property

    'OracleCommand.Parameters.Itemプロパティ(Object型の引数でParameters.Itemを設定させるために必要)
    Default Public Property Item(ByVal parameterName As String) As Object
        Get
            Return OraComm.Parameters.Item(parameterName)
        End Get
        Set(ByVal Value As Object)
            If Value Is Nothing OrElse Value Is String.Empty Then
                '値の参照がない、又は空文字列の場合は空白をパラメータにセットする
                OraComm.Parameters.AddWithValue(parameterName, " ")
            Else
                OraComm.Parameters.AddWithValue(parameterName, Value)
            End If
        End Set
    End Property
    '********************************************

    ' 機能　 ： New
    '
    ' 備考　 ： 
    Public Sub New()
        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        LOG = New BatchLOG("ClsOracle", "MyOracle")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = "[" & CStr(Me.GetHashCode) & "]"
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "生成")
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Call Connect()
    End Sub

    ' 機能　 ： New
    '
    ' 備考　 ： 
    Public Sub New(ByVal conn As OracleConnection)
        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        LOG = New BatchLOG("ClsOracle", "MyOracle")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = "[" & CStr(Me.GetHashCode) & "]"
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "生成（OracleConnection指定）")
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        OraConn = conn
    End Sub


    ' 機能　 ： オラクル接続
    '
    ' 備考　 ： 
    Public Sub Connect()
        Try
            If OraConn Is Nothing Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.Connect" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.Connect" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                OraConn = New OracleConnection(DB.CONNECT)

                OraConn.Open()

                OraTran = OraConn.BeginTransaction

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.Connect" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
            End If

        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("OracleConnection", "失敗", ex.Message & "：" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.Connect" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try
    End Sub

    ' 機能　 ： オラクル接続
    '
    ' 備考　 ： 
    Public Sub Connect(ByVal conn As OracleConnection)
        OraConn = conn
        Call Connect()
    End Sub

    ' 機能　 ： オラクル切断
    '
    ' 備考　 ： 
    Public Sub Close()
        If OraConn Is Nothing Then
            Exit Sub
        End If

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL4 = True Then
            sw = LOG.Write_Enter4("ClsOracle.Close" & sHashCode)
            LOG.Write_LEVEL4("ClsOracle.Close" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Call Commit()

        Try
            '2010/01/25 競合が発生しないよう明示的にガベージコレクションを実行
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '===============================================================
            OraConn.Close()
            OraConn.Dispose()

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            If IS_LEVEL4 = True Then
                LOG.Write_Exit4(sw, "ClsOracle.Close" & sHashCode)
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            If IS_LEVEL4 = True Then
                LOG.Write_Err("ClsOracle.Close" & sHashCode, ex)
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            OraConn.Dispose()
        End Try
        'OraConn.Close()
        'OraConn.Dispose()

    End Sub

    ' 機能　 ： トランザクションコミット
    '
    ' 備考　 ： 
    Public Sub Commit()
        Try
            If Not OraTran Is Nothing Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.Commit" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.Commit" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                OraTran.Commit()
                OraTran.Dispose()

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.Commit" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
            End If
            OraTran = Nothing
        Catch ex As Exception
            'Commit失敗時はログ出力する
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("Commit", "失敗", ex.Message & "：" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.Commit" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try
    End Sub

    ' 機能　 ： トランザクションロールバック
    '
    ' 備考　 ： 
    Public Sub Rollback()
        Try
            If Not OraTran Is Nothing Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.Rollback" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.Rollback" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                OraTran.Rollback()
                OraTran.Dispose()

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.Rollback" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
            End If
            OraTran = Nothing
        Catch ex As Exception
            'Rollback失敗時はログ出力する
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("Rollback", "失敗", ex.Message & "：" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.Rollback" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try
    End Sub


    ' 機能　 ： トランザクション開始
    '
    ' 備考　 ： 
    Public Sub BeginTrans()
        Try
            If OraTran Is Nothing Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.BeginTrans" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.BeginTrans" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                OraTran = OraConn.BeginTransaction

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.BeginTrans" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
            End If
        Catch ex As Exception
            'Transaction失敗時はログ出力する
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("BeginTransaction", "失敗", ex.Message & "：" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.BeginTransaction" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try
    End Sub

    ' 機能　 ： ＳＱＬ実行
    '
    ' 備考　 ： 
    Public Function ExecuteNonQuery(ByVal sql As StringBuilder) As Integer
        Return ExecuteNonQuery(sql.ToString)
    End Function

    ' 機能　 ： ＳＱＬ実行
    '
    ' 引数   ： ARG1 - ＳＱＬ
    '           ARG2 - TRUE = ORA-00001: 一意制約のエラーをスルーする
    '
    ' 備考　 ： 
    Public Function ExecuteNonQuery(ByVal sql As String, Optional ByVal code1OK As Boolean = False) As Integer
        Try
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            Dim sw As System.Diagnostics.Stopwatch = Nothing
            If IS_LEVEL3 = True Then
                sw = LOG.Write_Enter3("ClsOracle.ExecuteNonQuery" & sHashCode)
                If IS_LEVEL4 = True Then
                    LOG.Write_LEVEL4("ClsOracle.ExecuteNonQuery" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                End If
            End If
            If IS_SQLLOG = True Then
                LOG.Write_SQL("ClsOracle.ExecuteNonQuery" & sHashCode, sql)
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Code = 0
            Dim comm As OracleCommand
            If OraTran Is Nothing Then
                comm = New OracleCommand(sql, OraConn)
            Else
                comm = New OracleCommand(sql, OraConn, OraTran)
            End If

            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Return comm.ExecuteNonQuery
            Dim rtn As Integer = comm.ExecuteNonQuery
            If IS_LEVEL3 = True Then
                LOG.Write_Exit3(sw, "ClsOracle.ExecuteNonQuery" & sHashCode, "復帰値=" & rtn)
            End If
            Return rtn
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        Catch ex As OracleException
            Code = ex.Code
            If code1OK = True AndAlso ex.Code = 1 Then
                Return 0
            End If
            Message = ex.Message
            Code = ex.Code
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("ExecuteNonQuery", "失敗", "CODE:" & ex.Message & ":" & ex.Code & ":" & ex.StackTrace.ToString)
            'LOG.Write("ExecuteNonQuery", "失敗", sql)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, sql)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            Throw New Exception(Message)

            Return -1
        Catch ex As Exception
            Message = ex.Message
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("ExecuteNonQuery", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG.Write("ExecuteNonQuery", "失敗", sql)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, sql)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            Throw New Exception(Message)

            Return -1
        End Try
    End Function

    'バインド変数クエリ対応ExecuteNonQuery
    Public Function ExecuteNonQuery(Optional ByVal code1OK As Boolean = False) As Integer
        Try
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            Dim sw As System.Diagnostics.Stopwatch = Nothing
            If IS_LEVEL3 = True Then
                sw = LOG.Write_Enter3("ClsOracle.ExecuteNonQuery" & sHashCode)
                If IS_LEVEL4 = True Then
                    LOG.Write_LEVEL4("ClsOracle.ExecuteNonQuery" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                End If
            End If
            If IS_SQLLOG = True Then
                LOG.Write_SQL("ClsOracle.ExecuteNonQuery" & sHashCode, OraComm.CommandText)
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Code = 0

            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Return OraComm.ExecuteNonQuery
            Dim rtn As Integer = OraComm.ExecuteNonQuery
            If IS_LEVEL3 = True Then
                LOG.Write_Exit3(sw, "ClsOracle.ExecuteNonQuery" & sHashCode, "復帰値=" & rtn)
            End If

            Return rtn
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        Catch ex As OracleException
            Code = ex.Code
            If code1OK = True AndAlso ex.Code = 1 Then
                Return 0
            End If
            Message = ex.Message
            Code = ex.Code
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("ExecuteNonQuery", "失敗", "CODE:" & ex.Message & ":" & ex.Code & ":" & ex.StackTrace.ToString)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, OraComm.CommandText)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode,ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            Throw New Exception(Message)

            Return -1
        Catch ex As Exception
            Message = ex.Message
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("ExecuteNonQuery", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, OraComm.CommandText)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode,ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            Throw New Exception(Message)

            Return -1
        Finally
            'クエリ実行後はパラメータをクリアする
            ClearParameters()
        End Try
    End Function

    'パラメータクリア
    Public Sub ClearParameters()
        Try
            If Not OraComm Is Nothing Then
                OraComm.Parameters.Clear()
            End If
        Catch
        End Try
    End Sub

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
    Public Function getOracleDataReader(ByVal sql As String) As OracleDataReader

        Dim oraReader As OracleDataReader

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = LOG.Write_Enter3("ClsOracle.getOracleDataReader" & sHashCode)
            If IS_LEVEL4 = True Then
                LOG.Write_LEVEL4("ClsOracle.getOracleDataReader" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
            End If
        End If
        If IS_SQLLOG = True Then
            LOG.Write_SQL("ClsOracle.getOracleDataReader" & sHashCode, sql)
        End If

        Try
            If OraTran Is Nothing Then
                oraReader = New OracleCommand(sql, OraConn).ExecuteReader
            Else
                oraReader = New OracleCommand(sql, OraConn, OraTran).ExecuteReader
            End If

            Return oraReader

        Catch ex As OracleException
            LOG.Write_SQL_Err("ClsOracle.getOracleDataReader", sql)
            LOG.Write_Err("ClsOracle.getOracleDataReader", ex)

            Throw

        Finally
            If IS_LEVEL3 = True Then
                LOG.Write_Exit3(sw, "ClsOracle.getOracleDataReader" & sHashCode)
        End If
        End Try

    End Function
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

    Protected Overrides Sub Finalize()

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL4 = True Then
            sw = LOG.Write_Enter4("ClsOracle.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Call Rollback()
        Call Close()

        MyBase.Finalize()

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        If IS_LEVEL4 = True Then
            LOG.Write_Exit4(sw, "ClsOracle.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

    End Sub
End Class
