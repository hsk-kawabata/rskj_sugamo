' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFJMAIN110

#Region " �萔/�ϐ� "

    '--------------------------------
    ' ���ʊ֘A����
    '--------------------------------
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private MainDB As CASTCommon.MyOracle

    '--------------------------------
    ' LOG�֘A����
    '--------------------------------
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN110", "���s�f�[�^�}�̏������")
    Private Const msgtitle As String = "���s�f�[�^�}�̏������(KFJMAIN110)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure
    Private LW As LogWrite

    '--------------------------------
    ' INI�֘A����
    '--------------------------------
    Friend Structure INI_INFO
        Dim COMMON_BAITAIWRITE As String             ' �}�̏����f�[�^�i�[�t�H���_
        Dim COMMON_FDDRIVE As String                 ' �e�c�R�D�T�h���C�u
    End Structure
    Private IniInfo As INI_INFO

    '--------------------------------
    ' �������
    '--------------------------------
    Friend Structure TORI_INFO
        Dim CodeKbn As String
        Dim OutFileName As String
        Dim FtranP As String
        Dim FtranP_IBM As String
    End Structure
    Private ToriInfo As TORI_INFO

#End Region

#Region " ���[�h "

    Private Sub KFJMAIN110_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "�J�n", "")

            '------------------------------------------------
            ' �V�X�e�����t�ƃ��[�U����\��
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            ' INI���擾
            '--------------------------------
            If SetIniFIle() = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' ��ʏ����ݒ�
            '------------------------------------------------
            txtTorisCode.Text = ""
            txtTorifCode.Text = ""
            txtFuriDateY.Text = ""
            txtFuriDateY.Text = ""
            txtFuriDateY.Text = ""
            btnAction.Enabled = False
            cmbKinkoCode.Enabled = False
            btnReset.Enabled = False
            txtTorisCode.Focus()

            '--------------------------------
            ' �����R���{�{�b�N�X�̐ݒ�
            '--------------------------------
            Dim Jyoken As String = " AND TAKO_KBN_T = '1'"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "����", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���[�h", "�I��", "")
        End Try

    End Sub

#End Region

#Region " �{�^�� "

    '================================
    ' �����{�^��
    '================================
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����", "�J�n", "")

            '------------------------------------------------
            ' �e�L�X�g�{�b�N�X�̓��̓`�F�b�N
            '------------------------------------------------
            If CheckDisplayInput() = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' �������`�F�b�N
            '------------------------------------------------
            ToriInfo.CodeKbn = ""
            ToriInfo.OutFileName = ""
            If CheckToriInfo(txtTorisCode.Text, txtTorifCode.Text, "") = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' ���G���^�t�@�C����������
            '------------------------------------------------
            If GetFileInfo("Search", txtTorisCode.Text & txtTorifCode.Text, txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "���s�f�[�^����"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            btnAction.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����", "�I��", "")
        End Try

    End Sub

    '================================
    ' ���s�{�^��
    '================================
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "�J�n", "")

            If MessageBox.Show(String.Format(MSG0077I, "���s�f�[�^", "�}�̏�������"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Try
            End If

            '------------------------------------------------
            ' �t�@�C�����\�z�p�ϐ��ݒ�
            '------------------------------------------------
            Dim ToriCode As String = ""
            Dim FuriDate As String = ""
            Dim KinkoCode As String = ""
            ToriCode = txtTorisCode.Text & txtTorifCode.Text
            FuriDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            KinkoCode = cmbKinkoCode.SelectedItem

            '------------------------------------------------
            ' �������`�F�b�N
            '------------------------------------------------
            ToriInfo.CodeKbn = ""
            ToriInfo.OutFileName = ""
            If CheckToriInfo(txtTorisCode.Text, txtTorifCode.Text, KinkoCode) = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' ���́E�o�̓t�@�C�����\�z
            '------------------------------------------------
            Dim InFileName As String = ""
            InFileName = "TAK_FD_" & ToriCode & "_" & FuriDate & "_" & KinkoCode

            '------------------------------------------------
            ' �t�@�C������
            '------------------------------------------------
            Dim iRet As Integer = 0
            If BaitaiWrite(InFileName, ToriInfo.OutFileName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "���s", "���}�̏���")
                iRet = -1
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "����", "���}�̏���")
            End If

            If iRet = 0 Then
                If MessageBox.Show(MSG0061I, msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                    If BaitaiWrite(InFileName, ToriInfo.OutFileName) = False Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "���s", "���}�̏���")
                        iRet = -1
                    Else
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "����", "���}�̏���")
                    End If
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "���f", "���}�̏����L�����Z��")
                End If
            End If

            '------------------------------------------------
            ' ��ʏ����ݒ�
            '------------------------------------------------
            cmbKinkoCode.Items.Clear()
            btnSearch.Enabled = True
            txtFuriDateY.Enabled = True
            txtFuriDateM.Enabled = True
            txtFuriDateD.Enabled = True
            btnReset.Enabled = False
            cmbKinkoCode.Enabled = False
            btnAction.Enabled = False
            txtFuriDateY.Focus()

            '------------------------------------------------
            ' ���G���^�t�@�C����������
            '------------------------------------------------
            If GetFileInfo("Submit", txtTorisCode.Text & txtTorifCode.Text, txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "���s�f�[�^����"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            '------------------------------------------------
            ' �������b�Z�[�W�\��
            '------------------------------------------------
            If iRet = 0 Then
                MessageBox.Show(String.Format(MSG0078I, "���s�f�[�^�̔}�̏���"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "����", "")
            Else
                MessageBox.Show(String.Format(MSG0080I, "���s�f�[�^�̔}�̏���", "���f"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "���f", "")
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "�I��", "")
        End Try

    End Sub

    '================================
    ' ����{�^��
    '================================
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�J�n", "")

            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnReset.Enabled = False
            cmbKinkoCode.Enabled = False

            txtTorisCode.Text = ""
            txtTorifCode.Text = ""
            txtFuriDateY.Enabled = True
            txtFuriDateY.Text = ""
            txtFuriDateM.Enabled = True
            txtFuriDateM.Text = ""
            txtFuriDateD.Enabled = True
            txtFuriDateD.Text = ""

            cmbKinkoCode.Items.Clear()
            txtTorisCode.Focus()

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "����", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���", "�I��", "")
        End Try

    End Sub

    '================================
    ' �I���{�^��
    '================================
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "�J�n", "")

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�N���[�Y", "�I��", "")
        End Try

    End Sub

#End Region

#Region " �֐�(Function)"

    '================================
    ' INI���擾
    '================================
    Private Function SetIniFIle() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "�J�n", "")

            IniInfo.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
            If IniInfo.COMMON_BAITAIWRITE.ToUpper = "ERR" OrElse IniInfo.COMMON_BAITAIWRITE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�Ǎ��f�[�^�i�[�t�H���_", "COMMON", "BAITAIWRITE"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�����f�[�^�i�[�t�H���_ ����:COMMON ����:BAITAIWRITE")
                Return False
            End If

            IniInfo.COMMON_FDDRIVE = CASTCommon.GetFSKJIni("COMMON", "FDDRIVE")
            If IniInfo.COMMON_FDDRIVE.ToUpper = "ERR" OrElse IniInfo.COMMON_FDDRIVE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "�e�c�h���C�u", "COMMON", "FDDRIVE"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�e�c�h���C�u ����:COMMON ����:FDDRIVE")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "�I��", "")
        End Try
    End Function

    '================================
    ' ���̓`�F�b�N
    '================================
    Private Function CheckDisplayInput() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "�J�n", "����:" & txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text)

            '------------------------------------------------
            ' ������R�[�h�`�F�b�N
            '------------------------------------------------
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            '------------------------------------------------
            ' ����敛�R�[�h�`�F�b�N
            '------------------------------------------------
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If

            '------------------------------------------------
            ' �����N�`�F�b�N
            '------------------------------------------------
            ' �K�{�`�F�b�N
            If txtFuriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            ' �������`�F�b�N
            '------------------------------------------------
            ' �K�{�`�F�b�N
            If txtFuriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            ' �͈̓`�F�b�N
            If Not (txtFuriDateM.Text >= 1 And txtFuriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            ' �������`�F�b�N
            '------------------------------------------------
            ' �K�{�`�F�b�N
            If txtFuriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            ' �����`�F�b�N
            If Not (txtFuriDateD.Text >= 1 And txtFuriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            ' ���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "�I��", "")
        End Try

    End Function

    '================================
    ' �������`�F�b�N
    '================================
    Private Function CheckToriInfo(ByVal TorisCode As String, ByVal TorifCode As String, ByVal KinkoCode As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ϑ��ҏ��擾", "�J�n", "�����:" & TorisCode & "-" & TorifCode & " / ���Z�@�փR�[�h:" & KinkoCode)

            MainDB = New CASTCommon.MyOracle
            SQL = New StringBuilder(128)

            '------------------------------------------------
            ' �����}�X�^�`�F�b�N
            '------------------------------------------------
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & TorifCode & "'")
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                If GCom.NzStr(OraReader.Reader.Item("TAKO_KBN_T")) <> "1" Then
                    MessageBox.Show(String.Format(MSG0372W, "�����R�[�h�̔}��", "���s�f�[�^�쐬�Ώ�"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "���s", "���s�쐬��Ώ�")
                    Return False
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "����", "")
                End If
            Else
                MessageBox.Show(MSG0063W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "���s", "�����}�X�^�Y���Ȃ�")
                Return False
            End If
            OraReader.Close()

            '------------------------------------------------
            ' ���s�}�X�^�`�F�b�N
            '------------------------------------------------
            If KinkoCode <> "" Then
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append("     *")
                SQL.Append(" FROM")
                SQL.Append("     TORIMAST , TAKOUMAST")
                SQL.Append(" WHERE")
                SQL.Append("     TORIS_CODE_T = '" & TorisCode & "'")
                SQL.Append(" AND TORIF_CODE_T = '" & TorifCode & "'")
                SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_V")
                SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_V")
                SQL.Append(" AND TKIN_NO_V    = '" & KinkoCode & "'")
                OraReader = New CASTCommon.MyOracleReader(MainDB)
                If OraReader.DataReader(SQL) = True Then
                    If GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_V")) <> "01" Then
                        MessageBox.Show(String.Format(MSG0372W, "�����R�[�h�̔}��", "���s�f�[�^�����̑Ώ�"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "���s", "�}�̋敪�ΏۊO")
                        Return False
                    Else
                        ToriInfo.CodeKbn = GCom.NzStr(OraReader.Reader.Item("CODE_KBN_V"))
                        ToriInfo.OutFileName = GCom.NzStr(OraReader.Reader.Item("SFILE_NAME_V"))
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "����", "")
                    End If
                Else
                    MessageBox.Show(MSG0070W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "���s", "���s�}�X�^�Y���Ȃ�")
                    Return False
                End If
                OraReader.Close()
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������`�F�b�N", "�I��", "")
        End Try
    End Function

    '================================
    ' ���s�f�[�^���擾
    '================================
    Private Function GetFileInfo(ByVal Info As String, ByVal ToriCode As String, ByVal FuriDate As String) As Boolean

        Dim KinkoCode As String = ""

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s�f�[�^���擾", "�J�n", "������:" & FuriDate)

            '------------------------------------------------
            ' �R���{�{�b�N�X�̏�����
            '------------------------------------------------
            cmbKinkoCode.Items.Clear()

            '------------------------------------------------
            ' �ĕ`���~
            '------------------------------------------------
            cmbKinkoCode.BeginUpdate()

            '------------------------------------------------
            ' �R���{�{�b�N�X�ݒ�
            '------------------------------------------------
            Dim FileList() As String = Directory.GetFiles(IniInfo.COMMON_BAITAIWRITE)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim SetFileName() As String = Path.GetFileName(FileList(i)).Split("_"c)
                If SetFileName.Length = 5 Then
                    If SetFileName(0) = "TAK" And _
                        SetFileName(1) = "FD" And _
                        SetFileName(2) = ToriCode And _
                        SetFileName(3) = FuriDate Then
                        KinkoCode = SetFileName(4)
                        cmbKinkoCode.Items.Add(KinkoCode)
                    End If
                End If
            Next

            '------------------------------------------------
            ' �ĕ`��ĊJ
            '------------------------------------------------
            cmbKinkoCode.EndUpdate()

            '------------------------------------------------
            ' ��ʐݒ�
            '------------------------------------------------
            If cmbKinkoCode.Items.Count = 0 Then
                '--------------------------------------------
                ' �������̓��b�Z�[�W�o��
                '--------------------------------------------
                If Info = "Search" Then
                    MessageBox.Show(MSG0255W.Replace("{0}�t�@�C��", "���s�f�[�^"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                btnAction.Enabled = False
                txtTorisCode.Focus()
            Else
                btnAction.Enabled = True
                btnSearch.Enabled = False
                txtFuriDateD.Enabled = False
                txtFuriDateM.Enabled = False
                txtFuriDateY.Enabled = False
                cmbKinkoCode.Enabled = True
                btnReset.Enabled = True
                cmbKinkoCode.SelectedIndex = 0
                cmbKinkoCode.Focus()
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s�f�[�^���擾", "�J�n", ex.Message)

            cmbKinkoCode.EndUpdate()
            btnAction.Enabled = False
            txtFuriDateY.Focus()

            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s�f�[�^���擾", "�J�n", "")
        End Try

    End Function

    '================================
    ' �}�̏���
    '================================
    Private Function BaitaiWrite(ByVal InFileName As String, ByVal OutFileName As String) As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "�J�n", "")

            '------------------------------------
            '�}�̂ɏ�������
            '------------------------------------
            Dim FtranP As String = ""
            Select Case ToriInfo.CodeKbn
                Case "4"
                    FtranP = "120IBM.P"
                Case "0"
                    FtranP = "120JIS��JIS.P"
                Case Else
                    FtranP = "120JIS��JIS.P"
            End Select

            Dim intKEKKA As Integer = clsFUSION.fn_DISK_CPYTO_FD(txtTorisCode.Text & txtTorifCode.Text, _
                                                                 Path.Combine(IniInfo.COMMON_BAITAIWRITE, InFileName), _
                                                                 OutFileName, _
                                                                 120, _
                                                                 ToriInfo.CodeKbn, FtranP, False, msgtitle)

            Select Case intKEKKA
                Case 100
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�����ݎ��s�iIBM�`���j")
                    Return False
                Case 200
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���s ", "�e�c�����ݎ��s�iDOS�`���j")
                    Return False
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "���� ", "�e�c������")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�}�̏���", "�I��", "")
        End Try

    End Function

#End Region

#Region " �֐�(Sub) "

    '================================
    ' �����(�R���{�{�b�N�X)���擾
    '================================
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                Dim Jyoken As String = " AND TAKO_KBN_T = '1'"
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If

            cmbToriName.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����R���{�{�b�N�X�ݒ�", "", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' �����(�R���{�{�b�N�X)�I������
    '================================
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged

        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�����R�[�h�ݒ�", "", ex.Message)
        Finally
            ' NOP
        End Try

    End Sub

    '================================
    ' �[������
    '================================
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
                Handles txtTorisCode.Validating, txtTorifCode.Validating, _
                        txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
