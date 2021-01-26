Option Explicit On
Option Strict Off
Imports CASTCommon
Imports System.Text
Imports Microsoft.VisualBasic
Imports System.Windows.Forms

Public Class KFGPRNT160
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �w�Z�}�X�^���ڊm�F�[���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Private STR�w�Z�� As String
    Dim strKIGYO_CODE As String
    Dim strFURI_CODE As String
    Dim strGAKKOU_CODE As String
    Dim strKIN_NO As String
    Dim strSIT_NO As String
    Dim strKAMOKU As String
    Dim strKOUZA As String
    Dim strKNAME As String
    Dim strReKNAME As String
    Dim intKEKKA As Integer
    Dim STR���[�\�[�g�� As String
    Dim Str_Kubn As String
    Dim STR�w�Z�R�[�h As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT160", "�w�Z�}�X�^���ڊm�F�[������")
    Private Const msgTitle As String = "�w�Z�}�X�^���ڊm�F�[������(KFGPRNT160)"
    Private Const PrintName As String = "�w�Z�}�X�^���ڊm�F�["
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
    Private Sub KFGPRNT160_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '-------------------------------------
            ' ���O���ݒ�
            '-------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "�J�n", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Call GSUB_CONNECT()

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show("�w�Z���R���{�{�b�N�X�ݒ�ŃG���[���������܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '------------------------------------------
            '����̐ݒ�
            '------------------------------------------
            txtDateY.Text = Now.ToString("yyyy")
            txtDateM.Text = Now.ToString("MM")
            txtDateD.Text = Now.ToString("dd")

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
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            '�w�Z�R�[�h�K�{�`�F�b�N
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            '--------------------------------
            ' ����K�{�`�F�b�N
            '--------------------------------
            If CheckIsInputRequiredControl() = False Then
                Return
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            STR�w�Z�R�[�h = txtGAKKOU_CODE.Text
            Dim prmKijyunDate As String = txtDateY.Text & txtDateM.Text & txtDateD.Text '���

            '���O�C��ID,�w�Z�R�[�h,���
            Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & prmKijyunDate
            nRet = ExeRepo.ExecReport("KFGP038.EXE", Param)
            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case nRet
                Case 0
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
            End Select

            txtGAKKOU_CODE.Focus()
            MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
            MainDB.Rollback()
        Finally
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
    Private Function PFUNC_GAKNAME_GET(Optional ByVal NameChg As Boolean = True) As Boolean

        '�w�Z���̐ݒ�
        PFUNC_GAKNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            SQL.Append(" SELECT ")
            SQL.Append(" GAKMAST1.*")
            SQL.Append(",MEISAI_OUT_T")
            SQL.Append(" FROM GAKMAST1,GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
            SQL.Append(" AND GAKKOU_CODE_G = " & SQ(STR�w�Z�R�[�h))

            If OraReader.DataReader(SQL) = False Then

                MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                lab�w�Z��.Text = ""
                STR���[�\�[�g�� = 0

                Exit Function
            End If

            If NameChg Then lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")
            STR���[�\�[�g�� = OraReader.GetInt("MEISAI_OUT_T")

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
    ' ���̓`�F�b�N
    Private Function CheckIsInputRequiredControl() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "�J�n", "")

            '���(�N)
            Dim FURIKAE_DATE As String
            If fn_CHECK_NUM_MSG(txtDateY.Text, "���(�N)", msgTitle) = False Then
                txtDateY.Focus()
                Return False
            End If

            '���(��)
            If fn_CHECK_NUM_MSG(txtDateM.Text, "���(��)", msgTitle) = False Then
                txtDateM.Focus()
                Return False
            Else
                If txtDateM.Text < 1 Or txtDateM.Text > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateM.Focus()
                    Return False
                End If
            End If

            '���(��)
            If fn_CHECK_NUM_MSG(txtDateD.Text, "���(��)", msgTitle) = False Then
                txtDateD.Focus()
                Return False
            Else
                If txtDateD.Text < 1 Or txtDateD.Text > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateD.Focus()
                    Return False
                End If
            End If

            '����Ó����`�F�b�N
            If txtDateY.Text.Length <> 0 Or txtDateM.Text.Length <> 0 Or txtDateD.Text.Length <> 0 Then
                FURIKAE_DATE = txtDateY.Text & "/" & txtDateM.Text & "/" & txtDateD.Text
                If Not IsDate(FURIKAE_DATE) Then
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateY.Focus()
                    txtDateY.SelectionStart = 0
                    txtDateY.SelectionLength = txtDateY.Text.Length
                    Return False
                End If
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "����", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "�I��", "")
        End Try

    End Function

    Private Function fn_CHECK_NUM_MSG(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_NUM_MSG
        'Parameter      :objOBJ�F�`�F�b�N�ΏۃI�u�W�F�N�g�^strJNAME�F�I�u�W�F�N�g����
        '               :gstrTITLE�F�^�C�g��
        'Description    :���l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2004/05/28
        'Update         :
        '============================================================================

        Try

            If Trim(objOBJ).Length = 0 Then
                MessageBox.Show(String.Format(MSG0285W, strJNAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            For i As Integer = 0 To objOBJ.Length - 1 Step 1       '�����_/��������
                If Char.IsDigit(objOBJ.Chars(i)) = False Then
                    MessageBox.Show(String.Format(MSG0344W, strJNAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            Next i

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

#End Region

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

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
        '�w�Z���̎擾
        lab�w�Z��.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 �w�Z�R�[�h�[������
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '�w�Z���̎擾
            STR�w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

    '�e�L�X�g�{�b�N�X�[������
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtDateY.Validating, _
                txtDateM.Validating, _
                txtDateD.Validating


        Call GCom.NzNumberString(CType(sender, TextBox), True)

    End Sub

End Class
