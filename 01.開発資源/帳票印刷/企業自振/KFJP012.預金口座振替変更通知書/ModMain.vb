Imports System
Imports System.IO
Imports System.Diagnostics
' �a�������U�֕ύX�ʒm��������C�����W���[��
Module ModMain

    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFJP012", "�a�������U�֕ύX�ʒm������o�b�`")
    '
    ' �@�\�@ �F �a�������U�֕ύX�ʒm����� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F
    '
    Public Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Public LW As LogWrite
    Function Main(ByVal arg() As String) As Integer
        Dim KFJP012 As New ClsHenkouTuuti    ' �N���X�錾
        Dim nRtn As Integer    ' ���A�l
        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������)�J�n", "����", "")

            If arg.Length <> 1 Then
                MainLOG.Write("�J�n", "���s", "�p�����[�^�Ȃ�")
                Return -100
            End If

            Console.WriteLine("�����J�n")
            Try
                ' �N���p�����[�^�i�[
                Dim Param() As String = arg(0).Split(","c)
                LW.UserID = Param(0)
                KFJP012.FURI_DATE = Param(1)
                If Param.Length = 2 Then
                End If
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�p�����[�^�擾", "����", "�R�}���h���C������:" & arg(0))
            Catch ex As Exception
                MainLOG.Write("�p�����[�^�擾", "���s", "�R�}���h���C������:" & arg(0))
                Return -100
            End Try

            ' ���[������C������
            nRtn = KFJP012.PrintHenkouTuuti()
            If nRtn = 0 Then
                MainLOG.Write("�����I��", "����")
            Else
                MainLOG.Write("�����I��", "���s", "���A�l:" & nRtn)
            End If
            Return nRtn
        Catch ex As Exception
            MainLOG.Write("��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������)�I��", "����", "")
        End Try
    End Function
End Module
