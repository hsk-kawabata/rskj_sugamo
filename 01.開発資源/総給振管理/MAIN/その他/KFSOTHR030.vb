Option Strict On
Option Explicit On

Imports CASTCommon
Imports System.Text


' �X�V   �F RV-0024
Public Class KFSOTHR030
    Inherits System.Windows.Forms.Form

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFSOTHR030", "�ב֐������G���^�쐬������")
    Private Const msgtitle As String = "�ב֐������G���^�쐬������(KFSOTHR030)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U����
    End Structure

    Private LW As LogWrite

    '���LOAD��
    Private Sub KFSOTHR030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

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

            '------------------------------------------------
            '���s�{�^�����\���ݒ�
            '------------------------------------------------
            btnAction.Enabled = False
            cmbTimeStamp.Enabled = False
            btnReset.Enabled = False

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

        Dim lngSCH_CNT As Long
        Dim lngHASSIN_CNT As Long
        Dim MainDB As CASTCommon.MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")
            '------------------------------------------------
            '�^�C���X�^���v�̎擾
            '------------------------------------------------
            Dim strTIME_STAMP_W As String
            strTIME_STAMP_W = cmbTimeStamp.SelectedItem.ToString
            strTIME_STAMP_W = strTIME_STAMP_W.Substring(0, 4) & strTIME_STAMP_W.Substring(5, 2) & strTIME_STAMP_W.Substring(8, 2) _
                            & strTIME_STAMP_W.Substring(11, 2) & strTIME_STAMP_W.Substring(14, 2) & strTIME_STAMP_W.Substring(17, 2)

            If MessageBox.Show(MSG0015I.Replace("{0}", "�ב֐������G���^�쐬���"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
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
            '���U���σ}�X�^�폜����
            '------------------------------------------------
            If fn_delete_KESSAIMAST(strTIME_STAMP_W, lngHASSIN_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "�X�V"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0016I.Replace("{0}", "�ב֐������G���^�쐬���"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '------------------------------------------------
            '��ʍĕ\������
            '------------------------------------------------
            If fn_select_SCHMAST(1) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "�X�V"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            cmbTimeStamp.Enabled = False
            btnSearch.Enabled = True
            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            btnReset.Enabled = False
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            btnSearch.Enabled = True
            btnAction.Enabled = False
            cmbTimeStamp.Enabled = False

            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True

            txtSyoriDateY.Focus()

            cmbTimeStamp.Items.Clear()
            btnReset.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub

#Region "KFSOTHR030�p�֐�"

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
            If Not (GCom.NzInt(txtSyoriDateM.Text) >= 1 And GCom.NzInt(txtSyoriDateM.Text) <= 12) Then
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

            '�͈̓`�F�b�N
            If Not (GCom.NzInt(txtSyoriDateD.Text) >= 1 And GCom.NzInt(txtSyoriDateD.Text) <= 31) Then
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.Message)
            Return False

        Finally
        End Try

        Return True
    End Function

    Private Function fn_select_SCHMAST(ByVal aintFLG As Integer) As Boolean
        '============================================================================
        'NAME           :fn_select_SCHMAST
        'Parameter      :aintFLG=0:�����{�^���������@1:��ʍĕ\����
        'Description    :SCHMAST���甭�M�^�C���X�^���v�擾
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        Dim strSyoriDate As String
        Dim strJOKEN1 As String '���o����1
        Dim strJOKEN2 As String '���o����2
        Dim strTIME_STAMP As String

        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)

            '�������̎擾
            strSyoriDate = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text
            strJOKEN1 = strSyoriDate & "000000"
            strJOKEN2 = strSyoriDate & "999999"

            '------------------------------------------------
            '�X�P�W���[���}�X�^�̌�������
            '------------------------------------------------
            SQL.AppendLine("SELECT")
            SQL.AppendLine(" DISTINCT KESSAI_TIME_STAMP_S AS TIME_STAMP")
            SQL.AppendLine(" FROM")
            SQL.AppendLine(" S_SCHMAST, S_KESSAIMAST")
            SQL.AppendLine(" WHERE KESSAI_TIME_STAMP_S = TIME_STAMP_KR")
            SQL.AppendLine(" AND KESSAI_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
            SQL.AppendLine(" AND KESSAI_FLG_S = '1'")
            SQL.AppendLine(" ORDER BY TIME_STAMP")

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
                    MessageBox.Show(MSG0054W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                '���s�{�^�����\��
                btnAction.Enabled = False

                txtSyoriDateY.Focus()
            Else
                '���s�{�^����\��
                btnAction.Enabled = True
                cmbTimeStamp.Enabled = True
                btnSearch.Enabled = False
                txtSyoriDateD.Enabled = False
                txtSyoriDateM.Enabled = False
                txtSyoriDateY.Enabled = False
                btnReset.Enabled = True
                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[������)", "���s", ex.Message)

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

    Private Function fn_update_SCHMAST(ByVal strTIME_STAMP_W As String, ByRef lngSCH_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        '============================================================================
        'NAME           :fn_update_SCHMAST
        'Parameter      :
        'Description    :�X�P�W���[���}�X�^�X�V
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================
        Dim SQL As StringBuilder = New StringBuilder(128)

        Try

            lngSCH_CNT = 0

            '------------------------------------------------
            '�X�P�W���[���}�X�^�X�V
            '------------------------------------------------
            SQL.AppendLine("UPDATE S_SCHMAST SET")
            SQL.AppendLine(" KESSAI_DATE_S = '00000000'")
            SQL.AppendLine(",KESSAI_TIME_STAMP_S = '00000000000000'")
            SQL.AppendLine(",KESSAI_FLG_S = '0'")
            SQL.AppendLine(" WHERE KESSAI_TIME_STAMP_S = '" & strTIME_STAMP_W & "'")
            SQL.AppendLine(" AND KESSAI_FLG_S = '1'")
            SQL.AppendLine(" AND EXISTS")
            SQL.AppendLine(" (SELECT * FROM S_KESSAIMAST")
            SQL.AppendLine(" WHERE KESSAI_TIME_STAMP_S = TIME_STAMP_KR)")

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet >= 0 Then
                lngSCH_CNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���X�V)", "���s", "�\�����ʃG���[")
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�P�W���[���X�V)", "���s", ex.Message)
            Return False
        Finally
        End Try

    End Function
    Private Function fn_delete_KESSAIMAST(ByVal strTIME_STAMP_W As String, ByRef lngCNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        '============================================================================
        'NAME           :fn_delete_KESSAIMAST
        'Parameter      :
        'Description    :���U���σ}�X�^�폜
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================
        Dim SQL As String = ""
        lngCNT = 0

        Try
            '------------------------------------------------
            '���U���σ}�X�^�폜
            '------------------------------------------------
            SQL = "DELETE FROM S_KESSAIMAST"
            SQL &= " WHERE TIME_STAMP_KR = '" & strTIME_STAMP_W & "'"

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                lngCNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���σ}�X�^�폜)", "���s", "�\�����ʃG���[")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���σ}�X�^�폜)", "���s", ex.Message)
            Return False
        Finally
        End Try

        Return True
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
