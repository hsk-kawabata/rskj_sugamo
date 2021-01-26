Option Explicit On 
Option Strict On

Imports System.Reflection
Imports CASTCommon

<Assembly: AssemblyInformationalVersion("1.0.2.0")> 

Public Class KFKMENU010

    Private MyOwnerForm As Form
    Private noClose As Boolean

    Private MainLOG As New CASTCommon.BatchLOG("KFKMENU010", "�������σ��j���[���")
    Private Const msgTitle As String = "�������σ��j���[���(KFKMENU010)"

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure

    Private LW As LogWrite

    ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    Private INI_RSV2_EDITION As String
    ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

    Private Sub KFKMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    Private Sub KFKMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            Dim Temp As String() = Environment.GetCommandLineArgs

            With GCom
                .GetSysDate = Date.Now
                If Temp.Length <= 1 Then
                    .GetUserID = ""
                Else
                    .GetUserID = Temp(1).Trim
                End If
                gstrUSER_ID = .GetUserID        '���[�UID
            End With

            '#####################################
            '���O�̏����ɕK�v�ȏ��̎擾
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�J�n", "����", "")

            '------------------------------------------------
            '�V�X�e�����t�ƃ��[�U����\��
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
            If GetIniInfo() = False Then
                Exit Try
            End If

            If SetDisplayInit() = False Then
                Exit Try
            End If
            ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

            '���g�p�̃{�^�����\���ɂ���B
            For Each CTL As Control In TabPage1.Controls
                If TypeOf CTL Is Button Then
                    CTL.Visible = (GCom.NzInt(CTL.Tag) > 0)
                End If
            Next CTL

            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(gstrUSER_ID, Me, TabControl1, CAstExternal.ClsExternalMenu.ExternalMENU_KESSAI)
            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

            '*** Str Add 2015/12/09 SO)H.Yamagishi for �g����� ***
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(gstrUSER_ID, Me, TabControl1, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_KESSAI)
            '*** End Add 2015/12/09 SO)H.Yamagishi for �g����� ***

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(���[�h)�I��", "����", "")
        End Try

    End Sub

    Private Sub CmdAction01_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction01.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�쐬���(�ďo)�J�n", "����", "")

            Dim KFKMAIN010 As New KFKMAIN010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN010, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�쐬���(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�쐬���(�ďo)�I��", "����", "")
        End Try

    End Sub

    Private Sub CmdAction02_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction02.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�쐬������(�ďo)�J�n", "����", "")

            Dim KFKMAIN020 As New KFKMAIN020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN020, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�쐬������(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�쐬�����ʌďo)�I��", "����", "")
        End Try

    End Sub

    Private Sub CmdAction03_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction03.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^���ʍX�V���(�ďo)�J�n", "����", "")

            Dim KFKMAIN030 As New KFKMAIN030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN030, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^���ʍX�V���(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^���ʍX�V���(�ďo)�I��", "����", "")
        End Try

    End Sub

    ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    Private Sub CmdAction04_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction04.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�������(�ďo)�J�n", "����", "")

            Dim KFKMAIN040 As New KFKMAIN040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN040, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�������(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������σ��G���^�������(�ďo)�I��", "����", "")
        End Try

    End Sub
    ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

    Private Sub CmdAction10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction10.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���U������ƈꗗ�\������(�ďo)�J�n", "����", "")

            Dim KFKPRNT010 As New KFKPRNT010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT010, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���U������ƈꗗ�\������(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "���U������ƈꗗ�\������(�ďo)�I��", "����", "")
        End Try

    End Sub

    Private Sub CmdAction11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction11.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������ϊ�ƈꗗ�\������(�ďo)�J�n", "����", "")

            Dim KFKPRNT020 As New KFKPRNT020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT020, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������ϊ�ƈꗗ�\������(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������ϊ�ƈꗗ�\������(�ďo)�I��", "����", "")

        End Try

    End Sub

    Private Sub CmdAction12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction12.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔����������ƈꗗ�\���(�ďo)�J�n", "����", "")

            Dim KFKPRNT030 As New KFKPRNT030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT030, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔����������ƈꗗ�\���(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔����������ƈꗗ�\���(�ďo)�I��", "����", "")
        End Try
    End Sub

    Private Sub CmdAction13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction13.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔���ꊇ�������ו\���(�ďo)�J�n", "����", "")

            Dim KFKPRNT040 As New KFKPRNT040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT040, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔���ꊇ�������ו\���(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�萔���ꊇ�������ו\���(�ďo)�I��", "����", "")
        End Try

    End Sub

    Private Sub CmdAction14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction14.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������ʊm�F�\�Ĉ�����(�ďo)�J�n", "����", "")

            Dim KFKPRNT050 As New KFKPRNT050
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT050, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������ʊm�F�\�Ĉ�����(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�������ʊm�F�\�Ĉ�����(�ďo)�I��", "����", "")
        End Try
    End Sub

    '�����I����ʂ֖߂�
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�J�n", "����", "")

            Dim MenuModule As String = "MENU.EXE"
            Dim BinDirectory As String = GCom.SET_PATH(GCom.GetBinFolder)

            If System.IO.File.Exists(BinDirectory & MenuModule) Then
                Call StartProc(BinDirectory, MenuModule)
            End If
            Me.Close()
            Me.Dispose()
            Application.Exit()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(�N���[�Y)�I��", "����", "")
        End Try

    End Sub

    Private Function StartProc(ByVal dir As String, ByVal exename As String, Optional ByVal wait As Boolean = False) As String
        Dim Arguments As String = LW.UserID    '����
        Try
            Dim ExecProc As Process = Process.Start(dir & exename, Arguments)
            If ExecProc.Id <> 0 Then
                Me.Visible = False
                If wait = True Then
                    '�I���ҋ@
                    ExecProc.WaitForExit()
                    Me.Visible = True
                    Me.Activate()
                Else
                    Me.Close()
                End If
            Else
                Throw New Exception(String.Format("�A�v���P�[�V�����̋N���Ɏ��s���܂����B'{0}'", exename))
            End If

        Catch ex As Exception
            Dim MessageText As String
            MessageText = ex.Message
            MessageText &= Environment.NewLine
            MessageText &= dir & exename
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�A�v���P�[�V�����N��", "���s", ex.Message)
        End Try

        Return ""
    End Function


    Private Sub CmdAction15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction15.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�a�������U�֓���[�萔��������������(�ďo)�J�n", "����", "")

            Dim KFKPRNT060 As New KFKPRNT060
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT060, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�a�������U�֓���[�萔��������������(�ďo)", "���s", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�a�������U�֓���[�萔��������������(�ďo)�I��", "����", "")
        End Try
    End Sub

    ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
    '-----------------------------------
    ' �ݒ�t�@�C���擾
    '-----------------------------------
    Private Function GetIniInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ݒ�t�@�C���擾", "�J�n", "")

            '--------------------------------
            ' RSV2�@�\�ݒ�
            '--------------------------------
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If INI_RSV2_EDITION.ToUpper = "ERR" OrElse INI_RSV2_EDITION = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "RSV2�@�\�ݒ�", "RSV2_V1.0.0", "EDITION"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ݒ�t�@�C���擾", "���s", "���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ݒ�t�@�C���擾", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�ݒ�t�@�C���擾", "�I��", "")
        End Try
    End Function

    '-----------------------------------
    ' ��ʏ����ݒ�
    '-----------------------------------
    Private Function SetDisplayInit() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ʏ����ݒ�", "�J�n", "")

            Select Case INI_RSV2_EDITION
                Case "2"
                    Me.CmdAction04.Tag = 1
                Case Else
                    Me.CmdAction04.Tag = 0
            End Select

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ʏ����ݒ�", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "��ʏ����ݒ�", "�I��", "")
        End Try
    End Function
    ' 2016/01/19 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
End Class
