Imports System
Imports System.IO
Imports System.Diagnostics

' �������σf�[�^�쐬���� ���C�����W���[��
Module ModMain

    ' �@�\�@ �F ���U�쐬 ���C������
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
        Dim JikeiCreateClass As New ClsJifkeiCreate

        Try
            ELog.Write("�J�n")

            '��������
            If JikeiCreateClass.JikeiInit(CmdArgs) = False Then
                Return -1
            End If

            ' �又��
            ret = JikeiCreateClass.Main(CmdArgs(0))
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
