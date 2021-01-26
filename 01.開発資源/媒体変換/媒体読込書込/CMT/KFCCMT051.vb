Public Class KFCCMT051
    Inherits System.Windows.Forms.Form

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

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
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents LblDateCaption As System.Windows.Forms.Label
    Friend WithEvents LblUserIDCaption As System.Windows.Forms.Label
    Friend WithEvents ExeBtn As System.Windows.Forms.Button
    Friend WithEvents RetBtn As System.Windows.Forms.Button
    Friend WithEvents LblOtherCMT As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFCCMT051))
        Me.LblOtherCMT = New System.Windows.Forms.Label
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.LblDateCaption = New System.Windows.Forms.Label
        Me.LblUserIDCaption = New System.Windows.Forms.Label
        Me.ExeBtn = New System.Windows.Forms.Button
        Me.RetBtn = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'LblOtherCMT
        '
        Me.LblOtherCMT.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LblOtherCMT.Location = New System.Drawing.Point(228, 9)
        Me.LblOtherCMT.Name = "LblOtherCMT"
        Me.LblOtherCMT.Size = New System.Drawing.Size(336, 24)
        Me.LblOtherCMT.TabIndex = 0
        Me.LblOtherCMT.Text = "＜他行分CMT処理＞"
        Me.LblOtherCMT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Location = New System.Drawing.Point(680, 40)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 23
        Me.lblDate.Text = "9999年99月99日"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Location = New System.Drawing.Point(680, 16)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(47, 12)
        Me.lblUser.TabIndex = 22
        Me.lblUser.Text = "ユーザ名"
        Me.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblDateCaption
        '
        Me.LblDateCaption.AutoSize = True
        Me.LblDateCaption.Location = New System.Drawing.Point(600, 40)
        Me.LblDateCaption.Name = "LblDateCaption"
        Me.LblDateCaption.Size = New System.Drawing.Size(73, 12)
        Me.LblDateCaption.TabIndex = 21
        Me.LblDateCaption.Text = "システム日付："
        Me.LblDateCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblUserIDCaption
        '
        Me.LblUserIDCaption.AutoSize = True
        Me.LblUserIDCaption.Location = New System.Drawing.Point(616, 16)
        Me.LblUserIDCaption.Name = "LblUserIDCaption"
        Me.LblUserIDCaption.Size = New System.Drawing.Size(59, 12)
        Me.LblUserIDCaption.TabIndex = 20
        Me.LblUserIDCaption.Text = "ログイン名："
        Me.LblUserIDCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ExeBtn
        '
        Me.ExeBtn.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.ExeBtn.Location = New System.Drawing.Point(232, 523)
        Me.ExeBtn.Name = "ExeBtn"
        Me.ExeBtn.Size = New System.Drawing.Size(120, 40)
        Me.ExeBtn.TabIndex = 25
        Me.ExeBtn.Text = "実行"
        '
        'RetBtn
        '
        Me.RetBtn.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.RetBtn.Location = New System.Drawing.Point(649, 523)
        Me.RetBtn.Name = "RetBtn"
        Me.RetBtn.Size = New System.Drawing.Size(120, 40)
        Me.RetBtn.TabIndex = 26
        Me.RetBtn.Text = "戻る"
        '
        'KFCCMT051
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(794, 575)
        Me.Controls.Add(Me.RetBtn)
        Me.Controls.Add(Me.ExeBtn)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.LblDateCaption)
        Me.Controls.Add(Me.LblUserIDCaption)
        Me.Controls.Add(Me.LblOtherCMT)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFCCMT051"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFCCMT051"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Const ThisModuleName As String = "KFCCMT051.vb"
    Private MyOwnerForm As Form
    Private bSendFlag As Boolean ' 請求時 true / 結果受け取り時 false

    '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
    Private Const strSendOther As String = "＜他行向請求データ書込＞"
    Private Const strReceiveOther As String = "＜他行向結果データ取込＞"
    'Private Const strSendOther = "＜他行向請求データ書込＞"
    'Private Const strReceiveOther = "＜他行向結果データ取込＞"
    '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

    '画面起動時処理
    Private Sub KFJCMT0051G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GCom.GLog.Job1 = "他行向CMT処理画面"
        GCom.GLog.Job2 = "画面起動時処理"
        Try
            ' ユーザID・システム日付の取得と設定
            MyOwnerForm = GOwnerForm
            GOwnerForm = Me
            Me.lblUser.Text = GCom.GetUserID
            Me.lblDate.Text = String.Format("{0:yyyy年MM月dd日}", GCom.GetSysDate)

            '*** 修正 mitsu 2008/09/02 印字位置調整 ***
            'Call GCom.SetMonitorTopArea(LblUserIDCaption, LblDateCaption, lblUser, lblDate, -100)
            Call GCom.SetMonitorTopArea(LblUserIDCaption, LblDateCaption, lblUser, lblDate)
            '******************************************

            If (Me.bSendFlag) Then
                Me.LblOtherCMT.Text = strSendOther
            Else
                Me.LblOtherCMT.Text = strReceiveOther
            End If
        Catch ex As Exception
            With GCom.GLog
                .Result = MenteCommon.clsCommon.NG
                .Discription = ex.Message
            End With
            GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
        End Try

    End Sub

    ' 画面表示（再表示）時処理
    Private Sub KFJCMT0051G_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job1 = "他行向CMT処理画面"
    End Sub

    ' 画面終了時処理
    Private Sub KFJCMT0051G_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        GOwnerForm = MyOwnerForm
        GOwnerForm.Visible = True
        GCom.SetFormEnabled(GOwnerForm)
    End Sub

    ' 実行ボタン処理
    Private Sub ExeBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExeBtn.Click
        If Me.bSendFlag Then
            ' 請求データ作成
            CmtCom.WriteOtherCMT()
        Else
            ' 結果データ読取
            CmtCom.ReadOtherCMT()
        End If
    End Sub

    ' 戻るボタン処理
    Private Sub RetBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RetBtn.Click
        Me.Close()
        Me.Dispose()
    End Sub

    ' 請求/結果読取の切替
    Property SendFlag() As Boolean
        Get
            Return bSendFlag
        End Get
        Set(ByVal Value As Boolean)
            bSendFlag = Value
        End Set
    End Property

End Class
