Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

Public Class KFJP013
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP013"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP013_�������ʊm�F�\(�s�\���ʍX�V).rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F �����}�X�^�����e(�o�^)���f�[�^�ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    '�������ʊm�F�\(�s�\���ʍX�V)
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
