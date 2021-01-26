Public Class FrmAccess
    Inherits System.Windows.Forms.Form

    '�L�[�R���g���[���N���X
    Private CAST As New CASTCommon.Events

    ' ���O�C���N���X
    Private mLogin As ClsLogin = Nothing

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B
        AddHandler TxtUser.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtUser.LostFocus, AddressOf CAST.LostFocus
        AddHandler TxtUser.KeyPress, AddressOf CAST.KeyPress
        AddHandler TxtPassword.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtPassword.LostFocus, AddressOf CAST.LostFocus
        AddHandler TxtPassword.KeyPress, AddressOf CAST.KeyPress
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
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TxtPassword As System.Windows.Forms.TextBox
    Friend WithEvents TxtUser As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents BtnEnd As System.Windows.Forms.Button
    Friend WithEvents BtnLogin As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtPassword = New System.Windows.Forms.TextBox()
        Me.TxtUser = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.BtnEnd = New System.Windows.Forms.Button()
        Me.BtnLogin = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(206, 16)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "����ʕ\������������܂���B"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(8, 32)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(326, 16)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "�����̂��郆�[�U�hD/�p�X���[�h����͂��Ă��������B"
        '
        'TxtPassword
        '
        Me.TxtPassword.Font = New System.Drawing.Font("�l�r �o�S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtPassword.Location = New System.Drawing.Point(149, 124)
        Me.TxtPassword.MaxLength = 20
        Me.TxtPassword.Name = "TxtPassword"
        Me.TxtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TxtPassword.Size = New System.Drawing.Size(196, 22)
        Me.TxtPassword.TabIndex = 1
        '
        'TxtUser
        '
        Me.TxtUser.Font = New System.Drawing.Font("�l�r �o�S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtUser.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TxtUser.Location = New System.Drawing.Point(149, 88)
        Me.TxtUser.MaxLength = 20
        Me.TxtUser.Name = "TxtUser"
        Me.TxtUser.Size = New System.Drawing.Size(196, 22)
        Me.TxtUser.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("�l�r �o�S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(62, 128)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(78, 24)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "�p�X���[�h"
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("�l�r �o�S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(62, 88)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(78, 24)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "���[�U�h�c"
        '
        'BtnEnd
        '
        Me.BtnEnd.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnEnd.Font = New System.Drawing.Font("�l�r �o�S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.BtnEnd.Location = New System.Drawing.Point(209, 196)
        Me.BtnEnd.Name = "BtnEnd"
        Me.BtnEnd.Size = New System.Drawing.Size(128, 36)
        Me.BtnEnd.TabIndex = 3
        Me.BtnEnd.Text = "���"
        '
        'BtnLogin
        '
        Me.BtnLogin.Font = New System.Drawing.Font("�l�r �o�S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.BtnLogin.Location = New System.Drawing.Point(69, 196)
        Me.BtnLogin.Name = "BtnLogin"
        Me.BtnLogin.Size = New System.Drawing.Size(128, 36)
        Me.BtnLogin.TabIndex = 2
        Me.BtnLogin.Text = "���s"
        '
        'FrmAccess
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.CancelButton = Me.BtnEnd
        Me.ClientSize = New System.Drawing.Size(406, 244)
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnEnd)
        Me.Controls.Add(Me.BtnLogin)
        Me.Controls.Add(Me.TxtPassword)
        Me.Controls.Add(Me.TxtUser)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label4)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "FrmAccess"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "�����`�F�b�N"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Public ReadOnly Property UserID() As String
        Get
            Return TxtUser.Text
        End Get
    End Property

    Private Sub BtnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEnd.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub BtnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLogin.Click
        If mLogin Is Nothing Then
            mLogin = New ClsLogin
        End If

        ' ���[�U�����`�F�b�N
        Select Case mLogin.LoginCheck(TxtUser.Text, TxtPassword.Text)
            Case 0
                mLogin = Nothing

                Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Case 2
                TxtPassword.Focus()
            Case Else
                TxtUser.Focus()
        End Select
    End Sub

    Private Sub FrmAccess_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        mLogin = Nothing
    End Sub
End Class
