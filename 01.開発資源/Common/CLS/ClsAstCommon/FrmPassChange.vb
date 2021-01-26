Imports System
Imports System.Data
Imports System.Windows.Forms

Public Class FrmPassChange
    Inherits System.Windows.Forms.Form

#Region "宣言"

    'ログ書込クラス
    Private BatchLog As New CASTCommon.BatchLOG("KFUMAST050", "パスワード変更画面")

    'キーコントロールクラス
    Private CAST As New CASTCommon.Events
    Friend WithEvents TxtUser1 As System.Windows.Forms.Label
    Private mLogin As ClsLogin

    Public Sub New(ByVal login As ClsLogin)
        MyClass.New()
        mLogin = login
    End Sub

    Public WriteOnly Property UserID() As String
        Set(ByVal Value As String)
            TxtUser1.Text = Value
        End Set
    End Property

#End Region

#Region " Windows フォーム デザイナで生成されたコード "
    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。(CAstCommon.dll)
        AddHandler txtPassword1_Kyu.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtPassword1_Kyu.LostFocus, AddressOf CAST.LostFocus
        AddHandler txtPassword1_Kyu.KeyPress, AddressOf CAST.KeyPress
        AddHandler txtPassword1_Sin.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtPassword1_Sin.LostFocus, AddressOf CAST.LostFocus
        AddHandler txtPassword1_Sin.KeyPress, AddressOf CAST.KeyPress
        AddHandler txtPassword1_Sin_Kakunin.GotFocus, AddressOf CAST.GotFocus
        AddHandler txtPassword1_Sin_Kakunin.LostFocus, AddressOf CAST.LostFocus
        AddHandler txtPassword1_Sin_Kakunin.KeyPress, AddressOf CAST.KeyPress
    End Sub

    ' Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
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
    'Windows フォーム デザイナを使って変更してください。  
    ' コード エディタを使って変更しないでください。
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtPassword1_Kyu As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword1_Sin As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword1_Sin_Kakunin As System.Windows.Forms.TextBox
    Friend WithEvents btnKAKUNIN As System.Windows.Forms.Button
    Friend WithEvents btnEND As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPassChange))
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtPassword1_Kyu = New System.Windows.Forms.TextBox()
        Me.btnKAKUNIN = New System.Windows.Forms.Button()
        Me.btnEND = New System.Windows.Forms.Button()
        Me.txtPassword1_Sin = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtPassword1_Sin_Kakunin = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtUser1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(103, 141)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(135, 15)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "現在のパスワード"
        '
        'txtPassword1_Kyu
        '
        Me.txtPassword1_Kyu.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPassword1_Kyu.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtPassword1_Kyu.Location = New System.Drawing.Point(313, 138)
        Me.txtPassword1_Kyu.MaxLength = 20
        Me.txtPassword1_Kyu.Name = "txtPassword1_Kyu"
        Me.txtPassword1_Kyu.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword1_Kyu.Size = New System.Drawing.Size(196, 22)
        Me.txtPassword1_Kyu.TabIndex = 0
        '
        'btnKAKUNIN
        '
        Me.btnKAKUNIN.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnKAKUNIN.Location = New System.Drawing.Point(163, 396)
        Me.btnKAKUNIN.Name = "btnKAKUNIN"
        Me.btnKAKUNIN.Size = New System.Drawing.Size(128, 36)
        Me.btnKAKUNIN.TabIndex = 3
        Me.btnKAKUNIN.Text = "更　新"
        '
        'btnEND
        '
        Me.btnEND.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnEND.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEND.Location = New System.Drawing.Point(343, 396)
        Me.btnEND.Name = "btnEND"
        Me.btnEND.Size = New System.Drawing.Size(128, 36)
        Me.btnEND.TabIndex = 4
        Me.btnEND.Text = "終　了"
        '
        'txtPassword1_Sin
        '
        Me.txtPassword1_Sin.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPassword1_Sin.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtPassword1_Sin.Location = New System.Drawing.Point(313, 196)
        Me.txtPassword1_Sin.MaxLength = 20
        Me.txtPassword1_Sin.Name = "txtPassword1_Sin"
        Me.txtPassword1_Sin.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword1_Sin.Size = New System.Drawing.Size(196, 22)
        Me.txtPassword1_Sin.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(103, 199)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(135, 15)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "新しいパスワード"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(103, 257)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(199, 15)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "新しいパスワードの再入力"
        '
        'txtPassword1_Sin_Kakunin
        '
        Me.txtPassword1_Sin_Kakunin.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPassword1_Sin_Kakunin.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.txtPassword1_Sin_Kakunin.Location = New System.Drawing.Point(313, 254)
        Me.txtPassword1_Sin_Kakunin.MaxLength = 20
        Me.txtPassword1_Sin_Kakunin.Name = "txtPassword1_Sin_Kakunin"
        Me.txtPassword1_Sin_Kakunin.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword1_Sin_Kakunin.Size = New System.Drawing.Size(196, 22)
        Me.txtPassword1_Sin_Kakunin.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(0, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(633, 34)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "＜ログインパスワード変更＞"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TxtUser1
        '
        Me.TxtUser1.AutoSize = True
        Me.TxtUser1.Location = New System.Drawing.Point(247, 78)
        Me.TxtUser1.Name = "TxtUser1"
        Me.TxtUser1.Size = New System.Drawing.Size(38, 12)
        Me.TxtUser1.TabIndex = 12
        Me.TxtUser1.Text = "Label1"
        Me.TxtUser1.Visible = False
        '
        'FrmPassChange
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.CancelButton = Me.btnEND
        Me.ClientSize = New System.Drawing.Size(624, 442)
        Me.Controls.Add(Me.TxtUser1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtPassword1_Sin_Kakunin)
        Me.Controls.Add(Me.txtPassword1_Sin)
        Me.Controls.Add(Me.txtPassword1_Kyu)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnEND)
        Me.Controls.Add(Me.btnKAKUNIN)
        Me.Controls.Add(Me.Label2)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(640, 480)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(640, 480)
        Me.Name = "FrmPassChange"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFUMAST050"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

#Region "画面制御"

    Private Sub FrmPassChange_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
        'BatchLog.Write("0000000000-00", "00000000", "ロード(開始)", "成功")
        'BatchLog.Write("0000000000-00", "00000000", "ロード(終了)", "成功")
        '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***
    End Sub

#End Region

#Region "ボタン"

    Private Sub btnKAKUNIN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKAKUNIN.Click


        If txtPassword1_Kyu.Text = "" Then
            MessageBox.Show(MSG0007W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPassword1_Kyu.Focus()
            Return
        End If

        If txtPassword1_Sin.Text = "" Then
            MessageBox.Show(MSG0008W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPassword1_Sin.Focus()
            Return
        End If

        If txtPassword1_Sin_Kakunin.Text = "" Then
            MessageBox.Show(MSG0009W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPassword1_Sin_Kakunin.Focus()
            Return
        End If

        If txtPassword1_Sin.Text.Length < 6 Then
            MessageBox.Show(MSG0229W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPassword1_Sin.Focus()
            Return
        End If

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Try
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワード変更(開始)", "成功")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "パスワード変更", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            If mLogin Is Nothing Then
                mLogin = New ClsLogin
            End If

            ' パスワードチェック
            Select Case mLogin.PasswordCheck(TxtUser1.Text, txtPassword1_Kyu.Text, txtPassword1_Sin.Text, txtPassword1_Sin_Kakunin.Text)
                Case 0
                    mLogin = Nothing

                    Me.DialogResult = System.Windows.Forms.DialogResult.OK
                Case 1
                    txtPassword1_Kyu.Focus()
                Case Else
                    txtPassword1_Sin.Focus()
            End Select

        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワード変更", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "パスワード変更", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワード変更(終了)", "成功")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "パスワード変更", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        End Try

    End Sub

    Private Sub btnEND_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEND.Click

        Try
            '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "クローズ(開始)", "成功")
            '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel

        Catch ex As Exception
            MessageBox.Show(MSG0006E, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "クローズ", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "クローズ", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        Finally
            '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "クローズ(終了)", "成功")
            '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***

        End Try
    End Sub

#End Region


End Class
