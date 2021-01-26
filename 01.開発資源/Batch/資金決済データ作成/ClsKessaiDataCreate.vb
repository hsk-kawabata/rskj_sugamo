Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections
Imports System.Diagnostics
Imports CASTCommon
Imports Microsoft.VisualBasic


'�������σf�[�^�쐬����
Public Class ClsKessaiDataCreate

    Public MainLOG As New CASTCommon.BatchLOG("KFK010", "�������σ��G���^�쐬")

    Dim MainDB As CASTCommon.MyOracle

    Private strKEKKA As String              ' �f�[�^�쐬����

    Private jobMessage As String = ""          ' �W���u�Ď����b�Z�[�W
    Private CntDenbun As Integer = 0        '2010/02/03 �ǉ� �d������
    ' �������t
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
    Private KessaiKamokuCode_ETC As String = "xx"   '���ωȖڂ́u���̑��v�̐M���ȖڃR�[�h
    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END

    ''' <summary>
    ''' ini�t�@�C�����
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni
        Dim KAWASE_CENTER As String         ' ���M�Z���^��
        Dim JIKINKO_CODE As String          ' �����ɃR�[�h
        Dim JIKINKO_NAME As String          ' �����ɖ�
        Dim HONBU_CODE As String            ' �{���R�[�h
        Dim TESUU_KOUZA1 As String          ' �萔�����������P
        Dim UCHIWAKE1 As String             ' ����P
        Dim TESUU_KOUZA2 As String          ' �萔�����������Q
        Dim UCHIWAKE2 As String             ' ����Q
        Dim TESUU_KOUZA3 As String          ' �萔�����������R
        Dim UCHIWAKE3 As String             ' ����R
        Dim TESUU_KOUZA4 As String          ' �萔�����������S
        Dim UCHIWAKE4 As String             ' ����S
        Dim TESUU_KOUZA5 As String          ' �萔�����������T
        Dim UCHIWAKE5 As String             ' ����T
        Dim KAWASE_IRAININ As String        ' �בֈ˗��l��
        Dim RIENTA_PATH As String           ' ���G���^�t�@�C���쐬��
        Dim DAT_PATH As String              ' DAT�̃p�X
        Dim CSVPATH As String               ' CSV�t�@�C���쐬��
        Dim TESUU_OPEKBN As String          ' �萔���I�y�敪
        Dim RIENTA_FILENAME As String       ' ���G���^�t�@�C����
        Dim SIKINTEKIYOU As String          ' �E�v���̈ꕔ��INI�t�@�C�����擾���邽�� ����
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
        Dim RSV2_EDITION As String        ' RSV2�@�\�ݒ�
        Dim COMMON_BAITAIWRITE As String  ' �}�̏����p�t�H���_
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END
    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' ���σ}�X�^�̃L�[���ځ{���G���^�t�@�C����
    ''' </summary>
    ''' <remarks></remarks>
    Structure kesmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' ������
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private keskey As kesmastKey

    Dim ParaKessaiDate As String            ' �p�����[�^��������p�������ϓ�

    Structure KeyInfo
        '���σf�[�^�쐬�����[�b�r�u�f�[�^�쐬���p
        Dim TORIS_CODE As String            ' ������R�[�h
        Dim TORIF_CODE As String            ' ����敛�R�[�h
        Dim FURI_DATE As String             ' �U�֓�
        Dim KESSAI_YDATE As String          ' ���ϗ\���
        Dim TESUU_YDATE As String           ' �萔�������\���
        Dim TESUU_KIN As String             ' �萔�����z  �F�萔�����v
        Dim TESUU_KIN1 As String            ' �萔�����z�P�F�����萔��
        Dim TESUU_KIN2 As String            ' �萔�����z�Q�F����
        Dim TESUU_KIN3 As String            ' �萔�����z�R�F�U���萔��
        Dim FURI_KEN As String              ' �U�֍ό���
        Dim FURI_KIN As String              ' �U�֍ϋ��z
        Dim KIGYO_CODE As String            ' ��ƃR�[�h
        Dim BAITAI_CODE As String           ' �}�̃R�[�h
        Dim SYUBETU_CODE As String          ' ��ʃR�[�h
        Dim ITAKU_CODE As String            ' �ϑ��҃R�[�h
        Dim ITAKU_NNAME As String           ' �ϑ��Җ�����
        Dim ITAKU_KNAME As String           ' �ϑ��Җ��J�i
        Dim FURI_CODE As String             ' �U�փR�[�h
        Dim NS_KBN As String                ' ���o���敪
        Dim TESUUTYO_PATN As String         ' �萔���������@
        Dim TESUUTYO_KBN As String          ' �萔�������敪
        Dim KESSAI_KBN As String            ' ���ϋ敪
        Dim TORIMATOME_SIT_NO As String     ' �Ƃ�܂ƂߓX
        Dim HONBU_KOUZA As String           ' �{���ʒi�����ԍ�
        Dim TUKEKIN_NO As String            ' ���ϋ��Z�@��
        Dim TUKESIT_NO As String            ' ���ώx�X
        Dim TUKEKAMOKU As String            ' ���ωȖ�
        Dim TUKEKOUZA As String             ' ���ό����ԍ�
        Dim TUKEMEIGI As String             ' ���ϖ��`�l �i�J�i�j
        Dim BIKOU1 As String                ' ���l�P
        Dim BIKOU2 As String                ' ���l�Q
        Dim TSUUTYOSIT_NO As String         ' �萔�������x�X
        Dim TSUUTYOKAMOKU As String         ' �萔�������Ȗ�
        Dim TSUUTYOKOUZA As String          ' �萔���������ԍ�
        Dim KESSAI_TORI_KBN As String       ' 0�F�������ςƎ萔�������̗����̐�A1�F�������ς̂ݐ�A2�F�萔�������̂ݐ�
        Dim TESUUTYO_FLG As String          ' �萔�������σt���O
        Dim RECTUUBAN As Long               ' ���R�[�h�ԍ�
        Dim JIFURI_TESUU_KIN As Long        ' ���U�萔�����v
        Dim FURIKOMI_TESUU_KIN As Long      ' �U���萔�����v

        Dim KESSAI_FLG As String            ' ���σt���O
        Dim KESSAI_DATE As String           ' ���ϓ���
        Dim TESUU_DATE As String            ' �萔����������
        Dim SYORI_KEN As String             ' ��������
        Dim SYORI_KIN As String             ' �������z
        Dim FUNOU_KEN As String             ' �s�\����
        Dim FUNOU_KIN As String             ' �s�\���z
        Dim KAWASE_FURI_KEN As String       ' �ב֐U���搔
        Dim KAWASE_SEIKYU_KEN As String     ' �ב֐����搔

        ' �w�Z���U�p
        Dim GAKUNEN_FLG As ArrayList        ' �w�N�t���O
        Dim KESSAI_SYUBETU As String        ' ���ώ��(0�F�ϑ��҈ꊇ���� / 1�F��ڌ����P�ʐ���)
        Dim KESSAI_KIN_CODE As String       ' ���ϋ��Z�@�փR�[�h
        Dim KESSAI_TENPO As String          ' ���ώx�X�R�[�h
        Dim KESSAI_KAMOKU As String         ' ���ωȖ�
        Dim KESSAI_MEIGI As String          ' ���ϖ��`�l
        Dim KESSAI_KOUZA As String          ' ���ό����ԍ�
        Dim HIMOKU_KINGAKU As String        ' ��ڋ��z
        Dim HIMOKU_FURI_KIN As String       ' ��ڐU�֋��z
        Dim HIMOKU_FUNOU_KIN As String      ' ��ڕs�\���z
        Dim UpSchMastFLG As Boolean         ' �X�P�W���[���}�X�^�X�V�Ώۃt���O
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
        Dim HIMOKU_NAME As String           ' ��ږ�
        Dim HIMOKU_GASSAN As Long           ' �w�N�A��ڔԍ��ʂ̍��Z��
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        Dim MESSAGE As String

        '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------START
        Dim SEIKYU_KBN As String            '�萔�������敪
        '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------END
        ' ������
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            FURI_DATE = ""
            KIGYO_CODE = ""
            KESSAI_YDATE = ""
            TESUU_YDATE = ""
            TESUU_KIN = ""
            TESUU_KIN1 = ""
            TESUU_KIN2 = ""
            TESUU_KIN3 = ""
            FURI_KEN = ""
            FURI_KIN = ""
            BAITAI_CODE = ""
            SYUBETU_CODE = ""
            ITAKU_CODE = ""
            ITAKU_NNAME = ""
            ITAKU_KNAME = ""
            FURI_CODE = ""
            NS_KBN = ""
            TESUUTYO_PATN = ""
            TESUUTYO_KBN = ""
            KESSAI_KBN = ""
            TORIMATOME_SIT_NO = ""
            HONBU_KOUZA = ""
            TUKEKIN_NO = ""
            TUKESIT_NO = ""
            TUKEKAMOKU = ""
            TUKEKOUZA = ""
            TUKEMEIGI = ""
            BIKOU1 = ""
            BIKOU2 = ""
            TSUUTYOSIT_NO = ""
            TSUUTYOKAMOKU = ""
            TSUUTYOKOUZA = ""
            KESSAI_TORI_KBN = ""
            TESUUTYO_FLG = "0"

            KESSAI_FLG = ""
            KESSAI_DATE = ""
            TESUU_DATE = ""
            SYORI_KEN = ""
            SYORI_KIN = ""
            FUNOU_KEN = ""
            FUNOU_KIN = ""
            KAWASE_FURI_KEN = ""
            KAWASE_SEIKYU_KEN = ""

            KESSAI_SYUBETU = "0"
            KESSAI_KIN_CODE = ""
            KESSAI_TENPO = ""
            KESSAI_KAMOKU = ""
            KESSAI_MEIGI = ""
            KESSAI_KOUZA = ""
            HIMOKU_KINGAKU = ""

            '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------START
            SEIKYU_KBN = ""
            '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------END
            MESSAGE = ""
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            HIMOKU_NAME = ""
            HIMOKU_GASSAN = 0
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
        End Sub

        ' �c�a����̒l��ݒ�i�������σf�[�^�쐬�p�j
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_SV1").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_SV1").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_SV1").PadRight(8)
            KESSAI_YDATE = oraReader.GetString("KESSAI_YDATE_SV1")
            TESUU_YDATE = oraReader.GetString("TESUU_YDATE_SV1")
            TESUU_KIN = oraReader.GetString("TESUU_KIN_SV1")
            TESUU_KIN1 = oraReader.GetString("TESUU_KIN1_SV1")
            TESUU_KIN2 = oraReader.GetString("TESUU_KIN2_SV1")
            TESUU_KIN3 = oraReader.GetString("TESUU_KIN3_SV1")
            FURI_KEN = oraReader.GetString("FURI_KEN_SV1")
            FURI_KIN = oraReader.GetString("FURI_KIN_SV1")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_TV1")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_TV1")
            SYUBETU_CODE = oraReader.GetString("SYUBETU_TV1")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_TV1")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_TV1")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_TV1")
            FURI_CODE = oraReader.GetString("FURI_CODE_TV1")
            NS_KBN = oraReader.GetString("NS_KBN_TV1")
            TESUUTYO_PATN = oraReader.GetString("TESUUTYO_PATN_TV1")
            TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_TV1")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_TV1")
            TORIMATOME_SIT_NO = oraReader.GetString("TORIMATOME_SIT_TV1")
            HONBU_KOUZA = oraReader.GetString("HONBU_KOUZA_TV1")
            TUKEKIN_NO = oraReader.GetString("TUKEKIN_NO_TV1")
            TUKESIT_NO = oraReader.GetString("TUKESIT_NO_TV1")
            TUKEKAMOKU = oraReader.GetString("TUKEKAMOKU_TV1")
            TUKEKOUZA = oraReader.GetString("TUKEKOUZA_TV1")
            TUKEMEIGI = oraReader.GetString("TUKEMEIGI_KNAME_TV1")
            BIKOU1 = oraReader.GetString("BIKOU1_TV1")
            BIKOU2 = oraReader.GetString("BIKOU2_TV1")
            TSUUTYOSIT_NO = oraReader.GetString("TESUUTYO_SIT_TV1")
            TSUUTYOKAMOKU = oraReader.GetString("TESUUTYO_KAMOKU_TV1")
            TSUUTYOKOUZA = oraReader.GetString("TESUUTYO_KOUZA_TV1")
            KESSAI_TORI_KBN = oraReader.GetString("KESSAI_TORI_KBN")
            SYORI_KEN = oraReader.GetString("SYORI_KEN_SV1")
            SYORI_KIN = oraReader.GetString("SYORI_KIN_SV1")
            FUNOU_KEN = oraReader.GetString("FUNOU_KEN_SV1")
            FUNOU_KIN = oraReader.GetString("FUNOU_KIN_SV1")
            '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------START
            SEIKYU_KBN = oraReader.GetString("SEIKYU_KBN_TV1")
            '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------END
        End Sub

        ' �c�a����̒l��ݒ�i��ڌ����P�ʂ̌��Ϗ��p�j
        Friend Sub SetOraDataHimo(ByVal oraReader As CASTCommon.MyOracleReader)
            KESSAI_KIN_CODE = oraReader.GetString("KESSAI_KIN_CODE_H")
            KESSAI_TENPO = oraReader.GetString("KESSAI_TENPO_H")
            KESSAI_KAMOKU = oraReader.GetString("KESSAI_KAMOKU_H")
            KESSAI_MEIGI = oraReader.GetString("KESSAI_MEIGI_H")
            KESSAI_KOUZA = oraReader.GetString("KESSAI_KOUZA_H")
            HIMOKU_KINGAKU = oraReader.GetString("HIMOKU_KINGAKU_H")
            HIMOKU_FURI_KIN = oraReader.GetString("HIMOKU_FURI_KIN_H")
            HIMOKU_FUNOU_KIN = oraReader.GetString("HIMOKU_FUNOU_KIN_H")
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            HIMOKU_NAME = oraReader.GetString("HIMOKU_NAME_H")
            HIMOKU_GASSAN = oraReader.GetInt64("YOBI1_H")
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
        End Sub

    End Structure

    Structure KeyGakInfo
        Dim GAKKOU_CODE As String           ' �w�Z�R�[�h
        Dim GAKUNEN_CODE As String          ' �w�N�R�[�h
        Dim HIMOKU_NAME As String           ' ��ږ�
        Dim KESSAI_KIN_CODE As String       ' �戵���Z�@�փR�[�h
        Dim KESSAI_TENPO As String          ' �戵�x�X�R�[�h
        Dim KESSAI_KAMOKU As String         ' �Ȗ�
        Dim KESSAI_KOUZA As String          ' �����ԍ�
        Dim KESSAI_MEIGI As String          ' �������`�l
        Dim HIMOKU_KINGAKU As Long          ' ��ڋ��z
        Dim FURIKETU_CODE As String         ' �U�֌��ʃR�[�h
        Dim HIMOKU_FURI_KIN As Long         ' ��ڐU�֋��z
        Dim HIMOKU_FUNOU_KIN As Long        ' ��ڕs�\���z
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
        Dim HIMOKU_GASSAN As Long           ' �w�N�A��ڔԍ��ʂ̍��Z����
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END


        ' ������
        Public Sub Init()
            GAKKOU_CODE = ""
            GAKUNEN_CODE = ""
            HIMOKU_NAME = ""
            KESSAI_KIN_CODE = ""
            KESSAI_TENPO = ""
            KESSAI_KAMOKU = ""
            KESSAI_KOUZA = ""
            KESSAI_MEIGI = ""
            HIMOKU_KINGAKU = 0
            HIMOKU_FURI_KIN = 0
            HIMOKU_FUNOU_KIN = 0
            FURIKETU_CODE = ""
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            HIMOKU_GASSAN = 0               ' �w�N�A��ڔԍ��ʂ̍��Z����
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
        End Sub

        ' �c�a����̒l��ݒ�i���σ��[�N�}�X�^�쐬�p�j
        Friend Sub SetOraDataGak(ByVal oraReader As CASTCommon.MyOracleReader, ByVal HimoNo As String)
            GAKKOU_CODE = oraReader.GetString("GAKKOU_CODE_H")
            GAKUNEN_CODE = oraReader.GetString("GAKUNEN_CODE_H")
            HIMOKU_NAME = oraReader.GetString("HIMOKU_NAME" & HimoNo & "_H")
            KESSAI_KIN_CODE = oraReader.GetString("KESSAI_KIN_CODE" & HimoNo & "_H")
            KESSAI_TENPO = oraReader.GetString("KESSAI_TENPO" & HimoNo & "_H")
            KESSAI_KAMOKU = oraReader.GetString("KESSAI_KAMOKU" & HimoNo & "_H")
            KESSAI_KOUZA = oraReader.GetString("KESSAI_KOUZA" & HimoNo & "_H")
            KESSAI_MEIGI = oraReader.GetString("KESSAI_MEIGI" & HimoNo & "_H")
            HIMOKU_KINGAKU = oraReader.GetInt64("HIMOKU" & CInt(HimoNo) & "_KIN_M")
            FURIKETU_CODE = oraReader.GetString("FURIKETU_CODE_M")

            ' ��ڐU�֋��z,��ڕs�\���z�̐ݒ�
            If FURIKETU_CODE = "0" Then             ' 0:�U�֍�
                HIMOKU_FURI_KIN = HIMOKU_KINGAKU
                HIMOKU_FUNOU_KIN = 0
            Else                                    ' 1:�s�\
                HIMOKU_FURI_KIN = 0
                HIMOKU_FUNOU_KIN = HIMOKU_KINGAKU
            End If
        End Sub

    End Structure

    ' 2016/06/13 �^�X�N�j���� CHG �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
    Structure msgDATA
        Dim msg_DATA As String
        Public Sub Init()
            msg_DATA = ""
        End Sub
    End Structure
    ' 2016/06/13 �^�X�N�j���� CHG �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' �������Ϗ�������
    ''' </summary>
    ''' <returns>����:True �ُ�:False</returns>
    ''' <remarks></remarks>
    Public Function KessaiInit(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try

            '�p�����[�^�̓Ǎ�
            param = CmdArgs(0).Split(","c)
            If param.Length = 2 Then

                '���O�����ݏ��̐ݒ�
                MainLOG.FuriDate = param(0)                     '���ϓ����Z�b�g
                MainLOG.JobTuuban = CType(param(1), Integer)
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(��������)�J�n", "����")


                ParaKessaiDate = param(0)                       '���ϓ����Z�b�g

            ElseIf param.Length = 1 Then    '2010.01.27 �ǉ��@start

                '���O�����ݏ��̐ݒ�
                MainLOG.FuriDate = param(0)                     '���ϓ����Z�b�g
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(��������)�J�n", "����")

                ParaKessaiDate = param(0)                       '���ϓ����Z�b�g
                '2010.01.27 �ǉ��@end
            Else
                MainLOG.Write("(��������)�J�n", "���s", "�R�}���h���C�������̃p�����[�^���s���ł�")
                Return False

            End If

            'ini�t�@�C���̓Ǎ�
            If IniRead() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception

            MainLOG.Write("(��������)�J�n", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write("(��������)�I��", "����")
        End Try

    End Function

    Private Function IniRead() As Boolean

        ini_info.KAWASE_CENTER = CASTCommon.GetFSKJIni("KAWASE", "KAWASECENTER")      '���M�Z���^��
        If ini_info.KAWASE_CENTER = "err" OrElse ini_info.KAWASE_CENTER = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���M�Z���^�� ����:KAWASE ����:KAWASECENTER")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���M�Z���^�� ����:KAWASE ����:KAWASECENTER"
            Return False
        End If

        ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")           '�����ɃR�[�h
        If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD"
            Return False
        End If

        ini_info.JIKINKO_NAME = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")       '�����ɖ�
        If ini_info.JIKINKO_NAME = "err" OrElse ini_info.JIKINKO_NAME = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɖ� ����:COMMON ����:KINKONAME")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�����ɖ� ����:COMMON ����:KINKONAME"
            Return False
        End If

        ini_info.HONBU_CODE = CASTCommon.GetFSKJIni("COMMON", "HONBUCD")         '�{���R�[�h
        If ini_info.HONBU_CODE = "err" OrElse ini_info.HONBU_CODE = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�{���R�[�h ����:COMMON ����:HONBUCD")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�{���R�[�h ����:COMMON ����:HONBUCD"
            Return False
        End If

        ini_info.TESUU_KOUZA1 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA1")       '�萔�����������P
        If ini_info.TESUU_KOUZA1 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������P ����:KESSAI ����:TESUUKOUZA1")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������P ����:KESSAI ����:TESUUKOUZA1"
            Return False
        End If

        ini_info.UCHIWAKE1 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE1")         '����P
        If ini_info.UCHIWAKE1 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:����P ����:KESSAI ����:UTIWAKE1")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:����P ����:KESSAI ����:UTIWAKE1"
            Return False
        End If

        ini_info.TESUU_KOUZA2 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA2")       '�萔�����������Q
        If ini_info.TESUU_KOUZA2 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������Q ����:KESSAI ����:TESUUKOUZA2")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������Q ����:KESSAI ����:TESUUKOUZA2"
            Return False
        End If

        ini_info.UCHIWAKE2 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE2")         '����Q
        If ini_info.UCHIWAKE2 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:����Q ����:KESSAI ����:UTIWAKE2")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:����Q ����:KESSAI ����:UTIWAKE2"
            Return False
        End If

        ini_info.TESUU_KOUZA3 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA3")       '�萔�����������R
        If ini_info.TESUU_KOUZA3 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������R ����:KESSAI ����:TESUUKOUZA3")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������R ����:KESSAI ����:TESUUKOUZA3"
            Return False
        End If

        ini_info.UCHIWAKE3 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE3")         '����R
        If ini_info.UCHIWAKE3 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:����R ����:KESSAI ����:UTIWAKE3")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:����R ����:KESSAI ����:UTIWAKE3"
            Return False
        End If

        ini_info.TESUU_KOUZA4 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA4")       '�萔�����������S
        If ini_info.TESUU_KOUZA4 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������S ����:KESSAI ����:TESUUKOUZA4")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������S ����:KESSAI ����:TESUUKOUZA4"
            Return False
        End If

        ini_info.UCHIWAKE4 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE4")         '����S
        If ini_info.UCHIWAKE4 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:����S ����:KESSAI ����:UTIWAKE4")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:����S ����:KESSAI ����:UTIWAKE4"
            Return False
        End If

        ini_info.TESUU_KOUZA5 = CASTCommon.GetFSKJIni("KESSAI", "TESUUKOUZA5")       '�萔�����������T
        If ini_info.TESUU_KOUZA5 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������T ����:KESSAI ����:TESUUKOUZA5")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�萔�����������T ����:KESSAI ����:TESUUKOUZA5"
            Return False
        End If

        ini_info.UCHIWAKE5 = CASTCommon.GetFSKJIni("KESSAI", "UTIWAKE5")         '����T
        If ini_info.UCHIWAKE5 = "err" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:����T ����:KESSAI ����:UTIWAKE5")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:����T ����:KESSAI ����:UTIWAKE5"
            Return False
        End If

        ini_info.KAWASE_IRAININ = CASTCommon.GetFSKJIni("KESSAI", "IRAININ")     '�בֈ˗��l��
        If ini_info.KAWASE_IRAININ = "err" OrElse ini_info.KAWASE_IRAININ = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�בֈ˗��l�� ����:KESSAI ����:IRAININ")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�בֈ˗��l�� ����:KESSAI ����:IRAININ"
            Return False
        End If

        ini_info.SIKINTEKIYOU = CASTCommon.GetFSKJIni("KESSAI", "SIKINTEKIYOU")
        If ini_info.SIKINTEKIYOU = "err" OrElse ini_info.SIKINTEKIYOU = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�E�v ����:KESSAI ����:SIKINTEKIYOU")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�E�v ����:KESSAI ����:SIKINTEKIYOU"
            Return False
        End If

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        '���G���^�t�@�C���쐬��
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DAT�̃p�X
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT"
            Return False
        End If

        ini_info.CSVPATH = CASTCommon.GetFSKJIni("COMMON", "CSV")           'CSV�t�@�C���쐬��
        If ini_info.CSVPATH = "err" OrElse ini_info.CSVPATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:CSV�t�@�C���ۑ��� ����:COMMON ����:CSV")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:CSV�t�@�C���ۑ��� ����:COMMON ����:CSV"
            Return False
        End If

        ini_info.TESUU_OPEKBN = CASTCommon.GetFSKJIni("KESSAI", "OPE_TESUU")       '�萔���I�y�敪
        If ini_info.TESUU_OPEKBN = "err" OrElse ini_info.TESUU_OPEKBN = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�萔���I�y�敪 ����:KESSAI ����:OPE_TESUU")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�萔���I�y�敪 ����:KESSAI ����:OPE_TESUU"
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")       '���G���^�t�@�C����
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�萔���I�y�敪 ����:KESSAI ����:RIENTANAME")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�萔���I�y�敪 ����:KESSAI ����:RIENTANAME"
            Return False
        End If

        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
        ini_info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If ini_info.RSV2_EDITION = "err" OrElse ini_info.RSV2_EDITION = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION"
            Return False
        End If

        ini_info.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
        If ini_info.COMMON_BAITAIWRITE = "err" OrElse ini_info.COMMON_BAITAIWRITE = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�}�̏����p�t�H���_ ����:COMMON ����:BAITAIWRITE")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�}�̏����p�t�H���_ ����:COMMON ����:BAITAIWRITE"
            Return False
        End If
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

        Return True

    End Function

    ' �@�\�@ �F �������σf�[�^�쐬���� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Public Function Main(ByVal command As String) As Integer

        MainDB = New CASTCommon.MyOracle
        Dim bRet As Boolean = True
        Dim iRet As Integer
        Dim sw As System.Diagnostics.Stopwatch = Nothing

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

        ' �p�����[�^�`�F�b�N
        Try
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "�������σf�[�^�쐬����(�J�n)", "����")


            MainDB.BeginTrans()     ' �g�����U�N�V�����J�n

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("(�又��)", "���s", "�������σ��G���^�쐬�����Ŏ��s�҂��^�C���A�E�g")
                MainLOG.UpdateJOBMASTbyErr("�������σ��G���^�쐬�����Ŏ��s�҂��^�C���A�E�g")
                Return -1
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            '*******************************
            ' �񎟂��擾
            '*******************************
            If GetKaiji() = False Then
                MainLOG.Write("(�又��)", "���s", "�񎟂̎擾�Ɏ��s���܂���")
                Return -1
            End If

            '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
            '********************************************
            ' ���ωȖڂ́u���̑��v�̐M���ȖڃR�[�h���擾
            '********************************************
            Call GetKessaiKamokuCode_ETC()
            '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END

            '*****************************************
            ' �������σf�[�^���i�[�ƃX�P�W���[���̍X�V
            '*****************************************
            Dim aryKessaiData As New ArrayList      '�������σf�[�^�i�[�p
            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
            Dim aryMsgData As New ArrayList
            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            Dim aryHimokuName As New ArrayList      '��ږ��ێ��p���X�g
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            iRet = MakeKessaiData(aryKessaiData, aryMsgData, aryHimokuName)
            'iRet = MakeKessaiData(aryKessaiData, aryMsgData)
            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            Select Case iRet
                Case 0          ' �f�[�^�i�[����
                    bRet = True
                Case 1          ' �Ώۃf�[�^�O��
                    bRet = True
                Case Else       ' �f�[�^�i�[���s
                    bRet = False
            End Select

            '***********************
            ' ���G���^FD�쐬
            '***********************
            Dim totalRow As Integer = aryKessaiData.Count()
            Dim msgtitle As String = "�������σ��G���^�쐬(KFK010)"

            If iRet = 0 AndAlso aryKessaiData.Count() > 0 Then

                If MakeRientaFD(aryKessaiData) = False Then     '���s�ڂ��牽�s�ڂ̃f�[�^�܂ō쐬���邩�w��,FD�����ڂ����w��
                    jobMessage = "�������σ��G���^�쐬���s"
                    iRet = -1
                Else
                    Select Case ini_info.RSV2_EDITION
                        Case "2"
                            '---------------------------------------------------------------
                            ' �t�@�C�����\�z
                            '  [�t�@�C����] RNT_KR_yyyyMMddHHmmss_1(1�Œ�)
                            '---------------------------------------------------------------
                            Dim RientFileName As String = "RNT_KR_" & strDate & "_" & strTime & "_1"

                            '---------------------------------------------
                            ' �t�@�C���R�s�[
                            '---------------------------------------------
                            If File.Exists(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName)) Then
                                File.Delete(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName))
                            End If
                            File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME), Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName), True)

                        Case Else
                            Do
                                Try

                                    Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                                    Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                                    iRet = 0
                                    Exit Do

                                Catch ex As Exception
                                    If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                        iRet = -1
                                        jobMessage = "�}�̑}���L�����Z��"
                                        MainLOG.Write_LEVEL1("�}�̗v��", "���s", "�}�̑}���L�����Z��")
                                        Exit Do
                                    End If
                                End Try
                            Loop

                            Select Case iRet
                                Case 0          ' �f�[�^�i�[����
                                    If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME)) Then
                                        If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                            jobMessage = "�}�̓��t�@�C���폜�L�����Z��"
                                            iRet = -1
                                        End If
                                    End If

                                    If iRet = 0 Then
                                        File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME), Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME), True)
                                    End If
                            End Select
                    End Select
                End If

            End If

            If iRet <> 0 Then
                bRet = False
            End If

            '*******************************
            ' ���[�o��
            '*******************************
            ' �������σf�[�^���P���ȏ㑶�݂���ꍇ�A���[�o��
            If iRet = 0 Then
                ' ���[�o��
                Dim PrnSyoKekka As ClsPrnKessaiSyorikekkaKakuninhyo = Nothing
                Dim intPrnRet As Integer

                PrnSyoKekka = New ClsPrnKessaiSyorikekkaKakuninhyo

                PrnSyoKekka.OraDB = MainDB
                PrnSyoKekka.Tenmei = ""
                PrnSyoKekka.Itakusyamei = ""

                ' �������Ϗ������ʊm�F�\�@�^�C�g���s�o��
                PrnSyoKekka.CreateCsvFile()

                ' �������Ϗ������ʊm�F�\�@���׍s�o��
                '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                intPrnRet = PrnSyoKekka.OutputCSVKekka(aryKessaiData, ini_info.JIKINKO_CODE, strDate, strTime, ParaKessaiDate, MainDB, aryMsgData, aryHimokuName)
                'intPrnRet = PrnSyoKekka.OutputCSVKekka(aryKessaiData, ini_info.JIKINKO_CODE, strDate, strTime, ParaKessaiDate, MainDB, aryMsgData)
                '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                If intPrnRet <> 0 Then
                    bRet = False
                    MainLOG.Write("�������ʊm�F�\(��������)�o��", "���s", "�������ʊm�F�\(��������)�b�r�u�o�͂Ɏ��s���܂����B")

                End If

                ' �������Ϗ������ʊm�F�\
                If Not PrnSyoKekka Is Nothing And intPrnRet = 0 Then
                    PrnSyoKekka.CloseCsv()

                    '����o�b�`�Ăяo��
                    Dim ExeRepo As New CAstReports.ClsExecute
                    Dim param As String = ""
                    Dim nret As Integer

                    '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����
                    param = MainLOG.UserID & "," & PrnSyoKekka.FileName

                    nret = ExeRepo.ExecReport("KFKP001.EXE", param)

                    If nret <> 0 Then
                        '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                        Select Case nret
                            Case -1
                                jobMessage = "�������ʊm�F�\(��������)����ΏۂO���B"

                            Case Else

                                jobMessage = "�������ʊm�F�\(��������)������s�B�G���[�R�[�h�F" & nret
                        End Select
                        MainLOG.Write("�������ʊm�F�\(��������)���", "���s", jobMessage)

                        bRet = False
                    End If
                End If
            End If

            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            ' �W���u���s�A�����b�N
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            If bRet = False Then
                If MainLOG.JobTuuban <> 0 Then

                    If jobMessage = "" Then
                        Call MainLOG.UpdateJOBMASTbyErr("���O�Q��")
                    Else
                        Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                    End If

                End If
                ' ���[���o�b�N
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "�Ώۃf�[�^�O��"
                Else
                    jobMessage = "���G���^�쐬����:" & CntDenbun & "��"
                End If

                If MainLOG.JobTuuban <> 0 Then

                    Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                End If

                ' �R�~�b�g
                MainDB.Commit()
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            MainLOG.Write("(�又��)", "���s", ex.ToString)
            Return 1
        Finally
            '*** Str Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***
            If Not MainDB Is Nothing Then
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)

                ' ���[���o�b�N
                MainDB.Rollback()
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���d���s�Ή� ***

            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "�������σf�[�^�쐬����(�I��)", "����")

        End Try

        Return 0

    End Function

    ' �@�\�@ �F �������σf�[�^�쐬����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    '*** �C�� mitsu 2008/09/09 0���f�[�^�Ή��̂��ߑ啝�C��(�s�v�s�폜) ***
    '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
    Private Function MakeKessaiData(ByRef aryKes As ArrayList, ByRef aryMSG As ArrayList, ByRef aryHimokuName As ArrayList) As Integer
        'Private Function MakeKessaiData(ByRef aryKes As ArrayList, ByRef aryMSG As ArrayList) As Integer
        '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        Dim OraKesReader As CASTCommon.MyOracleReader       ' ���σr���[

        Dim SQL As StringBuilder
        Dim CommonSQL As StringBuilder
        Dim TudoWhereSQL As StringBuilder
        Dim KessaiOnlyWhereSQL As StringBuilder
        Dim TesuuOnlyWhereSQL As StringBuilder

        OraKesReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)
        CommonSQL = New StringBuilder(128)
        TudoWhereSQL = New StringBuilder(128)
        KessaiOnlyWhereSQL = New StringBuilder(128)
        TesuuOnlyWhereSQL = New StringBuilder(128)

        Try
            MainLOG.Write("(�������σf�[�^�i�[)�J�n", "����")

            CommonSQL.Append("SELECT")
            CommonSQL.Append(" TORIS_CODE_SV1")
            CommonSQL.Append(",TORIF_CODE_SV1")
            CommonSQL.Append(",FURI_DATE_SV1")
            CommonSQL.Append(",KESSAI_YDATE_SV1")
            CommonSQL.Append(",TESUU_YDATE_SV1")
            CommonSQL.Append(",TESUU_KIN_SV1")
            CommonSQL.Append(",TESUU_KIN1_SV1")
            CommonSQL.Append(",TESUU_KIN2_SV1")
            CommonSQL.Append(",TESUU_KIN3_SV1")
            CommonSQL.Append(",FURI_KEN_SV1")
            CommonSQL.Append(",FURI_KIN_SV1")
            CommonSQL.Append(",SYORI_KEN_SV1")
            CommonSQL.Append(",SYORI_KIN_SV1")
            CommonSQL.Append(",FUNOU_KEN_SV1")
            CommonSQL.Append(",FUNOU_KIN_SV1")
            CommonSQL.Append(",KIGYO_CODE_TV1")
            CommonSQL.Append(",BAITAI_CODE_TV1")
            CommonSQL.Append(",SYUBETU_TV1")
            CommonSQL.Append(",ITAKU_CODE_TV1")
            CommonSQL.Append(",ITAKU_NNAME_TV1")
            CommonSQL.Append(",ITAKU_KNAME_TV1")
            CommonSQL.Append(",FURI_CODE_TV1")
            CommonSQL.Append(",NS_KBN_TV1")
            CommonSQL.Append(",TESUUTYO_PATN_TV1")
            CommonSQL.Append(",TESUUTYO_KBN_TV1")
            CommonSQL.Append(",KESSAI_KBN_TV1")
            CommonSQL.Append(",TORIMATOME_SIT_TV1")
            CommonSQL.Append(",HONBU_KOUZA_TV1")
            CommonSQL.Append(",TUKEKIN_NO_TV1")
            CommonSQL.Append(",TUKESIT_NO_TV1")
            CommonSQL.Append(",TUKEKAMOKU_TV1")
            CommonSQL.Append(",TUKEKOUZA_TV1")
            CommonSQL.Append(",TUKEMEIGI_KNAME_TV1")
            CommonSQL.Append(",BIKOU1_TV1")
            CommonSQL.Append(",BIKOU2_TV1")
            CommonSQL.Append(",TESUUTYO_SIT_TV1")
            CommonSQL.Append(",TESUUTYO_KAMOKU_TV1")
            CommonSQL.Append(",TESUUTYO_KOUZA_TV1")
            CommonSQL.Append(",0 AS JYUNBAN")
            '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------START
            '�ʒi�o�������Ή��̂��߁A�萔�������敪�𒊏o
            CommonSQL.Append(",SEIKYU_KBN_TV1")
            '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------END

            ' �������ςƎ萔�������𓯎��ɍs����̏����r�p�k��(�s�x)
            TudoWhereSQL.Append(" WHERE KESSAI_YDATE_SV1 = " & SQ(ParaKessaiDate))
            TudoWhereSQL.Append("   AND TESUU_YDATE_SV1  = " & SQ(ParaKessaiDate))
            TudoWhereSQL.Append("   AND TESUUKEI_FLG_SV1 = '1'")                                    ' 1:�萔���v�Z�ς�
            TudoWhereSQL.Append("   AND KESSAI_FLG_SV1   = '2'")                                    ' 0:������
            TudoWhereSQL.Append("   AND TESUUTYO_FLG_SV1 = '2'")                                    ' 0:�萔��������
            TudoWhereSQL.Append("   AND TYUUDAN_FLG_SV1  = '0'")                                    ' 0:���f�Ȃ�
            TudoWhereSQL.Append("   AND FURI_KIN_SV1     >  0")                                     ' �U�֍ϋ��z����

            ' �������ς����s����̏����r�p�k���i�s�x�ȊO�j
            KessaiOnlyWhereSQL.Append(" WHERE KESSAI_YDATE_SV1  = " & SQ(ParaKessaiDate))
            'KessaiOnlyWhereSQL.Append("   AND TESUU_YDATE_SV1  != " & SQ(ParaKessaiDate))
            KessaiOnlyWhereSQL.Append("   AND TESUUKEI_FLG_SV1  = '1'")                             ' 1:�萔���v�Z��
            KessaiOnlyWhereSQL.Append("   AND KESSAI_FLG_SV1    = '2'")                             ' 0:������
            KessaiOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '0'")                             ' 0:�萔��������
            KessaiOnlyWhereSQL.Append("   AND TYUUDAN_FLG_SV1   = '0'")                             ' 0:���f�Ȃ�
            KessaiOnlyWhereSQL.Append("   AND FURI_KIN_SV1      >  0")                              ' �U�֍ϋ��z����

            '***�萔���݂̂͒��o���Ȃ� 2009.10.29 start
            ' �萔�����������s����̏����r�p�k��
            'TesuuOnlyWhereSQL.Append(" WHERE (KESSAI_YDATE_SV1 != " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUU_YDATE_SV1   = " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUUKEI_FLG_SV1  = '1'")                              ' 1:�萔���v�Z��
            'TesuuOnlyWhereSQL.Append("   AND KESSAI_FLG_SV1    = '1'")                              ' 1:���ύς�
            'TesuuOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '2'")                              ' 0:�萔��������
            'TesuuOnlyWhereSQL.Append("   AND TYUUDAN_FLG_SV1   = '0'")                              ' 0:���f�Ȃ�
            'TesuuOnlyWhereSQL.Append("   AND FURI_KIN_SV1      >  0)")                              ' �U�֍ϋ��z����
            'TesuuOnlyWhereSQL.Append("   OR")

            ' �萔�������敪�� 1:�ꊇ���� �̏ꍇ�A�������ςƎ萔�������𓯎��ɍs������܂߂�
            'TesuuOnlyWhereSQL.Append("      (KESSAI_YDATE_SV1  = " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUU_YDATE_SV1   = " & SQ(ParaKessaiDate))
            'TesuuOnlyWhereSQL.Append("   AND TESUUKEI_FLG_SV1  = '1'")                              ' 1:�萔���v�Z��
            'TesuuOnlyWhereSQL.Append("   AND KESSAI_FLG_SV1    = '2'")                              ' 2:���ϑ҂�
            'TesuuOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '0'")                              ' 0:�萔��������
            ''TesuuOnlyWhereSQL.Append("   AND TESUUTYO_FLG_SV1  = '2'")                             ' 0:�萔��������
            'TesuuOnlyWhereSQL.Append("   AND TYUUDAN_FLG_SV1   = '0'")                              ' 0:���f�Ȃ�
            'TesuuOnlyWhereSQL.Append("   AND FURI_KIN_SV1      >  0")                               ' �U�֍ϋ��z����
            'TesuuOnlyWhereSQL.Append("   AND TESUUTYO_KBN_TV1  = '1')")                             ' 1:�ꊇ����
            '***�萔���݂̂͒��o���Ȃ� 2009.10.29 end

            SQL.Append("(")
            SQL.Append(CommonSQL)
            SQL.Append(",0 AS KESSAI_TORI_KBN")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(TudoWhereSQL)
            SQL.Append(")")
            SQL.Append("UNION")
            SQL.Append("(")
            SQL.Append(CommonSQL)
            SQL.Append(",1 AS KESSAI_TORI_KBN")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(KessaiOnlyWhereSQL)
            SQL.Append(")")
            '***�萔���݂̂͒��o���Ȃ� 2009.10.29 start
            'SQL.Append("UNION")
            'SQL.Append("(")
            'SQL.Append("SELECT")
            'SQL.Append(" TORIS_CODE_SV1")
            'SQL.Append(",TORIF_CODE_SV1")
            'SQL.Append(",'00000000' AS FURI_DATE_SV1")
            'SQL.Append(",'00000000' AS KESSAI_YDATE_SV1")
            'SQL.Append(",MAX(TESUU_YDATE_SV1) AS TESUU_YDATE_SV1")
            'SQL.Append(",SUM(TESUU_KIN_SV1) AS TESUU_KIN_SV1")
            'SQL.Append(",SUM(TESUU_KIN1_SV1) AS TESUU_KIN1_SV1")
            'SQL.Append(",SUM(TESUU_KIN2_SV1) AS TESUU_KIN2_SV1")
            'SQL.Append(",SUM(TESUU_KIN3_SV1) AS TESUU_KIN3_SV1")
            'SQL.Append(",SUM(FURI_KEN_SV1) AS FURI_KEN_SV1")
            'SQL.Append(",SUM(FURI_KIN_SV1) AS FURI_KIN_SV1")
            'SQL.Append(",SUM(SYORI_KEN_SV1) AS SYORI_KEN_SV1")
            'SQL.Append(",SUM(SYORI_KIN_SV1) AS SYORI_KIN_SV1")
            'SQL.Append(",SUM(FUNOU_KEN_SV1) AS FUNOU_KEN_SV1")
            'SQL.Append(",SUM(FUNOU_KIN_SV1) AS FUNOU_KIN_SV1")
            'SQL.Append(",MAX(KIGYO_CODE_TV1) AS KIGYO_CODE_TV1")
            'SQL.Append(",MAX(BAITAI_CODE_TV1) AS BAITAI_CODE_TV1")
            'SQL.Append(",MAX(SYUBETU_TV1) AS SYUBETU_TV1")
            'SQL.Append(",MAX(ITAKU_CODE_TV1) AS ITAKU_CODE_TV1")
            'SQL.Append(",MAX(ITAKU_NNAME_TV1) AS ITAKU_NNAME_TV1")
            'SQL.Append(",MAX(ITAKU_KNAME_TV1) AS ITAKU_KNAME_TV1")
            'SQL.Append(",MAX(FURI_CODE_TV1) AS FURI_CODE_TV1")
            'SQL.Append(",MAX(NS_KBN_TV1) AS NS_KBN_TV1")
            'SQL.Append(",MAX(TESUUTYO_PATN_TV1) AS TESUUTYO_PATN_TV1")
            'SQL.Append(",MAX(TESUUTYO_KBN_TV1) AS TESUUTYO_KBN_TV1")
            'SQL.Append(",MAX(KESSAI_KBN_TV1) AS KESSAI_KBN_TV1")
            'SQL.Append(",MAX(TORIMATOME_SIT_TV1) AS TORIMATOME_SIT_TV1")
            'SQL.Append(",MAX(HONBU_KOUZA_TV1) AS HONBU_KOUZA_TV1")
            'SQL.Append(",MAX(TUKEKIN_NO_TV1) AS TUKEKIN_NO_TV1")
            'SQL.Append(",MAX(TUKESIT_NO_TV1) AS TUKESIT_NO_TV1")
            'SQL.Append(",MAX(TUKEKAMOKU_TV1) AS TUKEKAMOKU_TV1")
            'SQL.Append(",MAX(TUKEKOUZA_TV1) AS TUKEKOUZA_TV1")
            'SQL.Append(",MAX(TUKEMEIGI_KNAME_TV1) AS TUKEMEIGI_KNAME_TV1")
            'SQL.Append(",MAX(BIKOU1_TV1) AS BIKOU1_TV1")
            'SQL.Append(",MAX(BIKOU2_TV1) AS BIKOU2_TV1")
            'SQL.Append(",MAX(TESUUTYO_SIT_TV1) AS TESUUTYO_SIT_TV1")
            'SQL.Append(",MAX(TESUUTYO_KAMOKU_TV1) AS TESUUTYO_KAMOKU_TV1")
            'SQL.Append(",MAX(TESUUTYO_KOUZA_TV1) AS TESUUTYO_KOUZA_TV1")
            'SQL.Append(",1 AS JYUNBAN")
            'SQL.Append(",2 AS KESSAI_TORI_KBN")
            'SQL.Append(" FROM V1_KESMAST")
            'SQL.Append(TesuuOnlyWhereSQL)
            'SQL.Append(" GROUP BY TORIS_CODE_SV1, TORIF_CODE_SV1")
            'SQL.Append(")")
            '***�萔���݂̂͒��o���Ȃ� 2009.10.29 end 

            SQL.Append(" ORDER BY TORIS_CODE_SV1, TORIF_CODE_SV1, JYUNBAN, FURI_DATE_SV1")

            Dim Key As KeyInfo = Nothing
            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
            Dim MSG As msgDATA = Nothing
            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
            Dim test As String = SQL.ToString

            If OraKesReader.DataReader(SQL) = True Then

                Dim lstKessaiData As New ArrayList(128)
                Dim lstHimokuData As New ArrayList(128)
                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                Dim lstMsgData As New ArrayList(128)
                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                Dim lstHimokuName As New ArrayList(128)
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                ' �L�[������
                Key.Init()
                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                MSG.Init()
                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END

                ' �ŏ��̃L�[�ݒ�
                Call Key.SetOraDataKessai(OraKesReader)

                Do While OraKesReader.EOF = False

                    ' �}�̃R�[�h�̔���
                    If Key.BAITAI_CODE = "07" Then
                        ' �}�̃R�[�h��07�F�w�Z���U�̏ꍇ�A���ώ��(0�F�ϑ��҈ꊇ���� or 1�F��ڌ����P�ʐ���)�𔻒�
                        If GetKessaiSyubetu(Key.TORIS_CODE, Key.KESSAI_SYUBETU) = False Then
                            Return -1
                        End If
                    Else
                        ' �}�̃R�[�h��07�F�w�Z���U�ȊO�̏ꍇ�A���ώ�ʂ� 0�F�ϑ��҈ꊇ���� �̂�
                        Key.KESSAI_SYUBETU = "0"
                    End If

                    '*** �C�� mitsu 2008/10/15 �萔�����������s���ꍇ�͔�ڒP�ʏ����ɍs���Ȃ� ***
                    'If Key.KESSAI_SYUBETU = "0" Then
                    If Key.KESSAI_SYUBETU = "0" OrElse Key.KESSAI_TORI_KBN = "2" Then
                        '************************************************************************
                        ' ���ώ�ʂ� 0�F�ϑ��҈ꊇ���� �̏ꍇ

                        ' �������σf�[�^�擾����(��Ǝ��U�p)
                        '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                        If fn_GetKessaiData(Key, lstKessaiData, lstMsgData, lstHimokuName) = False Then
                            'If fn_GetKessaiData(Key, lstKessaiData, lstMsgData) = False Then
                            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                            Return -1
                        End If

                        Key.UpSchMastFLG = True
                    Else
                        ' ���ώ�ʂ� 1�F��ڌ����P�ʐ��� �̏ꍇ�i��ڌ����P�ʂŏW�v���s���j

                        ' ���σ��[�N�}�X�^�쐬����
                        If CreateMAIN0500G_WORK(Key) = False Then
                            Return -1
                        End If

                        ' ���σ��[�N�}�X�^��������
                        If SelectMAIN0500G_WORK(lstHimokuData) = False Then
                            Return -1
                        End If

                        ' �������σf�[�^�擾����(�w�Z���U�p)
                        If Not (lstHimokuData Is Nothing OrElse lstHimokuData.Count = 0) Then
                            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                            If fn_GetGakKessaiData(Key, lstKessaiData, lstHimokuData, lstMsgData, lstHimokuName) = False Then
                                'If fn_GetGakKessaiData(Key, lstKessaiData, lstHimokuData) = False Then
                                '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                Return -1
                            End If

                            Key.UpSchMastFLG = True
                        Else
                            ' ��ڌ����P�ʂ̌��Ϗ�񂪃[�����̏ꍇ
                            ' �������σf�[�^�쐬�A�X�P�W���[���}�X�^�X�V���s��Ȃ�
                            Key.UpSchMastFLG = False
                        End If
                    End If

                    ' �X�P�W���[���}�X�^�̍X�V���� 
                    If Key.UpSchMastFLG = True AndAlso UpdateSchMast(Key) = False Then
                        MainLOG.Write("�X�P�W���[���}�X�^�X�V", "���s", "������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                        Return -1
                    End If

                    ' �Ώۃf�[�^�̎����R�[�h��Ǎ���
                    OraKesReader.NextRead()

                    If OraKesReader.EOF = False Then
                        ' �L�[�̍Đݒ�
                        Call Key.SetOraDataKessai(OraKesReader)

                    End If

                    aryKes.AddRange(lstKessaiData)

                    ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                    aryMSG.AddRange(lstMsgData)
                    ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                    aryHimokuName.AddRange(lstHimokuName)
                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                Loop

            End If


            If aryKes.Count = 0 Then
                MainLOG.Write("(�������σf�[�^�i�[)", "���s", "�����O��")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(�������σf�[�^�i�[)", "���s", ex.Message)

            Return -1
        Finally
            If Not OraKesReader Is Nothing Then OraKesReader.Close()
            MainLOG.Write("(�������σf�[�^�i�[)�I��", "����")

        End Try

        Return 0

    End Function

    ' �@�\�@ �F �������σf�[�^�擾����(��Ǝ��U�p)
    '
    ' �����@ �F 
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
    Private Function fn_GetKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByRef lstMsgData As ArrayList, ByRef lstHimokuName As ArrayList) As Boolean
        'Private Function fn_GetKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByRef lstMsgData As ArrayList) As Boolean
        '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        Dim errFlg As Boolean = False
        Dim errMsg As String = "���Ϗ��Ɍ�肪����܂��B"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing
        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
        Dim MData As msgDATA = Nothing
        Dim OP_Count As Integer = 0
        lstMsgData = New ArrayList(128)
        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
        lstHimokuName = New ArrayList(128)
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        strKEKKA = ""
        lstKessaiData = New ArrayList(128)

        Try

            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
            MData.Init()
            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END

            KData.Init()

            '�萔�������σt���O������������
            Key.TESUUTYO_FLG = "0"

            ' �萔�������敪�� 0:�s�x�����A���A���ϗ\����Ǝ萔�������\����������Ώۓ��̏ꍇ
            If Key.TESUUTYO_KBN = "0" And Key.KESSAI_YDATE = ParaKessaiDate And Key.TESUU_YDATE = ParaKessaiDate Then
                ' ���ϋ敪�̔���
                Select Case Key.KESSAI_KBN
                    Case "00"    ' ���񂫂񒆋��a����

                        ' �萔���������@�̔���
                        Select Case Key.TESUUTYO_PATN
                            Case "0"    ' ��������

                                ' ���ωȖڂ̔���
                                Select Case Key.TUKEKAMOKU
                                    Case "99"   ' ������
                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ����������̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_SYOKANJO_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ����������z�Ɏ萔������������Ă���ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then

                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select

                                        End If

                                    Case Else   ' ��L�ȊO�̏ꍇ
                                        errFlg = True
                                        errMsg &= "(���ωȖ�)"
                                End Select

                            Case "1"    ' ���ړ���
                                errFlg = True
                                errMsg &= "(�萔���������@)"

                            Case Else   ' ��L�ȊO�̏ꍇ
                                errFlg = True
                                errMsg &= "(�萔���������@)"
                        End Select

                    Case "01"    ' ��������

                        ' �萔���������@�̔���
                        Select Case Key.TESUUTYO_PATN
                            Case "0"    ' ��������

                                ' ���ωȖڂ̔���
                                Select Case Key.TUKEKAMOKU
                                    Case "02"   ' ����

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ���ʓ����̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ���ʓ����z�Ɏ萔������������Ă���ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select

                                        End If

                                    Case "01"   ' ����

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ���������̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ���������z�Ɏ萔������������Ă���ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select
                                        End If

                                    Case "04"   ' �ʒi

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �ʒi�����̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �ʒi�����z�Ɏ萔������������Ă���ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select
                                        End If

                                    Case Else   ' ��L�ȊO�̏ꍇ
                                        errFlg = True
                                        errMsg &= "(���ωȖ�)"
                                End Select

                            Case "1"    ' ���ړ���

                                ' ���ωȖڂ̔���
                                Select Case Key.TUKEKAMOKU
                                    Case "02"   ' ����

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ���ʓ����̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �U�֍ϋ��z�Ǝ萔�����z�̍��z���O�~���̏ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select
                                        End If

                                    Case "01"   ' ����

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ���������̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �U�֍ϋ��z�Ǝ萔�����z�̍��z���O�~���̏ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select
                                        End If

                                    Case "04"   ' �ʒi

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �ʒi�����̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �U�֍ϋ��z�Ǝ萔�����z�̍��z���O�~���̏ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select
                                        End If

                                    Case Else   ' ��L�ȊO�̏ꍇ
                                        errFlg = True
                                        errMsg &= "(���ωȖ�)"
                                End Select

                            Case Else   ' ��L�ȊO�̏ꍇ
                                errFlg = True
                                errMsg &= "(�萔���������@)"
                        End Select

                    Case "02"    ' �ב֐U��

                        ' �萔���������@�̔���
                        Select Case Key.TESUUTYO_PATN
                            Case "0"    ' ��������

                                ' ���ωȖڂ̔���
                                Select Case Key.TUKEKAMOKU
                                    '2017/05/29 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                                    Case "01", "02", KessaiKamokuCode_ETC  ' �����A���ʁA���̑�"
                                        'Case "01", "02"  ' �����A����
                                        '2017/05/29 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �ב֐U���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' ����������z�Ɏ萔������������Ă���ꍇ�̂݁A�萔���f�[�^�쐬
                                        If strKEKKA <> "���z�O" Then
                                            Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                                Case "1"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                Case "2"
                                                    ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If

                                                    ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                    If CLng(Key.TESUU_KIN3) > 0 Then
                                                        If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                            OP_Count += 1
                                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                        Else
                                                            errFlg = True
                                                        End If
                                                    End If
                                                Case Else
                                                    errFlg = True
                                                    errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                            End Select
                                        End If

                                    Case Else   ' ��L�ȊO�̏ꍇ
                                        errFlg = True
                                        errMsg &= "(���ωȖ�)"
                                End Select

                            Case "1"    ' ���ړ���
                                errFlg = True
                                errMsg &= "(�萔���������@)"

                            Case Else   ' ��L�ȊO�̏ꍇ
                                errFlg = True
                                errMsg &= "(�萔���������@)"
                        End Select

                    Case "03"    ' �ב֕t��
                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                        If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                            errFlg = True
                            errMsg &= "(���ωȖ�)"
                        Else
                            '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                            ' �萔���������@�̔���
                            Select Case Key.TESUUTYO_PATN
                                Case "0"    ' ��������
                                    ' �ʒi�o���̃f�[�^�쐬
                                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' �������σf�[�^�̃��X�g�쐬
                                        lstKessaiData.Add(KData)
                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                        OP_Count += 1
                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                    Else
                                        errFlg = True
                                    End If

                                    ' �ב֕t�ւ̃f�[�^�쐬
                                    If errFlg = False AndAlso fn_KAWASE_TUKEKAE(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' �������σf�[�^�̃��X�g�쐬
                                        lstKessaiData.Add(KData)
                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                        OP_Count += 1
                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                    Else
                                        errFlg = True
                                    End If

                                    ' �ב֕t�֋��z�Ɏ萔������������Ă���ꍇ�̂݁A�萔���f�[�^�쐬
                                    If strKEKKA <> "���z�O" Then
                                        Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                            Case "1"
                                                ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' �������σf�[�^�̃��X�g�쐬
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If

                                                ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                If CLng(Key.TESUU_KIN3) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' �������σf�[�^�̃��X�g�쐬
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If

                                            Case "2"
                                                ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                                If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' �������σf�[�^�̃��X�g�쐬
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If

                                                ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                                If CLng(Key.TESUU_KIN3) > 0 Then
                                                    If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' �������σf�[�^�̃��X�g�쐬
                                                        lstKessaiData.Add(KData)
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                        OP_Count += 1
                                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                                    Else
                                                        errFlg = True
                                                    End If
                                                End If
                                            Case Else
                                                errFlg = True
                                                errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                        End Select
                                    End If

                                '*** �C�� mitsu 2008/06/25 �Ȗڂ̔���͂��Ȃ� ***
                                '    Case Else   ' ��L�ȊO�̏ꍇ
                                '        errFlg = True
                                '        errMsg &= "(���ωȖ�)"
                                'End Select
                                '************************************************

                                Case "1"    ' ���ړ���
                                    errFlg = True
                                    errMsg &= "(�萔���������@)"

                                Case Else   ' ��L�ȊO�̏ꍇ
                                    errFlg = True
                                    errMsg &= "(�萔���������@)"
                            End Select
                            '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                        End If
                    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                    Case "04"    ' �ʒi�o���̂�
                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                        If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                            errFlg = True
                            errMsg &= "(���ωȖ�)"
                        Else
                            '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                            ' �ʒi�o���̃f�[�^�쐬
                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                ' �������σf�[�^�̃��X�g�쐬
                                lstKessaiData.Add(KData)
                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                OP_Count += 1
                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                            Else
                                errFlg = True
                            End If
                            '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                        End If
                    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                    Case "05"    ' ���ʊ�ƊO
                        ' �����ΏۊO

                    Case "99"
                        ' �����ΏۊO

                    Case Else   ' ��L�ȊO�̏ꍇ
                        errFlg = True
                        errMsg &= "(���ϋ敪)"
                End Select

            ElseIf Key.TESUUTYO_KBN = "0" OrElse Key.TESUUTYO_KBN = "1" OrElse Key.TESUUTYO_KBN = "2" OrElse Key.TESUUTYO_KBN = "3" Then

                ' ���ϗ\����������Ώۓ��̏ꍇ
                If Key.KESSAI_YDATE = ParaKessaiDate Then

                    ' �萔���������@�̔���
                    Select Case Key.TESUUTYO_PATN
                        Case "0"    ' ��������

                            Select Case Key.KESSAI_KBN
                                Case "04"    ' �ʒi�o���̂�

                                    ' �ʒi�o���̃f�[�^�쐬
                                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' �������σf�[�^�̃��X�g�쐬
                                        lstKessaiData.Add(KData)
                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                        OP_Count += 1
                                        ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                    Else
                                        errFlg = True
                                    End If
                                Case Else
                                    errFlg = True
                                    errMsg &= "(���ϋ敪)"
                            End Select

                        Case "1"    ' ���ړ���
                            ' ���ϋ敪�̔���
                            Select Case Key.KESSAI_KBN
                                Case "00"    ' ���񂫂񒆋��a����

                                    ' ���ωȖڂ̔���
                                    Select Case Key.TUKEKAMOKU
                                        Case "99"   ' ������
                                            ' �ʒi�o���̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' ����������̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_SYOKANJO_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case Else   ' ��L�ȊO�̏ꍇ
                                            errFlg = True
                                            errMsg &= "(���ωȖ�)"
                                    End Select

                                Case "01"    ' ��������

                                    ' ���ωȖڂ̔���
                                    Select Case Key.TUKEKAMOKU
                                        Case "02"   ' ����

                                            ' �ʒi�o���̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' ���ʓ����̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case "01"   ' ����

                                            ' �ʒi�o���̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' ���������̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case "04"   ' �ʒi

                                            ' �ʒi�o���̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' �ʒi�����̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        Case Else   ' ��L�ȊO�̏ꍇ
                                            errFlg = True
                                            errMsg &= "(���ωȖ�)"
                                    End Select

                                Case "02"    ' �ב֐U��
                                    ' ���ωȖڂ̔���
                                    Select Case Key.TUKEKAMOKU
                                        Case "01", "02"  ' �����A����

                                            ' �ʒi�o���̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                            ' �ב֐U���̃f�[�^�쐬
                                            If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                ' �������σf�[�^�̃��X�g�쐬
                                                lstKessaiData.Add(KData)
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                                OP_Count += 1
                                                ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                            Else
                                                errFlg = True
                                            End If

                                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                                        Case KessaiKamokuCode_ETC   ' ���̑�

                                            '�萔�������敪=���ʖƏ��A�ʓr����
                                            If Key.TESUUTYO_KBN = "2" OrElse Key.TESUUTYO_KBN = "3" Then
                                                ' �ʒi�o���̃f�[�^�쐬
                                                If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' �������σf�[�^�̃��X�g�쐬
                                                    lstKessaiData.Add(KData)
                                                    OP_Count += 1
                                                Else
                                                    errFlg = True
                                                End If

                                                ' �ב֐U���̃f�[�^�쐬
                                                If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' �������σf�[�^�̃��X�g�쐬
                                                    lstKessaiData.Add(KData)
                                                    OP_Count += 1
                                                Else
                                                    errFlg = True
                                                End If
                                            Else
                                                errFlg = True
                                                errMsg &= "(�萔�������敪)"
                                            End If
                                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                                        Case Else   ' ��L�ȊO�̏ꍇ
                                            errFlg = True
                                            errMsg &= "(���ωȖ�)"
                                    End Select

                                Case "03"    ' �ב֕t��
                                    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                                    If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                                        errFlg = True
                                        errMsg &= "(���ωȖ�)"
                                    Else
                                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        ' �ב֕t�ւ̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_KAWASE_TUKEKAE(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                                    End If
                                '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                                Case "04"    ' �ʒi�o���̂� �����̋敪�͋C�ɂ����쐬�H

                                    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                                    If Key.TUKEKAMOKU = KessaiKamokuCode_ETC Then
                                        errFlg = True
                                        errMsg &= "(���ωȖ�)"
                                    Else
                                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END


                                        ' �ʒi�o���̃f�[�^�쐬
                                        If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                            ' �������σf�[�^�̃��X�g�쐬
                                            lstKessaiData.Add(KData)
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
                                            OP_Count += 1
                                            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END
                                        Else
                                            errFlg = True
                                        End If

                                        '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- START
                                    End If
                                '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j----------------- END

                                Case "05"    ' ���ʊ��
                                    ' �����ΏۊO

                                Case "99"    ' ���ϑΏۊO
                                    ' �����ΏۊO

                                Case Else   ' ��L�ȊO�̏ꍇ
                                    errFlg = True
                                    errMsg &= "(���ϋ敪)"
                            End Select

                        Case Else   ' ��L�ȊO�̏ꍇ
                            errFlg = True
                            errMsg &= "(�萔���������@)"
                    End Select

                End If

                '***�萔���݂̂̓d���͍쐬���Ȃ� 2009.10.28 start
                '' �萔�������\����������Ώۓ��̏ꍇ
                'If Key.TESUU_YDATE = ParaKessaiDate Then

                '    ' �ꊇ�����A���A�������ςƎ萔�������𓯎��ɍs����̏ꍇ�A�������σf�[�^�͍쐬���Ȃ�
                '    ' �i�ꊇ�����̎萔���́A�萔�����������s����ɂďW�v�ς݁j
                '    If Key.TESUUTYO_KBN = "1" And Key.KESSAI_TORI_KBN = "0" Then
                '        TesuuData_Flg = False
                '    Else
                '        TesuuData_Flg = True
                '    End If

                '    If TesuuData_Flg = True Then

                '        ' ���ϋ敪�̔���
                '        Select Case Key.KESSAI_KBN
                '            Case "00"    ' ���񂫂񒆋��a����
                '                errFlg = True
                '                errMsg &= "(���ϋ敪)"

                '            Case "01"    ' ��������

                '                ' �萔���������@�̔���
                '                Select Case Key.TESUUTYO_PATN
                '                    Case "0"    ' ��������
                '                        errFlg = True
                '                        errMsg &= "(�萔���������@)"

                '                    Case "1"    ' ���ړ���

                '                        ' ���ωȖڂ̔���
                '                        Select Case Key.TUKEKAMOKU
                '                            Case "02"   ' ����

                '                                ' �U�֍ϋ��z�Ǝ萔�����z�̍��z���O�~���̏ꍇ�̂݁A�萔���f�[�^�쐬
                '                                If strKEKKA <> "���z�O" Then
                '                                    Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                '                                        Case "1"
                '                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                        Case "2"
                '                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If
                '                                        Case Else
                '                                            errFlg = True
                '                                            errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                '                                    End Select
                '                                End If

                '                            Case "01"   ' ����

                '                                ' �U�֍ϋ��z�Ǝ萔�����z�̍��z���O�~���̏ꍇ�̂݁A�萔���f�[�^�쐬
                '                                If strKEKKA <> "���z�O" Then
                '                                    Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                '                                        Case "1"
                '                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                        Case "2"
                '                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If
                '                                        Case Else
                '                                            errFlg = True
                '                                            errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                '                                    End Select
                '                                End If

                '                            Case "04"   ' �ʒi

                '                                ' �U�֍ϋ��z�Ǝ萔�����z�̍��z���O�~���̏ꍇ�̂݁A�萔���f�[�^�쐬
                '                                If strKEKKA <> "���z�O" Then
                '                                    Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                '                                        Case "1"
                '                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                        Case "2"
                '                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If

                '                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                '                                            If CLng(Key.TESUU_KIN3) > 0 Then
                '                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJORENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                                                    ' �������σf�[�^�̃��X�g�쐬
                '                                                    lstKessaiData.Add(KData)
                '                                                Else
                '                                                    errFlg = True
                '                                                End If
                '                                            End If
                '                                        Case Else
                '                                            errFlg = True
                '                                            errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                '                                    End Select
                '                                End If

                '                            Case Else ' ��L�ȊO�̏ꍇ
                '                                errFlg = True
                '                                errMsg &= "(���ωȖ�)"
                '                        End Select

                '                    Case Else   ' ��L�ȊO�̏ꍇ
                '                        errFlg = True
                '                        errMsg &= "(�萔���������@)"
                '                End Select

                '            Case "02"    '�ב֐U��
                '                errFlg = True
                '                errMsg &= "(���ϋ敪)"

                '            Case "03"    ' �ב֕t��
                '                errFlg = True
                '                errMsg &= "(���ϋ敪)"

                '            Case "04"    ' �ʒi�o���̂�
                '                errFlg = True
                '                errMsg &= "(���ϋ敪)"

                '            Case "05"    ' ���ʊ��
                '                ' �����ΏۊO

                '            Case "99"    ' ���ϑΏۊO
                '                ' �����ΏۊO

                '            Case Else   ' ��L�ȊO�̏ꍇ
                '                errFlg = True
                '                errMsg &= "(���ϋ敪)"
                '        End Select

                '    End If
                'End If


                'ElseIf Key.TESUUTYO_KBN = "3" Then

                ''************************************************
                '' ���ϗ\����������Ώۓ��̏ꍇ
                'If Key.KESSAI_YDATE = ParaKessaiDate Then
                '    ' ���ϋ敪�̔���
                '    Select Case Key.KESSAI_KBN
                '        Case "00"    ' ���񂫂񒆋��a����

                '            '*** �C�� mitsu 2008/08/14 ���񂫂񒆋��a�����Ή� ***
                '            ' ���ωȖڂ̔���
                '            Select Case Key.TUKEKAMOKU
                '                Case "99"   ' ������
                '                    ' �ʒi�o���̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' ����������̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_SYOKANJO_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case Else   ' ��L�ȊO�̏ꍇ
                '                    errFlg = True
                '                    errMsg &= "(���ωȖ�)"
                '            End Select
                '            '****************************************************

                '        Case "01"    ' ��������

                '            ' ���ωȖڂ̔���
                '            Select Case Key.TUKEKAMOKU
                '                Case "02"   ' ����

                '                    ' �ʒi�o���̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' ���ʓ����̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case "01"   ' ����

                '                    ' �ʒi�o���̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' ���������̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case "04"   ' �ʒi

                '                    ' �ʒi�o���̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' �ʒi�����̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_BETUDAN_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case Else   ' ��L�ȊO�̏ꍇ
                '                    errFlg = True
                '                    errMsg &= "(���ωȖ�)"
                '            End Select

                '        Case "02"    ' �ב֐U��
                '            ' ���ωȖڂ̔���
                '            Select Case Key.TUKEKAMOKU
                '                Case "01", "02"  ' �����A����

                '                    ' �ʒi�o���̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                    ' �ב֐U���̃f�[�^�쐬
                '                    If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                        ' �������σf�[�^�̃��X�g�쐬
                '                        lstKessaiData.Add(KData)
                '                    Else
                '                        errFlg = True
                '                    End If

                '                Case Else   ' ��L�ȊO�̏ꍇ
                '                    errFlg = True
                '                    errMsg &= "(���ωȖ�)"
                '            End Select

                '        Case "03"    ' �ב֕t��
                '            '*** �C�� mitsu 2008/06/25 �Ȗڂ̔���͂��Ȃ� ***
                '            '' ���ωȖڂ̔���
                '            'Select Case Key.TUKEKAMOKU
                '            '    Case "01", "02"  ' �����A����
                '            '************************************************
                '            ' �ʒi�o���̃f�[�^�쐬
                '            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                ' �������σf�[�^�̃��X�g�쐬
                '                lstKessaiData.Add(KData)
                '            Else
                '                errFlg = True
                '            End If

                '            ' �ב֕t�ւ̃f�[�^�쐬
                '            If errFlg = False AndAlso fn_KAWASE_TUKEKAE(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                ' �������σf�[�^�̃��X�g�쐬
                '                lstKessaiData.Add(KData)
                '            Else
                '                errFlg = True
                '            End If

                '        Case "04"    ' �ʒi�o���̂� �����̋敪�͋C�ɂ����쐬�H

                '            ' �ʒi�o���̃f�[�^�쐬
                '            If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                '                ' �������σf�[�^�̃��X�g�쐬
                '                lstKessaiData.Add(KData)
                '            Else
                '                errFlg = True
                '            End If

                '        Case "05"    ' ���ʊ��
                '            ' �����ΏۊO

                '        Case "99"    ' ���ϑΏۊO
                '            ' �����ΏۊO

                '        Case Else   ' ��L�ȊO�̏ꍇ
                '            errFlg = True
                '            errMsg &= "(���ϋ敪)"
                '    End Select
                'End If
                ''********************************************************************************
                '***�萔���݂̂̓d���͍쐬���Ȃ� 2009.10.28 end

            Else ' ��L�ȊO�̏ꍇ
                errFlg = True
                errMsg &= "(�萔�������敪)"
            End If

            '*** �C�� mitsu 2008/05/30 �萔�������敪����R�����g�A�E�g ***
            '    Case "2"    ' ���ʖƏ��̏ꍇ
            '' �����ΏۊO

            '    Case "3"    ' �ʓr�����̏ꍇ
            '' �����ΏۊO

            '    Case Else   ' ��L�ȊO�̏ꍇ
            'errFlg = True
            'errMsg &= "(�萔�������敪)"
            'End Select
            '**************************************************************

            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- START
            If strKEKKA <> "���z�O" Then
                MData.msg_DATA = ""
            Else
                MData.msg_DATA = "***�萔��������***"
            End If

            For cnt As Integer = 1 To OP_Count
                lstMsgData.Add(MData)
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                lstHimokuName.Add("")
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            Next
            ' 2016/06/13 �^�X�N�j���� ADD �yPG�zUI-03-12(RSV2�Ή�<���l�M��>) -------------------- END

            If errFlg = True Then
                jobMessage = errMsg & " ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE &
                    " �萔�������敪�F" & Key.TESUUTYO_KBN & " ���ϋ敪�F" & Key.KESSAI_KBN & " �萔���������@�F" & Key.TESUUTYO_PATN & " ���ωȖځF" & Key.TUKEKAMOKU
                MainLOG.Write("�������σf�[�^�擾����(��Ǝ��U�p)", "���s", jobMessage)
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("�������σf�[�^�擾����(��Ǝ��U�p)", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �������σf�[�^�擾����(�w�Z���U�p)
    '
    ' �����@ �F 
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
    Private Function fn_GetGakKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByVal lstHimokuData As ArrayList, _
                                        ByRef lstMsgData As ArrayList, ByRef lstHimokuName As ArrayList) As Boolean
        'Private Function fn_GetGakKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As ArrayList, ByVal lstHimokuData As ArrayList) As Boolean
        '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        Dim errFlg As Boolean = False
        Dim errMsg As String = "���Ϗ��Ɍ�肪����܂��B"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing
        Dim HimoKey As KeyInfo
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
        Dim MData As msgDATA = Nothing
        Dim OP_Count As Integer = 0
        lstMsgData = New ArrayList(128)
        lstHimokuName = New ArrayList(128)
        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

        strKEKKA = ""
        lstKessaiData = New ArrayList(128)

        Try
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            MData.Init()
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

            ' �萔�������σt���O������������
            Key.TESUUTYO_FLG = "0"

            ' �萔�������敪�� 0:�s�x�����A���A���ϗ\����Ǝ萔�������\����������Ώۓ��̏ꍇ
            If Key.TESUUTYO_KBN = "0" And Key.KESSAI_YDATE = ParaKessaiDate And Key.TESUU_YDATE = ParaKessaiDate Then

                ' ���ϋ敪�̔���
                Select Case Key.KESSAI_KBN
                    Case "00"    ' ���񂫂񒆋��a����
                        errFlg = True
                        errMsg &= "(���ϋ敪)"

                    Case "01"    ' ��������

                        ' �萔���������@�̔���
                        Select Case Key.TESUUTYO_PATN
                            Case "1"    ' ���ړ���

                                ' �ʒi�o���̃f�[�^�쐬
                                If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                    ' �������σf�[�^�̃��X�g�쐬
                                    lstKessaiData.Add(KData)
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                    OP_Count += 1
                                    lstHimokuName.Add("")
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                Else
                                    errFlg = True
                                End If

                                For Count As Integer = 0 To lstHimokuData.Count - 1
                                    HimoKey = CType(lstHimokuData.Item(Count), KeyInfo)

                                    ' ��ڌ����P�ʂ̌��Ϗ����擾
                                    Key.TUKEKIN_NO = HimoKey.KESSAI_KIN_CODE
                                    Key.TUKESIT_NO = HimoKey.KESSAI_TENPO
                                    Key.TUKEKAMOKU = HimoKey.KESSAI_KAMOKU
                                    Key.KESSAI_MEIGI = HimoKey.KESSAI_MEIGI
                                    Key.TUKEKOUZA = HimoKey.KESSAI_KOUZA
                                    Key.FURI_KIN = HimoKey.HIMOKU_FURI_KIN
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                    Key.HIMOKU_NAME = HimoKey.HIMOKU_NAME
                                    Key.HIMOKU_GASSAN = HimoKey.HIMOKU_GASSAN
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                                    '���Z�@�փR�[�h�Ō����������ב֐U��������
                                    Select Case Key.TUKEKIN_NO
                                        Case ini_info.JIKINKO_CODE      '��������
                                            ' ���ωȖڂ̔���
                                            Select Case Key.TUKEKAMOKU
                                                Case "02"   ' ����

                                                    ' ���ʓ����̃f�[�^�쐬
                                                    If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' �������σf�[�^�̃��X�g�쐬
                                                        lstKessaiData.Add(KData)
                                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                        OP_Count += 1
                                                        lstHimokuName.Add(Key.HIMOKU_NAME)
                                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                    Else
                                                        errFlg = True
                                                    End If

                                                Case "01"   ' ����

                                                    ' ���������̃f�[�^�쐬
                                                    If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' �������σf�[�^�̃��X�g�쐬
                                                        lstKessaiData.Add(KData)
                                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                        OP_Count += 1
                                                        lstHimokuName.Add(Key.HIMOKU_NAME)
                                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                    Else
                                                        errFlg = True
                                                    End If

                                                Case Else   ' ��L�ȊO�̏ꍇ
                                                    errFlg = True
                                                    errMsg &= "(���ωȖ�)"
                                            End Select
                                        Case Else                       '�ב֐U��

                                            ' ���ωȖڂ̔���
                                            Select Case Key.TUKEKAMOKU
                                                Case "01", "02"  ' �����A����
                                                    ' �ב֐U���̃f�[�^�쐬
                                                    If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                        ' �������σf�[�^�̃��X�g�쐬
                                                        lstKessaiData.Add(KData)
                                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                        OP_Count += 1
                                                        lstHimokuName.Add(Key.HIMOKU_NAME)
                                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                    Else
                                                        errFlg = True
                                                    End If

                                                Case Else   ' ��L�ȊO�̏ꍇ
                                                    errFlg = True
                                                    errMsg &= "(���ωȖ�)"
                                            End Select

                                    End Select
                                Next

                                ' �U�֍ϋ��z�Ǝ萔�����z�̍��z���O�~���̏ꍇ�̂݁A�萔���f�[�^�쐬
                                If strKEKKA <> "���z�O" Then

                                    Select Case ini_info.TESUU_OPEKBN   '1:�萔�������@2:������

                                        Case "1"
                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' �������σf�[�^�̃��X�g�쐬
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If

                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                            If CLng(Key.TESUU_KIN3) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_RENDO(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' �������σf�[�^�̃��X�g�쐬
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If

                                        Case "2"
                                            ' ���U�萔�����O�~���̏ꍇ�̂݁A���U�萔�������i�A���j�̃f�[�^�쐬
                                            If CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 0) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' �������σf�[�^�̃��X�g�쐬
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If

                                            ' �U���萔�����O�~���̏ꍇ�̂݁A�U���萔�������i�A���j�̃f�[�^�쐬
                                            If CLng(Key.TESUU_KIN3) > 0 Then
                                                If errFlg = False AndAlso fn_TESUUIN_SYOKANJOIN(Key, KData, 1) = 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                    ' �������σf�[�^�̃��X�g�쐬
                                                    lstKessaiData.Add(KData)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                    OP_Count += 1
                                                    lstHimokuName.Add(Key.HIMOKU_NAME)
                                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                Else
                                                    errFlg = True
                                                End If
                                            End If
                                        Case Else
                                            errFlg = True
                                            errMsg &= "(�萔�������I�y�敪(ini�t�@�C��))"
                                    End Select
                                End If

                            Case Else   ' ��L�ȊO�̏ꍇ
                                errFlg = True
                                errMsg &= "(�萔���������@)"
                        End Select

                    Case "02"    ' �ב֐U��
                        errFlg = True
                        errMsg &= "(���ϋ敪)"

                    Case "03"    ' �ב֕t��
                        errFlg = True
                        errMsg &= "(���ϋ敪)"

                    Case "04"    ' �ʒi�o���̂� 
                        errFlg = True
                        errMsg &= "(���ϋ敪)"

                    Case "05"    ' ���ʊ��
                        ' �����ΏۊO

                    Case "99"    ' ���ϑΏۊO
                        ' �����ΏۊO

                    Case Else   ' ��L�ȊO�̏ꍇ
                        errFlg = True
                        errMsg &= "(���ϋ敪)"
                End Select


            ElseIf Key.TESUUTYO_KBN = "0" OrElse Key.TESUUTYO_KBN = "1" OrElse Key.TESUUTYO_KBN = "2" OrElse Key.TESUUTYO_KBN = "3" Then

                ' ���ϗ\����������Ώۓ��̏ꍇ
                If Key.KESSAI_YDATE = ParaKessaiDate Then
                    ' ���ϋ敪�̔���
                    Select Case Key.KESSAI_KBN
                        Case "00"    ' ���񂫂񒆋��a����
                            errFlg = True
                            errMsg &= "(���ϋ敪)"

                        Case "01"    ' ��������

                            ' �萔���������@�̔���
                            Select Case Key.TESUUTYO_PATN
                                Case "1"    ' ���ړ���

                                    ' �ʒi�o���̃f�[�^�쐬
                                    If errFlg = False AndAlso fn_BETUDAN_OUT(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                        ' �������σf�[�^�̃��X�g�쐬
                                        lstKessaiData.Add(KData)
                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                        OP_Count += 1
                                        lstHimokuName.Add("")
                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                    Else
                                        errFlg = True
                                    End If

                                    For Count As Integer = 0 To lstHimokuData.Count - 1
                                        HimoKey = CType(lstHimokuData.Item(Count), KeyInfo)

                                        ' ��ڌ����P�ʂ̌��Ϗ����擾
                                        Key.TUKEKIN_NO = HimoKey.KESSAI_KIN_CODE
                                        Key.TUKESIT_NO = HimoKey.KESSAI_TENPO
                                        Key.TUKEKAMOKU = HimoKey.KESSAI_KAMOKU
                                        Key.KESSAI_MEIGI = HimoKey.KESSAI_MEIGI
                                        Key.TUKEKOUZA = HimoKey.KESSAI_KOUZA
                                        Key.FURI_KIN = HimoKey.HIMOKU_FURI_KIN
                                        '************************************************
                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                        Key.HIMOKU_NAME = HimoKey.HIMOKU_NAME
                                        Key.HIMOKU_GASSAN = HimoKey.HIMOKU_GASSAN
                                        '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                                        '���Z�@�փR�[�h�Ō����������ב֐U��������
                                        Select Case Key.TUKEKIN_NO
                                            Case ini_info.JIKINKO_CODE      '��������
                                                ' ���ωȖڂ̔���
                                                Select Case Key.TUKEKAMOKU
                                                    Case "02"   ' ����

                                                        ' ���ʓ����̃f�[�^�쐬
                                                        If errFlg = False AndAlso fn_FUTUU_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                            OP_Count += 1
                                                            lstHimokuName.Add(Key.HIMOKU_NAME)
                                                            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                        Else
                                                            errFlg = True
                                                        End If

                                                    Case "01"   ' ����

                                                        ' ���������̃f�[�^�쐬
                                                        If errFlg = False AndAlso fn_TOUZA_IN(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                            OP_Count += 1
                                                            lstHimokuName.Add(Key.HIMOKU_NAME)
                                                            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                        Else
                                                            errFlg = True
                                                        End If

                                                    Case Else   ' ��L�ȊO�̏ꍇ
                                                        errFlg = True
                                                        errMsg &= "(���ωȖ�)"
                                                End Select
                                            Case Else                       '�ב֐U��

                                                ' ���ωȖڂ̔���
                                                Select Case Key.TUKEKAMOKU
                                                    Case "01", "02"  ' �����A����
                                                        ' �ב֐U���̃f�[�^�쐬
                                                        If errFlg = False AndAlso fn_KAWASE_FURIKOMI(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                                                            ' �������σf�[�^�̃��X�g�쐬
                                                            lstKessaiData.Add(KData)
                                                            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                                            OP_Count += 1
                                                            lstHimokuName.Add(Key.HIMOKU_NAME)
                                                            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
                                                        Else
                                                            errFlg = True
                                                        End If

                                                    Case Else   ' ��L�ȊO�̏ꍇ
                                                        errFlg = True
                                                        errMsg &= "(���ωȖ�)"
                                                End Select

                                        End Select
                                    Next

                                Case Else   ' ��L�ȊO�̏ꍇ
                                    errFlg = True
                                    errMsg &= "(�萔���������@)"
                            End Select

                        Case "02"    ' �ב֐U��
                            errFlg = True
                            errMsg &= "(���ϋ敪)"

                        Case "03"    ' �ב֕t��
                            errFlg = True
                            errMsg &= "(���ϋ敪)"

                        Case "04"    ' �ʒi�o���̂�
                            errFlg = True
                            errMsg &= "(���ϋ敪)"

                        Case "05"    ' ���ʊ��
                            ' �����ΏۊO

                        Case "99"    ' ���ϑΏۊO
                            ' �����ΏۊO

                        Case Else   ' ��L�ȊO�̏ꍇ
                            errFlg = True
                            errMsg &= "(���ϋ敪)"
                    End Select
                End If

            Else ' ��L�ȊO�̏ꍇ
                errFlg = True
                errMsg &= "(�萔�������敪)"
            End If

            '*** �C�� mitsu 2008/05/30 �萔�������敪����R�����g�A�E�g ***
            '    Case "2"    ' ���ʖƏ��̏ꍇ
            '' �����ΏۊO

            '    Case "3"    ' �ʓr�����̏ꍇ
            '' �����ΏۊO

            '    Case Else   ' ��L�ȊO�̏ꍇ
            'errFlg = True
            'errMsg &= "(�萔�������敪)"
            'End Select
            '**************************************************************

            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            If strKEKKA <> "���z�O" Then
                MData.msg_DATA = ""
            Else
                MData.msg_DATA = "***�萔��������***"
            End If

            For cnt As Integer = 1 To OP_Count
                lstMsgData.Add(MData)
            Next
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

            ' �G���[�t���O���n�m�̏ꍇ
            If errFlg = True Then
                jobMessage = errMsg & " ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE &
                    " �萔�������敪�F" & Key.TESUUTYO_KBN & " ���ϋ敪�F" & Key.KESSAI_KBN & " �萔���������@�F" & Key.TESUUTYO_PATN & " ���ωȖځF" & Key.TUKEKAMOKU
                MainLOG.Write("�������σf�[�^�擾����(�w�Z���U�p)", "���s", jobMessage)
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("�������σf�[�^�擾����(�w�Z���U�p)", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �������σf�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_KessaiData(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean

        Dim strKamokuOpeCode As String
        Dim strTUKEKIN_NO As String
        Dim strTUKESIT_NO As String
        Dim strHONBUCD As String

        Try
            strKamokuOpeCode = KData.OpeCode

            ' �Ȗڂ� 48100:�ב֐U�ցA���́A48500:�ב֕t�ւ̏ꍇ�̂ݐݒ�i�בֈȊO�͏����l�j
            If strKamokuOpeCode = "48100" Or strKamokuOpeCode = "48500" Then
                strTUKEKIN_NO = Key.TUKEKIN_NO          ' ��M���Z�@�փR�[�h
                strTUKESIT_NO = Key.TUKESIT_NO          ' ��M�X��
                strHONBUCD = ini_info.HONBU_CODE              ' ���M�X��
            Else
                strTUKEKIN_NO = "".PadLeft(4, "0"c)     ' ��M���Z�@�փR�[�h
                strTUKESIT_NO = "".PadLeft(3, "0"c)     ' ��M�X��
                strHONBUCD = "".PadLeft(3, "0"c)        ' ���M�X��
            End If

            ' �f�[�^�ݒ�
            With KData
                .KessaiDate = ParaKessaiDate                            '���ϓ�
                .TorisCode = Key.TORIS_CODE                              '������R�[�h
                .TorifCode = Key.TORIF_CODE                              '����敛�R�[�h
                .ToriKName = Key.ITAKU_KNAME
                .ToriNName = Key.ITAKU_NNAME
                .FuriCode = Key.FURI_CODE                                '�U�փR�[�h
                .KigyoCode = Key.KIGYO_CODE                              '��ƃR�[�h
                .FuriDate = Key.FURI_DATE                                '�U�֓�
                .KessaiKbn = Key.KESSAI_KBN                              '���ϋ敪
                .KesKinCode = Key.TUKEKIN_NO                        '���ϋ��Z�@�փR�[�h
                .KesSitCode = Key.TUKESIT_NO                           '���ώx�X�R�[�h
                .KesKamoku = Key.TUKEKAMOKU                           '���ωȖ�
                .KesKouza = Key.TUKEKOUZA                             '���ό����ԍ�
                .TesTyoKbn = Key.TESUUTYO_KBN                            '�萔�������敪
                .TesTyohh = Key.TESUUTYO_PATN                            '�萔���������@
                .TorimatomeSit = Key.TORIMATOME_SIT_NO                   '�Ƃ�܂ƂߓX�R�[�h
                .SyoriKen = Key.SYORI_KEN                                   '��������
                .Syorikin = Key.SYORI_KIN                                   '�������z
                .FunouKen = Key.FUNOU_KEN                                   '�s�\����
                .FunouKin = Key.FUNOU_KIN                                   '�s�\���z
                .FuriKen = Key.FURI_KEN                                     '�U�֌���
                .FuriKin = Key.FURI_KIN                                     '�U�֋��z
                .TesuuKin = Key.TESUU_KIN                                   '�萔��
                .JifutiTesuuKin = Key.TESUU_KIN1                            '���U�萔��
                .FurikomiTesuukin = Key.TESUU_KIN3                          '�U���萔��
                .SonotaTesuuKin = Key.TESUU_KIN2                            '���̑��萔��
                .NyukinKen = Key.FURI_KEN                               '��������
                .NyukinKin = ""                                         '�������z
                .ToriKbn = Key.KESSAI_TORI_KBN
                .TesuuTyoFlg = Key.TESUUTYO_FLG
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                .HonbuKouza = Key.HONBU_KOUZA                               '�{���ʒi�����ԍ�
                '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            End With

            ' �Œ蒷�ɕϊ�����
            KData.Data = KData.Data

        Catch ex As Exception
            MainLOG.Write("�������σf�[�^�쐬", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �ʒi�o���f�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F 0 - ���z�O�~���C1 - ���z�O�~�C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_BETUDAN_OUT(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim BetudanOutFmt As New CAstFormKes.ClsFormSikinFuri.T_04099
        Dim strKin As String
        Dim strSagaku As String
        Dim strTEKIYOU As String

        Try
            ' ������
            BetudanOutFmt.Init()

            ' �f�[�^�`�F�b�N
            If Key.HONBU_KOUZA = "" OrElse CInt(Key.HONBU_KOUZA) = 0 Then
                '���ݒ�̏ꍇ�A���́A�I�[���O�̏ꍇ�A�G���[�Ƃ���
                MainLOG.Write("�ʒi�o���f�[�^�쐬", "���s", "�ʒi�����ԍ����ݒ肳��Ă��܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' �E�v�̐ݒ�
            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            '���ώ�ʂ� 1�F��ڌ����P�ʐ��� �̑Ή���ǉ�
            If Key.KESSAI_SYUBETU = "0" Then
                ' ���`�l�J�i���ݒ�ς݂̏ꍇ�A���`�l�J�i��D��
                If Key.TUKEMEIGI <> "" Then
                    strTEKIYOU = Key.TUKEMEIGI
                Else
                    strTEKIYOU = Key.ITAKU_KNAME
                End If
            Else
                ' ���ώ�ʂ� 1�F��ڌ����P�ʐ��� �̏ꍇ�����͌Œ�
                strTEKIYOU = Key.FURI_DATE.Substring(4, 2) & "/" & Key.FURI_DATE.Substring(6, 2) & "*****1��"
            End If
            '' ���`�l�J�i���ݒ�ς݂̏ꍇ�A���`�l�J�i��D��
            'If Key.TUKEMEIGI <> "" Then
            '    strTEKIYOU = Key.TUKEMEIGI
            'Else
            '    strTEKIYOU = Key.ITAKU_KNAME
            'End If
            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

            If strTEKIYOU = "" Then
                MainLOG.Write("�ʒi�o���f�[�^�쐬", "���s", "�E�v������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            '�U���ϋ��z�Ǝ萔���z�̍��z
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' �萔���������z���O�~�ȉ��ƂȂ�ꍇ�͎萔�������������Ȃ�
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "���z�O"
            End If

            ' ���z�̐ݒ�
            ' �s�x�����A���A���������ȊO�͏o���z�Ɏ萔�������������Ȃ�
            '99-019 �̏ꍇ�͍��������Ȃ� 2009.10.30
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" And ini_info.TESUU_OPEKBN = "1" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' �f�[�^�ݒ�
            With BetudanOutFmt
                .KOUZA_NO = Key.HONBU_KOUZA.PadLeft(7, "0"c)        ' �����ԍ�
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' ���z
                .FURI_CODE = ""                                     ' �U�փR�[�h
                .KIGYO_CODE = ""                                    ' ��ƃR�[�h
                .TEKIYOU = strTEKIYOU                               ' �E�v
                .TORIATUKAI1 = ""                                   ' �戵�ԍ��P
                '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------START
                '�����ǉ�
                Select Case Key.SEIKYU_KBN
                    Case "0"
                        '����������
                        .KENSU = Key.SYORI_KEN.PadLeft(4, " "c)
                    Case "1"
                        '�U�֕�����
                        .KENSU = Key.FURI_KEN.PadLeft(4, " "c)
                    Case Else
                        '���̑�
                        .KENSU = ""
                End Select
                '.KENSU = ""                                         ' ����
                '2011/06/16 �W���ŏC�� �萔�������敪���猏���擾 ------------------END
                .TEGATA_NO = ""                                     ' ��`���؎�ԍ�
                .GENTEN_NO = ""                                     ' ���X�ԍ�
                .KISANBI = ""                                       ' �N�Z��
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = BetudanOutFmt.Data
            KData.OpeCode = String.Concat(BetudanOutFmt.KAMOKU_CODE, BetudanOutFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("�ʒi�o���f�[�^�쐬", "���s", ex.Message)

            Return -1
        End Try

        If strKEKKA = "���z�O" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' �@�\�@ �F ���������f�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F 0 - ���z�O�~���C1 - ���z�O�~�C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_TOUZA_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim TouzaInFmt As New CAstFormKes.ClsFormSikinFuri.T_01010
        Dim strKin As String
        Dim strSagaku As String
        Dim strGENTEN_NO As String
        Dim strTEKIYOU As String
        Dim strFURIKOMI_IRAI As String

        Try
            ' ������
            TouzaInFmt.Init()

            ' �f�[�^�`�F�b�N
            If Key.TUKEKOUZA = "" OrElse CInt(Key.TUKEKOUZA) = 0 Then
                MainLOG.Write("���������f�[�^�쐬", "���s", "�����ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If Key.FURI_DATE.Length <> 8 Or CLng(Key.FURI_KEN) = 0 Then
                MainLOG.Write("���������f�[�^�쐬", "���s", "�E�v������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If Key.TUKESIT_NO.Trim = "" OrElse CInt(Key.TUKESIT_NO) = 0 Then
                MainLOG.Write("���������f�[�^�쐬", "���s", "���X�ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            '�U���ϋ��z�Ǝ萔���z�̍��z
            ' ���ώ�ʂ� 1�F��ڌ����P�ʐ��� �̏ꍇ�A�萔���Ƃ̍��z�͎Z�o���Ȃ�
            If Key.KESSAI_SYUBETU = "0" Then
                strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))
            Else
                strSagaku = Key.FURI_KIN
            End If

            ' �萔���������z���O�~�ȉ��ƂȂ�ꍇ�͎萔�������������Ȃ�
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "���z�O"
            End If

            ' ���z�̐ݒ�
            ' �s�x�����A���A���������ȊO�͓����z�Ɏ萔�������������Ȃ�
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' �E�v�̐ݒ�
            If Key.KESSAI_SYUBETU = "0" Then
                ' ���ώ�ʂ� 0�F�ϑ��҈ꊇ���� �̏ꍇ
                strTEKIYOU = "����" & Key.FURI_KEN.PadRight(6, " "c).Trim & "��"
            Else

                ' ���ώ�ʂ� 1�F��ڌ����P�ʐ��� �̏ꍇ
                strTEKIYOU = Key.KESSAI_MEIGI
            End If

            ' �U���˗��l���̐ݒ�
            Select Case Key.FURI_CODE
                Case "040", "041", "042"
                    strFURIKOMI_IRAI = ""
                Case Else
                    strFURIKOMI_IRAI = Key.TUKEMEIGI
            End Select

            ' ���X�ԍ��̐ݒ�
            ' ���ώx�X��ini�t�@�C���̖{���R�[�h���r
            '   ��v����ꍇ�F��
            '   �قȂ�ꍇ  �F���ώx�X��ݒ�
            If Key.TUKESIT_NO = ini_info.HONBU_CODE Then
                strGENTEN_NO = ""
            Else
                strGENTEN_NO = Key.TUKESIT_NO.PadLeft(3, "0"c)
            End If

            ' �f�[�^�ݒ�
            With TouzaInFmt
                .KOUZA_NO = Key.TUKEKOUZA.PadLeft(7, "0"c)          ' �����ԍ�
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' ���z
                .SIKINKA_KBN = ""                                   ' �������敪�R�[�h
                .TATEN_TEKIYOU = ""                                 ' ���X���E�v
                .FURI_CODE = ""                                     ' �U�փR�[�h
                .TEKIYOU = strTEKIYOU                               ' �E�v
                .TESUU_KBN = ""                                     ' �萔�������敪
                .TESUU_KINGAKU = ""                                 ' �萔���z
                .KISANBI = ""                                       ' �N�Z��
                .SAKIHIDUKE_YOTEI = ""                              ' ����t�\���
                .FURIKOMI_IRAI = ""                                 ' �U���˗��l��
                .GENTEN_NO = strGENTEN_NO                           ' ���X�ԍ�
                .KINGAKU1 = ""                                      ' ���z�P
                .SIKINKA_KBN1 = ""                                  ' �������敪�R�[�h�P
                .TATEN_TEKIYOU1 = ""                                ' ���X���E�v�P
                .KINGAKU2 = ""                                      ' ���z�Q
                .SIKINKA_KBN2 = ""                                  ' �������敪�R�[�h�Q
                .TATEN_TEKIYO2 = ""                                 ' ���X���E�v�Q
                .KINGAKU3 = ""                                      ' ���z�R
                .SIKINKA_KBN3 = ""                                  ' �������敪�R�[�h�R
                .TATEN_TEKIYO3 = ""                                 ' ���X���E�v�R
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = TouzaInFmt.Data
            KData.OpeCode = String.Concat(TouzaInFmt.KAMOKU_CODE, TouzaInFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("���������f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        If strKEKKA = "���z�O" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' �@�\�@ �F ���ʓ����f�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F 0 - ���z�O�~���C1 - ���z�O�~�C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_FUTUU_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim FutuuInFmt As New CAstFormKes.ClsFormSikinFuri.T_02019
        Dim strKin As String
        Dim strSagaku As String
        Dim strGENTEN_NO As String
        Dim strTEKIYOU As String
        Dim strFURIKOMI_IRAI As String

        Try
            ' ������
            FutuuInFmt.Init()

            ' �f�[�^�`�F�b�N
            If Key.TUKEKOUZA = "" OrElse CInt(Key.TUKEKOUZA) = 0 Then
                MainLOG.Write("���ʓ����f�[�^�쐬", "���s", "�����ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If Key.FURI_DATE.Length <> 8 OrElse CLng(Key.FURI_KEN) = 0 Then
                MainLOG.Write("���ʓ����f�[�^�쐬", "���s", "�E�v������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If Key.TUKESIT_NO.Trim = "" Then
                MainLOG.Write("���ʓ����f�[�^�쐬", "���s", "���X�ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            '�U���ϋ��z�Ǝ萔���z�̍��z
            ' ���ώ�ʂ� 1�F��ڌ����P�ʐ��� �̏ꍇ�A�萔���Ƃ̍��z�͎Z�o���Ȃ�
            If Key.KESSAI_SYUBETU = "0" Then
                strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))
            Else
                strSagaku = Key.FURI_KIN
            End If

            ' �萔���������z���O�~�ȉ��ƂȂ�ꍇ�͎萔�������������Ȃ�
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "���z�O"
            End If

            ' ���z�̐ݒ�
            ' �s�x�����A���A���������ȊO�͓����z�Ɏ萔�������������Ȃ�
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' �E�v�̐ݒ�
            If Key.KESSAI_SYUBETU = "0" Then
                ' ���ώ�ʂ� 0�F�ϑ��҈ꊇ���� �̏ꍇ
                strTEKIYOU = "����" & Key.FURI_KEN.PadRight(6, " "c).Trim & "��"
            Else

                ' ���ώ�ʂ� 1�F��ڌ����P�ʐ��� �̏ꍇ
                strTEKIYOU = Key.KESSAI_MEIGI
            End If

            ' �U���˗��l���̐ݒ�
            Select Case Key.FURI_CODE
                Case "040", "041", "042"
                    strFURIKOMI_IRAI = ""
                Case Else
                    strFURIKOMI_IRAI = Key.TUKEMEIGI
            End Select

            ' ���X�ԍ��̐ݒ�
            ' ���ώx�X��ini�t�@�C���̖{���R�[�h���r
            '   ��v����ꍇ�F��
            '   �قȂ�ꍇ  �F���ώx�X��ݒ�
            If Key.TUKESIT_NO = ini_info.HONBU_CODE Then
                strGENTEN_NO = ""
            Else
                strGENTEN_NO = Key.TUKESIT_NO.PadLeft(3, "0"c)
            End If

            With FutuuInFmt
                .KOUZA_NO = Key.TUKEKOUZA.PadLeft(7, "0"c)          ' �����ԍ�
                .GYO = "01"                                         ' �s
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' ���z
                .SIKINKA_KBN = ""                                   ' �������敪�R�[�h
                .TATEN_TEKIYOU = ""                                 ' ���X���E�v
                .FURI_CODE = ""                                     ' �U�փR�[�h
                .TEKIYOU = strTEKIYOU                               ' �E�v
                .TESUU_KBN = ""                                     ' �萔�������敪
                .TESUU_KINGAKU = ""                                 ' �萔���z
                .KISANBI = ""                                       ' �N�Z��
                .SAKIHIDUKE_YOTEI = ""                              ' ����t�\���
                .FURIKOMI_IRAI = ""                                 ' �U���˗��l��
                .GENTEN_NO = strGENTEN_NO                           ' ���X�ԍ�
                .KINGAKU1 = ""                                      ' ���z�P
                .SIKINKA_KBN1 = ""                                  ' �������敪�R�[�h�P
                .TATEN_TEKIYOU1 = ""                                ' ���X���E�v�P
                .KINGAKU2 = ""                                      ' ���z�Q
                .SIKINKA_KBN2 = ""                                  ' �������敪�R�[�h�Q
                .TATEN_TEKIYO2 = ""                                 ' ���X���E�v�Q
                .KINGAKU3 = ""                                      ' ���z�R
                .SIKINKA_KBN3 = ""                                  ' �������敪�R�[�h�R
                .TATEN_TEKIYO3 = ""                                 ' ���X���E�v�R
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = FutuuInFmt.Data
            KData.OpeCode = String.Concat(FutuuInFmt.KAMOKU_CODE, FutuuInFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("���ʓ����f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        If strKEKKA = "���z�O" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' �@�\�@ �F �ʒi�����f�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F 0 - ���z�O�~���C1 - ���z�O�~�C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_BETUDAN_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim BetudanInFmt As New CAstFormKes.ClsFormSikinFuri.T_04019
        Dim strKin As String
        Dim strSagaku As String
        Dim strTEKIYOU As String
        Dim strGENTEN_NO As String

        Try
            ' ������
            BetudanInFmt.Init()

            ' �f�[�^�`�F�b�N
            If Key.TUKEKOUZA = "" OrElse CInt(Key.TUKEKOUZA) = 0 Then
                ' ���ݒ�̏ꍇ�A���́A�I�[���O�̏ꍇ�A�G���[�Ƃ���B
                MainLOG.Write("�ʒi�����f�[�^�쐬", "���s", "�����ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' �E�v�̐ݒ�
            ' ���`�l�J�i���ݒ�ς݂̏ꍇ�A���`�l�J�i��D��
            If Key.TUKEMEIGI <> "" Then
                strTEKIYOU = Key.TUKEMEIGI
            Else
                strTEKIYOU = Key.ITAKU_KNAME
            End If

            If strTEKIYOU = "" Then
                MainLOG.Write("�ʒi�����f�[�^�쐬", "���s", "�E�v������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If Key.TUKESIT_NO.Trim = "" Then
                MainLOG.Write("�ʒi�����f�[�^�쐬", "���s", "���X�ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            '�U���ϋ��z�Ǝ萔���z�̍��z
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' �萔���������z���O�~�ȉ��ƂȂ�ꍇ�͎萔�������������Ȃ�
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "���z�O"
            End If

            ' ���z�̐ݒ�
            ' �s�x�����A���A���������ȊO�͓������z�Ɏ萔�������������Ȃ�
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' ���X�ԍ��̐ݒ�
            ' ���ώx�X��ini�t�@�C���̖{���R�[�h���r
            '   ��v����ꍇ�F��
            '   �قȂ�ꍇ  �F���ώx�X��ݒ�
            If Key.TUKESIT_NO = ini_info.HONBU_CODE Then
                strGENTEN_NO = ""
            Else
                strGENTEN_NO = Key.TUKESIT_NO.PadLeft(3, "0"c)
            End If

            ' �f�[�^�ݒ�
            With BetudanInFmt
                .KOUZA_NO = Key.TUKEKOUZA.PadLeft(7, "0"c)          ' �����ԍ�
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' ���z
                .SIKINKA_KBN = ""                                   ' �������敪�R�[�h
                .TATEN_TEKIYOU = ""                                 ' ���X���E�v
                .FURI_CODE = ""                                     ' �U�փR�[�h
                .KIGYO_CODE = ""                                    ' ��ƃR�[�h
                .TEKIYOU = strTEKIYOU                               ' �E�v
                .TORIATUKAI1 = ""                                   ' �戵�ԍ��Q
                .KENSU = ""                                         ' ����
                .MADOGUTI_KBN = ""                                  ' �������[�敪
                .INSI_KEN = ""                                      ' �󎆌���
                .TEGATA_NO = ""                                     ' ��`���؎�ԍ�
                .HAKKOU_NO = ""                                     ' ���s��ڋq�ԍ�
                .TESUU_KBN = ""                                     ' �萔�������敪
                .TESUU_KINGAKU = ""                                 ' �萔���z
                .KISANBI = ""                                       ' �N�Z��
                .SAKIHIDUKE_YOTEI = ""                              ' ����t�\���
                .GENTEN_NO = strGENTEN_NO                           ' ���X�ԍ�
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = BetudanInFmt.Data
            KData.OpeCode = String.Concat(BetudanInFmt.KAMOKU_CODE, BetudanInFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("�ʒi�����f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        If strKEKKA = "���z�O" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' �@�\�@ �F ����������f�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F 0 - ���z�O�~���C1 - ���z�O�~�C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_SYOKANJO_IN(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer
        '*** �C�� mitsu 2008/10/10 99-019��99-011�ɕύX 99-019�ɕύXRSV1***
        Dim SyokanjyoInFmt As New CAstFormKes.ClsFormSikinFuri.T_99019
        'Dim SyokanjyoInFmt As CAstFormKes.ClsFormSikinFuri.T_99011
        '**************************************************
        Dim strKin As String
        Dim strSagaku As String
        Dim strKOUZA_NO As String
        Dim strUTIWAKE_CODE As String
        Dim strTEKIYOU As String

        Try
            ' ������
            SyokanjyoInFmt.Init()

            ' �f�[�^�`�F�b�N
            If Key.TUKEKOUZA = "" Then
                MainLOG.Write("����������f�[�^�쐬", "���s", "�����ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' �����ԍ��̐ݒ�i "00"�{���ό����ԍ��̏�T���j
            strKOUZA_NO = "00" & Key.TUKEKOUZA.Substring(0, 5)

            If CInt(strKOUZA_NO) = 0 Then
                MainLOG.Write("����������f�[�^�쐬", "���s", "�����ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' ����R�[�h�̐ݒ�i���ό����ԍ��̉��Q���j
            strUTIWAKE_CODE = Key.TUKEKOUZA.Substring(5, 2)

            If CInt(strUTIWAKE_CODE) = 0 Then
                MainLOG.Write("����������f�[�^�쐬", "���s", "����R�[�h���ݒ肳��Ă��܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If
            '2011/06/24 �W���ŏC�� ���`�l�J�i���ݒ�ς݂̏ꍇ�A���`�l�J�i��D�� ------------------START
            ' �E�v�̐ݒ�
            ' ���`�l�J�i���ݒ�ς݂̏ꍇ�A���`�l�J�i��D��
            ' *** �C�� nishida 2008/10/14 �E�v�͈ϑ��Җ���ݒ�
            If Key.TUKEMEIGI <> "" Then
                strTEKIYOU = Key.TUKEMEIGI
            Else
                strTEKIYOU = Key.ITAKU_KNAME
            End If
            '**************************************************
            '2011/06/24 �W���ŏC�� ���`�l�J�i���ݒ�ς݂̏ꍇ�A���`�l�J�i��D�� ------------------END

            If strTEKIYOU = "" Then
                MainLOG.Write("����������f�[�^�쐬", "���s", "�E�v������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            '�U���ϋ��z�Ǝ萔���z�̍��z
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' �萔���������z���O�~�ȉ��ƂȂ�ꍇ�͎萔�������������Ȃ�
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "���z�O"
            End If

            ' ���z�̐ݒ�
            ' �s�x�����A���A���������ȊO�͓����z�Ɏ萔�������������Ȃ�
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            With SyokanjyoInFmt
                .KOUZA_NO = strKOUZA_NO                             ' �����ԍ�
                .GYO = "01"                                         ' �s
                .UTIWAKE_CODE = strUTIWAKE_CODE                     ' ����R�[�h
                .ZENZAN = "0".PadLeft(15, " "c)                      ' �O�c
                .FUGOU_CODE = "1"                                   ' �����R�[�h
                .KINGAKU = strKin.PadLeft(13, " "c)                 ' ���z
                .TATEN_TEKIYOU = ""                                 ' ���X���E�v
                .KENSU = "1".PadLeft(5, " "c)                       ' ����
                .FURI_CODE = ""                                     ' �U�փR�[�h
                .TORIATUKAI1 = ""                                   ' �戵�ԍ��P
                .JINKAKU_CODE = ""                                  ' �l�i�R�[�h
                .KAZEI_CODE = ""                                    ' �ېŃR�[�h
                .TEKIYOU = strTEKIYOU                               ' �E�v
                .KISANBI = ""                                       ' �N�Z��
                .GENTEN_NO = ""                                     ' ���X�ԍ�
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = SyokanjyoInFmt.Data
            KData.OpeCode = String.Concat(SyokanjyoInFmt.KAMOKU_CODE, SyokanjyoInFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("����������f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        If strKEKKA = "���z�O" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' �@�\�@ �F �ב֐U���f�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F 0 - ���z�O�~���C1 - ���z�O�~�C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_KAWASE_FURIKOMI(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseFurikomiInFmt As New CAstFormKes.ClsFormSikinFuri.T_48100
        Dim strTUKEKIN_NO As String
        Dim strTUKESIT_NO As String
        Dim strKIN_NNM As String = ""                ' ���Z�@�֊�����
        Dim strSIT_NNM As String = ""                ' �x�X������
        Dim strKIN_KNM As String = ""                ' ���Z�@�փJ�i��
        Dim strSIT_KNM As String = ""               ' �x�X�J�i��
        '*** �C���@���l�Q�Ƃ�܂ƂߓX��ݒ肷�邽�߂̃��[�N�G���A�@2008/11/07 NISHIDA
        Dim strKIN_NNM2 As String = ""                ' ���Z�@�֊�����
        Dim strSIT_NNM2 As String = ""                ' �x�X������
        Dim strKIN_KNM2 As String = ""                ' ���Z�@�փJ�i��
        Dim strSIT_KNM2 As String = ""                ' �x�X�J�i��
        '*****************************************************************************
        Dim strSYUMOKU As String
        Dim strJUSIN_TEN As String
        Dim strHASSIN_TEN As String
        Dim strKin As String
        Dim strSagaku As String
        Dim strKINGAKU_LOCAL As String          ' ���z
        Dim strKINFUKKI_FUGOU As String = ""         ' ���z���L���� 
        Dim strUKETORI_KAMOKU As String
        Dim strBIKOU1 As String = "" '2011/06/17
        Dim strBIKOU2 As String = "" '2011/06/17
        Dim strTEKIYOU As String

        Try
            ' ������
            KawaseFurikomiInFmt.Init()

            ' ��M�X���̎擾
            strTUKEKIN_NO = Key.TUKEKIN_NO     ' ���ϋ��Z�@��
            strTUKESIT_NO = Key.TUKESIT_NO     ' ���ώx�X

            If GetTENMAST(strTUKEKIN_NO, strTUKESIT_NO, strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = False Then
                MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", "���Z�@�փR�[�h�捞�G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE & " ���Z�@�ցF" & Key.TUKEKIN_NO & Key.TUKESIT_NO)
                Return -1
            End If

            If Key.TUKEMEIGI = "" Then
                MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", "���l�����ݒ肳��Ă��܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' ��ڃR�[�h�̐ݒ�
            ' ���l�P�̏�U�����uߺ���߁v�A�uߺ���߁v�̏ꍇ�A�󔒂ɍĐݒ�
            strSYUMOKU = "1022"

            If Key.BIKOU1.Length >= 6 Then
                If Key.BIKOU1.Substring(0, 6) = "ߺ����" Then
                    strSYUMOKU = "1074"
                    Key.BIKOU1 = " "
                ElseIf Key.BIKOU1.Substring(0, 6) = "ߺ����" Then
                    strSYUMOKU = "1054"
                    Key.BIKOU1 = " "
                End If
            End If

            ' ��M�X���A���M�X���̐ݒ�
            If strTUKEKIN_NO = ini_info.JIKINKO_CODE Then         ' �{�x�X�בւ̏ꍇ�iINI�t�@�C���̎����ɃR�[�h�Ɠ���j
                strJUSIN_TEN = "� " & strSIT_KNM.Trim
                strHASSIN_TEN = "� ���-"
                strTEKIYOU = "����" & Key.FURI_KEN.PadRight(6, " "c).Trim & "��"
            Else                                            ' ���s�בւ̏ꍇ
                strJUSIN_TEN = strKIN_KNM.Trim & " " & strSIT_KNM.Trim
                strHASSIN_TEN = ini_info.KAWASE_CENTER
                strTEKIYOU = ini_info.SIKINTEKIYOU & " ����" & Key.FURI_KEN.PadRight(6, " "c).Trim & "��"
            End If

            '�U���ϋ��z�Ǝ萔���z�̍��z
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' �萔���������z���O�~�ȉ��ƂȂ�ꍇ�͎萔�������������Ȃ�
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "���z�O"
            End If

            ' ���z�̐ݒ�
            ' �s�x�����A���A���������ȊO�͓����z�Ɏ萔���������Ȃ��B
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' ���z���J���}�ҏW����
            strKINGAKU_LOCAL = CASTCommon.CADec(strKin).ToString("###,###,###,##0").PadLeft(10, " "c)

            ' ���z���L�L���̎擾
            If fn_FUGO_SETTEI(strKINGAKU_LOCAL, strKINFUKKI_FUGOU) = False Then
                MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", "���L�����ݒ菈���G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            '2017/05/29 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
            ' ���l�ȖڃR�[�h�̐ݒ�(�S��t�H�[�}�b�g�ɍ��킹��)
            ' 02�F���� �� 1
            ' 01�F���� �� 2
            ' xx�F���̑���? (���̑��̃R�[�h�l�́ACommon_���ωȖ�.TXT�ƉȖڕϊ��e�[�u��.ini���擾����)
            '2017/07/25 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
            'strUKETORI_KAMOKU = ConvertKamoku2TO1(KessaiKamokuCode_ETC)
            '----------------------------------------------------
            ' �ב֐U���́u���l�Ȗځv�ҏW�d�l�ύX
            '  �w�ב֐U���x���́u���̑��v�̎戵���\�Ƃ���
            '  �E�Ȗ�="01"(�M�� �����a��)�̏ꍇ�A"2"(�S�ⓖ��)
            '  �E�Ȗ�="02"(�M�� ���ʗa��)�̏ꍇ�A"1"(�S�╁��)
            '  �E�Ȗ�="xx"(�M�� ���̑�)�̏ꍇ�A�@"x"(�S�₻�̑�)
            '  �E��L�ȊO�̉Ȗڂ̏ꍇ�A�@�@�@�@�@"2"(�S�╁��)
            '�@�����̑��ȖڂɁuxx�v���ݒ肳��Ă���ꍇ�́A
            '�@�@���̑��Ȗږ��ݒ�Ƃ��A�W�����W�b�N�ŏ������s���B
            '----------------------------------------------------
            If KessaiKamokuCode_ETC = "xx" Then
                Select Case Key.TUKEKAMOKU.PadLeft(2, "0"c)
                    Case "02"
                        strUKETORI_KAMOKU = "1"
                    Case Else
                        strUKETORI_KAMOKU = "2"
                End Select
            Else
                Select Case Key.TUKEKAMOKU.PadLeft(2, "0"c)
                    Case "01"
                        strUKETORI_KAMOKU = "2"
                    Case "02"
                        strUKETORI_KAMOKU = "1"
                    Case KessaiKamokuCode_ETC
                        strUKETORI_KAMOKU = ConvertKamoku2TO1(KessaiKamokuCode_ETC)
                    Case Else
                        strUKETORI_KAMOKU = "2"
                End Select
            End If
            '2017/07/25 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END
            '' ���l�ȖڃR�[�h�̐ݒ�(�S��t�H�[�}�b�g�ɍ��킹��)
            '' 02�F���� �� 1
            '' 01�F���� �� 2
            'Select Case Key.TUKEKAMOKU.PadLeft(2, "0"c)
            '    Case "02"
            '        strUKETORI_KAMOKU = "1"
            '    Case Else
            '        strUKETORI_KAMOKU = "2"
            'End Select
            '2017/05/29 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END

            ' ���l�P�̐ݒ�
            If fn_BIKOU1_SETTEI(Key, strBIKOU1, 29) = False Then
                MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", "���l�P�ҏW�����G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If


            '*** �C�� NISHIDA 2008/11/07 ���l�Q�ɐݒ肷�鈵�X�͎����ɂƂ���
            If GetTENMAST(ini_info.JIKINKO_CODE, Key.TORIMATOME_SIT_NO, strKIN_NNM2, strSIT_NNM2, strKIN_KNM2, strSIT_KNM2) = False Then
                MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", "���Z�@�փR�[�h�捞�G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE & " ���Z�@�ցF" & ini_info.JIKINKO_CODE & Key.TORIMATOME_SIT_NO)
                Return -1
            End If
            ' ���l�Q�̐ݒ�
            If fn_BIKOU2_SETTEI(Key, strBIKOU2, 29, strSIT_KNM2) = False Then
                '**************************************************************
                MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", "���l�Q�ҏW�����G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' �f�[�^�ݒ�
            With KawaseFurikomiInFmt
                .TORIATUKAI = Key.KESSAI_YDATE                      ' �戵��
                .SYUMOKU = strSYUMOKU                               ' ��ڃR�[�h
                .JUSIN_TEN = strJUSIN_TEN                           ' ��M�X��
                .FUKA_CODE = "".PadLeft(3, "0"c)                    ' �t���R�[�h
                .HASSIN_TEN = strHASSIN_TEN                         ' ���M�X��
                .KINGAKU = strKin.PadLeft(10, " "c)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = "".PadLeft(1, " "c)                                    ' ���ω�
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.Trim.PadRight(15, " "c)   ' ���z���L����
                .KOKYAKU_TESUU = "0"                                 ' �ڋq�萔��
                .UKETORI_KAMOKU = strUKETORI_KAMOKU                 ' ���l�ȖڃR�[�h
                .UKETORI_KOUZA = Key.TUKEKOUZA.PadLeft(7, "0"c).PadRight(15)    ' ���l�����ԍ� 
                .UKETORI_NAME = Key.TUKEMEIGI.Trim                  ' ���l��
                .IRAI_NAME = ini_info.KAWASE_IRAININ                          ' �˗��l��
                .EDI_INFO = ""                                      ' EDI���
                .BIKOU1 = strBIKOU1                                 ' ���l�P
                .BIKOU2 = strBIKOU2                                 ' ���l�Q
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = KawaseFurikomiInFmt.Data
            KData.OpeCode = String.Concat(KawaseFurikomiInFmt.KAMOKU_CODE, KawaseFurikomiInFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        If strKEKKA = "���z�O" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' �@�\�@ �F �ב֕t�փf�[�^�쐬����
    '
    ' �����@ �F 
    '
    ' �߂�l �F 0 - ���z�O�~���C1 - ���z�O�~�C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_KAWASE_TUKEKAE(ByVal Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseTukekaeInFmt As New CAstFormKes.ClsFormSikinFuri.T_48500
        Dim strJUSIN_TEN As String
        Dim strHASSIN_TEN As String
        '2011/06/17 �x���Ȃ���
        Dim strKIN_NNM As String = ""                ' ���Z�@�֊�����
        Dim strSIT_NNM As String = ""                ' �x�X������
        Dim strKIN_KNM As String = ""                ' ���Z�@�փJ�i��
        Dim strSIT_KNM As String = ""                ' �x�X�J�i��
        '*** �C���@���l�Q�Ƃ�܂ƂߓX��ݒ肷�邽�߂̃��[�N�G���A�@2008/11/07 NISHIDA
        '2011/06/17 
        Dim strKIN_NNM2 As String = ""                ' ���Z�@�֊�����
        Dim strSIT_NNM2 As String = ""                ' �x�X������
        Dim strKIN_KNM2 As String = ""             ' ���Z�@�փJ�i��
        Dim strSIT_KNM2 As String = ""                ' �x�X�J�i��
        '*****************************************************************************
        Dim strTUKEKIN_NO As String
        Dim strTUKESIT_NO As String
        Dim strKin As String
        Dim strSagaku As String
        Dim strKINGAKU_LOCAL As String          ' ���z
        Dim strKINFUKKI_FUGOU As String = ""         ' ���z���L����'2011/06/17
        Dim strBANGOU As String
        Dim strBIKOU1 As String = ""
        Dim strBIKOU2 As String = ""
        Dim strTEKIYOU As String

        Try
            ' ������
            KawaseTukekaeInFmt.Init()

            ' ��M�X���̎擾
            strTUKEKIN_NO = Key.TUKEKIN_NO     ' ���ϋ��Z�@��
            strTUKESIT_NO = Key.TUKESIT_NO     ' ���ώx�X

            If GetTENMAST(strTUKEKIN_NO, strTUKESIT_NO, strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = False Then
                MainLOG.Write("�ב֕t�փf�[�^�쐬", "���s", "���Z�@�փR�[�h�捞�G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE & " ���Z�@�ցF" & Key.TUKEKIN_NO & Key.TUKESIT_NO)
                Return -1
            End If

            If Key.TUKEMEIGI = "" Then
                MainLOG.Write("�ב֕t�փf�[�^�쐬", "���s", "���l���i�������`�l���j���ݒ肳��Ă��܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' ��M�X���A���M�X���̐ݒ�
            If strTUKEKIN_NO = ini_info.JIKINKO_CODE Then         ' �{�x�X�בւ̏ꍇ�iINI�t�@�C���̎����ɃR�[�h�Ɠ���j
                strJUSIN_TEN = "� " & strSIT_KNM.Trim
                strHASSIN_TEN = "� ���-"
                strTEKIYOU = "����" & Key.FURI_KEN.PadRight(6, " "c).Trim & "��"
            Else                                            ' ���s�בւ̏ꍇ
                strJUSIN_TEN = strKIN_KNM.Trim & " " & strSIT_KNM.Trim
                strHASSIN_TEN = ini_info.KAWASE_CENTER
                strTEKIYOU = ini_info.SIKINTEKIYOU & " ����" & Key.FURI_KEN.PadRight(6, " "c).Trim & "��"
            End If

            '�U���ϋ��z�Ǝ萔���z�̍��z
            strSagaku = CStr(CLng(Key.FURI_KIN) - CLng(Key.TESUU_KIN))

            ' �萔���������z���O�~�ȉ��ƂȂ�ꍇ�͎萔�������������Ȃ�
            If CLng(strSagaku) <= 0 Then
                strSagaku = Key.FURI_KIN
                strKEKKA = "���z�O"
            End If

            ' ���z�̐ݒ�
            ' �s�x�����A���A���������ȊO�͓����z�Ɏ萔�������������Ȃ�
            If Key.TESUUTYO_KBN = "0" And Key.TESUUTYO_PATN = "0" Then
                strKin = strSagaku
            Else
                strKin = Key.FURI_KIN
            End If

            ' ���z���J���}�ҏW����
            strKINGAKU_LOCAL = CASTCommon.CADec(strKin).ToString("###,###,###,##0").PadLeft(10, " "c)

            ' ���z���L�����̎擾
            If fn_FUGO_SETTEI(strKINGAKU_LOCAL, strKINFUKKI_FUGOU) = False Then
                MainLOG.Write("�ב֕t�փf�[�^�쐬", "���s", "���L�����ݒ菈���G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' �ԍ��̐ݒ�
            '2018/11/21 �^�X�N�j���� CHG �W���Ńo�O�Ή�(�M��������̈ב֕t�ւ̐ݒ�l�ύX(������ݒ�s��))  -------------------- START
            '--------------------------------------------------------------------------------
            ' �ב֕t�ւ́u�ԍ����v�͐��l���ڂ̂��߁u����-1�v�̓G���[�A
            ' ���l�݂̂��ݒ�\(�ԍ����ɂ�"1"��ݒ肷��)
            '--------------------------------------------------------------------------------
            'If Key.TUKEKIN_NO = "1000" Then             ' �S�M�A�̏ꍇ
            '    strBANGOU = "����-1"
            'Else
            '    strBANGOU = ""                          ' �S�M�A�ȊO�̏ꍇ
            'End If
            Select Case Key.TUKEKIN_NO
                Case "1000"
                    '----------------------------------------
                    ' ���ϋ��Z�@�ւ��A�S�M�A(�M������)�̏ꍇ
                    ' �u�ԍ����v�́A"1"��ݒ�
                    '----------------------------------------
                    strBANGOU = "1"
                Case Else
                    '----------------------------------------
                    ' ���ϋ��Z�@�ւ��A�S�M�A�ȊO�̏ꍇ
                    ' �u�ԍ����v�́A�󔒂�ݒ�
                    '----------------------------------------
                    strBANGOU = ""
            End Select
            '2018/11/21 �^�X�N�j���� CHG �W���Ńo�O�Ή�(�M��������̈ב֕t�ւ̐ݒ�l�ύX(������ݒ�s��))  -------------------- END

            ' ���l�P�̐ݒ�
            If fn_BIKOU1_SETTEI(Key, strBIKOU1, 48) = False Then
                MainLOG.Write("�ב֕t�փf�[�^�쐬", "���s", "���l�P�ҏW�����G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If GetTENMAST(ini_info.JIKINKO_CODE, Key.TORIMATOME_SIT_NO, strKIN_NNM2, strSIT_NNM2, strKIN_KNM2, strSIT_KNM2) = False Then
                MainLOG.Write("�ב֐U���f�[�^�쐬", "���s", "���Z�@�փR�[�h�捞�G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE & " ���Z�@�ցF" & ini_info.JIKINKO_CODE & Key.TORIMATOME_SIT_NO)
                Return -1
            End If

            ' ���l�Q�̐ݒ�
            If fn_BIKOU2_SETTEI(Key, strBIKOU2, 48, strSIT_KNM2) = False Then
                '**************************************************************
                MainLOG.Write("�ב֕t�փf�[�^�쐬", "���s", "���l�Q�ҏW�����G���[�B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' �f�[�^�ݒ�
            With KawaseTukekaeInFmt
                .TORIATUKAI = Key.KESSAI_YDATE                      ' �戵��
                .SYUMOKU = "4301"                                   ' ��ڃR�[�h
                .JUSIN_TEN = strJUSIN_TEN                           ' ��M�X��
                .FUKA_CODE = "".PadLeft(3, "0"c)                    ' �t���R�[�h
                .HASSIN_TEN = strHASSIN_TEN                         ' ���M�X��
                .KINGAKU = strKin.PadLeft(10, " "c)                 ' ���z
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = "".PadLeft(1, " "c)                   ' ���ω�
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.Trim.PadRight(15, " "c)  ' ���z���L����
                .BANGOU = strBANGOU                                 ' �ԍ�
                '2018/11/21 �^�X�N�j���� CHG �W���Ńo�O�Ή�(�M��������̈ב֕t�ւ̐ݒ�l�ύX(������ݒ�s��))  -------------------- START
                '--------------------------------------------------------------------------------
                ' �ב֕t�ւ́u�ԍ����v�͐��l���ڂ̂��߁u����-1�v�̓G���[�A
                ' ���l�݂̂��ݒ�\(�ԍ����ɂ�"1"��ݒ肷��)
                '--------------------------------------------------------------------------------
                '.SIKIN_JIYUU1 = Key.TUKEMEIGI                       ' �����t�֎��R1�F���l���i�������`�l���j
                ''*** �C�� nishida 2008/11/07 �˗��l���ύX ����#####9��
                ''���Ƃɖ߂� 2009.10.29
                '.SIKIN_JIYUU2 = ini_info.KAWASE_IRAININ                         ' �����t�֎��R2�F�˗��l���iINI�t�@�C��)
                ''.SIKIN_JIYUU2 = strTEKIYOU
                ''*****************************************************************
                '.SIKIN_JIYUU3 = strBIKOU1                           ' �����t�֎��R3�F���l�P
                '.SIKIN_JIYUU4 = strBIKOU2                           ' �����t�֎��R4�F���l�Q
                .SIKIN_JIYUU1 = Key.TUKEMEIGI                       ' �����t�֎��R1�F�����}�X�^�u���ϖ��`�l�i�J�i�j�v
                .SIKIN_JIYUU2 = ini_info.KAWASE_IRAININ             ' �����t�֎��R2�FINI�t�@�C�� �u�˗��l���v([KESSAI]-[IRAININ])
                .SIKIN_JIYUU3 = strBIKOU1                           ' �����t�֎��R3�F�����}�X�^�u���l�P�v
                .SIKIN_JIYUU4 = strBIKOU2                           ' �����t�֎��R4�F�����}�X�^�u���l�Q�v
                '2018/11/21 �^�X�N�j���� CHG �W���Ńo�O�Ή�(�M��������̈ב֕t�ւ̐ݒ�l�ύX(������ݒ�s��))  -------------------- END
                .SYOKAI_NO = ""                                     ' �Ɖ�ԍ�
                '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ---------------------------------------->>>>
                .EDI_INFO = ""                                      ' EDI���
                '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ----------------------------------------<<<<
                .YOBI1 = ""                                         ' ���l
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = KawaseTukekaeInFmt.Data
            KData.OpeCode = String.Concat(KawaseTukekaeInFmt.KAMOKU_CODE, KawaseTukekaeInFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = strKin.PadLeft(13, "0"c)
            KData.ope_tesuu = "0".PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("�ב֕t�փf�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        If strKEKKA = "���z�O" Then
            Return 1
        Else
            Return 0
        End If

    End Function

    ' �@�\�@ �F �萔�������i�A���j�f�[�^�쐬����
    '
    ' �����@ �F TesuuMode�F0 - ���U�萔���C1 - �U���萔��
    '
    ' �߂�l �F 0 - ����C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_TESUUIN_RENDO(ByRef key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal TesuuMode As Integer) As Integer

        Dim TesuuRendoFmt As New CAstFormKes.ClsFormSikinFuri.T_99418
        Dim strTESUU_SYUBETU As String
        Dim strUTIWAKE_CODE As String
        Dim strKAMOKU_KOBAN As String
        Dim strKin As String
        Dim strTEKIYO As String

        Try
            ' ������
            TesuuRendoFmt.Init()

            ' �萔����ʂb�A�萔���z�A�����薾�דE�v�̐ݒ�
            If TesuuMode = 0 Then   ' 0:���U�萔��
                strTESUU_SYUBETU = "301"
                strKin = CStr(CLng(key.TESUU_KIN1) + CLng(key.TESUU_KIN2))
                strTEKIYO = "����ý��ֳ"
            Else                    ' 1:�U���萔��
                strTESUU_SYUBETU = "200"
                strKin = key.TESUU_KIN3
                strTEKIYO = "�غ�ý��ֳ"
            End If

            ' �����X�ԁA�����ȖځE���Ԃ̐ݒ�
            If key.TESUUTYO_PATN = "0" Then     ' 0:����
                ' �f�[�^�`�F�b�N
                If ini_info.HONBU_CODE = "" OrElse CLng(ini_info.HONBU_CODE) = 0 Then '�󔒂܂��̓I�[���O�̎�
                    MainLOG.Write("�萔�������i�A���j�f�[�^�쐬", "���s", "�����X�Ԃ�����܂���B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE)

                    Return -1
                End If

                If key.HONBU_KOUZA = "" OrElse CLng(key.HONBU_KOUZA) = 0 Then
                    MainLOG.Write("�萔�������i�A���j�f�[�^�쐬", "���s", "�����ȖځE���Ԃ�����܂���B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE)
                    Return -1
                End If

                '*** �C�� 2008/11/25 nishida �{���R�[�h���Ƃ�܂ƂߓX�̏ꍇ�󔒁A�{���R�[�h���Ƃ�܂ƂߓX�̏ꍇ�{���R�[�h���Z�b�g����BSTART
                If ini_info.HONBU_CODE = key.TORIMATOME_SIT_NO Then
                    strUTIWAKE_CODE = ""
                Else
                    strUTIWAKE_CODE = ini_info.HONBU_CODE
                End If
                '*** �C�� 2008/11/25 nishida �{���R�[�h���Ƃ�܂ƂߓX�̏ꍇ�󔒁A�{���R�[�h���Ƃ�܂ƂߓX�̏ꍇ�{���R�[�h���Z�b�g����BEND
                strKAMOKU_KOBAN = "04" & key.HONBU_KOUZA.PadLeft(7, "0"c)

            Else                                ' 1:����
                ' �f�[�^�`�F�b�N
                If key.TSUUTYOSIT_NO = "" OrElse CInt(key.TSUUTYOSIT_NO) = 0 Then
                    MainLOG.Write("�萔�������i�A���j�f�[�^�쐬", "���s", "�����X�Ԃ�����܂���B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE)
                    Return -1
                End If

                If key.TSUUTYOKAMOKU = "" OrElse CLng(key.TSUUTYOKAMOKU) = 0 Then
                    MainLOG.Write("�萔�������i�A���j�f�[�^�쐬", "���s", "�����ȖځE���Ԃ�����܂���B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE)
                    Return -1
                End If

                If key.TSUUTYOKOUZA = "" OrElse CLng(key.TSUUTYOKOUZA) = 0 Then
                    MainLOG.Write("�萔�������i�A���j�f�[�^�쐬", "���s", "�����ȖځE���Ԃ�����܂���B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE)
                    Return -1
                End If

                ' 2008.04.18 MODIFY �����X�ԂɂĎ萔�������敪�����ڂ̏ꍇ�C�t�֎x�X�R�[�h�Œ�ł͂Ȃ��t�֎x�X�ƂƂ�܂ƂߓX�𔻒肵����X�Ԃ̏ꍇ�͋󔒁C�قȂ�ꍇ�͕t�֎x�X��ݒ肷�� >>
                'strUTIWAKE_CODE = key.TSUUTYOSIT_NO
                If key.TSUUTYOSIT_NO = key.TORIMATOME_SIT_NO Then
                    strUTIWAKE_CODE = ""
                Else
                    strUTIWAKE_CODE = key.TSUUTYOSIT_NO
                End If
                ' 2008.04.18 MODIFY �����X�ԂɂĎ萔�������敪�����ڂ̏ꍇ�C�t�֎x�X�R�[�h�Œ�ł͂Ȃ��t�֎x�X�ƂƂ�܂ƂߓX�𔻒肵����X�Ԃ̏ꍇ�͋󔒁C�قȂ�ꍇ�͕t�֎x�X��ݒ肷�� <<
                strKAMOKU_KOBAN = key.TSUUTYOKAMOKU.PadLeft(2, "0"c) & key.TSUUTYOKOUZA.PadLeft(7, "0"c)
            End If

            '�f�[�^�ݒ�
            With TesuuRendoFmt
                .TESUU_SYUBETU = strTESUU_SYUBETU                   ' �萔����ʂb
                .TEUSU_UTIWAKE = ""                                 ' �萔������b
                .UTIWAKE_CODE = strUTIWAKE_CODE                     ' �����X��
                .KAMOKU_KOBAN = strKAMOKU_KOBAN                     ' �����ȖځE����
                .KOKYAKU_NO = ""                                    ' �ڋq�ԍ�
                .KAIIN_CODE = ""                                    ' ����R�[�h
                .TESUUTYO_KBN = ""                                  ' �萔�������敪
                .TESUU_KINGAKU = strKin.PadLeft(10, " "c)           ' �萔���z
                .TESUU_KEN = ""                                     ' �萔������
                .CLKAMOKU_KOBAN = ""                                ' �b�k�Ȗڌ���
                .TEKIYO = strTEKIYO                                 ' �����薾�דE�v
                .KOUSYU_NO = ""                                     ' ������הԍ�
                .KISANBI = ""                                       ' �N�Z��
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = TesuuRendoFmt.Data
            KData.OpeCode = String.Concat(TesuuRendoFmt.KAMOKU_CODE, TesuuRendoFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = "0".PadLeft(13, "0"c)
            KData.ope_tesuu = strKin.PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("�萔�������i�A���j�f�[�^�쐬", "���s", ex.Message)

            Return -1
        End Try

        '�萔�������σt���O���n�m�ɐݒ�
        key.TESUUTYO_FLG = "1"
        Return 0

    End Function


    ' �@�\�@�@�F����������i�萔���j99-019 �f�[�^�쐬����
    '
    ' �����@�@�F
    '
    ' �߂�l�@�F 0 - ���z 0 �~���A1 - ���z 0 �~�A-1 - �ُ�
    '
    ' ���l      ���l���ɏ]���܂�
    '
    '
    Private Function fn_TESUUIN_SYOKANJOIN(ByRef Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal TesuuMode As Integer) As Integer

        Dim TesuuInSyokanjoInFmt As New CAstFormKes.ClsFormSikinFuri.T_99019
        Dim strKOUZA_NO As String               '�����ԍ�
        Dim strUTIWAKE_CODE As String           '����R�[�h
        Dim strKin As String                    '���z
        Dim strTEKIYOU As String                '�E�v

        Try
            '������
            TesuuInSyokanjoInFmt.Init()

            If TesuuMode = 0 Then       '0:���U�萔��
                '�f�[�^�`�F�b�N
                '������ȖڃR�[�h(INI�t�@�C��)
                If ini_info.TESUU_KOUZA1 = "" OrElse CLng(ini_info.TESUU_KOUZA1) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "������ȖڃR�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If
                '����R�[�h�iINI�t�@�C���j
                If ini_info.UCHIWAKE1 = "" OrElse CLng(ini_info.UCHIWAKE1) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "����R�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If

                '�����ԍ��̐ݒ�iINI�t�@�C���̏�����ȖڃR�[�h�j
                strKOUZA_NO = ini_info.TESUU_KOUZA1.Trim.PadLeft(7, "0"c)

                '����R�[�h�̐ݒ�iINI�t�@�C���̓���R�[�h�j
                strUTIWAKE_CODE = ini_info.UCHIWAKE1.Trim.PadLeft(2, "0"c)

                '���z�̐ݒ�i����VIEW�� �萔�����z�P + �萔�����z�Q�j
                strKin = CStr(CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2))

            Else                        '1:�U���萔��
                '�f�[�^�`�F�b�N
                '������ȖڃR�[�h(INI�t�@�C��)
                If ini_info.TESUU_KOUZA2 = "" OrElse CLng(ini_info.TESUU_KOUZA2) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "������ȖڃR�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If
                '����R�[�h�iINI�t�@�C���j
                If ini_info.UCHIWAKE2 = "" OrElse CLng(ini_info.UCHIWAKE2) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "����R�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If

                If Key.KESSAI_SYUBETU = "0" Then
                    '�����ԍ��̐ݒ�iINI�t�@�C���̏�����ȖڃR�[�h�j
                    strKOUZA_NO = ini_info.TESUU_KOUZA2.Trim.PadLeft(7, "0"c)

                    '����R�[�h�̐ݒ�iINI�t�@�C���̓���R�[�h�j
                    strUTIWAKE_CODE = ini_info.UCHIWAKE2.Trim.PadLeft(2, "0"c)
                Else
                    '�����ԍ��̐ݒ�iINI�t�@�C���̏�����ȖڃR�[�h�j
                    strKOUZA_NO = ini_info.TESUU_KOUZA4.Trim.PadLeft(7, "0"c)

                    '����R�[�h�̐ݒ�iINI�t�@�C���̓���R�[�h�j
                    strUTIWAKE_CODE = ini_info.UCHIWAKE4.Trim.PadLeft(2, "0"c)
                End If

                '���z�̐ݒ�i����VIEW�� �萔�����z�R�j
                strKin = CStr(CLng(Key.TESUU_KIN3))
            End If

            If CInt(strKOUZA_NO) = 0 Then
                MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "�����ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If CInt(strUTIWAKE_CODE) = 0 Then
                MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "����R�[�h���ݒ肳��Ă��܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            ' �E�v�̐ݒ�
            ' ���`�l�J�i���ݒ�ς݂̏ꍇ�A���`�l�J�i��D��
            If Key.TUKEMEIGI <> "" Then
                strTEKIYOU = Key.TUKEMEIGI
            Else
                strTEKIYOU = Key.ITAKU_KNAME
            End If

            If strTEKIYOU = "" Then
                MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "�E�v������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            With TesuuInSyokanjoInFmt
                .KOUZA_NO = strKOUZA_NO                 '�����ԍ�
                .GYO = "01"                             '�s
                .UTIWAKE_CODE = strUTIWAKE_CODE         '����R�[�h
                .ZENZAN = "0".PadLeft(15, " "c)          '�O�c
                .FUGOU_CODE = "1"                       '�����R�[�h
                .KINGAKU = strKin.PadLeft(13, " "c)     '���z
                .TATEN_TEKIYOU = ""                     '���X���E�v
                .KENSU = "1".PadLeft(5, " "c)           '����
                .FURI_CODE = ""                         '�U�փR�[�h
                .TORIATUKAI1 = ""                       '�戵�ԍ��P
                .JINKAKU_CODE = ""                      '�l�i�R�[�h
                .KAZEI_CODE = ""                        '�ېŃR�[�h
                .TEKIYOU = strTEKIYOU                   '�E�v
                .KISANBI = ""                           '�N�Z��
                .GENTEN_NO = ""                         '���X�ԍ�
                .YOBI1 = ""                             '�\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = TesuuInSyokanjoInFmt.Data
            ' �ȖڃR�[�h�A�I�y�R�[�h�̐ݒ�
            KData.OpeCode = String.Concat(TesuuInSyokanjoInFmt.KAMOKU_CODE, TesuuInSyokanjoInFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = "0".PadLeft(13, "0"c)
            KData.ope_tesuu = strKin.PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        '�萔�������σt���O���n�m�ɐݒ�
        Key.TESUUTYO_FLG = "1"
        Return 0
    End Function

    ' �@�\�@�@�F������A�������i�萔���j99-419 �f�[�^�쐬����
    '
    ' �����@�@�F
    '
    ' �߂�l�@�F 0 - ���z 0 �~���A1 - ���z 0 �~�A-1 - �ُ�
    '
    ' ���l
    '
    Private Function fn_TESUUIN_SYOKANJORENDO(ByRef Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData, ByVal TesuuMode As Integer) As Integer

        Dim TesuuInSyokanjoRendoFmt As New CAstFormKes.ClsFormSikinFuri.T_99419
        Dim strKOUZA_NO As String               '�����ԍ�
        Dim strUTIWAKE_CODE As String           '����R�[�h
        Dim strKin As String                    '���z
        '2011/06/17
        Dim strTEKIYOU As String = ""                '�E�v
        Dim strTUKEKIN_NO As String             '���ϋ��Z�@��
        Dim strTUKESIT_NO As String             '���ώx�X
        Dim strTSUUTYOSIT_NO As String          '�萔�������x�X     '2008/07/22�@�l���M���@�萔�������x�X�ǉ�

        Try
            TesuuInSyokanjoRendoFmt.Init()

            If TesuuMode = 0 Then       '���U�萔��
                '�f�[�^�`�F�b�N
                '������ȖڃR�[�h(INI�t�@�C��)
                If ini_info.TESUU_KOUZA1 = "" OrElse CLng(ini_info.TESUU_KOUZA1) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "������ȖڃR�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If
                '����R�[�h�iINI�t�@�C���j
                If ini_info.UCHIWAKE1 = "" OrElse CLng(ini_info.UCHIWAKE1) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "����R�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If

                '�����ԍ��̐ݒ�iINI�t�@�C���̏�����ȖڃR�[�h�j
                strKOUZA_NO = ini_info.TESUU_KOUZA1.Trim.PadLeft(7, "0"c)

                '����R�[�h�̐ݒ�iINI�t�@�C���̓���R�[�h�j
                strUTIWAKE_CODE = ini_info.UCHIWAKE1.Trim.PadLeft(2, "0"c)

                '���z�̐ݒ�i����VIEW�� �萔�����z�P + �萔�����z�Q�j
                strKin = CStr(CLng(Key.TESUU_KIN1) + CLng(Key.TESUU_KIN2))

            Else                        '�U���萔��
                '�f�[�^�`�F�b�N
                '������ȖڃR�[�h(INI�t�@�C��)
                If ini_info.TESUU_KOUZA3 = "" OrElse CLng(ini_info.TESUU_KOUZA3) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "������ȖڃR�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If
                '����R�[�h�iINI�t�@�C���j
                If ini_info.UCHIWAKE3 = "" OrElse CLng(ini_info.UCHIWAKE3) = 0 Then '�󔒁A�܂��̓I�[���[���̎�
                    MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "����R�[�h������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                    Return -1
                End If

                If Key.KESSAI_SYUBETU = "0" Then
                    '�����ԍ��̐ݒ�iINI�t�@�C���̏�����ȖڃR�[�h�j
                    strKOUZA_NO = ini_info.TESUU_KOUZA3.Trim.PadLeft(7, "0"c)

                    '����R�[�h�̐ݒ�iINI�t�@�C���̓���R�[�h�j
                    strUTIWAKE_CODE = ini_info.UCHIWAKE3.Trim.PadLeft(2, "0"c)
                Else
                    '�����ԍ��̐ݒ�iINI�t�@�C���̏�����ȖڃR�[�h�j
                    strKOUZA_NO = ini_info.TESUU_KOUZA5.Trim.PadLeft(7, "0"c)

                    '����R�[�h�̐ݒ�iINI�t�@�C���̓���R�[�h�j
                    strUTIWAKE_CODE = ini_info.UCHIWAKE5.Trim.PadLeft(2, "0"c)
                End If

                '���z�̐ݒ�i����VIEW�� �萔�����z�R�j
                strKin = CStr(CLng(Key.TESUU_KIN3))
            End If

            If CInt(strKOUZA_NO) = 0 Then
                MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "�����ԍ�������܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            If CInt(strUTIWAKE_CODE) = 0 Then
                MainLOG.Write("����������i�萔���j�f�[�^�쐬", "���s", "����R�[�h���ݒ肳��Ă��܂���B������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�֓��F" & Key.FURI_DATE)
                Return -1
            End If

            strTUKEKIN_NO = Key.TUKEKIN_NO
            strTUKESIT_NO = Key.TUKESIT_NO
            strTSUUTYOSIT_NO = Key.TSUUTYOSIT_NO        '2008/07/22�@�l���M���@�萔�������x�X�ǉ�

            With TesuuInSyokanjoRendoFmt
                .KOUZA_NO = strKOUZA_NO                 '�����ԍ�
                .GYO = "01"                             '�s
                .UTIWAKE_CODE = strUTIWAKE_CODE         '����R�[�h
                .ZENZAN = "0".PadLeft(15, " "c)         '�O�c
                .FUGOU_CODE = "1"                       '�����R�[�h
                .KINGAKU = strKin.PadLeft(13, " "c)     '���z
                .KENSU = "1".PadLeft(5, " "c)           '����
                .FURI_CODE = ""                         '�U�փR�[�h
                .TORIATUKAI1 = ""                       '�戵�ԍ��P
                .JINKAKU_CODE = ""                      '�l�i�R�[�h
                .KAZEI_CODE = ""                        '�ېŃR�[�h
                .TEKIYOU = "ý��ֳ"                     '�E�v

                If strTUKEKIN_NO = ini_info.JIKINKO_CODE Then
                    .RENDO_TEN = strTSUUTYOSIT_NO.Trim.PadLeft(3, "0"c)        '�A���X��
                    .RENDO_KAMOKU = (Key.TSUUTYOKAMOKU).Trim.PadLeft(2, "0"c) + (Key.TSUUTYOKOUZA).Trim.PadLeft(7, "0"c) '�A���Ȗڌ���
                Else
                    .RENDO_TEN = ""
                    .RENDO_KAMOKU = ""
                End If
                .AITE_UTIWAKE = ""                      '�������R�[�h
                .SOFT_NO = ""                           '�\�t�g�@��
                .TORIATUKAI2 = ""                       '�戵�ԍ��Q
                .AITE_TEKIYOU = ""                      '����E�v
                .KISANBI = ""                           '�N�Z��
                .YOBI1 = ""                             '�\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = TesuuInSyokanjoRendoFmt.Data
            ' �ȖڃR�[�h�A�I�y�R�[�h�̐ݒ�
            KData.OpeCode = String.Concat(TesuuInSyokanjoRendoFmt.KAMOKU_CODE, TesuuInSyokanjoRendoFmt.OPE_CODE)

            ' ���M���z�̐ݒ�
            KData.ope_nyukin = "0".PadLeft(13, "0"c)
            KData.ope_tesuu = strKin.PadLeft(13, "0"c)

        Catch ex As Exception
            MainLOG.Write("������A�������i�萔���j�f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        '�萔�������σt���O���n�m�ɐݒ�
        Key.TESUUTYO_FLG = "1"
        Return 0
    End Function


    ' �@�\�@ �F ���L�����ݒ菈��
    '
    ' �����@ �F astrKEY1:�ϊ��O���z�i�J���}�ҏW�ς݁j
    '           astrKEY2
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F �p�����^�ł킽���ꂽ���z�����ƂɂP�T�P�^�̕��L������Ԃ�
    '
    Private Function fn_FUGO_SETTEI(ByVal astrKEY1 As String, ByRef astrKEY2 As String) As Boolean
        Dim intCount As Integer     '������
        Dim strASSYUKU As String    '���k
        Dim strFUGO(14) As String   '����
        Dim I As Integer

        Try
            astrKEY2 = ""
            strASSYUKU = "Y"

            For intCount = 0 To astrKEY1.Length - 1

                strFUGO(intCount) = " "

                Select Case astrKEY1.Substring(intCount, 1)
                    Case "0"
                        If strASSYUKU = "Y" Then
                            strFUGO(intCount) = " "
                        Else
                            strFUGO(intCount) = "�"
                        End If
                    Case "1"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "2"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "3"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "4"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "5"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "6"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "7"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "8"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case "9"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "�"
                    Case ","
                        strFUGO(intCount) = " "
                End Select
            Next

            For I = 0 To strFUGO.Length - 1
                astrKEY2 = astrKEY2 & strFUGO(I)
            Next

            astrKEY2 = astrKEY2.Trim

        Catch ex As Exception
            MainLOG.Write("���L�����ݒ菈��", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F ���l�P�ҏW����
    '
    ' �����@ �F Key:
    '           strBIKOU1:�ҏW��̔��l�P
    '           strKeta  :���l�P�̌���
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F ���l�P�����L�̂Ƃ���ҏW���Ė߂�
    '           �u@MM-DD@�v �� �U�֓��ɕҏW
    '           �u@KEN@�v   �� �U�֍ό����ɕҏW
    '           �i �����}�X�^�o�^���A�u@MM-DD@�v�A�u@TEN@�v�A�u@KEN@�v�͓����ɐݒ�s�j
    '
    Private Function fn_BIKOU1_SETTEI(ByVal Key As KeyInfo, ByRef strBIKOU1 As String, ByVal intKeta As Integer) As Boolean

        Try
            If Key.BIKOU1.Length >= 7 Then
                If Key.BIKOU1.Substring(0, 7) = "@MM-DD@" Then
                    strBIKOU1 = Key.FURI_DATE.Substring(4, 2) & "-" & Key.FURI_DATE.Substring(6, 2) & Key.BIKOU1.Substring(7, Key.BIKOU1.Length - 7)
                Else
                    strBIKOU1 = Key.BIKOU1
                End If
            ElseIf Key.BIKOU1.Length >= 5 Then
                strBIKOU1 = Key.BIKOU1.Replace("@KEN@", Key.FURI_KEN)
            Else
                strBIKOU1 = Key.BIKOU1
            End If

            ' �ݒ茅�����߃`�F�b�N
            If strBIKOU1.Length > intKeta Then
                strBIKOU1 = strBIKOU1.Substring(0, intKeta)
            End If

        Catch ex As Exception
            MainLOG.Write("���l�P�ҏW����", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F ���l�Q�ҏW����
    '
    ' �����@ �F Key:
    '           strBIKOU1:�ҏW��̔��l�Q
    '           strKeta  :���l�Q�̌���
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F ���l�Q�����L�̂Ƃ���ҏW���Ė߂�
    '           �u@MM-DD@�v �� �U�֓��ɕҏW
    '           �u@TEN@�v   �� �x�X���J�i�{�u�¶��v�ɕҏW
    '           �u@KEN@�v   �� �U�֍ό����ɕҏW
    '           �i �����}�X�^�o�^���A�u@MM-DD@�v�A�u@TEN@�v�A�u@KEN@�v�͓����ɐݒ�s�j
    '
    Private Function fn_BIKOU2_SETTEI(ByVal Key As KeyInfo, ByRef strBIKOU2 As String, ByVal intKeta As Integer, ByVal strSIT_KNM As String) As Boolean

        Try
            If Key.BIKOU2.Trim.Length >= 7 Then
                If Key.BIKOU2.Substring(0, 7) = "@MM-DD@" Then
                    strBIKOU2 = Key.FURI_DATE.Substring(4, 2) & "-" & Key.FURI_DATE.Substring(6, 2) & Key.BIKOU2.Substring(7, Key.BIKOU2.Length - 7)
                ElseIf Key.BIKOU2.Substring(0, 5) = "@TEN@" Then
                    strBIKOU2 = strSIT_KNM.Trim & " �¶�" & Key.BIKOU2.Substring(5, Key.BIKOU2.Length - 5)
                Else
                    strBIKOU2 = Key.BIKOU2
                End If
            ElseIf Key.BIKOU2.Length >= 5 Then
                If Key.BIKOU2.Substring(0, 5) = "@TEN@" Then
                    strBIKOU2 = strSIT_KNM.Trim & " �¶�" & Key.BIKOU2.Substring(5, Key.BIKOU2.Length - 5)
                ElseIf Key.BIKOU2.Substring(0, 5) = "@KEN@" Then
                    strBIKOU2 = Key.BIKOU2.Replace("@KEN@", Key.FURI_KEN)
                Else
                    strBIKOU2 = Key.BIKOU2
                End If
            Else
                strBIKOU2 = Key.BIKOU2
            End If

            ' �ݒ茅�����߃`�F�b�N
            If strBIKOU2.Length > intKeta Then
                strBIKOU2 = strBIKOU2.Substring(0, intKeta)
            End If

        Catch ex As Exception
            MainLOG.Write("���l�Q�ҏW����", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �������σ}�X�^�o�^
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertKesMast(ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            MainLOG.Write("(�������σ}�X�^�o�^)�J�n", "����")


            SQL.Append("INSERT INTO KESSAIMAST(")
            SQL.Append(" SYORI_DATE_KR")
            SQL.Append(",TIME_STAMP_KR")
            SQL.Append(",KAIJI_KR")
            SQL.Append(",RECORD_NO_KR")
            SQL.Append(",FILE_NAME_KR")
            SQL.Append(",TORIS_CODE_KR")
            SQL.Append(",TORIF_CODE_KR")
            SQL.Append(",FURI_DATE_KR")
            SQL.Append(",KAMOKU_CODE_KR")
            SQL.Append(",OPE_CODE_KR")
            SQL.Append(",DENBUN_ALL_KR")
            SQL.Append(",ERR_CODE_KR")
            SQL.Append(",ERR_MSG_KR")
            SQL.Append(",SAKUSEI_DATE_KR")
            SQL.Append(",KOUSIN_DATE_KR")
            SQL.Append(") VALUES (")
            SQL.Append(" " & SQ(strDate))                                   ' ������
            SQL.Append("," & SQ(String.Concat(strDate, strTime)))           ' �^�C���X�^���v
            SQL.Append("," & SQ(keskey.Kaiji))                              ' ��
            SQL.Append("," & SQ(keskey.RecordNo))                           ' �ʔ�
            SQL.Append("," & SQ(ini_info.RIENTA_FILENAME))                     ' ���G���^�t�@�C����
            SQL.Append("," & SQ(KData.TorisCode))                        ' ������R�[�h
            SQL.Append("," & SQ(KData.TorifCode))                        ' ����敛�R�[�h
            SQL.Append("," & SQ(KData.FuriDate))                         ' �U�֓�
            SQL.Append("," & SQ(KData.OpeCode.Substring(0, 2)))           ' �ȖڃR�[�h
            SQL.Append("," & SQ(KData.OpeCode.Substring(2, 3)))           ' �I�y�R�[�h
            SQL.Append("," & SQ(KData.record320))                           ' �ʃf�[�^
            SQL.Append("," & SQ(""))                                        ' ���ʃR�[�h
            SQL.Append("," & SQ(""))                                        ' �G���[���b�Z�[�W
            SQL.Append("," & SQ(strDate))                                   ' �쐬��
            SQL.Append("," & SQ(strDate))                                   ' �X�V��
            SQL.Append(")")

            Call MainDB.ExecuteNonQuery(SQL)
            CntDenbun += 1
        Catch ex As Exception
            MainLOG.Write("(�������σ}�X�^�o�^)", "���s", ex.Message)

            Return False
        Finally
            MainLOG.Write("(�������σ}�X�^�o�^)�J�n", "����")

        End Try

        Return True

    End Function

    ' �@�\�@ �F �X�P�W���[���}�X�^�X�V
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateSchMast(ByVal key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim Up_FLG As Boolean = False

        Try
            '*** �C�� mitsu 2008/07/31 �ʓr�����ł����σt���O���X�V���� ***
            '' �萔�������敪�� 3�F�ʓr���� �̏ꍇ�A�X�P�W���[���}�X�^�͍X�V���Ȃ�
            'If key.TESUUTYO_KBN = "3" Then
            '    LOG.Write("�X�P�W���[���}�X�^�X�V", "����", "�X�P�W���[���X�V�ΏۊO�i�萔�������敪�F�ʓr�����j�B" & "������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE)
            '    Return True
            'End If
            '**************************************************************

            SQL.Append("UPDATE SCHMAST")
            SQL.Append(" SET ")

            Select Case key.KESSAI_TORI_KBN

                Case "0"    ' �������ςƎ萔�������𓯎��ɍs����
                    Up_FLG = True

                    SQL.Append(" KESSAI_FLG_S = '1'")
                    SQL.Append(",KESSAI_DATE_S = " & SQ(strDate))
                    SQL.Append(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))

                    ' �萔�������ρA���́A���ϋ敪�� 04�F���ʊ�ƁA05�F���ϑΏۊO �̏ꍇ
                    If key.TESUUTYO_FLG = "1" Or _
                        (key.KESSAI_KBN = "04" Or key.KESSAI_KBN = "05") Then

                        SQL.Append(",TESUUTYO_FLG_S = '1'")
                        SQL.Append(",TESUU_DATE_S = " & SQ(strDate))
                        SQL.Append(",TESUU_TIME_STAMP_S = " & SQ(strDate & strTime))
                    End If

                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
                    SQL.Append("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
                    SQL.Append("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))

                Case "1"    ' �������ς����s����
                    Up_FLG = True

                    SQL.Append(" KESSAI_FLG_S = '1'")
                    SQL.Append(",KESSAI_DATE_S = " & SQ(strDate))
                    SQL.Append(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))
                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
                    SQL.Append("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
                    SQL.Append("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))

                Case "2"    ' �萔�����������s����

                    '*** �萔���݂͓̂��삳���Ȃ� 2009.10.29 start
                    ' �萔�������ρA���́A���ϋ敪�� 04�F���ʊ�ƁA05�F���ϑΏۊO�̏ꍇ
                    'If key.TESUUTYO_FLG = "1" Or _
                    '    (key.KESSAI_KBN = "04" Or key.KESSAI_KBN = "05") Then

                    '    Up_FLG = True

                    '    SQL.Append(" TESUUTYO_FLG_S = '1'")
                    '    SQL.Append(",TESUU_DATE_S = " & SQ(strDate))
                    '    SQL.Append(",TESUU_TIME_STAMP_S = " & SQ(strDate & strTime))
                    '    SQL.Append(" WHERE TORIS_CODE_S    = " & SQ(key.TORIS_CODE))
                    '    SQL.Append("   AND TORIF_CODE_S    = " & SQ(key.TORIF_CODE))
                    '    SQL.Append("   AND (KESSAI_YDATE_S != " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUU_YDATE_S   = " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUUKEI_FLG_S  = '1'")                              ' 1:�萔���v�Z��
                    '    SQL.Append("   AND KESSAI_FLG_S    = '1'")                              ' 1:���ύς�
                    '    SQL.Append("   AND TESUUTYO_FLG_S  = '2'")                              ' 0:�萔��������
                    '    SQL.Append("   AND TYUUDAN_FLG_S   = '0'")                              ' 0:���f�Ȃ�
                    '    SQL.Append("   AND FURI_KIN_S      >  0)")                              ' �U�֍ϋ��z����
                    '    SQL.Append("   OR")
                    '    ' �萔�������敪�� 1:�ꊇ���� �̏ꍇ�A�������ςƎ萔�������𓯎��ɍs������܂߂�
                    '    SQL.Append("      (KESSAI_YDATE_S  = " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUU_YDATE_S   = " & SQ(ParaKessaiDate))
                    '    SQL.Append("   AND TESUUKEI_FLG_S  = '1'")                              ' 1:�萔���v�Z��
                    '    SQL.Append("   AND KESSAI_FLG_S    = '1'")                              ' 1:���ύς�  �� ���σf�[�^�͊��Ɋ�����
                    '    SQL.Append("   AND TESUUTYO_FLG_S  = '2'")                              ' 0:�萔��������
                    '    SQL.Append("   AND TYUUDAN_FLG_S   = '0'")                              ' 0:���f�Ȃ�
                    '    SQL.Append("   AND FURI_KIN_S      >  0")                               ' �U�֍ϋ��z����
                    '    SQL.Append("   AND EXISTS")                                             ' 1:�ꊇ����
                    '    SQL.Append("       (SELECT TORIS_CODE_T")
                    '    SQL.Append("        FROM TORIMAST")
                    '    SQL.Append("        WHERE TORIS_CODE_T    = TORIS_CODE_S")
                    '    SQL.Append("          AND TORIF_CODE_T    = TORIF_CODE_S")
                    '    SQL.Append("          AND TESUUTYO_KBN_T  = '1'))")
                    'End If
                    '*** �萔���݂͓̂��삳���Ȃ� 2009.10.29 end 
            End Select

            If Up_FLG = True Then
                Call MainDB.ExecuteNonQuery(SQL)
            Else
                MainLOG.Write("�X�P�W���[���}�X�^�X�V", "����", "�X�P�W���[���X�V�ΏۊO�i�萔����������j�B" & "������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE)
            End If

        Catch ex As Exception
            MainLOG.Write("�X�P�W���[���}�X�^�X�V", "���s", "������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�֓��F" & key.FURI_DATE & " " & ex.Message)
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

    ' �@�\�@ �F �X�P�W���[���}�X�^�i�w�Z���U�j�̊w�N�t���O���擾
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function GetGakunenFLG(ByRef Key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraGSCHReader As New CASTCommon.MyOracleReader(MainDB)

        Key.GAKUNEN_FLG = New ArrayList(8)

        Try
            ' FURI_KBN_S =  0�F���U/ 1�F�ĐU/ 2�F����/ 3�F�o��
            ' TORIF_CODE = 01�F���U/02�F�ĐU/03�F����/04�F�o��

            SQL.Append("SELECT *")
            SQL.Append(" FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Key.TORIS_CODE))
            SQL.Append("   AND FURI_KBN_S = " & SQ(CInt(Key.TORIF_CODE) - 1))
            SQL.Append("   AND FURI_DATE_S = " & SQ(Key.FURI_DATE))
            '2017/03/14 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ START
            '���ʐU�֓��Ή�
            '���ʐU�֓��̏ꍇ�A�w�Z�X�P�W���[���}�X�^�͓���U�֓��ŕ������R�[�h�����Ƃ��ł���̂ŁA
            '�������R�[�h���݂���ꍇ�͊w�N�t���O���}�[�W����
            Dim GakunenFlg(8) As String
            '�w�N�t���O�͂��ׂ�0�ŏ�����
            For i As Integer = 0 To GakunenFlg.Length - 1
                GakunenFlg(i) = "0"
            Next

            If OraGSCHReader.DataReader(SQL) = True Then
                While OraGSCHReader.EOF = False
                    For iCount As Integer = 1 To 9
                        If OraGSCHReader.GetString("GAKUNEN" & iCount.ToString & "_FLG_S").Equals("1") = True Then
                            GakunenFlg(iCount - 1) = OraGSCHReader.GetString("GAKUNEN" & iCount.ToString & "_FLG_S")
                        End If
                    Next

                    OraGSCHReader.NextRead()
                End While
            Else
                Throw New Exception("�X�P�W���[���}�X�^�i�w�Z���U�j�ɊY���f�[�^�����݂��܂���B")
            End If

            For i As Integer = 0 To GakunenFlg.Length - 1
                Key.GAKUNEN_FLG.Add(GakunenFlg(i))
            Next

            'If OraGSCHReader.DataReader(SQL) = True Then
            '    For iCount As Integer = 1 To 9
            '        Key.GAKUNEN_FLG.Add(OraGSCHReader.GetString("GAKUNEN" & iCount & "_FLG_S"))
            '    Next iCount
            'Else
            '    Throw New Exception("�X�P�W���[���}�X�^�i�w�Z���U�j�ɊY���f�[�^�����݂��܂���B")
            'End If
            '2017/03/14 �^�X�N�j���� CHG �W���ŏC���i���݃o�O�C���j------------------------------------ END

            Return True
        Catch ex As Exception
            MainLOG.Write("�w�N�t���O�擾����", "���s", ex.Message)
            Return False
        Finally
            OraGSCHReader.Close()
            OraGSCHReader = Nothing
        End Try
    End Function

    ' �@�\�@ �F ���σ��[�N�}�X�^�쐬����
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function CreateMAIN0500G_WORK(ByVal Key As KeyInfo) As Boolean
        Dim HimoSQL As New StringBuilder(128)
        Dim OraHimoReader As New CASTCommon.MyOracleReader(MainDB)
        Dim GakunenFlg As String
        Dim KeyGak As KeyGakInfo = Nothing

        Try
            ' ���σ��[�N�}�X�^�폜����
            If DeleteMAIN0500G_WORK() = False Then
                Return False
            End If

            ' �w�N�t���O�擾����
            If GetGakunenFLG(Key) = False Then
                Return False
            End If

            HimoSQL.Append("SELECT")
            HimoSQL.Append("  HIMOMAST.*")
            HimoSQL.Append("  ,G_MEIMAST.*")
            HimoSQL.Append(" FROM")
            HimoSQL.Append("  HIMOMAST")
            HimoSQL.Append(" ,G_MEIMAST")
            HimoSQL.Append(" WHERE")
            HimoSQL.Append("      GAKKOU_CODE_M = GAKKOU_CODE_H")
            HimoSQL.Append("  AND GAKUNEN_CODE_M = GAKUNEN_CODE_H")
            HimoSQL.Append("  AND GAKKOU_CODE_M = " & SQ(Key.TORIS_CODE))
            HimoSQL.Append("  AND FURI_DATE_M = " & SQ(Key.FURI_DATE))
            HimoSQL.Append("  AND FURI_KBN_M = " & SQ(CInt(Key.TORIF_CODE) - 1))
            HimoSQL.Append("  AND HIMOKU_ID_H ='000'")                      ' 000:���ό���
            HimoSQL.Append(" AND (")

            Dim bLoopFlg As Boolean = False

            ' �����Ώۂ̊w�N�̂ݒ��o
            For iCount As Integer = 1 To Key.GAKUNEN_FLG.Count
                GakunenFlg = CType(Key.GAKUNEN_FLG.Item(iCount - 1), String)

                If GakunenFlg = "1" Then
                    If bLoopFlg = True Then
                        HimoSQL.Append(" OR ")
                    End If
                    HimoSQL.Append(" GAKUNEN_CODE_H = " & iCount)
                    bLoopFlg = True
                End If
            Next iCount

            HimoSQL.Append(" )")
            HimoSQL.Append(" ORDER BY GAKKOU_CODE_H,GAKUNEN_CODE_H,HIMOKU_ID_H,TUKI_NO_H")

            If OraHimoReader.DataReader(HimoSQL) = True Then
                Do While OraHimoReader.EOF = False

                    For iHimokuNo As Integer = 1 To 15
                        ' ������
                        KeyGak.Init()

                        ' �ŏ��̃L�[�ݒ�
                        Call KeyGak.SetOraDataGak(OraHimoReader, iHimokuNo.ToString("00"))

                        ' ��ږ����ݒ肳��Ă��郌�R�[�h��ΏۂƂ���
                        If KeyGak.HIMOKU_NAME <> "" Then
                            Dim WorkSQL As New StringBuilder(128)
                            Dim OraWorkReader As New CASTCommon.MyOracleReader(MainDB)

                            Try
                                ' ����̔�ڌ����P�ʂŌ��σ��[�N�}�X�^������
                                WorkSQL.Append(" SELECT * FROM MAIN0500G_WORK")
                                WorkSQL.Append(" WHERE")
                                WorkSQL.Append("     GAKKOU_CODE_H = " & SQ(KeyGak.GAKKOU_CODE))
                                WorkSQL.Append(" AND KESSAI_KIN_CODE_H = " & SQ(KeyGak.KESSAI_KIN_CODE))
                                WorkSQL.Append(" AND KESSAI_TENPO_H = " & SQ(KeyGak.KESSAI_TENPO))
                                WorkSQL.Append(" AND KESSAI_KAMOKU_H = " & SQ(KeyGak.KESSAI_KAMOKU))
                                WorkSQL.Append(" AND KESSAI_KOUZA_H = " & SQ(KeyGak.KESSAI_KOUZA))
                                WorkSQL.Append(" AND HIMOKU_NO_H = " & iHimokuNo)

                                ' ��ڌ����P�ʂŔ�ڋ��z�����Z����
                                If OraWorkReader.DataReader(WorkSQL) = True Then
                                    ' ����̔�ڌ��������݂���ꍇ

                                    ' ��ڋ��z�����Z����
                                    KeyGak.HIMOKU_KINGAKU += OraWorkReader.GetInt64("HIMOKU_KINGAKU_H")
                                    KeyGak.HIMOKU_FURI_KIN += OraWorkReader.GetInt64("HIMOKU_FURI_KIN_H")
                                    KeyGak.HIMOKU_FUNOU_KIN += OraWorkReader.GetInt64("HIMOKU_FUNOU_KIN_H")
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                    '�U�֍ς݂̂݃J�E���g�A�b�v����
                                    If KeyGak.FURIKETU_CODE = "0" Then
                                        KeyGak.HIMOKU_GASSAN = OraWorkReader.GetInt64("YOBI1_H") + 1
                                    Else
                                        KeyGak.HIMOKU_GASSAN = OraWorkReader.GetInt64("YOBI1_H")
                                    End If
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                                    ' ���σ��[�N�}�X�^�X�V����
                                    If UpdateMAIN0500G_WORK(KeyGak, iHimokuNo) = False Then
                                        Return False
                                    End If
                                Else
                                    ' ����̔�ڌ��������݂��Ȃ��ꍇ
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
                                    If KeyGak.FURIKETU_CODE = "0" Then
                                        KeyGak.HIMOKU_GASSAN = 1
                                    Else
                                        '�s�\�̏ꍇ�̓��R�[�h�����쐬����
                                        KeyGak.HIMOKU_GASSAN = 0
                                    End If
                                    '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END

                                    ' ���σ��[�N�}�X�^�o�^����
                                    If InsertMAIN0500G_WORK(KeyGak, iHimokuNo, Key) = False Then
                                        Return False
                                    End If
                                End If
                            Catch ex As Exception
                                MainLOG.Write("���σ��[�N�}�X�^�쐬����", "���s", ex.Message)
                                Return False
                            Finally
                                If Not OraWorkReader Is Nothing Then
                                    OraWorkReader.Close()
                                    OraWorkReader = Nothing
                                End If
                            End Try

                        End If
                    Next iHimokuNo

                    ' �Ώۃf�[�^�̎����R�[�h��Ǎ���
                    OraHimoReader.NextRead()
                Loop
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("���σ��[�N�}�X�^�쐬����", "���s", ex.Message)
            Return False
        Finally
            If Not OraHimoReader Is Nothing Then
                OraHimoReader.Close()
                OraHimoReader = Nothing
            End If
        End Try

    End Function

    ' �@�\�@ �F ���σ��[�N�}�X�^�폜����
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function DeleteMAIN0500G_WORK() As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            '���σ��[�N�}�X�^�폜
            '------------------------------------------------
            SQL.Append("DELETE FROM MAIN0500G_WORK")

            ' ���σ��[�N�}�X�^���[�����̏ꍇ�́A�G���[�Ƃ��Ȃ�
            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                Throw New Exception(MainDB.Message)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("���σ��[�N�}�X�^�폜����", "���s", ex.Message)
            Return False
        End Try

    End Function

    ' �@�\�@ �F ���σ��[�N�}�X�^�o�^����
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertMAIN0500G_WORK(ByVal KeyGak As KeyGakInfo, ByVal iHimokuNo As Integer, ByVal Key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            '���σ��[�N�}�X�^�o�^
            '------------------------------------------------
            SQL.Append("INSERT INTO MAIN0500G_WORK(")
            SQL.Append(" GAKKOU_CODE_H")
            SQL.Append(",HIMOKU_NAME_H")
            SQL.Append(",KESSAI_KIN_CODE_H")
            SQL.Append(",KESSAI_TENPO_H")
            SQL.Append(",KESSAI_KAMOKU_H")
            SQL.Append(",KESSAI_MEIGI_H")
            SQL.Append(",KESSAI_KOUZA_H")
            SQL.Append(",HIMOKU_KINGAKU_H")
            SQL.Append(",HIMOKU_FURI_KIN_H")
            SQL.Append(",HIMOKU_FUNOU_KIN_H")
            SQL.Append(",GAKUNEN_H")
            SQL.Append(",HIMOKU_NO_H")
            SQL.Append(",FURI_KBN_H")
            SQL.Append(",YOBI1_H")
            SQL.Append(",YOBI2_H")
            SQL.Append(",YOBI3_H")
            SQL.Append(") VALUES (")
            SQL.Append(SQ(KeyGak.GAKKOU_CODE))                      ' �w�Z�R�[�h
            SQL.Append("," & SQ(KeyGak.HIMOKU_NAME))                ' ��ږ�
            SQL.Append("," & SQ(KeyGak.KESSAI_KIN_CODE))            ' ���ϋ��Z�@�փR�[�h
            SQL.Append("," & SQ(KeyGak.KESSAI_TENPO))               ' ���ώx�X�R�[�h
            SQL.Append("," & SQ(KeyGak.KESSAI_KAMOKU))              ' ���ωȖ�
            SQL.Append("," & SQ(KeyGak.KESSAI_MEIGI))               ' ���ϖ��`�l
            SQL.Append("," & SQ(KeyGak.KESSAI_KOUZA))               ' ���ό����ԍ�
            SQL.Append("," & KeyGak.HIMOKU_KINGAKU)                 ' ��ڋ��z
            SQL.Append("," & KeyGak.HIMOKU_FURI_KIN)                ' ��ڐU�֋��z
            SQL.Append("," & KeyGak.HIMOKU_FUNOU_KIN)               ' ��ڕs�\���z
            SQL.Append(",0")                                        ' �w�N�R�[�h�����g�p�̂��ߏ����l�ݒ�
            SQL.Append("," & iHimokuNo)                             ' ��ڔԍ�
            SQL.Append("," & CInt(Key.TORIF_CODE) - 1)              ' �U�֋敪
            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            SQL.Append("," & SQ(KeyGak.HIMOKU_GASSAN.ToString))     ' �\��1(��ڔԍ��ʂ̍��Z��)
            'SQL.Append(",''")                                       ' �\��1     �����g�p�̂��ߏ����l�ݒ�
            '2017/03/16 �^�X�N�j���� CHG �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            SQL.Append(",''")                                       ' �\��2     �����g�p�̂��ߏ����l�ݒ�
            SQL.Append(",''")                                       ' �\��3     �����g�p�̂��ߏ����l�ݒ�
            SQL.Append(")")

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                Throw New Exception(MainDB.Message)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("���σ��[�N�}�X�^�o�^����", "���s", ex.Message)
            Return False
        End Try

    End Function

    ' �@�\�@ �F ���σ��[�N�}�X�^�X�V����
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateMAIN0500G_WORK(ByVal KeyGak As KeyGakInfo, ByVal iHimokuNo As Integer) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            '���σ��[�N�}�X�^�X�V
            '------------------------------------------------
            SQL.Append(" UPDATE MAIN0500G_WORK SET")
            SQL.Append("  HIMOKU_KINGAKU_H = " & KeyGak.HIMOKU_KINGAKU)
            SQL.Append(" ,HIMOKU_FURI_KIN_H = " & KeyGak.HIMOKU_FURI_KIN)
            SQL.Append(" ,HIMOKU_FUNOU_KIN_H = " & KeyGak.HIMOKU_FUNOU_KIN)
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ START
            SQL.Append(" ,YOBI1_H = " & SQ(KeyGak.HIMOKU_GASSAN.ToString))
            '2017/03/16 �^�X�N�j���� ADD �W���ŏC���i�������ϐ��\���P�j------------------------------------ END
            SQL.Append(" WHERE")
            SQL.Append("     GAKKOU_CODE_H =" & SQ(KeyGak.GAKKOU_CODE))
            SQL.Append(" AND KESSAI_KIN_CODE_H =" & SQ(KeyGak.KESSAI_KIN_CODE))
            SQL.Append(" AND KESSAI_TENPO_H =" & SQ(KeyGak.KESSAI_TENPO))
            SQL.Append(" AND KESSAI_KAMOKU_H =" & SQ(KeyGak.KESSAI_KAMOKU))
            SQL.Append(" AND KESSAI_KOUZA_H =" & SQ(KeyGak.KESSAI_KOUZA))
            SQL.Append(" AND HIMOKU_NO_H =" & iHimokuNo)

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then
                Throw New Exception(MainDB.Message)
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("���σ��[�N�}�X�^�X�V����", "���s", ex.Message)
            Return False
        End Try

    End Function

    ' �@�\�@ �F ���σ��[�N�}�X�^��������
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function SelectMAIN0500G_WORK(ByRef lstHimokuData As ArrayList) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraWorkReader As New CASTCommon.MyOracleReader(MainDB)
        Dim Key As KeyInfo = Nothing

        lstHimokuData = New ArrayList(128)

        Try
            SQL.Append("SELECT *")
            SQL.Append(" FROM MAIN0500G_WORK")
            SQL.Append(" WHERE HIMOKU_FURI_KIN_H > 0")          ' ��ڐU�֋��z����
            SQL.Append(" ORDER BY")
            SQL.Append("   GAKKOU_CODE_H")
            SQL.Append("  ,KESSAI_KAMOKU_H DESC")
            SQL.Append("  ,KESSAI_KIN_CODE_H")
            SQL.Append("  ,KESSAI_TENPO_H")
            SQL.Append("  ,KESSAI_KOUZA_H")
            SQL.Append("  ,HIMOKU_NO_H")

            If OraWorkReader.DataReader(SQL) = True Then
                Do While OraWorkReader.EOF = False
                    ' �L�[������
                    Key.Init()

                    ' �L�[�ݒ�
                    Key.SetOraDataHimo(OraWorkReader)

                    ' ��ڌ����P�ʂ̌��Ϗ����Z�b�g
                    lstHimokuData.Add(Key)

                    ' �Ώۃf�[�^�̎����R�[�h��Ǎ���
                    OraWorkReader.NextRead()
                Loop
            Else
                lstHimokuData = Nothing
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("���σ��[�N�}�X�^��������", "���s", ex.Message)
            Return False
        Finally
            OraWorkReader.Close()
            OraWorkReader = Nothing
        End Try

    End Function

    Private Function MakeRientaFD(ByVal ary As ArrayList) As Boolean
        Dim T_RIENT77 As New CAstFormKes.ClsT_RIENT77
        Dim T_RIENT10 As New CAstFormKes.ClsT_RIENT10

        Dim Kdata As CAstFormKes.ClsFormKes.KessaiData
        Dim EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")

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

        Dim StrmWrite As FileStream = Nothing

        Try

            ' �^���L���O�w�b�_  
            ' �����f�[�^�ݒ�
            Call T_RIENT77.TANKING_HEAD.Init()
            ' �s���t
            T_RIENT77.TANKING_HEAD.strT_HIDUKE = CASTCommon.Calendar.Now.ToString("yyyyMMdd")

            ' �X�܏�񃌃R�[�h�i�P�X�܏��j
            ' �����f�[�^�ݒ�
            Call T_RIENT77.TENPO_INFOREC(0).Init()
            ' ���ɃR�[�h
            T_RIENT77.TENPO_INFOREC(0).strKINKO_CD = ini_info.JIKINKO_CODE
            ' �X�܃R�[�h
            T_RIENT77.TENPO_INFOREC(0).strSIT_CD = ini_info.HONBU_CODE

            ' �X�܏�񃌃R�[�h�i�Q�`�R�Q�X�܏��j
            ' �����f�[�^�ݒ�
            For i As Integer = 1 To T_RIENT77.TENPO_INFOREC.Length - 1
                ' �Q�`�R�Q�̏����f�[�^�ݒ�
                Call T_RIENT77.TENPO_INFOREC(i).Init2_32()
            Next i

            ' �\���R
            ' ������
            Call T_RIENT77.DATA_SIKIBETU.Init()

            ' ���G���^�t�@�C�� �I�[�v��

            If File.Exists(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME)) = True Then
                ' ���ɑ��݂���ꍇ�́C�폜
                File.Delete(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME))
            End If

            StrmWrite = New FileStream(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME), FileMode.OpenOrCreate, FileAccess.ReadWrite)

            Dim Bytes As Byte() = EncdJ.GetBytes(ini_info.RIENTA_FILENAME.PadRight(12, Nothing).PadRight(16, " "c))
            StrmWrite.Write(Bytes, 0, 16)

            ' �^���L���O�w�b�_����
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.Data, 0, 28)

            ' �X�܏�񃌃R�[�h����
            For i As Integer = 0 To T_RIENT77.TENPO_INFOREC.Length - 1
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(i).Data, 0, 28)
            Next i

            ' �\���R����
            StrmWrite.Write(T_RIENT77.DATA_SIKIBETU.Data, 0, 84)

            Dim WriteCount As Integer = 0           ' ��������

            Dim RecLen As Integer

            ' �^���L���O�f�[�^����

            Dim cnt As Integer = ary.Count - 1 '���[�v��

            For i As Integer = 0 To cnt

                ' �^���L���O�f�[�^
                Kdata = CType(ary.Item(i), CAstFormKes.ClsFormKes.KessaiData)

                Select Case Kdata.OpeCode
                    Case "01010"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_01010.DataSepaPlus = Kdata.record320
                        RecLen = T_01010.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_01010.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3               '(�W���[�i���E�`�[)
                        T_RIENT10.TANKING_DATA.bytNYURYOKU(0) = 24
                        T_01010 = Nothing
                    Case "02019"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_02019.DataSepaPlus = Kdata.record320
                        RecLen = T_02019.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_02019.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 11                '(�W���[�i���E�`�[�E�؏�)
                        T_RIENT10.TANKING_DATA.bytNYURYOKU(0) = 24
                        T_02019 = Nothing
                    Case "04019"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_04019.DataSepaPlus = Kdata.record320
                        RecLen = T_04019.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_04019.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3                '(�W���[�i���E�`�[)
                        T_04099 = Nothing
                    Case "04099"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_04099.DataSepaPlus = Kdata.record320
                        RecLen = T_04099.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_04099.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3                '(�W���[�i���E�`�[)
                        T_04099 = Nothing
                    Case "04419"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_04419.DataSepaPlus = Kdata.record320
                        RecLen = T_04419.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_04419.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3                '(�W���[�i���E�`�[)
                        T_04419 = Nothing
                    Case "48100"
                        T_RIENT10.TANKING_DATA.Init_48()
                        T_48100.DataSepaPlus = Kdata.record320
                        RecLen = T_48100.DataSepaPlus.Length + 32
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48100.DataSepaPlus
                        T_48100 = Nothing
                    Case "48500"
                        T_RIENT10.TANKING_DATA.Init_48()
                        T_48500.DataSepaPlus = Kdata.record320
                        'RecLen = T_48500.DataSepaPlus.Replace(" ", "").Length + 32
                        RecLen = T_48500.DataSepaPlus.Length + 32
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48500.DataSepaPlus
                        T_48500 = Nothing
                    Case "48600"
                        T_RIENT10.TANKING_DATA.Init_48()
                        T_48600.DataSepaPlus = Kdata.record320
                        RecLen = T_48600.DataSepaPlus.Length + 32
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48600.DataSepaPlus
                        T_48600 = Nothing
                    Case "99019"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_99019.DataSepaPlus = Kdata.record320
                        RecLen = T_99019.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_99019.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 11           '(�W���[�i���E�`�[�E�؏�)
                        T_99019 = Nothing
                    Case "99418"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_99418.DataSepaPlus = Kdata.record320
                        RecLen = T_99418.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_99418.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 3           '(�W���[�i���E�`�[)
                        T_99418 = Nothing
                    Case "99419"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_99419.DataSepaPlus = Kdata.record320
                        RecLen = T_99419.DataSepaPlus.Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_99419.DataSepaPlus
                        T_RIENT10.TANKING_DATA.bytBAITAI_SET = 11           '(�W���[�i���E�`�[�E�؏�)
                        T_99419 = Nothing
                    Case Else
                        '�G���[
                End Select

                WriteCount += 1

                ' ���z�Z�p���[�^
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)

                ' �^���L���O�A��
                T_RIENT10.TANKING_DATA.bytTANKING_NO(0) = CType(WriteCount \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytTANKING_NO(1) = CType(WriteCount Mod 256, Byte)

                ' ���f�[�^�A�h���X
                If cnt + 1 = WriteCount Then
                    ' �ŏI�s
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = 255
                Else
                    Dim NextAddr As Integer = 1024 + (WriteCount * 256)

                    Dim NextAddr0 As Integer = CType(NextAddr \ 16777216, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = CType(NextAddr0, Byte)

                    Dim NextAddr1 As Integer = CType((NextAddr Mod 16777216) \ 65536, Integer)
                    Dim Amari1 As Integer = CType((NextAddr Mod 16777216) Mod 65536, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = CType(NextAddr1, Byte)

                    Dim NextAddr2 As Integer = CType(Amari1 \ 256, Integer)
                    Dim Amari2 As Integer = CType(Amari1 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = CType(NextAddr2, Byte)

                    Dim NextAddr3 As Integer = CType(Amari2 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = CType(NextAddr3, Byte)
                End If

                ' ���X�R�[�h
                T_RIENT10.TANKING_DATA.strKINTEN_CD = T_RIENT77.TENPO_INFOREC(0).strKINKO_CD & T_RIENT77.TENPO_INFOREC(0).strSIT_CD

                Select Case Kdata.OpeCode.Substring(0, 2)
                    Case "48"
                        StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_48, 0, 256)
                    Case Else
                        StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_10, 0, 256)
                End Select

                '*****************************************
                '���σ}�X�^�ɓo�^
                '*****************************************
                keskey.RecordNo = WriteCount
                If InsertKesMast(Kdata) = False Then
                    MainLOG.Write("�������σ}�X�^�o�^", "���s", "")
                    Return False
                End If
            Next

            ' �ŏI���R�[�h
            T_RIENT10.TANKING_LAST.Init()
            StrmWrite.Write(T_RIENT10.TANKING_LAST.Data, 0, 512)

            ' �^���L���O�w�b�_ �������� �ď���
            ' �S�X�܂s����
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(20 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN, 0, 2)

            ' �X�܂s����
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(36 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN, 0, 2)

            ' �X�܂s�I���e�c�A�h���X
            '***�C���@�O�c�@20080930****************************************
            'WriteCount��1�ȊO�Ȃ�A�I���A�h���X�𐳂����ݒ肷��
            If WriteCount <> 1 Then

                '***�C�� maeda 2001006*******************************************
                '��������-1�̒l�ŏI���A�h���X���v�Z����(���m�ɂ͍ŏI���R�[�h�̊J�n�A�h���X�̂���)
                Dim FinishAddr As Integer = 1024 + ((WriteCount - 1) * 256)
                'Dim FinishAddr As Integer = 1024 + (WriteCount * 256)
                '****************************************************************

                Dim FinishAddr0 As Integer = CType(FinishAddr \ 16777216, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(0) = CType(FinishAddr0, Byte)

                Dim FinishAddr1 As Integer = CType((FinishAddr Mod 16777216) \ 65536, Integer)
                Dim FinishAmari1 As Integer = CType((FinishAddr Mod 16777216) Mod 65536, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(1) = CType(FinishAddr1, Byte)

                Dim FinishAddr2 As Integer = CType(FinishAmari1 \ 256, Integer)
                Dim FinishAmari2 As Integer = CType(FinishAmari1 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(2) = CType(FinishAddr2, Byte)

                Dim FinishAddr3 As Integer = CType(FinishAmari2 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(3) = CType(FinishAddr3, Byte)
            End If

            StrmWrite.Seek(48 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD, 0, 4)

            StrmWrite.Close()

        Catch ex As Exception
            MainLOG.Write("���G���^�t�@�C���쐬", "���s", ex.Message)
            Return False
        Finally

        End Try
        Return True
    End Function

    Private Function GetKaiji() As Boolean

        Dim sql As New StringBuilder(64)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            MainLOG.Write("(�񎟎擾)�J�n", "����", "")

            sql.Append("SELECT NVL(MAX(KAIJI_KR),0) AS MAX_KAIJI FROM KESSAIMAST")
            sql.Append(" WHERE SYORI_DATE_KR = " & SQ(strDate))

            If OraReader.DataReader(sql) = True Then
                keskey.Kaiji = CType(OraReader.GetInt64("MAX_KAIJI"), Integer) + 1
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("(�񎟎擾)", "���s", ex.ToString)
            Return False
        Finally
            MainLOG.Write("(�񎟎擾)�I��", "����", "")
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return True

    End Function


    Private Function GetTENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(MainDB)

        Try
            KIN_NNAME = ""
            SIT_NNAME = ""
            KIN_KNAME = ""
            SIT_KNAME = ""

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & KIN_NO.Trim & "' AND SIT_NO_N = '" & SIT_NO.Trim & "'")

            If orareader.DataReader(sql) = True Then
                KIN_NNAME = orareader.GetString("KIN_NNAME_N")
                SIT_NNAME = orareader.GetString("SIT_NNAME_N")
                KIN_KNAME = orareader.GetString("KIN_KNAME_N")
                SIT_KNAME = orareader.GetString("SIT_KNAME_N")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

    '�������ρE�萔���������s�҂��̃t���O�����ׂČ��ɖ߂�
    Public Function ReturnFlg() As Integer
        Dim SQL As String
        Dim Ret As Integer = 0
        Try
            '�O�̂��߁A��[�N���[�Y
            If Not MainDB Is Nothing Then MainDB.Close()
            MainDB = New CASTCommon.MyOracle

            SQL = "UPDATE SCHMAST SET"
            SQL &= " KESSAI_FLG_S = '0'"
            SQL &= " WHERE KESSAI_YDATE_S = '" & ParaKessaiDate & "'"
            SQL &= "   AND KESSAI_FLG_S = '2'"

            Ret = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("���ϑ҂����", "����", Ret & "��")

            SQL = "UPDATE SCHMAST SET"
            SQL &= " TESUUTYO_FLG_S = '0'"
            SQL &= " WHERE TESUU_YDATE_S = '" & ParaKessaiDate & "'"
            SQL &= "   AND TESUUTYO_FLG_S = '2'"

            Ret = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("�萔�������҂����", "����", Ret & "��")

            MainDB.Commit()
            Return 0
        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write("���ϥ�萔�������҂����", "���s", ex.ToString)
            Return -1
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function

    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
    ''' <summary>
    ''' ���ωȖڂ́u���̑��v�̐M���ȖڃR�[�h���擾����
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetKessaiKamokuCode_ETC()

        Dim TxtFile As String = Path.Combine(GetFSKJIni("COMMON", "TXT"), "Common_���ωȖ�.TXT")

        '�t�@�C����������Δ�����
        '2017/07/25 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
        'If Not File.Exists(TxtFile) Then Return
        If Not File.Exists(TxtFile) Then
            KessaiKamokuCode_ETC = "xx"
            Return
        End If
        '2017/07/25 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END

        Try
            Using sr As New StreamReader(TxtFile, Encoding.GetEncoding("SHIFT-JIS"))
                While sr.Peek > -1
                    Dim strLineData() As String = sr.ReadLine().Split(","c)
                    If strLineData(1).Trim = "���̑�" Then
                        KessaiKamokuCode_ETC = strLineData(0).Trim
                        '2017/07/25 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
                        'Exit While
                        Return
                        '2017/07/25 �^�X�N�j���� CHG �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END
                    End If
                End While
            End Using

            '2017/07/25 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
            KessaiKamokuCode_ETC = "xx"
            '2017/07/25 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END

        Catch ex As Exception
            MainLOG.Write("�uCommon_���ωȖ�.TXT�v�Ǎ����s", "���s", ex.ToString)
            '2017/07/25 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- START
            KessaiKamokuCode_ETC = "xx"
            '2017/07/25 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END
        End Try

    End Sub
    '2017/05/29 �^�X�N�j���� ADD �W���ŏC���i���ωȖڂɁuxx:���̑��v��ǉ��j-------------------------- END

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
