Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports MenteCommon.clsCommon
Public Class KFJP063
    Inherits CAstReports.ClsReportBase

    '�����ɃR�[�h
    Private Jikinko As String
    '�����ɖ�
    Private JikinkoNm As String
    '������
    Private Busyo As String
    'NHK �R�[�h
    Private NHKCODE As String

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  ' ���ݓ��t
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")    ' ���ݎ���

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFJP063"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFJP063_�a�������U�֕ύX�ʒm��.rpd"
    End Sub

    Public Overrides Function CreateCsvFile() As String

        Return MyBase.CreateCsvFile
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
        Dim SQL As New StringBuilder(128)    ' SQL
        MainDB = New CASTCommon.MyOracle ' �I���N��
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim OraReader2 As New CASTCommon.MyOracleReader(MainDB) '2010/10/04.Sakon�@�ǉ�
        Try
            'INI�t�@�C���擾
            '�����ɃR�[�h
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" Or Jikinko = "" Then
                BatchLog.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD")
                Return False
            End If
            '�����ɖ�
            JikinkoNm = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
            If JikinkoNm.ToUpper = "ERR" Or JikinkoNm = "" Then
                BatchLog.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�����ɖ� ����:PRINT ����:KINKONAME")
                Return False
            End If
            '������
            Busyo = CASTCommon.GetFSKJIni("PRINT", "KINKOBUSYO")
            If Busyo.ToUpper = "ERR" Then '�󔒂͋�����
                BatchLog.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:������ ����:PRINT ����:KINKOBUSYO")
                Return False
            End If
            'NHK �R�[�h
            NHKCODE = CASTCommon.GetFSKJIni("COMMON", "NHKCD")
            If NHKCODE.ToUpper = "ERR" Then '�󔒂͋�����
                BatchLog.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:NHK�R�[�h ����:COMMON ����:NHKCD")
                Return False
            End If

            SQL = New StringBuilder(128)


            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '2010/02/08.Sakon�@�X�ܓ��p���l���|�e���Z�@�ցE�X�ܖ��擾�͕ʓr�s��

            SQL.Append("SELECT")
            SQL.Append(" TORIMAST.ITAKU_CODE_T      ITAKU_CODE_TORIMAST ")      '��ЃR�[�h  
            SQL.Append(",TORIMAST.ITAKU_NNAME_T     ITAKU_NNAME_TORIMAST ")     '��Ж�   
            SQL.Append(",TORIMAST.TSIT_NO_T         TSIT_NO ")                  '���܂ƂߓX��
            SQL.Append(",TORIMAST.FMT_KBN_T         FMT_KBN_TORIMAST ")         '�t�H�[�}�b�g�敪 
            SQL.Append(",MEIMAST.JYUYOUKA_NO_K      JYUYOUKA_NO_MEIMAST ")      '�_��Ҕԍ�    
            SQL.Append(",MEIMAST.KEIYAKU_KNAME_K    KEIYAKU_KNAME_MEIMAST ")    '�_��Җ�     
            SQL.Append(",MEIMAST.KEIYAKU_KNAME_K    YOKINSYA_NAME ")            '�a���Җ�    
            SQL.Append(",MEIMAST.KEIYAKU_SIT_K      KEIYAKU_SIT_MEIMAST ")      '�ύX�O�X��     
            SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K   KEIYAKU_KAMOKU_MEIMAST ")   '�ύX�O���     
            SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K    KEIYAKU_KOUZA_MEIMAST ")    '�ύX�O�����ԍ�    
            SQL.Append(",MEIMAST.TEISEI_SIT_K       TEISEI_SIT_MEIMAST ")       '�ύX��X��    
            SQL.Append(",MEIMAST.TEISEI_KAMOKU_K    TEISEI_KAMOKU_MEIMAST ")    '�ύX����     
            SQL.Append(",MEIMAST.TEISEI_KOUZA_K     TEISEI_KOUZA_MEIMAST")      '�ύX������ԍ�   
            SQL.Append(",MEIMAST.TORIS_CODE_K       TORIS_CODE_MEIMAST ")       '������R�[�h   
            SQL.Append(",MEIMAST.TORIF_CODE_K       TORIF_CODE_MEIMAST ")       '����敛�R�[�h
            SQL.Append(",MEIMAST.FURI_DATA_K        IRAI_DATA_MEIMAST ")        '�˗��f�[�^
            SQL.Append(" FROM ")
            SQL.Append(" MEIMAST ")
            SQL.Append(",TORIMAST ")
            SQL.Append(" WHERE ")
            SQL.Append(" MEIMAST.TORIS_CODE_K = TORIMAST.TORIS_CODE_T ")
            SQL.Append(" AND MEIMAST.TORIF_CODE_K = TORIMAST.TORIF_CODE_T ")
            SQL.Append(" AND MEIMAST.TORIS_CODE_K = '" & strToriSCd & "' ")
            SQL.Append(" AND MEIMAST.TORIF_CODE_K = '" & strToriFCd & "' ")
            SQL.Append(" AND MEIMAST.FURI_DATE_K = '" & strFuriDate & "' ")
            '���ł̏ꍇ���l��
            SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
            SQL.Append(" AND (NVL(TRIM(MEIMAST.TEISEI_SIT_K),'000') <> '000' ")
            SQL.Append("      OR NVL(TRIM(MEIMAST.TEISEI_KAMOKU_K),'00') <> '00' ")
            SQL.Append("      OR NVL(TRIM(MEIMAST.TEISEI_KOUZA_K),'0000000') <> '0000000') ")
            SQL.Append(" ORDER BY TORIMAST.TORIMATOME_SIT_T")
            SQL.Append(",MEIMAST.TORIS_CODE_K")
            SQL.Append(",MEIMAST.TORIF_CODE_K")
            SQL.Append(",MEIMAST.RECORD_NO_K")
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'SQL.Append("SELECT")
            'SQL.Append(" TORIMAST.ITAKU_CODE_T   ITAKU_CODE_TORIMAST ")     ' ��ЃR�[�h  
            'SQL.Append(",TORIMAST.ITAKU_NNAME_T   ITAKU_NNAME_TORIMAST ")   ' ��Ж�   
            'SQL.Append(",TORIMATOME.SIT_NNAME_N   SIT_NNAME_TORIMATOME ")   ' ���܂ƂߓX     
            'SQL.Append(",MEIMAST.JYUYOUKA_NO_K   JYUYOUKA_NO_MEIMAST ")     ' �_��Ҕԍ�    
            'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K   KEIYAKU_KNAME_MEIMAST ") ' �_��Җ�     
            'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K   YOKINSYA_NAME ")         ' �a���Җ�    
            'SQL.Append(",KEIYAKU.SIT_KNAME_N   SIT_KNAME_KEIYAKU ")         ' �ύX�O�X��    
            'SQL.Append(",MEIMAST.KEIYAKU_SIT_K   KEIYAKU_SIT_MEIMAST ")     ' �ύX�O�X��     
            'SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K   KEIYAKU_KAMOKU_MEIMAST ") ' �ύX�O���     
            'SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K   KEIYAKU_KOUZA_MEIMAST ") ' �ύX�O�����ԍ�    
            'SQL.Append(",TEISEI.SIT_KNAME_N   SIT_KNAME_TEISEI ")           ' �ύX��X��   
            'SQL.Append(",MEIMAST.TEISEI_SIT_K   TEISEI_SIT_MEIMAST ")       ' �ύX��X��    
            'SQL.Append(",MEIMAST.TEISEI_KAMOKU_K   TEISEI_KAMOKU_MEIMAST ") ' �ύX����     
            'SQL.Append(",MEIMAST.TEISEI_KOUZA_K    TEISEI_KOUZA_MEIMAST")   ' �ύX������ԍ�   
            'SQL.Append(",MEIMAST.TORIS_CODE_K   TORIS_CODE_MEIMAST ")       ' ������R�[�h   
            'SQL.Append(",MEIMAST.TORIF_CODE_K   TORIF_CODE_MEIMAST ")       ' ����敛�R�[�h
            'SQL.Append(",TORIMAST.FMT_KBN_T   FMT_KBN_TORIMAST ")           '�t�H�[�}�b�g�敪 
            'SQL.Append(",MEIMAST.FURI_DATA_K   IRAI_DATA_MEIMAST ")         '�˗��f�[�^
            'SQL.Append(" FROM ")
            'SQL.Append(" MEIMAST ")
            'SQL.Append(",TORIMAST ")
            'SQL.Append(",TENMAST TORIMATOME ")
            'SQL.Append(",TENMAST KEIYAKU ")
            'SQL.Append(",TENMAST TEISEI ")
            'SQL.Append(" WHERE ")
            'SQL.Append(" MEIMAST.TORIS_CODE_K = TORIMAST.TORIS_CODE_T ")
            'SQL.Append(" AND MEIMAST.TORIF_CODE_K = TORIMAST.TORIF_CODE_T ")
            'SQL.Append(" AND MEIMAST.FURI_DATE_K = '" & FURI_DATE & "' ")
            ''���ł̏ꍇ���l��
            'SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
            'SQL.Append(" AND (MEIMAST.TEISEI_SIT_K <> '000' ")
            'SQL.Append("      OR MEIMAST.TEISEI_KAMOKU_K <> '00' ")
            'SQL.Append("      OR MEIMAST.TEISEI_KOUZA_K <> '0000000') ")
            'SQL.Append(" AND '" & Jikinko & "' = TORIMATOME.KIN_NO_N ")
            'SQL.Append(" AND TORIMAST.TORIMATOME_SIT_T = TORIMATOME.SIT_NO_N ")
            'SQL.Append(" AND '" & Jikinko & "' = KEIYAKU.KIN_NO_N ")
            'SQL.Append(" AND MEIMAST.KEIYAKU_SIT_K = KEIYAKU.SIT_NO_N ")
            'SQL.Append(" AND '" & Jikinko & "' = TEISEI.KIN_NO_N ")
            'SQL.Append(" AND MEIMAST.TEISEI_SIT_K = TEISEI.SIT_NO_N(+) ")
            'SQL.Append(" ORDER BY TORIMAST.TORIMATOME_SIT_T")
            'SQL.Append(",MEIMAST.TORIS_CODE_K")
            'SQL.Append(",MEIMAST.TORIF_CODE_K")
            'SQL.Append(",MEIMAST.RECORD_NO_K")

        Catch ex As Exception
            BatchLog.Write("", "�G���[", ex.Message)
        End Try

        If OraReader.DataReader(SQL) = True Then
            Try
                Do
                    '������
                    OutputCsvData(mMatchingDate)
                    '�����ɃR�[�h
                    OutputCsvData(Jikinko)
                    '�����ɖ�
                    OutputCsvData(JikinkoNm)
                    '������
                    OutputCsvData(Busyo)
                    '��ЃR�[�h	
                    OutputCsvData(OraReader.GetString("ITAKU_CODE_TORIMAST"))
                    '��Ж�	
                    OutputCsvData(OraReader.GetString("ITAKU_NNAME_TORIMAST"))
                    '���܂ƂߓX	
                    '2010/10/04.Sakon�@�x�X���͕ʓr�擾 +++++++++++++++++++++++++++++++++++++++++++++++++++
                    OutputCsvData(GetTenName(OraReader.GetString("TSIT_NO"), OraReader2))
                    'OutputCsvData(OraReader.GetString("SIT_NNAME_TORIMATOME"))
                    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '�敪
                    OutputCsvData("�w���R�E")
                    '�_��Ҕԍ�	
                    OutputCsvData(OraReader.GetString("JYUYOUKA_NO_MEIMAST"))
                    Dim IRAI_DATA As String = OraReader.GetString("IRAI_DATA_MEIMAST", False)

                    Select Case OraReader.GetString("FMT_KBN_TORIMAST")
                        Case "01", "06" '�n����
                            '�擾�����˗��f�[�^����A�`���Җ����擾
                            Dim KEIYAKU_KNAME As String = IRAI_DATA.Substring(198, 30)

                            '�_��Җ�1	
                            OutputCsvData(KEIYAKU_KNAME.Substring(0, 15))
                            '�_��Җ�2	
                            OutputCsvData(KEIYAKU_KNAME.Substring(15))
                        Case Else
                            'NHK��p�_��Җ���
                            If OraReader.GetString("ITAKU_CODE_TORIMAST").Equals(NHKCODE) Then
                                Dim KEIYAKU_KNAME As String = IRAI_DATA.Substring(65, 15).Trim
                                '�_��Җ�1	
                                OutputCsvData(KEIYAKU_KNAME)
                                '�_��Җ�2	
                                OutputCsvData("")
                            Else
                                '�_��Җ�1	
                                OutputCsvData("")
                                '�_��Җ�2	
                                OutputCsvData("")
                            End If
                    End Select

                    ' NHK��p�a���Җ���
                    If OraReader.GetString("ITAKU_CODE_TORIMAST").Equals(NHKCODE) Then
                        Dim YOKINSYA_NAME As String = IRAI_DATA.Substring(50, 15).Trim
                        '�a���Җ�1	
                        OutputCsvData(YOKINSYA_NAME)
                        '�a���Җ�2	
                        OutputCsvData("")
                    Else
                        '�a���Җ�1	
                        OutputCsvData(OraReader.GetString("YOKINSYA_NAME"))
                        '�a���Җ�2	
                        OutputCsvData("")
                    End If

                    '�ύX�O�X��	
                    OutputCsvData(OraReader.GetString("KEIYAKU_SIT_MEIMAST"))
                    '�ύX�O�X��
                    '2010/10/04.Sakon�@�x�X���͕ʓr�擾 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    OutputCsvData(GetTenName(OraReader.GetString("KEIYAKU_SIT_MEIMAST"), OraReader2))
                    'OutputCsvData(OraReader.GetString("SIT_KNAME_KEIYAKU"))
                    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '�ύX�O���	
                    OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("KEIYAKU_KAMOKU_MEIMAST")))
                    '�ύX�O�����ԍ�	
                    OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_MEIMAST"))

                    If OraReader.GetString("TEISEI_SIT_MEIMAST").Equals("000") Then
                        '�ύX�O�X��
                        OutputCsvData(OraReader.GetString("KEIYAKU_SIT_MEIMAST"))
                        '�ύX�O�X��
                        '2010/10/04.Sakon�@�x�X���͕ʓr�擾 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        OutputCsvData(GetTenName(OraReader.GetString("KEIYAKU_SIT_MEIMAST"), OraReader2))
                        'OutputCsvData(OraReader.GetString("SIT_KNAME_KEIYAKU"))
                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    Else
                        '�ύX��X��
                        OutputCsvData(OraReader.GetString("TEISEI_SIT_MEIMAST"))
                        '�ύX��X��
                        '2010/10/04.Sakon�@�x�X���͕ʓr�擾 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        OutputCsvData(GetTenName(OraReader.GetString("TEISEI_SIT_MEIMAST"), OraReader2))
                        'OutputCsvData(OraReader.GetString("SIT_KNAME_TEISEI"))
                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    End If

                    '2011/06/16 �W���ŏC�� �X�܂̂ݕύX�̏ꍇ���l�� ------------------START
                    '�ύX����
                    '�ϊ���ȖځE�E�E00�̏ꍇ��""�����󎚂��Ȃ��B  
                    If OraReader.GetString("TEISEI_KAMOKU_MEIMAST").Equals("00") Then
                        OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("KEIYAKU_KAMOKU_MEIMAST")))
                    Else
                        OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("TEISEI_KAMOKU_MEIMAST")))
                    End If

                    '�ύX������ԍ�	
                    If OraReader.GetString("TEISEI_KOUZA_MEIMAST").Equals("0000000") Then
                        OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_MEIMAST"))
                    Else
                        OutputCsvData(OraReader.GetString("TEISEI_KOUZA_MEIMAST"))
                    End If

                    ''�ύX����
                    ''�ϊ���ȖځE�E�E00�̏ꍇ��""�����󎚂��Ȃ��B  
                    'If OraReader.GetString("TEISEI_KAMOKU_MEIMAST").Equals("00") Then
                    '    OutputCsvData("")
                    'Else
                    '    OutputCsvData(CASTCommon.ConvertKamoku2TO1(OraReader.GetString("TEISEI_KAMOKU_MEIMAST")))
                    'End If

                    ''�ύX������ԍ�	
                    'OutputCsvData(OraReader.GetString("TEISEI_KOUZA_MEIMAST"))
                    '2011/06/16 �W���ŏC�� �X�܂̂ݕύX�̏ꍇ���l�� ------------------END
                    '�Y���f�[�^�L��	
                    OutputCsvData("")
                    '�����啛	
                    OutputCsvData(OraReader.GetString("TORIS_CODE_MEIMAST") & OraReader.GetString("TORIF_CODE_MEIMAST"), False, True)
                    RecordCnt += 1
                    OraReader.NextRead()
                Loop Until OraReader.EOF
                OraReader.Close()
            Catch ex As Exception
                BatchLog.Write("�f�[�^����", "���s", ex.Message)
            End Try
            Return True
        Else
            '�O�̂��ߎc�� ===============================================
            'If name = "" Then
            '    ' �b�r�u���쐬����
            '    name = List.CreateCsvFile
            'End If
            'OutputCsvData("")    '��ЃR�[�h
            'OutputCsvData("")    '��Ж�
            'OutputCsvData("")    '���܂ƂߓX
            'OutputCsvData("")    '�敪
            'OutputCsvData("")    '�_��Ҕԍ�
            ''�_��Җ��A�a���Җ����i�ɕ����Ĉ�
            'OutputCsvData("")    '�_��Җ�1
            'OutputCsvData("")    '�_��Җ�2
            'OutputCsvData("")    '�a���Җ�1
            'OutputCsvData("")    '�a���Җ�2
            'OutputCsvData("")    '�ύX�O�X��
            'OutputCsvData("")    '�ύX�O�X��
            'OutputCsvData("")    '�ύX�O���
            'OutputCsvData("")    '�ύX�O�����ԍ�
            'OutputCsvData("")    '�ύX��X��
            'OutputCsvData("")    '�ύX��X��
            'OutputCsvData("")    '�ύX����
            'OutputCsvData("")    '�ύX������ԍ�
            'OutputCsvData("�Y���f�[�^�Ȃ�")    '�Y���f�[�^�L��
            'OutputCsvData("", False, True)    '�����啛
            '==============================================================
            RecordCnt = -1
            BatchLog.Write("���", "����", "����Ώ�0��")
            Return False
        End If
    End Function

    '========================================================================================
    ' �@�@�\�F�X�ܖ��擾
    ' ���@���F�x�X�R�[�h
    ' �߂�l�F�X�ܖ�
    ' ���@�l�F
    ' ��@���F2010/10/04
    ' �X�@�V�F
    '========================================================================================
    Private Function GetTenName(ByVal pstrSitNo As String, ByVal OraReader2 As CASTCommon.MyOracleReader) As String
        Dim SQL As New StringBuilder(128)    ' SQL

        SQL = New StringBuilder(128)

        SQL.Append("SELECT")
        SQL.Append(" SIT_NNAME_N")
        SQL.Append(" FROM TENMAST")
        SQL.Append(" WHERE KIN_NO_N = '" & Jikinko & "'")
        SQL.Append(" AND SIT_NO_N = '" & pstrSitNo.Trim & "'")

        If OraReader2.DataReader(SQL) = True Then
            Return OraReader2.GetString("SIT_NNAME_N")
        Else
            Return ""
        End If

    End Function

End Class
