Option Explicit On 
Option Strict On

Public Class KFUMAST011
    Inherits System.Windows.Forms.Form

    '���ʃC�x���g�R���g���[��
    Private CAST As New CASTCommon.Events

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B
        AddHandler ListView1.KeyPress, AddressOf CAST.KeyPress
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
    Friend WithEvents CmdTrue As System.Windows.Forms.Button
    Friend WithEvents CmdFalse As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents BK_NAME As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.CmdTrue = New System.Windows.Forms.Button
        Me.CmdFalse = New System.Windows.Forms.Button
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.BK_NAME = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'CmdTrue
        '
        Me.CmdTrue.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdTrue.Location = New System.Drawing.Point(196, 172)
        Me.CmdTrue.Name = "CmdTrue"
        Me.CmdTrue.Size = New System.Drawing.Size(96, 28)
        Me.CmdTrue.TabIndex = 1
        Me.CmdTrue.Text = "�m ��"
        '
        'CmdFalse
        '
        Me.CmdFalse.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CmdFalse.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdFalse.Location = New System.Drawing.Point(300, 172)
        Me.CmdFalse.Name = "CmdFalse"
        Me.CmdFalse.Size = New System.Drawing.Size(96, 28)
        Me.CmdFalse.TabIndex = 2
        Me.CmdFalse.Text = "�L�����Z��"
        '
        'ListView1
        '
        Me.ListView1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(12, 36)
        Me.ListView1.MultiSelect = False
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(384, 128)
        Me.ListView1.TabIndex = 0
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'BK_NAME
        '
        Me.BK_NAME.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BK_NAME.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.BK_NAME.Location = New System.Drawing.Point(12, 8)
        Me.BK_NAME.Name = "BK_NAME"
        Me.BK_NAME.Size = New System.Drawing.Size(384, 24)
        Me.BK_NAME.TabIndex = 3
        Me.BK_NAME.Text = "BK_NAME"
        Me.BK_NAME.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'KFUMAST011
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.CancelButton = Me.CmdFalse
        Me.ClientSize = New System.Drawing.Size(404, 212)
        Me.ControlBox = False
        Me.Controls.Add(Me.BK_NAME)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.CmdFalse)
        Me.Controls.Add(Me.CmdTrue)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "KFUMAST011"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.ResumeLayout(False)

    End Sub

#End Region

    '�ďo��ʃI�u�W�F�N�g
    Public KFUMAST010 As KFUMAST010
    Public oraReader As CASTCommon.MyOracleReader
    Public intLoadFlg As Integer = 0
    Private Const ThisModuleName As String = "KFUMAST011"

    Private BatchLog As New CASTCommon.BatchLOG("KFUMAST011", "���Z�@�ց^�x�X�Q�Ɖ��")
    'Const msgTitle As String = "���Z�@�ց^�x�X�Q�Ɖ��"
    Const msgTitle As String = "���Z�@�ց^�x�X�Q�Ɖ��(KFUMAST011)"

    '��ʋN��������
    Private Sub KFUMAST011_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim XRet As Integer
        Dim YRet As Integer

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(���[�h)�J�n", "����", "")
            Me.CmdFalse.DialogResult = DialogResult.None

            If intLoadFlg = 0 Then
                '---------------------------------------------------------
                '���Z�@�֎Q��
                '---------------------------------------------------------
                With Me.ListView1
                    .Clear()
                    .Columns.Add("���Z�@�փR�[�h", 0, HorizontalAlignment.Left)
                    '2011/03/30 �폜���l�� ���ڒǉ� ��������
                    '.Columns.Add("�t���R�[�h", 100, HorizontalAlignment.Center)
                    '.Columns.Add("���Z�@�֖�", 270, HorizontalAlignment.Left)
                    .Columns.Add("�t���R�[�h", 90, HorizontalAlignment.Center)
                    .Columns.Add("���Z�@�֖�", 200, HorizontalAlignment.Left)
                    .Columns.Add("���Z�@�֖��J�i", 0, HorizontalAlignment.Left)
                    .Columns.Add("�폜��", 80, HorizontalAlignment.Left)
                    '2011/03/30 �폜���l�� ���ڒǉ� �����܂�
                End With

                Me.BK_NAME.Text = ""

                '�I��p�X�v���b�h�̈�̕`��
                Do Until oraReader.EOF
                    If Me.BK_NAME.Text = "" Then
                        Me.BK_NAME.Text = GCom.NzStr(oraReader.GetString("KIN_NNAME_N"))
                    End If

                    '2011/03/30 �폜���l�� ���ڒǉ�
                    'Dim Data(2) As String
                    Dim Data(4) As String

                    Data(0) = oraReader.GetString("KIN_NO_N").ToString
                    Data(1) = oraReader.GetString("KIN_FUKA_N").ToString
                    Data(2) = oraReader.GetString("KIN_NNAME_N").ToString
                    '2011/03/30 �폜���l�� �폜��������ꍇ�͉�ʕ\������ ��������
                    Data(3) = oraReader.GetString("KIN_KNAME_N").ToString

                    If oraReader.GetString("KIN_DEL_DATE_N").ToString <> "00000000" Then
                        Data(4) = oraReader.GetString("KIN_DEL_DATE_N").Insert(4, "/").Insert(7, "/")
                    Else
                        Data(4) = ""
                    End If
                    '2011/03/30 �폜���l�� �폜��������ꍇ�͉�ʕ\������ �����܂�

                    Dim vLstItem As New ListViewItem(Data)
                    Me.ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    oraReader.NextRead()

                Loop

                Me.ListView1.Items(0).Selected = True

                '�\���ʒu�̒���
                With KFUMAST010
                    XRet = .Location.X + .btnKin_Fuka_Sansyo.Location.X + .btnKin_Fuka_Sansyo.Size.Width
                    YRet = .Location.Y + .btnKin_Fuka_Sansyo.Location.Y + .btnKin_Fuka_Sansyo.Size.Height
                End With
            Else
                '---------------------------------------------------------
                '�x�X�Q��
                '---------------------------------------------------------
                With Me.ListView1
                    .Clear()
                    .Columns.Add("�x�X�R�[�h", 0, HorizontalAlignment.Left)
                    '2011/03/30 �폜���l�� ���ڒǉ� ��������
                    '.Columns.Add("�t���R�[�h", 100, HorizontalAlignment.Center)
                    '.Columns.Add("�x�X��", 270, HorizontalAlignment.Left)
                    .Columns.Add("�t���R�[�h", 90, HorizontalAlignment.Center)
                    .Columns.Add("�x�X��", 200, HorizontalAlignment.Left)
                    .Columns.Add("�폜��", 80, HorizontalAlignment.Left)
                    '2011/03/30 �폜���l�� ���ڒǉ� �����܂�
                End With
                Me.BK_NAME.Text = ""


                '�I��p�X�v���b�h�̈�̕`��
                Do Until oraReader.EOF

                    If Me.BK_NAME.Text = "" Then
                        Me.BK_NAME.Text = oraReader.GetString("SIT_NNAME_N").ToString
                    End If

                    '2011/03/30 �폜���l�� ���ڒǉ�
                    'Dim Data(2) As String
                    Dim Data(3) As String

                    Data(0) = oraReader.GetString("SIT_NO_N").ToString
                    Data(1) = oraReader.GetString("SIT_FUKA_N").ToString
                    Data(2) = oraReader.GetString("SIT_NNAME_N").ToString
                    '2011/03/30 �폜���l�� �폜��������ꍇ�͉�ʕ\������ ��������
                    If oraReader.GetString("SIT_DEL_DATE_N").ToString <> "00000000" Then
                        Data(3) = oraReader.GetString("SIT_DEL_DATE_N").Insert(4, "/").Insert(7, "/")
                    Else
                        Data(3) = ""
                    End If
                    '2011/03/30 �폜���l�� �폜��������ꍇ�͉�ʕ\������ �����܂�

                    Dim vLstItem As New ListViewItem(Data)
                    Me.ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    oraReader.NextRead()
                Loop

                Me.ListView1.Items(0).Selected = True

                '�\���ʒu�̒���
                With KFUMAST010
                    XRet = .Location.X + .btnSit_Fuka_Sansyo.Location.X + .btnSit_Fuka_Sansyo.Size.Width
                    YRet = .Location.Y + .btnSit_Fuka_Sansyo.Location.Y + .btnSit_Fuka_Sansyo.Size.Height
                End With
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(���[�h)�I��", "���s", ex.Message)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If
            Me.Location = New Point(XRet, YRet)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(���[�h)�I��", "����", "")
        End Try
    End Sub

    '�m��{�^������
    Private Sub CmdTrue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdTrue.Click
        Call SetCustomer(True)
    End Sub

    '�ꗗ�\���̈�_�u���N���b�N����
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call SetCustomer(True)
    End Sub

    '�߂�{�^������
    Private Sub CmdFalse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdFalse.Click
        Call SetCustomer()
    End Sub

    '
    ' �@�@�\ : ���Z�@�֑I���I�y���[�V�����㏈��
    '
    ' �߂�l : �Ȃ�
    '
    ' ������ : ARG1 - �m�葀�삩�L�����Z������
    '
    ' ���@�l : ���U��p(�����U�͕ʂ̃v���W�F�N�g)
    '    
    Private Sub SetCustomer(Optional ByVal avAction As Boolean = False)
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(���Z�@�֑I������)�J�n", "����", "")
            If avAction Then
                '2011/03/30 �I���s�������ꍇ���l�� ��������
                If ListView1.SelectedItems.Count = 0 Then
                    '�����𔲂���
                    MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End If
                '2011/03/30 �I���s�������ꍇ���l�� �����܂�
                With KFUMAST010
                    If intLoadFlg = 0 Then
                        .KIN_FUKA_N.Text = GCom.SelectedItem(ListView1, 1).ToString
                        '2011/03/30 �폜���l�� �I����KFUMAST010�ɔ��f���� ��������
                        .KIN_NNAME_N.Text = GCom.SelectedItem(ListView1, 2).ToString
                        .KIN_KNAME_N.Text = GCom.SelectedItem(ListView1, 3).ToString

                        Dim KIN_DEL_DATE As String = GCom.SelectedItem(ListView1, 4).ToString

                        If KIN_DEL_DATE <> "" Then
                            .KIN_DEL_DATE_N.Text = KIN_DEL_DATE.Substring(0, 4)
                            .KIN_DEL_DATE_N1.Text = KIN_DEL_DATE.Substring(5, 2)
                            .KIN_DEL_DATE_N2.Text = KIN_DEL_DATE.Substring(8, 2)
                        Else
                            .KIN_DEL_DATE_N.Text = ""
                            .KIN_DEL_DATE_N1.Text = ""
                            .KIN_DEL_DATE_N2.Text = ""
                        End If
                        '2011/03/30 �폜���l�� �I����KFUMAST010�ɔ��f���� �����܂�
                        'Call .btnSansyo.PerformClick()
                        .CancelFlg = True '2011/03/30 �I����OK�Ȃ̂Ńt�H�[�J�X������
                    Else
                        .SIT_FUKA_N.Text = GCom.SelectedItem(ListView1, 1).ToString
                        '2011/03/30 �폜���l�� �I����KFUMAST010�ɔ��f���� ��������
                        Dim SIT_DEL_DATE As String = GCom.SelectedItem(ListView1, 3).ToString

                        If SIT_DEL_DATE <> "" Then
                            .SIT_DEL_DATE_N.Text = SIT_DEL_DATE.Substring(0, 4)
                            .SIT_DEL_DATE_N1.Text = SIT_DEL_DATE.Substring(5, 2)
                            .SIT_DEL_DATE_N2.Text = SIT_DEL_DATE.Substring(8, 2)
                        Else
                            .SIT_DEL_DATE_N.Text = ""
                            .SIT_DEL_DATE_N1.Text = ""
                            .SIT_DEL_DATE_N2.Text = ""
                        End If
                        '2011/03/30 �폜���l�� �I����KFUMAST010�ɔ��f���� �����܂�
                        Call .btnSansyo.PerformClick()
                    End If
                End With
            End If

            Me.Close()
            Me.Dispose()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(���Z�@�֑I������)�I��", "���s", ex.Message)
        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(���Z�@�֑I������)�I��", "����", "")
        End Try
    End Sub

End Class
