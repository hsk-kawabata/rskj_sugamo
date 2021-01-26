Option Explicit On 
Option Strict On

Public Class KFUMAST011
    Inherits System.Windows.Forms.Form

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。
        AddHandler ListView1.KeyPress, AddressOf CAST.KeyPress
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
    Friend WithEvents CmdTrue As System.Windows.Forms.Button
    Friend WithEvents CmdFalse As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents BK_NAME As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.CmdTrue = New System.Windows.Forms.Button
        Me.CmdFalse = New System.Windows.Forms.Button
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.BK_NAME = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'CmdTrue
        '
        Me.CmdTrue.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdTrue.Location = New System.Drawing.Point(196, 172)
        Me.CmdTrue.Name = "CmdTrue"
        Me.CmdTrue.Size = New System.Drawing.Size(96, 28)
        Me.CmdTrue.TabIndex = 1
        Me.CmdTrue.Text = "確 定"
        '
        'CmdFalse
        '
        Me.CmdFalse.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CmdFalse.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdFalse.Location = New System.Drawing.Point(300, 172)
        Me.CmdFalse.Name = "CmdFalse"
        Me.CmdFalse.Size = New System.Drawing.Size(96, 28)
        Me.CmdFalse.TabIndex = 2
        Me.CmdFalse.Text = "キャンセル"
        '
        'ListView1
        '
        Me.ListView1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(12, 36)
        Me.ListView1.MultiSelect = False
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(384, 128)
        Me.ListView1.TabIndex = 0
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'BK_NAME
        '
        Me.BK_NAME.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BK_NAME.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.BK_NAME.Location = New System.Drawing.Point(12, 8)
        Me.BK_NAME.Name = "BK_NAME"
        Me.BK_NAME.Size = New System.Drawing.Size(384, 24)
        Me.BK_NAME.TabIndex = 3
        Me.BK_NAME.Text = "BK_NAME"
        Me.BK_NAME.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'KFUMAST011
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.CancelButton = Me.CmdFalse
        Me.ClientSize = New System.Drawing.Size(404, 212)
        Me.ControlBox = False
        Me.Controls.Add(Me.BK_NAME)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.CmdFalse)
        Me.Controls.Add(Me.CmdTrue)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "KFUMAST011"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.ResumeLayout(False)

    End Sub

#End Region

    '呼出画面オブジェクト
    Public KFUMAST010 As KFUMAST010
    Public oraReader As CASTCommon.MyOracleReader
    Public intLoadFlg As Integer = 0
    Private Const ThisModuleName As String = "KFUMAST011"

    Private BatchLog As New CASTCommon.BatchLOG("KFUMAST011", "金融機関／支店参照画面")
    'Const msgTitle As String = "金融機関／支店参照画面"
    Const msgTitle As String = "金融機関／支店参照画面(KFUMAST011)"

    '画面起動時処理
    Private Sub KFUMAST011_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim XRet As Integer
        Dim YRet As Integer

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")
            Me.CmdFalse.DialogResult = DialogResult.None

            If intLoadFlg = 0 Then
                '---------------------------------------------------------
                '金融機関参照
                '---------------------------------------------------------
                With Me.ListView1
                    .Clear()
                    .Columns.Add("金融機関コード", 0, HorizontalAlignment.Left)
                    '2011/03/30 削除日考慮 項目追加 ここから
                    '.Columns.Add("付加コード", 100, HorizontalAlignment.Center)
                    '.Columns.Add("金融機関名", 270, HorizontalAlignment.Left)
                    .Columns.Add("付加コード", 90, HorizontalAlignment.Center)
                    .Columns.Add("金融機関名", 200, HorizontalAlignment.Left)
                    .Columns.Add("金融機関名カナ", 0, HorizontalAlignment.Left)
                    .Columns.Add("削除日", 80, HorizontalAlignment.Left)
                    '2011/03/30 削除日考慮 項目追加 ここまで
                End With

                Me.BK_NAME.Text = ""

                '選択用スプレッド領域の描画
                Do Until oraReader.EOF
                    If Me.BK_NAME.Text = "" Then
                        Me.BK_NAME.Text = GCom.NzStr(oraReader.GetString("KIN_NNAME_N"))
                    End If

                    '2011/03/30 削除日考慮 項目追加
                    'Dim Data(2) As String
                    Dim Data(4) As String

                    Data(0) = oraReader.GetString("KIN_NO_N").ToString
                    Data(1) = oraReader.GetString("KIN_FUKA_N").ToString
                    Data(2) = oraReader.GetString("KIN_NNAME_N").ToString
                    '2011/03/30 削除日考慮 削除日がある場合は画面表示する ここから
                    Data(3) = oraReader.GetString("KIN_KNAME_N").ToString

                    If oraReader.GetString("KIN_DEL_DATE_N").ToString <> "00000000" Then
                        Data(4) = oraReader.GetString("KIN_DEL_DATE_N").Insert(4, "/").Insert(7, "/")
                    Else
                        Data(4) = ""
                    End If
                    '2011/03/30 削除日考慮 削除日がある場合は画面表示する ここまで

                    Dim vLstItem As New ListViewItem(Data)
                    Me.ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    oraReader.NextRead()

                Loop

                Me.ListView1.Items(0).Selected = True

                '表示位置の調整
                With KFUMAST010
                    XRet = .Location.X + .btnKin_Fuka_Sansyo.Location.X + .btnKin_Fuka_Sansyo.Size.Width
                    YRet = .Location.Y + .btnKin_Fuka_Sansyo.Location.Y + .btnKin_Fuka_Sansyo.Size.Height
                End With
            Else
                '---------------------------------------------------------
                '支店参照
                '---------------------------------------------------------
                With Me.ListView1
                    .Clear()
                    .Columns.Add("支店コード", 0, HorizontalAlignment.Left)
                    '2011/03/30 削除日考慮 項目追加 ここから
                    '.Columns.Add("付加コード", 100, HorizontalAlignment.Center)
                    '.Columns.Add("支店名", 270, HorizontalAlignment.Left)
                    .Columns.Add("付加コード", 90, HorizontalAlignment.Center)
                    .Columns.Add("支店名", 200, HorizontalAlignment.Left)
                    .Columns.Add("削除日", 80, HorizontalAlignment.Left)
                    '2011/03/30 削除日考慮 項目追加 ここまで
                End With
                Me.BK_NAME.Text = ""


                '選択用スプレッド領域の描画
                Do Until oraReader.EOF

                    If Me.BK_NAME.Text = "" Then
                        Me.BK_NAME.Text = oraReader.GetString("SIT_NNAME_N").ToString
                    End If

                    '2011/03/30 削除日考慮 項目追加
                    'Dim Data(2) As String
                    Dim Data(3) As String

                    Data(0) = oraReader.GetString("SIT_NO_N").ToString
                    Data(1) = oraReader.GetString("SIT_FUKA_N").ToString
                    Data(2) = oraReader.GetString("SIT_NNAME_N").ToString
                    '2011/03/30 削除日考慮 削除日がある場合は画面表示する ここから
                    If oraReader.GetString("SIT_DEL_DATE_N").ToString <> "00000000" Then
                        Data(3) = oraReader.GetString("SIT_DEL_DATE_N").Insert(4, "/").Insert(7, "/")
                    Else
                        Data(3) = ""
                    End If
                    '2011/03/30 削除日考慮 削除日がある場合は画面表示する ここまで

                    Dim vLstItem As New ListViewItem(Data)
                    Me.ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    oraReader.NextRead()
                Loop

                Me.ListView1.Items(0).Selected = True

                '表示位置の調整
                With KFUMAST010
                    XRet = .Location.X + .btnSit_Fuka_Sansyo.Location.X + .btnSit_Fuka_Sansyo.Size.Width
                    YRet = .Location.Y + .btnSit_Fuka_Sansyo.Location.Y + .btnSit_Fuka_Sansyo.Size.Height
                End With
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)終了", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If
            Me.Location = New Point(XRet, YRet)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)終了", "成功", "")
        End Try
    End Sub

    '確定ボタン処理
    Private Sub CmdTrue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdTrue.Click
        Call SetCustomer(True)
    End Sub

    '一覧表示領域ダブルクリック処理
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call SetCustomer(True)
    End Sub

    '戻るボタン処理
    Private Sub CmdFalse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdFalse.Click
        Call SetCustomer()
    End Sub

    '
    ' 機　能 : 金融機関選択オペレーション後処理
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 確定操作かキャンセル操作
    '
    ' 備　考 : 自振専用(総給振は別のプロジェクト)
    '    
    Private Sub SetCustomer(Optional ByVal avAction As Boolean = False)
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(金融機関選択処理)開始", "成功", "")
            If avAction Then
                '2011/03/30 選択行が無い場合を考慮 ここから
                If ListView1.SelectedItems.Count = 0 Then
                    '処理を抜ける
                    MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End If
                '2011/03/30 選択行が無い場合を考慮 ここまで
                With KFUMAST010
                    If intLoadFlg = 0 Then
                        .KIN_FUKA_N.Text = GCom.SelectedItem(ListView1, 1).ToString
                        '2011/03/30 削除日考慮 選択をKFUMAST010に反映する ここから
                        .KIN_NNAME_N.Text = GCom.SelectedItem(ListView1, 2).ToString
                        .KIN_KNAME_N.Text = GCom.SelectedItem(ListView1, 3).ToString

                        Dim KIN_DEL_DATE As String = GCom.SelectedItem(ListView1, 4).ToString

                        If KIN_DEL_DATE <> "" Then
                            .KIN_DEL_DATE_N.Text = KIN_DEL_DATE.Substring(0, 4)
                            .KIN_DEL_DATE_N1.Text = KIN_DEL_DATE.Substring(5, 2)
                            .KIN_DEL_DATE_N2.Text = KIN_DEL_DATE.Substring(8, 2)
                        Else
                            .KIN_DEL_DATE_N.Text = ""
                            .KIN_DEL_DATE_N1.Text = ""
                            .KIN_DEL_DATE_N2.Text = ""
                        End If
                        '2011/03/30 削除日考慮 選択をKFUMAST010に反映する ここまで
                        'Call .btnSansyo.PerformClick()
                        .CancelFlg = True '2011/03/30 選択がOKなのでフォーカスを次へ
                    Else
                        .SIT_FUKA_N.Text = GCom.SelectedItem(ListView1, 1).ToString
                        '2011/03/30 削除日考慮 選択をKFUMAST010に反映する ここから
                        Dim SIT_DEL_DATE As String = GCom.SelectedItem(ListView1, 3).ToString

                        If SIT_DEL_DATE <> "" Then
                            .SIT_DEL_DATE_N.Text = SIT_DEL_DATE.Substring(0, 4)
                            .SIT_DEL_DATE_N1.Text = SIT_DEL_DATE.Substring(5, 2)
                            .SIT_DEL_DATE_N2.Text = SIT_DEL_DATE.Substring(8, 2)
                        Else
                            .SIT_DEL_DATE_N.Text = ""
                            .SIT_DEL_DATE_N1.Text = ""
                            .SIT_DEL_DATE_N2.Text = ""
                        End If
                        '2011/03/30 削除日考慮 選択をKFUMAST010に反映する ここまで
                        Call .btnSansyo.PerformClick()
                    End If
                End With
            End If

            Me.Close()
            Me.Dispose()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(金融機関選択処理)終了", "失敗", ex.Message)
        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(金融機関選択処理)終了", "成功", "")
        End Try
    End Sub

End Class
