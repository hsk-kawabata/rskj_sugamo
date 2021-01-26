Option Strict On
Option Explicit On

Imports System.Data.OracleClient
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.FileSystem    '2012/06/30 標準版　Web伝送対応
'*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
' 2016/06/02 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応) -------------------- START
Imports System.Security.Permissions
' 2016/06/02 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応) -------------------- END

' ジョブ管理メインフォーム
Public Class FrmJOB
    Inherits System.Windows.Forms.Form

#Region "宣言"

    Public gastrBATCH_SV As String
    Dim gastrJOB_LOOPTIME As Integer
    Dim gastrJOB_MULTI As Integer

    Dim strPC_NAME As String   'コンピュータ名

    Private intJOB_COUNT As Integer 'ジョブの件数
    Private intOLD_JOB_COUNT As Integer = 0 'タイマー更新前のジョブ件数

    Private OraMain As New CASTCommon.MyOracle

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
    Private mLockWaitTime As Integer = 30
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
    Friend Shared mThreadLock As New Object   ' スレッド排他ロックオブジェクト
    Friend Shared mThreadUseTbl() As Integer  ' スレッド使用管理テーブル
    Friend Shared mThreadClsTbl() As Thread   ' スレッド実行クラス管理テーブル
    Friend Shared mStopFlg As Boolean = False  ' 終了フラグ
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

    Dim strERR_JOB As String
    Private strERR_COLOR As Color = Color.Red
    Private strBLACK_COLOR As Color = Color.Black
    Private strSYOUSAI_PARA As String

    Private mNowMaxTuuban As Integer = 0                ' 直近で取得したジョブ通番

    Private mVisibleFlag As Boolean = False

    ' 詳細情報フォーム
    Private FrmSub As FrmSubJob = Nothing

    ' 自動起動モジュール一覧
    Private AutoList As New ArrayList

    Private ConnecttedFlag As Boolean = False
    Private ConnectStopFlag As Boolean = False

    Private UpdateGamenFlag As Boolean = False

    'データベース接続フラグ
    Private blnDB_CONNECT As Boolean = True

    '返還処理の処理異常検知
    Private ELog As New CASTCommon.ClsEventLOG
    Private htErrJob As Hashtable = Hashtable.Synchronized(New Hashtable)   '同期化されたhashtable

    'ログクラス
    Public BatchLog As New CASTCommon.BatchLOG("KFUJOBW010", "ジョブ監視")

    '2012/06/30 標準版　Web伝送対応
    Private strFilePath As String   'Web伝送のファイル格納場所
    Private strFileBkupPath As String   'Web伝送のバックアップファイル格納場所
    Private strFileSendBkupPath As String   'Web伝送の送信バックアップファイル格納場所
    Private strFileName As String    'Web伝送のファイル名
    Private strPara As String       'ジョブ監視登録用
    Private SiyouFlg As String       'WEB伝送使用フラグ

    '2016/10/07 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- START
    Private Structure JobInfo
        Dim JOBID As String
        Dim JOBNAME As String
    End Structure
    Private TxtJob() As JobInfo
    '2016/10/07 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- END

    ' 2016/06/02 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応) -------------------- START
    ' 画面右上の「×」ボタンを使用不可に設定する
    Protected Overrides ReadOnly Property CreateParams As System.Windows.Forms.CreateParams
        <SecurityPermission(SecurityAction.Demand, flags:=SecurityPermissionFlag.UnmanagedCode)> _
        Get
            Const Cs_NoClose As Integer = &H200
            Dim Cp As CreateParams = MyBase.CreateParams
            Cp.ClassStyle = Cp.ClassStyle Or Cs_NoClose
            Return Cp
        End Get
    End Property
    ' 2016/06/02 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応) -------------------- END

#End Region

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

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
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblKanshi As System.Windows.Forms.Label
    Friend WithEvents tmrLoop As System.Windows.Forms.Timer
    Friend WithEvents lblNowTime As System.Windows.Forms.Label
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents TmrUpdate As System.Windows.Forms.Timer
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents BtnRefresh As System.Windows.Forms.Button
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents ListView2 As System.Windows.Forms.ListView
    Friend WithEvents TmrDisplay As System.Windows.Forms.Timer
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmJOB))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblNowTime = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.btnEnd = New System.Windows.Forms.Button()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblKanshi = New System.Windows.Forms.Label()
        Me.tmrLoop = New System.Windows.Forms.Timer(Me.components)
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.TmrUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.BtnRefresh = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.ListView2 = New System.Windows.Forms.ListView()
        Me.TmrDisplay = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(5, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(665, 30)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "口座振替バッチジョブ監視画面"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(505, 11)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "現在時刻"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblNowTime
        '
        Me.lblNowTime.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblNowTime.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblNowTime.Location = New System.Drawing.Point(597, 30)
        Me.lblNowTime.Name = "lblNowTime"
        Me.lblNowTime.Size = New System.Drawing.Size(70, 16)
        Me.lblNowTime.TabIndex = 2
        Me.lblNowTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(8, 64)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(357, 13)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "明細ダブルクリックでジョブの詳細情報が参照できます"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(550, 15)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(110, 35)
        Me.btnEnd.TabIndex = 2
        Me.btnEnd.TabStop = False
        Me.btnEnd.Text = "業務終了"
        '
        'lblDate
        '
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblDate.Location = New System.Drawing.Point(570, 12)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblDate.Size = New System.Drawing.Size(100, 16)
        Me.lblDate.TabIndex = 95
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblKanshi
        '
        Me.lblKanshi.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblKanshi.Location = New System.Drawing.Point(8, 40)
        Me.lblKanshi.Name = "lblKanshi"
        Me.lblKanshi.Size = New System.Drawing.Size(208, 16)
        Me.lblKanshi.TabIndex = 96
        '
        'tmrLoop
        '
        Me.tmrLoop.Interval = 5000
        '
        'ListView1
        '
        Me.ListView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.ListView1.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.ListView1.Location = New System.Drawing.Point(3, 15)
        Me.ListView1.MultiSelect = False
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(668, 260)
        Me.ListView1.TabIndex = 1
        Me.ListView1.TabStop = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'TmrUpdate
        '
        Me.TmrUpdate.Interval = 20000
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnEnd)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox1.Location = New System.Drawing.Point(0, 579)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(674, 56)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.BtnRefresh)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.lblDate)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Controls.Add(Me.lblNowTime)
        Me.GroupBox2.Controls.Add(Me.lblKanshi)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(674, 88)
        Me.GroupBox2.TabIndex = 99
        Me.GroupBox2.TabStop = False
        '
        'BtnRefresh
        '
        Me.BtnRefresh.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.BtnRefresh.Location = New System.Drawing.Point(567, 50)
        Me.BtnRefresh.Name = "BtnRefresh"
        Me.BtnRefresh.Size = New System.Drawing.Size(100, 32)
        Me.BtnRefresh.TabIndex = 1
        Me.BtnRefresh.Text = "最新表示(F5)"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.ListView1)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GroupBox3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(0, 88)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(674, 278)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "正常終了・起動待ち・処理中・キャンセル"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.ListView2)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox4.Location = New System.Drawing.Point(0, 379)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(674, 200)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "異常終了"
        '
        'ListView2
        '
        Me.ListView2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ListView2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView2.FullRowSelect = True
        Me.ListView2.GridLines = True
        Me.ListView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.ListView2.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.ListView2.Location = New System.Drawing.Point(3, 15)
        Me.ListView2.MultiSelect = False
        Me.ListView2.Name = "ListView2"
        Me.ListView2.Size = New System.Drawing.Size(668, 182)
        Me.ListView2.TabIndex = 2
        Me.ListView2.TabStop = False
        Me.ListView2.UseCompatibleStateImageBehavior = False
        Me.ListView2.View = System.Windows.Forms.View.Details
        '
        'TmrDisplay
        '
        Me.TmrDisplay.Enabled = True
        Me.TmrDisplay.Interval = 1000
        '
        'FrmJOB
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(674, 635)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.KeyPreview = True
        Me.Location = New System.Drawing.Point(10, 10)
        Me.MaximumSize = New System.Drawing.Size(1000, 660)
        Me.MinimumSize = New System.Drawing.Size(680, 660)
        Me.Name = "FrmJOB"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "KFUJOBW010"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "画面のロード"
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        lblDate.Text = CASTCommon.Calendar.Now.ToString("yyyy年MM月dd日")
        '------------------------------------------
        'INIファイルの読み込み
        '------------------------------------------
        If fn_INI_READ() = False Then
            Exit Sub
        End If

        ' 2016/06/02 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応) -------------------- START
        BatchLog.Write("0000000000-00", "00000000", "業務開始", "")
        ' 2016/06/02 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応) -------------------- END

        '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- START
        'テキストファイルよりジョブ名を取得する
        getTxtToJobName()
        '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- END

        '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
        'threadMethodDelegateInstance = CType(Array.CreateInstance(GetType(ThreadMethodDelegate), gastrJOB_MULTI), ThreadMethodDelegate())
        mThreadUseTbl = New Integer(gastrJOB_MULTI - 1) {}
        mThreadClsTbl = New Thread(gastrJOB_MULTI - 1) {}
        '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***

        '------------------------------------------
        'コンピュータ名の取得
        '------------------------------------------
        strPC_NAME = Environment.MachineName

        If gastrJOB_LOOPTIME = 0 Then
            gastrJOB_LOOPTIME = 5000
        End If

        ' 初期画面設定処理
        Call StartGamen()

        Me.AddOwnedForm(FrmSub)


        '端末モードの場合は自動起動しない
        If strPC_NAME.Trim <> gastrBATCH_SV Then
        ElseIf TodayIsHoliday() = False Then
            Try
                '------------------------------------------
                '自動起動モジュール 設定
                '------------------------------------------
                Dim Keys() As String = CASTCommon.GetFSKJIniKeys("AUTO")
                For i As Integer = 0 To Keys.Length - 1
                    Dim AutoKey As String() = Keys(i).Split("="c)

                    Dim TMSpan As New TimeSpan(0, 15, 0)
                    Dim TMNow As New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CASTCommon.CAInt32(AutoKey(0).Substring(0, 2)), CASTCommon.CAInt32(AutoKey(0).Substring(2)), 0)
                    Dim TMAdd As DateTime = TMNow.Add(TMSpan)
                    Dim TMMinus As DateTime = TMNow.Subtract(TMSpan)

                    AutoList.Add(Keys(i) & "=" & TMNow.ToString("HHmm") & "=" & TMAdd.ToString("HHmm"))
                Next i
                TmrUpdate.Enabled = True
                TmrUpdate.Start()

            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("Form1_Load", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            End Try

        End If

        '------------------------------------------
        'タイマーの実行
        '------------------------------------------
        tmrLoop.Interval = gastrJOB_LOOPTIME
        tmrLoop.Start()

        '接続失敗時はタイマー停止
        If blnDB_CONNECT = False Then
            Try
                tmrLoop.Stop()
                TmrDisplay.Stop()
                TmrUpdate.Stop()

            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("Form1_Load", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                Console.WriteLine(ex.Message)
            End Try
        End If
    End Sub

    '*** Str Add 2015/12/01 SO)荒木 for 不要DBコネクション削除 ***
    Private Sub FrmJOB_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        Try
            SyncLock mThreadLock
                mStopFlg = True  ' スレッド側での例外ログ抑止のために停止フラグをＯＮにする

                For idx As Integer = 0 To mThreadClsTbl.Length - 1
                    ' スレッドクラス管理テーブルのスレッドを停止する
                    If Not mThreadClsTbl(idx) Is Nothing Then
                        mThreadClsTbl(idx).Abort()
                        'mThreadClsTbl(idx).Join()  ' 待つと終了が遅くなるので待たない

                        If BatchLog.IS_LEVEL2() = True Then
                            BatchLog.Write_LEVEL2("スレッド停止", "成功", "idx=" & idx)
                        End If
                    End If
                Next
            End SyncLock
        Catch ex As Exception
            BatchLog.Write_Err("FrmJOB_FormClosed", "失敗", ex)
        End Try

    End Sub
    '*** End Add 2015/12/01 SO)荒木 for 不要DBコネクション削除 ***

#End Region

#Region "タイマー"
    Private Sub tmrLoop_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrLoop.Tick

        tmrLoop.Enabled = False

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

            '--------------------------------------
            '画面のシステム時間の表示
            '--------------------------------------
            lblDate.Text = CASTCommon.Calendar.Now.ToString("yyyy年MM月dd日")
            lblNowTime.Text = CASTCommon.Calendar.Now.ToString("HH:mm:ss")

            Call fn_SET_GAMEN()

            Me.ResumeLayout(False)

            '--------------------------------------
            'ジョブの発行
            '--------------------------------------
            fn_CREATE_JOB()

            If strPC_NAME.Trim <> gastrBATCH_SV Then
                lblKanshi.Text = "実行状況更新中（端末モード）"
            Else
                lblKanshi.Text = "実行状況更新中"
            End If

            '2012/06/30 標準版　Web伝送対応----->
            If SiyouFlg <> "0" Then
                fn_File_Watch()
            End If
            '-----------------------------------<

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        End SyncLock
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

        tmrLoop.Enabled = True

    End Sub
#End Region

#Region "関数"
    Function fn_SET_GAMEN() As Boolean
        '=====================================================================================
        'NAME           :fn_SET_GAMEN
        'Parameter      :aintPAGE_NO：ページ番号
        'Description    :aintPAGE_NOページの画面表示
        'Return         :True=OK(正常終了),False=NG（失敗）
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        fn_SET_GAMEN = False

        If ListView1.Columns.Count = 0 Then
            ListView1.CheckBoxes = True
            With Me.ListView1
                .Clear()
                .Columns.Add("", 22, HorizontalAlignment.Left)
                .Columns.Add("実行状況", 80, HorizontalAlignment.Center)
                .Columns.Add("通番", 50, HorizontalAlignment.Right)
                .Columns.Add("  処理業務", 90, HorizontalAlignment.Left)
                .Columns.Add("開始時間", 70, HorizontalAlignment.Center)
                .Columns.Add("終了時間", 70, HorizontalAlignment.Center)
                .Columns.Add("取引先コード・委託者名", 190, HorizontalAlignment.Left)
                .Columns.Add("備考", 500, HorizontalAlignment.Left)
                .Columns.Add("ｼﾞｮﾌﾞID", 70, HorizontalAlignment.Left)
                .Columns.Add("パラメータ", 90, HorizontalAlignment.Left)
                .Columns.Add("STS", 30, HorizontalAlignment.Left)
            End With
        End If

        If ListView2.Columns.Count = 0 Then
            ListView2.CheckBoxes = True
            With Me.ListView2
                .Clear()
                .Columns.Add("", 22, HorizontalAlignment.Left)
                .Columns.Add("実行状況", 80, HorizontalAlignment.Center)
                .Columns.Add("通番", 50, HorizontalAlignment.Right)
                .Columns.Add("  処理業務", 90, HorizontalAlignment.Left)
                .Columns.Add("開始時間", 70, HorizontalAlignment.Center)
                .Columns.Add("終了時間", 70, HorizontalAlignment.Center)
                .Columns.Add("取引先コード・委託者名", 190, HorizontalAlignment.Left)
                .Columns.Add("備考", 500, HorizontalAlignment.Left)
                .Columns.Add("ｼﾞｮﾌﾞID", 70, HorizontalAlignment.Left)
                .Columns.Add("パラメータ", 90, HorizontalAlignment.Left)
                .Columns.Add("STS", 30, HorizontalAlignment.Left)
            End With
        End If

        ' DB接続確認
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        'ジョブの順番を降順に
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraTori As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Try
            OraReader = New CASTCommon.MyOracleReader(OraMain)
            ConnecttedFlag = True

            SQL.Append("SELECT * FROM JOBMAST")
            SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
            SQL.Append("   AND TUUBAN_J > " & mNowMaxTuuban.ToString)
            SQL.Append(" ORDER  BY TUUBAN_J ASC")

            Dim FrontColor As Color
            Dim LineColor As Color
            Dim ROW As Integer = 0
            If OraReader.DataReader(SQL) = True Then
                Do While OraReader.EOF = False
                    ROW += 1

                    Dim Data(10) As String
                    Data(1) = fn_YOMIKAE_JYOUKYO(OraReader.GetString("STATUS_J"), FrontColor)
                    mNowMaxTuuban = OraReader.GetInt("TUUBAN_J")
                    Data(2) = mNowMaxTuuban.ToString
                    Data(3) = fn_SET_JOBNAME(OraReader.GetString("JOBID_J").PadRight(1))
                    Data(4) = OraReader.GetString("STA_TIME_J").PadRight(6, "0"c).Insert(2, ":").Insert(5, ":")
                    Data(5) = OraReader.GetString("END_TIME_J").PadRight(6, "0"c).Insert(2, ":").Insert(5, ":")
                    Data(7) = OraReader.GetString("ERRMSG_J")
                    Data(8) = OraReader.GetString("JOBID_J")
                    Data(9) = OraReader.GetString("PARAMETA_J")
                    Data(10) = OraReader.GetString("STATUS_J")

                    Dim Param() As String = OraReader.GetString("PARAMETA_J").Split(","c)
                    If Param.Length >= 2 Then
                        '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        'Dim OraTori As New CASTCommon.MyOracleReader(OraMain)
                        OraTori = New CASTCommon.MyOracleReader(OraMain)
                        '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        SQL.Length = 0
                        SQL.Append("SELECT TORIS_CODE_T || '-' || TORIF_CODE_T TORI_CODE")
                        SQL.Append(", ITAKU_NNAME_T")

                        'ジョブIDで取得するマスタを判定する
                        Select Case Data(8).PadRight(1).Substring(0, 1)
                            Case "S" : SQL.Append(" FROM S_TORIMAST")
                            Case Else : SQL.Append(" FROM TORIMAST")
                        End Select

                        SQL.Append(" WHERE TORIS_CODE_T = " & CASTCommon.SQ(Param(0).PadRight(12).Substring(0, 10)))
                        SQL.Append(" AND TORIF_CODE_T = " & CASTCommon.SQ(Param(0).PadRight(12).Substring(10)))
                        If OraTori.DataReader(SQL) = True Then
                            Data(6) = OraTori.GetString("TORI_CODE") & " " & OraTori.GetString("ITAKU_NNAME_T")
                        End If
                        OraTori.Close()
                    End If

                    Dim StatusJ As String = OraReader.GetString("STATUS_J")
                    '正常終了とキャンセルのみを上段に表記する。起動待ちと処理中は下段に表記する。
                    If StatusJ = "0" OrElse StatusJ = "1" OrElse StatusJ = "2" OrElse StatusJ = "4" Then
                        If ListView1.Items.Count Mod 2 = 0 Then
                            LineColor = Color.White
                        Else
                            LineColor = Color.LightGoldenrodYellow
                        End If

                        Dim vLstItem As New ListViewItem(Data, -1, FrontColor, LineColor, Nothing)
                        ListView1.Items.Insert(0, vLstItem)
                    Else
                        If ListView2.Items.Count Mod 2 = 0 Then
                            LineColor = Color.White
                        Else
                            LineColor = Color.PaleGoldenrod
                        End If

                        Dim vLstItem As New ListViewItem(Data, -1, FrontColor, LineColor, Nothing)
                        ListView2.Items.Insert(0, vLstItem)
                    End If

                    OraReader.NextRead()
                Loop
            End If
            OraReader.Close()
        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("fn_SET_GAMEN", "失敗", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Console.WriteLine(ex.Message)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraTori Is Nothing Then
                OraTori.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        End Try

        fn_SET_GAMEN = True
        Exit Function
    End Function

    Function fn_YOMIKAE_JYOUKYO(ByVal astrSTATUS As String, ByRef astrJYOUKYOU_COLOR As Color) As String
        '=====================================================================================
        'NAME           :fn_YOMIKAE_JYOUKYO
        'Parameter      :astrSTATUS：JOBMAST.STATUS_Jの値／astrJYOUKYOU_COLOR：ラベルの文字の色
        'Description    :astrSTATUSを読み替える
        'Return         :読み替えた値
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        Select Case astrSTATUS
            Case "0"
                fn_YOMIKAE_JYOUKYO = "起動待ち"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "1"
                fn_YOMIKAE_JYOUKYO = "処理中"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "2"
                fn_YOMIKAE_JYOUKYO = "正常終了"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "3"
                '2011/05/25 異常終了を処理中表示にする(プロセスが起動している状態)
                'fn_YOMIKAE_JYOUKYO = "異常終了"
                fn_YOMIKAE_JYOUKYO = "処理中"
                astrJYOUKYOU_COLOR = strERR_COLOR
            Case "4"
                fn_YOMIKAE_JYOUKYO = "ｷｬﾝｾﾙ"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "7"
                '2011/05/25 起動失敗を異常終了表示にする(プロセスが終了している状態)
                'fn_YOMIKAE_JYOUKYO = "起動失敗"
                fn_YOMIKAE_JYOUKYO = "異常終了"
                astrJYOUKYOU_COLOR = strERR_COLOR
            Case "8"
                fn_YOMIKAE_JYOUKYO = "ABEND"
                astrJYOUKYOU_COLOR = strERR_COLOR
            Case Else
                fn_YOMIKAE_JYOUKYO = "その他"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
        End Select
    End Function

    Function fn_CHANGE_TIME(ByVal astrTIME As String) As String
        '=====================================================================================
        'NAME           :fn_CHANGE_TIME
        'Parameter      :astrTIME：時間（6桁）
        'Description    :astrTIMEを"："区切りに変換
        'Return         :astrTIMEを"："区切りに変換した値
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        fn_CHANGE_TIME = astrTIME.Substring(0, 2) & ":" & astrTIME.Substring(2, 2) & ":" & astrTIME.Substring(4, 2)
    End Function

    Function fn_INI_READ() As Boolean
        '============================================================================
        'NAME           :fn_INI_READ
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/10/05
        'Update         :
        '============================================================================
        fn_INI_READ = False

        gastrBATCH_SV = CASTCommon.GetFSKJIni("COMMON", "BATCHSV")
        If gastrBATCH_SV = "err" OrElse gastrBATCH_SV = "" Then
            MessageBox.Show(String.Format(MSG0001E, "バッチサーバ名", "COMMON", "BATCHSV"), _
                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        gastrJOB_LOOPTIME = CASTCommon.CAInt32(CASTCommon.GetFSKJIni("COMMON", "KANSHITIME"))
        If gastrBATCH_SV = "err" OrElse gastrBATCH_SV = "" Then
            MessageBox.Show(String.Format(MSG0001E, "監視ループ時間", "COMMON", "KANSHITIME"), _
                      msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        gastrJOB_MULTI = CASTCommon.CAInt32(CASTCommon.GetFSKJIni("COMMON", "JOBMULTI"))
        '*** Str Upd 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***
        'If gastrJOB_MULTI = 0 Then
        If gastrJOB_MULTI <= 0 Or gastrJOB_MULTI > 100 Then
            '*** End Upd 2015/12/01 SO)荒木 for ジョブ多重実行対応 ***
            MessageBox.Show(String.Format(MSG0001E, "ジョブ多重度", "COMMON", "JOBMULTI"), _
                   msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ExePath = CASTCommon.GetFSKJIni("COMMON", "EXE")
        If ExePath = "err" OrElse ExePath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " 実行フォルダ", "COMMON", "EXE"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '2012/06/30 標準版　Web伝送対応----------------------------------------------------------->
        strFilePath = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV")
        If strFilePath = "err" OrElse strFilePath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " WEB伝送受信フォルダ", "WEB_DEN", "WEB_REV"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        strFileBkupPath = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV_BK")
        If strFileBkupPath = "err" OrElse strFileBkupPath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " WEB伝送受信バックアップフォルダ", "WEB_DEN", "WEB_REV_BK"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        strFileSendBkupPath = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_SED_BK")
        If strFileSendBkupPath = "err" OrElse strFileSendBkupPath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " WEB伝送送信バックアップフォルダ", "WEB_DEN", "WEB_SED_BK"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        SiyouFlg = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_DENSO")
        If SiyouFlg = "err" OrElse SiyouFlg = "" Then
            MessageBox.Show(String.Format(MSG0001E, "WEB伝送使用フラグ", "WEB_DEN", "WEB_DENSO"), _
                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        '---------------------------------------------------------------------------------------------<

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            mLockWaitTime = CInt(sWork)
            If mLockWaitTime <= 0 Then
                mLockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

        fn_INI_READ = True
    End Function
    Function fn_CREATE_JOB() As Boolean
        '=====================================================================================
        'NAME           :fn_CREATE_JOB
        'Parameter      :
        'Description    :JOBMASTを検索し、起動待ちのジョブを発行する
        'Return         :True=OK(正常終了),False=NG（失敗）
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        fn_CREATE_JOB = False

        If strPC_NAME.Trim <> gastrBATCH_SV Then
            ' 端末モード
            tmrLoop.Enabled = False

            Call UpdateDisplayOnly()

            tmrLoop.Enabled = True
            Return True
        End If

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        ' DB接続確認
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        Try
            tmrLoop.Enabled = False

            Dim intMISYORI_COUNT As Integer = 0

            OraReader = New CASTCommon.MyOracleReader(OraMain)

            Dim SQL As String

            ' ----
            Dim SYORITYU_COUNT As Integer = 0
            SQL = "SELECT COUNT(*) CNT FROM JOBMAST"
            SQL &= " WHERE (STATUS_J = '1')"
            SQL &= "   AND TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')"

            If OraReader.DataReader(SQL) = True Then
                SYORITYU_COUNT = OraReader.GetInt("CNT")
            End If
            OraReader.Close()
            ' ----

            SQL = "SELECT * FROM JOBMAST"
            SQL &= " WHERE (STATUS_J = '0')"
            SQL &= "   AND TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')"
            SQL &= " ORDER BY STATUS_J DESC, TUUBAN_J ASC"

            intMISYORI_COUNT = SYORITYU_COUNT

            If OraReader.DataReader(SQL) = True Then
                '読込のみ
                While (OraReader.EOF = False)
                    intMISYORI_COUNT += 1

                    If intMISYORI_COUNT > gastrJOB_MULTI Then
                        Exit While
                    End If

                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
                    ' スレッド使用管理テーブルの空きインデックス番号を取得
                    Dim threadTblIdx As Integer = getFreeThreadTblIdx()
                    ' 空きが無い場合
                    If threadTblIdx = -1 Then
                        Exit While
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***

                    '--------------------------------------
                    'ジョブマスタ（JOBMAST）の更新
                    '--------------------------------------
                    Dim ExeName As String
                    Try
                        strERR_JOB = " "
                        If fn_JOBMAST_UPDATE("1", strERR_JOB, OraReader.GetInt("TUUBAN_J")) = False Then
                            Exit Function
                        End If

                        ExeName = "KF" & OraReader.GetItem("JOBID_J").TrimEnd & ".EXE"
                        If OraReader.GetItem("JOBID_J").Trim = "" Then
                            ExeName = "実行ファイル"
                        End If
                        If File.Exists(Path.Combine(ExePath, ExeName)) = False Then
                            If fn_JOBMAST_UPDATE("7", ExeName & "なし", OraReader.GetInt("TUUBAN_J")) = False Then
                                Exit Function
                            End If

                            intMISYORI_COUNT -= 1
                        Else
                            ' 実行
                            Dim Para As String = OraReader.GetItem("PARAMETA_J")
                            '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
                            'If JobExecute(CAInt32(OraReader.GetItem("TUUBAN_J")), ExeName, Para) = False Then
                            '    ' スレッドに空きがなかったので，あとからもう一度実行する
                            '    Call fn_JOBMAST_UPDATE("0", "リトライ", OraReader.GetInt("TUUBAN_J"))
                            '    Exit Function
                            'End If
                            JobExecute(threadTblIdx, CAInt32(OraReader.GetItem("TUUBAN_J")), ExeName, Para)
                            '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
                        End If
                    Catch ex As Exception
                        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                        BatchLog.Write_Err("fn_CREATE_JOB", "失敗", ex)
                        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                        Call fn_JOBMAST_UPDATE("0", "リトライ", OraReader.GetInt("TUUBAN_J"))
                        Exit Function
                    End Try

                    OraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("fn_CREATE_JOB", "失敗", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            tmrLoop.Enabled = True
        End Try

        fn_CREATE_JOB = True
    End Function
    Public Function fn_JOBMAST_UPDATE(ByVal astrSTATUS As String, ByVal astrERRMSG As String, ByVal intTUUBAN As Integer) As Boolean
        '============================================================================
        'NAME           :fn_JOBMAST_UPDATE
        'Parameter      :astrSTATUS：ステータス（0=起動待ち、1=処理中）／astrERRMSG：エラーメッセージ
        '               :intTUUBAN：通番
        'Description    :JOBMASTに結果更新
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/10/05
        'Update         :
        '============================================================================
        fn_JOBMAST_UPDATE = False

        Dim SQL As String
        SQL = "UPDATE JOBMAST SET "
        SQL = SQL & "STA_DATE_J ='" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & "',STA_TIME_J ='" & CASTCommon.Calendar.Now.ToString("HHmmss") & "',"
        SQL = SQL & "STATUS_J ='" & astrSTATUS & "',ERRMSG_J = '" & astrERRMSG & "' "
        SQL = SQL & "WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')"
        SQL = SQL & "  AND TUUBAN_J = " & intTUUBAN & ""

        ' DB接続確認
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        Try
            OraMain.ExecuteNonQuery(SQL)
            OraMain.Commit()
            OraMain.BeginTrans()
        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("fn_JOBMAST_UPDATE", "失敗", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            OraMain.Rollback()
            OraMain.BeginTrans()
            MessageBox.Show(MSG0014E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End Try

        fn_JOBMAST_UPDATE = True
    End Function

    '*** Str Del 2015/12/01 SO)荒木 for 未使用メソッド削除 ***
    '    Public Function fn_JOBMAST_UPDATE_END(ByVal astrSTATUS As String, ByVal astrERRMSG As String, ByVal intTUUBAN As Integer) As Boolean
    '        '============================================================================
    '        'NAME           :fn_JOBMAST_UPDATE_END
    '        'Parameter      :astrSTATUS：ステータス（0=起動待ち、1=処理中）／astrERRMSG：エラーメッセージ
    '        '               :intTUUBAN：通番
    '        'Description    :JOBMASTに結果更新
    '        'Return         :True=OK(成功),False=NG（失敗）
    '        'Create         :2004/10/05
    '        'Update         :
    '        '============================================================================
    '        fn_JOBMAST_UPDATE_END = False
    '
    '        Dim SQL As String
    '        SQL = "UPDATE JOBMAST SET "
    '        SQL = SQL & "END_DATE_J ='" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & "',END_TIME_J ='" & CASTCommon.Calendar.Now.ToString("HHmmss") & "',"
    '        SQL = SQL & "STATUS_J ='" & astrSTATUS & "',ERRMSG_J = '" & astrERRMSG & "' "
    '        SQL = SQL & "WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')"
    '        SQL = SQL & "  AND TUUBAN_J = " & intTUUBAN & ""
    '
    '        ' DB接続確認
    '        Do
    '            DBConnectJudge()
    '        Loop Until ConnectStopFlag = False
    '
    '        Try
    '            OraMain.ExecuteNonQuery(SQL)
    '            OraMain.Commit()
    '            OraMain.BeginTrans()
    '        Catch ex As Exception
    '            OraMain.Rollback()
    '            OraMain.BeginTrans()
    '            MessageBox.Show(MSG0014E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '            Exit Function
    '        End Try
    '
    '        fn_JOBMAST_UPDATE_END = True
    '    End Function
    '*** End Del 2015/12/01 SO)荒木 for 未使用メソッド削除 ***

    Function fn_SET_JOBNAME(ByVal astrJOB_ID As String) As String
        '============================================================================
        'NAME           :fn_SET_JOBNAME
        'Parameter      :astrJOB_ID：ジョブＩＤ
        'Description    :ジョブＩＤをジョブ名に変換
        'Return         :ジョブ名
        'Create         :2004/10/06
        'Update         :
        '============================================================================

        fn_SET_JOBNAME = ""

        '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- START
        If Not TxtJob Is Nothing AndAlso TxtJob.Length > 0 Then
            For i As Integer = 0 To TxtJob.Length - 1
                If astrJOB_ID = TxtJob(i).JOBID Then
                    Return TxtJob(i).JOBNAME.Trim
                End If
            Next
        End If
        '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- END

        Select Case astrJOB_ID.Substring(0, 1)

            Case "J"    '自振
                Select Case astrJOB_ID.TrimEnd
                    Case "J010"
                        fn_SET_JOBNAME = "口振落込"
                    Case "J011"
                        fn_SET_JOBNAME = "ｾﾝﾀｰ直接落込"
                    Case "J020"
                        fn_SET_JOBNAME = "他行"
                    Case "J021"
                        fn_SET_JOBNAME = "他行"
                    Case "J030"
                        fn_SET_JOBNAME = "配信"
                    Case "J040"
                        fn_SET_JOBNAME = "不能"
                    Case "J050"
                        fn_SET_JOBNAME = "日報"
                    Case "J060"
                        fn_SET_JOBNAME = "返還"
                    Case "J070"
                        fn_SET_JOBNAME = "手計"
                    Case "J080"
                        fn_SET_JOBNAME = "再振"
                    Case "J090"
                        fn_SET_JOBNAME = "自振契約"
                    Case "J100"
                        fn_SET_JOBNAME = "自振契約結果"
                    Case Else
                        fn_SET_JOBNAME = "口振その他"
                End Select

            Case "S"    '総振
                Select Case astrJOB_ID.TrimEnd
                    Case "S010"
                        fn_SET_JOBNAME = "総振落込"
                    Case "S020"
                        fn_SET_JOBNAME = "資金確保"
                    Case "S030"
                        fn_SET_JOBNAME = "資金確保結果"
                        '2016/01/11 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- START
                    Case "S040", "S045"
                        'Case "S040"
                        '2016/01/11 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- END
                        fn_SET_JOBNAME = "発信"
                    Case "S050"
                        fn_SET_JOBNAME = "発信結果"
                    Case "S060"
                        fn_SET_JOBNAME = "為替請求"
                    Case "S070"
                        fn_SET_JOBNAME = "為替請求結果"
                    Case Else
                        fn_SET_JOBNAME = "総振その他"
                End Select

            Case "G"    '学校
                Select Case astrJOB_ID.TrimEnd
                    Case "S010"
                        fn_SET_JOBNAME = "学校落込"
                    Case Else
                        fn_SET_JOBNAME = "学校その他"
                End Select

            Case "K"    '決済
                Select Case astrJOB_ID.TrimEnd
                    Case "K010"
                        fn_SET_JOBNAME = "資金決済"
                    Case "K020"
                        fn_SET_JOBNAME = "資金決済結果"
                    Case Else
                        fn_SET_JOBNAME = "決済その他"
                End Select

            Case "U"    '運用管理
                Select Case astrJOB_ID.TrimEnd
                    Case "U010"
                        fn_SET_JOBNAME = "金融機関更新"
                    Case Else
                        fn_SET_JOBNAME = "運管その他"
                End Select

            Case "T"    '他シス連携
                Select Case astrJOB_ID.TrimEnd
                    Case "T010"
                        fn_SET_JOBNAME = "他シス連携"
                End Select

                '2012/06/30 標準版　WEB伝送対応----->
            Case "W"    'WEB伝送連携
                Select Case astrJOB_ID.TrimEnd
                    Case "W010"
                        fn_SET_JOBNAME = "WEB伝送連携"
                End Select
                '-----------------------------------<
            Case Else
                ' 2016/09/28 タスク）西野 CHG 【PG】青梅信金 カスタマイズ対応(UI_12-10) -------------------- START
                Dim RetJobName As String = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "ジョブ名管理.TXT"), astrJOB_ID.TrimEnd)
                If RetJobName.Trim = "" Then
                    fn_SET_JOBNAME = "不明"
                Else
                    fn_SET_JOBNAME = RetJobName
                End If
                'fn_SET_JOBNAME = "不明"
                ' 2016/09/28 タスク）西野 CHG 【PG】青梅信金 カスタマイズ対応(UI_12-10) -------------------- END
        End Select

    End Function
#End Region

#Region "Web伝送対応"
    'ファイル監視
    Private Function fn_File_Watch() As Boolean
        '============================================================================
        'NAME           :fn_File_Watch
        'Parameter      :
        'Description    :ファイル監視処理（Web伝送対応）
        'Return         :True=OK(正常終了),False=NG（失敗）
        'Create         :2012/06/30
        'Update         :
        '============================================================================
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        '端末モード
        If strPC_NAME.Trim <> gastrBATCH_SV Then
            tmrLoop.Enabled = False
            Call UpdateDisplayOnly()
            tmrLoop.Enabled = True
            Return True
        End If

        Try
            '--------------------------------------
            'Web伝送ファイル監視
            '--------------------------------------
            If Dir(strFilePath & "END" & "*") <> "" Then
                '他シス連携処理登録
                Dim FileName As String
                Dim ret As String = ""
                Dim sr As StreamReader = Nothing

                Try
                    '監視フォルダ内のファイル名を取得
                    FileName = Dir(strFilePath & "END_" & "*")

                    '２重起動防止のため、真っ先にENDファイルを移動
                    If File.Exists(strFileBkupPath & FileName) Then
                        File.Delete(strFileBkupPath & FileName)
                    End If

                    File.Move(strFilePath & FileName, strFileBkupPath & FileName)

                    '2012/12/05 saitou WEB伝送 UPD -------------------------------------------------->>>>
                    'ENDファイルから件数と金額を取得する。
                    Dim strEndKen As String = String.Empty
                    Dim strEndKin As String = String.Empty
                    If File.Exists(Path.Combine(strFileBkupPath, FileName)) = True Then
                        'ENDファイルが存在する場合、ファイルを開いて件数と金額を取得する。
                        sr = New StreamReader(Path.Combine(strFileBkupPath, FileName), Encoding.GetEncoding(932))
                        Dim ReadLine As String = String.Empty

                        While sr.EndOfStream = False
                            ReadLine = sr.ReadLine
                            If Not ReadLine Is Nothing Then
                                Dim Item As String() = ReadLine.Split(","c)

                                If Item.Length = 2 Then
                                    strEndKen = Item(0)
                                    strEndKin = Item(1)
                                End If
                            End If
                        End While

                        sr.Close()
                        sr = Nothing
                    End If
                    '2012/12/05 saitou WEB伝送 UPD --------------------------------------------------<<<<

                    'FileArray(0) = ユーザ名
                    'FileArray(1) = 元ファイル名
                    'FileArray(2) = 配信日
                    'FileArray(3) = 配信時間
                    Dim FileArray() As String = FileName.Substring(4).Split("_"c)
                    Dim FileExt As String = Path.GetExtension(FileArray(1))

                    If FileExt <> "" Then   '拡張子がない場合は、処理しない

                        Dim GCOM As New MenteCommon.clsCommon
                        Dim TXTFileName As String = Path.Combine(GCOM.GetTXTFolder, "WEB伝送ファイル管理.txt")

                        sr = New StreamReader(TXTFileName, Encoding.GetEncoding(932))
                        Dim ReadLine As String = ""

                        While sr.EndOfStream = False
                            ReadLine = sr.ReadLine()
                            If Not ReadLine Is Nothing Then
                                Dim Item As String() = ReadLine.Split(","c)

                                If FileExt.Substring(1).ToUpper = Item(0).ToUpper Then
                                    ret = Item(1)
                                    'ファイル移動
                                    File.Copy(strFilePath & FileName.Substring(4), Item(1) & FileArray(0) & "_" & FileArray(2) & "_" & FileArray(3) & "_" & FileArray(1), True)
                                    File.Delete(strFilePath & FileName.Substring(4))
                                    Exit While
                                End If
                            End If
                        End While

                        sr.Close()
                        sr = Nothing
                    End If

                    If ret = "" Then

                        '20130607 ファイル名空白混在対応
                        '2012/12/05 saitou WEB伝送 UPD -------------------------------------------------->>>>
                        '実行パラメータに件数と金額を追加する。
                        strPara = """" & strFilePath & FileName.Substring(4) & """"
                        strPara &= "," & strEndKen & "," & strEndKin
                        'strPara = strFilePath & FileName.Substring(4) & "," & strEndKen & "," & strEndKin
                        'strPara = strFilePath & FileName.Substring(4)
                        '2012/12/05 saitou WEB伝送 UPD --------------------------------------------------<<<<
                        '20130607 ファイル名空白混在対応

                        Thread.Sleep(3000)

                        '2016/12/06 saitou RSV2 UPD メンテナンス ---------------------------------------- START
                        'パス固定を廃止し、EXEパスを設定ファイルから取得するように修正
                        Dim ExePath As String = CASTCommon.GetFSKJIni("COMMON", "EXE")
                        If ExePath = String.Empty OrElse ExePath = Nothing Then
                            '取得できなかった場合は今まで通りの設定
                            ExePath = "C:\RSKJ\EXE\"
                        End If
                        Dim ExecProc As Process = Process.Start(Path.Combine(ExePath, "KFW010.EXE"), strPara)
                        'Dim ExecProc As Process = Process.Start("C:\RSKJ\EXE\KFW010.EXE", strPara)
                        '2016/12/06 saitou RSV2 UPD ----------------------------------------------------- END

                        If ExecProc.Id <> 0 Then
                            '終了待機
                            ExecProc.WaitForExit()
                        Else
                            Throw New Exception(String.Format("アプリケーションの起動に失敗しました。'{0}'", "KFW010.EXE"))
                        End If
                        ExecProc.Close()
                        ExecProc.Dispose()

                    End If

                Catch ex As Exception
                    '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
                    'BatchLog.Write("Web伝送ファイル監視", "失敗", ex.Message)
                    BatchLog.Write_Err("Web伝送ファイル監視", "失敗", ex)
                    '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
                    MessageBox.Show("WEB伝送ファイルの処理に失敗しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Finally
                    If Not sr Is Nothing Then
                        sr.Close()
                        sr = Nothing
                    End If
                End Try
            End If

            '--------------------------------------
            'Web伝送送信ファイル監視
            '--------------------------------------

            Dim SQL As New StringBuilder(128)
            Dim Send_FileName As String
            OraReader = New CASTCommon.MyOracleReader(OraMain)

            SQL.Append(" SELECT * ")
            SQL.Append(" FROM WEB_RIREKIMAST ")
            SQL.Append(" WHERE FSYORI_KBN_W = '1' ")
            SQL.Append(" AND STATUS_KBN_W = '2' ")

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    Send_FileName = "END_" & OraReader.GetString("USER_ID_W") & "_" & OraReader.GetString("FILE_NAME_W") & "_" & OraReader.GetString("FURI_DATE_W")

                    If File.Exists(strFileSendBkupPath & Send_FileName) Then
                        SQL = New StringBuilder(128)
                        SQL.Append("UPDATE WEB_RIREKIMAST SET STATUS_KBN_W = '3' ")
                        SQL.Append(",SAKUSEI_DATE_W = '" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & "'")
                        SQL.Append(",SAKUSEI_TIME_W = '" & CASTCommon.Calendar.Now.ToString("HHmmss") & "'")
                        SQL.Append(" WHERE FURI_DATE_W = '" & OraReader.GetString("FURI_DATE_W") & "'")
                        SQL.Append("   AND TORIS_CODE_W = '" & OraReader.GetString("TORIS_CODE_W") & "'")
                        SQL.Append("   AND TORIF_CODE_W = '" & OraReader.GetString("TORIF_CODE_W") & "'")
                        SQL.Append("   AND STATUS_KBN_W = '2'")
                        SQL.Append("   AND FSYORI_KBN_W = '1' ")

                        Call OraMain.ExecuteNonQuery(SQL)
                        OraMain.Commit()

                        OraMain.BeginTrans()
                    End If

                    OraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("Web伝送ファイル監視", "失敗", ex.Message)
            BatchLog.Write_Err("Web伝送ファイル監視", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            MessageBox.Show("WEB伝送ファイルの処理に失敗しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            tmrLoop.Enabled = True
        End Try


    End Function
#End Region

#Region "終了ボタン"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        ' 2016/06/02 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応) -------------------- START
        BatchLog.Write("0000000000-00", "00000000", "業務終了", "")
        ' 2016/06/02 タスク）綾部 ADD 【OM】UI_B-99-99(RSV2対応) -------------------- END

        Me.Close()
    End Sub

#End Region

    Protected Overrides Sub Finalize()
        '2008/08/15 異常終了考慮
        If Not OraMain Is Nothing Then
            OraMain.Close()
        End If

        MyBase.Finalize()
    End Sub

    Public Function UpdateGamen(ByVal tuuban As Integer) As Boolean
        Dim StatusJ As String = ""
        Dim StartTime As String = ""
        Dim EndTime As String = ""
        Dim Msg As String = ""
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        ' DB接続確認
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        Try

            Try
                Dim SQL As New StringBuilder(256)
                SQL = New StringBuilder(128)
                OraReader = New CASTCommon.MyOracleReader(OraMain)
                SQL.Append("SELECT ")
                SQL.Append(" STATUS_J")
                SQL.Append(",STA_TIME_J")
                SQL.Append(",END_TIME_J")
                SQL.Append(",ERRMSG_J")
                SQL.Append(" FROM JOBMAST")
                SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append("   AND TUUBAN_J = " & tuuban.ToString)
                If OraReader.DataReader(SQL) = True Then
                    StatusJ = OraReader.GetValue(0)
                    StartTime = OraReader.GetValue(1).PadLeft(6, "0"c).Insert(2, ":").Insert(5, ":")
                    EndTime = OraReader.GetValue(2).PadLeft(6, "0"c).Insert(2, ":").Insert(5, ":")
                    Msg = OraReader.GetValue(3)
                Else
                    Return True
                End If
            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("UpdateGamen", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                Return True
            Finally
                OraReader.Close()
            End Try

            ' 通常リストから，ジョブ通番を捜す
            Dim LineColor As Color
            Dim FrontColor As Color
            Dim SubItem As ListViewItem.ListViewSubItem

            Dim Lsv1Count As Integer = ListView1.Items.Count - 1
            Dim Lsv2Count As Integer = ListView2.Items.Count - 1

            For i As Integer = 0 To Lsv1Count
                SubItem = ListView1.Items(i).SubItems(2)

                If SubItem.Text.ToString = tuuban.ToString Then
                    Dim ListItem As ListViewItem = ListView1.Items(i)

                    ListItem.SubItems(1).Text = fn_YOMIKAE_JYOUKYO(StatusJ, FrontColor)
                    ListItem.SubItems(4).Text = StartTime
                    ListItem.SubItems(5).Text = EndTime
                    ListItem.SubItems(7).Text = Msg
                    ListItem.SubItems(10).Text = StatusJ
                    ListItem.ForeColor = FrontColor
                    ListItem.Font = ListView1.Font
                    ListItem.Checked = False

                    If FrontColor.Equals(strERR_COLOR) = True Then
                        ' 異常の場合は，ListView2へ移動
                        ListItem.Remove()
                        If ListView2.Items.Count Mod 2 = 0 Then
                            LineColor = Color.White
                        Else
                            LineColor = Color.PaleGoldenrod
                        End If
                        ListItem.BackColor = LineColor
                        ListItem.Selected = True
                        ListView2.Items.Insert(0, ListItem)
                    End If

                    Exit For
                End If
            Next i

            ' 異常リストから，ジョブ通番を捜す
            For i As Integer = 0 To Lsv2Count
                SubItem = ListView2.Items(i).SubItems(2)

                If SubItem.Text.ToString = tuuban.ToString Then
                    Dim ListItem As ListViewItem = ListView2.Items(i)

                    ListItem.SubItems(1).Text = fn_YOMIKAE_JYOUKYO(StatusJ, FrontColor)
                    ListItem.SubItems(4).Text = StartTime
                    ListItem.SubItems(5).Text = EndTime
                    ListItem.SubItems(7).Text = Msg
                    ListItem.SubItems(10).Text = StatusJ
                    ListItem.ForeColor = FrontColor
                    ListItem.Font = ListView2.Font
                    ListItem.Checked = False

                    If StatusJ = "2" Then
                        ' 異常でなければ，ListView1へ移動
                        ListItem.Remove()
                        If ListView1.Items.Count Mod 2 = 0 Then
                            LineColor = Color.White
                        Else
                            LineColor = Color.LightGoldenrodYellow
                        End If
                        ListItem.BackColor = LineColor
                        ListItem.Selected = True
                        ListView1.Items.Insert(0, ListItem)
                    End If

                    Exit For
                End If
            Next i

            ListView1.Invalidate()
            ListView2.Invalidate()
        Catch exAll As Exception
            '【備忘録】
            ' VB.Netでのデバッグ時に以下の例外発生（既存のThreadMethodDelegate方式でも同様）
            '  「有効ではないスレッド間の操作: コントロールが作成されたスレッド以外のスレッドからコントロール 'ListView2' がアクセスされました」
            ' なお、画面EXEからの実行時には例外は発生しない
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
#If Not Debug Then
                BatchLog.Write_Err("UpdateGamen", "失敗", exAll)
#End If
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Console.WriteLine(exAll.Message)
        Finally

        End Try

        Return True
    End Function

    Public Function UpdateGamenItem1(ByVal item As ListViewItem) As Boolean
        Dim StatusJ As String = ""
        Dim StartTime As String = ""
        Dim EndTime As String = ""
        Dim Msg As String = ""
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            ' 通常リストから，ジョブ通番を捜す
            Dim LineColor As Color
            Dim FrontColor As Color
            Dim SubItem As ListViewItem.ListViewSubItem

            Dim Lsv1Count As Integer = ListView1.Items.Count - 1

            SubItem = item.SubItems(2)

            Try
                Dim SQL As New StringBuilder(256)
                SQL = New StringBuilder(128)
                OraReader = New CASTCommon.MyOracleReader(OraMain)
                SQL.Append("SELECT ")
                SQL.Append(" STATUS_J")
                SQL.Append(",STA_TIME_J")
                SQL.Append(",END_TIME_J")
                SQL.Append(",ERRMSG_J")
                SQL.Append(" FROM JOBMAST")
                SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append("   AND TUUBAN_J = " & SubItem.Text.ToString)
                If OraReader.DataReader(SQL) = True Then
                    StatusJ = OraReader.GetValue(0)
                    StartTime = OraReader.GetValue(1).PadLeft(6, "0"c).Insert(2, ":").Insert(5, ":")
                    EndTime = OraReader.GetValue(2).PadLeft(6, "0"c).Insert(2, ":").Insert(5, ":")
                    Msg = OraReader.GetValue(3)
                Else
                    Return True
                End If
            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("UpdateGamenItem1", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                Return True
            Finally
                OraReader.Close()
            End Try

            If item.SubItems(10).Text = StatusJ Then
                Return True
            End If

            item.SubItems(1).Text = fn_YOMIKAE_JYOUKYO(StatusJ, FrontColor)
            item.SubItems(4).Text = StartTime
            item.SubItems(5).Text = EndTime
            item.SubItems(7).Text = Msg
            item.SubItems(10).Text = StatusJ
            item.ForeColor = FrontColor
            item.Font = ListView1.Font
            item.Checked = False

            If FrontColor.Equals(strERR_COLOR) = True Then
                ' 異常の場合は，ListView2へ移動
                item.Remove()
                If ListView2.Items.Count Mod 2 = 0 Then
                    LineColor = Color.White
                Else
                    LineColor = Color.PaleGoldenrod
                End If
                item.BackColor = LineColor
                item.Selected = True
                ListView2.Items.Insert(0, item)
            End If
        Catch exAll As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("UpdateGamenItem1", "失敗", exAll)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Console.WriteLine(exAll.Message)
        Finally

        End Try

        Return True
    End Function

    Public Function UpdateGamenItem2(ByVal item As ListViewItem) As Boolean
        Dim StatusJ As String = ""
        Dim StartTime As String = ""
        Dim EndTime As String = ""
        Dim Msg As String = ""
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            ' 通常リストから，ジョブ通番を捜す
            Dim LineColor As Color
            Dim FrontColor As Color
            Dim SubItem As ListViewItem.ListViewSubItem

            Dim Lsv2Count As Integer = ListView2.Items.Count - 1

            SubItem = item.SubItems(2)

            Try
                Dim SQL As New StringBuilder(256)
                SQL = New StringBuilder(128)
                OraReader = New CASTCommon.MyOracleReader(OraMain)
                SQL.Append("SELECT ")
                SQL.Append(" STATUS_J")
                SQL.Append(",STA_TIME_J")
                SQL.Append(",END_TIME_J")
                SQL.Append(",ERRMSG_J")
                SQL.Append(" FROM JOBMAST")
                SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append("   AND TUUBAN_J = " & SubItem.Text.ToString)
                If OraReader.DataReader(SQL) = True Then
                    StatusJ = OraReader.GetValue(0)
                    StartTime = OraReader.GetValue(1).PadLeft(6, "0"c).Insert(2, ":").Insert(5, ":")
                    EndTime = OraReader.GetValue(2).PadLeft(6, "0"c).Insert(2, ":").Insert(5, ":")
                    Msg = OraReader.GetValue(3)
                Else
                    Return True
                End If
            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("UpdateGamenItem2", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                Return True
            Finally
                OraReader.Close()
            End Try

            If item.SubItems(10).Text = StatusJ Then
                Return True
            End If

            item.SubItems(1).Text = fn_YOMIKAE_JYOUKYO(StatusJ, FrontColor)
            item.SubItems(4).Text = StartTime
            item.SubItems(5).Text = EndTime
            item.SubItems(7).Text = Msg
            item.SubItems(10).Text = StatusJ
            item.ForeColor = FrontColor
            item.Font = ListView1.Font
            item.Checked = False

            ' 異常リストから，ジョブ通番を捜す
            SubItem = item.SubItems(2)

            item.SubItems(1).Text = fn_YOMIKAE_JYOUKYO(StatusJ, FrontColor)
            item.SubItems(4).Text = StartTime
            item.SubItems(5).Text = EndTime
            item.SubItems(7).Text = Msg
            item.SubItems(10).Text = StatusJ
            item.ForeColor = FrontColor
            item.Font = ListView2.Font
            item.Checked = False

            If StatusJ = "2" Then
                ' 異常でなければ，ListView1へ移動
                item.Remove()
                If ListView1.Items.Count Mod 2 = 0 Then
                    LineColor = Color.White
                Else
                    LineColor = Color.LightGoldenrodYellow
                End If
                item.BackColor = LineColor
                item.Selected = True
                ListView1.Items.Insert(0, item)
            End If
        Catch exAll As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("UpdateGamenItem2", "失敗", exAll)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Console.WriteLine(exAll.Message)
        Finally

        End Try

        Return True
    End Function

    Private Sub TmrUpdate_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TmrUpdate.Tick
        If strPC_NAME.Trim <> gastrBATCH_SV Then
            Exit Sub
        End If

        TmrUpdate.Enabled = False

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            Dim OraReader As CASTCommon.MyOracleReader = Nothing
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
            Dim dblock As CASTCommon.CDBLock = New CASTCommon.CDBLock
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

            ' DB接続確認
            Do
                DBConnectJudge()
            Loop Until ConnectStopFlag = False

            Try
                Dim FoundItem As New ArrayList

                For i As Integer = 0 To AutoList.Count - 1
                    Dim AutoKey As String() = CType(AutoList(i), String).Split("="c)

                    If AutoKey(2) <= DateTime.Now.ToString("HHmm") AndAlso DateTime.Now.ToString("HHmm") <= AutoKey(3) Then

                        ' 起動時間範囲内の場合，起動する
                        Dim SQL As New StringBuilder(128)
                        SQL.Append("SELECT STA_TIME_J FROM JOBMAST")
                        SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                        SQL.Append("   AND JOBID_J = '" & AutoKey(1) & "'")
                        SQL.Append("   AND (STATUS_J = '0'")
                        SQL.Append("    OR  STA_TIME_J BETWEEN '" & AutoKey(2) & "00' AND '" & AutoKey(3) & "99')")

                        '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        'Dim OraReader As New CASTCommon.MyOracleReader(OraMain)
                        OraReader = New CASTCommon.MyOracleReader(OraMain)
                        '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                        If OraReader.DataReader(SQL) = False Then
                            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
                            ' ジョブ登録ロック
                            If dblock.InsertJOBMAST_Lock(OraMain, mLockWaitTime) = False Then
                                Throw New Exception("ジョブ登録タイムアウト")
                            End If
                            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

                            SQL = New StringBuilder(128)
                            SQL.Append("INSERT INTO JOBMAST(")
                            SQL.Append(" TOUROKU_DATE_J")
                            SQL.Append(",TOUROKU_TIME_J")
                            SQL.Append(",JOBID_J")
                            SQL.Append(",STATUS_J")
                            SQL.Append(")")
                            SQL.Append(" VALUES(")
                            SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")
                            SQL.Append(",TO_CHAR(SYSDATE,'HH24MISS')")
                            SQL.Append(", '" & AutoKey(1) & "'")
                            SQL.Append(",'0')")
                            OraMain.ExecuteNonQuery(SQL)

                            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
                            ' ジョブ登録アンロック
                            dblock.InsertJOBMAST_UnLock(OraMain)
                            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

                            OraMain.Commit()

                            OraMain.BeginTrans()
                        End If
                        OraReader.Close()

                        FoundItem.Add(i)
                    Else
                        If DateTime.Now.ToString("HHmm") > AutoKey(3) Then
                            FoundItem.Add(i)
                        End If
                    End If
                Next i

                For i As Integer = 0 To FoundItem.Count - 1
                    AutoList.RemoveAt(CType(FoundItem.Item(i), Integer))
                Next i
            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                '【備忘録】
                ' iniファイルの[AUTO]の定義誤り（=がない）時に以下の例外発生
                '  「インデックスが配列の境界外です」
                BatchLog.Write_Err("TmrUpdate_Tick", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                End If
                '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
                ' ジョブ登録アンロック
                dblock.InsertJOBMAST_UnLock(OraMain)

                ' ロールバック
                OraMain.Rollback()
                OraMain.BeginTrans()
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

            End Try

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        End SyncLock
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

        If AutoList.Count > 0 Then
            TmrUpdate.Enabled = True
        End If
    End Sub

    ' 詳細情報フォームを開く
    Private Sub ListView1_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles ListView1.ItemCheck
        Try
            If e.NewValue = CheckState.Checked Then
                mVisibleFlag = True

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
                SyncLock mThreadLock
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

                    For i As Integer = 0 To ListView1.CheckedItems.Count - 1
                        If ListView1.CheckedItems(i).Index <> e.Index Then
                            ListView1.CheckedItems(i).Checked = False
                        End If
                    Next i
                    For i As Integer = 0 To ListView2.CheckedItems.Count - 1
                        If ListView2.CheckedItems(i).Index <> e.Index Then
                            ListView2.CheckedItems(i).Checked = False
                        End If
                    Next i

                    Dim Item As ListViewItem = ListView1.Items(e.Index)

                    ' DB接続確認
                    Do
                        DBConnectJudge()
                    Loop Until ConnectStopFlag = False

                    FrmSub.Owner = Me
                    FrmSub.OwnerForm = Me
                    FrmSub.DB = OraMain

                    FrmSub.JobTuuban = CASTCommon.CAInt32(Item.SubItems(2).Text)
                    FrmSub.JobID = Item.SubItems(8).Text
                    FrmSub.JobPara = Item.SubItems(9).Text
                    FrmSub.JobMessage = Item.SubItems(7).Text
                    FrmSub.Status = Item.SubItems(10).Text

                    CType(sender, ListView).Items(e.Index).Selected = True

                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
                End SyncLock
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

                If FrmSub.Visible = False Then
                    FrmSub.Visible = True
                End If

                mVisibleFlag = False
            Else
                If mVisibleFlag = False Then
                    FrmSub.Visible = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("詳細情報フォーム", "失敗", ex.ToString)
            BatchLog.Write_Err("詳細情報フォーム", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try

    End Sub

    ' 詳細情報フォームを開く
    Private Sub ListView2_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles ListView2.ItemCheck
        Try
            If e.NewValue = CheckState.Checked Then
                mVisibleFlag = True

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
                SyncLock mThreadLock
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

                    For i As Integer = 0 To ListView1.CheckedItems.Count - 1
                        If ListView1.CheckedItems(i).Index <> e.Index Then
                            ListView1.CheckedItems(i).Checked = False
                        End If
                    Next i
                    For i As Integer = 0 To ListView2.CheckedItems.Count - 1
                        If ListView2.CheckedItems(i).Index <> e.Index Then
                            ListView2.CheckedItems(i).Checked = False
                        End If
                    Next i

                    Dim Item As ListViewItem = ListView2.Items(e.Index)

                    FrmSub.Owner = Me
                    FrmSub.OwnerForm = Me
                    FrmSub.DB = OraMain

                    FrmSub.JobTuuban = CASTCommon.CAInt32(Item.SubItems(2).Text)
                    FrmSub.JobID = Item.SubItems(8).Text
                    FrmSub.JobPara = Item.SubItems(9).Text
                    FrmSub.JobMessage = Item.SubItems(7).Text
                    FrmSub.Status = Item.SubItems(10).Text

                    ListView2.Items(e.Index).Selected = True

                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
                End SyncLock
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

                If FrmSub.Visible = False Then
                    FrmSub.Visible = True
                End If

                mVisibleFlag = False
            Else
                If mVisibleFlag = False Then
                    FrmSub.Visible = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("詳細情報フォーム", "失敗", ex.ToString)
            BatchLog.Write_Err("詳細情報フォーム", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
        End Try
    End Sub

    '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
    ' 戻り値とパラメータのあるデリゲート
    'Delegate Function ThreadMethodDelegate(ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Integer '
    'Shared threadMethodDelegateInstance() As ThreadMethodDelegate '
    '*** End Del 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
    Shared ExePath As String

    '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
    'Private Function JobExecute(ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Boolean

    '    Try
    '        Dim EXEFlag As Boolean = False
    '        For syoriCount As Integer = 0 To threadMethodDelegateInstance.Length - 1
    '            If threadMethodDelegateInstance(syoriCount) Is Nothing OrElse threadMethodDelegateInstance(syoriCount).Target Is Nothing Then

    '                threadMethodDelegateInstance(syoriCount) _
    '                  = New ThreadMethodDelegate(AddressOf ThreadMethod) '

    '                ' デリゲートによるスレッド処理呼び出し
    '                threadMethodDelegateInstance(syoriCount).BeginInvoke(tuuban, exe, para, _
    '                    New AsyncCallback(AddressOf MyCallback), threadMethodDelegateInstance(syoriCount)) '
    '                EXEFlag = True

    '                Exit For
    '            End If
    '        Next

    '        If EXEFlag = False Then
    '            Return False
    '        End If

    '        Return True
    '    Catch ex As Exception
    '        Console.WriteLine(ex.Message)

    '        Call AtoSyori(tuuban)

    '        Return True
    '    End Try

    'End Function

    '' 別スレッドで呼び出されるメソッド
    'Private Shared Function ThreadMethod(ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Integer
    '    '--------------------------------------
    '    'ジョブの発行
    '    '--------------------------------------
    '    Try
    '        Dim SInfo As New ProcessStartInfo
    '        SInfo.WorkingDirectory = ExePath
    '        SInfo.FileName = Path.Combine(ExePath, exe)
    '        SInfo.Arguments = para.Trim & "," & tuuban.ToString
    '        SInfo.UseShellExecute = False
    '        SInfo.CreateNoWindow = True
    '        Dim WaitProc As Process = Process.Start(SInfo)

    '        If Not WaitProc Is Nothing Then
    '            WaitProc.WaitForExit()
    '        End If
    '    Catch e As Exception
    '    End Try

    '    Return tuuban
    'End Function

    '' スレッド処理終了後に呼び出されるコールバック・メソッド
    'Private Sub MyCallback(ByVal ar As IAsyncResult)  '
    '    Dim JobTuuban As Integer

    '    ' ジョブ通番取り出し
    '    Dim delG As ThreadMethodDelegate = CType(ar.AsyncState, ThreadMethodDelegate)
    '    JobTuuban = delG.EndInvoke(ar)

    '    Call AtoSyori(JobTuuban)
    'End Sub


    ' 機能　 ： ジョブを別スレッドで実行する
    ' 引数　 ： db ：DBコネクション
    '           waittime ： 待ち時間（秒）
    '           waittime ： 待ち時間（秒）
    '           waittime ： 待ち時間（秒）
    ' 復帰値 ： Trueのみ
    ' 備考　 ： 従来は、JobExecuteメソッド内でスレッドテーブルに空きがない場合にFlaseを返していたが、
    '           スレッドテーブルの空きチェックを呼出し元で行うように変更したためTrueのみを返す
    Private Function JobExecute(ByVal useTblIdx As Integer, ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Boolean

        Try
            ' スレッド使用管理テーブルを使用中にする
            mThreadUseTbl(useTblIdx) = 1

            Dim thrClass As New ThreadClass()
            thrClass.frm = Me
            thrClass.tuuban = tuuban
            thrClass.exe = exe
            thrClass.para = para
            thrClass.useTblIdx = useTblIdx

            Dim Thread As New Threading.Thread(AddressOf thrClass.Run)
            mThreadClsTbl(useTblIdx) = Thread

            Thread.Start()

            Return True

        Catch ex As Exception
            ' スレッド使用管理テーブルを未使用にする
            mThreadUseTbl(useTblIdx) = 0
            mThreadClsTbl(useTblIdx) = Nothing

            BatchLog.Write_Err("ジョブ実行", "失敗", ex)
            Console.WriteLine(ex.Message)

            Call AtoSyori(tuuban)

            Return True
        End Try

    End Function

    '----------------------------------------------------------
    ' スレッド実行インナークラス
    '----------------------------------------------------------
    Class ThreadClass

        Friend frm As FrmJOB
        Friend tuuban As Integer
        Friend exe As String
        Friend para As String
        Friend useTblIdx As Integer

        Sub Run()

            Dim SInfo As New ProcessStartInfo
            Dim sw As System.Diagnostics.Stopwatch = Nothing

            '--------------------------------------
            'ジョブの発行
            '--------------------------------------
            Try
                If frm.BatchLog.IS_LEVEL2() = True Then
                    sw = frm.BatchLog.Write_Enter2("ジョブ実行スレッド", SInfo.FileName & " " & SInfo.Arguments)
                End If

                SInfo.WorkingDirectory = ExePath
                SInfo.FileName = Path.Combine(ExePath, exe)
                SInfo.Arguments = para.Trim & "," & tuuban.ToString
                SInfo.UseShellExecute = False
                SInfo.CreateNoWindow = True
                Dim WaitProc As Process = Process.Start(SInfo)

                If Not WaitProc Is Nothing Then
                    WaitProc.WaitForExit()

                    SyncLock mThreadLock
                        ' ジョブ終了後の後処理
                        Call frm.AtoSyori(tuuban)
                    End SyncLock

                Else
                    Throw New Exception(SInfo.FileName & "の実行に失敗しました。")
                End If

            Catch ex As Exception
                ' 停止フラグがＯＦＦの場合
                If mStopFlg = False Then
                    frm.BatchLog.Write_LEVEL1("ジョブ実行スレッド", "失敗", SInfo.FileName & " " & SInfo.Arguments)
                    frm.BatchLog.Write_Err("ジョブ実行スレッド", "失敗", ex)
                    frm.fn_JOBMAST_UPDATE("7", ex.Message, tuuban)
                End If

            Finally
                If frm.BatchLog.IS_LEVEL2() = True Then
                    frm.BatchLog.Write_Exit2(sw, "ジョブ実行スレッド")
                End If

                SyncLock mThreadLock
                    ' スレッド使用管理テーブルを未使用にする
                    mThreadUseTbl(useTblIdx) = 0
                    mThreadClsTbl(useTblIdx) = Nothing
                End SyncLock

                frm = Nothing
                tuuban = Nothing
                exe = Nothing
                para = Nothing
                useTblIdx = Nothing

            End Try

        End Sub

    End Class
    '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***


    Private Sub AtoSyori(ByVal tuuban As Integer)
        Dim StatusJ As String = ""
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        ' DB接続確認
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        Try
            Dim SQL As New StringBuilder(128)
            OraReader = New CASTCommon.MyOracleReader(OraMain)
            SQL.Append("SELECT STATUS_J FROM JOBMAST")
            SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
            SQL.Append("   AND TUUBAN_J = " & tuuban.ToString)
            If OraReader.DataReader(SQL) = True Then
                StatusJ = OraReader.GetValue(0)
                '2011/05/25 異常終了の場合も含める
                'If StatusJ = "1" Then
                If StatusJ = "1" OrElse StatusJ = "3" Then
                    ' スレッドが終了したにもかかわらず，処理中の場合は，処理失敗とみなす
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE JOBMAST SET STATUS_J='7'")
                    SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    SQL.Append("   AND TUUBAN_J = " & tuuban.ToString)
                    '2011/05/25 異常終了の場合も含める
                    'SQL.Append("   AND STATUS_J = '1'")
                    SQL.Append("   AND STATUS_J IN ('1','3')")
                    Call OraMain.ExecuteNonQuery(SQL)
                    OraMain.Commit()

                    OraMain.BeginTrans()
                End If
            End If
        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("AtoSyori", "失敗", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Return
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

        '2011/05/25 プロセス終了後は画面更新させる
        Call UpdateGamen(tuuban)
    End Sub

    ' 最新状態表示
    Private Sub BtnRefresh_Click1(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnRefresh.Click

        Me.SuspendLayout()

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        SyncLock mThreadLock

            ' 強制GC
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

            mNowMaxTuuban = 0
            Try
                '*** Str Del 2015/12/01 SO)荒木 for 潜在障害（更新ボタン押下時にインデック不要となる） ***
                'Dim ListTop1 As Integer
                'If Not ListView1.TopItem Is Nothing Then
                '    ListTop1 = ListView1.TopItem.Index
                'Else
                '    ListTop1 = -1
                'End If
                'Dim ListTop2 As Integer
                'If Not ListView2.TopItem Is Nothing Then
                '    ListTop2 = ListView2.TopItem.Index
                'Else
                '    ListTop2 = -1
                'End If
                '*** End Del 2015/12/01 SO)荒木 for 潜在障害（更新ボタン押下時にインデック不要となる） ***

                ListView1.Clear()
                ListView2.Clear()

                Call fn_SET_GAMEN()

                '*** Str Add 2015/12/01 SO)荒木 for 潜在障害（更新ボタン押下時にインデック不要となる） ***
                Dim ListTop1 As Integer
                If Not ListView1.TopItem Is Nothing Then
                    ListTop1 = ListView1.TopItem.Index
                Else
                    ListTop1 = -1
                End If
                Dim ListTop2 As Integer
                If Not ListView2.TopItem Is Nothing Then
                    ListTop2 = ListView2.TopItem.Index
                Else
                    ListTop2 = -1
                End If
                '*** End Add 2015/12/01 SO)荒木 for 潜在障害（更新ボタン押下時にインデック不要となる） ***

                If ListTop1 >= 0 Then
                    ListView1.Items(ListTop1).Selected = True
                End If
                If ListTop2 >= 0 Then
                    ListView2.Items(ListTop2).Selected = True
                End If
            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("BtnRefresh_Click1", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                Console.WriteLine(ex.Message)
            End Try

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        End SyncLock
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

        Me.ResumeLayout(True)
    End Sub

    Private Sub UpdateDisplayOnly()
        ' DB接続確認
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        Try
            Dim SQL As New StringBuilder(128)
            '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            'Dim OraReader As New CASTCommon.MyOracleReader(OraMain)
            OraReader = New CASTCommon.MyOracleReader(OraMain)
            '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            Dim Lsv1Count As Integer = ListView1.Items.Count - 1
            Dim Lsv2Count As Integer = ListView2.Items.Count - 1

            For i As Integer = 0 To Lsv1Count
                Dim ListItem As ListViewItem = ListView1.Items(i)

                If ListItem.SubItems(10).Text = "0" OrElse ListItem.SubItems(10).Text = "1" Then
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT STATUS_J")
                    SQL.Append(" FROM JOBMAST")
                    SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    SQL.Append("   AND TUUBAN_J = " & ListItem.SubItems(2).Text)
                    SQL.Append(" ORDER  BY TUUBAN_J ASC")
                    If OraReader.DataReader(SQL) = True Then
                        If OraReader.GetValue(0) <> ListItem.SubItems(10).Text Then
                            Call UpdateGamen(CAInt32(ListItem.SubItems(2).Text))
                        End If
                    End If
                    OraReader.Close()
                End If
            Next i

            For i As Integer = 0 To Lsv2Count
                Dim ListItem As ListViewItem = ListView2.Items(i)

                If ListItem.SubItems(10).Text = "0" OrElse ListItem.SubItems(10).Text = "1" Then
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT STATUS_J")
                    SQL.Append(" FROM JOBMAST")
                    SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    SQL.Append("   AND TUUBAN_J = " & ListItem.SubItems(2).Text)
                    SQL.Append(" ORDER  BY TUUBAN_J ASC")
                    If OraReader.DataReader(SQL) = True Then
                        If OraReader.GetValue(0) <> ListItem.SubItems(10).Text Then
                            Call UpdateGamen(CType(ListItem.SubItems(2).Text, Integer))
                        End If
                    End If
                    OraReader.Close()
                End If
            Next i

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("UpdateDisplayOnly", "失敗", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Console.WriteLine(ex.Message)
        Finally
        End Try
    End Sub

    Private Sub StartGamen()
        ListView1.Clear()
        ListView2.Clear()

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

            ' DB接続確認
            Do
                DBConnectJudge()
            Loop Until ConnectStopFlag = False

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面の表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '*** Str Del 2015/12/01 SO)荒木 for 不要DBコネクション削除 ***
            'OraMain = New CASTCommon.MyOracle
            '*** End Del 2015/12/01 SO)荒木 for 不要DBコネクション削除 ***
            Dim SQL As New StringBuilder(128)

            ' 前日までの起動まちとなっているジョブをキャンセルする
            SQL = New StringBuilder(128)
            SQL.Append("UPDATE JOBMAST SET")
            SQL.Append(" STATUS_J='4'")
            SQL.Append(",ERRMSG_J='前日未起動処理をキャンセルしました'")
            SQL.Append(" WHERE TOUROKU_DATE_J < TO_CHAR(SYSDATE,'YYYYMMDD')")
            SQL.Append("   AND STATUS_J = '0'")

            '接続失敗を考慮
            Try
                Call OraMain.ExecuteNonQuery(SQL)
                OraMain.Commit()

                OraMain.BeginTrans()

            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("StartGamen", "失敗", ex)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                blnDB_CONNECT = False
                BtnRefresh.Enabled = False
                MessageBox.Show(MSG0030E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            '時刻設定
            lblDate.Text = CASTCommon.Calendar.Now.ToString("yyyy年MM月dd日")
            lblNowTime.Text = System.DateTime.Now.ToString("HH:mm:ss")

            Call fn_SET_GAMEN()

            If Not FrmSub Is Nothing Then
                FrmSub.Close()
                FrmSub.Dispose()
                FrmSub = Nothing
            End If

            FrmSub = New FrmSubJob
            FrmSub.Visible = False

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
            FrmSub.ThreadLock = mThreadLock
        End SyncLock
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

    End Sub

    Private Sub DBConnectJudge()
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Threading.Thread.Sleep(50) '0.05秒待機

        If ConnectStopFlag = True Then
            Exit Sub
        End If

        ' 他のスレッドから処理が走らないように，ロックをかける
        ConnectStopFlag = True

        Try
            SQL.Append("SELECT LOGINID_U FROM UIDMAST")
            OraReader = New CASTCommon.MyOracleReader(OraMain)
            If OraReader.DataReader(SQL) = False Then
                Try
                    ' DB再オープン
                    Console.WriteLine("OPEN OPEN")
                    OraMain.Close()
                    OraMain = Nothing
                    OraMain = New CASTCommon.MyOracle
                Catch exA As Exception
                    '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                    BatchLog.Write_Err("DBConnectJudge", "失敗", exA)
                    '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

                End Try
            Else
            End If
            ConnecttedFlag = True
        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("DBConnectJudge", "失敗", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Try
                ' DB再オープン
                Console.WriteLine("OPEN OPEN")
                OraMain.Close()
                OraMain = Nothing
                OraMain = New CASTCommon.MyOracle

            Catch exA As Exception
                '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
                BatchLog.Write_Err("DBConnectJudge", "失敗", exA)
                '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            End Try
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

        ' ロック解除
        ConnectStopFlag = False
    End Sub

    Private Sub TmrDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TmrDisplay.Tick
        If UpdateGamenFlag = True Then
            Return
        End If

        UpdateGamenFlag = True

        TmrDisplay.Enabled = False

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

            ' DB接続確認
            Do
                DBConnectJudge()
            Loop Until ConnectStopFlag = False

            For Each Item As ListViewItem In ListView1.Items
                If Item.SubItems(10).Text.ToCharArray <> "2" Then
                    Call UpdateGamenItem1(Item)
                End If
            Next
            For Each Item As ListViewItem In ListView2.Items
                Call UpdateGamenItem2(Item)
            Next

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***
        End SyncLock
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（マルチスレッド対応） ***

        UpdateGamenFlag = False

        TmrDisplay.Enabled = True
    End Sub

    'システム日付の休日判定
    Private Function TodayIsHoliday() As Boolean
        '土日の場合
        If System.DateTime.Today.DayOfWeek = DayOfWeek.Sunday OrElse System.DateTime.Today.DayOfWeek = DayOfWeek.Saturday Then
            Return True
        End If

        '*** Str Upd 2015/12/01 SO)荒木 for 潜在障害（起動遅延） ***
        'それ以外の場合
        '       Dim con As OracleConnection = New OracleConnection("User=kzamast;Password=kzamast;Data Source=FSKJ_LSNR;Pooling=false")
        '       Dim command As OracleCommand
        '
        '       Try
        '           Dim sql As String = "SELECT COUNT(YASUMI_DATE_Y) FROM YASUMIMAST WHERE YASUMI_DATE_Y = '" & System.DateTime.Today.ToString("yyyyMMdd") & "'"
        '
        '           con.Open()
        '           command = New OracleCommand(sql, con)
        '
        '           Dim ret As Object = command.ExecuteScalar
        '
        '           If ret Is DBNull.Value OrElse Integer.Parse(ret.ToString) = 0 Then
        '               Return False
        '           End If
        '
        '       Catch ex As Exception
        '           Return False
        '       Finally
        '           If Not con Is Nothing AndAlso con.State = ConnectionState.Open Then
        '               con.Close()
        '           End If
        '       End Try

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim sql As String = "SELECT * FROM YASUMIMAST WHERE YASUMI_DATE_Y = '" & System.DateTime.Today.ToString("yyyyMMdd") & "'"
            OraReader = New CASTCommon.MyOracleReader(OraMain)
            Return OraReader.DataReader(sql)

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
            BatchLog.Write_Err("TodayIsHoliday", "失敗", ex)
            '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

            Return False

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If

        End Try
        '*** End Upd 2015/12/01 SO)荒木 for 潜在障害（起動遅延） ***

        Return True
    End Function

    'F5キーで最新表示機能追加
    Private Sub FrmJOB_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F5 Then
            Me.KeyPreview = False
            Call BtnRefresh_Click1(sender, e)
            Thread.Sleep(100)
            Me.KeyPreview = True
        End If
    End Sub

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***
    ' 機能　 ： スレッド使用管理テーブルの空きインデックス番号を返す
    ' 復帰値 ： 0以上： 空きインデックス番号
    '           -1   ： 空き無し
    Private Function getFreeThreadTblIdx() As Integer

        For idx As Integer = 0 To mThreadUseTbl.Length - 1
            ' スレッド使用管理テーブルを未使用の場合
            If mThreadUseTbl(idx) = 0 Then
                If BatchLog.IS_LEVEL2() = True Then
                    BatchLog.Write_LEVEL2("getFreeThreadTblIdx", "成功", "idx=" & idx)
                End If
                Return idx
            End If
        Next

        ' 空き無し
        Return -1

    End Function
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ハンドルリークするのでマルチスレッド実装方法変更） ***

    '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- START
    'テキストファイルよりジョブ名を取得する
    Private Function getTxtToJobName() As Boolean
        Dim TXTFILE As String = System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "ジョブ名管理.TXT")
        Dim SR As StreamReader = Nothing
        Dim LineData As String
        Dim Count As Integer = 0

        Try
            'ファイルの存在チェック
            If Not File.Exists(TXTFILE) Then
                '無ければ正常で抜ける
                Return True
            End If

            SR = New StreamReader(TXTFILE, Encoding.GetEncoding(932))

            LineData = SR.ReadLine
            Do While Not LineData Is Nothing
                Dim Data() As String = LineData.Split(","c)
                If Data.Length = 2 Then
                    ReDim Preserve TxtJob(Count)

                    TxtJob(Count).JOBID = Data(0)
                    TxtJob(Count).JOBNAME = Data(1)

                    Count += 1
                End If

                LineData = SR.ReadLine
            Loop

            Return True

        Catch ex As Exception
            BatchLog.Write_Err("getTxtToJobName", "失敗", ex)
            Return False
        End Try
    End Function
    '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(標準版改善) -------------------- END

End Class
