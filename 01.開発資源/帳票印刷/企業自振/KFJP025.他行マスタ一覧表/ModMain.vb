Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' ���s�}�X�^�ꗗ�\���� ���C�����W���[��
Module ModMain

    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFJP025", "���s�}�X�^�ꗗ�\")

    '
    ' �@�\�@ �F ���s�}�X�^�ꗗ�\ ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F
    '
    Function Main(ByVal arg() As String) As Integer
        Try
            ' ���s�}�X�^�ꗗ�\�o�͏���
            Dim TAKOUMAST_LIST As New ClsTAKOUMAST_LIST

            ' �߂�l
            Dim ret As Integer

            If arg.Length = 0 Then
                MainLOG.Write("�J�n", "���s", "�����s��")
                Return 1
            End If

            If arg(0).Split(","c).Length <> 2 And arg(0).Split(","c).Length <> 3 Then
                MainLOG.Write("�J�n", "���s", "�����܂�����")
                Return 1
            End If

            Dim Cmd() As String = arg(0).Split(","c)

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("�����J�n")
            'MainLOG.Write("�����J�n", "����", arg(0))
            Try
                '�N���p�����[�^�i�[
                TAKOUMAST_LIST.TORIS_CODE = Cmd(0).PadRight(10).Substring(0, 10)
                TAKOUMAST_LIST.TORIF_CODE = Cmd(1).PadRight(2).Substring(0, 2)
                TAKOUMAST_LIST.ALL_PRINT = Cmd(2)

                MainLOG.ToriCode = Cmd(0) & Cmd(1)
                MainLOG.Write("�p�����[�^�擾", "����", arg(0))
            Catch ex As Exception
                MainLOG.Write("�p�����[�^�擾", "���s", ex.Message)
                Return -1
            End Try

            '���C������
            ret = TAKOUMAST_LIST.Main()
            ELog.Write("�����I��")
            MainLOG.Write("�����I��", "����")

            Return ret

        Catch ex As Exception
            MainLOG.Write("�z��O�̃G���[���������܂���", "���s", ex.Message & "�F" & ex.StackTrace)
            Return -1
        End Try
    End Function

End Module
