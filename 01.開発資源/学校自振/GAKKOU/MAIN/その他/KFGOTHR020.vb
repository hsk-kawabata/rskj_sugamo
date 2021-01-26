Option Explicit Off
Option Strict Off

Public Class KFGOTHR020

#Region " ���ʕϐ���` "
    'Dim STR��Ǝ��U�A�g_INI As String
    Dim LNG�Ǎ����� As Long
    Dim STR�w�Z�R�[�h(9) As String
    Dim STR���w�N�x(9) As String
    Dim INT�ʔ�(9) As Integer
    Dim STR���k�����J�i(9) As String
    Dim STR���k��������(9) As String
    Dim STR������ As String

    '2005/05/11
    Dim strSEITO_NENDO As String
    Dim strSEITO_TUUBAN As String
    '2006/10/13
    Public strTAKOU_FLG As String = "0"
    Public update_GAKUNEN As String = "0"

    Public UPD_SCH_KBN As String
    Public UPD_FURI_KBN As String
    Public UPD_FURI_DATE As String
    Public UPD_SFURI_DATE As String

    '2007/09/27
    Dim strJUYOUKA_NO As String

#End Region

#Region " Form Load "
    Private Sub KFGOTHR020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = False
        End With

        STR������ = "�U�֌��ʕύX"

        STR_SYORI_NAME = "�U�֌��ʕύX"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
        Call GSUB_CONNECT()
        '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

        '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) = False Then
            Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmbFURIKUBUN)")
            Exit Sub
        End If

        '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku) = False Then
            Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmbKamoku)")
            Exit Sub
        End If

        '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_FURIKETUCODE_TXT, cmbFuriketu) = False Then
            Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmbFuriketu)")
            Exit Sub
        End If

        '�w�Z�R���{�ݒ�i�S�w�Z�j
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")

            Call GSUB_MESSAGE_WARNING("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���")

            Exit Sub
        End If

        '�w�Z�w��̏ꍇ
        If rdbgakkouSeq.Checked = True Then
            '���Z�@�փR�[�h����U�֌��ʃR�[�h�i�O���[�v�P�j�̕\���}�~
            GroupBox1.Visible = False
            '�w�N�R�[�h
            txtGAKUNEN.Visible = True
            '"-"
            lbl1.Visible = True
            '�N���X�R�[�h
            txtCLASS.Visible = True
            '"-"
            lbl2.Visible = True
            '���k�ԍ�
            txtSEITONO.Visible = True

        End If

        '���̓{�^������
        btnAction.Enabled = False
        btnSansyou.Enabled = True
        btnEraser.Enabled = True
        btnEnd.Enabled = True

        'Oracle �ڑ�(Read��p)
        OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read��p)
        OBJ_CONNECTION_DREAD.Open()

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        '�X�V�{�^��
        STR_COMMAND = "�X�V"
        STR_LOG_GAKKOU_CODE = txtGAKKOU_CODE.Text
        STR_LOG_FURI_DATE = txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text

        '�U�֌��ʃR���{�I���`�F�b�N
        If cmbFuriketu.SelectedIndex < 0 Then
            Call GSUB_MESSAGE_WARNING("�U�֌��ʂ��I������Ă��܂���")
            '�U�֌��ʃR���{�ɃJ�[�\���ݒ�
            cmbFuriketu.Focus()

            Exit Sub
        End If

        If GFUNC_MESSAGE_QUESTION("�X�V���܂����H") <> vbOK Then
            Exit Sub
        End If

        '���S�M������ ���S�M���͑��s������ 2007/09/27
        ''���s�쐬�Ώې�t���O�擾 2006/10/13
        'If PFUC_TAKOUFLG_GET() = 1 Then
        '    Call GSUB_MESSAGE_WARNING( "�w�Z�����Ɏ��s���܂���")
        '    txtGAKKOU_CODE.Focus()
        '    txtGAKKOU_CODE.SelectionStart = 0
        '    txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
        '    Exit Sub
        'End If

        '�g�����U�N�V�����J�n
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Sub
        End If

        ''�����ԍ��w��̏ꍇ
        'If rdbKouza.Checked = True Then
        STR_SQL = ""
        STR_SQL += "UPDATE  KZFMAST.G_MEIMAST SET "
        '�U�֌��ʃR�[�h
        STR_SQL += " FURIKETU_CODE_M = " & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKETUCODE_TXT, cmbFuriketu)
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M = '" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M = '" & txtKinyuuCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TSIT_NO_M = '" & txtSitCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TKAMOKU_M = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKOUZA_M = '" & txtKouzaBan.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " RECORD_NO_M = " & CLng(txtRECNO.Text)
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_KIN_M = " & CLng(txtKingaku.Text)

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '�X�V�����G���[
            GSUB_MESSAGE_WARNING("���׃}�X�^�̍X�V�ŃG���[���������܂����B")
            '�g���L����
            GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)

            Call GSUB_LOG(0, "���׃}�X�^�̍X�V")

            Exit Sub
        End If

        '��Ǝ��U���׃}�X�^�X�V
        If PFUNC_KMEISAI_UPD() = False Then
            '�g���L����
            GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
            Call GSUB_LOG(0, "��Ǝ��U���׃}�X�^�X�V")

            Exit Sub
        End If

        '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Sub
        End If

        '------------------------------------------------------------
        '�X�P�W���[���̐U�֌����E���z�A�s�\�����E���z�̍X�V(2005/05/11)
        '------------------------------------------------------------
        '�g�����U�N�V�����J�n
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Sub
        End If
        If PFUNC_GSCHAMST_UPD() = False Then
            '�g���L����
            GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
            Exit Sub
        End If
        '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Sub
        End If

        '���S�M������ ���sSCHMAST�͊�Ƒ��ň��� 2007/09/27
        'If strTAKOU_FLG = "1" Then '���s�쐬�Ώې�̂�
        '    '------------------------------------------------------------
        '    '���s�X�P�W���[���̐U�֌����E���z�A�s�\�����E���z�̍X�V(2006/10/13)
        '    '------------------------------------------------------------
        '    '�g�����U�N�V�����J�n
        '    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
        '        Exit Sub
        '    End If
        '    If PFUNC_GSCHAMST_UPD_TAKOU() = False Then
        '        '�g���L����
        '        GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
        '        Exit Sub
        '    End If
        '    '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
        '    If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
        '        Exit Sub
        '    End If
        'End If

        '--------------------------------------------------------------

        Call GSUB_LOG(1, "�U�֌��ʍX�V")

        Call GSUB_MESSAGE_INFOMATION("�X�V���I�����܂���")

        '���͍��ځ@�}�~�̉���
        PSUB_INPUTEnabled_True()

        txtGAKKOU_CODE.Focus()

        '���̓{�^������
        btnAction.Enabled = False
        btnSansyou.Enabled = True
        btnEraser.Enabled = True
        btnEnd.Enabled = True

    End Sub
    Private Sub btnSansyou_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyou.Click
        '�Q�ƃ{�^��

        '���͍��ڂ̋��ʃ`�F�b�N
        If PFUNC_COMMON_CHECK() = False Then
            Exit Sub
        End If

        '�X�P�W���[���}�X�^����
        If PFUNC_SCHMAST_GET() = False Then
            '�w�Z�R�[�h�ɃJ�[�\���ݒ�
            txtGAKKOU_CODE.Focus()
            Exit Sub
        End If

        '�����U�փf�[�^����
        If PFUNC_MEISAI_GET() = False Then
            '�w�Z�R�[�h�ɃJ�[�\���ݒ�
            txtGAKKOU_CODE.Focus()
            Exit Sub
        End If

        '�O���[�v�̈�̕\��
        GroupBox1.Visible = True

        '���͍��ڐ���
        PSUB_INPUTEnabled_False()
        cmbFuriketu.Focus()

        '���̓{�^������
        btnAction.Enabled = True
        btnSansyou.Enabled = False
        btnEraser.Enabled = True
        btnEnd.Enabled = True

    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        '����{�^��

        cmbKana.SelectedIndex = -1
        '�w�Z�R���{�ݒ�i�S�w�Z�j
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Call GSUB_LOG(0, "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")

            Call GSUB_MESSAGE_WARNING("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���")

            Exit Sub
        End If

        '���͍��ځ@�}�~�̉���
        PSUB_INPUTEnabled_True()

        '2006/10/16�@�ǉ��F�U�֓��E�����Ώی����N���A
        txtFURINEN.Text = ""
        txtFURITUKI.Text = ""
        txtFURIHI.Text = ""
        txtSEIKYUNEN.Text = ""
        txtSEIKYUTUKI.Text = ""
        cmbFURIKUBUN.SelectedIndex = 0


        txtGAKUNEN.Text = ""
        txtCLASS.Text = ""
        txtSEITONO.Text = ""
        txtKinyuuCode.Text = ""
        txtSitCode.Text = ""

        cmbKamoku.SelectedIndex = 0

        txtKouzaBan.Text = ""
        txtKingaku.Text = ""
        lab������.Text = ""
        txtKeiyaku.Text = ""
        txtRECNO.Text = ""
        cmbFuriketu.SelectedIndex = 0

        '���͍��ڐ���
        rdbgakkouSeq.Checked = True
        GroupBox1.Visible = False

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()
        txtGAKKOU_CODE.SelectionStart = 0
        txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length

        '���̓{�^������
        btnAction.Enabled = False
        btnSansyou.Enabled = True
        btnEraser.Enabled = True
        btnEnd.Enabled = True

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        If OBJ_CONNECTION_DREAD Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD.Close()
            OBJ_CONNECTION_DREAD = Nothing
        End If

        '�I���{�^��
        Me.Close()
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_INPUTEnabled_False()
        '���͍��ڂ̗}�~

        cmbKana.Enabled = False
        cmbGakkouName.Enabled = False
        txtGAKKOU_CODE.Enabled = False
        txtFURINEN.Enabled = False
        txtFURITUKI.Enabled = False
        txtFURIHI.Enabled = False
        txtSEIKYUNEN.Enabled = False
        txtSEIKYUTUKI.Enabled = False
        cmbFURIKUBUN.Enabled = False
        txtGAKUNEN.Enabled = False
        txtCLASS.Enabled = False
        txtSEITONO.Enabled = False
        txtKinyuuCode.Enabled = False
        txtKinyuuCode.Enabled = False
        txtSitCode.Enabled = False
        cmbKamoku.Enabled = False
        txtKouzaBan.Enabled = False
        txtKingaku.Enabled = False
        txtKeiyaku.Enabled = False
        txtRECNO.Enabled = False
    End Sub
    Private Sub PSUB_INPUTEnabled_True()
        '���͍��ڂ̗}�~

        cmbKana.Enabled = True
        cmbGakkouName.Enabled = True
        txtGAKKOU_CODE.Enabled = True
        txtFURINEN.Enabled = True
        txtFURITUKI.Enabled = True
        txtFURIHI.Enabled = True
        txtSEIKYUNEN.Enabled = True
        txtSEIKYUTUKI.Enabled = True
        cmbFURIKUBUN.Enabled = True
        txtGAKUNEN.Enabled = True
        txtCLASS.Enabled = True
        txtSEITONO.Enabled = True
        txtKinyuuCode.Enabled = True
        txtKinyuuCode.Enabled = True
        txtSitCode.Enabled = True
        cmbKamoku.Enabled = True
        txtKouzaBan.Enabled = True
        txtKingaku.Enabled = True
        txtKeiyaku.Enabled = True
        txtRECNO.Enabled = True
    End Sub
    Private Sub PSUB_Kanma_Set(ByVal pText As TextBox, ByVal pIndex As Integer)

        Select Case (pIndex)
            Case 0
                If pText.MaxLength = 12 Then
                    pText.MaxLength = pText.MaxLength + 3
                End If
                pText.Text = Format(CInt(pText.Text), "#,##0")
            Case 1
                If pText.MaxLength = 15 Then
                    pText.MaxLength = pText.MaxLength - 3
                End If
                pText.Text = Format(CInt(pText.Text), "###0")
        End Select
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "

    Private Function PFUC_TAKOUFLG_GET() As Integer
        PFUC_TAKOUFLG_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST2 WHERE"
        STR_SQL += "     GAKKOU_CODE_T  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        '�w�Z�}�X�^2���s�t���O�`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            GSUB_MESSAGE_WARNING("�w�Z�}�X�^�ɓo�^����Ă��܂���")
            txtGAKKOU_CODE.Focus()
            PFUC_TAKOUFLG_GET = 1
            Exit Function
        End If

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            PFUC_TAKOUFLG_GET = 1
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                '���s�敪
                strTAKOU_FLG = .Item("TAKO_KBN_T")
            End With
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            PFUC_TAKOUFLG_GET = 1
            Exit Function
        End If

        PFUC_TAKOUFLG_GET = 0

    End Function

    Private Function PFUNC_GAKNAME_GET() As Boolean

        PFUNC_GAKNAME_GET = False

        '�w�Z���̐ݒ�
        STR_SQL = " SELECT * FROM GAKMAST1"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'"

        '�w�Z�}�X�^�P���݃`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("�w�Z�}�X�^�ɓo�^����Ă��܂���")

            lab�w�Z��.Text = ""
            txtGAKKOU_CODE.Focus()

            Exit Function
        End If

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        With OBJ_DATAREADER
            .Read()

            lab�w�Z��.Text = .Item("GAKKOU_NNAME_G")
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_COMMON_CHECK() As Boolean

        PFUNC_COMMON_CHECK = False

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("�w�Z�R�[�h�̓��͂Ɍ�肪����܂�")
            txtGAKKOU_CODE.Focus()
            Exit Function
        End If
        '�U�֔N
        If Trim(txtFURINEN.Text) < "2004" Then
            Call GSUB_MESSAGE_WARNING("�U�֔N�̓��͂Ɍ�肪����܂�")
            txtFURINEN.Focus()
            Exit Function
        End If
        '�U�֌�
        If Trim(txtFURITUKI.Text) >= "01" And Trim(txtFURITUKI.Text) <= "12" Then
        Else
            Call GSUB_MESSAGE_WARNING("�U�֌��̓��͂Ɍ�肪����܂�")
            txtFURITUKI.Focus()
            Exit Function
        End If

        '�U�֓�
        '�����֘A�`�F�b�N
        Select Case (Trim(txtFURITUKI.Text))
            Case "01", "03", "05", "07", "08", "10", "12"
                If Trim(txtFURIHI.Text) >= "01" And Trim(txtFURIHI.Text) <= "31" Then
                Else
                    Call GSUB_MESSAGE_WARNING("�U�֓��̓��͂Ɍ�肪����܂�")
                    txtFURIHI.Focus()
                    Exit Function
                End If
            Case "04", "06", "09", "11"
                If Trim(txtFURIHI.Text) >= "01" And Trim(txtFURIHI.Text) <= "30" Then
                Else
                    Call GSUB_MESSAGE_WARNING("�U�֓��̓��͂Ɍ�肪����܂�")
                    txtFURIHI.Focus()
                    Exit Function
                End If
            Case "02"
                If Trim(txtFURIHI.Text) >= "01" And Trim(txtFURIHI.Text) <= "29" Then
                Else
                    Call GSUB_MESSAGE_WARNING("�U�֓��̓��͂Ɍ�肪����܂�")
                    txtFURIHI.Focus()
                    Exit Function
                End If
        End Select

        '�����N
        If Trim(txtSEIKYUNEN.Text) < "2004" Then
            Call GSUB_MESSAGE_WARNING("�����N�̓��͂Ɍ�肪����܂�")
            txtSEIKYUNEN.Focus()
            Exit Function
        End If

        '������
        If Trim(txtSEIKYUTUKI.Text) >= "01" And Trim(txtSEIKYUTUKI.Text) <= "12" Then
        Else
            Call GSUB_MESSAGE_WARNING("�U�֌��̓��͂Ɍ�肪����܂�")
            txtSEIKYUTUKI.Focus()
            Exit Function
        End If

        '�U�֋敪�R���{
        If cmbFURIKUBUN.SelectedIndex < 0 Then
            Call GSUB_MESSAGE_WARNING("�U�֋敪���I������Ă��܂���")
            cmbFURIKUBUN.Focus()
            Exit Function
        End If

        '�w�Z�w��̏ꍇ
        If rdbgakkouSeq.Checked = True Then
            '�w�N
            If Trim(txtGAKUNEN.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("�w�N�̓��͂Ɍ�肪����܂�")
                txtGAKUNEN.Focus()
                Exit Function
            End If
            '�N���X
            If Trim(txtCLASS.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("�N���X�̓��͂Ɍ�肪����܂�")
                txtCLASS.Focus()
                Exit Function
            End If
            '���k�ԍ�
            If Trim(txtSEITONO.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("���k�ԍ��̓��͂Ɍ�肪����܂�")
                txtSEITONO.Focus()
                Exit Function
            End If
        End If

        '�����ԍ��w��̏ꍇ
        If rdbKouza.Checked = True Then
            '���Z�@�փR�[�h
            If Trim(txtKinyuuCode.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("���Z�@�փR�[�h�̓��͂Ɍ�肪����܂�")
                txtKinyuuCode.Focus()
                Exit Function
            End If
            '�x�X�R�[�h
            If Trim(txtSitCode.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("�x�X�R�[�h�̓��͂Ɍ�肪����܂�")
                txtSitCode.Focus()
                Exit Function
            End If
            '�ȖڃR���{
            If cmbKamoku.SelectedIndex < 0 Then
                Call GSUB_MESSAGE_WARNING("�Ȗڂ��I������Ă��܂���")
                cmbKamoku.Focus()
                Exit Function
            End If
            '�����ԍ�
            If Trim(txtKouzaBan.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("�����ԍ��̓��͂Ɍ�肪����܂�")
                txtKouzaBan.Focus()
                Exit Function
            End If
            '���z
            If Trim(txtKingaku.Text) = "" Then
                Call GSUB_MESSAGE_WARNING("���z�̓��͂Ɍ�肪����܂�")
                txtKingaku.Focus()
                Exit Function
            End If
        End If

        PFUNC_COMMON_CHECK = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean
        '�X�P�W���[������Ώۃf�[�^��������

        PFUNC_SCHMAST_GET = True

        '�����L�[�́A�w�Z�R�[�h�A�����N���A�U�֋敪�A�U�֓�

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " NENGETUDO_S ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"

        '�X�P�W���[���}�X�^���݃`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("�X�P�W���[�������݂��܂���")
            PFUNC_SCHMAST_GET = False
            Exit Function
        End If

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                '�s�\���ʍX�V�t���O
                If .Item("FUNOU_FLG_S") = "1" Then
                    '��Ǝ��U�X�P�W���[���}�X�^�̃t���O�`�F�b�N
                    If PFUNC_KSCHMAST_GET() = False Then
                        PFUNC_SCHMAST_GET = False
                    End If
                Else
                    Call GSUB_MESSAGE_WARNING("�s�\���ʂ��܂��X�V����Ă��܂���")
                    PFUNC_SCHMAST_GET = False
                End If
            End With
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

    End Function
    Private Function PFUNC_KSCHMAST_GET() As Boolean

        PFUNC_KSCHMAST_GET = False

        '��Ǝ��U�X�P�W���[���}�X�^�̃t���O�`�F�b�N
        STR_SQL = " SELECT * FROM SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " TORIS_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '��2005/03/26 �C��
        'STR_SQL += " AND"
        'STR_SQL += " TORIF_CODE_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " TORIF_CODE_S ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        '��2005/03/26 �C��
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S = '" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"

        '��Ǝ��U�X�P�W���[���}�X�^���݃`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("��Ǝ��U�X�P�W���[�������݂��܂���")
            Exit Function
        End If

        'LNG�Ǎ����� = 0

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                '��t�t���O�A�������݃t���O�A�z�M�t���O
                '��2005/04/06 �C�� ��Ǝ��U�͔z�M�t���O�̃`�F�b�N�͍s��Ȃ�
                'If .Item("UKETUKE_FLG_S") = "1" AND _
                '   .Item("TOUROKU_FLG_S") = "1" AND _
                '   .Item("HAISIN_FLG_S") = "0" Then
                If .Item("UKETUKE_FLG_S") = "1" And _
                   .Item("TOUROKU_FLG_S") = "1" Then
                    '��2005/04/06 �C��
                Else
                    If .Item("UKETUKE_FLG_S") <> "1" Or _
                       .Item("TOUROKU_FLG_S") <> "1" Then
                        Call GSUB_MESSAGE_WARNING("�U�փf�[�^���쐬����Ă��܂���")
                        Exit While
                    End If
                    '��2005/04/06 �C�� ��Ǝ��U�͔z�M�t���O�̃`�F�b�N�͍s��Ȃ�
                    'If .Item("HAISIN_FLG_S") <> "0" Then
                    '    Call GSUB_MESSAGE_WARNING( "�z�M�ςł�")
                    '    Exit While
                    'End If
                    '��2005/04/06 �C��
                End If
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_KSCHMAST_GET = True

    End Function
    Private Function PFUNC_MEISAI_GET() As Boolean
        '�����U�֖��׃f�[�^�̌���

        PFUNC_MEISAI_GET = False

        '�w�Z�w��̏ꍇ
        If rdbgakkouSeq.Checked = True Then
            If PFUNC_MEISAI_GAKGET() = False Then
                Exit Function
            End If
        End If

        '�����ԍ��w��̏ꍇ
        If rdbKouza.Checked = True Then
            If PFUNC_MEISAI_KOZGET() = False Then
                Exit Function
            End If
        End If

        PFUNC_MEISAI_GET = True

    End Function
    Private Function PFUNC_MEISAI_GAKGET() As Boolean
        '���׃}�X�^�̊w�Z�w��ł̌���


        PFUNC_MEISAI_GAKGET = False

        STR_SQL = " SELECT * FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKUNEN_CODE_M =" & txtGAKUNEN.Text
        STR_SQL += " AND"
        STR_SQL += " CLASS_CODE_M =" & txtCLASS.Text
        STR_SQL += " AND"
        STR_SQL += " SEITO_NO_M ='" & txtSEITONO.Text & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        LNG�Ǎ����� = 0

        '�����J�E���g
        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                If LNG�Ǎ����� > 9 Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If

                    Call GSUB_MESSAGE_WARNING("�z��i�P�O�j�𒴂��܂���")

                    Exit Function
                End If

                '�w�Z�R�[�h�ޔ�
                STR�w�Z�R�[�h(LNG�Ǎ�����) = .Item("GAKKOU_CODE_M")

                '���w�N�x�̑ޔ�
                STR���w�N�x(LNG�Ǎ�����) = .Item("NENDO_M")

                '�ʔԂ̑ޔ�
                INT�ʔ�(LNG�Ǎ�����) = .Item("TUUBAN_M")

                '�Ǎ��݌����̃J�E���g�A�b�v
                LNG�Ǎ����� += 1
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If LNG�Ǎ����� = 0 Then
            Call GSUB_MESSAGE_WARNING("���׃}�X�^�ɊY�����R�[�h�����݂��܂���")
            Exit Function
        End If

        '���k�}�X�^�̌���
        Call PSUB_SEITONAME_STORE()

        Select Case (LNG�Ǎ�����)
            Case Is = 1
                STR_SQL = " SELECT * FROM G_MEIMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_M =" & txtGAKUNEN.Text
                STR_SQL += " AND"
                STR_SQL += " CLASS_CODE_M =" & txtCLASS.Text
                STR_SQL += " AND"
                STR_SQL += " SEITO_NO_M ='" & txtSEITONO.Text & "'"

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                With OBJ_DATAREADER
                    .Read()

                    '���Z�@�փR�[�h
                    txtKinyuuCode.Text = .Item("TKIN_NO_M")
                    '�x�X�R�[�h
                    txtSitCode.Text = .Item("TSIT_NO_M")
                    '�Ȗ�
                    cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))

                    '�����ԍ�
                    txtKouzaBan.Text = .Item("TKOUZA_M")
                    '���z
                    If IsDBNull(.Item("SEIKYU_KIN_M")) = False Then
                        txtKingaku.Text = Format(CLng(.Item("SEIKYU_KIN_M")), "#,##0")
                    Else
                        txtKingaku.Text = 0
                    End If
                    '�������ҏW
                    lab������.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "�N" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "��"
                    '���`�l�J�i�ҏW
                    txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                    '���R�[�h�ԍ��ҏW
                    txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                    '�U�֌��ʃR�[�h�ҏW
                    cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                    '2005/05/11
                    strSEITO_NENDO = .Item("NENDO_M")
                    strSEITO_TUUBAN = .Item("TUUBAN_M")
                    '2006/10/26
                    update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                    '2007/09/27
                    strJUYOUKA_NO = .Item("JUYOUKA_NO_M")

                End With

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
            Case Is > 1

                STR_SQL = " SELECT * FROM G_MEIMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_M =" & txtGAKUNEN.Text
                STR_SQL += " AND"
                STR_SQL += " CLASS_CODE_M =" & txtCLASS.Text
                STR_SQL += " AND"
                STR_SQL += " SEITO_NO_M ='" & txtSEITONO.Text & "'"

                '2006/10/23 �f�[�^�����̃J�E���g
                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If
                Dim intREC_COUNT As Integer = 0
                While (OBJ_DATAREADER.Read = True)
                    intREC_COUNT += 1
                End While
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If


                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                Dim intCOUNT As Integer = 0
                Dim ICnt As Integer = 0
                While (OBJ_DATAREADER.Read = True)
                    intCOUNT += 1
                    With OBJ_DATAREADER
                        If intREC_COUNT = intCOUNT Then
                            '���Y���R�[�h�̊m�菈��
                            '���Z�@�փR�[�h
                            txtKinyuuCode.Text = .Item("TKIN_NO_M")
                            '�x�X�R�[�h
                            txtSitCode.Text = .Item("TSIT_NO_M")
                            '�Ȗ�
                            cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                            '�����ԍ�
                            txtKouzaBan.Text = OBJ_DATAREADER.Item("TKOUZA_M")
                            '���z
                            txtKingaku.Text = CStr(OBJ_DATAREADER.Item("SEIKYU_KIN_M"))
                            '�������ҏW
                            lab������.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "�N" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "��"
                            '���`�l�J�i�ҏW
                            txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                            '���R�[�h�ԍ��ҏW
                            txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                            '�U�֌��ʃR�[�h�ҏW
                            cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                            '2005/05/11
                            strSEITO_NENDO = .Item("NENDO_M")
                            strSEITO_TUUBAN = .Item("TUUBAN_M")
                            '2006/10/26
                            update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                            '2007/09/27
                            strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                        Else
                            '�m�F���b�Z�[�W�@�@�@'���k�}�X�^�̌���
                            If GFUNC_MESSAGE_YESNO("�Y�����R�[�h���������݂��܂�" & vbCr & _
                                                                                  "�ȉ��̃��R�[�h��\�����܂����H" & vbCr & _
                                                                                  "�w�N = " & .Item("GAKUNEN_CODE_M") & vbCr & _
                                                                                  "�N���X = " & .Item("CLASS_CODE_M") & vbCr & _
                                                                                  "���k�ԍ� = " & .Item("SEITO_NO_M") & vbCr & _
                                                                                  "���k�����i�J�i�j = " & STR���k�����J�i(ICnt) & vbCr & _
                                                                                  "���k�����i�����j= " & STR���k��������(ICnt) & vbCr & _
                                                                                  "������ = " & .Item("SEIKYU_TUKI_M") & vbCrLf & _
                                                                                  "���̃��R�[�h����������ꍇ�́u�������v�������Ă�������") = vbYes Then
                                '���Y���R�[�h�̊m�菈��
                                '���Z�@�փR�[�h
                                txtKinyuuCode.Text = .Item("TKIN_NO_M")
                                '�x�X�R�[�h
                                txtSitCode.Text = .Item("TSIT_NO_M")
                                '�Ȗ�
                                cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                                '�����ԍ�
                                txtKouzaBan.Text = OBJ_DATAREADER.Item("TKOUZA_M")
                                '���z
                                txtKingaku.Text = CStr(OBJ_DATAREADER.Item("SEIKYU_KIN_M"))
                                '�������ҏW
                                lab������.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "�N" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "��"
                                '���`�l�J�i�ҏW
                                txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                                '���R�[�h�ԍ��ҏW
                                txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                                '�U�֌��ʃR�[�h�ҏW
                                cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                                '2005/05/11
                                strSEITO_NENDO = .Item("NENDO_M")
                                strSEITO_TUUBAN = .Item("TUUBAN_M")
                                '2006/10/26
                                update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                                '2007/09/27
                                strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                                Exit While
                            End If

                        End If
                    End With

                    ICnt += 1
                End While

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
        End Select

        PFUNC_MEISAI_GAKGET = True

    End Function
    Private Function PFUNC_MEISAI_KOZGET() As Boolean
        '���׃}�X�^�̌����ԍ��w��ł̌���

        Dim sESC_SQL As String

        PFUNC_MEISAI_KOZGET = False

        STR_SQL = " SELECT * FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M ='" & txtSEIKYUNEN.Text & txtSEIKYUTUKI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M ='" & txtKinyuuCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TSIT_NO_M ='" & txtSitCode.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " TKAMOKU_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku) & "'"
        'STR_SQL += " TKAMOKU_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAMOKU_TXT, cmbKamoku) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKOUZA_M ='" & txtKouzaBan.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_KIN_M = " & CLng(txtKingaku.Text)

        sESC_SQL = STR_SQL

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        LNG�Ǎ����� = 0

        '�����J�E���g
        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                If LNG�Ǎ����� > 9 Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If

                    Call GSUB_MESSAGE_WARNING("�z��i�P�O�j�𒴂��܂���")

                    Exit Function
                End If

                '�w�Z�R�[�h�ޔ�
                STR�w�Z�R�[�h(LNG�Ǎ�����) = .Item("GAKKOU_CODE_M")

                '���w�N�x�̑ޔ�
                STR���w�N�x(LNG�Ǎ�����) = .Item("NENDO_M")

                '�ʔԂ̑ޔ�
                INT�ʔ�(LNG�Ǎ�����) = .Item("TUUBAN_M")
            End With

            '�Ǎ��݌����̃J�E���g�A�b�v
            LNG�Ǎ����� += 1
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If LNG�Ǎ����� = 0 Then
            Call GSUB_MESSAGE_WARNING("���׃}�X�^�ɊY�����R�[�h�����݂��܂���")
            Exit Function
        End If

        '���k�}�X�^�̌���
        Call PSUB_SEITONAME_STORE()

        Select Case (LNG�Ǎ�����)
            Case Is = 1
                STR_SQL = sESC_SQL

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                With OBJ_DATAREADER

                    .Read()

                    '���Z�@�փR�[�h
                    txtKinyuuCode.Text = .Item("TKIN_NO_M")
                    '�x�X�R�[�h
                    txtSitCode.Text = .Item("TSIT_NO_M")
                    '�Ȗ�
                    cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                    '�����ԍ�
                    txtKouzaBan.Text = .Item("TKOUZA_M")
                    '���z
                    txtKingaku.Text = CStr(.Item("SEIKYU_KIN_M"))
                    '�������ҏW
                    lab������.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "�N" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "��"
                    '���`�l�J�i�ҏW
                    txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                    '���R�[�h�ԍ��ҏW
                    txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                    '�U�֌��ʃR�[�h�ҏW
                    cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                    '2005/05/11
                    strSEITO_NENDO = .Item("NENDO_M")
                    strSEITO_TUUBAN = .Item("TUUBAN_M")
                    '2006/10/26
                    update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                    '2007/09/27
                    strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                End With

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
            Case Is > 1

                STR_SQL = sESC_SQL

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Function
                End If

                Dim ICnt As Integer = 0

                While (OBJ_DATAREADER.Read = True)
                    With OBJ_DATAREADER
                        'Debug.WriteLine("READ")
                        '�m�F���b�Z�[�W
                        If GFUNC_MESSAGE_YESNO("�Y�����R�[�h���������݂��܂�" & vbCr & _
                                                          "�ȉ��̃��R�[�h��\�����܂����H" & vbCr & _
                                                          "�w�N = " & .Item("GAKUNEN_CODE_M") & vbCr & _
                                                          "�N���X = " & .Item("CLASS_CODE_M") & vbCr & _
                                                          "���k�ԍ� = " & .Item("SEITO_NO_M") & vbCr & _
                                                          "���k�����i�J�i�j = " & STR���k�����J�i(ICnt) & vbCr & _
                                                          "���k�����i�����j= " & STR���k��������(ICnt) & vbCr & _
                                                          "������ = " & .Item("SEIKYU_TUKI_M") & vbCrLf & _
                                                          "���̃��R�[�h����������ꍇ�́u�������v�������Ă�������") = vbYes Then

                            '���Y���R�[�h�̊m�菈��
                            '���Z�@�փR�[�h
                            txtKinyuuCode.Text = .Item("TKIN_NO_M")
                            '�x�X�R�[�h
                            txtSitCode.Text = .Item("TSIT_NO_M")
                            '�Ȗ�
                            cmbKamoku.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, .Item("TKAMOKU_M"))
                            '�����ԍ�
                            txtKouzaBan.Text = .Item("TKOUZA_M")
                            '���z
                            txtKingaku.Text = CStr(.Item("SEIKYU_KIN_M"))
                            '�������ҏW
                            lab������.Text = Mid(.Item("SEIKYU_TUKI_M"), 1, 4) & "�N" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "��"
                            '���`�l�J�i�ҏW
                            txtKeiyaku.Text = .Item("TMEIGI_KNM_M")
                            '���R�[�h�ԍ��ҏW
                            txtRECNO.Text = CStr(.Item("RECORD_NO_M"))
                            '�U�֌��ʃR�[�h�ҏW
                            cmbFuriketu.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKETUCODE_TXT, .Item("FURIKETU_CODE_M"))
                            '2005/05/11
                            strSEITO_NENDO = .Item("NENDO_M")
                            strSEITO_TUUBAN = .Item("TUUBAN_M")
                            '2006/10/26
                            update_GAKUNEN = .Item("GAKUNEN_CODE_M")
                            '2007/09/27
                            strJUYOUKA_NO = .Item("JUYOUKA_NO_M")
                            Exit While
                        End If
                    End With

                    ICnt += 1
                End While

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
        End Select

        PFUNC_MEISAI_KOZGET = True

    End Function
    Private Function PFUNC_KMEISAI_UPD() As Boolean

        PFUNC_KMEISAI_UPD = False

        '��Ǝ��U�@���׃}�X�^�̍X�V
        STR_SQL = " UPDATE MEIMAST SET "
        '�U�֌��ʃR�[�h
        STR_SQL += " FURIKETU_CODE_K = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKETUCODE_TXT, cmbFuriketu) & "'"
        '�����L�[�́A�w�Z�R�[�h�A�U�֓��A�U�֓��o�敪�A���Z�@�փR�[�h�A�x�X�R�[�h�A�ȖڃR�[�h�A�����ԍ��A�������z
        STR_SQL += " WHERE TORIS_CODE_K ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND TORIF_CODE_K ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        STR_SQL += " AND FURI_DATE_K ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND FSYORI_KBN_K ='1'"
        STR_SQL += " AND KEIYAKU_KIN_K ='" & txtKinyuuCode.Text & "'"
        STR_SQL += " AND KEIYAKU_SIT_K ='" & txtSitCode.Text & "'"
        STR_SQL += " AND KEIYAKU_KAMOKU_K ='" & ConvKamoku1To2(CStr(CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKamoku)))) & "'"
        STR_SQL += " AND KEIYAKU_KOUZA_K ='" & txtKouzaBan.Text & "'"
        STR_SQL += " AND FURIKIN_K = " & CLng(txtKingaku.Text)
        STR_SQL += " AND"
        'STR_SQL += " SUBSTR(JYUYOUKA_NO_K,1,8) = '" & strSEITO_NENDO & strSEITO_TUUBAN.PadLeft(4, "0") & "'"
        '���S�M������ �w�Z�R�[�h(7)�{�w�N(1)+�N���X(2)+���k�ԍ�(7)+������(2)+�U�֋敪(1)�@2007/09/04
        STR_SQL += " SUBSTR(JYUYOUKA_NO_K,1,20) = '" & strJUYOUKA_NO & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '�X�V�����G���[
            Call GSUB_MESSAGE_WARNING("��Ǝ��U���׃}�X�^�̍X�V�ŃG���[���������܂����B")
            Exit Function
        End If

        PFUNC_KMEISAI_UPD = True

    End Function
    Private Function PSUB_SEITONAME_STORE() As Boolean
        '���k�}�X�^�̌���


        For i As Integer = 0 To LNG�Ǎ����� - 1

            STR_SQL = " SELECT * FROM SEITOMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_O ='" & STR�w�Z�R�[�h(i) & "'"
            STR_SQL += " AND"
            STR_SQL += " NENDO_O ='" & STR���w�N�x(i) & "'"
            STR_SQL += " AND"
            STR_SQL += " TUUBAN_O =" & INT�ʔ�(i)

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            With OBJ_DATAREADER
                If .Read() = True Then
                    STR���k�����J�i(i) = .Item("SEITO_KNAME_O")
                    STR���k��������(i) = ConvNullToString(.Item("SEITO_NNAME_O"), "")
                Else
                    STR���k�����J�i(i) = ""
                    STR���k��������(i) = ""
                End If
            End With

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
        Next i

    End Function
    Private Function PFUNC_GSCHAMST_UPD() As Boolean

        Dim iGakunen_Flag() As Integer = Nothing
        Dim bFlg As Boolean
        Dim bLoopFlg As Boolean
        Dim iLcount As Integer

        PFUNC_GSCHAMST_UPD = False


        '�X�V�w�N����X�P�W���[���̊w�N�t���O���擾���A�X�V�ΏۃX�P�W���[�����w�肷��
        '2006/10/26
        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND GAKUNEN" & update_GAKUNEN & "_FLG_S ='1'"
        '�ǉ� 2006/11/29
        STR_SQL += " AND FURI_KBN_S = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If
            Exit Function
        Else
            OBJ_DATAREADER_DREAD.Read()
            With OBJ_DATAREADER_DREAD
                UPD_SCH_KBN = .Item("SCH_KBN_S")
                UPD_FURI_KBN = .Item("FURI_KBN_S")
                UPD_FURI_DATE = .Item("FURI_DATE_S")
                UPD_SFURI_DATE = .Item("SFURI_DATE_S")
            End With
        End If
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '�X�P�W���[�����N�ԁA���ʂ���
        Select Case (PFUNC_Get_Gakunen(txtGAKKOU_CODE.Text, iGakunen_Flag))
            Case -1
                '�G���[
                Call GSUB_LOG(0, "�w��w�N�擾�ŃG���[���������܂���")

                Exit Function
            Case 0
                '�S�w�N���Ώ�
                bFlg = False
            Case Else
                '����w�N�݂̂��Ώ�
                bFlg = True
        End Select

        '---------------------------------------
        '�U�֍ό����E���z�̎擾
        '---------------------------------------
        Dim dblFURI_KEN As Double
        Dim dblFURI_KIN As Double
        STR_SQL = " SELECT COUNT(SEIKYU_KIN_M),SUM(SEIKYU_KIN_M) FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKETU_CODE_M ='0'"
        If bFlg = True Then
            STR_SQL += " AND ("
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"
        End If
        '�ǉ� 2006/11/29
        STR_SQL += " AND FURI_KBN_M ='" & UPD_FURI_KBN & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            dblFURI_KEN = 0
            dblFURI_KIN = 0
        Else
            OBJ_DATAREADER_DREAD.Read()
            With OBJ_DATAREADER_DREAD
                dblFURI_KEN = .Item("COUNT(SEIKYU_KIN_M)")
                dblFURI_KIN = CDbl(ConvNullToString(.Item("SUM(SEIKYU_KIN_M)"), "0"))
            End With
        End If
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        bLoopFlg = False

        '---------------------------------------
        '�s�\�����E���z�̎擾
        '---------------------------------------
        Dim dblFUNOU_KEN As Double
        Dim dblFUNOU_KIN As Double
        STR_SQL = " SELECT count(SEIKYU_KIN_M),sum(SEIKYU_KIN_M) FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND FURIKETU_CODE_M <> '0'"
        If bFlg = True Then
            STR_SQL += " AND ("
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"
        End If
        '�ǉ� 2006/11/29
        STR_SQL += " AND FURI_KBN_M ='" & UPD_FURI_KBN & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            dblFUNOU_KEN = 0
            dblFUNOU_KIN = 0
        Else
            OBJ_DATAREADER_DREAD.Read()
            With OBJ_DATAREADER_DREAD
                dblFUNOU_KEN = .Item("count(SEIKYU_KIN_M)")
                dblFUNOU_KIN = CDbl(ConvNullToString(.Item("sum(SEIKYU_KIN_M)"), "0"))
            End With
        End If
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '---------------------------------------
        '�X�P�W���[���}�X�^�̍X�V
        '---------------------------------------
        '���U�X�P�W���[���}�X�^�X�V
        STR_SQL = " UPDATE  G_SCHMAST SET "
        STR_SQL += " FURI_KEN_S =" & dblFURI_KEN & ","
        STR_SQL += " FURI_KIN_S =" & dblFURI_KIN & ","
        STR_SQL += " FUNOU_KEN_S =" & dblFUNOU_KEN & ","
        STR_SQL += " FUNOU_KIN_S =" & dblFUNOU_KIN & ""
        STR_SQL += " WHERE GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND SFURI_DATE_S ='" & UPD_SFURI_DATE & "'"
        STR_SQL += " AND SCH_KBN_S = '" & UPD_SCH_KBN & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '�X�V�����G���[
            Call GSUB_MESSAGE_WARNING("�X�P�W���[���}�X�^�̍X�V�ŃG���[���������܂����B")

            Exit Function
        End If

        PFUNC_GSCHAMST_UPD = True

    End Function

    Private Function PFUNC_GSCHAMST_UPD_TAKOU() As Boolean
        Dim dblFURI_KEN As Double
        Dim dblFURI_KIN As Double
        Dim dblFUNOU_KEN As Double
        Dim dblFUNOU_KIN As Double
        Dim intGAKUNEN_CODE As Integer = 0
        Dim intFURI_KBN As Integer = 0
        Dim strTAKOU_CD As String = ""

        PFUNC_GSCHAMST_UPD_TAKOU = False

        STR_SQL = " SELECT SUM(decode(FURIKETU_CODE_M,'0',1,0)) AS FURIKEN ,SUM(decode(FURIKETU_CODE_M,'0',SEIKYU_KIN_M,0)) AS FURIKIN,"
        STR_SQL += " SUM(DECODE(FURIKETU_CODE_M,'0',0,1)) AS FUNOUKEN ,SUM(decode(FURIKETU_CODE_M,'0',0,SEIKYU_KIN_M)) AS FUNOUKIN,"
        STR_SQL += " GAKUNEN_CODE_M,FURI_KBN_M,TKIN_NO_M"
        STR_SQL += " FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_M ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND SEIKYU_KIN_M > 0"
        STR_SQL += " AND TKIN_NO_M <> '" & STR_JIKINKO_CODE & "'"
        STR_SQL += " GROUP BY GAKKOU_CODE_M,GAKUNEN_CODE_M,FURI_KBN_M,TKIN_NO_M"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            dblFURI_KEN = 0
            dblFURI_KIN = 0
            intGAKUNEN_CODE = 0
            intFURI_KBN = 0
            strTAKOU_CD = ""
        Else
            While (OBJ_DATAREADER_DREAD.Read = True)

                With OBJ_DATAREADER_DREAD
                    dblFURI_KEN = .Item("FURIKEN")
                    dblFURI_KIN = CDbl(ConvNullToString(.Item("FURIKIN"), "0"))
                    dblFUNOU_KEN = .Item("FUNOUKEN")
                    dblFUNOU_KIN = CDbl(ConvNullToString(.Item("FUNOUKIN"), "0"))
                    intGAKUNEN_CODE = CInt(ConvNullToString(.Item("GAKUNEN_CODE_M"), 0))
                    intFURI_KBN = CInt(ConvNullToString(.Item("FURI_KBN_M"), 0))
                    strTAKOU_CD = .Item("TKIN_NO_M")
                    '---------------------------------------
                    '�X�P�W���[���}�X�^�̍X�V
                    '---------------------------------------
                    '���U�X�P�W���[���}�X�^�X�V
                    STR_SQL = " UPDATE  G_TAKOUSCHMAST SET "
                    STR_SQL += " FURI_KEN_U =" & dblFURI_KEN & ","
                    STR_SQL += " FURI_KIN_U =" & dblFURI_KIN & ","
                    STR_SQL += " FUNOU_KEN_U =" & dblFUNOU_KEN & ","
                    STR_SQL += " FUNOU_KIN_U =" & dblFUNOU_KIN
                    STR_SQL += " WHERE GAKKOU_CODE_U  ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND FURI_DATE_U ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
                    STR_SQL += " AND FURI_KBN_U ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
                    STR_SQL += " AND GAKUNEN_U  =" & intGAKUNEN_CODE
                    STR_SQL += " AND TKIN_NO_U  = '" & strTAKOU_CD & "'"

                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                        '�X�V�����G���[
                        Call GSUB_MESSAGE_WARNING("�X�P�W���[���}�X�^�̍X�V�ŃG���[���������܂����B")

                        Exit Function
                    End If

                End With
            End While

        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If


        PFUNC_GSCHAMST_UPD_TAKOU = True

    End Function
    Private Function PFUNC_Get_Gakunen(ByVal pGakkou_Code As String, ByRef pSiyou_gakunen() As Integer) As Integer

        Dim iLoopCount As Integer
        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(9)

        PFUNC_Get_Gakunen = -1

        '�I�����ꂽ�w�Z�̎w��U�֓��Œ��o
        '(�S�X�P�W���[���敪���Ώ�)
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND FURI_DATE_S ='" & txtFURINEN.Text & txtFURITUKI.Text & txtFURIHI.Text & "'"
        STR_SQL += " AND CHECK_FLG_S ='1'"
        STR_SQL += " AND DATA_FLG_S ='1'"
        STR_SQL += " AND FUNOU_FLG_S ='1'"
        STR_SQL += " AND TYUUDAN_FLG_S ='0'"
        STR_SQL += " AND SFURI_DATE_S ='" & UPD_SFURI_DATE & "'"
        STR_SQL += " AND FURI_KBN_S = '" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND SCH_KBN_S = '" & UPD_SCH_KBN & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read)
            With OBJ_DATAREADER_DREAD
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                For iLoopCount = 1 To iMaxGakunen
                    Select Case (CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S")))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        '�g�p�w�N�S�ĂɊw�N�t���O������ꍇ�͑S�w�N�ΏۂƂ��Ĉ���
        '�w�N
        For iLoopCount = 1 To iMaxGakunen
            Select Case (pSiyou_gakunen(iLoopCount))
                Case Is <> 1
                    PFUNC_Get_Gakunen = iMaxGakunen

                    Exit Function
            End Select
        Next iLoopCount

        PFUNC_Get_Gakunen = 0

    End Function


#End Region

#Region " CheckedChanged(RadioButton) "
    '****************************
    'CheckedChanged
    '****************************
    Private Sub rdbgakkouSeq_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbgakkouSeq.CheckedChanged
        '�w�Z�w��
        If rdbgakkouSeq.Checked = True Then
            txtGAKUNEN.Visible = True
            lbl1.Visible = True
            txtCLASS.Visible = True
            lbl2.Visible = True
            txtSEITONO.Visible = True
        Else
            txtGAKUNEN.Visible = False
            lbl1.Visible = False
            txtCLASS.Visible = False
            lbl2.Visible = False
            txtSEITONO.Visible = False
        End If
    End Sub
    Private Sub rdbKouza_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbKouza.CheckedChanged
        '�����ԍ��w��
        If rdbKouza.Checked = True Then
            GroupBox1.Visible = True
        Else
            GroupBox1.Visible = False
        End If
    End Sub
#End Region

#Region " LostFocus "
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z���̎擾
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '�w�Z���̎擾
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If

    End Sub
    Private Sub txtKingaku_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtKingaku.Validating

        If btnSansyou.Enabled = True Then
            btnSansyou.Focus()
        Else
            cmbFuriketu.Focus()
        End If
    End Sub
#End Region

#Region " SELECT edIndexChanged(ComboBox) "
    '****************************
    'SelectedIndexChanged
    '****************************
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
        If PFUNC_GAKNAME_GET() = False Then
            Exit Sub
        End If

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub
#End Region

End Class
