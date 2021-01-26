<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJTAKO020
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJTAKO020))
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.btnRead = New System.Windows.Forms.Button
        Me.cmbFileName = New System.Windows.Forms.ComboBox
        Me.txtHenkanDateY = New System.Windows.Forms.TextBox
        Me.txtHenkanDateM = New System.Windows.Forms.TextBox
        Me.txtHenkanDateD = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.lsbIrai = New System.Windows.Forms.ListBox
        Me.grpIrai = New System.Windows.Forms.GroupBox
        Me.lsbKekka = New System.Windows.Forms.ListBox
        Me.grpKekka = New System.Windows.Forms.GroupBox
        Me.grpIrai.SuspendLayout()
        Me.grpKekka.SuspendLayout()
        Me.SuspendLayout()
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
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(594, 43)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 12)
        Me.Label4.TabIndex = 143
        Me.Label4.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(608, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 144
        Me.Label2.Text = "ログイン名　:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(0, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(795, 30)
        Me.Label1.TabIndex = 141
        Me.Label1.Text = "＜一括送信ファイル作成(元請)＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 184
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 183
        Me.btnAction.Text = "実　行"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'btnRead
        '
        Me.btnRead.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnRead.Location = New System.Drawing.Point(550, 60)
        Me.btnRead.Name = "btnRead"
        Me.btnRead.Size = New System.Drawing.Size(120, 40)
        Me.btnRead.TabIndex = 183
        Me.btnRead.Text = "読　込"
        Me.btnRead.UseVisualStyleBackColor = True
        '
        'cmbFileName
        '
        Me.cmbFileName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFileName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbFileName.Location = New System.Drawing.Point(179, 60)
        Me.cmbFileName.Name = "cmbFileName"
        Me.cmbFileName.Size = New System.Drawing.Size(350, 24)
        Me.cmbFileName.TabIndex = 1
        '
        'txtHenkanDateY
        '
        Me.txtHenkanDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtHenkanDateY.Location = New System.Drawing.Point(179, 105)
        Me.txtHenkanDateY.MaxLength = 4
        Me.txtHenkanDateY.Name = "txtHenkanDateY"
        Me.txtHenkanDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtHenkanDateY.TabIndex = 2
        '
        'txtHenkanDateM
        '
        Me.txtHenkanDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtHenkanDateM.Location = New System.Drawing.Point(251, 105)
        Me.txtHenkanDateM.MaxLength = 2
        Me.txtHenkanDateM.Name = "txtHenkanDateM"
        Me.txtHenkanDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtHenkanDateM.TabIndex = 3
        '
        'txtHenkanDateD
        '
        Me.txtHenkanDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtHenkanDateD.Location = New System.Drawing.Point(303, 105)
        Me.txtHenkanDateD.MaxLength = 2
        Me.txtHenkanDateD.Name = "txtHenkanDateD"
        Me.txtHenkanDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtHenkanDateD.TabIndex = 4
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(40, 65)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(120, 16)
        Me.Label7.TabIndex = 185
        Me.Label7.Text = "送信ファイル名"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(40, 109)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(136, 16)
        Me.Label6.TabIndex = 186
        Me.Label6.Text = "返還データ振替日"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(225, 109)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 191
        Me.Label8.Text = "年"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(277, 109)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 192
        Me.Label9.Text = "月"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(329, 109)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 193
        Me.Label10.Text = "日"
        '
        'lsbIrai
        '
        Me.lsbIrai.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lsbIrai.Location = New System.Drawing.Point(16, 24)
        Me.lsbIrai.Name = "lsbIrai"
        Me.lsbIrai.Size = New System.Drawing.Size(304, 329)
        Me.lsbIrai.TabIndex = 0
        '
        'grpIrai
        '
        Me.grpIrai.Controls.Add(Me.lsbIrai)
        Me.grpIrai.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpIrai.ForeColor = System.Drawing.SystemColors.InfoText
        Me.grpIrai.Location = New System.Drawing.Point(59, 142)
        Me.grpIrai.Name = "grpIrai"
        Me.grpIrai.Size = New System.Drawing.Size(336, 368)
        Me.grpIrai.TabIndex = 5
        Me.grpIrai.TabStop = False
        Me.grpIrai.Text = "まとめ対象取引先コード(依頼)"
        '
        'lsbKekka
        '
        Me.lsbKekka.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lsbKekka.Location = New System.Drawing.Point(16, 24)
        Me.lsbKekka.Name = "lsbKekka"
        Me.lsbKekka.Size = New System.Drawing.Size(304, 329)
        Me.lsbKekka.TabIndex = 1
        '
        'grpKekka
        '
        Me.grpKekka.Controls.Add(Me.lsbKekka)
        Me.grpKekka.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpKekka.ForeColor = System.Drawing.SystemColors.InfoText
        Me.grpKekka.Location = New System.Drawing.Point(411, 142)
        Me.grpKekka.Name = "grpKekka"
        Me.grpKekka.Size = New System.Drawing.Size(336, 368)
        Me.grpKekka.TabIndex = 6
        Me.grpKekka.TabStop = False
        Me.grpKekka.Text = "まとめ対象取引先コード(返還)"
        '
        'KFJTAKO020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.grpKekka)
        Me.Controls.Add(Me.grpIrai)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtHenkanDateD)
        Me.Controls.Add(Me.txtHenkanDateM)
        Me.Controls.Add(Me.txtHenkanDateY)
        Me.Controls.Add(Me.cmbFileName)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnRead)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJTAKO020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJTAKO020"
        Me.grpIrai.ResumeLayout(False)
        Me.grpKekka.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '年
        AddHandler txtHenkanDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtHenkanDateY.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtHenkanDateY.KeyPress, AddressOf CAST.KeyPress

        '月
        AddHandler txtHenkanDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtHenkanDateM.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtHenkanDateM.KeyPress, AddressOf CAST.KeyPress

        '日cmbTimeStamp
        AddHandler txtHenkanDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtHenkanDateD.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtHenkanDateD.KeyPress, AddressOf CAST.KeyPress

        'コンボ
        AddHandler cmbFileName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbFileName.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbFileName.KeyPress, AddressOf CAST.KeyPress

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnRead As System.Windows.Forms.Button
    Friend WithEvents cmbFileName As System.Windows.Forms.ComboBox
    Friend WithEvents txtHenkanDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtHenkanDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtHenkanDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents lsbIrai As System.Windows.Forms.ListBox
    Friend WithEvents grpIrai As System.Windows.Forms.GroupBox
    Friend WithEvents lsbKekka As System.Windows.Forms.ListBox
    Friend WithEvents grpKekka As System.Windows.Forms.GroupBox
End Class
