<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJKOZA010
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

    Public Sub New()
        MyBase.New()

        'この呼び出しは Windows フォーム デザイナで必要です。  
        InitializeComponent()

        AddHandler RadMode1.GotFocus, AddressOf CAST.GotFocus
        AddHandler RadMode1.KeyPress, AddressOf CAST.KeyPress
        AddHandler RadMode1.LostFocus, AddressOf CAST.LostFocus

        AddHandler RadMode2.GotFocus, AddressOf CAST.GotFocus
        AddHandler RadMode2.KeyPress, AddressOf CAST.KeyPress
        AddHandler RadMode2.LostFocus, AddressOf CAST.LostFocus

        AddHandler cmbSyubetu.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbSyubetu.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbSyubetu.LostFocus, AddressOf CAST.LostFocus

        AddHandler txtTorokuDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorokuDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorokuDateY.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler txtTorokuDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorokuDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorokuDateM.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler txtTorokuDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorokuDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorokuDateD.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler cmbSyubetu_KOBETU.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbSyubetu_KOBETU.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbSyubetu_KOBETU.LostFocus, AddressOf CAST.LostFocus

        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus

        AddHandler cmbToriName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbToriName.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbToriName.LostFocus, AddressOf CAST.LostFocus

        AddHandler txtTorisCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorisCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorisCode.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler txtTorifCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorifCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorifCode.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler txtFuriDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateY.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler txtFuriDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateM.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler txtFuriDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateD.LostFocus, AddressOf CASTx01.LostFocus

        AddHandler txtMotikomiSEQ.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtMotikomiSEQ.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtMotikomiSEQ.LostFocus, AddressOf CASTx01.LostFocus


    End Sub

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJKOZA010))
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.btnAction = New System.Windows.Forms.Button
        Me.btnClose = New System.Windows.Forms.Button
        Me.PanKobetu = New System.Windows.Forms.Panel
        Me.txtMotikomiSEQ = New System.Windows.Forms.TextBox
        Me.lblMotikomiSEQ = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.cmbToriName = New System.Windows.Forms.ComboBox
        Me.txtTorisCode = New System.Windows.Forms.TextBox
        Me.txtTorifCode = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtFuriDateY = New System.Windows.Forms.TextBox
        Me.txtFuriDateM = New System.Windows.Forms.TextBox
        Me.txtFuriDateD = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.lblFuriDate = New System.Windows.Forms.Label
        Me.cmbSyubetu_KOBETU = New System.Windows.Forms.ComboBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.PanIkkatu = New System.Windows.Forms.Panel
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.txtTorokuDateY = New System.Windows.Forms.TextBox
        Me.txtTorokuDateM = New System.Windows.Forms.TextBox
        Me.txtTorokuDateD = New System.Windows.Forms.TextBox
        Me.Label16 = New System.Windows.Forms.Label
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.cmbSyubetu = New System.Windows.Forms.ComboBox
        Me.RadMode1 = New System.Windows.Forms.RadioButton
        Me.RadMode2 = New System.Windows.Forms.RadioButton
        Me.PanKobetu.SuspendLayout()
        Me.PanIkkatu.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "＜口座チェック(当日受付分)＞"
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
        Me.btnAction.TabIndex = 4
        Me.btnAction.Text = "実　行"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnClose.Location = New System.Drawing.Point(660, 520)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(120, 40)
        Me.btnClose.TabIndex = 5
        Me.btnClose.Text = "終　了"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'PanKobetu
        '
        Me.PanKobetu.Controls.Add(Me.txtMotikomiSEQ)
        Me.PanKobetu.Controls.Add(Me.lblMotikomiSEQ)
        Me.PanKobetu.Controls.Add(Me.Label9)
        Me.PanKobetu.Controls.Add(Me.cmbKana)
        Me.PanKobetu.Controls.Add(Me.Label10)
        Me.PanKobetu.Controls.Add(Me.cmbToriName)
        Me.PanKobetu.Controls.Add(Me.txtTorisCode)
        Me.PanKobetu.Controls.Add(Me.txtTorifCode)
        Me.PanKobetu.Controls.Add(Me.Label7)
        Me.PanKobetu.Controls.Add(Me.txtFuriDateY)
        Me.PanKobetu.Controls.Add(Me.txtFuriDateM)
        Me.PanKobetu.Controls.Add(Me.txtFuriDateD)
        Me.PanKobetu.Controls.Add(Me.Label6)
        Me.PanKobetu.Controls.Add(Me.Label8)
        Me.PanKobetu.Controls.Add(Me.Label11)
        Me.PanKobetu.Controls.Add(Me.lblFuriDate)
        Me.PanKobetu.Controls.Add(Me.cmbSyubetu_KOBETU)
        Me.PanKobetu.Controls.Add(Me.Label13)
        Me.PanKobetu.Location = New System.Drawing.Point(155, 142)
        Me.PanKobetu.Name = "PanKobetu"
        Me.PanKobetu.Size = New System.Drawing.Size(585, 200)
        Me.PanKobetu.TabIndex = 3
        '
        'txtMotikomiSEQ
        '
        Me.txtMotikomiSEQ.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtMotikomiSEQ.Location = New System.Drawing.Point(422, 148)
        Me.txtMotikomiSEQ.MaxLength = 6
        Me.txtMotikomiSEQ.Name = "txtMotikomiSEQ"
        Me.txtMotikomiSEQ.Size = New System.Drawing.Size(67, 23)
        Me.txtMotikomiSEQ.TabIndex = 8
        '
        'lblMotikomiSEQ
        '
        Me.lblMotikomiSEQ.AutoSize = True
        Me.lblMotikomiSEQ.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblMotikomiSEQ.Location = New System.Drawing.Point(352, 151)
        Me.lblMotikomiSEQ.Name = "lblMotikomiSEQ"
        Me.lblMotikomiSEQ.Size = New System.Drawing.Size(64, 16)
        Me.lblMotikomiSEQ.TabIndex = 36
        Me.lblMotikomiSEQ.Text = "持込SEQ"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(242, 151)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 24
        Me.Label9.Text = "月"
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(140, 60)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 1
        Me.cmbKana.TabStop = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(296, 151)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 25
        Me.Label10.Text = "日"
        '
        'cmbToriName
        '
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbToriName.Location = New System.Drawing.Point(190, 60)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(371, 21)
        Me.cmbToriName.TabIndex = 2
        Me.cmbToriName.TabStop = False
        '
        'txtTorisCode
        '
        Me.txtTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorisCode.Location = New System.Drawing.Point(140, 104)
        Me.txtTorisCode.MaxLength = 10
        Me.txtTorisCode.Name = "txtTorisCode"
        Me.txtTorisCode.Size = New System.Drawing.Size(90, 23)
        Me.txtTorisCode.TabIndex = 3
        '
        'txtTorifCode
        '
        Me.txtTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorifCode.Location = New System.Drawing.Point(266, 104)
        Me.txtTorifCode.MaxLength = 2
        Me.txtTorifCode.Name = "txtTorifCode"
        Me.txtTorifCode.Size = New System.Drawing.Size(32, 23)
        Me.txtTorifCode.TabIndex = 4
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(236, 107)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 19
        Me.Label7.Text = "－"
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.Location = New System.Drawing.Point(140, 148)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 5
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.Location = New System.Drawing.Point(215, 148)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 6
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.Location = New System.Drawing.Point(269, 148)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 7
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(30, 60)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 16)
        Me.Label6.TabIndex = 12
        Me.Label6.Text = "取引先検索"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(187, 151)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 23
        Me.Label8.Text = "年"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(30, 107)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(104, 16)
        Me.Label11.TabIndex = 13
        Me.Label11.Text = "取引先コード"
        '
        'lblFuriDate
        '
        Me.lblFuriDate.AutoSize = True
        Me.lblFuriDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDate.Location = New System.Drawing.Point(30, 151)
        Me.lblFuriDate.Name = "lblFuriDate"
        Me.lblFuriDate.Size = New System.Drawing.Size(88, 16)
        Me.lblFuriDate.TabIndex = 14
        Me.lblFuriDate.Text = "振　替　日"
        '
        'cmbSyubetu_KOBETU
        '
        Me.cmbSyubetu_KOBETU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSyubetu_KOBETU.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbSyubetu_KOBETU.ItemHeight = 16
        Me.cmbSyubetu_KOBETU.Location = New System.Drawing.Point(140, 16)
        Me.cmbSyubetu_KOBETU.Name = "cmbSyubetu_KOBETU"
        Me.cmbSyubetu_KOBETU.Size = New System.Drawing.Size(104, 24)
        Me.cmbSyubetu_KOBETU.TabIndex = 0
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(30, 19)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(56, 16)
        Me.Label13.TabIndex = 35
        Me.Label13.Text = "種　別"
        '
        'PanIkkatu
        '
        Me.PanIkkatu.Controls.Add(Me.Label14)
        Me.PanIkkatu.Controls.Add(Me.Label15)
        Me.PanIkkatu.Controls.Add(Me.txtTorokuDateY)
        Me.PanIkkatu.Controls.Add(Me.txtTorokuDateM)
        Me.PanIkkatu.Controls.Add(Me.txtTorokuDateD)
        Me.PanIkkatu.Controls.Add(Me.Label16)
        Me.PanIkkatu.Controls.Add(Me.Label17)
        Me.PanIkkatu.Controls.Add(Me.Label18)
        Me.PanIkkatu.Controls.Add(Me.cmbSyubetu)
        Me.PanIkkatu.Location = New System.Drawing.Point(155, 142)
        Me.PanIkkatu.Name = "PanIkkatu"
        Me.PanIkkatu.Size = New System.Drawing.Size(500, 106)
        Me.PanIkkatu.TabIndex = 2
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(352, 63)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(24, 16)
        Me.Label14.TabIndex = 39
        Me.Label14.Text = "月"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(407, 63)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(24, 16)
        Me.Label15.TabIndex = 40
        Me.Label15.Text = "日"
        '
        'txtTorokuDateY
        '
        Me.txtTorokuDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorokuDateY.Location = New System.Drawing.Point(250, 60)
        Me.txtTorokuDateY.MaxLength = 4
        Me.txtTorokuDateY.Name = "txtTorokuDateY"
        Me.txtTorokuDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtTorokuDateY.TabIndex = 1
        '
        'txtTorokuDateM
        '
        Me.txtTorokuDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorokuDateM.Location = New System.Drawing.Point(324, 60)
        Me.txtTorokuDateM.MaxLength = 2
        Me.txtTorokuDateM.Name = "txtTorokuDateM"
        Me.txtTorokuDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtTorokuDateM.TabIndex = 2
        '
        'txtTorokuDateD
        '
        Me.txtTorokuDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorokuDateD.Location = New System.Drawing.Point(380, 60)
        Me.txtTorokuDateD.MaxLength = 2
        Me.txtTorokuDateD.Name = "txtTorokuDateD"
        Me.txtTorokuDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtTorokuDateD.TabIndex = 3
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(297, 63)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(24, 16)
        Me.Label16.TabIndex = 38
        Me.Label16.Text = "年"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(108, 63)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(136, 16)
        Me.Label17.TabIndex = 37
        Me.Label17.Text = "受付日（確定日）"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(108, 19)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(56, 16)
        Me.Label18.TabIndex = 33
        Me.Label18.Text = "種　別"
        '
        'cmbSyubetu
        '
        Me.cmbSyubetu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSyubetu.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbSyubetu.ItemHeight = 16
        Me.cmbSyubetu.Location = New System.Drawing.Point(250, 16)
        Me.cmbSyubetu.Name = "cmbSyubetu"
        Me.cmbSyubetu.Size = New System.Drawing.Size(108, 24)
        Me.cmbSyubetu.TabIndex = 0
        '
        'RadMode1
        '
        Me.RadMode1.Checked = True
        Me.RadMode1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.RadMode1.Location = New System.Drawing.Point(251, 92)
        Me.RadMode1.Name = "RadMode1"
        Me.RadMode1.Size = New System.Drawing.Size(134, 32)
        Me.RadMode1.TabIndex = 0
        Me.RadMode1.TabStop = True
        Me.RadMode1.Text = "一括チェック"
        '
        'RadMode2
        '
        Me.RadMode2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.RadMode2.Location = New System.Drawing.Point(427, 92)
        Me.RadMode2.Name = "RadMode2"
        Me.RadMode2.Size = New System.Drawing.Size(122, 32)
        Me.RadMode2.TabIndex = 1
        Me.RadMode2.Text = "個別チェック"
        '
        'KFJKOZA010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.RadMode1)
        Me.Controls.Add(Me.RadMode2)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PanKobetu)
        Me.Controls.Add(Me.PanIkkatu)
        Me.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJKOZA010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJKOZA010"
        Me.PanKobetu.ResumeLayout(False)
        Me.PanKobetu.PerformLayout()
        Me.PanIkkatu.ResumeLayout(False)
        Me.PanIkkatu.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents PanKobetu As System.Windows.Forms.Panel
    Friend WithEvents txtMotikomiSEQ As System.Windows.Forms.TextBox
    Friend WithEvents lblMotikomiSEQ As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents txtTorisCode As System.Windows.Forms.TextBox
    Friend WithEvents txtTorifCode As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents lblFuriDate As System.Windows.Forms.Label
    Friend WithEvents cmbSyubetu_KOBETU As System.Windows.Forms.ComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents PanIkkatu As System.Windows.Forms.Panel
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtTorokuDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtTorokuDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtTorokuDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents cmbSyubetu As System.Windows.Forms.ComboBox
    Friend WithEvents RadMode1 As System.Windows.Forms.RadioButton
    Friend WithEvents RadMode2 As System.Windows.Forms.RadioButton

End Class
