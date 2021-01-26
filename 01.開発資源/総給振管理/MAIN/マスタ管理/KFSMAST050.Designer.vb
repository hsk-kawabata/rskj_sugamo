<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFSMAST050
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

        '税率ID
        AddHandler Me.txtTaxID.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtTaxID.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtTaxID.LostFocus, AddressOf CAST.LostFocus

        '振込手数料基準ID
        AddHandler Me.txtTesuuID.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtTesuuID.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtTesuuID.LostFocus, AddressOf CAST.LostFocus

        '振込手数料名
        AddHandler Me.txtTesuuName.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtTesuuName.KeyPress, AddressOf CASTx2.KeyPress
        AddHandler Me.txtTesuuName.LostFocus, AddressOf CAST.LostFocus

        '振込手数料A1
        AddHandler Me.txtTesuuA1F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA1F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA1F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料A2
        AddHandler Me.txtTesuuA2F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA2F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA2F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料A3
        AddHandler Me.txtTesuuA3F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA3F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA3F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料B1
        AddHandler Me.txtTesuuB1F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB1F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB1F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料B2
        AddHandler Me.txtTesuuB2F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB2F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB2F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料B3
        AddHandler Me.txtTesuuB3F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB3F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB3F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料C1
        AddHandler Me.txtTesuuC1F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC1F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC1F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料C2
        AddHandler Me.txtTesuuC2F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC2F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC2F.LostFocus, AddressOf CAST.LostFocusMoney

        '振込手数料C3
        AddHandler Me.txtTesuuC3F.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC3F.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC3F.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料A1
        AddHandler Me.txtTesuuA1S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA1S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA1S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料A2
        AddHandler Me.txtTesuuA2S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA2S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA2S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料A2
        AddHandler Me.txtTesuuA3S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA3S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA3S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料B1
        AddHandler Me.txtTesuuB1S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB1S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB1S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料B2
        AddHandler Me.txtTesuuB2S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB2S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB2S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料B2
        AddHandler Me.txtTesuuB3S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB3S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB3S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料C1
        AddHandler Me.txtTesuuC1S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC1S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC1S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料C2
        AddHandler Me.txtTesuuC2S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC2S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC2S.LostFocus, AddressOf CAST.LostFocusMoney

        '先振手数料C3
        AddHandler Me.txtTesuuC3S.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC3S.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC3S.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料A1
        AddHandler Me.txtTesuuA1K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA1K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA1K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料A2
        AddHandler Me.txtTesuuA2K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA2K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA2K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料A3
        AddHandler Me.txtTesuuA3K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuA3K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuA3K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料B1
        AddHandler Me.txtTesuuB1K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB1K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB1K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料B2
        AddHandler Me.txtTesuuB2K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB2K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB2K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料B3
        AddHandler Me.txtTesuuB3K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuB3K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuB3K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料C1
        AddHandler Me.txtTesuuC1K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC1K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC1K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料C2
        AddHandler Me.txtTesuuC2K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC2K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC2K.LostFocus, AddressOf CAST.LostFocusMoney

        '給振手数料C3
        AddHandler Me.txtTesuuC3K.GotFocus, AddressOf CAST.GotFocusMoney
        AddHandler Me.txtTesuuC3K.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler Me.txtTesuuC3K.LostFocus, AddressOf CAST.LostFocusMoney

        '手数料コンボボックス
        AddHandler Me.cmbTesuuName.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.cmbTesuuName.KeyPress, AddressOf CAST.KeyPress
        AddHandler Me.cmbTesuuName.LostFocus, AddressOf CAST.LostFocus

    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFSMAST050))
        Me.lbluser = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnSelect = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnAction = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtTesuuC3F = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.txtTesuuC2F = New System.Windows.Forms.TextBox()
        Me.txtTesuuB3F = New System.Windows.Forms.TextBox()
        Me.txtTesuuC1F = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtTesuuA3F = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtTesuuB2F = New System.Windows.Forms.TextBox()
        Me.txtTesuuB1F = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtTesuuA2F = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtTesuuA1F = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtTesuuC3S = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.txtTesuuC2S = New System.Windows.Forms.TextBox()
        Me.txtTesuuB3S = New System.Windows.Forms.TextBox()
        Me.txtTesuuC1S = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.txtTesuuA3S = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.txtTesuuB2S = New System.Windows.Forms.TextBox()
        Me.txtTesuuB1S = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.txtTesuuA2S = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.txtTesuuA1S = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.txtTesuuC3K = New System.Windows.Forms.TextBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.txtTesuuC2K = New System.Windows.Forms.TextBox()
        Me.txtTesuuB3K = New System.Windows.Forms.TextBox()
        Me.txtTesuuC1K = New System.Windows.Forms.TextBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.txtTesuuA3K = New System.Windows.Forms.TextBox()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.txtTesuuB2K = New System.Windows.Forms.TextBox()
        Me.txtTesuuB1K = New System.Windows.Forms.TextBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.txtTesuuA2K = New System.Windows.Forms.TextBox()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.txtTesuuA1K = New System.Windows.Forms.TextBox()
        Me.txtTesuuID = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.txtTesuuName = New System.Windows.Forms.TextBox()
        Me.txtTaxID = New System.Windows.Forms.TextBox()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.lblTax = New System.Windows.Forms.Label()
        Me.cmbTesuuName = New System.Windows.Forms.ComboBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
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
        Me.Label1.Text = "＜振込手数料マスタメンテナンス＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(682, 530)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(100, 32)
        Me.btnEnd.TabIndex = 12
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnUpdate
        '
        Me.btnUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(124, 530)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(100, 32)
        Me.btnUpdate.TabIndex = 7
        Me.btnUpdate.Text = "更　新"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(236, 530)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(100, 32)
        Me.btnDelete.TabIndex = 8
        Me.btnDelete.Text = "削　除"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnSelect
        '
        Me.btnSelect.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSelect.Location = New System.Drawing.Point(348, 530)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(100, 32)
        Me.btnSelect.TabIndex = 9
        Me.btnSelect.Text = "参　照"
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnClear.Location = New System.Drawing.Point(460, 530)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(100, 32)
        Me.btnClear.TabIndex = 10
        Me.btnClear.Text = "取　消"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(572, 530)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(100, 32)
        Me.btnPrint.TabIndex = 11
        Me.btnPrint.Text = "印　刷"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(12, 530)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(100, 32)
        Me.btnAction.TabIndex = 6
        Me.btnAction.Text = "登　録"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(321, 82)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(71, 15)
        Me.Label4.TabIndex = 59
        Me.Label4.Text = "手数料ID"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label20)
        Me.GroupBox1.Controls.Add(Me.Label19)
        Me.GroupBox1.Controls.Add(Me.Label16)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.txtTesuuC3F)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label17)
        Me.GroupBox1.Controls.Add(Me.txtTesuuC2F)
        Me.GroupBox1.Controls.Add(Me.txtTesuuB3F)
        Me.GroupBox1.Controls.Add(Me.txtTesuuC1F)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.txtTesuuA3F)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.txtTesuuB2F)
        Me.GroupBox1.Controls.Add(Me.txtTesuuB1F)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.txtTesuuA2F)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.txtTesuuA1F)
        Me.GroupBox1.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(54, 148)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(683, 120)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "振込手数料"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(272, 18)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(151, 15)
        Me.Label20.TabIndex = 64
        Me.Label20.Text = "1万円以上3万円未満"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(462, 18)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(79, 15)
        Me.Label19.TabIndex = 63
        Me.Label19.Text = "3万円以上"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(502, 92)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(39, 15)
        Me.Label16.TabIndex = 73
        Me.Label16.Text = "他行"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(312, 92)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(39, 15)
        Me.Label8.TabIndex = 73
        Me.Label8.Text = "他行"
        '
        'txtTesuuC3F
        '
        Me.txtTesuuC3F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC3F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC3F.Location = New System.Drawing.Point(547, 89)
        Me.txtTesuuC3F.MaxLength = 6
        Me.txtTesuuC3F.Name = "txtTesuuC3F"
        Me.txtTesuuC3F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC3F.TabIndex = 8
        Me.txtTesuuC3F.Text = "0"
        Me.txtTesuuC3F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(130, 92)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(39, 15)
        Me.Label7.TabIndex = 67
        Me.Label7.Text = "他行"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(486, 66)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(55, 15)
        Me.Label17.TabIndex = 71
        Me.Label17.Text = "本支店"
        '
        'txtTesuuC2F
        '
        Me.txtTesuuC2F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC2F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC2F.Location = New System.Drawing.Point(357, 89)
        Me.txtTesuuC2F.MaxLength = 6
        Me.txtTesuuC2F.Name = "txtTesuuC2F"
        Me.txtTesuuC2F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC2F.TabIndex = 7
        Me.txtTesuuC2F.Text = "0"
        Me.txtTesuuC2F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuB3F
        '
        Me.txtTesuuB3F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB3F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB3F.Location = New System.Drawing.Point(547, 63)
        Me.txtTesuuB3F.MaxLength = 6
        Me.txtTesuuB3F.Name = "txtTesuuB3F"
        Me.txtTesuuB3F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB3F.TabIndex = 5
        Me.txtTesuuB3F.Text = "0"
        Me.txtTesuuB3F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuC1F
        '
        Me.txtTesuuC1F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC1F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC1F.Location = New System.Drawing.Point(175, 89)
        Me.txtTesuuC1F.MaxLength = 6
        Me.txtTesuuC1F.Name = "txtTesuuC1F"
        Me.txtTesuuC1F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC1F.TabIndex = 6
        Me.txtTesuuC1F.Text = "0"
        Me.txtTesuuC1F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(502, 40)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(39, 15)
        Me.Label18.TabIndex = 69
        Me.Label18.Text = "自店"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(296, 66)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(55, 15)
        Me.Label9.TabIndex = 71
        Me.Label9.Text = "本支店"
        '
        'txtTesuuA3F
        '
        Me.txtTesuuA3F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA3F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA3F.Location = New System.Drawing.Point(547, 37)
        Me.txtTesuuA3F.MaxLength = 6
        Me.txtTesuuA3F.Name = "txtTesuuA3F"
        Me.txtTesuuA3F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA3F.TabIndex = 2
        Me.txtTesuuA3F.Text = "0"
        Me.txtTesuuA3F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(114, 66)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(55, 15)
        Me.Label6.TabIndex = 65
        Me.Label6.Text = "本支店"
        '
        'txtTesuuB2F
        '
        Me.txtTesuuB2F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB2F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB2F.Location = New System.Drawing.Point(357, 63)
        Me.txtTesuuB2F.MaxLength = 6
        Me.txtTesuuB2F.Name = "txtTesuuB2F"
        Me.txtTesuuB2F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB2F.TabIndex = 4
        Me.txtTesuuB2F.Text = "0"
        Me.txtTesuuB2F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuB1F
        '
        Me.txtTesuuB1F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB1F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB1F.Location = New System.Drawing.Point(175, 63)
        Me.txtTesuuB1F.MaxLength = 6
        Me.txtTesuuB1F.Name = "txtTesuuB1F"
        Me.txtTesuuB1F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB1F.TabIndex = 3
        Me.txtTesuuB1F.Text = "0"
        Me.txtTesuuB1F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(312, 40)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(39, 15)
        Me.Label10.TabIndex = 69
        Me.Label10.Text = "自店"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(130, 40)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(39, 15)
        Me.Label5.TabIndex = 63
        Me.Label5.Text = "自店"
        '
        'txtTesuuA2F
        '
        Me.txtTesuuA2F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA2F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA2F.Location = New System.Drawing.Point(357, 37)
        Me.txtTesuuA2F.MaxLength = 6
        Me.txtTesuuA2F.Name = "txtTesuuA2F"
        Me.txtTesuuA2F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA2F.TabIndex = 1
        Me.txtTesuuA2F.Text = "0"
        Me.txtTesuuA2F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(90, 18)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(79, 15)
        Me.Label11.TabIndex = 62
        Me.Label11.Text = "1万円未満"
        '
        'txtTesuuA1F
        '
        Me.txtTesuuA1F.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA1F.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA1F.Location = New System.Drawing.Point(175, 37)
        Me.txtTesuuA1F.MaxLength = 6
        Me.txtTesuuA1F.Name = "txtTesuuA1F"
        Me.txtTesuuA1F.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA1F.TabIndex = 0
        Me.txtTesuuA1F.Text = "0"
        Me.txtTesuuA1F.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.Label14)
        Me.GroupBox2.Controls.Add(Me.Label15)
        Me.GroupBox2.Controls.Add(Me.txtTesuuC3S)
        Me.GroupBox2.Controls.Add(Me.Label21)
        Me.GroupBox2.Controls.Add(Me.Label22)
        Me.GroupBox2.Controls.Add(Me.txtTesuuC2S)
        Me.GroupBox2.Controls.Add(Me.txtTesuuB3S)
        Me.GroupBox2.Controls.Add(Me.txtTesuuC1S)
        Me.GroupBox2.Controls.Add(Me.Label23)
        Me.GroupBox2.Controls.Add(Me.Label24)
        Me.GroupBox2.Controls.Add(Me.txtTesuuA3S)
        Me.GroupBox2.Controls.Add(Me.Label25)
        Me.GroupBox2.Controls.Add(Me.txtTesuuB2S)
        Me.GroupBox2.Controls.Add(Me.txtTesuuB1S)
        Me.GroupBox2.Controls.Add(Me.Label26)
        Me.GroupBox2.Controls.Add(Me.Label27)
        Me.GroupBox2.Controls.Add(Me.txtTesuuA2S)
        Me.GroupBox2.Controls.Add(Me.Label28)
        Me.GroupBox2.Controls.Add(Me.txtTesuuA1S)
        Me.GroupBox2.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(54, 274)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(683, 120)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "先振手数料"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(272, 18)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(151, 15)
        Me.Label12.TabIndex = 64
        Me.Label12.Text = "1万円以上3万円未満"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(462, 18)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(79, 15)
        Me.Label13.TabIndex = 63
        Me.Label13.Text = "3万円以上"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(502, 92)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(39, 15)
        Me.Label14.TabIndex = 73
        Me.Label14.Text = "他行"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(312, 92)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(39, 15)
        Me.Label15.TabIndex = 73
        Me.Label15.Text = "他行"
        '
        'txtTesuuC3S
        '
        Me.txtTesuuC3S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC3S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC3S.Location = New System.Drawing.Point(547, 89)
        Me.txtTesuuC3S.MaxLength = 6
        Me.txtTesuuC3S.Name = "txtTesuuC3S"
        Me.txtTesuuC3S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC3S.TabIndex = 8
        Me.txtTesuuC3S.Text = "0"
        Me.txtTesuuC3S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label21.Location = New System.Drawing.Point(130, 92)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(39, 15)
        Me.Label21.TabIndex = 67
        Me.Label21.Text = "他行"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label22.Location = New System.Drawing.Point(486, 66)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(55, 15)
        Me.Label22.TabIndex = 71
        Me.Label22.Text = "本支店"
        '
        'txtTesuuC2S
        '
        Me.txtTesuuC2S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC2S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC2S.Location = New System.Drawing.Point(357, 89)
        Me.txtTesuuC2S.MaxLength = 6
        Me.txtTesuuC2S.Name = "txtTesuuC2S"
        Me.txtTesuuC2S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC2S.TabIndex = 7
        Me.txtTesuuC2S.Text = "0"
        Me.txtTesuuC2S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuB3S
        '
        Me.txtTesuuB3S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB3S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB3S.Location = New System.Drawing.Point(547, 63)
        Me.txtTesuuB3S.MaxLength = 6
        Me.txtTesuuB3S.Name = "txtTesuuB3S"
        Me.txtTesuuB3S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB3S.TabIndex = 5
        Me.txtTesuuB3S.Text = "0"
        Me.txtTesuuB3S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuC1S
        '
        Me.txtTesuuC1S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC1S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC1S.Location = New System.Drawing.Point(175, 89)
        Me.txtTesuuC1S.MaxLength = 6
        Me.txtTesuuC1S.Name = "txtTesuuC1S"
        Me.txtTesuuC1S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC1S.TabIndex = 6
        Me.txtTesuuC1S.Text = "0"
        Me.txtTesuuC1S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label23.Location = New System.Drawing.Point(502, 40)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(39, 15)
        Me.Label23.TabIndex = 69
        Me.Label23.Text = "自店"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label24.Location = New System.Drawing.Point(296, 66)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(55, 15)
        Me.Label24.TabIndex = 71
        Me.Label24.Text = "本支店"
        '
        'txtTesuuA3S
        '
        Me.txtTesuuA3S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA3S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA3S.Location = New System.Drawing.Point(547, 37)
        Me.txtTesuuA3S.MaxLength = 6
        Me.txtTesuuA3S.Name = "txtTesuuA3S"
        Me.txtTesuuA3S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA3S.TabIndex = 2
        Me.txtTesuuA3S.Text = "0"
        Me.txtTesuuA3S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label25.Location = New System.Drawing.Point(114, 66)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(55, 15)
        Me.Label25.TabIndex = 65
        Me.Label25.Text = "本支店"
        '
        'txtTesuuB2S
        '
        Me.txtTesuuB2S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB2S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB2S.Location = New System.Drawing.Point(357, 63)
        Me.txtTesuuB2S.MaxLength = 6
        Me.txtTesuuB2S.Name = "txtTesuuB2S"
        Me.txtTesuuB2S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB2S.TabIndex = 4
        Me.txtTesuuB2S.Text = "0"
        Me.txtTesuuB2S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuB1S
        '
        Me.txtTesuuB1S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB1S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB1S.Location = New System.Drawing.Point(175, 63)
        Me.txtTesuuB1S.MaxLength = 6
        Me.txtTesuuB1S.Name = "txtTesuuB1S"
        Me.txtTesuuB1S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB1S.TabIndex = 3
        Me.txtTesuuB1S.Text = "0"
        Me.txtTesuuB1S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label26.Location = New System.Drawing.Point(312, 40)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(39, 15)
        Me.Label26.TabIndex = 69
        Me.Label26.Text = "自店"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label27.Location = New System.Drawing.Point(130, 40)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(39, 15)
        Me.Label27.TabIndex = 63
        Me.Label27.Text = "自店"
        '
        'txtTesuuA2S
        '
        Me.txtTesuuA2S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA2S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA2S.Location = New System.Drawing.Point(357, 37)
        Me.txtTesuuA2S.MaxLength = 6
        Me.txtTesuuA2S.Name = "txtTesuuA2S"
        Me.txtTesuuA2S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA2S.TabIndex = 1
        Me.txtTesuuA2S.Text = "0"
        Me.txtTesuuA2S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label28.Location = New System.Drawing.Point(90, 18)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(79, 15)
        Me.Label28.TabIndex = 62
        Me.Label28.Text = "1万円未満"
        '
        'txtTesuuA1S
        '
        Me.txtTesuuA1S.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA1S.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA1S.Location = New System.Drawing.Point(175, 37)
        Me.txtTesuuA1S.MaxLength = 6
        Me.txtTesuuA1S.Name = "txtTesuuA1S"
        Me.txtTesuuA1S.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA1S.TabIndex = 0
        Me.txtTesuuA1S.Text = "0"
        Me.txtTesuuA1S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label29)
        Me.GroupBox3.Controls.Add(Me.Label30)
        Me.GroupBox3.Controls.Add(Me.Label31)
        Me.GroupBox3.Controls.Add(Me.Label32)
        Me.GroupBox3.Controls.Add(Me.txtTesuuC3K)
        Me.GroupBox3.Controls.Add(Me.Label33)
        Me.GroupBox3.Controls.Add(Me.Label34)
        Me.GroupBox3.Controls.Add(Me.txtTesuuC2K)
        Me.GroupBox3.Controls.Add(Me.txtTesuuB3K)
        Me.GroupBox3.Controls.Add(Me.txtTesuuC1K)
        Me.GroupBox3.Controls.Add(Me.Label35)
        Me.GroupBox3.Controls.Add(Me.Label36)
        Me.GroupBox3.Controls.Add(Me.txtTesuuA3K)
        Me.GroupBox3.Controls.Add(Me.Label37)
        Me.GroupBox3.Controls.Add(Me.txtTesuuB2K)
        Me.GroupBox3.Controls.Add(Me.txtTesuuB1K)
        Me.GroupBox3.Controls.Add(Me.Label38)
        Me.GroupBox3.Controls.Add(Me.Label39)
        Me.GroupBox3.Controls.Add(Me.txtTesuuA2K)
        Me.GroupBox3.Controls.Add(Me.Label40)
        Me.GroupBox3.Controls.Add(Me.txtTesuuA1K)
        Me.GroupBox3.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(54, 400)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(683, 120)
        Me.GroupBox3.TabIndex = 5
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "給振手数料"
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label29.Location = New System.Drawing.Point(272, 18)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(151, 15)
        Me.Label29.TabIndex = 64
        Me.Label29.Text = "1万円以上3万円未満"
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label30.Location = New System.Drawing.Point(462, 18)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(79, 15)
        Me.Label30.TabIndex = 63
        Me.Label30.Text = "3万円以上"
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label31.Location = New System.Drawing.Point(502, 92)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(39, 15)
        Me.Label31.TabIndex = 73
        Me.Label31.Text = "他行"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label32.Location = New System.Drawing.Point(312, 92)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(39, 15)
        Me.Label32.TabIndex = 73
        Me.Label32.Text = "他行"
        '
        'txtTesuuC3K
        '
        Me.txtTesuuC3K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC3K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC3K.Location = New System.Drawing.Point(547, 89)
        Me.txtTesuuC3K.MaxLength = 6
        Me.txtTesuuC3K.Name = "txtTesuuC3K"
        Me.txtTesuuC3K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC3K.TabIndex = 8
        Me.txtTesuuC3K.Text = "0"
        Me.txtTesuuC3K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label33.Location = New System.Drawing.Point(130, 92)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(39, 15)
        Me.Label33.TabIndex = 67
        Me.Label33.Text = "他行"
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label34.Location = New System.Drawing.Point(486, 66)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(55, 15)
        Me.Label34.TabIndex = 71
        Me.Label34.Text = "本支店"
        '
        'txtTesuuC2K
        '
        Me.txtTesuuC2K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC2K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC2K.Location = New System.Drawing.Point(357, 89)
        Me.txtTesuuC2K.MaxLength = 6
        Me.txtTesuuC2K.Name = "txtTesuuC2K"
        Me.txtTesuuC2K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC2K.TabIndex = 7
        Me.txtTesuuC2K.Text = "0"
        Me.txtTesuuC2K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuB3K
        '
        Me.txtTesuuB3K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB3K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB3K.Location = New System.Drawing.Point(547, 63)
        Me.txtTesuuB3K.MaxLength = 6
        Me.txtTesuuB3K.Name = "txtTesuuB3K"
        Me.txtTesuuB3K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB3K.TabIndex = 5
        Me.txtTesuuB3K.Text = "0"
        Me.txtTesuuB3K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuC1K
        '
        Me.txtTesuuC1K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuC1K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuC1K.Location = New System.Drawing.Point(175, 89)
        Me.txtTesuuC1K.MaxLength = 6
        Me.txtTesuuC1K.Name = "txtTesuuC1K"
        Me.txtTesuuC1K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuC1K.TabIndex = 6
        Me.txtTesuuC1K.Text = "0"
        Me.txtTesuuC1K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label35.Location = New System.Drawing.Point(502, 40)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(39, 15)
        Me.Label35.TabIndex = 69
        Me.Label35.Text = "自店"
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label36.Location = New System.Drawing.Point(296, 66)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(55, 15)
        Me.Label36.TabIndex = 71
        Me.Label36.Text = "本支店"
        '
        'txtTesuuA3K
        '
        Me.txtTesuuA3K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA3K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA3K.Location = New System.Drawing.Point(547, 37)
        Me.txtTesuuA3K.MaxLength = 6
        Me.txtTesuuA3K.Name = "txtTesuuA3K"
        Me.txtTesuuA3K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA3K.TabIndex = 2
        Me.txtTesuuA3K.Text = "0"
        Me.txtTesuuA3K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label37
        '
        Me.Label37.AutoSize = True
        Me.Label37.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label37.Location = New System.Drawing.Point(114, 66)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(55, 15)
        Me.Label37.TabIndex = 65
        Me.Label37.Text = "本支店"
        '
        'txtTesuuB2K
        '
        Me.txtTesuuB2K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB2K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB2K.Location = New System.Drawing.Point(357, 63)
        Me.txtTesuuB2K.MaxLength = 6
        Me.txtTesuuB2K.Name = "txtTesuuB2K"
        Me.txtTesuuB2K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB2K.TabIndex = 4
        Me.txtTesuuB2K.Text = "0"
        Me.txtTesuuB2K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuB1K
        '
        Me.txtTesuuB1K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuB1K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuB1K.Location = New System.Drawing.Point(175, 63)
        Me.txtTesuuB1K.MaxLength = 6
        Me.txtTesuuB1K.Name = "txtTesuuB1K"
        Me.txtTesuuB1K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuB1K.TabIndex = 3
        Me.txtTesuuB1K.Text = "0"
        Me.txtTesuuB1K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label38.Location = New System.Drawing.Point(312, 40)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(39, 15)
        Me.Label38.TabIndex = 69
        Me.Label38.Text = "自店"
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label39.Location = New System.Drawing.Point(130, 40)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(39, 15)
        Me.Label39.TabIndex = 63
        Me.Label39.Text = "自店"
        '
        'txtTesuuA2K
        '
        Me.txtTesuuA2K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA2K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA2K.Location = New System.Drawing.Point(357, 37)
        Me.txtTesuuA2K.MaxLength = 6
        Me.txtTesuuA2K.Name = "txtTesuuA2K"
        Me.txtTesuuA2K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA2K.TabIndex = 1
        Me.txtTesuuA2K.Text = "0"
        Me.txtTesuuA2K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label40
        '
        Me.Label40.AutoSize = True
        Me.Label40.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label40.Location = New System.Drawing.Point(90, 18)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(79, 15)
        Me.Label40.TabIndex = 62
        Me.Label40.Text = "1万円未満"
        '
        'txtTesuuA1K
        '
        Me.txtTesuuA1K.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuA1K.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuA1K.Location = New System.Drawing.Point(175, 37)
        Me.txtTesuuA1K.MaxLength = 6
        Me.txtTesuuA1K.Name = "txtTesuuA1K"
        Me.txtTesuuA1K.Size = New System.Drawing.Size(70, 22)
        Me.txtTesuuA1K.TabIndex = 0
        Me.txtTesuuA1K.Text = "0"
        Me.txtTesuuA1K.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtTesuuID
        '
        Me.txtTesuuID.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuID.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTesuuID.Location = New System.Drawing.Point(398, 79)
        Me.txtTesuuID.MaxLength = 2
        Me.txtTesuuID.Name = "txtTesuuID"
        Me.txtTesuuID.Size = New System.Drawing.Size(35, 22)
        Me.txtTesuuID.TabIndex = 1
        Me.txtTesuuID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label41.Location = New System.Drawing.Point(130, 109)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(71, 15)
        Me.Label41.TabIndex = 63
        Me.Label41.Text = "手数料名"
        Me.Label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtTesuuName
        '
        Me.txtTesuuName.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTesuuName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.txtTesuuName.Location = New System.Drawing.Point(223, 106)
        Me.txtTesuuName.MaxLength = 50
        Me.txtTesuuName.Name = "txtTesuuName"
        Me.txtTesuuName.Size = New System.Drawing.Size(415, 22)
        Me.txtTesuuName.TabIndex = 2
        '
        'txtTaxID
        '
        Me.txtTaxID.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTaxID.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtTaxID.Location = New System.Drawing.Point(223, 78)
        Me.txtTaxID.MaxLength = 2
        Me.txtTaxID.Name = "txtTaxID"
        Me.txtTaxID.Size = New System.Drawing.Size(35, 22)
        Me.txtTaxID.TabIndex = 0
        Me.txtTaxID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label42.Location = New System.Drawing.Point(130, 81)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(55, 15)
        Me.Label42.TabIndex = 65
        Me.Label42.Text = "税率ID"
        Me.Label42.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTax
        '
        Me.lblTax.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTax.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTax.Location = New System.Drawing.Point(260, 78)
        Me.lblTax.Name = "lblTax"
        Me.lblTax.Size = New System.Drawing.Size(45, 22)
        Me.lblTax.TabIndex = 66
        Me.lblTax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cmbTesuuName
        '
        Me.cmbTesuuName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTesuuName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbTesuuName.Location = New System.Drawing.Point(223, 51)
        Me.cmbTesuuName.Name = "cmbTesuuName"
        Me.cmbTesuuName.Size = New System.Drawing.Size(415, 21)
        Me.cmbTesuuName.TabIndex = 68
        Me.cmbTesuuName.TabStop = False
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label43.Location = New System.Drawing.Point(130, 52)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(87, 15)
        Me.Label43.TabIndex = 69
        Me.Label43.Text = "手数料検索"
        Me.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'KFSMAST050
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.Label43)
        Me.Controls.Add(Me.cmbTesuuName)
        Me.Controls.Add(Me.lblTax)
        Me.Controls.Add(Me.Label42)
        Me.Controls.Add(Me.txtTaxID)
        Me.Controls.Add(Me.txtTesuuName)
        Me.Controls.Add(Me.Label41)
        Me.Controls.Add(Me.txtTesuuID)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.btnPrint)
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
        Me.Name = "KFSMAST050"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFSMAST050"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
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
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA1F As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuB1F As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuC1F As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuC3F As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuC2F As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuB3F As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA3F As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuB2F As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA2F As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuC3S As System.Windows.Forms.TextBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuC2S As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuB3S As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuC1S As System.Windows.Forms.TextBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA3S As System.Windows.Forms.TextBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuB2S As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuB1S As System.Windows.Forms.TextBox
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA2S As System.Windows.Forms.TextBox
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA1S As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuC3K As System.Windows.Forms.TextBox
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuC2K As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuB3K As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuC1K As System.Windows.Forms.TextBox
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA3K As System.Windows.Forms.TextBox
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuB2K As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuB1K As System.Windows.Forms.TextBox
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA2K As System.Windows.Forms.TextBox
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuA1K As System.Windows.Forms.TextBox
    Friend WithEvents txtTesuuID As System.Windows.Forms.TextBox
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents txtTesuuName As System.Windows.Forms.TextBox
    Friend WithEvents txtTaxID As System.Windows.Forms.TextBox
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents lblTax As System.Windows.Forms.Label
    Friend WithEvents cmbTesuuName As System.Windows.Forms.ComboBox
    Friend WithEvents Label43 As System.Windows.Forms.Label
End Class
