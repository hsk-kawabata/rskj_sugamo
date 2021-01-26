Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �������ρ^�萔�������s�\�ꗗ�\
Class ClsPrnFunoumeisai
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "�������ϕs�\�ꗗ"

        ' ��`�̖��Z�b�g
        reportBaseName = "�������ώ萔�������s�\�ꗗ�\.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' �^�C�g���s
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("��")
        CSVObject.Output("�ʔ�")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("�ϑ��Җ��i�����j")
        CSVObject.Output("�I�y�R�[�h")
        CSVObject.Output("���o���敪")
        CSVObject.Output("�f�[�^�敪")
        CSVObject.Output("���z")
        CSVObject.Output("����")
        CSVObject.Output("�G���[���e", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F �������ρ^�萔�������s�\�ꗗ�\���f�[�^�ɏ�������
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
