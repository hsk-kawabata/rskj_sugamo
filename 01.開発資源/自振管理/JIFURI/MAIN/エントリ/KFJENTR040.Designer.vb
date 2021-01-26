<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJENTR040
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJENTR040))
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.txtKaisiPage = New System.Windows.Forms.TextBox
        Me.txtSyokiti = New System.Windows.Forms.TextBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.txtFuriDateD = New System.Windows.Forms.TextBox
        Me.txtFuriDateM = New System.Windows.Forms.TextBox
        Me.txtFuriDateY = New System.Windows.Forms.TextBox
        Me.txtTorifCode = New System.Windows.Forms.TextBox
        Me.txtTorisCode = New System.Windows.Forms.TextBox
        Me.cmbToriName = New System.Windows.Forms.ComboBox
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.rdbSei = New System.Windows.Forms.RadioButton
        Me.rdbFuku = New System.Windows.Forms.RadioButton
        Me.txtSitSyokiti = New System.Windows.Forms.TextBox
        Me.txtKinSyokiti = New System.Windows.Forms.TextBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.ckbSIYOU = New System.Windows.Forms.CheckBox
        Me.ckbSAKUJO = New System.Windows.Forms.CheckBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(295, 90)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 0
        Me.cmbKana.TabStop = False
        '
        'txtKaisiPage
        '
        Me.txtKaisiPage.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKaisiPage.Location = New System.Drawing.Point(295, 270)
        Me.txtKaisiPage.MaxLength = 4
        Me.txtKaisiPage.Name = "txtKaisiPage"
        Me.txtKaisiPage.Size = New System.Drawing.Size(67, 23)
        Me.txtKaisiPage.TabIndex = 8
        Me.txtKaisiPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtSyokiti
        '
        Me.txtSyokiti.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSyokiti.Location = New System.Drawing.Point(295, 225)
        Me.txtSyokiti.MaxLength = 10
        Me.txtSyokiti.Name = "txtSyokiti"
        Me.txtSyokiti.Size = New System.Drawing.Size(104, 23)
        Me.txtSyokiti.TabIndex = 7
        Me.txtSyokiti.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(142, 272)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(88, 16)
        Me.Label12.TabIndex = 87
        Me.Label12.Text = "開始ページ"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(140, 227)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(88, 16)
        Me.Label11.TabIndex = 85
        Me.Label11.Text = "初　期　値"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 16
        Me.btnEnd.Text = "終　了"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 15
        Me.btnAction.Text = "実　行"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(445, 184)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 84
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(393, 184)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 83
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(341, 184)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 82
        Me.Label8.Text = "年"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(387, 139)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 81
        Me.Label7.Text = "－"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(140, 182)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(83, 16)
        Me.Label6.TabIndex = 80
        Me.Label6.Text = "振　替　日"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(140, 137)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(101, 16)
        Me.Label14.TabIndex = 79
        Me.Label14.Text = "取引先コード"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(140, 92)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(88, 16)
        Me.Label15.TabIndex = 77
        Me.Label15.Text = "取引先検索"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.Location = New System.Drawing.Point(419, 180)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 6
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.Location = New System.Drawing.Point(367, 180)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 5
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.Location = New System.Drawing.Point(295, 180)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 4
        '
        'txtTorifCode
        '
        Me.txtTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorifCode.Location = New System.Drawing.Point(413, 135)
        Me.txtTorifCode.MaxLength = 2
        Me.txtTorifCode.Name = "txtTorifCode"
        Me.txtTorifCode.Size = New System.Drawing.Size(24, 23)
        Me.txtTorifCode.TabIndex = 3
        '
        'txtTorisCode
        '
        Me.txtTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorisCode.Location = New System.Drawing.Point(295, 135)
        Me.txtTorisCode.MaxLength = 10
        Me.txtTorisCode.Name = "txtTorisCode"
        Me.txtTorisCode.Size = New System.Drawing.Size(90, 23)
        Me.txtTorisCode.TabIndex = 2
        '
        'cmbToriName
        '
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbToriName.Location = New System.Drawing.Point(345, 90)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(355, 21)
        Me.cmbToriName.TabIndex = 1
        Me.cmbToriName.TabStop = False
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(686, 19)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 64
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 42)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 61
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(607, 42)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 12)
        Me.Label4.TabIndex = 62
        Me.Label4.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(621, 19)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 63
        Me.Label2.Text = "ログイン名　:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 60
        Me.Label1.Text = "＜伝票用データ入力＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'rdbSei
        '
        Me.rdbSei.AutoSize = True
        Me.rdbSei.Checked = True
        Me.rdbSei.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbSei.Location = New System.Drawing.Point(295, 450)
        Me.rdbSei.Name = "rdbSei"
        Me.rdbSei.Size = New System.Drawing.Size(42, 20)
        Me.rdbSei.TabIndex = 13
        Me.rdbSei.TabStop = True
        Me.rdbSei.Text = "正"
        Me.rdbSei.UseVisualStyleBackColor = True
        '
        'rdbFuku
        '
        Me.rdbFuku.AutoSize = True
        Me.rdbFuku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbFuku.Location = New System.Drawing.Point(357, 450)
        Me.rdbFuku.Name = "rdbFuku"
        Me.rdbFuku.Size = New System.Drawing.Size(42, 20)
        Me.rdbFuku.TabIndex = 14
        Me.rdbFuku.Text = "副"
        Me.rdbFuku.UseVisualStyleBackColor = True
        '
        'txtSitSyokiti
        '
        Me.txtSitSyokiti.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSitSyokiti.Location = New System.Drawing.Point(295, 360)
        Me.txtSitSyokiti.MaxLength = 3
        Me.txtSitSyokiti.Name = "txtSitSyokiti"
        Me.txtSitSyokiti.Size = New System.Drawing.Size(67, 23)
        Me.txtSitSyokiti.TabIndex = 10
        '
        'txtKinSyokiti
        '
        Me.txtKinSyokiti.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinSyokiti.Location = New System.Drawing.Point(295, 315)
        Me.txtKinSyokiti.MaxLength = 4
        Me.txtKinSyokiti.Name = "txtKinSyokiti"
        Me.txtKinSyokiti.Size = New System.Drawing.Size(67, 23)
        Me.txtKinSyokiti.TabIndex = 9
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(140, 363)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(120, 16)
        Me.Label13.TabIndex = 94
        Me.Label13.Text = "支店ｺｰﾄﾞ初期値"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(140, 319)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(152, 16)
        Me.Label16.TabIndex = 93
        Me.Label16.Text = "金融機関ｺｰﾄﾞ初期値"
        '
        'ckbSIYOU
        '
        Me.ckbSIYOU.AutoSize = True
        Me.ckbSIYOU.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ckbSIYOU.Location = New System.Drawing.Point(295, 405)
        Me.ckbSIYOU.Name = "ckbSIYOU"
        Me.ckbSIYOU.Size = New System.Drawing.Size(59, 20)
        Me.ckbSIYOU.TabIndex = 11
        Me.ckbSIYOU.Text = "使用"
        Me.ckbSIYOU.UseVisualStyleBackColor = True
        '
        'ckbSAKUJO
        '
        Me.ckbSAKUJO.AutoSize = True
        Me.ckbSAKUJO.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ckbSAKUJO.Location = New System.Drawing.Point(360, 405)
        Me.ckbSAKUJO.Name = "ckbSAKUJO"
        Me.ckbSAKUJO.Size = New System.Drawing.Size(59, 20)
        Me.ckbSAKUJO.TabIndex = 12
        Me.ckbSAKUJO.Text = "破棄"
        Me.ckbSAKUJO.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(140, 452)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(88, 16)
        Me.Label3.TabIndex = 95
        Me.Label3.Text = "入力データ"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(140, 405)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(88, 16)
        Me.Label5.TabIndex = 96
        Me.Label5.Text = "前回データ"
        '
        'KFJENTR040
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ckbSAKUJO)
        Me.Controls.Add(Me.ckbSIYOU)
        Me.Controls.Add(Me.txtSitSyokiti)
        Me.Controls.Add(Me.txtKinSyokiti)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.txtKaisiPage)
        Me.Controls.Add(Me.txtSyokiti)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
        Me.Controls.Add(Me.txtTorifCode)
        Me.Controls.Add(Me.txtTorisCode)
        Me.Controls.Add(Me.cmbToriName)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.rdbSei)
        Me.Controls.Add(Me.rdbFuku)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJENTR040"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJENTR040"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        'カナ検索コンボボックス
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus

        '取引先検索コンボボックス
        AddHandler cmbToriName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbToriName.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbToriName.LostFocus, AddressOf CAST.LostFocus

        '取引先主コード
        AddHandler txtTorisCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtTorisCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorisCode.LostFocus, AddressOf CAST.LostFocus

        '取引先副コード
        AddHandler txtTorifCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtTorifCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorifCode.LostFocus, AddressOf CAST.LostFocus

        '振替日(年)
        AddHandler txtFuriDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateY.LostFocus, AddressOf CAST.LostFocus

        '振替日(月)
        AddHandler txtFuriDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateM.LostFocus, AddressOf CAST.LostFocus

        '振替日(日)
        AddHandler txtFuriDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateD.LostFocus, AddressOf CAST.LostFocus

        '開始ページ
        AddHandler txtKaisiPage.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKaisiPage.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKaisiPage.LostFocus, AddressOf CAST.LostFocus

        '初期値
        AddHandler txtSyokiti.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler txtSyokiti.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSyokiti.LostFocus, AddressOf CAST.LostFocusMoney

        '金融機関コード初期値
        AddHandler txtKinSyokiti.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKinSyokiti.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKinSyokiti.LostFocus, AddressOf CAST.LostFocus

        '支店コード初期値
        AddHandler txtSitSyokiti.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtSitSyokiti.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtSitSyokiti.LostFocus, AddressOf CAST.LostFocus

        '前回データ使用
        AddHandler ckbSIYOU.GotFocus, AddressOf CAST.GotFocus
        AddHandler ckbSIYOU.KeyPress, AddressOf CAST.KeyPress
        AddHandler ckbSIYOU.LostFocus, AddressOf CAST.LostFocus

        '前回データ削除
        AddHandler ckbSAKUJO.GotFocus, AddressOf CAST.GotFocus
        AddHandler ckbSAKUJO.KeyPress, AddressOf CAST.KeyPress
        AddHandler ckbSAKUJO.LostFocus, AddressOf CAST.LostFocus

        '正
        AddHandler rdbSei.GotFocus, AddressOf CAST.GotFocus
        AddHandler rdbSei.KeyPress, AddressOf CAST.KeyPress
        AddHandler rdbSei.LostFocus, AddressOf CAST.LostFocus

        '副
        AddHandler rdbFuku.GotFocus, AddressOf CAST.GotFocus
        AddHandler rdbFuku.KeyPress, AddressOf CAST.KeyPress
        AddHandler rdbFuku.LostFocus, AddressOf CAST.LostFocus
    End Sub

    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents txtKaisiPage As System.Windows.Forms.TextBox
    Friend WithEvents txtSyokiti As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtTorifCode As System.Windows.Forms.TextBox
    Friend WithEvents txtTorisCode As System.Windows.Forms.TextBox
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents rdbSei As System.Windows.Forms.RadioButton
    Friend WithEvents rdbFuku As System.Windows.Forms.RadioButton
    Friend WithEvents txtSitSyokiti As System.Windows.Forms.TextBox
    Friend WithEvents txtKinSyokiti As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents ckbSIYOU As System.Windows.Forms.CheckBox
    Friend WithEvents ckbSAKUJO As System.Windows.Forms.CheckBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
End Class
