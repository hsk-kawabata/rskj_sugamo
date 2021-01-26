<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGPRNT030
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGPRNT030))
        Me.btnPrnt = New System.Windows.Forms.Button()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lab学校名 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtFuriDateD = New System.Windows.Forms.TextBox()
        Me.txtFuriDateM = New System.Windows.Forms.TextBox()
        Me.txtFuriDateY = New System.Windows.Forms.TextBox()
        Me.txtGAKKOU_CODE = New System.Windows.Forms.TextBox()
        Me.cmbGakkouName = New System.Windows.Forms.ComboBox()
        Me.cmbKana = New System.Windows.Forms.ComboBox()
        Me.chk振替不能明細 = New System.Windows.Forms.CheckBox()
        Me.chk振替未納のお知らせ = New System.Windows.Forms.CheckBox()
        Me.chk振替結果一覧 = New System.Windows.Forms.CheckBox()
        Me.chk収納報告書 = New System.Windows.Forms.CheckBox()
        Me.chk振替店別集計表 = New System.Windows.Forms.CheckBox()
        Me.chk入金伝票 = New System.Windows.Forms.CheckBox()
        Me.chk振替結果一覧合算出力 = New System.Windows.Forms.CheckBox()
        Me.chk振替不能明細合算出力 = New System.Windows.Forms.CheckBox()
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
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(100, 470)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(309, 19)
        Me.Label11.TabIndex = 99
        Me.Label11.Text = "※ALL9指定時のみ、全件処理する"
        '
        'lab学校名
        '
        Me.lab学校名.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校名.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校名.Location = New System.Drawing.Point(308, 152)
        Me.lab学校名.Name = "lab学校名"
        Me.lab学校名.Size = New System.Drawing.Size(423, 24)
        Me.lab学校名.TabIndex = 98
        Me.lab学校名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(382, 194)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 97
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(322, 195)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 96
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(262, 195)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 95
        Me.Label8.Text = "年"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(100, 195)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(77, 16)
        Me.Label6.TabIndex = 94
        Me.Label6.Text = "振 替 日"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(100, 155)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 16)
        Me.Label5.TabIndex = 93
        Me.Label5.Text = "学校コード"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(100, 115)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 92
        Me.Label4.Text = "学校検索"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 91
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 90
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 89
        Me.Label3.Text = "＜口座振替結果帳票印刷＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(592, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 88
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 87
        Me.Label1.Text = "ログイン名　:"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateD.Location = New System.Drawing.Point(352, 192)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 3
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateM.Location = New System.Drawing.Point(292, 191)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 2
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateY.Location = New System.Drawing.Point(212, 191)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 1
        '
        'txtGAKKOU_CODE
        '
        Me.txtGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKKOU_CODE.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKKOU_CODE.Location = New System.Drawing.Point(212, 152)
        Me.txtGAKKOU_CODE.MaxLength = 10
        Me.txtGAKKOU_CODE.Name = "txtGAKKOU_CODE"
        Me.txtGAKKOU_CODE.Size = New System.Drawing.Size(90, 23)
        Me.txtGAKKOU_CODE.TabIndex = 0
        '
        'cmbGakkouName
        '
        Me.cmbGakkouName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGakkouName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGakkouName.Location = New System.Drawing.Point(260, 112)
        Me.cmbGakkouName.Name = "cmbGakkouName"
        Me.cmbGakkouName.Size = New System.Drawing.Size(471, 21)
        Me.cmbGakkouName.TabIndex = 82
        Me.cmbGakkouName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(212, 112)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 81
        Me.cmbKana.TabStop = False
        '
        'chk振替不能明細
        '
        Me.chk振替不能明細.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk振替不能明細.Location = New System.Drawing.Point(260, 275)
        Me.chk振替不能明細.Name = "chk振替不能明細"
        Me.chk振替不能明細.Size = New System.Drawing.Size(216, 24)
        Me.chk振替不能明細.TabIndex = 102
        Me.chk振替不能明細.TabStop = False
        Me.chk振替不能明細.Text = "口座振替不能明細一覧表"
        '
        'chk振替未納のお知らせ
        '
        Me.chk振替未納のお知らせ.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk振替未納のお知らせ.Location = New System.Drawing.Point(260, 374)
        Me.chk振替未納のお知らせ.Name = "chk振替未納のお知らせ"
        Me.chk振替未納のお知らせ.Size = New System.Drawing.Size(232, 24)
        Me.chk振替未納のお知らせ.TabIndex = 103
        Me.chk振替未納のお知らせ.TabStop = False
        Me.chk振替未納のお知らせ.Text = "口座振替未納のお知らせ"
        '
        'chk振替結果一覧
        '
        Me.chk振替結果一覧.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk振替結果一覧.Location = New System.Drawing.Point(260, 242)
        Me.chk振替結果一覧.Name = "chk振替結果一覧"
        Me.chk振替結果一覧.Size = New System.Drawing.Size(224, 24)
        Me.chk振替結果一覧.TabIndex = 104
        Me.chk振替結果一覧.TabStop = False
        Me.chk振替結果一覧.Text = "口座振替結果一覧表"
        '
        'chk収納報告書
        '
        Me.chk収納報告書.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk収納報告書.Location = New System.Drawing.Point(260, 308)
        Me.chk収納報告書.Name = "chk収納報告書"
        Me.chk収納報告書.Size = New System.Drawing.Size(216, 24)
        Me.chk収納報告書.TabIndex = 105
        Me.chk収納報告書.TabStop = False
        Me.chk収納報告書.Text = "収納報告書"
        '
        'chk振替店別集計表
        '
        Me.chk振替店別集計表.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk振替店別集計表.Location = New System.Drawing.Point(260, 341)
        Me.chk振替店別集計表.Name = "chk振替店別集計表"
        Me.chk振替店別集計表.Size = New System.Drawing.Size(216, 24)
        Me.chk振替店別集計表.TabIndex = 106
        Me.chk振替店別集計表.TabStop = False
        Me.chk振替店別集計表.Text = "口座振替店別集計表"
        '
        'chk入金伝票
        '
        Me.chk入金伝票.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk入金伝票.Location = New System.Drawing.Point(260, 407)
        Me.chk入金伝票.Name = "chk入金伝票"
        Me.chk入金伝票.Size = New System.Drawing.Size(232, 24)
        Me.chk入金伝票.TabIndex = 107
        Me.chk入金伝票.TabStop = False
        Me.chk入金伝票.Text = "要求性預金入金伝票"
        '
        'chk振替結果一覧合算出力
        '
        Me.chk振替結果一覧合算出力.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk振替結果一覧合算出力.Location = New System.Drawing.Point(470, 242)
        Me.chk振替結果一覧合算出力.Name = "chk振替結果一覧合算出力"
        Me.chk振替結果一覧合算出力.Size = New System.Drawing.Size(128, 24)
        Me.chk振替結果一覧合算出力.TabIndex = 108
        Me.chk振替結果一覧合算出力.TabStop = False
        Me.chk振替結果一覧合算出力.Text = "合算出力"
        '
        'chk振替不能明細合算出力
        '
        Me.chk振替不能明細合算出力.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk振替不能明細合算出力.Location = New System.Drawing.Point(470, 275)
        Me.chk振替不能明細合算出力.Name = "chk振替不能明細合算出力"
        Me.chk振替不能明細合算出力.Size = New System.Drawing.Size(128, 24)
        Me.chk振替不能明細合算出力.TabIndex = 109
        Me.chk振替不能明細合算出力.TabStop = False
        Me.chk振替不能明細合算出力.Text = "合算出力"
        Me.chk振替不能明細合算出力.Visible = False
        '
        'KFGPRNT030
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.chk振替不能明細合算出力)
        Me.Controls.Add(Me.chk振替結果一覧合算出力)
        Me.Controls.Add(Me.chk入金伝票)
        Me.Controls.Add(Me.chk振替店別集計表)
        Me.Controls.Add(Me.chk収納報告書)
        Me.Controls.Add(Me.chk振替結果一覧)
        Me.Controls.Add(Me.chk振替未納のお知らせ)
        Me.Controls.Add(Me.chk振替不能明細)
        Me.Controls.Add(Me.btnPrnt)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.lab学校名)
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
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
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
        Me.Name = "KFGPRNT030"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGPRNT030"
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
        AddHandler txtFuriDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateY.GotFocus, AddressOf CASTx01.GotFocus

        '***************
        'KeyPress
        '***************
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbGakkouName.KeyPress, AddressOf CAST.KeyPress
        AddHandler txtGAKKOU_CODE.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPressNum

        '***************
        'LostFocus
        '***************
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbGakkouName.LostFocus, AddressOf CAST.LostFocus
        AddHandler txtGAKKOU_CODE.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtFuriDateD.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtFuriDateM.LostFocus, AddressOf CASTx01.LostFocusZero
        AddHandler txtFuriDateY.LostFocus, AddressOf CASTx01.LostFocusZero

    End Sub

    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
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
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents cmbGakkouName As System.Windows.Forms.ComboBox
    Friend WithEvents txtGAKKOU_CODE As System.Windows.Forms.TextBox
    Friend WithEvents lab学校名 As System.Windows.Forms.Label
    Friend WithEvents btnPrnt As System.Windows.Forms.Button
    Friend WithEvents chk振替不能明細 As System.Windows.Forms.CheckBox
    Friend WithEvents chk振替未納のお知らせ As System.Windows.Forms.CheckBox
    Friend WithEvents chk振替結果一覧 As System.Windows.Forms.CheckBox
    Friend WithEvents chk収納報告書 As System.Windows.Forms.CheckBox
    Friend WithEvents chk振替店別集計表 As System.Windows.Forms.CheckBox
    Friend WithEvents chk入金伝票 As System.Windows.Forms.CheckBox
    Friend WithEvents chk振替結果一覧合算出力 As System.Windows.Forms.CheckBox
    Friend WithEvents chk振替不能明細合算出力 As System.Windows.Forms.CheckBox

End Class
