Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGNENJ021

#Region " ���ʕϐ���` "
    Dim STR���[�\�[�g�� As String
    Dim INTCNT01 As Integer
    Dim INTCNT02 As Integer
    '�ǉ� 2006/03/29
    Public strNENDO As String
    Public intTUUBAN As Integer

    Dim flg As Boolean = False

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ021", "�N���X�ւ��������")
    Private Const msgTitle As String = "�N���X�ւ��������(KFGNENJ021)"
    Private MainDB As MyOracle
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ021_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim strOLD_GAKKOU_NAME As String = "" '2008/03/12�@�ǉ�
        Dim OraReader As MyOracleReader = Nothing
        Dim OraReader2 As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Try
            '���O�p
            LW.UserID = GCom.GetUserID
            LW.ToriCode = STR_�N���X�֊w�Z�R�[�h
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")
            MainDB = New MyOracle


            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            labGAKKOU_CODE.Text = STR_�N���X�֊w�Z�R�[�h
            lab�w�Z��.Text = STR_�N���X�֊w�Z��
            labGAKUNEN.Text = STR_�N���X�֊w�N�R�[�h
            labGAKUNENMEI.Text = STR_�N���X�֊w�N��

            If PFUNC_GAKMAST2_GET() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", "�w�Z�}�X�^�Q�ɖ��o�^")
                Exit Sub
            End If

            '2007/08/27�@�X�v���b�h���ڕ\���ݒ�
            DataGridView1.Columns(7).Visible = False
            DataGridView1.Columns(8).Visible = False
            DataGridView1.Columns(9).Visible = False
            DataGridView1.Columns(10).Visible = False
            DataGridView1.Columns(11).Visible = False

            OraReader = New MyOracleReader(MainDB)
            OraReader2 = New MyOracleReader(MainDB)

            '���k�}�X�^�̓Ǎ���
            '2007/08/27�@�w�Z���擾��ǉ�
            SQL.Append(" SELECT SEITOMAST.*,GAKKOU_NNAME_G,GAKKOU_KNAME_G FROM SEITOMAST,GAKMAST1")
            SQL.Append(" WHERE GAKKOU_CODE_O = GAKKOU_CODE_G")
            SQL.Append(" AND GAKUNEN_CODE_O = GAKUNEN_CODE_G")
            SQL.Append(" AND GAKKOU_CODE_O =" & SQ(STR_�N���X�֊w�Z�R�[�h))
            SQL.Append(" AND GAKUNEN_CODE_O = " & GCom.NzInt(STR_�N���X�֊w�N�R�[�h))
            '�����S���Œ�Œ��o
            SQL.Append(" AND TUKI_NO_O ='04'")
            '---------------------------------------------------------------------------------------

            Select Case (STR���[�\�[�g��)
                Case "0"
                    '�w�N�A�N���X�A���k�ԍ�
                    SQL.Append(" ORDER BY GAKUNEN_CODE_O asc , CLASS_CODE_O asc , SEITO_NO_O ASC")
                Case "1"
                    '���w�N�x�A�ʔ�
                    SQL.Append(" ORDER BY NENDO_O asc , TUUBAN_O ASC")
                Case "2"
                    '���k���̃A�C�E�G�I��
                    SQL.Append(" ORDER BY SEITO_KNAME_O ASC")
            End Select

            If OraReader.DataReader(SQL) = False Then
                Exit Sub
            End If

            INTCNT01 = 0

            While OraReader.EOF = False
                Dim RowItem As New DataGridViewRow
                RowItem.CreateCells(DataGridView1)

                '���k�ԍ��̊i�[
                RowItem.Cells(1).Value = OraReader.GetString("SEITO_NO_O")

                '���k���̊i�[
                Select Case OraReader.GetString("SEITO_NNAME_O").Trim
                    Case ""
                        '���k�����J�i���i�[
                        RowItem.Cells(2).Value = OraReader.GetString("SEITO_KNAME_O")
                    Case Else
                        '���k�������̊i�[
                        RowItem.Cells(2).Value = OraReader.GetString("SEITO_NNAME_O")
                End Select

                '���ʂ̊i�[
                Select Case OraReader.GetString("SEIBETU_O")
                    Case "0"
                        RowItem.Cells(3).Value = "�j"
                    Case "1"
                        RowItem.Cells(3).Value = "��"
                    Case "2"
                        RowItem.Cells(3).Value = "�|"
                End Select

                '���N���X�̊i�[
                RowItem.Cells(4).Value = GCom.NzStr(OraReader.GetInt("CLASS_CODE_O"))
                '�V�N���X�̏�����
                RowItem.Cells(5).Value = ""
                '�V���k�ԍ��̏�����
                RowItem.Cells(6).Value = ""
                '���w�N�x
                RowItem.Cells(10).Value = GCom.NzStr(OraReader.GetString("NENDO_O"))
                '�ʔ�
                RowItem.Cells(11).Value = GCom.NzInt(OraReader.GetString("TUUBAN_O"))

                DataGridView1.Rows.Add(RowItem)

                INTCNT01 += 1
                OraReader.NextRead()
            End While

            '���̓{�^������
            btnUpd.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not OraReader2 Is Nothing Then OraReader2.Close()
        End Try

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnUpd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpd.Click

        '�X�V�{�^��

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�J�n", "����", "")
            LW.ToriCode = STR_�N���X�֊w�Z�R�[�h

            '�V�N���X�`�F�b�N
            For INTCNT02 = 0 To INTCNT01 - 1
                '�폜�`�F�b�N�{�b�N�X��OFF �ł��@�V�N���X�����͂���Ă���ꍇ
                If DataGridView1.Rows(INTCNT02).Cells(0).Value = False And Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) <> "" Then
                    If PFUNC_CLASS_CHK(Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value)) = False Then
                        Exit Sub
                    End If
                End If
            Next INTCNT02

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons. _
                           YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Return
            End If
            '�g�����U�N�V�����J�n
            MainDB.BeginTrans()

            '�X�v���b�h�̓��e�𐶓k�}�X�^���X�V����
            For INTCNT02 = 0 To INTCNT01 - 1 Step 1

                '�폜�`�F�b�N�{�b�N�X
                If DataGridView1.Rows(INTCNT02).Cells(0).Value = True Then
                    '�폜�w������̏ꍇ
                    If PFUNC_SEITOMAST_DEL() = False Then
                        Exit Sub
                    End If
                Else
                    '�폜�w���Ȃ��̏ꍇ
                    If PFUNC_SEITOMAST_UPD() = False Then
                        Exit Sub
                    End If
                End If
            Next INTCNT02

            '�g�����U�N�V�����I���i�b�n�l�l�h�s�j
            MainDB.Commit()

            Call MessageBox.Show(MSG0006I, msgTitle, _
                                     MessageBoxButtons.OK, MessageBoxIcon.Information)


            '���̓{�^������
            btnUpd.Enabled = False
            btnEnd.Enabled = True
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�X�V)�I��", "����", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
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
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKMAST2_GET() As Boolean

        '�w�Z�}�X�^�Q�̎擾

        PFUNC_GAKMAST2_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T =" & SQ(STR_�N���X�֊w�Z�R�[�h))

            If OraReader.DataReader(SQL) = False Then
                STR���[�\�[�g�� = "0"
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            Else
                STR���[�\�[�g�� = OraReader.GetString("MEISAI_OUT_T")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�w�Z�}�X�^����)", "���s", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_GAKMAST2_GET = True
    End Function
    Private Function PFUNC_SEITOMAST_DEL() As Boolean

        PFUNC_SEITOMAST_DEL = False
        Dim SQL As New StringBuilder
        Dim Ret As Integer
        Try
            '���k�}�X�^�̍폜
            SQL.Append(" DELETE  FROM SEITOMAST")
            SQL.Append(" WHERE GAKKOU_CODE_O =" & SQ(STR_�N���X�֊w�Z�R�[�h))
            SQL.Append(" AND GAKUNEN_CODE_O = " & GCom.NzInt(STR_�N���X�֊w�N�R�[�h))
            '�����ύX 2006/03/29
            SQL.Append(" AND NENDO_O = " & SQ(GCom.NzStr(DataGridView1.Rows(INTCNT02).Cells(10).Value)))
            SQL.Append(" AND TUUBAN_O =" & DataGridView1.Rows(INTCNT02).Cells(11).Value)

            Ret = MainDB.ExecuteNonQuery(SQL)
            Select Case Ret
                '�폜�����G���[
                Case Is <= 0
                    Throw New Exception("���k�}�X�^�̍폜�����Ɏ��s���܂����B")
                    Exit Function
            End Select
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���k�폜)", "���s", ex.ToString)
            Return False
        End Try

        PFUNC_SEITOMAST_DEL = True

    End Function
    Private Function PFUNC_SEITOMAST_UPD() As Boolean

        PFUNC_SEITOMAST_UPD = False
        Dim SQL As New StringBuilder
        Dim Ret As Integer
        Try
            '���k�}�X�^�̃N���X�A���k�ԍ��̍X�V
            Select Case (True)
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) = "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) = "")
                    '���͂Ȃ�(�X�V�Ȃ�)

                    PFUNC_SEITOMAST_UPD = True

                    Exit Function
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) <> "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) <> "")
                    '�V�N���X�A�V���k�ԍ��̓��͂���
                    SQL.Append(" UPDATE  SEITOMAST SET ")
                    SQL.Append(" CLASS_CODE_O = " & GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(5).Value))
                    SQL.Append(",SEITO_NO_O = " & SQ(Format(GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(6).Value), "0000000")))
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) <> "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) = "")
                    '�V�N���X�̂ݓ��͂���
                    SQL.Append(" UPDATE  SEITOMAST SET ")
                    SQL.Append(" CLASS_CODE_O = " & GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(5).Value))
                Case (Trim(DataGridView1.Rows(INTCNT02).Cells(5).Value) = "" And Trim(DataGridView1.Rows(INTCNT02).Cells(6).Value) <> "")
                    '�V���k�ԍ��̂ݓ��͂���
                    SQL.Append(" UPDATE  SEITOMAST SET ")
                    SQL.Append(" SEITO_NO_O = " & SQ(Format(GCom.NzInt(DataGridView1.Rows(INTCNT02).Cells(6).Value), "0000000")))
            End Select
            SQL.Append(",KOUSIN_DATE_O =" & SQ(Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd")))
            SQL.Append(" WHERE GAKKOU_CODE_O = " & SQ(STR_�N���X�֊w�Z�R�[�h))
            SQL.Append(" AND GAKUNEN_CODE_O = " & GCom.NzInt(STR_�N���X�֊w�N�R�[�h))
            '�����ύX 2006/03/29
            SQL.Append(" AND NENDO_O = " & SQ(GCom.NzStr(DataGridView1.Rows(INTCNT02).Cells(10).Value)))
            SQL.Append(" AND TUUBAN_O =" & DataGridView1.Rows(INTCNT02).Cells(11).Value)
          

            Ret = MainDB.ExecuteNonQuery(SQL)
            Select Case Ret
                '�폜�����G���[
                Case Is <= 0
                    Throw New Exception("���k�}�X�^�̍X�V�����Ɏ��s���܂����B")
                    Exit Function
            End Select

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���k�X�V)", "���s", ex.ToString)
            Return False
        End Try
        PFUNC_SEITOMAST_UPD = True

    End Function
    Private Function PFUNC_CLASS_CHK(ByVal STR�V�N���X As String) As Boolean

        Dim iCount As Integer

        '�N���X�`�F�b�N
        PFUNC_CLASS_CHK = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST1")
            SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(STR_�N���X�֊w�Z�R�[�h))
            SQL.Append(" AND GAKUNEN_CODE_G = " & GCom.NzInt(STR_�N���X�֊w�N�R�[�h))

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "����"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return False
            End If

            While OraReader.EOF = False
                For iCount = 1 To 20
                    '�N���X�R�[�h
                    '2006/10/19�@�V�N���X���o�^����Ă����ꍇ�̂݃N���X�ւ��������s�Ȃ�
                    If OraReader.GetInt("CLASS_CODE1" & Format(iCount, "00") & "_G") = GCom.NzInt(STR�V�N���X) Then
                        PFUNC_CLASS_CHK = True
                        Exit While
                    End If
                Next iCount
                OraReader.NextRead()
            End While
            If PFUNC_CLASS_CHK = False Then
                MessageBox.Show(String.Format(G_MSG0045W, DataGridView1.Rows(INTCNT02).Cells(1).Value), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return False
            End If
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���k�X�V)", "���s", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

#End Region

    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl

    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        CType(sender, DataGridView).ImeMode = ImeMode.Disable
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        With CType(sender, DataGridView)
            Dim str_Value As String = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value
            If Not str_Value Is Nothing Then
                If IsNumeric(str_Value) Then
                    Select Case colNo
                        Case 0 '�폜
                        Case 1 '���k�ԍ�
                        Case 2 '���k����
                        Case 3 '����
                        Case 4 '���N���X
                        Case 5 '�V�N���X
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = str_Value.Trim.PadLeft(2, "0"c)
                        Case 6 '�V���k�ԍ�
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = str_Value.Trim.PadLeft(7, "0"c)
                        Case 7 '�i�w�O�w�Z��
                        Case 8 '�i�w�O�N���X
                        Case 9 '�i�w�O���k�ԍ�
                        Case 10 '���w�N�x
                        Case 11 '�ʔ�
                    End Select
                End If
            End If
        End With

    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)

        Select Case colNo
            Case 0
            Case Else
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case colNo
            Case 0
            Case Else
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocus
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressNum
        End Select

        Call CellLeave(sender, e)
    End Sub

End Class
