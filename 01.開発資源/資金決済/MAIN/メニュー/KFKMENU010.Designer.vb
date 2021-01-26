<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFKMENU010
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        If GCom Is Nothing Then
            GCom = New MenteCommon.clsCommon
        End If
    End Sub

    ' Form は dispose をオーバーライドしてコンポーネント一覧を消去します。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    ' Windows フォーム デザイナを使って変更してください。  
    ' コード エディタは使用しないでください。
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents TmrForm As System.Windows.Forms.Timer
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents CmdAction12 As System.Windows.Forms.Button
    Friend WithEvents CmdAction02 As System.Windows.Forms.Button
    Friend WithEvents CmdAction01 As System.Windows.Forms.Button
    Friend WithEvents CmdAction03 As System.Windows.Forms.Button
    Friend WithEvents CmdAction10 As System.Windows.Forms.Button
    Friend WithEvents CmdAction04 As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents CmdAction05 As System.Windows.Forms.Button
    Friend WithEvents CmdAction11 As System.Windows.Forms.Button
    Friend WithEvents CmdAction14 As System.Windows.Forms.Button
    Friend WithEvents CmdAction15 As System.Windows.Forms.Button
    Friend WithEvents CmdAction16 As System.Windows.Forms.Button
    Friend WithEvents CmdAction17 As System.Windows.Forms.Button
    Friend WithEvents CmdAction08 As System.Windows.Forms.Button
    Friend WithEvents CmdAction07 As System.Windows.Forms.Button
    Friend WithEvents CmdAction06 As System.Windows.Forms.Button
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents CmdAction13 As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFKMENU010))
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.TmrForm = New System.Windows.Forms.Timer(Me.components)
        Me.CmdAction02 = New System.Windows.Forms.Button()
        Me.CmdAction01 = New System.Windows.Forms.Button()
        Me.CmdAction03 = New System.Windows.Forms.Button()
        Me.CmdAction10 = New System.Windows.Forms.Button()
        Me.CmdAction04 = New System.Windows.Forms.Button()
        Me.CmdAction12 = New System.Windows.Forms.Button()
        Me.CmdAction13 = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.CmdAction11 = New System.Windows.Forms.Button()
        Me.CmdAction14 = New System.Windows.Forms.Button()
        Me.CmdAction15 = New System.Windows.Forms.Button()
        Me.CmdAction16 = New System.Windows.Forms.Button()
        Me.CmdAction17 = New System.Windows.Forms.Button()
        Me.CmdAction08 = New System.Windows.Forms.Button()
        Me.CmdAction07 = New System.Windows.Forms.Button()
        Me.CmdAction06 = New System.Windows.Forms.Button()
        Me.CmdAction05 = New System.Windows.Forms.Button()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.SuspendLayout()
        '
        'CmdBack
        '
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdBack.Location = New System.Drawing.Point(615, 520)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(160, 40)
        Me.CmdBack.TabIndex = 17
        Me.CmdBack.Tag = ""
        Me.CmdBack.Text = "メインメニュー"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(615, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "ログイン名　:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(601, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "システム日付:"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(685, 8)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 5
        Me.lblUser.Text = "ユーザ名"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(685, 28)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 6
        Me.lblDate.Text = "9999年99月99日"
        '
        'TmrForm
        '
        Me.TmrForm.Enabled = True
        '
        'CmdAction02
        '
        Me.CmdAction02.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction02.Location = New System.Drawing.Point(55, 70)
        Me.CmdAction02.Name = "CmdAction02"
        Me.CmdAction02.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction02.TabIndex = 2
        Me.CmdAction02.Tag = "1"
        Me.CmdAction02.Text = "資金決済リエンタ作成取消"
        '
        'CmdAction01
        '
        Me.CmdAction01.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction01.Location = New System.Drawing.Point(55, 20)
        Me.CmdAction01.Name = "CmdAction01"
        Me.CmdAction01.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction01.TabIndex = 1
        Me.CmdAction01.Tag = "1"
        Me.CmdAction01.Text = "資金決済リエンタ作成"
        '
        'CmdAction03
        '
        Me.CmdAction03.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction03.Location = New System.Drawing.Point(55, 120)
        Me.CmdAction03.Name = "CmdAction03"
        Me.CmdAction03.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction03.TabIndex = 3
        Me.CmdAction03.Tag = "1"
        Me.CmdAction03.Text = "資金決済リエンタ結果更新"
        '
        'CmdAction10
        '
        Me.CmdAction10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction10.Location = New System.Drawing.Point(395, 20)
        Me.CmdAction10.Name = "CmdAction10"
        Me.CmdAction10.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction10.TabIndex = 9
        Me.CmdAction10.Tag = "1"
        Me.CmdAction10.Text = "自振処理企業一覧表印刷"
        '
        'CmdAction04
        '
        Me.CmdAction04.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction04.Location = New System.Drawing.Point(55, 170)
        Me.CmdAction04.Name = "CmdAction04"
        Me.CmdAction04.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction04.TabIndex = 4
        Me.CmdAction04.Tag = "0"
        Me.CmdAction04.Text = "資金決済リエンタ書込"
        '
        'CmdAction12
        '
        Me.CmdAction12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction12.Location = New System.Drawing.Point(395, 120)
        Me.CmdAction12.Name = "CmdAction12"
        Me.CmdAction12.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction12.TabIndex = 11
        Me.CmdAction12.Tag = "1"
        Me.CmdAction12.Text = "手数料未徴求企業一覧表印刷"
        '
        'CmdAction13
        '
        Me.CmdAction13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction13.Location = New System.Drawing.Point(395, 170)
        Me.CmdAction13.Name = "CmdAction13"
        Me.CmdAction13.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction13.TabIndex = 12
        Me.CmdAction13.Tag = "1"
        Me.CmdAction13.Text = "手数料一括徴求明細表印刷"
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
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.Transparent
        Me.TabPage1.Controls.Add(Me.CmdAction11)
        Me.TabPage1.Controls.Add(Me.CmdAction14)
        Me.TabPage1.Controls.Add(Me.CmdAction15)
        Me.TabPage1.Controls.Add(Me.CmdAction16)
        Me.TabPage1.Controls.Add(Me.CmdAction17)
        Me.TabPage1.Controls.Add(Me.CmdAction08)
        Me.TabPage1.Controls.Add(Me.CmdAction07)
        Me.TabPage1.Controls.Add(Me.CmdAction06)
        Me.TabPage1.Controls.Add(Me.CmdAction05)
        Me.TabPage1.Controls.Add(Me.CmdAction03)
        Me.TabPage1.Controls.Add(Me.CmdAction10)
        Me.TabPage1.Controls.Add(Me.CmdAction12)
        Me.TabPage1.Controls.Add(Me.CmdAction02)
        Me.TabPage1.Controls.Add(Me.CmdAction13)
        Me.TabPage1.Controls.Add(Me.CmdAction01)
        Me.TabPage1.Controls.Add(Me.CmdAction04)
        Me.TabPage1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage1.Location = New System.Drawing.Point(4, 24)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(747, 427)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "資金決済"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'CmdAction11
        '
        Me.CmdAction11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction11.Location = New System.Drawing.Point(395, 70)
        Me.CmdAction11.Name = "CmdAction11"
        Me.CmdAction11.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction11.TabIndex = 10
        Me.CmdAction11.Tag = "1"
        Me.CmdAction11.Text = "資金決済企業一覧表印刷"
        '
        'CmdAction14
        '
        Me.CmdAction14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction14.Location = New System.Drawing.Point(395, 220)
        Me.CmdAction14.Name = "CmdAction14"
        Me.CmdAction14.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction14.TabIndex = 13
        Me.CmdAction14.Tag = "1"
        Me.CmdAction14.Text = "処理結果確認表再印刷"
        '
        'CmdAction15
        '
        Me.CmdAction15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction15.Location = New System.Drawing.Point(395, 270)
        Me.CmdAction15.Name = "CmdAction15"
        Me.CmdAction15.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction15.TabIndex = 14
        Me.CmdAction15.Tag = "1"
        Me.CmdAction15.Text = "預金口座振替内訳票手数料請求書印刷"
        '
        'CmdAction16
        '
        Me.CmdAction16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction16.Location = New System.Drawing.Point(395, 320)
        Me.CmdAction16.Name = "CmdAction16"
        Me.CmdAction16.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction16.TabIndex = 15
        Me.CmdAction16.Tag = "0"
        '
        'CmdAction17
        '
        Me.CmdAction17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction17.Location = New System.Drawing.Point(395, 370)
        Me.CmdAction17.Name = "CmdAction17"
        Me.CmdAction17.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction17.TabIndex = 16
        Me.CmdAction17.Tag = "0"
        '
        'CmdAction08
        '
        Me.CmdAction08.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction08.Location = New System.Drawing.Point(55, 370)
        Me.CmdAction08.Name = "CmdAction08"
        Me.CmdAction08.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction08.TabIndex = 8
        Me.CmdAction08.Tag = "0"
        '
        'CmdAction07
        '
        Me.CmdAction07.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction07.Location = New System.Drawing.Point(55, 320)
        Me.CmdAction07.Name = "CmdAction07"
        Me.CmdAction07.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction07.TabIndex = 7
        Me.CmdAction07.Tag = "0"
        '
        'CmdAction06
        '
        Me.CmdAction06.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction06.Location = New System.Drawing.Point(55, 270)
        Me.CmdAction06.Name = "CmdAction06"
        Me.CmdAction06.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction06.TabIndex = 6
        Me.CmdAction06.Tag = "0"
        '
        'CmdAction05
        '
        Me.CmdAction05.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction05.Location = New System.Drawing.Point(55, 220)
        Me.CmdAction05.Name = "CmdAction05"
        Me.CmdAction05.Size = New System.Drawing.Size(300, 40)
        Me.CmdAction05.TabIndex = 5
        Me.CmdAction05.Tag = "0"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 33
        Me.lblTitle.Tag = "0"
        Me.lblTitle.Text = "RELATIONSTAGE企業自振V2/資金決済"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'KFKMENU010
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 12)
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFKMENU010"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFKMENU010"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

End Class
