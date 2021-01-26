Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.Collections.Generic
' �������ʊm�F�\(���U�_�񌋉�)����N���X
Class clsKFJP044
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP044"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP044_�������ʊm�F�\(���U�_�񌋉�).rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F �������ʊm�F�\(���U�_�񌋉�)���f�[�^�ɏ�������
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
        Public FuriCode As String
        Public KigyoCode As String
        Public KeiyakuSit As String
        Public KeiyakuKamoku As String
        Public KeiyakuKouza As String
        Public KeiyakuKname As String
        Public RecordNo As Long
        Public ItakuNName As String
        Public Kamoku As String
        Public OpeCode As String
        Public ErrCode As String
        Public ErrMsg As String
        Public TimeStamp As String
        Public Bikou As String
        Public Jikinko As String
    End Structure
    Public RecordCnt As Long
    '
    ' �@�\�@ �F ���U�_��}�X�^���������̒��o���s��
    '
    ' ���l�@ �F 
    '
    Public Function SetData(ByRef Item As UpdateInfo, ByVal MainDB As CASTCommon.MyOracle) As Boolean
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_JR")
            SQL.Append(",TORIF_CODE_JR")
            SQL.Append(",FURI_CODE_JR")
            SQL.Append(",KIGYO_CODE_JR")
            SQL.Append(",FURI_DATE_JR")
            SQL.Append(",TSIT_NO_JR")
            SQL.Append(",KAMOKU_JR")
            SQL.Append(",KOUZA_JR")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",KEIYAKU_KNAME_K")
            SQL.Append(" FROM JIKEIMAST,TORIMAST,MEIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_JR ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_JR ")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_JR ")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_JR ")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_JR ")
            SQL.Append(" AND RECORD_NO_K = MEI_RECORD_NO_JR ")
            SQL.Append(" AND TIME_STAMP_JR  = " & SQ(Item.TimeStamp))
            SQL.Append(" AND KAMOKU_CODE_JR = " & SQ(Item.Kamoku))
            SQL.Append(" AND OPE_CODE_JR    = " & SQ(Item.OpeCode))
            SQL.Append(" AND RECORD_NO_JR   = " & Item.RecordNo)

            If oraReader.DataReader(SQL) = True Then
                Item.TorisCode = oraReader.GetString("TORIS_CODE_JR")
                Item.TorifCode = oraReader.GetString("TORIF_CODE_JR")
                Item.FuriCode = oraReader.GetString("FURI_CODE_JR")
                Item.KigyoCode = oraReader.GetString("KIGYO_CODE_JR")
                Item.ItakuNName = oraReader.GetString("ITAKU_NNAME_T")
                Item.FuriDate = oraReader.GetString("FURI_DATE_JR")
                Item.KeiyakuSit = oraReader.GetString("TSIT_NO_JR")
                Item.KeiyakuKamoku = oraReader.GetString("KAMOKU_JR")
                Item.KeiyakuKouza = oraReader.GetString("KOUZA_JR")
                Item.KeiyakuKname = oraReader.GetString("KEIYAKU_KNAME_K")
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            MainLOG.Write("�������ʊm�F�\(���U�_�񌋉�)������擾", "���s", ex.Message)

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
                MainLOG.Write("�������ʊm�F�\(���U�_�񌋉�)���", "���s", "����Ώۖ���")
                Return False
            End If

            CreateCsvFile()
            Dim Today As String = Date.Now.ToString("yyyyMMdd")
            Dim Time As String = Date.Now.ToString("HHmmss")
            For No As Integer = 0 To DataList.Count - 1
                OutputCsvData(Today, True)                          '������
                OutputCsvData(Time, True)                           '�^�C���X�^���v
                OutputCsvData(DataList.Item(No).FuriCode, True)
                OutputCsvData(DataList.Item(No).KigyoCode, True)
                OutputCsvData(DataList.Item(No).TorisCode, True)    '������R�[�h
                OutputCsvData(DataList.Item(No).TorifCode, True)    '����敛�R�[�h
                OutputCsvData(DataList.Item(No).ItakuNName, True)   '�ϑ��Җ�
                OutputCsvData(DataList.Item(No).KeiyakuSit, True)     '
                OutputCsvData(GetTenmast(DataList.Item(No).Jikinko, DataList.Item(No).KeiyakuSit, MainDB), True)     '�x�X��
                OutputCsvData(DataList.Item(No).KeiyakuKamoku, True)     '
                OutputCsvData(DataList.Item(No).KeiyakuKouza, True)     '
                OutputCsvData(DataList.Item(No).KeiyakuKname, True)     '
                OutputCsvData(DataList.Item(No).ErrCode, True)      '�G���[�R�[�h
                OutputCsvData(DataList.Item(No).ErrMsg, True) '�G���[���b�Z�[�W
                OutputCsvData(DataList.Item(No).ErrMsg, True, True) '���l
                RecordCnt += 1
            Next

            CloseCsv()
            Return True
        Catch ex As Exception
            MainLOG.Write("�������ʊm�F�\(���U�_�񌋉�)���", "���s", ex.ToString)

            Return False
        End Try
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
