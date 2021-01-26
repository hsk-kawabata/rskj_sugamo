Imports System
Imports System.Text
Imports System.Data.OracleClient

' �I���N���ڑ��N���X
Public Class MyOracle
    ' �I���N��Connection
    Private OraConn As OracleConnection
    Private OraTran As OracleTransaction
    '�o�C���h�ϐ��N�G���p
    Private OraComm As OracleCommand

    ' ���O�t�@�C��
    '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
    'Private LOG As New BatchLOG("ClsOracle", "�I���N���ڑ�")
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
    '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

    '�@���b�Z�[�W
    Public Message As String = ""

    ' �R�[�h
    Public Code As Integer

    ' �I���N��Connection�v���p�e�B
    Public Property OracleConnection() As OracleConnection
        Get
            Return OraConn
        End Get
        Set(ByVal Value As OracleConnection)
            OraConn = Value
        End Set
    End Property

    ' �I���N��Transaction�v���p�e�B
    Public Property OracleTransaction() As OracleTransaction
        Get
            Return OraTran
        End Get
        Set(ByVal Value As OracleTransaction)
            OraTran = Value
        End Set
    End Property

    'OracleCommand�v���p�e�B
    Public Property OracleCommand() As OracleCommand
        Get
            Return OraComm
        End Get
        Set(ByVal Value As OracleCommand)
            OraComm = Value
        End Set
    End Property

    'Parameters�v���p�e�B
    Public ReadOnly Property Parameters() As OracleParameterCollection
        Get
            Return OraComm.Parameters
        End Get
    End Property

    'CommandText�v���p�e�B
    Public Property CommandText() As String
        Get
            Return OraComm.CommandText
        End Get
        Set(ByVal Value As String)
            If OraComm Is Nothing Then
                'OraComm�����݂��Ȃ��ꍇ�A�V�K�쐬
                If OraTran Is Nothing Then
                    OraComm = New OracleCommand(Value, OraConn)
                Else
                    OraComm = New OracleCommand(Value, OraConn, OraTran)
                End If
            Else
                '���݂���ꍇ�̓N�G���̂�
                OraComm.CommandText = Value
            End If
        End Set
    End Property

    'OracleCommand.Parameters.Item�v���p�e�B(Object�^�̈�����Parameters.Item��ݒ肳���邽�߂ɕK�v)
    Default Public Property Item(ByVal parameterName As String) As Object
        Get
            Return OraComm.Parameters.Item(parameterName)
        End Get
        Set(ByVal Value As Object)
            If Value Is Nothing OrElse Value Is String.Empty Then
                '�l�̎Q�Ƃ��Ȃ��A���͋󕶎���̏ꍇ�͋󔒂��p�����[�^�ɃZ�b�g����
                OraComm.Parameters.AddWithValue(parameterName, " ")
            Else
                OraComm.Parameters.AddWithValue(parameterName, Value)
            End If
        End Set
    End Property
    '********************************************

    ' �@�\�@ �F New
    '
    ' ���l�@ �F 
    Public Sub New()
        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        LOG = New BatchLOG("ClsOracle", "MyOracle")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = "[" & CStr(Me.GetHashCode) & "]"
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "����")
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Call Connect()
    End Sub

    ' �@�\�@ �F New
    '
    ' ���l�@ �F 
    Public Sub New(ByVal conn As OracleConnection)
        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        LOG = New BatchLOG("ClsOracle", "MyOracle")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = "[" & CStr(Me.GetHashCode) & "]"
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "�����iOracleConnection�w��j")
            LOG.Write_LEVEL4("ClsOracle" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        OraConn = conn
    End Sub


    ' �@�\�@ �F �I���N���ڑ�
    '
    ' ���l�@ �F 
    Public Sub Connect()
        Try
            If OraConn Is Nothing Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.Connect" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.Connect" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                OraConn = New OracleConnection(DB.CONNECT)

                OraConn.Open()

                OraTran = OraConn.BeginTransaction

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.Connect" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
            End If

        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("OracleConnection", "���s", ex.Message & "�F" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.Connect" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try
    End Sub

    ' �@�\�@ �F �I���N���ڑ�
    '
    ' ���l�@ �F 
    Public Sub Connect(ByVal conn As OracleConnection)
        OraConn = conn
        Call Connect()
    End Sub

    ' �@�\�@ �F �I���N���ؒf
    '
    ' ���l�@ �F 
    Public Sub Close()
        If OraConn Is Nothing Then
            Exit Sub
        End If

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL4 = True Then
            sw = LOG.Write_Enter4("ClsOracle.Close" & sHashCode)
            LOG.Write_LEVEL4("ClsOracle.Close" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Call Commit()

        Try
            '2010/01/25 �������������Ȃ��悤�����I�ɃK�x�[�W�R���N�V���������s
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '===============================================================
            OraConn.Close()
            OraConn.Dispose()

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If IS_LEVEL4 = True Then
                LOG.Write_Exit4(sw, "ClsOracle.Close" & sHashCode)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If IS_LEVEL4 = True Then
                LOG.Write_Err("ClsOracle.Close" & sHashCode, ex)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            OraConn.Dispose()
        End Try
        'OraConn.Close()
        'OraConn.Dispose()

    End Sub

    ' �@�\�@ �F �g�����U�N�V�����R�~�b�g
    '
    ' ���l�@ �F 
    Public Sub Commit()
        Try
            If Not OraTran Is Nothing Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.Commit" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.Commit" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                OraTran.Commit()
                OraTran.Dispose()

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.Commit" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
            End If
            OraTran = Nothing
        Catch ex As Exception
            'Commit���s���̓��O�o�͂���
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("Commit", "���s", ex.Message & "�F" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.Commit" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try
    End Sub

    ' �@�\�@ �F �g�����U�N�V�������[���o�b�N
    '
    ' ���l�@ �F 
    Public Sub Rollback()
        Try
            If Not OraTran Is Nothing Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.Rollback" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.Rollback" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                OraTran.Rollback()
                OraTran.Dispose()

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.Rollback" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
            End If
            OraTran = Nothing
        Catch ex As Exception
            'Rollback���s���̓��O�o�͂���
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("Rollback", "���s", ex.Message & "�F" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.Rollback" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try
    End Sub


    ' �@�\�@ �F �g�����U�N�V�����J�n
    '
    ' ���l�@ �F 
    Public Sub BeginTrans()
        Try
            If OraTran Is Nothing Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL3 = True Then
                    sw = LOG.Write_Enter3("ClsOracle.BeginTrans" & sHashCode)
                    If IS_LEVEL4 = True Then
                        LOG.Write_LEVEL4("ClsOracle.BeginTrans" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                    End If
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                OraTran = OraConn.BeginTransaction

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    LOG.Write_Exit3(sw, "ClsOracle.BeginTrans" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
            End If
        Catch ex As Exception
            'Transaction���s���̓��O�o�͂���
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("BeginTransaction", "���s", ex.Message & "�F" & ex.StackTrace)
            LOG.Write_Err("ClsOracle.BeginTransaction" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try
    End Sub

    ' �@�\�@ �F �r�p�k���s
    '
    ' ���l�@ �F 
    Public Function ExecuteNonQuery(ByVal sql As StringBuilder) As Integer
        Return ExecuteNonQuery(sql.ToString)
    End Function

    ' �@�\�@ �F �r�p�k���s
    '
    ' ����   �F ARG1 - �r�p�k
    '           ARG2 - TRUE = ORA-00001: ��Ӑ���̃G���[���X���[����
    '
    ' ���l�@ �F 
    Public Function ExecuteNonQuery(ByVal sql As String, Optional ByVal code1OK As Boolean = False) As Integer
        Try
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            Dim sw As System.Diagnostics.Stopwatch = Nothing
            If IS_LEVEL3 = True Then
                sw = LOG.Write_Enter3("ClsOracle.ExecuteNonQuery" & sHashCode)
                If IS_LEVEL4 = True Then
                    LOG.Write_LEVEL4("ClsOracle.ExecuteNonQuery" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                End If
            End If
            If IS_SQLLOG = True Then
                LOG.Write_SQL("ClsOracle.ExecuteNonQuery" & sHashCode, sql)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Code = 0
            Dim comm As OracleCommand
            If OraTran Is Nothing Then
                comm = New OracleCommand(sql, OraConn)
            Else
                comm = New OracleCommand(sql, OraConn, OraTran)
            End If

            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Return comm.ExecuteNonQuery
            Dim rtn As Integer = comm.ExecuteNonQuery
            If IS_LEVEL3 = True Then
                LOG.Write_Exit3(sw, "ClsOracle.ExecuteNonQuery" & sHashCode, "���A�l=" & rtn)
            End If
            Return rtn
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        Catch ex As OracleException
            Code = ex.Code
            If code1OK = True AndAlso ex.Code = 1 Then
                Return 0
            End If
            Message = ex.Message
            Code = ex.Code
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("ExecuteNonQuery", "���s", "CODE:" & ex.Message & ":" & ex.Code & ":" & ex.StackTrace.ToString)
            'LOG.Write("ExecuteNonQuery", "���s", sql)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, sql)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            Throw New Exception(Message)

            Return -1
        Catch ex As Exception
            Message = ex.Message
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("ExecuteNonQuery", "���s", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG.Write("ExecuteNonQuery", "���s", sql)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, sql)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            Throw New Exception(Message)

            Return -1
        End Try
    End Function

    '�o�C���h�ϐ��N�G���Ή�ExecuteNonQuery
    Public Function ExecuteNonQuery(Optional ByVal code1OK As Boolean = False) As Integer
        Try
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            Dim sw As System.Diagnostics.Stopwatch = Nothing
            If IS_LEVEL3 = True Then
                sw = LOG.Write_Enter3("ClsOracle.ExecuteNonQuery" & sHashCode)
                If IS_LEVEL4 = True Then
                    LOG.Write_LEVEL4("ClsOracle.ExecuteNonQuery" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                End If
            End If
            If IS_SQLLOG = True Then
                LOG.Write_SQL("ClsOracle.ExecuteNonQuery" & sHashCode, OraComm.CommandText)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Code = 0

            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Return OraComm.ExecuteNonQuery
            Dim rtn As Integer = OraComm.ExecuteNonQuery
            If IS_LEVEL3 = True Then
                LOG.Write_Exit3(sw, "ClsOracle.ExecuteNonQuery" & sHashCode, "���A�l=" & rtn)
            End If

            Return rtn
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        Catch ex As OracleException
            Code = ex.Code
            If code1OK = True AndAlso ex.Code = 1 Then
                Return 0
            End If
            Message = ex.Message
            Code = ex.Code
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("ExecuteNonQuery", "���s", "CODE:" & ex.Message & ":" & ex.Code & ":" & ex.StackTrace.ToString)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, OraComm.CommandText)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode,ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            Throw New Exception(Message)

            Return -1
        Catch ex As Exception
            Message = ex.Message
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("ExecuteNonQuery", "���s", ex.Message & ":" & ex.StackTrace.ToString)
            LOG.Write_SQL_Err("ClsOracle.ExecuteNonQuery" & sHashCode, OraComm.CommandText)
            LOG.Write_Err("ClsOracle.ExecuteNonQuery" & sHashCode,ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            Throw New Exception(Message)

            Return -1
        Finally
            '�N�G�����s��̓p�����[�^���N���A����
            ClearParameters()
        End Try
    End Function

    '�p�����[�^�N���A
    Public Sub ClearParameters()
        Try
            If Not OraComm Is Nothing Then
                OraComm.Parameters.Clear()
            End If
        Catch
        End Try
    End Sub

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
    Public Function getOracleDataReader(ByVal sql As String) As OracleDataReader

        Dim oraReader As OracleDataReader

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = LOG.Write_Enter3("ClsOracle.getOracleDataReader" & sHashCode)
            If IS_LEVEL4 = True Then
                LOG.Write_LEVEL4("ClsOracle.getOracleDataReader" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
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
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

    Protected Overrides Sub Finalize()

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL4 = True Then
            sw = LOG.Write_Enter4("ClsOracle.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Call Rollback()
        Call Close()

        MyBase.Finalize()

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        If IS_LEVEL4 = True Then
            LOG.Write_Exit4(sw, "ClsOracle.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

    End Sub
End Class
