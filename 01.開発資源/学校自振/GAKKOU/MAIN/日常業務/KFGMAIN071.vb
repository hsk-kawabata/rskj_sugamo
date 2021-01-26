Option Explicit On 
Option Strict Off

Imports CASTCommon

Public Class KFGMAIN071

#Region " ���ʕϐ���` "
    Private INTCNT01 As Integer
    Private INTCNT02 As Integer
    Private LNG���v���z As Long

    Private Str_Syori_Date(1) As String
    Private Str_Ginko(3) As String

    Dim flg As Boolean = False
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN071", "���k���ד��͉��")
    Private Const msgTitle As String = "���k���ד��͉��(KFGMAIN071)"
    Private MainDB As CASTCommon.MyOracle   '�p�u���b�N�f�[�^�[�x�[�X

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    Private Sub KFGMAIN071_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            LW.ToriCode = STR_���k���׊w�Z�R�[�h
            LW.FuriDate = STR_���k���אU�֓�
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '�w�Z�R�[�h
            lab�w�Z�R�[�h.Text = STR_���k���׊w�Z�R�[�h

            '�w�Z���̐ݒ�
            lab�w�Z��.Text = STR_���k���׊w�Z��

            lab�U�֓�.Text = Mid(STR_���k���אU�֓�, 1, 4) & "/" & Mid(STR_���k���אU�֓�, 5, 2) & "/" & Mid(STR_���k���אU�֓�, 7, 2)

            Select Case STR_���k���ד��o�敪
                Case "2"
                    lab���o���敪.Text = "����"
                Case "3"
                    lab���o���敪.Text = "�o��"
            End Select

            If Trim(STR_���k���׏����l) <> "" Then
                lab�����l.Text = Format(CDbl(STR_���k���׏����l), "#,##0")
            Else
                lab�����l.Text = ""
            End If

            '�G���g���}�X�^�̓Ǎ���
            If PFUNC_Spread_Set() = False Then
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnGOKEI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGOKEI.Click

        Try
            '���v�{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���v)�J�n", "����", "")

            Str_Syori_Date(0) = Format(Now, "yyyyMMdd")
            Str_Syori_Date(1) = Format(Now, "yyyyMMddHHmmss")

            '���͒l�`�F�b�N
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "���v"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            '�G���g���}�X�^�̍X�V
            If PFUNC_Update_ENTMAST() = False Then
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            '�X�P�W���[���}�X�^�̍X�V
            If PFUNC_Update_Schedule() = False Then
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            '�������b�Z�[�W
            MessageBox.Show(String.Format(MSG0016I, "���v"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���v)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���v)�I��", "����", "")
        End Try
    End Sub
    Private Sub btnTOUROKU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTOUROKU.Click

        Try
            '�o�^�{�^��
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�J�n", "����", "")

            If MessageBox.Show(String.Format(MSG0015I, "�o�^"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            '�G���g���}�X�^�̍X�V
            If PFUNC_Update_ENTMAST() = False Then
                Exit Sub
            End If

            '�������b�Z�[�W
            MessageBox.Show(String.Format(MSG0016I, "�o�^"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)", "���s", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�o�^)�I��", "����", "")
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
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        Dim ret As Boolean = False

        Try
            If Trim(txt���͍��v���z.Text) = "" Then
                MessageBox.Show(MSG0249W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txt���͍��v���z.Focus()
                Exit Try
            End If

            LNG���v���z = 0
            For INTCNT02 = 0 To INTCNT01 - 1
                With DataGridView
                    If Trim(.Rows(INTCNT02).Cells(17).Value) <> "" Then
                        LNG���v���z = LNG���v���z + CDec(.Rows(INTCNT02).Cells(17).Value)
                    End If
                End With
            Next INTCNT02

            If CDbl(txt���͍��v���z.Text) <> CDbl(LNG���v���z) Then
                MessageBox.Show(G_MSG0009W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txt���͍��v���z.Focus()
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            ret = False
        End Try

        Return ret

    End Function
    Private Function PFUNC_Spread_Set() As Boolean

        Dim iNo As Integer
        Dim MainDB As New MyOracle

        PFUNC_Spread_Set = False

        Try

            Select Case STR_���k���ד��o�敪
                Case "2"
                    iNo = 1
                Case "3"
                    iNo = 2
            End Select

            '�G���g���}�X�^����������


            '�X�v���b�h�w�b�_�̕ҏW
            Select Case iNo
                Case 1
                    DataGridView.Columns(17).HeaderText = "�������z"
                Case 2
                    DataGridView.Columns(17).HeaderText = "�o�����z"
            End Select

            '�G���g���}�X�^������SQL���쐬
            STR_SQL = " SELECT * FROM "
            STR_SQL += " G_ENTMAST" & iNo
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_E ='" & STR_���k���׊w�Z�R�[�h & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_E ='" & STR_���k���אU�֓� & "'"
            STR_SQL += " ORDER BY "
            Select Case STR_���k���׃\�[�g��
                Case "1"
                    '�w�N�A�N���X�A���k�ԍ�
                    STR_SQL += " GAKUNEN_CODE_E ASC, CLASS_CODE_E ASC, SEITO_NO_E ASC"
                Case "2"
                    '���w�N�x�A�ʔ�
                    STR_SQL += " GAKUNEN_CODE_E ASC, NENDO_E ASC, TUUBAN_E ASC"
                Case "3"
                    '���k���̃A�C�E�G�I��
                    STR_SQL += " GAKUNEN_CODE_E ASC, SEITO_KNAME_E ASC"
            End Select

            '�G���g���}�X�^���݃`�F�b�N
            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0007W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Function
            End If

            INTCNT01 = 0

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            With DataGridView
                Select Case STR_���k���׃\�[�g��
                    Case "1"
                        '�w�N�A�N���X�A���k�ԍ�
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = True
                        .Columns(3).Visible = True
                        .Columns(4).Visible = True
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False

                    Case "2"
                        '���w�N�x�A�ʔ�
                        .Columns(0).Visible = True
                        .Columns(1).Visible = True
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False
                    Case "3"
                        '���k���̃A�C�E�G�I��
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = True
                        .Columns(18).Visible = False
                End Select
            End With

            While (OBJ_DATAREADER.Read = True)
                With DataGridView

                    '�ǉ��s
                    Dim RowItem As New DataGridViewRow
                    RowItem.CreateCells(DataGridView)

                    '���w�N�x
                    RowItem.Cells(0).Value = OBJ_DATAREADER.Item("NENDO_E")
                    '�ʔ�
                    RowItem.Cells(1).Value = OBJ_DATAREADER.Item("TUUBAN_E")
                    '�w�N
                    RowItem.Cells(2).Value = OBJ_DATAREADER.Item("GAKUNEN_CODE_E")
                    '�N���X
                    RowItem.Cells(3).Value = OBJ_DATAREADER.Item("CLASS_CODE_E")
                    '���k�ԍ�
                    RowItem.Cells(4).Value = OBJ_DATAREADER.Item("SEITO_NO_E")
                    '���k���J�i
                    RowItem.Cells(5).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    '���k������ 2007/02/10
                    If IsDBNull(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = True Then
                        RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = "" Then '�X�y�[�X�̏ꍇ�J�i�\��
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                        Else
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_NNAME_E")
                        End If
                    End If

                    '�\���p���k��
                    If RowItem.Cells(6).Value = "" Then
                        RowItem.Cells(7).Value = RowItem.Cells(5).Value
                    Else
                        RowItem.Cells(7).Value = RowItem.Cells(6).Value
                    End If

                    Call PSUB_GET_GINKONAME(OBJ_DATAREADER.Item("TKIN_NO_E"), OBJ_DATAREADER.Item("TSIT_NO_E"), MainDB)

                    '���Z�@�փR�[�h�̊i�[
                    RowItem.Cells(8).Value = OBJ_DATAREADER.Item("TKIN_NO_E")
                    RowItem.Cells(9).Value = Str_Ginko(1)

                    '�x�X�R�[�h�̊i�[
                    RowItem.Cells(10).Value = OBJ_DATAREADER.Item("TSIT_NO_E")
                    RowItem.Cells(11).Value = Str_Ginko(3)

                    '�ȖڃR�[�h�̊i�[�i�Q������P���ɕϊ��j
                    Select Case OBJ_DATAREADER.Item("KAMOKU_E")
                        Case "01"
                            RowItem.Cells(12).Value = "2"
                        Case "02"
                            RowItem.Cells(12).Value = "1"
                        Case "05"
                            RowItem.Cells(12).Value = "3"
                        Case "37"
                            RowItem.Cells(12).Value = "4"
                        Case "04"
                            RowItem.Cells(12).Value = "9"
                        Case Else
                            RowItem.Cells(12).Value = "2"
                    End Select
                    '�Ȗږ��̕ϊ��A�i�[
                    Select Case OBJ_DATAREADER.Item("KAMOKU_E")
                        '2011/06/16 �W���ŏC�� �Ȗڂ�01�̏ꍇ���� ------------------START
                        'Case "02"
                        Case "01"
                            '2011/06/16 �W���ŏC�� �Ȗڂ�01�̏ꍇ���� ------------------END
                            '����
                            RowItem.Cells(13).Value = "��"
                        Case "03"
                            '
                            RowItem.Cells(13).Value = "�["
                        Case "04"
                            '
                            RowItem.Cells(13).Value = "�E"
                        Case Else
                            '����
                            '���̑�
                            RowItem.Cells(13).Value = "��"
                    End Select
                    '�����ԍ��̊i�[
                    RowItem.Cells(14).Value = OBJ_DATAREADER.Item("KOUZA_E")

                    '�_��Җ��̊i�[
                    '2006/12/08�@�f�[�^�x�[�X�ɂ̓X�y�[�X�������Ă��邽�߁AIsDBNull�ł͋󔒔���ł��Ȃ�
                    If IsDBNull(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = True Then
                        RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = "" Then '�X�y�[�X�̏ꍇ�J�i�\��
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                        Else
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")
                        End If
                    End If

                    '�_��Ҕԍ��̊i�[
                    RowItem.Cells(16).Value = OBJ_DATAREADER.Item("KEIYAKU_NO_E")

                    '���z�̊i�[
                    RowItem.Cells(17).ReadOnly = False

                    If Trim(STR_���k���׏����l) <> "" Then
                        RowItem.Cells(17).Value = Format(CDbl(STR_���k���׏����l), "#,##0")
                    Else
                        RowItem.Cells(17).Value = Format(CDbl(OBJ_DATAREADER.Item("FURIKIN_E")), "#,##0")
                    End If

                    '�萔���̊i�[
                    RowItem.Cells(18).Value = Format(CDbl(OBJ_DATAREADER.Item("TESUU_E")), "#,##0")

                    For Cnt As Integer = 0 To RowItem.Cells.Count - 1
                        If Cnt <> 17 Then
                            RowItem.Cells(Cnt).Style.BackColor = Color.Yellow
                        End If
                    Next

                    INTCNT01 += 1

                    .Rows.Add(RowItem)
                End With
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            lab����.Text = Format(CDbl(INTCNT01), "#,##0")

            With DataGridView
                Dim SumKin As Decimal = 0
                For RowCnt As Integer = 0 To INTCNT01 - 1
                    SumKin += CDec(.Rows(RowCnt).Cells(17).Value)
                Next
                .RowCount = INTCNT01 + 1

                '���v���z�\���s
                .Rows(INTCNT01).Cells(17).ReadOnly = True
                .Rows(INTCNT01).Cells(17).Value = Format(SumKin, "#,##0")

            End With

            PFUNC_Spread_Set = True
        Catch ex As Exception
            MainLOG.Write("PFUNC_Spread_Set", "���s", ex.Message)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
    Private Function PFUNC_Update_ENTMAST() As Boolean

        '�G���g���}�X�^���X�V����
        PFUNC_Update_ENTMAST = False

        For lLoopCount As Integer = 0 To (DataGridView.RowCount - 1)
            With DataGridView
                If Trim(.Rows(lLoopCount).Cells(0).Value) <> "" Then
                    Select Case STR_���k���ד��o�敪
                        Case "2"
                            '�G���g���}�X�^�P����
                            STR_SQL = " UPDATE  G_ENTMAST1 SET "
                        Case "3"
                            '�G���g���}�X�^�Q����
                            STR_SQL = " UPDATE  G_ENTMAST2 SET "
                    End Select
                    '�����E�o�����z
                    If Trim(.Rows(lLoopCount).Cells(17).Value) = "" Then
                        STR_SQL += " FURIKIN_E = " & 0
                    Else
                        STR_SQL += " FURIKIN_E = " & CDec(.Rows(lLoopCount).Cells(17).Value.ToString.Trim)
                    End If
                    '�X�V��
                    STR_SQL += ", KOUSIN_DATE_E    ='" & Str_Syori_Date(0) & "'"
                    STR_SQL += " WHERE"
                    STR_SQL += " GAKKOU_CODE_E  ='" & STR_���k���׊w�Z�R�[�h & "'"
                    STR_SQL += " AND"
                    STR_SQL += " FURI_DATE_E  ='" & STR_���k���אU�֓� & "'"
                    STR_SQL += " AND"
                    STR_SQL += " NENDO_E  =" & CInt(Trim(.Rows(lLoopCount).Cells(0).Value))
                    STR_SQL += " AND"
                    STR_SQL += " TUUBAN_E  =" & CInt(Trim(.Rows(lLoopCount).Cells(1).Value))
                Else
                    Exit For
                End If
            End With

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '�X�V�����G���[
                MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Function
            End If
        Next lLoopCount

        PFUNC_Update_ENTMAST = True

    End Function
    Private Function PFUNC_Update_Schedule() As Boolean
        '�X�P�W���[���}�X�^�̍X�V

        PFUNC_Update_Schedule = False


        STR_SQL = " UPDATE  G_SCHMAST SET "
        STR_SQL += " CHECK_FLG_S ='1'"
        STR_SQL += ",CHECK_DATE_S ='" & Str_Syori_Date(0) & "'"
        STR_SQL += ",TIME_STAMP_S ='" & Str_Syori_Date(1) & "'"
        STR_SQL += " WHERE"

        STR_SQL += " GAKKOU_CODE_S ='" & STR_���k���׊w�Z�R�[�h & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S   ='" & STR_���k���אU�֓� & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S ='2'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S   ='" & STR_���k���ד��o�敪 & "'"
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "�X�V"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function

    Private Sub PSUB_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String, ByVal db As MyOracle)

        '���Z�@�փR�[�h�Ǝx�X�R�[�h������Z�@�֖��A�x�X���𒊏o

        Str_Ginko(0) = ""
        Str_Ginko(1) = ""
        Str_Ginko(2) = ""
        Str_Ginko(3) = ""

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            If pGinko_Code.Trim = "" OrElse pSiten_Code.Trim = "" Then
                Exit Sub
            End If

            Dim SQL As New System.Text.StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" KIN_KNAME_N ")
            SQL.Append(",KIN_NNAME_N ")
            SQL.Append(",SIT_KNAME_N ")
            SQL.Append(",SIT_NNAME_N ")
            SQL.Append(" FROM TENMAST ")
            SQL.Append(" WHERE KIN_NO_N = '" & pGinko_Code & "'")
            SQL.Append(" AND SIT_NO_N = '" & pSiten_Code & "'")

            Orareader = New CASTCommon.MyOracleReader(db)

            If Orareader.DataReader(SQL) Then
                Str_Ginko(0) = Orareader.GetItem("KIN_KNAME_N")
                Str_Ginko(1) = Orareader.GetItem("KIN_NNAME_N")
                Str_Ginko(2) = Orareader.GetItem("SIT_KNAME_N")
                Str_Ginko(3) = Orareader.GetItem("SIT_NNAME_N")
            Else
                Exit Sub
            End If

            Orareader.Close()
            Orareader = Nothing

        Catch ex As Exception
            Throw New Exception("TENMAST�擾���s", ex)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
        End Try

    End Sub
#End Region

    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl

    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        CType(sender, DataGridView).ImeMode = ImeMode.Disable
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        '�X�v���b�g�����ڑOZERO�l��
        With CType(sender, DataGridView)
            Select Case colNo
                Case 17
                    Dim str_Value As String
                    str_Value = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    If Not str_Value Is Nothing Then
                        If IsNumeric(str_Value) Then
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Format(CDec(str_Value), "#,##0")
                        End If
                    End If
            End Select
        End With

    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)

        Select Case colNo
            Case 17
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case colNo
            Case 17
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney

                '���v���z�\��������ǉ�
                With DataGridView
                    Dim SumKin As Decimal = 0
                    For RowCnt As Integer = 0 To INTCNT01 - 1
                        SumKin += CDec(.Rows(RowCnt).Cells(17).Value)
                    Next
                    .RowCount = INTCNT01 + 1

                    '���v���z�\���s
                    .Rows(INTCNT01).Cells(17).ReadOnly = True
                    .Rows(INTCNT01).Cells(17).Value = Format(SumKin, "#,##0")
                End With

        End Select

        Call CellLeave(sender, e)
    End Sub

    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView.RowPostPaint
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

End Class
