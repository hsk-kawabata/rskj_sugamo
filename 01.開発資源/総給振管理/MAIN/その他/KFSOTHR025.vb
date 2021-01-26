Imports CASTCommon
Imports System.Text

''' <summary>
''' �U�����M�f�[�^�쐬�����ʃN���X
''' </summary>
''' <remarks>���o�b�`�Ή�</remarks>
Public Class KFSOTHR025

#Region "�N���X�萔"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFSOTHR025", "�U�����M�f�[�^�쐬���")
    Private Const msgtitle As String = "�U�����M�f�[�^�쐬������(KFSOTHR025)"

#End Region

#Region "�N���X�ϐ�"
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U����
    End Structure
    Private LW As LogWrite

#End Region

#Region "�C�x���g�n���h��"

    ''' <summary>
    ''' ��ʃ��[�h�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFSOTHR025_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '------------------------------------------------
            '�V�X�e�����t�ƃ��[�U����\��
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label1, Me.Label2, Me.lblUser, Me.lblDate)

            '------------------------------------------------
            '��ʕ\�����A�������ɃV�X�e�����t��\��
            '------------------------------------------------
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            Me.txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            Me.txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            Me.txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            '------------------------------------------------
            '���s�{�^�����\���ݒ�
            '------------------------------------------------
            Me.btnAction.Enabled = False
            Me.cmbTimeStamp.Enabled = False
            Me.btnReset.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    ''' <summary>
    ''' �����{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(����)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' ���s�{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim lngSCH_CNT As Long
        Dim lngKAKUHO_CNT As Long
        Dim MainDB As CASTCommon.MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")
            '------------------------------------------------
            '�^�C���X�^���v�̎擾
            '------------------------------------------------
            Dim strTIME_STAMP_W As String
            strTIME_STAMP_W = Me.cmbTimeStamp.SelectedItem.ToString
            strTIME_STAMP_W = strTIME_STAMP_W.Substring(0, 4) & strTIME_STAMP_W.Substring(5, 2) & strTIME_STAMP_W.Substring(8, 2) _
                            & strTIME_STAMP_W.Substring(11, 2) & strTIME_STAMP_W.Substring(14, 2) & strTIME_STAMP_W.Substring(17, 2)

            If MessageBox.Show(MSG0015I.Replace("{0}", "�U�����M�f�[�^�쐬���"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            MainDB = New MyOracle
            MainDB.BeginTrans()     '�g�����U�N�V�����J�n

            '------------------------------------------------
            '�X�P�W���[���}�X�^�X�V����
            '------------------------------------------------
            If fn_update_SCHMAST(strTIME_STAMP_W, lngSCH_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "�X�V"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            Else
                '�X�V�������[�����̏ꍇ�A�G���[���b�Z�[�W�o��
                If lngSCH_CNT = 0 Then
                    MessageBox.Show(MSG0054W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainDB.Rollback()
                    Return
                End If
            End If

            '------------------------------------------------
            '�����U�˗��l�A�g�f�[�^�e�[�u���폜����
            '------------------------------------------------
            If fn_delete_S_I_RENKEIMAST(strTIME_STAMP_W, lngKAKUHO_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "�X�V"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            '------------------------------------------------
            '�����U���טA�g�f�[�^�e�[�u���폜����
            '------------------------------------------------
            If fn_delete_S_M_RENKEIMAST(strTIME_STAMP_W, lngKAKUHO_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "�X�V"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0016I.Replace("{0}", "�U�����M�f�[�^�쐬���"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '------------------------------------------------
            '��ʍĕ\������
            '------------------------------------------------
            If fn_select_SCHMAST(1) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "�X�V"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.cmbTimeStamp.Enabled = False
            Me.btnSearch.Enabled = True
            Me.txtSyoriDateD.Enabled = True
            Me.txtSyoriDateM.Enabled = True
            Me.txtSyoriDateY.Enabled = True
            Me.btnReset.Enabled = False
            Me.txtSyoriDateY.Focus()
            Return

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", ex.ToString)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try

    End Sub

    ''' <summary>
    ''' ����{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            Me.btnSearch.Enabled = True
            Me.btnAction.Enabled = False
            Me.cmbTimeStamp.Enabled = False

            Me.txtSyoriDateD.Enabled = True
            Me.txtSyoriDateM.Enabled = True
            Me.txtSyoriDateY.Enabled = True

            Me.txtSyoriDateY.Focus()

            Me.cmbTimeStamp.Items.Clear()
            Me.btnReset.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �I���{�^�������C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X�[�����߃C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtSyoriDateY.Validating, txtSyoriDateM.Validating, txtSyoriDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

#Region "�v���C�x�[�g���\�b�h"

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X�̓��̓`�F�b�N���s���܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_check_TEXT() As Boolean
        Try
            '------------------------------------------------
            '�����N�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If Me.txtSyoriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '�������`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If Me.txtSyoriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (CInt(Me.txtSyoriDateM.Text) >= 1 And CInt(Me.txtSyoriDateM.Text) <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '�������`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If Me.txtSyoriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateD.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (CInt(Me.txtSyoriDateD.Text) >= 1 And CInt(Me.txtSyoriDateD.Text) <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = Me.txtSyoriDateY.Text & "/" & Me.txtSyoriDateM.Text & "/" & Me.txtSyoriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' �X�P�W���[���}�X�^�̌������s���܂��B
    ''' </summary>
    ''' <param name="aintFLG"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_select_SCHMAST(ByVal aintFLG As Integer) As Boolean
        Dim strSyoriDate As String
        Dim strJOKEN1 As String '���o����1
        Dim strJOKEN2 As String '���o����2
        Dim strTIME_STAMP As String

        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As MyOracleReader = Nothing
        Dim OraRenReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)
            OraRenReader = New MyOracleReader(MainDB)

            '�������̎擾
            strSyoriDate = Me.txtSyoriDateY.Text & Me.txtSyoriDateM.Text & Me.txtSyoriDateD.Text
            strJOKEN1 = strSyoriDate & "000000"
            strJOKEN2 = strSyoriDate & "999999"

            '------------------------------------------------
            '�X�P�W���[���}�X�^�̌�������
            '------------------------------------------------
            With SQL
                .Length = 0
                .Append("SELECT DISTINCT HASSIN_TIME_STAMP_S")
                .Append(" FROM S_SCHMAST, S_I_RENKEIMAST")
                .Append(" WHERE HASSIN_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
                .Append(" AND HASSIN_FLG_S = '1'")
                .Append(" AND TYUUDAN_FLG_S = '0'")
                .Append(" AND HASSIN_TIME_STAMP_S = SAKUSEI_DATE_SR || SAKUSEI_TIME_SR")
                .Append(" ORDER BY HASSIN_TIME_STAMP_S")
            End With

            '�R���{�{�b�N�X�̏�����
            Me.cmbTimeStamp.Items.Clear()

            '�ĕ`�悵�Ȃ��悤�ɂ���
            Me.cmbTimeStamp.BeginUpdate()

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF

                    '------------------------------------------------
                    '�����U�˗��l�f�[�^�e�[�u���̌���
                    '------------------------------------------------
                    With SQL
                        .Length = 0
                        .Append("SELECT KIN_STS_SR FROM S_I_RENKEIMAST")
                        .Append(" WHERE SAKUSEI_DATE_SR || SAKUSEI_TIME_SR = " & SQ(OraReader.GetString("HASSIN_TIME_STAMP_S")))
                        .Append(" ORDER BY KIN_STS_SR DESC")
                    End With

                    Dim bAddFlg As Boolean = False
                    If OraRenReader.DataReader(SQL) = True Then
                        While OraRenReader.EOF = False
                            '�v�����@���o�b�`������������
                            '�~���ɂ����̂ŁA����������Ύ������s��
                            If OraRenReader.GetInt("KIN_STS_SR") = 9 Then
                                bAddFlg = False
                                Exit While
                            ElseIf OraRenReader.GetInt("KIN_STS_SR") = 1 Then
                                Dim strMsg As String = "���o�b�`�捞�ςł��B���s���܂����H" & vbCrLf
                                strMsg &= "�쐬�^�C���X�^���v�F" & OraReader.GetString("HASSIN_TIME_STAMP_S")
                                If MessageBox.Show(strMsg, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                                    bAddFlg = False
                                    Exit While
                                Else
                                    MessageBox.Show("�����͕K�����o�b�`���ł�������������s���Ă��������B", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                    bAddFlg = True
                                    Exit While
                                End If
                            Else
                                bAddFlg = True
                                Exit While
                            End If

                            OraRenReader.NextRead()
                        End While
                    End If

                    OraRenReader.Close()

                    If bAddFlg = True Then
                        '------------------------------------------------
                        '�R���{�{�b�N�X�Ƀ��X�g��ǉ�����
                        '------------------------------------------------
                        strTIME_STAMP = GCom.NzStr(OraReader.GetString("HASSIN_TIME_STAMP_S"))
                        strTIME_STAMP = strTIME_STAMP.Substring(0, 4) & "/" & strTIME_STAMP.Substring(4, 2) & "/" & strTIME_STAMP.Substring(6, 2) & Space(1) _
                                        & strTIME_STAMP.Substring(8, 2) & ":" & strTIME_STAMP.Substring(10, 2) & ":" & strTIME_STAMP.Substring(12, 2)

                        Me.cmbTimeStamp.Items.Add(strTIME_STAMP)

                    End If

                    OraReader.NextRead()
                Loop
            End If

            '�ĕ`�悷��悤�ɂ���
            Me.cmbTimeStamp.EndUpdate()

            '�Y���������[�����̏ꍇ
            If Me.cmbTimeStamp.Items.Count = 0 Then
                '�����{�^���������̂݃��b�Z�[�W�o��
                If aintFLG = 0 Then
                    MessageBox.Show(MSG0054W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                '���s�{�^�����\��
                Me.btnAction.Enabled = False

                Me.txtSyoriDateY.Focus()
            Else
                '���s�{�^����\��
                Me.btnAction.Enabled = True
                Me.cmbTimeStamp.Enabled = True
                Me.btnSearch.Enabled = False
                Me.txtSyoriDateD.Enabled = False
                Me.txtSyoriDateM.Enabled = False
                Me.txtSyoriDateY.Enabled = False
                Me.btnReset.Enabled = True
                Me.cmbTimeStamp.SelectedIndex = 0
                Me.cmbTimeStamp.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[������)", "���s", ex.ToString)

            '�ĕ`�悷��悤�ɂ���
            Me.cmbTimeStamp.EndUpdate()

            '���s�{�^�����\��
            Me.btnAction.Enabled = False
            Me.txtSyoriDateY.Focus()

            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' �X�P�W���[���}�X�^�̍X�V���s���܂��B
    ''' </summary>
    ''' <param name="strTIME_STAMP_W">�^�C���X�^���v</param>
    ''' <param name="lngSCH_CNT">�X�V����(�Q�Ɠn��)</param>
    ''' <param name="rDB">�I���N��</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_update_SCHMAST(ByVal strTIME_STAMP_W As String, ByRef lngSCH_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As StringBuilder = New StringBuilder(128)

        Try

            lngSCH_CNT = 0

            '------------------------------------------------
            '�X�P�W���[���}�X�^�X�V
            '------------------------------------------------
            With SQL
                .Append("UPDATE S_SCHMAST SET ")
                .Append(" KAKUHO_FLG_S = '0'")
                .Append(",KAKUHO_DATE_S = '00000000'")
                .Append(",KAKUHO_TIME_STAMP_S = '00000000000000'")
                .Append(",HASSIN_FLG_S = '0'")
                .Append(",HASSIN_DATE_S = '00000000'")
                .Append(",HASSIN_TIME_STAMP_S = '00000000000000'")
                .Append(",TESUUTYO_FLG_S = '0'")
                .Append(",TESUU_DATE_S = '00000000'")
                .Append(",TESUU_TIME_STAMP_S = '00000000000000'")
                .Append(" WHERE HASSIN_TIME_STAMP_S = " & SQ(strTIME_STAMP_W))
                .Append(" AND HASSIN_FLG_S = '1'")
                .Append(" AND TYUUDAN_FLG_S = '0'")
                .Append(" AND EXISTS (")
                .Append("   SELECT * FROM S_I_RENKEIMAST ")
                .Append("   WHERE HASSIN_TIME_STAMP_S = SAKUSEI_DATE_SR || SAKUSEI_TIME_SR)")
            End With

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet >= 0 Then
                lngSCH_CNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���X�V)", "���s", "�\�����ʃG���[")
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���X�V)", "���s", ex.ToString)
            Return False
        Finally
        End Try

    End Function

    ''' <summary>
    ''' �����U�˗��l�A�g�f�[�^�e�[�u�����폜���܂��B
    ''' </summary>
    ''' <param name="strTIME_STAMP_W"></param>
    ''' <param name="lngCNT"></param>
    ''' <param name="rDB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function fn_delete_S_I_RENKEIMAST(ByVal strTIME_STAMP_W As String, ByRef lngCNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As New StringBuilder
        lngCNT = 0

        Try
            '------------------------------------------------
            '�����U�˗��l�A�g�f�[�^�e�[�u���폜
            '------------------------------------------------
            With SQL
                .Append("DELETE FROM S_I_RENKEIMAST")
                .Append(" WHERE SAKUSEI_DATE_SR || SAKUSEI_TIME_SR = " & SQ(strTIME_STAMP_W))
            End With

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                lngCNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U�˗��l�A�g�f�[�^�e�[�u���폜)", "���s", "�\�����ʃG���[")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U�˗��l�A�g�f�[�^�e�[�u���폜)", "���s", ex.ToString)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' �����U���טA�g�f�[�^�e�[�u�����폜���܂��B
    ''' </summary>
    ''' <param name="strTIME_STAMP_W"></param>
    ''' <param name="lngCNT"></param>
    ''' <param name="rDB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function fn_delete_S_M_RENKEIMAST(ByVal strTIME_STAMP_W As String, ByRef lngCNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As New StringBuilder
        lngCNT = 0

        Try
            '------------------------------------------------
            '�����U���טA�g�f�[�^�e�[�u���폜
            '------------------------------------------------
            With SQL
                .Append("DELETE FROM S_M_RENKEIMAST")
                .Append(" WHERE SAKUSEI_DATE_SR || SAKUSEI_TIME_SR = " & SQ(strTIME_STAMP_W))
            End With

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                lngCNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U���טA�g�f�[�^�e�[�u���폜)", "���s", "�\�����ʃG���[")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����U���טA�g�f�[�^�e�[�u���폜)", "���s", ex.ToString)
            Return False
        End Try

        Return True
    End Function

#End Region

End Class
