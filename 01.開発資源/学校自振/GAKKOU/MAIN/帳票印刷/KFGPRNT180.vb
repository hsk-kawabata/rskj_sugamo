Imports System.Text
Imports CASTCommon

''' <summary>
''' �N�ԃX�P�W���[���\������
''' </summary>
''' <remarks>2017/04/28 saitou RSV2 added for �W���@�\�ǉ�(�N�ԃX�P�W���[���\)</remarks>
Public Class KFGPRNT180

#Region "�N���X�ϐ�"
    Private Const msgTitle As String = "�N�ԃX�P�W���[���\������(KFGPRNT180)"

    '���O
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT180", "�N�ԃX�P�W���[���\������")
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure
    Private LW As LogWrite

    '�O���[�o���ϐ�
    Private GAKKOU_CODE As String
    Private NENDO As String

#End Region

#Region "�C�x���g�n���h��"

    ''' <summary>
    ''' �t�H�[�����[�h�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFGPRNT180_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '----------------------------------------
            '���O�����݂ɕK�v�ȏ��̎擾
            '----------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '���[�UID�A�V�X�e�����t�\��
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '�w�Z�p�I���N���ڑ�
            Call GSUB_CONNECT()

            '----------------------------------------
            '�w�Z�����R���{�{�b�N�X�ݒ�
            '----------------------------------------
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGakkouName)")
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �t�H�[���N���[�Y�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '�w�Z�p�I���N���ؒf
        Call GSUB_CLOSE()
    End Sub

    ''' <summary>
    ''' ����{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrnt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrnt.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            '----------------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '----------------------------------------
            If Me.CheckTextBox() = False Then
                Return
            End If

            '----------------------------------------
            '�}�X�^�̃`�F�b�N
            '----------------------------------------
            Me.GAKKOU_CODE = Me.txtGAKKOU_CODE.Text
            Me.NENDO = Me.txtFuriDateY.Text
            LW.ToriCode = Me.GAKKOU_CODE
            If Me.CheckTable() = False Then
                Return
            End If

            If MessageBox.Show(String.Format(MSG0013I, "�N�ԃX�P�W���[���\"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            '----------------------------------------
            '����o�b�`�Ăяo��
            '----------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim nRet As Integer
            Dim CmdArg As String = GCom.GetUserID & "," & Me.GAKKOU_CODE & "," & Me.NENDO

            nRet = ExeRepo.ExecReport("KFGP037.EXE", CmdArg)

            Select Case nRet
                Case 0
                    MessageBox.Show(String.Format(MSG0014I, "�N�ԃX�P�W���[���\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MessageBox.Show(String.Format(MSG0004E, "�N�ԃX�P�W���[���\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
            LW.ToriCode = "0000000000"
        End Try
    End Sub

    ''' <summary>
    ''' �I���{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEnd.Click
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

    ''' <summary>
    ''' �w�Z���J�i�R���{�{�b�N�X�@�C���f�b�N�X�`�F���W�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            If Me.cmbKana.SelectedItem.ToString = "" Then
                Return
            End If

            '�w�Z���R���{�{�b�N�X�ݒ�
            If GFUNC_DB_COMBO_SET(Me.cmbKana, cmbGakkouName) = True Then
                Me.cmbGakkouName.Focus()
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' �w�Z���R���{�{�b�N�X�@�C���f�b�N�X�`�F���W�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbGakkouName.SelectedIndexChanged
        Try
            If Not cmbGakkouName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbGakkouName.SelectedItem.ToString) Then
                '�w�Z���̎擾
                Me.lab�w�Z��.Text = Me.cmbGakkouName.Text
                Me.txtGAKKOU_CODE.Text = STR_GCOAD(Me.cmbGakkouName.SelectedIndex)

                '�w�Z�R�[�h�ɃJ�[�\���ݒ�
                Me.txtGAKKOU_CODE.Focus()
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' �w�Z�R�[�h�e�L�X�g�@�o���f�C�e�B���O�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Try
            Call GCom.NzNumberString(txtGAKKOU_CODE, True)
        Catch ex As Exception
            MainLOG.Write("�[���p�f�B���O", "���s", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' �w�Z�R�[�h�@�o���f�C�e�b�h�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtGAKKOU_CODE_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validated
        Dim dbConn As CASTCommon.MyOracle = Nothing
        Dim dbReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            Me.lab�w�Z��.Text = ""
            If Me.txtGAKKOU_CODE.Text = "9999999999" Then
                Return
            End If

            SQL.Append("select distinct")
            SQL.Append("     GAKKOU_NNAME_G")
            SQL.Append(" from")
            SQL.Append("     GAKMAST1")
            SQL.Append(" where")
            SQL.Append("     GAKKOU_CODE_G = " & SQ(Me.txtGAKKOU_CODE.Text))

            dbConn = New CASTCommon.MyOracle
            dbReader = New CASTCommon.MyOracleReader(dbConn)
            If dbReader.DataReader(SQL) = True Then
                lab�w�Z��.Text = dbReader.GetString("GAKKOU_NNAME_G")
            End If

        Catch ex As Exception
            MainLOG.Write("�w�Z���擾", "���s", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If
            If Not dbConn Is Nothing Then
                dbConn.Close()
                dbConn = Nothing
            End If
        End Try
    End Sub

#End Region

#Region "�v���C�x�[�g���\�b�h"

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X�̓��̓`�F�b�N���s���܂��B
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Private Function CheckTextBox() As Boolean
        Try
            '�w�Z�R�[�h
            If Me.txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtGAKKOU_CODE.Focus()
                Return False
            End If

            '�N�x
            If Me.txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�N�x"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�e�L�X�g�{�b�N�X���̓`�F�b�N", "���s", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' �}�X�^�̃`�F�b�N���s���܂��B
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Private Function CheckTable() As Boolean
        Dim dbConn As CASTCommon.MyOracle = Nothing
        Dim dbReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            dbConn = New CASTCommon.MyOracle
            dbReader = New CASTCommon.MyOracleReader(dbConn)

            '----------------------------------------
            '�w�Z�}�X�^�ɑ��݂��邩�`�F�b�N
            '----------------------------------------
            With SQL
                .Length = 0
                .Append("select")
                .Append("     *")
                .Append(" from")
                .Append("     GAKMAST2")
                If Me.GAKKOU_CODE = "9999999999" Then
                Else
                    .Append(" where")
                    .Append("     GAKKOU_CODE_T = " & SQ(Me.GAKKOU_CODE))
                End If
            End With

            If dbReader.DataReader(SQL) = False Then
                If Me.GAKKOU_CODE = "9999999999" Then
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                Else
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If

            dbReader.Close()

            '----------------------------------------
            '�X�P�W���[���}�X�^�ɑ��݂��邩�`�F�b�N
            '----------------------------------------
            With SQL
                .Length = 0
                .Append("select")
                .Append("     *")
                .Append(" from")
                .Append("     G_SCHMAST")
                .Append(" inner join")
                .Append("     GAKMAST2")
                .Append(" on  GAKKOU_CODE_S = GAKKOU_CODE_T")
                .Append(" where ")
                .Append("     NENGETUDO_S")
                .Append(" between")
                .Append("     " & SQ(Me.NENDO & "04"))
                .Append(" and " & SQ(CStr(CInt(Me.NENDO) + 1) & "03"))
                If Me.GAKKOU_CODE = "9999999999" Then
                Else
                    .Append(" and GAKKOU_CODE_S = " & SQ(Me.GAKKOU_CODE))
                End If
            End With

            If dbReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�}�X�^�`�F�b�N)", "���s", ex.Message)
            Return False
        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If

            If Not dbConn Is Nothing Then
                dbConn.Close()
                dbConn = Nothing
            End If
        End Try
    End Function

#End Region

End Class
