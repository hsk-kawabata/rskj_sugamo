Option Explicit On 
Option Strict Off
Imports System.Text
Imports CASTCommon
Public Class KFGPRNT060

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �w�Z�}�X�^��������
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT060", "�w�Z�}�X�^�����������")
    Private Const msgTitle As String = "�w�Z�}�X�^�����������(KFGPRNT060)"
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '���̓{�^������
            btnPrnt.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim Flg As String = ""
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            If chk�w�Z��.Checked = False And chk������������.Checked = False And chk�U�֓���.Checked = False And chk�ϑ��҃R�[�h��.Checked = False Then
                MessageBox.Show("�\�[�g���Ԃ��w�肵�Ă�������", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            Flg = IIf(chk�w�Z��.Checked, "1", "0") & IIf(chk������������.Checked, ",1", ",0") & IIf(chk�U�֓���.Checked, ",1", ",0") & IIf(chk�ϑ��҃R�[�h��.Checked, ",1", ",0")

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "�w�Z�}�X�^������"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '�p�����[�^�ݒ�F���O�C�����A�\�[�g��
            param = GCom.GetUserID & "," & Flg

            nRet = ExeRepo.ExecReport("KFGP025.EXE", param)

            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case nRet
                Case 0
                    '�����m�F���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0014I, "�w�Z�}�X�^������"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '����ΏۂȂ����b�Z�[�W
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, "�w�Z�}�X�^������"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region

End Class
