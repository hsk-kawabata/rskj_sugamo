Imports System
Imports System.Text
Imports System.Windows.Forms
Imports clsFUSION.clsMain
Imports CASTCommon
Imports MenteCommon

Module M_FUSION

    Public lngEXITCODE As Long
    Public clsFUSION As New clsFUSION.clsMain

    '���O�C�����[�U�p
    Public gstrUSER_ID As String
    Public gstrPASSWORD_ID As String

    'KFJMAIN0701G.lstSCHLIST�̃C���f�b�N�X�l
    Public gintMAIN0701G_LIST_INDEX As Integer
    'KFJMAIN0101G.lstSCHLIST�̃C���f�b�N�X�l
    Public gintMAIN0101G_LIST_INDEX As Integer
    'KFJMAIN1201G.lstSCHLIST�̃C���f�b�N�X�l
    Public gintMAIN1201G_LIST_INDEX As Integer
    'KFJENTR0101G.lstKOKYAKULIST�̃C���f�b�N�X�l
    Public gintENTR0101G_LIST_INDEX As Integer

    Public gstrDB_CONNECT As String = CASTCommon.DB.CONNECT

    Public gstrFURI_DATE As String

    '--------------------------------------------------------------------------
    ' �ȉ��AAstar �ǉ��� (2007�N�x����M���d�l)
    '--------------------------------------------------------------------------

    Public GCom As MenteCommon.clsCommon

    Public GOwnerForm As Form

    Public MainForm As KFJMENU010

    ' ���C��
    Sub Main()
        MainForm = New KFJMENU010

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
