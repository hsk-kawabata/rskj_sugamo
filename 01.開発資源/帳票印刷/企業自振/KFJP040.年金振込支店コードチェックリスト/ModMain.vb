Imports System
Imports System.IO
Imports System.Diagnostics

'=====================================================
'�N���U���x�X�R�[�h�`�F�b�N���X�g�@���C�����W���[��
'=====================================================
Module ModMain

    ' ���O�����N���X
    Public BatchLOG As New CASTCommon.BatchLOG("KFJP040", "�N���U���x�X�R�[�h�`�F�b�N���X�g")

    '=======================================================================
    ' �@�\�@ �F �N���U���x�X�R�[�h�`�F�b�N���X�g�@���C������
    ' �����@ �F ARG1 - �N���p�����[�^
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ' ���l�@ �F 
    ' �쐬�� �F 2009/09/29
    ' �X�V�� �F
    '=======================================================================
    Function Main(ByVal arg() As String) As Integer
        Dim ret As Integer
        Dim NenSitCheck As New ClsNenSitCheck

        Try
            If arg.Length <> 1 Then
                BatchLOG.Write("(��������)", "���s", "�����܂�����")
                Return 1
            End If

            '------------------------------------------
            '�N���p�����[�^�擾�^�`�F�b�N
            '------------------------------------------
            Dim Param() As String = arg(0).Split(","c)

            If Param.Length = 2 Then
                BatchLOG.UserID = Param(0)              '���O�C����(���O�����p)
                BatchLOG.FuriDate = Param(1)            '�U�֓�(���O�����p)
                NenSitCheck.FuriDate = Param(1)         '�U�֓�
                BatchLOG.Write("(��������)", "����", "�p�����[�^:" & arg(0))
            Else
                BatchLOG.Write("(��������)", "���s", "�p�����[�^���s���ł��B" & Param.ToString)
            End If

            '------------------------------------------
            '���C������
            '------------------------------------------
            ret = NenSitCheck.Main()
            Return ret

        Catch ex As Exception
            BatchLOG.Write("�p�����[�^�擾", "���s", ex.Message)
            Return -1

        End Try
    End Function

End Module
