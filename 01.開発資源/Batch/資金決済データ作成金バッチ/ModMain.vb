' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
Imports System
Imports System.IO
Imports System.Diagnostics

' �������σf�[�^�쐬���� ���C�����W���[��
Module ModMain
    ' ���O�����N���X



    '
    ' �@�\�@ �F �������σf�[�^�쐬 ���C������
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
        Dim ClsKessaiDataCreateKinBatch As New ClsKessaiDataCreateKinBatch
        
        Try
            ELog.Write("�J�n")

            '��������
            If ClsKessaiDataCreateKinBatch.KessaiInit(CmdArgs) = False Then
                Return -1
            End If

            ' �又��
            ret = ClsKessaiDataCreateKinBatch.Main(CmdArgs(0))
            '�t���O���
            ClsKessaiDataCreateKinBatch.ReturnFlg()
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
' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END
