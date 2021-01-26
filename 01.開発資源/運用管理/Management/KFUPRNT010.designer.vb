<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFUPRNT010
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFUPRNT010))
        Me.lblUser = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.btnAction = New System.Windows.Forms.Button()
        Me.lblSyoriDate = New System.Windows.Forms.Label()
        Me.txtSyoriDateY = New System.Windows.Forms.TextBox()
        Me.txtSyoriDateM = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtSyoriDateD = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(673, 20)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 145
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(673, 43)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 142
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(594, 43)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 143
        Me.Label2.Text = "システム日付:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(608, 20)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 12)
        Me.Label3.TabIndex = 144
        Me.Label3.Text = "ログイン名　:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 141
        Me.Label1.Text = "＜ジョブ監視状況確認一覧表印刷＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 5
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(530, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 4
        Me.btnAction.Text = "印　刷"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'lblSyoriDate
        '
        Me.lblSyoriDate.AutoSize = True
        Me.lblSyoriDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSyoriDate.Location = New System.Drawing.Point(258, 170)
        Me.lblSyoriDate.Name = "lblSyoriDate"
        Me.lblSyoriDate.Size = New System.Drawing.Size(93, 16)
        Me.lblSyoriDate.TabIndex = 148
        Me.lblSyoriDate.Text = "処　理　日"
        Me.lblSyoriDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtSyoriDateY
        '
        Me.txtSyoriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.txtSyoriDateY.Location = New System.Drawing.Point(370, 166)
        Me.txtSyoriDateY.MaxLength = 4
        Me.txtSyoriDateY.Name = "txtSyoriDateY"
        Me.txtSyoriDateY.Size = New System.Drawing.Size(40, 23)
        Me.txtSyoriDateY.TabIndex = 1
        '
        'txtSyoriDateM
        '
        Me.txtSyoriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.txtSyoriDateM.Location = New System.Drawing.Point(442, 166)
        Me.txtSyoriDateM.MaxLength = 2
        Me.txtSyoriDateM.Name = "txtSyoriDateM"
        Me.txtSyoriDateM.Size = New System.Drawing.Size(23, 23)
        Me.txtSyoriDateM.TabIndex = 2
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Label4.Location = New System.Drawing.Point(416, 170)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(24, 16)
        Me.Label4.TabIndex = 151
        Me.Label4.Text = "年"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Label5.Location = New System.Drawing.Point(468, 170)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(24, 16)
        Me.Label5.TabIndex = 152
        Me.Label5.Text = "月"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Label6.Location = New System.Drawing.Point(520, 170)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(24, 16)
        Me.Label6.TabIndex = 154
        Me.Label6.Text = "日"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtSyoriDateD
        '
        Me.txtSyoriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.txtSyoriDateD.Location = New System.Drawing.Point(494, 166)
        Me.txtSyoriDateD.MaxLength = 2
        Me.txtSyoriDateD.Name = "txtSyoriDateD"
        Me.txtSyoriDateD.Size = New System.Drawing.Size(23, 23)
        Me.txtSyoriDateD.TabIndex = 3
        '
        'KFUPRNT010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtSyoriDateD)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtSyoriDateM)
        Me.Controls.Add(Me.txtSyoriDateY)
        Me.Controls.Add(Me.lblSyoriDate)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFUPRNT010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFUPRNT010"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '処理日(年)
        AddHandler txtSyoriDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSyoriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSyoriDateY.LostFocus, AddressOf CAST.LostFocus

        '処理日(月)
        AddHandler txtSyoriDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSyoriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSyoriDateM.LostFocus, AddressOf CAST.LostFocus

        '処理日(日)
        AddHandler txtSyoriDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSyoriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSyoriDateD.LostFocus, AddressOf CAST.LostFocus

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents lblSyoriDate As System.Windows.Forms.Label
    Friend WithEvents txtSyoriDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtSyoriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtSyoriDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
End Class
