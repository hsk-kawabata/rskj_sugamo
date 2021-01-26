Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Diagnostics

' �Ԋҏ��� ���C�����W���[��
Module ModMain
    ' ���O�����N���X
    Private MainLOG As New CASTCommon.BatchLOG("�Ԋ҃f�[�^�쐬")

    '
    ' �@�\�@ �F �Ԋ� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '                  �����R�[�h�i������R�[�h,����敛�R�[�h,�U�֓�,�����V�[�P���X,�W���u�ʔ�
    '                  
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Function Main(ByVal CmdArgs() As String) As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "�Ԋ҃��C������(�J�n)", "����")
            'BatchLog.Write("0000000000-00", "00000000", "���O�C��(�J�n)", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            CASTCommon.TraceLog.AddErrHandler()

            ' �߂�l
            Dim ret As Integer

            Dim ELog As New CASTCommon.ClsEventLOG
            If CmdArgs.Length = 0 Then
                ELog.Write("�����J�n:�����Ȃ� PID:" & Process.GetCurrentProcess.Id)
            Else
                ELog.Write("�����J�n:" & CmdArgs(0) & " PID:" & Process.GetCurrentProcess.Id)
            End If

            If CmdArgs.Length = 0 Then
                MainLOG.Write("�J�n PID:" & Process.GetCurrentProcess.Id, "���s", "�����Ȃ�")
                Return -100
            End If

            Console.WriteLine("�����J�n")

            If CmdArgs.Length >= 2 Then
                CmdArgs(0) = String.Join(","c, CmdArgs).Replace(" "c, "")
            End If

            '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ''* ��d�N�����Ď����� ***
            ''1�b�Ԋu�A20���Ԃ܂œ�d�N���Ď�����
            'Dim waitCnt As Integer = CASTCommon.ModPublic.WatchAndWaitProcess(1000, 1200)
            'If waitCnt > 0 Then
            '    ELog.Write("��d�N�����m�F" & CmdArgs(0) & "�@" & waitCnt & "�b�ҋ@")
            'End If
            '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            Select Case CmdArgs(0).Split(","c).Length
                Case 4
                    '�ꊇ�Ԋҏ����͂��Ȃ�
                    Dim KobetuClass As New ClsHenkan
                    '���C������
                    ret = KobetuClass.Main(CmdArgs(0))
                Case Else
                    MainLOG.Write("�J�n PID:" & Process.GetCurrentProcess.Id, "���s", "�p�����^�擾���s[" & CmdArgs(0) & "]")
                    ret = -100
            End Select

            Console.WriteLine("�����I��")
            ELog.Write("�����I��:" & CmdArgs(0) & " PID:" & Process.GetCurrentProcess.Id)
            Return ret
        Catch ex As Exception

            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write("0000000000-00", "00000000", "�Ԋ҃��C������", "���s", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            CASTCommon.TraceLog.ELogWrite(ex, Diagnostics.EventLogEntryType.Error)
            
            Return -99
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "�Ԋ҃��C������(�I��)", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
        End Try
    End Function
End Module
