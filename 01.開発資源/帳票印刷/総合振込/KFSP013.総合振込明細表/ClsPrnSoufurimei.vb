Option Strict On
Option Explicit On

Imports System

' �����U�����ו\
Class ClsPrnSoufurimei
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFSP013"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFSP013_�����U�����ו\.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F �����U�����ו\���f�[�^�ɏ�������
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
