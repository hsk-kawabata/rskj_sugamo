Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' �U�֕s�\���R�ʏW�v�\���� ���C�����W���[��
Module ModMain

    ' ���O�����N���X
    Private MainLOG As New CASTCommon.BatchLOG("KFJP052", "�U�֕s�\���R�ʏW�v�\")

    '
    ' �@�\�@ �F �U�֕s�\���R�ʏW�v�\ ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F
    '
    Function Main(ByVal arg() As String) As Integer

        Try
            ' �U�֕s�\���R�ʏW�v�\�o�͏���
            Dim FurikaeFunouSyuukei As New ClsFurikaeFnouSyuukei

            ' �߂�l
            Dim ret As Integer

            If arg.Length = 0 Then
                MainLOG.Write("�J�n", "���s", "�����s��")
                Return 1
            End If

            '2012/06/30 �W���Ł@WEB�`���Ή�----------------------------------------------->
            If arg(0).Split(","c).Length <> 2 And arg(0).Split(","c).Length <> 3 And arg(0).Split(","c).Length <> 4 Then
                'If arg(0).Split(","c).Length <> 2 And arg(0).Split(","c).Length <> 3 Then
                '-------------------------------------------------------------------------<
                MainLOG.Write("�J�n", "���s", "�����܂�����")
                Return 1
            End If

            Dim Cmd() As String = arg(0).Split(","c)

            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("�����J�n")
            MainLOG.Write("�����J�n", "����", arg(0))

            Try
                ' �N���p�����[�^�i�[
                FurikaeFunouSyuukei.TORIS_CODE = Cmd(0).PadRight(10).Substring(0, 10)
                FurikaeFunouSyuukei.TORIF_CODE = Cmd(0).PadRight(2).Substring(10)
                FurikaeFunouSyuukei.FURI_DATE = Cmd(1)
                If Cmd.Length = 3 Then
                    FurikaeFunouSyuukei.PRINTERNAME = Cmd(2)
                End If
                '2012/06/30 �W���Ł@WEB�`���Ή�----------->
                If Cmd.Length = 4 Then
                    FurikaeFunouSyuukei.INVOKE_KBN = Cmd(3)
                End If
                '-----------------------------------------<
                MainLOG.ToriCode = Cmd(0)
                MainLOG.FuriDate = Cmd(1)
                MainLOG.Write("�p�����[�^�擾", "����", arg(0))
            Catch ex As Exception
                MainLOG.Write("�p�����[�^�擾", "���s", ex.Message)
                Return -1
            End Try

            ' ���C������
            ret = FurikaeFunouSyuukei.Main()
            MainLOG.Write("STEP�o��", "", "ret = Frikaekekkamei...") '2009/04/10
            ELog.Write("�����I��")
            MainLOG.Write("�����I��", "����")

            Return ret

        Catch ex As Exception
            MainLOG.Write("�z��O�̃G���[���������܂���", "���s", ex.Message & "�F" & ex.StackTrace)
            Return -1
        End Try
    End Function

End Module
