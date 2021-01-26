Imports CASTCommon
Imports System.Text
Public Class KFJMAST031

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "


#End Region
#Region " �ϐ�"
    Private noClose As Boolean
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST031", "�X�P�W���[���č쐬���")
    '2011/06/16 �W���ŏC�� �^�C�g���C�� ------------------
    Private Const msgTitle As String = "�X�P�W���[���č쐬���(KFJMAST031)"
    'Private Const msgTitle As String = "�X�P�W���[���č쐬��ʉ��(KFJMAST031)"
    Private CLS As ClsSchduleMaintenanceClass

    Public TORIS_CODE As String '������R�[�h
    Public TORIF_CODE As String '����敛�R�[�h
    Public KFURI_DATE As String '�_��U�֓�
    Public FURI_DATE As String  '�U�֓�
    Public FURI_DATE_After As String '�ύX��U�֓�
    Private MainDB As MyOracle
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
#End Region
#Region " ���[�h"
    Private Sub KFJMAST031_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE & TORIF_CODE
            LW.FuriDate = FURI_DATE
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            '�x�����̒~��
            Call CLS.SetKyuzituInformation()
            'SCHMAST���ږ��̒~��
            Call CLS.SetSchMastInformation()
            Me.TopMost = True
            txtFuriDateY.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " ���s"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim Ret As Integer = 0
        Dim OkFlg As Boolean
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")
            '--------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If

            If CheckSCHMAST() = False Then
                Exit Sub
            End If
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            '�g�����U�N�V�����J�n
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            CLS.SCH.FURI_DATE = FURI_DATE
            Ret = CLS.GET_SELECT_TORIMAST(Nothing, _
                               TORIS_CODE, TORIF_CODE, ClsSchduleMaintenanceClass.OPT.OptionNothing)

            '�X�P�W���[���}�X�^����Y���������R�[�h���폜

            If CLS.DELETE_SCHMAST(False) = False Then
                MessageBox.Show(String.Format(MSG0027E, "�X�P�W���[���}�X�^", "�폜"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                Return
            End If

            '�o�^�s�ׂ̎��s
            CLS.SCH.FURI_DATE = FURI_DATE_After
            CLS.SCH.KFURI_DATE = KFURI_DATE
            If CLS.INSERT_NEW_SCHMAST(0) = True Then

                If CLS.TR(0).SFURI_FLG = 1 Then      '�ĐU�̃X�P�W���[�������ς�

                    Dim wrkTorifCode As String = CLS.TR(0).TORIF_CODE
                    Dim wrkSfuriFlg As Integer = CLS.TR(0).SFURI_FLG
                    Dim wrkFuriDate As String = CLS.SCH.FURI_DATE
                    CLS.TR(0).TORIF_CODE = CLS.TR(0).SFURI_FCODE
                    CLS.TR(0).SFURI_FLG = 0
                    CLS.SCH.FURI_DATE = CLS.SCH.KSAIFURI_DATE

                    If CLS.INSERT_NEW_SCHMAST(0) = True Then

                        CLS.TR(0).TORIF_CODE = wrkTorifCode
                        CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                        CLS.SCH.FURI_DATE = wrkFuriDate
                    Else
                        CLS.TR(0).TORIF_CODE = wrkTorifCode
                        CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                        CLS.SCH.FURI_DATE = wrkFuriDate

                        MessageBox.Show(String.Format(MSG0002E, "�o�^"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                        Return
                    End If
                End If
            Else
                MessageBox.Show(String.Format(MSG0002E, "�o�^"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                Return
            End If
            '2011/07/04 �W���ŏC�� ���b�Z�[�W�ɂ�OK�{�^���̂ݕ\�� ------------------START
            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            'MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            '2011/07/04 �W���ŏC�� ���b�Z�[�W�ɂ�OK�{�^���̂ݕ\�� ------------------END

            gstrFURI_DATE = FURI_DATE_After
            noClose = True
            OkFlg = True
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", ex.ToString)
        Finally
            If OkFlg Then
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)
            Else
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try
    End Sub

#End Region
#Region " �N���[�Y"
    Private Sub KFJMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            gstrFURI_DATE = ""
            noClose = True
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            Me.Close()
            Me.Dispose()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try

    End Sub

#End Region
    '2011/06/16 �W���ŏC�� 0���߂��s�� ------------------START
#Region " �C�x���g"
    '�[���p�f�B���O
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception

        End Try
    End Sub
#End Region
    '2011/06/16 �W���ŏC�� 0���߂��s�� ------------------END

#Region " �֐�"
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :�X�V�{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2010/02/05
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
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
            FURI_DATE_After = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            If FURI_DATE_After = FURI_DATE Then
                MessageBox.Show(String.Format(MSG0318W, TORIS_CODE, TORIF_CODE, FURI_DATE.Substring(0, 4) & _
                                               "�N" & FURI_DATE.Substring(4, 2) & "��" & FURI_DATE.Substring(6, 2) & "��"), _
                                               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            If Not GCom.CheckDateModule(FURI_DATE_After, "", 0) Then
                MessageBox.Show(MSG0292W.Replace("{0}", "�U�֓�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally

        End Try
        fn_check_text = True
    End Function
    Public Function fn_check_text(ByVal TORIS As TextBox, ByVal TORIF As TextBox, _
                                  ByVal FURI_Y As TextBox, ByVal FURI_M As TextBox, ByVal FURI_D As TextBox) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :�X�V�{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2010/02/05
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
            '������R�[�h�K�{�`�F�b�N
            If TORIS.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If
            '����敛�R�[�h�K�{�`�F�b�N
            If TORIF.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIF.Focus()
                Return False
            End If
            '�N�K�{�`�F�b�N
            If FURI_Y.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_Y.Focus()
                Return False
            End If
            '���K�{�`�F�b�N
            If FURI_M.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_M.Focus()
                Return False
            End If
            '���͈̓`�F�b�N
            If GCom.NzInt(FURI_M.Text.Trim) < 1 OrElse GCom.NzInt(FURI_M.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_M.Focus()
                Return False
            End If
            '���t�K�{�`�F�b�N
            If FURI_D.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_D.Focus()
                Return False
            End If
            '���t�͈̓`�F�b�N
            If GCom.NzInt(FURI_D.Text.Trim) < 1 OrElse GCom.NzInt(FURI_D.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_D.Focus()
                Return False
            End If

            '���t�������`�F�b�N
            Dim WORK_DATE As String = FURI_Y.Text & "/" & FURI_M.Text & "/" & FURI_D.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_Y.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally

        End Try
        fn_check_text = True
    End Function
    Public Function fn_check_Table(ByVal TORIS As TextBox, ByVal TORIF As TextBox, _
                                   ByVal FURI_Y As TextBox, ByVal FURI_M As TextBox, ByVal FURI_D As TextBox, _
                                   Optional ByVal Iraisyo As Boolean = False) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :�X�V�{�^�����������Ƀ}�X�^���փ`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2010/02/05
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            MainDB = New MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            Dim strBAITAI_CODE As String

            '�������擾
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(TORIS.Text)))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(TORIF.Text)))
            If OraReader.DataReader(SQL) = True Then
                strBAITAI_CODE = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                OraReader.Close()
            Else
                '�����Ȃ�
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                OraReader.Close()
                Return False
            End If

            '�}�̃R�[�h�`�F�b�N
            If strBAITAI_CODE = "07" Then '�w�Z
                MessageBox.Show(String.Format(MSG0039I, "�ύX"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If

            If Iraisyo AndAlso strBAITAI_CODE <> "04" Then '�˗���
                MessageBox.Show(MSG0108W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If

            TORIS_CODE = TORIS.Text
            TORIF_CODE = TORIF.Text
            FURI_DATE = FURI_Y.Text + FURI_M.Text + FURI_D.Text

            '�������ĐU�p�̎����}�X�^�����f
            If CLS.CHECK_SAIFURI_SELF(TORIS_CODE, TORIF_CODE) = False Then
                MessageBox.Show(MSG0283W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If

            '�X�P�W���[���}�X�^�ɑΏۂ̃X�P�W���[�������݂��邩�`�F�b�N����
            SQL = New StringBuilder(128)
            SQL.Append("SELECT * ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            SQL.Append(" AND TORIS_CODE_T = " & SQ(TORIS.Text))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF.Text))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))

            If OraReader.DataReader(SQL) = True Then
                If GCom.NzStr(OraReader.Reader.Item("UKETUKE_FLG_S")) <> "0" OrElse _
                   GCom.NzStr(OraReader.Reader.Item("TOUROKU_FLG_S")) <> "0" OrElse _
                   GCom.NzStr(OraReader.Reader.Item("TYUUDAN_FLG_S")) <> "0" OrElse _
                   GCom.NzStr(OraReader.Reader.Item("NIPPO_FLG_S")) <> "0" Then
                    MessageBox.Show(String.Format(MSG0286W, GCom.NzStr(OraReader.Reader.Item("UKETUKE_FLG_S")), _
                                                  GCom.NzStr(OraReader.Reader.Item("TOUROKU_FLG_S")), _
                                                  GCom.NzStr(OraReader.Reader.Item("NIPPO_FLG_S")), _
                                                  GCom.NzStr(OraReader.Reader.Item("TYUUDAN_FLG_S"))), _
                                                  msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    TORIS.Focus()
                    OraReader.Close()
                    Return False
                Else
                    KFURI_DATE = GCom.NzStr(OraReader.Reader.Item("KFURI_DATE_S"))
                    OraReader.Close()
                End If
            Else
                '�X�P�W���[���Ȃ�
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                txtFuriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_check_Table = True
    End Function

    Public Function CheckSCHMAST() As Boolean
        Try
            If CLS.SEARCH_NEW_SCHMAST(TORIS_CODE, TORIF_CODE, FURI_DATE_After) = False Then
                MessageBox.Show(String.Format(MSG0319W, TORIS_CODE, TORIF_CODE, FURI_DATE_After.Substring(0, 4) & _
                                               "�N" & FURI_DATE_After.Substring(4, 2) & "��" & FURI_DATE_After.Substring(6, 2) & "��"), _
                                               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try
    End Function
#End Region
End Class
