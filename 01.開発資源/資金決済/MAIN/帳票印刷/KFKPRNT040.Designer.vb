<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFKPRNT040
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '決済日(年)
        AddHandler txtKessaiDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKessaiDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKessaiDateY.LostFocus, AddressOf CAST.LostFocus

        '決済日(月)
        AddHandler txtKessaiDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKessaiDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKessaiDateM.LostFocus, AddressOf CAST.LostFocus

        '決済日(日)
        AddHandler txtKessaiDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKessaiDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKessaiDateD.LostFocus, AddressOf CAST.LostFocus

        '2009/12/01 追加 =====================

        '決済日(年)
        AddHandler txtKessaiDateY2.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKessaiDateY2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKessaiDateY2.LostFocus, AddressOf CAST.LostFocus

        '決済日(月)
        AddHandler txtKessaiDateM2.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKessaiDateM2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKessaiDateM2.LostFocus, AddressOf CAST.LostFocus

        '決済日(日)
        AddHandler txtKessaiDateD2.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtKessaiDateD2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtKessaiDateD2.LostFocus, AddressOf CAST.LostFocus

        '=====================================

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
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents txtKessaiDateY As System.Windows.Forms.TextBox
    Friend WithEvents txtKessaiDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtKessaiDateD As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtKessaiDateY2 As System.Windows.Forms.TextBox
    Friend WithEvents txtKessaiDateM2 As System.Windows.Forms.TextBox
    Friend WithEvents txtKessaiDateD2 As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFKPRNT040))
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtKessaiDateY = New System.Windows.Forms.TextBox
        Me.txtKessaiDateM = New System.Windows.Forms.TextBox
        Me.txtKessaiDateD = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.btnAction = New System.Windows.Forms.Button
        Me.btnEnd = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.txtKessaiDateY2 = New System.Windows.Forms.TextBox
        Me.txtKessaiDateM2 = New System.Windows.Forms.TextBox
        Me.txtKessaiDateD2 = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(696, 32)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 11
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(696, 8)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(53, 12)
        Me.lblUser.TabIndex = 10
        Me.lblUser.Text = "ユーザ名"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "＜手数料一括徴求明細表印刷＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(610, 32)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(624, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "ログイン名　:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(150, 160)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(136, 16)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "手数料徴求予定日"
        '
        'txtKessaiDateY
        '
        Me.txtKessaiDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateY.Location = New System.Drawing.Point(292, 156)
        Me.txtKessaiDateY.MaxLength = 4
        Me.txtKessaiDateY.Name = "txtKessaiDateY"
        Me.txtKessaiDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtKessaiDateY.TabIndex = 0
        '
        'txtKessaiDateM
        '
        Me.txtKessaiDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateM.Location = New System.Drawing.Point(364, 156)
        Me.txtKessaiDateM.MaxLength = 2
        Me.txtKessaiDateM.Name = "txtKessaiDateM"
        Me.txtKessaiDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtKessaiDateM.TabIndex = 1
        '
        'txtKessaiDateD
        '
        Me.txtKessaiDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateD.Location = New System.Drawing.Point(416, 156)
        Me.txtKessaiDateD.MaxLength = 2
        Me.txtKessaiDateD.Name = "txtKessaiDateD"
        Me.txtKessaiDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtKessaiDateD.TabIndex = 2
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(338, 160)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 23
        Me.Label8.Text = "年"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(390, 160)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 24
        Me.Label9.Text = "月"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(442, 160)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 25
        Me.Label10.Text = "日"
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(530, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 6
        Me.btnAction.Text = "印　刷"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 7
        Me.btnEnd.Text = "終　了"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(472, 160)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(24, 16)
        Me.Label4.TabIndex = 25
        Me.Label4.Text = "～"
        '
        'txtKessaiDateY2
        '
        Me.txtKessaiDateY2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateY2.Location = New System.Drawing.Point(502, 157)
        Me.txtKessaiDateY2.MaxLength = 4
        Me.txtKessaiDateY2.Name = "txtKessaiDateY2"
        Me.txtKessaiDateY2.Size = New System.Drawing.Size(44, 23)
        Me.txtKessaiDateY2.TabIndex = 3
        '
        'txtKessaiDateM2
        '
        Me.txtKessaiDateM2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateM2.Location = New System.Drawing.Point(574, 157)
        Me.txtKessaiDateM2.MaxLength = 2
        Me.txtKessaiDateM2.Name = "txtKessaiDateM2"
        Me.txtKessaiDateM2.Size = New System.Drawing.Size(24, 23)
        Me.txtKessaiDateM2.TabIndex = 4
        '
        'txtKessaiDateD2
        '
        Me.txtKessaiDateD2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtKessaiDateD2.Location = New System.Drawing.Point(626, 157)
        Me.txtKessaiDateD2.MaxLength = 2
        Me.txtKessaiDateD2.Name = "txtKessaiDateD2"
        Me.txtKessaiDateD2.Size = New System.Drawing.Size(24, 23)
        Me.txtKessaiDateD2.TabIndex = 5
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(548, 161)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(24, 16)
        Me.Label5.TabIndex = 23
        Me.Label5.Text = "年"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(600, 161)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(24, 16)
        Me.Label7.TabIndex = 24
        Me.Label7.Text = "月"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(656, 161)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(24, 16)
        Me.Label11.TabIndex = 25
        Me.Label11.Text = "日"
        '
        'KFKPRNT040
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 12)
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtKessaiDateD2)
        Me.Controls.Add(Me.txtKessaiDateD)
        Me.Controls.Add(Me.txtKessaiDateM2)
        Me.Controls.Add(Me.txtKessaiDateM)
        Me.Controls.Add(Me.txtKessaiDateY2)
        Me.Controls.Add(Me.txtKessaiDateY)
        Me.Controls.Add(Me.Label3)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFKPRNT040"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFKPRNT040"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

End Class
