Option Strict On
Option Explicit On

Imports System

' �ב֐U���f�[�^�쐬���� ���C�����W���[��
Module ModMain
    '
    ' �@�\�@ �F �ב֐U���f�[�^�쐬 ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        ' �߂�l
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG
        Dim FurikomiDataCreateClass As New ClsFurikomiDataCreate

        Try
            ELog.Write("�J�n")

            '��������
            If FurikomiDataCreateClass.Init(CmdArgs) = False Then
                Return -1
            End If

            ' �又��
            ret = FurikomiDataCreateClass.Main(CmdArgs(0))
            '�t���O���
            FurikomiDataCreateClass.ReturnFlg()
            If ret <> 0 Then
                Return -1
            End If

        Catch ex As Exception
            ELog.Write(ex.Message)
            Return -1
        Finally
            ELog.Write("�I��")
        End Try

        Return ret
    End Function

End Module
