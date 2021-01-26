<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMENU010
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
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        If GCom Is Nothing Then
            GCom = New MenteCommon.clsCommon
        End If
    End Sub
    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMENU010))
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.TAB = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
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
        Me.TabPage2 = New System.Windows.Forms.TabPage()
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
        Me.TabPage3 = New System.Windows.Forms.TabPage()
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
        Me.TabPage4 = New System.Windows.Forms.TabPage()
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
        Me.TabPage5 = New System.Windows.Forms.TabPage()
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
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.Button82 = New System.Windows.Forms.Button()
        Me.Button83 = New System.Windows.Forms.Button()
        Me.Button84 = New System.Windows.Forms.Button()
        Me.Button85 = New System.Windows.Forms.Button()
        Me.Button86 = New System.Windows.Forms.Button()
        Me.Button87 = New System.Windows.Forms.Button()
        Me.Button88 = New System.Windows.Forms.Button()
        Me.Button89 = New System.Windows.Forms.Button()
        Me.Button90 = New System.Windows.Forms.Button()
        Me.Button91 = New System.Windows.Forms.Button()
        Me.Button92 = New System.Windows.Forms.Button()
        Me.Button93 = New System.Windows.Forms.Button()
        Me.Button94 = New System.Windows.Forms.Button()
        Me.Button95 = New System.Windows.Forms.Button()
        Me.Button96 = New System.Windows.Forms.Button()
        Me.Button97 = New System.Windows.Forms.Button()
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.Button98 = New System.Windows.Forms.Button()
        Me.Button99 = New System.Windows.Forms.Button()
        Me.Button100 = New System.Windows.Forms.Button()
        Me.Button101 = New System.Windows.Forms.Button()
        Me.Button102 = New System.Windows.Forms.Button()
        Me.Button103 = New System.Windows.Forms.Button()
        Me.Button104 = New System.Windows.Forms.Button()
        Me.Button105 = New System.Windows.Forms.Button()
        Me.Button106 = New System.Windows.Forms.Button()
        Me.Button107 = New System.Windows.Forms.Button()
        Me.Button108 = New System.Windows.Forms.Button()
        Me.Button109 = New System.Windows.Forms.Button()
        Me.Button110 = New System.Windows.Forms.Button()
        Me.Button111 = New System.Windows.Forms.Button()
        Me.Button112 = New System.Windows.Forms.Button()
        Me.Button113 = New System.Windows.Forms.Button()
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.HIZUKE = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.TAB.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.TabPage7.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(691, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 31
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
        Me.Label2.TabIndex = 29
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
        Me.Label1.TabIndex = 28
        Me.Label1.Text = "ログイン名　:"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 8)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(795, 30)
        Me.lblTitle.TabIndex = 32
        Me.lblTitle.Tag = "0"
        Me.lblTitle.Text = "RELATIONSTAGE企業自振V2/口座振替"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TAB
        '
        Me.TAB.Controls.Add(Me.TabPage1)
        Me.TAB.Controls.Add(Me.TabPage2)
        Me.TAB.Controls.Add(Me.TabPage3)
        Me.TAB.Controls.Add(Me.TabPage4)
        Me.TAB.Controls.Add(Me.TabPage5)
        Me.TAB.Controls.Add(Me.TabPage6)
        Me.TAB.Controls.Add(Me.TabPage7)
        Me.TAB.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TAB.ItemSize = New System.Drawing.Size(100, 20)
        Me.TAB.Location = New System.Drawing.Point(20, 55)
        Me.TAB.Name = "TAB"
        Me.TAB.Padding = New System.Drawing.Point(12, 3)
        Me.TAB.SelectedIndex = 0
        Me.TAB.Size = New System.Drawing.Size(755, 455)
        Me.TAB.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.Transparent
        Me.TabPage1.Controls.Add(Me.Button6)
        Me.TabPage1.Controls.Add(Me.Button4)
        Me.TabPage1.Controls.Add(Me.Button2)
        Me.TabPage1.Controls.Add(Me.Button16)
        Me.TabPage1.Controls.Add(Me.Button15)
        Me.TabPage1.Controls.Add(Me.Button14)
        Me.TabPage1.Controls.Add(Me.Button13)
        Me.TabPage1.Controls.Add(Me.Button12)
        Me.TabPage1.Controls.Add(Me.Button11)
        Me.TabPage1.Controls.Add(Me.Button10)
        Me.TabPage1.Controls.Add(Me.Button9)
        Me.TabPage1.Controls.Add(Me.Button8)
        Me.TabPage1.Controls.Add(Me.Button7)
        Me.TabPage1.Controls.Add(Me.Button5)
        Me.TabPage1.Controls.Add(Me.Button3)
        Me.TabPage1.Controls.Add(Me.Button1)
        Me.TabPage1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage1.Location = New System.Drawing.Point(4, 24)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(747, 427)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "日常業務"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'Button6
        '
        Me.Button6.BackColor = System.Drawing.Color.Transparent
        Me.Button6.Enabled = False
        Me.Button6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button6.Location = New System.Drawing.Point(395, 120)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(300, 40)
        Me.Button6.TabIndex = 10
        Me.Button6.Text = "他行データ媒体書込"
        Me.Button6.UseVisualStyleBackColor = False
        '
        'Button4
        '
        Me.Button4.BackColor = System.Drawing.Color.Transparent
        Me.Button4.Enabled = False
        Me.Button4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button4.Location = New System.Drawing.Point(395, 70)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(300, 40)
        Me.Button4.TabIndex = 9
        Me.Button4.Text = "返還データ作成（一括）"
        Me.Button4.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Transparent
        Me.Button2.Enabled = False
        Me.Button2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button2.Location = New System.Drawing.Point(395, 20)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(300, 40)
        Me.Button2.TabIndex = 8
        Me.Button2.Text = "依頼データ落込（一括）"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Button16
        '
        Me.Button16.BackColor = System.Drawing.Color.Transparent
        Me.Button16.Enabled = False
        Me.Button16.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button16.Location = New System.Drawing.Point(395, 370)
        Me.Button16.Name = "Button16"
        Me.Button16.Size = New System.Drawing.Size(300, 40)
        Me.Button16.TabIndex = 15
        Me.Button16.UseVisualStyleBackColor = False
        '
        'Button15
        '
        Me.Button15.BackColor = System.Drawing.Color.Transparent
        Me.Button15.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button15.Location = New System.Drawing.Point(55, 370)
        Me.Button15.Name = "Button15"
        Me.Button15.Size = New System.Drawing.Size(300, 40)
        Me.Button15.TabIndex = 7
        Me.Button15.Tag = "1"
        Me.Button15.Text = "再振データ作成"
        Me.Button15.UseVisualStyleBackColor = False
        '
        'Button14
        '
        Me.Button14.BackColor = System.Drawing.Color.Transparent
        Me.Button14.Enabled = False
        Me.Button14.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button14.Location = New System.Drawing.Point(395, 320)
        Me.Button14.Name = "Button14"
        Me.Button14.Size = New System.Drawing.Size(300, 40)
        Me.Button14.TabIndex = 14
        Me.Button14.UseVisualStyleBackColor = False
        '
        'Button13
        '
        Me.Button13.BackColor = System.Drawing.Color.Transparent
        Me.Button13.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button13.Location = New System.Drawing.Point(55, 320)
        Me.Button13.Name = "Button13"
        Me.Button13.Size = New System.Drawing.Size(300, 40)
        Me.Button13.TabIndex = 6
        Me.Button13.Tag = "1"
        Me.Button13.Text = "手数料計算"
        Me.Button13.UseVisualStyleBackColor = False
        '
        'Button12
        '
        Me.Button12.BackColor = System.Drawing.Color.Transparent
        Me.Button12.Enabled = False
        Me.Button12.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button12.Location = New System.Drawing.Point(395, 270)
        Me.Button12.Name = "Button12"
        Me.Button12.Size = New System.Drawing.Size(300, 40)
        Me.Button12.TabIndex = 13
        Me.Button12.UseVisualStyleBackColor = False
        '
        'Button11
        '
        Me.Button11.BackColor = System.Drawing.Color.Transparent
        Me.Button11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button11.Location = New System.Drawing.Point(55, 270)
        Me.Button11.Name = "Button11"
        Me.Button11.Size = New System.Drawing.Size(300, 40)
        Me.Button11.TabIndex = 5
        Me.Button11.Tag = "1"
        Me.Button11.Text = "返還データ作成"
        Me.Button11.UseVisualStyleBackColor = False
        '
        'Button10
        '
        Me.Button10.BackColor = System.Drawing.Color.Transparent
        Me.Button10.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button10.Enabled = False
        Me.Button10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button10.Location = New System.Drawing.Point(395, 220)
        Me.Button10.Name = "Button10"
        Me.Button10.Size = New System.Drawing.Size(300, 40)
        Me.Button10.TabIndex = 12
        Me.Button10.UseVisualStyleBackColor = False
        '
        'Button9
        '
        Me.Button9.BackColor = System.Drawing.Color.Transparent
        Me.Button9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button9.Location = New System.Drawing.Point(55, 220)
        Me.Button9.Name = "Button9"
        Me.Button9.Size = New System.Drawing.Size(300, 40)
        Me.Button9.TabIndex = 4
        Me.Button9.Tag = "1"
        Me.Button9.Text = "日報集計"
        Me.Button9.UseVisualStyleBackColor = False
        '
        'Button8
        '
        Me.Button8.BackColor = System.Drawing.Color.Transparent
        Me.Button8.Enabled = False
        Me.Button8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button8.Location = New System.Drawing.Point(395, 170)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(300, 40)
        Me.Button8.TabIndex = 11
        Me.Button8.UseVisualStyleBackColor = False
        '
        'Button7
        '
        Me.Button7.BackColor = System.Drawing.Color.Transparent
        Me.Button7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button7.Location = New System.Drawing.Point(55, 170)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(300, 40)
        Me.Button7.TabIndex = 3
        Me.Button7.Tag = "1"
        Me.Button7.Text = "不能結果更新"
        Me.Button7.UseVisualStyleBackColor = False
        '
        'Button5
        '
        Me.Button5.BackColor = System.Drawing.Color.Transparent
        Me.Button5.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button5.Location = New System.Drawing.Point(55, 120)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(300, 40)
        Me.Button5.TabIndex = 2
        Me.Button5.Tag = "1"
        Me.Button5.Text = "センターカットデータ作成"
        Me.Button5.UseVisualStyleBackColor = False
        '
        'Button3
        '
        Me.Button3.BackColor = System.Drawing.Color.Transparent
        Me.Button3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button3.Location = New System.Drawing.Point(55, 70)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(300, 40)
        Me.Button3.TabIndex = 1
        Me.Button3.Tag = "1"
        Me.Button3.Text = "他行データ作成"
        Me.Button3.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.Transparent
        Me.Button1.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button1.Location = New System.Drawing.Point(55, 20)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(300, 40)
        Me.Button1.TabIndex = 0
        Me.Button1.Tag = "1"
        Me.Button1.Text = "口振依頼データ落込"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.Color.Transparent
        Me.TabPage2.Controls.Add(Me.Button17)
        Me.TabPage2.Controls.Add(Me.Button18)
        Me.TabPage2.Controls.Add(Me.Button19)
        Me.TabPage2.Controls.Add(Me.Button20)
        Me.TabPage2.Controls.Add(Me.Button21)
        Me.TabPage2.Controls.Add(Me.Button22)
        Me.TabPage2.Controls.Add(Me.Button23)
        Me.TabPage2.Controls.Add(Me.Button24)
        Me.TabPage2.Controls.Add(Me.Button25)
        Me.TabPage2.Controls.Add(Me.Button26)
        Me.TabPage2.Controls.Add(Me.Button27)
        Me.TabPage2.Controls.Add(Me.Button28)
        Me.TabPage2.Controls.Add(Me.Button29)
        Me.TabPage2.Controls.Add(Me.Button30)
        Me.TabPage2.Controls.Add(Me.Button31)
        Me.TabPage2.Controls.Add(Me.Button32)
        Me.TabPage2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage2.Location = New System.Drawing.Point(4, 24)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(747, 427)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "マスタ管理"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Button17
        '
        Me.Button17.BackColor = System.Drawing.Color.Transparent
        Me.Button17.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button17.Location = New System.Drawing.Point(395, 120)
        Me.Button17.Name = "Button17"
        Me.Button17.Size = New System.Drawing.Size(300, 40)
        Me.Button17.TabIndex = 10
        Me.Button17.Tag = "1"
        Me.Button17.Text = "手数料徴求フラグ一括更新"
        Me.Button17.UseVisualStyleBackColor = False
        '
        'Button18
        '
        Me.Button18.BackColor = System.Drawing.Color.Transparent
        Me.Button18.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button18.Location = New System.Drawing.Point(395, 70)
        Me.Button18.Name = "Button18"
        Me.Button18.Size = New System.Drawing.Size(300, 40)
        Me.Button18.TabIndex = 9
        Me.Button18.Tag = "1"
        Me.Button18.Text = "スケジュール変更"
        Me.Button18.UseVisualStyleBackColor = False
        '
        'Button19
        '
        Me.Button19.BackColor = System.Drawing.Color.Transparent
        Me.Button19.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button19.Location = New System.Drawing.Point(395, 20)
        Me.Button19.Name = "Button19"
        Me.Button19.Size = New System.Drawing.Size(300, 40)
        Me.Button19.TabIndex = 8
        Me.Button19.Tag = "1"
        Me.Button19.Text = "スケジュール管理"
        Me.Button19.UseVisualStyleBackColor = False
        '
        'Button20
        '
        Me.Button20.BackColor = System.Drawing.Color.Transparent
        Me.Button20.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button20.Location = New System.Drawing.Point(395, 370)
        Me.Button20.Name = "Button20"
        Me.Button20.Size = New System.Drawing.Size(300, 40)
        Me.Button20.TabIndex = 15
        Me.Button20.UseVisualStyleBackColor = False
        '
        'Button21
        '
        Me.Button21.BackColor = System.Drawing.Color.Transparent
        Me.Button21.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button21.Location = New System.Drawing.Point(55, 370)
        Me.Button21.Name = "Button21"
        Me.Button21.Size = New System.Drawing.Size(300, 40)
        Me.Button21.TabIndex = 7
        Me.Button21.UseVisualStyleBackColor = False
        '
        'Button22
        '
        Me.Button22.BackColor = System.Drawing.Color.Transparent
        Me.Button22.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button22.Location = New System.Drawing.Point(395, 320)
        Me.Button22.Name = "Button22"
        Me.Button22.Size = New System.Drawing.Size(300, 40)
        Me.Button22.TabIndex = 14
        Me.Button22.UseVisualStyleBackColor = False
        '
        'Button23
        '
        Me.Button23.BackColor = System.Drawing.Color.Transparent
        Me.Button23.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button23.Location = New System.Drawing.Point(55, 320)
        Me.Button23.Name = "Button23"
        Me.Button23.Size = New System.Drawing.Size(300, 40)
        Me.Button23.TabIndex = 6
        Me.Button23.Tag = ""
        Me.Button23.UseVisualStyleBackColor = False
        '
        'Button24
        '
        Me.Button24.BackColor = System.Drawing.Color.Transparent
        Me.Button24.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button24.Location = New System.Drawing.Point(395, 270)
        Me.Button24.Name = "Button24"
        Me.Button24.Size = New System.Drawing.Size(300, 40)
        Me.Button24.TabIndex = 13
        Me.Button24.UseVisualStyleBackColor = False
        '
        'Button25
        '
        Me.Button25.BackColor = System.Drawing.Color.Transparent
        Me.Button25.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button25.Location = New System.Drawing.Point(55, 270)
        Me.Button25.Name = "Button25"
        Me.Button25.Size = New System.Drawing.Size(300, 40)
        Me.Button25.TabIndex = 5
        Me.Button25.Tag = "1"
        Me.Button25.Text = "振込手数料マスタメンテナンス"
        Me.Button25.UseVisualStyleBackColor = False
        '
        'Button26
        '
        Me.Button26.BackColor = System.Drawing.Color.Transparent
        Me.Button26.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button26.Location = New System.Drawing.Point(395, 220)
        Me.Button26.Name = "Button26"
        Me.Button26.Size = New System.Drawing.Size(300, 40)
        Me.Button26.TabIndex = 12
        Me.Button26.UseVisualStyleBackColor = False
        '
        'Button27
        '
        Me.Button27.BackColor = System.Drawing.Color.Transparent
        Me.Button27.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button27.Location = New System.Drawing.Point(55, 220)
        Me.Button27.Name = "Button27"
        Me.Button27.Size = New System.Drawing.Size(300, 40)
        Me.Button27.TabIndex = 4
        Me.Button27.Tag = "1"
        Me.Button27.Text = "支店読替マスタメンテナンス"
        Me.Button27.UseVisualStyleBackColor = False
        '
        'Button28
        '
        Me.Button28.BackColor = System.Drawing.Color.Transparent
        Me.Button28.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button28.Location = New System.Drawing.Point(395, 170)
        Me.Button28.Name = "Button28"
        Me.Button28.Size = New System.Drawing.Size(300, 40)
        Me.Button28.TabIndex = 11
        Me.Button28.UseVisualStyleBackColor = False
        '
        'Button29
        '
        Me.Button29.BackColor = System.Drawing.Color.Transparent
        Me.Button29.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button29.Location = New System.Drawing.Point(55, 170)
        Me.Button29.Name = "Button29"
        Me.Button29.Size = New System.Drawing.Size(300, 40)
        Me.Button29.TabIndex = 3
        Me.Button29.Tag = "1"
        Me.Button29.Text = "他行マスタメンテナンス"
        Me.Button29.UseVisualStyleBackColor = False
        '
        'Button30
        '
        Me.Button30.BackColor = System.Drawing.Color.Transparent
        Me.Button30.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button30.Location = New System.Drawing.Point(55, 120)
        Me.Button30.Name = "Button30"
        Me.Button30.Size = New System.Drawing.Size(300, 40)
        Me.Button30.TabIndex = 2
        Me.Button30.Tag = "1"
        Me.Button30.Text = "スケジュールメンテナンス"
        Me.Button30.UseVisualStyleBackColor = False
        '
        'Button31
        '
        Me.Button31.BackColor = System.Drawing.Color.Transparent
        Me.Button31.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button31.Location = New System.Drawing.Point(55, 70)
        Me.Button31.Name = "Button31"
        Me.Button31.Size = New System.Drawing.Size(300, 40)
        Me.Button31.TabIndex = 1
        Me.Button31.Tag = "1"
        Me.Button31.Text = "月間スケジュール作成"
        Me.Button31.UseVisualStyleBackColor = False
        '
        'Button32
        '
        Me.Button32.BackColor = System.Drawing.Color.Transparent
        Me.Button32.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button32.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Button32.Location = New System.Drawing.Point(55, 20)
        Me.Button32.Name = "Button32"
        Me.Button32.Size = New System.Drawing.Size(300, 40)
        Me.Button32.TabIndex = 0
        Me.Button32.Tag = "1"
        Me.Button32.Text = "取引先マスタメンテナンス"
        Me.Button32.UseVisualStyleBackColor = False
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.Button34)
        Me.TabPage3.Controls.Add(Me.Button35)
        Me.TabPage3.Controls.Add(Me.Button36)
        Me.TabPage3.Controls.Add(Me.Button37)
        Me.TabPage3.Controls.Add(Me.Button38)
        Me.TabPage3.Controls.Add(Me.Button39)
        Me.TabPage3.Controls.Add(Me.Button40)
        Me.TabPage3.Controls.Add(Me.Button41)
        Me.TabPage3.Controls.Add(Me.Button42)
        Me.TabPage3.Controls.Add(Me.Button43)
        Me.TabPage3.Controls.Add(Me.Button44)
        Me.TabPage3.Controls.Add(Me.Button45)
        Me.TabPage3.Controls.Add(Me.Button46)
        Me.TabPage3.Controls.Add(Me.Button47)
        Me.TabPage3.Controls.Add(Me.Button48)
        Me.TabPage3.Controls.Add(Me.Button49)
        Me.TabPage3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage3.Location = New System.Drawing.Point(4, 24)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(747, 427)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "エントリ"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'Button34
        '
        Me.Button34.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button34.Location = New System.Drawing.Point(395, 120)
        Me.Button34.Name = "Button34"
        Me.Button34.Size = New System.Drawing.Size(300, 40)
        Me.Button34.TabIndex = 10
        Me.Button34.UseVisualStyleBackColor = True
        '
        'Button35
        '
        Me.Button35.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button35.Location = New System.Drawing.Point(395, 70)
        Me.Button35.Name = "Button35"
        Me.Button35.Size = New System.Drawing.Size(300, 40)
        Me.Button35.TabIndex = 9
        Me.Button35.Tag = "1"
        Me.Button35.Text = "契約者一覧表印刷"
        Me.Button35.UseVisualStyleBackColor = True
        '
        'Button36
        '
        Me.Button36.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button36.Location = New System.Drawing.Point(395, 20)
        Me.Button36.Name = "Button36"
        Me.Button36.Size = New System.Drawing.Size(300, 40)
        Me.Button36.TabIndex = 8
        Me.Button36.Tag = "1"
        Me.Button36.Text = "口座振替入力チェックリスト印刷"
        Me.Button36.UseVisualStyleBackColor = True
        '
        'Button37
        '
        Me.Button37.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button37.Location = New System.Drawing.Point(395, 370)
        Me.Button37.Name = "Button37"
        Me.Button37.Size = New System.Drawing.Size(300, 40)
        Me.Button37.TabIndex = 15
        Me.Button37.UseVisualStyleBackColor = True
        '
        'Button38
        '
        Me.Button38.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button38.Location = New System.Drawing.Point(55, 370)
        Me.Button38.Name = "Button38"
        Me.Button38.Size = New System.Drawing.Size(300, 40)
        Me.Button38.TabIndex = 7
        Me.Button38.UseVisualStyleBackColor = True
        '
        'Button39
        '
        Me.Button39.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button39.Location = New System.Drawing.Point(395, 320)
        Me.Button39.Name = "Button39"
        Me.Button39.Size = New System.Drawing.Size(300, 40)
        Me.Button39.TabIndex = 14
        Me.Button39.UseVisualStyleBackColor = True
        '
        'Button40
        '
        Me.Button40.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button40.Location = New System.Drawing.Point(55, 320)
        Me.Button40.Name = "Button40"
        Me.Button40.Size = New System.Drawing.Size(300, 40)
        Me.Button40.TabIndex = 6
        Me.Button40.UseVisualStyleBackColor = True
        '
        'Button41
        '
        Me.Button41.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button41.Location = New System.Drawing.Point(395, 270)
        Me.Button41.Name = "Button41"
        Me.Button41.Size = New System.Drawing.Size(300, 40)
        Me.Button41.TabIndex = 13
        Me.Button41.UseVisualStyleBackColor = True
        '
        'Button42
        '
        Me.Button42.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button42.Location = New System.Drawing.Point(55, 270)
        Me.Button42.Name = "Button42"
        Me.Button42.Size = New System.Drawing.Size(300, 40)
        Me.Button42.TabIndex = 5
        Me.Button42.UseVisualStyleBackColor = True
        '
        'Button43
        '
        Me.Button43.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button43.Location = New System.Drawing.Point(395, 220)
        Me.Button43.Name = "Button43"
        Me.Button43.Size = New System.Drawing.Size(300, 40)
        Me.Button43.TabIndex = 12
        Me.Button43.Tag = "1"
        Me.Button43.Text = "振替口座一括削除"
        Me.Button43.UseVisualStyleBackColor = True
        '
        'Button44
        '
        Me.Button44.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button44.Location = New System.Drawing.Point(55, 220)
        Me.Button44.Name = "Button44"
        Me.Button44.Size = New System.Drawing.Size(300, 40)
        Me.Button44.TabIndex = 4
        Me.Button44.UseVisualStyleBackColor = True
        '
        'Button45
        '
        Me.Button45.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button45.Location = New System.Drawing.Point(395, 170)
        Me.Button45.Name = "Button45"
        Me.Button45.Size = New System.Drawing.Size(300, 40)
        Me.Button45.TabIndex = 11
        Me.Button45.UseVisualStyleBackColor = True
        '
        'Button46
        '
        Me.Button46.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button46.Location = New System.Drawing.Point(55, 170)
        Me.Button46.Name = "Button46"
        Me.Button46.Size = New System.Drawing.Size(300, 40)
        Me.Button46.TabIndex = 3
        Me.Button46.Tag = "1"
        Me.Button46.Text = "伝票用データ入力"
        Me.Button46.UseVisualStyleBackColor = True
        '
        'Button47
        '
        Me.Button47.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button47.Location = New System.Drawing.Point(55, 120)
        Me.Button47.Name = "Button47"
        Me.Button47.Size = New System.Drawing.Size(300, 40)
        Me.Button47.TabIndex = 2
        Me.Button47.Tag = "1"
        Me.Button47.Text = "依頼書用データ入力"
        Me.Button47.UseVisualStyleBackColor = True
        '
        'Button48
        '
        Me.Button48.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button48.Location = New System.Drawing.Point(55, 70)
        Me.Button48.Name = "Button48"
        Me.Button48.Size = New System.Drawing.Size(300, 40)
        Me.Button48.TabIndex = 1
        Me.Button48.Tag = "1"
        Me.Button48.Text = "口座振替依頼書印刷"
        Me.Button48.UseVisualStyleBackColor = True
        '
        'Button49
        '
        Me.Button49.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button49.Location = New System.Drawing.Point(55, 20)
        Me.Button49.Name = "Button49"
        Me.Button49.Size = New System.Drawing.Size(300, 40)
        Me.Button49.TabIndex = 0
        Me.Button49.Tag = "1"
        Me.Button49.Text = "依頼書用振替口座登録"
        Me.Button49.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.Button50)
        Me.TabPage4.Controls.Add(Me.Button51)
        Me.TabPage4.Controls.Add(Me.Button52)
        Me.TabPage4.Controls.Add(Me.Button53)
        Me.TabPage4.Controls.Add(Me.Button54)
        Me.TabPage4.Controls.Add(Me.Button55)
        Me.TabPage4.Controls.Add(Me.Button56)
        Me.TabPage4.Controls.Add(Me.Button57)
        Me.TabPage4.Controls.Add(Me.Button58)
        Me.TabPage4.Controls.Add(Me.Button59)
        Me.TabPage4.Controls.Add(Me.Button60)
        Me.TabPage4.Controls.Add(Me.Button61)
        Me.TabPage4.Controls.Add(Me.Button62)
        Me.TabPage4.Controls.Add(Me.Button63)
        Me.TabPage4.Controls.Add(Me.Button64)
        Me.TabPage4.Controls.Add(Me.Button65)
        Me.TabPage4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage4.Location = New System.Drawing.Point(4, 24)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(747, 427)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "その他"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'Button50
        '
        Me.Button50.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button50.Location = New System.Drawing.Point(395, 120)
        Me.Button50.Name = "Button50"
        Me.Button50.Size = New System.Drawing.Size(300, 40)
        Me.Button50.TabIndex = 10
        Me.Button50.UseVisualStyleBackColor = True
        '
        'Button51
        '
        Me.Button51.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button51.Location = New System.Drawing.Point(395, 70)
        Me.Button51.Name = "Button51"
        Me.Button51.Size = New System.Drawing.Size(300, 40)
        Me.Button51.TabIndex = 9
        Me.Button51.UseVisualStyleBackColor = True
        '
        'Button52
        '
        Me.Button52.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button52.Location = New System.Drawing.Point(395, 20)
        Me.Button52.Name = "Button52"
        Me.Button52.Size = New System.Drawing.Size(300, 40)
        Me.Button52.TabIndex = 8
        Me.Button52.UseVisualStyleBackColor = True
        '
        'Button53
        '
        Me.Button53.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button53.Location = New System.Drawing.Point(395, 370)
        Me.Button53.Name = "Button53"
        Me.Button53.Size = New System.Drawing.Size(300, 40)
        Me.Button53.TabIndex = 15
        Me.Button53.UseVisualStyleBackColor = True
        '
        'Button54
        '
        Me.Button54.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button54.Location = New System.Drawing.Point(55, 370)
        Me.Button54.Name = "Button54"
        Me.Button54.Size = New System.Drawing.Size(300, 40)
        Me.Button54.TabIndex = 7
        Me.Button54.UseVisualStyleBackColor = True
        '
        'Button55
        '
        Me.Button55.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button55.Location = New System.Drawing.Point(395, 320)
        Me.Button55.Name = "Button55"
        Me.Button55.Size = New System.Drawing.Size(300, 40)
        Me.Button55.TabIndex = 14
        Me.Button55.UseVisualStyleBackColor = True
        '
        'Button56
        '
        Me.Button56.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button56.Location = New System.Drawing.Point(55, 320)
        Me.Button56.Name = "Button56"
        Me.Button56.Size = New System.Drawing.Size(300, 40)
        Me.Button56.TabIndex = 6
        Me.Button56.UseVisualStyleBackColor = True
        '
        'Button57
        '
        Me.Button57.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button57.Location = New System.Drawing.Point(395, 270)
        Me.Button57.Name = "Button57"
        Me.Button57.Size = New System.Drawing.Size(300, 40)
        Me.Button57.TabIndex = 13
        Me.Button57.UseVisualStyleBackColor = True
        '
        'Button58
        '
        Me.Button58.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button58.Location = New System.Drawing.Point(55, 270)
        Me.Button58.Name = "Button58"
        Me.Button58.Size = New System.Drawing.Size(300, 40)
        Me.Button58.TabIndex = 5
        Me.Button58.UseVisualStyleBackColor = True
        '
        'Button59
        '
        Me.Button59.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button59.Location = New System.Drawing.Point(395, 220)
        Me.Button59.Name = "Button59"
        Me.Button59.Size = New System.Drawing.Size(300, 40)
        Me.Button59.TabIndex = 12
        Me.Button59.UseVisualStyleBackColor = True
        '
        'Button60
        '
        Me.Button60.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button60.Location = New System.Drawing.Point(55, 220)
        Me.Button60.Name = "Button60"
        Me.Button60.Size = New System.Drawing.Size(300, 40)
        Me.Button60.TabIndex = 4
        Me.Button60.UseVisualStyleBackColor = True
        '
        'Button61
        '
        Me.Button61.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button61.Location = New System.Drawing.Point(395, 170)
        Me.Button61.Name = "Button61"
        Me.Button61.Size = New System.Drawing.Size(300, 40)
        Me.Button61.TabIndex = 11
        Me.Button61.UseVisualStyleBackColor = True
        '
        'Button62
        '
        Me.Button62.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button62.Location = New System.Drawing.Point(55, 170)
        Me.Button62.Name = "Button62"
        Me.Button62.Size = New System.Drawing.Size(300, 40)
        Me.Button62.TabIndex = 3
        Me.Button62.UseVisualStyleBackColor = True
        '
        'Button63
        '
        Me.Button63.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button63.Location = New System.Drawing.Point(55, 120)
        Me.Button63.Name = "Button63"
        Me.Button63.Size = New System.Drawing.Size(300, 40)
        Me.Button63.TabIndex = 2
        Me.Button63.Tag = "1"
        Me.Button63.Text = "センター直接持込落込取消"
        Me.Button63.UseVisualStyleBackColor = True
        '
        'Button64
        '
        Me.Button64.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button64.Location = New System.Drawing.Point(55, 70)
        Me.Button64.Name = "Button64"
        Me.Button64.Size = New System.Drawing.Size(300, 40)
        Me.Button64.TabIndex = 1
        Me.Button64.Tag = "1"
        Me.Button64.Text = "振替結果変更"
        Me.Button64.UseVisualStyleBackColor = True
        '
        'Button65
        '
        Me.Button65.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button65.Location = New System.Drawing.Point(55, 20)
        Me.Button65.Name = "Button65"
        Me.Button65.Size = New System.Drawing.Size(300, 40)
        Me.Button65.TabIndex = 0
        Me.Button65.Tag = "1"
        Me.Button65.Text = "センターカットデータ作成取消"
        Me.Button65.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.Button66)
        Me.TabPage5.Controls.Add(Me.Button67)
        Me.TabPage5.Controls.Add(Me.Button68)
        Me.TabPage5.Controls.Add(Me.Button69)
        Me.TabPage5.Controls.Add(Me.Button70)
        Me.TabPage5.Controls.Add(Me.Button71)
        Me.TabPage5.Controls.Add(Me.Button72)
        Me.TabPage5.Controls.Add(Me.Button73)
        Me.TabPage5.Controls.Add(Me.Button74)
        Me.TabPage5.Controls.Add(Me.Button75)
        Me.TabPage5.Controls.Add(Me.Button76)
        Me.TabPage5.Controls.Add(Me.Button77)
        Me.TabPage5.Controls.Add(Me.Button78)
        Me.TabPage5.Controls.Add(Me.Button79)
        Me.TabPage5.Controls.Add(Me.Button80)
        Me.TabPage5.Controls.Add(Me.Button81)
        Me.TabPage5.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage5.Location = New System.Drawing.Point(4, 24)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(747, 427)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "帳票印刷"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'Button66
        '
        Me.Button66.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button66.Location = New System.Drawing.Point(395, 120)
        Me.Button66.Name = "Button66"
        Me.Button66.Size = New System.Drawing.Size(300, 40)
        Me.Button66.TabIndex = 10
        Me.Button66.Tag = "1"
        Me.Button66.Text = "店別集計表印刷"
        Me.Button66.UseVisualStyleBackColor = True
        '
        'Button67
        '
        Me.Button67.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button67.Location = New System.Drawing.Point(395, 70)
        Me.Button67.Name = "Button67"
        Me.Button67.Size = New System.Drawing.Size(300, 40)
        Me.Button67.TabIndex = 9
        Me.Button67.Tag = "1"
        Me.Button67.Text = "処理結果確認表一覧印刷"
        Me.Button67.UseVisualStyleBackColor = True
        '
        'Button68
        '
        Me.Button68.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button68.Location = New System.Drawing.Point(395, 20)
        Me.Button68.Name = "Button68"
        Me.Button68.Size = New System.Drawing.Size(300, 40)
        Me.Button68.TabIndex = 8
        Me.Button68.Tag = "1"
        Me.Button68.Text = "自振管理リスト印刷"
        Me.Button68.UseVisualStyleBackColor = True
        '
        'Button69
        '
        Me.Button69.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button69.Location = New System.Drawing.Point(395, 370)
        Me.Button69.Name = "Button69"
        Me.Button69.Size = New System.Drawing.Size(300, 40)
        Me.Button69.TabIndex = 15
        Me.Button69.Tag = "1"
        Me.Button69.Text = "取引先マスタ項目確認一覧票印刷"
        Me.Button69.UseVisualStyleBackColor = True
        '
        'Button70
        '
        Me.Button70.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button70.Location = New System.Drawing.Point(55, 370)
        Me.Button70.Name = "Button70"
        Me.Button70.Size = New System.Drawing.Size(300, 40)
        Me.Button70.TabIndex = 7
        Me.Button70.Tag = "1"
        Me.Button70.Text = "振替結果明細表印刷"
        Me.Button70.UseVisualStyleBackColor = True
        '
        'Button71
        '
        Me.Button71.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button71.Location = New System.Drawing.Point(395, 320)
        Me.Button71.Name = "Button71"
        Me.Button71.Size = New System.Drawing.Size(300, 40)
        Me.Button71.TabIndex = 14
        Me.Button71.Tag = "1"
        Me.Button71.Text = "再振対象先チェックリスト印刷"
        Me.Button71.UseVisualStyleBackColor = True
        '
        'Button72
        '
        Me.Button72.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button72.Location = New System.Drawing.Point(55, 320)
        Me.Button72.Name = "Button72"
        Me.Button72.Size = New System.Drawing.Size(300, 40)
        Me.Button72.TabIndex = 6
        Me.Button72.Tag = "1"
        Me.Button72.Text = "未処理一覧表(落込)印刷"
        Me.Button72.UseVisualStyleBackColor = True
        '
        'Button73
        '
        Me.Button73.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button73.Location = New System.Drawing.Point(395, 270)
        Me.Button73.Name = "Button73"
        Me.Button73.Size = New System.Drawing.Size(300, 40)
        Me.Button73.TabIndex = 13
        Me.Button73.Tag = "1"
        Me.Button73.Text = "データ伝送通知書印刷"
        Me.Button73.UseVisualStyleBackColor = True
        '
        'Button74
        '
        Me.Button74.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button74.Location = New System.Drawing.Point(55, 270)
        Me.Button74.Name = "Button74"
        Me.Button74.Size = New System.Drawing.Size(300, 40)
        Me.Button74.TabIndex = 5
        Me.Button74.Tag = "1"
        Me.Button74.Text = "振替結果変更チェックリスト印刷"
        Me.Button74.UseVisualStyleBackColor = True
        '
        'Button75
        '
        Me.Button75.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button75.Location = New System.Drawing.Point(395, 220)
        Me.Button75.Name = "Button75"
        Me.Button75.Size = New System.Drawing.Size(300, 40)
        Me.Button75.TabIndex = 12
        Me.Button75.Tag = "1"
        Me.Button75.Text = "振替不能事由別集計表印刷"
        Me.Button75.UseVisualStyleBackColor = True
        '
        'Button76
        '
        Me.Button76.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button76.Location = New System.Drawing.Point(55, 220)
        Me.Button76.Name = "Button76"
        Me.Button76.Size = New System.Drawing.Size(300, 40)
        Me.Button76.TabIndex = 4
        Me.Button76.Tag = "1"
        Me.Button76.Text = "処理結果確認表再印刷"
        Me.Button76.UseVisualStyleBackColor = True
        '
        'Button77
        '
        Me.Button77.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button77.Location = New System.Drawing.Point(395, 170)
        Me.Button77.Name = "Button77"
        Me.Button77.Size = New System.Drawing.Size(300, 40)
        Me.Button77.TabIndex = 11
        Me.Button77.Tag = "1"
        Me.Button77.Text = "受付明細表印刷"
        Me.Button77.UseVisualStyleBackColor = True
        '
        'Button78
        '
        Me.Button78.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button78.Location = New System.Drawing.Point(55, 170)
        Me.Button78.Name = "Button78"
        Me.Button78.Size = New System.Drawing.Size(300, 40)
        Me.Button78.TabIndex = 3
        Me.Button78.Tag = "1"
        Me.Button78.Text = "取引先マスタ索引簿印刷"
        Me.Button78.UseVisualStyleBackColor = True
        '
        'Button79
        '
        Me.Button79.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button79.Location = New System.Drawing.Point(55, 120)
        Me.Button79.Name = "Button79"
        Me.Button79.Size = New System.Drawing.Size(300, 40)
        Me.Button79.TabIndex = 2
        Me.Button79.Tag = "1"
        Me.Button79.Text = "スケジュール表印刷"
        Me.Button79.UseVisualStyleBackColor = True
        '
        'Button80
        '
        Me.Button80.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button80.Location = New System.Drawing.Point(55, 70)
        Me.Button80.Name = "Button80"
        Me.Button80.Size = New System.Drawing.Size(300, 40)
        Me.Button80.TabIndex = 1
        Me.Button80.Tag = "1"
        Me.Button80.Text = "口座振替明細表印刷"
        Me.Button80.UseVisualStyleBackColor = True
        '
        'Button81
        '
        Me.Button81.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button81.Location = New System.Drawing.Point(55, 20)
        Me.Button81.Name = "Button81"
        Me.Button81.Size = New System.Drawing.Size(300, 40)
        Me.Button81.TabIndex = 0
        Me.Button81.Tag = "1"
        Me.Button81.Text = "取引先マスタチェックリスト印刷"
        Me.Button81.UseVisualStyleBackColor = True
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.Button82)
        Me.TabPage6.Controls.Add(Me.Button83)
        Me.TabPage6.Controls.Add(Me.Button84)
        Me.TabPage6.Controls.Add(Me.Button85)
        Me.TabPage6.Controls.Add(Me.Button86)
        Me.TabPage6.Controls.Add(Me.Button87)
        Me.TabPage6.Controls.Add(Me.Button88)
        Me.TabPage6.Controls.Add(Me.Button89)
        Me.TabPage6.Controls.Add(Me.Button90)
        Me.TabPage6.Controls.Add(Me.Button91)
        Me.TabPage6.Controls.Add(Me.Button92)
        Me.TabPage6.Controls.Add(Me.Button93)
        Me.TabPage6.Controls.Add(Me.Button94)
        Me.TabPage6.Controls.Add(Me.Button95)
        Me.TabPage6.Controls.Add(Me.Button96)
        Me.TabPage6.Controls.Add(Me.Button97)
        Me.TabPage6.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage6.Location = New System.Drawing.Point(4, 24)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(747, 427)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "伝送管理"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'Button82
        '
        Me.Button82.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button82.Location = New System.Drawing.Point(395, 120)
        Me.Button82.Name = "Button82"
        Me.Button82.Size = New System.Drawing.Size(300, 40)
        Me.Button82.TabIndex = 10
        Me.Button82.UseVisualStyleBackColor = True
        '
        'Button83
        '
        Me.Button83.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button83.Location = New System.Drawing.Point(395, 70)
        Me.Button83.Name = "Button83"
        Me.Button83.Size = New System.Drawing.Size(300, 40)
        Me.Button83.TabIndex = 9
        Me.Button83.UseVisualStyleBackColor = True
        '
        'Button84
        '
        Me.Button84.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button84.Location = New System.Drawing.Point(395, 20)
        Me.Button84.Name = "Button84"
        Me.Button84.Size = New System.Drawing.Size(300, 40)
        Me.Button84.TabIndex = 8
        Me.Button84.UseVisualStyleBackColor = True
        '
        'Button85
        '
        Me.Button85.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button85.Location = New System.Drawing.Point(395, 370)
        Me.Button85.Name = "Button85"
        Me.Button85.Size = New System.Drawing.Size(300, 40)
        Me.Button85.TabIndex = 15
        Me.Button85.UseVisualStyleBackColor = True
        '
        'Button86
        '
        Me.Button86.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button86.Location = New System.Drawing.Point(55, 370)
        Me.Button86.Name = "Button86"
        Me.Button86.Size = New System.Drawing.Size(300, 40)
        Me.Button86.TabIndex = 7
        Me.Button86.UseVisualStyleBackColor = True
        '
        'Button87
        '
        Me.Button87.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button87.Location = New System.Drawing.Point(395, 320)
        Me.Button87.Name = "Button87"
        Me.Button87.Size = New System.Drawing.Size(300, 40)
        Me.Button87.TabIndex = 14
        Me.Button87.UseVisualStyleBackColor = True
        '
        'Button88
        '
        Me.Button88.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button88.Location = New System.Drawing.Point(55, 320)
        Me.Button88.Name = "Button88"
        Me.Button88.Size = New System.Drawing.Size(300, 40)
        Me.Button88.TabIndex = 6
        Me.Button88.UseVisualStyleBackColor = True
        '
        'Button89
        '
        Me.Button89.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button89.Location = New System.Drawing.Point(395, 270)
        Me.Button89.Name = "Button89"
        Me.Button89.Size = New System.Drawing.Size(300, 40)
        Me.Button89.TabIndex = 13
        Me.Button89.UseVisualStyleBackColor = True
        '
        'Button90
        '
        Me.Button90.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button90.Location = New System.Drawing.Point(55, 270)
        Me.Button90.Name = "Button90"
        Me.Button90.Size = New System.Drawing.Size(300, 40)
        Me.Button90.TabIndex = 5
        Me.Button90.UseVisualStyleBackColor = True
        '
        'Button91
        '
        Me.Button91.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button91.Location = New System.Drawing.Point(395, 220)
        Me.Button91.Name = "Button91"
        Me.Button91.Size = New System.Drawing.Size(300, 40)
        Me.Button91.TabIndex = 12
        Me.Button91.UseVisualStyleBackColor = True
        '
        'Button92
        '
        Me.Button92.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button92.Location = New System.Drawing.Point(55, 220)
        Me.Button92.Name = "Button92"
        Me.Button92.Size = New System.Drawing.Size(300, 40)
        Me.Button92.TabIndex = 4
        Me.Button92.UseVisualStyleBackColor = True
        '
        'Button93
        '
        Me.Button93.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button93.Location = New System.Drawing.Point(395, 170)
        Me.Button93.Name = "Button93"
        Me.Button93.Size = New System.Drawing.Size(300, 40)
        Me.Button93.TabIndex = 11
        Me.Button93.UseVisualStyleBackColor = True
        '
        'Button94
        '
        Me.Button94.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button94.Location = New System.Drawing.Point(55, 170)
        Me.Button94.Name = "Button94"
        Me.Button94.Size = New System.Drawing.Size(300, 40)
        Me.Button94.TabIndex = 3
        Me.Button94.UseVisualStyleBackColor = True
        '
        'Button95
        '
        Me.Button95.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button95.Location = New System.Drawing.Point(55, 120)
        Me.Button95.Name = "Button95"
        Me.Button95.Size = New System.Drawing.Size(300, 40)
        Me.Button95.TabIndex = 2
        Me.Button95.UseVisualStyleBackColor = True
        '
        'Button96
        '
        Me.Button96.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button96.Location = New System.Drawing.Point(55, 70)
        Me.Button96.Name = "Button96"
        Me.Button96.Size = New System.Drawing.Size(300, 40)
        Me.Button96.TabIndex = 1
        Me.Button96.Tag = "1"
        Me.Button96.Text = "一括送信ファイル作成(元請)"
        Me.Button96.UseVisualStyleBackColor = True
        '
        'Button97
        '
        Me.Button97.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button97.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button97.Location = New System.Drawing.Point(55, 20)
        Me.Button97.Name = "Button97"
        Me.Button97.Size = New System.Drawing.Size(300, 40)
        Me.Button97.TabIndex = 0
        Me.Button97.Tag = "1"
        Me.Button97.Text = "受信ファイル一括振分(元請)"
        Me.Button97.UseVisualStyleBackColor = True
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.Button98)
        Me.TabPage7.Controls.Add(Me.Button99)
        Me.TabPage7.Controls.Add(Me.Button100)
        Me.TabPage7.Controls.Add(Me.Button101)
        Me.TabPage7.Controls.Add(Me.Button102)
        Me.TabPage7.Controls.Add(Me.Button103)
        Me.TabPage7.Controls.Add(Me.Button104)
        Me.TabPage7.Controls.Add(Me.Button105)
        Me.TabPage7.Controls.Add(Me.Button106)
        Me.TabPage7.Controls.Add(Me.Button107)
        Me.TabPage7.Controls.Add(Me.Button108)
        Me.TabPage7.Controls.Add(Me.Button109)
        Me.TabPage7.Controls.Add(Me.Button110)
        Me.TabPage7.Controls.Add(Me.Button111)
        Me.TabPage7.Controls.Add(Me.Button112)
        Me.TabPage7.Controls.Add(Me.Button113)
        Me.TabPage7.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TabPage7.Location = New System.Drawing.Point(4, 24)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Size = New System.Drawing.Size(747, 427)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "口座ｵﾌﾟｼｮﾝ"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'Button98
        '
        Me.Button98.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button98.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button98.Location = New System.Drawing.Point(395, 120)
        Me.Button98.Name = "Button98"
        Me.Button98.Size = New System.Drawing.Size(300, 40)
        Me.Button98.TabIndex = 10
        Me.Button98.Text = "自振契約リエンタ書込"
        Me.Button98.UseVisualStyleBackColor = True
        '
        'Button99
        '
        Me.Button99.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button99.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button99.Location = New System.Drawing.Point(395, 70)
        Me.Button99.Name = "Button99"
        Me.Button99.Size = New System.Drawing.Size(300, 40)
        Me.Button99.TabIndex = 9
        Me.Button99.Tag = "1"
        Me.Button99.Text = "自振契約リエンタ結果更新"
        Me.Button99.UseVisualStyleBackColor = True
        '
        'Button100
        '
        Me.Button100.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button100.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button100.Location = New System.Drawing.Point(395, 20)
        Me.Button100.Name = "Button100"
        Me.Button100.Size = New System.Drawing.Size(300, 40)
        Me.Button100.TabIndex = 8
        Me.Button100.Tag = "1"
        Me.Button100.Text = "自振契約リエンタ作成"
        Me.Button100.UseVisualStyleBackColor = True
        '
        'Button101
        '
        Me.Button101.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button101.Location = New System.Drawing.Point(395, 370)
        Me.Button101.Name = "Button101"
        Me.Button101.Size = New System.Drawing.Size(300, 40)
        Me.Button101.TabIndex = 15
        Me.Button101.UseVisualStyleBackColor = True
        '
        'Button102
        '
        Me.Button102.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button102.Location = New System.Drawing.Point(55, 370)
        Me.Button102.Name = "Button102"
        Me.Button102.Size = New System.Drawing.Size(300, 40)
        Me.Button102.TabIndex = 7
        Me.Button102.UseVisualStyleBackColor = True
        '
        'Button103
        '
        Me.Button103.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button103.Location = New System.Drawing.Point(395, 320)
        Me.Button103.Name = "Button103"
        Me.Button103.Size = New System.Drawing.Size(300, 40)
        Me.Button103.TabIndex = 14
        Me.Button103.UseVisualStyleBackColor = True
        '
        'Button104
        '
        Me.Button104.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button104.Location = New System.Drawing.Point(55, 320)
        Me.Button104.Name = "Button104"
        Me.Button104.Size = New System.Drawing.Size(300, 40)
        Me.Button104.TabIndex = 6
        Me.Button104.UseVisualStyleBackColor = True
        '
        'Button105
        '
        Me.Button105.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button105.Location = New System.Drawing.Point(395, 270)
        Me.Button105.Name = "Button105"
        Me.Button105.Size = New System.Drawing.Size(300, 40)
        Me.Button105.TabIndex = 13
        Me.Button105.UseVisualStyleBackColor = True
        '
        'Button106
        '
        Me.Button106.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button106.Location = New System.Drawing.Point(55, 270)
        Me.Button106.Name = "Button106"
        Me.Button106.Size = New System.Drawing.Size(300, 40)
        Me.Button106.TabIndex = 5
        Me.Button106.UseVisualStyleBackColor = True
        '
        'Button107
        '
        Me.Button107.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button107.Location = New System.Drawing.Point(395, 220)
        Me.Button107.Name = "Button107"
        Me.Button107.Size = New System.Drawing.Size(300, 40)
        Me.Button107.TabIndex = 12
        Me.Button107.UseVisualStyleBackColor = True
        '
        'Button108
        '
        Me.Button108.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button108.Location = New System.Drawing.Point(55, 220)
        Me.Button108.Name = "Button108"
        Me.Button108.Size = New System.Drawing.Size(300, 40)
        Me.Button108.TabIndex = 4
        Me.Button108.UseVisualStyleBackColor = True
        '
        'Button109
        '
        Me.Button109.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button109.Location = New System.Drawing.Point(395, 170)
        Me.Button109.Name = "Button109"
        Me.Button109.Size = New System.Drawing.Size(300, 40)
        Me.Button109.TabIndex = 11
        Me.Button109.UseVisualStyleBackColor = True
        '
        'Button110
        '
        Me.Button110.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button110.Location = New System.Drawing.Point(55, 170)
        Me.Button110.Name = "Button110"
        Me.Button110.Size = New System.Drawing.Size(300, 40)
        Me.Button110.TabIndex = 3
        Me.Button110.UseVisualStyleBackColor = True
        '
        'Button111
        '
        Me.Button111.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button111.Location = New System.Drawing.Point(55, 120)
        Me.Button111.Name = "Button111"
        Me.Button111.Size = New System.Drawing.Size(300, 40)
        Me.Button111.TabIndex = 2
        Me.Button111.UseVisualStyleBackColor = True
        '
        'Button112
        '
        Me.Button112.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button112.Location = New System.Drawing.Point(55, 70)
        Me.Button112.Name = "Button112"
        Me.Button112.Size = New System.Drawing.Size(300, 40)
        Me.Button112.TabIndex = 1
        Me.Button112.Tag = "1"
        Me.Button112.Text = "口座チェック(随時)"
        Me.Button112.UseVisualStyleBackColor = True
        '
        'Button113
        '
        Me.Button113.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button113.Location = New System.Drawing.Point(55, 20)
        Me.Button113.Name = "Button113"
        Me.Button113.Size = New System.Drawing.Size(300, 40)
        Me.Button113.TabIndex = 0
        Me.Button113.Tag = "1"
        Me.Button113.Text = "口座チェック(当日受付分)"
        Me.Button113.UseVisualStyleBackColor = True
        '
        'CmdBack
        '
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdBack.Location = New System.Drawing.Point(615, 520)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(160, 40)
        Me.CmdBack.TabIndex = 2
        Me.CmdBack.Text = "メインメニュー"
        Me.CmdBack.UseVisualStyleBackColor = True
        '
        'HIZUKE
        '
        Me.HIZUKE.AutoSize = True
        Me.HIZUKE.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HIZUKE.Location = New System.Drawing.Point(686, 27)
        Me.HIZUKE.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.HIZUKE.Name = "HIZUKE"
        Me.HIZUKE.Size = New System.Drawing.Size(0, 12)
        Me.HIZUKE.TabIndex = 30
        Me.HIZUKE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(691, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(65, 12)
        Me.lblDate.TabIndex = 31
        Me.lblDate.Text = "9999/99/99"
        '
        'KFJMENU010
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.TAB)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.HIZUKE)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMENU010"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMENU010"
        Me.TAB.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage7.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents TAB As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button15 As System.Windows.Forms.Button
    Friend WithEvents Button13 As System.Windows.Forms.Button
    Friend WithEvents Button11 As System.Windows.Forms.Button
    Friend WithEvents Button9 As System.Windows.Forms.Button
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button16 As System.Windows.Forms.Button
    Friend WithEvents Button14 As System.Windows.Forms.Button
    Friend WithEvents Button12 As System.Windows.Forms.Button
    Friend WithEvents Button10 As System.Windows.Forms.Button
    Friend WithEvents Button8 As System.Windows.Forms.Button
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
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
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
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
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
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
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
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents Button82 As System.Windows.Forms.Button
    Friend WithEvents Button83 As System.Windows.Forms.Button
    Friend WithEvents Button84 As System.Windows.Forms.Button
    Friend WithEvents Button85 As System.Windows.Forms.Button
    Friend WithEvents Button86 As System.Windows.Forms.Button
    Friend WithEvents Button87 As System.Windows.Forms.Button
    Friend WithEvents Button88 As System.Windows.Forms.Button
    Friend WithEvents Button89 As System.Windows.Forms.Button
    Friend WithEvents Button90 As System.Windows.Forms.Button
    Friend WithEvents Button91 As System.Windows.Forms.Button
    Friend WithEvents Button92 As System.Windows.Forms.Button
    Friend WithEvents Button93 As System.Windows.Forms.Button
    Friend WithEvents Button94 As System.Windows.Forms.Button
    Friend WithEvents Button95 As System.Windows.Forms.Button
    Friend WithEvents Button96 As System.Windows.Forms.Button
    Friend WithEvents Button97 As System.Windows.Forms.Button
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents Button98 As System.Windows.Forms.Button
    Friend WithEvents Button99 As System.Windows.Forms.Button
    Friend WithEvents Button100 As System.Windows.Forms.Button
    Friend WithEvents Button101 As System.Windows.Forms.Button
    Friend WithEvents Button102 As System.Windows.Forms.Button
    Friend WithEvents Button103 As System.Windows.Forms.Button
    Friend WithEvents Button104 As System.Windows.Forms.Button
    Friend WithEvents Button105 As System.Windows.Forms.Button
    Friend WithEvents Button106 As System.Windows.Forms.Button
    Friend WithEvents Button107 As System.Windows.Forms.Button
    Friend WithEvents Button108 As System.Windows.Forms.Button
    Friend WithEvents Button109 As System.Windows.Forms.Button
    Friend WithEvents Button110 As System.Windows.Forms.Button
    Friend WithEvents Button111 As System.Windows.Forms.Button
    Friend WithEvents Button112 As System.Windows.Forms.Button
    Friend WithEvents Button113 As System.Windows.Forms.Button
    Friend WithEvents HIZUKE As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
End Class
