<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAST020
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

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '振替日(年)
        AddHandler FURI_DATE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler FURI_DATE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_S.LostFocus, AddressOf CAST.LostFocus

        '振替日(月)
        AddHandler FURI_DATE_S1.GotFocus, AddressOf CAST.GotFocus
        AddHandler FURI_DATE_S1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_S1.LostFocus, AddressOf CAST.LostFocus

        'オプションボタン       
        AddHandler opAddNew.GotFocus, AddressOf CAST.GotFocus
        AddHandler opAddNew.KeyPress, AddressOf CAST.KeyPress
        AddHandler opAddNew.LostFocus, AddressOf CAST.LostFocus

        AddHandler opAppend.GotFocus, AddressOf CAST.GotFocus
        AddHandler opAppend.KeyPress, AddressOf CAST.KeyPress
        AddHandler opAppend.LostFocus, AddressOf CAST.LostFocus

    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAST020))
        Me.lbluser = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.opAddNew = New System.Windows.Forms.RadioButton()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.FURI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.FURI_DATE_S = New System.Windows.Forms.TextBox()
        Me.opAppend = New System.Windows.Forms.RadioButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.CmdAction = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbluser
        '
        Me.lbluser.AutoSize = True
        Me.lbluser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lbluser.Location = New System.Drawing.Point(682, 9)
        Me.lbluser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lbluser.Name = "lbluser"
        Me.lbluser.Size = New System.Drawing.Size(35, 12)
        Me.lbluser.TabIndex = 9
        Me.lbluser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 11
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(599, 27)
        Me.Label3.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 12)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(615, 9)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "ログイン名　:"
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(208, 138)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(424, 32)
        Me.Label6.TabIndex = 13
        Me.Label6.Text = "※新規・再作成を選択した場合は、未使用のスケジュール　を一旦削除してからスケジュール作成を行います"
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(208, 178)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(380, 32)
        Me.Label7.TabIndex = 14
        Me.Label7.Text = "※追加作成を選択した場合は、取引先マスタを　　　修正・登録したもののみスケジュール作成します"
        '
        'opAddNew
        '
        Me.opAddNew.Checked = True
        Me.opAddNew.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.opAddNew.Location = New System.Drawing.Point(250, 98)
        Me.opAddNew.Name = "opAddNew"
        Me.opAddNew.Size = New System.Drawing.Size(138, 24)
        Me.opAddNew.TabIndex = 2
        Me.opAddNew.Text = "新規・再作成"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(469, 61)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 16
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(417, 61)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 15
        Me.Label8.Text = "年"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(289, 61)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(76, 16)
        Me.Label10.TabIndex = 12
        Me.Label10.Text = "対象年月"
        '
        'Label11
        '
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(0, 8)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(795, 30)
        Me.Label11.TabIndex = 7
        Me.Label11.Text = "＜月間スケジュール作成＞"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'FURI_DATE_S1
        '
        Me.FURI_DATE_S1.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_S1.Location = New System.Drawing.Point(443, 57)
        Me.FURI_DATE_S1.MaxLength = 2
        Me.FURI_DATE_S1.Name = "FURI_DATE_S1"
        Me.FURI_DATE_S1.Size = New System.Drawing.Size(24, 23)
        Me.FURI_DATE_S1.TabIndex = 1
        '
        'FURI_DATE_S
        '
        Me.FURI_DATE_S.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_S.Location = New System.Drawing.Point(367, 57)
        Me.FURI_DATE_S.MaxLength = 4
        Me.FURI_DATE_S.Name = "FURI_DATE_S"
        Me.FURI_DATE_S.Size = New System.Drawing.Size(48, 23)
        Me.FURI_DATE_S.TabIndex = 0
        '
        'opAppend
        '
        Me.opAppend.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.opAppend.Location = New System.Drawing.Point(431, 98)
        Me.opAppend.Name = "opAppend"
        Me.opAppend.Size = New System.Drawing.Size(112, 24)
        Me.opAppend.TabIndex = 3
        Me.opAppend.Text = "追加作成"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ListView1)
        Me.GroupBox1.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(40, 226)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(720, 256)
        Me.GroupBox1.TabIndex = 6
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "休日情報"
        '
        'ListView1
        '
        Me.ListView1.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.Location = New System.Drawing.Point(10, 31)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(700, 208)
        Me.ListView1.TabIndex = 0
        Me.ListView1.TabStop = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'CmdBack
        '
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdBack.Location = New System.Drawing.Point(660, 520)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(120, 40)
        Me.CmdBack.TabIndex = 5
        Me.CmdBack.Text = "終　了"
        Me.CmdBack.UseVisualStyleBackColor = True
        '
        'CmdAction
        '
        Me.CmdAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdAction.Location = New System.Drawing.Point(170, 520)
        Me.CmdAction.Name = "CmdAction"
        Me.CmdAction.Size = New System.Drawing.Size(120, 40)
        Me.CmdAction.TabIndex = 4
        Me.CmdAction.Text = "作　成"
        Me.CmdAction.UseVisualStyleBackColor = True
        '
        'KFJMAST020
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.CmdAction)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.opAddNew)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.FURI_DATE_S1)
        Me.Controls.Add(Me.FURI_DATE_S)
        Me.Controls.Add(Me.opAppend)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lbluser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label11)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMAST020"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAST020"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbluser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents opAddNew As System.Windows.Forms.RadioButton
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents FURI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents FURI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents opAppend As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents CmdAction As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
End Class
