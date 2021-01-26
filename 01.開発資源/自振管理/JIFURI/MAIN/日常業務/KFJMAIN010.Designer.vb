<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAIN010
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '金庫持込ラジオボタン
        AddHandler rbKinko.GotFocus, AddressOf CAST.GotFocus
        AddHandler rbKinko.LostFocus, AddressOf CAST.LostFocus
        AddHandler rbKinko.KeyPress, AddressOf CAST.KeyPress

        'センター直接持込ラジオボタン
        AddHandler rbCenter.GotFocus, AddressOf CAST.GotFocus
        AddHandler rbCenter.LostFocus, AddressOf CAST.LostFocus
        AddHandler rbCenter.KeyPress, AddressOf CAST.KeyPress

        '取引先主コード／金庫持込
        AddHandler txtTorisCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorisCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorisCode.LostFocus, AddressOf CASTx01.LostFocus

        '取引先副コード／金庫持込
        AddHandler txtTorifCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorifCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorifCode.LostFocus, AddressOf CASTx01.LostFocus

        'カタカナコンボボックス／金庫持込
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress

        '取引先名コンボボックス／金庫持込
        AddHandler cmbToriName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbToriName.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbToriName.KeyPress, AddressOf CAST.KeyPress

        '振替日(年)／金庫持込
        AddHandler txtKinkoFuriDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKinkoFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKinkoFuriDateY.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(月)／金庫持込
        AddHandler txtKinkoFuriDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKinkoFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKinkoFuriDateM.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(日)／金庫持込
        AddHandler txtKinkoFuriDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtKinkoFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKinkoFuriDateD.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(年)／センター直接持込
        AddHandler txtCenterFuriDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtCenterFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtCenterFuriDateY.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(月)／センター直接持込
        AddHandler txtCenterFuriDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtCenterFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtCenterFuriDateM.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(日)／センター直接持込
        AddHandler txtCenterFuriDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtCenterFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtCenterFuriDateD.LostFocus, AddressOf CASTx01.LostFocus

        '媒体コンボボックス／センター直接持込
        AddHandler cmbBaitai.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbBaitai.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbBaitai.KeyPress, AddressOf CAST.KeyPress

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAIN010))
        Me.lblTitle = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.btnAction = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.cmbToriName = New System.Windows.Forms.ComboBox
        Me.txtTorisCode = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.txtKinkoFuriDateY = New System.Windows.Forms.TextBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.txtKinkoFuriDateM = New System.Windows.Forms.TextBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.txtKinkoFuriDateD = New System.Windows.Forms.TextBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.txtTorifCode = New System.Windows.Forms.TextBox
        Me.Label14 = New System.Windows.Forms.Label
        Me.rbKinko = New System.Windows.Forms.RadioButton
        Me.rbCenter = New System.Windows.Forms.RadioButton
        Me.pKinko = New System.Windows.Forms.Panel
        Me.pCenter = New System.Windows.Forms.Panel
        Me.cmbBaitai = New System.Windows.Forms.ComboBox
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.txtCenterFuriDateY = New System.Windows.Forms.TextBox
        Me.txtCenterFuriDateM = New System.Windows.Forms.TextBox
        Me.txtCenterFuriDateD = New System.Windows.Forms.TextBox
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.lblFuriDate = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.pKinko.SuspendLayout()
        Me.pCenter.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "＜口振依頼データ落込＞"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(619, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ログイン名　:"
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
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(603, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "システム日付:"
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
        Me.btnAction.TabIndex = 14
        Me.btnAction.Text = "実　行"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 15
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(71, 21)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 16)
        Me.Label6.TabIndex = 4
        Me.Label6.Text = "取引先検索"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(71, 81)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(104, 16)
        Me.Label7.TabIndex = 4
        Me.Label7.Text = "取引先コード"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(71, 136)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(88, 16)
        Me.Label8.TabIndex = 4
        Me.Label8.Text = "振　替　日"
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbKana.FormattingEnabled = True
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(182, 19)
        Me.cmbKana.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 3
        Me.cmbKana.TabStop = False
        '
        'cmbToriName
        '
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbToriName.FormattingEnabled = True
        Me.cmbToriName.Location = New System.Drawing.Point(231, 19)
        Me.cmbToriName.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(350, 21)
        Me.cmbToriName.TabIndex = 4
        Me.cmbToriName.TabStop = False
        '
        'txtTorisCode
        '
        Me.txtTorisCode.BackColor = System.Drawing.Color.White
        Me.txtTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorisCode.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTorisCode.Location = New System.Drawing.Point(182, 79)
        Me.txtTorisCode.MaxLength = 10
        Me.txtTorisCode.Name = "txtTorisCode"
        Me.txtTorisCode.Size = New System.Drawing.Size(87, 23)
        Me.txtTorisCode.TabIndex = 5
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(279, 138)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 7
        Me.Label9.Text = "月"
        '
        'txtKinkoFuriDateY
        '
        Me.txtKinkoFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinkoFuriDateY.Location = New System.Drawing.Point(182, 134)
        Me.txtKinkoFuriDateY.MaxLength = 4
        Me.txtKinkoFuriDateY.Name = "txtKinkoFuriDateY"
        Me.txtKinkoFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtKinkoFuriDateY.TabIndex = 7
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(228, 138)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 7
        Me.Label10.Text = "年"
        '
        'txtKinkoFuriDateM
        '
        Me.txtKinkoFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinkoFuriDateM.Location = New System.Drawing.Point(253, 134)
        Me.txtKinkoFuriDateM.MaxLength = 2
        Me.txtKinkoFuriDateM.Name = "txtKinkoFuriDateM"
        Me.txtKinkoFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtKinkoFuriDateM.TabIndex = 8
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(330, 138)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(24, 16)
        Me.Label11.TabIndex = 7
        Me.Label11.Text = "日"
        '
        'txtKinkoFuriDateD
        '
        Me.txtKinkoFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinkoFuriDateD.Location = New System.Drawing.Point(304, 134)
        Me.txtKinkoFuriDateD.MaxLength = 2
        Me.txtKinkoFuriDateD.Name = "txtKinkoFuriDateD"
        Me.txtKinkoFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtKinkoFuriDateD.TabIndex = 9
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(120, 227)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(408, 16)
        Me.Label12.TabIndex = 4
        Me.Label12.Text = "※取引先コードALL1指定時は依頼書ベース全件処理する"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(120, 262)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(392, 16)
        Me.Label13.TabIndex = 4
        Me.Label13.Text = "※取引先コードALL2指定時は伝票ベース全件処理する"
        '
        'txtTorifCode
        '
        Me.txtTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorifCode.Location = New System.Drawing.Point(294, 79)
        Me.txtTorifCode.MaxLength = 2
        Me.txtTorifCode.Name = "txtTorifCode"
        Me.txtTorifCode.Size = New System.Drawing.Size(24, 23)
        Me.txtTorifCode.TabIndex = 6
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(275, 82)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(16, 16)
        Me.Label14.TabIndex = 7
        Me.Label14.Text = "-"
        '
        'rbKinko
        '
        Me.rbKinko.AutoSize = True
        Me.rbKinko.Checked = True
        Me.rbKinko.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbKinko.Location = New System.Drawing.Point(319, 93)
        Me.rbKinko.Name = "rbKinko"
        Me.rbKinko.Size = New System.Drawing.Size(90, 20)
        Me.rbKinko.TabIndex = 1
        Me.rbKinko.TabStop = True
        Me.rbKinko.Text = "金庫持込"
        Me.rbKinko.UseVisualStyleBackColor = True
        '
        'rbCenter
        '
        Me.rbCenter.AutoSize = True
        Me.rbCenter.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbCenter.Location = New System.Drawing.Point(424, 93)
        Me.rbCenter.Name = "rbCenter"
        Me.rbCenter.Size = New System.Drawing.Size(154, 20)
        Me.rbCenter.TabIndex = 2
        Me.rbCenter.Text = "センター直接持込"
        Me.rbCenter.UseVisualStyleBackColor = True
        '
        'pKinko
        '
        Me.pKinko.Controls.Add(Me.cmbToriName)
        Me.pKinko.Controls.Add(Me.Label6)
        Me.pKinko.Controls.Add(Me.Label7)
        Me.pKinko.Controls.Add(Me.Label11)
        Me.pKinko.Controls.Add(Me.Label8)
        Me.pKinko.Controls.Add(Me.Label14)
        Me.pKinko.Controls.Add(Me.Label12)
        Me.pKinko.Controls.Add(Me.Label10)
        Me.pKinko.Controls.Add(Me.Label13)
        Me.pKinko.Controls.Add(Me.txtKinkoFuriDateD)
        Me.pKinko.Controls.Add(Me.cmbKana)
        Me.pKinko.Controls.Add(Me.txtKinkoFuriDateM)
        Me.pKinko.Controls.Add(Me.txtTorisCode)
        Me.pKinko.Controls.Add(Me.txtKinkoFuriDateY)
        Me.pKinko.Controls.Add(Me.txtTorifCode)
        Me.pKinko.Controls.Add(Me.Label9)
        Me.pKinko.Location = New System.Drawing.Point(113, 151)
        Me.pKinko.Name = "pKinko"
        Me.pKinko.Size = New System.Drawing.Size(616, 325)
        Me.pKinko.TabIndex = 9
        '
        'pCenter
        '
        Me.pCenter.Controls.Add(Me.cmbBaitai)
        Me.pCenter.Controls.Add(Me.Label15)
        Me.pCenter.Controls.Add(Me.Label16)
        Me.pCenter.Controls.Add(Me.txtCenterFuriDateY)
        Me.pCenter.Controls.Add(Me.txtCenterFuriDateM)
        Me.pCenter.Controls.Add(Me.txtCenterFuriDateD)
        Me.pCenter.Controls.Add(Me.Label17)
        Me.pCenter.Controls.Add(Me.Label18)
        Me.pCenter.Controls.Add(Me.lblFuriDate)
        Me.pCenter.Location = New System.Drawing.Point(129, 148)
        Me.pCenter.Name = "pCenter"
        Me.pCenter.Size = New System.Drawing.Size(589, 325)
        Me.pCenter.TabIndex = 10
        Me.pCenter.Visible = False
        '
        'cmbBaitai
        '
        Me.cmbBaitai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBaitai.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbBaitai.FormattingEnabled = True
        Me.cmbBaitai.Location = New System.Drawing.Point(233, 102)
        Me.cmbBaitai.Name = "cmbBaitai"
        Me.cmbBaitai.Size = New System.Drawing.Size(171, 24)
        Me.cmbBaitai.TabIndex = 13
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(331, 50)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(24, 16)
        Me.Label15.TabIndex = 31
        Me.Label15.Text = "月"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(383, 50)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(24, 16)
        Me.Label16.TabIndex = 32
        Me.Label16.Text = "日"
        '
        'txtCenterFuriDateY
        '
        Me.txtCenterFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtCenterFuriDateY.Location = New System.Drawing.Point(233, 46)
        Me.txtCenterFuriDateY.MaxLength = 4
        Me.txtCenterFuriDateY.Name = "txtCenterFuriDateY"
        Me.txtCenterFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtCenterFuriDateY.TabIndex = 10
        '
        'txtCenterFuriDateM
        '
        Me.txtCenterFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtCenterFuriDateM.Location = New System.Drawing.Point(305, 46)
        Me.txtCenterFuriDateM.MaxLength = 2
        Me.txtCenterFuriDateM.Name = "txtCenterFuriDateM"
        Me.txtCenterFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtCenterFuriDateM.TabIndex = 11
        '
        'txtCenterFuriDateD
        '
        Me.txtCenterFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtCenterFuriDateD.Location = New System.Drawing.Point(357, 46)
        Me.txtCenterFuriDateD.MaxLength = 2
        Me.txtCenterFuriDateD.Name = "txtCenterFuriDateD"
        Me.txtCenterFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtCenterFuriDateD.TabIndex = 12
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(279, 50)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(24, 16)
        Me.Label17.TabIndex = 30
        Me.Label17.Text = "年"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(121, 104)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(72, 16)
        Me.Label18.TabIndex = 29
        Me.Label18.Text = "媒体区分"
        '
        'lblFuriDate
        '
        Me.lblFuriDate.AutoSize = True
        Me.lblFuriDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDate.Location = New System.Drawing.Point(121, 49)
        Me.lblFuriDate.Name = "lblFuriDate"
        Me.lblFuriDate.Size = New System.Drawing.Size(72, 16)
        Me.lblFuriDate.TabIndex = 29
        Me.lblFuriDate.Text = "登 録 日"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(231, 95)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(72, 16)
        Me.Label19.TabIndex = 11
        Me.Label19.Text = "持込区分"
        '
        'KFJMAIN010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.rbCenter)
        Me.Controls.Add(Me.rbKinko)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pKinko)
        Me.Controls.Add(Me.pCenter)
        Me.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMAIN010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAIN010"
        Me.pKinko.ResumeLayout(False)
        Me.pKinko.PerformLayout()
        Me.pCenter.ResumeLayout(False)
        Me.pCenter.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents txtTorisCode As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtKinkoFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtKinkoFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtKinkoFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtTorifCode As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents rbKinko As System.Windows.Forms.RadioButton
    Friend WithEvents rbCenter As System.Windows.Forms.RadioButton
    Friend WithEvents pKinko As System.Windows.Forms.Panel
    Friend WithEvents pCenter As System.Windows.Forms.Panel
    Friend WithEvents cmbBaitai As System.Windows.Forms.ComboBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtCenterFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtCenterFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtCenterFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents lblFuriDate As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label

End Class
