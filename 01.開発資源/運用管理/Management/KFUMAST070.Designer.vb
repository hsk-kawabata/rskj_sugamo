<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFUMAST070
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

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '印紙税ID
        AddHandler Me.txtInshizeiID.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtInshizeiID.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtInshizeiID.LostFocus, AddressOf CAST.LostFocus

        '印紙税１－１
        AddHandler Me.txtInshizei1_1.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtInshizei1_1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtInshizei1_1.LostFocus, AddressOf CAST.LostFocus
        '印紙税２－１
        AddHandler Me.txtInshizei2_1.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtInshizei2_1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtInshizei2_1.LostFocus, AddressOf CAST.LostFocus
        '印紙税２－２
        AddHandler Me.txtInshizei2_2.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtInshizei2_2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtInshizei2_2.LostFocus, AddressOf CAST.LostFocus
        '印紙税３－１
        AddHandler Me.txtInshizei3_1.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtInshizei3_1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtInshizei3_1.LostFocus, AddressOf CAST.LostFocus

        '適用開始日(年)
        AddHandler Me.txtKaishiDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtKaishiDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtKaishiDateY.LostFocus, AddressOf CAST.LostFocus

        '適用開始日(月)
        AddHandler Me.txtKaishiDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtKaishiDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtKaishiDateM.LostFocus, AddressOf CAST.LostFocus

        '適用開始日(日)
        AddHandler Me.txtKaishiDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtKaishiDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtKaishiDateD.LostFocus, AddressOf CAST.LostFocus

        '適用終了日(年)
        AddHandler Me.txtSyuryoDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtSyuryoDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtSyuryoDateY.LostFocus, AddressOf CAST.LostFocus

        '適用終了日(月)
        AddHandler Me.txtSyuryoDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtSyuryoDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtSyuryoDateM.LostFocus, AddressOf CAST.LostFocus

        '適用終了日(日)
        AddHandler Me.txtSyuryoDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtSyuryoDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtSyuryoDateD.LostFocus, AddressOf CAST.LostFocus

    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFUMAST070))
        Me.lbluser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnUpdate = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.btnSelect = New System.Windows.Forms.Button
        Me.btnClear = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.txtInshizeiID = New System.Windows.Forms.TextBox
        Me.txtInshizei1_1 = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtKaishiDateY = New System.Windows.Forms.TextBox
        Me.txtSyuryoDateY = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.txtKaishiDateM = New System.Windows.Forms.TextBox
        Me.txtSyuryoDateM = New System.Windows.Forms.TextBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.txtKaishiDateD = New System.Windows.Forms.TextBox
        Me.txtSyuryoDateD = New System.Windows.Forms.TextBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.txtInshizei2_1 = New System.Windows.Forms.TextBox
        Me.txtInshizei2_2 = New System.Windows.Forms.TextBox
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.Label20 = New System.Windows.Forms.Label
        Me.txtInshizei3_1 = New System.Windows.Forms.TextBox
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbluser
        '
        Me.lbluser.AutoSize = True
        Me.lbluser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lbluser.Location = New System.Drawing.Point(682, 9)
        Me.lbluser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lbluser.Name = "lbluser"
        Me.lbluser.Size = New System.Drawing.Size(35, 12)
        Me.lbluser.TabIndex = 32
        Me.lbluser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 31
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(599, 27)
        Me.Label3.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 12)
        Me.Label3.TabIndex = 30
        Me.Label3.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(615, 9)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 29
        Me.Label2.Text = "ログイン名　:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 34
        Me.Label1.Text = "＜印紙税マスタメンテナンス＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'btnUpdate
        '
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(140, 520)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(120, 40)
        Me.btnUpdate.TabIndex = 12
        Me.btnUpdate.Text = "更　新"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(270, 520)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(120, 40)
        Me.btnDelete.TabIndex = 13
        Me.btnDelete.Text = "削　除"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnSelect
        '
        Me.btnSelect.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSelect.Location = New System.Drawing.Point(400, 520)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(120, 40)
        Me.btnSelect.TabIndex = 14
        Me.btnSelect.Text = "参　照"
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnClear.Location = New System.Drawing.Point(530, 520)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(120, 40)
        Me.btnClear.TabIndex = 15
        Me.btnClear.Text = "取　消"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(10, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 11
        Me.btnAction.Text = "登　録"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(223, 74)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 59
        Me.Label4.Text = "印紙税ID"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtInshizeiID
        '
        Me.txtInshizeiID.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtInshizeiID.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtInshizeiID.Location = New System.Drawing.Point(356, 71)
        Me.txtInshizeiID.MaxLength = 2
        Me.txtInshizeiID.Name = "txtInshizeiID"
        Me.txtInshizeiID.Size = New System.Drawing.Size(35, 23)
        Me.txtInshizeiID.TabIndex = 0
        Me.txtInshizeiID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtInshizei1_1
        '
        Me.txtInshizei1_1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtInshizei1_1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtInshizei1_1.Location = New System.Drawing.Point(356, 100)
        Me.txtInshizei1_1.MaxLength = 7
        Me.txtInshizei1_1.Name = "txtInshizei1_1"
        Me.txtInshizei1_1.Size = New System.Drawing.Size(85, 23)
        Me.txtInshizei1_1.TabIndex = 1
        Me.txtInshizei1_1.Text = "0"
        Me.txtInshizei1_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(223, 190)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 16)
        Me.Label6.TabIndex = 62
        Me.Label6.Text = "適用開始日"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(223, 219)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(88, 16)
        Me.Label7.TabIndex = 63
        Me.Label7.Text = "適用終了日"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtKaishiDateY
        '
        Me.txtKaishiDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKaishiDateY.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtKaishiDateY.Location = New System.Drawing.Point(356, 187)
        Me.txtKaishiDateY.MaxLength = 4
        Me.txtKaishiDateY.Name = "txtKaishiDateY"
        Me.txtKaishiDateY.Size = New System.Drawing.Size(48, 23)
        Me.txtKaishiDateY.TabIndex = 5
        '
        'txtSyuryoDateY
        '
        Me.txtSyuryoDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSyuryoDateY.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtSyuryoDateY.Location = New System.Drawing.Point(356, 216)
        Me.txtSyuryoDateY.MaxLength = 4
        Me.txtSyuryoDateY.Name = "txtSyuryoDateY"
        Me.txtSyuryoDateY.Size = New System.Drawing.Size(48, 23)
        Me.txtSyuryoDateY.TabIndex = 8
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(410, 190)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 66
        Me.Label8.Text = "年"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(410, 219)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 67
        Me.Label9.Text = "年"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtKaishiDateM
        '
        Me.txtKaishiDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKaishiDateM.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtKaishiDateM.Location = New System.Drawing.Point(440, 187)
        Me.txtKaishiDateM.MaxLength = 2
        Me.txtKaishiDateM.Name = "txtKaishiDateM"
        Me.txtKaishiDateM.Size = New System.Drawing.Size(32, 23)
        Me.txtKaishiDateM.TabIndex = 6
        '
        'txtSyuryoDateM
        '
        Me.txtSyuryoDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSyuryoDateM.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtSyuryoDateM.Location = New System.Drawing.Point(440, 216)
        Me.txtSyuryoDateM.MaxLength = 2
        Me.txtSyuryoDateM.Name = "txtSyuryoDateM"
        Me.txtSyuryoDateM.Size = New System.Drawing.Size(32, 23)
        Me.txtSyuryoDateM.TabIndex = 9
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(478, 190)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 70
        Me.Label10.Text = "月"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(478, 219)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(24, 16)
        Me.Label11.TabIndex = 71
        Me.Label11.Text = "月"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtKaishiDateD
        '
        Me.txtKaishiDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKaishiDateD.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtKaishiDateD.Location = New System.Drawing.Point(508, 187)
        Me.txtKaishiDateD.MaxLength = 2
        Me.txtKaishiDateD.Name = "txtKaishiDateD"
        Me.txtKaishiDateD.Size = New System.Drawing.Size(32, 23)
        Me.txtKaishiDateD.TabIndex = 7
        '
        'txtSyuryoDateD
        '
        Me.txtSyuryoDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtSyuryoDateD.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtSyuryoDateD.Location = New System.Drawing.Point(508, 216)
        Me.txtSyuryoDateD.MaxLength = 2
        Me.txtSyuryoDateD.Name = "txtSyuryoDateD"
        Me.txtSyuryoDateD.Size = New System.Drawing.Size(32, 23)
        Me.txtSyuryoDateD.TabIndex = 10
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(546, 190)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(24, 16)
        Me.Label12.TabIndex = 74
        Me.Label12.Text = "日"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(546, 219)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(24, 16)
        Me.Label13.TabIndex = 75
        Me.Label13.Text = "日"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ListView1)
        Me.GroupBox1.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(84, 260)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(644, 222)
        Me.GroupBox1.TabIndex = 76
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "税率情報"
        '
        'ListView1
        '
        Me.ListView1.Location = New System.Drawing.Point(10, 31)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(620, 180)
        Me.ListView1.TabIndex = 0
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(223, 103)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 16)
        Me.Label5.TabIndex = 77
        Me.Label5.Text = "区分１"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(447, 103)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(56, 16)
        Me.Label14.TabIndex = 78
        Me.Label14.Text = "円未満"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(447, 132)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(56, 16)
        Me.Label15.TabIndex = 81
        Me.Label15.Text = "円以上"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(223, 132)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(56, 16)
        Me.Label16.TabIndex = 80
        Me.Label16.Text = "区分２"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtInshizei2_1
        '
        Me.txtInshizei2_1.Enabled = False
        Me.txtInshizei2_1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtInshizei2_1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtInshizei2_1.Location = New System.Drawing.Point(356, 129)
        Me.txtInshizei2_1.MaxLength = 7
        Me.txtInshizei2_1.Name = "txtInshizei2_1"
        Me.txtInshizei2_1.Size = New System.Drawing.Size(85, 23)
        Me.txtInshizei2_1.TabIndex = 2
        Me.txtInshizei2_1.Text = "0"
        Me.txtInshizei2_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtInshizei2_2
        '
        Me.txtInshizei2_2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtInshizei2_2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtInshizei2_2.Location = New System.Drawing.Point(509, 129)
        Me.txtInshizei2_2.MaxLength = 7
        Me.txtInshizei2_2.Name = "txtInshizei2_2"
        Me.txtInshizei2_2.Size = New System.Drawing.Size(85, 23)
        Me.txtInshizei2_2.TabIndex = 3
        Me.txtInshizei2_2.Text = "0"
        Me.txtInshizei2_2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(600, 132)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(56, 16)
        Me.Label17.TabIndex = 83
        Me.Label17.Text = "円未満"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(447, 161)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(56, 16)
        Me.Label19.TabIndex = 86
        Me.Label19.Text = "円以上"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(223, 161)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(56, 16)
        Me.Label20.TabIndex = 85
        Me.Label20.Text = "区分３"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtInshizei3_1
        '
        Me.txtInshizei3_1.Enabled = False
        Me.txtInshizei3_1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtInshizei3_1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtInshizei3_1.Location = New System.Drawing.Point(356, 158)
        Me.txtInshizei3_1.MaxLength = 7
        Me.txtInshizei3_1.Name = "txtInshizei3_1"
        Me.txtInshizei3_1.Size = New System.Drawing.Size(85, 23)
        Me.txtInshizei3_1.TabIndex = 4
        Me.txtInshizei3_1.Text = "0"
        Me.txtInshizei3_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'KFUMAST070
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.txtInshizei3_1)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.txtInshizei2_2)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.txtInshizei2_1)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.txtSyuryoDateD)
        Me.Controls.Add(Me.txtKaishiDateD)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.txtSyuryoDateM)
        Me.Controls.Add(Me.txtKaishiDateM)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtSyuryoDateY)
        Me.Controls.Add(Me.txtKaishiDateY)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtInshizei1_1)
        Me.Controls.Add(Me.txtInshizeiID)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnSelect)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.lbluser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFUMAST070"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFUMAST070"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbluser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnSelect As System.Windows.Forms.Button
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtInshizeiID As System.Windows.Forms.TextBox
    Friend WithEvents txtInshizei1_1 As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtKaishiDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtSyuryoDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtKaishiDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtSyuryoDateM As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtKaishiDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtSyuryoDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtInshizei2_1 As System.Windows.Forms.TextBox
    Friend WithEvents txtInshizei2_2 As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents txtInshizei3_1 As System.Windows.Forms.TextBox
End Class
