Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Collections.Generic
' �������ʊm�F�\(�������ό���)����N���X
Class KFKP003
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFKP003"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFK003_�������ʊm�F�\(�������ό���).rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F �������ʊm�F�\(�������ό���)���f�[�^�ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '�X�V���ڍ\����
    Public Structure UpdateInfo
        Public TorisCode As String
        Public TorifCode As String
        Public FuriDate As String
        Public RecordNo As Long
        Public ItakuNName As String
        Public Kamoku As String
        Public OpeCode As String
        Public ErrCode As String
        Public ErrMsg As String
        Public TimeStamp As String
        Public Bikou As String
    End Structure
    Public RecordCnt As Long
    '
    ' �@�\�@ �F ���σ}�X�^���������̒��o���s��
    '
    ' ���l�@ �F 
    '
    Public Function SetData(ByRef Item As UpdateInfo, ByVal MainDB As CASTCommon.MyOracle) As Boolean
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_KR")
            SQL.Append(",TORIF_CODE_KR")
            SQL.Append(",FURI_DATE_KR")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(" FROM KESSAIMAST,TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_KR ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_KR ")
            SQL.Append(" AND TIME_STAMP_KR  = " & SQ(Item.TimeStamp))
            SQL.Append(" AND KAMOKU_CODE_KR = " & SQ(Item.Kamoku))
            SQL.Append(" AND OPE_CODE_KR    = " & SQ(Item.OpeCode))
            SQL.Append(" AND RECORD_NO_KR   = " & Item.RecordNo)

            If oraReader.DataReader(SQL) = True Then
                Item.TorisCode = oraReader.GetString("TORIS_CODE_KR")
                Item.TorifCode = oraReader.GetString("TORIF_CODE_KR")
                Item.ItakuNName = oraReader.GetString("ITAKU_NNAME_T")
                Item.FuriDate = oraReader.GetString("FURI_DATE_KR")
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            MainLOG.Write("�������ʊm�F�\(�������ό���)������擾", "���s", ex.Message)

        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
    '
    ' �@�\�@ �F ���[����b�r�u�t�@�C�����쐬����
    '
    ' ���l�@ �F 
    '
    Public Function MakeRecord(ByVal DataList As List(Of UpdateInfo)) As Boolean
        Try
            If DataList.Count = 0 Then
                '����Ώۖ��� 
                MainLOG.Write("�������ʊm�F�\(�������ό���)���", "���s", "����Ώۖ���")

                Return False
            End If

            CreateCsvFile()
            Dim Today As String = Date.Now.ToString("yyyyMMdd")
            Dim Time As String = Date.Now.ToString("HHmmss")
            For No As Integer = 0 To DataList.Count - 1
                OutputCsvData(Today, True)                          '������
                OutputCsvData(Time, True)                           '�^�C���X�^���v
                OutputCsvData(DataList.Item(No).TorisCode, True)    '������R�[�h
                OutputCsvData(DataList.Item(No).TorifCode, True)    '����敛�R�[�h
                OutputCsvData(DataList.Item(No).ItakuNName, True)   '�ϑ��Җ�
                OutputCsvData(DataList.Item(No).FuriDate, True)     '�U�֓�
                OutputCsvData(DataList.Item(No).Kamoku, True)       '�Ȗ�
                OutputCsvData(DataList.Item(No).OpeCode, True)      '�I�y�R�[�h
                OutputCsvData(DataList.Item(No).ErrCode, True)      '�G���[�R�[�h
                OutputCsvData(DataList.Item(No).ErrMsg, True, True) '�G���[���b�Z�[�W
                RecordCnt += 1
            Next

            CloseCsv()
            Return True
        Catch ex As Exception
            MainLOG.Write("�������ʊm�F�\(�������ό���)���", "���s", ex.ToString)

            Return False
        End Try
    End Function
End Class
