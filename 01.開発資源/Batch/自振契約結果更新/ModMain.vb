Imports System
Imports System.IO

' ���U�_�񌋉ʍX�V ���C�����W���[��
Module ModMain

    Public MainLOG As New CASTCommon.BatchLOG("KFJ100", "���U�_�񌋉ʍX�V")


    ' �@�\�@ �F ���U�_�񌋉ʍX�V ���C������
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    Function Main(ByVal cmdArgs() As String) As Integer
        ' �߂�l

        Dim ret As Integer

        Dim ELog As New CASTCommon.ClsEventLOG
        If cmdArgs.Length = 0 Then
            ELog.Write("�����J�n:�����Ȃ�")
            Return -100
        End If

        ELog.Write("�����J�n:" & cmdArgs(0))
        
        ' ���U�_�񌋉ʍX�V����
        Dim JifuriKeiyakuKekkaClass As New ClsJikeiKekka

        ' ���C������
        ret = JifuriKeiyakuKekkaClass.Main(cmdArgs(0))

        ELog.Write("�����I�� " & cmdArgs(0))

        Return ret
    End Function

End Module
