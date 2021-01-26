Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT150
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' ���q�`�F�b�N���X�g���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Private STR�w�Z�� As String
    '2006/10/11 ���[�̃\�[�g�@�\�ǉ�
    Dim STR���[�\�[�g�� As String
    Dim STR�w�Z�R�[�h As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT150", "���k�}�X�^�Ǘ��\������")
    Private Const msgTitle As String = "���k�}�X�^�Ǘ��\������(KFGPRNT150)"
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
    Private Sub KFGMAST130_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            '�w�Z�R�[�h�K�{�`�F�b�N
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)

            SQL.Append("SELECT DISTINCT GAKKOU_CODE_T FROM GAKMAST2 ")

            'SQL.Append(" AND TYOUSI_FLG_O <> 0 ")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" WHERE ")
                SQL.Append(" GAKKOU_CODE_T = " & SQ(txtGAKKOU_CODE.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_T ASC")

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "���k�}�X�^�Ǘ��\"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim intINSATU_FLG2 As Integer = 0
            OraReader = New MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_T")
                    '���[�\�[�g���̎擾
                    Dim counter As Double = 0
                    If PFUNC_SEITO_CNT(STR�w�Z�R�[�h, counter) = False Then
                        Return
                    End If
                    '���O�C��ID,�w�Z�R�[�h
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & counter
                    nRet = ExeRepo.ExecReport("KFGP033.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                        Case -1
                            '����ΏۂȂ����b�Z�[�W
                            MessageBox.Show("���k�}�X�^�ɐ��k�����݂��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            Return
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "���k�}�X�^�Ǘ��\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select

                    OraReader.NextRead()
                End While
            Else
                MessageBox.Show("���k�}�X�^�̌����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z����2)", "���s", "")
                Return
            End If
            MessageBox.Show(String.Format(MSG0014I, "���k�}�X�^�Ǘ��\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
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
            If Trim(STR�w�Z�R�[�h) = "9999999999" Then
                lab�w�Z��.Text = ""
            Else
                SQL.Append(" SELECT ")
                SQL.Append(" DISTINCT GAKKOU_NNAME_G")
                SQL.Append(" FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(STR�w�Z�R�[�h))

                If OraReader.DataReader(SQL) = False Then

                    MessageBox.Show("�w�Z�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    'lab�w�Z��.Text = ""
                    'STR���[�\�[�g�� = 0

                    Exit Function
                End If

                If NameChg Then lab�w�Z��.Text = OraReader.GetString("GAKKOU_NNAME_G")
                'STR���[�\�[�g�� = OraReader.GetInt("MEISAI_OUT_T")

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

    Private Function PFUNC_SEITO_CNT(ByVal GakkouCode As String, ByRef Count As Double) As Boolean

        '�w�Z���̐ݒ�
        PFUNC_SEITO_CNT = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            If Trim(STR�w�Z�R�[�h) = "9999999999" Then
                lab�w�Z��.Text = ""
            Else
                SQL.Append(" SELECT ")
                SQL.Append(" COUNT(*) AS CNT")
                SQL.Append(" FROM SEITOMASTVIEW")
                SQL.Append(" WHERE ")
                SQL.Append(" GAKKOU_CODE_O = " & SQ(GakkouCode))

                If OraReader.DataReader(SQL) = False Then

                    MessageBox.Show("���k�}�X�^�̌����Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                    'lab�w�Z��.Text = ""
                    'STR���[�\�[�g�� = 0

                    Exit Function
                Else
                    Count = CDbl(OraReader.GetString("CNT"))
                End If


            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���k������)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_SEITO_CNT = True

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

End Class
