Module M_FUSION
    Public GCom As MenteCommon.clsCommon
    '���O�C�����[�U�p
    Public gstrUSER_ID As String

    '�f�B���N�g���ϐ�
    Public gstrEXE_OPENDIR As String
    Public gstrGAKKOU_EXE_OPENDIR As String '�w�Z���UEXE�w��ꏊ

    Public GOwnerForm As Form

    '
    ' �@�\�@ �F ��ʉE�ド�x���ʒu�ݒ�
    '
    ' �����@ �F ARG1 - ���O�C���� �F    (Label)
    ' �@�@�@ �@ ARG2 - �V�X�e�����t �F  (Label)
    ' �@�@�@ �@ ARG3 - ���[�U�h�c       (Label)
    ' �@�@�@ �@ ARG4 - �V�X�e�����t     (Label)
    '
    ' �߂�l �F �Ȃ�
    '
    ' ���l�@ �F �Ȃ�
    '
    Public Sub SetMonitorTopArea(ByVal Label2 As Label, ByVal Label3 As Label, _
                        ByVal lblUser As Label, ByVal lblDate As Label)
        Try
            Label2.Location = New Point(580, 8)
            Label3.Location = New Point(580, 28)
            lblUser.Location = New Point(665, 8)
            lblDate.Location = New Point(665, 28)
            lblUser.Text = gstrUSER_ID
            lblDate.Text = String.Format("{0:yyyy�NMM��dd��}", Date.Now)
        Catch ex As Exception
            lblUser.Text = "SYSTEM ERROR"
            lblDate.Text = "SYSTEM ERROR"
        End Try
    End Sub

End Module
