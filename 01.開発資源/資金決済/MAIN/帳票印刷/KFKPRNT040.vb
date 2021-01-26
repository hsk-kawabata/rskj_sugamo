Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT040

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT040", "�萔���ꊇ�������ו\������")
    Private Const msgtitle As String = "�萔���ꊇ�������ו\������(KFKPRNT040)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private Sub KFKPRNT040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            '��ʕ\�����A����ɃV�X�e�����t��\��
            '------------------------------------------------
            '�x���}�X�^��荞��
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MessageBox.Show(MSG0003E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʕ\�����ɐU�֓��ɑO�c�Ɠ���\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strGetdate As String = ""

            strGetdate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            'bRet = GCom.CheckDateModule(strGetdate, strGetdate, 1, 1)
            txtKessaiDateY.Text = strGetdate.Substring(0, 4)
            txtKessaiDateM.Text = strGetdate.Substring(4, 2)
            txtKessaiDateD.Text = strGetdate.Substring(6, 2)
            '2009/12/03 �ǉ� =======================================
            txtKessaiDateY2.Text = strGetdate.Substring(0, 4)
            txtKessaiDateM2.Text = strGetdate.Substring(4, 2)
            txtKessaiDateD2.Text = strGetdate.Substring(6, 2)
            '=======================================================
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    '����{�^��
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            '------------------------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '------------------------------------------------
            If fn_check_TEXT() = False Then Exit Sub

            '���ϓ��̎擾
            Dim strKESSAI_DATE As String
            strKESSAI_DATE = txtKessaiDateY.Text & txtKessaiDateM.Text & txtKessaiDateD.Text
            Dim strKESSAI_DATE2 As String
            strKESSAI_DATE2 = txtKessaiDateY2.Text & txtKessaiDateM2.Text & txtKessaiDateD2.Text
            '------------------------------------------------
            '����Ώې�̌����`�F�b�N
            '------------------------------------------------
            Dim lngDataCNT As Long = 0

            If fn_Select_Data(strKESSAI_DATE, strKESSAI_DATE2, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "����"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(MSG0013I.Replace("{0}", "�萔���ꊇ�������ו\"), _
                               msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                txtKessaiDateY.Focus()
                Return
            End If

            '------------------------------------------------
            ' �萔���ꊇ�������ו\����o�b�`�̌Ăяo��
            ' �����F���ϓ�
            ' �߂�l:1:����@-1:����0���@�ȊO:�G���[
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '2009/12/01 �����ύX =========================================
            'Dim Param As String = GCom.GetUserID & "," & strKESSAI_DATE
            Dim Param As String = GCom.GetUserID & "," & strKESSAI_DATE & "," & strKESSAI_DATE2
            '=============================================================
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP008.EXE", Param)

            If nRet = 0 Then
                '����
                MessageBox.Show(MSG0014I.Replace("{0}", "�萔���ꊇ�������ו\"), _
                                msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '�Ώ�0��
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '�ُ�@���|�G�[�W�F���g�G���[�R�[�h���󂯂���
                MessageBox.Show(MSG0004E.Replace("{0}", "�萔���ꊇ�������ו\"), _
                                msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")

        End Try

    End Sub

    '�I���{�^��
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")

            Me.Close()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try

    End Sub

#Region "KFKPRNT040�p�֐�"

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :����{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================
        Try
            '------------------------------------------------
            '���ϔN�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ό��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKessaiDateM.Text >= 1 And txtKessaiDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ϓ��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKessaiDateD.Text >= 1 And txtKessaiDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtKessaiDateY.Text & "/" & txtKessaiDateM.Text & "/" & txtKessaiDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            '2009/12/01 �`�F�b�N�ǉ� ========================
            '------------------------------------------------
            '���ϔN�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateY2.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY2.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ό��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateM2.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM2.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKessaiDateM2.Text >= 1 And txtKessaiDateM2.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM2.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ϓ��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateD2.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD2.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKessaiDateD2.Text >= 1 And txtKessaiDateD2.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD2.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE2 As String = txtKessaiDateY2.Text & "/" & txtKessaiDateM2.Text & "/" & txtKessaiDateD2.Text

            If Information.IsDate(WORK_DATE2) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY2.Focus()
                Return False
            End If

            '���t�O��`�F�b�N
            If WORK_DATE > WORK_DATE2 Then
                MessageBox.Show(MSG0099W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If
            '=================================================
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function fn_Select_Data(ByVal astrKESSAI_DATE As String, ByVal astrKESSAI_DATE2 As String, ByRef alngDataCNT As Long) As Boolean
        '============================================================================
        'NAME           :fn_Select_Data
        'Parameter      :
        'Description    :����Ώې悪���邩�ǂ���
        'Return         :True=OK(�Ώۂ���),False=NG(�ΏۂȂ�)
        'Create         :
        'Update         :2009/12/01 �����ǉ�
        '============================================================================

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE�@FSYORI_KBN_T ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TESUUKEI_FLG_S = '1' ")                        '1:�萔���v�Z��
            '2009/12/01 �����ύX =============
            'SQL.Append(" AND TESUU_YDATE_S = '" & astrKESSAI_DATE & "'")
            SQL.Append(" AND TESUU_YDATE_S >= '" & astrKESSAI_DATE & "'")
            SQL.Append(" AND TESUU_YDATE_S <= '" & astrKESSAI_DATE2 & "'")
            '=================================
            SQL.Append(" AND TESUUTYO_FLG_S = '0'")                         '0:�萔��������
            SQL.Append(" AND TESUUTYO_KBN_T = '1'")                         '1:�ꊇ����
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                           '0:�L��
            SQL.Append(" AND TOUROKU_FLG_S = '1'")                          '1:�o�^��
            SQL.Append(" AND TESUU_KIN_S > 0")                              '�萔�����z

            If OraReader.DataReader(SQL) = True Then
                alngDataCNT = OraReader.GetInt64("COUNTER")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^����)", "���s", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

    '�[������
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKessaiDateY.Validating, txtKessaiDateM.Validating, txtKessaiDateD.Validating, txtKessaiDateM2.Validating, txtKessaiDateD2.Validating, txtKessaiDateY2.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class