Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT040
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �������X�g���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Dim Str_Report_Path As String
    Dim STR������ As String
    '2006/10/20
    Dim STR�w�Z�R�[�h As String

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT040", "�������X�g������")
    Private Const msgTitle As String = "�������X�g������(KFGPRNT040)"
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
    Private Sub KFGPRNT040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

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
        Dim OraReader As MyOracleReader
        Dim SQL As New StringBuilder
        Dim Flg As String = ""
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            '���̓`�F�b�N
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            '�\�[�g���ԑI���`�F�b�N
            If chk���k�ԍ���.Checked = False And chk�ʔԏ�.Checked = False And chk������������.Checked = False And chk���ʏ�.Checked = False Then
                MessageBox.Show("�\�[�g���Ԃ��P���I������Ă��܂���B ", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                chk���k�ԍ���.Focus()
                Exit Sub
            End If
            Flg = IIf(chk���k�ԍ���.Checked, "1,", "0,") & IIf(chk�ʔԏ�.Checked, "1,", "0,") & _
                  IIf(chk������������.Checked, "1,", "0,") & IIf(chk���ʏ�.Checked, "1", "0")


            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            STR�w�Z�R�[�h = txtGAKKOU_CODE.Text

            If PFUC_SCHMAST_GET() <> 0 Then
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            If PFUNC_�������X�g����\����() = False Then
                '����ΏۂȂ����b�Z�[�W
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "�������X�g"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                OraReader = New MyOracleReader(MainDB)
                SQL.Append(" SELECT distinct GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")

                '2006/10/11 �s�\�t���O�������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" AND FUNOU_KEN_S > 0 ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                '�X�P�W���[�����݃`�F�b�N
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("�X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK)
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                While OraReader.EOF = False

                    STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & Flg
                    nRet = ExeRepo.ExecReport("KFGP022.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "�������X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select


                    OraReader.NextRead()
                End While
            Else
                Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & Flg
                nRet = ExeRepo.ExecReport("KFGP022.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "�������X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
            End If
            MessageBox.Show(String.Format(MSG0014I, "�������X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
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

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '�w�Z������̊w�Z�R�[�h�ݒ�
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '�w�Z���̎擾
        lab�w�Z��.Text = cmbGakkouName.Text

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
                '�w�Z���̎擾
                If PFUC_GAKNAME_GET() = False Then
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
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False

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

        PFUNC_Nyuryoku_Check = True

    End Function

    Private Function PFUC_GAKNAME_GET() As Boolean
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As MyOracle = Nothing
        Try

            '�w�Z���̐ݒ�
            PFUC_GAKNAME_GET = True

            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab�w�Z��.Text = "�S�w�Z�Ώ�"
            Else
                SQL.Append("SELECT * FROM KZFMAST.GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G  = " & SQ(txtGAKKOU_CODE.Text.Trim))
                '�w�Z�}�X�^�P���݃`�F�b�N
                OraDB = New MyOracle
                OraReader = New MyOracleReader(OraDB)
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab�w�Z��.Text = ""
                    txtGAKKOU_CODE.Focus()
                    PFUC_GAKNAME_GET = False
                    Exit Function
                End If

                lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z���擾)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try

    End Function
    Private Function PFUC_SCHMAST_GET() As Integer

        PFUC_SCHMAST_GET = 0
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append("SELECT * FROM KZFMAST.G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_S    = " & SQ(STR_FURIKAE_DATE(1)))
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_S = " & SQ(txtGAKKOU_CODE.Text.Trim))
            Else
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            End If
            SQL.Append(" AND (FURI_KBN_S = '0' OR  FURI_KBN_S = '1')")

            '�X�P�W���[�������݃`�F�b�N
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�ΏۃX�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                PFUC_SCHMAST_GET = 1
                Exit Function
            Else
                SQL = New StringBuilder
                SQL.Append("SELECT * FROM KZFMAST.G_SCHMAST WHERE")
                SQL.Append(" FURI_DATE_S    = " & SQ(STR_FURIKAE_DATE(1)))
                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    SQL.Append(" AND GAKKOU_CODE_S  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
                Else
                    SQL.Append(" AND FUNOU_FLG_S  = '1'")
                    SQL.Append(" AND FUNOU_KEN_S  > 0")
                End If
                SQL.Append(" AND (FURI_KBN_S = '0' OR  FURI_KBN_S = '1')")

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("�s�\������0���ȏ�̃X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    PFUC_SCHMAST_GET = 1
                    Exit Function
                End If

                If OraReader.GetString("FUNOU_FLG_S") = "0" Then
                    MessageBox.Show("���̃X�P�W���[���͕s�\���ʍX�V�������������ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                If OraReader.GetString("FUNOU_KEN_S") = 0 Then
                    MessageBox.Show("���̃X�P�W���[���͕s�\������0���ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    PFUC_SCHMAST_GET = 1
                    Exit Function
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���擾)", "���s", ex.ToString)
            Return 1
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_�������X�g����\����() As Boolean

        PFUNC_�������X�g����\���� = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM G_MEIMAST")
            SQL.Append(" WHERE")
            SQL.Append(" FURIKETU_CODE_M <> 0")
            '2006/10/16 �ĐU�ς݃t���O�̃`�F�b�N���͂����B�ĐU����������U�̖������X�g���o�͂ł���悤�ɂ��邽�߁B
            'STR_SQL += " G_MEIMAST.SAIFURI_SUMI_M ='0'"
            'STR_SQL += " AND"
            SQL.Append(" AND FURI_DATE_M <=" & SQ(STR_FURIKAE_DATE(1)))
            SQL.Append(" AND ( FURI_KBN_M ='0' OR FURI_KBN_M ='1')")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_M =" & SQ(txtGAKKOU_CODE.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            PFUNC_�������X�g����\���� = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������X�g����\����)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function


#End Region
#Region " ���N�G��(�N�����|)"
    Private Function PFUC_SQLQuery_MAKE() As String
        Dim SSQL As String

        PFUC_SQLQuery_MAKE = ""

        SSQL = "SELECT "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.NENDO_M "
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M "
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M "
        SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M "
        SSQL = SSQL & ",G_MEIMAST.TUUBAN_M "
        SSQL = SSQL & ",G_MEIMAST.TKIN_NO_M "
        SSQL = SSQL & ",G_MEIMAST.TSIT_NO_M "
        SSQL = SSQL & ",G_MEIMAST.TKAMOKU_M "
        SSQL = SSQL & ",G_MEIMAST.TKOUZA_M "
        SSQL = SSQL & ",G_MEIMAST.TMEIGI_KNM_M "
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_TUKI_M "
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M "

        SSQL = SSQL & ",SEITOMAST.GAKKOU_CODE_O"
        SSQL = SSQL & ",SEITOMAST.NENDO_O "
        SSQL = SSQL & ",SEITOMAST.TUUBAN_O "
        'SSQL = SSQL & ",SEITOMAST.GAKUNEN_CODE_O "
        'SSQL = SSQL & ",SEITOMAST.CLASS_CODE_O "
        SSQL = SSQL & ",SEITOMAST.SEITO_NO_O "
        SSQL = SSQL & ",SEITOMAST.SEITO_KNAME_O "
        SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O "
        SSQL = SSQL & ",SEITOMAST.SEIBETU_O "

        SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"
        'SSQL = SSQL & ",GAKMAST1.GAKUNEN_NNAME_G"
        'SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"

        SSQL = SSQL & ",TENMAST.KIN_NO_N"
        SSQL = SSQL & ",TENMAST.SIT_NO_N"
        'SSQL = SSQL & ",TENMAST.KIN_NNAME_N "
        'SSQL = SSQL & ",TENMAST.SIT_NNAME_N "

        SSQL = SSQL & ",NVL(G_MEIMAST.GAKKOU_CODE_M,0)"
        SSQL = SSQL & ",NVL(G_MEIMAST.NENDO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.GAKUNEN_CODE_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.CLASS_CODE_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.SEITO_NO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TUUBAN_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TKIN_NO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TSIT_NO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TKAMOKU_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TKOUZA_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TMEIGI_KNM_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.SEIKYU_TUKI_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.SEIKYU_KIN_M,0) "

        SSQL = SSQL & ",NVL(SEITOMAST.GAKKOU_CODE_O,0)"
        SSQL = SSQL & ",NVL(SEITOMAST.NENDO_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.TUUBAN_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEITO_NO_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEITO_KNAME_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEITO_NNAME_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEIBETU_O,0) "

        SSQL = SSQL & ",NVL(GAKMAST1.GAKKOU_NNAME_G,0)"

        SSQL = SSQL & ",NVL(TENMAST.KIN_NO_N,0)"
        SSQL = SSQL & ",NVL(TENMAST.SIT_NO_N,0)"

        SSQL = SSQL & " FROM "
        SSQL = SSQL & "  KZFMAST.G_MEIMAST"
        SSQL = SSQL & " ,KZFMAST.SEITOMAST"
        'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
        SSQL = SSQL & " ,KZFMAST.GAKMAST1"
        SSQL = SSQL & " ,KZFMAST.TENMAST"

        SSQL = SSQL & " WHERE "
        SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = SEITOMAST.GAKKOU_CODE_O  "
        SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMAST.NENDO_O "
        SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMAST.TUUBAN_O "
        SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = SEITOMAST.HIMOKU_ID_O "

        'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G  "

        SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N "
        SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N "

        SSQL = SSQL & " AND GAKMAST1.GAKUNEN_CODE_G = 1"

        SSQL = SSQL & " AND SEITOMAST.TUKI_NO_O ='" & Format(CInt(txtFuriDateM.Text), "00") & "'"

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR�w�Z�R�[�h & "'"

        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0"
        '2006/10/16�@�ĐU�t���O�̃`�F�b�N���͂����B�ĐU����������U�̖������X�g���o�͂��邽�߁B
        'SSQL = SSQL & " AND G_MEIMAST.SAIFURI_SUMI_M  =" & "'0'"
        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M     =" & "'" & STR_FURIKAE_DATE(1) & "'"
        SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M     =" & "'0'" & " OR G_MEIMAST.FURI_KBN_M = " & "'1')"

        '�\�[�g����
        SSQL = SSQL & " ORDER BY "
        '2006/10/12�@�w�Z�R�[�h���\�[�g�����ɒǉ�
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M ASC"
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M ASC"
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M ASC"
        'If chk���k�ԍ���.Checked = True OR chk�ʔԏ�.Checked = True OR _
        '   chk������������.Checked = True OR chk���ʏ�.Checked = True Then
        '    SSQL += ","
        'End If

        If chk���k�ԍ���.Checked = True Then
            SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M ASC"
        End If

        If chk�ʔԏ�.Checked = True Then
            'If chk���k�ԍ���.Checked = True Then
            '    SSQL = SSQL & ","
            'End If
            SSQL = SSQL & ",G_MEIMAST.NENDO_M ASC"
            SSQL = SSQL & ",G_MEIMAST.TUUBAN_M ASC"
        End If

        If chk������������.Checked = True Then
            'If chk���k�ԍ���.Checked = True OR chk�ʔԏ�.Checked = True Then
            '    SSQL = SSQL & ","
            'End If
            '2007/02/14 ���k�J�i�����ɏo��
            'SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O ASC"
            SSQL = SSQL & ",SEITOMAST.SEITO_KNAME_O ASC"
        End If

        If chk���ʏ�.Checked = True Then
            'If chk���k�ԍ���.Checked = True OR chk�ʔԏ�.Checked = True OR chk������������.Checked = True Then
            '    SSQL = SSQL & ","
            'End If
            SSQL = SSQL & ",SEITOMAST.SEIBETU_O ASC"
        End If

        PFUC_SQLQuery_MAKE = SSQL

    End Function

#End Region
End Class
