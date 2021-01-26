Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT110
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' ���k���׃`�F�b�N���X�g���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Private STR�����N�� As String
    Private STR�w�Z�� As String
    '2006/10/11 ���[�̃\�[�g�@�\�ǉ�
    Dim STR���[�\�[�g�� As String
    Dim Str_Kubn As String
    Dim STR�w�Z�R�[�h As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT110", "���k���ד��̓`�F�b�N���X�g������")
    Private Const msgTitle As String = "���k���ד��̓`�F�b�N���X�g������(KFGPRNT110)"
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
    Private Sub KFGPRNT110_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

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
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle
            Select Case True
                Case rdo����.Checked
                    Str_Kubn = "1"
                Case rdo�o��.Checked
                    Str_Kubn = "2"
            End Select

            If PFUNC_COMMON_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)
            LW.FuriDate = STR_FURIKAE_DATE(1)

            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab�w�Z��.Text = "�S�w�Z�Ώ�"
            End If

            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT DISTINCT G_SCHMAST.GAKKOU_CODE_S FROM G_SCHMAST,G_ENTMAST" & Str_Kubn)
            SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_S = " & SQ(txtGAKKOU_CODE.Text))
            End If
            '�U�֋敪�ݒ�
            If rdo����.Checked = True Then
                SQL.Append(" AND FURI_KBN_S ='2'")
            Else
                SQL.Append(" AND FURI_KBN_S ='3'")
            End If
            SQL.Append(" AND G_ENTMAST" & Str_Kubn & ".FURIKIN_E <> 0 ")
            SQL.Append(" AND G_SCHMAST.GAKKOU_CODE_S = G_ENTMAST" & Str_Kubn & ".GAKKOU_CODE_E")
            SQL.Append(" AND G_SCHMAST.FURI_DATE_S = G_ENTMAST" & Str_Kubn & ".FURI_DATE_E")
            SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

            '�X�P�W���[�����݃`�F�b�N
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�Ώۃf�[�^�����݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "���k���ד��̓`�F�b�N���X�g"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim intINSATU_FLG2 As Integer = 0
            While OraReader.EOF = False

                STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                '���[�\�[�g���̎擾
                If PFUNC_GAKNAME_GET(False) = False Then
                    STR���[�\�[�g�� = "0"
                End If

                '���O�C��ID,�w�Z�R�[�h,�U�֓�,����敪(1:���� 2:�o��),�\�[�g��
                Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & Str_Kubn & "," & STR���[�\�[�g��
                nRet = ExeRepo.ExecReport("KFGP030.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "���k���ד��̓`�F�b�N���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                OraReader.NextRead()
            End While
            txtGAKKOU_CODE.Focus()
            MessageBox.Show(String.Format(MSG0014I, "���k���ד��̓`�F�b�N���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
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

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET(Optional ByVal NameChg As Boolean = True) As Boolean

        '�w�Z���̐ݒ�
        PFUNC_GAKNAME_GET = False

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            If Trim(STR�w�Z�R�[�h) = "9999999999" Then
                lab�w�Z��.Text = ""
            Else
                SQL.Append(" SELECT ")
                SQL.Append(" GAKMAST1.*")
                SQL.Append(",MEISAI_OUT_T")
                SQL.Append(" FROM GAKMAST1,GAKMAST2")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
                SQL.Append(" AND GAKKOU_CODE_G = " & SQ(STR�w�Z�R�[�h))

                If OraReader.DataReader(SQL) = False Then

                    MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    lab�w�Z��.Text = ""
                    STR���[�\�[�g�� = 0

                    Exit Function
                End If

                If NameChg Then lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")
                STR���[�\�[�g�� = OraReader.GetInt("MEISAI_OUT_T")

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z����)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_GAKNAME_GET = True
    End Function
    Private Function PFUNC_SCHMAST_GET(ByVal str�w�Z�R�[�h As String) As Boolean

        PFUNC_SCHMAST_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            '�X�P�W���[���}�X�^�`�F�b�N
            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            If str�w�Z�R�[�h <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_S = " & SQ(str�w�Z�R�[�h))
            End If

            '�U�֋敪�ݒ�
            If rdo����.Checked = True Then
                SQL.Append(" AND FURI_KBN_S ='2'")
            Else
                SQL.Append(" AND FURI_KBN_S ='3'")
            End If
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�ΏۃX�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            STR�����N�� = OraReader.GetString("NENGETUDO_S")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_COMMON_CHECK() As Boolean

        PFUNC_COMMON_CHECK = False

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
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
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                '�w�Z�}�X�^���݃`�F�b�N
                SQL.Append("SELECT MEISAI_OUT_T")
                SQL.Append(" FROM GAKMAST2")
                SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(txtGAKKOU_CODE.Text))

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("���͂��ꂽ�w�Z�R�[�h�����݂��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If
                STR���[�\�[�g�� = OraReader.GetString("MEISAI_OUT_T")
            End If

            STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
            STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

            '���o���敪
            If rdo����.Checked = False And rdo�o��.Checked = False Then
                MessageBox.Show("�����A�o�����I������Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            If PFUNC_SCHMAST_GET(txtGAKKOU_CODE.Text) = False Then
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If

            '���z0�~�ȏ�̃f�[�^�����݂��邩����
            SQL = New StringBuilder
            SQL.Append(" SELECT * ")
            SQL.Append(" FROM KZFMAST.G_ENTMAST" & Str_Kubn)
            '�U�֓�
            SQL.Append(" WHERE FURI_DATE_E = " & SQ(STR_FURIKAE_DATE(1)))
            '�������z
            SQL.Append(" AND FURIKIN_E <> 0 ")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_E = " & SQ(txtGAKKOU_CODE.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("���ׂ�������0�~�ȏ�̖��ׂ��o�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_COMMON_CHECK = True

    End Function
#End Region

    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 �w�Z�R�[�h�[������
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '�w�Z���̎擾
            STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If

    End Sub
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

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
        '�w�Z���̎擾
        lab�w�Z��.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
