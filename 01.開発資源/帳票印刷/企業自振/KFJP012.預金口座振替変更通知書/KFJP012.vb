Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �����U�֎������ψꗗ�\
Class KFJP012
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP012"

        '�Z���^�[�R�[�h
        Dim CENTER As String = ""
        CENTER = CASTCommon.GetFSKJIni("COMMON", "CENTER")
        If CENTER.ToUpper = "ERR" Or CENTER = "" Then
            MainLOG.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�Z���^�[�R�[�h ����:COMMON ����:CENTER")
        End If
        ' ��`�̖��Z�b�g
        '2011/07/05 �W���ŏC�� ���C�Ƃ��̑��̒n��Ń��C�A�E�g��ύX���� ------------------START
        'ReportBaseName = "KFJP012_�a�������U�֕ύX�ʒm��.rpd"
        If CENTER = "4" Then
            ReportBaseName = "KFJP012_2_�a�������U�֕ύX�ʒm��_���C.rpd"
        Else
            ReportBaseName = "KFJP012_�a�������U�֕ύX�ʒm��.rpd"
        End If
        '2011/07/05 �W���ŏC�� ���C�Ƃ��̑��̒n��Ń��C�A�E�g��ύX���� ------------------END
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F CSV�t�@�C���ɏ�������
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