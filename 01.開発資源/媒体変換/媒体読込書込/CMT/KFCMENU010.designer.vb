<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFCMENU010
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
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents Button13 As System.Windows.Forms.Button
    Friend WithEvents Button11 As System.Windows.Forms.Button
    Friend WithEvents Button12 As System.Windows.Forms.Button
    Friend WithEvents Button10 As System.Windows.Forms.Button
    Friend WithEvents Button16 As System.Windows.Forms.Button
    Friend WithEvents Button9 As System.Windows.Forms.Button
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents Button14 As System.Windows.Forms.Button
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents Button15 As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFCMENU010))
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button9 = New System.Windows.Forms.Button()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.Button10 = New System.Windows.Forms.Button()
        Me.Button16 = New System.Windows.Forms.Button()
        Me.Button11 = New System.Windows.Forms.Button()
        Me.Button12 = New System.Windows.Forms.Button()
        Me.Button13 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.Button7 = New System.Windows.Forms.Button()
        Me.Button14 = New System.Windows.Forms.Button()
        Me.Button15 = New System.Windows.Forms.Button()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.TabControl1.ItemSize = New System.Drawing.Size(100, 20)
        Me.TabControl1.Location = New System.Drawing.Point(20, 55)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.Padding = New System.Drawing.Point(12, 3)
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(755, 455)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.Transparent
        Me.TabPage1.Controls.Add(Me.Button5)
        Me.TabPage1.Controls.Add(Me.Button4)
        Me.TabPage1.Controls.Add(Me.Button3)
        Me.TabPage1.Controls.Add(Me.Button1)
        Me.TabPage1.Controls.Add(Me.Button2)
        Me.TabPage1.Controls.Add(Me.Button9)
        Me.TabPage1.Controls.Add(Me.Button8)
        Me.TabPage1.Controls.Add(Me.Button10)
        Me.TabPage1.Controls.Add(Me.Button16)
        Me.TabPage1.Controls.Add(Me.Button11)
        Me.TabPage1.Controls.Add(Me.Button12)
        Me.TabPage1.Controls.Add(Me.Button13)
        Me.TabPage1.Controls.Add(Me.Button6)
        Me.TabPage1.Controls.Add(Me.Button7)
        Me.TabPage1.Controls.Add(Me.Button14)
        Me.TabPage1.Controls.Add(Me.Button15)
        Me.TabPage1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.TabPage1.Location = New System.Drawing.Point(4, 24)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(747, 427)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "媒体変換"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button5.Location = New System.Drawing.Point(53, 218)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(300, 40)
        Me.Button5.TabIndex = 4
        Me.Button5.Tag = "0"
        '
        'Button4
        '
        Me.Button4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button4.Location = New System.Drawing.Point(53, 168)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(300, 40)
        Me.Button4.TabIndex = 3
        Me.Button4.Tag = "0"
        '
        'Button3
        '
        Me.Button3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button3.Location = New System.Drawing.Point(53, 118)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(300, 40)
        Me.Button3.TabIndex = 2
        Me.Button3.Tag = "0"
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button1.Location = New System.Drawing.Point(53, 18)
        Me.Button1.Name = "Button1"
        Me.Button1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button1.Size = New System.Drawing.Size(300, 40)
        Me.Button1.TabIndex = 0
        Me.Button1.Tag = "1"
        Me.Button1.Text = "媒体読込（媒体→ディスク）"
        '
        'Button2
        '
        Me.Button2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button2.Location = New System.Drawing.Point(53, 68)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(300, 40)
        Me.Button2.TabIndex = 1
        Me.Button2.Tag = "1"
        Me.Button2.Text = "媒体書込（ディスク→媒体）"
        '
        'Button9
        '
        Me.Button9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button9.Location = New System.Drawing.Point(393, 18)
        Me.Button9.Name = "Button9"
        Me.Button9.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button9.Size = New System.Drawing.Size(300, 40)
        Me.Button9.TabIndex = 8
        Me.Button9.Tag = "0"
        Me.Button9.Text = "CMT読込（CMT→ディスク）"
        '
        'Button8
        '
        Me.Button8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button8.Location = New System.Drawing.Point(53, 368)
        Me.Button8.Name = "Button8"
        Me.Button8.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button8.Size = New System.Drawing.Size(300, 40)
        Me.Button8.TabIndex = 7
        Me.Button8.Tag = "0"
        '
        'Button10
        '
        Me.Button10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button10.Location = New System.Drawing.Point(393, 68)
        Me.Button10.Name = "Button10"
        Me.Button10.Size = New System.Drawing.Size(300, 40)
        Me.Button10.TabIndex = 9
        Me.Button10.Tag = "0"
        Me.Button10.Text = "CMT書込(ディスク→CMT）"
        '
        'Button16
        '
        Me.Button16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button16.Location = New System.Drawing.Point(393, 368)
        Me.Button16.Name = "Button16"
        Me.Button16.Size = New System.Drawing.Size(300, 40)
        Me.Button16.TabIndex = 15
        Me.Button16.Tag = "0"
        '
        'Button11
        '
        Me.Button11.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button11.Location = New System.Drawing.Point(393, 118)
        Me.Button11.Name = "Button11"
        Me.Button11.Size = New System.Drawing.Size(300, 40)
        Me.Button11.TabIndex = 10
        Me.Button11.Tag = "0"
        Me.Button11.Text = "他行分結果データ読込（CMT→ディスク）"
        '
        'Button12
        '
        Me.Button12.Font = New System.Drawing.Font("ＭＳ ゴシック", 10.0!)
        Me.Button12.Location = New System.Drawing.Point(393, 168)
        Me.Button12.Name = "Button12"
        Me.Button12.Size = New System.Drawing.Size(300, 40)
        Me.Button12.TabIndex = 11
        Me.Button12.Tag = "0"
        Me.Button12.Text = "他行分請求データ書込(ディスク→CMT）"
        '
        'Button13
        '
        Me.Button13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button13.Location = New System.Drawing.Point(393, 218)
        Me.Button13.Name = "Button13"
        Me.Button13.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button13.Size = New System.Drawing.Size(300, 40)
        Me.Button13.TabIndex = 12
        Me.Button13.Tag = "0"
        '
        'Button6
        '
        Me.Button6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button6.Location = New System.Drawing.Point(53, 268)
        Me.Button6.Name = "Button6"
        Me.Button6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button6.Size = New System.Drawing.Size(300, 40)
        Me.Button6.TabIndex = 5
        Me.Button6.Tag = "0"
        '
        'Button7
        '
        Me.Button7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button7.Location = New System.Drawing.Point(53, 318)
        Me.Button7.Name = "Button7"
        Me.Button7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button7.Size = New System.Drawing.Size(300, 40)
        Me.Button7.TabIndex = 6
        Me.Button7.Tag = "0"
        '
        'Button14
        '
        Me.Button14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button14.Location = New System.Drawing.Point(393, 268)
        Me.Button14.Name = "Button14"
        Me.Button14.Size = New System.Drawing.Size(300, 40)
        Me.Button14.TabIndex = 13
        Me.Button14.Tag = "0"
        '
        'Button15
        '
        Me.Button15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.Button15.Location = New System.Drawing.Point(393, 318)
        Me.Button15.Name = "Button15"
        Me.Button15.Size = New System.Drawing.Size(300, 40)
        Me.Button15.TabIndex = 14
        Me.Button15.Tag = "0"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(680, 40)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 5
        Me.lblDate.Text = "9999年99月99日"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(680, 16)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 3
        Me.lblUser.Text = "ユーザ名"
        Me.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(597, 40)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 12)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "システム日付："
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 12)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "ログイン名："
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmdBack
        '
        Me.CmdBack.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.CmdBack.Location = New System.Drawing.Point(615, 520)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(160, 40)
        Me.CmdBack.TabIndex = 1
        Me.CmdBack.Text = "メインメニュー"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 6
        Me.lblTitle.Tag = "0"
        Me.lblTitle.Text = "RELATIONSTAGE企業自振V2/媒体変換"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'KFCMENU010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("MS UI Gothic", 11.25!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFCMENU010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFCMENU010"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
End Class
