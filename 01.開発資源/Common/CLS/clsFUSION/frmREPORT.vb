Public Class frmREPORT
    Inherits System.Windows.Forms.Form

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �� dispose ���I�[�o�[���C�h���ăR���|�[�l���g�ꗗ���������܂��B
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
    ' Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^�͎g�p���Ȃ��ł��������B
    Friend WithEvents objAxPowerSORT As AxPowerSORT_Lib.AxPowerSORT
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmREPORT))
        Me.objAxPowerSORT = New AxPowerSORT_Lib.AxPowerSORT
        CType(Me.objAxPowerSORT, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'objAxPowerSORT
        '
        Me.objAxPowerSORT.Enabled = True
        Me.objAxPowerSORT.Location = New System.Drawing.Point(112, 128)
        Me.objAxPowerSORT.Name = "objAxPowerSORT"
        Me.objAxPowerSORT.OcxState = CType(resources.GetObject("objAxPowerSORT.OcxState"), System.Windows.Forms.AxHost.State)
        Me.objAxPowerSORT.Size = New System.Drawing.Size(28, 28)
        Me.objAxPowerSORT.TabIndex = 0
        '
        'frmREPORT
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(292, 273)
        Me.Controls.Add(Me.objAxPowerSORT)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmREPORT"
        Me.Text = "frmREPORT"
        CType(Me.objAxPowerSORT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
