Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Imports System.Data.OracleClient
Public Class KFGMAST040

    '��ڃ}�X�^���ʋ��z�ޔ�
    Private Str_Spread(11, 14) As String
    Private int���v���z As Integer
    Private intRC As Integer

    '�����`�F�b�N�p
    Private strKIGYO_CODE As String
    Private strFURI_CODE As String
    '�Ǎ��pDB
    Private MainDB As CASTCommon.MyOracle
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
    Private intMaxHimokuCnt As Integer = CInt(IIf(STR_HIMOKU_PTN = "1", 15, 10))
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST040", "���k�}�X�^�����e�i���X���")
    Private Const msgTitle As String = "���k�}�X�^�����e�i���X���(KFGMAST040)"

    ' 2010/09/16 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
    Private strCSV_FILE_NAME As String
    '2017/04/05 saitou ���t�M��(RSV2�W��) DEL ���k�}�X�^�����e���[���P ---------------------------------------- START
    'Private Const SeitoMastMaxColumns As Integer = 65
    '2017/04/05 saitou ���t�M��(RSV2�W��) DEL ----------------------------------------------------------------- END
    ' 2010/09/16 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<
    '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ���k�}�X�^�����e���[���P ---------------------------------------- START
    '���k�}�X�^�����e���[�o�͂ɕK�v�ȃL�[���ڂ��i�[����\����
    Private Structure strcSeitomastMenteListInfo
        Dim GakkouCode As String
        Dim Nendo As String
        Dim Tuuban As String
    End Structure
    Private SeitomastMenteListInfo As strcSeitomastMenteListInfo

    Private Structure strcSeitomastMenteUpdateListInfo
        Dim GakkouCode As String
        Dim Nendo As String
        Dim Tuuban As String
        Dim GakunenCode As String
        Dim ClassCode As String
        Dim SeitoNo As String
        Dim GakkouName As String
    End Structure
    Private SeitomastMenteUpdateListInfo As strcSeitomastMenteUpdateListInfo

    Private dtBeforeSeitomast As DataTable
    Private dtAfterSeitomast As DataTable
    '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ----------------------------------------------------------------- END
    '2017/11/28 �^�X�N�j���� ADD (�W���ŏC��(��177)) -------------------- START
    Private STR_TYOUSIUMU_TXT As String = "KFGMAST040_���q�L���t���O.TXT"
    '2017/11/28 �^�X�N�j���� ADD (�W���ŏC��(��177)) -------------------- END

#Region " Form Load "
    Private Sub KFGMAST040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "���k�}�X�^�����e�i���X���"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SEIBETU_TXT, cmbSEIBETU) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbSEIBETU)")
                MessageBox.Show(String.Format(MSG0013E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_FURIKAE_TXT, cmbFURIKAE) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbFURIKAE)")
                MessageBox.Show(String.Format(MSG0013E, "�U�֕��@"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbKAMOKU)")
                MessageBox.Show(String.Format(MSG0013E, "�Ȗ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SINKYU_TXT, cmbSINKYU_KBN) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbSINKYU_KBN)")
                MessageBox.Show(String.Format(MSG0013E, "�i���敪"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_KAIYAKU_TXT, cmbKAIYAKU_FLG) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbKAIYAKU_FLG)")
                MessageBox.Show(String.Format(MSG0013E, "���敪"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            '2017/11/28 �^�X�N�j���� ADD (�W���ŏC��(��177)) -------------------- START
            '���q�L���t���O
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_TYOUSIUMU_TXT, cmbTYOUSI) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbTYOUSI)")
                MessageBox.Show(String.Format(MSG0013E, "���q�L���t���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            '2017/11/28 �^�X�N�j���� ADD (�W���ŏC��(��177)) -------------------- END

            Dim ArrayDGV As CustomDataGridView() = { _
            CustomDataGridView1, _
            CustomDataGridView2, _
            CustomDataGridView3, _
            CustomDataGridView4, _
            CustomDataGridView5, _
            CustomDataGridView6, _
            CustomDataGridView7, _
            CustomDataGridView8, _
            CustomDataGridView9, _
            CustomDataGridView10, _
            CustomDataGridView11, _
            CustomDataGridView12}

            For MonthCnt As Integer = 0 To ArrayDGV.Length - 1 Step 1
                Dim CmbCol As New DataGridViewComboBoxColumn
                Dim MonthCntWide As String = StrConv((MonthCnt + 1).ToString, VbStrConv.Wide)

                CmbCol.Items.Add("�ꗥ")
                CmbCol.Items.Add("��")

                CmbCol.DataPropertyName = "�������@" & MonthCntWide
                ArrayDGV(MonthCnt).Columns.Insert(ArrayDGV(MonthCnt).Columns("�������@" & MonthCntWide).Index, CmbCol)
                ArrayDGV(MonthCnt).Columns.Remove("�������@" & MonthCntWide)
                CmbCol.Name = "�������@"
                '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                ArrayDGV(MonthCnt).Rows.Add(intMaxHimokuCnt)
                'ArrayDGV(MonthCnt).Rows.Add(10)
                '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
            Next

            '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- START
            CustomDataGridView13.Rows.Add(intMaxHimokuCnt)
            '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- END

            '�R���{�{�b�N�X�̐擪�Ɉʒu�Â�

            cmbSEIBETU.SelectedIndex = 0 '���ʃR���{
            cmbFURIKAE.SelectedIndex = 0   '�U�֕��@�R���{
            cmbKAMOKU.SelectedIndex = 0  '�ȖڃR���{
            cmbSINKYU_KBN.SelectedIndex = 0   '�i���敪�R���{
            cmbKAIYAKU_FLG.SelectedIndex = 0  '���敪�R���{
            cmbTYOUSI.SelectedIndex = 0    '���q���R���{

            '���͋֎~����
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False

            btnKOUZA_CHK.Visible = True
            lblKOUZA_CHK.Visible = True

            '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���q�ݒ荀�ڂ̕\���^��\������j---------------------- START
            If STR_TYOUSI_KBN = "0" Then
                '���q���ڂ��\���ɂ���
                Label28.Visible = False
                cmbTYOUSI.Visible = False
                btnTyousi.Visible = False
            End If
            '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���q�ݒ荀�ڂ̕\���^��\������j---------------------- END

            '���[�h�ŊJ����
            MainDB = New CASTCommon.MyOracle
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " Button Click "
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        '�V�K�o�^����
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�J�n", "����", "")
            Dim Orareader As CASTCommon.MyOracleReader = Nothing

            Try
                '���͒l�`�F�b�N
                If fn_CheckEntry(0) = False Then
                    Exit Sub
                End If

                '�d���`�F�b�N
                '�V�����}�X�^������d���`�F�b�N���s�Ȃ�
                Dim SQL As New StringBuilder(128)
                SQL.Append(" SELECT * FROM SEITOMAST S1,SEITOMAST2 S2")
                SQL.Append(" WHERE (S1.GAKKOU_CODE_O = '" & Trim(txtGAKKOU_CODE.Text) & "'")    '���k�}�X�^�F�w�Z�R�[�h
                SQL.Append(" AND S1.NENDO_O = '" & Trim(txtNENDO.Text) & "' ")                  '���k�}�X�^�F���w�N�x
                SQL.Append(" AND S1.TUUBAN_O = " & CInt(txtTUUBAN.Text) & ")")                  '���k�}�X�^�F�ʔ�
                SQL.Append(" OR (S2.GAKKOU_CODE_O = '" & Trim(txtGAKKOU_CODE.Text) & "'")       '�V�����}�X�^�F�w�Z�R�[�h
                SQL.Append(" AND S2.NENDO_O = '" & Trim(txtNENDO.Text) & "'")                   '�V�����}�X�^�F���w�N�x
                SQL.Append(" AND S2.TUUBAN_O = " & CInt(txtTUUBAN.Text) & ")")                  '�V�����}�X�^�F�ʔ�

                Orareader = New CASTCommon.MyOracleReader(MainDB)

                If Orareader.DataReader(SQL) Then
                    MessageBox.Show(G_MSG0064W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()

                    Orareader.Close()
                    Orareader = Nothing
                    Exit Sub
                End If

                Orareader.Close()
                Orareader = Nothing

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)
            Finally
                If Not Orareader Is Nothing Then
                    Orareader.Close()
                    Orareader = Nothing
                End If
            End Try


            '�m�F���b�Z�[�W��\��
            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ���k�}�X�^�����e���[���P ---------------------------------------- START
            '�L�[���ڊi�[
            Me.SeitomastMenteListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteListInfo.Tuuban = Me.txtTUUBAN.Text
            '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ----------------------------------------------------------------- END

            'DB���쌋�ʃt���O
            Dim bret As Boolean = False

            Try
                'TRANS
                Call GFUNC_EXECUTESQL_TRANS("", 0)

                If fn_INSERTSEITOMAST(CustomDataGridView1, 4) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView2, 5) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView3, 6) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView4, 7) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView5, 8) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView6, 9) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView7, 10) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView8, 11) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView9, 12) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView10, 1) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView11, 2) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView12, 3) = False Then
                    Exit Try
                End If

                '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ���k�}�X�^�����e���[���P ---------------------------------------- START
                '���ɖ߂�
                '��ʃN���A
                Call fn_FormInitialize()
                '���͍��ڐ���
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtGAKUNEN_CODE.Enabled = True
                txtCLASS_CODE.Enabled = True
                txtSEITO_NO.Enabled = True

                cmbKana.Enabled = True
                cmbGakkouName.Enabled = True

                '���͋֎~�{�^������
                btnAdd.Enabled = True
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
                btnFind.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = True
                txtTUUBAN.Focus()

                '' 2010/09/16 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
                '' ��ʍ��ڐ���𐶓k�}�X�^�`�F�b�N���X�g�o�͌�ɍs���悤�ɂ���i���ڂ��擾�ł�������ł��Ȃ����߁j
                ' ''��ʃN���A
                ''Call fn_FormInitialize()
                ' ''���͍��ڐ���
                ''txtGAKKOU_CODE.Enabled = True
                ''txtNENDO.Enabled = True
                ''txtTUUBAN.Enabled = True
                ''txtGAKUNEN_CODE.Enabled = True
                ''txtCLASS_CODE.Enabled = True
                ''txtSEITO_NO.Enabled = True

                ''cmbKana.Enabled = True
                ''cmbGakkouName.Enabled = True

                ' ''���͋֎~�{�^������
                ''btnAdd.Enabled = True
                ''btnUPDATE.Enabled = False
                ''btnDelete.Enabled = False
                ''btnFind.Enabled = True
                ''btnEraser.Enabled = True
                ''btnEnd.Enabled = True
                ''btnTuuban.Enabled = True
                ''txtTUUBAN.Focus()
                '' 2010/09/16 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<
                '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ----------------------------------------------------------------- END

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' 2010/09/16 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
                    ' ��������s����
                    If fn_CreateCSV_ADDDEL("0") Then
                        Call fn_Print(0)
                    End If

                    '2017/04/05 saitou ���t�M��(RSV2�W��) DEL ���k�}�X�^�����e���[���P ---------------------------------------- START
                    ''��ʃN���A
                    'Call fn_FormInitialize()
                    ''���͍��ڐ���
                    'txtGAKKOU_CODE.Enabled = True
                    'txtNENDO.Enabled = True
                    'txtTUUBAN.Enabled = True
                    'txtGAKUNEN_CODE.Enabled = True
                    'txtCLASS_CODE.Enabled = True
                    'txtSEITO_NO.Enabled = True

                    'cmbKana.Enabled = True
                    'cmbGakkouName.Enabled = True

                    ''���͋֎~�{�^������
                    'btnAdd.Enabled = True
                    'btnUPDATE.Enabled = False
                    'btnDelete.Enabled = False
                    'btnFind.Enabled = True
                    'btnEraser.Enabled = True
                    'btnEnd.Enabled = True
                    'btnTuuban.Enabled = True
                    'txtTUUBAN.Focus()
                    '2017/04/05 saitou ���t�M��(RSV2�W��) DEL ----------------------------------------------------------------- END
                    ' 2010/09/16 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<

                Else
                    'ROLLBACK
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    MessageBox.Show(String.Format(MSG0002E, "�o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�I��", "����", "")
            LW.ToriCode = "000000000000"
        End Try

    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click

        '�X�V�ύX����
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")

            '���͒l�`�F�b�N
            If fn_CheckEntry(1) = False Then
                Exit Sub
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ���k�}�X�^�����e���[���P ---------------------------------------- START
            '�L�[���ڊi�[
            Me.SeitomastMenteListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteListInfo.Tuuban = Me.txtTUUBAN.Text
            '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ----------------------------------------------------------------- END

            Dim bret As Boolean = False

            '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ���k�}�X�^�����e���[���P ---------------------------------------- START
            '�X�V�p�f�[�^�e�[�u���̏�����
            Me.CreateSeitomastDataTable()
            Dim beforeDataSetFlg As Boolean = False
            Dim afterDataSetFlg As Boolean = False

            '�X�V���X�g�p�̏��i�[
            Me.SeitomastMenteUpdateListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteUpdateListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteUpdateListInfo.Tuuban = Me.txtTUUBAN.Text
            Me.SeitomastMenteUpdateListInfo.GakunenCode = Me.txtGAKUNEN_CODE.Text
            Me.SeitomastMenteUpdateListInfo.ClassCode = Me.txtCLASS_CODE.Text
            Me.SeitomastMenteUpdateListInfo.SeitoNo = Me.txtSEITO_NO.Text
            Me.SeitomastMenteUpdateListInfo.GakkouName = Me.lab�w�Z��.Text.Trim

            '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
            'Dim beforeSEITOMAST(,) As String = Nothing
            'Dim afterSEITOMAST(,) As String = Nothing
            '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<
            '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ----------------------------------------------------------------- END

            Try
                '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ���k�}�X�^�����e���[���P ---------------------------------------- START
                beforeDataSetFlg = Me.fn_GetSEITOMAST_PrnCSVUpd(True)
                '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
                '' ���k�}�X�^���X�V�����O�Ɍ��݂̏���z��Ɋi�[���Ă���
                'beforeDataSetFlg = fn_GetSEITOMAST_PrnCSVUpd(beforeSEITOMAST)
                '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<
                '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ----------------------------------------------------------------- END

                '�g�����U�N�V�����J�n
                Call GFUNC_EXECUTESQL_TRANS("", 0)

                If fn_UPDATESEITOMAST(CustomDataGridView1, 4) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView2, 5) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView3, 6) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView4, 7) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView5, 8) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView6, 9) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView7, 10) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView8, 11) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView9, 12) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView10, 1) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView11, 2) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView12, 3) = False Then
                    Exit Try
                End If

                '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ���k�}�X�^�����e���[���P ---------------------------------------- START
                '���ɖ߂�
                '��ʃN���A 2007/10/31
                Call fn_FormInitialize()
                '���͍��ڐ���
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtGAKUNEN_CODE.Enabled = True
                txtCLASS_CODE.Enabled = True
                txtSEITO_NO.Enabled = True

                cmbKana.Enabled = True
                cmbGakkouName.Enabled = True

                '���͋֎~�{�^������
                btnAdd.Enabled = True
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
                btnFind.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = True
                txtTUUBAN.Focus()

                '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
                '' ��ʂ��N���A�����Ɖ�ʍ��ڂ��擾�ł��Ȃ����߁A�������ړ�
                ' ''��ʃN���A 2007/10/31
                ''Call fn_FormInitialize()
                ' ''���͍��ڐ���
                ''txtGAKKOU_CODE.Enabled = True
                ''txtNENDO.Enabled = True
                ''txtTUUBAN.Enabled = True
                ''txtGAKUNEN_CODE.Enabled = True
                ''txtCLASS_CODE.Enabled = True
                ''txtSEITO_NO.Enabled = True

                ''cmbKana.Enabled = True
                ''cmbGakkouName.Enabled = True

                ' ''���͋֎~�{�^������
                ''btnAdd.Enabled = True
                ''btnUPDATE.Enabled = False
                ''btnDelete.Enabled = False
                ''btnFind.Enabled = True
                ''btnEraser.Enabled = True
                ''btnEnd.Enabled = True
                ''btnTuuban.Enabled = True
                ''txtTUUBAN.Focus()
                '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<
                '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ----------------------------------------------------------------- END

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ���k�}�X�^�����e���[���P ---------------------------------------- START
                    If beforeDataSetFlg = True Then
                        afterDataSetFlg = Me.fn_GetSEITOMAST_PrnCSVUpd(False)
                        If afterDataSetFlg = True Then
                            '���k�}�X�^�`�F�b�N���X�g�i�X�V�j��CSV�f�[�^���쐬
                            If fn_CreateCSV_UPD() = True Then
                                Call fn_Print(1)
                            End If
                        End If
                    End If

                    '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
                    'If beforeDataSetFlg Then
                    '    afterDataSetFlg = fn_GetSEITOMAST_PrnCSVUpd(afterSEITOMAST)
                    '    If afterDataSetFlg Then
                    '        '���k�}�X�^�`�F�b�N���X�g�i�X�V�j��CSV�f�[�^���쐬
                    '        If fn_CreateCSV_UPD(beforeSEITOMAST, afterSEITOMAST) Then
                    '            Call fn_Print(1)
                    '        End If

                    '    End If
                    'End If

                    'Call fn_FormInitialize()
                    ''���͍��ڐ���
                    'txtGAKKOU_CODE.Enabled = True
                    'txtNENDO.Enabled = True
                    'txtTUUBAN.Enabled = True
                    'txtGAKUNEN_CODE.Enabled = True
                    'txtCLASS_CODE.Enabled = True
                    'txtSEITO_NO.Enabled = True

                    'cmbKana.Enabled = True
                    'cmbGakkouName.Enabled = True

                    ''���͋֎~�{�^������
                    'btnAdd.Enabled = True
                    'btnUPDATE.Enabled = False
                    'btnDelete.Enabled = False
                    'btnFind.Enabled = True
                    'btnEraser.Enabled = True
                    'btnEnd.Enabled = True
                    'btnTuuban.Enabled = True
                    '' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<
                    '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ----------------------------------------------------------------- END

                Else
                    'ROLLBACK
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        '�폜�{�^��
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�J�n", "����", "")

            '���͒l�`�F�b�N
            If fn_CheckEntry(1) = False Then
                Exit Sub
            End If

            '�m�F���b�Z�[�W��\��
            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ���k�}�X�^�����e���[���P ---------------------------------------- START
            '�L�[���ڊi�[
            Me.SeitomastMenteListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteListInfo.Tuuban = Me.txtTUUBAN.Text
            '2017/04/05 saitou ���t�M��(RSV2�W��) ADD ----------------------------------------------------------------- END

            Dim bret As Boolean = False

            ' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
            Dim blCsvFlg As Boolean = False         '����pCSV�쐬�t���O
            ' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<

            Try
                ' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� -------------------->
                ' SEITOMAST���폜�����O��CSV���쐬���Ă���
                blCsvFlg = fn_CreateCSV_ADDDEL("2")
                ' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή� --------------------<

                '�g�����U�N�V�����J�n
                Call GFUNC_EXECUTESQL_TRANS("", 0)

                Dim SQL As String = ""
                SQL = " DELETE  FROM SEITOMAST"
                SQL &= " WHERE"
                SQL &= " GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                SQL &= " AND"
                SQL &= " NENDO_O ='" & Trim(txtNENDO.Text) & "'"
                SQL &= " AND"
                SQL &= " TUUBAN_O = " & CInt(txtTUUBAN.Text)

                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                    Exit Try
                End If

                '��ʂ̏�����
                Call fn_FormInitialize()

                '���͍��ڐ���
                cmbKana.Enabled = True
                cmbGakkouName.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtSEITO_NO.Enabled = True
                txtGAKUNEN_CODE.Enabled = True
                txtCLASS_CODE.Enabled = True

                '���̓{�^������
                btnAdd.Enabled = True
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
                btnFind.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = True

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)��O�G���[", "���s", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή�-------------------->
                    If blCsvFlg Then
                        '����������s
                        Call fn_Print(2)
                    End If
                    ' 2010/09/17 TASK)saitou ���k�}�X�^�`�F�b�N���X�g�o�͑Ή�--------------------<
                Else
                    'ROLLBACK
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�I��", "����", "")
            LW.ToriCode = "00000000000"
        End Try
    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Dim intSpdRow As Integer
        Dim dblKingaku_Goukei As Double
        Dim strHimoku As String = ""

        '�Q�ƃ{�^��
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")

            Dim OraReader As CASTCommon.MyOracleReader = Nothing
            '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- START
            Dim OraReader_Cnt As CASTCommon.MyOracleReader = Nothing
            '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- END

            Try
                Dim SQL As New StringBuilder(128)
                '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- START
                Dim SQL_CNT As New StringBuilder(128)
                '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- END

                '�֐����œ��̓`�F�b�N���AOK�Ȃ猟����SQL��Ԃ�
                If fn_GetSELECTSEITOMASTSQL(SQL) = False Then
                    Exit Sub
                End If

                OraReader = New CASTCommon.MyOracleReader(MainDB)

                '���k�}�X�^���݃`�F�b�N
                If Not OraReader.DataReader(SQL) Then
                    MessageBox.Show(G_MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                Else
                    '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- START
                    '�Ɖ�����擾���A�Q���ȏ�̏ꍇ�͍Č����𑣂����b�Z�[�W��\������
                    SQL_CNT = SQL.Replace("*", "COUNT(*) CNT")
                    SQL_CNT.Append(" AND TUKI_NO_O='04'")
                    OraReader_Cnt = New CASTCommon.MyOracleReader(MainDB)
                    If Not OraReader_Cnt.DataReader(SQL_CNT) Then
                        MessageBox.Show(G_MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKKOU_CODE.Focus()
                        Exit Sub
                    Else
                        '�Ɖ���擾
                        Dim intCount As Integer = OraReader_Cnt.GetInt("CNT")
                        OraReader_Cnt.Close()

                        If intCount > 1 Then
                            '���b�Z�[�W����
                            Dim MSG_INFO As String = ""
                            While OraReader.EOF = False
                                If OraReader.GetString("TUKI_NO_O") = "04" Then
                                    MSG_INFO &= "���w�N�x=" & OraReader.GetItem("NENDO_O")
                                    MSG_INFO &= "�@�ʔ�=" & OraReader.GetItem("TUUBAN_O").ToString.Trim.PadLeft(4, "0"c)
                                    MSG_INFO &= vbCrLf
                                End If
                                OraReader.NextRead()
                            End While
                            OraReader.Close()

                            MessageBox.Show(String.Format(G_MSG0108W, "���k", MSG_INFO), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtNENDO.Focus()
                            Exit Sub
                        End If
                    End If
                    '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- END

                    '��ڏ��S�����痂�R���̃N���A
                    cmbHIMOKU.Text = ""
                    cmbHIMOKU.Items.Clear()
                    Call Sub_Himoku_Initialize()
                End If

                While OraReader.EOF = False
                    With OraReader
                        '���w�N�x
                        txtNENDO.Text = Trim(.GetItem("NENDO_O"))
                        '�ʔ�
                        txtTUUBAN.Text = .GetItem("TUUBAN_O").ToString.Trim.PadLeft(4, "0"c)
                        '�w�N�R�[�h
                        txtGAKUNEN_CODE.Text = Trim(.GetItem("GAKUNEN_CODE_O"))
                        '�N���X�R�[�h
                        txtCLASS_CODE.Text = Trim(.GetItem("CLASS_CODE_O"))
                        '���k�ԍ�
                        txtSEITO_NO.Text = Trim(.GetItem("SEITO_NO_O"))
                        '����
                        cmbSEIBETU.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEIBETU_TXT, Trim(.GetItem("SEIBETU_O")))
                        '���k��(�J�i)
                        txtSEITO_KNAME.Text = Trim(.GetItem("SEITO_KNAME_O"))
                        '���k��(����)
                        txtSEITO_NNAME.Text = ConvDBNullToString(.GetItem("SEITO_NNAME_O"))
                        '�U�ցH
                        cmbFURIKAE.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKAE_TXT, Trim(.GetItem("FURIKAE_O")))
                        '�戵���Z�@�փR�[�h
                        txtTKIN_NO.Text = ConvDBNullToString(.GetItem("TKIN_NO_O"))
                        '�戵�x�X�R�[�h
                        txtTSIT_NO.Text = ConvDBNullToString(.GetItem("TSIT_NO_O"))
                        '�Ȗ�
                        cmbKAMOKU.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, Trim(.GetItem("KAMOKU_O")))
                        '�����ԍ�
                        txtKOUZA.Text = ConvDBNullToString(.GetItem("KOUZA_O"))
                        '�������`�l�J�i
                        txtMEIGI_KNAME.Text = Trim(.GetItem("MEIGI_KNAME_O"))
                        '�ی�Ҋ���
                        txtMEIGI_NNAME.Text = ConvDBNullToString(.GetItem("MEIGI_NNAME_O"))
                        '�Z������
                        txtKEIYAKU_NJYU.Text = ConvDBNullToString(.GetItem("KEIYAKU_NJYU_O"))
                        '�i���敪
                        cmbSINKYU_KBN.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SINKYU_TXT, Trim(.GetItem("SINKYU_KBN_O")))
                        '�d�b�ԍ�
                        txtKEIYAKU_DENWA.Text = ConvDBNullToString(.GetItem("KEIYAKU_DENWA_O"))
                        '���t���O
                        cmbKAIYAKU_FLG.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_KAIYAKU_TXT, Trim(.GetItem("KAIYAKU_FLG_O")))
                        '��ڏ��R���{�̐ݒ�
                        strHimoku = .GetItem("HIMOKU_ID_O")

                        '���q���̐ݒ�
                        '2017/11/28 �^�X�N�j���� CHG (�W���ŏC��(��177)) -------------------- START
                        cmbTYOUSI.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_TYOUSIUMU_TXT, Trim(.GetItem("TYOUSI_FLG_O")))
                        Select Case cmbTYOUSI.SelectedIndex
                            Case 0
                                btnTyousi.Enabled = False
                                str���q�w�Z�R�[�h = ""
                                str���q���w�N�x = ""
                                str���q�ʔ� = ""
                                str���q�w�N = ""
                                str���q�N���X = ""
                                str���q���k�ԍ� = ""
                            Case 1
                                btnTyousi.Enabled = True

                                str���q�w�Z�R�[�h = .GetItem("GAKKOU_CODE_O")
                                str���q���w�N�x = .GetItem("TYOUSI_NENDO_O")
                                str���q�ʔ� = .GetItem("TYOUSI_TUUBAN_O")
                                str���q�w�N = .GetItem("TYOUSI_GAKUNEN_O")
                                str���q�N���X = .GetItem("TYOUSI_CLASS_O")
                                str���q���k�ԍ� = .GetItem("TYOUSI_SEITONO_O")
                        End Select
                        'Select Case Trim(.GetItem("TYOUSI_FLG_O"))
                        '    Case 0
                        '        cmbTYOUSI.SelectedIndex = 0
                        '        btnTyousi.Enabled = False
                        '        str���q�w�Z�R�[�h = ""
                        '        str���q���w�N�x = ""
                        '        str���q�ʔ� = ""
                        '        str���q�w�N = ""
                        '        str���q�N���X = ""
                        '        str���q���k�ԍ� = ""
                        '    Case 1
                        '        cmbTYOUSI.SelectedIndex = 1
                        '        btnTyousi.Enabled = True

                        '        str���q�w�Z�R�[�h = .GetItem("GAKKOU_CODE_O")
                        '        str���q���w�N�x = .GetItem("TYOUSI_NENDO_O")
                        '        str���q�ʔ� = .GetItem("TYOUSI_TUUBAN_O")
                        '        str���q�w�N = .GetItem("TYOUSI_GAKUNEN_O")
                        '        str���q�N���X = .GetItem("TYOUSI_CLASS_O")
                        '        str���q���k�ԍ� = .GetItem("TYOUSI_SEITONO_O")
                        'End Select
                        '2017/11/28 �^�X�N�j���� CHG (�W���ŏC��(��177)) -------------------- END

                        '2017/04/28 saitou RSV2 ADD �W���@�\�ǉ�(�X�V��) ---------------------------------------- START
                        Dim KousinDate As String = ConvDBNullToString(.GetItem("KOUSIN_DATE_O")).PadLeft(8, "0"c)
                        Me.lblKousinDate.Text = KousinDate.Substring(0, 4) & "�N" & KousinDate.Substring(4, 2) & "��" & KousinDate.Substring(6, 2) & "��"
                        '2017/04/28 saitou RSV2 ADD ------------------------------------------------------------- END
                    End With


                    '��ڏ����z�̐ݒ�
                    Dim dgv As DataGridView = Nothing
                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_O")), "00")
                        Case "04"
                            dgv = CustomDataGridView1
                        Case "05"
                            dgv = CustomDataGridView2
                        Case "06"
                            dgv = CustomDataGridView3
                        Case "07"
                            dgv = CustomDataGridView4
                        Case "08"
                            dgv = CustomDataGridView5
                        Case "09"
                            dgv = CustomDataGridView6
                        Case "10"
                            dgv = CustomDataGridView7
                        Case "11"
                            dgv = CustomDataGridView8
                        Case "12"
                            dgv = CustomDataGridView9
                        Case "01"
                            dgv = CustomDataGridView10
                        Case "02"
                            dgv = CustomDataGridView11
                        Case "03"
                            dgv = CustomDataGridView12
                    End Select

                    dblKingaku_Goukei = 0

                    With dgv
                        '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                        For intSpdRow = 0 To intMaxHimokuCnt - 1
                            'For intSpdRow = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                            '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                            If IsDBNull(OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_O")) = True Then
                                .Rows(intSpdRow).Cells(1).ReadOnly = True
                                .Rows(intSpdRow).Cells(2).ReadOnly = True
                            Else
                                '��ږ�
                                .Rows(intSpdRow).Cells(0).Value = OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_O")
                                .Rows(intSpdRow).Cells(1).ReadOnly = False

                                '�������@���ʂ̏ꍇ�͋��z�̃Z���̓��͂��ɂ���
                                Select Case OraReader.GetItem("SEIKYU" & Format(intSpdRow + 1, "00") & "_O")
                                    Case "0"
                                        .Rows(intSpdRow).Cells(1).Value = "�ꗥ"
                                        .Rows(intSpdRow).Cells(2).ReadOnly = True
                                    Case "1"
                                        .Rows(intSpdRow).Cells(1).Value = "��"
                                        .Rows(intSpdRow).Cells(2).ReadOnly = False
                                End Select

                                '���z
                                Str_Spread((CInt(OraReader.GetItem("TUKI_NO_O")) - 1), intSpdRow) = OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_O")
                                .Rows(intSpdRow).Cells(2).Value = Format(CDec(OraReader.GetItem("KINGAKU" & Format(intSpdRow + 1, "00") & "_O")), "#,##0")

                                dblKingaku_Goukei += CDbl(OraReader.GetItem("KINGAKU" & Format(intSpdRow + 1, "00") & "_O"))
                            End If
                        Next intSpdRow
                    End With

                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_O")), "00")
                        Case "04"
                            lblKingaku_4.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "05"
                            lblKingaku_5.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "06"
                            lblKingaku_6.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "07"
                            lblKingaku_7.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "08"
                            lblKingaku_8.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "09"
                            lblKingaku_9.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "10"
                            lblKingaku_10.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "11"
                            lblKingaku_11.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "12"
                            lblKingaku_12.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "01"
                            lblKingaku_1.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "02"
                            lblKingaku_2.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "03"
                            lblKingaku_3.Text = Format(dblKingaku_Goukei, "#,##0")
                    End Select

                    OraReader.NextRead()
                End While

                OraReader.Close()
                OraReader = Nothing

                '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    '��ږ���ݒ肷��
                    CustomDataGridView13.Rows(intRow).Cells(0).Value = CustomDataGridView1.Rows(intRow).Cells(0).Value
                Next
                '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- END

                '���͋֎~���ڐ���
                cmbKana.Enabled = False
                cmbGakkouName.Enabled = False
                txtGAKKOU_CODE.Enabled = False
                txtNENDO.Enabled = False
                txtTUUBAN.Enabled = False

                '���͋֎~�{�^������
                btnAdd.Enabled = False
                btnUPDATE.Enabled = True
                btnDelete.Enabled = True
                btnFind.Enabled = False
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = False '2006/10/19�@�ʔԎ擾�{�^��

                '�w�N���̎擾
                Call fn_GetGakunenName()
                '�N���X���̎擾
                Call fn_GetClassName()
                '���Z�@�֖��A�x�X���̐ݒ�
                Call fn_GetKinNameSitName()
                '��ڏ��R���{�̐ݒ�
                Call Sub_GetHimokuID()
                Call fn_SetHimokuID(strHimoku)

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)��O�G���[", "���s", ex.Message)
            Finally
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                    OraReader = Nothing
                End If
                '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- START
                If Not OraReader_Cnt Is Nothing Then
                    OraReader_Cnt.Close()
                    OraReader_Cnt = Nothing
                End If
                '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- END
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
            LW.ToriCode = "0000000000000"
        End Try
    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '����{�^��

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            Call fn_FormInitialize()

            str���q�w�Z�R�[�h = ""
            str���q���w�N�x = ""
            str���q�ʔ� = ""
            str���q�w�N = ""
            str���q�N���X = ""
            str���q���k�ԍ� = ""

            '���͍��ڐ���
            txtGAKKOU_CODE.Enabled = True
            txtNENDO.Enabled = True
            txtTUUBAN.Enabled = True
            txtGAKUNEN_CODE.Enabled = True
            txtCLASS_CODE.Enabled = True
            txtSEITO_NO.Enabled = True

            cmbKana.Enabled = True
            cmbGakkouName.Enabled = True

            '���͋֎~�{�^������
            btnAdd.Enabled = True
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False
            btnFind.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True
            btnTuuban.Enabled = True
            cmbKana.SelectedIndex = -1

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)��O�G���[", "���s", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
    Private Sub btnTuuban_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTuuban.Click

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtNENDO.Text) <> "" Then
                Dim SQL As New StringBuilder(128)

                SQL.Append(" SELECT * FROM SEITOMAST ")
                SQL.Append(" WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                SQL.Append(" AND NENDO_O =" & CInt(Trim(txtNENDO.Text)))
                SQL.Append(" ORDER BY TUUBAN_O DESC ")

                Orareader = New CASTCommon.MyOracleReader(MainDB)

                If Not Orareader.DataReader(SQL) Then
                    txtTUUBAN.Text = "0001"
                Else
                    '2019/11/07 saitou UPD �W���ŏC���i�ʔԎ擾���̍ő�l�`�F�b�N�j --------------------  START
                    If Orareader.GetInt("TUUBAN_O") + 1 > 9999 Then
                        MessageBox.Show(G_MSG0109W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Else
                        txtTUUBAN.Text = CStr(Orareader.GetInt("TUUBAN_O") + 1).Trim.PadLeft(4, "0"c)
                    End If
                    'txtTUUBAN.Text = CStr(Orareader.GetItem("TUUBAN_O") + 1).Trim.PadLeft(4, "0"c)
                    '2019/11/07 saitou UPD �W���ŏC���i�ʔԎ擾���̍ő�l�`�F�b�N�j --------------------  END
                End If

                Orareader.Close()
                Orareader = Nothing

                txtTUUBAN.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ʔԎ擾)��O�G���[", "���s", ex.Message)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
        End Try

    End Sub
    Private Sub btnHimoku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHimoku.Click

        Dim intSpdRow As Integer

        Dim dblKingaku_Goukei As Double
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(��ڎ擾)�J�n", "����", "")
            '��ڏ������ޯ�����I���`�F�b�N
            If cmbHIMOKU.SelectedIndex = -1 Then
                MessageBox.Show(G_MSG0066W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '�w�Z�R�[�h���̓`�F�b�N
            If txtGAKKOU_CODE.Text = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If
            '�w�N���̓`�F�b�N
            If txtGAKUNEN_CODE.Text = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�N�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKUNEN_CODE.Focus()
                Exit Sub
            End If
            '�m�F���b�Z�[�W
            If MessageBox.Show(G_MSG0019I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Exit Sub
            End If

            Dim OraReader As CASTCommon.MyOracleReader = Nothing


            '���z�ύX���̍č��v�v�Z
            '��ڃ��R�[�h�̎擾
            Dim SQL As New StringBuilder(128)
            SQL.Append(" SELECT * FROM HIMOMAST")
            SQL.Append(" WHERE GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")
            SQL.Append(" AND GAKUNEN_CODE_H ='" & Trim(txtGAKUNEN_CODE.Text) & "'")
            SQL.Append(" AND HIMOKU_ID_H ='" & fn_SplitHimokuID() & "'")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            '��ڃ��R�[�h���݃`�F�b�N
            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0067W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            Else
                '��ڏ��S�����痂�R���̃N���A
                Call Sub_Himoku_Initialize()

                '��ڏ��ւ̒l�̐ݒ�
                Dim dgv As DataGridView = Nothing
                While OraReader.EOF = False
                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_H")), "00")
                        Case "04"
                            dgv = CustomDataGridView1
                        Case "05"
                            dgv = CustomDataGridView2
                        Case "06"
                            dgv = CustomDataGridView3
                        Case "07"
                            dgv = CustomDataGridView4
                        Case "08"
                            dgv = CustomDataGridView5
                        Case "09"
                            dgv = CustomDataGridView6
                        Case "10"
                            dgv = CustomDataGridView7
                        Case "11"
                            dgv = CustomDataGridView8
                        Case "12"
                            dgv = CustomDataGridView9
                        Case "01"
                            dgv = CustomDataGridView10
                        Case "02"
                            dgv = CustomDataGridView11
                        Case "03"
                            dgv = CustomDataGridView12
                    End Select
                    dblKingaku_Goukei = 0
                    With dgv
                        '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                        For intSpdRow = 0 To intMaxHimokuCnt - 1
                            'For intSpdRow = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                            '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                            If IsDBNull(OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_H")) = True Then
                                .Rows(intSpdRow).Cells(1).ReadOnly = True
                                .Rows(intSpdRow).Cells(2).ReadOnly = True
                            Else
                                .Rows(intSpdRow).Cells(1).ReadOnly = False
                                .Rows(intSpdRow).Cells(2).ReadOnly = True

                                .Rows(intSpdRow).Cells(0).Value = OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_H")
                                '�R���{�{�b�N�X���h�ꗥ�h�ɂ���
                                .Rows(intSpdRow).Cells(1).Value = "�ꗥ"
                                Str_Spread((CInt(OraReader.GetItem("TUKI_NO_H")) - 1), intSpdRow) = OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_H")
                                .Rows(intSpdRow).Cells(2).Value = Format(CDec(OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_H")), "#,##0")

                                dblKingaku_Goukei += CDbl(OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_H"))
                            End If
                        Next intSpdRow
                    End With

                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_H")), "00")
                        Case "04"
                            lblKingaku_4.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "05"
                            lblKingaku_5.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "06"
                            lblKingaku_6.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "07"
                            lblKingaku_7.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "08"
                            lblKingaku_8.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "09"
                            lblKingaku_9.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "10"
                            lblKingaku_10.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "11"
                            lblKingaku_11.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "12"
                            lblKingaku_12.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "01"
                            lblKingaku_1.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "02"
                            lblKingaku_2.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "03"
                            lblKingaku_3.Text = Format(dblKingaku_Goukei, "#,##0")
                    End Select

                    OraReader.NextRead()
                End While

                OraReader.Close()
                OraReader = Nothing

            End If

            '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- START
            For intRow As Integer = 0 To intMaxHimokuCnt - 1
                '��ږ���ݒ肷��
                CustomDataGridView13.Rows(intRow).Cells(0).Value = CustomDataGridView1.Rows(intRow).Cells(0).Value
            Next
            '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(��ڎ擾)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(��ڎ擾)�I��", "����", "")
            LW.ToriCode = "000000000000"
        End Try

    End Sub
    Private Sub btnTyousi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTyousi.Click
        '���q�擾�{�^��
        Try
            Dim KFGMAST041 As New GAKKOU.KFGMAST041

            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                str���q�w�Z�R�[�h = txtGAKKOU_CODE.Text

                '���q�ݒ��ʕ\��
                KFGMAST041.ShowDialog(Me)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���q�擾)��O�G���[", "���s", ex.Message)
        End Try

    End Sub
    Private Sub btnKOUZA_CHK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKOUZA_CHK.Click
        Try
            Call fn_CheckEntryKouza()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����`�F�b�N)��O�G���[", "���s", ex.Message)
        End Try
    End Sub
#End Region

    Private Sub Sub_GetHimokuID()

        '�w�Z�R�[�h�Ɗw�N�����ڃ��R�[�h�i��ڂh�c�^��ڃp�^�[�����j���擾
        Dim str���ID As String = ""

        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" Then

            Dim OraReader As CASTCommon.MyOracleReader = Nothing

            Try
                Dim SQL As New StringBuilder(128)

                SQL.Append(" SELECT * FROM HIMOMAST")
                SQL.Append(" WHERE GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) & "'")
                SQL.Append(" AND GAKUNEN_CODE_H = " & CByte(txtGAKUNEN_CODE.Text))
                SQL.Append(" AND HIMOKU_ID_H <> '000'")
                SQL.Append(" ORDER BY HIMOKU_ID_H ASC")

                OraReader = New CASTCommon.MyOracleReader(MainDB)

                If Not OraReader.DataReader(SQL) Then
                    cmbHIMOKU.Text = ""
                    cmbHIMOKU.Items.Clear()
                    str���ID = ""
                Else
                    While OraReader.EOF = False
                        If str���ID <> OraReader.GetItem("HIMOKU_ID_H") Then
                            cmbHIMOKU.Items.Add(Trim(OraReader.GetItem("HIMOKU_ID_H") & _
                                                "�E" & _
                                                OraReader.GetItem("HIMOKU_ID_NAME_H")))
                            '��r�̂��߂̂h�c��ޔ�����
                            str���ID = OraReader.GetItem("HIMOKU_ID_H")
                        End If
                        OraReader.NextRead()
                    End While

                    OraReader.Close()
                    OraReader = Nothing

                    '�擪�̔�ڂh�c��\��
                    If cmbHIMOKU.Items.Count <> 0 Then
                        cmbHIMOKU.SelectedIndex = 0
                    End If
                End If
            Catch ex As Exception
            Finally
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                    OraReader = Nothing
                End If
            End Try
        End If
    End Sub
    Private Sub Sub_Himoku_Initialize()

        '�X�v���b�g�̃N���A
        Call Sub_ClearHimoku(CustomDataGridView1)
        Call Sub_ClearHimoku(CustomDataGridView2)
        Call Sub_ClearHimoku(CustomDataGridView3)
        Call Sub_ClearHimoku(CustomDataGridView4)
        Call Sub_ClearHimoku(CustomDataGridView5)
        Call Sub_ClearHimoku(CustomDataGridView6)
        Call Sub_ClearHimoku(CustomDataGridView7)
        Call Sub_ClearHimoku(CustomDataGridView8)
        Call Sub_ClearHimoku(CustomDataGridView9)
        Call Sub_ClearHimoku(CustomDataGridView10)
        Call Sub_ClearHimoku(CustomDataGridView11)
        Call Sub_ClearHimoku(CustomDataGridView12)

        '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- START
        For intRow As Integer = 0 To intMaxHimokuCnt - 1
            '�ύX�^�u���DataGridView�̔�ږ��N���A
            CustomDataGridView13.Rows(intRow).Cells(0).Value = ""
        Next
        '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- END

        '���v���z�̃N���A
        lblKingaku_1.Text = ""
        lblKingaku_2.Text = ""
        lblKingaku_3.Text = ""
        lblKingaku_4.Text = ""
        lblKingaku_5.Text = ""
        lblKingaku_6.Text = ""
        lblKingaku_7.Text = ""
        lblKingaku_8.Text = ""
        lblKingaku_9.Text = ""
        lblKingaku_10.Text = ""
        lblKingaku_11.Text = ""
        lblKingaku_12.Text = ""

    End Sub
    Private Sub Sub_ClearHimoku(ByVal dgv As DataGridView)

        Try
            With dgv
                '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '�@��ڂ͂P�O�܂�
                    '2017/02/23 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    For intCol As Integer = 0 To 2
                        If intCol <> 0 Then
                            .Rows(intRow).Cells(intCol).ReadOnly = False
                        End If

                        Select Case intCol
                            Case 0
                                '��ږ�
                                .Rows(intRow).Cells(intCol).Value = ""
                            Case 1
                                '�������@
                                .Rows(intRow).Cells(intCol).Value = ""
                            Case 2
                                '���z
                                .Rows(intRow).Cells(intCol).Value = ""
                        End Select
                    Next intCol
                Next intRow
            End With
        Catch ex As Exception
            '2014/01/06 saitou �W���� ���b�Z�[�W�萔�� UPD -------------------------------------------------->>>>
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'MessageBox.Show("��ڈꗗ�̏������Ɏ��s���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '2014/01/06 saitou �W���� ���b�Z�[�W�萔�� UPD --------------------------------------------------<<<<
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O�G���[", "���s", ex.Message)
        End Try
    End Sub
    Private Function fn_INSERTSEITOMAST(ByVal dgv As DataGridView, ByVal pTuki As Integer) As Boolean

        Dim ret As Boolean = False

        Try
            Dim SQL As New StringBuilder(128)

            SQL.Append("INSERT INTO SEITOMAST ")
            SQL.Append(" (GAKKOU_CODE_O ")
            SQL.Append(", NENDO_O ")
            SQL.Append(", TUUBAN_O ")
            SQL.Append(", GAKUNEN_CODE_O ")
            SQL.Append(", CLASS_CODE_O ")
            SQL.Append(", SEITO_NO_O ")
            SQL.Append(", SEITO_KNAME_O ")
            SQL.Append(", SEITO_NNAME_O ")
            SQL.Append(", SEIBETU_O ")
            SQL.Append(", TKIN_NO_O ")
            SQL.Append(", TSIT_NO_O ")
            SQL.Append(", KAMOKU_O ")
            SQL.Append(", KOUZA_O ")
            SQL.Append(", MEIGI_KNAME_O ")
            SQL.Append(", MEIGI_NNAME_O ")
            SQL.Append(", FURIKAE_O ")
            SQL.Append(", KEIYAKU_NJYU_O ")
            SQL.Append(", KEIYAKU_DENWA_O ")
            SQL.Append(", KAIYAKU_FLG_O ")
            SQL.Append(", SINKYU_KBN_O ")
            SQL.Append(", HIMOKU_ID_O ")
            SQL.Append(", TYOUSI_FLG_O ")
            SQL.Append(", TYOUSI_NENDO_O ")
            SQL.Append(", TYOUSI_TUUBAN_O ")
            SQL.Append(", TYOUSI_GAKUNEN_O ")
            SQL.Append(", TYOUSI_CLASS_O ")
            SQL.Append(", TYOUSI_SEITONO_O ")
            SQL.Append(", TUKI_NO_O ")
            SQL.Append(", SEIKYU01_O ")
            SQL.Append(", KINGAKU01_O ")
            SQL.Append(", SEIKYU02_O ")
            SQL.Append(", KINGAKU02_O ")
            SQL.Append(", SEIKYU03_O ")
            SQL.Append(", KINGAKU03_O ")
            SQL.Append(", SEIKYU04_O ")
            SQL.Append(", KINGAKU04_O ")
            SQL.Append(", SEIKYU05_O ")
            SQL.Append(", KINGAKU05_O ")
            SQL.Append(", SEIKYU06_O ")
            SQL.Append(", KINGAKU06_O ")
            SQL.Append(", SEIKYU07_O ")
            SQL.Append(", KINGAKU07_O ")
            SQL.Append(", SEIKYU08_O ")
            SQL.Append(", KINGAKU08_O ")
            SQL.Append(", SEIKYU09_O ")
            SQL.Append(", KINGAKU09_O ")
            SQL.Append(", SEIKYU10_O ")
            SQL.Append(", KINGAKU10_O ")
            SQL.Append(", SEIKYU11_O ")
            SQL.Append(", KINGAKU11_O ")
            SQL.Append(", SEIKYU12_O ")
            SQL.Append(", KINGAKU12_O ")
            SQL.Append(", SEIKYU13_O ")
            SQL.Append(", KINGAKU13_O ")
            SQL.Append(", SEIKYU14_O ")
            SQL.Append(", KINGAKU14_O ")
            SQL.Append(", SEIKYU15_O ")
            SQL.Append(", KINGAKU15_O ")
            SQL.Append(", SAKUSEI_DATE_O ")
            SQL.Append(", KOUSIN_DATE_O ) ")
            SQL.Append(" VALUES ( ")

            '�w�Z�R�[�h
            SQL.Append("'" & txtGAKKOU_CODE.Text & "'")
            '���w�N�x
            SQL.Append("," & "'" & txtNENDO.Text & "'")
            '�ʔ�
            SQL.Append("," & CInt(txtTUUBAN.Text))
            '�w�N
            SQL.Append("," & CByte(txtGAKUNEN_CODE.Text))
            '�N���X
            SQL.Append("," & CByte(txtCLASS_CODE.Text))
            '���k�ԍ�
            SQL.Append("," & "'" & txtSEITO_NO.Text & "'")
            '���k�����i�J�i�j
            SQL.Append("," & "'" & Trim(txtSEITO_KNAME.Text) & "'")
            '���k�����i�����j
            If Trim(txtSEITO_NNAME.Text) = "" Then
                SQL.Append("," & "'" & Space(50) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtSEITO_NNAME.Text) & "'")
            End If
            '����
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEIBETU_TXT, cmbSEIBETU) & "'")

            '���Z�@�փR�[�h
            If Trim(txtTKIN_NO.Text) = "" Then
                SQL.Append("," & "'    '")
            Else
                SQL.Append("," & "'" & Trim(txtTKIN_NO.Text) & "'")
            End If
            '�x�X�R�[�h
            If Trim(txtTSIT_NO.Text) = "" Then
                SQL.Append("," & "'   '")
            Else
                SQL.Append("," & "'" & Trim(txtTSIT_NO.Text) & "'")
            End If
            '�Ȗ�
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU) & "'")

            '�����ԍ�
            If Trim(txtKOUZA.Text) = "" Then
                SQL.Append("," & "'       '")
            Else
                SQL.Append("," & "'" & Trim(txtKOUZA.Text) & "'")
            End If
            '���`�l�i�J�i�j
            SQL.Append("," & "'" & Trim(txtMEIGI_KNAME.Text) & "'")
            '���`�l�i�����j
            If Trim(txtMEIGI_NNAME.Text) = "" Then
                SQL.Append("," & "'" & Space(50) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtMEIGI_NNAME.Text) & "'")
            End If
            '�U�֕��@
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKAE_TXT, cmbFURIKAE) & "'")

            '�_��Z���i�����j
            If Trim(txtKEIYAKU_NJYU.Text) = "" Then
                SQL.Append("," & "'" & Space(50) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtKEIYAKU_NJYU.Text) & "'")
            End If
            '�d�b�ԍ�
            If Trim(txtKEIYAKU_DENWA.Text) = "" Then
                SQL.Append("," & "'" & Space(13) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtKEIYAKU_DENWA.Text) & "'")
            End If
            '���敪
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAIYAKU_TXT, cmbKAIYAKU_FLG) & "'")

            '�i���敪
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SINKYU_TXT, cmbSINKYU_KBN) & "'")

            '��ڂh�c
            SQL.Append("," & "'" & fn_SplitHimokuID() & "'")
            '���q�L���t���O
            '2017/11/28 �^�X�N�j���� CHG (�W���ŏC��(��177)) -------------------- START
            SQL.Append("," & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TYOUSIUMU_TXT, cmbTYOUSI))
            'SQL.Append("," & cmbTYOUSI().SelectedIndex)
            '2017/11/28 �^�X�N�j���� CHG (�W���ŏC��(��177)) -------------------- END
            '���q���w�N�x
            If Trim(str���q���w�N�x) = "" Then
                SQL.Append("," & "'" & Space(4) & "'")
            Else
                SQL.Append("," & "'" & Trim(str���q���w�N�x) & "'")
            End If
            '���q�ʔ�
            If Trim(str���q�ʔ�) = "" Then
                SQL.Append("," & 0)
            Else
                SQL.Append("," & "'" & CDbl(str���q�ʔ�) & "'")
            End If
            '���q�w�N
            If Trim(str���q�w�N) = "" Then
                SQL.Append("," & 0)
            Else
                SQL.Append("," & "'" & CByte(str���q�w�N) & "'")
            End If
            '���q�N���X
            If Trim(str���q�N���X) = "" Then
                SQL.Append("," & 0)
            Else
                SQL.Append("," & "'" & CByte(str���q�N���X) & "'")
            End If
            '���q���k�ԍ�
            If Trim(str���q���k�ԍ�) = "" Then
                SQL.Append("," & "'" & Space(7) & "'")
            Else
                SQL.Append("," & "'" & Trim(str���q���k�ԍ�) & "'")
            End If
            '������
            SQL.Append("," & "'" & Format(pTuki, "00") & "'")

            '��ڂP�`��ڂP�T�̐������@,���z
            For intRowCnt As Integer = 0 To 14 '2007/04/04�@��ڂ͂P�O�܂ŉ�ʓ��́A�P�P����͂O�Œ�
                '2017/04/06 �^�X�N�j���� DEL �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                If intRowCnt <= (intMaxHimokuCnt - 1) Then
                    'If intRowCnt <= 9 Then
                    '2017/04/06 �^�X�N�j���� DEL �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    '��ڂP�`�P�O
                    With dgv
                        '��ږ��̗L��
                        Select Case Trim(.Rows(intRowCnt).Cells(0).Value)
                            Case ""
                                '�������@
                                SQL.Append(",'0'")
                                '���z
                                SQL.Append(",0")
                            Case Else
                                '�������@
                                '�������@���ꗥ(=0)�̏ꍇ�A���z��0�Œ��ݒ肷��
                                Select Case .Rows(intRowCnt).Cells(1).Value
                                    Case "�ꗥ"
                                        '�ꗥ
                                        SQL.Append(",0")
                                        SQL.Append(",0")
                                    Case "��"
                                        '��
                                        SQL.Append(",1")
                                        SQL.Append("," & CDec(.Rows(intRowCnt).Cells(2).Value).ToString)
                                End Select
                        End Select
                    End With
                    '2017/02/23 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                Else
                    '�������@
                    SQL.Append(",'0'")
                    '���z
                    SQL.Append(",0")
                End If
                'Else
                '       '��ڂP�P�`�P�T
                '       '�������@
                '       SQL.Append(",'0'")
                '       '���z
                '       SQL.Append(",0")
                '       End If
                '2017/02/23 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
            Next

            SQL.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            SQL.Append(",'00000000') ")

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)
            ret = False
        End Try

        Return ret


    End Function
    Private Function fn_UPDATESEITOMAST(ByVal dgv As DataGridView, ByVal pTuki As Integer) As Boolean

        Dim ret As Boolean = False

        Try
            Dim SQL As New StringBuilder(128)

            SQL.Append("UPDATE  SEITOMAST SET ")
            '�w�N
            SQL.Append(" GAKUNEN_CODE_O = " & CByte(txtGAKUNEN_CODE.Text))
            '�ǉ��F�N���X
            SQL.Append(" ,CLASS_CODE_O = " & CByte(txtCLASS_CODE.Text))
            '���k�ԍ�
            SQL.Append(" ,SEITO_NO_O = '" & Trim(txtSEITO_NO.Text) & "'")
            '���k���i�J�i�j
            SQL.Append(" ,SEITO_KNAME_O   = '" & Trim(txtSEITO_KNAME.Text) & "'")
            '���k���i�����j
            If Trim(txtSEITO_NNAME.Text) = "" Then
                SQL.Append(",SEITO_NNAME_O   = " & "'" & Space(50) & "'")
            Else
                SQL.Append(",SEITO_NNAME_O   = " & "'" & Trim(txtSEITO_NNAME.Text) & "'")
            End If

            '����
            SQL.Append(",SEIBETU_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEIBETU_TXT, cmbSEIBETU) & "'")

            '���Z�@�փR�[�h
            If Trim(txtTKIN_NO.Text) = "" Then
                SQL.Append(",TKIN_NO_O       = " & "'" & Space(4) & "'")
            Else
                SQL.Append(",TKIN_NO_O       = " & "'" & Trim(txtTKIN_NO.Text) & "'")
            End If
            '�x�X�R�[�h
            If Trim(txtTSIT_NO.Text) = "" Then
                SQL.Append(",TSIT_NO_O       = " & "'" & Space(3) & "'")
            Else
                SQL.Append(",TSIT_NO_O       = " & "'" & Trim(txtTSIT_NO.Text) & "'")
            End If
            '�Ȗ�
            SQL.Append(",KAMOKU_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU) & "'")

            '�����ԍ�
            If Trim(txtKOUZA.Text) = "" Then
                SQL.Append(",KOUZA_O         = " & "'" & Space(7) & "'")
            Else
                SQL.Append(",KOUZA_O         = " & "'" & Trim(txtKOUZA.Text) & "'")
            End If
            '���`�l�i�J�i�j
            SQL.Append(",MEIGI_KNAME_O   = " & "'" & Trim(txtMEIGI_KNAME.Text) & "'")
            '���`�l�i�����j
            If Trim(txtMEIGI_NNAME.Text) = "" Then
                SQL.Append(",MEIGI_NNAME_O   = " & "'" & Space(50) & "'")
            Else
                SQL.Append(",MEIGI_NNAME_O   = " & "'" & Trim(txtMEIGI_NNAME.Text) & "'")
            End If
            '�U�֋敪
            SQL.Append(",FURIKAE_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKAE_TXT, cmbFURIKAE) & "'")

            '�_��Z���i�����j
            If Trim(txtKEIYAKU_NJYU.Text) = "" Then
                SQL.Append(",KEIYAKU_NJYU_O  = " & "'" & Space(50) & "'")
            Else
                SQL.Append(",KEIYAKU_NJYU_O  = " & "'" & Trim(txtKEIYAKU_NJYU.Text) & "'")
            End If
            '�d�b�ԍ�
            If Trim(txtKEIYAKU_DENWA.Text) = "" Then
                SQL.Append(",KEIYAKU_DENWA_O = " & "'" & Space(12) & "'")
            Else
                SQL.Append(",KEIYAKU_DENWA_O = " & "'" & Trim(txtKEIYAKU_DENWA.Text) & "'")
            End If
            '���敪
            SQL.Append(",KAIYAKU_FLG_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAIYAKU_TXT, cmbKAIYAKU_FLG) & "'")

            '�i���敪
            SQL.Append(",SINKYU_KBN_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SINKYU_TXT, cmbSINKYU_KBN) & "'")

            '��ڂh�c
            SQL.Append(",HIMOKU_ID_O     = " & "'" & fn_SplitHimokuID() & "'")
            '���q�L���t���O
            '2017/11/28 �^�X�N�j���� CHG (�W���ŏC��(��177)) -------------------- START
            SQL.Append(",TYOUSI_FLG_O    = " & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TYOUSIUMU_TXT, cmbTYOUSI))
            'SQL.Append(",TYOUSI_FLG_O    = " & cmbTYOUSI().SelectedIndex)
            '2017/11/28 �^�X�N�j���� CHG (�W���ŏC��(��177)) -------------------- END

            '���q���w�N�x
            If Trim(str���q���w�N�x) <> "" Then
                SQL.Append(",TYOUSI_NENDO_O  = " & "'" & Trim(str���q���w�N�x) & "'")
            End If
            '���q�ʔ�
            If Trim(str���q�ʔ�) <> "" Then
                SQL.Append(",TYOUSI_TUUBAN_O = " & CInt(str���q�ʔ�))
            End If
            '���q�w�N
            If Trim(str���q�w�N) <> "" Then
                SQL.Append(",TYOUSI_GAKUNEN_O= " & CByte(str���q�w�N))
            End If
            '���q�N���X
            If Trim(str���q�N���X) <> "" Then
                SQL.Append(",TYOUSI_CLASS_O  = " & CByte(str���q�N���X))
            End If
            '���q���k�ԍ�
            If Trim(str���q���k�ԍ�) <> "" Then
                SQL.Append(",TYOUSI_SEITONO_O= " & "'" & Trim(str���q���k�ԍ�) & "'")
            End If

            '��ڂP�`��ڂP�T�̐������@,���z
            For intRowCnt As Integer = 0 To 14 '2007/04/04�@��ڂ͂P�O�܂ŉ�ʓ��́A�P�P����O�Œ�
                '2017/04/06 �^�X�N�j���� DEL �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                If intRowCnt <= (intMaxHimokuCnt - 1) Then
                    'If intRowCnt <= 9 Then
                    '2017/04/06 �^�X�N�j���� DEL �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    '��ڂP�`�P�O
                    With dgv
                        '��ږ��̗L��
                        Select Case Trim(.Rows(intRowCnt).Cells(0).Value)
                            Case ""
                                '�������@
                                SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='0'")
                                '���z
                                SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                            Case Else
                                '�������@
                                Dim SeikyuFlg As String = "0"
                                If .Rows(intRowCnt).Cells(1).Value = "�ꗥ" Then
                                    SeikyuFlg = "0"
                                ElseIf .Rows(intRowCnt).Cells(1).Value = "��" Then
                                    SeikyuFlg = "1"
                                End If
                                SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='" & SeikyuFlg & "'")

                                '2006/10/24�@�������@���ꗥ(=0)�̏ꍇ�E���z���󗓂������ꍇ�͋��z��0�Œ��ݒ肷�� 
                                '2017/05/09 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
                                '�^�Öٕϊ��ɂ��G���[�C��
                                If .Rows(intRowCnt).Cells(1).Value = "�ꗥ" Or CStr(.Rows(intRowCnt).Cells(2).Value).Trim = "" Then
                                    'If .Rows(intRowCnt).Cells(1).Value = "�ꗥ" Or .Rows(intRowCnt).Cells(2).Value = "" Then
                                    '2017/05/09 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
                                    '�ꗥ
                                    SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                                Else
                                    '��
                                    SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =" & CDec(.Rows(intRowCnt).Cells(2).Value).ToString)
                                End If
                        End Select
                    End With
                    '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                Else
                    '�������@
                    SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='0'")
                    '���z
                    SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                End If
                'Else
                '   '��ڂP�P�`�P�T
                '   '�������@
                '   SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='0'")
                '   '���z
                '   SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                '   End If
                '2017/02/23 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
            Next intRowCnt

            '�X�V���t
            SQL.Append(",KOUSIN_DATE_O ='" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            SQL.Append(" WHERE")
            SQL.Append(" GAKKOU_CODE_O  ='" & txtGAKKOU_CODE.Text & "'")
            SQL.Append(" AND NENDO_O        ='" & txtNENDO.Text & "'")
            SQL.Append(" AND TUUBAN_O       = " & CInt(txtTUUBAN.Text))
            SQL.Append(" AND TUKI_NO_O     ='" & Format(pTuki, "00") & "'")

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
            ret = False
        End Try

        Return ret

    End Function
    Private Function fn_GetKinNameSitName() As Integer
        '���Z�@�֖��A�x�X���̎擾

        fn_GetKinNameSitName = 0

        If Trim(txtTKIN_NO.Text) <> "" And Trim(txtTSIT_NO.Text) <> "" Then
            txtTKIN_NO.Text = txtTKIN_NO.Text.Trim.PadLeft(4, "0")
            txtTSIT_NO.Text = txtTSIT_NO.Text.Trim.PadLeft(3, "0")

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            SQL = " SELECT * FROM TENMAST"
            SQL += " WHERE KIN_NO_N ='" & Trim(txtTKIN_NO.Text) & "'"
            SQL += " AND SIT_NO_N ='" & Trim(txtTSIT_NO.Text) & "'"

            If Not OraReader.DataReader(SQL) Then
                txtTKIN_NO.Text = ""
                txtTSIT_NO.Text = ""
                MessageBox.Show(MSG0096W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                lab�戵���Z�@��.Text = ""
                lab�戵�x�X.Text = ""
                fn_GetKinNameSitName = 1
                txtTKIN_NO.Focus()
            Else
                lab�戵���Z�@��.Text = OraReader.GetItem("KIN_NNAME_N")
                lab�戵�x�X.Text = OraReader.GetItem("SIT_NNAME_N")
            End If

            OraReader.Close()
            OraReader = Nothing
        End If

    End Function
    Private Function fn_GetClassName() As Integer

        Dim intCnt As Integer
        Dim chkCLASS As Boolean = False '2006/10/11�@�N���X���݃`�F�b�N

        '�N���X���̎擾

        '�w�Z�R�[�h,�w�N�R�[�h�������͂̏ꍇ�̓`�F�b�N���s��Ȃ�
        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" And Trim(txtCLASS_CODE.Text) <> "" Then

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""

            SQL = " SELECT * FROM GAKMAST1"
            SQL += " WHERE GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) & "'"
            SQL += " AND GAKUNEN_CODE_G =" & CByte(txtGAKUNEN_CODE.Text)

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                lab�N���X��.Text = ""
                txtGAKUNEN_CODE.Focus()
            Else
                '���͂����N���X�ƈ�v����N���X��DB����擾
                '��v�����N���X�R�[�h�̃N���X���̂��擾����
                For intCnt = 1 To 20
                    '�ݒ肳��Ă��Ȃ�NULL�l�̏ꍇ�̓N���X���ݒ�𔲂���
                    '�N���X���ƂтƂтœo�^�����ꍇ�̑Ώ��ǉ�
                    If IsDBNull(OraReader.GetItem("CLASS_CODE1" & Format(intCnt, "00") & "_G")) = True Then
                        '�N���X��΂��Ή��̂��ߏ����Ȃ�
                    Else
                        '��ʏ�œ��͂���Ă���N���X�R�[�h�ƈ�v����N���X�R�[�h�̃N���X���̂����x���ɐݒ�
                        If GCom.NzInt(txtCLASS_CODE.Text) = GCom.NzInt((OraReader.GetItem("CLASS_CODE1" & Format(intCnt, "00") & "_G"))) Then
                            lab�N���X��.Text = OraReader.GetItem("CLASS_NAME1" & Format(intCnt, "00") & "_G")
                            chkCLASS = True
                            Exit For
                        End If
                    End If
                Next intCnt

                '�N���X�����o����Ȃ������ꍇ�A�N���X�����x���͋󔒂ɂ���
                If chkCLASS = False Then
                    MessageBox.Show(G_MSG0068W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtCLASS_CODE.Text = ""
                    txtCLASS_CODE.Focus()
                    lab�N���X��.Text = ""
                End If
            End If

            OraReader.Close()
            OraReader = Nothing
        Else
            '2006/10/11�@�w�N���E�N���X�����󗓂������ꍇ�A�N���X�����x�����󔒂ɂ���
            lab�N���X��.Text = ""
        End If

    End Function
    Private Function fn_GetSELECTSEITOMASTSQL(ByRef SQL As StringBuilder) As Boolean

        SQL.Append(" SELECT * FROM SEITOMASTVIEW ")
        Select Case True
            Case (Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtNENDO.Text) <> "" And Trim(txtTUUBAN.Text) <> "")
                '�w�Z�R�[�h�A���w�N�x�A�ʔԂ����͂���Ă���ꍇ
                SQL.Append(" WHERE GAKKOU_CODE_O ='" & txtGAKKOU_CODE.Text & "'")
                SQL.Append(" AND NENDO_O ='" & Trim(txtNENDO.Text) & "'")
                SQL.Append(" AND TUUBAN_O = " & CInt(txtTUUBAN.Text))

                Return True
            Case (Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" And Trim(txtCLASS_CODE.Text) <> "" And Trim(txtSEITO_NO.Text) <> "")
                '�w�Z�R�[�h�A�w�N�R�[�h�A�N���X�R�[�h�A���k�ԍ������͂���Ă���ꍇ
                SQL.Append(" WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                SQL.Append(" AND GAKUNEN_CODE_O = " & CByte(txtGAKUNEN_CODE.Text))
                SQL.Append(" AND CLASS_CODE_O  = " & CByte(txtCLASS_CODE.Text))
                SQL.Append(" AND SEITO_NO_O ='" & Trim(txtSEITO_NO.Text) & "'")

                Return True
            Case Else
                '�w�Z�R�[�h
                If Trim(txtGAKKOU_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If
                '���w�N�x
                If Trim(txtNENDO.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���w�N�x"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtNENDO.Focus()
                    Return False
                End If
                '�ʔ�
                If Trim(txtTUUBAN.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "�ʔ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTUUBAN.Focus()
                    Return False
                End If
                '�w�N
                If Trim(txtGAKUNEN_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "�w�N�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()
                    Return False
                End If
                '�N���X
                If Trim(txtCLASS_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "�N���X�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtCLASS_CODE.Focus()
                    Return False
                End If
                '���k�ԍ�
                If Trim(txtSEITO_NO.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "���k�ԍ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSEITO_NO.Focus()
                    Return False
                End If
        End Select

    End Function
    Private Function fn_GetGakunenName() As Boolean

        '�w�N���̎擾�i�w�Z�}�X�^�P�Q�Ɓj
        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" Then

            Dim OraReader As CASTCommon.MyOracleReader = Nothing

            Try
                Dim SQL As New StringBuilder(128)
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                SQL.Append(" AND GAKUNEN_CODE_G = " & CByte(txtGAKUNEN_CODE.Text))

                OraReader = New CASTCommon.MyOracleReader(MainDB)

                If Not OraReader.DataReader(SQL) Then
                    MessageBox.Show(G_MSG0069W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab�w�N��.Text = ""
                    txtGAKUNEN_CODE.Text = "" '2006/10/11
                    txtGAKUNEN_CODE.Focus() '2006/10/11�@�t�H�[�J�X���w�N�R�[�h���ɓ��Ă�

                    Return False
                Else
                    lab�w�N��.Text = OraReader.GetItem("GAKUNEN_NAME_G")

                    Return True
                End If

            Catch ex As Exception
                Return False
            Finally
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                    OraReader = Nothing
                End If
            End Try
        Else
            '�w�Z�R�[�h���󔒎��A�w�Z�����x�����󔒂ɂ���
            lab�w�N��.Text = ""
        End If

        Return True

    End Function
    Private Function fn_SplitHimokuID() As String
        '��ڂh�c�E�p�^�[���������ڂh�c�𒊏o
        Return cmbHIMOKU.Text.Substring(0, InStr(cmbHIMOKU.Text, "�E") - 1)
    End Function
    Private Function fn_SetHimokuID(ByVal pHimoku_ID As String) As Boolean
        If Trim(pHimoku_ID) = "" Then
            Exit Function
        End If
        For i As Integer = 0 To 998
            cmbHIMOKU.SelectedIndex = i
            If pHimoku_ID = Mid(cmbHIMOKU.Text, 1, 3) Then
                Exit For
            End If
        Next
    End Function
    Private Function fn_FormInitialize() As Integer
        '��ʂ̏���������

        fn_FormInitialize = 0

        '�ʔ�
        txtTUUBAN.Text = ""
        '���ʃR���{
        cmbSEIBETU.SelectedIndex = 0
        '���k�����i�J�i�j
        txtSEITO_KNAME.Text = ""
        '���k�����i�����j
        txtSEITO_NNAME.Text = ""
        '�U�֕��@�R���{
        cmbFURIKAE.SelectedIndex = 0
        '���Z�@�փR�[�h
        txtTKIN_NO.Text = ""
        lab�戵���Z�@��.Text = ""
        lab�戵�x�X.Text = ""
        '�x�X�R�[�h
        txtTSIT_NO.Text = ""
        '�ȖڃR���{
        cmbKAMOKU.SelectedIndex = 0
        '�����ԍ�
        txtKOUZA.Text = ""
        '���`�l�i�J�i�j
        txtMEIGI_KNAME.Text = ""
        '���`�l�i�����j
        txtMEIGI_NNAME.Text = ""
        '�_��Z���i�����j
        txtKEIYAKU_NJYU.Text = ""
        '�d�b�ԍ�
        txtKEIYAKU_DENWA.Text = ""
        '�i���敪�R���{
        cmbSINKYU_KBN.SelectedIndex = 0
        '���敪�R���{
        cmbKAIYAKU_FLG.SelectedIndex = 0
        '���q���R���{
        cmbTYOUSI.SelectedIndex = 0
        '��ڃR���{�{�b�N�X�̃N���A
        cmbHIMOKU.Items.Clear()
        '��ڏ��S�����痂�R���̃N���A
        Call Sub_Himoku_Initialize()

        lblKOUZA_CHK.Text = ""

        '2017/04/28 saitou RSV2 ADD �W���@�\�ǉ�(�X�V��) ---------------------------------------- START
        Me.lblKousinDate.Text = ""
        '2017/04/28 saitou RSV2 ADD ------------------------------------------------------------- END

    End Function
    Private Function fn_CheckEntry(ByVal pIndex As Integer) As Boolean

        Dim intCnt As Integer

        fn_CheckEntry = False

        '�w�Z�R�[�h
        '�����̓`�F�b�N
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Exit Function
        Else
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '�w�Z�}�X�^���݃`�F�b�N
            SQL = "SELECT GAKKOU_CODE_G"
            SQL += " FROM GAKMAST1"
            SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If
        End If

        '���w�N�x
        '�����̓`�F�b�N
        If Trim(txtNENDO.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "���w�N�x"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtNENDO.Focus()
            Exit Function
            'Else
            '    '2007/02/15
            '    If txtNENDO.Text.Trim.Length <> 4 Then
            '        MessageBox.Show("���w�N�x�͂S���Őݒ肵�Ă�������", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtNENDO.Focus()
            '        Exit Function
            '    End If
        End If

        '�ʔ�
        '�����̓`�F�b�N
        If Trim(txtTUUBAN.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�ʔ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTUUBAN.Focus()
            Exit Function
        Else
            '�V�K�o�^�̏ꍇ�̂݃`�F�b�N����
            Select Case pIndex
                Case 0
                    If Trim(txtGAKUNEN_CODE.Text) <> "" Then
                        '���k�}�X�^���݃`�F�b�N(�ʔ�)
                        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                        Dim SQL As String = ""
                        SQL = " SELECT * FROM SEITOMAST"
                        SQL += " WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                        SQL += " AND GAKUNEN_CODE_O =" & CByte(txtGAKUNEN_CODE.Text)
                        SQL += " AND TUUBAN_O = " & CDbl(txtTUUBAN.Text)
                        SQL += " AND NENDO_O = '" & Trim(txtNENDO.Text) & "'"

                        If Not OraReader.DataReader(SQL) Then
                        Else
                            MessageBox.Show(G_MSG0070W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtTUUBAN.Focus()
                            Exit Function
                        End If
                    End If
            End Select
        End If

        '�w�N
        '�����̓`�F�b�N
        If Trim(txtGAKUNEN_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�N�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN_CODE.Focus()
            Exit Function
        Else
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '�g�p�w�N�`�F�b�N
            SQL = "SELECT SIYOU_GAKUNEN_T "
            SQL += " FROM GAKMAST2"
            SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKUNEN_CODE.Focus()
                Exit Function
            Else
                Select Case CInt(OraReader.GetItem("SIYOU_GAKUNEN_T"))
                    Case Is < CInt(txtGAKUNEN_CODE.Text)
                        '�g�p�w�N�ȏ�̊w�N����͂����ꍇ�̓G���[
                        MessageBox.Show(G_MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKUNEN_CODE.Focus()
                        Exit Function
                End Select
            End If
        End If

        '�N���X
        '�����̓`�F�b�N
        If Trim(txtCLASS_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�N���X�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCLASS_CODE.Focus()
            Exit Function
        Else
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '�g�p�N���X�`�F�b�N
            SQL = "SELECT * FROM GAKMAST1"
            SQL += " WHERE GAKKOU_CODE_G = '" & Trim(txtGAKKOU_CODE.Text) & "'"
            SQL += " AND GAKUNEN_CODE_G = " & CByte(txtGAKUNEN_CODE.Text)
            SQL += " AND ("
            For intCnt = 1 To 20
                SQL += IIf(intCnt = 1, "", "or") & " CLASS_CODE1" & Format(intCnt, "00") & "_G = '" & Trim(txtCLASS_CODE.Text).PadLeft(2, "0"c) & "'"
            Next intCnt
            SQL += " )"

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0068W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCLASS_CODE.Focus()
                Exit Function
            End If
        End If

        '���k�ԍ�
        '�����̓`�F�b�N
        If Trim(txtSEITO_NO.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "���k�ԍ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSEITO_NO.Focus()
            Exit Function
        End If

        '����
        '���I���`�F�b�N
        If cmbSEIBETU.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0071W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbSEIBETU.Focus()
            Exit Function
        End If

        '���k�����i�J�i�j
        '�����̓`�F�b�N
        If Trim(txtSEITO_KNAME.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "���k��(�J�i)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSEITO_KNAME.Focus()
            Exit Function
        Else
            If StrConv(txtSEITO_KNAME.Text, VbStrConv.Narrow).Length > 40 Then
                MessageBox.Show(G_MSG0087W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSEITO_KNAME.Focus()
                Exit Function
            End If
        End If

        '�U�֕��@
        '���I���`�F�b�N
        If cmbFURIKAE.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0072W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbFURIKAE.Focus()
            Exit Function
        End If

        '�戵���Z�@�ց@
        '�U�֕��@�������U��(=0)�̏ꍇ�̂ݕK�{�`�F�b�N
        Select Case cmbFURIKAE.SelectedIndex
            Case 0
                '�戵���Z�@��
                '�����̓`�F�b�N
                If Trim(txtTKIN_NO.Text) = "" Then
                    MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTKIN_NO.Focus()
                    Exit Function
                End If

                '�戵�x�X
                '�����̓`�F�b�N
                If Trim(txtTSIT_NO.Text) = "" Then
                    MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTSIT_NO.Focus()
                    Exit Function
                End If

                '�戵���Z�@��
                '���݁E�w�Z�}�X�^�E���s�}�X�^�`�F�b�N
                If fn_GetKinNameSitName() = 1 Then
                    txtTKIN_NO.Focus()
                    Exit Function
                ElseIf fn_CheckGakMast2FromGakkouCodeTKinNo() = 1 AndAlso fn_CheckG_TakoMastFromGakkouCodeTKinNo() = False Then
                    MessageBox.Show(G_MSG0073W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTKIN_NO.Focus()
                    Exit Function
                End If

                '�Ȗ�
                '���I���`�F�b�N
                If cmbKAMOKU.SelectedIndex = -1 Then
                    MessageBox.Show(G_MSG0055W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbKAMOKU.Focus()
                    Exit Function
                End If

                '�����ԍ�
                '�����̓`�F�b�N
                If Trim(txtKOUZA.Text) = "" Then
                    MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKOUZA.Focus()
                    Exit Function
                End If

                '�`�F�b�N�f�W�b�g�ǉ� 2007/02/13
                If STR_CHK_DJT = "1" Then '<---ini�t�@�C���̃`�F�b�N�f�W�b�g����敪 0:���Ȃ� 1:����
                    If txtTKIN_NO.Text.Trim = STR_JIKINKO_CODE Then '�����Ƀf�[�^�̂݃`�F�b�N�f�W�b�g���s
                        If GFUNC_CHK_DEJIT(txtTKIN_NO.Text.Trim, txtTSIT_NO.Text.Trim, CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU)), txtKOUZA.Text.Trim) = False Then
                            MessageBox.Show(G_MSG0074W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtKOUZA.Focus()
                            Exit Function
                        End If
                    End If
                End If
        End Select

        '�������`�l�i�J�i�j
        '�����̓`�F�b�N
        If Trim(txtMEIGI_KNAME.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�������`�l(�J�i)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtMEIGI_KNAME.Focus()
            Exit Function
        Else
            If StrConv(txtMEIGI_KNAME.Text, VbStrConv.Narrow).Length > 40 Then
                MessageBox.Show(G_MSG0088W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtMEIGI_KNAME.Focus()
                Exit Function
            End If

        End If


        '�C�Ӎ��ڃ`�F�b�N�ǉ� 2006/05/10
        '���k�����i�����j
        If Trim(txtSEITO_NNAME.Text) <> "" Then
            If StrConv(txtSEITO_NNAME.Text, VbStrConv.Wide).Length > 25 Then
                MessageBox.Show(G_MSG0075W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSEITO_NNAME.Focus()
                Exit Function
            End If

        End If


        '�������`�l�����i�����j
        If Trim(txtMEIGI_NNAME.Text) <> "" Then
            If StrConv(txtMEIGI_NNAME.Text, VbStrConv.Wide).Length > 25 Then
                MessageBox.Show(G_MSG0076W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtMEIGI_NNAME.Focus()
                Exit Function
            End If

        End If

        '�_��Z���i�����j
        If Trim(txtKEIYAKU_NJYU.Text) <> "" Then
            If StrConv(txtKEIYAKU_NJYU.Text, VbStrConv.Wide).Length > 25 Then
                MessageBox.Show(G_MSG0077W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKEIYAKU_NJYU.Focus()
                Exit Function
            End If
        End If

        '�i���敪
        '���I���`�F�b�N
        If cmbSINKYU_KBN.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0078W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbSINKYU_KBN.Focus()
            Exit Function
        End If

        '���敪
        '���I���`�F�b�N
        If cmbKAIYAKU_FLG.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0079W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbKAIYAKU_FLG.Focus()
            Exit Function
        End If

        '��ڏ��
        '���I���`�F�b�N
        If cmbHIMOKU.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0080W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbHIMOKU.Focus()
            Exit Function
        End If

        '���q���
        Select Case cmbTYOUSI.SelectedIndex
            Case -1
                MessageBox.Show(G_MSG0081W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbTYOUSI.Focus()
                Exit Function
            Case 1
                '���q�L��I����
                If Trim(str���q�w�Z�R�[�h) = "" Or _
                   Trim(str���q���w�N�x) = "" Or _
                   Trim(str���q�ʔ�) = "" Or _
                   Trim(str���q�w�N) = "" Or _
                   Trim(str���q�N���X) = "" Or _
                   Trim(str���q���k�ԍ�) = "" Then
                    MessageBox.Show(String.Format(MSG0281W, "���q���"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbTYOUSI.Focus()

                    Exit Function
                End If
        End Select

        fn_CheckEntry = True

    End Function
    Private Function fn_CheckG_TakoMastFromGakkouCodeTKinNo() As Boolean
        If Trim(txtTKIN_NO.Text) <> "" And Trim(txtGAKKOU_CODE.Text) <> "" Then
            If STR_JIKINKO_CODE <> Trim(txtTKIN_NO.Text) Then
                Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                Dim SQL As String = ""
                SQL = " SELECT * FROM G_TAKOUMAST"
                SQL &= " WHERE GAKKOU_CODE_V ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                SQL &= " AND TKIN_NO_V ='" & Trim(txtTKIN_NO.Text) & "'"

                If Not OraReader.DataReader(SQL) Then
                    '���s�����ő���
                    OraReader.Close()
                    OraReader = Nothing

                    Return False
                End If

                OraReader.Close()
                OraReader = Nothing
            End If
        End If

        Return True

    End Function
    Private Function fn_CheckGakMast2FromGakkouCodeTKinNo() As Integer
        '���Z�@�ւ��w�Z�}�X�^�Q�ɑ��݂��邩�`�F�N
        fn_CheckGakMast2FromGakkouCodeTKinNo = 0

        If Trim(txtTKIN_NO.Text) <> "" And Trim(txtGAKKOU_CODE.Text) <> "" Then

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As String = ""
            SQL = " SELECT * FROM GAKMAST2"
            SQL += " WHERE GAKKOU_CODE_T ='" & Trim(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) & "'"
            SQL += " AND TKIN_NO_T ='" & Trim(txtTKIN_NO.Text) & "'"

            If OraReader.DataReader(SQL) Then
                '���s�����ő���
            Else
                fn_CheckGakMast2FromGakkouCodeTKinNo = 1
            End If

            OraReader.Close()
            OraReader = Nothing
        End If

    End Function
    Private Function fn_CheckEntryKouza() As Boolean
        lblKOUZA_CHK.Text = ""
        fn_CheckEntryKouza = False

        '�戵���Z�@�ց@
        '�戵���Z�@��
        '�����̓`�F�b�N
        If Trim(txtTKIN_NO.Text) = "" Then
            MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTKIN_NO.Focus()
            Exit Function
        End If

        '�戵�x�X
        '�����̓`�F�b�N
        If Trim(txtTSIT_NO.Text) = "" Then
            MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTSIT_NO.Focus()
            Exit Function
        End If

        '�Ȗ�
        '���I���`�F�b�N
        If cmbKAMOKU.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0055W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbKAMOKU.Focus()
            Exit Function
        End If

        '�����ԍ�
        '�����̓`�F�b�N
        If Trim(txtKOUZA.Text) = "" Then
            MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKOUZA.Focus()
            Exit Function
        End If

        '�����ɃR�[�h�̏ꍇ�̂݌����`�F�b�N���s
        If txtTKIN_NO.Text <> STR_JIKINKO_CODE Then
            lblKOUZA_CHK.Text = ""
            fn_CheckEntryKouza = True
            Exit Function
        End If


        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As String = ""
        '�g�p�w�N�`�F�b�N
        SQL = "SELECT KIGYO_CODE_T,FURI_CODE_T"
        SQL += " FROM GAKMAST2"
        SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

        If Not OraReader.DataReader(SQL) Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN_CODE.Focus()
            Exit Function
        Else
            strKIGYO_CODE = OraReader.GetItem("KIGYO_CODE_T")
            strFURI_CODE = OraReader.GetItem("FURI_CODE_T")
        End If

        '�������}�X�^���������A���������݂��邩�A���U�_�񂪂��邩����
        '����������A�������`�l�i�J�i�j�������͂̏ꍇ�́A�������}�X�^�̒l��\��

        '��ƃR�[�h�A�U�փR�[�h����v������̂����邩�ǂ�������
        OraReader.Close()
        OraReader = Nothing
        OraReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = " SELECT * FROM KDBMAST"
        SQL += " WHERE TSIT_NO_D =" & SQ(Trim(txtTSIT_NO.Text))
        SQL += " AND KAMOKU_D =" & SQ(CStr(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU)))
        SQL += " AND KOUZA_D =" & SQ(Trim(txtKOUZA.Text))
        SQL += " AND KIGYOU_CODE_D =" & SQ(strKIGYO_CODE)
        SQL += " AND FURI_CODE_D =" & SQ(strFURI_CODE)

        If Not OraReader.DataReader(SQL) Then
        Else
            lblKOUZA_CHK.Text = ""
            '2008/09/15 ���S�M���@���`�F�b�N��ǉ�===============
            If OraReader.GetItem("KATU_KOUZA_D") = "0" Then
                lblKOUZA_CHK.Text = "��������"
                fn_CheckEntryKouza = True
                Exit Function
            End If
            '======================================================
            If Trim(txtMEIGI_KNAME.Text) = "" Then
                If Trim(OraReader.GetItem("KOKYAKU_KNAME_D")).Length > 40 Then
                    txtMEIGI_KNAME.Text = Trim(OraReader.GetItem("KOKYAKU_KNAME_D")).Substring(0, 40)
                Else
                    txtMEIGI_KNAME.Text = Trim(OraReader.GetItem("KOKYAKU_KNAME_D"))
                End If
            Else
                If Trim(txtMEIGI_KNAME.Text) <> Trim(OraReader.GetItem("KOKYAKU_KNAME_D")) Then
                    MessageBox.Show(String.Format(G_MSG0083W, Trim(txtMEIGI_KNAME.Text), OraReader.GetItem("KOKYAKU_KNAME_D")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lblKOUZA_CHK.Text = "�J�i�s��v"
                Else
                    lblKOUZA_CHK.Text = ""
                End If
            End If
            fn_CheckEntryKouza = True
            Exit Function
        End If

        '��ƃR�[�h�A�U�փR�[�h����v������̂��Ȃ������Ƃ��A��������v������̂����邩����
        OraReader.Close()
        OraReader = Nothing
        OraReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = " SELECT * FROM KDBMAST"
        SQL += " WHERE TSIT_NO_D =" & SQ(Trim(txtTSIT_NO.Text))
        SQL += " AND KAMOKU_D =" & SQ(CStr(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU)))
        SQL += " AND KOUZA_D =" & SQ(Trim(txtKOUZA.Text))

        If Not OraReader.DataReader(SQL) Then
            lblKOUZA_CHK.Text = "�����Ȃ�"
            fn_CheckEntryKouza = True
            Exit Function
        Else
            '2008/09/15 ���S�M���@���`�F�b�N��ǉ�===============
            If OraReader.GetItem("KATU_KOUZA_D") = "0" Then
                lblKOUZA_CHK.Text = "��������"
                fn_CheckEntryKouza = True
                Exit Function
            End If
            '======================================================

            lblKOUZA_CHK.Text = "���U�_��Ȃ�"
            If Trim(txtMEIGI_KNAME.Text) = "" Then
                txtMEIGI_KNAME.Text = Mid(OraReader.GetItem("KOKYAKU_KNAME_D").ToString.Trim, 1, 40)
            Else
                lblKOUZA_CHK.Text = ""
                If Trim(txtMEIGI_KNAME.Text) <> Trim(OraReader.GetItem("KOKYAKU_KNAME_D")) Then
                    MessageBox.Show(String.Format(G_MSG0083W, Trim(txtMEIGI_KNAME.Text), OraReader.GetItem("KOKYAKU_KNAME_D")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lblKOUZA_CHK.Text = "�_�񖳶ŕs��v"
                Else
                    lblKOUZA_CHK.Text = "���U�_��Ȃ�"
                End If
            End If
            fn_CheckEntryKouza = True
            Exit Function
        End If
    End Function
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '�w�Z���R���{�{�b�N�X�ݒ�
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lab�w�Z��.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '���w�N�x�e�L�X�g�{�b�N�X��FOCUS
        txtNENDO.Focus()

        If txtGAKUNEN_CODE.Text.Trim <> "" Then
            '�w�N���̎擾
            If fn_GetGakunenName() = True Then
                '�w�Z�R�[�h�Ɗw�N�����ڃ��R�[�h�i��ڂh�c�^��ڃp�^�[�����j���擾
                Call Sub_GetHimokuID()
                Call Sub_Himoku_Initialize()
            End If
        End If
    End Sub
    Private Sub cmbTYOUSI_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbTYOUSI.SelectedIndexChanged

        Select Case cmbTYOUSI.SelectedIndex
            Case 0
                btnTyousi.Enabled = False
            Case Else
                btnTyousi.Enabled = True
        End Select

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '�w�Z������
            SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
            If Not OraReader.DataReader(SQL) Then
                lab�w�Z��.Text = ""
            Else

                lab�w�Z��.Text = CStr(OraReader.GetItem("GAKKOU_NNAME_G"))
            End If
        End If

    End Sub
    Private Sub txtCLASS_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCLASS_CODE.Validating
        If Trim(txtCLASS_CODE.Text) <> "" Then
            '�N���X���̎擾
            Call fn_GetClassName()
        End If
    End Sub
    Private Sub txtTKIN_NO_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTKIN_NO.Validating

        '���Z�@��
        If Trim(txtTKIN_NO.Text) <> "" Then
            '���Z�@�֖��A�x�X���̐ݒ�
            If fn_GetKinNameSitName() <> 0 Then
                Exit Sub
            End If
            '�w�Z�}�X�^�Q�`�F�b�N
            If fn_CheckGakMast2FromGakkouCodeTKinNo() = 0 Then
            Else
                '���s�}�X�^�`�F�b�N
                If fn_CheckG_TakoMastFromGakkouCodeTKinNo() = True Then
                Else
                    MessageBox.Show(G_MSG0073W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
        End If

    End Sub
    Private Sub txtTSIT_NO_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTSIT_NO.Validating
        '�x�X
        If Trim(txtTSIT_NO.Text) <> "" Then
            '���Z�@�֖��A�x�X���̐ݒ�
            If fn_GetKinNameSitName() <> 0 Then
                Exit Sub
            End If
            '�w�Z�}�X�^�Q�`�F�b�N
            If fn_CheckGakMast2FromGakkouCodeTKinNo() = 0 Then
            Else
                '���s�}�X�^�`�F�b�N
                If fn_CheckG_TakoMastFromGakkouCodeTKinNo() = True Then
                Else
                    MessageBox.Show(G_MSG0073W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
        End If

    End Sub
    Private Sub txtGAKUNEN_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN_CODE.Validating
        '�w�N���̎擾
        If fn_GetGakunenName() = True Then
            '�w�Z�R�[�h�Ɗw�N�����ڃ��R�[�h�i��ڂh�c�^��ڃp�^�[�����j���擾
            cmbHIMOKU.Items.Clear()
            Call Sub_GetHimokuID()
            '��ڏ��S�����痂�R���̃N���A
            Call Sub_Himoku_Initialize()
        End If
    End Sub
    Private Sub txtTUUBAN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTUUBAN.Validating
        If cmbHIMOKU.Items.Count = 0 AndAlso lab�w�N��.Text.Trim <> "" AndAlso lab�w�Z��.Text.Trim <> "" Then
            '�w�Z�R�[�h�Ɗw�N�����ڃ��R�[�h�i��ڂh�c�^��ڃp�^�[�����j���擾
            cmbHIMOKU.Items.Clear()
            Call Sub_GetHimokuID()
        End If
    End Sub
    '�S�p�����̈�o�C�g�������p
    Private Sub GetLimitString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles txtKEIYAKU_NJYU.Validating, txtSEITO_NNAME.Validating, txtMEIGI_NNAME.Validating
        With CType(sender, TextBox)
            .Text = GCom.GetLimitString(.Text, .MaxLength)
        End With
    End Sub
    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl
    Private CmbEditCtrl As DataGridViewComboBoxEditingControl
    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        Select Case e.ColumnIndex
            Case 0
                CType(sender, DataGridView).ImeMode = ImeMode.Hiragana
            Case 1, 2
                CType(sender, DataGridView).ImeMode = ImeMode.Disable
        End Select
    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        If colNo = 1 Then
            CmbEditCtrl = CType(e.Control, DataGridViewComboBoxEditingControl)
        Else
            TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)
        End If
        Select Case colNo
            Case 0
                AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 1
                AddHandler CmbEditCtrl.GotFocus, AddressOf CAST.GotFocus
                AddHandler CmbEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 2
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        Select Case colNo
            Case 0
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 1
                RemoveHandler CmbEditCtrl.GotFocus, AddressOf CAST.GotFocus
                RemoveHandler CmbEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 2
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select

        Call CellLeave(sender, e)
    End Sub
    Private Sub Sub_DataGridViewComboLock(ByVal dgv As DataGridView, ByVal RowNo As Integer, ByVal Tuki As Integer)

        '�������@�̑I���ɂ����z�̕ύX�E�s�̐ݒ�
        With dgv
            Select Case .Rows(RowNo).Cells(1).Value
                Case "�ꗥ"
                    .Rows(RowNo).Cells(2).Value = Format(CDec(Str_Spread(Tuki, RowNo)), "#,##0")
                    .Rows(RowNo).Cells(2).ReadOnly = True
                Case "��"
                    .Rows(RowNo).Cells(2).ReadOnly = False
            End Select
        End With

    End Sub
    Private Sub CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case CType(sender, DataGridView).Name
            Case "CustomDataGridView1"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 3)
            Case "CustomDataGridView2"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 4)
            Case "CustomDataGridView3"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 5)
            Case "CustomDataGridView4"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 6)
            Case "CustomDataGridView5"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 7)
            Case "CustomDataGridView6"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 8)
            Case "CustomDataGridView7"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 9)
            Case "CustomDataGridView8"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 10)
            Case "CustomDataGridView9"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 11)
            Case "CustomDataGridView10"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 0)
            Case "CustomDataGridView11"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 1)
            Case "CustomDataGridView12"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 2)
        End Select
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        Dim SumKingaku As Decimal = 0

        For cnt As Integer = 0 To CType(sender, DataGridView).Rows.Count - 1
            If IsNumeric(CType(sender, DataGridView).Rows(cnt).Cells(2).Value) Then
                SumKingaku += CDec(CType(sender, DataGridView).Rows(cnt).Cells(2).Value)
                '2017/08/04 �^�X�N�j���� ADD �W���ŏC���i�ʋ��z��0�~�ݒ�s��j---------------------- START
            Else
                CType(sender, DataGridView).Rows(cnt).Cells(2).Value = "0"
                '2017/08/04 �^�X�N�j���� ADD �W���ŏC���i�ʋ��z��0�~�ݒ�s��j---------------------- END
            End If
        Next

        Select Case CType(sender, DataGridView).Name
            Case "CustomDataGridView1"
                lblKingaku_4.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView2"
                lblKingaku_5.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView3"
                lblKingaku_6.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView4"
                lblKingaku_7.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView5"
                lblKingaku_8.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView6"
                lblKingaku_9.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView7"
                lblKingaku_10.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView8"
                lblKingaku_11.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView9"
                lblKingaku_12.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView10"
                lblKingaku_1.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView11"
                lblKingaku_2.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView12"
                lblKingaku_3.Text = Format(SumKingaku, "#,##0")
        End Select

        If e.ColumnIndex = 2 Then
            If IsNumeric(CType(sender, DataGridView).Rows(e.RowIndex).Cells(2).Value) Then
                CType(sender, DataGridView).Rows(e.RowIndex).Cells(2).Value = Format(CDec(CType(sender, DataGridView).Rows(e.RowIndex).Cells(2).Value), "#,##0")
            End If
        End If

    End Sub
    Private Function ConvDBNullToString(ByVal Item As Object) As String
        If IsDBNull(Item) Then
            Return ""
        Else
            Return Item.ToString.Trim
        End If
    End Function
    '2017/05/09 �^�X�N�j���� CHG �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- START
    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles _
      CustomDataGridView1.RowPostPaint, CustomDataGridView2.RowPostPaint, CustomDataGridView3.RowPostPaint, _
      CustomDataGridView4.RowPostPaint, CustomDataGridView5.RowPostPaint, CustomDataGridView6.RowPostPaint, _
      CustomDataGridView7.RowPostPaint, CustomDataGridView8.RowPostPaint, CustomDataGridView9.RowPostPaint, _
      CustomDataGridView10.RowPostPaint, CustomDataGridView11.RowPostPaint, CustomDataGridView12.RowPostPaint, CustomDataGridView13.RowPostPaint
        ' Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles _
        'CustomDataGridView1.RowPostPaint, CustomDataGridView2.RowPostPaint, CustomDataGridView3.RowPostPaint, _
        'CustomDataGridView4.RowPostPaint, CustomDataGridView5.RowPostPaint, CustomDataGridView6.RowPostPaint, _
        'CustomDataGridView7.RowPostPaint, CustomDataGridView8.RowPostPaint, CustomDataGridView9.RowPostPaint, _
        'CustomDataGridView10.RowPostPaint, CustomDataGridView11.RowPostPaint, CustomDataGridView12.RowPostPaint
        '2017/05/09 �^�X�N�j���� CHG �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- END

        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' �s�w�b�_�̃Z���̈���A�s�ԍ���`�悷�钷���`�Ƃ���
        ' �i�������E�[��4�h�b�g�̂����Ԃ��󂯂�j
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' ��L�̒����`���ɍs�ԍ����c�����������E�l�ŕ`�悷��
        ' �t�H���g��F�͍s�w�b�_�̊���l���g�p����
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

    '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- START
    Private Sub DataGridView13_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles CustomDataGridView13.CellContentClick
        Dim dgv_H As DataGridView = CType(sender, CustomDataGridView)

        If dgv_H.Rows(e.RowIndex).Cells(0).Value = "" Then
            Exit Sub
        End If

        Dim HimoName As String = Trim(dgv_H.Rows(e.RowIndex).Cells(0).Value)
        Dim Seikyu As String = ""
        Select Case e.ColumnIndex
            Case 1  '�ꗥ
                Seikyu = "�ꗥ"
            Case 2  '��
                Seikyu = "��"
        End Select

        If MessageBox.Show(HimoName & "�̐������@���ꊇ��" & Seikyu & "�ɕύX���܂��B" & vbCrLf & "��낵���ł����H", "�������@�ꊇ�ύX", _
                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
            Exit Sub
        End If

        For TUKI As Integer = 1 To 12
            Dim dgv As DataGridView = Nothing
            Dim kei As Label = Nothing
            Select Case TUKI.ToString.PadLeft(2, "0"c)
                Case "04"
                    dgv = CustomDataGridView1
                    kei = lblKingaku_4
                Case "05"
                    dgv = CustomDataGridView2
                    kei = lblKingaku_5
                Case "06"
                    dgv = CustomDataGridView3
                    kei = lblKingaku_6
                Case "07"
                    dgv = CustomDataGridView4
                    kei = lblKingaku_7
                Case "08"
                    dgv = CustomDataGridView5
                    kei = lblKingaku_8
                Case "09"
                    dgv = CustomDataGridView6
                    kei = lblKingaku_9
                Case "10"
                    dgv = CustomDataGridView7
                    kei = lblKingaku_10
                Case "11"
                    dgv = CustomDataGridView8
                    kei = lblKingaku_11
                Case "12"
                    dgv = CustomDataGridView9
                    kei = lblKingaku_12
                Case "01"
                    dgv = CustomDataGridView10
                    kei = lblKingaku_1
                Case "02"
                    dgv = CustomDataGridView11
                    kei = lblKingaku_2
                Case "03"
                    dgv = CustomDataGridView12
                    kei = lblKingaku_3
            End Select

            Dim lngGoukei As Long = 0

            With dgv
                Select Case e.ColumnIndex
                    Case 1  '�ꗥ
                        .Rows(e.RowIndex).Cells(1).Value = "�ꗥ"
                        .Rows(e.RowIndex).Cells(2).Value = Str_Spread(0, e.RowIndex)
                        .Rows(e.RowIndex).Cells(2).ReadOnly = True
                    Case 2  '��
                        .Rows(e.RowIndex).Cells(1).Value = "��"
                        .Rows(e.RowIndex).Cells(2).ReadOnly = False
                        .Rows(e.RowIndex).Cells(2).Value = 0
                End Select

                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    lngGoukei += CLng(.Rows(intRow).Cells(2).Value)
                Next
                kei.Text = Format(lngGoukei, "#,##0")
            End With

        Next

    End Sub
    '2017/05/09 �^�X�N�j���� ADD �W���ŏC���i��ڐݒ�̈ꊇ�ύX�Ή��j---------------------- END

    '2017/06/05 saitou ADD �W���ŏC���i�C��No.0004�y���k���J�i�A�������`�l�J�i�ɑS�p������z�j ---------------------------------------- START
    Private Sub NzCheckString_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles _
        txtSEITO_KNAME.Validating, txtMEIGI_KNAME.Validating
        Try
            Call GCom.NzCheckString(CType(sender, TextBox))
            Dim BRet As Boolean = GCom.CheckZenginChar(CType(sender, TextBox))
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("�J�i�ϊ�", "���s", ex.ToString)
        End Try
    End Sub
    '2017/06/05 saitou ADD �W���ŏC�� ------------------------------------------------------------------------------------------------- END

#Region "���[���"
    '============================================================================
    'NAME           :fn_CreateCSV_ADDDEL
    'Parameter      :PntSyoriKbn ��������敪
    'Description    :KFGP106,108(���k�}�X�^�����e(�o�^,�폜))����p�b�r�u�t�@�C���쐬
    'Return         :
    'Create         :2010/09/16
    'Update         :
    '============================================================================
    Private Function fn_CreateCSV_ADDDEL(ByVal PntSyoriKbn As String) As Boolean

        '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ���k�}�X�^�����e���[���P ---------------------------------------- START
        Dim oraMainReader As CASTCommon.MyOracleReader = Nothing
        Dim oraHimoReader As CASTCommon.MyOracleReader = Nothing

        Dim SQL As New StringBuilder

        Try
            '2010/10/18 �o�^�A�폜�N���X�̔���
            Dim CreateCSV As Object = Nothing
            Select Case PntSyoriKbn
                Case "0" : CreateCSV = New KFGP106
                Case "2" : CreateCSV = New KFGP108
                Case Else
            End Select
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")

            '���k�}�X�^�r���[���琶�k���擾
            '��ڏ��͂����ł͎擾���Ȃ�
            With SQL
                .Append("select distinct ")
                .Append(" GAKKOU_CODE_O")
                .Append(",NENDO_O")
                .Append(",TUUBAN_O")
                .Append(",GAKUNEN_CODE_O")
                .Append(",CLASS_CODE_O")
                .Append(",SEITO_NO_O")
                .Append(",SEITO_KNAME_O")
                .Append(",SEITO_NNAME_O")
                .Append(",SEIBETU_O")
                .Append(",TKIN_NO_O")
                .Append(",TSIT_NO_O")
                .Append(",KAMOKU_O")
                .Append(",KOUZA_O")
                .Append(",MEIGI_KNAME_O")
                .Append(",MEIGI_NNAME_O")
                .Append(",FURIKAE_O")
                .Append(",KEIYAKU_NJYU_O")
                .Append(",KEIYAKU_DENWA_O")
                .Append(",KAIYAKU_FLG_O")
                .Append(",SINKYU_KBN_O")
                .Append(",HIMOKU_ID_O")
                .Append(",GAKKOU_NNAME_G")
                .Append(",TENMAST_G.SIT_NNAME_N as G_SIT_NNAME")
                .Append(",TENMAST_S.KIN_NNAME_N as S_KIN_NNAME")
                .Append(",TENMAST_S.SIT_NNAME_N as S_SIT_NNAME")

                .Append(" from SEITOMASTVIEW, GAKMAST1, GAKMAST2, TENMAST TENMAST_G, TENMAST TENMAST_S")

                .Append(" where GAKKOU_CODE_O = GAKKOU_CODE_G")
                .Append(" and GAKUNEN_CODE_O = GAKUNEN_CODE_G")

                .Append(" and GAKKOU_CODE_G = GAKKOU_CODE_T")

                .Append(" and TKIN_NO_T = TENMAST_G.KIN_NO_N")
                .Append(" and TSIT_NO_T = TENMAST_G.SIT_NO_N")

                '�W�������͋��Z�@�֓��͂����Ƃ��o�^�ł��邽�ߊO������
                .Append(" and TKIN_NO_O = TENMAST_S.KIN_NO_N(+)")
                .Append(" and TSIT_NO_O = TENMAST_S.SIT_NO_N(+)")

                .Append(" and GAKKOU_CODE_O = " & SQ(Me.SeitomastMenteListInfo.GakkouCode))
                .Append(" and NENDO_O = " & SQ(Me.SeitomastMenteListInfo.Nendo))
                .Append(" and TUUBAN_O = " & GCom.NzInt(Me.SeitomastMenteListInfo.Tuuban))

            End With

            oraMainReader = New CASTCommon.MyOracleReader(Me.MainDB)

            If oraMainReader.DataReader(SQL) = True Then
                'CSV�쐬
                Me.strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

                oraHimoReader = New CASTCommon.MyOracleReader(Me.MainDB)

                While oraMainReader.EOF = False

                    '��ڏ��擾 
                    For i As Integer = 1 To 15
                        With SQL
                            .Length = 0
                            .Append("select ")
                            .Append(" KINGAKU" & i.ToString("00") & "_O")
                            .Append(",HIMOKU_NAME" & i.ToString("00") & "_O")
                            .Append(" from SEITOMASTVIEW")
                            .Append(" where GAKKOU_CODE_O = " & SQ(Me.SeitomastMenteListInfo.GakkouCode))
                            .Append(" and NENDO_O = " & SQ(Me.SeitomastMenteListInfo.Nendo))
                            .Append(" and TUUBAN_O = " & GCom.NzInt(Me.SeitomastMenteListInfo.Tuuban))
                            .Append(" order by TUKI_NO_O")
                        End With

                        If oraHimoReader.DataReader(SQL) = True Then
                            With CreateCSV
                                .OutputCsvData(PntSyoriKbn, True)
                                .OutputCsvData(Today, True)
                                .OutputCsvData(NowTime, True)
                                .OutputCsvData(oraMainReader.GetString("G_SIT_NNAME"), True)
                                .OutputCsvData(oraMainReader.GetString("GAKKOU_CODE_O"), True)
                                .OutputCsvData(oraMainReader.GetString("GAKKOU_NNAME_G"), True)
                                .OutputCsvData(oraMainReader.GetString("NENDO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("GAKUNEN_CODE_O"), True)
                                .OutputCsvData(oraMainReader.GetString("TUUBAN_O"), True)
                                .OutputCsvData(oraMainReader.GetString("SEITO_NO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("SEITO_KNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("SEITO_NNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("MEIGI_KNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("MEIGI_NNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("TKIN_NO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("S_KIN_NNAME"), True)
                                .OutputCsvData(oraMainReader.GetString("TSIT_NO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("S_SIT_NNAME"), True)
                                Select Case oraMainReader.GetString("KAMOKU_O")
                                    Case "02" : .OutputCsvData("����", True)
                                    Case "01" : .OutputCsvData("����", True)
                                    Case Else : .OutputCsvData("", True)
                                End Select
                                .OutputCsvData(oraMainReader.GetString("KOUZA_O"), True)
                                .OutputCsvData(oraMainReader.GetString("CLASS_CODE_O"), True)
                                Select Case oraMainReader.GetString("SEIBETU_O")
                                    Case "0" : .OutputCsvData("�j", True)
                                    Case "1" : .OutputCsvData("��", True)
                                    Case "2" : .OutputCsvData("�|", True)
                                End Select
                                Select Case oraMainReader.GetString("FURIKAE_O")
                                    Case "0" : .OutputCsvData("�����U��", True)
                                    Case "1" : .OutputCsvData("�W������", True)
                                    Case "2" : .OutputCsvData("��~", True)
                                End Select
                                Select Case oraMainReader.GetString("KAIYAKU_FLG_O")
                                    Case "0" : .OutputCsvData("�ʏ�", True)
                                    Case "9" : .OutputCsvData("���", True)
                                End Select
                                .OutputCsvData(oraMainReader.GetString("KEIYAKU_DENWA_O"), True)
                                Select Case oraMainReader.GetString("SINKYU_KBN_O")
                                    Case "0" : .OutputCsvData("�i������", True)
                                    Case "1" : .OutputCsvData("�i�����Ȃ�", True)
                                End Select
                                .OutputCsvData(oraMainReader.GetString("KEIYAKU_NJYU_O"), True)
                                .OutputCsvData(oraMainReader.GetString("HIMOKU_ID_O"), True)

                                .OutputCsvData(oraHimoReader.GetString("HIMOKU_NAME" & i.ToString("00") & "_O"))
                                Dim bHimokuHyoujiFlg As Boolean = False
                                If oraHimoReader.GetString("HIMOKU_NAME" & i.ToString("00") & "_O") = String.Empty Then
                                    bHimokuHyoujiFlg = False
                                Else
                                    bHimokuHyoujiFlg = True
                                End If

                                While oraHimoReader.EOF = False
                                    If bHimokuHyoujiFlg = True Then
                                        .OutputCsvData(oraHimoReader.GetString("KINGAKU" & i.ToString("00") & "_O"))
                                    Else
                                        .OutputCsvData("")
                                    End If
                                    oraHimoReader.NextRead()
                                End While

                                If bHimokuHyoujiFlg = True Then
                                    .OutputCsvData("1", True, True)
                                Else
                                    .OutputCsvData("0", True, True)
                                End If

                            End With

                            oraHimoReader.Close()

                        End If
                    Next

                    oraMainReader.NextRead()
                End While

            Else
                Select Case PntSyoriKbn
                    Case "0"
                        MessageBox.Show(String.Format(MSG0231W, "���k�}�X�^�����e(�o�^)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", "����ΏۂȂ�")
                    Case "2"
                        MessageBox.Show(String.Format(MSG0231W, "���k�}�X�^�����e(�폜)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", "����ΏۂȂ�")
                    Case Else

                End Select

                Return False

            End If

            CreateCSV.CloseCsv()
            CreateCSV = Nothing

            Return True

        Catch ex As Exception
            Select Case PntSyoriKbn
                Case "0"
                    MessageBox.Show(String.Format(MSG0231W, "���k�}�X�^�����e(�o�^)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", ex.Message)
                Case "2"
                    MessageBox.Show(String.Format(MSG0231W, "���k�}�X�^�����e(�폜)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", ex.Message)
                Case Else

            End Select
            Return False
        Finally
            If Not oraMainReader Is Nothing Then
                oraMainReader.Close()
                oraMainReader = Nothing
            End If

            If Not oraHimoReader Is Nothing Then
                oraHimoReader.Close()
                oraHimoReader = Nothing
            End If
        End Try

        'Dim i As Integer = 0
        'Dim REC As OracleDataReader = Nothing
        'Dim REC2 As OracleDataReader = Nothing

        'Try
        '    '2010/10/18 �o�^�A�폜�N���X�̔���
        '    'Dim CreateCSV As New KFGP106
        '    Dim CreateCSV As Object = Nothing
        '    Select Case PntSyoriKbn
        '        Case "0" : CreateCSV = New KFGP106
        '        Case "2" : CreateCSV = New KFGP108
        '        Case Else
        '    End Select
        '    Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
        '    Dim NowTime As String = Format(DateTime.Now, "HHmmss")
        '    Dim SQL As New StringBuilder(128)

        '    '���k�}�X�^����SEIKYU01_O�`SEIKYU15_O�AKINGAKU01_O�`KINGAKU15_O���擾
        '    Dim SeitoSEIKYU(12, 15) As String
        '    Dim SeitoKINGAKU(12, 15) As String
        '    SQL.Append("SELECT * ")
        '    SQL.Append(" FROM SEITOMAST")
        '    SQL.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
        '    SQL.Append(" AND SEITOMAST.NENDO_O = " & SQ(txtNENDO.Text))
        '    SQL.Append(" AND SEITOMAST.TUUBAN_O = " & SQ(txtTUUBAN.Text))
        '    SQL.Append(" ORDER BY SEITOMAST.TUKI_NO_O asc")

        '    'SQL���s
        '    If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
        '        Do
        '            For j As Integer = 0 To 14
        '                SeitoSEIKYU(i, j) = GCom.NzStr(REC.Item("SEIKYU" & (j + 1).ToString("00") & "_O"))
        '                SeitoKINGAKU(i, j) = GCom.NzStr(REC.Item("KINGAKU" & (j + 1).ToString("00") & "_O"))
        '            Next
        '            i = i + 1
        '        Loop Until REC.Read = False
        '    End If

        '    'CSV�쐬�pSQL���쐬
        '    Dim StData(12, 56) As String       '������J�X�^�}�C�Y�����ӁI
        '    Dim SQL2 As New StringBuilder(128)

        '    SQL2.Append("SELECT DISTINCT")
        '    SQL2.Append(" TENMAST_G.SIT_NNAME_N AS G_SIT_NNAME")   '�Ƃ�܂ƂߓX��
        '    SQL2.Append(",SEITOMAST.GAKKOU_CODE_O")      '�w�Z�R�[�h
        '    SQL2.Append(",GAKMAST1.GAKKOU_NNAME_G")      '�w�Z��
        '    SQL2.Append(",SEITOMAST.NENDO_O")            '���w�N�x
        '    SQL2.Append(",SEITOMAST.GAKUNEN_CODE_O")     '�w�N
        '    SQL2.Append(",SEITOMAST.TUUBAN_O")           '�ʔ�
        '    SQL2.Append(",SEITOMAST.SEITO_NO_O")         '���k�ԍ�
        '    SQL2.Append(",SEITOMAST.SEITO_KNAME_O")      '���k�����J�i
        '    SQL2.Append(",SEITOMAST.SEITO_NNAME_O")      '���k��������
        '    SQL2.Append(",SEITOMAST.MEIGI_KNAME_O")      '�������`�J�i
        '    SQL2.Append(",SEITOMAST.MEIGI_NNAME_O")      '�������`����
        '    SQL2.Append(",SEITOMAST.TKIN_NO_O")          '�戵���Z�@�փR�[�h
        '    SQL2.Append(",TENMAST_S.KIN_NNAME_N")        '�戵���Z�@�֖�
        '    SQL2.Append(",SEITOMAST.TSIT_NO_O")          '�戵�x�X�R�[�h
        '    SQL2.Append(",TENMAST_S.SIT_NNAME_N AS S_SIT_NNAME")          '�戵�x�X��
        '    SQL2.Append(",SEITOMAST.KAMOKU_O")           '�Ȗځ�
        '    SQL2.Append(",SEITOMAST.KOUZA_O")            '�����ԍ�
        '    SQL2.Append(",SEITOMAST.CLASS_CODE_O")       '�N���X
        '    SQL2.Append(",SEITOMAST.SEIBETU_O")          '���ʁ�
        '    SQL2.Append(",SEITOMAST.FURIKAE_O")          '�U�֕��@��
        '    SQL2.Append(",SEITOMAST.KAIYAKU_FLG_O")      '���敪��
        '    SQL2.Append(",SEITOMAST.KEIYAKU_DENWA_O")    '�d�b�ԍ�
        '    SQL2.Append(",SEITOMAST.SINKYU_KBN_O")       '�i���敪��
        '    SQL2.Append(",SEITOMAST.KEIYAKU_NJYU_O")     '�Z��
        '    SQL2.Append(",SEITOMAST.HIMOKU_ID_O")        '���ID
        '    For i = 1 To 15
        '        SQL2.Append(",HIMOMAST.HIMOKU_NAME" & i.ToString("00") & "_H")  '��ږ�
        '    Next
        '    For i = 1 To 15
        '        SQL2.Append(",HIMOMAST.HIMOKU_KINGAKU" & i.ToString("00") & "_H")   '���z
        '    Next

        '    SQL2.Append(",HIMOMAST.TUKI_NO_H")

        '    SQL2.Append(" FROM")
        '    SQL2.Append(" SEITOMAST")
        '    SQL2.Append(" INNER JOIN GAKMAST1")
        '    SQL2.Append(" ON SEITOMAST.GAKKOU_CODE_O = GAKMAST1.GAKKOU_CODE_G")
        '    SQL2.Append(" AND SEITOMAST.GAKUNEN_CODE_O = GAKMAST1.GAKUNEN_CODE_G")

        '    SQL2.Append(" INNER JOIN GAKMAST2")
        '    SQL2.Append(" ON GAKMAST1.GAKKOU_CODE_G = GAKMAST2.GAKKOU_CODE_T")
        '    SQL2.Append(" INNER JOIN TENMAST TENMAST_G")
        '    SQL2.Append(" ON GAKMAST2.TKIN_NO_T = TENMAST_G.KIN_NO_N")
        '    SQL2.Append(" AND GAKMAST2.TSIT_NO_T = TENMAST_G.SIT_NO_N")

        '    SQL2.Append(" INNER JOIN TENMAST TENMAST_S")
        '    SQL2.Append(" ON SEITOMAST.TKIN_NO_O = TENMAST_S.KIN_NO_N")
        '    SQL2.Append(" AND SEITOMAST.TSIT_NO_O = TENMAST_S.SIT_NO_N")

        '    SQL2.Append(" INNER JOIN HIMOMAST")
        '    SQL2.Append(" ON SEITOMAST.GAKKOU_CODE_O = HIMOMAST.GAKKOU_CODE_H")
        '    SQL2.Append(" AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H")
        '    SQL2.Append(" AND SEITOMAST.HIMOKU_ID_O = HIMOMAST.HIMOKU_ID_H")
        '    '2011/06/16 �W���ŏC�� �������x���� ------------------START
        '    SQL2.Append(" AND SEITOMAST.TUKI_NO_O = HIMOMAST.TUKI_NO_H")
        '    '2011/06/16 �W���ŏC�� �������x���� ------------------END

        '    SQL2.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
        '    SQL2.Append(" AND SEITOMAST.NENDO_O = " & SQ(txtNENDO.Text))
        '    SQL2.Append(" AND SEITOMAST.TUUBAN_O = " & SQ(txtTUUBAN.Text))

        '    SQL2.Append(" ORDER BY HIMOMAST.TUKI_NO_H ASC")


        '    'SQL���s
        '    i = 0
        '    If GCom.SetDynaset(SQL2.ToString, REC2) AndAlso REC2.Read Then
        '        Do
        '            StData(i, 0) = GCom.NzStr(REC2.Item("G_SIT_NNAME"))             '�Ƃ�܂ƂߓX��
        '            StData(i, 1) = GCom.NzStr(REC2.Item("GAKKOU_CODE_O"))           '�w�Z�R�[�h
        '            StData(i, 2) = GCom.NzStr(REC2.Item("GAKKOU_NNAME_G"))          '�w�Z��
        '            StData(i, 3) = GCom.NzStr(REC2.Item("NENDO_O"))                 '���w�N�x
        '            StData(i, 4) = GCom.NzStr(REC2.Item("GAKUNEN_CODE_O"))          '�w�N
        '            StData(i, 5) = GCom.NzStr(REC2.Item("TUUBAN_O"))                '�ʔ�
        '            StData(i, 6) = GCom.NzStr(REC2.Item("SEITO_NO_O"))              '���k�ԍ�
        '            StData(i, 7) = GCom.NzStr(REC2.Item("SEITO_KNAME_O"))           '���k�����J�i
        '            StData(i, 8) = GCom.NzStr(REC2.Item("SEITO_NNAME_O")).Trim      '���k��������
        '            StData(i, 9) = GCom.NzStr(REC2.Item("MEIGI_KNAME_O"))           '�������`�J�i
        '            StData(i, 10) = GCom.NzStr(REC2.Item("MEIGI_NNAME_O")).Trim     '�������`����
        '            StData(i, 11) = GCom.NzStr(REC2.Item("TKIN_NO_O"))              '�戵���Z�@�փR�[�h
        '            StData(i, 12) = GCom.NzStr(REC2.Item("KIN_NNAME_N"))            '�戵���Z�@�֖�
        '            StData(i, 13) = GCom.NzStr(REC2.Item("TSIT_NO_O"))              '�戵�x�X�R�[�h
        '            StData(i, 14) = GCom.NzStr(REC2.Item("S_SIT_NNAME"))            '�戵�x�X��
        '            StData(i, 15) = GCom.NzStr(REC2.Item("KAMOKU_O"))               '�Ȗ�
        '            StData(i, 16) = GCom.NzStr(REC2.Item("KOUZA_O"))                '�����ԍ�
        '            StData(i, 17) = GCom.NzStr(REC2.Item("CLASS_CODE_O"))           '�N���X
        '            StData(i, 18) = GCom.NzStr(REC2.Item("SEIBETU_O"))              '����
        '            StData(i, 19) = GCom.NzStr(REC2.Item("FURIKAE_O"))              '�U�֋敪
        '            StData(i, 20) = GCom.NzStr(REC2.Item("KAIYAKU_FLG_O"))          '���敪
        '            StData(i, 21) = GCom.NzStr(REC2.Item("KEIYAKU_DENWA_O")).Trim   '�d�b�ԍ�
        '            StData(i, 22) = GCom.NzStr(REC2.Item("SINKYU_KBN_O"))           '�i���敪
        '            StData(i, 23) = GCom.NzStr(REC2.Item("KEIYAKU_NJYU_O")).Trim    '�Z��
        '            StData(i, 24) = GCom.NzStr(REC2.Item("HIMOKU_ID_O"))            '���ID
        '            For j As Integer = 25 To 39
        '                StData(i, j) = GCom.NzStr(REC2.Item("HIMOKU_NAME" & (j - 24).ToString("00") & "_H")).Trim
        '            Next
        '            For j As Integer = 40 To 54
        '                StData(i, j) = GCom.NzStr(REC2.Item("HIMOKU_KINGAKU" & (j - 39).ToString("00") & "_H"))
        '            Next
        '            StData(i, 55) = GCom.NzStr(REC2.Item("TUKI_NO_H"))

        '            i = i + 1
        '        Loop Until REC2.Read = False
        '    End If

        '    'CSV�쐬
        '    strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

        '    '2010/10/18 �������P �e�L�X�g��Ǎ���Ń��X�g�� ��������
        '    Dim KAMOKU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_KAMOKU_TXT)
        '    Dim SEIBETU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_SEIBETU_TXT)
        '    Dim FURIKAE_LIST As New MyStringDictionary(STR_TXT_PATH & STR_FURIKAE_TXT)
        '    Dim KAIYAKU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_KAIYAKU_TXT)
        '    Dim SINKYU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_SINKYU_TXT)
        '    '2010/10/18 �������P �e�L�X�g��Ǎ���Ń��X�g�� �����܂�

        '    ' CSV�쐬���@
        '    ' ��LSQL����12���R�[�h�i���P�ʁj�ł������ڂ��擾�ł��Ȃ����߁ACSV�p�Ƀf�[�^�����H����
        '    ' �iDB����̎擾��12���R�[�h�ɑ΂��A���[��CSV��15���ڂ̂��߃f�[�^�����H����K�v������j
        '    ' ���k�}�X�^�̐������@��"��"�̏ꍇ�A���z�͐��k�}�X�^�Ɋi�[����邽�߁A���k�}�X�^�̋��z���Q��

        '    '��ڐ������[�v����
        '    For i = 0 To 14
        '        With CreateCSV
        '            .OutputCsvData(PntSyoriKbn, True)                           '�^�C�g���i�o�^or�폜�j
        '            .OutputCsvData(Today, True)                                 '�V�X�e�����t
        '            .OutputCsvData(NowTime, True)                               '�^�C���X�^���v
        '            .OutputCsvData(StData(0, 0), True)                          '�Ƃ�܂ƂߓX��
        '            .OutputCsvData(StData(0, 1), True)                          '�w�Z�R�[�h
        '            .OutputCsvData(StData(0, 2), True)                          '�w�Z��
        '            .OutputCsvData(StData(0, 3), True)                          '���w�N�x
        '            .OutputCsvData(StData(0, 4), True)                          '�w�N
        '            .OutputCsvData(StData(0, 5), True)                          '�ʔ�
        '            .OutputCsvData(StData(0, 6), True)                          '���k�ԍ�
        '            .OutputCsvData(StData(0, 7), True)                          '���k�����J�i
        '            .OutputCsvData(StData(0, 8), True)                          '���k��������
        '            .OutputCsvData(StData(0, 9), True)                          '�������`�J�i
        '            .OutputCsvData(StData(0, 10), True)                         '�������`����
        '            .OutputCsvData(StData(0, 11), True)                         '�戵���Z�@�փR�[�h
        '            .OutputCsvData(StData(0, 12), True)                         '�戵���Z�@�֖�
        '            .OutputCsvData(StData(0, 13), True)                         '�戵�x�X�R�[�h
        '            .OutputCsvData(StData(0, 14), True)                         '�戵�x�X��
        '            '2010/10/18 ���X�g����l���擾����
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_KAMOKU_TXT, _
        '            '                                 StData(0, 15)), True)      '�Ȗ�
        '            .OutputCsvData(KAMOKU_LIST.Item(StData(0, 15)), True)       '�Ȗ�
        '            .OutputCsvData(StData(0, 16), True)                         '�����ԍ�
        '            .OutputCsvData(StData(0, 17), True)                         '�N���X
        '            '2010/10/18 ���X�g����l���擾����
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_SEIBETU_TXT, _
        '            '                                  StData(0, 18)), True)     '����
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_FURIKAE_TXT, _
        '            '                                  StData(0, 19)), True)     '�U�֕��@
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_KAIYAKU_TXT, _
        '            '                                  StData(0, 20)), True)     '���敪
        '            .OutputCsvData(SEIBETU_LIST.Item(StData(0, 18)), True)      '����
        '            .OutputCsvData(FURIKAE_LIST.Item(StData(0, 19)), True)      '�U�֕��@
        '            .OutputCsvData(KAIYAKU_LIST(StData(0, 20)), True)           '���敪
        '            .OutputCsvData(StData(0, 21), True)                         '�d�b�ԍ�
        '            '2010/10/18 ���X�g����l���擾����
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_SINKYU_TXT, _
        '            '                                  StData(0, 22)), True)     '�i���敪
        '            .OutputCsvData(SINKYU_LIST(StData(0, 22)), True)            '�i���敪
        '            .OutputCsvData(StData(0, 23), True)                         '�Z��
        '            .OutputCsvData(StData(0, 24), True)                         '���ID
        '            .OutputCsvData(StData(0, i + 25), True)                     '��ږ�
        '            For j As Integer = 0 To 11
        '                ' ���z�N���A
        '                ' ��ږ������͂���Ă��Ȃ����R�[�h�̏ꍇ�A���z��0���󔒂ɕύX
        '                If Trim(StData(0, i + 25)) <> "" Then
        '                    '��r����
        '                    '���k�}�X�^�̐������@��"��"�̏ꍇ�A���k�}�X�^�̋��z���o��
        '                    If SeitoSEIKYU(j, i) = "1" Then
        '                        .OutputCsvData(SeitoKINGAKU(j, i), True)
        '                    Else
        '                        .OutputCsvData(StData(j, i + 15 + 25), True)
        '                    End If

        '                Else
        '                    .OutputCsvData("", True)
        '                End If
        '            Next
        '            '�\���敪
        '            If Trim(StData(0, i + 25)) <> "" Then
        '                .OutputCsvData("1", True)
        '            Else
        '                .OutputCsvData("0", True)
        '            End If

        '            .OutputCsvData("", False, True)                             '���s
        '        End With
        '    Next

        '    CreateCSV.CloseCsv()
        '    CreateCSV = Nothing

        '    Return True

        'Catch ex As Exception
        '    Select Case PntSyoriKbn
        '        Case "0"
        '            MessageBox.Show(String.Format(MSG0231W, "���k�}�X�^�����e(�o�^)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", ex.Message)
        '        Case "2"
        '            MessageBox.Show(String.Format(MSG0231W, "���k�}�X�^�����e(�폜)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", ex.Message)
        '        Case Else

        '    End Select
        '    Return False
        'Finally
        '    If Not REC Is Nothing Then
        '        REC.Close()
        '        REC.Dispose()
        '    End If
        '    If Not REC2 Is Nothing Then
        '        REC2.Close()
        '        REC2.Dispose()
        '    End If
        '    Me.ResumeLayout()
        'End Try
        '2017/04/05 saitou ���t�M��(RSV2�W��) UPD ----------------------------------------------------------------- END
    End Function

    '============================================================================
    'NAME           :fn_CreateCSV_UPD
    'Parameter      :PntSyoriKbn ��������敪
    'Description    :KFGP107(���k�}�X�^�����e(�X�V))����p�b�r�u�t�@�C���쐬
    'Return         :
    'Create         :2010/09/16
    'Update         :2017/04/05 saitou ���t�M��(RSV2�W��) deleted for ���k�}�X�^�����e���[���P
    '============================================================================
    'Private Function fn_CreateCSV_UPD(ByVal beforeData(,) As String _
    '                                  , ByVal afterData(,) As String) As Boolean
    '    Try
    '        Dim CreateCSV As New KFGP107
    '        Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
    '        Dim NowTime As String = Format(DateTime.Now, "HHmmss")

    '        If beforeData.GetLength(0) <> afterData.GetLength(0) Then
    '            Return False
    '        End If
    '        If beforeData.GetLength(1) <> afterData.GetLength(1) Then
    '            Return False
    '        End If

    '        Dim bUpdFlg As Boolean = False      '�X�V�t���O

    '        '���X�g�w�b�_���擾
    '        Dim strGakCode As String = afterData(0, 0)
    '        Dim strNendo As String = afterData(0, 1)
    '        Dim strTuuban As String = afterData(0, 2)
    '        Dim strGakunen As String = afterData(0, 3)
    '        Dim strClass As String = afterData(0, 4)
    '        Dim strSeitoNo As String = afterData(0, 5)
    '        Dim strGakName As String = lab�w�Z��.Text

    '        '�I���N�����ږ��擾
    '        Dim oraColumnJapanName() As String = Nothing
    '        Dim oraColumnName() As String = Nothing
    '        fn_GetSEITOMAST_ColumnName(oraColumnName, oraColumnJapanName)

    '        'CSV�쐬
    '        strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

    '        ' �X�V����Ă���f�[�^������
    '        ' �������@
    '        ' �@�X�V�O�ƍX�V��̃f�[�^���ׂ�
    '        ' �ATUKI_NO_O�̉e���ŁA1�ӏ��X�V��12�ӏ��X�V�����̂ŁA��ڊ֘A�ȊO��1���R�[�h�ڂ��ׂ�
    '        ' �B��L���@�ő��Ⴊ�������ꍇ�ACSV�f�[�^�쐬
    '        ' ���X�V���͏o�͂��Ȃ�
    '        ' ���X�V���ڂ����݂��Ȃ��ꍇ�A���[��������Ȃ�

    '        ' ������ ����_���������ꍇ�̂b�r�u�쐬�C���[�W ��������������������������������������������������������������������������������������������������������
    '        ' �w�Z�R�[�h�@���w�N�x�@�c�@���1�̐������@�@���1�̐������z�@�c�@���15�̐������@�@���15�̐������z�@�c�@�X�V���@�c�@�\��5
    '        '     ��         ��     �c        ��               ��         �c         ��                ��               �~    �c    ��          (�z��1���R�[�h)
    '        '     �~         �~     �c        ��               ��         �c         ��                ��               �~    �c    �~          (�z��2���R�[�h)
    '        '     �E         �E     �@        �E               �E         �@         �E                �E               �E    �@    �E
    '        '     �E         �E     �@        �E               �E         �@         �E                �E               �E    �@    �E
    '        '     �~         �~     �c        ��               ��         �c         ��                ��               �~    �c    �~          (�z��ŏI���R�[�h)
    '        ' ������������������������������������������������������������������������������������������������������������������������������������������������������

    '        For i As Integer = 0 To beforeData.GetLength(0) - 1
    '            For j As Integer = 0 To beforeData.GetLength(1) - 1
    '                If beforeData(i, j) <> afterData(i, j) Then
    '                    If i = 0 Then
    '                        '1���R�[�h�ڂ̑���͖������ŏ������݁i�X�V���������j
    '                        If j = 59 Then
    '                        Else
    '                            'CSV�f�[�^�쐬
    '                            With CreateCSV
    '                                .OutputCsvData(Today, True)                         '������
    '                                .OutputCsvData(NowTime, True)                       '�^�C���X�^���v
    '                                .OutputCsvData(GCom.GetUserID, True)                '���O�C����
    '                                .OutputCsvData(Environment.MachineName, True)       '�[����
    '                                .OutputCsvData(strGakCode, True)                    '�w�Z�R�[�h
    '                                .OutputCsvData(strNendo, True)                      '�N�x
    '                                .OutputCsvData(strTuuban, True)                     '�ʔ�
    '                                .OutputCsvData(strGakunen, True)                    '�w�N
    '                                .OutputCsvData(strClass, True)                      '�N���X
    '                                .OutputCsvData(strSeitoNo, True)                    '���k�ԍ�
    '                                .OutputCsvData(strGakName, True)                    '�w�Z��
    '                                .OutputCsvData(oraColumnJapanName(j), True)         '���{��\����
    '                                .OutputCsvData(oraColumnName(j), True)              'ORACLE���ږ�
    '                                .OutputCsvData(beforeData(i, j), True)              '�ύX�O
    '                                .OutputCsvData(afterData(i, j), True, True)         '�ύX��
    '                            End With
    '                        End If

    '                        '�X�V�t���O�X�V
    '                        bUpdFlg = True

    '                    Else
    '                        '1���R�[�h�ڂłȂ��ꍇ
    '                        Select Case j
    '                            Case 28 To 57
    '                                '��ڊ֘A�̏�����������
    '                                'CSV�f�[�^�쐬
    '                                With CreateCSV
    '                                    .OutputCsvData(Today, True)                         '������
    '                                    .OutputCsvData(NowTime, True)                       '�^�C���X�^���v
    '                                    .OutputCsvData(GCom.GetUserID, True)                '���O�C����
    '                                    .OutputCsvData(Environment.MachineName, True)       '�[����
    '                                    .OutputCsvData(strGakCode, True)                    '�w�Z�R�[�h
    '                                    .OutputCsvData(strNendo, True)                      '�N�x
    '                                    .OutputCsvData(strTuuban, True)                     '�ʔ�
    '                                    .OutputCsvData(strGakunen, True)                    '�w�N
    '                                    .OutputCsvData(strClass, True)                      '�N���X
    '                                    .OutputCsvData(strSeitoNo, True)                    '���k�ԍ�
    '                                    .OutputCsvData(strGakName, True)                    '�w�Z��
    '                                    .OutputCsvData(oraColumnJapanName(j), True)         '���{��\����
    '                                    .OutputCsvData(oraColumnName(j), True)              'ORACLE���ږ�
    '                                    .OutputCsvData(beforeData(i, j), True)              '�ύX�O
    '                                    .OutputCsvData(afterData(i, j), True, True)         '�ύX��
    '                                End With
    '                            Case Else
    '                                '��ڊ֘A�ȊO�͓ǂݔ�΂�
    '                        End Select
    '                    End If
    '                End If
    '            Next
    '        Next

    '        CreateCSV.CloseCsv()
    '        CreateCSV = Nothing

    '        Return bUpdFlg

    '    Catch ex As Exception
    '        MessageBox.Show(String.Format(MSG0231W, "���k�}�X�^�����e(�X�V)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����f�[�^�쐬", "���s", ex.Message)
    '        Return False
    '    Finally

    '    End Try
    'End Function

    '============================================================================
    'NAME           :fn_GetSEITOMAST_PrnCSVUpd
    'Parameter      :seitoData(,) ���k�}�X�^�̃��R�[�h
    'Description    :���k�}�X�^�̃��R�[�h���擾����i�����k�}�X�^�����e�i�X�V�j�p�j
    'Return         :
    'Create         :2010/09/16
    'Update         :2017/04/05 saitou ���t�M��(RSV2�W��) deleted for ���k�}�X�^�����e���[���P
    '============================================================================
    'Private Function fn_GetSEITOMAST_PrnCSVUpd(ByRef seitoData(,) As String) As Boolean
    '    Dim REC As OracleDataReader = Nothing
    '    Try
    '        Dim SQL As New StringBuilder(128)
    '        Dim RecordIndex As Integer = 0
    '        Dim RecordCount As Integer = 0
    '        SQL.Append("SELECT * ")
    '        SQL.Append(" FROM SEITOMAST")
    '        SQL.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
    '        SQL.Append(" AND SEITOMAST.NENDO_O = " & SQ(txtNENDO.Text))
    '        SQL.Append(" AND SEITOMAST.TUUBAN_O = " & SQ(txtTUUBAN.Text))
    '        SQL.Append(" ORDER BY SEITOMAST.TUKI_NO_O ASC")

    '        ' ���ڐ��擾�̌����i�J�E���g�̂݁j
    '        If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
    '            Do
    '                RecordCount = RecordCount + 1
    '            Loop Until REC.Read = False
    '        End If

    '        ' �z��̗v�f�����擾
    '        ReDim seitoData(RecordCount, SeitoMastMaxColumns)

    '        ' ���ڎ擾�̂��߂ɂ�����x����
    '        If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
    '            Do
    '                seitoData(RecordIndex, 0) = GCom.NzStr(REC.Item("GAKKOU_CODE_O"))
    '                seitoData(RecordIndex, 1) = GCom.NzStr(REC.Item("NENDO_O"))
    '                seitoData(RecordIndex, 2) = GCom.NzStr(REC.Item("TUUBAN_O"))
    '                seitoData(RecordIndex, 3) = GCom.NzStr(REC.Item("GAKUNEN_CODE_O"))
    '                seitoData(RecordIndex, 4) = GCom.NzStr(REC.Item("CLASS_CODE_O"))
    '                seitoData(RecordIndex, 5) = GCom.NzStr(REC.Item("SEITO_NO_O"))
    '                seitoData(RecordIndex, 6) = GCom.NzStr(REC.Item("SEITO_KNAME_O"))
    '                seitoData(RecordIndex, 7) = GCom.NzStr(REC.Item("SEITO_NNAME_O"))
    '                seitoData(RecordIndex, 8) = GCom.NzStr(REC.Item("SEIBETU_O"))
    '                seitoData(RecordIndex, 9) = GCom.NzStr(REC.Item("TKIN_NO_O"))
    '                seitoData(RecordIndex, 10) = GCom.NzStr(REC.Item("TSIT_NO_O"))
    '                seitoData(RecordIndex, 11) = GCom.NzStr(REC.Item("KAMOKU_O"))
    '                seitoData(RecordIndex, 12) = GCom.NzStr(REC.Item("KOUZA_O"))
    '                seitoData(RecordIndex, 13) = GCom.NzStr(REC.Item("MEIGI_KNAME_O"))
    '                seitoData(RecordIndex, 14) = GCom.NzStr(REC.Item("MEIGI_NNAME_O"))
    '                seitoData(RecordIndex, 15) = GCom.NzStr(REC.Item("FURIKAE_O"))
    '                seitoData(RecordIndex, 16) = GCom.NzStr(REC.Item("KEIYAKU_NJYU_O"))
    '                seitoData(RecordIndex, 17) = GCom.NzStr(REC.Item("KEIYAKU_DENWA_O"))
    '                seitoData(RecordIndex, 18) = GCom.NzStr(REC.Item("KAIYAKU_FLG_O"))
    '                seitoData(RecordIndex, 19) = GCom.NzStr(REC.Item("SINKYU_KBN_O"))
    '                seitoData(RecordIndex, 20) = GCom.NzStr(REC.Item("HIMOKU_ID_O"))
    '                seitoData(RecordIndex, 21) = GCom.NzStr(REC.Item("TYOUSI_FLG_O"))
    '                seitoData(RecordIndex, 22) = GCom.NzStr(REC.Item("TYOUSI_NENDO_O"))
    '                seitoData(RecordIndex, 23) = GCom.NzStr(REC.Item("TYOUSI_TUUBAN_O"))
    '                seitoData(RecordIndex, 24) = GCom.NzStr(REC.Item("TYOUSI_GAKUNEN_O"))
    '                seitoData(RecordIndex, 25) = GCom.NzStr(REC.Item("TYOUSI_CLASS_O"))
    '                seitoData(RecordIndex, 26) = GCom.NzStr(REC.Item("TYOUSI_SEITONO_O"))
    '                seitoData(RecordIndex, 27) = GCom.NzStr(REC.Item("TUKI_NO_O"))
    '                seitoData(RecordIndex, 28) = GCom.NzStr(REC.Item("SEIKYU01_O"))
    '                seitoData(RecordIndex, 29) = GCom.NzStr(REC.Item("KINGAKU01_O"))
    '                seitoData(RecordIndex, 30) = GCom.NzStr(REC.Item("SEIKYU02_O"))
    '                seitoData(RecordIndex, 31) = GCom.NzStr(REC.Item("KINGAKU02_O"))
    '                seitoData(RecordIndex, 32) = GCom.NzStr(REC.Item("SEIKYU03_O"))
    '                seitoData(RecordIndex, 33) = GCom.NzStr(REC.Item("KINGAKU03_O"))
    '                seitoData(RecordIndex, 34) = GCom.NzStr(REC.Item("SEIKYU04_O"))
    '                seitoData(RecordIndex, 35) = GCom.NzStr(REC.Item("KINGAKU04_O"))
    '                seitoData(RecordIndex, 36) = GCom.NzStr(REC.Item("SEIKYU05_O"))
    '                seitoData(RecordIndex, 37) = GCom.NzStr(REC.Item("KINGAKU05_O"))
    '                seitoData(RecordIndex, 38) = GCom.NzStr(REC.Item("SEIKYU06_O"))
    '                seitoData(RecordIndex, 39) = GCom.NzStr(REC.Item("KINGAKU06_O"))
    '                seitoData(RecordIndex, 40) = GCom.NzStr(REC.Item("SEIKYU07_O"))
    '                seitoData(RecordIndex, 41) = GCom.NzStr(REC.Item("KINGAKU07_O"))
    '                seitoData(RecordIndex, 42) = GCom.NzStr(REC.Item("SEIKYU08_O"))
    '                seitoData(RecordIndex, 43) = GCom.NzStr(REC.Item("KINGAKU08_O"))
    '                seitoData(RecordIndex, 44) = GCom.NzStr(REC.Item("SEIKYU09_O"))
    '                seitoData(RecordIndex, 45) = GCom.NzStr(REC.Item("KINGAKU09_O"))
    '                seitoData(RecordIndex, 46) = GCom.NzStr(REC.Item("SEIKYU10_O"))
    '                seitoData(RecordIndex, 47) = GCom.NzStr(REC.Item("KINGAKU10_O"))
    '                seitoData(RecordIndex, 48) = GCom.NzStr(REC.Item("SEIKYU11_O"))
    '                seitoData(RecordIndex, 49) = GCom.NzStr(REC.Item("KINGAKU11_O"))
    '                seitoData(RecordIndex, 50) = GCom.NzStr(REC.Item("SEIKYU12_O"))
    '                seitoData(RecordIndex, 51) = GCom.NzStr(REC.Item("KINGAKU12_O"))
    '                seitoData(RecordIndex, 52) = GCom.NzStr(REC.Item("SEIKYU13_O"))
    '                seitoData(RecordIndex, 53) = GCom.NzStr(REC.Item("KINGAKU13_O"))
    '                seitoData(RecordIndex, 54) = GCom.NzStr(REC.Item("SEIKYU14_O"))
    '                seitoData(RecordIndex, 55) = GCom.NzStr(REC.Item("KINGAKU14_O"))
    '                seitoData(RecordIndex, 56) = GCom.NzStr(REC.Item("SEIKYU15_O"))
    '                seitoData(RecordIndex, 57) = GCom.NzStr(REC.Item("KINGAKU15_O"))
    '                seitoData(RecordIndex, 58) = GCom.NzStr(REC.Item("SAKUSEI_DATE_O"))
    '                seitoData(RecordIndex, 59) = GCom.NzStr(REC.Item("KOUSIN_DATE_O"))
    '                seitoData(RecordIndex, 60) = GCom.NzStr(REC.Item("YOBI1_O"))
    '                seitoData(RecordIndex, 61) = GCom.NzStr(REC.Item("YOBI2_O"))
    '                seitoData(RecordIndex, 62) = GCom.NzStr(REC.Item("YOBI3_O"))
    '                seitoData(RecordIndex, 63) = GCom.NzStr(REC.Item("YOBI4_O"))
    '                seitoData(RecordIndex, 64) = GCom.NzStr(REC.Item("YOBI5_O"))

    '                RecordIndex = RecordIndex + 1

    '            Loop Until REC.Read = False
    '        End If

    '        Return True
    '    Catch ex As Exception
    '        Return False
    '    Finally
    '        If Not REC Is Nothing Then
    '            REC.Close()
    '            REC.Dispose()
    '        End If
    '        Me.ResumeLayout()
    '    End Try
    'End Function

    '============================================================================
    'NAME           :fn_GetSEITOMAST_ColumnName
    'Parameter      :ColumnNameE() ���k�}�X�^�̍��ږ��i�p���j
    '               :ColumnNameJ() ���k�}�X�^�̍��ږ��i���{���j
    'Description    :���k�}�X�^�̗񖼂�ݒ�
    'Return         :
    'Create         :2010/09/16
    'Update         :2017/04/05 saitou ���t�M��(RSV2�W��) deleted for ���k�}�X�^�����e���[���P
    '============================================================================
    'Private Sub fn_GetSEITOMAST_ColumnName(ByRef ColumnNameE() As String, ByRef ColumnNameJ() As String)
    '    ReDim ColumnNameE(SeitoMastMaxColumns)
    '    ReDim ColumnNameJ(SeitoMastMaxColumns)

    '    ColumnNameE(0) = "GAKKOU_CODE_O"
    '    ColumnNameJ(0) = "�w�Z�R�[�h"
    '    ColumnNameE(1) = "NENDO_O"
    '    ColumnNameJ(1) = "���w�N�x"
    '    ColumnNameE(2) = "TUUBAN_O"
    '    ColumnNameJ(2) = "�ʔ�"
    '    ColumnNameE(3) = "GAKUNEN_CODE_O"
    '    ColumnNameJ(3) = "�w�N�R�[�h"
    '    ColumnNameE(4) = "CLASS_CODE_O"
    '    ColumnNameJ(4) = "�N���X�R�[�h"
    '    ColumnNameE(5) = "SEITO_NO_O"
    '    ColumnNameJ(5) = "���k�ԍ�"
    '    ColumnNameE(6) = "SEITO_KNAME_O"
    '    ColumnNameJ(6) = "���k�����i�J�i�j"
    '    ColumnNameE(7) = "SEITO_NNAME_O"
    '    ColumnNameJ(7) = "���k�����i�����j"
    '    ColumnNameE(8) = "SEIBETU_O"
    '    ColumnNameJ(8) = "����"
    '    ColumnNameE(9) = "TKIN_NO_O"
    '    ColumnNameJ(9) = "���Z�@�փR�[�h"
    '    ColumnNameE(10) = "TSIT_NO_O"
    '    ColumnNameJ(10) = "�x�X�R�[�h"
    '    ColumnNameE(11) = "KAMOKU_O"
    '    ColumnNameJ(11) = "�Ȗ�"
    '    ColumnNameE(12) = "KOUZA_O"
    '    ColumnNameJ(12) = "�����ԍ�"
    '    ColumnNameE(13) = "MEIGI_KNAME_O"
    '    ColumnNameJ(13) = "���`�l�i�J�i�j"
    '    ColumnNameE(14) = "MEIGI_NNAME_O"
    '    ColumnNameJ(14) = "���`�l�i�����j"
    '    ColumnNameE(15) = "FURIKAE_O"
    '    ColumnNameJ(15) = "�U�֕��@"
    '    ColumnNameE(16) = "KEIYAKU_NJYU_O"
    '    ColumnNameJ(16) = "�_��Z���i�����j"
    '    ColumnNameE(17) = "KEIYAKU_DENWA_O"
    '    ColumnNameJ(17) = "�_��d�b�ԍ�"
    '    ColumnNameE(18) = "KAIYAKU_FLG_O"
    '    ColumnNameJ(18) = "���敪"
    '    ColumnNameE(19) = "SINKYU_KBN_O"
    '    ColumnNameJ(19) = "�i���敪"
    '    ColumnNameE(20) = "HIMOKU_ID_O"
    '    ColumnNameJ(20) = "���ID"
    '    ColumnNameE(21) = "TYOUSI_FLG_O"
    '    ColumnNameJ(21) = "���q�L���t���O"
    '    ColumnNameE(22) = "TYOUSI_NENDO_O"
    '    ColumnNameJ(22) = "���q���w�N�x"
    '    ColumnNameE(23) = "TYOUSI_TUUBAN_O"
    '    ColumnNameJ(23) = "���q�ʔ�"
    '    ColumnNameE(24) = "TYOUSI_GAKUNEN_O"
    '    ColumnNameJ(24) = "���q�w�N"
    '    ColumnNameE(25) = "TYOUSI_CLASS_O"
    '    ColumnNameJ(25) = "���q�N���X"
    '    ColumnNameE(26) = "TYOUSI_SEITONO_O"
    '    ColumnNameJ(26) = "���q���k�ԍ�"
    '    ColumnNameE(27) = "TUKI_NO_O"
    '    ColumnNameJ(27) = "������"
    '    ColumnNameE(28) = "SEIKYU01_O"
    '    ColumnNameJ(28) = "���1�̐������@"
    '    ColumnNameE(29) = "KINGAKU01_O"
    '    ColumnNameJ(29) = "���1�̐������z"
    '    ColumnNameE(30) = "SEIKYU02_O"
    '    ColumnNameJ(30) = "���2�̐������@"
    '    ColumnNameE(31) = "KINGAKU02_O"
    '    ColumnNameJ(31) = "���2�̐������z"
    '    ColumnNameE(32) = "SEIKYU03_O"
    '    ColumnNameJ(32) = "���3�̐������@"
    '    ColumnNameE(33) = "KINGAKU03_O"
    '    ColumnNameJ(33) = "���3�̐������z"
    '    ColumnNameE(34) = "SEIKYU04_O"
    '    ColumnNameJ(34) = "���4�̐������@"
    '    ColumnNameE(35) = "KINGAKU04_O"
    '    ColumnNameJ(35) = "���4�̐������z"
    '    ColumnNameE(36) = "SEIKYU05_O"
    '    ColumnNameJ(36) = "���5�̐������@"
    '    ColumnNameE(37) = "KINGAKU05_O"
    '    ColumnNameJ(37) = "���5�̐������z"
    '    ColumnNameE(38) = "SEIKYU06_O"
    '    ColumnNameJ(38) = "���6�̐������@"
    '    ColumnNameE(39) = "KINGAKU06_O"
    '    ColumnNameJ(39) = "���6�̐������z"
    '    ColumnNameE(40) = "SEIKYU07_O"
    '    ColumnNameJ(40) = "���7�̐������@"
    '    ColumnNameE(41) = "KINGAKU07_O"
    '    ColumnNameJ(41) = "���7�̐������z"
    '    ColumnNameE(42) = "SEIKYU08_O"
    '    ColumnNameJ(42) = "���8�̐������@"
    '    ColumnNameE(43) = "KINGAKU08_O"
    '    ColumnNameJ(43) = "���8�̐������z"
    '    ColumnNameE(44) = "SEIKYU09_O"
    '    ColumnNameJ(44) = "���9�̐������@"
    '    ColumnNameE(45) = "KINGAKU09_O"
    '    ColumnNameJ(45) = "���9�̐������z"
    '    ColumnNameE(46) = "SEIKYU10_O"
    '    ColumnNameJ(46) = "���10�̐������@"
    '    ColumnNameE(47) = "KINGAKU10_O"
    '    ColumnNameJ(47) = "���10�̐������z"
    '    ColumnNameE(48) = "SEIKYU11_O"
    '    ColumnNameJ(48) = "���11�̐������@"
    '    ColumnNameE(49) = "KINGAKU11_O"
    '    ColumnNameJ(49) = "���11�̐������z"
    '    ColumnNameE(50) = "SEIKYU12_O"
    '    ColumnNameJ(50) = "���12�̐������@"
    '    ColumnNameE(51) = "KINGAKU12_O"
    '    ColumnNameJ(51) = "���12�̐������z"
    '    ColumnNameE(52) = "SEIKYU13_O"
    '    ColumnNameJ(52) = "���13�̐������@"
    '    ColumnNameE(53) = "KINGAKU13_O"
    '    ColumnNameJ(53) = "���13�̐������z"
    '    ColumnNameE(54) = "SEIKYU14_O"
    '    ColumnNameJ(54) = "���14�̐������@"
    '    ColumnNameE(55) = "KINGAKU14_O"
    '    ColumnNameJ(55) = "���14�̐������z"
    '    ColumnNameE(56) = "SEIKYU15_O"
    '    ColumnNameJ(56) = "���15�̐������@"
    '    ColumnNameE(57) = "KINGAKU15_O"
    '    ColumnNameJ(57) = "���15�̐������z"
    '    ColumnNameE(58) = "SAKUSEI_DATE_O"
    '    ColumnNameJ(58) = "�쐬���t"
    '    ColumnNameE(59) = "KOUSIN_DATE_O"
    '    ColumnNameJ(59) = "�X�V���t"
    '    ColumnNameE(60) = "YOBI1_O"
    '    ColumnNameJ(60) = "�\��1"
    '    ColumnNameE(61) = "YOBI2_O"
    '    ColumnNameJ(61) = "�\��2"
    '    ColumnNameE(62) = "YOBI3_O"
    '    ColumnNameJ(62) = "�\��3"
    '    ColumnNameE(63) = "YOBI4_O"
    '    ColumnNameJ(63) = "�\��4"
    '    ColumnNameE(64) = "YOBI5_O"
    '    ColumnNameJ(64) = "�\��5"
    'End Sub



    '============================================================================
    'NAME           :fn_Print
    'Parameter      :PrintNo 0:�o�^ 1:�X�V 2:�폜
    'Description    :���[����p�b�r�u�t�@�C���쐬
    'Return         :
    'Create         :2010/03/16
    'Update         :
    '============================================================================
    Public Function fn_Print(ByVal PrintNo As Integer) As Boolean
        Dim ErrMessage As String = ""
        Dim Param As String = ""
        Dim nRet As Integer = 0
        Dim Syori As String = ""

        Select Case PrintNo
            Case 0
                Syori = "�o�^"
            Case 1
                Syori = "�X�V"
            Case 2
                Syori = "�폜"
        End Select
        Try
            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C�����A�����R�[�h
            Param = GCom.GetUserID & "," & strCSV_FILE_NAME & "," & _
                    LW.ToriCode
            Select Case PrintNo
                Case 0
                    nRet = ExeRepo.ExecReport("KFGP106.EXE", Param)
                Case 1
                    nRet = ExeRepo.ExecReport("KFGP107.EXE", Param)
                Case 2
                    nRet = ExeRepo.ExecReport("KFGP108.EXE", Param)
            End Select

            If nRet <> 0 Then
                '������s�F�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case -1
                        ErrMessage = String.Format(MSG0106W, "���k�}�X�^�����e(" & Syori & ")")
                    Case Else
                        ErrMessage = String.Format(MSG0004E, "���k�}�X�^�����e(" & Syori & ")")
                End Select

                MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
            MessageBox.Show(String.Format(MSG0014I, "���k�}�X�^�����e(" & Syori & ")"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            ErrMessage = String.Format(MSG0004E, "���k�}�X�^�����e(" & Syori & ")")
            MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���k�}�X�^�����e(" & Syori & ")���", "���s")
            Return False
        End Try

    End Function

    ''' <summary>
    ''' ���k�}�X�^�����e�i�X�V�j����p�̃f�[�^�e�[�u�������������܂��B
    ''' </summary>
    ''' <remarks>2017/04/05 saitou ���t�M��(RSV2�W��) added for ���k�}�X�^�����e���[���P</remarks>
    Private Sub CreateSeitomastDataTable()
        Try
            '���k�}�X�^�����e�X�V���[����N���X���琶�k�}�X�^�̃J���������擾����
            Dim KFGP107 As New KFGP107

            Me.dtBeforeSeitomast = New DataTable
            Me.dtAfterSeitomast = New DataTable

            '�J�������擾
            For Each strColumnName As String In KFGP107.TableInfo.ColumnEnglishName
                Me.dtBeforeSeitomast.Columns.Add(strColumnName)
                Me.dtAfterSeitomast.Columns.Add(strColumnName)
            Next

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("���k�}�X�^�X�V�f�[�^�e�[�u���쐬", "���s", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' ���k�}�X�^�����e�i�X�V�j�̍X�V�O�ƍX�V��̐��k�}�X�^�̏����f�[�^�e�[�u���Ɋi�[���܂��B
    ''' </summary>
    ''' <param name="bBeforeFlg">True : �X�V�O False : �X�V��</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/04/05 saitou ���t�M��(RSV2�W��) added for ���k�}�X�^�����e���[���P</remarks>
    Private Function fn_GetSEITOMAST_PrnCSVUpd(ByVal bBeforeFlg As Boolean) As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder
        Dim dtWorkTable As New DataTable

        Try
            '���[�N�p�̃f�[�^�e�[�u���̍쐬
            dtWorkTable = Me.dtBeforeSeitomast.Clone

            '���k�}�X�^���o�p��SQL�쐬
            With SQL
                .Append("select ")
                For i As Integer = 0 To KFGP107.TableInfo.ColumnEnglishName.Length - 1
                    If i = 0 Then : .Append(" " & KFGP107.TableInfo.ColumnEnglishName(i))
                    Else : .Append("," & KFGP107.TableInfo.ColumnEnglishName(i))
                    End If
                Next
                .Append(" from SEITOMAST")
                .Append(" where GAKKOU_CODE_O = " & SQ(Me.SeitomastMenteListInfo.GakkouCode))
                .Append(" and NENDO_O = " & SQ(Me.SeitomastMenteListInfo.Nendo))
                .Append(" and TUUBAN_O = " & GCom.NzInt(Me.SeitomastMenteListInfo.Tuuban))
                .Append(" order by TUKI_NO_O")
            End With

            '���k�}�X�^���i�[
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Dim dr As DataRow = dtWorkTable.NewRow
                    For i As Integer = 0 To KFGP107.TableInfo.ColumnEnglishName.Length - 1
                        dr(KFGP107.TableInfo.ColumnEnglishName(i)) = oraReader.GetString(KFGP107.TableInfo.ColumnEnglishName(i))
                    Next
                    dtWorkTable.Rows.Add(dr)
                    oraReader.NextRead()
                End While
            Else
                '���k�}�X�^�Ȃ�
                MainLOG.Write("���k�}�X�^����", "���s", "���k�}�X�^�ΏۂȂ�")
                Return False
            End If

            If bBeforeFlg = True Then
                Me.dtBeforeSeitomast = dtWorkTable.Copy
            Else
                Me.dtAfterSeitomast = dtWorkTable.Copy
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("���k�}�X�^�X�V�f�[�^�e�[�u���쐬", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' ���k�}�X�^�����e�i�X�V�j��CSV���쐬���܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/04/05 saitou ���t�M��(RSV2�W��) added for ���k�}�X�^�����e���[���P</remarks>
    Private Function fn_CreateCSV_UPD() As Boolean
        Try
            Dim CreateCSV As New KFGP107
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")
            Dim bUpdFlg As Boolean = False      '�X�V�t���O

            'CSV�쐬
            strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

            '���k�̐U�֏��ōX�V���ꂽ���ڂ����邩�`�F�b�N
            Dim drBeforeValue As DataRow() = Me.dtBeforeSeitomast.Select("TUKI_NO_O = '04'")
            Dim drAfterValue As DataRow() = Me.dtAfterSeitomast.Select("TUKI_NO_O = '04'")
            If drBeforeValue.Length = 0 OrElse drAfterValue.Length = 0 Then
                Return False
            End If

            '��ڂ̏�񂪐ݒ肳��Ă���J�����̑O�܂łŃ`�F�b�N
            For i As Integer = 0 To KFGP107.TableInfo.HIMOKU_INFO_START_INDEX - 1
                If drBeforeValue(0)(i).ToString.Trim <> drAfterValue(0)(i).ToString.Trim Then
                    Call Me.SetUpdateListData(CreateCSV, Today, NowTime, i, drBeforeValue(0)(i).ToString.Trim, drAfterValue(0)(i).ToString)
                    bUpdFlg = True
                End If
            Next

            '�X�V���ȍ~�̗\�����ڂ̃`�F�b�N
            For i As Integer = KFGP107.TableInfo.KOUSIN_DATE_INDEX + 1 To KFGP107.TableInfo.ColumnEnglishName.Length - 1
                If drBeforeValue(0)(i).ToString.Trim <> drAfterValue(0)(i).ToString.Trim Then
                    Call Me.SetUpdateListData(CreateCSV, Today, NowTime, i, drBeforeValue(0)(i).ToString.Trim, drAfterValue(0)(i).ToString)
                    bUpdFlg = True
                End If
            Next

            '��ڏ��̃`�F�b�N
            '�o�͏���4����3��
            For i As Integer = 4 To 12
                drBeforeValue = Me.dtBeforeSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")
                drAfterValue = Me.dtAfterSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")

                '��ڂ̏�񂪐ݒ肳��Ă���J��������X�V���̑O�܂Ń`�F�b�N
                For j As Integer = KFGP107.TableInfo.HIMOKU_INFO_START_INDEX To KFGP107.TableInfo.KOUSIN_DATE_INDEX - 1
                    If drBeforeValue(0)(j).ToString.Trim <> drAfterValue(0)(j).ToString.Trim Then
                        Call Me.SetUpdateListData(CreateCSV, Today, NowTime, j, drBeforeValue(0)(j).ToString.Trim, drAfterValue(0)(j).ToString)
                        bUpdFlg = True
                    End If
                Next
            Next

            For i As Integer = 1 To 3
                drBeforeValue = Me.dtBeforeSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")
                drAfterValue = Me.dtAfterSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")

                '��ڂ̏�񂪐ݒ肳��Ă���J��������X�V���̑O�܂Ń`�F�b�N
                For j As Integer = KFGP107.TableInfo.HIMOKU_INFO_START_INDEX To KFGP107.TableInfo.KOUSIN_DATE_INDEX - 1
                    If drBeforeValue(0)(j).ToString.Trim <> drAfterValue(0)(j).ToString.Trim Then
                        Call Me.SetUpdateListData(CreateCSV, Today, NowTime, j, drBeforeValue(0)(j).ToString.Trim, drAfterValue(0)(j).ToString)
                        bUpdFlg = True
                    End If
                Next
            Next

            CreateCSV.CloseCsv()
            CreateCSV = Nothing

            Return bUpdFlg

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(���k�}�X�^�����e(�X�V))���", "���s", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' ���k�}�X�^�����e�i�X�V�j��CSV��ݒ肵�܂��B
    ''' </summary>
    ''' <remarks>2017/04/05 saitou ���t�M��(RSV2�W��) added for ���k�}�X�^�����e���[���P</remarks>
    Private Sub SetUpdateListData(ByRef CreateCSV As KFGP107, _
                                      ByVal Today As String, _
                                      ByVal NowTime As String, _
                                      ByVal intColumnIndex As Integer, _
                                      ByVal strBeforeValue As String, _
                                      ByVal strAfterValue As String)

        With CreateCSV
            .OutputCsvData(Today, True)
            .OutputCsvData(NowTime, True)
            .OutputCsvData(GCom.GetUserID, True)
            .OutputCsvData(Environment.MachineName, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.GakkouCode, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.Nendo, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.Tuuban, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.GakunenCode, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.ClassCode, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.SeitoNo, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.GakkouName, True)
            .OutputCsvData(KFGP107.TableInfo.GetColumnJapaneseName(intColumnIndex), True)
            .OutputCsvData(KFGP107.TableInfo.GetColumnEnglishName(intColumnIndex), True)
            .OutputCsvData(strBeforeValue, True)
            .OutputCsvData(strAfterValue, True, True)
        End With
    End Sub

    '2017/04/05 saitou ���t�M��(RSV2�W��) DEL ���k�}�X�^�����e���[���P ---------------------------------------- START
    ''2010/10/18 ���k�}�X�^�����e�i���X����������P�p�N���X
    'Private Class MyStringDictionary
    '    Inherits System.Collections.Specialized.StringDictionary

    '    Public Sub New(ByVal filename As String)
    '        Try
    '            Dim FileNameFull As String = Path.Combine(GCom.GetTXTFolder, filename)

    '            If File.Exists(FileNameFull) Then
    '                Using sr As StreamReader = New StreamReader(FileNameFull, Encoding.GetEncoding(932))
    '                    While sr.Peek > -1
    '                        Dim s() As String = sr.ReadLine().Split(","c)
    '                        If MyBase.ContainsKey(s(0).Trim) = False Then
    '                            MyBase.Add(s(0).Trim, s(1).Trim)
    '                        End If
    '                    End While
    '                End Using
    '            End If
    '        Catch
    '            Throw
    '        End Try
    '    End Sub

    '    Default Public Overrides Property Item(ByVal key As String) As String
    '        Get
    '            If ContainsKey(key) = True Then
    '                Return MyBase.Item(key)
    '            Else
    '                Return ""
    '            End If
    '        End Get
    '        Set(ByVal value As String)
    '            MyBase.Item(key) = value
    '        End Set
    '    End Property

    'End Class
    '2017/04/05 saitou ���t�M��(RSV2�W��) DEL ----------------------------------------------------------------- END

#End Region


End Class
