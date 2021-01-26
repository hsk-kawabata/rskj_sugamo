Option Strict On
Option Explicit On

Imports CASTCommon
Imports System.Text

Public Class KFSMAIN040

#Region "�ϐ��錾"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN040", "�U�����M���G���^�쐬���")
    Private Const msgTitle As String = "�U�����M���G���^�쐬���(KFSMAIN040)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U����
    End Structure

    Private LW As LogWrite

    Private KYUFURI_HANI As Integer             '���U�͈�
    Private SOUFURI_HANI As Integer             '���U�͈�
    Private HASSIN_DATE As String               '���M��
    Private KYUFURI_END_DATE As String          '���U�͈͏I����
    Private SOUFURI_END_DATE As String          '���U�͈͏I����
    Private EIGYOUBI_1_AFTER As String          '�P�c�Ɠ���
    Private EIGYOUBI_2_AFTER As String          '�Q�c�Ɠ���

    '�N���b�N������̔ԍ�
    Dim ClickedColumn As Integer
    '�\�[�g�I�[�_�[�t���O
    Dim SortOrderFlag As Boolean = True
#End Region

#Region "�֐�"
    Private Function HassinDataList() As Integer
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim row As Integer = 0
        Dim Data(10) As String

        Try
            MainDB = New MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As StringBuilder = New StringBuilder(128)

            SQL.AppendLine("SELECT *")

            SQL.AppendLine(" FROM S_SCHMAST, S_TORIMAST")

            SQL.AppendLine(" WHERE FSYORI_KBN_S = '3'")
            SQL.AppendLine(" AND TOUROKU_FLG_S = '1'")
            '���M�t���O�O�ƂQ��ΏۂƂ���
            SQL.AppendLine(" AND HASSIN_FLG_S IN ('0', '2')")
            SQL.AppendLine(" AND FURI_DATE_S >= '" & HASSIN_DATE & "'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            SQL.AppendLine(" AND (")
            SQL.AppendLine("(FURI_DATE_S <= '" & KYUFURI_END_DATE & "' AND SYUBETU_S IN ('11', '12'))")
            SQL.AppendLine(" OR (FURI_DATE_S <= '" & SOUFURI_END_DATE & "' AND SYUBETU_S = '21')")
            SQL.AppendLine(")")

            SQL.AppendLine(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S = TORIF_CODE_T ")

            SQL.AppendLine(" ORDER BY FURI_DATE_S")
            SQL.AppendLine(", SOUSIN_KBN_S")
            SQL.AppendLine(", TORIS_CODE_S")
            SQL.AppendLine(", TORIF_CODE_S")
            SQL.AppendLine(", MOTIKOMI_SEQ_S")

            If OraReader.DataReader(SQL) = True Then

                Do While OraReader.EOF = False

                    row += 1

                    'ListView�̒l�ݒ�

                    Data(1) = row.ToString("#,##0")
                    '����於
                    Data(2) = GCom.GetLimitString(OraReader.GetString("ITAKU_NNAME_T"), 40)
                    '�����R�[�h�i��R�[�h + "-" + ���R�[�h�j
                    Data(3) = OraReader.GetString("TORIS_CODE_T") & "-" & OraReader.GetString("TORIF_CODE_T")
                    '�U����
                    Dim furidate As String = OraReader.GetString("FURI_DATE_S")
                    Data(7) = furidate.Substring(0, 4) & "/" & furidate.Substring(4, 2) & "/" & furidate.Substring(6, 2)

                    '�����敪�擾
                    '�M�g�Ή�����Ȃ�X�ɒǉ��K�v�E�E�E
                    Dim koukin As Boolean = False
                    Select Case OraReader.GetString("SYUMOKU_CODE_T")
                        Case "01", "02"
                            koukin = True
                        Case Else
                            koukin = False
                    End Select

                    '�_����
                    Select Case OraReader.GetString("SYUBETU_S")
                        Case "11"
                            If koukin Then
                                Data(4) = "���^����"
                            Else
                                Data(4) = "���^"
                            End If
                        Case "12"
                            If koukin Then
                                Data(4) = "�ܗ^����"
                            Else
                                Data(4) = "�ܗ^"
                            End If
                        Case Else
                            If koukin Then
                                Data(4) = "�U������"
                            Else
                                Data(4) = "�U��"
                            End If
                    End Select

                    '�K�p���
                    Select Case furidate
                        Case HASSIN_DATE : Data(5) = "�U��"
                        Case EIGYOUBI_1_AFTER : Data(5) = "��U"
                        Case Else
                            Select Case OraReader.GetString("SYUBETU_S")
                                Case "11" : Data(5) = "���^"
                                Case "12" : Data(5) = "�ܗ^"
                                Case Else : Data(5) = "��U"
                            End Select
                    End Select

                    If koukin Then
                        Data(5) &= "����"
                    End If

                    '���M�敪
                    Select Case OraReader.GetString("SOUSIN_KBN_S")
                        Case "0"
                            Data(6) = "�ב֐U��"
                        Case "1"
                            Data(6) = "���M���O"
                        Case "2"
                            Data(6) = "CSVش��"
                        Case Else
                            Data(6) = "�ב֐U��"
                    End Select

                    '����SEQ
                    Data(8) = OraReader.GetString("MOTIKOMI_SEQ_S")

                    '2017/04/05 saitou �ߋE�Y�ƐM�g(RSV2�W��) MODIFY �C���v�b�g�G���[�������� ------------------------------------- START
                    Data(9) = (OraReader.GetInt64("SYORI_KEN_S") - OraReader.GetInt64("ERR_KEN_S")).ToString("###,###")
                    Data(10) = (OraReader.GetInt64("SYORI_KIN_S") - OraReader.GetInt64("ERR_KIN_S")).ToString("###,###")
                    'Data(9) = OraReader.GetInt64("SYORI_KEN_S").ToString("###,###")
                    'Data(10) = OraReader.GetInt64("SYORI_KIN_S").ToString("###,###")
                    '2017/04/05 saitou �ߋE�Y�ƐM�g(RSV2�W��) MODIFY -------------------------------------------------------------- END

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, Color.White, Nothing)

                    '�`�F�b�N�{�b�N�X����
                    Select Case GCom.NzDec(OraReader.GetString("KESSAI_PATN_T"), 0)
                        Case 0
                            If OraReader.GetString("KAKUHO_FLG_S") = "1" Then
                                vLstItem.Checked = True
                            End If
                        Case Else
                            vLstItem.Checked = True
                    End Select

                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})
                    
                    OraReader.NextRead()
                Loop
            Else
                Return 0    '0��
            End If

            Return row      '���� ����

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^����)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
#End Region

    '���LOAD��
    Private Sub KFSMAIN040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim strSysDate As String

        Try
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '------------------------------------------------
            '�V�X�e�����t�ƃ��[�U����\��
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '�x���}�X�^��荞��
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�x�����擾)", "���s", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʕ\�����ɃV�X�e�����t��\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '���U�^���U�͈�
            KYUFURI_HANI = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "KYUFURI"), 0)
            SOUFURI_HANI = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "SOUFURI"), 0)

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    '���s�{�^��������
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim MainDB As MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")

            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '���X�g�ɂP�����\������Ă��Ȃ��Ƃ�
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '���X�g�ɂP���ȏ�\������Ă��邪�A�`�F�b�N����Ă��Ȃ��Ƃ�
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim dRet As DialogResult
            dRet = MessageBox.Show(MSG0023I.Replace("{0}", "�U�����M���G���^�쐬"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If Not dRet = DialogResult.OK Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle
            Dim SQL As StringBuilder = New StringBuilder

            '***���ޭ��Ͻ��̍X�V
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems

                Dim FURI_DATE As String = GCom.NzDec(item.SubItems(7).Text, "")
                Dim Temp As String = GCom.NzDec(item.SubItems(3).Text, "")
                Dim TORIS_CODE As String = Temp.Substring(0, 10)
                Dim TORIF_CODE As String = Temp.Substring(10)
                Dim MOTIKOMI_SEQ As String = GCom.NzDec(item.SubItems(8).Text, "0")

                SQL.Length = 0
                LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                LW.FuriDate = FURI_DATE

                SQL.Append("UPDATE S_SCHMAST SET ")

                If item.Checked = True Then
                    SQL.Append("HASSIN_FLG_S = '2'")
                Else
                    SQL.Append("HASSIN_FLG_S = '0'")
                End If

                SQL.Append(" WHERE TORIS_CODE_S = '" & TORIS_CODE & "'")
                SQL.Append(" AND TORIF_CODE_S = '" & TORIF_CODE & "'")
                SQL.Append(" AND FURI_DATE_S = '" & FURI_DATE & "'")
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)

                Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    MainDB.Rollback()
                    Return
                ElseIf nRet < 0 Then
                    Throw New Exception(MSG0002E.Replace("{0}", "�o�^"))
                End If
            Next

            Dim jobid As String
            Dim para As String

            '�W���u�}�X�^�ɓo�^
            jobid = "S040"

            para = HASSIN_DATE                 '���M�����p�����^�Ƃ��Đݒ�
            para &= "," & KYUFURI_END_DATE
            para &= "," & SOUFURI_END_DATE

            '#########################
            'job����
            '#########################
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                Throw New Exception(MSG0002E.Replace("{0}", "����"))
            End If

            '#########################
            'job�o�^
            '#########################
            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                Throw New Exception(MSG0005E)
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "�U�����M���G���^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception

            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", ex.Message)

        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try
    End Sub

    '�I���{�^��������
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

#Region "KFSMAIN040�p�֐�"

    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :���s�{�^�����������ɕK�{���ڂ̃e�L�X�g�{�b�N�X�̓��͒l�`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================

        Try
            '------------------------------------------------
            '�U���N�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '�U�����`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (GCom.NzInt(txtKijyunDateM.Text) >= 1 And GCom.NzInt(txtKijyunDateM.Text) <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '�U�����`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtKijyunDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (GCom.NzInt(txtKijyunDateD.Text) >= 1 And GCom.NzInt(txtKijyunDateD.Text) <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtKijyunDateY.Text & "/" & txtKijyunDateM.Text & "/" & txtKijyunDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
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

    '�[������
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '�ꗗ�\���̈�̃\�[�g�ǉ�
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

        Try

            With CType(sender, ListView)

                If ClickedColumn = e.Column Then
                    ' ��������N���b�N�����ꍇ�́C�t���ɂ��� 
                    SortOrderFlag = Not SortOrderFlag
                End If

                ' ��ԍ��ݒ�
                ClickedColumn = e.Column

                ' �񐅕������z�u
                Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

                ' �\�[�g
                .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

                ' �\�[�g���s
                .Sort()

            End With

        Catch ex As Exception
            Throw
        End Try

    End Sub
#End Region

    Private Sub btnAllon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True '((CType(sender, Button).Name.ToUpper = "BTNALLON")) OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)", "���s", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�I��", "����", "")

        End Try

    End Sub

    Private Sub btnAlloff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�J�n", "����", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False 'OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)", "���s", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�I��", "����", "")

        End Try

    End Sub

    Private Sub btnRef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRef.Click

        Dim iRet As Integer

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")


            '�e�L�X�g�{�b�N�X���̓`�F�b�N
            If fn_check_text() = False Then
                Return
            End If

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            HASSIN_DATE = txtKijyunDateY.Text & txtKijyunDateM.Text & txtKijyunDateD.Text

            '�P�^�Q�c�Ɠ���
            GCom.CheckDateModule(HASSIN_DATE, EIGYOUBI_1_AFTER, 1, 0)
            GCom.CheckDateModule(HASSIN_DATE, EIGYOUBI_2_AFTER, 2, 0)

            '�����U�͈͎擾
            GCom.CheckDateModule(HASSIN_DATE, KYUFURI_END_DATE, KYUFURI_HANI, 0)
            GCom.CheckDateModule(HASSIN_DATE, SOUFURI_END_DATE, SOUFURI_HANI, 0)

            iRet = HassinDataList()

            If iRet = 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return
            ElseIf iRet = -1 Then

                Return

            Else

                Me.btnRef.Enabled = False
                Me.btnClear.Enabled = True
                Me.btnAllOn.Enabled = True
                Me.btnAllOff.Enabled = True
                Me.btnAction.Enabled = True
                Me.txtKijyunDateY.Enabled = False
                Me.txtKijyunDateM.Enabled = False
                Me.txtKijyunDateD.Enabled = False
                Me.btnAction.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)", "���s", ex.Message)

        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
        End Try

    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)�J�n", "����", "")

            '����ɃV�X�e�����t��\��
            '�x���}�X�^��荞��
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʕ\�����ɐU�����ɑO�c�Ɠ���\������
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '���X�g��ʂ̏�����
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False
            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)�I��", "����", "")
        End Try

    End Sub

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim mRet As Integer
        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFSPxxx("KFSP009")

        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", "������")
                Return
            End If

            mRet = MessageBox.Show(MSG0013I.Replace("{0}", "�U�����M���G���^�Ώۈꗗ"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If mRet <> DialogResult.OK Then
                Return
            End If

            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems
                CreateCSV.OutputCsvData(HASSIN_DATE)                                    '���M�\���
                CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace(",", ""))         '����
                CreateCSV.OutputCsvData(SyoriDate)                                      '������
                CreateCSV.OutputCsvData(SyoriTime)                                      '�^�C���X�^���v
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(0, 10))         '������R�[�h
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(11, 2))         '����敛�R�[�h
                CreateCSV.OutputCsvData(item.SubItems(2).Text)                          '����於�i�����j
                CreateCSV.OutputCsvData(item.SubItems(4).Text)                          '�_����
                CreateCSV.OutputCsvData(item.SubItems(5).Text)                          '�K�p���
                CreateCSV.OutputCsvData(item.SubItems(6).Text)                          '���M�敪
                CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""))         '�U����
                CreateCSV.OutputCsvData(item.SubItems(8).Text)                          '����SEQ
                CreateCSV.OutputCsvData(item.SubItems(9).Text.Replace(",", ""))         '�˗�����
                CreateCSV.OutputCsvData(item.SubItems(10).Text.Replace(",", ""), False, True) '�˗����z
          
            Next
            CreateCSV.CloseCsv()

            '����o�b�`�Ăяo��
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C�����A�����R�[�h
            '2016/02/04 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- START
            '�p�����[�^�ύX�@���O�C�����A�b�r�u�t�@�C�����A���o�b�`�g�p�t���O
            param = GCom.GetUserID & "," & strCSVFileName & ",0"
            'param = GCom.GetUserID & "," & strCSVFileName
            '2016/02/04 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- END
            iRet = ExeRepo.ExecReport("KFSP009.EXE", param)

            '2016/02/04 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- START
            '����������b�Z�[�W���o�͂���
            Select Case iRet
                Case 0
                    '�����m�F���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0014I, "�U�����M���G���^�Ώۈꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '����ΏۂȂ����b�Z�[�W
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�����M���G���^�Ώۈꗗ���", "���s", "����ΏۂȂ�")
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, "�U�����M���G���^�Ώۈꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�����M���G���^�Ώۈꗗ���", "���s")
            End Select

            'If iRet <> 0 Then
            '    '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
            '    Select Case iRet
            '        Case -1
            '            errMsg = MSG0226W.Replace("{0}", "�U�����M���G���^�Ώۈꗗ")
            '            MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        Case Else
            '            errMsg = MSG0004E.Replace("{0}", "�U�����M���G���^�Ώۈꗗ")
            '            MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    End Select

            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�����M���G���^�Ώۈꗗ���", "���s")
            '    Return
            'End If
            '2016/02/04 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- END

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
        End Try

    End Sub
End Class

