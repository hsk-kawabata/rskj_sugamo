Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �U�֌��ʖ��ו\
Class ClsPrnFrikaeKekkameisai
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        'InfoReport.ReportName = "Frikaekekkameisai"

        InfoReport.ReportName = "KFJP018"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP018_�U�֌��ʖ��ו\.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' �^�C�g���s
        '*** �C�� mitsu 2008/09/18 ���ڕs���C�� ***
        '    CSVObject.Output ("�����R�[�h")
        '    CSVObject.Output ("�ϑ��Җ��J�i")
        '    CSVObject.Output ("�U�֓�")
        '    CSVObject.Output ("�^�C�g���敪")
        '    CSVObject.Output ("�戵���Z�@�փR�[�h")
        '    CSVObject.Output ("���Z�@�֖��J�i")
        '    CSVObject.Output ("�戵�x�X�R�[�h")
        '    CSVObject.Output ("�x�X���J�i")
        '    CSVObject.Output ("�戵�X��")
        '    CSVObject.Output ("�a�����")
        '    CSVObject.Output ("�����ԍ�")
        '    CSVObject.Output ("�a���Җ�")
        '    CSVObject.Output ("�U�ֈ˗����z")
        '    CSVObject.Output ("�U�֕s�\���R")
        '    CSVObject.Output ("���l")
        '    CSVObject.Output ("�U�ֈ˗�����")
        '    CSVObject.Output ("�U�ֈ˗����zS")
        '    CSVObject.Output ("�U�֕s�\����")
        '    CSVObject.Output ("�U�֕s�\���z")
        '    CSVObject.Output ("�U�֍ό���")
        '    CSVObject.Output ("�U�֍ϋ��z")
        '    CSVObject.Output ("�萔��(����ō�)")
        '    CSVObject.Output ("�s�\�敪")
        '    CSVObject.Output ("�ϑ��҃R�[�h")
        '    CSVObject.Output ("������R�[�h")
        '    CSVObject.Output("����敛�R�[�h", False, True)
        '******************************************

        CSVObject.Output("�����ɖ�")
        CSVObject.Output("�����R�[�h")
        CSVObject.Output("�ϑ��Җ�����")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("�Ƃ�܂ƂߓX�R�[�h")
        CSVObject.Output("�Ƃ�܂ƂߓX������")
        CSVObject.Output("���Z�@�փR�[�h")
        CSVObject.Output("�x�X")
        CSVObject.Output("�������`")
        CSVObject.Output("�Ȗ�")
        CSVObject.Output("�����ԍ�")
        CSVObject.Output("�U�֋��z")
        CSVObject.Output("�U�֕s�\���R")
        CSVObject.Output("���v�Ɣԍ�")
        CSVObject.Output("�U�֍ό���")
        CSVObject.Output("�U�֍ϋ��z")
        CSVObject.Output("�U�֕s�\����")
        CSVObject.Output("�U�֕s�\���z")
        CSVObject.Output("�U�ֈ˗�����")
        CSVObject.Output("�U�ֈ˗����z�r")
        CSVObject.Output("�萔��(����ō�)")
        CSVObject.Output("������")
        CSVObject.Output("���Z�@�֖�")
        'CSVObject.Output("�x�X��", False, True)
        CSVObject.Output("�x�X��")
        CSVObject.Output("�t�H�[�}�b�g�敪", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F �U�֌��ʖ��ו\���f�[�^�ɏ�������
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
