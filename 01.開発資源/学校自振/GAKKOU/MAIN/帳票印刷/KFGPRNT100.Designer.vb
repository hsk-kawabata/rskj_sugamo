﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFGPRNT100
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
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents cmbGakkouName As System.Windows.Forms.ComboBox
    Friend WithEvents txtGAKKOU_CODE As System.Windows.Forms.TextBox
    Friend WithEvents lab学校名 As System.Windows.Forms.Label
    Friend WithEvents btnPrnt As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFGPRNT100))
        Me.btnPrnt = New System.Windows.Forms.Button()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lab学校名 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtGAKKOU_CODE = New System.Windows.Forms.TextBox()
        Me.cmbGakkouName = New System.Windows.Forms.ComboBox()
        Me.cmbKana = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'btnPrnt
        '
        Me.btnPrnt.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrnt.Location = New System.Drawing.Point(534, 521)
        Me.btnPrnt.Name = "btnPrnt"
        Me.btnPrnt.Size = New System.Drawing.Size(120, 40)
        Me.btnPrnt.TabIndex = 1
        Me.btnPrnt.Text = "印　刷"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 521)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 2
        Me.btnEnd.Text = "終　了"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(100, 270)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(309, 19)
        Me.Label11.TabIndex = 142
        Me.Label11.Text = "※ALL9指定時のみ、全件処理する"
        '
        'lab学校名
        '
        Me.lab学校名.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lab学校名.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lab学校名.Location = New System.Drawing.Point(298, 169)
        Me.lab学校名.Name = "lab学校名"
        Me.lab学校名.Size = New System.Drawing.Size(423, 24)
        Me.lab学校名.TabIndex = 141
        Me.lab学校名.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(101, 173)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(93, 16)
        Me.Label5.TabIndex = 136
        Me.Label5.Text = "学校コード"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(101, 120)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 16)
        Me.Label4.TabIndex = 135
        Me.Label4.Text = "学校検索"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(667, 44)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 134
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(667, 20)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 133
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 132
        Me.Label3.Text = "＜長子チェックリスト明細表印刷＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(579, 44)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 131
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(595, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 130
        Me.Label1.Text = "ログイン名　:"
        '
        'txtGAKKOU_CODE
        '
        Me.txtGAKKOU_CODE.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtGAKKOU_CODE.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtGAKKOU_CODE.Location = New System.Drawing.Point(200, 170)
        Me.txtGAKKOU_CODE.MaxLength = 10
        Me.txtGAKKOU_CODE.Name = "txtGAKKOU_CODE"
        Me.txtGAKKOU_CODE.Size = New System.Drawing.Size(90, 23)
        Me.txtGAKKOU_CODE.TabIndex = 0
        '
        'cmbGakkouName
        '
        Me.cmbGakkouName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbGakkouName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbGakkouName.Location = New System.Drawing.Point(250, 120)
        Me.cmbGakkouName.Name = "cmbGakkouName"
        Me.cmbGakkouName.Size = New System.Drawing.Size(471, 21)
        Me.cmbGakkouName.TabIndex = 125
        Me.cmbGakkouName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(200, 120)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 124
        Me.cmbKana.TabStop = False
        '
        'KFGPRNT100
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.btnPrnt)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.lab学校名)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtGAKKOU_CODE)
        Me.Controls.Add(Me.cmbGakkouName)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFGPRNT100"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFGPRNT100"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。


        '***************
        'GotFocus
        '***************
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbGakkouName.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtGAKKOU_CODE.GotFocus, AddressOf CASTx01.GotFocus

        '***************
        'KeyPress
        '***************
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbGakkouName.KeyPress, AddressOf CAST.KeyPress
        AddHandler txtGAKKOU_CODE.KeyPress, AddressOf CASTx01.KeyPressNum

        '***************
        'LostFocus
        '***************
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbGakkouName.LostFocus, AddressOf CAST.LostFocus
        AddHandler txtGAKKOU_CODE.LostFocus, AddressOf CASTx01.LostFocusZero
    End Sub

End Class
