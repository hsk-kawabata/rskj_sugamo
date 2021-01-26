Option Strict On
Option Explicit On

Imports System

' �������ݏ��� ���C�����W���[��
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFS010", "�U���˗��f�[�^��������")
    Public Const msgTitle As String = "�U���˗��f�[�^��������(KFS010)"

    '=================================================================
    '�@�\�@�F�������� ���C������
    '�����@�FARG1 - �N���p�����[�^
    '�߂�l�F0 - ���� �C 0 �ȊO - �ُ�
    '���l�@�F
    '�쐬�@�F2009/09/07
    '�X�V�@�F
    '=================================================================
    Function Main(ByVal CmdArgs() As String) As Integer
        ' �߂�l
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "(���C������)�J�n", "����")
            'BatchLog.Write("0000000000-00", "00000000", "(���C������)�J�n", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            '------------------------------------------------------------
            '�p�����^�`�F�b�N
            '------------------------------------------------------------
            If CmdArgs.Length = 0 Then
                ELog.Write("�����J�n:�����Ȃ�")
            Else
                ELog.Write("�����J�n:" & CmdArgs(0))
            End If

            Console.WriteLine("�����J�n")

            If CmdArgs.Length = 0 Then
                Return -100
            End If

            Select Case CmdArgs(0).Split(","c).Length
                Case 7, 8, 9, 13
                Case Else
                    '�p�����[�^�ُ�
                    ret = -100
            End Select

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ''------------------------------------------------------------
            ''1�b�Ԋu�A10���Ԃ܂œ�d�N���Ď�����
            ''------------------------------------------------------------
            'CASTCommon.ModPublic.WatchAndWaitProcess(1000, 600)
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '------------------------------------------------------------
            '�����又�����s
            '------------------------------------------------------------
            Dim TourokuClass As New ClsTouroku

            ret = TourokuClass.Main(CmdArgs(0))
            Console.WriteLine("�����I��")
            ELog.Write("�����I��:" & CmdArgs(0))

            Return ret

        Catch ex As Exception
            BatchLog.Write("", "0000000000-00", "00000000", "(���C������)", "���s", ex.Message)
            Return -999
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "(���C������)�I��", "����")
            'BatchLog.Write("", "0000000000-00", "00000000", "(���C������)�I��", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
        End Try
    End Function



End Module
