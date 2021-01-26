' �������ʊm�F�\
Class ClsPrnSyorikekkaKakuninhyo
    Inherits CAstReports.ClsReportBase

    Public CsvData(12) As String

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "���V�X�������ʊm�F�\"

        ' ��`�̖��Z�b�g
        ReportBaseName = "���V�X�������ʊm�F�\.rpd"

        ' ������
        CsvData(0) = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        CsvData(1) = "00010101"                         ' �U�֓�
        CsvData(2) = "0"                                ' ������R�[�h
        CsvData(3) = "0"                                ' ����敛�R�[�h
        CsvData(4) = ""                                 ' ����於
        CsvData(5) = ""                                 ' �E�v
        CsvData(6) = "0"                                ' �˗�����
        CsvData(7) = "0"                                ' �˗����z
        CsvData(8) = "0"                                ' ��������
        CsvData(9) = "0"                                ' �������z
        CsvData(10) = ""                                ' ���l
        CsvData(11) = ""                                ' �敪
        CsvData(12) = ""                                ' �U�֏����敪
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' �^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("����於")
        CSVObject.Output("�E�v")
        CSVObject.Output("�˗�����")
        CSVObject.Output("�˗����z")
        CSVObject.Output("��������")
        CSVObject.Output("�������z")
        CSVObject.Output("���l")
        CSVObject.Output("�敪")
        CSVObject.Output("�U�֏����敪", False, True)
    End Function

    Public Function OutputCSVKekkaSysError(ByVal aItakuCode As String, ByVal fSyoriKbn As String, ByVal outFileName As String, ByVal oraDB As CASTCommon.MyOracle) As Boolean
        ' �b�r�u�t�@�C���쐬
        Itakusyamei = "ERROR"
        CreateCsvFile()

        CsvData(2) = aItakuCode
        CsvData(3) = ""
        CsvData(11) = "1"

        CsvData(12) = fSyoriKbn                 ' �U�֏����敪

        ' �O�s��
        CsvData(4) = ""
        CSVObject.Output(CsvData)

        ' �P�s��
        CsvData(2) = "0"
        CsvData(3) = "0"
        CsvData(4) = "�A�g�����ł��܂���ł��� "
        CSVObject.Output(CsvData)

        ' �Q�s��
        CsvData(4) = "�t�@�C�����F" & outFileName
        CSVObject.Output(CsvData)

        ' �R�s��
        CsvData(4) = "�������擾���s"
        CSVObject.Output(CsvData)

        If ReportExecute() = False Then
            Return False
        End If

        Return True
    End Function

    '
    ' �@�\�@ �F �������ʊm�F�\�b�r�u���o�͂���
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCSVKekka(ByVal aReadFmt As CAstFormat.CFormat, ByVal aToriComm As CAstBatch.CommData)
        ' �p�����[�^���
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = aToriComm.INFOParameter
        ' �������
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = aToriComm.INFOToriMast
        ' �˗����׏��
        Dim InfoMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast

        CsvData(1) = InfoPara.FURI_DATE        ' �U�֓�
        CsvData(2) = InfoTori.TORIS_CODE_T     ' ������R�[�h
        CsvData(3) = InfoTori.TORIF_CODE_T     ' ����敛�R�[�h
        CsvData(4) = InfoTori.ITAKU_NNAME_T    ' ����於
        'Select Case InfoTori.TEKIYOU_KBN_T          ' �E�v
        '    Case "0"
        '        CsvData(5) = InfoMei.KTEKIYO
        '    Case "1"
        '        CsvData(5) = InfoMei.NTEKIYO
        '    Case "2"
        '        CsvData(5) = "�f�[�^�E�v"
        'End Select
        CsvData(5) = InfoMei.ITAKU_CODE         ' �ϑ��҃R�[�h
        CsvData(6) = InfoMei.TOTAL_IRAI_KEN.ToString                        ' �˗�����
        CsvData(7) = InfoMei.TOTAL_IRAI_KIN.ToString                        ' �˗����z
        CsvData(8) = (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN).ToString   ' ��������
        CsvData(9) = (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN).ToString   ' �������z
        CsvData(10) = ""                       ' ���l
        If aReadFmt.InfoMeisaiMast.DUPLICATE_KBN = "" Then
            If InfoMei.TOTAL_IRAI_KEN <> (InfoMei.TOTAL_KEN - InfoMei.TOTAL_IJO_KEN) Then
                CsvData(10) = "�����ُ�"
            ElseIf InfoMei.TOTAL_IRAI_KIN <> (InfoMei.TOTAL_KIN - InfoMei.TOTAL_IJO_KIN) Then
                CsvData(10) = "���z�ُ�"
            End If
        Else
            CsvData(10) = "��d��������"
        End If
        CsvData(11) = "0"                       ' �敪
        CsvData(12) = InfoTori.FSYORI_KBN_T     ' �U�֏����敪

        CSVObject.Output(CsvData)
    End Sub

    Public Overloads Overrides Function ReportExecute() As Boolean
        Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
    End Function
End Class
