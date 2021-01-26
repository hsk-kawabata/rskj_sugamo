<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAIN072
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
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents lab振替日 As System.Windows.Forms.Label
    Friend WithEvents lab学校コード As System.Windows.Forms.Label
    Friend WithEvents lab学校名 As System.Windows.Forms.Label
    Friend WithEvents lab入出金区分 As System.Windows.Forms.Label
    Friend WithEvents lab件数 As System.Windows.Forms.Label
    Friend WithEvents txt入力合計金額 As System.Windows.Forms.TextBox
    Friend WithEvents btnCreate As System.Windows.Forms.Button
    Friend WithEvents DataGridView As CustomDataGridView
    Friend WithEvents 入学年度 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 通番 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 学年 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents クラス As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 生徒番号 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 生徒名カナ As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 生徒名漢字 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 生徒名 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 金融機関コード As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 振替金融機関 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 支店コード As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 支店名 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 科目コード As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 科目 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 口座番号 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 契約者名 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 契約者番号 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 金額 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 手数料 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Label4 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAIN072))
        Me.btnCreate = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label6 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lab振替日 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.lab学校コード = New System.Windows.Forms.Label
        Me.lab学校名 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.lab件数 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.txt入力合計金額 = New System.Windows.Forms.TextBox
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.lab入出金区分 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.DataGridView = New GAKKOU.CustomDataGridView
        Me.入学年度 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.通番 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.学年 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.クラス = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.生徒番号 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.生徒名カナ = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.生徒名漢字 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.生徒名 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.金融機関コード = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.振替金融機関 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.支店コード = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.支店名 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.科目コード = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.科目 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.口座番号 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.契約者名 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.契約者番号 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.金額 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.手数料 = New System.Windows.Forms.DataGridViewTextBoxColumn
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCreate
        '
        Me.btnCreate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnCreate.Location = New System.Drawing.Point(140, 520)
        Me.btnCreate.Name = "btnCreate"
        Me.btnCreate.Size = New System.Drawing.Size(120, 40)
        Me.btnCreate.TabIndex = 0
        Me.btnCreate.TabStop = False
        Me.btnCreate.Text = "データ作成"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 0
        Me.btnEnd.TabStop = False
        Me.btnEnd.Text = "終　了"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(399, 70)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(71, 15)
        Me.Label6.TabIndex = 186
        Me.Label6.Text = "振 替 日"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 185
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 184
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 183
        Me.Label3.Text = "＜随時データ作成＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(592, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 182
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 181
        Me.Label1.Text = "ログイン名　:"
        '
        'lab振替日
        '
        Me.lab振替日.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab振替日.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab振替日.Location = New System.Drawing.Point(476, 65)
        Me.lab振替日.Name = "lab振替日"
        Me.lab振替日.Size = New System.Drawing.Size(96, 24)
        Me.lab振替日.TabIndex = 199
        Me.lab振替日.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(23, 70)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(87, 15)
        Me.Label7.TabIndex = 203
        Me.Label7.Text = "学校コード"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(23, 105)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(71, 15)
        Me.Label8.TabIndex = 204
        Me.Label8.Text = "学 校 名"
        '
        'lab学校コード
        '
        Me.lab学校コード.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校コード.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校コード.Location = New System.Drawing.Point(116, 65)
        Me.lab学校コード.Name = "lab学校コード"
        Me.lab学校コード.Size = New System.Drawing.Size(96, 24)
        Me.lab学校コード.TabIndex = 205
        Me.lab学校コード.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lab学校名
        '
        Me.lab学校名.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校名.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校名.Location = New System.Drawing.Point(116, 100)
        Me.lab学校名.Name = "lab学校名"
        Me.lab学校名.Size = New System.Drawing.Size(480, 24)
        Me.lab学校名.TabIndex = 206
        Me.lab学校名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(476, 485)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(39, 15)
        Me.Label12.TabIndex = 207
        Me.Label12.Text = "合計"
        '
        'lab件数
        '
        Me.lab件数.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab件数.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab件数.Location = New System.Drawing.Point(521, 480)
        Me.lab件数.Name = "lab件数"
        Me.lab件数.Size = New System.Drawing.Size(51, 24)
        Me.lab件数.TabIndex = 208
        Me.lab件数.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label14
        '
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(577, 485)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(26, 23)
        Me.Label14.TabIndex = 209
        Me.Label14.Text = "件"
        '
        'txt入力合計金額
        '
        Me.txt入力合計金額.Enabled = False
        Me.txt入力合計金額.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txt入力合計金額.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txt入力合計金額.Location = New System.Drawing.Point(617, 480)
        Me.txt入力合計金額.Name = "txt入力合計金額"
        Me.txt入力合計金額.Size = New System.Drawing.Size(128, 23)
        Me.txt入力合計金額.TabIndex = 210
        Me.txt入力合計金額.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label15
        '
        Me.Label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label15.Location = New System.Drawing.Point(521, 521)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(72, 24)
        Me.Label15.TabIndex = 211
        Me.Label15.Visible = False
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(628, 70)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(87, 15)
        Me.Label16.TabIndex = 212
        Me.Label16.Text = "入出金区分"
        '
        'lab入出金区分
        '
        Me.lab入出金区分.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab入出金区分.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab入出金区分.Location = New System.Drawing.Point(721, 65)
        Me.lab入出金区分.Name = "lab入出金区分"
        Me.lab入出金区分.Size = New System.Drawing.Size(48, 24)
        Me.lab入出金区分.TabIndex = 213
        Me.lab入出金区分.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(754, 482)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(26, 23)
        Me.Label4.TabIndex = 217
        Me.Label4.Text = "円"
        '
        'DataGridView
        '
        Me.DataGridView.AllowUserToAddRows = False
        Me.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.入学年度, Me.通番, Me.学年, Me.クラス, Me.生徒番号, Me.生徒名カナ, Me.生徒名漢字, Me.生徒名, Me.金融機関コード, Me.振替金融機関, Me.支店コード, Me.支店名, Me.科目コード, Me.科目, Me.口座番号, Me.契約者名, Me.契約者番号, Me.金額, Me.手数料})
        Me.DataGridView.Location = New System.Drawing.Point(12, 139)
        Me.DataGridView.MultiSelect = False
        Me.DataGridView.Name = "DataGridView"
        Me.DataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders
        Me.DataGridView.RowTemplate.Height = 21
        Me.DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridView.Size = New System.Drawing.Size(768, 335)
        Me.DataGridView.TabIndex = 218
        '
        '入学年度
        '
        Me.入学年度.HeaderText = "年度"
        Me.入学年度.MaxInputLength = 4
        Me.入学年度.Name = "入学年度"
        Me.入学年度.ReadOnly = True
        Me.入学年度.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.入学年度.Width = 40
        '
        '通番
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.通番.DefaultCellStyle = DataGridViewCellStyle1
        Me.通番.HeaderText = "通番"
        Me.通番.MaxInputLength = 4
        Me.通番.Name = "通番"
        Me.通番.ReadOnly = True
        Me.通番.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.通番.Width = 40
        '
        '学年
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.学年.DefaultCellStyle = DataGridViewCellStyle2
        Me.学年.HeaderText = "学"
        Me.学年.MaxInputLength = 1
        Me.学年.Name = "学年"
        Me.学年.ReadOnly = True
        Me.学年.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.学年.Width = 20
        '
        'クラス
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.クラス.DefaultCellStyle = DataGridViewCellStyle3
        Me.クラス.HeaderText = "ク"
        Me.クラス.MaxInputLength = 2
        Me.クラス.Name = "クラス"
        Me.クラス.ReadOnly = True
        Me.クラス.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.クラス.Width = 30
        '
        '生徒番号
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.生徒番号.DefaultCellStyle = DataGridViewCellStyle4
        Me.生徒番号.HeaderText = "生徒番号"
        Me.生徒番号.MaxInputLength = 7
        Me.生徒番号.Name = "生徒番号"
        Me.生徒番号.ReadOnly = True
        Me.生徒番号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.生徒番号.Width = 80
        '
        '生徒名カナ
        '
        Me.生徒名カナ.HeaderText = "生徒名カナ"
        Me.生徒名カナ.MaxInputLength = 40
        Me.生徒名カナ.Name = "生徒名カナ"
        Me.生徒名カナ.ReadOnly = True
        Me.生徒名カナ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.生徒名カナ.Visible = False
        Me.生徒名カナ.Width = 5
        '
        '生徒名漢字
        '
        Me.生徒名漢字.HeaderText = "生徒名漢字"
        Me.生徒名漢字.MaxInputLength = 40
        Me.生徒名漢字.Name = "生徒名漢字"
        Me.生徒名漢字.ReadOnly = True
        Me.生徒名漢字.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.生徒名漢字.Visible = False
        Me.生徒名漢字.Width = 5
        '
        '生徒名
        '
        Me.生徒名.HeaderText = "生徒名"
        Me.生徒名.MaxInputLength = 40
        Me.生徒名.Name = "生徒名"
        Me.生徒名.ReadOnly = True
        Me.生徒名.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.生徒名.Width = 80
        '
        '金融機関コード
        '
        Me.金融機関コード.HeaderText = "金融機関コード"
        Me.金融機関コード.MaxInputLength = 4
        Me.金融機関コード.Name = "金融機関コード"
        Me.金融機関コード.ReadOnly = True
        Me.金融機関コード.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.金融機関コード.Visible = False
        Me.金融機関コード.Width = 50
        '
        '振替金融機関
        '
        Me.振替金融機関.HeaderText = "振替金融機関"
        Me.振替金融機関.MaxInputLength = 40
        Me.振替金融機関.Name = "振替金融機関"
        Me.振替金融機関.ReadOnly = True
        Me.振替金融機関.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.振替金融機関.Width = 122
        '
        '支店コード
        '
        Me.支店コード.HeaderText = "支店コード"
        Me.支店コード.MaxInputLength = 3
        Me.支店コード.Name = "支店コード"
        Me.支店コード.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.支店コード.Visible = False
        '
        '支店名
        '
        Me.支店名.HeaderText = "支店名"
        Me.支店名.MaxInputLength = 40
        Me.支店名.Name = "支店名"
        Me.支店名.ReadOnly = True
        Me.支店名.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.支店名.Width = 80
        '
        '科目コード
        '
        Me.科目コード.HeaderText = "科目コード"
        Me.科目コード.MaxInputLength = 2
        Me.科目コード.Name = "科目コード"
        Me.科目コード.ReadOnly = True
        Me.科目コード.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.科目コード.Visible = False
        Me.科目コード.Width = 5
        '
        '科目
        '
        Me.科目.HeaderText = "科"
        Me.科目.MaxInputLength = 2
        Me.科目.Name = "科目"
        Me.科目.ReadOnly = True
        Me.科目.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.科目.Width = 22
        '
        '口座番号
        '
        Me.口座番号.HeaderText = "口番"
        Me.口座番号.MaxInputLength = 7
        Me.口座番号.Name = "口座番号"
        Me.口座番号.ReadOnly = True
        Me.口座番号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.口座番号.Width = 56
        '
        '契約者名
        '
        Me.契約者名.HeaderText = "契約者名"
        Me.契約者名.MaxInputLength = 40
        Me.契約者名.Name = "契約者名"
        Me.契約者名.ReadOnly = True
        Me.契約者名.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.契約者名.Width = 80
        '
        '契約者番号
        '
        Me.契約者番号.HeaderText = "契約者番号"
        Me.契約者番号.MaxInputLength = 40
        Me.契約者番号.Name = "契約者番号"
        Me.契約者番号.ReadOnly = True
        Me.契約者番号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.契約者番号.Visible = False
        Me.契約者番号.Width = 69
        '
        '金額
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.金額.DefaultCellStyle = DataGridViewCellStyle5
        Me.金額.HeaderText = "金額"
        Me.金額.MaxInputLength = 10
        Me.金額.Name = "金額"
        Me.金額.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.金額.Width = 90
        '
        '手数料
        '
        Me.手数料.HeaderText = "手数料"
        Me.手数料.MaxInputLength = 10
        Me.手数料.Name = "手数料"
        Me.手数料.ReadOnly = True
        Me.手数料.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.手数料.Visible = False
        Me.手数料.Width = 34
        '
        'KFGMAIN072
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.DataGridView)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lab入出金区分)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.txt入力合計金額)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.lab件数)
        Me.Controls.Add(Me.lab学校名)
        Me.Controls.Add(Me.lab学校コード)
        Me.Controls.Add(Me.lab振替日)
        Me.Controls.Add(Me.btnCreate)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGMAIN072"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAIN072"
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '****************
        'txt入力合計金額
        '****************
        AddHandler txt入力合計金額.GotFocus, AddressOf CASTx01.GotFocusMoney
        AddHandler txt入力合計金額.KeyPress, AddressOf CASTx01.KeyPressMoney
        AddHandler txt入力合計金額.LostFocus, AddressOf CASTx01.LostFocusMoney

        '****************
        'DataGridView
        '****************
        AddHandler DataGridView.EditingControlShowing, AddressOf EditingControlShowing
        AddHandler DataGridView.CellEndEdit, AddressOf CellEndEdit
        AddHandler DataGridView.CellEnter, AddressOf CellEnter
        AddHandler DataGridView.CellLeave, AddressOf CellLeave

    End Sub

End Class
