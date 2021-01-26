<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGPRNT160
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
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents cmbGakkouName As System.Windows.Forms.ComboBox
    Friend WithEvents txtGAKKOU_CODE As System.Windows.Forms.TextBox
    Friend WithEvents lab学校名 As System.Windows.Forms.Label
    Friend WithEvents btnPrnt As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGPRNT160))
        Me.btnPrnt = New System.Windows.Forms.Button()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.lab学校名 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtGAKKOU_CODE = New System.Windows.Forms.TextBox()
        Me.cmbGakkouName = New System.Windows.Forms.ComboBox()
        Me.cmbKana = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtDateD = New System.Windows.Forms.TextBox()
        Me.txtDateM = New System.Windows.Forms.TextBox()
        Me.txtDateY = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'btnPrnt
        '
        Me.btnPrnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrnt.Location = New System.Drawing.Point(534, 521)
        Me.btnPrnt.Name = "btnPrnt"
        Me.btnPrnt.Size = New System.Drawing.Size(120, 40)
        Me.btnPrnt.TabIndex = 4
        Me.btnPrnt.Text = "印　刷"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 521)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 5
        Me.btnEnd.Text = "終　了"
        '
        'lab学校名
        '
        Me.lab学校名.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校名.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校名.Location = New System.Drawing.Point(360, 214)
        Me.lab学校名.Name = "lab学校名"
        Me.lab学校名.Size = New System.Drawing.Size(333, 24)
        Me.lab学校名.TabIndex = 141
        Me.lab学校名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(152, 218)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 16)
        Me.Label5.TabIndex = 136
        Me.Label5.Text = "学校コード"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(152, 170)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 135
        Me.Label4.Text = "学校検索"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(667, 44)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 134
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(667, 20)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 133
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(780, 30)
        Me.Label3.TabIndex = 132
        Me.Label3.Text = "＜学校マスタ項目確認票印刷＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(579, 44)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 131
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(595, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 130
        Me.Label1.Text = "ログイン名　:"
        '
        'txtGAKKOU_CODE
        '
        Me.txtGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKKOU_CODE.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKKOU_CODE.Location = New System.Drawing.Point(264, 215)
        Me.txtGAKKOU_CODE.MaxLength = 10
        Me.txtGAKKOU_CODE.Name = "txtGAKKOU_CODE"
        Me.txtGAKKOU_CODE.Size = New System.Drawing.Size(90, 23)
        Me.txtGAKKOU_CODE.TabIndex = 0
        '
        'cmbGakkouName
        '
        Me.cmbGakkouName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGakkouName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGakkouName.Location = New System.Drawing.Point(313, 168)
        Me.cmbGakkouName.Name = "cmbGakkouName"
        Me.cmbGakkouName.Size = New System.Drawing.Size(380, 21)
        Me.cmbGakkouName.TabIndex = 125
        Me.cmbGakkouName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(264, 168)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 124
        Me.cmbKana.TabStop = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Label10.Location = New System.Drawing.Point(419, 264)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 339
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Label9.Location = New System.Drawing.Point(365, 264)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 338
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Label8.Location = New System.Drawing.Point(311, 264)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 337
        Me.Label8.Text = "年"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.Location = New System.Drawing.Point(152, 264)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(93, 16)
        Me.Label6.TabIndex = 336
        Me.Label6.Text = "基　準　日"
        '
        'txtDateD
        '
        Me.txtDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.txtDateD.Location = New System.Drawing.Point(392, 261)
        Me.txtDateD.MaxLength = 2
        Me.txtDateD.Name = "txtDateD"
        Me.txtDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtDateD.TabIndex = 3
        '
        'txtDateM
        '
        Me.txtDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.txtDateM.Location = New System.Drawing.Point(338, 261)
        Me.txtDateM.MaxLength = 2
        Me.txtDateM.Name = "txtDateM"
        Me.txtDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtDateM.TabIndex = 2
        '
        'txtDateY
        '
        Me.txtDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.txtDateY.Location = New System.Drawing.Point(264, 261)
        Me.txtDateY.MaxLength = 4
        Me.txtDateY.Name = "txtDateY"
        Me.txtDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtDateY.TabIndex = 1
        '
        'KFGPRNT160
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtDateD)
        Me.Controls.Add(Me.txtDateM)
        Me.Controls.Add(Me.txtDateY)
        Me.Controls.Add(Me.btnPrnt)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.lab学校名)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtGAKKOU_CODE)
        Me.Controls.Add(Me.cmbGakkouName)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGPRNT160"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGPRNT160"
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
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbGakkouName.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtGAKKOU_CODE.GotFocus, AddressOf CASTx01.GotFocus

        '***************
        'KeyPress
        '***************
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbGakkouName.KeyPress, AddressOf CAST.KeyPress
        AddHandler txtGAKKOU_CODE.KeyPress, AddressOf CASTx01.KeyPressNum

        '***************
        'LostFocus
        '***************
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbGakkouName.LostFocus, AddressOf CAST.LostFocus
        AddHandler txtGAKKOU_CODE.LostFocus, AddressOf CASTx01.LostFocusZero

        '基準日(年)
        AddHandler txtDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtDateY.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtDateY.KeyPress, AddressOf CASTx01.KeyPress

        '基準日(月)
        AddHandler txtDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtDateM.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtDateM.KeyPress, AddressOf CASTx01.KeyPress

        '基準日(日)
        AddHandler txtDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtDateD.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtDateD.KeyPress, AddressOf CASTx01.KeyPress
    End Sub
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtDateY As System.Windows.Forms.TextBox

End Class
