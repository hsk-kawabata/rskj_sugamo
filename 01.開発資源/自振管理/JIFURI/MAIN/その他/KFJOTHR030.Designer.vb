﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJOTHR030
    Inherits System.Windows.Forms.Form


    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '年
        AddHandler txtFuriDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateY.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CAST.KeyPress

        '月
        AddHandler txtFuriDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateM.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CAST.KeyPress

        '日cmbTimeStamp
        AddHandler txtFuriDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateD.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CAST.KeyPress

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJOTHR030))
        Me.Label22 = New System.Windows.Forms.Label
        Me.Label23 = New System.Windows.Forms.Label
        Me.Label24 = New System.Windows.Forms.Label
        Me.txtFuriDateD = New System.Windows.Forms.TextBox
        Me.txtFuriDateM = New System.Windows.Forms.TextBox
        Me.txtFuriDateY = New System.Windows.Forms.TextBox
        Me.Label21 = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblTITLE = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label22.Location = New System.Drawing.Point(513, 170)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(24, 16)
        Me.Label22.TabIndex = 182
        Me.Label22.Text = "日"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label23.Location = New System.Drawing.Point(461, 170)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(24, 16)
        Me.Label23.TabIndex = 181
        Me.Label23.Text = "月"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label24.Location = New System.Drawing.Point(409, 170)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(24, 16)
        Me.Label24.TabIndex = 180
        Me.Label24.Text = "年"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.Location = New System.Drawing.Point(487, 166)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 3
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.Location = New System.Drawing.Point(435, 166)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 2
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.Location = New System.Drawing.Point(363, 166)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 1
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label21.Location = New System.Drawing.Point(227, 170)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(93, 16)
        Me.Label21.TabIndex = 179
        Me.Label21.Text = "処　理　日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(673, 20)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 145
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(673, 43)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 142
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(594, 43)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 143
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(608, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 144
        Me.Label1.Text = "ログイン名　:"
        '
        'lblTITLE
        '
        Me.lblTITLE.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTITLE.Location = New System.Drawing.Point(0, 8)
        Me.lblTITLE.Name = "lblTITLE"
        Me.lblTITLE.Size = New System.Drawing.Size(795, 30)
        Me.lblTITLE.TabIndex = 141
        Me.lblTITLE.Text = "＜センター直接持込落込取消＞"
        Me.lblTITLE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 8
        Me.btnEnd.Text = "終　了"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 6
        Me.btnAction.Text = "実　行"
        '
        'KFJOTHR030
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTITLE)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJOTHR030"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJOTHR030"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblTITLE As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
End Class
