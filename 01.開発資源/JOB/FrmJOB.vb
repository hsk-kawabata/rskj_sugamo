Option Strict On
Option Explicit On

Imports System.Data.OracleClient
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.FileSystem    '2012/06/30 �W���Ł@Web�`���Ή�
'*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
' 2016/06/02 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- START
Imports System.Security.Permissions
' 2016/06/02 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- END

' �W���u�Ǘ����C���t�H�[��
Public Class FrmJOB
    Inherits System.Windows.Forms.Form

#Region "�錾"

    Public gastrBATCH_SV As String
    Dim gastrJOB_LOOPTIME As Integer
    Dim gastrJOB_MULTI As Integer

    Dim strPC_NAME As String   '�R���s���[�^��

    Private intJOB_COUNT As Integer '�W���u�̌���
    Private intOLD_JOB_COUNT As Integer = 0 '�^�C�}�[�X�V�O�̃W���u����

    Private OraMain As New CASTCommon.MyOracle

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
    Private mLockWaitTime As Integer = 30
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
    Friend Shared mThreadLock As New Object   ' �X���b�h�r�����b�N�I�u�W�F�N�g
    Friend Shared mThreadUseTbl() As Integer  ' �X���b�h�g�p�Ǘ��e�[�u��
    Friend Shared mThreadClsTbl() As Thread   ' �X���b�h���s�N���X�Ǘ��e�[�u��
    Friend Shared mStopFlg As Boolean = False  ' �I���t���O
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

    Dim strERR_JOB As String
    Private strERR_COLOR As Color = Color.Red
    Private strBLACK_COLOR As Color = Color.Black
    Private strSYOUSAI_PARA As String

    Private mNowMaxTuuban As Integer = 0                ' ���߂Ŏ擾�����W���u�ʔ�

    Private mVisibleFlag As Boolean = False

    ' �ڍ׏��t�H�[��
    Private FrmSub As FrmSubJob = Nothing

    ' �����N�����W���[���ꗗ
    Private AutoList As New ArrayList

    Private ConnecttedFlag As Boolean = False
    Private ConnectStopFlag As Boolean = False

    Private UpdateGamenFlag As Boolean = False

    '�f�[�^�x�[�X�ڑ��t���O
    Private blnDB_CONNECT As Boolean = True

    '�Ԋҏ����̏����ُ팟�m
    Private ELog As New CASTCommon.ClsEventLOG
    Private htErrJob As Hashtable = Hashtable.Synchronized(New Hashtable)   '���������ꂽhashtable

    '���O�N���X
    Public BatchLog As New CASTCommon.BatchLOG("KFUJOBW010", "�W���u�Ď�")

    '2012/06/30 �W���Ł@Web�`���Ή�
    Private strFilePath As String   'Web�`���̃t�@�C���i�[�ꏊ
    Private strFileBkupPath As String   'Web�`���̃o�b�N�A�b�v�t�@�C���i�[�ꏊ
    Private strFileSendBkupPath As String   'Web�`���̑��M�o�b�N�A�b�v�t�@�C���i�[�ꏊ
    Private strFileName As String    'Web�`���̃t�@�C����
    Private strPara As String       '�W���u�Ď��o�^�p
    Private SiyouFlg As String       'WEB�`���g�p�t���O

    '2016/10/07 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
    Private Structure JobInfo
        Dim JOBID As String
        Dim JOBNAME As String
    End Structure
    Private TxtJob() As JobInfo
    '2016/10/07 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END

    ' 2016/06/02 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- START
    ' ��ʉE��́u�~�v�{�^�����g�p�s�ɐݒ肷��
    Protected Overrides ReadOnly Property CreateParams As System.Windows.Forms.CreateParams
        <SecurityPermission(SecurityAction.Demand, flags:=SecurityPermissionFlag.UnmanagedCode)> _
        Get
            Const Cs_NoClose As Integer = &H200
            Dim Cp As CreateParams = MyBase.CreateParams
            Cp.ClassStyle = Cp.ClassStyle Or Cs_NoClose
            Return Cp
        End Get
    End Property
    ' 2016/06/02 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- END

#End Region

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �� dispose ���I�[�o�[���C�h���ăR���|�[�l���g�ꗗ���������܂��B
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    ' Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^�͎g�p���Ȃ��ł��������B
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
        Me.Label1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(5, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(665, 30)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "�����U�փo�b�`�W���u�Ď����"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(505, 11)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "���ݎ���"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblNowTime
        '
        Me.lblNowTime.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.Label7.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(8, 64)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(357, 13)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "���׃_�u���N���b�N�ŃW���u�̏ڍ׏�񂪎Q�Ƃł��܂�"
        '
        'btnEnd
        '
        Me.btnEnd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(550, 15)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(110, 35)
        Me.btnEnd.TabIndex = 2
        Me.btnEnd.TabStop = False
        Me.btnEnd.Text = "�Ɩ��I��"
        '
        'lblDate
        '
        Me.lblDate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.lblKanshi.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.ListView1.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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
        Me.BtnRefresh.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.BtnRefresh.Location = New System.Drawing.Point(567, 50)
        Me.BtnRefresh.Name = "BtnRefresh"
        Me.BtnRefresh.Size = New System.Drawing.Size(100, 32)
        Me.BtnRefresh.TabIndex = 1
        Me.BtnRefresh.Text = "�ŐV�\��(F5)"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.ListView1)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GroupBox3.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(0, 88)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(674, 278)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "����I���E�N���҂��E�������E�L�����Z��"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.ListView2)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox4.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.GroupBox4.Location = New System.Drawing.Point(0, 379)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(674, 200)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "�ُ�I��"
        '
        'ListView2
        '
        Me.ListView2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ListView2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
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

#Region "��ʂ̃��[�h"
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        lblDate.Text = CASTCommon.Calendar.Now.ToString("yyyy�NMM��dd��")
        '------------------------------------------
        'INI�t�@�C���̓ǂݍ���
        '------------------------------------------
        If fn_INI_READ() = False Then
            Exit Sub
        End If

        ' 2016/06/02 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- START
        BatchLog.Write("0000000000-00", "00000000", "�Ɩ��J�n", "")
        ' 2016/06/02 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- END

        '2016/10/06 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
        '�e�L�X�g�t�@�C�����W���u�����擾����
        getTxtToJobName()
        '2016/10/06 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END

        '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
        'threadMethodDelegateInstance = CType(Array.CreateInstance(GetType(ThreadMethodDelegate), gastrJOB_MULTI), ThreadMethodDelegate())
        mThreadUseTbl = New Integer(gastrJOB_MULTI - 1) {}
        mThreadClsTbl = New Thread(gastrJOB_MULTI - 1) {}
        '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***

        '------------------------------------------
        '�R���s���[�^���̎擾
        '------------------------------------------
        strPC_NAME = Environment.MachineName

        If gastrJOB_LOOPTIME = 0 Then
            gastrJOB_LOOPTIME = 5000
        End If

        ' ������ʐݒ菈��
        Call StartGamen()

        Me.AddOwnedForm(FrmSub)


        '�[�����[�h�̏ꍇ�͎����N�����Ȃ�
        If strPC_NAME.Trim <> gastrBATCH_SV Then
        ElseIf TodayIsHoliday() = False Then
            Try
                '------------------------------------------
                '�����N�����W���[�� �ݒ�
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
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("Form1_Load", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            End Try

        End If

        '------------------------------------------
        '�^�C�}�[�̎��s
        '------------------------------------------
        tmrLoop.Interval = gastrJOB_LOOPTIME
        tmrLoop.Start()

        '�ڑ����s���̓^�C�}�[��~
        If blnDB_CONNECT = False Then
            Try
                tmrLoop.Stop()
                TmrDisplay.Stop()
                TmrUpdate.Stop()

            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("Form1_Load", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                Console.WriteLine(ex.Message)
            End Try
        End If
    End Sub

    '*** Str Add 2015/12/01 SO)�r�� for �s�vDB�R�l�N�V�����폜 ***
    Private Sub FrmJOB_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        Try
            SyncLock mThreadLock
                mStopFlg = True  ' �X���b�h���ł̗�O���O�}�~�̂��߂ɒ�~�t���O���n�m�ɂ���

                For idx As Integer = 0 To mThreadClsTbl.Length - 1
                    ' �X���b�h�N���X�Ǘ��e�[�u���̃X���b�h���~����
                    If Not mThreadClsTbl(idx) Is Nothing Then
                        mThreadClsTbl(idx).Abort()
                        'mThreadClsTbl(idx).Join()  ' �҂ƏI�����x���Ȃ�̂ő҂��Ȃ�

                        If BatchLog.IS_LEVEL2() = True Then
                            BatchLog.Write_LEVEL2("�X���b�h��~", "����", "idx=" & idx)
                        End If
                    End If
                Next
            End SyncLock
        Catch ex As Exception
            BatchLog.Write_Err("FrmJOB_FormClosed", "���s", ex)
        End Try

    End Sub
    '*** End Add 2015/12/01 SO)�r�� for �s�vDB�R�l�N�V�����폜 ***

#End Region

#Region "�^�C�}�["
    Private Sub tmrLoop_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrLoop.Tick

        tmrLoop.Enabled = False

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

            '--------------------------------------
            '��ʂ̃V�X�e�����Ԃ̕\��
            '--------------------------------------
            lblDate.Text = CASTCommon.Calendar.Now.ToString("yyyy�NMM��dd��")
            lblNowTime.Text = CASTCommon.Calendar.Now.ToString("HH:mm:ss")

            Call fn_SET_GAMEN()

            Me.ResumeLayout(False)

            '--------------------------------------
            '�W���u�̔��s
            '--------------------------------------
            fn_CREATE_JOB()

            If strPC_NAME.Trim <> gastrBATCH_SV Then
                lblKanshi.Text = "���s�󋵍X�V���i�[�����[�h�j"
            Else
                lblKanshi.Text = "���s�󋵍X�V��"
            End If

            '2012/06/30 �W���Ł@Web�`���Ή�----->
            If SiyouFlg <> "0" Then
                fn_File_Watch()
            End If
            '-----------------------------------<

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        End SyncLock
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

        tmrLoop.Enabled = True

    End Sub
#End Region

#Region "�֐�"
    Function fn_SET_GAMEN() As Boolean
        '=====================================================================================
        'NAME           :fn_SET_GAMEN
        'Parameter      :aintPAGE_NO�F�y�[�W�ԍ�
        'Description    :aintPAGE_NO�y�[�W�̉�ʕ\��
        'Return         :True=OK(����I��),False=NG�i���s�j
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        fn_SET_GAMEN = False

        If ListView1.Columns.Count = 0 Then
            ListView1.CheckBoxes = True
            With Me.ListView1
                .Clear()
                .Columns.Add("", 22, HorizontalAlignment.Left)
                .Columns.Add("���s��", 80, HorizontalAlignment.Center)
                .Columns.Add("�ʔ�", 50, HorizontalAlignment.Right)
                .Columns.Add("  �����Ɩ�", 90, HorizontalAlignment.Left)
                .Columns.Add("�J�n����", 70, HorizontalAlignment.Center)
                .Columns.Add("�I������", 70, HorizontalAlignment.Center)
                .Columns.Add("�����R�[�h�E�ϑ��Җ�", 190, HorizontalAlignment.Left)
                .Columns.Add("���l", 500, HorizontalAlignment.Left)
                .Columns.Add("�ޮ��ID", 70, HorizontalAlignment.Left)
                .Columns.Add("�p�����[�^", 90, HorizontalAlignment.Left)
                .Columns.Add("STS", 30, HorizontalAlignment.Left)
            End With
        End If

        If ListView2.Columns.Count = 0 Then
            ListView2.CheckBoxes = True
            With Me.ListView2
                .Clear()
                .Columns.Add("", 22, HorizontalAlignment.Left)
                .Columns.Add("���s��", 80, HorizontalAlignment.Center)
                .Columns.Add("�ʔ�", 50, HorizontalAlignment.Right)
                .Columns.Add("  �����Ɩ�", 90, HorizontalAlignment.Left)
                .Columns.Add("�J�n����", 70, HorizontalAlignment.Center)
                .Columns.Add("�I������", 70, HorizontalAlignment.Center)
                .Columns.Add("�����R�[�h�E�ϑ��Җ�", 190, HorizontalAlignment.Left)
                .Columns.Add("���l", 500, HorizontalAlignment.Left)
                .Columns.Add("�ޮ��ID", 70, HorizontalAlignment.Left)
                .Columns.Add("�p�����[�^", 90, HorizontalAlignment.Left)
                .Columns.Add("STS", 30, HorizontalAlignment.Left)
            End With
        End If

        ' DB�ڑ��m�F
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        '�W���u�̏��Ԃ��~����
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraTori As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
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
                        '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        'Dim OraTori As New CASTCommon.MyOracleReader(OraMain)
                        OraTori = New CASTCommon.MyOracleReader(OraMain)
                        '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        SQL.Length = 0
                        SQL.Append("SELECT TORIS_CODE_T || '-' || TORIF_CODE_T TORI_CODE")
                        SQL.Append(", ITAKU_NNAME_T")

                        '�W���uID�Ŏ擾����}�X�^�𔻒肷��
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
                    '����I���ƃL�����Z���݂̂���i�ɕ\�L����B�N���҂��Ə������͉��i�ɕ\�L����B
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
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("fn_SET_GAMEN", "���s", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Console.WriteLine(ex.Message)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraTori Is Nothing Then
                OraTori.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        End Try

        fn_SET_GAMEN = True
        Exit Function
    End Function

    Function fn_YOMIKAE_JYOUKYO(ByVal astrSTATUS As String, ByRef astrJYOUKYOU_COLOR As Color) As String
        '=====================================================================================
        'NAME           :fn_YOMIKAE_JYOUKYO
        'Parameter      :astrSTATUS�FJOBMAST.STATUS_J�̒l�^astrJYOUKYOU_COLOR�F���x���̕����̐F
        'Description    :astrSTATUS��ǂݑւ���
        'Return         :�ǂݑւ����l
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        Select Case astrSTATUS
            Case "0"
                fn_YOMIKAE_JYOUKYO = "�N���҂�"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "1"
                fn_YOMIKAE_JYOUKYO = "������"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "2"
                fn_YOMIKAE_JYOUKYO = "����I��"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "3"
                '2011/05/25 �ُ�I�����������\���ɂ���(�v���Z�X���N�����Ă�����)
                'fn_YOMIKAE_JYOUKYO = "�ُ�I��"
                fn_YOMIKAE_JYOUKYO = "������"
                astrJYOUKYOU_COLOR = strERR_COLOR
            Case "4"
                fn_YOMIKAE_JYOUKYO = "��ݾ�"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
            Case "7"
                '2011/05/25 �N�����s���ُ�I���\���ɂ���(�v���Z�X���I�����Ă�����)
                'fn_YOMIKAE_JYOUKYO = "�N�����s"
                fn_YOMIKAE_JYOUKYO = "�ُ�I��"
                astrJYOUKYOU_COLOR = strERR_COLOR
            Case "8"
                fn_YOMIKAE_JYOUKYO = "ABEND"
                astrJYOUKYOU_COLOR = strERR_COLOR
            Case Else
                fn_YOMIKAE_JYOUKYO = "���̑�"
                astrJYOUKYOU_COLOR = strBLACK_COLOR
        End Select
    End Function

    Function fn_CHANGE_TIME(ByVal astrTIME As String) As String
        '=====================================================================================
        'NAME           :fn_CHANGE_TIME
        'Parameter      :astrTIME�F���ԁi6���j
        'Description    :astrTIME��"�F"��؂�ɕϊ�
        'Return         :astrTIME��"�F"��؂�ɕϊ������l
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        fn_CHANGE_TIME = astrTIME.Substring(0, 2) & ":" & astrTIME.Substring(2, 2) & ":" & astrTIME.Substring(4, 2)
    End Function

    Function fn_INI_READ() As Boolean
        '============================================================================
        'NAME           :fn_INI_READ
        'Parameter      :
        'Description    :FSKJ.INI�t�@�C���̓ǂݍ���
        'Return         :True=OK(����),False=NG�i���s�j
        'Create         :2004/10/05
        'Update         :
        '============================================================================
        fn_INI_READ = False

        gastrBATCH_SV = CASTCommon.GetFSKJIni("COMMON", "BATCHSV")
        If gastrBATCH_SV = "err" OrElse gastrBATCH_SV = "" Then
            MessageBox.Show(String.Format(MSG0001E, "�o�b�`�T�[�o��", "COMMON", "BATCHSV"), _
                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        gastrJOB_LOOPTIME = CASTCommon.CAInt32(CASTCommon.GetFSKJIni("COMMON", "KANSHITIME"))
        If gastrBATCH_SV = "err" OrElse gastrBATCH_SV = "" Then
            MessageBox.Show(String.Format(MSG0001E, "�Ď����[�v����", "COMMON", "KANSHITIME"), _
                      msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        gastrJOB_MULTI = CASTCommon.CAInt32(CASTCommon.GetFSKJIni("COMMON", "JOBMULTI"))
        '*** Str Upd 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***
        'If gastrJOB_MULTI = 0 Then
        If gastrJOB_MULTI <= 0 Or gastrJOB_MULTI > 100 Then
            '*** End Upd 2015/12/01 SO)�r�� for �W���u���d���s�Ή� ***
            MessageBox.Show(String.Format(MSG0001E, "�W���u���d�x", "COMMON", "JOBMULTI"), _
                   msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ExePath = CASTCommon.GetFSKJIni("COMMON", "EXE")
        If ExePath = "err" OrElse ExePath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " ���s�t�H���_", "COMMON", "EXE"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '2012/06/30 �W���Ł@Web�`���Ή�----------------------------------------------------------->
        strFilePath = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV")
        If strFilePath = "err" OrElse strFilePath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " WEB�`����M�t�H���_", "WEB_DEN", "WEB_REV"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        strFileBkupPath = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV_BK")
        If strFileBkupPath = "err" OrElse strFileBkupPath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " WEB�`����M�o�b�N�A�b�v�t�H���_", "WEB_DEN", "WEB_REV_BK"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        strFileSendBkupPath = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_SED_BK")
        If strFileSendBkupPath = "err" OrElse strFileSendBkupPath = "" Then
            MessageBox.Show(String.Format(MSG0001E, " WEB�`�����M�o�b�N�A�b�v�t�H���_", "WEB_DEN", "WEB_SED_BK"), _
               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        SiyouFlg = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_DENSO")
        If SiyouFlg = "err" OrElse SiyouFlg = "" Then
            MessageBox.Show(String.Format(MSG0001E, "WEB�`���g�p�t���O", "WEB_DEN", "WEB_DENSO"), _
                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        '---------------------------------------------------------------------------------------------<

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            mLockWaitTime = CInt(sWork)
            If mLockWaitTime <= 0 Then
                mLockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

        fn_INI_READ = True
    End Function
    Function fn_CREATE_JOB() As Boolean
        '=====================================================================================
        'NAME           :fn_CREATE_JOB
        'Parameter      :
        'Description    :JOBMAST���������A�N���҂��̃W���u�𔭍s����
        'Return         :True=OK(����I��),False=NG�i���s�j
        'Create         :2004/10/05
        'Update         :
        '=====================================================================================
        fn_CREATE_JOB = False

        If strPC_NAME.Trim <> gastrBATCH_SV Then
            ' �[�����[�h
            tmrLoop.Enabled = False

            Call UpdateDisplayOnly()

            tmrLoop.Enabled = True
            Return True
        End If

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        ' DB�ڑ��m�F
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
                '�Ǎ��̂�
                While (OraReader.EOF = False)
                    intMISYORI_COUNT += 1

                    If intMISYORI_COUNT > gastrJOB_MULTI Then
                        Exit While
                    End If

                    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
                    ' �X���b�h�g�p�Ǘ��e�[�u���̋󂫃C���f�b�N�X�ԍ����擾
                    Dim threadTblIdx As Integer = getFreeThreadTblIdx()
                    ' �󂫂������ꍇ
                    If threadTblIdx = -1 Then
                        Exit While
                    End If
                    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***

                    '--------------------------------------
                    '�W���u�}�X�^�iJOBMAST�j�̍X�V
                    '--------------------------------------
                    Dim ExeName As String
                    Try
                        strERR_JOB = " "
                        If fn_JOBMAST_UPDATE("1", strERR_JOB, OraReader.GetInt("TUUBAN_J")) = False Then
                            Exit Function
                        End If

                        ExeName = "KF" & OraReader.GetItem("JOBID_J").TrimEnd & ".EXE"
                        If OraReader.GetItem("JOBID_J").Trim = "" Then
                            ExeName = "���s�t�@�C��"
                        End If
                        If File.Exists(Path.Combine(ExePath, ExeName)) = False Then
                            If fn_JOBMAST_UPDATE("7", ExeName & "�Ȃ�", OraReader.GetInt("TUUBAN_J")) = False Then
                                Exit Function
                            End If

                            intMISYORI_COUNT -= 1
                        Else
                            ' ���s
                            Dim Para As String = OraReader.GetItem("PARAMETA_J")
                            '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
                            'If JobExecute(CAInt32(OraReader.GetItem("TUUBAN_J")), ExeName, Para) = False Then
                            '    ' �X���b�h�ɋ󂫂��Ȃ������̂ŁC���Ƃ��������x���s����
                            '    Call fn_JOBMAST_UPDATE("0", "���g���C", OraReader.GetInt("TUUBAN_J"))
                            '    Exit Function
                            'End If
                            JobExecute(threadTblIdx, CAInt32(OraReader.GetItem("TUUBAN_J")), ExeName, Para)
                            '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
                        End If
                    Catch ex As Exception
                        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                        BatchLog.Write_Err("fn_CREATE_JOB", "���s", ex)
                        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                        Call fn_JOBMAST_UPDATE("0", "���g���C", OraReader.GetInt("TUUBAN_J"))
                        Exit Function
                    End Try

                    OraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("fn_CREATE_JOB", "���s", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

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
        'Parameter      :astrSTATUS�F�X�e�[�^�X�i0=�N���҂��A1=�������j�^astrERRMSG�F�G���[���b�Z�[�W
        '               :intTUUBAN�F�ʔ�
        'Description    :JOBMAST�Ɍ��ʍX�V
        'Return         :True=OK(����),False=NG�i���s�j
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

        ' DB�ڑ��m�F
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        Try
            OraMain.ExecuteNonQuery(SQL)
            OraMain.Commit()
            OraMain.BeginTrans()
        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("fn_JOBMAST_UPDATE", "���s", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            OraMain.Rollback()
            OraMain.BeginTrans()
            MessageBox.Show(MSG0014E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End Try

        fn_JOBMAST_UPDATE = True
    End Function

    '*** Str Del 2015/12/01 SO)�r�� for ���g�p���\�b�h�폜 ***
    '    Public Function fn_JOBMAST_UPDATE_END(ByVal astrSTATUS As String, ByVal astrERRMSG As String, ByVal intTUUBAN As Integer) As Boolean
    '        '============================================================================
    '        'NAME           :fn_JOBMAST_UPDATE_END
    '        'Parameter      :astrSTATUS�F�X�e�[�^�X�i0=�N���҂��A1=�������j�^astrERRMSG�F�G���[���b�Z�[�W
    '        '               :intTUUBAN�F�ʔ�
    '        'Description    :JOBMAST�Ɍ��ʍX�V
    '        'Return         :True=OK(����),False=NG�i���s�j
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
    '        ' DB�ڑ��m�F
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
    '*** End Del 2015/12/01 SO)�r�� for ���g�p���\�b�h�폜 ***

    Function fn_SET_JOBNAME(ByVal astrJOB_ID As String) As String
        '============================================================================
        'NAME           :fn_SET_JOBNAME
        'Parameter      :astrJOB_ID�F�W���u�h�c
        'Description    :�W���u�h�c���W���u���ɕϊ�
        'Return         :�W���u��
        'Create         :2004/10/06
        'Update         :
        '============================================================================

        fn_SET_JOBNAME = ""

        '2016/10/06 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
        If Not TxtJob Is Nothing AndAlso TxtJob.Length > 0 Then
            For i As Integer = 0 To TxtJob.Length - 1
                If astrJOB_ID = TxtJob(i).JOBID Then
                    Return TxtJob(i).JOBNAME.Trim
                End If
            Next
        End If
        '2016/10/06 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END

        Select Case astrJOB_ID.Substring(0, 1)

            Case "J"    '���U
                Select Case astrJOB_ID.TrimEnd
                    Case "J010"
                        fn_SET_JOBNAME = "���U����"
                    Case "J011"
                        fn_SET_JOBNAME = "�������ڗ���"
                    Case "J020"
                        fn_SET_JOBNAME = "���s"
                    Case "J021"
                        fn_SET_JOBNAME = "���s"
                    Case "J030"
                        fn_SET_JOBNAME = "�z�M"
                    Case "J040"
                        fn_SET_JOBNAME = "�s�\"
                    Case "J050"
                        fn_SET_JOBNAME = "����"
                    Case "J060"
                        fn_SET_JOBNAME = "�Ԋ�"
                    Case "J070"
                        fn_SET_JOBNAME = "��v"
                    Case "J080"
                        fn_SET_JOBNAME = "�ĐU"
                    Case "J090"
                        fn_SET_JOBNAME = "���U�_��"
                    Case "J100"
                        fn_SET_JOBNAME = "���U�_�񌋉�"
                    Case Else
                        fn_SET_JOBNAME = "���U���̑�"
                End Select

            Case "S"    '���U
                Select Case astrJOB_ID.TrimEnd
                    Case "S010"
                        fn_SET_JOBNAME = "���U����"
                    Case "S020"
                        fn_SET_JOBNAME = "�����m��"
                    Case "S030"
                        fn_SET_JOBNAME = "�����m�ی���"
                        '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- START
                    Case "S040", "S045"
                        'Case "S040"
                        '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- END
                        fn_SET_JOBNAME = "���M"
                    Case "S050"
                        fn_SET_JOBNAME = "���M����"
                    Case "S060"
                        fn_SET_JOBNAME = "�ב֐���"
                    Case "S070"
                        fn_SET_JOBNAME = "�ב֐�������"
                    Case Else
                        fn_SET_JOBNAME = "���U���̑�"
                End Select

            Case "G"    '�w�Z
                Select Case astrJOB_ID.TrimEnd
                    Case "S010"
                        fn_SET_JOBNAME = "�w�Z����"
                    Case Else
                        fn_SET_JOBNAME = "�w�Z���̑�"
                End Select

            Case "K"    '����
                Select Case astrJOB_ID.TrimEnd
                    Case "K010"
                        fn_SET_JOBNAME = "��������"
                    Case "K020"
                        fn_SET_JOBNAME = "�������ό���"
                    Case Else
                        fn_SET_JOBNAME = "���ς��̑�"
                End Select

            Case "U"    '�^�p�Ǘ�
                Select Case astrJOB_ID.TrimEnd
                    Case "U010"
                        fn_SET_JOBNAME = "���Z�@�֍X�V"
                    Case Else
                        fn_SET_JOBNAME = "�^�ǂ��̑�"
                End Select

            Case "T"    '���V�X�A�g
                Select Case astrJOB_ID.TrimEnd
                    Case "T010"
                        fn_SET_JOBNAME = "���V�X�A�g"
                End Select

                '2012/06/30 �W���Ł@WEB�`���Ή�----->
            Case "W"    'WEB�`���A�g
                Select Case astrJOB_ID.TrimEnd
                    Case "W010"
                        fn_SET_JOBNAME = "WEB�`���A�g"
                End Select
                '-----------------------------------<
            Case Else
                ' 2016/09/28 �^�X�N�j���� CHG �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(UI_12-10) -------------------- START
                Dim RetJobName As String = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�W���u���Ǘ�.TXT"), astrJOB_ID.TrimEnd)
                If RetJobName.Trim = "" Then
                    fn_SET_JOBNAME = "�s��"
                Else
                    fn_SET_JOBNAME = RetJobName
                End If
                'fn_SET_JOBNAME = "�s��"
                ' 2016/09/28 �^�X�N�j���� CHG �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(UI_12-10) -------------------- END
        End Select

    End Function
#End Region

#Region "Web�`���Ή�"
    '�t�@�C���Ď�
    Private Function fn_File_Watch() As Boolean
        '============================================================================
        'NAME           :fn_File_Watch
        'Parameter      :
        'Description    :�t�@�C���Ď������iWeb�`���Ή��j
        'Return         :True=OK(����I��),False=NG�i���s�j
        'Create         :2012/06/30
        'Update         :
        '============================================================================
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        '�[�����[�h
        If strPC_NAME.Trim <> gastrBATCH_SV Then
            tmrLoop.Enabled = False
            Call UpdateDisplayOnly()
            tmrLoop.Enabled = True
            Return True
        End If

        Try
            '--------------------------------------
            'Web�`���t�@�C���Ď�
            '--------------------------------------
            If Dir(strFilePath & "END" & "*") <> "" Then
                '���V�X�A�g�����o�^
                Dim FileName As String
                Dim ret As String = ""
                Dim sr As StreamReader = Nothing

                Try
                    '�Ď��t�H���_���̃t�@�C�������擾
                    FileName = Dir(strFilePath & "END_" & "*")

                    '�Q�d�N���h�~�̂��߁A�^�����END�t�@�C�����ړ�
                    If File.Exists(strFileBkupPath & FileName) Then
                        File.Delete(strFileBkupPath & FileName)
                    End If

                    File.Move(strFilePath & FileName, strFileBkupPath & FileName)

                    '2012/12/05 saitou WEB�`�� UPD -------------------------------------------------->>>>
                    'END�t�@�C�����猏���Ƌ��z���擾����B
                    Dim strEndKen As String = String.Empty
                    Dim strEndKin As String = String.Empty
                    If File.Exists(Path.Combine(strFileBkupPath, FileName)) = True Then
                        'END�t�@�C�������݂���ꍇ�A�t�@�C�����J���Č����Ƌ��z���擾����B
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
                    '2012/12/05 saitou WEB�`�� UPD --------------------------------------------------<<<<

                    'FileArray(0) = ���[�U��
                    'FileArray(1) = ���t�@�C����
                    'FileArray(2) = �z�M��
                    'FileArray(3) = �z�M����
                    Dim FileArray() As String = FileName.Substring(4).Split("_"c)
                    Dim FileExt As String = Path.GetExtension(FileArray(1))

                    If FileExt <> "" Then   '�g���q���Ȃ��ꍇ�́A�������Ȃ�

                        Dim GCOM As New MenteCommon.clsCommon
                        Dim TXTFileName As String = Path.Combine(GCOM.GetTXTFolder, "WEB�`���t�@�C���Ǘ�.txt")

                        sr = New StreamReader(TXTFileName, Encoding.GetEncoding(932))
                        Dim ReadLine As String = ""

                        While sr.EndOfStream = False
                            ReadLine = sr.ReadLine()
                            If Not ReadLine Is Nothing Then
                                Dim Item As String() = ReadLine.Split(","c)

                                If FileExt.Substring(1).ToUpper = Item(0).ToUpper Then
                                    ret = Item(1)
                                    '�t�@�C���ړ�
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

                        '20130607 �t�@�C�����󔒍��ݑΉ�
                        '2012/12/05 saitou WEB�`�� UPD -------------------------------------------------->>>>
                        '���s�p�����[�^�Ɍ����Ƌ��z��ǉ�����B
                        strPara = """" & strFilePath & FileName.Substring(4) & """"
                        strPara &= "," & strEndKen & "," & strEndKin
                        'strPara = strFilePath & FileName.Substring(4) & "," & strEndKen & "," & strEndKin
                        'strPara = strFilePath & FileName.Substring(4)
                        '2012/12/05 saitou WEB�`�� UPD --------------------------------------------------<<<<
                        '20130607 �t�@�C�����󔒍��ݑΉ�

                        Thread.Sleep(3000)

                        '2016/12/06 saitou RSV2 UPD �����e�i���X ---------------------------------------- START
                        '�p�X�Œ��p�~���AEXE�p�X��ݒ�t�@�C������擾����悤�ɏC��
                        Dim ExePath As String = CASTCommon.GetFSKJIni("COMMON", "EXE")
                        If ExePath = String.Empty OrElse ExePath = Nothing Then
                            '�擾�ł��Ȃ������ꍇ�͍��܂Œʂ�̐ݒ�
                            ExePath = "C:\RSKJ\EXE\"
                        End If
                        Dim ExecProc As Process = Process.Start(Path.Combine(ExePath, "KFW010.EXE"), strPara)
                        'Dim ExecProc As Process = Process.Start("C:\RSKJ\EXE\KFW010.EXE", strPara)
                        '2016/12/06 saitou RSV2 UPD ----------------------------------------------------- END

                        If ExecProc.Id <> 0 Then
                            '�I���ҋ@
                            ExecProc.WaitForExit()
                        Else
                            Throw New Exception(String.Format("�A�v���P�[�V�����̋N���Ɏ��s���܂����B'{0}'", "KFW010.EXE"))
                        End If
                        ExecProc.Close()
                        ExecProc.Dispose()

                    End If

                Catch ex As Exception
                    '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
                    'BatchLog.Write("Web�`���t�@�C���Ď�", "���s", ex.Message)
                    BatchLog.Write_Err("Web�`���t�@�C���Ď�", "���s", ex)
                    '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
                    MessageBox.Show("WEB�`���t�@�C���̏����Ɏ��s���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Finally
                    If Not sr Is Nothing Then
                        sr.Close()
                        sr = Nothing
                    End If
                End Try
            End If

            '--------------------------------------
            'Web�`�����M�t�@�C���Ď�
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
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("Web�`���t�@�C���Ď�", "���s", ex.Message)
            BatchLog.Write_Err("Web�`���t�@�C���Ď�", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            MessageBox.Show("WEB�`���t�@�C���̏����Ɏ��s���܂���", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            tmrLoop.Enabled = True
        End Try


    End Function
#End Region

#Region "�I���{�^��"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        ' 2016/06/02 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- START
        BatchLog.Write("0000000000-00", "00000000", "�Ɩ��I��", "")
        ' 2016/06/02 �^�X�N�j���� ADD �yOM�zUI_B-99-99(RSV2�Ή�) -------------------- END

        Me.Close()
    End Sub

#End Region

    Protected Overrides Sub Finalize()
        '2008/08/15 �ُ�I���l��
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

        ' DB�ڑ��m�F
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
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("UpdateGamen", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                Return True
            Finally
                OraReader.Close()
            End Try

            ' �ʏ탊�X�g����C�W���u�ʔԂ�{��
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
                        ' �ُ�̏ꍇ�́CListView2�ֈړ�
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

            ' �ُ탊�X�g����C�W���u�ʔԂ�{��
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
                        ' �ُ�łȂ���΁CListView1�ֈړ�
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
            '�y���Y�^�z
            ' VB.Net�ł̃f�o�b�O���Ɉȉ��̗�O�����i������ThreadMethodDelegate�����ł����l�j
            '  �u�L���ł͂Ȃ��X���b�h�Ԃ̑���: �R���g���[�����쐬���ꂽ�X���b�h�ȊO�̃X���b�h����R���g���[�� 'ListView2' ���A�N�Z�X����܂����v
            ' �Ȃ��A���EXE����̎��s���ɂ͗�O�͔������Ȃ�
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
#If Not Debug Then
                BatchLog.Write_Err("UpdateGamen", "���s", exAll)
#End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

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
            ' �ʏ탊�X�g����C�W���u�ʔԂ�{��
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
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("UpdateGamenItem1", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

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
                ' �ُ�̏ꍇ�́CListView2�ֈړ�
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
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("UpdateGamenItem1", "���s", exAll)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

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
            ' �ʏ탊�X�g����C�W���u�ʔԂ�{��
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
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("UpdateGamenItem2", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

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

            ' �ُ탊�X�g����C�W���u�ʔԂ�{��
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
                ' �ُ�łȂ���΁CListView1�ֈړ�
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
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("UpdateGamenItem2", "���s", exAll)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

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

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            Dim OraReader As CASTCommon.MyOracleReader = Nothing
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
            Dim dblock As CASTCommon.CDBLock = New CASTCommon.CDBLock
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

            ' DB�ڑ��m�F
            Do
                DBConnectJudge()
            Loop Until ConnectStopFlag = False

            Try
                Dim FoundItem As New ArrayList

                For i As Integer = 0 To AutoList.Count - 1
                    Dim AutoKey As String() = CType(AutoList(i), String).Split("="c)

                    If AutoKey(2) <= DateTime.Now.ToString("HHmm") AndAlso DateTime.Now.ToString("HHmm") <= AutoKey(3) Then

                        ' �N�����Ԕ͈͓��̏ꍇ�C�N������
                        Dim SQL As New StringBuilder(128)
                        SQL.Append("SELECT STA_TIME_J FROM JOBMAST")
                        SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                        SQL.Append("   AND JOBID_J = '" & AutoKey(1) & "'")
                        SQL.Append("   AND (STATUS_J = '0'")
                        SQL.Append("    OR  STA_TIME_J BETWEEN '" & AutoKey(2) & "00' AND '" & AutoKey(3) & "99')")

                        '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                        'Dim OraReader As New CASTCommon.MyOracleReader(OraMain)
                        OraReader = New CASTCommon.MyOracleReader(OraMain)
                        '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                        If OraReader.DataReader(SQL) = False Then
                            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
                            ' �W���u�o�^���b�N
                            If dblock.InsertJOBMAST_Lock(OraMain, mLockWaitTime) = False Then
                                Throw New Exception("�W���u�o�^�^�C���A�E�g")
                            End If
                            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

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

                            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
                            ' �W���u�o�^�A�����b�N
                            dblock.InsertJOBMAST_UnLock(OraMain)
                            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

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
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                '�y���Y�^�z
                ' ini�t�@�C����[AUTO]�̒�`���i=���Ȃ��j���Ɉȉ��̗�O����
                '  �u�C���f�b�N�X���z��̋��E�O�ł��v
                BatchLog.Write_Err("TmrUpdate_Tick", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                End If
                '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***
                ' �W���u�o�^�A�����b�N
                dblock.InsertJOBMAST_UnLock(OraMain)

                ' ���[���o�b�N
                OraMain.Rollback()
                OraMain.BeginTrans()
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�W���u�o�^���̈�Ӑ��ۏ؁j ***

            End Try

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        End SyncLock
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

        If AutoList.Count > 0 Then
            TmrUpdate.Enabled = True
        End If
    End Sub

    ' �ڍ׏��t�H�[�����J��
    Private Sub ListView1_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles ListView1.ItemCheck
        Try
            If e.NewValue = CheckState.Checked Then
                mVisibleFlag = True

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
                SyncLock mThreadLock
                    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

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

                    ' DB�ڑ��m�F
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

                    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
                End SyncLock
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

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
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("�ڍ׏��t�H�[��", "���s", ex.ToString)
            BatchLog.Write_Err("�ڍ׏��t�H�[��", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try

    End Sub

    ' �ڍ׏��t�H�[�����J��
    Private Sub ListView2_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles ListView2.ItemCheck
        Try
            If e.NewValue = CheckState.Checked Then
                mVisibleFlag = True

                '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
                SyncLock mThreadLock
                    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

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

                    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
                End SyncLock
                '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

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
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'BatchLog.Write("�ڍ׏��t�H�[��", "���s", ex.ToString)
            BatchLog.Write_Err("�ڍ׏��t�H�[��", "���s", ex)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
        End Try
    End Sub

    '*** Str Del 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
    ' �߂�l�ƃp�����[�^�̂���f���Q�[�g
    'Delegate Function ThreadMethodDelegate(ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Integer '
    'Shared threadMethodDelegateInstance() As ThreadMethodDelegate '
    '*** End Del 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
    Shared ExePath As String

    '*** Str Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
    'Private Function JobExecute(ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Boolean

    '    Try
    '        Dim EXEFlag As Boolean = False
    '        For syoriCount As Integer = 0 To threadMethodDelegateInstance.Length - 1
    '            If threadMethodDelegateInstance(syoriCount) Is Nothing OrElse threadMethodDelegateInstance(syoriCount).Target Is Nothing Then

    '                threadMethodDelegateInstance(syoriCount) _
    '                  = New ThreadMethodDelegate(AddressOf ThreadMethod) '

    '                ' �f���Q�[�g�ɂ��X���b�h�����Ăяo��
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

    '' �ʃX���b�h�ŌĂяo����郁�\�b�h
    'Private Shared Function ThreadMethod(ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Integer
    '    '--------------------------------------
    '    '�W���u�̔��s
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

    '' �X���b�h�����I����ɌĂяo�����R�[���o�b�N�E���\�b�h
    'Private Sub MyCallback(ByVal ar As IAsyncResult)  '
    '    Dim JobTuuban As Integer

    '    ' �W���u�ʔԎ��o��
    '    Dim delG As ThreadMethodDelegate = CType(ar.AsyncState, ThreadMethodDelegate)
    '    JobTuuban = delG.EndInvoke(ar)

    '    Call AtoSyori(JobTuuban)
    'End Sub


    ' �@�\�@ �F �W���u��ʃX���b�h�Ŏ��s����
    ' �����@ �F db �FDB�R�l�N�V����
    '           waittime �F �҂����ԁi�b�j
    '           waittime �F �҂����ԁi�b�j
    '           waittime �F �҂����ԁi�b�j
    ' ���A�l �F True�̂�
    ' ���l�@ �F �]���́AJobExecute���\�b�h���ŃX���b�h�e�[�u���ɋ󂫂��Ȃ��ꍇ��Flase��Ԃ��Ă������A
    '           �X���b�h�e�[�u���̋󂫃`�F�b�N���ďo�����ōs���悤�ɕύX��������True�݂̂�Ԃ�
    Private Function JobExecute(ByVal useTblIdx As Integer, ByVal tuuban As Integer, ByVal exe As String, ByVal para As String) As Boolean

        Try
            ' �X���b�h�g�p�Ǘ��e�[�u�����g�p���ɂ���
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
            ' �X���b�h�g�p�Ǘ��e�[�u���𖢎g�p�ɂ���
            mThreadUseTbl(useTblIdx) = 0
            mThreadClsTbl(useTblIdx) = Nothing

            BatchLog.Write_Err("�W���u���s", "���s", ex)
            Console.WriteLine(ex.Message)

            Call AtoSyori(tuuban)

            Return True
        End Try

    End Function

    '----------------------------------------------------------
    ' �X���b�h���s�C���i�[�N���X
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
            '�W���u�̔��s
            '--------------------------------------
            Try
                If frm.BatchLog.IS_LEVEL2() = True Then
                    sw = frm.BatchLog.Write_Enter2("�W���u���s�X���b�h", SInfo.FileName & " " & SInfo.Arguments)
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
                        ' �W���u�I����̌㏈��
                        Call frm.AtoSyori(tuuban)
                    End SyncLock

                Else
                    Throw New Exception(SInfo.FileName & "�̎��s�Ɏ��s���܂����B")
                End If

            Catch ex As Exception
                ' ��~�t���O���n�e�e�̏ꍇ
                If mStopFlg = False Then
                    frm.BatchLog.Write_LEVEL1("�W���u���s�X���b�h", "���s", SInfo.FileName & " " & SInfo.Arguments)
                    frm.BatchLog.Write_Err("�W���u���s�X���b�h", "���s", ex)
                    frm.fn_JOBMAST_UPDATE("7", ex.Message, tuuban)
                End If

            Finally
                If frm.BatchLog.IS_LEVEL2() = True Then
                    frm.BatchLog.Write_Exit2(sw, "�W���u���s�X���b�h")
                End If

                SyncLock mThreadLock
                    ' �X���b�h�g�p�Ǘ��e�[�u���𖢎g�p�ɂ���
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
    '*** End Upd 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***


    Private Sub AtoSyori(ByVal tuuban As Integer)
        Dim StatusJ As String = ""
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        ' DB�ڑ��m�F
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
                '2011/05/25 �ُ�I���̏ꍇ���܂߂�
                'If StatusJ = "1" Then
                If StatusJ = "1" OrElse StatusJ = "3" Then
                    ' �X���b�h���I�������ɂ�������炸�C�������̏ꍇ�́C�������s�Ƃ݂Ȃ�
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE JOBMAST SET STATUS_J='7'")
                    SQL.Append(" WHERE TOUROKU_DATE_J = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    SQL.Append("   AND TUUBAN_J = " & tuuban.ToString)
                    '2011/05/25 �ُ�I���̏ꍇ���܂߂�
                    'SQL.Append("   AND STATUS_J = '1'")
                    SQL.Append("   AND STATUS_J IN ('1','3')")
                    Call OraMain.ExecuteNonQuery(SQL)
                    OraMain.Commit()

                    OraMain.BeginTrans()
                End If
            End If
        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("AtoSyori", "���s", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Return
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

        '2011/05/25 �v���Z�X�I����͉�ʍX�V������
        Call UpdateGamen(tuuban)
    End Sub

    ' �ŐV��ԕ\��
    Private Sub BtnRefresh_Click1(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnRefresh.Click

        Me.SuspendLayout()

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        SyncLock mThreadLock

            ' ����GC
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

            mNowMaxTuuban = 0
            Try
                '*** Str Del 2015/12/01 SO)�r�� for ���ݏ�Q�i�X�V�{�^���������ɃC���f�b�N�s�v�ƂȂ�j ***
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
                '*** End Del 2015/12/01 SO)�r�� for ���ݏ�Q�i�X�V�{�^���������ɃC���f�b�N�s�v�ƂȂ�j ***

                ListView1.Clear()
                ListView2.Clear()

                Call fn_SET_GAMEN()

                '*** Str Add 2015/12/01 SO)�r�� for ���ݏ�Q�i�X�V�{�^���������ɃC���f�b�N�s�v�ƂȂ�j ***
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
                '*** End Add 2015/12/01 SO)�r�� for ���ݏ�Q�i�X�V�{�^���������ɃC���f�b�N�s�v�ƂȂ�j ***

                If ListTop1 >= 0 Then
                    ListView1.Items(ListTop1).Selected = True
                End If
                If ListTop2 >= 0 Then
                    ListView2.Items(ListTop2).Selected = True
                End If
            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("BtnRefresh_Click1", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                Console.WriteLine(ex.Message)
            End Try

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        End SyncLock
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

        Me.ResumeLayout(True)
    End Sub

    Private Sub UpdateDisplayOnly()
        ' DB�ڑ��m�F
        Do
            DBConnectJudge()
        Loop Until ConnectStopFlag = False

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        Try
            Dim SQL As New StringBuilder(128)
            '*** Str Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            'Dim OraReader As New CASTCommon.MyOracleReader(OraMain)
            OraReader = New CASTCommon.MyOracleReader(OraMain)
            '*** End Upd 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

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
            '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("UpdateDisplayOnly", "���s", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Console.WriteLine(ex.Message)
        Finally
        End Try
    End Sub

    Private Sub StartGamen()
        ListView1.Clear()
        ListView2.Clear()

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

            ' DB�ڑ��m�F
            Do
                DBConnectJudge()
            Loop Until ConnectStopFlag = False

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '��ʂ̕\��
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '*** Str Del 2015/12/01 SO)�r�� for �s�vDB�R�l�N�V�����폜 ***
            'OraMain = New CASTCommon.MyOracle
            '*** End Del 2015/12/01 SO)�r�� for �s�vDB�R�l�N�V�����폜 ***
            Dim SQL As New StringBuilder(128)

            ' �O���܂ł̋N���܂��ƂȂ��Ă���W���u���L�����Z������
            SQL = New StringBuilder(128)
            SQL.Append("UPDATE JOBMAST SET")
            SQL.Append(" STATUS_J='4'")
            SQL.Append(",ERRMSG_J='�O�����N���������L�����Z�����܂���'")
            SQL.Append(" WHERE TOUROKU_DATE_J < TO_CHAR(SYSDATE,'YYYYMMDD')")
            SQL.Append("   AND STATUS_J = '0'")

            '�ڑ����s���l��
            Try
                Call OraMain.ExecuteNonQuery(SQL)
                OraMain.Commit()

                OraMain.BeginTrans()

            Catch ex As Exception
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("StartGamen", "���s", ex)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                blnDB_CONNECT = False
                BtnRefresh.Enabled = False
                MessageBox.Show(MSG0030E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            '�����ݒ�
            lblDate.Text = CASTCommon.Calendar.Now.ToString("yyyy�NMM��dd��")
            lblNowTime.Text = System.DateTime.Now.ToString("HH:mm:ss")

            Call fn_SET_GAMEN()

            If Not FrmSub Is Nothing Then
                FrmSub.Close()
                FrmSub.Dispose()
                FrmSub = Nothing
            End If

            FrmSub = New FrmSubJob
            FrmSub.Visible = False

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
            FrmSub.ThreadLock = mThreadLock
        End SyncLock
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

    End Sub

    Private Sub DBConnectJudge()
        Dim SQL As New StringBuilder
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Threading.Thread.Sleep(50) '0.05�b�ҋ@

        If ConnectStopFlag = True Then
            Exit Sub
        End If

        ' ���̃X���b�h���珈��������Ȃ��悤�ɁC���b�N��������
        ConnectStopFlag = True

        Try
            SQL.Append("SELECT LOGINID_U FROM UIDMAST")
            OraReader = New CASTCommon.MyOracleReader(OraMain)
            If OraReader.DataReader(SQL) = False Then
                Try
                    ' DB�ăI�[�v��
                    Console.WriteLine("OPEN OPEN")
                    OraMain.Close()
                    OraMain = Nothing
                    OraMain = New CASTCommon.MyOracle
                Catch exA As Exception
                    '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                    BatchLog.Write_Err("DBConnectJudge", "���s", exA)
                    '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

                End Try
            Else
            End If
            ConnecttedFlag = True
        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("DBConnectJudge", "���s", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Try
                ' DB�ăI�[�v��
                Console.WriteLine("OPEN OPEN")
                OraMain.Close()
                OraMain = Nothing
                OraMain = New CASTCommon.MyOracle

            Catch exA As Exception
                '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
                BatchLog.Write_Err("DBConnectJudge", "���s", exA)
                '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            End Try
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

        ' ���b�N����
        ConnectStopFlag = False
    End Sub

    Private Sub TmrDisplay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TmrDisplay.Tick
        If UpdateGamenFlag = True Then
            Return
        End If

        UpdateGamenFlag = True

        TmrDisplay.Enabled = False

        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        SyncLock mThreadLock
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

            ' DB�ڑ��m�F
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

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***
        End SyncLock
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�}���`�X���b�h�Ή��j ***

        UpdateGamenFlag = False

        TmrDisplay.Enabled = True
    End Sub

    '�V�X�e�����t�̋x������
    Private Function TodayIsHoliday() As Boolean
        '�y���̏ꍇ
        If System.DateTime.Today.DayOfWeek = DayOfWeek.Sunday OrElse System.DateTime.Today.DayOfWeek = DayOfWeek.Saturday Then
            Return True
        End If

        '*** Str Upd 2015/12/01 SO)�r�� for ���ݏ�Q�i�N���x���j ***
        '����ȊO�̏ꍇ
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
            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
            BatchLog.Write_Err("TodayIsHoliday", "���s", ex)
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

            Return False

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If

        End Try
        '*** End Upd 2015/12/01 SO)�r�� for ���ݏ�Q�i�N���x���j ***

        Return True
    End Function

    'F5�L�[�ōŐV�\���@�\�ǉ�
    Private Sub FrmJOB_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F5 Then
            Me.KeyPreview = False
            Call BtnRefresh_Click1(sender, e)
            Thread.Sleep(100)
            Me.KeyPreview = True
        End If
    End Sub

    '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***
    ' �@�\�@ �F �X���b�h�g�p�Ǘ��e�[�u���̋󂫃C���f�b�N�X�ԍ���Ԃ�
    ' ���A�l �F 0�ȏ�F �󂫃C���f�b�N�X�ԍ�
    '           -1   �F �󂫖���
    Private Function getFreeThreadTblIdx() As Integer

        For idx As Integer = 0 To mThreadUseTbl.Length - 1
            ' �X���b�h�g�p�Ǘ��e�[�u���𖢎g�p�̏ꍇ
            If mThreadUseTbl(idx) = 0 Then
                If BatchLog.IS_LEVEL2() = True Then
                    BatchLog.Write_LEVEL2("getFreeThreadTblIdx", "����", "idx=" & idx)
                End If
                Return idx
            End If
        Next

        ' �󂫖���
        Return -1

    End Function
    '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή��i�n���h�����[�N����̂Ń}���`�X���b�h�������@�ύX�j ***

    '2016/10/06 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- START
    '�e�L�X�g�t�@�C�����W���u�����擾����
    Private Function getTxtToJobName() As Boolean
        Dim TXTFILE As String = System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�W���u���Ǘ�.TXT")
        Dim SR As StreamReader = Nothing
        Dim LineData As String
        Dim Count As Integer = 0

        Try
            '�t�@�C���̑��݃`�F�b�N
            If Not File.Exists(TXTFILE) Then
                '������ΐ���Ŕ�����
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
            BatchLog.Write_Err("getTxtToJobName", "���s", ex)
            Return False
        End Try
    End Function
    '2016/10/06 �^�X�N�j���� ADD �yPG�z�~�M�� �J�X�^�}�C�Y�Ή�(�W���ŉ��P) -------------------- END

End Class
