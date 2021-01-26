Imports System.IO


Module ModMain

    Private Const KekkaOKCode As Integer = 0
    Private Const KekkaNGCode As Integer = -1

    ' ���O�����N���X
    Public MainLOG As New CASTCommon.BatchLOG("KFJP050", "�����U�֏������ʌ����\")

    Function Main(ByVal CmdArgs() As String) As Integer

        Dim ret As Integer = KekkaNGCode

        Try
            ' �����U�֏������ʌ����\�o�͏���
            Dim Ryousyusyosyo As New ClsSyorikekkakensuhyou

            Dim ELog As New CASTCommon.ClsEventLOG

            '��������
            If CmdArgs.Length <> 1 Then
                MainLOG.Write("�����J�n", "���s", "�����܂�����")

                Exit Try
            Else
                MainLOG.Write("�����J�n", "����", CmdArgs(0))
            End If

            '���[�������
            If Not Ryousyusyosyo.Main(CmdArgs(0)) Then

                Exit Try
            End If

            ret = KekkaOKCode

        Catch ex As Exception
            MainLOG.Write("�z��O�̃G���[���������܂���", "���s", ex.Message & "�F" & ex.StackTrace)
            ret = KekkaNGCode
        Finally
            MainLOG.Write("�����I��", "����")
        End Try

        Return ret

    End Function


End Module
