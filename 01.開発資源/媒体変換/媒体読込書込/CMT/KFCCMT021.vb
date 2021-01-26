Option Explicit On 
Option Strict On

Imports System.Data.OracleClient
Imports MenteCommon

Public Class KFCCMT021
    Inherits System.Windows.Forms.Form

    '�\�[�g�I�[�_�[�t���O
    Dim SortOrderFlag As Boolean = True

    '�N���b�N������̔ԍ�
    Dim ClickedColumn As Integer

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
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
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents LblTop As System.Windows.Forms.Label
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents LblDateCaption As System.Windows.Forms.Label
    Friend WithEvents LblUserIDCaption As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CmdWrite As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents ReceptionNo As System.Windows.Forms.ColumnHeader
    Friend WithEvents FileSEQ As System.Windows.Forms.ColumnHeader
    Friend WithEvents SyoriDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents StackerNo As System.Windows.Forms.ColumnHeader
    Friend WithEvents ItakuCD As System.Windows.Forms.ColumnHeader
    Friend WithEvents FuriDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents ItakuKana As System.Windows.Forms.ColumnHeader
    Friend WithEvents ItakuKanji As System.Windows.Forms.ColumnHeader
    Friend WithEvents SyoriKen As System.Windows.Forms.ColumnHeader
    Friend WithEvents SyoriKin As System.Windows.Forms.ColumnHeader
    Friend WithEvents ErrCD As System.Windows.Forms.ColumnHeader
    Friend WithEvents ErrorName As System.Windows.Forms.ColumnHeader
    Friend WithEvents BankCode As System.Windows.Forms.ColumnHeader
    Friend WithEvents BankName As System.Windows.Forms.ColumnHeader
    Friend WithEvents BranchCode As System.Windows.Forms.ColumnHeader
    Friend WithEvents BranchName As System.Windows.Forms.ColumnHeader
    Friend WithEvents FuriKen As System.Windows.Forms.ColumnHeader
    Friend WithEvents FuriKin As System.Windows.Forms.ColumnHeader
    Friend WithEvents FunouKen As System.Windows.Forms.ColumnHeader
    Friend WithEvents FunouKin As System.Windows.Forms.ColumnHeader
    Friend WithEvents FSyoriKbn As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToriSCode As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToriFCode As System.Windows.Forms.ColumnHeader
    Friend WithEvents StationNo As System.Windows.Forms.ColumnHeader
    Friend WithEvents WriteCounter As System.Windows.Forms.ColumnHeader
    Friend WithEvents OverrideFlg As System.Windows.Forms.ColumnHeader
    Friend WithEvents PrintButton As System.Windows.Forms.Button
    Friend WithEvents FileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents JSFlg As System.Windows.Forms.ColumnHeader
    Friend WithEvents CreateDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents ComplockFlg As System.Windows.Forms.ColumnHeader
    Friend WithEvents UpdateDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents ComboFormat As System.Windows.Forms.ComboBox
    Friend WithEvents TRTotalKen As System.Windows.Forms.ColumnHeader
    Friend WithEvents TRTotalKin As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFCCMT021))
        Me.CmdBack = New System.Windows.Forms.Button
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblUser = New System.Windows.Forms.Label
        Me.LblTop = New System.Windows.Forms.Label
        Me.LblDateCaption = New System.Windows.Forms.Label
        Me.LblUserIDCaption = New System.Windows.Forms.Label
        Me.CmdWrite = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.ReceptionNo = New System.Windows.Forms.ColumnHeader
        Me.FileSEQ = New System.Windows.Forms.ColumnHeader
        Me.SyoriDate = New System.Windows.Forms.ColumnHeader
        Me.StackerNo = New System.Windows.Forms.ColumnHeader
        Me.WriteCounter = New System.Windows.Forms.ColumnHeader
        Me.ItakuCD = New System.Windows.Forms.ColumnHeader
        Me.FuriDate = New System.Windows.Forms.ColumnHeader
        Me.ItakuKana = New System.Windows.Forms.ColumnHeader
        Me.ItakuKanji = New System.Windows.Forms.ColumnHeader
        Me.SyoriKen = New System.Windows.Forms.ColumnHeader
        Me.SyoriKin = New System.Windows.Forms.ColumnHeader
        Me.TRTotalKen = New System.Windows.Forms.ColumnHeader
        Me.TRTotalKin = New System.Windows.Forms.ColumnHeader
        Me.ErrCD = New System.Windows.Forms.ColumnHeader
        Me.ErrorName = New System.Windows.Forms.ColumnHeader
        Me.BankCode = New System.Windows.Forms.ColumnHeader
        Me.BankName = New System.Windows.Forms.ColumnHeader
        Me.BranchCode = New System.Windows.Forms.ColumnHeader
        Me.BranchName = New System.Windows.Forms.ColumnHeader
        Me.JSFlg = New System.Windows.Forms.ColumnHeader
        Me.FuriKen = New System.Windows.Forms.ColumnHeader
        Me.FuriKin = New System.Windows.Forms.ColumnHeader
        Me.FunouKen = New System.Windows.Forms.ColumnHeader
        Me.FunouKin = New System.Windows.Forms.ColumnHeader
        Me.FSyoriKbn = New System.Windows.Forms.ColumnHeader
        Me.ToriSCode = New System.Windows.Forms.ColumnHeader
        Me.ToriFCode = New System.Windows.Forms.ColumnHeader
        Me.StationNo = New System.Windows.Forms.ColumnHeader
        Me.FileName = New System.Windows.Forms.ColumnHeader
        Me.OverrideFlg = New System.Windows.Forms.ColumnHeader
        Me.ComplockFlg = New System.Windows.Forms.ColumnHeader
        Me.CreateDate = New System.Windows.Forms.ColumnHeader
        Me.UpdateDate = New System.Windows.Forms.ColumnHeader
        Me.PrintButton = New System.Windows.Forms.Button
        Me.ComboFormat = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'CmdBack
        '
        Me.CmdBack.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CmdBack.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!)
        Me.CmdBack.Location = New System.Drawing.Point(660, 513)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(120, 40)
        Me.CmdBack.TabIndex = 2
        Me.CmdBack.Text = "�� ��"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Location = New System.Drawing.Point(693, 29)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 19
        Me.lblDate.Text = "9999�N99��99��"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Location = New System.Drawing.Point(693, 9)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(47, 12)
        Me.lblUser.TabIndex = 18
        Me.lblUser.Text = "���[�U��"
        Me.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblTop
        '
        Me.LblTop.AutoSize = True
        Me.LblTop.Font = New System.Drawing.Font("�l�r �S�V�b�N", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LblTop.Location = New System.Drawing.Point(274, 9)
        Me.LblTop.Name = "LblTop"
        Me.LblTop.Size = New System.Drawing.Size(303, 19)
        Me.LblTop.TabIndex = 17
        Me.LblTop.Text = "�������i�f�B�X�N���b�l�s�j��"
        Me.LblTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblDateCaption
        '
        Me.LblDateCaption.AutoSize = True
        Me.LblDateCaption.Location = New System.Drawing.Point(615, 29)
        Me.LblDateCaption.Name = "LblDateCaption"
        Me.LblDateCaption.Size = New System.Drawing.Size(73, 12)
        Me.LblDateCaption.TabIndex = 16
        Me.LblDateCaption.Text = "�V�X�e�����t�F"
        Me.LblDateCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblUserIDCaption
        '
        Me.LblUserIDCaption.AutoSize = True
        Me.LblUserIDCaption.Location = New System.Drawing.Point(629, 9)
        Me.LblUserIDCaption.Name = "LblUserIDCaption"
        Me.LblUserIDCaption.Size = New System.Drawing.Size(59, 12)
        Me.LblUserIDCaption.TabIndex = 15
        Me.LblUserIDCaption.Text = "���O�C�����F"
        Me.LblUserIDCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CmdWrite
        '
        Me.CmdWrite.Font = New System.Drawing.Font("�l�r �S�V�b�N", 10.0!)
        Me.CmdWrite.Location = New System.Drawing.Point(534, 513)
        Me.CmdWrite.Name = "CmdWrite"
        Me.CmdWrite.Size = New System.Drawing.Size(120, 40)
        Me.CmdWrite.TabIndex = 1
        Me.CmdWrite.Text = "�b�l�s�����J�n"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 54)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 12)
        Me.Label1.TabIndex = 45
        Me.Label1.Text = "�t�H�[�}�b�g�I��"
        '
        'ListView1
        '
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ReceptionNo, Me.FileSEQ, Me.SyoriDate, Me.StackerNo, Me.WriteCounter, Me.ItakuCD, Me.FuriDate, Me.ItakuKana, Me.ItakuKanji, Me.SyoriKen, Me.SyoriKin, Me.TRTotalKen, Me.TRTotalKin, Me.ErrCD, Me.ErrorName, Me.BankCode, Me.BankName, Me.BranchCode, Me.BranchName, Me.JSFlg, Me.FuriKen, Me.FuriKin, Me.FunouKen, Me.FunouKin, Me.FSyoriKbn, Me.ToriSCode, Me.ToriFCode, Me.StationNo, Me.FileName, Me.OverrideFlg, Me.ComplockFlg, Me.CreateDate, Me.UpdateDate})
        Me.ListView1.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListView1.GridLines = True
        Me.ListView1.Location = New System.Drawing.Point(12, 84)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(768, 400)
        Me.ListView1.TabIndex = 46
        Me.ListView1.TabStop = False
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ReceptionNo
        '
        Me.ReceptionNo.Text = "��tNo"
        Me.ReceptionNo.Width = 54
        '
        'FileSEQ
        '
        Me.FileSEQ.Text = "SEQ"
        Me.FileSEQ.Width = 39
        '
        'SyoriDate
        '
        Me.SyoriDate.Text = "������"
        Me.SyoriDate.Width = 71
        '
        'StackerNo
        '
        Me.StackerNo.Text = "����"
        Me.StackerNo.Width = 0
        '
        'WriteCounter
        '
        Me.WriteCounter.Text = "������"
        Me.WriteCounter.Width = 65
        '
        'ItakuCD
        '
        Me.ItakuCD.Text = "�ϑ���CD"
        Me.ItakuCD.Width = 92
        '
        'FuriDate
        '
        Me.FuriDate.Text = "�U�֓�"
        Me.FuriDate.Width = 63
        '
        'ItakuKana
        '
        Me.ItakuKana.Text = "�ϑ��Җ��J�i"
        Me.ItakuKana.Width = 117
        '
        'ItakuKanji
        '
        Me.ItakuKanji.Text = "�ϑ��Ҋ�����"
        Me.ItakuKanji.Width = 0
        '
        'SyoriKen
        '
        Me.SyoriKen.Text = "����"
        Me.SyoriKen.Width = 0
        '
        'SyoriKin
        '
        Me.SyoriKin.Text = "���z"
        Me.SyoriKin.Width = 0
        '
        'TRTotalKen
        '
        Me.TRTotalKen.Text = "TR���v����"
        Me.TRTotalKen.Width = 0
        '
        'TRTotalKin
        '
        Me.TRTotalKin.Text = "TR���v���z"
        Me.TRTotalKin.Width = 0
        '
        'ErrCD
        '
        Me.ErrCD.Text = "�G���[CD"
        Me.ErrCD.Width = 61
        '
        'ErrorName
        '
        Me.ErrorName.Text = "�G���[��"
        Me.ErrorName.Width = 102
        '
        'BankCode
        '
        Me.BankCode.Text = "���Z�@�փR�[�h"
        Me.BankCode.Width = 0
        '
        'BankName
        '
        Me.BankName.Text = "���Z�@�֖�"
        Me.BankName.Width = 0
        '
        'BranchCode
        '
        Me.BranchCode.Text = "�x�X�ԍ�"
        Me.BranchCode.Width = 0
        '
        'BranchName
        '
        Me.BranchName.Text = "�x�X��"
        Me.BranchName.Width = 0
        '
        'JSFlg
        '
        Me.JSFlg.Text = "��/��"
        Me.JSFlg.Width = 0
        '
        'FuriKen
        '
        Me.FuriKen.Text = "�U�֍ό���"
        Me.FuriKen.Width = 0
        '
        'FuriKin
        '
        Me.FuriKin.Text = "�U�֍ϋ��z"
        Me.FuriKin.Width = 0
        '
        'FunouKen
        '
        Me.FunouKen.Text = "�s�\����"
        Me.FunouKen.Width = 0
        '
        'FunouKin
        '
        Me.FunouKin.Text = "�s�\���z"
        Me.FunouKin.Width = 0
        '
        'FSyoriKbn
        '
        Me.FSyoriKbn.Text = "F�����敪"
        Me.FSyoriKbn.Width = 0
        '
        'ToriSCode
        '
        Me.ToriSCode.Text = "������R�[�h"
        Me.ToriSCode.Width = 92
        '
        'ToriFCode
        '
        Me.ToriFCode.Text = "���R�[�h"
        '
        'StationNo
        '
        Me.StationNo.Text = "CMT�ǎ�@��"
        Me.StationNo.Width = 0
        '
        'FileName
        '
        Me.FileName.Text = "�t�@�C����"
        Me.FileName.Width = 0
        '
        'OverrideFlg
        '
        Me.OverrideFlg.Text = "���������t���O"
        Me.OverrideFlg.Width = 0
        '
        'ComplockFlg
        '
        Me.ComplockFlg.Text = "�Í���"
        Me.ComplockFlg.Width = 0
        '
        'CreateDate
        '
        Me.CreateDate.Text = "�쐬��"
        Me.CreateDate.Width = 97
        '
        'UpdateDate
        '
        Me.UpdateDate.Text = "�X�V��"
        Me.UpdateDate.Width = 128
        '
        'PrintButton
        '
        Me.PrintButton.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!)
        Me.PrintButton.Location = New System.Drawing.Point(14, 513)
        Me.PrintButton.Name = "PrintButton"
        Me.PrintButton.Size = New System.Drawing.Size(120, 40)
        Me.PrintButton.TabIndex = 74
        Me.PrintButton.Text = "���"
        Me.PrintButton.Visible = False
        '
        'ComboFormat
        '
        Me.ComboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboFormat.Items.AddRange(New Object() {"�S��/SSS", "�n����(350)", "�n����(300)", "����", "NHK"})
        Me.ComboFormat.Location = New System.Drawing.Point(96, 52)
        Me.ComboFormat.Name = "ComboFormat"
        Me.ComboFormat.Size = New System.Drawing.Size(132, 20)
        Me.ComboFormat.TabIndex = 75
        Me.ComboFormat.Tag = "0"
        '
        'KFCCMT021
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.CancelButton = Me.CmdBack
        Me.ClientSize = New System.Drawing.Size(794, 568)
        Me.Controls.Add(Me.ComboFormat)
        Me.Controls.Add(Me.PrintButton)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.LblTop)
        Me.Controls.Add(Me.LblDateCaption)
        Me.Controls.Add(Me.LblUserIDCaption)
        Me.Controls.Add(Me.CmdWrite)
        Me.Controls.Add(Me.CmdBack)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFCCMT021"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFCCMT021"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private MyOwnerForm As Form
    Private Const ThisModuleName As String = "KFCCMT021.vb"
    Protected Const strPrtTitle As String = "�b�l�s��������"


    '��ʋN��������
    Private Sub KFJCMT0021G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        With GCom.GLog
            .Job1 = "�Í����Ȃ���������"
            .Job2 = "��ʋN��������"
            Try
                MyOwnerForm = GOwnerForm
                GOwnerForm = Me
                Me.CmdBack.DialogResult = DialogResult.None
                Me.lblUser.Text = GCom.GetUserID
                Me.lblDate.Text = String.Format("{0:yyyy�NMM��dd��}", GCom.GetSysDate)

                Call GCom.SetMonitorTopArea(LblUserIDCaption, LblDateCaption, lblUser, lblDate)

                Me.ComboFormat.SelectedIndex = 0 ' �f�t�H���g�őS��t�H�[�}�b�g��ݒ�
                ListView1.View = View.Details

            Catch ex As Exception
                .Result = clsCommon.NG
                .Discription = ex.Message
                GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            End Try

        End With
    End Sub

    '��ʕ\���i�ĕ\���j������
    Private Sub KFJCMT0021G_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job1 = "�Í����Ȃ���������"
    End Sub

    '��ʏI��������
    Private Sub KFJCMT0021G_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        GOwnerForm = MyOwnerForm
        GOwnerForm.Visible = True
        GCom.SetFormEnabled(GOwnerForm)
    End Sub

    '�ꗗ�\���̈�_�u���N���b�N������
    Private Sub ListView1_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(ListView1)
    End Sub

    '�����{�^������
    Private Sub CmdWrite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdWrite.Click
        Me.CmdWrite.Enabled = False
        Me.CmdBack.Enabled = False
        Me.PrintButton.Enabled = False
        Dim Index As Integer
        Select Case Me.ComboFormat.SelectedIndex
            Case 4
                Index = 6
            Case Else
                Index = Me.ComboFormat.SelectedIndex
        End Select
        Call CmtCom.CmtWriter(Index, False, ListView1, MAXSTACKER)
        Me.CmdWrite.Enabled = True
        Me.CmdBack.Enabled = True
        Me.PrintButton.Enabled = True
    End Sub

    '��ʏI���{�^������
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
        Me.Close()
        Me.Dispose()
    End Sub


    '�ꗗ�\���̈�̃\�[�g
    Private Sub LetSort_ListView(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick

        With CType(sender, ListView)

            If ClickedColumn = e.Column Then
                ' ��������N���b�N�����ꍇ�́C�t���ɂ��� 
                SortOrderFlag = Not SortOrderFlag
            End If

            ' ��ԍ��ݒ�
            ClickedColumn = e.Column

            ' �񐅕������z�u
            Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

            ' �\�[�g
            .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

            ' �\�[�g���s
            .Sort()

        End With
    End Sub

    ' ����{�^��
    Private Sub PrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintButton.Click
        Me.CmdWrite.Enabled = False
        Me.CmdBack.Enabled = False
        Me.PrintButton.Enabled = False
        Call CmtCom.PrintButton(ListView1, KFCCMT021.strPrtTitle) ' ����{�^������
        Me.CmdWrite.Enabled = True
        Me.CmdBack.Enabled = True
        Me.PrintButton.Enabled = True
    End Sub
End Class
