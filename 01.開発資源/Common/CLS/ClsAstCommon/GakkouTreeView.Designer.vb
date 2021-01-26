<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GakkouTreeView
    Inherits System.Windows.Forms.UserControl

    'UserControl はコンポーネント一覧をクリーンアップするために dispose をオーバーライドします。
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TreeView = New System.Windows.Forms.TreeView()
        Me.LabelItakuKName = New System.Windows.Forms.Label()
        Me.TextBoxItakuKName = New System.Windows.Forms.TextBox()
        Me.CmdCIFForm = New System.Windows.Forms.Button()
        Me.LabelItakuCode = New System.Windows.Forms.Label()
        Me.TextBoxItakuCode = New System.Windows.Forms.TextBox()
        Me.CmdCIFClear = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'TreeView
        '
        Me.TreeView.Location = New System.Drawing.Point(5, 55)
        Me.TreeView.Name = "TreeView"
        Me.TreeView.ShowLines = False
        Me.TreeView.ShowPlusMinus = False
        Me.TreeView.ShowRootLines = False
        Me.TreeView.Size = New System.Drawing.Size(190, 440)
        Me.TreeView.TabIndex = 3
        Me.TreeView.TabStop = False
        '
        'LabelItakuKName
        '
        Me.LabelItakuKName.AutoSize = True
        Me.LabelItakuKName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelItakuKName.Location = New System.Drawing.Point(3, 12)
        Me.LabelItakuKName.Name = "LabelItakuKName"
        Me.LabelItakuKName.Size = New System.Drawing.Size(53, 12)
        Me.LabelItakuKName.TabIndex = 4
        Me.LabelItakuKName.Text = "カナ検索"
        '
        'TextBoxItakuKName
        '
        Me.TextBoxItakuKName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItakuKName.ImeMode = System.Windows.Forms.ImeMode.KatakanaHalf
        Me.TextBoxItakuKName.Location = New System.Drawing.Point(74, 8)
        Me.TextBoxItakuKName.MaxLength = 3
        Me.TextBoxItakuKName.Name = "TextBoxItakuKName"
        Me.TextBoxItakuKName.Size = New System.Drawing.Size(25, 19)
        Me.TextBoxItakuKName.TabIndex = 0
        '
        'CmdCIFForm
        '
        Me.CmdCIFForm.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdCIFForm.Location = New System.Drawing.Point(146, 30)
        Me.CmdCIFForm.Name = "CmdCIFForm"
        Me.CmdCIFForm.Size = New System.Drawing.Size(49, 21)
        Me.CmdCIFForm.TabIndex = 2
        Me.CmdCIFForm.Text = "参 照"
        '
        'LabelItakuCode
        '
        Me.LabelItakuCode.AutoSize = True
        Me.LabelItakuCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelItakuCode.Location = New System.Drawing.Point(3, 34)
        Me.LabelItakuCode.Name = "LabelItakuCode"
        Me.LabelItakuCode.Size = New System.Drawing.Size(65, 12)
        Me.LabelItakuCode.TabIndex = 5
        Me.LabelItakuCode.Text = "コード検索"
        '
        'TextBoxItakuCode
        '
        Me.TextBoxItakuCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItakuCode.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.TextBoxItakuCode.Location = New System.Drawing.Point(74, 31)
        Me.TextBoxItakuCode.MaxLength = 10
        Me.TextBoxItakuCode.Name = "TextBoxItakuCode"
        Me.TextBoxItakuCode.Size = New System.Drawing.Size(66, 19)
        Me.TextBoxItakuCode.TabIndex = 1
        '
        'CmdCIFClear
        '
        Me.CmdCIFClear.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdCIFClear.Location = New System.Drawing.Point(146, 6)
        Me.CmdCIFClear.Name = "CmdCIFClear"
        Me.CmdCIFClear.Size = New System.Drawing.Size(49, 21)
        Me.CmdCIFClear.TabIndex = 6
        Me.CmdCIFClear.Text = "クリア"
        '
        'GakkouTreeView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.CmdCIFClear)
        Me.Controls.Add(Me.TextBoxItakuCode)
        Me.Controls.Add(Me.LabelItakuCode)
        Me.Controls.Add(Me.CmdCIFForm)
        Me.Controls.Add(Me.TextBoxItakuKName)
        Me.Controls.Add(Me.LabelItakuKName)
        Me.Controls.Add(Me.TreeView)
        Me.Name = "GakkouTreeView"
        Me.Size = New System.Drawing.Size(200, 508)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents TreeView As System.Windows.Forms.TreeView
    Public WithEvents LabelItakuKName As System.Windows.Forms.Label
    Public WithEvents TextBoxItakuKName As System.Windows.Forms.TextBox
    Public WithEvents CmdCIFForm As System.Windows.Forms.Button

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events
    '非許可文字設定
    Private CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)
    Private CASTx2 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Public Sub New()
        MyBase.New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        ' 検索条件　カナ委託者名
        AddHandler Me.TextBoxItakuKName.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.TextBoxItakuKName.KeyPress, AddressOf CASTx1.KeyPress
        AddHandler Me.TextBoxItakuKName.LostFocus, AddressOf CAST.LostFocus

        ' 検索条件　委託者コード
        AddHandler Me.TextBoxItakuCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler Me.TextBoxItakuCode.KeyPress, AddressOf CASTx2.KeyPress
        AddHandler Me.TextBoxItakuCode.LostFocus, AddressOf CAST.LostFocus

    End Sub
    Friend WithEvents LabelItakuCode As System.Windows.Forms.Label
    Friend WithEvents TextBoxItakuCode As System.Windows.Forms.TextBox
    Friend WithEvents CmdCIFClear As System.Windows.Forms.Button
End Class
