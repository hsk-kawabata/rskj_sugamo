<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFUMAST010
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()
        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()


        '金融機関コード
        AddHandler KIN_NO_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler KIN_NO_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KIN_NO_N.LostFocus, AddressOf CAST.LostFocus

        '金融機関名（カナ）
        AddHandler KIN_KNAME_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler KIN_KNAME_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler KIN_KNAME_N.LostFocus, AddressOf CAST.LostFocus

        '金融機関名（漢字）
        AddHandler KIN_NNAME_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler KIN_NNAME_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler KIN_NNAME_N.LostFocus, AddressOf CAST.LostFocus

        '支店コード
        AddHandler SIT_NO_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler SIT_NO_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler SIT_NO_N.LostFocus, AddressOf CAST.LostFocus

        '金融機関付加コード
        AddHandler KIN_FUKA_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler KIN_FUKA_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KIN_FUKA_N.LostFocus, AddressOf CAST.LostFocus

        '支店付加コード
        AddHandler SIT_FUKA_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler SIT_FUKA_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler SIT_FUKA_N.LostFocus, AddressOf CAST.LostFocus

        '支店名（カナ）
        AddHandler SIT_KNAME_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler SIT_KNAME_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler SIT_KNAME_N.LostFocus, AddressOf CAST.LostFocus

        '支店名（漢字）
        AddHandler SIT_NNAME_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler SIT_NNAME_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler SIT_NNAME_N.LostFocus, AddressOf CAST.LostFocus

        '提携区分
        AddHandler TEIKEI_KBN_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler TEIKEI_KBN_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler TEIKEI_KBN_N.LostFocus, AddressOf CAST.LostFocus

        '電話番号
        AddHandler DENWA_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler DENWA_N.KeyPress, AddressOf CASTx03.KeyPress
        AddHandler DENWA_N.LostFocus, AddressOf CAST.LostFocus

        'FAX番号
        AddHandler FAX_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler FAX_N.KeyPress, AddressOf CASTx03.KeyPress
        AddHandler FAX_N.LostFocus, AddressOf CAST.LostFocus

        '郵便番号
        AddHandler YUUBIN_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler YUUBIN_N.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler YUUBIN_N.LostFocus, AddressOf CAST.LostFocus

        'カナ住所
        AddHandler KJYU_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler KJYU_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler KJYU_N.LostFocus, AddressOf CAST.LostFocus

        '漢字住所
        AddHandler NJYU_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler NJYU_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler NJYU_N.LostFocus, AddressOf CAST.LostFocus

        '新金融機関コード
        AddHandler NEW_KIN_NO_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler NEW_KIN_NO_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler NEW_KIN_NO_N.LostFocus, AddressOf CAST.LostFocus

        '新金融機関付加コード
        AddHandler NEW_KIN_FUKA_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler NEW_KIN_FUKA_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler NEW_KIN_FUKA_N.LostFocus, AddressOf CAST.LostFocus

        '新支店コード
        AddHandler NEW_SIT_NO_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler NEW_SIT_NO_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler NEW_SIT_NO_N.LostFocus, AddressOf CAST.LostFocus

        '新支店付加コード
        AddHandler NEW_SIT_FUKA_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler NEW_SIT_FUKA_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler NEW_SIT_FUKA_N.LostFocus, AddressOf CAST.LostFocus

        '削除日付（年）
        AddHandler KIN_DEL_DATE_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler KIN_DEL_DATE_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KIN_DEL_DATE_N.LostFocus, AddressOf CAST.LostFocus

        '削除日付（月）
        AddHandler KIN_DEL_DATE_N1.GotFocus, AddressOf CAST.GotFocus
        AddHandler KIN_DEL_DATE_N1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KIN_DEL_DATE_N1.LostFocus, AddressOf CAST.LostFocus

        '削除日付（日）
        AddHandler KIN_DEL_DATE_N2.GotFocus, AddressOf CAST.GotFocus
        AddHandler KIN_DEL_DATE_N2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KIN_DEL_DATE_N2.LostFocus, AddressOf CAST.LostFocus

        '金融機関削除日（年）
        AddHandler SIT_DEL_DATE_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler SIT_DEL_DATE_N.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler SIT_DEL_DATE_N.LostFocus, AddressOf CAST.LostFocus

        '金融機関削除日（月）
        AddHandler SIT_DEL_DATE_N1.GotFocus, AddressOf CAST.GotFocus
        AddHandler SIT_DEL_DATE_N1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler SIT_DEL_DATE_N1.LostFocus, AddressOf CAST.LostFocus

        '金融機関削除日（日）
        AddHandler SIT_DEL_DATE_N2.GotFocus, AddressOf CAST.GotFocus
        AddHandler SIT_DEL_DATE_N2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler SIT_DEL_DATE_N2.LostFocus, AddressOf CAST.LostFocus

        '***** 2009/10/12 kakiwaki *******
        '金融機関種類
        AddHandler SYUBETU_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler SYUBETU_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler SYUBETU_N.LostFocus, AddressOf CAST.LostFocus

        '地区コード
        AddHandler TIKU_CODE_N.GotFocus, AddressOf CAST.GotFocus
        AddHandler TIKU_CODE_N.KeyPress, AddressOf CAST.KeyPress
        AddHandler TIKU_CODE_N.LostFocus, AddressOf CAST.LostFocus

        '支払手数料
        AddHandler TESUU_TANKA_N.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler TESUU_TANKA_N.KeyPress, AddressOf CAST.KeyPressMoney
        AddHandler TESUU_TANKA_N.LostFocus, AddressOf CAST.LostFocusMoney
        '**************** 2009/10/12 *****

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFUMAST010))
        Me.lblTitle = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.btnKousin = New System.Windows.Forms.Button
        Me.btnEraser = New System.Windows.Forms.Button
        Me.btnSansyo = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.DEL_KIN_DATE_L2 = New System.Windows.Forms.Label
        Me.DEL_KIN_DATE_L1 = New System.Windows.Forms.Label
        Me.DEL_KIN_DATE_L0 = New System.Windows.Forms.Label
        Me.TEIKEI_KBN_N = New System.Windows.Forms.ComboBox
        Me.SIT_DEL_DATE_N2 = New System.Windows.Forms.TextBox
        Me.TEIKEI_KBN_L = New System.Windows.Forms.Label
        Me.SIT_DEL_DATE_N1 = New System.Windows.Forms.TextBox
        Me.SIT_DEL_DATE_N = New System.Windows.Forms.TextBox
        Me.DEL_KIN_DATE_L = New System.Windows.Forms.Label
        Me.DEL_DATE_L2 = New System.Windows.Forms.Label
        Me.DEL_DATE_L1 = New System.Windows.Forms.Label
        Me.DEL_DATE_L0 = New System.Windows.Forms.Label
        Me.KOUSIN_DATE_N = New System.Windows.Forms.Label
        Me.KOUSIN_DATE_L = New System.Windows.Forms.Label
        Me.SAKUSEI_DATE_N = New System.Windows.Forms.Label
        Me.SAKUSEI_DATE_L = New System.Windows.Forms.Label
        Me.KIN_DEL_DATE_N2 = New System.Windows.Forms.TextBox
        Me.KIN_DEL_DATE_N1 = New System.Windows.Forms.TextBox
        Me.KIN_DEL_DATE_N = New System.Windows.Forms.TextBox
        Me.DEL_DATE_L = New System.Windows.Forms.Label
        Me.NEW_SIT_NO_N = New System.Windows.Forms.TextBox
        Me.NEW_SIT_FUKA_N = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.OLDSIT_NO_L = New System.Windows.Forms.Label
        Me.NJYU_N = New System.Windows.Forms.TextBox
        Me.NEW_KIN_FUKA_N = New System.Windows.Forms.TextBox
        Me.NEW_KIN_NO_N = New System.Windows.Forms.TextBox
        Me.KJYU_N = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.OLDKIN_NO_L = New System.Windows.Forms.Label
        Me.NJYU_L = New System.Windows.Forms.Label
        Me.KJYU_L = New System.Windows.Forms.Label
        Me.FAX_N = New System.Windows.Forms.TextBox
        Me.FAX_L = New System.Windows.Forms.Label
        Me.YUUBIN_N = New System.Windows.Forms.TextBox
        Me.DENWA_N = New System.Windows.Forms.TextBox
        Me.Label14 = New System.Windows.Forms.Label
        Me.DENWA_L = New System.Windows.Forms.Label
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.TESUU_TANKA_N = New System.Windows.Forms.TextBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.TIKU_CODE_N = New System.Windows.Forms.ComboBox
        Me.SYUBETU_N = New System.Windows.Forms.ComboBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.btnSit_Fuka_Sansyo = New System.Windows.Forms.Button
        Me.SIT_FUKA_N = New System.Windows.Forms.TextBox
        Me.EDA_L = New System.Windows.Forms.Label
        Me.SIT_NNAME_N = New System.Windows.Forms.TextBox
        Me.SIT_KNAME_N = New System.Windows.Forms.TextBox
        Me.KIN_FUKA_N = New System.Windows.Forms.TextBox
        Me.SIT_NO_N = New System.Windows.Forms.TextBox
        Me.KIN_NNAME_N = New System.Windows.Forms.TextBox
        Me.KIN_KNAME_N = New System.Windows.Forms.TextBox
        Me.SIT_NNAME_L = New System.Windows.Forms.Label
        Me.SIT_KNAME_L = New System.Windows.Forms.Label
        Me.KIN_KNAME_L = New System.Windows.Forms.Label
        Me.KIN_NNAME_L = New System.Windows.Forms.Label
        Me.SIT_NO_L = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.KIN_NO_L = New System.Windows.Forms.Label
        Me.KIN_NO_N = New System.Windows.Forms.TextBox
        Me.btnKin_Fuka_Sansyo = New System.Windows.Forms.Button
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "＜金融機関マスタメンテナンス＞"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(620, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ログイン名　:"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(685, 10)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 2
        Me.lblUser.Text = "admin"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(606, 33)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "システム日付:"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(685, 33)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 1
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'btnKousin
        '
        Me.btnKousin.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKousin.Location = New System.Drawing.Point(140, 520)
        Me.btnKousin.Name = "btnKousin"
        Me.btnKousin.Size = New System.Drawing.Size(120, 40)
        Me.btnKousin.TabIndex = 101
        Me.btnKousin.Text = "更　新"
        '
        'btnEraser
        '
        Me.btnEraser.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEraser.Location = New System.Drawing.Point(530, 520)
        Me.btnEraser.Name = "btnEraser"
        Me.btnEraser.Size = New System.Drawing.Size(120, 40)
        Me.btnEraser.TabIndex = 104
        Me.btnEraser.Text = "取　消"
        '
        'btnSansyo
        '
        Me.btnSansyo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSansyo.Location = New System.Drawing.Point(270, 520)
        Me.btnSansyo.Name = "btnSansyo"
        Me.btnSansyo.Size = New System.Drawing.Size(120, 40)
        Me.btnSansyo.TabIndex = 102
        Me.btnSansyo.Text = "参　照"
        '
        'btnDelete
        '
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(400, 520)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(120, 40)
        Me.btnDelete.TabIndex = 103
        Me.btnDelete.Text = "削　除"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 105
        Me.btnEnd.Text = "終　了"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(10, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 100
        Me.btnAction.Text = "登　録"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.ItemSize = New System.Drawing.Size(100, 21)
        Me.TabControl1.Location = New System.Drawing.Point(27, 169)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(739, 323)
        Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabControl1.TabIndex = 10
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.DEL_KIN_DATE_L2)
        Me.TabPage1.Controls.Add(Me.DEL_KIN_DATE_L1)
        Me.TabPage1.Controls.Add(Me.DEL_KIN_DATE_L0)
        Me.TabPage1.Controls.Add(Me.TEIKEI_KBN_N)
        Me.TabPage1.Controls.Add(Me.SIT_DEL_DATE_N2)
        Me.TabPage1.Controls.Add(Me.TEIKEI_KBN_L)
        Me.TabPage1.Controls.Add(Me.SIT_DEL_DATE_N1)
        Me.TabPage1.Controls.Add(Me.SIT_DEL_DATE_N)
        Me.TabPage1.Controls.Add(Me.DEL_KIN_DATE_L)
        Me.TabPage1.Controls.Add(Me.DEL_DATE_L2)
        Me.TabPage1.Controls.Add(Me.DEL_DATE_L1)
        Me.TabPage1.Controls.Add(Me.DEL_DATE_L0)
        Me.TabPage1.Controls.Add(Me.KOUSIN_DATE_N)
        Me.TabPage1.Controls.Add(Me.KOUSIN_DATE_L)
        Me.TabPage1.Controls.Add(Me.SAKUSEI_DATE_N)
        Me.TabPage1.Controls.Add(Me.SAKUSEI_DATE_L)
        Me.TabPage1.Controls.Add(Me.KIN_DEL_DATE_N2)
        Me.TabPage1.Controls.Add(Me.KIN_DEL_DATE_N1)
        Me.TabPage1.Controls.Add(Me.KIN_DEL_DATE_N)
        Me.TabPage1.Controls.Add(Me.DEL_DATE_L)
        Me.TabPage1.Controls.Add(Me.NEW_SIT_NO_N)
        Me.TabPage1.Controls.Add(Me.NEW_SIT_FUKA_N)
        Me.TabPage1.Controls.Add(Me.Label8)
        Me.TabPage1.Controls.Add(Me.OLDSIT_NO_L)
        Me.TabPage1.Controls.Add(Me.NJYU_N)
        Me.TabPage1.Controls.Add(Me.NEW_KIN_FUKA_N)
        Me.TabPage1.Controls.Add(Me.NEW_KIN_NO_N)
        Me.TabPage1.Controls.Add(Me.KJYU_N)
        Me.TabPage1.Controls.Add(Me.Label7)
        Me.TabPage1.Controls.Add(Me.OLDKIN_NO_L)
        Me.TabPage1.Controls.Add(Me.NJYU_L)
        Me.TabPage1.Controls.Add(Me.KJYU_L)
        Me.TabPage1.Controls.Add(Me.FAX_N)
        Me.TabPage1.Controls.Add(Me.FAX_L)
        Me.TabPage1.Controls.Add(Me.YUUBIN_N)
        Me.TabPage1.Controls.Add(Me.DENWA_N)
        Me.TabPage1.Controls.Add(Me.Label14)
        Me.TabPage1.Controls.Add(Me.DENWA_L)
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(731, 294)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "基本情報"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'DEL_KIN_DATE_L2
        '
        Me.DEL_KIN_DATE_L2.AutoSize = True
        Me.DEL_KIN_DATE_L2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_KIN_DATE_L2.Location = New System.Drawing.Point(340, 262)
        Me.DEL_KIN_DATE_L2.Name = "DEL_KIN_DATE_L2"
        Me.DEL_KIN_DATE_L2.Size = New System.Drawing.Size(24, 16)
        Me.DEL_KIN_DATE_L2.TabIndex = 217
        Me.DEL_KIN_DATE_L2.Text = "日"
        '
        'DEL_KIN_DATE_L1
        '
        Me.DEL_KIN_DATE_L1.AutoSize = True
        Me.DEL_KIN_DATE_L1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_KIN_DATE_L1.Location = New System.Drawing.Point(276, 262)
        Me.DEL_KIN_DATE_L1.Name = "DEL_KIN_DATE_L1"
        Me.DEL_KIN_DATE_L1.Size = New System.Drawing.Size(24, 16)
        Me.DEL_KIN_DATE_L1.TabIndex = 216
        Me.DEL_KIN_DATE_L1.Text = "月"
        '
        'DEL_KIN_DATE_L0
        '
        Me.DEL_KIN_DATE_L0.AutoSize = True
        Me.DEL_KIN_DATE_L0.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_KIN_DATE_L0.Location = New System.Drawing.Point(212, 262)
        Me.DEL_KIN_DATE_L0.Name = "DEL_KIN_DATE_L0"
        Me.DEL_KIN_DATE_L0.Size = New System.Drawing.Size(24, 16)
        Me.DEL_KIN_DATE_L0.TabIndex = 215
        Me.DEL_KIN_DATE_L0.Text = "年"
        '
        'TEIKEI_KBN_N
        '
        Me.TEIKEI_KBN_N.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TEIKEI_KBN_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TEIKEI_KBN_N.Location = New System.Drawing.Point(509, 46)
        Me.TEIKEI_KBN_N.Name = "TEIKEI_KBN_N"
        Me.TEIKEI_KBN_N.Size = New System.Drawing.Size(160, 24)
        Me.TEIKEI_KBN_N.TabIndex = 3
        '
        'SIT_DEL_DATE_N2
        '
        Me.SIT_DEL_DATE_N2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_DEL_DATE_N2.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.SIT_DEL_DATE_N2.Location = New System.Drawing.Point(304, 259)
        Me.SIT_DEL_DATE_N2.MaxLength = 2
        Me.SIT_DEL_DATE_N2.Name = "SIT_DEL_DATE_N2"
        Me.SIT_DEL_DATE_N2.Size = New System.Drawing.Size(32, 23)
        Me.SIT_DEL_DATE_N2.TabIndex = 15
        '
        'TEIKEI_KBN_L
        '
        Me.TEIKEI_KBN_L.AutoSize = True
        Me.TEIKEI_KBN_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TEIKEI_KBN_L.Location = New System.Drawing.Point(387, 49)
        Me.TEIKEI_KBN_L.Name = "TEIKEI_KBN_L"
        Me.TEIKEI_KBN_L.Size = New System.Drawing.Size(72, 16)
        Me.TEIKEI_KBN_L.TabIndex = 223
        Me.TEIKEI_KBN_L.Text = "提携区分"
        '
        'SIT_DEL_DATE_N1
        '
        Me.SIT_DEL_DATE_N1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_DEL_DATE_N1.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.SIT_DEL_DATE_N1.Location = New System.Drawing.Point(240, 259)
        Me.SIT_DEL_DATE_N1.MaxLength = 2
        Me.SIT_DEL_DATE_N1.Name = "SIT_DEL_DATE_N1"
        Me.SIT_DEL_DATE_N1.Size = New System.Drawing.Size(25, 23)
        Me.SIT_DEL_DATE_N1.TabIndex = 14
        '
        'SIT_DEL_DATE_N
        '
        Me.SIT_DEL_DATE_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_DEL_DATE_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.SIT_DEL_DATE_N.Location = New System.Drawing.Point(158, 259)
        Me.SIT_DEL_DATE_N.MaxLength = 4
        Me.SIT_DEL_DATE_N.Name = "SIT_DEL_DATE_N"
        Me.SIT_DEL_DATE_N.Size = New System.Drawing.Size(40, 23)
        Me.SIT_DEL_DATE_N.TabIndex = 13
        '
        'DEL_KIN_DATE_L
        '
        Me.DEL_KIN_DATE_L.AutoSize = True
        Me.DEL_KIN_DATE_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_KIN_DATE_L.Location = New System.Drawing.Point(10, 262)
        Me.DEL_KIN_DATE_L.Name = "DEL_KIN_DATE_L"
        Me.DEL_KIN_DATE_L.Size = New System.Drawing.Size(88, 16)
        Me.DEL_KIN_DATE_L.TabIndex = 214
        Me.DEL_KIN_DATE_L.Text = "支店削除日"
        '
        'DEL_DATE_L2
        '
        Me.DEL_DATE_L2.AutoSize = True
        Me.DEL_DATE_L2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_DATE_L2.Location = New System.Drawing.Point(340, 232)
        Me.DEL_DATE_L2.Name = "DEL_DATE_L2"
        Me.DEL_DATE_L2.Size = New System.Drawing.Size(24, 16)
        Me.DEL_DATE_L2.TabIndex = 213
        Me.DEL_DATE_L2.Text = "日"
        '
        'DEL_DATE_L1
        '
        Me.DEL_DATE_L1.AutoSize = True
        Me.DEL_DATE_L1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_DATE_L1.Location = New System.Drawing.Point(276, 232)
        Me.DEL_DATE_L1.Name = "DEL_DATE_L1"
        Me.DEL_DATE_L1.Size = New System.Drawing.Size(24, 16)
        Me.DEL_DATE_L1.TabIndex = 212
        Me.DEL_DATE_L1.Text = "月"
        '
        'DEL_DATE_L0
        '
        Me.DEL_DATE_L0.AutoSize = True
        Me.DEL_DATE_L0.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_DATE_L0.Location = New System.Drawing.Point(212, 232)
        Me.DEL_DATE_L0.Name = "DEL_DATE_L0"
        Me.DEL_DATE_L0.Size = New System.Drawing.Size(24, 16)
        Me.DEL_DATE_L0.TabIndex = 211
        Me.DEL_DATE_L0.Text = "年"
        '
        'KOUSIN_DATE_N
        '
        Me.KOUSIN_DATE_N.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.KOUSIN_DATE_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KOUSIN_DATE_N.Location = New System.Drawing.Point(618, 260)
        Me.KOUSIN_DATE_N.Name = "KOUSIN_DATE_N"
        Me.KOUSIN_DATE_N.Size = New System.Drawing.Size(92, 19)
        Me.KOUSIN_DATE_N.TabIndex = 210
        Me.KOUSIN_DATE_N.Tag = "1"
        Me.KOUSIN_DATE_N.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'KOUSIN_DATE_L
        '
        Me.KOUSIN_DATE_L.AutoSize = True
        Me.KOUSIN_DATE_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KOUSIN_DATE_L.Location = New System.Drawing.Point(544, 261)
        Me.KOUSIN_DATE_L.Name = "KOUSIN_DATE_L"
        Me.KOUSIN_DATE_L.Size = New System.Drawing.Size(56, 16)
        Me.KOUSIN_DATE_L.TabIndex = 209
        Me.KOUSIN_DATE_L.Text = "更新日"
        '
        'SAKUSEI_DATE_N
        '
        Me.SAKUSEI_DATE_N.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SAKUSEI_DATE_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAKUSEI_DATE_N.Location = New System.Drawing.Point(430, 260)
        Me.SAKUSEI_DATE_N.Name = "SAKUSEI_DATE_N"
        Me.SAKUSEI_DATE_N.Size = New System.Drawing.Size(92, 19)
        Me.SAKUSEI_DATE_N.TabIndex = 208
        Me.SAKUSEI_DATE_N.Tag = "1"
        Me.SAKUSEI_DATE_N.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'SAKUSEI_DATE_L
        '
        Me.SAKUSEI_DATE_L.AutoSize = True
        Me.SAKUSEI_DATE_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAKUSEI_DATE_L.Location = New System.Drawing.Point(364, 261)
        Me.SAKUSEI_DATE_L.Name = "SAKUSEI_DATE_L"
        Me.SAKUSEI_DATE_L.Size = New System.Drawing.Size(56, 16)
        Me.SAKUSEI_DATE_L.TabIndex = 207
        Me.SAKUSEI_DATE_L.Text = "作成日"
        '
        'KIN_DEL_DATE_N2
        '
        Me.KIN_DEL_DATE_N2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_DEL_DATE_N2.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.KIN_DEL_DATE_N2.Location = New System.Drawing.Point(304, 229)
        Me.KIN_DEL_DATE_N2.MaxLength = 2
        Me.KIN_DEL_DATE_N2.Name = "KIN_DEL_DATE_N2"
        Me.KIN_DEL_DATE_N2.Size = New System.Drawing.Size(32, 23)
        Me.KIN_DEL_DATE_N2.TabIndex = 12
        '
        'KIN_DEL_DATE_N1
        '
        Me.KIN_DEL_DATE_N1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_DEL_DATE_N1.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.KIN_DEL_DATE_N1.Location = New System.Drawing.Point(240, 229)
        Me.KIN_DEL_DATE_N1.MaxLength = 2
        Me.KIN_DEL_DATE_N1.Name = "KIN_DEL_DATE_N1"
        Me.KIN_DEL_DATE_N1.Size = New System.Drawing.Size(25, 23)
        Me.KIN_DEL_DATE_N1.TabIndex = 11
        '
        'KIN_DEL_DATE_N
        '
        Me.KIN_DEL_DATE_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_DEL_DATE_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.KIN_DEL_DATE_N.Location = New System.Drawing.Point(158, 229)
        Me.KIN_DEL_DATE_N.MaxLength = 4
        Me.KIN_DEL_DATE_N.Name = "KIN_DEL_DATE_N"
        Me.KIN_DEL_DATE_N.Size = New System.Drawing.Size(40, 23)
        Me.KIN_DEL_DATE_N.TabIndex = 10
        '
        'DEL_DATE_L
        '
        Me.DEL_DATE_L.AutoSize = True
        Me.DEL_DATE_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DEL_DATE_L.Location = New System.Drawing.Point(10, 231)
        Me.DEL_DATE_L.Name = "DEL_DATE_L"
        Me.DEL_DATE_L.Size = New System.Drawing.Size(120, 16)
        Me.DEL_DATE_L.TabIndex = 204
        Me.DEL_DATE_L.Text = "金融機関削除日" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'NEW_SIT_NO_N
        '
        Me.NEW_SIT_NO_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.NEW_SIT_NO_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.NEW_SIT_NO_N.Location = New System.Drawing.Point(465, 200)
        Me.NEW_SIT_NO_N.MaxLength = 3
        Me.NEW_SIT_NO_N.Name = "NEW_SIT_NO_N"
        Me.NEW_SIT_NO_N.Size = New System.Drawing.Size(32, 23)
        Me.NEW_SIT_NO_N.TabIndex = 8
        '
        'NEW_SIT_FUKA_N
        '
        Me.NEW_SIT_FUKA_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.NEW_SIT_FUKA_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.NEW_SIT_FUKA_N.Location = New System.Drawing.Point(662, 200)
        Me.NEW_SIT_FUKA_N.MaxLength = 2
        Me.NEW_SIT_FUKA_N.Name = "NEW_SIT_FUKA_N"
        Me.NEW_SIT_FUKA_N.Size = New System.Drawing.Size(25, 23)
        Me.NEW_SIT_FUKA_N.TabIndex = 9
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(364, 203)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(104, 16)
        Me.Label8.TabIndex = 202
        Me.Label8.Text = "新支店コード"
        '
        'OLDSIT_NO_L
        '
        Me.OLDSIT_NO_L.AutoSize = True
        Me.OLDSIT_NO_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.OLDSIT_NO_L.Location = New System.Drawing.Point(524, 204)
        Me.OLDSIT_NO_L.Name = "OLDSIT_NO_L"
        Me.OLDSIT_NO_L.Size = New System.Drawing.Size(136, 16)
        Me.OLDSIT_NO_L.TabIndex = 203
        Me.OLDSIT_NO_L.Text = "新支店付加コード"
        '
        'NJYU_N
        '
        Me.NJYU_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.NJYU_N.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.NJYU_N.Location = New System.Drawing.Point(158, 140)
        Me.NJYU_N.MaxLength = 110
        Me.NJYU_N.Multiline = True
        Me.NJYU_N.Name = "NJYU_N"
        Me.NJYU_N.Size = New System.Drawing.Size(552, 48)
        Me.NJYU_N.TabIndex = 5
        '
        'NEW_KIN_FUKA_N
        '
        Me.NEW_KIN_FUKA_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.NEW_KIN_FUKA_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.NEW_KIN_FUKA_N.Location = New System.Drawing.Point(318, 200)
        Me.NEW_KIN_FUKA_N.MaxLength = 1
        Me.NEW_KIN_FUKA_N.Name = "NEW_KIN_FUKA_N"
        Me.NEW_KIN_FUKA_N.Size = New System.Drawing.Size(17, 23)
        Me.NEW_KIN_FUKA_N.TabIndex = 7
        '
        'NEW_KIN_NO_N
        '
        Me.NEW_KIN_NO_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.NEW_KIN_NO_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.NEW_KIN_NO_N.Location = New System.Drawing.Point(158, 199)
        Me.NEW_KIN_NO_N.MaxLength = 4
        Me.NEW_KIN_NO_N.Name = "NEW_KIN_NO_N"
        Me.NEW_KIN_NO_N.Size = New System.Drawing.Size(40, 23)
        Me.NEW_KIN_NO_N.TabIndex = 6
        '
        'KJYU_N
        '
        Me.KJYU_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KJYU_N.ImeMode = System.Windows.Forms.ImeMode.KatakanaHalf
        Me.KJYU_N.Location = New System.Drawing.Point(158, 80)
        Me.KJYU_N.MaxLength = 80
        Me.KJYU_N.Multiline = True
        Me.KJYU_N.Name = "KJYU_N"
        Me.KJYU_N.Size = New System.Drawing.Size(552, 48)
        Me.KJYU_N.TabIndex = 4
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(211, 203)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(104, 16)
        Me.Label7.TabIndex = 200
        Me.Label7.Text = "新付加コード"
        '
        'OLDKIN_NO_L
        '
        Me.OLDKIN_NO_L.AutoSize = True
        Me.OLDKIN_NO_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.OLDKIN_NO_L.Location = New System.Drawing.Point(10, 203)
        Me.OLDKIN_NO_L.Name = "OLDKIN_NO_L"
        Me.OLDKIN_NO_L.Size = New System.Drawing.Size(136, 16)
        Me.OLDKIN_NO_L.TabIndex = 201
        Me.OLDKIN_NO_L.Text = "新金融機関コード"
        '
        'NJYU_L
        '
        Me.NJYU_L.AutoSize = True
        Me.NJYU_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.NJYU_L.Location = New System.Drawing.Point(10, 142)
        Me.NJYU_L.Name = "NJYU_L"
        Me.NJYU_L.Size = New System.Drawing.Size(72, 16)
        Me.NJYU_L.TabIndex = 199
        Me.NJYU_L.Text = "漢字住所"
        '
        'KJYU_L
        '
        Me.KJYU_L.AutoSize = True
        Me.KJYU_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KJYU_L.Location = New System.Drawing.Point(10, 82)
        Me.KJYU_L.Name = "KJYU_L"
        Me.KJYU_L.Size = New System.Drawing.Size(72, 16)
        Me.KJYU_L.TabIndex = 198
        Me.KJYU_L.Text = "カナ住所"
        '
        'FAX_N
        '
        Me.FAX_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FAX_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.FAX_N.Location = New System.Drawing.Point(508, 13)
        Me.FAX_N.MaxLength = 13
        Me.FAX_N.Name = "FAX_N"
        Me.FAX_N.Size = New System.Drawing.Size(112, 23)
        Me.FAX_N.TabIndex = 1
        '
        'FAX_L
        '
        Me.FAX_L.AutoSize = True
        Me.FAX_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FAX_L.Location = New System.Drawing.Point(388, 15)
        Me.FAX_L.Name = "FAX_L"
        Me.FAX_L.Size = New System.Drawing.Size(88, 16)
        Me.FAX_L.TabIndex = 197
        Me.FAX_L.Text = "ＦＡＸ番号"
        '
        'YUUBIN_N
        '
        Me.YUUBIN_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.YUUBIN_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.YUUBIN_N.Location = New System.Drawing.Point(158, 47)
        Me.YUUBIN_N.MaxLength = 8
        Me.YUUBIN_N.Name = "YUUBIN_N"
        Me.YUUBIN_N.Size = New System.Drawing.Size(72, 23)
        Me.YUUBIN_N.TabIndex = 2
        '
        'DENWA_N
        '
        Me.DENWA_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DENWA_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.DENWA_N.Location = New System.Drawing.Point(158, 13)
        Me.DENWA_N.MaxLength = 13
        Me.DENWA_N.Name = "DENWA_N"
        Me.DENWA_N.Size = New System.Drawing.Size(112, 23)
        Me.DENWA_N.TabIndex = 0
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(10, 48)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(72, 16)
        Me.Label14.TabIndex = 196
        Me.Label14.Text = "郵便番号"
        '
        'DENWA_L
        '
        Me.DENWA_L.AutoSize = True
        Me.DENWA_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DENWA_L.Location = New System.Drawing.Point(10, 15)
        Me.DENWA_L.Name = "DENWA_L"
        Me.DENWA_L.Size = New System.Drawing.Size(72, 16)
        Me.DENWA_L.TabIndex = 195
        Me.DENWA_L.Text = "電話番号"
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Size = New System.Drawing.Size(731, 294)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "拡張情報"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.TESUU_TANKA_N)
        Me.TabPage3.Controls.Add(Me.Label13)
        Me.TabPage3.Controls.Add(Me.Label12)
        Me.TabPage3.Controls.Add(Me.TIKU_CODE_N)
        Me.TabPage3.Controls.Add(Me.SYUBETU_N)
        Me.TabPage3.Controls.Add(Me.Label11)
        Me.TabPage3.Controls.Add(Me.Label10)
        Me.TabPage3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage3.Location = New System.Drawing.Point(4, 25)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(731, 294)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "地域センタ情報"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'TESUU_TANKA_N
        '
        Me.TESUU_TANKA_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_TANKA_N.Location = New System.Drawing.Point(347, 159)
        Me.TESUU_TANKA_N.MaxLength = 6
        Me.TESUU_TANKA_N.Name = "TESUU_TANKA_N"
        Me.TESUU_TANKA_N.Size = New System.Drawing.Size(68, 23)
        Me.TESUU_TANKA_N.TabIndex = 18
        Me.TESUU_TANKA_N.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(431, 165)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(24, 16)
        Me.Label13.TabIndex = 227
        Me.Label13.Text = "円"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(224, 161)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(88, 16)
        Me.Label12.TabIndex = 227
        Me.Label12.Text = "支払手数料"
        '
        'TIKU_CODE_N
        '
        Me.TIKU_CODE_N.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TIKU_CODE_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TIKU_CODE_N.Location = New System.Drawing.Point(347, 114)
        Me.TIKU_CODE_N.Name = "TIKU_CODE_N"
        Me.TIKU_CODE_N.Size = New System.Drawing.Size(160, 24)
        Me.TIKU_CODE_N.TabIndex = 17
        '
        'SYUBETU_N
        '
        Me.SYUBETU_N.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SYUBETU_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SYUBETU_N.Location = New System.Drawing.Point(346, 68)
        Me.SYUBETU_N.Name = "SYUBETU_N"
        Me.SYUBETU_N.Size = New System.Drawing.Size(160, 24)
        Me.SYUBETU_N.TabIndex = 16
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(224, 116)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(88, 16)
        Me.Label11.TabIndex = 225
        Me.Label11.Text = "地区コード"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(224, 71)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(104, 16)
        Me.Label10.TabIndex = 225
        Me.Label10.Text = "金融機関種類"
        '
        'btnSit_Fuka_Sansyo
        '
        Me.btnSit_Fuka_Sansyo.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSit_Fuka_Sansyo.Location = New System.Drawing.Point(589, 80)
        Me.btnSit_Fuka_Sansyo.Name = "btnSit_Fuka_Sansyo"
        Me.btnSit_Fuka_Sansyo.Size = New System.Drawing.Size(52, 25)
        Me.btnSit_Fuka_Sansyo.TabIndex = 5
        Me.btnSit_Fuka_Sansyo.Text = "参照"
        '
        'SIT_FUKA_N
        '
        Me.SIT_FUKA_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_FUKA_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.SIT_FUKA_N.Location = New System.Drawing.Point(536, 80)
        Me.SIT_FUKA_N.MaxLength = 2
        Me.SIT_FUKA_N.Name = "SIT_FUKA_N"
        Me.SIT_FUKA_N.Size = New System.Drawing.Size(40, 23)
        Me.SIT_FUKA_N.TabIndex = 4
        Me.SIT_FUKA_N.Tag = ""
        '
        'EDA_L
        '
        Me.EDA_L.AutoSize = True
        Me.EDA_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.EDA_L.Location = New System.Drawing.Point(422, 84)
        Me.EDA_L.Name = "EDA_L"
        Me.EDA_L.Size = New System.Drawing.Size(112, 16)
        Me.EDA_L.TabIndex = 224
        Me.EDA_L.Text = "支店付加ｺｰﾄﾞ"
        '
        'SIT_NNAME_N
        '
        Me.SIT_NNAME_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_NNAME_N.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.SIT_NNAME_N.Location = New System.Drawing.Point(536, 141)
        Me.SIT_NNAME_N.MaxLength = 30
        Me.SIT_NNAME_N.Name = "SIT_NNAME_N"
        Me.SIT_NNAME_N.Size = New System.Drawing.Size(249, 23)
        Me.SIT_NNAME_N.TabIndex = 9
        '
        'SIT_KNAME_N
        '
        Me.SIT_KNAME_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_KNAME_N.ImeMode = System.Windows.Forms.ImeMode.KatakanaHalf
        Me.SIT_KNAME_N.Location = New System.Drawing.Point(536, 111)
        Me.SIT_KNAME_N.MaxLength = 15
        Me.SIT_KNAME_N.Name = "SIT_KNAME_N"
        Me.SIT_KNAME_N.Size = New System.Drawing.Size(200, 23)
        Me.SIT_KNAME_N.TabIndex = 8
        '
        'KIN_FUKA_N
        '
        Me.KIN_FUKA_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_FUKA_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.KIN_FUKA_N.Location = New System.Drawing.Point(165, 80)
        Me.KIN_FUKA_N.MaxLength = 1
        Me.KIN_FUKA_N.Name = "KIN_FUKA_N"
        Me.KIN_FUKA_N.Size = New System.Drawing.Size(23, 23)
        Me.KIN_FUKA_N.TabIndex = 1
        Me.KIN_FUKA_N.Tag = ""
        '
        'SIT_NO_N
        '
        Me.SIT_NO_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_NO_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.SIT_NO_N.Location = New System.Drawing.Point(536, 50)
        Me.SIT_NO_N.MaxLength = 3
        Me.SIT_NO_N.Name = "SIT_NO_N"
        Me.SIT_NO_N.Size = New System.Drawing.Size(40, 23)
        Me.SIT_NO_N.TabIndex = 3
        Me.SIT_NO_N.Tag = ""
        '
        'KIN_NNAME_N
        '
        Me.KIN_NNAME_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_NNAME_N.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.KIN_NNAME_N.Location = New System.Drawing.Point(165, 141)
        Me.KIN_NNAME_N.MaxLength = 30
        Me.KIN_NNAME_N.Name = "KIN_NNAME_N"
        Me.KIN_NNAME_N.Size = New System.Drawing.Size(249, 23)
        Me.KIN_NNAME_N.TabIndex = 7
        '
        'KIN_KNAME_N
        '
        Me.KIN_KNAME_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_KNAME_N.ImeMode = System.Windows.Forms.ImeMode.KatakanaHalf
        Me.KIN_KNAME_N.Location = New System.Drawing.Point(165, 111)
        Me.KIN_KNAME_N.MaxLength = 15
        Me.KIN_KNAME_N.Name = "KIN_KNAME_N"
        Me.KIN_KNAME_N.Size = New System.Drawing.Size(200, 23)
        Me.KIN_KNAME_N.TabIndex = 6
        '
        'SIT_NNAME_L
        '
        Me.SIT_NNAME_L.AutoSize = True
        Me.SIT_NNAME_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_NNAME_L.Location = New System.Drawing.Point(422, 145)
        Me.SIT_NNAME_L.Name = "SIT_NNAME_L"
        Me.SIT_NNAME_L.Size = New System.Drawing.Size(111, 16)
        Me.SIT_NNAME_L.TabIndex = 222
        Me.SIT_NNAME_L.Text = "支店名(漢字)"
        '
        'SIT_KNAME_L
        '
        Me.SIT_KNAME_L.AutoSize = True
        Me.SIT_KNAME_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_KNAME_L.Location = New System.Drawing.Point(422, 115)
        Me.SIT_KNAME_L.Name = "SIT_KNAME_L"
        Me.SIT_KNAME_L.Size = New System.Drawing.Size(111, 16)
        Me.SIT_KNAME_L.TabIndex = 221
        Me.SIT_KNAME_L.Text = "支店名(カナ)"
        '
        'KIN_KNAME_L
        '
        Me.KIN_KNAME_L.AutoSize = True
        Me.KIN_KNAME_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_KNAME_L.Location = New System.Drawing.Point(17, 115)
        Me.KIN_KNAME_L.Name = "KIN_KNAME_L"
        Me.KIN_KNAME_L.Size = New System.Drawing.Size(145, 16)
        Me.KIN_KNAME_L.TabIndex = 220
        Me.KIN_KNAME_L.Text = "金融機関名(カナ)"
        '
        'KIN_NNAME_L
        '
        Me.KIN_NNAME_L.AutoSize = True
        Me.KIN_NNAME_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_NNAME_L.Location = New System.Drawing.Point(17, 145)
        Me.KIN_NNAME_L.Name = "KIN_NNAME_L"
        Me.KIN_NNAME_L.Size = New System.Drawing.Size(145, 16)
        Me.KIN_NNAME_L.TabIndex = 219
        Me.KIN_NNAME_L.Text = "金融機関名(漢字)"
        '
        'SIT_NO_L
        '
        Me.SIT_NO_L.AutoSize = True
        Me.SIT_NO_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SIT_NO_L.Location = New System.Drawing.Point(422, 54)
        Me.SIT_NO_L.Name = "SIT_NO_L"
        Me.SIT_NO_L.Size = New System.Drawing.Size(93, 16)
        Me.SIT_NO_L.TabIndex = 218
        Me.SIT_NO_L.Text = "支店コード"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(17, 84)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(146, 16)
        Me.Label6.TabIndex = 216
        Me.Label6.Text = "金融機関付加ｺｰﾄﾞ"
        '
        'KIN_NO_L
        '
        Me.KIN_NO_L.AutoSize = True
        Me.KIN_NO_L.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_NO_L.Location = New System.Drawing.Point(17, 54)
        Me.KIN_NO_L.Name = "KIN_NO_L"
        Me.KIN_NO_L.Size = New System.Drawing.Size(127, 16)
        Me.KIN_NO_L.TabIndex = 217
        Me.KIN_NO_L.Text = "金融機関コード"
        '
        'KIN_NO_N
        '
        Me.KIN_NO_N.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIN_NO_N.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.KIN_NO_N.Location = New System.Drawing.Point(165, 52)
        Me.KIN_NO_N.MaxLength = 4
        Me.KIN_NO_N.Name = "KIN_NO_N"
        Me.KIN_NO_N.Size = New System.Drawing.Size(48, 23)
        Me.KIN_NO_N.TabIndex = 0
        Me.KIN_NO_N.Tag = ""
        '
        'btnKin_Fuka_Sansyo
        '
        Me.btnKin_Fuka_Sansyo.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKin_Fuka_Sansyo.Location = New System.Drawing.Point(205, 80)
        Me.btnKin_Fuka_Sansyo.Name = "btnKin_Fuka_Sansyo"
        Me.btnKin_Fuka_Sansyo.Size = New System.Drawing.Size(52, 25)
        Me.btnKin_Fuka_Sansyo.TabIndex = 2
        Me.btnKin_Fuka_Sansyo.Text = "参照"
        '
        'KFUMAST010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.btnKin_Fuka_Sansyo)
        Me.Controls.Add(Me.btnSit_Fuka_Sansyo)
        Me.Controls.Add(Me.SIT_FUKA_N)
        Me.Controls.Add(Me.EDA_L)
        Me.Controls.Add(Me.SIT_NNAME_N)
        Me.Controls.Add(Me.SIT_KNAME_N)
        Me.Controls.Add(Me.KIN_FUKA_N)
        Me.Controls.Add(Me.SIT_NO_N)
        Me.Controls.Add(Me.KIN_NNAME_N)
        Me.Controls.Add(Me.KIN_KNAME_N)
        Me.Controls.Add(Me.SIT_NNAME_L)
        Me.Controls.Add(Me.SIT_KNAME_L)
        Me.Controls.Add(Me.KIN_KNAME_L)
        Me.Controls.Add(Me.KIN_NNAME_L)
        Me.Controls.Add(Me.SIT_NO_L)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.KIN_NO_L)
        Me.Controls.Add(Me.KIN_NO_N)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.btnKousin)
        Me.Controls.Add(Me.btnEraser)
        Me.Controls.Add(Me.btnSansyo)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFUMAST010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFUMAST010"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnKousin As System.Windows.Forms.Button
    Friend WithEvents btnEraser As System.Windows.Forms.Button
    Friend WithEvents btnSansyo As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents DEL_KIN_DATE_L2 As System.Windows.Forms.Label
    Friend WithEvents DEL_KIN_DATE_L1 As System.Windows.Forms.Label
    Friend WithEvents DEL_KIN_DATE_L0 As System.Windows.Forms.Label
    Friend WithEvents SIT_DEL_DATE_N2 As System.Windows.Forms.TextBox
    Friend WithEvents SIT_DEL_DATE_N1 As System.Windows.Forms.TextBox
    Friend WithEvents SIT_DEL_DATE_N As System.Windows.Forms.TextBox
    Friend WithEvents DEL_KIN_DATE_L As System.Windows.Forms.Label
    Friend WithEvents DEL_DATE_L2 As System.Windows.Forms.Label
    Friend WithEvents DEL_DATE_L1 As System.Windows.Forms.Label
    Friend WithEvents DEL_DATE_L0 As System.Windows.Forms.Label
    Friend WithEvents KOUSIN_DATE_N As System.Windows.Forms.Label
    Friend WithEvents KOUSIN_DATE_L As System.Windows.Forms.Label
    Friend WithEvents SAKUSEI_DATE_N As System.Windows.Forms.Label
    Friend WithEvents SAKUSEI_DATE_L As System.Windows.Forms.Label
    Friend WithEvents KIN_DEL_DATE_N2 As System.Windows.Forms.TextBox
    Friend WithEvents KIN_DEL_DATE_N1 As System.Windows.Forms.TextBox
    Friend WithEvents KIN_DEL_DATE_N As System.Windows.Forms.TextBox
    Friend WithEvents DEL_DATE_L As System.Windows.Forms.Label
    Friend WithEvents NEW_SIT_NO_N As System.Windows.Forms.TextBox
    Friend WithEvents NEW_SIT_FUKA_N As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents OLDSIT_NO_L As System.Windows.Forms.Label
    Friend WithEvents NJYU_N As System.Windows.Forms.TextBox
    Friend WithEvents NEW_KIN_FUKA_N As System.Windows.Forms.TextBox
    Friend WithEvents NEW_KIN_NO_N As System.Windows.Forms.TextBox
    Friend WithEvents KJYU_N As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents OLDKIN_NO_L As System.Windows.Forms.Label
    Friend WithEvents NJYU_L As System.Windows.Forms.Label
    Friend WithEvents KJYU_L As System.Windows.Forms.Label
    Friend WithEvents FAX_N As System.Windows.Forms.TextBox
    Friend WithEvents FAX_L As System.Windows.Forms.Label
    Friend WithEvents YUUBIN_N As System.Windows.Forms.TextBox
    Friend WithEvents DENWA_N As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents DENWA_L As System.Windows.Forms.Label
    Friend WithEvents btnSit_Fuka_Sansyo As System.Windows.Forms.Button
    Friend WithEvents SIT_FUKA_N As System.Windows.Forms.TextBox
    Friend WithEvents EDA_L As System.Windows.Forms.Label
    Friend WithEvents TEIKEI_KBN_N As System.Windows.Forms.ComboBox
    Friend WithEvents TEIKEI_KBN_L As System.Windows.Forms.Label
    Friend WithEvents SIT_NNAME_N As System.Windows.Forms.TextBox
    Friend WithEvents SIT_KNAME_N As System.Windows.Forms.TextBox
    Friend WithEvents KIN_FUKA_N As System.Windows.Forms.TextBox
    Friend WithEvents SIT_NO_N As System.Windows.Forms.TextBox
    Friend WithEvents KIN_NNAME_N As System.Windows.Forms.TextBox
    Friend WithEvents KIN_KNAME_N As System.Windows.Forms.TextBox
    Friend WithEvents SIT_NNAME_L As System.Windows.Forms.Label
    Friend WithEvents SIT_KNAME_L As System.Windows.Forms.Label
    Friend WithEvents KIN_KNAME_L As System.Windows.Forms.Label
    Friend WithEvents KIN_NNAME_L As System.Windows.Forms.Label
    Friend WithEvents SIT_NO_L As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents KIN_NO_L As System.Windows.Forms.Label
    Friend WithEvents KIN_NO_N As System.Windows.Forms.TextBox
    Friend WithEvents btnKin_Fuka_Sansyo As System.Windows.Forms.Button
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TIKU_CODE_N As System.Windows.Forms.ComboBox
    Friend WithEvents SYUBETU_N As System.Windows.Forms.ComboBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TESUU_TANKA_N As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label

End Class
