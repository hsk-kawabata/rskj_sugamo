<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFOTMENU010
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

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        If GCom Is Nothing Then
            GCom = New MenteCommon.clsCommon
        End If
    End Sub

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    ' Windows フォーム デザイナを使って変更してください。  
    ' コード エディタは使用しないでください。
    Friend WithEvents tabpage2 As System.Windows.Forms.TabPage
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents TAB As System.Windows.Forms.TabControl
    Friend WithEvents tabpage1 As System.Windows.Forms.TabPage
    Friend WithEvents tabpage4 As System.Windows.Forms.TabPage
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tabpage3 As System.Windows.Forms.TabPage
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFOTMENU010))
        Me.TAB = New System.Windows.Forms.TabControl()
        Me.tabpage1 = New System.Windows.Forms.TabPage()
        Me.tabpage2 = New System.Windows.Forms.TabPage()
        Me.tabpage3 = New System.Windows.Forms.TabPage()
        Me.tabpage4 = New System.Windows.Forms.TabPage()
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TAB.SuspendLayout()
        Me.SuspendLayout()
        '
        'TAB
        '
        Me.TAB.Controls.Add(Me.tabpage1)
        Me.TAB.Controls.Add(Me.tabpage2)
        Me.TAB.Controls.Add(Me.tabpage3)
        Me.TAB.Controls.Add(Me.tabpage4)
        Me.TAB.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.TAB.ItemSize = New System.Drawing.Size(100, 20)
        Me.TAB.Location = New System.Drawing.Point(20, 55)
        Me.TAB.Name = "TAB"
        Me.TAB.Padding = New System.Drawing.Point(12, 3)
        Me.TAB.SelectedIndex = 0
        Me.TAB.Size = New System.Drawing.Size(755, 455)
        Me.TAB.TabIndex = 0
        '
        'tabpage1
        '
        Me.tabpage1.BackColor = System.Drawing.Color.Transparent
        Me.tabpage1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.tabpage1.Location = New System.Drawing.Point(4, 24)
        Me.tabpage1.Name = "tabpage1"
        Me.tabpage1.Size = New System.Drawing.Size(747, 427)
        Me.tabpage1.TabIndex = 1
        Me.tabpage1.Text = "日常業務"
        Me.tabpage1.UseVisualStyleBackColor = True
        '
        'tabpage2
        '
        Me.tabpage2.Location = New System.Drawing.Point(4, 24)
        Me.tabpage2.Name = "tabpage2"
        Me.tabpage2.Size = New System.Drawing.Size(747, 427)
        Me.tabpage2.TabIndex = 2
        Me.tabpage2.Text = "マスタ管理"
        Me.tabpage2.UseVisualStyleBackColor = True
        '
        'tabpage3
        '
        Me.tabpage3.Location = New System.Drawing.Point(4, 24)
        Me.tabpage3.Name = "tabpage3"
        Me.tabpage3.Size = New System.Drawing.Size(747, 427)
        Me.tabpage3.TabIndex = 3
        Me.tabpage3.Text = "随時処理"
        Me.tabpage3.UseVisualStyleBackColor = True
        '
        'tabpage4
        '
        Me.tabpage4.Location = New System.Drawing.Point(4, 24)
        Me.tabpage4.Name = "tabpage4"
        Me.tabpage4.Size = New System.Drawing.Size(747, 427)
        Me.tabpage4.TabIndex = 4
        Me.tabpage4.Text = "帳票印刷"
        Me.tabpage4.UseVisualStyleBackColor = True
        '
        'CmdBack
        '
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.CmdBack.Location = New System.Drawing.Point(615, 520)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(160, 40)
        Me.CmdBack.TabIndex = 1
        Me.CmdBack.Text = "メインメニュー"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 100
        Me.lblTitle.Text = "RELATIONSTAGE企業自振V2/運用試験"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(691, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 203
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(691, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 202
        Me.lblUser.Text = "admin"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(600, 27)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 201
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(600, 9)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 200
        Me.Label1.Text = "ログイン名　:"
        '
        'KFOTMENU010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.TAB)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFOTMENU010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFOTMENU010"
        Me.TAB.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class
