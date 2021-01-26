Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGMAIN060
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' ���k���׍쐬
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " ���ʕϐ���` "
    Private Str_Syori_Date(1) As String
    Private Str_Gakunen_Flg() As String
    Private Bln_Gakunen_Flg As Boolean

    Private Str_Gakkou_Code As String
    Private Str_Syori_Nentuki As String
    Private Str_FurikaeDate As String
    Private Str_Nyusyutu_Kbn As String

    Private Str_GAK_INFO(3) As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN060", "���k���׍쐬���")
    Private Const msgTitle As String = "���k���׍쐬���(KFGMAIN060)"
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

#End Region

#Region " Form Load "
    Private Sub KFGMAIN060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

            txtTAISYONEN.Text = Format(Now, "yyyy")
            txtTAISYOUTUKI.Text = Format(Now, "MM")
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
            '�쐬�{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)�J�n", "����", "")
            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "���k���׍쐬"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = Str_FurikaeDate

            '�w��w�N�̎擾
            If PFUNC_Get_Gakunen() = False Then
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            '�����f�[�^�̍폜
            If PFUNC_Delete_ENTMAST() = False Then
                Exit Sub
            End If

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            '�G���g���f�[�^�̍쐬
            If PFUNC_Insert_ENTMAST() = False Then
                Exit Sub
            End If

            '�X�P�W���[���}�X�^�̖��׍쐬�σt���O���u�P�v�i�쐬�ρj�ɂ���
            If PFUNC_Update_Schedule() = False Then
                Exit Sub
            End If

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            '�������b�Z�[�W
            MessageBox.Show(String.Format(MSG0016I, "���k���׍쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)", "���s", ex.ToString)
        Finally
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�쐬)�I��", "����", "")
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

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_Get_GakkouName() As Boolean

        Dim sGakkou_Name As String

        '�w�Z���̐ݒ�
        PFUNC_Get_GakkouName = False

        Dim SQL As New System.Text.StringBuilder(128)

        SQL.Append(" SELECT GAKKOU_NNAME_G ")
        SQL.Append(" FROM GAKMAST1 ")
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

        PFUNC_Get_GakkouName = True

    End Function
    Private Function PFUNC_Set_cmbFURIKAEBI() As Boolean

        '�U�֓��R���{�̐ݒ�
        Dim str�U�֓� As String

        PFUNC_Set_cmbFURIKAEBI = False

        If Trim(txtGAKKOU_CODE.Text) <> "" And _
           Trim(txtTAISYONEN.Text) <> "" And _
           Trim(txtTAISYOUTUKI.Text) <> "" Then

            '�U�֓��R���{�{�b�N�X�̃N���A
            cmbFURIKAEBI.Items.Clear()

            Dim SQL As New System.Text.StringBuilder(128)
            '�X�P�W���[���}�X�^�̌����A�L�[�͊w�Z�R�[�h�A�X�P�W���[���敪�A���׍쐬�t���O
            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            '2017/05/15 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '�����ł͑O�[�����߂ɂȂ��Ă��Ȃ��̂ŕ⊮����
            SQL.Append(" AND NENGETUDO_S ='" & txtTAISYONEN.Text.PadLeft(4, "0"c) & txtTAISYOUTUKI.Text.PadLeft(2, "0"c) & "'")
            'SQL.Append(" AND NENGETUDO_S ='" & Trim(txtTAISYONEN.Text) & Trim(txtTAISYOUTUKI.Text) & "'")
            '2017/05/15 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
            SQL.Append(" AND SCH_KBN_S ='2'")
            SQL.Append(" ORDER BY FURI_DATE_S")

            If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                Exit Function
            End If

            If OBJ_DATAREADER.HasRows = False Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If

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
        End If

        PFUNC_Set_cmbFURIKAEBI = True

    End Function
    Private Function PFUNC_Update_Schedule() As Boolean

        '�X�P�W���[���}�X�^�̍X�V

        PFUNC_Update_Schedule = False
        Dim SQL As New System.Text.StringBuilder(128)
        SQL.Append(" UPDATE  G_SCHMAST SET ")
        SQL.Append(" ENTRI_FLG_S ='1'")
        SQL.Append(" ,CHECK_FLG_S ='0'") '�`�F�b�N�t���O������ 2007/02/10
        SQL.Append(",TIME_STAMP_S ='" & Str_Syori_Date(1) & "'")
        SQL.Append(" WHERE GAKKOU_CODE_S ='" & Str_Gakkou_Code & "'")
        SQL.Append(" AND NENGETUDO_S ='" & Str_Syori_Nentuki & "'")
        SQL.Append(" AND SCH_KBN_S ='2'")
        SQL.Append(" AND FURI_KBN_S ='" & Str_Nyusyutu_Kbn & "'")
        SQL.Append(" AND FURI_DATE_S ='" & Str_FurikaeDate & "'")

        If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False

        '�w�Z�R�[�h
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "�w�Z�R�[�h"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Exit Function
        Else
            '�w�Z�}�X�^���݃`�F�b�N
            Dim SQL As String = " SELECT * FROM GAKMAST1"
            SQL &= " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_ISEXIST(SQL) = False Then

                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If

        End If

        Str_Gakkou_Code = Trim(txtGAKKOU_CODE.Text)

        '�Ώ۔N
        If Trim(txtTAISYONEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYONEN.Focus()
            Exit Function
        Else
            '���l�`�F�b�N 2006/10/10
            If IsNumeric(txtTAISYONEN.Text) = False Then
                MessageBox.Show(MSG0019W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTAISYONEN.Focus()
                Exit Function
                'Else
                '    If txtTAISYONEN.Text.Trim.Length < 4 Then
                '        MessageBox.Show("�����Ώ۔N�̓��͂��s���ł�", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '        txtTAISYONEN.Focus()
                '        Exit Function
                '    End If

            End If

        End If

        '�Ώی�
        If Trim(txtTAISYOUTUKI.Text) = "" Then
            Call MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYOUTUKI.Focus()
            Exit Function
        Else

            '���l�`�F�b�N 2006/10/10
            If IsNumeric(txtTAISYOUTUKI.Text) = False Then
                Call MessageBox.Show(MSG0021W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTAISYOUTUKI.Focus()
                Exit Function
            End If

            Select Case CInt(txtTAISYOUTUKI.Text)
                Case 1 To 12
                Case Else
                    Call MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTAISYOUTUKI.Focus()
                    Exit Function
            End Select
        End If

        Str_Syori_Nentuki = Trim(txtTAISYONEN.Text) & Trim(txtTAISYOUTUKI.Text)
        Str_FurikaeDate = Mid(cmbFURIKAEBI.Text, 1, 4) & Mid(cmbFURIKAEBI.Text, 6, 2) & Mid(cmbFURIKAEBI.Text, 9, 2)
        Select Case Mid(cmbFURIKAEBI.Text, 12, 2)
            Case "����"
                Str_Nyusyutu_Kbn = "2"
            Case "�o��"
                Str_Nyusyutu_Kbn = "3"
        End Select

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Get_Gakunen() As Boolean

        '�N�ԃX�P�W���[�����琏�������̂��̂���������
        Dim iLoopCount As Integer
        Dim iGakunenCount As Integer

        PFUNC_Get_Gakunen = False

        Dim SQL As New System.Text.StringBuilder(128)
        SQL.Append(" SELECT ")
        SQL.Append(" G_SCHMAST.* ")
        SQL.Append(", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T")
        SQL.Append(" FROM G_SCHMAST , GAKMAST2")
        SQL.Append(" WHERE GAKKOU_CODE_S = GAKKOU_CODE_T")
        SQL.Append(" AND GAKKOU_CODE_S ='" & Str_Gakkou_Code & "'")
        SQL.Append(" AND FURI_DATE_S ='" & Str_FurikaeDate & "'")
        SQL.Append(" AND SCH_KBN_S = '2'")
        SQL.Append(" AND FURI_KBN_S ='" & Str_Nyusyutu_Kbn & "'")

        If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
            Exit Function
        End If

        '�X�P�W���[���}�X�^���݃`�F�b�N
        If OBJ_DATAREADER.HasRows = False Then
            Call MessageBox.Show(G_MSG0008W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Call GFUNC_SELECT_SQL2("", 1)
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        '���k���׃f�[�^�쐬�t���O�`�F�b�N 2007/02/10
        If OBJ_DATAREADER.Item("DATA_FLG_S") = "1" Then
            Call MessageBox.Show(G_MSG0012W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Call GFUNC_SELECT_SQL2("", 1)
            txtGAKKOU_CODE.Focus()
            Exit Function
        End If
        '���k���׃f�[�^���v�`�F�b�N�t���O�`�F�b�N 2007/02/10
        If OBJ_DATAREADER.Item("CHECK_FLG_S") = "1" Then
            If MessageBox.Show(G_MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If
        End If

        If OBJ_DATAREADER.Item("ENTRI_FLG_S") = "1" Then
            If MessageBox.Show(G_MSG0004I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If
        End If

        iGakunenCount = 1

        With OBJ_DATAREADER

            For iLoopCount = 1 To CInt(.Item("SIYOU_GAKUNEN_T"))
                If .Item("GAKUNEN" & iLoopCount & "_FLG_S") = "1" Then
                    ReDim Preserve Str_Gakunen_Flg(iGakunenCount)
                    Str_Gakunen_Flg(iGakunenCount) = iLoopCount
                    iGakunenCount += 1
                End If
            Next

            '�g�p�w�N�����w�Z�}�X�^�Őݒ肳��Ă���g�p�w�N���ƈ�v����ꍇ��
            '�S�w�N�����o�Ώ�
            If CInt(.Item("SIYOU_GAKUNEN_T")) = UBound(Str_Gakunen_Flg) Then
                Bln_Gakunen_Flg = False
            Else
                Bln_Gakunen_Flg = True
            End If
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen = True

    End Function
    Private Function PFUNC_Delete_ENTMAST() As Boolean
        '�폜�����A�L�[�͊w�Z�R�[�h�A�Ώ۔N���A�w�N
        '�U�֓��ǉ� 2006/10/16

        Dim iLoopCount As Integer

        PFUNC_Delete_ENTMAST = False

        Dim SQL As String = ""

        Select Case Str_Nyusyutu_Kbn
            Case "2"
                '����
                SQL = " DELETE  FROM G_ENTMAST1"
            Case "3"
                '�o��
                SQL = " DELETE  FROM G_ENTMAST2"
        End Select
        SQL += " WHERE GAKKOU_CODE_E ='" & Str_Gakkou_Code & "'"
        SQL += " AND SYORI_NENGETU_E ='" & Str_Syori_Nentuki & "'"
        SQL += " AND FURI_DATE_E ='" & Str_FurikaeDate & "'" '�ǉ� 2006/10/16

        '�w�N�w�肪���݂���ꍇ�́A�w�肳��Ă���w�N�݂̂��폜����
        If Bln_Gakunen_Flg = True Then
            SQL += " and("
            For iLoopCount = 1 To UBound(Str_Gakunen_Flg)
                If iLoopCount = 1 Then
                    SQL += " GAKUNEN_CODE_E =" & Str_Gakunen_Flg(iLoopCount)
                Else
                    SQL += " OR GAKUNEN_CODE_E =" & Str_Gakunen_Flg(iLoopCount)
                End If
            Next iLoopCount
            SQL += ")"
        End If

        If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "�폜"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        PFUNC_Delete_ENTMAST = True

    End Function
    Private Function PFUNC_Insert_ENTMAST() As Boolean

        Dim iLoopCount As Integer

        PFUNC_Insert_ENTMAST = False

        Dim SQL As String = ""

        Select Case Str_Nyusyutu_Kbn
            Case "2"
                '����
                SQL = " INSERT INTO G_ENTMAST1"
            Case "3"
                '�o��
                SQL = " INSERT INTO G_ENTMAST2"
        End Select
        SQL += " SELECT "
        SQL += " GAKKOU_CODE_O"
        SQL += ",'" & Str_Syori_Nentuki & "'"
        SQL += ",'" & Str_FurikaeDate & "'"
        SQL += ", rownum"
        SQL += ", NENDO_O"
        SQL += ", TUUBAN_O"
        SQL += ", GAKUNEN_CODE_O"
        SQL += ", CLASS_CODE_O"
        SQL += ", SEITO_NO_O"
        SQL += ", SEITO_KNAME_O"
        SQL += ", SEITO_NNAME_O"
        SQL += ", SEIBETU_O"
        SQL += ", KAMOKU_O"
        SQL += ", KOUZA_O"
        SQL += ", concat('000' , SEITO_NO_O)"
        SQL += ", MEIGI_KNAME_O"
        SQL += ", MEIGI_NNAME_O"
        SQL += ", 0"
        SQL += ", 0"
        SQL += ", 0"
        SQL += ", '0'"
        SQL += ", TKIN_NO_O"
        SQL += ", TSIT_NO_O"
        SQL += ", '0'"
        SQL += ", rownum"
        SQL += ", 0"
        SQL += ",'" & Str_Syori_Date(0) & "'"
        SQL += ", '00000000'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += " FROM SEITOMAST"
        SQL += " WHERE"
        SQL += " GAKKOU_CODE_O ='" & Str_Gakkou_Code & "'"
        '�P���k�ɕ������R�[�h�����݂��邽�߂̑Ώ�
        '��
        SQL += " AND"
        SQL += " TUKI_NO_O = '04'"
        '��
        SQL += " AND"
        SQL += " FURIKAE_O = '0'"
        SQL += " AND"
        SQL += " KAIYAKU_FLG_O <> '9'"
        '�w�N�w�肪���݂���ꍇ�́A�w�肳��Ă���w�N�݂̂��폜����
        If Bln_Gakunen_Flg = True Then
            SQL += " and("
            For iLoopCount = 1 To UBound(Str_Gakunen_Flg)
                If iLoopCount = 1 Then
                    SQL += " GAKUNEN_CODE_O =" & Str_Gakunen_Flg(iLoopCount)
                Else
                    SQL += " OR GAKUNEN_CODE_O =" & Str_Gakunen_Flg(iLoopCount)
                End If
            Next iLoopCount
            SQL += ")"
        End If

        If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "�o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        PFUNC_Insert_ENTMAST = True

    End Function

#End Region

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

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        'COMBOBOX�I�����w�Z��,�w�Z�R�[�h�ݒ�
        lab�w�Z��.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '�w�N�e�L�X�g�{�b�N�X��FOCUS
        txtGAKKOU_CODE.Focus()
    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtGAKKOU_CODE.Validating, _
    txtTAISYOUTUKI.Validating

        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                Select Case .Name
                    Case "txtGAKKOU_CODE"
                        If IsNumeric(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) = False Then
                            Call MessageBox.Show(G_MSG0013W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtTAISYONEN.Focus()
                            Exit Sub
                        End If

                        '�w�Z���̎擾
                        If PFUNC_Get_GakkouName() = False Then
                            Exit Sub
                        Else
                            If PFUNC_Set_cmbFURIKAEBI() = False Then
                                Exit Sub
                            End If
                        End If
                    Case "txtTAISYOUTUKI"
                        '�U�֓��R���{�{�b�N�X�̐ݒ�
                        If PFUNC_Set_cmbFURIKAEBI() = False Then
                            Exit Sub
                        End If
                End Select
            Else
                If .Name = "txtGAKKOU_CODE" Then
                    lab�w�Z��.Text = ""
                End If
            End If
        End With
    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
