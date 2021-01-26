Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon


Public Class KFGNENJ040
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' �i���߂�����
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

#Region " ���ʕϐ���` "
    Dim INT�g�p�w�N�� As Integer
    Dim INT�ō��w�N�� As Integer
    Dim STR�i���N�x As String
    Dim STR�w�Z�R�[�h As String
    Dim STR���w�N�x As String
    Dim INT�ʔ� As Integer
    Dim INT�w�N As Integer
    Dim INT�N���X As Integer
    Dim STR���k�ԍ� As String
    Dim STR���k�����J�i As String
    Dim STR���k�������� As String
    Dim STR���� As String
    Dim STR���Z�@�փR�[�h As String
    Dim STR�x�X�R�[�h As String
    Dim STR�Ȗ� As String
    Dim STR�����ԍ� As String
    Dim STR���`�l�J�i As String
    Dim STR���`�l���� As String
    Dim STR�U�֕��@ As String
    Dim STR�_��Z������ As String
    Dim STR�_��d�b�ԍ� As String
    Dim STR���敪 As String
    Dim STR�i���敪 As String
    Dim STR��ڂh�c As String
    Dim INT���q_�L���t���O As Integer
    Dim STR���q_���w�N�x As String
    Dim INT���q_�ʔ� As Integer
    Dim INT���q_�w�N As Integer
    Dim INT���q_�N���X As Integer
    Dim STR���q_���k�ԍ� As String
    Dim STR������ As String
    Dim STR��ڂP�������@ As String
    Dim LNG��ڂP�������z As Long
    Dim STR��ڂQ�������@ As String
    Dim LNG��ڂQ�������z As Long
    Dim STR��ڂR�������@ As String
    Dim LNG��ڂR�������z As Long
    Dim STR��ڂS�������@ As String
    Dim LNG��ڂS�������z As Long
    Dim STR��ڂT�������@ As String
    Dim LNG��ڂT�������z As Long
    Dim STR��ڂU�������@ As String
    Dim LNG��ڂU�������z As Long
    Dim STR��ڂV�������@ As String
    Dim LNG��ڂV�������z As Long
    Dim STR��ڂW�������@ As String
    Dim LNG��ڂW�������z As Long
    Dim STR��ڂX�������@ As String
    Dim LNG��ڂX�������z As Long
    Dim STR��ڂP�O�������@ As String
    Dim LNG��ڂP�O�������z As Long
    Dim STR��ڂP�P�������@ As String
    Dim LNG��ڂP�P�������z As Long
    Dim STR��ڂP�Q�������@ As String
    Dim LNG��ڂP�Q�������z As Long
    Dim STR��ڂP�R�������@ As String
    Dim LNG��ڂP�R�������z As Long
    Dim STR��ڂP�S�������@ As String
    Dim LNG��ڂP�S�������z As Long
    Dim STR��ڂP�T�������@ As String
    Dim LNG��ڂP�T�������z As Long
    Dim STR�쐬���t As String
    Dim STR�X�V���t As String
    Dim STR������ As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ040", "�i���߂��������")
    Private Const msgTitle As String = "�i���߂��������(KFGNENJ040)"

    '2010/10/21 �V�X�e�����t���ߋ��N�x�̏ꍇ�A�i���s�Ƃ���--------------------------------
    '�^�C���X�^���v�擾
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '���ݓ��t
    '-----------------------------------------------------------------------------------------

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '���O�p
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            With Me
                .WindowState = FormWindowState.Normal
                .FormBorderStyle = FormBorderStyle.FixedDialog
                .ControlBox = True
            End With

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            txtNendo.Text = (CInt(Mid(lblDate.Text, 1, 4)) - 1).ToString("0000")

            '���̓{�^������
            btnAction.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New MyOracle
        Dim oraReader As MyOracleReader = Nothing

        Try

            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check(MainDB) = False Then
                Return
            End If

            If MessageBox.Show(String.Format(MSG0015I, "�i���߂�"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '���͊w�Z�R�[�h�̂ݑΏ�
            LW.ToriCode = txtGAKKOU_CODE.Text

            '�g�����U�N�V�����J�n
            MainDB.BeginTrans()
            MainLOG.Write("�i�����C������", "����", "�J�n")

            '�i���߂��������s
            If Not PFUNC_RESTORE(txtGAKKOU_CODE.Text, MainDB) Then
                MainLOG.Write("�i���߂�����", "���s", "")
                MainDB.Rollback()
                Return
            End If

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            MainDB.Commit()
            MainLOG.Write("�i���߂�����", "����", "�R�~�b�g")

            MessageBox.Show(String.Format(MSG0016I, "�i���߂�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)��O�G���[", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '�I���{�^��
        Me.Close()

    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        Dim sGakkkou_Name As String = ""
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '�w�Z���̐ݒ�
        sql.Append(" SELECT GAKKOU_NNAME_G FROM GAKMAST1")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = True Then
            sGakkkou_Name = oraReader.GetString("GAKKOU_NNAME_G")
        End If
        oraReader.Close()

        '�w�Z�}�X�^�P���݃`�F�b�N
        If sGakkkou_Name = "" Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lab�w�Z��.Text = ""
            txtGAKKOU_CODE.Focus()
            Return False
        End If

        lab�w�Z��.Text = sGakkkou_Name

        Return True

    End Function

    Private Sub PFUNC_GET_NENDO()

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '�i���p�w�Z�}�X�^�Q�̎擾
        sql.Append(" SELECT * FROM SINQBK_GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = False Then
            txtNendo.Text = ""
        Else

            '�i���N�x
            If IsDBNull(oraReader.GetString("SINKYU_NENDO_T")) = False Then
                txtNendo.Text = oraReader.GetString("SINKYU_NENDO_T")
            Else
                txtNendo.Text = ""
            End If

        End If
        oraReader.Close()

    End Sub

    Private Function PFUNC_Nyuryoku_Check(ByVal db As MyOracle) As Boolean
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        Else
            '�w�Z�}�X�^���݃`�F�b�N
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(db)
            Try

                sql.Append("SELECT *")
                sql.Append(" FROM GAKMAST2")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = False Then
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If
            Catch ex As Exception
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_Nyuryoku_Check)�w�Z�}�X�^���݃`�F�b�N", "���s", ex.ToString)
                Return False
            Finally
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End Try

        End If

        If (Trim(txtNendo.Text)) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�ޔ�N�x"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtNendo.Focus()
            Return False
        Else
            '�ޔ�N�x�`�F�b�N
            sql.Length = 0
            oraReader = New MyOracleReader(db)
            Try

                sql.Append("SELECT *")
                sql.Append(" FROM SINQBK_GAKMAST2")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
                sql.Append(" AND   SINKYU_NENDO_T = '" & txtNendo.Text.Trim.PadLeft(txtNendo.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = False Then
                    MessageBox.Show(G_MSG0107W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If
            Catch ex As Exception
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_Nyuryoku_Check)�ޔ�N�x�`�F�b�N", "���s", ex.ToString)
                Return False
            Finally
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End Try
        End If

        Return True

    End Function

    ''' <summary>
    ''' �w�肳�ꂽ�w�Z�̐i���O����ޔ�����
    ''' </summary>
    ''' <param name="GAKKOU_CODE">�Ώۂ̊w�Z�R�[�h</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:�ޔ𐬌� FALSE:�ޔ����s</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_RESTORE(ByVal GAKKOU_CODE As String, ByRef db As MyOracle) As Boolean
        Dim TARGET_TABLE As String = ""
        Dim SINQBK_TABLE As String = ""
        Dim KEY_NAME As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_RESTORE)�J�n", "����", "�w�Z�R�[�h�F" & GAKKOU_CODE)

        '-----------------------------------------------
        ' �w�Z�}�X�^�Q�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "GAKMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_T"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' ��ڃ}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "HIMOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_H"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z�X�P�W���[���}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_SCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_S"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z���s�X�P�W���[���}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_TAKOUSCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_U"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' ���k�}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �V�����}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z���׃}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_MEIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_M"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z�G���g���}�X�^�P�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST1"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' �w�Z�G���g���}�X�^�Q�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' ���у}�X�^�ޔ�����
        '-----------------------------------------------
        TARGET_TABLE = "JISSEKIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_F"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_RESTORE)�I��", "����", "�w�Z�R�[�h�F" & GAKKOU_CODE)
        Return True
    End Function

    ''' <summary>
    ''' �}�X�^�߂�����
    ''' </summary>
    ''' <param name="GAKKOU_CODE">�w�Z�R�[�h</param>
    ''' <param name="TARGET_TABLE">�ޔ����e�[�u����</param>
    ''' <param name="SINQBK_TABLE">�ޔ��e�[�u����</param>
    ''' <param name="KEY_NAME">�L�[��</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:�ޔ𐬌� FALSE:�ޔ����s</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_SINQBKtoMAST(ByVal GAKKOU_CODE As String, _
                                        ByVal TARGET_TABLE As String, _
                                        ByVal SINQBK_TABLE As String, _
                                        ByVal KEY_NAME As String, _
                                        ByRef db As MyOracle) As Boolean

        Dim SYORI_NAME As String = ""

        Dim SQL As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)
        Dim bRet As Boolean = False
        Dim DEL_CNT As Long = 0
        Dim INS_CNT As Long = 0
        Dim ResultMsg As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�J�n", "����", TARGET_TABLE)

        '-----------------------------------------------
        ' �}�X�^�e�[�u�����̍폜
        '-----------------------------------------------
        Try
            SYORI_NAME = "�}�X�^�폜"

            With SQL
                .Append("DELETE FROM " & TARGET_TABLE)
                .Append(" WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            DEL_CNT = db.ExecuteNonQuery(SQL)
            If DEL_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�I��", "���s", SYORI_NAME & "�������s �w�Z�R�[�h�F" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�I��", "���s", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        '-----------------------------------------------
        ' �}�X�^�e�[�u���ւ̖߂�
        '-----------------------------------------------
        Try
            SYORI_NAME = "�}�X�^�߂�"

            SQL.Length = 0
            With SQL
                .Append("INSERT INTO " & TARGET_TABLE)
                .Append(" SELECT  * FROM " & SINQBK_TABLE & " WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            INS_CNT = db.ExecuteNonQuery(SQL)
            If INS_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�I��", "���s", SYORI_NAME & "�������s �w�Z�R�[�h�F" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�I��", "���s", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        '-----------------------------------------------
        ' �i���p�e�[�u�����̍폜
        '-----------------------------------------------
        Try
            SYORI_NAME = "�i���p�}�X�^�폜"

            SQL.Length = 0
            With SQL
                .Append("DELETE FROM " & SINQBK_TABLE)
                .Append(" WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            If db.ExecuteNonQuery(SQL) = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�I��", "���s", SYORI_NAME & "�������s �w�Z�R�[�h�F" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�I��", "���s", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        ResultMsg = String.Format("{0} �}�X�^���R�[�h�폜����={1}�� �߂�����={2}��", TARGET_TABLE, DEL_CNT.ToString, INS_CNT.ToString)
        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)�I��", "����", ResultMsg)
        Return True
    End Function

#End Region

    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            '�i���p�w�Z�}�X�^�Q���N�x���擾����
            PFUNC_GET_NENDO()

        End If

    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z����
        Call GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName)

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '�w�Z������̊w�Z�R�[�h�ݒ�
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '�w�Z���̎擾
        lab�w�Z��.Text = cmbGakkouName.Text

        '�i���p�w�Z�}�X�^�Q���N�x���擾����
        PFUNC_GET_NENDO()

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub

    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
End Class
