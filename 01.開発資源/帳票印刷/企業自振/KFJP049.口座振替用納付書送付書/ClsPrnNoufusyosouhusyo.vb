Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �����U�֗p�[�t�����t��
Class ClsPrnNoufusyosouhusyo
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP049"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP049_�����U�֗p�[�t�����t��.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        CSVObject.Output("�Ǐ��ԍ�")
        CSVObject.Output("���Z�@�֗X�֔ԍ�")
        CSVObject.Output("�ŖڃR�[�h")
        CSVObject.Output("�[���敪")
        CSVObject.Output("�\���[���敪")
        CSVObject.Output("�s�s�於")
        CSVObject.Output("�N�x")
        CSVObject.Output("���ݒn�P")
        CSVObject.Output("���ݒn�Q")
        CSVObject.Output("����")
        CSVObject.Output("���Z�@�֖��̂P")
        CSVObject.Output("���Z�@�֖��̂Q")
        CSVObject.Output("�X�ܖ���")
        CSVObject.Output("���t������")
        CSVObject.Output("���t�����v���z")
        CSVObject.Output("�U�֔[�t�s�\����")
        CSVObject.Output("�U�֔[�t�s�\���v���z")
        CSVObject.Output("�U�֔[�t����")
        CSVObject.Output("�U�֔[�t���v���z")
        CSVObject.Output("�����N����(�N)")
        CSVObject.Output("�����N����(��)")
        CSVObject.Output("�����N����(��)")
        CSVObject.Output("�Ŗ�����")
        CSVObject.Output("����R�[�h", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F �����U�֗p�[�t�����t�����f�[�^�ɏ�������
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
