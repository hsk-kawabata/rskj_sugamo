<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJPRNT070
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJPRNT070))
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.Label22 = New System.Windows.Forms.Label
        Me.Label23 = New System.Windows.Forms.Label
        Me.Label24 = New System.Windows.Forms.Label
        Me.txtKaisiDateD = New System.Windows.Forms.TextBox
        Me.txtKaisiDateM = New System.Windows.Forms.TextBox
        Me.txtKaisiDateY = New System.Windows.Forms.TextBox
        Me.Label21 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtSyuryoDateY = New System.Windows.Forms.TextBox
        Me.txtSyuryoDateM = New System.Windows.Forms.TextBox
        Me.txtSyuryoDateD = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
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
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(594, 43)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 12)
        Me.Label4.TabIndex = 143
        Me.Label4.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(608, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 144
        Me.Label2.Text = "ログイン名　:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 141
        Me.Label1.Text = "＜未処理一覧表(落込)印刷＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 8
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(530, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 7
        Me.btnAction.Text = "印　刷"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label22.Location = New System.Drawing.Point(400, 181)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(24, 16)
        Me.Label22.TabIndex = 198
        Me.Label22.Text = "日"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label23.Location = New System.Drawing.Point(348, 181)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(24, 16)
        Me.Label23.TabIndex = 197
        Me.Label23.Text = "月"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label24.Location = New System.Drawing.Point(296, 181)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(24, 16)
        Me.Label24.TabIndex = 196
        Me.Label24.Text = "年"
        '
        'txtKaisiDateD
        '
        Me.txtKaisiDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKaisiDateD.Location = New System.Drawing.Point(374, 177)
        Me.txtKaisiDateD.MaxLength = 2
        Me.txtKaisiDateD.Name = "txtKaisiDateD"
        Me.txtKaisiDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtKaisiDateD.TabIndex = 3
        '
        'txtKaisiDateM
        '
        Me.txtKaisiDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKaisiDateM.Location = New System.Drawing.Point(322, 177)
        Me.txtKaisiDateM.MaxLength = 2
        Me.txtKaisiDateM.Name = "txtKaisiDateM"
        Me.txtKaisiDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtKaisiDateM.TabIndex = 2
        '
        'txtKaisiDateY
        '
        Me.txtKaisiDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKaisiDateY.Location = New System.Drawing.Point(250, 177)
        Me.txtKaisiDateY.MaxLength = 4
        Me.txtKaisiDateY.Name = "txtKaisiDateY"
        Me.txtKaisiDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtKaisiDateY.TabIndex = 1
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label21.Location = New System.Drawing.Point(153, 181)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(83, 16)
        Me.Label21.TabIndex = 195
        Me.Label21.Text = "振  替  日"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(427, 181)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(24, 16)
        Me.Label6.TabIndex = 198
        Me.Label6.Text = "～"
        '
        'txtSyuryoDateY
        '
        Me.txtSyuryoDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSyuryoDateY.Location = New System.Drawing.Point(457, 177)
        Me.txtSyuryoDateY.MaxLength = 4
        Me.txtSyuryoDateY.Name = "txtSyuryoDateY"
        Me.txtSyuryoDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtSyuryoDateY.TabIndex = 4
        '
        'txtSyuryoDateM
        '
        Me.txtSyuryoDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSyuryoDateM.Location = New System.Drawing.Point(529, 177)
        Me.txtSyuryoDateM.MaxLength = 2
        Me.txtSyuryoDateM.Name = "txtSyuryoDateM"
        Me.txtSyuryoDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtSyuryoDateM.TabIndex = 5
        '
        'txtSyuryoDateD
        '
        Me.txtSyuryoDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSyuryoDateD.Location = New System.Drawing.Point(581, 177)
        Me.txtSyuryoDateD.MaxLength = 2
        Me.txtSyuryoDateD.Name = "txtSyuryoDateD"
        Me.txtSyuryoDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtSyuryoDateD.TabIndex = 6
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(503, 181)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 196
        Me.Label7.Text = "年"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(555, 181)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 197
        Me.Label8.Text = "月"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(607, 181)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 198
        Me.Label9.Text = "日"
        '
        'KFJPRNT070
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.txtSyuryoDateD)
        Me.Controls.Add(Me.txtKaisiDateD)
        Me.Controls.Add(Me.txtSyuryoDateM)
        Me.Controls.Add(Me.txtKaisiDateM)
        Me.Controls.Add(Me.txtSyuryoDateY)
        Me.Controls.Add(Me.txtKaisiDateY)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJPRNT070"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJPRNT070"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '処理日(開始)(年)
        AddHandler txtKaisiDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKaisiDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKaisiDateY.LostFocus, AddressOf CAST.LostFocus

        '処理日(開始)(月)
        AddHandler txtKaisiDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKaisiDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKaisiDateM.LostFocus, AddressOf CAST.LostFocus

        '処理日(開始)(日)
        AddHandler txtKaisiDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKaisiDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKaisiDateD.LostFocus, AddressOf CAST.LostFocus

        '処理日(終了)(年)
        AddHandler txtSyuryoDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSyuryoDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSyuryoDateY.LostFocus, AddressOf CAST.LostFocus

        '処理日(終了)(月)
        AddHandler txtSyuryoDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSyuryoDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSyuryoDateM.LostFocus, AddressOf CAST.LostFocus

        '処理日(終了)(日)
        AddHandler txtSyuryoDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSyuryoDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSyuryoDateD.LostFocus, AddressOf CAST.LostFocus
    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents txtKaisiDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtKaisiDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtKaisiDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtSyuryoDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtSyuryoDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtSyuryoDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
End Class
