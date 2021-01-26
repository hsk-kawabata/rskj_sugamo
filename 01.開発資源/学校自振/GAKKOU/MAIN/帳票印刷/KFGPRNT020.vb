Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT020

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �����U�֗\��ꗗ�\���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Dim Str_Report_Path As String
    Dim STR������ As String
    Dim STR�P�w�N As String
    Dim STR�Q�w�N As String
    Dim STR�R�w�N As String
    Dim STR�S�w�N As String
    Dim STR�T�w�N As String
    Dim STR�U�w�N As String
    Dim STR�V�w�N As String
    Dim STR�W�w�N As String
    Dim STR�X�w�N As String
    Dim STR�w�Z�R�[�h As String
    Dim STR���[�\�[�g�� As String
    Dim STR���[�\�[�g�敪 As String

    Private STR�w�Z�� As String

    Private Str_Sch_Kbn As String
    Private STR_FURI_DATE_SAIFURI As String

    Dim str�����N���x As String
    Dim LNG�U�֋��z���v As Long
    Dim str�O��_�U�֓� As String
    Dim LNG�s�\���� As Long

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT020", "�����U�֗\��ꗗ�\������")
    Private Const msgTitle As String = "�����U�֗\��ꗗ�\������(KFGPRNT020)"
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
    Private Sub KFGPRNT020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
            Dim MSG As String
            Select Case GCom.SetComboBox(cmbFURIKUBUN, STR_FURI_SFURI_KBN_TXT, True)
                Case 1  '�t�@�C���Ȃ�
                    MSG = String.Format(MSG0025E, "�U�֋敪", STR_FURI_SFURI_KBN_TXT)
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MSG = "�U�֋敪" & "�ݒ�t�@�C���Ȃ��B�t�@�C��:" & STR_FURI_SFURI_KBN_TXT
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", MSG)
                    Return
                Case 2  '�ُ�
                    MSG = String.Format(MSG0026E, "�U�֋敪")
                    MessageBox.Show(MSG.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MSG = "�R���{�{�b�N�X�ݒ莸�s �R���{�{�b�N�X��:" & "�U�֋敪"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", MSG)
                    Return
            End Select

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
        Try
            Dim Param As String
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            LNG�s�\���� = 0
            MainDB = New MyOracle
            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            Str_Sch_Kbn = ""

            STR�P�w�N = ""
            STR�Q�w�N = ""
            STR�R�w�N = ""
            STR�S�w�N = ""
            STR�T�w�N = ""
            STR�U�w�N = ""
            STR�V�w�N = ""
            STR�W�w�N = ""
            STR�X�w�N = ""

            STR_COMMAND = "���"


            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)


            MainDB.BeginTrans()

            '���[�N�}�X�^�폜
            If PFUNC_PRTWORK_DEL() = False Then
                Exit Sub
            End If

            MainDB.Commit()

            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)

                '�ĐU���A���U�̏������I�����Ă��邩�̃`�F�b�N
                If PFUNC_SCHMAST_CHECK() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '�ĐU���A���U�����擾
                Call PSUB_GETSYOFURI()


                '�����Ώۊw�N�̎擾
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '���[�\�[�g�w���̎擾
                If PFUNC_GAKMAST2_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '����O�m�F���b�Z�[�W
                If MessageBox.Show(String.Format(MSG0013I, "�����U�֗\��ꗗ�\"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                Dim nRet As Integer
                If Trim(STR_FURI_DATE_SAIFURI) <> "" Then
                    '�ĐU�p���[
                    '�����U�ֈꗗ�\(�����f�[�^),�����U�֗\��ꗗ�\(�����f�[�^0���p)��� 
                    '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�ĐU��,������,���[�\�[�g��,�s�\����
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURI_DATE_SAIFURI & "," & STR_FURIKAE_DATE(1) & "," & _
                            STR������ & "," & STR���[�\�[�g�� & "," & LNG�s�\����

                    nRet = ExeRepo.ExecReport("KFGP014.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                            '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------START
                        Case -1
                            '����ΏۂȂ����b�Z�[�W
                            MessageBox.Show(String.Format("����Ώ�0���ł��B", "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                            '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------END
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��ꗗ�\(�����f�[�^)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select
                Else
                    'G_PRTWORK�Ƀf�[�^���C���T�[�g���AG_PRTWORK�̒l���������
                    MainDB.BeginTrans()
                    If PFUNC_PRTWORK_SEITOINS() = False Then
                        MainDB.Rollback()
                        Exit Sub
                    End If
                    MainDB.Commit()
                    '�˗��p���[
                    '�����U�ֈꗗ�\��� 
                    '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�ĐU��,������,���[�\�[�g��,�s�\����
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR���[�\�[�g��

                    nRet = ExeRepo.ExecReport("KFGP013.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                            '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------START
                        Case -1
                            '����ΏۂȂ����b�Z�[�W
                            MessageBox.Show(String.Format("����Ώ�0���ł��B", "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                            '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------END
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                    End Select
                End If

                MessageBox.Show(String.Format(MSG0014I, "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '�S�w�Z�R�[�h�Ώ�
                Dim SQL As New StringBuilder
                Dim OraReader As New MyOracleReader(MainDB)
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")
                SQL.Append(" AND FURI_KBN_S ='" & GCom.GetComboBox(cmbFURIKUBUN) & "'")
                '2006/10/26�@�s�\�t���O�`�F�b�N���͂���
                'If GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURI_SFURI_KBN_TXT, cmbFURIKUBUN) = 1 Then
                '    STR_SQL += " AND"
                '    STR_SQL += " FUNOU_FLG_S ='1' "
                'End If
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")


                Dim intCOUNT As Integer = 0
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '����O�m�F���b�Z�[�W
                If MessageBox.Show(String.Format(MSG0013I, "�����U�֗\��ꗗ�\"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                While OraReader.EOF = False
                    STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")

                    '�ĐU���A���U�̏������I�����Ă��邩�̃`�F�b�N
                    If PFUNC_SCHMAST_CHECK() = False Then
                        GoTo NEXT_DATA
                    End If
                    '�ĐU���A���U�����擾
                    Call PSUB_GETSYOFURI()


                    '�Ώۊw�N�̎擾
                    If PFUNC_SCHMAST_GET() = False Then
                        Exit Sub
                    End If

                    '���[�\�[�g���̎擾
                    If PFUNC_GAKMAST2_GET() = False Then
                        '    Exit Sub
                    End If

                    intCOUNT += 1

                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    Dim nRet As Integer
                    If Trim(STR_FURI_DATE_SAIFURI) <> "" Then
                        '�ĐU�p���[
                        '�����U�ֈꗗ�\(�����f�[�^),�����U�֗\��ꗗ�\(�����f�[�^0���p)���
                        '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�ĐU��,������,���[�\�[�g��,�s�\����
                        Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURI_DATE_SAIFURI & "," & STR_FURIKAE_DATE(1) & "," & _
                                STR������ & "," & STR���[�\�[�g�� & "," & LNG�s�\����

                        nRet = ExeRepo.ExecReport("KFGP014.EXE", Param)
                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                        Select Case nRet
                            Case 0
                                '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------START
                            Case -1
                                '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------END
                            Case Else
                                '������s���b�Z�[�W
                                MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��ꗗ�\(�����f�[�^)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select
                    Else
                        'G_PRTWORK�Ƀf�[�^���C���T�[�g���AG_PRTWORK�̒l���������
                        MainDB.BeginTrans()
                        If PFUNC_PRTWORK_SEITOINS() = False Then
                            MainDB.Rollback()
                        Else
                            MainDB.Commit()
                            '�˗��p���[
                            '�����U�ֈꗗ�\��� 
                            '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�ĐU��,������,���[�\�[�g��,�s�\����
                            Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR���[�\�[�g��

                            nRet = ExeRepo.ExecReport("KFGP013.EXE", Param)
                            '�߂�l�ɑΉ��������b�Z�[�W��\������
                            Select Case nRet
                                Case 0
                                    '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------START
                                Case -1
                                    '2011/06/16 �W���ŏC�� ���R�[�h����0���̏ꍇ�A���b�Z�[�W�ύX------------------END
                                Case Else
                                    '������s���b�Z�[�W
                                    MessageBox.Show(String.Format(MSG0004E, "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Exit Sub
                            End Select
                        End If

                    End If
NEXT_DATA:
                    OraReader.NextRead()
                End While

                If intCOUNT = 0 And GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURI_SFURI_KBN_TXT, cmbFURIKUBUN) = "1" Then
                    MessageBox.Show(G_MSG0024W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    MessageBox.Show(String.Format(MSG0014I, "�����U�֗\��ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

            txtGAKKOU_CODE.Focus()

        Catch ex As Exception
            MainDB.Rollback()
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
#Region " �C�x���g"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '�w�Z�J�i�i���݃R���{
        '********************************************
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z���R���{�{�b�N�X�ݒ�
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lab�w�Z��.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '�w�N�e�L�X�g�{�b�N�X��FOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                .Text = .Text.Trim.PadLeft(.MaxLength, "0")
                STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
                '�w�Z���̎擾
                If PFUNC_GAKNAME_GET() = False Then
                    Exit Sub
                End If
            End If
        End With
    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
       Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("�[���p�f�B���O", "���s", ex.ToString)
        End Try
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region
#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean
        '�w�Z���̐ݒ�
        PFUNC_GAKNAME_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As MyOracle = Nothing
        Try
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab�w�Z��.Text = ""
            Else
                If OraDB Is Nothing Then OraDB = New MyOracle
                OraReader = New MyOracleReader(OraDB)
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G =" & SQ(txtGAKKOU_CODE.Text))

                '�w�Z�}�X�^�P���݃`�F�b�N
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab�w�Z��.Text = ""
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z�}�X�^����)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_GAKMAST2_GET() As Boolean
        '�w�Z�}�X�^�Q�̎擾

        PFUNC_GAKMAST2_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST2")
            SQL.Append(" WHERE")
            SQL.Append(" GAKKOU_CODE_T =" & SQ(STR�w�Z�R�[�h))

            If OraReader.DataReader(SQL) = False Then
                STR���[�\�[�g�� = "0"
            Else
                STR���[�\�[�g�� = OraReader.GetString("MEISAI_OUT_T")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z�}�X�^����)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_GAKMAST2_GET = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR�w�Z�R�[�h))
            SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            SQL.Append(" AND FURI_KBN_S =" & SQ(GCom.GetComboBox(cmbFURIKUBUN)))

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '2017/03/14 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '���ʐU�֓��Ή�
            STR�P�w�N = ""
            STR�Q�w�N = ""
            STR�R�w�N = ""
            STR�S�w�N = ""
            STR�T�w�N = ""
            STR�U�w�N = ""
            STR�V�w�N = ""
            STR�W�w�N = ""
            STR�X�w�N = ""
            '2017/03/14 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ END

            While OraReader.EOF = False
                With OraReader
                    If Trim(Str_Sch_Kbn) = "" Then
                        Str_Sch_Kbn = .GetString("SCH_KBN_S")
                    Else
                        Str_Sch_Kbn = 9
                    End If

                    'If Trim(Str_Sch_Kbn) <> "" AND Str_Sch_Kbn <> .GetString("SCH_KBN_S") Then
                    '    Str_Sch_Kbn = 9
                    'Else
                    '    Str_Sch_Kbn = .GetString("SCH_KBN_S")
                    'End If

                    STR������ = Mid(.GetString("NENGETUDO_S"), 5, 2)
                    str�����N���x = .GetString("NENGETUDO_S")
                    If .GetString("GAKUNEN1_FLG_S") = "1" Then
                        STR�P�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�P�w�N <> "" Then
                        '        STR�P�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN2_FLG_S") = "1" Then
                        STR�Q�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�Q�w�N <> "" Then
                        '        STR�Q�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN3_FLG_S") = "1" Then
                        STR�R�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�R�w�N <> "" Then
                        '        STR�R�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN4_FLG_S") = "1" Then
                        STR�S�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�S�w�N <> "" Then
                        '        STR�S�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN5_FLG_S") = "1" Then
                        STR�T�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�T�w�N <> "" Then
                        '        STR�T�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN6_FLG_S") = "1" Then
                        STR�U�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�U�w�N <> "" Then
                        '        STR�U�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN7_FLG_S") = "1" Then
                        STR�V�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�V�w�N <> "" Then
                        '        STR�V�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN8_FLG_S") = "1" Then
                        STR�W�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�W�w�N <> "" Then
                        '        STR�W�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                    If .GetString("GAKUNEN9_FLG_S") = "1" Then
                        STR�X�w�N = "1"
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        '���ʐU�֓��Ή�
                        'Else
                        '    If STR�X�w�N <> "" Then
                        '        STR�X�w�N = ""
                        '    End If
                        '2017/03/14 �^�X�N�j���� DEL �W���ŏC���i���݃o�O�C���j------------------------------------ END
                    End If
                End With
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z�}�X�^����)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_SCHMAST_CHECK() As Boolean

        PFUNC_SCHMAST_CHECK = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '�I�������U�֋敪���ĐU�̏ꍇ�̂�
            If GCom.GetComboBox(cmbFURIKUBUN) = 1 Then
                OraReader = New MyOracleReader(MainDB)

                '�w�肵�����t���ĐU���Ɏ����U�̃X�P�W���[�����s�\���ʍX�V�ς݂ł��邩�ǂ����̃`�F�b�N
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR�w�Z�R�[�h))
                SQL.Append(" AND SFURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND FURI_KBN_S ='0'")

                '���U�A�ĐU
                If OraReader.DataReader(SQL) Then
                    If OraReader.GetInt64("FUNOU_FLG_S") = 0 Then
                        MessageBox.Show(G_MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Else
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_SCHMAST_CHECK = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Try
            '�w�Z�R�[�h�K�{�`�F�b�N
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            End If
            '�N�K�{�`�F�b�N
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            '���K�{�`�F�b�N
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '���͈̓`�F�b�N
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '���t�K�{�`�F�b�N
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If
            '���t�͈̓`�F�b�N
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '���t�������`�F�b�N
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
            STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Sub PSUB_GETSYOFURI()

        STR_FURI_DATE_SAIFURI = ""
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '�I�������U�֋敪���ĐU�̏ꍇ�̂�
            If GCom.GetComboBox(cmbFURIKUBUN) = 1 Then
                OraReader = New MyOracleReader(MainDB)
                '��ʂœ��͂������t���ĐU���Ɏ����U�̃X�P�W���[�����擾
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S ='" & STR�w�Z�R�[�h & "'")
                SQL.Append(" AND FURI_KBN_S ='0'")
                SQL.Append(" AND SFURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")


                If OraReader.DataReader(SQL) = False Then
                    Exit Sub
                Else
                    STR_FURI_DATE_SAIFURI = OraReader.GetString("FURI_DATE_S")
                    LNG�s�\���� = OraReader.GetString("FUNOU_KEN_S")
                End If
            Else

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���U���擾", "���s", ex.ToString)
            Return
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

    End Sub
    Private Function PFUNC_PRTWORK_SEITOINS() As Boolean

        PFUNC_PRTWORK_SEITOINS = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
        Dim Funou_FLG As Boolean = False
        '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END

        ' 2017/05/25 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
        Dim SFuriCode As String = String.Empty
        ' 2017/05/25 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

        Try
            ' 2017/05/25 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/05/25 �^�X�N�j���� ADD �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

            OraReader = New MyOracleReader(MainDB)
            '-----------------------
            '���z���肩����
            '-----------------------
            SQL.Append(" SELECT * FROM GAKMAST2")
            SQL.Append(" WHERE")
            SQL.Append(" GAKKOU_CODE_T =" & SQ(STR�w�Z�R�[�h))

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If

            Dim strSFURI_SYUBETU As String = ""

            strSFURI_SYUBETU = OraReader.GetString("SFURI_SYUBETU_T")

            OraReader.Close()

            '���z������̂Ƃ�
            If strSFURI_SYUBETU = "2" Or strSFURI_SYUBETU = "3" Then
                Select Case (STR������)
                    Case "04"  '4���͎��z�����Ȃ����ߔ�΂�
                    Case Else
                        Dim strNEGETU As String, strNEN As String
                        If STR������ = "01" Then
                            strNEGETU = "12"
                            strNEN = CStr(Val(str�����N���x.Substring(0, 4)) - 1)
                        Else
                            strNEGETU = CStr(Val(STR������) - 1).PadLeft(2, "0")
                            strNEN = str�����N���x.Substring(0, 4)
                        End If
                        SQL = New StringBuilder(128)
                        SQL.Append(" SELECT * FROM G_SCHMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_S =" & SQ(STR�w�Z�R�[�h))
                        SQL.Append(" AND FURI_DATE_S < " & SQ(STR_FURIKAE_DATE(1))) '���͐U�֓��ȑO�̂��̂ŕs�\�̂��̂�T��
                        '  SQL.Append(" AND NENGETUDO_S ='" & strNEN & strNEGETU & "'")
                        SQL.Append(" AND (FURI_KBN_S =  '0' OR FURI_KBN_S = '1')")
                        '  SQL.Append(" FURI_KBN_S ='0'")
                        If Str_Sch_Kbn = "9" Then
                            SQL.Append(" AND SCH_KBN_S ='0'")
                        End If
                        SQL.Append(" AND FUNOU_FLG_S =  '1'") '���͐U�֓��ȑO�ŕs�\�t���O�������Ă���X�P�W���[��
                        SQL.Append(" ORDER BY")
                        SQL.Append(" FURI_DATE_S desc")
                        '2010/10/21 ���z���������X�P�W���[���������ꍇ�̑Ή��ǉ�-------------------------------------
                        'OraReader = New MyOracleReader(MainDB)
                        Dim G_SchReader As MyOracleReader = Nothing
                        G_SchReader = New MyOracleReader(MainDB)

                        'If OraReader.DataReader(SQL) = False Then
                        '    OraReader.Close()
                        '    Exit Select
                        'End If
                        If G_SchReader.DataReader(SQL) = False Then
                            G_SchReader.Close()
                            Exit Select
                        End If
                        '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
                        Dim Furi_Date_BK As String
                        Furi_Date_BK = G_SchReader.GetString("FURI_DATE_S")
                        '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END
                        While G_SchReader.EOF = False

                            Dim dblFUNOU_KEN As Double = 0

                            'str�O��_�U�֓� = OraReader.GetString("FURI_DATE_S")
                            'dblFUNOU_KEN = OraReader.GetString("FUNOU_KEN_S")
                            str�O��_�U�֓� = G_SchReader.GetString("FURI_DATE_S")
                            dblFUNOU_KEN = G_SchReader.GetString("FUNOU_KEN_S")
                            '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
                            If Furi_Date_BK <> str�O��_�U�֓� Then
                                OraReader.Close()
                                G_SchReader.Close()
                                Exit Select
                            End If
                            '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END
                            'OraReader.Close() '2010/10/21 �R�����g�A�E�g
                            '---------------------------------------------------------------------------------------
                            If dblFUNOU_KEN = 0 Then
                                '2010/10/21 Exit Select���Ȃ� ��������
                                'Exit Select
                                'End If
                            Else
                                '2010/10/21 Exit Select���Ȃ� �����܂�

                                '���z������ŁA�挎�̕s�\������0���ȏゾ�����ꍇ�A�挎�s�\�����C���T�[�g
                                SQL = New StringBuilder(128)
                                SQL.Append(" SELECT * FROM G_MEIMAST")
                                SQL.Append(" WHERE")
                                SQL.Append(" GAKKOU_CODE_M = " & SQ(STR�w�Z�R�[�h))
                                SQL.Append(" AND FURI_DATE_M = " & SQ(str�O��_�U�֓�))
                                SQL.Append(" AND (FURI_KBN_M =  '0' OR FURI_KBN_M = '1')")
                                '  SQL.Append( " FURI_KBN_M ='0'")
                                ''  SQL.Append(" AND SEIKYU_TUKI_M ='" & strNEN & strNEGETU & "'")

                                ' 2017/05/25 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
                                'SQL.Append(" AND FURIKETU_CODE_M <> '0'")
                                SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
                                ' 2017/05/25 �^�X�N�j���� CHG �yOT�z(�ѓc�M�� �ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

                                '2011/06/16 �W���ŏC�� �O�N�x���͎����z���Ȃ��i���o���Ȃ��j------------------START                                '2011/05/30 �C��
                                If CInt(STR������) >= 4 And CInt(STR������) <= 12 Then                  '�U�֓��́u���v���S�`�P�Q���������炻�̔N�̂S���ȍ~�̂ݒ��o����
                                    SQL.Append(" AND FURI_DATE_M >= '" & str�����N���x.Substring(0, 4) & "0401'  ")
                                ElseIf CInt(STR������) >= 1 And CInt(STR������) <= 3 Then               '�U�֓��́u���v���P�`�R���������炻�̑O�̔N�̂S���ȍ~�̂ݒ��o����
                                    SQL.Append(" AND FURI_DATE_M >= '" & CStr(Val(str�����N���x.Substring(0, 4)) - 1) & "0401'  ")
                                End If
                                '2011/06/16 �W���ŏC�� �O�N�x���͎����z���Ȃ��i���o���Ȃ��j------------------END

                                OraReader = New MyOracleReader(MainDB)

                                If OraReader.DataReader(SQL) = False Then
                                    OraReader.Close()
                                    '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
                                    G_SchReader.Close()
                                    '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END
                                    Exit Select
                                End If

                                While OraReader.EOF = False

                                    LNG�U�֋��z���v = OraReader.GetString("SEIKYU_KIN_M")

                                    If INSERT_PRTWORK_MEIMAST(OraReader) = False Then
                                        OraReader.Close()
                                        '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
                                        G_SchReader.Close()
                                        '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END
                                        Exit Function
                                    End If
                                    '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
                                    Funou_FLG = True
                                    '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END
                                    OraReader.NextRead()
                                End While
                                OraReader.Close()
                            End If '2010/10/21 Exit Select���Ȃ� End If�ǉ�
                            '2010/10/21 ���z���������X�P�W���[���������ꍇ�̑Ή��ǉ�
                            '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
                            Furi_Date_BK = str�O��_�U�֓�
                            '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END
                            G_SchReader.NextRead()
                        End While
                        G_SchReader.Close()
                        '------------------------------------------------------------
                End Select
            End If
            '�����̑Ώۃf�[�^�𐶓k�}�X�^�r���[���璊�o
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM SEITOMASTVIEW WHERE ")
            SQL.Append(" SEITOMASTVIEW.GAKKOU_CODE_O  =" & SQ(STR�w�Z�R�[�h))
            SQL.Append(" AND�@(")

            '�Ώۊw�N�̓X�P�W���[���̊w�N�t���O��ON�̂���
            If STR�P�w�N = "1" Then
                SQL.Append(" SEITOMASTVIEW.GAKUNEN_CODE_O = 1")
                If STR�Q�w�N = "1" Or STR�R�w�N = "1" Or STR�S�w�N = "1" Or STR�T�w�N = "1" Or STR�U�w�N = "1" Or STR�V�w�N = "1" Or STR�W�w�N = "1" Or STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�Q�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 2")
                If STR�R�w�N = "1" Or STR�S�w�N = "1" Or STR�T�w�N = "1" Or STR�U�w�N = "1" Or STR�V�w�N = "1" Or STR�W�w�N = "1" Or STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�R�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 3")
                If STR�S�w�N = "1" Or STR�T�w�N = "1" Or STR�U�w�N = "1" Or STR�V�w�N = "1" Or STR�W�w�N = "1" Or STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�S�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 4")
                If STR�T�w�N = "1" Or STR�U�w�N = "1" Or STR�V�w�N = "1" Or STR�W�w�N = "1" Or STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�T�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 5")
                If STR�U�w�N = "1" Or STR�V�w�N = "1" Or STR�W�w�N = "1" Or STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�U�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 6")
                If STR�V�w�N = "1" Or STR�W�w�N = "1" Or STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�V�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 7")
                If STR�W�w�N = "1" Or STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�W�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 8")
                If STR�X�w�N = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR�X�w�N = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 9)")
            End If
            '�X�P�W���[���̐������Ɠ������k�}�X�^�r��
            SQL.Append(" AND SEITOMASTVIEW.TUKI_NO_O =" & SQ(STR������))
            '�U�֕��@�i0:�����U�ցj
            SQL.Append(" AND SEITOMASTVIEW.FURIKAE_O = '0'")
            '���敪=0 �i�ʏ�j
            SQL.Append(" AND SEITOMASTVIEW.KAIYAKU_FLG_O  =" & "'0'")
            '2006/10/16�@�U�֋��z��0�~�̃f�[�^�͈󎚂��Ȃ��悤�ɏC��
            '2013/01/23 saitou ��_�M�� DEL -------------------------------------------------->>>>
            '�U�֋��z��0�~���ΏۂƂ���B
            'SQL.Append(" AND (SEITOMASTVIEW.KINGAKU01_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU02_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU03_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU04_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU05_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU06_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU07_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU08_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU09_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU10_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU11_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU12_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU13_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU14_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU15_O  <> 0) ")
            '2013/01/23 saitou ��_�M�� DEL --------------------------------------------------<<<<

            OraReader = New MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = False Then
                '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------START
                If Funou_FLG = False Then
                    If txtGAKKOU_CODE.Text <> "9999999999" Then
                        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    OraReader.Close()
                    Exit Function
                End If
                'If txtGAKKOU_CODE.Text <> "9999999999" Then
                '    MessageBox.Show("����Ώۃf�[�^�����݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'End If
                'OraReader.Close()
                'Exit Function
                '2011/06/16 �W���ŏC�� �����z���͒��߂̂݊܂߂�------------------END
            End If

            While OraReader.EOF = False

                '�U�֋��z���v�v�Z
                LNG�U�֋��z���v = OraReader.GetInt64("KINGAKU01_O") _
                                + OraReader.GetInt64("KINGAKU02_O") _
                                + OraReader.GetInt64("KINGAKU03_O") _
                                + OraReader.GetInt64("KINGAKU04_O") _
                                + OraReader.GetInt64("KINGAKU05_O") _
                                + OraReader.GetInt64("KINGAKU06_O") _
                                + OraReader.GetInt64("KINGAKU07_O") _
                                + OraReader.GetInt64("KINGAKU08_O") _
                                + OraReader.GetInt64("KINGAKU09_O") _
                                + OraReader.GetInt64("KINGAKU10_O") _
                                + OraReader.GetInt64("KINGAKU11_O") _
                                + OraReader.GetInt64("KINGAKU12_O") _
                                + OraReader.GetInt64("KINGAKU13_O") _
                                + OraReader.GetInt64("KINGAKU14_O") _
                                + OraReader.GetInt64("KINGAKU15_O")


                If INSERT_PRTWORK_SEITOMASTVIEW(OraReader) = False Then
                    OraReader.Close()
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            OraReader.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�e�[�u���쐬)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try


        PFUNC_PRTWORK_SEITOINS = True

    End Function
    Function INSERT_PRTWORK_SEITOMASTVIEW(ByVal OraReader As MyOracleReader) As Boolean
        INSERT_PRTWORK_SEITOMASTVIEW = False

        Dim SQL As New StringBuilder
        Try
            SQL.Append("INSERT INTO KZFMAST.G_PRTWORK2 ")
            SQL.Append(" (GAKKOU_CODE_P ")
            SQL.Append(", NENDO_P ")
            SQL.Append(", TUUBAN_P ")
            SQL.Append(", SEIKYU_TUKI_P ")
            SQL.Append(", FURI_DATE_P ")
            SQL.Append(", GAKUNEN_CODE_P ")
            SQL.Append(", CLASS_CODE_P ")
            SQL.Append(", SEITO_NO_P ")
            SQL.Append(", SEITO_KNAME_P ")
            SQL.Append(", SEITO_NNAME_P ")
            SQL.Append(", TKIN_NO_P ")
            SQL.Append(", TSIT_NO_P ")
            SQL.Append(", KAMOKU_P ")
            SQL.Append(", KOUZA_P ")
            SQL.Append(", MEIGI_KNAME_P ")
            SQL.Append(", HIMOKU_ID_P ")
            SQL.Append(", KINGAKU_P ")
            SQL.Append(", KINGAKU01_P ")
            SQL.Append(", KINGAKU02_P ")
            SQL.Append(", KINGAKU03_P ")
            SQL.Append(", KINGAKU04_P ")
            SQL.Append(", KINGAKU05_P ")
            SQL.Append(", KINGAKU06_P ")
            SQL.Append(", KINGAKU07_P ")
            SQL.Append(", KINGAKU08_P ")
            SQL.Append(", KINGAKU09_P ")
            SQL.Append(", KINGAKU10_P ")
            SQL.Append(", KINGAKU11_P ")
            SQL.Append(", KINGAKU12_P ")
            SQL.Append(", KINGAKU13_P ")
            SQL.Append(", KINGAKU14_P ")
            SQL.Append(", KINGAKU15_P ")
            SQL.Append(", YOBI01_P ")
            SQL.Append(", YOBI02_P ")
            SQL.Append(", YOBI03_P ")
            SQL.Append(", YOBI04_P ")
            SQL.Append(", YOBI05_P )")
            SQL.Append(" VALUES ( ")
            '�w�Z�R�[�h
            SQL.Append(SQ(STR�w�Z�R�[�h))
            '���w�N�x
            SQL.Append("," & SQ(OraReader.GetString("NENDO_O")))
            '�ʔ�
            SQL.Append("," & OraReader.GetInt("TUUBAN_O"))
            '������
            SQL.Append("," & SQ(str�����N���x.Substring(4, 2)))
            '�U�֓�
            SQL.Append("," & SQ(STR_FURIKAE_DATE(1)))
            '�w�N�R�[�h
            SQL.Append("," & OraReader.GetInt("GAKUNEN_CODE_O"))
            '�N���X�R�[�h
            SQL.Append("," & OraReader.GetInt("CLASS_CODE_O"))
            '���k�ԍ�
            SQL.Append("," & "'" & OraReader.GetString("SEITO_NO_O") & "'")
            '���k���i�J�i�j
            SQL.Append("," & "'" & OraReader.GetString("SEITO_KNAME_O") & "'")
            '���k���i�����j
            SQL.Append("," & SQ(IIf(OraReader.GetString("SEITO_NNAME_O") = "", Space(50), OraReader.GetString("SEITO_NNAME_O"))))
            '�戵���Z�@��
            SQL.Append("," & "'" & OraReader.GetString("TKIN_NO_O") & "'")
            '�戵�x�X�R�[�h
            SQL.Append("," & "'" & OraReader.GetString("TSIT_NO_O") & "'")
            '�Ȗ�
            SQL.Append("," & "'" & OraReader.GetString("KAMOKU_O") & "'")
            '�����ԍ�
            SQL.Append("," & "'" & OraReader.GetString("KOUZA_O") & "'")
            '�������`�l�J�i
            SQL.Append("," & "'" & OraReader.GetString("MEIGI_KNAME_O") & "'")
            '��ڂh�c
            SQL.Append("," & "'" & OraReader.GetString("HIMOKU_ID_O") & "'")
            '�U�֋��z�i���v�j
            SQL.Append("," & LNG�U�֋��z���v)
            '�U�֋��z�i��ڂP�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU01_O"))
            '�U�֋��z�i��ڂQ�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU02_O"))
            '�U�֋��z�i��ڂR�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU03_O"))
            '�U�֋��z�i��ڂS�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU04_O"))
            '�U�֋��z�i��ڂT�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU05_O"))
            '�U�֋��z�i��ڂU�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU06_O"))
            '�U�֋��z�i��ڂV�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU07_O"))
            '�U�֋��z�i��ڂW�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU08_O"))
            '�U�֋��z�i��ڂX�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU09_O"))
            '�U�֋��z�i��ڂP�O�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU10_O"))
            '�U�֋��z�i��ڂP�P�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU11_O"))
            '�U�֋��z�i��ڂP�Q�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU12_O"))
            '�U�֋��z�i��ڂP�R�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU13_O"))
            '�U�֋��z�i��ڂP�S�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU14_O"))
            '�U�֋��z�i��ڂP�T�j
            SQL.Append("," & OraReader.GetInt64("KINGAKU15_O"))

            '�\��1
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��2
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��3
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��4
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��5
            SQL.Append("," & "'" & Space(20) & "')")

            MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try


        INSERT_PRTWORK_SEITOMASTVIEW = True
    End Function
    Function INSERT_PRTWORK_MEIMAST(ByVal OraReader As MyOracleReader) As Boolean
        INSERT_PRTWORK_MEIMAST = False
        Dim SQL As New StringBuilder
        Try
            SQL.Append("INSERT INTO KZFMAST.G_PRTWORK2 ")
            SQL.Append(" (GAKKOU_CODE_P ")
            SQL.Append(", NENDO_P ")
            SQL.Append(", TUUBAN_P ")
            SQL.Append(", SEIKYU_TUKI_P ")
            SQL.Append(", FURI_DATE_P ")
            SQL.Append(", GAKUNEN_CODE_P ")
            SQL.Append(", CLASS_CODE_P ")
            SQL.Append(", SEITO_NO_P ")
            SQL.Append(", SEITO_KNAME_P ")
            SQL.Append(", SEITO_NNAME_P ")
            SQL.Append(", TKIN_NO_P ")
            SQL.Append(", TSIT_NO_P ")
            SQL.Append(", KAMOKU_P ")
            SQL.Append(", KOUZA_P ")
            SQL.Append(", MEIGI_KNAME_P ")
            SQL.Append(", HIMOKU_ID_P ")
            SQL.Append(", KINGAKU_P ")
            SQL.Append(", KINGAKU01_P ")
            SQL.Append(", KINGAKU02_P ")
            SQL.Append(", KINGAKU03_P ")
            SQL.Append(", KINGAKU04_P ")
            SQL.Append(", KINGAKU05_P ")
            SQL.Append(", KINGAKU06_P ")
            SQL.Append(", KINGAKU07_P ")
            SQL.Append(", KINGAKU08_P ")
            SQL.Append(", KINGAKU09_P ")
            SQL.Append(", KINGAKU10_P ")
            SQL.Append(", KINGAKU11_P ")
            SQL.Append(", KINGAKU12_P ")
            SQL.Append(", KINGAKU13_P ")
            SQL.Append(", KINGAKU14_P ")
            SQL.Append(", KINGAKU15_P ")
            SQL.Append(", YOBI01_P ")
            SQL.Append(", YOBI02_P ")
            SQL.Append(", YOBI03_P ")
            SQL.Append(", YOBI04_P ")
            SQL.Append(", YOBI05_P )")
            SQL.Append(" VALUES ( ")
            '�w�Z�R�[�h
            SQL.Append("'" & STR�w�Z�R�[�h & "'")
            '���w�N�x
            SQL.Append("," & "'" & OraReader.GetString("NENDO_M") & "'")
            '�ʔ�
            SQL.Append("," & CInt(OraReader.GetString("TUUBAN_M")))
            '������
            Dim strTUKI As String
            strTUKI = OraReader.GetString("SEIKYU_TUKI_M")
            strTUKI = strTUKI.Substring(4, 2)
            SQL.Append(",'" & strTUKI)
            '�U�֓�
            SQL.Append("'," & "'" & STR_FURIKAE_DATE(1) & "'")
            '�w�N�R�[�h
            SQL.Append("," & OraReader.GetInt("GAKUNEN_CODE_M"))
            '�N���X�R�[�h
            SQL.Append("," & OraReader.GetInt("CLASS_CODE_M"))
            '���k�ԍ�
            SQL.Append("," & "'" & OraReader.GetString("SEITO_NO_M") & "'")
            '���k���i�J�i�j
            SQL.Append("," & "'" & Space(1) & "'")
            '���k���i�����j
            SQL.Append("," & "'" & Space(1) & "'")
            '�戵���Z�@��
            SQL.Append("," & "'" & OraReader.GetString("TKIN_NO_M") & "'")
            '�戵�x�X�R�[�h
            SQL.Append("," & "'" & OraReader.GetString("TSIT_NO_M") & "'")
            '�Ȗ�
            SQL.Append("," & "'" & OraReader.GetString("TKAMOKU_M") & "'")
            '�����ԍ�
            SQL.Append("," & "'" & OraReader.GetString("TKOUZA_M") & "'")
            '�������`�l�J�i
            SQL.Append("," & "'" & OraReader.GetString("TMEIGI_KNM_M") & "'")
            '��ڂh�c
            SQL.Append("," & "'" & OraReader.GetString("HIMOKU_ID_M") & "'")
            '�U�֋��z�i���v�j
            SQL.Append("," & LNG�U�֋��z���v)
            '�U�֋��z�i��ڂP�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU1_KIN_M"))
            '�U�֋��z�i��ڂQ�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU2_KIN_M"))
            '�U�֋��z�i��ڂR�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU3_KIN_M"))
            '�U�֋��z�i��ڂS�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU4_KIN_M"))
            '�U�֋��z�i��ڂT�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU5_KIN_M"))
            '�U�֋��z�i��ڂU�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU6_KIN_M"))
            '�U�֋��z�i��ڂV�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU7_KIN_M"))
            '�U�֋��z�i��ڂW�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU8_KIN_M"))
            '�U�֋��z�i��ڂX�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU9_KIN_M"))
            '�U�֋��z�i��ڂP�O�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU10_KIN_M"))
            '�U�֋��z�i��ڂP�P�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU11_KIN_M"))
            '�U�֋��z�i��ڂP�Q�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU12_KIN_M"))
            '�U�֋��z�i��ڂP�R�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU13_KIN_M"))
            '�U�֋��z�i��ڂP�S�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU14_KIN_M"))
            '�U�֋��z�i��ڂP�T�j
            SQL.Append("," & OraReader.GetInt64("HIMOKU15_KIN_M"))
            '�\��1
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��2
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��3
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��4
            SQL.Append("," & "'" & Space(20) & "'")
            '�\��5
            SQL.Append("," & "'" & Space(20) & "')")

            '2010/10/21 ���U�E�ĐU�Ƃ��ɕs�\�̏ꍇ�͈�Ӑ���ᔽ�ɂȂ�̂Ŗ�������
            'MainDB.ExecuteNonQuery(SQL)
            MainDB.ExecuteNonQuery(SQL.ToString, True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try

        INSERT_PRTWORK_MEIMAST = True
    End Function
    Private Function PFUNC_PRTWORK_DEL() As Boolean
        Try
            PFUNC_PRTWORK_DEL = False

            Dim SQL As New StringBuilder
            SQL.Append("DELETE  FROM G_PRTWORK2")

            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�폜)", "���s", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_DEL = True
    End Function
#End Region

End Class
