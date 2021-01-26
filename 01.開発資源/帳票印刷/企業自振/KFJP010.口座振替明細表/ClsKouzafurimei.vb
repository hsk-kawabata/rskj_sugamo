Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

Public Class ClsKouzafurimei

    '���ݓ��t�Ǝ���
    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    '����
    Protected Friend TORI_CODE As String
    Protected Friend HAISINTIME As String
    Protected Friend PRINTERNAME As String

    Dim strJYUYOUKA As String = ""

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    ' FSKJ.INI �Z�N�V������
    Private ReadOnly AppTOUROKU As String = "REPORTS"

    ' �@�\   �F �����U�֖��ו\���C������
    '
    ' ����   �F �Ȃ�
    '
    ' �߂�l �F 0 - ���� 0�ȊO - �ُ�
    '
    ' ���l   �F 
    '
    Function Main() As Integer
        ' �I���N��
        MainDB = New CASTCommon.MyOracle

        Dim nRet As Integer
        Try

            MainLOG.Write("(�又��)�J�n", "����")

            ' �������
            nRet = PrintKouzafurimei()

        Catch ex As Exception
            MainLOG.Write("(�又��)", "���s", ex.Message & ":" & ex.StackTrace)
            Return -1
        Finally
            MainDB.Close()
            MainLOG.Write("(�又��)�I��", "����")
        End Try

        If nRet < 0 Then
            Return 2
        End If

        Return nRet

    End Function

    ' �@�\   �F �����U�֖��ו\���[�o�͏���
    '
    ' �߂�l �F 0 - ���� �C -1 - �ُ� , 100 - �O��
    '
    ' ���l   �F 
    '
    Private Function PrintKouzafurimei() As Integer

        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim strJikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

        Dim strSort As String = CASTCommon.GetFSKJIni("PRINT", "HAISIN_FLG")
        Dim strSortMei As String = CASTCommon.GetFSKJIni("PRINT", "SORTMEI")

        Dim PrnFurimei As New ClsPrnKouzafurimei(strSort)

        '2013/10/21 saitou �W���C�� UPD -------------------------------------------------->>>>
        '�p�t�H�[�}���X����
        Dim strCenter As String = CASTCommon.GetFSKJIni("COMMON", "CENTER")
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing
        Dim name As String = ""

        Try
            '�܂��A�����ƃX�P�W���[���̌���
            With SQL
                .Length = 0
                .Append("SELECT * FROM SCHMAST, TORIMAST")
                .Append(" WHERE FSYORI_KBN_S = '1'")
                .Append(" AND JIFURI_TIME_STAMP_S = " & SQ(HAISINTIME))
                .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
                .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                .Append(" AND HAISIN_FLG_S = '1'")
                .Append(" AND TYUUDAN_FLG_S = '0'")
                If TORI_CODE <> "000000000000" Then
                    .Append(" AND TORIS_CODE_T = " & SQ(TORI_CODE.Substring(0, 10)))
                    .Append(" AND TORIF_CODE_T = " & SQ(TORI_CODE.Substring(10, 2)))
                End If
                .Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S")
            End With

            If OraReader.DataReader(SQL) = True Then

                If name = "" Then
                    ' �b�r�u���쐬����
                    name = PrnFurimei.CreateCsvFile()
                End If

                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While OraReader.EOF = False
                    '���׃}�X�^�̎Q��
                    With SQL
                        .Length = 0
                        .Append("SELECT * FROM MEIMAST, TENMAST")
                        .Append(" WHERE FSYORI_KBN_K = '1'")
                        .Append(" AND KEIYAKU_KIN_K = " & SQ(strJikinko))
                        .Append(" AND FURI_DATE_K = " & SQ(OraReader.GetString("FURI_DATE_S")))
                        .Append(" AND TORIS_CODE_K = " & SQ(OraReader.GetString("TORIS_CODE_S")))
                        .Append(" AND TORIF_CODE_K = " & SQ(OraReader.GetString("TORIF_CODE_S")))
                        If OraReader.GetString("FMT_KBN_T") = "02" Then
                            .Append(" AND DATA_KBN_K = '3'")
                        Else
                            .Append(" AND DATA_KBN_K = '2'")
                        End If
                        .Append(" AND FURIKETU_CODE_K = 0")
                        If strCenter <> "5" Then
                            .Append(" AND KEIYAKU_KOUZA_K <> '0000000'")
                        End If
                        .Append(" AND KEIYAKU_KIN_K = KIN_NO_N")
                        .Append(" AND KEIYAKU_SIT_K = SIT_NO_N")
                        Select Case String.Concat(strSort, strSortMei)
                            Case "11"    '�����\�[�g�@        ���ו��F��Ƃr�d�p
                                .Append(" ORDER BY KIGYO_SEQ_K")
                            Case "21"    '�x�X�A�����\�[�g�@  ���ו��F��Ƃr�d�p   
                                .Append(" ORDER BY KEIYAKU_SIT_K, KIGYO_SEQ_K")
                            Case "12"    '�����\�[�g�@        ���ו��F�X�Ȗڌ���
                                .Append(" ORDER BY KEIYAKU_SIT_K, KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
                            Case "22"    '�x�X�A�����\�[�g�@  ���ו��F�X�Ȗڌ���
                                .Append(" ORDER BY KEIYAKU_SIT_K, KEIYAKU_KAMOKU_K, KEIYAKU_KOUZA_K")
                            Case Else   '�擾�ł���������"11"����
                                .Append(" ORDER BY KIGYO_SEQ_K")
                        End Select
                    End With

                    If oraMeiReader.DataReader(SQL) = True Then
                        While oraMeiReader.EOF = False
                            Dim strNS As String = ""
                            Dim strKAMOKU As String = ""
                            Dim strTEKIYOU As String = ""

                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("FURI_DATE_K"), True)
                            PrnFurimei.OutputCsvData(mMatchingDate, True)
                            PrnFurimei.OutputCsvData(mMatchingTime, True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("TORIS_CODE_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("TORIF_CODE_K"), True)
                            PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"), True)
                            PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_KIN_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KIN_NNAME_N"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_SIT_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("SIT_NNAME_N"), True)
                            Select Case oraMeiReader.GetString("KEIYAKU_KAMOKU_K")
                                Case "02"
                                    strKAMOKU = "����"
                                Case "01"
                                    strKAMOKU = "����"
                                Case "05"
                                    strKAMOKU = "�[��"
                                Case "37"
                                    strKAMOKU = "�E��"
                                Case Else
                                    strKAMOKU = "����"
                            End Select
                            PrnFurimei.OutputCsvData(strKAMOKU, True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_KOUZA_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KEIYAKU_KNAME_K"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("FURIKIN_K"))

                            Select Case OraReader.GetString("NS_KBN_T")
                                Case "1"
                                    strNS = "��"
                                Case "9"
                                    strNS = "�o"
                                Case Else
                                    strNS = "��"
                            End Select
                            PrnFurimei.OutputCsvData(strNS, True)

                            PrnFurimei.OutputCsvData(OraReader.GetString("FURI_CODE_T"), True)
                            PrnFurimei.OutputCsvData(OraReader.GetString("KIGYO_CODE_T"), True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("KIGYO_SEQ_K"), True)
                            Select Case OraReader.GetString("TEKIYOU_KBN_T")
                                Case "0", "2", "3" '�J�ior�f�[�^
                                    strTEKIYOU = oraMeiReader.GetString("KTEKIYO_K")
                                Case "1" '����
                                    strTEKIYOU = oraMeiReader.GetString("NTEKIYO_K")
                            End Select
                            PrnFurimei.OutputCsvData(strTEKIYOU, True)
                            PrnFurimei.OutputCsvData(oraMeiReader.GetString("JYUYOUKA_NO_K"), True, True)

                            oraMeiReader.NextRead()
                        End While
                    Else
                        MainLOG.Write("����Ώۃf�[�^0��", "����")
                        'Return 100
                    End If

                    oraMeiReader.Close()

                    OraReader.NextRead()
                End While
            Else
                MainLOG.Write("����Ώۃf�[�^0��", "����")
                Return 100
            End If

            PrnFurimei.CloseCsv()

            If PrnFurimei.ReportExecute(PRINTERNAME) = True Then
                MainLOG.Write("���", "����")
                Return 0
            Else
                MainLOG.Write("���", "���s", PrnFurimei.ReportMessage)
                Return -1
            End If

        Catch ex As Exception
            MainLOG.Write("���", "���s", ex.Message)
            Return -1
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not oraMeiReader Is Nothing Then
                oraMeiReader.Close()
                oraMeiReader = Nothing
            End If
        End Try

        'SQL = New StringBuilder(128)
        'SQL.Append("SELECT * FROM MEIMAST,TORIMAST,SCHMAST,TENMAST")
        'SQL.Append(" WHERE FSYORI_KBN_K = '1'")
        'SQL.Append(" AND JIFURI_TIME_STAMP_S = '" & HAISINTIME & "'")
        'SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_S AND TORIF_CODE_K=TORIF_CODE_S AND FURI_DATE_K = FURI_DATE_S ")
        'SQL.Append(" AND TORIS_CODE_K = TORIS_CODE_T AND TORIF_CODE_K=TORIF_CODE_T ")
        'SQL.Append(" AND KIN_NO_N = KEIYAKU_KIN_K AND KEIYAKU_SIT_K = SIT_NO_N ")
        'SQL.Append(" AND HAISIN_FLG_S = '1'")
        'SQL.Append(" AND TYUUDAN_FLG_S = '0'")

        'SQL.Append(" AND KEIYAKU_KIN_K = '" & strJikinko & "'")

        'If TORI_CODE <> "000000000000" Then
        '    SQL.Append(" AND TORIS_CODE_T = '" & TORI_CODE.Substring(0, 10) & "'")
        '    SQL.Append(" AND TORIF_CODE_T = '" & TORI_CODE.Substring(10, 2) & "'")
        'End If
        ''2010.02.01 start
        ''SQL.Append(" AND DATA_KBN_K = '2'")
        'SQL.Append(" AND ((DATA_KBN_K = '2' AND FMT_KBN_T<>'02')")
        'SQL.Append(" OR (DATA_KBN_K = '3' AND FMT_KBN_T='02'))")
        ''2010.02.01 end
        'SQL.Append(" AND FURIKETU_CODE_K = 0")
        'If CASTCommon.GetFSKJIni("COMMON", "CENTER") <> "5" Then
        '    SQL.Append(" AND KEIYAKU_KOUZA_K <> '0000000'")
        'End If
        'Select Case String.Concat(strSort, strSortMei)
        '    Case "11"    '�����\�[�g�@        ���ו��F��Ƃr�d�p
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KIGYO_SEQ_K ASC")
        '    Case "21"    '�x�X�A�����\�[�g�@  ���ו��F��Ƃr�d�p   
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KEIYAKU_SIT_K ASC,KIGYO_SEQ_K ASC")
        '    Case "12"    '�����\�[�g�@        ���ו��F�X�Ȗڌ���
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KEIYAKU_SIT_K ASC,KEIYAKU_KAMOKU_K ASC,KEIYAKU_KOUZA_K ASC")
        '    Case "22"    '�x�X�A�����\�[�g�@  ���ו��F�X�Ȗڌ���
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KEIYAKU_SIT_K ASC,KEIYAKU_KAMOKU_K ASC,KEIYAKU_KOUZA_K ASC")
        '    Case Else   '�擾�ł���������"11"����
        '        SQL.Append(" ORDER BY FURI_DATE_S ASC,KEIYAKU_SIT_K ASC,TORIS_CODE_K ASC,TORIF_CODE_K ASC,KIGYO_SEQ_K ASC")
        'End Select

        'Dim name As String = ""


        'Dim bSQL As Boolean
        'bSQL = OraReader.DataReader(SQL)

        'If bSQL = True Then

        '    name = PrnFurimei.CreateCsvFile

        '    If name = "" Then
        '        ' �b�r�u���쐬����
        '        name = PrnFurimei.CreateCsvFile()
        '    End If

        '    Do
        '        Dim strNS As String = ""
        '        Dim strKAMOKU As String = ""
        '        Dim strTEKIYOU As String = ""

        '        PrnFurimei.OutputCsvData(OraReader.GetString("FURI_DATE_K"))
        '        PrnFurimei.OutputCsvData(mMatchingDate)
        '        PrnFurimei.OutputCsvData(mMatchingTime)
        '        PrnFurimei.OutputCsvData(OraReader.GetString("TORIS_CODE_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("TORIF_CODE_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("SIT_NNAME_N"))
        '        Select Case OraReader.GetString("KEIYAKU_KAMOKU_K")
        '            Case "02"
        '                strKAMOKU = "����"
        '            Case "01"
        '                strKAMOKU = "����"
        '            Case "05"
        '                strKAMOKU = "�[��"
        '            Case "37"
        '                strKAMOKU = "�E��"
        '            Case Else
        '                strKAMOKU = "����"
        '        End Select
        '        PrnFurimei.OutputCsvData(strKAMOKU)
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("FURIKIN_K"))

        '        Select Case OraReader.GetString("NS_KBN_T")
        '            Case "1"
        '                strNS = "��"
        '            Case "9"
        '                strNS = "�o"
        '            Case Else
        '                strNS = "��"
        '        End Select
        '        PrnFurimei.OutputCsvData(strNS)

        '        PrnFurimei.OutputCsvData(OraReader.GetString("FURI_CODE_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KIGYO_CODE_T"))
        '        PrnFurimei.OutputCsvData(OraReader.GetString("KIGYO_SEQ_K"))
        '        Select Case OraReader.GetString("TEKIYOU_KBN_T")
        '            Case "0", "2", "3" '�J�ior�f�[�^
        '                strTEKIYOU = OraReader.GetString("KTEKIYO_K")
        '            Case "1" '����
        '                strTEKIYOU = OraReader.GetString("NTEKIYO_K")
        '        End Select
        '        PrnFurimei.OutputCsvData(strTEKIYOU)
        '        PrnFurimei.OutputCsvData(OraReader.GetString("JYUYOUKA_NO_K"), False, True)

        '        OraReader.NextRead()

        '    Loop Until OraReader.EOF    ' EOF�܂ō�Ƃ��J��Ԃ��B
        '    OraReader.Close()

        '    PrnFurimei.CloseCsv()

        '    If PrnFurimei.ReportExecute(PRINTERNAME) = True Then
        '        MainLOG.Write("���", "����")
        '        Return 0
        '    Else
        '        MainLOG.Write("���", "���s", PrnFurimei.ReportMessage)
        '        Return -1
        '    End If
        'Else
        '    MainLOG.Write("����Ώۃf�[�^�O��", "����")
        '    Return 100
        'End If
        '2013/10/21 saitou �W���C�� UPD --------------------------------------------------<<<<

    End Function

End Class
