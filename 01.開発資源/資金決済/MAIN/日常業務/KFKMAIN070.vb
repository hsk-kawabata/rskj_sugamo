Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN070

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN070", "�������σf�[�^���ʍX�V���")
    Private Const msgtitle As String = "�������σf�[�^���ʍX�V���(KFKMAIN070)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    '���LOAD��
    Private Sub KFKMAIN070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

            If MessageBox.Show(String.Format(MSG0015I, "�������σf�[�^���ʍX�V"), _
                                   msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '���o�b�`���ʍX�V����
            If Me.UpdateKekkaKinBatch(strTIME_STAMP_W, MainDB) = False Then
                Return
            End If

            MessageBox.Show(String.Format(MSG0016I, "�������σf�[�^���ʍX�V"), _
                            msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            
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

#Region "KFKMAIN070�p�֐�"

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
            With SQL
                .Length = 0
                .Append("SELECT TIME_STAMP FROM (")
                .Append(" (SELECT KESSAI_TIME_STAMP_S AS TIME_STAMP")
                .Append("  FROM SCHMAST, K_I_RENKEIMAST")
                .Append("  WHERE KESSAI_TIME_STAMP_S = SAKUSEI_DATE_KR || SAKUSEI_TIME_KR")
                .Append("  AND KESSAI_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
                .Append("  AND KESSAI_FLG_S = '1'")
                .Append("  AND SYORI_STS_KR = 9)")
                .Append(" UNION")
                .Append(" (SELECT TESUU_TIME_STAMP_S AS TIME_STAMP")
                .Append("  FROM SCHMAST, K_I_RENKEIMAST")
                .Append("  WHERE TESUU_TIME_STAMP_S = SAKUSEI_DATE_KR || SAKUSEI_TIME_KR")
                .Append("  AND TESUU_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
                .Append("  AND TESUUTYO_FLG_S = '1'")
                .Append("  AND SYORI_STS_KR = 9)")
                .Append(" )")
                .Append(" GROUP BY TIME_STAMP")
                .Append(" ORDER BY TIME_STAMP")
            End With
            
            '�R���{�{�b�N�X�̏�����
            cmbTimeStamp.Items.Clear()

            '�ĕ`�悵�Ȃ��悤�ɂ���
            cmbTimeStamp.BeginUpdate()

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    '------------------------------------------------
                    '�R���{�{�b�N�X�Ƀ��X�g��ǉ�����
                    '------------------------------------------------
                    strTIME_STAMP = GCom.NzStr(OraReader.GetString("TIME_STAMP"))
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

    ''' <summary>
    ''' �������σf�[�^���ʍX�V(���o�b�`)�������s���܂��B
    ''' </summary>
    ''' <param name="TimeStamp">�^�C���X�^���v</param>
    ''' <param name="db">�I���N��</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/09/17 ��_�M�� ���o�b�`�Ή�</remarks>
    Private Function UpdateKekkaKinBatch(ByVal TimeStamp As String, _
                                         ByVal db As CASTCommon.MyOracle) As Boolean

        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim List As KFKP003
        Dim CsvFileName As String = String.Empty
        Dim strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        Dim strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

        Try
            '�������ϘA�g�f�[�^�e�[�u������Ώۂ̃��R�[�h���擾����
            Dim strSakuseiDate As String = TimeStamp.Substring(0, 8)
            Dim strSakuseiTime As String = TimeStamp.Substring(8, 6)

            With SQL
                .Length = 0
                .Append("SELECT")
                .Append(" TORIS_CODE_KR")
                .Append(",TORIF_CODE_KR")
                .Append(",ITAKU_NNAME_KR")
                .Append(",FURI_DATE_KR")
                .Append(",KAMOKU_KR")
                .Append(",OPE_KR")
                .Append(",ERR_MESSAGE_CODE_KR")
                .Append(",ERR_MESSAGE_KR")
                .Append(" FROM K_I_RENKEIMAST")
                .Append(" WHERE SAKUSEI_DATE_KR = " & SQ(strSakuseiDate))
                .Append(" AND SAKUSEI_TIME_KR = " & SQ(strSakuseiTime))
                .Append(" AND SYORI_STS_KR = 9")
                .Append(" ORDER BY KAIJI_KR, RECORD_KR")
            End With

            oraReader = New CASTCommon.MyOracleReader(db)

            If oraReader.DataReader(SQL) = True Then
                '�X�V�Ώۂ���
                List = New KFKP003
                CsvFileName = List.CreateCsvFile

                While oraReader.EOF = False
                    List.CSVObject.Output(strDate, True)            '������
                    List.CSVObject.Output(strTime, True)            '��������
                    List.CSVObject.Output(oraReader.GetString("TORIS_CODE_KR"), True)   '������R�[�h
                    List.CSVObject.Output(oraReader.GetString("TORIF_CODE_KR"), True)   '����敛�R�[�h
                    List.CSVObject.Output(oraReader.GetString("ITAKU_NNAME_KR"), True)  '�ϑ��Җ�
                    List.CSVObject.Output(oraReader.GetString("FURI_DATE_KR"), True)    '�U�֓�
                    List.CSVObject.Output(oraReader.GetString("KAMOKU_KR"), True)       '�ȖڃR�[�h
                    List.CSVObject.Output(oraReader.GetString("OPE_KR"), True)          '�I�y�R�[�h
                    List.CSVObject.Output(oraReader.GetString("ERR_MESSAGE_CODE_KR"), True) '�G���[���b�Z�[�W�R�[�h
                    List.CSVObject.Output(oraReader.GetString("ERR_MESSAGE_KR"), True, True) '�G���[���b�Z�[�W

                    oraReader.NextRead()

                End While
            Else
                '�X�V�ΏۂȂ�
                oraReader.Close()
                oraReader = Nothing
                MessageBox.Show(MSG0055W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '�������ʊm�F�\�̈������
            oraReader.Close()
            oraReader = Nothing

            List.CloseCsv()
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            Dim nRet As Integer

            ExeRepo.SetOwner = Me
            param = GCom.GetUserID & "," & CsvFileName & "," & "1"
            nRet = ExeRepo.ExecReport("KFKP003.EXE", param)

            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case nRet
                Case 0
                Case -1
                    '����ΏۂȂ����b�Z�[�W
                    MessageBox.Show(String.Format(MSG0226W, "�������ʊm�F�\(�������σf�[�^���ʍX�V)"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, "�������ʊm�F�\(�������σf�[�^���ʍX�V)"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            Return True

        Catch ex As Exception
            MainLOG.Write("�������σf�[�^���ʍX�V", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try

    End Function

    '�[������
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, _
            txtSyoriDateM.Validating, _
            txtSyoriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
