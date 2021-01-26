<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAIN020
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '取引先カナコンボボックス
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress

        '取引先名コンボボックス
        AddHandler cmbToriName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbToriName.LostFocus, AddressOf CAST.LostFocus
        AddHandler cmbToriName.KeyPress, AddressOf CAST.KeyPress

        '取引先主コード
        AddHandler txtTorisCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorisCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorisCode.LostFocus, AddressOf CASTx01.LostFocus

        '取引先副コード
        AddHandler txtTorifCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorifCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorifCode.LostFocus, AddressOf CASTx01.LostFocus

        ' 振替年
        AddHandler txtFuriDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateY.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPress

        ' 振替月
        AddHandler txtFuriDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateM.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPress

        ' 振替日
        AddHandler txtFuriDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateD.LostFocus, AddressOf CASTx01.LostFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPress

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAIN020))
        Me.lblUser = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.lblToriSearch = New System.Windows.Forms.Label
        Me.lblToriHyphen = New System.Windows.Forms.Label
        Me.txtTorifCode = New System.Windows.Forms.TextBox
        Me.txtTorisCode = New System.Windows.Forms.TextBox
        Me.lblToriCode = New System.Windows.Forms.Label
        Me.lblFuriDateD = New System.Windows.Forms.Label
        Me.lblFuriDateY = New System.Windows.Forms.Label
        Me.txtFuriDateD = New System.Windows.Forms.TextBox
        Me.txtFuriDateM = New System.Windows.Forms.TextBox
        Me.txtFuriDateY = New System.Windows.Forms.TextBox
        Me.lblFuriDateM = New System.Windows.Forms.Label
        Me.lblFuriDate = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnAction = New System.Windows.Forms.Button
        Me.cmbToriName = New System.Windows.Forms.ComboBox
        Me.cmbKana = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(683, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 3
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 28)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 4
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(603, 28)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(619, 9)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ログイン名　:"
        '
        'Label13
        '
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(0, 8)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(795, 30)
        Me.Label13.TabIndex = 0
        Me.Label13.Text = "＜他行データ作成＞"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblToriSearch
        '
        Me.lblToriSearch.AutoSize = True
        Me.lblToriSearch.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToriSearch.Location = New System.Drawing.Point(165, 156)
        Me.lblToriSearch.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToriSearch.Name = "lblToriSearch"
        Me.lblToriSearch.Size = New System.Drawing.Size(88, 16)
        Me.lblToriSearch.TabIndex = 5
        Me.lblToriSearch.Text = "取引先検索"
        '
        'lblToriHyphen
        '
        Me.lblToriHyphen.AutoSize = True
        Me.lblToriHyphen.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToriHyphen.Location = New System.Drawing.Point(368, 221)
        Me.lblToriHyphen.Name = "lblToriHyphen"
        Me.lblToriHyphen.Size = New System.Drawing.Size(24, 16)
        Me.lblToriHyphen.TabIndex = 10
        Me.lblToriHyphen.Text = "－"
        '
        'txtTorifCode
        '
        Me.txtTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorifCode.Location = New System.Drawing.Point(394, 217)
        Me.txtTorifCode.MaxLength = 2
        Me.txtTorifCode.Name = "txtTorifCode"
        Me.txtTorifCode.Size = New System.Drawing.Size(24, 23)
        Me.txtTorifCode.TabIndex = 11
        '
        'txtTorisCode
        '
        Me.txtTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorisCode.Location = New System.Drawing.Point(276, 217)
        Me.txtTorisCode.MaxLength = 10
        Me.txtTorisCode.Name = "txtTorisCode"
        Me.txtTorisCode.Size = New System.Drawing.Size(90, 23)
        Me.txtTorisCode.TabIndex = 9
        '
        'lblToriCode
        '
        Me.lblToriCode.AutoSize = True
        Me.lblToriCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToriCode.Location = New System.Drawing.Point(165, 220)
        Me.lblToriCode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToriCode.Name = "lblToriCode"
        Me.lblToriCode.Size = New System.Drawing.Size(104, 16)
        Me.lblToriCode.TabIndex = 8
        Me.lblToriCode.Text = "取引先コード"
        '
        'lblFuriDateD
        '
        Me.lblFuriDateD.AutoSize = True
        Me.lblFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateD.Location = New System.Drawing.Point(426, 286)
        Me.lblFuriDateD.Name = "lblFuriDateD"
        Me.lblFuriDateD.Size = New System.Drawing.Size(24, 16)
        Me.lblFuriDateD.TabIndex = 18
        Me.lblFuriDateD.Text = "日"
        '
        'lblFuriDateY
        '
        Me.lblFuriDateY.AutoSize = True
        Me.lblFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateY.Location = New System.Drawing.Point(322, 286)
        Me.lblFuriDateY.Name = "lblFuriDateY"
        Me.lblFuriDateY.Size = New System.Drawing.Size(24, 16)
        Me.lblFuriDateY.TabIndex = 14
        Me.lblFuriDateY.Text = "年"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.Location = New System.Drawing.Point(400, 282)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 17
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.Location = New System.Drawing.Point(348, 282)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 15
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.Location = New System.Drawing.Point(276, 282)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 13
        '
        'lblFuriDateM
        '
        Me.lblFuriDateM.AutoSize = True
        Me.lblFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateM.Location = New System.Drawing.Point(374, 286)
        Me.lblFuriDateM.Name = "lblFuriDateM"
        Me.lblFuriDateM.Size = New System.Drawing.Size(24, 16)
        Me.lblFuriDateM.TabIndex = 16
        Me.lblFuriDateM.Text = "月"
        '
        'lblFuriDate
        '
        Me.lblFuriDate.AutoSize = True
        Me.lblFuriDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDate.Location = New System.Drawing.Point(165, 286)
        Me.lblFuriDate.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblFuriDate.Name = "lblFuriDate"
        Me.lblFuriDate.Size = New System.Drawing.Size(88, 16)
        Me.lblFuriDate.TabIndex = 12
        Me.lblFuriDate.Text = "振　替　日"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 20
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(140, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 19
        Me.btnAction.Text = "作　成"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'cmbToriName
        '
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbToriName.FormattingEnabled = True
        Me.cmbToriName.Location = New System.Drawing.Point(325, 152)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(355, 21)
        Me.cmbToriName.TabIndex = 7
        Me.cmbToriName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbKana.FormattingEnabled = True
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(276, 152)
        Me.cmbKana.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 6
        Me.cmbKana.TabStop = False
        '
        'KFJMAIN020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(794, 568)
        Me.Controls.Add(Me.cmbToriName)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.lblFuriDateD)
        Me.Controls.Add(Me.lblFuriDateY)
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
        Me.Controls.Add(Me.lblFuriDateM)
        Me.Controls.Add(Me.lblFuriDate)
        Me.Controls.Add(Me.lblToriHyphen)
        Me.Controls.Add(Me.txtTorifCode)
        Me.Controls.Add(Me.txtTorisCode)
        Me.Controls.Add(Me.lblToriCode)
        Me.Controls.Add(Me.lblToriSearch)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label13)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "KFJMAIN020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAIN020"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents lblToriSearch As System.Windows.Forms.Label
    Friend WithEvents lblToriHyphen As System.Windows.Forms.Label
    Friend WithEvents txtTorifCode As System.Windows.Forms.TextBox
    Friend WithEvents txtTorisCode As System.Windows.Forms.TextBox
    Friend WithEvents lblToriCode As System.Windows.Forms.Label
    Friend WithEvents lblFuriDateD As System.Windows.Forms.Label
    Friend WithEvents lblFuriDateY As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents lblFuriDateM As System.Windows.Forms.Label
    Friend WithEvents lblFuriDate As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
End Class
