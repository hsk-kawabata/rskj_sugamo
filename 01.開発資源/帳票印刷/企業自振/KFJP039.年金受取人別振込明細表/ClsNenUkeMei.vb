Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

' 
Public Class ClsNenUkeMei

    '�^�C���X�^���v�擾
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '���ݓ��t
    
    '�p�����[�^
    Protected Friend ToriSCode As String                '������R�[�h
    Protected Friend ToriFCode As String                '����敛�R�[�h
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
            nRet = PrintNenUkeMei()

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
    Private Function PrintNenUkeMei() As Integer
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim PrnNenSitCheck As New ClsPrnNenUkeMei()
        Dim strPrinterName As String = ""
        Dim CSVName As String = ""
        Dim bSQL As Boolean
        Dim strKamoku As String = ""
        
        Try
            strPrinterName = CASTCommon.GetFSKJIni("COMMON", "PRINTER_1")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT ITAKU_NNAME_T,KEIYAKU_SIT_K")
            SQL.Append(", YOBI3_K, KEIYAKU_KNAME_K")
            SQL.Append(", KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
            SQL.Append(", FURIKIN_K, YOBI1_K, YOBI2_K,TORIS_CODE_K,TORIF_CODE_K")
            SQL.Append(" FROM NENKINMAST, SCHMAST, TORIMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & FuriDate & "'")
            If Not (ToriSCode = "9999999999" And ToriFCode = "99") Then
                SQL.Append(" AND TORIS_CODE_K = '" & ToriSCode & "'")
                SQL.Append(" AND TORIF_CODE_K = '" & ToriFCode & "'")
            End If
            SQL.Append(" AND DATA_KBN_K = '2'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_T")
            SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_K = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_K = FURI_DATE_S")
            SQL.Append(" ORDER BY TORIS_CODE_K, TORIF_CODE_K, KEIYAKU_SIT_K")

            bSQL = OraReader.DataReader(SQL)

            If bSQL = True Then

                CSVName = PrnNenSitCheck.CreateCsvFile()

                Do
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
                            strKamoku = "���̑�"
                    End Select

                    '--------------------------------------
                    '���ڏ��o
                    '--------------------------------------
                    With PrnNenSitCheck
                        .OutputCsvData(mMatchingDate)                                   '������
                        .OutputCsvData(OraReader.GetString("TORIS_CODE_K"))             '������R�[�h
                        .OutputCsvData(OraReader.GetString("TORIF_CODE_K"))             '����敛�R�[�h
                        .OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))            '�ϑ��Җ�
                        .OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))            '�_��x�X�R�[�h
                        .OutputCsvData(FuriDate)                                        '�U�֓�
                        .OutputCsvData(OraReader.GetString("YOBI3_K"))                  '�N���؏��ԍ�
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)    '���l��
                        .OutputCsvData(strKamoku, True)                                 '�_��Ȗ�
                        .OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))          '�_������ԍ�
                        .OutputCsvData(OraReader.GetString("FURIKIN_K"))                '�U�����z
                        .OutputCsvData(OraReader.GetString("YOBI2_K"))                  '�����ԍ�
                        .OutputCsvData(OraReader.GetString("YOBI1_K"), True)            '�o�^�X�ܖ�
                        .OutputCsvData("�W�t��", True, True)                            '�������@�i�W�t���Œ�j
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
