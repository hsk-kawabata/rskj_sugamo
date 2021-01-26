Imports System

'�������ʊm�F�\
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase
    Public CsvData(10) As String

    Sub New()
        'CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP002"

        '��`�̖��Z�b�g
        ReportBaseName = "KFJP002_�������ʊm�F�\�i�Z���^�[���ڎ����j.rpd"


        CsvData(0) = CASTCommon.Calendar.Now.ToString("yyyyMMdd")   '������
        CsvData(1) = CASTCommon.Calendar.Now.ToString("HHmmss")     '�^�C���X�^���v
        CsvData(2) = "0"                                            '������R�[�h
        CsvData(3) = "0"                                            '����敛�R�[�h
        CsvData(4) = ""                                             '����於
        CsvData(5) = "00010101"                                     '�U�֓�
        CsvData(6) = "0"                                            '�˗�����
        CsvData(7) = "0"                                            '�˗����z
        CsvData(8) = "0"                                           '��������
        CsvData(9) = "0"                                           '�������z
        CsvData(10) = ""                                            '���l
        
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        Return file
    End Function

    Public Function OutputCSVKekkaSysError(ByVal fsyoriKbn As String, _
                        ByVal aToriS As String, ByVal aToriF As String, _
                        ByVal aTuuban As Integer, ByVal aInfile As String, _
                        ByVal aMSG As String, ByVal oraDB As CASTCommon.MyOracle) As Boolean
        '------------------------------------------------------
        ' �b�r�u�t�@�C���쐬(�G���[��)
        '------------------------------------------------------
        Dim Comm As New CAstBatch.CommData(oraDB)

        Try
            'Call Comm.GetTORIMAST(aToriS, aToriF)

            'Itakusyamei = "ERROR"
            CreateCsvFile()

            CsvData(2) = ""
            CsvData(3) = ""
            
            '�O�s��
            If Not Comm.INFOToriMast.ITAKU_NNAME_T Is Nothing Then
                CsvData(4) = Comm.INFOToriMast.ITAKU_NNAME_T.Trim   '�ϑ��Җ�����
            Else
                CsvData(4) = ""                                     '�����}�X�^�ɑ��݂��Ȃ��ꍇ
            End If

            CsvData(5) = ""                                         '�U�֓�
            CsvData(6) = ""
            CsvData(7) = ""
            CsvData(8) = ""
            CsvData(9) = ""
            CsvData(10) = ""

            If CASTCommon.CAInt32(aToriS) <> 0 Then
                CSVObject.Output(CsvData)
            End If

            '�P�s��
            CsvData(2) = ""
            CsvData(3) = ""
            CsvData(4) = "�����ł��܂���ł��� "
            CsvData(5) = ""
            CsvData(6) = ""

            CSVObject.Output(CsvData)

            '�Q�s��
            CsvData(4) = "�W���u�ʔԁF" & aTuuban.ToString
            CSVObject.Output(CsvData)

            '�R�s��
            CsvData(4) = "�t�@�C�����F" & aInfile
            CSVObject.Output(CsvData)

            '�S�s��
            CsvData(4) = """���e�F" & aMSG & """"

            CSVObject.Output(CsvData)

            If ReportExecute() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write("(�������ʊm�F�\(�Z���^�[���ڎ���))CSV�f�[�^�o��", "���s", ex.Message)
        End Try
    End Function

    '
    ' �@�\�@ �F �������ʊm�F�\�b�r�u���o�͂���
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCSVKekka(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter    '�p�����[�^���
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast      '�������
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast          '�˗����׏��

        Try
            '------------------------------------------------------
            ' �b�r�u�t�@�C���쐬(����I����)
            '------------------------------------------------------
            CsvData(0) = InfoPara.FURI_DATE                                     '
            CsvData(2) = InfoTori.TORIS_CODE_T                                  '������R�[�h
            CsvData(3) = InfoTori.TORIF_CODE_T                                  '����敛�R�[�h
            CsvData(4) = InfoTori.ITAKU_NNAME_T                                 '����於
            CsvData(5) = InfoMei.FURIKAE_DATE                                   '�U�֓�
            CsvData(6) = InfoMei.TOTAL_IRAI_KEN.ToString                        '�˗�����
            CsvData(7) = InfoMei.TOTAL_IRAI_KIN.ToString                        '�˗����z
            CsvData(8) = (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN).ToString  '��������
            CsvData(9) = (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN).ToString  '�������z
            CsvData(10) = ""                                                    '���l

            '�G���[���b�Z�[�W�͔��l���ɏo�͂���
            If aReadFmt.InfoMeisaiMast.DUPLICATE_KBN = "" Then
                If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN) Then
                    CsvData(10) = "�����ُ�"
                ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                    CsvData(10) = "���z�ُ�"
                End If
            End If

            CSVObject.Output(CsvData)

        Catch ex As Exception
            BatchLog.Write("(�������ʊm�F�\(�Z���^�[���ڎ���))CSV�f�[�^�o��", "���s", ex.Message)
            End Try
    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean
        Try
            '2010/02/03 �ʏ�g�p����v�����^�ɂ��� ===
            'Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
            Return MyBase.ReportExecute()
            '=======================================================
        Catch ex As Exception
            BatchLog.Write("(�������ʊm�F�\(�Z���^�[���ڎ���))������s", "���s", ex.Message)
            
            Return False
        End Try
    End Function
End Class
