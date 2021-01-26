<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAST041
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAST041))
        Me.Button5 = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.labSEITO_KANJI = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtGAKUNEN = New System.Windows.Forms.TextBox
        Me.txtCLASS = New System.Windows.Forms.TextBox
        Me.txtSEITONO = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.labSEITO_KANA = New System.Windows.Forms.Label
        Me.txtTUUBAN = New System.Windows.Forms.TextBox
        Me.txtNENDO = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.btnClear = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Button5
        '
        Me.Button5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button5.Location = New System.Drawing.Point(-120, 103)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(120, 32)
        Me.Button5.TabIndex = 441
        Me.Button5.Text = "登　録"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(388, 202)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 6
        Me.btnEnd.Text = "閉じる"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(16, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(303, 15)
        Me.Label1.TabIndex = 442
        Me.Label1.Text = "長子となる生徒を設定してください。"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(32, 100)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(39, 15)
        Me.Label2.TabIndex = 443
        Me.Label2.Text = "学年"
        '
        'labSEITO_KANJI
        '
        Me.labSEITO_KANJI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.labSEITO_KANJI.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labSEITO_KANJI.Location = New System.Drawing.Point(187, 168)
        Me.labSEITO_KANJI.Name = "labSEITO_KANJI"
        Me.labSEITO_KANJI.Size = New System.Drawing.Size(288, 23)
        Me.labSEITO_KANJI.TabIndex = 444
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(32, 169)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(120, 15)
        Me.Label4.TabIndex = 445
        Me.Label4.Text = "生徒氏名(漢字)"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(138, 100)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(55, 15)
        Me.Label5.TabIndex = 446
        Me.Label5.Text = "クラス"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(242, 100)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(71, 15)
        Me.Label6.TabIndex = 447
        Me.Label6.Text = "生徒番号"
        '
        'txtGAKUNEN
        '
        Me.txtGAKUNEN.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKUNEN.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKUNEN.Location = New System.Drawing.Point(103, 97)
        Me.txtGAKUNEN.MaxLength = 1
        Me.txtGAKUNEN.Name = "txtGAKUNEN"
        Me.txtGAKUNEN.Size = New System.Drawing.Size(32, 22)
        Me.txtGAKUNEN.TabIndex = 2
        Me.txtGAKUNEN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtCLASS
        '
        Me.txtCLASS.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtCLASS.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtCLASS.Location = New System.Drawing.Point(199, 97)
        Me.txtCLASS.MaxLength = 2
        Me.txtCLASS.Name = "txtCLASS"
        Me.txtCLASS.Size = New System.Drawing.Size(32, 22)
        Me.txtCLASS.TabIndex = 3
        '
        'txtSEITONO
        '
        Me.txtSEITONO.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSEITONO.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSEITONO.Location = New System.Drawing.Point(319, 97)
        Me.txtSEITONO.MaxLength = 7
        Me.txtSEITONO.Name = "txtSEITONO"
        Me.txtSEITONO.Size = New System.Drawing.Size(73, 22)
        Me.txtSEITONO.TabIndex = 4
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(32, 136)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(120, 24)
        Me.Label7.TabIndex = 451
        Me.Label7.Text = "生徒氏名(カナ)"
        '
        'labSEITO_KANA
        '
        Me.labSEITO_KANA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.labSEITO_KANA.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labSEITO_KANA.Location = New System.Drawing.Point(187, 136)
        Me.labSEITO_KANA.Name = "labSEITO_KANA"
        Me.labSEITO_KANA.Size = New System.Drawing.Size(288, 23)
        Me.labSEITO_KANA.TabIndex = 452
        '
        'txtTUUBAN
        '
        Me.txtTUUBAN.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTUUBAN.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtTUUBAN.Location = New System.Drawing.Point(270, 57)
        Me.txtTUUBAN.MaxLength = 4
        Me.txtTUUBAN.Name = "txtTUUBAN"
        Me.txtTUUBAN.Size = New System.Drawing.Size(42, 22)
        Me.txtTUUBAN.TabIndex = 1
        '
        'txtNENDO
        '
        Me.txtNENDO.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtNENDO.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtNENDO.Location = New System.Drawing.Point(102, 57)
        Me.txtNENDO.MaxLength = 4
        Me.txtNENDO.Name = "txtNENDO"
        Me.txtNENDO.Size = New System.Drawing.Size(42, 22)
        Me.txtNENDO.TabIndex = 0
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(225, 60)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(39, 15)
        Me.Label9.TabIndex = 454
        Me.Label9.Text = "通番"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(32, 60)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(71, 15)
        Me.Label10.TabIndex = 453
        Me.Label10.Text = "入学年度"
        '
        'Label11
        '
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(15, 32)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(449, 16)
        Me.Label11.TabIndex = 457
        Me.Label11.Text = "入学年度・通番または学年・ｸﾗｽ・生徒番号を設定してください。"
        '
        'btnClear
        '
        Me.btnClear.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnClear.Location = New System.Drawing.Point(12, 202)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(120, 40)
        Me.btnClear.TabIndex = 5
        Me.btnClear.Text = "クリア"
        '
        'KFGMAST041
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(519, 260)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.txtTUUBAN)
        Me.Controls.Add(Me.txtNENDO)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.labSEITO_KANA)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtSEITONO)
        Me.Controls.Add(Me.txtCLASS)
        Me.Controls.Add(Me.txtGAKUNEN)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.labSEITO_KANJI)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.btnEnd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(525, 285)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(525, 285)
        Me.Name = "KFGMAST041"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAST041"
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

        '*********************
        'txtNENDO
        '*********************
        AddHandler txtNENDO.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtNENDO.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtNENDO.LostFocus, AddressOf CASTx01.LostFocusZero

        '*********************
        'txtTUUBAN
        '*********************
        AddHandler txtTUUBAN.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTUUBAN.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtTUUBAN.LostFocus, AddressOf CASTx01.LostFocusZero

        '*********************
        'txtGAKUNEN
        '*********************
        AddHandler txtGAKUNEN.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtGAKUNEN.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtGAKUNEN.LostFocus, AddressOf CASTx01.LostFocusZero

        '*********************
        'txtCLASS
        '*********************
        AddHandler txtCLASS.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtCLASS.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtCLASS.LostFocus, AddressOf CASTx01.LostFocusZero

        '*********************
        'txtSEITONO
        '*********************
        AddHandler txtSEITONO.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSEITONO.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSEITONO.LostFocus, AddressOf CASTx01.LostFocusZero

    End Sub

    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents labSEITO_KANJI As System.Windows.Forms.Label
    Friend WithEvents txtGAKUNEN As System.Windows.Forms.TextBox
    Friend WithEvents txtCLASS As System.Windows.Forms.TextBox
    Friend WithEvents txtSEITONO As System.Windows.Forms.TextBox
    Friend WithEvents labSEITO_KANA As System.Windows.Forms.Label
    Friend WithEvents txtTUUBAN As System.Windows.Forms.TextBox
    Friend WithEvents txtNENDO As System.Windows.Forms.TextBox
    Friend WithEvents btnClear As System.Windows.Forms.Button

End Class
