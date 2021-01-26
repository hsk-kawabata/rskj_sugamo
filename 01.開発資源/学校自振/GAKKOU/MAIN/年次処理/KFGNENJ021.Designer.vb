<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGNENJ021
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
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents labGAKKOU_CODE As System.Windows.Forms.Label
    Friend WithEvents lab学校名 As System.Windows.Forms.Label
    Friend WithEvents labGAKUNEN As System.Windows.Forms.Label
    Friend WithEvents labGAKUNENMEI As System.Windows.Forms.Label
    Friend WithEvents btnUpd As System.Windows.Forms.Button
    Friend WithEvents 削除 As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents 生徒番号 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 生徒氏名 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 性別 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 旧クラス As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 新クラス As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 新生徒番号 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 進学前学校名 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 進学前クラス As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 進学前生徒番号 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 入学年度 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 通番 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridView1 As CustomDataGridView
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGNENJ021))
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnUpd = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.labGAKKOU_CODE = New System.Windows.Forms.Label
        Me.lab学校名 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.labGAKUNEN = New System.Windows.Forms.Label
        Me.labGAKUNENMEI = New System.Windows.Forms.Label
        Me.DataGridView1 = New GAKKOU.CustomDataGridView
        Me.削除 = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.生徒番号 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.生徒氏名 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.性別 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.旧クラス = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.新クラス = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.新生徒番号 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.進学前学校名 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.進学前クラス = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.進学前生徒番号 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.入学年度 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.通番 = New System.Windows.Forms.DataGridViewTextBoxColumn
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(672, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 162
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(672, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 161
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 160
        Me.Label3.Text = "＜クラス替え入力＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(584, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 159
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(600, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 158
        Me.Label1.Text = "ログイン名　:"
        '
        'btnUpd
        '
        Me.btnUpd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpd.Location = New System.Drawing.Point(140, 521)
        Me.btnUpd.Name = "btnUpd"
        Me.btnUpd.Size = New System.Drawing.Size(120, 40)
        Me.btnUpd.TabIndex = 175
        Me.btnUpd.Text = "更　新"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 521)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 172
        Me.btnEnd.Text = "終　了"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(22, 47)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(87, 15)
        Me.Label11.TabIndex = 176
        Me.Label11.Text = "学校コード"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(22, 82)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(71, 15)
        Me.Label12.TabIndex = 177
        Me.Label12.Text = "学 校 名"
        '
        'labGAKKOU_CODE
        '
        Me.labGAKKOU_CODE.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.labGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labGAKKOU_CODE.Location = New System.Drawing.Point(115, 46)
        Me.labGAKKOU_CODE.Name = "labGAKKOU_CODE"
        Me.labGAKKOU_CODE.Size = New System.Drawing.Size(100, 23)
        Me.labGAKKOU_CODE.TabIndex = 178
        Me.labGAKKOU_CODE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lab学校名
        '
        Me.lab学校名.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校名.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校名.Location = New System.Drawing.Point(115, 78)
        Me.lab学校名.Name = "lab学校名"
        Me.lab学校名.Size = New System.Drawing.Size(443, 23)
        Me.lab学校名.TabIndex = 179
        Me.lab学校名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(577, 82)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(39, 15)
        Me.Label15.TabIndex = 180
        Me.Label15.Text = "学年"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'labGAKUNEN
        '
        Me.labGAKUNEN.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.labGAKUNEN.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labGAKUNEN.Location = New System.Drawing.Point(622, 78)
        Me.labGAKUNEN.Name = "labGAKUNEN"
        Me.labGAKUNEN.Size = New System.Drawing.Size(24, 23)
        Me.labGAKUNEN.TabIndex = 181
        Me.labGAKUNEN.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'labGAKUNENMEI
        '
        Me.labGAKUNENMEI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.labGAKUNENMEI.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labGAKUNENMEI.Location = New System.Drawing.Point(652, 78)
        Me.labGAKUNENMEI.Name = "labGAKUNENMEI"
        Me.labGAKUNENMEI.Size = New System.Drawing.Size(128, 23)
        Me.labGAKUNENMEI.TabIndex = 182
        Me.labGAKUNENMEI.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.削除, Me.生徒番号, Me.生徒氏名, Me.性別, Me.旧クラス, Me.新クラス, Me.新生徒番号, Me.進学前学校名, Me.進学前クラス, Me.進学前生徒番号, Me.入学年度, Me.通番})
        Me.DataGridView1.Location = New System.Drawing.Point(27, 124)
        Me.DataGridView1.MultiSelect = False
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders
        Me.DataGridView1.RowTemplate.Height = 21
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridView1.Size = New System.Drawing.Size(740, 385)
        Me.DataGridView1.TabIndex = 184
        '
        '削除
        '
        Me.削除.HeaderText = "削除"
        Me.削除.Name = "削除"
        Me.削除.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.削除.Width = 35
        '
        '生徒番号
        '
        Me.生徒番号.HeaderText = "生徒番号"
        Me.生徒番号.MaxInputLength = 7
        Me.生徒番号.Name = "生徒番号"
        Me.生徒番号.ReadOnly = True
        Me.生徒番号.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.生徒番号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.生徒番号.Width = 70
        '
        '生徒氏名
        '
        Me.生徒氏名.HeaderText = "生徒氏名"
        Me.生徒氏名.MaxInputLength = 100
        Me.生徒氏名.Name = "生徒氏名"
        Me.生徒氏名.ReadOnly = True
        Me.生徒氏名.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.生徒氏名.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.生徒氏名.Width = 200
        '
        '性別
        '
        Me.性別.HeaderText = "性別"
        Me.性別.MaxInputLength = 5
        Me.性別.Name = "性別"
        Me.性別.ReadOnly = True
        Me.性別.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.性別.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.性別.Width = 50
        '
        '旧クラス
        '
        Me.旧クラス.HeaderText = "旧クラス"
        Me.旧クラス.MaxInputLength = 2
        Me.旧クラス.Name = "旧クラス"
        Me.旧クラス.ReadOnly = True
        Me.旧クラス.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.旧クラス.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.旧クラス.Width = 55
        '
        '新クラス
        '
        Me.新クラス.HeaderText = "新クラス"
        Me.新クラス.MaxInputLength = 2
        Me.新クラス.Name = "新クラス"
        Me.新クラス.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.新クラス.Width = 65
        '
        '新生徒番号
        '
        Me.新生徒番号.HeaderText = "新生徒番号"
        Me.新生徒番号.MaxInputLength = 7
        Me.新生徒番号.Name = "新生徒番号"
        Me.新生徒番号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.新生徒番号.Width = 95
        '
        '進学前学校名
        '
        Me.進学前学校名.HeaderText = "進学前学校名"
        Me.進学前学校名.MaxInputLength = 100
        Me.進学前学校名.Name = "進学前学校名"
        Me.進学前学校名.ReadOnly = True
        Me.進学前学校名.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.進学前学校名.Visible = False
        Me.進学前学校名.Width = 170
        '
        '進学前クラス
        '
        Me.進学前クラス.HeaderText = "進学前クラス"
        Me.進学前クラス.MaxInputLength = 2
        Me.進学前クラス.Name = "進学前クラス"
        Me.進学前クラス.ReadOnly = True
        Me.進学前クラス.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.進学前クラス.Visible = False
        Me.進学前クラス.Width = 70
        '
        '進学前生徒番号
        '
        Me.進学前生徒番号.HeaderText = "進学前生徒番号"
        Me.進学前生徒番号.MaxInputLength = 7
        Me.進学前生徒番号.Name = "進学前生徒番号"
        Me.進学前生徒番号.ReadOnly = True
        Me.進学前生徒番号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.進学前生徒番号.Visible = False
        Me.進学前生徒番号.Width = 95
        '
        '入学年度
        '
        Me.入学年度.HeaderText = "入学年度"
        Me.入学年度.MaxInputLength = 4
        Me.入学年度.Name = "入学年度"
        Me.入学年度.ReadOnly = True
        Me.入学年度.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.入学年度.Width = 70
        '
        '通番
        '
        Me.通番.HeaderText = "通番"
        Me.通番.MaxInputLength = 4
        Me.通番.Name = "通番"
        Me.通番.ReadOnly = True
        Me.通番.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.通番.Width = 70
        '
        'KFGNENJ021
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.labGAKUNENMEI)
        Me.Controls.Add(Me.labGAKUNEN)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.lab学校名)
        Me.Controls.Add(Me.labGAKKOU_CODE)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.btnUpd)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGNENJ021"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGNENJ021"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '****************
        'DataGridView
        '****************
        AddHandler DataGridView1.EditingControlShowing, AddressOf EditingControlShowing
        AddHandler DataGridView1.CellEndEdit, AddressOf CellEndEdit
        AddHandler DataGridView1.CellEnter, AddressOf CellEnter
        AddHandler DataGridView1.CellLeave, AddressOf CellLeave

    End Sub

End Class
