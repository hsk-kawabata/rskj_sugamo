Imports System
Imports System.IO
Imports System.Diagnostics

' �����G���[���X�g ���C�����W���[��
Module ModMain

    ' ���O�����N���X
    Private BatchLog As New CASTCommon.BatchLOG("KFJP015", "�����}�X�^�G���[���X�g����o�b�`")

    Function Main(ByVal CmdArgs() As String) As Integer
        Dim PrinterName As String       '�v�����^��
        Dim LoginID As String = ""      '���O�C����
        Dim CSVFileName As String = ""  '�b�r�u�t�@�C����(�t���p�X)

        Try
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "(�������)�J�n", "����", "")
            PrinterName = ""    '�ʏ�g���v�����^

            '---------------------------------------------------------
            '�p�����[�^�擾
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                '�p�����[�^�擾���s
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "�p�����[�^�擾", "���s", "�R�}���h���C�������Ȃ�")
                Return 1
            End If

            Dim Cmd() As String = String.Join(",", CmdArgs).Split(","c)
            If Cmd.Length <> 2 Then
                '�p�����[�^�ԈႢ
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "�p�����[�^�擾", "���s", "�R�}���h���C�������ُ�F" & CmdArgs(0))
                Return 1
            End If

            LoginID = Cmd(0)        '���O�C�����擾
            CSVFileName = Cmd(1)    '�b�r�u�t�@�C�����擾
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "�p�����[�^�擾", "����", "�R�}���h���C�������F" & CmdArgs(0))
            '---------------------------------------------------------
            ' �����G���[���X�g�������
            '---------------------------------------------------------
            Dim PrnSchErrList As New PrnSchErrList(CSVFileName)

            '����������s
            If PrnSchErrList.ReportExecute(PrinterName) = True Then
                '�������
                'BatchLog.Write(LoginID, "0000000000-00", "00000000", "(���C��)�I��", "����","")
            Else
                '������s
                BatchLog.Write(LoginID, "0000000000-00", "00000000", "(���)", "���s", PrnSchErrList.ReportMessage)
                Return 2
            End If

            If Not PrnSchErrList.HostCsvName Is Nothing AndAlso PrnSchErrList.HostCsvName <> "" Then
                Try
                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    DestName &= PrnSchErrList.HostCsvName
                    File.Copy(PrnSchErrList.FileName, DestName, True)
                Catch ex As Exception
                    '�b�r�u�t�@�C���㏈�����s
                    BatchLog.Write(LoginID, "0000000000-00", "00000000", "�b�r�u�t�@�C���㏈��", "���s", "")
                End Try
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "��O", "���s", ex.Message)
            Return -1
        Finally
            BatchLog.Write(LoginID, "0000000000-00", "00000000", "(�������)�I��", "����", "")
        End Try
    End Function

End Module
