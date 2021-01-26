Option Explicit On
Option Strict Off
Imports CASTCommon
Public Class KFGOTHR010

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �����U�փf�[�^�쐬���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Dim STR�����N�� As String
    Dim STR�I��U�֓� As String
    Dim STR�ҏW�U�֓� As String
    Dim STR�ҏW�ĐU�֓� As String
    Dim STR�ĐU�֓� As String
    Dim LNG�폜���� As Long
    Dim LNG�Ǎ����� As Long
    Dim STR�O��U�֓� As String
    Dim STR�ĐU��� As String
    Dim STR������ As String

    Private STR�I��U�֋敪 As String

    Public iGakunen_Flag() As Integer
    Public strSCH_KBN As String '�X�P�W���[���敪

    Dim LNG��ƍ폜���� As Long '�ǉ� 2007/09/05
    Private MainLOG As New CASTCommon.BatchLOG("KFGOTHR010", "�����U�փf�[�^�쐬������")
    Private Const msgTitle As String = "�����U�փf�[�^�쐬������(KFGOTHR010)"
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

    '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
    '�V�X�e���Őݒ�ł���ŏ��̊w�N�R�[�h�ƍő�̊w�N�R�[�h
    Private Const MIN_SYS_GAKUNEN_CODE As Integer = 1
    Private Const MAX_SYS_GAKUNEN_CODE As Integer = 9
    '�g�p�w�N
    Private INT�g�p�w�N As Integer
    '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGOTHR010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "�����U�փf�[�^�쐬������"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbFURIKUBUN)")
                Exit Sub
            End If

            'Oracle �ڑ�(Read��p)
            OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
            'Oracle OPEN(Read��p)
            OBJ_CONNECTION_DREAD.Open()

            '���̓{�^������
            btnSEARCH.Enabled = True
            btnAction.Enabled = False
            btnEraser.Enabled = True
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
    Private Sub btnSEARCH_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSEARCH.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)�J�n", "����", "")
            '�����{�^��
            '���͍��ڂ̋��ʃ`�F�b�N
            If PFUNC_COMMON_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            If PFUNC_SCHMAST_GET() = False Then
                Exit Sub
            End If

            '���͍��ڐ���
            Call PSUB_INPUTEnabled(False)

            '���̓{�^������
            btnSEARCH.Enabled = False
            btnAction.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        '���s�{�^��
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")
            '���͍��ڂ̋��ʃ`�F�b�N
            If PFUNC_COMMON_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '�`�F�b�N�{�b�N�X��ԃ`�F�b�N
            '�󃊃X�g�̏ꍇ�͏I��
            If chkLIST.Items.Count = 0 Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "�����U�փf�[�^�쐬���"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Exit Sub
            End If

            '�g�����U�N�V�����J�n
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
                Exit Sub
            End If

            LNG�폜���� = 0

            '�`�F�b�N�{�b�N�X��on�̂��̂�T��
            For iCount As Integer = 0 To (chkLIST.Items.Count - 1)
                'If chkLIST.GetItemChecked(iCount) = True Then
                STR�I��U�֓� = chkLIST.Items(iCount).ToString

                '�����N���̊l���̂��߁A�X�P�W���[���}�X�^�̍�GET
                If PFUNC_SEIKYUNENTUKI_GET() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����N���̊l��", "���s", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)

                    Exit Sub
                End If

                '�����U�֖��׃f�[�^�̍폜�i���U�A�ĐU�j
                If PFUNC_MEISAI_DEL() = False Then
                    '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���׃}�X�^�̍폜�i���U�A�ĐU�j", "���s", Err.Description)
                    'MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�̍폜�i���U�A�ĐU�j", "���s", Err.Description)
                    '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If

                '�X�P�W���[���}�X�^�̍X�V�i���U�A�ĐU�j
                If PFUNC_SCHMAST_UPD() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�̍X�V�i���U�A�ĐU�j", "���s", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If

                '�O��U�֖��׃f�[�^�̍X�V
                '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
                '�U�֋敪�����������i�����E�o���j�̏ꍇ�́A�O��U�֖��ׂ̃f�[�^�X�V�͍s��Ȃ��B
                Select Case Me.STR�I��U�֋敪
                    Case "2", "3"
                        '���������i�����E�o���j
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�O��U�֖��׃f�[�^�̍X�V", "����", "���������̎���̂��ߏ����s�v")

                    Case Else
                        '���������ȊO
                        Select Case (STR�ĐU���)
                            Case "1", "2", "3"   '�ĐU����A�J�z����
                                If PFUNC_ZENMEISAI_UPD() = False Then
                                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�O��U�֖��׃f�[�^�̍X�V", "���s", Err.Description)
                                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                                    Exit Sub
                                End If
                        End Select
                End Select
                'Select Case (STR�ĐU���)
                '    Case "1", "2", "3"   '�ĐU����A�J�z����
                '        If PFUNC_ZENMEISAI_UPD() = False Then
                '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�O��U�֖��׃f�[�^�̍X�V", "���s", Err.Description)
                '            Call GFUNC_EXECUTESQL_TRANS("", 3)
                '            Exit Sub
                '        End If
                'End Select
                '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
                'End If
            Next iCount

            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            If CASTCommon.GetFSKJIni("COMMON", "KIGYO_JIFURI") = "1" Then
                '���׃}�X�^�̍폜�i��Ɓj
                If Me.PFUNC_KIGYO_MEISAI_DEL() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���׃}�X�^�̍폜�i��Ɓj", "���s", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If

                '�X�P�W���[���}�X�^�̍폜�i��Ɓj
                If Me.PFUNC_KIGYO_SCHMAST_UPD() = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�X�P�W���[���}�X�^�̍X�V�i��Ɓj", "���s", Err.Description)
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    Exit Sub
                End If
            End If
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

            '���X�g�{�b�N�X�Ń`�F�b�NON�̂��̂���납��폜����
            For iCount As Integer = (chkLIST.Items.Count - 1) To 0 Step -1
                chkLIST.Items.RemoveAt(iCount)
            Next

            '���X�g�{�b�N�X�̗L��
            If chkLIST.Items.Count = 0 Then
                '���X�g�{�b�N�X�̖��̏ꍇ
                '���͍��ڐ���
                Call PSUB_INPUTEnabled(True)

                '�w�Z�R�[�h�ɃJ�[�\���ݒ�
                txtGAKKOU_CODE.Focus()

                '���̓{�^������
                btnSEARCH.Enabled = True
                btnAction.Enabled = False
                btnEraser.Enabled = True
                btnEnd.Enabled = True
            Else
                '���X�g�{�b�N�X�̗L�̏ꍇ
                '���͍��ڐ���
                Call PSUB_INPUTEnabled(False)

                '���̓{�^������
                btnSEARCH.Enabled = False
                btnAction.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
            End If

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                Exit Sub
            End If

            MessageBox.Show(String.Format(G_MSG0001I, LNG�폜����, LNG��ƍ폜����), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '����{�^��
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            '���͍��ځ@�}�~�̉���
            Call PSUB_INPUTEnabled(True)

            '�����U�փf�[�^�쐬�ꗗ
            chkLIST.Items.Clear()

            '���͍��ڐ���
            '�w�Z�R�[�h�ɃJ�[�\���ݒ�
            txtGAKKOU_CODE.Focus()

            '���̓{�^������
            btnSEARCH.Enabled = True
            btnAction.Enabled = False
            btnEraser.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '********************************************
        '�I���{�^��
        '********************************************
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD.Close()
            OBJ_CONNECTION_DREAD = Nothing
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_INPUTEnabled(ByVal pValue As Boolean)

        '���͍��ڂ̗}�~
        cmbKana.Enabled = pValue
        cmbGakkouName.Enabled = pValue

        txtGAKKOU_CODE.Enabled = pValue
        txtFURINEN.Enabled = pValue
        txtFURITUKI.Enabled = pValue
        txtFURIHI.Enabled = pValue

        cmbFURIKUBUN.Enabled = pValue

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
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        '�w�Z���̎擾
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '�w�Z������
            STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            OBJ_DATAREADER.Read()
            If OBJ_DATAREADER.HasRows = True Then
                lab�w�Z��.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
            Else
                lab�w�Z��.Text = ""
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If
        End If

    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_COMMON_CHECK() As Boolean

        Dim bFlg As Boolean

        PFUNC_COMMON_CHECK = False

        STR�I��U�֋敪 = ""

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '�w�Z�}�X�^���݃`�F�b�N
            STR_SQL = "SELECT SFURI_SYUBETU_T"
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            '�g�p�w�N���擾
            STR_SQL += ", SIYOU_GAKUNEN_T"
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
            STR_SQL += " FROM GAKMAST2"
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            STR�ĐU��� = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SFURI_SYUBETU_T")
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            INT�g�p�w�N = GCom.NzInt(GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SIYOU_GAKUNEN_T"))
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

            If Trim(STR�ĐU���) = "" Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If
        End If

        '�U�֓�
        If Trim(txtFURINEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURINEN.Focus()

            Exit Function

        End If

        If Trim(txtFURITUKI.Text) = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURITUKI.Focus()

            Exit Function
        Else
            Select Case (CInt(txtFURITUKI.Text))
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFURITUKI.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFURIHI.Text) = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURIHI.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtFURINEN.Text) & "/" & Trim(txtFURITUKI.Text) & "/" & Trim(txtFURIHI.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFURINEN.Text) & Format(CInt(txtFURITUKI.Text), "00") & Format(CInt(txtFURIHI.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then

            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            txtFURINEN.Focus()

            Exit Function
        End If

        '�U�֋敪
        If cmbFURIKUBUN.SelectedIndex < 0 Then

            MessageBox.Show(G_MSG0006W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbFURIKUBUN.Focus()
            Exit Function
        End If

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        bFlg = False

        With OBJ_DATAREADER
            .Read()

            If Trim(.Item("FUNOU_FLG_S")) = "1" Then
                bFlg = True
            End If
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If bFlg = True Then
            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        STR�I��U�֋敪 = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)

        PFUNC_COMMON_CHECK = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False

        '�X�P�W���[������Ώۃf�[�^��������

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
        STR_SQL += " AND FURI_KBN_S ='" & Me.STR�I��U�֋敪 & "'"
        'STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        LNG�Ǎ����� = 0

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        '���X�g�{�b�N�X�̃N���A
        chkLIST.Items.Clear()

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                '���z�m�F�σt���O�A�f�[�^�쐬�σt���O���P�̕��̂�
                If .Item("CHECK_FLG_S") = "1" And .Item("DATA_FLG_S") = "1" Then
                    '�U�֓��̕ҏW
                    STR�ҏW�U�֓� = Mid(.Item("FURI_DATE_S"), 1, 4) & "/" & Mid(.Item("FURI_DATE_S"), 5, 2) & "/" & Mid(.Item("FURI_DATE_S"), 7, 2)
                    '�ĐU�֓��̕ҏW
                    If .Item("SFURI_DATE_S") <> "00000000" Then
                        STR�ҏW�ĐU�֓� = Mid(.Item("SFURI_DATE_S"), 1, 4) & "/" & Mid(.Item("SFURI_DATE_S"), 5, 2) & "/" & Mid(.Item("SFURI_DATE_S"), 7, 2)
                    Else
                        STR�ҏW�ĐU�֓� = ""
                    End If

                    '��Ǝ��U�X�P�W���[���}�X�^�̃t���O�`�F�b�N
                    If PFUNC_KSCHMAST_GET() = False Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If

                        Exit Function
                    End If
                    '���X�g�{�b�N�X�֒ǉ�
                    chkLIST.Items.Add("�U�֓��F" & STR�ҏW�U�֓� & "  " & "�ĐU�֓��F" & STR�ҏW�ĐU�֓�)
                    LNG�Ǎ����� += 1
                End If
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If LNG�Ǎ����� = 0 Then
            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_KSCHMAST_GET() As Boolean

        PFUNC_KSCHMAST_GET = False

        '��Ǝ��U�X�P�W���[���}�X�^�̃t���O�`�F�b�N
        STR_SQL = " SELECT * FROM SCHMAST"
        STR_SQL += " WHERE TORIS_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
        STR_SQL += " AND TORIF_CODE_S ='0" & (CInt(Me.STR�I��U�֋敪) + 1) & "'"
        'STR_SQL += " AND TORIF_CODE_S ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If

            PFUNC_KSCHMAST_GET = True

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                If .Item("HAISIN_FLG_S") <> "0" Then
                    If GFUNC_SELECT_SQL3("", 1) = False Then
                        Exit Function
                    End If

                    MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    Exit Function
                End If
            End With
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        PFUNC_KSCHMAST_GET = True

    End Function
    Private Function PFUNC_SEIKYUNENTUKI_GET() As Boolean

        PFUNC_SEIKYUNENTUKI_GET = False

        '�X�P�W���[������Ώۃf�[�^�̐����N�����l������

        '���X�g�{�b�N�X����I���������̂���A�ĐU�֓��𒊏o����
        STR�ĐU�֓� = Mid(STR�I��U�֓�, 22, 4) & Mid(STR�I��U�֓�, 27, 2) & Mid(STR�I��U�֓�, 30, 2)
        If STR�ĐU�֓� = "" Then
            STR�ĐU�֓� = "00000000"
        End If

        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
        STR_SQL += " AND FURI_KBN_S ='" & Me.STR�I��U�֋敪 & "'"
        'STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND SFURI_DATE_S ='" & STR�ĐU�֓� & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If

            STR�����N�� = ""

            MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function

        End If

        OBJ_DATAREADER_DREAD.Read()

        STR�����N�� = OBJ_DATAREADER_DREAD.Item("NENGETUDO_S")

        '�X�P�W���[�����u���ʁv����Ώۊw�N�擾 2005/12/09
        strSCH_KBN = OBJ_DATAREADER_DREAD.Item("SCH_KBN_S")
        With OBJ_DATAREADER_DREAD
            'SCH_KBN_S=1:���ʐU�֓� SCH_KBN_S=2:�����U�֓�(�w�N�w��)
            If strSCH_KBN <> "0" Then

                ReDim iGakunen_Flag(0)  '������
                '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
                For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    'For i As Integer = 1 To 9
                    '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
                    ReDim Preserve iGakunen_Flag(i)
                    If .Item("GAKUNEN" & i & "_FLG_S") = "1" Then
                        iGakunen_Flag(i) = 1
                    Else
                        iGakunen_Flag(i) = 0
                    End If
                Next
            End If
        End With

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        PFUNC_SEIKYUNENTUKI_GET = True

    End Function
    Private Function PFUNC_MEISAI_DEL() As Boolean

        PFUNC_MEISAI_DEL = False

        '�����U�֖��׃f�[�^�̍폜

        '���U �U�փf�[�^�폜����
        '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        STR_SQL = " SELECT COUNT(*) AS CNT FROM G_MEIMAST"
        'STR_SQL = " SELECT * FROM G_MEIMAST"
        '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR�����N�� & "'"
        STR_SQL += " AND FURI_DATE_M ='" & STR_FURIKAE_DATE(1) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
        STR_SQL += " AND FURI_KBN_M ='" & Me.STR�I��U�֋敪 & "'"
        'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
        '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '���ʐU�֓��A�����X�P�W���[�����͏����Ώۂ̊w�N�������Ɋ܂߂�B
        If Me.strSCH_KBN <> "0" Then
            '���ʐU�֓��A�����X�P�W���[��
            STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
            For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                If iGakunen_Flag(i) = 1 Then
                    STR_SQL += ","
                    STR_SQL += i.ToString
                End If
            Next
            STR_SQL += ")"
        End If
        'IN��̃_�~�[�����폜
        STR_SQL = STR_SQL.Replace("XXX,", "")
        '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        '�����J�E���g
        '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        If OBJ_DATAREADER_DREAD.Read = True Then
            LNG�폜���� += GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))
        End If
        'While (OBJ_DATAREADER_DREAD.Read = True)
        '    LNG�폜���� += 1
        'End While
        '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '�폜����
        STR_SQL = " DELETE  FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR�����N�� & "'"
        STR_SQL += " AND FURI_DATE_M ='" & STR_FURIKAE_DATE(1) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
        STR_SQL += " AND FURI_KBN_M ='" & Me.STR�I��U�֋敪 & "'"
        'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
        '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '���ʐU�֓��A�����X�P�W���[�����͏����Ώۂ̊w�N�������Ɋ܂߂�B
        If Me.strSCH_KBN <> "0" Then
            '���ʐU�֓��A�����X�P�W���[��
            STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
            For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                If iGakunen_Flag(i) = 1 Then
                    STR_SQL += ","
                    STR_SQL += i.ToString
                End If
            Next
            STR_SQL += ")"
        End If
        'IN��̃_�~�[�����폜
        STR_SQL = STR_SQL.Replace("XXX,", "")
        '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        '�ĐU�@�U�փf�[�^�폜����
        '2020/02/06 �^�X�N�j�֓� NOTE �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '���L��If�������́A���݂̎d�l�ɉ����Ă��Ȃ��B�i�ĐU�܂ŏ������i��ł��鏉�U������������̂��H�j
        '���U�̎���@�@�@�@�@���@�ĐU�̖��ׂ͑��݂��Ȃ�
        '�ĐU�̎���@�@�@�@�@���@�ĐU����"00000000"
        '���z�L�̍ĐU�̎���@���@�ĐU���ɂ͎��񏉐U�������邪�A�������Ă��Ȃ����ߖ��ׂ͖���
        '2020/02/06 �^�X�N�j�֓� NOTE �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
        If STR�ĐU�֓� <> "00000000" Then
            '�ĐU�X�P�W���[������̏ꍇ
            '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            STR_SQL = " SELECT COUNT(*) AS CNT FROM G_MEIMAST"
            'STR_SQL = " SELECT * FROM G_MEIMAST"
            '2020/02/06 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
            STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR�����N�� & "'"
            STR_SQL += " AND FURI_DATE_M ='" & STR�ĐU�֓� & "'"
            '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
            STR_SQL += " AND FURI_KBN_M ='" & Me.STR�I��U�֋敪 & "'"
            'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
            '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            '���ʐU�֓��A�����X�P�W���[�����͏����Ώۂ̊w�N�������Ɋ܂߂�B
            If Me.strSCH_KBN <> "0" Then
                '���ʐU�֓��A�����X�P�W���[��
                STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
                For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    If iGakunen_Flag(i) = 1 Then
                        STR_SQL += ","
                        STR_SQL += i.ToString
                    End If
                Next
                STR_SQL += ")"
            End If
            'IN��̃_�~�[�����폜
            STR_SQL = STR_SQL.Replace("XXX,", "")
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

            If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
                Exit Function
            End If

            '�����J�E���g
            '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            If OBJ_DATAREADER_DREAD.Read = True Then
                LNG�폜���� += GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))
            End If
            'While (OBJ_DATAREADER_DREAD.Read = True)
            '    LNG�폜���� += 1
            'End While
            '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If

            '�폜����
            STR_SQL = " DELETE  FROM G_MEIMAST"
            STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND SEIKYU_TAISYOU_M ='" & STR�����N�� & "'"
            STR_SQL += " AND FURI_DATE_M ='" & STR�ĐU�֓� & "'"
            '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
            STR_SQL += " AND FURI_KBN_M ='" & Me.STR�I��U�֋敪 & "'"
            'STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
            '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
            '���ʐU�֓��A�����X�P�W���[�����͏����Ώۂ̊w�N�������Ɋ܂߂�B
            If Me.strSCH_KBN <> "0" Then
                '���ʐU�֓��A�����X�P�W���[��
                STR_SQL += " AND GAKUNEN_CODE_M IN (XXX"
                For i As Integer = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    If iGakunen_Flag(i) = 1 Then
                        STR_SQL += ","
                        STR_SQL += i.ToString
                    End If
                Next
                STR_SQL += ")"
            End If
            'IN��̃_�~�[�����폜
            STR_SQL = STR_SQL.Replace("XXX,", "")
            '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '�폜�����G���[
                MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        '2020/02/06 �^�X�N�j�֓� DEL �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '���ʐU�֓��ŏ��U����������A�ĐU�����ʓ��̏ꍇ�ɁA���U�̎���ł��̐������ɓo�^����Ă�����ʐU�֓�����
        '���׃}�X�^�̍폜���s�����߁A�ʓr���׃}�X�^�̍폜��������������B
        'STR_SQL = " SELECT * FROM MEIMAST"
        'STR_SQL += " WHERE TORIS_CODE_K ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND TORIF_CODE_K ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        'STR_SQL += " AND FURI_DATE_K ='" & STR_FURIKAE_DATE(1) & "'"

        'If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
        '    Exit Function
        'End If

        'LNG��ƍ폜���� = 0
        ''�����J�E���g
        'While (OBJ_DATAREADER_DREAD.Read = True)
        '    LNG��ƍ폜���� += 1
        'End While

        'If GFUNC_SELECT_SQL3("", 1) = False Then
        '    Exit Function
        'End If

        ''�폜����
        'STR_SQL = " DELETE FROM MEIMAST"
        'STR_SQL += " WHERE TORIS_CODE_K ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND TORIF_CODE_K ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        'STR_SQL += " AND FURI_DATE_K ='" & STR_FURIKAE_DATE(1) & "'"

        'If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '    '�폜�����G���[
        '    MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    Exit Function
        'End If
        '2020/02/06 �^�X�N�j�֓� DEL �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

        PFUNC_MEISAI_DEL = True

    End Function
    Private Function PFUNC_SCHMAST_UPD() As Boolean

        Dim sSyori_Date As String

        PFUNC_SCHMAST_UPD = False

        '�X�P�W���[���}�X�^�̍X�V
        sSyori_Date = Format(Now, "yyyyMMddHHmmss")

        '���U�X�P�W���[���}�X�^�X�V
        STR_SQL = " UPDATE  G_SCHMAST SET "
        '���z�m�F�t���O
        STR_SQL += " CHECK_FLG_S ='0'"
        '�U�փf�[�^�쐬�t���O
        STR_SQL += ",DATA_FLG_S ='0'"
        STR_SQL += " WHERE GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '��ʂ̐U�֋敪�͕ϐ��Ɋi�[�ς݂Ȃ̂ŁA��������g�p����B
        STR_SQL += " AND FURI_KBN_S ='" & Me.STR�I��U�֋敪 & "'"
        'STR_SQL += " AND FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '�X�V�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        '�ĐU�X�P�W���[���}�X�^����
        If STR�ĐU�֓� <> "00000000" Then
            '�ĐU�X�P�W���[������̏ꍇ
            STR_SQL = " UPDATE  G_SCHMAST SET "
            If STR�ĐU�֓� <> "00000000" Then
                '�ĐU�f�[�^�쐬�t���O
                STR_SQL += " SAIFURI_FLG_S ='0'"
            End If

            STR_SQL += " WHERE GAKKOU_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND FURI_DATE_S ='" & STR�ĐU�֓� & "'"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '�X�V�����G���[
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        '2020/02/06 �^�X�N�j�֓� DEL �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '���ʐU�֓��ŏ��U����������A�ĐU�����ʓ��̏ꍇ�ɁA���U�̎���ł��̐������ɓo�^����Ă�����ʐU�֓�����
        '�X�P�W���[���}�X�^�̍폜���s�����߁A�ʓr�X�P�W���[���}�X�^�̍폜��������������B
        ''��Ǝ��U�A�g
        'STR_SQL = " UPDATE  SCHMAST SET "
        ''��t�t���O
        'STR_SQL += " UKETUKE_FLG_S ='0'"
        ''�o�^�t���O
        'STR_SQL += ",TOUROKU_FLG_S ='0'"
        '' 2016/04/25 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
        ''�X�P�W���[���̍X�V���ڂ̒ǉ��i��Ǝ��U�̗������ݎ���ɍ��킹��j
        'STR_SQL += ",SYORI_KEN_S = 0"
        'STR_SQL += ",SYORI_KIN_S = 0"
        'STR_SQL += ",ERR_KEN_S = 0"
        'STR_SQL += ",ERR_KIN_S = 0"
        'STR_SQL += ",TESUU_KIN_S = 0"
        'STR_SQL += ",TESUU_KIN1_S = 0"
        'STR_SQL += ",TESUU_KIN2_S = 0"
        'STR_SQL += ",TESUU_KIN3_S = 0"
        'STR_SQL += ",FURI_KEN_S = 0"
        'STR_SQL += ",FURI_KIN_S = 0"
        'STR_SQL += ",FUNOU_KEN_S = 0"
        'STR_SQL += ",FUNOU_KIN_S = 0"
        'STR_SQL += ",UKETUKE_DATE_S = '" & New String("0"c, 8) & "'"
        'STR_SQL += ",TOUROKU_DATE_S = '" & New String("0"c, 8) & "'"
        'STR_SQL += ",UFILE_NAME_S = NULL"
        'STR_SQL += ",ERROR_INF_S = NULL"
        '' 2016/04/25 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END
        'STR_SQL += " WHERE TORIS_CODE_S  ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND TORIF_CODE_S ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        'STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

        'If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '    '�X�V�����G���[
        '    MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    Exit Function
        'End If

        ''2018/10/23 saitou �L���M��(RSV2�Ή�) ADD ��K�͋@�\�ǉ� START
        'If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
        '    STR_SQL = " UPDATE SCHMAST_SUB SET "
        '    STR_SQL &= "  SYOUGOU_FLG_S = '0' "
        '    STR_SQL &= " ,SYOUGOU_DATE_S = '00000000' "
        '    STR_SQL &= " ,UKETUKE_TIME_STAMP_S = '00000000000000' "
        '    STR_SQL &= " WHERE TORIS_CODE_SSUB  ='" & txtGAKKOU_CODE.Text & "'"
        '    STR_SQL &= " AND TORIF_CODE_SSUB ='0" & (CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN)) + 1) & "'"
        '    STR_SQL &= " AND FURI_DATE_SSUB ='" & STR_FURIKAE_DATE(1) & "'"

        '    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '        '�X�V�����G���[
        '        MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        Exit Function
        '    End If
        'End If
        ''2018/10/23 saitou �L���M��(RSV2�Ή�) ADD ��K�͋@�\�ǉ� END
        '2020/02/06 �^�X�N�j�֓� DEL �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

        PFUNC_SCHMAST_UPD = True

    End Function
    Private Function PFUNC_ZENMEISAI_UPD() As Boolean
        Dim bLoopFlg As Boolean = False '�w��w�N�������ɒǉ��������`�F�b�N
        Dim iLcount As Integer '�w��w�N���[�v��

        PFUNC_ZENMEISAI_UPD = False

        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
        '�ĐU��ʂƏ��U�E�ĐU�ɕs�\�����݂������A���ݎ���Ώۂ̃X�P�W���[���̐U�֋敪�͉����ŏ������ύX�ɂȂ邽�߁A
        '�����̌����������{����B

        '�w�Z�}�X�^�̍ĐU��ʂƉ�ʑI������Ă���U�֋敪�ɂ���āA�O�񖾍ׂ̍X�V���s�������f
        Select Case Me.STR�ĐU���
            Case "0"
                '�ĐU��ʁu���v�E������ʁu���v
                PFUNC_ZENMEISAI_UPD = True
                Exit Function
            Case "1"
                '�ĐU��ʁu�L�v�E������ʁu���v
                If Me.STR�I��U�֋敪 = "0" Then
                    '�U�֋敪�u���U�v
                    PFUNC_ZENMEISAI_UPD = True
                    Exit Function
                End If
        End Select

        Dim ZenFuriDateArray(INT�g�p�w�N) As String

        '�O�񖾍׃}�X�^�擾
        '�����L�[�́A�w�Z�R�[�h�A�U�֌��ʃR�[�h�A�ĐU�σt���O�ŁA�e�w�N�̍ő�̐U�֓�
        Dim SQL As New System.Text.StringBuilder
        With SQL
            .Length = 0
            .Append("SELECT")
            .Append("     GAKUNEN_CODE_M")
            .Append("   , MAX(FURI_DATE_M) AS FURI_DATE_M")
            .Append(" FROM")
            .Append("     G_MEIMAST")
            .Append(" WHERE")
            .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
            .Append(" AND SAIFURI_SUMI_M = '1'")
            .Append(" AND FURI_KBN_M IN ('0', '1')")
            If Me.strSCH_KBN = "0" Then
                '�N�ԃX�P�W���[���̏ꍇ
                .Append(" AND GAKUNEN_CODE_M IN (XXX")
                For iLcount = MIN_SYS_GAKUNEN_CODE To INT�g�p�w�N
                    .Append(",").Append(iLcount.ToString)
                Next
                .Append(")")
            Else
                '���ʐU�֓��̏ꍇ
                .Append(" AND GAKUNEN_CODE_M IN (XXX")
                For iLcount = MIN_SYS_GAKUNEN_CODE To MAX_SYS_GAKUNEN_CODE
                    If iGakunen_Flag(iLcount) = 1 Then
                        .Append(",").Append(iLcount.ToString)
                    End If
                Next
                .Append(")")
            End If
            .Append(" GROUP BY")
            .Append("     GAKUNEN_CODE_M")
            .Append(" ORDER BY")
            .Append("     GAKUNEN_CODE_M")
            'IN��̃_�~�[�����폜
            .Replace("XXX,", "")
        End With

        If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If
            PFUNC_ZENMEISAI_UPD = True

            Exit Function
        End If

        For i As Integer = 0 To ZenFuriDateArray.Length - 1
            ZenFuriDateArray(i) = String.Empty
        Next

        While (OBJ_DATAREADER_DREAD.Read = True)
            '�O��U�֓��̎擾
            Dim GetGakunenCode As Integer = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("GAKUNEN_CODE_M"))
            ZenFuriDateArray(GetGakunenCode) = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M").ToString
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '�O��U�֓��Ɠ�����t�̖��׃}�X�^���X�V����O�ɁA�{���ɍX�V���Ă悢���`�F�b�N
        If Me.STR�I��U�֋敪 = "0" Then
            '�U�֋敪���u���U�v�̏ꍇ�A�O��U�֓���薢���ŕs�\���������݂��Ȃ��X�P�W���[�������邩�`�F�b�N����
            For iLcount = MIN_SYS_GAKUNEN_CODE To ZenFuriDateArray.Length - 1
                If ZenFuriDateArray(iLcount) <> String.Empty Then

                    Me.STR�O��U�֓� = ZenFuriDateArray(iLcount)

                    With SQL
                        .Length = 0
                        .Append("SELECT")
                        .Append("     *")
                        .Append(" FROM")
                        .Append("     G_SCHMAST")
                        .Append(" WHERE")
                        .Append("     GAKKOU_CODE_S = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                        .Append(" AND FURI_DATE_S > '").Append(Me.STR�O��U�֓�).Append("'")
                        .Append(" AND FURI_KBN_S IN ('0', '1')")
                        .Append(" AND FUNOU_FLG_S = '1'")
                        .Append(" AND GAKUNEN").Append(iLcount.ToString).Append("_FLG_S = '1'")
                        .Append(" ORDER BY")
                        .Append("     FURI_DATE_S")
                    End With

                    If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
                        Exit Function
                    End If

                    If OBJ_DATAREADER_DREAD.Read() = True Then
                        '�s�\�����̎擾
                        Dim FunouKen As Integer = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("FUNOU_KEN_S"))

                        Dim FuriDate As String = OBJ_DATAREADER_DREAD.Item("FURI_DATE_S").ToString
                        Dim FuriKbn As String = OBJ_DATAREADER_DREAD.Item("FURI_KBN_S").ToString

                        '���׃}�X�^�X�V�Ώۂ��`�F�b�N
                        '�O��U�֓���薢���ŕs�\���������݂��Ȃ��ꍇ�A
                        '�O��U�֓����獡�����U�֓��ԂŎ������������Ƃ���Ă��邽�߁A
                        '���׃}�X�^�̍X�V�͕s�v
                        If FunouKen = 0 Then
                            If GFUNC_SELECT_SQL3("", 1) = False Then
                                Exit Function
                            End If

                            Continue For
                            'PFUNC_ZENMEISAI_UPD = True
                            'Exit Function
                        Else
                            '�ēx�����Ŗ��׃}�X�^���猏���̎擾���s��
                            If GFUNC_SELECT_SQL3("", 1) = False Then
                                Exit Function
                            End If

                            With SQL
                                .Length = 0
                                .Append("SELECT")
                                .Append("     COUNT(*) AS CNT")
                                .Append(" FROM")
                                .Append("     G_MEIMAST")
                                .Append(" WHERE")
                                .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                                .Append(" AND FURI_DATE_M = '").Append(FuriDate).Append("'")
                                .Append(" AND FURI_KBN_M = '").Append(FuriKbn).Append("'")
                                .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
                                .Append(" AND GAKUNEN_CODE_M = ").Append(iLcount.ToString)
                            End With

                            If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
                                Exit Function
                            End If

                            If OBJ_DATAREADER_DREAD.Read() = True Then
                                '�s�\�����̎擾
                                FunouKen = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))

                                If FunouKen = 0 Then
                                    If GFUNC_SELECT_SQL3("", 1) = False Then
                                        Exit Function
                                    End If

                                    Continue For
                                End If
                            End If

                            If GFUNC_SELECT_SQL3("", 1) = False Then
                                Exit Function
                            End If
                        End If
                    Else
                        '���R�[�h�����݂��Ȃ��A���Ȃ킿�擾�����U�֓����O��U�֓��Ȃ̂ŁA���׃}�X�^���X�V����
                        If GFUNC_SELECT_SQL3("", 1) = False Then
                            Exit Function
                        End If
                    End If

                    '�O��U�֓��Ɠ�����t�̖��׃}�X�^���X�V����
                    With SQL
                        .Length = 0
                        .Append("UPDATE")
                        .Append("     G_MEIMAST")
                        .Append(" SET SAIFURI_SUMI_M = '0'")
                        .Append(" WHERE")
                        .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                        .Append(" AND FURI_DATE_M = '").Append(Me.STR�O��U�֓�).Append("'")
                        .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
                        .Append(" AND SAIFURI_SUMI_M = '1'")
                        .Append(" AND GAKUNEN_CODE_M = ").Append(iLcount.ToString)
                    End With

                    If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                        MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Function
                    End If
                End If
            Next
        Else
            '�U�֋敪���u�ĐU�v�̏ꍇ
            Me.STR�O��U�֓� = String.Empty

            '�ĐU�̏ꍇ�A�擾�����w�N���Ƃ̍ő�U�֓����炳��ɍő�̐U�֓����Ώ�
            For iLcount = MIN_SYS_GAKUNEN_CODE To ZenFuriDateArray.Length - 1
                If ZenFuriDateArray(iLcount) >= Me.STR�O��U�֓� Then
                    Me.STR�O��U�֓� = ZenFuriDateArray(iLcount)
                End If
            Next

            '�O��U�֓��Ɠ�����t�̖��׃}�X�^���X�V����
            With SQL
                .Length = 0
                .Append("UPDATE")
                .Append("     G_MEIMAST")
                .Append(" SET SAIFURI_SUMI_M = '0'")
                .Append(" WHERE")
                .Append("     GAKKOU_CODE_M = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                .Append(" AND FURI_DATE_M = '").Append(Me.STR�O��U�֓�).Append("'")
                .Append(" AND FURIKETU_CODE_M IN (").Append(SFuriCode).Append(")")
                .Append(" AND SAIFURI_SUMI_M = '1'")
                .Append(" AND GAKUNEN_CODE_M IN (XXX")
                For iLcount = MIN_SYS_GAKUNEN_CODE To ZenFuriDateArray.Length - 1
                    If ZenFuriDateArray(iLcount).Trim <> String.Empty Then
                        .Append(",").Append(iLcount.ToString)
                    End If
                Next
                .Append(")")
                'IN��̃_�~�[�����폜
                .Replace("XXX,", "")
            End With

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        PFUNC_ZENMEISAI_UPD = True

        ''�O�񖾍׃}�X�^�X�V�i�ĐU�σt���O�j
        ''�����L�[�́A�w�Z�R�[�h�A�U�֋敪�A�U�֌��ʃR�[�h�A�ĐU�σt���O�ŁA�U�֓��̍~��

        'STR_SQL = " SELECT * FROM G_MEIMAST"
        'STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        ''STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
        ''STR_SQL += " AND FURIKETU_CODE_M <> 0"
        'STR_SQL += " AND FURIKETU_CODE_M IN (" & SFuriCode & ")"
        '' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
        'STR_SQL += " AND SAIFURI_SUMI_M ='1'"
        'STR_SQL += " ORDER BY FURI_DATE_M desc"

        'If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
        '    Exit Function
        'End If

        'If OBJ_DATAREADER_DREAD.Read() = True Then

        '    '��ԋ߂��U�֓���O��U�֓��Ƃ��čĐU�σt���O��0�ɂ��� 2007/01/10
        '    '���U�̎��
        '    'Select case STR�I��U�֋敪
        '    '    Case "0"
        '    'Select case STR�ĐU���
        '    '    Case "1", "2"
        '    '        '�ĐU����̏ꍇ�Ŏ擾�������ׂ����U�̖��ׂ̏ꍇ
        '    '        '�O��̍ĐU�ŐU�����������Ă���̂Ŗ��׃}�X�^�̍X�V�͍s��Ȃ�
        '    '        If OBJ_DATAREADER_DREAD.Item("FURI_KBN_M") = 0 Then
        '    '            If GFUNC_SELECT_SQL3("", 1) = False Then
        '    '                Exit Function
        '    '            End If
        '    '            PFUNC_ZENMEISAI_UPD = True

        '    '            Exit Function
        '    '        End If
        '    'End Select
        '    'End Select

        '    STR�O��U�֓� = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M")
        'Else
        '    If GFUNC_SELECT_SQL3("", 1) = False Then
        '        Exit Function
        '    End If
        '    PFUNC_ZENMEISAI_UPD = True

        '    Exit Function
        'End If

        'If GFUNC_SELECT_SQL3("", 1) = False Then
        '    Exit Function
        'End If

        ''�O��U�֓��Ɠ�����t�̖��׃}�X�^���X�V����
        'STR_SQL = " UPDATE  G_MEIMAST SET "
        ''�ĐU�σt���O
        'STR_SQL += " SAIFURI_SUMI_M ='0'"
        'STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND FURI_DATE_M ='" & STR�O��U�֓� & "' "
        ''STR_SQL += " AND FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        '' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
        ''STR_SQL += " AND FURIKETU_CODE_M <> 0"
        'STR_SQL += " AND FURIKETU_CODE_M IN (" & SFuriCode & ")"
        '' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
        ' STR_SQL += " AND SAIFURI_SUMI_M ='1'"
        'If strSCH_KBN <> "0" Then '�ʏ�ȊO�̃X�P�W���[�����͊w�N�w�� 2005/12/09
        '    STR_SQL += " AND ("
        '    For iLcount = 1 To 9
        '        If iGakunen_Flag(iLcount) = 1 Then
        '            If bLoopFlg = True Then
        '                STR_SQL += " OR "
        '            End If
        '            STR_SQL += " GAKUNEN_CODE_M = " & iLcount
        '            bLoopFlg = True
        '        End If
        '    Next iLcount
        '    STR_SQL += " )"
        'End If

        'If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
        '    '�X�V�����G���[
        '    MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

        '    Exit Function
        'End If

        'PFUNC_ZENMEISAI_UPD = True
        '2020/02/05 �^�X�N�j�֓� CHG �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

    End Function

    '2020/02/05 �^�X�N�j�֓� DEL �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
    '���g�p���W�b�N�̂��߃R�����g�A�E�g
    'Private Function PFUNC_MEISAI_SAIFURI_UPD() As Boolean

    '    PFUNC_MEISAI_SAIFURI_UPD = False

    '    '���͂����U�֓����ĐU���ɊY�����鏉�U�X�P�W���[�����Q��
    '    STR_SQL = " SELECT * FROM G_SCHMAST"
    '    STR_SQL += " WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
    '    STR_SQL += " AND FURI_KBN_S ='0'"
    '    STR_SQL += " AND SFURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

    '    If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
    '        Exit Function
    '    End If

    '    '�擾�����X�P�W���[�����U�֓�(�ĐU�������������U�����擾)
    '    While (OBJ_DATAREADER_DREAD.Read = True)
    '        With OBJ_DATAREADER_DREAD
    '            STR_SQL = " UPDATE  G_MEIMAST SET "
    '            STR_SQL += " SAIFURI_SUMI_M ='0'"
    '            STR_SQL += " WHERE GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
    '            STR_SQL += " AND FURI_DATE_M ='" & .Item("FURI_DATE_S") & "'"
    '            STR_SQL += " AND FURI_KBN_M ='0'"

    '            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
    '                Exit Function
    '            End If
    '        End With
    '    End While

    '    If GFUNC_SELECT_SQL3("", 1) = False Then
    '        Exit Function
    '    End If

    '    PFUNC_MEISAI_SAIFURI_UPD = True

    'End Function
    '2020/02/05 �^�X�N�j�֓� DEL �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

    '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- START
    Private Function PFUNC_KIGYO_MEISAI_DEL() As Boolean

        PFUNC_KIGYO_MEISAI_DEL = False

        Dim SQL As System.Text.StringBuilder

        '�폜�����J�E���g
        SQL = New System.Text.StringBuilder
        With SQL
            .Length = 0
            .Append("SELECT")
            .Append("     COUNT(*) AS CNT")
            .Append(" FROM")
            .Append("     MEIMAST")
            .Append(" WHERE")
            .Append("     TORIS_CODE_K = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND TORIF_CODE_K = '").Append(CStr(CInt(Me.STR�I��U�֋敪) + 1).PadLeft(2, "0"c)).Append("'")
            .Append(" AND FURI_DATE_K = '").Append(STR_FURIKAE_DATE(1)).Append("'")
        End With

        If GFUNC_SELECT_SQL3(SQL.ToString, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.Read = True Then
            LNG��ƍ폜���� = GCom.NzInt(OBJ_DATAREADER_DREAD.Item("CNT"))
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        '�폜����
        With SQL
            .Length = 0
            .Append("DELETE FROM")
            .Append("    MEIMAST")
            .Append(" WHERE")
            .Append("     TORIS_CODE_K = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND TORIF_CODE_K = '").Append(CStr(CInt(Me.STR�I��U�֋敪) + 1).PadLeft(2, "0"c)).Append("'")
            .Append(" AND FURI_DATE_K = '").Append(STR_FURIKAE_DATE(1)).Append("'")
        End With

        If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
            '�폜�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        PFUNC_KIGYO_MEISAI_DEL = True

    End Function

    Private Function PFUNC_KIGYO_SCHMAST_UPD() As Boolean

        PFUNC_KIGYO_SCHMAST_UPD = False

        Dim SQL As System.Text.StringBuilder

        SQL = New System.Text.StringBuilder

        With SQL
            .Length = 0
            .Append("UPDATE")
            .Append("     SCHMAST")
            .Append(" SET UKETUKE_FLG_S = '0'")
            .Append("   , TOUROKU_FLG_S = '0'")
            .Append("   , SYORI_KEN_S = 0")
            .Append("   , SYORI_KIN_S = 0")
            .Append("   , ERR_KEN_S = 0")
            .Append("   , ERR_KIN_S = 0")
            .Append("   , TESUU_KIN_S = 0")
            .Append("   , TESUU_KIN1_S = 0")
            .Append("   , TESUU_KIN2_S = 0")
            .Append("   , TESUU_KIN3_S = 0")
            .Append("   , FURI_KEN_S = 0")
            .Append("   , FURI_KIN_S = 0")
            .Append("   , FUNOU_KEN_S = 0")
            .Append("   , FUNOU_KIN_S = 0")
            .Append("   , UKETUKE_DATE_S = '00000000'")
            .Append("   , TOUROKU_DATE_S = '00000000'")
            .Append("   , UFILE_NAME_S = NULL")
            .Append("   , ERROR_INF_S = NULL")
            .Append(" WHERE")
            .Append("     TORIS_CODE_S = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
            .Append(" AND TORIF_CODE_S = '").Append(CStr(CInt(Me.STR�I��U�֋敪) + 1).PadLeft(2, "0"c)).Append("'")
            .Append(" AND FURI_DATE_S = '").Append(STR_FURIKAE_DATE(1)).Append("'")
        End With

        If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
            '�X�V�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
            With SQL
                .Length = 0
                .Append("UPDATE")
                .Append("     SCHMAST_SUB")
                .Append(" SET SYOUGOU_FLG_S = '0'")
                .Append("   , SYOUGOU_DATE_S = '00000000'")
                .Append("   , UKETUKE_TIME_STAMP_S = '00000000000000'")
                .Append(" WHERE")
                .Append("     TORIS_CODE_SSUB = '").Append(Me.txtGAKKOU_CODE.Text).Append("'")
                .Append(" AND TORIF_CODE_SSUB = '").Append(CStr(CInt(Me.STR�I��U�֋敪) + 1).PadLeft(2, "0"c)).Append("'")
                .Append(" AND FURI_DATE_SSUB = '").Append(STR_FURIKAE_DATE(1)).Append("'")
            End With

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                '�X�V�����G���[
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If
        End If

        PFUNC_KIGYO_SCHMAST_UPD = True

    End Function
    '2020/02/06 �^�X�N�j�֓� ADD �W���őΉ��i�����U�փf�[�^�쐬�����ʉ��C�j -------------------- END

#End Region

End Class
