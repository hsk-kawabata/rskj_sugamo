<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAIN010
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
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
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
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents btnSerch As System.Windows.Forms.Button
    Friend WithEvents btnFind As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtGAKKOU_CODE As System.Windows.Forms.TextBox
    Friend WithEvents lblGAKKOU_NAME As System.Windows.Forms.Label
    Friend WithEvents cmbGAKKOUNAME As System.Windows.Forms.ComboBox
    Friend WithEvents txtSMonth As System.Windows.Forms.TextBox
    Friend WithEvents txtSYear As System.Windows.Forms.TextBox
    Friend WithEvents txtSeikyuCnt As System.Windows.Forms.TextBox
    Friend WithEvents lblSyukeiCnt As System.Windows.Forms.Label
    Friend WithEvents lblSyofuriCnt As System.Windows.Forms.Label
    Friend WithEvents lblSaifuriCnt As System.Windows.Forms.Label
    Friend WithEvents txtSeikyuKingaku As System.Windows.Forms.TextBox
    Friend WithEvents lblSyukeiKingaku As System.Windows.Forms.Label
    Friend WithEvents lblSyofuriKingaku As System.Windows.Forms.Label
    Friend WithEvents lblSaifuriKingaku As System.Windows.Forms.Label
    Friend WithEvents lblKekka As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAIN010))
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtFuriDateD = New System.Windows.Forms.TextBox
        Me.txtFuriDateM = New System.Windows.Forms.TextBox
        Me.txtFuriDateY = New System.Windows.Forms.TextBox
        Me.cmbGAKKOUNAME = New System.Windows.Forms.ComboBox
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.txtSMonth = New System.Windows.Forms.TextBox
        Me.txtSYear = New System.Windows.Forms.TextBox
        Me.btnSerch = New System.Windows.Forms.Button
        Me.btnFind = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label21 = New System.Windows.Forms.Label
        Me.txtSeikyuCnt = New System.Windows.Forms.TextBox
        Me.lblSyukeiCnt = New System.Windows.Forms.Label
        Me.lblSyofuriCnt = New System.Windows.Forms.Label
        Me.lblSaifuriCnt = New System.Windows.Forms.Label
        Me.txtSeikyuKingaku = New System.Windows.Forms.TextBox
        Me.lblSyukeiKingaku = New System.Windows.Forms.Label
        Me.lblSyofuriKingaku = New System.Windows.Forms.Label
        Me.lblSaifuriKingaku = New System.Windows.Forms.Label
        Me.Label28 = New System.Windows.Forms.Label
        Me.lblKekka = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtGAKKOU_CODE = New System.Windows.Forms.TextBox
        Me.lblGAKKOU_NAME = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(346, 211)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 27
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(286, 211)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 26
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(226, 211)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 25
        Me.Label8.Text = "年"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(81, 211)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(77, 16)
        Me.Label6.TabIndex = 17
        Me.Label6.Text = "振 替 日"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(81, 80)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "学校検索"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(672, 40)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 44
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(673, 9)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 42
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 40
        Me.Label3.Text = "＜口座振替金額確認＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(584, 40)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 43
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(598, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 41
        Me.Label1.Text = "ログイン名　:"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateD.Location = New System.Drawing.Point(316, 208)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 5
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateM.Location = New System.Drawing.Point(256, 208)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 4
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateY.Location = New System.Drawing.Point(176, 208)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 3
        '
        'cmbGAKKOUNAME
        '
        Me.cmbGAKKOUNAME.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGAKKOUNAME.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGAKKOUNAME.Location = New System.Drawing.Point(224, 77)
        Me.cmbGAKKOUNAME.Name = "cmbGAKKOUNAME"
        Me.cmbGAKKOUNAME.Size = New System.Drawing.Size(502, 21)
        Me.cmbGAKKOUNAME.TabIndex = 13
        Me.cmbGAKKOUNAME.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(176, 77)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 12
        Me.cmbKana.TabStop = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(286, 171)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(24, 16)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "月"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(226, 171)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(24, 16)
        Me.Label12.TabIndex = 23
        Me.Label12.Text = "年"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(81, 171)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(76, 16)
        Me.Label13.TabIndex = 16
        Me.Label13.Text = "請求対象"
        '
        'txtSMonth
        '
        Me.txtSMonth.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSMonth.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSMonth.Location = New System.Drawing.Point(256, 168)
        Me.txtSMonth.MaxLength = 2
        Me.txtSMonth.Name = "txtSMonth"
        Me.txtSMonth.Size = New System.Drawing.Size(24, 23)
        Me.txtSMonth.TabIndex = 2
        '
        'txtSYear
        '
        Me.txtSYear.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSYear.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSYear.Location = New System.Drawing.Point(176, 168)
        Me.txtSYear.MaxLength = 4
        Me.txtSYear.Name = "txtSYear"
        Me.txtSYear.Size = New System.Drawing.Size(44, 23)
        Me.txtSYear.TabIndex = 1
        '
        'btnSerch
        '
        Me.btnSerch.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSerch.Location = New System.Drawing.Point(10, 520)
        Me.btnSerch.Name = "btnSerch"
        Me.btnSerch.Size = New System.Drawing.Size(120, 40)
        Me.btnSerch.TabIndex = 8
        Me.btnSerch.Text = "確　認"
        '
        'btnFind
        '
        Me.btnFind.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnFind.Location = New System.Drawing.Point(140, 520)
        Me.btnFind.Name = "btnFind"
        Me.btnFind.Size = New System.Drawing.Size(120, 40)
        Me.btnFind.TabIndex = 9
        Me.btnFind.Text = "参　照"
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(270, 520)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(120, 40)
        Me.btnCancel.TabIndex = 10
        Me.btnCancel.Text = "取　消"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 11
        Me.btnEnd.Text = "終　了"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(81, 266)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(76, 16)
        Me.Label14.TabIndex = 18
        Me.Label14.Text = "請求件数"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(81, 307)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(72, 16)
        Me.Label15.TabIndex = 19
        Me.Label15.Text = "集計件数"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(81, 347)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(72, 16)
        Me.Label16.TabIndex = 20
        Me.Label16.Text = "初 振 分"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(81, 387)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(72, 16)
        Me.Label17.TabIndex = 21
        Me.Label17.Text = "再 振 分"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(310, 266)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(76, 16)
        Me.Label18.TabIndex = 28
        Me.Label18.Text = "請求金額"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(310, 307)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(72, 16)
        Me.Label19.TabIndex = 30
        Me.Label19.Text = "集計金額"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(310, 347)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(72, 16)
        Me.Label20.TabIndex = 35
        Me.Label20.Text = "初 振 分"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label21.Location = New System.Drawing.Point(310, 387)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(72, 16)
        Me.Label21.TabIndex = 38
        Me.Label21.Text = "再 振 分"
        '
        'txtSeikyuCnt
        '
        Me.txtSeikyuCnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSeikyuCnt.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSeikyuCnt.Location = New System.Drawing.Point(176, 263)
        Me.txtSeikyuCnt.MaxLength = 6
        Me.txtSeikyuCnt.Name = "txtSeikyuCnt"
        Me.txtSeikyuCnt.Size = New System.Drawing.Size(120, 23)
        Me.txtSeikyuCnt.TabIndex = 6
        Me.txtSeikyuCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblSyukeiCnt
        '
        Me.lblSyukeiCnt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSyukeiCnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSyukeiCnt.Location = New System.Drawing.Point(176, 304)
        Me.lblSyukeiCnt.Name = "lblSyukeiCnt"
        Me.lblSyukeiCnt.Size = New System.Drawing.Size(120, 24)
        Me.lblSyukeiCnt.TabIndex = 29
        Me.lblSyukeiCnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSyofuriCnt
        '
        Me.lblSyofuriCnt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSyofuriCnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSyofuriCnt.Location = New System.Drawing.Point(176, 344)
        Me.lblSyofuriCnt.Name = "lblSyofuriCnt"
        Me.lblSyofuriCnt.Size = New System.Drawing.Size(120, 24)
        Me.lblSyofuriCnt.TabIndex = 34
        Me.lblSyofuriCnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSaifuriCnt
        '
        Me.lblSaifuriCnt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSaifuriCnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSaifuriCnt.Location = New System.Drawing.Point(176, 384)
        Me.lblSaifuriCnt.Name = "lblSaifuriCnt"
        Me.lblSaifuriCnt.Size = New System.Drawing.Size(120, 24)
        Me.lblSaifuriCnt.TabIndex = 37
        Me.lblSaifuriCnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtSeikyuKingaku
        '
        Me.txtSeikyuKingaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSeikyuKingaku.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtSeikyuKingaku.Location = New System.Drawing.Point(392, 263)
        Me.txtSeikyuKingaku.MaxLength = 12
        Me.txtSeikyuKingaku.Name = "txtSeikyuKingaku"
        Me.txtSeikyuKingaku.Size = New System.Drawing.Size(176, 23)
        Me.txtSeikyuKingaku.TabIndex = 7
        Me.txtSeikyuKingaku.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'lblSyukeiKingaku
        '
        Me.lblSyukeiKingaku.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSyukeiKingaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSyukeiKingaku.Location = New System.Drawing.Point(392, 304)
        Me.lblSyukeiKingaku.Name = "lblSyukeiKingaku"
        Me.lblSyukeiKingaku.Size = New System.Drawing.Size(176, 24)
        Me.lblSyukeiKingaku.TabIndex = 31
        Me.lblSyukeiKingaku.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSyofuriKingaku
        '
        Me.lblSyofuriKingaku.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSyofuriKingaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSyofuriKingaku.Location = New System.Drawing.Point(392, 344)
        Me.lblSyofuriKingaku.Name = "lblSyofuriKingaku"
        Me.lblSyofuriKingaku.Size = New System.Drawing.Size(176, 24)
        Me.lblSyofuriKingaku.TabIndex = 36
        Me.lblSyofuriKingaku.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblSaifuriKingaku
        '
        Me.lblSaifuriKingaku.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSaifuriKingaku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblSaifuriKingaku.Location = New System.Drawing.Point(392, 384)
        Me.lblSaifuriKingaku.Name = "lblSaifuriKingaku"
        Me.lblSaifuriKingaku.Size = New System.Drawing.Size(176, 24)
        Me.lblSaifuriKingaku.TabIndex = 39
        Me.lblSaifuriKingaku.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label28
        '
        Me.Label28.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label28.Location = New System.Drawing.Point(576, 306)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(56, 23)
        Me.Label28.TabIndex = 32
        Me.Label28.Text = "結 果"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblKekka
        '
        Me.lblKekka.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblKekka.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKekka.Location = New System.Drawing.Point(632, 304)
        Me.lblKekka.Name = "lblKekka"
        Me.lblKekka.Size = New System.Drawing.Size(71, 24)
        Me.lblKekka.TabIndex = 33
        Me.lblKekka.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(81, 124)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 16)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "学校コード"
        '
        'txtGAKKOU_CODE
        '
        Me.txtGAKKOU_CODE.BackColor = System.Drawing.Color.White
        Me.txtGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKKOU_CODE.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKKOU_CODE.Location = New System.Drawing.Point(176, 121)
        Me.txtGAKKOU_CODE.MaxLength = 10
        Me.txtGAKKOU_CODE.Name = "txtGAKKOU_CODE"
        Me.txtGAKKOU_CODE.Size = New System.Drawing.Size(90, 23)
        Me.txtGAKKOU_CODE.TabIndex = 0
        '
        'lblGAKKOU_NAME
        '
        Me.lblGAKKOU_NAME.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblGAKKOU_NAME.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblGAKKOU_NAME.Location = New System.Drawing.Point(272, 121)
        Me.lblGAKKOU_NAME.Name = "lblGAKKOU_NAME"
        Me.lblGAKKOU_NAME.Size = New System.Drawing.Size(454, 24)
        Me.lblGAKKOU_NAME.TabIndex = 22
        Me.lblGAKKOU_NAME.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'KFGMAIN010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txtGAKKOU_CODE)
        Me.Controls.Add(Me.lblGAKKOU_NAME)
        Me.Controls.Add(Me.lblKekka)
        Me.Controls.Add(Me.Label28)
        Me.Controls.Add(Me.lblSaifuriKingaku)
        Me.Controls.Add(Me.lblSyofuriKingaku)
        Me.Controls.Add(Me.lblSyukeiKingaku)
        Me.Controls.Add(Me.txtSeikyuKingaku)
        Me.Controls.Add(Me.lblSaifuriCnt)
        Me.Controls.Add(Me.lblSyofuriCnt)
        Me.Controls.Add(Me.lblSyukeiCnt)
        Me.Controls.Add(Me.txtSeikyuCnt)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnFind)
        Me.Controls.Add(Me.btnSerch)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.txtSMonth)
        Me.Controls.Add(Me.txtSYear)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
        Me.Controls.Add(Me.cmbGAKKOUNAME)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGMAIN010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAIN010"
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
        AddHandler cmbGAKKOUNAME.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbGAKKOUNAME.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbGAKKOUNAME.LostFocus, AddressOf CAST.LostFocus

        '****************
        'txtGAKKOU_CODE
        '****************
        AddHandler txtGAKKOU_CODE.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtGAKKOU_CODE.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtGAKKOU_CODE.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtSYear
        '****************
        AddHandler txtSYear.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSYear.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSYear.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtSMonth
        '****************
        AddHandler txtSMonth.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtSMonth.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtSMonth.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtFuriDateY
        '****************
        AddHandler txtFuriDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFuriDateY.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtFuriDateM
        '****************
        AddHandler txtFuriDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFuriDateM.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtFuriDateD
        '****************
        AddHandler txtFuriDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPressNum
        AddHandler txtFuriDateD.LostFocus, AddressOf CASTx01.LostFocusZero

        '****************
        'txtSeikyuCnt
        '****************
        AddHandler txtSeikyuCnt.GotFocus, AddressOf CASTx01.GotFocusMoney
        AddHandler txtSeikyuCnt.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler txtSeikyuCnt.LostFocus, AddressOf CASTx01.LostFocusMoney

        '****************
        'txtSeikyuKingaku
        '****************
        AddHandler txtSeikyuKingaku.GotFocus, AddressOf CASTx01.GotFocusMoney
        AddHandler txtSeikyuKingaku.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler txtSeikyuKingaku.LostFocus, AddressOf CASTx01.LostFocusMoney
    End Sub

End Class
