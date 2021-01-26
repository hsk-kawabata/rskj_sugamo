﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFWMAIN020
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '処理日(年)
        AddHandler txtJyusinDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtJyusinDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtJyusinDateY.LostFocus, AddressOf CASTx01.LostFocus

        '処理日(月)
        AddHandler txtJyusinDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtJyusinDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtJyusinDateM.LostFocus, AddressOf CASTx01.LostFocus

        '処理日(日)
        AddHandler txtJyusinDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtJyusinDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtJyusinDateD.LostFocus, AddressOf CASTx01.LostFocus

    End Sub

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFWMAIN020))
        Me.lblTitle = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.btnPrint = New System.Windows.Forms.Button
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtJyusinDateD = New System.Windows.Forms.TextBox
        Me.txtJyusinDateM = New System.Windows.Forms.TextBox
        Me.txtJyusinDateY = New System.Windows.Forms.TextBox
        Me.btnEnd = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "＜WEB伝送ログ一覧印刷＞"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "システム日付:"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(685, 33)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 4
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'btnPrint
        '
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(136, 520)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(120, 40)
        Me.btnPrint.TabIndex = 5
        Me.btnPrint.Text = "印　刷"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(486, 188)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 14
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(430, 188)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 12
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(378, 188)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "年"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(238, 188)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(88, 16)
        Me.Label7.TabIndex = 8
        Me.Label7.Text = "処　理　日"
        '
        'txtJyusinDateD
        '
        Me.txtJyusinDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinDateD.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtJyusinDateD.Location = New System.Drawing.Point(460, 184)
        Me.txtJyusinDateD.MaxLength = 2
        Me.txtJyusinDateD.Name = "txtJyusinDateD"
        Me.txtJyusinDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtJyusinDateD.TabIndex = 2
        '
        'txtJyusinDateM
        '
        Me.txtJyusinDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinDateM.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtJyusinDateM.Location = New System.Drawing.Point(404, 184)
        Me.txtJyusinDateM.MaxLength = 2
        Me.txtJyusinDateM.Name = "txtJyusinDateM"
        Me.txtJyusinDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtJyusinDateM.TabIndex = 1
        '
        'txtJyusinDateY
        '
        Me.txtJyusinDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinDateY.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtJyusinDateY.Location = New System.Drawing.Point(332, 184)
        Me.txtJyusinDateY.MaxLength = 4
        Me.txtJyusinDateY.Name = "txtJyusinDateY"
        Me.txtJyusinDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtJyusinDateY.TabIndex = 0
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 6
        Me.btnEnd.Text = "終　了"
        '
        'KFWMAIN020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 568)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtJyusinDateD)
        Me.Controls.Add(Me.txtJyusinDateM)
        Me.Controls.Add(Me.txtJyusinDateY)
        Me.Controls.Add(Me.btnEnd)
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
        Me.Name = "KFWMAIN020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFWMAIN020"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtJyusinDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtJyusinDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtJyusinDateY As System.Windows.Forms.TextBox
    Friend WithEvents btnEnd As System.Windows.Forms.Button

End Class
