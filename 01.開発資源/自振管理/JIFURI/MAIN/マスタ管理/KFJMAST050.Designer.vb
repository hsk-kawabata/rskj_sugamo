<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAST050
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '振替日(日)／金庫持込
        AddHandler txtOldKinCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtOldKinCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtOldKinCode.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(年)／センター直接持込
        AddHandler txtOldSitCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtOldSitCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtOldSitCode.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(月)／センター直接持込
        AddHandler txtNewKinCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtNewKinCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtNewKinCode.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(日)／センター直接持込
        AddHandler txtNewSitCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtNewSitCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtNewSitCode.LostFocus, AddressOf CASTx01.LostFocus

    End Sub

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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAST050))
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnSelect = New System.Windows.Forms.Button
        Me.Label17 = New System.Windows.Forms.Label
        Me.btnAction = New System.Windows.Forms.Button
        Me.btnUpdate = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.btnClear = New System.Windows.Forms.Button
        Me.Label8 = New System.Windows.Forms.Label
        Me.lblOldSitNName = New System.Windows.Forms.Label
        Me.lblOldKinNName = New System.Windows.Forms.Label
        Me.txtOldSitCode = New System.Windows.Forms.TextBox
        Me.txtOldKinCode = New System.Windows.Forms.TextBox
        Me.Label15 = New System.Windows.Forms.Label
        Me.txtNewKinCode = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtNewSitCode = New System.Windows.Forms.TextBox
        Me.lblNewKinNName = New System.Windows.Forms.Label
        Me.lblNewSitNName = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(682, 9)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 32
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 27)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 31
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(599, 27)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 30
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(615, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 29
        Me.Label1.Text = "ログイン名　:"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 28
        Me.lblTitle.Text = "＜支店読替マスタメンテナンス＞"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(655, 520)
        Me.btnEnd.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 10
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnSelect
        '
        Me.btnSelect.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSelect.Location = New System.Drawing.Point(268, 520)
        Me.btnSelect.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(120, 40)
        Me.btnSelect.TabIndex = 7
        Me.btnSelect.Text = "参　照"
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(203, 132)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(144, 16)
        Me.Label17.TabIndex = 50
        Me.Label17.Text = "旧金融機関コード"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(10, 520)
        Me.btnAction.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 5
        Me.btnAction.Text = "登　録"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'btnUpdate
        '
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(138, 520)
        Me.btnUpdate.Margin = New System.Windows.Forms.Padding(4)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(120, 40)
        Me.btnUpdate.TabIndex = 6
        Me.btnUpdate.Text = "更　新"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(397, 520)
        Me.btnDelete.Margin = New System.Windows.Forms.Padding(4)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(120, 40)
        Me.btnDelete.TabIndex = 8
        Me.btnDelete.Text = "削　除"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnClear.Location = New System.Drawing.Point(526, 520)
        Me.btnClear.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(120, 40)
        Me.btnClear.TabIndex = 9
        Me.btnClear.Text = "取　消"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(203, 249)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(178, 16)
        Me.Label8.TabIndex = 50
        Me.Label8.Text = "読替後金融機関コード"
        '
        'lblOldSitNName
        '
        Me.lblOldSitNName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblOldSitNName.Location = New System.Drawing.Point(448, 175)
        Me.lblOldSitNName.Name = "lblOldSitNName"
        Me.lblOldSitNName.Size = New System.Drawing.Size(240, 19)
        Me.lblOldSitNName.TabIndex = 190
        Me.lblOldSitNName.Text = "NNNNNNNNN"
        Me.lblOldSitNName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblOldKinNName
        '
        Me.lblOldKinNName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblOldKinNName.Location = New System.Drawing.Point(448, 133)
        Me.lblOldKinNName.Name = "lblOldKinNName"
        Me.lblOldKinNName.Size = New System.Drawing.Size(240, 19)
        Me.lblOldKinNName.TabIndex = 189
        Me.lblOldKinNName.Text = "NNNNNNNNN"
        Me.lblOldKinNName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtOldSitCode
        '
        Me.txtOldSitCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtOldSitCode.Location = New System.Drawing.Point(392, 172)
        Me.txtOldSitCode.MaxLength = 3
        Me.txtOldSitCode.Name = "txtOldSitCode"
        Me.txtOldSitCode.Size = New System.Drawing.Size(40, 23)
        Me.txtOldSitCode.TabIndex = 2
        '
        'txtOldKinCode
        '
        Me.txtOldKinCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtOldKinCode.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtOldKinCode.Location = New System.Drawing.Point(392, 130)
        Me.txtOldKinCode.MaxLength = 4
        Me.txtOldKinCode.Name = "txtOldKinCode"
        Me.txtOldKinCode.Size = New System.Drawing.Size(48, 23)
        Me.txtOldKinCode.TabIndex = 1
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(203, 172)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(110, 16)
        Me.Label15.TabIndex = 188
        Me.Label15.Text = "旧支店コード"
        '
        'txtNewKinCode
        '
        Me.txtNewKinCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtNewKinCode.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtNewKinCode.Location = New System.Drawing.Point(392, 247)
        Me.txtNewKinCode.MaxLength = 4
        Me.txtNewKinCode.Name = "txtNewKinCode"
        Me.txtNewKinCode.Size = New System.Drawing.Size(48, 23)
        Me.txtNewKinCode.TabIndex = 3
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(203, 289)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(144, 16)
        Me.Label6.TabIndex = 188
        Me.Label6.Text = "読替後支店コード"
        '
        'txtNewSitCode
        '
        Me.txtNewSitCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtNewSitCode.Location = New System.Drawing.Point(392, 287)
        Me.txtNewSitCode.MaxLength = 3
        Me.txtNewSitCode.Name = "txtNewSitCode"
        Me.txtNewSitCode.Size = New System.Drawing.Size(40, 23)
        Me.txtNewSitCode.TabIndex = 4
        '
        'lblNewKinNName
        '
        Me.lblNewKinNName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblNewKinNName.Location = New System.Drawing.Point(448, 249)
        Me.lblNewKinNName.Name = "lblNewKinNName"
        Me.lblNewKinNName.Size = New System.Drawing.Size(240, 19)
        Me.lblNewKinNName.TabIndex = 189
        Me.lblNewKinNName.Text = "NNNNNNNNN"
        Me.lblNewKinNName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblNewSitNName
        '
        Me.lblNewSitNName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblNewSitNName.Location = New System.Drawing.Point(448, 291)
        Me.lblNewSitNName.Name = "lblNewSitNName"
        Me.lblNewSitNName.Size = New System.Drawing.Size(240, 19)
        Me.lblNewSitNName.TabIndex = 190
        Me.lblNewSitNName.Text = "NNNNNNNNN"
        Me.lblNewSitNName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'KFJMAST050
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.lblNewSitNName)
        Me.Controls.Add(Me.lblOldSitNName)
        Me.Controls.Add(Me.lblNewKinNName)
        Me.Controls.Add(Me.lblOldKinNName)
        Me.Controls.Add(Me.txtNewSitCode)
        Me.Controls.Add(Me.txtOldSitCode)
        Me.Controls.Add(Me.txtNewKinCode)
        Me.Controls.Add(Me.txtOldKinCode)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnSelect)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMAST050"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAST050"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnSelect As System.Windows.Forms.Button
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents lblOldSitNName As System.Windows.Forms.Label
    Friend WithEvents lblOldKinNName As System.Windows.Forms.Label
    Friend WithEvents txtOldSitCode As System.Windows.Forms.TextBox
    Friend WithEvents txtOldKinCode As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtNewKinCode As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtNewSitCode As System.Windows.Forms.TextBox
    Friend WithEvents lblNewKinNName As System.Windows.Forms.Label
    Friend WithEvents lblNewSitNName As System.Windows.Forms.Label
End Class
