<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAIN051
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAIN051))
        Me.btnAction = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lstSCHLIST = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.lstSCHLIST, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(390, 278)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(80, 32)
        Me.btnAction.TabIndex = 91
        Me.btnAction.Text = " 実    行 "
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(119, 9)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(255, 16)
        Me.Label11.TabIndex = 103
        Me.Label11.Text = "＜日報集計（スケジュール選択）＞"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(103, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(291, 30)
        Me.Label1.TabIndex = 103
        Me.Label1.Text = "複数のスケジュールが存在します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "更新するスケジュールを選択してください。"
        '
        'lstSCHLIST
        '
        Me.lstSCHLIST.AllowUserToAddRows = False
        Me.lstSCHLIST.AllowUserToDeleteRows = False
        Me.lstSCHLIST.AllowUserToResizeColumns = False
        Me.lstSCHLIST.AllowUserToResizeRows = False
        Me.lstSCHLIST.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3, Me.Column4, Me.Column5})
        Me.lstSCHLIST.Location = New System.Drawing.Point(22, 65)
        Me.lstSCHLIST.MultiSelect = False
        Me.lstSCHLIST.Name = "lstSCHLIST"
        Me.lstSCHLIST.ReadOnly = True
        Me.lstSCHLIST.RowHeadersVisible = False
        Me.lstSCHLIST.RowTemplate.Height = 21
        Me.lstSCHLIST.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.lstSCHLIST.Size = New System.Drawing.Size(458, 207)
        Me.lstSCHLIST.TabIndex = 104
        '
        'Column1
        '
        Me.Column1.FillWeight = 85.0!
        Me.Column1.HeaderText = "取引先コード"
        Me.Column1.MinimumWidth = 85
        Me.Column1.Name = "Column1"
        Me.Column1.ReadOnly = True
        Me.Column1.Width = 85
        '
        'Column2
        '
        Me.Column2.FillWeight = 175.0!
        Me.Column2.HeaderText = "取引先名"
        Me.Column2.MinimumWidth = 175
        Me.Column2.Name = "Column2"
        Me.Column2.ReadOnly = True
        Me.Column2.Width = 175
        '
        'Column3
        '
        Me.Column3.FillWeight = 65.0!
        Me.Column3.HeaderText = "振替コード"
        Me.Column3.MinimumWidth = 65
        Me.Column3.Name = "Column3"
        Me.Column3.ReadOnly = True
        Me.Column3.Width = 65
        '
        'Column4
        '
        Me.Column4.FillWeight = 65.0!
        Me.Column4.HeaderText = "企業コード"
        Me.Column4.MinimumWidth = 65
        Me.Column4.Name = "Column4"
        Me.Column4.ReadOnly = True
        Me.Column4.Width = 65
        '
        'Column5
        '
        Me.Column5.FillWeight = 65.0!
        Me.Column5.HeaderText = "振替日"
        Me.Column5.MinimumWidth = 65
        Me.Column5.Name = "Column5"
        Me.Column5.ReadOnly = True
        Me.Column5.Width = 65
        '
        'KFJMAIN051
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(504, 322)
        Me.Controls.Add(Me.lstSCHLIST)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.btnAction)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(520, 360)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(520, 360)
        Me.Name = "KFJMAIN051"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAIN051"
        Me.TopMost = True
        CType(Me.lstSCHLIST, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lstSCHLIST As System.Windows.Forms.DataGridView
    Friend WithEvents Column1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Column5 As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
