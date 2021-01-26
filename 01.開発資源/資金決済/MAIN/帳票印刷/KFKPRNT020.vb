Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT020

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT020", "�������ϊ�ƈꗗ�\������")
    Private Const msgTitle As String = "�������ϊ�ƈꗗ�\������(KFKPRNT020)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    '2017/05/16 �^�X�N�j���� ADD �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- START
    Private PRINT_MODE As String = CASTCommon.GetRSKJIni("PRINT", "KFKP005_MODE")   '0:�\�蒠�[�A0�ȊO:���ʒ��[
    '2017/05/16 �^�X�N�j���� ADD �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- END

    Private Sub KFKPRNT020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            '��ʕ\�����A���ϓ��ɃV�X�e�����t��\��
            '------------------------------------------------
            Dim strSysDate As String

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtKessaiDateY.Text = strSysDate.Substring(0, 4)
            txtKessaiDateM.Text = strSysDate.Substring(4, 2)
            txtKessaiDateD.Text = strSysDate.Substring(6, 2)

            ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
            ''------------------------------------------------
            ''����敪��ʏ�ɐݒ�
            ''------------------------------------------------
            'cmbPrintKbn.SelectedIndex = 0
            '�R���{�{�b�N�X�ݒ�l���e�L�X�g�ɐݒ肷��
            Select Case GCom.SetComboBox(Me.cmbPrintKbn, "KFKPRNT020_����敪.TXT", True)
                Case 1  '�t�@�C���Ȃ�
                    MessageBox.Show(String.Format(MSG0025E, "����敪", "KFKPRNT020_����敪.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", "KFKPRNT020_����敪.TXT�Ȃ�")
                Case 2  '�ُ�
                    MessageBox.Show(String.Format(MSG0026E, "����敪"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", "����敪�R���{�{�b�N�X�ݒ�ُ�")
            End Select
            ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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

            '------------------------------------------------
            '����Ώې�̌����`�F�b�N
            '------------------------------------------------
            Dim lngDataCNT As Long = 0
            Dim strKbn As String = ""

            ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
            'Select Case cmbPrintKbn.Text
            '    Case "�ʏ�"
            '        strKbn = "1"
            '    Case "���񂫂񒆋���"
            '        strKbn = "2"
            '    Case "���񂫂񒆋����ȊO"
            '        strKbn = "3"
            '    Case Else
            '        MessageBox.Show(MSG0232W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtKessaiDateY.Focus()
            '        Exit Sub
            'End Select
            '�R���{�{�b�N�X�ݒ�l���e�L�X�g�ɐݒ肷��
            strKbn = GCom.GetComboBox(Me.cmbPrintKbn)
            ' 2016/04/22 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

            If fn_Select_Data(strKESSAI_DATE, strKbn, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(MSG0013I.Replace("{0}", "�������ϊ�ƈꗗ�\"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            '------------------------------------------------
            ' �������ϊ�ƈꗗ�\����o�b�`�̌Ăяo��
            ' �����F���ϓ�
            ' �߂�l:1:����@-1:����0���@�ȊO:�G���[
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim Param As String = GCom.GetUserID & "," & strKESSAI_DATE & "," & strKbn
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP005.EXE", Param)

            If nRet = 0 Then
                '����
                MessageBox.Show(MSG0014I.Replace("{0}", "�������ϊ�ƈꗗ�\"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '�Ώ�0��
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '�ُ�@���|�G�[�W�F���g�G���[�R�[�h���󂯂���
                MessageBox.Show(MSG0004E.Replace("{0}", "�������ϊ�ƈꗗ�\"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            'btnEnd.Focus()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try

    End Sub

    '�I���{�^��
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

#Region "KFKMAIN020�p�֐�"

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
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ό��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKessaiDateM.Text >= 1 And txtKessaiDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '���ϓ��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKessaiDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKessaiDateD.Text >= 1 And txtKessaiDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtKessaiDateY.Text & "/" & txtKessaiDateM.Text & "/" & txtKessaiDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
        End Try

    End Function

    Public Function fn_Select_Data(ByVal astrKESSAI_DATE As String, ByVal astrMode As String, ByRef alngDataCNT As Long) As Boolean
        '    '============================================================================
        '    'NAME           :fn_Select_Data
        '    'Parameter      :
        '    'Description    :����Ώې悪���邩�ǂ���
        '    'Return         :True=OK(�Ώۂ���),False=NG(�ΏۂȂ�)
        '    'Create         :
        '    'Update         :
        '    '============================================================================

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append(" SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
            SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
            '2009/09/23 �������ύς݃f�[�^�𒊏o����悤�C��
            'SQL.Append(" AND (TESUU_DATE_S = '" & astrKESSAI_DATE & "'")
            'SQL.Append("  OR  KESSAI_DATE_S =  '" & astrKESSAI_DATE & "'")
            'SQL.Append("  OR  (TESUUTYO_FLG_S = '0'")
            'SQL.Append("   AND TESUU_YDATE_S <= '" & astrKESSAI_DATE & "'))")
            'SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND SCHMAST.FUNOU_FLG_S = '1'")
            '2017/05/16 �^�X�N�j���� CHG �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- START
            If PRINT_MODE = "0" Then
                '�\�蒠�[
                SQL.Append(" AND SCHMAST.KESSAI_YDATE_S = '" & astrKESSAI_DATE & "'")
                SQL.Append(" AND SCHMAST.TESUUKEI_FLG_S = '1'")
            Else
                '���ʒ��[
                SQL.Append(" AND SCHMAST.KESSAI_DATE_S = '" & astrKESSAI_DATE & "'")
                SQL.Append(" AND SCHMAST.KESSAI_FLG_S = '1'")
            End If
            '2017/05/16 �^�X�N�j���� CHG �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- END
            SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND SCHMAST.SYORI_KIN_S > 0")

            Select Case astrMode
                Case "1"    '�ʏ큨���ϑΏۊO�ȊO
                    SQL.Append(" AND KESSAI_KBN_T <> '99'")
                Case "2"    '�M��������
                    SQL.Append(" AND KESSAI_KBN_T = '00'")
                Case "3"    '���񂫂񒆋��ȊO
                    SQL.Append(" AND KESSAI_KBN_T NOT IN('00','99')")
            End Select

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
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKessaiDateY.Validating, txtKessaiDateM.Validating, txtKessaiDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)

    End Sub

#End Region

End Class