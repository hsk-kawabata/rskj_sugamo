Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class ClsPrnKouzafurikaeiraisyo
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "Kouzafurikaeiraisyo"

        ' ��`�̖��Z�b�g
        reportBaseName = "�����U�ֈ˗���.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' �^�C�g���s
        CSVObject.Output("���܂Ƃߋ��Z�@�֖�")
        CSVObject.Output("���܂Ƃߎx�X��")
        CSVObject.Output("�戵���Z�@�փR�[�h")
        CSVObject.Output("�戵�x�X�R�[�h")
        CSVObject.Output("�戵���Z�@�֖�")
        CSVObject.Output("�戵�x�X��")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("�ϑ��Җ�")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("���o�敪")
        CSVObject.Output("���o������R�[�h")
        CSVObject.Output("���o����敛�R�[�h")
        CSVObject.Output("�������Z�@�փR�[�h")
        CSVObject.Output("�������Z�@�֖�")
        CSVObject.Output("�����x�X�R�[�h")
        CSVObject.Output("�����x�X��")
        CSVObject.Output("�_��Җ��J�i")
        CSVObject.Output("�_��Ҕԍ�")
        CSVObject.Output("�Ȗ�")
        CSVObject.Output("�����ԍ�")
        CSVObject.Output("�������z", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F �����U�ֈ˗������f�[�^�ɏ�������
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
