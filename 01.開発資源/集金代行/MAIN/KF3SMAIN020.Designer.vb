<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KF3SMAIN020
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '振替日(年)テキストボックス
        AddHandler Me.txtFuriDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtFuriDateY.LostFocus, AddressOf CAST.LostFocus

        '振替日(月)テキストボックス
        AddHandler Me.txtFuriDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtFuriDateM.LostFocus, AddressOf CAST.LostFocus

        '振替日(日)テキストボックス
        AddHandler Me.txtFuriDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler Me.txtFuriDateD.LostFocus, AddressOf CAST.LostFocus

        '企業シーケンスラジオボタン
        AddHandler Me.rdbKIGYO_SEQ.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.rdbKIGYO_SEQ.LostFocus, AddressOf CAST.LostFocus
        AddHandler Me.rdbKIGYO_SEQ.KeyPress, AddressOf CAST.KeyPress

        '口座情報ラジオボタン
        AddHandler Me.rdbKOUZA.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.rdbKOUZA.LostFocus, AddressOf CAST.LostFocus
        AddHandler Me.rdbKOUZA.KeyPress, AddressOf CAST.KeyPress

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
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblTITLE As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents rdbKOUZA As System.Windows.Forms.RadioButton
    Friend WithEvents rdbKIGYO_SEQ As System.Windows.Forms.RadioButton
    Friend WithEvents Label7 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KF3SMAIN020))
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.lblTITLE = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnAction = New System.Windows.Forms.Button()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtFuriDateD = New System.Windows.Forms.TextBox()
        Me.txtFuriDateM = New System.Windows.Forms.TextBox()
        Me.txtFuriDateY = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.rdbKOUZA = New System.Windows.Forms.RadioButton()
        Me.rdbKIGYO_SEQ = New System.Windows.Forms.RadioButton()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
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
        'lblTITLE
        '
        Me.lblTITLE.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTITLE.Location = New System.Drawing.Point(0, 8)
        Me.lblTITLE.Name = "lblTITLE"
        Me.lblTITLE.Size = New System.Drawing.Size(795, 30)
        Me.lblTITLE.TabIndex = 141
        Me.lblTITLE.Text = "＜他行分不能結果更新＞"
        Me.lblTITLE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 5
        Me.btnAction.Text = "実　行"
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
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(494, 178)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(24, 16)
        Me.Label11.TabIndex = 150
        Me.Label11.Text = "日"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(390, 178)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 152
        Me.Label10.Text = "年"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.Location = New System.Drawing.Point(468, 174)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 2
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.Location = New System.Drawing.Point(416, 174)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 1
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.Location = New System.Drawing.Point(344, 174)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 0
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(442, 178)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 151
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(233, 178)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(88, 16)
        Me.Label8.TabIndex = 149
        Me.Label8.Text = "振　替　日"
        '
        'rdbKOUZA
        '
        Me.rdbKOUZA.AutoSize = True
        Me.rdbKOUZA.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbKOUZA.Location = New System.Drawing.Point(488, 235)
        Me.rdbKOUZA.Name = "rdbKOUZA"
        Me.rdbKOUZA.Size = New System.Drawing.Size(90, 20)
        Me.rdbKOUZA.TabIndex = 4
        Me.rdbKOUZA.Text = "口座情報"
        Me.rdbKOUZA.UseVisualStyleBackColor = True
        '
        'rdbKIGYO_SEQ
        '
        Me.rdbKIGYO_SEQ.AutoSize = True
        Me.rdbKIGYO_SEQ.Checked = True
        Me.rdbKIGYO_SEQ.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rdbKIGYO_SEQ.Location = New System.Drawing.Point(344, 235)
        Me.rdbKIGYO_SEQ.Name = "rdbKIGYO_SEQ"
        Me.rdbKIGYO_SEQ.Size = New System.Drawing.Size(138, 20)
        Me.rdbKIGYO_SEQ.TabIndex = 3
        Me.rdbKIGYO_SEQ.TabStop = True
        Me.rdbKIGYO_SEQ.Text = "企業シーケンス"
        Me.rdbKIGYO_SEQ.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(232, 237)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(104, 16)
        Me.Label7.TabIndex = 155
        Me.Label7.Text = "更新キー項目"
        '
        'KF3SMAIN020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.rdbKOUZA)
        Me.Controls.Add(Me.rdbKIGYO_SEQ)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblTITLE)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "KF3SMAIN020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KF3SMAIN020"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

End Class
