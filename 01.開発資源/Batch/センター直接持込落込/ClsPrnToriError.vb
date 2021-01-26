Imports System
Imports System.IO
Imports Microsoft.VisualBasic.Strings

' �X�P�W���[���G���[���X�g
Class ClsPrnToriError
    Inherits CAstReports.ClsReportBase

    Sub New()

        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP015"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP015_�����}�X�^�G���[���X�g.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        Return file
    End Function

    Public Sub OutputData(ByVal fmt As CAstFormat.CFormat)

        Dim mei As CAstFormat.CFormat.MEISAI = fmt.InfoMeisaiMast

        Try

            CSVObject.Output("�Z���^�[���ڎ�������")                        '�T�u�^�C�g��
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '������
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    '�^�C���X�^���v
            CSVObject.Output(mei.FURI_CODE, True)                           '�U�փR�[�h
            CSVObject.Output(mei.KIGYO_CODE, True)                          '��ƃR�[�h
            CSVObject.Output(mei.FURIKAE_DATE, True, True)                  '�U�֓�

        Catch ex As Exception
            BatchLog.Write("(�����G���[���X�g���)CSV�f�[�^�o��", "���s", ex.Message)
        End Try

    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean

        '���[�o�͏���
        '2010/02/03 �ʏ�g�p����v�����^�ɂ��� ===
        'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
        Return MyBase.ReportExecute()
        '=======================================================
    End Function

    Protected Overrides Sub Finalize()

        MyBase.Finalize()

    End Sub

End Class
