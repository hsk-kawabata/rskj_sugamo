Option Explicit On
Option Strict Off

Imports Microsoft.VisualBasic
Imports System.Text
Imports System.Data.OracleClient
Imports CASTCommon
Public Class KFGMAST021

    Public KFGMAST020_KCoad As String
    Public KFGMAST020_GCoad As String
    Private MainDB As CASTCommon.MyOracle

    Private Structure TAKOMAST_INF
        Dim TORIS_CODE As String        '10
        Dim TORIF_CODE As String        '2
        Dim TKIN_NO As String           '4
        Dim TSIT_NO As String           '3
        Dim ITAKU_CODE As String        '10
        Dim KAMOKU As String            '2
        Dim LABEL_KBN As String         '1
        Dim KOUZA As String             '7
        Dim BAITAI_CODE As String       '2
        Dim CODE_KBN As String          '1
        Dim SFILE_NAME As String        '26
        Dim RFILE_NAME As String        '26
        Dim SAKUSEI_DATE As String
        Dim KOUSIN_DATE As String
    End Structure
    Private memData As TAKOMAST_INF

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST021", "�w�Z���s�}�X�^�����e�i���X�ڍ׉��")
    Private Const msgTitle As String = "�w�Z���s�}�X�^�����e�i���X�ڍ׉��(KFGMAST021)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
#Region " Form Load "
    Private Sub KFGMAST021_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = True
        End With
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            STR_SYORI_NAME = "���s�}�X�^�o�^"
            STR_COMMAND = "Form_Load"
            STR_LOG_GAKKOU_CODE = KFGMAST020_GCoad
            STR_LOG_FURI_DATE = ""

            '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_KAMOKU_TXT, cmbKamoku) = False Then

                MessageBox.Show(String.Format(MSG0013E, "�Ȗ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Sub
            End If

            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_TAKO_CODE_TXT, cmbCodeKbn) = False Then

                Call MessageBox.Show(String.Format(MSG0013E, "�R�[�h�敪"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Sub
            End If

            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_TAKO_BAITAI_TXT, cmbBaitai) = False Then

                Call MessageBox.Show(String.Format(MSG0013E, "�}�̃R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Sub
            End If

            Call Set_Format(1)

            If txtKinCode.Text = "" Then
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
            End If

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)

        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        Try
            MainLOG.Write(LW.ToriCode, LW.FuriDate, "(�o�^)�J�n", "����")
            '���̓`�F�b�N
            If Check() = False Then
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            '���Z�@�փ}�X�^�ɓo�^����Ă����񂩃`�F�b�N
            STR_SQL = " SELECT * FROM TENMAST"
            STR_SQL += " WHERE"
            STR_SQL += " KIN_NO_N ='" & Trim(txtKinCode.Text) & "'"
            STR_SQL += " AND"
            STR_SQL += " SIT_NO_N ='" & Trim(txtSitCode.Text) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(MSG0096W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '���s�}�X�^�ɓo�^����Ă����񂩃`�F�b�N
            STR_SQL = " SELECT * FROM G_TAKOUMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_V ='" & KFGMAST020_GCoad & "'"
            STR_SQL += " AND"
            STR_SQL += " TKIN_NO_V ='" & Trim(txtKinCode.Text) & "'"

            If GFUNC_ISEXIST(STR_SQL) = True Then
                MessageBox.Show(MSG0122W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            Else
                '2013/12/24 saitou �W���� ���쐫���� ADD -------------------------------------------------->>>>
                If Me.txtKinCode.Text = CASTCommon.GetFSKJIni("COMMON", "KINKOCD") Then
                    MessageBox.Show(MSG0323W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtKinCode.Focus()
                    Return
                End If
                '2013/12/24 saitou �W���� ���쐫���� ADD --------------------------------------------------<<<<
            End If

            '2013/12/24 saitou �W���� ���쐫���� DEL -------------------------------------------------->>>>
            '���ł�20���ȏ�o�^����Ă����炱�̉�ʂɑJ�ڂł��Ȃ��̂ŕs�v�`�F�b�N
            ''���s��20���o�^����Ă��Ȃ����`�F�b�N
            'STR_SQL = " SELECT count(*) as kensuu FROM G_TAKOUMAST"
            'STR_SQL += " WHERE"
            'STR_SQL += " GAKKOU_CODE_V ='" & KFGMAST020_GCoad & "'"

            'If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            '    Exit Sub
            'End If
            'With OBJ_DATAREADER
            '    OBJ_DATAREADER.Read()
            '    If 20 <= .Item("kensuu") Then
            '        Call MessageBox.Show("���s�f�[�^�����ł�20�����݂��܂��B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        GFUNC_SELECT_SQL2("", 1)
            '        Exit Sub
            '    End If
            '    GFUNC_SELECT_SQL2("", 1)
            'End With
            '2013/12/24 saitou �W���� ���쐫���� DEL --------------------------------------------------<<<<

            '��Ǝ��U���̑��s�}�X�^�`�F�b�N
            KFGMAST020_KCoad = txtKinCode.Text

            FN_MOVE_TAKOUMAST()

            If FN_SELECT_TAKOUMAST() > 0 Then

                MessageBox.Show(MSG0122W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinCode.Focus()
                Return
            End If

            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '�o�^����
            MainDB.BeginTrans()   '�g�����U�N�V�����J�n

            If DB_Insert() = False Then
                MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "�o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If FN_INSERT_TAKOUMAST() Then

                ''''''���ʁE�����i�����E�o���j�͍쐬���Ȃ��B
                '���R�[�h02�`04�̕����o�^
                If Not FN_INSERT_TAKOUMAST_FUKU() Then
                    MainDB.Rollback()
                    MessageBox.Show(String.Format(MSG0002E, "���R�[�h���o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Return
                End If

            Else
                MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "�o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            '**********************************************************************************************

            MainDB.Commit()

            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '2006/10/24�@�o�^��A�Q�Ǝ��Ɠ�����Ԃ̉�ʂɐݒ�
            txtKinCode.Enabled = False
            btnAdd.Enabled = False
            btnUPDATE.Enabled = True
            btnDelete.Enabled = True
        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try


    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click

        'Dim MSG As String
        'Dim DRet As DialogResult
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")
            '���̓`�F�b�N
            If Check() = False Then
                Exit Sub
            End If
            MainDB = New CASTCommon.MyOracle
            '���Z�@�փ}�X�^�ɓo�^����Ă����񂩃`�F�b�N
            STR_SQL = " SELECT * FROM TENMAST"
            STR_SQL += " WHERE"
            STR_SQL += " KIN_NO_N ='" & Trim(txtKinCode.Text) & "'"
            STR_SQL += " AND"
            STR_SQL += " SIT_NO_N ='" & Trim(txtSitCode.Text) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(MSG0096W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '���s�}�X�^�ɓo�^����Ă����񂩃`�F�b�N
            STR_SQL = " SELECT * FROM G_TAKOUMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_V ='" & KFGMAST020_GCoad & "'"
            STR_SQL += " AND"
            STR_SQL += " TKIN_NO_V ='" & Trim(txtKinCode.Text) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then

                MessageBox.Show(MSG0070W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            MainDB.BeginTrans() '�g�����U�N�V�����J�n

            If DB_Update() = False Then
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            KFGMAST020_KCoad = txtKinCode.Text
            '**********************************************************************************************
            '��������d�l update 2008/02/15 
            '�o�^���Ɋ�Ǝ��U���̑��s�}�X�^�ɍX�V����B

            FN_MOVE_TAKOUMAST()

            Dim SQL As String = "DELETE FROM TAKOUMAST"
            SQL &= " WHERE TORIS_CODE_V = '" & KFGMAST020_GCoad & "'"
            SQL &= "AND TKIN_NO_V = '" & memData.TKIN_NO & "'"

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
            If Ret < 0 Then
                MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If


            If FN_INSERT_TAKOUMAST() Then

                ''''''���ʁE�����i�����E�o���j�͍쐬���Ȃ��B
                '���R�[�h02�`04�̕����o�^
                If Not FN_INSERT_TAKOUMAST_FUKU() Then
                    MainDB.Rollback()
                    MessageBox.Show(String.Format(MSG0002E, "���R�[�h���o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            Else
                MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "�o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '**********************************************************************************************
            MainDB.Commit()

            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try


    End Sub
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Dim MSG As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�J�n", "����", "")
            MainDB = New CASTCommon.MyOracle
            If Trim(txtKinCode.Text) = "" Then
                MessageBox.Show(String.Format(MSG0281W, "���Z�@�փR�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            Call FN_MOVE_TAKOUMAST()

            MainDB.BeginTrans()

            If DB_Delete() = False Then
                MainDB.Rollback()
                MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '�o�^���Ɋ�Ǝ��U���̑��s�}�X�^�폜����B

            Dim SQL As String = "DELETE FROM TAKOUMAST"
            SQL &= " WHERE TORIS_CODE_V = '" & KFGMAST020_GCoad & "'"
            SQL &= "AND TKIN_NO_V = '" & memData.TKIN_NO & "'"

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
            If Ret > -1 Then
                MainDB.Commit()
                MSG = MSG0008I
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '2006/10/24�@�폜��A�V�K�o�^���Ɠ�����Ԃ̉�ʂɐݒ�
            txtKinCode.Enabled = True
            btnAdd.Enabled = True
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False

            '������
            Call Set_Default()
        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            '������
            Call Set_Default()
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)�I��", "����", "")
        End Try
    End Sub
#End Region
    Private Sub txtKinCode_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtKinCode.Validating
        '2013/12/24 saitou �W���� ���쐫���� UPD -------------------------------------------------->>>>
        '��������Ǝ��U�ɍ��킹��
        Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
        If Temp.Length = 0 Then
            Me.txtKinCode.Text = String.Empty
            Me.lblKIN_NM.Text = String.Empty
        Else
            Call GCom.NzNumberString(CType(sender, TextBox), True)
            Me.lblKIN_NM.Text = GCom.GetBKBRName(Me.txtKinCode.Text, "", 40)
            If Me.txtSitCode.Text.Trim <> "" Then
                Me.lblSIT_NM.Text = GCom.GetBKBRName(Me.txtKinCode.Text, Me.txtSitCode.Text, 40)
            End If
        End If
        'If Trim(txtKinCode.Text) <> "" Then
        '    Call Set_LabelText()
        'Else
        '    '��s�������󔒂�
        '    lblKIN_NM.Text = ""
        '    '�x�X�������󔒂�
        '    lblSIT_NM.Text = ""
        'End If
        '2013/12/24 saitou �W���� ���쐫���� UPD --------------------------------------------------<<<<
    End Sub
    Private Sub txtSitCode_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSitCode.Validating
        '2013/12/24 saitou �W���� ���쐫���� UPD -------------------------------------------------->>>>
        '��������Ǝ��U�ɍ��킹��
        Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
        If Temp.Length = 0 Then
            Me.txtSitCode.Text = String.Empty
            Me.lblSIT_NM.Text = String.Empty
        Else
            Call GCom.NzNumberString(CType(sender, TextBox), True)
            Me.lblKIN_NM.Text = GCom.GetBKBRName(Me.txtKinCode.Text, "", 40)
            Me.lblSIT_NM.Text = GCom.GetBKBRName(Me.txtKinCode.Text, Me.txtSitCode.Text, 40)
        End If
        'If Trim(txtSitCode.Text) <> "" Then
        '    Call Set_LabelText()
        'Else
        '    '�x�X�������󔒂�
        '    lblSIT_NM.Text = ""
        'End If
        '2013/12/24 saitou �W���� ���쐫���� UPD --------------------------------------------------<<<<
    End Sub


#Region " Private Sub "
    Private Sub Set_Format(ByVal pIndex As Integer)

        Call Set_Default()

        '�l���ē���
        txtKinCode.Text = KFGMAST020_KCoad

        If txtKinCode.Text = "" Then
            Exit Sub
        End If

        Select Case pIndex
            Case 0
                Exit Sub
        End Select

        '��񒊏o
        STR_SQL = " SELECT * FROM "
        STR_SQL += " G_TAKOUMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_V = '" & KFGMAST020_GCoad & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_V = '" & txtKinCode.Text & "'"
        'STR_SQL += " AND"
        'STR_SQL += " TSIT_NO_V = '" & txtSitCode.Text & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Sub
        End If

        OBJ_DATAREADER.Read()

        With OBJ_DATAREADER
            '�x�X�R�[�h
            txtSitCode.Text = Trim(.Item("TSIT_NO_V"))

            '�ȖڃR�[�h
            cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_KAMOKU_TXT, .Item("KAMOKU_V"))

            '�����ԍ�
            txtKouza.Text = Trim(.Item("KOUZA_V"))

            '�ϑ��҃R�[�h
            txtItakuCode.Text = Trim(.Item("ITAKU_CODE_V"))

            '�}�̃R�[�h
            cmbBaitai.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_TAKO_BAITAI_TXT, .Item("BAITAI_CODE_V"))

            '�R�[�h�敪
            cmbCodeKbn.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_TAKO_CODE_TXT, .Item("CODE_KBN_V"))

            '���M�t�@�C����
            If IsDBNull(.Item("SFILE_NAME_V")) = False Then
                txtSousinFile.Text = Trim(.Item("SFILE_NAME_V"))
            End If

            If IsDBNull(.Item("RFILE_NAME_V")) = False Then
                '��M�t�@�C����
                txtJyusinFile.Text = Trim(.Item("RFILE_NAME_V"))
            End If
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Sub
        End If

        '2006/10/24�@�Q�Ǝ��A�V�K�o�^�ł��Ȃ��悤�ɐݒ�
        txtKinCode.Enabled = False
        btnAdd.Enabled = False

        Call Set_LabelText()
    End Sub
    Private Sub Set_LabelText()

        '���Z�@�փR�[�h�Ǝx�X�R�[�h������Z�@�֖��A�x�X���𒊏o
        lblKIN_NM.Text = ""
        lblSIT_NM.Text = ""

        If Trim(txtKinCode.Text) = "" Or Trim(txtSitCode.Text) = "" Then
            Exit Sub
        End If

        STR_SQL = " SELECT KIN_NNAME_N , SIT_NNAME_N  FROM TENMAST "
        STR_SQL += " WHERE"
        STR_SQL += " KIN_NO_N = '" & txtKinCode.Text.Trim.PadLeft(txtKinCode.MaxLength, "0"c) & "'"
        STR_SQL += " AND"
        STR_SQL += " SIT_NO_N = '" & txtSitCode.Text.Trim.PadLeft(txtSitCode.MaxLength, "0"c) & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If
            txtSitCode.Text = ""
            lblSIT_NM.Text = ""
            Call MessageBox.Show(MSG0096W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSitCode.Focus()
            Exit Sub
        End If

        OBJ_DATAREADER.Read()

        lblKIN_NM.Text = OBJ_DATAREADER.Item("KIN_NNAME_N")
        lblSIT_NM.Text = OBJ_DATAREADER.Item("SIT_NNAME_N")

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Sub
        End If

    End Sub
    Private Function DB_Delete() As Boolean
        Dim Str_OleSql As String
        Try
            DB_Delete = False
            Str_OleSql = "DELETE  FROM G_TAKOUMAST "
            Str_OleSql += " WHERE GAKKOU_CODE_V = '" & KFGMAST020_GCoad & "'"
            Str_OleSql += " AND TKIN_NO_V = '" & Trim(txtKinCode.Text).PadLeft(4, "0"c) & "'"

            If MainDB.ExecuteNonQuery(Str_OleSql) <> 1 Then
                '�G���[���b�Z�[�W�̕\��
                Call MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            DB_Delete = True
        Catch ex As Exception
            Throw New Exception(ex.ToString, ex)
        End Try

    End Function
    Private Function DB_Insert() As Boolean

        Dim str_date As String
        DB_Insert = False
        Try
            str_date = Format(Now.Year, "0000") & Format(Now.Month, "00") & Format(Now.Day, "00")

            STR_SQL = " INSERT INTO G_TAKOUMAST"
            STR_SQL += " values("
            STR_SQL += " '" & KFGMAST020_GCoad & "'"
            STR_SQL += ",'" & Trim(txtKinCode.Text).PadLeft(4, "0"c) & "'"
            STR_SQL += ",'" & Trim(txtSitCode.Text).PadLeft(3, "0"c) & "'"
            STR_SQL += ",'" & txtItakuCode.Text & "'"
            STR_SQL += ",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAMOKU_TXT, cmbKamoku) & "'"
            STR_SQL += ",'" & txtKouza.Text & "'"
            STR_SQL += ",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TAKO_BAITAI_TXT, cmbBaitai) & "'"
            STR_SQL += ",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TAKO_CODE_TXT, cmbCodeKbn) & "'"
            STR_SQL += ",'" & Trim(txtSousinFile.Text) & "'"
            STR_SQL += ",'" & Trim(txtJyusinFile.Text) & "'"
            STR_SQL += ",'" & str_date & "'"
            STR_SQL += ",'00000000'"
            STR_SQL += ") "

            If MainDB.ExecuteNonQuery(STR_SQL) < 0 Then
                '�G���[���b�Z�[�W�̕\��
                Call MessageBox.Show(String.Format(MSG0002E, "�}��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Function
            End If
            DB_Insert = True
        Catch ex As Exception

        End Try
    End Function
    Private Function DB_Update() As Boolean

        Dim Str_OleSql As String
        Dim str_date As String
        DB_Update = False
        Try
            str_date = Format(Now.Year, "0000") & Format(Now.Month, "00") & Format(Now.Day, "00")

            Str_OleSql = "UPDATE  G_TAKOUMAST set "
            Str_OleSql += " TKIN_NO_V = '" & Trim(txtKinCode.Text).PadLeft(4, "0"c) & "', "
            Str_OleSql += " TSIT_NO_V = '" & Trim(txtSitCode.Text).PadLeft(3, "0"c) & "', "
            Str_OleSql += " ITAKU_CODE_V = '" & txtItakuCode.Text & "' , "
            Str_OleSql += " KAMOKU_V = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAMOKU_TXT, cmbKamoku) & "' , "

            Str_OleSql += " KOUZA_V = '" & txtKouza.Text & "' , "
            Str_OleSql += " BAITAI_CODE_V = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TAKO_BAITAI_TXT, cmbBaitai) & "' , "

            Str_OleSql += " CODE_KBN_V = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TAKO_CODE_TXT, cmbCodeKbn) & "' , "

            Str_OleSql += " SFILE_NAME_V = '" & txtSousinFile.Text & "' , "
            Str_OleSql += " RFILE_NAME_V = '" & txtJyusinFile.Text & "' , "
            Str_OleSql += " KOUSIN_DATE_V = '" & str_date & "'"
            Str_OleSql += " WHERE GAKKOU_CODE_V = '" & KFGMAST020_GCoad & "'"
            Str_OleSql += " AND TKIN_NO_V = '" & KFGMAST020_KCoad & "'"

            If MainDB.ExecuteNonQuery(Str_OleSql) < 0 Then
                '�G���[���b�Z�[�W�̕\��
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
            DB_Update = True
        Catch ex As Exception

        End Try


    End Function
    Private Sub Set_Default()

        '�R���{�������ʒu�ɐݒ�
        '�ȖڃR�[�h
        If cmbKamoku.Items.Count() >= 0 Then
            cmbKamoku.SelectedIndex = 0
        End If

        '�ȖڃR�[�h
        If cmbBaitai.Items.Count() >= 0 Then
            cmbBaitai.SelectedIndex = 0
        End If

        '�R�[�h�敪
        If cmbCodeKbn.Items.Count() >= 0 Then
            cmbCodeKbn.SelectedIndex = 0
        End If

        '�e�L�X�g�{�b�N�X���󔒂ɂ���
        '2006/10/24�@�Q�Ǝ��ł���΁A����ɃR�[�h�E����ɖ��͋󔒂ɂ��Ȃ�
        If txtKinCode.Enabled = True Then
            txtKinCode.Text = ""
            lblKIN_NM.Text = ""
        End If

        'txtKinCode.Text = ""
        txtSitCode.Text = ""
        txtKouza.Text = ""
        txtItakuCode.Text = ""
        txtSousinFile.Text = ""
        txtJyusinFile.Text = ""

        'lblKIN_NM.Text = ""
        lblSIT_NM.Text = ""
    End Sub

#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function Check() As Boolean
        Dim intFname_Count As Integer = 0


        Check = False

        '2013/12/24 saitou �W���� ���쐫���� UPD -------------------------------------------------->>>>
        '�`�F�b�N���@����Ǝ��U�ɍ��킹��B
        Try
            Dim Temp As String
            If Me.lblKIN_NM.Text = "" Then
                MessageBox.Show(String.Format(MSG0281W, "���Z�@�փR�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinCode.Focus()
                Exit Function
            End If

            If Me.lblSIT_NM.Text = "" Then
                MessageBox.Show(String.Format(MSG0281W, "�x�X�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSitCode.Focus()
                Exit Function
            End If

            If Me.cmbKamoku.SelectedIndex = -1 Then
                MessageBox.Show(String.Format(MSG0281W, "�Ȗ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.cmbKamoku.Focus()
                Exit Function
            End If

            Temp = GCom.NzDec(Me.txtKouza.Text, "")
            If Temp.Length = 0 Then
                MessageBox.Show(String.Format(MSG0281W, "�����ԍ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKouza.Focus()
                Exit Function
            End If

            Temp = GCom.NzDec(Me.txtItakuCode.Text, "")
            If Temp.Length = 0 Then
                MessageBox.Show(String.Format(MSG0281W, "�ϑ��҃R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtItakuCode.Focus()
                Exit Function
            End If

            If Me.cmbBaitai.SelectedIndex = -1 Then
                MessageBox.Show(String.Format(MSG0281W, "�}�̃R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.cmbBaitai.Focus()
                Exit Function
            End If

            If Me.cmbCodeKbn.SelectedIndex = -1 Then
                MessageBox.Show(String.Format(MSG0281W, "�R�[�h�敪"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.cmbCodeKbn.Focus()
                Exit Function
            End If

            '�t�@�C����
            '2007/01/17�@�S�p�����ɑΉ�
            If txtSousinFile.Text.Trim <> "" Then
                For i As Integer = 1 To txtSousinFile.Text.Length
                    '�A�X�L�[�R�[�h�����A�S�p�E���p�����𔻒�
                    If Asc(Mid(txtSousinFile.Text, i)) < 0 Or Asc(Mid(txtSousinFile.Text, i)) > 255 Then
                        intFname_Count += 2 '�S�p
                    Else
                        intFname_Count += 1 '���p
                    End If
                Next
                If intFname_Count > 26 Then
                    Call MessageBox.Show(G_MSG0062W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSousinFile.Focus()
                    txtSousinFile.SelectionStart = 0
                    txtSousinFile.SelectionLength = txtSousinFile.Text.Length
                    Exit Function
                End If
            End If

            intFname_Count = 0
            If txtJyusinFile.Text.Trim <> "" Then
                For i As Integer = 1 To txtJyusinFile.Text.Length
                    '�A�X�L�[�R�[�h�����A�S�p�E���p�����𔻒�
                    If Asc(Mid(txtJyusinFile.Text, i)) < 0 Or Asc(Mid(txtJyusinFile.Text, i)) > 255 Then
                        intFname_Count += 2 '�S�p
                    Else
                        intFname_Count += 1 '���p
                    End If
                Next
                If intFname_Count > 26 Then
                    Call MessageBox.Show(G_MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtJyusinFile.Focus()
                    txtJyusinFile.SelectionStart = 0
                    txtJyusinFile.SelectionLength = txtJyusinFile.Text.Length
                    Exit Function
                End If
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        'If Trim(txtKinCode.Text) = "" Then
        '    Call MessageBox.Show("���Z�@�փR�[�h�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    txtKinCode.Focus()
        '    Exit Function
        'End If

        'If Trim(txtSitCode.Text) = "" Then
        '    Call MessageBox.Show("�x�X�R�[�h�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    txtSitCode.Focus()
        '    Exit Function
        'End If

        'If Trim(txtKinCode.Text) = Trim(STR_JIKINKO_CODE) Then
        '    Call MessageBox.Show("���Z�@�փR�[�h�F" & Trim(txtKinCode.Text) & " �͎����ɃR�[�h�Ȃ̂œo�^�ł��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    txtKinCode.Focus()
        '    Exit Function
        'End If
        ''���͋��Z�@�փ}�X�^���݃`�F�b�N
        'STR_SQL = " SELECT * FROM TENMAST "
        'STR_SQL += " WHERE KIN_NO_N ='" & Trim(txtKinCode.Text).PadLeft(4, "0"c) & "'"
        'STR_SQL += " AND SIT_NO_N ='" & Trim(txtSitCode.Text).PadLeft(3, "0"c) & "'"

        'If GFUNC_ISEXIST(STR_SQL) = False Then
        '    Call MessageBox.Show("���͂��ꂽ���Z�@�փR�[�h�A�x�X�R�[�h�̋��Z�@�ւ͑��݂��܂���ł����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    txtKinCode.Focus()

        '    Exit Function
        'End If

        'If cmbKamoku.SelectedIndex = -1 Then
        '    Call MessageBox.Show("�ȖڃR�[�h�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    cmbKamoku.Focus()
        '    Exit Function
        'End If

        'If Trim(txtKouza.Text) = "" Then
        '    Call MessageBox.Show("�����ԍ������͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    txtKouza.Focus()
        '    Exit Function
        'End If
        'If Trim(txtItakuCode.Text) = "" Then
        '    Call MessageBox.Show("�ϑ��҃R�[�h�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    txtItakuCode.Focus()
        '    Exit Function
        'End If

        'If cmbBaitai.SelectedIndex = -1 Then
        '    Call MessageBox.Show("�}�̃R�[�h�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    cmbBaitai.Focus()
        '    Exit Function
        'End If

        'If cmbCodeKbn.SelectedIndex = -1 Then
        '    Call MessageBox.Show("�R�[�h�敪�����͂���Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    cmbCodeKbn.Focus()
        '    Exit Function
        'End If

        ''�t�@�C����
        ''2007/01/17�@�S�p�����ɑΉ�
        'If txtSousinFile.Text.Trim <> "" Then
        '    For i As Integer = 1 To txtSousinFile.Text.Length
        '        '�A�X�L�[�R�[�h�����A�S�p�E���p�����𔻒�
        '        If Asc(Mid(txtSousinFile.Text, i)) < 0 Or Asc(Mid(txtSousinFile.Text, i)) > 255 Then
        '            intFname_Count += 2 '�S�p
        '        Else
        '            intFname_Count += 1 '���p
        '        End If
        '    Next
        '    If intFname_Count > 26 Then
        '        Call MessageBox.Show("���M�t�@�C�����͔��p26����(�S�p13����)�ȓ��ɐݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '        txtSousinFile.Focus()
        '        txtSousinFile.SelectionStart = 0
        '        txtSousinFile.SelectionLength = txtSousinFile.Text.Length
        '        Exit Function
        '    End If
        'End If

        'intFname_Count = 0
        'If txtJyusinFile.Text.Trim <> "" Then
        '    For i As Integer = 1 To txtJyusinFile.Text.Length
        '        '�A�X�L�[�R�[�h�����A�S�p�E���p�����𔻒�
        '        If Asc(Mid(txtJyusinFile.Text, i)) < 0 Or Asc(Mid(txtJyusinFile.Text, i)) > 255 Then
        '            intFname_Count += 2 '�S�p
        '        Else
        '            intFname_Count += 1 '���p
        '        End If
        '    Next
        '    If intFname_Count > 26 Then
        '        Call MessageBox.Show("��M�t�@�C�����͔��p26����(�S�p13����)�ȓ��ɐݒ肵�Ă��������B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '        txtJyusinFile.Focus()
        '        txtJyusinFile.SelectionStart = 0
        '        txtJyusinFile.SelectionLength = txtJyusinFile.Text.Length
        '        Exit Function
        '    End If
        'End If
        '2013/12/24 saitou �W���� ���쐫���� UPD --------------------------------------------------<<<<

        Check = True

    End Function
#End Region


    Private Sub FN_MOVE_TAKOUMAST()

        memData.TORIS_CODE = KFGMAST020_GCoad
        memData.TORIF_CODE = "01"
        memData.TKIN_NO = Trim(txtKinCode.Text).PadLeft(4, "0"c)
        memData.TSIT_NO = Trim(txtSitCode.Text).PadLeft(3, "0"c)
        memData.ITAKU_CODE = txtItakuCode.Text

        memData.KAMOKU = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAMOKU_TXT, cmbKamoku)

        memData.KOUZA = txtKouza.Text
        memData.BAITAI_CODE = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TAKO_BAITAI_TXT, cmbBaitai)

        'update 2008/03/04
        memData.BAITAI_CODE = Trim(memData.BAITAI_CODE).PadLeft(2, "0"c)

        memData.CODE_KBN = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TAKO_CODE_TXT, cmbCodeKbn)
        memData.SFILE_NAME = Trim(txtSousinFile.Text)
        memData.RFILE_NAME = Trim(txtJyusinFile.Text)
        memData.SAKUSEI_DATE = String.Format("{0:yyyy.MM.dd}", Date.Now)
        memData.KOUSIN_DATE = "00000000"
        memData.LABEL_KBN = "0"
    End Sub
    '
    ' �@�\�@ �F ���s�}�X�^�E�`�F�b�N
    '
    ' �����@ �F 
    '
    ' �߂�l �F ���ꃌ�R�[�h�̐�
    '
    ' ���l�@ �F ���ꃌ�R�[�h�̗L��
    '
    Private Function FN_SELECT_TAKOUMAST() As Integer
        Dim REC As OracleDataReader = Nothing
        Dim Ret As Integer = 0
        Try
            Dim SQL As String = "SELECT * FROM TAKOUMAST"
            SQL &= " WHERE TKIN_NO_V = '" & memData.TKIN_NO & "'"
            SQL &= " AND TORIS_CODE_V = '" & memData.TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_V = '" & memData.TORIF_CODE & "'"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read
                    Ret += 1
                Loop
            End If
        Catch ex As Exception
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Ret
    End Function
    '============================================================================
    'NAME           :FN_INSERT_TAKOUMAST
    'Parameter      :���s�}�X�^�\����
    'Description    :�Z�b�g�����l�𑼍s���Z�@�փ}�X�^�ɓo�^����
    'Return         :True=OK,False=NG
    'Create         :
    'Update         :
    '============================================================================
    Private Function FN_INSERT_TAKOUMAST() As Boolean
        Try
            Dim SQL As String = "INSERT INTO TAKOUMAST"
            SQL &= " (TORIS_CODE_V"
            SQL &= ", TORIF_CODE_V"
            SQL &= ", TKIN_NO_V"
            SQL &= ", TSIT_NO_V"
            SQL &= ", ITAKU_CODE_V"
            SQL &= ", KAMOKU_V"
            SQL &= ", KOUZA_V"
            SQL &= ", BAITAI_CODE_V"
            SQL &= ", LABEL_KBN_V"
            SQL &= ", CODE_KBN_V"
            SQL &= ", SFILE_NAME_V"
            SQL &= ", RFILE_NAME_V"
            SQL &= ", SAKUSEI_DATE_V"
            SQL &= ", KOUSIN_DATE_V"
            SQL &= ") VALUES"
            With memData
                SQL &= " ('" & .TORIS_CODE & "'"
                SQL &= ", '" & .TORIF_CODE & "'"
                SQL &= ", '" & .TKIN_NO & "'"
                SQL &= ", '" & .TSIT_NO & "'"
                SQL &= ", '" & .ITAKU_CODE & "'"
                SQL &= ", '" & .KAMOKU & "'"
                SQL &= ", '" & .KOUZA & "'"
                SQL &= ", '" & .BAITAI_CODE & "'"
                SQL &= ", '" & .LABEL_KBN & "'"
                SQL &= ", '" & .CODE_KBN & "'"
                SQL &= ", '" & .SFILE_NAME & "'"
                SQL &= ", '" & .RFILE_NAME & "'"
                SQL &= ", TO_CHAR(SYSDATE, 'yyyymmdd')"
                SQL &= ", '00000000')"
            End With

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
            If Ret <> 1 Then
                Exit Try
            End If
        Catch ex As Exception
        End Try
        Return True
    End Function
    '============================================================================
    'NAME           :FN_INSERT_TAKOUMAST_FUKU
    'Parameter      :���s�}�X�^�\����, ��Ӑ���ᔽ���̑Ή�
    'Description    :�Z�b�g�����l�𑼍s���Z�@�փ}�X�^�ɓo�^����(���R�[�h02�`04��)
    'Return         :True=OK,False=NG
    'Create         :2006.01.30
    'Update         :2007.12.20 By Astar
    '============================================================================
    Private Function FN_INSERT_TAKOUMAST_FUKU(Optional ByVal ErrorExit As Boolean = True) As Boolean
        Try
            For Cnt As Integer = 2 To 4 Step 1

                Dim SQL As String = "INSERT INTO TAKOUMAST"
                SQL &= " (TORIS_CODE_V"
                SQL &= ", TORIF_CODE_V"
                SQL &= ", TKIN_NO_V"
                SQL &= ", TSIT_NO_V"
                SQL &= ", ITAKU_CODE_V"
                SQL &= ", KAMOKU_V"
                SQL &= ", KOUZA_V"
                SQL &= ", BAITAI_CODE_V"
                SQL &= ", LABEL_KBN_V"
                SQL &= ", CODE_KBN_V"
                SQL &= ", SFILE_NAME_V"
                SQL &= ", RFILE_NAME_V"
                SQL &= ", SAKUSEI_DATE_V"
                SQL &= ", KOUSIN_DATE_V"
                SQL &= ") VALUES"

                'memData.ITAKU_CODE = memData.ITAKU_CODE.Substring(0, 9) & Cnt - 1

                With memData
                    SQL &= " ('" & .TORIS_CODE & "'"
                    SQL &= ", '" & Cnt.ToString.PadLeft(2, "0"c) & "'"
                    SQL &= ", '" & .TKIN_NO & "'"
                    SQL &= ", '" & .TSIT_NO & "'"
                    SQL &= ", '" & .ITAKU_CODE & "'"
                    SQL &= ", '" & .KAMOKU & "'"
                    SQL &= ", '" & .KOUZA & "'"
                    SQL &= ", '" & .BAITAI_CODE & "'"
                    SQL &= ", '" & .LABEL_KBN & "'"
                    SQL &= ", '" & .CODE_KBN & "'"
                    SQL &= ", '" & .SFILE_NAME & "'"
                    SQL &= ", '" & .RFILE_NAME & "'"
                    SQL &= ", TO_CHAR(SYSDATE, 'yyyymmdd')"
                    SQL &= ", '00000000')"
                End With

                Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
                If Ret <> 1 Then
                    Exit Try
                End If
            Next Cnt

            Return True
        Catch ex As Exception
        End Try
        Return False
    End Function

End Class
