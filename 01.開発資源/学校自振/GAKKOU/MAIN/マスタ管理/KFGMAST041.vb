Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Public Class KFGMAST041

#Region " ���ʕϐ���` "
    Private str���w�N�x As String
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST041", "���q�ݒ���")
    Private Const msgTitle As String = "���q�ݒ���(KFGMAST041)"
#End Region

#Region " Form Load "
    Private Sub KFGMAST041_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")
            Call PSUB_FORMAT_VALUE()
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
    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            '�N���A�{�^��
            txtNENDO.Text = ""
            txtTUUBAN.Text = ""
            txtGAKUNEN.Text = ""
            txtCLASS.Text = ""
            txtSEITONO.Text = ""

            labSEITO_KANA.Text = ""
            labSEITO_KANJI.Text = ""

            txtNENDO.Enabled = True
            txtTUUBAN.Enabled = True
            txtGAKUNEN.Enabled = True
            txtCLASS.Enabled = True
            txtSEITONO.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)��O�G���[", "���s", ex.Message)
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")
            '����{�^��
            Select Case True
                Case (Trim(txtNENDO.Text) <> "" And Trim(txtTUUBAN.Text) <> "")
                    STR_SQL = " SELECT * FROM SEITOMAST"
                    STR_SQL += " WHERE"
                    STR_SQL += " GAKKOU_CODE_O ='" & str���q�w�Z�R�[�h & "'"
                    STR_SQL += " AND"
                    STR_SQL += " NENDO_O ='" & txtNENDO.Text & "'"
                    STR_SQL += " AND"
                    STR_SQL += " TUUBAN_O = " & txtTUUBAN.Text
                Case (Trim(txtGAKUNEN.Text) <> "" And Trim(txtCLASS.Text) <> "" And Trim(txtSEITONO.Text) <> "")
                    STR_SQL = " SELECT * FROM SEITOMAST"
                    STR_SQL += " WHERE"
                    STR_SQL += " GAKKOU_CODE_O ='" & str���q�w�Z�R�[�h & "'"
                    STR_SQL += " AND"
                    STR_SQL += " GAKUNEN_CODE_O = " & txtGAKUNEN.Text
                    STR_SQL += " AND"
                    STR_SQL += " CLASS_CODE_O = " & txtCLASS.Text
                    STR_SQL += " AND"
                    STR_SQL += " SEITO_NO_O ='" & txtSEITONO.Text & "'"
                Case Else
                    Me.Close()

                    Exit Sub
            End Select

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            If OBJ_DATAREADER.HasRows = False Then
                str���q���w�N�x = ""
                str���q�ʔ� = ""
                str���q�w�N = ""
                str���q�N���X = ""
                str���q���k�ԍ� = ""
            Else
                OBJ_DATAREADER.Read()

                str���q���w�N�x = OBJ_DATAREADER.Item("NENDO_O")
                str���q�ʔ� = OBJ_DATAREADER.Item("TUUBAN_O")
                str���q�w�N = OBJ_DATAREADER.Item("GAKUNEN_CODE_O")
                str���q�N���X = OBJ_DATAREADER.Item("CLASS_CODE_O")
                str���q���k�ԍ� = OBJ_DATAREADER.Item("SEITO_NO_O")
            End If

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)��O�G���[", "����", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_FORMAT_VALUE()

        txtNENDO.Text = str���q���w�N�x
        txtTUUBAN.Text = str���q�ʔ�
        txtGAKUNEN.Text = str���q�w�N
        txtCLASS.Text = str���q�N���X
        txtSEITONO.Text = str���q���k�ԍ�

        Select Case True
            Case (Trim(txtNENDO.Text) <> "" And Trim(txtTUUBAN.Text) <> "")
                Call PSUB_Set_SEITONAME(0)
            Case (Trim(txtGAKUNEN.Text) <> "" And Trim(txtCLASS.Text) <> "" And Trim(txtSEITONO.Text) <> "")
                Call PSUB_Set_SEITONAME(1)
        End Select

    End Sub
    Private Sub PSUB_Set_SEITONAME(ByVal pIndex As Integer)

        Select Case pIndex
            Case 0
                STR_SQL = " SELECT * FROM SEITOMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_O ='" & str���q�w�Z�R�[�h.Trim.PadLeft(10, "0"c) & "'"
                STR_SQL += " AND"
                STR_SQL += " NENDO_O ='" & txtNENDO.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " TUUBAN_O = " & txtTUUBAN.Text
            Case 1
                STR_SQL = " SELECT * FROM SEITOMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_O ='" & str���q�w�Z�R�[�h.Trim.PadLeft(10, "0"c) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_O = " & txtGAKUNEN.Text
                STR_SQL += " AND"
                If txtCLASS.Text.Trim = "" Then
                    STR_SQL += " CLASS_CODE_O = 0"
                Else
                    STR_SQL += " CLASS_CODE_O = " & CLng(txtCLASS.Text)
                End If
                STR_SQL += " AND"
                STR_SQL += " SEITO_NO_O ='" & txtSEITONO.Text & "'"
        End Select

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            '2014/01/06 saitou �W���� ���b�Z�[�W�萔�� UPD -------------------------------------------------->>>>
            MessageBox.Show(G_MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'MessageBox.Show("���k�}�X�^�ɓo�^����Ă��܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '2014/01/06 saitou �W���� ���b�Z�[�W�萔�� UPD --------------------------------------------------<<<<

            labSEITO_KANA.Text = ""
            labSEITO_KANJI.Text = ""
            txtNENDO.Text = ""
            txtTUUBAN.Text = ""
            txtCLASS.Text = ""
            txtSEITONO.Text = ""


            Select Case pIndex
                Case 0
                    txtGAKUNEN.Enabled = True
                    txtCLASS.Enabled = True
                    txtSEITONO.Enabled = True
                Case 1
                    txtNENDO.Enabled = True
                    txtTUUBAN.Enabled = True
            End Select

            btnClear.Focus()

            Exit Sub
        Else
            OBJ_DATAREADER.Read()

            If IsDBNull(OBJ_DATAREADER.Item("SEITO_KNAME_O")) = True Then
                labSEITO_KANA.Text = ""
            Else
                labSEITO_KANA.Text = OBJ_DATAREADER.Item("SEITO_KNAME_O")
            End If
            If IsDBNull(OBJ_DATAREADER.Item("SEITO_NNAME_O")) = True Then
                labSEITO_KANJI.Text = ""
            Else
                labSEITO_KANJI.Text = OBJ_DATAREADER.Item("SEITO_NNAME_O")
            End If

            txtNENDO.Text = OBJ_DATAREADER.Item("NENDO_O")
            txtTUUBAN.Text = OBJ_DATAREADER.Item("TUUBAN_O")
            txtGAKUNEN.Text = OBJ_DATAREADER.Item("GAKUNEN_CODE_O")
            txtCLASS.Text = Format(CLng(OBJ_DATAREADER.Item("CLASS_CODE_O")), "00")
            txtSEITONO.Text = Format(CLng(OBJ_DATAREADER.Item("SEITO_NO_O")), "0000000")
        End If

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Sub
        End If

        Select Case pIndex
            Case 0
                txtGAKUNEN.Enabled = False
                txtCLASS.Enabled = False
                txtSEITONO.Enabled = False
            Case 1
                txtNENDO.Enabled = False
                txtTUUBAN.Enabled = False
        End Select

    End Sub
#End Region

    Private Sub TextBox0_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtNENDO.Validating, _
    txtTUUBAN.Validating

        If txtNENDO.Text.Trim <> "" And txtTUUBAN.Text.Trim <> "" Then
            Call PSUB_Set_SEITONAME(0)
        End If
    End Sub

    Private Sub TextBox1_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtGAKUNEN.Validating, _
    txtCLASS.Validating, _
    txtSEITONO.Validating

        If Len(txtGAKUNEN.Text) <> 0 And Len(txtCLASS.Text) <> 0 And Len(txtSEITONO.Text) <> 0 Then
            Call PSUB_Set_SEITONAME(1)
        End If
    End Sub

End Class
