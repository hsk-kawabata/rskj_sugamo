Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Collections.Generic
Imports CASTCommon

Public Class ClsKessaiDataCreate

    Private Const FD_COUNT_LIMIT As Integer = 5000 '���G���^�e�c�P��������̍ő匏�����Z�b�g����

    Public MainLOG As New CASTCommon.BatchLOG("KFS060", "�ב֐������G���^�쐬")

    Dim MainDB As CASTCommon.MyOracle

    Private strKEKKA As String              ' �f�[�^�쐬����

    Private jobMessage As String = ""          ' �W���u�Ď����b�Z�[�W

    ' �������t
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ''' <summary>
    ''' ini�t�@�C�����
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni

        Dim JIKINKO_CODE As String       '�����ɃR�[�h
        Dim JIKINKO_NAME As String       '�����ɖ�
        Dim HONBU_CODE As String         '�{���R�[�h
        Dim RIENTA_PATH As String        '���G���^�t�@�C���쐬��
        Dim DAT_PATH As String           'DAT�̃p�X
        Dim RIENTA_FILENAME As String   '���G���^�t�@�C����

    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' ���U���σ}�X�^�̃L�[���ځ{���G���^�t�@�C����
    ''' </summary>
    ''' <remarks></remarks>
    Structure kesmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' ������
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private keskey As kesmastKey

    Private ParaKessaiDate As String        '�p�����[�^��������p�������ϓ�
    Private KessaiList As New List(Of CAstFormKes.ClsFormKes.KessaiData) '�������σf�[�^�i�[�p
    Private HONBU_KNAME As String = ""

    Private Structure KeyInfo
        '���M�f�[�^�쐬�����[�b�r�u�f�[�^�쐬���p
        Dim TORIS_CODE As String            ' ������R�[�h
        Dim TORIF_CODE As String            ' ����敛�R�[�h
        Dim FURI_DATE As String             ' �U����
        Dim ITAKU_CODE As String            ' �ϑ��҃R�[�h
        Dim ITAKU_KNAME As String           ' �ϑ��Җ��J�i
        Dim TORIMATOME_SIT As String        ' ���܂ƂߓX
        Dim SIT_KNAME As String             ' ���܂ƂߓX��
        Dim KESSAI_KBN As String            ' ���ϋ敪
        Dim KESSAI_PATN As String           ' �����m�ە��@
        Dim BAITAI_CODE As String           ' �}�̃R�[�h

        Dim FURI_KEN As String              ' �U���ό���
        Dim FURI_KIN As String              ' �U���ϋ��z

        Dim MESSAGE As String

        ' ������
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            FURI_DATE = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            TORIMATOME_SIT = ""
            SIT_KNAME = ""
            KESSAI_KBN = ""
            KESSAI_PATN = ""
            BAITAI_CODE = ""

            FURI_KEN = ""
            FURI_KIN = ""

            MESSAGE = ""
        End Sub

        ' �c�a����̒l��ݒ�i�ב֐������G���^�쐬�p�j
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_S")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            SIT_KNAME = oraReader.GetString("SIT_KNAME_N")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_T")
            KESSAI_PATN = oraReader.GetString("KESSAI_PATN_T")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")

            FURI_KEN = oraReader.GetString("FURI_KEN_S")
            FURI_KIN = oraReader.GetString("FURI_KIN_S")
        End Sub

    End Structure

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' �ב֐������G���^�쐬��������
    ''' </summary>
    ''' <returns>����:True �ُ�:False</returns>
    ''' <remarks></remarks>
    Public Function KessaiInit(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try
            '�p�����[�^�̓Ǎ�
            param = CmdArgs(0).Split(","c)
            If param.Length = 2 Then

                '���O�����ݏ��̐ݒ�
                MainLOG.FuriDate = param(0)                     '���ϓ��Z�b�g
                MainLOG.JobTuuban = CType(param(1), Integer)
                MainLOG.ToriCode = "000000000000"
                '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                MainLOG.Write_LEVEL1("(��������)�J�n", "����")
                'MainLOG.Write("(��������)�J�n", "����")
                '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***


                ParaKessaiDate = param(0)                       '���ϓ����Z�b�g

            Else
                '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                MainLOG.Write_LEVEL1("(��������)�J�n", "���s", "�R�}���h���C�������̃p�����[�^���s���ł�")
                'MainLOG.Write("(��������)�J�n", "���s", "�R�}���h���C�������̃p�����[�^���s���ł�")
                '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
                Return False

            End If

            'ini�t�@�C���̓Ǎ�
            If IniRead() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("(��������)�J�n", "���s", ex.Message)
            'MainLOG.Write("(��������)�J�n", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            Return False
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("(��������)�I��", "����")
            'MainLOG.Write("(��������)�I��", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        End Try

    End Function

    Private Function IniRead() As Boolean

        ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")           '�����ɃR�[�h
        If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD")
            'MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD"
            Return False
        End If

        ini_info.JIKINKO_NAME = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")       '�����ɖ�
        If ini_info.JIKINKO_NAME = "err" OrElse ini_info.JIKINKO_NAME = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɖ� ����:COMMON ����:KINKONAME")
            'MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɖ� ����:COMMON ����:KINKONAME")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�����ɖ� ����:COMMON ����:KINKONAME"
            Return False
        End If

        ini_info.HONBU_CODE = CASTCommon.GetFSKJIni("COMMON", "HONBUCD")         '�{���R�[�h
        If ini_info.HONBU_CODE = "err" OrElse ini_info.HONBU_CODE = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�{���R�[�h ����:COMMON ����:HONBUCD")
            'MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�{���R�[�h ����:COMMON ����:HONBUCD")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�{���R�[�h ����:COMMON ����:HONBUCD"
            Return False
        End If

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        '���G���^�t�@�C���쐬��
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR")
            'MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DAT�̃p�X
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT")
            'MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT"
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")       '���G���^�t�@�C����
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�������σ��G���^�t�@�C���� ����:KESSAI ����:RIENTANAME")
            'MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�������σ��G���^�t�@�C���� ����:KESSAI ����:RIENTANAME")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�������σ��G���^�t�@�C���� ����:KESSAI ����:RIENTANAME"
            Return False
        End If

        Return True

    End Function

    ' �@�\�@ �F �ב֐������G���^�쐬���� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Public Function Main(ByVal command As String) As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

        MainDB = New CASTCommon.MyOracle
        Dim bRet As Boolean = True
        Dim iRet As Integer

        ' �p�����[�^�`�F�b�N
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "�ב֐������G���^�쐬����(�J�n)", "����")
            'MainLOG.Write("(�又��)�J�n", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***


            MainDB.BeginTrans()     ' �g�����U�N�V�����J�n

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("(�又��)", "���s", "�ב֐������G���^�쐬�����Ŏ��s�҂��^�C���A�E�g")
                MainLOG.UpdateJOBMASTbyErr("�ב֐������G���^�쐬�����Ŏ��s�҂��^�C���A�E�g")
                Return -1
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '*******************************
            ' �񎟂��擾
            '*******************************
            If GetKaiji() = False Then
                '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                MainLOG.Write_LEVEL1("(�又��)", "���s", "�񎟂̎擾�Ɏ��s���܂���")
                'MainLOG.Write("(�又��)", "���s", "�񎟂̎擾�Ɏ��s���܂���")
                '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

                Return -1
            End If

            '*****************************************
            ' �������σf�[�^�̊i�[�ƃX�P�W���[���̍X�V
            '*****************************************
            iRet = MakeKessaiData()
            Select Case iRet
                Case 0          ' �f�[�^�i�[����
                    bRet = True
                Case 1          ' �Ώۃf�[�^�O��
                    bRet = True
                Case Else       ' �f�[�^�i�[���s
                    bRet = False
            End Select

            '***********************
            ' ���G���^FD�쐬
            '***********************
            Dim msgtitle As String = "�ב֐������G���^�쐬(KFS060)"
            Dim FDCnt As Integer = 1 'FD����

            If iRet = 0 AndAlso KessaiList.Count > 0 Then

                If MakeRientaFD(FDCnt) = False Then
                    jobMessage = "�ב֐������G���^�쐬���s"
                    iRet = -1
                Else

                    For i As Integer = 1 To FDCnt
                        If i > 1 Then
                            MessageBox.Show(String.Format(MSG0500I, FD_COUNT_LIMIT, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                            Windows.Forms.MessageBoxDefaultButton.Button1, Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
                        End If

                        Do
                            Try

                                Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                                Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                                iRet = 0
                                Exit Do

                            Catch ex As Exception
                                If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                    iRet = -1
                                    jobMessage = "FD�}�����L�����Z������܂����B"
                                    '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                                    MainLOG.Write_LEVEL1("FD�v��", "���s", "FD�}�����L�����Z��")
                                    'MainLOG.Write("FD�v��", "���s", "FD�}�����L�����Z��")
                                    '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

                                    Exit Do
                                End If
                            End Try
                        Loop

                        Select Case iRet
                            Case 0          ' �f�[�^�i�[����
                                If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME)) Then
                                    If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                        jobMessage = "�t���b�s�[���t�@�C���폜�L�����Z��"
                                        iRet = -1
                                    End If
                                End If

                                If iRet = 0 Then
                                    File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME & i), Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME), True)
                                End If
                        End Select
                    Next

                End If

            End If

            If iRet <> 0 Then
                bRet = False
            End If

            '*******************************
            ' ���[�o��
            '*******************************
            ' �������σf�[�^���P���ȏ㑶�݂���ꍇ�A���[�o��
            If iRet = 0 Then
            End If

            If bRet = False Then
                If jobMessage = "" Then
                    Call MainLOG.UpdateJOBMASTbyErr("���O�Q��")
                Else
                    Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                End If

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                ' ���[���o�b�N
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "�Ώۃf�[�^�O��"
                End If

                Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

                ' �R�~�b�g
                MainDB.Commit()
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("(�又��)", "���s", ex.ToString)
            'MainLOG.Write("(�又��)", "���s", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            Return 1
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            If Not MainDB Is Nothing Then
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)

                ' ���[���o�b�N
                MainDB.Rollback()
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "�ב֐������G���^�쐬����(�I��)", "����")
            'MainLOG.Write("(�又��)�I��", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        End Try

        Return 0

    End Function

    ' �@�\�@ �F �������σf�[�^�쐬����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '

    Private Function MakeKessaiData() As Integer

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = New StringBuilder(256)

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("(�������σf�[�^�i�[)�J�n", "����")
            'MainLOG.Write("(�������σf�[�^�i�[)�J�n", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***


            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.AppendLine("SELECT")
            SQL.AppendLine(" MAX(TORIS_CODE_S) TORIS_CODE_S")
            SQL.AppendLine(",MAX(TORIF_CODE_S) TORIF_CODE_S")
            SQL.AppendLine(",MAX(FURI_DATE_S) FURI_DATE_S")
            SQL.AppendLine(",MAX(SYUBETU_S) SYUBETU_S")
            SQL.AppendLine(",MAX(ITAKU_CODE_S) ITAKU_CODE_S")
            SQL.AppendLine(",SUM(FURI_KEN_S) FURI_KEN_S")
            SQL.AppendLine(",SUM(FURI_KIN_S) FURI_KIN_S")

            SQL.AppendLine(",MAX(ITAKU_KNAME_T) ITAKU_KNAME_T")
            SQL.AppendLine(",MAX(TORIMATOME_SIT_T) TORIMATOME_SIT_T")
            SQL.AppendLine(",MAX(KESSAI_KBN_T) KESSAI_KBN_T")
            SQL.AppendLine(",MAX(KESSAI_PATN_T) KESSAI_PATN_T")

            SQL.AppendLine(",MAX(KIN_KNAME_N) KIN_KNAME_N")
            SQL.AppendLine(",MAX(SIT_KNAME_N) SIT_KNAME_N")
            SQL.AppendLine(",MAX(BAITAI_CODE_S) BAITAI_CODE_S")
            SQL.AppendLine(" FROM S_TORIMAST")
            SQL.AppendLine("     ,S_SCHMAST")
            SQL.AppendLine("     ,TENMAST")

            SQL.AppendLine(" WHERE KESSAI_YDATE_S = " & SQ(ParaKessaiDate))
            SQL.AppendLine(" AND KESSAI_FLG_S = '0'")
            SQL.AppendLine(" AND HASSIN_FLG_S = '1'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            SQL.AppendLine(" AND FURI_KIN_S > 0")
            SQL.AppendLine(" AND KESSAI_KBN_T <> '99'")
            SQL.AppendLine(" AND TORIS_CODE_S   = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S   = TORIF_CODE_T")
            SQL.AppendLine(" AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
            SQL.AppendLine(" AND TORIMATOME_SIT_T = SIT_NO_N(+)")
            SQL.AppendLine(" GROUP BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")
            SQL.AppendLine(" ORDER BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")

            Dim Key As KeyInfo = Nothing
            Dim test As String = SQL.ToString

            If OraReader.DataReader(SQL) = True Then

                Dim lstKessaiData As New List(Of CAstFormKes.ClsFormKes.KessaiData)

                ' �L�[������
                Key.Init()

                ' �{���X���擾
                HONBU_KNAME = GetTenmast()

                ' �ŏ��̃L�[�ݒ�
                Call Key.SetOraDataKessai(OraReader)

                Do While OraReader.EOF = False
                    lstKessaiData.Clear()

                    ' �������σf�[�^�擾����(�����U���p)
                    If fn_GetKessaiData(Key, lstKessaiData) = False Then
                        Return -1
                    End If

                    If Not (lstKessaiData Is Nothing OrElse lstKessaiData.Count = 0) Then
                        ' �擾�����������σf�[�^����ɁA���U���σ}�X�^�o�^���s��
                        For i As Integer = 0 To lstKessaiData.Count - 1
                            Dim KData As CAstFormKes.ClsFormKes.KessaiData = lstKessaiData.Item(i)

                            ' ���U���σ}�X�^�̓o�^����
                            If InsertKessaiMast(Key, KData) = False Then
                                jobMessage = "���U���σ}�X�^�o�^���s ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & _
                                             " �U�����F" & Key.FURI_DATE
                                Return -1
                            End If
                        Next
                    End If

                    ' �X�P�W���[���}�X�^�̍X�V���� 
                    If UpdateSchMast(Key) = False Then
                        jobMessage = "�X�P�W���[���}�X�^�X�V���s ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & _
                                     " �U�����F" & Key.FURI_DATE
                        Return -1
                    End If

                    ' �Ώۃf�[�^�̎����R�[�h��Ǎ���
                    OraReader.NextRead()

                    If OraReader.EOF = False Then
                        ' �L�[�ݒ�
                        Call Key.SetOraDataKessai(OraReader)
                    End If

                    KessaiList.AddRange(lstKessaiData)
                Loop
            End If

            If KessaiList.Count = 0 Then
                '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                MainLOG.Write_LEVEL1("(�������σf�[�^�i�[)", "���s", "�����O��")
                'MainLOG.Write("(�������σf�[�^�i�[)", "���s", "�����O��")
                '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

                Return 1
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("(�������σf�[�^�i�[)", "���s", ex.Message)
            'MainLOG.Write("(�������σf�[�^�i�[)", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("(�������σf�[�^�i�[)�I��", "����")
            'MainLOG.Write("(�������σf�[�^�i�[)�I��", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
        End Try

        Return 0

    End Function


    ' �@�\�@ �F �������σf�[�^�擾����(�����U���p)
    '
    ' �����@ �F 
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_GetKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As List(Of CAstFormKes.ClsFormKes.KessaiData)) As Boolean

        Dim errFlg As Boolean = False
        Dim errMsg As String = "���Ϗ��Ɍ�肪����܂��B"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing

        strKEKKA = ""

        Try
            '�ב֐����̂�
            Select Case Key.KESSAI_KBN
                Case "00"
                    ' �ב֐����̃f�[�^�쐬
                    If errFlg = False AndAlso fn_KAWASE_SEIKYU(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                        ' �������σf�[�^�̃��X�g�쐬
                        lstKessaiData.Add(KData)
                    Else
                        errFlg = True
                    End If
                Case "99"

                Case Else
                    errFlg = True
                    errMsg &= "(���ϋ敪)"
            End Select

            If errFlg = True Then
                jobMessage = errMsg & " ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�����F" & Key.FURI_DATE & _
                    " ���ϋ敪�F" & Key.KESSAI_KBN
                '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                MainLOG.Write_LEVEL1("�������σf�[�^�擾����(�����U���p)", "���s", jobMessage)
                'MainLOG.Write("�������σf�[�^�擾����(�����U���p)", "���s", jobMessage)
                '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

                Return False
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("�������σf�[�^�擾����(�����U���p)", "���s", ex.Message)
            'MainLOG.Write("�������σf�[�^�擾����(�����U���p)", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �������σf�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_KessaiData(ByRef Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean

        Try
            ' �ʔԂ̃J�E���g�A�b�v
            keskey.RecordNo += 1
         
        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("�������σf�[�^�쐬", "���s", ex.Message)
            'MainLOG.Write("�������σf�[�^�쐬", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �ב֐����f�[�^�쐬����
    '
    ' �����@ �F
    '
    ' �߂�l �F 0 - ����C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_KAWASE_SEIKYU(ByRef key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseSeikyuInFmt As New CAstFormKes.ClsFormSikinFuri.T_48600
        Dim strKINFUKKI_FUGOU As String = ""    ' ���z���L����

        Try
            ' ������
            KawaseSeikyuInFmt.Init()

            ' ���z���L�����̎擾
            If fn_FUGO_SETTEI(CASTCommon.CADec(key.FURI_KIN).ToString("#,##0"), strKINFUKKI_FUGOU) = False Then
                '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                MainLOG.Write_LEVEL1("�ב֐����f�[�^�쐬", "���s", "���L�����ݒ菈���G���[�B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�����F" & key.FURI_DATE)
                'MainLOG.Write("�ב֐����f�[�^�쐬", "���s", "���L�����ݒ菈���G���[�B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�����F" & key.FURI_DATE)
                '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
                Return -1
            End If

            '�f�[�^�ݒ�
            With KawaseSeikyuInFmt
                .TORIATUKAI = ParaKessaiDate
                .SYUMOKU = "4701"                                   ' ��ڃR�[�h
                .JUSIN_TEN = "� " & key.SIT_KNAME                   ' ��M�X��
                .FUKA_CODE = "000"                                  ' �t���R�[�h
                .HASSIN_TEN = "� " & HONBU_KNAME                    ' ���M�X��
                .KINGAKU = key.FURI_KIN.ToString.PadLeft(10)        ' ���z
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = " "                                   ' ���ω�
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.PadRight(15, " "c) ' ���z���L����
                .BANGOU = ""                                        ' �ԍ�
                .SIKIN_JIYUU1 = "�ײ��" & key.ITAKU_KNAME.Trim & "���"  ' �����t�֗��R
                .SIKIN_JIYUU2 = key.FURI_KEN.Trim & "��"            ' �����t�֗��R�Q
                .BIKOU1 = ""                                        ' ���l�P
                .BIKOU2 = ""                                        ' ���l�Q
                .SYOKAI_NO = ""                                     ' �Ɖ�ԍ�
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = KawaseSeikyuInFmt.Data
            KData.OpeCode = String.Concat(KawaseSeikyuInFmt.KAMOKU_CODE, KawaseSeikyuInFmt.OPE_CODE)
            KData.TorimatomeSit = key.TORIMATOME_SIT

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("�ב֐����f�[�^�쐬", "���s", ex.Message)
            'MainLOG.Write("�ב֐����f�[�^�쐬", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            Return -1
        End Try

        Return 0

    End Function

    ' �@�\�@ �F ���L�����ݒ菈��
    '
    ' �����@ �F astrKEY1:�ϊ��O���z�i�J���}�ҏW�ς݁j
    '           astrKEY2
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F �p�����^�ł킽���ꂽ���z�����ƂɂP�T�P�^�̕��L������Ԃ�
    '
    Private Function fn_FUGO_SETTEI(ByVal astrKEY1 As String, ByRef astrKEY2 As String) As Boolean
        Dim intCount As Integer     '������
        Dim strASSYUKU As String    '���k
        Dim strFUGO(14) As String   '����
        Dim I As Integer

        Try
            astrKEY2 = ""
            strASSYUKU = "Y"

            For intCount = 0 To astrKEY1.Length - 1

                strFUGO(intCount) = " "

                Select Case astrKEY1.Substring(intCount, 1)
                    Case "0"
                        If strASSYUKU = "Y" Then
                            strFUGO(intCount) = " "
                        Else
                            strFUGO(intCount) = "�"
                        End If
                    Case "1"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "2"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "3"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "4"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "5"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "6"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "7"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "8"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "9"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case ","
                        strFUGO(intCount) = " "
                End Select
            Next

            For I = 0 To strFUGO.Length - 1
                astrKEY2 = astrKEY2 & strFUGO(I)
            Next

            astrKEY2 = astrKEY2.Trim

        Catch ex As Exception

            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("���L�����ݒ菈��", "���s", ex.Message)
            'MainLOG.Write("���L�����ݒ菈��", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F ���U���σ}�X�^�o�^
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertKessaiMast(ByVal Key As KeyInfo, ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("INSERT INTO S_KESSAIMAST(")
            SQL.AppendLine(" SYORI_DATE_KR")
            SQL.AppendLine(",TIME_STAMP_KR")
            SQL.AppendLine(",KAIJI_KR")
            SQL.AppendLine(",RECORD_NO_KR")
            SQL.AppendLine(",FILE_NAME_KR")
            SQL.AppendLine(",TORIS_CODE_KR")
            SQL.AppendLine(",TORIF_CODE_KR")
            SQL.AppendLine(",FURI_DATE_KR")
            SQL.AppendLine(",MOTIKOMI_SEQ_KR")
            SQL.AppendLine(",KAMOKU_CODE_KR")
            SQL.AppendLine(",OPE_CODE_KR")
            SQL.AppendLine(",DENBUN_ALL_KR")
            SQL.AppendLine(",ERR_CODE_KR")
            SQL.AppendLine(",ERR_MSG_KR")
            SQL.AppendLine(",SAKUSEI_DATE_KR")
            SQL.AppendLine(",KOUSIN_DATE_KR")
            SQL.AppendLine(") VALUES (")
            SQL.AppendLine(" " & SQ(strDate))                                   ' ������
            SQL.AppendLine("," & SQ(String.Concat(strDate, strTime)))           ' �^�C���X�^���v
            SQL.AppendLine("," & SQ(keskey.Kaiji))                              ' ��
            SQL.AppendLine("," & SQ(keskey.RecordNo))                           ' �ʔ�
            SQL.AppendLine("," & SQ(ini_info.RIENTA_FILENAME))                  ' ���G���^�t�@�C����
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' ������R�[�h
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' ����敛�R�[�h
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' �U����
            SQL.AppendLine(",1")                                                ' ����SEQ
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(0, 2)))             ' �ȖڃR�[�h
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(2, 3)))             ' �I�y�R�[�h
            SQL.AppendLine("," & SQ(KData.record320))                           ' �ʃf�[�^
            SQL.AppendLine("," & SQ(""))                                        ' ���ʃR�[�h
            SQL.AppendLine("," & SQ(""))                                        ' �G���[���b�Z�[�W
            SQL.AppendLine("," & SQ(strDate))                                   ' �쐬��
            SQL.AppendLine("," & SQ("00000000"))                                ' �X�V��
            SQL.AppendLine(")")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("(���U���σ}�X�^�o�^)", "���s", ex.Message)
            'MainLOG.Write("(���U���σ}�X�^�o�^)", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            Return False
        Finally
        End Try

        Return True

    End Function

    ' �@�\�@ �F �X�P�W���[���}�X�^�X�V
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateSchMast(ByVal key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("UPDATE S_SCHMAST")
            SQL.AppendLine(" SET")
            SQL.AppendLine(" KESSAI_FLG_S = '1'")
            SQL.AppendLine(",KESSAI_DATE_S = " & SQ(ParaKessaiDate))
            SQL.AppendLine(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))
            SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("�X�P�W���[���}�X�^�X�V", "���s", "������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & _
                          " �U�����F" & key.FURI_DATE & " " & ex.Message)
            'MainLOG.Write("�X�P�W���[���}�X�^�X�V", "���s", "������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & _
            '              " �U�����F" & key.FURI_DATE & " " & ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            
            Return False
        End Try

        Return True

    End Function

    Private Function MakeRientaFD(ByRef FDCnt As Integer) As Boolean
        Dim T_RIENT77 As New CAstFormKes.ClsT_RIENT77
        Dim T_RIENT10 As New CAstFormKes.ClsT_RIENT10

        Dim Kdata As CAstFormKes.ClsFormKes.KessaiData
        Dim EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")

        Dim T_48100 As New CAstFormKes.ClsFormSikinFuri.T_48100
        Dim T_48600 As New CAstFormKes.ClsFormSikinFuri.T_48600

        Dim StrmWrite As FileStream = Nothing

        Dim iLoop As Integer = 0 '����FD�ł������������Ȃ����߂̃��[�v�p�J�E���^

        Try
            ' �^���L���O�w�b�_  
            ' �����f�[�^�ݒ�
            Call T_RIENT77.TANKING_HEAD.Init()
            ' �s���t
            T_RIENT77.TANKING_HEAD.strT_HIDUKE = CASTCommon.Calendar.Now.ToString("yyyyMMdd")

            ' �X�܏�񃌃R�[�h�i�P�X�܏��j
            ' �����f�[�^�ݒ�
            Call T_RIENT77.TENPO_INFOREC(0).Init()
            ' ���ɃR�[�h
            T_RIENT77.TENPO_INFOREC(0).strKINKO_CD = ini_info.JIKINKO_CODE
            ' �X�܃R�[�h
            T_RIENT77.TENPO_INFOREC(0).strSIT_CD = ini_info.HONBU_CODE

            ' �X�܏�񃌃R�[�h�i�Q�`�R�Q�X�܏��j
            ' �����f�[�^�ݒ�
            For i As Integer = 1 To T_RIENT77.TENPO_INFOREC.Length - 1
                ' �Q�`�R�Q�̏����f�[�^�ݒ�
                Call T_RIENT77.TENPO_INFOREC(i).Init2_32()
            Next i

            ' �\���R
            ' ������
            Call T_RIENT77.DATA_SIKIBETU.Init()

            '�������p���[�v
            While iLoop < KessaiList.Count - 1 OrElse iLoop = 0
                ' ���G���^�t�@�C�� �I�[�v��
                Dim filename As String = Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME & FDCnt)

                If File.Exists(filename) = True Then
                    ' ���ɑ��݂���ꍇ�́C�폜
                    File.Delete(filename)
                End If

                StrmWrite = New FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite)

                Dim Bytes As Byte() = EncdJ.GetBytes(ini_info.RIENTA_FILENAME.PadRight(12, Nothing).PadRight(16, " "c))
                StrmWrite.Write(Bytes, 0, 16)

                ' �^���L���O�w�b�_����
                StrmWrite.Write(T_RIENT77.TANKING_HEAD.Data, 0, 28)

                ' �X�܏�񃌃R�[�h����
                For i As Integer = 0 To T_RIENT77.TENPO_INFOREC.Length - 1
                    StrmWrite.Write(T_RIENT77.TENPO_INFOREC(i).Data, 0, 28)
                Next i

                ' �\���R����
                StrmWrite.Write(T_RIENT77.DATA_SIKIBETU.Data, 0, 84)

                Dim WriteCount As Integer = 0           ' ��������

                Dim RecLen As Integer

                ' �^���L���O�f�[�^����

                Dim cnt As Integer = KessaiList.Count - 1 '���[�v��

                For i As Integer = iLoop To cnt

                    ' �^���L���O�f�[�^
                    Kdata = KessaiList.Item(i)

                    Select Case Kdata.OpeCode
                        Case "48100"
                            T_RIENT10.TANKING_DATA.Init_48()
                            T_48100.DataSepaPlus = Kdata.record320
                            'RecLen = T_48100.DataSepaPlus.Replace(" ", "").Length + 32
                            RecLen = T_48100.DataSepaPlus.Length + 32
                            T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                            T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                            T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                            T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48100.DataSepaPlus
                            T_48100 = Nothing
                        Case "48600"
                            T_RIENT10.TANKING_DATA.Init_48()
                            T_48600.DataSepaPlus = Kdata.record320
                            'RecLen = T_48600.DataSepaPlus.Replace(" ", "").Length + 32
                            RecLen = T_48600.DataSepaPlus.Length + 32
                            T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                            T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                            T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                            T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48600.DataSepaPlus
                            T_48600 = Nothing
                        Case Else
                            '�G���[
                    End Select

                    WriteCount += 1

                    ' ���z�Z�p���[�^
                    T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                    T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)

                    ' �^���L���O�A��
                    T_RIENT10.TANKING_DATA.bytTANKING_NO(0) = CType(WriteCount \ 256, Byte)
                    T_RIENT10.TANKING_DATA.bytTANKING_NO(1) = CType(WriteCount Mod 256, Byte)

                    ' ���f�[�^�A�h���X
                    If cnt + 1 = WriteCount Then
                        ' �ŏI�s
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = 255
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = 255
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = 255
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = 255
                    Else
                        Dim NextAddr As Integer = 1024 + (WriteCount * 256)

                        Dim NextAddr0 As Integer = CType(NextAddr \ 16777216, Integer)      '2010.05.08 /�@���@\
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = CType(NextAddr0, Byte)

                        Dim NextAddr1 As Integer = CType((NextAddr Mod 16777216) \ 65536, Integer)  '2010.05.08 /�@���@\
                        Dim Amari1 As Integer = CType((NextAddr Mod 16777216) Mod 65536, Integer)
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = CType(NextAddr1, Byte)

                        Dim NextAddr2 As Integer = CType(Amari1 \ 256, Integer)     '2010.05.08 /�@���@\
                        Dim Amari2 As Integer = CType(Amari1 Mod 256, Integer)
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = CType(NextAddr2, Byte)

                        Dim NextAddr3 As Integer = CType(Amari2 Mod 256, Integer)
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = CType(NextAddr3, Byte)
                    End If

                    ' ���X�R�[�h
                    T_RIENT10.TANKING_DATA.strKINTEN_CD = T_RIENT77.TENPO_INFOREC(0).strKINKO_CD & T_RIENT77.TENPO_INFOREC(0).strSIT_CD

                    '�X�y�[�X��""�ɒu���Ȃ���
                    'T_RIENT10.TANKING_DATA.strTANKING_DATA = T_RIENT10.TANKING_DATA.strTANKING_DATA.Replace(" ", "")


                    '���z�Z�p���[�^���Čv�Z���ăZ�b�g
                    'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                    'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)
                    '*****************************************
                    Select Case Kdata.OpeCode.Substring(0, 2)
                        Case "48"
                            StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_48, 0, 256)
                        Case Else
                            StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_10, 0, 256)
                    End Select

                    iLoop += 1
                    '�����݌������ő匏���ɂȂ����ꍇ�͏����݂��~�߂�
                    If iLoop >= FD_COUNT_LIMIT AndAlso iLoop Mod FD_COUNT_LIMIT = 0 Then
                        '���̃��R�[�h�����݂���ꍇ��FD�����J�E���g�A�b�v
                        If iLoop < KessaiList.Count - 1 Then
                            FDCnt += 1
                        End If

                        Exit For
                    End If
                Next

                ' �ŏI���R�[�h
                T_RIENT10.TANKING_LAST.Init()
                StrmWrite.Write(T_RIENT10.TANKING_LAST.Data, 0, 512)

                ' �^���L���O�w�b�_ �������� �ď���
                ' �S�X�܂s����
                T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)  '2010.05.08 /�@���@\
                T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
                StrmWrite.Seek(20 + 16, SeekOrigin.Begin)
                StrmWrite.Write(T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN, 0, 2)

                ' �X�܂s����
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)   '2010.05.08 /�@���@\
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
                StrmWrite.Seek(36 + 16, SeekOrigin.Begin)
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN, 0, 2)

                ' �X�܂s�I���e�c�A�h���X
                'WriteCount��1�ȊO�Ȃ�A�I���A�h���X�𐳂����ݒ肷��
                If WriteCount <> 1 Then

                    '��������-1�̒l�ŏI���A�h���X���v�Z����(���m�ɂ͍ŏI���R�[�h�̊J�n�A�h���X�̂���)
                    Dim FinishAddr As Integer = 1024 + ((WriteCount - 1) * 256)
                    'Dim FinishAddr As Integer = 1024 + (WriteCount * 256)

                    Dim FinishAddr0 As Integer = CType(FinishAddr \ 16777216, Integer)     '2010.05.08 /�@���@\
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(0) = CType(FinishAddr0, Byte)

                    Dim FinishAddr1 As Integer = CType((FinishAddr Mod 16777216) \ 65536, Integer)     '2010.05.08 /�@���@\
                    Dim FinishAmari1 As Integer = CType((FinishAddr Mod 16777216) Mod 65536, Integer)
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(1) = CType(FinishAddr1, Byte)

                    Dim FinishAddr2 As Integer = CType(FinishAmari1 \ 256, Integer)        '2010.05.08 /�@���@\
                    Dim FinishAmari2 As Integer = CType(FinishAmari1 Mod 256, Integer)
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(2) = CType(FinishAddr2, Byte)

                    Dim FinishAddr3 As Integer = CType(FinishAmari2 Mod 256, Integer)
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(3) = CType(FinishAddr3, Byte)
                End If

                StrmWrite.Seek(48 + 16, SeekOrigin.Begin)
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD, 0, 4)

                StrmWrite.Close()

            End While

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("���G���^�t�@�C���쐬", "���s", ex.Message)
            'MainLOG.Write("���G���^�t�@�C���쐬", "���s", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            Return False
        Finally
            If Not StrmWrite Is Nothing Then StrmWrite.Close()
        End Try
        Return True
    End Function

    Private Function GetKaiji() As Boolean

        Dim sql As New StringBuilder(64)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("(�񎟎擾)�J�n", "����", "")
            'MainLOG.Write("(�񎟎擾)�J�n", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***


            sql.Append("SELECT NVL(MAX(KAIJI_KR),0) AS MAX_KAIJI FROM S_KESSAIMAST")
            sql.Append(" WHERE SYORI_DATE_KR = " & SQ(strDate))

            If OraReader.DataReader(sql) = True Then
                keskey.Kaiji = CType(OraReader.GetInt64("MAX_KAIJI"), Integer) + 1
            Else
                Return False
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Err("(�񎟎擾)", "���s", ex.ToString)
            'MainLOG.Write("(�񎟎擾)", "���s", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            Return False
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_LEVEL1("(�񎟎擾)�I��", "����", "")
            'MainLOG.Write("(�񎟎擾)�I��", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return True

    End Function

    Private Function GetTenmast() As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append("SELECT SIT_KNAME_N FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(ini_info.JIKINKO_CODE))
            SQL.Append("   AND SIT_NO_N = " & SQ(ini_info.HONBU_CODE))
            If OraReader.DataReader(SQL) = True Then
                Return OraReader.GetString("SIT_KNAME_N")
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
