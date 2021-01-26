<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAST040
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '取引先主コード
        AddHandler txtTorisCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorisCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorisCode.LostFocus, AddressOf CASTx01.LostFocus

        '取引先副コード
        AddHandler txtTorifCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorifCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorifCode.LostFocus, AddressOf CASTx01.LostFocus

        'カナコンボボックス
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress

        '取引先名コンボボックス
        AddHandler cmbToriName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbToriName.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbToriName.KeyPress, AddressOf CAST.KeyPress

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAST040))
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnPrint = New System.Windows.Forms.Button
        Me.Label8 = New System.Windows.Forms.Label
        Me.txtKinNName20 = New System.Windows.Forms.TextBox
        Me.txtKinNName19 = New System.Windows.Forms.TextBox
        Me.txtKinNName18 = New System.Windows.Forms.TextBox
        Me.txtKinNName17 = New System.Windows.Forms.TextBox
        Me.txtKinNName16 = New System.Windows.Forms.TextBox
        Me.txtKinNName15 = New System.Windows.Forms.TextBox
        Me.txtKinNName14 = New System.Windows.Forms.TextBox
        Me.txtKinNName13 = New System.Windows.Forms.TextBox
        Me.txtKinNName12 = New System.Windows.Forms.TextBox
        Me.txtKinNName11 = New System.Windows.Forms.TextBox
        Me.txtKinCode20 = New System.Windows.Forms.TextBox
        Me.txtKinCode19 = New System.Windows.Forms.TextBox
        Me.txtKinCode18 = New System.Windows.Forms.TextBox
        Me.txtKinCode17 = New System.Windows.Forms.TextBox
        Me.txtKinCode16 = New System.Windows.Forms.TextBox
        Me.txtKinCode15 = New System.Windows.Forms.TextBox
        Me.txtKinCode14 = New System.Windows.Forms.TextBox
        Me.txtKinCode13 = New System.Windows.Forms.TextBox
        Me.txtKinCode12 = New System.Windows.Forms.TextBox
        Me.txtKinCode11 = New System.Windows.Forms.TextBox
        Me.btnTakouKin20 = New System.Windows.Forms.Button
        Me.btnTakouKin19 = New System.Windows.Forms.Button
        Me.btnTakouKin18 = New System.Windows.Forms.Button
        Me.btnTakouKin17 = New System.Windows.Forms.Button
        Me.btnTakouKin16 = New System.Windows.Forms.Button
        Me.btnTakouKin15 = New System.Windows.Forms.Button
        Me.btnTakouKin14 = New System.Windows.Forms.Button
        Me.btnTakouKin13 = New System.Windows.Forms.Button
        Me.btnTakouKin12 = New System.Windows.Forms.Button
        Me.btnTakouKin11 = New System.Windows.Forms.Button
        Me.txtKinNName10 = New System.Windows.Forms.TextBox
        Me.txtKinNName9 = New System.Windows.Forms.TextBox
        Me.txtKinNName8 = New System.Windows.Forms.TextBox
        Me.txtKinNName7 = New System.Windows.Forms.TextBox
        Me.txtKinNName6 = New System.Windows.Forms.TextBox
        Me.txtKinNName5 = New System.Windows.Forms.TextBox
        Me.txtKinNName4 = New System.Windows.Forms.TextBox
        Me.txtKinNName3 = New System.Windows.Forms.TextBox
        Me.txtKinNName2 = New System.Windows.Forms.TextBox
        Me.txtKinNName1 = New System.Windows.Forms.TextBox
        Me.txtKinCode10 = New System.Windows.Forms.TextBox
        Me.txtKinCode9 = New System.Windows.Forms.TextBox
        Me.txtKinCode8 = New System.Windows.Forms.TextBox
        Me.txtKinCode7 = New System.Windows.Forms.TextBox
        Me.txtKinCode6 = New System.Windows.Forms.TextBox
        Me.txtKinCode5 = New System.Windows.Forms.TextBox
        Me.txtKinCode4 = New System.Windows.Forms.TextBox
        Me.txtKinCode3 = New System.Windows.Forms.TextBox
        Me.txtKinCode2 = New System.Windows.Forms.TextBox
        Me.txtKinCode1 = New System.Windows.Forms.TextBox
        Me.btnTakouKin10 = New System.Windows.Forms.Button
        Me.btnTakouKin9 = New System.Windows.Forms.Button
        Me.btnTakouKin8 = New System.Windows.Forms.Button
        Me.btnTakouKin7 = New System.Windows.Forms.Button
        Me.btnTakouKin6 = New System.Windows.Forms.Button
        Me.btnTakouKin5 = New System.Windows.Forms.Button
        Me.btnTakouKin4 = New System.Windows.Forms.Button
        Me.btnTakouKin3 = New System.Windows.Forms.Button
        Me.btnTakouKin2 = New System.Windows.Forms.Button
        Me.btnTakouKin1 = New System.Windows.Forms.Button
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtTorifCode = New System.Windows.Forms.TextBox
        Me.txtTorisCode = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnSansyou = New System.Windows.Forms.Button
        Me.cmbToriName = New System.Windows.Forms.ComboBox
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(682, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 3
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 4
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(599, 27)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(615, 9)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ログイン名　:"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 16
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(270, 520)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(120, 40)
        Me.btnPrint.TabIndex = 15
        Me.btnPrint.Text = "印　刷"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(177, 138)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(353, 32)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = "※全取引先他行マスタ一覧を印刷したい場合は　取引先コード未入力で印刷してください"
        '
        'txtKinNName20
        '
        Me.txtKinNName20.BackColor = System.Drawing.Color.White
        Me.txtKinNName20.Enabled = False
        Me.txtKinNName20.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName20.Location = New System.Drawing.Point(496, 443)
        Me.txtKinNName20.MaxLength = 4
        Me.txtKinNName20.Name = "txtKinNName20"
        Me.txtKinNName20.ReadOnly = True
        Me.txtKinNName20.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName20.TabIndex = 77
        Me.txtKinNName20.TabStop = False
        '
        'txtKinNName19
        '
        Me.txtKinNName19.BackColor = System.Drawing.Color.White
        Me.txtKinNName19.Enabled = False
        Me.txtKinNName19.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName19.Location = New System.Drawing.Point(496, 419)
        Me.txtKinNName19.MaxLength = 4
        Me.txtKinNName19.Name = "txtKinNName19"
        Me.txtKinNName19.ReadOnly = True
        Me.txtKinNName19.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName19.TabIndex = 76
        Me.txtKinNName19.TabStop = False
        '
        'txtKinNName18
        '
        Me.txtKinNName18.BackColor = System.Drawing.Color.White
        Me.txtKinNName18.Enabled = False
        Me.txtKinNName18.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName18.Location = New System.Drawing.Point(496, 395)
        Me.txtKinNName18.MaxLength = 4
        Me.txtKinNName18.Name = "txtKinNName18"
        Me.txtKinNName18.ReadOnly = True
        Me.txtKinNName18.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName18.TabIndex = 75
        Me.txtKinNName18.TabStop = False
        '
        'txtKinNName17
        '
        Me.txtKinNName17.BackColor = System.Drawing.Color.White
        Me.txtKinNName17.Enabled = False
        Me.txtKinNName17.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName17.Location = New System.Drawing.Point(496, 371)
        Me.txtKinNName17.MaxLength = 4
        Me.txtKinNName17.Name = "txtKinNName17"
        Me.txtKinNName17.ReadOnly = True
        Me.txtKinNName17.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName17.TabIndex = 74
        Me.txtKinNName17.TabStop = False
        '
        'txtKinNName16
        '
        Me.txtKinNName16.BackColor = System.Drawing.Color.White
        Me.txtKinNName16.Enabled = False
        Me.txtKinNName16.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName16.Location = New System.Drawing.Point(496, 347)
        Me.txtKinNName16.MaxLength = 4
        Me.txtKinNName16.Name = "txtKinNName16"
        Me.txtKinNName16.ReadOnly = True
        Me.txtKinNName16.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName16.TabIndex = 73
        Me.txtKinNName16.TabStop = False
        '
        'txtKinNName15
        '
        Me.txtKinNName15.BackColor = System.Drawing.Color.White
        Me.txtKinNName15.Enabled = False
        Me.txtKinNName15.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName15.Location = New System.Drawing.Point(496, 323)
        Me.txtKinNName15.MaxLength = 4
        Me.txtKinNName15.Name = "txtKinNName15"
        Me.txtKinNName15.ReadOnly = True
        Me.txtKinNName15.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName15.TabIndex = 72
        Me.txtKinNName15.TabStop = False
        '
        'txtKinNName14
        '
        Me.txtKinNName14.BackColor = System.Drawing.Color.White
        Me.txtKinNName14.Enabled = False
        Me.txtKinNName14.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName14.Location = New System.Drawing.Point(496, 299)
        Me.txtKinNName14.MaxLength = 4
        Me.txtKinNName14.Name = "txtKinNName14"
        Me.txtKinNName14.ReadOnly = True
        Me.txtKinNName14.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName14.TabIndex = 71
        Me.txtKinNName14.TabStop = False
        '
        'txtKinNName13
        '
        Me.txtKinNName13.BackColor = System.Drawing.Color.White
        Me.txtKinNName13.Enabled = False
        Me.txtKinNName13.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName13.Location = New System.Drawing.Point(496, 275)
        Me.txtKinNName13.MaxLength = 4
        Me.txtKinNName13.Name = "txtKinNName13"
        Me.txtKinNName13.ReadOnly = True
        Me.txtKinNName13.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName13.TabIndex = 70
        Me.txtKinNName13.TabStop = False
        '
        'txtKinNName12
        '
        Me.txtKinNName12.BackColor = System.Drawing.Color.White
        Me.txtKinNName12.Enabled = False
        Me.txtKinNName12.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName12.Location = New System.Drawing.Point(496, 251)
        Me.txtKinNName12.MaxLength = 4
        Me.txtKinNName12.Name = "txtKinNName12"
        Me.txtKinNName12.ReadOnly = True
        Me.txtKinNName12.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName12.TabIndex = 69
        Me.txtKinNName12.TabStop = False
        '
        'txtKinNName11
        '
        Me.txtKinNName11.BackColor = System.Drawing.Color.White
        Me.txtKinNName11.Enabled = False
        Me.txtKinNName11.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName11.Location = New System.Drawing.Point(496, 227)
        Me.txtKinNName11.MaxLength = 30
        Me.txtKinNName11.Name = "txtKinNName11"
        Me.txtKinNName11.ReadOnly = True
        Me.txtKinNName11.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName11.TabIndex = 68
        Me.txtKinNName11.TabStop = False
        '
        'txtKinCode20
        '
        Me.txtKinCode20.BackColor = System.Drawing.Color.White
        Me.txtKinCode20.Enabled = False
        Me.txtKinCode20.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode20.Location = New System.Drawing.Point(448, 443)
        Me.txtKinCode20.MaxLength = 4
        Me.txtKinCode20.Name = "txtKinCode20"
        Me.txtKinCode20.ReadOnly = True
        Me.txtKinCode20.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode20.TabIndex = 67
        Me.txtKinCode20.TabStop = False
        '
        'txtKinCode19
        '
        Me.txtKinCode19.BackColor = System.Drawing.Color.White
        Me.txtKinCode19.Enabled = False
        Me.txtKinCode19.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode19.Location = New System.Drawing.Point(448, 419)
        Me.txtKinCode19.MaxLength = 4
        Me.txtKinCode19.Name = "txtKinCode19"
        Me.txtKinCode19.ReadOnly = True
        Me.txtKinCode19.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode19.TabIndex = 66
        Me.txtKinCode19.TabStop = False
        '
        'txtKinCode18
        '
        Me.txtKinCode18.BackColor = System.Drawing.Color.White
        Me.txtKinCode18.Enabled = False
        Me.txtKinCode18.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode18.Location = New System.Drawing.Point(448, 395)
        Me.txtKinCode18.MaxLength = 4
        Me.txtKinCode18.Name = "txtKinCode18"
        Me.txtKinCode18.ReadOnly = True
        Me.txtKinCode18.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode18.TabIndex = 65
        Me.txtKinCode18.TabStop = False
        '
        'txtKinCode17
        '
        Me.txtKinCode17.BackColor = System.Drawing.Color.White
        Me.txtKinCode17.Enabled = False
        Me.txtKinCode17.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode17.Location = New System.Drawing.Point(448, 371)
        Me.txtKinCode17.MaxLength = 4
        Me.txtKinCode17.Name = "txtKinCode17"
        Me.txtKinCode17.ReadOnly = True
        Me.txtKinCode17.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode17.TabIndex = 64
        Me.txtKinCode17.TabStop = False
        '
        'txtKinCode16
        '
        Me.txtKinCode16.BackColor = System.Drawing.Color.White
        Me.txtKinCode16.Enabled = False
        Me.txtKinCode16.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode16.Location = New System.Drawing.Point(448, 347)
        Me.txtKinCode16.MaxLength = 4
        Me.txtKinCode16.Name = "txtKinCode16"
        Me.txtKinCode16.ReadOnly = True
        Me.txtKinCode16.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode16.TabIndex = 63
        Me.txtKinCode16.TabStop = False
        '
        'txtKinCode15
        '
        Me.txtKinCode15.BackColor = System.Drawing.Color.White
        Me.txtKinCode15.Enabled = False
        Me.txtKinCode15.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode15.Location = New System.Drawing.Point(448, 323)
        Me.txtKinCode15.MaxLength = 4
        Me.txtKinCode15.Name = "txtKinCode15"
        Me.txtKinCode15.ReadOnly = True
        Me.txtKinCode15.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode15.TabIndex = 62
        Me.txtKinCode15.TabStop = False
        '
        'txtKinCode14
        '
        Me.txtKinCode14.BackColor = System.Drawing.Color.White
        Me.txtKinCode14.Enabled = False
        Me.txtKinCode14.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode14.Location = New System.Drawing.Point(448, 299)
        Me.txtKinCode14.MaxLength = 4
        Me.txtKinCode14.Name = "txtKinCode14"
        Me.txtKinCode14.ReadOnly = True
        Me.txtKinCode14.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode14.TabIndex = 61
        Me.txtKinCode14.TabStop = False
        '
        'txtKinCode13
        '
        Me.txtKinCode13.BackColor = System.Drawing.Color.White
        Me.txtKinCode13.Enabled = False
        Me.txtKinCode13.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode13.Location = New System.Drawing.Point(448, 275)
        Me.txtKinCode13.MaxLength = 4
        Me.txtKinCode13.Name = "txtKinCode13"
        Me.txtKinCode13.ReadOnly = True
        Me.txtKinCode13.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode13.TabIndex = 60
        Me.txtKinCode13.TabStop = False
        '
        'txtKinCode12
        '
        Me.txtKinCode12.BackColor = System.Drawing.Color.White
        Me.txtKinCode12.Enabled = False
        Me.txtKinCode12.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode12.Location = New System.Drawing.Point(448, 251)
        Me.txtKinCode12.MaxLength = 4
        Me.txtKinCode12.Name = "txtKinCode12"
        Me.txtKinCode12.ReadOnly = True
        Me.txtKinCode12.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode12.TabIndex = 59
        Me.txtKinCode12.TabStop = False
        '
        'txtKinCode11
        '
        Me.txtKinCode11.BackColor = System.Drawing.Color.White
        Me.txtKinCode11.Enabled = False
        Me.txtKinCode11.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode11.Location = New System.Drawing.Point(448, 227)
        Me.txtKinCode11.MaxLength = 4
        Me.txtKinCode11.Name = "txtKinCode11"
        Me.txtKinCode11.ReadOnly = True
        Me.txtKinCode11.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode11.TabIndex = 58
        Me.txtKinCode11.TabStop = False
        '
        'btnTakouKin20
        '
        Me.btnTakouKin20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin20.Location = New System.Drawing.Point(424, 443)
        Me.btnTakouKin20.Name = "btnTakouKin20"
        Me.btnTakouKin20.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin20.TabIndex = 57
        Me.btnTakouKin20.Tag = "20"
        '
        'btnTakouKin19
        '
        Me.btnTakouKin19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin19.Location = New System.Drawing.Point(424, 419)
        Me.btnTakouKin19.Name = "btnTakouKin19"
        Me.btnTakouKin19.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin19.TabIndex = 56
        Me.btnTakouKin19.Tag = "19"
        '
        'btnTakouKin18
        '
        Me.btnTakouKin18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin18.Location = New System.Drawing.Point(424, 395)
        Me.btnTakouKin18.Name = "btnTakouKin18"
        Me.btnTakouKin18.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin18.TabIndex = 55
        Me.btnTakouKin18.Tag = "18"
        '
        'btnTakouKin17
        '
        Me.btnTakouKin17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin17.Location = New System.Drawing.Point(424, 371)
        Me.btnTakouKin17.Name = "btnTakouKin17"
        Me.btnTakouKin17.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin17.TabIndex = 54
        Me.btnTakouKin17.Tag = "17"
        '
        'btnTakouKin16
        '
        Me.btnTakouKin16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin16.Location = New System.Drawing.Point(424, 347)
        Me.btnTakouKin16.Name = "btnTakouKin16"
        Me.btnTakouKin16.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin16.TabIndex = 53
        Me.btnTakouKin16.Tag = "16"
        '
        'btnTakouKin15
        '
        Me.btnTakouKin15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin15.Location = New System.Drawing.Point(424, 323)
        Me.btnTakouKin15.Name = "btnTakouKin15"
        Me.btnTakouKin15.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin15.TabIndex = 52
        Me.btnTakouKin15.Tag = "15"
        '
        'btnTakouKin14
        '
        Me.btnTakouKin14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin14.Location = New System.Drawing.Point(424, 299)
        Me.btnTakouKin14.Name = "btnTakouKin14"
        Me.btnTakouKin14.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin14.TabIndex = 51
        Me.btnTakouKin14.Tag = "14"
        '
        'btnTakouKin13
        '
        Me.btnTakouKin13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin13.Location = New System.Drawing.Point(424, 275)
        Me.btnTakouKin13.Name = "btnTakouKin13"
        Me.btnTakouKin13.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin13.TabIndex = 50
        Me.btnTakouKin13.Tag = "13"
        '
        'btnTakouKin12
        '
        Me.btnTakouKin12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin12.Location = New System.Drawing.Point(424, 251)
        Me.btnTakouKin12.Name = "btnTakouKin12"
        Me.btnTakouKin12.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin12.TabIndex = 49
        Me.btnTakouKin12.Tag = "12"
        '
        'btnTakouKin11
        '
        Me.btnTakouKin11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin11.Location = New System.Drawing.Point(424, 227)
        Me.btnTakouKin11.Name = "btnTakouKin11"
        Me.btnTakouKin11.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin11.TabIndex = 48
        Me.btnTakouKin11.Tag = "11"
        '
        'txtKinNName10
        '
        Me.txtKinNName10.BackColor = System.Drawing.Color.White
        Me.txtKinNName10.Enabled = False
        Me.txtKinNName10.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName10.Location = New System.Drawing.Point(216, 443)
        Me.txtKinNName10.MaxLength = 4
        Me.txtKinNName10.Name = "txtKinNName10"
        Me.txtKinNName10.ReadOnly = True
        Me.txtKinNName10.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName10.TabIndex = 47
        Me.txtKinNName10.TabStop = False
        '
        'txtKinNName9
        '
        Me.txtKinNName9.BackColor = System.Drawing.Color.White
        Me.txtKinNName9.Enabled = False
        Me.txtKinNName9.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName9.Location = New System.Drawing.Point(216, 419)
        Me.txtKinNName9.MaxLength = 4
        Me.txtKinNName9.Name = "txtKinNName9"
        Me.txtKinNName9.ReadOnly = True
        Me.txtKinNName9.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName9.TabIndex = 46
        Me.txtKinNName9.TabStop = False
        '
        'txtKinNName8
        '
        Me.txtKinNName8.BackColor = System.Drawing.Color.White
        Me.txtKinNName8.Enabled = False
        Me.txtKinNName8.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName8.Location = New System.Drawing.Point(216, 395)
        Me.txtKinNName8.MaxLength = 4
        Me.txtKinNName8.Name = "txtKinNName8"
        Me.txtKinNName8.ReadOnly = True
        Me.txtKinNName8.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName8.TabIndex = 45
        Me.txtKinNName8.TabStop = False
        '
        'txtKinNName7
        '
        Me.txtKinNName7.BackColor = System.Drawing.Color.White
        Me.txtKinNName7.Enabled = False
        Me.txtKinNName7.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName7.Location = New System.Drawing.Point(216, 371)
        Me.txtKinNName7.MaxLength = 4
        Me.txtKinNName7.Name = "txtKinNName7"
        Me.txtKinNName7.ReadOnly = True
        Me.txtKinNName7.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName7.TabIndex = 44
        Me.txtKinNName7.TabStop = False
        '
        'txtKinNName6
        '
        Me.txtKinNName6.BackColor = System.Drawing.Color.White
        Me.txtKinNName6.Enabled = False
        Me.txtKinNName6.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName6.Location = New System.Drawing.Point(216, 347)
        Me.txtKinNName6.MaxLength = 4
        Me.txtKinNName6.Name = "txtKinNName6"
        Me.txtKinNName6.ReadOnly = True
        Me.txtKinNName6.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName6.TabIndex = 43
        Me.txtKinNName6.TabStop = False
        '
        'txtKinNName5
        '
        Me.txtKinNName5.BackColor = System.Drawing.Color.White
        Me.txtKinNName5.Enabled = False
        Me.txtKinNName5.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName5.Location = New System.Drawing.Point(216, 323)
        Me.txtKinNName5.MaxLength = 4
        Me.txtKinNName5.Name = "txtKinNName5"
        Me.txtKinNName5.ReadOnly = True
        Me.txtKinNName5.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName5.TabIndex = 42
        Me.txtKinNName5.TabStop = False
        '
        'txtKinNName4
        '
        Me.txtKinNName4.BackColor = System.Drawing.Color.White
        Me.txtKinNName4.Enabled = False
        Me.txtKinNName4.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName4.Location = New System.Drawing.Point(216, 299)
        Me.txtKinNName4.MaxLength = 4
        Me.txtKinNName4.Name = "txtKinNName4"
        Me.txtKinNName4.ReadOnly = True
        Me.txtKinNName4.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName4.TabIndex = 41
        Me.txtKinNName4.TabStop = False
        '
        'txtKinNName3
        '
        Me.txtKinNName3.BackColor = System.Drawing.Color.White
        Me.txtKinNName3.Enabled = False
        Me.txtKinNName3.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName3.Location = New System.Drawing.Point(216, 275)
        Me.txtKinNName3.MaxLength = 4
        Me.txtKinNName3.Name = "txtKinNName3"
        Me.txtKinNName3.ReadOnly = True
        Me.txtKinNName3.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName3.TabIndex = 40
        Me.txtKinNName3.TabStop = False
        '
        'txtKinNName2
        '
        Me.txtKinNName2.BackColor = System.Drawing.Color.White
        Me.txtKinNName2.Enabled = False
        Me.txtKinNName2.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName2.Location = New System.Drawing.Point(216, 251)
        Me.txtKinNName2.MaxLength = 4
        Me.txtKinNName2.Name = "txtKinNName2"
        Me.txtKinNName2.ReadOnly = True
        Me.txtKinNName2.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName2.TabIndex = 39
        Me.txtKinNName2.TabStop = False
        '
        'txtKinNName1
        '
        Me.txtKinNName1.BackColor = System.Drawing.Color.White
        Me.txtKinNName1.Enabled = False
        Me.txtKinNName1.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinNName1.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtKinNName1.Location = New System.Drawing.Point(216, 227)
        Me.txtKinNName1.MaxLength = 30
        Me.txtKinNName1.Name = "txtKinNName1"
        Me.txtKinNName1.ReadOnly = True
        Me.txtKinNName1.Size = New System.Drawing.Size(192, 22)
        Me.txtKinNName1.TabIndex = 38
        Me.txtKinNName1.TabStop = False
        '
        'txtKinCode10
        '
        Me.txtKinCode10.BackColor = System.Drawing.Color.White
        Me.txtKinCode10.Enabled = False
        Me.txtKinCode10.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode10.Location = New System.Drawing.Point(168, 443)
        Me.txtKinCode10.MaxLength = 4
        Me.txtKinCode10.Name = "txtKinCode10"
        Me.txtKinCode10.ReadOnly = True
        Me.txtKinCode10.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode10.TabIndex = 37
        Me.txtKinCode10.TabStop = False
        '
        'txtKinCode9
        '
        Me.txtKinCode9.BackColor = System.Drawing.Color.White
        Me.txtKinCode9.Enabled = False
        Me.txtKinCode9.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode9.Location = New System.Drawing.Point(168, 419)
        Me.txtKinCode9.MaxLength = 4
        Me.txtKinCode9.Name = "txtKinCode9"
        Me.txtKinCode9.ReadOnly = True
        Me.txtKinCode9.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode9.TabIndex = 36
        Me.txtKinCode9.TabStop = False
        '
        'txtKinCode8
        '
        Me.txtKinCode8.BackColor = System.Drawing.Color.White
        Me.txtKinCode8.Enabled = False
        Me.txtKinCode8.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode8.Location = New System.Drawing.Point(168, 395)
        Me.txtKinCode8.MaxLength = 4
        Me.txtKinCode8.Name = "txtKinCode8"
        Me.txtKinCode8.ReadOnly = True
        Me.txtKinCode8.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode8.TabIndex = 35
        Me.txtKinCode8.TabStop = False
        '
        'txtKinCode7
        '
        Me.txtKinCode7.BackColor = System.Drawing.Color.White
        Me.txtKinCode7.Enabled = False
        Me.txtKinCode7.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode7.Location = New System.Drawing.Point(168, 371)
        Me.txtKinCode7.MaxLength = 4
        Me.txtKinCode7.Name = "txtKinCode7"
        Me.txtKinCode7.ReadOnly = True
        Me.txtKinCode7.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode7.TabIndex = 34
        Me.txtKinCode7.TabStop = False
        '
        'txtKinCode6
        '
        Me.txtKinCode6.BackColor = System.Drawing.Color.White
        Me.txtKinCode6.Enabled = False
        Me.txtKinCode6.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode6.Location = New System.Drawing.Point(168, 347)
        Me.txtKinCode6.MaxLength = 4
        Me.txtKinCode6.Name = "txtKinCode6"
        Me.txtKinCode6.ReadOnly = True
        Me.txtKinCode6.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode6.TabIndex = 33
        Me.txtKinCode6.TabStop = False
        '
        'txtKinCode5
        '
        Me.txtKinCode5.BackColor = System.Drawing.Color.White
        Me.txtKinCode5.Enabled = False
        Me.txtKinCode5.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode5.Location = New System.Drawing.Point(168, 323)
        Me.txtKinCode5.MaxLength = 4
        Me.txtKinCode5.Name = "txtKinCode5"
        Me.txtKinCode5.ReadOnly = True
        Me.txtKinCode5.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode5.TabIndex = 32
        Me.txtKinCode5.TabStop = False
        '
        'txtKinCode4
        '
        Me.txtKinCode4.BackColor = System.Drawing.Color.White
        Me.txtKinCode4.Enabled = False
        Me.txtKinCode4.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode4.Location = New System.Drawing.Point(168, 299)
        Me.txtKinCode4.MaxLength = 4
        Me.txtKinCode4.Name = "txtKinCode4"
        Me.txtKinCode4.ReadOnly = True
        Me.txtKinCode4.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode4.TabIndex = 31
        Me.txtKinCode4.TabStop = False
        '
        'txtKinCode3
        '
        Me.txtKinCode3.BackColor = System.Drawing.Color.White
        Me.txtKinCode3.Enabled = False
        Me.txtKinCode3.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode3.Location = New System.Drawing.Point(168, 275)
        Me.txtKinCode3.MaxLength = 4
        Me.txtKinCode3.Name = "txtKinCode3"
        Me.txtKinCode3.ReadOnly = True
        Me.txtKinCode3.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode3.TabIndex = 30
        Me.txtKinCode3.TabStop = False
        '
        'txtKinCode2
        '
        Me.txtKinCode2.BackColor = System.Drawing.Color.White
        Me.txtKinCode2.Enabled = False
        Me.txtKinCode2.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode2.Location = New System.Drawing.Point(168, 251)
        Me.txtKinCode2.MaxLength = 4
        Me.txtKinCode2.Name = "txtKinCode2"
        Me.txtKinCode2.ReadOnly = True
        Me.txtKinCode2.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode2.TabIndex = 29
        Me.txtKinCode2.TabStop = False
        '
        'txtKinCode1
        '
        Me.txtKinCode1.BackColor = System.Drawing.Color.White
        Me.txtKinCode1.Enabled = False
        Me.txtKinCode1.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKinCode1.Location = New System.Drawing.Point(168, 227)
        Me.txtKinCode1.MaxLength = 4
        Me.txtKinCode1.Name = "txtKinCode1"
        Me.txtKinCode1.ReadOnly = True
        Me.txtKinCode1.Size = New System.Drawing.Size(40, 22)
        Me.txtKinCode1.TabIndex = 28
        Me.txtKinCode1.TabStop = False
        '
        'btnTakouKin10
        '
        Me.btnTakouKin10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin10.Location = New System.Drawing.Point(144, 443)
        Me.btnTakouKin10.Name = "btnTakouKin10"
        Me.btnTakouKin10.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin10.TabIndex = 27
        Me.btnTakouKin10.Tag = "10"
        '
        'btnTakouKin9
        '
        Me.btnTakouKin9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin9.Location = New System.Drawing.Point(144, 419)
        Me.btnTakouKin9.Name = "btnTakouKin9"
        Me.btnTakouKin9.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin9.TabIndex = 26
        Me.btnTakouKin9.Tag = "9"
        '
        'btnTakouKin8
        '
        Me.btnTakouKin8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin8.Location = New System.Drawing.Point(144, 395)
        Me.btnTakouKin8.Name = "btnTakouKin8"
        Me.btnTakouKin8.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin8.TabIndex = 25
        Me.btnTakouKin8.Tag = "8"
        '
        'btnTakouKin7
        '
        Me.btnTakouKin7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin7.Location = New System.Drawing.Point(144, 371)
        Me.btnTakouKin7.Name = "btnTakouKin7"
        Me.btnTakouKin7.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin7.TabIndex = 24
        Me.btnTakouKin7.Tag = "7"
        '
        'btnTakouKin6
        '
        Me.btnTakouKin6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin6.Location = New System.Drawing.Point(144, 347)
        Me.btnTakouKin6.Name = "btnTakouKin6"
        Me.btnTakouKin6.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin6.TabIndex = 23
        Me.btnTakouKin6.Tag = "6"
        '
        'btnTakouKin5
        '
        Me.btnTakouKin5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin5.Location = New System.Drawing.Point(144, 323)
        Me.btnTakouKin5.Name = "btnTakouKin5"
        Me.btnTakouKin5.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin5.TabIndex = 22
        Me.btnTakouKin5.Tag = "5"
        '
        'btnTakouKin4
        '
        Me.btnTakouKin4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin4.Location = New System.Drawing.Point(144, 299)
        Me.btnTakouKin4.Name = "btnTakouKin4"
        Me.btnTakouKin4.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin4.TabIndex = 21
        Me.btnTakouKin4.Tag = "4"
        '
        'btnTakouKin3
        '
        Me.btnTakouKin3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin3.Location = New System.Drawing.Point(144, 275)
        Me.btnTakouKin3.Name = "btnTakouKin3"
        Me.btnTakouKin3.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin3.TabIndex = 20
        Me.btnTakouKin3.Tag = "3"
        '
        'btnTakouKin2
        '
        Me.btnTakouKin2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin2.Location = New System.Drawing.Point(144, 251)
        Me.btnTakouKin2.Name = "btnTakouKin2"
        Me.btnTakouKin2.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin2.TabIndex = 19
        Me.btnTakouKin2.Tag = "2"
        '
        'btnTakouKin1
        '
        Me.btnTakouKin1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTakouKin1.Location = New System.Drawing.Point(144, 227)
        Me.btnTakouKin1.Name = "btnTakouKin1"
        Me.btnTakouKin1.Size = New System.Drawing.Size(24, 24)
        Me.btnTakouKin1.TabIndex = 18
        Me.btnTakouKin1.Tag = "1"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(269, 106)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 10
        Me.Label7.Text = "－"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(103, 195)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(110, 16)
        Me.Label6.TabIndex = 13
        Me.Label6.Text = "金融機関情報"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(68, 102)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(101, 16)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "取引先コード"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(68, 73)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(88, 16)
        Me.Label10.TabIndex = 5
        Me.Label10.Text = "取引先検索"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "＜他行マスタメンテナンス＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtTorifCode
        '
        Me.txtTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorifCode.Location = New System.Drawing.Point(295, 102)
        Me.txtTorifCode.MaxLength = 2
        Me.txtTorifCode.Name = "txtTorifCode"
        Me.txtTorifCode.Size = New System.Drawing.Size(24, 22)
        Me.txtTorifCode.TabIndex = 11
        '
        'txtTorisCode
        '
        Me.txtTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorisCode.Location = New System.Drawing.Point(180, 102)
        Me.txtTorisCode.MaxLength = 10
        Me.txtTorisCode.Name = "txtTorisCode"
        Me.txtTorisCode.Size = New System.Drawing.Size(87, 22)
        Me.txtTorisCode.TabIndex = 9
        '
        'GroupBox1
        '
        Me.GroupBox1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(112, 197)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(608, 296)
        Me.GroupBox1.TabIndex = 17
        Me.GroupBox1.TabStop = False
        '
        'btnSansyou
        '
        Me.btnSansyou.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSansyou.Location = New System.Drawing.Point(140, 520)
        Me.btnSansyou.Name = "btnSansyou"
        Me.btnSansyou.Size = New System.Drawing.Size(120, 40)
        Me.btnSansyou.TabIndex = 14
        Me.btnSansyou.Text = "参　照"
        Me.btnSansyou.UseVisualStyleBackColor = True
        '
        'cmbToriName
        '
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbToriName.FormattingEnabled = True
        Me.cmbToriName.Location = New System.Drawing.Point(230, 73)
        Me.cmbToriName.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(355, 21)
        Me.cmbToriName.TabIndex = 7
        Me.cmbToriName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbKana.FormattingEnabled = True
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(181, 73)
        Me.cmbKana.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 6
        Me.cmbKana.TabStop = False
        '
        'KFJMAST040
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.cmbToriName)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtKinNName20)
        Me.Controls.Add(Me.txtKinNName19)
        Me.Controls.Add(Me.txtKinNName18)
        Me.Controls.Add(Me.txtKinNName17)
        Me.Controls.Add(Me.txtKinNName16)
        Me.Controls.Add(Me.txtKinNName15)
        Me.Controls.Add(Me.txtKinNName14)
        Me.Controls.Add(Me.txtKinNName13)
        Me.Controls.Add(Me.txtKinNName12)
        Me.Controls.Add(Me.txtKinNName11)
        Me.Controls.Add(Me.txtKinCode20)
        Me.Controls.Add(Me.txtKinCode19)
        Me.Controls.Add(Me.txtKinCode18)
        Me.Controls.Add(Me.txtKinCode17)
        Me.Controls.Add(Me.txtKinCode16)
        Me.Controls.Add(Me.txtKinCode15)
        Me.Controls.Add(Me.txtKinCode14)
        Me.Controls.Add(Me.txtKinCode13)
        Me.Controls.Add(Me.txtKinCode12)
        Me.Controls.Add(Me.txtKinCode11)
        Me.Controls.Add(Me.btnTakouKin20)
        Me.Controls.Add(Me.btnTakouKin19)
        Me.Controls.Add(Me.btnTakouKin18)
        Me.Controls.Add(Me.btnTakouKin17)
        Me.Controls.Add(Me.btnTakouKin16)
        Me.Controls.Add(Me.btnTakouKin15)
        Me.Controls.Add(Me.btnTakouKin14)
        Me.Controls.Add(Me.btnTakouKin13)
        Me.Controls.Add(Me.btnTakouKin12)
        Me.Controls.Add(Me.btnTakouKin11)
        Me.Controls.Add(Me.txtKinNName10)
        Me.Controls.Add(Me.txtKinNName9)
        Me.Controls.Add(Me.txtKinNName8)
        Me.Controls.Add(Me.txtKinNName7)
        Me.Controls.Add(Me.txtKinNName6)
        Me.Controls.Add(Me.txtKinNName5)
        Me.Controls.Add(Me.txtKinNName4)
        Me.Controls.Add(Me.txtKinNName3)
        Me.Controls.Add(Me.txtKinNName2)
        Me.Controls.Add(Me.txtKinNName1)
        Me.Controls.Add(Me.txtKinCode10)
        Me.Controls.Add(Me.txtKinCode9)
        Me.Controls.Add(Me.txtKinCode8)
        Me.Controls.Add(Me.txtKinCode7)
        Me.Controls.Add(Me.txtKinCode6)
        Me.Controls.Add(Me.txtKinCode5)
        Me.Controls.Add(Me.txtKinCode4)
        Me.Controls.Add(Me.txtKinCode3)
        Me.Controls.Add(Me.txtKinCode2)
        Me.Controls.Add(Me.txtKinCode1)
        Me.Controls.Add(Me.btnTakouKin10)
        Me.Controls.Add(Me.btnTakouKin9)
        Me.Controls.Add(Me.btnTakouKin8)
        Me.Controls.Add(Me.btnTakouKin7)
        Me.Controls.Add(Me.btnTakouKin6)
        Me.Controls.Add(Me.btnTakouKin5)
        Me.Controls.Add(Me.btnTakouKin4)
        Me.Controls.Add(Me.btnTakouKin3)
        Me.Controls.Add(Me.btnTakouKin2)
        Me.Controls.Add(Me.btnTakouKin1)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.txtTorifCode)
        Me.Controls.Add(Me.txtTorisCode)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnSansyou)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMAST040"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAST040"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtKinNName20 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName19 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName18 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName17 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName16 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName15 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName14 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName13 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName12 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName11 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode20 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode19 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode18 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode17 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode16 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode15 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode14 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode13 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode12 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode11 As System.Windows.Forms.TextBox
    Friend WithEvents btnTakouKin20 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin19 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin18 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin17 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin16 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin15 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin14 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin13 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin12 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin11 As System.Windows.Forms.Button
    Friend WithEvents txtKinNName10 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName9 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName8 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName7 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName6 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName5 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName4 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName3 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName2 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinNName1 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode10 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode9 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode8 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode7 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode6 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode5 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode4 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode3 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode2 As System.Windows.Forms.TextBox
    Friend WithEvents txtKinCode1 As System.Windows.Forms.TextBox
    Friend WithEvents btnTakouKin10 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin9 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin8 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin7 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin6 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin5 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin4 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin3 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin2 As System.Windows.Forms.Button
    Friend WithEvents btnTakouKin1 As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtTorifCode As System.Windows.Forms.TextBox
    Friend WithEvents txtTorisCode As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnSansyou As System.Windows.Forms.Button
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
End Class
