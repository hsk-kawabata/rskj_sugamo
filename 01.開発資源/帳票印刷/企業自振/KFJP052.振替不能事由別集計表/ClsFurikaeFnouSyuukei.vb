Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic

' 
Public Class ClsFurikaeFnouSyuukei

    ' ����
    Public TORIS_CODE As String         ' ������R�[�h
    Public TORIF_CODE As String         ' ����敛�R�[�h
    Public FURI_DATE As String          ' �U�֓�
    Public PRINTERNAME As String = ""   ' �o�̓v�����^
    Public INVOKE_KBN As String = ""    ' �Ăяo���敪      '2012/06/30 �W���Ł@WEB�`���Ή�

    ' ���O�����N���X
    Private LOG As CASTCommon.BatchLOG

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    ' FSKJ.INI �Z�N�V������
    Private ReadOnly AppTOUROKU As String = "REPORTS"

    ' �@�\   �F �U�֕s�\���R�ʏW�v�\���C������
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

        Dim bRet As Boolean
        Try
            LOG = New CASTCommon.BatchLOG("�U�֕s�\���R�ʏW�v�\", AppTOUROKU)

            LOG.ToriCode = TORIS_CODE & TORIF_CODE
            LOG.FuriDate = FURI_DATE

            ' �U�֕s�\���R�ʏW�v�\�������
            bRet = PrintFunouSyuukei()

        Catch ex As Exception
            LOG.Write("�U�֕s�\���R�ʏW�v�\", "���s", ex.Message & ":" & ex.StackTrace)

            Return -1
        Finally
            MainDB.Close()
        End Try

        If bRet Then
            Return 0
        Else
            Return 2
        End If

    End Function

    ' �@�\   �F �U�֕s�\���R�ʏW�v�\����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l   �F 
    '
    Private Function PrintFunouSyuukei() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim sFMTKbn As String = ""

        ' �����ɃR�[�h
        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

        '�����ɖ���ini�t�@�C������擾
        Dim Jikinko_NAME As String = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
        '�Ƃ�܂ƂߓX��
        Dim strTORIMATOME_SIT_T As String = ""
        Dim strTORIMATOME_SIT_NAME As String = ""
        Dim strITAKU_KANRI_CODE As String = ""

        '2013/11/14 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
        '' �����
        'Dim sTax As String = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
        'If sTax = "err" Then
        '    sTax = "1.05"
        'End If
        '2013/11/14 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<

        Dim TesuuKin1 As Long = 0                    ' �萔�����z�P
        Dim TesuuKin2 As Long = 0                    ' �萔�����z�Q
        Dim TesuuKin3 As Long = 0                    ' �萔�����z�R
        Dim JifuriKin As Long = 0                    ' ���U���z

        '2012/06/30 �W���Ł@WEB�`���Ή�
        Dim User_ID As String = ""  '���[�U�h�c
        Dim ITAKU_CODE As String = "" '�ϑ��҃R�[�h
        Dim WEB_PRINTERNAME As String = "" 'WEB�`���p�v�����^��
        Dim WEB_PRINT As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_PRINT")    'WEB�`���v�����g�敪(0:PDF�̂݁A1:PDF�Ǝ��j

        SQL = New StringBuilder(128)
        SQL.Append("SELECT")
        SQL.Append(" FMT_KBN_T")
        SQL.Append(", TORIMATOME_SIT_T")
        SQL.Append(" FROM TORIMAST")
        SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TORIS_CODE))
        SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF_CODE))
        Dim OraTori As New CASTCommon.MyOracleReader(MainDB)
        If OraTori.DataReader(SQL) = True Then
            sFMTKbn = OraTori.GetValue(0)
            strTORIMATOME_SIT_T = OraTori.GetValue(1)
        End If
        OraTori.Close()

        '�Ƃ�܂ƂߓX���擾
        Dim OraTen As New CASTCommon.MyOracleReader(MainDB)
        SQL.Length = 0
        SQL.Append("SELECT SIT_NNAME_N")
        SQL.Append(" FROM TENMAST")
        SQL.Append(" WHERE KIN_NO_N = " & SQ(Jikinko))
        SQL.Append(" AND SIT_NO_N = " & SQ(strTORIMATOME_SIT_T))
        SQL.Append(" GROUP BY KIN_NO_N")
        SQL.Append(", SIT_NO_N")
        SQL.Append(", SIT_NNAME_N")
        If OraTen.DataReader(SQL) = True Then
            strTORIMATOME_SIT_NAME = OraTen.GetValue(0)
        End If
        OraTen.Close()

        Dim PrnFurikaeFunouSyuukei As New ClsPrnFurikaeFunouSyuukei    ' �U�֕s�\���R�ʏW�v�\

        SQL = New StringBuilder(128)
        '2013/11/14 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
        '����őΉ��Ɠ����ɕs�v�ȍ��ڂ��R�����g�A�E�g����B
        SQL.Append("SELECT")
        SQL.Append(" TORIMAST.TORIS_CODE_T")
        SQL.Append(",TORIMAST.TORIF_CODE_T")
        SQL.Append(",TORIMAST.ITAKU_NNAME_T")
        SQL.Append(",MEIMAST.FURI_DATE_K")
        SQL.Append(",TORIMAST.TORIMATOME_SIT_T")
        'SQL.Append(",TORIMAST.FUNOU_MEISAI_KBN_T")
        'SQL.Append(",TENMAST.KIN_NNAME_N")
        'SQL.Append(",TENMAST.SIT_NNAME_N")
        'SQL.Append(",MEIMAST.ITAKU_KIN_K")
        'SQL.Append(",MEIMAST.ITAKU_SIT_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KIN_K")
        'SQL.Append(",MEIMAST.KEIYAKU_SIT_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K")
        'SQL.Append(",MEIMAST.KEIYAKU_KNAME_K")
        SQL.Append(",MEIMAST.FURIKIN_K")
        SQL.Append(",MEIMAST.FURIKETU_CODE_K")
        'SQL.Append(",MEIMAST.JYUYOUKA_NO_K")
        'SQL.Append(",SCHMAST.SYORI_KEN_S")
        'SQL.Append(",SCHMAST.SYORI_KIN_S")
        'SQL.Append(",SCHMAST.FUNOU_KEN_S")
        'SQL.Append(",SCHMAST.FUNOU_KIN_S")
        'SQL.Append(",SCHMAST.FURI_KEN_S")
        'SQL.Append(",SCHMAST.FURI_KIN_S")
        'SQL.Append(",SCHMAST.TESUU_KIN_S")
        'SQL.Append(",SCHMAST.TESUU_KIN1_S")
        'SQL.Append(",SCHMAST.TESUU_KIN2_S")
        'SQL.Append(",SCHMAST.TESUU_KIN3_S")
        SQL.Append(",TORIMAST.ITAKU_CODE_T")
        'SQL.Append(",TORIMAST.TESUUTYO_KBN_T")
        'SQL.Append(",TORIMAST.TESUUMAT_PATN_T")
        'SQL.Append(",TRUNC(")
        'SQL.Append("  ((KIHTESUU_T * DECODE(SEIKYU_KBN_T,'0',SYORI_KEN_S,FURI_KEN_S) / 100)")
        'SQL.Append("   + NVL(KOTEI_TESUU1_T,0)")
        'SQL.Append("   + NVL(KOTEI_TESUU2_T,0))")
        'SQL.Append("   * DECODE(SYOUHI_KBN_T,'0'," & sTax & ",1)) TESUU_KIN1")     ' �����萔��
        'SQL.Append(",SOURYO_T")                       ' ����
        'SQL.Append(",TESUU1_T")                       ' ��ʎ萔���P
        'SQL.Append(",TESUU2_T")                       ' ��ʎ萔���Q
        'SQL.Append(",TESUU3_T")                       ' ��ʎ萔���R
        'SQL.Append(",TESUU4_T")                       ' ��ʎ萔���S
        'SQL.Append(",TESUU5_T")                       ' ��ʎ萔���T
        'SQL.Append(",TESUU6_T")                       ' ��ʎ萔���U
        'SQL.Append(",TUKEKIN_NO_T")                   ' ���ϋ��Z�@��
        'SQL.Append(",TUKESIT_NO_T")                   ' ���ώx�X
        'SQL.Append(",TORIMATOME_SIT_T")              ' �Ƃ�܂ƂߓX
        'SQL.Append(", SCHMAST.FILE_SEQ_S")
        ' 2012/06/30 �W���Ł@WEB�`���Ή�------------------------------------->
        SQL.Append(", TORIMAST.BAITAI_CODE_T")      '�}�̃R�[�h
        SQL.Append(", TORIMAST.MULTI_KBN_T")        '�}���`�敪
        SQL.Append(", TORIMAST.ITAKU_KANRI_CODE_T") '��\�ϑ��҃R�[�h
        ' 2012/06/30 �W���Ł@WEB�`���Ή�-------------------------------------<
        SQL.Append(" FROM TORIMAST")
        'SQL.Append(", (SELECT KIN_NO_N")
        'SQL.Append(", SIT_NO_N")
        'SQL.Append(", KIN_NNAME_N")
        'SQL.Append(", SIT_NNAME_N")
        'SQL.Append(" FROM TENMAST")
        'SQL.Append(" GROUP BY KIN_NO_N")
        'SQL.Append(", SIT_NO_N")
        'SQL.Append(", KIN_NNAME_N")
        'SQL.Append(", SIT_NNAME_N) TENMAST")
        SQL.Append(",MEIMAST")
        SQL.Append(",SCHMAST")
        SQL.Append(" WHERE ")
        SQL.Append(" FSYORI_KBN_T = '1'")
        If TORIS_CODE = "9999999999" AndAlso TORIF_CODE = "99" Then
            ' �S���
            SQL.Append(" AND MEIMAST.DATA_KBN_K = DECODE(FMT_KBN_T,'02','3','2')")
        Else
            SQL.Append(" AND TORIS_CODE_T = " & SQ(TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF_CODE))
            If sFMTKbn = "02" Then
                ' ���ł̏ꍇ�C
                SQL.Append(" AND MEIMAST.DATA_KBN_K = '3'")
            Else
                SQL.Append(" AND MEIMAST.DATA_KBN_K = '2'")
            End If
        End If
        SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))
        SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND SCHMAST.FURI_DATE_S = MEIMAST.FURI_DATE_K")
        SQL.Append(" AND SCHMAST.TORIS_CODE_S = MEIMAST.TORIS_CODE_K")
        SQL.Append(" AND SCHMAST.TORIF_CODE_S = MEIMAST.TORIF_CODE_K")
        SQL.Append(" AND SCHMAST.FSYORI_KBN_S = TORIMAST.FSYORI_KBN_T")
        SQL.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
        SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
        'SQL.Append(" AND TENMAST.KIN_NO_N = MEIMAST.KEIYAKU_KIN_K")
        'SQL.Append(" AND TENMAST.SIT_NO_N = MEIMAST.KEIYAKU_SIT_K")
        SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
        SQL.Append(", SCHMAST.TORIS_CODE_S")
        SQL.Append(", SCHMAST.TORIF_CODE_S")
        SQL.Append(", MEIMAST.RECORD_NO_K")
        '2013/11/14 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<

        Dim name As String = ""
        'Dim KinkoCd As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

        Dim bSQL As Boolean
        bSQL = OraReader.DataReader(SQL)

        If bSQL = True Then

            ' 2012/06/30 �W���Ł@WEB�`���Ή�------------------------------------->
            If OraReader.GetString("BAITAI_CODE_T") = "10" And INVOKE_KBN = "1" Then
                WEB_PRINTERNAME = CASTCommon.GetFSKJIni("WEB_DEN", "PRINTER")  'PDF���쐬����v�����^����ݒ�
                SQL = New StringBuilder(128)
                Dim OraWebReader As New CASTCommon.MyOracleReader(MainDB)
                SQL.Append(" SELECT USER_ID_W ")
                SQL.Append(" FROM WEB_RIREKIMAST ")
                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                    SQL.Append(" WHERE TORIS_CODE_W = '" & OraReader.GetString("TORIS_CODE_T") & "'")
                    SQL.Append(" AND TORIF_CODE_W  = '" & OraReader.GetString("TORIF_CODE_T") & "'")
                Else
                    SQL.Append(" WHERE ITAKU_KANRI_CODE_W = '" & OraReader.GetString("ITAKU_KANRI_CODE_T") & "'")
                End If
                SQL.Append(" AND FURI_DATE_W = '" & CASTCommon.ConvertDate(OraReader.GetItem("FURI_DATE_K"), "yyyyMMdd") & "'")
                SQL.Append(" AND FSYORI_KBN_W = '1'")

                If OraWebReader.DataReader(SQL) Then
                    User_ID = OraWebReader.GetString("USER_ID_W")
                Else
                    User_ID = ""
                End If

                ITAKU_CODE = OraReader.GetString("ITAKU_CODE_T")

                OraWebReader.Close()

            End If
            ' 2012/06/30 �W���Ł@WEB�`���Ή�-------------------------------------<

            name = PrnFurikaeFunouSyuukei.CreateCsvFile()

            Dim OldKey As String = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")

            Do
                PrnFurikaeFunouSyuukei.OutputCsvData(CASTCommon.ConvertDate(OraReader.GetItem("FURI_DATE_K"), "yyyyMMdd"))  '�U�֓�
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))          ' �ϑ��Җ�����
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"))           ' �ϑ��҃R�[�h

                PrnFurikaeFunouSyuukei.OutputCsvData(Jikinko_NAME)      '�����ɖ�
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"))       ' �Ƃ�܂ƂߓX�R�[�h
                PrnFurikaeFunouSyuukei.OutputCsvData(strTORIMATOME_SIT_NAME)                        ' �Ƃ�܂ƂߓX������
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetString("FURIKETU_CODE_K"))        ' �U�֌��ʃR�[�h
                PrnFurikaeFunouSyuukei.OutputCsvData(OraReader.GetInt64("FURIKIN_K").ToString)      ' �U�ֈ˗����z�i�U�֋��zK�j

                PrnFurikaeFunouSyuukei.OutputCsvData(mMatchingDate, False, True)

                OraReader.NextRead()

                If OraReader.EOF = True OrElse OldKey <> OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") Then
                    If OraReader.EOF = False Then
                        OldKey = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                    End If
                End If

            Loop Until OraReader.EOF    ' EOF�܂ō�Ƃ��J��Ԃ��B

            OraReader.Close()

            ' ���[�o��
            'If PrnFurikaeFunouSyuukei.ReportExecute(PRINTERNAME) = True Then
            '    LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "����")
            'Else
            '    LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "���s", PrnFurikaeFunouSyuukei.ReportMessage)
            '    Return False
            'End If

            '2012/06/30 �W���Ł@WEB�`���Ή�
            If User_ID <> "" Then
                If PrnFurikaeFunouSyuukei.ReportExecute(WEB_PRINTERNAME, User_ID, ITAKU_CODE) = True Then
                    LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "����")
                Else
                    LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "���s", PrnFurikaeFunouSyuukei.ReportMessage)
                    Return False
                End If

                If WEB_PRINT = "1" Then '�敪���P�̏ꍇ�A�ʏ�g���v�����^�ł��������
                    If PrnFurikaeFunouSyuukei.ReportExecute(PRINTERNAME) = True Then
                       LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "����")
                    Else
                        LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "���s", PrnFurikaeFunouSyuukei.ReportMessage)
                        Return False
                    End If
                End If
            Else
                If PrnFurikaeFunouSyuukei.ReportExecute(PRINTERNAME) = True Then
                    LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "����")
                Else
                    LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "���s", PrnFurikaeFunouSyuukei.ReportMessage)
                    Return False
                End If
            End If

            If Not PrnFurikaeFunouSyuukei.HostCsvName Is Nothing AndAlso PrnFurikaeFunouSyuukei.HostCsvName <> "" Then
                Try
                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    DestName &= PrnFurikaeFunouSyuukei.HostCsvName
                    File.Copy(PrnFurikaeFunouSyuukei.FileName, DestName, True)
                Catch ex As Exception
                    LOG.Write("�U�֕s�\���R�ʏW�v�\�o��", "���s", ex.Message)
                End Try
            End If

            Return True
        Else
            LOG.Write("�Ώۃf�[�^ �O��", "����")
            '20190910 maeda �Ώ�0�����̑Ή�
            Return True
            '20190910 maeda �Ώ�0�����̑Ή�
        End If
    End Function

End Class
