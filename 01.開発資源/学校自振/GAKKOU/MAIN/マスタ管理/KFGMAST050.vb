Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Public Class KFGMAST050

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
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST050", "�V�����}�X�^�����e�i���X���")
    Private Const msgTitle As String = "�V�����}�X�^�����e�i���X���(KFGMAST050)"

#Region " Form Load "
    Private Sub KFGMAST050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "�V�����}�X�^�����e�i���X���"
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

            '���͋֎~����
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False

            btnKOUZA_CHK.Visible = True
            lblKOUZA_CHK.Visible = True

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
                    MessageBox.Show(G_MSG0084W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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

                '��ʃN���A
                Call fn_FormInitialize()
                '���͍��ڐ���
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtGAKUNEN_CODE.Enabled = False
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

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
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

            Dim bret As Boolean = False

            Try
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

                '��ʃN���A 2007/10/31
                Call fn_FormInitialize()
                '���͍��ڐ���
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtGAKUNEN_CODE.Enabled = False
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

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            LW.ToriCode = txtGAKKOU_CODE.Text = "000000000000"
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

            Dim bret As Boolean = False

            Try
                '�g�����U�N�V�����J�n
                Call GFUNC_EXECUTESQL_TRANS("", 0)

                Dim SQL As String = ""
                SQL = " DELETE  FROM SEITOMAST2"
                SQL &= " WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                SQL &= " AND NENDO_O ='" & Trim(txtNENDO.Text) & "'"
                SQL &= " AND TUUBAN_O = " & CInt(txtTUUBAN.Text)
                SQL &= " AND GAKUNEN_CODE_O = " & CByte(txtGAKUNEN_CODE.Text)
                SQL &= " AND CLASS_CODE_O = " & CByte(txtCLASS_CODE.Text)
                SQL &= " AND SEITO_NO_O ='" & Trim(txtSEITO_NO.Text) & "'"

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
                txtGAKUNEN_CODE.Enabled = False
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
            LW.ToriCode = "000000000000"
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

                '�V�����}�X�^���݃`�F�b�N
                If Not OraReader.DataReader(SQL) Then
                    MessageBox.Show(G_MSG0085W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                Else
                    '2017/05/11 �^�X�N�j���� ADD �W���ŏC���i���k�}�X�^�̌����Q�d�Ή��j---------------------- START
                    '�Ɖ�����擾���A�Q���ȏ�̏ꍇ�͍Č���𑣂����b�Z�[�W��\������
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

                            MessageBox.Show(String.Format(G_MSG0108W, "�V����", MSG_INFO), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
                        Select Case Trim(.GetItem("TYOUSI_FLG_O"))
                            Case 0
                                str���q�w�Z�R�[�h = ""
                                str���q���w�N�x = ""
                                str���q�ʔ� = ""
                                str���q�w�N = ""
                                str���q�N���X = ""
                                str���q���k�ԍ� = ""
                            Case 1

                                str���q�w�Z�R�[�h = .GetItem("GAKKOU_CODE_O")
                                str���q���w�N�x = .GetItem("TYOUSI_NENDO_O")
                                str���q�ʔ� = .GetItem("TYOUSI_TUUBAN_O")
                                str���q�w�N = .GetItem("TYOUSI_GAKUNEN_O")
                                str���q�N���X = .GetItem("TYOUSI_CLASS_O")
                                str���q���k�ԍ� = .GetItem("TYOUSI_SEITONO_O")
                        End Select

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
            LW.ToriCode = "000000000000"
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
            txtGAKUNEN_CODE.Enabled = False
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

        '�ʔԎ擾�{�^��

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtNENDO.Text) <> "" Then
                Dim SQL As New StringBuilder(128)

                SQL.Append(" SELECT * FROM SEITOMAST2 ")
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
            LW.ToriCode = "000000000000"
        End Try

    End Sub
    Private Sub btnHimoku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHimoku.Click

        Dim intSpdRow As Integer

        Dim dblKingaku_Goukei As Double

        '��ڎ擾�{�^��
        Try
            '��ڎ擾�{�^��
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
            SQL.Append(" AND GAKUNEN_CODE_H ='1'")
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
                SQL.Append(" AND GAKUNEN_CODE_H = 1")
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
                                '�������@
                                .Rows(intRow).Cells(intCol).Value = ""
                            Case 1
                                '��ږ�
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

            SQL.Append("INSERT INTO SEITOMAST2 ")
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
            SQL.Append(",0")
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
                '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                If intRowCnt <= (intMaxHimokuCnt - 1) Then
                    'If intRowCnt <= 9 Then
                    '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
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
                    '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                Else
                    '�������@
                    SQL.Append(",'0'")
                    '���z
                    SQL.Append(",0")
                End If
                'Else
                '           '��ڂP�P�`�P�T
                '           '�������@
                '           SQL.Append(",'0'")
                '           '���z
                '           SQL.Append(",0")
                '           End If
                '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
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

            SQL.Append("UPDATE  SEITOMAST2 SET ")
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
            SQL.Append(",TYOUSI_FLG_O    = 0")

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
                '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                If intRowCnt <= (intMaxHimokuCnt - 1) Then
                    'If intRowCnt <= 9 Then
                    '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
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
                '            '��ڂP�P�`�P�T
                '            '�������@
                '            SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='0'")
                '            '���z
                '            SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                'End If
                '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
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
            SQL += " AND GAKUNEN_CODE_G = 1"

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                lab�N���X��.Text = ""
                txtCLASS_CODE.Focus()
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
                        If GCom.NzInt(txtCLASS_CODE.Text) = GCom.NzInt(OraReader.GetItem("CLASS_CODE1" & Format(intCnt, "00") & "_G")) Then
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

        SQL.Append(" SELECT * FROM SEITOMASTVIEW2 ")
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
                        SQL = " SELECT * FROM SEITOMAST2"
                        SQL += " WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                        SQL += " AND GAKUNEN_CODE_O =" & CByte(txtGAKUNEN_CODE.Text)
                        SQL += " AND TUUBAN_O = " & CDbl(txtTUUBAN.Text)
                        SQL += " AND NENDO_O = '" & Trim(txtNENDO.Text) & "'"

                        If Not OraReader.DataReader(SQL) Then
                        Else
                            MessageBox.Show(G_MSG0086W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtTUUBAN.Focus()
                            Exit Function
                        End If
                    End If
            End Select
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
            SQL += " AND GAKUNEN_CODE_G = 1"
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
            txtGAKKOU_CODE.Focus()
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
        SQL += " WHERE TSIT_NO_D =" & SQ(Trim(txtTSIT_NO.Text)) & ""
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
                txtMEIGI_KNAME.Text = Mid(OraReader.GetItem("KOKYAKU_KNAME_D").Trim, 1, 40)
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
    Private Sub txtTUUBAN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTUUBAN.Validating
        If cmbHIMOKU.Items.Count = 0 AndAlso lab�w�N��.Text.Trim <> "" AndAlso lab�w�Z��.Text.Trim <> "" Then
            '�w�Z�R�[�h�Ɗw�N�����ڃ��R�[�h�i��ڂh�c�^��ڃp�^�[�����j���擾
            cmbHIMOKU.Items.Clear()
            Call Sub_GetHimokuID()
        End If
    End Sub
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
            cmbHIMOKU.Items.Clear()
            '�w�Z�R�[�h�Ɗw�N�����ڃ��R�[�h�i��ڂh�c�^��ڃp�^�[�����j���擾
            Call Sub_GetHimokuID()
            Call Sub_Himoku_Initialize()
        End If
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
                cmbHIMOKU.Items.Clear()
                Call Sub_GetHimokuID()
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
        'Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles _
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

End Class
