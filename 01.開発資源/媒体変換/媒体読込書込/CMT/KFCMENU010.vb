Option Explicit On 
Option Strict On

Imports System.Data.OracleClient
Imports System
Imports System.IO
Imports System.text
Imports CASTCommon
Imports System.Collections
Imports System.String

Public Class KFCMENU010

#Region " �萔�E�ϐ� "

    Private Const ThisModuleName As String = "KFCMENU010.vb"
    Private noClose As Boolean

    Private MainLOG As New CASTCommon.BatchLOG("KFCMENU010", "�}�̕ϊ����j���[")
    Private Const msgTitle As String = "�}�̕ϊ����j���[(KFCMENU010)"
    Private DisplayName As String = ""
    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            '���[�UID
        Dim ToriCode As String          '�����啛�R�[�h
        Dim FuriDate As String          '�U�֓�
    End Structure
    Private LW As LogWrite

#End Region

#Region " ���[�h "

    '================================
    ' ��ʋN��������
    '================================
    Private Sub KFCMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        DisplayName = "���[�h"

        Try
            '-------------------------------------
            ' ���O���擾
            '-------------------------------------
            Dim Temp As String() = Environment.GetCommandLineArgs
            With GCom
                .GetSysDate = Date.Now
                If Temp.Length <= 1 Then
                    .GetUserID = ""
                Else
                    .GetUserID = Temp(1).Trim
                End If
            End With

            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            '-------------------------------------
            ' ��ʏ����ݒ�
            '-------------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Me.CmdBack.DialogResult = DialogResult.None
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            '-------------------------------------
            ' ���g�p�{�^����\��(Tag)
            '-------------------------------------
            For Each CTL As Control In TabPage1.Controls
                If TypeOf CTL Is Button Then
                    CTL.Visible = (GCom.NzInt(CTL.Tag) > 0)
                End If
            Next CTL

            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(GCom.GetUserID, Me, TabControl1, CAstExternal.ClsExternalMenu.ExternalMENU_BAITAI)
            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(GCom.GetUserID, Me, TabControl1, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_BAITAI)
            ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try
    End Sub

#End Region

#Region " �{�^�� "

    '================================
    ' �}�̓Ǎ��i�}�́��f�B�X�N�j
    '================================
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        DisplayName = "�}�̓Ǎ��i�}�́��f�B�X�N�j���(�ďo)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Dim DispLayClass As New KFCMAIN010
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try
    End Sub

    '================================
    ' �}�̏����i�f�B�X�N���}�́j
    '================================
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        DisplayName = "�}�̏����i�f�B�X�N���}�́j���(�ďo)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Dim DispLayClass As New KFCMAIN020
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try
    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click

    End Sub

    '================================
    ' �b�l�s�Ǎ�
    '================================
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click

        DisplayName = "�b�l�s�Ǎ����(�ďo)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Dim DispLayClass As New KFCCMT011
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try
    End Sub

    '================================
    ' �b�l�s�ʏ폑��
    '================================
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click

        DisplayName = "�b�l�s�ʏ폑�����(�ďo)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Dim DispLayClass As New KFCCMT021
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try
    End Sub

    '================================
    ' ���s�����ʃf�[�^�捞
    '================================
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click

        DisplayName = "���s�����ʃf�[�^�捞���(�ďo)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Dim DispLayClass As New KFCCMT051
            DispLayClass.SendFlag = False
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try
    End Sub

    '================================
    ' ���s�������f�[�^�����{�^������
    '================================
    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click

        DisplayName = "���s�������f�[�^�������(�ďo)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Dim DispLayClass As New KFCCMT051
            DispLayClass.SendFlag = True
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click

    End Sub

    '================================
    ' ���g�p�{�^��
    '================================
    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click

    End Sub

    '================================
    ' ���C�����j���[�{�^��
    '================================
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click

        DisplayName = "�N���[�Y"

        Try
            noClose = True
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�J�n", "")

            Dim MenuModule As String = "MENU.EXE"
            Dim BinDirectory As String = GCom.SET_PATH(GCom.GetBinFolder)

            If System.IO.File.Exists(BinDirectory & MenuModule) Then
                Call StartProc(BinDirectory, MenuModule)
            End If
            Me.Close()
            Me.Dispose()
            Application.Exit()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "���s", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "�I��", "")
        End Try

    End Sub

    '================================
    ' ��ʃN���[�Y
    '================================
    Private Sub KFJCMT0001G_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    '================================
    ' ��ʕ\���i�ĕ\���j������
    '================================
    Private Sub KFJCMT0001G_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job1 = "�}�̕ϊ������I�����"
    End Sub

#End Region

#Region " �֐� "

    Private Function StartProc(ByVal dir As String, ByVal exename As String, Optional ByVal wait As Boolean = False) As String
        Dim Arguments As String = GCom.GetUserID     '����
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
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "�A�v���P�[�V�����N��", "���s", MessageText)
            '    MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return ""
    End Function

#End Region

End Class
