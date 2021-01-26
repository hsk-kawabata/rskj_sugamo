<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFCMAIN021
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '取引先主コード
        AddHandler txtTorisCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtTorisCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorisCode.LostFocus, AddressOf CAST.LostFocus

        '取引先副コード
        AddHandler txtTorifCode.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtTorifCode.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtTorifCode.LostFocus, AddressOf CAST.LostFocus

        '振替日(年)
        AddHandler txtFuriDateY.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateY.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateY.LostFocus, AddressOf CAST.LostFocus

        '振替日(月)
        AddHandler txtFuriDateM.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateM.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateM.LostFocus, AddressOf CAST.LostFocus

        '振替日(日)
        AddHandler txtFuriDateD.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtFuriDateD.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler txtFuriDateD.LostFocus, AddressOf CAST.LostFocus

        '書込方法
        AddHandler cmbKakikomiHouhou.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKakikomiHouhou.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKakikomiHouhou.LostFocus, AddressOf CAST.LostFocus

        '取引先名カナ
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus

        '取引先名
        AddHandler cmbToriName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbToriName.KeyPress, AddressOf CAST.KeyPress
        AddHandler cmbToriName.LostFocus, AddressOf CAST.LostFocus

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
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents Column1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column7 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column12 As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFCMAIN021))
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.Column1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column11 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Column12 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.cmbKakikomiHouhou = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtFuriDateD = New System.Windows.Forms.TextBox()
        Me.txtFuriDateM = New System.Windows.Forms.TextBox()
        Me.txtFuriDateY = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblItakuName = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lblBaitai = New System.Windows.Forms.Label()
        Me.lblFileName = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblCodeKbn = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.lblItakuCode = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblToriHyphen = New System.Windows.Forms.Label()
        Me.txtTorifCode = New System.Windows.Forms.TextBox()
        Me.txtTorisCode = New System.Windows.Forms.TextBox()
        Me.lblToriCode = New System.Windows.Forms.Label()
        Me.cmbToriName = New System.Windows.Forms.ComboBox()
        Me.cmbKana = New System.Windows.Forms.ComboBox()
        Me.lblToriSearch = New System.Windows.Forms.Label()
        Me.btnWrite = New System.Windows.Forms.Button()
        Me.btnReset = New System.Windows.Forms.Button()
        Me.ItakuTreeView = New CASTCommon.ItakuTreeView()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
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
        Me.lblUser.Location = New System.Drawing.Point(683, 12)
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
        Me.Label3.Padding = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label3.Size = New System.Drawing.Size(795, 30)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "＜媒体書込（ディスク→媒体）＞"
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
        Me.Label1.Location = New System.Drawing.Point(611, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 12)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "ログイン名　:"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(651, 517)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(120, 40)
        Me.btnEnd.TabIndex = 4
        Me.btnEnd.Text = "終　了"
        '
        'ListView1
        '
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Column1, Me.Column2, Me.Column3, Me.Column4, Me.Column5, Me.Column6, Me.Column7, Me.Column8, Me.Column9, Me.Column10, Me.Column11, Me.Column12})
        Me.ListView1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(20, 230)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(750, 281)
        Me.ListView1.TabIndex = 5
        Me.ListView1.TabStop = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'Column1
        '
        Me.Column1.Text = "受付No"
        Me.Column1.Width = 50
        '
        'Column2
        '
        Me.Column2.Text = "委託者コード"
        Me.Column2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.Column2.Width = 90
        '
        'Column3
        '
        Me.Column3.Text = "委託者名"
        Me.Column3.Width = 200
        '
        'Column4
        '
        Me.Column4.Text = "振替日"
        Me.Column4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.Column4.Width = 65
        '
        'Column5
        '
        Me.Column5.Text = "種別"
        Me.Column5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.Column5.Width = 65
        '
        'Column6
        '
        Me.Column6.Text = "TR依頼件数"
        Me.Column6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Column6.Width = 75
        '
        'Column7
        '
        Me.Column7.Text = "TR依頼金額"
        Me.Column7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Column7.Width = 100
        '
        'Column8
        '
        Me.Column8.Text = "TR振替済件数"
        Me.Column8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Column8.Width = 75
        '
        'Column9
        '
        Me.Column9.Text = "TR振替済金額"
        Me.Column9.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Column9.Width = 100
        '
        'Column10
        '
        Me.Column10.Text = "TR不能件数"
        Me.Column10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Column10.Width = 75
        '
        'Column11
        '
        Me.Column11.Text = "TR不能金額"
        Me.Column11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.Column11.Width = 100
        '
        'Column12
        '
        Me.Column12.Text = "取引先コード"
        Me.Column12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.Column12.Width = 100
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.cmbKakikomiHouhou)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.txtFuriDateD)
        Me.GroupBox1.Controls.Add(Me.txtFuriDateM)
        Me.GroupBox1.Controls.Add(Me.txtFuriDateY)
        Me.GroupBox1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!)
        Me.GroupBox1.Location = New System.Drawing.Point(20, 46)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(750, 58)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "処理種別"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(343, 29)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(76, 16)
        Me.Label13.TabIndex = 8
        Me.Label13.Text = "書込方法"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmbKakikomiHouhou
        '
        Me.cmbKakikomiHouhou.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKakikomiHouhou.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKakikomiHouhou.ItemHeight = 16
        Me.cmbKakikomiHouhou.Items.AddRange(New Object() {"口座振替(金庫持込)", "口座振替(媒体)", "口座振替(依頼書)", "口座振替(伝票)", "口座振替(学校)", "口座振替(センター直接持込分)"})
        Me.cmbKakikomiHouhou.Location = New System.Drawing.Point(426, 25)
        Me.cmbKakikomiHouhou.Name = "cmbKakikomiHouhou"
        Me.cmbKakikomiHouhou.Size = New System.Drawing.Size(144, 24)
        Me.cmbKakikomiHouhou.TabIndex = 3
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(298, 29)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(24, 16)
        Me.Label10.TabIndex = 7
        Me.Label10.Text = "日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(238, 29)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(24, 16)
        Me.Label9.TabIndex = 6
        Me.Label9.Text = "月"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(178, 29)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(24, 16)
        Me.Label8.TabIndex = 5
        Me.Label8.Text = "年"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(17, 29)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(83, 16)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "振　替　日"
        '
        'txtFuriDateD
        '
        Me.txtFuriDateD.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateD.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateD.Location = New System.Drawing.Point(268, 25)
        Me.txtFuriDateD.MaxLength = 2
        Me.txtFuriDateD.Name = "txtFuriDateD"
        Me.txtFuriDateD.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateD.TabIndex = 2
        '
        'txtFuriDateM
        '
        Me.txtFuriDateM.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateM.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateM.Location = New System.Drawing.Point(208, 25)
        Me.txtFuriDateM.MaxLength = 2
        Me.txtFuriDateM.Name = "txtFuriDateM"
        Me.txtFuriDateM.Size = New System.Drawing.Size(24, 23)
        Me.txtFuriDateM.TabIndex = 1
        '
        'txtFuriDateY
        '
        Me.txtFuriDateY.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtFuriDateY.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtFuriDateY.Location = New System.Drawing.Point(128, 26)
        Me.txtFuriDateY.MaxLength = 4
        Me.txtFuriDateY.Name = "txtFuriDateY"
        Me.txtFuriDateY.Size = New System.Drawing.Size(44, 23)
        Me.txtFuriDateY.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lblItakuName)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.lblBaitai)
        Me.GroupBox2.Controls.Add(Me.lblFileName)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Controls.Add(Me.lblCodeKbn)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.lblItakuCode)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.lblToriHyphen)
        Me.GroupBox2.Controls.Add(Me.txtTorifCode)
        Me.GroupBox2.Controls.Add(Me.txtTorisCode)
        Me.GroupBox2.Controls.Add(Me.lblToriCode)
        Me.GroupBox2.Controls.Add(Me.cmbToriName)
        Me.GroupBox2.Controls.Add(Me.cmbKana)
        Me.GroupBox2.Controls.Add(Me.lblToriSearch)
        Me.GroupBox2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!)
        Me.GroupBox2.Location = New System.Drawing.Point(20, 110)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(750, 114)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "取引先情報"
        '
        'lblItakuName
        '
        Me.lblItakuName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblItakuName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.lblItakuName.Location = New System.Drawing.Point(382, 52)
        Me.lblItakuName.Name = "lblItakuName"
        Me.lblItakuName.Size = New System.Drawing.Size(357, 23)
        Me.lblItakuName.TabIndex = 665
        Me.lblItakuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(288, 56)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(72, 16)
        Me.Label11.TabIndex = 664
        Me.Label11.Text = "委託者名"
        '
        'lblBaitai
        '
        Me.lblBaitai.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblBaitai.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.lblBaitai.Location = New System.Drawing.Point(382, 83)
        Me.lblBaitai.Name = "lblBaitai"
        Me.lblBaitai.Size = New System.Drawing.Size(53, 23)
        Me.lblBaitai.TabIndex = 663
        Me.lblBaitai.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblFileName
        '
        Me.lblFileName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFileName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.lblFileName.Location = New System.Drawing.Point(441, 83)
        Me.lblFileName.Name = "lblFileName"
        Me.lblFileName.Size = New System.Drawing.Size(231, 23)
        Me.lblFileName.TabIndex = 662
        Me.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(288, 86)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(88, 16)
        Me.Label7.TabIndex = 661
        Me.Label7.Text = "ファイル名"
        '
        'lblCodeKbn
        '
        Me.lblCodeKbn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCodeKbn.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.lblCodeKbn.Location = New System.Drawing.Point(649, 21)
        Me.lblCodeKbn.Name = "lblCodeKbn"
        Me.lblCodeKbn.Size = New System.Drawing.Size(90, 23)
        Me.lblCodeKbn.TabIndex = 3
        Me.lblCodeKbn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(554, 24)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 16)
        Me.Label6.TabIndex = 660
        Me.Label6.Text = "コード区分"
        '
        'lblItakuCode
        '
        Me.lblItakuCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblItakuCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.lblItakuCode.Location = New System.Drawing.Point(128, 83)
        Me.lblItakuCode.Name = "lblItakuCode"
        Me.lblItakuCode.Size = New System.Drawing.Size(90, 23)
        Me.lblItakuCode.TabIndex = 2
        Me.lblItakuCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(17, 86)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(104, 16)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "委託者コード"
        '
        'lblToriHyphen
        '
        Me.lblToriHyphen.AutoSize = True
        Me.lblToriHyphen.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToriHyphen.Location = New System.Drawing.Point(221, 56)
        Me.lblToriHyphen.Name = "lblToriHyphen"
        Me.lblToriHyphen.Size = New System.Drawing.Size(24, 16)
        Me.lblToriHyphen.TabIndex = 8
        Me.lblToriHyphen.Text = "－"
        '
        'txtTorifCode
        '
        Me.txtTorifCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorifCode.Location = New System.Drawing.Point(247, 52)
        Me.txtTorifCode.MaxLength = 2
        Me.txtTorifCode.Name = "txtTorifCode"
        Me.txtTorifCode.Size = New System.Drawing.Size(24, 23)
        Me.txtTorifCode.TabIndex = 1
        '
        'txtTorisCode
        '
        Me.txtTorisCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtTorisCode.Location = New System.Drawing.Point(128, 52)
        Me.txtTorisCode.MaxLength = 10
        Me.txtTorisCode.Name = "txtTorisCode"
        Me.txtTorisCode.Size = New System.Drawing.Size(90, 23)
        Me.txtTorisCode.TabIndex = 0
        '
        'lblToriCode
        '
        Me.lblToriCode.AutoSize = True
        Me.lblToriCode.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblToriCode.Location = New System.Drawing.Point(17, 55)
        Me.lblToriCode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToriCode.Name = "lblToriCode"
        Me.lblToriCode.Size = New System.Drawing.Size(101, 16)
        Me.lblToriCode.TabIndex = 7
        Me.lblToriCode.Text = "取引先コード"
        '
        'cmbToriName
        '
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbToriName.FormattingEnabled = True
        Me.cmbToriName.Location = New System.Drawing.Point(177, 20)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(354, 21)
        Me.cmbToriName.TabIndex = 6
        Me.cmbToriName.TabStop = False
        '
        'cmbKana
        '
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!)
        Me.cmbKana.FormattingEnabled = True
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(128, 20)
        Me.cmbKana.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 5
        Me.cmbKana.TabStop = False
        '
        'lblToriSearch
        '
        Me.lblToriSearch.AutoSize = True
        Me.lblToriSearch.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToriSearch.Location = New System.Drawing.Point(17, 24)
        Me.lblToriSearch.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToriSearch.Name = "lblToriSearch"
        Me.lblToriSearch.Size = New System.Drawing.Size(88, 16)
        Me.lblToriSearch.TabIndex = 4
        Me.lblToriSearch.Text = "取引先検索"
        '
        'btnWrite
        '
        Me.btnWrite.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnWrite.Location = New System.Drawing.Point(20, 517)
        Me.btnWrite.Name = "btnWrite"
        Me.btnWrite.Size = New System.Drawing.Size(120, 40)
        Me.btnWrite.TabIndex = 2
        Me.btnWrite.Text = "書込開始"
        '
        'btnReset
        '
        Me.btnReset.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReset.Location = New System.Drawing.Point(525, 517)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(120, 40)
        Me.btnReset.TabIndex = 3
        Me.btnReset.Text = "取　消"
        '
        'ItakuTreeView
        '
        Me.ItakuTreeView.ITV_ADD_SQL = "BAITAI_CODE_T IN ('01','11','12','13','14','15')"
        Me.ItakuTreeView.ITV_CANCEL_BUTTON = Nothing
        Me.ItakuTreeView.ITV_FSYORI_KBN = "1"
        Me.ItakuTreeView.ITV_NEXT_FOCUS = Me.txtTorifCode
        Me.ItakuTreeView.ITV_NEXT_FOCUS_BUTTON = Me.btnWrite
        Me.ItakuTreeView.ITV_SELECT_BUTTON = Nothing
        Me.ItakuTreeView.ITV_SORT_PATN = "2"
        Me.ItakuTreeView.ITV_TORIF_CODE_TEXTBOX = Me.txtTorifCode
        Me.ItakuTreeView.ITV_TORIS_CODE_TEXTBOX = Me.txtTorisCode
        Me.ItakuTreeView.Location = New System.Drawing.Point(782, 54)
        Me.ItakuTreeView.Name = "ItakuTreeView"
        Me.ItakuTreeView.Size = New System.Drawing.Size(200, 508)
        Me.ItakuTreeView.TabIndex = 215
        '
        'KFCMAIN021
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(984, 562)
        Me.Controls.Add(Me.ItakuTreeView)
        Me.Controls.Add(Me.btnReset)
        Me.Controls.Add(Me.btnWrite)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.btnEnd)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(1000, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(1000, 600)
        Me.Name = "KFCMAIN021"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFCMAIN021"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents lblItakuCode As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblToriHyphen As System.Windows.Forms.Label
    Friend WithEvents txtTorifCode As System.Windows.Forms.TextBox
    Friend WithEvents txtTorisCode As System.Windows.Forms.TextBox
    Friend WithEvents lblToriCode As System.Windows.Forms.Label
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents lblToriSearch As System.Windows.Forms.Label
    Friend WithEvents btnWrite As System.Windows.Forms.Button
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents lblCodeKbn As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtFuriDateD As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateM As System.Windows.Forms.TextBox
    Friend WithEvents txtFuriDateY As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents cmbKakikomiHouhou As System.Windows.Forms.ComboBox
    Friend WithEvents Column8 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column9 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column10 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Column11 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ItakuTreeView As CASTCommon.ItakuTreeView
    Friend WithEvents lblFileName As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblBaitai As System.Windows.Forms.Label
    Friend WithEvents lblItakuName As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label

End Class
