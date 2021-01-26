<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGPRNT140
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGPRNT140))
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.cmbKbn = New System.Windows.Forms.ComboBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.txtGAKKOU_CODE = New System.Windows.Forms.TextBox
        Me.cmbGakkouName = New System.Windows.Forms.ComboBox
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblGAKKOU_NAME = New System.Windows.Forms.Label
        Me.chk対象_全学年 = New System.Windows.Forms.CheckBox
        Me.Label106 = New System.Windows.Forms.Label
        Me.Label71 = New System.Windows.Forms.Label
        Me.Label72 = New System.Windows.Forms.Label
        Me.Label73 = New System.Windows.Forms.Label
        Me.Label74 = New System.Windows.Forms.Label
        Me.Label75 = New System.Windows.Forms.Label
        Me.Label76 = New System.Windows.Forms.Label
        Me.Label77 = New System.Windows.Forms.Label
        Me.Label78 = New System.Windows.Forms.Label
        Me.Label79 = New System.Windows.Forms.Label
        Me.Label80 = New System.Windows.Forms.Label
        Me.chk対象_６学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_５学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_４学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_９学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_３学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_８学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_２学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_７学年 = New System.Windows.Forms.CheckBox
        Me.chk対象_１学年 = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(210, 120)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 20
        Me.cmbKana.TabStop = False
        '
        'cmbKbn
        '
        Me.cmbKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKbn.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKbn.Location = New System.Drawing.Point(210, 220)
        Me.cmbKbn.Name = "cmbKbn"
        Me.cmbKbn.Size = New System.Drawing.Size(94, 24)
        Me.cmbKbn.TabIndex = 7
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(101, 223)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(93, 16)
        Me.Label11.TabIndex = 68
        Me.Label11.Text = "入出金区分"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(101, 173)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(93, 16)
        Me.Label13.TabIndex = 62
        Me.Label13.Text = "学校コード"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(101, 120)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(72, 16)
        Me.Label14.TabIndex = 61
        Me.Label14.Text = "学校検索"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtGAKKOU_CODE
        '
        Me.txtGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKKOU_CODE.Location = New System.Drawing.Point(210, 170)
        Me.txtGAKKOU_CODE.MaxLength = 10
        Me.txtGAKKOU_CODE.Name = "txtGAKKOU_CODE"
        Me.txtGAKKOU_CODE.Size = New System.Drawing.Size(87, 23)
        Me.txtGAKKOU_CODE.TabIndex = 2
        '
        'cmbGakkouName
        '
        Me.cmbGakkouName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGakkouName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGakkouName.Location = New System.Drawing.Point(260, 120)
        Me.cmbGakkouName.Name = "cmbGakkouName"
        Me.cmbGakkouName.Size = New System.Drawing.Size(464, 21)
        Me.cmbGakkouName.TabIndex = 21
        Me.cmbGakkouName.TabStop = False
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 19
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(534, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 18
        Me.btnAction.Text = "印　刷"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 51
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 47)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 48
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(601, 47)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 12)
        Me.Label4.TabIndex = 49
        Me.Label4.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(615, 24)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 50
        Me.Label2.Text = "ログイン名　:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 47
        Me.Label1.Text = "＜口座振替依頼書印刷＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblGAKKOU_NAME
        '
        Me.lblGAKKOU_NAME.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblGAKKOU_NAME.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblGAKKOU_NAME.Location = New System.Drawing.Point(308, 169)
        Me.lblGAKKOU_NAME.Name = "lblGAKKOU_NAME"
        Me.lblGAKKOU_NAME.Size = New System.Drawing.Size(423, 24)
        Me.lblGAKKOU_NAME.TabIndex = 99
        Me.lblGAKKOU_NAME.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'chk対象_全学年
        '
        Me.chk対象_全学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_全学年.Location = New System.Drawing.Point(501, 288)
        Me.chk対象_全学年.Name = "chk対象_全学年"
        Me.chk対象_全学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_全学年.TabIndex = 17
        Me.chk対象_全学年.TabStop = False
        '
        'Label106
        '
        Me.Label106.AutoSize = True
        Me.Label106.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label106.Location = New System.Drawing.Point(496, 268)
        Me.Label106.Name = "Label106"
        Me.Label106.Size = New System.Drawing.Size(24, 16)
        Me.Label106.TabIndex = 966
        Me.Label106.Text = "全"
        '
        'Label71
        '
        Me.Label71.AutoSize = True
        Me.Label71.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label71.Location = New System.Drawing.Point(101, 288)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(144, 16)
        Me.Label71.TabIndex = 965
        Me.Label71.Text = "処理対象学年指定"
        Me.Label71.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label72
        '
        Me.Label72.AutoSize = True
        Me.Label72.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label72.Location = New System.Drawing.Point(466, 268)
        Me.Label72.Name = "Label72"
        Me.Label72.Size = New System.Drawing.Size(16, 16)
        Me.Label72.TabIndex = 964
        Me.Label72.Text = "9"
        '
        'Label73
        '
        Me.Label73.AutoSize = True
        Me.Label73.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label73.Location = New System.Drawing.Point(441, 268)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(16, 16)
        Me.Label73.TabIndex = 963
        Me.Label73.Text = "8"
        '
        'Label74
        '
        Me.Label74.AutoSize = True
        Me.Label74.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label74.Location = New System.Drawing.Point(416, 268)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(16, 16)
        Me.Label74.TabIndex = 962
        Me.Label74.Text = "7"
        '
        'Label75
        '
        Me.Label75.AutoSize = True
        Me.Label75.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label75.Location = New System.Drawing.Point(391, 268)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(16, 16)
        Me.Label75.TabIndex = 961
        Me.Label75.Text = "6"
        '
        'Label76
        '
        Me.Label76.AutoSize = True
        Me.Label76.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label76.Location = New System.Drawing.Point(366, 268)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(16, 16)
        Me.Label76.TabIndex = 960
        Me.Label76.Text = "5"
        '
        'Label77
        '
        Me.Label77.AutoSize = True
        Me.Label77.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label77.Location = New System.Drawing.Point(341, 268)
        Me.Label77.Name = "Label77"
        Me.Label77.Size = New System.Drawing.Size(16, 16)
        Me.Label77.TabIndex = 959
        Me.Label77.Text = "4"
        '
        'Label78
        '
        Me.Label78.AutoSize = True
        Me.Label78.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label78.Location = New System.Drawing.Point(316, 268)
        Me.Label78.Name = "Label78"
        Me.Label78.Size = New System.Drawing.Size(16, 16)
        Me.Label78.TabIndex = 958
        Me.Label78.Text = "3"
        '
        'Label79
        '
        Me.Label79.AutoSize = True
        Me.Label79.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label79.Location = New System.Drawing.Point(291, 268)
        Me.Label79.Name = "Label79"
        Me.Label79.Size = New System.Drawing.Size(16, 16)
        Me.Label79.TabIndex = 957
        Me.Label79.Text = "2"
        '
        'Label80
        '
        Me.Label80.AutoSize = True
        Me.Label80.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label80.Location = New System.Drawing.Point(266, 268)
        Me.Label80.Name = "Label80"
        Me.Label80.Size = New System.Drawing.Size(16, 16)
        Me.Label80.TabIndex = 956
        Me.Label80.Text = "1"
        '
        'chk対象_６学年
        '
        Me.chk対象_６学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_６学年.Location = New System.Drawing.Point(391, 288)
        Me.chk対象_６学年.Name = "chk対象_６学年"
        Me.chk対象_６学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_６学年.TabIndex = 13
        Me.chk対象_６学年.TabStop = False
        '
        'chk対象_５学年
        '
        Me.chk対象_５学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_５学年.Location = New System.Drawing.Point(366, 288)
        Me.chk対象_５学年.Name = "chk対象_５学年"
        Me.chk対象_５学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_５学年.TabIndex = 12
        Me.chk対象_５学年.TabStop = False
        '
        'chk対象_４学年
        '
        Me.chk対象_４学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_４学年.Location = New System.Drawing.Point(341, 288)
        Me.chk対象_４学年.Name = "chk対象_４学年"
        Me.chk対象_４学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_４学年.TabIndex = 11
        Me.chk対象_４学年.TabStop = False
        '
        'chk対象_９学年
        '
        Me.chk対象_９学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_９学年.Location = New System.Drawing.Point(466, 288)
        Me.chk対象_９学年.Name = "chk対象_９学年"
        Me.chk対象_９学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_９学年.TabIndex = 16
        Me.chk対象_９学年.TabStop = False
        '
        'chk対象_３学年
        '
        Me.chk対象_３学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_３学年.Location = New System.Drawing.Point(316, 288)
        Me.chk対象_３学年.Name = "chk対象_３学年"
        Me.chk対象_３学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_３学年.TabIndex = 10
        Me.chk対象_３学年.TabStop = False
        '
        'chk対象_８学年
        '
        Me.chk対象_８学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_８学年.Location = New System.Drawing.Point(441, 288)
        Me.chk対象_８学年.Name = "chk対象_８学年"
        Me.chk対象_８学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_８学年.TabIndex = 15
        Me.chk対象_８学年.TabStop = False
        '
        'chk対象_２学年
        '
        Me.chk対象_２学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_２学年.Location = New System.Drawing.Point(291, 288)
        Me.chk対象_２学年.Name = "chk対象_２学年"
        Me.chk対象_２学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_２学年.TabIndex = 9
        Me.chk対象_２学年.TabStop = False
        '
        'chk対象_７学年
        '
        Me.chk対象_７学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_７学年.Location = New System.Drawing.Point(416, 288)
        Me.chk対象_７学年.Name = "chk対象_７学年"
        Me.chk対象_７学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_７学年.TabIndex = 14
        Me.chk対象_７学年.TabStop = False
        '
        'chk対象_１学年
        '
        Me.chk対象_１学年.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chk対象_１学年.Location = New System.Drawing.Point(266, 288)
        Me.chk対象_１学年.Name = "chk対象_１学年"
        Me.chk対象_１学年.Size = New System.Drawing.Size(16, 24)
        Me.chk対象_１学年.TabIndex = 8
        Me.chk対象_１学年.TabStop = False
        '
        'KFGPRNT140
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.chk対象_全学年)
        Me.Controls.Add(Me.Label106)
        Me.Controls.Add(Me.Label71)
        Me.Controls.Add(Me.Label72)
        Me.Controls.Add(Me.Label73)
        Me.Controls.Add(Me.Label74)
        Me.Controls.Add(Me.Label75)
        Me.Controls.Add(Me.Label76)
        Me.Controls.Add(Me.Label77)
        Me.Controls.Add(Me.Label78)
        Me.Controls.Add(Me.Label79)
        Me.Controls.Add(Me.Label80)
        Me.Controls.Add(Me.chk対象_６学年)
        Me.Controls.Add(Me.chk対象_５学年)
        Me.Controls.Add(Me.chk対象_４学年)
        Me.Controls.Add(Me.chk対象_９学年)
        Me.Controls.Add(Me.chk対象_３学年)
        Me.Controls.Add(Me.chk対象_８学年)
        Me.Controls.Add(Me.chk対象_２学年)
        Me.Controls.Add(Me.chk対象_７学年)
        Me.Controls.Add(Me.chk対象_１学年)
        Me.Controls.Add(Me.lblGAKKOU_NAME)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.cmbKbn)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.txtGAKKOU_CODE)
        Me.Controls.Add(Me.cmbGakkouName)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGPRNT140"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGPRNT140"
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
        AddHandler cmbGakkouName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbGakkouName.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbGakkouName.LostFocus, AddressOf CAST.LostFocus

        '取引先主コード
        AddHandler txtGAKKOU_CODE.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtGAKKOU_CODE.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtGAKKOU_CODE.LostFocus, AddressOf CAST.LostFocus

        '出力区分コンボボックス
        AddHandler cmbKbn.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKbn.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKbn.LostFocus, AddressOf CAST.LostFocus
    End Sub
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents cmbKbn As System.Windows.Forms.ComboBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtGAKKOU_CODE As System.Windows.Forms.TextBox
    Friend WithEvents cmbGakkouName As System.Windows.Forms.ComboBox
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblGAKKOU_NAME As System.Windows.Forms.Label
    Friend WithEvents chk対象_全学年 As System.Windows.Forms.CheckBox
    Friend WithEvents Label106 As System.Windows.Forms.Label
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents chk対象_６学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_５学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_４学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_９学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_３学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_８学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_２学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_７学年 As System.Windows.Forms.CheckBox
    Friend WithEvents chk対象_１学年 As System.Windows.Forms.CheckBox
End Class
