Imports System.Text
Imports CASTCommon
Public Class FrmSubJob
    Inherits System.Windows.Forms.Form
    Private Const msgTitle As String = "�W���u�ڍ׏����(KFUJOBW011)"
    Private mJobTuuban As Integer           ' �W���u�ʔ�
    Private mJobID As String                ' �W���uID
    Private mJobPara As String              ' �p�����[�^
    Private mJobMessage As String           ' ���b�Z�[�W
    Private mStatus As String               ' �X�e�[�^�X
    Private OraMain As CASTCommon.MyOracle
    Friend WithEvents Label1 As System.Windows.Forms.Label

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
    ' �X���b�h���b�N
    Private mThreadLock As Object   ' �X���b�h�r�����b�N�I�u�W�F�N�g
    Public WriteOnly Property ThreadLock() As Object
        Set(ByVal Value As Object)
            mThreadLock = Value
        End Set
    End Property
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

    ' Oracle
    Public WriteOnly Property DB() As CASTCommon.MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            OraMain = Value
        End Set
    End Property

    ' �W���u�ʔ�
    Public WriteOnly Property JobTuuban() As Integer
        Set(ByVal Value As Integer)
            mJobTuuban = Value

            lblTuuban.Text = Value
        End Set
    End Property

    ' �W���uID
    Public WriteOnly Property JobID() As String
        Set(ByVal Value As String)
            mJobID = Value

            lblJobId.Text = Value
        End Set
    End Property

    ' �W���u�p�����[�^
    Public WriteOnly Property JobPara() As String
        Set(ByVal Value As String)
            mJobPara = Value

            txtPara.Text = Value
        End Set
    End Property

    ' ���b�Z�[�W
    Public WriteOnly Property JobMessage() As String
        Set(ByVal Value As String)
            mJobMessage = Value

            txtErrMessage.Text = Value
        End Set
    End Property

    ' �X�e�[�^�X
    Public WriteOnly Property Status() As String
        Set(ByVal Value As String)
            mStatus = Value
        End Set
    End Property

    Private mOwner As FrmJOB
    Public WriteOnly Property OwnerForm() As FrmJOB
        Set(ByVal Value As FrmJOB)
            mOwner = Value
        End Set
    End Property

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    Friend WithEvents btnDetailEnd As System.Windows.Forms.Button
    Friend WithEvents btnReAction As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents txtPara As System.Windows.Forms.TextBox
    Friend WithEvents txtErrMessage As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents lblJobId As System.Windows.Forms.Label
    Friend WithEvents lblTuuban As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSubJob))
        Me.btnDetailEnd = New System.Windows.Forms.Button()
        Me.btnReAction = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.txtPara = New System.Windows.Forms.TextBox()
        Me.txtErrMessage = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.lblJobId = New System.Windows.Forms.Label()
        Me.lblTuuban = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnDetailEnd
        '
        Me.btnDetailEnd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDetailEnd.Location = New System.Drawing.Point(372, 100)
        Me.btnDetailEnd.Name = "btnDetailEnd"
        Me.btnDetailEnd.Size = New System.Drawing.Size(88, 28)
        Me.btnDetailEnd.TabIndex = 2
        Me.btnDetailEnd.Text = "�I�@��"
        '
        'btnReAction
        '
        Me.btnReAction.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReAction.Location = New System.Drawing.Point(276, 100)
        Me.btnReAction.Name = "btnReAction"
        Me.btnReAction.Size = New System.Drawing.Size(88, 28)
        Me.btnReAction.TabIndex = 1
        Me.btnReAction.Text = "�Ď��s"
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(20, 100)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(88, 28)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "�ޮ�޷�ݾ�"
        '
        'txtPara
        '
        Me.txtPara.BackColor = System.Drawing.SystemColors.Control
        Me.txtPara.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPara.Location = New System.Drawing.Point(284, 38)
        Me.txtPara.Name = "txtPara"
        Me.txtPara.ReadOnly = True
        Me.txtPara.Size = New System.Drawing.Size(198, 19)
        Me.txtPara.TabIndex = 190
        Me.txtPara.TabStop = False
        '
        'txtErrMessage
        '
        Me.txtErrMessage.BackColor = System.Drawing.SystemColors.Control
        Me.txtErrMessage.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtErrMessage.Location = New System.Drawing.Point(20, 76)
        Me.txtErrMessage.Name = "txtErrMessage"
        Me.txtErrMessage.ReadOnly = True
        Me.txtErrMessage.Size = New System.Drawing.Size(462, 19)
        Me.txtErrMessage.TabIndex = 188
        Me.txtErrMessage.TabStop = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(12, 60)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(119, 13)
        Me.Label8.TabIndex = 184
        Me.Label8.Text = "�G���[���b�Z�[�W"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(192, 41)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(91, 13)
        Me.Label3.TabIndex = 189
        Me.Label3.Text = "�N���p�����^"
        '
        'Label75
        '
        Me.Label75.AutoSize = True
        Me.Label75.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label75.Location = New System.Drawing.Point(12, 40)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(63, 13)
        Me.Label75.TabIndex = 181
        Me.Label75.Text = "�W���uID"
        '
        'lblJobId
        '
        Me.lblJobId.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblJobId.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblJobId.Location = New System.Drawing.Point(132, 38)
        Me.lblJobId.Name = "lblJobId"
        Me.lblJobId.Size = New System.Drawing.Size(56, 18)
        Me.lblJobId.TabIndex = 183
        Me.lblJobId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTuuban
        '
        Me.lblTuuban.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTuuban.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTuuban.Location = New System.Drawing.Point(80, 38)
        Me.lblTuuban.Name = "lblTuuban"
        Me.lblTuuban.Size = New System.Drawing.Size(48, 18)
        Me.lblTuuban.TabIndex = 182
        Me.lblTuuban.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(5, 5)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(485, 30)
        Me.Label1.TabIndex = 191
        Me.Label1.Text = "�W���u�ڍ׏��"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'FrmSubJob
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 12)
        Me.ClientSize = New System.Drawing.Size(494, 136)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnDetailEnd)
        Me.Controls.Add(Me.btnReAction)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.txtPara)
        Me.Controls.Add(Me.txtErrMessage)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label75)
        Me.Controls.Add(Me.lblJobId)
        Me.Controls.Add(Me.lblTuuban)
        Me.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Location = New System.Drawing.Point(100, 300)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmSubJob"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "KFUJOBW011"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Sub btnDetailEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDetailEnd.Click
        If mOwner.ListView1.CheckedItems.Count > 0 Then
            mOwner.ListView1.CheckedItems(0).Checked = False
        End If
        If mOwner.ListView2.CheckedItems.Count > 0 Then
            mOwner.ListView2.CheckedItems(0).Checked = False
        End If

        Me.Visible = False
    End Sub

    ' �W���u�L�����Z��
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '2011/05/25 �W���u�L�����Z�������O�o�͒ǉ� ��������

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        ' 2016/02/05 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
        Dim INI_RSV2_EDITION As String = String.Empty
        ' 2016/02/05 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

        Try
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�W���u�L�����Z��)�J�n", "����")
            sw = mOwner.BatchLog.Write_Enter1("0000000000-00", "00000000", "�W���u�L�����Z��", "�ʔ�=" & mJobTuuban)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            '*** Str Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***
            SyncLock mThreadLock
                '*** End Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***

                '2011/05/25 �W���u�L�����Z�������O�o�͒ǉ� �����܂�
                ' 2016/02/05 �^�X�N�j���� CHG �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                ' ��K�͔ŋ@�\���ɁA�W���u�L�����Z����[�����[�h����ł��\�Ƃ���
                'If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                '    MessageBox.Show(MSG0001W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '    Return
                'End If
                INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
                Select Case INI_RSV2_EDITION
                    Case "2"
                        ' NOP
                    Case Else
                        If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                            MessageBox.Show(MSG0001W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                End Select
                ' 2016/02/05 �^�X�N�j���� CHG �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

                If mStatus = "2" Then
                    MessageBox.Show(MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                '--------------------------------------
                '��ݾق̊m�F
                '--------------------------------------
                If MessageBox.Show(MSG0001I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                '--------------------------------------
                'exe�̏I��
                '--------------------------------------
                '*** Str Upd 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***
                'Dim ps() As System.Diagnostics.Process = System.Diagnostics.Process.GetProcesses()
                'Dim p As System.Diagnostics.Process
                'For Each p In ps
                '    '�v���Z�X����EXE���ƈ�v������̂��I������
                '    If p.ProcessName = "KF" & lblJobId.Text Then
                '        p.Kill()
                '    End If
                'Next
                Dim mngcls As New System.Management.ManagementClass("Win32_Process")
                Dim mngcol As System.Management.ManagementObjectCollection = mngcls.GetInstances()
                Dim mngobj As System.Management.ManagementObject
                For Each mngobj In mngcol
                    If mngobj("Name").ToUpper = "KF" & lblJobId.Text.ToUpper & ".EXE" Then
                        Dim commandLine As String = mngobj("CommandLine")
                        Dim tuuban As String = commandLine.Substring(commandLine.LastIndexOf(",") + 1)

                        ' ����W���u�ʔԂ̃v���Z�X���I������
                        If CInt(tuuban) = mJobTuuban Then
                            Dim pid As Integer = CInt(mngobj("ProcessId"))
                            Dim proc As System.Diagnostics.Process = System.Diagnostics.Process.GetProcessById(pid)
                            If Not proc Is Nothing Then
                                proc.Kill

                                If mOwner.BatchLog.IS_LEVEL2() = True Then
                                    mOwner.BatchLog.Write_LEVEL2("�W���u�L�����Z��", "����",  "tuuban=" & tuuban & " , pid=" & pid)
                                End If
                            End If

                            mngobj.Dispose()
                            Exit For
                        End If
                    End If
                    mngobj.Dispose()
                Next
                mngcol.Dispose()
                mngcls.Dispose()
                '*** End Upd 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***

                '--------------------------------------
                '�W���u�}�X�^�iJOBMAST�j�̍X�V
                '--------------------------------------
                Dim SQL As New StringBuilder(128)
                SQL.Append("UPDATE JOBMAST SET STATUS_J='4'")
                SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append("   AND TUUBAN_J = " & mJobTuuban.ToString)
                If OraMain.ExecuteNonQuery(SQL) = 0 Then
                    MessageBox.Show(MSG0029E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraMain.Rollback()
                End If
                OraMain.Commit()

                Call mOwner.UpdateGamen(mJobTuuban)

                OraMain.BeginTrans()

                '*** Str Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***
            End SyncLock
            '*** End Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***

            '*** Str Del 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�W���u�L�����Z��)", "����", "�ʔ�=" & mJobTuuban) '2011/05/25 �W���u�L�����Z�������O�o�͒ǉ�
            '*** End Del 2015/12/01 SO)�r�� for ���O���� ***
            Return
            '2011/05/25 �W���u�L�����Z�������O�o�͒ǉ� ��������
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�W���u�L�����Z��)", "���s", ex.Message)
            mOwner.BatchLog.Write_Err("0000000000-00", "00000000", "�W���u�L�����Z��", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�W���u�L�����Z��)�I��", "����")
            mOwner.BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "�W���u�L�����Z��", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try
        '2011/05/25 �W���u�L�����Z�������O�o�͒ǉ� �����܂�
    End Sub

    Private Sub btnReAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReAction.Click
        '2011/05/25 �Ď��s�����O�o�͒ǉ� ��������

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        ' 2016/02/05 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
        Dim INI_RSV2_EDITION As String = String.Empty
        ' 2016/02/05 �^�X�N�j���� ADD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END

        Try
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�Ď��s)�J�n", "����")
            sw = mOwner.BatchLog.Write_Enter1("0000000000-00", "00000000", "�W���u�Ď��s", "�ʔ�=" & mJobTuuban)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

            '*** Str Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***
            SyncLock mThreadLock
                '*** End Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***

                '2011/05/25 �Ď��s�����O�o�͒ǉ� �����܂�
                ' 2016/02/05 �^�X�N�j���� CHG �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                ' �[�����[�h�ł��Ď��s�\�Ƃ���悤�ύX����
                '*** �C�� mitsu 2008/06/18 �[�����[�h�ōĎ��s�s�ɂ��� ***
                'If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                '    MessageBox.Show(MSG0002I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Question)
                '    Return
                'End If
                ''**********************************************************
                INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
                Select Case INI_RSV2_EDITION
                    Case "2"
                        ' NOP
                    Case Else
                        If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                            MessageBox.Show(MSG0147W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Question)
                            Return
                        End If
                End Select
                ' 2016/02/05 �^�X�N�j���� CHG �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START

                If mStatus = "1" Then
                    MessageBox.Show(MSG0003W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                '--------------------------------------
                '���s�̊m�F
                '--------------------------------------
                If MessageBox.Show(MSG0002I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                '--------------------------------------
                '�W���u�}�X�^�iJOBMAST�j�̍X�V
                '--------------------------------------
                Dim SQL As New StringBuilder(128)
                SQL.Append("UPDATE JOBMAST SET STATUS_J='0'")
                SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append("   AND TUUBAN_J = " & mJobTuuban.ToString)
                If OraMain.ExecuteNonQuery(SQL) = 0 Then
                    MessageBox.Show(MSG0028E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraMain.Rollback()
                End If
                OraMain.Commit()

                Call mOwner.UpdateGamen(mJobTuuban)

                OraMain.BeginTrans()

                '*** Str Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***
            End SyncLock
            '*** End Add 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***

            '*** Str Del 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�Ď��s)", "����", "�ʔ�=" & mJobTuuban) '2011/05/25 �Ď��s�����O�o�͒ǉ�
            '*** End Del 2015/12/01 SO)�r�� for ���O���� ***
            Return
            '2011/05/25 �W���u�L�����Z�������O�o�͒ǉ� ��������
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�Ď��s)", "���s", ex.Message)
            mOwner.BatchLog.Write_Err("0000000000-00", "00000000", "�W���u�Ď��s", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        Finally
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(�Ď��s)�I��", "����")
            mOwner.BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "�W���u�Ď��s", "")
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try
        '2011/05/25 �W���u�L�����Z�������O�o�͒ǉ� �����܂�
    End Sub

    Private Sub FrmSubJob_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible = False Then
            mOwner.Show()
            mOwner.Select()
        End If
    End Sub

    Private Sub FrmSubJob_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
