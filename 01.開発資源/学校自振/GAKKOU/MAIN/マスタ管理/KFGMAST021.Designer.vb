<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAST021
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAST021))
        Me.lblSIT_NM = New System.Windows.Forms.Label
        Me.lblKIN_NM = New System.Windows.Forms.Label
        Me.txtJyusinFile = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.cmbCodeKbn = New System.Windows.Forms.ComboBox
        Me.cmbBaitai = New System.Windows.Forms.ComboBox
        Me.txtItakuCode = New System.Windows.Forms.TextBox
        Me.btnUPDATE = New System.Windows.Forms.Button
        Me.btnEraser = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.cmbKamoku = New System.Windows.Forms.ComboBox
        Me.txtKouza = New System.Windows.Forms.TextBox
        Me.txtSitCode = New System.Windows.Forms.TextBox
        Me.txtKinCode = New System.Windows.Forms.TextBox
        Me.txtSousinFile = New System.Windows.Forms.TextBox
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAdd = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lblSIT_NM
        '
        Me.lblSIT_NM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSIT_NM.Location = New System.Drawing.Point(206, 53)
        Me.lblSIT_NM.Name = "lblSIT_NM"
        Me.lblSIT_NM.Size = New System.Drawing.Size(271, 24)
        Me.lblSIT_NM.TabIndex = 102
        '
        'lblKIN_NM
        '
        Me.lblKIN_NM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKIN_NM.Location = New System.Drawing.Point(206, 21)
        Me.lblKIN_NM.Name = "lblKIN_NM"
        Me.lblKIN_NM.Size = New System.Drawing.Size(269, 24)
        Me.lblKIN_NM.TabIndex = 101
        '
        'txtJyusinFile
        '
        Me.txtJyusinFile.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinFile.ImeMode = System.Windows.Forms.ImeMode.Alpha
        Me.txtJyusinFile.Location = New System.Drawing.Point(158, 212)
        Me.txtJyusinFile.MaxLength = 26
        Me.txtJyusinFile.Name = "txtJyusinFile"
        Me.txtJyusinFile.Size = New System.Drawing.Size(312, 22)
        Me.txtJyusinFile.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(270, 151)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(93, 16)
        Me.Label2.TabIndex = 100
        Me.Label2.Text = "コード区分"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(22, 151)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(93, 16)
        Me.Label1.TabIndex = 99
        Me.Label1.Text = "媒体コード"
        '
        'cmbCodeKbn
        '
        Me.cmbCodeKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCodeKbn.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbCodeKbn.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.cmbCodeKbn.Location = New System.Drawing.Point(366, 149)
        Me.cmbCodeKbn.Name = "cmbCodeKbn"
        Me.cmbCodeKbn.Size = New System.Drawing.Size(104, 23)
        Me.cmbCodeKbn.TabIndex = 6
        '
        'cmbBaitai
        '
        Me.cmbBaitai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBaitai.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbBaitai.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.cmbBaitai.Location = New System.Drawing.Point(158, 149)
        Me.cmbBaitai.Name = "cmbBaitai"
        Me.cmbBaitai.Size = New System.Drawing.Size(96, 23)
        Me.cmbBaitai.TabIndex = 5
        '
        'txtItakuCode
        '
        Me.txtItakuCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtItakuCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtItakuCode.Location = New System.Drawing.Point(158, 117)
        Me.txtItakuCode.MaxLength = 10
        Me.txtItakuCode.Name = "txtItakuCode"
        Me.txtItakuCode.Size = New System.Drawing.Size(104, 22)
        Me.txtItakuCode.TabIndex = 4
        '
        'btnUPDATE
        '
        Me.btnUPDATE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUPDATE.Location = New System.Drawing.Point(98, 271)
        Me.btnUPDATE.Name = "btnUPDATE"
        Me.btnUPDATE.Size = New System.Drawing.Size(80, 40)
        Me.btnUPDATE.TabIndex = 10
        Me.btnUPDATE.Text = "更　新"
        '
        'btnEraser
        '
        Me.btnEraser.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEraser.Location = New System.Drawing.Point(274, 271)
        Me.btnEraser.Name = "btnEraser"
        Me.btnEraser.Size = New System.Drawing.Size(80, 40)
        Me.btnEraser.TabIndex = 12
        Me.btnEraser.Text = "取　消"
        '
        'btnDelete
        '
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(184, 271)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(80, 40)
        Me.btnDelete.TabIndex = 11
        Me.btnDelete.Text = "削　除"
        '
        'cmbKamoku
        '
        Me.cmbKamoku.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKamoku.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKamoku.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.cmbKamoku.Location = New System.Drawing.Point(158, 85)
        Me.cmbKamoku.Name = "cmbKamoku"
        Me.cmbKamoku.Size = New System.Drawing.Size(104, 23)
        Me.cmbKamoku.TabIndex = 2
        '
        'txtKouza
        '
        Me.txtKouza.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKouza.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKouza.Location = New System.Drawing.Point(382, 85)
        Me.txtKouza.MaxLength = 7
        Me.txtKouza.Name = "txtKouza"
        Me.txtKouza.Size = New System.Drawing.Size(67, 22)
        Me.txtKouza.TabIndex = 3
        '
        'txtSitCode
        '
        Me.txtSitCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSitCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSitCode.Location = New System.Drawing.Point(158, 53)
        Me.txtSitCode.MaxLength = 3
        Me.txtSitCode.Name = "txtSitCode"
        Me.txtSitCode.Size = New System.Drawing.Size(40, 22)
        Me.txtSitCode.TabIndex = 1
        '
        'txtKinCode
        '
        Me.txtKinCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtKinCode.Location = New System.Drawing.Point(158, 21)
        Me.txtKinCode.MaxLength = 4
        Me.txtKinCode.Name = "txtKinCode"
        Me.txtKinCode.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode.TabIndex = 0
        '
        'txtSousinFile
        '
        Me.txtSousinFile.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSousinFile.ImeMode = System.Windows.Forms.ImeMode.Alpha
        Me.txtSousinFile.Location = New System.Drawing.Point(158, 181)
        Me.txtSousinFile.MaxLength = 26
        Me.txtSousinFile.Name = "txtSousinFile"
        Me.txtSousinFile.Size = New System.Drawing.Size(312, 22)
        Me.txtSousinFile.TabIndex = 7
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(22, 119)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(110, 16)
        Me.Label20.TabIndex = 98
        Me.Label20.Text = "委託者コード"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(287, 87)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(76, 16)
        Me.Label19.TabIndex = 97
        Me.Label19.Text = "口座番号"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(22, 87)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(76, 16)
        Me.Label18.TabIndex = 96
        Me.Label18.Text = "科　　目"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(22, 53)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(93, 16)
        Me.Label15.TabIndex = 95
        Me.Label15.Text = "支店コード"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(22, 21)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(127, 16)
        Me.Label14.TabIndex = 94
        Me.Label14.Text = "金融機関コード"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(22, 214)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(120, 16)
        Me.Label13.TabIndex = 93
        Me.Label13.Text = "受信ファイル名"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(22, 183)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(120, 16)
        Me.Label12.TabIndex = 92
        Me.Label12.Text = "送信ファイル名"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(400, 271)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(80, 40)
        Me.btnEnd.TabIndex = 13
        Me.btnEnd.Text = "終　了"
        '
        'btnAdd
        '
        Me.btnAdd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAdd.Location = New System.Drawing.Point(12, 271)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(80, 40)
        Me.btnAdd.TabIndex = 9
        Me.btnAdd.Text = "登　録"
        '
        'KFGMAST021
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(494, 330)
        Me.Controls.Add(Me.lblSIT_NM)
        Me.Controls.Add(Me.lblKIN_NM)
        Me.Controls.Add(Me.txtJyusinFile)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmbCodeKbn)
        Me.Controls.Add(Me.cmbBaitai)
        Me.Controls.Add(Me.txtItakuCode)
        Me.Controls.Add(Me.btnUPDATE)
        Me.Controls.Add(Me.btnEraser)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.cmbKamoku)
        Me.Controls.Add(Me.txtKouza)
        Me.Controls.Add(Me.txtSitCode)
        Me.Controls.Add(Me.txtKinCode)
        Me.Controls.Add(Me.txtSousinFile)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(500, 355)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(500, 355)
        Me.Name = "KFGMAST021"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAST021"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        'Public CASTx As New CASTCommon.Events("0-9a-zA-Z", CASTCommon.Events.KeyMode.GOOD)
        'Public CASTxx As New CASTCommon.Events("0-9a-zA-Z._-", CASTCommon.Events.KeyMode.GOOD)
        'Public CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
        'Public CASTx02 As New CASTCommon.Events("0-9-", CASTCommon.Events.KeyMode.GOOD)
        'Public CASTx03 As New CASTCommon.Events("0-9()-", CASTCommon.Events.KeyMode.GOOD)


        '****************
        'txtKinCode
        '****************
        AddHandler txtKinCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKinCode.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtKinCode.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtSitCode
        '****************
        AddHandler txtSitCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSitCode.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSitCode.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'cmbKamoku
        '****************
        AddHandler cmbKamoku.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKamoku.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKamoku.LostFocus, AddressOf CAST.LostFocus

        '****************
        'txtKouza
        '****************
        AddHandler txtKouza.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKouza.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtKouza.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtItakuCode
        '****************
        AddHandler txtItakuCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtItakuCode.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtItakuCode.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'cmbBaitai
        '****************
        AddHandler cmbBaitai.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbBaitai.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbBaitai.LostFocus, AddressOf CAST.LostFocus

        '****************
        'cmbCodeKbn
        '****************
        AddHandler cmbCodeKbn.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbCodeKbn.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbCodeKbn.LostFocus, AddressOf CAST.LostFocus

        '****************
        'txtJyusinFile
        '****************
        AddHandler txtJyusinFile.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtJyusinFile.KeyPress, AddressOf CAST.KeyPress
        AddHandler txtJyusinFile.LostFocus, AddressOf CAST.LostFocus

        '****************
        'txtSousinFile
        '****************
        AddHandler txtSousinFile.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSousinFile.KeyPress, AddressOf CAST.KeyPress
        AddHandler txtSousinFile.LostFocus, AddressOf CAST.LostFocus

    End Sub

    Friend WithEvents lblSIT_NM As System.Windows.Forms.Label
    Friend WithEvents lblKIN_NM As System.Windows.Forms.Label
    Friend WithEvents txtJyusinFile As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbCodeKbn As System.Windows.Forms.ComboBox
    Friend WithEvents cmbBaitai As System.Windows.Forms.ComboBox
    Friend WithEvents txtItakuCode As System.Windows.Forms.TextBox
    Friend WithEvents btnEraser As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents cmbKamoku As System.Windows.Forms.ComboBox
    Friend WithEvents txtKouza As System.Windows.Forms.TextBox
    Friend WithEvents txtSitCode As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode As System.Windows.Forms.TextBox
    Friend WithEvents txtSousinFile As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnUPDATE As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button

End Class
