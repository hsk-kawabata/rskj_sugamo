Imports System
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports CASTCommon
Imports MenteCommon

Module M_FUSION

    '���O�C�����[�U�p
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String

    '--------------------------------------------------------------------------
    ' �ȉ��AAstar �ǉ��� (2007�N�x����M���d�l)
    '--------------------------------------------------------------------------

    Public GCom As MenteCommon.clsCommon

    Public GOwnerForm As Form

    Public MainForm As KFKMENU010

    ' ���C��
    Sub Main()
        MainForm = New KFKMENU010

        MainForm.Show()

        Application.Run(MainForm)
    End Sub

    '
    ' �@�\�@ �F ���C�����j���[��\������
    '
    ' ���l�@ �F
    '
    Public Sub ShowMain()
        MainForm.Visible = True
        MainForm.Activate()
    End Sub

    '
    ' �@�\�@ �F ���C�����j���[���B��
    '
    ' ���l�@ �F
    '
    Public Sub HideMain()
        MainForm.Visible = False
    End Sub

End Module
