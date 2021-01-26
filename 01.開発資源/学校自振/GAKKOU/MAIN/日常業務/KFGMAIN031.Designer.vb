<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGMAIN031
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
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents lblFuriDateY As System.Windows.Forms.Label
    Friend WithEvents lblFuriDateM As System.Windows.Forms.Label
    Friend WithEvents lblFuriDateD As System.Windows.Forms.Label
    Friend WithEvents btnChkAll As System.Windows.Forms.Button
    Friend WithEvents btnClrChkAll As System.Windows.Forms.Button
    Friend WithEvents btnCreate As System.Windows.Forms.Button
    Friend WithEvents DataGridView As CustomDataGridView
    Friend WithEvents 選択 As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents 学校名 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 学校コード As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 振替日 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 請求年月 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 媒体 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 入出金 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 処理済 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 振替入出区分 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ファイル名称 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ソート順 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 保存ファイル名称 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents 帳票 As System.Windows.Forms.DataGridViewTextBoxColumn
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGMAIN031))
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblFuriDateY = New System.Windows.Forms.Label
        Me.lblFuriDateM = New System.Windows.Forms.Label
        Me.lblFuriDateD = New System.Windows.Forms.Label
        Me.btnCreate = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnChkAll = New System.Windows.Forms.Button
        Me.btnClrChkAll = New System.Windows.Forms.Button
        Me.DataGridView = New GAKKOU.CustomDataGridView
        Me.選択 = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.学校名 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.学校コード = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.振替日 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.請求年月 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.媒体 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.入出金 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.処理済 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.振替入出区分 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ファイル名称 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ソート順 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.保存ファイル名称 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.帳票 = New System.Windows.Forms.DataGridViewTextBoxColumn
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(294, 47)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 172
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(224, 47)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 171
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(154, 47)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 170
        Me.Label8.Text = "年"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(22, 47)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(72, 16)
        Me.Label6.TabIndex = 169
        Me.Label6.Text = "振 替 日"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 48)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 168
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 24)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 167
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 166
        Me.Label3.Text = "＜口座振替データ作成＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(592, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 165
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 164
        Me.Label1.Text = "ログイン名　:"
        '
        'lblFuriDateY
        '
        Me.lblFuriDateY.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateY.Location = New System.Drawing.Point(100, 44)
        Me.lblFuriDateY.Name = "lblFuriDateY"
        Me.lblFuriDateY.Size = New System.Drawing.Size(48, 23)
        Me.lblFuriDateY.TabIndex = 173
        Me.lblFuriDateY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblFuriDateM
        '
        Me.lblFuriDateM.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateM.Location = New System.Drawing.Point(184, 43)
        Me.lblFuriDateM.Name = "lblFuriDateM"
        Me.lblFuriDateM.Size = New System.Drawing.Size(34, 24)
        Me.lblFuriDateM.TabIndex = 174
        Me.lblFuriDateM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblFuriDateD
        '
        Me.lblFuriDateD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateD.Location = New System.Drawing.Point(254, 44)
        Me.lblFuriDateD.Name = "lblFuriDateD"
        Me.lblFuriDateD.Size = New System.Drawing.Size(34, 23)
        Me.lblFuriDateD.TabIndex = 175
        Me.lblFuriDateD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnCreate
        '
        Me.btnCreate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnCreate.Location = New System.Drawing.Point(140, 521)
        Me.btnCreate.Name = "btnCreate"
        Me.btnCreate.Size = New System.Drawing.Size(120, 40)
        Me.btnCreate.TabIndex = 177
        Me.btnCreate.Text = "作　成"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 521)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 176
        Me.btnEnd.Text = "終　了"
        '
        'btnChkAll
        '
        Me.btnChkAll.Font = New System.Drawing.Font("MS UI Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnChkAll.Location = New System.Drawing.Point(25, 75)
        Me.btnChkAll.Name = "btnChkAll"
        Me.btnChkAll.Size = New System.Drawing.Size(80, 30)
        Me.btnChkAll.TabIndex = 179
        Me.btnChkAll.Text = "全選択"
        '
        'btnClrChkAll
        '
        Me.btnClrChkAll.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnClrChkAll.Location = New System.Drawing.Point(111, 75)
        Me.btnClrChkAll.Name = "btnClrChkAll"
        Me.btnClrChkAll.Size = New System.Drawing.Size(80, 30)
        Me.btnClrChkAll.TabIndex = 180
        Me.btnClrChkAll.Text = "全解除"
        '
        'DataGridView
        '
        Me.DataGridView.AllowUserToAddRows = False
        Me.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.選択, Me.学校名, Me.学校コード, Me.振替日, Me.請求年月, Me.媒体, Me.入出金, Me.処理済, Me.振替入出区分, Me.ファイル名称, Me.ソート順, Me.保存ファイル名称, Me.帳票})
        Me.DataGridView.Location = New System.Drawing.Point(34, 113)
        Me.DataGridView.MultiSelect = False
        Me.DataGridView.Name = "DataGridView"
        Me.DataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders
        Me.DataGridView.RowTemplate.Height = 21
        Me.DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridView.Size = New System.Drawing.Size(726, 400)
        Me.DataGridView.TabIndex = 182
        '
        '選択
        '
        Me.選択.HeaderText = "選択"
        Me.選択.MinimumWidth = 40
        Me.選択.Name = "選択"
        Me.選択.Width = 40
        '
        '学校名
        '
        Me.学校名.HeaderText = "学校名"
        Me.学校名.MaxInputLength = 20
        Me.学校名.MinimumWidth = 210
        Me.学校名.Name = "学校名"
        Me.学校名.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.学校名.Width = 210
        '
        '学校コード
        '
        Me.学校コード.HeaderText = "学校コード"
        Me.学校コード.MaxInputLength = 10
        Me.学校コード.MinimumWidth = 100
        Me.学校コード.Name = "学校コード"
        Me.学校コード.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        '振替日
        '
        Me.振替日.HeaderText = "振替日"
        Me.振替日.MaxInputLength = 20
        Me.振替日.MinimumWidth = 70
        Me.振替日.Name = "振替日"
        Me.振替日.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.振替日.Width = 70
        '
        '請求年月
        '
        Me.請求年月.HeaderText = "請求年月"
        Me.請求年月.MaxInputLength = 10
        Me.請求年月.MinimumWidth = 80
        Me.請求年月.Name = "請求年月"
        Me.請求年月.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.請求年月.Width = 80
        '
        '媒体
        '
        Me.媒体.HeaderText = "媒体"
        Me.媒体.MaxInputLength = 20
        Me.媒体.MinimumWidth = 60
        Me.媒体.Name = "媒体"
        Me.媒体.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.媒体.Width = 60
        '
        '入出金
        '
        Me.入出金.HeaderText = "入出金"
        Me.入出金.MaxInputLength = 10
        Me.入出金.MinimumWidth = 70
        Me.入出金.Name = "入出金"
        Me.入出金.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.入出金.Width = 70
        '
        '処理済
        '
        Me.処理済.HeaderText = "処理済"
        Me.処理済.MaxInputLength = 10
        Me.処理済.MinimumWidth = 70
        Me.処理済.Name = "処理済"
        Me.処理済.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.処理済.Width = 70
        '
        '振替入出区分
        '
        Me.振替入出区分.HeaderText = "振替入出区分"
        Me.振替入出区分.MaxInputLength = 10
        Me.振替入出区分.Name = "振替入出区分"
        Me.振替入出区分.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.振替入出区分.Visible = False
        '
        'ファイル名称
        '
        Me.ファイル名称.HeaderText = "ファイル名称"
        Me.ファイル名称.MaxInputLength = 50
        Me.ファイル名称.Name = "ファイル名称"
        Me.ファイル名称.ReadOnly = True
        Me.ファイル名称.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.ファイル名称.Visible = False
        '
        'ソート順
        '
        Me.ソート順.HeaderText = "ソート順"
        Me.ソート順.MaxInputLength = 10
        Me.ソート順.Name = "ソート順"
        Me.ソート順.ReadOnly = True
        Me.ソート順.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.ソート順.Visible = False
        '
        '保存ファイル名称
        '
        Me.保存ファイル名称.HeaderText = "保存ファイル名称"
        Me.保存ファイル名称.MaxInputLength = 50
        Me.保存ファイル名称.Name = "保存ファイル名称"
        Me.保存ファイル名称.ReadOnly = True
        Me.保存ファイル名称.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.保存ファイル名称.Visible = False
        '
        '帳票
        '
        Me.帳票.HeaderText = "帳票"
        Me.帳票.MaxInputLength = 30
        Me.帳票.Name = "帳票"
        Me.帳票.ReadOnly = True
        Me.帳票.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.帳票.Visible = False
        '
        'KFGMAIN031
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.DataGridView)
        Me.Controls.Add(Me.btnClrChkAll)
        Me.Controls.Add(Me.btnChkAll)
        Me.Controls.Add(Me.btnCreate)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.lblFuriDateD)
        Me.Controls.Add(Me.lblFuriDateM)
        Me.Controls.Add(Me.lblFuriDateY)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
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
        Me.Name = "KFGMAIN031"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGMAIN031"
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

End Class
