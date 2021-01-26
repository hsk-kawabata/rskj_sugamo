Option Strict On
Option Explicit On

Imports System
Imports System.Text
Imports CASTCommon.ModPublic
Imports System.IO
Imports System.Collections

Class KFKP002
    Inherits CAstReports.ClsReportBase

    Public CsvData(38) As String            '2010/02/02 36��38

    Private BasePath As String              ' INI�t�@�C���̎������σt�@�C���p�X
    Private KessaiFileName As String        ' �������σt�@�C����

    Private strKESSAI_DATE As String                    ' ���ϓ�
    Private Mainlog As CASTCommon.BatchLOG
    Sub New()
        ' CSV�t�@�C���Z�b�g
        InfoReport.ReportName = "KFKP002"

        ' ��`�̖��Z�b�g
        ReportBaseName = "KFKP002_�������ʊm�F�\(�������ώ��).rpd"

        CsvData(0) = "00010101"                                     ' ������
        CsvData(1) = "00010101"                                     ' ���ϓ�
        CsvData(2) = "00010101"                                     ' �^�C���X�^���v
        CsvData(3) = ""                                             ' ������R�[�h
        CsvData(4) = ""                                             ' ����敛�R�[�h
        CsvData(5) = ""                                             ' ����於
        CsvData(6) = ""                                             ' �U�փR�[�h
        CsvData(7) = ""                                             ' ��ƃR�[�h
        CsvData(8) = "00010101"                                     ' �U�֓�
        CsvData(9) = ""                                             ' ���ϋ敪�i�����j
        CsvData(10) = ""                                            ' ���ϋ��Z�@�փR�[�h
        CsvData(11) = ""                                            ' ���ϋ��Z�@�֎x�X�R�[�h
        CsvData(12) = ""                                            ' ���ωȖ�
        CsvData(13) = ""                                            ' ���ό����ԍ�
        CsvData(14) = ""                                            ' �萔�������敪
        CsvData(15) = ""                                            ' �萔���������@
        CsvData(16) = ""                                            ' ���܂ƂߓX�R�[�h
        CsvData(17) = ""                                            ' ���܂ƂߓX��
        CsvData(18) = "0"                                            ' ��������
        CsvData(19) = "0"                                            ' �������z
        CsvData(20) = "0"                                            ' �s�\����
        CsvData(21) = "0"                                            ' �s�\���z
        CsvData(22) = "0"                                            ' �U�֌���
        CsvData(23) = "0"                                           ' �U�֋��z
        CsvData(24) = "0"                                           ' �萔��
        CsvData(25) = "0"                                           ' �萔������|���U
        CsvData(26) = "0"                                           ' �萔������|�U��
        CsvData(27) = "0"                                           ' �萔������|���̑�
        CsvData(28) = "0"                                           ' ����������
        CsvData(29) = "0"                                           ' ���������z
        CsvData(30) = ""                                            ' �ȖڃR�[�h
        CsvData(31) = ""                                            ' �I�y�R�[�h
        CsvData(32) = ""                                            ' �I�y���[�V������
        CsvData(33) = "0"                                           ' ���o���z�i�I�y�R�[�h�P�ʁj
        CsvData(34) = "0"                                           ' �萔���z�i�I�y�R�[�h�P�ʁj
        CsvData(35) = "0"                                           ' �W�v�t���O
        CsvData(36) = "0"                                           ' ���G���^�쐬�敪
        '2010/02/02 �ǉ�
        CsvData(37) = ""                                            ' ���ϋ��Z�@�֖�
        CsvData(38) = ""                                            ' ���ώx�X��
        '===============
    End Sub

    Public Overrides Function CreateCsvFile() As String
        Dim file As String = MyBase.CreateCsvFile()

        ' �^�C�g���s
        CSVObject.Output("������")
        CSVObject.Output("���ϓ�")
        CSVObject.Output("�^�C���X�^���v")
        CSVObject.Output("������R�[�h")
        CSVObject.Output("����敛�R�[�h")
        CSVObject.Output("����於")
        CSVObject.Output("�U�փR�[�h")
        CSVObject.Output("��ƃR�[�h")
        CSVObject.Output("�U�֓�")
        CSVObject.Output("���ϋ敪")
        CSVObject.Output("���ϋ��Z�@�փR�[�h")
        CSVObject.Output("���ώx�X�R�[�h")
        CSVObject.Output("���ωȖ�")
        CSVObject.Output("���ό����ԍ�")
        CSVObject.Output("�萔�������敪")
        CSVObject.Output("�萔���������@")
        CSVObject.Output("�Ƃ�܂ƂߓX�R�[�h")
        CSVObject.Output("�Ƃ�܂ƂߓX��")
        CSVObject.Output("��������")
        CSVObject.Output("�������z")
        CSVObject.Output("�s�\����")
        CSVObject.Output("�s�\���z")
        CSVObject.Output("�U�֌���")
        CSVObject.Output("�U�֋��z")
        CSVObject.Output("�萔��")
        CSVObject.Output("���U�萔��")
        CSVObject.Output("�U���萔��")
        CSVObject.Output("���̑��萔��")
        CSVObject.Output("��������")
        CSVObject.Output("�������z")
        CSVObject.Output("�ȖڃR�[�h")
        CSVObject.Output("�I�y�R�[�h")
        CSVObject.Output("�I�y���[�V������")
        CSVObject.Output("���o���z")
        CSVObject.Output("�萔���z")
        CSVObject.Output("�W�v�t���O")
        '2010/02/02 ���ڒǉ�
        'CSVObject.Output("�쐬�敪", False, True)
        CSVObject.Output("�쐬�敪")
        CSVObject.Output("���ϋ��Z�@�֖�")
        CSVObject.Output("���ώx�X��", False, True)
        '====================

        Return file
    End Function

    '
    ' �@�\�@ �F �������Ϗ������ʊm�F�\(�������ώ��)�b�r�u���o�͂���
    '
    ' ���l�@ �F 
    '
    Public Function OutputCSVKekka(ByVal TimeStamp As String, ByVal Jikinko As String, ByVal db As CASTCommon.MyOracle, ByVal WriteLog As CASTCommon.BatchLOG) As Integer

        Mainlog = WriteLog
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing
        Dim newkey As String = ""
        Dim oldkey As String = ""
        Dim SQL As New StringBuilder(128)
        Dim strDate As String = Now.ToString("yyyyMMdd")
        Dim strTime As String = Now.ToString("HHmmss")
        Dim OraKesReader As CASTCommon.MyOracleReader = Nothing
        Dim CommonSQL As New StringBuilder(128)

        Try
            OraKesReader = New CASTCommon.MyOracleReader(db)
            SQL.Append("(")
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(",KAMOKU_CODE_KR")
            SQL.Append(",OPE_CODE_KR")
            SQL.Append(",DENBUN_ALL_KR")
            SQL.Append(",RECORD_NO_KR")
            SQL.Append(",KESSAI_YDATE_S")
            SQL.Append(",TESUU_YDATE_S")
            SQL.Append(",TESUU_KIN_S")
            SQL.Append(",TESUU_KIN1_S")
            SQL.Append(",TESUU_KIN2_S")
            SQL.Append(",TESUU_KIN3_S")
            SQL.Append(",FURI_KEN_S")
            SQL.Append(",FURI_KIN_S")
            SQL.Append(",SYORI_KEN_S")
            SQL.Append(",SYORI_KIN_S")
            SQL.Append(",FUNOU_KEN_S")
            SQL.Append(",FUNOU_KIN_S")
            SQL.Append(",KIGYO_CODE_T")
            SQL.Append(",BAITAI_CODE_T")
            SQL.Append(",SYUBETU_T")
            SQL.Append(",ITAKU_CODE_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",ITAKU_KNAME_T")
            SQL.Append(",FURI_CODE_T")
            SQL.Append(",NS_KBN_T")
            SQL.Append(",TESUUTYO_PATN_T")
            SQL.Append(",TESUUTYO_KBN_T")
            SQL.Append(",KESSAI_KBN_T")
            SQL.Append(",TORIMATOME_SIT_T")
            SQL.Append(",HONBU_KOUZA_T")
            SQL.Append(",TUKEKIN_NO_T")
            SQL.Append(",TUKESIT_NO_T")
            SQL.Append(",TUKEKAMOKU_T")
            SQL.Append(",TUKEKOUZA_T")
            SQL.Append(",TUKEMEIGI_KNAME_T")
            SQL.Append(",BIKOU1_T")
            SQL.Append(",BIKOU2_T")
            SQL.Append(",TESUUTYO_SIT_T")
            SQL.Append(",TESUUTYO_KAMOKU_T")
            SQL.Append(",TESUUTYO_KOUZA_T")
            SQL.Append(",TESUUTYO_FLG_S")
            SQL.Append(",TESUU_TIME_STAMP_S")
            SQL.Append(",KESSAI_TIME_STAMP_S")
            SQL.Append(",0 AS JYUNBAN")
            SQL.Append(" FROM KESSAIMAST,TORIMAST,SCHMAST")
            SQL.Append(" WHERE TIME_STAMP_KR = " & SQ(TimeStamp))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_KR")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_KR")
            SQL.Append(" AND FURI_DATE_S  = FURI_DATE_KR")
            SQL.Append(" AND (TESUU_TIME_STAMP_S  =" & SQ(TimeStamp))
            SQL.Append(" OR  KESSAI_TIME_STAMP_S  =" & SQ(TimeStamp) & ")")
            SQL.Append(")")

            SQL.Append("UNION")
            SQL.Append("(")
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",MAX(FURI_DATE_S) AS FURI_DATE_S")
            SQL.Append(",KAMOKU_CODE_KR")
            SQL.Append(",OPE_CODE_KR")
            SQL.Append(",MAX(DENBUN_ALL_KR) AS DENBUN_ALL_KR")
            SQL.Append(",MAX(RECORD_NO_KR) AS RECORD_NO_KR")
            SQL.Append(",'00000000' AS KESSAI_YDATE_S")
            SQL.Append(",MAX(TESUU_YDATE_S) AS TESUU_YDATE_S")
            SQL.Append(",SUM(TESUU_KIN_S) AS TESUU_KIN_S")
            SQL.Append(",SUM(TESUU_KIN1_S) AS TESUU_KIN1_S")
            SQL.Append(",SUM(TESUU_KIN2_S) AS TESUU_KIN2_S")
            SQL.Append(",SUM(TESUU_KIN3_S) AS TESUU_KIN3_S")
            SQL.Append(",SUM(FURI_KEN_S) AS FURI_KEN_S")
            SQL.Append(",SUM(FURI_KIN_S) AS FURI_KIN_SV1")
            SQL.Append(",SUM(SYORI_KEN_S) AS SYORI_KEN_S")
            SQL.Append(",SUM(SYORI_KIN_S) AS SYORI_KIN_S")
            SQL.Append(",SUM(FUNOU_KEN_S) AS FUNOU_KEN_S")
            SQL.Append(",SUM(FUNOU_KIN_S) AS FUNOU_KIN_S")
            SQL.Append(",MAX(KIGYO_CODE_T) AS KIGYO_CODE_T")
            SQL.Append(",MAX(BAITAI_CODE_T) AS BAITAI_CODE_T")
            SQL.Append(",MAX(SYUBETU_T) AS SYUBETU_T")
            SQL.Append(",MAX(ITAKU_CODE_T) AS ITAKU_CODE_T")
            SQL.Append(",MAX(ITAKU_NNAME_T) AS ITAKU_NNAME_T")
            SQL.Append(",MAX(ITAKU_KNAME_T) AS ITAKU_KNAME_T")
            SQL.Append(",MAX(FURI_CODE_T) AS FURI_CODE_T")
            SQL.Append(",MAX(NS_KBN_T) AS NS_KBN_T")
            SQL.Append(",MAX(TESUUTYO_PATN_T) AS TESUUTYO_PATN_T")
            SQL.Append(",MAX(TESUUTYO_KBN_T) AS TESUUTYO_KBN_T")
            SQL.Append(",MAX(KESSAI_KBN_T) AS KESSAI_KBN_T")
            SQL.Append(",MAX(TORIMATOME_SIT_T) AS TORIMATOME_SIT_T")
            SQL.Append(",MAX(HONBU_KOUZA_T) AS HONBU_KOUZA_T")
            SQL.Append(",MAX(TUKEKIN_NO_T) AS TUKEKIN_NO_T")
            SQL.Append(",MAX(TUKESIT_NO_T) AS TUKESIT_NO_T")
            SQL.Append(",MAX(TUKEKAMOKU_T) AS TUKEKAMOKU_T")
            SQL.Append(",MAX(TUKEKOUZA_T) AS TUKEKOUZA_T")
            SQL.Append(",MAX(TUKEMEIGI_KNAME_T) AS TUKEMEIGI_KNAME_T")
            SQL.Append(",MAX(BIKOU1_T) AS BIKOU1_T")
            SQL.Append(",MAX(BIKOU2_T) AS BIKOU2_T")
            SQL.Append(",MAX(TESUUTYO_SIT_T) AS TESUUTYO_SIT_T")
            SQL.Append(",MAX(TESUUTYO_KAMOKU_T) AS TESUUTYO_KAMOKU_T")
            SQL.Append(",MAX(TESUUTYO_KOUZA_T) AS TESUUTYO_KOUZA_T")
            SQL.Append(",MAX(TESUUTYO_FLG_S) AS TESUUTYO_FLG_S")
            SQL.Append(",MAX(TESUU_TIME_STAMP_S) AS TESUU_TIME_STAMP_S")
            SQL.Append(",MAX(KESSAI_TIME_STAMP_S) AS KESSAI_TIME_STAMP_S")
            SQL.Append(",1 AS JYUNBAN")
            SQL.Append(" FROM KESSAIMAST,TORIMAST,SCHMAST")
            SQL.Append(" WHERE TIME_STAMP_KR = " & SQ(TimeStamp))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_KR")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_KR")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_KR")
            SQL.Append(" AND TRIM(FURI_DATE_KR) IS NULL ")
            SQL.Append(" AND (TESUU_TIME_STAMP_S  =" & SQ(TimeStamp))
            SQL.Append(" OR  KESSAI_TIME_STAMP_S  =" & SQ(TimeStamp) & ")")
            SQL.Append(" GROUP BY TORIS_CODE_T, TORIF_CODE_T,KAMOKU_CODE_KR,OPE_CODE_KR,FURI_DATE_KR")
            SQL.Append(")")
            SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T,JYUNBAN,RECORD_NO_KR,KAMOKU_CODE_KR,OPE_CODE_KR,FURI_DATE_S")

            If OraKesReader.DataReader(SQL) = True Then
                While OraKesReader.EOF = False
                    KData.Init()
                    fn_KessaiData(KData, OraKesReader, TimeStamp)

                    newkey = KData.TorisCode & KData.TorifCode & KData.FuriDate
                    ' �b�r�u�f�[�^�ݒ�
                    CsvData(0) = strDate                                    ' ������
                    CsvData(1) = Mid(TimeStamp, 1, 8)                       ' ���
                    CsvData(2) = strTime                                    ' �^�C���X�^���v
                    CsvData(3) = KData.TorisCode                             ' ������R�[�h
                    CsvData(4) = KData.TorifCode                             ' ����敛�R�[�h
                    CsvData(5) = KData.ToriNName.Trim                        ' ����於
                    CsvData(6) = KData.FuriCode                              ' �U�փR�[�h
                    CsvData(7) = KData.KigyoCode                             ' ��ƃR�[�h
                    CsvData(8) = KData.FuriDate                              ' �U�֓�

                    Select Case KData.KessaiKbn
                        Case "00"
                            CsvData(9) = "�a����"
                        Case "01"
                            CsvData(9) = "��������"                                            ' ���ϋ敪�i�����j
                        Case "02"
                            CsvData(9) = "�ב֐U��"
                        Case "03"
                            CsvData(9) = "�ב֕t��"
                        Case "04"
                            CsvData(9) = "�ʒi�o���̂�"
                        Case "05"
                            CsvData(9) = "���ʊ��"
                        Case "99"
                            CsvData(9) = "���ϑΏۊO"
                    End Select

                    CsvData(10) = KData.KesKinCode                                            ' ���ϋ��Z�@�փR�[�h
                    CsvData(11) = KData.KesSitCode                                            ' ���ϋ��Z�@�֎x�X�R�[�h
                    CsvData(12) = KData.KesKamoku                                            ' ���ωȖ�
                    CsvData(13) = KData.KesKouza                                ' ���ό����ԍ�

                    Select Case KData.TesTyoKbn
                        Case "0"    '�s�x�����̏ꍇ
                            CsvData(14) = "�s�x����"                               ' �萔�������敪
                        Case "1"    '�ꊇ�����̏ꍇ
                            CsvData(14) = "�ꊇ����"
                        Case "2"    '���ʖƏ��̏ꍇ
                            CsvData(14) = "���ʖƏ�"

                        Case "3"    '�ʓr�����̏ꍇ
                            CsvData(14) = "�ʓr����"
                        Case Else
                            CsvData(14) = "�萔��������"
                    End Select

                    Select Case KData.TesTyohh
                        Case "0"
                            CsvData(15) = "����"                               ' �萔���������@
                        Case "1"
                            CsvData(15) = "����"                               ' �萔���������@
                        Case Else
                            CsvData(15) = ""                                    ' �萔���������@
                    End Select

                    CsvData(16) = KData.TorimatomeSit                           ' ���܂ƂߓX�R�[�h
                    CsvData(17) = GetTenmast(Jikinko, KData.TorimatomeSit, db)                                            ' ���܂ƂߓX��
                    CsvData(18) = KData.SyoriKen.Trim                                            ' ��������
                    CsvData(19) = KData.Syorikin.Trim                                            ' �������z
                    CsvData(20) = KData.FunouKen.Trim                                            ' �s�\����
                    CsvData(21) = KData.FunouKin.Trim                                            ' �s�\���z
                    CsvData(22) = KData.FuriKen.Trim                                             ' �U�֌���
                    CsvData(23) = KData.FuriKin.Trim                                             ' �U�֋��z
                    CsvData(24) = KData.TesuuKin.Trim                                            ' �萔��
                    CsvData(25) = KData.JifutiTesuuKin.Trim                                           ' �萔������|���U
                    CsvData(26) = KData.FurikomiTesuukin.Trim                                           ' �萔������|�U��
                    CsvData(27) = KData.SonotaTesuuKin.Trim                                           ' �萔������|���̑�
                    CsvData(28) = KData.NyukinKen.Trim                                           ' ����������
                    CsvData(29) = KData.NyukinKin.Trim                                           ' ���������z
                    CsvData(30) = KData.OpeCode.Substring(0, 2)                             ' �ȖڃR�[�h
                    CsvData(31) = KData.OpeCode.Substring(2, 3)                             ' �I�y�R�[�h
                    Select Case KData.OpeCode
                        Case "04099"
                            CsvData(32) = "�ʒi�x��"                                                        ' �I�y���[�V������
                        Case "01010"
                            CsvData(32) = "��������"
                        Case "02019"
                            CsvData(32) = "���ʓ���(NB)"
                        Case "04019"
                            CsvData(32) = "�ʒi����"
                        Case "99019"
                            CsvData(32) = "���������"
                        Case "48100"
                            CsvData(32) = "�ב֐U��"
                        Case "48500"
                            CsvData(32) = "�ב֕t��"
                        Case "48600"
                            CsvData(32) = "�ב֐���"
                        Case "99418"
                            CsvData(32) = "�萔������(�A��)"
                        Case "99419"
                            CsvData(32) = "������A������"
                    End Select

                    CsvData(33) = KData.ope_nyukin.Trim                                          ' ���o���z�i�I�y�R�[�h�P�ʁj
                    CsvData(34) = KData.ope_tesuu.Trim                                           ' �萔���z�i�I�y�R�[�h�P�ʁj

                    If newkey = oldkey Then
                        CsvData(35) = "0"                                           ' �W�v�t���O �W�v���Ȃ�
                    Else
                        CsvData(35) = "1"                                          ' �W�v�t���O �W�v����
                    End If

                    Select Case KData.ToriKbn                     ' 0�F�������ςƎ萔�������̗����̐�A1�F�������ς̂ݐ�A2�F�萔�������̂ݐ�
                        Case "0"
                            CsvData(36) = "���ρE�萔"
                        Case "1"
                            CsvData(36) = "����"
                        Case "2"
                            CsvData(36) = "�萔"
                        Case Else
                            CsvData(36) = ""
                    End Select

                    '2010/02/02 �ǉ�
                    CsvData(37) = GetTenmast(KData.KesKinCode, "", db, True)                        '���ϋ��Z�@�֖�
                    CsvData(38) = GetTenmast(KData.KesKinCode, KData.KesSitCode, db, False)         '���ώx�X��
                    '================

                    '�b�r�u�o�͏���
                    If CSVObject.Output(CsvData) = 0 Then
                        Return -1
                    End If

                    oldkey = KData.TorisCode & KData.TorifCode & KData.FuriDate
                    OraKesReader.NextRead()
                End While
            Else
                Return -1
            End If


        Catch ex As Exception
            Mainlog.Write("�������ώ������f�[�^�쐬", "���s", ex.Message)
            Return -300
        Finally
            If Not OraKesReader Is Nothing Then OraKesReader.Close()
        End Try

        Return 0

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
    '2010/02/02 �ǉ�
    Private Function GetTenmast(ByVal kinCode As String, ByVal sitCode As String, ByVal db As CASTCommon.MyOracle, ByVal KIN As Boolean) As String


        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(db)
        Dim strKinName As String = ""

        Try
            If KIN = True Then
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' ORDER BY SIT_NO_N")
            Else
                sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & kinCode & "' AND SIT_NO_N = '" & sitCode & "'")
            End If


            If orareader.DataReader(sql) = True Then
                If KIN = True Then
                    strKinName = orareader.GetString("KIN_KNAME_N")
                Else
                    strKinName = orareader.GetString("SIT_KNAME_N")
                End If
                Return strKinName
            Else
                Return ""
            End If

        Catch ex As Exception
            Return ""
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function
    '===============
    ' �@�\�@ �F �������σf�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    ''
    Private Function fn_KessaiData(ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal OraReader As CASTCommon.MyOracleReader, ByVal TimeStamp As String) As Boolean

        Dim strKamokuOpeCode As String

        Try
            Dim T_01010 As New CAstFormKes.ClsFormSikinFuri.T_01010
            Dim T_02019 As New CAstFormKes.ClsFormSikinFuri.T_02019
            Dim T_04019 As New CAstFormKes.ClsFormSikinFuri.T_04019
            Dim T_04099 As New CAstFormKes.ClsFormSikinFuri.T_04099
            Dim T_04419 As New CAstFormKes.ClsFormSikinFuri.T_04419
            Dim T_48100 As New CAstFormKes.ClsFormSikinFuri.T_48100
            Dim T_48500 As New CAstFormKes.ClsFormSikinFuri.T_48500
            Dim T_48600 As New CAstFormKes.ClsFormSikinFuri.T_48600
            Dim T_99019 As New CAstFormKes.ClsFormSikinFuri.T_99019
            Dim T_99418 As New CAstFormKes.ClsFormSikinFuri.T_99418
            Dim T_99419 As New CAstFormKes.ClsFormSikinFuri.T_99419

            KData.OpeCode = OraReader.GetString("KAMOKU_CODE_KR") + OraReader.GetString("OPE_CODE_KR")
            strKamokuOpeCode = KData.OpeCode

            KData.record320 = OraReader.GetString("DENBUN_ALL_KR")
            KData.KessaiKbn = OraReader.GetString("KESSAI_KBN_T")                              '���ϋ敪
            KData.KesKouza = OraReader.GetString("TUKEKOUZA_T")
            Select Case KData.OpeCode
                Case "01010" '��������
                    T_01010.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_01010.KINGAKU
                    KData.ope_tesuu = ""
                Case "02019" '���ʓ���(NB)
                    T_02019.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_02019.KINGAKU
                    KData.ope_tesuu = ""
                Case "04019" '�ʒi����(NB)
                    T_04019.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_04019.KINGAKU
                    KData.ope_tesuu = ""
                Case "04099" '�ʒi�x��(NB)
                    T_04099.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_04099.KINGAKU
                    KData.ope_tesuu = ""
                Case "48100" '�U���֘A
                    T_48100.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_48100.KINGAKU
                    KData.ope_tesuu = ""
                Case "48500" '�G�t��
                    T_48500.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_48500.KINGAKU
                    KData.ope_tesuu = ""
                Case "48600" '�G����
                    T_48600.DataSepaPlus = KData.record320
                    KData.ope_nyukin = T_48600.KINGAKU
                    KData.ope_tesuu = ""
                Case "99019" '���������
                    T_99019.DataSepaPlus = KData.record320
                    If KData.KessaiKbn = "00" AndAlso _
                       T_99019.KOUZA_NO = "00" & Mid(KData.KesKouza, 1, 5) Then
                        KData.ope_tesuu = ""
                        KData.ope_nyukin = T_99019.KINGAKU
                    Else
                        KData.ope_tesuu = T_99019.KINGAKU
                        KData.ope_nyukin = ""
                    End If
                    'If T_99019.ZENZAN.Trim = "0" Then
                    '    KData.ope_tesuu = T_99019.KINGAKU
                    '    KData.ope_nyukin = ""
                    'Else
                    '    KData.ope_tesuu = ""
                    '    KData.ope_nyukin = T_99019.KINGAKU
                    'End If
                Case "99418" '�萔������(�A��)
                    T_99418.DataSepaPlus = KData.record320
                    KData.ope_nyukin = ""
                    KData.ope_tesuu = T_99418.TESUU_KINGAKU
                Case "99419" '������A������
                    T_99419.DataSepaPlus = KData.record320
                    KData.ope_nyukin = ""
                    KData.ope_tesuu = T_99419.KINGAKU
            End Select
            ' �f�[�^�ݒ�
            With KData
                '.KessaiDate = ParaKessaiDate                            '���ϓ�
                .TorisCode = OraReader.GetString("TORIS_CODE_T")                              '������R�[�h
                .TorifCode = OraReader.GetString("TORIF_CODE_T")                              '����敛�R�[�h
                .ToriKName = OraReader.GetString("ITAKU_KNAME_T")
                .ToriNName = OraReader.GetString("ITAKU_NNAME_T")
                .FuriCode = OraReader.GetString("FURI_CODE_T")                                '�U�փR�[�h
                .KigyoCode = OraReader.GetString("KIGYO_CODE_T")                              '��ƃR�[�h
                .FuriDate = OraReader.GetString("FURI_DATE_S")                                '�U�֓�
                .KessaiKbn = OraReader.GetString("KESSAI_KBN_T")                              '���ϋ敪
                .KesKinCode = OraReader.GetString("TUKEKIN_NO_T")                             '���ϋ��Z�@�փR�[�h
                .KesSitCode = OraReader.GetString("TUKESIT_NO_T")                             '���ώx�X�R�[�h
                .KesKamoku = OraReader.GetString("TUKEKAMOKU_T")                              '���ωȖ�
                .KesKouza = OraReader.GetString("TUKEKOUZA_T")                                '���ό����ԍ�
                .TesTyoKbn = OraReader.GetString("TESUUTYO_KBN_T")                            '�萔�������敪
                .TesTyohh = OraReader.GetString("TESUUTYO_PATN_T")                            '�萔���������@
                .TorimatomeSit = OraReader.GetString("TORIMATOME_SIT_T")                      '�Ƃ�܂ƂߓX�R�[�h
                .SyoriKen = OraReader.GetString("SYORI_KEN_S")                                  '��������
                .Syorikin = OraReader.GetString("SYORI_KIN_S")                                   '�������z
                .FunouKen = OraReader.GetString("FUNOU_KEN_S")                                   '�s�\����
                .FunouKin = OraReader.GetString("FUNOU_KIN_S")                                   '�s�\���z
                .FuriKen = OraReader.GetString("FURI_KEN_S")                                     '�U�֌���
                .FuriKin = OraReader.GetString("FURI_KIN_S")                                     '�U�֋��z
                .TesuuKin = OraReader.GetString("TESUU_KIN_S")                                   '�萔��
                .JifutiTesuuKin = OraReader.GetString("TESUU_KIN1_S")                           '���U�萔��
                .FurikomiTesuukin = OraReader.GetString("TESUU_KIN3_S")                          '�U���萔��
                .SonotaTesuuKin = OraReader.GetString("TESUU_KIN2_S")                           '���̑��萔��
                .NyukinKen = OraReader.GetString("FURI_KEN_S")              '��������
                If .TesTyohh = "0" AndAlso .TesTyoKbn = "0" Then
                    If Long.Parse(.FuriKin) - Long.Parse(.TesuuKin) >= 0 Then
                        .NyukinKin = (Long.Parse(.FuriKin) - Long.Parse(.TesuuKin)).ToString              '�������z
                    Else
                        .NyukinKin = .FuriKin
                    End If
                Else
                    .NyukinKin = .FuriKin
                End If
                If OraReader.GetString("TESUU_TIME_STAMP_S") = TimeStamp AndAlso OraReader.GetString("KESSAI_TIME_STAMP_S") = TimeStamp Then
                    .ToriKbn = "0"
                ElseIf OraReader.GetString("KESSAI_TIME_STAMP_S") = TimeStamp Then
                    .ToriKbn = "1"
                ElseIf OraReader.GetString("TESUU_TIME_STAMP_S") = TimeStamp Then
                    .ToriKbn = "2"
                End If
                .TesuuTyoFlg = OraReader.GetString("TESUUTYO_FLG_S")
            End With

            ' �Œ蒷�ɕϊ�����
            KData.Data = KData.Data

        Catch ex As Exception
            Mainlog.Write("�������σf�[�^�쐬", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function
    ' �@�\�@ �F �w�Z�}�X�^�Q�̌��ώ�ʂ��擾
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F ���ώ�� = 0�F�ϑ��҈ꊇ���� or 1�F��ڌ����P�ʐ���
    '
    Private Function GetKessaiSyubetu(ByVal GakkouCode As String, ByRef KessaiSyubetu As String) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraGak2Reader As New CASTCommon.MyOracleReader(MainDB)
        Try
            SQL.Append("SELECT")
            SQL.Append(" KESSAI_SYUBETU_T")
            SQL.Append(" FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(GakkouCode))
            If OraGak2Reader.DataReader(SQL) = True Then
                KessaiSyubetu = OraGak2Reader.GetString("KESSAI_SYUBETU_T")
            Else
                Throw New Exception("�w�Z�}�X�^�Q�ɊY���f�[�^�����݂��܂���B�w�Z�R�[�h�F" & GakkouCode)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("���ώ�ʎ擾����", "���s", ex.Message)
            Return False
        Finally
            OraGak2Reader.Close()
            OraGak2Reader = Nothing
        End Try
    End Function
End Class
