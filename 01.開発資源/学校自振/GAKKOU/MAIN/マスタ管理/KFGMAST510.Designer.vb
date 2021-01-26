<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAST510
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
    Friend WithEvents CmdFinal As System.Windows.Forms.Button
    Friend WithEvents CmdCreate As System.Windows.Forms.Button
    Friend WithEvents FURI_DATE_L As System.Windows.Forms.Label
    Friend WithEvents FURI_DATE As System.Windows.Forms.DateTimePicker
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents CK_L As System.Windows.Forms.Label
    Friend WithEvents CmdSelect As System.Windows.Forms.Button
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtSMonth As System.Windows.Forms.TextBox
    Friend WithEvents txtSYear As System.Windows.Forms.TextBox
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnAllOff As System.Windows.Forms.Button
    Friend WithEvents btnAllOn As System.Windows.Forms.Button
    Friend WithEvents cmbPrint As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAST510))
        Me.CmdFinal = New System.Windows.Forms.Button()
        Me.CmdCreate = New System.Windows.Forms.Button()
        Me.FURI_DATE_L = New System.Windows.Forms.Label()
        Me.FURI_DATE = New System.Windows.Forms.DateTimePicker()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.CK_L = New System.Windows.Forms.Label()
        Me.CmdSelect = New System.Windows.Forms.Button()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtSMonth = New System.Windows.Forms.TextBox()
        Me.txtSYear = New System.Windows.Forms.TextBox()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnAllOff = New System.Windows.Forms.Button()
        Me.btnAllOn = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'CmdFinal
        '
        Me.CmdFinal.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdFinal.Location = New System.Drawing.Point(660, 521)
        Me.CmdFinal.Name = "CmdFinal"
        Me.CmdFinal.Size = New System.Drawing.Size(120, 40)
        Me.CmdFinal.TabIndex = 7
        Me.CmdFinal.Text = "終　了"
        '
        'CmdCreate
        '
        Me.CmdCreate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdCreate.Location = New System.Drawing.Point(140, 521)
        Me.CmdCreate.Name = "CmdCreate"
        Me.CmdCreate.Size = New System.Drawing.Size(120, 40)
        Me.CmdCreate.TabIndex = 6
        Me.CmdCreate.Text = "移　出"
        '
        'FURI_DATE_L
        '
        Me.FURI_DATE_L.AutoSize = True
        Me.FURI_DATE_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_L.Location = New System.Drawing.Point(270, 81)
        Me.FURI_DATE_L.Name = "FURI_DATE_L"
        Me.FURI_DATE_L.Size = New System.Drawing.Size(56, 16)
        Me.FURI_DATE_L.TabIndex = 10
        Me.FURI_DATE_L.Text = "振替日"
        '
        'FURI_DATE
        '
        Me.FURI_DATE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.FURI_DATE.Location = New System.Drawing.Point(328, 76)
        Me.FURI_DATE.Name = "FURI_DATE"
        Me.FURI_DATE.Size = New System.Drawing.Size(255, 23)
        Me.FURI_DATE.TabIndex = 3
        '
        'ListView1
        '
        Me.ListView1.CheckBoxes = True
        Me.ListView1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(99, 152)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(592, 344)
        Me.ListView1.TabIndex = 5
        Me.ListView1.TabStop = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'CK_L
        '
        Me.CK_L.AutoSize = True
        Me.CK_L.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CK_L.ForeColor = System.Drawing.Color.Brown
        Me.CK_L.Location = New System.Drawing.Point(349, 62)
        Me.CK_L.Name = "CK_L"
        Me.CK_L.Size = New System.Drawing.Size(113, 12)
        Me.CK_L.TabIndex = 11
        Me.CK_L.Text = "営業日判定結果表示"
        '
        'CmdSelect
        '
        Me.CmdSelect.Font = New System.Drawing.Font("MS UI Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdSelect.Location = New System.Drawing.Point(590, 74)
        Me.CmdSelect.Name = "CmdSelect"
        Me.CmdSelect.Size = New System.Drawing.Size(64, 25)
        Me.CmdSelect.TabIndex = 4
        Me.CmdSelect.Text = "参　照"
        '
        'CheckBox1
        '
        Me.CheckBox1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CheckBox1.Location = New System.Drawing.Point(16, 48)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(188, 23)
        Me.CheckBox1.TabIndex = 0
        Me.CheckBox1.TabStop = False
        Me.CheckBox1.Text = "請求対象年月指定"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(215, 80)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(24, 16)
        Me.Label11.TabIndex = 168
        Me.Label11.Text = "月"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(167, 80)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(24, 16)
        Me.Label12.TabIndex = 167
        Me.Label12.Text = "年"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(13, 80)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(104, 16)
        Me.Label13.TabIndex = 166
        Me.Label13.Text = "請求対象年月"
        '
        'txtSMonth
        '
        Me.txtSMonth.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSMonth.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSMonth.Location = New System.Drawing.Point(191, 76)
        Me.txtSMonth.MaxLength = 2
        Me.txtSMonth.Name = "txtSMonth"
        Me.txtSMonth.Size = New System.Drawing.Size(24, 22)
        Me.txtSMonth.TabIndex = 2
        Me.txtSMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtSYear
        '
        Me.txtSYear.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSYear.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSYear.Location = New System.Drawing.Point(119, 76)
        Me.txtSYear.MaxLength = 4
        Me.txtSYear.Name = "txtSYear"
        Me.txtSYear.Size = New System.Drawing.Size(44, 22)
        Me.txtSYear.TabIndex = 1
        Me.txtSYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 218
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 217
        Me.lblUser.Text = "ユーザ名"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(592, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 216
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 215
        Me.Label1.Text = "ログイン名　:"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 219
        Me.Label3.Text = "＜学校ツール連携（データ移出）＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnAllOff
        '
        Me.btnAllOff.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAllOff.Location = New System.Drawing.Point(189, 116)
        Me.btnAllOff.Name = "btnAllOff"
        Me.btnAllOff.Size = New System.Drawing.Size(80, 30)
        Me.btnAllOff.TabIndex = 221
        Me.btnAllOff.TabStop = False
        Me.btnAllOff.Text = "全解除"
        '
        'btnAllOn
        '
        Me.btnAllOn.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAllOn.Location = New System.Drawing.Point(99, 116)
        Me.btnAllOn.Name = "btnAllOn"
        Me.btnAllOn.Size = New System.Drawing.Size(80, 30)
        Me.btnAllOn.TabIndex = 220
        Me.btnAllOn.TabStop = False
        Me.btnAllOn.Text = "全選択"
        '
        'KFGMAST510
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.btnAllOff)
        Me.Controls.Add(Me.btnAllOn)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.txtSMonth)
        Me.Controls.Add(Me.txtSYear)
        Me.Controls.Add(Me.CK_L)
        Me.Controls.Add(Me.FURI_DATE_L)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.CmdSelect)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.FURI_DATE)
        Me.Controls.Add(Me.CmdFinal)
        Me.Controls.Add(Me.CmdCreate)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGMAST510"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAST510"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub


    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '開始年月(年)
        AddHandler txtSYear.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSYear.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSYear.LostFocus, AddressOf CAST.LostFocus

        '開始年月(月)
        AddHandler txtSMonth.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSMonth.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSMonth.LostFocus, AddressOf CAST.LostFocus


    End Sub

End Class
