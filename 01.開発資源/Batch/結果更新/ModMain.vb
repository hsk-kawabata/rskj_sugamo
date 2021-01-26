Imports System
Imports System.IO

' ���ʍX�V ���C�����W���[��
Module ModMain
    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFJ040", "�s�\���ʍX�V�o�b�`")
    Public Const msgTitle As String = "�s�\���ʍX�V�o�b�`(KFJ040)"
    Public Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Public LW As LogWrite
    '
    ' �@�\�@ �F ���ʍX�V ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    Function Main(ByVal CmdArgs() As String) As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1(LW.UserID, LW.ToriCode, LW.FuriDate, "���ʍX�V(�J�n)", "����")
            'MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�J�n", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
            

            ' �߂�l
            Dim ret As Integer

            Dim ELog As New CASTCommon.ClsEventLOG
            If CmdArgs.Length = 0 Then
                ELog.Write("�����J�n:�����Ȃ�")
            Else
                ELog.Write("�����J�n:" & CmdArgs(0))
            End If

            Console.WriteLine("�����J�n")

            If CmdArgs.Length = 0 Then
                MainLOG.Write("�J�n", "���s", "�p�����^�擾���s[" & CmdArgs(0) & "]")
                Return -100
            End If

            If CmdArgs.Length >= 2 Then
                CmdArgs(0) = String.Join("", CmdArgs).Replace(" "c, "")
            End If

            Dim KekkaClass As New ClsKekka

            ret = KekkaClass.Main(CmdArgs(0))

            Console.WriteLine("�����I��")
            ELog.Write("�����I��:" & CmdArgs(0))

            Return ret
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, LW.UserID, LW.UserID, LW.FuriDate, "���ʍX�V(�I��)", "����")
            'MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�I��", "����", "")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        End Try

    End Function

End Module
