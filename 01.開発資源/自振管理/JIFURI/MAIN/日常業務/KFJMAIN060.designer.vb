<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAIN060
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '取引先主コード
        AddHandler txtTorisCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorisCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorisCode.LostFocus, AddressOf CASTx01.LostFocus

        '取引先副コード
        AddHandler txtTorifCode.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtTorifCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorifCode.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(年)
        AddHandler txtFuriDateY.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateY.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(月)
        AddHandler txtFuriDateM.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateM.LostFocus, AddressOf CASTx01.LostFocus

        '振替日(日)
        AddHandler txtFuriDateD.GotFocus, AddressOf CASTx01.GotFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateD.LostFocus, AddressOf CASTx01.LostFocus
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAIN060))
        Me.lblUser = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblFuriDateD = New System.Windows.Forms.Label()
        Me.lblFuriDateY = New System.Windows.Forms.Label()
        Me.txtFuriDateD = New System.Windows.Forms.TextBox()
        Me.txtFuriDateM = New System.Windows.Forms.TextBox()
        Me.txtFuriDateY = New System.Windows.Forms.TextBox()
        Me.lblFuriDateM = New System.Windows.Forms.Label()
        Me.lblFuriDate = New System.Windows.Forms.Label()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnAction = New System.Windows.Forms.Button()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ColumnHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lblTorifCode = New System.Windows.Forms.Label()
        Me.txtTorifCode = New System.Windows.Forms.TextBox()
        Me.txtTorisCode = New System.Windows.Forms.TextBox()
        Me.lblTorisCode = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.btnSelect = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(682, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 2
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 4
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(599, 27)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(615, 9)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ログイン名　:"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "＜返還データ作成＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblFuriDateD
        '
        Me.lblFuriDateD.AutoSize = True
        Me.lblFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateD.Location = New System.Drawing.Point(743, 467)
        Me.lblFuriDateD.Name = "lblFuriDateD"
        Me.lblFuriDateD.Size = New System.Drawing.Size(24, 16)
        Me.lblFuriDateD.TabIndex = 11
        Me.lblFuriDateD.Text = "日"
        '
        'lblFuriDateY
        '
        Me.lblFuriDateY.AutoSize = True
        Me.lblFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateY.Location = New System.Drawing.Point(639, 467)
        Me.lblFuriDateY.Name = "lblFuriDateY"
        Me.lblFuriDateY.Size = New System.Drawing.Size(24, 16)
        Me.lblFuriDateY.TabIndex = 7
        Me.lblFuriDateY.Text = "年"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.Location = New System.Drawing.Point(717, 463)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 10
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.Location = New System.Drawing.Point(665, 463)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 8
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.Location = New System.Drawing.Point(593, 463)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 6
        '
        'lblFuriDateM
        '
        Me.lblFuriDateM.AutoSize = True
        Me.lblFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDateM.Location = New System.Drawing.Point(691, 467)
        Me.lblFuriDateM.Name = "lblFuriDateM"
        Me.lblFuriDateM.Size = New System.Drawing.Size(24, 16)
        Me.lblFuriDateM.TabIndex = 9
        Me.lblFuriDateM.Text = "月"
        '
        'lblFuriDate
        '
        Me.lblFuriDate.AutoSize = True
        Me.lblFuriDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblFuriDate.Location = New System.Drawing.Point(482, 467)
        Me.lblFuriDate.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblFuriDate.Name = "lblFuriDate"
        Me.lblFuriDate.Size = New System.Drawing.Size(88, 16)
        Me.lblFuriDate.TabIndex = 5
        Me.lblFuriDate.Text = "振　替　日"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(660, 520)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 19
        Me.btnEnd.Text = "終　了"
        Me.btnEnd.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnPrint.Location = New System.Drawing.Point(530, 520)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(120, 40)
        Me.btnPrint.TabIndex = 18
        Me.btnPrint.Text = "印　刷"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'btnAction
        '
        Me.btnAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnAction.Location = New System.Drawing.Point(130, 520)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(120, 40)
        Me.btnAction.TabIndex = 17
        Me.btnAction.Text = "作　成"
        Me.btnAction.UseVisualStyleBackColor = True
        '
        'ListView1
        '
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader, Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6, Me.ColumnHeader7, Me.ColumnHeader8})
        Me.ListView1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(12, 57)
        Me.ListView1.MultiSelect = False
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(768, 400)
        Me.ListView1.TabIndex = 20
        Me.ListView1.TabStop = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader
        '
        Me.ColumnHeader.Text = ""
        Me.ColumnHeader.Width = 20
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "項番"
        Me.ColumnHeader1.Width = 40
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "取引先名"
        Me.ColumnHeader2.Width = 235
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "取引先コード"
        Me.ColumnHeader3.Width = 100
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "振替日"
        Me.ColumnHeader4.Width = 90
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "振替コード"
        Me.ColumnHeader5.Width = 75
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "企業コード"
        Me.ColumnHeader6.Width = 70
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "媒体コード"
        Me.ColumnHeader7.Width = 75
        '
        'ColumnHeader8
        '
        Me.ColumnHeader8.Text = "処理済"
        '
        'lblTorifCode
        '
        Me.lblTorifCode.AutoSize = True
        Me.lblTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTorifCode.Location = New System.Drawing.Point(683, 494)
        Me.lblTorifCode.Name = "lblTorifCode"
        Me.lblTorifCode.Size = New System.Drawing.Size(24, 16)
        Me.lblTorifCode.TabIndex = 14
        Me.lblTorifCode.Text = "－"
        '
        'txtTorifCode
        '
        Me.txtTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorifCode.Location = New System.Drawing.Point(709, 490)
        Me.txtTorifCode.MaxLength = 2
        Me.txtTorifCode.Name = "txtTorifCode"
        Me.txtTorifCode.Size = New System.Drawing.Size(24, 23)
        Me.txtTorifCode.TabIndex = 15
        '
        'txtTorisCode
        '
        Me.txtTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorisCode.Location = New System.Drawing.Point(593, 490)
        Me.txtTorisCode.MaxLength = 10
        Me.txtTorisCode.Name = "txtTorisCode"
        Me.txtTorisCode.Size = New System.Drawing.Size(88, 23)
        Me.txtTorisCode.TabIndex = 13
        '
        'lblTorisCode
        '
        Me.lblTorisCode.AutoSize = True
        Me.lblTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTorisCode.Location = New System.Drawing.Point(482, 494)
        Me.lblTorisCode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTorisCode.Name = "lblTorisCode"
        Me.lblTorisCode.Size = New System.Drawing.Size(104, 16)
        Me.lblTorisCode.TabIndex = 12
        Me.lblTorisCode.Text = "取引先コード"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(14, 467)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(120, 16)
        Me.Label6.TabIndex = 21
        Me.Label6.Text = "※全件処理方法"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(31, 493)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(384, 16)
        Me.Label12.TabIndex = 22
        Me.Label12.Text = "ALL1:依頼書　ALL2:伝票　ALL3:伝送　ALL4:WEB伝送"
        '
        'btnSelect
        '
        Me.btnSelect.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnSelect.Location = New System.Drawing.Point(257, 520)
        Me.btnSelect.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(120, 40)
        Me.btnSelect.TabIndex = 16
        Me.btnSelect.Text = "参　照"
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'KFJMAIN060
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.btnSelect)
        Me.Controls.Add(Me.lblTorifCode)
        Me.Controls.Add(Me.txtTorifCode)
        Me.Controls.Add(Me.txtTorisCode)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.lblTorisCode)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.btnAction)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.lblFuriDateD)
        Me.Controls.Add(Me.lblFuriDateY)
        Me.Controls.Add(Me.txtFuriDateD)
        Me.Controls.Add(Me.txtFuriDateM)
        Me.Controls.Add(Me.txtFuriDateY)
        Me.Controls.Add(Me.lblFuriDateM)
        Me.Controls.Add(Me.lblFuriDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMAIN060"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAIN060"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblFuriDateD As System.Windows.Forms.Label
    Friend WithEvents lblFuriDateY As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents lblFuriDateM As System.Windows.Forms.Label
    Friend WithEvents lblFuriDate As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents lblTorifCode As System.Windows.Forms.Label
    Friend WithEvents txtTorifCode As System.Windows.Forms.TextBox
    Friend WithEvents txtTorisCode As System.Windows.Forms.TextBox
    Friend WithEvents lblTorisCode As System.Windows.Forms.Label
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader8 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents btnSelect As System.Windows.Forms.Button
End Class
