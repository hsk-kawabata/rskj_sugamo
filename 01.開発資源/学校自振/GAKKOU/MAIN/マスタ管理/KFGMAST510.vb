Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient

Public Class KFGMAST510
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST510", "�w�Z�c�[���A�g�i�f�[�^�ڏo�j���")
    Private Const msgTitle As String = "�w�Z�c�[���A�g�i�f�[�^�ڏo�j���(KFGMAST510)"
    Private Const ThisFormName As String = "�w�Z�c�[���A�g�i�f�[�^�ڏo�j"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    Private Structure ThisFormUseData
        Dim TORIS_CODE As String
        Dim FURI_DATE As String
    End Structure
    Private DT As ThisFormUseData

    '�\�[�g�I�[�_�[�t���O
    Dim SortOrderFlag As Boolean = True

    '�N���b�N������̔ԍ�
    Dim ClickedColumn As Integer

    Private SourceDirectory As String = GCom.SET_PATH(System.IO.Path.GetTempPath) & "GData"
    Private DestinationDirectory As String
    Private DestinationDirectory_KEKKA As String
    Private strHENKAN_CSV_SEITO As String
    Private strHENKAN_CSV_GAKKO As String
    Private strHENKAN_CSV_HIMOKU As String
    Private strHENKAN_CSV_GAKKO2 As String
    Private strHENKAN_CSV_KEKKA As String
    Private strHENKAN_CSV_KITEN As String
    Private DRet As DialogResult

    Private Sub KFGMAST510_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            GCom.GetSysDate = Date.Now
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            Dim strDirectory As String

            strDirectory = CASTCommon.GetFSKJIni("GHENKAN", "SAVE_PATH")
            If InStrRev(strDirectory, "\") = strDirectory.Length Then
                DestinationDirectory = strDirectory.Substring(0, strDirectory.Length - 1)
            End If
            strDirectory = CASTCommon.GetFSKJIni("GHENKAN", "KEKKA_SAVE_PATH")
            If InStrRev(strDirectory, "\") = strDirectory.Length Then
                DestinationDirectory_KEKKA = strDirectory.Substring(0, strDirectory.Length - 1)
            End If


            '�ۑ��t�@�C����ini�t�@�C���擾�� + �w�Z�R�[�h.csv
            strHENKAN_CSV_SEITO = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_SEITO")
            strHENKAN_CSV_GAKKO = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_GAKKO")
            strHENKAN_CSV_HIMOKU = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_HIMOKU")
            strHENKAN_CSV_GAKKO2 = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_GAKKO2")
            strHENKAN_CSV_KEKKA = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_KEKKA")
            strHENKAN_CSV_KITEN = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_KITEN")

            Me.FURI_DATE.Value = Date.Now
            Me.FURI_DATE.CustomFormat = " yyyy �N MM �� dd �� dddd"

            '�J�����_�[�����ݒ�
            Dim Ret As Boolean = GCom.CheckDateModule(Nothing, 1)

            Me.CK_L.Text = ""

            '�X�v���b�h�̈��{�ݒ�
            With Me.ListView1
                .Clear()
                .Columns.Add("", 22, HorizontalAlignment.Center)
                .Columns.Add("�w�Z�R�[�h", 100, HorizontalAlignment.Center)
                .Columns.Add("�w�Z��", 250, HorizontalAlignment.Left)
                .Columns.Add("�w�Z�J�i��", 0, HorizontalAlignment.Left)
                .Columns.Add("�U�֓�", 100, HorizontalAlignment.Center)
                .Columns.Add("�ĐU��", 100, HorizontalAlignment.Center)
                .Columns.Add("�Ώ۔N��", 0, HorizontalAlignment.Center)
            End With

            Me.CheckBox1.Checked = True

            If Me.CheckBox1.Checked = False Then
                Label13.Enabled = False
                txtSYear.Enabled = False
                Label12.Enabled = False
                txtSMonth.Enabled = False
                Label11.Enabled = False

                FURI_DATE_L.Enabled = True
                FURI_DATE.Enabled = True
            Else
                Label13.Enabled = True
                txtSYear.Enabled = True
                Label12.Enabled = True
                txtSMonth.Enabled = True
                Label11.Enabled = True

                FURI_DATE_L.Enabled = False
                FURI_DATE.Enabled = False

                txtSYear.Text = Format(Now, "yyyy")
                txtSMonth.Text = Format(Now, "MM")

            End If

            Me.CmdSelect.Enabled = True

            Application.DoEvents()
            Me.FURI_DATE.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    '�����N���Ώۃ`�F�b�N�{�b�N�X�ύX
    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        Me.ListView1.Items.Clear()

        If Me.CheckBox1.Checked = False Then
            Label13.Enabled = False
            txtSYear.Enabled = False
            Label12.Enabled = False
            txtSMonth.Enabled = False
            Label11.Enabled = False

            FURI_DATE_L.Enabled = True
            FURI_DATE.Enabled = True
        Else
            Label13.Enabled = True
            txtSYear.Enabled = True
            Label12.Enabled = True
            txtSMonth.Enabled = True
            Label11.Enabled = True

            FURI_DATE_L.Enabled = False
            FURI_DATE.Enabled = False

            txtSYear.Text = Format(Now, "yyyy")
            txtSMonth.Text = Format(Now, "MM")
        End If

        Me.FURI_DATE.Focus()
    End Sub

    '���t�`�F�b�N
    Private Sub FURI_DATE_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FURI_DATE.ValueChanged
        Call CK_EigyouDay()
    End Sub

    '�ꗗ�\���̈�̃\�[�g
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

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
    End Sub

    '
    ' �@�\�@ �F �c�Ɠ�����֐�
    '
    ' �����@ �F �Ȃ�
    '
    ' �߂�l �F �c�Ɠ� = False, ��c�Ɠ� = True
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Function CK_EigyouDay() As Boolean
        Try
            '------------------------------------------------
            '�S�x������~��
            '------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�x�����擾)", "���s", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If

            Dim ChangeDate As String = ""
            Dim CurrDate As String = String.Format("{0:yyyyMMdd}", Me.FURI_DATE.Value)
            Dim FLG As Boolean = GCom.CheckDateModule(CurrDate, ChangeDate, 0)

            Me.CK_L.Visible = (Not FLG)
            Me.CmdSelect.Enabled = FLG


        Catch ex As Exception

        End Try
    End Function

    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSelect.Click
        Call Get_GakkouMonitor()
    End Sub

    '�ꗗ�\���̈�s�f�[�^�\��
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    ' �@�\�@ �F �Ώۊw�Z�ꗗ��ݒ肷��
    '
    ' �����@ �F �Ȃ�
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Sub Get_GakkouMonitor()

        Me.SuspendLayout()
        Dim SQL As String
        Dim MSG As String
        Dim BRet As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim RC2 As OracleDataReader = Nothing
        Dim onText(2) As Integer
        Dim onDate As Date

        Me.CK_L.Text = ""

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�J�n", "����", "")

            SQL = "SELECT GAKKOU_CODE_S"
            SQL &= " , GAKKOU_NNAME_G"
            SQL &= " , GAKKOU_KNAME_G"
            SQL &= " , NENGETUDO_S"
            SQL &= " , FURI_DATE_S"
            SQL &= " , SFURI_DATE_S"
            SQL &= " FROM (SELECT DISTINCT GAKKOU_CODE_G,GAKKOU_NNAME_G, GAKKOU_KNAME_G FROM GAKMAST1),G_SCHMAST"
            SQL &= " WHERE GAKKOU_CODE_G = GAKKOU_CODE_S"
            Select Case Me.CheckBox1.Checked
                Case True
                    SQL &= " AND NENGETUDO_S = '" & txtSYear.Text + txtSMonth.Text & "'"
                Case Else
                    SQL &= " AND FURI_DATE_S = '" & String.Format("{0:yyyyMMdd}", Me.FURI_DATE.Value) & "'"
            End Select
            SQL &= " AND SCH_KBN_S IN ('0','1')"
            SQL &= " AND FURI_KBN_S = '0'"
            'SQL &= " AND DATA_FLG_S = '0'"
            SQL &= " ORDER BY FURI_DATE_S,GAKKOU_CODE_S"

            Me.ListView1.Items.Clear()

            BRet = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read
                    If Not RC2 Is Nothing Then
                        RC2.Close()
                        RC2.Dispose()
                    End If

                    Dim Data(6) As String

                    '�w�Z�R�[�h
                    Data(1) = GCom.NzDec(REC.Item("GAKKOU_CODE_S"), "")

                    '�w�Z��
                    Data(2) = GCom.NzStr(REC.Item("GAKKOU_NNAME_G")).Trim

                    '�w�Z�J�i��
                    Data(3) = GCom.NzStr(REC.Item("GAKKOU_KNAME_G")).Trim

                    '�U�֓�
                    Data(4) = GCom.NzDec(REC.Item("FURI_DATE_S"), "")
                    If Data(4).Length >= 8 Then
                        onText(0) = GCom.NzInt(Data(4).Substring(0, 4))
                        onText(1) = GCom.NzInt(Data(4).Substring(4, 2))
                        onText(2) = GCom.NzInt(Data(4).Substring(6, 2))
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            Data(4) = String.Format("{0:yyyy/MM/dd}", onDate)
                        End If
                    End If

                    '�ĐU�֓�
                    Data(5) = GCom.NzDec(REC.Item("SFURI_DATE_S"), "")
                    If Data(5).Length >= 8 Then
                        onText(0) = GCom.NzInt(Data(5).Substring(0, 4))
                        onText(1) = GCom.NzInt(Data(5).Substring(4, 2))
                        onText(2) = GCom.NzInt(Data(5).Substring(6, 2))
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            Data(5) = String.Format("{0:yyyy/MM/dd}", onDate)
                        End If
                    End If
                    If Data(5) = "00000000" Then
                        Data(5) = ""
                    End If

                    '�Ώ۔N��
                    Data(6) = GCom.NzStr(REC.Item("NENGETUDO_S")).Trim

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, Color.White, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                Loop

                Me.ListView1.Focus()
            Else
                MSG = "�ΏۃX�P�W���[��������܂���B" & Space(5)
                DRet = MessageBox.Show(MSG, _
                        "�w�Z�c�[���A�g�f�[�^�ڏo����", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                LW.FuriDate = String.Format("{0:yyyyMMdd}", Me.FURI_DATE.Value)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ΏۃX�P�W���[��������܂���", "���s", "")

            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not RC2 Is Nothing Then
                RC2.Close()
                RC2.Dispose()
            End If
        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not RC2 Is Nothing Then
                RC2.Close()
                RC2.Dispose()
            End If
            Me.ResumeLayout()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�Q��)�I��", "����", "")
        End Try
    End Sub

    Private Sub CmdCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdCreate.Click
        Dim MSG As String = ""
        Dim Temp As String
        Dim BRet As Boolean
        Dim LBRet As Boolean

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)�J�n", "����", "")

            Dim BreakFast As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '���X�g�ɂP�����\������Ă��Ȃ��Ƃ�
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '���X�g�ɂP���ȏ�\������Ă��邪�A�`�F�b�N����Ă��Ȃ��Ƃ�
                If BreakFast.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            For Row As Integer = 0 To BreakFast.Count - 1 Step 1

                Me.SuspendLayout()
                Dim lsvItem As ListViewItem.ListViewSubItemCollection = BreakFast.Item(Row).SubItems
                Dim Data(6) As String

                For Idx As Integer = 0 To 6 Step 1
                    Select Case Idx
                        Case 1, 2, 3, 4, 5, 6
                            Data(Idx) = lsvItem.Item(Idx).Text
                    End Select
                Next Idx

                '�A�g�̊w�Z(�悸�쐬��̃t�H���_����ɂ���)
                If System.IO.Directory.Exists(SourceDirectory) Then
                    Dim DFL() As String = System.IO.Directory.GetFiles(SourceDirectory)
                    For Each Temp In DFL
                        System.IO.File.Delete(Temp)
                    Next Temp
                Else
                    System.IO.Directory.CreateDirectory(SourceDirectory)
                End If
                Application.DoEvents()


                '�w�Z���1CSV (GAKKO+�w�Z�R�[�h.CSV)
                BRet = Set_GAKKO(Data)

                '�w�Z���2CSV (GAKKO2+�w�Z�R�[�h.CSV)
                If BRet Then
                    BRet = Set_GAKKO2(Data(1))
                End If

                '��ڏ��CSV (HIMOKU+�w�Z�R�[�h.CSV) 
                If BRet Then
                    BRet = Set_HIMOKU(Data(1), Data(6))
                End If

                '�_��ҏ��o�^CSV (SEITO+�w�Z�R�[�h.CSV)
                If BRet Then
                    BRet = Set_SEITO(Data(1), Data(6))
                End If


                '���Z�@�֏��CSV (BANK+�w�Z�R�[�h.CSV) 
                If BRet Then
                    BRet = Set_BANK_INF(Data(1))
                End If

                LBRet = False

                Dim onDate As Date

                MSG = "(" & Data(1) & ") " & Data(2)
                MSG &= New String(ControlChars.Cr, 2)
                MSG &= "�f�[�^�ڏo���������s���Ă���낵���ł����H" & Space(5)
                DRet = MessageBox.Show(MSG, "�w�Z�c�[���A�g�ڏo����", MessageBoxButtons.OKCancel, _
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                BRet = (DRet = DialogResult.OK)

                '�ڏo�L�����Z���ꍇ
                If BRet = False Then
                    Exit Sub
                End If


                '���ʊ֐�CALL
                LBRet = CopyInterfaceFile(0, Data(1), Data(4), Data(5), onDate)

                MSG = "(" & Data(1) & ") " & Data(2)
                MSG &= New String(ControlChars.Cr, 2)
                If LBRet = True Then
                    MSG &= "�f�[�^�ڏo�������������܂����B" & Space(5)
                    DRet = MessageBox.Show(MSG, "�w�Z�c�[���A�g�ڏo����", MessageBoxButtons.OK, _
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                Else
                    MSG &= "�f�[�^�ڏo���������s���܂����B" & Space(5)
                    DRet = MessageBox.Show(MSG, "�w�Z�c�[���A�g�ڏo����", MessageBoxButtons.OK, _
                        MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                End If
                BRet = (DRet = DialogResult.OK)

                Me.ResumeLayout()
                Application.DoEvents()

            Next Row

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            Application.DoEvents()
            Me.FURI_DATE.Focus()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo)�I��", "����", "")
        End Try
    End Sub

    Private Sub CmdFinal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdFinal.Click
        Me.Close()
        Me.Dispose()
    End Sub

    '
    ' �@�\�@ �F �쐬�f�[�^���w��t�H���_�ɕ��ʂ���
    '
    ' �����@ �F ARG1 - �����񐔁i�g�p���Ă��Ȃ��j
    ' �@�@�@ �@ ARG2 - �w�Z�R�[�h
    ' �@�@�@ �@ ARG3 - �U�֓�
    ' �@�@�@ �@ ARG4 - �ĐU�֓�
    ' �@�@�@ �@ ARG5 - ���ʓ��t
    '
    ' �߂�l �F OK=True, NG=False
    '
    ' ���l�@ �F �w�Z�X�P�W���[���X�V�@�\�܂�
    '
    Private Function CopyInterfaceFile(ByRef STATUS As Integer, ByVal TORIS_CODE As String, _
        ByVal FURI_DATE As String, ByVal SFURI_DATE As String, ByVal ONDATE As Date) As Boolean
        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = FURI_DATE
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-����)�J�n", "����", "")

            Dim FD() As String

            FD = System.IO.Directory.GetDirectories(DestinationDirectory) '.Substring(0, 3))

            If Not FD Is Nothing AndAlso FD.Length > 0 Then
                If System.IO.Directory.Exists(DestinationDirectory) Then
                Else
                    '�w��f�B���N�g�����Ȃ��׍쐬����
                    System.IO.Directory.CreateDirectory(DestinationDirectory)
                End If
            Else
                '�w��f�B���N�g�����Ȃ��׍쐬����
                System.IO.Directory.CreateDirectory(DestinationDirectory)
            End If

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-����)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        End Try

        Try

            '�ڏo����
            Dim SFL() As String = System.IO.Directory.GetFiles(SourceDirectory)

            For Each SourcePath As String In SFL

                Dim DestinationPath As String


                DestinationPath = GCom.SET_PATH(DestinationDirectory)

                DestinationPath &= System.IO.Path.GetFileName(SourcePath)

                System.IO.File.Copy(SourcePath, DestinationPath, True)

                ONDATE = System.DateTime.Now
                Call System.IO.File.SetLastWriteTime(SourcePath, ONDATE)
            Next SourcePath

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-����)", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        End Try

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-����)�I��", "����", "")
        Return True

    End Function

    '
    ' �@�\�@ �F �w�Z���CSV (GAKKO.CSV) �쐬
    '
    ' �����@ �F Data()
    '
    ' �߂�l �F OK=True, NG=False
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Function Set_GAKKO(ByVal DATA() As String) As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Try

            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = DATA(1)
            LW.FuriDate = DATA(4).Substring(0, 4) & DATA(4).Substring(5, 2) & DATA(4).Substring(8, 2)
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�w�Z���CSV))�J�n", "����", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_GAKKO & DATA(1) & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim SQL As String = "SELECT SIYOU_GAKUNEN_T,SAIKOU_GAKUNEN_T,TKIN_NO_T,TSIT_NO_T,KIN_NNAME_N,SIT_NNAME_N"
            SQL &= " FROM GAKMAST2,TENMAST"
            SQL &= " WHERE GAKKOU_CODE_T = '" & DATA(1) & "'"
            SQL &= " AND TKIN_NO_T = KIN_NO_N AND TSIT_NO_T = SIT_NO_N"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = DATA(1) & ","
                    LineData &= DATA(2) & ","
                    LineData &= DATA(3) & ","
                    LineData &= GCom.NzDec(REC.Item("SIYOU_GAKUNEN_T"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("SAIKOU_GAKUNEN_T"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("TKIN_NO_T")) & ","
                    LineData &= GCom.NzStr(REC.Item("KIN_NNAME_N")) & ","
                    LineData &= GCom.NzStr(REC.Item("TSIT_NO_T")) & ","
                    LineData &= GCom.NzStr(REC.Item("SIT_NNAME_N")) & ","
                    LineData &= "00000000" & ","
                    LineData &= "00000000" & ","
                    LineData &= DATA(4).Substring(0, 4) & DATA(4).Substring(5, 2) & DATA(4).Substring(8, 2) & ","
                    If DATA(5) = "" Then
                        LineData &= "00000000" & ","
                    Else
                        LineData &= DATA(5).Substring(0, 4) & DATA(5).Substring(5, 2) & DATA(5).Substring(8, 2) & ","
                    End If
                    LineData &= "00000000" & ","
                    LineData &= "0" & ","
                    LineData &= "1"

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True

        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�w�Z���CSV))", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�w�Z���CSV))�I��", "����", "")
        End Try

    End Function

    '
    ' �@�\�@ �F �w�Z�w�N�N���X���CSV (GAKKO2.CSV) �쐬
    '
    ' �����@ �F ARG1 - �w�Z�R�[�h
    '
    ' �߂�l �F OK=True, NG=False
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Function Set_GAKKO2(ByVal TORIS_CODE As String) As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�w�Z�w�N�N���X���CSV))�J�n", "����", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_GAKKO2 & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim SQL As String = "SELECT * FROM GAKMAST1"
            SQL &= " WHERE GAKKOU_CODE_G = '" & TORIS_CODE & "'"
            SQL &= " ORDER BY GAKUNEN_CODE_G ASC"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = GCom.NzDec(REC.Item("GAKKOU_CODE_G"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("GAKUNEN_CODE_G"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("GAKUNEN_NAME_G")).Trim.PadLeft(1, " "c) & ","
                    For Cnt As Integer = 1 To 19 Step 1
                        Dim Temp As String = (100 + Cnt).ToString
                        LineData &= GCom.NzDec(REC.Item("CLASS_CODE" & Temp & "_G"), "") & ","
                        LineData &= GCom.NzStr(REC.Item("CLASS_NAME" & Temp & "_G")).Trim.PadLeft(1, " "c) & ","
                    Next Cnt
                    LineData &= GCom.NzDec(REC.Item("CLASS_CODE120_G"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("CLASS_NAME120_G")).Trim.PadLeft(1, " "c)

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True
        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�w�Z�w�N�N���X���CSV))", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�w�Z�w�N�N���X���CSV))�I��", "����", "")
        End Try

    End Function

    '
    ' �@�\�@ �F ��ڏ��CSV (HIMOKU.CSV) �쐬
    '
    ' �����@ �F ARG1 - �w�Z�R�[�h
    ' �@�@�@ �@ ARG2 - �U�֓�
    '
    ' �߂�l �F OK=True, NG=False
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Function Set_HIMOKU(ByVal TORIS_CODE As String, ByVal FURI_MMDD As String) As Boolean
        Dim Temp As String
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Dim SQL As String
        Dim BRet As Boolean
        Try

            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(���CSV�쐬))�J�n", "����", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_HIMOKU & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            SQL = "SELECT ' ' KBN"
            SQL &= ", GAKKOU_CODE_H"
            SQL &= ", GAKUNEN_CODE_H"
            SQL &= ", HIMOKU_ID_H"
            SQL &= ", HIMOKU_ID_NAME_H"
            SQL &= ", TUKI_NO_H"
            For Idx As Integer = 1 To 10 Step 1
                Temp = String.Format("{0:00}", Idx)
                SQL &= ", HIMOKU_NAME" & Temp & "_H"
                SQL &= ", KESSAI_KIN_CODE" & Temp & "_H"
                SQL &= ", KESSAI_TENPO" & Temp & "_H"
                SQL &= ", KESSAI_KAMOKU" & Temp & "_H"
                SQL &= ", KESSAI_KOUZA" & Temp & "_H"
                SQL &= ", KESSAI_MEIGI" & Temp & "_H"
                SQL &= ", HIMOKU_KINGAKU" & Temp & "_H"
            Next Idx
            SQL &= " FROM HIMOMAST"
            SQL &= " WHERE GAKKOU_CODE_H = '" & TORIS_CODE & "'"
            SQL &= " ORDER BY"
            SQL &= "      GAKUNEN_CODE_H ASC"
            SQL &= "    , HIMOKU_ID_H ASC"
            SQL &= "    , TUKI_NO_H ASC"

            BRet = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = GCom.NzStr(REC.Item("KBN")) & ","
                    LineData &= GCom.NzDec(REC.Item("GAKKOU_CODE_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("GAKUNEN_CODE_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("HIMOKU_ID_H"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("HIMOKU_ID_NAME_H")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("TUKI_NO_H"), "") & ","
                    For Cnt As Integer = 1 To 9 Step 1
                        Temp = Cnt.ToString.PadLeft(2, "0"c)
                        LineData &= GCom.NzStr(REC.Item("HIMOKU_NAME" & Temp & "_H")).Trim.PadLeft(1, " "c) & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_KIN_CODE" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_TENPO" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_KAMOKU" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_KOUZA" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_MEIGI" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("HIMOKU_KINGAKU" & Temp & "_H"), "") & ","
                    Next Cnt
                    LineData &= GCom.NzStr(REC.Item("HIMOKU_NAME10_H")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_KIN_CODE10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_TENPO10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_KAMOKU10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_KOUZA10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_MEIGI10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("HIMOKU_KINGAKU10_H"), "")

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True
        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(���CSV�쐬))", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(���CSV�쐬))�I��", "����", "")

        End Try

    End Function

    '
    ' �@�\�@ �F �_��ҏ��o�^CSV (SEITO+�w�Z�R�[�h.CSV) �쐬
    '
    ' �����@ �F ARG1 - �w�Z�R�[�h
    ' �@�@�@ �@ ARG2 - �U�֓�
    '
    ' �߂�l �F OK=True, NG=False
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Function Set_SEITO(ByVal TORIS_CODE As String, ByVal FURI_MMDD As String) As Boolean
        Dim Temp As String
        Dim Tuki As String
        Dim REC As OracleDataReader = Nothing
        Dim RECA As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Try

            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�_��ҏ��o�^CSV�쐬))�J�n", "����", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_SEITO & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            If FURI_MMDD.Length >= 6 Then
                Tuki = FURI_MMDD.Substring(4, 2)
            Else
                Tuki = "00"
            End If

            Dim SQL As String = "SELECT ' ' KBN"
            SQL &= ", GAKKOU_CODE_O"
            SQL &= ", NENDO_O"
            SQL &= ", TUUBAN_O"
            SQL &= ", GAKUNEN_CODE_O"
            SQL &= ", CLASS_CODE_O"
            SQL &= ", SEITO_NO_O"
            SQL &= ", SEITO_KNAME_O"
            SQL &= ", SEITO_NNAME_O"
            SQL &= ", SEIBETU_O"
            SQL &= ", HIMOKU_ID_O"
            For Idx As Integer = 1 To 10 Step 1
                Temp = String.Format("{0:00}", Idx)
                SQL &= ", SEIKYU" & Temp & "_O"
                SQL &= ", KINGAKU" & Temp & "_O"
            Next Idx
            SQL &= ", 0 TOTAL"
            SQL &= ", TKIN_NO_O"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", TSIT_NO_O"
            SQL &= ", SIT_NNAME_N"
            SQL &= ", KAMOKU_O"
            SQL &= ", KOUZA_O"
            SQL &= ", MEIGI_KNAME_O"
            SQL &= ", MEIGI_NNAME_O"
            SQL &= ", FURIKAE_O"
            SQL &= ", KEIYAKU_NJYU_O"
            SQL &= ", KEIYAKU_DENWA_O"
            SQL &= ", KAIYAKU_FLG_O"
            SQL &= " FROM SEITOMASTVIEW"
            SQL &= ", (SELECT KIN_NO_N"                 'BANK-Code
            SQL &= ", SIT_NO_N"                         'BRANCH-Code
            SQL &= ", KIN_NNAME_N"                      'BANK-Name
            SQL &= ", SIT_NNAME_N"                      'BRANCH-Name
            SQL &= " FROM TENMAST"
            SQL &= ")"
            SQL &= " WHERE GAKKOU_CODE_O = '" & TORIS_CODE & "'"


            SQL &= " AND TUKI_NO_O = '" & Tuki & "'"

            SQL &= " AND TKIN_NO_O = KIN_NO_N (+)"
            SQL &= " AND TSIT_NO_O = SIT_NO_N (+)"
            SQL &= " ORDER BY GAKUNEN_CODE_O ASC"
            SQL &= ", CLASS_CODE_O ASC"
            SQL &= ", SEITO_NO_O ASC"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = GCom.NzStr(REC.Item("KBN")) & ","
                    LineData &= GCom.NzDec(REC.Item("GAKKOU_CODE_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("NENDO_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("TUUBAN_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("GAKUNEN_CODE_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("CLASS_CODE_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("SEITO_NO_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("SEITO_KNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzStr(REC.Item("SEITO_NNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("SEIBETU_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("HIMOKU_ID_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("TOTAL"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("TKIN_NO_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("KIN_NNAME_N")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("TSIT_NO_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("SIT_NNAME_N")).Trim.PadLeft(1, " "c) & ","

                    '1:�����A2:����
                    If String.Format("{0:0}", GCom.NzInt(GCom.NzDec(REC.Item("KAMOKU_O"))), "") = "1" Then
                        LineData &= "1" & ","
                    Else
                        LineData &= "2" & ","
                    End If

                    LineData &= GCom.NzDec(REC.Item("KOUZA_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("MEIGI_KNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzStr(REC.Item("MEIGI_NNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("FURIKAE_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("KEIYAKU_NJYU_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzStr(REC.Item("KEIYAKU_DENWA_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("KAIYAKU_FLG_O"), "") & ","

                    For Cnt As Integer = 1 To 10 Step 1
                        Temp = Cnt.ToString.PadLeft(2, "0"c)

                        LineData &= GCom.NzDec(REC.Item("SEIKYU" & Temp & "_O"), "") & ","
                        If Cnt = 10 Then
                            LineData &= GCom.NzDec(REC.Item("KINGAKU" & Temp & "_O"), "")
                        Else
                            LineData &= GCom.NzDec(REC.Item("KINGAKU" & Temp & "_O"), "") & ","
                        End If
                        If Not RECA Is Nothing Then
                            RECA.Close()
                            RECA.Dispose()
                        End If
                    Next Cnt

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True
        Catch ex As Exception

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�_��ҏ��o�^CSV�쐬))", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not RECA Is Nothing Then
                RECA.Close()
                RECA.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(�_��ҏ��o�^CSV�쐬))�I��", "����", "")

        End Try

    End Function

    '
    ' �@�\�@ �F ���Z�@�֏��CSV (BANK.CSV) �쐬
    '
    ' �����@ �F ARG1 - �w�Z�R�[�h
    '
    ' �߂�l �F OK=True, NG=False
    '
    ' ���l�@ �F �Ȃ�
    '
    Private Function Set_BANK_INF(ByVal TORIS_CODE As String) As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Dim SELF_BANK_CODE As String                            'INI�t�@�C������̎����ɃR�[�h
        'Dim SELF_BANK_NAME As String                            'INI�t�@�C������̎����ɃR�[�h
        Dim LineData As String

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(���Z�@��CSV�쐬))�J�n", "����", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_KITEN & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim SQL As String = "SELECT TKIN_NO_V"
            SQL &= ", KIN_KNAME_N"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", SIT_NO_N"
            SQL &= ", SIT_KNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM G_TAKOUMAST"
            SQL &= ", ("
            SQL &= "SELECT KIN_NO_N"
            SQL &= ", KIN_KNAME_N"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", SIT_NO_N"
            SQL &= ", SIT_KNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= ")"
            SQL &= " WHERE TKIN_NO_V = KIN_NO_N"
            SQL &= " AND GAKKOU_CODE_V = '" & TORIS_CODE & "'"
            SQL &= " ORDER BY"
            SQL &= "  TKIN_NO_V , SIT_NO_N"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    LineData = GCom.NzDec(REC.Item("TKIN_NO_V"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("KIN_NNAME_N"))
                    LineData &= "," & GCom.NzDec(REC.Item("SIT_NO_N"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("SIT_NNAME_N"))

                    FW.WriteLine(LineData)
                Loop
            End If

            '�����ɃR�[�h�擾
            SELF_BANK_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

            SQL = "SELECT KIN_NO_N"
            SQL &= ", KIN_KNAME_N"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", SIT_NO_N"
            SQL &= ", SIT_KNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & SELF_BANK_CODE & "'"
            SQL &= " ORDER BY"
            SQL &= "  KIN_NO_N , SIT_NO_N"

            BRet = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    LineData = GCom.NzDec(REC.Item("KIN_NO_N"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("KIN_NNAME_N"))
                    LineData &= "," & GCom.NzDec(REC.Item("SIT_NO_N"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("SIT_NNAME_N"))

                    FW.WriteLine(LineData)
                Loop
            End If

            'LineData = SELF_BANK_CODE
            'LineData &= "," & SELF_BANK_NAME

            'FW.WriteLine(LineData)

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            Return True

        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(���Z�@��CSV�쐬))", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�ڏo-�w�Z�c�[���A�g�f�[�^�ڏo(���Z�@��CSV�쐬))�I��", "����", "")

        End Try
    End Function

    Private Sub txtSYear_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSYear.LostFocus
        txtSYear.Text = String.Format("{0:0000}", GCom.NzInt(txtSYear.Text))
    End Sub

    Private Sub txtSMonth_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSMonth.LostFocus
        txtSMonth.Text = String.Format("{0:00}", GCom.NzInt(txtSMonth.Text))
    End Sub

    Private Sub btnAllon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000"
            LW.FuriDate = "00000000"
            '#####################################
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
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000"
            LW.FuriDate = "00000000"
            '#####################################
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

    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
