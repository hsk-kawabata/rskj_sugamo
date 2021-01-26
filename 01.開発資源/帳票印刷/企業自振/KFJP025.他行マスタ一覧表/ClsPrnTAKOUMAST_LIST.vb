Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' ���s�}�X�^�ꗗ�\�\
Class ClsPrnTAKOUMAST_LIST
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP025"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP025_���s�}�X�^�ꗗ�\.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' �^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("����於")
        CSVObject.Output("���Z�@�փR�[�h")
        CSVObject.Output("���Z�@�֖�")
        CSVObject.Output("�x�X�R�[�h")
        CSVObject.Output("�x�X��")
        CSVObject.Output("�Ȗ�")
        CSVObject.Output("�����ԍ�")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("�}�̃R�[�h")
        CSVObject.Output("�R�[�h�敪")
        CSVObject.Output("���M�t�@�C����")
        CSVObject.Output("��M�t�@�C����", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F ���s�}�X�^�ꗗ�\���f�[�^�ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
