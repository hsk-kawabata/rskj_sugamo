Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT030

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT030", "�萔����������ƈꗗ�\������")
    Private Const msgtitle As String = "�萔����������ƈꗗ�\������(KFKPRNT030)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private Sub KFKPRNT030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            txtKijyunDateY.Text = strGetdate.Substring(0, 4)
            txtKijyunDateM.Text = strGetdate.Substring(4, 2)
            txtKijyunDateD.Text = strGetdate.Substring(6, 2)

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

            '����̎擾
            Dim strKIJYUN_DATE As String
            strKIJYUN_DATE = txtKijyunDateY.Text & txtKijyunDateM.Text & txtKijyunDateD.Text

            '------------------------------------------------
            '����Ώې�̌����`�F�b�N
            '------------------------------------------------
            Dim lngDataCNT As Long = 0
            If fn_Select_Data(strKIJYUN_DATE, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "����"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtKijyunDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Exit Sub
            End If


            '����O�m�F���b�Z�[�W
            If MessageBox.Show(MSG0013I.Replace("{0}", "�萔����������ƈꗗ�\"), _
                               msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                txtKijyunDateY.Focus()
                Return
            End If

            '------------------------------------------------
            ' �萔����������ƈꗗ�\����o�b�`�̌Ăяo��
            ' �����F���
            ' �߂�l:1:����@-1:����0���@�ȊO:�G���[
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim Param As String = GCom.GetUserID & "," & strKIJYUN_DATE
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP007.EXE", Param)

            If nRet = 0 Then
                '����
                MessageBox.Show(MSG0014I.Replace("{0}", "�萔����������ƈꗗ�\"), _
                                msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '�Ώ�0��
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '�ُ�@���|�G�[�W�F���g�G���[�R�[�h���󂯂���
                MessageBox.Show(MSG0004E.Replace("{0}", "�萔����������ƈꗗ�\"), _
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
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try

    End Sub

#Region "KFKPRNT030�p�֐�"

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
            '��N�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '����`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKijyunDateM.Text >= 1 And txtKijyunDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '����`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtKijyunDateD.Text >= 1 And txtKijyunDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtKijyunDateY.Text & "/" & txtKijyunDateM.Text & "/" & txtKijyunDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
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

    Private Function fn_Select_Data(ByVal astrKIJYUN_DATE As String, ByRef alngDataCNT As Long) As Boolean
        '============================================================================
        'NAME           :fn_Select_Data
        'Parameter      :
        'Description    :����Ώې悪���邩�ǂ���
        'Return         :True=OK(�Ώۂ���),False=NG(�ΏۂȂ�)
        'Create         :
        'Update         :
        '============================================================================

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try
            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TESUU_YDATE_SV1 <= '" & astrKIJYUN_DATE & "'")
            '2009/09/23 �萔�������҂��E���ύς݁E���f�t���O�������ɒǉ�
            'SQL.Append(" AND TESUUTYO_FLG_SV1 = '0'")
            SQL.Append(" AND TESUUTYO_FLG_SV1 IN('0','2')")   '�������E�����҂�
            SQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")
            SQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")
            '=====================================
            SQL.Append(" AND TESUU_KIN_SV1 > 0")
            SQL.Append(" AND TOUROKU_FLG_SV1 = '1'")
            SQL.Append(" AND FUNOU_FLG_SV1 = '1' ")
            ' 00:���񂫂񒆋��a����,01:��������,02:�ב֐U��,03:�ב֕t��,04:���ʊ��,05:���ϑΏۊO
            'SQL.Append(" AND (KESSAI_KBN_TV1 = '00'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '01'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '02'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '03'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '04'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '05')")

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
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
