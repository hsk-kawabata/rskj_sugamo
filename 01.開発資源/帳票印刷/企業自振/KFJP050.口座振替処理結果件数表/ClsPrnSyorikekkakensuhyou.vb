Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �����U�֏������ʌ����\
Class ClsPrnSyorikekkakensuhyou
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP050"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP050_�����U�֏������ʌ����\.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("������")
        CSVObject.Output("����於")
        CSVObject.Output("�����S������")
        CSVObject.Output("�����ɖ�")
        CSVObject.Output("�N�x")
        CSVObject.Output("��")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("�Ŗ�����")
        CSVObject.Output("�Ǐ��ԍ���")
        CSVObject.Output("���t������")
        CSVObject.Output("���t�����v���z")
        CSVObject.Output("�U�֔[�t�s�\����")
        CSVObject.Output("�U�֔[�t�s�\���v���z")
        CSVObject.Output("�U�֔[�t����")
        CSVObject.Output("�U�֔[�t���v���z", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F �����U�֏������ʌ����\���f�[�^�ɏ�������
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
