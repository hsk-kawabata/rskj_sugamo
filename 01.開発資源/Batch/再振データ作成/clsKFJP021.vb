Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Collections.Generic
' �������ʊm�F�\(�ĐU�f�[�^�쐬)����N���X
Class clsKFJP021
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP021"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP021_�������ʊm�F�\(�ĐU�f�[�^�쐬).rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F �������ʊm�F�\(�ĐU�f�[�^�쐬)���f�[�^�ɏ�������
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
