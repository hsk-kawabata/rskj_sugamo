Imports System.Text
Imports CASTCommon

''' <summary>
''' �U�����MCSV���G���^�쐬��ʁ@���C���N���X
''' </summary>
''' <remarks>
''' 2016/10/12 saitou added for RSV2�Ή�(�M�g)
''' �ߋE�Y�ƐM�g�x�[�X��RSV2�ɕW���g����
''' </remarks>
Public Class KFSMAIN120

#Region "�N���X�萔"

    '�C�x���g
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    '���b�Z�[�W
    Private Const msgTitle As String = "�U�����MCSV���G���^�쐬���(KFSMAIN120)"

#End Region

#Region "�N���X�ϐ�"

    '�C�x���g���O
    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN120", "�U�����MCSV���G���^�쐬���")
    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����R�[�h
        Dim FuriDate As String          '�U����
    End Structure
    Private LW As LogWrite

    Private HASSIN_DATE As String                   '���M��
    Private SOUFURI_DATE As String                  '���U�Ώۓ�
    Private KYUFURI_DATE As String                  '���U�Ώۓ�

    '�N���b�N������̔ԍ�
    Private ClickedColumn As Integer
    '�\�[�g�I�[�_�[�t���O
    Private SortOrderFlag As Boolean = True

#End Region

#Region "�C�x���g�n���h��"

    ''' <summary>
    ''' �t�H�[�����[�h�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KSFMAIN120_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            '--------------------------------------------------
            '���O�������ݕϐ��̏�����
            '--------------------------------------------------
            With LW
                .UserID = GCom.GetUserID
                .ToriCode = "0000000000-00"
                .FuriDate = "00000000"
            End With

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '--------------------------------------------------
            '�V�X�e�����t�ƃ��[�U����\��
            '--------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------------------------
            '���M���ɃV�X�e�����t��ݒ�
            '--------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Me.txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            Me.txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            Me.txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '--------------------------------------------------
            '��ʍ��ڂ̏�����
            '--------------------------------------------------
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �Q�ƃ{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnRef_Click(sender As Object, e As EventArgs) Handles btnRef.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")

            '--------------------------------------------------
            '�e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '--------------------------------------------------
            If Me.fn_check_text() = False Then
                Return
            End If

            '--------------------------------------------------
            '���X�g�r���[�̐ݒ�
            '--------------------------------------------------
            Dim iRet As Integer
            Me.ListView1.Items.Clear()

            Me.HASSIN_DATE = String.Concat(New String() {Me.txtKijyunDateY.Text, Me.txtKijyunDateM.Text, Me.txtKijyunDateD.Text})

            '���U�Ώۓ��͔��M���Ɠ���i�V�X�e�����t�j
            Me.SOUFURI_DATE = Me.HASSIN_DATE
            '���U�Ώۓ��͔��M���̂Q�c�Ɠ���i�V�X�e�����t�̂Q�c�Ɠ���j
            GCom.CheckDateModule(Me.HASSIN_DATE, Me.KYUFURI_DATE, 2, 0)

            iRet = Me.HassinDataList()

            If iRet = 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateY.Focus()
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �N���A�{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���A)�J�n", "����", "")

            '--------------------------------------------------
            '���M���ɃV�X�e�����t��ݒ�
            '--------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Me.txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            Me.txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            Me.txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '--------------------------------------------------
            '��ʍ��ڂ̏�����
            '--------------------------------------------------
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

    ''' <summary>
    ''' �S�I���{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAllOn_Click(sender As Object, e As EventArgs) Handles btnAllOn.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�J�n", "����", "")
            For Each item As ListViewItem In ListView1.Items
                item.Checked = True
            Next item
            Me.ListView1.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S�I��)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �S�����{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAllOff_Click(sender As Object, e As EventArgs) Handles btnAllOff.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�J�n", "����", "")
            For Each item As ListViewItem In ListView1.Items
                item.Checked = False
            Next item
            Me.ListView1.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�S����)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �쐬�{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(sender As Object, e As EventArgs) Handles btnAction.Click

        Dim db As CASTCommon.MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�J�n", "����", "")

            '--------------------------------------------------
            '���X�g�r���[�̃`�F�b�N
            '--------------------------------------------------
            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '���X�g�ɂP�����\������Ă��Ȃ��Ƃ�
            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '���X�g�ɂP���ȏ�\������Ă��邪�A�`�F�b�N����Ă��Ȃ��Ƃ�
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            If MessageBox.Show(String.Format(MSG0023I, "�U�����MCSV���G���^�쐬"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '--------------------------------------------------
            '�X�P�W���[���}�X�^�̍X�V
            '--------------------------------------------------
            db = New CASTCommon.MyOracle
            Dim SQL As New StringBuilder
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems

                Dim FURI_DATE As String = GCom.NzDec(item.SubItems(7).Text, "")
                Dim Temp As String = GCom.NzDec(item.SubItems(3).Text, "")
                Dim TORIS_CODE As String = Temp.Substring(0, 10)
                Dim TORIF_CODE As String = Temp.Substring(10)
                Dim MOTIKOMI_SEQ As String = GCom.NzDec(item.SubItems(8).Text, "0")

                LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                LW.FuriDate = FURI_DATE

                With SQL
                    .Length = 0
                    .Append("update S_SCHMAST set ")
                    If item.Checked = True Then
                        .Append("HASSIN_FLG_S = '2'")
                    Else
                        .Append("HASSIN_FLG_S = '0'")
                    End If
                    .Append(" where TORIS_CODE_S = '" & TORIS_CODE & "'")
                    .Append(" and TORIF_CODE_S = '" & TORIF_CODE & "'")
                    .Append(" and FURI_DATE_S = '" & FURI_DATE & "'")
                    .Append(" and MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
                End With

                Dim nRet As Integer = db.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    db.Rollback()
                    Return
                ElseIf nRet < 0 Then
                    Throw New Exception(String.Format(MSG0002E, "�X�V"))
                End If
            Next

            '--------------------------------------------------
            '�W���u�o�^
            '--------------------------------------------------
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            Dim jobid As String
            Dim para As String

            jobid = "S120"
            para = String.Concat(New String() {Me.HASSIN_DATE, ",", Me.KYUFURI_DATE, ",", Me.SOUFURI_DATE})

            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, db)
            If iRet = 1 Then
                db.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                Throw New Exception(String.Format(MSG0002E, "����"))
            End If

            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, db) = False Then
                Throw New Exception(MSG0005E)
            End If

            db.Commit()

            MessageBox.Show(String.Format(MSG0021I, "�U�����MCSV���G���^�쐬"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '--------------------------------------------------
            '��ʍ��ڂ̏�����
            '--------------------------------------------------
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
            db.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not db Is Nothing Then
                db.Close()
                db = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���s)�I��", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' ����{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFSPxxx("KFSP009")

        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")

            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)", "���s", "������")
                Return
            End If

            If MessageBox.Show(String.Format(MSG0013I, "�U�����MCSV���G���^�Ώۈꗗ"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '--------------------------------------------------
            '���[����p�f�[�^�쐬
            '--------------------------------------------------
            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems
                CreateCSV.OutputCsvData(Me.HASSIN_DATE)                                         '���M�\���
                CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace(",", ""))                 '����
                CreateCSV.OutputCsvData(SyoriDate)                                              '������
                CreateCSV.OutputCsvData(SyoriTime)                                              '�^�C���X�^���v
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(0, 10))                 '������R�[�h
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(11, 2))                 '����敛�R�[�h
                CreateCSV.OutputCsvData(item.SubItems(2).Text)                                  '����於�i�����j
                CreateCSV.OutputCsvData(item.SubItems(4).Text)                                  '�_����
                CreateCSV.OutputCsvData(item.SubItems(5).Text)                                  '�K�p���
                CreateCSV.OutputCsvData(item.SubItems(6).Text)                                  '���M�敪
                CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""))                 '�U����
                CreateCSV.OutputCsvData(item.SubItems(8).Text)                                  '����SEQ
                CreateCSV.OutputCsvData(item.SubItems(9).Text.Replace(",", ""))                 '�˗�����
                CreateCSV.OutputCsvData(item.SubItems(10).Text.Replace(",", ""), False, True)   '�˗����z
            Next
            CreateCSV.CloseCsv()

            '--------------------------------------------------
            '����������s
            '--------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            '�p�����[�^�ݒ�F���[�U�[ID,CSV�t�@�C����,3(�U�����MCSV���G���^)
            param = GCom.GetUserID & "," & strCSVFileName & ",3"
            iRet = ExeRepo.ExecReport("KFSP009.EXE", param)

            Select Case iRet
                Case 0
                    '�����m�F���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0014I, "�U�����MCSV���G���^�Ώۈꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '����ΏۂȂ����b�Z�[�W
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�����MCSV���G���^�Ώۈꗗ���", "���s", "����ΏۂȂ�")
                Case Else
                    '������s���b�Z�[�W
                    MessageBox.Show(String.Format(MSG0004E, "�U�����MCSV���G���^�Ώۈꗗ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�U�����MCSV���G���^�Ώۈꗗ���", "���s")
            End Select

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)��O�G���[", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���)�J�n", "����", "")
        End Try
    End Sub

    ''' <summary>
    ''' �I���{�^���N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(sender As Object, e As EventArgs) Handles btnEnd.Click
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

    ''' <summary>
    ''' ���X�g�r���[�J�����N���b�N�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>���X�g�r���[�̃\�[�g</remarks>
    Private Sub ListView1_ColumnClick(sender As Object, e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick
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

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X�o���f�[�e�B���O�C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>�e�L�X�g�{�b�N�X�̃[������</remarks>
    Private Sub TextBox_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles _
        txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

#Region "�v���C�x�[�g���\�b�h"

    ''' <summary>
    ''' ���M�Ώۂ̃��X�g�r���[��ݒ肵�܂��B
    ''' </summary>
    ''' <returns>�Ώۃ��R�[�h����</returns>
    ''' <remarks></remarks>
    Private Function HassinDataList() As Integer

        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim row As Integer = 0
        Dim Data(10) As String

        Try
            With SQL
                .Append("select *")
                .Append(" from S_SCHMAST")
                .Append(" inner join S_TORIMAST")
                .Append(" on TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                .Append(" where FSYORI_KBN_S = '3'")
                .Append(" and TOUROKU_FLG_S = '1'")
                .Append(" and HASSIN_FLG_S = '0'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and SOUSIN_KBN_S = '2'")          'CSV���G���^
                .Append(" and ((SYUBETU_S in ('11', '12') and FURI_DATE_S = '" & Me.KYUFURI_DATE & "')")
                .Append(" or   (SYUBETU_S = '21'          and FURI_DATE_S = '" & Me.SOUFURI_DATE & "'))")
                .Append(" order by")
                .Append(" FURI_DATE_S")
                .Append(",SOUSIN_KBN_S")
                .Append(",TORIS_CODE_S")
                .Append(",TORIF_CODE_S")
                .Append(",MOTIKOMI_SEQ_S")
            End With

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False

                    row += 1

                    '����
                    Data(1) = row.ToString("#,##0")
                    '����於
                    Data(2) = GCom.GetLimitString(OraReader.GetString("ITAKU_NNAME_T"), 40)
                    '�����R�[�h�i��R�[�h + "-" + ���R�[�h�j
                    Data(3) = OraReader.GetString("TORIS_CODE_T") & "-" & OraReader.GetString("TORIF_CODE_T")
                    '�U����
                    Dim furidate As String = OraReader.GetString("FURI_DATE_S")
                    Data(7) = furidate.Substring(0, 4) & "/" & furidate.Substring(4, 2) & "/" & furidate.Substring(6, 2)

                    '�����敪�擾
                    Dim koukin As Boolean = False
                    Select Case OraReader.GetString("SYUMOKU_CODE_T")
                        Case "01", "02", "03", "04"
                            '���ɋ��A�����A����(�w�������)�A����(�w����O�c�Ɠ�����)
                            koukin = True
                        Case Else
                            '��ʁA�����z����(���)�A�����z����(���s)�A�ݕt�z�����v�z�����A�N�����t��(�N���M��)�A�N�����t��(���I�N��)�A�N�����t��(��Õی�)
                            koukin = False
                    End Select

                    '�_����
                    Select Case OraReader.GetString("SYUBETU_S")
                        Case "11"
                            Data(4) = "���^"
                        Case "12"
                            Data(4) = "�ܗ^"
                        Case Else
                            Data(4) = "�U��"
                    End Select

                    If koukin Then
                        Data(4) &= "����"
                    End If

                    '�K�p���
                    Select Case OraReader.GetString("SYUBETU_S")
                        Case "11"
                            Data(5) = "���^"
                        Case "12"
                            Data(5) = "�ܗ^"
                        Case Else
                            Data(5) = "�U��"
                    End Select

                    If koukin Then
                        Data(5) &= "����"
                    End If

                    '���M�敪
                    Data(6) = "CSVش��"

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
                End While
            Else
                Return 0
            End If

            Return row

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�f�[�^����)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' �e�L�X�g�{�b�N�X�̕K�{�`�F�b�N���s���܂��B
    ''' </summary>
    ''' <returns>True - ���� , False - �ُ�</returns>
    ''' <remarks></remarks>
    Private Function fn_check_text() As Boolean
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)�J�n", "����", "")

            '���M���i�N�j�`�F�b�N
            If Me.txtKijyunDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateY.Focus()
                Return False
            End If

            '���M���i���j�`�F�b�N
            If Me.txtKijyunDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateM.Focus()
                Return False
            End If

            If Not (GCom.NzInt(Me.txtKijyunDateM.Text) >= 1 And GCom.NzInt(Me.txtKijyunDateM.Text) <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateM.Focus()
                Return False
            End If

            '���M���i���j�`�F�b�N
            If Me.txtKijyunDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateD.Focus()
                Return False
            End If

            If Not (GCom.NzInt(Me.txtKijyunDateD.Text) >= 1 And GCom.NzInt(Me.txtKijyunDateD.Text) <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateD.Focus()
                Return False
            End If

            '���M�����t�������`�F�b�N
            Dim WORK_DATE As String = String.Concat(New String() {Me.txtKijyunDateY.Text, "/", Me.txtKijyunDateM.Text, "/", Me.txtKijyunDateD.Text})
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���̓`�F�b�N)�I��", "����", "")
        End Try
    End Function

#End Region

End Class

