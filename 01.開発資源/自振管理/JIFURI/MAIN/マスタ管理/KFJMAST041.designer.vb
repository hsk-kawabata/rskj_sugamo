<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAST041
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '金融機関コード
        AddHandler txtKinCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKinCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKinCode.LostFocus, AddressOf CAST.LostFocus

        '支店コード
        AddHandler txtSitCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSitCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSitCode.LostFocus, AddressOf CAST.LostFocus

        '科目コード
        AddHandler cmbKamoku.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKamoku.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKamoku.LostFocus, AddressOf CAST.LostFocus

        '口座番号
        AddHandler txtKouza.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKouza.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKouza.LostFocus, AddressOf CAST.LostFocus

        '委託者コード
        AddHandler txtItakuCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtItakuCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtItakuCode.LostFocus, AddressOf CAST.LostFocus

        '媒体コード
        AddHandler cmbBaitai.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbBaitai.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbBaitai.LostFocus, AddressOf CAST.LostFocus

        'コード区分
        AddHandler cmbCodeKbn.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbCodeKbn.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbCodeKbn.LostFocus, AddressOf CAST.LostFocus

        '請求ファイル名
        AddHandler txtSousinFile.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSousinFile.KeyPress, AddressOf CASTx1.KeyPress
        AddHandler txtSousinFile.LostFocus, AddressOf CAST.LostFocus

        '結果ファイル名
        AddHandler txtJyusinFile.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtJyusinFile.KeyPress, AddressOf CASTx1.KeyPress
        AddHandler txtJyusinFile.LostFocus, AddressOf CAST.LostFocus

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAST041))
        Me.lblSIT_NM = New System.Windows.Forms.Label
        Me.lblKIN_NM = New System.Windows.Forms.Label
        Me.txtJyusinFile = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.cmbBaitai = New System.Windows.Forms.ComboBox
        Me.txtItakuCode = New System.Windows.Forms.TextBox
        Me.btnKousin = New System.Windows.Forms.Button
        Me.btnEraser = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.cmbKamoku = New System.Windows.Forms.ComboBox
        Me.txtKouza = New System.Windows.Forms.TextBox
        Me.txtSitCode = New System.Windows.Forms.TextBox
        Me.txtKinCode = New System.Windows.Forms.TextBox
        Me.txtSousinFile = New System.Windows.Forms.TextBox
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.Label11 = New System.Windows.Forms.Label
        Me.cmbCodeKbn = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblSIT_NM
        '
        Me.lblSIT_NM.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSIT_NM.Location = New System.Drawing.Point(206, 79)
        Me.lblSIT_NM.Name = "lblSIT_NM"
        Me.lblSIT_NM.Size = New System.Drawing.Size(216, 24)
        Me.lblSIT_NM.TabIndex = 102
        Me.lblSIT_NM.Tag = "1"
        Me.lblSIT_NM.Text = "NNNNNNNNN"
        Me.lblSIT_NM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblKIN_NM
        '
        Me.lblKIN_NM.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKIN_NM.Location = New System.Drawing.Point(206, 47)
        Me.lblKIN_NM.Name = "lblKIN_NM"
        Me.lblKIN_NM.Size = New System.Drawing.Size(216, 24)
        Me.lblKIN_NM.TabIndex = 101
        Me.lblKIN_NM.Tag = "1"
        Me.lblKIN_NM.Text = "NNNNNNNNNNNN"
        Me.lblKIN_NM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtJyusinFile
        '
        Me.txtJyusinFile.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinFile.Location = New System.Drawing.Point(158, 238)
        Me.txtJyusinFile.MaxLength = 26
        Me.txtJyusinFile.Name = "txtJyusinFile"
        Me.txtJyusinFile.Size = New System.Drawing.Size(312, 22)
        Me.txtJyusinFile.TabIndex = 86
        Me.txtJyusinFile.Tag = "1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(22, 178)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(93, 16)
        Me.Label1.TabIndex = 99
        Me.Label1.Text = "媒体コード"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(22, 146)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(110, 16)
        Me.Label20.TabIndex = 98
        Me.Label20.Text = "委託者コード"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(277, 113)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(76, 16)
        Me.Label19.TabIndex = 97
        Me.Label19.Text = "口座番号"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(22, 113)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(76, 16)
        Me.Label18.TabIndex = 96
        Me.Label18.Text = "科　　目"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(22, 82)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(93, 16)
        Me.Label15.TabIndex = 95
        Me.Label15.Text = "支店コード"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(22, 52)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(127, 16)
        Me.Label14.TabIndex = 94
        Me.Label14.Text = "金融機関コード"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(22, 238)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(120, 16)
        Me.Label13.TabIndex = 93
        Me.Label13.Text = "受信ファイル名"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(22, 207)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(120, 16)
        Me.Label12.TabIndex = 92
        Me.Label12.Text = "送信ファイル名"
        '
        'cmbBaitai
        '
        Me.cmbBaitai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBaitai.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbBaitai.Location = New System.Drawing.Point(158, 175)
        Me.cmbBaitai.Name = "cmbBaitai"
        Me.cmbBaitai.Size = New System.Drawing.Size(96, 23)
        Me.cmbBaitai.TabIndex = 83
        Me.cmbBaitai.Tag = "1"
        '
        'txtItakuCode
        '
        Me.txtItakuCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtItakuCode.Location = New System.Drawing.Point(158, 143)
        Me.txtItakuCode.MaxLength = 10
        Me.txtItakuCode.Name = "txtItakuCode"
        Me.txtItakuCode.Size = New System.Drawing.Size(104, 22)
        Me.txtItakuCode.TabIndex = 82
        Me.txtItakuCode.Tag = "1"
        '
        'btnKousin
        '
        Me.btnKousin.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKousin.Location = New System.Drawing.Point(114, 278)
        Me.btnKousin.Name = "btnKousin"
        Me.btnKousin.Size = New System.Drawing.Size(80, 32)
        Me.btnKousin.TabIndex = 88
        Me.btnKousin.Text = "更　新"
        '
        'btnEraser
        '
        Me.btnEraser.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEraser.Location = New System.Drawing.Point(298, 278)
        Me.btnEraser.Name = "btnEraser"
        Me.btnEraser.Size = New System.Drawing.Size(80, 32)
        Me.btnEraser.TabIndex = 90
        Me.btnEraser.Text = "取　消"
        '
        'btnDelete
        '
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(206, 278)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(80, 32)
        Me.btnDelete.TabIndex = 89
        Me.btnDelete.Text = "削　除"
        '
        'cmbKamoku
        '
        Me.cmbKamoku.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKamoku.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKamoku.Location = New System.Drawing.Point(158, 111)
        Me.cmbKamoku.Name = "cmbKamoku"
        Me.cmbKamoku.Size = New System.Drawing.Size(104, 23)
        Me.cmbKamoku.TabIndex = 80
        Me.cmbKamoku.Tag = "1"
        '
        'txtKouza
        '
        Me.txtKouza.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKouza.Location = New System.Drawing.Point(372, 111)
        Me.txtKouza.MaxLength = 7
        Me.txtKouza.Name = "txtKouza"
        Me.txtKouza.Size = New System.Drawing.Size(67, 22)
        Me.txtKouza.TabIndex = 81
        Me.txtKouza.Tag = "1"
        '
        'txtSitCode
        '
        Me.txtSitCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSitCode.Location = New System.Drawing.Point(158, 79)
        Me.txtSitCode.MaxLength = 3
        Me.txtSitCode.Name = "txtSitCode"
        Me.txtSitCode.Size = New System.Drawing.Size(40, 22)
        Me.txtSitCode.TabIndex = 79
        Me.txtSitCode.Tag = "1"
        '
        'txtKinCode
        '
        Me.txtKinCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode.Location = New System.Drawing.Point(158, 47)
        Me.txtKinCode.MaxLength = 4
        Me.txtKinCode.Name = "txtKinCode"
        Me.txtKinCode.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode.TabIndex = 78
        Me.txtKinCode.Tag = "1"
        '
        'txtSousinFile
        '
        Me.txtSousinFile.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSousinFile.Location = New System.Drawing.Point(158, 207)
        Me.txtSousinFile.MaxLength = 26
        Me.txtSousinFile.Name = "txtSousinFile"
        Me.txtSousinFile.Size = New System.Drawing.Size(312, 22)
        Me.txtSousinFile.TabIndex = 85
        Me.txtSousinFile.Tag = "1"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(390, 278)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(80, 32)
        Me.btnEnd.TabIndex = 91
        Me.btnEnd.Text = "終　了"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(22, 278)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(80, 32)
        Me.btnAction.TabIndex = 87
        Me.btnAction.Text = "登　録"
        '
        'Label11
        '
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(0, 8)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(495, 30)
        Me.Label11.TabIndex = 103
        Me.Label11.Text = "＜他行マスタ詳細＞"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmbCodeKbn
        '
        Me.cmbCodeKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCodeKbn.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbCodeKbn.Location = New System.Drawing.Point(372, 175)
        Me.cmbCodeKbn.Name = "cmbCodeKbn"
        Me.cmbCodeKbn.Size = New System.Drawing.Size(104, 21)
        Me.cmbCodeKbn.TabIndex = 84
        Me.cmbCodeKbn.Tag = "1"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(277, 178)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(93, 16)
        Me.Label3.TabIndex = 100
        Me.Label3.Text = "コード区分"
        '
        'KFJMAST041
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(494, 335)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.lblSIT_NM)
        Me.Controls.Add(Me.lblKIN_NM)
        Me.Controls.Add(Me.txtJyusinFile)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.cmbCodeKbn)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.cmbBaitai)
        Me.Controls.Add(Me.txtItakuCode)
        Me.Controls.Add(Me.btnKousin)
        Me.Controls.Add(Me.btnEraser)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.cmbKamoku)
        Me.Controls.Add(Me.txtKouza)
        Me.Controls.Add(Me.txtSitCode)
        Me.Controls.Add(Me.txtKinCode)
        Me.Controls.Add(Me.txtSousinFile)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(500, 360)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(500, 360)
        Me.Name = "KFJMAST041"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAST041"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblSIT_NM As System.Windows.Forms.Label
    Friend WithEvents lblKIN_NM As System.Windows.Forms.Label
    Friend WithEvents txtJyusinFile As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents cmbBaitai As System.Windows.Forms.ComboBox
    Friend WithEvents txtItakuCode As System.Windows.Forms.TextBox
    Friend WithEvents btnKousin As System.Windows.Forms.Button
    Friend WithEvents btnEraser As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents cmbKamoku As System.Windows.Forms.ComboBox
    Friend WithEvents txtKouza As System.Windows.Forms.TextBox
    Friend WithEvents txtSitCode As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode As System.Windows.Forms.TextBox
    Friend WithEvents txtSousinFile As System.Windows.Forms.TextBox
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents cmbCodeKbn As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
