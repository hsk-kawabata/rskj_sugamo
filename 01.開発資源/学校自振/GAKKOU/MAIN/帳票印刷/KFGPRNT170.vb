Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text

Public Class KFGPRNT170
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �w�Z���k������
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Private STR�w�Z�� As String
    Private STR�w�N�� As String
    '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- START
    '���[���ԈႢ
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT170", "���k�o�^���ꗗ�\������")
    Private Const msgTitle As String = "���k�o�^���ꗗ�\������(KFGPRNT170)"
    'Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT170", "�w�Z���k���������")
    'Private Const msgTitle As String = "�w�Z���k���������(KFGPRNT170)"
    '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- END
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
    Private Sub GFJPRNT0700G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Dim Flg As String = ""
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            '���͒l�`�F�b�N
            If PFUNC_NYUURYOKU_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text


            SQL = New StringBuilder
            SQL.Append("SELECT DISTINCT GAKKOU_CODE_O")
            SQL.Append(" FROM SEITOMAST")
            SQL.Append(" WHERE KAIYAKU_FLG_O ='0'")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
            ElseIf Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) <> "" Then
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            ElseIf Trim(txtGAKKOU_CODE.Text) <> "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            Else
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_O")

            OraReader = New MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '����O�m�F���b�Z�[�W
            '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- START
            '���[���ԈႢ
            If MessageBox.Show(String.Format(MSG0013I, "���k�o�^���ꗗ�\"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            'If MessageBox.Show(String.Format(MSG0013I, "�w�Z���k����"), _
            '                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            '    Return
            'End If
            '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- END

            While OraReader.EOF = False
                '���O�C��ID,�w�Z�R�[�h,�w�N,�N���X
                Param = GCom.GetUserID & "," & OraReader.GetString("GAKKOU_CODE_O") & "," & txtGAKUNEN.Text.Trim & "," & txtCLASS.Text
                nRet = ExeRepo.ExecReport("KFGP039.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- START
                        '���[���ԈႢ
                        MessageBox.Show(String.Format(MSG0004E, "���k�o�^���ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        'MessageBox.Show(String.Format(MSG0004E, "�w�Z���k����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- END
                        Return
                End Select
                OraReader.NextRead()
            End While

            '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- START
            '���[���ԈႢ
            MessageBox.Show(String.Format(MSG0014I, "���k�o�^���ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            'MessageBox.Show(String.Format(MSG0014I, "�w�Z���k����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '2017/06/23 saitou �W���ŏC�� UPD ---------------------------------------- END

        Catch ex As Exception
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

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        PFUNC_GAKNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            '�w�Z���̐ݒ�
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab�w�Z��.Text = "�S�w�Z�Ώ�"
            Else
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE")
                SQL.Append(" GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab�w�Z��.Text = ""
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")
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
    Private Function PFUNC_GAKUNENNAME_GET() As Boolean

        PFUNC_GAKUNENNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            '�w�Z���̐ݒ�
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lblGAKUNEN_NAME.Text = ""
            Else
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))
                SQL.Append(" AND GAKUNEN_CODE_G =" & txtGAKUNEN.Text)

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("�w�N�F " & txtGAKUNEN.Text & " �͊w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lblGAKUNEN_NAME.Text = ""
                    txtGAKUNEN.Text = ""
                    txtGAKUNEN.Focus()
                    Exit Function
                End If

                STR�w�N�� = OraReader.GetString("GAKUNEN_NAME_G")

                lblGAKUNEN_NAME.Text = STR�w�N��
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�N����)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_GAKUNENNAME_GET = True

    End Function

    Private Function PFUNC_NYUURYOKU_CHECK() As Boolean

        PFUNC_NYUURYOKU_CHECK = False

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '�w�Z�R�[�h�K�{�`�F�b�N
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL = New StringBuilder
                '�w�Z�}�X�^���݃`�F�b�N
                SQL.Append("SELECT GAKKOU_CODE_G")
                SQL.Append(" FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("���͂��ꂽ�w�Z�R�[�h�����݂��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If
            End If


            '������鐶�k�f�[�^�����邩����
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM SEITOMAST")
            SQL.Append(" WHERE KAIYAKU_FLG_O ='0'")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) = "" Then

            ElseIf Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) <> "" Then
                SQL.Append(" AND GAKUNEN_CODE_O = " & SQ(Trim(txtGAKUNEN.Text)))
            ElseIf Trim(txtGAKKOU_CODE.Text) <> "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            Else
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                If Trim(txtGAKUNEN.Text) <> "" Then
                    MessageBox.Show("�w��w�N�ɐ��k���o�^����Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    MessageBox.Show("�w��w�Z�ɐ��k���o�^����Ă��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                txtGAKUNEN.Focus()
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_NYUURYOKU_CHECK = True

    End Function
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
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
       Handles txtCLASS.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("�[���p�f�B���O", "���s", ex.ToString)
        End Try
    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            CType(sender, TextBox).Text = CType(sender, TextBox).Text.Trim.PadLeft(CType(sender, TextBox).MaxLength, "0")
            '�w�Z���̎擾
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If
    End Sub
    Private Sub txtGAKUNEN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN.Validating
        '�w�N
        If Trim(txtGAKUNEN.Text) <> "" Then
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                If PFUNC_GAKUNENNAME_GET() = False Then
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

End Class
