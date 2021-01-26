<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAIN081
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAIN081))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.btnAction = New System.Windows.Forms.Button()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtFuriDateY = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtFuriDateM = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtFuriDateD = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtSFuriDateD = New System.Windows.Forms.TextBox()
        Me.txtSFuriDateM = New System.Windows.Forms.TextBox()
        Me.txtSFuriDateY = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "＜再振データ作成＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(619, 9)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "ログイン名　:"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(683, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 2
        Me.lblUser.Text = "admin"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(603, 28)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 12)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "システム日付:"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 28)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 1
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 10
        Me.btnAction.Text = "作　成"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 11
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(246, 219)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(88, 16)
        Me.Label8.TabIndex = 4
        Me.Label8.Text = "振　替　日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(453, 219)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 7
        Me.Label9.Text = "月"
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.Location = New System.Drawing.Point(355, 215)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 4
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(401, 219)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 7
        Me.Label10.Text = "年"
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.Location = New System.Drawing.Point(427, 215)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 5
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(505, 219)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(24, 16)
        Me.Label11.TabIndex = 7
        Me.Label11.Text = "日"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.Location = New System.Drawing.Point(479, 215)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 6
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(505, 266)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(24, 16)
        Me.Label15.TabIndex = 12
        Me.Label15.Text = "日"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(401, 266)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(24, 16)
        Me.Label16.TabIndex = 14
        Me.Label16.Text = "年"
        '
        'txtSFuriDateD
        '
        Me.txtSFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSFuriDateD.Location = New System.Drawing.Point(479, 262)
        Me.txtSFuriDateD.MaxLength = 2
        Me.txtSFuriDateD.Name = "txtSFuriDateD"
        Me.txtSFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtSFuriDateD.TabIndex = 9
        '
        'txtSFuriDateM
        '
        Me.txtSFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSFuriDateM.Location = New System.Drawing.Point(427, 262)
        Me.txtSFuriDateM.MaxLength = 2
        Me.txtSFuriDateM.Name = "txtSFuriDateM"
        Me.txtSFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtSFuriDateM.TabIndex = 8
        '
        'txtSFuriDateY
        '
        Me.txtSFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSFuriDateY.Location = New System.Drawing.Point(355, 262)
        Me.txtSFuriDateY.MaxLength = 4
        Me.txtSFuriDateY.Name = "txtSFuriDateY"
        Me.txtSFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtSFuriDateY.TabIndex = 7
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(453, 266)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(24, 16)
        Me.Label17.TabIndex = 13
        Me.Label17.Text = "月"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(246, 266)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(88, 16)
        Me.Label18.TabIndex = 8
        Me.Label18.Text = "再　振　日"
        '
        'KFJMAIN081
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.txtSFuriDateD)
        Me.Controls.Add(Me.txtSFuriDateM)
        Me.Controls.Add(Me.txtSFuriDateY)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMAIN081"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAIN081"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '振替日(年)
        AddHandler txtFuriDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateY.LostFocus, AddressOf CAST.LostFocus

        '振替日(月)
        AddHandler txtFuriDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateM.LostFocus, AddressOf CAST.LostFocus

        '振替日(日)
        AddHandler txtFuriDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateD.LostFocus, AddressOf CAST.LostFocus

        '再振日(年)
        AddHandler txtSFuriDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSFuriDateY.LostFocus, AddressOf CAST.LostFocus

        '再振日(月)
        AddHandler txtSFuriDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSFuriDateM.LostFocus, AddressOf CAST.LostFocus

        '再振日(日)
        AddHandler txtSFuriDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSFuriDateD.LostFocus, AddressOf CAST.LostFocus

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtSFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtSFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtSFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label

End Class
