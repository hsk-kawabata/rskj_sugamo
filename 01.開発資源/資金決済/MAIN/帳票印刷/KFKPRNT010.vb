Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT010

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT010", "���U������ƈꗗ�\������")
    Private Const msgTitle As String = "���U������ƈꗗ�\������(KFKPRNT010)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure
    Private LW As LogWrite

    Private Sub KFKPRNT010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            '�S�x������~��
            '------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�x�����擾)", "���s", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʕ\�����ɐU�֓��ɑO�c�Ɠ���\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
            Dim strGetdate As String = ""
            Call GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            Me.txtFuriDateY.Text = strGetdate.Substring(0, 4)
            Me.txtFuriDateM.Text = strGetdate.Substring(4, 2)
            Me.txtFuriDateD.Text = strGetdate.Substring(6, 2)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    '����{�^��
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        '�U�֓��̎擾
        Dim strFURI_DATE As String

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '------------------------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '------------------------------------------------

            If fn_check_TEXT() = False Then Exit Sub

            '------------------------------------------------
            '����Ώې�̌����`�F�b�N
            '------------------------------------------------
            Dim lngDataCNT As Long = 0
            If fn_Select_Data(strFURI_DATE, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(MSG0013I.Replace("{0}", "���U������ƈꗗ�\"), msgTitle, MessageBoxButtons.YesNo, _
                               MessageBoxIcon.Question) <> DialogResult.Yes Then
                txtFuriDateY.Focus()
                Return
            End If

            '------------------------------------------------
            ' ���U������ƈꗗ�\����o�b�`�̌Ăяo��
            ' �����F�U�֓�
            ' �߂�l:1:����-1:����0���@�ȊO:�G���[
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim Param As String = GCom.GetUserID & "," & strFURI_DATE
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP004.EXE", Param)

            If nRet = 0 Then
                '����
                MessageBox.Show(MSG0014I.Replace("{0}", "���U������ƈꗗ�\"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '�Ώ�0��
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '�ُ�@���|�G�[�W�F���g�G���[�R�[�h���󂯂���
                MessageBox.Show(MSG0004E.Replace("{0}", "���U������ƈꗗ�\"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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

#Region "KFJKESS0400G�p�֐�"

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :����{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2004/09/10
        'Update         :
        '============================================================================
        Try

            '------------------------------------------------
            '�U�֔N�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtFuriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '�U�֌��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtFuriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtFuriDateM.Text >= 1 And txtFuriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '�U�֓��`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtFuriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '�����`�F�b�N
            If Not (txtFuriDateD.Text >= 1 And txtFuriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
        End Try

    End Function

    ''' <summary>
    ''' fn_Select_Data
    ''' </summary>
    ''' <param name="astrFURI_DATE"></param>
    ''' <param name="alngDataCNT"></param>
    ''' <returns>True=OK(�Ώۂ���) False=NG(�ΏۂȂ�)</returns>
    ''' <remarks></remarks>
    Private Function fn_Select_Data(ByVal astrFURI_DATE As String, ByRef alngDataCNT As Long) As Integer


        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FURI_DATE_SV1 = '" & astrFURI_DATE & "'")
            SQL.Append(" AND SYORI_KIN_SV1 > 0")
            SQL.Append(" AND TOUROKU_FLG_SV1 = '1'")
            SQL.Append(" AND FUNOU_FLG_SV1 = '1'")
            SQL.Append(" AND TESUUKEI_FLG_SV1 = '1'") '2009/09/23 �ǉ�
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
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtFuriDateY.Validating, _
            txtFuriDateM.Validating, _
            txtFuriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
