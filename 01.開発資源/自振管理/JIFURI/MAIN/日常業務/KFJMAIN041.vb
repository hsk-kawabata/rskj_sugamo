Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFJMAIN041

    Public FURI_DATE As String
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private strSOUSIN_KBN As String

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN041", "�s�\���ʍX�V(��Ǝ���)���")
    Private Const msgTitle As String = "�s�\���ʍX�V(��Ǝ���)���(KFJMAIN041)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle
    '�p�u���b�N�f�[�^�[�x�[�X

    Private Const ThisModuleName As String = "KFKMAST041.vb"

#Region " ���[�h"
    Private Sub KFJMAIN041_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '�V�X�e�����t�ƃ��[�U����\��
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            '�ϑ��Җ����X�g�{�b�N�X�̐ݒ�
            '--------------------------------
            '�����R���{�{�b�N�X�ݒ�
            Dim Jyoken As String = " AND SOUSIN_KBN_T = '1'"   '���M�敪���S�����
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " �X�V"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :�s�\���ʍX�V���s�{�^��(��Ǝ���)
        'Return         :
        'Create         :2009/09/10
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")
            '--------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            FURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            Dim strTORIS_CODE As String = txtTorisCode.Text
            Dim strTORIF_CODE As String = txtTorifCode.Text

            LW.ToriCode = strTORIS_CODE & strTORIF_CODE
            LW.FuriDate = FURI_DATE

            '--------------------------------
            '�}�X�^�`�F�b�N
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            '�X�V�O�m�F���b�Z�[�W
            If MessageBox.Show(String.Format(MSG0023I, "�s�\���ʍX�V(��Ǝ���)"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()

            Dim jobid As String
            Dim para As String

            ' 2016/04/22 �^�X�N�j���� CHG �yPG�zUI_B-99-99(RSV2�Ή�) -------------------- START
            ''�W���u�}�X�^�ɓo�^
            'jobid = "J040"                      '..\Batch\���ʍX�V\
            ''�p�����[�^(�U�֓�,�����敪(��Ǝ���),�X�V�L�[����,�����R�[�h)
            'para = FURI_DATE + ",1,0," + strTORIS_CODE & strTORIF_CODE

            ''#########################
            ''job����
            ''#########################
            'Dim iRet As Integer
            'iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            'If iRet = 1 Then    '�W���u�o�^��
            '    MainDB.Rollback()
            '    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return
            'ElseIf iRet = -1 Then '�W���u�������s
            '    MainDB.Rollback()
            '    MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)", "���s", "�W���u�}�X�^�������s �p�����[�^:" & para)
            '    Return
            'End If

            ''#########################
            ''job�o�^
            ''#########################
            'If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then '�W���u�o�^���s
            '    MainDB.Rollback()
            '    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)", "���s", "�W���u�o�^���s �p�����[�^:" & para)
            '    Return
            'End If
            '--------------------------------
            ' JOB�o�^�iALL8�w��j
            '--------------------------------
            If Trim(txtTorisCode.Text) & Trim(txtTorifCode.Text) = "888888888888" Then
                Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                Dim OraReader2 As New CASTCommon.MyOracleReader(MainDB)
                Dim SQL As New StringBuilder(128)
                Dim SQL2 As New StringBuilder(128)

                SQL = New StringBuilder(128)
                SQL.Append("SELECT")
                SQL.Append("      ITAKU_KANRI_CODE_T")
                SQL.Append(" FROM")
                SQL.Append("      TORIMAST")
                SQL.Append("    , SCHMAST")
                SQL.Append(" WHERE ")
                SQL.Append("      TORIS_CODE_T   = TORIS_CODE_S")
                SQL.Append("  AND TORIF_CODE_T   = TORIF_CODE_S")
                SQL.Append("  AND FURI_DATE_S    = " & SQ(FURI_DATE))
                SQL.Append("  AND SOUSIN_KBN_S   = '1'")
                SQL.Append("  AND HAISIN_FLG_S   = '1'")
                SQL.Append("  AND FUNOU_FLG_S    = '0'")
                SQL.Append("  AND TYUUDAN_FLG_S  = '0'")
                SQL.Append(" GROUP BY")
                SQL.Append("      ITAKU_KANRI_CODE_T")

                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        Dim tmp_ITAKU_KANRI_CODE_T As String = ""
                        '2017/03/28 FJH)�X CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        tmp_ITAKU_KANRI_CODE_T = OraReader.Reader.Item("ITAKU_KANRI_CODE_T")
                        'tmp_ITAKU_KANRI_CODE_T = GCom.NzInt(OraReader.Reader.Item("ITAKU_KANRI_CODE_T"))
                        '2017/03/28 FJH)�X CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END

                        SQL2 = New StringBuilder(128)
                        SQL2.Append("SELECT")
                        SQL2.Append("      TORIS_CODE_T")
                        SQL2.Append("    , TORIF_CODE_T")
                        SQL2.Append(" FROM")
                        SQL2.Append("      TORIMAST")
                        SQL2.Append("    , SCHMAST")
                        SQL2.Append(" WHERE ")
                        SQL2.Append("      TORIS_CODE_T       = TORIS_CODE_S")
                        SQL2.Append("  AND TORIF_CODE_T       = TORIF_CODE_S")
                        SQL2.Append("  AND FURI_DATE_S        = " & SQ(FURI_DATE))
                        SQL2.Append("  AND SOUSIN_KBN_S       = '1'")
                        SQL2.Append("  AND HAISIN_FLG_S       = '1'")
                        SQL2.Append("  AND FUNOU_FLG_S        = '0'")
                        SQL2.Append("  AND TYUUDAN_FLG_S      = '0'")
                        '2017/03/28 FJH)�X CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
                        SQL2.Append("  AND ITAKU_KANRI_CODE_T = " & SQ(tmp_ITAKU_KANRI_CODE_T))
                        'SQL2.Append("  AND ITAKU_KANRI_CODE_T = " & tmp_ITAKU_KANRI_CODE_T)
                        '2017/03/28 FJH)�X CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
                        SQL2.Append(" ORDER BY")
                        SQL2.Append("      TORIS_CODE_T")
                        SQL2.Append("    , TORIF_CODE_T")

                        Dim tmp_TORIS_CODE_T As String = ""
                        Dim tmp_TORIF_CODE_T As String = ""

                        If OraReader2.DataReader(SQL2) = True Then
                            tmp_TORIS_CODE_T = OraReader2.Reader.Item("TORIS_CODE_T")
                            tmp_TORIF_CODE_T = OraReader2.Reader.Item("TORIF_CODE_T")
                        End If

                        '�p�����[�^(�U�֓�,�����敪(��Ǝ���),�X�V�L�[����,�����R�[�h)
                        jobid = "J040"                      '..\Batch\���ʍX�V\
                        para = FURI_DATE + ",1,0," + tmp_TORIS_CODE_T & tmp_TORIF_CODE_T

                        '--------------------------------
                        ' ����JOB����
                        '--------------------------------
                        Dim iRet As Integer
                        iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                        If iRet = 1 Then    '�W���u�o�^��
                            MainDB.Rollback()
                            MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        ElseIf iRet = -1 Then '�W���u�������s
                            MainDB.Rollback()
                            MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)", "���s", "�W���u�}�X�^�������s �p�����[�^:" & para)
                            Return
                        End If

                        '--------------------------------
                        ' JOB�o�^
                        '--------------------------------
                        If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then '�W���u�o�^���s
                            MainDB.Rollback()
                            MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)", "���s", "�W���u�o�^���s �p�����[�^:" & para)
                            Return
                        End If

                        OraReader.NextRead()
                    End While
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
                    'MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MessageBox.Show("�s�\���ʍX�V�Ώۂ������݂��܂���B", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    txtFuriDateY.Focus()
                    Exit Sub
                End If
            Else
                '--------------------------------
                ' JOB�o�^�i�����R�[�h�w��j
                '--------------------------------
                '�p�����[�^(�U�֓�,�����敪(��Ǝ���),�X�V�L�[����,�����R�[�h)
                jobid = "J040"
                para = FURI_DATE + ",1,0," + strTORIS_CODE & strTORIF_CODE

                '--------------------------------
                ' ����JOB����
                '--------------------------------
                Dim iRet As Integer
                iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                If iRet = 1 Then    '�W���u�o�^��
                    MainDB.Rollback()
                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                ElseIf iRet = -1 Then '�W���u�������s
                    MainDB.Rollback()
                    MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)", "���s", "�W���u�}�X�^�������s �p�����[�^:" & para)
                    Return
                End If

                '--------------------------------
                ' JOB�o�^
                '--------------------------------
                If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then '�W���u�o�^���s
                    MainDB.Rollback()
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)", "���s", "�W���u�o�^���s �p�����[�^:" & para)
                    Return
                End If
            End If
            ' 2016/04/22 �^�X�N�j���� CHG �yPG�zUI_B-99-99(RSV2�Ή�) -------------------- END

            MainDB.Commit()

            MessageBox.Show(String.Format(MSG0021I, "�s�\���ʍX�V(��Ǝ���)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)", "���s", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��O", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region
#Region " �֐�"
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :�X�V�{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2009/09/09
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
            '���͈̓`�F�b�N
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then '(MSG0022W)
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
            fn_check_text = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try

    End Function
    Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_Table
        'Parameter      :
        'Description    :�X�V�{�^�����������Ƀ}�X�^�̑��փ`�F�b�N�����s
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_Table = False
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try
            ' 2016/04/22 �^�X�N�j���� CHG �yPG�zUI_B-99-99(RSV2�Ή�) -------------------- START
            ''�������擾
            'SQL.Append("SELECT * FROM TORIMAST")
            'SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
            'SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            'If OraReader.DataReader(SQL) = True Then
            '    strSOUSIN_KBN = GCom.NzStr(OraReader.Reader.Item("SOUSIN_KBN_T"))
            '    OraReader.Close()
            'Else
            '    '�����Ȃ�
            '    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    OraReader.Close()
            '    txtTorisCode.Focus()
            '    Return False
            'End If

            ''���M�敪�`�F�b�N
            'If strSOUSIN_KBN <> "1" Then '�S�����
            '    MessageBox.Show(MSG0336W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    txtTorisCode.Focus()
            '    Return False
            'End If
            '--------------------------------
            ' �������擾
            '--------------------------------
            If Trim(txtTorisCode.Text) & Trim(txtTorifCode.Text) <> "888888888888" Then
                '�������擾
                SQL.Append("SELECT * FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
                If OraReader.DataReader(SQL) = True Then
                    strSOUSIN_KBN = GCom.NzStr(OraReader.Reader.Item("SOUSIN_KBN_T"))
                    OraReader.Close()
                Else
                    '�����Ȃ�
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Return False
                End If

                '���M�敪�`�F�b�N
                If strSOUSIN_KBN <> "1" Then '�S�����
                    MessageBox.Show(MSG0336W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorisCode.Focus()
                    Return False
                End If
            End If
            ' 2016/04/22 �^�X�N�j���� CHG �yPG�zUI_B-99-99(RSV2�Ή�) -------------------- END

            ' 2016/04/22 �^�X�N�j���� CHG �yPG�zUI_B-99-99(RSV2�Ή�) -------------------- START
            ''�X�P�W���[�����擾
            'SQL = New StringBuilder(128)
            'SQL.Append("SELECT COUNT(*) COUNTER FROM TORIMAST,SCHMAST")
            'SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
            'SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            'SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            'SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            'SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))
            'SQL.Append(" AND SOUSIN_KBN_S = '1'")
            'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND HAISIN_FLG_S = '1'")
            '--------------------------------
            ' �X�P�W���[�����擾
            '--------------------------------
            SQL = New StringBuilder(128)
            If Trim(txtTorisCode.Text) & Trim(txtTorifCode.Text) <> "888888888888" Then
                SQL.Append("SELECT")
                SQL.Append("      COUNT(*) COUNTER")
                SQL.Append(" FROM")
                SQL.Append("      TORIMAST")
                SQL.Append("    , SCHMAST")
                SQL.Append(" WHERE")
                SQL.Append("      TORIS_CODE_T  = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append("  AND TORIF_CODE_T  = " & SQ(Trim(txtTorifCode.Text)))
                SQL.Append("  AND TORIS_CODE_T  = TORIS_CODE_S ")
                SQL.Append("  AND TORIF_CODE_T  = TORIF_CODE_S ")
                SQL.Append("  AND FURI_DATE_S   = " & SQ(FURI_DATE))
                SQL.Append("  AND SOUSIN_KBN_S  = '1'")
                SQL.Append("  AND HAISIN_FLG_S  = '1'")
                SQL.Append("  AND TYUUDAN_FLG_S = '0'")
            Else
                SQL.Append("SELECT")
                SQL.Append("      COUNT(*) COUNTER")
                SQL.Append(" FROM")
                SQL.Append("      TORIMAST")
                SQL.Append("    , SCHMAST")
                SQL.Append(" WHERE")
                SQL.Append("      TORIS_CODE_T  = TORIS_CODE_S ")
                SQL.Append("  AND TORIF_CODE_T  = TORIF_CODE_S ")
                SQL.Append("  AND FURI_DATE_S   = " & SQ(FURI_DATE))
                SQL.Append("  AND SOUSIN_KBN_S  = '1'")
                SQL.Append("  AND HAISIN_FLG_S  = '1'")
                SQL.Append("  AND TYUUDAN_FLG_S = '0'")
            End If
            ' 2016/04/22 �^�X�N�j���� CHG �yPG�zUI_B-99-99(RSV2�Ή�) -------------------- END

            Dim Count As Integer
            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                OraReader.Close()
            Else
                '�������s(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                txtTorisCode.Focus()
                Return False
            End If

            If Count = 0 Then
                '�X�P�W���[���Ȃ�(MSG0068W)
                MessageBox.Show(MSG0068W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", ex.ToString)
            txtTorisCode.Focus()
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If MainDB IsNot Nothing Then MainDB.Close()
        End Try
        fn_check_Table = True
    End Function
#End Region
#Region " �C�x���g"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '�I���J�i�Ŏn�܂�ϑ��Җ����擾
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '�����R���{�{�b�N�X�ݒ�
                Dim Jyoken As String = " AND SOUSIN_KBN_T = '1'"   '���M�敪���S�����
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
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
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�����R�[�h�擾)", "���s", ex.ToString)
        End Try

    End Sub
    '�[���p�f�B���O
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorisCode.Validating, txtTorifCode.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�[���p�f�B���O)", "���s", ex.ToString)
        End Try
    End Sub
#End Region

    '2010/12/24 �M�g�Ή� �������ʊm�F�\����{�^���ǉ�
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            If txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "999999999999" AndAlso txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "" Then
                '������R�[�h�K�{�`�F�b�N
                If txtTorisCode.Text.Trim = "" Then
                    MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorisCode.Focus()
                    Exit Sub
                End If
                '����敛�R�[�h�K�{�`�F�b�N
                If txtTorifCode.Text.Trim = "" Then
                    MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorifCode.Focus()
                    Exit Sub
                End If
            End If
            '�N�K�{�`�F�b�N
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Exit Sub
            End If
            '���K�{�`�F�b�N
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Exit Sub
            End If
            '���͈̓`�F�b�N
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then '(MSG0022W)
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Exit Sub
            End If
            '���t�K�{�`�F�b�N
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Exit Sub
            End If
            '���t�͈̓`�F�b�N
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Exit Sub
            End If

            '���t�������`�F�b�N
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Exit Sub
            End If

            Dim strTORIS_CODE As String = txtTorisCode.Text
            Dim strTORIF_CODE As String = txtTorifCode.Text
            FURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.ToriCode = strTORIS_CODE + strTORIF_CODE
            LW.FuriDate = FURI_DATE

            '--------------------------------
            '�}�X�^�`�F�b�N
            '--------------------------------
            MainDB = New CASTCommon.MyOracle
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As New StringBuilder(128)

            Try
                If txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "999999999999" AndAlso txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "" Then
                    '�������擾
                    SQL.Append("SELECT SOUSIN_KBN_T FROM TORIMAST")
                    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(txtTorisCode.Text.Trim))
                    SQL.Append(" AND TORIF_CODE_T = " & SQ(txtTorifCode.Text.Trim))
                    If OraReader.DataReader(SQL) = True Then
                        strSOUSIN_KBN = GCom.NzStr(OraReader.Reader.Item("SOUSIN_KBN_T"))
                        OraReader.Close()
                    Else
                        '�����Ȃ�
                        MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        OraReader.Close()
                        txtTorisCode.Focus()
                        Exit Sub
                    End If

                    '���M�敪�`�F�b�N
                    If strSOUSIN_KBN <> "1" Then '�S�����
                        MessageBox.Show(MSG0336W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtTorisCode.Focus()
                        Exit Sub
                    End If
                End If

                '�X�P�W���[�����擾
                SQL.Length = 0
                SQL.Append("SELECT COUNT(*) COUNTER FROM TORIMAST,SCHMAST")
                If txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "999999999999" AndAlso txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "" Then
                    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(txtTorisCode.Text.Trim))
                    SQL.Append(" AND TORIF_CODE_T = " & SQ(txtTorifCode.Text.Trim))
                    SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
                    SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
                Else
                    SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_S ")
                    SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
                End If
                SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))
                SQL.Append(" AND SOUSIN_KBN_S = '1'")
                SQL.Append(" AND HAISIN_FLG_S = '1'")
                SQL.Append(" AND FUNOU_FLG_S = '1'")
                SQL.Append(" AND TYUUDAN_FLG_S = '0'")

                Dim Count As Integer
                If OraReader.DataReader(SQL) = True Then
                    Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                    OraReader.Close()
                Else
                    '�������s(MSG0002E)
                    MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Exit Sub
                End If

                If Count = 0 Then
                    '�X�P�W���[���Ȃ�(MSG0056W)
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateY.Focus()
                    Exit Sub
                End If
            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", ex.ToString)
                txtTorisCode.Focus()
                Exit Sub
            Finally
                If OraReader IsNot Nothing Then OraReader.Close()
                If MainDB IsNot Nothing Then MainDB.Close()
            End Try

            Dim PrintName As String = "�������ʊm�F�\(�s�\���ʍX�V�E��Ǝ���)"

            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer
            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            param = GCom.GetUserID & "," & strTORIS_CODE & "," & strTORIF_CODE & "," & FURI_DATE
            nRet = ExeRepo.ExecReport("KFJP013_2.EXE", param)

            '�߂�l�ɑΉ��������b�Z�[�W��\������
            Select Case nRet
                Case 0
                    '�����m�F���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '����ΏۂȂ����b�Z�[�W
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

        End Try

    End Sub
End Class
