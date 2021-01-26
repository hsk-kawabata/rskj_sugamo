Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGMAIN070

    Public STR_0700G_FLG As String
    Public strCHECK_FLG As String = ""
    Public strDATA_FLG As String = ""
    Private Str_GAK_INFO(3) As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN070", "���k���ד��͉��")
    Private Const msgTitle As String = "���k���ד��͉��(KFGMAIN070)"
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

#Region " Form Load "
    Private Sub KFGMAIN070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '�w�Z�R���{�ݒ�i�S�w�Z�j
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�R���{�{�b�N�X�ݒ�(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "�w�Z����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            '���s�{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")
            Dim KFGMAIN071 As New GAKKOU.KFGMAIN071

            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check(0) = False Then
                Exit Sub
            End If

            '��ʊԎd�l�l��ϐ��Ɋi�[
            Call PSUB_Keep_Value()

            '���k���ד��͉�ʂɑJ��
            KFGMAIN071.ShowDialog(Me)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", ex.ToString)
        Finally
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreate.Click
        Try
            '���s�{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^�쐬)�J�n", "����", "")
            Dim KFGMAIN072 As New GAKKOU.KFGMAIN072

            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check(1) = False Then
                Exit Sub
            End If

            '��ʊԎd�l�l��ϐ��Ɋi�[
            Call PSUB_Keep_Value()

            '���k���ד��͉�ʂɑJ��
            KFGMAIN072.ShowDialog(Me)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^�쐬)", "���s", ex.ToString)
        Finally
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^�쐬)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '�I���{�^��
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
#End Region

#Region " Private Sub "
    Private Sub PSUB_Keep_Value()

        '�w�Z�R�[�h�̐ݒ�
        STR_���k���׊w�Z�R�[�h = Trim(txtGAKKOU_CODE.Text)

        STR_���k���׊w�Z�� = Trim(lab�w�Z��.Text)

        '�U�֓��̐ݒ�
        STR_���k���אU�֓� = Mid(cmbFURIKAEBI.Text, 1, 4) & Mid(cmbFURIKAEBI.Text, 6, 2) & Mid(cmbFURIKAEBI.Text, 9, 2)

        '���o���敪�̐ݒ�
        Select Case Mid(cmbFURIKAEBI.Text, 12, 2)
            Case "����"
                STR_���k���ד��o�敪 = 2
            Case "�o��"
                STR_���k���ד��o�敪 = 3
        End Select

        '�����l�̐ݒ�
        If txtSYOKICHI.Text.Trim = "" Then
            STR_���k���׏����l = ""
        Else
            STR_���k���׏����l = CStr(CDbl(Trim(txtSYOKICHI.Text)))
        End If

        '�\�[�g���̐ݒ�
        Select Case True
            Case rdoButton1.Checked
                '�w�N�E�N���X�E���k�ԍ���
                STR_���k���׃\�[�g�� = 1
            Case rdoButton2.Checked
                '���w�N�x�E�ʔԏ�
                STR_���k���׃\�[�g�� = 2
            Case rdoButton3.Checked
                '������������
                STR_���k���׃\�[�g�� = 3
        End Select

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        With CType(sender, TextBox)
            If .Text.Trim <> "" Then

                '�w�Z���̎擾
                If PFUNC_Get_GakkouName() = False Then
                    Exit Sub
                Else
                    '�U�֓��R���{�{�b�N�X�̐ݒ�
                    If PFUNC_Set_cmbFURIKAEBI() = False Then
                        Exit Sub
                    End If
                End If

            End If
        End With

    End Sub
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '�w�Z�J�i�i���݃R���{
        '********************************************
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 �^�X�N�j���� DEL �W���ŏC���i�J�i�����̃N���A�Ή��j----------------- END

        '�w�Z���R���{�{�b�N�X�ݒ�
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lab�w�Z��.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '�w�N�e�L�X�g�{�b�N�X��FOCUS
        txtGAKKOU_CODE.Focus()
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check(ByVal pIndex As Integer) As Boolean

        PFUNC_Nyuryoku_Check = False

        Dim SQL As New System.Text.StringBuilder(128)

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '�w�Z�}�X�^���݃`�F�b�N
            SQL.Length = 0
            SQL.Append(" SELECT * FROM GAKMAST1 ")
            SQL.Append(" WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If GFUNC_ISEXIST(SQL.ToString) = False Then

                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If

            SQL.Length = 0
            SQL.Append(" SELECT * FROM GAKMAST2 ")
            SQL.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                Exit Function
            End If

            With OBJ_DATAREADER
                .Read()

                STR_���k���׈���敪 = .Item("MEISAI_KBN_T")
            End With

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
        End If

        '�U�֓�
        If Trim(cmbFURIKAEBI.Text) = "" Then
            MessageBox.Show(G_MSG0010W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbFURIKAEBI.Focus()

            Exit Function
        End If

        SQL.Length = 0
        SQL.Append(" SELECT CHECK_FLG_S,DATA_FLG_S FROM G_SCHMAST ")
        SQL.Append(" WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'")
        SQL.Append(" AND FURI_DATE_S ='" & Mid(cmbFURIKAEBI.Text, 1, 4) & Mid(cmbFURIKAEBI.Text, 6, 2) & Mid(cmbFURIKAEBI.Text, 9, 2) & "'")
        SQL.Append(" AND SCH_KBN_S ='2'")
        SQL.Append(" AND ENTRI_FLG_S ='1'")
        '2006/10/20 ���o���敪�����������ɒǉ�
        Select Case Mid(cmbFURIKAEBI.Text, 12, 2)
            Case "����"
                SQL.Append(" AND FURI_KBN_S ='2'")
            Case "�o��"
                SQL.Append(" AND FURI_KBN_S ='3'")
        End Select

        STR_0700G_FLG = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "CHECK_FLG_S")
        strCHECK_FLG = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "CHECK_FLG_S")
        strDATA_FLG = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "DATA_FLG_S")

        Select Case pIndex
            Case 0 '���ד���
            Case 1 '�f�[�^�쐬
                If strCHECK_FLG = "0" Then
                    MessageBox.Show(G_MSG0011W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    btnAction.Focus()
                    Exit Function
                End If
        End Select

        If strDATA_FLG = "1" Then
            MessageBox.Show(G_MSG0012W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            btnEnd.Focus()
            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Get_GakkouName() As Boolean

        Dim sGakkou_Name As String

        '�w�Z���̐ݒ�
        PFUNC_Get_GakkouName = False

        Dim SQL As New System.Text.StringBuilder(128)
        SQL.Append(" SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
        SQL.Append(" WHERE GAKKOU_CODE_G  ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

        sGakkou_Name = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "GAKKOU_NNAME_G")
        If Trim(sGakkou_Name) = "" Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            lab�w�Z��.Text = ""
            txtGAKKOU_CODE.Text = "" '2006/10/24�@�ǉ�
            txtGAKKOU_CODE.Focus()

            Exit Function
        End If

        lab�w�Z��.Text = sGakkou_Name
        STR_���k���׊w�Z�� = sGakkou_Name

        PFUNC_Get_GakkouName = True

    End Function
    Private Function PFUNC_Set_cmbFURIKAEBI() As Boolean

        '�U�֓��R���{�̐ݒ�
        Dim str�U�֓� As String

        PFUNC_Set_cmbFURIKAEBI = False

        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '�U�֓��R���{�{�b�N�X�̃N���A
            cmbFURIKAEBI.Items.Clear()

            '�X�P�W���[���}�X�^�̌����A�L�[�͊w�Z�R�[�h�A�X�P�W���[���敪�A���׍쐬�t���O
            Dim SQL As New System.Text.StringBuilder(128)
            SQL.Append(" SELECT * FROM G_SCHMAST ")
            SQL.Append(" WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            SQL.Append(" AND SCH_KBN_S ='2'")
            SQL.Append(" AND ENTRI_FLG_S ='1'")
            SQL.Append(" ORDER BY FURI_DATE_S")

            If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                Exit Function
            End If

            If OBJ_DATAREADER.HasRows = True Then

                While (OBJ_DATAREADER.Read = True)
                    With OBJ_DATAREADER
                        '�U�֓��̕ҏW
                        str�U�֓� = Mid(.Item("FURI_DATE_S"), 1, 4) & "/" & Mid(.Item("FURI_DATE_S"), 5, 2) & "/" & Mid(.Item("FURI_DATE_S"), 7, 2)

                        '�����A�o���̕ҏW
                        Select Case OBJ_DATAREADER.Item("FURI_KBN_S")
                            Case "2"
                                str�U�֓� += " ����"
                            Case "3"
                                str�U�֓� += " �o��"
                        End Select
                        '�U�֓��R���{�{�b�N�X�֒ǉ�
                        cmbFURIKAEBI.Items.Add(str�U�֓�)
                    End With
                End While

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                '�R���{�擪�̐ݒ�
                cmbFURIKAEBI.SelectedIndex = 0
            Else
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
            End If
        End If

        PFUNC_Set_cmbFURIKAEBI = True

    End Function
#End Region

End Class
