Imports System
Imports System.IO
Imports System.Diagnostics

' �����U�֖��ו\ ���C�����W���[��
Module ModMain

    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFJP010", "�����U�֖��ו\")

    '
    ' �@�\�@ �F �����U�֖��ו\ ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F
    '
    Function Main(ByVal arg() As String) As Integer
        ' �߂�l
        Dim ret As Integer

        Dim Kouzafurimei As New ClsKouzafurimei

        Try
            MainLOG.Write("(��������)�J�n", "����")

            If arg.Length <> 1 Then
                MainLOG.Write("(��������)", "���s", "�����܂�����")
                Return 1
            End If

            ' �N���p�����[�^�i�[
            Dim Param() As String = arg(0).Split(","c)
            If Param.Length = 3 Then
                Kouzafurimei.TORI_CODE = Param(1)
                Kouzafurimei.HAISINTIME = Param(2)                ' �z�M�^�C���X�^���v
            ElseIf Param.Length = 4 Then
                Kouzafurimei.TORI_CODE = Param(1)               ' �����R�[�h
                Kouzafurimei.HAISINTIME = Param(2)                ' �z�M�^�C���X�^���v
                Kouzafurimei.PRINTERNAME = Param(3)          ' �o�̓v�����^
            Else
                MainLOG.Write("(��������)", "���s", "�p�����[�^���s���ł��B" & arg(0))
                Return -100
            End If
            MainLOG.ToriCode = Kouzafurimei.TORI_CODE
            MainLOG.FuriDate = Kouzafurimei.HAISINTIME
            MainLOG.Write("�p�����[�^�擾", "����", "�R�}���h���C������:" & arg(0))
        Catch ex As Exception
            MainLOG.Write("�p�����[�^�擾", "���s", ex.Message)
            Return -100
        End Try

        Dim ELog As New CASTCommon.ClsEventLOG
        ELog.Write("�����J�n")

        ' ���C������
        ret = Kouzafurimei.Main()
        ELog.Write("�����I��")

        Return ret
    End Function

End Module
