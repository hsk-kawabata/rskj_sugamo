Imports System
Imports System.IO
Imports System.Diagnostics

'=====================================================
'�N�����l�ʐU�����ו\�@���C�����W���[��
'=====================================================
Module ModMain

    ' ���O�����N���X
    Public BatchLOG As New CASTCommon.BatchLOG("KFJP039", "�N�����l�ʐU�����ו\")

    '=======================================================================
    ' �@�\�@ �F �N�����l�ʐU�����ו\�@���C������
    ' �����@ �F ARG1 - �N���p�����[�^
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    ' ���l�@ �F 
    ' �쐬�� �F 2009/10/02
    ' �X�V�� �F
    '=======================================================================
    Function Main(ByVal arg() As String) As Integer
        Dim ret As Integer
        Dim NenUkeMei As New ClsNenUkeMei

        Try
            If arg.Length <> 1 Then
                BatchLOG.Write("(��������)", "���s", "�����܂�����")
                Return 1
            End If

            '------------------------------------------
            '�N���p�����[�^�擾�^�`�F�b�N
            '------------------------------------------
            Dim Param() As String = arg(0).Split(","c)

            If Param.Length = 3 Then
                BatchLOG.UserID = Param(0)                                          '���O�C����(���O�����p)
                NenUkeMei.ToriSCode = Param(1).Substring(0, 10)                     '������R�[�h
                NenUkeMei.ToriFCode = Param(1).Substring(10, 2)                     '����敛�R�[�h
                BatchLOG.ToriCode = NenUkeMei.ToriSCode & "-" & NenUkeMei.ToriFCode '�����R�[�h(���O�����p)
                NenUkeMei.FuriDate = Param(2)                                       '�U�֓�
                BatchLOG.FuriDate = Param(2)                                        '�U�֓�(���O�����p)
                BatchLOG.Write("(��������)", "����", "�p�����[�^:" & arg(0))
            Else
                BatchLOG.Write("(��������)", "���s", "�p�����[�^���s���ł��B" & Param.ToString)
            End If

            '------------------------------------------
            '���C������
            '------------------------------------------
            ret = NenUkeMei.Main()
            Return ret

        Catch ex As Exception
            BatchLOG.Write("�p�����[�^�擾", "���s", ex.Message)
            Return -1

        End Try
    End Function

End Module
