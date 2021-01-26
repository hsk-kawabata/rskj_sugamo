Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' �U�֕s�\���ו\���� ���C�����W���[��
Module ModMain
    '2009/12/29 �萔���v�Z�p ==========
    '2013/11/14 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
    'Public Const strTESUU_TABLE_FILE_NAME As String = "KFJMAST010_�U���萔���ID.TXT"
    'Public TXT_FOLDER As String
    '2013/11/14 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<
    Structure TESUU_TABLE
        Dim strKIJYUN_ID_CODE As String         '�萔���ID
        Dim strKIJYUN_ID_TEXT As String         '�萔���ID��
        Dim lng10000UNDER_JITEN As Long         '�萔���i���X�P���~�����j
        Dim lng30000UNDER_JITEN As Long         '�萔���i���X�P���~�ȏ�R���~�����j
        Dim lng30000OVER_JITEN As Long          '�萔���i���X�R���~�ȏ�j
        Dim lng10000UNDER_HONSITEN As Long      '�萔���i�{�x�X�P���~�����j
        Dim lng30000UNDER_HONSITEN As Long      '�萔���i�{�x�X�P���~�ȏ�R���~�����j
        Dim lng30000OVER_HONSITEN As Long       '�萔���i�{�x�X�R���~�ȏ�j
        Dim lng10000UNDER_TAKOU As Long         '�萔���i���s�R���~�����j
        Dim lng30000UNDER_TAKOU As Long         '�萔���i���s�P���~�ȏ�R���~�ȏ�j
        Dim lng30000OVER_TAKOU As Long          '�萔���i���s�R���~�ȏ�j
    End Structure
    ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
    ' �萔���e�[�u���̌�����100���܂łɕύX
    'Public TESUU_TABLE_DATA(10) As TESUU_TABLE
    Public TESUU_TABLE_DATA(100) As TESUU_TABLE
    ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END
    '=================================

    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFJP017", "�U�֕s�\���ו\")

    '
    ' �@�\�@ �F �U�֕s�\���ו\ ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F
    '
    Function Main(ByVal arg() As String) As Integer
        '*** �C�� mitsu 2009/04/10 ���O�@�\���� ***
        Try
            '**************************************

            ' �U�֕s�\���ו\�o�͏���
            Dim FrikaekeFunoumeisai As New ClsFrikaeFnoumeisai

            ' �߂�l
            Dim ret As Integer

            If arg.Length = 0 Then
                MainLOG.Write("�J�n", "���s", "�����s��")
                Return 1
            End If

            '2012/06/30 �W���Ł@WEB�`���Ή�------------------------------------------------->
            If arg(0).Split(","c).Length <> 3 And arg(0).Split(","c).Length <> 4 And arg(0).Split(","c).Length <> 5 Then
                'If arg(0).Split(","c).Length <> 3 And arg(0).Split(","c).Length <> 4 Then
                '---------------------------------------------------------------------------<
                MainLOG.Write("�J�n", "���s", "�����܂�����")
                Return 1
            End If

            Dim Cmd() As String = arg(0).Split(","c)

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("�����J�n")
            '*** �C�� mitsu 2009/04/10 ���O�@�\���� ***
            'MainLOG.Write("�����J�n", "����", arg(0))
            '******************************************
            Try
                ' �N���p�����[�^�i�[
                FrikaekeFunoumeisai.TORIS_CODE = Cmd(0).PadRight(10).Substring(0, 10)
                FrikaekeFunoumeisai.TORIF_CODE = Cmd(0).PadRight(2).Substring(10)
                FrikaekeFunoumeisai.FURI_DATE = Cmd(1)
                FrikaekeFunoumeisai.FUNO_FLG = Cmd(2)
                If Cmd.Length = 4 Then
                    FrikaekeFunoumeisai.PRINTERNAME = Cmd(3)
                End If
                '2012/06/30 �W���Ł@WEB�`���Ή�------------->
                If Cmd.Length = 5 Then
                    FrikaekeFunoumeisai.INVOKE_KBN = Cmd(4)
                End If
                '-------------------------------------------<
                MainLOG.ToriCode = Cmd(0)
                MainLOG.FuriDate = Cmd(1)
                MainLOG.Write("�p�����[�^�擾", "����")
            Catch ex As Exception
                MainLOG.Write("�p�����[�^�擾", "���s", ex.Message)
                Return -1
            End Try

            '2013/11/14 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
            ''2009/12/29 �ǉ� ===========================
            'TXT_FOLDER = CASTCommon.GetFSKJIni("COMMON", "TXT")
            'If TXT_FOLDER = "err" Then
            '    MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - TXT)")
            '    Return 1
            'End If
            ''===========================================
            '2013/11/14 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<

            ' ���C������
            ret = FrikaekeFunoumeisai.Main()
            ELog.Write("�����I��")
            '*** �C�� mitsu 2009/04/10 ���O�@�\���� ***
            MainLOG.Write("�����I��", "����")
            '******************************************

            Return ret

            '*** �C�� mitsu 2009/04/10 ���O�@�\���� ***
        Catch ex As Exception
            MainLOG.Write("�z��O�̃G���[���������܂���", "���s", ex.Message & "�F" & ex.StackTrace)
            Return -1
        End Try
        '**********************************************
    End Function

End Module
