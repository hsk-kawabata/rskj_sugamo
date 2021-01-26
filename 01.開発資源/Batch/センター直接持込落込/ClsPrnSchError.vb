Imports System
Imports System.IO
Imports Microsoft.VisualBasic.Strings

' �X�P�W���[���G���[���X�g
Class ClsPrnSchError
    Inherits CAstReports.ClsReportBase

    Sub New()

        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP014"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP014_�X�P�W���[���G���[���X�g.rpd"

    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile()

        Return file
    End Function

    Public Sub OutputData(ByVal aToriComm As CAstBatch.CommData, ByVal fmt As CAstFormat.CFormat, ByVal errPtn As Integer)

        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '�������
        Dim mei As CAstFormat.CFormat.MEISAI = fmt.InfoMeisaiMast

        Try

            CSVObject.Output("�Z���^�[���ڎ�������")  '
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))  '������
            CSVObject.Output(CASTCommon.Calendar.Now.ToString("HHmmss"))    '�^�C���X�^���v
            CSVObject.Output(InfoTori.TORIS_CODE_T)                         '������R�[�h 
            CSVObject.Output(InfoTori.TORIF_CODE_T)                         '����敛�R�[�h
            CSVObject.Output(InfoTori.ITAKU_NNAME_T, True)                  '����於
            CSVObject.Output(InfoTori.FURI_CODE_T, True)                          '�U�փR�[�h
            CSVObject.Output(InfoTori.KIGYO_CODE_T, True)                         '��ƃR�[�h
            CSVObject.Output(mei.FURIKAE_DATE, True)                            '�U�֓�

            Select Case errPtn
                Case 1
                    CSVObject.Output("�X�P�W���[���Ȃ�", True, True)                '�G���[���e
                Case 2
                    CSVObject.Output("���f��", True, True)
                Case 3
                    CSVObject.Output("������", True, True)
            End Select


        Catch ex As Exception
            BatchLog.Write("(�X�P�W���[���G���[���X�g���)CSV�f�[�^�o��", "���s", ex.Message)
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
