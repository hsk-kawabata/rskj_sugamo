Option Explicit On 
Option Strict Off

Imports System.data
Imports System.Data.SqlClient
Imports System.Data.SqlDbType
Imports CASTCommon
Imports System.Text
Public Class KFGMAST020

    Private btnTakouKin(20) As Button
    Private txtKinCode(20) As TextBox
    Private txtKinNName(20) As TextBox
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST020", "�w�Z���s�}�X�^�����e�i���X���")
    Private Const msgTitle As String = "�w�Z���s�}�X�^�����e�i���X���(KFGMAST020)"
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite


#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim mycontrol As Control

        '#####################################
        '���O�̏����ɕK�v�ȏ��̎擾
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "�w�Z���s�}�X�^�����e�i���X���"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�R���g���[�����O���[�v��
            For Each mycontrol In Me.Controls
                If mycontrol.Name.Length >= 7 Then
                    Select Case mycontrol.Name.Substring(0, 7)
                        Case "btnTako"
                            AddHandler mycontrol.Click, AddressOf btnTakouKin_Click
                            btnTakouKin(CInt(mycontrol.Name.Substring(11, mycontrol.Name.Length - 11))) = mycontrol
                            '2006/10/23�@�Q�ƃ{�^���������Ȃ��Ă����͂ł���悤�ɏC��
                            'mycontrol.Enabled = False
                        Case "txtKinC"
                            txtKinCode(CInt(mycontrol.Name.Substring(10, mycontrol.Name.Length - 10))) = mycontrol
                        Case "txtKinN"
                            txtKinNName(CInt(mycontrol.Name.Substring(11, mycontrol.Name.Length - 11))) = mycontrol
                    End Select
                End If
            Next

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME, True) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbToriName)")
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                btnFind.Enabled = False
                btnPrint.Enabled = False

                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Activated "

    '2006/10/20�@�o�^��A�����I�ɓǂݍ���
    Private Sub KFGMAST020_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '�Q�Ə���
            Call PSUB_Set_TakouIchiran()
        End If
    End Sub

#End Region

#Region " Button Click "
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")

            '********************************************
            '�Q�ƃ{�^��
            '********************************************
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                '���s���ꗗ�ݒ�
                Call PSUB_Set_TakouIchiran()
            Else
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '********************************************
        '�I���{�^��
        '********************************************
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnTakouKin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************
        '���s�{�^���i�ڍ׉�ʂɑJ�ځj
        '********************************************
        Dim KFGMAST021 As New KFGMAST021
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s�{�^��)�J�n", "����", "")

            STR_SQL = "SELECT * FROM GAKMAST1,GAKMAST2 "
            STR_SQL += " WHERE GAKKOU_CODE_G = GAKKOU_CODE_T"
            STR_SQL += " AND GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text) & "'"
            STR_SQL += " AND TAKO_KBN_T ='1'"

            If GFUNC_ISEXIST(STR_SQL) = False Then

                Call MessageBox.Show(G_MSG0004W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If

            '���Z�@�փR�[�h��n��
            KFGMAST021.KFGMAST020_KCoad = txtKinCode(sender.Name.Substring(11, sender.Name.Length - 11)).Text
            '�w�Z�R�[�h��n��
            KFGMAST021.KFGMAST020_GCoad = txtGAKKOU_CODE.Text

            KFGMAST021.ShowDialog(Me)

            '���s���ꗗ�č쐬
            Call PSUB_Set_TakouIchiran()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s�{�^��)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)
            '********************************************
            '����{�^��
            '********************************************
            '�w�Z�R�[�h�`�F�b�N
            If Trim(txtGAKKOU_CODE.Text) = "" Then

                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '���݃`�F�b�N
            SQL.Append(" SELECT DISTINCT GAKKOU_CODE_V")
            SQL.Append(",TKIN_NO_V")
            SQL.Append(",KIN_NNAME_N")
            SQL.Append(" FROM G_TAKOUMAST")
            SQL.Append(" LEFT JOIN TENMAST ON ")
            SQL.Append(" TKIN_NO_V = KIN_NO_N ")
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                SQL.Append(" WHERE GAKKOU_CODE_V = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_V ")

            If OraReader.DataReader(SQL) = False Then

                Call MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0013I, "�w�Z���s�}�X�^�ꗗ"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            Dim nRet As Integer
            Dim Param As String

            Param = GCom.GetUserID & "," & txtGAKKOU_CODE.Text
            nRet = ExeRepo.ExecReport("KFGP009.EXE", Param)
            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case nRet
                Case 0
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, "�w�Z���s�}�X�^�ꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
            End Select
            OraReader.NextRead()

            MessageBox.Show(String.Format(MSG0014I, "�w�Z���s�}�X�^�ꗗ"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_Set_TakouIchiran()
        '********************************************
        '���s���ݒ�
        '********************************************
        Dim Int_Counter As Integer
        Dim Int_Start As Integer
        Dim Str_Ginko_Code As String = ""

        '��񒊏o
        STR_SQL = ""
        STR_SQL += " SELECT "
        STR_SQL += " TKIN_NO_V "
        STR_SQL += ",KIN_NNAME_N "
        STR_SQL += " FROM G_TAKOUMAST left join TENMAST on "
        STR_SQL += " TKIN_NO_V = KIN_NO_N "
        STR_SQL += " WHERE GAKKOU_CODE_V = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " ORDER BY TKIN_NO_V "

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If

            '�{�^���Đ���
            Call PSUB_Set_TakouButton_Clear()
            Exit Sub
        End If

        Int_Counter = 1
        '������
        While (OBJ_DATAREADER.Read = True)
            If Str_Ginko_Code <> Trim(OBJ_DATAREADER.Item("TKIN_NO_V")).PadLeft(4, "0"c) Then
                txtKinCode(Int_Counter).Text = Trim(OBJ_DATAREADER.Item("TKIN_NO_V")).PadLeft(4, "0"c)
                txtKinNName(Int_Counter).Text = OBJ_DATAREADER.Item("KIN_NNAME_N")

                Int_Counter += 1
            End If

            Str_Ginko_Code = Trim(OBJ_DATAREADER.Item("TKIN_NO_V")).PadLeft(4, "0"c)
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Sub
        End If

        Int_Start = Int_Counter

        For Int_Counter = Int_Start To 20
            txtKinCode(Int_Counter).Text = ""
            txtKinNName(Int_Counter).Text = ""

            btnTakouKin(Int_Counter).Enabled = False
        Next

        '�{�^���Đ���
        Call PSUB_Set_TakouButton()
    End Sub
    Private Sub PSUB_Set_TakouButton()
        '********************************************
        '���s�{�^�����͐���
        '********************************************
        Dim Int_Hantei As Integer
        Dim Int_Counter As Integer

        '���͏ꏊ����
        Int_Hantei = 0
        For Int_Counter = 1 To 20
            If txtKinCode(Int_Counter).Text = "" Then
                Int_Hantei += 1
            End If
            '�{�^���͂ЂƂ����
            If Int_Hantei >= 2 Then
                btnTakouKin(Int_Counter).Enabled = False
            Else
                btnTakouKin(Int_Counter).Enabled = True
            End If
        Next

    End Sub
    Private Sub PSUB_Set_TakouButton_Clear()
        '********************************************
        '���s�{�^�����͐���(�S�N���A)
        '********************************************
        Dim Int_Hantei As Integer
        Dim Int_Counter As Integer

        '���͏ꏊ����
        Int_Hantei = 0
        For Int_Counter = 1 To 20
            txtKinCode(Int_Counter).Text = ""
            txtKinNName(Int_Counter).Text = ""

            '�{�^���͂ЂƂ����
            If Int_Counter >= 2 Then
                btnTakouKin(Int_Counter).Enabled = False
            Else
                btnTakouKin(Int_Counter).Enabled = True
            End If
        Next

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
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME, True) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged
        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        Label4.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '�Q�ƃ{�^����FOCUS
        btnFind.Focus()
    End Sub
    '2010/10/20 �w�Z�R�[�h���ύX���ꂽ�ꍇ�\�����N���A����
    Private Sub txtTorisCode_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.TextChanged
        PSUB_Set_TakouButton_Clear()
    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '********************************************
        '�w�Z�R�[�h LostFocus
        '********************************************

        If GFUNC_GAKKOU_ISEXIST2(txtGAKKOU_CODE, Label4) = True Then
            '�Q�ƃ{�^����FOCUS
            btnFind.Focus()
        ElseIf txtGAKKOU_CODE.Text = "9999999999" Then '2006/10/20�@�ǉ��F�ꗗ���
            '���x���ɕ�����\��
            Label4.Text = "���s�}�X�^�ꗗ"
            '����{�^����FOCUS
            btnPrint.Focus()
        Else
            '���s���N���A
            For Int_Counter As Integer = 1 To 20
                '2006/10/23�@�Q�ƃ{�^���������Ȃ��Ă����͂ł���悤�ɏC��
                'btnTakouKin(Int_Counter).Enabled = False
                txtKinCode(Int_Counter).Text = ""
                txtKinNName(Int_Counter).Text = ""
            Next

            '2006/10/11�@�ǉ�:���b�Z�[�W��\����A�t�H�[�J�X�𓖂Ă�
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                Call MessageBox.Show(G_MSG0061W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Text = ""
                txtGAKKOU_CODE.Focus()
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
    Private Function GFUNC_GAKKOU_ISEXIST2(ByVal pGakkou_Code As TextBox, ByVal pGakkou_Name As Label) As Boolean
        '*****************************************
        '�w�Z�R�[�h���݃`�F�b�N
        '*****************************************
        GFUNC_GAKKOU_ISEXIST2 = False

        If pGakkou_Code.Text = "" Then

            Exit Function
        End If

        '�w�Z������
        STR_SQL = ""
        STR_SQL += "SELECT GAKKOU_CODE_G , GAKKOU_NNAME_G FROM GAKMAST1,GAKMAST2 "
        STR_SQL += " WHERE GAKKOU_CODE_G = GAKKOU_CODE_T"
        STR_SQL += " AND GAKKOU_CODE_G = '" & pGakkou_Code.Text.PadLeft(pGakkou_Code.MaxLength, "0"c) & "'"
        STR_SQL += " AND TAKO_KBN_T ='1'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            '�Y���R�[�h����
            pGakkou_Name.Text = ""
        Else
            OBJ_DATAREADER.Read()

            pGakkou_Name.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G").ToString

            GFUNC_GAKKOU_ISEXIST2 = True
        End If

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

    End Function
#End Region

End Class
