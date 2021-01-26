Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT060

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT060", "�a�������U�֓���[�萔��������������")
    Private Const msgTitle As String = "�a�������U�֓���[�萔��������������(KFKPRNT060)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
    Private PrintName As String  '���[��
    Private PrintID As String    '���[ID
    Private CsvNames() As String '�擾CSV�t�@�C����
    Private SyoriDate As String

    '2011/06/24 �W���ŏC�� �����l�͑O�c�Ɠ���\�� ------------------START
    Private FmtComm As New CAstFormat.CFormat
    '2011/06/24 �W���ŏC�� �����l�͑O�c�Ɠ���\�� ------------------END
#Region " ���[�h"
    Private Sub KFKPRNT060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Dim strSysDate As String
            '2011/06/24 �W���ŏC�� �����l�͑O�c�Ɠ���\�� ------------------START
            '2016/11/11 saitou RSV2 ADD �x���擾�s��Ή� ---------------------------------------- START
            '�I���N���R�l�N�V�������t�H�[�}�b�g�N���X�ɓn���ċx�����X�g�̍쐬���s��
            Dim MainDB As CASTCommon.MyOracle = Nothing
            Try
                MainDB = New CASTCommon.MyOracle
                Me.FmtComm.Oracle = MainDB

            Catch ex As Exception

            Finally
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End Try
            '2016/11/11 saitou RSV2 ADD ----------------------------------------------------------- END
            strSysDate = String.Format("{0:yyyyMMdd}", CASTCommon.GetEigyobi(Date.Now, -1, FmtComm.HolidayList))
            'strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            '2011/06/24 �W���ŏC�� �����l�͑O�c�Ɠ���\�� ------------------END


            txtFuriDateY.Text = strSysDate.Substring(0, 4)
            txtFuriDateM.Text = strSysDate.Substring(4, 2)
            txtFuriDateD.Text = strSysDate.Substring(6, 2)

            Dim Jyoken As String = " AND KESSAI_KBN_T <> '99'"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '�R���g���[������

            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
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
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            If fn_check_text() = False Then
                Return
            End If

            '------------------------------------------------
            '����Ώې�̌����`�F�b�N
            '------------------------------------------------
            Dim lngDataCNT As Long = 0

            If fn_Select_Data(String.Concat(New String() {txtFuriDateY.Text, txtFuriDateM.Text, txtFuriDateD.Text}), lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtTorisCode.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            End If

            '����O�m�F���b�Z�[�W
            If MessageBox.Show(MSG0013I.Replace("{0}", "�a�������U�֓���[�萔��������"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) <> DialogResult.Yes Then
                txtTorisCode.Focus()
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����

            param = GCom.GetUserID & "," & String.Concat(txtTorisCode.Text, txtTorifCode.Text) & "," _
            & String.Concat(New String() {txtFuriDateY.Text, txtFuriDateM.Text, txtFuriDateD.Text})

            nRet = ExeRepo.ExecReport("KFKP009.EXE", param)

            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case nRet
                Case 0
                    MessageBox.Show(MSG0014I.Replace("{0}", "�a�������U�֓���[�萔��������"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MessageBox.Show(MSG0004E.Replace("{0}", "�a�������U�֓���[�萔��������"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region
#Region " ���"
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :����{�^��
        'Return         :
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
            '�R���g���[������
            txtFuriDateY.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " �I��"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)��O", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " �֐�"
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :�X�V�{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2009/09/18
        'Update         :
        '============================================================================
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

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.Message)
            Return False
        Finally

        End Try

        Return True

    End Function


    Private Function fn_Select_Data(ByVal astrFURI_DATE As String, ByRef alngDataCNT As Long) As Boolean
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
        Dim ToriCode As String = String.Concat(txtTorisCode.Text, txtTorifCode.Text)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE�@FSYORI_KBN_T ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

            If ToriCode <> "0".PadLeft(12, "0"c) Then

                SQL.Append(" AND TORIS_CODE_S = " & SQ(ToriCode.Substring(0, 10)))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(ToriCode.Substring(10, 2)))

            End If

            SQL.Append(" AND FURI_DATE_S = '" & astrFURI_DATE & "'")        '�U�֓����p�����[�^��
            SQL.Append(" AND TESUUKEI_FLG_S = '1'")                       '�萔���v�Z��
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                       '0:�L��

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

#End Region
#Region " �C�x���g"
    '�[���p�f�B���O
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorisCode.Validating, txtTorifCode.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�[���p�f�B���O", "���s", ex.Message)
        End Try
    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '�I���J�i�Ŏn�܂�ϑ��Җ����擾
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '�����R���{�{�b�N�X�ݒ�(Msg????W)
                Dim Jyoken As String = " AND KESSAI_KBN_T <> '99'"
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
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

#End Region

End Class
