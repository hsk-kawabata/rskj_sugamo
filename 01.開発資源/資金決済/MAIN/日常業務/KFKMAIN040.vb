' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN040

#Region " �萔/�ϐ� "

    '--------------------------------
    ' ���ʊ֘A����
    '--------------------------------
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    '--------------------------------
    ' LOG�֘A����
    '--------------------------------
    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN040", "�������σ��G���^�������")
    Private Const msgtitle As String = "�������σ��G���^�������(KFKMAIN040)"

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
        Dim COMMON_RIENTADR As String                ' ���G���^�t�@�C���i�[��
        Dim KESSAI_RIENTANAME As String              ' ���G���^�t�@�C����
    End Structure
    Private IniInfo As INI_INFO

#End Region

#Region " ���[�h "

    Private Sub KFKMAIN040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            ' ��ʕ\�����A�������ɃV�X�e�����t��\��
            '------------------------------------------------
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            '------------------------------------------------
            ' ��ʏ����ݒ�
            '------------------------------------------------
            btnAction.Enabled = False
            cmbTimeStamp.Enabled = False
            btnReset.Enabled = False

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
            ' ���G���^�t�@�C����������
            '------------------------------------------------
            If GetFileInfo("Search", txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "���G���^�t�@�C������"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "����", "�I��", "")
        End Try

    End Sub

    '================================
    ' �����s�{�^��
    '================================
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "�J�n", "")

            If MessageBox.Show(String.Format(MSG0077I, "�������σ��G���^", "�}�̏�������"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Try
            End If

            '------------------------------------------------
            ' �^�C���X�^���v�̎擾
            '------------------------------------------------
            Dim TimeStamp As String = ""
            Dim TimeStamp_Date As String = ""
            Dim TimeStamp_Time As String = ""
            TimeStamp = cmbTimeStamp.SelectedItem
            TimeStamp_Date = TimeStamp.Substring(0, 4) & TimeStamp.Substring(5, 2) & TimeStamp.Substring(8, 2)
            TimeStamp_Time = TimeStamp.Substring(11, 2) & TimeStamp.Substring(14, 2) & TimeStamp.Substring(17, 2)

            '------------------------------------------------
            ' �t�@�C�����\�z
            '------------------------------------------------
            Dim FileName As String = ""
            FileName = "RNT_KR_" & TimeStamp_Date & "_" & TimeStamp_Time & "_" & "1"

            '------------------------------------------------
            ' �t�@�C������
            '------------------------------------------------
            Dim iRet As Integer = 0
            Do
                Try
                    Dim DirInfo As New DirectoryInfo(IniInfo.COMMON_RIENTADR)
                    Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()
                    iRet = 0
                    Exit Do
                Catch ex As Exception
                    If MessageBox.Show(String.Format(MSG0079I, "�}��", "�h���C�u(" & Path.GetPathRoot(IniInfo.COMMON_RIENTADR) & ")"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "���f", "�}�̑}���L�����Z��")
                        iRet = -1
                        Exit Do
                    End If
                End Try
            Loop

            Select Case iRet
                Case 0          ' �f�[�^�i�[����
                    If File.Exists(Path.Combine(IniInfo.COMMON_RIENTADR, IniInfo.KESSAI_RIENTANAME)) Then
                        If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "���f", "�����t�@�C���㏑�L�����Z��")
                            iRet = -1
                        End If
                    End If

                    If iRet = 0 Then
                        File.Copy(Path.Combine(IniInfo.COMMON_BAITAIWRITE, FileName), Path.Combine(IniInfo.COMMON_RIENTADR, IniInfo.KESSAI_RIENTANAME), True)
                    End If
            End Select

            '------------------------------------------------
            ' ��ʏ����ݒ�
            '------------------------------------------------
            cmbTimeStamp.Items.Clear()
            btnSearch.Enabled = True
            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False
            btnAction.Enabled = False
            txtSyoriDateY.Focus()

            '------------------------------------------------
            ' ���G���^�t�@�C����������
            '------------------------------------------------
            If GetFileInfo("Submit", txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "���G���^�t�@�C������"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            '------------------------------------------------
            ' �������b�Z�[�W�\��
            '------------------------------------------------
            If iRet = 0 Then
                MessageBox.Show(String.Format(MSG0078I, "�������σ��G���^�̔}�̏���"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���s", "����", "�o�̓t�@�C��:" & IniInfo.COMMON_BAITAIWRITE & FileName)
            Else
                MessageBox.Show(String.Format(MSG0080I, "�������σ��G���^�̔}�̏���", "���f"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            cmbTimeStamp.Enabled = False

            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True

            '�V�X�e�����t���ĕ\��
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            cmbTimeStamp.Items.Clear()
            txtSyoriDateY.Focus()

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
                MessageBox.Show(String.Format(MSG0001E, "�����f�[�^�i�[�t�H���_", "COMMON", "BAITAIWRITE"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:�����f�[�^�i�[�t�H���_ ����:COMMON ����:BAITAIWRITE")
                Return False
            End If

            IniInfo.COMMON_RIENTADR = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")
            If IniInfo.COMMON_RIENTADR.ToUpper = "ERR" OrElse IniInfo.COMMON_RIENTADR = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "���G���^�t�@�C���i�[��", "COMMON", "RIENTADR"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:���G���^�t�@�C���i�[�� ����:COMMON ����:RIENTADR")
                Return False
            End If

            IniInfo.KESSAI_RIENTANAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")
            If IniInfo.KESSAI_RIENTANAME.ToUpper = "ERR" OrElse IniInfo.KESSAI_RIENTANAME = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "���G���^�t�@�C����", "KESSAI", "RIENTANAME"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI���擾", "���s", "���ږ�:���G���^�t�@�C���� ����:KESSAI ����:RIENTANAME")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���̓`�F�b�N", "�J�n", "����:" & txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text)

            '------------------------------------------------
            '�����N�`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtSyoriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '�������`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtSyoriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '�͈̓`�F�b�N
            If Not (txtSyoriDateM.Text >= 1 And txtSyoriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '�������`�F�b�N
            '------------------------------------------------
            '�K�{�`�F�b�N
            If txtSyoriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '�����`�F�b�N
            If Not (txtSyoriDateD.Text >= 1 And txtSyoriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '���t�`�F�b�N
            '------------------------------------------------
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
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
    ' ���G���^���擾
    '================================
    Private Function GetFileInfo(ByVal Info As String, ByVal SyoriDate As String) As Boolean

        Dim TimeStamp As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���G���^���擾", "�J�n", "������:" & SyoriDate)

            '------------------------------------------------
            ' �R���{�{�b�N�X�̏�����
            '------------------------------------------------
            cmbTimeStamp.Items.Clear()

            '------------------------------------------------
            ' �ĕ`���~
            '------------------------------------------------
            cmbTimeStamp.BeginUpdate()

            '------------------------------------------------
            ' �R���{�{�b�N�X�ݒ�
            '------------------------------------------------
            Dim FileList() As String = Directory.GetFiles(IniInfo.COMMON_BAITAIWRITE)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim SetFileName() As String = Path.GetFileName(FileList(i)).Split("_"c)
                If SetFileName.Length = 5 Then
                    If SetFileName(0) = "RNT" And _
                       SetFileName(1) = "KR" And _
                       SetFileName(2) = SyoriDate And _
                       SetFileName(4) = "1" Then
                        TimeStamp = SetFileName(2) & SetFileName(3)
                        TimeStamp = TimeStamp.Substring(0, 4) & "/" & TimeStamp.Substring(4, 2) & "/" & TimeStamp.Substring(6, 2) & _
                                    Space(1) & _
                                    TimeStamp.Substring(8, 2) & ":" & TimeStamp.Substring(10, 2) & ":" & TimeStamp.Substring(12, 2)

                        cmbTimeStamp.Items.Add(TimeStamp)
                    End If
                End If
            Next

            '------------------------------------------------
            ' �ĕ`��ĊJ
            '------------------------------------------------
            cmbTimeStamp.EndUpdate()

            '------------------------------------------------
            ' ��ʐݒ�
            '------------------------------------------------
            If cmbTimeStamp.Items.Count = 0 Then
                '--------------------------------------------
                ' �������̓��b�Z�[�W�o��
                '--------------------------------------------
                If Info = "Search" Then
                    MessageBox.Show(MSG0255W.Replace("{0}", "���G���^"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                btnAction.Enabled = False
                txtSyoriDateY.Focus()
            Else
                btnAction.Enabled = True
                btnSearch.Enabled = False
                txtSyoriDateD.Enabled = False
                txtSyoriDateM.Enabled = False
                txtSyoriDateY.Enabled = False
                cmbTimeStamp.Enabled = True
                btnReset.Enabled = True
                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Focus()
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���G���^���擾", "�J�n", ex.Message)

            cmbTimeStamp.EndUpdate()
            btnAction.Enabled = False
            txtSyoriDateY.Focus()

            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���G���^���擾", "�J�n", "")
        End Try

    End Function

#End Region

#Region " �֐�(Sub) "

    '================================
    ' �[������
    '================================
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, _
            txtSyoriDateM.Validating, _
            txtSyoriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
