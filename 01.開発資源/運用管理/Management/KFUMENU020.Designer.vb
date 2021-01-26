<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFUMENU020
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
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        If GCom Is Nothing Then
            GCom = New MenteCommon.clsCommon
        End If
    End Sub
    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFUMENU020))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.btnDailyJob12 = New System.Windows.Forms.Button()
        Me.btnDailyJob7 = New System.Windows.Forms.Button()
        Me.btnDailyJob16 = New System.Windows.Forms.Button()
        Me.btnDailyJob15 = New System.Windows.Forms.Button()
        Me.btnDailyJob14 = New System.Windows.Forms.Button()
        Me.btnDailyJob13 = New System.Windows.Forms.Button()
        Me.btnDailyJob11 = New System.Windows.Forms.Button()
        Me.btnDailyJob10 = New System.Windows.Forms.Button()
        Me.btnDailyJob9 = New System.Windows.Forms.Button()
        Me.btnDailyJob8 = New System.Windows.Forms.Button()
        Me.btnDailyJob6 = New System.Windows.Forms.Button()
        Me.btnDailyJob5 = New System.Windows.Forms.Button()
        Me.btnDailyJob4 = New System.Windows.Forms.Button()
        Me.btnDailyJob3 = New System.Windows.Forms.Button()
        Me.btnDailyJob2 = New System.Windows.Forms.Button()
        Me.btnDailyJob1 = New System.Windows.Forms.Button()
        Me.btnEND = New System.Windows.Forms.Button()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.SuspendLayout()
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
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabControl1.ItemSize = New System.Drawing.Size(100, 20)
        Me.TabControl1.Location = New System.Drawing.Point(20, 55)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.Padding = New System.Drawing.Point(12, 3)
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(755, 455)
        Me.TabControl1.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.Transparent
        Me.TabPage1.Controls.Add(Me.btnDailyJob12)
        Me.TabPage1.Controls.Add(Me.btnDailyJob7)
        Me.TabPage1.Controls.Add(Me.btnDailyJob16)
        Me.TabPage1.Controls.Add(Me.btnDailyJob15)
        Me.TabPage1.Controls.Add(Me.btnDailyJob14)
        Me.TabPage1.Controls.Add(Me.btnDailyJob13)
        Me.TabPage1.Controls.Add(Me.btnDailyJob11)
        Me.TabPage1.Controls.Add(Me.btnDailyJob10)
        Me.TabPage1.Controls.Add(Me.btnDailyJob9)
        Me.TabPage1.Controls.Add(Me.btnDailyJob8)
        Me.TabPage1.Controls.Add(Me.btnDailyJob6)
        Me.TabPage1.Controls.Add(Me.btnDailyJob5)
        Me.TabPage1.Controls.Add(Me.btnDailyJob4)
        Me.TabPage1.Controls.Add(Me.btnDailyJob3)
        Me.TabPage1.Controls.Add(Me.btnDailyJob2)
        Me.TabPage1.Controls.Add(Me.btnDailyJob1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 24)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(747, 427)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "運用管理"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'btnDailyJob12
        '
        Me.btnDailyJob12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob12.Location = New System.Drawing.Point(395, 170)
        Me.btnDailyJob12.Name = "btnDailyJob12"
        Me.btnDailyJob12.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob12.TabIndex = 11
        Me.btnDailyJob12.Tag = "1"
        Me.btnDailyJob12.Text = "消費税マスタメンテナンス"
        '
        'btnDailyJob7
        '
        Me.btnDailyJob7.Enabled = False
        Me.btnDailyJob7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob7.Location = New System.Drawing.Point(55, 320)
        Me.btnDailyJob7.Name = "btnDailyJob7"
        Me.btnDailyJob7.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob7.TabIndex = 6
        Me.btnDailyJob7.Tag = "1"
        Me.btnDailyJob7.Text = "提携区分マスタメンテナンス"
        Me.btnDailyJob7.Visible = False
        '
        'btnDailyJob16
        '
        Me.btnDailyJob16.Enabled = False
        Me.btnDailyJob16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob16.Location = New System.Drawing.Point(395, 370)
        Me.btnDailyJob16.Name = "btnDailyJob16"
        Me.btnDailyJob16.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob16.TabIndex = 15
        Me.btnDailyJob16.Tag = "0"
        Me.btnDailyJob16.Visible = False
        '
        'btnDailyJob15
        '
        Me.btnDailyJob15.Enabled = False
        Me.btnDailyJob15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob15.Location = New System.Drawing.Point(395, 320)
        Me.btnDailyJob15.Name = "btnDailyJob15"
        Me.btnDailyJob15.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob15.TabIndex = 14
        Me.btnDailyJob15.Tag = "0"
        Me.btnDailyJob15.Visible = False
        '
        'btnDailyJob14
        '
        Me.btnDailyJob14.Enabled = False
        Me.btnDailyJob14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob14.Location = New System.Drawing.Point(395, 270)
        Me.btnDailyJob14.Name = "btnDailyJob14"
        Me.btnDailyJob14.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob14.TabIndex = 13
        Me.btnDailyJob14.Tag = "0"
        Me.btnDailyJob14.Visible = False
        '
        'btnDailyJob13
        '
        Me.btnDailyJob13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob13.Location = New System.Drawing.Point(395, 220)
        Me.btnDailyJob13.Name = "btnDailyJob13"
        Me.btnDailyJob13.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob13.TabIndex = 12
        Me.btnDailyJob13.Tag = "1"
        Me.btnDailyJob13.Text = "印紙税マスタメンテナンス"
        '
        'btnDailyJob11
        '
        Me.btnDailyJob11.Enabled = False
        Me.btnDailyJob11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob11.Location = New System.Drawing.Point(395, 120)
        Me.btnDailyJob11.Name = "btnDailyJob11"
        Me.btnDailyJob11.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob11.TabIndex = 10
        Me.btnDailyJob11.Tag = "0"
        Me.btnDailyJob11.Visible = False
        '
        'btnDailyJob10
        '
        Me.btnDailyJob10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob10.Location = New System.Drawing.Point(395, 70)
        Me.btnDailyJob10.Name = "btnDailyJob10"
        Me.btnDailyJob10.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob10.TabIndex = 9
        Me.btnDailyJob10.Tag = "1"
        Me.btnDailyJob10.Text = "データ伝送ログ参照"
        '
        'btnDailyJob9
        '
        Me.btnDailyJob9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob9.Location = New System.Drawing.Point(395, 20)
        Me.btnDailyJob9.Name = "btnDailyJob9"
        Me.btnDailyJob9.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob9.TabIndex = 8
        Me.btnDailyJob9.Tag = "1"
        Me.btnDailyJob9.Text = "システムログ参照"
        '
        'btnDailyJob8
        '
        Me.btnDailyJob8.Enabled = False
        Me.btnDailyJob8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob8.Location = New System.Drawing.Point(55, 370)
        Me.btnDailyJob8.Name = "btnDailyJob8"
        Me.btnDailyJob8.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob8.TabIndex = 7
        Me.btnDailyJob8.Tag = "0"
        Me.btnDailyJob8.Visible = False
        '
        'btnDailyJob6
        '
        Me.btnDailyJob6.Enabled = False
        Me.btnDailyJob6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob6.Location = New System.Drawing.Point(55, 270)
        Me.btnDailyJob6.Name = "btnDailyJob6"
        Me.btnDailyJob6.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob6.TabIndex = 5
        Me.btnDailyJob6.Tag = "0"
        Me.btnDailyJob6.Visible = False
        '
        'btnDailyJob5
        '
        Me.btnDailyJob5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob5.Location = New System.Drawing.Point(55, 220)
        Me.btnDailyJob5.Name = "btnDailyJob5"
        Me.btnDailyJob5.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob5.TabIndex = 4
        Me.btnDailyJob5.Tag = "1"
        Me.btnDailyJob5.Text = "ログインパスワード変更"
        '
        'btnDailyJob4
        '
        Me.btnDailyJob4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob4.Location = New System.Drawing.Point(55, 170)
        Me.btnDailyJob4.Name = "btnDailyJob4"
        Me.btnDailyJob4.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob4.TabIndex = 3
        Me.btnDailyJob4.Tag = "1"
        Me.btnDailyJob4.Text = "ユーザ情報メンテナンス"
        '
        'btnDailyJob3
        '
        Me.btnDailyJob3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob3.Location = New System.Drawing.Point(55, 120)
        Me.btnDailyJob3.Name = "btnDailyJob3"
        Me.btnDailyJob3.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob3.TabIndex = 2
        Me.btnDailyJob3.Tag = "1"
        Me.btnDailyJob3.Text = "休日情報マスタメンテナンス"
        '
        'btnDailyJob2
        '
        Me.btnDailyJob2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob2.Location = New System.Drawing.Point(55, 70)
        Me.btnDailyJob2.Name = "btnDailyJob2"
        Me.btnDailyJob2.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob2.TabIndex = 1
        Me.btnDailyJob2.Tag = "1"
        Me.btnDailyJob2.Text = "金融機関マスタ更新"
        '
        'btnDailyJob1
        '
        Me.btnDailyJob1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDailyJob1.Location = New System.Drawing.Point(55, 20)
        Me.btnDailyJob1.Name = "btnDailyJob1"
        Me.btnDailyJob1.Size = New System.Drawing.Size(300, 40)
        Me.btnDailyJob1.TabIndex = 0
        Me.btnDailyJob1.Tag = "1"
        Me.btnDailyJob1.Text = "金融機関マスタメンテナンス"
        '
        'btnEND
        '
        Me.btnEND.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEND.Location = New System.Drawing.Point(615, 520)
        Me.btnEND.Name = "btnEND"
        Me.btnEND.Size = New System.Drawing.Size(160, 40)
        Me.btnEND.TabIndex = 59
        Me.btnEND.Text = "メインメニュー"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 61
        Me.lblTitle.Tag = "0"
        Me.lblTitle.Text = "RELATIONSTAGE企業自振V2/運用管理"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'KFUMENU020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.btnEND)
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
        Me.Name = "KFUMENU020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFUMENU020"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents btnDailyJob12 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob7 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob16 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob15 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob14 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob13 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob11 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob10 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob9 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob8 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob6 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob5 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob4 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob3 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob2 As System.Windows.Forms.Button
    Friend WithEvents btnDailyJob1 As System.Windows.Forms.Button
    Friend WithEvents btnEND As System.Windows.Forms.Button
    Friend WithEvents lblTitle As System.Windows.Forms.Label

End Class
