Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic

' �ϑ��ҕʐU�֌��ʈꗗ�\
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    Public CsvData(13) As String
    ' 2016/06/17 ���� ADD (RSV2�Ή�<���l�M��>) ------------------------------------ START
    Public strATENA_UMU As String
    ' 2016/06/17 ���� ADD (RSV2�Ή�<���l�M��>) -------------------------------------- END

    Sub New()
        '   ' CSV�t�@�C���Z�b�g
        '   InfoReport.ReportName = "�ϑ��ҕʐU�֌��ʈꗗ�\"

        '   ' ��`�̖��Z�b�g
        '   ReportBaseName = "�ϑ��ҕʐU�֌��ʈꗗ�\.rpd"

        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP020"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP020_�������ʊm�F�\(�Ԋ҃f�[�^�쐬).rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile()

        ' �^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("�U�֓�")
        ' 2016/06/17 ���� ADD (RSV2�Ή�<���l�M��>) -------------------------------- START
        If strATENA_UMU <> "err" Then         '���l�M���ȍ~��Ver�̂ݍ��ڒǉ�
            CSVObject.Output("�X���於")      '�X����i�Ȃ���Έϑ��Җ�)�ݒ�
            CSVObject.Output("���ɖ�")        '���ɖ��iini�t�@�C��)�ݒ�
        End If
        ' 2016/06/17 ���� ADD (RSV2�Ή�<���l�M��>) ---------------------------------- END
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("����於")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("��������")
        CSVObject.Output("�������z")
        CSVObject.Output("�U�֌���")
        CSVObject.Output("�U�֋��z")
        CSVObject.Output("�s�\����")
        CSVObject.Output("�s�\���z")
        CSVObject.Output("���l")
        CSVObject.Output("�敪", False, True)

        Return file
    End Function

    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
