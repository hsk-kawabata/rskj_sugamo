<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJOTHR020
    Inherits System.Windows.Forms.Form

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
        AddHandler TORIS_CODE_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler TORIS_CODE_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TORIS_CODE_K.LostFocus, AddressOf CASTx01.LostFocus

        '取引先副コード
        AddHandler TORIF_CODE_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler TORIF_CODE_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TORIF_CODE_K.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(年)
        AddHandler FURI_DATE_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler FURI_DATE_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_K.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(月)
        AddHandler FURI_DATE_K1.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler FURI_DATE_K1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_K1.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(日)
        AddHandler FURI_DATE_K2.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler FURI_DATE_K2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_K2.LostFocus, AddressOf CASTx01.LostFocus

        '企業シーケンス
        AddHandler KIGYO_SEQ_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler KIGYO_SEQ_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KIGYO_SEQ_K.LostFocus, AddressOf CASTx01.LostFocus

        '金融機関コード
        AddHandler KEIYAKU_KIN_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler KEIYAKU_KIN_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KEIYAKU_KIN_K.LostFocus, AddressOf CASTx01.LostFocus

        '支店コード
        AddHandler KEIYAKU_SIT_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler KEIYAKU_SIT_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KEIYAKU_SIT_K.LostFocus, AddressOf CASTx01.LostFocus

        '口座番号
        AddHandler KEIYAKU_KOUZA_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler KEIYAKU_KOUZA_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KEIYAKU_KOUZA_K.LostFocus, AddressOf CASTx01.LostFocus

        '金額
        AddHandler FURIKIN_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler FURIKIN_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURIKIN_K.LostFocus, AddressOf CASTx01.LostFocus

        '契約者名
        AddHandler KEIYAKU_KNAME_K.GotFocus, AddressOf CASTx2.GotFocus
        AddHandler KEIYAKU_KNAME_K.KeyPress, AddressOf CASTx2.KeyPress
        AddHandler KEIYAKU_KNAME_K.LostFocus, AddressOf CASTx2.LostFocus

        'レコード番号
        AddHandler RECORD_NO_K.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler RECORD_NO_K.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler RECORD_NO_K.LostFocus, AddressOf CASTx01.LostFocus

        '科目
        AddHandler KEIYAKU_KAMOKU_K.GotFocus, AddressOf CAST.GotFocus
        AddHandler KEIYAKU_KAMOKU_K.KeyPress, AddressOf CAST.KeyPress
        AddHandler KEIYAKU_KAMOKU_K.LostFocus, AddressOf CAST.LostFocus

        '振替結果コード
        AddHandler FURIKETU_CODE_K.GotFocus, AddressOf CAST.GotFocus
        AddHandler FURIKETU_CODE_K.KeyPress, AddressOf CAST.KeyPress
        AddHandler FURIKETU_CODE_K.LostFocus, AddressOf CAST.LostFocus

        '企業シーケンス
        AddHandler rdbKigyoSeq.GotFocus, AddressOf CAST.GotFocus
        AddHandler rdbKigyoSeq.KeyPress, AddressOf CAST.KeyPress
        AddHandler rdbKigyoSeq.LostFocus, AddressOf CAST.LostFocus

        '口座番号指定
        AddHandler rdbKouza.GotFocus, AddressOf CAST.GotFocus
        AddHandler rdbKouza.KeyPress, AddressOf CAST.KeyPress
        AddHandler rdbKouza.LostFocus, AddressOf CAST.LostFocus
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJOTHR020))
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblTITLE = New System.Windows.Forms.Label
        Me.KIGYO_SEQ_K = New System.Windows.Forms.TextBox
        Me.btnEraser = New System.Windows.Forms.Button
        Me.btnSansyou = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label19 = New System.Windows.Forms.Label
        Me.RECORD_NO_K = New System.Windows.Forms.TextBox
        Me.KEIYAKU_KNAME_K = New System.Windows.Forms.TextBox
        Me.FURIKIN_K = New System.Windows.Forms.TextBox
        Me.KEIYAKU_KOUZA_K = New System.Windows.Forms.TextBox
        Me.KEIYAKU_SIT_K = New System.Windows.Forms.TextBox
        Me.KEIYAKU_KIN_K = New System.Windows.Forms.TextBox
        Me.Label18 = New System.Windows.Forms.Label
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.KEIYAKU_KAMOKU_K = New System.Windows.Forms.ComboBox
        Me.FURIKETU_CODE_K = New System.Windows.Forms.ComboBox
        Me.rdbKouza = New System.Windows.Forms.RadioButton
        Me.rdbKigyoSeq = New System.Windows.Forms.RadioButton
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.Label20 = New System.Windows.Forms.Label
        Me.FURI_DATE_K2 = New System.Windows.Forms.TextBox
        Me.FURI_DATE_K1 = New System.Windows.Forms.TextBox
        Me.FURI_DATE_K = New System.Windows.Forms.TextBox
        Me.TORIF_CODE_K = New System.Windows.Forms.TextBox
        Me.TORIS_CODE_K = New System.Windows.Forms.TextBox
        Me.cmbToriName = New System.Windows.Forms.ComboBox
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Location = New System.Drawing.Point(673, 20)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 145
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Location = New System.Drawing.Point(673, 43)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 142
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(594, 43)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 143
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(608, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 144
        Me.Label1.Text = "ログイン名　:"
        '
        'lblTITLE
        '
        Me.lblTITLE.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTITLE.Location = New System.Drawing.Point(0, 8)
        Me.lblTITLE.Name = "lblTITLE"
        Me.lblTITLE.Size = New System.Drawing.Size(795, 30)
        Me.lblTITLE.TabIndex = 141
        Me.lblTITLE.Text = "＜振替結果変更＞"
        Me.lblTITLE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'KIGYO_SEQ_K
        '
        Me.KIGYO_SEQ_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIGYO_SEQ_K.Location = New System.Drawing.Point(337, 234)
        Me.KIGYO_SEQ_K.MaxLength = 8
        Me.KIGYO_SEQ_K.Name = "KIGYO_SEQ_K"
        Me.KIGYO_SEQ_K.Size = New System.Drawing.Size(81, 23)
        Me.KIGYO_SEQ_K.TabIndex = 8
        '
        'btnEraser
        '
        Me.btnEraser.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEraser.Location = New System.Drawing.Point(530, 520)
        Me.btnEraser.Name = "btnEraser"
        Me.btnEraser.Size = New System.Drawing.Size(120, 40)
        Me.btnEraser.TabIndex = 11
        Me.btnEraser.Text = "取　消"
        '
        'btnSansyou
        '
        Me.btnSansyou.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSansyou.Location = New System.Drawing.Point(270, 520)
        Me.btnSansyou.Name = "btnSansyou"
        Me.btnSansyou.Size = New System.Drawing.Size(120, 40)
        Me.btnSansyou.TabIndex = 10
        Me.btnSansyou.Text = "参　照"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label19)
        Me.GroupBox1.Controls.Add(Me.RECORD_NO_K)
        Me.GroupBox1.Controls.Add(Me.KEIYAKU_KNAME_K)
        Me.GroupBox1.Controls.Add(Me.FURIKIN_K)
        Me.GroupBox1.Controls.Add(Me.KEIYAKU_KOUZA_K)
        Me.GroupBox1.Controls.Add(Me.KEIYAKU_SIT_K)
        Me.GroupBox1.Controls.Add(Me.KEIYAKU_KIN_K)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.Label17)
        Me.GroupBox1.Controls.Add(Me.Label15)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.KEIYAKU_KAMOKU_K)
        Me.GroupBox1.Controls.Add(Me.FURIKETU_CODE_K)
        Me.GroupBox1.Location = New System.Drawing.Point(131, 269)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(536, 224)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        '
        'Label19
        '
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(320, 25)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(88, 16)
        Me.Label19.TabIndex = 22
        Me.Label19.Text = "支店コード"
        '
        'RECORD_NO_K
        '
        Me.RECORD_NO_K.Enabled = False
        Me.RECORD_NO_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.RECORD_NO_K.Location = New System.Drawing.Point(146, 182)
        Me.RECORD_NO_K.MaxLength = 5
        Me.RECORD_NO_K.Name = "RECORD_NO_K"
        Me.RECORD_NO_K.Size = New System.Drawing.Size(56, 23)
        Me.RECORD_NO_K.TabIndex = 7
        '
        'KEIYAKU_KNAME_K
        '
        Me.KEIYAKU_KNAME_K.Enabled = False
        Me.KEIYAKU_KNAME_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KEIYAKU_KNAME_K.Location = New System.Drawing.Point(146, 142)
        Me.KEIYAKU_KNAME_K.MaxLength = 15
        Me.KEIYAKU_KNAME_K.Name = "KEIYAKU_KNAME_K"
        Me.KEIYAKU_KNAME_K.Size = New System.Drawing.Size(168, 23)
        Me.KEIYAKU_KNAME_K.TabIndex = 6
        '
        'FURIKIN_K
        '
        Me.FURIKIN_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURIKIN_K.Location = New System.Drawing.Point(146, 102)
        Me.FURIKIN_K.MaxLength = 13
        Me.FURIKIN_K.Name = "FURIKIN_K"
        Me.FURIKIN_K.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.FURIKIN_K.Size = New System.Drawing.Size(168, 23)
        Me.FURIKIN_K.TabIndex = 5
        '
        'KEIYAKU_KOUZA_K
        '
        Me.KEIYAKU_KOUZA_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KEIYAKU_KOUZA_K.Location = New System.Drawing.Point(410, 62)
        Me.KEIYAKU_KOUZA_K.MaxLength = 7
        Me.KEIYAKU_KOUZA_K.Name = "KEIYAKU_KOUZA_K"
        Me.KEIYAKU_KOUZA_K.Size = New System.Drawing.Size(72, 23)
        Me.KEIYAKU_KOUZA_K.TabIndex = 4
        '
        'KEIYAKU_SIT_K
        '
        Me.KEIYAKU_SIT_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KEIYAKU_SIT_K.Location = New System.Drawing.Point(410, 22)
        Me.KEIYAKU_SIT_K.MaxLength = 3
        Me.KEIYAKU_SIT_K.Name = "KEIYAKU_SIT_K"
        Me.KEIYAKU_SIT_K.Size = New System.Drawing.Size(32, 23)
        Me.KEIYAKU_SIT_K.TabIndex = 2
        '
        'KEIYAKU_KIN_K
        '
        Me.KEIYAKU_KIN_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KEIYAKU_KIN_K.Location = New System.Drawing.Point(146, 22)
        Me.KEIYAKU_KIN_K.MaxLength = 4
        Me.KEIYAKU_KIN_K.Name = "KEIYAKU_KIN_K"
        Me.KEIYAKU_KIN_K.Size = New System.Drawing.Size(44, 23)
        Me.KEIYAKU_KIN_K.TabIndex = 1
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(253, 186)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(120, 16)
        Me.Label18.TabIndex = 21
        Me.Label18.Text = "振替結果コード"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(320, 64)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(72, 16)
        Me.Label17.TabIndex = 20
        Me.Label17.Text = "口座番号"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(24, 186)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(104, 16)
        Me.Label15.TabIndex = 18
        Me.Label15.Text = "レコード番号"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(24, 145)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(72, 16)
        Me.Label14.TabIndex = 17
        Me.Label14.Text = "契約者名"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(24, 105)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(72, 16)
        Me.Label13.TabIndex = 16
        Me.Label13.Text = "振替金額"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(24, 66)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(72, 16)
        Me.Label12.TabIndex = 15
        Me.Label12.Text = "科　　目"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(24, 25)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(120, 16)
        Me.Label11.TabIndex = 14
        Me.Label11.Text = "金融機関コード"
        '
        'KEIYAKU_KAMOKU_K
        '
        Me.KEIYAKU_KAMOKU_K.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.KEIYAKU_KAMOKU_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KEIYAKU_KAMOKU_K.Location = New System.Drawing.Point(146, 62)
        Me.KEIYAKU_KAMOKU_K.Name = "KEIYAKU_KAMOKU_K"
        Me.KEIYAKU_KAMOKU_K.Size = New System.Drawing.Size(112, 24)
        Me.KEIYAKU_KAMOKU_K.TabIndex = 3
        '
        'FURIKETU_CODE_K
        '
        Me.FURIKETU_CODE_K.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.FURIKETU_CODE_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURIKETU_CODE_K.Location = New System.Drawing.Point(375, 182)
        Me.FURIKETU_CODE_K.Name = "FURIKETU_CODE_K"
        Me.FURIKETU_CODE_K.Size = New System.Drawing.Size(144, 24)
        Me.FURIKETU_CODE_K.TabIndex = 8
        '
        'rdbKouza
        '
        Me.rdbKouza.Checked = True
        Me.rdbKouza.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbKouza.Location = New System.Drawing.Point(468, 229)
        Me.rdbKouza.Name = "rdbKouza"
        Me.rdbKouza.Size = New System.Drawing.Size(140, 32)
        Me.rdbKouza.TabIndex = 7
        Me.rdbKouza.TabStop = True
        Me.rdbKouza.Text = "口座番号指定"
        '
        'rdbKigyoSeq
        '
        Me.rdbKigyoSeq.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbKigyoSeq.Location = New System.Drawing.Point(191, 229)
        Me.rdbKigyoSeq.Name = "rdbKigyoSeq"
        Me.rdbKigyoSeq.Size = New System.Drawing.Size(146, 32)
        Me.rdbKigyoSeq.TabIndex = 6
        Me.rdbKigyoSeq.TabStop = True
        Me.rdbKigyoSeq.Text = "企業シーケンス"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 12
        Me.btnEnd.Text = "終　了"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 9
        Me.btnAction.Text = "更　新"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(439, 185)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 166
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(387, 185)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 165
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(335, 185)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 164
        Me.Label8.Text = "年"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(389, 137)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 162
        Me.Label7.Text = "－"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(177, 185)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 16)
        Me.Label6.TabIndex = 158
        Me.Label6.Text = "振　替　日"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(177, 137)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(104, 16)
        Me.Label16.TabIndex = 157
        Me.Label16.Text = "取引先コード"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(177, 88)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(88, 16)
        Me.Label20.TabIndex = 156
        Me.Label20.Text = "取引先検索"
        '
        'FURI_DATE_K2
        '
        Me.FURI_DATE_K2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_K2.Location = New System.Drawing.Point(413, 181)
        Me.FURI_DATE_K2.MaxLength = 2
        Me.FURI_DATE_K2.Name = "FURI_DATE_K2"
        Me.FURI_DATE_K2.Size = New System.Drawing.Size(24, 23)
        Me.FURI_DATE_K2.TabIndex = 5
        '
        'FURI_DATE_K1
        '
        Me.FURI_DATE_K1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_K1.Location = New System.Drawing.Point(361, 181)
        Me.FURI_DATE_K1.MaxLength = 2
        Me.FURI_DATE_K1.Name = "FURI_DATE_K1"
        Me.FURI_DATE_K1.Size = New System.Drawing.Size(24, 23)
        Me.FURI_DATE_K1.TabIndex = 4
        '
        'FURI_DATE_K
        '
        Me.FURI_DATE_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_K.Location = New System.Drawing.Point(289, 181)
        Me.FURI_DATE_K.MaxLength = 4
        Me.FURI_DATE_K.Name = "FURI_DATE_K"
        Me.FURI_DATE_K.Size = New System.Drawing.Size(44, 23)
        Me.FURI_DATE_K.TabIndex = 3
        '
        'TORIF_CODE_K
        '
        Me.TORIF_CODE_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TORIF_CODE_K.Location = New System.Drawing.Point(415, 133)
        Me.TORIF_CODE_K.MaxLength = 2
        Me.TORIF_CODE_K.Name = "TORIF_CODE_K"
        Me.TORIF_CODE_K.Size = New System.Drawing.Size(24, 23)
        Me.TORIF_CODE_K.TabIndex = 2
        '
        'TORIS_CODE_K
        '
        Me.TORIS_CODE_K.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TORIS_CODE_K.Location = New System.Drawing.Point(289, 133)
        Me.TORIS_CODE_K.MaxLength = 10
        Me.TORIS_CODE_K.Name = "TORIS_CODE_K"
        Me.TORIS_CODE_K.Size = New System.Drawing.Size(98, 23)
        Me.TORIS_CODE_K.TabIndex = 1
        '
        'cmbToriName
        '
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbToriName.Location = New System.Drawing.Point(340, 85)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(350, 21)
        Me.cmbToriName.TabIndex = 147
        Me.cmbToriName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(289, 85)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 146
        Me.cmbKana.TabStop = False
        '
        'KFJOTHR020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.KIGYO_SEQ_K)
        Me.Controls.Add(Me.btnEraser)
        Me.Controls.Add(Me.btnSansyou)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.rdbKouza)
        Me.Controls.Add(Me.rdbKigyoSeq)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.FURI_DATE_K2)
        Me.Controls.Add(Me.FURI_DATE_K1)
        Me.Controls.Add(Me.FURI_DATE_K)
        Me.Controls.Add(Me.TORIF_CODE_K)
        Me.Controls.Add(Me.TORIS_CODE_K)
        Me.Controls.Add(Me.cmbToriName)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTITLE)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJOTHR020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJOTHR020"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblTITLE As System.Windows.Forms.Label
    Friend WithEvents KIGYO_SEQ_K As System.Windows.Forms.TextBox
    Friend WithEvents btnEraser As System.Windows.Forms.Button
    Friend WithEvents btnSansyou As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents RECORD_NO_K As System.Windows.Forms.TextBox
    Friend WithEvents KEIYAKU_KNAME_K As System.Windows.Forms.TextBox
    Friend WithEvents FURIKIN_K As System.Windows.Forms.TextBox
    Friend WithEvents KEIYAKU_KOUZA_K As System.Windows.Forms.TextBox
    Friend WithEvents KEIYAKU_SIT_K As System.Windows.Forms.TextBox
    Friend WithEvents KEIYAKU_KIN_K As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents KEIYAKU_KAMOKU_K As System.Windows.Forms.ComboBox
    Friend WithEvents FURIKETU_CODE_K As System.Windows.Forms.ComboBox
    Friend WithEvents rdbKouza As System.Windows.Forms.RadioButton
    Friend WithEvents rdbKigyoSeq As System.Windows.Forms.RadioButton
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents FURI_DATE_K2 As System.Windows.Forms.TextBox
    Friend WithEvents FURI_DATE_K1 As System.Windows.Forms.TextBox
    Friend WithEvents FURI_DATE_K As System.Windows.Forms.TextBox
    Friend WithEvents TORIF_CODE_K As System.Windows.Forms.TextBox
    Friend WithEvents TORIS_CODE_K As System.Windows.Forms.TextBox
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
End Class
