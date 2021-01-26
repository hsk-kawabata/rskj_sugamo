Imports System.Text
Imports CASTCommon
Public Class FrmSubJob
    Inherits System.Windows.Forms.Form
    Private Const msgTitle As String = "ジョブ詳細情報画面(KFUJOBW011)"
    Private mJobTuuban As Integer           ' ジョブ通番
    Private mJobID As String                ' ジョブID
    Private mJobPara As String              ' パラメータ
    Private mJobMessage As String           ' メッセージ
    Private mStatus As String               ' ステータス
    Private OraMain As CASTCommon.MyOracle
    Friend WithEvents Label1 As System.Windows.Forms.Label

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
    ' スレッドロック
    Private mThreadLock As Object   ' スレッド排他ロックオブジェクト
    Public WriteOnly Property ThreadLock() As Object
        Set(ByVal Value As Object)
            mThreadLock = Value
        End Set
    End Property
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

    ' Oracle
    Public WriteOnly Property DB() As CASTCommon.MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            OraMain = Value
        End Set
    End Property

    ' ジョブ通番
    Public WriteOnly Property JobTuuban() As Integer
        Set(ByVal Value As Integer)
            mJobTuuban = Value

            lblTuuban.Text = Value
        End Set
    End Property

    ' ジョブID
    Public WriteOnly Property JobID() As String
        Set(ByVal Value As String)
            mJobID = Value

            lblJobId.Text = Value
        End Set
    End Property

    ' ジョブパラメータ
    Public WriteOnly Property JobPara() As String
        Set(ByVal Value As String)
            mJobPara = Value

            txtPara.Text = Value
        End Set
    End Property

    ' メッセージ
    Public WriteOnly Property JobMessage() As String
        Set(ByVal Value As String)
            mJobMessage = Value

            txtErrMessage.Text = Value
        End Set
    End Property

    ' ステータス
    Public WriteOnly Property Status() As String
        Set(ByVal Value As String)
            mStatus = Value
        End Set
    End Property

    Private mOwner As FrmJOB
    Public WriteOnly Property OwnerForm() As FrmJOB
        Set(ByVal Value As FrmJOB)
            mOwner = Value
        End Set
    End Property

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
    Friend WithEvents btnDetailEnd As System.Windows.Forms.Button
    Friend WithEvents btnReAction As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents txtPara As System.Windows.Forms.TextBox
    Friend WithEvents txtErrMessage As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents lblJobId As System.Windows.Forms.Label
    Friend WithEvents lblTuuban As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSubJob))
        Me.btnDetailEnd = New System.Windows.Forms.Button()
        Me.btnReAction = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.txtPara = New System.Windows.Forms.TextBox()
        Me.txtErrMessage = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.lblJobId = New System.Windows.Forms.Label()
        Me.lblTuuban = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnDetailEnd
        '
        Me.btnDetailEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDetailEnd.Location = New System.Drawing.Point(372, 100)
        Me.btnDetailEnd.Name = "btnDetailEnd"
        Me.btnDetailEnd.Size = New System.Drawing.Size(88, 28)
        Me.btnDetailEnd.TabIndex = 2
        Me.btnDetailEnd.Text = "終　了"
        '
        'btnReAction
        '
        Me.btnReAction.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReAction.Location = New System.Drawing.Point(276, 100)
        Me.btnReAction.Name = "btnReAction"
        Me.btnReAction.Size = New System.Drawing.Size(88, 28)
        Me.btnReAction.TabIndex = 1
        Me.btnReAction.Text = "再実行"
        '
        'btnCancel
        '
        Me.btnCancel.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(20, 100)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(88, 28)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "ｼﾞｮﾌﾞｷｬﾝｾﾙ"
        '
        'txtPara
        '
        Me.txtPara.BackColor = System.Drawing.SystemColors.Control
        Me.txtPara.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPara.Location = New System.Drawing.Point(284, 38)
        Me.txtPara.Name = "txtPara"
        Me.txtPara.ReadOnly = True
        Me.txtPara.Size = New System.Drawing.Size(198, 19)
        Me.txtPara.TabIndex = 190
        Me.txtPara.TabStop = False
        '
        'txtErrMessage
        '
        Me.txtErrMessage.BackColor = System.Drawing.SystemColors.Control
        Me.txtErrMessage.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtErrMessage.Location = New System.Drawing.Point(20, 76)
        Me.txtErrMessage.Name = "txtErrMessage"
        Me.txtErrMessage.ReadOnly = True
        Me.txtErrMessage.Size = New System.Drawing.Size(462, 19)
        Me.txtErrMessage.TabIndex = 188
        Me.txtErrMessage.TabStop = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(12, 60)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(119, 13)
        Me.Label8.TabIndex = 184
        Me.Label8.Text = "エラーメッセージ"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(192, 41)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(91, 13)
        Me.Label3.TabIndex = 189
        Me.Label3.Text = "起動パラメタ"
        '
        'Label75
        '
        Me.Label75.AutoSize = True
        Me.Label75.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label75.Location = New System.Drawing.Point(12, 40)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(63, 13)
        Me.Label75.TabIndex = 181
        Me.Label75.Text = "ジョブID"
        '
        'lblJobId
        '
        Me.lblJobId.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblJobId.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblJobId.Location = New System.Drawing.Point(132, 38)
        Me.lblJobId.Name = "lblJobId"
        Me.lblJobId.Size = New System.Drawing.Size(56, 18)
        Me.lblJobId.TabIndex = 183
        Me.lblJobId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTuuban
        '
        Me.lblTuuban.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTuuban.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTuuban.Location = New System.Drawing.Point(80, 38)
        Me.lblTuuban.Name = "lblTuuban"
        Me.lblTuuban.Size = New System.Drawing.Size(48, 18)
        Me.lblTuuban.TabIndex = 182
        Me.lblTuuban.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(5, 5)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(485, 30)
        Me.Label1.TabIndex = 191
        Me.Label1.Text = "ジョブ詳細情報"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'FrmSubJob
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 12)
        Me.ClientSize = New System.Drawing.Size(494, 136)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnDetailEnd)
        Me.Controls.Add(Me.btnReAction)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.txtPara)
        Me.Controls.Add(Me.txtErrMessage)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label75)
        Me.Controls.Add(Me.lblJobId)
        Me.Controls.Add(Me.lblTuuban)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Location = New System.Drawing.Point(100, 300)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmSubJob"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "KFUJOBW011"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Sub btnDetailEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDetailEnd.Click
        If mOwner.ListView1.CheckedItems.Count > 0 Then
            mOwner.ListView1.CheckedItems(0).Checked = False
        End If
        If mOwner.ListView2.CheckedItems.Count > 0 Then
            mOwner.ListView2.CheckedItems(0).Checked = False
        End If

        Me.Visible = False
    End Sub

    ' ジョブキャンセル
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '2011/05/25 ジョブキャンセル時ログ出力追加 ここから

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        ' 2016/02/05 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
        Dim INI_RSV2_EDITION As String = String.Empty
        ' 2016/02/05 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

        Try
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(ジョブキャンセル)開始", "成功")
            sw = mOwner.BatchLog.Write_Enter1("0000000000-00", "00000000", "ジョブキャンセル", "通番=" & mJobTuuban)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            '*** Str Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***
            SyncLock mThreadLock
                '*** End Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***

                '2011/05/25 ジョブキャンセル時ログ出力追加 ここまで
                ' 2016/02/05 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応) -------------------- START
                ' 大規模版機能時に、ジョブキャンセルを端末モードからでも可能とする
                'If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                '    MessageBox.Show(MSG0001W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '    Return
                'End If
                INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
                Select Case INI_RSV2_EDITION
                    Case "2"
                        ' NOP
                    Case Else
                        If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                            MessageBox.Show(MSG0001W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                End Select
                ' 2016/02/05 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応) -------------------- END

                If mStatus = "2" Then
                    MessageBox.Show(MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                '--------------------------------------
                'ｷｬﾝｾﾙの確認
                '--------------------------------------
                If MessageBox.Show(MSG0001I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                '--------------------------------------
                'exeの終了
                '--------------------------------------
                '*** Str Upd 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***
                'Dim ps() As System.Diagnostics.Process = System.Diagnostics.Process.GetProcesses()
                'Dim p As System.Diagnostics.Process
                'For Each p In ps
                '    'プロセス名がEXE名と一致するものを終了する
                '    If p.ProcessName = "KF" & lblJobId.Text Then
                '        p.Kill()
                '    End If
                'Next
                Dim mngcls As New System.Management.ManagementClass("Win32_Process")
                Dim mngcol As System.Management.ManagementObjectCollection = mngcls.GetInstances()
                Dim mngobj As System.Management.ManagementObject
                For Each mngobj In mngcol
                    If mngobj("Name").ToUpper = "KF" & lblJobId.Text.ToUpper & ".EXE" Then
                        Dim commandLine As String = mngobj("CommandLine")
                        Dim tuuban As String = commandLine.Substring(commandLine.LastIndexOf(",") + 1)

                        ' 同一ジョブ通番のプロセスを終了する
                        If CInt(tuuban) = mJobTuuban Then
                            Dim pid As Integer = CInt(mngobj("ProcessId"))
                            Dim proc As System.Diagnostics.Process = System.Diagnostics.Process.GetProcessById(pid)
                            If Not proc Is Nothing Then
                                proc.Kill

                                If mOwner.BatchLog.IS_LEVEL2() = True Then
                                    mOwner.BatchLog.Write_LEVEL2("ジョブキャンセル", "成功",  "tuuban=" & tuuban & " , pid=" & pid)
                                End If
                            End If

                            mngobj.Dispose()
                            Exit For
                        End If
                    End If
                    mngobj.Dispose()
                Next
                mngcol.Dispose()
                mngcls.Dispose()
                '*** End Upd 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***

                '--------------------------------------
                'ジョブマスタ（JOBMAST）の更新
                '--------------------------------------
                Dim SQL As New StringBuilder(128)
                SQL.Append("UPDATE JOBMAST SET STATUS_J='4'")
                SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append("   AND TUUBAN_J = " & mJobTuuban.ToString)
                If OraMain.ExecuteNonQuery(SQL) = 0 Then
                    MessageBox.Show(MSG0029E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraMain.Rollback()
                End If
                OraMain.Commit()

                Call mOwner.UpdateGamen(mJobTuuban)

                OraMain.BeginTrans()

                '*** Str Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***
            End SyncLock
            '*** End Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***

            '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(ジョブキャンセル)", "成功", "通番=" & mJobTuuban) '2011/05/25 ジョブキャンセル時ログ出力追加
            '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***
            Return
            '2011/05/25 ジョブキャンセル時ログ出力追加 ここから
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(ジョブキャンセル)", "失敗", ex.Message)
            mOwner.BatchLog.Write_Err("0000000000-00", "00000000", "ジョブキャンセル", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(ジョブキャンセル)終了", "成功")
            mOwner.BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "ジョブキャンセル", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try
        '2011/05/25 ジョブキャンセル時ログ出力追加 ここまで
    End Sub

    Private Sub btnReAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReAction.Click
        '2011/05/25 再実行時ログ出力追加 ここから

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        ' 2016/02/05 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
        Dim INI_RSV2_EDITION As String = String.Empty
        ' 2016/02/05 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

        Try
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(再実行)開始", "成功")
            sw = mOwner.BatchLog.Write_Enter1("0000000000-00", "00000000", "ジョブ再実行", "通番=" & mJobTuuban)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            '*** Str Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***
            SyncLock mThreadLock
                '*** End Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***

                '2011/05/25 再実行時ログ出力追加 ここまで
                ' 2016/02/05 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応) -------------------- START
                ' 端末モードでも再実行可能とするよう変更する
                '*** 修正 mitsu 2008/06/18 端末モードで再実行不可にする ***
                'If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                '    MessageBox.Show(MSG0002I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Question)
                '    Return
                'End If
                ''**********************************************************
                INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
                Select Case INI_RSV2_EDITION
                    Case "2"
                        ' NOP
                    Case Else
                        If Environment.MachineName.Trim <> mOwner.gastrBATCH_SV Then
                            MessageBox.Show(MSG0147W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Question)
                            Return
                        End If
                End Select
                ' 2016/02/05 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応) -------------------- START

                If mStatus = "1" Then
                    MessageBox.Show(MSG0003W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                '--------------------------------------
                '実行の確認
                '--------------------------------------
                If MessageBox.Show(MSG0002I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                '--------------------------------------
                'ジョブマスタ（JOBMAST）の更新
                '--------------------------------------
                Dim SQL As New StringBuilder(128)
                SQL.Append("UPDATE JOBMAST SET STATUS_J='0'")
                SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append("   AND TUUBAN_J = " & mJobTuuban.ToString)
                If OraMain.ExecuteNonQuery(SQL) = 0 Then
                    MessageBox.Show(MSG0028E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraMain.Rollback()
                End If
                OraMain.Commit()

                Call mOwner.UpdateGamen(mJobTuuban)

                OraMain.BeginTrans()

                '*** Str Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***
            End SyncLock
            '*** End Add 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***

            '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(再実行)", "成功", "通番=" & mJobTuuban) '2011/05/25 再実行時ログ出力追加
            '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***
            Return
            '2011/05/25 ジョブキャンセル時ログ出力追加 ここから
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(再実行)", "失敗", ex.Message)
            mOwner.BatchLog.Write_Err("0000000000-00", "00000000", "ジョブ再実行", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'mOwner.BatchLog.Write("0000000000-00", "00000000", "(再実行)終了", "成功")
            mOwner.BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "ジョブ再実行", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try
        '2011/05/25 ジョブキャンセル時ログ出力追加 ここまで
    End Sub

    Private Sub FrmSubJob_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.VisibleChanged
        If Me.Visible = False Then
            mOwner.Show()
            mOwner.Select()
        End If
    End Sub

    Private Sub FrmSubJob_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
