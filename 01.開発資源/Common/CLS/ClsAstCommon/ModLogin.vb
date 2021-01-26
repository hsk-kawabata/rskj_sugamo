Imports System
Imports System.IO
Imports System.Windows.Forms

Public Module ModLogin

    Private OraDB As MyOracle = Nothing

    Private ShowFormFlag As Boolean = False

    '�@�\   :   ���O�C���`�F�b�N
    '
    '����   :   
    '
    '�߂�l :  
    '
    '���l   :   
    '
    Public Function ChangePassword(ByVal loginuserid As String, ByRef owner As Windows.Forms.Form) As Integer
        ' �p�X���[�h�ύX��ʕ\��
        Dim oFrm As New FrmPassChange
        oFrm.UserID = loginuserid
        Call oFrm.ShowDialog(owner)
        If oFrm.DialogResult = DialogResult.OK Then
            Return 0
        Else
            Return 1
        End If
    End Function

    '�@�\   :   ���O�C���`�F�b�N
    '
    '����   :   
    '
    '�߂�l :  
    '
    '���l   :   �����n�j�̏ꍇ�͉�ʂ�\���C�����m�f�̏ꍇ�̓��O�C����ʂ��J��
    '
    Public Function CheckPermission(ByVal loginuserid As String, ByRef gamenname As String) As Boolean
        Dim Ret As DialogResult
        Dim bRet As Boolean

        If OraDB Is Nothing Then
            OraDB = New MyOracle
        Else
            OraDB.BeginTrans()
        End If

        Do
            Ret = AccessUIDMAST(OraDB, loginuserid, gamenname, Nothing)
            If Ret = DialogResult.Retry Then
                Windows.Forms.MessageBox.Show(MSG0031E, "���O�C�����s", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Loop While Ret = DialogResult.Retry
        If Ret = DialogResult.OK Then
            bRet = True
        Else
            bRet = False
            OraDB.Rollback()
        End If

        Return bRet
    End Function

    '�@�\   :   ���O�C���`�F�b�N
    '
    '����   :   
    '
    '�߂�l :  
    '
    '���l   :   �����n�j�̏ꍇ�͉�ʂ�\���C�����m�f�̏ꍇ�̓��O�C����ʂ��J��
    '
    Public Function ShowFORM(ByVal loginuserid As String, ByRef gamen As Windows.Forms.Form, ByRef owner As Windows.Forms.Form) As Boolean
        Dim Ret As DialogResult

        If ShowFormFlag = True Then
            Return True
        End If

        ShowFormFlag = True

        If OraDB Is Nothing Then
            OraDB = New MyOracle
        Else
            OraDB.BeginTrans()
        End If

        Do
            Ret = AccessUIDMAST(OraDB, loginuserid, gamen.Name, owner)
            If Ret = DialogResult.Retry Then
                Windows.Forms.MessageBox.Show(MSG0031E, "���O�C�����s", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Loop While Ret = DialogResult.Retry
        If Ret = DialogResult.OK Then
            ' 2016/03/08 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
            'gamen.ShowInTaskbar = False
            'owner.AddOwnedForm(gamen)

            'gamen.ShowInTaskbar = True
            'gamen.Show()

            'If gamen.ShowInTaskbar = True Then
            '    owner.Visible = False

            '    AddHandler gamen.Closing, AddressOf Closing
            '    'AddHandler gamen.Closed, AddressOf Closed
            'End If

            'OraDB.Close()
            'OraDB = Nothing
            Ret = DispUIDMAST(OraDB, loginuserid, gamen)
            Select Case Ret
                Case DialogResult.Abort
                    Windows.Forms.MessageBox.Show(MSG0234W, "�{�^���ݒ莸�s", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraDB.Rollback()
                Case DialogResult.None
                    Windows.Forms.MessageBox.Show(MSG0032E, "���s���s", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraDB.Rollback()
                Case Else
                    If owner.Visible = False Then
                        owner.Visible = True
                    Else
                        AddHandler gamen.Closing, AddressOf Closing
                        gamen.ShowInTaskbar = True
                        gamen.Show(owner)

                        If gamen.ShowInTaskbar = False Or _
                           gamen.Visible = False Then
                            owner.Visible = True
                        Else
                            owner.Visible = False
                        End If
                    End If

                    OraDB.Close()
                    OraDB = Nothing
            End Select
            ' 2016/03/08 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END
        Else
            OraDB.Rollback()
        End If

        ShowFormFlag = False

        Return True
    End Function

    '�@�\   :   UIDMAST���Q�Ƃ��C���O�C���������m�F����
    '
    '����   :   
    '
    '�߂�l :  
    '
    '���l   :   
    '
    Private Function AccessUIDMAST(ByRef oradb As MyOracle, ByVal loginuserid As String, ByRef gamenname As String, ByRef owner As Windows.Forms.Form) As DialogResult
        Dim OraReader As New MyOracleReader(oradb)
        Dim SQL As String

        Dim LoginKengen As String
        ' 2009/10/14 �����ύX 9 �� 1 kakiwaki
        '   Dim AccessKengen As String = New String(" "c, 128)
        Dim AccessKengen As String = "1"c

        ' UIDMAST�����擾
        SQL = "SELECT KENGEN_U FROM UIDMAST WHERE LOGINID_U = " & SQ(loginuserid)
        If OraReader.DataReader(SQL) = False Then
            OraReader.Close()
            Windows.Forms.MessageBox.Show(MSG0032E, "���s���s", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return DialogResult.Abort
        End If
        LoginKengen = OraReader.GetString("KENGEN_U")
        OraReader.Close()

        ' INI�����ʏ���`�������擾
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        ' 2009/10/14 �����ύX 9 �� 1 kakiwaki
        '   Call GetPrivateProfileString("KENGEN", gamenname, "9", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
        'Call GetPrivateProfileString("KENGEN", gamenname, "1", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
        Call GetPrivateProfileString("KENGEN", gamenname, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))

        ' 2009/10/14 �����ύX 9 �� 1 kakiwaki
        '   If LoginKengen < AccessKengen.Trim Then
        'If LoginKengen.Trim = "0" OrElse LoginKengen > AccessKengen.Trim Then
        If LoginKengen < AccessKengen.Trim Then
            ' ���O�C����ʂ��J��
            Dim oAccess As New FrmAccess
            If owner Is Nothing Then
                Call oAccess.ShowDialog()
            Else
                Call oAccess.ShowDialog(owner)
            End If
            If oAccess.DialogResult = DialogResult.OK Then
                SQL = "SELECT KENGEN_U FROM UIDMAST WHERE LOGINID_U = " & SQ(oAccess.UserID)
                If OraReader.DataReader(SQL) = False Then
                    OraReader.Close()
                    MessageBox.Show(String.Format(MSG0027E, "UIDMAST", "����"), "���s���s", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return DialogResult.Abort
                End If
                LoginKengen = OraReader.GetString("KENGEN_U")
                OraReader.Close()
                ' 2009/10/14 �����ύX 9 �� 1 kakiwaki
                '   If LoginKengen < AccessKengen.Trim Then
                'If LoginKengen <> AccessKengen.Trim Then
                'If LoginKengen.Trim = "0" OrElse LoginKengen > AccessKengen.Trim Then
                If LoginKengen < AccessKengen.Trim Then
                    Return DialogResult.Retry
                End If
            End If
            Return oAccess.DialogResult
        Else
            Return DialogResult.OK
        End If
    End Function

    ' 2016/03/08 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
    '�@�\   :   UIDMAST���Q�Ƃ��C��ʂ̃{�^���̎g�p�ۂ�ݒ肷��
    '
    '����   :   
    '
    '�߂�l :  
    '
    '���l   :   
    '
    Private Function DispUIDMAST(ByRef oradb As MyOracle, ByVal loginuserid As String, ByRef gamen As Windows.Forms.Form) As DialogResult

        Dim LoginKengen As String
        Dim AccessKengen As String = "0"c
        Dim SQL As String
        Dim OraReader As MyOracleReader = Nothing

        Try
            '=============================================================================
            ' UIDMAST�����擾
            '=============================================================================
            OraReader = New MyOracleReader(oradb)
            SQL = "SELECT KENGEN_U FROM UIDMAST WHERE LOGINID_U = " & SQ(loginuserid)
            If OraReader.DataReader(SQL) = False Then
                OraReader.Close()
                Return DialogResult.Abort
            End If
            LoginKengen = OraReader.GetString("KENGEN_U")
            OraReader.Close()

            '=============================================================================
            ' INI����{�^������`�������擾���A��ʂ̃{�^���̕\���E��\����ݒ�
            '=============================================================================
            Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
            For Each CTL As Object In gamen.Controls
                If TypeOf CTL Is Button Then
                    Call GetPrivateProfileString("KENGEN_BUTTON", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                ElseIf TypeOf CTL Is Label Then
                    Call GetPrivateProfileString("KENGEN_LABEL", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                ElseIf TypeOf CTL Is TextBox Then
                    Call GetPrivateProfileString("KENGEN_TEXTBOX", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                ElseIf TypeOf CTL Is ComboBox Then
                    Call GetPrivateProfileString("KENGEN_COMBOBOX", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                ElseIf TypeOf CTL Is GroupBox Then
                    Call GetPrivateProfileString("KENGEN_GROUPBOX", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                ElseIf TypeOf CTL Is TabControl Then
                    Call GetPrivateProfileString("KENGEN_TAB", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                ElseIf TypeOf CTL Is RadioButton Then
                    Call GetPrivateProfileString("KENGEN_RADIO", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                ElseIf TypeOf CTL Is CheckBox Then
                    Call GetPrivateProfileString("KENGEN_CHECKBOX", gamen.Name & "." & CTL.Name, "0", AccessKengen, 128, Path.Combine(sIFileName, "FSKJGINFO.INI"))
                End If

                If CInt(LoginKengen.Trim) < CInt(AccessKengen.Trim) Then
                    CTL.Visible = False
                End If
            Next

            Return DialogResult.OK

        Catch ex As Exception
            Return DialogResult.Abort
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

    End Function
    ' 2016/03/07 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START

    Private Sub Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
        Try
            Dim Frm As Windows.Forms.Form = CType(sender, Form)
            Frm.Owner.Visible = True
            Frm.Owner.Refresh()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Closed(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim Frm As Windows.Forms.Form = CType(sender, Form)
            Frm.Owner.Visible = True
        Catch ex As Exception

        End Try
    End Sub
End Module
