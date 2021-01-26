<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGOTHR020
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
    Friend WithEvents btnEraser As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents txtKeiyaku As System.Windows.Forms.TextBox
    Friend WithEvents txtKingaku As System.Windows.Forms.TextBox
    Friend WithEvents txtKouzaBan As System.Windows.Forms.TextBox
    Friend WithEvents txtSitCode As System.Windows.Forms.TextBox
    Friend WithEvents txtKinyuuCode As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents cmbKamoku As System.Windows.Forms.ComboBox
    Friend WithEvents cmbFuriketu As System.Windows.Forms.ComboBox
    Friend WithEvents rdbKouza As System.Windows.Forms.RadioButton
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents btnSansyou As System.Windows.Forms.Button
    Friend WithEvents lbl1 As System.Windows.Forms.Label
    Friend WithEvents lbl2 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents cmbGakkouName As System.Windows.Forms.ComboBox
    Friend WithEvents txtGAKKOU_CODE As System.Windows.Forms.TextBox
    Friend WithEvents lab学校名 As System.Windows.Forms.Label
    Friend WithEvents txtFURINEN As System.Windows.Forms.TextBox
    Friend WithEvents txtFURITUKI As System.Windows.Forms.TextBox
    Friend WithEvents txtFURIHI As System.Windows.Forms.TextBox
    Friend WithEvents txtGAKUNEN As System.Windows.Forms.TextBox
    Friend WithEvents lab請求月 As System.Windows.Forms.Label
    Friend WithEvents txtSEIKYUTUKI As System.Windows.Forms.TextBox
    Friend WithEvents txtSEIKYUNEN As System.Windows.Forms.TextBox
    Friend WithEvents cmbFURIKUBUN As System.Windows.Forms.ComboBox
    Friend WithEvents txtCLASS As System.Windows.Forms.TextBox
    Friend WithEvents txtSEITONO As System.Windows.Forms.TextBox
    Friend WithEvents rdbgakkouSeq As System.Windows.Forms.RadioButton
    Friend WithEvents txtRECNO As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGOTHR020))
        Me.txtGAKUNEN = New System.Windows.Forms.TextBox
        Me.btnEraser = New System.Windows.Forms.Button
        Me.btnSansyou = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.lab請求月 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.txtRECNO = New System.Windows.Forms.TextBox
        Me.txtKeiyaku = New System.Windows.Forms.TextBox
        Me.txtKingaku = New System.Windows.Forms.TextBox
        Me.txtKouzaBan = New System.Windows.Forms.TextBox
        Me.txtSitCode = New System.Windows.Forms.TextBox
        Me.txtKinyuuCode = New System.Windows.Forms.TextBox
        Me.Label18 = New System.Windows.Forms.Label
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.cmbKamoku = New System.Windows.Forms.ComboBox
        Me.cmbFuriketu = New System.Windows.Forms.ComboBox
        Me.rdbKouza = New System.Windows.Forms.RadioButton
        Me.rdbgakkouSeq = New System.Windows.Forms.RadioButton
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtFURIHI = New System.Windows.Forms.TextBox
        Me.txtFURITUKI = New System.Windows.Forms.TextBox
        Me.txtFURINEN = New System.Windows.Forms.TextBox
        Me.txtGAKKOU_CODE = New System.Windows.Forms.TextBox
        Me.cmbGakkouName = New System.Windows.Forms.ComboBox
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.lab学校名 = New System.Windows.Forms.Label
        Me.lbl1 = New System.Windows.Forms.Label
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label21 = New System.Windows.Forms.Label
        Me.Label22 = New System.Windows.Forms.Label
        Me.txtSEIKYUTUKI = New System.Windows.Forms.TextBox
        Me.txtSEIKYUNEN = New System.Windows.Forms.TextBox
        Me.cmbFURIKUBUN = New System.Windows.Forms.ComboBox
        Me.lbl2 = New System.Windows.Forms.Label
        Me.txtCLASS = New System.Windows.Forms.TextBox
        Me.txtSEITONO = New System.Windows.Forms.TextBox
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtGAKUNEN
        '
        Me.txtGAKUNEN.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKUNEN.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKUNEN.Location = New System.Drawing.Point(290, 245)
        Me.txtGAKUNEN.MaxLength = 1
        Me.txtGAKUNEN.Name = "txtGAKUNEN"
        Me.txtGAKUNEN.Size = New System.Drawing.Size(32, 23)
        Me.txtGAKUNEN.TabIndex = 7
        Me.txtGAKUNEN.Visible = False
        '
        'btnEraser
        '
        Me.btnEraser.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEraser.Location = New System.Drawing.Point(530, 520)
        Me.btnEraser.Name = "btnEraser"
        Me.btnEraser.Size = New System.Drawing.Size(120, 40)
        Me.btnEraser.TabIndex = 19
        Me.btnEraser.Text = "取　消"
        '
        'btnSansyou
        '
        Me.btnSansyou.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSansyou.Location = New System.Drawing.Point(140, 520)
        Me.btnSansyou.Name = "btnSansyou"
        Me.btnSansyou.Size = New System.Drawing.Size(120, 40)
        Me.btnSansyou.TabIndex = 16
        Me.btnSansyou.Text = "参　照"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lab請求月)
        Me.GroupBox1.Controls.Add(Me.Label16)
        Me.GroupBox1.Controls.Add(Me.Label19)
        Me.GroupBox1.Controls.Add(Me.txtRECNO)
        Me.GroupBox1.Controls.Add(Me.txtKeiyaku)
        Me.GroupBox1.Controls.Add(Me.txtKingaku)
        Me.GroupBox1.Controls.Add(Me.txtKouzaBan)
        Me.GroupBox1.Controls.Add(Me.txtSitCode)
        Me.GroupBox1.Controls.Add(Me.txtKinyuuCode)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.Label17)
        Me.GroupBox1.Controls.Add(Me.Label15)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.cmbKamoku)
        Me.GroupBox1.Controls.Add(Me.cmbFuriketu)
        Me.GroupBox1.Location = New System.Drawing.Point(128, 279)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(520, 224)
        Me.GroupBox1.TabIndex = 10
        Me.GroupBox1.TabStop = False
        '
        'lab請求月
        '
        Me.lab請求月.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab請求月.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab請求月.Location = New System.Drawing.Point(384, 101)
        Me.lab請求月.Name = "lab請求月"
        Me.lab請求月.Size = New System.Drawing.Size(100, 23)
        Me.lab請求月.TabIndex = 24
        Me.lab請求月.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(322, 104)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(56, 16)
        Me.Label16.TabIndex = 23
        Me.Label16.Text = "請求月"
        '
        'Label19
        '
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(285, 24)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(93, 20)
        Me.Label19.TabIndex = 22
        Me.Label19.Text = "支店コード"
        '
        'txtRECNO
        '
        Me.txtRECNO.BackColor = System.Drawing.SystemColors.InactiveBorder
        Me.txtRECNO.Enabled = False
        Me.txtRECNO.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtRECNO.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtRECNO.Location = New System.Drawing.Point(144, 182)
        Me.txtRECNO.MaxLength = 5
        Me.txtRECNO.Name = "txtRECNO"
        Me.txtRECNO.Size = New System.Drawing.Size(56, 23)
        Me.txtRECNO.TabIndex = 0
        Me.txtRECNO.TabStop = False
        '
        'txtKeiyaku
        '
        Me.txtKeiyaku.BackColor = System.Drawing.SystemColors.InactiveBorder
        Me.txtKeiyaku.Enabled = False
        Me.txtKeiyaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKeiyaku.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKeiyaku.Location = New System.Drawing.Point(144, 142)
        Me.txtKeiyaku.MaxLength = 15
        Me.txtKeiyaku.Name = "txtKeiyaku"
        Me.txtKeiyaku.Size = New System.Drawing.Size(168, 23)
        Me.txtKeiyaku.TabIndex = 0
        Me.txtKeiyaku.TabStop = False
        '
        'txtKingaku
        '
        Me.txtKingaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKingaku.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKingaku.Location = New System.Drawing.Point(144, 102)
        Me.txtKingaku.MaxLength = 8
        Me.txtKingaku.Name = "txtKingaku"
        Me.txtKingaku.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtKingaku.Size = New System.Drawing.Size(168, 23)
        Me.txtKingaku.TabIndex = 15
        '
        'txtKouzaBan
        '
        Me.txtKouzaBan.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKouzaBan.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKouzaBan.Location = New System.Drawing.Point(384, 62)
        Me.txtKouzaBan.MaxLength = 7
        Me.txtKouzaBan.Name = "txtKouzaBan"
        Me.txtKouzaBan.Size = New System.Drawing.Size(72, 23)
        Me.txtKouzaBan.TabIndex = 14
        '
        'txtSitCode
        '
        Me.txtSitCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSitCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSitCode.Location = New System.Drawing.Point(384, 22)
        Me.txtSitCode.MaxLength = 3
        Me.txtSitCode.Name = "txtSitCode"
        Me.txtSitCode.Size = New System.Drawing.Size(32, 23)
        Me.txtSitCode.TabIndex = 12
        '
        'txtKinyuuCode
        '
        Me.txtKinyuuCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinyuuCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKinyuuCode.Location = New System.Drawing.Point(144, 22)
        Me.txtKinyuuCode.MaxLength = 4
        Me.txtKinyuuCode.Name = "txtKinyuuCode"
        Me.txtKinyuuCode.Size = New System.Drawing.Size(44, 23)
        Me.txtKinyuuCode.TabIndex = 11
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(227, 184)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(127, 16)
        Me.Label18.TabIndex = 21
        Me.Label18.Text = "振替結果コード"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(285, 64)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(76, 16)
        Me.Label17.TabIndex = 20
        Me.Label17.Text = "口座番号"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(24, 184)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(104, 16)
        Me.Label15.TabIndex = 18
        Me.Label15.Text = "レコード番号"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(24, 144)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(72, 16)
        Me.Label14.TabIndex = 17
        Me.Label14.Text = "契約者名"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(19, 104)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(76, 16)
        Me.Label13.TabIndex = 16
        Me.Label13.Text = "金　　額"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(19, 64)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(76, 16)
        Me.Label12.TabIndex = 15
        Me.Label12.Text = "科　　目"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(19, 24)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(127, 16)
        Me.Label11.TabIndex = 14
        Me.Label11.Text = "金融機関コード"
        '
        'cmbKamoku
        '
        Me.cmbKamoku.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKamoku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKamoku.Items.AddRange(New Object() {"普通", "当座", "納税", "職員", "その他"})
        Me.cmbKamoku.Location = New System.Drawing.Point(144, 61)
        Me.cmbKamoku.Name = "cmbKamoku"
        Me.cmbKamoku.Size = New System.Drawing.Size(112, 24)
        Me.cmbKamoku.TabIndex = 13
        '
        'cmbFuriketu
        '
        Me.cmbFuriketu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFuriketu.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbFuriketu.Location = New System.Drawing.Point(355, 179)
        Me.cmbFuriketu.Name = "cmbFuriketu"
        Me.cmbFuriketu.Size = New System.Drawing.Size(144, 24)
        Me.cmbFuriketu.TabIndex = 18
        '
        'rdbKouza
        '
        Me.rdbKouza.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbKouza.Location = New System.Drawing.Point(512, 239)
        Me.rdbKouza.Name = "rdbKouza"
        Me.rdbKouza.Size = New System.Drawing.Size(153, 32)
        Me.rdbKouza.TabIndex = 39
        Me.rdbKouza.Text = "口座番号指定"
        '
        'rdbgakkouSeq
        '
        Me.rdbgakkouSeq.Checked = True
        Me.rdbgakkouSeq.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbgakkouSeq.Location = New System.Drawing.Point(104, 239)
        Me.rdbgakkouSeq.Name = "rdbgakkouSeq"
        Me.rdbgakkouSeq.Size = New System.Drawing.Size(180, 32)
        Me.rdbgakkouSeq.TabIndex = 99
        Me.rdbgakkouSeq.TabStop = True
        Me.rdbgakkouSeq.Text = "学年-ｸﾗｽ-生徒番号"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 20
        Me.btnEnd.Text = "終　了"
        '
        'btnAction
        '
        Me.btnAction.Enabled = False
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(270, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 17
        Me.btnAction.Text = "更　新"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(370, 167)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 55
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(310, 167)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 54
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(250, 167)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 53
        Me.Label8.Text = "年"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(84, 167)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(77, 16)
        Me.Label6.TabIndex = 51
        Me.Label6.Text = "振 替 日"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(84, 128)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 16)
        Me.Label5.TabIndex = 50
        Me.Label5.Text = "学校コード"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(84, 85)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 49
        Me.Label4.Text = "学校検索"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(688, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 47
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(688, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 45
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 44
        Me.Label3.Text = "＜振替結果変更＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(600, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 42
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(616, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 40
        Me.Label1.Text = "ログイン名　:"
        '
        'txtFURIHI
        '
        Me.txtFURIHI.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFURIHI.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFURIHI.Location = New System.Drawing.Point(340, 165)
        Me.txtFURIHI.MaxLength = 2
        Me.txtFURIHI.Name = "txtFURIHI"
        Me.txtFURIHI.Size = New System.Drawing.Size(24, 23)
        Me.txtFURIHI.TabIndex = 3
        '
        'txtFURITUKI
        '
        Me.txtFURITUKI.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFURITUKI.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFURITUKI.Location = New System.Drawing.Point(280, 165)
        Me.txtFURITUKI.MaxLength = 2
        Me.txtFURITUKI.Name = "txtFURITUKI"
        Me.txtFURITUKI.Size = New System.Drawing.Size(24, 23)
        Me.txtFURITUKI.TabIndex = 2
        '
        'txtFURINEN
        '
        Me.txtFURINEN.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFURINEN.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFURINEN.Location = New System.Drawing.Point(200, 165)
        Me.txtFURINEN.MaxLength = 4
        Me.txtFURINEN.Name = "txtFURINEN"
        Me.txtFURINEN.Size = New System.Drawing.Size(44, 23)
        Me.txtFURINEN.TabIndex = 1
        '
        'txtGAKKOU_CODE
        '
        Me.txtGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKKOU_CODE.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKKOU_CODE.Location = New System.Drawing.Point(200, 125)
        Me.txtGAKKOU_CODE.MaxLength = 10
        Me.txtGAKKOU_CODE.Name = "txtGAKKOU_CODE"
        Me.txtGAKKOU_CODE.Size = New System.Drawing.Size(90, 23)
        Me.txtGAKKOU_CODE.TabIndex = 0
        '
        'cmbGakkouName
        '
        Me.cmbGakkouName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGakkouName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGakkouName.Location = New System.Drawing.Point(248, 85)
        Me.cmbGakkouName.Name = "cmbGakkouName"
        Me.cmbGakkouName.Size = New System.Drawing.Size(471, 21)
        Me.cmbGakkouName.TabIndex = 0
        Me.cmbGakkouName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKana.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(200, 85)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 0
        Me.cmbKana.TabStop = False
        '
        'lab学校名
        '
        Me.lab学校名.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校名.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校名.Location = New System.Drawing.Point(295, 124)
        Me.lab学校名.Name = "lab学校名"
        Me.lab学校名.Size = New System.Drawing.Size(423, 24)
        Me.lab学校名.TabIndex = 59
        Me.lab学校名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbl1
        '
        Me.lbl1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lbl1.Location = New System.Drawing.Point(328, 247)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Size = New System.Drawing.Size(13, 24)
        Me.lbl1.TabIndex = 60
        Me.lbl1.Text = "-"
        Me.lbl1.Visible = False
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(310, 207)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(24, 16)
        Me.Label20.TabIndex = 66
        Me.Label20.Text = "月"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label21.Location = New System.Drawing.Point(250, 207)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(24, 16)
        Me.Label21.TabIndex = 65
        Me.Label21.Text = "年"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label22.Location = New System.Drawing.Point(84, 207)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(110, 16)
        Me.Label22.TabIndex = 64
        Me.Label22.Text = "請求対象年月"
        '
        'txtSEIKYUTUKI
        '
        Me.txtSEIKYUTUKI.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSEIKYUTUKI.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSEIKYUTUKI.Location = New System.Drawing.Point(280, 205)
        Me.txtSEIKYUTUKI.MaxLength = 2
        Me.txtSEIKYUTUKI.Name = "txtSEIKYUTUKI"
        Me.txtSEIKYUTUKI.Size = New System.Drawing.Size(24, 23)
        Me.txtSEIKYUTUKI.TabIndex = 5
        '
        'txtSEIKYUNEN
        '
        Me.txtSEIKYUNEN.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSEIKYUNEN.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSEIKYUNEN.Location = New System.Drawing.Point(200, 205)
        Me.txtSEIKYUNEN.MaxLength = 4
        Me.txtSEIKYUNEN.Name = "txtSEIKYUNEN"
        Me.txtSEIKYUNEN.Size = New System.Drawing.Size(44, 23)
        Me.txtSEIKYUNEN.TabIndex = 4
        '
        'cmbFURIKUBUN
        '
        Me.cmbFURIKUBUN.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFURIKUBUN.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbFURIKUBUN.Items.AddRange(New Object() {"初振", "再振", "入金", "出金"})
        Me.cmbFURIKUBUN.Location = New System.Drawing.Point(340, 204)
        Me.cmbFURIKUBUN.Name = "cmbFURIKUBUN"
        Me.cmbFURIKUBUN.Size = New System.Drawing.Size(64, 24)
        Me.cmbFURIKUBUN.TabIndex = 6
        '
        'lbl2
        '
        Me.lbl2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lbl2.Location = New System.Drawing.Point(385, 247)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Size = New System.Drawing.Size(14, 24)
        Me.lbl2.TabIndex = 68
        Me.lbl2.Text = "-"
        Me.lbl2.Visible = False
        '
        'txtCLASS
        '
        Me.txtCLASS.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtCLASS.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtCLASS.Location = New System.Drawing.Point(347, 245)
        Me.txtCLASS.MaxLength = 2
        Me.txtCLASS.Name = "txtCLASS"
        Me.txtCLASS.Size = New System.Drawing.Size(32, 23)
        Me.txtCLASS.TabIndex = 8
        Me.txtCLASS.Visible = False
        '
        'txtSEITONO
        '
        Me.txtSEITONO.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSEITONO.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSEITONO.Location = New System.Drawing.Point(405, 245)
        Me.txtSEITONO.MaxLength = 7
        Me.txtSEITONO.Name = "txtSEITONO"
        Me.txtSEITONO.Size = New System.Drawing.Size(94, 23)
        Me.txtSEITONO.TabIndex = 9
        Me.txtSEITONO.Visible = False
        '
        'KFGOTHR020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.txtSEITONO)
        Me.Controls.Add(Me.txtCLASS)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.txtSEIKYUTUKI)
        Me.Controls.Add(Me.txtSEIKYUNEN)
        Me.Controls.Add(Me.txtGAKUNEN)
        Me.Controls.Add(Me.rdbgakkouSeq)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtFURIHI)
        Me.Controls.Add(Me.txtFURITUKI)
        Me.Controls.Add(Me.txtFURINEN)
        Me.Controls.Add(Me.txtGAKKOU_CODE)
        Me.Controls.Add(Me.lbl2)
        Me.Controls.Add(Me.cmbFURIKUBUN)
        Me.Controls.Add(Me.lbl1)
        Me.Controls.Add(Me.lab学校名)
        Me.Controls.Add(Me.btnEraser)
        Me.Controls.Add(Me.btnSansyou)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.rdbKouza)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.cmbGakkouName)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGOTHR020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGOTHR020"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '***************
        'GotFocus
        '***************
        AddHandler cmbKamoku.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbFuriketu.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbGakkouName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbFURIKUBUN.GotFocus, AddressOf CAST.GotFocus

        AddHandler txtKeiyaku.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKingaku.GotFocus, AddressOf CASTx01.GotFocusMoney
        AddHandler txtKouzaBan.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSitCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKinyuuCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtGAKKOU_CODE.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFURINEN.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFURITUKI.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFURIHI.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtGAKUNEN.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSEIKYUTUKI.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSEIKYUNEN.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtCLASS.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSEITONO.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtRECNO.GotFocus, AddressOf CASTx01.GotFocus

        '***************
        'Keypress
        '***************
        AddHandler cmbKamoku.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbFuriketu.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbGakkouName.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbFURIKUBUN.KeyPress, AddressOf CAST.KeyPress

        AddHandler txtKeiyaku.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtKingaku.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler txtKouzaBan.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSitCode.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtKinyuuCode.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtGAKKOU_CODE.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFURINEN.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFURITUKI.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFURIHI.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtGAKUNEN.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSEIKYUTUKI.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSEIKYUNEN.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtCLASS.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSEITONO.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtRECNO.KeyPress, AddressOf CASTx01.KeyPressNum

        '***************
        'Lostfocus
        '***************
        AddHandler cmbKamoku.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbFuriketu.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbGakkouName.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbFURIKUBUN.LostFocus, AddressOf CAST.LostFocus

        AddHandler txtKeiyaku.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtKingaku.LostFocus, AddressOf CASTx01.LostFocusMoney
        AddHandler txtKouzaBan.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtSitCode.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtKinyuuCode.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtGAKKOU_CODE.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtFURINEN.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtFURITUKI.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtFURIHI.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtGAKUNEN.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtSEIKYUTUKI.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtSEIKYUNEN.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtCLASS.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtSEITONO.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtRECNO.LostFocus, AddressOf CASTx01.LostFocusZero

    End Sub


End Class
