Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT130
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �����U�֌��ʒ��[���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Dim Str_Report_Path As String
    Dim STR�w�Z�R�[�h As String
    Dim STR������ As String
    Dim STR���[�\�[�g�� As String
    Dim STR�U�֋敪 As String
    Dim STR���U�� As String
    Dim STR�Ώ۔N�� As String
    Private STR_REPORT_KBN(5) As String
    '2006/10/20
    Dim blnPRINT_FLG As Boolean
    Dim strFURI_DATE_Y As String
    Dim strFURI_DATE_M As String
    Dim strFURI_DATE_D As String

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT130", "�����������ʒ��[������")
    Private Const msgTitle As String = "�����������ʒ��[������(KFGPRNT130)"
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
    Private Sub GFJPRNT0300G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            blnPRINT_FLG = False

            '���̓`�F�b�N
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '�o�͒��[�I���`�F�b�N
            If chk�U�֌��ʈꗗ.Checked = False And chk�U�֕s�\����.Checked = False And chk�U�֓X�ʏW�v�\.Checked = False Then
                MessageBox.Show("���[���I������Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "���ʒ��["), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            If chk�U�֌��ʈꗗ.Checked = True Then
                STR�w�Z�R�[�h = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If
                STR�w�Z�R�[�h = txtGAKKOU_CODE.Text

                '�����U�֌��ʈꗗ�\��� 
                '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,���U��,�Ώ۔N��,���Z����,����敪("0"�Œ�),�U�֋敪,���[�\�[�g��
                Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR���U�� & "," & _
                        STR�Ώ۔N�� & "," & IIf(chk�U�֌��ʈꗗ���Z�o��.Checked, "1", "0") & ",0," & STR�U�֋敪 & "," & STR���[�\�[�g��

                nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "�����U�֌��ʈꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                blnPRINT_FLG = True
            End If

            If chk�U�֕s�\����.Checked = True Then
                STR�w�Z�R�[�h = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If
                '�s�\���ʈꗗ�\��� 
                '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,���U��,�Ώ۔N��,���Z����,����敪("1"�Œ�),�U�֋敪,���[�\�[�g��
                Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR���U�� & "," & _
                        STR�Ώ۔N�� & "," & IIf(chk�U�֌��ʈꗗ���Z�o��.Checked, "1", "0") & ",1," & STR�U�֋敪 & "," & STR���[�\�[�g��

                nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "�����U�֕s�\���׈ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                blnPRINT_FLG = True
            End If

            If chk�U�֓X�ʏW�v�\.Checked = True Then
                If PFUNC_�����U�֓X�ʏW�v�\() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If
                '�����U�֓X�ʏW�v�\��� 
                '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,���o���敪
                Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR�U�֋敪
                nRet = ExeRepo.ExecReport("KFGP019.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "�����U�֓X�ʏW�v�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                blnPRINT_FLG = True
            End If

            '1���ł�������Ă�����A�������b�Z�[�W���o�͂���
            If blnPRINT_FLG = True Then
                MessageBox.Show("������������܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
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
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_CHK_ONOFF()

        '�����U�֌��ʈꗗ�\
        Select Case (STR_REPORT_KBN(0))
            Case "1"
                chk�U�֌��ʈꗗ.Checked = True
            Case Else
                chk�U�֌��ʈꗗ.Checked = False
        End Select

        '�����U�֕s�\���׈ꗗ�\
        Select Case (STR_REPORT_KBN(1))
            Case "1"
                chk�U�֕s�\����.Checked = True
            Case Else
                chk�U�֕s�\����.Checked = False
        End Select

        '�����U�֓X�ʏW�v�\
        Select Case (STR_REPORT_KBN(3))
            Case "1"
                chk�U�֓X�ʏW�v�\.Checked = True
            Case Else
                chk�U�֓X�ʏW�v�\.Checked = False
        End Select


    End Sub
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
        lab�w�Z��.Text = cmbGakkouName.Text.Trim
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        STR�w�Z�R�[�h = txtGAKKOU_CODE.Text.Trim

        '�w�Z���̎擾
        If PFUNC_GAKNAME_GET() = False Then
            Exit Sub
        End If

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z���̎擾
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 �w�Z�R�[�h�[������
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '�w�Z���̎擾
            STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            Else
                '�U�֓��R���{�{�b�N�X�̐ݒ�
                If PFUNC_Set_cmbFURIKAEBI() = False Then
                    Exit Sub
                End If
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
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        '�w�Z���̐ݒ�
        PFUNC_GAKNAME_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As New MyOracle
        Try
            If Trim(STR�w�Z�R�[�h) = "9999999999" Then
                lab�w�Z��.Text = ""
            Else
                OraReader = New MyOracleReader(OraDB)
                SQL.Append(" SELECT ")
                SQL.Append(" GAKMAST1.*")
                SQL.Append(",MEISAI_FUNOU_T")
                SQL.Append(",MEISAI_KEKKA_T")
                SQL.Append(",MEISAI_HOUKOKU_T")
                SQL.Append(",MEISAI_TENBETU_T")
                SQL.Append(",MEISAI_MINOU_T")
                SQL.Append(",MEISAI_YOUKYU_T")
                SQL.Append(",MEISAI_OUT_T")
                SQL.Append(" FROM GAKMAST1,GAKMAST2")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
                SQL.Append(" AND GAKKOU_CODE_G =" & SQ(STR�w�Z�R�[�h))

                If OraReader.DataReader(SQL) = False Then
                    lab�w�Z��.Text = ""
                    STR���[�\�[�g�� = 0
                    Exit Function
                End If

                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")
                    STR_REPORT_KBN(0) = OraReader.GetString("MEISAI_FUNOU_T")
                    STR_REPORT_KBN(1) = OraReader.GetString("MEISAI_KEKKA_T")
                    STR_REPORT_KBN(2) = OraReader.GetString("MEISAI_HOUKOKU_T")
                    STR_REPORT_KBN(3) = OraReader.GetString("MEISAI_TENBETU_T")
                    STR_REPORT_KBN(4) = OraReader.GetString("MEISAI_MINOU_T")
                    STR_REPORT_KBN(5) = OraReader.GetString("MEISAI_YOUKYU_T")
                End If

                STR���[�\�[�g�� = OraReader.GetString("MEISAI_OUT_T")

                OraReader.Close()

                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    Call PSUB_CHK_ONOFF()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z���擾)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)

            '�w�Z�R�[�h�K�{�`�F�b�N
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

            If Trim(cmbFURIKAEBI.Text) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�U�֔N����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbFURIKAEBI.Focus()
                Exit Function
            End If

            strFURI_DATE_Y = Mid(cmbFURIKAEBI.Text, 1, 4)
            strFURI_DATE_M = Mid(cmbFURIKAEBI.Text, 6, 2)
            strFURI_DATE_D = Mid(cmbFURIKAEBI.Text, 9, 2)
            STR_FURIKAE_DATE(0) = Mid(cmbFURIKAEBI.Text, 1, 4) & "/" & Mid(cmbFURIKAEBI.Text, 6, 2) & "/" & Mid(cmbFURIKAEBI.Text, 9, 2)
            STR_FURIKAE_DATE(1) = Trim(Mid(cmbFURIKAEBI.Text, 1, 4)) & Format(CInt(Mid(cmbFURIKAEBI.Text, 6, 2)), "00") & Format(CInt(Mid(cmbFURIKAEBI.Text, 9, 2)), "00")

            '���t�������`�F�b�N
            If Information.IsDate(STR_FURIKAE_DATE(0)) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbFURIKAEBI.Focus()
                Return False
            End If

            SQL.Append("SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
                Case "����"
                    SQL.Append(" AND FURI_KBN_S ='2'")
                Case "�o��"
                    SQL.Append(" AND FURI_KBN_S ='3'")
            End Select

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            If OraReader.GetString("FUNOU_FLG_S") = "0" Then
                MessageBox.Show("���̃X�P�W���[���͕s�\���ʍX�V�������������ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S  = " & SQ(STR�w�Z�R�[�h))
            SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            '2017/03/14 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ START
            Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
                Case "����"
                    SQL.Append(" AND FURI_KBN_S ='2'")
                Case "�o��"
                    SQL.Append(" AND FURI_KBN_S ='3'")
            End Select
            '2017/03/14 �^�X�N�j���� ADD �W���ŏC���i���݃o�O�C���j------------------------------------ END

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '�U�֋敪�̎擾
            STR�U�֋敪 = OraReader.GetString("FURI_KBN_S")
            STR�Ώ۔N�� = OraReader.GetString("NENGETUDO_S")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_�����U�֓X�ʏW�v�\() As Boolean

        PFUNC_�����U�֓X�ʏW�v�\ = False

        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '������s���f�[�^�����݂��邩�ǂ����̔���
            SQL.Append(" SELECT * FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(txtGAKKOU_CODE.Text)))
            Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
                Case "����"
                    SQL.Append(" AND G_MEIMAST.FURI_KBN_M ='2'")
                Case "�o��"
                    SQL.Append(" AND G_MEIMAST.FURI_KBN_M ='3'")
            End Select
            '2006/10/11 �s�\�t���O�������ɒǉ�
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")
            '2006/12/25 �X�P�W���[���̐U�֋敪�ǉ�
            SQL.Append(" AND G_MEIMAST.FURI_KBN_M =G_SCHMAST.FURI_KBN_S ")

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_�����U�֓X�ʏW�v�\ = True
    End Function
    Private Function PFUNC_Set_cmbFURIKAEBI() As Boolean

        '�U�֓��R���{�̐ݒ�
        Dim str�U�֓� As String

        PFUNC_Set_cmbFURIKAEBI = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As New MyOracle
        Try
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                '�U�֓��R���{�{�b�N�X�̃N���A
                cmbFURIKAEBI.Items.Clear()
                OraReader = New MyOracleReader(OraDB)
                '�X�P�W���[���}�X�^�̌����A�L�[�͊w�Z�R�[�h�A�X�P�W���[���敪�A���׍쐬�t���O
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S =" & SQ(txtGAKKOU_CODE.Text))
                SQL.Append(" AND SCH_KBN_S ='2'")
                SQL.Append(" AND ENTRI_FLG_S ='1'")
                SQL.Append(" ORDER BY FURI_DATE_S")

                If OraReader.DataReader(SQL) = False Then
                    Exit Function
                End If


                While OraReader.EOF = False
                    '�U�֓��̕ҏW
                    str�U�֓� = Mid(OraReader.GetString("FURI_DATE_S"), 1, 4) & "/" & Mid(OraReader.GetString("FURI_DATE_S"), 5, 2) & _
                                "/" & Mid(OraReader.GetString("FURI_DATE_S"), 7, 2)

                    '�����A�o���̕ҏW
                    Select Case OraReader.GetString("FURI_KBN_S")
                        Case "2"
                            str�U�֓� += " ����"
                        Case "3"
                            str�U�֓� += " �o��"
                    End Select
                    '�U�֓��R���{�{�b�N�X�֒ǉ�
                    cmbFURIKAEBI.Items.Add(str�U�֓�)
                    OraReader.NextRead()
                End While

                '�R���{�擪�̐ݒ�
                cmbFURIKAEBI.SelectedIndex = 0
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�֓��ݒ�)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_Set_cmbFURIKAEBI = True

    End Function
#End Region

#Region " ���N�G��(�N�����|)"
    Private Function PFUC_SQLQuery_�U�֌���() As String
        Dim SSQL As String

        PFUC_SQLQuery_�U�֌��� = ""


        SSQL = "SELECT "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.NENDO_M"
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
        SSQL = SSQL & ",G_MEIMAST.TUUBAN_M"
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

        '2006/02/14
        SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"
        'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"

        SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
        SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

        'SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
        'SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
        SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
        SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


        SSQL = SSQL & ",TENMAST.KIN_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


        SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.NENDO_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.TUUBAN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

        '2006/02/14
        'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

        SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

        SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

        SSQL = SSQL & " FROM "
        SSQL = SSQL & "  KZFMAST.G_MEIMAST"
        'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
        SSQL = SSQL & " ,KZFMAST.GAKMAST1"
        SSQL = SSQL & " ,KZFMAST.HIMOMAST"
        SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
        SSQL = SSQL & " ,KZFMAST.TENMAST"

        SSQL = SSQL & " WHERE "
        '2006/02/14
        'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

        'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
        SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
        SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

        SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
        SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR�w�Z�R�[�h & "'"
        '2006/10/13 �������z��0�~�̃f�[�^�͏o�͂��Ȃ��悤�ɏC��
        SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

        If chk�U�֌��ʈꗗ���Z�o��.Checked = True And STR���U�� <> "" Then
            '�ĐU���̓��͂ō��Z�o�͂̏ꍇ
            '���͂����ĐU���̖��ׂ͑S�đΏۂ���
            '�擾�������U���̖��ׂ͐U�֍ς̂��̂��Ώ�
            SSQL = SSQL & " AND (G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
            SSQL = SSQL & " OR (G_MEIMAST.FURI_DATE_M = '" & STR���U�� & "'"
            SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M = 0))"
        Else
            '���U���܂��͍ĐU���̓��͂ō��Z�o�͂Ȃ��̏ꍇ
            SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    =" & "'" & STR_FURIKAE_DATE(1) & "'"
        End If

        '2006/10/20 ���o���敪�����������ɒǉ�
        Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
            Case "����"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='2'"
            Case "�o��"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='3'"
        End Select

        SSQL = SSQL & " ORDER BY "
        Select Case (STR���[�\�[�g��)
            Case "0"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
                SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case "1"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case Else
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '���׃}�X�^�ɂ͖��`�l�������� 2006/10/11
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
        End Select

        PFUC_SQLQuery_�U�֌��� = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

    End Function
    Private Function PFUC_SQLQuery_�U�֕s�\����() As String
        Dim SSQL As String

        PFUC_SQLQuery_�U�֕s�\���� = ""


        SSQL = "SELECT "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

        '2006/02/14
        'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"
        SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"

        SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
        SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

        SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
        SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
        SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
        SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


        SSQL = SSQL & ",TENMAST.KIN_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


        SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

        '2006/02/14
        'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

        SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

        SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

        SSQL = SSQL & " FROM "
        SSQL = SSQL & "  KZFMAST.G_MEIMAST"
        '2006/02/14
        'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
        SSQL = SSQL & " ,KZFMAST.GAKMAST1"
        SSQL = SSQL & " ,KZFMAST.HIMOMAST"
        SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
        SSQL = SSQL & " ,KZFMAST.TENMAST"

        SSQL = SSQL & " WHERE "
        '2006/02/14
        'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

        'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & "  G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
        SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
        SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

        SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
        SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR�w�Z�R�[�h & "'"
        '2006/10/13 �������z��0�~�̃f�[�^�͏o�͂��Ȃ��悤�ɏC��
        SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

        If chk�U�֕s�\���׍��Z�o��.Checked = True And STR���U�� <> "" Then
            '�ĐU���̓��͂ō��Z�o�͂̏ꍇ
            SSQL = SSQL & " AND (G_MEIMAST.FURI_DATE_M = '" & strFURI_DATE_Y & strFURI_DATE_M & strFURI_DATE_D & "'"
            SSQL = SSQL & "  OR  G_MEIMAST.FURI_DATE_M = '" & STR���U�� & "'" & ") "
        Else
            '���U���܂��͍ĐU���̓��͂ō��Z�o�͂Ȃ��̏ꍇ
            SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & strFURI_DATE_Y & strFURI_DATE_M & strFURI_DATE_D & "'"
        End If

        '2006/10/20 ���o���敪�����������ɒǉ�
        Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
            Case "����"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='2'"
            Case "�o��"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='3'"
        End Select

        '2006/10/12 ���v���ɐU�֍ό����E���z���o�͂���悤�ɕύX
        'SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0 "

        SSQL = SSQL & " ORDER BY "
        Select Case (STR���[�\�[�g��)
            Case "0"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
                SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case "1"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case Else
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '���׃}�X�^�ɂ͖��`�l�������� 2006/10/11
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
        End Select

        PFUC_SQLQuery_�U�֕s�\���� = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

    End Function
    Private Function PFUC_SQLQuery_�X�ʏW�v() As String
        Dim SSQL As String = ""

        PFUC_SQLQuery_�X�ʏW�v = ""

        SSQL = SSQL & " SELECT "
        SSQL = SSQL & "NVL(GAKMAST1.GAKKOU_NNAME_G,0), "
        SSQL = SSQL & "NVL(TENMAST.SIT_NNAME_N,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.GAKKOU_CODE_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.TKIN_NO_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.FURIKETU_CODE_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.FURI_DATE_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.SEIKYU_KIN_M,0), "
        SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G, "
        SSQL = SSQL & "NVL(GAKMAST2.TSIT_NO_T,0), "
        SSQL = SSQL & "NVL(TENMAST_1.SIT_NNAME_N,0), "
        SSQL = SSQL & "GAKMAST2.TKIN_NO_T, "
        SSQL = SSQL & "G_MEIMAST.TSIT_NO_M "
        SSQL = SSQL & "FROM   "
        SSQL = SSQL & "KZFMAST.G_MEIMAST G_MEIMAST, "
        SSQL = SSQL & "KZFMAST.TENMAST TENMAST, "
        SSQL = SSQL & "KZFMAST.GAKMAST1 GAKMAST1, "
        SSQL = SSQL & "KZFMAST.GAKMAST2 GAKMAST2, "
        SSQL = SSQL & "KZFMAST.TENMAST TENMAST_1, "
        SSQL = SSQL & "KZFMAST.G_SCHMAST G_SCHMAST "
        SSQL = SSQL & "WHERE  "
        SSQL = SSQL & "((G_MEIMAST.TKIN_NO_M=TENMAST.KIN_NO_N) AND (G_MEIMAST.TSIT_NO_M=TENMAST.SIT_NO_N)) AND "
        SSQL = SSQL & "((G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S) AND (G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S)) AND "
        SSQL = SSQL & "(G_MEIMAST.GAKKOU_CODE_M=GAKMAST1.GAKKOU_CODE_G) AND (GAKMAST1.GAKKOU_CODE_G=GAKMAST2.GAKKOU_CODE_T) AND "
        SSQL = SSQL & "((GAKMAST2.TSIT_NO_T=TENMAST_1.SIT_NO_N (+)) AND (GAKMAST2.TKIN_NO_T=TENMAST_1.KIN_NO_N (+))) AND "
        SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G=1 "
        Select Case (Trim(txtGAKKOU_CODE.Text))
            Case Is <> "9999999999"
                '�w��w�Z�R�[�h
                SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(txtGAKKOU_CODE.Text) & "'"
        End Select
        '�U�֓�
        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"

        '�_����Z�@��
        '2006/10/12�@�����Ƀf�[�^�ȊO�����̑����ɏW�v���Ĉ󎚂���悤�ɕύX
        'SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M = '" & STR�����ɃR�[�h_INI & "'"

        '2006/10/20 ���o���敪�����������ɒǉ�
        Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
            Case "����"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='2'"
            Case "�o��"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='3'"
        End Select

        '2006/10/11 �����ɕs�\�t���O�ǉ�
        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = G_SCHMAST.FURI_DATE_S  "
        SSQL = SSQL & " AND G_SCHMAST.FUNOU_FLG_S = '1'  "
        '2006/10/13 �������z��0�~�̃f�[�^�͏o�͂��Ȃ��悤�ɏC��
        SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

        SSQL = SSQL & "ORDER BY "
        SSQL = SSQL & "G_MEIMAST.GAKKOU_CODE_M ASC , G_MEIMAST.FURI_DATE_M ASC"


        PFUC_SQLQuery_�X�ʏW�v = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

    End Function
#End Region

End Class
