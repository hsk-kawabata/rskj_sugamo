Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon.ModPublic

' �����U�֎������ψꗗ�\
Class ClsPrnJifurisyori
    Inherits CAstReports.ClsReportBase

    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFKP007"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFKP007_�萔����������ƈꗗ�\.rpd"
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
        SQL.Append(",TORIMAST.TORIMATOME_SIT_T")    '���܂ƂߓX
        SQL.Append(",TORIMAST.TUKEKIN_NO_T")        '���ϋ��Z�@��
        SQL.Append(",TORIMAST.TUKESIT_NO_T")        '���ώx�X
        SQL.Append(",TORIMAST.TUKEKAMOKU_T")        '���ωȖ�
        SQL.Append(",TORIMAST.TUKEKOUZA_T")         '���ό���
        SQL.Append(",TORIMAST.TESUUTYO_KBN_T")      '�萔�������敪
        SQL.Append(",SCHMAST.TORIS_CODE_S")         '������R�[�h
        SQL.Append(",SCHMAST.TORIF_CODE_S")         '����敛�R�[�h
        SQL.Append(",SCHMAST.FURI_DATE_S")          '�U�֓�
        SQL.Append(",SCHMAST.KESSAI_DATE_S")        '���ϓ�
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
        SQL.Append(",SCHMAST.KESSAI_YDATE_S")       '���ϗ\���
        SQL.Append(",TENMAST.SIT_NNAME_N")          '�x�X��
        SQL.Append(" FROM SCHMAST")
        SQL.Append(",TORIMAST")
        SQL.Append(",TENMAST")
        SQL.Append(" WHERE SCHMAST.FSYORI_KBN_S = TORIMAST.FSYORI_KBN_T")
        SQL.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
        SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
        SQL.Append(" AND SCHMAST.TESUU_YDATE_S <= " & SQ(txtTESUU_YDATE))
        SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND SCHMAST.TESUUTYO_FLG_S IN('0','2')")   '�������E�����҂�
        SQL.Append(" AND SCHMAST.TESUUKEI_FLG_S = '1'")
        SQL.Append(" AND SCHMAST.TESUU_KIN_S > 0")
        SQL.Append(" AND TENMAST.KIN_NO_N = " & SQ(Jikinko))
        SQL.Append(" AND TENMAST.SIT_NO_N = TORIMAST.TORIMATOME_SIT_T")
        ''���ϑΏۊO�ȊO(���͏�������͂���)
        'SQL.Append(" AND KESSAI_KBN_T <> '99'")
        SQL.Append(" ORDER BY TORIMATOME_SIT_T,FURI_DATE_S,TORIS_CODE_T,TORIF_CODE_T")

        Dim bSQL As Boolean
        Dim substrKESSAI_DATE As String = ""
        Dim substrTESUU_DATE As String = ""
        Try
            bSQL = OraReader.DataReader(SQL)
            If bSQL = True Then
                Do
                    OutputCsvData(mMatchingDate, True)                                  '������
                    OutputCsvData(mMatchingTime, True)                                  '�^�C���X�^���v
                    OutputCsvData(txtTESUU_YDATE, True)                                '���(�萔�������\���)
                    OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"), True)        '���܂ƂߓX�R�[�h
                    OutputCsvData(OraReader.GetString("SIT_NNAME_N"), True)             '���܂ƂߓX��
                    OutputCsvData(OraReader.GetString("TORIS_CODE_S"), True)            '������R�[�h
                    OutputCsvData(OraReader.GetString("TORIF_CODE_S"), True)            '����敛�R�[�h
                    OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"), True)           '����於
                    OutputCsvData(OraReader.GetString("FURI_CODE_T"), True)             '�U�փR�[�h
                    OutputCsvData(OraReader.GetString("KIGYO_CODE_T"), True)            '��ƃR�[�h
                    OutputCsvData(OraReader.GetString("FURI_DATE_S"), True)             '�U�֓�

                    substrKESSAI_DATE = OraReader.GetString("KESSAI_YDATE_S")
                    If substrKESSAI_DATE = "00000000" Then
                        substrKESSAI_DATE = ""
                    End If
                    OutputCsvData(substrKESSAI_DATE, True)                              '����(�\��)��

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
                    OutputCsvData(OraReader.GetString("TUKEKIN_NO_T"), True)            '���ϋ��Z�@��
                    OutputCsvData(OraReader.GetString("TUKESIT_NO_T"), True)            '���ώx�X
                    '2017/05/19 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                    '���ωȖږ����e�L�X�g����擾����
                    OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(PATH_TXT, "Common_���ωȖ�.TXT"), _
                                                                OraReader.GetString("TUKEKAMOKU_T")), True)
                    'Select Case OraReader.GetString("TUKEKAMOKU_T")                     '���ωȖ�
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
                    OutputCsvData(OraReader.GetString("TUKEKOUZA_T"), True, True)       '���ό����ԍ�
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
