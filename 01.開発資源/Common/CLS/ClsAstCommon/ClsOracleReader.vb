Option Strict On

Imports System
Imports System.Collections
Imports System.Data.OracleClient
Imports System.Diagnostics

' オラクルリーダークラス
Public Class MyOracleReader
    ' オラクルConnection
    Private CurrentConnection As OracleConnection = Nothing
    Private CurrentTransaction As OracleTransaction
    ' オラクルCommand
    Private CurrentCommand As OracleCommand
    ' オラクルReader
    Private CurrentReader As OracleDataReader

    '*** 修正 mitsu 2008/09/10 不要 ***
    'Private CurrentItems As CASTCommon.DBItem
    '**********************************

    ' Code 
    Public Code As Integer = 0
    Public Message As String = ""

    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
    Private LOG As BatchLOG
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean
    Private IS_SQLLOG As Boolean
    Private sHashCode As String = ""
    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

    Private mbEOF As Boolean = True
    Public ReadOnly Property EOF() As Boolean
        Get
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            If IS_LEVEL4 = True Then
                LOG.Write_LEVEL4("ClsOracleReader.EOF" & sHashCode, "EOF=" & mbEOF)
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Return mbEOF
        End Get
    End Property

    Private MainDB As MyOracle = Nothing

    ' New
    Sub New()

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = "[" & CStr(Me.GetHashCode) & "]"
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "生成")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        MainDB = New MyOracle

        CurrentConnection = MainDB.OracleConnection
        CurrentTransaction = MainDB.OracleTransaction

    End Sub

    ' New
    Sub New(ByVal ora As MyOracle)

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        If ora Is Nothing Then
            LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        Else
            LOG = ora.BatchLOG
        End If

        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = "[" & CStr(Me.GetHashCode) & "]"
            If ora Is Nothing Then
                LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "生成（MyOracle指定[Nothing]）")
            Else
                LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "生成（MyOracle指定[" & ora.GetHashCode & "]）")
            End If
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)

            If ora Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "引数のMyOracleがNothingです")
            End If
        End If

        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        CurrentConnection = ora.OracleConnection
        CurrentTransaction = ora.OracleTransaction

    End Sub

    ' New
    Sub New(ByVal conn As OracleConnection)

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = CStr(Me.GetHashCode)
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "生成（OracleConnection指定）")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)

            If conn Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "引数のOracleConnectionがNothingです")
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        CurrentConnection = conn

    End Sub

    ' New
    Sub New(ByVal conn As OracleConnection, ByVal tran As OracleTransaction)

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = CStr(Me.GetHashCode)
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "生成（OracleTransaction指定）")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)

            If conn Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "引数のOracleConnectionがNothingです")
            End If

            If tran Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "引数のOracleTransactionがNothingです")
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        CurrentConnection = conn
        CurrentTransaction = tran

    End Sub

    ' New
    Sub New(ByVal ora As MyOracleReader)

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = CStr(Me.GetHashCode)
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "生成（MyOracleReader指定）")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)

            If ora Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "引数のMyOracleReaderがNothingです")
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        CurrentConnection = ora.CurrentConnection
        CurrentTransaction = ora.CurrentTransaction

    End Sub

    ' 機能　 ： ＳＥＬＥＣＴ　ＳＱＬ発行
    '
    ' 引数　 ： ARG1 - ＳＱＬ
    '
    ' 戻り値 ： True - データあり，False - EOF
    '
    ' 備考　 ： 
    Public Function DataReader(ByVal sql As System.Text.StringBuilder, _
                                Optional ByVal Behavior As System.Data.CommandBehavior = System.Data.CommandBehavior.Default) As Boolean
        Return DataReader(sql.ToString, Behavior)
    End Function

    ' 機能　 ： ＳＥＬＥＣＴ　COMMAND作成のみ
    '
    ' 引数　 ： ARG1 - ＳＱＬ
    '
    ' 戻り値 ： True - データあり，False - EOF
    '
    ' 備考　 ： 
    Public Function DataCommand(ByVal sql As System.Text.StringBuilder) As Boolean
        Dim ret As Boolean = False

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Code = 0
        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（データ無しと例外を区別） ***
        Message = ""
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（データ無しと例外を区別） ***

        Try
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            If IS_LEVEL3 = True Then
                sw = LOG.Write_Enter3("ClsOracleReader.DataCommand" & sHashCode)
                If IS_LEVEL4 = True Then
                    LOG.Write_LEVEL4("ClsOracleReader.DataCommand" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                End If
            End If
            If IS_SQLLOG = True Then
                LOG.Write_SQL("ClsOracleReader.DataCommand" & sHashCode, sql.toString)
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            ' Oracleコマンド作成
            CurrentCommand = New OracleCommand(sql.ToString, CurrentConnection, CurrentTransaction)

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            If IS_LEVEL3 = True Then
                LOG.Write_Exit3(sw, "ClsOracleReader.DataCommand" & sHashCode, "復帰値=True")
            End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Return True
        Catch ex As OracleException
            Code = ex.Code
            Message = ex.Message
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("DataCommand", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " " & sql.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_SQL_Err("ClsOracleReader.DataCommand" & sHashCode, sql.toString)
            LOG.Write_Err("ClsOracleReader.DataCommand" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'Message = ex.Message
            'LOG.Write("DataCommand", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " " & sql.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_SQL_Err("ClsOracleReader.DataCommand" & sHashCode, sql.toString)
            LOG.Write_Err("ClsOracleReader.DataCommand" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try

        Return ret
    End Function

    '*** 修正 mitsu 2008/09/30 未使用のためコメントアウト ***
    '' 機能　 ： ＳＥＬＥＣＴ　ＳＱＬ発行
    ''
    '' 引数　 ： ARG1 - ＳＱＬ
    ''
    '' 戻り値 ： True - データあり，False - EOF
    ''
    '' 備考　 ： 
    'Public Function DataReaderParam(ByVal param() As OracleParameter) As Boolean
    '    Dim ret As Boolean = False

    '    Code = 0
    '    Try
    '        ' Oracleコマンド作成
    '        For i As Integer = 0 To param.Length - 1
    '            CurrentCommand.Parameters.Add(param(i))
    '        Next i

    '        mbEOF = True
    '        CurrentReader = CurrentCommand.ExecuteReader(Data.CommandBehavior.Default)
    '        If CurrentReader.HasRows = True Then
    '            ret = CurrentReader.Read()
    '            mbEOF = Not ret
    '        Else
    '            ret = False
    '        End If
    '    Catch ex As OracleException
    '        Code = ex.Code
    '        Message = ex.Message
    '        Dim LOG As New BatchLOG("MyOracleReader", "ORACLE")
    '        LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
    '        LOG = Nothing
    '        Call PutLogTrace()
    '    Catch ex As Exception
    '        Dim LOG As New BatchLOG("MyOracleReader", "ORACLE")
    '        Message = ex.Message
    '        LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
    '        LOG = Nothing
    '        Call PutLogTrace()
    '    End Try

    '    Return ret
    'End Function
    '********************************************************

    ' 機能　 ： ＳＥＬＥＣＴ　ＳＱＬ発行
    '
    ' 引数　 ： ARG1 - ＳＱＬ
    '
    ' 戻り値 ： True - データあり，False - EOF
    '
    ' 備考　 ： 
    Public Function DataReader(ByVal sql As String, _
                                Optional ByVal Behavior As System.Data.CommandBehavior = System.Data.CommandBehavior.Default) As Boolean
        Return DataReader(sql, CurrentConnection, CurrentTransaction, Behavior)
    End Function


    ' 機能　 ： ＳＥＬＥＣＴ　ＳＱＬ発行
    '
    ' 引数　 ： ARG1 - ＳＱＬ
    '
    ' 戻り値 ： True - データあり，False - EOF
    '
    ' 備考　 ： 
    Public Function DataReader(ByVal sql As String, _
                                ByVal conn As OracleConnection, ByVal tran As OracleTransaction, _
                                Optional ByVal Behavior As System.Data.CommandBehavior = System.Data.CommandBehavior.Default) As Boolean
        Dim ret As Boolean = False

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = LOG.Write_Enter3("ClsOracleReader.DataReader" & sHashCode)
            If IS_LEVEL4 = True Then
                LOG.Write_LEVEL4("ClsOracleReader.DataReader" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
            End If
        End If
        If IS_SQLLOG = True Then
            LOG.Write_SQL("ClsOracleReader.DataReader" & sHashCode, sql)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Code = 0
        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（データ無しと例外を区別） ***
        Message = ""
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（データ無しと例外を区別） ***

        Try
            ' Oracleコマンド作成
            CurrentCommand = New OracleCommand(sql, conn, tran)

            mbEOF = True
            CurrentReader = CurrentCommand.ExecuteReader(Behavior)
            If CurrentReader.HasRows = True Then
                ret = CurrentReader.Read()
                mbEOF = Not ret
            Else
                ret = False
            End If
        Catch ex As OracleException
            'Code = ex.Code
            Message = ex.Message
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " " & sql)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_SQL_Err("ClsOracleReader.DataReader" & sHashCode, sql)
            LOG.Write_Err("ClsOracleReader.DataReader" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'Message = ex.Message
            'LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " " & sql)
            'LOG = Nothing
            'Call PutLogTrace()
            Message = ex.Message
            LOG.Write_SQL_Err("ClsOracleReader.DataReader" & sHashCode, sql)
            LOG.Write_Err("ClsOracleReader.DataReader" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        If IS_LEVEL3 = True Then
            LOG.Write_Exit3(sw, "ClsOracleReader.DataReader" & sHashCode, "復帰値=" & ret)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Return ret
    End Function

    ' Reader クローズ
    Public Sub Close()
        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Try
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            If Not CurrentReader Is Nothing AndAlso CurrentReader.IsClosed = False Then
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing

                If IS_LEVEL4 = True Then
                    sw = LOG.Write_Enter4("ClsOracleReader.Close" & sHashCode)
                    LOG.Write_LEVEL4("ClsOracleReader.Close" & sHashCode, "呼出", "呼出しスタック: " & Environment.StackTrace)
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                CurrentReader.Close()
                CurrentReader.Dispose()

                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                If IS_LEVEL4 = True Then
                    LOG.Write_Exit4(sw, "ClsOracleReader.Close" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***
            End If
            If Not CurrentCommand Is Nothing Then
                CurrentCommand.Dispose()
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Catch ex As Exception
            If IS_LEVEL4 = True Then
                LOG.Write_Err("ClsOracleReader.Close" & sHashCode, ex)
            End If
        End Try
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

    End Sub

    ' リーダー取得
    Public Function Reader() As OracleDataReader
        Return CurrentReader
    End Function

    ' NULL判定
    Public Function IsNull(ByVal column As String) As Boolean
        Code = 0
        Try
            Dim num As Integer = CurrentReader.GetOrdinal(column)
            Return CurrentReader.IsDBNull(num)
        Catch ex As OracleException
            Code = ex.Code
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.IsNull" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return True
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("IsNull", "失敗", ex.Message & ":" & ex.StackTrace & " " & column)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.IsNull" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return True
        End Try
    End Function

    ' 列の値を取得
    Public Function GetItem(ByVal column As String) As String
        Code = 0
        Try
            Dim GetObj As Object = CurrentReader.Item(column)
            '*** 修正 mitsu 2008/09/30 処理高速化 ***
            'If GetObj Is System.DBNull.Value Then
            '    Return ""
            'End If

            'If GetObj Is Nothing Then
            '    Return ""
            'End If

            If GetObj Is System.DBNull.Value OrElse GetObj Is Nothing Then
                Return ""
            End If
            '****************************************

            Return CType(GetObj, String)
        Catch ex As OracleException
            Code = ex.Code
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetItem" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return ""
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetItem", "失敗", ex.Message & ":" & ex.StackTrace & " " & column)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetItem" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return ""
        End Try
    End Function

    ' 列の値を取得
    Public Function GetValue(ByVal column As Integer) As String
        Code = 0
        Try
            If CurrentReader.IsClosed = False Then
                If CurrentReader.IsDBNull(column) Then
                    Return ""
                Else
                    Return CType(CurrentReader.Item(column), String)
                End If
            Else
                Return ""
            End If
        Catch ex As OracleException
            Code = ex.Code
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValue" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return ""
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetValue", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " " & column.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValue" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return ""
        End Try
    End Function

    ' 列の値を取得
    Public Function GetValueInt(ByVal column As Integer) As Int32
        Code = 0
        Try
            If CurrentReader.IsDBNull(column) Then
                Return 0
            Else
                Return CurrentReader.GetInt32(column)
            End If
        Catch ex As OracleException
            Code = ex.Code
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "失敗", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValueInt" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 0
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetValueInt", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " " & column.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValueInt" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 0
        End Try
    End Function

    ' 列の値を取得
    Public Function GetString(ByVal name As String, Optional ByVal trimend As Boolean = True) As String
        Dim GetObj As Object = CurrentReader.Item(name)
        If GetObj Is System.DBNull.Value Then
            Return ""
        End If

        '*** 修正 mitsu 2008/09/30 処理高速化 ***
        'If trimend = False Then
        '    Return CType(GetObj, String)
        'End If
        'Return CType(GetObj, String).TrimEnd
        If trimend = False Then
            Return GetObj.ToString
        End If
        Return GetObj.ToString.TrimEnd
        '****************************************
    End Function

    '*** 修正 mitsu 2008/09/30 未使用のためコメントアウト ***
    'Public Function GetValues() As Object()
    '    Dim Obj() As Object = CType(Array.CreateInstance(GetType(Object), CurrentReader.FieldCount), Object())
    '    Dim ObjCount As Integer = CurrentReader.GetValues(Obj)
    '    For i As Integer = 0 To ObjCount - 1
    '        If Obj(i) Is System.DBNull.Value Then
    '            Obj(i) = ""
    '        End If
    '    Next i

    '    Return Obj
    'End Function
    '********************************************************

    ' 取得
    Public Function GetInt(ByVal name As String) As Integer
        Try
            Dim GetObj As Object = CurrentReader.Item(name)
            If GetObj Is System.DBNull.Value Then
                Return 0
            End If
            Return CType(GetObj, Integer)
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetInt", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " name:" & name)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetInt" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " name:" & name)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 0
        End Try
    End Function

    ' 取得
    Public Function GetInt(ByVal num As Integer) As Integer
        Try
            If CurrentReader.IsDBNull(num) = True Then
                Return 0
            End If
            Return CurrentReader.GetInt32(num)
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetInt", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " num:" & num)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetInt" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " num:" & num)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 0
        End Try
    End Function

    ' 取得
    Public Function GetInt64(ByVal name As String) As Long
        Try
            Dim GetObj As Object = CurrentReader.Item(name)
            If GetObj Is System.DBNull.Value Then
                Return 0
            End If
            Return CType(GetObj, Int64)
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetInt64", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " name:" & name)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetInt64" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " name:" & name)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 0
        End Try
    End Function

    ' 取得
    Public Function GetDate(ByVal num As Integer) As Date
        Try
            If CurrentReader.IsDBNull(num) = True Then
                Return New Date
            End If
            Return CurrentReader.GetDateTime(num)
        Catch ex As Exception
            Return New Date
        End Try
    End Function

    '*** 修正 mitsu 2008/09/30 バイナリデータ取得 ***
    Public Function GetBytes(ByVal name As String) As Byte()
        Try
            Dim GetObj As Object = CurrentReader.Item(name)
            If GetObj Is System.DBNull.Value Then
                Return Nothing
            End If
            Return DirectCast(GetObj, Byte())
        Catch ex As Exception
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetBytes", "失敗", ex.Message & ":" & ex.StackTrace.ToString & " name:" & name)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetBytes" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " name:" & name)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return Nothing
        End Try
    End Function
    '************************************************

    Public Function NextRead() As Boolean

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Try
            If IS_LEVEL4 = True Then
                sw = LOG.Write_Enter4("ClsOracleReader.NextRead" & sHashCode)
            End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            mbEOF = Not CurrentReader.Read()
            Return Not mbEOF

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Finally
            If IS_LEVEL4 = True Then
                LOG.Write_Exit4(sw, "ClsOracleReader.NextRead" & sHashCode)
            End If

        End Try
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

    End Function

'*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
'    Private Sub PutLogTrace()
'        Dim strace As New StackTrace(True)
'        Dim count As Integer
'        Dim Msg As New System.Text.StringBuilder(128)
'
'        ' High up the call stack, there is only one stack frame
'        While count < strace.FrameCount
'            Dim frame As New StackFrame
'            frame = strace.GetFrame(count)
'            Msg.Append(" : ")
'            Msg.Append(frame.GetMethod())
'            Msg.Append(" Line:")
'            Msg.Append(frame.GetFileLineNumber().ToString)
'            count += 1
'        End While
'        '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
'        Dim LOG As New BatchLOG("MyOracleReader")
'        LOG.Write("MyOracleReader", "トレース", Msg.ToString)
'
'    End Sub
'*** End Del 2015/12/01 SO)荒木 for ログ強化 ***

    Protected Overrides Sub Finalize()

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL4 = True Then
            sw = LOG.Write_Enter4("ClsOracleReader.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        MyBase.Finalize()

        '*** Str Upd 2015/12/01 SO)荒木 for 潜在障害（DBリソース解放漏れ） ***
        'If Not MainDB Is Nothing Then
        '    MainDB.Close()
        '    MainDB = Nothing
        'End If
        Close()
        '*** End Upd 2015/12/01 SO)荒木 for 潜在障害（DBリソース解放漏れ） ***

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        If IS_LEVEL4 = True Then
            LOG.Write_Exit4(sw, "ClsOracleReader.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

    End Sub
End Class
