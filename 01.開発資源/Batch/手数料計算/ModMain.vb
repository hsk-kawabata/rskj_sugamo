Option Strict On
Option Explicit On

Imports System
Imports System.IO

' �萔���v�Z ���C�����W���[��
Module ModMain
    ' ���O�����N���X
    '       2009.08.27 �R���p�C����ʂ����߃R�����g
    '       Private MainLOG As New CASTCommon.BatchLOG("�萔���v�Z", "TESUU")
    Private MainLOG As New CASTCommon.BatchLOG("�萔���v�Z")
    '
    ' �@�\�@ �F �萔���v�Z ���C������
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    Function Main(ByVal cmdArgs() As String) As Integer
        ' �߂�l
        Dim ret As Integer
        Dim ELog As New CASTCommon.ClsEventLOG

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "�萔���v�Z(�J�n)", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            ELog.Write("�����J�n " & cmdArgs(0))


            Dim FuriDate As String              ' �U�֓�
            Dim ReCalcFlag As Integer = 0       ' �Čv�Z�t���O
            Dim JobTuuban As Integer = 0        ' �W���u�ʔ�

            If cmdArgs.Length = 1 Then
                Dim arr() As String = cmdArgs(0).Split(","c)
                '*** �C�� mitsu 2008/09/17 �p�����[�^�p�^�[���ǉ� ***
                'If arr.Length = 2 Then
                '�s�\���ʍX�V����Ăяo�����ꍇ 
                If arr.Length = 1 Then
                    FuriDate = arr(0)
                    JobTuuban = 0
                    ReCalcFlag = 0
                ElseIf arr.Length = 2 Then
                    '************************************************
                    FuriDate = arr(0)
                    JobTuuban = CASTCommon.CAInt32(arr(1))
                    ReCalcFlag = 0
                ElseIf arr.Length = 3 Then
                    FuriDate = arr(0)
                    ReCalcFlag = CASTCommon.CAInt32(arr(1))
                    JobTuuban = CASTCommon.CAInt32(arr(2))
                Else
                    ELog.Write("�����J�n:�����Ȃ� PIG:" & Process.GetCurrentProcess.Id)
                    Return 1
                End If
            Else
                ELog.Write("�����J�n:�����Ȃ� PIG:" & Process.GetCurrentProcess.Id)
                Return 1
            End If

            ' �萔���v�Z����
            Dim TesuuCalcClass As New ClsTesuu

            ' ���C������
            ret = TesuuCalcClass.Main(FuriDate, ReCalcFlag, JobTuuban)
            ELog.Write("�����I�� " & cmdArgs(0))

            Return ret
        Catch ex As Exception
            ELog.Write("�萔���v�Z�Ɏ��s���܂����B", EventLogEntryType.Warning)
            MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "�萔���v�Z", "���s", ex.Message)
            Return 1
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "�萔���v�Z(�I��)", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
        End Try
    End Function
End Module
