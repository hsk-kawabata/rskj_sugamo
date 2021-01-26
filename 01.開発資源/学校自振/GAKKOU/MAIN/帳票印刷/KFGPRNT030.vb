Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text

Public Class KFGPRNT030
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �����U�֌��ʒ��[���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private gstrLISTWORKS_FLG As String  '2007/10/07�ǉ�

#Region " ���ʕϐ���` "
    Private gstrPRINT_DEVICE As String
    Private gstrPRINT_NAME As String
    Private gstrPRINT_PORT As String

    Private Str_Report_Path As String
    Private STR�w�Z�R�[�h As String
    Private STR������ As String
    Private STR���[�\�[�g�� As String
    Private STR�U�֋敪 As String
    Private STR���U�� As String

    Private STR_REPORT_KBN(5) As String
    '2006/10/20
    Private blnPRINT_FLG As Boolean
    Private STR�ĐU�֓� As String
    Private STR�Ώ۔N�� As String


    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT030", "�����U�֌��ʒ��[������")
    Private Const msgTitle As String = "�����U�֌��ʒ��[������(KFGPRNT030)"
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 �^�X�N�j���� ADD �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

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
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click

        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            blnPRINT_FLG = False
            '����{�^��
            '���̓`�F�b�N
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "���ʒ��["), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            '�o�͒��[�I���`�F�b�N
            If chk�U�֌��ʈꗗ.Checked = False AndAlso chk�U�֕s�\����.Checked = False AndAlso chk���[�񍐏�.Checked = False AndAlso _
               chk�U�֓X�ʏW�v�\.Checked = False AndAlso chk�U�֖��[�̂��m�点.Checked = False AndAlso chk�����`�[.Checked = False Then
                MessageBox.Show(G_MSG0026W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If chk�U�֌��ʈꗗ.Checked = True Then
                If PrintKFGP030_1() = False Then
                    Exit Sub
                End If
            End If

            If chk�U�֕s�\����.Checked = True Then
                If PrintKFGP030_2() = False Then
                    Exit Sub
                End If
            End If


            If chk���[�񍐏�.Checked = True Then
                If PrintKFGP030_3() = False Then
                    Exit Sub
                End If
            End If

            If chk�U�֓X�ʏW�v�\.Checked = True Then
                If PrintKFGP030_4() = False Then
                    Exit Sub
                End If
            End If

            If chk�U�֖��[�̂��m�点.Checked = True Then
                If PrintKFGP030_5() = False Then
                    Exit Sub
                End If
            End If

            If chk�����`�[.Checked = True Then
                If PrintKFGP030_6() = False Then
                    Exit Sub
                End If
            End If

            '1���ł�������Ă�����A�������b�Z�[�W���o�͂���
            If blnPRINT_FLG = True Then
                MessageBox.Show(String.Format(MSG0014I, "���ʒ��["), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
                '�w�Z���̎擾
                If PFUNC_GAKNAME_GET() = False Then
                    Exit Sub
                End If
            End If
        End With
    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("�[���p�f�B���O", "���s", ex.ToString)
        End Try
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#End Region
#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_CHK_ONOFF()

        '�����U�֌��ʈꗗ�\
        Select Case STR_REPORT_KBN(0)
            Case "1"
                chk�U�֌��ʈꗗ.Checked = True
            Case Else
                chk�U�֌��ʈꗗ.Checked = False
        End Select

        '�����U�֕s�\���׈ꗗ�\
        Select Case STR_REPORT_KBN(1)
            Case "1"
                chk�U�֕s�\����.Checked = True
            Case Else
                chk�U�֕s�\����.Checked = False
        End Select

        '���[�񍐏�
        Select Case STR_REPORT_KBN(2)
            Case "1"
                chk���[�񍐏�.Checked = True
            Case Else
                chk���[�񍐏�.Checked = False
        End Select

        '�����U�֓X�ʏW�v�\
        Select Case STR_REPORT_KBN(3)
            Case "1"
                chk�U�֓X�ʏW�v�\.Checked = True
            Case Else
                chk�U�֓X�ʏW�v�\.Checked = False
        End Select

        '�����U�֖��[�̂��m�点
        Select Case STR_REPORT_KBN(4)
            Case "1"
                chk�U�֖��[�̂��m�点.Checked = True
            Case Else
                chk�U�֖��[�̂��m�点.Checked = False
        End Select

        '�v�����a�������`�[
        Select Case STR_REPORT_KBN(5)
            Case "1"
                chk�����`�[.Checked = True
            Case Else
                chk�����`�[.Checked = False
        End Select

    End Sub
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

            STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
            STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

            SQL.Append("SELECT * FROM G_SCHMAST")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                '2006/10/11�@�s�\�ς݂������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            Else
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            End If
            SQL.Append(" AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')")

            '���ޭ�ً敪2(����)�͏����Ɋ܂܂Ȃ���
            SQL.Append(" AND SCH_KBN_S <> '2'")

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            If OraReader.GetString("FUNOU_FLG_S") <> "1" Then
                MessageBox.Show(MSG0085W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            '�U�֋敪�̎擾
            STR�U�֋敪 = OraReader.GetString("FURI_KBN_S")
            STR�Ώ۔N�� = OraReader.GetString("NENGETUDO_S")
            OraReader.Close()

            '���U���̎擾
            If STR�U�֋敪 = "1" Then
                OraReader = New MyOracleReader(MainDB)
                SQL = New StringBuilder(128)
                '�ĐU������ʂœ��͂��Ă����ꍇ�ɁA
                '��ʂœ��͂��Ă����U�֓����ĐU���ɂ����U�̃X�P�W���[�����擾��
                '���̏��U����ޔ����Ă���
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S  = " & SQ(Trim(STR�w�Z�R�[�h)))
                SQL.Append(" AND SFURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND FURI_KBN_S ='0'")

                If OraReader.DataReader(SQL) Then
                    '���U���̎擾
                    STR���U�� = OraReader.GetString("FURI_DATE_S")
                Else
                    STR���U�� = ""
                End If

                OraReader.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_SCHMAST_GET = True

    End Function
#End Region
#Region " ���ʈꗗ�\����֐�"
    Private Function PrintKFGP030_1() As Boolean

        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR�w�Z�R�[�h = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '���Z�͍ĐU���𕪊������Ƃ��A�Ō�̍ĐU���̏������I���܂ň���s��
                If chk�U�֌��ʈꗗ���Z�o��.Checked = True Then
                    SQL.Append(" SELECT * FROM G_SCHMAST")
                    SQL.Append(" WHERE GAKKOU_CODE_S  = " & SQ(Trim(STR�w�Z�R�[�h)))
                    SQL.Append(" AND NENGETUDO_S = " & SQ(STR�Ώ۔N��))
                    SQL.Append(" AND FURI_DATE_S > " & SQ(STR_FURIKAE_DATE(1)))
                    SQL.Append(" AND FURI_KBN_S ='1'")

                    '�X�P�W���[�����݃`�F�b�N
                    If OraReader.DataReader(SQL) = True Then
                        MessageBox.Show(G_MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKKOU_CODE.Focus()
                        Exit Function
                    End If
                End If
                OraReader.Close()

                STR�w�Z�R�[�h = txtGAKKOU_CODE.Text
                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                Dim nRet As Integer
                Dim Param As String
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
                        Return False
                End Select
                blnPRINT_FLG = True
            Else
                ''�S�w�Z���R�[�h���Ώ�
                SQL.Append(" SELECT DISTINCT GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND (FURI_KBN_S ='0'")
                SQL.Append(" OR FURI_KBN_S ='1')")
                '2006/10/11 �s�\�t���O�������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                '�X�P�W���[�������݃`�F�b�N
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                While OraReader.EOF = False
                    STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                    '���[�\�[�g���̎擾
                    If PFUNC_GAKNAME_GET() = False Then
                        STR���[�\�[�g�� = "0"
                    End If

                    '2014/04/28 saitou ���Z�M�� MODIFY ----------------------------------------------->>>>
                    '�����U�֌��ʈꗗ�\�s��C���iALL9������Ɉُ�I������j
                    If PFUNC_SCHMAST_GET() = False Then
                        Exit While
                    End If
                    '2014/04/28 saitou ���Z�M�� MODIFY -----------------------------------------------<<<<

                    SQL = New StringBuilder(128)
                    '2006/10/27 ���Z�͍ĐU���𕪊������Ƃ��A�Ō�̍ĐU���̏������I���܂ň���s��
                    If chk�U�֌��ʈꗗ���Z�o��.Checked = True Then
                        SQL = New StringBuilder(128)
                        SQL.Append(" SELECT * FROM G_SCHMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_S  = '" & Trim(STR�w�Z�R�[�h) & "'")
                        SQL.Append(" AND NENGETUDO_S = '" & STR�Ώ۔N�� & "'")
                        SQL.Append(" AND FURI_DATE_S > '" & STR_FURIKAE_DATE(1) & "'")
                        SQL.Append(" AND FURI_KBN_S ='1'")

                        Dim OraReader2 As New MyOracleReader(MainDB)
                        '�X�P�W���[�������݃`�F�b�N
                        If OraReader2.DataReader(SQL) = True Then
                            'Call GSUB_MESSAGE_WARNING( "�ĐU���𕪊������ꍇ�A���Z�w��͍Ō�̍ĐU���ł�������ł��܂���")
                            'txtGAKKOU_CODE.Focus()
                            OraReader2.Close()
                            GoTo next_GAKKOU
                        End If
                    End If

                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    Dim nRet As Integer
                    Dim Param As String
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
                            Return False
                    End Select

                    blnPRINT_FLG = True
next_GAKKOU:
                    OraReader.NextRead()
                End While
                OraReader.Close()
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

    End Function
#End Region
#Region " �s�\���ʈꗗ�\����֐�"
    Private Function PrintKFGP030_2() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)

            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR�w�Z�R�[�h = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                Dim nRet As Integer
                Dim Param As String
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
                        Return False
                End Select
                blnPRINT_FLG = True

            Else
                ''�S�w�Z���R�[�h���Ώ�
                SQL.Append(" SELECT distinct GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND ( FURI_KBN_S ='0'")
                SQL.Append(" OR FURI_KBN_S ='1')")

                '2006/10/11 �s�\�t���O�������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S ='1' ")

                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                '�X�P�W���[�������݃`�F�b�N
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                While OraReader.EOF = False
                    STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                    '���[�\�[�g���̎擾
                    If PFUNC_GAKNAME_GET() = False Then
                        STR���[�\�[�g�� = "0"
                    End If

                    If PFUNC_SCHMAST_GET() = False Then
                        Exit While
                    End If

                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    Dim nRet As Integer
                    Dim Param As String
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
                            Return False
                    End Select

                    blnPRINT_FLG = True
                    OraReader.NextRead()
                End While
                OraReader.Close()
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�s�\���ʈꗗ�\����֐�)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
#End Region
#Region " ���[�񍐏�����֐�"
    Private Function PrintKFGP030_3() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            Dim nRet As Integer
            Dim Param As String
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
                If PFUNC_���[�񍐏�() = True Then
                    '���[�񍐏���� 
                    '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1)
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP018.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "���[�񍐏�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                End If
            Else
                ''�S�w�Z���R�[�h���Ώ�
                SQL.Append(" SELECT GAKKOU_CODE_S,SFURI_DATE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND (FURI_KBN_S ='0' OR FURI_KBN_S ='1')")
                '2006/10/11 �s�\�t���O�������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC,SFURI_DATE_S DESC")

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                STR�w�Z�R�[�h = ""
                While OraReader.EOF = False
                    If STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                        STR�ĐU�֓� = OraReader.GetString("SFURI_DATE_S")

                        '���[�񍐏���� 
                        '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�
                        Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1)
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        nRet = ExeRepo.ExecReport("KFGP018.EXE", Param)
                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                        Select Case nRet
                            Case 0
                            Case Else
                                '������s���b�Z�[�W
                                MessageBox.Show(String.Format(MSG0004E, "���[�񍐏�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                        End Select

                        blnPRINT_FLG = True
                    End If
                    OraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�񍐏�����֐�)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_���[�񍐏�() As Boolean

        PFUNC_���[�񍐏� = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '������s���f�[�^�����݂��邩�ǂ����̔���
            SQL.Append(" SELECT distinct GAKKOU_CODE_S,SFURI_DATE_S")
            SQL.Append(" FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case Trim(STR�w�Z�R�[�h)
                Case Is <> "9999999999"
                    '�w��w�Z�R�[�h
                    SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(STR�w�Z�R�[�h)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1') ")

            '2006/10/11 �s�\�t���O�������ɒǉ�
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")
            SQL.Append(" ORDER BY SFURI_DATE_S DESC ")


            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(G_MSG0009I, "���[�񍐏�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Function
            End If

            While OraReader.EOF = False
                STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                STR�ĐU�֓� = OraReader.GetString("SFURI_DATE_S")
                OraReader.NextRead()
            End While

            PFUNC_���[�񍐏� = True
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function

#End Region
#Region " �����U�֓X�ʏW�v�\����֐�"
    Private Function PrintKFGP030_4() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Dim nRet As Integer
        Dim Param As String
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
                If PFUNC_�����U�֓X�ʏW�v�\() = True Then
                    '�����U�֓X�ʏW�v�\��� 
                    '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,���o���敪
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR�U�֋敪
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP019.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "�����U�֓X�ʏW�v�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                    MessageBox.Show(String.Format(G_MSG0009I, "�����U�֓X�ʏW�v�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ''�S�w�Z���R�[�h���Ώ�
                SQL.Append(" SELECT GAKKOU_CODE_S,SFURI_DATE_S,FURI_KBN_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND ( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")

                '2006/10/11 �s�\�t���O�������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC,SFURI_DATE_S DESC")

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                STR�w�Z�R�[�h = ""
                While OraReader.EOF = False
                    If STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                        STR�ĐU�֓� = OraReader.GetString("SFURI_DATE_S")
                        STR�U�֋敪 = OraReader.GetString("FURI_KBN_S")
                        '�����U�֓X�ʏW�v�\��� 
                        '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,���o���敪
                        Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR�U�֋敪
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        nRet = ExeRepo.ExecReport("KFGP019.EXE", Param)
                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                        Select Case nRet
                            Case 0
                            Case Else
                                '������s���b�Z�[�W
                                MessageBox.Show(String.Format(MSG0004E, "�����U�֓X�ʏW�v�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                        End Select
                        blnPRINT_FLG = True
                    End If
                    OraReader.NextRead()
                End While
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U�֓X�ʏW�v�\����֐�)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
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
            Select Case Trim(txtGAKKOU_CODE.Text)
                Case Is <> "9999999999"
                    '�w��w�Z�R�[�h
                    SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(txtGAKKOU_CODE.Text)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1') ")

            '���݃`�F�b�N�͎��s���E���s���܂Ƃ߂čs�� 2006/10/04
            'SQL.Append(" AND")
            'SQL.Append( " TKIN_NO_M ='" & STR�����ɃR�[�h_INI & "'")
            '2006/10/11 �s�\�t���O�������ɒǉ�
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            STR�U�֋敪 = OraReader.GetString("FURI_KBN_M")

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_�����U�֓X�ʏW�v�\ = True

    End Function

#End Region
#Region " �����U�֖��[�̂��m�点����֐�"
    Private Function PrintKFGP030_5() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Dim Param As String
        Dim nRet As Integer
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                '�������
                If PFUNC_���[�̂��m�点����\����() = True Then
                    '�����U�֖��[�̂��m�点
                    '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�\�[�g��
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR���[�\�[�g��
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP020.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "�����U�֖��[�̂��m�点"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                    MessageBox.Show(String.Format(G_MSG0009I, "�����U�֖��[�̂��m�点"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ''�S�w�Z���R�[�h���Ώ�
                SQL.Append(" SELECT GAKKOU_CODE_S,SFURI_DATE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")

                '2006/10/11 �s�\�t���O�������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S ='1' ")

                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC,SFURI_DATE_S DESC")

                '�X�P�W���[�������݃`�F�b�N
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                STR�w�Z�R�[�h = ""
                Dim intINSATU_FLG As Integer = 0
                While OraReader.EOF = False
                    If STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                        STR�ĐU�֓� = OraReader.GetString("SFURI_DATE_S")
                        '���[�\�[�g���̎擾
                        If PFUNC_GAKNAME_GET() = False Then
                            STR���[�\�[�g�� = "0"
                        End If

                        '�������
                        If PFUNC_���[�̂��m�点����\����() = True Then
                            intINSATU_FLG = 1
                            '�����U�֖��[�̂��m�点
                            '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�\�[�g��
                            Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR���[�\�[�g��
                            Dim ExeRepo As New CAstReports.ClsExecute
                            ExeRepo.SetOwner = Me
                            nRet = ExeRepo.ExecReport("KFGP020.EXE", Param)
                            '�߂�l�ɑΉ��������b�Z�[�W��\������
                            Select Case nRet
                                Case 0
                                Case Else
                                    '������s���b�Z�[�W
                                    MessageBox.Show(String.Format(MSG0004E, "�����U�֖��[�̂��m�点"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return False
                            End Select
                            blnPRINT_FLG = True
                        Else
                            MessageBox.Show(String.Format(G_MSG0009I, "�����U�֖��[�̂��m�点"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                        If intINSATU_FLG = 0 Then
                            MessageBox.Show(String.Format(G_MSG0009I, "�����U�֖��[�̂��m�点"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End If
                    OraReader.NextRead()
                End While
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U�֖��[�̂��m�点����֐�)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_���[�̂��m�点����\����() As Boolean

        PFUNC_���[�̂��m�点����\���� = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '������s���f�[�^�����݂��邩�ǂ����̔���
            SQL.Append(" SELECT * FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case Trim(STR�w�Z�R�[�h)
                Case Is <> "9999999999"
                    '�w��w�Z�R�[�h
                    SQL.Append(" AND GAKKOU_CODE_M  = " & SQ(Trim(STR�w�Z�R�[�h)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1') ")
            ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            'SQL.Append(" AND FURIKETU_CODE_M <>0 ")
            SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
            ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
            '2006/10/11 �s�\�t���O�������ɒǉ�
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")

            If OraReader.DataReader(SQL) = False Then
                Return False
            End If

            STR�U�֋敪 = OraReader.GetString("FURI_KBN_M")


            PFUNC_���[�̂��m�点����\���� = True
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
#End Region
#Region " �v�����a�������`�[����֐�"
    Private Function PrintKFGP030_6() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Dim Param As String
        Dim nRet As Integer
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                If PFUNC_�����`�[����\����() = True Then
                    '�v���������`�[(���ʗa�������`�[)���
                    '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�\�[�g��
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR���[�\�[�g��
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP021.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "�v���������`�["), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                    MessageBox.Show(String.Format(G_MSG0009I, "�v���������`�["), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ''�S�w�Z���R�[�h���Ώ�
                SQL.Append(" SELECT DISTINCT GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")
                '2006/10/11 �s�\�t���O�������ɒǉ�
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                '�X�P�W���[�������݃`�F�b�N
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                Dim intINSATU_FLG2 As Integer = 0
                While OraReader.EOF = False
                    STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                    '���[�\�[�g���̎擾
                    If PFUNC_GAKNAME_GET() = False Then
                        STR���[�\�[�g�� = "0"
                    End If

                    If PFUNC_�����`�[����\����() = True Then
                        intINSATU_FLG2 = 1
                        '�v���������`�[(���ʗa�������`�[)���
                        '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�U�֓�,�\�[�g��
                        Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR_FURIKAE_DATE(1) & "," & STR���[�\�[�g��
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        nRet = ExeRepo.ExecReport("KFGP021.EXE", Param)
                        '�߂�l�ɑΉ��������b�Z�[�W��\������
                        Select Case nRet
                            Case 0
                            Case Else
                                '������s���b�Z�[�W
                                MessageBox.Show(String.Format(MSG0004E, "�v���������`�["), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                        End Select
                        blnPRINT_FLG = True
                    Else
                        'MessageBox.Show("���[(�v���������`�[)�͈���Ώۂ��P�������݂��܂���ł����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                    If intINSATU_FLG2 = 0 Then
                        MessageBox.Show(String.Format(G_MSG0009I, "�v���������`�["), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                    OraReader.NextRead()
                End While
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�v���������`�[����֐�)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_�����`�[����\����() As Boolean

        PFUNC_�����`�[����\���� = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '������s���f�[�^�����݂��邩�ǂ����̔���
            SQL.Append(" SELECT * FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case Trim(STR�w�Z�R�[�h)
                Case Is <> "9999999999"
                    '�w��w�Z�R�[�h
                    SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(STR�w�Z�R�[�h)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1' ) ")
            SQL.Append(" AND TKIN_NO_M =" & SQ(STR_JIKINKO_CODE))
            ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
            'SQL.Append(" AND FURIKETU_CODE_M <> 0 ")
            SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
            ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END
            '2006/10/11 �s�\�t���O�������ɒǉ�
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")

            If OraReader.DataReader(SQL) = False Then
                Return False
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_�����`�[����\���� = True

    End Function
#End Region
#Region " ���N�G��(�N�����|)"
    'Private Function PFUC_SQLQuery_�U�֌���() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_�U�֌��� = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.NENDO_M"
    '    SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.TUUBAN_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

    '    '2006/02/14
    '    SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"
    '    'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

    '    'SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
    '    'SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


    '    SSQL = SSQL & ",TENMAST.KIN_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.NENDO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TUUBAN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

    '    '2006/02/14
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.HIMOMAST"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"

    '    SSQL = SSQL & " WHERE "
    '    '2006/02/14
    '    'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

    '    'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
    '    SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR�w�Z�R�[�h & "'"
    '    '2006/10/13 �������z��0�~�̃f�[�^�͏o�͂��Ȃ��悤�ɏC��
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    If chk�U�֌��ʈꗗ���Z�o��.Checked = True And STR���U�� <> "" Then
    '        '�ĐU���̓��͂ō��Z�o�͂̏ꍇ
    '        '���͂����ĐU���̖��ׂ͑S�đΏۂ���
    '        '�擾�������U���̖��ׂ͐U�֍ς̂��̂��Ώ�
    '        SSQL = SSQL & " AND ((G_MEIMAST.FURI_DATE_M <= '" & STR_FURIKAE_DATE(1) & "' AND G_MEIMAST.SEIKYU_TAISYOU_M = '" & STR�Ώ۔N�� & "' AND G_MEIMAST.FURI_KBN_M =  '1')"
    '        SSQL = SSQL & " OR (G_MEIMAST.FURI_DATE_M = '" & STR���U�� & "'"
    '        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M = 0)"
    '        SSQL = SSQL & " OR (G_MEIMAST.FURI_DATE_M = '" & STR���U�� & "'"
    '        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0"
    '        SSQL = SSQL & " AND G_MEIMAST.SAIFURI_SUMI_M = 0))"
    '    Else
    '        '���U���܂��͍ĐU���̓��͂ō��Z�o�͂Ȃ��̏ꍇ
    '        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    =" & "'" & STR_FURIKAE_DATE(1) & "'"
    '    End If

    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "

    '    '        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M = 0 "
    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR���[�\�[�g��
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '���׃}�X�^�ɂ͖��`�l�������� 2006/10/11
    '            SSQL = SSQL & "    ,SEITOMASTVIEW.SEITO_KNAME_O   ASC" '2007/02/14 ���k���J�i���ɏo��
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"

    '    End Select

    '    PFUC_SQLQuery_�U�֌��� = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_�U�֕s�\����() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_�U�֕s�\���� = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

    '    '2006/02/14
    '    'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"
    '    SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

    '    SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


    '    SSQL = SSQL & ",TENMAST.KIN_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

    '    '2006/02/14
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    '2006/02/14
    '    'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.HIMOMAST"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"

    '    SSQL = SSQL & " WHERE "
    '    '2006/02/14
    '    'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

    '    'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & "  G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
    '    SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR�w�Z�R�[�h & "'"
    '    '2006/10/13 �������z��0�~�̃f�[�^�͏o�͂��Ȃ��悤�ɏC��
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    If chk�U�֕s�\���׍��Z�o��.Checked = True And STR���U�� <> "" Then
    '        '�ĐU���̓��͂ō��Z�o�͂̏ꍇ
    '        SSQL = SSQL & " AND (G_MEIMAST.FURI_DATE_M = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'"
    '        SSQL = SSQL & "  OR  G_MEIMAST.FURI_DATE_M = '" & STR���U�� & "'" & ") "
    '    Else
    '        '���U���܂��͍ĐU���̓��͂ō��Z�o�͂Ȃ��̏ꍇ
    '        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'"
    '    End If

    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "

    '    '2006/10/12 ���v���ɐU�֍ό����E���z���o�͂���悤�ɕύX
    '    'SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0 "

    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR���[�\�[�g��
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '���׃}�X�^�ɂ͖��`�l�������� 2006/10/11
    '            SSQL = SSQL & "    ,SEITOMASTVIEW.SEITO_KNAME_O   ASC" '2007/02/14 ���k���J�i���ɏo��
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '    End Select

    '    PFUC_SQLQuery_�U�֕s�\���� = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_�X�ʏW�v() As String

    '    Dim SSQL As String = ""

    '    PFUC_SQLQuery_�X�ʏW�v = ""

    '    SSQL = SSQL & " SELECT "
    '    SSQL = SSQL & "NVL(GAKMAST1.GAKKOU_NNAME_G,0), "
    '    SSQL = SSQL & "NVL(TENMAST.SIT_NNAME_N,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.GAKKOU_CODE_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.TKIN_NO_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.FURIKETU_CODE_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.FURI_DATE_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.SEIKYU_KIN_M,0), "
    '    SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G, "
    '    SSQL = SSQL & "NVL(GAKMAST2.TSIT_NO_T,0), "
    '    SSQL = SSQL & "NVL(TENMAST_1.SIT_NNAME_N,0), "
    '    SSQL = SSQL & "GAKMAST2.TKIN_NO_T, "
    '    SSQL = SSQL & "G_MEIMAST.TSIT_NO_M "
    '    SSQL = SSQL & "FROM   "
    '    SSQL = SSQL & "KZFMAST.G_MEIMAST G_MEIMAST, "
    '    SSQL = SSQL & "KZFMAST.TENMAST TENMAST, "
    '    SSQL = SSQL & "KZFMAST.GAKMAST1 GAKMAST1, "
    '    SSQL = SSQL & "KZFMAST.GAKMAST2 GAKMAST2, "
    '    SSQL = SSQL & "KZFMAST.TENMAST TENMAST_1 "
    '    SSQL = SSQL & "WHERE  "
    '    SSQL = SSQL & "((G_MEIMAST.TKIN_NO_M=TENMAST.KIN_NO_N) AND (G_MEIMAST.TSIT_NO_M=TENMAST.SIT_NO_N)) AND "
    '    SSQL = SSQL & "(G_MEIMAST.GAKKOU_CODE_M=GAKMAST1.GAKKOU_CODE_G) AND (GAKMAST1.GAKKOU_CODE_G=GAKMAST2.GAKKOU_CODE_T) AND "
    '    SSQL = SSQL & "((GAKMAST2.TSIT_NO_T=TENMAST_1.SIT_NO_N (+)) AND (GAKMAST2.TKIN_NO_T=TENMAST_1.KIN_NO_N (+))) AND "
    '    SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G=1 "
    '    If STR�w�Z�R�[�h.Trim <> "9999999999" Then
    '        '�w��w�Z�R�[�h
    '        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(STR�w�Z�R�[�h) & "'"
    '    End If
    '    '�U�֓�
    '    SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"

    '    '�_����Z�@��
    '    '2006/10/12�@�����Ƀf�[�^�ȊO�����̑����ɏW�v���Ĉ󎚂���悤�ɕύX
    '    'SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M = '" & STR�����ɃR�[�h_INI & "'"

    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    SSQL = SSQL & "ORDER BY "
    '    SSQL = SSQL & "G_MEIMAST.GAKKOU_CODE_M ASC , G_MEIMAST.FURI_DATE_M ASC"


    '    PFUC_SQLQuery_�X�ʏW�v = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_���[�̂��m�点() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_���[�̂��m�点 = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.TKAMOKU_M"
    '    SSQL = SSQL & ",G_MEIMAST.TKOUZA_M"
    '    SSQL = SSQL & ",G_MEIMAST.TMEIGI_KNM_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_TUKI_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",SEITOMAST.SEITO_KNAME_O "
    '    SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O "

    '    SSQL = SSQL & ",TENMAST.KIN_NNAME_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TKAMOKU_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TKOUZA_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TMEIGI_KNM_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_TUKI_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMAST.SEITO_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMAST.SEITO_NNAME_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NNAME_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMAST"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"
    '    ''SSQL = SSQL & " ,KZFMAST.G_SCHMAST"

    '    SSQL = SSQL & " WHERE "
    '    SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMAST.GAKKOU_CODE_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMAST.NENDO_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMAST.TUUBAN_O  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMAST.TUKI_NO_O  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N "

    '    Select Case Trim(STR�w�Z�R�[�h)
    '        Case Is <> "9999999999"
    '            '�w��w�Z�R�[�h
    '            SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(STR�w�Z�R�[�h) & "'"
    '    End Select

    '    '�U�֓�
    '    SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"

    '    '�_����Z�@��
    '    '���s�����o�� 2006/10/04
    '    'SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M = '" & STR�����ɃR�[�h_INI & "'"
    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "

    '    SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <>0 "
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR���[�\�[�g��
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '���׃}�X�^�ɂ͖��`�l�������� 2006/10/05
    '            SSQL = SSQL & "    ,SEITOMAST.SEITO_KNAME_O   ASC" '2007/02/14�@���k�J�i�����ŏo��
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '    End Select

    '    PFUC_SQLQuery_���[�̂��m�点 = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_�����`�[() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_�����`�[ = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.TUUBAN_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_TAISYOU_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
    '    'SSQL = SSQL & ",G_MEIMAST.FURI_KBN_M"
    '    'SSQL = SSQL & ",G_MEIMAST.G_MEIMAST.FURIKETU_CODE_M"
    '    'SSQL = SSQL & ",G_MEIMAST.TKIN_NO_M"
    '    'SSQL = SSQL & ",G_MEIMAST.TSIT_NO_M"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",GAKMAST2.KAMOKU_T"
    '    SSQL = SSQL & ",GAKMAST2.KTEKIYOU_T"

    '    SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O "

    '    SSQL = SSQL & ",TENMAST.KIN_NNAME_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "

    '    'SSQL = SSQL & ", NVL(G_SCHMAST.GAKKOU_CODE_S, 0)"
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_KBN_S, 0)"
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"

    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TUUBAN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_TAISYOU_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
    '    'SSQL = SSQL & ", NVL(G_MEIMAST.FURI_KBN_M, 0)"
    '    'SSQL = SSQL & ", NVL(G_MEIMAST.FURIKETU_CODE_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST2.KAMOKU_T, 0)"
    '    SSQL = SSQL & ", NVL(GAKMAST2.KTEKIYOU_T, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMAST.SEITO_NNAME_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NNAME_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST2"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMAST"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"

    '    SSQL = SSQL & " WHERE "
    '    SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST2.GAKKOU_CODE_T  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMAST.GAKKOU_CODE_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMAST.NENDO_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMAST.TUUBAN_O  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMAST.TUKI_NO_O  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N "

    '    Select Case Trim(STR�w�Z�R�[�h)
    '        Case Is <> "9999999999"
    '            '�w��w�Z�R�[�h
    '            SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(STR�w�Z�R�[�h) & "'"
    '    End Select
    '    '�U�֓�
    '    SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
    '    '�U�֋敪=0,1
    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1'�@) "

    '    '�_����Z�@��
    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M='" & STR_JIKINKO_CODE & "'"

    '    SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <>0 "
    '    '2006/10/13 �������z��0�~�̃f�[�^�͏o�͂��Ȃ��悤�ɏC��
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR���[�\�[�g��
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '���׃}�X�^�ɂ͖��`�l�������� 2006/10/11
    '            SSQL = SSQL & "    ,SEITOMAST.SEITO_KNAME_O   ASC" '2007/02/14 ���k�J�i�����ɏo��
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '    End Select

    '    PFUC_SQLQuery_�����`�[ = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_���[�񍐏�() As String
    '    '2006/10/11 ���[�̏o�͏����w�肷�邽�߁AJYOKEN�ł͂Ȃ�SQL���w�肷��悤�ɕύX
    '    Dim SSQL As String = ""

    '    PFUC_SQLQuery_���[�񍐏� = ""


    '    SSQL = SSQL & " SELECT "
    '    SSQL = SSQL & " GAKMAST1.GAKKOU_NNAME_G, "
    '    SSQL = SSQL & " TENMAST.KIN_NNAME_N, "
    '    SSQL = SSQL & " TENMAST.SIT_NNAME_N, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME01_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME02_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME03_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME04_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME05_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME06_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO01_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO02_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO03_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO04_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO05_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO06_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA01_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA02_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA03_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA04_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA05_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA06_H, "
    '    SSQL = SSQL & " GAKMAST2.TKIN_NO_T, "
    '    SSQL = SSQL & " GAKMAST2.TSIT_NO_T, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME07_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO07_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA07_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME08_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO08_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA08_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME09_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO09_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA09_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME10_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO10_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA10_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME11_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO11_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA11_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME12_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO12_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA12_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME13_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO13_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA13_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME14_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO14_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA14_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME15_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO15_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA15_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU01_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU02_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU03_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU04_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU05_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU06_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU07_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU08_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU09_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU10_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU11_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU12_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU13_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU14_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU15_H, "
    '    SSQL = SSQL & " G_MEIMAST.GAKUNEN_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU1_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU2_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU3_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU4_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU5_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU6_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU7_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU8_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU9_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU10_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU11_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU12_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU13_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU14_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU15_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.FURI_DATE_M, "
    '    SSQL = SSQL & " G_MEIMAST.FURIKETU_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.SEIKYU_KIN_M, "
    '    SSQL = SSQL & " GAKMAST1.GAKUNEN_NAME_G, "
    '    SSQL = SSQL & " G_MEIMAST.SEIKYU_TUKI_M "
    '    SSQL = SSQL & " FROM   "
    '    SSQL = SSQL & " KZFMAST.G_MEIMAST G_MEIMAST, "
    '    SSQL = SSQL & " KZFMAST.GAKMAST2 GAKMAST2, "
    '    SSQL = SSQL & " KZFMAST.HIMOMAST HIMOMAST, "
    '    SSQL = SSQL & " KZFMAST.GAKMAST1 GAKMAST1, "
    '    SSQL = SSQL & " KZFMAST.TENMAST TENMAST"
    '    SSQL = SSQL & " WHERE  "
    '    SSQL = SSQL & " (G_MEIMAST.GAKKOU_CODE_M=GAKMAST2.GAKKOU_CODE_T (+)) AND "
    '    SSQL = SSQL & " (((G_MEIMAST.GAKKOU_CODE_M=HIMOMAST.GAKKOU_CODE_H (+)) AND "
    '    SSQL = SSQL & " (G_MEIMAST.GAKUNEN_CODE_M=HIMOMAST.GAKUNEN_CODE_H (+))) AND "
    '    SSQL = SSQL & " (G_MEIMAST.HIMOKU_ID_M=HIMOMAST.HIMOKU_ID_H (+))) AND "
    '    SSQL = SSQL & " ((G_MEIMAST.GAKKOU_CODE_M=GAKMAST1.GAKKOU_CODE_G (+)) AND "
    '    SSQL = SSQL & " (G_MEIMAST.GAKUNEN_CODE_M=GAKMAST1.GAKUNEN_CODE_G (+))) AND "
    '    SSQL = SSQL & " ((GAKMAST2.TKIN_NO_T=TENMAST.KIN_NO_N (+)) AND "
    '    SSQL = SSQL & " (GAKMAST2.TSIT_NO_T=TENMAST.SIT_NO_N (+))) AND "
    '    Select Case Trim(STR�w�Z�R�[�h)
    '        Case Is <> "9999999999"
    '            SSQL = SSQL & " GAKMAST2.GAKKOU_CODE_T= '" & Trim(STR�w�Z�R�[�h) & "'AND "
    '        Case Else
    '    End Select

    '    SSQL = SSQL & " G_MEIMAST.FURI_DATE_M= '" & STR_FURIKAE_DATE(1) & "' AND "
    '    SSQL = SSQL & " (G_MEIMAST.FURI_KBN_M= '0' OR G_MEIMAST.FURI_KBN_M= '1') AND "

    '    SSQL = SSQL & " HIMOMAST.TUKI_NO_H= SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  "

    '    SSQL = SSQL & " ORDER BY "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.GAKUNEN_CODE_M"

    '    PFUC_SQLQuery_���[�񍐏� = SSQL

    'End Function

#End Region
End Class
