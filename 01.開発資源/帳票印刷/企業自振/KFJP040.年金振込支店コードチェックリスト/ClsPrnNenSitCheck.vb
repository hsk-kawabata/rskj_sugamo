Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

'�N���U���x�X�R�[�h�`�F�b�N���X�g
Class ClsPrnNenSitCheck
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP040"
        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP040_�N���U���x�X�R�[�h�`�F�b�N���X�g.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile
        Return file
    End Function

    '=======================================================================
    ' �@�\�@ �F �N���U���x�X�R�[�h�`�F�b�N���X�g���f�[�^�ɏ�������
    ' ���l�@ �F 
    ' �U�֓� �F 2009/09/29
    ' �X�V�� �F
    '=======================================================================
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
