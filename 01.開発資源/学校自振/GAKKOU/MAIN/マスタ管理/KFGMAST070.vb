Option Explicit On 
Option Strict Off
Imports System.Text
Imports CASTCommon
Public Class KFGMAST070

#Region " ���ʕϐ���` "
    Dim STR�U�֓� As String
    Dim STR�G���g���f�[�^�\��� As String
    Dim STR�`�F�b�N�\��� As String
    Dim STR�U�փf�[�^�쐬�\��� As String
    Dim STR�s�\���ʍX�V�\��� As String
    Dim STR������ As String
    Dim LNG�Ǎ����� As Long

    Private Str_TimeStamp As String
    Private Str_Nentuki As String
    Private Str_Kessai_Yotei As String

    Private Str_Sch_Kbn As String
    Private Str_Sch_Name As String

    Public intCHECK_FLG As Integer = 0
    Public intDATA_FLG As Integer = 0
    Public STR�ĐU��� As String = ""
    Public STR�O��U�֓� As String = ""
    Public STR�I��U�֋敪 As String = ""
    Public iGakunen_Flag() As Integer
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST070", "�X�P�W���[�������e�i���X���")
    Private Const msgTitle As String = "�X�P�W���[�������e�i���X���(KFGMAST070)"

    ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

#End Region

#Region " Form Load "
    Private Sub KFGMAST070_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            STR������ = "�X�P�W���[���}�X�^�X�V"

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Call GCom.SetMonitorTopArea(Label95, Label94, lblUser, lblDate)

            '�e�L�X�g�t�@�C������R���{�{�b�N�X�ݒ�
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbFURI_KBN)")
                MessageBox.Show(String.Format(MSG0013E, "�U�֋敪"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '�U�֋敪�̏����l�ݒ�
            'cmbFURI_KBN.SelectedIndex = 0
            '���׍쐬�t���O
            txtENTRI_FLG.Text = "0"
            '���z�m�F�σt���O
            txtCHECK_FLG.Text = "0"
            '�U�փf�[�^�쐬��
            txtDATA_FLG.Text = "0"
            '�s�\���ʊm�F��
            txtFUNOU_FLG.Text = "0"
            '�ĐU�f�[�^�쐬��
            txtSAIFURI_FLG.Text = "0"
            '���ύ�
            txtKESSAI_FLG.Text = "0"
            '���f�t���O
            txtTYUUDAN_FLG.Text = "0"

            ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

            '���̓{�^������
            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnSansyo.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True

            If OBJ_CONNECTION_DREAD Is Nothing Then
                'Oracle �ڑ�(Read��p)
                OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
                'Oracle OPEN(Read��p)
                OBJ_CONNECTION_DREAD.Open()

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click

        '�X�V�{�^��
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")
            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check(0) = False Then
                Exit Sub
            End If

            '2006/10/12�@�ǉ��F�m�F���b�Z�[�W��\��
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            STR_COMMAND = "�X�V"
            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '�\����Čv�Z
            ' �G���g���f�[�^
            ' �`�F�b�N
            ' �f�[�^�쐬
            ' �s�\
            Call PSUB_Get_Yotei_Date()

            '�g�����U�N�V�����J�n
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
                Exit Sub
            End If

            STR_SQL = " UPDATE  G_SCHMAST SET "
            '�G���g���f�[�^�\���
            STR_SQL += " ENTRI_YDATE_S ='" & STR�G���g���f�[�^�\��� & "'"
            '�`�F�b�N�\���
            STR_SQL += ",CHECK_YDATE_S ='" & STR�`�F�b�N�\��� & "'"
            '�f�[�^�쐬�\���
            STR_SQL += ",DATA_YDATE_S ='" & STR�U�փf�[�^�쐬�\��� & "'"
            '�s�\�\���
            STR_SQL += ",FUNOU_YDATE_S ='" & STR�s�\���ʍX�V�\��� & "'"
            '�������ϗ\���
            STR_SQL += ",KESSAI_YDATE_S ='" & Str_Kessai_Yotei & "'"
            '���׍쐬�t���O
            STR_SQL += ",ENTRI_FLG_S ='" & txtENTRI_FLG.Text & "'"
            '���z�`�F�b�N�t���O
            STR_SQL += ",CHECK_FLG_S ='" & txtCHECK_FLG.Text & "'"
            '�U�փf�[�^�t���O
            STR_SQL += ",DATA_FLG_S ='" & txtDATA_FLG.Text & "'"
            '�s�\�`�F�b�N�t���O
            STR_SQL += ",FUNOU_FLG_S ='" & txtFUNOU_FLG.Text & "'"
            '�ĐU�t���O
            STR_SQL += ",SAIFURI_FLG_S ='" & txtSAIFURI_FLG.Text & "'"
            '���σt���O
            STR_SQL += ",KESSAI_FLG_S ='" & txtKESSAI_FLG.Text & "'"
            '���f�t���O
            STR_SQL += ",TYUUDAN_FLG_S ='" & txtTYUUDAN_FLG.Text & "'"
            '�^�C���X�^���v
            STR_SQL += ",TIME_STAMP_S ='" & Str_TimeStamp & "'"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S ='" & Str_Sch_Kbn & "'"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '�X�V�����G���[
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            '���s���̑O��s�\���̍ĐU�t���O��"0"�ɂ��� 2006/10/23
            '���z�m�F�t���O�̂�1��0�ɖ߂���
            If (intCHECK_FLG = 1 And txtCHECK_FLG.Text = "0") And (intDATA_FLG = 0 And txtDATA_FLG.Text = "0") Then
                '�O��U�֖��׃f�[�^�̍X�V
                Select Case STR�ĐU���
                    Case "1", "2", "3"   '�ĐU����A�J�z����
                        STR�I��U�֋敪 = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN)

                        If PFUNC_ZENMEISAI_UPD() = False Then
                            MainLOG.Write("�X�V", "���s", "�O��U�֖��׃f�[�^�X�V���s")
                            Exit Sub
                        End If
                End Select

            End If


            Call MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '���͍��ڐ���
            'Call PSUB_KEY_Enabled(True)

            '�w�Z�R�[�h���͂ɃJ�[�\���ʒu�t��
            'txtGAKKOU_CODE.Focus()

            '���̓{�^������
            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnSansyo.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        '�폜�{�^��
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�J�n", "����", "")
            Dim int�����t���O���� As Integer

            int�����t���O���� = 0

            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check(0) = False Then
                Exit Sub
            End If

            '2006/10/12�@�ǉ��F�m�F���b�Z�[�W��\��
            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '�����t���O�̗����Ă�����̂𒲂ׂ�
            STR_SQL = " SELECT * FROM G_SCHMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S ='" & Str_Sch_Kbn & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()

                Exit Sub
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            '�����t���O��ON�̃��R�[�h�̌������擾����
            While (OBJ_DATAREADER.Read = True)
                If OBJ_DATAREADER.Item("ENTRI_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("CHECK_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("DATA_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("FUNOU_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("SAIFURI_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("KESSAI_FLG_S") = "1" Or _
                   OBJ_DATAREADER.Item("TYUUDAN_FLG_S") = "1" Then
                    int�����t���O���� = int�����t���O���� + 1
                End If
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            If int�����t���O���� <> 0 Then
                MessageBox.Show(MSG0290W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '�S�폜�����A�L�[�͊w�Z�R�[�h�A�Ώ۔N�x�A�U�֋敪�A�U�֓�
            STR_SQL = " DELETE  FROM G_SCHMAST"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S ='" & Str_Sch_Kbn & "'"
            'STR_SQL += " AND"
            'STR_SQL += " SCH_KBN_S =0"
            STR_SQL += " AND"
            STR_SQL += " ENTRI_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " CHECK_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " DATA_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " FUNOU_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " SAIFURI_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " KESSAI_FLG_S =0"
            STR_SQL += " AND"
            STR_SQL += " TYUUDAN_FLG_S =0"

            '�g�����U�N�V�����J�n
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '�폜�����G���[
                MainLOG.Write("�폜", "���s", "�X�P�W���[���̍폜�����ŃG���[")
                MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            '�\�����ڂ̃N���A
            Call PSUB_GAMEN_CLEAR()

            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Call PSUB_KEY_Enabled(True)

            '�w�Z�R�[�h���͂ɃJ�[�\���ʒu�t��
            txtGAKKOU_CODE.Focus()

            '���̓{�^������
            btnKousin.Enabled = True
            btnDelete.Enabled = True
            btnSansyo.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True

            '�ǉ� 2007/02/15
            txtTAISYONEN.Text = ""
            txtTAISYOUTUKI.Text = ""
            txtFURI_DATENEN.Text = ""
            txtFURI_DATETUKI.Text = ""
            txtFURI_DATEHI.Text = ""
            cmbFURI_KBN.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnSansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyo.Click
        '�Q�ƃ{�^��
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")
            If PFUNC_Nyuryoku_Check(1) = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '�X�P�W���[���}�X�^���݃`�F�b�N
            STR_SQL = " SELECT G_SCHMAST.* , FILE_NAME_T,SFURI_SYUBETU_T FROM G_SCHMAST , GAKMAST2"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
            STR_SQL += " AND"
            STR_SQL += " GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND"
            STR_SQL += " NENGETUDO_S ='" & Str_Nentuki & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MainLOG.Write("�Q��", "���s", "�X�P�W���[���}�X�^�Q�Ə����łO��")
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()

                Exit Sub
            End If

            '��ɊY���X�P�W���[���}�X�^�̌����𒲂ׂ�
            LNG�Ǎ����� = 0

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            While (OBJ_DATAREADER.Read = True)
                LNG�Ǎ����� += 1
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            Select Case LNG�Ǎ�����
                Case Is = 1
                    If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                        Exit Sub
                    End If

                    While (OBJ_DATAREADER.Read = True)
                        Str_Sch_Kbn = OBJ_DATAREADER.Item("SCH_KBN_S")

                        '�ϑ��҃R�[�h
                        txtITAKU_CODE.Text = OBJ_DATAREADER.Item("ITAKU_CODE_S")
                        '�ĐU�֓�
                        txtSFURI_DATENEN.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 1, 4)
                        txtSFURI_DATETUKI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 5, 2)
                        txtSFURI_DATEHI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 7, 2)

                        '�G���g���[�f�[�^�\���
                        txtENTRI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 1, 4)
                        txtENTRI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 5, 2)
                        txtENTRI_YDATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 7, 2)
                        '�G���g���[�f�[�^��
                        txtENTRI_DATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 1, 4)
                        txtENTRI_DATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 5, 2)
                        txtENTRI_DATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 7, 2)
                        '�`�F�b�N�\���
                        txtCHECK_YDATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 1, 4)
                        txtCHECK_YDATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 5, 2)
                        txtCHECK_YDATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 7, 2)
                        '�`�F�b�N��
                        txtCHECK_DATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 1, 4)
                        txtCHECK_DATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 5, 2)
                        txtCHECK_DATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 7, 2)
                        '�f�[�^�쐬�\���
                        txtDATA_YDATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 1, 4)
                        txtDATA_YDATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 5, 2)
                        txtDATA_YDATED.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 7, 2)
                        '�f�[�^�쐬��
                        txtDATA_DATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 1, 4)
                        txtDATA_DATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 5, 2)
                        txtDATA_DATED.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 7, 2)
                        '�s�\�\���
                        txtFUNOU_YDATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 1, 4)
                        txtFUNOU_YDATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 5, 2)
                        txtFUNOU_YDATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 7, 2)
                        '�s�\��
                        txtFUNOU_DATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 1, 4)
                        txtFUNOU_DATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 5, 2)
                        txtFUNOU_DATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 7, 2)
                        '�������ϗ\���
                        txtKESSAI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 1, 4)
                        txtKESSAI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 5, 2)
                        txtKESSAI_YDATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 7, 2)
                        '�������ϓ�
                        txtKESSAI_DATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 1, 4)
                        txtKESSAI_DATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 5, 2)
                        txtKESSAI_DATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 7, 2)
                        '���׍쐬
                        txtENTRI_FLG.Text = OBJ_DATAREADER.Item("ENTRI_FLG_S")
                        '���z�m�F��
                        txtCHECK_FLG.Text = OBJ_DATAREADER.Item("CHECK_FLG_S")
                        '�ǉ� 2006/10/23
                        intCHECK_FLG = CInt(OBJ_DATAREADER.Item("CHECK_FLG_S"))
                        '�U�փf�[�^�쐬��
                        txtDATA_FLG.Text = OBJ_DATAREADER.Item("DATA_FLG_S")
                        '�ǉ� 2006/10/23
                        intDATA_FLG = CInt(OBJ_DATAREADER.Item("DATA_FLG_S"))
                        '�s�\���ʊm�F��
                        txtFUNOU_FLG.Text = OBJ_DATAREADER.Item("FUNOU_FLG_S")
                        '�ĐU�f�[�^�쐬��
                        txtSAIFURI_FLG.Text = OBJ_DATAREADER.Item("SAIFURI_FLG_S")
                        '���ύ�
                        txtKESSAI_FLG.Text = OBJ_DATAREADER.Item("KESSAI_FLG_S")
                        '���f�t���O
                        txtTYUUDAN_FLG.Text = OBJ_DATAREADER.Item("TYUUDAN_FLG_S")
                        '��������
                        txtSYORI_KEN.Text = Format(OBJ_DATAREADER.Item("SYORI_KEN_S"), "#,##0")
                        '�������z
                        txtSYORI_KIN.Text = Format(OBJ_DATAREADER.Item("SYORI_KIN_S"), "#,##0")
                        '�萔���v�Z�敪
                        txtTESUU_KBN.Text = OBJ_DATAREADER.Item("TESUU_KBN_S")
                        '�U�֍ό���
                        txtFURI_KEN.Text = Format(OBJ_DATAREADER.Item("FURI_KEN_S"), "#,##0")
                        '�U�֍ϋ��z
                        txtFURI_KIN.Text = Format(OBJ_DATAREADER.Item("FURI_KIN_S"), "#,##0")
                        '�萔��
                        txtTESUU_KIN.Text = Format(OBJ_DATAREADER.Item("TESUU_KIN_S"), "#,##0")
                        '�s�\����
                        txtFUNOU_KEN.Text = Format(OBJ_DATAREADER.Item("FUNOU_KEN_S"), "#,##0")
                        '�s�\���z
                        txtFUNOU_KIN.Text = Format(OBJ_DATAREADER.Item("FUNOU_KIN_S"), "#,##0")

                        If IsDBNull(OBJ_DATAREADER.Item("FILE_NAME_T")) = False Then
                            '���M�t�@�C����
                            txtSousinFile.Text = OBJ_DATAREADER.Item("FILE_NAME_T")
                        Else
                            '���M�t�@�C����
                            txtSousinFile.Text = ""
                        End If

                        '�쐬���t
                        txtSAKUSEI_DATEY.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 1, 4)
                        txtSAKUSEI_DATEM.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 5, 2)
                        txtSAKUSEI_DATED.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 7, 2)

                        '�ĐU��ʐݒ� 2006/10/23
                        STR�ĐU��� = OBJ_DATAREADER.Item("SFURI_SYUBETU_T")
                        ReDim iGakunen_Flag(0)  '������
                        For iGAK_CNT As Integer = 1 To 9
                            ReDim Preserve iGakunen_Flag(iGAK_CNT)
                            If OBJ_DATAREADER.Item("GAKUNEN" & iGAK_CNT & "_FLG_S") = "1" Then
                                iGakunen_Flag(iGAK_CNT) = 1
                            Else
                                iGakunen_Flag(iGAK_CNT) = 0
                            End If
                        Next
                    End While

                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Sub
                    End If

                Case Is > 1
                    If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                        Exit Sub
                    End If

                    Dim ICnt As Integer = 0

                    While (OBJ_DATAREADER.Read = True)

                        Dim str���o�敪 As String = ""

                        Select Case OBJ_DATAREADER.Item("FURI_KBN_S")
                            Case "0"
                                str���o�敪 = "���U"
                            Case "1"
                                str���o�敪 = "�ĐU"
                            Case "2"
                                str���o�敪 = "����"
                            Case "3"
                                str���o�敪 = "�o��"
                        End Select

                        '�X�P�W���[���敪
                        Str_Sch_Kbn = OBJ_DATAREADER.Item("SCH_KBN_S")

                        Select Case CInt(Str_Sch_Kbn)
                            Case 0
                                Str_Sch_Name = "�N�ԃX�P�W���[��"
                            Case 1
                                Str_Sch_Name = "���ʃX�P�W���[��"
                            Case 2
                                Str_Sch_Name = "�����X�P�W���[��"
                        End Select

                        '�m�F���b�Z�[�W
                        '���b�Z�[�W��YES�̏ꍇ���ݓǂݍ��ݒ��̃��R�[�h����ʂɐݒ肷��
                        '���b�Z�[�W��NO�̏ꍇ�͎����R�[�h��ǂݍ���
                        If MessageBox.Show(String.Format(G_MSG0011I, Str_Sch_Name, _
                                                                     Mid(OBJ_DATAREADER.Item("NENGETUDO_S"), 1, 4) & "�N" & Mid(OBJ_DATAREADER.Item("NENGETUDO_S"), 5, 2) & "���x", _
                                                                     str���o�敪, _
                                                                     Mid(OBJ_DATAREADER.Item("FURI_DATE_S"), 1, 4) & "/" & Mid(OBJ_DATAREADER.Item("FURI_DATE_S"), 5, 2) & "/" & Mid(OBJ_DATAREADER.Item("FURI_DATE_S"), 7, 2), _
                                                                     Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 1, 4) & "/" & Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 5, 2) & "/" & Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 7, 2)), _
                                           msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                            Str_Sch_Kbn = OBJ_DATAREADER.Item("SCH_KBN_S")

                            '���Y���R�[�h�̊m�菈��
                            '�ϑ��҃R�[�h
                            txtITAKU_CODE.Text = OBJ_DATAREADER.Item("ITAKU_CODE_S")
                            '�ĐU�֓�
                            txtSFURI_DATENEN.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 1, 4)
                            txtSFURI_DATETUKI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 5, 2)
                            txtSFURI_DATEHI.Text = Mid(OBJ_DATAREADER.Item("SFURI_DATE_S"), 7, 2)

                            '�G���g���[�f�[�^�\���
                            txtENTRI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 1, 4)
                            txtENTRI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 5, 2)
                            txtENTRI_YDATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_YDATE_S"), 7, 2)
                            '�G���g���[�f�[�^��
                            txtENTRI_DATEY.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 1, 4)
                            txtENTRI_DATEM.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 5, 2)
                            txtENTRI_DATED.Text = Mid(OBJ_DATAREADER.Item("ENTRI_DATE_S"), 7, 2)
                            '�`�F�b�N�\���
                            txtCHECK_YDATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 1, 4)
                            txtCHECK_YDATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 5, 2)
                            txtCHECK_YDATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_YDATE_S"), 7, 2)
                            '�`�F�b�N��
                            txtCHECK_DATEY.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 1, 4)
                            txtCHECK_DATEM.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 5, 2)
                            txtCHECK_DATED.Text = Mid(OBJ_DATAREADER.Item("CHECK_DATE_S"), 7, 2)
                            '�f�[�^�쐬�\���
                            txtDATA_YDATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 1, 4)
                            txtDATA_YDATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 5, 2)
                            txtDATA_YDATED.Text = Mid(OBJ_DATAREADER.Item("DATA_YDATE_S"), 7, 2)
                            '�f�[�^�쐬��
                            txtDATA_DATEY.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 1, 4)
                            txtDATA_DATEM.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 5, 2)
                            txtDATA_DATED.Text = Mid(OBJ_DATAREADER.Item("DATA_DATE_S"), 7, 2)
                            '�s�\�\���
                            txtFUNOU_YDATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 1, 4)
                            txtFUNOU_YDATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 5, 2)
                            txtFUNOU_YDATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_YDATE_S"), 7, 2)
                            '�s�\��
                            txtFUNOU_DATEY.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 1, 4)
                            txtFUNOU_DATEM.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 5, 2)
                            txtFUNOU_DATED.Text = Mid(OBJ_DATAREADER.Item("FUNOU_DATE_S"), 7, 2)
                            '�������ϗ\���
                            txtKESSAI_YDATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 1, 4)
                            txtKESSAI_YDATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 5, 2)
                            txtKESSAI_YDATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_YDATE_S"), 7, 2)
                            '�������ϓ�
                            txtKESSAI_DATEY.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 1, 4)
                            txtKESSAI_DATEM.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 5, 2)
                            txtKESSAI_DATED.Text = Mid(OBJ_DATAREADER.Item("KESSAI_DATE_S"), 7, 2)
                            '���׍쐬
                            txtENTRI_FLG.Text = OBJ_DATAREADER.Item("ENTRI_FLG_S")
                            '���z�m�F��
                            txtCHECK_FLG.Text = OBJ_DATAREADER.Item("CHECK_FLG_S")
                            '�U�փf�[�^�쐬��
                            txtDATA_FLG.Text = OBJ_DATAREADER.Item("DATA_FLG_S")
                            '�s�\���ʊm�F��
                            txtFUNOU_FLG.Text = OBJ_DATAREADER.Item("FUNOU_FLG_S")
                            '�ĐU�f�[�^�쐬��
                            txtSAIFURI_FLG.Text = OBJ_DATAREADER.Item("SAIFURI_FLG_S")
                            '���ύ�
                            txtKESSAI_FLG.Text = OBJ_DATAREADER.Item("KESSAI_FLG_S")
                            '���f�t���O
                            txtTYUUDAN_FLG.Text = OBJ_DATAREADER.Item("TYUUDAN_FLG_S")
                            '��������
                            txtSYORI_KEN.Text = OBJ_DATAREADER.Item("SYORI_KEN_S")
                            '�������z
                            txtSYORI_KIN.Text = OBJ_DATAREADER.Item("SYORI_KIN_S")
                            '�萔���v�Z�敪
                            txtTESUU_KBN.Text = OBJ_DATAREADER.Item("TESUU_KBN_S")
                            '�U�֍ό���
                            txtFURI_KEN.Text = OBJ_DATAREADER.Item("FURI_KEN_S")
                            '�U�֍ϋ��z
                            txtFURI_KIN.Text = OBJ_DATAREADER.Item("FURI_KIN_S")
                            '�萔��
                            txtTESUU_KIN.Text = OBJ_DATAREADER.Item("TESUU_KIN_S")
                            '�s�\����
                            txtFUNOU_KEN.Text = OBJ_DATAREADER.Item("FUNOU_KEN_S")
                            '�s�\���z
                            txtFUNOU_KIN.Text = OBJ_DATAREADER.Item("FUNOU_KIN_S")

                            If IsDBNull(OBJ_DATAREADER.Item("FILE_NAME_T")) = False Then
                                '���M�t�@�C����
                                txtSousinFile.Text = OBJ_DATAREADER.Item("FILE_NAME_T")
                            Else
                                '���M�t�@�C����
                                txtSousinFile.Text = ""
                            End If
                            '�쐬���t
                            txtSAKUSEI_DATEY.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 1, 4)
                            txtSAKUSEI_DATEM.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 5, 2)
                            txtSAKUSEI_DATED.Text = Mid(OBJ_DATAREADER.Item("SAKUSEI_DATE_S"), 7, 2)

                            '�ĐU��ʐݒ� 2006/10/23
                            STR�ĐU��� = OBJ_DATAREADER.Item("SFURI_SYUBETU_T")
                            Dim iGAK_CNT As Integer
                            ReDim iGakunen_Flag(0)  '������
                            For iGAK_CNT = 1 To 9
                                ReDim Preserve iGakunen_Flag(iGAK_CNT)
                                If OBJ_DATAREADER.Item("GAKUNEN" & iGAK_CNT & "_FLG_S") = "1" Then
                                    iGakunen_Flag(iGAK_CNT) = 1
                                Else
                                    iGakunen_Flag(iGAK_CNT) = 0
                                End If
                            Next
                            Exit While
                        End If

                        ICnt += 1

                    End While

                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Sub
                    End If
            End Select

            '�L�[���ڂ̓��͕s�Ƃ���
            Call PSUB_KEY_Enabled(False)

            '�U�֓��̔N�Ƀt�H�[�J�X�ݒ�

            txtFURI_DATENEN.Focus()

            '���̓{�^������
            btnKousin.Enabled = True
            btnDelete.Enabled = True
            btnSansyo.Enabled = False
            btnEraser.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '����{�^��
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            Str_Sch_Kbn = ""
            '��ʂ̃N���A
            Call PSUB_GAMEN_CLEAR()

            '�L�[���ڂ̓��͉Ƃ���
            Call PSUB_KEY_Enabled(True)

            '�w�Z�R�[�h���͂ɃJ�[�\���ʒu�t��
            txtGAKKOU_CODE.Focus()

            '���̓{�^������
            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnSansyo.Enabled = True
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
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)��O�G���[", "���s", ex.Message)
        End Try
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_Get_Yotei_Date()

        '�G���g���[�f�[�^�\����Čv�Z
        '���������i�����A�o���j�́A�U�֓���ݒ�
        Select Case GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURI_KBN)
            Case "02", "03"
                STR�G���g���f�[�^�\��� = STR_FURIKAE_DATE(1)

                txtENTRI_YDATEY.Text = Mid(STR�G���g���f�[�^�\���, 1, 4)
                txtENTRI_YDATEM.Text = Mid(STR�G���g���f�[�^�\���, 5, 2)
                txtENTRI_YDATED.Text = Mid(STR�G���g���f�[�^�\���, 7, 2)
            Case Else
                STR�G���g���f�[�^�\��� = "00000000"

                txtENTRI_YDATEY.Text = "0000"
                txtENTRI_YDATEM.Text = "00"
                txtENTRI_YDATED.Text = "00"
        End Select

        '�`�F�b�N�\����Z�o
        STR�`�F�b�N�\��� = PFUNC_EIGYOUBI_GET(STR_FURIKAE_DATE(1), STR_JIFURI_CHECK, "-")
        If STR�`�F�b�N�\��� <> "" Then
            txtCHECK_YDATEY.Text = Mid(STR�`�F�b�N�\���, 1, 4)
            txtCHECK_YDATEM.Text = Mid(STR�`�F�b�N�\���, 5, 2)
            txtCHECK_YDATED.Text = Mid(STR�`�F�b�N�\���, 7, 2)
        Else
            txtCHECK_YDATEY.Text = ""
            txtCHECK_YDATEM.Text = ""
            txtCHECK_YDATED.Text = ""
        End If

        '�U�փf�[�^�쐬�\����Z�o
        STR�U�փf�[�^�쐬�\��� = PFUNC_EIGYOUBI_GET(STR_FURIKAE_DATE(1), STR_JIFURI_HAISIN, "-")
        If STR�U�փf�[�^�쐬�\��� <> "" Then
            txtDATA_YDATEY.Text = Mid(STR�U�փf�[�^�쐬�\���, 1, 4)
            txtDATA_YDATEM.Text = Mid(STR�U�փf�[�^�쐬�\���, 5, 2)
            txtDATA_YDATED.Text = Mid(STR�U�փf�[�^�쐬�\���, 7, 2)
        Else
            txtDATA_YDATEY.Text = ""
            txtDATA_YDATEM.Text = ""
            txtDATA_YDATED.Text = ""
        End If

        '�s�\���ʍX�V�\����Z�o
        STR�s�\���ʍX�V�\��� = PFUNC_EIGYOUBI_GET(STR_FURIKAE_DATE(1), STR_JIFURI_FUNOU, "+")
        If STR�s�\���ʍX�V�\��� <> "" Then
            txtFUNOU_YDATEY.Text = Mid(STR�s�\���ʍX�V�\���, 1, 4)
            txtFUNOU_YDATEM.Text = Mid(STR�s�\���ʍX�V�\���, 5, 2)
            txtFUNOU_YDATED.Text = Mid(STR�s�\���ʍX�V�\���, 7, 2)
        Else
            txtFUNOU_YDATEY.Text = ""
            txtFUNOU_YDATEM.Text = ""
            txtFUNOU_YDATED.Text = ""
        End If

        '�^�C���X�^���v
        Str_TimeStamp = Format(Now, "yyyyMMddHHmmss")

    End Sub
    Private Sub PSUB_GAMEN_CLEAR()

        '��ʂ̕\�����ڃN���A
        '�ϑ��҃R�[�h
        txtITAKU_CODE.Text = ""
        '�ĐU�֓�
        txtSFURI_DATENEN.Text = ""
        txtSFURI_DATETUKI.Text = ""
        txtSFURI_DATEHI.Text = ""

        '�G���g���[�f�[�^�\���
        txtENTRI_YDATEY.Text = ""
        txtENTRI_YDATEM.Text = ""
        txtENTRI_YDATED.Text = ""
        '�G���g���[�f�[�^��
        txtENTRI_DATEY.Text = ""
        txtENTRI_DATEM.Text = ""
        txtENTRI_DATED.Text = ""
        '�`�F�b�N�\���
        txtCHECK_YDATEY.Text = ""
        txtCHECK_YDATEM.Text = ""
        txtCHECK_YDATED.Text = ""
        '�`�F�b�N��
        txtCHECK_DATEY.Text = ""
        txtCHECK_DATEM.Text = ""
        txtCHECK_DATED.Text = ""
        '�f�[�^�쐬�\���
        txtDATA_YDATEY.Text = ""
        txtDATA_YDATEM.Text = ""
        txtDATA_YDATED.Text = ""
        '�f�[�^�쐬��
        txtDATA_DATEY.Text = ""
        txtDATA_DATEM.Text = ""
        txtDATA_DATED.Text = ""
        '�s�\�\���
        txtFUNOU_YDATEY.Text = ""
        txtFUNOU_YDATEM.Text = ""
        txtFUNOU_YDATED.Text = ""
        '�s�\��
        txtFUNOU_DATEY.Text = ""
        txtFUNOU_DATEM.Text = ""
        txtFUNOU_DATED.Text = ""
        '�������ϗ\���
        txtKESSAI_YDATEY.Text = ""
        txtKESSAI_YDATEM.Text = ""
        txtKESSAI_YDATED.Text = ""
        '�������ϓ�
        txtKESSAI_DATEY.Text = ""
        txtKESSAI_DATEM.Text = ""
        txtKESSAI_DATED.Text = ""
        '���׍쐬�t���O
        txtENTRI_FLG.Text = "0"
        '���z�m�F�σt���O
        txtCHECK_FLG.Text = "0"
        '�U�փf�[�^�쐬��
        txtDATA_FLG.Text = "0"
        '�s�\���ʊm�F��
        txtFUNOU_FLG.Text = "0"
        '�ĐU�f�[�^�쐬��
        txtSAIFURI_FLG.Text = "0"
        '���ύ�
        txtKESSAI_FLG.Text = "0"
        '���f�t���O
        txtTYUUDAN_FLG.Text = "0"
        '��������
        txtSYORI_KEN.Text = ""
        '�������z
        txtSYORI_KIN.Text = ""
        '�萔���v�Z�敪
        txtTESUU_KBN.Text = ""
        '�U�֍ό���
        txtFURI_KEN.Text = ""
        '�U�֍ϋ��z
        txtFURI_KIN.Text = ""
        '�萔��
        txtTESUU_KIN.Text = ""
        '�s�\����
        txtFUNOU_KEN.Text = ""
        '�s�\���z
        txtFUNOU_KIN.Text = ""
        '���M�t�@�C����
        txtSousinFile.Text = ""
        '�쐬���t
        txtSAKUSEI_DATEY.Text = ""
        txtSAKUSEI_DATEM.Text = ""
        txtSAKUSEI_DATED.Text = ""

    End Sub
    Private Sub PSUB_KEY_Enabled(ByVal pValue As Boolean)

        '�L�[���ڂ��g�p�Ƃ���
        cmbKana.Enabled = pValue
        cmbGakkouName.Enabled = pValue

        txtFURI_DATENEN.Enabled = pValue
        txtFURI_DATETUKI.Enabled = pValue
        txtFURI_DATEHI.Enabled = pValue

        txtGAKKOU_CODE.Enabled = pValue
        txtTAISYONEN.Enabled = pValue
        txtTAISYOUTUKI.Enabled = pValue

        cmbFURI_KBN.Enabled = pValue

    End Sub
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

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
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If
        '�w�Z������̊w�Z�R�[�h�ݒ�
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        '�w�Z���̎擾
        lab�w�Z��.Text = cmbGakkouName.Text

        ' 2017/05/26 �^�X�N�j���� ADD �yME�z(RSV2�Ή� �t�H�[�J�X�ݒ�) -------------------- START
        txtTAISYONEN.Focus()
        ' 2017/05/26 �^�X�N�j���� ADD �yME�z(RSV2�Ή� �t�H�[�J�X�ݒ�) -------------------- END

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z���̎擾
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 �w�Z�R�[�h�[������
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '�w�Z���̎擾
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If

    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtENTRI_FLG.Validating, _
    txtCHECK_FLG.Validating, _
    txtDATA_FLG.Validating, _
    txtFUNOU_FLG.Validating, _
    txtSAIFURI_FLG.Validating, _
    txtKESSAI_FLG.Validating, _
    txtTYUUDAN_FLG.Validating

        If CType(sender, TextBox).Text.Trim = "" Then
            CType(sender, TextBox).Text = "0"
        End If

    End Sub
    Private Sub txtTYUUDAN_FLG_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTYUUDAN_FLG.Validating
        If txtTYUUDAN_FLG.Text.Length = 1 Then
            If btnSansyo.Enabled = True Then
                btnSansyo.Focus()
            ElseIf btnKousin.Enabled = True Then
                btnKousin.Focus()
            End If
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check(ByVal pIndex As Integer) As Boolean

        PFUNC_Nyuryoku_Check = False

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '�w�Z�}�X�^���݃`�F�b�N
            STR_SQL = "SELECT *"
            STR_SQL += " FROM GAKMAST2"
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()

                Exit Function
            End If
        End If

        '�Ώ۔N
        If Trim(txtTAISYONEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYONEN.Focus()
            Exit Function
            'Else
            '    If CStr(CLng(txtTAISYONEN.Text)).Length <> 4 Then
            '        MessageBox.Show("�w�肵���Ώ۔N���Ԉ���Ă��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtTAISYONEN.Focus()

            '        Exit Function
            '    End If
        End If

        '�Ώی�
        If Trim(txtTAISYOUTUKI.Text) = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYOUTUKI.Focus()
            Exit Function
        Else
            Select Case CInt(txtTAISYOUTUKI.Text)
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTAISYOUTUKI.Focus()

                    Exit Function
            End Select
        End If

        Str_Nentuki = txtTAISYONEN.Text & Format(CInt(txtTAISYOUTUKI.Text), "00")

        '�U�֓�
        If Trim(txtFURI_DATENEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURI_DATENEN.Focus()

            Exit Function
            'Else
            '    If CStr(CLng(txtFURI_DATENEN.Text)).Trim.Length <> 4 Then
            '        MessageBox.Show("�w�肵���U�֓�(�N)���Ԉ���Ă��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFURI_DATENEN.Focus()

            '        Exit Function
            '    End If
        End If

        If Trim(txtFURI_DATETUKI.Text) = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURI_DATETUKI.Focus()

            Exit Function
        Else
            Select Case CInt(txtFURI_DATETUKI.Text)
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFURI_DATETUKI.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFURI_DATEHI.Text) = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFURI_DATEHI.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtFURI_DATENEN.Text) & "/" & Trim(txtFURI_DATETUKI.Text) & "/" & Trim(txtFURI_DATEHI.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFURI_DATENEN.Text) & Format(CInt(txtFURI_DATETUKI.Text), "00") & Format(CInt(txtFURI_DATEHI.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            txtFURI_DATENEN.Focus()

            Exit Function
        End If

        Select Case pIndex
            Case 0
                '�������ϗ\���
                If Trim(txtKESSAI_YDATEY.Text) = "" Then
                    MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKESSAI_YDATEY.Focus()

                    Exit Function
                    'Else
                    '    If CStr(CLng(txtKESSAI_YDATEY.Text)).Trim.Length <> 4 Then
                    '        MessageBox.Show("�w�肵���������ϗ\���(�N)���Ԉ���Ă��܂�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '        txtKESSAI_YDATEY.Focus()

                    '        Exit Function
                    '    End If

                End If

                If Trim(txtKESSAI_YDATEM.Text) = "" Then
                    MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKESSAI_YDATEM.Focus()

                    Exit Function
                Else
                    Select Case CInt(txtKESSAI_YDATEM.Text)
                        Case 1 To 12
                        Case Else
                            MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtKESSAI_YDATEM.Focus()

                            Exit Function
                    End Select
                End If

                If Trim(txtKESSAI_YDATED.Text) = "" Then
                    MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKESSAI_YDATED.Focus()

                    Exit Function
                End If

                Str_Kessai_Yotei = Trim(txtKESSAI_YDATEY.Text) & "/" & Trim(txtKESSAI_YDATEM.Text) & "/" & Trim(txtKESSAI_YDATED.Text)

                If Not IsDate(Str_Kessai_Yotei) Then
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    txtKESSAI_YDATEY.Focus()

                    Exit Function
                End If

                Str_Kessai_Yotei = Trim(txtKESSAI_YDATEY.Text) & Format(CInt(txtKESSAI_YDATEM.Text), "00") & Format(CInt(txtKESSAI_YDATED.Text), "00")

                '���׍쐬�t���O
                Select Case Trim(txtENTRI_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "���׍쐬�t���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '���z�`�F�b�N�t���O
                Select Case Trim(txtCHECK_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "���z�`�F�b�N�t���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '�U�փf�[�^�t���O
                Select Case Trim(txtDATA_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "�U�փf�[�^�t���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '�s�\�t���O
                Select Case Trim(txtFUNOU_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "�s�\�t���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '�ĐU�t���O
                Select Case Trim(txtSAIFURI_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "�ĐU�t���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '���σt���O
                Select Case Trim(txtKESSAI_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "���σt���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select

                '���f�t���O
                Select Case Trim(txtTYUUDAN_FLG.Text)
                    Case "0", "1"
                    Case Else
                        MessageBox.Show(String.Format(MSG0294W, "���f�t���O"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                End Select
        End Select

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_EIGYOUBI_GET(ByVal str�N���� As String, _
                                        ByVal str���� As String, _
                                        ByVal str�O��c�Ɠ��敪 As String) As String

        '�c�Ɠ��Z�o
        Dim WORK_DATE As Date
        Dim YOUBI As Long
        Dim HOSEI As Integer
        'Dim FLG As Integer
        Dim int���� As Integer



        PFUNC_EIGYOUBI_GET = ""

        int���� = CInt(str����)
        'Debug.WriteLine("int���� =" & int����)

        '-------------------------------------
        '�����␳�i�����w��̏ꍇ�����ɕϊ�����j
        '-------------------------------------
        Select Case Mid(str�N����, 5, 2)
            Case "01", "03", "05", "07", "08", "10", "12"
                If Mid(str�N����, 7, 2) < "01" Then
                    Mid(str�N����, 7, 2) = "01"
                End If
                If Mid(str�N����, 7, 2) > "31" Then
                    Mid(str�N����, 7, 2) = "31"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str�N����, 1, 4)), CInt(Mid(str�N����, 5, 2)), CInt(Mid(str�N����, 7, 2)))
            Case "04", "06", "09", "11"
                If Mid(str�N����, 7, 2) < "01" Then
                    Mid(str�N����, 7, 2) = "01"
                End If
                If Mid(str�N����, 7, 2) > "30" Then
                    Mid(str�N����, 7, 2) = "30"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str�N����, 1, 4)), CInt(Mid(str�N����, 5, 2)), CInt(Mid(str�N����, 7, 2)))
            Case "02"
                If Mid(str�N����, 7, 2) < "01" Then
                    Mid(str�N����, 7, 2) = "01"
                End If
                '�Q���Q�X��,�Q���R�O��,�Q���R�P���͂Q�������w�舵���łQ�������i�����ɕϊ��j
                If Mid(str�N����, 7, 2) > "28" Then
                    '�P�����̎����œ��t�^�f�[�^�ϊ�
                    WORK_DATE = Mid(str�N����, 1, 4) & "/" & "01" & "/" & "31"
                    '�Q�����̎������Z�o
                    WORK_DATE = DateAdd(DateInterval.Month, 1, WORK_DATE)
                Else
                    '�Q�������ȊO�̓��t�ϊ�
                    WORK_DATE = DateSerial(CInt(Mid(str�N����, 1, 4)), CInt(Mid(str�N����, 5, 2)), CInt(Mid(str�N����, 7, 2)))
                End If
        End Select


        'Debug.WriteLine("WORK_DATE =" & WORK_DATE)



        '------------
        '�O�c�Ɠ��Ή�
        '------------
        If int���� = 0 Then
            YOUBI = Weekday(WORK_DATE)

            '�j������(Sun = 1:Sat = 7)
            If YOUBI = 1 Or YOUBI = 7 Or PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = False Then
                HOSEI = 1
            Else
                HOSEI = 0
            End If

            Do Until HOSEI = 0
                If str�O��c�Ɠ��敪 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str�O��c�Ɠ��敪 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If
                YOUBI = Weekday(WORK_DATE)

                '�j������(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        HOSEI = HOSEI - 1
                    End If
                End If
            Loop
        Else
            '-----------------
            '�O�c�Ɠ��ȊO�̏���
            '-----------------
            Do Until int���� = 0
                If str�O��c�Ɠ��敪 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str�O��c�Ɠ��敪 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If

                'Debug.WriteLine("WORK_DATE3 =" & WORK_DATE)

                YOUBI = Weekday(WORK_DATE)

                'Debug.WriteLine("YOUBI =" & YOUBI)

                '�j������(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        int���� = int���� - 1
                    End If
                End If
            Loop
        End If

        PFUNC_EIGYOUBI_GET = Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")
        'PFUC_EIGYOUBI_GET = WORK_DATE


    End Function
    Private Function PFUNC_COMMON_YASUMIGET(ByVal str�N���� As String) As Boolean

        PFUNC_COMMON_YASUMIGET = False

        '�x���}�X�^���݃`�F�b�N
        STR_SQL = " SELECT * FROM YASUMIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " YASUMI_DATE_Y ='" & str�N���� & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 5) = True Then
            Exit Function
        End If

        PFUNC_COMMON_YASUMIGET = True

    End Function
    Private Function PFUNC_GAKNAME_GET() As Boolean

        PFUNC_GAKNAME_GET = False

        '�w�Z���̐ݒ�
        STR_SQL = " SELECT * FROM GAKMAST1"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            lab�w�Z��.Text = ""
            txtGAKKOU_CODE.Focus()

            Exit Function
        End If

        OBJ_DATAREADER.Read()
        lab�w�Z��.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G")

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_ZENMEISAI_UPD() As Boolean
        Dim bLoopFlg As Boolean = False '�w��w�N�������ɒǉ��������`�F�b�N
        Dim iLcount As Integer '�w��w�N���[�v��

        PFUNC_ZENMEISAI_UPD = False

        '�O�񖾍׃}�X�^�X�V�i�ĐU�σt���O�j

        '�����L�[�́A�w�Z�R�[�h�A�U�֋敪�A�U�֌��ʃR�[�h�A�ĐU�σt���O�ŁA�U�֓��̍~��

        STR_SQL = " SELECT * FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND"
        'STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
        'STR_SQL += " FURIKETU_CODE_M <> 0"
        STR_SQL += " FURIKETU_CODE_M IN (" & SFuriCode & ")"
        ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
        STR_SQL += " AND"
        STR_SQL += " SAIFURI_SUMI_M ='1'"
        STR_SQL += " ORDER BY FURI_DATE_M desc"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.Read() = True Then

            '���U�̎��
            Select Case STR�I��U�֋敪
                Case "0"
                    Select Case STR�ĐU���
                        Case "1", "2"
                            '�ĐU����̏ꍇ�Ŏ擾�������ׂ����U�̖��ׂ̏ꍇ
                            '�O��̍ĐU�ŐU�����������Ă���̂Ŗ��׃}�X�^�̍X�V�͍s��Ȃ�
                            If OBJ_DATAREADER_DREAD.Item("FURI_KBN_M") = 0 Then
                                If GFUNC_SELECT_SQL3("", 1) = False Then
                                    Exit Function
                                End If
                                PFUNC_ZENMEISAI_UPD = True

                                Exit Function
                            End If
                    End Select
            End Select

            STR�O��U�֓� = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M")

            '�ǉ� 2006/10/06
            If STR_FURIKAE_DATE(1) = STR�O��U�֓� Then
                While OBJ_DATAREADER_DREAD.Read()
                    STR�O��U�֓� = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M")
                    If STR_FURIKAE_DATE(1) <> STR�O��U�֓� Then
                        Exit While
                    End If
                End While
            End If

        Else
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If
            PFUNC_ZENMEISAI_UPD = True

            Exit Function
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            '�X�V�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�Q��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If


        '�O��U�֓��Ɠ�����t�̖��׃}�X�^���X�V����
        STR_SQL = " UPDATE  G_MEIMAST SET "
        '�ĐU�σt���O
        STR_SQL += " SAIFURI_SUMI_M ='0'"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & STR�O��U�֓� & "' "
        'STR_SQL += " AND"
        'STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
        'STR_SQL += " FURIKETU_CODE_M <> 0"
        STR_SQL += " FURIKETU_CODE_M IN (" & SFuriCode & ")"
        ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
        STR_SQL += " AND"
        STR_SQL += " SAIFURI_SUMI_M ='1'"
        'If strSCH_KBN <> "0" Then '�ʏ�ȊO�̃X�P�W���[�����͊w�N�w�� 2005/12/09
        STR_SQL += " AND ("
        For iLcount = 1 To 9
            If iGakunen_Flag(iLcount) = 1 Then
                If bLoopFlg = True Then
                    STR_SQL += " OR "
                End If
                STR_SQL += " GAKUNEN_CODE_M = " & iLcount
                bLoopFlg = True
            End If
        Next iLcount
        STR_SQL += " )"
        'End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 3) = False Then
                '�X�V�����G���[
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Function
            End If
            '�X�V�����G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        Else
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                '�X�V�����G���[
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Function
            End If

        End If

        PFUNC_ZENMEISAI_UPD = True

    End Function


#End Region

End Class

