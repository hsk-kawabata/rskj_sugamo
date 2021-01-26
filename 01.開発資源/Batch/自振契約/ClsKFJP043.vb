Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.IO
Imports System.Collections
Imports CAstBatch

Class ClsKFJP043
    Inherits CAstReports.ClsReportBase

    Public CsvData(12) As String

    Private strKESSAI_DATE As String                    ' ���ϓ�

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP043"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP043_�������ʊm�F�\(���U�_��).rpd"

        CsvData(0) = "00010101"                                     ' ������
        CsvData(1) = "00010101"                                     ' �^�C���X�^���v
        CsvData(2) = ""                                             ' �U�փR�[�h
        CsvData(3) = ""                                             ' ��ƃR�[�h
        CsvData(4) = ""                                             ' ������R�[�h
        CsvData(5) = ""                                             ' ����敛�R�[�h
        CsvData(6) = ""                                             ' ����於
        CsvData(7) = ""                                             ' �_��Ҏx�X�R�[�h
        CsvData(8) = ""                                             ' �_��Ҏx�X��
        CsvData(9) = ""                                             ' �_��҉Ȗ�
        CsvData(10) = ""                                            ' �_��Ҍ����ԍ�
        CsvData(11) = ""                                            ' �_��҃J�i����
        CsvData(12) = ""                                            ' ���l

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' �^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("�U�փR�[�h")
        CSVObject.Output("��ƃR�[�h")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("����於")
        CSVObject.Output("�_��Ҏx�X�R�[�h")
        CSVObject.Output("�_��Ҏx�X��")
        CSVObject.Output("�_��҉Ȗ�")
        CSVObject.Output("�_��Ҍ����ԍ�")
        CSVObject.Output("�_��҃J�i����")
        CSVObject.Output("���l", False, True)

        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Return file
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

    End Function

    '
    ' �@�\�@ �F ���U�_�񏈗����ʊm�F�\�b�r�u���o�͂���
    '
    ' ���l�@ �F 
    '
    Public Function OutputCSVKekka(ByVal ary As ArrayList, ByVal jikinko As String, ByVal strDate As String, ByVal strTime As String) As Integer


        Dim JData As New CAstFormKes.ClsFormKes.JifkeiData
        Dim fmt10004 As New CAstFormKes.ClsFormSikinFuri.T_10004

        Dim cnt As Integer = ary.Count - 1 '���[�v��

        Try

            For i As Integer = 0 To cnt

                JData.Init()
                fmt10004.Init()

                JData = CType(ary.Item(i), CAstFormKes.ClsFormKes.JifkeiData)
                fmt10004.Data = JData.record320

                ' �b�r�u�f�[�^�ݒ�
                CsvData(0) = strDate                                                ' ������
                CsvData(1) = strTime                                                ' �^�C���X�^���v
                CsvData(2) = JData.FuriCode                                         ' �U�փR�[�h
                CsvData(3) = JData.KigyoCode                                        ' ��ƃR�[�h
                CsvData(4) = JData.TorisCode                                        ' ������R�[�h
                CsvData(5) = JData.TorifCode                                        ' ����敛�R�[�h
                CsvData(6) = JData.ToriNName.Trim                                   ' ����於
                CsvData(7) = fmt10004.GENTEN_NO                                     ' �_��Ҏx�X�R�[�h
                CsvData(8) = GetTenmast(jikinko, fmt10004.GENTEN_NO, MainDB)        ' �_��Ҏx�X��
                CsvData(9) = fmt10004.KAMOKU_KOUZA_NO.Substring(0, 2)               ' �_��҉Ȗ�
                CsvData(10) = fmt10004.KAMOKU_KOUZA_NO.Substring(2, 7)              ' �_��Ҍ����ԍ�
                CsvData(11) = JData.KeiyakuKname                                    ' �_��҃J�i����
                CsvData(12) = ""                                                    ' ���l

                '�b�r�u�o�͏���
                If CSVObject.Output(CsvData) = 0 Then
                    Return -1
                End If

            Next

        Catch ex As Exception

            'MainLOG.Write("�������ʊm�F�\(���U�_��)�b�r�u�o��", "���s", ex.Message)
            Return -1
        Finally

        End Try

        Return 0

    End Function

    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strSitName As String = ""

        Try

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")

            If orareader.DataReader(sql) = True Then
                strSitName = orareader.GetString("SIT_NNAME_N")
                Return strSitName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

End Class
