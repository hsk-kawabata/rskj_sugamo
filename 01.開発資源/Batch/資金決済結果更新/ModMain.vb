Imports System
Imports System.IO

' �������ό��ʍX�V ���C�����W���[��
Module ModMain

    Public MainLOG As New CASTCommon.BatchLOG("KFK020", "�������ό��ʍX�V")
    '
    ' �@�\�@ �F �������ό��ʍX�V ���C������
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    Function Main(ByVal cmdArgs() As String) As Integer
        ' �߂�l
        Dim ret As Integer

        Dim ELog As New CASTCommon.ClsEventLOG
        If cmdArgs.Length = 0 Then
            ELog.Write("�����J�n:�����Ȃ�")
            MainLOG.Write("�J�n", "���s", "�����Ȃ�")
            Return -100
        End If

        ELog.Write("�����J�n:" & cmdArgs(0))
        
        ' �������ό��ʍX�V����
        Dim KessaiKekkaClass As New ClsKessaiKekka

        ' ���C������
        ret = KessaiKekkaClass.Main(cmdArgs(0))

        ELog.Write("�����I�� " & cmdArgs(0))

        Return ret
    End Function

End Module
