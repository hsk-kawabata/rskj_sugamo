Imports System
Imports System.IO
Imports CASTCommon
Imports System.Runtime.InteropServices

REM #Const DEBUGCMT = True

Public Class ClsCMTCTRL

    <VBFixedString(8)> Dim strMemMitsudo As String    ' �L�^���x
    <VBFixedString(8)> Dim strProtect As String    ' ���O�p�v���e�N�g����
    Private LOG As New CASTCommon.BatchLOG("CMT�Ǐ�����", "CMT")
    Private strLogwrite As String    ' ���O�ڍ�
    Private bit_status As DEVICE_status    ' �o�C�g�ϊ��������A�l

    '***ASTAR �u���b�N�T�C�Y�Ή� >>
    Private mnBlockSize As Integer = -1
    Public Property BlockSize() As Integer
        Get
            Return mnBlockSize
        End Get
        Set(ByVal Value As Integer)
            mnBlockSize = Value
        End Set
    End Property
    '***ASTAR �u���b�N�T�C�Y�Ή� <<

    '-------------------------------------------------------------------------------------------
    '----- �b�l�s�A�N�Z�X�p�֐���`�i�i�o�b�Ђc�k�k�p�j-----------------------------------------
    '-------------------------------------------------------------------------------------------

    ' �V���W���[���p��`
    Private Declare Function mtinit Lib "mtdll53.dll" (ByRef lpmtinf As MTINFBLOCK) As Integer
    '���u�̃X�e�[�^�X�ǂݎ��
    Private Declare Function mtstat Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�a�n�s�܂Ŋ����߂�
    Private Declare Function mtrewind Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�P�u���b�N�ǂݍ���
    Private Declare Function mtrblock Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef buff As Byte, ByRef blklen_ As Integer, ByVal bufflen_ As Long) As Integer
    '�P�u���b�N��������
    Private Declare Function mtwblock Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef buff As Byte, ByVal blklen_ As Integer) As Integer
    '�P�u���b�N�O�i
    Private Declare Function mtfblock Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�P�u���b�N���
    Private Declare Function mtbblock Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�e�[�v�}�[�N�����o����܂őO�i
    Private Declare Function mtffile Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�e�[�v�}�[�N�����o����܂Ō��
    Private Declare Function mtbfile Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�e�[�v�}�[�N�P���C�g
    Private Declare Function mtwtmk Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�e�[�v�}�[�N�Q���C�g
    Private Declare Function mtwmtmk Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�}���`�e�[�v�}�[�N�̌��o
    Private Declare Function mtsmtmk Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�e�[�v�̃A�����[�h
    Private Declare Function mtunload Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '���u�̃I�����C��
    Private Declare Function mtonline Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '�d�a�b�c�h�b�R�[�h�ϊ�
    Private Declare Function mtebc Lib "mtdll53.dll" (ByVal code_ As Byte) As Integer
    '���C�e�[�v���Ƃɂd�a�b�c�h�b�R�[�h�ϊ�
    Private Declare Function mtebcex Lib "mtdll53.dll" (ByVal code_ As Byte) As Integer
    '�l�s�̃C���[�Y
    Private Declare Function mters Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    ' �g���b�N���A�L�^���x�ύX 
    Private Declare Function mtdensity Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByVal code_ As Byte) As Integer
    ' �b�r�k����e�[�v�����[�h���� 
    'Private Declare Function mtloadcsl Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef slot_no_ As Byte, ByVal ctrlflag_ As Byte) As Integer
    ' �b�r�k����e�[�v�����[�h���� 
    'Private Declare Function mtstatcsl Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef CSLDATA As CSLDATA) As Integer

    '------------------------------------------
    ' �l�s����p
    '------------------------------------------

    ' �c�k�k���Ăяo���ׂɕK�v�Ȑ錾
    Structure MTINFBLOCK
        Public UnitNo As Byte
        Public HostNo As Byte
        Public TargetNo As Byte
        <MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.ByValTStr, SizeConst:=8), VBFixedArray(8)> Public Vender() As Char
        <MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.ByValTStr, SizeConst:=16), VBFixedArray(16)> Public Product() As Char
        <MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.ByValTStr, SizeConst:=4), VBFixedArray(4)> Public VERSION() As Char
        Public Reserve As Byte
    End Structure

    ' �b�r�k���\����
    'Structure CSLDATA
    '    Public CslModeCode As Byte
    '    Public Drive As Byte
    '    Public SlotPosition As Byte
    '    Public Slot1 As Byte
    '    Public Slot2 As Byte
    '    Public Slot3 As Byte
    '    Public Slot4 As Byte
    '    Public Slot5 As Byte
    '    Public Slot6 As Byte
    '    Public Slot7 As Byte
    '    Public Slot8 As Byte
    '    Public Slot9 As Byte
    '    Public Slot10 As Byte
    'End Structure

    ' �\���̂̐錾
    Private CMTINFO As New MTINFBLOCK
    'Private LOADERDATA As New CSLDATA

    ' �b�l�s�֐����A�l�o�C�g�i�[
    Structure DEVICE_status
        <VBFixedString(1)> Public BIT_TMK As String
        <VBFixedString(1)> Public BIT_EOT As String
        <VBFixedString(1)> Public BIT_BOT As String
        <VBFixedString(1)> Public BIT_DEN0 As String
        <VBFixedString(1)> Public BIT_DEN1 As String
        <VBFixedString(1)> Public BIT_CSL As String
        <VBFixedString(1)> Public BIT_PRO As String
        <VBFixedString(1)> Public BIT_FIL1 As String
        <VBFixedString(1)> Public BIT_DTE As String
        <VBFixedString(1)> Public BIT_HDE As String
        <VBFixedString(1)> Public BIT_NRDY As String
        <VBFixedString(1)> Public BIT_ILC As String
        <VBFixedString(1)> Public BIT_SCE As String
        <VBFixedString(1)> Public BIT_UDC As String
        <VBFixedString(1)> Public BIT_TIM As String
        <VBFixedString(1)> Public BIT_CHC As String
        <VBFixedString(1)> Public BIT_FIL2 As String
        <VBFixedString(1)> Public BIT_FIL3 As String
        <VBFixedString(1)> Public BIT_FIL4 As String
        <VBFixedString(1)> Public BIT_FIL5 As String
        <VBFixedString(1)> Public BIT_ROB As String
        <VBFixedString(1)> Public BIT_FIL6 As String
        <VBFixedString(1)> Public BIT_FIL7 As String
        <VBFixedString(1)> Public BIT_FIL8 As String
        <VBFixedString(1)> Public BIT_FIL9 As String
        <VBFixedString(1)> Public BIT_FIL10 As String
        <VBFixedString(1)> Public BIT_FIL11 As String
        <VBFixedString(1)> Public BIT_FIL12 As String
        <VBFixedString(1)> Public BIT_FIL13 As String
        <VBFixedString(1)> Public BIT_FIL14 As String
        <VBFixedString(1)> Public BIT_FIL15 As String
        <VBFixedString(1)> Public BIT_FIL16 As String

    End Structure

#Region "public CmtCtrl()"
    '
    ' �@�@�\ : �b�l�s�Ǐ�����֐�
    '          �����ɉ����ČĂяo���֐��𐧌䂷��B
    '
    ' �߂�l : 0 - ����  0�ȊO - �ُ�
    '
    ' ��  �� : ARG1 - 11=�Í����Ȃ��ǎ�A12=�Í�������ǎ�A 22=�Í������菑��
    '
    ' ���@�l : 
    ' 
    Public Function CmtCtrl(ByVal read As Integer) As Integer
        Dim nRtn As Integer   ' �֐����A�l     
        Dim nSlotSum As Integer    ' �b�l�s�X���b�g�i�[��
        Dim bSlotNo As Byte    ' ���ݓǍ���ł���b�l�s�e�[�v�̃X���b�g�ԍ�

        'LOG.SyoriMei = "CmtCtrl"

        ' �\���̏�����
        Call CmtInfoInit()
        'Call CslInfoInit()

        ' �b�l�s���u��������
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return nRtn
        End If

        Select Case read
            Case 11    ' �I�[�g���[�_�[�Ǎ�����
                nRtn = ReadAutoLoaderCmt()

            Case 12    ' �Í�����Ǎ�
                nRtn = ChkCmtStat(bSlotNo, nSlotSum)
                If nRtn = 1 And bSlotNo = 0 And nSlotSum = 1 Then
                    ' �荷���Í�����Ǎ�
                    If ReadCmt(1, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                ElseIf nRtn = 1 And bSlotNo <> 0 And nSlotSum = 1 Then
                    ' �}�K�W���Ɉ�i�[�Í�����Ǎ�
                    If ReadCmt(bSlotNo, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                Else
                    nRtn = 1
                End If
                'Case 21    ' �Í��Ȃ�����
                '    nRtn = NotEncCmtWrite()
            Case 22    ' �Í����菑��
                nRtn = ChkCmtStat(bSlotNo, nSlotSum)
                If nRtn = 1 And bSlotNo = 0 And nSlotSum = 1 Then
                    ' �荷���Í����菑��
                    If WriteCmt(1, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                ElseIf nRtn = 1 And bSlotNo <> 0 And nSlotSum = 1 Then
                    ' �}�K�W���Ɉ�i�[�Í����菑��
                    If WriteCmt(bSlotNo, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                Else
                    nRtn = 1
                End If
            Case Else
                strLogwrite = "�p�����[�^������������܂���" & _
                                "�p�����[�^�F" & read.ToString
                LOG.Write("�N���p�����[�^����", "���s", strLogwrite)
                nRtn = 1
        End Select
        Return nRtn
    End Function
#End Region
#Region "public SelectCmt()"
    '
    ' �@�@�\ : �b�l�s���[�h�֐�
    '
    ' �߂�l : TRUE - ����  FALSE - �ُ�
    '
    ' ��  �� : ARG1 - 1-10: ���[�h����X���b�g�ԍ�
    '                 21  : �}�K�W���̃��[�h
    '                 22  : �}�K�W���̃C�W�F�N�g
    '
    ' ���@�l : 
    ' 
    Public Function SelectCmt(ByVal slotno As Byte) As Boolean
        Dim nRtn As Integer = 0
        Dim nMtRtn As Integer
        Dim cflg As Byte = 0

        'LOG.SyoriMei = "SelectCmt"

        ' �\���̏�����
        Call CmtInfoInit()

        If Not (slotno >= 1 And slotno <= 10 Or slotno = 21 Or slotno = 22) Then
            LOG.Write("�p�����[�^�ُ�", "slotno�F" & slotno.ToString)
            Return False
        End If

        ' �b�l�s���u��������
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

#If DEBUGCMT Then
        LOADERDATA.Slot1 = 1
        '        LOADERDATA.Slot2 = 1
#End If

        nRtn = MtCHGStatus(nMtRtn, bit_status)

        If bit_status.BIT_CSL = "0" Then
            Return True
        Else
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("SelectCmt", "CSL�G���[���N���܂���", strLogwrite)
            Return False
        End If
    End Function
#End Region
#Region "public UnloadCmt()"
    '
    ' �@�@�\ : �b�l�s�A�����[�h�֐�
    '
    ' �߂�l : TRUE - ����  FALSE - �ُ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : �b�l�s���u���Ɋi�[���̂b�l�s�e�[�v�����A�����[�h����
    ' 
    Public Function UnloadCmt() As Boolean
        Dim nRtn As Integer = 0
        Dim bRtn As Boolean
        Dim nMtRtn As Integer
        'LOG.SyoriMei = "UnloadCmt"

        ' �\���̏�����
        Call CmtInfoInit()
        'Call CslInfoInit()

        ' �b�l�s���u��������
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

        ' �b�l�s�X�e�[�^�X
        nRtn = FuncMtStat()
        If nRtn <> 0 Then
            bRtn = False
        End If

        '------------------------------------------
        '�b�l�s�A�����[�h
        '------------------------------------------
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)
        If bit_status.BIT_NRDY = "0" Then
            nMtRtn = mtunload(CMTINFO.UnitNo)
        End If

        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)
        If bit_status.BIT_NRDY = "1" Then
            bRtn = True
        Else
            nRtn = MtCHGStatus(nMtRtn, bit_status)
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("UnloadCmt", nMtRtn.ToString, strLogwrite)
            bRtn = False
        End If
        Return bRtn
    End Function
#End Region
#Region "public ReadCmt()"
    '
    ' �@�@�\ : �b�l�s�e�[�v�ǎ�֐�
    '          �w�肵���b�l�s�e�[�v����t�@�C����Ǎ���
    '
    ' �߂�l : TRUE - ����  FALSE - �ُ�
    '
    ' ��  �� : ARG1 - �b�l�s�e�[�v�i�[���X���b�g�ԍ�
    '          ARG2 - TURE - �Í��Ȃ��Ǎ� FALSE - �Í�����Ǎ�
    '
    ' ���@�l : 
    '
    Public Function ReadCmt(ByVal siteislotno As Byte) As Boolean
        ' �I�[�o�[���C�h
        Return ReadCmt(siteislotno, True)
    End Function

    Public Function ReadCmt(ByVal siteislotno As Byte, ByVal codeflg As Boolean) As Boolean
        Dim nRtn As Integer    ' �֐����A�l     
        Dim bHeadchk As Boolean    ' �b�l�s�w�b�_�[���x���L������ TRUE�F�t�@�C���L FALSE�F�t�@�C���Ȃ�
        Dim bFilechk As Boolean    ' �b�l�s�t�@�C���L������ TRUE�F�t�@�C���L FALSE�F�t�@�C���Ȃ�
        Dim bEndchk As Boolean    ' �b�l�s�G���h���x���L������ TRUE�F�t�@�C���L FALSE�F�t�@�C���Ȃ�
        Dim strPath As String    ' �p�X�i�[
        Dim nMtRtn As Integer    ' �b�l�s�֐����A�l
        Dim len As Integer = 0    ' �Ǎ��o�b�t�@��
        Dim buff(&HF000 - 1) As Byte    ' �Ǎ��X�g���[��
        Dim fl As FileStream    ' �t�@�C���X�g���[��

        ' ��������
        'LOG.SyoriMei = "ReadCmt"
        bHeadchk = False
        bFilechk = False
        bEndchk = False

        If siteislotno > 10 Or siteislotno < 1 Then
            LOG.Write("�w��X���b�g�ԍ�������������܂���", "siteislotno:" & siteislotno.ToString)
            Return False
        End If

        ' �b�l�s���u��������
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

        ' �b�l�s���u�X�e�[�^�X����
        nRtn = FuncMtStat()
        If nRtn <> 0 Then
            Return False
        End If

        ' �b�l�s���u�I�����C������
        nRtn = FuncMtOnline()
        If nRtn <> 0 Then
            Return False
        End If

        ' �Í������肩�Ȃ����t���O
        If codeflg Then
            ' �����Ǝ��ۊi�[�X���b�g�Ƃ̐����r
            If Not ChkSlot(siteislotno) Then
                Return False
            End If
        End If

        ' CMT.INI�ݒ�
        PutCMTIni("READ-RESULT", siteislotno.ToString, "1")
        PutCMTIni("LABEL-EXIST", siteislotno.ToString, "0")

        ' �p�X�i�[
        strPath = GetCMTIni("READ-DIRECTORY", siteislotno.ToString)

        '�t�@�C���o�͐�̐�s����
        If Not ChkReadDrFl(strPath, siteislotno) Then
            Return False
        End If

        ' �L�^���x����
        If Not ChkKirokumitsudo() Then
            PutCMTIni("READ-RESULT", siteislotno.ToString, "4")
            Return False
        End If

        '------------------------------------------
        '�R�[�h�ϊ����[�h�ݒ�
        '------------------------------------------
        'If codeflg Then
        '    nMtRtn = mtebc(7)
        'Else
        '    nMtRtn = mtebc(0)
        'End If
        nMtRtn = mtebc(0)

#If DEBUGCMT Then
        File.Copy(strPath & "\..\" & GetCMTIni("FILE-NAME", "READ"), strPath & "\" & GetCMTIni("FILE-NAME", "READ"), True)
        bFilechk = True
        If File.Exists(strPath & "\..\" & GetCMTIni("FILE-NAME", "HEAD")) = True Then
            File.Copy(strPath & "\..\" & GetCMTIni("FILE-NAME", "HEAD"), strPath & "\" & GetCMTIni("FILE-NAME", "HEAD"), True)
            bHeadchk = True
        End If
        If File.Exists(strPath & "\..\" & GetCMTIni("FILE-NAME", "END")) = True Then
            File.Copy(strPath & "\..\" & GetCMTIni("FILE-NAME", "END"), strPath & "\" & GetCMTIni("FILE-NAME", "END"), True)
            bEndchk = True
        End If
        bit_status.BIT_TMK = 1
#Else
        '------------------------------------------
        '�t�@�C���Ǎ�����
        '------------------------------------------
        Try
            fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "HEAD"), FileMode.CreateNew)
            ' �w�b�_�[���x���Ǎ�
            Do While True
                ' �o�b�t�@�̈�N���A
                Array.Clear(buff, 0, buff.Length)
                ' ��u���b�N�Ǎ���
                nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                'LOG.Write("mtrblock", "�w�b�h���x���Ǎ�", "�Ǎ��o�b�t�@��:" & len.ToString)
                '�f�o�C�X�X�e�[�^�X�擾
                nRtn = MtCHGStatus(nMtRtn, bit_status)
                ' �G���[����
                If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                    ' ���O�o��
                    strLogwrite = "�b�l�s�Ǎ��G���[�i�w�b�_�[���x���j���N���܂���" & vbCrLf & _
                                " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                " �G���[�R�[�h�F" & nMtRtn.ToString
                    LOG.Write("�w�b�_�[���x���Ǎ�����", "���s", strLogwrite)
                    Call LogCmtStat()
                    PutCMTIni("READ-RESULT", siteislotno.ToString, "4")

                    fl.Close()
                    Exit Do    ' �w�b�h���x���Ǎ����[�v�E�o
                End If
#If DEBUGCMT Then
                bit_status.BIT_TMK = 1
#End If
                '�e�[�v�}�[�N���o
                If bit_status.BIT_TMK = 1 Then
                    fl.Close()
                    Exit Do    ' �w�b�h���x���Ǎ����[�v�E�o
                End If
                ' HEADLABEL�ɏ���
                fl.Write(buff, 0, len)
                ' �w�b�_�[���x���Ǎ��t���O
                bHeadchk = True
            Loop
        Catch ex As Exception
            LOG.Write("�w�b�_�[���x���Ǎ�", "��O", ex.Message)
        End Try

        Try
            fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "READ"), FileMode.CreateNew)
            ' �t�@�C���Ǎ�
            Do While True
                ' �o�b�t�@�̈�N���A
                Array.Clear(buff, 0, buff.Length)
                ' ��u���b�N�Ǎ���
                nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                'LOG.Write("mtrblock", "�t�@�C���Ǎ�", "�Ǎ��o�b�t�@��:" & len.ToString)
                '�f�o�C�X�X�e�[�^�X�擾
                nRtn = MtCHGStatus(nMtRtn, bit_status)
                ' �G���[����
                If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                    ' ���O�o��
                    strLogwrite = "�b�l�s�Ǎ��G���[�i�t�@�C���j���N���܂���" & vbCrLf & _
                                " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                " �G���[�R�[�h�F" & nMtRtn.ToString
                    LOG.Write("�t�@�C���Ǎ�����", "���s", strLogwrite)
                    Call LogCmtStat()
                    PutCMTIni("READ-RESULT", siteislotno.ToString, "4")

                    fl.Close()
                    Exit Do    ' �t�@�C���Ǎ����[�v�E�o
                End If
                ' �e�[�v�}�[�N���o
                If bit_status.BIT_TMK = 1 Then
                    fl.Close()
                    Exit Do    ' �t�@�C���Ǎ����[�v�E�o
                End If
                ' INPUT�ɏ���
                fl.Write(buff, 0, len)

                ' �t�@�C���Ǎ��t���O
                bFilechk = True
            Loop
        Catch ex As Exception
            LOG.Write("�t�@�C���Ǎ�", "��O", ex.Message)
        End Try

        Try
            ' �w�b�_�[���x���A�t�@�C����Ǎ��񂾂Ƃ��̂ݎ��s
            If bHeadchk = True And bFilechk = True Then
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "END"), FileMode.CreateNew)
                ' �G���h���x���Ǎ�
                Do While True
                    ' �o�b�t�@�̈�N���A
                    Array.Clear(buff, 0, buff.Length)
                    ' ��u���b�N�Ǎ�
                    nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                    'LOG.Write("mtrblock", "�G���h���x���Ǎ�", "�Ǎ��o�b�t�@��:" & len.ToString)
                    '�f�o�C�X�X�e�[�^�X�擾
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    ' �G���[����
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ���O�o��
                        strLogwrite = "�b�l�s�Ǎ��G���[�i�G���h�j���N���܂���" & vbCrLf & _
                                    " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                    " �G���[�R�[�h�F" & nMtRtn.ToString
                        LOG.Write("�Ǎ�����", "���s", strLogwrite)
                        Call LogCmtStat()
                        PutCMTIni("READ-RESULT", siteislotno.ToString, "4")

                        fl.Close()
                        Exit Do    ' �G���h���x���Ǎ����[�v�E�o
                    End If
                    ' �e�[�v�}�[�N���o
                    If bit_status.BIT_TMK = 1 Then
                        fl.Close()
                        Exit Do    ' �G���h���x���Ǎ����[�v�E�o
                    End If
                    ' ENDLABEL�ɏ���
                    fl.Write(buff, 0, len)

                    ' �G���h���x���Ǎ��t���O
                    bEndchk = True
                Loop
            End If

        Catch ex As Exception
            LOG.Write("�G���h���x���Ǎ�", "��O", ex.Message)
        End Try
#End If

        ' �Ǎ��b�l�s���x���L������
        nRtn = ChkLabel(strPath, nMtRtn, siteislotno, bHeadchk, bFilechk, bEndchk)
        If nRtn <> 0 Then
            LOG.Write("�t�@�C���ǂݍ��ُ݈�ł�", "HEADLABEL:" & bHeadchk & " INPUT:" & bFilechk & " ENDLABEL:" & bEndchk)
        End If

        ' �b�l�s���u�����߂�����
        If FuncMtrewind() <> 0 Then
            Return False
        End If
        Call LogCmtStat()

        ' �ǎ挋�ʔ���
        If Not "0" = GetCMTIni("READ-RESULT", siteislotno.ToString) Then
            Return False
        End If
        Return True
    End Function
#End Region
#Region "public WriteCmt()"

    '
    ' �@�@�\ : �b�l�s�e�[�v�����֐�
    '
    ' �߂�l : TRUE - ����  FALSE - �ُ�
    '
    ' ��  �� : ARG1 - �b�l�s�e�[�v�i�[���X���b�g�ԍ�
    '          ARG2 - TRUE - EBCDIC�ϊ����� FALSE -EBCDIC�ϊ����Ȃ� 
    '
    ' ���@�l : 
    '   
    Public Function WriteCmt(ByVal siteislotno As Byte) As Boolean
        ' �I�[�o�[���C�h
        Return WriteCmt(siteislotno, True)
    End Function
    Public Function WriteCmt(ByVal siteislotno As Byte, ByVal codeflg As Boolean) As Boolean
        Dim strPath As String    ' �p�X�i�[
        Dim nRtn As Integer    ' �֐����A�l     
        Dim nMtRtn As Integer    ' CMT���A�l
        Dim buff(&HF000 - 1) As Byte ' �����o�C�g�X�g���[��
        Dim count As Integer    ' �t�@�C���ǂݍ��ݎ��̕�����
        Dim nBlocklen As Integer   ' �u���b�N��
        Dim bHeadchk As Boolean    ' �����w�b�_�[���x���L������ TRUE�F�t�@�C������ FALSE�F�t�@�C���Ȃ�
        Dim bEndchk As Boolean    ' �����G���h���x���L������ TRUE�F�t�@�C������ FALSE�F�t�@�C���Ȃ�
        Dim bFilechk As Boolean    ' �����t�@�C���L������ TRUE�F�t�@�C������ FALSE�F�t�@�C���Ȃ�
        Dim bDrive As Boolean    ' �b�l�s�Ǎ��L������ TRUE�F�Ǎ���� FALSE�F��Ǎ����
        Dim nCnt As Integer    ' �ėp�J�E���^
        Dim fl As FileStream


        ' ��������
        'LOG.SyoriMei = "WriteCmt"
        bHeadchk = False
        bFilechk = False
        bEndchk = False
        bDrive = False

        If siteislotno > 10 Or siteislotno < 1 Then
            LOG.Write("�w��X���b�g�ԍ�������������܂���", "siteislotno:" & siteislotno.ToString)
            Return False
        End If

        ' �b�l�s���u��������
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

        ' �b�l�s���u�X�e�[�^�X����
        nRtn = FuncMtStat()
        If nRtn <> 0 Then
            Return False
        End If

        ' �b�l�s���u�I�����C������
        nRtn = FuncMtOnline()
        If nRtn <> 0 Then
            Return False
        Else
            bDrive = True
        End If

        ' �Í�������Ȃ��t���O
        If codeflg Then
            ' �����Ǝ��ۊi�[�X���b�g�Ƃ̐����r
            If Not ChkSlot(siteislotno) Then
                Return False
            End If
        End If

        ' CMT.INI�ݒ�
        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "1")

        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        '�L�^���x����
        If Not ChkKirokumitsudo() Then
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
            Return False
        End If

        '�v���e�N�g����
        Select Case bit_status.BIT_PRO
            Case 0
                strProtect = "�����\"
            Case 1
                strProtect = "�����֎~"
                strLogwrite = "�����֎~�ł�  �v���e�N�g���������Ă�������" & vbCrLf & _
                "�X�e�[�^�X�F" & bit_status.BIT_PRO & bit_status.BIT_DEN1
                LOG.Write("�v���e�N�g����", "���s", strLogwrite)
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            Case Else
        End Select

        ' �p�X�i�[
        strPath = GetCMTIni("WRITE-DIRECTORY", siteislotno.ToString)

        If Not ChkWriteDrFl(strPath, siteislotno, bDrive, bHeadchk, bFilechk, bEndchk) Then
            Return False
        End If

        '------------------------------------------
        '�R�[�h�ϊ����[�h�ݒ�
        '------------------------------------------
        'If codeflg Then
        '    nMtRtn = mtebc(7)
        'Else
        '    nMtRtn = mtebc(0)
        'End If

        nMtRtn = mtebc(0)

        '------------------------------------------
        '�t�@�C����������
        '------------------------------------------
        LOG.Write("head:" & bHeadchk.ToString, "file:" & bFilechk.ToString, "end:" & bEndchk.ToString)

        If bHeadchk = True And bFilechk = True And bEndchk = True Then
            ' ���x������ԋp�t�@�C���̏ꍇ
            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "HEAD"))) Then
                    LOG.Write("��������", "���s", Path.Combine(strPath, GetCMTIni("FILE-NAME", "HEAD")) & "������܂���")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' �w�b�_�[���x������
                fl = New FileStream(Path.Combine(strPath, GetCMTIni("FILE-NAME", "HEAD")), FileMode.Open)
                ' �o�b�t�@�̈�N���A
                Array.Clear(buff, 0, buff.Length)
                ' �J�E���g��0��
                nCnt = 0
                ' �w�b�_�[���x���Ǎ�
                count = fl.Read(buff, 0, 80)
                Do
                    ' �w�b�_�[���x���Q�Ɋi�[���Ă���u���b�N�����擾����
                    If nCnt = 2 Then
                        Try
                            nBlocklen = Integer.Parse("0" & System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(buff, 5, 5).Trim)
                            LOG.Write("���x������w�b�_�[����", "nCnt:" & nCnt.ToString, "�u���b�N���F" & nBlocklen.ToString)
                        Catch ex As Exception
                            '***ASTAR SUSUKI 2008.06.13             ***
                            'EBCDIC�Ή�                             ***
                            nBlocklen = Integer.Parse("0" & System.Text.Encoding.GetEncoding("IBM290").GetString(buff, 5, 5).Trim)
                            LOG.Write("���x������w�b�_�[����", "nCnt:" & nCnt.ToString, "�u���b�N���F" & nBlocklen.ToString)
                            '******************************************
                        End Try
                    End If
                    nCnt = nCnt + 1
                    ' ��u���b�N����
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    LOG.Write("mtwblock", "�w�b�_�[���x������", "�����o�b�t�@��:" & count.ToString)
                    ' �f�o�C�X�X�e�[�^�X�擾
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    '�G���[����
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ���O�o��
                        strLogwrite = "�w�b�_�[���x�������ُ킪�N���܂���" & vbCrLf & _
                                    " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                    " �G���[�R�[�h�F" & nMtRtn.ToString
                        LOG.Write("�w�b�_�[���x����������", "���s", strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Return False
                    End If
                    ' �o�b�t�@�̈�N���A
                    Array.Clear(buff, 0, buff.Length)
                    ' �ԋp�w�b�_�[���x�����o�b�t�@�ɓǍ�
                    count = fl.Read(buff, 0, 80)
                Loop While count > 0    ' �t�@�C������ǎ�����o�b�t�@����0�ɂȂ�܂ŌJ��Ԃ�
                fl.Close()
            Catch ex As Exception
                LOG.Write("����������O", "�w�b�_�[���x��", ex.Message)
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try
            ' �e�[�v�}�[�N����
            nMtRtn = mtwtmk(CMTINFO.UnitNo)
            LOG.Write("�w�b�_�[���x������", "mtwtmk", nMtRtn.ToString)

            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE"))) Then
                    LOG.Write("��������", "���s", Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE")) & "����܂���")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' �t�@�C������
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "WRITE"), FileMode.Open)
                ' �o�b�t�@�̈�N���A
                Array.Clear(buff, 0, buff.Length)
                ' �w�b�_�[���x���������Ɏ擾�����u���b�N�����o�b�t�@�ɓǍ���
                count = fl.Read(buff, 0, nBlocklen)
                Do
                    ' ��u���b�N����
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    '*** �C�� mitsu 2008/08/29 �R�����g�A�E�g ***
                    'LOG.Write("mtwblock", "�ԋp�t�@�C������", "�����o�b�t�@��:" & count.ToString)
                    '********************************************
                    '�f�o�C�X�X�e�[�^�X�擾
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    '�G���[����
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ���O�o��
                        strLogwrite = "�ԋp�t�@�C�������ُ킪�N���܂���" & vbCrLf & _
                                    " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                    " �G���[�R�[�h�F" & nMtRtn.ToString
                        LOG.Write("�ԋp�t�@�C����������", "���s", strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Return False
                    End If
                    ' �o�b�t�@�̈�N���A
                    Array.Clear(buff, 0, buff.Length)
                    ' �w�b�_�[���x���������Ɏ擾�����u���b�N�����o�b�t�@�ɓǍ���
                    count = fl.Read(buff, 0, nBlocklen)
                Loop While count > 0
                fl.Close()
            Catch ex As Exception
                LOG.Write("����������O", "�ԋp�t�@�C��", ex.Message)
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try
            nMtRtn = mtwtmk(CMTINFO.UnitNo)
            LOG.Write("�ԋp�t�@�C������", "mtwtmk", nMtRtn.ToString)

            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "END"))) Then
                    LOG.Write("��������", "���s", Path.Combine(strPath, GetCMTIni("FILE-NAME", "END")) & "����܂���")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' �G���h���x������
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "END"), FileMode.Open)
                ' �o�b�t�@�̈�N���A
                Array.Clear(buff, 0, buff.Length)
                ' �G���h���x���Ǎ�
                count = fl.Read(buff, 0, 80)
                Do
                    ' ��u���b�N����
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    LOG.Write("mtwblock", "�G���h���x������", "�����o�b�t�@��:" & count.ToString)
                    '�f�o�C�X�X�e�[�^�X�擾
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    '�G���[����
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ���O�o��
                        strLogwrite = "�G���h���x�������ُ킪�N���܂���" & vbCrLf & _
                                    " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                    " �G���[�R�[�h�F" & nMtRtn.ToString
                        LOG.Write("��������", "���s", strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Return False
                    End If
                    ' �o�b�t�@�̈�N���A
                    Array.Clear(buff, 0, buff.Length)
                    ' �ԋp�G���h���x�����o�b�t�@�ɓǍ��� 
                    count = fl.Read(buff, 0, 80)
                Loop While count > 0
                fl.Close()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "0")
            Catch ex As Exception
                LOG.Write("����������O", "�G���h���R�[�h", ex.Message)
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try
            nMtRtn = mtwmtmk(CMTINFO.UnitNo)
            LOG.Write("�ԋp�t�@�C������", "mtwmtmk", nMtRtn.ToString)
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "0")

        ElseIf bHeadchk = False And bFilechk = True And bEndchk = False Then
            ' ���x���Ȃ��ԋp�t�@�C���̏ꍇ
            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE"))) Then
                    LOG.Write("��������", "���s", Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE")) & "����܂���")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' �e�[�v�}�[�N����t�H�[�}�b�g�̏ꍇ
                If GetCMTIni("LABEL-EXIST", siteislotno.ToString) = "2" Then
                    nMtRtn = mtwtmk(CMTINFO.UnitNo)
                    LOG.Write("�e�[�v�}�[�N����t�H�[�}�b�g", "mtwtmk", nMtRtn.ToString)
                End If

                '***ASTAR 2008.08.07 �u���b�N�w��Ή� >>
                Dim nBuffLen As Integer
                If Me.BlockSize = -1 Then
                    '�u���b�N�T�C�Y�w��Ȃ�
                    nBuffLen = buff.Length
                Else
                    '�u���b�N�T�C�Y�w�肠��
                    nBuffLen = Me.BlockSize
                End If
                '***ASTAR 2008.08.07 �u���b�N�w��Ή� <<

                ' �t�@�C������
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "WRITE"), FileMode.Open)
                ' �o�b�t�@�̈���N���A
                Array.Clear(buff, 0, buff.Length)
                ' �ԋp�t�@�C����Ǎ���
                '***ASTAR 2008.08.07 �u���b�N�w��Ή� >>
                'count = fl.Read(buff, 0, buff.Length)
                count = fl.Read(buff, 0, nBuffLen)
                '***ASTAR 2008.08.07 �u���b�N�w��Ή� <<
                Do While count > 0
                    ' ��u���b�N����
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    '*** �C�� mitsu 2008/08/29 �R�����g�A�E�g ***
                    'LOG.Write("mtwblock", "�ԋp�t�@�C������", "�����o�b�t�@��:" & count.ToString)
                    '********************************************
                    '�f�o�C�X�X�e�[�^�X�擾
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    '�G���[����
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        '���O�o��
                        LOG.Write("�m�����x���ԋp�t�@�C�������ُ킪�N���܂���", _
                                "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & _
                                " DTE:" & bit_status.BIT_DTE & " HDE:" & bit_status.BIT_HDE, strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Exit Do
                    End If
                    ' �o�b�t�@�̈���N���A
                    Array.Clear(buff, 0, buff.Length)
                    '�ԋp�t�@�C�����o�b�t�@�ɓǍ���
                    '***ASTAR 2008.08.07 �u���b�N�w��Ή� >>
                    'count = fl.Read(buff, 0, buff.Length)
                    count = fl.Read(buff, 0, nBuffLen)
                    '***ASTAR 2008.08.07 �u���b�N�w��Ή� <<
                Loop
                fl.Close()
            Catch ex As Exception
                If GetCMTIni("LABEL-EXIST", siteislotno.ToString) = "0" Then
                    LOG.Write("����������O", "�m�����x���ԋp�t�@�C��", ex.Message)
                ElseIf GetCMTIni("LABEL-EXIST", siteislotno.ToString) = "2" Then
                    LOG.Write("����������O", "�e�[�v�}�[�N����ԋp�t�@�C��", ex.Message)
                End If
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try

            mtwmtmk(CMTINFO.UnitNo)
            LOG.Write("�ԋp�t�@�C������", "mtwmtmk", nMtRtn.ToString)
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "0")

        ElseIf bHeadchk = False And bFilechk = False And bEndchk = False Then
            LOG.Write("�����t�@�C��������܂���", "HEAD:" & bHeadchk & " FILE:" & bFilechk & "END" & bEndchk)
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "2")
        Else
            LOG.Write("�����t�@�C��������܂���", "HEAD:" & bHeadchk & " FILE:" & bFilechk & "END" & bEndchk)
        End If

        ' �����߂�
        If FuncMtrewind() <> 0 Then
            Return False
        End If
        Call LogCmtStat()

        ' �������ʔ���
        If Not "0" = GetCMTIni("WRITE-RESULT", siteislotno.ToString) Then
            Return False
        End If
        Return True
    End Function
#End Region
#Region "public ChkCmtStat()"
    '
    ' �@�@�\ : �b�l�s���擾�֐�
    '
    ' �߂�l : 0 - �b�l�s�e�[�v���b�l�s���u�ɂȂ� 
    '          1 - �b�l�s�e�[�v���b�l�s���u�ɂ���
    '          4 - �b�l�s���u�G���[
    '
    ' ��  �� : ARG1 - �b�l�s�i�[���X���b�g
    '          ARG2 - �b�l�s�e�[�v���v
    '
    ' ���@�l : 
    ' 
    Public Function ChkCmtStat(ByRef slotno As Byte, ByRef slotsum As Integer) As Integer
        Dim nRtn As Integer  ' �֐����A�l

        slotno = 0
        slotsum = 0

        'mtinit
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            slotno = 0
            Return 4
        End If

        'mtstat
        nRtn = FuncMtStat()
        'If nRtn <> 0 Then
        '    slotno = 0
        '    Return 4
        'End If
        slotsum = 1
    End Function
#End Region
#Region "private ReadAutoLoaderCmt()"
    '
    ' �@�@�\ : �I�[�g���[�_�[�b�l�s�ǎ�֐�
    '
    ' �߂�l : 0 - ����  1 - �ُ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : 
    '
    Private Function ReadAutoLoaderCmt() As Integer
        Dim nRtn As Integer    ' �֐����A�l     
        Dim nMtRtn As Integer    ' �b�l�s�֐����A�l
        Dim len As Integer = 0    '�Ǎ��o�b�t�@��
        Dim strPath As String    ' �p�X
        Dim buff(&HF000 - 1) As Byte    ' �Ǎ��o�b�t�@
        Dim bSlotNo As Byte    ' �b�l�s�X���b�g�i���o�[�i���[�v�Ɏg�p�j
        Dim cflg As Byte = 1    ' �b�r�k�X�e�[�^�X�t���O
        Dim bMemNo As Byte = 0    ' �b�l�s�e�[�v�i�[���ōő�X���b�g�ԍ�
        Dim fl As FileStream    ' �t�@�C���X�g���[��
        Dim nErrCnt As Integer    ' �ŏI�G���[���J�E���g
        Dim bHandFlg As Boolean = False  ' �荷���t���O
        Dim bFstFlg As Boolean = False    ' ��񏈗�����
        Dim bHeadchk As Boolean    ' �b�l�s�w�b�_�[���x���L������ TRUE�F�t�@�C���L FALSE�F�t�@�C���Ȃ�
        Dim bFilechk As Boolean    ' �b�l�s�t�@�C���L������ TRUE�F�t�@�C���L FALSE�F�t�@�C���Ȃ�
        Dim bEndchk As Boolean    ' �b�l�s�G���h���x���L������ TRUE�F�t�@�C���L FALSE�F�t�@�C���Ȃ�

        'LOG.SyoriMei = "ReadAutoLoaderCmt"

        'CMT.INI�ɃX�e�[�^�X���i�[
        For bSlotNo = 1 To 10 Step 1
            PutCMTIni("READ-RESULT", bSlotNo.ToString, "1")
            PutCMTIni("LABEL-EXIST", bSlotNo.ToString, "0")
        Next

        For bSlotNo = 1 To 10 Step 1
            ' ��������
            bHeadchk = False
            bFilechk = False
            bEndchk = False

            ' �G���[���X�L�b�v�pDo
            Do
                ' �荷���t���O�𗧂Ă�
                bHandFlg = True

                ' �b�l�s���u�X�e�[�^�X����
                nRtn = FuncMtStat()
                If nRtn <> 0 Then
                    PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                    Exit Do
                End If

                ' �b�l�s���u�I�����C������
                nRtn = FuncMtOnline()
                If nRtn <> 0 Then
                    PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                    Exit Do
                End If

                ' �p�X�i�[
                strPath = GetCMTIni("READ-DIRECTORY", bSlotNo.ToString)

                '�t�@�C���o�͐��s����
                If Not ChkReadDrFl(strPath, bSlotNo) Then
                    Exit Do
                End If

                ' �L�^���x����
                If Not ChkKirokumitsudo() Then
                    PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                    Return False
                End If

                '------------------------------------------
                '�R�[�h�ϊ����[�h�ݒ�
                '------------------------------------------
                nMtRtn = mtebc(0)

                '------------------------------------------
                '�t�@�C���Ǎ�����
                '------------------------------------------
                Try
                    fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "HEAD"), FileMode.CreateNew)
                    ' �w�b�_�[���x���Ǎ� 
                    Do While True
                        ' �o�b�t�@�̈�N���A
                        Array.Clear(buff, 0, buff.Length)
                        ' ��u���b�N�Ǎ�
                        nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                        'LOG.Write("mtrblock", "�w�b�h���x���Ǎ�", "�Ǎ��o�b�t�@��:" & len.ToString)
                        '�f�o�C�X�X�e�[�^�X�擾
                        nRtn = MtCHGStatus(nMtRtn, bit_status)

                        ' �G���[����
                        If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                            ' ���O�o��
                            strLogwrite = "�b�l�s�Ǎ��G���[�i�w�b�_�[���x���j���N���܂���" & vbCrLf & _
                                        " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                        " �G���[�R�[�h�F" & nMtRtn.ToString
                            LOG.Write("�w�b�_�[���x���Ǎ�����", "���s", strLogwrite)
                            Call LogCmtStat()

                            PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                            fl.Close()
                            Exit Do    ' �w�b�h���x���Ǎ����[�v�E�o
                        End If
                        '�e�[�v�}�[�N���o
                        If bit_status.BIT_TMK = 1 Then
                            fl.Close()
                            Exit Do    ' �w�b�h���x���Ǎ����[�v�E�o
                        End If

                        ' HEADLABEL�ɏ���
                        fl.Write(buff, 0, len)
                        ' �w�b�_�[���x���Ǎ��t���O
                        bHeadchk = True
                    Loop
                Catch ex As Exception
                    LOG.Write("�w�b�h���x���Ǎ�", "��O", ex.Message)
                    Exit Do    ' �G���[�E�o
                End Try

                Try

                    fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "READ"), FileMode.CreateNew)
                    ' �t�@�C���Ǎ�
                    Do While True
                        ' �o�b�t�@�̈�N���A
                        Array.Clear(buff, 0, buff.Length)
                        ' ��u���b�N�Ǎ���
                        nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)
                        'LOG.Write("mtrblock", "�t�@�C���Ǎ�", "�Ǎ��o�b�t�@��:" & len.ToString)
                        '�f�o�C�X�X�e�[�^�X�擾
                        nRtn = MtCHGStatus(nMtRtn, bit_status)
                        ' �G���[����
                        If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                            ' ���O�o��
                            strLogwrite = "�b�l�s�Ǎ��G���[�i�t�@�C���j���N���܂���" & vbCrLf & _
                                        " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                        " �G���[�R�[�h�F" & nMtRtn.ToString
                            LOG.Write("�Ǎ�����", "���s", strLogwrite)
                            Call LogCmtStat()
                            PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")

                            fl.Close()
                            Exit Do    ' �t�@�C���Ǎ����[�v�E�o
                        End If
                        ' �e�[�v�}�[�N���o
                        If bit_status.BIT_TMK = 1 Then
                            fl.Close()
                            Exit Do    ' �t�@�C���Ǎ����[�v�E�o
                        End If
                        ' INPUT�ɏ���
                        fl.Write(buff, 0, len)

                        ' �t�@�C���Ǎ��t���O
                        bFilechk = True
                    Loop

                Catch ex As Exception
                    LOG.Write("�t�@�C���Ǎ�", "��O", ex.Message)
                    Exit Do
                End Try

                Try
                    ' �w�b�_�[���x���A�t�@�C����Ǎ��񂾂Ƃ��̂ݎ��s
                    If bHeadchk = True And bFilechk = True Then
                        fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "END"), FileMode.CreateNew)
                        ' �G���h���x���Ǎ�
                        Do While True
                            ' ��u���b�N�Ǎ�
                            nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)
                            'LOG.Write("mtrblock", "�G���h���x���Ǎ�", "�Ǎ��o�b�t�@��:" & len.ToString)
                            '�f�o�C�X�X�e�[�^�X�擾
                            nRtn = MtCHGStatus(nMtRtn, bit_status)
                            ' �G���[����
                            If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                                strLogwrite = "�b�l�s�Ǎ��G���[�i�G���h�j���N���܂���" & vbCrLf & _
                                            " �c�s�d�F" & bit_status.BIT_DTE & " �g�c�d�F" & bit_status.BIT_HDE & _
                                            " �G���[�R�[�h�F" & nMtRtn.ToString
                                LOG.Write("�Ǎ�����", "���s", strLogwrite)
                                PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                                fl.Close()
                                Exit Do    ' �G���h���x���Ǎ����[�v�E�o
                            End If
                            ' �e�[�v�}�[�N���o
                            If bit_status.BIT_TMK = 1 Then
                                fl.Close()
                                Exit Do    ' �G���h���x���Ǎ����[�v�E�o
                            End If
                            ' ENDLABEL�ɏ���
                            fl.Write(buff, 0, len)
                            ' �G���h���x���Ǎ��t���O
                            bEndchk = True
                        Loop
                    End If

                Catch ex As Exception
                    LOG.Write("�G���h���x���Ǎ�", "��O", ex.Message)
                    Exit Do
                End Try

                ' �Ǎ��b�l�s���x���L������
                nRtn = ChkLabel(strPath, nMtRtn, bSlotNo, bHeadchk, bFilechk, bEndchk)
                If nRtn <> 0 Then
                    LOG.Write("�t�@�C���ǂݍ��ُ݈�ł�", "HEADLABEL:" & bHeadchk & " INPUT:" & bFilechk & " ENDLABEL:" & bEndchk)
                    Exit Do
                End If

            Loop While False
            If bHandFlg Then
                Exit For    '�荷���Ǎ��Ȃ烋�[�v�E�o
            End If

            If bSlotNo = bMemNo Then
                '------------------------------------------
                '�b�l�s�A�����[�h
                '------------------------------------------
                nMtRtn = mtstat(CMTINFO.UnitNo)
                nRtn = MtCHGStatus(nMtRtn, bit_status)
                If bit_status.BIT_NRDY = "0" Then
                    nMtRtn = mtunload(CMTINFO.UnitNo)
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                End If
                Exit For
            End If
        Next bSlotNo
        ' �ǎ挋�ʔ���
        If bHandFlg Then
            If Not "0" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Then
                Return 1
            End If
        Else
            nErrCnt = 0
            For bSlotNo = 1 To 10 Step 1
                ' ��s�t�@�C������A�ǂݍ��ُ݈킪�ЂƂł��������ꍇ�ُ͈한�A����
                If "3" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Or "4" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Then
                    Return 1
                End If
                ' �S�ẴX���b�g�ɂb�l�s�������Ă��Ȃ��A
                ' �b�l�s�Ƀt�@�C���������Ă��Ȃ������ꍇ�̓G���[�J�E���g�A�b�v����
                If "1" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Or "2" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Then
                    nErrCnt = nErrCnt + 1
                End If
            Next
            LOG.Write("ErrCnt", nErrCnt.ToString)
            ' ���ׂăG���[�������ꍇ
            If nErrCnt = 10 Then
                Return 1
            End If
        End If
        Return 0
    End Function
#End Region
#Region "private FuncMtinit()"
    '
    ' �@�@�\ : �b�l�s���u�����֐�
    '
    ' �߂�l : 0 - ����  1 - �ُ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : �ŏ��ɌĂяo���K�v����
    ' 
    Private Function FuncMtinit() As Integer
        Dim nMtRtn, nRtn As Integer    ' MT�֐��̕��A�l

        '*** ASTAR.S.S 2008.05.28 �o�b�N������b�l�s����s��Ή�		***
        nMtRtn = mtstat(0)
        nMtRtn = mtstat(1)
        '******************************************************************

        ' mtinit����
        nMtRtn = mtinit(CMTINFO)
        '*** ASTAR.S.S 2008.05.28 �o�b�N������b�l�s����s��Ή�		***
        Call mtonline(CMTINFO.UnitNo)
        '******************************************************************
        nRtn = MtCHGStatus(nMtRtn, bit_status)
#If Not DEBUGCMT Then
        If nMtRtn = 0 Or nMtRtn = &HFFFFFFFF Then
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("�b�l�s���u�̏�������", "���s", strLogwrite)
            Return 1
        End If
#End If

        Return 0
    End Function
#End Region
#Region "private FuncMtStat()"
    '
    ' �@�@�\ : �b�l�s�X�e�[�^�X�擾�֐�
    '
    ' �߂�l : 0 - ����  0�ȊO - �ُ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : �b�l�s�e�[�v�𑀍삷��O�ɌĂяo���K�v����
    ' 
    Private Function FuncMtStat() As Integer
        Dim nMtRtn, nRtn As Integer    ' MT�֐��̕��A�l

        ' mtstat����(���s��)
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        If nRtn <> 0 Then
            If bit_status.BIT_PRO = "1" OrElse _
                bit_status.BIT_BOT = "1" OrElse _
                bit_status.BIT_EOT = "1" OrElse _
                bit_status.BIT_TMK = "1" Then
                ' �v���e�N�g�A�a�n�s���o�A�d�n�s���o�A�e�[�v�}�[�N���o�� ���^�[������
                Return 0
            End If
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("�b�l�s���u�̃X�e�[�^�X�擾", "���s", strLogwrite)
            Return nRtn
        End If
        Return nRtn
    End Function
#End Region
#Region "private FuncMtOnline()"
    '
    ' �@�@�\ : �b�l�s�e�[�v�I�����C���֐�
    '
    ' �߂�l : 0 - ����  0�ȊO - �ُ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : 
    ' 
    Private Function FuncMtOnline() As Integer

        Dim nMtRtn, nRtn As Integer    ' MT�֐��̕��A�l

        ' mtonline����
        nMtRtn = mtonline(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        If nMtRtn <> 0 Then
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("�b�l�s���u�̃I�����C������", "���s", strLogwrite)
            Return nMtRtn
        End If
        Return nMtRtn
    End Function
#End Region
#Region "private FuncMtrewind()"
    '
    ' �@�@�\ : �b�l�s�e�[�v�����߂��֐�
    '
    ' �߂�l : 0 - ����  0�ȊO - �ُ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : ���łɂb�l�s�e�[�v�����[�h��Ԃ̏ꍇ�́A�a�n�s�ʒu�܂Ŋ����߂�
    ' 
    Private Function FuncMtrewind() As Integer
        Dim nMtRtn, nRtn As Integer

        nMtRtn = mtrewind(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)
        If nMtRtn <> 0 Then
            strLogwrite = Setbit_statusLog(nMtRtn, nRtn)
            LOG.Write("�b�l�s�e�[�v�����߂�����", "���s", strLogwrite)
            Return nMtRtn
        End If
        Return nMtRtn
    End Function
#End Region
#Region "private CslInfoInit()"
    '
    ' �@�@�\ : CSLDATA�\���̂̏�����
    '
    ' �߂�l : 0 - ����
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : 
    ' 
    'Private Function CslInfoInit() As Integer
    '    LOADERDATA.CslModeCode = 2
    '    LOADERDATA.Drive = 0
    '    'LOADERDATA.SlotPosition = 0	'***�C�� ���� 2008.11.13 �������݂������Ȃ��̂ŏC��
    '    LOADERDATA.SlotPosition = 1		'***�C�� ���� 2008.11.13 �������݂������Ȃ��̂ŏC��
    '    LOADERDATA.Slot1 = 0
    '    LOADERDATA.Slot2 = 0
    '    LOADERDATA.Slot3 = 0
    '    LOADERDATA.Slot4 = 0
    '    LOADERDATA.Slot5 = 0
    '    LOADERDATA.Slot6 = 0
    '    LOADERDATA.Slot7 = 0
    '    LOADERDATA.Slot8 = 0
    '    LOADERDATA.Slot9 = 0
    '    LOADERDATA.Slot10 = 0
    '    Return 0
    'End Function
#End Region
#Region "private CmtInfoInit()"
    '
    ' �@�@�\ : MTINFOBLOCK�\���̂̏�����
    '
    ' �߂�l : 0 - ����
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : 
    '   
    Private Function CmtInfoInit() As Integer
        CMTINFO.UnitNo = 0
        CMTINFO.HostNo = 0
        CMTINFO.TargetNo = 0
        CMTINFO.Vender = Array.CreateInstance(GetType(Char), 8)
        CMTINFO.Product = Array.CreateInstance(GetType(Char), 16)
        CMTINFO.VERSION = Array.CreateInstance(GetType(Char), 4)
        CMTINFO.Reserve = 0
        Return 0
    End Function
#End Region
#Region "private MtCHGStatus()"
    '
    ' �@�@�\ : �b�l�s�A�N�Z�X�֐����A�l�ϊ�
    '          �r�b�g����̊֐����A�l���o�C�g����ɕϊ�����
    '
    ' �߂�l : 0 - ����  0�ȊO - �ُ�
    '
    ' ��  �� : ARG1 - �b�l�s�A�N�Z�X�֐����A�l ARG2 - �ϊ���o�C�g����\����
    '
    ' ���@�l : 
    '
    Private Function MtCHGStatus(ByVal Fskj_mt_rtn As Integer, ByRef Fskj_mt_BITrtn As DEVICE_status) As Integer

        Dim STATUS_BIT(31) As Byte    ' �o�C�g�f�[�^�ꎞ�ۑ�
        Dim count As Integer    ' �J�E���^�[

        ' �G���[�N���A
        Err.Clear()

        MtCHGStatus = -1
        For count = 0 To 31
            'STATUS_BIT(count) = (Fskj_mt_rtn And (1L << count)) >> count
            STATUS_BIT(count) = Fskj_mt_rtn And &H1
            Fskj_mt_rtn = Fskj_mt_rtn >> 1

        Next
        Fskj_mt_BITrtn.BIT_TMK = CStr(Math.Abs(STATUS_BIT(0)))
        Fskj_mt_BITrtn.BIT_EOT = CStr(Math.Abs(STATUS_BIT(1)))
        Fskj_mt_BITrtn.BIT_BOT = CStr(Math.Abs(STATUS_BIT(2)))
        Fskj_mt_BITrtn.BIT_DEN0 = CStr(Math.Abs(STATUS_BIT(3)))
        Fskj_mt_BITrtn.BIT_DEN1 = CStr(Math.Abs(STATUS_BIT(4)))
        Fskj_mt_BITrtn.BIT_CSL = CStr(Math.Abs(STATUS_BIT(5)))
        Fskj_mt_BITrtn.BIT_PRO = CStr(Math.Abs(STATUS_BIT(6)))
        Fskj_mt_BITrtn.BIT_FIL1 = CStr(Math.Abs(STATUS_BIT(7)))
        Fskj_mt_BITrtn.BIT_DTE = CStr(Math.Abs(STATUS_BIT(8)))
        Fskj_mt_BITrtn.BIT_HDE = CStr(Math.Abs(STATUS_BIT(9)))
        Fskj_mt_BITrtn.BIT_NRDY = CStr(Math.Abs(STATUS_BIT(10)))
        Fskj_mt_BITrtn.BIT_ILC = CStr(Math.Abs(STATUS_BIT(11)))
        Fskj_mt_BITrtn.BIT_SCE = CStr(Math.Abs(STATUS_BIT(12)))
        Fskj_mt_BITrtn.BIT_UDC = CStr(Math.Abs(STATUS_BIT(13)))
        Fskj_mt_BITrtn.BIT_TIM = CStr(Math.Abs(STATUS_BIT(14)))
        Fskj_mt_BITrtn.BIT_CHC = CStr(Math.Abs(STATUS_BIT(15)))
        Fskj_mt_BITrtn.BIT_FIL2 = CStr(Math.Abs(STATUS_BIT(16)))
        Fskj_mt_BITrtn.BIT_FIL3 = CStr(Math.Abs(STATUS_BIT(17)))
        Fskj_mt_BITrtn.BIT_FIL4 = CStr(Math.Abs(STATUS_BIT(18)))
        Fskj_mt_BITrtn.BIT_FIL5 = CStr(Math.Abs(STATUS_BIT(19)))
        Fskj_mt_BITrtn.BIT_ROB = CStr(Math.Abs(STATUS_BIT(20)))
        Fskj_mt_BITrtn.BIT_FIL6 = CStr(Math.Abs(STATUS_BIT(21)))
        Fskj_mt_BITrtn.BIT_FIL7 = CStr(Math.Abs(STATUS_BIT(22)))
        Fskj_mt_BITrtn.BIT_FIL8 = CStr(Math.Abs(STATUS_BIT(23)))
        Fskj_mt_BITrtn.BIT_FIL9 = CStr(Math.Abs(STATUS_BIT(24)))
        Fskj_mt_BITrtn.BIT_FIL10 = CStr(Math.Abs(STATUS_BIT(25)))
        Fskj_mt_BITrtn.BIT_FIL11 = CStr(Math.Abs(STATUS_BIT(26)))
        Fskj_mt_BITrtn.BIT_FIL12 = CStr(Math.Abs(STATUS_BIT(27)))
        Fskj_mt_BITrtn.BIT_FIL13 = CStr(Math.Abs(STATUS_BIT(28)))
        Fskj_mt_BITrtn.BIT_FIL14 = CStr(Math.Abs(STATUS_BIT(29)))
        Fskj_mt_BITrtn.BIT_FIL15 = CStr(Math.Abs(STATUS_BIT(30)))
        Fskj_mt_BITrtn.BIT_FIL16 = CStr(Math.Abs(STATUS_BIT(31)))

        ' �G���[����
        If Err.Number <> 0 Then
            Return Err.Number
        Else
            Return 0
        End If
    End Function
#End Region
#Region "private LoaderMax()"
    '
    ' �@�@�\ : �b�l�s�}�K�W�����̍ł��ԍ��������X���b�g��ԋp����
    '
    ' �߂�l : 1-10 - �i�[���̍ő�̃X���b�g�ԍ� 1-10�ȊO - �ُ�A�}�K�W�����烍�[�h���Ă��Ȃ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : 
    '
    Private Function LoaderMax() As Byte
        Dim nMtRtn As Integer
        Dim slotno As Byte
        Dim slotsum As Integer

        nMtRtn = ChkCmtStat(slotno, slotsum)

        ' �b�l�s�e�[�v�����[�h���Ă��Ȃ�����
        If slotsum = 0 Then
            Return 0
        End If

        ' �b�l�s�e�[�v����Ǎ��܂�Ă���
        If slotsum = 1 Then
            Return slotno
        End If

        Return 0
    End Function
#End Region
#Region "private ChkSlot()"
    '
    ' �@�@�\ : �X���b�g�|�W�V�������딻��
    '
    ' �߂�l : true - ���� false - �ُ�
    '
    ' ��  �� : ARG1 - �X���b�g�ԍ�
    '
    ' ���@�l : 
    '
    Private Function ChkSlot(ByVal siteislotno As Byte) As Boolean
        Dim nRtn As Integer    ' ���A���
        Dim bSlotNo As Byte    ' �X���b�g�ԍ�
        Dim nSlotSum As Integer    ' �b�l�s�e�[�v���v

        nRtn = ChkCmtStat(bSlotNo, nSlotSum)

        ' �荷���Ȃ琳��m�F�͂��Ȃ�
        If nRtn = 1 And bSlotNo = 0 And nSlotSum = 1 Then
            Return True
        End If

        Return True
    End Function
#End Region
#Region "private ChkLabel()"
    '
    ' �@�@�\ : �Ǎ��b�l�s���x���L������
    '
    ' �߂�l : 0 - ���� 0�ȊO - �ُ�
    '
    ' ��  �� : ARG1 - �����f�B���N�g��
    '          ARG2 - �w�b�_�[���x���t���O ARG3 - �Ǎ��t�@�C���t���O ARG4 - �G���h���x���t���O
    '
    ' ���@�l : 
    '
    Private Function ChkLabel(ByVal pathread As String, ByVal mtrtn As Integer, ByVal slotno As Integer, ByVal headchk As Boolean, ByVal filechk As Boolean, ByVal endchk As Boolean) As Integer
        Dim Bbuff(256 - 1) As Byte ' ���x������Ǎ��o�C�g�X�g���[��
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim sBlock As String = String.Empty
        'Dim sBlock As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<
        Dim fl As FileStream

        ' ���x�������Ǎ��񂾏ꍇ
        If headchk = True And filechk = True And endchk = True Then
            LOG.Write("�O�u���b�N�Ǎ�", "")
            PutCMTIni("READ-RESULT", slotno.ToString, "0")
            PutCMTIni("LABEL-EXIST", slotno.ToString, "1")
            Return 0
        End If

        ' ���x���Ȃ���Ǎ��񂾏ꍇ
        If headchk = True And filechk = False And endchk = False Then
            Try
                ' �Ǎ��t�@�C���̃`�F�b�N
                fl = New FileStream(Path.Combine(pathread, GetCMTIni("FILE-NAME", "HEAD")), FileMode.Open)
                Array.Clear(Bbuff, 0, Bbuff.Length)
                fl.Read(Bbuff, 0, 3)
                sBlock = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(Bbuff, 0, 3).Trim
                LOG.Write("strblocklen:", sBlock)
                '***ASTAR SUSUKI 2008.06.13             ***
                'EBCDIC�Ή�                             ***
                If sBlock <> "VOL" Then
                    sBlock = System.Text.Encoding.GetEncoding("IBM290").GetString(Bbuff, 0, 3).Trim
                    LOG.Write("strblocklen:", sBlock)
                End If
                '******************************************
                fl.Close()
                '�w�b�_�[���x�����i�[����Ă�����폜
                If sBlock = "VOL" Then
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
                    PutCMTIni("READ-RESULT", slotno.ToString, "2")
                    PutCMTIni("LABEL-EXIST", slotno.ToString, "0")
                    LOG.Write("ChkLabel", "�t�@�C��������܂���ł���")
                    Return 1
                End If
            Catch ex As Exception
                LOG.Write("strblocklen", sBlock)
                LOG.Write("chkLabel", "��O", ex.Message)
            End Try
            ' ���������g���i�[����Ă����烊�l�[������
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "END"))
            Rename(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"), pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
            PutCMTIni("READ-RESULT", slotno.ToString, "0")
            PutCMTIni("LABEL-EXIST", slotno.ToString, "0")
            Return 0
        End If

        ' �e�[�v�}�[�N�����Ǎ��񂾏ꍇ
        If headchk = False And filechk = True And endchk = False Then
            Try
                ' �Ǎ��t�@�C���̃`�F�b�N
                fl = New FileStream(Path.Combine(pathread, GetCMTIni("FILE-NAME", "READ")), FileMode.Open)
                Array.Clear(Bbuff, 0, Bbuff.Length)
                fl.Read(Bbuff, 0, 3)
                sBlock = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(Bbuff, 0, 3).Trim
                LOG.Write("strblocklen:", sBlock)
                '***ASTAR SUSUKI 2008.06.13             ***
                'EBCDIC�Ή�                             ***
                If sBlock <> "VOL" Then
                    sBlock = System.Text.Encoding.GetEncoding("IBM290").GetString(Bbuff, 0, 3).Trim
                    LOG.Write("strblocklen:", sBlock)
                End If
                '******************************************
                fl.Close()
                '�w�b�_�[���x�����i�[����Ă�����폜
                If sBlock = "VOL" Then
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
                    PutCMTIni("READ-RESULT", slotno.ToString, "2")
                    PutCMTIni("LABEL-EXIST", slotno.ToString, "0")
                    LOG.Write("ChkLabel", "�t�H�[�}�b�g�ُ�ł�")
                    Return 0
                End If
            Catch ex As Exception
                LOG.Write("strblocklen", sBlock)
                LOG.Write("chkLabel", "��O", ex.Message)
            End Try
            ' ���������g���i�[����Ă���
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "END"))
            PutCMTIni("READ-RESULT", slotno.ToString, "0")
            PutCMTIni("LABEL-EXIST", slotno.ToString, "2")
            Return 0
        End If

        ' ��u���b�N���Ǎ��܂Ȃ������ꍇ
        ' ��̃t�@�C�����폜����
        If headchk = False And filechk = False And endchk = False Then
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
            strLogwrite = "�b�l�s���Ƀt�@�C���͂���܂���ł���" & vbCrLf & _
            "�G���[�R�[�h�F" & mtrtn.ToString
            LOG.Write("�b�l�s�ǎ揈��", "�t�@�C���Ȃ�", strLogwrite)
            PutCMTIni("READ-RESULT", slotno.ToString, "2")
            Return 1
        End If

        If headchk = True And filechk = True And endchk = False Then    ' ��u���b�N�Ǎ��񂾏ꍇ
            strLogwrite = "�G���h�{�����[��������܂���" & vbCrLf & _
            "�G���[�R�[�h�F" & mtrtn.ToString
            LOG.Write("�b�l�s�ǎ揈��", "�t�@�C���j��", strLogwrite)
            PutCMTIni("READ-RESULT", slotno.ToString, "4")
            Return 1
        End If
        Return 1
    End Function
#End Region
#Region "private ChkWriteDrFl()"
    '
    ' �@�@�\ : �b�l�s�e�[�v���[�h�A�����t�@�C������
    '
    ' �߂�l : true - ���� false - �ُ�
    '
    ' ��  �� : ARG1 - �����f�B���N�g�� ARG2 - �X���b�g�ԍ� ARG3 - �h���C�o�[�t���O
    '          ARG4 - �w�b�_�[���x���t���O ARG5 - �����t�@�C���t���O ARG6 - �G���h���x���t���O
    '
    ' ���@�l : 
    '
    Private Function ChkWriteDrFl(ByVal path As String, ByVal slotno As Byte, ByVal drive As Boolean, ByRef headlabel As Boolean, ByRef writefile As Boolean, ByRef endlabel As Boolean) As Boolean
        ' �Ώۃf�B���N�g������
        If Not Directory.Exists(path) Then
            strLogwrite = "�f�B���N�g���Ȃ�" & vbCrLf & _
            "�f�B���N�g���F" & path
            LOG.Write("�Ώۃf�B���N�g������", "���s", strLogwrite)
            Return False
        End If

        ' �����w�b�_�[���x������
        headlabel = File.Exists(path & "\" & GetCMTIni("FILE-NAME", "HEAD"))

        ' �����t�@�C������
        writefile = File.Exists(path & "\" & GetCMTIni("FILE-NAME", "WRITE"))

        ' �����G���h���x������
        endlabel = File.Exists(path & "\" & GetCMTIni("FILE-NAME", "END"))

        LOG.Write("head:" & headlabel.ToString, "file:" & writefile.ToString, "end:" & endlabel.ToString)

        If drive Then
            ' �b�l�s���u�Ƀe�[�v�����[�h����Ă���
            If headlabel = True And writefile = True And endlabel = True Then
                ' �w�b�_�[���x������A�t�@�C������A�G���h���x������
                Return True
            ElseIf headlabel = False And writefile = True And endlabel = False Then
                '�t�@�C���݂̂���
                Return True
            Else
                ' ����ȊO�̓G���[
                If Not headlabel Then
                    strLogwrite = "�����w�b�_�[���x���Ȃ�" & vbCrLf & _
                                "�t�@�C���F" & path & "\" & GetCMTIni("FILE-NAME", "HEAD")
                    LOG.Write("�����t�@�C������", "���s", strLogwrite.ToString)
                End If
                If Not writefile Then
                    strLogwrite = "�����t�@�C���Ȃ�" & vbCrLf & _
                                "�t�@�C���F" & path & "\" & GetCMTIni("FILE-NAME", "WRITE")
                    LOG.Write("�����t�@�C������", "���s", strLogwrite.ToString)
                End If
                If Not endlabel Then
                    strLogwrite = "�����G���h���x���Ȃ�" & vbCrLf & _
                    "�t�@�C���F" & path & "\" & GetCMTIni("FILE-NAME", "END")
                    LOG.Write("�����t�@�C������", "���s", strLogwrite.ToString)
                End If
                PutCMTIni("WRITE-RESULT", slotno.ToString, "2")
                Return False
            End If
        Else
            ' �b�l�s�e�[�v�����[�h���Ă��Ȃ�
            LOG.Write("�����t�@�C������", "���s", "�b�l�s�e�[�v�����[�h����Ă��Ȃ�")

            '  �w�b�_�[���x���Ȃ�
            If Not headlabel Then
                strLogwrite = "�����w�b�_�[���x���Ȃ�" & vbCrLf & _
                            "�t�@�C���F" & path & "\" & GetCMTIni("FILE-NAME", "HEAD")
                LOG.Write("�����t�@�C������", "���s", strLogwrite.ToString)
            End If
            ' �t�@�C���Ȃ�
            If Not writefile Then
                strLogwrite = "�����t�@�C���Ȃ�" & vbCrLf & _
                            "�t�@�C���F" & path & "\" & GetCMTIni("FILE-NAME", "WRITE")
                LOG.Write("�����t�@�C������", "���s", strLogwrite.ToString)
            End If
            ' �G���h���x���Ȃ�
            If Not endlabel Then
                strLogwrite = "�����G���h���x���Ȃ�" & vbCrLf & _
                "�t�@�C���F" & path & "\" & GetCMTIni("FILE-NAME", "END")
                LOG.Write("�����t�@�C������", "���s", strLogwrite.ToString)
            End If
            Return False
        End If
    End Function
#End Region
#Region "private ChkReadDrFl()"
    '
    ' �@�@�\ : ��s�t�@�C���L������
    '
    ' �߂�l : true - ���� false - �ُ�
    '
    ' ��  �� : ARG1 - �����f�B���N�g�� ARG2 - �w�b�_�[���x���t���O
    '          ARG3 - �Ǎ��t�@�C���t���O ARG4 - �G���h���x���t���O
    '
    ' ���@�l : 
    '
    Private Function ChkReadDrFl(ByVal path As String, ByVal slotno As Byte) As Boolean
        ' �Ώۃf�B���N�g������
        If Not Directory.Exists(path) Then
            strLogwrite = "�f�B���N�g���Ȃ�" & vbCrLf & _
            "�f�B���N�g���F" & path
            LOG.Write("�Ώۃf�B���N�g������", "���s", strLogwrite)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If

        ' ��s�t�@�C������
        If File.Exists(path & "\" & GetCMTIni("FILE-NAME", "READ")) Then
            strLogwrite = "��s�t�@�C���L��" & vbCrLf & _
            "�t�@�C���F" & path
            LOG.Write("��s�t�@�C������", "���s", strLogwrite.ToString)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If

        ' ��s�w�b�_�[���x������
        If File.Exists(path & "\" & GetCMTIni("FILE-NAME", "HEAD")) Then
            strLogwrite = "��s�w�b�_�[���x���L��" & vbCrLf & _
            "�t�@�C���F" & path
            LOG.Write("��s�t�@�C������", "���s", strLogwrite.ToString)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If

        ' ��s�G���h���x������
        If File.Exists(path & "\" & GetCMTIni("FILE-NAME", "END")) Then
            strLogwrite = "��s�G���h���x���L��" & vbCrLf & _
            "�t�@�C���F" & path
            LOG.Write("��s�t�@�C������", "���s", strLogwrite.ToString)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If
        Return True
    End Function
#End Region
#Region "private ChkKirokumitsudo()"
    '
    ' �b�l�s�e�[�v�̋L�^���x�𑪒肷��
    '
    Private Function ChkKirokumitsudo() As Boolean
        Dim nMtRtn, nRtn As Integer
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        '�L�^���x����
        Select Case bit_status.BIT_DEN0
            Case 1
                If bit_status.BIT_DEN1 = 1 Then
                    strMemMitsudo = "6250BPI"
                Else
                    strMemMitsudo = "1600BPI"
                End If
            Case 0
                If bit_status.BIT_DEN1 = 1 Then
                    strMemMitsudo = "3200BPI"
                Else
                    strMemMitsudo = "800BPI"
                End If
            Case Else
                strLogwrite = "�X�e�[�^�X�ω��G���[(�L�^���x����)" & vbCrLf & _
                "�X�e�[�^�X�F" & bit_status.BIT_DEN0 & bit_status.BIT_DEN1 & _
                "�G���[�R�[�h�F" & nMtRtn.ToString
                LOG.Write("�L�^���x����", "���s", strLogwrite)
                strLogwrite = Setbit_statusLog(nMtRtn, nRtn)
                LOG.Write(strLogwrite, "")
                Return False
        End Select
        Return True
    End Function
#End Region
#Region "private LogCmtStat()"
    ' ���݂̂b�l�s�X�e�[�^�X�����O�ɏo��
    Private Function LogCmtStat() As Integer
        Dim nRtn As Integer = 0
        Dim nMtRtn As Integer
        Dim cflg As Byte = 0

        Call CmtInfoInit()
        'Call CslInfoInit()

        'mtinit
        nMtRtn = FuncMtinit()
        If nMtRtn <> 0 Then
            nRtn = nMtRtn
        End If

        'mtstat
        nMtRtn = FuncMtStat()
        If nMtRtn <> 0 Then
            nRtn = nMtRtn
        End If

        nRtn = MtCHGStatus(nMtRtn, bit_status)

        strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
        LOG.Write("LogCmtStat()", "CMTINFO", strLogwrite)

        strLogwrite = Setbit_statusLog(nMtRtn, nRtn)
        LOG.Write("LogCmtStat()", "bit_status", strLogwrite)

        Return nMtRtn
    End Function
#End Region
#Region "private SetCmtInfoLog()"
    ' CMTINFO�̃��O���쐬
    Private Function SetCmtInfoLog(ByVal nMtRtn As Integer, ByVal nRtn As Integer) As String
        Return "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & " nRtn:" & String.Format("{0}:{0:X}", nRtn) & " UnitNo:" & CMTINFO.UnitNo & _
            " HostNo:" & CMTINFO.HostNo & " TargetNo:" & CMTINFO.TargetNo & _
            " Vender:" & CMTINFO.Vender & " Product:" & CMTINFO.Product & _
            " VERSION:" & CMTINFO.VERSION & " reserve:" & CMTINFO.Reserve
    End Function
#End Region
#Region "private SetCslDataLog()"
    ' LOADERDATA�̃��O���쐬
    'Private Function SetCslDataLog(ByVal nMtRtn As Integer, ByVal nRtn As Integer) As String
    '    Return "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & " nRtn:" & String.Format("{0}:{0:X}", nRtn) & _
    '        " CslModeCode:" & LOADERDATA.CslModeCode & " Drive:" & LOADERDATA.Drive & " SlotPosition:" & LOADERDATA.SlotPosition & _
    '        " Slot1:" & LOADERDATA.Slot1 & " Slot2:" & LOADERDATA.Slot2 & " Slot3:" & LOADERDATA.Slot3 & " Slot4:" & LOADERDATA.Slot4 & _
    '        " Slot5:" & LOADERDATA.Slot5 & " Slot6:" & LOADERDATA.Slot6 & " Slot7:" & LOADERDATA.Slot7 & " Slot8:" & LOADERDATA.Slot8 & _
    '        " Slot9:" & LOADERDATA.Slot9 & " Slot10:" & LOADERDATA.Slot10
    'End Function
#End Region
#Region "private Setbit_statusLog()"
    ' bit_status�̃��O���쐬
    Private Function Setbit_statusLog(ByVal nMtRtn As Integer, ByVal nRtn As Integer) As String
        Return "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & " nRtn:" & String.Format("{0}:{0:X}", nRtn) & _
            " TMK:" & bit_status.BIT_TMK & " EOT:" & bit_status.BIT_EOT & " BOT:" & bit_status.BIT_BOT & _
            " DEN0:" & bit_status.BIT_DEN0 & " DEN1:" & bit_status.BIT_DEN1 & " CSL:" & bit_status.BIT_CSL & _
            " PRO:" & bit_status.BIT_PRO & " FIL1:" & bit_status.BIT_FIL1 & " DTE:" & bit_status.BIT_DTE & _
            " HDE:" & bit_status.BIT_HDE & " NRDY:" & bit_status.BIT_NRDY & " ILC:" & bit_status.BIT_ILC & _
            " SCE:" & bit_status.BIT_SCE & " UDC:" & bit_status.BIT_UDC & " TIM:" & bit_status.BIT_TIM & _
            " CHC:" & bit_status.BIT_CHC & " FIL2:" & bit_status.BIT_FIL2 & " FIL3:" & bit_status.BIT_FIL3 & _
            " FIL4:" & bit_status.BIT_FIL4 & " FIL5:" & bit_status.BIT_FIL5 & " ROB:" & bit_status.BIT_ROB & _
            " FIL6:" & bit_status.BIT_FIL6 & " FIL7:" & bit_status.BIT_FIL7 & " FIL8:" & bit_status.BIT_FIL8 & _
            " FIL9:" & bit_status.BIT_FIL9 & " FIL10:" & bit_status.BIT_FIL10 & " FIL11:" & bit_status.BIT_FIL11 & _
            " FIL12:" & bit_status.BIT_FIL12 & " FIL13:" & bit_status.BIT_FIL13 & " FIL14:" & bit_status.BIT_FIL14 & _
            " FIL15:" & bit_status.BIT_FIL15 & " FIL16:" & bit_status.BIT_FIL16
    End Function
#End Region

    '
    ' �@�@�\ : �b�l�s���u�̐ڑ���Ԃ��擾
    '
    ' �߂�l : TRUE - ����  FALSE - �ُ�
    '
    ' ��  �� : �Ȃ�
    '
    ' ���@�l : �ŏ��ɌĂяo���K�v����
    ' 
    Public Function ChkLoader() As Boolean
        Dim nMtRtn As Integer    ' �b�l�s�֐��̕��A�l

        'LOG.SyoriMei = "ChkLoader"

        ' �ڑ��m�F
        nMtRtn = mtinit(CMTINFO)
#If Not DEBUGCMT Then
        If nMtRtn = 0 Or nMtRtn = &HFFFFFFFF Then
            LOG.Write("�b�l�s���u���ڑ���Ԃɂ���܂���", "mtinit:" & nMtRtn.ToString)
            Return False
        End If
#End If
        Return True
    End Function

    '#Region "�P�̏���"

    '    ' 
    '    ' �P�̃e�[�v�}�[�N����
    '    '
    '    ' ���A�l�F �f�o�C�X�X�e�[�^�X
    '    ' 

    '    Public Function writeTMK() As Integer
    '        Dim nRtn As Integer    ' �֐����A�l
    '        Dim nMtRtn As Integer    ' �b�l�s�֐����A�l


    '        ' �b�l�s���u��������
    '        nRtn = FuncMtinit()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        ' �b�l�s���u�X�e�[�^�X����
    '        nRtn = FuncMtStat()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        nRtn = FuncMtOnline()
    '        If nRtn <> 0 Then
    '            Return 1

    '        End If

    '        nMtRtn = mtwtmk(CMTINFO.UnitNo)

    '        Call LogCmtStat()

    '        Return nMtRtn

    '    End Function

    '    ' 
    '    ' �P�̃_�u���e�[�v�}�[�N����
    '    '
    '    ' ���A�l�F �f�o�C�X�X�e�[�^�X
    '    ' 

    '    Public Function writeWTMK() As Integer
    '        Dim nRtn As Integer    ' �֐����A�l
    '        Dim nMtRtn As Integer    ' �b�l�s�֐����A�l


    '        ' �b�l�s���u��������
    '        nRtn = FuncMtinit()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        ' �b�l�s���u�X�e�[�^�X����
    '        nRtn = FuncMtStat()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        nMtRtn = mtwmtmk(CMTINFO.UnitNo)

    '        Call LogCmtStat()

    '        Return nMtRtn

    '    End Function


    '    ' 
    '    ' �P�̃_�u���e�[�v�}�[�N����
    '    '
    '    ' ���A�l�F �f�o�C�X�X�e�[�^�X
    '    ' 

    '    Public Function writeBlock(ByVal slotno As Byte, ByVal filename As String) As Integer
    '        Dim nRtn As Integer    ' �֐����A�l
    '        Dim nMtRtn As Integer    ' �b�l�s�֐����A�l
    '        Dim sPath As String
    '        Dim fl As FileStream
    '        Dim buff(&HF000 - 1) As Byte ' �����o�C�g�X�g���[��
    '        Dim count As Integer    ' �t�@�C���ǂݍ��ݎ��̕�����


    '        sPath = GetCMTIni("WRITE-DIRECTORY", slotno.ToString)


    '        ' �b�l�s���u��������
    '        nRtn = FuncMtinit()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        ' �b�l�s���u�X�e�[�^�X����
    '        nRtn = FuncMtStat()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        Try
    '            If Not File.Exists(Path.Combine(sPath, filename)) Then
    '                LOG.Write("��������", "���s", Path.Combine(sPath, filename) & "������܂���")
    '                PutCMTIni("WRITE-RESULT", slotno.ToString, "4")
    '                Return 1
    '            End If

    '            ' �w�b�_�[���x������
    '            fl = New FileStream(Path.Combine(sPath, filename), FileMode.Open)
    '            ' �o�b�t�@�̈�N���A
    '            Array.Clear(buff, 0, buff.Length)
    '            ' �����t�@�C����Ǎ���
    '            count = fl.Read(buff, 0, buff.Length)
    '            Do While count > 0
    '                ' ��u���b�N����
    '                LOG.Write("", "�ʉ�")
    '                nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
    '                LOG.Write("����", "nMtRtn:" & nMtRtn.ToString)

    '                LOG.Write("mtwblock", "�ԋp�t�@�C������", "�����o�b�t�@��:" & count.ToString)
    '                '�f�o�C�X�X�e�[�^�X�擾
    '                nRtn = MtCHGStatus(nMtRtn, bit_status)
    '                '�G���[����
    '                If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
    '                    '���O�o��
    '                    LOG.Write("�m�����x���ԋp�t�@�C�������ُ킪�N���܂���", _
    '                            "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & _
    '                            " DTE:" & bit_status.BIT_DTE & " HDE:" & bit_status.BIT_HDE, strLogwrite)
    '                    PutCMTIni("WRITE-RESULT", slotno.ToString, "4")
    '                    Call LogCmtStat()
    '                    fl.Close()
    '                    Exit Do
    '                End If
    '                ' �o�b�t�@�̈���N���A
    '                Array.Clear(buff, 0, buff.Length)
    '                '�ԋp�t�@�C�����o�b�t�@�ɓǍ���
    '                count = fl.Read(buff, 0, buff.Length)
    '            Loop
    '            fl.Close()

    '        Catch ex As Exception
    '            LOG.Write("����������O", "�w�b�_�[���x��", ex.Message)
    '            Call LogCmtStat()
    '            PutCMTIni("WRITE-RESULT", slotno.ToString, "4")
    '            Return False
    '        End Try

    '        Call LogCmtStat()

    '        Return nMtRtn

    '    End Function
    '#End Region

    Public Sub New()
        mnBlockSize = -1
    End Sub
End Class