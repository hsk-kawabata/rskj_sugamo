Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFGNENJ020

#Region " ���ʕϐ���` "
    Dim strCSV_InFileName As String
    Dim strCSV_OutFileName As String
    Dim strExport_Sort As String
    Dim strExport_GakkouCode As String
    Dim strExport_Nendo As String
    Dim intExport_Tuban As Integer
    Dim strExport_KName As String
    Dim strExport_NName As String
    Dim intExport_Gakunen As Integer
    Dim intExport_Class As Integer
    Dim strExport_SeitoNo As String
    Dim strExport_OldGakkouName As String
    Dim strExport_OldClass As String
    Dim strExport_OldSeitoNo As String
    Dim strLine As String
    Dim strImport_SakujoFLG As String
    Dim strImport_GakkouCode As String
    Dim strImport_Nendo As String
    Dim strImport_Tuban As String
    Dim strImport_KName As String
    Dim strImport_NName As String
    Dim strImport_Gakunen As String
    Dim strImport_Class As String
    Dim strImport_SeitoNo As String
    Dim strImport_NewClass As String
    Dim strImport_NewSeitoNo As String
    Dim strImport_Kaigyo As String
    Dim fnbr As Integer

    Dim strErrLog As String
    Dim Str_Report_Path As String
    Dim intSiyouGakunen As Integer
    Dim strSyori As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ020", "�N���X�ւ��������")
    Private Const msgTitle As String = "�N���X�ւ��������(KFGNENJ020)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

            '���̓{�^������
            btnInyu.Enabled = True
            btnIsyutu.Enabled = True
            btnPrint.Enabled = True
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
    Private Sub btnInyu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInyu.Click
        '�ړ��{�^��
        '�b�r�u�`���e�L�X�g�𐶓k�}�X�^�Ɏ捞��
        Dim strDIR As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)�J�n", "����", "")

            MainDB = New MyOracle

            strDIR = CurDir()

            '���ʃ`�F�b�N
            If PFUC_COMMON_CHK() = False Then
                Return
            End If

            If MessageBox.Show(String.Format(MSG0015I, "�ړ�"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '���O�t�@�C���폜
            KobetuLogFileName = Path.Combine(STR_LOG_PATH, "CLASS" & txtGAKKOU_CODE.Text & ".log")
            If File.Exists(KobetuLogFileName) Then
                File.Delete(KobetuLogFileName)
            End If

            '�t�@�C�����J���_�C�A���O�{�b�N�X�̐ݒ�

            With OpenFileDialog1
                .InitialDirectory = STR_CSV_PATH
                .Filter = "csv̧��(*.csv)|*.csv"
                .FilterIndex = 1
                .FileName = "CLASS" & txtGAKKOU_CODE.Text & txtGAKUNEN.Text & ".csv"
            End With

            If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                strCSV_InFileName = OpenFileDialog1.FileName
            Else
                ChDir(strDIR)
                txtGAKKOU_CODE.Focus()
                txtGAKKOU_CODE.SelectionStart = 0
                txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
                Return
            End If

            '�b�r�u�t�@�C���̃I�[�v��
            fnbr = FreeFile()
            FileOpen(fnbr, strCSV_InFileName, OpenMode.Input)
            Try
                If PFUC_CSVINPUT_CHK() <> 0 Then
                    '�b�r�u�t�@�C���N���[�Y
                    FileClose(fnbr)

                    ChDir(strDIR)   '2008/04/17 �ǉ�

                    MainLOG.Write("�b�r�u�t�@�C���̃I�[�v��", "���s", "")

                    MessageBox.Show(String.Format(G_MSG0041W, strErrLog, STR_CSV_PATH & "CLASS" & txtGAKKOU_CODE.Text & txtGAKUNEN.Text & ".log"), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    Return
                End If
            Catch ex As Exception
                Throw New Exception(ex.ToString, ex)
            Finally
                '�b�r�u�t�@�C���N���[�Y
                FileClose(fnbr)
            End Try


            '�b�r�u�t�@�C���̃I�[�v��
            fnbr = FreeFile()
            FileOpen(fnbr, strCSV_InFileName, OpenMode.Input)

            '�g�����U�N�V�����J�n
            MainDB.BeginTrans()

            If PFUC_SEITOMAST_KOUSIN() <> 0 Then
                MainDB.Rollback()
                '�b�r�u�t�@�C���N���[�Y
                FileClose(fnbr)
                MainLOG.Write("�ړ�", "���s", "���k�}�X�^�X�V")
                ChDir(strDIR)   '2008/04/17 �ǉ�
                Return
            End If

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            MainDB.Commit()

            '�b�r�u�t�@�C���N���[�Y
            FileClose(fnbr)
            MainLOG.Write("�ړ�", "����", "")

            ChDir(strDIR)

            MessageBox.Show(String.Format(MSG0016I, "�ړ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            txtGAKKOU_CODE.Focus()
            txtGAKKOU_CODE.SelectionStart = 0
            txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)��O�G���[", "���s", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ړ�)�I��", "����", "")
        End Try

    End Sub
    Private Sub btnIsyutu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIsyutu.Click
        '�ڏo�{�^��
        '���k�}�X�^����b�r�u�`���e�L�X�g���͂��o��
        '2007/08/16�@�i�w���k�Ή�

        Dim wrt As StreamWriter = Nothing
        Dim csvFileName As String

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        Dim lngLine As Long = 0
        Dim strLine As String

        Dim strDIR As String
        strDIR = CurDir()

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)�J�n", "����", "")

            '==========================================
            '���ʃ`�F�b�N
            '==========================================
            If PFUC_COMMON_CHK() = False Then
                Return
            End If

            '���k���݃`�F�b�N
            '2006/10/19�@���k��1�l���o�^����Ă��Ȃ��ꍇ�A���b�Z�[�W��\�����A�����𒆒f����
            If PFUNC_SEITO_CHECK() = False Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "�ڏo"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '==========================================
            '�t�@�C���ۑ��̃_�C�A���O�{�b�N�X�̐ݒ�
            '==========================================
            SaveFileDialog1.InitialDirectory = STR_CSV_PATH
            SaveFileDialog1.Filter = "csv̧��(*.csv)|*.csv|�S�Ă�̧��(*.*)|*.*"
            SaveFileDialog1.FilterIndex = 1
            SaveFileDialog1.FileName = "CLASS" & txtGAKKOU_CODE.Text & txtGAKUNEN.Text & ".csv"

            If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                csvFileName = SaveFileDialog1.FileName
            Else
                ChDir(strDIR)
                txtGAKKOU_CODE.Focus()
                txtGAKKOU_CODE.SelectionStart = 0
                txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
                Exit Sub
            End If

            '==========================================
            '���k�}�X�^�̓Ǎ���
            '==========================================

            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            sql.Append("SELECT * FROM SEITOMAST")
            sql.Append(" WHERE GAKKOU_CODE_O = " & "'" & txtGAKKOU_CODE.Text & "'")
            sql.Append(" AND GAKUNEN_CODE_O = " & CInt(txtGAKUNEN.Text))
            sql.Append(" AND TUKI_NO_O ='04'")

            Select Case strExport_Sort
                Case "0"      '�w�N�A�N���X�A���k�ԍ�
                    sql.Append(" ORDER BY GAKUNEN_CODE_O ASC")
                    sql.Append(",CLASS_CODE_O ASC")
                    sql.Append(",SEITO_NO_O ASC")
                Case "1"      '���w�N�x�A�ʔ�
                    sql.Append(" ORDER BY NENDO_O ASC")
                    sql.Append(",TUUBAN_O ASC")
                Case "2"      '���k���̃A�C�E�G�I��
                    sql.Append(" ORDER BY SEITO_KNAME_O ASC")
            End Select

            If oraReader.DataReader(sql) = True Then

                '�^�C�g���o��
                strLine = "�폜�t���O,�w�Z�R�[�h,�N�x,�ʔ�,���k���J�i,���k��,�w�N,���N���X,�����k�ԍ�,�V�N���X,�V���k�ԍ�"
                strLine += vbCrLf

                Do Until oraReader.EOF
                    '�e���ڎ擾
                    strExport_GakkouCode = Trim(oraReader.GetString("GAKKOU_CODE_O"))
                    strExport_Nendo = Trim(oraReader.GetString("NENDO_O"))
                    intExport_Tuban = oraReader.GetString("TUUBAN_O")
                    strExport_KName = Trim(oraReader.GetString("SEITO_KNAME_O"))
                    If IsDBNull(oraReader.GetString("SEITO_NNAME_O")) = False Then
                        strExport_NName = Trim(oraReader.GetString("SEITO_NNAME_O"))
                    Else
                        strExport_NName = ""
                    End If
                    intExport_Gakunen = oraReader.GetString("GAKUNEN_CODE_O")
                    intExport_Class = oraReader.GetString("CLASS_CODE_O")
                    strExport_SeitoNo = Trim(oraReader.GetString("SEITO_NO_O"))

                    '�b�r�u�t�@�C��������
                    strLine += "0" & ","
                    strLine += strExport_GakkouCode & ","
                    strLine += strExport_Nendo & ","
                    strLine += intExport_Tuban & ","
                    strLine += strExport_KName & ","
                    strLine += strExport_NName & ","
                    strLine += intExport_Gakunen & ","
                    strLine += intExport_Class & ","
                    strLine += strExport_SeitoNo & ","
                    strLine += " " & ","
                    strLine += " "
                    strLine += vbCrLf

                    oraReader.NextRead()

                Loop

                wrt = New System.IO.StreamWriter(csvFileName, False, System.Text.Encoding.Default)

                wrt.Write(strLine)

                wrt.Close()

            Else
                MessageBox.Show(G_MSG0042W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                oraReader.Close()
                ChDir(strDIR)
                Return
            End If

            oraReader.Close()

            ChDir(strDIR)

            MessageBox.Show(String.Format(MSG0016I, "�ڏo"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            txtGAKKOU_CODE.Focus()
            txtGAKKOU_CODE.SelectionStart = 0
            txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)�I��", "����", "")
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not wrt Is Nothing Then wrt.Close()
        End Try

    End Sub

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim Param As String
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle

            LW.ToriCode = txtGAKKOU_CODE.Text

            '���ʃ`�F�b�N
            If PFUC_COMMON_CHK() = False Then
                Exit Sub
            End If

            '2006/10/19�@���k��1�l���o�^����Ă��Ȃ��ꍇ�A���b�Z�[�W��\�����A�����𒆒f����
            If PFUNC_SEITO_CHECK() = False Then
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0013I, "�i�����X�g"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            Dim nRet As Integer
            '���ԃX�P�W���[���\��� 
            '�p�����[�^�ݒ�F���O�C����,�w�Z�R�[�h,�w�N,���[�\�[�g��
            Param = GCom.GetUserID & "," & txtGAKKOU_CODE.Text & "," & txtGAKUNEN.Text & "," & strExport_Sort

            nRet = ExeRepo.ExecReport("KFGP011.EXE", Param)
            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case nRet
                Case 0
                    '�����m�F���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0014I, "�i�����X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '����ΏۂȂ����b�Z�[�W
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, "�i�����X�g"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
            MainDB.Rollback()
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '���s�{�^��

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")

            '���ʃ`�F�b�N
            If PFUC_COMMON_CHK() = False Then
                Exit Sub
            End If

            '2006/10/19�@���k��1�l���o�^����Ă��Ȃ��ꍇ�A���b�Z�[�W��\�����A�����𒆒f����
            If PFUNC_SEITO_CHECK() = False Then
                Exit Sub
            End If

            STR_�N���X�֊w�Z�R�[�h = txtGAKKOU_CODE.Text
            STR_�N���X�֊w�Z�� = lab�w�Z��.Text
            STR_�N���X�֊w�N�R�[�h = txtGAKUNEN.Text
            STR_�N���X�֊w�N�� = labGAKUNEN.Text

            Dim KFGNENJ021 As New GAKKOU.KFGNENJ021
            'KFGNENJ021.ShowDialog(Me)
            Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGNENJ021, Form), Me)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
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
    Private Function PFUC_GAKNAME_GET() As Integer
        '�w�Z���̐ݒ�
        PFUC_GAKNAME_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST1 WHERE"
        STR_SQL += "     GAKKOU_CODE_G  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        '�w�Z�}�X�^�P���݃`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lab�w�Z��.Text = ""
            txtGAKKOU_CODE.Focus()
            PFUC_GAKNAME_GET = 1
            Exit Function
        End If
        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
        OBJ_DATAREADER.Read()

        lab�w�Z��.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G")

        OBJ_DATAREADER.Close()


    End Function
    Private Function PFUC_GAKNENNAME_GET() As Integer
        '�w�N���̐ݒ�
        PFUC_GAKNENNAME_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST1 WHERE"
        STR_SQL += "     GAKKOU_CODE_G  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " AND GAKUNEN_CODE_G = " & "'" & txtGAKUNEN.Text & "'"
        '�w�Z�}�X�^�P���݃`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            MessageBox.Show(String.Format(G_MSG0033W, txtGAKUNEN.Text), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            labGAKUNEN.Text = ""
            txtGAKUNEN.Focus()
            PFUC_GAKNENNAME_GET = 1
            Exit Function
        End If
        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
        OBJ_DATAREADER.Read()

        labGAKUNEN.Text = OBJ_DATAREADER.Item("GAKUNEN_NAME_G")

        OBJ_DATAREADER.Close()


    End Function
    Private Function PFUC_GAKMAST2_GET() As Integer
        '�w�Z�}�X�^�Q�̎擾
        PFUC_GAKMAST2_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST2 WHERE"
        STR_SQL += "     GAKKOU_CODE_T  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        '�w�Z�}�X�^�Q���݃`�F�b�N
        If GFUNC_ISEXIST(STR_SQL) = False Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            PFUC_GAKMAST2_GET = 1
            Exit Function
        End If
        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

        OBJ_DATAREADER.Read()

        '�g�p�w�N���̊i�[
        intSiyouGakunen = OBJ_DATAREADER.Item("SIYOU_GAKUNEN_T")
        '���[�\�[�g��
        If IsDBNull(OBJ_DATAREADER.Item("MEISAI_OUT_T")) = False Then
            strExport_Sort = OBJ_DATAREADER.Item("MEISAI_OUT_T")
        Else
            '�����l�ݒ�
            strExport_Sort = "0"
        End If
        OBJ_DATAREADER.Close()

    End Function
    Private Function PFUC_COMMON_CHK() As Boolean

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        End If

        '�w�N
        If Trim(txtGAKUNEN.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�N�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN.Focus()
            Return False
        End If

        If CInt(txtGAKUNEN.Text) > intSiyouGakunen Then
            MessageBox.Show(String.Format(G_MSG0033W, txtGAKUNEN.Text), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN.Focus()
            Return False
        End If

        Return True

    End Function
    Private Function PFUC_CSVINPUT_CHK() As Integer

        '�b�r�u�`���e�L�X�g�̓Ǎ��݂ƃ`�F�b�N
        Dim J As Integer
        Dim SEQNO As Integer
        Dim chkSEITO As Boolean = False
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUC_CSVINPUT_CHK = 0

        SEQNO = 0

        Do While Not EOF(fnbr)
            '�w�b�_�^�C�g���̓ǎ̂�
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
            Next i

            SEQNO = SEQNO + 1
            Exit Do
        Loop

        Do While Not EOF(fnbr)
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
                Select Case i
                    Case 1
                        strImport_SakujoFLG = strLine
                    Case 2
                        strImport_GakkouCode = strLine
                    Case 3
                        strImport_Nendo = strLine
                    Case 4
                        strImport_Tuban = strLine
                    Case 5
                        strImport_KName = strLine
                    Case 6
                        strImport_NName = strLine
                    Case 7
                        strImport_Gakunen = strLine
                    Case 8
                        strImport_Class = strLine
                    Case 9
                        strImport_SeitoNo = strLine
                    Case 10
                        strImport_NewClass = strLine
                    Case 11
                        strImport_NewSeitoNo = strLine
                        'Case 12
                        'strImport_Kaigyo = strLine
                End Select
            Next i

            SEQNO = SEQNO + 1

            '���͍��ڂ̃`�F�N
            ''�폜�t���O
            If strImport_SakujoFLG = "0" Or strImport_SakujoFLG = "1" Then
            Else
                strErrLog = "�폜�t���O�G���[" & " SEQNO=" & SEQNO & " �폜�t���O��" & strImport_SakujoFLG
                PFUC_ERRLOG_OUT()
                PFUC_CSVINPUT_CHK = 1
            End If


            ''�w�Z�}�X�^�P�̌���
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)
            sql.Append("SELECT * FROM KZFMAST.GAKMAST1 WHERE")
            sql.Append("     GAKKOU_CODE_G  = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
            sql.Append("AND  GAKUNEN_CODE_G  = " & CInt(strImport_Gakunen)) '�ǉ� 2006/12/05

            '�w�Z�}�X�^�P���݃`�F�b�N
            If oraReader.DataReader(sql) = False Then
                strErrLog = "�w�Z�R�[�h�G���[" & " SEQNO=" & SEQNO & "�@�w�Z�R�[�h��" & strImport_GakkouCode
                PFUC_ERRLOG_OUT()
                PFUC_CSVINPUT_CHK = 1
            Else
                If Trim(strImport_NewClass) <> "" Then

                    '�N���X�R�[�h
                    J = 0
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE101_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE101_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE102_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE102_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE103_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE103_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE104_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE104_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE105_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE105_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE106_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE106_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE107_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE107_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE108_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE108_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE109_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE109_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE110_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE110_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE111_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE111_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE112_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE112_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE113_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE113_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE114_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE114_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE115_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE115_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE116_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE116_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE117_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE117_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE118_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE118_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE119_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE119_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE120_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE120_G")) Then
                                J = 1
                            End If
                    End Select

                    '�V�N���X���w�Z�}�X�^�P�ɂ��邩�H
                    If J = 0 Then
                        strErrLog = "�V�N���X���o�^�G���[" & " SEQNO=" & SEQNO & " �V�N���X��" & strImport_NewClass
                        PFUC_ERRLOG_OUT()
                        PFUC_CSVINPUT_CHK = 1
                    End If
                End If

            End If
            oraReader.Close()

            '���k�}�X�^�̌���
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)
            sql.Append("SELECT * FROM KZFMAST.SEITOMAST WHERE")
            sql.Append("     GAKKOU_CODE_O   = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
            sql.Append(" AND NENDO_O         = " & "'" & strImport_Nendo & "'")
            sql.Append(" AND TUUBAN_O        = " & CInt(strImport_Tuban))
            sql.Append(" AND GAKUNEN_CODE_O  = " & CInt(strImport_Gakunen))
            If oraReader.DataReader(sql) = False Then
                strErrLog = "���k�}�X�^�ɑ��݂��܂���" & " SEQNO=" & SEQNO
                PFUC_ERRLOG_OUT()
                PFUC_CSVINPUT_CHK = 1
            End If
            oraReader.Close()
            chkSEITO = True
        Loop

        '�b�r�u�t�@�C����1�������͂���Ă��Ȃ��ꍇ�A�ړ��ł��Ȃ�
        If chkSEITO = False Then
            PFUC_CSVINPUT_CHK = 1
            MessageBox.Show(G_MSG0043W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Function
    Private Function PFUC_ERRLOG_OUT() As Integer
        Dim Obj_StreamWriter As StreamWriter

        PFUC_ERRLOG_OUT = 0

        Try

            '���O�̏�������
            Obj_StreamWriter = New StreamWriter(KobetuLogFileName, _
                                                True, _
                                                Encoding.GetEncoding("Shift_JIS"))
        Catch ex As Exception

            PFUC_ERRLOG_OUT = 1
            Exit Function

        End Try

        Obj_StreamWriter.WriteLine(strErrLog)
        Obj_StreamWriter.Flush()
        Obj_StreamWriter.Close()
    End Function
    Private Function PFUC_SEITOMAST_KOUSIN() As Integer
        '���k�}�X�^�X�V�i�V�N���X�A�V���k�ԍ��j

        PFUC_SEITOMAST_KOUSIN = 0

        Do While Not EOF(fnbr)
            '�w�b�_�^�C�g���̓ǎ̂�
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
            Next i
            Exit Do
        Loop

        Do While Not EOF(fnbr)
            '�b�r�u�P�s�Ǎ���
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
                Select Case i
                    Case 1
                        strImport_SakujoFLG = Trim(strLine)
                    Case 2
                        strImport_GakkouCode = Trim(strLine)
                    Case 3
                        strImport_Nendo = Trim(strLine)
                    Case 4
                        strImport_Tuban = Trim(strLine)
                    Case 5
                        strImport_KName = Trim(strLine)
                    Case 6
                        strImport_NName = Trim(strLine)
                    Case 7
                        strImport_Gakunen = Trim(strLine)
                    Case 8
                        strImport_Class = Trim(strLine)
                    Case 9
                        strImport_SeitoNo = Trim(strLine)
                    Case 10
                        strImport_NewClass = Trim(strLine)
                    Case 11
                        strImport_NewSeitoNo = Trim(strLine)
                        'Case 12
                        'strImport_Kaigyo = strLine
                End Select
            Next i

            If strImport_SakujoFLG = "1" Then
                '���k�}�X�^�폜����
                If PFUC_SEITOMAST_DEL() = False Then
                    Return False
                End If
            Else
                '���k�}�X�^�X�V����
                If PFUC_SEITOMAST_UPD() = False Then
                    Return False
                End If
            End If

        Loop

    End Function

    Private Function PFUC_SEITOMAST_UPD() As Boolean

        Dim sql As New StringBuilder(128)

        '�V�N���X�A�V���k�ԍ�
        If Trim(strImport_NewClass) = "" And Trim(strImport_NewSeitoNo) = "" Then
            Return True
        Else
            '�V�N���X�A�V���k�ԍ��̓��͂���
            If Trim(strImport_NewClass) <> "" And Trim(strImport_NewSeitoNo) <> "" Then
                sql.Append("UPDATE  KZFMAST.SEITOMAST SET ")
                sql.Append("   CLASS_CODE_O     = " & CInt(strImport_NewClass))
                sql.Append("  ,SEITO_NO_O       = " & "'" & Format(CInt(strImport_NewSeitoNo), "0000000") & "'")
            Else
                '�V�N���X�̂ݓ��͂���
                If Trim(strImport_NewClass) <> "" And Trim(strImport_NewSeitoNo) = "" Then
                    sql.Append("UPDATE  KZFMAST.SEITOMAST SET ")
                    sql.Append("   CLASS_CODE_O     = " & CInt(strImport_NewClass))
                Else
                    '�V���k�ԍ��̂ݓ��͂���
                    If Trim(strImport_NewClass) = "" And Trim(strImport_NewSeitoNo) <> "" Then
                        sql.Append("UPDATE  KZFMAST.SEITOMAST SET ")
                        sql.Append("   SEITO_NO_O       = " & "'" & Format(CInt(strImport_NewSeitoNo), "0000000") & "'")
                    End If
                End If
            End If
        End If
        sql.Append("  ,KOUSIN_DATE_O    = " & "'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
        sql.Append("  WHERE ")
        sql.Append("     GAKKOU_CODE_O  = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
        sql.Append(" AND GAKUNEN_CODE_O = " & CInt(strImport_Gakunen))
        sql.Append(" AND NENDO_O   = '" & CStr(strImport_Nendo) & "'")
        sql.Append(" AND TUUBAN_O   �@= " & CInt(strImport_Tuban))

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    Private Function PFUC_SEITOMAST_DEL() As Boolean
        '���k�}�X�^�̍폜

        Dim sql As New StringBuilder(128)

        sql.Append("DELETE  FROM KZFMAST.SEITOMAST WHERE")
        sql.Append("     GAKKOU_CODE_O   = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
        sql.Append(" AND GAKUNEN_CODE_O  = " & CInt(strImport_Gakunen))
        sql.Append(" AND NENDO_O   = '" & CStr(strImport_Nendo) & "'")
        sql.Append(" AND TUUBAN_O   �@= " & CInt(strImport_Tuban))

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_SEITO_CHECK() As Boolean
        '2006/10/19�@�ǉ��F���͂��ꂽ�w�Z�E�w�N�̐��k�����݂��邩�ǂ����`�F�b�N
        '2007/08/16�@�ǉ��F�ʏ�Ɛi�w���k�̑I���ɂ���ă`�F�b�N���e��ύX

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_SEITO_CHECK = False

        '���k�}�X�^�̓Ǎ���
        sql.Append("SELECT * FROM KZFMAST.SEITOMAST WHERE")
        sql.Append(" GAKKOU_CODE_O = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
        sql.Append(" AND GAKUNEN_CODE_O = " & CInt(txtGAKUNEN.Text))
        sql.Append(" AND TUKI_NO_O ='04'")

        If oraReader.DataReader(sql) = True Then
            oraReader.Close()
            Return True
        Else
            oraReader.Close()
            MessageBox.Show(G_MSG0044W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        End If

    End Function
#End Region

    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) <> "" Then

            '�w�Z���̎擾
            If PFUC_GAKNAME_GET() <> 0 Then
                Exit Sub
            End If

            '�w�N�R�[�h�����͂���Ă����ꍇ�A�w�N���擾
            If Trim(txtGAKUNEN.Text) <> "" Then
                If PFUC_GAKNENNAME_GET() <> 0 Then
                    Exit Sub
                End If
            End If

            '���[�\�[�g���̎擾
            If PFUC_GAKMAST2_GET() <> 0 Then
                Exit Sub
            End If
        Else
            lab�w�Z��.Text = ""
            labGAKUNEN.Text = ""
        End If

    End Sub
    Private Sub txtGAKUNEN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN.Validating
        '�w�N�R�[�h
        If Trim(txtGAKUNEN.Text) <> "" Then
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                If PFUC_GAKNENNAME_GET() <> 0 Then
                    Exit Sub
                End If
            End If
        Else
            labGAKUNEN.Text = ""
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

        lab�w�Z��.Text = cmbGakkouName.Text

        '�w�Z������̊w�Z�R�[�h�ݒ�
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
