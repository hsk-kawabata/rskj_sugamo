'========================================================================
'ModMain
'�����}�X�^���ڊm�F�[�o�́iKFSP036�j�@���C�����W���[��
'
'�쐬���F2017/03/06
'�쐬�ҁF
'
'���l�F
'========================================================================
Imports System
Imports System.Text
Imports System.IO
Imports System.Diagnostics
Imports MenteCommon.clsCommon
Imports Microsoft.VisualBasic

Module ModMain

#Region "�N���X�萔��`"
    Public BatchLog As New CASTCommon.BatchLOG("KFSP036", "�����}�X�^���ڊm�F�[")

    '���[��
    Public Const strPrintName As String = "�����}�X�^���ڊm�F�["
    '���O�o�͗p
    Public Const strLogWrite As String = "KFSP036�i" & strPrintName & "�j"
    'RSV2 EDITION
    Public ReadOnly RSV2_MASTPTN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN")
#End Region


#Region "�N���X�ϐ���`"

    '�f�[�^�x�[�X�C���X�^���X
    Dim orcl As CASTCommon.MyOracle

    '���O���\����
    Public Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure
    Dim stcLog As LogWrite

    Private PrinterName As String = ""  '�ʏ�g���v�����^�[

    '���ʊ֐��C���X�^���X
    Public GCom As New MenteCommon.clsCommon

    '�V�X�e�����t
    Public StrSysDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    '�V�X�e������
    Public StrSysTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    '�p�����[�^�p�ϐ�
    Public strUserId As String
    Public strToriSCd As String
    Public strToriFCd As String
    Public strKijyunDate As String

    Public RecordCnt As Long = 0        ' �o�̓��R�[�h��
    Public LW As LogWrite

#End Region


#Region "���C�����\�b�h"

    '=======================================================================
    'Main
    '
    '���T�v��
    '�@�����}�X�^���ڊm�F�[�o�͂̃��C�����\�b�h�B
    '
    '���p�����[�^��
    '�@Args()�F�N���p�����[�^
    '�@�@(0)���[�U�h�c
    '�@�@(1)������R�[�h
    '�@�@(2)����敛�R�[�h
    '�@�@(3)���(yyyyMMdd)
    '
    '���߂�l��
    '�@�@0�F����I��
    '�@ -1�F�o�̓f�[�^�Ȃ��G���[
    '�@100�F�p���[���[�^�ُ�i�p�����[�^�Ȃ��j
    '�@101�Fini�t�@�C���擾�G���[
    '�@�@9�F���̑��ُ�I��
    '=======================================================================
    Function Main(ByVal CmdArgs() As String) As Integer

        Dim PrinterName As String       ' �v�����^��
        Dim LoginID As String = ""      ' ���O�C����
        Try
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = ""
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�J�n", "����")
            PrinterName = ""            '�ʏ�g���v�����^

            '---------------------------------------------------------
            '�p�����[�^�擾
            '---------------------------------------------------------
            If CmdArgs.Length = 0 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�J�n", "���s", "�����s��")
                Return 100
            End If

            If CmdArgs(0).Split(","c).Length <> 4 Then
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�J�n", "���s", "�����܂�����")
                Return 100
            End If

            Dim Cmd() As String = CmdArgs(0).Split(","c)
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����J�n", "����", CmdArgs(0))
            Try
                ' �N���p�����[�^�i�[
                strUserId = Cmd(0)
                strToriSCd = Cmd(1)
                strToriFCd = Cmd(2)
                strKijyunDate = Cmd(3)

                With LW
                    .UserID = strUserId
                    .ToriCode = strToriSCd & strToriFCd
                End With
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�p�����[�^�擾", "����")
            Catch ex As Exception
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�p�����[�^�擾", "���s", ex.Message)
                Return 9
            End Try

            'KFSP036�N���X����
            Dim clsRep As New KFSP036
            clsRep.CreateCsvFile()
            If clsRep.MakeRecord = False Then
                If RecordCnt = -1 Then      '����ΏۂȂ�
                    BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�I��", "����", "����ΏۂȂ�")
                    Return -1
                Else
                    Return 9
                End If
            End If

            '����������s
            If clsRep.ReportExecute(PrinterName) = True Then
                '�������
            Else
                '������s
                BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "RepoAgent���", "���s", clsRep.ReportMessage)
                Return -999     '���|�G�[�W�F���g����̖߂�l��Ԃ�(�b��-999)
            End If
            Return 0

        Catch ex As Exception
            '�ُ�I�����O
            BatchLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            Return 9
        End Try

    End Function

#End Region

End Module
