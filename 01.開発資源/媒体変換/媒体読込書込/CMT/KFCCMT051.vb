Public Class KFCCMT051
    Inherits System.Windows.Forms.Form

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
        Me.LblOtherCMT.Font = New System.Drawing.Font("�l�r �S�V�b�N", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LblOtherCMT.Location = New System.Drawing.Point(228, 9)
        Me.LblOtherCMT.Name = "LblOtherCMT"
        Me.LblOtherCMT.Size = New System.Drawing.Size(336, 24)
        Me.LblOtherCMT.TabIndex = 0
        Me.LblOtherCMT.Text = "�����s��CMT������"
        Me.LblOtherCMT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Location = New System.Drawing.Point(680, 40)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 23
        Me.lblDate.Text = "9999�N99��99��"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Location = New System.Drawing.Point(680, 16)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(47, 12)
        Me.lblUser.TabIndex = 22
        Me.lblUser.Text = "���[�U��"
        Me.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LblDateCaption
        '
        Me.LblDateCaption.AutoSize = True
        Me.LblDateCaption.Location = New System.Drawing.Point(600, 40)
        Me.LblDateCaption.Name = "LblDateCaption"
        Me.LblDateCaption.Size = New System.Drawing.Size(73, 12)
        Me.LblDateCaption.TabIndex = 21
        Me.LblDateCaption.Text = "�V�X�e�����t�F"
        Me.LblDateCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'LblUserIDCaption
        '
        Me.LblUserIDCaption.AutoSize = True
        Me.LblUserIDCaption.Location = New System.Drawing.Point(616, 16)
        Me.LblUserIDCaption.Name = "LblUserIDCaption"
        Me.LblUserIDCaption.Size = New System.Drawing.Size(59, 12)
        Me.LblUserIDCaption.TabIndex = 20
        Me.LblUserIDCaption.Text = "���O�C�����F"
        Me.LblUserIDCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ExeBtn
        '
        Me.ExeBtn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!)
        Me.ExeBtn.Location = New System.Drawing.Point(232, 523)
        Me.ExeBtn.Name = "ExeBtn"
        Me.ExeBtn.Size = New System.Drawing.Size(120, 40)
        Me.ExeBtn.TabIndex = 25
        Me.ExeBtn.Text = "���s"
        '
        'RetBtn
        '
        Me.RetBtn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!)
        Me.RetBtn.Location = New System.Drawing.Point(649, 523)
        Me.RetBtn.Name = "RetBtn"
        Me.RetBtn.Size = New System.Drawing.Size(120, 40)
        Me.RetBtn.TabIndex = 26
        Me.RetBtn.Text = "�߂�"
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
    Private bSendFlag As Boolean ' ������ true / ���ʎ󂯎�莞 false

    '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
    Private Const strSendOther As String = "�����s�������f�[�^������"
    Private Const strReceiveOther As String = "�����s�����ʃf�[�^�捞��"
    'Private Const strSendOther = "�����s�������f�[�^������"
    'Private Const strReceiveOther = "�����s�����ʃf�[�^�捞��"
    '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

    '��ʋN��������
    Private Sub KFJCMT0051G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GCom.GLog.Job1 = "���s��CMT�������"
        GCom.GLog.Job2 = "��ʋN��������"
        Try
            ' ���[�UID�E�V�X�e�����t�̎擾�Ɛݒ�
            MyOwnerForm = GOwnerForm
            GOwnerForm = Me
            Me.lblUser.Text = GCom.GetUserID
            Me.lblDate.Text = String.Format("{0:yyyy�NMM��dd��}", GCom.GetSysDate)

            '*** �C�� mitsu 2008/09/02 �󎚈ʒu���� ***
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

    ' ��ʕ\���i�ĕ\���j������
    Private Sub KFJCMT0051G_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job1 = "���s��CMT�������"
    End Sub

    ' ��ʏI��������
    Private Sub KFJCMT0051G_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        GOwnerForm = MyOwnerForm
        GOwnerForm.Visible = True
        GCom.SetFormEnabled(GOwnerForm)
    End Sub

    ' ���s�{�^������
    Private Sub ExeBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExeBtn.Click
        If Me.bSendFlag Then
            ' �����f�[�^�쐬
            CmtCom.WriteOtherCMT()
        Else
            ' ���ʃf�[�^�ǎ�
            CmtCom.ReadOtherCMT()
        End If
    End Sub

    ' �߂�{�^������
    Private Sub RetBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RetBtn.Click
        Me.Close()
        Me.Dispose()
    End Sub

    ' ����/���ʓǎ�̐ؑ�
    Property SendFlag() As Boolean
        Get
            Return bSendFlag
        End Get
        Set(ByVal Value As Boolean)
            bSendFlag = Value
        End Set
    End Property

End Class
