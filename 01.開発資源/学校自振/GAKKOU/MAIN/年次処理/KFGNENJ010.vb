Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon


Public Class KFGNENJ010
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �i������
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

#Region " ���ʕϐ���` "
    Dim INT�g�p�w�N�� As Integer
    Dim INT�ō��w�N�� As Integer
    Dim STR�i���N�x As String
    Dim STR�w�Z�R�[�h As String
    Dim STR���w�N�x As String
    Dim INT�ʔ� As Integer
    Dim INT�w�N As Integer
    Dim INT�N���X As Integer
    Dim STR���k�ԍ� As String
    Dim STR���k�����J�i As String
    Dim STR���k�������� As String
    Dim STR���� As String
    Dim STR���Z�@�փR�[�h As String
    Dim STR�x�X�R�[�h As String
    Dim STR�Ȗ� As String
    Dim STR�����ԍ� As String
    Dim STR���`�l�J�i As String
    Dim STR���`�l���� As String
    Dim STR�U�֕��@ As String
    Dim STR�_��Z������ As String
    Dim STR�_��d�b�ԍ� As String
    Dim STR���敪 As String
    Dim STR�i���敪 As String
    Dim STR��ڂh�c As String
    Dim INT���q_�L���t���O As Integer
    Dim STR���q_���w�N�x As String
    Dim INT���q_�ʔ� As Integer
    Dim INT���q_�w�N As Integer
    Dim INT���q_�N���X As Integer
    Dim STR���q_���k�ԍ� As String
    Dim STR������ As String
    Dim STR��ڂP�������@ As String
    Dim LNG��ڂP�������z As Long
    Dim STR��ڂQ�������@ As String
    Dim LNG��ڂQ�������z As Long
    Dim STR��ڂR�������@ As String
    Dim LNG��ڂR�������z As Long
    Dim STR��ڂS�������@ As String
    Dim LNG��ڂS�������z As Long
    Dim STR��ڂT�������@ As String
    Dim LNG��ڂT�������z As Long
    Dim STR��ڂU�������@ As String
    Dim LNG��ڂU�������z As Long
    Dim STR��ڂV�������@ As String
    Dim LNG��ڂV�������z As Long
    Dim STR��ڂW�������@ As String
    Dim LNG��ڂW�������z As Long
    Dim STR��ڂX�������@ As String
    Dim LNG��ڂX�������z As Long
    Dim STR��ڂP�O�������@ As String
    Dim LNG��ڂP�O�������z As Long
    Dim STR��ڂP�P�������@ As String
    Dim LNG��ڂP�P�������z As Long
    Dim STR��ڂP�Q�������@ As String
    Dim LNG��ڂP�Q�������z As Long
    Dim STR��ڂP�R�������@ As String
    Dim LNG��ڂP�R�������z As Long
    Dim STR��ڂP�S�������@ As String
    Dim LNG��ڂP�S�������z As Long
    Dim STR��ڂP�T�������@ As String
    Dim LNG��ڂP�T�������z As Long
    Dim STR�쐬���t As String
    Dim STR�X�V���t As String
    Dim STR������ As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ010", "�i���������")
    Private Const msgTitle As String = "�i���������(KFGNENJ010)"

    '2010/10/21 �V�X�e�����t���ߋ��N�x�̏ꍇ�A�i���s�Ƃ���--------------------------------
    '�^�C���X�^���v�擾
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '���ݓ��t
    '-----------------------------------------------------------------------------------------
    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
    Private HIMOKU_CLR_FLG As String = "0"  '��ڋ��z�N���A�t���O(0:�N���A����A1:�N���A���Ȃ�)
    Private HIMOKU_CHK As String = "0"      '�i����̔�ڑ��݃`�F�b�N�v��(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)

    Private Structure typPrnList
        Dim GAKKOU_CODE As String
        Dim GAKKOU_NNAME As String
        Dim NENDO As String
        Dim TUUBAN As Integer
        Dim GAKUNEN As String
        Dim CLASS_CODE As String
        Dim SEITO_NO As String
        Dim SEITO_KNAME As String
        Dim SEITO_NNAME As String
        Dim HIMOKU_ID_OLD As String
        Dim HIMOKU_ID_NEW As String
    End Structure

    Private ArrPrnList As ArrayList
    'Private PrnList As typPrnList
    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '���O�p
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            With Me
                .WindowState = FormWindowState.Normal
                .FormBorderStyle = FormBorderStyle.FixedDialog
                .ControlBox = True
            End With

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            txtFuriDateY.Text = Mid(lblDate.Text, 1, 4)

            '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
            Dim strFlg As String = GetFSKJIni("GNENJI", "HIMOKU_KINGAKU_CLEAR_FLG")
            If Not strFlg = "" AndAlso Not strFlg.ToUpper = "ERR" AndAlso Not strFlg Is Nothing Then
                HIMOKU_CLR_FLG = strFlg
            End If

            Dim strChk As String = GetFSKJIni("GNENJI", "HIMOKU_CHK")
            If Not strChk = "" AndAlso Not strChk.ToUpper = "ERR" AndAlso Not strChk Is Nothing Then
                HIMOKU_CHK = strChk
            End If

            If HIMOKU_CLR_FLG = "1" Then
                Label7.Text = "��ڃ}�X�^�̊e��ڋ��z�̓N���A�����A���k�}�X�^�̌ʐݒ�Ƌ��z�̃N���A�̂ݍs���܂��B"
            End If
            '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END

            '���̓{�^������
            btnAction.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New MyOracle
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        Try

            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check(MainDB) = False Then
                Return
            End If

            '2017/04/18 �^�X�N�j���� ADD �W���ŏC���i�i���������O�`�F�b�N�����ǉ��j------------------------------------ START
            If Not PFUNC_PROGRESS_CHECK(txtGAKKOU_CODE.Text, MainDB) Then
                MessageBox.Show(G_MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If
            '2017/04/18 �^�X�N�j���� ADD �W���ŏC���i�i���������O�`�F�b�N�����ǉ��j------------------------------------ END

            If MessageBox.Show(String.Format(MSG0015I, "�i��"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            Select Case (Trim(txtGAKKOU_CODE.Text))
                Case "9999999999"

                    '�S�w�Z�Ώ�
                    '�w�Z�}�X�^�Q�Ǎ��ݗp�̃R�l�N�V������V��

                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)

                    sql.Append(" SELECT * FROM GAKMAST2")
                    sql.Append(" ORDER BY GAKKOU_CODE_T ASC")

                    If oraReader.DataReader(sql) = False Then

                        oraReader.Close()
                        Return

                    Else

                        Do Until oraReader.EOF

                            '�w�Z�}�X�^�Q����i���N�x�Ǝg�p�w�N�𓾂�
                            '�w�Z�R�[�h�����ʕϐ��ɐݒ�

                            STR�w�Z�R�[�h = oraReader.GetString("GAKKOU_CODE_T")
                            LW.ToriCode = STR�w�Z�R�[�h

                            '�g�p�w�N��
                            INT�g�p�w�N�� = oraReader.GetString("SIYOU_GAKUNEN_T")

                            '�ō��w�N��
                            INT�ō��w�N�� = oraReader.GetString("SAIKOU_GAKUNEN_T")

                            '�i���N�x
                            If IsDBNull(oraReader.GetString("SINKYU_NENDO_T")) = False Then
                                STR�i���N�x = oraReader.GetString("SINKYU_NENDO_T")
                            Else
                                STR�i���N�x = ""
                            End If

                            '�w�Z�}�X�^�Q�̐i���N�x�ƑΏ۔N�x���݂āA�����̑Ó������`�F�b�N
                            '2010/10/21 �V�X�e�����t���ߋ��N�x�̏ꍇ�A�i���s�Ƃ���
                            ''2007/08/17�@�ď����ł���悤�ɕύX
                            'If Trim(txtFuriDateY.Text) <= STR�i���N�x Then
                            '    If MessageBox.Show("���ł�" & STR�i���N�x & "�N�x�̐i�������͏I�����Ă��܂�" & vbCrLf & "�������܂����H", STR�w�Z�R�[�h, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = DialogResult.Cancel Then
                            '        Return
                            '    End If
                            'End If
                            If Trim(txtFuriDateY.Text) >= mMatchingDate.Substring(0, 4) Then
                                '2007/08/17�@�ď����ł���悤�ɕύX
                                If Trim(txtFuriDateY.Text) <= STR�i���N�x Then
                                    If MessageBox.Show(String.Format(G_MSG0010I, STR�i���N�x, STR�w�Z�R�[�h), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                                        GoTo NEXT_WHILE
                                    End If
                                End If
                            Else
                                MessageBox.Show(G_MSG0039W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Sub
                            End If


                            '�g�����U�N�V�����J�n
                            MainDB.BeginTrans()
                            MainLOG.Write("�i�����C������", "����", "�J�n")

                            '2017/04/19 �^�X�N�j���� ADD �W���ŏC���i�i���߂������Ή��j------------------------------------ START
                            If Not PFUNC_SINQBK(txtGAKKOU_CODE.Text, MainDB) Then
                                MainLOG.Write("�i���O���ޔ�����", "���s", "")
                                MainDB.Rollback()
                                Return
                            End If
                            '2017/04/19 �^�X�N�j���� ADD �W���ŏC���i�i���߂������Ή��j------------------------------------ END

                            '���k�}�X�^�̍X�V
                            If PFUNC_SEITOMAST_UPD(MainDB) = False Then
                                MainLOG.Write("���k�}�X�^�X�V", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            '�V�����}�X�^�̍X�V
                            If PFUNC_SEITOMAST2_UPD(MainDB) = False Then
                                MainLOG.Write("�V�����}�X�^", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            '�V�����̍폜����
                            If PFUNC_SEITOMAST2_DEL(MainDB) = False Then
                                MainLOG.Write("�V�����폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            '�w�Z�}�X�^�Q�̍X�V
                            If PFUNC_GAKMAST2_UPD(MainDB) = False Then
                                MainLOG.Write("�w�Z�}�X�^�Q�X�V����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            '�G���g���P�̍폜����
                            If PFUNC_G_ENTMAST1_DEL(MainDB) = False Then
                                MainLOG.Write("�G���g���}�X�^�P�폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '�G���g���Q�̍폜����
                            If PFUNC_G_ENTMAST2_DEL(MainDB) = False Then
                                MainLOG.Write("�G���g���}�X�^�Q�폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '�ُ탊�X�g�̍폜����
                            If PFUNC_G_IJYOLIST_DEL(MainDB) = False Then
                                MainLOG.Write("�ُ탊�X�g�폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '���׃}�X�^�̍폜����
                            If PFUNC_G_MEIMAST_DEL(MainDB) = False Then
                                MainLOG.Write("���׃}�X�^�폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '���у}�X�^�̍폜����
                            If PFUNC_JISSEKIMAST_DEL(MainDB) = False Then
                                MainLOG.Write("���у}�X�^�폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '�X�P�W���[���}�X�^�̍폜����
                            If PFUNC_G_SCHMAST_DEL(MainDB) = False Then
                                MainLOG.Write("�X�P�W���[���}�X�^�폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '���s�X�P�W���[���}�X�^�̍폜����
                            If PFUNC_G_TAKOUSCHMAST_DEL(MainDB) = False Then
                                MainLOG.Write("���s�X�P�W���[���}�X�^�폜����", "���s", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '2009/01/08 ��ڃ}�X�^���z�̃N���A���s��----
                            '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
                            If HIMOKU_CLR_FLG = "0" Then
                                '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END
                                '��ڃ}�X�^�̍X�V
                                If PFUNC_HIMOMAST_UPD(MainDB) = False Then
                                    MainLOG.Write("��ڃ}�X�^�X�V����", "���s", "")
                                    MainDB.Rollback()
                                    GoTo NEXT_WHILE
                                End If
                                '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
                            End If
                            '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END
                            '-------------------------------------------

                            '2016/11/07 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
                            '�i����̐��k�}�X�^�������`�F�b�N���s��
                            If HIMOKU_CHK = "1" Then
                                If PFUNC_HIMOKU_CHK(MainDB) = False Then
                                    MainLOG.Write("���k�}�X�^�������`�F�b�N", "���s", "")
                                    MainDB.Rollback()
                                    GoTo NEXT_WHILE
                                Else
                                    MessageBox.Show(String.Format(MSG0014I, "���k�}�X�^�������`�F�b�N���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                End If
                            End If
                            '2016/11/07 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END

                            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
                            MainDB.Commit()
                            MainLOG.Write("�i�����C������", "����", "�R�~�b�g")

NEXT_WHILE:

                            oraReader.NextRead()

                        Loop

                    End If

                    oraReader.Close()

                Case Else

                    '���͊w�Z�R�[�h�̂ݑΏ�
                    '===2008/04/16 �w�Z�R�[�h�����O�ɏo�͂���悤�C��===
                    LW.ToriCode = txtGAKKOU_CODE.Text
                    '===================================================

                    '�w�Z�}�X�^�Q����i���N�x�Ǝg�p�w�N�𓾂�
                    If PFUNC_GAKMAST2_GET(MainDB) = False Then
                        Exit Sub
                    End If

                    '�w�Z�}�X�^�Q�̐i���N�x�ƑΏ۔N�x���݂āA�����̑Ó������`�F�b�N
                    '2010/10/21 �V�X�e�����t���ߋ��N�x�̏ꍇ�A�i���s�Ƃ���
                    ''2007/08/17�@�ď����ł���悤�ɕύX
                    'If Trim(txtFuriDateY.Text) <= STR�i���N�x Then
                    '    If MessageBox.Show("���ł�" & STR�i���N�x & "�N�x�̐i�������͏I�����Ă��܂�" & vbCrLf & "�������܂����H", STR�w�Z�R�[�h, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = DialogResult.Cancel Then
                    '        Return
                    '    End If
                    'End If
                    If Trim(txtFuriDateY.Text) >= mMatchingDate.Substring(0, 4) Then
                        '2007/08/17�@�ď����ł���悤�ɕύX
                        If Trim(txtFuriDateY.Text) <= STR�i���N�x Then
                            If MessageBox.Show(String.Format(G_MSG0010I, STR�i���N�x, STR�w�Z�R�[�h), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                                Return
                            End If
                        End If
                    Else
                        MessageBox.Show(G_MSG0039W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                    '-----------------------------------------------------------------

                    '�g�����U�N�V�����J�n
                    MainDB.BeginTrans()
                    MainLOG.Write("�i�����C������", "����", "�J�n")

                    '2017/04/19 �^�X�N�j���� ADD �W���ŏC���i�i���߂������Ή��j------------------------------------ START
                    If Not PFUNC_SINQBK(txtGAKKOU_CODE.Text, MainDB) Then
                        MainLOG.Write("�i���O���ޔ�����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '2017/04/19 �^�X�N�j���� ADD �W���ŏC���i�i���߂������Ή��j------------------------------------ END

                    '���k�}�X�^�̍X�V
                    If PFUNC_SEITOMAST_UPD(MainDB) = False Then
                        MainLOG.Write("���k�}�X�^�X�V����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '�V�����}�X�^�̍X�V
                    If PFUNC_SEITOMAST2_UPD(MainDB) = False Then
                        MainLOG.Write("�V�����}�X�^�X�V����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '�V�����̍폜����
                    If PFUNC_SEITOMAST2_DEL(MainDB) = False Then
                        MainLOG.Write("�V�����폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '�w�Z�}�X�^�Q�̍X�V
                    If PFUNC_GAKMAST2_UPD(MainDB) = False Then
                        MainLOG.Write("�w�Z�}�X�^�Q�X�V����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '�G���g���P�̍폜����
                    If PFUNC_G_ENTMAST1_DEL(MainDB) = False Then
                        MainLOG.Write("�G���g���}�X�^�P�폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '�G���g���Q�̍폜����
                    If PFUNC_G_ENTMAST2_DEL(MainDB) = False Then
                        MainLOG.Write("�G���g���}�X�^�Q�폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '�ُ탊�X�g�̍폜����
                    If PFUNC_G_IJYOLIST_DEL(MainDB) = False Then
                        MainLOG.Write("�ُ탊�X�g�폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '���׃}�X�^�̍폜����
                    If PFUNC_G_MEIMAST_DEL(MainDB) = False Then
                        MainLOG.Write("���׃}�X�^�폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '���у}�X�^�̍폜����
                    If PFUNC_JISSEKIMAST_DEL(MainDB) = False Then
                        MainLOG.Write("���у}�X�^�폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '�X�P�W���[���}�X�^�̍폜����
                    If PFUNC_G_SCHMAST_DEL(MainDB) = False Then
                        MainLOG.Write("�X�P�W���[���}�X�^�폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '���s�X�P�W���[���}�X�^�̍폜����
                    If PFUNC_G_TAKOUSCHMAST_DEL(MainDB) = False Then
                        MainLOG.Write("���s�X�P�W���[���}�X�^�폜����", "���s", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '2009/01/08 ��ڃ}�X�^���z�̃N���A���s��----
                    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
                    If HIMOKU_CLR_FLG = "0" Then
                        '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END
                        '��ڃ}�X�^�̍X�V
                        If PFUNC_HIMOMAST_UPD(MainDB) = False Then
                            MainLOG.Write("��ڃ}�X�^�X�V����", "���s", "")
                            MainDB.Rollback()
                            Return
                        End If
                        '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
                    End If
                    '2016/11/04 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END
                    '-------------------------------------------

                    '2016/11/07 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
                    '�i����̐��k�}�X�^�������`�F�b�N���s��
                    If HIMOKU_CHK = "1" Then
                        If PFUNC_HIMOKU_CHK(MainDB) = False Then
                            MainLOG.Write("���k�}�X�^�������`�F�b�N", "���s", "")
                            MainDB.Rollback()
                            Return
                        End If
                    End If
                    '2016/11/07 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END

                    '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
                    MainDB.Commit()
                    MainLOG.Write("�i�����C������", "����", "�R�~�b�g")

            End Select

            MessageBox.Show(String.Format(MSG0016I, "�i��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)��O�G���[", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '�I���{�^��
        Me.Close()

    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        Dim sGakkkou_Name As String = ""
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '�w�Z���̐ݒ�
        sql.Append(" SELECT GAKKOU_NNAME_G FROM GAKMAST1")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = True Then
            sGakkkou_Name = oraReader.GetString("GAKKOU_NNAME_G")
        End If
        oraReader.Close()

        '�w�Z�}�X�^�P���݃`�F�b�N
        If sGakkkou_Name = "" Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lab�w�Z��.Text = ""
            txtGAKKOU_CODE.Focus()
            Return False
        End If

        lab�w�Z��.Text = sGakkkou_Name

        Return True

    End Function
    Private Function PFUNC_GAKMAST2_GET(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)

        '�w�Z�}�X�^�Q�̎擾
        sql.Append(" SELECT * FROM GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        Else

            '�w�Z�R�[�h�����ʕϐ��ɐݒ�
            STR�w�Z�R�[�h = oraReader.GetString("GAKKOU_CODE_T")

            '�g�p�w�N��
            INT�g�p�w�N�� = oraReader.GetString("SIYOU_GAKUNEN_T")

            '�ō��w�N��
            INT�ō��w�N�� = oraReader.GetString("SAIKOU_GAKUNEN_T")

            '�i���N�x
            If IsDBNull(oraReader.GetString("SINKYU_NENDO_T")) = False Then
                STR�i���N�x = oraReader.GetString("SINKYU_NENDO_T")
            Else
                STR�i���N�x = ""
            End If

        End If
        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_SEITOMAST_UPD(ByVal db As MyOracle) As Boolean

        Dim sql As StringBuilder

        '���k�}�X�^�̍X�V�i���Ɛ��̍폜�j

        PFUNC_SEITOMAST_UPD = False

        '���Ɛ��̍폜
        sql = New StringBuilder(128)
        sql.Append(" DELETE  FROM SEITOMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR�w�Z�R�[�h & "'")
        sql.Append(" AND")
        sql.Append(" GAKUNEN_CODE_O = " & INT�ō��w�N��)
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O ='0'")


        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '�ݍZ���̐i������
        sql = New StringBuilder(128)
        sql.Append(" UPDATE  SEITOMAST SET ")
        sql.Append(" GAKUNEN_CODE_O = GAKUNEN_CODE_O + 1")
        sql.Append(",KOUSIN_DATE_O = '" & Format(Now, "yyyyMMdd") & "'")

        '2008/02/07�@�������@�Ƌ��z���N���A�ɂ���
        For i As Integer = 1 To 15
            sql.Append(" ,SEIKYU" & Format(i, "00") & "_O ='0' ")
            sql.Append(" ,KINGAKU" & Format(i, "00") & "_O = 0 ")
        Next
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR�w�Z�R�[�h & "'")
        sql.Append(" AND")
        sql.Append(" GAKUNEN_CODE_O < " & INT�ō��w�N��)
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O ='0'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '2008/03/13 ���ςݐ��k�i���t���O���X�j�̐��k�}�X�^���폜����============================
        sql = New StringBuilder(128)
        sql.Append(" DELETE  FROM SEITOMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR�w�Z�R�[�h & "'")
        sql.Append(" AND")
        sql.Append(" KAIYAKU_FLG_O = '9'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        '==========================================================================================

        '2009/01/14 �i�����Ȃ����k���������@�Ƌ��z���N���A����----------------------------------------------------
        sql = New StringBuilder(128)
        sql.Append(" UPDATE  SEITOMAST SET ")
        sql.Append(" SEIKYU01_O ='0' ")
        sql.Append(" ,KINGAKU01_O = 0 ")
        Dim j As Integer
        For j = 2 To 15
            sql.Append(" ,SEIKYU" & Format(j, "00") & "_O ='0' ")
            sql.Append(" ,KINGAKU" & Format(j, "00") & "_O = 0 ")
        Next
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR�w�Z�R�[�h & "'")
        sql.Append(" AND")
        sql.Append(" ( SINKYU_KBN_O ='1' OR GAKUNEN_CODE_O > " & INT�ō��w�N�� & ")")  '�i�����Ȃ�or�w�N>�ō��w�N

        If db.ExecuteNonQuery(sql) < 0 Then
            '�X�V�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        '----------------------------------------------------------------------------------------------------------

        Return True

    End Function
    Private Function PFUNC_SEITOMAST2_UPD(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�V�����}�X�^�̏���
        sql.Append(" INSERT INTO SEITOMAST")
        sql.Append(" SELECT ")
        sql.Append(" GAKKOU_CODE_O")
        sql.Append(", NENDO_O")
        sql.Append(", TUUBAN_O")
        sql.Append(", 1")
        sql.Append(", CLASS_CODE_O")
        sql.Append(", SEITO_NO_O")
        sql.Append(", SEITO_KNAME_O")
        sql.Append(", SEITO_NNAME_O")
        sql.Append(", SEIBETU_O")
        sql.Append(", TKIN_NO_O")
        sql.Append(", TSIT_NO_O")
        sql.Append(", KAMOKU_O")
        sql.Append(", KOUZA_O")
        sql.Append(", MEIGI_KNAME_O")
        sql.Append(", MEIGI_NNAME_O")
        sql.Append(", FURIKAE_O")
        sql.Append(", KEIYAKU_NJYU_O")
        sql.Append(", KEIYAKU_DENWA_O")
        sql.Append(", KAIYAKU_FLG_O")
        sql.Append(", SINKYU_KBN_O")
        sql.Append(", HIMOKU_ID_O")
        sql.Append(", TYOUSI_FLG_O")
        sql.Append(", TYOUSI_NENDO_O")
        sql.Append(", TYOUSI_TUUBAN_O")
        sql.Append(", TYOUSI_GAKUNEN_O")
        sql.Append(", TYOUSI_CLASS_O")
        sql.Append(", TYOUSI_SEITONO_O")
        sql.Append(", TUKI_NO_O")
        sql.Append(", SEIKYU01_O         ")
        sql.Append(", KINGAKU01_O")
        sql.Append(", SEIKYU02_O         ")
        sql.Append(", KINGAKU02_O")
        sql.Append(", SEIKYU03_O         ")
        sql.Append(", KINGAKU03_O")
        sql.Append(", SEIKYU04_O         ")
        sql.Append(", KINGAKU04_O")
        sql.Append(", SEIKYU05_O         ")
        sql.Append(", KINGAKU05_O")
        sql.Append(", SEIKYU06_O        ")
        sql.Append(", KINGAKU06_O")
        sql.Append(", SEIKYU07_O        ")
        sql.Append(", KINGAKU07_O")
        sql.Append(", SEIKYU08_O         ")
        sql.Append(", KINGAKU08_O")
        sql.Append(", SEIKYU09_O         ")
        sql.Append(", KINGAKU09_O")
        sql.Append(", SEIKYU10_O         ")
        sql.Append(", KINGAKU10_O")
        sql.Append(", SEIKYU11_O         ")
        sql.Append(", KINGAKU11_O")
        sql.Append(", SEIKYU12_O         ")
        sql.Append(", KINGAKU12_O")
        sql.Append(", SEIKYU13_O        ")
        sql.Append(", KINGAKU13_O")
        sql.Append(", SEIKYU14_O        ")
        sql.Append(", KINGAKU14_O")
        sql.Append(", SEIKYU15_O         ")
        sql.Append(", KINGAKU15_O")
        sql.Append(", SAKUSEI_DATE_O")
        sql.Append(", KOUSIN_DATE_O")
        sql.Append(", YOBI1_O")
        sql.Append(", YOBI2_O")
        sql.Append(", YOBI3_O")
        sql.Append(", YOBI4_O")
        sql.Append(", YOBI5_O")
        sql.Append(" FROM SEITOMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR�w�Z�R�[�h & "'")
        sql.Append(" AND")
        sql.Append(" NENDO_O = '" & txtFuriDateY.Text & "'")
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O = '0'")
        sql.Append(" AND KAIYAKU_FLG_O = '0' ")      '2008/03/23 ���̐��k��INSERT���Ȃ�

        If db.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "�}��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SEITOMAST2_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�V�����}�X�^�̍폜
        sql.Append(" DELETE  FROM SEITOMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR�w�Z�R�[�h & "'")
        sql.Append(" AND")
        sql.Append(" NENDO_O = '" & txtFuriDateY.Text & "'")
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O = '0'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_GAKMAST2_UPD(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�w�Z�}�X�^�Q�̐i���N�x�X�V
        sql.Append(" UPDATE  GAKMAST2 SET ")
        sql.Append(" SINKYU_NENDO_T ='" & txtFuriDateY.Text & "'")
        sql.Append(",KOUSIN_DATE_T ='" & Format(Now, "yyyyMMdd") & "'")
        sql.Append("  WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_Nyuryoku_Check(ByVal db As MyOracle) As Boolean

        Dim sStart As String
        Dim sEnd As String

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        ElseIf txtGAKKOU_CODE.Text <> "9999999999" Then '2007/03/28�@�S���Ώۂ̏ꍇ�A���݃`�F�b�N�����Ȃ�
            '�w�Z�}�X�^���݃`�F�b�N

            Dim sql As New StringBuilder(128)
            Dim oraReader As New MyOracleReader(db)

            sql.Append("SELECT *")
            sql.Append(" FROM GAKMAST2")
            sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(sql) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                oraReader.Close()
                Return False
            Else

                sStart = Mid(oraReader.GetString("KAISI_DATE_T"), 1, 4)
                '2007/03/28�@�C��
                'sEnd = Mid(OBJ_DATAREADER.Item("SYURYOU_DATE_T"), 1, 3)
                sEnd = Mid(oraReader.GetString("SYURYOU_DATE_T"), 1, 4)

            End If
            oraReader.Close()

        Else
            '2007/03/28�@�S���Ώۂ̏ꍇ�A����Ώ۔N�x�͈͂̃`�F�b�N�͂��Ȃ�
            If (Trim(txtFuriDateY.Text)) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�V�N�x"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            Else
                Return True
            End If

        End If

        If (Trim(txtFuriDateY.Text)) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�V�N�x"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Return False
        Else
            '2007/03/28�@�J�n�N�E�I���N�`�F�b�N�̏C��
            'Select case (sStart <= txtFuriDateY.Text >= sEnd)
            '    Case False
            '        Call GSUB_MESSAGE_WARNING( "�Ώ۔N�x�����͔͈͊O�ł�(" & sStart & "�`" & sEnd & ")")
            '        txtFuriDateY.Focus()
            '        Exit Function
            'End Select
            If sStart > txtFuriDateY.Text Or sEnd < txtFuriDateY.Text Then
                MessageBox.Show(String.Format(G_MSG0040W, sStart, sEnd), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

        End If

        Return True

    End Function

    Private Function PFUNC_G_IJYOLIST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�ُ탊�X�g�̍폜
        sql.Append(" DELETE  FROM G_IJYOLIST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_L ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_ENTMAST1_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�G���g���}�X�^�P�̍폜
        sql.Append(" DELETE  FROM G_ENTMAST1")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_E ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_ENTMAST2_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�G���g���}�X�^�Q�̍폜
        sql.Append(" DELETE  FROM G_ENTMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_E ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_MEIMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�V�����}�X�^�̍폜
        sql.Append(" DELETE  FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_JISSEKIMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�V�����}�X�^�̍폜
        sql.Append(" DELETE  FROM JISSEKIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_F ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_SCHMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�V�����}�X�^�̍폜
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" NENGETUDO_S BETWEEN '" & CStr(CInt(txtFuriDateY.Text) - 1) & "04' AND '" & txtFuriDateY.Text & "03'")
        sql.Append(" AND")
        sql.Append(" GAKKOU_CODE_S ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_TAKOUSCHMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '�V�����}�X�^�̍폜
        sql.Append(" DELETE  FROM G_TAKOUSCHMAST")
        sql.Append(" WHERE")
        sql.Append(" NENGETUDO_U BETWEEN '" & CStr(CInt(txtFuriDateY.Text) - 1) & "04' AND '" & txtFuriDateY.Text & "03'")
        sql.Append(" AND")
        sql.Append(" GAKKOU_CODE_U ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    '2009/01/08 �ǉ�
    Private Function PFUNC_HIMOMAST_UPD(ByVal db As MyOracle) As Boolean

        '��ڃ}�X�^�̍X�V�i���z�N���A�j

        Dim sql As New StringBuilder(128)

        sql.Append(" UPDATE  HIMOMAST SET ")
        sql.Append(" KOUSIN_DATE_H = '" & Format(Now, "yyyyMMdd") & "'")

        For i As Integer = 1 To 15
            sql.Append(" ,HIMOKU_KINGAKU" & Format(i, "00") & "_H = 0 ")
        Next
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_H ='" & STR�w�Z�R�[�h & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    '2016/11/07 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- START
    '�i����̔�ڃ}�X�^�����݂��邩�`�F�b�N���A���݂��Ȃ���ΑΏۊw�N�̍ŏ���ڂh�c��ݒ肷��i�O�O�O�ȊO�j
    '�܂��A��ڃ}�X�^�����݂��Ȃ������ꍇ�́A���k�}�X�^�������`�F�b�N���X�g���o�͂���B
    Private Function PFUNC_HIMOKU_CHK(ByVal db As MyOracle) As Boolean
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)�J�n", "����", "")

        '����z����N���A����
        If Not ArrPrnList Is Nothing Then
            ArrPrnList.Clear()
        Else
            ArrPrnList = New ArrayList
        End If

        Try
            '�V����̔�ڃ}�X�^�����݂��Ȃ����k���擾����
            With sql
                .Append("SELECT ")
                .Append(" GAKKOU_CODE_O ")
                .Append(",GAKKOU_NNAME_G ")
                .Append(",GAKKOU_KNAME_G ")
                .Append(",NENDO_O ")
                .Append(",TUUBAN_O ")
                .Append(",GAKUNEN_CODE_O ")
                .Append(",CLASS_CODE_O ")
                .Append(",SEITO_NO_O ")
                .Append(",SEITO_KNAME_O ")
                .Append(",SEITO_NNAME_O ")
                .Append(",HIMOKU_ID_O ")
                .Append(",HIMOKU_ID_H ")
                .Append("FROM ")
                .Append(" SEITOMAST ")
                .Append(",HIMOMAST ")
                .Append(",(SELECT DISTINCT ")
                .Append(" GAKKOU_CODE_G ")
                .Append(",GAKKOU_NNAME_G ")
                .Append(",GAKKOU_KNAME_G ")
                .Append("FROM ")
                .Append(" GAKMAST1) GMAST ")
                .Append("WHERE ")
                .Append("SEITOMAST.GAKKOU_CODE_O = GMAST.GAKKOU_CODE_G ")
                .Append("AND SEITOMAST.GAKKOU_CODE_O = HIMOMAST.GAKKOU_CODE_H(+) ")
                .Append("AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H(+) ")
                .Append("AND SEITOMAST.HIMOKU_ID_O = HIMOMAST.HIMOKU_ID_H(+) ")
                .Append("AND tuki_no_o=tuki_no_h(+) ")
                .Append("AND tuki_no_o='04' ")
                .Append("AND himoku_id_h is null ")
                .Append("order by GAKKOU_CODE_O,GAKUNEN_CODE_O,HIMOKU_ID_O ")
            End With

            If oraReader.DataReader(sql) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)�I��", "���s", "�w�Z�R�[�h�F" & STR�w�Z�R�[�h)
                MessageBox.Show(String.Format(MSG0034E, "��ڃ`�F�b�N"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            Else
                Do Until oraReader.EOF
                    Dim PrnLiST As New typPrnList

                    '����p�̍\���̂ɕێ�����
                    With PrnList
                        .GAKKOU_CODE = oraReader.GetString("GAKKOU_CODE_O")
                        .GAKKOU_NNAME = oraReader.GetString("GAKKOU_NNAME_G")
                        .NENDO = oraReader.GetString("NENDO_O")
                        .TUUBAN = oraReader.GetInt64("TUUBAN_O")
                        .GAKUNEN = oraReader.GetString("GAKUNEN_CODE_O")
                        .CLASS_CODE = oraReader.GetString("CLASS_CODE_O")
                        .SEITO_NO = oraReader.GetString("SEITO_NO_O")
                        .SEITO_KNAME = oraReader.GetString("SEITO_KNAME_O")
                        .SEITO_NNAME = oraReader.GetString("SEITO_NNAME_O")
                        .HIMOKU_ID_OLD = oraReader.GetString("HIMOKU_ID_O")
                    End With

                    '���k�f�[�^����Ώۊw�N�̍ŏ���ڃR�[�h���擾���X�V����
                    Dim oraReaderHimo As New MyOracleReader(db)
                    Dim SQL_HIMO As New StringBuilder
                    Dim SQL_UPD As New StringBuilder

                    Try
                        With SQL_HIMO
                            .Append("SELECT ")
                            .Append(" MIN(HIMOKU_ID_H) MIN_HIMOKU_ID ")
                            .Append("FROM ")
                            .Append(" HIMOMAST ")
                            .Append("WHERE ")
                            .Append(" GAKKOU_CODE_H = " & SQ(PrnList.GAKKOU_CODE))
                            .Append(" AND GAKUNEN_CODE_H = " & SQ(PrnList.GAKUNEN))
                            .Append(" AND HIMOKU_ID_H <> '000' ")
                            .Append("GROUP BY ")
                            .Append(" GAKKOU_CODE_H")
                            .Append(",GAKUNEN_CODE_H")
                        End With

                        If oraReaderHimo.DataReader(SQL_HIMO) = False Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)�I��", "���s", _
                                          String.Format("�ŏ���ڃR�[�h�擾���s�@�w�Z�R�[�h�F{0}�A�w�N�F{1}", PrnList.GAKKOU_CODE, PrnList.GAKUNEN))
                            MessageBox.Show(String.Format(MSG0034E, "��ڃ`�F�b�N"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtGAKKOU_CODE.Focus()
                            Return False
                        Else
                            Dim MIN_HIMOKU_ID As String = oraReaderHimo.GetString("MIN_HIMOKU_ID")
                            With SQL_UPD
                                .Append(" UPDATE SEITOMAST SET ")
                                .Append(" HIMOKU_ID_O = " & SQ(MIN_HIMOKU_ID))
                                .Append(" WHERE")
                                .Append(" GAKKOU_CODE_O =" & SQ(PrnList.GAKKOU_CODE))
                                .Append(" AND")
                                .Append(" NENDO_O = " & SQ(PrnList.NENDO))
                                .Append(" AND")
                                .Append(" TUUBAN_O = " & PrnList.TUUBAN.ToString)
                            End With

                            If db.ExecuteNonQuery(SQL_UPD) < 0 Then
                                '�X�V�����G���[
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)�I��", "���s", _
                                    String.Format("���k�}�X�^�X�V���s�@�w�Z�R�[�h�F{0}�A���w�N�x�F{1}�A�ʔԁF{2}", PrnList.GAKKOU_CODE, PrnList.NENDO, PrnList.TUUBAN.ToString))
                                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                            Else
                                '�X�V������ڂh�c���\���̂ɕێ�����
                                PrnList.HIMOKU_ID_NEW = oraReaderHimo.GetString("MIN_HIMOKU_ID")
                            End If
                        End If

                    Catch ex As Exception
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)�I��", "���s", ex.ToString)
                        MessageBox.Show(String.Format(MSG0034E, "��ڃ`�F�b�N"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    Finally
                        If Not oraReaderHimo Is Nothing Then
                            oraReaderHimo.Close()
                            oraReaderHimo = Nothing
                        End If
                    End Try

                    ArrPrnList.Add(PrnLiST)
                    oraReader.NextRead()
                Loop

            End If

            If ArrPrnList.Count > 0 Then
                Dim CreateCSV As New KFGP036(STR�w�Z�R�[�h)

                Dim CSV_FILE_NAME As String = CreateCSV.CreateCsvFile()

                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���k�}�X�^�������`�F�b�N���X�g)����J�n", "����", "CSV�t�@�C�����F" & CSV_FILE_NAME)

                For i As Long = 0 To ArrPrnList.Count - 1
                    Dim PrnList As typPrnList = CType(ArrPrnList(i), typPrnList)

                    With CreateCSV
                        .OutputCsvData(PrnList.GAKKOU_CODE, True)
                        .OutputCsvData(PrnList.GAKKOU_NNAME, True)
                        .OutputCsvData(txtFuriDateY.Text, True)
                        .OutputCsvData(PrnList.NENDO, True)
                        .OutputCsvData(PrnList.TUUBAN.ToString, True)
                        .OutputCsvData(PrnList.GAKUNEN, True)
                        .OutputCsvData(PrnList.CLASS_CODE)
                        .OutputCsvData(PrnList.SEITO_NO, True)
                        .OutputCsvData(PrnList.SEITO_KNAME, True)
                        .OutputCsvData(PrnList.SEITO_NNAME, True)
                        .OutputCsvData(PrnList.HIMOKU_ID_OLD, True)
                        .OutputCsvData(PrnList.HIMOKU_ID_NEW, True, True)
                    End With
                Next
                CreateCSV.CloseCsv()

                '����o�b�`�Ăяo��
                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me

                '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C�����A�w�Z�R�[�h
                Dim Param As String = GCom.GetUserID & "," & CSV_FILE_NAME & "," & STR�w�Z�R�[�h
                Dim nRet As Integer = ExeRepo.ExecReport("KFGP036.EXE", Param)
                Dim ErrMessage As String = ""
                If nRet <> 0 Then
                    '������s�F�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case -1
                            ErrMessage = String.Format(MSG0106W, "���k�}�X�^�������`�F�b�N���X�g")
                        Case Else
                            ErrMessage = String.Format(MSG0004E, "���k�}�X�^�������`�F�b�N���X�g")
                    End Select

                    MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���k�}�X�^�������`�F�b�N���X�g)����I��", "����", "")
                If STR�w�Z�R�[�h <> "9999999999" Then
                    MessageBox.Show(String.Format(MSG0014I, "���k�}�X�^�������`�F�b�N���X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)�I��", "����", "")
            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)�I��", "���s", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, "��ڃ`�F�b�N"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function
    '2016/11/07 �^�X�N�j���� ADD �yPG�z�ѓc�M�� �J�X�^�}�C�Y�Ή�(UI_11-012) -------------------- END

    '2017/04/18 �^�X�N�j���� ADD �W���ŏC���i�i���������O�`�F�b�N�����ǉ��j------------------------------------ START
    ''' <summary>
    ''' �w�肳�ꂽ�w�Z�̐i�s���X�P�W���[�����`�F�b�N����
    ''' </summary>
    ''' <param name="GAKKOU_CODE">�Ώۂ̊w�Z�R�[�h</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:�i�s���X�P�W���[���Ȃ� FALSE:�i�s���X�P�W���[������</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_PROGRESS_CHECK(ByVal GAKKOU_CODE As String, ByRef db As MyOracle) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)
        Dim bRet As Boolean = False

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)�J�n", "����", "")

        Try
            With SQL
                .Append("SELECT ")
                .Append(" COUNT(FURI_DATE_S) CNT ")
                .Append("FROM ")
                .Append(" G_SCHMAST ")
                .Append("WHERE ")
                .Append(" (ENTRI_FLG_S = '1' OR CHECK_FLG_S = '1' OR DATA_FLG_S = '1') ")   '���׍쐬�܂��͋��z�`�F�b�N�܂��͗�����������
                .Append(" AND FUNOU_FLG_S = '0' ")                                          '�s�\��������
                .Append(" AND TYUUDAN_FLG_S = '0' ")                                        '���f�ł͂Ȃ�
                If GAKKOU_CODE <> "9999999999" Then
                    .Append(" AND GAKKOU_CODE_S = " & SQ(GAKKOU_CODE))
                End If
            End With

            If oraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)�I��", "���s", "�w�Z�R�[�h�F" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, "�i�s���X�P�W���[���`�F�b�N"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            Else
                If oraReader.GetInt64("CNT") = 0 Then
                    '�i�s���X�P�W���[���Ȃ�
                    bRet = True
                Else
                    '�i�s���X�P�W���[������
                    bRet = False
                End If
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)�I��", "����", "")
            Return bRet

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)�I��", "���s", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, "�i�s���X�P�W���[���`�F�b�N"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function
    '2017/04/18 �^�X�N�j���� ADD �W���ŏC���i�i���������O�`�F�b�N�����ǉ��j------------------------------------ END

    '2017/04/19 �^�X�N�j���� ADD �W���ŏC���i�i���߂������Ή��j------------------------------------ START
    ''' <summary>
    ''' �w�肳�ꂽ�w�Z�̐i���O����ޔ�����
    ''' </summary>
    ''' <param name="GAKKOU_CODE">�Ώۂ̊w�Z�R�[�h</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:�ޔ𐬌� FALSE:�ޔ����s</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_SINQBK(ByVal GAKKOU_CODE As String, ByRef db As MyOracle) As Boolean
        Dim TARGET_TABLE As String = ""
        Dim SINQBK_TABLE As String = ""
        Dim KEY_NAME As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBK)�J�n", "����", "�w�Z�R�[�h�F" & GAKKOU_CODE)

        '-----------------------------------------------
        ' �w�Z�}�X�^�Q�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "GAKMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_T"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' ��ڃ}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "HIMOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_H"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z�X�P�W���[���}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_SCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_S"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z���s�X�P�W���[���}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_TAKOUSCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_U"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' ���k�}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �V�����}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z���׃}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_MEIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_M"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z�G���g���}�X�^�P�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST1"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z�G���g���}�X�^�Q�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' ���у}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "JISSEKIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_F"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBK)�I��", "����", "�w�Z�R�[�h�F" & GAKKOU_CODE)
        Return True
    End Function

    ''' <summary>
    ''' �}�X�^�ޔ�����
    ''' </summary>
    ''' <param name="GAKKOU_CODE">�w�Z�R�[�h</param>
    ''' <param name="TARGET_TABLE">�ޔ����e�[�u����</param>
    ''' <param name="SINQBK_TABLE">�ޔ��e�[�u����</param>
    ''' <param name="KEY_NAME">�L�[��</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:�ޔ𐬌� FALSE:�ޔ����s</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_MASTtoSINQBK(ByVal GAKKOU_CODE As String, _
                                        ByVal TARGET_TABLE As String, _
                                        ByVal SINQBK_TABLE As String, _
                                        ByVal KEY_NAME As String, _
                                        ByRef db As MyOracle) As Boolean

        Dim SYORI_NAME As String = ""

        Dim SQL As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)
        Dim bRet As Boolean = False
        Dim DEL_CNT As Long = 0
        Dim INS_CNT As Long = 0
        Dim ResultMsg As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)�J�n", "����", TARGET_TABLE)

        '-----------------------------------------------
        ' �i���p�e�[�u�����̍폜
        '-----------------------------------------------
        Try
            SYORI_NAME = "�i���p�}�X�^�폜"

            With SQL
                .Append("DELETE FROM " & SINQBK_TABLE)
                .Append(" WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            DEL_CNT = db.ExecuteNonQuery(SQL)
            If DEL_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)�I��", "���s", SYORI_NAME & "�������s �w�Z�R�[�h�F" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)�I��", "���s", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        '-----------------------------------------------
        ' �i���p�e�[�u���ւ̑ޔ�
        '-----------------------------------------------
        Try
            SYORI_NAME = "�}�X�^�ޔ�"

            SQL.Length = 0
            With SQL
                .Append("INSERT INTO " & SINQBK_TABLE)
                .Append(" SELECT  * FROM " & TARGET_TABLE & " WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            INS_CNT = db.ExecuteNonQuery(SQL)
            If INS_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)�I��", "���s", SYORI_NAME & "�������s �w�Z�R�[�h�F" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)�I��", "���s", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        ResultMsg = String.Format("{0} ���[�N���R�[�h�폜����={1}�� �ޔ�����={2}��", TARGET_TABLE, DEL_CNT.ToString, INS_CNT.ToString)
        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)�I��", "����", ResultMsg)
        Return True
    End Function
    '2017/04/19 �^�X�N�j���� ADD �W���ŏC���i�i���߂������Ή��j------------------------------------ END

#End Region

    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 �w�Z�R�[�h�[������
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            Select Case (Trim(txtGAKKOU_CODE.Text))
                Case "9".PadLeft(10, "9"c)
                    lab�w�Z��.Text = "���ׂĂ̊w�Z���Ώۂł�"
                Case Else
                    If PFUNC_GAKNAME_GET() = False Then
                        Exit Sub
                    End If
            End Select
        End If

    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z����
        Call GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName)

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

    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
