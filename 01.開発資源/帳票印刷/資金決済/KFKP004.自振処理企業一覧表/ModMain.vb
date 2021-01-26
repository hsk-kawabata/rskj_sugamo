Imports System
Imports System.IO
Imports System.Diagnostics

' ���U������ƈꗗ�\��� ���C�����W���[��
Module ModMain

    Public BatchLog As New CASTCommon.BatchLOG("KFKP004", "���U������ƈꗗ�\")
    Public Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Public LW As LogWrite
    Public RecordCnt As Long = 0     '�o�̓��R�[�h��
    Public mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Public mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    Public txtFURI_DATE As String
    Public Jikinko As String
    Private PrinterName As String = ""
    ' �p�u���b�N�c�a
    Public MainDB As CASTCommon.MyOracle
    '
    ' �@�\�@ �F ���U������ƈꗗ�\��� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^, ARG2 - ���ϓ�
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        ' �߂�l
        Dim ret As Integer
        Try
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������)�J�n", "����", "")

            ' ���U������ƈꗗ�\�������
            Dim ELog As New CASTCommon.ClsEventLOG

            If CmdArgs.Length = 0 Then
                '�p�����[�^�擾���s
                BatchLog.Write("�p�����[�^�`�F�b�N", "���s", "�R�}���h���C�������Ȃ�")
                Return -100
            End If

            Dim Param() As String = CmdArgs(0).Split(",")
            If Param.Length <> 2 Then
                '�p�����[�^�ԈႢ
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�p�����[�^�`�F�b�N", "���s", "�R�}���h���C�������ُ�F" & CmdArgs(0))
                Return -100
            End If

            ELog.Write("�����J�n")
            LW.UserID = Param(0)             ' ���O�C����
            txtFURI_DATE = Param(1)        ' ���ϓ�
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�p�����[�^�擾", "����", "�R�}���h���C�������F" & CmdArgs(0))

            '---------------------------------------------------------
            ' ���U������ƈꗗ�\�������
            '---------------------------------------------------------
            Dim Itiranhyo As New ClsPrnJifurisyori()
            Itiranhyo.CreateCsvFile()
            If Itiranhyo.MakeRecord = False Then
                If RecordCnt = -1 Then   '����ΏۂȂ�
                    Return -1
                Else
                    Return -300 '�������̃G���[
                End If
            End If

            '����������s
            If Itiranhyo.ReportExecute(PrinterName) = True Then
                '�������
            Else
                '������s
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent���", "���s", Itiranhyo.ReportMessage)
                Return -999     '���|�G�[�W�F���g����̖߂�l��Ԃ�(�b��-999)
            End If
            Return 0

            ELog.Write("�����I��:" & CmdArgs(0))

        Catch ex As Exception
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
        Finally
            If RecordCnt > 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������)�I��", "����", RecordCnt & "��")
            Else
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������)�I��", "����", "")
            End If
        End Try


        Return ret
    End Function

End Module
