Imports System
Imports System.IO

' ���Z�@�փ}�X�^�X�V ���C�����W���[��
Module ModMain
    '
    ' �@�\�@ �F ���Z�@�փ}�X�^�X�V ���C������
    '
    ' �����@ �F ARG1 - �t�@�C�����i�p�X�t���j
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    '
    Function Main(ByVal CmdArgs() As String) As Integer
        ' �߂�l
        Dim ret As Integer

        Dim ELog As New CASTCommon.ClsEventLOG
        If CmdArgs.Length = 0 Then
            ELog.Write("�����J�n:�����Ȃ�")
        Else
            ELog.Write("�����J�n:" & CmdArgs(0))
        End If

        If CmdArgs.Length = 0 Then
            ELog.Write("�������s�F�����Ȃ�", Diagnostics.EventLogEntryType.Error)
            Return -100
        End If

        If CmdArgs(0).Split(","c).Length = 3 Then
            CmdArgs(0) = String.Join("", CmdArgs).Replace(" "c, "")
        Else
            '�����Ⴂ
            Return -1
        End If


        ' ���Z�@�փ}�X�^�X�V
        Dim TenmastKousinClass As New ClsKousin

        ' ���C������
        ret = TenmastKousinClass.Main(CmdArgs(0))


        If ret = -100 Then
            ELog.Write("�������s�F�p�����^�擾���s[" & CmdArgs(0) & "]", Diagnostics.EventLogEntryType.Error)
        Else
            ELog.Write("�����I��:" & CmdArgs(0))
        End If

        Return ret
    End Function

End Module
