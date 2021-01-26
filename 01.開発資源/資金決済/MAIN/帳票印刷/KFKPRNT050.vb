Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT050

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT050", "�������ʊm�F�\�Ĉ�����")
    Private Const msgTitle As String = "�������ʊm�F�\�Ĉ�����(KFKPRNT050)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
    Private PrintName As String  '���[��
    Private PrintID As String    '���[ID
    Private CsvNames() As String '�擾CSV�t�@�C����
    Private SyoriDate As String  '������
    Private PrtFolder As String
#Region " ���[�h"
    Private Sub KFKPRNT050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�V�X�e�����t�ƃ��[�U����\��
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------
            'INI�t�@�C���̓ǂݍ���
            '------------------------------------
            If fn_INI_Read() = False Then
                Return
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʕ\�����ɏ������ɃV�X�e�����t��\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�R���{�{�b�N�X��\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim Msg As String
            Select Case GCom.SetComboBox(cmbPrint, "KFKPRNT050_�Ώے��[.TXT", True)
                Case 1  '�t�@�C���Ȃ�
                    Msg = "�Ώے��[�̐ݒ�Ɏ��s���܂����B" & vbCrLf & _
                          "�e�L�X�g�t�@�C��:KFKPRNT050_�Ώے��[.TXT�����݂��܂���B"
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "�Ώے��[�ݒ�t�@�C���Ȃ��B�t�@�C��:KFKPRNT050_�Ώے��[.TXT"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", Msg)
                Case 2  '�ُ�
                    Msg = "�Ώے��[�̐ݒ�Ɏ��s���܂����B"
                    MessageBox.Show(Msg.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "�R���{�{�b�N�X�ݒ莸�s �R���{�{�b�N�X��:�Ώے��["
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", Msg)
            End Select
            '�R���g���[������
            btnAction.Enabled = False
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False

            cmbPrint.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub

#End Region
#Region " ����"
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :�����{�^��
        'Return         :
        'Create         :2009/09/09
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)�J�n", "����", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�K�{���ڂ̓��͒l�`�F�b�N
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�K�{���ځF�U�֓�
            '--------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            SyoriDate = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text
            Call Get_PrintKbn()

            '�Ώۂ�CSV�t�@�C������(���[ID_������(YYYYMMDD)�^�C���X�^���v(HHmmss)_�v���Z�XID_�A��.CSV)
            Dim FileList() As String = Directory.GetFiles(PrtFolder, PrintID & "_" & SyoriDate & "*.CSV")
            If FileList.Length = 0 Then
                '�Ώۈ���p��CSV�Ȃ�
                MessageBox.Show(MSG0232W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return
            End If
            ReDim CsvNames(0)
            '�^�C���X�^���v�R���{�{�b�N�X�ɃA�C�e����ǉ�
            For No As Integer = 0 To FileList.Length - 1
                Dim FileInfo() As String = Split(Path.GetFileName(FileList(No)), "_"c)
                'YYYY/MM/DD HH:mm:dd�`���ɕҏW
                Dim Time As String = Mid(FileInfo(1), 1, 4) & "/" & Mid(FileInfo(1), 5, 2) & "/" & Mid(FileInfo(1), 7, 2) _
                                     & " " & Mid(FileInfo(1), 9, 2) & ":" & Mid(FileInfo(1), 11, 2) & ":" & Mid(FileInfo(1), 13, 2)
                cmbTimeStamp.Items.Add(Time)
                ReDim Preserve CsvNames(No)
                CsvNames(No) = FileList(No)
            Next

            '�R���g���[������
            cmbPrint.Enabled = False
            cmbTimeStamp.SelectedIndex = 0
            txtSyoriDateY.Enabled = False
            txtSyoriDateM.Enabled = False
            txtSyoriDateD.Enabled = False
            btnSearch.Enabled = False
            btnAction.Enabled = True
            btnReset.Enabled = True
            cmbTimeStamp.Enabled = True
            cmbTimeStamp.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)�I��", "����", "")
        End Try
    End Sub

#End Region
#Region " ���"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :����{�^��
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(MSG0013I.Replace("{0}", PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����

            param = GCom.GetUserID & "," & CsvNames(cmbTimeStamp.SelectedIndex)

            nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

            '�߂�l�ɑΉ��������b�Z�[�W��\������
            ' 2017/05/26 �^�X�N�j���� CHG �yME�z(RSV2�Ή� ���b�Z�[�W�萔��) -------------------- START
            'Select Case nRet
            '    Case 0
            '        '�����m�F���b�Z�[�W
            '        MessageBox.Show(PrintName & "�̈�����s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '    Case -1
            '        '����ΏۂȂ����b�Z�[�W
            '        MessageBox.Show(PrintName & "�̈���Ώۂ�0���ł��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Case Else
            '        '������s���b�Z�[�W
            '        MessageBox.Show(PrintName & "�̈���Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End Select
            Select Case nRet
                Case 0
                    MessageBox.Show(MSG0014I.Replace("{0}", PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MessageBox.Show(MSG0004E.Replace("{0}", PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select
            ' 2017/05/26 �^�X�N�j���� CHG �yME�z(RSV2�Ή� ���b�Z�[�W�萔��) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region
#Region " ���"
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :����{�^��
        'Return         :
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            '�R���g���[������
            cmbTimeStamp.Items.Clear()  '�^�C���X�^���v������
            cmbPrint.Enabled = True
            txtSyoriDateY.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateD.Enabled = True
            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnReset.Enabled = False
            txtSyoriDateY.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " �I��"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)��O", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " �֐�"
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :�X�V�{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
            '�N�K�{�`�F�b�N
            If txtSyoriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If
            '���K�{�`�F�b�N
            If txtSyoriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If
            '���͈̓`�F�b�N
            If GCom.NzInt(txtSyoriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If
            '���t�K�{�`�F�b�N
            If txtSyoriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If
            '���t�͈̓`�F�b�N
            If GCom.NzInt(txtSyoriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '���t�������`�F�b�N
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.Message)
            Return False
        Finally

        End Try
        fn_check_text = True
    End Function
    Public Function fn_INI_Read() As Boolean
        '============================================================================
        'NAME           :fn_INI_Read
        'Parameter      :
        'Description    :FSKJ.INI�t�@�C���̓ǂݍ���
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        fn_INI_Read = False
        'PRT�i�[�t�H���_
        PrtFolder = CASTCommon.GetFSKJIni("COMMON", "PRT")
        If PrtFolder.ToUpper = "ERR" Or PrtFolder = Nothing Then

            MessageBox.Show(String.Format(MSG0001E, "PRTCSV�i�[�t�H���_", "COMMON", "PRT"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        fn_INI_Read = True
    End Function
    Private Function Get_PrintKbn() As Boolean
        '============================================================================
        'NAME           :Get_PrintKbn
        'Parameter      :
        'Description    :���[�̏���ϐ��Ɋi�[����
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        Try
            Select Case GCom.GetComboBox(cmbPrint)
                Case 0
                    PrintID = "KFKP001"
                    PrintName = "�������ʊm�F�\(��������)"
                Case 1
                    PrintID = "KFKP003"
                    PrintName = "�������ʊm�F�\(���ʍX�V)"
                Case Else
                    Return False
            End Select
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "������擾", "���s", ex.Message)
            Return False
        End Try
    End Function
#End Region
#Region " �C�x���g"
    '�[���p�f�B���O
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtSyoriDateY.Validating, txtSyoriDateM.Validating, txtSyoriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�[���p�f�B���O", "���s", ex.Message)
        End Try
    End Sub
#End Region
End Class
