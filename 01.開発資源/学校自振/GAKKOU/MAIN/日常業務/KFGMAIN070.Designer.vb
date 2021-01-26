<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAIN070
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
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents cmbGakkouName As System.Windows.Forms.ComboBox
    Friend WithEvents txtGAKKOU_CODE As System.Windows.Forms.TextBox
    Friend WithEvents lab学校名 As System.Windows.Forms.Label
    Friend WithEvents txtSYOKICHI As System.Windows.Forms.TextBox
    Friend WithEvents cmbFURIKAEBI As System.Windows.Forms.ComboBox
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents rdoButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoButton3 As System.Windows.Forms.RadioButton
    Friend WithEvents btnCreate As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAIN070))
        Me.txtSYOKICHI = New System.Windows.Forms.TextBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.btnAction = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.lab学校名 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtGAKKOU_CODE = New System.Windows.Forms.TextBox
        Me.cmbGakkouName = New System.Windows.Forms.ComboBox
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.cmbFURIKAEBI = New System.Windows.Forms.ComboBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.rdoButton1 = New System.Windows.Forms.RadioButton
        Me.rdoButton2 = New System.Windows.Forms.RadioButton
        Me.rdoButton3 = New System.Windows.Forms.RadioButton
        Me.btnCreate = New System.Windows.Forms.Button
        Me.Label7 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'txtSYOKICHI
        '
        Me.txtSYOKICHI.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSYOKICHI.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSYOKICHI.Location = New System.Drawing.Point(214, 257)
        Me.txtSYOKICHI.MaxLength = 12
        Me.txtSYOKICHI.Name = "txtSYOKICHI"
        Me.txtSYOKICHI.Size = New System.Drawing.Size(130, 23)
        Me.txtSYOKICHI.TabIndex = 2
        Me.txtSYOKICHI.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(102, 259)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(72, 16)
        Me.Label12.TabIndex = 0
        Me.Label12.Text = "初 期 値"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 6
        Me.btnAction.Text = "実　行"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 8
        Me.btnEnd.Text = "終　了"
        '
        'lab学校名
        '
        Me.lab学校名.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校名.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校名.Location = New System.Drawing.Point(310, 159)
        Me.lab学校名.Name = "lab学校名"
        Me.lab学校名.Size = New System.Drawing.Size(423, 24)
        Me.lab学校名.TabIndex = 196
        Me.lab学校名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(102, 209)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(77, 16)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "振 替 日"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(102, 163)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 16)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "学校コード"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(102, 117)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "学校検索"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 189
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 188
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "＜生徒明細入力＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(592, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 186
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 185
        Me.Label1.Text = "ログイン名　:"
        '
        'txtGAKKOU_CODE
        '
        Me.txtGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKKOU_CODE.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKKOU_CODE.Location = New System.Drawing.Point(214, 160)
        Me.txtGAKKOU_CODE.MaxLength = 10
        Me.txtGAKKOU_CODE.Name = "txtGAKKOU_CODE"
        Me.txtGAKKOU_CODE.Size = New System.Drawing.Size(90, 23)
        Me.txtGAKKOU_CODE.TabIndex = 0
        '
        'cmbGakkouName
        '
        Me.cmbGakkouName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGakkouName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGakkouName.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.cmbGakkouName.Location = New System.Drawing.Point(262, 113)
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
        Me.cmbKana.Location = New System.Drawing.Point(214, 113)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 0
        Me.cmbKana.TabStop = False
        '
        'cmbFURIKAEBI
        '
        Me.cmbFURIKAEBI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFURIKAEBI.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbFURIKAEBI.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.cmbFURIKAEBI.Location = New System.Drawing.Point(214, 209)
        Me.cmbFURIKAEBI.Name = "cmbFURIKAEBI"
        Me.cmbFURIKAEBI.Size = New System.Drawing.Size(184, 24)
        Me.cmbFURIKAEBI.TabIndex = 1
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(102, 305)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(104, 16)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "入力ソート順"
        '
        'rdoButton1
        '
        Me.rdoButton1.Checked = True
        Me.rdoButton1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdoButton1.Location = New System.Drawing.Point(214, 305)
        Me.rdoButton1.Name = "rdoButton1"
        Me.rdoButton1.Size = New System.Drawing.Size(264, 24)
        Me.rdoButton1.TabIndex = 3
        Me.rdoButton1.TabStop = True
        Me.rdoButton1.Text = "学年・クラス・生徒番号順"
        '
        'rdoButton2
        '
        Me.rdoButton2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdoButton2.Location = New System.Drawing.Point(214, 345)
        Me.rdoButton2.Name = "rdoButton2"
        Me.rdoButton2.Size = New System.Drawing.Size(248, 24)
        Me.rdoButton2.TabIndex = 4
        Me.rdoButton2.Text = "入学年度・通番順"
        '
        'rdoButton3
        '
        Me.rdoButton3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdoButton3.Location = New System.Drawing.Point(214, 385)
        Me.rdoButton3.Name = "rdoButton3"
        Me.rdoButton3.Size = New System.Drawing.Size(192, 24)
        Me.rdoButton3.TabIndex = 5
        Me.rdoButton3.Text = "あいうえお順"
        '
        'btnCreate
        '
        Me.btnCreate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnCreate.Location = New System.Drawing.Point(270, 520)
        Me.btnCreate.Name = "btnCreate"
        Me.btnCreate.Size = New System.Drawing.Size(120, 40)
        Me.btnCreate.TabIndex = 7
        Me.btnCreate.Text = "データ作成"
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(151, 434)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(558, 36)
        Me.Label7.TabIndex = 197
        Me.Label7.Text = "※口座振替予定明細表は学校マスタ登録画面で指定したソート順で出力"
        '
        'KFGMAIN070
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.btnCreate)
        Me.Controls.Add(Me.rdoButton3)
        Me.Controls.Add(Me.rdoButton2)
        Me.Controls.Add(Me.rdoButton1)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.cmbFURIKAEBI)
        Me.Controls.Add(Me.txtSYOKICHI)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.lab学校名)
        Me.Controls.Add(Me.Label6)
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
        Me.Name = "KFGMAIN070"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAIN070"
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
        'cmbKana
        '****************
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus

        '****************
        'cmbGAKKOUNAME
        '****************
        AddHandler cmbGakkouName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbGakkouName.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbGakkouName.LostFocus, AddressOf CAST.LostFocus

        '****************
        'txtGAKKOU_CODE
        '****************
        AddHandler txtGAKKOU_CODE.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtGAKKOU_CODE.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtGAKKOU_CODE.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'cmbFURIKAEBI
        '****************
        AddHandler cmbFURIKAEBI.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbFURIKAEBI.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbFURIKAEBI.LostFocus, AddressOf CAST.LostFocus

        '****************
        'txtSYOKICHI
        '****************
        AddHandler txtSYOKICHI.GotFocus, AddressOf CASTx01.GotFocusMoney
        AddHandler txtSYOKICHI.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler txtSYOKICHI.LostFocus, AddressOf CASTx01.LostFocusMoney

    End Sub

End Class
