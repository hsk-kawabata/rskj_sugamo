Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT100
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
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT100", "���q�`�F�b�N���X�g���ו\������")
    Private Const msgTitle As String = "���q�`�F�b�N���X�g���ו\������(KFGPRNT100)"
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
    Private Sub KFGPRNT100_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

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
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
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


            OraReader = New MyOracleReader(MainDB)

            SQL.Append("SELECT DISTINCT GAKKOU_CODE_O FROM SEITOMAST ")
            SQL.Append(" WHERE TUKI_NO_O = '04'")
            SQL.Append(" AND TYOUSI_FLG_O <> 0 ")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_O ASC")

            '���k���݃`�F�b�N
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("���q����̐��k�����݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "���q�`�F�b�N���X�g���ו\"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim intINSATU_FLG2 As Integer = 0
            While OraReader.EOF = False
                STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_O")
                '���[�\�[�g���̎擾
                If PFUNC_GAKNAME_GET(False) = False Then
                    STR���[�\�[�g�� = "0"
                End If

                '���O�C��ID,�w�Z�R�[�h,�\�[�g��
                Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR���[�\�[�g��
                nRet = ExeRepo.ExecReport("KFGP029.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "���q�`�F�b�N���X�g���ו\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select

                OraReader.NextRead()
            End While
            MessageBox.Show(String.Format(MSG0014I, "���q�`�F�b�N���X�g���ו\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
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
    Private Function PFUC_SQLQuery_���q���X�g() As String
        '2006/10/11 ���[�̏o�͏����w�肷�邽�߁AJYOKEN�ł͂Ȃ�SQL���w�肷��悤�ɕύX
        Dim SSQL As String

        PFUC_SQLQuery_���q���X�g = ""


        SSQL = " SELECT "
        SSQL = SSQL & " GAKMAST1.GAKKOU_NNAME_G, "
        SSQL = SSQL & " SEITOMAST.GAKKOU_CODE_O, "
        SSQL = SSQL & " SEITOMAST.NENDO_O, "
        SSQL = SSQL & " SEITOMAST.TUUBAN_O, "
        SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O, "
        SSQL = SSQL & " SEITOMAST.CLASS_CODE_O, "
        SSQL = SSQL & " SEITOMAST.SEITO_NO_O, "
        SSQL = SSQL & " SEITOMAST.SEITO_KNAME_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_NENDO_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_GAKUNEN_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_CLASS_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_SEITONO_O, "
        SSQL = SSQL & " SEITOMAST_1.SEITO_KNAME_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_FLG_O, "
        SSQL = SSQL & " SEITOMAST_1.TUUBAN_O "
        SSQL = SSQL & " FROM   "
        SSQL = SSQL & " KZFMAST.SEITOMAST SEITOMAST, "
        SSQL = SSQL & " KZFMAST.GAKMAST1 GAKMAST1, "
        SSQL = SSQL & " KZFMAST.SEITOMAST SEITOMAST_1"
        SSQL = SSQL & "  WHERE  "
        SSQL = SSQL & " ((SEITOMAST.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G) AND "
        SSQL = SSQL & " (SEITOMAST.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G)) AND "
        SSQL = SSQL & " (((((((SEITOMAST.GAKKOU_CODE_O=SEITOMAST_1.GAKKOU_CODE_O (+)) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_NENDO_O=SEITOMAST_1.NENDO_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_TUUBAN_O=SEITOMAST_1.TUUBAN_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_GAKUNEN_O=SEITOMAST_1.GAKUNEN_CODE_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_CLASS_O=SEITOMAST_1.CLASS_CODE_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_SEITONO_O=SEITOMAST_1.SEITO_NO_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TUKI_NO_O=SEITOMAST_1.TUKI_NO_O (+))) AND "

        If Trim(STR�w�Z�R�[�h) <> "9999999999" Then
            '���k�}�X�^�i�w�Z�R�[�h�A�쐬���A�X�V���j
            '���R�[�h���o�ݒ�
            '�w��w�Z�R�[�h
            SSQL = SSQL & " SEITOMAST.GAKKOU_CODE_O = '" & Trim(STR�w�Z�R�[�h) & "' AND "
        End If

        SSQL = SSQL & " SEITOMAST.TUKI_NO_O = '04' AND "
        SSQL = SSQL & " SEITOMAST.TYOUSI_FLG_O <>0 "

        SSQL = SSQL & " ORDER BY "
        SSQL = SSQL & " SEITOMAST.GAKKOU_CODE_O�@ASC, "
        Select Case STR���[�\�[�g��
            Case "0"
                SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O�@ASC, "
                SSQL = SSQL & " SEITOMAST.CLASS_CODE_O     ASC,"
                SSQL = SSQL & " SEITOMAST.SEITO_NO_O       ASC,"
                SSQL = SSQL & " SEITOMAST.NENDO_O  ASC, "
                SSQL = SSQL & " SEITOMAST.TUUBAN_O  ASC"
            Case "1"
                SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O�@ASC, "
                '2007/02/14 �N�x���~���ɏC��
                'SSQL = SSQL & " SEITOMAST.NENDO_O  ASC, "
                SSQL = SSQL & " SEITOMAST.NENDO_O  DESC, "
                SSQL = SSQL & " SEITOMAST.TUUBAN_O  ASC"
            Case Else
                SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O�@ASC, "
                SSQL = SSQL & " SEITOMAST.SEITO_KNAME_O   ASC,"
                SSQL = SSQL & " SEITOMAST.NENDO_O  DESC, "
                SSQL = SSQL & " SEITOMAST.TUUBAN_O  ASC"
        End Select
        PFUC_SQLQuery_���q���X�g = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

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
