Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

' 
Public Class ClsNenSitCheck

    '�^�C���X�^���v�擾
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '���ݓ��t
    Private mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    '���ݎ���

    '�p�����[�^
    Protected Friend FuriDate As String                 '�U�֓�

    '���ʏ�������
    Private MainDB As CASTCommon.MyOracle               '�p�u���b�N�c�a
    Private ReadOnly AppTOUROKU As String = "REPORTS"   'FSKJ.INI �Z�N�V������
    
    '=======================================================================
    ' �@�\   �F �����U�֖��ו\���C������
    ' ����   �F �Ȃ�
    ' �߂�l �F 0 - ���� 0�ȊO - �ُ�
    ' ���l   �F 
    ' �쐬�� �F 2009/09/29
    ' �X�V�� �F 
    '=======================================================================
    Function Main() As Integer
        Dim nRet As Integer

        Try
            MainDB = New CASTCommon.MyOracle

            '--------------------------------------
            '����������s
            '--------------------------------------
            nRet = PrintKouzafurimei()

            If nRet < 0 Then
                Return 2
            Else
                Return nRet
            End If

        Catch ex As Exception
           BatchLOG.Write("(�又��)", "���s", ex.Message & ":" & ex.StackTrace)
            Return -1

        End Try
    End Function

    '=======================================================================
    ' �@�\   �F �����U�֖��ו\���C������
    ' ����   �F �Ȃ�
    ' �߂�l �F 0 - ���� �C -1 - �ُ� , 100 - �O��
    ' ���l   �F 
    ' �쐬�� �F 2009/09/29
    ' �X�V�� �F 
    '=======================================================================
    Private Function PrintKouzafurimei() As Integer
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim PrnNenSitCheck As New ClsPrnNenSitCheck()
        Dim strNenkinSit As String = "000"
        Dim strPrinterName As String = ""
        Dim CSVName As String = ""
        Dim bSQL As Boolean
        Dim strKamoku As String = ""
        Dim strNenkinSyubetuCode As String = "00"
        Dim strNenkinSyubetuName As String = ""
        Dim strNenkinSyousyo As String = "000000000000"

        Try
            strNenkinSit = CASTCommon.GetFSKJIni("COMMON", "NENKIN_SIT")
            strPrinterName = CASTCommon.GetFSKJIni("COMMON", "PRINTER_1")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT TORIS_CODE_K, TORIF_CODE_K")
            SQL.Append(", FURI_DATE_K, JYUYOUKA_NO_K, YOBI2_K")
            SQL.Append(", KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
            SQL.Append(", KEIYAKU_KNAME_K, FURIKIN_K, KEIYAKU_KIN_K, YOBI1_K")
            SQL.Append(" FROM NENKINMAST, SCHMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & FuriDate & "'")
            SQL.Append(" AND DATA_KBN_K = '2'")
            SQL.Append(" AND KEIYAKU_SIT_K = '" & strNenkinSit & "'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_S")
            SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, YOBI2_K")

            bSQL = OraReader.DataReader(SQL)

            If bSQL = True Then

                CSVName = PrnNenSitCheck.CreateCsvFile()

                Do
                    '--------------------------------------
                    '�N����ʐݒ�
                    '--------------------------------------
                    If OraReader.GetString("JYUYOUKA_NO_K").Length >= 2 Then
                        strNenkinSyubetuCode = OraReader.GetString("JYUYOUKA_NO_K").Substring(0, 2)
                    End If

                    Select Case strNenkinSyubetuCode
                        Case "61"
                            strNenkinSyubetuName = "������"
                        Case "62"
                            strNenkinSyubetuName = "���D���ی�"
                        Case "63"
                            strNenkinSyubetuName = "������"
                        Case "64"
                            strNenkinSyubetuName = "�J��"
                        Case "65"
                            strNenkinSyubetuName = "�V�����ی�"
                        Case "66"
                            strNenkinSyubetuName = "�V�D���ی�"
                        Case "67"
                            strNenkinSyubetuName = "�������Z��"
                    End Select

                    '--------------------------------------
                    '�N���؏��ԍ��ݒ�
                    '--------------------------------------
                    If OraReader.GetString("JYUYOUKA_NO_K").Length >= 17 Then
                        strNenkinSyousyo = OraReader.GetString("JYUYOUKA_NO_K").Substring(2, 15)
                    End If

                    '--------------------------------------
                    '�Ȗڐݒ�
                    '--------------------------------------
                    Select Case OraReader.GetString("KEIYAKU_KAMOKU_K")
                        Case "02"
                            strKamoku = "����"
                        Case "01"
                            strKamoku = "����"
                        Case "05"
                            strKamoku = "�[��"
                        Case "37"
                            strKamoku = "�E��"
                        Case Else
                            strKamoku = "����"
                    End Select

                    '--------------------------------------
                    '���ڏ��o
                    '--------------------------------------
                    With PrnNenSitCheck
                        .OutputCsvData(OraReader.GetString("FURI_DATE_K"))              '�U�֓�
                        .OutputCsvData(mMatchingDate)                                   '������
                        .OutputCsvData(mMatchingTime)                                   '�^�C���X�^���v
                        .OutputCsvData(OraReader.GetString("TORIS_CODE_K"))             '������R�[�h
                        .OutputCsvData(OraReader.GetString("TORIF_CODE_K"))             '����敛�R�[�h
                        .OutputCsvData(strNenkinSyubetuCode)                            '�N����ʃR�[�h
                        .OutputCsvData(strNenkinSyubetuName, True)                      '�N����ʖ�
                        .OutputCsvData(OraReader.GetString("YOBI2_K"))                  '�����ԍ�
                        .OutputCsvData(strNenkinSyousyo)                                '�N���؏��ԍ�
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"), True)      '�_����Z�@�փR�[�h
                        .OutputCsvData(OraReader.GetString("YOBI1_K"), True)            '�_��x�X��
                        .OutputCsvData(strKamoku, True)                                 '�_��Ȗ�
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))          '�_������ԍ�
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)    '�_��҃J�i��
                        .OutputCsvData(OraReader.GetString("FURIKIN_K"), False, True)   '�U�֋��z
                    End With

                    OraReader.NextRead()
                Loop Until OraReader.EOF    ' EOF�܂ō�Ƃ��J��Ԃ��B

                OraReader.Close()
                PrnNenSitCheck.CloseCsv()

                If PrnNenSitCheck.ReportExecute(strPrinterName) = True Then
                    Return 0
                Else
                    BatchLOG.Write("���", "���s", PrnNenSitCheck.ReportMessage)
                    Return -1
                End If
            Else
                BatchLOG.Write("����Ώۃf�[�^�O��", "����")
                Return 100
            End If

        Catch ex As Exception
            BatchLOG.Write("���", "���s", ex.Message)
            Return -1
        End Try
    End Function

End Class
