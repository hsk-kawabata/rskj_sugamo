<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFSMAST041
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

        'この呼び出しは Windows フォーム デザイナで必要です。  
        InitializeComponent()

        '自年
        AddHandler TxtYear1.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtYear1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TxtYear1.LostFocus, AddressOf CAST.LostFocus

        '至年
        AddHandler TxtYear2.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtYear2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TxtYear2.LostFocus, AddressOf CAST.LostFocus

        '自月
        AddHandler TxtMonth1.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtMonth1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TxtMonth1.LostFocus, AddressOf CAST.LostFocus

        '至月
        AddHandler TxtMonth2.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtMonth2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TxtMonth2.LostFocus, AddressOf CAST.LostFocus

        '自日
        AddHandler TxtDay1.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtDay1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TxtDay1.LostFocus, AddressOf CAST.LostFocus

        '至日
        AddHandler TxtDay2.GotFocus, AddressOf CAST.GotFocus
        AddHandler TxtDay2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TxtDay2.LostFocus, AddressOf CAST.LostFocus

        '進捗表示画面
        AddHandler ListView1.GotFocus, AddressOf CAST.GotFocus
        AddHandler ListView1.KeyPress, AddressOf CAST.KeyPress
        AddHandler ListView1.LostFocus, AddressOf CAST.LostFocus

    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(New String() {"", "1,234", "テスト委託者", "0000001-01", "9999999999", "依頼書", "2009/06/22", "○", "○", "○", "×", "-", "123,456", "1,234,567,890,123"}, -1)
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFSMAST041))
        Me.CmdCancel = New System.Windows.Forms.Button()
        Me.CmdSelect = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.FormDateL2 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.FormDateL1 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.FormDateL0 = New System.Windows.Forms.Label()
        Me.TxtDay2 = New System.Windows.Forms.TextBox()
        Me.TxtDay1 = New System.Windows.Forms.TextBox()
        Me.TxtMonth2 = New System.Windows.Forms.TextBox()
        Me.TxtMonth1 = New System.Windows.Forms.TextBox()
        Me.TxtYear2 = New System.Windows.Forms.TextBox()
        Me.TxtYear1 = New System.Windows.Forms.TextBox()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.lblHassinDateL = New System.Windows.Forms.Label()
        Me.CmdSchPrint = New System.Windows.Forms.Button()
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.cmdOtosi = New System.Windows.Forms.Button()
        Me.CmdUkeDelete = New System.Windows.Forms.Button()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CmdInputRePrint = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'CmdCancel
        '
        Me.CmdCancel.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdCancel.Location = New System.Drawing.Point(511, 64)
        Me.CmdCancel.Name = "CmdCancel"
        Me.CmdCancel.Size = New System.Drawing.Size(50, 25)
        Me.CmdCancel.TabIndex = 7
        Me.CmdCancel.TabStop = False
        Me.CmdCancel.Text = "ｸﾘｱ"
        '
        'CmdSelect
        '
        Me.CmdSelect.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdSelect.Location = New System.Drawing.Point(451, 64)
        Me.CmdSelect.Name = "CmdSelect"
        Me.CmdSelect.Size = New System.Drawing.Size(50, 25)
        Me.CmdSelect.TabIndex = 6
        Me.CmdSelect.Text = "参照"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(426, 68)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 351
        Me.Label8.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(250, 68)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 352
        Me.Label9.Text = "～"
        '
        'FormDateL2
        '
        Me.FormDateL2.AutoSize = True
        Me.FormDateL2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormDateL2.Location = New System.Drawing.Point(221, 68)
        Me.FormDateL2.Name = "FormDateL2"
        Me.FormDateL2.Size = New System.Drawing.Size(24, 16)
        Me.FormDateL2.TabIndex = 353
        Me.FormDateL2.Text = "日"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(374, 68)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 349
        Me.Label7.Text = "月"
        '
        'FormDateL1
        '
        Me.FormDateL1.AutoSize = True
        Me.FormDateL1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormDateL1.Location = New System.Drawing.Point(169, 68)
        Me.FormDateL1.Name = "FormDateL1"
        Me.FormDateL1.Size = New System.Drawing.Size(24, 16)
        Me.FormDateL1.TabIndex = 350
        Me.FormDateL1.Text = "月"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(322, 68)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(24, 16)
        Me.Label6.TabIndex = 347
        Me.Label6.Text = "年"
        '
        'FormDateL0
        '
        Me.FormDateL0.AutoSize = True
        Me.FormDateL0.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormDateL0.Location = New System.Drawing.Point(117, 68)
        Me.FormDateL0.Name = "FormDateL0"
        Me.FormDateL0.Size = New System.Drawing.Size(24, 16)
        Me.FormDateL0.TabIndex = 348
        Me.FormDateL0.Text = "年"
        '
        'TxtDay2
        '
        Me.TxtDay2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtDay2.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TxtDay2.Location = New System.Drawing.Point(400, 64)
        Me.TxtDay2.MaxLength = 2
        Me.TxtDay2.Name = "TxtDay2"
        Me.TxtDay2.Size = New System.Drawing.Size(24, 23)
        Me.TxtDay2.TabIndex = 5
        '
        'TxtDay1
        '
        Me.TxtDay1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtDay1.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TxtDay1.Location = New System.Drawing.Point(195, 64)
        Me.TxtDay1.MaxLength = 2
        Me.TxtDay1.Name = "TxtDay1"
        Me.TxtDay1.Size = New System.Drawing.Size(24, 23)
        Me.TxtDay1.TabIndex = 2
        '
        'TxtMonth2
        '
        Me.TxtMonth2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtMonth2.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TxtMonth2.Location = New System.Drawing.Point(348, 64)
        Me.TxtMonth2.MaxLength = 2
        Me.TxtMonth2.Name = "TxtMonth2"
        Me.TxtMonth2.Size = New System.Drawing.Size(24, 23)
        Me.TxtMonth2.TabIndex = 4
        '
        'TxtMonth1
        '
        Me.TxtMonth1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtMonth1.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TxtMonth1.Location = New System.Drawing.Point(143, 64)
        Me.TxtMonth1.MaxLength = 2
        Me.TxtMonth1.Name = "TxtMonth1"
        Me.TxtMonth1.Size = New System.Drawing.Size(24, 23)
        Me.TxtMonth1.TabIndex = 1
        '
        'TxtYear2
        '
        Me.TxtYear2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtYear2.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TxtYear2.Location = New System.Drawing.Point(276, 64)
        Me.TxtYear2.MaxLength = 4
        Me.TxtYear2.Name = "TxtYear2"
        Me.TxtYear2.Size = New System.Drawing.Size(44, 23)
        Me.TxtYear2.TabIndex = 3
        '
        'TxtYear1
        '
        Me.TxtYear1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TxtYear1.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TxtYear1.Location = New System.Drawing.Point(71, 64)
        Me.TxtYear1.MaxLength = 4
        Me.TxtYear1.Name = "TxtYear1"
        Me.TxtYear1.Size = New System.Drawing.Size(44, 23)
        Me.TxtYear1.TabIndex = 0
        '
        'ListView1
        '
        Me.ListView1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        ListViewItem1.StateImageIndex = 0
        Me.ListView1.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1})
        Me.ListView1.Location = New System.Drawing.Point(11, 96)
        Me.ListView1.MultiSelect = False
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(768, 408)
        Me.ListView1.TabIndex = 340
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'lblHassinDateL
        '
        Me.lblHassinDateL.AutoSize = True
        Me.lblHassinDateL.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblHassinDateL.Location = New System.Drawing.Point(13, 68)
        Me.lblHassinDateL.Name = "lblHassinDateL"
        Me.lblHassinDateL.Size = New System.Drawing.Size(56, 16)
        Me.lblHassinDateL.TabIndex = 339
        Me.lblHassinDateL.Text = "対象日"
        '
        'CmdSchPrint
        '
        Me.CmdSchPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!)
        Me.CmdSchPrint.Location = New System.Drawing.Point(659, 64)
        Me.CmdSchPrint.Name = "CmdSchPrint"
        Me.CmdSchPrint.Size = New System.Drawing.Size(120, 25)
        Me.CmdSchPrint.TabIndex = 90
        Me.CmdSchPrint.TabStop = False
        Me.CmdSchPrint.Tag = "　"
        Me.CmdSchPrint.Text = "画面一覧印刷"
        '
        'CmdBack
        '
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 10.0!)
        Me.CmdBack.Location = New System.Drawing.Point(660, 525)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(120, 40)
        Me.CmdBack.TabIndex = 16
        Me.CmdBack.Text = "終　了"
        '
        'cmdOtosi
        '
        Me.cmdOtosi.Font = New System.Drawing.Font("ＭＳ ゴシック", 10.0!)
        Me.cmdOtosi.Location = New System.Drawing.Point(10, 525)
        Me.cmdOtosi.Name = "cmdOtosi"
        Me.cmdOtosi.Size = New System.Drawing.Size(120, 40)
        Me.cmdOtosi.TabIndex = 11
        Me.cmdOtosi.Text = "落　込"
        '
        'CmdUkeDelete
        '
        Me.CmdUkeDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 10.0!)
        Me.CmdUkeDelete.Location = New System.Drawing.Point(140, 525)
        Me.CmdUkeDelete.Name = "CmdUkeDelete"
        Me.CmdUkeDelete.Size = New System.Drawing.Size(120, 40)
        Me.CmdUkeDelete.TabIndex = 12
        Me.CmdUkeDelete.Text = "落込取消"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Location = New System.Drawing.Point(681, 15)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 334
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Location = New System.Drawing.Point(681, 38)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 331
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(602, 38)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 12)
        Me.Label3.TabIndex = 332
        Me.Label3.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(616, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 333
        Me.Label2.Text = "ログイン名　:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 330
        Me.Label1.Text = "＜スケジュール管理＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(212, 507)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(377, 12)
        Me.Label4.TabIndex = 354
        Me.Label4.Text = "※マルチデータの落込取消は、同一ヘッダの取引先の取消も必要です"
        '
        'CmdInputRePrint
        '
        Me.CmdInputRePrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdInputRePrint.Location = New System.Drawing.Point(400, 525)
        Me.CmdInputRePrint.Name = "CmdInputRePrint"
        Me.CmdInputRePrint.Size = New System.Drawing.Size(120, 40)
        Me.CmdInputRePrint.TabIndex = 14
        Me.CmdInputRePrint.Text = "入力帳票再印刷"
        '
        'KFSMAST041
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.CmdInputRePrint)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.CmdCancel)
        Me.Controls.Add(Me.CmdSelect)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.FormDateL2)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.FormDateL1)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.FormDateL0)
        Me.Controls.Add(Me.TxtDay2)
        Me.Controls.Add(Me.TxtDay1)
        Me.Controls.Add(Me.TxtMonth2)
        Me.Controls.Add(Me.TxtMonth1)
        Me.Controls.Add(Me.TxtYear2)
        Me.Controls.Add(Me.TxtYear1)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.lblHassinDateL)
        Me.Controls.Add(Me.CmdSchPrint)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.cmdOtosi)
        Me.Controls.Add(Me.CmdUkeDelete)
        Me.Controls.Add(Me.lblUser)
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
        Me.Name = "KFSMAST041"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFSMAST041"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CmdCancel As System.Windows.Forms.Button
    Friend WithEvents CmdSelect As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents FormDateL2 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents FormDateL1 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents FormDateL0 As System.Windows.Forms.Label
    Friend WithEvents TxtDay2 As System.Windows.Forms.TextBox
    Friend WithEvents TxtDay1 As System.Windows.Forms.TextBox
    Friend WithEvents TxtMonth2 As System.Windows.Forms.TextBox
    Friend WithEvents TxtMonth1 As System.Windows.Forms.TextBox
    Friend WithEvents TxtYear2 As System.Windows.Forms.TextBox
    Friend WithEvents TxtYear1 As System.Windows.Forms.TextBox
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents lblHassinDateL As System.Windows.Forms.Label
    Friend WithEvents CmdSchPrint As System.Windows.Forms.Button
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents cmdOtosi As System.Windows.Forms.Button
    Friend WithEvents CmdUkeDelete As System.Windows.Forms.Button
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CmdInputRePrint As System.Windows.Forms.Button
End Class
