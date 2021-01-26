Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' WEB�`�����O�ꗗ�\
Class ClsPrnWebDensoLog
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFWP001"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFWP001_WEB�`�����O�ꗗ.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F WEB�`�����O�ꗗ���f�[�^�ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    ' �@�\   �F WEB�`�����O�ꗗ���[�o�͏���
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l   �F 
    '
    Public Function MakeRecord() As Boolean
        Dim SQL As New StringBuilder(128)
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        SQL = New StringBuilder(128)
        SQL.Append("SELECT *")
        SQL.Append(" FROM WEB_RIREKIMAST")
        SQL.Append(" WHERE SAKUSEI_DATE_W = '" & txtSYORI_DATE & "'")
        SQL.Append(" AND STATUS_KBN_W <> '2'")
        SQL.Append(" ORDER BY SAKUSEI_TIME_W")

        Dim bSQL As Boolean

        Try
            bSQL = OraReader.DataReader(SQL)
            If bSQL = True Then
                Do
                    '������
                    OutputCsvData(mMatchingDate, True)
                    '�^�C���X�^���v
                    OutputCsvData(mMatchingTime, True)
                    '�����敪
                    Select Case OraReader.GetString("FSYORI_KBN_W")
                        Case "1"
                            OutputCsvData("���U", True)
                        Case "3"
                            OutputCsvData("���U", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    '�`�����t
                    OutputCsvData(OraReader.GetString("SAKUSEI_DATE_W"), True)
                    '�`������
                    OutputCsvData(OraReader.GetString("SAKUSEI_TIME_W"), True)
                    '���[�U��
                    OutputCsvData(OraReader.GetString("USER_ID_W"), True)
                    '�t�@�C����
                    OutputCsvData(OraReader.GetString("FILE_NAME_W"), True)

                    '2012/12/05 saitou WEB�`�� UPD -------------------------------------------------->>>>
                    '�󎚓��e�Ɍ����Ƌ��z��ǉ�

                    '����M
                    Select Case OraReader.GetString("STATUS_KBN_W")
                        Case "0", "1"
                            OutputCsvData("��t", True)
                        Case "3"
                            OutputCsvData("�ԋp", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    '����
                    OutputCsvData(OraReader.GetInt("END_KEN_W"))
                    '���z
                    OutputCsvData(OraReader.GetInt64("END_KIN_W"), False, True)

                    ''����M
                    'Select Case OraReader.GetString("STATUS_KBN_W")
                    '    Case "0", "1"
                    '        OutputCsvData("��t", True, True)
                    '    Case "3"
                    '        OutputCsvData("�ԋp", True, True)
                    '    Case Else
                    '        OutputCsvData("", True, True)
                    'End Select
                    '2012/12/05 saitou WEB�`�� UPD --------------------------------------------------<<<<

                    OraReader.NextRead()
                Loop Until OraReader.EOF
                Return True
            Else
                BatchLog.Write("���R�[�h�쐬", "���s", "����ΏۂȂ�")
                RecordCnt = -1
                Return False
            End If
        Catch ex As Exception
            BatchLog.Write("���R�[�h�쐬", "���s", ex.ToString)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

End Class
