Option Strict On

Imports System
Imports System.Collections
Imports System.Data.OracleClient
Imports System.Diagnostics

' �I���N�����[�_�[�N���X
Public Class MyOracleReader
    ' �I���N��Connection
    Private CurrentConnection As OracleConnection = Nothing
    Private CurrentTransaction As OracleTransaction
    ' �I���N��Command
    Private CurrentCommand As OracleCommand
    ' �I���N��Reader
    Private CurrentReader As OracleDataReader

    '*** �C�� mitsu 2008/09/10 �s�v ***
    'Private CurrentItems As CASTCommon.DBItem
    '**********************************

    ' Code 
    Public Code As Integer = 0
    Public Message As String = ""

    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
    Private LOG As BatchLOG
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean
    Private IS_SQLLOG As Boolean
    Private sHashCode As String = ""
    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

    Private mbEOF As Boolean = True
    Public ReadOnly Property EOF() As Boolean
        Get
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If IS_LEVEL4 = True Then
                LOG.Write_LEVEL4("ClsOracleReader.EOF" & sHashCode, "EOF=" & mbEOF)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Return mbEOF
        End Get
    End Property

    Private MainDB As MyOracle = Nothing

    ' New
    Sub New()

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = "[" & CStr(Me.GetHashCode) & "]"
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "����")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        MainDB = New MyOracle

        CurrentConnection = MainDB.OracleConnection
        CurrentTransaction = MainDB.OracleTransaction

    End Sub

    ' New
    Sub New(ByVal ora As MyOracle)

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
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
                LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�����iMyOracle�w��[Nothing]�j")
            Else
                LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�����iMyOracle�w��[" & ora.GetHashCode & "]�j")
            End If
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)

            If ora Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "������MyOracle��Nothing�ł�")
            End If
        End If

        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        CurrentConnection = ora.OracleConnection
        CurrentTransaction = ora.OracleTransaction

    End Sub

    ' New
    Sub New(ByVal conn As OracleConnection)

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = CStr(Me.GetHashCode)
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�����iOracleConnection�w��j")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)

            If conn Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "������OracleConnection��Nothing�ł�")
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        CurrentConnection = conn

    End Sub

    ' New
    Sub New(ByVal conn As OracleConnection, ByVal tran As OracleTransaction)

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = CStr(Me.GetHashCode)
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�����iOracleTransaction�w��j")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)

            If conn Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "������OracleConnection��Nothing�ł�")
            End If

            If tran Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "������OracleTransaction��Nothing�ł�")
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        CurrentConnection = conn
        CurrentTransaction = tran

    End Sub

    ' New
    Sub New(ByVal ora As MyOracleReader)

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        LOG = New BatchLOG("ClsOracleReader", "MyOracleReader")
        IS_LEVEL3 = LOG.IS_LEVEL3()
        IS_LEVEL4 = LOG.IS_LEVEL4()
        IS_SQLLOG = LOG.IS_SQLLOG()

        If IS_LEVEL4 = True Then
            sHashCode = CStr(Me.GetHashCode)
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�����iMyOracleReader�w��j")
            LOG.Write_LEVEL4("ClsOracleReader" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)

            If ora Is Nothing Then
                LOG.Write_Err("ClsOracleReader.New" & sHashCode, "������MyOracleReader��Nothing�ł�")
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        CurrentConnection = ora.CurrentConnection
        CurrentTransaction = ora.CurrentTransaction

    End Sub

    ' �@�\�@ �F �r�d�k�d�b�s�@�r�p�k���s
    '
    ' �����@ �F ARG1 - �r�p�k
    '
    ' �߂�l �F True - �f�[�^����CFalse - EOF
    '
    ' ���l�@ �F 
    Public Function DataReader(ByVal sql As System.Text.StringBuilder, _
                                Optional ByVal Behavior As System.Data.CommandBehavior = System.Data.CommandBehavior.Default) As Boolean
        Return DataReader(sql.ToString, Behavior)
    End Function

    ' �@�\�@ �F �r�d�k�d�b�s�@COMMAND�쐬�̂�
    '
    ' �����@ �F ARG1 - �r�p�k
    '
    ' �߂�l �F True - �f�[�^����CFalse - EOF
    '
    ' ���l�@ �F 
    Public Function DataCommand(ByVal sql As System.Text.StringBuilder) As Boolean
        Dim ret As Boolean = False

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Code = 0
        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�f�[�^�����Ɨ�O����ʁj ***
        Message = ""
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�f�[�^�����Ɨ�O����ʁj ***

        Try
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If IS_LEVEL3 = True Then
                sw = LOG.Write_Enter3("ClsOracleReader.DataCommand" & sHashCode)
                If IS_LEVEL4 = True Then
                    LOG.Write_LEVEL4("ClsOracleReader.DataCommand" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                End If
            End If
            If IS_SQLLOG = True Then
                LOG.Write_SQL("ClsOracleReader.DataCommand" & sHashCode, sql.toString)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            ' Oracle�R�}���h�쐬
            CurrentCommand = New OracleCommand(sql.ToString, CurrentConnection, CurrentTransaction)

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If IS_LEVEL3 = True Then
                LOG.Write_Exit3(sw, "ClsOracleReader.DataCommand" & sHashCode, "���A�l=True")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Return True
        Catch ex As OracleException
            Code = ex.Code
            Message = ex.Message
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("DataCommand", "���s", ex.Message & ":" & ex.StackTrace.ToString & " " & sql.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_SQL_Err("ClsOracleReader.DataCommand" & sHashCode, sql.toString)
            LOG.Write_Err("ClsOracleReader.DataCommand" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'Message = ex.Message
            'LOG.Write("DataCommand", "���s", ex.Message & ":" & ex.StackTrace.ToString & " " & sql.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_SQL_Err("ClsOracleReader.DataCommand" & sHashCode, sql.toString)
            LOG.Write_Err("ClsOracleReader.DataCommand" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try

        Return ret
    End Function

    '*** �C�� mitsu 2008/09/30 ���g�p�̂��߃R�����g�A�E�g ***
    '' �@�\�@ �F �r�d�k�d�b�s�@�r�p�k���s
    ''
    '' �����@ �F ARG1 - �r�p�k
    ''
    '' �߂�l �F True - �f�[�^����CFalse - EOF
    ''
    '' ���l�@ �F 
    'Public Function DataReaderParam(ByVal param() As OracleParameter) As Boolean
    '    Dim ret As Boolean = False

    '    Code = 0
    '    Try
    '        ' Oracle�R�}���h�쐬
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
    '        LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString)
    '        LOG = Nothing
    '        Call PutLogTrace()
    '    Catch ex As Exception
    '        Dim LOG As New BatchLOG("MyOracleReader", "ORACLE")
    '        Message = ex.Message
    '        LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString)
    '        LOG = Nothing
    '        Call PutLogTrace()
    '    End Try

    '    Return ret
    'End Function
    '********************************************************

    ' �@�\�@ �F �r�d�k�d�b�s�@�r�p�k���s
    '
    ' �����@ �F ARG1 - �r�p�k
    '
    ' �߂�l �F True - �f�[�^����CFalse - EOF
    '
    ' ���l�@ �F 
    Public Function DataReader(ByVal sql As String, _
                                Optional ByVal Behavior As System.Data.CommandBehavior = System.Data.CommandBehavior.Default) As Boolean
        Return DataReader(sql, CurrentConnection, CurrentTransaction, Behavior)
    End Function


    ' �@�\�@ �F �r�d�k�d�b�s�@�r�p�k���s
    '
    ' �����@ �F ARG1 - �r�p�k
    '
    ' �߂�l �F True - �f�[�^����CFalse - EOF
    '
    ' ���l�@ �F 
    Public Function DataReader(ByVal sql As String, _
                                ByVal conn As OracleConnection, ByVal tran As OracleTransaction, _
                                Optional ByVal Behavior As System.Data.CommandBehavior = System.Data.CommandBehavior.Default) As Boolean
        Dim ret As Boolean = False

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = LOG.Write_Enter3("ClsOracleReader.DataReader" & sHashCode)
            If IS_LEVEL4 = True Then
                LOG.Write_LEVEL4("ClsOracleReader.DataReader" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        If IS_SQLLOG = True Then
            LOG.Write_SQL("ClsOracleReader.DataReader" & sHashCode, sql)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Code = 0
        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�f�[�^�����Ɨ�O����ʁj ***
        Message = ""
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�f�[�^�����Ɨ�O����ʁj ***

        Try
            ' Oracle�R�}���h�쐬
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
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString & " " & sql)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_SQL_Err("ClsOracleReader.DataReader" & sHashCode, sql)
            LOG.Write_Err("ClsOracleReader.DataReader" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'Message = ex.Message
            'LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString & " " & sql)
            'LOG = Nothing
            'Call PutLogTrace()
            Message = ex.Message
            LOG.Write_SQL_Err("ClsOracleReader.DataReader" & sHashCode, sql)
            LOG.Write_Err("ClsOracleReader.DataReader" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        If IS_LEVEL3 = True Then
            LOG.Write_Exit3(sw, "ClsOracleReader.DataReader" & sHashCode, "���A�l=" & ret)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Return ret
    End Function

    ' Reader �N���[�Y
    Public Sub Close()
        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Try
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            If Not CurrentReader Is Nothing AndAlso CurrentReader.IsClosed = False Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing

                If IS_LEVEL4 = True Then
                    sw = LOG.Write_Enter4("ClsOracleReader.Close" & sHashCode)
                    LOG.Write_LEVEL4("ClsOracleReader.Close" & sHashCode, "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                CurrentReader.Close()
                CurrentReader.Dispose()

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL4 = True Then
                    LOG.Write_Exit4(sw, "ClsOracleReader.Close" & sHashCode)
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
            End If
            If Not CurrentCommand Is Nothing Then
                CurrentCommand.Dispose()
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Catch ex As Exception
            If IS_LEVEL4 = True Then
                LOG.Write_Err("ClsOracleReader.Close" & sHashCode, ex)
            End If
        End Try
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

    End Sub

    ' ���[�_�[�擾
    Public Function Reader() As OracleDataReader
        Return CurrentReader
    End Function

    ' NULL����
    Public Function IsNull(ByVal column As String) As Boolean
        Code = 0
        Try
            Dim num As Integer = CurrentReader.GetOrdinal(column)
            Return CurrentReader.IsDBNull(num)
        Catch ex As OracleException
            Code = ex.Code
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.IsNull" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return True
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("IsNull", "���s", ex.Message & ":" & ex.StackTrace & " " & column)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.IsNull" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return True
        End Try
    End Function

    ' ��̒l���擾
    Public Function GetItem(ByVal column As String) As String
        Code = 0
        Try
            Dim GetObj As Object = CurrentReader.Item(column)
            '*** �C�� mitsu 2008/09/30 ���������� ***
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
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetItem" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return ""
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetItem", "���s", ex.Message & ":" & ex.StackTrace & " " & column)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetItem" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return ""
        End Try
    End Function

    ' ��̒l���擾
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
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValue" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return ""
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetValue", "���s", ex.Message & ":" & ex.StackTrace.ToString & " " & column.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValue" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return ""
        End Try
    End Function

    ' ��̒l���擾
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
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("ExecuteReader", "���s", ex.Message & ":" & ex.StackTrace.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValueInt" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 0
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetValueInt", "���s", ex.Message & ":" & ex.StackTrace.ToString & " " & column.ToString)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetValueInt" & sHashCode, ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 0
        End Try
    End Function

    ' ��̒l���擾
    Public Function GetString(ByVal name As String, Optional ByVal trimend As Boolean = True) As String
        Dim GetObj As Object = CurrentReader.Item(name)
        If GetObj Is System.DBNull.Value Then
            Return ""
        End If

        '*** �C�� mitsu 2008/09/30 ���������� ***
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

    '*** �C�� mitsu 2008/09/30 ���g�p�̂��߃R�����g�A�E�g ***
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

    ' �擾
    Public Function GetInt(ByVal name As String) As Integer
        Try
            Dim GetObj As Object = CurrentReader.Item(name)
            If GetObj Is System.DBNull.Value Then
                Return 0
            End If
            Return CType(GetObj, Integer)
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetInt", "���s", ex.Message & ":" & ex.StackTrace.ToString & " name:" & name)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetInt" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " name:" & name)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 0
        End Try
    End Function

    ' �擾
    Public Function GetInt(ByVal num As Integer) As Integer
        Try
            If CurrentReader.IsDBNull(num) = True Then
                Return 0
            End If
            Return CurrentReader.GetInt32(num)
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetInt", "���s", ex.Message & ":" & ex.StackTrace.ToString & " num:" & num)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetInt" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " num:" & num)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 0
        End Try
    End Function

    ' �擾
    Public Function GetInt64(ByVal name As String) As Long
        Try
            Dim GetObj As Object = CurrentReader.Item(name)
            If GetObj Is System.DBNull.Value Then
                Return 0
            End If
            Return CType(GetObj, Int64)
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetInt64", "���s", ex.Message & ":" & ex.StackTrace.ToString & " name:" & name)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetInt64" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " name:" & name)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return 0
        End Try
    End Function

    ' �擾
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

    '*** �C�� mitsu 2008/09/30 �o�C�i���f�[�^�擾 ***
    Public Function GetBytes(ByVal name As String) As Byte()
        Try
            Dim GetObj As Object = CurrentReader.Item(name)
            If GetObj Is System.DBNull.Value Then
                Return Nothing
            End If
            Return DirectCast(GetObj, Byte())
        Catch ex As Exception
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim LOG As New BatchLOG("MyOracleReader")
            'LOG.Write("GetBytes", "���s", ex.Message & ":" & ex.StackTrace.ToString & " name:" & name)
            'LOG = Nothing
            'Call PutLogTrace()
            LOG.Write_Err("ClsOracleReader.GetBytes" & sHashCode, ex.Message & ": " & ex.StackTrace.ToString & " name:" & name)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            Return Nothing
        End Try
    End Function
    '************************************************

    Public Function NextRead() As Boolean

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        Try
            If IS_LEVEL4 = True Then
                sw = LOG.Write_Enter4("ClsOracleReader.NextRead" & sHashCode)
            End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            mbEOF = Not CurrentReader.Read()
            Return Not mbEOF

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Finally
            If IS_LEVEL4 = True Then
                LOG.Write_Exit4(sw, "ClsOracleReader.NextRead" & sHashCode)
            End If

        End Try
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

    End Function

'*** Str Del 2015/12/01 SO)�r�� for ���O���� ***
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
'        '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("MyOracleReader", "ORACLE")
'        Dim LOG As New BatchLOG("MyOracleReader")
'        LOG.Write("MyOracleReader", "�g���[�X", Msg.ToString)
'
'    End Sub
'*** End Del 2015/12/01 SO)�r�� for ���O���� ***

    Protected Overrides Sub Finalize()

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL4 = True Then
            sw = LOG.Write_Enter4("ClsOracleReader.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        MyBase.Finalize()

        '*** Str Upd 2015/12/01 SO)�r�� for ���ݏ�Q�iDB���\�[�X����R��j ***
        'If Not MainDB Is Nothing Then
        '    MainDB.Close()
        '    MainDB = Nothing
        'End If
        Close()
        '*** End Upd 2015/12/01 SO)�r�� for ���ݏ�Q�iDB���\�[�X����R��j ***

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        If IS_LEVEL4 = True Then
            LOG.Write_Exit4(sw, "ClsOracleReader.Finalize" & sHashCode)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

    End Sub
End Class
