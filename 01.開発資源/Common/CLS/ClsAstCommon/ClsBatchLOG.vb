Imports System
Imports System.IO
Imports System.Diagnostics
Imports System.Data.OracleClient
'*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
Imports System.Threading
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)�r�� for ���O���� ***

' �o�b�`���O �N���X
Public Class BatchLOG

#Region "���O�̏o�͂ɂ���"
    '�@��������(�����b)
    '�A�v���Z�XID(OS�t�^�ԍ�)
    '�B�W���u�ʔ�(�o�b�`�̂�(�W���u�}�X�^�ɓo�^�����ԍ�)
    '�C�����[��(�R���s���[�^��)
    '�D���O�C�����[�U��
    '�E�����R�[�h(XXXXXXXXXX-XX�`��)
    '�F�U�֓�(�U����/������/���)(yyyymmdd�`��)
    '�G�������W���[��(���(���ID),�o�b�`(Exe��))
    '�H������(�V�X�e���@�\�ꗗ(�V�X�e���@�\��(�V�X�e���@�\ID)))
    '�I�������e(�������e+�J�nOR�������e+�I��)
    '�J��������
    '  �E���/�f�[�^�쐬�n�ɂ��Ă͏���������t������(����+(******��))
    '�K���l(�o�b�`�Ȃ�p�����[�^�Ȃ�?,���s�Ȃ�G���[���e)

    '���ʔԂ͔p�~���܂�
#End Region

    ' ���J�ϐ�
    Public PID As Integer                 ' �v���Z�XID(OS�t�^�ԍ�)
    '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
    'Private mTuuban As Integer           ' �W���u�ʔ�(�o�b�`�̂�(�W���u�}�X�^�ɓo�^�����ԍ�)
    Private Shared mTuuban As Integer = 0 ' �W���u�ʔ�(�o�b�`�̂�(�W���u�}�X�^�ɓo�^�����ԍ�)
    '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
    Public HostName As String             ' �����[��(�R���s���[�^��)
    Public UserID As String               ' ���O�C�����[�U��
    Private mToriCode As String           ' �����R�[�h
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

    Public FuriDate As String             ' �U�֓�(�U����/������/���)
    Public ModuleName As String           ' �������W���[��(���(���ID),�o�b�`(Exe��))
    Public SyoriName As String            ' ������(�V�X�e���@�\�ꗗ(�V�X�e���@�\��(�V�X�e���@�\ID)))

    ' ���O�p�X
    Private ReadOnly LOGPATH As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "LOG"), DateTime.Today.ToString("yyyyMMdd"))
    ' ���O�t�@�C����
    '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
    'Private LogName As String
    Private Shared LogName As String = ""
    '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

    Private LogNamePre As String = ""

    Private ReadOnly kai As String = System.Text.Encoding.ASCII.GetString(New Byte() {13, 10})

    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
    ' ���O���x���萔
    Private ReadOnly LOG_LEVEL1 As Integer = 1    ' �Ɩ����W�b�N���O
    Private ReadOnly LOG_LEVEL2 As Integer = 2    ' �Ɩ����W�b�N�ڍ׃��O
    Private ReadOnly LOG_LEVEL3 As Integer = 3    ' ���ʃ��W���[�����O
    Private ReadOnly LOG_LEVEL4 As Integer = 4    ' ���ʃ��W���[���ڍ׃��O�A�f�o�b�O���O���x��

    ' ini�t�@�C���Ŏw�肳�ꂽ���O���x��
    Private mLogLevel As Integer = 1      ' �g���[�X���O���x��
    Private mPfmLog As Integer = 1        ' ���\���O���x��
    Private mSqlLog As Integer = 0        ' SQL���O���x��

    Private mRetryCnt As Integer = 10     ' ���O�r���҂����g���C��
    Private mWaitTime As Integer = 10     ' ���O�r���҂����ԁi�~���b�j

    ' �}���`�X���b�h����̓������O�o�͂̔r���p
    Private Shared mWriteLock As New Object 
    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
    Private mLockWaitTime As Integer = 30
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***


    ' �ʒm���b�Z�[�W
    Public JobMessage As String = ""

    '�W���u�}�X�^�p
    Public Enum JobStatus
        NormalEnd = 2                       ' ����I��
        AbnormalEnd = 3                     ' �ُ�I��
    End Enum

    ' �ʔ�
    Public Property JobTuuban() As Integer
        Get
            Return mTuuban
        End Get
        Set(ByVal Value As Integer)
            mTuuban = Value

            If mTuuban > 0 Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                ' ���O�t�@�C����
                LogName = Path.Combine(LOGPATH, LogNamePre & "." & mTuuban.ToString("000000") & ".LOG")

                Dim sw As System.Diagnostics.Stopwatch
                sw = Write_Enter3("ClsBatchLOG.JobTuuban")
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                ' �ʔԂ���JOB�}�X�^�̃��[�UID���擾����
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
                        Throw New Exception("JOBMAST��������܂��� TUUBAN_J = " & mTuuban)
                        UserID = ""
                    End If
                    SQLReader.Close()
                Catch ex As Exception
                    Throw New Exception("JOBMAST��������܂��� TUUBAN_J = " & mTuuban & " " & ex.Message)
                Finally
                    OraDB.Close()
                End Try

                OraDB = Nothing

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                Write_Exit3(sw, "ClsBatchLOG.JobTuuban")
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                '*** Str Del 2015/12/01 SO)�r�� for ���O���� ***
                '' ���O�t�@�C����
                'LogName = Path.Combine(LOGPATH, LogNamePre & "." & mTuuban.ToString("000000") & ".LOG")
                '*** End Del 2015/12/01 SO)�r�� for ���O���� ***

            Else
                UserID = ""

                ' ���O�t�@�C����
                LogName = Path.Combine(LOGPATH, LogNamePre & ".000000.LOG")
            End If
        End Set
    End Property

    ' New
    Public Sub New(Optional ByVal ModuleName As String = "", Optional ByVal SyoriName As String = "")
        LogNamePre = Process.GetCurrentProcess.ProcessName

        ' ���O�t�@�C����
        '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
        'LogName = Path.Combine(LOGPATH, LogNamePre & ".000000.LOG")
        If LogName = "" Then
            LogName = Path.Combine(LOGPATH, LogNamePre & ".000000.LOG")
        End If
        '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        PID = Process.GetCurrentProcess.Id
        '*** Str Del 2015/12/01 SO)�r�� for ���O���� ***
        'mTuuban = 0
        '*** End Del 2015/12/01 SO)�r�� for ���O���� ***
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

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sWork As String
        ' SQL���O�o�̓`�F�b�N
        If LogNamePre.ToUpper.StartsWith("JOBKANSHI") Then
            sWork = CASTCommon.GetFSKJIni("COMMON", "JOBKANSHI_SQLLOG")
        Else
            sWork = CASTCommon.GetFSKJIni("COMMON", "SQLLOG")
        End If
        sWork = sWork.Trim
        If sWork.ToUpper = "YES" Then
            mSqlLog = 1
        End If

        ' ���\���O�o�̓`�F�b�N
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

        ' ���O���x���`�F�b�N
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

        ' ���O�r���҂����g���C��
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOG_RETRY_COUNT")
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mRetryCnt = CInt(sWork)
            If mRetryCnt < 1 OrElse mRetryCnt > 100 Then
                mRetryCnt = 10
            End If
        End If

        ' ���O�r���҂����ԁi�~���b�j
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOG_WAIT_TIME")
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mWaitTime = CInt(sWork)
            If mWaitTime < 1 OrElse mWaitTime > 1000 Then
                mWaitTime = 10
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            mLockWaitTime = CInt(sWork)
            If mLockWaitTime <= 0 Then
                mLockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***


        '*** Str Del 2015/12/01 SO)�r�� for ���O�����i�璷�ȃ��O�폜�j ***
        'Write("�J�n", "����")
        '*** End Del 2015/12/01 SO)�r�� for ���O�����i�璷�ȃ��O�폜�j ***

    End Sub

    Public Sub New(ByVal ModuleName As String, ByVal SyoriName As String, ByVal LogFileName As String)

        If LogFileName = "" Then
            LogNamePre = Process.GetCurrentProcess.ProcessName
        Else
            LogNamePre = LogFileName
        End If

        ' ���O�t�@�C����
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

        ' ���\���O�o�̓`�F�b�N
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

        ' ���O���x���`�F�b�N
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

        ' ���O�r���҂����g���C��
        sWork = CASTCommon.GetFSKJIni("COMMON", "LOG_RETRY_COUNT")
        sWork = sWork.Trim
        If IsNumeric(sWork) Then
            mRetryCnt = CInt(sWork)
            If mRetryCnt < 1 OrElse mRetryCnt > 100 Then
                mRetryCnt = 10
            End If
        End If

        ' ���O�r���҂����ԁi�~���b�j
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
    ' �@�\�@ �F ���O�o��
    '
    ' �����@ �F Detail�F�������e�^Result�F���ʁ^Message�F���l
    '
    ' ���l�@ �F 
    '
    Public Sub Write(ByVal Detail As String, ByVal Result As String, Optional ByVal Message As String = "")
        Call Write(UserID, mToriCode, FuriDate, Detail, Result, Message)
    End Sub

    '
    ' �@�\�@ �F ���O�o��
    '
    ' �����@ �F Detail�F�������e�^Result�F���ʁ^Message�F���l
    '
    ' ���l�@ �F 
    '
    Public Sub Write(ByVal ToriCode As String, ByVal FuriDate As String, _
                     ByVal Detail As String, ByVal Result As String, Optional ByVal Message As String = "")
        Call Write(UserID, ToriCode, FuriDate, Detail, Result, Message)
    End Sub



    '
    ' �@�\�@ �F ���O�o��
    '
    ' �����@ �F aUserID�F���[�U���^aToriCode�F�����R�[�h�^aFuriDate�F�U�֓�
    '           �^Detail�F�������e�^Result�F���ʁ^Message�F���l
    '
    ' ���l�@ �F 
    '
    Public Sub Write(ByVal UserID As String, ByVal ToriCode As String, _
                        ByVal FuriDate As String, _
                        ByVal Detail As String, ByVal Result As String, _
                        ByVal Message As String, Optional ByVal Tuuban As Integer = -1)

'*** Str Add 2016/01/20 SO)�r�� for ���O���� ***

        If Not Tuuban = -1 Then
            JobTuuban = Tuuban
        End If

        If Result = "���s" Then
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
'*** End Add 2016/01/20 SO)�r�� for ���O���� ***

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim oCSV As CSVBase = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            ' �����ۑ�
            Me.UserID = UserID
            Me.ToriCode = ToriCode
            Me.FuriDate = FuriDate
            If Message Is Nothing Then
                Message = ""
            End If

            If Not Tuuban = -1 Then
                JobTuuban = Tuuban
            End If

            ' �f�B���N�g���`�F�b�N
            If File.Exists(LOGPATH) = False Then
                Directory.CreateDirectory(LOGPATH)
            End If

            ' ���O�o��
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Dim oCSV As New CSVBase(LogName)
            'Dim nRet As Integer = oCSV.Open(FileMode.Append)
            'If nRet = 0 Then
            '    ' �P�񂾂����g���C����
            '    nRet = oCSV.Open(FileMode.Append)
            'End If
            ' �r���I�[�v���ɕύX�i���g���C��OpenLock���\�b�h���ōs���j
            oCSV = New CSVBase(LogName)
            Dim nRet As Integer = oCSV.OpenLock(FileMode.Append, mRetryCnt, mWaitTime)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            oCSV.Output(DateTime.Now.ToString("HHmmss"), True)
            oCSV.Output(PID, True)
            If JobTuuban > 0 Then
                oCSV.Output(JobTuuban, True)
            Else
                oCSV.Output("", True)
            End If
            Dim WorkMessage As String = Message.Replace(Environment.NewLine, "��")
            WorkMessage = WorkMessage.Replace(kai.Substring(0, 1), "->")
            WorkMessage = WorkMessage.Replace(kai.Substring(1), "->")
            oCSV.OutputPlus(HostName, UserID, mToriCode, FuriDate, ModuleName, SyoriName, Detail, Result, WorkMessage)

            oCSV.Close()

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for �N���[�Y�Y����C���i���݁j ***
            If Not oCSV Is Nothing Then
                oCSV.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for �N���[�Y�Y����C���i���݁j ***

        End Try
    End Sub

    '
    ' �@�\�@ �F �W���u�}�X�^�X�V
    '
    ' �����@ �F ARG1 - �ʔԃ^�X
    '           ARG2 - �X�e�[�^�X
    '           ARG2 - ���b�Z�[�W
    '
    ' �߂�l �F True - �����CFalse - ���s
    '
    ' ���l�@ �F 
    '
    Public Function UpdateJOBMASTbyOK(ByVal message As String) As Boolean
        Return UpdateJOBMAST(JobStatus.NormalEnd, message)
    End Function

    '
    ' �@�\�@ �F �W���u�}�X�^�X�V
    '
    ' �����@ �F ARG1 - �ʔԃ^�X
    '           ARG2 - �X�e�[�^�X
    '           ARG2 - ���b�Z�[�W
    '
    ' �߂�l �F True - �����CFalse - ���s
    '
    ' ���l�@ �F 
    '
    Public Function UpdateJOBMASTbyErr(ByVal Message As String) As Boolean
        ' 2008.04.22 ADD �C�x���g���O
        Try
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write(Message, Diagnostics.EventLogEntryType.Error)
        Catch ex As Exception

        End Try

        JobMessage = Message
        Return UpdateJOBMAST(JobStatus.AbnormalEnd, Message)
    End Function

    '
    ' �@�\�@ �F �W���u�}�X�^�X�V
    '
    ' �����@ �F ARG1 - �ʔԃ^�X
    '           ARG2 - �X�e�[�^�X
    '           ARG2 - ���b�Z�[�W
    '
    ' �߂�l �F True - �����CFalse - ���s
    '
    ' ���l�@ �F 
    '
    Public Function UpdateJOBMASTbyErr(ByVal Message As String, ByVal Level As System.Diagnostics.EventLogEntryType) As Boolean
        ' 2008.04.22 ADD �C�x���g���O
        Try
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write(Message, Level)
        Catch ex As Exception

        End Try

        JobMessage = Message
        Return UpdateJOBMAST(JobStatus.AbnormalEnd, Message)
    End Function

    '
    ' �@�\�@ �F �W���u�}�X�^�X�V
    '
    ' �����@ �F ARG1 - �ʔԃ^�X
    '           ARG2 - �X�e�[�^�X
    '           ARG2 - ���b�Z�[�W
    '
    ' �߂�l �F True - �����CFalse - ���s
    '
    ' ���l�@ �F 
    '
    Public Function UpdateJOBMAST(ByVal Status As JobStatus, ByVal Message As String) As Boolean

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Dim oDB As New CASTCommon.MyOracle

        Dim SQL As String
        Try
            ' �W���u�}�X�^�X�V
            SQL = "UPDATE JOBMAST SET " _
                & " END_DATE_J ='" & DateTime.Now.ToString("yyyyMMdd") & "'" _
                & ",END_TIME_J ='" & DateTime.Now.ToString("HHmmss") & "'" _
                & ",STATUS_J ='" & Status & "'" _
                & ",ERRMSG_J ='" & Cutting(Message & New String(" "c, 200), 200) & "'" _
                & " WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE, 'YYYYMMDD')" _
                & "   AND TUUBAN_J = " & JobTuuban.ToString

            Dim nRet As Integer = oDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                Call Write("JOBMAST�X�V", "�f�[�^�Ȃ�", Message)
            ElseIf nRet < 0 Then
                Call Write("JOBMAST�X�V", "���s", oDB.Message)
                Return False
            End If
            oDB.Commit()
        Catch ex As Exception
            oDB.Rollback()
            Call Write("JOBMAST�X�V", "���s", Message & " " & ex.Message)
            Return False
        Finally
            oDB.Close()
        End Try

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Write_Exit1(sw, "ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Return True
    End Function

    '
    ' �@�\�@ �F �W���u�}�X�^�X�V
    '
    ' �����@ �F ARG1 - ���b�Z�[�W
    '
    ' �߂�l �F True - �����CFalse - ���s
    '
    ' ���l�@ �F 
    '
    Public Function UpdateJOBMAST(ByVal Message As String) As Boolean

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Dim oDB As New CASTCommon.MyOracle

        Dim SQL As String
        Try
            ' �W���u�}�X�^�X�V
            SQL = "UPDATE JOBMAST SET " _
                & "ERRMSG_J ='" & Cutting(Message & New String(" "c, 200), 200) & "'" _
                & " WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE, 'YYYYMMDD')" _
                & "   AND TUUBAN_J = " & JobTuuban.ToString

            Dim nRet As Integer = oDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                Call Write("JOBMAST�X�V", "�f�[�^�Ȃ�", Message)
            ElseIf nRet < 0 Then
                Call Write("JOBMAST�X�V", "���s", oDB.Message)
                Return False
            End If
            oDB.Commit()
        Catch ex As Exception
            oDB.Rollback()
            Call Write("JOBMAST�X�V", "���s", Message & " " & ex.Message)
            Return False
        Finally
            oDB.Close()
        End Try

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Write_Exit1(sw, "ClsBatchLOG.UpdateJOBMAST")
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Return True
    End Function

    Public Function InsertJOBMAST(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle) As Boolean
        '=====================================================================================
        'NAME           :InsertJOBMAST
        'Parameter      :jobid�F�N������W���u�h�c�^userid�F���O�C�����[�U
        '�@�@�@�@�@�@�@�@:para�F�p�����[�^
        'Description    :�W���u�}�X�^�ɃW���u��o�^����
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2004/07/14
        'Update         :
        '=====================================================================================

        'Dim oDB As New CASTCommon.MyOracle
        Dim sql As String

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.InsertJOBMAST")
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
        Dim dblock As CASTCommon.CDBLock = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

        Try

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
            ' �W���u�o�^���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.InsertJOBMAST_Lock(db, mLockWaitTime) = False Then
                Call Write_Err("JOBMAST�o�^", "���s", "�^�C���A�E�g")
                Return False
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

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
                Call Write("JOBMAST�o�^", "�f�[�^�Ȃ�", "")
            ElseIf nRet < 0 Then
                Call Write("JOBMAST�o�^", "���s", db.Message)
                Return False
            End If
            'oDB.Commit()

        Catch ex As Exception
            'oDB.Rollback()
            Call Write("JOBMAST�o�^", "���s", "�W���u�}�X�^�̓o�^�Ɏ��s���܂����@" & ex.Message)
            Return False
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
            If Not dblock Is Nothing Then
                ' �W���u�o�^�A�����b�N
                dblock.InsertJOBMAST_UnLock(db)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
            'oDB.Close()

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            Write_Exit1(sw, "ClsBatchLOG.InsertJOBMAST")
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
        End Try

        Return True

    End Function

    Public Function SearchJOBMAST(ByVal jobid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle) As Integer
        '=====================================================================================
        'NAME           :SearchJOBMAST
        'Parameter      :jobid�F�N������W���u�h�c
        '�@�@�@�@�@�@�@�@:para�F�p�����[�^
        'Description    :�W���u�}�X�^����W���u����������
        'Return         :�P�F�o�^�L��@�O�F�o�^�O���@�|�P�F�G���[
        'Create         :2004/07/14
        'Update         :
        '=====================================================================================

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.SearchJOBMAST")
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

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
                    Write("�W���u�}�X�^����", "���s", "�W���u�}�X�^�ɓo�^�ς݂ł�")
                    Return 1
                End If
            Else
                Write("�W���u�}�X�^����", "���s", db.Message)
                Return -1
            End If

        Catch ex As Exception
            'oDB.Rollback()
            Call Write("�W���u�}�X�^����", "���s", ex.Message)
            Return -1
        Finally
            'oDB.Close()
            orareader.Close()

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            Write_Exit1(sw, "ClsBatchLOG.SearchJOBMAST")
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
        End Try

    End Function

    '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ START
    Public Function InsertJOBMAST(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, _
                                   ByVal status As String, Optional ByVal message As String = " ") As Boolean
        'Public Function InsertJOBMAST(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, ByVal status As String) As Boolean
        '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ END
        '=====================================================================================
        'NAME           :InsertJOBMAST
        'Parameter      : jobid   : �N������W���u�h�c
        '�@�@�@�@�@�@�@ : userid  : ���O�C�����[�U
        '�@�@�@�@�@�@�@ : para    : �p�����[�^
        '�@�@�@�@�@�@�@ : db      : �I���N���C���X�^���X
        '�@�@�@�@�@�@�@ : status  : �o�^����W���u�̃X�e�[�^�X
        '�@�@�@�@�@�@�@ : message : �o�̓��b�Z�[�W�i�ȗ��j
        'Description    : �W���u�}�X�^�ɁA�o�^����X�e�[�^�X���w�肵�W���u��o�^����
        'Return         : True=OK(����),False=NG�i���s�j
        'Create         : 2016/10/27
        'Update         :
        '=====================================================================================

        Dim sql As String
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.InsertJOBMAST")
        Dim dblock As CASTCommon.CDBLock = Nothing

        Try

            ' �W���u�o�^���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.InsertJOBMAST_Lock(db, mLockWaitTime) = False Then
                Call Write_Err("JOBMAST�o�^", "���s", "�^�C���A�E�g")
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
            '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ START
            sql &= ",'" & message & "'"
            'sql &= ",' '"
            '2017/04/12 �^�X�N�j���� CHG �W���ŏC���i�ѓc�M�������f�j------------------------------------ END
            sql &= ")"

            Dim nRet As Integer = db.ExecuteNonQuery(sql)
            If nRet = 0 Then
                Call Write("JOBMAST�o�^", "�f�[�^�Ȃ�", "")
            ElseIf nRet < 0 Then
                Call Write("JOBMAST�o�^", "���s", db.Message)
                Return False
            End If

        Catch ex As Exception
            Call Write("JOBMAST�o�^", "���s", "�W���u�}�X�^�̓o�^�Ɏ��s���܂����@" & ex.Message)
            Return False
        Finally
            If Not dblock Is Nothing Then
                ' �W���u�o�^�A�����b�N
                dblock.InsertJOBMAST_UnLock(db)
            End If
            Write_Exit1(sw, "ClsBatchLOG.InsertJOBMAST")
        End Try

        Return True

    End Function

    '2017/04/12 �^�X�N�j���� ADD �W���ŏC���i�ѓc�M�������f�j------------------------------------ START
    Public Function InsertJOBMASTbyError(ByVal jobid As String, ByVal userid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, Optional ByVal message As String = " ") As Boolean
        '=====================================================================================
        'NAME           :InsertJOBMASTbyError
        'Parameter      : jobid  : �N������W���u�h�c
        '�@�@�@�@�@�@�@ : userid : ���O�C�����[�U
        '�@�@�@�@�@�@�@ : para   : �p�����[�^
        '�@�@�@�@�@�@�@ : db     : �I���N���C���X�^���X
        '�@�@�@�@�@�@�@ : status : �o�^����W���u�̃X�e�[�^�X
        'Description    : �W���u�}�X�^�ɁA�ُ�I��(�ð��=7)�Ƃ��ăW���u��o�^����
        'Return         : True=OK(����),False=NG�i���s�j
        'Create         : 2017/03/23
        'Update         :
        '=====================================================================================

        Dim sql As String
        Dim sw As System.Diagnostics.Stopwatch
        sw = Write_Enter1("ClsBatchLOG.InsertJOBMASTbyError")
        Dim dblock As CASTCommon.CDBLock = Nothing

        Try

            ' �W���u�o�^���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.InsertJOBMAST_Lock(db, mLockWaitTime) = False Then
                Call Write_Err("JOBMAST�o�^(byError)", "���s", "�^�C���A�E�g")
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
                Call Write("JOBMAST�o�^(byError)", "�f�[�^�Ȃ�", "")
            ElseIf nRet < 0 Then
                Call Write("JOBMAST�o�^(byError)", "���s", db.Message)
                Return False
            End If

        Catch ex As Exception
            Call Write("JOBMAST�o�^(byError)", "���s", "�W���u�}�X�^�̓o�^�Ɏ��s���܂����@" & ex.Message)
            Return False
        Finally
            If Not dblock Is Nothing Then
                ' �W���u�o�^�A�����b�N
                dblock.InsertJOBMAST_UnLock(db)
            End If
            Write_Exit1(sw, "ClsBatchLOG.InsertJOBMASTbyError")
        End Try

        Return True

    End Function
    '2017/04/12 �^�X�N�j���� ADD �W���ŏC���i�ѓc�M�������f�j------------------------------------ END

    Public Function SearchJOBMAST(ByVal jobid As String, ByVal para As String, ByVal db As CASTCommon.MyOracle, ByVal status As String) As Integer
        '=====================================================================================
        'NAME           :SearchJOBMAST
        'Parameter      : jobid  : ��������W���u�h�c
        '�@�@�@�@�@�@�@ : para   : �p�����[�^
        '�@�@�@�@�@�@�@ : db     : �I���N���C���X�^���X
        '�@�@�@�@�@�@�@ : status : ��������ǉ��X�e�[�^�X
        'Description    : �W���u�}�X�^����A�w�肵���X�e�[�^�X��ǉ����W���u����������
        'Return         : �P�F�o�^�L��@�O�F�o�^�O���@�|�P�F�G���[
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
                    Write("�W���u�}�X�^����", "���s", "�W���u�}�X�^�ɓo�^�ς݂ł�")
                    Return 1
                End If
            Else
                Write("�W���u�}�X�^����", "���s", db.Message)
                Return -1
            End If

        Catch ex As Exception
            Call Write("�W���u�}�X�^����", "���s", ex.Message)
            Return -1
        Finally
            orareader.Close()
            Write_Exit1(sw, "ClsBatchLOG.SearchJOBMAST")
        End Try

    End Function

'*** Str Add 2015/12/01 SO)�r�� for ���O���� ***

    '
    ' �@�\�@ �F �G���[���O�o��
    ' �����@ �F func    �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           errinfo �F���O�̕⑫����ɏo�͂���G���[��񕶎���
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h���s�h���o�͂����
    '
    Public Sub Write_Err(ByVal func As String, ByVal errinfo As String)

        If errinfo = "���s" Then
            Write_Err(UserID, mToriCode, FuriDate, func, "���s", "")
        Else
            Write_Err(UserID, mToriCode, FuriDate, func, "���s", errinfo)
        End If

    End Sub


    '
    ' �@�\�@ �F �G���[���O�o��
    ' �����@ �F func    �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           result  �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           errinfo �F���O�̕⑫����ɏo�͂���G���[��񕶎���
    '
    Public Sub Write_Err(ByVal func As String, ByVal result As String, ByVal errinfo As String)

        Write_Err(UserID, mToriCode, FuriDate, func, result, errinfo)

    End Sub


    '
    ' �@�\�@ �F �G���[���O�o��
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           errinfo  �F���O�̕⑫����ɏo�͂���G���[��񕶎���
    '
    Public Sub Write_Err(ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal errinfo As String)

        Write_Err(UserID, aToriCode, aFuriDate, func, result, errinfo)

    End Sub


    '
    ' �@�\�@ �F �G���[���O�o��
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           errinfo  �F���O�̕⑫����ɏo�͂���G���[��񕶎���
    '
    Public Sub Write_Err(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal errinfo As String)

        Write_Err(Thread.CurrentThread.ManagedThreadId, aUserID, aToriCode, aFuriDate, func, result, errinfo)

    End Sub


    '
    ' �@�\�@ �F ��O�G���[���O�o��
    ' �����@ �F func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           ex  �F���O�̕⑫����ɏo�͂����O
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h���s�h���o�͂����
    '
    Public Sub Write_Err(ByVal func As String, ByVal ex As Exception)

        Write_Err(UserID, mToriCode, FuriDate, func, "���s", ex)

    End Sub


    '
    ' �@�\�@ �F ��O�G���[���O�o��
    ' �����@ �F func   �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           result �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           ex     �F���O�̕⑫����ɏo�͂����O
    '
    Public Sub Write_Err(ByVal func As String, ByVal result As String, ByVal ex As Exception)

        Write_Err(UserID, mToriCode, FuriDate, func, result, ex)

    End Sub


    '
    ' �@�\�@ �F ��O�G���[���O�o��
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           ex       �F���O�̕⑫����ɏo�͂����O
    '
    Public Sub Write_Err(ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal ex As Exception)

        Write_Err(UserID, aToriCode, aFuriDate, func, result, ex)

    End Sub


    '
    ' �@�\�@ �F ��O�G���[���O�o��
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           ex       �F���O�̕⑫����ɏo�͂����O
    '
    Public Sub Write_Err(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                         ByVal func As String, ByVal result As String, ByVal ex As Exception)

        Write_Err(Thread.CurrentThread.ManagedThreadId, aUserID, aToriCode, aFuriDate, func, result, ex.Message & ": " & ex.StackTrace)

    End Sub


    '
    ' �@�\�@ �F ��O�G���[���O�o��
    ' �����@ �F threadID �F�X���b�hID
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           errinfo  �F���O�̕⑫����ɏo�͂���G���[��񕶎���
    '
    Private Sub Write_Err(ByVal threadID As String, _
                          ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                          ByVal func As String, ByVal result As String, ByVal errinfo As String)

        SyncLock mWriteLock
            Call WriteLog(aUserID, aToriCode, aFuriDate, "[ERR ][" & threadID & "] " & _
                       func, result, errinfo)
            Call WriteLog(aUserID, aToriCode, aFuriDate, "[ERR ][" & threadID & "] " & _
                       func, result, "�ďo���X�^�b�N: " & Environment.StackTrace)
        End SyncLock

    End Sub


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���P�j
    ' �����@ �F func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter1(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���P�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter1(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL1, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���P�j
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter1(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL1, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���Q�j
    ' �����@ �F func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter2(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���Q�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter2(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL2, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���Q�j
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter2(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL2, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���R�j
    ' �����@ �F func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter3(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���R�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter3(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL3, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���R�j
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter3(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL3, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���S�j
    ' �����@ �F func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter4(ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���S�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter4(ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, Optional ByVal msg As String = "") As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL4, UserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���S�j
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Public Function Write_Enter4(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        return Write_Enter(LOG_LEVEL4, aUserID, aToriCode, aFuriDate, func, msg)

    End Function


    '
    ' �@�\�@ �F �����J�n���O�o�́i���O���x���w��j
    ' �����@ �F loglevel �F���O���x��
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i�p�����^�Ȃǁj
    ' �߂�l �F Stopwatch�I�u�W�F�N�g
    '           ���\���O���̎悵�Ȃ��ꍇ��Nothing���Ԃ�
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�J�n�h���o�͂����
    '
    Private Function Write_Enter(ByVal loglevel As Integer, _
                                 ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                                 ByVal func As String, ByVal msg As String) As System.Diagnostics.Stopwatch

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        ' �����J�n���O�o��
        Write_LEVEL(loglevel, aUserID, aToriCode, aFuriDate, func, "�J�n", msg)

        ' ���\���O�o�͎��́AStopwatch�I�u�W�F�N�g�𐶐����ĕԂ�
        If loglevel <= mPfmLog Then
            sw = System.Diagnostics.Stopwatch.StartNew()
        End If

        return sw

    End Function


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���P�j
    ' �����@ �F sw  �FStopwatch�I�u�W�F�N�g
    '           func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit1(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL1, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���P�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit1(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL1, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���P�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit1(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL1, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���Q�j
    ' �����@ �F sw  �FStopwatch�I�u�W�F�N�g
    '           func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit2(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL2, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���Q�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit2(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL2, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���Q�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit2(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL2, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���R�j
    ' �����@ �F sw  �FStopwatch�I�u�W�F�N�g
    '           func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit3(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL3, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���R�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit3(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL3, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���R�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit3(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL3, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���S�j
    ' �����@ �F sw  �FStopwatch�I�u�W�F�N�g
    '           func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit4(ByVal sw As System.Diagnostics.Stopwatch, ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL4, sw, UserID, mToriCode, FuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���S�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit4(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, Optional ByVal msg As String = "")

        Write_Exit(LOG_LEVEL4, sw, UserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���S�j
    ' �����@ �F sw       �FStopwatch�I�u�W�F�N�g
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '
    Public Sub Write_Exit4(ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Write_Exit(LOG_LEVEL4, sw, aUserID, aToriCode, aFuriDate, func, msg)

    End Sub


    '
    ' �@�\�@ �F �����I�����O�o�́i���O���x���w��j
    ' �����@ �F loglevel �F���O���x��
    '           sw       �FStopwatch�I�u�W�F�N�g
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����i���A�R�[�h�Ȃǁj
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h�I���h���o�͂����
    '           ini�t�@�C���Ő��\���O�o�͂��w�肳��Ă���ꍇ�́A���\���O���o�͂����
    '
    Private Sub Write_Exit(ByVal loglevel As Integer, ByVal sw As System.Diagnostics.Stopwatch, _
                           ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                           ByVal func As String, ByVal msg As String)

        Dim sLevel As String = ""

        ' ���\���O�o��
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
                               func, "����", sw.Elapsed.toString)
                End SyncLock

                sw = Nothing
            End If
        End If

        ' �����I�����O�o��
        Write_LEVEL(loglevel, aUserID, aToriCode, aFuriDate, func, "�I��", msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���P�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL1(ByVal func As String, ByVal msg As String)

        If msg = "����" Or msg = "���s" Then
            Write_LEVEL(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���P�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL1(ByVal func As String, ByVal result As String, ByVal msg As String)

        Write_LEVEL(LOG_LEVEL1, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���P�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL1(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL1, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���P�j
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL1(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL1, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���Q�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL2(ByVal func As String, ByVal msg As String)

        If msg = "����" Or msg = "���s" Then
            Write_LEVEL(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���Q�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL2(ByVal func As String, ByVal result As String, ByVal msg As String)

        Write_LEVEL(LOG_LEVEL2, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���Q�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL2(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL2, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���Q�j
    ' �����@ �F aUserID�F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL2(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL2, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���R�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL3(ByVal func As String, ByVal msg As String)

        If msg = "����" Or msg = "���s" Then
            Write_LEVEL(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���R�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL3(ByVal func As String, ByVal result As String, ByVal msg As String)

        Write_LEVEL(LOG_LEVEL3, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���R�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL3(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL3, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���R�j
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL3(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL3, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���S�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL4(ByVal func As String, ByVal msg As String)

        If msg = "����" Or msg = "���s" Then
            Write_LEVEL(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, msg, "")
        Else
            Write_LEVEL(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, "", msg)
        End If

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���S�j
    ' �����@ �F func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL4(ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL4, UserID, mToriCode, FuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���S�j
    ' �����@ �F aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL4(ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL4, UserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���S�j
    ' �����@ �F aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
    '
    Public Sub Write_LEVEL4(ByVal aUserID As String, ByVal aToriCode As String, ByVal aFuriDate As String, _
                            ByVal func As String, ByVal result As String, Optional ByVal msg As String = "")

        Write_LEVEL(LOG_LEVEL4, aUserID, aToriCode, aFuriDate, func, result, msg)

    End Sub


    '
    ' �@�\�@ �F ���O�o�́i���O���x���w��j
    ' �����@ �F loglevel �F���O���x��
    '           aUserID  �F���[�U��
    '           aToriCode�F�����R�[�h
    '           aFuriDate�F�U�֓�
    '           func     �F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�������A�N���X.���\�b�h���Ȃǁj
    '           result   �F���O�̌��ʈ�ɏo�͂��錋�ʕ�����
    '           msg      �F���O�̕⑫����ɏo�͂�����
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
    ' �@�\�@ �F ���O���x���P���L�����𔻒肷��
    ' �߂�l �F True - �L���CFalse - ����
    '
    Public Function IS_LEVEL1() As Boolean

        return IsLogLevel(LOG_LEVEL1)

    End Function


    '
    ' �@�\�@ �F ���O���x���Q���L�����𔻒肷��
    ' �߂�l �F True - �L���CFalse - ����
    '
    Public Function IS_LEVEL2() As Boolean

        return IsLogLevel(LOG_LEVEL2)

    End Function


    '
    ' �@�\�@ �F ���O���x���R���L�����𔻒肷��
    ' �߂�l �F True - �L���CFalse - ����
    '
    Public Function IS_LEVEL3() As Boolean

        return IsLogLevel(LOG_LEVEL3)

    End Function


    '
    ' �@�\�@ �F ���O���x���S���L�����𔻒肷��
    ' �߂�l �F True - �L���CFalse - ����
    '
    Public Function IS_LEVEL4() As Boolean

        return IsLogLevel(LOG_LEVEL4)

    End Function


    '
    ' �@�\�@ �F SQL���O���x�����L�����𔻒肷��
    ' �߂�l �F True - �L���CFalse - ����
    '
    Public Function IS_SQLLOG() As Boolean

        If mSqlLog = 1 Then
            return True
        Else
            return False
        End If

    End Function


    '
    ' �@�\�@ �F �w�胍�O���x�����L�����𔻒肷��
    ' �����@ �F loglevel�F ���O���x��
    ' �߂�l �F True - �L���CFalse - ����
    '
    Private Function IsLogLevel(ByVal loglevel As Integer) As Boolean

        If (loglevel > mLogLevel) And (loglevel > mPfmLog) Then
            return False
        Else
            return True
        End If

    End Function


    '
    ' �@�\�@ �F SQL���O�o�́iDB�A�N�Z�X�N���X�p�j
    ' �����@ �F func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           sql �FSQL��
    ' ���l�@ �F UIDMAST��INSERT/UPDATE�̏ꍇ�ASQL���O�o�͂��Ȃ�
    '
    Public Sub Write_SQL(ByVal func As String, ByVal sql As String)

        If mSqlLog = 1 Then
            ' ���[�U�p�X���[�h������SQL���O�ɏo���Ȃ�
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
    ' �@�\�@ �F SQL�G���[���O�o�́iDB�A�N�Z�X�N���X�p�j
    ' �����@ �F func�F���O�̏����ڍ׈�ɏo�͂��鏈�����e�i�N���X.���\�b�h���Ȃǁj
    '           sql �FSQL��
    ' ���l�@ �F ���O�̌��ʈ�ɂ́h���s�h���o�͂����
    '           UIDMAST��INSERT/UPDATE�̏ꍇ�ASQL���O�o�͂��Ȃ�
    '
    Public Sub Write_SQL_Err(ByVal func As String, ByVal sql As String)

        ' ���[�U�p�X���[�h������SQL���O�ɏo���Ȃ�
        Dim sqlUpper As String = sql.ToUpper
        If sqlUpper.IndexOf("UIDMAST") >= 0 Then
            If sqlUpper.IndexOf("INSERT") >= 0 OrElse sqlUpper.IndexOf("UPDATE") >= 0 Then
                Exit Sub
            End If
        End If

        SyncLock mWriteLock
            Call WriteLog(UserID, mToriCode, FuriDate, "[ERR ][" & Thread.CurrentThread.ManagedThreadId & "] " & _
                       func, "���s", "SQL=" & sql)
        End SyncLock

    End Sub

'*** End Add 2015/12/01 SO)�r�� for ���O���� ***

End Class
