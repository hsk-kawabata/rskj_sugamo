Imports System

''' <summary>
''' �������ʊm�F�\(�������σ��G���^���ʍX�V)����N���X
''' </summary>
''' <remarks>2013/09/17 saitou ��_�M�� ���o�b�`�Ή�</remarks>
Class KFKP003
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFKP003"
        ' ��`�̖��Z�b�g
        ReportBaseName = "KFKP003_�������ʊm�F�\(�������ό���_���o�b�`).rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()
        Return file
    End Function

End Class
