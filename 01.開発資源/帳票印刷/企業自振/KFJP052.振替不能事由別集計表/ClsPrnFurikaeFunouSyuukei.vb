Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �U�֕s�\���R�ʏW�v�\
Class ClsPrnFurikaeFunouSyuukei
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP052"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP052_�U�֕s�\���R�ʏW�v�\.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        ' �^�C�g���s
        CSVObject.Output("�U�֓�")
        CSVObject.Output("�ϑ��Җ�����")
        CSVObject.Output("�ϑ��҃R�[�h")
        CSVObject.Output("�����ɖ�")
        CSVObject.Output("�Ƃ�܂ƂߓX�R�[�h")
        CSVObject.Output("�Ƃ�܂ƂߓX������")
        CSVObject.Output("�U�֌��ʃR�[�h")
        CSVObject.Output("�U�֋��z")
        CSVObject.Output("������", False, True)

        Return file
    End Function

    '
    ' �@�\�@ �F �U�֕s�\���R�ʏW�v�\���f�[�^�ɏ�������
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
