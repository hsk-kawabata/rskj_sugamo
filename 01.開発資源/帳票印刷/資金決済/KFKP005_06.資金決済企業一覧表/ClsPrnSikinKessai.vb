Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �����U�֎������ψꗗ�\
Class ClsPrnSikinKessai
    Inherits CAstReports.ClsReportBase

    Sub New(ByVal PrintKbn As Integer)
        If PrintKbn = 1 OrElse PrintKbn = 3 Then
            ' CSV�t�@�C���Z�b�g
            InfoReport.ReportName = "KFKP005"

            ' ��`�̖��Z�b�g
            ReportBaseName = "KFKP005_�������ϊ�ƈꗗ�\.rpd"
        Else
            ' CSV�t�@�C���Z�b�g
            InfoReport.ReportName = "KFKP006"

            ' ��`�̖��Z�b�g
            ReportBaseName = "KFKP006_�������ϊ�ƈꗗ�\(���񂫂񒆋���).rpd"
        End If
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile

        Return file
    End Function

    '
    ' �@�\�@ �F �����U�֎������ψꗗ�\���f�[�^�ɏ�������
    '
    ' ���l�@ �F 
    '
    Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
        CSVObject.Output(data, dq, crlf)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    ' �@�\   �F �������ϒ��[�o�͏���
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l   �F 
    '
    Public Function MakeRecord() As Boolean
        Dim SQL As New StringBuilder(128)
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim TYO_KBN As String = ""
        Dim MISHUU_KBN As String = ""
        '2017/05/19 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
        Dim PATH_TXT As String = CASTCommon.GetFSKJIni("COMMON", "TXT")
        '2017/05/19 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

        'INI�t�@�C���擾
        '�����ɃR�[�h
        Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        If Jikinko.ToUpper = "ERR" Or Jikinko = "" Then
            BatchLog.Write("�ݒ�t�@�C���擾", "���s", "���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD")
            Return False
        End If

        SQL = New StringBuilder(128)
        SQL.Append("SELECT")
        SQL.Append(" TORIMAST.FURI_CODE_T")         '�U�փR�[�h
        SQL.Append(",TORIMAST.KIGYO_CODE_T")        '��ƃR�[�h
        SQL.Append(",TORIMAST.ITAKU_NNAME_T")       '�ϑ��Ҋ�����
        SQL.Append(",TORIMAST.HONBU_KOUZA_T")       '�{�������ԍ�
        SQL.Append(",TORIMAST.TESUUTYO_KBN_T")      '�萔�������敪
        SQL.Append(",TORIMAST.TESUUTYO_PATN_T")     '�萔���������@
        SQL.Append(",TORIMAST.TORIMATOME_SIT_T")    '���܂ƂߓX
        SQL.Append(",TORIMAST.TUKEKIN_NO_T")        '���ϋ��Z�@��
        SQL.Append(",TORIMAST.TUKESIT_NO_T")        '���ώx�X
        SQL.Append(",TORIMAST.TUKEKAMOKU_T")        '���ωȖ�
        SQL.Append(",TORIMAST.TUKEKOUZA_T")         '���ό���
        SQL.Append(",TORIMAST.KESSAI_KBN_T")        '���ϋ敪
        SQL.Append(",SCHMAST.TORIS_CODE_S")         '������R�[�h
        SQL.Append(",SCHMAST.TORIF_CODE_S")         '����敛�R�[�h
        SQL.Append(",SCHMAST.FURI_DATE_S")          '�U�֓�
        SQL.Append(",SCHMAST.KESSAI_DATE_S")        '���ϓ�
        SQL.Append(",SCHMAST.TESUU_DATE_S")         '�萔��������
        SQL.Append(",SCHMAST.TESUU_KIN_S")          '�萔�����z
        SQL.Append(",SCHMAST.TESUU_KIN1_S")         '�萔��(���U)
        SQL.Append(",SCHMAST.TESUU_KIN2_S")         '�萔��(���̑�)
        SQL.Append(",SCHMAST.TESUU_KIN3_S")         '�萔��(�U��)
        SQL.Append(",SCHMAST.SYORI_KEN_S")          '��������
        SQL.Append(",SCHMAST.SYORI_KIN_S")          '�������z
        SQL.Append(",SCHMAST.FURI_KEN_S")           '�U�֌���
        SQL.Append(",SCHMAST.FURI_KIN_S")           '�U�֋��z
        SQL.Append(",SCHMAST.FUNOU_KEN_S")          '�s�\����
        SQL.Append(",SCHMAST.FUNOU_KIN_S")          '�s�\���z
        SQL.Append(",SCHMAST.TESUUTYO_FLG_S")       '�萔�������t���O
        SQL.Append(",SCHMAST.TESUU_YDATE_S")        '�萔�������\���
        SQL.Append(",TENMAST.SIT_NNAME_N")          '�x�X��
        '2010/01/20 ���ϋ��Z�@�ցE�x�X���ǉ�
        SQL.Append(",KESSAI_KIN.KIN_KNAME_N KESSAI_KIN_KNAME")          '���ϋ��Z�@�֖�
        SQL.Append(",KESSAI_KIN.SIT_KNAME_N KESSAI_SIT_KNAME")          '���ώx�X��
        '===================================
        '2017/05/16 �^�X�N�j���� ADD �W���ŏC���i�������ϒ��[�ɐU�֎萔���P���ǉ��j----------------- START
        SQL.Append(",SCHMAST.KESSAI_YDATE_S")       '���ϗ\���
        SQL.Append(",TORIMAST.KIHTESUU_T")          '�U�֎萔���P���ǉ�
        '2017/05/16 �^�X�N�j���� ADD �W���ŏC���i�������ϒ��[�ɐU�֎萔���P���ǉ��j----------------- END
        SQL.Append(" FROM SCHMAST")
        SQL.Append(",TORIMAST")
        SQL.Append(",TENMAST")
        '2010/01/20 ���ϋ��Z�@�֒ǉ�
        SQL.Append(",TENMAST KESSAI_KIN")
        '===========================
        SQL.Append(" WHERE SCHMAST.FSYORI_KBN_S = TORIMAST.FSYORI_KBN_T")
        SQL.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
        SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
        '2017/05/16 �^�X�N�j���� CHG �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- START
        If PRINT_MODE = "0" Then
            '�\�蒠�[
            SQL.Append(" AND SCHMAST.KESSAI_YDATE_S = " & SQ(txtKESSAI_DATE))
            SQL.Append(" AND SCHMAST.TESUUKEI_FLG_S = '1'")
        Else
            '���ʒ��[
            SQL.Append(" AND SCHMAST.KESSAI_DATE_S = " & SQ(txtKESSAI_DATE))
            SQL.Append(" AND SCHMAST.KESSAI_FLG_S = '1'")
        End If
        'SQL.Append(" AND SCHMAST.KESSAI_DATE_S = " & SQ(txtKESSAI_DATE))
        'SQL.Append(" AND SCHMAST.KESSAI_FLG_S = '1'")
        '2017/05/16 �^�X�N�j���� CHG �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- END
        SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND SCHMAST.SYORI_KIN_S > 0")
        SQL.Append(" AND TENMAST.KIN_NO_N = " & SQ(Jikinko))
        SQL.Append(" AND TENMAST.SIT_NO_N = TORIMAST.TORIMATOME_SIT_T")
        Select Case txtcommand
            Case "1"    '�ʏ큨���ϑΏۊO�ȊO
                SQL.Append(" AND KESSAI_KBN_T <> '99'")
            Case "2"    '�M��������
                SQL.Append(" AND KESSAI_KBN_T = '00'")
            Case "3"    '���񂫂񒆋��ȊO
                SQL.Append(" AND KESSAI_KBN_T NOT IN('00','99')")
        End Select
        '2010/01/20 ���ϋ��Z�@�֒ǉ�
        SQL.Append(" AND KESSAI_KIN.KIN_NO_N = TUKEKIN_NO_T")
        SQL.Append(" AND KESSAI_KIN.SIT_NO_N = TUKESIT_NO_T")
        '===========================
        SQL.Append(" ORDER BY TORIMATOME_SIT_T,KESSAI_KBN_T, FURI_DATE_S,FURI_CODE_T, KIGYO_CODE_T,TORIS_CODE_T,TORIF_CODE_T")

        Dim bSQL As Boolean
        Dim substrTESUU_DATE As String = ""
        Dim strNYUU_KIN As String               ' ���������z
        Dim lngSagaku As Long                   ' �U�֍ϋ��z(FURI_KIN)�Ǝ萔�����z(TESUU_KIN)�̍��z
        Try
            bSQL = OraReader.DataReader(SQL)
            If bSQL = True Then
                Do
                    OutputCsvData(mMatchingDate, True)                                  '������
                    OutputCsvData(mMatchingTime, True)                                  '�^�C���X�^���v
                    OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"), True)        '���܂ƂߓX�R�[�h
                    OutputCsvData(OraReader.GetString("SIT_NNAME_N"), True)             '���܂ƂߓX��
                    OutputCsvData(OraReader.GetString("TORIS_CODE_S"), True)            '������R�[�h
                    OutputCsvData(OraReader.GetString("TORIF_CODE_S"), True)            '����敛�R�[�h
                    OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"), True)           '����於
                    OutputCsvData(OraReader.GetString("FURI_CODE_T"), True)             '�U�փR�[�h
                    OutputCsvData(OraReader.GetString("KIGYO_CODE_T"), True)            '��ƃR�[�h
                    OutputCsvData(OraReader.GetString("FURI_DATE_S"), True)             '�U�֓�
                    '2017/05/16 �^�X�N�j���� CHG �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- START
                    If PRINT_MODE = "0" Then
                        '�\�蒠�[
                        OutputCsvData(OraReader.GetString("KESSAI_YDATE_S"), True)      '���ϗ\���
                    Else
                        '���ʒ��[
                        OutputCsvData(OraReader.GetString("KESSAI_DATE_S"), True)       '���ϓ�
                    End If
                    'OutputCsvData(OraReader.GetString("KESSAI_DATE_S"), True)           '���ϓ�
                    '2017/05/16 �^�X�N�j���� CHG �W���ŏC���i���ϗ\����ł̏o�͑Ή��j----------------- END
                    OutputCsvData(OraReader.GetString("HONBU_KOUZA_T"), True)           '�{���ʒi�����ԍ�
                    OutputCsvData(OraReader.GetInt64("SYORI_KEN_S"), True)              '��������
                    OutputCsvData(OraReader.GetInt64("SYORI_KIN_S"), True)              '�������z
                    OutputCsvData(OraReader.GetInt64("FUNOU_KEN_S"), True)              '�s�\����
                    OutputCsvData(OraReader.GetInt64("FUNOU_KIN_S"), True)              '�s�\���z
                    OutputCsvData(OraReader.GetInt64("FURI_KEN_S"), True)               '�U�֌���
                    OutputCsvData(OraReader.GetInt64("FURI_KIN_S"), True)               '�U�֋��z
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN_S"), True)              '�萔��
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN1_S"), True)             '�萔������-���U
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN3_S"), True)             '�萔������-�U��
                    OutputCsvData(OraReader.GetInt64("TESUU_KIN2_S"), True)             '�萔������-���̑�
                    OutputCsvData(OraReader.GetInt64("FURI_KEN_S"), True)               '��������
                    '�������z
                    lngSagaku = CLng(OraReader.GetInt64("FURI_KIN_S")) - CLng(OraReader.GetInt64("TESUU_KIN_S"))    ' �U�֍ϋ��z(FURI_KIN)�Ǝ萔�����z(TESUU_KIN)�̍��z���Z�o
                    If lngSagaku > 0 AndAlso OraReader.GetString("TESUUTYO_KBN_T") = "0" AndAlso OraReader.GetString("TESUUTYO_PATN_T") = "0" Then    ' �U�֍ϋ��z - �萔�����z > 0 and �萔�������敪 = 0�F�s�x���� and �萔���������@=0�F��������
                        strNYUU_KIN = CStr(lngSagaku)
                    Else
                        strNYUU_KIN = OraReader.GetInt64("FURI_KIN_S")
                    End If
                    OutputCsvData(strNYUU_KIN, True)
                    Select Case OraReader.GetString("KESSAI_KBN_T")      '���ϋ敪
                        Case "00"
                            OutputCsvData("�a����", True)
                        Case "01"
                            OutputCsvData("��������", True)
                        Case "02"
                            OutputCsvData("�ב֐U��", True)
                        Case "03"
                            OutputCsvData("�ב֕t��", True)
                        Case "04"
                            OutputCsvData("�ʒi�o���̂�", True)
                        Case "05"
                            OutputCsvData("���ʊ��", True)
                        Case "99"
                            OutputCsvData("���ϑΏۊO", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select
                    OutputCsvData(OraReader.GetString("TUKEKIN_NO_T"), True)    '���ϋ��Z�@��
                    OutputCsvData(OraReader.GetString("TUKESIT_NO_T"), True)    '���ώx�X
                    '2010/01/20 ���ϋ��Z�@�֒ǉ�
                    OutputCsvData(OraReader.GetString("KESSAI_KIN_KNAME"), True)    '���ϋ��Z�@�֖�
                    OutputCsvData(OraReader.GetString("KESSAI_SIT_KNAME"), True)    '���ώx�X��
                    '===========================
                    '2017/05/19 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                    '���ωȖږ����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_���ωȖ�.TXT"), _
                                                                OraReader.GetString("TUKEKAMOKU_T")), True)
                    'Select Case OraReader.GetString("TUKEKAMOKU_T") '���ωȖ�
                    '    Case "01"
                    '        OutputCsvData("����", True)
                    '    Case "02"
                    '        OutputCsvData("����", True)
                    '    Case "04"
                    '        OutputCsvData("�ʒi", True)
                    '    Case "99"
                    '        OutputCsvData("������", True)
                    '    Case Else
                    '        OutputCsvData("", True)
                    'End Select
                    '2017/05/19 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END
                    OutputCsvData(OraReader.GetString("TUKEKOUZA_T"), True)  '���ό����ԍ�
                    Select Case OraReader.GetString("TESUUTYO_KBN_T")   '�萔�������敪
                        Case "0"
                            OutputCsvData("�s�x����", True)
                        Case "1"
                            OutputCsvData("�ꊇ����", True)
                        Case "2"
                            OutputCsvData("���ʖƏ�", True)
                        Case "3"
                            OutputCsvData("�ʓr����", True)
                        Case Else
                            OutputCsvData("", True)
                    End Select

                    Select Case OraReader.GetString("TESUUTYO_PATN_T") '�萔���������@
                        Case "0"
                            OutputCsvData("��������", True)
                        Case "1"
                            OutputCsvData("���ړ���", True)
                    End Select

                    substrTESUU_DATE = OraReader.GetString("TESUU_DATE_S")
                    If substrTESUU_DATE = "00000000" Then
                        substrTESUU_DATE = ""
                    End If
                    OutputCsvData(substrTESUU_DATE, True)                       '�萔��������

                    '2017/07/25 ���� CHG �W���ŏC���i�U�֊z�Ǝ萔�������z�̏ꍇ�̍l���j----------------- START
                    'If OraReader.GetInt64("FURI_KIN_S") < OraReader.GetInt64("TESUU_KIN_S") Then '�����敪
                    If OraReader.GetInt64("FURI_KIN_S") <= OraReader.GetInt64("TESUU_KIN_S") Then '�����敪
                        MISHUU_KBN = "5"
                        'OutputCsvData("�����z����", True)
                        OutputCsvData("�萔��������", True)
                    '2017/07/25 ���� CHG �W���ŏC���i�U�֊z�Ǝ萔�������z�̏ꍇ�̍l���j----------------- END
                    Else
                        If OraReader.GetString("TESUUTYO_FLG_S") = "1" Then     '������
                            MISHUU_KBN = "3"
                            OutputCsvData("", True)
                        ElseIf OraReader.GetString("TESUUTYO_KBN_T") = "2" Then '������ ���ʖƏ�������ς�
                            MISHUU_KBN = "3"
                            OutputCsvData("�Ə�", True)
                        ElseIf OraReader.GetString("TESUUTYO_KBN_T") = "0" And OraReader.GetString("TESUU_YDATE_S") > mMatchingDate Then
                            MISHUU_KBN = "4"
                            OutputCsvData("����������", True)
                        ElseIf OraReader.GetString("TESUU_YDATE_S") <= mMatchingDate And OraReader.GetString("TESUUTYO_FLG_S") = "0" Then
                            MISHUU_KBN = "1"
                            OutputCsvData("����(��������)", True)
                        ElseIf OraReader.GetString("TESUUTYO_KBN_T") = "1" OrElse OraReader.GetString("TESUUTYO_KBN_T") = "3" Then
                            MISHUU_KBN = "1"
                            OutputCsvData("����(���)", True)
                        Else
                            MISHUU_KBN = ""
                            OutputCsvData("", True)
                        End If
                    End If

                    '2017/05/16 �^�X�N�j���� ADD �W���ŏC���i�������ϒ��[�ɐU�֎萔���P���ǉ��j----------------- START
                    OutputCsvData(OraReader.GetInt64("KIHTESUU_T"))                 '�U�֎萔���P���ǉ�
                    If PRINT_MODE = "0" Then
                        OutputCsvData("0")  '�\�蒠�[
                    Else
                        OutputCsvData("1")  '���ʒ��[
                    End If
                    '2017/05/16 �^�X�N�j���� ADD �W���ŏC���i�������ϒ��[�ɐU�֎萔���P���ǉ��j----------------- END

                    If MISHUU_KBN = "3" Then                                        '�������萔��
                        OutputCsvData(0, True, True)
                    Else
                        OutputCsvData(OraReader.GetInt64("TESUU_KIN_S"), True, True)
                    End If
                    RecordCnt += 1
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
