Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �����U�֖��ו\
Class ClsPrnKouzafurimei
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal SortKey As String)
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP010"

        ' ��`�̖��Z�b�g
        Select Case SortKey
            '2018/01/10 �^�X�N�j���� CHG (�W���ŏC��(��181)�@���[��`�̃t�@�C�����̕ύX) -------------------- START
            Case "1"
                ReportBaseName = "KFJP010_�����U�֖��ו\(������).rpd"
            Case "2"
                ReportBaseName = "KFJP010_�����U�֖��ו\(�x�X��).rpd"
            Case Else
                ReportBaseName = "KFJP010_�����U�֖��ו\(������).rpd"
                'Case "1"
                '    ReportBaseName = "KFJP010.�����U�֖��ו\(������).rpd"
                'Case "2"
                '    ReportBaseName = "KFJP010.�����U�֖��ו\(�x�X��).rpd"
                'Case Else
                '    ReportBaseName = "KFJP010.�����U�֖��ו\(������).rpd"
                '2018/01/10 �^�X�N�j���� CHG (�W���ŏC��(��181)�@���[��`�̃t�@�C�����̕ύX) -------------------- END
        End Select

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' �^�C�g���s
        CSVObject.Output("�U�֓�")
        CSVObject.Output("������")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("�U�֊�Ɩ�")
        CSVObject.Output("���Z�@�փR�[�h")
        CSVObject.Output("���Z�@�֖�")
        CSVObject.Output("�x�X�R�[�h")
        CSVObject.Output("�x�X��")
        CSVObject.Output("�Ȗ�")
        CSVObject.Output("�����ԍ�")
        CSVObject.Output("�_��Ҏ���")
        CSVObject.Output("���z")
        CSVObject.Output("���o���敪")
        CSVObject.Output("�U�փR�[�h")
        CSVObject.Output("��ƃR�[�h")
        CSVObject.Output("��ƃV�[�P���X")
        CSVObject.Output("�E�v")
        CSVObject.Output("���v�Ɣԍ�", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F �����U�֖��ו\���f�[�^�ɏ�������
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
