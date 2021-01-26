Imports System
Imports System.IO
Imports System.Diagnostics

Public Class ClsDensou
    ' �Ɩ��w�b�_�p�X
    Private GYOMUHEADPATH As String
    Private GYOMUHEADNAME As String

    Public Message As String = ""
    Public MessageDetail As String = ""

    ' �Ɩ��w�b�_
    Structure DENSOUPARAM
        Dim RecordKubun As String       ' ���R�[�h�敪
        Dim RecordName As String        ' ���R�[�h���ʖ�
        Dim CenterCode As String        ' ����Z���^�[�m�F�R�[�h
        Dim TouhouCode As String        ' �����Z���^�[�m�F�R�[�h
        Dim ZenginName As String        ' �S��t�@�C����
        Dim Syuhaisin As String         ' �W�z�M�敪
        Dim DensouNitiji As String      ' �`������
        Dim HostTuuban As String        ' �z�X�g�ʔ�
        Dim CodeKubun As String         ' �R�[�h�敪
        Dim EncodeKubun As String       ' �Í����敪
        Dim RecordLen As String         ' ���R�[�h��
        Dim RecordCount As String       ' ���R�[�h����
        Dim FileName As String          ' �t�@�C����    
        Dim AES As String               ' �`�d�r�i0:AES�Ȃ��C1:32�C2:48�C3:64�j
        Dim EncodeKey As String         ' �Í����L�[
        Dim EncodeVIKey As String       ' �Í���IV�L�[
        Dim Yobi As String              ' �\��
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(RecordKubun, 1), _
                    SubData(RecordName, 4), _
                    SubData(CenterCode, 14), _
                    SubData(TouhouCode, 14), _
                    SubData(ZenginName, 12), _
                    SubData(Syuhaisin, 1), _
                    SubData(DensouNitiji, 14), _
                    SubData(HostTuuban, 2), _
                    SubData(CodeKubun, 1), _
                    SubData(EncodeKubun, 1), _
                    SubData(RecordLen, 4, 1, "0"c), _
                    SubData(RecordCount, 8, 1, "0"c), _
                    SubData(FileName, 50), _
                    SubData(AES, 1), _
                    SubData(EncodeKey, 64), _
                    SubData(EncodeVIKey, 32), _
                    SubData(Yobi, 27) _
                    })
            End Get
            Set(ByVal value As String)
                value = value.PadRight(200, " "c)

                RecordKubun = CuttingData(value, 1)
                RecordName = CuttingData(value, 4).Trim
                CenterCode = CuttingData(value, 14).Trim
                TouhouCode = CuttingData(value, 14).Trim
                ZenginName = CuttingData(value, 12).Trim
                Syuhaisin = CuttingData(value, 1)
                DensouNitiji = CuttingData(value, 14)
                HostTuuban = CuttingData(value, 2)
                CodeKubun = CuttingData(value, 1)
                EncodeKubun = CuttingData(value, 1)
                RecordLen = CuttingData(value, 4).Trim
                RecordCount = CuttingData(value, 8)
                FileName = CuttingData(value, 50).Trim
                AES = CuttingData(value, 1)
                EncodeKey = CuttingData(value, 64)
                EncodeVIKey = CuttingData(value, 32)
                Yobi = CuttingData(value, 27)
            End Set
        End Property
    End Structure

    ' �Ɩ��w�b�_
    Public Property GyoumuHeadName() As String
        Get
            ' �Ɩ��w�b�_���̎擾�i�p�X�t���j
            Return Path.Combine(GYOMUHEADPATH, GYOMUHEADNAME)
        End Get
        Set(ByVal Value As String)
            ' �Ɩ��w�b�_���� �ݒ�
            GYOMUHEADPATH = Path.GetDirectoryName(Value)
            If GYOMUHEADPATH = "" Then
                GYOMUHEADPATH = CASTCommon.GetFSKJIni("OTHERSYS", "GYOMUPATH")
                GYOMUHEADNAME = Value
            Else
                GYOMUHEADNAME = Path.GetFileName(Value)
            End If
        End Set
    End Property

    Public Function LinkProcess() As Boolean
        ' LinkExpress �o�^
        '*** �C�� mitsu 2008/07/24 �G���[���̓p�����[�^���Ԃ� ***
        Dim errArguments As String = ""
        '********************************************************
        Try
            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = CASTCommon.GetFSKJIni("OTHERSYS", "LINK")
            Dim Argum As String = CASTCommon.GetFSKJIni("OTHERSYS", "LINK-PARAM")
            If File.Exists(ProcInfo.FileName) = True Then
                ProcInfo.WorkingDirectory = GYOMUHEADPATH
                ProcInfo.Arguments = Argum.Replace("%1", Path.Combine(GYOMUHEADPATH, GYOMUHEADNAME)).Replace("%2", GYOMUHEADNAME)
                '*** �C�� mitsu 2008/07/24 �G���[���̓p�����[�^���Ԃ� ***
                errArguments = ProcInfo.Arguments
                '********************************************************
                Proc = Process.Start(ProcInfo)
                Proc.WaitForExit()
                If Proc.ExitCode = 0 Then
                    ' �A�g����
                    Return True
                Else
                    ' �A�g���s
                    Message = "Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments
                    Proc.StandardOutput.ReadToEnd()
                    Return False
                End If
            Else
                Message = "�N���A�v���P�[�V�����Ȃ��F" & ProcInfo.FileName
                Return False
            End If
            Proc.Close()
        Catch ex As Exception
            '*** �C�� mitsu 2008/07/24 �G���[���̓p�����[�^���Ԃ� ***
            'Message = ex.Message
            Message = ex.Message & " " & errArguments
            '********************************************************
            Return False
        End Try
    End Function

    Public Function ReadHeader() As String
        Dim ReadData As String

        Dim FStream As New StreamReader(GyoumuHeadName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
        ReadData = FStream.ReadLine()
        FStream.Close()
        FStream = Nothing

        Return ReadData
    End Function

    Public Function SaveHeader(ByVal data As String) As Boolean
        Dim sw As New StreamWriter(GyoumuHeadName, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
        sw.Write(data)
        sw.Close()

        sw = Nothing
    End Function

    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '           ARG3 - �O�F���l�C�P�F�E�l
    '           ARG4 - ���ߕ���
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer, _
                Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            ' �؂��镶����
            If align = 0 Then
                ' ���l
                value = value.PadRight(len, pad)
            Else
                ' �E�l
                value = value.PadLeft(len, pad)
            End If
            Dim bt() As Byte = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(value)
            Return System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Protected Friend Shared Function CuttingData(ByRef value As String, ByVal len As Integer) As String
        Try
            ' �؂��镶����
            Dim ret As String
            Dim bt() As Byte = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(value)
            ret = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(bt, 0, len)
            ' �؂�������̎c��̕�����
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
