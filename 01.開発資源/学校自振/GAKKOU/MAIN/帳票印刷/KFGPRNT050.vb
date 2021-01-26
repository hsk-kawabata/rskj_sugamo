' ============================================================================
'  HISTORY
'   No  Ver     Date          Name              Comment
'   01  V01L01  2020/06/16    FJH)AMANO         �o�j�f�C��(PKG_2020_0012_000)
' ============================================================================
Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text

Public Class KFGPRNT050
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' ���[�󋵈ꗗ�\���
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

#Region " ���ʕϐ���` "
    Dim Str_Report_Path As String
    Dim Str_Report_Path2 As String
    'Dim Str_Connection As String

    'Dim OBJ_CONNECTION2 As New OleDb.OleDbConnection
    'Dim OBJ_DATAREADER2 As OleDb.OleDbDataReader
    'Dim OBJ_COMMAND2 As New OleDb.OleDbCommand

    'Dim OBJ_CONNECTION3 As New OleDb.OleDbConnection
    'Dim OBJ_DATAREADER3 As OleDb.OleDbDataReader
    'Dim OBJ_COMMAND3 As New OleDb.OleDbCommand

    'Dim OBJ_CONNECTION4 As New OleDb.OleDbConnection
    'Dim OBJ_DATAREADER4 As OleDb.OleDbDataReader
    'Dim OBJ_COMMAND4 As New OleDb.OleDbCommand

    Dim STR�w�Z���J�i As String
    Dim STR�w�Z������ As String
    Dim STR�w�N������ As String
    Dim STR�N���X�� As String
    Dim STR�U�֌��� As String
    Dim STR�����N�� As String
    Dim LNG�U�֋��z���v As Long
    Dim STR��ږ��P As String
    Dim STR��ږ��Q As String
    Dim STR��ږ��R As String
    Dim STR��ږ��S As String
    Dim STR��ږ��T As String
    Dim STR��ږ��U As String
    Dim STR��ږ��V As String
    Dim STR��ږ��W As String
    Dim STR��ږ��X As String
    Dim STR��ږ��P�O As String
    Dim STR��ږ��P�P As String
    Dim STR��ږ��P�Q As String
    Dim STR��ږ��P�R As String
    Dim STR��ږ��P�S As String
    Dim STR��ږ��P�T As String
    Dim STR�w�Z�R�[�h As String
    Dim STR������ As String

    Private STR�\�[�g�� As String

    Private Str_GAKKOU_CODE As String
    Private Str_FURI_DATE As String
    ''2006/10/12 �ĐU�A���z���ŏ����ςɂȂ����ꍇ���l������
    ''DATAREADER���g�p���Ă��鎞�Ƀf�[�^�o�^���s���ۂɎg�p
    'Public OBJ_CONNECTION_DREAD4 As Data.OracleClient.OracleConnection
    'Public OBJ_DATAREADER_DREAD4 As Data.OracleClient.OracleDataReader
    'Public OBJ_COMMAND_DREAD4 As Data.OracleClient.OracleCommand

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT050", "���[�󋵈ꗗ�\������")
    Private Const msgTitle As String = "���[�󋵈ꗗ�\������(KFGPRNT050)"
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
    Private Sub KFGPRNT050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Dim PrtCount As Integer = 0
        Try
            '����{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            MainDB = New MyOracle
            '����{�^��
            '���͒l�`�F�b�N
            If PFUNC_NYUURYOKU_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text & "00"
            LW.FuriDate = STR_FURIKAE_DATE(1)

            Dim str���[�󋵈ꗗ�\_NAME As String = "���[�󋵈ꗗ�\.rpt"
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then

                STR�w�Z�R�[�h = Str_GAKKOU_CODE
                STR�����N�� = Mid(Str_FURI_DATE, 1, 6)

                If PSUB_GAK_SORT() = False Then
                    Return
                End If

                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                MainDB.BeginTrans()
                If PFUNC_PRTWORK_DEL() = False Then
                    MainDB.Rollback()
                    Exit Sub
                End If
                'G_PRTWORK�̍쐬
                If PFUNC_PRTWORK_MAKE() = False Then
                    Exit Sub
                End If
                MainDB.Commit()

                '����O�m�F���b�Z�[�W
                If MessageBox.Show(String.Format(MSG0013I, "���[�󋵈ꗗ�\"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                '�`�F�b�N����Ă���Έ�� 
                If chk1.Checked = True Then
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR�\�[�g��
                    nRet = ExeRepo.ExecReport("KFGP023.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                            PrtCount += 1
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "���[�󋵈ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select
                End If

                If chk2.Checked = True Then '���[�󋵈ꗗ�\(��ڕʍ��v)���
                    Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & Str_FURI_DATE & "," & STR�����N��
                    nRet = ExeRepo.ExecReport("KFGP024.EXE", Param)
                    '�߂�l�ɑΉ��������b�Z�[�W��\������
                    Select Case nRet
                        Case 0
                            PrtCount += 1
                        Case Else
                            '������s���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0004E, "���[�󋵈ꗗ�\(��ڕʍ��v)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select
                End If
            Else
                '���͊w�Z�R�[�h���`�k�k�X�̏ꍇ
                '�X�P�W���[���}�X�^�̌���
                '�U�֓��A�U�֋敪�i���U�j
                SQL.Append("SELECT * FROM KZFMAST.G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(Str_FURI_DATE))
                SQL.Append(" AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')")
                SQL.Append(" AND FUNOU_FLG_S = '1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ")
                OraReader = New MyOracleReader(MainDB)
                '�X�P�W���[�������݃`�F�b�N
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("�X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                MainDB.BeginTrans()
                If PFUNC_PRTWORK_DEL() = False Then
                    MainDB.Rollback()
                    Exit Sub
                End If
                MainDB.Commit()
                STR�w�Z�R�[�h = ""
                '����O�m�F���b�Z�[�W
                If MessageBox.Show(String.Format(MSG0013I, "���[�󋵈ꗗ�\"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If
                While OraReader.EOF = False
                    If STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR�w�Z�R�[�h = OraReader.GetString("GAKKOU_CODE_S")
                        If PFUNC_SCHMAST_GET() = False Then
                            GoTo next_GAKKOU
                        End If

                        STR�����N�� = OraReader.GetString("NENGETUDO_S")

                        If PSUB_GAK_SORT() = False Then
                            Return
                        End If

                        'PRTWORK�̍쐬
                        MainDB.BeginTrans()
                        If PFUNC_PRTWORK_MAKE() = False Then
                            MainDB.Rollback()
                            Exit Sub
                        End If
                        MainDB.Commit()

                        '�`�F�b�N����Ă���Έ��
                        If chk1.Checked = True Then
                            Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & STR�\�[�g��
                            nRet = ExeRepo.ExecReport("KFGP023.EXE", Param)
                            '�߂�l�ɑΉ��������b�Z�[�W��\������
                            Select Case nRet
                                Case 0
                                    PrtCount += 1
                                Case Else
                                    '������s���b�Z�[�W
                                    MessageBox.Show(String.Format(MSG0004E, "���[�󋵈ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return
                            End Select
                        End If

                        If chk2.Checked = True AndAlso False Then   '�ꉞ�A����ł��Ȃ��悤�ɂ��Ă���
                            Param = GCom.GetUserID & "," & STR�w�Z�R�[�h & "," & Str_FURI_DATE & "," & STR�����N��
                            nRet = ExeRepo.ExecReport("KFGP024.EXE", Param)
                            '�߂�l�ɑΉ��������b�Z�[�W��\������
                            Select Case nRet
                                Case 0
                                    PrtCount += 1
                                Case Else
                                    '������s���b�Z�[�W
                                    MessageBox.Show(String.Format(MSG0004E, "���[�󋵈ꗗ�\(��ڕʍ��v)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return
                            End Select
                        End If
                    End If
NEXT_GAKKOU:
                    OraReader.NextRead()
                End While
            End If

            '���R�[�h�I�������ݒ�

            '���[�󋵈ꗗ�\�o��
            If PrtCount >= 1 Then
                MessageBox.Show(String.Format(MSG0014I, "���[�󋵈ꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '����ΏۂȂ����b�Z�[�W
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            txtGAKKOU_CODE.Focus()
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

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_PRTWORK_MAKE() As Boolean

        PFUNC_PRTWORK_MAKE = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            '���k�}�X�^�r���̌����@�L�[�͊w�Z�R�[�h�A�������iPRTWORK�̑n���j
            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
            '���ʒ��[�Ȃ̂ŁA���k�}�X�^�r���[����̎擾�ł͂Ȃ��A�w�Z���׃}�X�^������z���擾����
            SQL.Length = 0
            SQL.Append("select ")
            SQL.Append(" SEITOMASTVIEW.*")
            SQL.Append(",G_MEIMAST.HIMOKU_ID_M")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU1_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU2_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU3_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU4_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU5_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU6_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU7_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU8_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU9_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU10_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU11_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU12_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU13_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU14_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU15_KIN_M")
            SQL.Append(" from SEITOMASTVIEW, G_MEIMAST")
            SQL.Append(" where GAKKOU_CODE_O = " & SQ(STR�w�Z�R�[�h))
            SQL.Append(" and TUKI_NO_O = " & SQ(Mid(STR�����N��, 5, 2)))

            ' V01L01 2020/06/16 CHG FJH)AMANO PKG_2020_0012_000 -------------------------------- START
            ' �������z��0�~�̐��k���o�͂���Ȃ��̂ŁA�o�͂����悤�ɏC��
            ' �������z��0�~�̏ꍇ�A�w�Z���׃}�X�^���쐬����Ȃ����ߊO�������ɕύX����
            SQL.Append(" and GAKKOU_CODE_O = GAKKOU_CODE_M(+)")
            SQL.Append(" and NENDO_O = NENDO_M(+)")
            SQL.Append(" and TUUBAN_O = TUUBAN_M(+)")
            SQL.Append(" and TUKI_NO_O = substr(SEIKYU_TUKI_M(+), 5, 2)")
            SQL.Append(" and FURI_DATE_M(+) = " & SQ(Str_FURI_DATE))
            SQL.Append(" and FURI_KBN_M(+) IN ('0','1')")
            'SQL.Append(" and GAKKOU_CODE_O = GAKKOU_CODE_M")
            'SQL.Append(" and NENDO_O = NENDO_M")
            'SQL.Append(" and TUUBAN_O = TUUBAN_M")
            'SQL.Append(" and TUKI_NO_O = substr(SEIKYU_TUKI_M, 5, 2)")
            'SQL.Append(" and FURI_DATE_M = " & SQ(Str_FURI_DATE))
            ''2017/05/15 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
            ''�����ɓ����U�֓������݂����ꍇ�Ɉ�Ӑ���ᔽ����������
            'SQL.Append(" and FURI_KBN_M IN ('0','1')")
            ''2017/05/15 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
            ' V01L01 2020/06/16 CHG FJH)AMANO PKG_2020_0012_000 -------------------------------- END

            SQL.Append(" order by GAKKOU_CODE_O, GAKUNEN_CODE_O, CLASS_CODE_O, TUUBAN_O")

            'SQL.Append(" SELECT * FROM SEITOMASTVIEW")
            'SQL.Append(" WHERE GAKKOU_CODE_O = " & SQ(STR�w�Z�R�[�h))
            'SQL.Append(" AND TUKI_NO_O = " & SQ(Mid(STR�����N��, 5, 2)))
            'SQL.Append(" ORDER BY GAKKOU_CODE_O, GAKUNEN_CODE_O, CLASS_CODE_O")
            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("���[�N�}�X�^�̍쐬�Ɏ��s���܂����B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            While OraReader.EOF = False
                '�U�֋��z���v�v�Z
                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
                '���׃}�X�^����擾
                LNG�U�֋��z���v = OraReader.GetInt64("SEIKYU_KIN_M")

                'LNG�U�֋��z���v = OraReader.GetInt64("KINGAKU01_O") _
                '                + OraReader.GetInt64("KINGAKU02_O") _
                '                + OraReader.GetInt64("KINGAKU03_O") _
                '                + OraReader.GetInt64("KINGAKU04_O") _
                '                + OraReader.GetInt64("KINGAKU05_O") _
                '                + OraReader.GetInt64("KINGAKU06_O") _
                '                + OraReader.GetInt64("KINGAKU07_O") _
                '                + OraReader.GetInt64("KINGAKU08_O") _
                '                + OraReader.GetInt64("KINGAKU09_O") _
                '                + OraReader.GetInt64("KINGAKU10_O") _
                '                + OraReader.GetInt64("KINGAKU11_O") _
                '                + OraReader.GetInt64("KINGAKU12_O") _
                '                + OraReader.GetInt64("KINGAKU13_O") _
                '                + OraReader.GetInt64("KINGAKU14_O") _
                '                + OraReader.GetInt64("KINGAKU15_O")
                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

                '���k�}�X�^�r������ɂo�q�s�v�n�q�j���쐬
                If PFUNC_PRTWORK_SEITOINS(OraReader) = False Then
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
            '������ƃN���[�Y
            OraReader.Close()
            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

            'G_PRTWORK�̓Ǎ��݁i�w�Z�}�X�^����̊w�Z�����̎捞�݁j
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM G_PRTWORK")
            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(STR�w�Z�R�[�h))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                '�o�q�s�v�n�q�j���쐬
                If PFUNC_PRTWORK_GAKMASTINS(OraReader) = False Then
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
            '������ƃN���[�Y
            OraReader.Close()
            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

            'G_PRTWORK�̓Ǎ��݁i���׃}�X�^����̐U�֌��ʂ̎捞�݁j
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM G_PRTWORK")
            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(STR�w�Z�R�[�h))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                '�o�q�s�v�n�q�j�̍X�V
                If PFUNC_PRTWORK_MEISAIINS(OraReader) = False Then
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
            '������ƃN���[�Y
            OraReader.Close()
            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
            '���k�}�X�^�r���[������Ɏ擾���Ă��邽�ߕs�v
            ''G_PRTWORK�̓Ǎ��݁i��ڃ}�X�^����̔�ږ��̎捞�݁j
            'SQL = New StringBuilder
            'SQL.Append("SELECT * FROM G_PRTWORK")
            'SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(STR�w�Z�R�[�h))

            'If OraReader.DataReader(SQL) = False Then
            '    Exit Function
            'End If

            'While OraReader.EOF = False
            '    '�o�q�s�v�n�q�j�̍X�V
            '    If PFUNC_PRTWORK_HIMOMASTINS(OraReader) = False Then
            '        Exit Function
            '    End If
            '    OraReader.NextRead()
            'End While
            ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���擾)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_PRTWORK_MAKE = True

    End Function
    Private Function PFUNC_GAKNAME_GET2(ByVal WorkReader As MyOracleReader) As Boolean

        '�w�Z���̐ݒ�
        PFUNC_GAKNAME_GET2 = False

        STR�w�Z���J�i = ""
        STR�w�Z������ = ""
        STR�w�N������ = ""
        STR�N���X�� = ""

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST1")
            SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" AND GAKUNEN_CODE_G  = " & WorkReader.GetString("GAKUNEN_CODE_P"))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                With OraReader
                    STR�w�Z���J�i = .GetString("GAKKOU_KNAME_G")
                    STR�w�Z������ = .GetString("GAKKOU_NNAME_G")
                    STR�w�N������ = .GetString("GAKUNEN_NAME_G")
                    '�N���X�R�[�h����N���X���擾
                    For No As Integer = 1 To 20
                        If WorkReader.GetString("CLASS_CODE_P") = .GetString("CLASS_CODE1" & No.ToString("00") & "_G") Then
                            STR�N���X�� = .GetString("CLASS_NAME1" & No.ToString("00") & "_G")
                            Exit For
                        End If
                    Next
                End With
                OraReader.NextRead()
            End While
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�s�}��)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_GAKNAME_GET2 = True
    End Function
    Private Function PFUNC_PRTWORK_MEISAIINS(ByVal WorkReader As MyOracleReader) As Boolean

        PFUNC_PRTWORK_MEISAIINS = False

        '�U�֌��ʂ���PRTWORK�ɐݒ肷��
        '���׃}�X�^����
        '        Call PFUNC_MEISAI_GET()

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)

            SQL.Append("SELECT * FROM KZFMAST.G_MEIMAST WHERE")
            SQL.Append(" GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" AND NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" AND FURI_DATE_M <= " & SQ(WorkReader.GetString("FURI_DATE_P")))
            SQL.Append(" AND TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)

            ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- START
            ' ���ID�̕ύX�l���s���̂��ߍ폜
            'SQL.Append(" AND HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
            ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- END

            ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- START
            ' �O�N�x���̊w�Z���׃}�X�^������ꍇ�ɁA�O�N�x���̐U�֌��ʂ��o�͂���鎞������̂�
            ' �Ώ۔N�x�̃X�P�W���[���ōi��悤�ɏC��
            ' �܂��A�U�֋敪�̍l�������݂��Ȃ����ߏC���i���������A�����o���̍X�V�����Ă��܂��ꍇ����j
            Select Case Mid(STR�����N��, 5, 2)
                Case "01", "02", "03"
                    SQL.Append(" AND SEIKYU_TUKI_M  BETWEEN '" & (CInt(Mid(STR�����N��, 1, 4)) - 1).ToString & "04' AND '" & Mid(STR�����N��, 1, 4) & "03'")
                Case Else
                    SQL.Append(" AND SEIKYU_TUKI_M  BETWEEN '" & Mid(STR�����N��, 1, 4) & "04' AND '" & (CInt(Mid(STR�����N��, 1, 4)) + 1).ToString & "03'")
            End Select
            SQL.Append(" AND FURI_KBN_M IN ('0','1')")
            ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- END

            If OraReader.DataReader(SQL) = False Then
                Return True
            End If

            While OraReader.EOF = False
                With OraReader

                    STR�U�֌��� = .GetInt64("FURIKETU_CODE_M")
                    '2006/10/12 �ĐU�������͎��z���ŏ����ςɂȂ��������肭��ǉ�
                    ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- START
                    'If STR�U�֌��� <> "0" Then
                    '    Dim DataReader As MyOracleReader = New MyOracleReader(MainDB)
                    '    SQL = New StringBuilder
                    '    SQL.Append("SELECT * FROM KZFMAST.G_MEIMAST")
                    '    SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
                    '    SQL.Append(" AND NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
                    '    SQL.Append(" AND TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)
                    '    SQL.Append(" AND HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
                    '    SQL.Append(" AND SEIKYU_TUKI_M = " & SQ(OraReader.GetString("SEIKYU_TUKI_M")))
                    '    SQL.Append(" AND FURIKETU_CODE_M = '0' ")

                    '    If DataReader.DataReader(SQL) = False Then
                    '    Else
                    '        STR�U�֌��� = DataReader.GetInt("FURIKETU_CODE_M")
                    '    End If
                    '    DataReader.Close()
                    'End If
                    If SFuriCode.IndexOf(STR�U�֌���) >= 0 Then
                        Dim DataReader As MyOracleReader = New MyOracleReader(MainDB)
                        SQL = New StringBuilder
                        SQL.Append("SELECT * FROM KZFMAST.G_MEIMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
                        SQL.Append(" AND NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
                        SQL.Append(" AND TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)

                        ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- START
                        ' ���ID�̕ύX�l���s���̂��ߍ폜
                        'SQL.Append(" AND HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
                        ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- END

                        ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- START
                        ' ���z�������ɐ���ƂȂ����ꍇ�ɁA�ߋ����ň�������ۂɖ����̎��z���핪�ŏ㏑������Ă��܂��̂ŁA
                        ' ���z�������o����Ȃ��悤�ɁA���o�����ɉ�ʓ��͂����U�֓���ǉ�����
                        ' �܂��A�U�֋敪�̍l�������݂��Ȃ����ߏC���i���������A�����o���̍X�V�����Ă��܂��ꍇ����j
                        SQL.Append(" AND FURI_DATE_M <= " & SQ(Str_FURI_DATE))
                        SQL.Append(" AND FURI_KBN_M IN ('0','1')")
                        ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- END

                        SQL.Append(" AND SEIKYU_TUKI_M = " & SQ(OraReader.GetString("SEIKYU_TUKI_M")))
                        SQL.Append(" AND FURIKETU_CODE_M = '0' ")

                        If DataReader.DataReader(SQL) = False Then
                        Else
                            STR�U�֌��� = DataReader.GetInt("FURIKETU_CODE_M")
                        End If
                        DataReader.Close()
                    End If
                    ' 2017/06/06 �^�X�N�j���� CHG �yOT�z(�ĐU�ΏۃR�[�hIni�t�@�C����) -------------------- END

                    SQL = New StringBuilder
                    SQL.Append("UPDATE  KZFMAST.G_PRTWORK SET ")
                    '  '�U�֌��ʁi�������j
                    SQL.Append(" KEKKA_MM" & Mid(.GetString("SEIKYU_TUKI_M"), 5, 2) & "_P = " & SQ(STR�U�֌���))
                    SQL.Append((" WHERE GAKKOU_CODE_P  = " & SQ(WorkReader.GetString("GAKKOU_CODE_P"))))
                    SQL.Append((" And NENDO_P = " & SQ(WorkReader.GetString("NENDO_P"))))
                    SQL.Append((" And TUUBAN_P = " & WorkReader.GetString("TUUBAN_P")))

                    MainDB.ExecuteNonQuery(SQL)
                End With
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�s�}��)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_PRTWORK_MEISAIINS = True

    End Function
    Private Function PFUNC_MEISAI_GET(ByVal WorkReader As MyOracleReader) As Boolean

        '�U�֌��ʂ̎擾
        PFUNC_MEISAI_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            STR�U�֌��� = ""

            SQL.Append("Select * FROM KZFMAST.G_MEIMAST")
            SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" And FURI_DATE_M <= " & SQ(WorkReader.GetString("FURI_DATE_P")))
            SQL.Append(" And TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)
            '        SQL.Append(" And SEIKYU_TUKI_M = " & SQ(STR�����N��))
            SQL.Append(" And HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                STR�U�֌��� = OraReader.GetInt("FURIKETU_CODE_M").ToString
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�֌��ʎ擾)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_MEISAI_GET = True

    End Function
    Private Function PFUNC_PRTWORK_HIMOMASTINS(ByVal WorkReader As MyOracleReader) As Boolean

        '��ږ��̐ݒ�
        PFUNC_PRTWORK_HIMOMASTINS = False
        Dim SQL As New StringBuilder
        Try
            '��ڃ}�X�^�̌���
            Call PFUNC_HIMOMAST_GET(WorkReader)

            SQL.Append("UPDATE  KZFMAST.G_PRTWORK Set ")
            '��ږ��P
            SQL.Append(" HIMOKU_NAME01_P = " & SQ(STR��ږ��P))
            '��ږ��Q
            SQL.Append(",HIMOKU_NAME02_P = " & SQ(STR��ږ��Q))
            '��ږ��R
            SQL.Append(",HIMOKU_NAME03_P = " & SQ(STR��ږ��R))
            '��ږ��S
            SQL.Append(",HIMOKU_NAME04_P = " & SQ(STR��ږ��S))
            '��ږ��T
            SQL.Append(",HIMOKU_NAME05_P = " & SQ(STR��ږ��T))
            '��ږ��U
            SQL.Append(",HIMOKU_NAME06_P = " & SQ(STR��ږ��U))
            '��ږ��V
            SQL.Append(",HIMOKU_NAME07_P = " & SQ(STR��ږ��V))
            '��ږ��W
            SQL.Append(",HIMOKU_NAME08_P = " & SQ(STR��ږ��W))
            '��ږ��X
            SQL.Append(",HIMOKU_NAME09_P = " & SQ(STR��ږ��X))
            '��ږ��P�O
            SQL.Append(",HIMOKU_NAME10_P = " & SQ(STR��ږ��P�O))
            '��ږ��P�P
            SQL.Append(",HIMOKU_NAME11_P = " & SQ(STR��ږ��P�P))
            '��ږ��P�Q
            SQL.Append(",HIMOKU_NAME12_P = " & SQ(STR��ږ��P�Q))
            '��ږ��P�R
            SQL.Append(",HIMOKU_NAME13_P = " & SQ(STR��ږ��P�R))
            '��ږ��P�S
            SQL.Append(",HIMOKU_NAME14_P = " & SQ(STR��ږ��P�S))
            '��ږ��P�T
            SQL.Append(",HIMOKU_NAME15_P = " & SQ(STR��ږ��P�T))

            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And NENDO_P = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" And TUUBAN_P = " & WorkReader.GetInt("TUUBAN_P").ToString)

            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�֌��ʎ擾)", "���s", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_HIMOMASTINS = True

    End Function
    Private Function PFUNC_HIMOMAST_GET(ByVal WorkReader As MyOracleReader) As Boolean

        '��ږ��̎擾
        PFUNC_HIMOMAST_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '��ږ��̏�����
            STR��ږ��P = ""
            STR��ږ��Q = ""
            STR��ږ��R = ""
            STR��ږ��S = ""
            STR��ږ��T = ""
            STR��ږ��U = ""
            STR��ږ��V = ""
            STR��ږ��W = ""
            STR��ږ��X = ""
            STR��ږ��P�O = ""
            STR��ږ��P�P = ""
            STR��ږ��P�Q = ""
            STR��ږ��P�R = ""
            STR��ږ��P�S = ""
            STR��ږ��P�T = ""

            SQL.Append("Select * FROM KZFMAST.HIMOMAST")
            SQL.Append(" WHERE GAKKOU_CODE_H = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And GAKUNEN_CODE_H = " & WorkReader.GetString("GAKUNEN_CODE_P"))
            SQL.Append(" And HIMOKU_ID_H = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
            SQL.Append(" And TUKI_NO_H = " & SQ(Mid(STR�����N��, 5, 2)))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                STR��ږ��P = OraReader.GetString("HIMOKU_NAME01_H")
                STR��ږ��Q = OraReader.GetString("HIMOKU_NAME02_H")
                STR��ږ��R = OraReader.GetString("HIMOKU_NAME03_H")
                STR��ږ��S = OraReader.GetString("HIMOKU_NAME04_H")
                STR��ږ��T = OraReader.GetString("HIMOKU_NAME05_H")
                STR��ږ��U = OraReader.GetString("HIMOKU_NAME06_H")
                STR��ږ��V = OraReader.GetString("HIMOKU_NAME07_H")
                STR��ږ��W = OraReader.GetString("HIMOKU_NAME08_H")
                STR��ږ��X = OraReader.GetString("HIMOKU_NAME09_H")
                STR��ږ��P�O = OraReader.GetString("HIMOKU_NAME10_H")
                STR��ږ��P�P = OraReader.GetString("HIMOKU_NAME11_H")
                STR��ږ��P�Q = OraReader.GetString("HIMOKU_NAME12_H")
                STR��ږ��P�R = OraReader.GetString("HIMOKU_NAME13_H")
                STR��ږ��P�S = OraReader.GetString("HIMOKU_NAME14_H")
                STR��ږ��P�T = OraReader.GetString("HIMOKU_NAME15_H")
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�U�֌��ʎ擾)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_HIMOMAST_GET = True

    End Function
    Private Function PSUB_GAK_SORT() As Boolean
        PSUB_GAK_SORT = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" Select MEISAI_OUT_T FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(STR�w�Z�R�[�h))

            If OraReader.DataReader(SQL) = False Then
                STR�\�[�g�� = "0"
            Else
                STR�\�[�g�� = OraReader.GetString("MEISAI_OUT_T")
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�\�[�g���擾)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        Return True
    End Function
    Private Function PFUNC_PRTWORK_SEITOINS(ByVal OraReader As MyOracleReader) As Boolean

        PFUNC_PRTWORK_SEITOINS = False
        Dim SQL As New StringBuilder
        Try
            With OraReader
                SQL.Append("INSERT INTO KZFMAST.G_PRTWORK ")
                SQL.Append(" (GAKKOU_CODE_P ")
                SQL.Append(", NENDO_P ")
                SQL.Append(", TUUBAN_P ")
                SQL.Append(", FURI_DATE_P ")
                SQL.Append(", GAKKOU_KNAME_P ")
                SQL.Append(", GAKKOU_NNAME_P ")
                SQL.Append(", SEIBETU_P ")
                SQL.Append(", GAKUNEN_CODE_P ")
                SQL.Append(", GAKUNEN_NAME_P ")
                SQL.Append(", CLASS_CODE_P ")
                SQL.Append(", CLASS_NAME_P ")
                SQL.Append(", SEITO_NO_P ")
                SQL.Append(", SEITO_KNAME_P ")
                SQL.Append(", SEITO_NNAME_P ")
                SQL.Append(", TKIN_NO_P ")
                SQL.Append(", TSIT_NO_P ")
                SQL.Append(", KAMOKU_P ")
                SQL.Append(", KOUZA_P ")
                SQL.Append(", MEIGI_KNAME_P ")
                SQL.Append(", HIMOKU_ID_P ")
                SQL.Append(", KINGAKU_P ")
                SQL.Append(", KINGAKU01_P ")
                SQL.Append(", KINGAKU02_P ")
                SQL.Append(", KINGAKU03_P ")
                SQL.Append(", KINGAKU04_P ")
                SQL.Append(", KINGAKU05_P ")
                SQL.Append(", KINGAKU06_P ")
                SQL.Append(", KINGAKU07_P ")
                SQL.Append(", KINGAKU08_P ")
                SQL.Append(", KINGAKU09_P ")
                SQL.Append(", KINGAKU10_P ")
                SQL.Append(", KINGAKU11_P ")
                SQL.Append(", KINGAKU12_P ")
                SQL.Append(", KINGAKU13_P ")
                SQL.Append(", KINGAKU14_P ")
                SQL.Append(", KINGAKU15_P ")
                SQL.Append(", FUNOU_RIYUU_P ")
                SQL.Append(", KEKKA_MM04_P ")
                SQL.Append(", KEKKA_MM05_P ")
                SQL.Append(", KEKKA_MM06_P ")
                SQL.Append(", KEKKA_MM07_P ")
                SQL.Append(", KEKKA_MM08_P ")
                SQL.Append(", KEKKA_MM09_P ")
                SQL.Append(", KEKKA_MM10_P ")
                SQL.Append(", KEKKA_MM11_P ")
                SQL.Append(", KEKKA_MM12_P ")
                SQL.Append(", KEKKA_MM01_P ")
                SQL.Append(", KEKKA_MM02_P ")
                SQL.Append(", KEKKA_MM03_P ")
                SQL.Append(", HIMOKU_NAME01_P ")
                SQL.Append(", HIMOKU_NAME02_P ")
                SQL.Append(", HIMOKU_NAME03_P ")
                SQL.Append(", HIMOKU_NAME04_P ")
                SQL.Append(", HIMOKU_NAME05_P ")
                SQL.Append(", HIMOKU_NAME06_P ")
                SQL.Append(", HIMOKU_NAME07_P ")
                SQL.Append(", HIMOKU_NAME08_P ")
                SQL.Append(", HIMOKU_NAME09_P ")
                SQL.Append(", HIMOKU_NAME10_P ")
                SQL.Append(", HIMOKU_NAME11_P ")
                SQL.Append(", HIMOKU_NAME12_P ")
                SQL.Append(", HIMOKU_NAME13_P ")
                SQL.Append(", HIMOKU_NAME14_P ")
                SQL.Append(", HIMOKU_NAME15_P )")
                SQL.Append(" VALUES ( ")
                '�w�Z�R�[�h
                SQL.Append(SQ(.GetString("GAKKOU_CODE_O")))
                '���w�N�x
                SQL.Append("," & SQ(.GetString("NENDO_O")))
                '�ʔ�
                SQL.Append("," & GCom.NzInt(.GetString("TUUBAN_O")))
                '�U�֓�
                SQL.Append("," & SQ(Str_FURI_DATE))
                '�w�Z���i�J�i�j
                SQL.Append("," & SQ(Space(40)))
                '�w�Z���i�����j
                SQL.Append("," & SQ(Space(50)))
                '����
                SQL.Append("," & SQ(.GetString("SEIBETU_O")))
                '�w�N�R�[�h
                SQL.Append("," & GCom.NzInt(.GetString("GAKUNEN_CODE_O")))
                '�w�N����
                SQL.Append("," & SQ(Space(20)))
                '�N���X�R�[�h
                SQL.Append("," & GCom.NzInt(.GetString("CLASS_CODE_O")))
                '�N���X��
                SQL.Append("," & SQ(Space(20)))
                '���k�ԍ�
                SQL.Append("," & SQ(.GetString("SEITO_NO_O")))
                '���k���i�J�i�j
                SQL.Append("," & SQ(.GetString("SEITO_KNAME_O")))
                '���k���i�����j
                SQL.Append("," & SQ(.GetString("SEITO_NNAME_O")))
                '�戵���Z�@��
                SQL.Append("," & SQ(.GetString("TKIN_NO_O")))
                '�戵�x�X�R�[�h
                SQL.Append("," & SQ(.GetString("TSIT_NO_O")))
                '�Ȗ�
                SQL.Append("," & SQ(.GetString("KAMOKU_O")))
                '�����ԍ�
                SQL.Append("," & SQ(.GetString("KOUZA_O")))
                '�������`�l�J�i
                SQL.Append("," & SQ(.GetString("MEIGI_KNAME_O")))
                '��ڂh�c
                SQL.Append("," & SQ(.GetString("HIMOKU_ID_O")))
                '�U�֋��z�i���v�j
                SQL.Append("," & LNG�U�֋��z���v)
                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
                '���ʒ��[�Ȃ̂ŁA���k�}�X�^�r���[����̎擾�ł͂Ȃ��A�w�Z���׃}�X�^������z���擾����
                '�U�֋��z�i��ڂP�j
                SQL.Append("," & .GetInt64("HIMOKU1_KIN_M"))
                '�U�֋��z�i��ڂQ�j
                SQL.Append("," & .GetInt64("HIMOKU2_KIN_M"))
                '�U�֋��z�i��ڂR�j
                SQL.Append("," & .GetInt64("HIMOKU3_KIN_M"))
                '�U�֋��z�i��ڂS�j
                SQL.Append("," & .GetInt64("HIMOKU4_KIN_M"))
                '�U�֋��z�i��ڂT�j
                SQL.Append("," & .GetInt64("HIMOKU5_KIN_M"))
                '�U�֋��z�i��ڂU�j
                SQL.Append("," & .GetInt64("HIMOKU6_KIN_M"))
                '�U�֋��z�i��ڂV�j
                SQL.Append("," & .GetInt64("HIMOKU7_KIN_M"))
                '�U�֋��z�i��ڂW�j
                SQL.Append("," & .GetInt64("HIMOKU8_KIN_M"))
                '�U�֋��z�i��ڂX�j
                SQL.Append("," & .GetInt64("HIMOKU9_KIN_M"))
                '�U�֋��z�i��ڂP�O�j
                SQL.Append("," & .GetInt64("HIMOKU10_KIN_M"))
                '�U�֋��z�i��ڂP�P�j
                SQL.Append("," & .GetInt64("HIMOKU11_KIN_M"))
                '�U�֋��z�i��ڂP�Q�j
                SQL.Append("," & .GetInt64("HIMOKU12_KIN_M"))
                '�U�֋��z�i��ڂP�R�j
                SQL.Append("," & .GetInt64("HIMOKU13_KIN_M"))
                '�U�֋��z�i��ڂP�S�j
                SQL.Append("," & .GetInt64("HIMOKU14_KIN_M"))
                '�U�֋��z�i��ڂP�T�j
                SQL.Append("," & .GetInt64("HIMOKU15_KIN_M"))

                ''�U�֋��z�i��ڂP�j
                'SQL.Append("," & .GetInt64("KINGAKU01_O"))
                ''�U�֋��z�i��ڂQ�j
                'SQL.Append("," & .GetInt64("KINGAKU02_O"))
                ''�U�֋��z�i��ڂR�j
                'SQL.Append("," & .GetInt64("KINGAKU03_O"))
                ''�U�֋��z�i��ڂS�j
                'SQL.Append("," & .GetInt64("KINGAKU04_O"))
                ''�U�֋��z�i��ڂT�j
                'SQL.Append("," & .GetInt64("KINGAKU05_O"))
                ''�U�֋��z�i��ڂU�j
                'SQL.Append("," & .GetInt64("KINGAKU06_O"))
                ''�U�֋��z�i��ڂV�j
                'SQL.Append("," & .GetInt64("KINGAKU07_O"))
                ''�U�֋��z�i��ڂW�j
                'SQL.Append("," & .GetInt64("KINGAKU08_O"))
                ''�U�֋��z�i��ڂX�j
                'SQL.Append("," & .GetInt64("KINGAKU09_O"))
                ''�U�֋��z�i��ڂP�O�j
                'SQL.Append("," & .GetInt64("KINGAKU10_O"))
                ''�U�֋��z�i��ڂP�P�j
                'SQL.Append("," & .GetInt64("KINGAKU11_O"))
                ''�U�֋��z�i��ڂP�Q�j
                'SQL.Append("," & .GetInt64("KINGAKU12_O"))
                ''�U�֋��z�i��ڂP�R�j
                'SQL.Append("," & .GetInt64("KINGAKU13_O"))
                ''�U�֋��z�i��ڂP�S�j
                'SQL.Append("," & .GetInt64("KINGAKU14_O"))
                ''�U�֋��z�i��ڂP�T�j
                'SQL.Append("," & .GetInt64("KINGAKU15_O"))
                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

                '�s�\���R 
                SQL.Append("," & SQ(Space(10)))
                '�U�֌��ʁi�S���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�T���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�U���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�V���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�W���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�X���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�P�O���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�P�P���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�P�Q���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�P���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�Q���j
                SQL.Append("," & SQ(Space(1)))
                '�U�֌��ʁi�R���j
                SQL.Append("," & SQ(Space(1)))

                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
                '���k�}�X�^�r���[�Ɋ��Ɏ����Ă��邽�߁A�ݒ肵�Ă��܂��B
                '��ږ��P
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME01_O")))
                '��ږ��Q
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME02_O")))
                '��ږ��R
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME03_O")))
                '��ږ��S
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME04_O")))
                '��ږ��T
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME05_O")))
                '��ږ��U
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME06_O")))
                '��ږ��V
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME07_O")))
                '��ږ��W
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME08_O")))
                '��ږ��X
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME09_O")))
                '��ږ��P�O
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME10_O")))
                '��ږ��P�P
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME11_O")))
                '��ږ��P�Q
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME12_O")))
                '��ږ��P�R
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME13_O")))
                '��ږ��P�S
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME14_O")))
                '��ږ��P�T
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME15_O")) & ")")

                ''��ږ��P
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��Q
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��R
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��S
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��T
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��U
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��V
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��W
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��X
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��P�O
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��P�P
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��P�Q
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��P�R
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��P�S
                'SQL.Append("," & SQ(Space(20)))
                ''��ږ��P�T
                'SQL.Append("," & SQ(Space(20)) & ")")
                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END
            End With

            MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�s�}��)", "���s", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_SEITOINS = True

    End Function
    Private Function PFUNC_PRTWORK_GAKMASTINS(ByVal WorkReader As MyOracleReader) As Boolean
        '�w�Z���A�w�N���A�N���X����PRTWORK�ɐݒ肷��

        PFUNC_PRTWORK_GAKMASTINS = False
        Dim SQL As New StringBuilder
        Dim Ret As Integer
        Try
            '�w�Z�}�X�^�P����
            Call PFUNC_GAKNAME_GET2(WorkReader)

            SQL.Append("UPDATE  KZFMAST.G_PRTWORK Set ")
            '�w�Z���i�J�i�j
            SQL.Append(" GAKKOU_KNAME_P = " & SQ(STR�w�Z���J�i))
            '�w�Z���i�����j
            SQL.Append(",GAKKOU_NNAME_P = " & SQ(STR�w�Z������))
            '�w�N����
            SQL.Append(",GAKUNEN_NAME_P = " & SQ(STR�w�N������))
            '�N���X��
            SQL.Append(",CLASS_NAME_P = " & SQ(STR�N���X��))

            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And NENDO_P = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" And TUUBAN_P = " & WorkReader.GetString("TUUBAN_P"))

            Ret = MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�w�Z�X�V)", "���s", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_GAKMASTINS = True

    End Function
    Private Function PFUNC_PRTWORK_DEL() As Boolean
        Try
            PFUNC_PRTWORK_DEL = False

            Dim SQL As New StringBuilder
            SQL.Append("DELETE FROM G_PRTWORK")

            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�N�}�X�^�폜)", "���s", ex.ToString)
            Return False
        End Try
        PFUNC_PRTWORK_DEL = True
    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False

        Dim OraReader As MyOracleReader = Nothing
        Dim OraReader2 As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            '�X�P�W���[���}�X�^�̌���
            '�w�Z�R�[�h�A�U�֓��A�U�֋敪�i���U�j

            SQL.Append(" Select * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR�w�Z�R�[�h))

            SQL.Append(" And FURI_DATE_S = " & SQ(Str_FURI_DATE))

            'SQL.Append(" And FURI_KBN_S = '0'")

            '�X�P�W���[�������݃`�F�b�N
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            OraReader2 = New MyOracleReader(MainDB)
            While OraReader.EOF = False
                STR�����N�� = OraReader.GetString("NENGETUDO_S")
                SQL = New StringBuilder
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR�w�Z�R�[�h))
                SQL.Append(" AND FURI_DATE_S > " & SQ(Str_FURI_DATE))
                SQL.Append(" AND NENGETUDO_S = " & SQ(STR�����N��))
                SQL.Append(" AND CHECK_FLG_S = '1'")
                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
                '���U�ƍĐU�̊Ԃɐ������܂܂��ƈ���ł��Ȃ����ߏ����ǉ�
                SQL.Append(" AND FURI_KBN_S IN ('0', '1')")
                SQL.Append(" AND SCH_KBN_S <> '2'")
                ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END
                '�X�P�W���[�������݃`�F�b�N
                If OraReader2.DataReader(SQL) Then
                    If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                        MessageBox.Show("���̃X�P�W���[���͍ĐU�����ɐi��ł��܂�" & vbCrLf & "�ߋ��̏󋵂��o�͂��邱�Ƃ͂ł��܂���", _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    Exit Function
                End If
                OraReader.NextRead()
            End While
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���擾)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraReader2 IsNot Nothing Then OraReader2.Close()
        End Try
        PFUNC_SCHMAST_GET = True

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
            Str_GAKKOU_CODE = txtGAKKOU_CODE.Text.Trim
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

            Str_FURI_DATE = Trim(txtFuriDateY.Text) & Trim(txtFuriDateM.Text) & Trim(txtFuriDateD.Text)

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


            '�X�P�W���[�����s�\���ʍX�V�ς݂�����ǉ�
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM G_SCHMAST")
            If Trim(Str_GAKKOU_CODE) = "9999999999" Then
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(Str_FURI_DATE))
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            Else
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Trim(Str_GAKKOU_CODE)))
                SQL.Append(" AND FURI_DATE_S = " & SQ(Str_FURI_DATE))
            End If
            STR_SQL += " AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')"

            '���ޭ�ً敪2(����)�͏����Ɋ܂܂Ȃ���
            STR_SQL += " AND SCH_KBN_S <> '2'"

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("�X�P�W���[�������݂��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            If OraReader.GetString("FUNOU_FLG_S") = "0" Then
                MessageBox.Show("���̃X�P�W���[���͕s�\���ʍX�V�������������ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '������[�Ƀ`�F�b�N�����Ă��Ȃ�������G���[
            If chk1.Checked = False AndAlso chk2.Checked = False Then
                MessageBox.Show("����Ώے��[���w�肳��Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
    Private Function PFUNC_GAKNAME_GET() As Boolean
        '�w�Z���̐ݒ�
        PFUNC_GAKNAME_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As MyOracle = Nothing
        Try
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab�w�Z��.Text = ""
            Else
                If OraDB Is Nothing Then OraDB = New MyOracle
                OraReader = New MyOracleReader(OraDB)
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G =" & SQ(txtGAKKOU_CODE.Text))

                '�w�Z�}�X�^�P���݃`�F�b�N
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z�}�X�^����)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function

#End Region

#Region " �C�x���g"
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '�w�Z�R�[�h
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '���S�M�������@ALL9�w�莞�A�u���[�󋵈ꗗ�\(��ڕʍ��v)�v�̃`�F�b�N�{�b�N�X�͓��͕s�Ƃ��� 2007/08/16
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                chk2.Checked = False
                chk2.Enabled = False
                lab�w�Z��.Text = ""
            Else
                ' 2017/05/26 �^�X�N�j���� DEL �yME�z(RSV2�Ή� �����`�F�b�N�@�\�폜) -------------------- START
                'chk2.Checked = True
                ' 2017/05/26 �^�X�N�j���� DEL �yME�z(RSV2�Ή� �����`�F�b�N�@�\�폜) -------------------- END
                chk2.Enabled = True

                '�w�Z���̎擾
                If PFUNC_GAKNAME_GET() <> False Then
                    Exit Sub
                End If
            End If

        End If
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
        'PFUC_GAKNAME_GET()

        '�w�Z�R�[�h�ɃJ�[�\���ݒ�
        txtGAKKOU_CODE.Focus()

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

End Class
