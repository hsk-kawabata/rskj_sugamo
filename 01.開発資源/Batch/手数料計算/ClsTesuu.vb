Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports System.Data
Imports CASTCommon.ModPublic
Imports CASTCommon
' �萔���v�Z����
Public Class ClsTesuu
    ' ���O�����N���X
    Private LOG As CASTCommon.BatchLOG

    ' �p�u���b�N�c�a
    Private MainDB As CASTCommon.MyOracle

    Friend Structure INI_PARAM
        Dim LOG_FOLDER As String
        Dim ZEI_RITU As String
        Dim EXE_FOLDER As String
        Dim LST_FOLDER As String
        Dim TXT_FOLDER As String
        Dim KINKO_CODE As String
        Dim CENTER_CODE As String
        '2013/11/13 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
        Dim ZEIKIJUN As String
        '2013/11/13 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<
    End Structure
    Private strcINI_PARAM As INI_PARAM

    '2013/11/13 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    '2013/11/13 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

    '2013/11/13 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
    'Private Const strTESUU_TABLE_FILE_NAME As String = "KFJMAST010_�U���萔���ID.TXT"
    '2013/11/13 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<
    '�萔���v�Z�p
    Structure TESUU_TABLE
        Dim strKIJYUN_ID_CODE As String         '�萔���ID
        Dim strKIJYUN_ID_TEXT As String         '�萔���ID��
        Dim lng10000UNDER_JITEN As Long         '�萔���i���X�P���~�����j
        Dim lng30000UNDER_JITEN As Long         '�萔���i���X�P���ȏ�R���~�����j
        Dim lng30000OVER_JITEN As Long          '�萔���i���X�R���~�ȏ�j
        Dim lng10000UNDER_HONSITEN As Long      '�萔���i�{�x�X�P���~�����j
        Dim lng30000UNDER_HONSITEN As Long      '�萔���i�{�x�X�P���ȏ�R���~�����j
        Dim lng30000OVER_HONSITEN As Long       '�萔���i�{�x�X�R���~�ȏ�j
        Dim lng10000UNDER_TAKOU As Long         '�萔���i���s�P�������j
        Dim lng30000UNDER_TAKOU As Long         '�萔���i���s�P���ȏ�R���~�����j
        Dim lng30000OVER_TAKOU As Long          '�萔���i���s�R���~�ȏ�j

        Public Sub Init()
            strKIJYUN_ID_CODE = String.Empty
            strKIJYUN_ID_TEXT = String.Empty
            lng10000UNDER_JITEN = 0
            lng30000UNDER_JITEN = 0
            lng30000OVER_JITEN = 0
            lng10000UNDER_HONSITEN = 0
            lng30000UNDER_HONSITEN = 0
            lng30000OVER_HONSITEN = 0
            lng10000UNDER_TAKOU = 0
            lng30000UNDER_TAKOU = 0
            lng30000OVER_TAKOU = 0
        End Sub
    End Structure
    ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
    ' �萔���e�[�u���̌�����100���܂łɕύX
    'Public TESUU_TABLE_DATA(10) As TESUU_TABLE
    Public TESUU_TABLE_DATA(100) As TESUU_TABLE
    ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

    Private Const msgTitle As String = "�萔���v�Z����(KFJ070)"

    ' �@�\�@ �F �萔���v�Z ���C������
    '
    ' �����@ �F ARG1 - �U�֓�
    '           ARG2 - �Čv�Z�t���O
    '           ARG3 - �W���u�ʔ�
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Public Function Main(ByVal furiDate As String, _
                         ByVal reCalcFlag As Integer, _
                         ByVal tuuban As Integer) As Integer


        '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 60
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME1")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 60
            End If
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

        Try

            LOG = New CASTCommon.BatchLOG("KFJ070", "�萔���v�Z")

            LOG.JobTuuban = tuuban
            LOG.FuriDate = furiDate

            LOG.Write("�萔���v�Z�����J�n", "����", "�U�֓��F" & furiDate & " �v�Z�t���O�F" & reCalcFlag.ToString)

            'INI�t�@�C���ǂݍ���
            If ReadIni() = False Then
                Return 1
            End If

            '2013/11/13 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
            Me.TAX = New CASTCommon.ClsTAX
            '2013/11/13 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

            '2013/11/13 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
            '�����ł͓ǂݍ��݂��s��Ȃ��B
            ''�萔���e�[�u���t�@�C���̓Ǎ�
            'Dim strTESUU_TABLE_FILE As String = ""
            'Dim intFILE_NO As Integer = 0

            'strTESUU_TABLE_FILE = Path.Combine(strcINI_PARAM.TXT_FOLDER, strTESUU_TABLE_FILE_NAME)
            'intFILE_NO = FreeFile()
            ''�ǎ�A�N�Z�X�ŊJ��
            'FileOpen(intFILE_NO, strTESUU_TABLE_FILE, OpenMode.Input)

            'Dim strKIJYUN_ID_CODE As String = ""
            'Dim strKIJYUN_ID_TEXT As String = ""
            'Dim intIndex As Integer = 0

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
            '2013/11/13 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<

            MainDB = New CASTCommon.MyOracle

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                LOG.Write_Err("�萔���v�Z����", "���s", "�萔���v�Z�����Ŏ��s�҂��^�C���A�E�g")
                If LOG.JobTuuban <> 0 Then
                    LOG.UpdateJOBMASTbyErr("�萔���v�Z�����Ŏ��s�҂��^�C���A�E�g")
                End If
                Return 1
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '�Čv�Z�t���O���L���Ȃ�A�萔���Čv�Z����
            If reCalcFlag = 1 Then
                Call KekkaReBorn(furiDate)
            End If

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            If CalcTesuu(furiDate, reCalcFlag) = 0 Then
                MainDB.Commit()
                LOG.Write("�萔���v�Z�����I��", "����")
                If LOG.JobTuuban <> 0 Then
                    LOG.UpdateJOBMASTbyOK("")
                End If
            Else
                MainDB.Rollback()
                LOG.Write("�萔���v�Z�����I��", "���s")
                If LOG.JobTuuban <> 0 Then
                    LOG.UpdateJOBMASTbyErr("�萔���v�Z ���s")
                End If
                Return 1
            End If

            Return 0

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "�萔���v�Z����", "���s", ex.Message)
            Return 1

        Finally
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            If Not MainDB Is Nothing Then 
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)
                ' ���[���o�b�N
                MainDB.Rollback()
                ' DB�N���[�Y
                MainDB.Close()
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
        End Try
    End Function

    ' �@�\�@ �F �萔���v�Z����
    '
    ' �����@ �F ARG1 - �U�֓�
    '           ARG2 - �Čv�Z�t���O �O�F�ʏ�C�P�F�Čv�Z
    '           ARG3 - �W���u�ʔ�
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function CalcTesuu(ByVal furiDate As String, ByVal reCalcFlag As Integer) As Integer

        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        ' �����ɃR�[�h
        Dim Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        '2013/11/13 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
        '' �����
        'Dim sTax As String = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
        'If sTax = "err" Then
        '    sTax = "1.05"
        'End If
        '2013/11/13 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<

        '2013/11/13 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
        '�^�ϊ��iDecimal��Long�j
        Dim TesuuKin1 As Long = 0                   ' �萔�����z�P
        Dim TesuuKin2 As Long = 0                   ' �萔�����z�Q
        Dim TesuuKin3 As Long = 0                   ' �萔�����z�R
        'Dim TesuuKin1 As Decimal = 0                    ' �萔�����z�P
        'Dim TesuuKin2 As Decimal = 0                    ' �萔�����z�Q
        'Dim TesuuKin3 As Decimal = 0                    ' �萔�����z�R
        '2013/11/13 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<

        Dim JifuriKin As Decimal = 0                    ' ���U���z

        Dim nSyoriKen As Integer = 0                    ' ��������

        Dim intKIJYUN_ID As Integer = 0                 '�U���萔����h�c

        Try
            ' �X�P�W���[���}�X�^�I��
            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_SV1")               '������R�[�h
            SQL.Append(", TORIF_CODE_SV1")              '����敛�R�[�h
            SQL.Append(", FURI_DATE_SV1")               '�U�֓�
            SQL.Append(", TESUUTYO_KBN_T")                   ' �萔�������敪
            '2013/11/13 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
            'SQL�ł̈����萔���̌v�Z�͍s�킸�A�v�Z�ɕK�v�ȍ��ڂ̎擾�ɕύX����B
            SQL.Append(", SYORI_KEN_SV1")               '��������
            SQL.Append(", FURI_KEN_SV1")                '�U�֌���
            'SQL.Append(", TRUNC(")
            ''***** 2009/10/22 kakiwaki *****
            ''   SQL.Append(" ((KIHTESUU_TV1 * DECODE(SEIKYU_KBN_TV1,'0',SYORI_KEN_SV1,FURI_KEN_SV1) / 100)")
            'SQL.Append(" ((KIHTESUU_TV1 * DECODE(SEIKYU_KBN_TV1,'0',SYORI_KEN_SV1,FURI_KEN_SV1))")
            ''*** 2008/05/21 nishida �C���F�Č�No.418 �Œ�萔��1/�Œ�萔��2�@=Null�̏ꍇ�̑Ή� START
            ''SQL.Append("   + KOTEI_TESUU1_T")
            ''SQL.Append("   + KOTEI_TESUU2_T)")
            'SQL.Append(" + NVL(KOTEI_TESUU1_T,0)")
            'SQL.Append(" + NVL(KOTEI_TESUU2_T,0))")
            ''*** 2008/05/21 nishida �C���F�Č�No.418 �Œ�萔��1/�Œ�萔��2�@=Null�̏ꍇ�̑Ή� END
            'SQL.Append(" * DECODE(SYOUHI_KBN_T,'0'," & sTax & ",1)) TESUU_KIN1")     ' �����萔��
            '2013/11/13 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<
            SQL.Append(", SOURYO_TV1")                       ' ����
            SQL.Append(", FURI_KIN_SV1")                     ' �U�֍ς݋��z
            'SQL.Append(", TESUU1_TV1")                       ' ��ʎ萔���P
            'SQL.Append(", TESUU2_TV1")                       ' ��ʎ萔���Q
            'SQL.Append(", TESUU3_TV1")                       ' ��ʎ萔���R
            'SQL.Append(", TESUU4_TV1")                       ' ��ʎ萔���S
            'SQL.Append(", TESUU5_TV1")                       ' ��ʎ萔���T
            'SQL.Append(", TESUU6_T TESUU6_TV1")              ' ��ʎ萔���U
            SQL.Append(", TUKEKIN_NO_TV1")                   ' ���ϋ��Z�@��
            SQL.Append(", TUKESIT_NO_TV1")                   ' ���ώx�X
            'SQL.Append(", TORIMATOME_SIT_NO_T")              ' �Ƃ�܂ƂߓX
            SQL.Append(", TORIMATOME_SIT_T")                 ' �Ƃ�܂ƂߓX
            '*** �C�� mitsu 2009/05/28 ���v�����ɑ΂���萔���Ή� ***
            SQL.Append(", TESUUMAT_PATN_T")                  ' �萔���W�v���@
            SQL.Append(", SYOUHI_KBN_T")                     ' ����ŋ敪
            SQL.Append(", KIHTESUU_TV1")                     ' �U�֎萔���P��
            SQL.Append(", SEIKYU_KBN_TV1")                   ' ���������敪
            SQL.Append(", TESUU_YDATE_SV1")                  ' �萔�������\���
            SQL.Append(", NVL(KOTEI_TESUU1_T,0) KOTEI_TESUU1_T") ' �Œ�萔���P
            SQL.Append(", NVL(KOTEI_TESUU2_T,0) KOTEI_TESUU2_T") ' �Œ�萔���Q
            SQL.Append(", TESUU_TABLE_ID_T")                '�U���萔����h�c
            '********************************************************
            SQL.Append(", BAITAI_CODE_T")   '2010/03/25 �}�̃R�[�h�ǉ�
            '2013/11/13 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
            SQL.Append(", KESSAI_YDATE_SV1")
            '2013/11/13 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<
            SQL.Append(" FROM V1_KESMAST, TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_SV1 = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_SV1 = TORIF_CODE_T")
            SQL.Append(" AND FSYORI_KBN_SV1 = FSYORI_KBN_T")
            SQL.Append(" AND FURI_DATE_SV1  = " & SQ(furiDate))       ' �U�֓�
            SQL.Append(" AND TESUUKEI_FLG_SV1 = " & SQ(reCalcFlag))   ' �萔���v�Z�σt���O ������
            SQL.Append(" AND TESUUTYO_FLG_SV1 = '0'")     ' �萔�������σt���O ������
            SQL.Append(" AND FUNOU_FLG_SV1 = '1'")        ' �s�\�σt���O �����ς�
            SQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")      ' ���f�t���O
            If OraReader.DataReader(SQL) = True Then
                Do While OraReader.EOF = False
                    nSyoriKen += 1

                    '*** �C�� mitsu 2009/05/28 ���������� ***
                    'SQL = New StringBuilder(128)
                    SQL.Length = 0
                    '****************************************

                    '*** �C�� mitsu 2009/05/28 ���v�����ɑ΂���萔�����v ***
                    If OraReader.GetString("TESUUMAT_PATN_T") = "1" Then
                        '2013/11/13 saitou �W���� ����őΉ� DEL -------------------------------------------------->>>>
                        '�ꎞ�I�ɃR�����g�A�E�g�i�v�d�l�����j
                        'CalcGoukeiTesuu(OraReader, Jikinko, sTax)
                        '2013/11/13 saitou �W���� ����őΉ� DEL --------------------------------------------------<<<<
                    Else
                        '****************************************************
                        If OraReader.GetString("TESUUTYO_KBN_T") <> "2" Then
                            ' �萔�������敪�����ʖƏ��ȊO�̏ꍇ

                            '2013/11/13 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
                            Dim strKijunDate As String = String.Empty
                            If strcINI_PARAM.ZEIKIJUN.Equals("0") = True Then
                                '�U�֓��
                                strKijunDate = OraReader.GetString("FURI_DATE_SV1")
                            Else
                                '���ϓ��
                                strKijunDate = OraReader.GetString("KESSAI_YDATE_SV1")
                            End If

                            '�ŗ��擾
                            Me.TAX.GetZeiritsu(strKijunDate)
                            If Me.TAX.ZEIRITSU.Equals("err") = True Then
                                LOG.Write("�ŗ��擾", "���s", "����F" & strKijunDate)
                                Return 1
                            End If

                            '�U���萔���}�X�^�ǂݍ���
                            If Me.GetJifuriTesuuTable(Me.TAX.ZEIRITSU_ID) = False Then
                                LOG.Write("�U���萔���}�X�^�Ǎ�", "���s", "����F" & strKijunDate)
                                Return 1
                            End If
                            '2013/11/13 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

                            '2013/12/27 saitou �W���� �󎆐őΉ� ADD -------------------------------------------------->>>>
                            '�󎆐Ŏ擾
                            Me.TAX.GetInshizei(strKijunDate)
                            If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                                LOG.Write("�󎆐Ŏ擾", "���s", "����F" & strKijunDate)
                                Return 1
                            End If
                            '2013/12/27 saitou �W���� �󎆐őΉ� ADD --------------------------------------------------<<<<

                            '2013/11/13 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
                            '�����萔���͕ʓr�v�Z����
                            If Me.CalcTesuuKin1(TesuuKin1, OraReader) = False Then
                                Return 1
                            End If
                            'TesuuKin1 = OraReader.GetInt64("TESUU_KIN1")        ' �����萔��
                            '2013/11/13 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<
                            TesuuKin2 = OraReader.GetInt64("SOURYO_TV1")        ' ����
                            JifuriKin = OraReader.GetInt64("FURI_KIN_SV1") - (TesuuKin1 + TesuuKin2)
                            TesuuKin3 = 0

                            '�U���萔����h�c��ݒ�
                            If OraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                                intKIJYUN_ID = -1
                            Else
                                intKIJYUN_ID = CInt(OraReader.GetString("TESUU_TABLE_ID_T"))
                                If Me.TESUU_TABLE_DATA(intKIJYUN_ID).strKIJYUN_ID_CODE = String.Empty Then
                                    LOG.Write("�U���萔���}�X�^�擾", "���s", "����F" & strKijunDate & " �萔��ID�F" & intKIJYUN_ID.ToString)
                                    Return 1
                                End If
                            End If

                            '2014/03/04 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
                            If intKIJYUN_ID = -1 Then
                                TesuuKin3 = 0
                            Else
                                If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                                    ' ���ϋ��Z�@�ւ��C�����ɂ̏ꍇ
                                    If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                                        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�C���X��
                                        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN
                                        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN
                                        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN
                                        End If
                                    Else
                                        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�C�{�x�X
                                        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN
                                        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN
                                        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN
                                        End If
                                    End If
                                Else
                                    ' ���s
                                    If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                                        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU
                                    ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                                        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU
                                    ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                                        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU
                                    End If
                                End If
                            End If

                            ''2013/12/27 saitou �W���� �󎆐őΉ� UPD -------------------------------------------------->>>>
                            'If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                            '    ' ���ϋ��Z�@�ւ��C�����ɂ̏ꍇ
                            '    If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                            '        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�C���X��
                            '        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            '            '    �O�~���傫�� ���� ���~����
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN
                            '        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            '            '    ���~�ȏ� ���� ���~����
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN
                            '        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            '            '    ���~�ȏ�
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN
                            '        End If
                            '    Else
                            '        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�C�{�x�X
                            '        If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            '            '    �O�~���傫�� ���� ���~����
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN
                            '        ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            '            '    ���~�ȏ� ���� ���~����
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN
                            '        ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            '            '    ���~�ȏ�
                            '            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN
                            '        End If
                            '    End If
                            'Else
                            '    ' ���s
                            '    If 0 < JifuriKin And JifuriKin < Me.TAX.INSHIZEI1 Then
                            '        '    �O�~���傫�� ���� ���~����
                            '        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU
                            '    ElseIf Me.TAX.INSHIZEI1 <= JifuriKin And JifuriKin < Me.TAX.INSHIZEI2 Then
                            '        '    ���~�ȏ� ���� ���~����
                            '        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU
                            '    ElseIf Me.TAX.INSHIZEI2 <= JifuriKin Then
                            '        '    ���~�ȏ�
                            '        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU
                            '    End If
                            'End If

                            ''If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                            ''    ' ���ϋ��Z�@�ւ��C�����ɂ̏ꍇ
                            ''    If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                            ''        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�C���X��
                            ''        If 0 < JifuriKin And JifuriKin < 10000 Then
                            ''            '    �O�~���傫�� ���� �P���~����
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN
                            ''        ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                            ''            '    �P���~�ȏ� ���� �R���~����
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN
                            ''        ElseIf 30000 <= JifuriKin Then
                            ''            '    �R���~�ȏ�
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN
                            ''        End If
                            ''    Else
                            ''        ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�C�{�x�X
                            ''        If 0 < JifuriKin And JifuriKin < 10000 Then
                            ''            '    �O�~���傫�� ���� �P���~����
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN
                            ''        ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                            ''            '    �P���~�ȏ� ���� �R���~����
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN
                            ''        ElseIf 30000 <= JifuriKin Then
                            ''            '    �R���~�ȏ�
                            ''            TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN
                            ''        End If
                            ''    End If
                            ''Else
                            ''    ' ���s
                            ''    If 0 < JifuriKin And JifuriKin < 10000 Then
                            ''        '    �O�~���傫�� ���� �P���~����
                            ''        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU
                            ''    ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                            ''        '    �P���~�ȏ� ���� �R���~����
                            ''        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU
                            ''    ElseIf 30000 <= JifuriKin Then
                            ''        '    �R���~�ȏ�
                            ''        TesuuKin3 = TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU
                            ''    End If
                            ''End If
                            ''2013/12/27 saitou �W���� �󎆐őΉ� UPD --------------------------------------------------<<<<
                            '2014/03/04 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<

                        Else
                            ' �Ə�
                            '*** �C���@nishida 2008/05/20
                            TesuuKin1 = 0
                            TesuuKin2 = 0
                            '*** �C���@nishida 2008/05/20
                            TesuuKin3 = 0
                        End If

                        ' �X�P�W���[���}�X�^�X�V
                        SQL.Append("UPDATE SCHMAST SET")
                        SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
                        SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
                        SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
                        SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
                        SQL.Append(", TESUUKEI_FLG_S = '1'")
                        SQL.Append(" WHERE TORIS_CODE_S = " & SQ(OraReader.GetString("TORIS_CODE_SV1")))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(OraReader.GetString("TORIF_CODE_SV1")))
                        SQL.Append(" AND FURI_DATE_S = " & SQ(OraReader.GetString("FURI_DATE_SV1")))
                        Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                        LOG.ToriCode = OraReader.GetString("TORIS_CODE_SV1") & OraReader.GetString("TORIF_CODE_SV1")

                        '2010/03/25 �w�Z�X�P�W���[���}�X�^�X�V
                        If OraReader.GetString("BAITAI_CODE_T").Trim = "07" Then
                            SQL = New StringBuilder
                            SQL.Append("UPDATE G_SCHMAST SET")
                            SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
                            SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
                            SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
                            SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
                            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(OraReader.GetString("TORIS_CODE_SV1")))
                            Select Case OraReader.GetString("TORIF_CODE_SV1").Trim
                                Case "01"
                                    SQL.Append(" AND FURI_KBN_S = '0'")
                                Case "02"
                                    SQL.Append(" AND FURI_KBN_S = '1'")
                                Case "03"
                                    SQL.Append(" AND FURI_KBN_S = '2'")
                                Case "04"
                                    SQL.Append(" AND FURI_KBN_S = '3'")
                                Case Else
                                    SQL.Append(" AND FURI_KBN_S = '0'")
                            End Select
                            SQL.Append(" AND FURI_DATE_S = " & SQ(OraReader.GetString("FURI_DATE_SV1")))
                            nRet = MainDB.ExecuteNonQuery(SQL)
                        End If
                        '=====================================
                        '*** �C�� mitsu 2009/05/28 ���v�����ɑ΂���萔�����v ***
                    End If
                    '************************************************************

                    OraReader.NextRead()
                Loop
            End If
            OraReader.Close()

            LOG.Write("�萔���v�Z", "����", "�����F" & nSyoriKen.ToString)

        Catch ex As Exception
            '*** �C�� mitsu 2009/07/29 ��Q�Ή� �p�g���C�g�_�� ***
            Try
                Dim ELog As New CASTCommon.ClsEventLOG
                If LOG.ToriCode Is Nothing OrElse LOG.ToriCode = "" Then
                    ELog.Write("�萔���v�Z�Ɉُ픭���F" & ex.Message, EventLogEntryType.Error)
                Else
                    ELog.Write("�萔���v�Z�Ɉُ픭���F�����R�[�h�F" & LOG.ToriCode & " " & ex.Message, EventLogEntryType.Error)
                End If
            Catch
            End Try
            '*****************************************************
            LOG.Write("�萔���v�Z", "���s", ex.Message)


            OraReader.Close()

            Return 1

        '*** Str Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        '*** End Add 2015/12/01 SO)�r�� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        End Try

        Return 0
    End Function

#Region "���v�萔���ɑ΂���萔���v�Z"
    '*** �C�� mitsu 2009/05/28 ���v�����ɑ΂���萔���Ή� ***
    Private Function CalcGoukeiTesuu(ByRef OraReader As CASTCommon.MyOracleReader, ByVal Jikinko As String, ByVal sTax As String) As Integer
        Dim SchReader As CASTCommon.MyOracleReader
        Dim SQL As StringBuilder = New StringBuilder(128)
        Dim nRet As Integer = 0

        SchReader = New CASTCommon.MyOracleReader

        Try
            '�����}�X�^���擾
            Dim TorisCode As String = OraReader.GetString("TORIS_CODE_SV1")
            Dim TorifCode As String = OraReader.GetString("TORIF_CODE_SV1")
            Dim FuriDate As String = OraReader.GetString("FURI_DATE_SV1")
            Dim SyouhiKbn As String = OraReader.GetString("SYOUHI_KBN_T")
            Dim KihonTesuu As Integer = OraReader.GetInt("KIHTESUU_TV1")
            Dim SeikyuKbn As String = OraReader.GetString("SEIKYU_KBN_TV1")
            Dim TesuuYdate As String = OraReader.GetString("TESUU_YDATE_SV1")
            Dim KoteiTesuu1 As Integer = OraReader.GetInt("KOTEI_TESUU1_T")
            Dim KoteiTesuu2 As Integer = OraReader.GetInt("KOTEI_TESUU2_T")

            '�萔���v�Z�p
            Dim TesuuKin1 As Long = 0
            Dim TesuuKin2 As Integer = OraReader.GetInt("SOURYO_TV1")
            Dim TesuuKin3 As Integer = 0
            Dim TesuuKen As Long = 0            '��{�萔���Ώی���
            Dim SchKen As Integer = 0           '�X�P�W���[������

            Dim intKIJYUN_ID As Integer = 0     '�U���萔����h�c

            If OraReader.GetString("TESUUTYO_KBN_T") <> "2" Then
                ' �萔�������敪�����ʖƏ��ȊO�̏ꍇ
                SchReader = New CASTCommon.MyOracleReader(MainDB)

                '����萔���������̃X�P�W���[���̎萔�������Z�b�g����
                SQL.Append("UPDATE SCHMAST SET")
                SQL.Append(" TESUU_KIN_S = 0")
                SQL.Append(", TESUU_KIN1_S = 0")
                SQL.Append(", TESUU_KIN2_S = 0")
                SQL.Append(", TESUU_KIN3_S = 0")
                SQL.Append(" WHERE TORIS_CODE_S = " & SQ(TorisCode))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(TorifCode))
                SQL.Append(" AND TESUU_YDATE_S = " & SQ(TesuuYdate))
                SQL.Append(" AND TESUUTYO_FLG_S = '0'")
                SQL.Append(" AND FUNOU_FLG_S = '1'")
                SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                MainDB.ExecuteNonQuery(SQL)

                '�e�X�P�W���[���̏��擾�ƐU���萔���v�Z
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append(" FURI_DATE_SV1")
                Select Case SeikyuKbn
                    Case "0" : SQL.Append(", SYORI_KEN_SV1 TESUU_KEN")
                    Case Else : SQL.Append(", FURI_KEN_SV1 TESUU_KEN")
                End Select
                SQL.Append(", FURI_KIN_SV1")
                SQL.Append(" FROM V1_KESMAST")
                SQL.Append(" WHERE TORIS_CODE_SV1 = " & SQ(TorisCode))
                SQL.Append(" AND TORIF_CODE_SV1 = " & SQ(TorifCode))
                SQL.Append(" AND TESUU_YDATE_SV1  = " & SQ(TesuuYdate))
                SQL.Append(" AND TESUUTYO_FLG_SV1 = '0'")
                SQL.Append(" AND FUNOU_FLG_SV1 = '1'")
                SQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")
                SQL.Append(" ORDER BY FURI_DATE_SV1")

                If SchReader.DataReader(SQL) = True Then
                    While SchReader.EOF = False
                        TesuuKen += SchReader.GetInt64("TESUU_KEN")
                        SchKen += 1

                        '�U���萔�������߂邽�߂̎��U�萔���v�Z
                        '       '*** �C�� mitsu 2009/06/09 �����_�ȉ��͐؂�̂� ***
                        '       'TesuuKin1 = CLng(KihonTesuu * SchReader.GetInt64("TESUU_KEN") / 100) + KoteiTesuu1 + KoteiTesuu2
                        '   TesuuKin1 = CLng(Math.Floor(KihonTesuu * SchReader.GetInt64("TESUU_KEN") / 100)) + KoteiTesuu1 + KoteiTesuu2
                        '       '**************************************************
                        TesuuKin1 = CLng(Math.Floor(KihonTesuu * SchReader.GetInt64("TESUU_KEN"))) + KoteiTesuu1 + KoteiTesuu2

                        Dim JifuriKin As Long = SchReader.GetInt64("FURI_KIN_SV1") - (TesuuKin1 + TesuuKin2)

                        '�U���萔����h�c��ݒ�
                        If OraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                            intKIJYUN_ID = 0
                        Else
                            intKIJYUN_ID = CInt(OraReader.GetString("TESUU_TABLE_ID_T"))
                        End If

                        '�U���萔���v�Z
                        If OraReader.GetString("TUKEKIN_NO_TV1") = Jikinko Then
                            ' ���ϋ��Z�@�ւ��C�����ɂ̏ꍇ
                            If OraReader.GetString("TUKESIT_NO_TV1") = OraReader.GetString("TORIMATOME_SIT_T") Then
                                ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�C���X��
                                If 0 < JifuriKin And JifuriKin < 10000 Then
                                    '    �O�~���傫�� ���� �P���~����
                                    TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_JITEN)
                                ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                                    '    �P���~�ȏ� ���� �R���~����
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_JITEN)
                                ElseIf 30000 <= JifuriKin Then
                                    ' �R���~�ȏ�
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_JITEN)
                                End If
                            Else
                                ' ���ώx�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�C�{�x�X
                                If 0 < JifuriKin And JifuriKin < 10000 Then
                                    '    �O�~���傫�� ���� �P���~����
                                    TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_HONSITEN)
                                ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                                    '    �P���~�ȏ� ���� �R���~����
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_HONSITEN)
                                ElseIf 30000 <= JifuriKin Then
                                    ' �R���~�ȏ�
                                    TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_HONSITEN)
                                End If
                            End If
                        Else
                            ' ���s
                            If 0 < JifuriKin And JifuriKin < 10000 Then
                                '    �O�~���傫�� ���� �P���~����
                                TesuuKin3 = CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng10000UNDER_TAKOU)
                            ElseIf 10000 <= JifuriKin And JifuriKin < 30000 Then
                                ' �O�~���傫�� ���� �R���~����
                                TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000UNDER_TAKOU)
                            ElseIf 30000 <= JifuriKin Then
                                ' �R���~�ȏ�
                                TesuuKin3 += CInt(TESUU_TABLE_DATA(intKIJYUN_ID).lng30000OVER_TAKOU)
                            End If
                        End If

                        SchReader.NextRead()
                    End While
                End If

                '����ŋ敪���
                If SyouhiKbn <> "0" Then
                    sTax = "1"
                End If

                '�����萔���v�Z
                '*** �C�� mitsu 2009/06/09 �����_�ȉ��͐؂�̂� ***
                'TesuuKin1 = CLng((KihonTesuu * TesuuKen / 100 + SchKen * (KoteiTesuu1 + KoteiTesuu2)) * Double.Parse(sTax))
                TesuuKin1 = CLng(Math.Floor((KihonTesuu * TesuuKen / 100 + SchKen * (KoteiTesuu1 + KoteiTesuu2)) * Double.Parse(sTax)))
                '**************************************************
                TesuuKin2 = SchKen * TesuuKin2
            Else
                TesuuKin1 = 0
                TesuuKin2 = 0
                TesuuKin3 = 0
            End If

            ' �X�P�W���[���}�X�^�X�V
            SQL.Length = 0
            SQL.Append("UPDATE SCHMAST SET")
            SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
            SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
            SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
            SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
            SQL.Append(", TESUUKEI_FLG_S = '1'")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(TorisCode))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(TorifCode))
            SQL.Append(" AND FURI_DATE_S  = " & SQ(FuriDate))
            nRet = MainDB.ExecuteNonQuery(SQL)
            LOG.ToriCode = TorisCode & TorifCode
            '2010/03/25 �w�Z�X�P�W���[���}�X�^�X�V
            If OraReader.GetString("BAITAI_CODE_T").Trim = "07" Then
                SQL = New StringBuilder
                SQL.Append("UPDATE G_SCHMAST SET")
                SQL.Append(" TESUU_KIN1_S = " & TesuuKin1.ToString)
                SQL.Append(", TESUU_KIN2_S = " & TesuuKin2.ToString)
                SQL.Append(", TESUU_KIN3_S = " & TesuuKin3.ToString)
                SQL.Append(", TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(TorisCode))
                Select Case TorifCode.Trim
                    Case "01"
                        SQL.Append(" AND FURI_KBN_S = '0'")
                    Case "02"
                        SQL.Append(" AND FURI_KBN_S = '1'")
                    Case "03"
                        SQL.Append(" AND FURI_KBN_S = '2'")
                    Case "04"
                        SQL.Append(" AND FURI_KBN_S = '3'")
                    Case Else
                        SQL.Append(" AND FURI_KBN_S = '0'")
                End Select
                SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
                nRet = MainDB.ExecuteNonQuery(SQL)
            End If
            '====================================
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            '*** �C�� mitsu 2009/06/09 Nothing�̏ꍇ���l�� ***
            If Not SchReader Is Nothing Then
                SchReader.Close()
            End If
            '*************************************************
        End Try

        Return nRet
    End Function
    '********************************************************
#End Region

    ' �@�\�@ �F �X�P�W���[���}�X�^�C�ρE�s�\�������z�Čv�Z
    '
    ' �����@ �F ARG1 - �U�֓�
    '
    ' ���l�@ �F 
    '
    Private Sub KekkaReBorn(ByVal furiDate As String)
        Dim SQL As New StringBuilder(256)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim ToriSCode As String
        Dim ToriFCode As String
        Dim MotikomiKubun As String
        Dim UpdateFlg As Boolean = False

        Try
            LOG.Write("�萔���v�Z", "����", "�萔���Čv�Z(�J�n) �U�֓�:" & furiDate)

            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(", TORIF_CODE_T")
            SQL.Append(", FMT_KBN_T")
            SQL.Append(", MOTIKOMI_KBN_T")
            SQL.Append(", BAITAI_CODE_T")  '2010/03/25 �}�̃R�[�h�ǉ�
            SQL.Append(" FROM SCHMAST")
            SQL.Append(", TORIMAST")
            SQL.Append(" WHERE FURI_DATE_S = '" & furiDate & "'")
            SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND FSYORI_KBN_T = '1'")
            SQL.Append(" AND KESSAI_FLG_S = '0'")   '2010/03/25 HENKAN_FLG_S��KESSAI_FLG_S
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            If OraReader.DataReader(SQL) = True Then
                While (OraReader.EOF = False)
                    ToriSCode = OraReader.GetString("TORIS_CODE_T")
                    ToriFCode = OraReader.GetString("TORIF_CODE_T")
                    MotikomiKubun = OraReader.GetString("MOTIKOMI_KBN_T")

                    Select Case strcINI_PARAM.CENTER_CODE
                        Case "4"
                            UpdateFlg = True
                        Case Else
                            If MotikomiKubun = "0" Then
                                UpdateFlg = True
                            Else
                                UpdateFlg = False
                            End If
                    End Select

                    If UpdateFlg = True Then
                        Dim FunoKensu As Decimal = 0
                        Dim FunoKingaku As Decimal = 0
                        Dim ZumiKensu As Decimal = 0
                        Dim ZumiKingaku As Decimal = 0

                        '----------------------
                        '�s�\�����A���z�擾
                        '----------------------
                        Dim FunoReader As New CASTCommon.MyOracleReader(MainDB)
                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT")
                        SQL.Append(" COUNT(FURIKIN_K) KEN")
                        SQL.Append(", SUM(FURIKIN_K) KIN")
                        SQL.Append(" FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = '" & furiDate & "'")
                        If OraReader.GetItem("FMT_KBN_T") = "02" Then
                            ' ���ł́A�f�[�^�敪���R�̃��R�[�h
                            SQL.Append(" AND DATA_KBN_K = '3'")
                        Else
                            SQL.Append(" AND DATA_KBN_K = '2'")
                        End If
                        SQL.Append(" AND FURIKETU_CODE_K <> '0'")
                        ' 2008.03.14 �U�֋��z���O�~�̂��̂͊܂܂Ȃ�
                        SQL.Append(" AND FURIKIN_K > 0")
                        SQL.Append(" AND TORIS_CODE_K = '" & ToriSCode & "'")
                        SQL.Append(" AND TORIF_CODE_K = '" & ToriFCode & "'")
                        If FunoReader.DataReader(SQL) = True Then
                            FunoKensu = FunoReader.GetInt64("KEN")
                            FunoKingaku = FunoReader.GetInt64("KIN")
                        End If
                        FunoReader.Close()

                        '----------------------
                        '�U�֍ό����A���z�擾
                        '----------------------
                        Dim ZumiReader As New CASTCommon.MyOracleReader(MainDB)
                        If ZumiReader.DataReader(SQL.Replace("FURIKETU_CODE_K <> '0'", "FURIKETU_CODE_K = '0'")) = True Then
                            ZumiKensu = ZumiReader.GetInt64("KEN")
                            ZumiKingaku = ZumiReader.GetInt64("KIN")
                        End If
                        ZumiReader.Close()

                        '-------------------------------------------
                        '�X�P�W���[���}�X�^�̍X�V
                        '-------------------------------------------
                        SQL = New StringBuilder(256)
                        SQL.Append("UPDATE SCHMAST SET")
                        SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                        SQL.Append(", FURI_KIN_S = " & ZumiKingaku.ToString)
                        SQL.Append(", FUNOU_KEN_S = " & FunoKensu.ToString)
                        SQL.Append(", FUNOU_KIN_S =" & FunoKingaku.ToString)
                        SQL.Append(" WHERE TORIS_CODE_S = '" & ToriSCode & "'")
                        SQL.Append(" AND TORIF_CODE_S = '" & ToriFCode & "'")
                        SQL.Append(" AND FURI_DATE_S = '" & furiDate & "'")
                        Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                        LOG.ToriCode = ToriSCode & ToriFCode
                        LOG.FuriDate = furiDate
                        '2010/03/25 �w�Z�X�P�W���[���}�X�^�X�V
                        If OraReader.GetString("BAITAI_CODE_T").Trim = "07" Then
                            '2017/03/14 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
                            '���ʐU�֓��Ή�
                            Dim OraGakReader As CASTCommon.MyOracleReader = Nothing

                            Try
                                '--------------------------------------------------
                                '�w�Z�X�P�W���[���}�X�^�̃��R�[�h�������o
                                '--------------------------------------------------
                                Dim MultiGScheduleFlg As Boolean = True

                                With SQL
                                    .Length = 0
                                    .Append("select count(*) as COUNTER from G_SCHMAST")
                                    .Append(" where GAKKOU_CODE_S = " & SQ(ToriSCode))
                                    Select Case ToriFCode.Trim
                                        Case "01" : .Append(" and FURI_KBN_S = '0'")
                                        Case "02" : .Append(" and FURI_KBN_S = '1'")
                                        Case "03" : .Append(" and FURI_KBN_S = '2'")
                                        Case "04" : .Append(" and FURI_KBN_S = '3'")
                                        Case Else : .Append(" and FURI_KBN_S = '0'")
                                    End Select
                                    .Append(" and FURI_DATE_S = " & SQ(furiDate))
                                End With

                                OraGakReader = New CASTCommon.MyOracleReader(MainDB)
                                If OraGakReader.DataReader(SQL) = True Then
                                    '�X�P�W���[����1���R�[�h
                                    If OraGakReader.GetInt("COUNTER") = 1 Then
                                        MultiGScheduleFlg = False
                                    End If
                                End If

                                OraGakReader.Close()

                                If MultiGScheduleFlg = False Then
                                    SQL = New StringBuilder
                                    SQL.Append("UPDATE G_SCHMAST SET")
                                    SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                                    SQL.Append(", FURI_KIN_S = " & ZumiKingaku.ToString)
                                    SQL.Append(", FUNOU_KEN_S = " & FunoKensu.ToString)
                                    SQL.Append(", FUNOU_KIN_S =" & FunoKingaku.ToString)
                                    SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(ToriSCode))
                                    Select Case ToriFCode.Trim
                                        Case "01"
                                            SQL.Append(" AND FURI_KBN_S = '0'")
                                        Case "02"
                                            SQL.Append(" AND FURI_KBN_S = '1'")
                                        Case "03"
                                            SQL.Append(" AND FURI_KBN_S = '2'")
                                        Case "04"
                                            SQL.Append(" AND FURI_KBN_S = '3'")
                                        Case Else
                                            SQL.Append(" AND FURI_KBN_S = '0'")
                                    End Select
                                    SQL.Append(" AND FURI_DATE_S = " & SQ(furiDate))
                                    nRet = MainDB.ExecuteNonQuery(SQL)

                                Else
                                    '�X�V����������
                                    nRet = 0

                                    '--------------------------------------------------
                                    '�w�Z�X�P�W���[���}�X�^���o
                                    '--------------------------------------------------
                                    With SQL
                                        .Length = 0
                                        .Append("select ")
                                        .Append(" GAKUNEN1_FLG_S")
                                        .Append(",GAKUNEN2_FLG_S")
                                        .Append(",GAKUNEN3_FLG_S")
                                        .Append(",GAKUNEN4_FLG_S")
                                        .Append(",GAKUNEN5_FLG_S")
                                        .Append(",GAKUNEN6_FLG_S")
                                        .Append(",GAKUNEN7_FLG_S")
                                        .Append(",GAKUNEN8_FLG_S")
                                        .Append(",GAKUNEN9_FLG_S")
                                        .Append(" from G_SCHMAST")
                                        .Append(" where GAKKOU_CODE_S = " & SQ(ToriSCode))
                                        Select Case ToriFCode.Trim
                                            Case "01" : .Append(" and FURI_KBN_S = '0'")
                                            Case "02" : .Append(" and FURI_KBN_S = '1'")
                                            Case "03" : .Append(" and FURI_KBN_S = '2'")
                                            Case "04" : .Append(" and FURI_KBN_S = '3'")
                                            Case Else : .Append(" and FURI_KBN_S = '0'")
                                        End Select
                                        .Append(" and FURI_DATE_S = " & SQ(furiDate))
                                    End With

                                    If OraGakReader.DataReader(SQL) = True Then
                                        While OraGakReader.EOF = False

                                            ZumiKensu = 0
                                            ZumiKingaku = 0
                                            FunoKensu = 0
                                            FunoKingaku = 0

                                            '--------------------------------------------------
                                            '�w�N�t���O�������Ă��閾�ׂɑ΂��ďW�v���s��
                                            '--------------------------------------------------
                                            For i As Integer = 1 To 9
                                                If OraGakReader.GetString("GAKUNEN" & i.ToString & "_FLG_S") = "1" Then
                                                    With SQL
                                                        .Length = 0
                                                        .Append("select ")
                                                        .Append(" sum(decode(FURIKETU_CODE_M, 0, 1, 0)) as FURI_KEN")
                                                        .Append(",sum(decode(FURIKETU_CODE_M, 0, SEIKYU_KIN_M)) as FURI_KIN")
                                                        .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, 1)) as FUNO_KEN")
                                                        .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, SEIKYU_KIN_M)) as FUNO_KIN")
                                                        .Append(" from G_MEIMAST")
                                                        .Append(" where GAKKOU_CODE_M = " & SQ(ToriSCode))
                                                        Select Case ToriFCode.Trim
                                                            Case "01" : .Append(" and FURI_KBN_M = '0'")
                                                            Case "02" : .Append(" and FURI_KBN_M = '1'")
                                                            Case "03" : .Append(" and FURI_KBN_M = '2'")
                                                            Case "04" : .Append(" and FURI_KBN_M = '3'")
                                                            Case Else : .Append(" and FURI_KBN_M = '0'")
                                                        End Select
                                                        .Append(" and FURI_DATE_M = " & SQ(furiDate))
                                                        .Append(" and GAKUNEN_CODE_M = " & i.ToString)
                                                    End With

                                                    '�s�\�I���N�����[�_�[�ė��p
                                                    FunoReader.Close()
                                                    If FunoReader.DataReader(SQL) = True Then
                                                        ZumiKensu += FunoReader.GetInt("FURI_KEN")
                                                        ZumiKingaku += FunoReader.GetInt64("FURI_KIN")
                                                        FunoKensu += FunoReader.GetInt("FUNO_KEN")
                                                        FunoKingaku += FunoReader.GetInt64("FUNO_KIN")
                                                    End If
                                                End If
                                            Next

                                            '--------------------------------------------------
                                            '�w�Z�X�P�W���[���}�X�^�X�V
                                            '--------------------------------------------------
                                            With SQL
                                                .Length = 0
                                                .Append("update G_SCHMAST set")
                                                .Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                                                .Append(",FURI_KIN_S = " & ZumiKingaku.ToString)
                                                .Append(",FUNOU_KEN_S = " & FunoKensu.ToString)
                                                .Append(",FUNOU_KIN_S = " & FunoKingaku.ToString)
                                                .Append(" where GAKKOU_CODE_S = " & SQ(ToriSCode))
                                                Select Case ToriFCode.Trim
                                                    Case "01" : .Append(" and FURI_KBN_S = '0'")
                                                    Case "02" : .Append(" and FURI_KBN_S = '1'")
                                                    Case "03" : .Append(" and FURI_KBN_S = '2'")
                                                    Case "04" : .Append(" and FURI_KBN_S = '3'")
                                                    Case Else : .Append(" and FURI_KBN_S = '0'")
                                                End Select
                                                .Append(" and FURI_DATE_S = " & SQ(furiDate))
                                                .Append(" and GAKUNEN1_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN1_FLG_S")))
                                                .Append(" and GAKUNEN2_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN2_FLG_S")))
                                                .Append(" and GAKUNEN3_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN3_FLG_S")))
                                                .Append(" and GAKUNEN4_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN4_FLG_S")))
                                                .Append(" and GAKUNEN5_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN5_FLG_S")))
                                                .Append(" and GAKUNEN6_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN6_FLG_S")))
                                                .Append(" and GAKUNEN7_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN7_FLG_S")))
                                                .Append(" and GAKUNEN8_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN8_FLG_S")))
                                                .Append(" and GAKUNEN9_FLG_S = " & SQ(OraGakReader.GetString("GAKUNEN9_FLG_S")))
                                            End With

                                            nRet += MainDB.ExecuteNonQuery(SQL)

                                            OraGakReader.NextRead()
                                        End While
                                    End If
                                End If

                            Catch ex As Exception
                                LOG.Write("�萔���v�Z", "���s", "�萔���Čv�Z �U�֓�:" & furiDate & "�Aerr = " & ex.Message)

                            Finally
                                If Not OraGakReader Is Nothing Then
                                    OraGakReader.Close()
                                    OraGakReader = Nothing
                                End If
                            End Try

                            'SQL = New StringBuilder
                            'SQL.Append("UPDATE G_SCHMAST SET")
                            'SQL.Append(" FURI_KEN_S = " & ZumiKensu.ToString)
                            'SQL.Append(", FURI_KIN_S = " & ZumiKingaku.ToString)
                            'SQL.Append(", FUNOU_KEN_S = " & FunoKensu.ToString)
                            'SQL.Append(", FUNOU_KIN_S =" & FunoKingaku.ToString)
                            'SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(ToriSCode))
                            'Select Case ToriFCode.Trim
                            '    Case "01"
                            '        SQL.Append(" AND FURI_KBN_S = '0'")
                            '    Case "02"
                            '        SQL.Append(" AND FURI_KBN_S = '1'")
                            '    Case "03"
                            '        SQL.Append(" AND FURI_KBN_S = '2'")
                            '    Case "04"
                            '        SQL.Append(" AND FURI_KBN_S = '3'")
                            '    Case Else
                            '        SQL.Append(" AND FURI_KBN_S = '0'")
                            'End Select
                            'SQL.Append(" AND FURI_DATE_S = " & SQ(furiDate))
                            'nRet = MainDB.ExecuteNonQuery(SQL)
                            '2017/03/14 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END
                        End If
                        '====================================
                        LOG.Write("�X�P�W���[���}�X�^ ��/�s�\ �������z�ďW�v", "����", "�����F" & nRet.ToString)

                    End If
                    OraReader.NextRead()
                End While
            Else
                LOG.Write("�X�P�W���[���}�X�^ ��/�s�\ �������z�ďW�v", "����", "�O��")
            End If

            OraReader.Close()
            LOG.Write("�萔���v�Z", "����", "�萔���Čv�Z(�I��) �U�֓�:" & furiDate)
        Catch ex As Exception
            LOG.Write("�萔���v�Z", "���s", "�萔���Čv�Z �U�֓�:" & furiDate & "�Aerr = " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' INI�t�@�C����ǂݍ��݂܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function ReadIni() As Boolean
        Try
            strcINI_PARAM.LOG_FOLDER = CASTCommon.GetFSKJIni("COMMON", "LOG")
            If strcINI_PARAM.LOG_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - LOG)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(COMMON - LOG)")
                Return False
            End If

            strcINI_PARAM.ZEI_RITU = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
            If strcINI_PARAM.ZEI_RITU = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - ZEIRITU)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(COMMON - ZEIRITU)")
                Return False
            End If

            strcINI_PARAM.EXE_FOLDER = CASTCommon.GetFSKJIni("COMMON", "EXE")
            If strcINI_PARAM.EXE_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - EXE)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(COMMON - EXE)")
                Return False
            End If

            strcINI_PARAM.LST_FOLDER = CASTCommon.GetFSKJIni("COMMON", "LST")
            If strcINI_PARAM.LST_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - LST)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(COMMON - LST)")
                Return False
            End If

            strcINI_PARAM.TXT_FOLDER = CASTCommon.GetFSKJIni("COMMON", "TXT")
            If strcINI_PARAM.TXT_FOLDER = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - TXT)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(COMMON - TXT)")
                Return False
            End If

            strcINI_PARAM.KINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If strcINI_PARAM.KINKO_CODE = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - KINKOCD)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(COMMON - KINKOCD)")
                Return False
            End If

            strcINI_PARAM.CENTER_CODE = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If strcINI_PARAM.CENTER_CODE = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(COMMON - CENTER)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(COMMON - CENTER)")
                Return False
            End If

            '2013/11/13 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
            strcINI_PARAM.ZEIKIJUN = CASTCommon.GetFSKJIni("JIFURI", "ZEIKIJUN")
            If strcINI_PARAM.ZEIKIJUN = "err" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "���ݒ�t�@�C��", "���s", "�C�j�V�����t�@�C������̎擾���s(JIFURI - ZEIKIJUN)")
                LOG.UpdateJOBMASTbyErr("�C�j�V�����t�@�C������̎擾���s(JIFURI - ZEIKIJUN)")
                Return False
            End If
            '2013/11/13 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

            Return True

        Catch ex As Exception
            LOG.Write("�C�j�V�����t�@�C���ǂݍ���", "���s", ex.Message)

            Return False
        End Try
    End Function

    ''' <summary>
    ''' �����萔�����v�Z���܂��B
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <returns>�����萔��</returns>
    ''' <remarks>2013/11/13 �W���� ����őΉ�</remarks>
    Private Function CalcTesuuKin1(ByRef TesuuKin1 As Long, _
                                   ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Try
            '-------------------------------------------------------
            '�����萔���̌v�Z
            '-------------------------------------------------------
            Dim intKen As Integer = 0
            Select Case oraReader.GetString("SEIKYU_KBN_TV1")
                Case "0" : intKen = oraReader.GetInt("SYORI_KEN_SV1")
                Case "1" : intKen = oraReader.GetInt("FURI_KEN_SV1")
                Case Else   '�����敪�����ɂ���ΐݒ�
            End Select

            Select Case oraReader.GetString("SYOUHI_KBN_T")
                Case "0" : TesuuKin1 = CLng(Math.Floor((oraReader.GetInt("KIHTESUU_TV1") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")) * Double.Parse(Me.TAX.ZEIRITSU)))
                Case "1" : TesuuKin1 = oraReader.GetInt("KIHTESUU_TV1") * intKen + oraReader.GetInt("KOTEI_TESUU1_T") + oraReader.GetInt("KOTEI_TESUU2_T")
                Case Else
            End Select

            Return True

        Catch ex As Exception
            LOG.Write("�����萔���v�Z", "���s", ex.Message)

            Return False
        End Try

    End Function

    ''' <summary>
    ''' �U���萔���}�X�^��ǂݍ��݂܂��B
    ''' </summary>
    ''' <param name="TAX_ID">�ŗ�ID</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/27 �W���� ����őΉ�</remarks>
    Private Function GetJifuriTesuuTable(ByVal TAX_ID As String) As Boolean

        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            For i As Integer = 0 To Me.TESUU_TABLE_DATA.Length - 1
                Me.TESUU_TABLE_DATA(i).Init()
            Next

            With SQL
                .Append("select * from TESUUMAST")
                .Append(" where TAX_ID_C = " & SQ(TAX_ID))
                .Append(" and FSYORI_KBN_C = '1'")
                .Append(" and SYUBETU_C = '91'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- START
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
                    ' 2016/09/01 �^�X�N�j���� CHG �yPG�z(�ѓc�M�� �J�X�^�}�C�Y�O�Ή�(��K�̓}�X�^����)) -------------------- END

                    oraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            LOG.Write("�U���萔���}�X�^�ǂݍ���", "���s", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

End Class
