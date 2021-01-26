'========================================================================
'KFSPRNT070
'�����}�X�^���ڊm�F�[������
'
'�쐬���F2017/03/06
'
'���l�F
'========================================================================
Imports System.Windows.Forms
Imports CASTCommon
Imports System.Text

Public Class KFSPRNT070

#Region " ���ʐ錾"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

#End Region

#Region " �N���X�ϐ��錾"

    Private MainLOG As New CASTCommon.BatchLOG("KFSPRNT070", "�����}�X�^���ڊm�F�[������")
    Private Const msgTitle As String = "�����}�X�^���ڊm�F�[������(KFSPRNT070)"

    Private strJyoken As String = ""
    Private strPrintName As String = "�����}�X�^���ڊm�F�["

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U����
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

#End Region

#Region "���[�h"

    '=======================================================================
    'KFSPRNT070_Load
    '
    '���T�v��
    '�@�����}�X�^���ڊm�F�[�����ʃ��[�h�C�x���g�n���h��
    '
    '���p�����[�^��
    '�@sender�F
    '�@e�F
    '
    '���߂�l��
    '�@�Ȃ�
    '=======================================================================
    Private Sub KFSPRNT070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '------------------------------------------
            ' ���O���ݒ�
            '------------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "�J�n", "")

            '------------------------------------------
            '�ϑ��Җ����X�g�{�b�N�X�̐ݒ�
            '------------------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "3", strJyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '------------------------------------------
            '����̐ݒ�
            '------------------------------------------
            txtDateY.Text = Now.ToString("yyyy")
            txtDateM.Text = Now.ToString("MM")
            txtDateD.Text = Now.ToString("dd")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "�I��", "")
        End Try
    End Sub

#End Region

#Region " �{�^��"

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�J�n", "")

            '--------------------------------
            ' �K�{�`�F�b�N
            '--------------------------------
            If CheckIsInputRequiredControl() = False Then
                Return
            End If

            '--------------------------------
            ' ����O�m�F���b�Z�[�W
            '--------------------------------
            If MessageBox.Show(String.Format(MSG0013I, strPrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�L�����Z��", "")
                Return
            End If

            '--------------------------------
            ' �p�����[�^�쐬
            '--------------------------------
            Dim prmUserId As String = GCom.GetUserID                                    '���[�U�h�c
            Dim prmToriSCd As String = Me.txtTorisCode.Text.Trim                        '������R�[�h
            Dim prmToriFCd As String = Me.txtTorifCode.Text.Trim                        '����敛�R�[�h
            Dim prmKijyunDate As String = txtDateY.Text & txtDateM.Text & txtDateD.Text '���

            '------------------------------------------
            ' ���|�G�[�W�F���g���
            '------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim strParam As String = String.Format("{0},{1},{2},{3}", prmUserId, prmToriSCd, prmToriFCd, prmKijyunDate)

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�J�n", "�p�����[�^:" & strParam)
            Dim iRetValue As Integer = ExeRepo.ExecReport("KFSP036", strParam)
            Select Case iRetValue
                Case 0
                    '����
                    MessageBox.Show(String.Format(MSG0014I, strPrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "����", "")
                Case -1
                    '�o�̓f�[�^�Ȃ�
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "����", "����ΏۂȂ�")
                Case Else
                    '���s�G���[
                    MessageBox.Show(String.Format(MSG0004E, strPrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "���s", "�߂�l = " & iRetValue.ToString)
            End Select


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�I��", "")
        End Try

    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "�J�n", "")
            Me.Close()
            Me.Dispose()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "�I��", "")
        End Try
    End Sub

#End Region

#Region " �֐�"

    ' ���̓`�F�b�N
    Private Function CheckIsInputRequiredControl() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "�J�n", "")

            '������R�[�h
            Dim strToriSCd As String = Me.txtTorisCode.Text.Trim()
            If strToriSCd = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "������R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '����敛�R�[�h
            Dim strToriFCd As String = Me.txtTorifCode.Text.Trim()
            If strToriFCd = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "����敛�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

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

#Region " �C�x���g"

    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtTorisCode.Validating, _
                txtTorifCode.Validating, _
                txtDateY.Validating, _
                txtDateM.Validating, _
                txtDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)

    End Sub

    '�������ݒ�(����於�E�U�֓�)
    Private Sub ToriCode_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles txtTorisCode.Validated, _
                txtTorifCode.Validated


        lblToriName.Text = ""
        If (txtTorisCode.Text.Trim & txtTorifCode.Text.Trim).Length <> 12 Then
            Return
        End If

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder()

        Try
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)

            SQL.Append(" SELECT * FROM S_TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(txtTorisCode.Text))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(txtTorifCode.Text))


            If OraReader.DataReader(SQL) = True Then
                lblToriName.Text = OraReader.GetString("ITAKU_NNAME_T")
            Else
                lblToriName.Text = ""
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(�������ݒ�)", "���s", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '�ϑ��Җ����X�g�{�b�N�X�̐ݒ�
        '--------------------------------
        If GCom.SelectItakuName(cmbKana.SelectedItem, cmbToriName, txtTorisCode, txtTorifCode, "3", strJyoken) = -1 Then
            MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '�����R�[�h�e�L�X�g�{�b�N�X�Ɏ����R�[�h�ݒ�
        '-------------------------------------------
        If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
            GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
            ToriCode_Validated(sender, e)
        End If
    End Sub

#End Region

    Private Function txtStratDateY() As Object
        Throw New NotImplementedException
    End Function

End Class
