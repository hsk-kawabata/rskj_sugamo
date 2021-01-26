Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT120
    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT120", "�U�֕s�\���R�ʏW�v�\������")
    Private Const msgTitle As String = "�U�֕s�\���R�ʏW�v�\������(KFJPRNT120)"
    Private Const errMsg As String = "��O���������܂����B���O���m�F�̂����A�ێ�v���ɘA�����Ă��������B"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

#Region "�錾"
    Dim strTORIS_CODE As String
    Dim strTORIF_CODE As String
    Dim strFURI_DATE As String
#End Region

    Private Sub KFJPRNT120_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '#####################################
        '���O�̏����ɕK�v�ȏ��̎擾
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�V�X�e�����t�ƃ��[�U����\��
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            '�ϑ��Җ����X�g�{�b�N�X�̐ݒ�
            '--------------------------------
            '�����R���{�{�b�N�X�ݒ�
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub

#Region " �I��"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " ���"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :����{�^��
        'Return         :
        'Create         :2009/09/19
        'Update         :
        '=====================================================================================
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Dim lngDataCNT As Long = 0

        Dim PrintID As String = ""
        Dim PrintName As String = ""
        Dim param As String
        Dim nRet As Integer
        Dim EndCode As Integer

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            PrintID = "KFJP052"
            PrintName = "�U�֕s�\���R�ʏW�v�\"

            '2014/03/19 saitou �ޗǐM�� �W���C�� MODIFY ----------------------------------------------->>>>
            '���b�Z�[�W�ړ�
            'If MessageBox.Show(String.Format(MSG0013I, PrintName), _
            '                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            '    Return
            'End If
            '2014/03/19 saitou �ޗǐM�� �W���C�� MODIFY -----------------------------------------------<<<<

            '--------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '--------------------------------
            If fn_check_text() = False Then
                Return
            End If

            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.ToriCode = strTORIS_CODE + strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            '------------------------------------------
            '�����}�X�^���݃`�F�b�N
            '------------------------------------------
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append("SELECT ")
                SQL.Append(" COUNT(*) COUNTER")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

                If OraReader.DataReader(SQL) = True Then
                    lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
                    MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    txtTorisCode.Focus()
                    Exit Sub
                End If

                If lngDataCNT = 0 Then
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Exit Sub
                End If
            End If

            '------------------------------------------
            '�Ώۃf�[�^�擾
            '------------------------------------------
            '2018/06/26 saitou �L���M��(RSV2) ADD ���׃}�X�^���݃`�F�b�N�ǉ� ---------------------------------------- START
            '�X�P�W���[�������݂��āA���׃}�X�^�����݂��Ȃ��ꍇ�i���n��̓���Ȃǁj�ɁA
            '����������ُ�I������̂��ُ�I�����Ȃ��悤�ɍl������B
            With SQL
                .Length = 0

                .Append("select")
                .Append("     TORIS_CODE_T")
                .Append("    ,TORIF_CODE_T")
                .Append(" from")
                .Append("     SCHMAST")
                .Append("    ,TORIMAST")
                .Append("    ,MEIMAST")
                .Append(" where")
                .Append("     FURI_DATE_S = '" & strFURI_DATE & "'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FUNOU_FLG_S = '1'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                .Append(" and TORIS_CODE_S = TORIS_CODE_K")
                .Append(" and TORIF_CODE_S = TORIF_CODE_K")
                .Append(" and FURI_DATE_S = FURI_DATE_K")

                If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                    .Append(" and TORIS_CODE_S = '" & strTORIS_CODE & "'")
                    .Append(" and TORIF_CODE_S = '" & strTORIF_CODE & "'")
                End If

                .Append(" group by")
                .Append("     TORIS_CODE_T")
                .Append("    ,TORIF_CODE_T")
                .Append(" having")
                .Append("     count(*) > 0")
                .Append(" order by")
                .Append("     TORIS_CODE_T")
                .Append("    ,TORIF_CODE_T")
            End With

            'SQL = New StringBuilder(128)

            'SQL.Append(" SELECT")
            'SQL.Append(" TORIS_CODE_T,TORIF_CODE_T")
            'SQL.Append(" FROM SCHMAST,TORIMAST")
            'SQL.Append(" WHERE ")
            'SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND FUNOU_FLG_S = '1'")
            'SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            'SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

            'If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
            '    SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
            '    SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            'End If
            '2018/06/26 saitou �L���M��(RSV2) ADD ------------------------------------------------------------------- END

            If OraReader.DataReader(SQL) = True Then

                '2014/03/19 saitou �ޗǐM�� �W���C�� MODIFY ----------------------------------------------->>>>
                '���b�Z�[�W�ړ�
                If MessageBox.Show(String.Format(MSG0013I, PrintName),
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If
                '2014/03/19 saitou �ޗǐM�� �W���C�� MODIFY -----------------------------------------------<<<<

                While OraReader.EOF = False
                    strTORIS_CODE = OraReader.GetString("TORIS_CODE_T")
                    strTORIF_CODE = OraReader.GetString("TORIF_CODE_T")

                    '------------------------------------------
                    '����������s
                    '------------------------------------------

                    '����o�b�`�Ăяo��
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me

                    param = strTORIS_CODE & strTORIF_CODE & "," & strFURI_DATE
                    nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

                    If nRet <> 0 Then
                        EndCode = 2
                    End If

                    OraReader.NextRead()
                End While

            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
                '2014/03/19 saitou �ޗǐM�� �W���C�� MODIFY ----------------------------------------------->>>>
                '���b�Z�[�W�ύX
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '2014/03/19 saitou �ޗǐM�� �W���C�� MODIFY -----------------------------------------------<<<<
                txtFuriDateY.Focus()
                Exit Sub
            End If

            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case EndCode
                Case 0
                    '�����m�F���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub

#End Region

#Region "�֐�"

    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :����{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2009/09/19
        'Update         :
        '============================================================================
        fn_check_text = False
        Try
            '������R�[�h�K�{�`�F�b�N
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            '����敛�R�[�h�K�{�`�F�b�N
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If

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
            '2011/06/16 �W���ŏC�� �`�F�b�N�������ւ� ------------------START

            '���t�K�{�`�F�b�N
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '���͈̓`�F�b�N
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            ''���t�K�{�`�F�b�N
            'If txtFuriDateD.Text.Trim = "" Then
            '    MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    txtFuriDateD.Focus()
            '    Return False
            'End If
            '2011/06/16 �W���ŏC�� �`�F�b�N�������ւ� ------------------END
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

            fn_check_text = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try
    End Function

#End Region

#Region " �C�x���g"

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '�ϑ��Җ����X�g�{�b�N�X�̐ݒ�
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '�����R���{�{�b�N�X�ݒ�
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                cmbToriName.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����R���{�{�b�N�X�ݒ�)", "���s", ex.ToString)
        End Try
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '�����R�[�h�e�L�X�g�{�b�N�X�Ɏ����R�[�h�ݒ�
        '-------------------------------------------
        Try
            '�����R���{�{�b�N�X�ݒ�
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����R�[�h�擾)", "���s", ex.ToString)
        End Try

    End Sub
    '�[������
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�[���p�f�B���O)", "���s", ex.ToString)
        End Try
    End Sub
#End Region

End Class
