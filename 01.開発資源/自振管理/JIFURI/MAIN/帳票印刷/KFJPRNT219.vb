Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT219
    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT219", "�X�ʏW�v�\������")
    Private Const msgTitle As String = "�X�ʏW�v�\������(KFJPRNT219)"
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

    '2010/10/05.Sakon�@ALL9�΍� ++++++++++
    Private TORICODE As New ArrayList
    Private FUNOUFLG As New ArrayList
    '+++++++++++++++++++++++++++++++++++++

#Region "�錾"
    Dim strTORIS_CODE As String
    Dim strTORIF_CODE As String
    Dim strFURI_DATE As String

    Public strFLG As String = ""
    Public strFMT As String = "" '�t�H�[�}�b�g�敪

#End Region

    Private Sub KFJPRNT219_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            Dim Msg As String
            Select Case GCom.SetComboBox(cmbPrintKbn, "KFJPRNT219_�Ώے��[.TXT", True)
                Case 1  '�t�@�C���Ȃ�
                    MessageBox.Show(String.Format(MSG0025E, "�Ώے��[", "KFJPRNT219_�Ώے��[.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "�Ώے��[�ݒ�t�@�C���Ȃ��B�t�@�C��:KFJPRNT219_�Ώے��[.TXT"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", Msg)
                Case 2  '�ُ�
                    MessageBox.Show(String.Format(MSG0026E, "�Ώے��["), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "�R���{�{�b�N�X�ݒ莸�s �R���{�{�b�N�X��:�Ώے��["
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�R���{�{�b�N�X�ݒ�)", "���s", Msg)
            End Select

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
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

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
            '--------------------------------
            '�}�X�^�`�F�b�N
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            Dim PrintID As String = ""
            Dim PrintName As String = ""
            Dim Funou As String = "0"
            Select Case GCom.GetComboBox(cmbPrintKbn)
                Case 0 '�ʏ�
                    PrintID = "KFJP019"
                    PrintName = "�����U�֓X�ʏW�v�\"
                    Funou = strFLG
                Case 1 '�������Z�p
                    Select Case strFMT
                        Case "51", "53"
                            PrintID = "KFJP219"
                            PrintName = "�����U�֓X�ʏW�v�\(�������Z)"
                            Funou = strFLG
                        Case Else
                            MessageBox.Show("�w��̒��[��ނł͈���ł��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                    End Select
                  
                Case 2 '�����s��

                    If strFMT <> "54" Then
                        MessageBox.Show("�w��̒��[��ނł͈���ł��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    Else
                        PrintID = "KFJP220"
                        PrintName = "�����U�֓X�ʏW�v�\(�����s��)"
                        Funou = strFLG
                    End If

            End Select

            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer
            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '2010/10/05.Sakon�@ALL9�Ή� +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim i As Integer

            For i = 0 To TORICODE.Count - 1
                '�p�����[�^�Z�b�g
                param = TORICODE(i).ToString & "," & strFURI_DATE & "," & FUNOUFLG(i).ToString
                nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

                '�߂�l�ɑΉ��������b�Z�[�W��\������(ALL9���͎��s���b�Z�[�W�̂ݕ\������)
                Select Case nRet
                    Case 0
                        If txtTorisCode.Text & txtTorifCode.Text <> "999999999999" Then
                            '�����m�F���b�Z�[�W
                            MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    Case -1
                        '����ΏۂȂ����b�Z�[�W
                        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select

            Next

            TORICODE.Clear()    '2010/11/11 ADD

            'ALL9���͂����Ŋ������b�Z�[�W��\������
            If txtTorisCode.Text & txtTorifCode.Text = "999999999999" Then
                '�����m�F���b�Z�[�W
                MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            ''param = GCom.GetUserID & "," & strTORIS_CODE + strTORIF_CODE & "," & strFURI_DATE & "," & Funou
            'param = strTORIS_CODE + strTORIF_CODE & "," & strFURI_DATE & "," & Funou
            'nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

            ''�߂�l�ɑΉ��������b�Z�[�W��\������
            'Select Case nRet
            '    Case 0
            '        '�����m�F���b�Z�[�W
            '        MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '    Case -1
            '        '����ΏۂȂ����b�Z�[�W
            '        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Case Else
            '        '������s���b�Z�[�W
            '        MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End Select
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            txtTorisCode.Focus()
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
            fn_check_text = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)", "���s", ex.ToString)
            Return False
        End Try
    End Function
    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :����{�^�����������Ƀ}�X�^���փ`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2009/09/19
        'Update         :2010/10/05�FALL9�Ή��ǉ�
        '============================================================================
        fn_check_Table = False
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        fn_check_Table = False
        Dim lngDataCNT As Long = 0
        Try
            '------------------------------------------
            '�����}�X�^���݃`�F�b�N
            '------------------------------------------
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                '�������擾
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
                    Return False
                End If
                If lngDataCNT = 0 Then
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Return False
                End If
            End If

            SQL = New StringBuilder(128)
            '------------------------------------------
            '�X�P�W���[���}�X�^���݃`�F�b�N
            '------------------------------------------
            '�o�^�ς݃X�P�W���[�������݂��邩�`�F�b�N
            SQL.Append(" SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            'SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            End If

            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
                MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Return False
            End If
            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            SQL = New StringBuilder(128)
            '------------------------------------------
            '���׃}�X�^���݃`�F�b�N
            '------------------------------------------
            SQL.Append(" SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST,MEIMAST,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            'SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_K")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_K")
            SQL.Append(" AND FURI_DATE_S = FURI_DATE_K")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '0���ް�������ΏۂƂ���ꍇ�A����SQL�͕s�v ============
            SQL.Append(" AND ((FMT_KBN_T <> '02' AND DATA_KBN_K = '2')")   '���ňȊO�̃f�[�^���R�[�h
            SQL.Append(" OR (FMT_KBN_T = '02' AND DATA_KBN_K = '3'))")     '���ł̃f�[�^���R�[�h
            '=======================================================
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            End If


            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
                MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Return False
            End If
            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            'ALL9�΍�F�ʏ펞�̓t�H�[�}�b�g�`�F�b�N���s��Ȃ� +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '�t�H�[�}�b�g�`�F�b�N
            If GCom.GetComboBox(cmbPrintKbn) <> 0 Then
                SQL = New StringBuilder(128)

                SQL.Append(" SELECT")
                SQL.Append(" FMT_KBN_T")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

                If OraReader.DataReader(SQL) = True Then
                    strFMT = OraReader.GetItem("FMT_KBN_T")
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
                    MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    txtFuriDateY.Focus()
                    Return False
                End If
                If strFMT.Trim = "" Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateY.Focus()
                    Return False
                End If
            End If
            'SQL = New StringBuilder(128)

            'SQL.Append(" SELECT")
            'SQL.Append(" FMT_KBN_T")
            'SQL.Append(" FROM TORIMAST")
            'SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            'SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

            'If OraReader.DataReader(SQL) = True Then
            '    strFMT = OraReader.GetItem("FMT_KBN_T")
            'Else
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
            '    MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    txtFuriDateY.Focus()
            '    Return False
            'End If
            'If strFMT.Trim = "" Then
            '    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    txtFuriDateY.Focus()
            '    Return False
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            '�p�����[�^���ڂ�ێ��i�����R�[�h�E�s�\�t���O�j
            SQL = New StringBuilder(128)

            '2017/06/22 saitou MODIFY �W���ŏC�� ---------------------------------------- START
            '�X�P�W���[�������݂��āA���׃}�X�^�����݂��Ȃ��ꍇ�i���n��̓���Ȃǁj�ɁA
            '���׃}�X�^�����݂��Ȃ�����悪�S��0���E0�~�ŏo�͂����̂��C���B
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("    ,TORIF_CODE_S")
            SQL.Append("    ,FUNOU_FLG_S")
            SQL.Append("    ,COUNT(*)")
            SQL.Append(" FROM")
            SQL.Append("     SCHMAST")
            SQL.Append("    ,TORIMAST")
            SQL.Append("    ,MEIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            End If
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_K")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_K")
            SQL.Append(" AND FURI_DATE_S = FURI_DATE_K")
            SQL.Append(" GROUP BY")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("    ,TORIF_CODE_S")
            SQL.Append("    ,FUNOU_FLG_S")
            SQL.Append(" HAVING")
            SQL.Append("     COUNT(*) > 0")
            SQL.Append(" ORDER BY")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("    ,TORIF_CODE_S")

            'SQL.Append(" SELECT")
            'SQL.Append(" TORIS_CODE_S")
            'SQL.Append(",TORIF_CODE_S")
            'SQL.Append(",FUNOU_FLG_S")
            'SQL.Append(" FROM SCHMAST")
            'SQL.Append(" WHERE ")
            'SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND TOUROKU_FLG_S = '1'")
            'If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
            '    SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
            '    SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            'End If
            '2017/06/22 saitou MODIFY --------------------------------------------------- END

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    TORICODE.Add(OraReader.GetItem("TORIS_CODE_S") & OraReader.GetItem("TORIF_CODE_S"))
                    FUNOUFLG.Add(OraReader.GetItem("FUNOU_FLG_S"))
                    OraReader.NextRead()
                End While
                'strFLG = OraReader.GetItem("FUNOU_FLG_S")
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", "�}�X�^�������s")
                MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Return False
            End If
            'If strFLG = "" Then
            If TORICODE.Count = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            fn_check_Table = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�e�[�u���`�F�b�N)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
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
