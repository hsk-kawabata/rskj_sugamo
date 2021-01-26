Imports System.Globalization
Imports System.Text

' �n��`���f�[�^���M�A���[
Class ClsPrnChikuSoufu
    Inherits CAstReports.ClsReportBase

    Public CsvData(16) As String

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP051"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP051_�n��`���f�[�^���M�A���[.rpd"

        ' ������
        CsvData(0) = ""                                 ' �S����
        CsvData(1) = ""                                 ' �d�b�ԍ�
        CsvData(2) = ""                                 ' ���Z�@�֖�
        CsvData(3) = "0"                                ' �����������P
        CsvData(4) = "0"                                ' �����������Q
        CsvData(5) = "0"                                ' �����������R
        CsvData(6) = "0"                                ' �����������S
        CsvData(7) = "0"                                ' �����������T
        CsvData(8) = "0"                                ' �����������U
        CsvData(9) = "0"                                ' �����������V
        CsvData(10) = "0"                               ' ���X���������P
        CsvData(11) = "0"                               ' ���X���������Q
        CsvData(12) = "0"                               ' ���X�����������R
        CsvData(13) = "0"                               ' ���X�������S
        CsvData(14) = "0"                               ' ���X���������T
        CsvData(15) = "0"                               ' ���X���������U
        CsvData(16) = "0"                               ' ���X���������V

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' �^�C�g���s
        CSVObject.Output("�S����")
        CSVObject.Output("�d�b�ԍ�")
        CSVObject.Output("���Z�@�֖�")
        CSVObject.Output("�����������P")
        CSVObject.Output("�����������Q")
        CSVObject.Output("�����������R")
        CSVObject.Output("�����������S")
        CSVObject.Output("�����������T")
        CSVObject.Output("�����������U")
        CSVObject.Output("�����������V")
        CSVObject.Output("���X���������P")
        CSVObject.Output("���X���������Q")
        CSVObject.Output("���X���������R")
        CSVObject.Output("���X���������S")
        CSVObject.Output("���X���������T")
        CSVObject.Output("���X���������U")
        CSVObject.Output("���X���������V", False, True)

        Return file

    End Function

    '
    ' �@�\�@ �F �n��`���f�[�^���M�A���[�b�r�u���o�͂���
    '
    ' ���l�@ �F 
    '
    Public Function OutputCSVKekka(ByVal SyoriKen As String, ByVal Time As String, _
                                   ByVal KINKOBUSYO As String, ByVal KINKOTANTO As String, ByVal KINKOTEL As String, ByVal KINKONAME As String) As Boolean


        Dim SyoriAM As String = ""
        Dim SyoriPM As String = ""

        '2010.02.19 ���E���Ԃ�ini���擾 start
        Dim strBorder As String = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
        If strBorder = "err" OrElse strBorder = "" Then
            strBorder = "1200"
        End If

        If Time < strBorder Then
            SyoriAM = SyoriKen
        Else
            SyoriPM = SyoriKen
        End If
        '2010.02.19 ���E���Ԃ�ini���擾 end

        Try
            '2010/01/20 �����ǉ� =====
            'CsvData(0) = KINKOTANTO
            CsvData(0) = (KINKOBUSYO & " " & KINKOTANTO).Trim
            '=========================
            CsvData(1) = KINKOTEL
            CsvData(2) = KINKONAME
            CsvData(3) = SyoriAM.PadLeft(7, "0"c).Substring(0, 1)
            CsvData(4) = SyoriAM.PadLeft(7, "0"c).Substring(1, 1)
            CsvData(5) = SyoriAM.PadLeft(7, "0"c).Substring(2, 1)
            CsvData(6) = SyoriAM.PadLeft(7, "0"c).Substring(3, 1)
            CsvData(7) = SyoriAM.PadLeft(7, "0"c).Substring(4, 1)
            CsvData(8) = SyoriAM.PadLeft(7, "0"c).Substring(5, 1)
            CsvData(9) = SyoriAM.PadLeft(7, "0"c).Substring(6, 1)
            CsvData(10) = SyoriPM.PadLeft(7, "0"c).Substring(0, 1)
            CsvData(11) = SyoriPM.PadLeft(7, "0"c).Substring(1, 1)
            CsvData(12) = SyoriPM.PadLeft(7, "0"c).Substring(2, 1)
            CsvData(13) = SyoriPM.PadLeft(7, "0"c).Substring(3, 1)
            CsvData(14) = SyoriPM.PadLeft(7, "0"c).Substring(4, 1)
            CsvData(15) = SyoriPM.PadLeft(7, "0"c).Substring(5, 1)
            CsvData(16) = SyoriPM.PadLeft(7, "0"c).Substring(6, 1)

            CSVObject.Output(CsvData)

            Return True

        Catch ex As Exception
            MainLOG.Write("�n��`���f�[�^���M�A���[CSV�쐬", "���s", ex.Message)
            Return False
        Finally
        End Try
    End Function

    'Public Overloads Overrides Function ReportExecute() As Boolean
    '    Return MyBase.ReportExecute(CAstReports.ClsReportBase.PrinterType.Printer5F)
    'End Function
End Class
