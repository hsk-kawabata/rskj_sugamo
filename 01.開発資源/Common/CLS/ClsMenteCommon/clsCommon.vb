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
    Public MainLog As CASTCommon.BatchLOG   '���O����p
    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
    Private IS_LEVEL3 As Boolean
    Private IS_LEVEL4 As Boolean
    Private IS_SQLLOG As Boolean
    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

    Public LogUserID As String
    Public LogToriCode As String
    Public LogFuriDate As String
    Private Structure DefineOnThisProject
        Dim UserID As String                'UserID(�N���p�����[�^�A�g)
        Dim SysDate As Date                 'SystemDate(�N���p�����[�^�A�g)
        Dim BinFolder As String             '���s���W���[���i�[�p�X
        Dim LogFolder As String             '���O���i�[�p�X
        Dim TXTFolder As String             '�e�L�X�g�t�@�C���i�[�p�X
        Dim PRTFolder As String             '����A�g�p�t�@�C���i�[�p�X
        Dim LSTFolder As String             '���\��`�̊i�[�p�X
        Dim DLLFolder As String             'DLL�i�[�p�X
        Dim Item() As String                '(�ėp)�z����L��
        Dim GLogFolder As String            '�w�Z���O���i�[�p�X
    End Structure
    Private GSYS As DefineOnThisProject

    Public Structure LogWriteStructure
        Dim ToriCode As String          '�����R�[�h(0000000-00)
        Dim FuriDate As String          '�U�֓�(yyyymmdd)
        Dim Job As String               '(�啪�ރW���u��) Log File�̖��̂Ɏg�p
        Dim Job1 As String              '(�����ރW���u��)
        Dim Job2 As String              '(�����ރW���u��)
        Dim Result As String            '�����̌��ʓ�
        Dim Discription As String       '���l��(�i�[�t�@�C����+���s�֐���+�G���[���e)
    End Structure
    Public GLog As LogWriteStructure

    '�x�����
    Private Data() As Integer

    Public Enum enDB
        DB_Connect = 1        'DB�ڑ�
        DB_Begin = 2          '�g�����U�N�V�����J�n
        DB_Execute = 4        'SQL���̎��s
        DB_Commit = 8         '�R�~�b�g
        DB_Rollback = 16      '���[���o�b�N
        DB_Terminate = 32     'DB�ؒf
    End Enum

    Public Structure JIFURI_Session
        Dim FUNOU As Integer                'GIntFUNOU          �s�\�敪��荞��
        Dim FUNOU_TAKOU_1 As Integer        'GIntFUNOU_TAKOU_1  �s�\�敪��荞�݁����s��g����
        Dim FUNOU_TAKOU_2 As Integer        'GIntFUNOU_TAKOU_2  �s�\�敪��荞�݁����s��g�O��
        Dim FUNOU_SSS_1 As Integer          'GIntFUNOU_SSS_1    �s�\�敪��荞�݁�SSS���s��g����
        Dim FUNOU_SSS_2 As Integer          'GIntFUNOU_SSS_2    �s�\�敪��荞�݁�SSS���s��g�O��
        Dim HAISIN As Integer               'GIntHAISIN         �z�M�敪��荞��
        Dim HAISIN_TAKOU_1 As Integer       'GIntHAISIN_TAKOU_1 �z�M�敪(���s��g��)��荞��
        Dim HAISIN_TAKOU_2 As Integer       'GIntHAISIN_TAKOU_2 �z�M�敪(���s��g�O)��荞��
        Dim HAISIN_SSS_1 As Integer         'GIntHAISIN_SSS_1   �z�M�敪(SSS���s��g��)��荞��
        Dim HAISIN_SSS_2 As Integer         'GINtHAISIN_SSS_2   �z�M�敪(SSS���s��g�O)��荞��
        Dim KAISYU As Integer               'gintKAISYU_KBN     ����敪��荞��
        Dim HITS As Integer
        Dim HITQ As Integer
    End Structure
    Public DayINI As JIFURI_Session

    Private OraConnect As OracleClient.OracleConnection
    Private Tran As OracleTransaction
    Private Command As OracleCommand

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
    Private GCOM_MainDB As CASTCommon.MyOracle   '���C��DB�R�l�N�V����

    Public WriteOnly Property Oracle() As CASTCommon.MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            GCOM_MainDB = Value
        End Set
    End Property
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

    Public Const GAppName As String = "Main"
    Public Const NG As String = "���s"
    Public Const OK As String = "����"
    Public Const ErrorString As String = "�\�����ʃG���["
    Public Const BadResultDate As Date = #1/1/1900#

    '*** �C�� mitsu 2008/09/01 �b���C�� ***
    Private Const IntWaitTimer As Integer = 15000
    '**************************************
    Private Const ThisModuleName As String = "clsCommon.vb"

    '*** �C�� mitsu 2008/05/27 �����ɃR�[�h�ǉ� ***
    Public ReadOnly JIKINKOCD As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
    '**********************************************

    '*** �C�� mitsu 2008/09/01 �G���R�[�f�B���O�ǉ� ***
    Public ReadOnly EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")
    '**************************************************

    '�N���p�����[�^����擾�������[�UID
    Public Property GetUserID() As String
        Get
            Return GSYS.UserID
        End Get
        Set(ByVal Value As String)
            GSYS.UserID = Value
        End Set
    End Property

    '�N���p�����[�^����擾�����V�X�e�����t
    Public Property GetSysDate() As Date
        Get
            Return GSYS.SysDate
        End Get
        Set(ByVal Value As Date)
            GSYS.SysDate = Value
        End Set
    End Property

    'INI�t�@�C������擾�������s���W���[���i�[�p�X
    Public ReadOnly Property GetBinFolder() As String
        Get
            Return GSYS.BinFolder
        End Get
    End Property

    'INI�t�@�C������擾�������O�t�@�C���i�[�p�X
    Public Property GetLogFolder() As String
        Get
            Return GSYS.LogFolder
        End Get
        Set(ByVal Value As String)
            GSYS.LogFolder = Value
        End Set
    End Property

    'INI�t�@�C������擾�������O�t�@�C���i�[�p�X
    Public Property GetGLogFolder() As String
        Get
            Return GSYS.GLogFolder
        End Get
        Set(ByVal Value As String)
            GSYS.GLogFolder = Value
        End Set
    End Property

    'INI�t�@�C������擾�����e�L�X�g���i�[�p�X
    Public Property GetTXTFolder() As String
        Get
            Return GSYS.TXTFolder
        End Get
        Set(ByVal Value As String)
            GSYS.TXTFolder = Value
        End Set
    End Property

    'INI�t�@�C������擾�������\��`�̊i�[�p�X
    Public Property GetLSTFolder() As String
        Get
            Return GSYS.LSTFolder
        End Get
        Set(ByVal Value As String)
            GSYS.LSTFolder = Value
        End Set
    End Property

    'INI�t�@�C������擾����DLL�i�[�p�X
    Public ReadOnly Property GetDLLFolder() As String
        Get
            Return GSYS.DLLFolder
        End Get
    End Property

    'INI�t�@�C������擾��������pCSV�i�[�p�X
    Public ReadOnly Property GetPRTFolder() As String
        Get
            Return GSYS.PRTFolder
        End Get
    End Property

    '�N���p�����[�^����擾�������[�UID
    Public Property UseItem() As String()
        Get
            Return GSYS.Item
        End Get
        Set(ByVal Value As String())
            GSYS.Item = Value
        End Set
    End Property

    '�����W���[���i�[�t�@�C����
    Public ReadOnly Property GetThisModuleName() As String
        Get
            Return ThisModuleName
        End Get
    End Property

    Public Sub New()

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        MainLog = New BatchLOG("clsCommon", "GCOM")
        IS_LEVEL3 = MainLog.IS_LEVEL3()
        IS_LEVEL4 = MainLog.IS_LEVEL4()
        IS_SQLLOG = MainLog.IS_SQLLOG()
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        '���s�t�H���_�ݒ�
        GSYS.BinFolder = GetAppPath()

        'TXT�t�@�C���i�[�ꏊ�ݒ�
        GSYS.TXTFolder = CASTCommon.GetFSKJIni("COMMON", "TXT")

        '��`�̃t�@�C���i�[�ꏊ�ݒ�
        GSYS.LSTFolder = CASTCommon.GetFSKJIni("COMMON", "LST")

        'DLL�t�@�C���i�[�ꏊ�ݒ�
        GSYS.DLLFolder = CASTCommon.GetFSKJIni("COMMON", "DLL")

        'LOG�t�@�C���i�[�ꏊ�ݒ�
        GSYS.LogFolder = CASTCommon.GetFSKJIni("COMMON", "LOG")
        If Not GSYS.LogFolder.ToUpper = "ERR" Then

            '�t�H���_�L���m�F(������΍쐬)
            If Not System.IO.Directory.Exists(GSYS.LogFolder) Then
                System.IO.Directory.CreateDirectory(GSYS.LogFolder)
            End If
        End If

        'GLOG�t�@�C���i�[�ꏊ�ݒ�
        GSYS.GLogFolder = CASTCommon.GetFSKJIni("GCOMMON", "LOG")
        If Not GSYS.GLogFolder.ToUpper = "ERR" Then

            '�t�H���_�L���m�F(������΍쐬)
            If Not System.IO.Directory.Exists(GSYS.GLogFolder) Then
                System.IO.Directory.CreateDirectory(GSYS.GLogFolder)
            End If
        End If

        'CSV�t�@�C���i�[�ꏊ�ݒ�
        GSYS.PRTFolder = CASTCommon.GetFSKJIni("COMMON", "PRT")
        If Not GSYS.PRTFolder.ToUpper = "ERR" Then

            '�t�H���_�L���m�F(������΍쐬)
            If Not System.IO.Directory.Exists(GSYS.PRTFolder) Then
                System.IO.Directory.CreateDirectory(GSYS.PRTFolder)
            End If
        End If

        '2007,12,12 �ǉ� By Astar
        With DayINI
            '�s�\�敪��荞��
            .FUNOU = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU"))

            '�s�\�敪��荞�݁����s��g����
            .FUNOU_TAKOU_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_TAKOU_1"))
            If .FUNOU_TAKOU_1 = 0 Then
                .FUNOU_TAKOU_1 = .FUNOU
            End If

            '�s�\�敪��荞�݁����s��g�O��
            .FUNOU_TAKOU_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_TAKOU_2"))
            If .FUNOU_TAKOU_2 = 0 Then
                .FUNOU_TAKOU_2 = .FUNOU_TAKOU_1
            End If

            '�s�\�敪��荞�݁�SSS���s��g����
            .FUNOU_SSS_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_1"))
            If .FUNOU_SSS_1 = 0 Then
                .FUNOU_SSS_1 = .FUNOU
            End If

            '�s�\�敪��荞�݁�SSS���s��g�O��
            .FUNOU_SSS_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_2"))
            If .FUNOU_SSS_2 = 0 Then
                .FUNOU_SSS_2 = .FUNOU_SSS_1
            End If

            '�z�M�敪��荞��
            .HAISIN = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN"))

            '�z�M�敪(���s��g��)��荞��
            .HAISIN_TAKOU_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_TAKOU_1"))
            If .HAISIN_TAKOU_1 = 0 Then
                .HAISIN_TAKOU_1 = .HAISIN
            End If

            '�z�M�敪(���s��g�O)��荞��
            .HAISIN_TAKOU_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_TAKOU_2"))
            If .HAISIN_TAKOU_2 = 0 Then
                .HAISIN_TAKOU_2 = .HAISIN_TAKOU_1
            End If

            '�z�M�敪(SSS���s��g��)��荞��
            .HAISIN_SSS_1 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_SSS_1"))
            If .HAISIN_SSS_1 = 0 Then
                .HAISIN_SSS_1 = .HAISIN
            End If

            '�z�M�敪(SSS���s��g�O)��荞��
            .HAISIN_SSS_2 = NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_SSS_2"))
            If .HAISIN_SSS_2 = 0 Then
                .HAISIN_SSS_2 = .HAISIN_SSS_1
            End If

            '����敪��荞��
            .KAISYU = NzInt(CASTCommon.GetFSKJIni("JIFURI", "KAISYU"))
        End With

        '���O�֘A�l�̏�����
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
    ' �@�@�\ : �ғ��ꏊ�̎擾
    '
    ' �߂�l : �ғ��ꏊ
    '
    ' ������ : ARG1 - �Ȃ�
    '
    ' ���@�l : �ėp
    '    
    Public Function GetAppPath() As String
        Dim FL As New System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)
        Return FL.DirectoryName
    End Function

    '
    ' �@�\�@�@�@: ���O�o�̓��W���[��
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - ���N�G�X�g�E���W���[���i�[�t�@�C����
    ' �@�@�@�@�@  ARG2 - ���N�G�X�g�E���W���[��
    '
    ' ���l�@�@�@: ���p����
    '           : ���O�o�͂Ɏ��s�����ꍇ(�ʏ킠�肦�Ȃ�)�͒ʔԂ��J�E���g�A�b�v����
    '
    Public Sub FN_LOG_WRITE(ByVal avOnCallFileName As String, ByVal avOnCallModule As StackTrace)
        Dim MSG As String
        Dim FS As FileStream
        Dim FW As StreamWriter = Nothing
        Const C34 As String = ControlChars.Quote
        Dim DRet As DialogResult
        '*** �C�� mitsu 2008/08/04 ���O�o�͎��s�Ή� ***
        Dim FileSeq As Integer = 0
        Dim ErrCnt As Integer = 0
        '**********************************************

        '*** �C�� mitsu 2008/08/04 ���O�o�͎��s�Ή� ***
        While True
            '******************************************
            Try
                Dim sFileName As String = SET_PATH(GSYS.LogFolder)
                sFileName &= GLog.Job
                '*** �C�� mitsu 2008/08/04 �t�@�C�����ɃV�[�P���X�ǉ� ***
                'sFileName &= String.Format("{0:yyyyMMdd}", Date.Now) & ".LOG"
                sFileName &= String.Format("{0:yyyyMMdd}", Date.Now) & "." & FileSeq.ToString("00000") & ".LOG"
                '********************************************************

                '*** �C�� mitsu 2008/09/01 ����̏ꍇ�̓X�^�b�N�g���[�X���o�͂��Ȃ� ***
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
                    '*** �C�� mitsu 2008/09/01 ���������� ***
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

                    '�t�@�C����������΍��
                    FS = New FileStream(sFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
                    FW = New StreamWriter(FS, EncdJ)
                Else
                    '*** �C�� mitsu 2008/08/04 ���O�o�͎��s�Ή� ***

                    '���V�[�P���X�t�@�C�������݂���ԃJ�E���g�A�b�v
                    While File.Exists(SET_PATH(GSYS.LogFolder) & GLog.Job & _
                        String.Format("{0:yyyyMMdd}", Date.Now) & "." & (FileSeq + 1).ToString("00000") & ".LOG")
                        FileSeq += 1

                        sFileName = SET_PATH(GSYS.LogFolder)
                        sFileName &= GLog.Job
                        sFileName &= String.Format("{0:yyyyMMdd}", Date.Now) & "." & FileSeq.ToString("00000") & ".LOG"
                    End While
                    '**********************************************

                    '�t�@�C����ǉ����[�h�ŊJ��
                    FS = New FileStream(sFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                    FW = New StreamWriter(FS, EncdJ)
                End If

                '*** �C�� mitsu 2008/09/08 User���ɃR���s���[�^����ǉ� ***
                '0 = ID�ԍ�(�ʔ�)
                '1 = ����
                '2 = JOB�ʔ�(JOB�Ǘ�Master)
                '3 = User�� + �R���s���[�^��
                '4 = �����R�[�h�i��^���j
                '5 = �U�֓�
                '6 = �������e
                '7 = �W���u���e
                '8 = ����
                '9 = ����(FREE)
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
                '*** �C�� mitsu 2008/08/04 ���O�o�͎��s�Ή� ***
                Exit While
                '**********************************************

            Catch ex As Exception
                '*** �C�� mitsu 2008/08/04 ���O�o�͎��s�Ή� ***
                'DRet = MessageBox.Show(ex.Message, New StackTrace(True).GetFrame(0).GetMethod.Name, _
                '       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FileSeq += 1
                ErrCnt += 1

                '�G���[�񐔂�3��𒴂����ꍇ�͋����I��
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
            '*** �C�� mitsu 2008/08/04 ���O�o�͎��s�Ή� ***
        End While
        '**************************************************
    End Sub

'*** Str Add 2015/12/01 SO)�r�� for ���O�����i���g�p���\�b�h�Ȃ̂ŃR�����g���j ***
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
'*** End Add 2015/12/01 SO)�r�� for ���O�����i���g�p���\�b�h�Ȃ̂ŃR�����g���j ***

    '
    ' �@�\�@�@�@: SQL�G���[���O�o�̓��W���[��
    '
    ' �߂�l�@�@: �Ȃ�
    '
    ' �������@�@: ARG1 - ���N�G�X�g�E���W���[���i�[�t�@�C����
    ' �@�@�@�@�@  ARG2 - ���N�G�X�g�E���W���[��
    ' �@�@�@�@�@  ARG3 - SQL ERROR ONLY
    '
    ' ���l�@�@�@: ���p����
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

            '*** �C�� mitsu 2008/09/01 ����̏ꍇ�̓X�^�b�N�g���[�X���o�͂��Ȃ� ***
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
                '*** �C�� mitsu 2008/09/01 ���������� ***
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

                '�t�@�C����������΍��
                FS = New FileStream(sFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
                FW = New StreamWriter(FS, EncdJ)
            Else
                '�t�@�C����ǉ����[�h�ŊJ��
                FS = New FileStream(sFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                FW = New StreamWriter(FS, EncdJ)
            End If

            '*** �C�� mitsu 2008/09/08 User���ɃR���s���[�^����ǉ� ***
            '0 = ID�ԍ�(�ʔ�)
            '1 = ����
            '2 = JOB�ʔ�(JOB�Ǘ�Master)
            '3 = User�� + �R���s���[�^��
            '4 = �����R�[�h�i��^���j
            '5 = �U�֓�
            '6 = �������e
            '7 = �W���u���e
            '8 = ����
            '9 = ����(FREE)
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
    ' �@�@�\ : �p�X����
    '
    ' �߂�l : �����l
    '
    ' ������ : ARG1 - �p�X��
    ' �@�@�@   ARG2 - ��������
    ' �@�@�@   ARG3 - True = �t��, False = �폜
    '
    ' ���@�l : �ėp
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
    ' �@�\�@�@�@: �e�L�X�g�{�b�N�X�œ��͂��ꂽ���t�f�[�^��]������
    '
    ' �߂�l�@�@: ������t = OK(-1)
    ' �@�@�@�@�@  �ُ���t = SetFocus���ׂ�TextBox(Index)�l
    '
    ' �������@�@: ARG1 - ���t
    ' �@�@�@�@�@  ARG2 - �e�L�X�g�{�b�N�X���
    '
    ' �@�\�����@: �e�L�X�g�{�b�N�X�p�̃`�F�b�N�֐�
    '
    ' ���l�@�@�@: ���p����
    '
    Public Function SET_DATE(ByRef onDate As Date, ByRef onText() As Integer) As Integer
        Try
            Select Case onText(0)
                Case 1900 To 2100
                    '�����Ĕ͈͂��w�肷��

                    Select Case onText(1)
                        Case 1 To 12
                            '�����G���[

                            onDate = DateSerial(onText(0), onText(1), onText(2))

                            If Not onDate.Year = onText(0) OrElse _
                                Not onDate.Month = onText(1) OrElse _
                                Not onDate.Day = onText(2) Then

                                Return 2
                            End If
                        Case Else
                            '�����G���[

                            Return 1
                    End Select
                Case Else
                    '�N���ΏۊO

                    Return 0
            End Select

            Return -1
        Catch ex As Exception
            With GLog
                .Job2 = "���͓��t�f�[�^�̕]��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��

            '�z��O�G���[
            Return 9
        End Try
    End Function

    '
    ' �@�\�@�@�@: ���t�f�[�^�������]�����ē��t�^�ɕϊ�����
    '
    ' �߂�l�@�@: ������t = OK(-1)
    ' �@�@�@�@�@  �ُ���t = Nothing
    '
    ' �������@�@: ARG1 - ���t
    ' �@�@�@�@�@  ARG2 - �e�L�X�g�{�b�N�X���
    '
    ' �@�\�����@: �e�L�X�g�{�b�N�X�p�̃`�F�b�N�֐�
    '
    ' ���l�@�@�@: ���p����
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
                .Job2 = "���t�f�[�^�̓��t�^�ϊ�"
                .Result = NG
                .Discription = "(" & onString & ") " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Nothing
    End Function

    '
    ' �@�\�@ �F �c�Ɠ�����֐�
    '
    ' �����@ �F ARG1(onDate)    - �]��������t
    ' �@�@�@ �@ ARG2(SEL)       - �]�� = 0, �~�� = Else
    ' �@�@�@ �@ ARG3(SubSQL)    - �~�ώ��̏�����
    '
    ' �߂�l �F �x���w���  = False
    ' �@�@�@ �@ �c�Ɠ�      = True
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function CheckDateModule(ByVal onDate As Date, _
                Optional ByVal SEL As Short = 0, Optional ByVal SubSQL As String = "") As Boolean
        If SEL = 0 Then
            '�]��
            Try
                If onDate.DayOfWeek = System.DayOfWeek.Saturday OrElse _
                   onDate.DayOfWeek = System.DayOfWeek.Sunday Then
                    '�y���̏ꍇ�͔�c�Ɠ�
                    Return False
                Else
                    Dim Temp As Integer = NzInt(String.Format("{0:yyyyMMdd}", onDate))
                    '�x���o�^�����m�ł��Ȃ��ꍇ�͉c�Ɠ��Ƃ���
                    For Index As Integer = 1 To Data.GetUpperBound(0) Step 1
                        Select Case Data(Index)
                            Case Is = Temp
                                '����l�͋x���̏�
                                Return False
                            Case Is > Temp
                                '��������薢���ɂȂ�ΏI��
                                Exit For
                        End Select
                    Next Index

                    Return True
                End If
            Catch ex As Exception
                With GLog
                    .Job2 = "�c�Ɠ�����֐�(�]��)"
                    .Result = NG
                    .Discription = ex.Message
                End With
                'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            End Try
        Else
            '�~��
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
                    .Job2 = "�c�Ɠ�����֐�(�~��)"
                    .Result = NG
                    .Discription = ex.Message
                End With
                'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
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
    ' �@�\�@ �F �c�Ɠ�����֐�
    '
    ' �����@ �F ARG1(aonDate)        - �]��������t
    ' �@�@�@ �@ ARG2(aOpenDate)      - �]����̓��t
    ' �@�@�@ �@ ARG3(aFrontBackType) - 0    = �O���փX���C�h����(���Ԃ��i��)
    ' �@�@�@ �@                        Else = ����փX���C�h����(���Ԃ��߂�)
    '
    ' �߂�l �F aonDate<>��� = False
    ' �@�@�@ �@ aonDate=���=�c�Ɠ� = True
    ' �@�@�@ �@ aonDate=���<>�c�Ɠ� = False
    '
    ' ���l�@ �F ����Target���t�͉c�Ɠ����肳��A��c�Ɠ��̏ꍇ�ɂ�aOpenDate�ɑO��̉c�Ɠ����Z�b�g����
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aFrontBackType As Integer) As Boolean

        aOpenDate = aTgetDate               '����������
        Dim ReturnValue As Boolean = False  'aonDate���c�Ɠ����ۂ��̔���(�߂�l)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '���t�ϐ�

            '�����������t�]���ł��Ȃ��ꍇ�ɂ̓_��
            If Not onDate = Nothing Then

                Dim LoopCounter As Integer = 0              '���[�v�����̉�
                Do
                    '�Q��ڈȍ~�͓��t���X���C�h������
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType
                            Case Is = 0
                                '�O���փX���C�h����(���Ԃ��i��)
                                onDate = onDate.AddDays(1)
                            Case Else
                                '����փX���C�h����(���Ԃ��߂�)
                                onDate = onDate.AddDays(-1)
                        End Select
                    End If

                    '�c�Ɠ�����(�c�Ɠ��ł���΃��[�v�I��)
                    If CheckDateModule(onDate) = True Then

                        Exit Do
                    End If

                    LoopCounter += 1
                Loop While LoopCounter <= 366

                '�����̉c�Ɠ����t��ݒ肷��
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)

                ReturnValue = (aOpenDate = aTgetDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "�P���c�Ɠ�����֐�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
        Return ReturnValue
    End Function

    '
    ' �@�\�@ �F �c�Ɠ��Z�o�֐�
    '
    ' �����@ �F ARG1(aonDate)        - �]��������t
    ' �@�@�@ �@ ARG2(aOpenDate)      - �]����̓��t
    ' �@�@�@ �@ ARG3(aDayTeam)       - �X���C�h�������(�W���l=1�c�Ɠ�)
    ' �@�@�@ �@ ARG4(aFrontBackType) - 0    = �O���փX���C�h����(���Ԃ��i��:�W���l)
    ' �@�@�@ �@                        Else = ����փX���C�h����(���Ԃ��߂�)
    '
    ' �߂�l �F aonDate<>��� = False
    ' �@�@�@ �@ aonDate=���=�c�Ɠ� = True
    ' �@�@�@ �@ aonDate=���<>�c�Ɠ� = False
    '
    ' ���l�@ �F ����Target���t�͏�ɉc�Ɠ����肷��̂�
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aDayTeam As Integer, _
                                    ByVal aFrontBackType As Integer) As Boolean

        aOpenDate = aTgetDate               '����������
        Dim ReturnValue As Boolean = False  'aonDate���c�Ɠ����ۂ��̔���(�߂�l)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '���t�ϐ�

            '�����������t�]���ł��Ȃ��ꍇ�ɂ̓_��
            If Not onDate = Nothing Then

                Dim DayTeamCounter As Integer = 0           '�c�Ɠ��o�߃J�E���^�[
                Dim LoopCounter As Integer = 0              '���[�v�����̉�
                Do
                    '�Q��ڈȍ~�͓��t���X���C�h������
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType
                            Case Is = 0
                                '�O���փX���C�h����(���Ԃ��i��)
                                onDate = onDate.AddDays(1)
                            Case Else
                                '����փX���C�h����(���Ԃ��߂�)
                                onDate = onDate.AddDays(-1)
                        End Select
                    End If

                    '�y���ȊO�ŋx���o�^�����m�ł��Ȃ��ꍇ�͉c�Ɠ��Ƃ���
                    If CheckDateModule(onDate) = True Then
                        '
                        If LoopCounter = 0 Then
                            '���񂾂��ݒ肷��(����͉c�Ɠ������҂���)

                            ReturnValue = True
                        Else
                            '�Q��ڈȍ~�͉c�Ɠ������J�E���^�[���J��グ��
                            DayTeamCounter += 1
                        End If
                    End If

                    LoopCounter += 1

                    '�w�w�c�Ɠ������[������ΏI������
                Loop While DayTeamCounter < aDayTeam

                '�����̂w�w�c�Ɠ�����t��ݒ肷��
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "�O��c�Ɠ��Z�o�֐�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
        Return ReturnValue
    End Function

    '
    ' �@�\�@ �F �c�Ɠ�����֐�
    '
    ' �����@ �F ARG1(aonDate)        - �]��������t
    ' �@�@�@ �@ ARG2(aOpenDate)      - �]����̓��t
    ' �@�@�@ �@ ARG3(aFrontBackType) - 0    = �O���փX���C�h����(���Ԃ��i��)
    ' �@�@�@ �@                        Else = ����փX���C�h����(���Ԃ��߂�)
    '
    ' �߂�l �F aonDate<>��� = False
    ' �@�@�@ �@ aonDate=���=�c�Ɠ� = True
    ' �@�@�@ �@ aonDate=���<>�c�Ɠ� = False
    '
    ' ���l�@ �F ����Target���t�͉c�Ɠ����肳��A��c�Ɠ��̏ꍇ�ɂ�aOpenDate�ɑO��̉c�Ɠ����Z�b�g����
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aFrontBackType As String) As Boolean

        aOpenDate = aTgetDate               '����������
        Dim ReturnValue As Boolean = False  'aonDate���c�Ɠ����ۂ��̔���(�߂�l)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '���t�ϐ�

            '�����������t�]���ł��Ȃ��ꍇ�ɂ̓_��
            If Not onDate = Nothing Then

                Dim LoopCounter As Integer = 0              '���[�v�����̉�
                Do
                    '�Q��ڈȍ~�͓��t���X���C�h������
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType.ToUpper
                            Case Is = "BACK"
                                '����փX���C�h����(���Ԃ��߂�)
                                onDate = onDate.AddDays(-1)
                            Case Else
                                '�O���փX���C�h����(���Ԃ��i��)
                                onDate = onDate.AddDays(1)
                        End Select
                    End If

                    '�c�Ɠ�����(�c�Ɠ��ł���΃��[�v�I��)
                    If CheckDateModule(onDate) = True Then

                        Exit Do
                    End If

                    LoopCounter += 1
                Loop While LoopCounter <= 366

                '�����̉c�Ɠ����t��ݒ肷��
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)

                ReturnValue = (aOpenDate = aTgetDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "�P���c�Ɠ�����֐�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
        Return ReturnValue
    End Function

    '
    ' �@�\�@ �F �c�Ɠ��Z�o�֐�
    '
    ' �����@ �F ARG1(aonDate)        - �]��������t
    ' �@�@�@ �@ ARG2(aOpenDate)      - �]����̓��t
    ' �@�@�@ �@ ARG3(aDayTeam)       - �X���C�h�������(�W���l=1�c�Ɠ�)
    ' �@�@�@ �@ ARG4(aFrontBackType) - 0    = �O���փX���C�h����(���Ԃ��i��:�W���l)
    ' �@�@�@ �@                        BACK = ����փX���C�h����(���Ԃ��߂�)
    '
    ' �߂�l �F aonDate<>��� = False
    ' �@�@�@ �@ aonDate=���=�c�Ɠ� = True
    ' �@�@�@ �@ aonDate=���<>�c�Ɠ� = False
    '
    ' ���l�@ �F ����Target���t�͏�ɉc�Ɠ����肷��̂�
    '
    Public Function CheckDateModule(ByRef aTgetDate As String, _
                                    ByRef aOpenDate As String, _
                                    ByVal aDayTeam As Integer, _
                                    ByVal aFrontBackType As String) As Boolean

        aOpenDate = aTgetDate               '����������
        Dim ReturnValue As Boolean = False  'aonDate���c�Ɠ����ۂ��̔���(�߂�l)
        Try
            Dim onDate As Date = SET_DATE(aTgetDate)    '���t�ϐ�

            '�����������t�]���ł��Ȃ��ꍇ�ɂ̓_��
            If Not onDate = Nothing Then

                Dim DayTeamCounter As Integer = 0           '�c�Ɠ��o�߃J�E���^�[
                Dim LoopCounter As Integer = 0              '���[�v�����̉�
                Do
                    '�Q��ڈȍ~�͓��t���X���C�h������
                    If LoopCounter > 0 Then

                        Select Case aFrontBackType.ToUpper
                            Case Is = "BACK"
                                '����փX���C�h����(���Ԃ��߂�)
                                onDate = onDate.AddDays(-1)
                            Case Else
                                '�O���փX���C�h����(���Ԃ��i��)
                                onDate = onDate.AddDays(1)
                        End Select
                    End If

                    '�y���ȊO�ŋx���o�^�����m�ł��Ȃ��ꍇ�͉c�Ɠ��Ƃ���
                    If CheckDateModule(onDate) = True Then
                        '
                        If LoopCounter = 0 Then
                            '���񂾂��ݒ肷��(����͉c�Ɠ������҂���)

                            ReturnValue = True
                        Else
                            '�Q��ڈȍ~�͉c�Ɠ������J�E���^�[���J��グ��
                            DayTeamCounter += 1
                        End If
                    End If

                    LoopCounter += 1

                    '�w�w�c�Ɠ������[������ΏI������
                Loop While DayTeamCounter < aDayTeam

                '�����̂w�w�c�Ɠ�����t��ݒ肷��
                aOpenDate = String.Format("{0:yyyyMMdd}", onDate)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "�O��c�Ɠ��Z�o�֐�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
        Return ReturnValue
    End Function

    '
    ' �@�@�\ : ���׍s�̏ڍו\��
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �ΏۃR���g���[��
    '
    ' ���@�l : ���׍s�Q�Ǝ��̔ėp�I�Ȋ֐�
    '    
    Public Sub MonitorCsvFile(ByVal ListView As ListView)

        '2008.05.01 By Astar �@�\�p�~
        '*** 2008/5/14 kakinoki �@�\���A ***
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
                .Job2 = "���׍s�̏ڍו\��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        Finally
            FL = Nothing
        End Try
    End Sub

    '
    ' �@�@�\ : �Ώےl�̎Q��
    '
    ' �߂�l : �Q�ƒl
    '
    ' ������ : ARG1 - �ΏۃR���g���[��
    ' �@�@�@   ARG2 - �Ώۗ�
    '
    ' ���@�l : ���׍s�Q�Ǝ��̐�p�֐�
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
                .Job2 = "�Ώےl�̎Q��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Function

    '
    ' �@�@�\ : �Ώےl�̐ݒ�
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �ΏۃR���g���[��
    ' �@�@�@   ARG2 - �Ώۗ�
    ' �@�@�@   ARG3 - �Ώۗ�̒l
    '
    ' ���@�l : ���׍s�Q�Ǝ��̐�p�֐�
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
                .Job2 = "�Ώےl�̐ݒ�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Sub

    '
    ' �@�@�\ : �\���̈�̗L���\���s��
    '
    ' �߂�l : �s��
    '
    ' ������ : ARG1 - �ΏۃR���g���[��
    '
    ' ���@�l : ���׍s�Q�Ǝ��̐�p�֐�
    '    
    Public Function GetListViewHasRow(ByVal ListView As ListView) As Integer
        Try
            Dim BreakFast As ListView.SelectedListViewItemCollection = ListView.SelectedItems

            GetListViewHasRow = BreakFast.Count

        Catch ex As Exception
            With GLog
                .Job2 = "�\���̈�̗L���\���s��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Function

    '
    ' �@�@�\ : ��ʖ������^�L����
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �Ώۉ��
    ' �@�@�@   ARG2 - �L���^����
    '
    ' ���@�l : ��ʑJ�ڎ��̐���֐�
    '    
    Public Sub SetFormEnabled(ByVal onForm As Form, Optional ByVal SEL As Boolean = True)
        Try
            Dim CTL As Control
            For Each CTL In onForm.Controls
                CTL.Enabled = SEL
            Next
        Catch ex As Exception
            With GLog
                .Job2 = "��ʖ������^�L����"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Sub

    '
    ' �@�@�\ : �Y��ComboBox�̍��ڐݒ�
    '
    ' �߂�l : �Ȃ� >����(Integer 0:���� 1:�t�@�C���Ȃ� 2:���s)2009/09/10 T-Sakai �ǉ�
    '
    ' ������ : ARG1 - ComboBox Object
    ' �@�@�@   ARG2 - �t�@�C����
    ' �@�@�@   ARG3 - �C���f�b�N�X���ŏ��̈ʒu�ɐݒ肷��
    ' �@�@�@   ARG4 - �t�@�C���̓ǂݕ�
    '
    ' ���@�l : �R���{�{�b�N�X���ʊ֐�
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

                Return 1 '�t�@�C���Ȃ� 2009/09/10 T-Sakai �ǉ�
            End If
            Return 0 '����I�� 2009/09/10 T-Sakai�ǉ�
        Catch ex As Exception
            With GLog
                .Job2 = "�Y��ComboBox�̍��ڐݒ�"
                .Result = NG
                .Discription = aComboBox.Name & " : " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            Return 2 '�ُ�I�� 2009/09/10 T-Sakai�ǉ�
        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Function

    '
    ' �@�@�\ : �Y��ComboBox�̍��ڐݒ�
    '
    ' �߂�l : �Ȃ� >����(Integer 0:���� 1:�t�@�C���Ȃ� 2:���s)2009/09/10 T-Sakai �ǉ� 
    '
    ' ������ : ARG1 - ComboBox Object
    ' �@�@�@   ARG2 - �t�@�C����
    ' �@�@�@   ARG3 - �C���f�b�N�X���ŏ��̈ʒu�ɐݒ肷��
    ' �@�@�@   ARG4 - �t�@�C���̓ǂݕ�
    '
    ' ���@�l : �R���{�{�b�N�X���ʊ֐�
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

                Return 1 '�t�@�C���Ȃ� 2009/09/10 T-Sakai �ǉ�
            End If
            Return 0 '����I�� 2009/09/10 T-Sakai�ǉ�
        Catch ex As Exception
            With GLog
                .Job2 = "�Y��ComboBox�̍��ڐݒ�"
                .Result = NG
                .Discription = aComboBox.Name & " : " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            Return 2 '�ُ�I�� 2009/09/10 T-Sakai�ǉ�
        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Function

    '
    ' �@�@�\ : �Y��INDEX�̍��ڎQ��
    '
    ' �߂�l : �ŏ��̐��l�f�[�^
    '
    ' ������ : ARG1 - ComboBox Object
    '
    ' ���@�l : �R���{�{�b�N�X���ʊ֐�
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
                .Job2 = "�Y��INDEX�̍��ڎQ��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Function

    '
    ' �@�@�\ : �R���{�{�b�N�X����C�K�v�̂Ȃ��A�C�e�����폜����
    '
    ' ������ : ARG1 - �R���{�{�b�N�X
    '          ARG2 - ��r�z��i������ԍ���o�^�j
    '
    ' ���@�l : ��r�z��Ɉ�v���Ȃ��A�C�e�����폜����
    '   
    Public Sub RemoveComboItem(ByVal combo As ComboBox, ByVal dataarray As Array)
        Dim RemoveArray As New ArrayList        ' �폜���X�g

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
    ' �@�@�\ : �w�蒷���̕������Ԃ�
    '
    ' �߂�l : ������f�[�^
    '
    ' ������ : ARG1 - �]���Ώےl
    ' �@�@�@   ARG2 - �w�蒷(Byte)
    '
    ' ���@�l : ����������̕�����]��
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
                .Job2 = "�w�蒷���̕������Ԃ�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            Return ""
        End Try
    End Function

    '
    ' �@�@�\ : TextBox�̕�����l��]������
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �]���Ώۃe�L�X�g�{�b�N�X
    '
    ' ���@�l : ���p���͗̈�ɗp����
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
                .Job2 = "TextBox�̕�����l��]������"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        Finally
            avTextBox.Text = Ret
        End Try
    End Sub

    '
    ' �@�@�\ : TextBox�̐����l��]������
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �]���Ώےl
    ' �@�@�@   ARG2 - Format ����^���Ȃ�
    '
    ' ���@�l : ��ʂŎg�p
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
                .Job2 = "TextBox�̐����l��]������"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Sub

    '
    ' �@�@�\ : �����l��]������
    '
    ' �߂�l : �����l
    '
    ' ������ : ARG1 - �]���Ώےl
    '
    ' ���@�l : �e�L�X�g�����n�֐�
    '    
    Public Function NzInt(ByVal avValue As Object) As Integer
        Dim Ret As Integer = 0
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) AndAlso IsNumber(avValue) Then

                Ret = CType(avValue, Integer)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "�����l��]������"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : �����l�]��
    '
    ' �߂�l : �]����̐����l
    '
    ' ������ : ARG1 - �]���Ώےl
    ' �@�@�@   ARG2 - ����
    '
    ' ���@�l : ��ʌn��p�֐�
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
                .Job2 = "�����l�]��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : �������l��]������
    '
    ' �߂�l : �������l
    '
    ' ������ : ARG1 - �]���Ώےl
    '
    ' ���@�l : �e�L�X�g�����n�֐�
    '    
    Public Function NzLong(ByVal avValue As Object) As Long
        Dim Ret As Long = 0
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) AndAlso IsNumber(avValue) Then

                Ret = CType(avValue, Long)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "�������l"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : �������l�]��
    '
    ' �߂�l : �]����̒������l
    '
    ' ������ : ARG1 - �]���Ώےl
    ' �@�@�@   ARG2 - ����
    '
    ' ���@�l : ��ʌn��p�֐�
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
                .Job2 = "�������l�]��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : ���l��]������
    '
    ' �߂�l : ���l
    '
    ' ������ : ARG1 - �]���Ώےl
    '
    ' ���@�l : �e�L�X�g�����n�֐�
    '    
    Public Function NzDec(ByVal avValue As Object) As Decimal
        Dim Ret As Decimal = 0
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) AndAlso IsNumber(avValue) Then

                Ret = CType(avValue, Decimal)
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "���l��]������"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : ���l�]��
    '
    ' �߂�l : �]����̐��l
    '
    ' ������ : ARG1 - �]���Ώےl
    ' �@�@�@   ARG2 - ����
    '
    ' ���@�l : ��ʌn��p�֐�
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
                .Job2 = "���l�]��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : ���l�]��
    '
    ' �߂�l : �]����̕�����^�̐��l���
    '
    ' ������ : ARG1 - �]���Ώےl
    ' �@�@�@   ARG2 - ����
    '
    ' ���@�l : ��ʌn��p�֐�
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
                .Job2 = "���l�]��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : ������l��]������
    '
    ' �߂�l : ������l
    '
    ' ������ : ARG1 - �]���Ώےl
    '
    ' ���@�l : �ėp�I�Ɏg��
    '    
    Public Function NzStr(ByVal avValue As Object) As String
        Dim Ret As String = Space$(0)
        Try
            If Not IsDBNull(avValue) AndAlso Not IsNothing(avValue) Then

                Ret = avValue.ToString
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "������l��]������"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�@�\ : �����ꂽ������Ńt�B���^��������
    '
    ' �߂�l : ������l
    '
    ' ������ : ARG1 - ���Ώە������
    ' �@�@�@   ARG2 - �]���Ώےl
    '
    ' ���@�l : �ėp�I�Ɏg��(��ɗX�֔ԍ�)
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
                .Job2 = "�����ꂽ������Ńt�B���^��������"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
        Return Ret
    End Function

    '
    ' �@�\�@ �F �v�����^���݊m�F
    '
    ' �����@ �F ARG1 - ���W���[���W��
    '
    ' �߂�l �F �v�����^�L��
    '
    ' ���l�@ �F ����n�ŕK�v�ƂȂ�֐��i�b��I�Ɋi�[����j2007.10.05 By K.Seto
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
            Dim MSG As String = "���p�\�ȃv�����^�[������܂���B"
            MSG &= " �F " & ex.Message
            With GLog
                .Job2 = "�v�����^���݊m�F"
                .Result = NG
                .Discription = MSG
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            Return 0
        End Try
    End Function

    '
    ' �@�\�@ �F ��������
    '
    ' �����@ �F ARG1 - �Ώےl
    ' �@�@�@ �@ ARG2 - �����㌅��
    ' �@�@�@ �@ ARG3 - �X�y�[�X(���p�^�S�p)
    ' �@�@�@ �@ ARG4 - ���l����(���E)
    '
    ' �߂�l �F ���o���R�[�h��
    '
    ' ���l�@ �F �t�H�[�����ϐ��֐ݒ�
    ' �@�@�@ �@ ����n�ŕK�v�ƂȂ�֐��i�b��I�Ɋi�[����j2007.10.05 By K.Seto
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
                            SetCol &= "�@"
                        End If
                    Else
                        SetCol = " " & SetCol
                    End If
                Loop
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "��������"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Function

    '
    ' �@�\�@ �F ���l�]��
    '
    ' �����@ �F ARG1 - �Ώےl
    '
    ' �߂�l �F ���l = True
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Function IsNumber(ByVal avValue As Object) As Boolean
        Try
            Return (New System.Text.RegularExpressions.Regex("^[-]*\d+$")).IsMatch(avValue.ToString)

        Catch ex As Exception
            With GLog
                .Job2 = "���l�]��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
    End Function

    '
    ' �@�@�\ : �S��w�蕶���`�F�b�N
    '
    ' �߂�l : �S���K�蕶���̏ꍇ �@= True
    ' �@�@�@   �K��O����������ꍇ = False
    '
    ' ������ : ARG1 - �]���Ώە�����
    '
    ' ���@�l : �ϊ��o���镶���͕ϊ����Ă��܂�
    '    
    Public Function CheckZenginChar(ByVal avTextBox As TextBox) As Boolean
        Dim BRet As Boolean = True
        Try
            Dim Chars() As Char = StrConv(avTextBox.Text, VbStrConv.Narrow).ToUpper.ToCharArray()
            Dim GetString As String = ""
            For Each C As Char In Chars
                Select Case C.ToString
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "�"c
                    Case "�"
                        C = "-"c
                    Case "A" To "Z"
                    Case "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", _
                         "�", "�", "�", "�", "�", _
                         "�", "�", "�", _
                         "�", "�", "�", "�", "�", _
                         "�", "�", "�"
                    Case "�", "�", " "
                    Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                        '*** �C�� mitsu 2009/04/17 *&$�͋K��O���� ***
                    Case "\", ",", ".", "�", "�", "-", "/", "(", ")"
                        '*********************************************
                    Case Else
                        '�G���[����
                        BRet = False
                End Select
                '�`�F�b�N�ϕ������~��
                GetString = GetString + C.ToString
            Next
            'TextBox�l���㏑������
            avTextBox.Text = GetString
        Catch ex As Exception
            BRet = False
            With GLog
                .Job2 = "�S��w�蕶���`�F�b�N"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try
        '���ʕԋp
        Return BRet
    End Function

    '--------------------------------------------------------------------------------------------------
    '' �f�[�^�x�[�X����֘A�֐��Q
    '--------------------------------------------------------------------------------------------------

    '
    ' �@�\�@�@ : �J�[�\�����`������
    '
    ' �߂�l�@ : ORACLE Reader Object
    '
    ' �������@ : ARG1 - SQL String
    '
    ' ���l�@�@ : ���p����
    '
    Public Function SetDynaset(ByVal SQL As String, ByRef oraReader As OracleDataReader) As Boolean
        Dim BRet As Boolean = False

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = MainLog.Write_Enter3("clsCommon.SetDynaset")
            If IS_LEVEL4 = True Then
                MainLog.Write_LEVEL4("clsCommon.SetDynaset", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        If IS_SQLLOG = True Then
            MainLog.Write_SQL("clsCommon.SetDynaset", SQL)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            If Not GCOM_MainDB Is Nothing Then
                oraReader = GCOM_MainDB.getOracleDataReader(SQL)
                BRet = oraReader.HasRows

            Else
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

                If ConnectDataBase() Then

                    If Tran Is Nothing Then
                        oraReader = New OracleClient.OracleCommand(SQL, OraConnect).ExecuteReader
                    Else
                        oraReader = New OracleClient.OracleCommand(SQL, OraConnect, Tran).ExecuteReader
                    End If

                    BRet = oraReader.HasRows
                End If

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

        Catch ex As OracleException
            '*** Str Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader.Dispose()
                oraReader = Nothing
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***

            With GLog
                .Job2 = ex.Message.Replace(",", " ")
                .Result = NG
                .Discription = SQL.Replace(",", ":")
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True), 0) '2010/01/13 �R�����g��

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_SQL_Err("clsCommon.SetDynaset", SQL)
            MainLog.Write_Err("clsCommon.SetDynaset", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader.Dispose()
                oraReader = Nothing
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***

            BRet = False
            With GLog
                .Job2 = "�J�[�\���`��"
                .Result = NG
                .Discription = ex.Message.Replace(",", " ")
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_SQL_Err("clsCommon.SetDynaset", SQL)
            MainLog.Write_Err("clsCommon.SetDynaset", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        End Try

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        If IS_LEVEL3 = True Then
            MainLog.Write_Exit3(sw, "clsCommon.SetDynaset", "���A�l=" & BRet)
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Return BRet
    End Function

    '
    ' �@�@�\ : �f�[�^�x�[�X����
    '
    ' �߂�l : OK           ����I��
    '          NG           �ُ�I��
    '          ��DB_Execute �̏ꍇ�͎������R�[�h����Ԃ�
    '
    ' ���@�� : onProcess      ���䎯��
    ' �@�@�@   SQL            SQL��(Optional) 
    '          SQLCode        �X�e�[�^�X(Optional)
    '          LogOut         ���O�o�͉�(Optional)
    '
    ' ���@�l : onProcess = Public Enum OraModule
    '                         DB_Connect   = 1      DB�ڑ�
    '                         DB_Begin     = 2      �g�����U�N�V�����J�n
    '                         DB_Execute   = 4      SQL���̎��s
    '                         DB_Commit    = 8      �R�~�b�g
    '                         DB_Rollback  = 16     ���[���o�b�N
    '                         DB_Terminate = 32     DB�ؒf
    '                      End Enum
    '
    Public Function DBExecuteProcess(ByVal onProcess As Integer, Optional ByVal SQL As String = "", _
            Optional ByRef SQLCode As Integer = 0, Optional ByVal LogOut As Boolean = True) As Integer
        Dim Ret As Integer = 0

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If IS_LEVEL3 = True Then
            sw = MainLog.Write_Enter3("clsCommon.DBExecuteProcess", "onProcess=" & onProcess)
            If IS_LEVEL4 = True Then
                MainLog.Write_LEVEL4("clsCommon.DBExecuteProcess", "�ďo", "�ďo���X�^�b�N: " & Environment.StackTrace)
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Try
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            If GCOM_MainDB Is Nothing Then
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                'ORACLE�ڑ��m�F
                If Not ConnectDataBase() Then
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If IS_LEVEL3 = True Then
                        MainLog.Write_Exit3(sw, "clsCommon.DBExecuteProcess", "���A�l=-9")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    'ORACLE �ڑ��s��
                    Return -9
                End If
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

            If (onProcess And enDB.DB_Begin) = enDB.DB_Begin Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Begin")
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                If Not GCOM_MainDB Is Nothing Then
                    GCOM_MainDB.BeginTrans()

                Else
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

                    '�g�����U�N�V�����̊J�n
                    Tran = OraConnect.BeginTransaction

                    Command.Transaction = Tran

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            End If

            If (onProcess And enDB.DB_Execute) = enDB.DB_Execute Then

                Try
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If IS_LEVEL3 = True Then
                        MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Execute")
                    End If
                    If IS_SQLLOG = True Then
                        MainLog.Write_SQL("clsCommon.DBExecuteProcess", SQL)
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    If Not GCOM_MainDB Is Nothing Then
                        Ret = GCOM_MainDB.ExecuteNonQuery(SQL)

                    Else
                    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

                        'SQL���̎��s
                        Command.CommandText = SQL

                        Ret = Command.ExecuteNonQuery

                    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

                    If LogOut AndAlso SQLCode = 0 Then
                        With GLog
                            .Job2 = SQL.Substring(0, 6).ToUpper
                            .Result = OK
                            .Discription = "�e������=" & Ret & ": "
                        End With
                        'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
                    End If
                Catch ex As OracleException
                    SQLCode = ex.Code
                    If LogOut OrElse SQLCode > 1 Then
                        With GLog
                            .Job2 = ex.Message.Replace(",", "")
                            .Result = NG
                            .Discription = SQL.Replace(",", ":")
                        End With
                        'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True), 0) '2010/01/13 �R�����g��
                    End If

                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    MainLog.Write_SQL_Err("clsCommon.DBExecuteProcess", SQL)
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '*** Str Add 2016/01/21 SO)�r�� for ���ݏ�Q�i��O�������Ɍďo�����Ő��폈�����s����j ***
                    Throw
                    '*** End Add 2016/01/21 SO)�r�� for ���ݏ�Q�i��O�������Ɍďo�����Ő��폈�����s����j ***

                Catch ex As Exception
                    If LogOut Then
                        With GLog
                            .Job2 = SQL.Substring(0, 6).ToUpper
                            .Result = NG
                            .Discription = ex.Message.Replace(",", " ")
                        End With
                        'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
                    End If

                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    MainLog.Write_SQL_Err("clsCommon.DBExecuteProcess", SQL)
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    '*** Str Add 2016/01/21 SO)�r�� for ���ݏ�Q�i��O�������Ɍďo�����Ő��폈�����s����j ***
                    Throw
                    '*** End Add 2016/01/21 SO)�r�� for ���ݏ�Q�i��O�������Ɍďo�����Ő��폈�����s����j ***

                End Try
            End If

            If (onProcess And enDB.DB_Commit) = enDB.DB_Commit Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Commit")
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                If Not GCOM_MainDB Is Nothing Then
                    GCOM_MainDB.Commit()

                Else
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

                    '�g�����U�N�V�������R�~�b�g
                    '*** Str Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***
                    'Tran.Commit()
                    If Not Tran Is Nothing Then
                        Tran.Commit()
                        Tran = Nothing
                    End If
                    '*** End Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            End If

            If (onProcess And enDB.DB_Rollback) = enDB.DB_Rollback Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Rollback")
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                If Not GCOM_MainDB Is Nothing Then
                    GCOM_MainDB.Rollback()

                Else
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

                    '���[���o�b�N
                    '*** Str Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***
                    'Tran.Rollback()
                    If Not Tran Is Nothing Then
                        Tran.Rollback()
                        Tran = Nothing
                    End If
                    '*** End Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i�g�����I�����Rollback�ďo���ŗ�O�����j ***

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            End If

            If (onProcess And enDB.DB_Terminate) = enDB.DB_Terminate Then
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL3 = True Then
                    MainLog.Write_LEVEL3("clsCommon.DBExecuteProcess", "DB_Terminate")
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                If Not GCOM_MainDB Is Nothing Then
                    'GCOM_MainDB.Close()
                    'GCOM_MainDB = Nothing

                Else
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***

                    '�f�[�^�x�[�X�ڑ������
                    OraConnect.Close()

                    Tran = Nothing
                    OraConnect.Dispose()
                    OraConnect = Nothing

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��iDB�R�l�N�V�����̈�{���j ***
            End If

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            If IS_LEVEL3 = True Then
                MainLog.Write_Exit3(sw, "clsCommon.DBExecuteProcess", "���A�l=" & Ret)
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Return Ret
        Catch ex As Exception
            With GLog
                .Job2 = "�f�[�^�x�[�X����"
                .Result = NG
                .Discription = ex.Message.Replace(",", " ")
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("clsCommon.DBExecuteProcess", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            '*** Str Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i��O�������Ɍďo�����Ő��폈�����s����j ***
            'Return -9
            Throw
            '*** End Upd 2016/01/21 SO)�r�� for ���ݏ�Q�i��O�������Ɍďo�����Ő��폈�����s����j ***

        End Try
    End Function

    '
    ' �@�\�@�@ : ORACLE�֐ڑ�����
    '
    ' �߂�l�@ : ���� = True
    ' �@�@�@�@   ���s = False
    '
    ' �������@ : ARG1 - �Ȃ�
    '
    ' ���l�@�@ : �Ȃ�
    '
    Private Function ConnectDataBase() As Boolean
        Try
            If OraConnect Is Nothing OrElse Not OraConnect.State = ConnectionState.Open Then

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                Dim sw As System.Diagnostics.Stopwatch = Nothing
                If IS_LEVEL4 = True Then
                    sw = MainLog.Write_Enter4("clsCommon.ConnectDataBase")
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                OraConnect = New OracleClient.OracleConnection

                OraConnect.ConnectionString = CASTCommon.DB.CONNECT

                OraConnect.Open()

                Command = OraConnect.CreateCommand

                If OraConnect.State = ConnectionState.Open Then
                    'With GLog
                    '    .Job2 = "ORACLE�ڑ�"
                    '    .Result = OK
                    '    .Discription = ""
                    'End With
                    'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                Else
                    With GLog
                        .Job2 = "ORACLE�ڑ�"
                        .Result = NG
                        .Discription = ""
                    End With
                    'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��

                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    If IS_LEVEL4 = True Then
                        MainLog.Write_Exit4(sw, "clsCommon.ConnectDataBase", "���A�l=False")
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                    Return False
                End If

                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                If IS_LEVEL4 = True Then
                    MainLog.Write_Exit4(sw, "clsCommon.ConnectDataBase", "���A�l=True")
                End If
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***
            End If

            Return True
        Catch ex As OracleException
            With GLog
                .Job2 = ex.Message
                .Result = NG
                .Discription = "ORACLE.CONNECT"
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True), 0) '2010/01/13 �R�����g��

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("clsCommon.ConnectDataBase",ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            OraConnect = Nothing
            Return False
        Catch ex As Exception
            With GLog
                .Job2 = "ORACLE�ڑ�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            MainLog.Write_Err("clsCommon.ConnectDataBase", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            OraConnect = Nothing
            Return False
        End Try
    End Function

    '
    ' �@�\�@�@�@: ���g�̒[���ԍ����Q�Ƃ���
    '
    ' �߂�l�@�@: �[���ԍ�
    '
    ' �������@�@: ARG1 - �Ȃ�
    '
    ' ���l�@�@�@: �Ȃ�
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
                .Job2 = "���g�̒[���ԍ��Q��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return "NULL"
    End Function

    '
    ' �@�\�@�@�@: ��s�����Q�Ƃ���
    '
    ' �߂�l�@�@: 0 = OK, 1 = Bank Error, 2 = Branch Error
    '
    ' �������@�@: ARG1 - Bank Code
    ' �@�@�@ �@�@ ARG2 - Branch Code
    '
    ' ���l�@�@�@: �Ȃ�
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
                .Job2 = "��s���Q��"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function

    '
    ' �@�\�@ �F ���Z�@�֏�񌟍�
    '
    ' �����@ �F ARG1 - ���Z�@�փR�[�h
    ' �@�@�@ �@ ARG2 - �x�X�R�[�h
    ' �@�@�@ �@ ARG3 - �����o�C�g��
    '
    ' �߂�l �F ���Z�@�֖��^�x�X��
    '
    ' ���l�@ �F �Ȃ�
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
                    '�x�X��
                    If avShort = 0 Then
                        Return NzStr(REC.Item("SIT_NNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("SIT_NNAME_N").ToString, avShort)
                    End If
                Else
                    '���Z�@�֖�
                    If avShort = 0 Then
                        Return NzStr(REC.Item("KIN_NNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("KIN_NNAME_N").ToString, avShort)
                    End If
                End If
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "���Z�@�֏�񌟍�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
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
    ' �@�\�@ �F ���Z�@�֏�񌟍�(�J�i��)
    '
    ' �����@ �F ARG1 - ���Z�@�փR�[�h
    ' �@�@�@ �@ ARG2 - �x�X�R�[�h
    ' �@�@�@ �@ ARG3 - �����o�C�g��
    '
    ' �߂�l �F ���Z�@�֖��^�x�X��
    '
    ' ���l�@ �F 2009/09/29 �ǉ�
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
                    '�x�X��
                    If avShort = 0 Then
                        Return NzStr(REC.Item("SIT_KNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("SIT_KNAME_N").ToString, avShort)
                    End If
                Else
                    '���Z�@�֖�
                    If avShort = 0 Then
                        Return NzStr(REC.Item("KIN_KNAME_N")).Trim
                    Else
                        Return GetLimitString(REC.Item("KIN_KNAME_N").ToString, avShort)
                    End If
                End If
            End If
        Catch ex As Exception
            With GLog
                .Job2 = "���Z�@�֏�񌟍�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            Return ""
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Function
    '--------------------------------------------------------------------------------------------------
    '' �a��ϊ��֐� 2007.10.29 By K.Seto
    '--------------------------------------------------------------------------------------------------
    '
    ' �@�\�@ �F ����a�
    '
    ' �����@ �F ARG1 - �N�����z��
    ' �@�@�@ �@ ARG2 - �ԊҌ㕶����
    ' �@�@�@ �@ ARG3 - Optional �j���v��
    ' �@�@�@ �@ ARG4 - Optional ������t
    '
    ' �߂�l �F -1 ����A0=�N����A1=������A2=������ 
    '
    ' ���l�@ �F �Ȃ�
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
                        aonString = onDate.ToString("ggy�NMM��dd��", Culture)
                    Case Else
                        aonString = onDate.ToString("ggy�NMM��dd��(dddd)", Culture)
                End Select
            Else
                Return 0
            End If
        Catch ex As Exception
            onDate = BadResultDate
            With GLog
                .Job2 = "����a�"
                .Result = NG
                .Discription = ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�\�@ �F �a����
    '
    ' �����@ �F ARG1 - �a�����
    '
    ' �߂�l �F ������� 
    '
    ' ���l�@ �F �Ȃ�
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
                    Case "�N", "��", "��"
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
                .Job2 = "�a����"
                .Result = NG
                .Discription = aonString & " �� " & Tmp2 & " �F " & ex.Message
            End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
        End Try

        Return Ret
    End Function

    '
    ' �@�\�@ �F ��ʉE�ド�x���ʒu�ݒ�
    '
    ' �����@ �F ARG1 - ���O�C���� �F    (Label)
    ' �@�@�@ �@ ARG2 - �V�X�e�����t �F  (Label)
    ' �@�@�@ �@ ARG3 - ���[�U�h�c       (Label)
    ' �@�@�@ �@ ARG4 - �V�X�e�����t     (Label)
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F �Ȃ�
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
            'lblDate.Text = String.Format("{0:yyyy�NMM��dd��}", GetSysDate)
            Label2.Location = New Point(580 + AddLeng, 8)
            Label3.Location = New Point(580 + AddLeng, 28)
            lblUser.Location = New Point(665 + AddLeng, 8)
            lblDate.Location = New Point(665 + AddLeng, 28)
            lblUser.Text = GetUserID
            lblDate.Text = String.Format("{0:yyyy�NMM��dd��}", Date.Parse("2021/02/17"))
            '======================================================
        Catch ex As Exception
            '2009/09/23 =================================================
            'With GLog
            '    .Job2 = "��ʉE�ド�x���ʒu�ݒ�"
            '    .Result = NG
            '    .Discription = ex.Message
            'End With
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            lblUser.Text = "SYSTEM ERROR"
            lblDate.Text = "SYSTEM ERROR"
            '============================================================
        End Try
    End Sub

    '*** �C�� mitsu 2008/05/23 �X�P�W���[���̌����A���z�Čv�Z ***
    '
    ' �@�@�\ : �e�X�P�W���[���}�X�^�̐U�֍ό����E���z�̍Čv�Z
    '
    ' �����@ : ARG1 - �����敪
    ' �@�@�@   ARG2 - ������R�[�h
    ' �@�@�@   ARG3 - ����敛�R�[�h
    ' �@�@�@   ARG4 - �U�֓�or�U����
    ' �@�@�@   ARG5 - ����SEQ(�ȗ���)
    '
    ' �߂�l : 0�ȏ� = OK, -1 = NG
    '
    ' ���@�l : ���d�l�ł͑��U�͏������Ȃ�
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
                '���U�͕s�\�t���O�������Ă���ꍇ�̂ݍX�V����
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            Case "3"
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
                '���U�͍X�V���Ȃ�(�v�d�l����)
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
                '�s�\�����A���z�擾
                '----------------------
                SQL.Length = 0
                SQL.Append("SELECT ")
                SQL.Append(" COUNT(FURIKIN_K) KEN")
                SQL.Append(",SUM(FURIKIN_K)   KIN")
                SQL.Append(" FROM " & MEIMAST)
                SQL.Append(" WHERE ")
                SQL.Append("     FURI_DATE_K = " & SQ(FURI_DATE))
                If FMT_KBN = "02" Then
                    ' ���ł́A�f�[�^�敪���R�̃��R�[�h
                    SQL.Append(" AND DATA_KBN_K = '3'")
                Else
                    SQL.Append(" AND DATA_KBN_K = '2'")
                End If

                SQL.Append(" AND FURIKETU_CODE_K <> '0'")
                ' 2008.03.14 �U�֋��z���O�~�̂��̂͊܂܂Ȃ�
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
                '�U�֍ό����A���z�擾
                '----------------------
                SQL = SQL.Replace("FURIKETU_CODE_K <> '0'", "FURIKETU_CODE_K = '0'")

                If TotalReader.DataReader(SQL) = True Then
                    ZumiKensu = TotalReader.GetInt64("KEN")
                    ZumiKingaku = TotalReader.GetInt64("KIN")
                End If
                TotalReader.Close()

                '-------------------------------------------
                '�X�P�W���[���}�X�^�̍X�V
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
                '�w�Z�X�P�W���[���}�X�^�̍X�V
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
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            MainDB.Rollback()
            nRet = -1
        End Try

        Return nRet
    End Function

    '
    ' �@�@�\ : �e���s�X�P�W���[���}�X�^�̐U�֍ό����E���z�̍Čv�Z
    '
    ' �����@ : ARG1 - �����敪
    ' �@�@�@   ARG2 - ������R�[�h
    ' �@�@�@   ARG3 - ����敛�R�[�h
    ' �@�@�@   ARG4 - �U�֓�or�U����
    ' �@�@�@   ARG5 - ���Z�@�փR�[�h(�ȗ���)
    '
    ' �߂�l : 0�ȏ� = OK, -1 = NG
    '
    ' ���@�l : ���d�l�ł͑��U�͏������Ȃ�
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
        '�s�\�t���O�������Ă���ꍇ�̂ݍX�V����
        SQL.Append(" AND FUNOU_FLG_U = '1'")
        '���Z�@�փR�[�h�w�莞
        If TKIN_NO <> "" Then
            SQL.Append(" AND (TKIN_NO_U = " & SQ(TKIN_NO))
            'SSS�̏ꍇ�͋��Z�@�փR�[�h���w�肵�Ȃ�
            SQL.Append(" OR FMT_KBN_T IN ('20','21'))")
        End If

        Try
            If OraReader.DataReader(SQL) = True Then
                Dim BAITAI_CODE As String = OraReader.GetString("BAITAI_CODE_T")
                Dim FMT_KBN As String = OraReader.GetString("FMT_KBN_T")
                Dim TotalReader As New MyOracleReader(MainDB)
                '���s�X�P�W���[���}�X�^�̃��R�[�h���ɍX�V
                While OraReader.EOF = False
                    TKIN_NO = OraReader.GetString("TKIN_NO_U")
                    'Dim TEIKEI_KBN As String = OraReader.GetString("TEIKEI_KBN_U") '2010/01/13 �R�����g��

                    Dim FunoKensu As Long = 0
                    Dim FunoKingaku As Long = 0
                    Dim ZumiKensu As Long = 0
                    Dim ZumiKingaku As Long = 0

                    '----------------------
                    '�s�\�����A���z�擾
                    '----------------------
                    SQL.Length = 0
                    SQL.Append("SELECT ")
                    SQL.Append(" COUNT(FURIKIN_K) KEN")
                    SQL.Append(",SUM(FURIKIN_K)   KIN")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE ")
                    SQL.Append("     FURI_DATE_K = " & SQ(FURI_DATE))
                    If FMT_KBN = "02" Then
                        ' ���ł́A�f�[�^�敪���R�̃��R�[�h
                        SQL.Append(" AND DATA_KBN_K = '3'")
                    Else
                        SQL.Append(" AND DATA_KBN_K = '2'")
                    End If

                    SQL.Append(" AND FURIKETU_CODE_K <> '0'")
                    ' 2008.03.14 �U�֋��z���O�~�̂��̂͊܂܂Ȃ�
                    SQL.Append(" AND FURIKIN_K > 0")
                    SQL.Append(" AND TORIS_CODE_K = " & SQ(TORIS_CODE))
                    SQL.Append(" AND TORIF_CODE_K = " & SQ(TORIF_CODE))
                    Select Case FMT_KBN
                        'SSS�̏ꍇ
                        Case "20", "21"
                            '���s���͑ΏۊO�Ƃ���
                            SQL.Append(" AND KEIYAKU_KIN_K <> " & SQ(JIKINKOCD))
                            '2010/01/13 �R�����g��
                            'If TEIKEI_KBN = "1" Then
                            '    ' ��g��
                            '    SQL.Append(" AND EXISTS (")
                            'Else
                            '    ' ��g�O
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
                    '�U�֍ό����A���z�擾
                    '----------------------
                    SQL = SQL.Replace("FURIKETU_CODE_K <> '0'", "FURIKETU_CODE_K = '0'")

                    If TotalReader.DataReader(SQL) = True Then
                        ZumiKensu = TotalReader.GetInt64("KEN")
                        ZumiKingaku = TotalReader.GetInt64("KIN")
                    End If
                    TotalReader.Close()

                    '-------------------------------------------
                    '���s�X�P�W���[���}�X�^�̍X�V
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
                        'SSS�̏ꍇ
                        Case "20", "21"
                            'SQL.Append("   AND TEIKEI_KBN_U = " & SQ(TEIKEI_KBN))  '2010/01/13 �R�����g��
                        Case Else
                            SQL.Append("   AND TKIN_NO_U = " & SQ(TKIN_NO))
                    End Select

                    nRet = MainDB.ExecuteNonQuery(SQL)

                    '-------------------------------------------
                    '�w�Z���s�X�P�W���[���}�X�^�̍X�V
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
            'Call FN_LOG_WRITE(ThisModuleName, New StackTrace(True)) '2010/01/13 �R�����g��
            MainDB.Rollback()
            nRet = -1
        End Try

        Return nRet
    End Function
    '************************************************************

    '
    ' �@�\�@�@�@: �����R���{�{�b�N�X�ݒ�֐�
    '
    ' �߂�l�@�@: 0 = OK, -1 = Err
    '
    ' �������@�@: ARG1 - �擪�J�i����
    ' �@�@�@ �@�@ ARG2 - �R���{�{�b�N�X��
    '             ARG3 - ������R�[�h�e�L�X�g�{�b�N�X
    '             ARG4 - ����敛�R�[�h�e�L�X�g�{�b�N�X
    '             ARG5 - �����敪(1:���U 3:���U)
    '             ARG6 - �}�̃R�[�h
    ' ���l�@�@�@: T-Sakai�ǉ�
    '
    Public Function SelectItakuName(ByVal KanaKey As String, ByVal Cmbbox As ComboBox, _
                                    ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_F As TextBox, _
                                    Optional ByVal FSYORI_KBN As String = "1", Optional ByVal Jyoken As String = "") As Integer

        '--------------------------------------------
        '�ϑ��Җ����J�iKEY�l�Ŏn�܂�ϑ��Җ��J�i������
        '--------------------------------------------
        Dim Ret As Integer = -1
        Dim REC As OracleDataReader = Nothing
        Dim Toris_Code As String
        Dim Torif_Code As String
        Dim Itaku_NName As String
        Try
            '-----------------------------------------
            '���݂̃R���{�{�b�N�X���N���A�ɂ���
            '-----------------------------------------
            Cmbbox.Text = ""
            TORIS_CODE_T.Text = ""
            TORIF_CODE_F.Text = ""
            Cmbbox.Items.Clear()

            If String.IsNullOrEmpty(KanaKey) Then
                KanaKey = ""
            End If

            '2014/02/18 saitou �W���� ���쐫���� UPD -------------------------------------------------->>>>
            '�K�v�ȍ��ڂ̂ݎ擾����B
            Dim SQL As String = "SELECT TORIS_CODE_T, TORIF_CODE_T, ITAKU_NNAME_T FROM"
            'Dim SQL As String = "SELECT * FROM"
            '2014/02/18 saitou �W���� ���쐫���� UPD --------------------------------------------------<<<<
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
            '�R���{�{�b�N�X�Ƀ��X�g��ǉ�����
            '-----------------------------------------
            If SetDynaset(SQL, REC) Then
                Do While REC.Read
                    Toris_Code = NzStr(REC.Item("TORIS_CODE_T")).Trim
                    Torif_Code = NzStr(REC.Item("TORIF_CODE_T")).Trim
                    Itaku_NName = NzStr(REC.Item("ITAKU_NNAME_T")).Trim
                    '2013/12/27 saitou ���Z�M�� MODIFY ----------------------------------------------->>>>
                    '15�����Ő؂�̂���߂�
                    Cmbbox.Items.Add(Toris_Code & " - " & Torif_Code & " " & Itaku_NName.Trim)
                    'Cmbbox.Items.Add(Toris_Code & " - " & Torif_Code & "  " & Mid(Itaku_NName.Trim, 1, 15))
                    '2013/12/27 saitou ���Z�M�� MODIFY -----------------------------------------------<<<<
                Loop

            '*** Str Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***
            ElseIf REC Is Nothing Then
                Return -1
            '*** End Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***

            End If
            '-----------------------------------------
            '�R���{�{�b�N�X��1�ԍŏ��̃��X�g��\������
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
    ' �@�\�@�@�@: �����R���{�{�b�N�X(VIEW�p)�ݒ�֐�
    '
    ' �߂�l�@�@: 0 = OK, -1 = Err
    '
    ' �������@�@: ARG1 - �擪�J�i����
    ' �@�@�@ �@�@ ARG2 - �R���{�{�b�N�X��
    '             ARG3 - ������R�[�h�e�L�X�g�{�b�N�X
    '             ARG4 - ����敛�R�[�h�e�L�X�g�{�b�N�X
    '             ARG5 - �����敪(1:���U 3:���U)
    '             ARG6 - �}�̃R�[�h
    ' ���l�@�@�@: T-Sakai�ǉ�
    '
    Public Function SelectItakuName_View(ByVal KanaKey As String, ByVal Cmbbox As ComboBox, _
                                    ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_F As TextBox, _
                                    Optional ByVal FSYORI_KBN As String = "1", Optional ByVal Jyoken As String = "") As Integer

        '--------------------------------------------
        '�ϑ��Җ����J�iKEY�l�Ŏn�܂�ϑ��Җ��J�i������
        '--------------------------------------------
        Dim Ret As Integer = -1
        Dim REC As OracleDataReader = Nothing
        Dim Toris_Code As String
        Dim Torif_Code As String
        Dim Itaku_NName As String
        Try
            '-----------------------------------------
            '���݂̃R���{�{�b�N�X���N���A�ɂ���
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
            '�R���{�{�b�N�X�Ƀ��X�g��ǉ�����
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
            '�R���{�{�b�N�X��1�ԍŏ��̃��X�g��\������
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

    '2018/01/17 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�����V���Ή��j---------------------- START
    ''' <summary>
    ''' �����R���{�{�b�N�X�̐ݒ���s���i�e�[�u�����w��j
    ''' </summary>
    ''' <param name="KanaKey">�擪�J�i����</param>
    ''' <param name="Cmbbox">�R���{�{�b�N�X��</param>
    ''' <param name="TORIS_CODE_T">������R�[�h�e�L�X�g�{�b�N�X</param>
    ''' <param name="TORIF_CODE_F">����敛�R�[�h�e�L�X�g�{�b�N�X</param>
    ''' <param name="TABLE_NAME">�Ώۂ̎����}�X�^</param>
    ''' <param name="Jyoken">�����w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SelectItakuName_SelectTable(ByVal KanaKey As String, ByVal Cmbbox As ComboBox, _
                                    ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_F As TextBox, _
                                    ByVal TABLE_NAME As String, Optional ByVal Jyoken As String = "") As Integer

        '--------------------------------------------
        '�ϑ��Җ����J�iKEY�l�Ŏn�܂�ϑ��Җ��J�i������
        '--------------------------------------------
        Dim Ret As Integer = -1
        Dim REC As OracleDataReader = Nothing
        Dim Toris_Code As String
        Dim Torif_Code As String
        Dim Itaku_NName As String
        Dim WhereFlg As Boolean = False     'SQL��Where�傪���݂��邩

        Try
            '-----------------------------------------
            '���݂̃R���{�{�b�N�X���N���A�ɂ���
            '-----------------------------------------
            Cmbbox.Text = ""
            TORIS_CODE_T.Text = ""
            TORIF_CODE_F.Text = ""
            Cmbbox.Items.Clear()

            If String.IsNullOrEmpty(KanaKey) Then
                KanaKey = ""
            End If

            '�K�v�ȍ��ڂ̂ݎ擾����B
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
            '�R���{�{�b�N�X�Ƀ��X�g��ǉ�����
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
            '�R���{�{�b�N�X��1�ԍŏ��̃��X�g��\������
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
    '2018/01/17 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�����V���Ή��j---------------------- END

    '
    ' �@�\�@�@�@: �����R���{�{�b�N�X�ݒ�֐�
    '
    ' �߂�l�@�@: 0 = OK, -1 = Err
    '
    ' �������@�@: ARG1 - �擪�J�i����
    ' �@�@�@ �@�@ ARG2 - �R���{�{�b�N�X��
    '             ARG3 - ������R�[�h�e�L�X�g�{�b�N�X
    '             ARG4 - ����敛�R�[�h�e�L�X�g�{�b�N�X
    '             ARG5 - �����敪(1:���U 3:���U)
    '             ARG6 - �}�̃R�[�h
    ' ���l�@�@�@: T-Sakai�ǉ�
    Public Function Set_TORI_CODE(ByVal Cmbbox As ComboBox, ByVal TORIS_CODE_T As TextBox, ByVal TORIF_CODE_T As TextBox) As Integer
        Dim Ret As Integer = -1
        Try
            '-----------------------------------------
            '���݂̃e�L�X�g�{�b�N�X���N���A�ɂ���
            '-----------------------------------------
            TORIS_CODE_T.Text = ""
            TORIF_CODE_T.Text = ""

            '-----------------------------------------------------------------------
            '�R���{�{�b�N�X���烊�X�g���擾���A�擾�l���e�L�X�g�{�b�N�X�ɐݒ肷��
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
    ' �@�\�@�@�@: �����`�F�b�N�֐�(�G���g���Ŏg�p)
    '
    ' �߂�l�@�@: 0:�ُ�Ȃ� 1:�������� 2:���U�_�񖳂� 3:�������� -1:�ُ� (�K�v�ł���Ώ����ǉ�)
    '
    ' �������@�@: ARG1 - �x�X�R�[�h
    ' �@�@�@ �@�@ ARG2 - �ȖڃR�[�h�i2���j
    '             ARG3 - �����ԍ�
    '             ARG4 - ��ƃR�[�h
    '             ARG5 - �U�փR�[�h
    '             ARG6 - �_��҃J�i����
    '             ARG7 - �G���[���b�Z�[�W
    ' ���l�@�@�@: 
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
            '20130712 maeda ����������C��
            SQL.Append(" ORDER BY KATU_KOUZA_D ")
            '20130712 maeda ����������C��

            If OraReader.DataReader(SQL) = True Then
                Do
                    KokyakuName = NzStr(OraReader.GetString("KOKYAKU_KNAME_D"))
                    KatuKouzaD = NzStr(OraReader.GetString("KATU_KOUZA_D"))

                    If FuriCode.Trim = NzStr(OraReader.GetString("FURI_CODE_D")).Trim AndAlso _
                       KigyoCode.Trim = NzStr(OraReader.GetString("KIGYOU_CODE_D")).Trim AndAlso _
                       NzStr(OraReader.GetString("KATU_KOUZA_D")) = "1" Then
                        ErrMsg = ""
                        Ret = 0    '��������
                        Exit Try
                    End If
                Loop Until OraReader.NextRead = False
                '2011/06/16 �W���ŏC�� �������`�F�b�N�ǉ� ------------------START
                If KatuKouzaD = "0" Then
                    ErrMsg = "��������"
                    Ret = 3 '��������
                    Exit Try
                End If
                '2011/06/16 �W���ŏC�� �������`�F�b�N�ǉ� ------------------END

                ErrMsg = "���U�_�񖳂�"
                Ret = 2 '���U�_�񖳂�
                Exit Try
            End If

            ErrMsg = "��������"
            Ret = 1 '��������
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        '20130717 �ڊǃ`�F�b�N�ǉ� maeda
        If Ret = 1 OrElse Ret = 3 Then
            OraReader = New MyOracleReader(MainDB)

            Try
                SQL.Length = 0
                SQL.Append("SELECT *")
                SQL.Append(" FROM KDBMAST")
                SQL.Append(" WHERE OLD_TSIT_NO_D = " & SQ(SitCode))
                SQL.Append(" AND KAMOKU_D = " & SQ(Kamoku))
                SQL.Append(" AND OLD_KOUZA_D = " & SQ(Kouza))
                '20130712 maeda ����������C��
                SQL.Append(" ORDER BY KATU_KOUZA_D ")
                '20130712 maeda ����������C��

                KatuKouzaD = "0"

                If OraReader.DataReader(SQL) = True Then
                    Do
                        KokyakuName = NzStr(OraReader.GetString("KOKYAKU_KNAME_D"))
                        KatuKouzaD = NzStr(OraReader.GetString("KATU_KOUZA_D"))

                        If FuriCode.Trim = NzStr(OraReader.GetString("FURI_CODE_D")).Trim AndAlso _
                           KigyoCode.Trim = NzStr(OraReader.GetString("KIGYOU_CODE_D")).Trim AndAlso _
                           NzStr(OraReader.GetString("KATU_KOUZA_D")) = "1" Then
                            ErrMsg = "�ڊǍ�"
                            Ret = 6    '��������
                            Exit Try
                        End If
                    Loop Until OraReader.NextRead = False
                    '2011/06/16 �W���ŏC�� �������`�F�b�N�ǉ� ------------------START
                    If KatuKouzaD = "0" Then
                        ErrMsg = "(�ڊ�)����"
                        Ret = 5 '��������
                        Exit Try
                    End If
                    '2011/06/16 �W���ŏC�� �������`�F�b�N�ǉ� ------------------END

                    ErrMsg = "(�ڊ�)���U�_��"
                    Ret = 4 '���U�_�񖳂�
                    Exit Try
                End If

            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return -1
            Finally
                If Not OraReader Is Nothing Then OraReader.Close()
            End Try
        End If
        '20130717 �ڊǃ`�F�b�N�ǉ� maeda

        Return Ret
    End Function

    ''' <summary>
    ''' �`�F�b�N�f�W�b�g�֐�(�G���g���Ŏg�p)
    ''' </summary>
    ''' <param name="KinCode">���ɃR�[�h</param>
    ''' <param name="SitCode">�x�X�R�[�h</param>
    ''' <param name="Kamoku">�Ȗځi2���j</param>
    ''' <param name="Kouza">�����ԍ�</param>
    ''' <returns>True or False</returns>
    ''' <remarks>ClsFUSION���ڍs</remarks>
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
                    '�`�F�b�N�f�W�b�g�ΏۊO
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
    ''' �U���萔���ID�R���{�{�b�N�X�ݒ�
    ''' </summary>
    ''' <param name="aComboBox">�R���{�{�b�N�X�I�u�W�F�N�g</param>
    ''' <param name="aTaxID">�ŗ�ID</param>
    ''' <param name="aItemData">�����ݒ�C���f�b�N�X</param>
    ''' <param name="aFSyoriKbn">�����敪(���U:1(�f�t�H���g), ���U:3)</param>
    ''' <returns>0 - ����, 2 - �ُ�</returns>
    ''' <remarks>2013/11/27 �W���� ����őΉ� ADD</remarks>
    Public Function SetComboBox_TESUU_TABLE_ID_T(ByVal aComboBox As ComboBox, _
                                                 ByVal aTaxID As String, _
                                                 ByVal aItemData As Integer, _
                                                 Optional ByVal aFSyoriKbn As String = "1") As Integer

        '--------------------------------------------------
        '�ŗ�ID�ɕR�t���U���萔�����擾
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
                '���U�̏ꍇ�͐U���萔���Ɍ���
                SQL &= " AND SYUBETU_C = '10'"
            End If
            SQL &= " ORDER BY TESUU_TABLE_ID_C"

            '-----------------------------------------
            '�R���{�{�b�N�X�Ƀ��X�g��ǉ�����
            '-----------------------------------------

            '�U���萔����ݒ肵�Ȃ��󔒌Œ�p�^�[��
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

            '*** Str Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***
            ElseIf REC Is Nothing Then
                Return 2
            '*** End Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***

            End If

            Return 0
        Catch ex As Exception
            With GLog
                .Job2 = "�Y��ComboBox�̍��ڐݒ�"
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
    ''' �U���萔�������R���{�{�b�N�X��ݒ肵�܂��B
    ''' </summary>
    ''' <param name="CmbBox">�U���萔�������R���{�{�b�N�X</param>
    ''' <param name="txtTAX_ID">�ŗ�ID�e�L�X�g�{�b�N�X</param>
    ''' <param name="txtTESUU_TABLE_ID">�萔��ID�e�L�X�g�{�b�N�X</param>
    ''' <param name="FSYORI_KBN">�����敪(1:���U , 3:���U)</param>
    ''' <returns>0:���� , -1:�ُ�</returns>
    ''' <remarks>2013/12/02 ����őΉ� ADD</remarks>
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
            '���݂̃R���{�{�b�N�X���N���A�ɂ���
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
                SQL &= " AND SYUBETU_C = '10'"      '���U�͐U���萔������
            End If
            SQL &= " ORDER BY TAX_ID_C, TESUU_TABLE_ID_C"

            Console.WriteLine(SQL)
            CmbBox.Items.Add(Space(50))

            '--------------------------------------------------
            '�R���{�{�b�N�X�ɒǉ�����
            '--------------------------------------------------
            If SetDynaset(SQL, REC) Then
                Do While REC.Read
                    strTaxID = NzStr(REC.Item("TAX_ID_C")).Trim
                    strTesuuTableID = NzStr(REC.Item("TESUU_TABLE_ID_C")).Trim
                    strTesuuTableName = NzStr(REC.Item("TESUU_TABLE_NAME_C")).Trim
                    CmbBox.Items.Add(strTaxID & " - " & strTesuuTableID & "�@" & Mid(strTesuuTableName.Trim, 1, 15))
                Loop

            '*** Str Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***
            ElseIf REC Is Nothing Then
                Return -1
            '*** End Add 2015/12/01 SO)�r�� for ���ݏ�Q�iSQL�G���[���ɉ�ʑ��ŃG���[���b�Z�[�W���o�Ȃ��j ***

            End If

            '--------------------------------------------------
            '�R���{�{�b�N�X��1�ԍŏ��̃��X�g��\������
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
    ''' �U���萔���R���{�{�b�N�X�ݒ�֐�
    ''' </summary>
    ''' <param name="CmbBox">�U���萔�������R���{�{�b�N�X</param>
    ''' <param name="txtTAX_ID">�ŗ�ID�e�L�X�g�{�b�N�X</param>
    ''' <param name="txtTESUU_TABLE_ID">�萔��ID�e�L�X�g�{�b�N�X</param>
    ''' <returns>0:����, -1:�ُ�</returns>
    ''' <remarks>2013/12/02 ����őΉ� ADD</remarks>
    Public Function Set_TESUU_CODE(ByVal CmbBox As ComboBox, _
                                   ByVal txtTAX_ID As TextBox, _
                                   ByVal txtTESUU_TABLE_ID As TextBox) As Integer
        Dim Ret As Integer = -1
        Try
            '--------------------------------------------------
            '���݂̃e�L�X�g�{�b�N�X���N���A�ɂ���
            '--------------------------------------------------
            txtTAX_ID.Text = ""
            txtTESUU_TABLE_ID.Text = ""

            '----------------------------------------------------------------------
            '�R���{�{�b�N�X���烊�X�g���擾���A�擾�l���e�L�X�g�{�b�N�X�ɐݒ肷��
            '----------------------------------------------------------------------
            Dim strTesuuLine As String
            strTesuuLine = CmbBox.SelectedItem.ToString
            Dim strTesuuItem1() As String = strTesuuLine.Split("-"c)
            If strTesuuItem1.Length >= 2 Then
                txtTAX_ID.Text = strTesuuItem1(0).Trim
                Dim strTesuuItem2() As String = strTesuuItem1(1).Split("�@"c)
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

    '2017/04/26 �^�X�N�j���� ADD �W���ŏC���i�\�[�g��INI���Ή��j------------------------------------ START
    ''' <summary>
    ''' �w�肵����ʁ^���[�̌ʏ�����INI�t�@�C�����擾����
    ''' </summary>
    ''' <param name="OBJ_ID">��ʁ^���[ID</param>
    ''' <param name="KEY_NAME">���ږ�</param>
    ''' <param name="MODE">0:��ʁA1:���[</param>
    ''' <returns>�\�[�g�L�[</returns>
    ''' <remarks></remarks>
    Public Function GetObjectParam(ByVal OBJ_ID As String, ByVal KEY_NAME As String, ByVal MODE As String) As String
        Dim INI_KEY As String = ""

        Select Case MODE
            Case "0"    '���
                INI_KEY = "FORM"
            Case "1"    '���[
                INI_KEY = "PRINT"
            Case Else   '��L�ȊO�͔�����
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
    '2017/04/26 �^�X�N�j���� ADD �W���ŏC���i�\�[�g��INI���Ή��j------------------------------------ END
    '2017/04/26 �^�X�N�j���� ADD �W���ŏC���iFrom-To��INI���j------------------------------------ START
    ''' <summary>
    ''' �w�肵����ʂŎg�p������Ԃ�From-To���擾����
    ''' </summary>
    ''' <param name="FORM_ID">���ID</param>
    ''' <param name="RET_FROM">���c�Ɠ��O����ԋp����</param>
    ''' <param name="RET_TO">���c�Ɠ��ォ��ԋp����</param>
    ''' <param name="DEF_FROM">INI�t�@�C������̎擾�G���[���̕ԋp�l</param>
    ''' <param name="DEF_TO">INI�t�@�C������̎擾�G���[���̕ԋp�l</param>
    ''' <remarks>INI�t�@�C������擾�ł��Ȃ��ꍇ�́ADEF_FROM/DEF_TO�̒l��ԋp���܂�</remarks>
    Public Sub GetFromTo(ByVal FORM_ID As String, ByRef RET_FROM As Integer, ByRef RET_TO As Integer, ByVal DEF_FROM As Integer, ByVal DEF_TO As Integer)
        Dim wkFROM As String = GetRSKJIni("FROM_TO", FORM_ID & "_FROM")
        Dim wkTO As String = GetRSKJIni("FROM_TO", FORM_ID & "_TO")

        'FROM�̐ݒ�
        Select Case wkFROM
            Case "err", ""
                RET_FROM = DEF_FROM
            Case Else
                RET_FROM = CInt(wkFROM)
        End Select

        'TO�̐ݒ�
        Select Case wkTO
            Case "err", ""
                RET_TO = DEF_TO
            Case Else
                RET_TO = CInt(wkTO)
        End Select

    End Sub
    '2017/04/26 �^�X�N�j���� ADD �W���ŏC���iFrom-To��INI���j------------------------------------ END

End Class

'--------------------------------------------------------------------------------------------------
'' �e��ʂ̃R���{�{�b�N�X�i�[�l����ŕK�v�ƂȂ�֐� 2007.10.05 By K.Seto
'--------------------------------------------------------------------------------------------------
Public Class clsAddItem
    '
    ' �@�@�\ : �R���{�{�b�N�X����N���X
    '
    ' ���@�l : �ݒ�^�Q�ƂɎg�p����
    '    
    Public Item As String           '�\���e�L�X�g
    Public Data1 As Integer         '�L���l(Number)
    Public Data2 As Integer         '�L���l(Number)
    Public Data3 As String          '�L���l(String)

    Public Sub New(ByVal aItem As String, ByVal aData1 As Integer, _
        Optional ByVal aData2 As Integer = 0, Optional ByVal aData3 As String = "")
        Item = aItem
        Data1 = aData1
        Data2 = aData2
        Data3 = aData3
    End Sub

    '
    ' �@�@�\ : �Y��INDEX�̍��ڏ��Q��
    '
    ' �߂�l : �Y��INDEX�̍��ڏ��
    '
    ' ������ : ARG1 - �Ȃ�
    '
    ' ���@�l : �R���{�{�b�N�X���ʊ֐�
    '    
    Public Overrides Function ToString() As String
        Return Item
    End Function


End Class
