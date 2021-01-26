<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAIN050
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnCreate As System.Windows.Forms.Button
    Friend WithEvents txtKessaiDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtKessaiDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtKessaiDateY As System.Windows.Forms.TextBox
    Friend WithEvents AxPowerSORT1 As AxPowerSORT_Lib.AxPowerSORT
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAIN050))
        Me.btnCreate = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtKessaiDateD = New System.Windows.Forms.TextBox
        Me.txtKessaiDateM = New System.Windows.Forms.TextBox
        Me.txtKessaiDateY = New System.Windows.Forms.TextBox
        Me.AxPowerSORT1 = New AxPowerSORT_Lib.AxPowerSORT
        CType(Me.AxPowerSORT1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCreate
        '
        Me.btnCreate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnCreate.Location = New System.Drawing.Point(140, 521)
        Me.btnCreate.Name = "btnCreate"
        Me.btnCreate.Size = New System.Drawing.Size(120, 40)
        Me.btnCreate.TabIndex = 3
        Me.btnCreate.Text = "作　成"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 521)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 4
        Me.btnEnd.Text = "終　了"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(525, 153)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 160
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(465, 153)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 159
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(405, 153)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 158
        Me.Label8.Text = "年"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(245, 153)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(77, 16)
        Me.Label6.TabIndex = 157
        Me.Label6.Text = "決 済 日"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 154
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 153
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 152
        Me.Label3.Text = "＜資金決済データ作成＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(592, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 12)
        Me.Label2.TabIndex = 151
        Me.Label2.Text = "システム日付："
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 12)
        Me.Label1.TabIndex = 150
        Me.Label1.Text = "ログイン名："
        '
        'txtKessaiDateD
        '
        Me.txtKessaiDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateD.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKessaiDateD.Location = New System.Drawing.Point(495, 150)
        Me.txtKessaiDateD.MaxLength = 2
        Me.txtKessaiDateD.Name = "txtKessaiDateD"
        Me.txtKessaiDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtKessaiDateD.TabIndex = 2
        Me.txtKessaiDateD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtKessaiDateM
        '
        Me.txtKessaiDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateM.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKessaiDateM.Location = New System.Drawing.Point(435, 150)
        Me.txtKessaiDateM.MaxLength = 2
        Me.txtKessaiDateM.Name = "txtKessaiDateM"
        Me.txtKessaiDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtKessaiDateM.TabIndex = 1
        Me.txtKessaiDateM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtKessaiDateY
        '
        Me.txtKessaiDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateY.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKessaiDateY.Location = New System.Drawing.Point(355, 150)
        Me.txtKessaiDateY.MaxLength = 4
        Me.txtKessaiDateY.Name = "txtKessaiDateY"
        Me.txtKessaiDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtKessaiDateY.TabIndex = 0
        Me.txtKessaiDateY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'AxPowerSORT1
        '
        Me.AxPowerSORT1.Enabled = True
        Me.AxPowerSORT1.Location = New System.Drawing.Point(382, 272)
        Me.AxPowerSORT1.Name = "AxPowerSORT1"
        Me.AxPowerSORT1.OcxState = CType(resources.GetObject("AxPowerSORT1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxPowerSORT1.Size = New System.Drawing.Size(28, 28)
        Me.AxPowerSORT1.TabIndex = 161
        '
        'KFGMAIN050
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.AxPowerSORT1)
        Me.Controls.Add(Me.btnCreate)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtKessaiDateD)
        Me.Controls.Add(Me.txtKessaiDateM)
        Me.Controls.Add(Me.txtKessaiDateY)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGMAIN050"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAIN050"
        CType(Me.AxPowerSORT1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '****************
        'txtKessaiDateY
        '****************
        AddHandler txtKessaiDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKessaiDateY.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtKessaiDateY.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtKessaiDateM
        '****************
        AddHandler txtKessaiDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKessaiDateM.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtKessaiDateM.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtKessaiDateD
        '****************
        AddHandler txtKessaiDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKessaiDateD.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtKessaiDateD.LostFocus, AddressOf CASTx01.LostFocusZero

    End Sub

End Class
