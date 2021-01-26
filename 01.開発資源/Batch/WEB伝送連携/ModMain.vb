Option Strict On
Option Explicit On

Imports System

' WEB�`���A�g���� ���C�����W���[��
Module ModMain
    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFW010", "WEB�`���A�g")

    '
    ' �@�\�@ �F WEB�`���A�g ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 1,06,00,000007
    '           
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            ' �߂�l
            Dim ret As Integer

            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "(WEB�`�����C������)�J�n", "����")
            'MainLOG.Write("0000000000-00", "00000000", "(���C������)�J�n", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***


            If CmdArgs.Length = 0 Then
                Return -100
            End If

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ''------------------------------------------------------------
            ''1�b�Ԋu�A10���Ԃ܂œ�d�N���Ď�����
            ''------------------------------------------------------------
            'CASTCommon.ModPublic.WatchAndWaitProcess(1000, 600)
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            Select Case CmdArgs(0).Split(","c).Length
                Case 1, 2, 3            '2012/12/05 WEB�`�� UPD �����Ƌ��z�ǉ�
                    ' �`���A�g
                    Dim DensouClass As New ClsDensouRenkei
                    Dim Param() As String = CmdArgs(0).Split(","c)

                    '�W���u�ʔԎ擾
                    If Param.Length = 2 Then
                        MainLOG.JobTuuban = CInt(Param(1))
                    Else
                        MainLOG.JobTuuban = 0
                    End If

                    '�����Ƌ��z�擾
                    If Param.Length = 3 Then
                        DensouClass.END_KEN = Param(1)
                        DensouClass.END_KIN = Param(2)
                    Else
                        DensouClass.END_KEN = "0"
                        DensouClass.END_KIN = "0"
                    End If

                    DensouClass.JobTuuban = MainLOG.JobTuuban

                    ret = DensouClass.Main(Param(0))

                Case Else
                    ret = -100
            End Select

            Return ret

        Catch ex As Exception
            MainLOG.Write("", "0000000000-00", "00000000", "(���C������)", "���s", ex.Message)

            Return -999
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "(WEB�`�����C������)�I��", "����")
            'MainLOG.Write("", "0000000000-00", "00000000", "(���C������)�I��", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        End Try
    End Function

End Module
