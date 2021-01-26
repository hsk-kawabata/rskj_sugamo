Option Strict On
Option Explicit On

Imports System

' �����U�����ו\ ���C�����W���[��
Module ModMain

    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFSP013", "�����U�����ו\")

    '
    ' �@�\�@ �F �����U�����ו\ ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F
    '
    Function Main(ByVal arg() As String) As Integer
        ' �߂�l
        Dim ret As Integer

        Dim Soufurimei As New ClsSoufurimei

        Try
            MainLOG.Write("(��������)�J�n", "����")

            If arg.Length <> 1 Then
                MainLOG.Write("(��������)", "���s", "�����܂�����")
                Return 1
            End If

            ' �N���p�����[�^�i�[
            Dim Param() As String = arg(0).Split(","c)
            If Param.Length = 3 Then
                Soufurimei.TORI_CODE = Param(1)
                Soufurimei.FURI_DATE = Param(2)
            Else
                MainLOG.Write("(��������)", "���s", "�p�����[�^���s���ł��B" & arg(0))
                Return -100
            End If

            MainLOG.UserID = Param(0)
            MainLOG.ToriCode = Soufurimei.TORI_CODE
            MainLOG.FuriDate = Soufurimei.FURI_DATE
            MainLOG.Write("�p�����[�^�擾", "����", "�R�}���h���C������:" & arg(0))

        Catch ex As Exception
            MainLOG.Write("�p�����[�^�擾", "���s", ex.Message)
            Return -100
        End Try

        Dim ELog As New CASTCommon.ClsEventLOG
        ELog.Write("�����J�n")

        ' ���C������
        ret = Soufurimei.Main()

        ELog.Write("�����I��")

        Return ret
    End Function

End Module
