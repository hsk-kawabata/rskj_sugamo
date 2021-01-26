<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJTAKO010
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJTAKO010))
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.grpKekka = New System.Windows.Forms.GroupBox
        Me.chlKekka = New System.Windows.Forms.CheckedListBox
        Me.btnKousin = New System.Windows.Forms.Button
        Me.grpIrai = New System.Windows.Forms.GroupBox
        Me.btnTouroku = New System.Windows.Forms.Button
        Me.chlIrai = New System.Windows.Forms.CheckedListBox
        Me.btnReAction = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtJyusinDateD = New System.Windows.Forms.TextBox
        Me.txtJyusinDateM = New System.Windows.Forms.TextBox
        Me.txtJyusinDateY = New System.Windows.Forms.TextBox
        Me.cmbFileName = New System.Windows.Forms.ComboBox
        Me.grpKekka.SuspendLayout()
        Me.grpIrai.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Location = New System.Drawing.Point(673, 20)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 145
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Location = New System.Drawing.Point(673, 43)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 142
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(594, 43)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 12)
        Me.Label4.TabIndex = 143
        Me.Label4.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
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
        Me.Label1.Text = "＜受信ファイル一括振分(元請)＞"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpKekka
        '
        Me.grpKekka.Controls.Add(Me.chlKekka)
        Me.grpKekka.Controls.Add(Me.btnKousin)
        Me.grpKekka.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpKekka.ForeColor = System.Drawing.SystemColors.InfoText
        Me.grpKekka.Location = New System.Drawing.Point(404, 140)
        Me.grpKekka.Name = "grpKekka"
        Me.grpKekka.Size = New System.Drawing.Size(340, 370)
        Me.grpKekka.TabIndex = 159
        Me.grpKekka.TabStop = False
        Me.grpKekka.Text = "振分結果(結果分)"
        '
        'chlKekka
        '
        Me.chlKekka.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chlKekka.Location = New System.Drawing.Point(10, 24)
        Me.chlKekka.Name = "chlKekka"
        Me.chlKekka.Size = New System.Drawing.Size(320, 289)
        Me.chlKekka.TabIndex = 33
        '
        'btnKousin
        '
        Me.btnKousin.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKousin.Location = New System.Drawing.Point(110, 320)
        Me.btnKousin.Name = "btnKousin"
        Me.btnKousin.Size = New System.Drawing.Size(120, 40)
        Me.btnKousin.TabIndex = 32
        Me.btnKousin.Text = "不能結果更新"
        '
        'grpIrai
        '
        Me.grpIrai.Controls.Add(Me.btnTouroku)
        Me.grpIrai.Controls.Add(Me.chlIrai)
        Me.grpIrai.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpIrai.ForeColor = System.Drawing.SystemColors.InfoText
        Me.grpIrai.Location = New System.Drawing.Point(52, 140)
        Me.grpIrai.Name = "grpIrai"
        Me.grpIrai.Size = New System.Drawing.Size(340, 370)
        Me.grpIrai.TabIndex = 158
        Me.grpIrai.TabStop = False
        Me.grpIrai.Text = "振分結果(依頼分)"
        '
        'btnTouroku
        '
        Me.btnTouroku.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnTouroku.Location = New System.Drawing.Point(110, 320)
        Me.btnTouroku.Name = "btnTouroku"
        Me.btnTouroku.Size = New System.Drawing.Size(120, 40)
        Me.btnTouroku.TabIndex = 31
        Me.btnTouroku.Text = "個別登録"
        '
        'chlIrai
        '
        Me.chlIrai.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chlIrai.Location = New System.Drawing.Point(10, 24)
        Me.chlIrai.Name = "chlIrai"
        Me.chlIrai.Size = New System.Drawing.Size(320, 289)
        Me.chlIrai.TabIndex = 31
        '
        'btnReAction
        '
        Me.btnReAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReAction.Location = New System.Drawing.Point(662, 62)
        Me.btnReAction.Name = "btnReAction"
        Me.btnReAction.Size = New System.Drawing.Size(120, 40)
        Me.btnReAction.TabIndex = 156
        Me.btnReAction.Text = "再読み込み"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 157
        Me.btnEnd.Text = "終　了"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(530, 62)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 155
        Me.btnAction.Text = "読み込み"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(300, 110)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 154
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(248, 109)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 153
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(196, 110)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 152
        Me.Label8.Text = "年"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(28, 110)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 16)
        Me.Label6.TabIndex = 147
        Me.Label6.Text = "受　信　日"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(28, 70)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(120, 16)
        Me.Label7.TabIndex = 146
        Me.Label7.Text = "受信ファイル名"
        '
        'txtJyusinDateD
        '
        Me.txtJyusinDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinDateD.Location = New System.Drawing.Point(274, 106)
        Me.txtJyusinDateD.MaxLength = 2
        Me.txtJyusinDateD.Name = "txtJyusinDateD"
        Me.txtJyusinDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtJyusinDateD.TabIndex = 151
        '
        'txtJyusinDateM
        '
        Me.txtJyusinDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinDateM.Location = New System.Drawing.Point(222, 106)
        Me.txtJyusinDateM.MaxLength = 2
        Me.txtJyusinDateM.Name = "txtJyusinDateM"
        Me.txtJyusinDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtJyusinDateM.TabIndex = 150
        '
        'txtJyusinDateY
        '
        Me.txtJyusinDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtJyusinDateY.Location = New System.Drawing.Point(150, 106)
        Me.txtJyusinDateY.MaxLength = 4
        Me.txtJyusinDateY.Name = "txtJyusinDateY"
        Me.txtJyusinDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtJyusinDateY.TabIndex = 149
        '
        'cmbFileName
        '
        Me.cmbFileName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFileName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbFileName.Location = New System.Drawing.Point(150, 65)
        Me.cmbFileName.Name = "cmbFileName"
        Me.cmbFileName.Size = New System.Drawing.Size(350, 24)
        Me.cmbFileName.TabIndex = 148
        '
        'KFJTAKO010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.grpKekka)
        Me.Controls.Add(Me.grpIrai)
        Me.Controls.Add(Me.btnReAction)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtJyusinDateD)
        Me.Controls.Add(Me.txtJyusinDateM)
        Me.Controls.Add(Me.txtJyusinDateY)
        Me.Controls.Add(Me.cmbFileName)
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
        Me.Name = "KFJTAKO010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJTAKO010"
        Me.grpKekka.ResumeLayout(False)
        Me.grpIrai.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '年
        AddHandler txtJyusinDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtJyusinDateY.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtJyusinDateY.KeyPress, AddressOf CAST.KeyPress

        '月
        AddHandler txtJyusinDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtJyusinDateM.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtJyusinDateM.KeyPress, AddressOf CAST.KeyPress

        '日cmbTimeStamp
        AddHandler txtJyusinDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtJyusinDateD.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtJyusinDateD.KeyPress, AddressOf CAST.KeyPress

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
    Friend WithEvents grpKekka As System.Windows.Forms.GroupBox
    Friend WithEvents chlKekka As System.Windows.Forms.CheckedListBox
    Friend WithEvents btnKousin As System.Windows.Forms.Button
    Friend WithEvents grpIrai As System.Windows.Forms.GroupBox
    Friend WithEvents btnTouroku As System.Windows.Forms.Button
    Friend WithEvents chlIrai As System.Windows.Forms.CheckedListBox
    Friend WithEvents btnReAction As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtJyusinDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtJyusinDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtJyusinDateY As System.Windows.Forms.TextBox
    Friend WithEvents cmbFileName As System.Windows.Forms.ComboBox
End Class
