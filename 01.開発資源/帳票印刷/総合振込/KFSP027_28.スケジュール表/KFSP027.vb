Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic

Class KFSP027
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal PrintNo As Integer)
        If PrintNo = 1 Then
            ' CSV�t�@�C���Z�b�g
            InfoReport.ReportName = "KFSP027"

            ' ��`�̖��Z�b�g
            ReportBaseName = "KFSP027_���ԃX�P�W���[���\.rpd"
        Else
            ' CSV�t�@�C���Z�b�g
            InfoReport.ReportName = "KFSP028"

            ' ��`�̖��Z�b�g
            ReportBaseName = "KFSP028_�U�����w��X�P�W���[���\.rpd"
        End If

        '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�\�[�g��INI���Ή��j------------------------------------ START
        SORT_KEY = GCOM.GetObjectParam(InfoReport.ReportName, "SORT", "1")
        '2017/05/08 �^�X�N�j���� ADD �W���ŏC���i�\�[�g��INI���Ή��j------------------------------------ END

    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F CSV�t�@�C���ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    '
    ' �@�\�@ �F ����f�[�^�̍쐬
    '
    ' ���l�@ �F 
    '
    Public Function MakeRecord() As Boolean
        Dim oraDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            oraReader = New CASTCommon.MyOracleReader(oraDB)

            SQL.Append("SELECT TORIS_CODE_S")                   '������R�[�h
            SQL.Append(" ,TORIF_CODE_S")                        '����敛�R�[�h
            SQL.Append(" ,ITAKU_NNAME_T")                       '�ϑ��Ҋ�����
            SQL.Append(" ,ITAKU_CODE_S")                        '�ϑ��҃R�[�h
            SQL.Append(" ,SYUBETU_S")                           '���
            SQL.Append(" ,TOUROKU_FLG_S")                       '�o�^�σt���O
            SQL.Append(" ,HASSIN_FLG_S")                        '���M�σt���O
            SQL.Append(" ,KAKUHO_FLG_S")                        '�m�ۍσt���O
            SQL.Append(" ,KESSAI_FLG_S")                        '���ύσt���O
            SQL.Append(" ,TESUUTYO_FLG_S")                      '�萔�������σt���O
            SQL.Append(" ,FURI_CODE_S")                         '�U�փR�[�h
            SQL.Append(" ,KIGYO_CODE_S")                        '��ƃR�[�h
            SQL.Append(" ,BAITAI_CODE_S")                       '�}�̃R�[�h
            SQL.Append(" ,FURI_DATE_S")                         '�U����
            SQL.Append(" ,MOTIKOMI_SEQ_S")                      '������
            SQL.Append(" ,KAKUHO_YDATE_S")                      '�m�ۗ\���
            SQL.Append(" ,KAKUHO_DATE_S")                       '�m�ۓ�
            SQL.Append(" ,HASSIN_YDATE_S")                      '���M�\���
            SQL.Append(" ,HASSIN_DATE_S")                       '���M��
            SQL.Append(" ,KESSAI_KBN_T")                        '���ϋ敪
            SQL.Append(" ,KESSAI_PATN_T")                       '�����m�ە��@

            SQL.Append(" FROM S_SCHMAST,S_TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='3'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            Select Case PrintKbn
                Case 1  '�`��
                    'SQL.Append(" AND BAITAI_CODE_S = '00'")
                    SQL.Append(" AND BAITAI_CODE_S IN ('00','10')")    '2012/06/30 �W���Ł@WEB�`���Ή�
                Case 2  '�}��
                    '2013/12/24 saitou �W���� �O���}�̑Ή� UPD -------------------------------------------------->>>>
                    SQL.Append(" AND BAITAI_CODE_S IN ('01','05','06','11','12','13','14','15')") 'FD3.5/MT/CMT/DVD/���̑�
                    'SQL.Append(" AND BAITAI_CODE_S IN ('01','05', '06')") 'FD3.5/MT/CMT
                    '2013/12/24 saitou �W���� �O���}�̑Ή� UPD --------------------------------------------------<<<<
                Case 3  '�˗���
                    SQL.Append(" AND BAITAI_CODE_S = '04'")
                Case 4  '�`�[
                    SQL.Append(" AND BAITAI_CODE_S = '09'")
            End Select
            If PrintNo = 2 Then '�U�����w��
                SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            Else                '�Ώی��w��
                Dim strKIJYUN_DATE1 As String = FuriDate & "01"
                Dim strKIJYUN_DATE2 As String = FuriDate & "31"
                SQL.Append("  AND (FURI_DATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR KAKUHO_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR HASSIN_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR KESSAI_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR TESUU_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" )")
            End If
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�\�[�g��INI���Ή��j------------------------------------ START
            If SORT_KEY = "" Then
                SQL.Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")
            Else
                SQL.Append(" ORDER BY " & SORT_KEY)
            End If
            'SQL.Append(" ORDER BY FURI_DATE_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")
            '2017/05/08 �^�X�N�j���� CHG �W���ŏC���i�\�[�g��INI���Ή��j------------------------------------ END

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    'GCOM.NzStr��NULL�l�΍�
                    OutputCsvData(mMatchingDate, True)                                      '������
                    OutputCsvData(mMatchingTime, True)                                      '�^�C���X�^���v
                    '2010/01/06 �ǉ�
                    Select Case PrintKbn    '����敪
                        Case 0
                            OutputCsvData("���U�@�S��", True)
                        Case 1
                            OutputCsvData("���U�@�`��", True)
                        Case 2
                            OutputCsvData("���U�@�}��", True)
                        Case 3
                            OutputCsvData("���U�@�˗���", True)
                        Case 4
                            OutputCsvData("���U�@�`�[", True)
                    End Select
                    '=====================
                    If PrintNo = 1 Then                                                     '�Ώ۔N��(���Ԃ̂�)
                        OutputCsvData(FuriDate, True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIS_CODE_S")), True)    '������R�[�h
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("TORIF_CODE_S")), True)    '����敛�R�[�h
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_NNAME_T")), True)   '�ϑ��Җ�
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("ITAKU_CODE_S")), True)    '�ϑ��҃R�[�h
                    If GCOM.NzStr(oraReader.GetString("TOUROKU_FLG_S")) = "1" Then          '�o�^
                        OutputCsvData("��", True)
                    Else
                        OutputCsvData("��", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KAKUHO_FLG_S")) = "1" Then           '�m��
                        OutputCsvData("��", True)
                    Else
                        OutputCsvData("��", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HASSIN_FLG_S")) = "1" Then           '���M
                        OutputCsvData("��", True)
                    Else
                        OutputCsvData("��", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KESSAI_FLG_S")) = "1" Then           '����
                        OutputCsvData("��", True)
                    Else
                        OutputCsvData("��", True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("TESUUTYO_FLG_S")) = "1" Then         '�萔��
                        OutputCsvData("��", True)
                    Else
                        OutputCsvData("��", True)
                    End If
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_CODE_S")), True)     '�U�փR�[�h
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("KIGYO_CODE_S")), True)    '��ƃR�[�h
                    Select Case GCOM.NzStr(oraReader.GetString("SYUBETU_S"))
                        Case "21"
                            OutputCsvData("���U", True)
                        Case "11"
                            OutputCsvData("���^", True)
                        Case "12"
                            OutputCsvData("�ܗ^", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                    '�}�̖����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_���U_�}�̃R�[�h.TXT"), _
                                                                GCOM.NzStr(oraReader.GetString("BAITAI_CODE_S"))), True)
                    'Select Case GCOM.NzStr(oraReader.GetString("BAITAI_CODE_S"))            '�}�̃R�[�h
                    '    Case "00"
                    '        OutputCsvData("�`��", True)
                    '    Case "01"
                    '        OutputCsvData("FD3.5", True)
                    '    Case "04"
                    '        OutputCsvData("�˗���", True)
                    '    Case "05"
                    '        OutputCsvData("MT", True)
                    '    Case "06"
                    '        OutputCsvData("CMT", True)
                    '    Case "09"
                    '        OutputCsvData("�`�[", True)
                    '        '2012/06/30 �W���Ł@WEB�`���Ή�
                    '    Case "10"
                    '        OutputCsvData("WEB�`��", True)
                    '        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD -------------------------------------------------->>>>
                    '    Case "11"
                    '        OutputCsvData("DVD-RAM", True)
                    '    Case "12"
                    '        OutputCsvData("���̑�", True)
                    '    Case "13"
                    '        OutputCsvData("���̑�", True)
                    '    Case "14"
                    '        OutputCsvData("���̑�", True)
                    '    Case "15"
                    '        OutputCsvData("���̑�", True)
                    '        '2013/12/24 saitou �W���� �O���}�̑Ή� ADD --------------------------------------------------<<<<
                    '    Case Else
                    '        OutputCsvData("", True)
                    'End Select
                    ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("FURI_DATE_S")), True)     '�U����
                    OutputCsvData(GCOM.NzStr(oraReader.GetString("MOTIKOMI_SEQ_S")), True)  '������
                    If GCOM.NzStr(oraReader.GetString("KESSAI_PATN_T")) = "2" OrElse GCOM.NzStr(oraReader.GetString("KAKUHO_YDATE_S")) = "00000000" Then  '�m�ۗ\���
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KAKUHO_YDATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("KESSAI_PATN_T")) = "2" OrElse GCOM.NzStr(oraReader.GetString("KAKUHO_DATE_S")) = "00000000" Then   '�m�ۓ�
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("KAKUHO_DATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HASSIN_YDATE_S")) = "00000000" Then  '���M�\���
                        OutputCsvData("", True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HASSIN_YDATE_S")), True)
                    End If
                    If GCOM.NzStr(oraReader.GetString("HASSIN_DATE_S")) = "00000000" Then   '���M��
                        OutputCsvData("", True, True)
                    Else
                        OutputCsvData(GCOM.NzStr(oraReader.GetString("HASSIN_DATE_S")), True, True)
                    End If

                    oraReader.NextRead()
                    RecordCnt += 1
                End While
            Else
                BatchLog.Write("���R�[�h�쐬", "���s", "����ΏۂȂ�")
                RecordCnt = -1
                Return False
            End If
            Return True
        Catch ex As Exception
            BatchLog.Write("���R�[�h�쐬", "���s", ex.ToString)
            Return False
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
End Class