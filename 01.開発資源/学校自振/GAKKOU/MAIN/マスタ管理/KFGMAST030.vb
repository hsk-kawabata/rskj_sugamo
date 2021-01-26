Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Public Class KFGMAST030

#Region " ���ʕϐ���` "
    Private Str_Spread1(14, 6) As String
    Private Str_Spread2(12, 14) As String

    '���ݗL���ȃ^�u����ݒ�
    Private Int_TabFlag As Integer
    Private tab_Ctrl(1) As TabPage

    Private Str_HName() As String

    Private Str_Work(,) As String
    Private blnEnter_Check As Boolean
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
    Private intMaxHimokuCnt As Integer = CInt(IIf(STR_HIMOKU_PTN = "1", 15, 10))
    '2017/02/22 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST030", "��ڃ}�X�^�����e�i���X���")
    Private Const msgTitle As String = "��ڃ}�X�^�����e�i���X���(KFGMAST030)"
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "��ڃ}�X�^�����e�i���X���"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            If fn_SetDataGridViewKamokuCmbFromText() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�")
                MessageBox.Show(String.Format(MSG0013E, "�Ȗ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            TabControl1.TabPages(0).Enabled = False
            TabControl1.TabPages(1).Enabled = False

            '���͋֎~����
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")

                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Sub
            End If

            Int_TabFlag = 0

            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
            DataGridView1.Rows.Add(intMaxHimokuCnt)
            'DataGridView1.Rows.Add(10)
            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
            DataGridView2.Rows.Add(12)
            '2017/02/23 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
            If STR_HIMOKU_PTN = "1" Then
                '��ڂP�P�`�P�T����\���̂��ߕ\������
                For i As Integer = 10 To intMaxHimokuCnt - 1
                    DataGridView2.Columns(i).Visible = True
                Next
            End If
            '2017/02/23 �^�X�N�j���� ADD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�J�n", "����", "")
            LW.ToriCode = txtGAKKOU_CODE.Text
            '��{���ړ��͒l�`�F�b�N
            '����LostFocus���ɂ����Ă��邪
            '���ڏ����{�^�������������Ƃ��̑Ώ�
            If PFUNC_Hisuu_Check() = False Then
                Exit Sub
            End If

            '�d���`�F�b�N
            STR_SQL = " SELECT "
            STR_SQL += " GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H"
            STR_SQL += " FROM HIMOMAST"
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " AND HIMOKU_ID_H ='" & cmbHimoku.Text & "'"

            If GFUNC_ISEXIST(STR_SQL) = True Then
                MessageBox.Show(G_MSG0047W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If

            Select Case Int_TabFlag
                Case 0
                    '��ږ����ό����^�u

                    '���͒l�`�F�b�N
                    If PFUNC_Nyuryoku_Check() = False Then
                        Exit Sub
                    End If

                    '�m�F���b�Z�[�W
                    If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If

                    '�f�[�^�o�^
                    Call PSUB_Spread1_Insert()

                    '�f�[�^�o�^��A�ꕔ���͓��e���N���A
                    Call PSUB_Nyuuryoku_Clear(1)

                    '�R���{�č쐬
                    STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                    STR_SQL += " ORDER BY HIMOKU_ID_H "

                    Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)

                Case 1
                    '���ʋ��z�^�u
                    If Trim(txtHIMOKU_PATERN.Text) = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "��ږ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Focus()
                        Exit Sub
                    End If

                    '�m�F���b�Z�[�W
                    If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If

                    '�f�[�^�o�^
                    '�g�����U�N�V�����f�[�^����J�n
                    Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)
                    If PSUB_Spread2_Insert() = False Then
                        '���[���o�b�N
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                    Else
                        '�g�����U�N�V�����f�[�^�����m��
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)
                        MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                    '�f�[�^�o�^��A�ꕔ���͓��e���N���A
                    Call PSUB_Nyuuryoku_Clear(1)
                    Call PSUB_Nyuuryoku_Clear(4)

                    '�R���{�č쐬
                    STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                    STR_SQL += " ORDER BY HIMOKU_ID_H "

                    Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)
            End Select

            cmbHimoku.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")
            LW.ToriCode = txtGAKKOU_CODE.Text

            Select Case Int_TabFlag
                Case 0
                    '��ږ����ό����^�u

                    '���͒l�`�F�b�N
                    If PFUNC_Nyuryoku_Check() = False Then
                        Exit Sub
                    End If

                    '�m�F���b�Z�[�W
                    If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If

                    '�f�[�^�X�V
                    Call PSUB_Spread_Update()
                Case 1
                    '���ʋ��z�^�u
                    If Trim(txtHIMOKU_PATERN.Text) = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "��ږ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Focus()
                        Exit Sub
                    End If
                    '�m�F���b�Z�[�W
                    If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If
                    If Trim(txtHIMOKU_PATERN.Text) = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "��ږ�"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Focus()
                        Exit Sub
                    End If

                    '�g�����U�N�V�����f�[�^����J�n
                    Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)
                    '�f�[�^�폜
                    If PSUB_Spread_Delete() = False Then
                        '���[���o�b�N
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                        Return
                    End If
                    '�f�[�^�o�^
                    If PSUB_Spread2_Insert() = False Then
                        '���[���o�b�N
                        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                        Return
                    End If
                    '�g�����U�N�V�����f�[�^�����m��
                    Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    '�R���{�č쐬
                    STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                    STR_SQL += " ORDER BY HIMOKU_ID_H "

                    Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)
            End Select
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�J�n", "����", "")
            LW.ToriCode = txtGAKKOU_CODE.Text

            If PFUNC_Hisuu_Check() = False Then
                Exit Sub
            End If

            '�m�F���b�Z�[�W
            Select Case Int_TabFlag
                Case 0
                    If MessageBox.Show(G_MSG0017I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If
                Case 1
                    If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Exit Sub
                    End If
            End Select

            '2007/02/12�@�K�{���ڃ`�F�b�N
            If cmbHimoku.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "���ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbHimoku.Focus()
                Exit Sub
            End If

            '�g�����U�N�V�����f�[�^����J�n
            Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)
            '�f�[�^�폜
            If PSUB_Spread_Delete() = False Then
                '���[���o�b�N
                Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
                Return
            End If
            '�g�����U�N�V�����f�[�^�����m��
            Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)

            '�R���{�č쐬
            STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " GROUP BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
            STR_SQL += " ORDER BY HIMOKU_ID_H "

            Call PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName)

            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '�f�[�^�폜��A���͓��e���N���A
            Call PSUB_Nyuuryoku_Clear(0)
            txtGAKUNEN_CODE.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�폜)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")
            LW.ToriCode = txtGAKKOU_CODE.Text

            Dim Str_Sql_local As String

            '���͒l�`�F�b�N
            If PFUNC_Hisuu_Check() = False Then

                Exit Sub
            End If

            '���݃`�F�b�N
            STR_SQL = " SELECT *"
            STR_SQL += " FROM HIMOMAST"
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " AND HIMOKU_ID_H = '" & cmbHimoku.Text & "'"

            Select Case cmbHimoku.Text
                Case "000"
                    Int_TabFlag = 0
                    '��ږ����ό���
                    Call PFUNC_SET_SPDDATA(STR_SQL)

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        '�f�[�^�o�^��A�ꕔ���͓��e���N���A
                        Call PSUB_Nyuuryoku_Clear(2)
                    Else
                        Call PSUB_Nyuuryoku_Clear(3)
                    End If

                    Me.DataGridView1.Focus()
                Case Else
                    '���ʋ��z
                    Int_TabFlag = 1

                    For intRow As Integer = 0 To 11
                        '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                        For intCol As Integer = 0 To intMaxHimokuCnt - 1
                            'For intCol As Integer = 0 To 9 '��ڂ͂P�O�܂�
                            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                            DataGridView2.Rows(intRow).Cells(intCol).Value = ""
                            DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                            DataGridView2.Columns(intCol).HeaderText = "���" & StrConv(CStr(intCol + 1), VbStrConv.Wide)
                        Next intCol
                    Next intRow

                    Str_Sql_local = " SELECT * "
                    Str_Sql_local += " FROM HIMOMAST"
                    Str_Sql_local += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
                    Str_Sql_local += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    Str_Sql_local += " AND HIMOKU_ID_H = '000'"

                    Call PFUNC_SET_SPDDATA(Str_Sql_local)

                    Call PFUNC_SET_SPDDATA2(STR_SQL)

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        Call PSUB_Nyuuryoku_Clear(5)
                    Else
                        Call PSUB_Nyuuryoku_Clear(6)
                    End If

                    DataGridView1.Focus()
            End Select
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
        End Try

    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")
            Call PSUB_Nyuuryoku_Clear(0)
            '2007/02/12�@��ʃN���A��A�w�Z�R�[�h�Ƀt�H�[�J�X�����Ă�
            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)�J�n", "����", "")

            Me.Close()
        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)��O�G���[", "���s", ex.Message)

        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�I��)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim OraDB As New MyOracle
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            '2012/04/13 saitou �W���C�� ADD ---------------------------------------->>>>
            '�e���ڂ̓��̓`�F�b�N��ǉ�
            If Me.txtGAKKOU_CODE.Text.Trim = String.Empty Then
                '�w�Z�R�[�h���󔒂Ȃ�΁A�w�N�R�[�h�Ɣ��ID���󔒂ł��邱��
                If Me.txtGAKUNEN_CODE.Text.Trim <> String.Empty Then
                    MessageBox.Show(G_MSG0048W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtGAKKOU_CODE.Focus()
                    Return
                End If
                If Me.cmbHimoku.Text.Trim <> String.Empty Then
                    MessageBox.Show(G_MSG0049W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtGAKKOU_CODE.Focus()
                    Return
                End If
            End If
            '2012/04/13 saitou �W���C�� ADD ----------------------------------------<<<<

            '�w�Z�R�[�h
            If Trim(txtGAKKOU_CODE.Text) <> "" Then

                '�w�Z�}�X�^���݃`�F�b�N
                STR_SQL = " SELECT GAKKOU_CODE_G "
                STR_SQL += " FROM GAKMAST1"
                STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

                If GFUNC_ISEXIST(STR_SQL) = False Then

                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    lblGAKKOU_NAME.Text = "" '2007/02/14
                    Exit Sub
                End If
            End If

            '�w�N
            If Trim(txtGAKUNEN_CODE.Text) <> "" Then

                Select Case CInt(txtGAKUNEN_CODE.Text)
                    Case 0
                        MessageBox.Show(G_MSG0050W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKUNEN_CODE.Focus()

                        Exit Sub
                End Select
                Dim sSiyou_Gakunen As String
                '�g�p�w�N�`�F�b�N
                STR_SQL = " SELECT SIYOU_GAKUNEN_T"
                STR_SQL += " FROM GAKMAST2"
                STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

                sSiyou_Gakunen = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SIYOU_GAKUNEN_T")

                If Trim(sSiyou_Gakunen) = "" AndAlso Trim(txtGAKKOU_CODE.Text) <> "" Then
                    MessageBox.Show(G_MSG0051W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()
                    Exit Sub
                End If

                Select Case CInt(sSiyou_Gakunen)
                    Case Is < CInt(txtGAKUNEN_CODE.Text)
                        '�g�p�ō��w�N�ȏ�̊w�N�����
                        MessageBox.Show(G_MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKUNEN_CODE.Focus()
                        Exit Sub
                End Select
            End If

            '���ID
            If Trim(cmbHimoku.Text) <> "" Then
                '���ID�w�莞�Ɋw�N���󔒂̏ꍇ�̓G���[�Ƃ��� 2007/02/14
                If Trim(txtGAKUNEN_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "�w�N"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()

                    Exit Sub
                End If
            End If

            '����p���[�N�e�[�u���쐬
            If PFUNC_Print_Work() = False Then
                Exit Sub
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            Dim nRet As Integer
            Dim Param As String

            OraReader = New MyOracleReader(OraDB)

            SQL.Append("SELECT DISTINCT GAKKOU_CODE_H FROM MAST0300G_WORK")

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "�Q��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            If MessageBox.Show(String.Format(MSG0013I, "��ڃ}�X�^�ꗗ"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            While OraReader.EOF = False
                Param = GCom.GetUserID & "," & OraReader.GetString("GAKKOU_CODE_H")
                nRet = ExeRepo.ExecReport("KFGP010.EXE", Param)
                '�߂�l�ɑΉ��������b�Z�[�W��\������
                Select Case nRet
                    Case 0
                    Case Else
                        '������s���b�Z�[�W
                        MessageBox.Show(String.Format(MSG0004E, "��ڃ}�X�^�ꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                OraReader.NextRead()
            End While

            MessageBox.Show(String.Format(MSG0014I, "��ڃ}�X�^�ꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            btnPrint.Focus()
        Catch ex As Exception
            LW.ToriCode = "000000000000"
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�I��", "����", "")
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Sub
#End Region


    Private Sub PSUB_Spread1_Insert()

        With DataGridView1
            STR_SQL = " INSERT INTO HIMOMAST VALUES ("
            STR_SQL += "'" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += "," & txtGAKUNEN_CODE.Text
            '��ږ��E���ό����̓o�^�͔�ڃR�[�h���w000�x�A��ږ��w���ό����x�Œ�
            STR_SQL += ",'" & cmbHimoku.Text & "'"
            STR_SQL += ",'" & txtHIMOKU_PATERN.Text & "'"
            '�������͎b��Łw  �x��ݒ�
            STR_SQL += ",'" & Space(2) & "'"

            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
            For intCnt As Integer = 0 To intMaxHimokuCnt - 1
                'For intCnt As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                '��ږ������͂���Ă��Ȃ���Ύ��̍s�̓��͒l�`�F�b�N���s��
                If IsDBNull(Trim(.Rows(intCnt).Cells(0).Value)) = False And Trim(.Rows(intCnt).Cells(0).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(0).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(2).Value)) = False And Trim(.Rows(intCnt).Cells(2).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(2).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(3).Value)) = False And Trim(.Rows(intCnt).Cells(3).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(3).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If

                If IsDBNull(.Rows(intCnt).Cells(4).Value) = False And Trim(.Rows(intCnt).Cells(4).Value) <> "" Then
                    STR_SQL += ",'" & fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If

                If IsDBNull(Trim(.Rows(intCnt).Cells(5).Value)) = False And Trim(.Rows(intCnt).Cells(5).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(5).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(6).Value)) = False And Trim(.Rows(intCnt).Cells(6).Value) <> "" Then
                    STR_SQL += ",'" & Trim(.Rows(intCnt).Cells(6).Value) & "'"
                Else
                    STR_SQL += ",''"
                End If
                If IsDBNull(Trim(.Rows(intCnt).Cells(1).Value)) = False And Trim(.Rows(intCnt).Cells(1).Value) <> "" Then
                    If Trim(.Rows(intCnt).Cells(1).Value) <> "" Then
                        STR_SQL += "," & Format(CLng(Trim(.Rows(intCnt).Cells(1).Value)), "###0")
                    Else
                        STR_SQL += ",0"
                    End If
                Else
                    STR_SQL += ",0"
                End If
            Next intCnt

            '2017/04/06 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
            For intCnt As Integer = intMaxHimokuCnt To 14
                STR_SQL += ",'','','','','','',0"
            Next
            ''2007/04/04�@���11�`15�̕��͋󗓁E0�œ��͂���
            'For intCnt As Integer = 0 To 4
            '    STR_SQL += ",'','','','','','',0"
            'Next
            '2017/04/06 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END

            STR_SQL += ",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'"
            STR_SQL += ",'00000000'"
            '�\�����ڒǉ�
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ",'" & Space(50) & "'"
            STR_SQL += ")"
        End With

        '�g�����U�N�V�����f�[�^����J�n
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)

        '�g�����U�N�V�����f�[�^�������s
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

            '�f�[�^�������G���[
            MessageBox.Show(String.Format(MSG0002E, "�}��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        '�g�����U�N�V�����f�[�^�����m��
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)

        MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)


    End Sub
    Private Function PSUB_Spread2_Insert() As Boolean
        PSUB_Spread2_Insert = False
        With DataGridView2
            For intRow As Integer = 0 To 11
                STR_SQL = " INSERT INTO HIMOMAST VALUES ("
                STR_SQL += "'" & txtGAKKOU_CODE.Text & "'"
                STR_SQL += "," & txtGAKUNEN_CODE.Text
                STR_SQL += ",'" & cmbHimoku.Text & "'"
                STR_SQL += ",'" & txtHIMOKU_PATERN.Text & "'"
                Select Case intRow
                    Case 0 To 8
                        STR_SQL += ",'" & Format((intRow + 4), "00") & "'"
                    Case 9 To 11
                        STR_SQL += ",'" & Format((intRow - 8), "00") & "'"
                End Select

                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                For intCol As Integer = 0 To intMaxHimokuCnt - 1
                    'For intCol As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    If IsDBNull(Trim(.Rows(intRow).Cells(intCol).Value)) = False Then
                        '��ځw000�x�̏���ݒ肷��i���z�ȊO�j
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 0)) & "'"
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 2)) & "'"
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 3)) & "'"
                        '2014/05/15 saitou �W���C�� MODIFY ----------------------------------------------->>>>
                        If Trim(Str_Spread1(intCol, 4)) = "0" Then
                            '�Ȗ�0�͊i�[���Ȃ�
                            STR_SQL += ",''"
                        Else
                            STR_SQL += ",'" & Trim(Str_Spread1(intCol, 4)) & "'"
                        End If
                        'STR_SQL += ",'" & Trim(Str_Spread1(intCol, 4)) & "'"
                        '2014/05/15 saitou �W���C�� MODIFY -----------------------------------------------<<<<
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 5)) & "'"
                        STR_SQL += ",'" & Trim(Str_Spread1(intCol, 6)) & "'"
                        '���z�ɒl���Ȃ��ꍇ�̓[����ݒ肷��
                        If Trim(.Rows(intRow).Cells(intCol).Value) <> "" Then
                            STR_SQL += "," & Format(CLng(Trim(.Rows(intRow).Cells(intCol).Value)), "###0")
                        Else
                            STR_SQL += ",0"
                        End If
                    Else
                        STR_SQL += ",'','','','','','',0"
                    End If
                Next

                '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                For intCol As Integer = intMaxHimokuCnt To 14
                    STR_SQL += ",'','','','','','',0"
                Next
                ''2007/04/04�@���11�`15�̕��͋󗓁E0�œ��͂���
                'For intCol As Integer = 0 To 4
                '    STR_SQL += ",'','','','','','',0"
                'Next
                '2017/04/06 �^�X�N�j���� UPD �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END

                STR_SQL += ",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'"
                STR_SQL += ",'00000000'"
                '�\�����ڒǉ�
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ",'" & Space(50) & "'"
                STR_SQL += ")"

                '�g�����U�N�V�����f�[�^�������s
                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

                    '�f�[�^�������G���[
                    MessageBox.Show(String.Format(MSG0002E, "�}��"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Exit Function
                End If
            Next
        End With
        PSUB_Spread2_Insert = True

    End Function
    Private Function PSUB_Spread_Delete() As Boolean
        PSUB_Spread_Delete = False
        '�폜����

        '�m�F���b�Z�[�W

        'SQL���쐬
        '��ʂɐݒ肳��Ă���w�Z�R�[�h�A�w�N�R�[�h�A���ID�������ɔ�ڃ}�X�^���폜����
        STR_SQL = " DELETE  FROM HIMOMAST"
        STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"

        '��ږ��E���ό������گĂ̍폜���́A��ځw000�x�ȊO��ں��ނ��폜�ΏۂƂ����
        '�w�Z���ށA�w�N���ނ݂̂ō폜��������
        Select Case Int_TabFlag
            Case 1
                STR_SQL += " AND HIMOKU_ID_H = '" & cmbHimoku.Text & "'"
        End Select

        '�g�����U�N�V�����f�[�^�������s
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

            '�f�[�^�������G���[
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        '�������b�Z�[�W

        PSUB_Spread_Delete = True
    End Function
    Private Sub PSUB_Spread_Update()

        Dim Str_Sql1 As String
        Dim Str_Sql2 As String

        With Me.DataGridView1
            Str_Sql1 = ""
            Str_Sql2 = ""

            STR_SQL = " UPDATE  HIMOMAST SET "
            STR_SQL += " KOUSIN_DATE_H='" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'"

            Str_Sql1 += STR_SQL
            Str_Sql2 += STR_SQL

            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
            For intCnt As Integer = 0 To intMaxHimokuCnt - 1
                'For intCnt As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                '��ږ������͂���Ă��Ȃ���Ύ��̍s�̓��͒l�`�F�b�N���s��
                '�C���O�ƏC����ŕύX������ꍇ�ɂ̂ݏC��
                If IsDBNull(Trim(.Rows(intCnt).Cells(0).Value)) = False Then
                    If Trim(.Rows(intCnt).Cells(0).Value) <> "" Then
                        STR_SQL = ""
                        If Trim(.Rows(intCnt).Cells(0).Value) <> Trim(Str_Spread1(intCnt, 0)) Then
                            STR_SQL += ",HIMOKU_NAME" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(0).Value) & "'"
                            Str_Spread1(intCnt, 0) = Trim(.Rows(intCnt).Cells(0).Value)
                        End If
                        If Trim(.Rows(intCnt).Cells(2).Value) <> Trim(Str_Spread1(intCnt, 2)) Then
                            STR_SQL += ",KESSAI_KIN_CODE" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(2).Value) & "'"
                            Str_Spread1(intCnt, 2) = Trim(.Rows(intCnt).Cells(2).Value)
                        End If
                        If Trim(.Rows(intCnt).Cells(3).Value) <> Trim(Str_Spread1(intCnt, 3)) Then
                            STR_SQL += ",KESSAI_TENPO" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(3).Value) & "'"
                            Str_Spread1(intCnt, 3) = Trim(.Rows(intCnt).Cells(3).Value)
                        End If

                        If fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value) <> Trim(Str_Spread1(intCnt, 4)) Then

                            STR_SQL += ",KESSAI_KAMOKU" & Format((intCnt + 1), "00") & "_H ='" & fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value) & "'"

                            Str_Spread1(intCnt, 4) = CStr(GFUNC_NAME_TO_CODE2(STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT, Trim(.Rows(intCnt).Cells(4).Value)))
                        End If
                        If Trim(.Rows(intCnt).Cells(5).Value) <> Trim(Str_Spread1(intCnt, 5)) Then
                            STR_SQL += ",KESSAI_KOUZA" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(5).Value) & "'"
                            Str_Spread1(intCnt, 5) = Trim(.Rows(intCnt).Cells(5).Value)
                        End If
                        If Trim(.Rows(intCnt).Cells(6).Value) <> Trim(Str_Spread1(intCnt, 6)) Then
                            STR_SQL += ",KESSAI_MEIGI" & Format((intCnt + 1), "00") & "_H ='" & Trim(.Rows(intCnt).Cells(6).Value) & "'"
                            Str_Spread1(intCnt, 6) = Trim(.Rows(intCnt).Cells(6).Value)
                        End If

                        '2006/10/24�@���z���󔒑΍�
                        If Trim(.Rows(intCnt).Cells(1).Value) <> "" Then
                            Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =" & Format(CLng(Trim(.Rows(intCnt).Cells(1).Value)), "###0")
                        Else
                            Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =" & "0"
                        End If

                        Str_Sql2 += STR_SQL
                    Else
                        If Trim(Str_Spread1(intCnt, 0)) <> "" Then
                            STR_SQL = ",HIMOKU_NAME" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_KIN_CODE" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_TENPO" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_KAMOKU" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_KOUZA" & Format((intCnt + 1), "00") & "_H =''"
                            STR_SQL += ",KESSAI_MEIGI" & Format((intCnt + 1), "00") & "_H =''"

                            Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =0"
                            Str_Sql2 += STR_SQL
                        End If
                    End If
                Else
                    If Trim(Str_Spread1(intCnt, 0)) <> "" Then
                        STR_SQL = ",HIMOKU_NAME" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_KIN_CODE" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_TENPO" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_KAMOKU" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_KOUZA" & Format((intCnt + 1), "00") & "_H =''"
                        STR_SQL += ",KESSAI_MEIGI" & Format((intCnt + 1), "00") & "_H =''"

                        Str_Sql1 += ",HIMOKU_KINGAKU" & Format((intCnt + 1), "00") & "_H =0"
                        Str_Sql2 += STR_SQL
                    End If
                End If
            Next intCnt
            STR_SQL = " WHERE "
            STR_SQL += " GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H =" & txtGAKUNEN_CODE.Text

            Str_Sql1 += STR_SQL & " AND HIMOKU_ID_H ='" & cmbHimoku.Text & "'"
            Str_Sql2 += STR_SQL
            '            STR_SQL += " AND TUKI_NO_H ='" & Space(2) & "'"
        End With

        '�g�����U�N�V�����f�[�^����J�n
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 0)

        '�g�����U�N�V�����f�[�^�������s
        If GFUNC_EXECUTESQL_TRANS(Str_Sql1, 1) = False Then

            '�f�[�^�������G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '���[���o�b�N
            Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 3)
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(Str_Sql2, 1) = False Then

            '�f�[�^�������G���[
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Sub
        End If

        '�g�����U�N�V�����f�[�^�����m��
        Call GFUNC_EXECUTESQL_TRANS(STR_SQL, 2)

        MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)


    End Sub
    Private Sub PSUB_Nyuuryoku_Clear(ByVal pIndex As Integer)
        '�����p�w�Z�������ޯ��
        Select Case pIndex
            Case 0
                '���I�����
                cmbGAKKOUNAME.SelectedIndex = -1
                cmbGAKKOUNAME.Enabled = True
                cmbKana.Enabled = True
            Case 5, 6
                cmbGAKKOUNAME.Enabled = False
                cmbKana.Enabled = False
        End Select

        '2007/02/12�@�w�Z�R�[�h�E�w�N�R�[�h�͏��������Ȃ�

        '�w�Z�R�[�h÷���ޯ��+�w�Z�����x��
        Select Case pIndex
            Case 0
                txtGAKKOU_CODE.Enabled = True
            Case 5, 6
                txtGAKKOU_CODE.Enabled = False
        End Select

        '�w�N÷���ޯ��
        Select Case pIndex
            Case 0
                txtGAKUNEN_CODE.Enabled = True
            Case 5, 6
                txtGAKUNEN_CODE.Enabled = False
        End Select

        '��ڃR�[�h�����ޯ��+��ږ�÷���ޯ��
        Select Case pIndex
            Case 0, 1, 4
                cmbHimoku.Enabled = True
                cmbHimoku.Text = ""
                cmbHimoku.Items.Clear()
                ReDim Str_HName(0)

                txtHIMOKU_PATERN.Enabled = True
                txtHIMOKU_PATERN.Text = ""
            Case 2, 3
                txtHIMOKU_PATERN.Enabled = False
            Case 5
                txtGAKUNEN_CODE.Enabled = False
                txtHIMOKU_PATERN.Enabled = True
        End Select

        '��ږ��E���ό������گ�
        Select Case pIndex
            Case 0, 1
                TabControl1.SelectedIndex = 0
                TabControl1.TabPages(0).Enabled = False

                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    For intCol As Integer = 0 To 6
                        DataGridView1.Rows(intRow).Cells(intCol).Value = ""
                        DataGridView1.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow
            Case 2, 3
                TabControl1.SelectedIndex = 0

                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    For intCol As Integer = 0 To 6
                        DataGridView1.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow


                TabControl1.TabPages(0).Enabled = True
            Case 5, 6

                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    For intCol As Integer = 0 To 6
                        DataGridView1.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow

                TabControl1.TabPages(0).Enabled = False
        End Select

        '���ʋ��z���گ�
        Select Case pIndex
            Case 0, 4
                TabControl1.TabPages(1).Enabled = False
                For intRow As Integer = 0 To 11
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                    For intCol As Integer = 0 To intMaxHimokuCnt - 1
                        'For intCol As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                        '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                        DataGridView2.Rows(intRow).Cells(intCol).Value = ""
                        DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                        DataGridView2.Columns(intCol).HeaderText = "���" & StrConv(CStr(intCol + 1), VbStrConv.Wide)

                    Next intCol
                Next intRow
            Case 2, 3
                For intRow As Integer = 0 To 11
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                    For intCol As Integer = 0 To intMaxHimokuCnt - 1
                        'For intCol As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                        '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                        DataGridView2.Rows(intRow).Cells(intCol).Value = ""
                        DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                        DataGridView2.Columns(intCol).HeaderText = "���" & StrConv(CStr(intCol + 1), VbStrConv.Wide)
                    Next intCol
                Next intRow

                TabControl1.TabPages(1).Enabled = False
            Case 5, 6
                TabControl1.SelectedIndex = 1

                For intRow As Integer = 0 To 11
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                    For intCol As Integer = 0 To intMaxHimokuCnt - 1
                        'For intCol As Integer = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                        '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                        DataGridView2.Rows(intRow).Cells(intCol).Style.BackColor = Color.White
                    Next intCol
                Next intRow

                TabControl1.TabPages(1).Enabled = True
        End Select

        '�o�^�{�^��
        Select Case pIndex
            Case 0, 2, 5
                btnAdd.Enabled = True
            Case 3, 6
                btnAdd.Enabled = False
        End Select

        '�C���E�폜�{�^��
        Select Case pIndex
            Case 0, 1, 2, 4, 5
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
            Case 3, 6
                btnUPDATE.Enabled = True
                btnDelete.Enabled = True
        End Select

    End Sub
    Private Function PFUNC_SET_SPDDATA(ByVal pSql As String) As Boolean

        PFUNC_SET_SPDDATA = False

        If Trim(pSql) = "" Then
            Exit Function
        End If

        If GFUNC_SELECT_SQL2(pSql, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        OBJ_DATAREADER.Read()

        ReDim Str_Spread1(14, 6)

        '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
        For intCnt As Integer = 1 To intMaxHimokuCnt
            'For intCnt As Integer = 1 To 10 '2007/04/04�@��ڂ͂P�O�܂� 
            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
            If IsDBNull(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCnt, "00") & "_H")) = True Then
                With DataGridView1
                    '��ږ�
                    .Rows(intCnt - 1).Cells(0).Value = ""
                    Str_Spread1((intCnt - 1), 0) = ""

                    '���z
                    .Rows(intCnt - 1).Cells(1).Value = ""
                    Str_Spread1((intCnt - 1), 1) = 0

                    '���Z�@�փR�[�h
                    .Rows(intCnt - 1).Cells(2).Value = ""
                    Str_Spread1((intCnt - 1), 2) = ""

                    '�x�X�R�[�h
                    .Rows(intCnt - 1).Cells(3).Value = ""
                    Str_Spread1((intCnt - 1), 3) = ""

                    '�Ȗ�
                    .Rows(intCnt - 1).Cells(4).Value = ""
                    Str_Spread1((intCnt - 1), 4) = 0

                    '�����ԍ�
                    .Rows(intCnt - 1).Cells(5).Value = ""
                    Str_Spread1((intCnt - 1), 5) = ""

                    '�������`�l(��)
                    .Rows(intCnt - 1).Cells(6).Value = ""
                    Str_Spread1((intCnt - 1), 6) = ""
                End With

            Else
                With DataGridView1
                    '��ږ�
                    Dim HimokaName As String = ConvNullToString(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCnt, "00") & "_H")).Trim
                    .Rows(intCnt - 1).Cells(0).Value = HimokaName
                    Str_Spread1((intCnt - 1), 0) = HimokaName
                    '���z
                    Dim Kingaku As String = ConvNullToString(OBJ_DATAREADER.Item("HIMOKU_KINGAKU" & Format(intCnt, "00") & "_H")).Trim
                    .Rows(intCnt - 1).Cells(1).Value = Format(CDbl(Kingaku), "#,##0")
                    Str_Spread1((intCnt - 1), 1) = Kingaku
                    Dim KinCode As String = ConvNullToString(OBJ_DATAREADER.Item("KESSAI_KIN_CODE" & Format(intCnt, "00") & "_H")).Trim
                    '���Z�@�փR�[�h
                    .Rows(intCnt - 1).Cells(2).Value = KinCode
                    Str_Spread1((intCnt - 1), 2) = KinCode
                    Dim SitCode As String = ConvNullToString(OBJ_DATAREADER.Item("KESSAI_TENPO" & Format(intCnt, "00") & "_H")).Trim
                    '�x�X�R�[�h
                    .Rows(intCnt - 1).Cells(3).Value = SitCode
                    Str_Spread1((intCnt - 1), 3) = SitCode

                    '�Ȗ�
                    .Rows(intCnt - 1).Cells(4).Value = fn_GetKamokuNameFromKamokuCode(OBJ_DATAREADER.Item("KESSAI_KAMOKU" & Format(intCnt, "00") & "_H"))
                    Str_Spread1((intCnt - 1), 4) = CStr(OBJ_DATAREADER.Item("KESSAI_KAMOKU" & Format(intCnt, "00") & "_H"))
                    Dim Kouza As String = ConvNullToString(OBJ_DATAREADER.Item("KESSAI_KOUZA" & Format(intCnt, "00") & "_H")).Trim
                    '�����ԍ�
                    .Rows(intCnt - 1).Cells(5).Value = Kouza
                    Str_Spread1((intCnt - 1), 5) = Kouza

                    '�������`�l(��)
                    If IsDBNull(OBJ_DATAREADER.Item("KESSAI_MEIGI" & Format(intCnt, "00") & "_H")) = False Then
                        .Rows(intCnt - 1).Cells(6).Value = Trim(CStr(OBJ_DATAREADER.Item("KESSAI_MEIGI" & Format(intCnt, "00") & "_H")))
                        Str_Spread1((intCnt - 1), 6) = Trim(CStr(OBJ_DATAREADER.Item("KESSAI_MEIGI" & Format(intCnt, "00") & "_H")))
                    Else
                        .Rows(intCnt - 1).Cells(6).Value = ""
                        Str_Spread1((intCnt - 1), 6) = ""
                    End If

                End With
            End If
        Next intCnt

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_SET_SPDDATA = True

    End Function
    Private Function PFUNC_SET_SPDDATA2(ByVal pSql As String) As Boolean
        Dim intCnt As Integer
        Dim intCol As Integer
        Dim intRow As Integer

        Dim intTuki As Integer

        PFUNC_SET_SPDDATA2 = False

        If Trim(pSql) = "" Then
            Exit Function
        End If

        If GFUNC_SELECT_SQL2(pSql, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            With DataGridView2
                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                For intCnt = 0 To intMaxHimokuCnt - 1
                    'For intCnt = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                    '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                    If Trim(Str_Spread1((intCnt), 0)) = "" Then
                        .Columns(intCnt).HeaderText = "���" & StrConv(CStr(intCnt + 1), VbStrConv.Wide)
                        .Columns(intCnt).ReadOnly = True
                    Else
                        .Columns(intCnt).HeaderText = Trim(Str_Spread1((intCnt), 0))
                        .Columns(intCnt).ReadOnly = False
                        For intRow = 0 To 11
                            .Rows(intRow).Cells(intCnt).ReadOnly = False
                            If IsNumeric(CStr(Trim(Str_Spread1((intCnt), 1)))) Then
                                .Rows(intRow).Cells(intCnt).Value = Format(CDbl(Trim(Str_Spread1((intCnt), 1))), "#,##0")
                            Else
                                .Rows(intRow).Cells(intCnt).Value = CStr(Trim(Str_Spread1((intCnt), 1)))
                            End If
                        Next intRow
                    End If
                Next intCnt
            End With

            Exit Function
        End If

        With DataGridView2
            While (OBJ_DATAREADER.Read = True)
                If IsDBNull(OBJ_DATAREADER.Item("TUKI_NO_H")) = True Then
                    Exit While
                End If

                intTuki = CInt(OBJ_DATAREADER.Item("TUKI_NO_H"))

                Select Case intTuki
                    Case 1 To 3
                        '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                        For intCol = 0 To intMaxHimokuCnt - 1
                            'For intCol = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                            '��ږ������͂���Ă���Δ�ږ����w�b�_���̂ɐݒ肵�A
                            '�Ώې������s�̋��z�Z���ɋ��z��}��
                            '���͂���Ă��Ȃ���΃f�t�H���g�l��ݒ�
                            If IsDBNull(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")) = True Then
                                .Columns(intCol).HeaderText = "���" & StrConv(CStr(intCol + 1), VbStrConv.Wide)
                                .Rows(intTuki + 8).Cells(intCol).ReadOnly = True
                            Else
                                .Columns(intCol).HeaderText = Trim(CStr(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")))
                                .Rows(intTuki + 8).Cells(intCol).ReadOnly = False
                                .Rows(intTuki + 8).Cells(intCol).Value = Format(OBJ_DATAREADER.Item("HIMOKU_KINGAKU" & Format(intCol + 1, "00") & "_H"), "#,##0")
                            End If
                        Next intCol
                    Case 4 To 12
                        '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
                        For intCol = 0 To intMaxHimokuCnt - 1
                            'For intCol = 0 To 9 '2007/04/04�@��ڂ͂P�O�܂�
                            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                            '��ږ������͂���Ă���Δ�ږ����w�b�_���̂ɐݒ肵�A
                            '�Ώې������s�̋��z�Z���ɋ��z��}��
                            '���͂���Ă��Ȃ���΃f�t�H���g�l��ݒ�
                            If IsDBNull(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")) = True Then
                                .Columns(intCol).HeaderText = "���" & CStr(StrConv(CStr(intCol + 1), VbStrConv.Wide))
                                .Rows(intTuki - 4).Cells(intCol).ReadOnly = True
                            Else
                                .Columns(intCol).HeaderText = Trim(CStr(OBJ_DATAREADER.Item("HIMOKU_NAME" & Format(intCol + 1, "00") & "_H")))
                                .Rows(intTuki - 4).Cells(intCol).ReadOnly = False
                                .Rows(intTuki - 4).Cells(intCol).Value = Format(OBJ_DATAREADER.Item("HIMOKU_KINGAKU" & Format(intCol + 1, "00") & "_H"), "#,##0")
                            End If
                        Next intCol
                End Select
            End While
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_SET_SPDDATA2 = True

    End Function
    Private Function PFUNC_Hisuu_Check() As Boolean

        PFUNC_Hisuu_Check = False

        Dim sSiyou_Gakunen As String

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '�w�Z�}�X�^���݃`�F�b�N
            STR_SQL = " SELECT GAKKOU_CODE_G"
            STR_SQL += " FROM GAKMAST1"
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                lblGAKKOU_NAME.Text = "" '2007/02/14

                Exit Function
            End If
        End If

        '�w�N
        If Trim(txtGAKUNEN_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�N�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN_CODE.Focus()

            Exit Function
        Else

            Select Case CInt(txtGAKUNEN_CODE.Text)
                Case 0
                    MessageBox.Show(G_MSG0050W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()

                    Exit Function
            End Select

            '�g�p�w�N�`�F�b�N
            STR_SQL = " SELECT SIYOU_GAKUNEN_T"
            STR_SQL += " FROM GAKMAST2"
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            sSiyou_Gakunen = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "SIYOU_GAKUNEN_T")

            If Trim(sSiyou_Gakunen) = "" Then
                MessageBox.Show(G_MSG0051W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKUNEN_CODE.Focus()

                Exit Function
            End If

            Select Case CInt(sSiyou_Gakunen)
                Case Is < CInt(txtGAKUNEN_CODE.Text)
                    '�g�p�ō��w�N�ȏ�̊w�N�����
                    MessageBox.Show(G_MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()

                    Exit Function
            End Select
        End If

        '���ID
        If Trim(cmbHimoku.Text) = "" Then
            MessageBox.Show(G_MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbHimoku.Focus()

            Exit Function
        End If

        PFUNC_Hisuu_Check = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        Dim bNyuryokuFlg As Boolean

        PFUNC_Nyuryoku_Check = False

        bNyuryokuFlg = False

        With DataGridView1
            '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ START
            For intCnt As Integer = 0 To intMaxHimokuCnt - 1
                'For intCnt As Integer = 0 To 9
                '2017/02/22 �^�X�N�j���� CHG �W���ŏC���i��ڐ��P�O�˂P�T�j------------------------------------ END
                '��ږ������͂���Ă��Ȃ���Ύ��̍s�̓��͒l�`�F�b�N���s��
                If IsDBNull(Trim(.Rows(intCnt).Cells(0).Value)) = False Then
                    If Trim(.Rows(intCnt).Cells(0).Value) <> "" Then
                        bNyuryokuFlg = True

                        '��ږ� 
                        If .Rows(intCnt).Cells(0).Value.Trim.Length > 10 Then
                            MessageBox.Show(G_MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function

                        End If

                        '���Z�@�փR�[�h
                        If IsDBNull(Trim(.Rows(intCnt).Cells(2).Value)) = True Then
                            MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                            'Else
                            '    '���͌��`�F�b�N
                            '    If Len(Trim(.Rows(intCnt).Cells(2).Value)) <> 4 Then
                            '        MessageBox.Show("���Z�@�փR�[�h�̓��͌��͂S���Œ�ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            '        Exit Function
                            '    End If
                        End If

                        '�x�X�R�[�h
                        If IsDBNull(Trim(.Rows(intCnt).Cells(3).Value)) = True Then
                            MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                            'Else
                            '    '���͌��`�F�b�N
                            '    If Len(Trim(.Rows(intCnt).Cells(3).Value)) <> 3 Then
                            '        MessageBox.Show("�x�X�R�[�h�̓��͌��͂R���Œ�ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            '        Exit Function
                            '    End If
                        End If

                        '���͋��Z�@�փ}�X�^���݃`�F�b�N
                        STR_SQL = " SELECT * FROM TENMAST "
                        STR_SQL += " WHERE KIN_NO_N ='" & Trim(.Rows(intCnt).Cells(2).Value) & "'"
                        STR_SQL += " AND SIT_NO_N ='" & Trim(.Rows(intCnt).Cells(3).Value) & "'"

                        If GFUNC_ISEXIST(STR_SQL) = False Then
                            MessageBox.Show(MSG0096W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                        End If

                        '�Ȗ�
                        If .Rows(intCnt).Cells(4).Value = Nothing OrElse IsNumeric(fn_GetKamokuCodeFromKamokuName(.Rows(intCnt).Cells(4).Value)) = False Then
                            MessageBox.Show(G_MSG0055W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If

                        '�����ԍ�
                        If .Rows(intCnt).Cells(5).Value = Nothing OrElse IsDBNull(Trim(.Rows(intCnt).Cells(5).Value)) = True Then
                            MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                            'Else
                            '    '���͌��`�F�b�N
                            '    If Len(Trim(.Rows(intCnt).Cells(5).Value)) <> 7 Then
                            '        MessageBox.Show("�����ԍ��̓��͌��͂V���Œ�ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            '        Exit Function
                            '    End If
                        End If

                        If Trim(.Rows(intCnt).Cells(6).Value) <> "" Then
                            '�ǉ� 
                            If .Rows(intCnt).Cells(6).Value.Trim.Length > 40 Then
                                MessageBox.Show(G_MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            Dim StrConvertRet As String = ""
                            If ConvKanaNGToKanaOK(Trim(.Rows(intCnt).Cells(6).Value), StrConvertRet) = False Then
                                MessageBox.Show(G_MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            Else
                                .Rows(intCnt).Cells(6).Value = StrConvertRet
                            End If
                        End If
                    End If
                End If
            Next intCnt
        End With

        If bNyuryokuFlg = False Then
            MessageBox.Show(G_MSG0058W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Print_Work() As Boolean

        Dim sESC_Gakkou_Code As String = ""
        Dim sESC_Himoku_Id As String = ""
        Dim sESC_Gakunen_Code As String = "" '�ǉ� 2005/10/20

        Dim iColumCount As Integer

        PFUNC_Print_Work = False

        ReDim Str_Work(277, 0)

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        STR_SQL = " DELETE  FROM MAST0300G_WORK"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        STR_SQL = " SELECT "
        STR_SQL += " HIMOMAST.*"
        STR_SQL += ", GAKKOU_NNAME_G , GAKUNEN_NAME_G"
        STR_SQL += " FROM "
        STR_SQL += " HIMOMAST , GAKMAST1"
        STR_SQL += " WHERE GAKKOU_CODE_H = GAKKOU_CODE_G"
        STR_SQL += " AND GAKUNEN_CODE_H = GAKUNEN_CODE_G"

        Select Case Trim(txtGAKKOU_CODE.Text)
            Case ""
                'Case "", "9999999999"
                '�S�w�Z�w��
            Case Else
                STR_SQL += " AND GAKKOU_CODE_H = '" & Trim(txtGAKKOU_CODE.Text) & "'"
        End Select

        Select Case Trim(txtGAKUNEN_CODE.Text)
            Case ""
                'Case "", "9"
                '�S�w�N�w��
            Case Else
                STR_SQL += " AND GAKUNEN_CODE_H = " & Trim(txtGAKUNEN_CODE.Text)
        End Select

        Select Case Trim(cmbHimoku.Text)
            Case ""
                '�S��ڎw��
            Case Else
                STR_SQL += " AND HIMOKU_ID_H = '" & Trim(cmbHimoku.Text) & "'"
        End Select
        STR_SQL += " ORDER BY GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , TUKI_NO_H"



        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        Dim lLoopCount As Long = 0

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                '�w�Z�R�[�h���ς��x�ɒǉ�
                If Trim(sESC_Gakkou_Code) <> Trim(.Item("GAKKOU_CODE_H")) Then
                    If Trim(sESC_Gakkou_Code) <> "" Then
                        lLoopCount += 1
                    End If

                    ReDim Preserve Str_Work(277, lLoopCount)
                Else '�ǉ� 2007/01/17
                    '�w�N�R�[�h���ς��x�ɒǉ�
                    If sESC_Gakunen_Code <> Trim(.Item("GAKUNEN_CODE_H")) Then
                        If Trim(sESC_Gakunen_Code) <> "" Then
                            lLoopCount += 1
                        End If

                        ReDim Preserve Str_Work(277, lLoopCount)
                    End If
                End If

                Select Case .Item("HIMOKU_ID_H")
                    Case "000"
                        Str_Work(0, lLoopCount) = .Item("GAKKOU_CODE_H")
                        Str_Work(1, lLoopCount) = .Item("GAKKOU_NNAME_G")
                        Str_Work(2, lLoopCount) = .Item("GAKUNEN_CODE_H")
                        Str_Work(3, lLoopCount) = .Item("GAKUNEN_NAME_G")
                        Str_Work(4, lLoopCount) = .Item("HIMOKU_ID_H")
                        Str_Work(5, lLoopCount) = .Item("HIMOKU_ID_NAME_H")

                        Str_Work(276, lLoopCount) = .Item("SAKUSEI_DATE_H")
                        Str_Work(277, lLoopCount) = .Item("KOUSIN_DATE_H")

                        iColumCount = 6

                        For iHimokuCount As Integer = 1 To 15 '2007/04/04�@��ڂ͂P�O�܂� ���@15�܂�
                            If IsDBNull(.Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H")) = False Then
                                Str_Work(iColumCount, lLoopCount) = Trim(.Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H"))

                                If IsDBNull(.Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 1, lLoopCount) = .Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 1, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 2, lLoopCount) = .Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 2, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 3, lLoopCount) = Trim(.Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 3, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 4, lLoopCount) = Trim(.Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 4, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 5, lLoopCount) = Trim(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 5, lLoopCount) = ""
                                End If
                            End If

                            iColumCount += 5

                            For iTukiCount As Integer = 1 To 12
                                If IsDBNull(.Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + iTukiCount, lLoopCount) = .Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + iTukiCount, lLoopCount) = "0"
                                End If
                            Next iTukiCount

                            iColumCount += 13
                        Next iHimokuCount
                    Case Else
                        If Trim(sESC_Himoku_Id) <> Trim(.Item("HIMOKU_ID_H")) Then
                            If Trim(sESC_Gakkou_Code) = Trim(.Item("GAKKOU_CODE_H")) Then

                                lLoopCount += 1
                                ReDim Preserve Str_Work(277, lLoopCount)
                            Else
                                '�w�N�R�[�h���ς��x�ɒǉ�
                                If sESC_Gakunen_Code <> Trim(.Item("GAKUNEN_CODE_H")) Then
                                    If Trim(sESC_Gakunen_Code) <> "" Then
                                        lLoopCount += 1
                                    End If

                                    ReDim Preserve Str_Work(277, lLoopCount)
                                End If
                            End If

                            Str_Work(0, lLoopCount) = .Item("GAKKOU_CODE_H")
                            Str_Work(1, lLoopCount) = .Item("GAKKOU_NNAME_G")
                            Str_Work(2, lLoopCount) = .Item("GAKUNEN_CODE_H")
                            Str_Work(3, lLoopCount) = .Item("GAKUNEN_NAME_G")
                            Str_Work(4, lLoopCount) = .Item("HIMOKU_ID_H")
                            Str_Work(5, lLoopCount) = .Item("HIMOKU_ID_NAME_H")

                            Str_Work(276, lLoopCount) = .Item("SAKUSEI_DATE_H")
                            Str_Work(277, lLoopCount) = .Item("KOUSIN_DATE_H")
                        End If

                        iColumCount = 6

                        For iHimokuCount As Integer = 1 To 15
                            If IsDBNull(.Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H")) = False Then
                                Str_Work(iColumCount, lLoopCount) = .Item("HIMOKU_NAME" & Format(iHimokuCount, "00") & "_H")

                                If IsDBNull(.Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 1, lLoopCount) = .Item("KESSAI_KIN_CODE" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 1, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 2, lLoopCount) = .Item("KESSAI_TENPO" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 2, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 3, lLoopCount) = .Item("KESSAI_KAMOKU" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 3, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 4, lLoopCount) = .Item("KESSAI_KOUZA" & Format(iHimokuCount, "00") & "_H")
                                Else
                                    Str_Work(iColumCount + 4, lLoopCount) = ""
                                End If

                                If IsDBNull(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H")) = False Then
                                    Str_Work(iColumCount + 5, lLoopCount) = Trim(.Item("KESSAI_MEIGI" & Format(iHimokuCount, "00") & "_H"))
                                Else
                                    Str_Work(iColumCount + 5, lLoopCount) = ""
                                End If
                            End If

                            iColumCount += 5

                            '2005/10/06
                            If IsDBNull(.Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")) = False Then
                                Str_Work(iColumCount + CInt(.Item("TUKI_NO_H")), lLoopCount) = .Item("HIMOKU_KINGAKU" & Format(iHimokuCount, "00") & "_H")

                            Else
                                Str_Work(iColumCount + CInt(.Item("TUKI_NO_H")), lLoopCount) = "0"
                            End If

                            iColumCount += 13
                        Next iHimokuCount

                End Select
                sESC_Himoku_Id = Trim(.Item("HIMOKU_ID_H"))
                sESC_Gakunen_Code = Trim(.Item("GAKUNEN_CODE_H"))
                sESC_Gakkou_Code = Trim(.Item("GAKKOU_CODE_H"))
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        For l As Integer = 0 To lLoopCount
            STR_SQL = " INSERT INTO MAST0300G_WORK VALUES ("
            For l2 As Integer = 0 To 277
                Select Case l2
                    Case 0
                        STR_SQL += "'" & Str_Work(l2, l) & "'" & vbCrLf
                    Case 2, _
                        12 To 23, 30 To 41, 48 To 59, 66 To 77, 84 To 95, _
                        102 To 113, 120 To 131, 138 To 149, 156 To 167, 174 To 185, _
                        192 To 203, 210 To 221, 228 To 239, 246 To 257, 264 To 275
                        STR_SQL += "," & IIf(Trim(Str_Work(l2, l)) = "", 0, Str_Work(l2, l)) & vbCrLf
                    Case Else
                        STR_SQL += ",'" & IIf(Trim(Str_Work(l2, l)) = "", " ", Str_Work(l2, l)) & "'" & vbCrLf
                End Select
            Next

            STR_SQL += ")"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                Exit Function
            End If
        Next l

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Print_Work = True

    End Function
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '�w�Z�J�i�i���݃R���{
        '********************************************
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z���R���{�{�b�N�X�ݒ�
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbGAKKOUNAME_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        If cmbGAKKOUNAME.SelectedIndex = -1 Then

            Exit Sub
        End If


        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)
        '2007/02/14
        If Trim(txtGAKUNEN_CODE.Text) <> "" Then
            '��񒊏o
            STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
            STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text & "'"
            STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
            STR_SQL += " group by GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
            STR_SQL += " ORDER BY HIMOKU_ID_H "

            '���͂����w�Z����,�w�N�Ŕ�ڃ}�X�^�𒊏o����
            If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then

            End If
        End If
        '�w�N�e�L�X�g�{�b�N�X��FOCUS
        txtGAKUNEN_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '�w�Z������
            STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            If OBJ_DATAREADER.HasRows = False Then
                lblGAKKOU_NAME.Text = ""
            Else
                OBJ_DATAREADER.Read()
                lblGAKKOU_NAME.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If

            If Trim(txtGAKUNEN_CODE.Text) <> "" Then
                '��񒊏o
                STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                STR_SQL += " group by GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                STR_SQL += " ORDER BY HIMOKU_ID_H "

                If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then

                End If
            End If
        End If

    End Sub
    Private Sub txtGAKUNEN_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN_CODE.Validating

        If Trim(txtGAKKOU_CODE.Text) <> "" Then

            If Trim(txtGAKUNEN_CODE.Text) <> "" Then
                '��񒊏o
                STR_SQL = "SELECT GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H FROM HIMOMAST "
                STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                STR_SQL += " group by GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H "
                STR_SQL += " ORDER BY HIMOKU_ID_H "

                '���͂����w�Z����,�w�N�Ŕ�ڃ}�X�^�𒊏o����
                If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then

                End If
            End If
        End If

    End Sub
    Private Sub cmbHimoku_Validating(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbHimoku.Validating

        If Trim(cmbHimoku.Text) <> "" Then
            If Not IsNumeric(cmbHimoku.Text) Then
                MessageBox.Show(G_MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbHimoku.Focus()
                Exit Sub
            End If
            If cmbHimoku.SelectedIndex <> -1 Then
                txtHIMOKU_PATERN.Text = Trim(Str_HName(cmbHimoku.SelectedIndex))
            Else
                txtHIMOKU_PATERN.Text = ""
            End If

            '2006/10/13�@�w�Z�R�[�h�E�w�N�R�[�h�̑��݃`�F�b�N
            If PFUNC_Hisuu_Check() = False Then
                Exit Sub
            End If

            '2010/09/10.Sakon�@�C���|�O���ߌ�̒l�Ŕ��肷�� +++++++++++++++++++++++
            Select Case cmbHimoku.Text.Trim.PadLeft(cmbHimoku.MaxLength, "0"c)
                'Select Case Trim(cmbHimoku.Text)
                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                Case "000"
                    '��񒊏o

                    Int_TabFlag = 0

                    STR_SQL = "SELECT GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H, HIMOKU_ID_NAME_H FROM HIMOMAST "
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " group by GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H,HIMOKU_ID_NAME_H "
                    '===2008/04/13 000�Q�Ɖ\�Ή�========================================================
                    STR_SQL += " ORDER BY GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H,HIMOKU_ID_NAME_H "
                    '=====================================================================================

                    If PFUNC_DB_COMBO_SET3(STR_SQL, cmbHimoku, Str_HName) = False Then
                        txtHIMOKU_PATERN.Text = "���ό���"

                        '̧�ق����݂��Ȃ��ꍇ�͔�ږ��Ƀt�H�[�J�X�ړ�
                        Call PSUB_Nyuuryoku_Clear(2)
                        DataGridView1.Focus()
                    Else
                        cmbHimoku.SelectedIndex = 0
                        txtHIMOKU_PATERN.Text = Trim(Str_HName(cmbHimoku.SelectedIndex))
                        '̧�ق����݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
                        btnFind.Focus()
                    End If
                Case Else
                    '��񒊏o

                    Int_TabFlag = 1

                    '��{���݃`�F�b�N
                    STR_SQL = " SELECT "
                    STR_SQL += " GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H"
                    STR_SQL += " FROM HIMOMAST"
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " AND HIMOKU_ID_H ='000'"

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        MessageBox.Show(G_MSG0060W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtHIMOKU_PATERN.Text = ""
                        Exit Sub
                    End If

                    '��ڍ��ڑ��݃`�F�b�N
                    STR_SQL = " SELECT "
                    STR_SQL += " GAKKOU_CODE_H , GAKUNEN_CODE_H , HIMOKU_ID_H , HIMOKU_ID_NAME_H"
                    STR_SQL += " FROM HIMOMAST"
                    STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                    STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                    STR_SQL += " AND HIMOKU_ID_H ='" & cmbHimoku.Text.Trim.PadLeft(3, "0"c) & "'"

                    If GFUNC_ISEXIST(STR_SQL) = False Then
                        '̧�ق����݂��Ȃ��ꍇ�͔�ږ��Ƀt�H�[�J�X�ړ�
                        STR_SQL = "SELECT *"
                        STR_SQL += " FROM HIMOMAST"
                        STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                        STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                        STR_SQL += " AND HIMOKU_ID_H = '000'"

                        Call PFUNC_SET_SPDDATA(STR_SQL)

                        '���݃`�F�b�N
                        STR_SQL = "SELECT *"
                        STR_SQL += " FROM HIMOMAST"
                        STR_SQL += " WHERE GAKKOU_CODE_H ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
                        STR_SQL += " AND GAKUNEN_CODE_H ='" & txtGAKUNEN_CODE.Text & "'"
                        STR_SQL += " AND HIMOKU_ID_H = '" & cmbHimoku.Text & "'"

                        Call PFUNC_SET_SPDDATA2(STR_SQL)

                        Call PSUB_Nyuuryoku_Clear(5)

                        txtHIMOKU_PATERN.Focus()
                    Else
                        txtHIMOKU_PATERN.Text = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "HIMOKU_ID_NAME_H")
                        '̧�ق����݂���ꍇ�͎Q�ƃ{�^���Ƀt�H�[�J�X�ړ�
                        btnFind.Focus()
                    End If
            End Select
        End If

    End Sub
    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl
    Private ComboBoxEditCtrl As DataGridViewComboBoxEditingControl
    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                Select Case e.ColumnIndex
                    Case 0
                        CType(sender, DataGridView).ImeMode = ImeMode.Hiragana
                    Case 6
                        CType(sender, DataGridView).ImeMode = ImeMode.KatakanaHalf
                    Case 1, 2, 3, 4, 5
                        CType(sender, DataGridView).ImeMode = ImeMode.Disable
                End Select
            Case "DataGridView2"
                CType(sender, DataGridView).ImeMode = ImeMode.Disable
        End Select
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)


        '�X�v���b�g�����ڑOZERO�l��
        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                With CType(sender, DataGridView)
                    Select Case colNo
                        Case 0 '2007/02/12�@��ږ��`�F�b�N
                            If Not DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                                If DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.Length > 10 Then
                                    MessageBox.Show(G_MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    DataGridView1.Focus()
                                End If
                            End If
                        Case 1, 2, 3, 5, 6
                            If Not DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value Is Nothing Then
                                '��ږ������͂���Ă���s�̂ݗL��
                                If Trim(.Rows(e.RowIndex).Cells(0).Value) <> "" Then
                                    If IsDBNull(.Rows(e.RowIndex).Cells(e.ColumnIndex).Value) = False Then
                                        Dim str_Value As String

                                        str_Value = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value

                                        If str_Value Is Nothing Then
                                            str_Value = ""
                                        End If

                                        Select Case e.ColumnIndex
                                            Case 1
                                                str_Value = Format(GCom.NzLong(str_Value.Replace(",", "")), "#,##0")
                                            Case 2
                                                str_Value = str_Value.PadLeft(4, "0"c)
                                            Case 3
                                                str_Value = str_Value.PadLeft(3, "0"c)
                                            Case 5
                                                str_Value = str_Value.PadLeft(7, "0"c)
                                            Case 6
                                                str_Value = StrConv(str_Value, VbStrConv.Uppercase)

                                                Dim StrConvertRet As String = ""
                                                str_Value = StrConv(StrConv(str_Value, VbStrConv.Katakana), VbStrConv.Narrow)
                                                If ConvKanaNGToKanaOK(str_Value, StrConvertRet) = True Then
                                                    str_Value = Replace(StrConvertRet, "�", "-")
                                                End If
                                                If .Rows(e.RowIndex).Cells(e.ColumnIndex).Value.Length > 40 Then
                                                    MessageBox.Show(G_MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                    DataGridView1.Focus()
                                                End If
                                        End Select

                                        .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Trim(str_Value)
                                    End If
                                End If
                            End If
                    End Select
                End With
            Case "DataGridView2"
                Dim StrValue As String = DataGridView2.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                If IsNumeric(StrValue) Then
                    DataGridView2.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Format(CDbl(StrValue), "#,##0")
                End If
        End Select
    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                If colNo <> 4 Then
                    TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)
                Else
                    ComboBoxEditCtrl = CType(e.Control, DataGridViewComboBoxEditingControl)
                End If
                Select Case colNo
                    Case 0
                        AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 6
                        AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 2, 3, 5
                        AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                        AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
                    Case 1
                        AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                        AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
                    Case 4
                        AddHandler ComboBoxEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        AddHandler ComboBoxEditCtrl.KeyPress, AddressOf CAST.KeyPress
                End Select
            Case "DataGridView2"
                TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)
                AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case CType(sender, DataGridView).Name
            Case "DataGridView1"
                Select Case colNo
                    Case 0
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 6
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
                    Case 2, 3, 5
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
                    Case 1
                        RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                        RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
                    Case 4
                        RemoveHandler ComboBoxEditCtrl.GotFocus, AddressOf CAST.GotFocus
                        RemoveHandler ComboBoxEditCtrl.KeyPress, AddressOf CAST.KeyPress
                End Select
            Case "DataGridView2"
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPressMoney
        End Select

        Call CellLeave(sender, e)
    End Sub
    Private Sub DataGridView1_DataError(ByVal sender As Object, _
        ByVal e As DataGridViewDataErrorEventArgs) _
        Handles DataGridView1.DataError
        e.Cancel = False
    End Sub
    Private Function fn_SetDataGridViewKamokuCmbFromText() As Boolean

        Dim cbstr As String() = Nothing
        Dim intCnt As Integer
        Dim Sp() As String

        Dim pTxtFile As String = STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT
        '*****************************************
        '�e�L�X�gFILE����R���{�{�b�N�X�ݒ�
        '*****************************************
        fn_SetDataGridViewKamokuCmbFromText = False

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", "GFUNC_TXT_TO_DBCOMBO", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, Encoding.GetEncoding(932))
        Dim Str_Line As String

        intCnt = 0
        'COMBOBOX ADD

        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Sp = Split(Str_Line, ",")

                ReDim Preserve cbstr(intCnt)
                cbstr(intCnt) = Sp(1)
                intCnt += 1
            End If
        Loop Until Str_Line Is Nothing


        Dim CmbCol As New DataGridViewComboBoxColumn

        For cnt As Integer = 0 To cbstr.Length - 1
            CmbCol.Items.Add(cbstr(cnt))
        Next

        CmbCol.DataPropertyName = "�Ȗ�"
        DataGridView1.Columns.Insert(DataGridView1.Columns("�Ȗ�").Index, CmbCol)
        DataGridView1.Columns.Remove("�Ȗ�")
        CmbCol.Name = "�Ȗ�"

        'FILE CLOSE
        Sr_File.Close()

        fn_SetDataGridViewKamokuCmbFromText = True
    End Function
    Private Function fn_GetKamokuCodeFromKamokuName(ByVal KamokuCode As String) As String

        Dim pTxtFile As String = STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", "GFUNC_TXT_TO_DBCOMBO", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return ""
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, Encoding.GetEncoding(932))
        Dim Str_Line As String

        Dim ArrayKamokuCode As String() = Nothing
        Dim ArrayKamokuName As String() = Nothing
        Dim intCnt As Integer
        Dim Sp() As String

        intCnt = 0
        'COMBOBOX ADD

        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Sp = Split(Str_Line, ",")

                ReDim Preserve ArrayKamokuCode(intCnt)
                ReDim Preserve ArrayKamokuName(intCnt)

                ArrayKamokuCode(intCnt) = Sp(0)
                ArrayKamokuName(intCnt) = Sp(1)

                intCnt += 1
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()

        Dim CmbCol As New DataGridViewComboBoxColumn

        For cnt As Integer = 0 To ArrayKamokuName.Length - 1
            If ArrayKamokuName(cnt).Trim = KamokuCode.Trim Then
                Return ArrayKamokuCode(cnt)
            End If
        Next

        Return ""

    End Function
    Private Function fn_GetKamokuNameFromKamokuCode(ByVal KamokuCode As String) As String

        Dim pTxtFile As String = STR_TXT_PATH & STR_HIMOKU_KAMOKU_TXT

        If File.Exists(pTxtFile) = False Then
            MessageBox.Show(pTxtFile & ":���o�^", "GFUNC_TXT_TO_DBCOMBO", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return ""
        End If

        'FILE OPEN
        Dim Sr_File As New StreamReader(pTxtFile, Encoding.GetEncoding(932))
        Dim Str_Line As String

        Dim ArrayKamokuCode As String() = Nothing
        Dim ArrayKamokuName As String() = Nothing
        Dim intCnt As Integer
        Dim Sp() As String

        intCnt = 0
        'COMBOBOX ADD

        Do
            Str_Line = Sr_File.ReadLine()
            If Str_Line Is Nothing = False Then
                Sp = Split(Str_Line, ",")

                ReDim Preserve ArrayKamokuCode(intCnt)
                ReDim Preserve ArrayKamokuName(intCnt)

                ArrayKamokuCode(intCnt) = Sp(0)
                ArrayKamokuName(intCnt) = Sp(1)

                intCnt += 1
            End If
        Loop Until Str_Line Is Nothing

        'FILE CLOSE
        Sr_File.Close()


        For cnt As Integer = 0 To ArrayKamokuCode.Length - 1
            If ArrayKamokuCode(cnt).Trim = KamokuCode.Trim Then
                Return ArrayKamokuName(cnt)
            End If
        Next

        Return ""

    End Function
    Public Function PFUNC_DB_COMBO_SET3(ByVal pSql As String, _
                   ByVal pCmb As ComboBox, _
                   ByRef pHName() As String) As Boolean
        '*****************************************
        '��ڃR���{�{�b�N�X�ݒ�
        '*****************************************
        Dim iCounter As Integer

        PFUNC_DB_COMBO_SET3 = False

        '�R���{�N���A
        pCmb.Items.Clear()

        If GFUNC_SELECT_SQL2(pSql, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If
        iCounter = 0
        '�R���{�ݒ�
        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                pCmb.Items.Add(.Item("HIMOKU_ID_H"))
                ReDim Preserve pHName(iCounter)
                pHName(iCounter) = CStr(.Item("HIMOKU_ID_NAME_H"))

                iCounter += 1
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_DB_COMBO_SET3 = True
    End Function
    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView1.RowPostPaint

        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' �s�w�b�_�̃Z���̈���A�s�ԍ���`�悷�钷���`�Ƃ���
        ' �i�������E�[��4�h�b�g�̂����Ԃ��󂯂�j
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' ��L�̒����`���ɍs�ԍ����c�����������E�l�ŕ`�悷��
        ' �t�H���g��F�͍s�w�b�_�̊���l���g�p����
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub
    Private Sub CustomDataGridView_RowPostPaint2(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView2.RowPostPaint

        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' �s�w�b�_�̃Z���̈���A�s�ԍ���`�悷�钷���`�Ƃ���
        ' �i�������E�[��4�h�b�g�̂����Ԃ��󂯂�j
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth, dgv.Rows(e.RowIndex).Height)

        ' ��L�̒����`���ɍs�ԍ����c�����������E�l�ŕ`�悷��
        ' �t�H���g��F�͍s�w�b�_�̊���l���g�p����
        Dim strMonth As String

        Select Case e.RowIndex
            Case 11, 10, 9
                strMonth = e.RowIndex - 8
            Case Else
                strMonth = e.RowIndex + 4
        End Select

        TextRenderer.DrawText(e.Graphics, strMonth.ToString() + "��", dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub
    '++++++++++++++++++++++++++++++++++++++++++++++
    '2010/10/12.Sakon�@�Z���̃y�[�X�g�Ή�
    '++++++++++++++++++++++++++++++++++++++++++++++
    Private Sub CustomDataGridView_Past(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _
        DataGridView1.KeyDown, DataGridView2.KeyDown

        Dim dgv As DataGridView = CType(sender, DataGridView)
        Dim CellMaxLength As Integer = 0
        Dim pText As String

        If (e.Modifiers And Keys.Control) = Keys.Control And e.KeyCode = Keys.V Then

            '=====================================================
            '[Ctrl + V]�L�[�������ꂽ�Ƃ��Ƀy�[�X�g���������s
            '=====================================================
            pText = Clipboard.GetText           '�N���b�v�{�[�h�̃e�L�X�g���擾
            pText = pText.Replace(vbCrLf, "")   '���s����菜��

            '�ő啶�����擾�E�K�蕶���`�F�b�N�i�Ǒցj
            If dgv.Name = "DataGridView1" Then
                Select Case dgv.CurrentCell.ColumnIndex
                    Case 0
                        CellMaxLength = Me.��ږ�.MaxInputLength
                    Case 1
                        CellMaxLength = Me.���z.MaxInputLength
                        pText = String.Format("{0:#,##0}", GCom.NzDec(pText, 1))
                    Case 2
                        CellMaxLength = Me.���Z�@�փR�[�h.MaxInputLength
                        pText = CStr(GCom.NzDec(pText, 1))
                    Case 3
                        CellMaxLength = Me.�x�X�R�[�h.MaxInputLength
                        pText = CStr(GCom.NzDec(pText, 1))
                    Case 5
                        CellMaxLength = Me.�����ԍ�.MaxInputLength
                        pText = CStr(GCom.NzDec(pText, 1))
                    Case 6
                        CellMaxLength = Me.�������`�l�J�i.MaxInputLength
                        If ConvKanaNGToKanaOK(Trim(pText), pText) = False Then
                            '���p�J�i���ł��Ȃ��ꍇ�̓y�[�X�g���s��Ȃ�
                            Exit Sub
                        End If
                    Case Else
                        Exit Sub
                End Select
            ElseIf dgv.Name = "DataGridView2" Then
                CellMaxLength = Me.��ڂP.MaxInputLength
                pText = String.Format("{0:#,##0}", GCom.NzDec(pText, 1))
            End If

            '�ő啶�����l��
            If pText.Length > CellMaxLength Then
                pText = pText.Substring(0, CellMaxLength)
            End If

            dgv.CurrentCell.Value = pText   '�e�L�X�g���Z���ɏo��

        ElseIf e.KeyCode = Keys.Delete Or e.KeyCode = Keys.Back Then

            '==============================================================
            '[Delete]�܂���[BackSpace]�L�[�������ꂽ�Ƃ��ɃZ�����N���A
            '==============================================================
            dgv.CurrentCell.Value = ""

        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
