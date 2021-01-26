Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN030

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN030", "�������σ��G���^���ʍX�V���")
    Private Const msgtitle As String = "�������σ��G���^���ʍX�V���(KFKMAIN030)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    '2018/01/23 saitou �L���M��(RSV2�W��) ADD �������σ��G���^���ʍX�V�Ή� -------------------- START
    Private Structure strcIni
        Dim RIENTA_PATH As String
        Dim RIENTA_FILENAME As String
        Dim RSV2_EDITION As String
        Dim COMMON_BAITAIREAD As String
    End Structure
    Private ini_info As strcIni
    '2018/01/23 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END

    '���LOAD��
    Private Sub KFKMAIN030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '------------------------------------------------
            '�V�X�e�����t�ƃ��[�U����\��
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------------
            '��ʕ\�����A�������ɃV�X�e�����t��\��
            '------------------------------------------------
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)
            cmbTimeStamp.Enabled = False
            btnReset.Enabled = False
            '------------------------------------------------
            '���s�{�^�����\���ݒ�
            '------------------------------------------------
            btnAction.Enabled = False

            '2018/01/23 saitou �L���M��(RSV2�W��) ADD �������σ��G���^���ʍX�V�Ή� -------------------- START
            '------------------------------------------------
            '�ݒ�t�@�C���擾
            '------------------------------------------------
            If Me.SetIniFIle() = False Then
                Return
            End If
            '2018/01/23 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    '�����{�^��������
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)�J�n", "����", "")

            '------------------------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '------------------------------------------------
            If fn_check_TEXT() = False Then Exit Sub

            '------------------------------------------------
            '�X�P�W���[���}�X�^��������
            '------------------------------------------------
            If fn_select_SCHMAST(0) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "����"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)�I��", "����", "")
        End Try

    End Sub

    '���s�{�^��������
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New CASTCommon.MyOracle

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")
            '------------------------------------------------
            '�^�C���X�^���v�̎擾
            '------------------------------------------------
            Dim strTIME_STAMP_W As String
            strTIME_STAMP_W = cmbTimeStamp.SelectedItem
            strTIME_STAMP_W = strTIME_STAMP_W.Substring(0, 4) & strTIME_STAMP_W.Substring(5, 2) & strTIME_STAMP_W.Substring(8, 2) _
                            & strTIME_STAMP_W.Substring(11, 2) & strTIME_STAMP_W.Substring(14, 2) & strTIME_STAMP_W.Substring(17, 2)

            If MessageBox.Show(MSG0023I.Replace("{0}", "�������σ��G���^���ʍX�V"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2018/01/23 saitou �L���M��(RSV2�W��) ADD �������σ��G���^���ʍX�V�Ή� -------------------- START
            Dim ret As Integer = 0
            If ini_info.RSV2_EDITION = "2" Then
                '��K�͔ł̓o�b�`�o�^�O�ɔ}�̂��烊�G���^���R�s�[����
                Do
                    Try
                        Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                        Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories

                        ret = 0
                        Exit Do

                    Catch ex As Exception
                        If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> Windows.Forms.DialogResult.OK Then
                            ret = -1
                            Exit Do
                        End If
                    End Try
                Loop

                If ret = 0 Then
                    File.Copy(Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME), Path.Combine(ini_info.COMMON_BAITAIREAD, ini_info.RIENTA_FILENAME), True)
                End If
            End If

            If ret <> 0 Then
                Return
            End If
            '2018/01/23 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END

            MainDB.BeginTrans()     '�g�����U�N�V�����J�n

            Dim jobid As String
            Dim para As String

            '�W���u�}�X�^�ɓo�^
            jobid = "K020"                      '..\Batch\�������σf�[�^�쐬\
            para = strTIME_STAMP_W                  '���ϓ����p�����^�Ƃ��Đݒ�

            '#########################
            'job����
            '#########################
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                Throw New Exception(MSG0002E.Replace("{0}", "����"))
            End If

            '#########################
            'job�o�^
            '#########################
            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                Throw New Exception(MSG0005E)
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "�������σ��G���^���ʍX�V"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            cmbTimeStamp.Items.Clear()

            btnSearch.Enabled = True
            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False
            btnAction.Enabled = False
            txtSyoriDateY.Focus()

            Return

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try

    End Sub

    '�I���{�^��������
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False

            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True

            '�V�X�e�����t���ĕ\��
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            cmbTimeStamp.Items.Clear()
            txtSyoriDateY.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub

#Region "KFKMAIN030�p�֐�"

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :�����{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        Try

            '------------------------------------------------
            '�����N�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtSyoriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '�������`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtSyoriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtSyoriDateM.Text >= 1 And txtSyoriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '�������`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtSyoriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '�����`�F�b�N
            If Not (txtSyoriDateD.Text >= 1 And txtSyoriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
        End Try

        Return True
    End Function

    Private Function fn_select_SCHMAST(ByVal aintFLG As Integer) As Boolean
        '============================================================================
        'NAME           :fn_select_SCHMAST
        'Parameter      :aintFLG=0:�����{�^���������@1:��ʍĕ\����
        'Description    :SCHMAST���猈�σ^�C���X�^���v�擾
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        Dim strSyoriDate As String
        Dim strJOKEN1 As String '���o����1
        Dim strJOKEN2 As String '���o����2
        Dim strTIME_STAMP As String

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            '�������̎擾
            strSyoriDate = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text
            strJOKEN1 = strSyoriDate & "000000"
            strJOKEN2 = strSyoriDate & "999999"

            '------------------------------------------------
            '�X�P�W���[���}�X�^�̌�������
            '------------------------------------------------
            SQL.Append("SELECT")
            SQL.Append(" TIME_STUMP")
            SQL.Append(" FROM")
            SQL.Append(" ((SELECT")
            SQL.Append("   KESSAI_TIME_STAMP_S AS TIME_STUMP")
            SQL.Append("  FROM")
            SQL.Append("   SCHMAST")
            SQL.Append("  ,KESSAIMAST")
            SQL.Append("  WHERE")
            SQL.Append("       KESSAI_TIME_STAMP_S = TIME_STAMP_KR")
            SQL.Append("   AND KESSAI_TIME_STAMP_S BETWEEN '" & strJOKEN1 & "' AND '" & strJOKEN2 & "'")
            SQL.Append("   AND KESSAI_FLG_S = '1')")
            SQL.Append("  UNION ")
            SQL.Append(" (SELECT")
            SQL.Append("   TESUU_TIME_STAMP_S AS TIME_STUMP")
            SQL.Append("  FROM")
            SQL.Append("   SCHMAST")
            SQL.Append("  ,KESSAIMAST")
            SQL.Append("  WHERE")
            SQL.Append("       TESUU_TIME_STAMP_S = TIME_STAMP_KR")
            SQL.Append("   AND TESUU_TIME_STAMP_S BETWEEN '" & strJOKEN1 & "' AND '" & strJOKEN2 & "'")
            SQL.Append("   AND TESUUTYO_FLG_S = '1'))")
            SQL.Append(" GROUP BY TIME_STUMP")
            SQL.Append(" ORDER BY TIME_STUMP")

            '�R���{�{�b�N�X�̏�����
            cmbTimeStamp.Items.Clear()

            '�ĕ`�悵�Ȃ��悤�ɂ���
            cmbTimeStamp.BeginUpdate()

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    '------------------------------------------------
                    '�R���{�{�b�N�X�Ƀ��X�g��ǉ�����
                    '------------------------------------------------
                    strTIME_STAMP = GCom.NzStr(OraReader.GetString("TIME_STUMP"))

                    strTIME_STAMP = strTIME_STAMP.Substring(0, 4) & "/" & strTIME_STAMP.Substring(4, 2) & "/" & strTIME_STAMP.Substring(6, 2) & Space(1) _
                                    & strTIME_STAMP.Substring(8, 2) & ":" & strTIME_STAMP.Substring(10, 2) & ":" & strTIME_STAMP.Substring(12, 2)

                    cmbTimeStamp.Items.Add(strTIME_STAMP)
                    OraReader.NextRead()
                Loop
            End If

            '�ĕ`�悷��悤�ɂ���
            cmbTimeStamp.EndUpdate()

            '�Y���������[�����̏ꍇ
            If cmbTimeStamp.Items.Count = 0 Then
                '�����{�^���������̂݃��b�Z�[�W�o��
                If aintFLG = 0 Then
                    MessageBox.Show(MSG0055W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                '���s�{�^�����\��
                btnAction.Enabled = False
                txtSyoriDateY.Focus()
            Else
                '���s�{�^����\��
                btnAction.Enabled = True

                btnSearch.Enabled = False
                txtSyoriDateD.Enabled = False
                txtSyoriDateM.Enabled = False
                txtSyoriDateY.Enabled = False
                cmbTimeStamp.Enabled = True
                btnReset.Enabled = True
                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���}�X�^����)", "���s", ex.Message)

            '�ĕ`�悷��悤�ɂ���
            cmbTimeStamp.EndUpdate()

            '���s�{�^�����\��
            btnAction.Enabled = False
            txtSyoriDateY.Focus()

            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

        Return True
    End Function

    '2018/01/23 saitou �L���M��(RSV2�W��) DEL �������σ��G���^���ʍX�V�Ή� -------------------- START
    '�s�v�֐�
    'Private Function fn_update_SCHMAST(ByVal strTIME_STAMP_W As String, ByRef lngSCH_KES_CNT As Long, ByRef lngSCH_TES_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
    '    '============================================================================
    '    'NAME           :fn_update_SCHMAST
    '    'Parameter      :
    '    'Description    :�X�P�W���[���}�X�^�X�V
    '    'Return         :True=OK,False=NG
    '    'Create         :
    '    'Update         :
    '    '============================================================================
    '    Dim SQL As String = ""

    '    Try

    '        lngSCH_KES_CNT = 0
    '        '------------------------------------------------
    '        '�X�P�W���[���}�X�^�X�V[����]
    '        '------------------------------------------------
    '        SQL = ""
    '        SQL = "UPDATE SCHMAST SET"
    '        SQL &= "  KESSAI_DATE_S = '" & "".PadLeft(8, "0"c) & "'"
    '        SQL &= " ,KESSAI_TIME_STAMP_S = '" & "".PadLeft(14, "0"c) & "'"
    '        SQL &= " ,KESSAI_FLG_S = '0'"
    '        SQL &= " WHERE KESSAI_TIME_STAMP_S = '" & strTIME_STAMP_W & "'"
    '        SQL &= "   AND KESSAI_FLG_S = '1'"
    '        SQL &= "   AND EXISTS"
    '        SQL &= "       (SELECT * FROM KESSAIMAST"
    '        SQL &= "         WHERE KESSAI_TIME_STAMP_S = TIME_STAMP_KR)"
    '        'SQL &= "           AND DATA_CRT_KBN_K = '01')"          '01:���G���^FD�쐬��

    '        Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
    '        If nRet >= 0 Then
    '            lngSCH_KES_CNT = nRet
    '        ElseIf nRet < 0 Then
    '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(fn_update_SCHMAST)", "���s", "�\�����ʃG���[")
    '            Return False
    '        End If

    '        lngSCH_TES_CNT = 0
    '        '------------------------------------------------
    '        '�X�P�W���[���}�X�^�X�V[�萔������]
    '        '------------------------------------------------
    '        SQL = ""
    '        SQL = "UPDATE SCHMAST SET"
    '        SQL &= "  TESUU_DATE_S = '" & "".PadLeft(8, "0"c) & "'"
    '        SQL &= " ,TESUU_TIME_STAMP_S = '" & "".PadLeft(14, "0"c) & "'"
    '        SQL &= " ,TESUUTYO_FLG_S = '0'"
    '        SQL &= " WHERE TESUU_TIME_STAMP_S = '" & strTIME_STAMP_W & "'"
    '        SQL &= "   AND TESUUTYO_FLG_S = '1'"
    '        SQL &= "   AND EXISTS"
    '        SQL &= "       (SELECT * FROM KESSAIMAST"
    '        SQL &= "         WHERE TESUU_TIME_STAMP_S = TIME_STAMP_KR)"
    '        ' SQL &= "           AND DATA_CRT_KBN_K = '01')"          '01:���G���^FD�쐬��

    '        nRet = rDB.ExecuteNonQuery(SQL)
    '        If nRet >= 0 Then
    '            lngSCH_TES_CNT = nRet
    '        ElseIf nRet < 0 Then
    '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���}�X�^�X�V)", "���s", "�\�����ʃG���[")
    '            Return False
    '        End If

    '        Return True

    '    Catch ex As Exception
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���}�X�^�X�V)", "���s", ex.Message)
    '        Return False
    '    Finally
    '    End Try

    'End Function

    'Private Function fn_delete_KESSAIMAST(ByVal strTIME_STAMP_W As String, ByRef lngKES_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
    '    '============================================================================
    '    'NAME           :fn_delete_KESSAIMAST
    '    'Parameter      :
    '    'Description    :�������σ}�X�^�폜
    '    'Return         :True=OK,False=NG
    '    'Create         :
    '    'Update         :
    '    '============================================================================
    '    Dim SQL As String = ""
    '    lngKES_CNT = 0
    '    Dim nRet As Integer
    '    Try

    '        '------------------------------------------------
    '        '�������σ}�X�^�폜
    '        '------------------------------------------------
    '        SQL = "DELETE FROM KESSAIMAST"
    '        SQL &= " WHERE TIME_STAMP_KR = '" & strTIME_STAMP_W & "'"

    '        nRet = rDB.ExecuteNonQuery(SQL)
    '        If nRet = 0 Then
    '            lngKES_CNT = nRet
    '        ElseIf nRet < 0 Then
    '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������σ}�X�^�폜)", "���s", "�\�����ʃG���[")
    '            Return False
    '        End If

    '    Catch ex As Exception
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�������σ}�X�^�폜)", "���s", ex.Message)
    '        Return False
    '    Finally

    '    End Try

    '    Return True
    'End Function
    '2018/01/23 saitou �L���M��(RSV2�W��) DEL ------------------------------------------------- END

    '�[������
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, _
            txtSyoriDateM.Validating, _
            txtSyoriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '2018/01/23 saitou �L���M��(RSV2�W��) ADD �������σ��G���^���ʍX�V�Ή� -------------------- START
    Private Function SetIniFIle() As Boolean
        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        '���G���^�t�@�C���쐬��
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR")
            MessageBox.Show(String.Format(MSG0001E, "���G���^�t�@�C���쐬�t�H���_", "COMMON", "RIENTADR"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")       '���G���^�t�@�C����
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���� ����:KESSAI ����:RIENTANAME")
            MessageBox.Show(String.Format(MSG0001E, "���G���^�t�@�C����", "COMMON", "RIENTANAME"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ini_info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If ini_info.RSV2_EDITION = "err" OrElse ini_info.RSV2_EDITION = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION")
            MessageBox.Show(String.Format(MSG0001E, "RSV2�@�\�ݒ�", "RSV2_V1.0.0", "EDITION"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ini_info.COMMON_BAITAIREAD = CASTCommon.GetFSKJIni("COMMON", "BAITAIREAD")
        If ini_info.COMMON_BAITAIREAD = "err" OrElse ini_info.COMMON_BAITAIREAD = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�}�̓Ǎ��p�t�H���_ ����:COMMON ����:BAITAIREAD")
            MessageBox.Show(String.Format(MSG0001E, "�}�̓Ǎ��p�t�H���_", "COMMON", "BAITAIREAD"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    '2018/01/23 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END

#End Region


End Class
