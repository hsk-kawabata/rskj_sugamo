<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFSMENU010
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

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        If GCom Is Nothing Then
            GCom = New MenteCommon.clsCommon
        End If
    End Sub

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    ' Windows フォーム デザイナを使って変更してください。  
    ' コード エディタは使用しないでください。
    Friend WithEvents tbpMast As System.Windows.Forms.TabPage
    Friend WithEvents tbpEntry As System.Windows.Forms.TabPage
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents TAB As System.Windows.Forms.TabControl
    Friend WithEvents tbpDailyJob As System.Windows.Forms.TabPage
    Friend WithEvents tbpPrint As System.Windows.Forms.TabPage
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button16 As System.Windows.Forms.Button
    Friend WithEvents Button15 As System.Windows.Forms.Button
    Friend WithEvents Button14 As System.Windows.Forms.Button
    Friend WithEvents Button13 As System.Windows.Forms.Button
    Friend WithEvents Button12 As System.Windows.Forms.Button
    Friend WithEvents Button11 As System.Windows.Forms.Button
    Friend WithEvents Button10 As System.Windows.Forms.Button
    Friend WithEvents Button9 As System.Windows.Forms.Button
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button17 As System.Windows.Forms.Button
    Friend WithEvents Button18 As System.Windows.Forms.Button
    Friend WithEvents Button19 As System.Windows.Forms.Button
    Friend WithEvents Button20 As System.Windows.Forms.Button
    Friend WithEvents Button21 As System.Windows.Forms.Button
    Friend WithEvents Button22 As System.Windows.Forms.Button
    Friend WithEvents Button23 As System.Windows.Forms.Button
    Friend WithEvents Button24 As System.Windows.Forms.Button
    Friend WithEvents Button25 As System.Windows.Forms.Button
    Friend WithEvents Button26 As System.Windows.Forms.Button
    Friend WithEvents Button27 As System.Windows.Forms.Button
    Friend WithEvents Button28 As System.Windows.Forms.Button
    Friend WithEvents Button29 As System.Windows.Forms.Button
    Friend WithEvents Button30 As System.Windows.Forms.Button
    Friend WithEvents Button31 As System.Windows.Forms.Button
    Friend WithEvents Button32 As System.Windows.Forms.Button
    Friend WithEvents Button34 As System.Windows.Forms.Button
    Friend WithEvents Button35 As System.Windows.Forms.Button
    Friend WithEvents Button36 As System.Windows.Forms.Button
    Friend WithEvents Button37 As System.Windows.Forms.Button
    Friend WithEvents Button38 As System.Windows.Forms.Button
    Friend WithEvents Button39 As System.Windows.Forms.Button
    Friend WithEvents Button40 As System.Windows.Forms.Button
    Friend WithEvents Button41 As System.Windows.Forms.Button
    Friend WithEvents Button42 As System.Windows.Forms.Button
    Friend WithEvents Button43 As System.Windows.Forms.Button
    Friend WithEvents Button44 As System.Windows.Forms.Button
    Friend WithEvents Button45 As System.Windows.Forms.Button
    Friend WithEvents Button46 As System.Windows.Forms.Button
    Friend WithEvents Button47 As System.Windows.Forms.Button
    Friend WithEvents Button48 As System.Windows.Forms.Button
    Friend WithEvents Button49 As System.Windows.Forms.Button
    Friend WithEvents Button50 As System.Windows.Forms.Button
    Friend WithEvents Button51 As System.Windows.Forms.Button
    Friend WithEvents Button52 As System.Windows.Forms.Button
    Friend WithEvents Button53 As System.Windows.Forms.Button
    Friend WithEvents Button54 As System.Windows.Forms.Button
    Friend WithEvents Button55 As System.Windows.Forms.Button
    Friend WithEvents Button56 As System.Windows.Forms.Button
    Friend WithEvents Button57 As System.Windows.Forms.Button
    Friend WithEvents Button58 As System.Windows.Forms.Button
    Friend WithEvents Button59 As System.Windows.Forms.Button
    Friend WithEvents Button60 As System.Windows.Forms.Button
    Friend WithEvents Button61 As System.Windows.Forms.Button
    Friend WithEvents Button62 As System.Windows.Forms.Button
    Friend WithEvents Button63 As System.Windows.Forms.Button
    Friend WithEvents Button64 As System.Windows.Forms.Button
    Friend WithEvents Button65 As System.Windows.Forms.Button
    Friend WithEvents Button66 As System.Windows.Forms.Button
    Friend WithEvents Button67 As System.Windows.Forms.Button
    Friend WithEvents Button68 As System.Windows.Forms.Button
    Friend WithEvents Button69 As System.Windows.Forms.Button
    Friend WithEvents Button70 As System.Windows.Forms.Button
    Friend WithEvents Button71 As System.Windows.Forms.Button
    Friend WithEvents Button72 As System.Windows.Forms.Button
    Friend WithEvents Button73 As System.Windows.Forms.Button
    Friend WithEvents Button74 As System.Windows.Forms.Button
    Friend WithEvents Button75 As System.Windows.Forms.Button
    Friend WithEvents Button76 As System.Windows.Forms.Button
    Friend WithEvents Button77 As System.Windows.Forms.Button
    Friend WithEvents Button78 As System.Windows.Forms.Button
    Friend WithEvents Button79 As System.Windows.Forms.Button
    Friend WithEvents Button80 As System.Windows.Forms.Button
    Friend WithEvents Button81 As System.Windows.Forms.Button
    Friend WithEvents tbpOther As System.Windows.Forms.TabPage
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFSMENU010))
        Me.TAB = New System.Windows.Forms.TabControl()
        Me.tbpDailyJob = New System.Windows.Forms.TabPage()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button16 = New System.Windows.Forms.Button()
        Me.Button15 = New System.Windows.Forms.Button()
        Me.Button14 = New System.Windows.Forms.Button()
        Me.Button13 = New System.Windows.Forms.Button()
        Me.Button12 = New System.Windows.Forms.Button()
        Me.Button11 = New System.Windows.Forms.Button()
        Me.Button10 = New System.Windows.Forms.Button()
        Me.Button9 = New System.Windows.Forms.Button()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.Button7 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.tbpMast = New System.Windows.Forms.TabPage()
        Me.Button17 = New System.Windows.Forms.Button()
        Me.Button18 = New System.Windows.Forms.Button()
        Me.Button19 = New System.Windows.Forms.Button()
        Me.Button20 = New System.Windows.Forms.Button()
        Me.Button21 = New System.Windows.Forms.Button()
        Me.Button22 = New System.Windows.Forms.Button()
        Me.Button23 = New System.Windows.Forms.Button()
        Me.Button24 = New System.Windows.Forms.Button()
        Me.Button25 = New System.Windows.Forms.Button()
        Me.Button26 = New System.Windows.Forms.Button()
        Me.Button27 = New System.Windows.Forms.Button()
        Me.Button28 = New System.Windows.Forms.Button()
        Me.Button29 = New System.Windows.Forms.Button()
        Me.Button30 = New System.Windows.Forms.Button()
        Me.Button31 = New System.Windows.Forms.Button()
        Me.Button32 = New System.Windows.Forms.Button()
        Me.tbpEntry = New System.Windows.Forms.TabPage()
        Me.Button34 = New System.Windows.Forms.Button()
        Me.Button35 = New System.Windows.Forms.Button()
        Me.Button36 = New System.Windows.Forms.Button()
        Me.Button37 = New System.Windows.Forms.Button()
        Me.Button38 = New System.Windows.Forms.Button()
        Me.Button39 = New System.Windows.Forms.Button()
        Me.Button40 = New System.Windows.Forms.Button()
        Me.Button41 = New System.Windows.Forms.Button()
        Me.Button42 = New System.Windows.Forms.Button()
        Me.Button43 = New System.Windows.Forms.Button()
        Me.Button44 = New System.Windows.Forms.Button()
        Me.Button45 = New System.Windows.Forms.Button()
        Me.Button46 = New System.Windows.Forms.Button()
        Me.Button47 = New System.Windows.Forms.Button()
        Me.Button48 = New System.Windows.Forms.Button()
        Me.Button49 = New System.Windows.Forms.Button()
        Me.tbpOther = New System.Windows.Forms.TabPage()
        Me.Button50 = New System.Windows.Forms.Button()
        Me.Button51 = New System.Windows.Forms.Button()
        Me.Button52 = New System.Windows.Forms.Button()
        Me.Button53 = New System.Windows.Forms.Button()
        Me.Button54 = New System.Windows.Forms.Button()
        Me.Button55 = New System.Windows.Forms.Button()
        Me.Button56 = New System.Windows.Forms.Button()
        Me.Button57 = New System.Windows.Forms.Button()
        Me.Button58 = New System.Windows.Forms.Button()
        Me.Button59 = New System.Windows.Forms.Button()
        Me.Button60 = New System.Windows.Forms.Button()
        Me.Button61 = New System.Windows.Forms.Button()
        Me.Button62 = New System.Windows.Forms.Button()
        Me.Button63 = New System.Windows.Forms.Button()
        Me.Button64 = New System.Windows.Forms.Button()
        Me.Button65 = New System.Windows.Forms.Button()
        Me.tbpPrint = New System.Windows.Forms.TabPage()
        Me.Button66 = New System.Windows.Forms.Button()
        Me.Button67 = New System.Windows.Forms.Button()
        Me.Button68 = New System.Windows.Forms.Button()
        Me.Button69 = New System.Windows.Forms.Button()
        Me.Button70 = New System.Windows.Forms.Button()
        Me.Button71 = New System.Windows.Forms.Button()
        Me.Button72 = New System.Windows.Forms.Button()
        Me.Button73 = New System.Windows.Forms.Button()
        Me.Button74 = New System.Windows.Forms.Button()
        Me.Button75 = New System.Windows.Forms.Button()
        Me.Button76 = New System.Windows.Forms.Button()
        Me.Button77 = New System.Windows.Forms.Button()
        Me.Button78 = New System.Windows.Forms.Button()
        Me.Button79 = New System.Windows.Forms.Button()
        Me.Button80 = New System.Windows.Forms.Button()
        Me.Button81 = New System.Windows.Forms.Button()
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TAB.SuspendLayout()
        Me.tbpDailyJob.SuspendLayout()
        Me.tbpMast.SuspendLayout()
        Me.tbpEntry.SuspendLayout()
        Me.tbpOther.SuspendLayout()
        Me.tbpPrint.SuspendLayout()
        Me.SuspendLayout()
        '
        'TAB
        '
        Me.TAB.Controls.Add(Me.tbpDailyJob)
        Me.TAB.Controls.Add(Me.tbpMast)
        Me.TAB.Controls.Add(Me.tbpEntry)
        Me.TAB.Controls.Add(Me.tbpOther)
        Me.TAB.Controls.Add(Me.tbpPrint)
        Me.TAB.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.TAB.ItemSize = New System.Drawing.Size(100, 20)
        Me.TAB.Location = New System.Drawing.Point(20, 55)
        Me.TAB.Name = "TAB"
        Me.TAB.Padding = New System.Drawing.Point(12, 3)
        Me.TAB.SelectedIndex = 0
        Me.TAB.Size = New System.Drawing.Size(755, 455)
        Me.TAB.TabIndex = 1
        '
        'tbpDailyJob
        '
        Me.tbpDailyJob.BackColor = System.Drawing.Color.Transparent
        Me.tbpDailyJob.Controls.Add(Me.Button6)
        Me.tbpDailyJob.Controls.Add(Me.Button4)
        Me.tbpDailyJob.Controls.Add(Me.Button2)
        Me.tbpDailyJob.Controls.Add(Me.Button16)
        Me.tbpDailyJob.Controls.Add(Me.Button15)
        Me.tbpDailyJob.Controls.Add(Me.Button14)
        Me.tbpDailyJob.Controls.Add(Me.Button13)
        Me.tbpDailyJob.Controls.Add(Me.Button12)
        Me.tbpDailyJob.Controls.Add(Me.Button11)
        Me.tbpDailyJob.Controls.Add(Me.Button10)
        Me.tbpDailyJob.Controls.Add(Me.Button9)
        Me.tbpDailyJob.Controls.Add(Me.Button8)
        Me.tbpDailyJob.Controls.Add(Me.Button7)
        Me.tbpDailyJob.Controls.Add(Me.Button5)
        Me.tbpDailyJob.Controls.Add(Me.Button3)
        Me.tbpDailyJob.Controls.Add(Me.Button1)
        Me.tbpDailyJob.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.tbpDailyJob.Location = New System.Drawing.Point(4, 24)
        Me.tbpDailyJob.Name = "tbpDailyJob"
        Me.tbpDailyJob.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpDailyJob.Size = New System.Drawing.Size(747, 427)
        Me.tbpDailyJob.TabIndex = 0
        Me.tbpDailyJob.Text = "日常業務"
        Me.tbpDailyJob.UseVisualStyleBackColor = True
        '
        'Button6
        '
        Me.Button6.BackColor = System.Drawing.Color.Transparent
        Me.Button6.Enabled = False
        Me.Button6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button6.Location = New System.Drawing.Point(393, 118)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(300, 40)
        Me.Button6.TabIndex = 26
        Me.Button6.UseVisualStyleBackColor = False
        '
        'Button4
        '
        Me.Button4.BackColor = System.Drawing.Color.Transparent
        Me.Button4.Enabled = False
        Me.Button4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button4.Location = New System.Drawing.Point(393, 68)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(300, 40)
        Me.Button4.TabIndex = 25
        Me.Button4.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Transparent
        Me.Button2.Enabled = False
        Me.Button2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button2.Location = New System.Drawing.Point(393, 18)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(300, 40)
        Me.Button2.TabIndex = 24
        Me.Button2.Text = "依頼データ落込（一括）"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Button16
        '
        Me.Button16.BackColor = System.Drawing.Color.Transparent
        Me.Button16.Enabled = False
        Me.Button16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button16.Location = New System.Drawing.Point(393, 368)
        Me.Button16.Name = "Button16"
        Me.Button16.Size = New System.Drawing.Size(300, 40)
        Me.Button16.TabIndex = 31
        Me.Button16.UseVisualStyleBackColor = False
        '
        'Button15
        '
        Me.Button15.BackColor = System.Drawing.Color.Transparent
        Me.Button15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button15.Location = New System.Drawing.Point(53, 368)
        Me.Button15.Name = "Button15"
        Me.Button15.Size = New System.Drawing.Size(300, 40)
        Me.Button15.TabIndex = 23
        Me.Button15.Tag = ""
        Me.Button15.UseVisualStyleBackColor = False
        '
        'Button14
        '
        Me.Button14.BackColor = System.Drawing.Color.Transparent
        Me.Button14.Enabled = False
        Me.Button14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button14.Location = New System.Drawing.Point(393, 318)
        Me.Button14.Name = "Button14"
        Me.Button14.Size = New System.Drawing.Size(300, 40)
        Me.Button14.TabIndex = 30
        Me.Button14.UseVisualStyleBackColor = False
        '
        'Button13
        '
        Me.Button13.BackColor = System.Drawing.Color.Transparent
        Me.Button13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button13.Location = New System.Drawing.Point(53, 318)
        Me.Button13.Name = "Button13"
        Me.Button13.Size = New System.Drawing.Size(300, 40)
        Me.Button13.TabIndex = 22
        Me.Button13.Tag = ""
        Me.Button13.UseVisualStyleBackColor = False
        '
        'Button12
        '
        Me.Button12.BackColor = System.Drawing.Color.Transparent
        Me.Button12.Enabled = False
        Me.Button12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button12.Location = New System.Drawing.Point(393, 268)
        Me.Button12.Name = "Button12"
        Me.Button12.Size = New System.Drawing.Size(300, 40)
        Me.Button12.TabIndex = 29
        Me.Button12.UseVisualStyleBackColor = False
        '
        'Button11
        '
        Me.Button11.BackColor = System.Drawing.Color.Transparent
        Me.Button11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button11.Location = New System.Drawing.Point(53, 268)
        Me.Button11.Name = "Button11"
        Me.Button11.Size = New System.Drawing.Size(300, 40)
        Me.Button11.TabIndex = 21
        Me.Button11.Tag = "1"
        Me.Button11.Text = "振込発信データ作成（当日異例）"
        Me.Button11.UseVisualStyleBackColor = False
        '
        'Button10
        '
        Me.Button10.BackColor = System.Drawing.Color.Transparent
        Me.Button10.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button10.Enabled = False
        Me.Button10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button10.Location = New System.Drawing.Point(393, 218)
        Me.Button10.Name = "Button10"
        Me.Button10.Size = New System.Drawing.Size(300, 40)
        Me.Button10.TabIndex = 28
        Me.Button10.UseVisualStyleBackColor = False
        '
        'Button9
        '
        Me.Button9.BackColor = System.Drawing.Color.Transparent
        Me.Button9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button9.Location = New System.Drawing.Point(53, 218)
        Me.Button9.Name = "Button9"
        Me.Button9.Size = New System.Drawing.Size(300, 40)
        Me.Button9.TabIndex = 20
        Me.Button9.Tag = "1"
        Me.Button9.Text = "振込発信データ作成（通常定例）"
        Me.Button9.UseVisualStyleBackColor = False
        '
        'Button8
        '
        Me.Button8.BackColor = System.Drawing.Color.Transparent
        Me.Button8.Enabled = False
        Me.Button8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button8.Location = New System.Drawing.Point(393, 168)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(300, 40)
        Me.Button8.TabIndex = 27
        Me.Button8.UseVisualStyleBackColor = False
        '
        'Button7
        '
        Me.Button7.BackColor = System.Drawing.Color.Transparent
        Me.Button7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button7.Location = New System.Drawing.Point(53, 168)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(300, 40)
        Me.Button7.TabIndex = 19
        Me.Button7.Tag = ""
        Me.Button7.Text = "振込発信リエンタ書込"
        Me.Button7.UseVisualStyleBackColor = False
        '
        'Button5
        '
        Me.Button5.BackColor = System.Drawing.Color.Transparent
        Me.Button5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button5.Location = New System.Drawing.Point(53, 118)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(300, 40)
        Me.Button5.TabIndex = 18
        Me.Button5.Tag = "1"
        Me.Button5.Text = "振込発信データ作成"
        Me.Button5.UseVisualStyleBackColor = False
        '
        'Button3
        '
        Me.Button3.BackColor = System.Drawing.Color.Transparent
        Me.Button3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button3.Location = New System.Drawing.Point(53, 68)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(300, 40)
        Me.Button3.TabIndex = 17
        Me.Button3.Tag = "1"
        Me.Button3.Text = "振込発信リエンタ作成"
        Me.Button3.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.Transparent
        Me.Button1.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button1.Location = New System.Drawing.Point(53, 18)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(300, 40)
        Me.Button1.TabIndex = 16
        Me.Button1.Tag = "1"
        Me.Button1.Text = "振込依頼データ落込"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'tbpMast
        '
        Me.tbpMast.Controls.Add(Me.Button17)
        Me.tbpMast.Controls.Add(Me.Button18)
        Me.tbpMast.Controls.Add(Me.Button19)
        Me.tbpMast.Controls.Add(Me.Button20)
        Me.tbpMast.Controls.Add(Me.Button21)
        Me.tbpMast.Controls.Add(Me.Button22)
        Me.tbpMast.Controls.Add(Me.Button23)
        Me.tbpMast.Controls.Add(Me.Button24)
        Me.tbpMast.Controls.Add(Me.Button25)
        Me.tbpMast.Controls.Add(Me.Button26)
        Me.tbpMast.Controls.Add(Me.Button27)
        Me.tbpMast.Controls.Add(Me.Button28)
        Me.tbpMast.Controls.Add(Me.Button29)
        Me.tbpMast.Controls.Add(Me.Button30)
        Me.tbpMast.Controls.Add(Me.Button31)
        Me.tbpMast.Controls.Add(Me.Button32)
        Me.tbpMast.Location = New System.Drawing.Point(4, 24)
        Me.tbpMast.Name = "tbpMast"
        Me.tbpMast.Size = New System.Drawing.Size(747, 427)
        Me.tbpMast.TabIndex = 1
        Me.tbpMast.Text = "マスタ管理"
        Me.tbpMast.UseVisualStyleBackColor = True
        '
        'Button17
        '
        Me.Button17.BackColor = System.Drawing.Color.Transparent
        Me.Button17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button17.Location = New System.Drawing.Point(393, 118)
        Me.Button17.Name = "Button17"
        Me.Button17.Size = New System.Drawing.Size(300, 40)
        Me.Button17.TabIndex = 26
        Me.Button17.UseVisualStyleBackColor = False
        '
        'Button18
        '
        Me.Button18.BackColor = System.Drawing.Color.Transparent
        Me.Button18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button18.Location = New System.Drawing.Point(393, 68)
        Me.Button18.Name = "Button18"
        Me.Button18.Size = New System.Drawing.Size(300, 40)
        Me.Button18.TabIndex = 25
        Me.Button18.Tag = ""
        Me.Button18.Text = "スケジュール変更"
        Me.Button18.UseVisualStyleBackColor = False
        '
        'Button19
        '
        Me.Button19.BackColor = System.Drawing.Color.Transparent
        Me.Button19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button19.Location = New System.Drawing.Point(393, 18)
        Me.Button19.Name = "Button19"
        Me.Button19.Size = New System.Drawing.Size(300, 40)
        Me.Button19.TabIndex = 24
        Me.Button19.Tag = "1"
        Me.Button19.Text = "スケジュール管理"
        Me.Button19.UseVisualStyleBackColor = False
        '
        'Button20
        '
        Me.Button20.BackColor = System.Drawing.Color.Transparent
        Me.Button20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button20.Location = New System.Drawing.Point(393, 368)
        Me.Button20.Name = "Button20"
        Me.Button20.Size = New System.Drawing.Size(300, 40)
        Me.Button20.TabIndex = 31
        Me.Button20.UseVisualStyleBackColor = False
        '
        'Button21
        '
        Me.Button21.BackColor = System.Drawing.Color.Transparent
        Me.Button21.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button21.Location = New System.Drawing.Point(53, 368)
        Me.Button21.Name = "Button21"
        Me.Button21.Size = New System.Drawing.Size(300, 40)
        Me.Button21.TabIndex = 23
        Me.Button21.UseVisualStyleBackColor = False
        '
        'Button22
        '
        Me.Button22.BackColor = System.Drawing.Color.Transparent
        Me.Button22.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button22.Location = New System.Drawing.Point(393, 318)
        Me.Button22.Name = "Button22"
        Me.Button22.Size = New System.Drawing.Size(300, 40)
        Me.Button22.TabIndex = 30
        Me.Button22.UseVisualStyleBackColor = False
        '
        'Button23
        '
        Me.Button23.BackColor = System.Drawing.Color.Transparent
        Me.Button23.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button23.Location = New System.Drawing.Point(53, 318)
        Me.Button23.Name = "Button23"
        Me.Button23.Size = New System.Drawing.Size(300, 40)
        Me.Button23.TabIndex = 22
        Me.Button23.UseVisualStyleBackColor = False
        '
        'Button24
        '
        Me.Button24.BackColor = System.Drawing.Color.Transparent
        Me.Button24.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button24.Location = New System.Drawing.Point(393, 268)
        Me.Button24.Name = "Button24"
        Me.Button24.Size = New System.Drawing.Size(300, 40)
        Me.Button24.TabIndex = 29
        Me.Button24.UseVisualStyleBackColor = False
        '
        'Button25
        '
        Me.Button25.BackColor = System.Drawing.Color.Transparent
        Me.Button25.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button25.Location = New System.Drawing.Point(53, 268)
        Me.Button25.Name = "Button25"
        Me.Button25.Size = New System.Drawing.Size(300, 40)
        Me.Button25.TabIndex = 21
        Me.Button25.UseVisualStyleBackColor = False
        '
        'Button26
        '
        Me.Button26.BackColor = System.Drawing.Color.Transparent
        Me.Button26.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button26.Location = New System.Drawing.Point(393, 218)
        Me.Button26.Name = "Button26"
        Me.Button26.Size = New System.Drawing.Size(300, 40)
        Me.Button26.TabIndex = 28
        Me.Button26.UseVisualStyleBackColor = False
        '
        'Button27
        '
        Me.Button27.BackColor = System.Drawing.Color.Transparent
        Me.Button27.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button27.Location = New System.Drawing.Point(53, 218)
        Me.Button27.Name = "Button27"
        Me.Button27.Size = New System.Drawing.Size(300, 40)
        Me.Button27.TabIndex = 20
        Me.Button27.Tag = ""
        Me.Button27.UseVisualStyleBackColor = False
        '
        'Button28
        '
        Me.Button28.BackColor = System.Drawing.Color.Transparent
        Me.Button28.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button28.Location = New System.Drawing.Point(393, 168)
        Me.Button28.Name = "Button28"
        Me.Button28.Size = New System.Drawing.Size(300, 40)
        Me.Button28.TabIndex = 27
        Me.Button28.UseVisualStyleBackColor = False
        '
        'Button29
        '
        Me.Button29.BackColor = System.Drawing.Color.Transparent
        Me.Button29.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button29.Location = New System.Drawing.Point(53, 168)
        Me.Button29.Name = "Button29"
        Me.Button29.Size = New System.Drawing.Size(300, 40)
        Me.Button29.TabIndex = 19
        Me.Button29.Tag = "1"
        Me.Button29.Text = "振込手数料マスタメンテナンス"
        Me.Button29.UseVisualStyleBackColor = False
        '
        'Button30
        '
        Me.Button30.BackColor = System.Drawing.Color.Transparent
        Me.Button30.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button30.Location = New System.Drawing.Point(53, 118)
        Me.Button30.Name = "Button30"
        Me.Button30.Size = New System.Drawing.Size(300, 40)
        Me.Button30.TabIndex = 18
        Me.Button30.Tag = "1"
        Me.Button30.Text = "スケジュールメンテナンス"
        Me.Button30.UseVisualStyleBackColor = False
        '
        'Button31
        '
        Me.Button31.BackColor = System.Drawing.Color.Transparent
        Me.Button31.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button31.Location = New System.Drawing.Point(53, 68)
        Me.Button31.Name = "Button31"
        Me.Button31.Size = New System.Drawing.Size(300, 40)
        Me.Button31.TabIndex = 17
        Me.Button31.Tag = "1"
        Me.Button31.Text = "月間スケジュール作成"
        Me.Button31.UseVisualStyleBackColor = False
        '
        'Button32
        '
        Me.Button32.BackColor = System.Drawing.Color.Transparent
        Me.Button32.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button32.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Button32.Location = New System.Drawing.Point(53, 18)
        Me.Button32.Name = "Button32"
        Me.Button32.Size = New System.Drawing.Size(300, 40)
        Me.Button32.TabIndex = 16
        Me.Button32.Tag = "1"
        Me.Button32.Text = "取引先マスタメンテナンス"
        Me.Button32.UseVisualStyleBackColor = False
        '
        'tbpEntry
        '
        Me.tbpEntry.Controls.Add(Me.Button34)
        Me.tbpEntry.Controls.Add(Me.Button35)
        Me.tbpEntry.Controls.Add(Me.Button36)
        Me.tbpEntry.Controls.Add(Me.Button37)
        Me.tbpEntry.Controls.Add(Me.Button38)
        Me.tbpEntry.Controls.Add(Me.Button39)
        Me.tbpEntry.Controls.Add(Me.Button40)
        Me.tbpEntry.Controls.Add(Me.Button41)
        Me.tbpEntry.Controls.Add(Me.Button42)
        Me.tbpEntry.Controls.Add(Me.Button43)
        Me.tbpEntry.Controls.Add(Me.Button44)
        Me.tbpEntry.Controls.Add(Me.Button45)
        Me.tbpEntry.Controls.Add(Me.Button46)
        Me.tbpEntry.Controls.Add(Me.Button47)
        Me.tbpEntry.Controls.Add(Me.Button48)
        Me.tbpEntry.Controls.Add(Me.Button49)
        Me.tbpEntry.Location = New System.Drawing.Point(4, 24)
        Me.tbpEntry.Name = "tbpEntry"
        Me.tbpEntry.Size = New System.Drawing.Size(747, 427)
        Me.tbpEntry.TabIndex = 2
        Me.tbpEntry.Text = "エントリ"
        Me.tbpEntry.UseVisualStyleBackColor = True
        '
        'Button34
        '
        Me.Button34.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button34.Location = New System.Drawing.Point(393, 118)
        Me.Button34.Name = "Button34"
        Me.Button34.Size = New System.Drawing.Size(300, 40)
        Me.Button34.TabIndex = 26
        Me.Button34.UseVisualStyleBackColor = True
        '
        'Button35
        '
        Me.Button35.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button35.Location = New System.Drawing.Point(393, 68)
        Me.Button35.Name = "Button35"
        Me.Button35.Size = New System.Drawing.Size(300, 40)
        Me.Button35.TabIndex = 25
        Me.Button35.Tag = "1"
        Me.Button35.Text = "契約者一覧表印刷"
        Me.Button35.UseVisualStyleBackColor = True
        '
        'Button36
        '
        Me.Button36.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button36.Location = New System.Drawing.Point(393, 18)
        Me.Button36.Name = "Button36"
        Me.Button36.Size = New System.Drawing.Size(300, 40)
        Me.Button36.TabIndex = 24
        Me.Button36.Tag = "1"
        Me.Button36.Text = "総合振込入力チェックリスト印刷"
        Me.Button36.UseVisualStyleBackColor = True
        '
        'Button37
        '
        Me.Button37.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button37.Location = New System.Drawing.Point(393, 368)
        Me.Button37.Name = "Button37"
        Me.Button37.Size = New System.Drawing.Size(300, 40)
        Me.Button37.TabIndex = 31
        Me.Button37.UseVisualStyleBackColor = True
        '
        'Button38
        '
        Me.Button38.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button38.Location = New System.Drawing.Point(53, 368)
        Me.Button38.Name = "Button38"
        Me.Button38.Size = New System.Drawing.Size(300, 40)
        Me.Button38.TabIndex = 23
        Me.Button38.UseVisualStyleBackColor = True
        '
        'Button39
        '
        Me.Button39.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button39.Location = New System.Drawing.Point(393, 318)
        Me.Button39.Name = "Button39"
        Me.Button39.Size = New System.Drawing.Size(300, 40)
        Me.Button39.TabIndex = 30
        Me.Button39.UseVisualStyleBackColor = True
        '
        'Button40
        '
        Me.Button40.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button40.Location = New System.Drawing.Point(53, 318)
        Me.Button40.Name = "Button40"
        Me.Button40.Size = New System.Drawing.Size(300, 40)
        Me.Button40.TabIndex = 22
        Me.Button40.UseVisualStyleBackColor = True
        '
        'Button41
        '
        Me.Button41.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button41.Location = New System.Drawing.Point(393, 268)
        Me.Button41.Name = "Button41"
        Me.Button41.Size = New System.Drawing.Size(300, 40)
        Me.Button41.TabIndex = 29
        Me.Button41.UseVisualStyleBackColor = True
        '
        'Button42
        '
        Me.Button42.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button42.Location = New System.Drawing.Point(53, 268)
        Me.Button42.Name = "Button42"
        Me.Button42.Size = New System.Drawing.Size(300, 40)
        Me.Button42.TabIndex = 21
        Me.Button42.UseVisualStyleBackColor = True
        '
        'Button43
        '
        Me.Button43.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button43.Location = New System.Drawing.Point(393, 218)
        Me.Button43.Name = "Button43"
        Me.Button43.Size = New System.Drawing.Size(300, 40)
        Me.Button43.TabIndex = 28
        Me.Button43.Tag = "1"
        Me.Button43.Text = "振込口座一括削除"
        Me.Button43.UseVisualStyleBackColor = True
        '
        'Button44
        '
        Me.Button44.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button44.Location = New System.Drawing.Point(53, 218)
        Me.Button44.Name = "Button44"
        Me.Button44.Size = New System.Drawing.Size(300, 40)
        Me.Button44.TabIndex = 20
        Me.Button44.UseVisualStyleBackColor = True
        '
        'Button45
        '
        Me.Button45.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button45.Location = New System.Drawing.Point(393, 168)
        Me.Button45.Name = "Button45"
        Me.Button45.Size = New System.Drawing.Size(300, 40)
        Me.Button45.TabIndex = 27
        Me.Button45.UseVisualStyleBackColor = True
        '
        'Button46
        '
        Me.Button46.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button46.Location = New System.Drawing.Point(53, 168)
        Me.Button46.Name = "Button46"
        Me.Button46.Size = New System.Drawing.Size(300, 40)
        Me.Button46.TabIndex = 19
        Me.Button46.Tag = "1"
        Me.Button46.Text = "伝票用データ入力"
        Me.Button46.UseVisualStyleBackColor = True
        '
        'Button47
        '
        Me.Button47.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button47.Location = New System.Drawing.Point(53, 118)
        Me.Button47.Name = "Button47"
        Me.Button47.Size = New System.Drawing.Size(300, 40)
        Me.Button47.TabIndex = 18
        Me.Button47.Tag = "1"
        Me.Button47.Text = "依頼書用データ入力"
        Me.Button47.UseVisualStyleBackColor = True
        '
        'Button48
        '
        Me.Button48.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button48.Location = New System.Drawing.Point(53, 68)
        Me.Button48.Name = "Button48"
        Me.Button48.Size = New System.Drawing.Size(300, 40)
        Me.Button48.TabIndex = 17
        Me.Button48.Tag = "1"
        Me.Button48.Text = "総合振込依頼書印刷"
        Me.Button48.UseVisualStyleBackColor = True
        '
        'Button49
        '
        Me.Button49.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button49.Location = New System.Drawing.Point(53, 18)
        Me.Button49.Name = "Button49"
        Me.Button49.Size = New System.Drawing.Size(300, 40)
        Me.Button49.TabIndex = 16
        Me.Button49.Tag = "1"
        Me.Button49.Text = "依頼書用振込口座登録"
        Me.Button49.UseVisualStyleBackColor = True
        '
        'tbpOther
        '
        Me.tbpOther.Controls.Add(Me.Button50)
        Me.tbpOther.Controls.Add(Me.Button51)
        Me.tbpOther.Controls.Add(Me.Button52)
        Me.tbpOther.Controls.Add(Me.Button53)
        Me.tbpOther.Controls.Add(Me.Button54)
        Me.tbpOther.Controls.Add(Me.Button55)
        Me.tbpOther.Controls.Add(Me.Button56)
        Me.tbpOther.Controls.Add(Me.Button57)
        Me.tbpOther.Controls.Add(Me.Button58)
        Me.tbpOther.Controls.Add(Me.Button59)
        Me.tbpOther.Controls.Add(Me.Button60)
        Me.tbpOther.Controls.Add(Me.Button61)
        Me.tbpOther.Controls.Add(Me.Button62)
        Me.tbpOther.Controls.Add(Me.Button63)
        Me.tbpOther.Controls.Add(Me.Button64)
        Me.tbpOther.Controls.Add(Me.Button65)
        Me.tbpOther.Location = New System.Drawing.Point(4, 24)
        Me.tbpOther.Name = "tbpOther"
        Me.tbpOther.Size = New System.Drawing.Size(747, 427)
        Me.tbpOther.TabIndex = 7
        Me.tbpOther.Text = "その他"
        Me.tbpOther.UseVisualStyleBackColor = True
        '
        'Button50
        '
        Me.Button50.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button50.Location = New System.Drawing.Point(393, 118)
        Me.Button50.Name = "Button50"
        Me.Button50.Size = New System.Drawing.Size(300, 40)
        Me.Button50.TabIndex = 26
        Me.Button50.UseVisualStyleBackColor = True
        '
        'Button51
        '
        Me.Button51.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button51.Location = New System.Drawing.Point(393, 68)
        Me.Button51.Name = "Button51"
        Me.Button51.Size = New System.Drawing.Size(300, 40)
        Me.Button51.TabIndex = 25
        Me.Button51.UseVisualStyleBackColor = True
        '
        'Button52
        '
        Me.Button52.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button52.Location = New System.Drawing.Point(393, 18)
        Me.Button52.Name = "Button52"
        Me.Button52.Size = New System.Drawing.Size(300, 40)
        Me.Button52.TabIndex = 24
        Me.Button52.UseVisualStyleBackColor = True
        '
        'Button53
        '
        Me.Button53.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button53.Location = New System.Drawing.Point(393, 368)
        Me.Button53.Name = "Button53"
        Me.Button53.Size = New System.Drawing.Size(300, 40)
        Me.Button53.TabIndex = 31
        Me.Button53.UseVisualStyleBackColor = True
        '
        'Button54
        '
        Me.Button54.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button54.Location = New System.Drawing.Point(53, 368)
        Me.Button54.Name = "Button54"
        Me.Button54.Size = New System.Drawing.Size(300, 40)
        Me.Button54.TabIndex = 23
        Me.Button54.UseVisualStyleBackColor = True
        '
        'Button55
        '
        Me.Button55.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button55.Location = New System.Drawing.Point(393, 318)
        Me.Button55.Name = "Button55"
        Me.Button55.Size = New System.Drawing.Size(300, 40)
        Me.Button55.TabIndex = 30
        Me.Button55.UseVisualStyleBackColor = True
        '
        'Button56
        '
        Me.Button56.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button56.Location = New System.Drawing.Point(53, 318)
        Me.Button56.Name = "Button56"
        Me.Button56.Size = New System.Drawing.Size(300, 40)
        Me.Button56.TabIndex = 22
        Me.Button56.UseVisualStyleBackColor = True
        '
        'Button57
        '
        Me.Button57.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button57.Location = New System.Drawing.Point(393, 268)
        Me.Button57.Name = "Button57"
        Me.Button57.Size = New System.Drawing.Size(300, 40)
        Me.Button57.TabIndex = 29
        Me.Button57.UseVisualStyleBackColor = True
        '
        'Button58
        '
        Me.Button58.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button58.Location = New System.Drawing.Point(53, 268)
        Me.Button58.Name = "Button58"
        Me.Button58.Size = New System.Drawing.Size(300, 40)
        Me.Button58.TabIndex = 21
        Me.Button58.UseVisualStyleBackColor = True
        '
        'Button59
        '
        Me.Button59.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button59.Location = New System.Drawing.Point(393, 218)
        Me.Button59.Name = "Button59"
        Me.Button59.Size = New System.Drawing.Size(300, 40)
        Me.Button59.TabIndex = 28
        Me.Button59.UseVisualStyleBackColor = True
        '
        'Button60
        '
        Me.Button60.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button60.Location = New System.Drawing.Point(53, 218)
        Me.Button60.Name = "Button60"
        Me.Button60.Size = New System.Drawing.Size(300, 40)
        Me.Button60.TabIndex = 20
        Me.Button60.UseVisualStyleBackColor = True
        '
        'Button61
        '
        Me.Button61.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button61.Location = New System.Drawing.Point(393, 168)
        Me.Button61.Name = "Button61"
        Me.Button61.Size = New System.Drawing.Size(300, 40)
        Me.Button61.TabIndex = 27
        Me.Button61.UseVisualStyleBackColor = True
        '
        'Button62
        '
        Me.Button62.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button62.Location = New System.Drawing.Point(53, 168)
        Me.Button62.Name = "Button62"
        Me.Button62.Size = New System.Drawing.Size(300, 40)
        Me.Button62.TabIndex = 19
        Me.Button62.UseVisualStyleBackColor = True
        '
        'Button63
        '
        Me.Button63.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button63.Location = New System.Drawing.Point(53, 118)
        Me.Button63.Name = "Button63"
        Me.Button63.Size = New System.Drawing.Size(300, 40)
        Me.Button63.TabIndex = 18
        Me.Button63.Tag = "1"
        Me.Button63.Text = "振込発信データ作成取消"
        Me.Button63.UseVisualStyleBackColor = True
        '
        'Button64
        '
        Me.Button64.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button64.Location = New System.Drawing.Point(53, 68)
        Me.Button64.Name = "Button64"
        Me.Button64.Size = New System.Drawing.Size(300, 40)
        Me.Button64.TabIndex = 17
        Me.Button64.Tag = "1"
        Me.Button64.Text = "為替明細発信停止"
        Me.Button64.UseVisualStyleBackColor = True
        '
        'Button65
        '
        Me.Button65.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button65.Location = New System.Drawing.Point(53, 18)
        Me.Button65.Name = "Button65"
        Me.Button65.Size = New System.Drawing.Size(300, 40)
        Me.Button65.TabIndex = 16
        Me.Button65.Tag = "1"
        Me.Button65.Text = "振込発信リエンタ作成取消"
        Me.Button65.UseVisualStyleBackColor = True
        '
        'tbpPrint
        '
        Me.tbpPrint.Controls.Add(Me.Button66)
        Me.tbpPrint.Controls.Add(Me.Button67)
        Me.tbpPrint.Controls.Add(Me.Button68)
        Me.tbpPrint.Controls.Add(Me.Button69)
        Me.tbpPrint.Controls.Add(Me.Button70)
        Me.tbpPrint.Controls.Add(Me.Button71)
        Me.tbpPrint.Controls.Add(Me.Button72)
        Me.tbpPrint.Controls.Add(Me.Button73)
        Me.tbpPrint.Controls.Add(Me.Button74)
        Me.tbpPrint.Controls.Add(Me.Button75)
        Me.tbpPrint.Controls.Add(Me.Button76)
        Me.tbpPrint.Controls.Add(Me.Button77)
        Me.tbpPrint.Controls.Add(Me.Button78)
        Me.tbpPrint.Controls.Add(Me.Button79)
        Me.tbpPrint.Controls.Add(Me.Button80)
        Me.tbpPrint.Controls.Add(Me.Button81)
        Me.tbpPrint.Location = New System.Drawing.Point(4, 24)
        Me.tbpPrint.Name = "tbpPrint"
        Me.tbpPrint.Size = New System.Drawing.Size(747, 427)
        Me.tbpPrint.TabIndex = 6
        Me.tbpPrint.Text = "帳票印刷"
        Me.tbpPrint.UseVisualStyleBackColor = True
        '
        'Button66
        '
        Me.Button66.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button66.Location = New System.Drawing.Point(393, 118)
        Me.Button66.Name = "Button66"
        Me.Button66.Size = New System.Drawing.Size(300, 40)
        Me.Button66.TabIndex = 26
        Me.Button66.UseVisualStyleBackColor = True
        '
        'Button67
        '
        Me.Button67.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button67.Location = New System.Drawing.Point(393, 68)
        Me.Button67.Name = "Button67"
        Me.Button67.Size = New System.Drawing.Size(300, 40)
        Me.Button67.TabIndex = 25
        Me.Button67.UseVisualStyleBackColor = True
        '
        'Button68
        '
        Me.Button68.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button68.Location = New System.Drawing.Point(393, 18)
        Me.Button68.Name = "Button68"
        Me.Button68.Size = New System.Drawing.Size(300, 40)
        Me.Button68.TabIndex = 24
        Me.Button68.UseVisualStyleBackColor = True
        '
        'Button69
        '
        Me.Button69.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button69.Location = New System.Drawing.Point(393, 368)
        Me.Button69.Name = "Button69"
        Me.Button69.Size = New System.Drawing.Size(300, 40)
        Me.Button69.TabIndex = 31
        Me.Button69.UseVisualStyleBackColor = True
        '
        'Button70
        '
        Me.Button70.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button70.Location = New System.Drawing.Point(53, 368)
        Me.Button70.Name = "Button70"
        Me.Button70.Size = New System.Drawing.Size(300, 40)
        Me.Button70.TabIndex = 23
        Me.Button70.Tag = ""
        Me.Button70.UseVisualStyleBackColor = True
        '
        'Button71
        '
        Me.Button71.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button71.Location = New System.Drawing.Point(393, 318)
        Me.Button71.Name = "Button71"
        Me.Button71.Size = New System.Drawing.Size(300, 40)
        Me.Button71.TabIndex = 30
        Me.Button71.UseVisualStyleBackColor = True
        '
        'Button72
        '
        Me.Button72.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button72.Location = New System.Drawing.Point(53, 18)
        Me.Button72.Name = "Button72"
        Me.Button72.Size = New System.Drawing.Size(300, 40)
        Me.Button72.TabIndex = 1
        Me.Button72.Tag = "1"
        Me.Button72.Text = "取引先マスタチェックリスト印刷"
        Me.Button72.UseVisualStyleBackColor = True
        '
        'Button73
        '
        Me.Button73.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button73.Location = New System.Drawing.Point(393, 268)
        Me.Button73.Name = "Button73"
        Me.Button73.Size = New System.Drawing.Size(300, 40)
        Me.Button73.TabIndex = 29
        Me.Button73.UseVisualStyleBackColor = True
        '
        'Button74
        '
        Me.Button74.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button74.Location = New System.Drawing.Point(53, 268)
        Me.Button74.Name = "Button74"
        Me.Button74.Size = New System.Drawing.Size(300, 40)
        Me.Button74.TabIndex = 6
        Me.Button74.Tag = "1"
        Me.Button74.Text = "為替振込明細表再印刷"
        Me.Button74.UseVisualStyleBackColor = True
        '
        'Button75
        '
        Me.Button75.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button75.Location = New System.Drawing.Point(393, 218)
        Me.Button75.Name = "Button75"
        Me.Button75.Size = New System.Drawing.Size(300, 40)
        Me.Button75.TabIndex = 28
        Me.Button75.UseVisualStyleBackColor = True
        '
        'Button76
        '
        Me.Button76.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button76.Location = New System.Drawing.Point(53, 218)
        Me.Button76.Name = "Button76"
        Me.Button76.Size = New System.Drawing.Size(300, 40)
        Me.Button76.TabIndex = 5
        Me.Button76.Tag = "1"
        Me.Button76.Text = "処理結果確認表再印刷"
        Me.Button76.UseVisualStyleBackColor = True
        '
        'Button77
        '
        Me.Button77.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button77.Location = New System.Drawing.Point(393, 168)
        Me.Button77.Name = "Button77"
        Me.Button77.Size = New System.Drawing.Size(300, 40)
        Me.Button77.TabIndex = 27
        Me.Button77.UseVisualStyleBackColor = True
        '
        'Button78
        '
        Me.Button78.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button78.Location = New System.Drawing.Point(53, 168)
        Me.Button78.Name = "Button78"
        Me.Button78.Size = New System.Drawing.Size(300, 40)
        Me.Button78.TabIndex = 4
        Me.Button78.Tag = "1"
        Me.Button78.Text = "取引先マスタ索引簿印刷"
        Me.Button78.UseVisualStyleBackColor = True
        '
        'Button79
        '
        Me.Button79.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button79.Location = New System.Drawing.Point(53, 118)
        Me.Button79.Name = "Button79"
        Me.Button79.Size = New System.Drawing.Size(300, 40)
        Me.Button79.TabIndex = 3
        Me.Button79.Tag = "1"
        Me.Button79.Text = "スケジュール表印刷"
        Me.Button79.UseVisualStyleBackColor = True
        '
        'Button80
        '
        Me.Button80.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button80.Location = New System.Drawing.Point(53, 318)
        Me.Button80.Name = "Button80"
        Me.Button80.Size = New System.Drawing.Size(300, 40)
        Me.Button80.TabIndex = 7
        Me.Button80.Tag = "1"
        Me.Button80.Text = "取引先マスタ項目確認一覧票印刷"
        Me.Button80.UseVisualStyleBackColor = True
        '
        'Button81
        '
        Me.Button81.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button81.Location = New System.Drawing.Point(53, 68)
        Me.Button81.Name = "Button81"
        Me.Button81.Size = New System.Drawing.Size(300, 40)
        Me.Button81.TabIndex = 2
        Me.Button81.Tag = "1"
        Me.Button81.Text = "総合振込明細表印刷"
        Me.Button81.UseVisualStyleBackColor = True
        '
        'CmdBack
        '
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.CmdBack.Location = New System.Drawing.Point(615, 520)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(160, 40)
        Me.CmdBack.TabIndex = 2
        Me.CmdBack.Text = "メインメニュー"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 4
        Me.lblTitle.Text = "RELATIONSTAGE企業自振V2/総合振込"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(691, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 34
        Me.lblDate.Text = "9999年99月99日"
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(691, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 35
        Me.lblUser.Text = "admin"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(600, 27)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 33
        Me.Label2.Text = "システム日付:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(600, 9)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 32
        Me.Label1.Text = "ログイン名　:"
        '
        'KFSMENU010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.TAB)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFSMENU010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFSMENU010"
        Me.TAB.ResumeLayout(False)
        Me.tbpDailyJob.ResumeLayout(False)
        Me.tbpMast.ResumeLayout(False)
        Me.tbpEntry.ResumeLayout(False)
        Me.tbpOther.ResumeLayout(False)
        Me.tbpPrint.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class
