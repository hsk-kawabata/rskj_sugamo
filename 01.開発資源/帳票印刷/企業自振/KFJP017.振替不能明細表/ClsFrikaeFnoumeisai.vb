Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic
' 
Public Class ClsFrikaeFnoumeisai

    ' ����
    Public TORIS_CODE As String         ' ������R�[�h
    Public TORIF_CODE As String         ' ����敛�R�[�h
    Public FURI_DATE As String          ' �U�֓�
    Public FUNO_FLG As String           ' �s�\�t���O
    Public PRINTERNAME As String = ""   ' �o�̓v�����^
    Public INVOKE_KBN As String = ""    ' �Ăяo���敪      '2012/06/30 �W���Ł@WEB�`���Ή�

    Dim mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Dim mMatchingTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

    ' 2015/12/18 �^�X�N�j���� ADD �yPG�zUI_B-14-05(RSV2�Ή�) -------------------- START
    Private INI_RSV2_J_FUNOU As String = ""
    Private INI_COMMON_PRINT_NAME As String = ""
    ' 2015/12/18 �^�X�N�j���� ADD �yPG�zUI_B-14-05(RSV2�Ή�) -------------------- END

    ' �@�\   �F �U�֕s�\���ו\���C������
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
            MainLOG.ToriCode = TORIS_CODE & TORIF_CODE
            MainLOG.FuriDate = FURI_DATE

            ' �U�֕s�\���ו\�������
            bRet = PrintFunoumeisai()

        Catch ex As Exception
            MainLOG.Write("�U�֕s�\���ו\", "���s", ex.Message & ":" & ex.StackTrace)

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

    ' �@�\   �F �U�֕s�\���ו\�o�͏���
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l   �F 
    '
    Private Function PrintFunoumeisai() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim sFMTKbn As String = ""

        '*** �C�� mitsu 2009/06/11 ���v�����ɑ΂���萔���l�� ***
        ' �����ɃR�[�h
        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

        '�����ɖ���ini�t�@�C������擾
        Dim Jikinko_NAME As String = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
        '�Ƃ�܂ƂߓX��
        Dim strTORIMATOME_SIT_T As String = ""
        Dim strTORIMATOME_SIT_NAME As String = ""
        '�t�H�[�}�b�g�敪
        Dim strFormatKubun As String = ""
        Dim strFormatKubunName As String = ""

        '2013/11/14 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
        '' �����
        'Dim sTax As String = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
        'If sTax = "err" Then
        '    sTax = "1.05"
        'End If
        '2013/11/14 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<

        ' 2015/12/18 �^�X�N�j���� ADD �yPG�zUI_B-14-05(RSV2�Ή�) -------------------- START
        INI_RSV2_J_FUNOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "J_FUNOU")
        INI_COMMON_PRINT_NAME = CASTCommon.GetFSKJIni("COMMON", "PRINT_NAME")
        ' 2015/12/18 �^�X�N�j���� ADD �yPG�zUI_B-14-05(RSV2�Ή�) -------------------- END

        Dim TesuuKin1 As Long = 0                    ' �萔�����z�P
        Dim TesuuKin2 As Long = 0                    ' �萔�����z�Q
        Dim TesuuKin3 As Long = 0                    ' �萔�����z�R
        Dim JifuriKin As Long = 0                    ' ���U���z
        '********************************************************

        '2009/12/29      '�萔���e�[�u���t�@�C���̓Ǎ�
        '2013/11/14 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
        'Dim strTESUU_TABLE_FILE As String = ""
        'Dim intFILE_NO As Integer = 0
        '2013/11/14 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<
        Dim intKIJYUN_ID As Integer
        '2013/11/14 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
        'strTESUU_TABLE_FILE = Path.Combine(TXT_FOLDER, strTESUU_TABLE_FILE_NAME)
        'intFILE_NO = FreeFile()
        ''�ǎ�A�N�Z�X�ŊJ��
        'FileOpen(intFILE_NO, strTESUU_TABLE_FILE, OpenMode.Input)

        'Dim strKIJYUN_ID_CODE As String = ""
        'Dim strKIJYUN_ID_TEXT As String = ""
        'Dim intIndex As Integer = 0
        '2013/11/14 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<

        '2010/10/07.Sakon�@�t�H�[�}�b�g�敪�󎚗v�ۂ�INI�t�@�C������ݒ� +++++++++++++++
        Dim FMTKBN_PRINT As String = CASTCommon.GetFSKJIni("PRINT", "FMTKBN_PRINT")
        If FMTKBN_PRINT = "err" Then
            FMTKBN_PRINT = "1"
        End If
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        '2012/06/30 �W���Ł@WEB�`���Ή�
        Dim User_ID As String = ""  '���[�U�h�c
        Dim ITAKU_CODE As String = "" '�ϑ��҃R�[�h
        Dim WEB_PRINTERNAME As String = "" 'WEB�`���p�v�����^��
        Dim WEB_PRINT As String = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_PRINT")    'WEB�`���v�����g�敪(0:PDF�̂݁A1:PDF�Ǝ��j

        '2013/11/14 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
        'Do Until EOF(intFILE_NO)
        '    Input(intFILE_NO, strKIJYUN_ID_CODE)
        '    If strKIJYUN_ID_CODE = "" Then
        '        Exit Do
        '    End If
        '    Input(intFILE_NO, strKIJYUN_ID_TEXT)
        '    intIndex = CInt(strKIJYUN_ID_CODE)
        '    If intIndex = -1 Then
        '    Else
        '        If intIndex > 0 And intIndex < 10 Then
        '            TESUU_TABLE_DATA(intIndex).strKIJYUN_ID_CODE = strKIJYUN_ID_CODE
        '            TESUU_TABLE_DATA(intIndex).strKIJYUN_ID_TEXT = strKIJYUN_ID_TEXT
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_JITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_JITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_JITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_HONSITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_HONSITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_HONSITEN)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng10000UNDER_TAKOU)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000UNDER_TAKOU)
        '            Input(intFILE_NO, TESUU_TABLE_DATA(intIndex).lng30000OVER_TAKOU)
        '        End If
        '    End If
        'Loop
        'FileClose(intFILE_NO)
        '2013/11/14 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<
        '=============================================

        '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
        Me.TAX = New CASTCommon.ClsTAX
        Dim ZEIKIJUN As String = CASTCommon.GetFSKJIni("JIFURI", "ZEIKIJUN")
        If ZEIKIJUN.Equals("err") = True OrElse ZEIKIJUN = String.Empty Then
            MainLOG.Write("�U�֕s�\���ו\���", "���s", "[JIFURI]ZEIKIJUN �ݒ�Ȃ�")
            Return False
        End If
        Dim TaxKey As String = String.Empty         '�ŗ��擾���f�L�[
        '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

        '2011/06/16 �W���ŏC�� �����R�[�hALL9�Ή� ------------------START
        If TORIS_CODE = "9999999999" AndAlso TORIF_CODE = "99" Then
        Else
            '2011/06/16 �W���ŏC�� �����R�[�hALL9�Ή� ------------------END
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
            '2011/06/16 �W���ŏC�� �����R�[�hALL9�Ή� ------------------START
        End If
        '2011/06/16 �W���ŏC�� �����R�[�hALL9�Ή� ------------------END

        Dim PrnFunoumeisai As New ClsPrnFrikaeFunoumeisai    ' �U�֕s�\���ו\

        SQL = New StringBuilder(128)
        SQL.Append("SELECT")
        SQL.Append(" TORIMAST.TORIS_CODE_T")
        SQL.Append(",TORIMAST.TORIF_CODE_T")
        SQL.Append(",TORIMAST.ITAKU_NNAME_T")
        SQL.Append(",MEIMAST.FURI_DATE_K")
        SQL.Append(",TORIMAST.TORIMATOME_SIT_T")
        SQL.Append(",TORIMAST.FUNOU_MEISAI_KBN_T")
        SQL.Append(",TENMAST.KIN_NNAME_N")
        SQL.Append(",TENMAST.SIT_NNAME_N")
        SQL.Append(",MEIMAST.ITAKU_KIN_K")
        SQL.Append(",MEIMAST.ITAKU_SIT_K")
        SQL.Append(",MEIMAST.KEIYAKU_KIN_K")
        SQL.Append(",MEIMAST.KEIYAKU_SIT_K")
        SQL.Append(",MEIMAST.KEIYAKU_KAMOKU_K")
        SQL.Append(",MEIMAST.KEIYAKU_KOUZA_K")
        SQL.Append(",MEIMAST.KEIYAKU_KNAME_K")
        SQL.Append(",MEIMAST.FURIKIN_K")
        SQL.Append(",MEIMAST.FURIKETU_CODE_K")
        SQL.Append(",MEIMAST.JYUYOUKA_NO_K")
        SQL.Append(",SCHMAST.SYORI_KEN_S")
        SQL.Append(",SCHMAST.SYORI_KIN_S")
        SQL.Append(",SCHMAST.FUNOU_KEN_S")
        SQL.Append(",SCHMAST.FUNOU_KIN_S")
        SQL.Append(",SCHMAST.FURI_KEN_S")
        SQL.Append(",SCHMAST.FURI_KIN_S")
        SQL.Append(",SCHMAST.TESUU_KIN_S")
        SQL.Append(",SCHMAST.TESUU_KIN1_S")
        SQL.Append(",SCHMAST.TESUU_KIN2_S")
        SQL.Append(",SCHMAST.TESUU_KIN3_S")
        SQL.Append(",TORIMAST.ITAKU_CODE_T")
        '*** �C�� mitsu 2009/06/11 ���v�����ɑ΂���萔���l�� ***
        '�萔���v�Z�Ɏg�p���鍀�ڂ�ǉ�
        SQL.Append(",TORIMAST.TESUUTYO_KBN_T")
        SQL.Append(",TORIMAST.TESUUMAT_PATN_T")
        '2013/11/14 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
        '�����萔���͕ʓr�v�Z����
        SQL.Append(", TORIMAST.KIHTESUU_T")
        SQL.Append(", TORIMAST.SEIKYU_KBN_T")
        SQL.Append(", TORIMAST.KOTEI_TESUU1_T")
        SQL.Append(", TORIMAST.KOTEI_TESUU2_T")
        SQL.Append(", TORIMAST.SYOUHI_KBN_T")
        'SQL.Append(",TRUNC(")
        'SQL.Append("  ((KIHTESUU_T * DECODE(SEIKYU_KBN_T,'0',SYORI_KEN_S,FURI_KEN_S) / 100)")
        'SQL.Append("   + NVL(KOTEI_TESUU1_T,0)")
        'SQL.Append("   + NVL(KOTEI_TESUU2_T,0))")
        'SQL.Append("   * DECODE(SYOUHI_KBN_T,'0'," & sTax & ",1)) TESUU_KIN1")     ' �����萔��
        '2013/11/14 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<
        SQL.Append(",SOURYO_T")                       ' ����
        SQL.Append(", TORIMAST.TESUU_TABLE_ID_T")   '2009/12/29 �ǉ�
        SQL.Append(",TUKEKIN_NO_T")                   ' ���ϋ��Z�@��
        SQL.Append(",TUKESIT_NO_T")                   ' ���ώx�X
        '********************************************************
        SQL.Append(", SCHMAST.FILE_SEQ_S")
        SQL.Append(", TORIMAST.FMT_KBN_T")
        SQL.Append(", TORIMAST.FURI_CODE_T")        '�U�փR�[�h
        SQL.Append(", TORIMAST.KIGYO_CODE_T")       '��ƃR�[�h
        ' 2012/06/30 �W���Ł@WEB�`���Ή�------------------------------------->
        SQL.Append(", TORIMAST.BAITAI_CODE_T")      '�}�̃R�[�h
        SQL.Append(", TORIMAST.MULTI_KBN_T")        '�}���`�敪
        SQL.Append(", TORIMAST.ITAKU_KANRI_CODE_T") '��\�ϑ��҃R�[�h
        ' 2012/06/30 �W���Ł@WEB�`���Ή�-------------------------------------<
        '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
        SQL.Append(", SCHMAST.KESSAI_YDATE_S")
        '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<
        SQL.Append(" FROM TORIMAST")
        SQL.Append(", (SELECT KIN_NO_N")
        SQL.Append(", SIT_NO_N")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", SIT_NNAME_N")
        SQL.Append(" FROM TENMAST")
        SQL.Append(" GROUP BY KIN_NO_N")
        SQL.Append(", SIT_NO_N")
        SQL.Append(", KIN_NNAME_N")
        SQL.Append(", SIT_NNAME_N) TENMAST")
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
        SQL.Append(" AND TENMAST.KIN_NO_N(+) = MEIMAST.KEIYAKU_KIN_K")
        SQL.Append(" AND TENMAST.SIT_NO_N(+) = MEIMAST.KEIYAKU_SIT_K")
        SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
        SQL.Append(", SCHMAST.TORIS_CODE_S")
        SQL.Append(", SCHMAST.TORIF_CODE_S")
        SQL.Append(", MEIMAST.RECORD_NO_K")

        Dim name As String = ""
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

            name = PrnFunoumeisai.CreateCsvFile()


            Dim OldKey As String = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")

            Do
                '2011/06/16 �W���ŏC�� �����R�[�hALL9�Ή� ------------------START
                If TORIS_CODE = "9999999999" AndAlso TORIF_CODE = "99" Then
                    Dim OraTen As New CASTCommon.MyOracleReader(MainDB)
                    SQL.Length = 0
                    SQL.Append("SELECT SIT_NNAME_N")
                    SQL.Append(" FROM TENMAST")
                    SQL.Append(" WHERE KIN_NO_N = " & SQ(Jikinko))
                    SQL.Append(" AND SIT_NO_N = " & SQ(OraReader.GetString("TORIMATOME_SIT_T")))
                    SQL.Append(" GROUP BY KIN_NO_N")
                    SQL.Append(", SIT_NO_N")
                    SQL.Append(", SIT_NNAME_N")
                    If OraTen.DataReader(SQL) = True Then
                        strTORIMATOME_SIT_NAME = OraTen.GetValue(0)
                    End If
                    OraTen.Close()
                End If
                '2011/06/16 �W���ŏC�� �����R�[�hALL9�Ή� ------------------END
                '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
                If TaxKey.Equals(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & OraReader.GetString("FURI_DATE_K")) = False Then
                    '�ŗ��擾�L�[���قȂ�ꍇ�ɐŗ����擾����
                    Dim strKijunDate As String = String.Empty
                    If ZEIKIJUN.Equals("0") = True Then
                        strKijunDate = OraReader.GetString("FURI_DATE_K")
                    Else
                        strKijunDate = OraReader.GetString("KESSAI_YDATE_S")
                    End If

                    '�ŗ��擾
                    Me.TAX.GetZeiritsu(strKijunDate)
                    If Me.TAX.ZEIRITSU.Equals("err") = True Then
                        MainLOG.Write("�ŗ��擾", "���s", "����F" & strKijunDate)
                        Return False
                    End If

                    '�U���萔���}�X�^�ǂݍ���
                    If Me.GetJifuriTesuuTable(Me.TAX.ZEIRITSU_ID) = False Then
                        Return False
                    End If

                    '2013/12/27 saitou �W���� �󎆐őΉ� ADD -------------------------------------------------->>>>
                    '�󎆐Ŏ擾
                    Me.TAX.GetInshizei(strKijunDate)
                    If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                        MainLOG.Write("�󎆐Ŏ擾", "���s", "����F" & strKijunDate)
                        Return False
                    End If
                    '2013/12/27 saitou �W���� �󎆐őΉ� ADD --------------------------------------------------<<<<
                End If
                '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

                PrnFunoumeisai.OutputCsvData(Jikinko_NAME)      '�����ɖ�
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("ITAKU_NNAME_T"))      ' �ϑ��Җ�����
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("ITAKU_CODE_T"))       ' �ϑ��҃R�[�h
                PrnFunoumeisai.OutputCsvData(CASTCommon.ConvertDate(OraReader.GetItem("FURI_DATE_K"), "yyyyMMdd"))  '�U�֓�
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("TORIMATOME_SIT_T"))   ' �Ƃ�܂ƂߓX�R�[�h
                PrnFunoumeisai.OutputCsvData(strTORIMATOME_SIT_NAME)        ' �Ƃ�܂ƂߓX������
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_KIN_K"))      ' ���Z�@�փR�[�h
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_SIT_K"))      ' �x�X
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_KNAME_K"), True)    ' �������`
                If OraReader.GetString("KEIYAKU_KAMOKU_K") = "01" Then
                    PrnFunoumeisai.OutputCsvData("����")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "02" Then
                    PrnFunoumeisai.OutputCsvData("����")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "05" Then
                    PrnFunoumeisai.OutputCsvData("�[��")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "37" Then
                    PrnFunoumeisai.OutputCsvData("�E��")
                ElseIf OraReader.GetString("KEIYAKU_KAMOKU_K") = "09" Then
                    PrnFunoumeisai.OutputCsvData("���̑�")
                Else
                    PrnFunoumeisai.OutputCsvData("")
                End If

                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KEIYAKU_KOUZA_K"))        ' �����ԍ��i�_��Ҍ������j
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FURIKIN_K").ToString)      ' �U�ֈ˗����z�i�U�֋��zK�j
                If OraReader.GetInt64("FURIKETU_CODE_K") = 0 Then
                    PrnFunoumeisai.OutputCsvData("")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 1 Then
                    PrnFunoumeisai.OutputCsvData("1:�����s��")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 2 Then
                    PrnFunoumeisai.OutputCsvData("2:����Ȃ�")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 3 Then
                    PrnFunoumeisai.OutputCsvData("3:�a���ғs��")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 4 Then
                    PrnFunoumeisai.OutputCsvData("4:�˗����Ȃ�")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 8 Then
                    PrnFunoumeisai.OutputCsvData("8:�ϑ��ғs��")
                ElseIf OraReader.GetInt64("FURIKETU_CODE_K") = 9 Then
                    PrnFunoumeisai.OutputCsvData("9:���̑�")
                Else
                    PrnFunoumeisai.OutputCsvData("9:���̑�")
                End If
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("JYUYOUKA_NO_K"))      ' ���l�@
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FURI_KEN_S").ToString)     ' �U�֍ό���
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FURI_KIN_S").ToString)     ' �U�֍ϋ��z
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FUNOU_KEN_S").ToString)    ' �U�֕s�\����
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("FUNOU_KIN_S").ToString)    ' �U�֕s�\���z
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("SYORI_KEN_S").ToString)    ' �U�ֈ˗�����
                PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("SYORI_KIN_S").ToString)    ' �U�ֈ˗����z
                '*** �C�� mitsu 2009/06/11 ���v�����ɑ΂���萔���l�� ***
                'PrnKekkameisai.OutputCsvData(OraReader.GetInt64("TESUU_KIN_S"))         ' �萔������ō���S�i�萔��S�j
                If OraReader.GetString("TESUUTYO_KBN_T") <> "2" AndAlso OraReader.GetString("TESUUMAT_PATN_T") = "1" Then
                    '�萔�������敪�����ʖƏ��ȊO�Ŏ萔���������@�����v�����ɑ΂���萔���̏ꍇ�́A
                    '�}�X�^�̎萔��������萔���������̍��Z�l�ɂȂ��Ă���̂ŁA
                    '�P��X�P�W���[���݂̂̎萔�����Z�o����B
                    '2013/11/14 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
                    '�����萔���͕ʓr�v�Z����
                    If Me.CalcTesuuKin1(TesuuKin1, OraReader) = False Then
                        Return False
                    End If
                    'TesuuKin1 = OraReader.GetInt64("TESUU_KIN1") ' �����萔��
                    '2013/11/14 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<
                    TesuuKin2 = OraReader.GetInt64("SOURYO_T") ' ����
                    JifuriKin = OraReader.GetInt64("FURI_KIN_S") - (TesuuKin1 + TesuuKin2)
                    TesuuKin3 = 0

                    '2009/12/29
                    '�U���萔����h�c��ݒ�/�v�Z���̒ǉ�
                    If OraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                        intKIJYUN_ID = 0
                    Else
                        intKIJYUN_ID = CInt(OraReader.GetString("TESUU_TABLE_ID_T"))
                    End If

                    '2013/12/27 saitou �W���� �󎆐őΉ� UPD -------------------------------------------------->>>>
                    If OraReader.GetString("TUKEKIN_NO_T") = Jikinko Then
                        ' ���ϋ��Z�@�ւ��C�����ɂ̏ꍇ
                        If OraReader.GetString("TUKESIT_NO_T") = OraReader.GetString("TORIMATOME_SIT_T") Then
                            ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�C���X��
                            If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                            ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                ' �O�~���傫�� ���� �R���~����
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                            ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                ' �R���~�ȏ�
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                            End If
                        Else
                            ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�C�{�x�X
                            If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                ' �O�~���傫�� ���� �P���~����
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                            ElseIf Me.TAX.INSHIZEI1 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                ' �P���~���傫�� ���� �R���~����
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                            ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                ' �R���~�ȏ�
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                            End If
                        End If
                    Else
                        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            ' �O�~���傫�� ���� �P���~����
                            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                        ElseIf Me.TAX.INSHIZEI1 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            ' �P���~�~���傫�� ���� �R���~����
                            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            ' �R���~�ȏ�
                            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                        End If
                    End If

                    'If OraReader.GetString("TUKEKIN_NO_T") = Jikinko Then
                    '    ' ���ϋ��Z�@�ւ��C�����ɂ̏ꍇ
                    '    If OraReader.GetString("TUKESIT_NO_T") = OraReader.GetString("TORIMATOME_SIT_NO_T") Then
                    '        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�C���X��
                    '        If 0 < JifuriKin And JifuriKin < 10000 Then
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                    '        ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                    '            ' �O�~���傫�� ���� �R���~����
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                    '        ElseIf 30000 <= JifuriKin Then
                    '            ' �R���~�ȏ�
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                    '        End If
                    '    Else
                    '        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�C�{�x�X
                    '        If 0 < JifuriKin And JifuriKin < 10000 Then
                    '            ' �O�~���傫�� ���� �P���~����
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                    '        ElseIf 10000 < JifuriKin And JifuriKin < 30000 Then
                    '            ' �P���~���傫�� ���� �R���~����
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                    '        ElseIf 30000 <= JifuriKin Then
                    '            ' �R���~�ȏ�
                    '            TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                    '        End If
                    '    End If
                    'Else
                    '    If 0 < JifuriKin And JifuriKin < 10000 Then
                    '        ' �O�~���傫�� ���� �P���~����
                    '        TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                    '    ElseIf 10000 < JifuriKin And JifuriKin < 30000 Then
                    '        ' �P���~�~���傫�� ���� �R���~����
                    '        TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                    '    ElseIf 30000 <= JifuriKin Then
                    '        ' �R���~�ȏ�
                    '        TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                    '    End If
                    'End If
                    '2013/12/27 saitou �W���� �󎆐őΉ� UPD --------------------------------------------------<<<<
                    '=========================
                    '�}�X�^�̒l�ł͂Ȃ��Z�o�����l���o��
                    PrnFunoumeisai.OutputCsvData((TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)  ' �萔������ō���S�i�萔��S�j
                Else
                    PrnFunoumeisai.OutputCsvData(OraReader.GetInt64("TESUU_KIN_S").ToString)    ' �萔������ō���S�i�萔��S�j
                End If
                '********************************************************

                PrnFunoumeisai.OutputCsvData(mMatchingDate)
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KIN_NNAME_N"))
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("SIT_NNAME_N"))

                '2010/10/07.Sakon�@�t�H�[�}�b�g�敪�󎚗v�ۂ�INI�t�@�C������ݒ� +++++
                If FMTKBN_PRINT = "1" Then
                    strFormatKubun = OraReader.GetString("FMT_KBN_T")
                    ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- START
                    '�t�H�[�}�b�g�����e�L�X�g����擾����
                    strFormatKubunName = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_�t�H�[�}�b�g�敪.TXT"), _
                                                                       strFormatKubun)
                    'Select Case strFormatKubun
                    '    Case "00"
                    '        strFormatKubunName = "�S��"
                    '    Case "01"
                    '        strFormatKubunName = "�m�g�j"
                    '    Case "02"
                    '        strFormatKubunName = "����"
                    '    Case "03"
                    '        strFormatKubunName = "�N��"
                    '    Case "04"
                    '        strFormatKubunName = "�˗���"
                    '    Case "05"
                    '        strFormatKubunName = "�`�["
                    '    Case Else
                    '        strFormatKubunName = ""
                    'End Select
                    ' 2016/01/23 �^�X�N�j�֓� UPD �yPG�zUI_B-14-99(RSV2�Ή�) -------------------- END
                Else
                    strFormatKubunName = ""
                End If
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                PrnFunoumeisai.OutputCsvData(strFormatKubunName)                                '�t�H�[�}�b�g�敪
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("FURI_CODE_T"))                '�U�փR�[�h
                PrnFunoumeisai.OutputCsvData(OraReader.GetString("KIGYO_CODE_T"), False, True)  '��ƃR�[�h

                '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
                TaxKey = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & OraReader.GetString("FURI_DATE_K")
                '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

                OraReader.NextRead()

                If OraReader.EOF = True OrElse OldKey <> OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") Then
                    If OraReader.EOF = False Then
                        OldKey = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                    End If
                End If

            Loop Until OraReader.EOF    ' EOF�܂ō�Ƃ��J��Ԃ��B

            OraReader.Close()

            ' ���[�o��

            'If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
            '    MainLOG.Write("�U�֕s�\���ו\�o��", "����")
            'Else
            '    MainLOG.Write("�U�֕s�\���ו\�o��", "���s", PrnFunoumeisai.ReportMessage)
            '    Return False
            'End If

            '2012/06/30 �W���Ł@WEB�`���Ή�
            If User_ID <> "" Then
                If PrnFunoumeisai.ReportExecute(WEB_PRINTERNAME, User_ID, ITAKU_CODE) = True Then
                    MainLOG.Write("�U�֕s�\���ו\�o��", "����")
                Else
                    MainLOG.Write("�U�֕s�\���ו\�o��", "���s", PrnFunoumeisai.ReportMessage)
                    Return False
                End If

                If WEB_PRINT = "1" Then '�敪���P�̏ꍇ�A�ʏ�g���v�����^�ł��������
                    If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                        MainLOG.Write("�U�֕s�\���ו\�o��", "����")
                    Else
                        MainLOG.Write("�U�֕s�\���ו\�o��", "���s", PrnFunoumeisai.ReportMessage)
                        Return False
                    End If
                End If
            Else
                ' 2015/12/18 �^�X�N�j���� CHG �yPG�zUI_B-14-05(RSV2�Ή�) -------------------- START
                'If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                '    MainLOG.Write("�U�֕s�\���ו\�o��", "����")
                'Else
                '    MainLOG.Write("�U�֕s�\���ו\�o��", "���s", PrnFunoumeisai.ReportMessage)
                '    Return False
                'End If
                '=====================================================================================
                ' <INI_RSV2_J_FUNOU>
                '  CASE "1"  : �ʏ�v�����^���           (PRINTERNAME)
                '  CASE "2"  : ListWorks���              (INI_COMMON_PRINT_NAME)
                '  CASE "3"  : �ʏ�v�����^,ListWorks��� (PRINTERNAME,INI_COMMON_PRINT_NAME)
                '  CASE ELSE : �ʏ�v�����^���           (PRINTERNAME)
                '=====================================================================================
                Select Case INI_RSV2_J_FUNOU
                    Case "1"
                        If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                            MainLOG.Write("�U�֕s�\���ו\�o��", "����")
                        Else
                            MainLOG.Write("�U�֕s�\���ו\�o��", "���s", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                    Case "2"
                        If PrnFunoumeisai.ReportExecute(INI_COMMON_PRINT_NAME) = True Then
                            MainLOG.Write("�U�֕s�\���ו\�o��(ListWorks)", "����")
                        Else
                            MainLOG.Write("�U�֕s�\���ו\�o��(ListWorks)", "���s", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                    Case "3"
                        If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                            MainLOG.Write("�U�֕s�\���ו\�o��", "����")
                        Else
                            MainLOG.Write("�U�֕s�\���ו\�o��", "���s", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If

                        If PrnFunoumeisai.ReportExecute(INI_COMMON_PRINT_NAME) = True Then
                            MainLOG.Write("�U�֕s�\���ו\�o��(ListWorks)", "����")
                        Else
                            MainLOG.Write("�U�֕s�\���ו\�o��(ListWorks)", "���s", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                    Case Else
                        If PrnFunoumeisai.ReportExecute(PRINTERNAME) = True Then
                            MainLOG.Write("�U�֕s�\���ו\�o��", "����")
                        Else
                            MainLOG.Write("�U�֕s�\���ו\�o��", "���s", PrnFunoumeisai.ReportMessage)
                            Return False
                        End If
                End Select
                ' 2015/12/18 �^�X�N�j���� CHG �yPG�zUI_B-14-05(RSV2�Ή�) -------------------- END
            End If
 
            If Not PrnFunoumeisai.HostCsvName Is Nothing AndAlso PrnFunoumeisai.HostCsvName <> "" Then
                Try
                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    DestName &= PrnFunoumeisai.HostCsvName
                    File.Copy(PrnFunoumeisai.FileName, DestName, True)
                Catch ex As Exception
                    MainLOG.Write("���ʂb�r�u�o�͏���", "���s", ex.Message)
                End Try
            End If

            Return True
        Else
            MainLOG.Write("�Ώۃf�[�^ �O��", "����")
            '2010/02/18 0�~�f�[�^�Ή�
            Return True
            '========================
        End If

    End Function

    ''' <summary>
    ''' �����萔�����v�Z���܂��B
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <returns>�����萔��</returns>
    ''' <remarks>2013/11/14 �W���� ����őΉ�</remarks>
    Private Function CalcTesuuKin1(ByRef TesuuKin1 As Long, _
                                   ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Try
            '-------------------------------------------------------
            '�����萔���̌v�Z
            '-------------------------------------------------------
            Dim intKen As Integer = 0
            Select Case oraReader.GetString("SEIKYU_KBN_T")
                Case "0" : intKen = oraReader.GetInt("SYORI_KEN_S")
                Case "1" : intKen = oraReader.GetInt("FURI_KEN_S")
                Case Else   '�����敪�����ɂ���ΐݒ�
            End Select

            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "0" : TesuuKin1 = CLng(Math.Floor((oraReader.GetInt("KIHTESUU_T") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")) * Double.Parse(Me.TAX.ZEIRITSU)))
                Case "1" : TesuuKin1 = oraReader.GetInt("KIHTESUU_T") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")
                Case Else
            End Select

            Return True

        Catch ex As Exception
            MainLOG.Write("�����萔���v�Z", "���s", ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' �U���萔���ID�e�L�X�g��ǂݍ��݂܂��B
    ''' </summary>
    ''' <param name="TAX_ID">�ŗ�ID</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/27 �W���� ����őΉ�</remarks>
    Private Function GetJifuriTesuuTable(ByVal TAX_ID As String) As Boolean

        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            With SQL
                .Append("select * from TESUUMAST")
                .Append(" where TAX_ID_C = " & SQ(TAX_ID))
                .Append(" and FSYORI_KBN_C = '1'")
                .Append(" and SYUBETU_C = '91'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
                    'If oraReader.GetInt("TESUU_TABLE_ID_C") > 0 AndAlso oraReader.GetInt("TESUU_TABLE_ID_C") < 10 Then
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).strKIJYUN_ID_CODE = oraReader.GetString("TESUU_TABLE_ID_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).strKIJYUN_ID_TEXT = oraReader.GetString("TESUU_TABLE_NAME_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_JITEN = oraReader.GetInt("TESUU_A1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_JITEN = oraReader.GetInt("TESUU_A2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_JITEN = oraReader.GetInt("TESUU_A3_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_HONSITEN = oraReader.GetInt("TESUU_B1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_HONSITEN = oraReader.GetInt("TESUU_B2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_HONSITEN = oraReader.GetInt("TESUU_B3_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng10000UNDER_TAKOU = oraReader.GetInt("TESUU_C1_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000UNDER_TAKOU = oraReader.GetInt("TESUU_C2_C")
                    '    TESUU_TABLE_DATA(oraReader.GetInt("TESUU_TABLE_ID_C")).lng30000OVER_TAKOU = oraReader.GetInt("TESUU_C3_C")
                    'End If
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).strKIJYUN_ID_CODE = oraReader.GetString("TESUU_TABLE_ID_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).strKIJYUN_ID_TEXT = oraReader.GetString("TESUU_TABLE_NAME_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_JITEN = oraReader.GetInt("TESUU_A1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_JITEN = oraReader.GetInt("TESUU_A2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_JITEN = oraReader.GetInt("TESUU_A3_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_HONSITEN = oraReader.GetInt("TESUU_B1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_HONSITEN = oraReader.GetInt("TESUU_B2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_HONSITEN = oraReader.GetInt("TESUU_B3_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng10000UNDER_TAKOU = oraReader.GetInt("TESUU_C1_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000UNDER_TAKOU = oraReader.GetInt("TESUU_C2_C")
                    TESUU_TABLE_DATA(CInt(oraReader.GetString("TESUU_TABLE_ID_C"))).lng30000OVER_TAKOU = oraReader.GetInt("TESUU_C3_C")
                    ' 2016/09/01 �^�X�N�j���� ADD �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

                    oraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("�U���萔���}�X�^�ǂݍ���", "���s", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

End Class
