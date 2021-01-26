Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstFormKes
Imports System.Collections.Generic
Imports System.Collections
'*** Str Add 2016/01/11 �^�X�N)�֓� for ���d���s�Ή� ***
Imports Microsoft.VisualBasic
'*** End Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***

'�U�����M�f�[�^�쐬����
Public Class ClsFurikomiDataCreateKinBatch

    '***************************************************************************************************
    '�K�ǁI
    '�ב֐����Ɋւ���C���͈ב֐������G���^�쐬�����C�����邱��
    '***************************************************************************************************

    Private Const FD_COUNT_LIMIT As Integer = 5000 '���G���^�e�c�P��������̍ő匏�����Z�b�g����

    Public MainLOG As New CASTCommon.BatchLOG("KFS045", "�U�����M�f�[�^�쐬")

    Private MainDB As CASTCommon.MyOracle

    ' �p�u���b�N�t�H�[�}�b�g
    Private FmtComm As New CAstFormat.CFormat

    Private strKEKKA As String              ' �f�[�^�쐬����

    Private jobMessage As String = ""          ' �W���u�Ď����b�Z�[�W

    ' �������t
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ''' <summary>
    ''' ini�t�@�C�����
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni
        Dim KAWASE_CENTER As String          '���M�Z���^��
        Dim JIKINKO_CODE As String           '�����ɃR�[�h
        Dim JIKINKO_NAME As String           '�����ɖ�
        Dim HONBU_CODE As String             '�{���R�[�h
        Dim TESUU_KOUZA1 As String           '�萔�����������P
        Dim UCHIWAKE1 As String              '����P
        Dim TESUU_KOUZA2 As String           '�萔�����������Q
        Dim UCHIWAKE2 As String              '����Q
        Dim TESUU_KOUZA3 As String           '�萔�����������R
        Dim UCHIWAKE3 As String              '����R
        Dim TESUU_KOUZA4 As String           '�萔�����������S
        Dim UCHIWAKE4 As String              '����S
        Dim TESUU_KOUZA5 As String           '�萔�����������T
        Dim UCHIWAKE5 As String              '����T
        Dim KAWASE_IRAININ As String         '�בֈ˗��l��
        Dim RIENTA_PATH As String            '���G���^�t�@�C���쐬��
        Dim DAT_PATH As String               'DAT�̃p�X
        Dim CSVPATH As String                'CSV�t�@�C���쐬��
        Dim TESUU_OPEKBN As String           '�萔���I�y�敪
        Dim RIENTA_FILENAME As String        '���G���^�t�@�C����
        Dim SIKINTEKIYOU As String           '�E�v���̈ꕔ��INI�t�@�C�����擾���邽��
        Dim SEIKYU As String                 '�ב֐����ݒ�
        '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
        Dim ZEIKIJUN As String               '����Ŋ
        '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<
        ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
        Dim RSV2_S_KAWASE_HONSITEN As String ' �ב֐U�����ו[(�{�x�X�ב�)����v��
        Dim RSV2_S_KAWASE_TAKOU As String    ' �ב֐U�����ו[(���s�ב�)����v��
        Dim RSV2_S_KAWASE_LOGGING As String  ' �ב֐U�����ו[(���M���O)����v��
        ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END
    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' ���M�}�X�^�̃L�[���ځ{���G���^�t�@�C����
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure hasmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' ������
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private haskey As hasmastKey

    ''' <summary>
    ''' ���U���σ}�X�^�̃L�[���ځ{���G���^�t�@�C����
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

    Private ParaHassinDate As String     '�p�����[�^��������p�������M��
    Private ParaKyuDate As String        '�p�����[�^��������p�������U�\��
    Private ParaSouDate As String        '�p�����[�^��������p�������U�\��

    Private HassinList As New List(Of CAstFormKes.ClsFormKes.KessaiData) '�U�����M�f�[�^�i�[�p
    Private HONBU_KNAME As String = ""

    Private TesuuList As New System.Collections.Hashtable
    Private Structure TesuuTable
        Dim TESUU_A1 As Integer
        Dim TESUU_A2 As Integer
        Dim TESUU_A3 As Integer
        Dim TESUU_B1 As Integer
        Dim TESUU_B2 As Integer
        Dim TESUU_B3 As Integer
        Dim TESUU_C1 As Integer
        Dim TESUU_C2 As Integer
        Dim TESUU_C3 As Integer
    End Structure

    Private JIKOU_KEN As Long = 0         '���s����
    Private TAKOU_KEN As Long = 0         '���s������
    Private JIFURI_KEN As Long = 0        '���U���M���O����

    Private Structure FKeyInfo
        '���M�f�[�^�쐬�����[�b�r�u�f�[�^�쐬���p
        Dim TORIS_CODE As String            ' ������R�[�h
        Dim TORIF_CODE As String            ' ����敛�R�[�h
        Dim BAITAI_CODE As String           ' �}�̃R�[�h
        Dim FURI_DATE As String             ' �U����
        Dim MOTIKOMI_SEQ As Integer         ' ����SEQ
        Dim SOUSIN_KBN As String            ' ���M�敪
        Dim SYUBETU As String               ' ���
        Dim SYUMOKU_CODE As String          ' ��ڃR�[�h
        Dim FUKA_CODE As String             ' �t���R�[�h
        Dim FURI_CODE As String             ' �U�փR�[�h
        Dim KIGYO_CODE As String            ' ��ƃR�[�h
        Dim ITAKU_CODE As String            ' �ϑ��҃R�[�h
        Dim ITAKU_KNAME As String           ' �ϑ��Җ��J�i
        Dim ITAKU_NNAME As String           ' �ϑ��Җ�����
        Dim TORIMATOME_SIT As String        ' ���܂ƂߓX
        Dim TUKEKIN_KNAME As String         ' ���Z�@�֖�
        Dim TUKESIT_KNAME As String         ' �x�X��
        Dim BIKOU1 As String                ' ���l�P
        Dim BIKOU2 As String                ' ���l�Q
        '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
        Dim TSIT_NO As String               ' �戵�x�X�R�[�h
        '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

        Dim TEKIYOU_SYUBETU As String       ' �K�p���
        Dim FURI_NAME As String             ' �U�֖���

        Dim TotalKen As Long                ' ���ׂ̍��v����
        Dim TotalKin As Long                ' ���ׂ̍��v���z
        Dim JikouKen As Long                ' ���ׂ̎��s���v����
        Dim JikouKin As Long                ' ���ׂ̎��s���v���z
        Dim TakouKen As Long                ' ���ׂ̑��s���v����
        Dim TakouKin As Long                ' ���ׂ̑��s���v���z
        Dim JifuriKen As Long               ' ���ׂ̎��U���M���O���v����
        Dim JifuriKin As Long               ' ���ׂ̎��U���M���O���v���z
        '2018/03/15 saitou �L���M��(RSV2�W��) ADD �X�P�W���[���}�X�^�̎萔���X�V -------------------- START
        Dim TesuuKin As Long                ' ���ׂ̎萔�����v
        '2018/03/15 saitou �L���M��(RSV2�W��) ADD --------------------------------------------------- END

        Dim MESSAGE As String

        Dim TOUROKU_DATE As String          ' ��t��
        Dim TUKEKIN_NO As String            ' ���ϋ��Z�@�փR�[�h
        Dim TUKESIT_NO As String            ' ���ώx�X�R�[�h
        Dim TUKEKAMOKU As String            ' ���ωȖ�
        Dim TUKEKOUZA As String             ' ���ό����ԍ�
        Dim KESSAI_PATN As String           ' ���σp�^�[��
        Dim TESUUTYO_KBN As String          ' �萔�������敪

        Dim FURIKIN_KEI As Long
        Dim FURIKIN_KEIJ As Long
        Dim FURIKIN_KEIH As Long
        Dim FURIKIN_KEIT As Long
        Dim TESUU_KIN_KEI As Long

        Dim TESUU_TABLE_ID As String        ' �U���萔���ID

        ' ������
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            BAITAI_CODE = ""
            FURI_DATE = ""
            MOTIKOMI_SEQ = 0
            SOUSIN_KBN = ""
            SYUBETU = ""
            SYUMOKU_CODE = ""
            FUKA_CODE = ""
            FURI_CODE = ""
            KIGYO_CODE = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            ITAKU_NNAME = ""
            TORIMATOME_SIT = ""
            TUKEKIN_KNAME = ""
            TUKESIT_KNAME = ""
            BIKOU1 = ""
            BIKOU2 = ""
            '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
            TSIT_NO = ""
            '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

            TEKIYOU_SYUBETU = ""
            FURI_NAME = ""

            TotalKen = 0
            TotalKin = 0
            JikouKen = 0
            JikouKin = 0
            TakouKen = 0
            TakouKin = 0
            JifuriKen = 0
            JifuriKin = 0
            '2018/03/15 saitou �L���M��(RSV2�W��) ADD �X�P�W���[���}�X�^�̎萔���X�V -------------------- START
            TesuuKin = 0
            '2018/03/15 saitou �L���M��(RSV2�W��) ADD --------------------------------------------------- END

            MESSAGE = ""

            TOUROKU_DATE = ""
            TUKEKIN_NO = ""
            TUKESIT_NO = ""
            TUKEKAMOKU = ""
            TUKEKOUZA = ""
            KESSAI_PATN = ""
            TESUUTYO_KBN = ""

            FURIKIN_KEI = 0
            FURIKIN_KEIJ = 0
            FURIKIN_KEIH = 0
            FURIKIN_KEIT = 0
            TESUU_KIN_KEI = 0

            TESUU_TABLE_ID = ""
        End Sub

        ' �c�a����̒l��ݒ�i�U�����M���G���^�쐬�p�j
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            MOTIKOMI_SEQ = oraReader.GetInt("MOTIKOMI_SEQ_S")
            SOUSIN_KBN = oraReader.GetString("SOUSIN_KBN_S")
            SYUBETU = oraReader.GetString("SYUBETU_S")
            SYUMOKU_CODE = oraReader.GetString("SYUMOKU_CODE_T")
            FUKA_CODE = oraReader.GetString("FUKA_CODE_T")
            FURI_CODE = oraReader.GetString("FURI_CODE_S")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_S")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_S")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            TUKEKIN_KNAME = oraReader.GetString("KIN_KNAME_N")
            TUKESIT_KNAME = oraReader.GetString("SIT_KNAME_N")
            BIKOU1 = oraReader.GetString("BIKOU1_T")
            BIKOU2 = oraReader.GetString("BIKOU2_T")
            '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
            TSIT_NO = oraReader.GetString("TSIT_NO_T")
            '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

            TOUROKU_DATE = oraReader.GetString("TOUROKU_DATE_S")
            TUKEKIN_NO = oraReader.GetString("TUKEKIN_NO_T")
            TUKESIT_NO = oraReader.GetString("TUKESIT_NO_T")
            TUKEKAMOKU = oraReader.GetString("TUKEKAMOKU_T")
            TUKEKOUZA = oraReader.GetString("TUKEKOUZA_T")
            KESSAI_PATN = oraReader.GetString("KESSAI_PATN_T")
            TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_T")

            TESUU_TABLE_ID = oraReader.GetString("TESUU_TABLE_ID_T")
        End Sub

    End Structure

    Private Structure KKeyInfo
        '���M�f�[�^�쐬�����[�b�r�u�f�[�^�쐬���p
        Dim TORIS_CODE As String            ' ������R�[�h
        Dim TORIF_CODE As String            ' ����敛�R�[�h
        Dim FURI_DATE As String             ' �U����
        Dim ITAKU_CODE As String            ' �ϑ��҃R�[�h
        Dim ITAKU_KNAME As String           ' �ϑ��Җ��J�i
        Dim TORIMATOME_SIT As String        ' ���܂ƂߓX
        Dim SIT_KNAME As String             ' ���܂ƂߓX��
        Dim KESSAI_KBN As String            ' ���ϋ敪
        Dim KESSAI_PATN As String           ' �����m�ە��@
        Dim BAITAI_CODE As String           ' �}�̃R�[�h

        Dim FURI_KEN As String              ' �U���ό���
        Dim FURI_KIN As String              ' �U���ϋ��z
        Dim MESSAGE As String

        ' ������
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            FURI_DATE = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            TORIMATOME_SIT = ""
            SIT_KNAME = ""
            KESSAI_KBN = ""
            KESSAI_PATN = ""
            BAITAI_CODE = ""

            FURI_KEN = ""
            FURI_KIN = ""

            MESSAGE = ""
        End Sub

        ' �c�a����̒l��ݒ�i�ב֐������G���^�쐬�p�j
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_S")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            SIT_KNAME = oraReader.GetString("SIT_KNAME_N")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_T")
            KESSAI_PATN = oraReader.GetString("KESSAI_PATN_T")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")

            FURI_KEN = oraReader.GetString("FURI_KEN_S")
            FURI_KIN = oraReader.GetString("FURI_KIN_S")
        End Sub

    End Structure

    '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    Private htFuriTesuuID As Hashtable
    Private Structure strcFuriTesuuInfo
        Dim TESUU_A1 As String
        Dim TESUU_A2 As String
        Dim TESUU_A3 As String
        Dim TESUU_B1 As String
        Dim TESUU_B2 As String
        Dim TESUU_B3 As String
        Dim TESUU_C1 As String
        Dim TESUU_C2 As String
        Dim TESUU_C3 As String
    End Structure
    '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' �U�����M��������
    ''' </summary>
    ''' <returns>����:True �ُ�:False</returns>
    ''' <remarks></remarks>
    Public Function Init(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try

            '�p�����[�^�̓Ǎ�
            param = CmdArgs(0).Split(","c)
            If param.Length = 4 Then

                '���O�����ݏ��̐ݒ�
                MainLOG.FuriDate = "00000000"
                MainLOG.JobTuuban = CInt(param(3))
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(��������)�J�n", "����")

                ParaHassinDate = param(0)                       '���M�����Z�b�g
                ParaKyuDate = param(1)                       '���U�\�����Z�b�g
                ParaSouDate = param(2)                       '���U�\�����Z�b�g

            Else
                MainLOG.Write("(��������)�J�n", "���s", "�R�}���h���C�������̃p�����[�^���s���ł�")
                Return False
            End If

            'ini�t�@�C���̓Ǎ�
            If IniRead() = False Then
                Return False
            End If

            '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
            '����ŊǗ��N���X�C���X�^���X����
            Me.TAX = New CASTCommon.ClsTAX
            '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

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

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KAWASE", "RIENTANAME")       '���G���^�t�@�C����
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���� ����:KAWASE ����:RIENTANAME")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���� ����:KAWASE ����:RIENTANAME"
            Return False
        End If

        ini_info.SEIKYU = CASTCommon.GetFSKJIni("KAWASE", "SEIKYU")             '�ב֐����ݒ�
        If ini_info.SEIKYU = "err" OrElse ini_info.SEIKYU = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�ב֐����ݒ� ����:KAWASE ����:SEIKYU")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�ב֐����ݒ� ����:KAWASE ����:SEIKYU"
            Return False
        End If

        '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
        ini_info.ZEIKIJUN = CASTCommon.GetFSKJIni("KAWASE", "ZEIKIJUN")
        If ini_info.ZEIKIJUN = "err" OrElse ini_info.ZEIKIJUN = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�Ŋ ����:KAWASE ����:ZEIKIJUN")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�Ŋ ����:KAWASE ����:ZEIKIJUN"
            Return False
        End If
        '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

        ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
        ini_info.RSV2_S_KAWASE_HONSITEN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_HONSITEN")
        If ini_info.RSV2_S_KAWASE_HONSITEN = "err" OrElse ini_info.RSV2_S_KAWASE_HONSITEN = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�ב֐U�����ו\(�{�x�X�ב�)����v�� ����:RSV2_V1.0.0 ����:S_KAWASE_HONSITEN")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�ב֐U�����ו\(�{�x�X�ב�)����v�� ����:RSV2_V1.0.0 ����:S_KAWASE_HONSITEN"
            Return False
        End If

        ini_info.RSV2_S_KAWASE_TAKOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_TAKOU")
        If ini_info.RSV2_S_KAWASE_TAKOU = "err" OrElse ini_info.RSV2_S_KAWASE_TAKOU = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�ב֐U�����ו\(���s�ב�)����v�� ����:RSV2_V1.0.0 ����:S_KAWASE_TAKOU")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�ב֐U�����ו\(���s�ב�)����v�� ����:RSV2_V1.0.0 ����:S_KAWASE_TAKOU"
            Return False
        End If

        ini_info.RSV2_S_KAWASE_LOGGING = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_LOGGING")
        If ini_info.RSV2_S_KAWASE_LOGGING = "err" OrElse ini_info.RSV2_S_KAWASE_LOGGING = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�ב֐U�����ו\(���M���O)����v�� ����:RSV2_V1.0.0 ����:S_KAWASE_LOGGING")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�ב֐U�����ו\(���M���O)����v�� ����:RSV2_V1.0.0 ����:S_KAWASE_LOGGING"
            Return False
        End If
        ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END

        Return True

    End Function

    ' �@�\�@ �F �U�����M���G���^�쐬���� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Public Function Main(ByVal command As String) As Integer

        '*** Str Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If
        '*** End Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***

        MainDB = New CASTCommon.MyOracle
        FmtComm.Oracle = MainDB

        Dim bRet As Boolean = True
        Dim iRet As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "�U�����M�f�[�^�쐬���o�b�`(�J�n)", "����")
            'MainLOG.Write("(�又��)�J�n", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***


            MainDB.BeginTrans()     ' �g�����U�N�V�����J�n

            '*** Str Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***
            ' �W���u���s���b�N
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write("(�又��)", "���s", "�U�����M���G���^�쐬�����Ŏ��s�҂��^�C���A�E�g")
                MainLOG.UpdateJOBMASTbyErr("�U�����M���G���^�쐬�����Ŏ��s�҂��^�C���A�E�g")
                Return -1
            End If
            '*** End Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***

            '*******************************
            ' �񎟂��擾
            '*******************************
            If GetKaiji() = False Then
                MainLOG.Write("(�又��)", "���s", "�񎟂̎擾�Ɏ��s���܂���")
                Return -1
            End If

            '*****************************************
            ' �U�����M�f�[�^�̊i�[�ƃX�P�W���[���̍X�V
            '*****************************************
            iRet = MakeFurikomiData()
            Select Case iRet
                Case 0          ' �f�[�^�i�[����
                    bRet = True

                Case 1          ' �Ώۃf�[�^�O��
                    bRet = True
                Case Else       ' �f�[�^�i�[���s
                    bRet = False
            End Select

            '*******************************
            ' ���[�o��
            '*******************************
            ' �U�����M�f�[�^���P���ȏ㑶�݂���ꍇ�A���[�o��
            If iRet = 0 Then
                Dim ExeRepo As New CAstReports.ClsExecute
                bRet = True

                '�ב֐U�����ו\(�{�x�X�ב�)
                If bRet = True AndAlso JIKOU_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP010", "�ב֐U�����ו\(�{�x�X�ב�)")
                End If

                '�ב֐U�����ו\(���s�ב�)
                If bRet = True AndAlso TAKOU_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP011", "�ב֐U�����ו\(���s�ב�)")
                End If

                '�ב֐U�����ו\(���U���M���O�o�^)
                If bRet = True AndAlso JIFURI_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP012", "�ב֐U�����ו\(���U���M���O�o�^)")
                End If

            End If

            If bRet = False Then
                If jobMessage = "" Then
                    Call MainLOG.UpdateJOBMASTbyErr("���O�Q��")
                Else
                    Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                End If

                '*** Str Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)
                '*** End Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***

                ' ���[���o�b�N
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "�Ώۃf�[�^�O��"
                End If

                Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                '*** Str Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)
                '*** End Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***

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
            '*** Str Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***
            If Not MainDB Is Nothing Then
                ' �W���u���s�A�����b�N
                dblock.Job_UnLock(MainDB)

                ' ���[���o�b�N
                MainDB.Rollback()
            End If
            '*** End Add 2016/01/11 �^�X�N�j�֓� for ���d���s�Ή� ***

            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "�U�����M�f�[�^�쐬���o�b�`(�I��)", "����")
            'MainLOG.Write("(�又��)�I��", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***
        End Try

        Return 0

    End Function

    ' �@�\�@ �F �U�����M�f�[�^�쐬����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function MakeFurikomiData() As Integer

        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = New StringBuilder(256)

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim tbl As New TesuuTable

        Try
            MainLOG.Write("(�U�����M�f�[�^�i�[)�J�n", "����")

            '--------------------------------------------------
            '�U���萔���}�X�^����Ǎ�
            '--------------------------------------------------
            With SQL
                .Append("select * from TESUUMAST, TAXMAST")
                .Append(" where TAX_ID_C = TAX_ID_Z")
                .Append(" and FSYORI_KBN_C = '3'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With


            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    tbl.TESUU_A1 = Integer.Parse(OraReader.GetString("TESUU_A1_C"))
                    tbl.TESUU_A2 = Integer.Parse(OraReader.GetString("TESUU_A2_C"))
                    tbl.TESUU_A3 = Integer.Parse(OraReader.GetString("TESUU_A3_C"))
                    tbl.TESUU_B1 = Integer.Parse(OraReader.GetString("TESUU_B1_C"))
                    tbl.TESUU_B2 = Integer.Parse(OraReader.GetString("TESUU_B2_C"))
                    tbl.TESUU_B3 = Integer.Parse(OraReader.GetString("TESUU_B3_C"))
                    tbl.TESUU_C1 = Integer.Parse(OraReader.GetString("TESUU_C1_C"))
                    tbl.TESUU_C2 = Integer.Parse(OraReader.GetString("TESUU_C2_C"))
                    tbl.TESUU_C3 = Integer.Parse(OraReader.GetString("TESUU_C3_C"))

                    '�ŗ�ID�A�U���萔���ID�A��ʂ��L�[�Ƀn�b�V���e�[�u���Ɋi�[����
                    TesuuList.Add(OraReader.GetString("TAX_ID_C") & OraReader.GetString("TESUU_TABLE_ID_C") & OraReader.GetString("SYUBETU_C"), tbl)

                    OraReader.NextRead()
                End While
            Else
                MainLOG.Write("�U���萔���}�X�^�擾", "���s", "�萔���e�[�u���擾���s")
                jobMessage = "�U���萔���}�X�^�擾 ���s "
                Return -1
            End If

        Catch ex As Exception
            MainLOG.Write("�U���萔���}�X�^�擾", "���s", ex.Message)
            jobMessage = "�U���萔���}�X�^�擾 ���s ��O���������܂����B"
            Return -1
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Try

            OraSchReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Length = 0
            SQL.AppendLine("SELECT")
            SQL.AppendLine(" TORIS_CODE_S")
            SQL.AppendLine(",TORIF_CODE_S")
            SQL.AppendLine(",BAITAI_CODE_S")
            SQL.AppendLine(",FURI_DATE_S")
            SQL.AppendLine(",MOTIKOMI_SEQ_S")
            SQL.AppendLine(",SYUBETU_S")
            SQL.AppendLine(",FURI_CODE_S")
            SQL.AppendLine(",KIGYO_CODE_S")
            SQL.AppendLine(",ITAKU_CODE_S")
            SQL.AppendLine(",SOUSIN_KBN_S")

            SQL.AppendLine(",ITAKU_KNAME_T")
            SQL.AppendLine(",ITAKU_NNAME_T")
            SQL.AppendLine(",SYUMOKU_CODE_T")
            SQL.AppendLine(",FUKA_CODE_T")
            SQL.AppendLine(",TORIMATOME_SIT_T")
            SQL.AppendLine(",BIKOU1_T")
            SQL.AppendLine(",BIKOU2_T")
            '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
            SQL.AppendLine(",TSIT_NO_T")
            '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

            SQL.AppendLine(",TOUROKU_DATE_S")
            SQL.AppendLine(",TUKEKIN_NO_T")
            SQL.AppendLine(",TUKESIT_NO_T")
            SQL.AppendLine(",TUKEKAMOKU_T")
            SQL.AppendLine(",TUKEKOUZA_T")
            SQL.AppendLine(",KESSAI_PATN_T")
            SQL.AppendLine(",TESUUTYO_KBN_T")
            SQL.AppendLine(",TESUU_TABLE_ID_T")

            SQL.AppendLine(",KIN_KNAME_N")
            SQL.AppendLine(",SIT_KNAME_N")

            SQL.AppendLine(" FROM S_TORIMAST")
            SQL.AppendLine("     ,S_SCHMAST")
            SQL.AppendLine("     ,TENMAST")

            SQL.AppendLine(" WHERE TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine("   AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.AppendLine(" AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
            SQL.AppendLine(" AND TORIMATOME_SIT_T = SIT_NO_N(+)")
            SQL.AppendLine("   AND FURI_DATE_S >= " & SQ(ParaHassinDate))

            ' �����U��
            SQL.AppendLine("   AND ((SYUBETU_T = '21' AND FURI_DATE_S BETWEEN " & SQ(ParaHassinDate) & " AND " & SQ(ParaSouDate) & ")")
            ' ���^�U���C�ܗ^�U��
            SQL.AppendLine("    OR (SYUBETU_T <> '21' AND FURI_DATE_S BETWEEN " & SQ(ParaHassinDate) & " AND " & SQ(ParaKyuDate) & "))")

            SQL.AppendLine("   AND TOUROKU_FLG_S = '1'")
            SQL.AppendLine("   AND HASSIN_FLG_S = '2'")
            SQL.AppendLine("   AND TYUUDAN_FLG_S = '0'")

            SQL.AppendLine(" ORDER BY FURI_DATE_S, SOUSIN_KBN_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")


            Dim Key As FKeyInfo = Nothing
            Dim test As String = SQL.ToString

            If OraSchReader.DataReader(SQL) = True Then
                ' �L�[������
                Key.Init()

                ' �ŏ��̃L�[�ݒ�
                Call Key.SetOraDataKessai(OraSchReader)

                Do While OraSchReader.EOF = False
                    MainLOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                    MainLOG.FuriDate = Key.FURI_DATE

                    '2013/11/14 saitou �W���� ����őΉ� ADD -------------------------------------------------->>>>
                    '�������ŗ����擾����
                    Dim strKijunDate As String = String.Empty
                    If ini_info.ZEIKIJUN.Equals("0") = True Then
                        '�U�����
                        strKijunDate = Key.FURI_DATE
                    Else
                        '���M���
                        strKijunDate = ParaHassinDate
                    End If

                    Me.TAX.GetZeiritsu(strKijunDate)
                    If Me.TAX.ZEIRITSU.Equals("err") = True Then
                        MainLOG.Write("�ŗ��擾", "���s", "����F" & strKijunDate)
                        Return -1
                    End If

                    '2013/12/27 saitou �W���� �󎆐őΉ� ADD -------------------------------------------------->>>>
                    Me.TAX.GetInshizei(strKijunDate)
                    If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                        MainLOG.Write("�󎆐Ŏ擾", "���s", "����F" & strKijunDate)
                        Return -1
                    End If
                    '2013/12/27 saitou �W���� �󎆐őΉ� ADD --------------------------------------------------<<<<

                    '�U���萔���̍Čv�Z���s��
                    If Me.CalcFurikomiTesuu(Key) = False Then
                        Return -1
                    End If
                    '2013/11/14 saitou �W���� ����őΉ� ADD --------------------------------------------------<<<<

                    ' ���׃}�X�^����C���M�}�X�^���쐬����
                    If GetFurikomiData(Key) = False Then
                        Return -1
                    End If

                    ' �X�P�W���[���}�X�^�̍X�V���� 
                    If UpdateSchMast(Key) = False Then
                        Return -1
                    End If

                    JIKOU_KEN += Key.JikouKen
                    TAKOU_KEN += Key.TakouKen
                    JIFURI_KEN += Key.JifuriKen

                    ' �Ώۃf�[�^�̎����R�[�h��Ǎ���
                    OraSchReader.NextRead()

                    If OraSchReader.EOF = False Then
                        ' �L�[������
                        Key.Init()

                        ' �L�[�̍Đݒ�
                        Call Key.SetOraDataKessai(OraSchReader)

                    End If
                Loop

            End If

            If haskey.RecordNo = 0 Then
                MainLOG.Write("(�U�����M�f�[�^�i�[)", "���s", "�����O��")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(�U�����M�f�[�^�i�[)", "���s", ex.Message)
            Return -1
        Finally
            If Not OraSchReader Is Nothing Then OraSchReader.Close()
            MainLOG.Write("(�U�����M�f�[�^�i�[)�I��", "����")
        End Try

        Return 0

    End Function

    ' �@�\�@ �F �ב֐U���f�[�^�쐬����
    '
    ' �����@ �F ARG1 - �L�[���
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function GetFurikomiData(ByRef Key As FKeyInfo) As Boolean
        Dim SQL As StringBuilder = New StringBuilder(256)
        '*** Str Upd 2016/01/11 �^�X�N�j�֓� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        'Dim OraMeiReader As CASTCommon.MyOracleReader       ' ���׃}�X�^
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing       ' ���׃}�X�^
        '*** End Upd 2016/01/11 �^�X�N�j�֓� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

        ' �U����
        Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

        ' �U�����̂P�c�Ɠ��O�C�Q�c�Ɠ��O
        Dim Zen1Day As String = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

        '*** Str Add 2016/01/11 �^�X�N�j�֓� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Try
            '*** End Add 2016/01/11 �^�X�N�j�֓� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

            SQL.AppendLine("SELECT")
            SQL.AppendLine("      FURIKIN_K")
            SQL.AppendLine("    , TESUU_KIN_K")
            SQL.AppendLine("    , KEIYAKU_KIN_K")
            SQL.AppendLine("    , KEIYAKU_SIT_K")
            SQL.AppendLine("    , SUBSTRB(FURI_DATA_K, 43, 1)   KEIYAKU_KAMOKU_K")  ' �Ȗڂ�FURI_DATA_K�̒l��ݒ肷��
            SQL.AppendLine("    , KEIYAKU_KOUZA_K")
            SQL.AppendLine("    , KEIYAKU_KNAME_K")
            SQL.AppendLine("    , SINKI_CODE_K")
            SQL.AppendLine("    , TEKIYO_KBN_K")
            SQL.AppendLine("    , KTEKIYO_K")
            SQL.AppendLine("    , NTEKIYO_K")
            SQL.AppendLine("    , JYUYOUKA_NO_K")
            SQL.AppendLine("    , SUBSTRB(FURI_DATA_K, 113, 1)  SIKIBETU")          ' EDI���p ���ʕ\��
            SQL.AppendLine("    , KIN_KNAME_N")
            SQL.AppendLine("    , SIT_KNAME_N")
            SQL.AppendLine(" FROM ")
            SQL.AppendLine("      S_MEIMAST")
            SQL.AppendLine("    , TENMAST")
            SQL.AppendLine(" WHERE ")
            SQL.AppendLine("      TORIS_CODE_K     = " & SQ(Key.TORIS_CODE))
            SQL.AppendLine("  AND TORIF_CODE_K     = " & SQ(Key.TORIF_CODE))
            SQL.AppendLine("  AND FURI_DATE_K      = " & SQ(Key.FURI_DATE))
            SQL.AppendLine("  AND MOTIKOMI_SEQ_K   = " & Key.MOTIKOMI_SEQ)
            SQL.AppendLine("  AND DATA_KBN_K       = '2'")
            SQL.AppendLine("  AND FURIKETU_CODE_K  = 0")
            SQL.AppendLine("  AND FURIKIN_K       >= 0")
            SQL.AppendLine("  AND KEIYAKU_KIN_K    = KIN_NO_N(+)")
            SQL.AppendLine("  AND KEIYAKU_SIT_K    = SIT_NO_N(+)")
            SQL.AppendLine(" ORDER BY ")
            SQL.AppendLine("       RECORD_NO_K")

            OraMeiReader = New CASTCommon.MyOracleReader(MainDB)

            If OraMeiReader.DataReader(SQL) = True Then

                Dim KData As New ClsFormKes.KessaiData

                Do While OraMeiReader.EOF = False
                    haskey.RecordNo += 1
                    Key.TotalKen += 1
                    Key.TotalKin += OraMeiReader.GetInt64("FURIKIN_K")
                    '2018/03/15 saitou �L���M��(RSV2�W��) ADD �X�P�W���[���}�X�^�̎萔���X�V -------------------- START
                    Key.TesuuKin += OraMeiReader.GetInt64("TESUU_KIN_K")
                    '2018/03/15 saitou �L���M��(RSV2�W��) ADD --------------------------------------------------- END

                    '�K�p��ʐݒ�
                    Select Case ParaHassinDate
                        Case Key.FURI_DATE
                            ' ����
                            Key.TEKIYOU_SYUBETU = "�غ�"
                        Case Zen1Day
                            ' �P�c�Ɠ��O
                            Key.TEKIYOU_SYUBETU = "����"
                        Case Is <= Zen2Day
                            ' �Q�c�Ɠ��O�ȑO
                            Select Case Key.SYUBETU
                                Case "21"
                                    ' ���U
                                    Key.TEKIYOU_SYUBETU = "����"
                                Case "11"
                                    ' ���^
                                    Key.TEKIYOU_SYUBETU = "�ճ�"
                                Case "12"
                                    ' �ܗ^
                                    Key.TEKIYOU_SYUBETU = "�ֳ�"
                            End Select
                    End Select

                    Select Case Key.SOUSIN_KBN
                        Case "0"
                            '�ב֐U��
                            If OraMeiReader.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
                                Key.JikouKen += 1
                                Key.JikouKin += OraMeiReader.GetInt64("FURIKIN_K")
                            Else
                                Key.TakouKen += 1
                                Key.TakouKin += OraMeiReader.GetInt64("FURIKIN_K")
                            End If

                            KData = fn_KAWASE_FURIKOMI(Key, OraMeiReader, Zen1Day, Zen2Day)
                        Case "1"
                            '���M���O
                            Key.JifuriKen += 1
                            Key.JifuriKin += OraMeiReader.GetInt64("FURIKIN_K")

                        Case "2"
                            'CSV���G���^(�M�g�p)
                        Case Else
                            '���M�敪�ُ�
                            jobMessage = "���M�敪�ُ�F" & Key.SOUSIN_KBN
                            Return False
                    End Select

                    '�����U���טA�g�f�[�^�e�[�u���ɑ}��
                    If Me.InsertSMRenkeimast(Key, KData, OraMeiReader) = False Then
                        Return False
                    End If

                    ' �U�����M�f�[�^�̃��X�g�쐬
                    HassinList.Add(KData)

                    OraMeiReader.NextRead()
                Loop

                '�U�����z�̍��v���v�Z
                If Me.CalcFurikinKei(Key) = False Then
                    Return False
                End If

                '�����U�˗��l�A�g�f�[�^�e�[�u���ɑ}��
                If Me.InsertSIRenkeimast(Key, KData) = False Then
                    Return False
                End If

            End If
            OraMeiReader.Close()

            Return True

            '*** Str Add 2016/01/11 �^�X�N�j�֓� for MyOracleReader�N���[�Y�Y����C���i���݁j ***
        Finally
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
        End Try
        '*** End Add 2016/01/11 �^�X�N�j�֓� for MyOracleReader�N���[�Y�Y����C���i���݁j ***

    End Function

    ' �@�\�@ �F �ב֐U���f�[�^�쐬����
    '
    ' �����@ �F ARG1 - �L�[���
    '           ARG1 - �I���N�����[�_
    '
    ' �߂�l �F �P���R�[�h�f�[�^
    '
    ' ���l�@ �F 
    '
    Private Function fn_KAWASE_FURIKOMI(ByVal Key As FKeyInfo, ByVal oraMei As CASTCommon.MyOracleReader, ByVal zen1Day As String, ByVal zen2Day As String) As CAstFormKes.ClsFormKes.KessaiData
        Dim Data As New CAstFormKes.ClsFormSikinFuri.T_48100

        Data.KAMOKU_CODE = "48"
        Data.OPE_CODE = "100"
        Data.TORIATUKAI = Key.FURI_DATE

        If Key.SYUBETU = "11" AndAlso Key.TEKIYOU_SYUBETU <> "�ճ�" Then
            '�_���ʂ����^�Ŏ�ʃR�[�h�����^�ɂȂ�Ȃ��ꍇ
            Data.BIKOU2 = "�ճ�"
        ElseIf Key.SYUBETU = "12" AndAlso Key.TEKIYOU_SYUBETU <> "�ֳ�" Then
            '�_���ʂ��ܗ^�Ŏ�ʃR�[�h���ܗ^�ɂȂ�Ȃ��ꍇ
            Data.BIKOU2 = "�ֳ�"
        Else
            Data.BIKOU2 = ""
        End If

        '��ڃR�[�h�A���l�Q�ݒ�
        Select Case Key.TEKIYOU_SYUBETU
            Case "�غ�"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '�U������
                        Data.SYUMOKU = "1074"
                        Data.BIKOU1 = "ߺ���� "

                    Case "01"
                        '�U�����ɋ�
                        Data.SYUMOKU = "1054"
                        Data.BIKOU1 = "ߺ���� "

                    Case Else
                        '�U�����
                        Data.SYUMOKU = "1022"
                        Data.BIKOU1 = ""

                End Select

            Case "����"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '��U����
                        Data.SYUMOKU = "1174"
                        Data.BIKOU1 = "ߺ���� "

                    Case "01"
                        '��U���ɋ�
                        Data.SYUMOKU = "1154"
                        Data.BIKOU1 = "ߺ���� "

                    Case Else
                        '��U���
                        Data.SYUMOKU = "1122"
                        Data.BIKOU1 = ""

                End Select

            Case "�ճ�"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '���^����
                        Data.SYUMOKU = "1271"
                        Data.BIKOU1 = "ߺ���� "

                    Case "01"
                        '���^���ɋ�
                        Data.SYUMOKU = "1251"
                        Data.BIKOU1 = "ߺ���� "

                    Case Else
                        '���^���
                        Data.SYUMOKU = "1211"
                        Data.BIKOU1 = ""

                End Select

            Case "�ֳ�"
                Select Case Key.SYUMOKU_CODE
                    Case "02"
                        '�ܗ^����
                        Data.SYUMOKU = "1272"
                        Data.BIKOU1 = "ߺ���� "

                    Case "01"
                        ''�ܗ^���ɋ�
                        Data.SYUMOKU = "1252"
                        Data.BIKOU1 = "ߺ���� "

                    Case Else
                        '�ܗ^���
                        Data.SYUMOKU = "1212"
                        Data.BIKOU1 = ""

                End Select
        End Select

        '20131111 ���o�b�`�A�g���͕s�v
        'Data.BIKOU1 = Data.BIKOU1 & Key.TUKESIT_KNAME & " �¶�"
        '20131111 ���o�b�`�A�g���͕s�v

        If oraMei.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
            Data.JUSIN_TEN = "� " & oraMei.GetString("SIT_KNAME_N")
            Data.HASSIN_TEN = "� ���-"
        Else
            Data.JUSIN_TEN = oraMei.GetString("KIN_KNAME_N") & " " & oraMei.GetString("SIT_KNAME_N")
            Data.HASSIN_TEN = ini_info.KAWASE_CENTER
        End If

        Data.FUKA_CODE = Key.FUKA_CODE '000�Œ�
        Data.KINGAKU = oraMei.GetInt64("FURIKIN_K").ToString.PadLeft(10)
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
        'Data.KESSAI_CNT = " "
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
        Call fn_FUGO_SETTEI(oraMei.GetInt64("FURIKIN_K").ToString("#,##0"), Data.KINGAKU_FUGOU)
        Data.KINGAKU_FUGOU = Data.KINGAKU_FUGOU.Trim.PadRight(15, " "c)
        Data.KOKYAKU_TESUU = ""
        Data.UKETORI_KAMOKU = oraMei.GetItem("KEIYAKU_KAMOKU_K")
        Data.UKETORI_KOUZA = oraMei.GetString("KEIYAKU_KOUZA_K").Trim.PadLeft(7, "0"c).PadRight(15)
        Data.UKETORI_NAME = oraMei.GetString("KEIYAKU_KNAME_K").PadRight(29).Substring(0, 29).Trim
        Data.IRAI_NAME = Key.ITAKU_KNAME.PadRight(48).Substring(0, 48).Trim

        If Key.SYUBETU = "21" AndAlso oraMei.GetString("SIKIBETU") = "Y" Then
            '���U�Ŏ��ʕ\����Y�̏ꍇ��EDI���ݒ�
            Data.EDI_INFO = oraMei.GetString("JYUYOUKA_NO_K")
        Else
            Data.EDI_INFO = ""
        End If

        '���l���ݒ肳��Ă���ꍇ
        If Key.BIKOU1 <> "" Then
            Data.BIKOU1 = Key.BIKOU1
        End If
        If Key.BIKOU2 <> "" Then
            Data.BIKOU2 = Key.BIKOU2
        End If

        Data.YOBI1 = ""

        Dim Kdata As New CAstFormKes.ClsFormKes.KessaiData
        Kdata.record320 = Data.Data
        Kdata.OpeCode = String.Concat(Data.KAMOKU_CODE, Data.OPE_CODE)
        Kdata.TorisCode = Key.TORIS_CODE
        Kdata.TorifCode = Key.TORIF_CODE
        Kdata.ToriNName = Key.ITAKU_NNAME
        Kdata.TorimatomeSit = Key.TORIMATOME_SIT
        Kdata.KesKinCode = oraMei.GetString("KEIYAKU_KIN_K")
        Kdata.KesSitCode = oraMei.GetString("KEIYAKU_SIT_K")
        Kdata.FurikomiTesuukin = oraMei.GetString("TESUU_KIN_K")

        Return Kdata
    End Function

#Region "���ϗp"
    ' �@�\�@ �F �������σf�[�^�쐬����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '

    Private Function MakeKessaiData() As Integer

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = New StringBuilder(256)

        Try
            MainLOG.Write("(�������σf�[�^�i�[)�J�n", "����")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.AppendLine("SELECT")
            SQL.AppendLine(" MAX(TORIS_CODE_S) TORIS_CODE_S")
            SQL.AppendLine(",MAX(TORIF_CODE_S) TORIF_CODE_S")
            SQL.AppendLine(",MAX(FURI_DATE_S) FURI_DATE_S")
            SQL.AppendLine(",MAX(SYUBETU_S) SYUBETU_S")
            SQL.AppendLine(",MAX(ITAKU_CODE_S) ITAKU_CODE_S")
            SQL.AppendLine(",SUM(FURI_KEN_S) FURI_KEN_S")
            SQL.AppendLine(",SUM(FURI_KIN_S) FURI_KIN_S")

            SQL.AppendLine(",MAX(ITAKU_KNAME_T) ITAKU_KNAME_T")
            SQL.AppendLine(",MAX(TORIMATOME_SIT_T) TORIMATOME_SIT_T")
            SQL.AppendLine(",MAX(KESSAI_KBN_T) KESSAI_KBN_T")
            SQL.AppendLine(",MAX(KESSAI_PATN_T) KESSAI_PATN_T")

            SQL.AppendLine(",MAX(KIN_KNAME_N) KIN_KNAME_N")
            SQL.AppendLine(",MAX(SIT_KNAME_N) SIT_KNAME_N")
            SQL.AppendLine(",MAX(BAITAI_CODE_S) BAITAI_CODE_S")
            SQL.AppendLine(" FROM S_TORIMAST")
            SQL.AppendLine("     ,S_SCHMAST")
            SQL.AppendLine("     ,TENMAST")

            SQL.AppendLine(" WHERE KESSAI_YDATE_S = " & SQ(ParaHassinDate))
            SQL.AppendLine(" AND KESSAI_FLG_S = '0'")
            SQL.AppendLine(" AND HASSIN_FLG_S = '1'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            SQL.AppendLine(" AND FURI_KIN_S > 0")
            SQL.AppendLine(" AND KESSAI_KBN_T <> '99'")
            SQL.AppendLine(" AND TORIS_CODE_S   = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S   = TORIF_CODE_T")
            SQL.AppendLine(" AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
            SQL.AppendLine(" AND TORIMATOME_SIT_T = SIT_NO_N(+)")
            SQL.AppendLine(" GROUP BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")
            SQL.AppendLine(" ORDER BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")

            Dim Key As KKeyInfo = Nothing
            Dim test As String = SQL.ToString

            If OraReader.DataReader(SQL) = True Then

                Dim lstKessaiData As New List(Of CAstFormKes.ClsFormKes.KessaiData)

                ' �L�[������
                Key.Init()

                ' �{���X���擾
                HONBU_KNAME = GetTenmast()

                ' �ŏ��̃L�[�ݒ�
                Call Key.SetOraDataKessai(OraReader)

                keskey.RecordNo = haskey.RecordNo

                Do While OraReader.EOF = False
                    lstKessaiData.Clear()

                    ' �������σf�[�^�擾����(�����U���p)
                    If fn_GetKessaiData(Key, lstKessaiData) = False Then
                        Return -1
                    End If

                    If Not (lstKessaiData Is Nothing OrElse lstKessaiData.Count = 0) Then
                        ' �擾�����������σf�[�^����ɁA���U���σ}�X�^�o�^���s��
                        For i As Integer = 0 To lstKessaiData.Count - 1
                            Dim KData As CAstFormKes.ClsFormKes.KessaiData = lstKessaiData.Item(i)

                            ' ���U���σ}�X�^�̓o�^����
                            If InsertKessaiMast(Key, KData) = False Then
                                jobMessage = "���U���σ}�X�^�o�^���s ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & _
                                             " �U�����F" & Key.FURI_DATE
                                Return -1
                            End If
                        Next
                    End If

                    ' �X�P�W���[���}�X�^�̍X�V���� 
                    If UpdateSchMast(Key) = False Then
                        jobMessage = "�X�P�W���[���}�X�^�X�V���s ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & _
                                     " �U�����F" & Key.FURI_DATE
                        Return -1
                    End If

                    ' �Ώۃf�[�^�̎����R�[�h��Ǎ���
                    OraReader.NextRead()

                    If OraReader.EOF = False Then
                        ' �L�[�ݒ�
                        Call Key.SetOraDataKessai(OraReader)
                    End If

                    HassinList.AddRange(lstKessaiData)
                Loop
            End If

            If HassinList.Count = 0 Then
                MainLOG.Write("(�������σf�[�^�i�[)", "���s", "�����O��")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(�������σf�[�^�i�[)", "���s", ex.Message)
            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            MainLOG.Write("(�������σf�[�^�i�[)�I��", "����")
        End Try

        Return 0

    End Function


    ' �@�\�@ �F �������σf�[�^�擾����(�����U���p)
    '
    ' �����@ �F 
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_GetKessaiData(ByRef Key As KKeyInfo, ByRef lstKessaiData As List(Of CAstFormKes.ClsFormKes.KessaiData)) As Boolean

        Dim errFlg As Boolean = False
        Dim errMsg As String = "���Ϗ��Ɍ�肪����܂��B"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing

        strKEKKA = ""

        Try
            Select Case Key.KESSAI_KBN
                Case "00"
                    ' �ב֐����̃f�[�^�쐬
                    If errFlg = False AndAlso fn_KAWASE_SEIKYU(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                        ' �������σf�[�^�̃��X�g�쐬
                        lstKessaiData.Add(KData)
                    Else
                        errFlg = True
                    End If
                Case "99"

                Case Else
                    errFlg = True
                    errMsg &= "(���ϋ敪)"
            End Select

            If errFlg = True Then
                jobMessage = errMsg & " ������R�[�h�F" & Key.TORIS_CODE & " ����敛�R�[�h�F" & Key.TORIF_CODE & " �U�����F" & Key.FURI_DATE & _
                    " ���ϋ敪�F" & Key.KESSAI_KBN
                MainLOG.Write("�������σf�[�^�擾����(�����U���p)", "���s", jobMessage)
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("�������σf�[�^�擾����(�����U���p)", "���s", ex.Message)
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
    Private Function fn_KessaiData(ByRef Key As KKeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean

        Try
            ' �ʔԂ̃J�E���g�A�b�v
            keskey.RecordNo += 1

        Catch ex As Exception
            MainLOG.Write("�������σf�[�^�쐬", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ' �@�\�@ �F �ב֐����f�[�^�쐬����
    '
    ' �����@ �F
    '
    ' �߂�l �F 0 - ����C-1 - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function fn_KAWASE_SEIKYU(ByRef key As KKeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseSeikyuInFmt As New CAstFormKes.ClsFormSikinFuri.T_48600
        Dim strKINFUKKI_FUGOU As String = ""    ' ���z���L����

        Try
            ' ������
            KawaseSeikyuInFmt.Init()

            ' ���z���L�����̎擾
            If fn_FUGO_SETTEI(CASTCommon.CADec(key.FURI_KIN).ToString("#,##0"), strKINFUKKI_FUGOU) = False Then
                MainLOG.Write("�ב֐����f�[�^�쐬", "���s", "���L�����ݒ菈���G���[�B������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�����F" & key.FURI_DATE)
                Return -1
            End If

            '�f�[�^�ݒ�
            With KawaseSeikyuInFmt
                .TORIATUKAI = ParaHassinDate
                .SYUMOKU = "4701"                                   ' ��ڃR�[�h
                .JUSIN_TEN = "� " & key.SIT_KNAME                   ' ��M�X��
                .FUKA_CODE = "000"                                  ' �t���R�[�h
                .HASSIN_TEN = "� " & HONBU_KNAME                    ' ���M�X��
                .KINGAKU = key.FURI_KIN.ToString.PadLeft(10)        ' ���z
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = " "                                   ' ���ω�
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.PadRight(15, " "c) ' ���z���L����
                .BANGOU = ""                                        ' �ԍ�
                .SIKIN_JIYUU1 = "�ײ��" & key.ITAKU_KNAME.Trim & "���"  ' �����t�֗��R
                .SIKIN_JIYUU2 = key.FURI_KEN.Trim & "��"            ' �����t�֗��R�Q
                .BIKOU1 = ""                                        ' ���l�P
                .BIKOU2 = ""                                        ' ���l�Q
                .SYOKAI_NO = ""                                     ' �Ɖ�ԍ�
                .YOBI1 = ""                                         ' �\���P
            End With

            ' �������σf�[�^�ɃI�y�R�[�h���̌ʃf�[�^��ݒ�
            KData.record320 = KawaseSeikyuInFmt.Data
            KData.OpeCode = String.Concat(KawaseSeikyuInFmt.KAMOKU_CODE, KawaseSeikyuInFmt.OPE_CODE)
            KData.TorimatomeSit = key.TORIMATOME_SIT

        Catch ex As Exception
            MainLOG.Write("�ב֐����f�[�^�쐬", "���s", ex.Message)
            Return -1
        End Try

        Return 0

    End Function

    ' �@�\�@ �F ���U���σ}�X�^�o�^
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertKessaiMast(ByVal Key As KKeyInfo, ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("INSERT INTO S_KESSAIMAST(")
            SQL.AppendLine(" SYORI_DATE_KR")
            SQL.AppendLine(",TIME_STAMP_KR")
            SQL.AppendLine(",KAIJI_KR")
            SQL.AppendLine(",RECORD_NO_KR")
            SQL.AppendLine(",FILE_NAME_KR")
            SQL.AppendLine(",TORIS_CODE_KR")
            SQL.AppendLine(",TORIF_CODE_KR")
            SQL.AppendLine(",FURI_DATE_KR")
            SQL.AppendLine(",MOTIKOMI_SEQ_KR")
            SQL.AppendLine(",KAMOKU_CODE_KR")
            SQL.AppendLine(",OPE_CODE_KR")
            SQL.AppendLine(",DENBUN_ALL_KR")
            SQL.AppendLine(",ERR_CODE_KR")
            SQL.AppendLine(",ERR_MSG_KR")
            SQL.AppendLine(",SAKUSEI_DATE_KR")
            SQL.AppendLine(",KOUSIN_DATE_KR")
            SQL.AppendLine(") VALUES (")
            SQL.AppendLine(" " & SQ(strDate))                                   ' ������
            SQL.AppendLine("," & SQ(String.Concat(strDate, strTime)))           ' �^�C���X�^���v
            SQL.AppendLine("," & SQ(keskey.Kaiji))                              ' ��
            SQL.AppendLine("," & SQ(keskey.RecordNo))                           ' �ʔ�
            SQL.AppendLine("," & SQ(ini_info.RIENTA_FILENAME))                  ' ���G���^�t�@�C����
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' ������R�[�h
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' ����敛�R�[�h
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' �U����
            SQL.AppendLine(",1")                                                ' ����SEQ
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(0, 2)))             ' �ȖڃR�[�h
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(2, 3)))             ' �I�y�R�[�h
            SQL.AppendLine("," & SQ(KData.record320))                           ' �ʃf�[�^
            SQL.AppendLine("," & SQ(""))                                        ' ���ʃR�[�h
            SQL.AppendLine("," & SQ(""))                                        ' �G���[���b�Z�[�W
            SQL.AppendLine("," & SQ(strDate))                                   ' �쐬��
            SQL.AppendLine("," & SQ("00000000"))                                ' �X�V��
            SQL.AppendLine(")")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(���U���σ}�X�^�o�^)", "���s", ex.Message)
            Return False
        Finally
        End Try

        Return True

    End Function

    ' �@�\�@ �F �X�P�W���[���}�X�^�X�V
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateSchMast(ByVal key As KKeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("UPDATE S_SCHMAST")
            SQL.AppendLine(" SET")
            SQL.AppendLine(" KESSAI_FLG_S = '1'")
            SQL.AppendLine(",KESSAI_DATE_S = " & SQ(ParaHassinDate))
            SQL.AppendLine(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))
            SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))
            SQL.AppendLine("   AND FURI_KIN_S > 0")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("�X�P�W���[���}�X�^�X�V", "���s", "������R�[�h�F" & key.TORIS_CODE & " ����敛�R�[�h�F" & key.TORIF_CODE & _
              " �U�����F" & key.FURI_DATE & " " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function GetTenmast() As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append("SELECT SIT_KNAME_N FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(ini_info.JIKINKO_CODE))
            SQL.Append("   AND SIT_NO_N = " & SQ(ini_info.HONBU_CODE))
            If OraReader.DataReader(SQL) = True Then
                Return OraReader.GetString("SIT_KNAME_N")
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

#End Region

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

    ' �@�\�@ �F �U�����M�}�X�^�o�^
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertHassinMast(ByVal Key As FKeyInfo, ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("INSERT INTO HASSINMAST(")
            SQL.AppendLine(" SYORI_DATE_FH")
            SQL.AppendLine(",TIME_STAMP_FH")
            SQL.AppendLine(",KAIJI_FH")
            SQL.AppendLine(",RECORD_NO_FH")
            SQL.AppendLine(",FILE_NAME_FH")
            SQL.AppendLine(",TORIS_CODE_FH")
            SQL.AppendLine(",TORIF_CODE_FH")
            SQL.AppendLine(",FURI_DATE_FH")
            SQL.AppendLine(",MOTIKOMI_SEQ_FH")
            SQL.AppendLine(",KAMOKU_CODE_FH")
            SQL.AppendLine(",OPE_CODE_FH")
            SQL.AppendLine(",DENBUN_ALL_FH")
            SQL.AppendLine(",ERR_CODE_FH")
            SQL.AppendLine(",ERR_MSG_FH")
            SQL.AppendLine(",SAKUSEI_DATE_FH")
            SQL.AppendLine(",KOUSIN_DATE_FH")
            SQL.AppendLine(") VALUES (")
            SQL.AppendLine(" " & SQ(strDate))                                   ' ������
            SQL.AppendLine("," & SQ(String.Concat(strDate, strTime)))           ' �^�C���X�^���v
            SQL.AppendLine("," & SQ(haskey.Kaiji))                              ' ��
            SQL.AppendLine("," & SQ(haskey.RecordNo))                           ' �ʔ�
            SQL.AppendLine("," & SQ(ini_info.RIENTA_FILENAME))                  ' ���G���^�t�@�C����
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' ������R�[�h
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' ����敛�R�[�h
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' �U����
            SQL.AppendLine("," & SQ(Key.MOTIKOMI_SEQ))                          ' ����SEQ
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(0, 2)))             ' �ȖڃR�[�h
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(2, 3)))             ' �I�y�R�[�h
            SQL.AppendLine("," & SQ(KData.record320))                           ' �ʃf�[�^
            SQL.AppendLine("," & SQ(""))                                        ' ���ʃR�[�h
            SQL.AppendLine("," & SQ(""))                                        ' �G���[���b�Z�[�W
            SQL.AppendLine("," & SQ(strDate))                                   ' �쐬��
            SQL.AppendLine("," & SQ("00000000"))                                ' �X�V��
            SQL.AppendLine(")")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(�U�����M�}�X�^�o�^)", "���s", ex.Message)
            Return False
        Finally
        End Try

        Return True

    End Function

    ' �@�\�@ �F �X�P�W���[���}�X�^�X�V
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function UpdateSchMast(ByVal key As FKeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine(" UPDATE S_SCHMAST SET")
            SQL.AppendLine(" HASSIN_DATE_S = " & SQ(strDate))
            SQL.AppendLine(",HASSIN_FLG_S = '1'")
            SQL.AppendLine(",HASSIN_TIME_STAMP_S = " & SQ(strDate & strTime))
            Select Case Me.GetKessaiPatn(key)
                Case "0", "1"           '�����m�ەs�v�i�ב֎������M�ΏۊO�j�A�����m�ەs�v�i�ב֎������M�Ώہj

                Case "2", "3"           '�萔���ʏo���A�萔�����Z
                    SQL.AppendLine(",KAKUHO_FLG_S = '1'")
                    SQL.AppendLine(",KAKUHO_DATE_S = " & SQ(ParaHassinDate))
                    SQL.AppendLine(",KAKUHO_TIME_STAMP_S = " & SQ(strDate & strTime))
                    SQL.AppendLine(",TESUUTYO_FLG_S = '1'")
                    SQL.AppendLine(",TESUU_DATE_S = " & SQ(ParaHassinDate))
                    SQL.AppendLine(",TESUU_TIME_STAMP_S = " & SQ(strDate & strTime))

                Case "4", "5"           '�萔������A�萔���ʎ�
                    SQL.AppendLine(",KAKUHO_FLG_S = '1'")
                    SQL.AppendLine(",KAKUHO_DATE_S = " & SQ(ParaHassinDate))
                    SQL.AppendLine(",KAKUHO_TIME_STAMP_S = " & SQ(strDate & strTime))

            End Select

            SQL.AppendLine(",FURI_KEN_S    = " & key.TotalKen)
            SQL.AppendLine(",FURI_KIN_S    = " & key.TotalKin)
            '2018/03/15 saitou �L���M��(RSV2�W��) ADD �X�P�W���[���}�X�^�̎萔���X�V -------------------- START
            SQL.AppendLine(",TESUU_KIN_S   = " & key.TesuuKin)
            '2018/03/15 saitou �L���M��(RSV2�W��) ADD --------------------------------------------------- END
            SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))
            SQL.AppendLine("   AND MOTIKOMI_SEQ_S = " & SQ(key.MOTIKOMI_SEQ))

            If MainDB.ExecuteNonQuery(SQL) = 0 Then
                MainLOG.Write("�X�P�W���[���}�X�^�X�V", "���s", "������R�[�h�F" & key.TORIS_CODE & _
                              " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�����F" & key.FURI_DATE & " �����񐔁F" & key.MOTIKOMI_SEQ)
            Else
                MainLOG.Write("�X�P�W���[���}�X�^�X�V", "����", "������R�[�h�F" & key.TORIS_CODE & _
                              " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�����F" & key.FURI_DATE & " �����񐔁F" & key.MOTIKOMI_SEQ)
            End If

        Catch ex As Exception
            MainLOG.Write("�X�P�W���[���}�X�^�X�V", "���s", "������R�[�h�F" & key.TORIS_CODE & _
                          " ����敛�R�[�h�F" & key.TORIF_CODE & " �U�����F" & key.FURI_DATE & " �����񐔁F" & key.MOTIKOMI_SEQ & " " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function GetKaiji() As Boolean

        Dim SQL As New StringBuilder()
        Dim OraReader As MyOracleReader = Nothing

        Try
            MainLOG.Write("(�񎟎擾)�J�n", "����", "")

            SQL.Append("SELECT NVL(MAX(KAIJI_SR),0) AS MAX_KAIJI FROM S_I_RENKEIMAST")
            SQL.Append(" WHERE SAKUSEI_DATE_SR = " & SQ(strDate))

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                haskey.Kaiji = OraReader.GetInt("MAX_KAIJI") + 1
                keskey.Kaiji = OraReader.GetInt("MAX_KAIJI") + 1
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

    '�U�����M���s�҂��̃t���O�����ׂČ��ɖ߂�
    Public Function ReturnFlg() As Integer
        Dim SQL As String
        Dim Ret As Integer = 0
        '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� ADD ---------------------------------------- START
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� ADD ---------------------------------------- END

        Try
            '�O�̂��߁A��[�N���[�Y
            'INC-No.ME-0276 Not�ǉ�
            If Not MainDB Is Nothing Then MainDB.Close()
            MainDB = New CASTCommon.MyOracle

            '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- START
            '�f�[�^�쐬�Ώۂ̃X�P�W���[�����o�����ɍ��킹��
            SQL = "SELECT "
            SQL &= " TORIS_CODE_S"
            SQL &= ",TORIF_CODE_S"
            SQL &= ",FURI_DATE_S"
            SQL &= ",MOTIKOMI_SEQ_S"
            SQL &= " FROM S_TORIMAST, S_SCHMAST"
            SQL &= " WHERE TORIS_CODE_T = TORIS_CODE_S"
            SQL &= " AND TORIF_CODE_T = TORIF_CODE_S"
            SQL &= " AND FURI_DATE_S >= '" & ParaHassinDate & "'"
            ' �����U��
            SQL &= " AND ((SYUBETU_T = '21'  AND FURI_DATE_S BETWEEN '" & ParaHassinDate & "' AND '" & ParaSouDate & "')"
            ' ���^�U���A�ܗ^�U��
            SQL &= "  OR  (SYUBETU_T <> '21' AND FURI_DATE_S BETWEEN '" & ParaHassinDate & "' AND '" & ParaKyuDate & "'))"
            SQL &= " AND TOUROKU_FLG_S = '1'"
            SQL &= " AND HASSIN_FLG_S = '2'"
            SQL &= " AND TYUUDAN_FLG_S = '0'"

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    SQL = "UPDATE S_SCHMAST SET"
                    SQL &= " HASSIN_FLG_S = '0'"
                    SQL &= " WHERE TORIS_CODE_S = '" & OraReader.GetString("TORIS_CODE_S") & "'"
                    SQL &= " AND TORIF_CODE_S = '" & OraReader.GetString("TORIF_CODE_S") & "'"
                    SQL &= " AND FURI_DATE_S = '" & OraReader.GetString("FURI_DATE_S") & "'"
                    SQL &= " AND MOTIKOMI_SEQ_S = " & OraReader.GetString("MOTIKOMI_SEQ_S")

                    Ret += MainDB.ExecuteNonQuery(SQL)

                    OraReader.NextRead()
                End While
            End If

            OraReader.Close()
            OraReader = Nothing

            'SQL = "UPDATE S_SCHMAST SET"
            'SQL &= " HASSIN_FLG_S = '0'"
            'SQL &= " WHERE HASSIN_YDATE_S = '" & ParaHassinDate & "'"
            ''2012/04/02 saitou �W���C�� MODIFY ---------------------------------------->>>>
            'SQL &= "   AND HASSIN_FLG_S = '2'"
            ''SQL &= "   AND KESSAI_FLG_S = '2'"
            ''2012/04/02 saitou �W���C�� MODIFY ----------------------------------------<<<<

            'Ret = MainDB.ExecuteNonQuery(SQL)
            '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- END
            MainLOG.Write("���M�҂����", "����", Ret & "��")

            MainDB.Commit()
            Return 0
        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write("���M�҂����", "���s", ex.ToString)
            Return -1
        Finally
            '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� ADD ---------------------------------------- START
            If Not OraReader Is Nothing Then OraReader.Close()
            '2016/01/11 �^�X�N�j�֓� RSV2���o�b�`�Ή� ADD ---------------------------------------- END
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function

    Private Function PrintKawaseFurikomiMeisai(ByVal ExeRepo As CAstReports.ClsExecute, ByVal ReportID As String, ByVal ReportName As String) As Boolean
        Dim iRet As Integer
        Dim bRet As Boolean = True

        Dim PrnMeisai As New ClsPrnKawaseFurikomiMeisai(ReportID, ReportName)

        PrnMeisai.OraDB = MainDB
        PrnMeisai.CreateCsvFile()

        '���׍s�o��
        iRet = PrnMeisai.OutputCSVKekka(HassinList, ini_info.JIKINKO_CODE, ParaHassinDate, strDate, strTime)

        If iRet <> 0 Then
            bRet = False
            MainLOG.Write(ReportName & "�o��", "���s", ReportName & "�b�r�u�o�͂Ɏ��s���܂����B")
        End If

        If Not PrnMeisai Is Nothing And iRet = 0 Then
            PrnMeisai.CloseCsv()

            Select Case ReportID
                Case "KFSP010"
                    '���Z�@�փR�[�h�E�x�X�R�[�h�E�U�����E������R�[�h�E����敛�R�[�h�E���R�[�h�ԍ����Ń\�[�g
                    PrnMeisai.SortFile("14.4sjia 15.3sjia 3.8sjia 4.10sjia 5.2sjia 0.6sjia")

                    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
                    If ini_info.RSV2_S_KAWASE_HONSITEN = "0" Then
                        MainLOG.Write(ReportName & "���", "�I��", "����s�v")
                        Return bRet
                    End If
                    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END

                Case "KFSP011"
                    '�U�����E������R�[�h�E����敛�R�[�h�E���R�[�h�ԍ����Ń\�[�g
                    PrnMeisai.SortFile("3.8sjia 4.10sjia 5.2sjia 0.6sjia")

                    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
                    If ini_info.RSV2_S_KAWASE_TAKOU = "0" Then
                        MainLOG.Write(ReportName & "���", "�I��", "����s�v")
                        Return bRet
                    End If
                    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END
                Case "KFSP012"

                    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- START
                    If ini_info.RSV2_S_KAWASE_LOGGING = "0" Then
                        MainLOG.Write(ReportName & "���", "�I��", "����s�v")
                        Return bRet
                    End If
                    ' 2016/03/15 �^�X�N�j���� ADD �yST�zUI_B-14-99(RSV2�Ή�) -------------------- END

            End Select

            '����o�b�`�Ăяo��
            Dim param As String = ""

            '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����
            param = MainLOG.UserID & "," & PrnMeisai.FileName

            iRet = ExeRepo.ExecReport(ReportID & ".EXE", param)

            If iRet <> 0 Then
                '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                Select Case iRet
                    Case -1
                        jobMessage = ReportName & "����ΏۂO���B"
                    Case Else
                        jobMessage = ReportName & "������s�B�G���[�R�[�h�F" & iRet
                End Select

                MainLOG.Write(ReportName & "���", "���s", jobMessage)
                bRet = False
            End If

        End If

        Return bRet
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' �U���萔�����v�Z���܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/14 �W���� ����őΉ�</remarks>
    Private Function CalcFurikomiTesuu(ByVal Key As FKeyInfo) As Boolean
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        ' �U����
        Dim dtFuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

        ' �U�����̂P�c�Ɠ��O, �Q�c�Ɠ��O
        Dim Zen1Day As String = CASTCommon.GetEigyobi(dtFuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(dtFuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

        Dim SQL As New StringBuilder

        Try
            '�K�p��ʂ̔���
            Dim strTekiyoSyubetu As String = String.Empty
            Select Case ParaHassinDate
                Case Key.FURI_DATE
                    '�U��
                    strTekiyoSyubetu = "10"
                Case Zen1Day
                    '��U
                    strTekiyoSyubetu = "11"
                Case Is <= Zen2Day
                    '�Q�c�Ɠ��ȑO
                    Select Case Key.SYUBETU
                        Case "21"
                            '��U
                            strTekiyoSyubetu = "11"
                        Case "11", "12"
                            '���U
                            strTekiyoSyubetu = "12"
                    End Select
            End Select

            '�U���萔���}�X�^�̎擾
            If Me.SetFuriTesuu(Me.TAX.ZEIRITSU_ID, strTekiyoSyubetu) = False Then
                Return False
            End If

            '���׃}�X�^���擾
            With SQL
                .Append("select * ")
                .Append(" from S_MEIMAST, S_TORIMAST")
                .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
                .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
                .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
                .Append(" and MOTIKOMI_SEQ_K = " & Key.MOTIKOMI_SEQ)
                .Append(" and DATA_KBN_K = '2'")
                .Append(" and FURIKETU_CODE_K = 0")
                .Append(" and TORIS_CODE_K = TORIS_CODE_T")
                .Append(" and TORIF_CODE_K = TORIF_CODE_T")
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Dim intTesuuKin As Integer = 0

                    '�U���萔�����v�Z����
                    If oraReader.GetString("TESUU_TABLE_ID_T") = "" Then
                        '�U���萔���ID���ݒ肳��Ă��Ȃ��ꍇ�͐U���萔��0�~�Ƃ���
                        intTesuuKin = 0
                    Else
                        '�U���萔���ID���ݒ肳��Ă���ꍇ�́A���̊ID�ɕR�t���萔�����v�Z����
                        If Me.GetFurikomiTesuu(oraReader, Key, oraReader.GetString("TESUU_TABLE_ID_T"), intTesuuKin) = False Then
                            Return False
                        End If
                    End If

                    '���׃}�X�^���X�V
                    If Me.UpdTesuuKin(oraReader, intTesuuKin) = False Then
                        Return False
                    End If

                    oraReader.NextRead()
                End While
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("�U���萔���v�Z", "���s", ex.Message)
            jobMessage = "�U���萔���v�Z ���s ��O���������܂����B"
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' �U���萔���}�X�^��ǂݍ��݂܂��B
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/27 �W���� ����őΉ�</remarks>
    Private Function SetFuriTesuu(ByVal TAX_ID As String, _
                                  ByVal SYUBETU As String) As Boolean

        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            Me.htFuriTesuuID = New Hashtable

            '--------------------------------------------------
            '�U���萔���}�X�^����U���萔�����擾����
            '--------------------------------------------------
            With SQL
                .Append("select * from TESUUMAST, TAXMAST")
                .Append(" where TAX_ID_C = TAX_ID_Z")
                .Append(" and TAX_ID_C = " & SQ(TAX_ID))
                .Append(" and SYUBETU_C = " & SQ(SYUBETU))
                .Append(" and FSYORI_KBN_C = '3'")
                .Append(" order by TESUU_TABLE_ID_C")
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Dim tesuu As strcFuriTesuuInfo = New strcFuriTesuuInfo
                    tesuu.TESUU_A1 = oraReader.GetString("TESUU_A1_C")
                    tesuu.TESUU_A2 = oraReader.GetString("TESUU_A2_C")
                    tesuu.TESUU_A3 = oraReader.GetString("TESUU_A3_C")
                    tesuu.TESUU_B1 = oraReader.GetString("TESUU_B1_C")
                    tesuu.TESUU_B2 = oraReader.GetString("TESUU_B2_C")
                    tesuu.TESUU_B3 = oraReader.GetString("TESUU_B3_C")
                    tesuu.TESUU_C1 = oraReader.GetString("TESUU_C1_C")
                    tesuu.TESUU_C2 = oraReader.GetString("TESUU_C2_C")
                    tesuu.TESUU_C3 = oraReader.GetString("TESUU_C3_C")


                    '�U���萔���ID���L�[�Ƀn�b�V���e�[�u���Ɋi�[����
                    htFuriTesuuID.Add(oraReader.GetString("TESUU_TABLE_ID_C"), tesuu)

                    oraReader.NextRead()
                End While
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("�U���萔���}�X�^�擾", "���s", ex.Message)
            jobMessage = "�U���萔���}�X�^�擾 ���s ��O���������܂����B"
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' �U�����z�ɑ΂���U���萔�����v�Z���܂��B
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <param name="Key"></param>
    ''' <param name="strTesuuTableId"></param>
    ''' <param name="intTesuuKin"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/14 �W���� ����őΉ�</remarks>
    Private Function GetFurikomiTesuu(ByVal oraReader As CASTCommon.MyOracleReader, _
                                      ByVal Key As FKeyInfo, _
                                      ByVal strTesuuTableId As String, _
                                      ByRef intTesuuKin As Integer) As Boolean

        intTesuuKin = 0
        Dim tesuu As strcFuriTesuuInfo

        '�U���萔���ID�ɕR�t���萔����ݒ�
        If htFuriTesuuID.ContainsKey(strTesuuTableId) = True Then
            tesuu = New strcFuriTesuuInfo
            tesuu = DirectCast(htFuriTesuuID.Item(strTesuuTableId), strcFuriTesuuInfo)
        Else
            '�U���萔���ID�ɕR�t���萔�����ݒ肳��Ă��Ȃ��ꍇ
            '2014/03/02 saitou �W���� ����őΉ� UPD -------------------------------------------------->>>>
            '�U���萔��=0�ɂ����A�ُ�I��������
            MainLOG.Write("�U���萔���v�Z", "���s", "�U���萔���}�X�^�ݒ�Ȃ� �萔��ID�F" & strTesuuTableId)
            Return False
            ''���̂܂�0��Ԃ�
            'intTesuuKin = 0
            'Return True
            '2014/03/02 saitou �W���� ����őΉ� UPD --------------------------------------------------<<<<
        End If

        Try
            If oraReader.GetInt("FURIKETU_CODE_K") = 0 Then
                '2013/12/27 saitou �W���� �󎆐őΉ� UPD -------------------------------------------------->>>>
                If oraReader.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
                    '�U�����Z�@�ւ������ɂ̏ꍇ
                    If oraReader.GetString("KEIYAKU_SIT_K") = Key.TSIT_NO Then
                        '�U���x�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�A���X��
                        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI1 Then
                            intTesuuKin = CInt(tesuu.TESUU_A1)
                        ElseIf Me.TAX.INSHIZEI1 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI2 Then
                            intTesuuKin = CInt(tesuu.TESUU_A2)
                        ElseIf Me.TAX.INSHIZEI2 <= oraReader.GetInt64("FURIKIN_K") Then
                            intTesuuKin = CInt(tesuu.TESUU_A3)
                        End If
                    Else
                        '�U���x�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�A�{�x�X
                        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI1 Then
                            intTesuuKin = CInt(tesuu.TESUU_B1)
                        ElseIf Me.TAX.INSHIZEI1 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI2 Then
                            intTesuuKin = CInt(tesuu.TESUU_B2)
                        ElseIf Me.TAX.INSHIZEI2 <= oraReader.GetInt64("FURIKIN_K") Then
                            intTesuuKin = CInt(tesuu.TESUU_B3)
                        End If
                    End If
                Else
                    '�U�����Z�@�ւ����s�̏ꍇ
                    If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI1 Then
                        intTesuuKin = CInt(tesuu.TESUU_C1)
                    ElseIf Me.TAX.INSHIZEI1 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < Me.TAX.INSHIZEI2 Then
                        intTesuuKin = CInt(tesuu.TESUU_C2)
                    ElseIf Me.TAX.INSHIZEI2 <= oraReader.GetInt64("FURIKIN_K") Then
                        intTesuuKin = CInt(tesuu.TESUU_C3)
                    End If
                End If

                'If oraReader.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
                '    '�U�����Z�@�ւ������ɂ̏ꍇ
                '    If oraReader.GetString("KEIYAKU_SIT_K") = Key.TSIT_NO Then
                '        '�U���x�X���Ƃ�܂ƂߓX�ƈ�v����ꍇ�A���X��
                '        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 10000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_A1)
                '        ElseIf 10000 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 30000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_A2)
                '        ElseIf 30000 <= oraReader.GetInt64("FURIKIN_K") Then
                '            intTesuuKin = CInt(tesuu.TESUU_A3)
                '        End If
                '    Else
                '        '�U���x�X���Ƃ�܂ƂߓX�ƈ�v���Ȃ��ꍇ�A�{�x�X
                '        If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 10000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_B1)
                '        ElseIf 10000 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 30000 Then
                '            intTesuuKin = CInt(tesuu.TESUU_B2)
                '        ElseIf 30000 <= oraReader.GetInt64("FURIKIN_K") Then
                '            intTesuuKin = CInt(tesuu.TESUU_B3)
                '        End If
                '    End If
                'Else
                '    '�U�����Z�@�ւ����s�̏ꍇ
                '    If 0 < oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 10000 Then
                '        intTesuuKin = CInt(tesuu.TESUU_C1)
                '    ElseIf 10000 <= oraReader.GetInt64("FURIKIN_K") AndAlso oraReader.GetInt64("FURIKIN_K") < 30000 Then
                '        intTesuuKin = CInt(tesuu.TESUU_C2)
                '    ElseIf 30000 <= oraReader.GetInt64("FURIKIN_K") Then
                '        intTesuuKin = CInt(tesuu.TESUU_C3)
                '    End If
                'End If
                '2013/12/27 saitou �W���� �󎆐őΉ� UPD --------------------------------------------------<<<<
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("�U���萔���v�Z", "���s", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' ���׃}�X�^�̐U���萔�����X�V���܂��B
    ''' </summary>
    ''' <param name="oraReader"></param>
    ''' <param name="intTesuuKin"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/11/14 �W���� ����őΉ�</remarks>
    Private Function UpdTesuuKin(ByVal oraReader As CASTCommon.MyOracleReader, _
                                 ByVal intTesuuKin As Integer) As Boolean
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("update S_MEIMAST set ")
                .Append(" TESUU_KIN_K = " & intTesuuKin.ToString)
                .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_K")))
                .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_K")))
                .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_K")))
                .Append(" and MOTIKOMI_SEQ_K = " & oraReader.GetString("MOTIKOMI_SEQ_K"))
                .Append(" and RECORD_NO_K = " & oraReader.GetString("RECORD_NO_K"))
            End With

            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                MainLOG.Write("�U���萔���X�V", "���s", MainDB.Message)
                jobMessage = "�U���萔���X�V ���s ���O���Q�Ƃ��ĉ������B"
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("�U���萔���X�V", "���s", ex.Message)
            jobMessage = "�U���萔���X�V ���s ��O���������܂����B"
            Return False
        End Try

    End Function


    ''' <summary>
    ''' �����U�˗��l�A�g�f�[�^�e�[�u���ɑ}�����܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="KData"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function InsertSIRenkeimast(ByVal Key As FKeyInfo, _
                                        ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim Data As New CAstFormKes.ClsFormSikinFuri.T_48100
        Data.Data = KData.record320

        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("insert into S_I_RENKEIMAST (")
                .Append(" FSYORI_KBN_SR")
                .Append(",TORIS_CODE_SR")
                .Append(",TORIF_CODE_SR")
                .Append(",REC_RENKEI_KBN_SR")
                .Append(",SAKUSEI_DATE_SR")
                .Append(",SAKUSEI_TIME_SR")
                .Append(",KAIJI_SR")
                .Append(",UKETUKE_DATE_SR")
                .Append(",FURI_DATE_SR")
                .Append(",REN_TORIKOMI_DATE_SR")
                .Append(",REN_KEKKA_DATE_SR")
                .Append(",FURI_CODE_SR")
                .Append(",ITAKU_CODE_SR")
                .Append(",CYCLE_NO_SR")
                .Append(",TUKESIT_NO_SR")
                .Append(",TUKEKAMOKU_SR")
                .Append(",TUKEKOUZA_SR")
                .Append(",ITAKU_KNAME_SR")
                .Append(",ITAKU_NNAME_SR")
                .Append(",KYUUYO_KBN_SR")
                .Append(",SYUBETU_KBN_SR")
                .Append(",KESSAI_PATN_SR")
                .Append(",TIEN_KBN_SR")
                .Append(",HASSIN_NO_SR")
                .Append(",KESSAI_NO_SR")
                .Append(",FURIKIN_KEI_SR")
                .Append(",FURIKIN_KEIJ_SR")
                .Append(",FURIKIN_KEIH_SR")
                .Append(",FURIKIN_KEIT_SR")
                .Append(",TESUU_KIN_KEI_SR")
                .Append(",KIN_STS_SR")
                .Append(",SOU_SYUBETU_SR")
                .Append(",TESUUTYO_KBN_SR")
                .Append(",FURI_STS_SR")
                .Append(",TESUU_STS_SR")
                .Append(",KAWASE_STS_SR")
                .Append(",MODOSI_STS_SR")
                .Append(",TUKEKAE_STS_SR")
                .Append(",TEN_STS_SR")
                .Append(",ERR_MESSAGE_CODE_SR")
                .Append(",ERR_MESSAGE_SR")
                .Append(",TIME_STAMP_SR")
                .Append(",RIENTA_FD_SR")
                .Append(",SOUFURI_NO_SR")
                .Append(",HASSIN_DATE_SR")
                .Append(",KESSAI_KANRYO_DATE_SR")
                .Append(",HASSIN_FURI_CODE_SR")
                .Append(",HASSIN_KBN_SR")
                .Append(",TORIKESI_HORYU_KBN_SR")
                .Append(",HASSIN_FURIKIN_KEI_SR")
                .Append(",HASSIN_FURIKIN_KEIJ_SR")
                .Append(",HASSIN_FURIKIN_KEIH_SR")
                .Append(",HASSIN_FURIKIN_KEIT_SR")
                .Append(",HASSIN_TESUU_KIN_KEI_SR")
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- START
                .Append(",UKETUKE_KBN_SR")
                .Append(",KEKKA_HASSO_KBN_SR")
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- END
                .Append(" ) values (")
                .Append(" " & SQ("3"))                          '�U�֏����敪
                .Append("," & SQ(Key.TORIS_CODE))               '������R�[�h
                .Append("," & SQ(Key.TORIF_CODE))               '����敛�R�[�h
                .Append("," & SQ("1"))                          '���R�[�h�A�g�敪
                .Append("," & SQ(strDate))                      '�쐬��
                .Append("," & SQ(strTime))                      '�쐬����
                .Append("," & SQ(haskey.Kaiji.ToString.PadLeft(2, "0"c)))                 '�쐬��
                .Append("," & SQ(Key.TOUROKU_DATE))             '��t��
                .Append("," & SQ(Key.FURI_DATE))                '�U���w���
                .Append("," & SQ("00000000000000"))             '�A�g��g��
                .Append("," & SQ("00000000000000"))             '�A�g���ʍX�V��
                .Append("," & SQ(Data.SYUMOKU))                 '�U�����
                .Append("," & SQ(Key.ITAKU_CODE))               '�ϑ��҃R�[�h
                .Append("," & Key.MOTIKOMI_SEQ)                 '�T�C�N���ԍ�
                .Append("," & SQ(Key.TUKESIT_NO))               '�˗��l�X�܃R�[�h
                .Append("," & SQ(Key.TUKEKAMOKU))               '�˗��l�ȖڃR�[�h
                .Append("," & SQ(Key.TUKEKOUZA))                '�˗��l�����ԍ�
                .Append("," & SQ(Key.ITAKU_KNAME))              '�˗��l���J�i
                .Append("," & SQ(Key.ITAKU_NNAME))              '�˗��l������
                Select Case Key.SYUBETU
                    Case "21"
                        .Append("," & SQ("0"))
                    Case Else
                        Select Case Data.SYUMOKU.Substring(0, 2)
                            Case "10", "11"
                                .Append("," & SQ("1"))
                            Case "12"
                                .Append("," & SQ("2"))
                        End Select
                End Select
                Select Case Key.SYUBETU
                    Case "21" : .Append("," & SQ("0"))
                    Case "11" : .Append("," & SQ("1"))
                    Case "12" : .Append("," & SQ("2"))
                End Select

                .Append("," & SQ(Me.GetKessaiPatn(Key)))        '�����m�ۃp�^�[��
                .Append("," & CInt(Me.GetTienKbn(Key)))         '����x���敪
                Select Case Data.SYUMOKU.Substring(0, 2)        '���M������
                    Case "10" : .Append("," & "0")
                    Case "11" : .Append("," & "1")
                    Case "12" : .Append("," & "2")
                End Select
                Select Case Data.SYUMOKU.Substring(0, 2)        '�����m�ۓ���
                    Case "10" : .Append("," & "0")
                    Case "11" : .Append("," & "1")
                    Case "12" : .Append("," & "2")
                End Select

                .Append("," & Key.FURIKIN_KEI)                  '�U�����z���v
                .Append("," & Key.FURIKIN_KEIJ)                 '�U�����z���v���󎩓X��
                .Append("," & Key.FURIKIN_KEIH)                 '�U�����z���v����{�x�X
                .Append("," & Key.FURIKIN_KEIT)                 '�U�����z���v���󑼍s��
                .Append("," & Key.TESUU_KIN_KEI)                '�萔�����z���v
                .Append("," & "0")                              '������
                .Append("," & "0")                              '���U���
                Select Case Key.TESUUTYO_KBN                    '�萔�������敪
                    Case "0" : .Append("," & "1")
                    Case Else : .Append("," & "3")
                End Select

                '�U�����z�o���󋵁i�����l�v�m�F�j
                Select Case CInt(Me.GetKessaiPatn(Key))
                    Case 0, 1 '�����m�ەs�v
                        .Append("," & "0")
                    Case Else
                        .Append("," & "1")
                End Select

                '�萔���o���󋵁i�����l�v�m�F�j
                Select Case CInt(Me.GetKessaiPatn(Key))
                    Case 2, 3 '�萔���ʏo���A���Z
                        .Append("," & "1")
                    Case Else
                        .Append("," & "0")
                End Select
                .Append("," & "0")                              '�ב֔��M�󋵁i�����l�v�m�F�j
                .Append("," & "0")                              '�g�ߏ�
                .Append("," & "0")                              '�ʒi�t�֏�
                .Append("," & "0")                              '�����������
                .Append("," & SQ("0000"))                       '�G���[�R�[�h
                .Append("," & SQ(""))                           '�G���[���e
                .Append("," & SQ("00000000000000"))             '���G���^�쐬�^�C���X�^���v
                .Append("," & SQ("0"))                          '���G���^FD�쐬�t���O
                .Append("," & "0")                              '�����U���ʔ�
                .Append("," & "0")                              '���M��
                .Append("," & "0")                              '�����m�ۊ�����
                .Append("," & "0")                              '���M�U�����
                .Append("," & "0")                              '���M�敪
                .Append("," & "0")                              '����ۗ��敪
                .Append("," & "0")                              '���M���U�����z���v
                .Append("," & "0")                              '���M���U�����z���v���󎩓X��
                .Append("," & "0")                              '���M���U�����z���v����{�x�X
                .Append("," & "0")                              '���M���U�����z���v���󑼍s��
                .Append("," & "0")                              '���M���U���萔�����v
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- START
                .Append("," & CInt(Me.GetUketukeKbn(Key)))      '��t�`�ԋ敪
                .Append("," & "0")                              '�U���T�[�r�X�������ʗv�ۋ敪
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- END
                .Append(")")
            End With

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(�����U�˗��l�A�g�f�[�^�e�[�u���o�^)", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' �����U���טA�g�f�[�^�e�[�u���ɑ}�����܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="KData"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function InsertSMRenkeimast(ByVal Key As FKeyInfo, _
                                        ByVal KData As CAstFormKes.ClsFormKes.KessaiData, _
                                        ByVal oraMei As CASTCommon.MyOracleReader) As Boolean
        Dim Data As New CAstFormKes.ClsFormSikinFuri.T_48100
        Data.Data = KData.record320

        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("insert into S_M_RENKEIMAST (")
                .Append(" FSYORI_KBN_SR")
                .Append(",TORIS_CODE_SR")
                .Append(",TORIF_CODE_SR")
                .Append(",REC_RENKEI_KBN_SR")
                .Append(",KAWASE_STS_SR")
                .Append(",SAKUSEI_DATE_SR")
                .Append(",SAKUSEI_TIME_SR")
                .Append(",KAIJI_SR")
                .Append(",UKETUKE_DATE_SR")
                .Append(",FURI_DATE_SR")
                .Append(",REN_TORIKOMI_DATE_SR")
                .Append(",REN_KEKKA_DATE_SR")
                .Append(",SYUMOKU_SR")
                .Append(",ITAKU_CODE_SR")
                .Append(",CYCLE_NO_SR")
                .Append(",RECORD_SR")
                .Append(",KEIYAKU_KIN_KNAME_SR")
                .Append(",KEIYAKU_KIN_SR")
                .Append(",KEIYAKU_SIT_KNAME_SR")
                .Append(",KEIYAKU_SIT_SR")
                .Append(",FUKA_CODE_SR")
                .Append(",FURIKIN_SR")
                .Append(",TESUU_KIN1_SR")
                .Append(",TESUU_KIN2_SR")
                .Append(",TESUU_KIN3_SR")
                .Append(",KEIYAKU_KAMOKU_SR")
                .Append(",KEIYAKU_KOUZA_SR")
                .Append(",KEIYAKU_KNAME_SR")
                .Append(",BIKOU1_SR")
                .Append(",BIKOU2_SR")
                .Append(",EDI_SR")
                .Append(",TIME_STAMP_SR")
                .Append(",TORIKESI_KBN_SR")
                .Append(",HASSIN_TESUU_KIN_SR")
                .Append(",KAWASE_NO_SR")
                .Append(",ERR_MESSAGE_CODE_SR")
                .Append(",ERR_MESSAGE_SR")
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- START
                .Append(",CC_KBN_SR")
                .Append(",SYAIN_NO_SR")
                .Append(",SYOZOKU_CODE_SR")
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- END
                .Append(" ) values ( ")
                .Append(" " & SQ("3"))                      '�U�֏����敪
                .Append("," & SQ(Key.TORIS_CODE))           '������R�[�h
                .Append("," & SQ(Key.TORIF_CODE))           '����敛�R�[�h
                .Append("," & SQ("2"))                      '���R�[�h�A�g�敪
                .Append("," & "0")                          '�ב֔��M��
                .Append("," & SQ(strDate))                  '�쐬��
                .Append("," & SQ(strTime))                  '�쐬����
                .Append("," & SQ(haskey.Kaiji.ToString.PadLeft(2, "0"c)))             '�쐬��
                .Append("," & SQ(Key.TOUROKU_DATE))         '��t��
                .Append("," & SQ(Key.FURI_DATE))            '�U���w���
                .Append("," & SQ("00000000000000"))         '�A�g�捞��
                .Append("," & SQ("00000000000000"))         '�A�g���ʍX�V��
                .Append("," & SQ(Data.SYUMOKU))             '�U�����
                .Append("," & SQ(Key.ITAKU_CODE))           '�˗��l�ԍ�
                .Append("," & Key.MOTIKOMI_SEQ)             '�T�C�N���ԍ�
                .Append("," & haskey.RecordNo)              '���R�[�h�ʔ�
                Dim KinCd As String = KData.KesKinCode
                Dim SitCd As String = KData.KesSitCode
                Dim KinName As String = String.Empty
                Dim SitName As String = String.Empty
                If GetTenmast(KinCd, SitCd, "", "", KinName, SitName) Then
                    If KinCd.Equals(ini_info.JIKINKO_CODE) = True Then
                        .Append("," & SQ(""))
                    Else
                        .Append("," & SQ(KinName))
                    End If
                    .Append("," & SQ(KinCd))                '���l���Z�@�֔ԍ�
                    .Append("," & SQ(SitName))              '���l�x�X���J�i
                    .Append("," & SQ(SitCd))                '���l�x�X�ԍ�
                Else
                    .Append("," & SQ(""))
                    .Append("," & SQ(""))
                    .Append("," & SQ(""))
                    .Append("," & SQ(""))
                End If
                .Append("," & SQ("000"))                    '���l�t���R�[�h
                .Append("," & CLng(Data.KINGAKU.Trim))      '���l�U�����z
                '2016/01/20 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- START
                Dim FurikomiTesuuKin As Integer = 0

                Select Case Data.SYUMOKU.Substring(0, 2)
                    Case "12"
                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "12", "12")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "���U�萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "11", "12")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "��U�萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "10", "12")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "�U���萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                    Case "11"
                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "12", "11")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "���U�萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "11", "11")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "��U�萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "10", "11")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "�U���萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                    Case "10"
                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "12", "10")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "���U�萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "11", "10")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "��U�萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                        FurikomiTesuuKin = Me.GetFurikomiTesuuKinBatch(Key, KData, Data, "10", "10")
                        If FurikomiTesuuKin = -1 Then
                            MainLOG.Write("�����U���טA�g�f�[�^�e�[�u���o�^", "���s", "�U���萔���ݒ莸�s")
                            Return False
                        End If

                        .Append("," & FurikomiTesuuKin.ToString)

                End Select

                'Select Case Data.SYUMOKU.Substring(0, 2)
                '    Case "12"
                '        .Append("," & KData.FurikomiTesuukin)
                '        .Append("," & KData.FurikomiTesuukin)
                '        .Append("," & KData.FurikomiTesuukin)
                '    Case "11"
                '        .Append("," & "0")
                '        .Append("," & KData.FurikomiTesuukin)
                '        .Append("," & KData.FurikomiTesuukin)
                '    Case "10"
                '        .Append("," & "0")
                '        .Append("," & "0")
                '        .Append("," & KData.FurikomiTesuukin)
                'End Select
                '2016/01/20 �^�X�N�j�֓� RSV2���o�b�`�Ή� UPD ---------------------------------------- END
                .Append("," & SQ(Data.UKETORI_KAMOKU))      '���l�ȖڃR�[�h
                .Append("," & SQ(Data.UKETORI_KOUZA.Trim))  '���l�����ԍ�
                .Append("," & SQ(Data.UKETORI_NAME))        '���l���J�i
                .Append("," & SQ(Data.BIKOU1))              '���l1
                .Append("," & SQ(Data.BIKOU2))              '���l2
                .Append("," & SQ(Data.EDI_INFO))            'EDI���
                .Append("," & SQ("00000000000000"))         '�^�C���X�^���v
                .Append("," & "0")                          '����敪
                .Append("," & "0")                          '���M�����l�萔��
                .Append("," & SQ(""))                       '�ב֒ʔ�
                .Append("," & SQ(""))                       '�G���[���b�Z�[�W�R�[�h
                .Append("," & SQ(""))                       '�G���[���b�Z�[�W
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- START
                .Append("," & "0")                          '�Z���^�J�b�g�Ώۋ敪
                .Append("," & SQ(oraMei.GetString("JYUYOUKA_NO_K", False).PadRight(24).Substring(0, 10)))
                .Append("," & SQ(oraMei.GetString("JYUYOUKA_NO_K", False).PadRight(24).Substring(10, 10)))
                ' 2017/04/13 �^�X�N�j���� ADD �yME�z(SKForce�A�g�e�[�u�����ڒǉ�(�W����)) -------------------- END
                .Append(" )")
            End With

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(�����U���טA�g�f�[�^�e�[�u���o�^)", "���s", ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' �e��U�����z���v���v�Z���܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function CalcFurikinKei(ByRef Key As FKeyInfo) As Boolean
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("select ")
                .Append(" sum(FURIKIN_K) as FURIKIN_KEI")
                .Append(",sum(case when KEIYAKU_KIN_K = " & SQ(Key.TUKEKIN_NO) & " and KEIYAKU_SIT_K = " & SQ(Key.TUKESIT_NO) & " then FURIKIN_K else 0 end) as FURIKIN_KEIJ")
                .Append(",sum(case when KEIYAKU_KIN_K = " & SQ(Key.TUKEKIN_NO) & " and KEIYAKU_SIT_K <> " & SQ(Key.TUKESIT_NO) & " then FURIKIN_K else 0 end) as FURIKIN_KEIH")
                .Append(",sum(case when KEIYAKU_KIN_K <> " & SQ(Key.TUKEKIN_NO) & " then FURIKIN_K else 0 end) as FURIKIN_KEIT")
                .Append(",sum(TESUU_KIN_K) as TESUU_KIN_KEI")
                .Append(" from S_MEIMAST")
                .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
                .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
                .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
                .Append(" and MOTIKOMI_SEQ_K = " & Key.MOTIKOMI_SEQ)
                .Append(" and DATA_KBN_K = '2'")
                .Append(" and FURIKETU_CODE_K = 0")
            End With

            If oraReader.DataReader(SQL) = True Then
                Key.FURIKIN_KEI = oraReader.GetInt64("FURIKIN_KEI")
                Key.FURIKIN_KEIJ = oraReader.GetInt64("FURIKIN_KEIJ")
                Key.FURIKIN_KEIH = oraReader.GetInt64("FURIKIN_KEIH")
                Key.FURIKIN_KEIT = oraReader.GetInt64("FURIKIN_KEIT")
                Key.TESUU_KIN_KEI = oraReader.GetInt64("TESUU_KIN_KEI")
            Else
                Key.FURIKIN_KEI = 0
                Key.FURIKIN_KEIJ = 0
                Key.FURIKIN_KEIH = 0
                Key.FURIKIN_KEIT = 0
                Key.TESUU_KIN_KEI = 0
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("(�U�����z�v�Z)", "���s", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' �����m�ۃp�^�[����Ԃ��܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetKessaiPatn(ByVal Key As FKeyInfo) As String

        ' �U����
        Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)
        ' �U�����̂P�c�Ɠ��O�C�Q�c�Ɠ��O�A�R�c�Ɠ��O
        Dim Zen1Day As String = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen3Day As String = CASTCommon.GetEigyobi(FuriDate, -3, FmtComm.HolidayList).ToString("yyyyMMdd")

        '2016/01/11 �^�X�N�j�֓� ���o�b�`�Ή� UPD ---------------------------------------- START
        Select Case Key.KESSAI_PATN
            Case "1"        '�c�ƓX�m��
                Return "1"      '�����m�ەs�v(�ב֎������M�Ώ�)

            Case "0"        '���O�m��
                Select Case Key.TESUUTYO_KBN
                    Case "1"        '�ꊇ����
                        Return "4"      '�萔�����
                    Case "3"        '�ʓr����
                        Return "5"      '�萔���ʎ�
                    Case Else
                        Return "2"      '�萔���ʏo��
                End Select

            Case Else       '�m�ۑΏۊO
                Return "1"      '�����m�ەs�v(�ב֎������M�Ώ�)

        End Select

        ''�m�ۑO�ɔ��M��������̃`�F�b�N
        'Dim bHassinBeforeKakuho As Boolean = False
        'Select Case Key.SYUBETU
        '    Case "21"       '�U��
        '        If Key.KINSI.Equals("1") = False Then
        '            '�֎~�łȂ�
        '            If Key.IRAI_DATE <= Zen2Day Then
        '                '�˗������U������2�c�Ɠ��ȑO
        '                bHassinBeforeKakuho = True
        '            End If
        '        End If

        '    Case "11", "12"     '���^�E�ܗ^
        '        If Key.KINSI.Equals("1") = False Then
        '            '�֎~�łȂ�
        '            If Key.IRAI_DATE <= Zen3Day Then
        '                '�˗������U������3�c�Ɠ��ȑO
        '                bHassinBeforeKakuho = True
        '            End If
        '        End If
        'End Select

        'Select Case Key.KESSAI_PATN
        '    Case "1"        '�c�ƓX�m��
        '        If bHassinBeforeKakuho = True Then
        '            '�m�ۑO�ɔ��M����
        '            Return "1"          '�����m�ەs�v(�ב֎������M�Ώ�)
        '        Else
        '            '�m�ۑO�ɔ��M���Ȃ�
        '            Return "0"          '�����m�ەs�v(�ב֎������M�ΏۊO)
        '        End If

        '    Case "0"        '���O�m��
        '        If bHassinBeforeKakuho = True Then
        '            Select Case Key.TESUUTYO_KBN
        '                Case "1"        '�ꊇ����
        '                    Return "4"      '�萔�����
        '                Case "3"        '�ʓr����
        '                    Return "5"      '�萔���ʎ�
        '                Case Else
        '                    Return "2"      '�萔���ʏo��
        '            End Select
        '        Else
        '            Select Case Key.TESUUTYO_KBN
        '                Case "1"        '�ꊇ����
        '                    Return "4"      '�萔�����
        '                Case "3"        '�ʓr����
        '                    Return "5"      '�萔���ʎ�
        '                Case Else
        '                    Return "2"      '�萔���ʏo��
        '            End Select
        '        End If

        '    Case Else
        '        Return "2"

        'End Select
        '2016/01/11 �^�X�N�j�֓� ���o�b�`�Ή� UPD ---------------------------------------- END

    End Function

    ''' <summary>
    ''' ����x���敪��Ԃ��܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetTienKbn(ByVal Key As FKeyInfo) As String

        '2016/01/11 �^�X�N�j�֓� ���o�b�`�Ή� UPD ---------------------------------------- START
        '�x������Ȃ�
        Return "0"

        'If Key.SYUBETU = "11" OrElse Key.SYUBETU = "12" Then
        '    Dim TienDay As Integer

        '    If sdDelayList.ContainsKey(Key.BAITAI_CODE) = True Then
        '        TienDay = CInt(sdDelayList.Item(Key.BAITAI_CODE))
        '    Else
        '        TienDay = 3
        '    End If

        '    Dim FURI_DATE As Date = CASTCommon.ConvertDate(Key.FURI_DATE)
        '    Dim TienDate As String = CASTCommon.GetEigyobi(FURI_DATE, TienDay * (-1), FmtComm.HolidayList).ToString("yyyyMMdd")

        '    If Key.IRAI_DATE > TienDate Then
        '        Return "1"
        '    Else
        '        Return "0"
        '    End If

        'Else
        '    '���U�͒x�����薳��
        '    Return "0"
        'End If
        '2016/01/11 �^�X�N�j�֓� ���o�b�`�Ή� UPD ---------------------------------------- END

    End Function

    ''' <summary>
    ''' �����U���טA�g�f�[�^�e�[�u���̐U���萔�����v�Z���܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="KData"></param>
    ''' <param name="Data"></param>
    ''' <param name="GetTesuuSyubetu"></param>
    ''' <param name="HassinSyubetu"></param>
    ''' <returns>���� - �U���萔�� , �ُ� - -1</returns>
    ''' <remarks>2016/01/20 �^�X�N�j�֓� RSV2 added for ���o�b�`�Ή�</remarks>
    Private Function GetFurikomiTesuuKinBatch(ByVal Key As FKeyInfo, _
                                              ByVal KData As CAstFormKes.ClsFormKes.KessaiData, _
                                              ByVal Data As CAstFormKes.ClsFormSikinFuri.T_48100, _
                                              ByVal GetTesuuSyubetu As String, _
                                              ByVal HassinSyubetu As String) As Integer

        Try
            Dim intTesuuKin As Integer = 0
            Dim tesuu As TesuuTable

            If Me.ini_info.ZEIKIJUN.Equals("0") = True Then
                '�U�����

                '�ŗ��ƉېŔ͈͂��擾����
                Me.TAX.GetZeiritsu(Key.FURI_DATE)
                Me.TAX.GetInshizei(Key.FURI_DATE)

                '�U���萔�����擾
                If Me.TesuuList.ContainsKey(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu) = True Then
                    tesuu = New TesuuTable
                    tesuu = DirectCast(Me.TesuuList.Item(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu), TesuuTable)
                Else
                    '�U���萔�����ݒ肳��Ă��Ȃ��ꍇ
                    MainLOG.Write("�U���萔���v�Z", "���s", "�U���萔���}�X�^�ݒ�Ȃ� �萔��ID�F" & Key.TESUU_TABLE_ID)
                    Return -1
                End If

            Else
                '���M���
                Dim KijunDate As String = String.Empty

                Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

                Select Case HassinSyubetu
                    Case "12"
                        '���U���M
                        If GetTesuuSyubetu = "12" Then
                            '�萔����ʂ����U�̏ꍇ�́A����ɔ��M����ݒ�
                            KijunDate = ParaHassinDate
                        ElseIf GetTesuuSyubetu = "11" Then
                            '�萔����ʂ���U�̏ꍇ�́A����ɐU������1�c�Ɠ��O��ݒ�
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
                        Else
                            '�萔����ʂ��U���̏ꍇ�́A����ɐU������ݒ�
                            KijunDate = Key.FURI_DATE
                        End If

                    Case "11"
                        '��U���M
                        If GetTesuuSyubetu = "12" Then
                            '�萔����ʂ����U�̏ꍇ�́A����ɐU������2�c�Ɠ��O��ݒ�
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")
                        ElseIf GetTesuuSyubetu = "11" Then
                            '�萔����ʂ���U�̏ꍇ�́A����ɔ��M����ݒ�
                            KijunDate = ParaHassinDate
                        Else
                            '�萔����ʂ��U���̏ꍇ�́A����ɐU������ݒ�
                            KijunDate = Key.FURI_DATE
                        End If
                    Case "10"
                        '�U�����M
                        If GetTesuuSyubetu = "12" Then
                            '�萔����ʂ����U�̏ꍇ�́A����ɐU������2�c�Ɠ��O��ݒ�
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")
                        ElseIf GetTesuuSyubetu = "11" Then
                            '�萔����ʂ���U�̏ꍇ�́A����ɐU������1�c�Ɠ��O��ݒ�
                            KijunDate = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
                        Else
                            '�萔����ʂ��U���̏ꍇ�́A����ɔ��M����ݒ�
                            KijunDate = ParaHassinDate
                        End If
                End Select

                '�ŗ��ƉېŔ͈͂��擾����
                Me.TAX.GetZeiritsu(KijunDate)
                Me.TAX.GetInshizei(KijunDate)

                '�U���萔�����擾
                If Me.TesuuList.ContainsKey(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu) = True Then
                    tesuu = New TesuuTable
                    tesuu = DirectCast(Me.TesuuList.Item(Me.TAX.ZEIRITSU_ID & Key.TESUU_TABLE_ID & GetTesuuSyubetu), TesuuTable)
                Else
                    '�U���萔�����ݒ肳��Ă��Ȃ��ꍇ
                    MainLOG.Write("�U���萔���v�Z", "���s", "�U���萔���}�X�^�ݒ�Ȃ� �萔��ID�F" & Key.TESUU_TABLE_ID)
                    Return -1
                End If
            End If

            '�U���萔���̌���
            If KData.KesKinCode = Me.ini_info.JIKINKO_CODE Then
                If KData.KesSitCode = Key.TSIT_NO Then
                    If 0 < CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI1 Then
                        intTesuuKin = CInt(tesuu.TESUU_A1)
                    ElseIf Me.TAX.INSHIZEI1 <= CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI2 Then
                        intTesuuKin = CInt(tesuu.TESUU_A2)
                    ElseIf Me.TAX.INSHIZEI2 <= CLng(Data.KINGAKU) Then
                        intTesuuKin = CInt(tesuu.TESUU_A3)
                    End If
                Else
                    If 0 < CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI1 Then
                        intTesuuKin = CInt(tesuu.TESUU_B1)
                    ElseIf Me.TAX.INSHIZEI1 <= CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI2 Then
                        intTesuuKin = CInt(tesuu.TESUU_B2)
                    ElseIf Me.TAX.INSHIZEI2 <= CLng(Data.KINGAKU) Then
                        intTesuuKin = CInt(tesuu.TESUU_B3)
                    End If
                End If

            Else
                If 0 < CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI1 Then
                    intTesuuKin = CInt(tesuu.TESUU_C1)
                ElseIf Me.TAX.INSHIZEI1 <= CLng(Data.KINGAKU) AndAlso CLng(Data.KINGAKU) < Me.TAX.INSHIZEI2 Then
                    intTesuuKin = CInt(tesuu.TESUU_C2)
                ElseIf Me.TAX.INSHIZEI2 <= CLng(Data.KINGAKU) Then
                    intTesuuKin = CInt(tesuu.TESUU_C3)
                End If
            End If

            Return intTesuuKin

        Catch ex As Exception
            MainLOG.Write("�U���萔���v�Z", "���s", ex.Message)
            Return -1

        End Try
    End Function

    ''' <summary>
    ''' ��t�`�ԋ敪��Ԃ��܂��B
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetUketukeKbn(ByVal Key As FKeyInfo) As String

        Try
            Dim ReturnCode As String = "0"
            Dim TextFile As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "��t�`�ԋ敪_�ϊ��e�[�u��.TXT")
            ReturnCode = CASTCommon.GetText_CodeToName(TextFile, Key.BAITAI_CODE)

            Select Case ReturnCode.Trim
                Case ""
                    Return "0"
                Case Else
                    Return ReturnCode
            End Select
        Catch ex As Exception
            Return "0"
        End Try

    End Function

End Class
