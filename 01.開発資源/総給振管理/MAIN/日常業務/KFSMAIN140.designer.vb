<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFSMAIN140
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '新規ラジオボタン
        AddHandler rbNew.GotFocus, AddressOf CAST.GotFocus
        AddHandler rbNew.LostFocus, AddressOf CAST.LostFocus
        AddHandler rbNew.KeyPress, AddressOf CAST.KeyPress

        '変更ラジオボタン
        AddHandler rbUpd.GotFocus, AddressOf CAST.GotFocus
        AddHandler rbUpd.LostFocus, AddressOf CAST.LostFocus
        AddHandler rbUpd.KeyPress, AddressOf CAST.KeyPress

        '入力日（年）
        AddHandler txtNyuryokuDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtNyuryokuDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtNyuryokuDateY.LostFocus, AddressOf CAST.LostFocus

        '入力日（月）
        AddHandler txtNyuryokuDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtNyuryokuDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtNyuryokuDateM.LostFocus, AddressOf CAST.LostFocus

        '入力日（日）
        AddHandler txtNyuryokuDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtNyuryokuDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtNyuryokuDateD.LostFocus, AddressOf CAST.LostFocus

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFSMAIN140))
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.btnAction = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.rbNew = New System.Windows.Forms.RadioButton
        Me.rbUpd = New System.Windows.Forms.RadioButton
        Me.pnlNyuryokuDate = New System.Windows.Forms.Panel
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtNyuryokuDateD = New System.Windows.Forms.TextBox
        Me.txtNyuryokuDateM = New System.Windows.Forms.TextBox
        Me.txtNyuryokuDateY = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.pnlNyuryokuDate.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "＜送付状入力処理選択＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(619, 9)
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
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 2
        Me.lblUser.Text = "admin"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(603, 28)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 12)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "システム日付:"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 28)
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
        Me.btnAction.TabIndex = 3
        Me.btnAction.Text = "実　行"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 4
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(231, 110)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "処理区分"
        '
        'rbNew
        '
        Me.rbNew.AutoSize = True
        Me.rbNew.Checked = True
        Me.rbNew.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbNew.Location = New System.Drawing.Point(366, 108)
        Me.rbNew.Name = "rbNew"
        Me.rbNew.Size = New System.Drawing.Size(66, 20)
        Me.rbNew.TabIndex = 0
        Me.rbNew.TabStop = True
        Me.rbNew.Text = "新 規"
        Me.rbNew.UseVisualStyleBackColor = True
        '
        'rbUpd
        '
        Me.rbUpd.AutoSize = True
        Me.rbUpd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbUpd.Location = New System.Drawing.Point(470, 108)
        Me.rbUpd.Name = "rbUpd"
        Me.rbUpd.Size = New System.Drawing.Size(66, 20)
        Me.rbUpd.TabIndex = 1
        Me.rbUpd.Text = "変 更"
        Me.rbUpd.UseVisualStyleBackColor = True
        '
        'pnlNyuryokuDate
        '
        Me.pnlNyuryokuDate.Controls.Add(Me.Label8)
        Me.pnlNyuryokuDate.Controls.Add(Me.Label6)
        Me.pnlNyuryokuDate.Controls.Add(Me.txtNyuryokuDateD)
        Me.pnlNyuryokuDate.Controls.Add(Me.txtNyuryokuDateM)
        Me.pnlNyuryokuDate.Controls.Add(Me.txtNyuryokuDateY)
        Me.pnlNyuryokuDate.Controls.Add(Me.Label7)
        Me.pnlNyuryokuDate.Controls.Add(Me.Label5)
        Me.pnlNyuryokuDate.Location = New System.Drawing.Point(198, 150)
        Me.pnlNyuryokuDate.Name = "pnlNyuryokuDate"
        Me.pnlNyuryokuDate.Size = New System.Drawing.Size(383, 52)
        Me.pnlNyuryokuDate.TabIndex = 2
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(324, 18)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 29
        Me.Label8.Text = "日"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(216, 18)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(24, 16)
        Me.Label6.TabIndex = 30
        Me.Label6.Text = "年"
        '
        'txtNyuryokuDateD
        '
        Me.txtNyuryokuDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtNyuryokuDateD.Location = New System.Drawing.Point(297, 15)
        Me.txtNyuryokuDateD.MaxLength = 2
        Me.txtNyuryokuDateD.Name = "txtNyuryokuDateD"
        Me.txtNyuryokuDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtNyuryokuDateD.TabIndex = 2
        '
        'txtNyuryokuDateM
        '
        Me.txtNyuryokuDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtNyuryokuDateM.Location = New System.Drawing.Point(243, 15)
        Me.txtNyuryokuDateM.MaxLength = 2
        Me.txtNyuryokuDateM.Name = "txtNyuryokuDateM"
        Me.txtNyuryokuDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtNyuryokuDateM.TabIndex = 1
        '
        'txtNyuryokuDateY
        '
        Me.txtNyuryokuDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtNyuryokuDateY.Location = New System.Drawing.Point(169, 15)
        Me.txtNyuryokuDateY.MaxLength = 4
        Me.txtNyuryokuDateY.Name = "txtNyuryokuDateY"
        Me.txtNyuryokuDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtNyuryokuDateY.TabIndex = 0
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(270, 18)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 27
        Me.Label7.Text = "月"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(34, 18)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(72, 16)
        Me.Label5.TabIndex = 26
        Me.Label5.Text = "入 力 日"
        '
        'KFSMAIN140
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.pnlNyuryokuDate)
        Me.Controls.Add(Me.rbUpd)
        Me.Controls.Add(Me.rbNew)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label3)
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
        Me.Name = "KFSMAIN140"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFSMAIN140"
        Me.pnlNyuryokuDate.ResumeLayout(False)
        Me.pnlNyuryokuDate.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents rbNew As System.Windows.Forms.RadioButton
    Friend WithEvents rbUpd As System.Windows.Forms.RadioButton
    Friend WithEvents pnlNyuryokuDate As System.Windows.Forms.Panel
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtNyuryokuDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtNyuryokuDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtNyuryokuDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label

End Class
