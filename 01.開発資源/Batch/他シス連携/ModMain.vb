Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' ���V�X�A�g���� ���C�����W���[��
Module ModMain
    ' ���O�����N���X
    Private MainLOG As New CASTCommon.BatchLOG("���V�X�A�g", "OTHERSYS")

    '
    ' �@�\�@ �F ���V�X�A�g ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 1,06,00,000007
    '           
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        '*** �C�� mitsu 2009/08/07 �G���[���m�@�\����3 ***
        Try
            CASTCommon.TraceLog.AddErrHandler()
            '*********************************************
            ' �߂�l
            Dim ret As Integer

            Console.WriteLine("�����J�n")

            'Dim ELog As New CASTCommon.ClsEventLOG
            'ELog.Write("�����J�n:" & CmdArgs(0))

            If CmdArgs.Length = 0 Then
                '*** �C�� mitsu 2009/08/07 PID�ǉ� ***
                'MainLOG.Write("�J�n", "���s", "�����Ȃ�")
                MainLOG.Write("�J�n PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "���s", "�����Ȃ�")
                '*************************************
                Return -100
            End If

            '*** �C�� mitsu 2009/08/07 �J�n���O�ǉ� ***
            MainLOG.Write("�����J�n PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "����")
            '******************************************

            Select Case CmdArgs(0).Split(","c).Length
                Case 4
                    ' �ꊇ�A�g
                    Dim IkkatuClass As New ClsIkkatuRenkei

                    ' ���C������
                    ret = IkkatuClass.Main(CmdArgs(0))
                Case 1, 2
                    ' �`���A�g
                    Dim DensouClass As New ClsDensouRenkei
                    Dim Param() As String = CmdArgs(0).Split(","c)
                    If Param.Length = 2 Then
                        MainLOG.JobTuuban = CInt(Param(1))
                    End If

                    ret = DensouClass.Main(Param(0))
                    If ret <> 0 Then
                        Call DensouClass.InsertJOBMASTbyError(Param(0))
                    Else
                        If Param.Length = 2 Then
                            MainLOG.UpdateJOBMASTbyOK("�Ď��s ����")
                        End If
                    End If
                Case Else
                    '*** �C�� mitsu 2009/08/07 PID�ǉ� ***
                    'MainLOG.Write("�J�n", "���s", "�p�����^�擾���s[" & CmdArgs(0) & "]")
                    MainLOG.Write("�J�n PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "���s", "�p�����^�擾���s[" & CmdArgs(0) & "]")
                    '*************************************
                    '*** �C�� mitsu 2009/08/07 ���^�[���R�[�h�ǉ� ***
                    ret = -100
                    '************************************************
            End Select

            Console.WriteLine("�����I��")
            'ELog.Write("�����I��:" & CmdArgs(0))
            '*** �C�� mitsu 2009/08/07 �I�����O�ǉ� ***
            MainLOG.Write("�����I�� PID:" & System.Diagnostics.Process.GetCurrentProcess.Id, "����")
            '******************************************

            Return ret
            '*** �C�� mitsu 2009/08/07 �G���[���m�@�\����3 ***
        Catch ex As Exception
            CASTCommon.TraceLog.ELogWrite(ex, Diagnostics.EventLogEntryType.Error)
            Return -99
        End Try
        '*****************************************************
    End Function

End Module
