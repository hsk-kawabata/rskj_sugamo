Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports CASTCommon.ModPublic

'*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
'*** Str Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***
Imports System.Configuration
Imports System.Reflection
Imports System.Xml
'*** End Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***

' �f�[�^�t�H�[�}�b�g��{�N���X
Public Class CFormat
    Implements IDisposable

    Protected clsfusion As New clsFUSION.clsMain

    '*** Str Add 2015/12/01 SO)�r�� for �o�^�o���Ή� ***
    Private FmtKubun As String = ""
    '*** End Add 2015/12/01 SO)�r�� for �o�^�o���Ή� ***

    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
    Protected MOTIKOMI_DATE As String = ""  '��������
    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

    '2014/05/21 saitou �W���� DEL -------------------------------------------------->>>>
    ''�R���t�����g�U�֓��}���`�Ή� >>
    'Private PreFURI_DATE As String = ""
    ''�R���t�����g�U�֓��}���`�Ή� <<
    '2014/05/21 saitou �W���� DEL --------------------------------------------------<<<<

    ' �����N���ی��� �����R�[�h
    '*** Str Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή��i�ʃN���X�̌ʃ��\�b�h����Q�Ɖ\�Ƃ���j ***
    'Protected KokuminNenkinTori As String = CASTCommon.GetFSKJIni("KEKKA", "KOKUMINNENKIN")
    Public KokuminNenkinTori As String = CASTCommon.GetFSKJIni("KEKKA", "KOKUMINNENKIN")
    '*** End Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή��i�ʃN���X�̌ʃ��\�b�h����Q�Ɖ\�Ƃ���j ***

    '2010/10/07.Sakon�@�C���v�b�g�G���[�̔z�M�Ώۉ�
    Public SouInputErr As String = CASTCommon.GetFSKJIni("JIFURI", "SOU_INPUTERR")

    ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
    Private strUketukeDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private strUketukeTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
    ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

    ' �Œ蒷�\���̗p�C���^�[�t�F�[�X
    Protected Friend Interface IFormat
        ' �f�[�^
        Property Data() As String
    End Interface

    ' SHIT-JIS�G���R�[�f�B���O
    Protected Friend Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")
    ' EBCDIC(����)�G���R�[�f�B���O
    Protected Friend Shared EncdE As System.Text.Encoding = System.Text.Encoding.GetEncoding("IBM290")

    ' �g�p����G���R�[�f�B���O
    Protected Friend CurrEncd As System.Text.Encoding

    ' �ǂݏ����X�g���[��
    Protected IOfileStream As FileStream

    ' MEIMAST�pOracleReader
    Protected IOreader As CASTCommon.MyOracleReader

    ' 1�s�f�[�^�ǂݍ��ݗp
    Protected ReadByte() As Byte
    Protected mRecordData As String         ' �����ϊ�����
    Protected mRecordDataNoChange As String ' �����ϊ��O
    Public Property RecordData() As String
        Get
            Return mRecordData
        End Get
        Set(ByVal Value As String)
            '*** �C�� mitsu 2008/06/03 NULL�����u��(���s�������ΊO���I) ***
            mRecordData = Value
            'mRecordData = Value.Replace(Convert.ToChar(0), " ")
            '******************************************************************
            mRecordDataNoChange = Value
        End Set
    End Property
    Public ReadOnly Property RecordDataNoChange() As String
        Get
            Return mRecordDataNoChange
        End Get
    End Property

    '*** �C�� 2008/09/30 EBCDIC�f�[�^�Ή� ***
    '�Ԋҏ����ݗp ���R�[�h���o�C�g�z��ŕԂ�
    Public ReadOnly Property RecordDataToBytes() As Byte()
        Get
            Return EncdJ.GetBytes(mRecordData)
        End Get
    End Property

    '�o�C�i���f�[�^(EBCDIC�f�[�^)�g�p�t���O
    Protected BinDataMode As Boolean = False
    Public ReadOnly Property BinMode() As Boolean
        Get
            Return BinDataMode
        End Get
    End Property

    '�o�C�i���f�[�^(EBCDIC�f�[�^)�i�[�p
    Public ReadByteBin As BinaryString
    Public Property RecordDataBin() As Byte()
        Get
            If ReadByteBin Is Nothing Then
                Return Nothing
            Else
                Return ReadByteBin.Buffer
            End If
        End Get
        Set(ByVal Value As Byte())
            If Not Value Is Nothing Then
                If ReadByteBin Is Nothing Then
                    ReadByteBin = New BinaryString(Value, EncdE)
                Else
                    ReadByteBin.Buffer = Value
                End If
            End If
        End Set
    End Property
    '****************************************

    Friend code_flg As Boolean = False '���Z���^�Ή��|�R�[�h�ϊ��t���O

    ' �e�s�q�`�m�{ �p�����[�^�t�@�C��
    Protected Friend FtranPfile As String
    Protected Friend FtranIBMPfile As String
    Protected Friend FtranIBMBinaryPfile As String

    Protected Friend CMTBlockSize As Integer

    ' �f�[�^���
    Friend Structure DATAINFORMATION
        Dim FileName As String                      ' �t�@�C����
        Dim FileLen As Long                         ' �t�@�C����
        Dim Encoding As EncodingType                ' �G���R�[�f�B���O
        Dim NewLine As NewLineType                  ' ���s�R�[�h
        Dim Message As String                       ' ���
        Dim RecoedLen As Integer                    ' �f�[�^��
        Dim LenOfOneRec As Integer                  ' �P���R�[�h�̒���
        Dim MinRecordCode() As String               ' �Œᕪ�̃��R�[�h�敪
        '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- START
        Dim WorkLen As Integer                      '���ԃt�@�C���̃��R�[�h�T�C�Y
        '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- END
    End Structure
    Friend DataInfo As DATAINFORMATION

    ' �P���R�[�h�O�̃��R�[�h�敪
    Friend BeforeRecKbn As String

    ' �w�b�_���R�[�h�敪
    Protected Friend HeaderKubun() As String
    ' �f�[�^���R�[�h�敪
    Protected Friend DataKubun() As String
    ' �g���[�����R�[�h�敪
    Protected Friend TrailerKubun() As String

    ' �t�@�C�����v���p�e�B
    Public ReadOnly Property FileName() As String
        Get
            Return DataInfo.FileName
        End Get
    End Property

    ' �f�[�^���v���p�e�B
    Public Property RecordLen() As Integer
        Get
            Return DataInfo.RecoedLen
        End Get
        Set(ByVal Value As Integer)
            DataInfo.RecoedLen = Value
        End Set
    End Property

    ' CRLF�v���p�e�B
    Public ReadOnly Property CRLF() As Boolean
        Get
            If DataInfo.NewLine = NewLineType.None Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    ' FTRAN �p�����[�^�t�@�C���v���p�e�B
    Public ReadOnly Property FTRANP() As String
        Get
            Return FtranPfile
        End Get
    End Property

    ' FTRAN IBM �p�����[�^�t�@�C���v���p�e�B
    Public ReadOnly Property FTRANIBMP() As String
        Get
            Return FtranIBMPfile
        End Get
    End Property

    ' FTRAN IBM �p�����[�^�t�@�C���v���p�e�B
    Public ReadOnly Property FTRANIBMBINARYP() As String
        Get
            Return FtranIBMBinaryPfile
        End Get
    End Property

    ' CMT�u���b�N�T�C�Y �p�����[�^�t�@�C���v���p�e�B
    Public ReadOnly Property BLOCKSIZE() As Integer
        Get
            Return CMTBlockSize
        End Get
    End Property

    'Public Shared ErrorNum As Err.InputErrorType    ' �G���[�ԍ�

    ' ���_�m�f
    '*** �C�� mitsu 2009/04/17 &$*�͋K��O���� ***
    Protected Shared ReadOnly RegularStringVerA As String =
                                            "\,.��()-/" &
                                            "0123456789" &
                                            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" &
                                            "��������������������������������������������ݦ��" &
                                            " "
    '*********************************************
    ' ���_�n�j
    Protected Shared ReadOnly RegularStringVerB As String = RegularStringVerA

    ' �Q�̃p�^�[�������ꍇ�Ƀ`�F�b�N����
    '*** �C�� mitsu 2008/09/30 �R���p�C���I�v�V�������g�p���� ***
    'Protected Shared ReadOnly RegularRegexVerA As New Regex("[^" & Regex.Escape(RegularStringVerA) & "]")
    'Protected Shared ReadOnly RegularRegexVerB As New Regex("[^" & Regex.Escape(RegularStringVerB) & "]")
    'Protected Shared ReadOnly RegularRegexVerPlus As New Regex("[\+]")
    Protected Shared ReadOnly RegularRegexVerA As New Regex("[^" & Regex.Escape(RegularStringVerA) & "]", RegexOptions.Compiled)
    Protected Shared ReadOnly RegularRegexVerB As New Regex("[^" & Regex.Escape(RegularStringVerB) & "]", RegexOptions.Compiled)
    Protected Shared ReadOnly RegularRegexVerPlus As New Regex("[\+]", RegexOptions.Compiled)
    '************************************************************

    Private ReplaceStringPattern As Hashtable   '�K��O�����ϊ��p�^�[��

    '���s�R�[�h
    Friend Enum NewLineType As Integer
        None = 0                                ' �Ȃ�
        CrLf                                    ' CRLF
        Cr                                      ' CR
        Lf                                      ' LF
    End Enum

    '�G���R�[�f�B���O
    Friend Enum EncodingType As Integer
        NONE                                    ' �Ȃ�
        SJIS                                    ' SHIFT-JIS
        EBCDIC                                  ' EBCDIC
    End Enum

    Public SitenYomikae As String = CASTCommon.GetFSKJIni("YOMIKAE", "TENPO")  '2010/02/03 �x�X�Ǒ֋敪�@�ǉ�
    Public KouzaYomikae As String = CASTCommon.GetFSKJIni("YOMIKAE", "KOUZA")  '2010/02/03 �����Ǒ֋敪�@�ǉ�

    '�@���׃f�[�^
    Public Structure MEISAI
        ' �敪
        Dim DATA_KBN As String                  ' �f�[�^�敪
        Dim CODE_KBN As String                  ' �R�[�h�敪
        Dim RECORD_NO As Integer                ' ���R�[�h�ԍ�
        ' �w�b�_���
        Dim SYUBETU_CODE As String              ' ��ʃR�[�h
        Dim ITAKU_CODE As String                ' �ϑ��҃R�[�h
        Dim FURIKAE_DATE As String              ' �U�֓�
        Dim FURIKAE_DATE_MOTO As String         ' �U�֓��i�ϊ����f�[�^�j
        Dim KIGYO_SEQ As String                 ' ��Ƃr�d�p
        Dim ITAKU_KIN As String                 ' �ϑ��ҋ��Z�@�փR�[�h
        Dim ITAKU_SIT As String                 ' �ϑ��Ҏx�X�R�[�h
        Dim I_SIT_NNAME As String               ' �ϑ��Ҏx�X��
        Dim ITAKU_KAMOKU As String              ' �ϑ��҉Ȗ�
        Dim ITAKU_KOUZA As String               ' �ϑ��Ҍ����ԍ�
        Dim ITAKU_KNAME As String               ' �ϑ��Җ��J�i
        ' �f�[�^���
        Dim KEIYAKU_KIN As String               ' �_����Z�@�փR�[�h
        Dim KEIYAKU_SIT As String               ' �_����Z�@�֎x�X�R�[�h
        Dim KEIYAKU_NO As String                ' �_��Ҕԍ�
        Dim KEIYAKU_KNAME As String             ' �_��Җ��J�i
        Dim KEIYAKU_KAMOKU As String            ' �_��҉Ȗ�
        Dim KEIYAKU_BsKAMOKU As String          ' �_��҉Ȗ� �ϊ��O
        Dim KEIYAKU_KOUZA As String             ' �_��Ҍ����ԍ�
        Dim FURIKIN As Decimal                  ' �U�֋��z
        Dim FURIKIN_MOTO As String              ' �U�֋��z�i�ϊ����f�[�^�j
        Dim FURIKETU_CODE As Integer            ' �U�֌���
        Dim FURIKETU_MOTO As String             ' �U�֌��ʁi�ϊ����f�[�^�j
        Dim SINKI_CODE As String                ' �V�K�R�[�h
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        Dim NS_KBN As String                    ' ���o���敪
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        Dim KTEKIYO As String                   ' �J�i�E�v
        Dim NTEKIYO As String                   ' �����E�v
        Dim JYUYOKA_NO As String                ' ���v�Ɣԍ�
        Dim TEISEI_SIT As String                ' ������x�X�R�[�h
        Dim TEISEI_KAMOKU As String             ' ������Ȗ�
        Dim TEISEI_KOUZA As String              ' ���������
        Dim YOBI1 As String                     ' �\���P
        Dim YOBI2 As String                     ' �\���Q
        Dim YOBI3 As String                     ' �\���R
        Dim YOBI4 As String                     ' �\���S
        Dim YOBI5 As String                     ' �\���T
        Dim YOBI6 As String                     ' �\���U
        Dim YOBI7 As String                     ' �\���V
        Dim YOBI8 As String                     ' �\���W
        Dim YOBI9 As String                     ' �\���X
        Dim YOBI10 As String                    ' �\���P�O
        ' �g���[�����
        Dim TOTAL_IRAI_KEN As Decimal           ' �g���[���˗����v
        Dim TOTAL_IRAI_KEN_MOTO As String       ' �g���[���˗����v
        Dim TOTAL_IRAI_KIN As Decimal           ' �g���[�����z���v
        Dim TOTAL_IRAI_KIN_MOTO As String       ' �g���[�����z���v
        Dim TOTAL_ZUMI_KEN As Decimal           ' �g���[���ό������v
        Dim TOTAL_ZUMI_KEN_MOTO As String       ' �g���[���ό������v
        Dim TOTAL_ZUMI_KIN As Decimal           ' �g���[���ϋ��z���v
        Dim TOTAL_ZUMI_KIN_MOTO As String       ' �g���[���ϋ��z���v
        Dim TOTAL_FUNO_KEN As Decimal           ' �g���[���s�\�������v
        Dim TOTAL_FUNO_KEN_MOTO As String       ' �g���[���s�\�������v
        Dim TOTAL_FUNO_KIN As Decimal           ' �g���[���s�\���z���v
        Dim TOTAL_FUNO_KIN_MOTO As String       ' �g���[���s�\���z���v

        ' �s�\���ʏ��
        Dim FURI_CODE As String                 ' �U�փR�[�h
        Dim KIGYO_CODE As String                ' ��ƃR�[�h
        Dim FURIKETU_CENTERCODE As String       ' �s�\���R�R�[�h
        Dim MINASI As String                    ' �݂Ȃ�����
        Dim TEISEI_AKAMOKU As String            ' �����㑊��Ȗ�
        Dim TEISEI_AKOUZA As String             ' �����㑊�����

        ' �Ϗ��
        Dim MOTIKOMI_SEQ As Integer             ' �������݂r�d�p ���������R�[�h�E�U�֓����̏���
        Dim SCH_UPDATE_FLG As Boolean           ' �X�P�W���[���A�b�v�f�[�g�t���O
        Dim FILE_SEQ As Integer                 ' �e�h�k�d�r�d�p ����}�̓��̏���

        Dim FURIKEN As Integer                  ' �v�Z�Ώی����i�f�[�^���R�[�h�ǂݍ��ݎ��͂P�C�ȊO�͂O�j
        Dim TOTAL_KEN As Decimal                ' ���v�˗����� �v�Z�l
        '2009/12/03 �ǉ� ============================================================
        Dim TOTAL_KEN2 As Decimal                ' ���v�˗����� �v�Z�l(0�~�f�[�^����)
        '============================================================================
        Dim TOTAL_KIN As Decimal                ' ���v�˗����z �v�Z�l
        Dim TOTAL_IJO_KEN As Decimal            ' ���v�C���v�b�g�G���[�i�s�\�j���� �v�Z�l
        Dim TOTAL_IJO_KIN As Decimal            ' ���v�C���v�b�g�G���[�i�s�\�j���z �v�Z�l
        Dim TOTAL_NORM_KEN As Decimal           ' ���v���팏�� �v�Z�l
        Dim TOTAL_NORM_KIN As Decimal           ' ���v������z �v�Z�l
        Dim TOTAL_NORM_KEN2 As Decimal          ' ���v���팏�� �v�Z�l(0�~�f�[�^����)

        Dim DUPLICATE_KBN As String             ' ��d�����敪

        Dim FURIKETU_KEKKA As String            ' ���׃}�X�^�U�֌���

        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        Dim TimeOver As Boolean                 ' ���ԊO��t�敪
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- START
        Dim KinTenSoui As Boolean               ' ���Z�@�֖��A�x�X������t���O
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- END

        Dim OLD_KIN_NO As String                '2009/09/17�@�Ǒ֑O���Z�@�փR�[�h
        Dim OLD_SIT_NO As String                '2009/09/17�@�Ǒ֑O�x�X�R�[�h
        Dim OLD_KOUZA As String                 '2009/09/17�@�Ǒ֑O�����ԍ�
        Dim IDOU_DATE As String                 '2009/09/19�@�ٓ���
        Dim TESUU_KIN As Integer                '2010/01/22  �萔��

        '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------START
        Dim HENKANKBN As String                 '�Ԋҋ敪
        '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------END
        Public Sub Init()
            SYUBETU_CODE = ""
            ITAKU_CODE = ""
            FURIKAE_DATE = ""
            FURIKAE_DATE_MOTO = ""
            KIGYO_SEQ = "00000000"
            ITAKU_KIN = ""
            ITAKU_SIT = ""
            ITAKU_KAMOKU = ""
            ITAKU_KOUZA = ""
            ITAKU_KNAME = ""
            KEIYAKU_KIN = ""
            KEIYAKU_SIT = ""
            KEIYAKU_NO = ""
            KEIYAKU_KNAME = ""
            KEIYAKU_KAMOKU = ""
            KEIYAKU_KOUZA = ""
            FURIKIN = 0
            FURIKETU_CODE = 0
            FURIKETU_CENTERCODE = "0"           ' 2008.04.22 ADD
            SINKI_CODE = ""
            '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
            NS_KBN = ""             ' ���o���敪
            '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            KTEKIYO = ""
            NTEKIYO = ""
            JYUYOKA_NO = ""
            TEISEI_SIT = ""
            TEISEI_KAMOKU = ""
            TEISEI_KOUZA = ""
            DATA_KBN = ""

            YOBI1 = ""
            YOBI2 = ""
            YOBI3 = ""
            YOBI4 = ""
            YOBI5 = ""
            YOBI6 = ""
            YOBI7 = ""
            YOBI8 = ""
            YOBI9 = ""
            YOBI10 = ""

            TOTAL_IRAI_KEN = 0
            TOTAL_IRAI_KIN = 0
            TOTAL_ZUMI_KEN = 0
            TOTAL_ZUMI_KIN = 0
            TOTAL_FUNO_KEN = 0
            TOTAL_FUNO_KIN = 0

            FURIKEN = 0
            TOTAL_KEN = 0
            TOTAL_KIN = 0
            TOTAL_IJO_KEN = 0
            TOTAL_IJO_KIN = 0
            TOTAL_NORM_KEN = 0
            TOTAL_NORM_KIN = 0

            FURIKETU_KEKKA = ""

            OLD_KIN_NO = ""
            OLD_SIT_NO = ""

            '2009/10/21 �ǉ�
            FURI_CODE = ""
            KIGYO_CODE = ""

            '2010/01/22 �ǉ�
            TESUU_KIN = 0

            '2010.03.02 start �}���`�Ή�
            TOTAL_KEN2 = 0
            TOTAL_NORM_KEN2 = 0
            '2010.03.02 end

            '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------START
            HENKANKBN = ""
            '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------END
            '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
            TimeOver = True                ' ���ԊO��t�敪
            '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
        End Sub

        Public Sub InitData()
            KEIYAKU_KIN = ""
            KEIYAKU_SIT = ""
            KEIYAKU_NO = ""
            KEIYAKU_KNAME = ""
            KEIYAKU_KAMOKU = ""
            KEIYAKU_KOUZA = ""
            FURIKIN = 0
            FURIKETU_CODE = 0
            FURIKETU_CENTERCODE = "0"           ' 2008.04.22 ADD
            SINKI_CODE = ""
            '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
            NS_KBN = ""             ' ���o���敪
            '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            KTEKIYO = ""
            NTEKIYO = ""
            JYUYOKA_NO = ""
            TEISEI_SIT = ""
            TEISEI_KAMOKU = ""
            TEISEI_KOUZA = ""
            DATA_KBN = ""
            YOBI1 = ""
            YOBI2 = ""
            YOBI3 = ""
            YOBI4 = ""
            YOBI5 = ""
            YOBI6 = ""
            YOBI7 = ""
            YOBI8 = ""
            YOBI9 = ""
            YOBI10 = ""

            TOTAL_IRAI_KEN = 0
            TOTAL_IRAI_KIN = 0
            TOTAL_ZUMI_KEN = 0
            TOTAL_ZUMI_KIN = 0
            TOTAL_FUNO_KEN = 0
            TOTAL_FUNO_KIN = 0

            FURIKETU_KEKKA = ""

            OLD_KIN_NO = ""
            OLD_SIT_NO = ""

            '2010/01/22 �ǉ�
            TESUU_KIN = 0
            '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------START
            HENKANKBN = ""
            '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------END
        End Sub
    End Structure
    Public InfoMeisaiMast As MEISAI

    '***�C�� meada 2008/05/12*****************************************************
    '�f�[�^���R�[�h�̋��Z�@�֖����Z�b�g
    '�@���׃f�[�^
    Public Structure MEISAI2
        ' �f�[�^���
        Dim KEIYAKU_KIN_KNAME As String '�_����Z�@�֖�
        Dim KEIYAKU_SIT_KNAME As String '�_��X�ܖ�

        '***�C�� meada 20181001 maeda �W���C��*****************************************************
        Dim TENMAST_SIT_KNAME As String 'TENMAST�X�ܖ�
        '***�C�� meada 20181001 maeda �W���C��*****************************************************

        Public Sub Init()
            KEIYAKU_KIN_KNAME = "" '�_����Z�@�֖�
            KEIYAKU_SIT_KNAME = "" '�_��X�ܖ�
            TENMAST_SIT_KNAME = ""
        End Sub

    End Structure
    Public InfoMeisaiMast2 As MEISAI2
    '************************************************************************

    '�G���[��`
    Public Class Err
        ' 2008.04.08 ADD >> �V�K�R�[�h�ُ�ǉ�
        '***�C�� maeda 2008/05/13***************************************************************
        '�G���[��`�ǉ�
        '���Z�@�փR�[�h�ُ�
        '�X�ܓ��p��
        '�K��O��������
        '*** �C�� mitsu 2008/07/01 �U�֌��ʃR�[�h�ُ�ǉ� ***
        '�U�֌��ʃR�[�h�ُ�
        '****************************************************

        Private Shared ReadOnly InputErrorString() As String = {
                                                "",
                                                "�X�Ԑ��l�ُ�",
                                                "�Ȗڈُ�",
                                                "�����ԍ��ُ�",
                                                "�J�i�����ُ�",
                                                "���z�ُ�",
                                                "���s�X�Ԉُ�",
                                                "���s�ُ�",
                                                "���s�X�Ԉُ�",
                                                "���z0�~",
                                                "�U�֓�",
                                                "��g�O",
                                                "�`�������M",
                                                "CMT�f�[�^�쐬",
                                                "�V�K�R�[�h�ُ�",
                                                "���Z�@�փR�[�h�ُ�",
                                                "�X�ܓ��p��",
                                                "�K��O��������",
                                                "�U�֌��ʃR�[�h�ُ�"
                                                }
        'Private Shared ReadOnly InputErrorString() As String = { _
        '                        "", _
        '                        "�X�Ԑ��l�ُ�", _
        '                        "�Ȗڈُ�", _
        '                        "�����ԍ��ُ�", _
        '                        "�J�i�����ُ�", _
        '                        "���z�ُ�", _
        '                        "���s�X�Ԉُ�", _
        '                        "���s�ُ�", _
        '                        "���s�X�Ԉُ�", _
        '                        "���z0�~", _
        '                        "�U�֓�", _
        '                        "��g�O", _
        '                        "�`�������M", _
        '                        "CMT�f�[�^�쐬", _
        '                        "�V�K�R�[�h�ُ�" _
        '                        }


        '***************************************************************************************
        Public Enum InputErrorType As Integer
            ' 2008.04.08 ADD >> �V�K�R�[�h�ُ�ǉ�
            Tenban = 1
            Kamoku
            Kouza
            Kana
            Kingaku
            JikouTenban
            Takou
            TakouTenban
            KingakuZero
            Furikaebi
            TeikeiGai
            DensoMi
            CmtMeked
            SinkiCode
            '***�C�� maeda 2008/05/13*************************************************************
            GinkouCode
            TenpoTougou
            Kiteigaimoji
            '*************************************************************************************
            '*** �C�� mitsu 2008/07/01 �U�֌��ʃR�[�h�ُ�ǉ� ***
            FuriketuCode
            '****************************************************
        End Enum
        Public Shared Function Name(ByVal num As InputErrorType) As String
            Try
                Return InputErrorString(num)
            Catch ex As Exception
                Return ""
            End Try
        End Function
    End Class

    ' �C���v�b�g�G���[�\����
    Public Structure INPUTERROR
        Dim RECNO As Long
        Dim KIN As String
        Dim SIT As String
        Dim KAMOKU As String
        Dim KOUZA As String
        Dim JYUYOKA_NO As String
        Dim KNAME As String
        Dim FURIKIN As String
        Dim ERRINFO As String
        Public WriteOnly Property DATA() As MEISAI
            Set(ByVal Value As MEISAI)
                RECNO = Value.RECORD_NO
                KIN = Value.KEIYAKU_KIN
                SIT = Value.KEIYAKU_SIT
                KAMOKU = Value.KEIYAKU_KAMOKU
                KOUZA = Value.KEIYAKU_KOUZA
                JYUYOKA_NO = Value.JYUYOKA_NO
                KNAME = Value.KEIYAKU_KNAME
                FURIKIN = Value.FURIKIN_MOTO
                ERRINFO = ""
            End Set
        End Property
    End Structure
    Public InErrorArray As New ArrayList

    ' �G���[�ԍ�
    Protected mnErrorNumber As Long = 0
    Public ReadOnly Property ErrorNumber() As Long
        Get
            Return mnErrorNumber
        End Get
    End Property

    ' �E�v�f�[�^
    Protected mTekiyoData() As String

    ' ORACLE
    Protected Friend OraDB As CASTCommon.MyOracle
    ' LOG
    Protected Friend BLOG As CASTCommon.BatchLOG

    ' �x���e�[�u��
    Public HolidayList As New ArrayList(10)

    ' �e�w�b�_���Ƃ̈˗��f�[�^���
    Protected mInfoComm As CAstBatch.CommData
    Public Property ToriData() As CAstBatch.CommData
        Get
            Return mInfoComm
        End Get
        Set(ByVal Value As CAstBatch.CommData)
            mInfoComm = Value
            'mInfoComm = New CAstBatch.CommData(Value.INFOToriMast, Value.INFOParameter)
        End Set
    End Property

    ' �����ɃR�[�h
    Public ReadOnly JIKINKO As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

    ' �s�\�\�����
    Protected mnJifuriFunou As Long = CASTCommon.GetFSKJIniNum("JIFURI", "FUNOU")
    ' �z�M�\�����
    Protected mnJifuriHaisin As Long = CASTCommon.GetFSKJIniNum("JIFURI", "HAISIN")
    ' ����\�����
    Protected mnJifuriKaisyu As Long = CASTCommon.GetFSKJIniNum("JIFURI", "KAISYU")

    ' �`�F�b�N�f�W�b�g
    Protected mCheckDigitFlag As String = CASTCommon.GetFSKJIni("COMMON", "CHKDJT")

    '2010/09/08.Sakon�@�V�K�R�[�h�`�F�b�N�t���O�ǉ�
    Protected mSinkiCheckFlag As String = CASTCommon.GetFSKJIni("COMMON", "SINKICHECK")

    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
    '��t�����`�F�b�N(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)
    Protected INI_UKETUKE_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_KIJITU_CHK")
    '��t����(���c�Ɠ��O�� )
    Protected INI_UKETUKE_KIJITU As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_KIJITU")
    '���������`�F�b�N(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)
    Protected INI_MOTIKOMI_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MOTIKOMI_KIJITU_CHK")
    '��t���ԃ`�F�b�N(0:�`�F�b�N���Ȃ��A1:�`�F�b�N����)
    Protected INI_UKETUKE_JIKAN_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_JIKAN_CHK")
    '��t����(��t�\���� HHMMSS)
    Protected INI_UKETUKE_JIKAN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_JIKAN")
    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- START
    Protected INI_KINKO_SOUI_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KINKO_SOUI_CHK")
    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- END
    '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i��ʃR�[�h�ǉ�(���^�E�ܗ^�œ���ϑ��҃R�[�h�����݂��邽��)�j----- START
    Protected INI_JIFURI_SYUBETU_KEY As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "JIFURI_SYUBETU_KEY")
    '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i��ʃR�[�h�ǉ�(���^�E�ܗ^�œ���ϑ��҃R�[�h�����݂��邽��)�j----- END
    '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- START
    Protected INI_IRAI_KYUJITU_HOSEI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IRAI_KYUJITU_HOSEI")
    '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- END
    '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�������`�F�b�N�j------------------ START
    Protected INI_S_KDBMAST_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KDBMAST_CHK")
    '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�������`�F�b�N�j------------------ END

    '*** �C�� mitsu 2008/06/24 �w�b�_���J�E���g ***
    Protected HeaderCnt As Integer = 0
    '**********************************************

    Public TAKOU_ON As Boolean                  ' ���s����

    '2018/02/14 saitou �L���M��(RSV2�W��) ADD ���Z�@�֖��u���Ή� ---------------------------------------- START
    Protected ReplaceKinNamePattern As Hashtable
    Protected ReplaceSitNamePattern As Hashtable
    '2018/02/14 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END

    ' New
    Public Sub New()
        With DataInfo
            .Encoding = EncodingType.NONE
            .NewLine = NewLineType.None
            .RecoedLen = 0
            .LenOfOneRec = 0
            .MinRecordCode = New String() {}
            '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- START
            .WorkLen = 0
            '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- END
        End With

        ' �f�t�H���g�̃G���R�[�f�B���O��SHIFT-JIS�ɐݒ肷��
        CurrEncd = EncdJ

        InfoMeisaiMast.MOTIKOMI_SEQ = 0
        InfoMeisaiMast.FILE_SEQ = 0

        InfoMeisaiMast.DUPLICATE_KBN = ""
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- START
        InfoMeisaiMast.KinTenSoui = False
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���Z�@�֖�����Ή��j---------------- END

        FtranPfile = ""
        FtranIBMPfile = ""

        ReplaceStringPattern = New Hashtable

        Try
            Dim sr As New StreamReader(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�K��O�����ϊ��p�^�[��.txt"), EncdJ)
            While sr.Peek > -1
                Dim s() As String = sr.ReadLine().Split(","c)
                ReplaceStringPattern.Add(s(0), s(1))
            End While
            sr.Close()

        Catch ex As Exception
            ReplaceStringPattern = Nothing
            Throw
        End Try

        '2018/02/14 saitou �L���M��(RSV2�W��) ADD ���Z�@�֖��u���Ή� ---------------------------------------- START
        '���Z�@�֖��u���p�^�[���A�x�X���u���p�^�[������荞��
        ReplaceKinNamePattern = New Hashtable
        ReplaceSitNamePattern = New Hashtable
        Dim RepKinNameTxt As StreamReader = Nothing

        Try
            Dim RepKinNameTxtName As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "���Z�@�֖��ϊ��p�^�[��.TXT")
            Dim SectionName As String = String.Empty

            If File.Exists(RepKinNameTxtName) Then
                RepKinNameTxt = New StreamReader(RepKinNameTxtName, EncdJ)
                While RepKinNameTxt.Peek > -1
                    Dim s() As String = RepKinNameTxt.ReadLine().Split("="c)
                    If s.Length = 1 Then
                        If s(0).Trim = "[KIN]" OrElse s(0).Trim = "[SIT]" Then
                            SectionName = s(0)
                        Else
                            SectionName = String.Empty
                        End If

                    ElseIf s.Length = 2 Then
                        Select Case SectionName
                            Case "[KIN]"
                                If ReplaceKinNamePattern.ContainsKey(s(0)) = False Then
                                    ReplaceKinNamePattern.Add(s(0), s(1))
                                End If

                            Case "[SIT]"
                                If ReplaceSitNamePattern.ContainsKey(s(0)) = False Then
                                    ReplaceSitNamePattern.Add(s(0), s(1))
                                End If
                        End Select
                    End If
                End While

                RepKinNameTxt.Close()
                RepKinNameTxt = Nothing

            End If

        Catch ex As Exception
            Throw
        Finally
            If Not RepKinNameTxt Is Nothing Then
                RepKinNameTxt.Close()
                RepKinNameTxt = Nothing
            End If
        End Try
        '2018/02/14 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------------- END

    End Sub

    ' New
    Public Sub New(ByVal filename As String)
        MyClass.New()

        DataInfo.FileName = filename
    End Sub

    ' New
    Public Sub New(ByVal filename As String, ByVal len As Integer)
        MyClass.New()

        DataInfo.FileName = filename
        DataInfo.RecoedLen = len
        '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- START
        DataInfo.WorkLen = 120
        '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- END
    End Sub

    Public Sub Dispose() Implements System.IDisposable.Dispose
        Try
            If Not IOfileStream Is Nothing Then
                IOfileStream.Close()
            End If
            IOfileStream = Nothing
        Catch ex As Exception

        End Try
    End Sub

#Region "�I���N��"
    Public WriteOnly Property Oracle() As CASTCommon.MyOracle
        Set(ByVal Value As CASTCommon.MyOracle)
            OraDB = Value

            ' �x���}�X�^��z��ɓǂݍ���
            HolidayList.Clear()
            Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
            Dim SQL As New StringBuilder("SELECT YASUMI_DATE_Y,YASUMI_NAME_Y FROM YASUMIMAST ORDER BY YASUMI_DATE_Y")
            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF = True
                    HolidayList.Add(OraReader.GetValue(0).ToString)

                    OraReader.NextRead()
                Loop
            End If
            OraReader.Close()
        End Set
    End Property
#End Region

#Region "���O"
    Public WriteOnly Property LOG() As CASTCommon.BatchLOG
        Set(ByVal Value As CASTCommon.BatchLOG)
            BLOG = Value
        End Set
    End Property
#End Region

    '
    ' �@�\�@ �F �t�H�[�}�b�g�敪����C�e�t�H�[�}�b�g�N���X���擾����
    '
    ' ����   �F ARG1 - �N���p�����[�^���
    '
    ' �߂�l �F �e�t�H�[�}�b�g�N���X
    '
    ' ���l�@ �F
    '
    Public Shared Shadows Function GetFormat(ByVal para As CAstBatch.CommData.stPARAMETER) As CFormat
        Try
            Select Case para.FMT_KBN
                Case "00", "20", "21", "04", "05" '�˗�����`�[�ǉ�
                    If para.FSYORI_KBN = "3" Then
                        '�S��t�H�[�}�b�g�i�U���j
                        '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
                        '���s�Ȃ��̃��R�[�h����ݒ�ł���悤�ɂ���
                        Dim fmt As CFormat
                        Select Case para.CODE_KBN
                            Case "2"
                                fmt = New CAstFormat.CFormatFurikomi(119)
                            Case "3"
                                fmt = New CAstFormat.CFormatFurikomi(118)
                            Case Else
                                fmt = New CAstFormat.CFormatFurikomi
                        End Select
                        Return fmt
                        'Return New CAstFormat.CFormatFurikomi
                        '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END
                    Else
                        '�S��t�H�[�}�b�g�i���U�j,�W����s�T�[�r�X
                        '*** Str Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***
                        'Return New CAstFormat.CFormatZengin
                        '2017/05/24 �^�X�N�j���� CHG �W���ŏC���iJIS118,119���Ή��j-------------------------- START
                        '���s�Ȃ��̃��R�[�h����ݒ�ł���悤�ɂ���
                        Dim fmt As CFormat
                        Select Case para.CODE_KBN
                            Case "2"
                                fmt = New CAstFormat.CFormatZengin(119)
                            Case "3"
                                fmt = New CAstFormat.CFormatZengin(118)
                            Case Else
                                fmt = New CAstFormat.CFormatZengin
                        End Select
                        'Dim fmt As CFormat = New CAstFormat.CFormatZengin
                        '2017/05/24 �^�X�N�j���� CHG �W���ŏC���iJIS118,119���Ή��j-------------------------- END
                        fmt.setFmtKubun(para.FMT_KBN)

                        Return fmt
                        '*** End Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***
                    End If
                Case "01"
                    '***** 2009/10/02 kakiwaki *******
                    '       '�n���̃t�H�[�}�b�g350
                    '       Return New CAstFormat.CFormatZeikin350
                    'NHK�t�H�[�}�b�g�ɕύX
                    Return New CAstFormat.CFormatNHK
                    '**************** 2009/10/02 *****
                Case "02"
                    '���Ńt�H�[�}�b�g
                    Return New CAstFormat.CFormatKokuzei
                Case "03"
                    '�N���U���t�H�[�}�b�g
                    Return New CAstFormat.CFormatNenkin
                Case "06"
                    '***** 2009/09/30 kakiwaki *******
                    '       '300�o�C�g�t�H�[�}�b�g(����s��)
                    '       Return New CAstFormat.CFormatZeikin300
                    '**************** 2009/09/30 *****
                Case "TO"
                    ' �Z���^���ڎ�������
                    Return New CAstFormat.CFormatTokCenter
                Case "MT"
                    ' ���U�s�\���ɕԊ҂l�s
                    Select Case CASTCommon.GetFSKJIni("COMMON", "CENTER")
                        Case "0" '2010/12/24 �M�g�Ή� SKC�s�\�t�H�[�}�b�g�ǉ�
                            Return New CAstFormat.CFormatSKCFunou
                        Case "1" '�k�C���Z���^�[

                        Case "2", "3", "5", "6"
                            Return New CAstFormat.FUNOU_164_DATA
                        Case "4" '���C�Z���^�[
                            Return New CAstFormat.CFormatTokFunou
                        Case "7"

                        Case Else
                            Return Nothing
                    End Select

                Case "SC"
                    ' �r�r�b����
                    Return New CAstFormat.ClsFormatSSCKekka
                Case Else
                    '*** Str Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
                    'Return Nothing
                    If IsNumeric(para.FMT_KBN) Then
                        Dim nFmtKbn As Integer = CInt(para.FMT_KBN)
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            '*** Str Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***
                            'Return New CAstFormat.CFormatXML(para.FMT_KBN)
                            Dim fmt As CFormat = New CAstFormat.CFormatXML(para.FMT_KBN)
                            fmt.setFmtKubun(para.FMT_KBN)
                            Return fmt
                            '*** End Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***
                        Else
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                    '*** End Upd 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

            End Select
        Catch ex As Exception
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'Return Nothing
            Throw ex
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***

        Finally
        End Try

        Return Nothing
    End Function

    '
    ' �@�\�@ �F �}�X�^�������ǂݎ��i�Q�d�ǂݍ��ݗp�j
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Public Overridable Function FirstReadMasterData() As Integer
        Dim SQL As New StringBuilder(128)

        SQL.Append("SELECT ")
        SQL.Append(" FURI_DATA_K")
        SQL.Append(",MOTIKOMI_SEQ_S")
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        SQL.Append(",MOTIKOMI_DATE_S")  '��������
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
        If mInfoComm.INFOParameter.FSYORI_KBN = "1" Then
            SQL.Append(" FROM MEIMAST")
            SQL.Append("   ,  SCHMAST")
        Else
            SQL.Append(" FROM S_MEIMAST")
            SQL.Append("    , S_SCHMAST")
        End If
        SQL.Append(" WHERE FSYORI_KBN_S   = FSYORI_KBN_K")
        SQL.Append("   AND TORIS_CODE_S   = TORIS_CODE_K")
        SQL.Append("   AND TORIF_CODE_S   = TORIF_CODE_K")
        SQL.Append("   AND FURI_DATE_S    = FURI_DATE_K")
        If mInfoComm.INFOParameter.FSYORI_KBN <> "1" Then
            SQL.Append("   AND MOTIKOMI_SEQ_S = CYCLE_NO_K")
        End If
        SQL.Append("   AND FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append("   AND ERROR_INF_S    = '020'")
        SQL.Append("   AND MOTIKOMI_SEQ_S = ")
        SQL.Append("(SELECT MOTIKOMI_SEQ_S FROM")
        If mInfoComm.INFOParameter.FSYORI_KBN = "1" Then
            SQL.Append(" SCHMAST")
        Else
            SQL.Append(" S_SCHMAST")
        End If
        SQL.Append(" WHERE FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append("   AND ERROR_INF_S    = '020'")
        SQL.Append("   AND ROWNUM <= 1")
        SQL.Append(")")
        SQL.Append(" ORDER BY RECORD_NO_K")
        If Not IOreader Is Nothing Then
            IOreader.Close()
        End If

        IOreader = New CASTCommon.MyOracleReader(OraDB)
        If IOreader.DataReader(SQL) = False Then
            DataInfo.Message = "�X�P�W���[���Ȃ� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & CASTCommon.ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "yyyy�NMM��dd��") & " �ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T
            Return 0
        End If

        ' �X�V�p�Ɏ������݂r�d�p��ۑ�����
        InfoMeisaiMast.MOTIKOMI_SEQ = IOreader.GetInt("MOTIKOMI_SEQ_S")
        InfoMeisaiMast.FURIKETU_CODE = IOreader.GetInt("FURIKETU_CODE_K")

        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        MOTIKOMI_DATE = IOreader.GetString("MOTIKOMI_DATE_S")   '��������
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

        Return 1
    End Function

    '
    ' �@�\�@ �F �}�X�^�������ǂݎ��i�Ԋҗp�j
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Public Overridable Function FirstReadMasterDataHenkan() As Integer
        '*** �C�� mitsu 2008/09/30 ���������� ***
        'Dim SQL As New StringBuilder(128)
        Dim SQL As New StringBuilder(512)
        '****************************************
        SQL.Append("SELECT ")
        SQL.Append(" FURI_DATA_K")
        SQL.Append(",MOTIKOMI_SEQ_S")
        SQL.Append(",FURIKETU_CODE_K")
        SQL.Append(",TEISEI_SIT_K")
        SQL.Append(",TEISEI_KAMOKU_K")
        SQL.Append(",TEISEI_KOUZA_K")
        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
        '�o�C�i���f�[�^���擾����
        SQL.Append(",BIN_DATA_K")
        '**********************************************
        SQL.Append(" FROM MEIMAST")
        SQL.Append("   ,  SCHMAST")
        SQL.Append(" WHERE FSYORI_KBN_S   = FSYORI_KBN_K")
        SQL.Append("   AND TORIS_CODE_S   = TORIS_CODE_K")
        SQL.Append("   AND TORIF_CODE_S   = TORIF_CODE_K")
        SQL.Append("   AND FURI_DATE_S    = FURI_DATE_K")
        SQL.Append("   AND FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        '*** �C�� nishida 2008/10/29 �s����� �R�����g�� ***
        'If mInfoComm.INFOToriMast.KESSAI_KYU_CODE_T = "1" OrElse mInfoComm.INFOToriMast.KESSAI_KYU_CODE_T = "3" Then
        '    ' �s�\���ׂ̂ݒ��o
        '    SQL.Append(" AND FURIKETU_CODE_K <> 0")
        'End If
        '**********************************************
        SQL.Append(" ORDER BY RECORD_NO_K")
        If Not IOreader Is Nothing Then
            IOreader.Close()
        End If

        IOreader = New CASTCommon.MyOracleReader(OraDB)
        If IOreader.DataReader(SQL) = False Then
            DataInfo.Message = "�X�P�W���[���Ȃ� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & CASTCommon.ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mInfoComm.INFOParameter.TORIS_CODE & "-" & mInfoComm.INFOParameter.TORIF_CODE
            Return 0
        End If

        ' �X�V�p�Ɏ������݂r�d�p��ۑ�����
        InfoMeisaiMast.MOTIKOMI_SEQ = IOreader.GetInt("MOTIKOMI_SEQ_S")
        InfoMeisaiMast.FURIKETU_CODE = IOreader.GetInt("FURIKETU_CODE_K")

        Return 1
    End Function

    '
    ' �@�\�@ �F �t�@�C����������擾����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Public Overridable Function FirstRead() As Integer

        '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
        'If Not mInfoComm Is Nothing AndAlso _
        '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
        '    ' �����̏ꍇ�́C���̂܂܃��^�[������
        '    If Not IOreader Is Nothing Then
        '        IOreader.Close()
        '        IOreader = Nothing
        '    End If
        '    Return 1
        'End If
        ''If Not mInfoComm Is Nothing Then
        ''    If Not IOreader Is Nothing Then
        ''        IOreader.Close()
        ''        IOreader = Nothing
        ''    End If
        ''    Return 1
        ''End If
        '+++++++++++++++++++++++++++++++++++++++++++++++

        If DataInfo.FileName Is Nothing Then
            DataInfo.Message = "�t�@�C�����w�肳��Ă��܂���"
            Return 0
        Else
            Return FirstRead(DataInfo.FileName)
        End If
    End Function
    '
    ' �@�\�@ �F �}�X�^�E�t�@�C����������擾����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Public Overridable Function FirstReadMast() As Integer


        If Not mInfoComm Is Nothing Then
            If Not IOreader Is Nothing Then
                IOreader.Close()
                IOreader = Nothing
            End If
            Return 1
        End If

        If DataInfo.FileName Is Nothing Then
            DataInfo.Message = "�t�@�C�����w�肳��Ă��܂���"
            Return 0
        Else
            Return FirstRead(DataInfo.FileName)
        End If
    End Function
    '
    ' �@�\�@ �F �}�X�^�������ǂݎ��i�ĐU�p�j
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Public Overridable Function FirstReadMasterDataSaifuri() As Integer
        Dim SQL As New StringBuilder(512)
        SQL.Append("SELECT ")
        SQL.Append(" FURI_DATA_K")
        SQL.Append(",MOTIKOMI_SEQ_S")
        SQL.Append(",FURIKETU_CODE_K")
        SQL.Append(",TEISEI_SIT_K")
        SQL.Append(",TEISEI_KAMOKU_K")
        SQL.Append(",TEISEI_KOUZA_K")
        '�o�C�i���f�[�^���擾����
        SQL.Append(",BIN_DATA_K")
        SQL.Append(" FROM MEIMAST")
        SQL.Append("   ,  SCHMAST")
        SQL.Append(" WHERE FSYORI_KBN_S   = FSYORI_KBN_K")
        SQL.Append("   AND TORIS_CODE_S   = TORIS_CODE_K")
        SQL.Append("   AND TORIF_CODE_S   = TORIF_CODE_K")
        SQL.Append("   AND FURI_DATE_S    = FURI_DATE_K")
        SQL.Append("   AND FSYORI_KBN_S   =" & SQ(mInfoComm.INFOParameter.FSYORI_KBN))
        SQL.Append("   AND TORIS_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIS_CODE))
        SQL.Append("   AND TORIF_CODE_S   =" & SQ(mInfoComm.INFOParameter.TORIF_CODE))
        SQL.Append("   AND FURI_DATE_S    =" & SQ(mInfoComm.INFOParameter.FURI_DATE))
        '    ' �s�\���ׂ̂ݒ��o
        '    SQL.Append(" AND FURIKETU_CODE_K <> 0")
        SQL.Append(" ORDER BY RECORD_NO_K")
        If Not IOreader Is Nothing Then
            IOreader.Close()
        End If

        IOreader = New CASTCommon.MyOracleReader(OraDB)
        If IOreader.DataReader(SQL) = False Then
            DataInfo.Message = "�X�P�W���[���Ȃ� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & CASTCommon.ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mInfoComm.INFOParameter.TORIS_CODE & "-" & mInfoComm.INFOParameter.TORIF_CODE
            Return 0
        End If

        ' �X�V�p�Ɏ������݂r�d�p��ۑ�����
        InfoMeisaiMast.MOTIKOMI_SEQ = IOreader.GetInt("MOTIKOMI_SEQ_S")
        InfoMeisaiMast.FURIKETU_CODE = IOreader.GetInt("FURIKETU_CODE_K")

        Return 1
    End Function

    '
    ' �@�\�@ �F �t�@�C����������擾����(�Ԋ҃f�[�^�쐬)
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Public Overridable Function FirstRead_Henkan() As Integer

        If Not mInfoComm Is Nothing Then
            If Not IOreader Is Nothing Then
                IOreader.Close()
                IOreader = Nothing
            End If
            Return 1
        End If

        If DataInfo.FileName Is Nothing Then
            DataInfo.Message = "�t�@�C�����w�肳��Ă��܂���"
            Return 0
        Else
            Return FirstRead(DataInfo.FileName)
        End If
    End Function

    ' �@�\�@ �F �t�@�C����������擾����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F ���L���[�h
    '
    Public Overridable Function FirstRead(ByVal filename As String) As Integer
        Return FirstReadMode(filename, FileShare.ReadWrite)
    End Function

    ' �@�\�@ �F �t�@�C����������擾����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F �r�����[�h
    '
    Public Overridable Function FirstReadShare(ByVal filename As String) As Integer
        Return FirstReadMode(filename, FileShare.None)
    End Function

    ' �@�\�@ �F �t�@�C����������擾����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Protected Friend Function FirstReadMode(ByVal filename As String, ByVal share As System.IO.FileShare) As Integer

        ' �t�@�C����񏉊���
        With DataInfo
            .FileName = filename
            .FileLen = -1
            .Encoding = EncodingType.NONE
            .NewLine = NewLineType.None
        End With

        InfoMeisaiMast.FILE_SEQ = 0

        Call Close()

        Try
            ' �t�@�C�����擾
            Dim InfileInfo As New FileInfo(filename)
            If InfileInfo.Exists() = False Then
                Throw New FileNotFoundException
            End If
            DataInfo.FileLen = InfileInfo.Length
        Catch ex As UnauthorizedAccessException
            DataInfo.Message = String.Format("�t�@�C���ɃA�N�Z�X�ł��܂���")
            Return 0
        Catch ex As FileNotFoundException
            DataInfo.Message = String.Format("�t�@�C����������܂���")
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Try
            ' �t�@�C���I�[�v��
            IOfileStream = New FileStream(filename, FileMode.Open, FileAccess.Read, share)
        Catch ex As FileNotFoundException
            DataInfo.Message = String.Format("�t�@�C����������܂���")
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Dim TwoByte(1) As Byte

        ' �f�[�^�ǂݎ��
        Try
            '���s�R�[�h�̃`�F�b�N
            DataInfo.LenOfOneRec = DataInfo.RecoedLen

            '*** �C�� mitsu 2008/12/05 ���C�Z���^�[�s�\�t�H�[�}�b�g�̓`�F�b�N���Ȃ� ***
            'If IOfileStream.Seek(DataInfo.RecoedLen, SeekOrigin.Begin) = DataInfo.RecoedLen Then
            If Not (DataInfo.RecoedLen = 237 OrElse DataInfo.RecoedLen = 164) AndAlso IOfileStream.Seek(DataInfo.RecoedLen, SeekOrigin.Begin) = DataInfo.RecoedLen Then
                '**********************************************************************
                If IOfileStream.Read(TwoByte, 0, 2) = 0 Then
                    Throw New System.Exception("�t�@�C���`��������������܂���B")
                End If

                Dim NewLineLen As Integer = 0   '�@���s�R�[�h�̒���
                '*** �C�� mitsu 2008/09/30 ���������� ***
                'Select Case String.Format("{0:X2}", TwoByte(0))
                '    Case "0D"
                '        Select Case String.Format("{0:X2}", TwoByte(1))
                '            Case "0A", "25"
                Select Case TwoByte(0)
                    Case &HD  '0D
                        Select Case TwoByte(1)
                            Case &HA, &H25 '0A, 25
                                '************************
                                DataInfo.NewLine = NewLineType.CrLf
                                NewLineLen = 2
                            Case Else
                                DataInfo.NewLine = NewLineType.Cr
                                NewLineLen = 1
                        End Select
                        '*** �C�� mitsu 2008/09/30 ���������� ***
                        'Case "0A"
                    Case &HA  '0A
                        '****************************************
                        DataInfo.NewLine = NewLineType.Lf
                        NewLineLen = 1
                        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
                    Case &HF1 'F1
                        '���s�R�[�h�̈ʒu�ɃR�[�hF1������ꍇ��EBCDIC�f�[�^�t���O�𗧂Ă�
                        DataInfo.NewLine = NewLineType.None
                        BinDataMode = True
                        '**********************************************
                    Case Else
                        DataInfo.NewLine = NewLineType.None
                End Select
                DataInfo.LenOfOneRec += NewLineLen
            End If

            ' �P���R�[�h�p�̃o�C�g�z���p��
            '*** �C�� mitsu 2008/09/30 ���������� ***
            'ReadByte = CType(Array.CreateInstance(GetType(Byte), DataInfo.LenOfOneRec), Byte())
            ReadByte = New Byte(DataInfo.LenOfOneRec - 1) {}
            '****************************************

            BeforeRecKbn = ""

            ' �Œ���̃��R�[�h�ȉ��̃o�C�g���̏ꍇ�́C�G���[
            If DataInfo.FileLen < (DataInfo.RecoedLen * DataInfo.MinRecordCode.Length) Then
                '*** �C�� mitsu 2008/12/05 ���C�Z���^�[�s�\�t�H�[�}�b�g��1���R�[�h165�o�C�g ***
                'Throw New System.Exception("�t�@�C���`��������������܂���B")
                If Not DataInfo.RecoedLen = 225 OrElse DataInfo.FileLen < 165 Then
                    Throw New System.Exception("�t�@�C���`��������������܂���B")
                End If
                '******************************************************************************
            End If

            IOfileStream.Seek(0, SeekOrigin.Begin)
            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                Throw New System.Exception("�t�@�C���`��������������܂���B")
            End If

            ' �擪���C�w�b�_���R�[�h�ł͂Ȃ��ꍇ�́C�G���[
            If DataInfo.MinRecordCode.Length > 0 Then
                Select Case TwoByte(0)
                    Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                        ' SHIFT-JIS ���ǂ����𔻒�
                        DataInfo.Encoding = EncodingType.SJIS
                        CurrEncd = EncdJ
                        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
                        If BinDataMode Then
                            '���R�[�h�T�C�Y * 2�̗̈���m�ۂ���
                            ReadByte = New Byte(DataInfo.LenOfOneRec * 2 - 1) {}
                            'EBCDIC�f�[�^�i�[�p�z���p�ӂ���
                            RecordDataBin = New Byte(DataInfo.LenOfOneRec - 1) {}
                        End If
                        '**********************************************

                    Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                        ' EBCDIC ���ǂ����𔻒�
                        DataInfo.Encoding = EncodingType.EBCDIC
                        CurrEncd = EncdE
                    Case Else
                        If DataInfo.RecoedLen = 640 Then
                            ' �Z���^�[���ڎ������݃t�H�[�}�b�g�Ή�
                            IOfileStream.Seek(88, SeekOrigin.Begin)
                            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                                Throw New System.Exception("�t�@�C���`��������������܂���B")
                            End If
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' SHIFT-JIS ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' EBCDIC ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("�t�@�C���`��������������܂���B")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 180 Then
                            ' ���񂫂�t�H�[�}�b�g�i��Ɨp�j�Ή�
                            IOfileStream.Seek(59, SeekOrigin.Begin)
                            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                                Throw New System.Exception("�t�@�C���`��������������܂���B")
                            End If
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' SHIFT-JIS ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' EBCDIC ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("�t�@�C���`��������������܂���B")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 120 Then
                            ' ���񂫂�t�H�[�}�b�g�Ή�
                            IOfileStream.Seek(7, SeekOrigin.Begin)
                            If IOfileStream.Read(TwoByte, 0, 1) = 0 Then
                                Throw New System.Exception("�t�@�C���`��������������܂���B")
                            End If
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' SHIFT-JIS ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case EncdE.GetBytes(DataInfo.MinRecordCode(0))(0)
                                    ' EBCDIC ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("�t�@�C���`��������������܂���B")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 237 Then
                            ' �s�\�l�s(���C�Z���^�[)
                            '�w�b�_�̂Ȃ��s�\�t�@�C����"2"
                            Select Case TwoByte(0)
                                Case EncdJ.GetBytes("2")(0)
                                    ' SHIFT-JIS ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.SJIS
                                    CurrEncd = EncdJ
                                Case 2
                                    ' EBCDIC ���ǂ����𔻒�
                                    DataInfo.Encoding = EncodingType.EBCDIC
                                    CurrEncd = EncdE
                                Case Else
                                    Throw New System.Exception("�t�@�C���`��������������܂���B")
                                    DataInfo.Encoding = EncodingType.NONE
                            End Select
                        ElseIf DataInfo.RecoedLen = 164 Then
                            ' �s�\�l�s(���k�E�����E���E�����Z���^�[)
                            If code_flg = True Then
                                ' SHIFT-JIS ���ǂ����𔻒�
                                DataInfo.Encoding = EncodingType.SJIS
                                CurrEncd = EncdJ
                            Else
                                ' EBCDIC ���ǂ����𔻒�
                                DataInfo.Encoding = EncodingType.EBCDIC
                                CurrEncd = EncdE
                                code_flg = True
                            End If
                        ElseIf DataInfo.RecoedLen = 580 Then
                            ' �z�M���[�N�t�@�C��
                        Else
                            Throw New System.Exception("�t�@�C���`��������������܂���B")
                            DataInfo.Encoding = EncodingType.NONE
                        End If
                End Select
            End If

            ' �ŏ��Ɉʒu�Â�
            IOfileStream.Seek(0, SeekOrigin.Begin)
        Catch ex As IO.DirectoryNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As IO.FileNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As IO.IOException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
        End Try

        Return 1
    End Function

    '
    ' �@�\�@ �F �t�@�C����������擾����
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F
    '
    Public Overridable Function OpenWriteFile(ByVal filename As String) As Integer
        Try
            ' �������݃t�@�C���I�[�v��
            If File.Exists(filename) = True Then
                File.Delete(filename)
            End If
            IOfileStream = New FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)
        Catch ex As ArgumentNullException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As ArgumentOutOfRangeException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As ArgumentException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As FileNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As DirectoryNotFoundException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As PathTooLongException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As IOException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As System.Security.SecurityException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As UnauthorizedAccessException
            DataInfo.Message = ex.Message
            Return 0
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Return 1
    End Function

    '
    ' �@�\�@ �F �P���R�[�h�ǂݍ���
    '
    ' �����@ �F ARG1 - ������i�Q�Ɠn���j
    '
    ' �߂�l �F �ǂݍ��񂾃��R�[�h��
    '
    ' ���l�@ �F
    '
    Public Overridable Function GetFileData() As Integer
        Return MyClass.GetFileData(RecordData)
    End Function

    '
    ' �@�\�@ �F �P���R�[�h�ǂݍ��݁i�Ԋҗp�j
    '
    ' �����@ �F ARG1 - ������i�Q�Ɠn���j
    '
    ' �߂�l �F �ǂݍ��񂾃��R�[�h��
    '
    ' ���l�@ �F
    '
    Public Overridable Function GetFileDataHenkan() As Integer
        Dim len As Integer = 0          ' ���R�[�h��

        Try
            '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
            'If Not mInfoComm Is Nothing AndAlso _
            '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
            ''If Not mInfoComm Is Nothing Then


            ''    If IOreader Is Nothing Then
            ''        If FirstReadMasterDataHenkan() = 0 Then
            ''            mRecordData = New String(" "c, RecordLen)
            ''            InfoMeisaiMast.FURIKETU_KEKKA = ""
            ''            Return 0
            ''        End If
            ''    Else
            ''        If IOreader.NextRead() = False Then
            ''            mRecordData = New String(" "c, RecordLen)
            ''            InfoMeisaiMast.FURIKETU_KEKKA = ""
            ''            Return 0
            ''        End If
            ''    End If
            ''    mRecordData = IOreader.GetString("FURI_DATA_K", False)
            ''    InfoMeisaiMast.FURIKETU_KEKKA = IOreader.GetInt("FURIKETU_CODE_K").ToString
            ''    InfoMeisaiMast.TEISEI_SIT = IOreader.GetString("TEISEI_SIT_K")
            ''    InfoMeisaiMast.TEISEI_KAMOKU = IOreader.GetString("TEISEI_KAMOKU_K")
            ''    InfoMeisaiMast.TEISEI_KOUZA = IOreader.GetString("TEISEI_KOUZA_K")
            ''    '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
            ''    '�o�C�i���f�[�^���擾
            ''    RecordDataBin = IOreader.GetBytes("BIN_DATA_K")
            ''    '**********************************************
            ''    Return mRecordData.Length
            ''End If
            '++++++++++++++++++++++++++++++++++++++++++++++++

            '***** 2009/09/20 kakiwaki *******
            '���A�g�敪�̏������ȊO��߂�
            If Not mInfoComm Is Nothing Then


                If IOreader Is Nothing Then
                    If FirstReadMasterDataHenkan() = 0 Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                Else
                    If IOreader.NextRead() = False Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                End If
                mRecordData = IOreader.GetString("FURI_DATA_K", False)
                InfoMeisaiMast.FURIKETU_KEKKA = IOreader.GetInt("FURIKETU_CODE_K").ToString
                InfoMeisaiMast.TEISEI_SIT = IOreader.GetString("TEISEI_SIT_K")
                InfoMeisaiMast.TEISEI_KAMOKU = IOreader.GetString("TEISEI_KAMOKU_K")
                InfoMeisaiMast.TEISEI_KOUZA = IOreader.GetString("TEISEI_KOUZA_K")
                '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
                '�o�C�i���f�[�^���擾
                RecordDataBin = IOreader.GetBytes("BIN_DATA_K")
                '**********************************************
                Return mRecordData.Length
            End If
            '**************** 2009/09/20 *****

            ' �P�s��ǂݍ��݁i���s�f�[�^���܂ށj
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' �Ǎ��f�[�^�iBYTE)�𕶎���ɕϊ�
            mRecordData = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = mRecordData
        End Try

        Return len
    End Function
    '
    ' �@�\�@ �F �P���R�[�h�ǂݍ��݁i�s�\�p�j
    '
    ' �����@ �F ARG1 - ������i�Q�Ɠn���j
    '
    ' �߂�l �F �ǂݍ��񂾃��R�[�h��
    '
    ' ���l�@ �F
    '
    Public Overridable Function GetFileDataFunou() As Integer
        Dim len As Integer = 0          ' ���R�[�h��

        Try
            ' �P�s��ǂݍ��݁i���s�f�[�^���܂ށj
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' �Ǎ��f�[�^�iBYTE)�𕶎���ɕϊ�
            mRecordData = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = mRecordData
        End Try

        Return len
    End Function

    '
    ' �@�\�@ �F �P���R�[�h�ǂݍ��݁i�ĐU�p�j
    '
    ' �����@ �F ARG1 - ������i�Q�Ɠn���j
    '
    ' �߂�l �F �ǂݍ��񂾃��R�[�h��
    '
    ' ���l�@ �F
    '
    Public Overridable Function GetFileDataSaifuri() As Integer
        Dim len As Integer = 0          ' ���R�[�h��

        Try
            If Not mInfoComm Is Nothing Then


                If IOreader Is Nothing Then
                    If FirstReadMasterDataSaifuri() = 0 Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                Else
                    If IOreader.NextRead() = False Then
                        mRecordData = New String(" "c, RecordLen)
                        InfoMeisaiMast.FURIKETU_KEKKA = ""
                        Return 0
                    End If
                End If
                mRecordData = IOreader.GetString("FURI_DATA_K", False)
                InfoMeisaiMast.FURIKETU_KEKKA = IOreader.GetInt("FURIKETU_CODE_K").ToString
                InfoMeisaiMast.TEISEI_SIT = IOreader.GetString("TEISEI_SIT_K")
                InfoMeisaiMast.TEISEI_KAMOKU = IOreader.GetString("TEISEI_KAMOKU_K")
                InfoMeisaiMast.TEISEI_KOUZA = IOreader.GetString("TEISEI_KOUZA_K")
                RecordDataBin = IOreader.GetBytes("BIN_DATA_K")
                '**********************************************
                Return mRecordData.Length
            End If

            ' �P�s��ǂݍ��݁i���s�f�[�^���܂ށj
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' �Ǎ��f�[�^�iBYTE)�𕶎���ɕϊ�
            mRecordData = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = mRecordData
        End Try

        Return len
    End Function
    '
    ' �@�\�@ �F �P���R�[�h�ǂݍ��݁i�������ݎ��Ɏg�p����j
    '
    ' �����@ �F ARG1 - ������i�Q�Ɠn���j
    '
    ' �߂�l �F �ǂݍ��񂾃��R�[�h��
    '
    ' ���l�@ �F
    '
    Public Overridable Function GetFileData(ByRef data As String, ByVal seekPos As Integer) As Integer
        Dim len As Integer = 0          ' ���R�[�h��

        Try
            IOfileStream.Seek((seekPos - 1) * DataInfo.LenOfOneRec, SeekOrigin.Begin)

            ' �P�s��ǂݍ��݁i���s�f�[�^���܂ށj
            len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)

            ' �Ǎ��f�[�^�iBYTE)�𕶎���ɕϊ�
            data = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = data
        End Try

        Return len
    End Function

    '
    ' �@�\�@ �F �P���R�[�h�ǂݍ��݁i�������ݎ��Ɏg�p����j
    '
    ' �����@ �F ARG1 - ������i�Q�Ɠn���j
    '
    ' �߂�l �F �ǂݍ��񂾃��R�[�h��
    '
    ' ���l�@ �F
    '
    Public Overridable Function GetFileData(ByRef data As String) As Integer
        Dim len As Integer = 0          ' ���R�[�h��

        Try
            '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
            'If Not mInfoComm Is Nothing AndAlso _
            '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
            ''If Not mInfoComm Is Nothing Then
            '    If IOreader Is Nothing Then
            '        ' �d���f�[�^�݂̂𒊏o
            '        If FirstReadMasterData() = 0 Then
            '            data = New String(" "c, RecordLen)
            '            Return 0
            '        End If
            '    Else
            '        If IOreader.NextRead() = False Then
            '            data = New String(" "c, RecordLen)
            '            Return 0
            '        End If
            '    End If
            '    data = IOreader.GetString("FURI_DATA_K", False)
            '    Return data.Length
            'End If
            '+++++++++++++++++++++++++++++++++++++++++++++++

            ' �P�s��ǂݍ��݁i���s�f�[�^���܂ށj
            '*** 2008/09/30 EBCDIC�f�[�^�Ή� ***
            'len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)
            If BinDataMode = False Then
                len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec)
            Else
                'EBCDIC�f�[�^�̏ꍇ�͔{���R�[�h�����擾����
                len = IOfileStream.Read(ReadByte, 0, DataInfo.LenOfOneRec * 2)
                '���R�[�h����{���̃T�C�Y�ɖ߂�
                len = Convert.ToInt32(len / 2)
                '���R�[�h�̌㔼����EBCDIC�f�[�^�Ȃ̂�ReadByteBin�Ɋi�[
                Array.Copy(ReadByte, len, RecordDataBin, 0, len)
            End If
            '***********************************

            ' �Ǎ��f�[�^�iBYTE)�𕶎���ɕϊ�
            data = CurrEncd.GetString(ReadByte, 0, len)
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        Finally
            RecordData = data
        End Try

        Return len
    End Function

    '
    ' �@�\�@ �F �P�o�C�g�ǂݍ���
    '
    ' �����@ �F ARG1 - �o�C�g�i�Q�Ɠn���j
    '
    ' �߂�l �F �ǂݍ��񂾒���
    '
    ' ���l�@ �F
    '
    Public Overridable Function GetFileData(ByRef data As Byte) As Integer
        Dim len As Integer = 0          ' ���R�[�h��
        Dim bt(0) As Byte

        Try
            ' �P�o�C�g��ǂݍ���
            len = IOfileStream.Read(bt, 0, 1)
            If len = 1 Then
                data = bt(0)
            End If
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return 0
        End Try

        Return len
    End Function

    '
    ' �@�\�@ �F �t�@�C���ɏ�������
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F ���s�R�[�h�͊܂܂Ȃ�
    '
    Public Overridable Function WriteData(ByVal data As String) As Integer
        Try
            IOfileStream.Write(EncdJ.GetBytes(data), 0, DataInfo.RecoedLen)

            '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- START
            With DataInfo
                If .WorkLen > .RecoedLen Then
                    '���s���X�y�[�X�Ŗ��߂�
                    Dim b As String = Space(.WorkLen - .RecoedLen)
                    IOfileStream.Write(EncdJ.GetBytes(b), 0, b.Length)
                End If
            End With
            '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- END

            Return 1
        Catch ex As Exception
            Try
                If EncdJ.GetBytes(data).Length < DataInfo.RecoedLen Then
                    IOfileStream.Write(EncdJ.GetBytes(data), 0, EncdJ.GetBytes(data).Length)
                    Return 1
                Else
                    DataInfo.Message = ex.Message
                End If
            Catch exB As Exception
                DataInfo.Message = ex.Message
            End Try
        End Try
        Return 0
    End Function

    '
    ' �@�\�@ �F �t�@�C���ɏ�������
    '
    ' �߂�l �F 0 - ���s�C1 - ����
    '
    ' ���l�@ �F ���s�R�[�h�͊܂܂Ȃ�
    '
    Public Overridable Function WriteCrLf() As Integer
        Try
            IOfileStream.Write(New Byte() {13, 10}, 0, 2)
        Catch ex As Exception
            DataInfo.Message = ex.Message
        End Try
    End Function

    '
    ' �@�\�@ �F �d�n�e����
    '
    ' �����@ �F CheckFLG - �`�F�b�N���@�t���O
    '
    ' �߂�l �F �d�n�e
    '
    ' ���l�@ �F2009/09/23.Sakon�@CheckFLG�ɂ��A�`�F�b�N���@�ύX��ǉ�
    '
    Public Function EOF(Optional ByVal CheckFLG As Integer = 0) As Boolean
        Try
            '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
            'If Not mInfoComm Is Nothing AndAlso _
            '    (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
            ''If Not mInfoComm Is Nothing Then

            '    If IOreader Is Nothing Then
            '        Return False
            '    End If
            '    Return IOreader.EOF
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++

            ''***** 2009/09/20 kakiwaki *******
            ''���A�g�敪�̏������ȊO��߂�
            'If Not mInfoComm Is Nothing Then

            '    If IOreader Is Nothing Then
            '        Return False
            '    End If
            '    Return IOreader.EOF
            'End If
            ''**************** 2009/09/20 *****

            'If IOfileStream.Position >= DataInfo.FileLen Then
            '    Return True
            'End If
            'Return False

            '-------------------------------------
            '�����ɂ���ă`�F�b�N������ύX����
            '-------------------------------------
            If CheckFLG = 0 Then
                If IOfileStream.Position >= DataInfo.FileLen Then
                    Return True
                End If
                Return False
            Else
                '***** 2009/09/20 kakiwaki *******
                '���A�g�敪�̏������ȊO��߂�
                If Not mInfoComm Is Nothing Then

                    If IOreader Is Nothing Then
                        Return False
                    End If
                    Return IOreader.EOF
                End If
                '**************** 2009/09/20 *****
            End If

        Catch ex As Exception
            Return True
        End Try
    End Function

    '
    ' �@�\�@ �F �w�b�_���R�[�h����
    '
    ' �߂�l �F True - �w�b�_���R�[�h
    '
    ' ���l�@ �F 
    '
    Public Overridable Function IsHeaderRecord() As Boolean
        For i As Integer = 0 To HeaderKubun.Length - 1
            If RecordData.StartsWith(HeaderKubun(i)) = True Then
                Return True
            End If
        Next i
        Return False
    End Function

    '
    ' �@�\�@ �F �f�[�^���R�[�h����
    '
    ' �߂�l �F True - �f�[�^���R�[�h
    '
    ' ���l�@ �F 
    '
    Public Overridable Function IsDataRecord() As Boolean
        For i As Integer = 0 To DataKubun.Length - 1
            If RecordData.StartsWith(DataKubun(i)) = True Then
                Return True
            End If
        Next i
        Return False
    End Function

    '
    ' �@�\�@ �F �g���[�����R�[�h����
    '
    ' �߂�l �F True - �g���[���[���R�[�h
    '
    ' ���l�@ �F �˗��f�[�^�f�[�^�̏I��肩�ǂ����𔻒�
    '
    Public Overridable Function IsTrailerRecord() As Boolean
        For i As Integer = 0 To TrailerKubun.Length - 1
            If RecordData.StartsWith(TrailerKubun(i)) = True Then
                Return True
            End If
        Next i
        Return False
    End Function

    '
    ' �@�\�@ �F �G���h���R�[�h����
    '
    ' �߂�l �F True - �G���h���R�[�h
    '
    ' ���l�@ �F �˗��f�[�^�f�[�^�̏I��肩�ǂ����𔻒�
    '
    Public Overridable Function IsEndRecord() As Boolean
        If RecordData.StartsWith(DataInfo.MinRecordCode(DataInfo.MinRecordCode.Length - 1)) = True Then
            Return True
        End If
        Return False
    End Function

    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Protected Friend Shared Function CuttingData(ByRef value As String, ByVal len As Integer) As String
        Try
            ' �؂��镶����
            'Dim ret As String = value.Substring(0, len)
            Dim ret As String
            Dim bt() As Byte = EncdJ.GetBytes(value)
            ret = EncdJ.GetString(bt, 0, len)
            ' �؂�������̎c��̕�����
            'value = value.Substring(len)
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' �@�\�@ �F ���p�����݂̂��`�F�b�N����
    '
    ' �����@ �F ARG1 - ������
    '
    ' �߂�l �F True�F���p�̂݁CFalse�F�S�p����
    '
    ' ���l�@ �F
    '
    Public Function CheckHankaku(ByVal value As String) As Boolean
        Return (value.Length = EncdJ.GetByteCount(value))
    End Function

    '
    ' �@�\�@ �F ���R�[�h�`�F�b�N
    '
    ' �߂�l �F �s�������̈ʒu
    '
    ' ���l�@ �F
    '
    Public Overridable Function CheckRegularString() As Long
        Dim nRet As Long

        ' �e�t�H�[�}�b�g ���ʃ`�F�b�N
        Select Case RecordData.Substring(0, 1)
            Case "1"        ' �w�b�_
                ' �S�p�`�F�b�N
                nRet = GetZenkakuPos(RecordData)
                If nRet = -1 Then
                    '�K�蕶���`�F�b�N �w�b�_�[���R�[�h�͒��_�n�j
                    nRet = CheckRegularStringVerB(RecordData)
                End If
                Return nRet
            Case "8", "9"   ' �g���[�����R�[�h�C�G���h���R�[�h

                nRet = GetZenkakuPos(RecordData)
                If nRet = -1 Then
                    '�K�蕶���`�F�b�N �w�b�_�[���R�[�h�͒��_�m�f
                    nRet = CheckRegularStringVerA(RecordData)
                End If
                Return nRet
        End Select

        ' �S�p�`�F�b�N
        Return GetZenkakuPos(RecordData)
    End Function

    '
    ' �@�\�@ �F �o�C�g�z�񂩂�C��������擾����
    '
    ' �����@ �F ARG1 - �o�C�g�z��
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Public Function GetString(ByVal bt() As Byte) As String
        Return CurrEncd.GetString(bt)
    End Function

    '
    ' �@�\�@ �F �t�@�C�������
    '
    ' ���l�@ �F
    '
    Public Sub Close()
        '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
        'If Not mInfoComm Is Nothing AndAlso _
        '        (mInfoComm.INFOParameter.RENKEI_KBN = "88" OrElse mInfoComm.INFOParameter.RENKEI_KBN = "98") Then
        ''If Not mInfoComm Is Nothing Then

        'If Not IOreader Is Nothing Then
        '    IOreader.Close()
        'End If

        'Return
        'End If
        '+++++++++++++++++++++++++++++++++++++++++++
        If Not IOfileStream Is Nothing Then
            IOfileStream.Close()
        End If
        IOfileStream = Nothing
    End Sub

    '
    ' �@�\�@ �F �d�a�b�c�h�b�̔���
    '
    ' ���l�@ �F
    '
    Public ReadOnly Property IsEBCDIC() As Boolean
        Get
            If CurrEncd.Equals(EncdE) = True Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    '
    ' �@�\�@ �F ���b�Z�[�W
    '
    ' ���l�@ �F
    '
    Public ReadOnly Property Message() As String
        Get
            If DataInfo.Message Is Nothing Then
                Return ""
            End If
            Return DataInfo.Message
        End Get
    End Property

#Region "�ʒu�擾"

    '
    ' �@�\�@ �F �S�p�����̈ʒu���擾����
    '
    ' �����@ �F ARG1 - ������
    '
    ' �߂�l �F �S�p�����̈ʒu
    '
    ' ���l�@ �F
    '
    Public Function GetZenkakuPos(ByVal value As String) As Long
        If CheckHankaku(value) = True Then
            Return -1
        End If
        For i As Integer = 0 To value.Length - 1
            Dim s As String = value.Substring(i, 1)
            If EncdJ.GetByteCount(s) = 2 Then
                Return i
            End If
        Next i
        Return -1
    End Function
#End Region

#Region "�`�F�b�N�֐�"
    '
    ' �@�\�@ �F �K��O������u��
    '
    ' �����@ �F ARG1 - ������
    '
    ' �߂�l �F �u���㕶����
    '
    ' ���l�@ �F�󔒈ȊO�ɒu������ꍇ�͂��̊֐����ɒǉ�����
    '
    Public Function ReplaceString(ByVal Value As String) As String
        Dim work As String = Value

        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "�")       ' �
        'work = work.Replace("�", "-")       ' �
        'work = work.Replace("�", ".")       ' ��i���p���_�j
        'work = work.Replace("�@", "  ")     ' �S�p��
        ''2008/04/30 �u�������ǉ�
        'work = work.Replace("`", " ")       ' `
        'work = work.Replace("�", " ")       ' �
        'work = work.Replace("<", " ")       ' <
        'work = work.Replace(">", " ")       ' >
        ''*** �C�� mitsu 2008/11/26 �u�������ǉ� ***
        'work = work.Replace("*", " ")       ' *
        ''******************************************

        For Each de As DictionaryEntry In ReplaceStringPattern
            work = work.Replace(de.Key.ToString, de.Value.ToString)
        Next
        work = work.Replace(Convert.ToChar(0), " ") 'NULL

        work = work.ToUpper()

        Return work
    End Function

    '
    ' �@�\�@ �F �K��O������u��
    '
    ' �����@ �F ARG1 - ������ ARG2 - �S�p�����ʒu�C���f�b�N�X(Long)
    '
    ' �߂�l �F �u���㕶����
    '
    ' ���l�@ �F�g���[�����R�[�h�E�G���h���R�[�h�ɑ΂��鏈�u
    '
    Public Function ReplaceString(ByVal Value As String, ByVal nRet As Long) As String

        '2010/10/07.Sakon�@�R�����g���F�S�p�ϊ����t�@�C������擾���� ****************************
        'If nRet < 0 Then 'nRet�̒l���s���ȂƂ�
        '    nRet = GetZenkakuPos(Value)
        'End If

        ''�S�p���������݂��Ȃ��Ȃ�܂Ń��[�v
        'While nRet >= 0
        '    '2009/09/17.Sakon�@�S�p���ʁA�S�p���_�̓Ǒ֍l���ǉ� ++++++++++++++++++++++++++++
        '    Select Case Value.Substring(Convert.ToInt32(nRet), 1)
        '        Case "�i"
        '            '�S�p"�i" �� ���p��߰� & "(" 
        '            Value = Value.Replace("�i", " (")
        '            nRet = GetZenkakuPos(Value)
        '        Case "�j"
        '            '�S�p"�j" �� ")" & ���p��߰�
        '            Value = Value.Replace("�j", ") ")
        '            nRet = GetZenkakuPos(Value)
        '        Case "�E"
        '            '�S�p"�E" �� "�" & ���p��߰�
        '            Value = Value.Replace("�E", "� ")
        '            nRet = GetZenkakuPos(Value)
        '        Case Else
        '            '�S�p������؂�o����*2�ɒu��
        '            Value = Value.Replace(Value.Substring(Convert.ToInt32(nRet), 1), "  ")
        '            nRet = GetZenkakuPos(Value)
        '    End Select
        '    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        'End While
        '*****************************************************************************************

        '�K��O�����u��
        Value = ReplaceString(Value)                    '�󔒈ȊO�̋K��O�����u��
        'Value = RegularRegexVerA.Replace(Value, " ")    '�K��O�����u��A
        'Value = RegularRegexVerB.Replace(Value, " ")    '�K��O�����u��B
        'Value = RegularRegexVerPlus.Replace(Value, " ") '�u+�v�ɑ΂���u��

        Return Value
    End Function

    '
    ' �@�\�@ �F �K��O����������s��(���_�m�f)
    '
    ' �����@ �F ARG1 - �����R�[�h
    '
    ' �߂�l �F �����ʒu
    '
    ' ���l�@ �F
    '
    Protected Friend Function CheckRegularStringVerA(ByVal Value As String) As Integer
        Return CheckRegularStringVerA(Value, 0)
    End Function

    '
    ' �@�\�@ �F �K��O����������s��(���_�m�f)
    '
    ' �����@ �F ARG1 - �����R�[�h
    '
    ' �߂�l �F �����ʒu
    '
    ' ���l�@ �F
    '
    Protected Friend Function CheckRegularStringVerA(ByVal Value As String, ByVal startat As Integer) As Integer
        If RegularRegexVerA.IsMatch(Value, startat) = True Then
            ' �K��O����������ꍇ�́C�����ʒu����������
            Return RegularRegexVerA.Match(Value, startat).Index
        Else
            If RegularRegexVerPlus.IsMatch(Value) = True Then
                ' �K��O����������ꍇ�́C�����ʒu����������
                Return RegularRegexVerPlus.Match(Value).Index
            End If
        End If

        Return -1
    End Function

    '
    ' �@�\�@ �F �K��O����������s��(���_�m�f)
    '
    ' �����@ �F ARG1 - �����R�[�h
    '
    ' �߂�l �F �����ʒu
    '
    ' ���l�@ �F
    '
    Protected Friend Function CheckRegularStringVerA(ByVal Value As String, ByVal beginning As Integer, ByVal length As Integer) As Integer
        If RegularRegexVerA.IsMatch(Value, beginning) = True Then
            ' �K��O����������ꍇ�́C�����ʒu����������
            Dim mat As Match = RegularRegexVerA.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        If RegularRegexVerPlus.IsMatch(Value, beginning) = True Then
            ' �K��O����������ꍇ�́C�����ʒu����������
            Dim mat As Match = RegularRegexVerPlus.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        Return -1
    End Function

    '
    ' �@�\�@ �F �K��O����������s��(���_�n�j)
    '
    ' �����@ �F ARG1 - �����R�[�h
    '
    ' �߂�l �F �����ʒu
    '
    ' ���l�@ �F
    '
    Protected Friend Function CheckRegularStringVerB(ByVal Value As String) As Integer
        Return CheckRegularStringVerB(Value, 0)
    End Function

    '
    ' �@�\�@ �F �K��O����������s��(���_�n�j)
    '
    ' �����@ �F ARG1 - �����R�[�h
    '
    ' �߂�l �F �����ʒu
    '
    ' ���l�@ �F
    '
    Protected Friend Function CheckRegularStringVerB(ByVal Value As String, ByVal startat As Integer) As Integer
        If RegularRegexVerB.IsMatch(Value, startat) = True Then
            ' �K��O����������ꍇ�́C�����ʒu����������
            Return RegularRegexVerB.Match(Value, startat).Index
        End If

        Return -1
    End Function

    '
    ' �@�\�@ �F �K��O����������s��(���_�n�j)
    '
    ' �����@ �F ARG1 - �����R�[�h
    '
    ' �߂�l �F �����ʒu
    '
    ' ���l�@ �F
    '
    Protected Friend Function CheckRegularStringVerB(ByVal Value As String, ByVal beginning As Integer, ByVal length As Integer) As Integer
        If RegularRegexVerB.IsMatch(Value, beginning) = True Then
            ' �K��O����������ꍇ�́C�����ʒu����������
            Dim mat As Match = RegularRegexVerB.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        If RegularRegexVerPlus.IsMatch(Value, beginning) = True Then
            ' �K��O����������ꍇ�́C�����ʒu����������
            Dim mat As Match = RegularRegexVerPlus.Match(Value, beginning, length)
            If mat.Success = True Then
                Return mat.Index
            End If
        End If

        Return -1
    End Function
#End Region

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F 
    '
    Public Overridable Function CheckDataFormat() As String
        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1���R�[�h�Ǎ�
            If GetFileData() > 0 Then
                ' �����u������
                If CheckRegularString() > 0 Then
                    '***�C�� maeda 2008/05/14*************************************************
                    '�U�֏����敪���A3(�����U)�̏ꍇ�K��O���������݂��Ă��G���[�Ƃ��Ȃ�
                    'If mInfoComm.INFOParameter.FSYORI_KBN <> "3" Then
                    '    ' �K��O��������
                    '    Return "ERR"
                    'End If
                    '' �K��O��������
                    'Return "ERR"
                    '*************************************************************************
                End If
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try
    End Function

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F �s�\���ʃf�[�^�������ǂݍ���
    '
    Public Overridable Function CheckKekkaFormat() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1���R�[�h�Ǎ�
            'If GetFileDataHenkan() > 0 Then
            If GetFileDataFunou() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F �ԊҎ��C���׃}�X�^�������ǂݍ���
    '
    Public Overridable Function CheckHenkanFormat() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1���R�[�h�Ǎ�
            If GetFileDataHenkan() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function
    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F �ԊҎ��C���׃}�X�^�������ǂݍ���
    '
    Public Overridable Function CheckSaifuriFormat() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1���R�[�h�Ǎ�
            If GetFileDataSaifuri() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function
    '
    ' �@�\�@ �F �w�b�_���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Public Overridable Function CheckRecord1() As String
        Return ""
    End Function

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N �㏈��
    '
    ' ���l�@ �F
    '
    Protected Friend Sub CheckDataFormatAfter()
        Try
            ' ���v�˗������C���v�˗����z
            InfoMeisaiMast.TOTAL_KEN += InfoMeisaiMast.FURIKEN
            '2009/12/03 0�~�f�[�^���������˗����� ===========
            If InfoMeisaiMast.FURIKIN > 0 Then
                InfoMeisaiMast.TOTAL_KEN2 += InfoMeisaiMast.FURIKEN
            End If
            '================================================
            If InfoMeisaiMast.FURIKEN = 1 Then
                InfoMeisaiMast.TOTAL_KIN += InfoMeisaiMast.FURIKIN
            End If

            '***�C�� maeda 2008/05/15**********************************************************************
            '���U�ł��U�֌��ʃR�[�h������悤�ɍďC��(�˗��f�[�^�̐U�֌��ʃR�[�h��7,8�ȊO�̏ꍇ) 
            '***�C�� 2008/05/01 mitsu *********************************************************
            '���U�̏ꍇ�͐U�֌��ʃR�[�h�����Ȃ�
            If Not (mInfoComm Is Nothing) Then
                If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
                    ' 2008.04.22 MODIFY �������݃`�F�b�N�d�l�ύX (FJH�a�w��) >>
                    'If InfoMeisaiMast.FURIKETU_CODE = 9 Then
                    If InfoMeisaiMast.FURIKETU_CODE <> 0 Then
                        ' 2008.04.22 MODIFY �������݃`�F�b�N�d�l�ύX (FJH�a�w��) <<
                        ' �ُ팏���C�ُ���z
                        InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
                        If InfoMeisaiMast.FURIKEN = 1 Then
                            InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
                        End If
                    End If
                Else
                    Select Case InfoMeisaiMast.FURIKETU_CODE
                        Case 7, 8 '�˗��f�[�^�ɓ����Ă���f�[�^
                            InfoMeisaiMast.FURIKETU_CODE = 0
                        Case 2, 9 '�V�X�e���ňُ펞�Ɏg�p����R�[�h
                            ' �ُ팏���C�ُ���z
                            InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
                            If InfoMeisaiMast.FURIKEN = 1 Then
                                InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
                            End If
                        Case Else
                            InfoMeisaiMast.FURIKETU_CODE = 0
                    End Select
                End If
            End If
            '**********************************************************************************
            ''***�C�� 2008/05/01 mitsu *********************************************************
            ''���U�̏ꍇ�͐U�֌��ʃR�[�h�����Ȃ�
            'If Not (mInfoComm Is Nothing) Then
            '    If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
            '        ' 2008.04.22 MODIFY �������݃`�F�b�N�d�l�ύX (FJH�a�w��) >>
            '        'If InfoMeisaiMast.FURIKETU_CODE = 9 Then
            '        If InfoMeisaiMast.FURIKETU_CODE <> 0 Then
            '            ' 2008.04.22 MODIFY �������݃`�F�b�N�d�l�ύX (FJH�a�w��) <<
            '            ' �ُ팏���C�ُ���z
            '            InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
            '            If InfoMeisaiMast.FURIKEN = 1 Then
            '                InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
            '            End If
            '        End If
            '    End If
            'End If
            ''**********************************************************************************
            '*********************************************************************************************

            ' ���R�[�h�敪��ۑ�
            BeforeRecKbn = RecordData.Substring(0, 1)
        Catch ex As Exception
        End Try
    End Sub

    Private Function GetJifuriSQL() As System.Text.StringBuilder
        Dim SQL As New StringBuilder(128)

        '�����R�[�h�����i�}���`�̏ꍇ���l������j�A�ϑ��҃R�[�h�̃`�F�b�N
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(",TYUUDAN_FLG_S")
        SQL.Append(",TOUROKU_FLG_S")
        SQL.Append(",UKETUKE_FLG_S")
        SQL.Append(",BAITAI_CODE_T")
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        SQL.Append(",MOTIKOMI_DATE_S")  '��������
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
        SQL.Append(" FROM TORIMAST,SCHMAST")

        Select Case mInfoComm.INFOToriMast.MULTI_KBN_T
            Case "0"    '�V���O�� (������R�[�h�A���R�[�h�Ō���)
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                SQL.Append("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            Case Else   '�}���`�i��\�ϑ��҃R�[�h�A�ϑ��҃R�[�h�Ō����j
                ' 2016/11/08 �^�X�N�j���� CHG �yME�z(RSV2�Ή� �W���o�O�C��) -------------------- START
                ''2011/06/16 �W���ŏC�� �}���`�̏ꍇ�́A�`�[�E�˗����͍l�����Ȃ� ------------------START
                'If mInfoComm.INFOToriMast.BAITAI_CODE_T <> "09" OrElse mInfoComm.INFOToriMast.BAITAI_CODE_T <> "04" Then
                '    SQL.Append(" WHERE ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                '    SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mInfoComm.INFOToriMast.ITAKU_KANRI_CODE_T))
                'Else
                '    '�}�̃R�[�h�`�[�̏ꍇ�̓V���O�������Ƃ���B 2011.5.27
                '    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                '    SQL.Append("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                'End If
                ''2011/06/16 �W���ŏC�� �}���`�̏ꍇ�́A�`�[�E�˗����͍l�����Ȃ� ------------------END
                Select Case mInfoComm.INFOToriMast.BAITAI_CODE_T
                    Case "04", "09"
                        '-----------------------------------------------
                        ' �}�̃R�[�h���A�˗����E�`�[�̓V���O������
                        '-----------------------------------------------
                        SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                        SQL.Append("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                    Case Else
                        '-----------------------------------------------
                        '�}�̃R�[�h���A�˗����E�`�[�ȊO�̓}���`����
                        '-----------------------------------------------
                        SQL.Append(" WHERE ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mInfoComm.INFOToriMast.ITAKU_KANRI_CODE_T))
                End Select
                ' 2016/11/08 �^�X�N�j���� CHG �yME�z(RSV2�Ή� �W���o�O�C��) -------------------- END
                '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i��ʃR�[�h�ǉ�(���^�E�ܗ^�œ���ϑ��҃R�[�h�����݂��邽��)�j----- START
                If INI_JIFURI_SYUBETU_KEY = "1" Then
                    SQL.Append(" AND SYUBETU_T = " & SQ(InfoMeisaiMast.SYUBETU_CODE))
                End If
                '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i��ʃR�[�h�ǉ�(���^�E�ܗ^�œ���ϑ��҃R�[�h�����݂��邽��)�j----- END
        End Select
        SQL.Append(" AND FURI_DATE_S = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

        Return SQL
    End Function

    Private Function GetFurikomiSQL() As System.Text.StringBuilder
        Dim SQL As New StringBuilder(128)

        '�����R�[�h�����i�}���`�̏ꍇ���l������j�A�ϑ��҃R�[�h�̃`�F�b�N
        SQL.AppendLine("SELECT")
        SQL.AppendLine(" TORIS_CODE_T")
        SQL.AppendLine(",TORIF_CODE_T")
        SQL.AppendLine(",TYUUDAN_FLG_S")
        SQL.AppendLine(",TOUROKU_FLG_S")
        SQL.AppendLine(",UKETUKE_FLG_S")
        SQL.AppendLine(",BAITAI_CODE_T")
        SQL.AppendLine(",CYCLE_T")
        SQL.AppendLine(",ERROR_INF_S")
        SQL.AppendLine(",MOTIKOMI_SEQ_S")
        SQL.AppendLine(",TUKI1_T")
        SQL.AppendLine(",TUKI2_T")
        SQL.AppendLine(",TUKI3_T")
        SQL.AppendLine(",TUKI4_T")
        SQL.AppendLine(",TUKI5_T")
        SQL.AppendLine(",TUKI6_T")
        SQL.AppendLine(",TUKI7_T")
        SQL.AppendLine(",TUKI8_T")
        SQL.AppendLine(",TUKI9_T")
        SQL.AppendLine(",TUKI10_T")
        SQL.AppendLine(",TUKI11_T")
        SQL.AppendLine(",TUKI12_T")
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        SQL.AppendLine(",MOTIKOMI_DATE_S")  '��������
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
        SQL.AppendLine(" FROM S_TORIMAST,S_SCHMAST")

        Select Case mInfoComm.INFOToriMast.MULTI_KBN_T
            Case "0"    '�V���O�� (������R�[�h�A���R�[�h�Ō���)
                SQL.AppendLine(" WHERE TORIS_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                SQL.AppendLine("   AND TORIF_CODE_T = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            Case Else   '�}���`�i��\�ϑ��҃R�[�h�A�ϑ��҃R�[�h�A��ʂŌ����j
                SQL.AppendLine(" WHERE ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                SQL.AppendLine("   AND SYUBETU_T =" & SQ(InfoMeisaiMast.SYUBETU_CODE))
                SQL.AppendLine("   AND ITAKU_KANRI_CODE_T = " & SQ(mInfoComm.INFOToriMast.ITAKU_KANRI_CODE_T))
        End Select

        If mInfoComm.INFOToriMast.KIJITU_KANRI_T = "1" Then
            ' �����Ǘ�����
            SQL.AppendLine(" AND FURI_DATE_S = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            SQL.AppendLine(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.AppendLine(" ORDER BY MOTIKOMI_SEQ_S")
        Else
            '�����Ǘ��Ȃ�
            SQL.AppendLine(" AND FURI_DATE_S(+)  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            SQL.AppendLine(" AND TORIS_CODE_S(+) = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S(+) = TORIF_CODE_T")
        End If

        Return SQL
    End Function

    Public Function GetTorimastFromItakuCode(ByVal db As CASTCommon.MyOracle) As Boolean
        Return GetTorimastFromItakuCode(InfoMeisaiMast.SYUBETU_CODE, InfoMeisaiMast.ITAKU_CODE, db)
    End Function

    Public Function GetTorimastFromItakuCodeSSS(ByVal db As CASTCommon.MyOracle) As Boolean
        'Return GetTorimastFromItakuCode(InfoMeisaiMast.ITAKU_CODE.Substring(5, 4).PadLeft(10, "0"c), db)
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)

        '2017/02/27 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
        Dim SSS_ITAKUCODE_PATN As String = String.Empty
        SSS_ITAKUCODE_PATN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_ITAKUCODE_PATN")
        If SSS_ITAKUCODE_PATN = "err" OrElse SSS_ITAKUCODE_PATN = "" Then
            SSS_ITAKUCODE_PATN = "0"
        End If
        '2017/02/27 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END

        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE ")
            '2017/02/27 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            Select Case SSS_ITAKUCODE_PATN
                Case "0"
                    SQL.Append(" ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
                Case "1"
                    SQL.Append(" SUBSTR(ITAKU_CODE_T, 1, 9) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(0, 9)))
                Case "2"
                    SQL.Append(" SUBSTR(ITAKU_CODE_T, 7, 4) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(5, 4)))
            End Select
            ''*** �C�� START 2009/03/05 NISHIDA SSS�ϑ��҃R�[�h�̎擾�ӏ������
            ''SQL.Append(" SUBSTR(ITAKU_CODE_T, 7, 4) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(5, 4)))
            'SQL.Append(" SUBSTR(ITAKU_CODE_T, 1, 9) = " & SQ(InfoMeisaiMast.ITAKU_CODE.Substring(0, 9)))
            ''*** �C�� END
            '2017/02/27 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
            '2008/04/17 ��ʃR�[�h�ǉ�
            SQL.Append("   AND SYUBETU_T =" & SQ(InfoMeisaiMast.SYUBETU_CODE))
            SQL.Append(" AND FMT_KBN_T IN ('20', '21')")
            If OraReader.DataReader(SQL) = True Then
                Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
                Return True
            Else
                Call mInfoComm.GetTORIMAST("", "")
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            OraReader.Close()
        End Try
    End Function

    Public Function GetTorimastFromItakuCodeTAKO(ByVal db As CASTCommon.MyOracle) As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)
        Dim ToriCode As String

        SQL.Append("SELECT TORIS_CODE_V || TORIF_CODE_V TORI_CODE")
        SQL.Append(" FROM TAKOUMAST, TAKOSCHMAST")
        SQL.Append(" WHERE TKIN_NO_V = " & CASTCommon.SQ(InfoMeisaiMast.ITAKU_KIN))
        SQL.Append("   AND ITAKU_CODE_V = " & CASTCommon.SQ(InfoMeisaiMast.ITAKU_CODE))
        SQL.Append("   AND TORIS_CODE_V = TORIS_CODE_U")
        SQL.Append("   AND TORIF_CODE_V = TORIF_CODE_U")
        SQL.Append("   AND TKIN_NO_V    = TKIN_NO_U")
        SQL.Append("   AND FURI_DATE_U  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
        If OraReader.DataReader(SQL) = False Then
            Return False
        End If
        ToriCode = OraReader.GetString("TORI_CODE")

        Call GetTorimastFromToriCode(ToriCode, db)
        If ToriData.INFOToriMast.TORIS_CODE_T Is Nothing Then
            Return False
        End If

        Return True
    End Function

    '2011/03/01 ��Ǝ����s�\�X�V�p�̊֐��ǉ�
    Public Function GetTorimastFromItakuCodeKigyo(ByVal db As CASTCommon.MyOracle) As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST, SCHMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))
            SQL.Append(" AND FURI_DATE_S = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" ORDER BY FSYORI_KBN_T, TORIS_CODE_T, TORIF_CODE_T")

            If OraReader.DataReader(SQL) = True Then
                Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
                Return True
            Else
                Call mInfoComm.GetTORIMAST("", "")
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            OraReader.Close()
        End Try
    End Function

    Public Sub GetTorimastFromToriCode(ByVal toriCode As String, ByVal db As CASTCommon.MyOracle)
        Call mInfoComm.GetTORIMAST(toriCode.Substring(0, 10), toriCode.Substring(10))
    End Sub

    Public Function GetTorimastFromItakuCode(ByVal SyubetuCode As String, ByVal itakusyaCode As String, ByVal db As CASTCommon.MyOracle) As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" ITAKU_CODE_T = " & SQ(itakusyaCode))
            ' 2008/04/24 DELETE ��ʃR�[�h���`�F�b�N����̂͐U���݂̂� �ďC�� >>
            ''2008/04/17 ��ʃR�[�h�ǉ�
            'SQL.Append("   AND SYUBETU_T =" & SQ(SyubetuCode))
            ' 2008/04/24 DELETE ��ʃR�[�h���`�F�b�N����̂͐U���݂̂� �ďC�� <<
            SQL.Append(" ORDER BY FSYORI_KBN_T, TORIS_CODE_T, TORIF_CODE_T")
            If OraReader.DataReader(SQL) = False Then
                OraReader.Close()
                ' 2008/04/24 MODIFY ��ʃR�[�h���`�F�b�N����̂͐U���݂̂� �ďC�� >>
                'OraReader.DataReader(SQL.Replace("TORIMAST", "S_TORIMAST"))
                SQL = New StringBuilder
                SQL.Append("SELECT ")
                SQL.Append(" TORIS_CODE_T")
                SQL.Append(",TORIF_CODE_T")
                SQL.Append(" FROM S_TORIMAST")
                SQL.Append(" WHERE ")
                SQL.Append(" ITAKU_CODE_T = " & SQ(itakusyaCode))
                SQL.Append("   AND SYUBETU_T =" & SQ(SyubetuCode))
                SQL.Append(" ORDER BY FSYORI_KBN_T, TORIS_CODE_T, TORIF_CODE_T")
                ' 2008/04/24 MODIFY ��ʃR�[�h���`�F�b�N����̂͐U���݂̂� �ďC�� <<
            End If

            If OraReader.DataReader(SQL) = True Then
                Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
                Return True
            Else
                Call mInfoComm.GetTORIMAST("", "")
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            OraReader.Close()
        End Try
    End Function

    '
    ' �@�\�@ �F �w�b�_���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' ����   �F ARG1 - ���[�h(0 - �ʏ�C1 - �����U�������ݎ��Ɏd�l�j
    '
    ' ���l�@ �F �e�t�H�[�}�b�g�`�F�b�N����C�Ă΂�鋤�ʊ֐�
    '�@�@�@     �w�b�_��񂩂�C�����}�X�^���Q�Ƃ���
    '
    Protected Overridable Function CheckHeaderRecord(Optional ByVal mode As Integer = 0) As Boolean

        ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-02(RSV2�Ή�) -------------------- START
        Dim INI_RSV2_HEAD_BANKCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BANKCODE")
        '*** Str Add 2016/01/05 sys)mori for �o�O�C���Ή� ***
        'Dim INI_RSV2_HEAD_S_SITENCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_SITENCOD")
        Dim INI_RSV2_HEAD_S_SITENCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_SITENCODE")
        '*** End Add 2016/01/05 sys)mori for �o�O�C���Ή� ***
        Dim INI_RSV2_HEAD_S_KAMOKUCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAMOKUCODE")
        Dim INI_RSV2_HEAD_S_KOUZABANGO As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KOUZABANGO")
        ' 2015/12/11 �^�X�N�j���� ADD �yPG�zUI_B-14-02(RSV2�Ή�) -------------------- END
        '2018/09/24 saitou �L���M��(RSV2�Ή�) ADD �y���s�z�i���U�ϑ��҃R�[�h�`�F�b�N�ہj ---------- START
        Dim INI_RSV2_HEAD_S_ITAKUCODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_ITAKUCODE")
        '2018/09/24 saitou �L���M��(RSV2�Ή�) ADD --------------------------------------------------- END

        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        ' �c�a�ڑ������݂��Ȃ��ꍇ�C����l��Ԃ�
        If OraDB Is Nothing Then
            Return True
        End If

        '�}���`�敪��0�̏ꍇ�A�����w�b�_�Ȃ�Έُ�I���Ƃ���
        HeaderCnt += 1

        If ToriData.INFOToriMast.MULTI_KBN_T = "0" AndAlso HeaderCnt > 2 Then '���V���O���ł�2��ʂ�
            WriteBLog("�}���`�敪�ُ�", "�G���[����")
            DataInfo.Message = " �����}�X�^�o�^�ُ�F�}���`�敪"
            Return False
        End If

        TAKOU_ON = False

        If CASTCommon.IsDecimal(InfoMeisaiMast.FURIKAE_DATE_MOTO) = False Then
            WriteBLog("�w�b�_" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�ُ�", "�ُ�", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
            DataInfo.Message = "�w�b�_" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�ُ� �ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO
            Return False
        Else
            If InfoMeisaiMast.FURIKAE_DATE = "00010101" Then
                WriteBLog("�w�b�_" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�ُ�", "�ُ�", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
                DataInfo.Message = "�w�b�_" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�ُ� �ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO
                Return False
            End If
        End If

        If mInfoComm.INFOParameter.FSYORI_KBN = "1" Then
            ' ���U
            SQL = GetJifuriSQL()
        Else
            ' �U��
            SQL = GetFurikomiSQL()
        End If

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then
            DataInfo.Message = "�����/���ޭ�ٌ������s"
            DataInfo.Message &= " �ϑ���" & mInfoComm.INFOToriMast.ITAKU_CODE_T

            Select Case mInfoComm.INFOToriMast.MULTI_KBN_T
                Case "0"    '�V���O�� (������R�[�h�A���R�[�h�Ō���)
                    WriteBLog("�t�@�C���w�b�_����挟��", "���s",
                        "�����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "�|" & mInfoComm.INFOToriMast.TORIS_CODE_T)
                Case Else   '�}���`�i��\�ϑ��҃R�[�h�A�ϑ��҃R�[�h�Ō����j
                    WriteBLog("�����/���ޭ�ٌ���", "���s",
                        " �ϑ��Һ���:" & InfoMeisaiMast.ITAKU_CODE &
                        " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
                    DataInfo.Message = "�����/���ޭ�ٌ������s"
                    DataInfo.Message &= " �ϑ���" & InfoMeisaiMast.ITAKU_CODE
            End Select
            ' �����}�X�^�����N���A����
            Call mInfoComm.ClearTORIMAST()
            Return False
        Else
            ' �����}�X�^���擾
            Call mInfoComm.SelectTORIMAST(mInfoComm.INFOParameter.FSYORI_KBN, OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
        End If

        ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-02(RSV2�Ή�) -------------------- START
        '' �����s�ԍ��`�F�b�N
        'If InfoMeisaiMast.ITAKU_KIN <> JIKINKO Then
        '    WriteBLog("ͯ�ގ���������", "�s��v", "���Z�@�ֺ���:" & InfoMeisaiMast.ITAKU_KIN)
        '    DataInfo.Message = "ͯ�ގ����������s��v ���Z�@�ֺ���:" & InfoMeisaiMast.ITAKU_KIN
        '    Return False
        'End If

        ''���U�̏ꍇ�̓w�b�_���R�[�h�ϑ��ҏ��`�F�b�N
        'If mode = 1 Then
        '    '�����ԍ�ALL0�ȊO��ΏۂƂ���
        '    If mInfoComm.INFOToriMast.KOUZA_T <> "0000000" Then
        '        ' �x�X�ԍ��`�F�b�N
        '        If InfoMeisaiMast.ITAKU_SIT <> mInfoComm.INFOToriMast.TSIT_NO_T Then
        '            WriteBLog("ͯ�ގx�X�ԍ�����", "�s��v", "�x�X����:" & InfoMeisaiMast.ITAKU_SIT)
        '            DataInfo.Message = "ͯ�ގx�X�ԍ��s��v �x�X����:" & InfoMeisaiMast.ITAKU_SIT
        '            Return False
        '        End If

        '        ' �a����ڃ`�F�b�N(���ʁE�����̂ݑΏۂƂ���)
        '        If (mInfoComm.INFOToriMast.KAMOKU_T = "01" OrElse mInfoComm.INFOToriMast.KAMOKU_T = "02") AndAlso _
        '            InfoMeisaiMast.ITAKU_KAMOKU <> ConvertKamoku2TO1(mInfoComm.INFOToriMast.KAMOKU_T) Then
        '            WriteBLog("ͯ�ޗa���������", "�s��v", "�a����ځF" & InfoMeisaiMast.ITAKU_KAMOKU)
        '            DataInfo.Message = "ͯ�ޗa����ڕs��v �a����ځF" & InfoMeisaiMast.ITAKU_KAMOKU
        '            Return False
        '        End If

        '        ' �����ԍ��`�F�b�N
        '        If InfoMeisaiMast.ITAKU_KOUZA <> mInfoComm.INFOToriMast.KOUZA_T Then
        '            WriteBLog("ͯ�ތ����ԍ�����", "�s��v", "�����ԍ��F" & InfoMeisaiMast.ITAKU_KOUZA)
        '            DataInfo.Message = "ͯ�ތ����ԍ��s��v �����ԍ��F" & InfoMeisaiMast.ITAKU_KOUZA
        '            Return False
        '        End If
        '    End If

        '    Return True
        'End If
        ' �����s�ԍ��`�F�b�N
        If INI_RSV2_HEAD_BANKCODE = "1" Then
            If InfoMeisaiMast.ITAKU_KIN <> JIKINKO Then
                WriteBLog("ͯ�ގ���������", "�s��v", "���Z�@�ֺ���:" & InfoMeisaiMast.ITAKU_KIN)
                DataInfo.Message = "ͯ�ގ����������s��v ���Z�@�ֺ���:" & InfoMeisaiMast.ITAKU_KIN
                Return False
            End If
        End If

        '���U�̏ꍇ�̓w�b�_���R�[�h�ϑ��ҏ��`�F�b�N
        If mode = 1 Then
            '�����ԍ�ALL0�ȊO��ΏۂƂ���
            If mInfoComm.INFOToriMast.KOUZA_T <> "0000000" Then
                ' �x�X�ԍ��`�F�b�N
                If INI_RSV2_HEAD_S_SITENCODE = "1" Then
                    If InfoMeisaiMast.ITAKU_SIT <> mInfoComm.INFOToriMast.TSIT_NO_T Then
                        WriteBLog("ͯ�ގx�X�ԍ�����", "�s��v", "�x�X����:" & InfoMeisaiMast.ITAKU_SIT)
                        DataInfo.Message = "ͯ�ގx�X�ԍ��s��v �x�X����:" & InfoMeisaiMast.ITAKU_SIT
                        Return False
                    End If
                End If

                ' �a����ڃ`�F�b�N(���ʁE�����̂ݑΏۂƂ���)
                If INI_RSV2_HEAD_S_KAMOKUCODE = "1" Then
                    If (mInfoComm.INFOToriMast.KAMOKU_T = "01" OrElse mInfoComm.INFOToriMast.KAMOKU_T = "02") AndAlso
                        InfoMeisaiMast.ITAKU_KAMOKU <> ConvertKamoku2TO1(mInfoComm.INFOToriMast.KAMOKU_T) Then
                        WriteBLog("ͯ�ޗa���������", "�s��v", "�a����ځF" & InfoMeisaiMast.ITAKU_KAMOKU)
                        DataInfo.Message = "ͯ�ޗa����ڕs��v �a����ځF" & InfoMeisaiMast.ITAKU_KAMOKU
                        Return False
                    End If
                End If

                ' �����ԍ��`�F�b�N
                If INI_RSV2_HEAD_S_KOUZABANGO = "1" Then
                    If InfoMeisaiMast.ITAKU_KOUZA <> mInfoComm.INFOToriMast.KOUZA_T Then
                        WriteBLog("ͯ�ތ����ԍ�����", "�s��v", "�����ԍ��F" & InfoMeisaiMast.ITAKU_KOUZA)
                        DataInfo.Message = "ͯ�ތ����ԍ��s��v �����ԍ��F" & InfoMeisaiMast.ITAKU_KOUZA
                        Return False
                    End If
                End If
            End If

            Return True
        End If
        ' 2015/12/11 �^�X�N�j���� CHG �yPG�zUI_B-14-02(RSV2�Ή�) -------------------- END

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.ITAKU_KIN = mInfoComm.INFOToriMast.TKIN_NO_T     ' �戵���Z�@�փR�[�h
        InfoMeisaiMast.ITAKU_SIT = mInfoComm.INFOToriMast.TSIT_NO_T     ' �戵�x�X�R�[�h
        InfoMeisaiMast.ITAKU_KAMOKU = mInfoComm.INFOToriMast.KAMOKU_T   ' �Ȗ�
        InfoMeisaiMast.ITAKU_KOUZA = mInfoComm.INFOToriMast.KOUZA_T     ' �����ԍ�

        BLOG.ToriCode = mInfoComm.INFOToriMast.TORIS_CODE_T & mInfoComm.INFOToriMast.TORIF_CODE_T

        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        MOTIKOMI_DATE = OraReader.GetString("MOTIKOMI_DATE_S")    '��������
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

        '�}�̃R�[�h�̃`�F�b�N
        Select Case mInfoComm.INFOParameter.BAITAI_CODE
            Case "01"
                If mInfoComm.INFOToriMast.BAITAI_CODE_T <> "01" And mInfoComm.INFOToriMast.BAITAI_CODE_T <> "02" And mInfoComm.INFOToriMast.BAITAI_CODE_T <> "06" Then
                    WriteBLog("�}�̃R�[�h�ُ�", "�G���[����")
                    DataInfo.Message = "�����}�X�^�o�^�ُ�F�}�̃R�[�h"
                    Return False
                End If
            Case "00"
                If mInfoComm.INFOToriMast.BAITAI_CODE_T <> "00" Then
                    WriteBLog("�}�̃R�[�h�ُ�", "�G���[����")
                    DataInfo.Message = "�����}�X�^�o�^�ُ�F�}�̃R�[�h"
                    Return False
                End If
        End Select

        Dim CheckFlag As Boolean = False
        ' �����񎝍�����̏ꍇ�C�X�P�W���[�����쐬����

        If mInfoComm.INFOParameter.FSYORI_KBN = "3" Then

            If mInfoComm.INFOToriMast.KIJITU_KANRI_T = "1" Then
                ' �����Ǘ�����
                If OraReader.GetItem("TYUUDAN_FLG_S") = "0" And
                    (OraReader.GetItem("UKETUKE_FLG_S") = "1" Or OraReader.GetItem("ERROR_INF_S") = "020") Then
                    ' ���łɁC���Ƃ����ݍς݂̏ꍇ
                    If OraReader.GetItem("CYCLE_T") = "1" Then
                        Dim Tuki As String
                        Tuki = "TUKI" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE).ToString("M") & "_T"
                        If OraReader.GetItem(Tuki) = "0" Then
                            WriteBLog("�w�b�_" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & " �����Ώی��ȊO ", "�ُ�", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
                            DataInfo.Message = "�w�b�_" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & " �����Ώی��ȊO �ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO
                            Return False
                        End If

                        ' �����񎝍�����
                        If InsertSCHMAST() = False Then
                            Return False
                        End If
                    Else
                        ' �����񎝍��Ȃ�
                        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetInt("MOTIKOMI_SEQ_S")
                        CheckFlag = True
                    End If
                Else
                    ' ����SEQ�̐ݒ�
                    Dim SEQReader As New CASTCommon.MyOracleReader(OraDB)
                    Dim nGetSEQ As Integer
                    ' ����SEQ�̍ő�l���擾����
                    Dim SQL_SEQ As New StringBuilder
                    SQL_SEQ.AppendLine("SELECT")
                    SQL_SEQ.AppendLine(" NVL(MAX(MOTIKOMI_SEQ_S), 0)  SEQ")
                    SQL_SEQ.AppendLine(" FROM S_SCHMAST")
                    SQL_SEQ.AppendLine(" WHERE TORIS_CODE_S  =" & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                    SQL_SEQ.AppendLine(" AND TORIF_CODE_S  =" & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                    SQL_SEQ.AppendLine(" AND FURI_DATE_S  =" & SQ(InfoMeisaiMast.FURIKAE_DATE))

                    Dim nRet As Long = 0
                    If InfoMeisaiMast.FILE_SEQ = 0 AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
                        ' ����SEQ�J�E���gSQL���s
                        If SEQReader.DataReader(SQL_SEQ) = True Then
                            nGetSEQ = SEQReader.GetValueInt(0)
                        Else
                            WriteBLog("�X�P�W���[���쐬", "���s", OraDB.Message & ":" & SQL_SEQ.ToString)
                            Return False
                        End If
                        SEQReader.Close()
                    Else
                        nGetSEQ = InfoMeisaiMast.MOTIKOMI_SEQ
                    End If

                    InfoMeisaiMast.MOTIKOMI_SEQ = nGetSEQ

                    CheckFlag = True
                End If
            Else
                ' �����Ǘ��Ȃ��̏ꍇ
                If OraReader.GetItem("CYCLE_T") = "1" Then
                    ' �����񎝍�����
                    If InsertSCHMAST() = False Then
                        '�G���[���e����̏ꍇ�̂�
                        If DataInfo.Message = "" Then
                            WriteBLog("����̧�ٓ��ɓ����ϑ��ҁC" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�̃f�[�^������", "�ُ�", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO)
                            DataInfo.Message = "����̧�ٓ��ɓ����ϑ���" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "���ް������� �ϑ��Һ���:" & InfoMeisaiMast.ITAKU_CODE & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & InfoMeisaiMast.FURIKAE_DATE_MOTO
                        End If

                        Return False
                    End If
                Else
                    ' �����񎝍��Ȃ�
                    If OraReader.GetItem("TYUUDAN_FLG_S") = "" Then
                        ' �X�P�W���[���������ꍇ�ɍ쐬����
                        If InsertSCHMAST() = False Then
                            Return False
                        End If
                    Else
                        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetInt("MOTIKOMI_SEQ_S")
                        CheckFlag = True
                    End If
                End If
            End If

        Else
            CheckFlag = True
        End If

        If CheckFlag = True Then
            If OraReader.IsNull("TYUUDAN_FLG_S") = True Then
                ' �����Ǘ�����ꍇ�ŁC�X�P�W���[���}�X�^�������ꍇ�C�G���[
                WriteBLog("�t�@�C���w�b�_�X�P�W���[������", "���s", "�ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
                DataInfo.Message = "���ޭ�ٌ������s �ϑ��Һ���:" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
                Return False
            End If

            If OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
                WriteBLog("�X�P�W���[��:���f�t���O�ݒ�� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"), "���f")
                DataInfo.Message = "���f�׸ސݒ�� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
                Return False
            End If

            '2017/12/04 �^�X�N�j���� CHG �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ START
            If mInfoComm.INFOToriMast.SYOUGOU_KBN_T = "1" Then
                ' �ƍ�����̏ꍇ�A��t�ς݃t���O�ɂĔ���
                If OraReader.GetItem("UKETUKE_FLG_S") <> "0" Then
                    WriteBLog("�X�P�W���[��:�������ݏ����� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T, "���f")
                    DataInfo.Message = "�������ݏ����� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��" & " �����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
                    Return False
                End If
            Else
                ' �`���C�e�c�C�b�l�s�̏ꍇ�i�ƍ��Ȃ��j �o�^�σt���O�ɂĔ���
                If OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
                    WriteBLog("�X�P�W���[��:�������ݏ����� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T, "���f")
                    DataInfo.Message = "�������ݏ����� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��" & " �����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
                    Return False
                End If
            End If
            '' �`���C�e�c�C�b�l�s�̏ꍇ�i�ƍ��Ȃ��j �o�^�σt���O�ɂĔ���
            'If OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
            '    WriteBLog("�X�P�W���[��:�������ݏ����� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��") & " �����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T, "���f")
            '    DataInfo.Message = "�������ݏ����� " & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��" & " �����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
            '    Return False
            'End If
            '2017/12/04 �^�X�N�j���� CHG �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ END

        End If

        ' �I���N��Reader�N���[�Y
        OraReader.Close()

        '�t�H�[�}�b�g�敪�擾
        If mInfoComm.INFOToriMast.FMT_KBN_T = "20" Or mInfoComm.INFOToriMast.FMT_KBN_T = "21" Then  '�W����s�T�[�r�X�̎�
            '2017/01/16 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
            If mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "0" Then
                If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim = "" Then
                    '�J�i�E�v���ݒ肳��Ă��Ȃ��ƃG���[
                    WriteBLog("�œE�v", "���ݒ�", "�ϑ��Һ���:" & InfoMeisaiMast.ITAKU_CODE)
                    DataInfo.Message = "�œE�v���ݒ�@����溰��:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
                    Return False
                End If

            ElseIf mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
                '�x�X�E�v��OK

            Else
                '��L�ȊO��NG
                WriteBLog("�E�v�敪", "�ݒ�~�X", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE)
                DataInfo.Message = "�E�v�敪���u�Łv�u�x�X�v�ɐݒ肵�Ă��������@����溰��:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
                Return False
            End If

            'If mInfoComm.INFOToriMast.TEKIYOU_KBN_T <> "0" Then '�œE�v�ȊO�̓G���[�Ƃ���
            '    WriteBLog("�E�v�敪", "�ݒ�~�X", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE)
            '    DataInfo.Message = "�E�v�敪���u�Łv�ɐݒ肵�Ă��������@����溰��:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
            '    Return False
            'End If
            ''�œE�v���ݒ肳��Ă��Ȃ��ƃG���[
            'If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim = "" Then
            '    WriteBLog("�œE�v", "���ݒ�", "�ϑ��Һ���:" & InfoMeisaiMast.ITAKU_CODE)
            '    DataInfo.Message = "�œE�v���ݒ�@����溰��:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
            '    Return False
            'Else
            '    '�W����s�T�[�r�X�p�̓E�v�ɕҏW
            '    '' ���׃}�X�^���ڐݒ� �E�v
            '    If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Length > 10 Then
            '        InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Substring(0, 10)
            '    Else
            '        InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim
            '    End If
            'End If
            '2017/01/16 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
        End If
        WriteBLog("�w�b�_����挟��", "����", "����溰��:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
        Select Case mInfoComm.INFOParameter.FMT_KBN
            Case "02"     '���Ńt�H�[�}�b�g
            Case Else     '���Ńt�H�[�}�b�g�ȊO
                '2018/09/24 saitou �L���M��(RSV2�Ή�) UPD �y���s�z�i���U�ϑ��҃R�[�h�`�F�b�N�ہj ---------- START
                If mInfoComm.INFOToriMast.FSYORI_KBN_T = "1" Then
                    If InfoMeisaiMast.ITAKU_CODE <> mInfoComm.INFOToriMast.ITAKU_CODE_T Then
                        ' �ϑ��҃R�[�h�s��v
                        WriteBLog("�w�b�_�ϑ��҃R�[�h", "�s��v", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE)
                        DataInfo.Message = "ͯ�ވϑ��Һ��ޕs��v:" & InfoMeisaiMast.ITAKU_CODE
                        Return False
                    End If
                Else
                    If INI_RSV2_HEAD_S_ITAKUCODE = "1" Then
                        If InfoMeisaiMast.ITAKU_CODE <> mInfoComm.INFOToriMast.ITAKU_CODE_T Then
                            ' �ϑ��҃R�[�h�s��v
                            WriteBLog("�w�b�_�ϑ��҃R�[�h", "�s��v", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE)
                            DataInfo.Message = "ͯ�ވϑ��Һ��ޕs��v:" & InfoMeisaiMast.ITAKU_CODE
                            Return False
                        End If
                    End If
                End If
                'If InfoMeisaiMast.ITAKU_CODE <> mInfoComm.INFOToriMast.ITAKU_CODE_T Then
                '    ' �ϑ��҃R�[�h�s��v
                '    WriteBLog("�w�b�_�ϑ��҃R�[�h", "�s��v", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE)
                '    DataInfo.Message = "ͯ�ވϑ��Һ��ޕs��v:" & InfoMeisaiMast.ITAKU_CODE
                '    Return False
                'End If
                '2018/09/24 saitou �L���M��(RSV2�Ή�) UPD --------------------------------------------------- END
        End Select

        '���ׂ̐U�֓��ƃp�����[�^�̐U�֓��̃`�F�b�N
        If InfoMeisaiMast.FURIKAE_DATE <> mInfoComm.INFOParameter.FURI_DATE Then
            '�U�֓��s��v
            '�U�֓�������Ă���ƈُ�I������ꍇ
            WriteBLog("�t�@�C���w�b�_" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "�s��v", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
            DataInfo.Message = "ͯ��" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�s��v:" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyy�NMM��dd��")
            Return False
        End If

        ' ���o���敪 ��ʃR�[�h�`�F�b�N
        Select Case mInfoComm.INFOToriMast.NS_KBN_T
            Case "1"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "11", "12", "21", "41", "43", "44", "45", "71", "72"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_���o���敪�A���", "�s��v", "��ʁF" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ޓ��o���敪�A��ʕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "9"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "91"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_���o���敪�A���", "�s��v", "��ʁF" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ޓ��o���敪�A��ʕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
        End Select

        '  ��ʃR�[�h�`�F�b�N
        Select Case mInfoComm.INFOToriMast.SYUBETU_T
            Case "91"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "91"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "21"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "21", "41", "43", "44", "45"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "12"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "12"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "11"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "11"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "71"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "71"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
            Case "72"
                Select Case InfoMeisaiMast.SYUBETU_CODE
                    Case "72"
                        'OK
                    Case Else
                        'NG
                        WriteBLog("�t�@�C���w�b�_��ʃR�[�h", "�s��v", "��ʃR�[�h�F" & InfoMeisaiMast.SYUBETU_CODE)
                        DataInfo.Message = "ͯ�ގ�ʺ��ޕs��v:" & InfoMeisaiMast.SYUBETU_CODE
                        Return False
                End Select
        End Select

        Return True
    End Function

    '
    ' �@�\�@ �F �f�[�^���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' ���l�@ �F2010/10/07.Sakon�@�C���v�b�g�G���[���Ɍ��ʃR�[�h���Z�b�g���邩��INI�t�@�C����蔻�肷��
    '
    Protected Overridable Function CheckDataRecord() As Boolean
        Dim InError As INPUTERROR = Nothing

        InError.DATA = InfoMeisaiMast
        InErrorArray = New ArrayList

        If mInfoComm Is Nothing Then
            Return True
        End If

        '�x�X�E�����Ǒ֑Ή�(�ύX�O���̕ێ�)
        InfoMeisaiMast.OLD_KIN_NO = InfoMeisaiMast.KEIYAKU_KIN
        InfoMeisaiMast.OLD_SIT_NO = InfoMeisaiMast.KEIYAKU_SIT
        InfoMeisaiMast.OLD_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA

        '2010.02.09 �U�֌��ʂ�����͎̂�ʂX�P�݂̂Ƃ���
        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" AndAlso
            InfoMeisaiMast.FURIKETU_CODE <> 0 AndAlso mInfoComm.INFOToriMast.NS_KBN_T = "9" Then
            ' �U�֌��ʃR�[�h�ُ�
            InError.ERRINFO = Err.Name(Err.InputErrorType.FuriketuCode)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " �U�֌��ʃR�[�h�ُ�F" & InfoMeisaiMast.FURIKETU_CODE
        ElseIf mInfoComm.INFOToriMast.NS_KBN_T = "1" Then   '�U���w��敪�Ή� 2010.02.09
            InfoMeisaiMast.FURIKETU_CODE = 0
        End If

        '�K��O�����`�F�b�N���C���v�b�g�G���[�Ƃ��ďo��
        Dim kiteiRem As Long = CheckRegularString()

        If kiteiRem <> -1 Then
            ' �K��O�����ُ�
            If SouInputErr <> "0" Then
                InfoMeisaiMast.FURIKETU_CODE = 9
            End If
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kiteigaimoji)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " �K��O�����F" & kiteiRem & "�o�C�g��"
        End If

        '���Z�@�փ}�X�^���݃`�F�b�N�t���O�ǉ�
        '���Z�@�փ}�X�^���݃`�F�b�N���s�����ǂ����𔻒肷��t���O
        Dim TenMastExistCheck_Flg As Boolean = True

        '���Z�@�փR�[�h���l�`�F�b�N
        If IsDecimal(InfoMeisaiMast.KEIYAKU_KIN) = False OrElse
            InfoMeisaiMast.KEIYAKU_KIN.Equals("0000") = True OrElse
            InfoMeisaiMast.KEIYAKU_KIN.Equals("9999") = True Then
            ' ��s�R�[�h���l�ُ�
            If SouInputErr <> "0" Then
                InfoMeisaiMast.FURIKETU_CODE = 9
            End If
            '��s�R�[�h�ُ펞�̃��b�Z�[�W��ύX(�X�Ԉُ큨��s�R�[�h�ُ�)
            InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN

            '���l�ُ�̏ꍇ�A�t���O��FALSE�ɂ��鏈����ǉ�
            TenMastExistCheck_Flg = False

        ElseIf IsDecimal(InfoMeisaiMast.KEIYAKU_SIT) = False OrElse
            InfoMeisaiMast.KEIYAKU_SIT.Equals("999") = True Then

            '�䂤�����s�̏ꍇ��"000"��"999"���ُ�Ƃ��Ȃ�
            If InfoMeisaiMast.KEIYAKU_KIN.Equals("9900") = False Then
                ' �X�Ԑ��l�ُ�
                If SouInputErr <> "0" Then
                    InfoMeisaiMast.FURIKETU_CODE = 9
                End If
                InError.ERRINFO = Err.Name(Err.InputErrorType.Tenban)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " �x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT

                '���l�ُ�̏ꍇ�A�t���O��FALSE�ɂ��鏈����ǉ�
                TenMastExistCheck_Flg = False
            End If
        End If

        Select Case mInfoComm.INFOToriMast.SYUBETU_T
            Case "91"
                '�Ȗڃ`�F�b�N��9������
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 0
                        '���Ńt�H�[�}�b�g��0������
                        If mInfoComm.INFOToriMast.FMT_KBN_T <> "02" Then
                            ' �Ȗڈُ�
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                    Case 1, 2, 3, 9
                    Case Else
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- START
                        '' �Ȗڈُ�
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 9
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_91").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                            ' �Ȗڈُ�
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- END
                End Select
            Case "11", "12"
                '�Ȗڃ`�F�b�N��9������
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 9
                    Case Else
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- START
                        '' �Ȗڈُ�
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 9
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        Select Case mInfoComm.INFOToriMast.SYUBETU_T
                            Case "11"
                                If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_11").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' �Ȗڈُ�
                                    If SouInputErr <> "0" Then
                                        InfoMeisaiMast.FURIKETU_CODE = 9
                                    End If
                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                            Case "12"
                                If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_12").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                                    ' �Ȗڈُ�
                                    If SouInputErr <> "0" Then
                                        InfoMeisaiMast.FURIKETU_CODE = 9
                                    End If
                                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                                    InErrorArray.Add(InError)
                                    DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                                End If
                        End Select
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- END
                End Select
            Case "21"
                Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
                    Case 1, 2, 4, 9
                    Case Else
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- START
                        '' �Ȗڈُ�
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 9
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                        'InErrorArray.Add(InError)
                        '' 2008.04.22 ADD
                        'DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        If CASTCommon.GetRSKJIni("FORMAT", "J_KEIYAKUKAMOKU_21").IndexOf(InfoMeisaiMast.KEIYAKU_KAMOKU) < 0 Then
                            ' �Ȗڈُ�
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.Kamoku)
                            InErrorArray.Add(InError)
                            ' 2008.04.22 ADD
                            DataInfo.Message = InError.ERRINFO & " �ȖځF" & InfoMeisaiMast.KEIYAKU_KAMOKU
                        End If
                        '2017/12/18 �^�X�N�j���� CHG ����M���@�\�W���K�p(UI_5-01,5-11<PG>) -------------------- END
                End Select
        End Select

        ' �V�K�R�[�h�`�F�b�N
        '�V�K�R�[�h�ɋ󔒂�����
        '2010/09/08.Sakon�@�V�K�R�[�h�`�F�b�N���s�����ۂ����h�m�h�t�@�C���Ɏw�肷�� ++++++++++++++++++++++
        If mSinkiCheckFlag = "1" Then
            '***�C�� maeda 2008/05/15****************************************************************
            '�V�K�R�[�h�ɋ󔒂�����
            Select Case InfoMeisaiMast.SINKI_CODE
                Case " ", "0", "1", "2"
                Case Else
                    ' �V�K�R�[�h�ُ�
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.SinkiCode)
                    InErrorArray.Add(InError)
                    ' 2008.04.22 ADD
                    DataInfo.Message = InError.ERRINFO & " �V�K�R�[�h�F" & InfoMeisaiMast.SINKI_CODE
            End Select
            'Select Case InfoMeisaiMast.SINKI_CODE
            '    Case " ", "0", "1", "2"
            '    Case Else
            '        ' �V�K�R�[�h�ُ�
            '        InfoMeisaiMast.FURIKETU_CODE = 9
            '        InError.ERRINFO = Err.Name(Err.InputErrorType.SinkiCode)
            '        InErrorArray.Add(InError)
            '        ' 2008.04.22 ADD
            '        DataInfo.Message = InError.ERRINFO & " �V�K�R�[�h�F" & InfoMeisaiMast.SINKI_CODE
            'End Select
            '****************************************************************************************
        End If
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        Dim KouzaCheck As Boolean = True

        '�����ԍ����̃n�C�t���͏Ȃ��A��0���߂���
        InfoMeisaiMast.KEIYAKU_KOUZA = InfoMeisaiMast.KEIYAKU_KOUZA.Replace("-"c, "").PadLeft(7, "0"c)
        '�Ȗڂ�9�������ԍ���ALL0����ALL9�̎��͌����`�F�b�N�Ȃ�
        '���łŉȖڂ�0�������ԍ���ALL0�̎��͌����`�F�b�N�Ȃ�
        Select Case CAInt32(InfoMeisaiMast.KEIYAKU_KAMOKU)
            Case 0
                If mInfoComm.INFOToriMast.FMT_KBN_T = "02" AndAlso InfoMeisaiMast.KEIYAKU_KOUZA.Trim = "0000000" Then
                    KouzaCheck = False
                End If

            Case 9
                '���U�̏ꍇ��ALL0�̌����`�F�b�N������
                Select Case InfoMeisaiMast.KEIYAKU_KOUZA.Trim
                    Case "0000000"
                        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
                            KouzaCheck = False
                        End If

                    Case "9999999"
                        KouzaCheck = False
                End Select
        End Select

        '�����ԍ��`�F�b�N�f�B�W�b�g�`�F�b�N���������AKOUZACHECK�t���O���Q�Ƃ���悤�C��
        If KouzaCheck = True Then
            If mCheckDigitFlag = "1" Then
                '�����ԍ��`�F�b�N�f�W�b�g�`�F�b�N
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    If CheckDigitCheck() = False Then
                        ' �����ԍ��ُ�
                        If SouInputErr <> "0" Then
                            InfoMeisaiMast.FURIKETU_CODE = 9
                        End If
                        InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                        KouzaCheck = False
                    End If
                End If
            End If
        End If

        If KouzaCheck = True Then
            If mInfoComm.INFOToriMast.FMT_KBN_T = "02" Then
                ' ���� �擪�X�y�[�X��OK
                If IsDecimal(InfoMeisaiMast.KEIYAKU_KOUZA.TrimStart) = False Then
                    ' �����ԍ��ُ�
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
                End If
            Else
                If IsDecimal(InfoMeisaiMast.KEIYAKU_KOUZA) = False Then
                    ' �����ԍ��ُ�
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
                End If
            End If
        End If

        '�����ԍ�ALL0�͐U�֌��ʃR�[�h2���Z�b�g����
        If KouzaCheck = True AndAlso (InfoMeisaiMast.KEIYAKU_KOUZA = "0000000" OrElse InfoMeisaiMast.KEIYAKU_KOUZA = "9999999") Then
            ' �����ԍ��ُ�
            Select Case InfoMeisaiMast.KEIYAKU_KOUZA
                Case "0000000"
                    If SouInputErr <> "0" Then
                        '���Z���^�̏ꍇ�͒�~���Ȃ�
                        If CASTCommon.GetFSKJIni("COMMON", "CENTER") <> "5" Then
                            InfoMeisaiMast.FURIKETU_CODE = 2
                            InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        End If
                    End If

                    InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                    KouzaCheck = False
                Case "9999999"
                    '���U�̏ꍇ��ALL9���G���[�Ƃ��Ȃ�
                    If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "3" Then
                        If SouInputErr <> "0" Then
                            '���Z���^�̏ꍇ�͒�~���Ȃ�
                            If CASTCommon.GetFSKJIni("COMMON", "CENTER") <> "5" Then
                                InfoMeisaiMast.FURIKETU_CODE = 9
                            End If
                        End If

                        InError.ERRINFO = Err.Name(Err.InputErrorType.Kouza)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " �����ԍ��F" & InfoMeisaiMast.KEIYAKU_KOUZA
                        KouzaCheck = False
                    End If
            End Select
        End If

        '�x�X�ǂݑւ�
        With InfoMeisaiMast
            If SitenYomikae = "1" Then
                '�x�X�ǂݑւ��Ώ�
                Call fn_TENPO_YOMIKAE(.KEIYAKU_KIN, .KEIYAKU_SIT, .KEIYAKU_KIN, .KEIYAKU_SIT)
            End If
        End With

        '���s�f�[�^�쐬��Ώۂ̏ꍇ�A�����Ƀ`�F�b�N
        '120byte�i�W����s�Ή�:���s�f�[�^���o�^�ł���悤�ɂ���j
        If mInfoComm.INFOToriMast.FMT_KBN_T <> "20" AndAlso mInfoComm.INFOToriMast.FMT_KBN_T <> "21" Then
            ' �W����s�t�H�[�}�b�g�ł͖����ꍇ
            If mInfoComm.INFOToriMast.TAKO_KBN_T = "0" And
                InfoMeisaiMast.KEIYAKU_KIN <> "0000" And InfoMeisaiMast.KEIYAKU_KIN <> "9999" Then  '���s�敪��Ώ� 
                If InfoMeisaiMast.KEIYAKU_KIN <> JIKINKO Then
                    ' ���s�ُ�
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.Takou)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN
                End If
            End If
        Else
            If mInfoComm.INFOToriMast.FMT_KBN_T = "20" Then
                ' �r�r�r��g�� ���s�ُ�
                Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT)
                If Not OraReader Is Nothing AndAlso OraReader.GetItem("TEIKEI_KBN_N") <> "1" Then
                    ' ��g�O�̋��Z�@�ւ̏ꍇ�C�r�r�r��g�O
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 9
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.TeikeiGai)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN
                    OraReader.Close()
                End If
            End If
        End If

        If mInfoComm.INFOToriMast.TAKO_KBN_T = "1" Then
            If InfoMeisaiMast.KEIYAKU_KIN <> JIKINKO Then
                TAKOU_ON = True             '���s�L��
            End If
        End If


        '�����ǂݑւ�
        With InfoMeisaiMast
            If KouzaYomikae = "1" AndAlso JIKINKO = .KEIYAKU_KIN Then
                '�x�X�ǂݑւ��Ώ�
                Call fn_KOUZA_YOMIKAE(.KEIYAKU_SIT, .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA,
                                                .KEIYAKU_SIT, .KEIYAKU_KOUZA, .IDOU_DATE)
            End If
        End With

        '���Z�@�փR�[�h���݃`�F�b�N
        Dim nRet As Integer
        If TenMastExistCheck_Flg = True Then
            nRet = GetTENMASTExists(InfoMeisaiMast.KEIYAKU_KIN, InfoMeisaiMast.KEIYAKU_SIT, InfoMeisaiMast.FURIKAE_DATE)
        Else '���Z�@�փR�[�h���l�`�F�b�N���s
            nRet = 9
        End If

        '���Z�@�֎擾�������̃G���[�����ύX(�ȉ��̒ʂ�)
        '===================================================================
        '0:���Z�@�֎擾���s(GetTENMASTExist�ŗ�O����)
        '1:���Z�@�ւ���x�X�Ȃ�
        '2:���Z�@�ւ���x�X����(����I��)
        '3:�U�֓����폜������(�X�ܓ��p��)
        '9:���Z�@�փR�[�h���l�`�F�b�N���s
        '===================================================================
        Select Case nRet
            Case 0 '���Z�@�ւȂ��̏ꍇ

                '���Z�@�ւȂ�
                '2018/10/16 saitou �L���M��(RSV2�W��) UPD �i���Z�@�փ}�X�^�`�F�b�N�j ------------------------------ START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "GINKOUCODE") = "1" Then
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 2
                        InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                End If
                'If SouInputErr <> "0" Then
                '    InfoMeisaiMast.FURIKETU_CODE = 2
                '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                'End If
                'InError.ERRINFO = Err.Name(Err.InputErrorType.GinkouCode)
                'InErrorArray.Add(InError)
                'DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                '2018/10/16 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END

            Case 1 '���Z�@�ւ���C�x�X�Ȃ�
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO Then
                    '���s�Ŏx�X�Ȃ��̏ꍇ�͎��s�X�Ԉُ�
                    '2018/10/05 saitou �L���M��(RSV2�W��) UPD �i���Z�@�փ}�X�^�`�F�b�N�j ------------------------------ START
                    If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "JIKOUTENBAN") = "1" Then
                        If SouInputErr <> "0" Then
                            InfoMeisaiMast.FURIKETU_CODE = 2
                            InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        End If
                        InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                    End If
                    'If SouInputErr <> "0" Then
                    '    InfoMeisaiMast.FURIKETU_CODE = 2
                    '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    'End If
                    'InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    'InErrorArray.Add(InError)
                    'DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                    '2018/10/05 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END

                    '�x�X�Ȃ��̏ꍇ�͑��s�X�Ԉُ�
                Else
                    '���s�Ŏx�X�Ȃ��̏ꍇ�͑��s�X�Ԉُ�
                    'SSS�ł䂤�����s�̏ꍇ�͑��s�X�Ԉُ�Ƃ��Ȃ�
                    If Not ((mInfoComm.INFOToriMast.FMT_KBN_T = "20" Or mInfoComm.INFOToriMast.FMT_KBN_T = "21") AndAlso InfoMeisaiMast.KEIYAKU_KIN = "9900") Then
                        '2018/10/15 saitou �L���M��(RSV2�W��) UPD �i���Z�@�փ}�X�^�`�F�b�N�j ------------------------------ START
                        If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TAKOUTENBAN") = "1" Then
                            If SouInputErr <> "0" Then
                                InfoMeisaiMast.FURIKETU_CODE = 2
                                InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                            End If
                            InError.ERRINFO = Err.Name(Err.InputErrorType.TakouTenban)
                            InErrorArray.Add(InError)
                            DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                        End If
                        'If SouInputErr <> "0" Then
                        '    InfoMeisaiMast.FURIKETU_CODE = 2
                        '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        'End If
                        'InError.ERRINFO = Err.Name(Err.InputErrorType.TakouTenban)
                        'InErrorArray.Add(InError)
                        'DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                        '2018/10/05 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
                    End If
                End If
            Case 2 '���Z�@�ւ���C�x�X����
                '���s�œX��000�̏ꍇ�͎��s�X�Ԉُ�
                If InfoMeisaiMast.KEIYAKU_KIN = JIKINKO AndAlso
                   InfoMeisaiMast.KEIYAKU_SIT = "000" Then

                    '2018/10/15 saitou �L���M��(RSV2�W��) UPD �i���Z�@�փ}�X�^�`�F�b�N�j ------------------------------ START
                    If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "JIKOUTENBAN") = "1" Then
                        If SouInputErr <> "0" Then
                            InfoMeisaiMast.FURIKETU_CODE = 2
                            InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                        End If
                        InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                        InErrorArray.Add(InError)
                        DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                    End If
                    'If SouInputErr <> "0" Then
                    '    InfoMeisaiMast.FURIKETU_CODE = 2
                    '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    'End If
                    'InError.ERRINFO = Err.Name(Err.InputErrorType.JikouTenban)
                    'InErrorArray.Add(InError)
                    'DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                    '2018/10/05 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
                End If

                '����I���̂��ߏ����Ȃ�

            Case 3 '�U�֓����폜������(�X�ܓ��p��)
                '2018/10/15 saitou �L���M��(RSV2�W��) UPD �i���Z�@�փ}�X�^�`�F�b�N�j ------------------------------ START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TENPOTOUGOU") = "1" Then
                    If SouInputErr <> "0" Then
                        InfoMeisaiMast.FURIKETU_CODE = 2
                        InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                    End If
                    InError.ERRINFO = Err.Name(Err.InputErrorType.TenpoTougou)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                End If
                'If SouInputErr <> "0" Then
                '    InfoMeisaiMast.FURIKETU_CODE = 2
                '    InfoMeisaiMast.FURIKETU_CENTERCODE = "86"
                'End If
                'InError.ERRINFO = Err.Name(Err.InputErrorType.TenpoTougou)
                'InErrorArray.Add(InError)
                'DataInfo.Message = InError.ERRINFO & " ���Z�@�փR�[�h�F" & InfoMeisaiMast.KEIYAKU_KIN & "�x�X�R�[�h�F" & InfoMeisaiMast.KEIYAKU_SIT
                '2018/10/05 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            Case 9 '���Z�@�փR�[�h���l�`�F�b�N�Ŏ��s�����ꍇ
            Case Else '��O

        End Select

        '���U�Ŏ��l�����󔒂̏ꍇ�̓G���[�Ƃ���
        If mInfoComm.INFOToriMast.FSYORI_KBN_T = "3" AndAlso InfoMeisaiMast.KEIYAKU_KNAME.Trim = "" Then
            InfoMeisaiMast.FURIKETU_CODE = 9

            InError.ERRINFO = Err.Name(Err.InputErrorType.Kana)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " ���l���Ȃ�"
        End If

        '���z�`�F�b�N
        If IsDecimal(InfoMeisaiMast.FURIKIN_MOTO) = False Then
            If SouInputErr <> "0" Then
                InfoMeisaiMast.FURIKETU_CODE = 9
            End If
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
            InErrorArray.Add(InError)
            DataInfo.Message = InError.ERRINFO & " ���z�F" & InfoMeisaiMast.FURIKIN_MOTO
        Else
            If InfoMeisaiMast.FURIKIN < 0 Then
                ' �}�C�i�X���z
                If SouInputErr <> "0" Then
                    InfoMeisaiMast.FURIKETU_CODE = 9
                End If
                InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
                InErrorArray.Add(InError)
                DataInfo.Message = InError.ERRINFO & " ���z�F" & InfoMeisaiMast.FURIKIN_MOTO
            End If

            If InfoMeisaiMast.FURIKIN = 0 Then
                InfoMeisaiMast.FURIKETU_CODE = 0
                'NHK�̓C���v�b�g�G���[�ɂ̂��Ȃ�
                If mInfoComm.INFOToriMast.FMT_KBN_T <> "01" Then
                    InError.ERRINFO = Err.Name(Err.InputErrorType.KingakuZero)
                    InErrorArray.Add(InError)
                    DataInfo.Message = InError.ERRINFO & " ���z�F" & InfoMeisaiMast.FURIKIN_MOTO
                End If
            End If
        End If

        InErrorArray.TrimToSize()

        If InErrorArray.Count > 0 Then
            Return False
        End If
        Return True
    End Function
    '2010/02/03 ���������� ========================
    Function fn_TENPO_YOMIKAE(ByVal astrKIN_NO As String, ByVal astrSIT_NO As String, ByRef astrNEW_KIN_NO As String, ByRef astrNEW_SIT_NO As String) As Boolean
        '=====================================================================================
        'NAME           :fn_TENPO_YOMIKAE
        'Parameter      :astrKIN_NO�F���Z�@�փR�[�h�^astrSIT_NO�F�x�X�R�[�h�^
        '               :astrNEW_KIN_NO�F�ǂݑւ�����Z�@�փR�[�h�^astrNEW_SIT_NO�F�ǂݑւ���x�X�R�[�h
        'Description    :YOMIKAEMAST����X�ܓǂݑւ����s��
        'Return         :�ǂݑւ���̋��Z�@�փR�[�h�A�x�X�R�[�h�ATrue=OK(�ǂݑւ��ς�),False=NG�i���ǂݑւ��j
        'Create         :2010/02/03
        'Update         :
        '=====================================================================================
        fn_TENPO_YOMIKAE = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            SQL.Append("SELECT NEW_KIN_NO_S, NEW_SIT_NO_S FROM SITENYOMIKAE")
            SQL.Append(" WHERE OLD_KIN_NO_S = " & SQ(astrKIN_NO))
            SQL.Append(" AND OLD_SIT_NO_S = " & SQ(astrSIT_NO))

            If OraReader.DataReader(SQL) Then
                astrNEW_KIN_NO = OraReader.GetString("NEW_KIN_NO_S")
                astrNEW_SIT_NO = OraReader.GetString("NEW_SIT_NO_S")
                fn_TENPO_YOMIKAE = True
            Else
                astrNEW_KIN_NO = astrKIN_NO
                astrNEW_SIT_NO = astrSIT_NO
            End If

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function
    Function fn_KOUZA_YOMIKAE(ByVal astrSIT_NO As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String,
                          ByRef astrNEW_SIT_NO As String, ByRef astrNEW_KOUZA As String, ByRef astrIDOU_DATE As String) As Boolean
        '=====================================================================================
        'NAME           :fn_KOUZA_YOMIKAE
        'Parameter      :astrKIN_NO�F���Z�@�փR�[�h�^astrSIT_NO�F�x�X�R�[�h�^astrKAMOKU�F�Ȗ�
        '               :astrKOUZA�F�����ԍ�
        'Description    :KDBMAST��������ǂݑւ����s��
        'Return         :�ǂݑւ���̎x�X�R�[�h�A�����ԍ��ATrue=OK(�ǂݑւ��ς�),False=NG�i���ǂݑւ��j
        'Create         :2010/02/03
        'Update         :
        '=====================================================================================
        fn_KOUZA_YOMIKAE = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            astrKAMOKU = CASTCommon.ConvertKamoku1TO2(astrKAMOKU)

            SQL.Append("SELECT TSIT_NO_D, KOUZA_D, IDOU_DATE_D FROM KDBMAST")
            SQL.Append(" WHERE OLD_TSIT_NO_D = '" & astrSIT_NO & "'")
            SQL.Append(" AND KAMOKU_D = '" & astrKAMOKU & "'")
            SQL.Append(" AND OLD_KOUZA_D = '" & astrKOUZA & "'")

            If OraReader.DataReader(SQL) Then
                astrNEW_SIT_NO = OraReader.GetString("TSIT_NO_D").PadLeft(3, "0"c)
                astrNEW_KOUZA = OraReader.GetString("KOUZA_D").PadLeft(7, "0"c)
                astrIDOU_DATE = OraReader.GetString("IDOU_DATE_D")
                fn_KOUZA_YOMIKAE = True
            Else
                astrNEW_SIT_NO = astrSIT_NO.PadLeft(3, "0"c)
                astrNEW_KOUZA = astrKOUZA.PadLeft(7, "0"c)
                astrIDOU_DATE = ""
            End If

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function
    '==================================================

    '
    ' �@�\�@ �F �g���[���[���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' ���l�@ �F
    '
    Protected Function CheckTrailerRecord() As Boolean
        '�˗��������v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[������", "�ُ�", "�����F" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO)
            DataInfo.Message = "̧���ڰ׌����ُ� ����:" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO
            Return False
        End If

        '�˗����z���v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[�����z", "�ُ�", "���z�F" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO)
            DataInfo.Message = "̧���ڰ׋��z�ُ� ���z:" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO
            Return False
        End If

        '2009/12/03 �g���[���`�F�b�N�ɁA0�~���������ꍇ��ǉ� ==============
        'If InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN Then
        If InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN AndAlso
            InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN2 Then
            '===============================================================
            WriteBLog("�t�@�C���g���[������", "�s��v", "�����F" & InfoMeisaiMast.TOTAL_KEN.ToString)
            '2010/10/20 �g���[�������s��v���̃��b�Z�[�W�����z�s��v�̏ꍇ�Ɠ��l�ɏW�v�l�A�g���[���̕�����\��������-------------------------
            'DataInfo.Message = "̧���ڰ׌����s��v ����:" & InfoMeisaiMast.TOTAL_KEN.ToString & "," & InfoMeisaiMast.TOTAL_IRAI_KEN.ToString
            DataInfo.Message = "̧���ڰ׌����s��v ����(�W�v�l):" & InfoMeisaiMast.TOTAL_KEN.ToString & " ����(�g���[��):" & InfoMeisaiMast.TOTAL_IRAI_KEN.ToString
            '--------------------------------------------------------------------------------------------------------------------------------
            Return False
        End If

        '***�C�� maeda 2008/05/16********************************************************************************************************************************************
        '�g���[�����z�s��v���̃��b�Z�[�W�ɏW�v���z�ƁA���g���[���̋��z��\������悤�ɕύX
        If InfoMeisaiMast.TOTAL_IRAI_KIN <> InfoMeisaiMast.TOTAL_KIN Then
            WriteBLog("�t�@�C���g���[�����z", "�s��v", "���z�F" & InfoMeisaiMast.TOTAL_KIN.ToString)
            DataInfo.Message = "̧���ڰ׋��z�s��v ���z(�W�v�l):" & InfoMeisaiMast.TOTAL_KIN.ToString & "�@���z(�g���[��):" & InfoMeisaiMast.TOTAL_IRAI_KIN.ToString
            Return False
        End If

        'If InfoMeisaiMast.TOTAL_IRAI_KIN <> InfoMeisaiMast.TOTAL_KIN Then
        '    WriteBLog("�t�@�C���g���[�����z", "�s��v", "���z�F" & InfoMeisaiMast.TOTAL_KIN.ToString)
        '    DataInfo.Message = "�t�@�C���g���[�����z�s��v ���z:" & InfoMeisaiMast.TOTAL_KIN.ToString
        '    Return False
        'End If

        '********************************************************************************************************************************************************************

        If DataInfo.RecoedLen <> RecordData.Length Then
            WriteBLog("�g���[�����R�[�h��", "�s��v", "���R�[�h���F" & RecordData.Length.ToString)
            DataInfo.Message = "�ڰ�ں��ޒ��s��v ں��ޒ�:" & RecordData.Length.ToString
            Return False
        End If

        Return True
    End Function

    '
    ' �@�\�@ �F �G���h���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' ���l�@ �F
    '
    Protected Function CheckEndRecord() As Boolean

        '2014/05/21 saitou �W���� DEL -------------------------------------------------->>>>
        '' 2008.04.23 ADD �R���t�����g�U�֓��}���`�Ή� >>
        'PreFURI_DATE = ""
        '' 2008.04.23 ADD �R���t�����g�U�֓��}���`�Ή� <<
        '2014/05/21 saitou �W���� DEL --------------------------------------------------<<<<

        If DataInfo.RecoedLen <> RecordData.Length Then
            WriteBLog("�G���h���R�[�h��", "�s��v", "���R�[�h���F" & RecordData.Length.ToString)
            DataInfo.Message = "����ں��ޒ��s��v ں��ޒ�:" & RecordData.Length.ToString
            Return False
        End If

        Return True
    End Function

    '
    ' �@�\�@ �F �g���[���[���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' ���l�@ �F
    '
    Protected Function CheckTrailerRecordFunou() As Boolean
        '�˗��������v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[���˗�����", "�ُ�", "�����F" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO)
            DataInfo.Message = "̧���ڰ׈˗������ُ� ����:" & InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO
            Return False
        End If

        '�˗����z���v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[���˗����z", "�ُ�", "���z�F" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO)
            DataInfo.Message = "̧���ڰ׈˗����z�ُ� ���z:" & InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO
            Return False
        End If

        '�U�֍ς݌������v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[���U�֍ς݌���", "�ُ�", "�����F" & InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO)
            DataInfo.Message = "̧���ڰאU�֍ό����ُ� ����:" & InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO
            Return False
        End If

        '�U�֍ς݋��z���v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[���U�֍ς݋��z", "�ُ�", "���z�F" & InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO)
            DataInfo.Message = "̧���ڰאU�֍ϋ��z�ُ� ���z:" & InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO
            Return False
        End If

        '�s�\�������v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[���s�\����", "�ُ�", "�����F" & InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO)
            DataInfo.Message = "̧���ڰוs�\�����ُ� ����:" & InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO
            Return False
        End If

        '�s�\���z���v�`�F�b�N
        If IsDecimal(InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO) = False Then
            WriteBLog("�t�@�C���g���[���s�\���z", "�ُ�", "���z�F" & InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO)
            DataInfo.Message = "̧���ڰוs�\���z�ُ� ���z:" & InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO
            Return False
        End If


        If InfoMeisaiMast.TOTAL_IRAI_KEN <> InfoMeisaiMast.TOTAL_KEN Then
            WriteBLog("�t�@�C���g���[���˗�����", "�s��v", "�����F" & InfoMeisaiMast.TOTAL_KEN.ToString)
            DataInfo.Message = "̧���ڰ׈˗������s��v ����:" & InfoMeisaiMast.TOTAL_KEN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_IRAI_KIN <> InfoMeisaiMast.TOTAL_KIN Then
            WriteBLog("�t�@�C���g���[���˗����z", "�s��v", "���z�F" & InfoMeisaiMast.TOTAL_KIN.ToString)
            DataInfo.Message = "̧���ڰ׈˗����z�s��v ���z:" & InfoMeisaiMast.TOTAL_KIN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_ZUMI_KEN <> InfoMeisaiMast.TOTAL_NORM_KEN Then
            WriteBLog("�t�@�C���g���[���U�֍ς݌���", "�s��v", "�����F" & InfoMeisaiMast.TOTAL_NORM_KEN.ToString)
            DataInfo.Message = "̧���ڰאU�֍ό����s��v ����:" & InfoMeisaiMast.TOTAL_NORM_KEN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_ZUMI_KIN <> InfoMeisaiMast.TOTAL_NORM_KIN Then
            WriteBLog("�t�@�C���g���[���U�֍ς݋��z", "�s��v", "���z�F" & InfoMeisaiMast.TOTAL_NORM_KIN.ToString)
            DataInfo.Message = "̧���ڰאU�֍ϋ��z�s��v ���z:" & InfoMeisaiMast.TOTAL_NORM_KIN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_FUNO_KEN <> InfoMeisaiMast.TOTAL_IJO_KEN Then
            WriteBLog("�t�@�C���g���[���s�\����", "�s��v", "�����F" & InfoMeisaiMast.TOTAL_IJO_KEN.ToString)
            DataInfo.Message = "̧���ڰוs�\�����s��v ���z:" & InfoMeisaiMast.TOTAL_IJO_KEN.ToString
            Return False
        End If

        If InfoMeisaiMast.TOTAL_FUNO_KIN <> InfoMeisaiMast.TOTAL_IJO_KIN Then
            WriteBLog("�t�@�C���g���[���s�\���z", "�s��v", "���z�F" & InfoMeisaiMast.TOTAL_IJO_KIN.ToString)
            DataInfo.Message = "̧���ڰוs�\���z�s��v ���z:" & InfoMeisaiMast.TOTAL_IJO_KIN.ToString
            Return False
        End If

        Return True
    End Function

    ' �@�\�@ �F �X�܃}�X�^�̑��݂��m�F����
    '
    ' ����   �F ARG1 - ���Z�@�փR�[�h
    '        �F ARG2 - �x�X�R�[�h
    '        �F ARG3 - �U�֓�
    '
    ' �߂�l �F 0 - ���Z�@�ւȂ��C1 - ���Z�@�ւ���C�x�X�Ȃ��C2 - ���Z�@�ցC�x�X����
    '
    ' ���l�@ �F
    '
    Public Function GetTENMASTExists(ByVal kinno As String, ByVal sitno As String, ByVal furiDate As String) As Integer

        ' �c�a�ڑ������݂��Ȃ��ꍇ�C����l��Ԃ�
        If OraDB Is Nothing Then
            Return 2
        End If


        Dim nRet As Integer = 0
        Try
            Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(kinno, sitno)
            If Not OraReader Is Nothing Then
                If OraReader.EOF = False Then

                    '2009/09/10.Sakon�@�폜���`�F�b�N�Ɏx�X�폜����ǉ� +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    '' 2008.04.22 ADD OraReader.GetItem("SIT_N") = "OK" �ǉ� 
                    'If OraReader.GetItem("DEL_KIN_DATE_N") <> "" AndAlso OraReader.GetItem("DEL_KIN_DATE_N") < furiDate AndAlso OraReader.GetItem("SIT_N") = "OK" Then
                    'If ((OraReader.GetItem("KIN_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("KIN_DEL_DATE_N") < furiDate) Or _
                    '    (OraReader.GetItem("SIT_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("SIT_DEL_DATE_N") < furiDate)) AndAlso OraReader.GetItem("SIT_N") = "OK" Then
                    If ((OraReader.GetItem("KIN_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("KIN_DEL_DATE_N") <> "00000000" AndAlso OraReader.GetItem("KIN_DEL_DATE_N") < furiDate) Or
                        (OraReader.GetItem("SIT_DEL_DATE_N") <> "" AndAlso OraReader.GetItem("SIT_DEL_DATE_N") <> "00000000" AndAlso OraReader.GetItem("SIT_DEL_DATE_N") < furiDate)) AndAlso OraReader.GetItem("SIT_N") = "OK" Then

                        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        ' �U�֓����폜����肠�ƂɂȂ�܂�
                        nRet = 3
                    Else
                        If OraReader.GetItem("SIT_N") = "OK" Then
                            ' ���Z�@�ցC�x�X����
                            nRet = 2
                        Else
                            ' ���Z�@�ցC�x�X�Ȃ�
                            nRet = 1
                        End If
                    End If
                End If
                OraReader.Close()
            End If
            OraReader = Nothing
        Catch ex As Exception

        End Try

        Return nRet
    End Function

    ' �@�\�@ �F �X�܃}�X�^���擾����
    '
    ' ����   �F ARG1 - ���Z�@�փR�[�h
    '           ARG2 - �x�X�R�[�h
    '
    ' ���l�@ �F 2017/01/16 saitou ���t�M��(RSV2�W��) update for �X���[�G�X�Ή�
    '           �ETEIKEI_KBN_N(��g�敪)�̕���
    '
    Public Function GetTENMAST(ByVal kinno As String, ByVal sitno As String) As CASTCommon.MyOracleReader
        Dim SQL As New StringBuilder(256)
        Dim OraReader As CASTCommon.MyOracleReader

        Try

            ' 2016/01/27 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
            ' �c�a�ڑ������݂��Ȃ��ꍇ�C�������s��Ȃ�
            If OraDB Is Nothing Then
                Return Nothing
            End If
            ' 2016/01/27 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

            SQL.Append("SELECT")
            SQL.Append(" KIN_NNAME_N")
            SQL.Append(",SIT_NNAME_N")
            SQL.Append(",KIN_KNAME_N")
            SQL.Append(",SIT_KNAME_N")
            SQL.Append(",KIN_DEL_DATE_N")
            SQL.Append(",SIT_DEL_DATE_N")
            SQL.Append(",TEIKEI_KBN_N")
            SQL.Append(",'OK' SIT_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(kinno))
            SQL.Append("   AND SIT_NO_N = " & SQ(sitno))

            OraReader = New CASTCommon.MyOracleReader(OraDB)

            If OraReader.DataReader(SQL) = True Then
                Return OraReader
            Else
                OraReader.Close()

                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append(" KIN_NNAME_N")
                SQL.Append(",SIT_NNAME_N")
                SQL.Append(",KIN_KNAME_N")
                SQL.Append(",SIT_KNAME_N")
                SQL.Append(",KIN_DEL_DATE_N")
                SQL.Append(",SIT_DEL_DATE_N")
                SQL.Append(",TEIKEI_KBN_N")
                SQL.Append(",'NG' SIT_N")
                SQL.Append(" FROM TENMAST")
                SQL.Append(" WHERE KIN_NO_N = " & SQ(kinno))

                OraReader = New CASTCommon.MyOracleReader(OraDB)
                If OraReader.DataReader(SQL) = True Then
                    Return OraReader
                End If
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        Finally
        End Try
    End Function

    ' �@�\�@ �F �X�P�W���[���}�X�^��o�^����
    '
    ' ���l�@ �F
    '
    Private Function InsertSCHMAST() As Boolean
        Dim SQL As New StringBuilder(1024)

        ' �c�a�ڑ������݂��Ȃ��ꍇ�C����l��Ԃ�
        If OraDB Is Nothing Then
            Return True
        End If

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If Not BLOG Is Nothing Then
            sw = BLOG.Write_Enter1("ClsFormat.InsertSCHMAST")
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Dim SCHData As CAstFormatMini.ClsSchduleMaintenanceClass.SCHMAST_Data

        ' 2016/04/25 �^�X�N�j���� DEL �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- START
        ''�R���t�����g�U�֓��}���`�Ή� >>
        'If Not InfoMeisaiMast.FURIKAE_DATE Is Nothing AndAlso InfoMeisaiMast.FURIKAE_DATE <> "" Then
        '    Dim IParam As CAstBatch.CommData.stPARAMETER
        '    IParam = mInfoComm.INFOParameter
        '    IParam.FURI_DATE = InfoMeisaiMast.FURIKAE_DATE
        '    mInfoComm.INFOParameter = IParam
        'End If
        ''�R���t�����g�U�֓��}���`�Ή� <<
        ' 2016/04/25 �^�X�N�j���� CHG �yOM�zUI_B-99-99(RSV2�Ή�(�W���o�O�C��)) -------------------- END

        ' �X�P�W���[�� �\������Z�o
        Dim CLS As New CAstFormatMini.ClsMain(mInfoComm.INFOToriMast.TORIS_CODE_T, mInfoComm.INFOToriMast.TORIF_CODE_T, mInfoComm.INFOParameter.FURI_DATE)

        SCHData = CLS.SCHData

        CLS = Nothing

        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        '����������ޔ�
        MOTIKOMI_DATE = SCHData.MOTIKOMI_DATE
        '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

        ' �U����
        Dim FurikaeDate As Date = ConvertDate(mInfoComm.INFOParameter.FURI_DATE)
        If FurikaeDate.ToString("yyyyMMdd") <> mInfoComm.INFOParameter.FURI_DATE Then
            WriteBLog("����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "�ُ�", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��"))
            DataInfo.Message = "����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�ُ�@" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��")
            Return False
        End If

        ' �j������
        Select Case FurikaeDate.DayOfWeek()
            Case DayOfWeek.Saturday
                WriteBLog("����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "�y�j��", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��"))
                DataInfo.Message = "����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�i�y�j���j�@" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��")
                Return False
            Case DayOfWeek.Sunday
                WriteBLog("����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "���j��", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��"))
                DataInfo.Message = "����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�i���j���j�@" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��")
                Return False
        End Select

        ' �x������
        If HolidayList.BinarySearch(FurikaeDate.ToString("yyyyMMdd")) >= 0 Then
            WriteBLog("����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN), "�x��", GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��"))
            DataInfo.Message = "����" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�i�x���j�@" & GetFuriDate(mInfoComm.INFOParameter.FSYORI_KBN) & "�F" & ConvertDate(mInfoComm.INFOParameter.FURI_DATE, "MM��dd��")
            Return False
        End If

        '------------------
        '�}�X�^�o�^���ڐݒ�
        '------------------
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = mInfoComm.INFOParameter
        Dim InfoTori As CAstBatch.CommData.stTORIMAST = mInfoComm.INFOToriMast

        SQL = New StringBuilder(512)
        SQL.AppendLine("INSERT INTO S_SCHMAST(")
        SQL.AppendLine(" FSYORI_KBN_S")                     ' FSYORI_KBN_S
        SQL.AppendLine(",TORIS_CODE_S")                     ' TORIS_CODE_S
        SQL.AppendLine(",TORIF_CODE_S")                     ' TORIF_CODE_S
        SQL.AppendLine(",FURI_DATE_S")                      ' FURI_DATE_S
        SQL.AppendLine(",KFURI_DATE_S")                     ' KFURI_DATE_S
        SQL.AppendLine(",SYUBETU_S")                        ' SYUBETU_S
        SQL.AppendLine(",FURI_CODE_S")                      ' FURI_CODE_S
        SQL.AppendLine(",KIGYO_CODE_S")                     ' KIGYO_CODE_S
        SQL.AppendLine(",ITAKU_CODE_S")                     ' ITAKU_CODE_S
        SQL.AppendLine(",TKIN_NO_S")                        ' TKIN_NO_S
        SQL.AppendLine(",TSIT_NO_S")                        ' TSIT_NO_S
        SQL.AppendLine(",SOUSIN_KBN_S")                     ' SOUSIN_KBN_S
        SQL.AppendLine(",BAITAI_CODE_S")                    ' BAITAI_CODE_S
        SQL.AppendLine(",MOTIKOMI_SEQ_S")                   ' MOTIKOMI_SEQ_S
        SQL.AppendLine(",FILE_SEQ_S")                       ' FILE_SEQ_S
        SQL.AppendLine(",TESUU_KBN_S")                      ' TESUU_KBN_S
        SQL.AppendLine(",IRAISYO_DATE_S")                   ' IRAISYO_DATE_S
        SQL.AppendLine(",IRAISYOK_YDATE_S")                 ' IRAISYOK_YDATE_S
        SQL.AppendLine(",MOTIKOMI_DATE_S")                  ' MOTIKOMI_DATE_S
        SQL.AppendLine(",UKETUKE_DATE_S")                   ' UKETUKE_DATE_S
        SQL.AppendLine(",TOUROKU_DATE_S")                   ' TOUROKU_DATE_S
        SQL.AppendLine(",KAKUHO_YDATE_S")                   ' KAKUHO_YDATE_S
        SQL.AppendLine(",KAKUHO_DATE_S")                    ' KAKUHO_DATE_S
        SQL.AppendLine(",HASSIN_YDATE_S")                   ' HASSIN_YDATE_S
        SQL.AppendLine(",HASSIN_DATE_S")                    ' HASSIN_DATE_S
        SQL.AppendLine(",SOUSIN_YDATE_S")                   ' SOUSIN_YDATE_S
        SQL.AppendLine(",SOUSIN_DATE_S")                    ' SOUSIN_DATE_S
        SQL.AppendLine(",KESSAI_YDATE_S")                   ' KESSAI_YDATE_S  
        SQL.AppendLine(",KESSAI_DATE_S")                    ' KESSAI_DATE_S    
        SQL.AppendLine(",TESUU_YDATE_S")                    ' TESUU_YDATE_S      
        SQL.AppendLine(",TESUU_DATE_S")                     ' TESUU_DATE_S    
        SQL.AppendLine(",UKETORI_DATE_S")                   ' UKETORI_DATE_S
        SQL.AppendLine(",UKETUKE_FLG_S")                    ' UKETUKE_FLG_S
        SQL.AppendLine(",TOUROKU_FLG_S")                    ' TOUROKU_FLG_S
        SQL.AppendLine(",KAKUHO_FLG_S")                     ' KAKUHO_FLG_S
        SQL.AppendLine(",HASSIN_FLG_S")                     ' HASSIN_FLG_S
        SQL.AppendLine(",SOUSIN_FLG_S")                     ' SOUSIN_FLG_S
        SQL.AppendLine(",TESUUKEI_FLG_S")                   ' TESUUKEI_FLG_S
        SQL.AppendLine(",TESUUTYO_FLG_S")                   ' TESUUTYO_FLG_S
        SQL.AppendLine(",KESSAI_FLG_S")                     ' KESSAI_FLG_S
        SQL.AppendLine(",TYUUDAN_FLG_S")                    ' TYUUDAN_FLG_S
        SQL.AppendLine(",ERROR_INF_S")                      ' ERROR_INF_S
        SQL.AppendLine(",SYORI_KEN_S")                      ' SYORI_KEN_S
        SQL.AppendLine(",SYORI_KIN_S")                      ' SYORI_KIN_S
        SQL.AppendLine(",ERR_KEN_S")                        ' ERR_KEN_S
        SQL.AppendLine(",ERR_KIN_S")                        ' ERR_KIN_S
        SQL.AppendLine(",TESUU_KIN_S")                      ' TESUU_KIN_S
        SQL.AppendLine(",TESUU_KIN1_S")                     ' TESUU_KIN1_S
        SQL.AppendLine(",TESUU_KIN2_S")                     ' TESUU_KIN2_S
        SQL.AppendLine(",TESUU_KIN3_S")                     ' TESUU_KIN3_S
        SQL.AppendLine(",FURI_KEN_S")                       ' FURI_KEN_S
        SQL.AppendLine(",FURI_KIN_S")                       ' FURI_KIN_S
        SQL.AppendLine(",FUNOU_KEN_S")                      ' FUNOU_KEN_S
        SQL.AppendLine(",FUNOU_KIN_S")                      ' FUNOU_KIN_S
        SQL.AppendLine(",UFILE_NAME_S")                     ' UFILE_NAME_S
        SQL.AppendLine(",SFILE_NAME_S")                     ' SFILE_NAME_S
        SQL.AppendLine(",SAKUSEI_DATE_S")                   ' SAKUSEI_DATE_S
        SQL.AppendLine(",KAKUHO_TIME_STAMP_S")              ' KAKUHO_TIME_STAMP_S
        SQL.AppendLine(",HASSIN_TIME_STAMP_S")              ' HASSIN_TIME_STAMP_S
        SQL.AppendLine(",KESSAI_TIME_STAMP_S")              ' KESSAI_TIME_STAMP_S
        SQL.AppendLine(",TESUU_TIME_STAMP_S")               ' TESUU_TIME_STAMP_S
        SQL.AppendLine(",YOBI1_S")                          ' YOBI1_S
        SQL.AppendLine(",YOBI2_S")                          ' YOBI2_S
        SQL.AppendLine(",YOBI3_S")                          ' YOBI3_S
        SQL.AppendLine(",YOBI4_S")                          ' YOBI4_S
        SQL.AppendLine(",YOBI5_S")                          ' YOBI5_S
        SQL.AppendLine(",YOBI6_S")                          ' YOBI6_S
        SQL.AppendLine(",YOBI7_S")                          ' YOBI7_S
        SQL.AppendLine(",YOBI8_S")                          ' YOBI8_S
        SQL.AppendLine(",YOBI9_S")                          ' YOBI9_S
        SQL.AppendLine(",YOBI10_S")                         ' YOBI10_S
        SQL.AppendLine(") VALUES(")
        SQL.AppendLine(" " & SQ(InfoTori.FSYORI_KBN_T))             ' FSYORI_KBN_S
        SQL.AppendLine("," & SQ(InfoTori.TORIS_CODE_T))             ' TORIS_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.TORIF_CODE_T))             ' TORIF_CODE_S
        SQL.AppendLine("," & SQ(InfoMeisaiMast.FURIKAE_DATE))       ' FURI_DATE_S
        SQL.AppendLine("," & SQ(InfoMeisaiMast.FURIKAE_DATE))       ' KFURI_DATE_S
        SQL.AppendLine("," & SQ(InfoTori.SYUBETU_T))                ' SYUBETU_S
        SQL.AppendLine("," & SQ(InfoTori.FURI_CODE_T))              ' FURI_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.KIGYO_CODE_T))             ' KIGYO_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.ITAKU_CODE_T))             ' ITAKU_CODE_S
        SQL.AppendLine("," & SQ(InfoTori.TKIN_NO_T))                ' TKIN_NO_S
        SQL.AppendLine("," & SQ(InfoTori.TSIT_NO_T))                ' TSIT_NO_S
        SQL.AppendLine("," & SQ(InfoTori.SOUSIN_KBN_T))             ' SOUSIN_KBN_S
        SQL.AppendLine("," & SQ(InfoTori.BAITAI_CODE_T))            ' BAITAI_CODE_S
        SQL.AppendLine(",{0}")                                      ' MOTIKOMI_SEQ_S
        SQL.AppendLine("," & (InfoMeisaiMast.FILE_SEQ + 1).ToString) ' FILE_SEQ_S

        '�萔���v�Z�敪�̎Z�o
        Select Case InfoTori.TESUUTYO_KBN_T
            Case "0"    ' �s�x����
                SQL.AppendLine(",1")                                ' TESUU_KBN_S
            Case "1"    ' �ꊇ����
                Select Case InfoTori.TUKI_T(FurikaeDate.Month - 1)
                    Case "1", "3"
                        SQL.AppendLine(",'2'")                      ' TESUU_KBN_S
                    Case Else
                        SQL.AppendLine(",'3'")                      ' TESUU_KBN_S
                End Select
            Case "2"    ' ���ʖƏ�
                SQL.AppendLine(",'0'")                              ' TESUU_KBN_S
            Case Else   ' �ʓr����
                SQL.AppendLine(",'0'")                              ' TESUU_KBN_S
        End Select

        SQL.AppendLine(",'00000000'")                               ' IRAISYO_DATE_S        
        SQL.AppendLine("," & SQ(SCHData.IRAISYOK_YDATE))            ' IRAISYOK_YDATE_S
        '2017/12/07 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
        If INI_MOTIKOMI_KIJITU_CHK = "1" Then
            SQL.AppendLine("," & SQ(SCHData.MOTIKOMI_DATE))             ' MOTIKOMI_DATE_S
        Else
            SQL.AppendLine(",'00000000'")                               ' MOTIKOMI_DATE_S
        End If
        'SQL.AppendLine(",'00000000'")                               ' MOTIKOMI_DATE_S
        '2017/12/07 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END
        SQL.AppendLine(",'00000000'")                               ' UKETUKE_DATE_S
        SQL.AppendLine(",'00000000'")                               ' TOUROKU_DATE_S
        SQL.AppendLine("," & SQ(SCHData.KAKUHO_YDATE))              ' KAKUHO_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' KAKUHO_DATE_S
        SQL.AppendLine("," & SQ(SCHData.HASSIN_YDATE))              ' HASSIN_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' HASSIN_DATE_S
        SQL.AppendLine("," & SQ(SCHData.HASSIN_YDATE))              ' SOUSIN_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' SOUSIN_DATE_S
        SQL.AppendLine("," & SQ(SCHData.KESSAI_YDATE))              ' KESSAI_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' KESSAI_DATE_S
        SQL.AppendLine("," & SQ(SCHData.TESUU_YDATE))               ' TESUU_YDATE_S
        SQL.AppendLine(",'00000000'")                               ' TESUU_DATE_S
        SQL.AppendLine(",'00000000'")                               ' UKETORI_DATE_S
        SQL.AppendLine(",'0'")                                      ' UKETUKE_FLG_S
        SQL.AppendLine(",'0'")                                      ' TOUROKU_FLG_S
        SQL.AppendLine(",'0'")                                      ' KAKUHO_FLG_S
        SQL.AppendLine(",'0'")                                      ' HASSIN_FLG_S
        SQL.AppendLine(",'0'")                                      ' SOUSIN_FLG_S
        SQL.AppendLine(",'0'")                                      ' TESUUKEI_FLG_S
        SQL.AppendLine(",'0'")                                      ' TESUUTYO_FLG_S
        SQL.AppendLine(",'0'")                                      ' KESSAI_FLG_S
        SQL.AppendLine(",'0'")                                      ' TYUUDAN_FLG_S
        SQL.AppendLine(",' '")                                      ' ERROR_INF_S
        SQL.AppendLine(",0")                                        ' SYORI_KEN_S
        SQL.AppendLine(",0")                                        ' SYORI_KIN_S
        SQL.AppendLine(",0")                                        ' ERR_KEN_S
        SQL.AppendLine(",0")                                        ' ERR_KIN_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN1_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN2_S
        SQL.AppendLine(",0")                                        ' TESUU_KIN3_S
        SQL.AppendLine(",0")                                        ' FURI_KEN_S
        SQL.AppendLine(",0")                                        ' FURI_KIN_S
        SQL.AppendLine(",0")                                        ' FUNOU_KEN_S
        SQL.AppendLine(",0")                                        ' FUNOU_KIN_S
        SQL.AppendLine(",' '")                                      ' UFILE_NAME_S
        SQL.AppendLine(",' '")                                      ' SFILE_NAME_S
        SQL.AppendLine("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' SAKUSEI_DATE_S
        SQL.AppendLine(",'00000000000000'")                         ' KAKUHO_TIME_STAMP_S
        SQL.AppendLine(",'00000000000000'")                         ' HASSIN_TIME_STAMP_S
        SQL.AppendLine(",'00000000000000'")                         ' KESSAI_TIME_STAMP_S
        SQL.AppendLine(",'00000000000000'")                         ' TESUU_TIME_STAMP_S
        SQL.AppendLine(",' '")                                      ' YOBI1_S
        SQL.AppendLine(",' '")                                      ' YOBI2_S
        SQL.AppendLine(",' '")                                      ' YOBI3_S
        SQL.AppendLine(",' '")                                      ' YOBI4_S
        SQL.AppendLine(",' '")                                      ' YOBI5_S
        SQL.AppendLine(",' '")                                      ' YOBI6_S
        SQL.AppendLine(",' '")                                      ' YOBI7_S
        SQL.AppendLine(",' '")                                      ' YOBI8_S
        SQL.AppendLine(",' '")                                      ' YOBI9_S
        SQL.AppendLine(",' '")                                      ' YOBI10_S
        SQL.AppendLine(")")

        ' ����SEQ�̐ݒ�
        Dim SEQReader As New CASTCommon.MyOracleReader(OraDB)
        Dim nGetSEQ As Integer
        ' ����SEQ���J�E���g�A�b�v����
        Dim SQL_SEQ As New StringBuilder
        ' 2017/08/23 �^�X�N�j���� CHG�yME�z(�W���@�\���P(���U�}���`�̎����r�d�p�擾���P)) -------------------- START
        'SQL_SEQ.AppendLine("SELECT")
        'SQL_SEQ.AppendLine(" NVL(MAX(MOTIKOMI_SEQ_S), 0) + 1 SEQ")
        'SQL_SEQ.AppendLine(" FROM S_SCHMAST")
        'SQL_SEQ.AppendLine(" WHERE TORIS_CODE_S  =" & SQ(InfoTori.TORIS_CODE_T))
        'SQL_SEQ.AppendLine(" AND TORIF_CODE_S  =" & SQ(InfoTori.TORIF_CODE_T))
        'SQL_SEQ.AppendLine(" AND FURI_DATE_S  =" & SQ(InfoMeisaiMast.FURIKAE_DATE))
        Select Case InfoTori.MULTI_KBN_T
            Case "1"
                '----------------------------------------
                ' �}���`(����ͯ��/1̧��) �̎擾SQL
                '----------------------------------------
                SQL_SEQ.Append("SELECT")
                SQL_SEQ.Append("     NVL(MAX(MOTIKOMI_SEQ_S), 0) + 1 SEQ")
                SQL_SEQ.Append(" FROM")
                SQL_SEQ.Append("     S_SCHMAST")
                SQL_SEQ.Append("   , S_TORIMAST")
                SQL_SEQ.Append(" WHERE ")
                SQL_SEQ.Append("     FURI_DATE_S        = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
                SQL_SEQ.Append(" AND TORIS_CODE_S       = TORIS_CODE_T")
                SQL_SEQ.Append(" AND TORIF_CODE_S       = TORIF_CODE_T")
                SQL_SEQ.Append(" AND MULTI_KBN_T        = '1'")
                SQL_SEQ.Append(" AND ITAKU_KANRI_CODE_T = ( SELECT ITAKU_KANRI_CODE_T")
                SQL_SEQ.Append("                             FROM  S_TORIMAST")
                SQL_SEQ.Append("                             WHERE")
                SQL_SEQ.Append("                                   TORIS_CODE_T = " & SQ(InfoTori.TORIS_CODE_T))
                SQL_SEQ.Append("                               AND TORIF_CODE_T = " & SQ(InfoTori.TORIF_CODE_T))
                SQL_SEQ.Append("                          )")
            Case Else
                '----------------------------------------
                ' �V���O��(1ͯ��/1̧��) �̎擾SQL
                '----------------------------------------
                SQL_SEQ.Append("SELECT")
                SQL_SEQ.Append("     NVL(MAX(MOTIKOMI_SEQ_S), 0) + 1 SEQ")
                SQL_SEQ.Append(" FROM")
                SQL_SEQ.Append("     S_SCHMAST")
                SQL_SEQ.Append(" WHERE ")
                SQL_SEQ.Append("     TORIS_CODE_S =" & SQ(InfoTori.TORIS_CODE_T))
                SQL_SEQ.Append(" AND TORIF_CODE_S =" & SQ(InfoTori.TORIF_CODE_T))
                SQL_SEQ.Append(" AND FURI_DATE_S  =" & SQ(InfoMeisaiMast.FURIKAE_DATE))
        End Select
        ' 2017/08/23 �^�X�N�j���� CHG�yME�z(�W���@�\���P(���U�}���`�̎����r�d�p�擾���P)) -------------------- END

        Dim nRet As Long = 0
        Do Until nRet = 1
            If InfoMeisaiMast.FILE_SEQ = 0 AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
                ' ����SEQ�J�E���g�A�b�vSQL���s
                If SEQReader.DataReader(SQL_SEQ) = True Then
                    nGetSEQ = SEQReader.GetValueInt(0)
                Else
                    WriteBLog("�X�P�W���[���쐬", "���s", OraDB.Message & ":" & SQL_SEQ.ToString)
                    Return False
                End If
                SEQReader.Close()
            Else
                nGetSEQ = InfoMeisaiMast.MOTIKOMI_SEQ
            End If

            ' INSERT
            nRet = OraDB.ExecuteNonQuery(String.Format(SQL.ToString, nGetSEQ), True)

            If nRet <= 0 Then
                If OraDB.Code <> 1 OrElse InfoMeisaiMast.MOTIKOMI_SEQ <> 0 Then
                    WriteBLog("�X�P�W���[���쐬", "���s", OraDB.Message & ":" & nRet.ToString)
                    Return False
                End If

                ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            Else
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Dim ReturnMessage As String = String.Empty
                    Dim SubMastInsert_Ret As Integer = 0
                    Call CAstExternal.ModExternal.Ex_InsertSchmastSub(InfoTori.FSYORI_KBN_T,
                                                                      InfoTori.TORIS_CODE_T,
                                                                      InfoTori.TORIF_CODE_T,
                                                                      InfoMeisaiMast.FURIKAE_DATE,
                                                                      nGetSEQ,
                                                                      ReturnMessage,
                                                                      OraDB)
                End If
                ' 2016/10/14 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

            End If
        Loop
        InfoMeisaiMast.MOTIKOMI_SEQ = nGetSEQ

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        If Not BLOG Is Nothing Then
            BLOG.Write_Exit1(sw, "ClsFormat.InsertSCHMAST")
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Return True

    End Function

    ' �@�\�@ �F �X�P�W���[���}�X�^ UPDATE
    '
    ' �߂�l �F 0 - ���� �C 0 �ȊO - �ُ�
    '
    ' ���l�@ �F 
    '
    Public Overridable Function UpdateSCHMAST() As Boolean
        Dim SQL As New StringBuilder(128)                        ' �r�p�k

        '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        If Not BLOG Is Nothing Then
            sw = BLOG.Write_Enter1("ClsFormat.UpdateSCHMAST")
        End If
        '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        Dim InfoTori As CAstBatch.CommData.stTORIMAST   ' �������
        InfoTori = mInfoComm.INFOToriMast
        Dim InfoPara As CAstBatch.CommData.stPARAMETER = mInfoComm.INFOParameter

        '2010/10/04.Sakon�@�e�X�g���[�h����ǉ�
        Dim TestMode As String

        ' �X�V�����̎������݂r�d�p
        Dim WhereMotikomiSEQ As Integer = -1
        If InfoTori.CYCLE_T = "1" Then
            ' �����񎝍�����̏ꍇ
            WhereMotikomiSEQ = InfoMeisaiMast.MOTIKOMI_SEQ
        End If

        ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
        Dim Schmast_Updateflg As String = ""
        Dim UkeTimeStamp_UpdateFlg As String = ""
        Dim Update_MotikomiSeq As Integer = 0
        ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

        '2010/10/04.Sakon�@�e�X�g���[�h����ǉ� +++++++++++++++++++++++++
        Try
            'INI�t�@�C�����e�X�g���[�h������s��
            TestMode = CASTCommon.GetFSKJIni("COMMON", "TESTMODE")
            If TestMode = "err" Then
                TestMode = "0"
            End If

            '����敛�R�[�h��99�̏ꍇ�̂݃e�X�g���[�h�Ƃ���
            If TestMode = "1" And InfoTori.TORIF_CODE_T <> "99" Then
                TestMode = "0"
            End If
        Catch ex As Exception
            TestMode = "0"
        End Try
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        Try
            InfoMeisaiMast.SCH_UPDATE_FLG = False
            ' �����Ǘ�����̏ꍇ

            '2017/04/27 saitou RSV2 UPD �����r�d�p�s��Ή� ---------------------------------------- START
            If mInfoComm.INFOParameter.FMT_KBN <> "TO" Then
                '�Z���^�[���ڎ����ȊO�͊����̏���
                If InfoPara.RENKEI_KBN <> "88" AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
                    ' �����ȊO�C �����񎝂�����
                    If InfoMeisaiMast.FILE_SEQ = 1 Then
                        '���߂�1�񂾂����ׂ�΂悢�i�}���`�̂Ƃ��j
                        '�����r�d�p�̎擾
                        SQL = New StringBuilder(1024)
                        SQL.Append("SELECT")
                        SQL.Append(" NVL(MAX(MOTIKOMI_SEQ_S),0)+1 MOTIKOMI_MAX")
                        If InfoTori.FSYORI_KBN_T = "1" Then
                            SQL.Append(" FROM SCHMAST")
                        Else
                            SQL.Append(" FROM S_SCHMAST")
                        End If
                        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                        ' ���ς̂��߃R�����g��
                        'SQL.Append("   AND BAITAI_CODE_S= " & SQ(InfoTori.BAITAI_CODE_T))

                        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
                        If OraReader.DataReader(SQL) = False Then
                            WriteBLog("�������݂r�d�p�擾", "���s")
                            Return False
                        End If

                        ' ���׃}�X�^���ڐݒ� �����r�d�p
                        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetValueInt(0)
                        InfoMeisaiMast.SCH_UPDATE_FLG = True
                        WriteBLog("�������݂r�d�p�擾", "����", "�������݂r�d�p�F" & OraReader.GetValueInt(0).ToString)

                        OraReader.Close()
                        OraReader = Nothing
                    End If
                End If
            Else
                '�Z���^�[���ڎ����͖��񎝍��r�d�p�̎擾���s��
                If InfoPara.RENKEI_KBN <> "88" Then
                    ' �����ȊO�C �����񎝂�����
                    '�����r�d�p�̎擾
                    SQL = New StringBuilder(1024)
                    SQL.Append("SELECT")
                    SQL.Append(" NVL(MAX(MOTIKOMI_SEQ_S),0)+1 MOTIKOMI_MAX")
                    If InfoTori.FSYORI_KBN_T = "1" Then
                        SQL.Append(" FROM SCHMAST")
                    Else
                        SQL.Append(" FROM S_SCHMAST")
                    End If
                    SQL.Append(" WHERE FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
                    ' ���ς̂��߃R�����g��
                    'SQL.Append("   AND BAITAI_CODE_S= " & SQ(InfoTori.BAITAI_CODE_T))

                    Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
                    If OraReader.DataReader(SQL) = False Then
                        WriteBLog("�������݂r�d�p�擾", "���s")
                        Return False
                    End If

                    ' ���׃}�X�^���ڐݒ� �����r�d�p
                    InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetValueInt(0)
                    InfoMeisaiMast.SCH_UPDATE_FLG = True
                    WriteBLog("�������݂r�d�p�擾", "����", "�������݂r�d�p�F" & OraReader.GetValueInt(0).ToString)

                    OraReader.Close()
                    OraReader = Nothing
                End If
            End If

            'If InfoPara.RENKEI_KBN <> "88" AndAlso InfoMeisaiMast.MOTIKOMI_SEQ = 0 Then
            '    ' �����ȊO�C �����񎝂�����
            '    If InfoMeisaiMast.FILE_SEQ = 1 Then
            '        '���߂�1�񂾂����ׂ�΂悢�i�}���`�̂Ƃ��j
            '        '�����r�d�p�̎擾
            '        SQL = New StringBuilder(1024)
            '        SQL.Append("SELECT")
            '        SQL.Append(" NVL(MAX(MOTIKOMI_SEQ_S),0)+1 MOTIKOMI_MAX")
            '        If InfoTori.FSYORI_KBN_T = "1" Then
            '            SQL.Append(" FROM SCHMAST")
            '        Else
            '            SQL.Append(" FROM S_SCHMAST")
            '        End If
            '        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            '        ' ���ς̂��߃R�����g��
            '        'SQL.Append("   AND BAITAI_CODE_S= " & SQ(InfoTori.BAITAI_CODE_T))

            '        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
            '        If OraReader.DataReader(SQL) = False Then
            '            WriteBLog("�������݂r�d�p�擾", "���s")
            '            Return False
            '        End If

            '        ' ���׃}�X�^���ڐݒ� �����r�d�p
            '        InfoMeisaiMast.MOTIKOMI_SEQ = OraReader.GetValueInt(0)
            '        InfoMeisaiMast.SCH_UPDATE_FLG = True
            '        WriteBLog("�������݂r�d�p�擾", "����", "�������݂r�d�p�F" & OraReader.GetValueInt(0).ToString)

            '        OraReader.Close()
            '        OraReader = Nothing
            '    End If
            'End If
            '2017/04/27 saitou RSV2 UPD ------------------------------------------------------------- END

            SQL = New StringBuilder(1024)
            SQL.Append("UPDATE")

            If InfoTori.FSYORI_KBN_T = "1" Then
                SQL.Append(" SCHMAST")
            Else
                SQL.Append(" S_SCHMAST")
            End If

            SQL.Append(" SET ")
            SQL.Append(" SYORI_KEN_S = " & InfoMeisaiMast.TOTAL_IRAI_KEN.ToString)
            SQL.Append(",SYORI_KIN_S = " & InfoMeisaiMast.TOTAL_IRAI_KIN.ToString)
            SQL.Append(",ERR_KEN_S = " & InfoMeisaiMast.TOTAL_IJO_KEN.ToString)
            SQL.Append(",ERR_KIN_S = " & InfoMeisaiMast.TOTAL_IJO_KIN.ToString)

            If InfoMeisaiMast.DUPLICATE_KBN = "020" Then
                ' �Q�d�̏ꍇ�́C��t�ςɂ��Ȃ�
                SQL.Append(",UKETUKE_FLG_S ='0'")       ' ��t��
                SQL.Append(",ERROR_INF_S = '020'")      ' �Q�d�o�^
                '2018/10/04 saitou �L���M��(RSV2�W��) ADD ---------------------------------------- START
                '��d�����̏ꍇ����t�����͍X�V����B
                UkeTimeStamp_UpdateFlg = "1"
                '2018/10/04 saitou �L���M��(RSV2�W��) ADD ---------------------------------------- END
            Else
                SQL.Append(",ERROR_INF_S = ' '")        ' �G���[�N���A
                SQL.Append(",UKETUKE_FLG_S ='1'")       ' ��t��
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
                UkeTimeStamp_UpdateFlg = "1"
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END
                '2018/03/16 saitou �L���M��(RSV2�W��) ADD �ƍ��@�\�ǉ� ---------------------------------------- START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    '�˗����A�`�[�ȊO�͎�t�����X�V
                    If InfoTori.BAITAI_CODE_T <> "04" AndAlso InfoTori.BAITAI_CODE_T <> "09" Then
                        SQL.Append(",UKETUKE_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    End If
                End If
                '2018/03/16 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------- END
            End If

            If InfoMeisaiMast.TOTAL_IJO_KEN > 0 Then
                '2009/09/29.Sakon�@ERROR_INF2_S����菜�� ++++++++++++++++++++++++
                '                  (���U�̏ꍇ�̍l���K�v�����H)
                'SQL.Append(",ERROR_INF2_S = 'ERR'")     ' �C���v�b�g�G���[����
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            End If

            '2017/12/04 �^�X�N�j���� CHG �W���ŏC���i�ƍ��@�\�ǉ��A���Z�@�֖�����Ή��j------------------------------------ START
            If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" AndAlso
                CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYOUGOU") = "1" Then
                '�ƍ��@�\�L��̏ꍇ
                If InfoTori.SYOUGOU_KBN_T = "1" OrElse InfoMeisaiMast.DUPLICATE_KBN = "020" OrElse
                   (INI_KINKO_SOUI_CHK = "1" AndAlso InfoMeisaiMast.KinTenSoui = True) OrElse
                   (InfoMeisaiMast.TOTAL_IJO_KEN > 0 AndAlso InfoMeisaiMast.DUPLICATE_KBN <> "020") Then
                    '�ƍ��v�̏ꍇ
                    '�Q�d�������݂̏ꍇ�A�Q�d�������݈ȊO�̃C���v�b�g�G���[������ꍇ�܂���
                    '���Z�@�֖������Ⴕ�Ă���ꍇ(���U�̏ꍇ�̂ݔ��肵�Ă���)
                    SQL.Append(",TOUROKU_FLG_S ='0'")       ' ���o�^
                Else
                    SQL.Append(",TOUROKU_FLG_S ='1'")       ' �o�^��
                    '2018/03/16 saitou �L���M��(RSV2�W��) ADD �ƍ��@�\�ǉ� ---------------------------------------- START
                    '�o�^���̍X�V
                    SQL.Append(",TOUROKU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    '2018/03/16 saitou �L���M��(RSV2�W��) ADD ----------------------------------------------------- END
                End If
            Else
                SQL.Append(",TOUROKU_FLG_S ='1'")       ' �o�^��
                SQL.Append(",TOUROKU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
            End If
            'SQL.Append(",TOUROKU_FLG_S ='1'")       ' �o�^��
            'SQL.Append(",TOUROKU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
            '2017/12/04 �^�X�N�j���� CHG �W���ŏC���i�ƍ��@�\�ǉ��A���Z�@�֖�����Ή��j------------------------------------ END

            '2010/10/04.Sakon�@�e�X�g���[�h�ǉ��i�z�M���t���O�𗧂Ă�j +++++++++++++++
            If TestMode = "1" Then
                Select Case InfoTori.FSYORI_KBN_T
                    Case "1"
                        SQL.Append(",HAISIN_FLG_S ='1'")
                        SQL.Append(",HAISIN_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    Case "3"
                        SQL.Append(",HASSIN_FLG_S ='1'")
                        SQL.Append(",HASSIN_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                End Select
            End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            If InfoPara.RENKEI_KBN <> "88" AndAlso (WhereMotikomiSEQ = -1) Then
                ' �����񎝂����݂���̏ꍇ
                SQL.Append(",MOTIKOMI_SEQ_S = " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
                Update_MotikomiSeq = InfoMeisaiMast.MOTIKOMI_SEQ
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END
            End If

            SQL.Append(",FILE_SEQ_S = " & InfoMeisaiMast.FILE_SEQ.ToString)

            ' �Z���^�[���ڎ������݂́C�z�M�t���O�ɂP�����Ă�
            If InfoTori.MOTIKOMI_KBN_T = "1" Then
                '�Z���^�[���ڎ����ݒ���Q�Ƃ���
                If CASTCommon.GetFSKJIni("JIFURI", "CENTER_MOTIKOMI") = "1" Then
                    SQL.Append(", HAISIN_FLG_S = '1'")
                End If
            End If

            '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
            If INI_MOTIKOMI_KIJITU_CHK = "1" OrElse INI_UKETUKE_JIKAN_CHK = "1" Then
                '���U�����ԊO�̏ꍇ�A���f�t���O�𗧂Ă�
                If InfoTori.FSYORI_KBN_T = "3" AndAlso InfoMeisaiMast.TimeOver = False Then
                    SQL.Append(", TYUUDAN_FLG_S = '1'")
                End If
            End If
            '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

            ' SQL �X�V����
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(InfoTori.TORIS_CODE_T))
            SQL.Append("   AND TORIF_CODE_S = " & SQ(InfoTori.TORIF_CODE_T))
            '�Z���^�[���ڎ����t�H�[�}�b�g�����U�֓��Ή�
            If mInfoComm.INFOParameter.FMT_KBN = "TO" Then
                SQL.Append("   AND FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
            Else
                SQL.Append("   AND FURI_DATE_S  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            End If

            SQL.Append("   AND TOUROKU_FLG_S = '0'")

            ' �����Ǘ��Ȃ��̏ꍇ
            If WhereMotikomiSEQ <> -1 Then
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & WhereMotikomiSEQ)
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
                Update_MotikomiSeq = WhereMotikomiSEQ
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END
            End If

            SQL.Append(" AND ROWNUM <= 1")

            ' UPDATE ���s
            If OraDB.ExecuteNonQuery(SQL) <= 0 Then
                WriteBLog("UpdateSCHMAST", "���s", OraDB.Message)
                Return False
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            Else
                Schmast_Updateflg = "1"
                ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END
            End If

            If WhereMotikomiSEQ <> -1 Then
                '+++++++++++++++++++++++++++++++++++++++++++
                ' �����Ǘ��Ȃ��̏ꍇ
                ' �Q�d�������݃G���[���́C�����������ݒP�ʂ̃t�@�C����S�ĂQ�d�G���[�ɂ���
                SQL = New StringBuilder("UPDATE S_SCHMAST SET ", 128)
                SQL.Append(" ERROR_INF_S = ")
                SQL.Append(" (SELECT MAX(ERROR_INF_S) FROM S_SCHMAST")
                SQL.Append("   WHERE TORIS_CODE_S  = " & SQ(InfoTori.TORIS_CODE_T))
                SQL.Append("     AND FURI_DATE_S   = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQL.Append("     AND MOTIKOMI_SEQ_S= " & WhereMotikomiSEQ)
                SQL.Append(" )")
                SQL.Append(",UKETUKE_FLG_S = ")
                SQL.Append(" (SELECT MIN(UKETUKE_FLG_S) FROM S_SCHMAST")
                SQL.Append("   WHERE TORIS_CODE_S  = " & SQ(InfoTori.TORIS_CODE_T))
                SQL.Append("     AND FURI_DATE_S   = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQL.Append("     AND MOTIKOMI_SEQ_S= " & WhereMotikomiSEQ)
                SQL.Append(" )")
                SQL.Append(" WHERE TORIS_CODE_S   = " & SQ(InfoTori.TORIS_CODE_T))
                SQL.Append("   AND FURI_DATE_S    = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQL.Append("   AND MOTIKOMI_SEQ_S = " & WhereMotikomiSEQ)
                ' UPDATE ���s
                Dim nRet As Integer = OraDB.ExecuteNonQuery(SQL)
                WriteBLog("UpdateSCHMAST", "����", "�d���o�^�����F" & nRet.ToString)
                If nRet < 0 Then
                    WriteBLog("UpdateSCHMAST", "���s", OraDB.Message)
                End If
            End If

            ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- START
            If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                If UkeTimeStamp_UpdateFlg = "1" And
                   Schmast_Updateflg = "1" Then

                    SQL.Length = 0
                    Select Case InfoTori.FSYORI_KBN_T
                        Case "1"
                            SQL.Append("UPDATE   SCHMAST_SUB SET ")
                        Case Else
                            SQL.Append("UPDATE S_SCHMAST_SUB SET ")
                    End Select

                    SQL.Append("      UKETUKE_TIME_STAMP_S  = " & SQ(strUketukeDate & strUketukeTime))

                    SQL.Append(" WHERE ")
                    SQL.Append("      TORIS_CODE_SSUB       = " & SQ(InfoTori.TORIS_CODE_T))
                    SQL.Append("  AND TORIF_CODE_SSUB       = " & SQ(InfoTori.TORIF_CODE_T))

                    If mInfoComm.INFOParameter.FMT_KBN = "TO" Then
                        SQL.Append("  AND FURI_DATE_SSUB    = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
                    Else
                        SQL.Append("  AND FURI_DATE_SSUB    = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                    End If

                    Select Case InfoTori.FSYORI_KBN_T
                        Case "3"
                            SQL.Append("  AND MOTIKOMI_SEQ_SSUB = " & Update_MotikomiSeq)
                    End Select
                    SQL.Append("  AND ROWNUM               <= 1")

                    ' UPDATE ���s
                    Dim nRet As Integer = OraDB.ExecuteNonQuery(SQL)
                    If nRet < 0 Then
                        WriteBLog("UpdateSCHMAST(��t���ԍX�V)", "���s", OraDB.Message)
                    Else
                        WriteBLog("UpdateSCHMAST(��t���ԍX�V)", "����", "�X�V����:" & strUketukeDate & strUketukeTime & "/����:" & nRet.ToString)
                    End If
                End If
            End If
            ' 2016/10/17 �^�X�N�j���� ADD �yME�zUI_B-99-99(RSV2�Ή�) -------------------- END

        Catch ex As Exception
            WriteBLog("UpdateSCHMAST", "���s", ex.Message)
            Return False

            '*** Str Add 2015/12/01 SO)�r�� for ���O���� ***
        Finally
            If Not BLOG Is Nothing Then
                BLOG.Write_Exit1(sw, "ClsFormat.UpdateSCHMAST")
            End If
            '*** End Add 2015/12/01 SO)�r�� for ���O���� ***

        End Try

        Return True
    End Function

    ' �@�\�@ �F ��d�������݃`�F�b�N
    '
    ' ����   �F ARG1 - ���׃f�[�^�i�w�b�_�{���ׁi�l�`�w�T���j�{�g���[���j
    '
    ' ���l�@ �F
    '
    Public Function CheckDuplicate(ByVal Arr As ArrayList) As Boolean
        Dim Ora1StMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora2ndMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora3rdMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQLBase As StringBuilder
        Dim SQL2Jyu As StringBuilder

        If Arr.Count = 0 Then
            Return False
        End If

        Try
            SQLBase = New StringBuilder("SELECT MOTIKOMI_SEQ_K , RECORD_NO_K,  FURI_DATA_K", 128)
            SQLBase.Append(" FROM S_MEIMAST")
            SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
            SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))

            SQL2Jyu = New StringBuilder(SQLBase.ToString, 128)
            SQL2Jyu.Append("   AND RECORD_NO_K  = 1")
            SQL2Jyu.Append("   AND MOTIKOMI_SEQ_K <> " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
            If Ora1StMeiReader.DataReader(SQL2Jyu) = True Then
                '�K��O������S�Ēu�����Ĕ�r����
                If ReplaceString(Ora1StMeiReader.GetItem("FURI_DATA_K"), -1).Equals(ReplaceString(CType(Arr.Item(0), String), -1)) = False Then
                    ' ��v���郌�R�[�h�������ꍇ�CFALSE��Ԃ�
                    Return False
                End If

                ' �ŏ��̂P������v�����̂ŁC�����T�[�`����  ' �T�C�N���ԍ� �ł̃��[�v
                Dim nDupli As Integer = 0
                Do Until Ora1StMeiReader.EOF = True
                    SQL2Jyu = New StringBuilder(SQLBase.ToString, 128)
                    SQL2Jyu.Append(" AND MOTIKOMI_SEQ_K = " & Ora1StMeiReader.GetItem("MOTIKOMI_SEQ_K"))
                    SQL2Jyu.Append(" ORDER BY MOTIKOMI_SEQ_K , RECORD_NO_K")
                    If Ora2ndMeiReader.DataReader(SQL2Jyu) = True Then
                        Do Until Ora2ndMeiReader.EOF = True
                            '�K��O������S�Ēu�����Ĕ�r����
                            Dim MotoData As String = ReplaceString(Ora2ndMeiReader.GetItem("FURI_DATA_K"), -1)
                            If MotoData.Equals(ReplaceString(CType(Arr.Item(nDupli), String), -1)) = False Then
                                ' ��v���Ȃ��ꍇ�́C����
                                Exit Do
                            End If

                            nDupli += 1
                            If (nDupli) = (Arr.Count - 1) Then
                                ' �g���[�����R�[�h�`�F�b�N
                                SQL2Jyu = New StringBuilder(SQLBase.ToString, 128)
                                SQL2Jyu.Append(" AND MOTIKOMI_SEQ_K = " & Ora1StMeiReader.GetItem("MOTIKOMI_SEQ_K"))
                                SQL2Jyu.Append(" AND DATA_KBN_K = " & SQ(TrailerKubun(TrailerKubun.Length - 1)))

                                If Ora3rdMeiReader.DataReader(SQL2Jyu) = True Then
                                    '�K��O������S�Ēu�����Ĕ�r����
                                    If ReplaceString(Ora3rdMeiReader.GetItem("FURI_DATA_K"), -1).Equals(ReplaceString(CType(Arr.Item(nDupli), String), -1)) = True Then
                                        ' �Q�d�ǂݍ��݌���
                                        InfoMeisaiMast.DUPLICATE_KBN = "020"
                                        Return True
                                    End If
                                End If
                            End If

                            ' ���̖��ׂ�����
                            Ora2ndMeiReader.NextRead()
                        Loop
                    End If

                    ' ���̃T�C�N���ԍ�������
                    Ora1StMeiReader.NextRead()
                    '*** �C�� mitsu 2009/07/09 ���[�v���ɃJ�E���^�̃��Z�b�g���s�� ***
                    nDupli = 0
                    '****************************************************************
                Loop
            End If
        Catch ex As Exception
            BLOG.Write("�Q�d�`�F�b�N����", "���s", ex.Message & ":" & ex.StackTrace)
        Finally
            Ora1StMeiReader.Close()
            Ora2ndMeiReader.Close()
            Ora3rdMeiReader.Close()
        End Try

        Return False
    End Function

    ''' <summary>
    ''' ���U�����˗����݃`�F�b�N
    ''' </summary>
    ''' <param name="Arr">���׃f�[�^</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/12/06 �^�X�N�j����@�L���M��(RSV2�W����) added for ��K�͍\�z�Ή��i�d�����R�[�h�`�F�b�N�j</remarks>
    Public Function fn_Meisai_FukusuuIrai_Check(ByVal Arr As ArrayList) As Boolean

        Dim Ora1stMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora2ndMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim Ora3rdMeiReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQLBase As StringBuilder

        If Arr.Count = 0 Then
            Return False
        End If

        Try

            SQLBase = New StringBuilder("SELECT * ", 128)
            SQLBase.Append(" FROM S_MEIMAST")
            SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
            SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
            SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
            SQLBase.Append("   AND MOTIKOMI_SEQ_K <> " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)

            '��������E����U���w������Ȃ���Ώ������Ȃ��B
            If Ora1stMeiReader.DataReader(SQLBase) = True Then

                '���񗎂����݂̑S���׍s��ǂݎ��B
                SQLBase = New StringBuilder("SELECT * ", 128)
                SQLBase.Append(" FROM S_MEIMAST")
                SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                SQLBase.Append("   AND MOTIKOMI_SEQ_K = " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
                SQLBase.Append("   AND DATA_KBN_K  = '2'")

                If Ora2ndMeiReader.DataReader(SQLBase) = True Then
                    BLOG.Write("�f�[�^�Ǎ�", "����", "�U���˗��f�[�^��d�`�F�b�N�J�n")
                    Do Until Ora2ndMeiReader.EOF = True
                        '��������E����U���w����E����˗��l�������i���E�X�E�ȖځE���ԁj�E������l�������i���E�X�E�ȖځE���ԁE���v�Ɣԍ��j�����ɂ���΁A
                        '���̈˗��S�̂��Q�d�����̋��ꂠ��Ɣ��f����B
                        SQLBase = New StringBuilder("SELECT * ", 128)
                        SQLBase.Append(" FROM S_MEIMAST")
                        SQLBase.Append(" WHERE TORIS_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
                        SQLBase.Append("   AND TORIF_CODE_K = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
                        SQLBase.Append("   AND FURI_DATE_K  = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
                        SQLBase.Append("   AND MOTIKOMI_SEQ_K <> " & InfoMeisaiMast.MOTIKOMI_SEQ.ToString)
                        SQLBase.Append("   AND ITAKU_KIN_K = " & SQ(Ora2ndMeiReader.GetString("ITAKU_KIN_K")))
                        SQLBase.Append("   AND ITAKU_SIT_K= " & SQ(Ora2ndMeiReader.GetString("ITAKU_SIT_K")))
                        SQLBase.Append("   AND ITAKU_KAMOKU_K= " & SQ(Ora2ndMeiReader.GetString("ITAKU_KAMOKU_K")))
                        SQLBase.Append("   AND ITAKU_KOUZA_K= " & SQ(Ora2ndMeiReader.GetString("ITAKU_KOUZA_K")))
                        SQLBase.Append("   AND KEIYAKU_KIN_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_KIN_K")))
                        SQLBase.Append("   AND KEIYAKU_SIT_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_SIT_K")))
                        If Ora2ndMeiReader.GetString("KEIYAKU_KNAME_K") = "" Then
                            SQLBase.Append("   AND KEIYAKU_KNAME_K= ' '")
                        Else
                            SQLBase.Append("   AND KEIYAKU_KNAME_K= " & SQ(Ora2ndMeiReader.GetItem("KEIYAKU_KNAME_K")))
                        End If
                        SQLBase.Append("   AND KEIYAKU_KAMOKU_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_KAMOKU_K")))
                        SQLBase.Append("   AND KEIYAKU_KOUZA_K= " & SQ(Ora2ndMeiReader.GetString("KEIYAKU_KOUZA_K")))
                        SQLBase.Append("   AND FURIKIN_K= " & Ora2ndMeiReader.GetInt64("FURIKIN_K"))
                        If Ora2ndMeiReader.GetString("JYUYOUKA_NO_K") = "" Then
                            SQLBase.Append("   AND JYUYOUKA_NO_K= ' '")
                        Else
                            SQLBase.Append("   AND JYUYOUKA_NO_K= " & SQ(Ora2ndMeiReader.GetItem("JYUYOUKA_NO_K")))
                        End If
                        If Ora3rdMeiReader.DataReader(SQLBase) = True Then
                            InfoMeisaiMast.DUPLICATE_KBN = "020" '��d���������Ƃ���B
                            BLOG.Write("�f�[�^�Ǎ�", "����", "�U���˗��f�[�^��d�`�F�b�N�Y������")
                            Return True
                        End If
                        Ora2ndMeiReader.NextRead()
                    Loop
                End If
            End If
        Catch ex As Exception
            BLOG.Write("�U���˗��f�[�^�Q�d�`�F�b�N����", "���s", ex.Message & ":" & ex.StackTrace)
        Finally
            Ora1stMeiReader.Close()
            Ora2ndMeiReader.Close()
            Ora3rdMeiReader.Close()
        End Try

        BLOG.Write("�f�[�^�Ǎ�", "����", "�U���˗��f�[�^��d�`�F�b�N�Y���Ȃ�")
        Return False

    End Function

    ''' <summary>
    ''' ��t���ԊO�`�F�b�N
    ''' ��t���ԓ��̏������`�F�b�N����
    ''' </summary>
    ''' <param name="FuriDate">�U����</param>
    ''' <returns>True,False</returns>
    ''' <remarks>2017/12/07 �^�X�N�j����@�L���M��(RSV2�W����) added for ��K�͍\�z�Ή��i���������Ή��j</remarks>
    Public Function CheckInDatetime(ByVal FuriDate As String) As Boolean
        Dim MSG As String = ""
        Dim strSYORI As String = "��t���ԊO�`�F�b�N����"

        BLOG.Write(strSYORI & "�i�J�n�j", "����", "")

        Try
            Dim after1Date As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, 1, HolidayList)
            Dim strAfter1Date As String = String.Format("{0:yyyyMMdd}", after1Date)
            Dim SyoriDate As String = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)
            Dim SyoriTime As String = String.Format("{0:HHmmss}", CASTCommon.Calendar.Now)

            '�U����������������̎�t�͑S������
            If FuriDate < SyoriDate Then
                InfoMeisaiMast.TimeOver = False
                MSG = "�U������"
                Return False
            End If

            '�U�����������ŁA��t���ԊO�͂���
            If IsNumeric(INI_UKETUKE_JIKAN) Then
                If FuriDate = SyoriDate AndAlso SyoriTime >= INI_UKETUKE_JIKAN Then
                    InfoMeisaiMast.TimeOver = False
                    MSG = "��t���ԊO"
                    Return False
                End If
            End If

            Return True
        Catch ex As Exception
            BLOG.Write(strSYORI & "�i�I���j", "���s", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
            BLOG.Write(strSYORI & "�i�I���j", "����", MSG)
        End Try
    End Function

    ''' <summary>
    ''' ��t�����`�F�b�N
    ''' ����������t�J�n�������`�F�b�N����
    ''' </summary>
    ''' <param name="FuriDate">�U��/�U����</param>
    ''' <returns>True,False</returns>
    ''' <remarks>2017/12/07 �^�X�N�j����@�L���M��(RSV2�W����) added for ��K�͍\�z�Ή��i���������Ή��j</remarks>
    Public Function CheckKaisiDate(ByVal FuriDate As String) As Boolean
        Dim MSG As String = ""
        Dim strSYORI As String = "��t�����`�F�b�N"

        BLOG.Write(strSYORI & "�i�J�n�j", "����", "")

        Try
            Dim Kaisibi As Integer = 15     '��{��15���O

            'INI�t�@�C���̎�t�J�n�������l�̏ꍇ�AINI�t�@�C���̒l���g�p����
            If IsNumeric(INI_UKETUKE_KIJITU) Then
                Kaisibi = CInt(INI_UKETUKE_KIJITU)
            End If

            Dim beforeDate As Date = CASTCommon.GetEigyobi(ConvertDate(FuriDate), Kaisibi * -1, HolidayList)
            Dim strbeforeDate As String = String.Format("{0:yyyyMMdd}", beforeDate)
            Dim SyoriDate As String = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)

            '�J�n���O�͎󂯕t���Ȃ�
            If strbeforeDate > SyoriDate Then
                InfoMeisaiMast.TimeOver = False
                MSG = "��t�J�n���O" & strbeforeDate
                Return False
            End If

            Return True
        Catch ex As Exception
            BLOG.Write(strSYORI & "�i�I���j", "���s", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
            BLOG.Write(strSYORI & "�i�I���j", "����", MSG)
        End Try
    End Function

    ''' <summary>
    ''' ���������`�F�b�N
    ''' ���������X�P�W���[���}�X�^�̎������������`�F�b�N����
    ''' </summary>
    ''' <returns>True,False</returns>
    ''' <remarks>2017/12/07 �^�X�N�j����@�L���M��(RSV2�W����) added for ��K�͍\�z�Ή��i���������Ή��j</remarks>
    Public Function CheckMotikomiDate() As Boolean
        Dim MSG As String = ""
        Dim strSYORI As String = "���������`�F�b�N"

        BLOG.Write(strSYORI & "�i�J�n�j", "����", "")

        Try
            Dim SyoriDate As String = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)

            '�J�n���O�͎󂯕t���Ȃ�
            If MOTIKOMI_DATE < SyoriDate Then
                InfoMeisaiMast.TimeOver = False
                MSG = "����������"
                Return False
            End If

            Return True
        Catch ex As Exception
            BLOG.Write(strSYORI & "�i�I���j", "���s", ex.Message & ":" & ex.StackTrace)
            Return False
        Finally
            BLOG.Write(strSYORI & "�i�I���j", "����", MSG)
        End Try
    End Function

    ' �@�\�@ �F �Ԋҗ\������擾����
    '
    ' �߂�l �F 
    '
    ' ���l�@ �F
    '
    Protected Function GetHenkanYDate() As String
        Dim FurikaeDate As Date = ConvertDate(mInfoComm.INFOParameter.FURI_DATE)
        Dim OraInReader As New CASTCommon.MyOracleReader(OraDB)

        Return "00000000"
    End Function

    ' �@�\�@ �F ���O�o��
    '
    ' ����   �F ARG1 - �W���u��
    '           ARG2 - ����
    '
    ' ���l�@ �F
    '
    Protected Sub WriteBLog(ByVal aJob As String, ByVal aKekka As String, Optional ByVal aErr As String = "")
        If BLOG Is Nothing Then
            Return
        End If

        BLOG.Write(aJob, aKekka, aErr)

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Function GetEigyobiFmt(ByVal base As Date, ByVal days As Long) As Date
        Return GetEigyobi(base, days, HolidayList)
    End Function

    '
    ' �@�\�@ �F �����񂩂�C�w��̒�����؂���
    '
    ' �����@ �F ARG1 - ������
    ' �@�@�@ �@ ARG2 - ����
    '           ARG3 - �O�F���l�C�P�F�E�l
    '           ARG4 - ���ߕ���
    '
    ' �߂�l �F �؂�������̎c��̕�����
    '
    ' ���l�@ �F
    '
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer,
                    Optional ByVal align As Integer = 0, Optional ByVal pad As Char = " "c) As String
        Try
            If len = 0 Then
                Return ""
            End If

            ' �؂��镶����
            If align = 0 Then
                ' ���l
                value = value.PadRight(len, pad)
            Else
                ' �E�l
                value = value.PadLeft(len, pad)
            End If

            ' �؂��镶����
            Dim bt() As Byte = EncdJ.GetBytes(value)
            Return EncdJ.GetString(bt, 0, len)
        Catch ex As Exception
            Return New String(" "c, len)
        End Try
    End Function

    '
    ' �@�\�@ �F ���ʃ��R�[�h��ǂݍ���Ń`�F�b�N �㏈��
    '
    ' ���l�@ �F
    '
    Protected Friend Sub CheckDataFormatAfterFunou()
        Try
            ' ���v�˗������C���v�˗����z
            InfoMeisaiMast.TOTAL_KEN += InfoMeisaiMast.FURIKEN
            If InfoMeisaiMast.FURIKEN = 1 Then
                InfoMeisaiMast.TOTAL_KIN += InfoMeisaiMast.FURIKIN
            End If

            '2009.12.05 �O�~���בΉ� start
            If InfoMeisaiMast.FURIKIN > 0 Then
                InfoMeisaiMast.TOTAL_KEN2 += InfoMeisaiMast.FURIKEN
            End If
            '2009.12.05 �O�~���בΉ� end

            If InfoMeisaiMast.FURIKETU_CODE = 0 Then
                ' �U�֍ς݌����C�U�֍ς݋��z 
                InfoMeisaiMast.TOTAL_NORM_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    InfoMeisaiMast.TOTAL_NORM_KIN += InfoMeisaiMast.FURIKIN
                End If

                '2010.03.02 start NHK�Ή�
                If InfoMeisaiMast.FURIKIN > 0 Then
                    InfoMeisaiMast.TOTAL_NORM_KEN2 += InfoMeisaiMast.FURIKEN
                End If
                '2010.03.02 end

            Else
                ' �ُ팏���C�ُ���z
                InfoMeisaiMast.TOTAL_IJO_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    InfoMeisaiMast.TOTAL_IJO_KIN += InfoMeisaiMast.FURIKIN
                End If
            End If

            ' ���R�[�h�敪��ۑ�
            BeforeRecKbn = RecordData.Substring(0, 1)
        Catch ex As Exception

        End Try
    End Sub

    '*** Str Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***
    ' �@�\�@ �F �Ԋ҃w�b�_���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overridable Sub GetHenkanHeaderRecord()
        Call CheckRecord1()
    End Sub
    '*** End Add 2015/12/01 SO)�r�� for XML�t�H�[�}�b�g�ϊ��Ή� ***

    ' �@�\�@ �F �Ԋ҃f�[�^���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overridable Sub GetHenkanDataRecord()
        Call CheckDataFormatAfterFunou()
    End Sub

    ' �@�\�@ �F �Ԋ҃g���[�����R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overridable Sub GetHenkanTrailerRecord()
    End Sub

    ' �@�\�@ �F �ĐU�w�b�_�[���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overridable Sub GetSaifuriHeaderRecord(ByVal SAIFURI_DATE As String)
    End Sub

    ' �@�\�@ �F �ĐU�f�[�^���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overridable Sub GetSaifuriDataRecord()
        Call CheckDataFormatAfterFunou()
    End Sub

    ' �@�\�@ �F �Ԋ҃g���[�����R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overridable Sub GetSaifuriTrailerRecord(Optional ByVal SyoriKen As Long = 0, Optional ByVal SyoriKin As Long = 0,
                                                       Optional ByVal Write As Boolean = False)
    End Sub
    ' �@�\�@ �F �`�F�b�N�f�W�b�g�`�F�b�N
    '
    ' ����   �F ARG1 - �����ԍ�
    '
    ' �߂�l �F TRUE-����CFALSE-���s
    '
    ' ���l�@ �F
    '
    Protected Function CheckDigitCheck() As Boolean
        Dim Omomi() As String = {"379187432",
                            "987473219387432",
                            "987453259587432",
                            "579587432"}

        Dim Value As String
        Dim Kouza As String = InfoMeisaiMast.KEIYAKU_KOUZA.Substring(0, 6)
        Dim Digit As Integer

        For nOmo As Integer = 0 To Omomi.Length - 1
            Digit = 0
            Select Case nOmo
                Case 0
                    ' �T�ڋq�ԍ�
                    Value = InfoMeisaiMast.KEIYAKU_SIT & Kouza
                Case 1
                    ' �U�����ԍ�
                    Value = InfoMeisaiMast.KEIYAKU_KIN & InfoMeisaiMast.KEIYAKU_SIT & GetKamoku1(InfoMeisaiMast.KEIYAKU_KAMOKU) & Kouza
                Case 2
                    ' �V�����ԍ��i���̂Q�j
                    Value = InfoMeisaiMast.KEIYAKU_KIN & InfoMeisaiMast.KEIYAKU_SIT & GetKamoku2(InfoMeisaiMast.KEIYAKU_KAMOKU) & Kouza
                Case Else
                    ' �W�ڋq�ԍ�
                    Value = InfoMeisaiMast.KEIYAKU_SIT & Kouza
            End Select
            For i As Integer = 0 To Value.Length - 1
                Digit += CASTCommon.CAInt32(Value.Substring(i, 1)) * CASTCommon.CAInt32(Omomi(nOmo).Substring(i, 1))
            Next i

            If (10 - (Digit Mod 10)).ToString = InfoMeisaiMast.KEIYAKU_KOUZA.Substring(6) Then
                Return True
            End If
        Next nOmo

        Return False
    End Function

    Private Function GetKamoku1(ByVal kamoku As String) As String
        Select Case kamoku
            Case "1"
                ' ���ʗa��
                Return "02"
            Case "2"
                ' �����a��
                Return "01"
            Case "3", "4"
                ' �[�ŏ������C�E���a���
                Return "02"
        End Select

        Return "00"
    End Function

    Private Function GetKamoku2(ByVal kamoku As String) As String
        Select Case kamoku
            Case "1"
                ' ���ʗa��
                Return "02"
            Case "2"
                ' �����a��
                Return "01"
            Case "3"
                ' �[�ŏ�����
                Return "05"
            Case "4"
                ' �E���a���
                Return "37"
        End Select

        Return "00"
    End Function

    '�����U�̏ꍇ�A�U�֓����U����
    Private Function GetFuriDate(ByVal FSYORI_KBN As String) As String
        Try
            Select Case FSYORI_KBN
                Case "3"
                    Return "�U����"
                Case Else
                    Return "�U�֓�"
            End Select
        Catch ex As Exception
            Return "�U�֓�"
        End Try
    End Function

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N�i�X���[�G�X�p�j
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F 2017/01/16 saitou ���t�M��(RSV2�W��) added for �X���[�G�X�Ή�
    '
    Public Overridable Function CheckKekkaFormatSSS() As String
        mnErrorNumber = 0

        Try
            mnErrorNumber = 0

            InfoMeisaiMast.FURIKEN = 0

            ' 1���R�[�h�Ǎ�
            If GetFileDataFunou() > 0 Then
            End If

            Return ""
        Catch ex As Exception
            DataInfo.Message = ex.Message
            Return "ERR"
        End Try

        Return ""
    End Function

    '*** Str Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***
    '
    ' �@�\   �F �t�H�[�}�b�g��ݒ肷��
    ' ����   �F �t�H�[�}�b�g�敪
    '
    Public Sub setFmtKubun(ByVal kubun As String)

        FmtKubun = kubun

    End Sub

    '
    ' �@�\   �F �������ݗp�o�^�o�����\�b�h�����s����
    ' ����   �F �����R�[�h�z��
    '           �U�֓�
    ' �߂�l �F True - ���� �C False - �ُ�
    '
    Public Function CallTourokuExit(ByVal toriCode As String(), ByVal furiDate As String) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        If BLOG Is Nothing Then
            BLOG = New CASTCommon.BatchLOG("ClsFormat", "CFormat")
        End If

        sw = BLOG.Write_Enter1("ClsFormat.CallTourokuExit", "FmtKubun=" & FmtKubun)

        Dim rtn As Boolean = CallExitMethod("�������ݗp�o�^�o�����\�b�h", toriCode, furiDate)

        BLOG.Write_Exit1(sw, "ClsFormat.CallTourokuExit", "rtn=" & rtn)

        Return rtn

    End Function


    '
    ' �@�\   �F �Ԋҗp�o�^�o�����\�b�h�����s����
    ' ����   �F �����R�[�h�z��
    '           �U�֓�
    ' �߂�l �F True - ���� �C False - �ُ�
    '
    Public Function CallHenkanExit(ByVal toriCode As String(), ByVal furiDate As String) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        If BLOG Is Nothing Then
            BLOG = New CASTCommon.BatchLOG("ClsFormat", "CFormat")
        End If

        sw = BLOG.Write_Enter1("ClsFormat.CallHenkanExit", "FmtKubun=" & FmtKubun)

        Dim rtn As Boolean = CallExitMethod("�Ԋҗp�o�^�o�����\�b�h", toriCode, furiDate)

        BLOG.Write_Exit1(sw, "ClsFormat.CallHenkanExit", "rtn=" & rtn)

        Return rtn

    End Function


    '
    ' �@�\   �F �Ԋҗp�o�^�o�����\�b�h�����s����
    ' ����   �F �o����ʕ�����
    '           �����R�[�h�z��
    '           �U�֓�
    ' �߂�l �F True - ���� �C False - �ُ�
    '
    Private Function CallExitMethod(ByVal kindStr As String, ByVal toriCode As String(), ByVal furiDate As String) As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        sw = BLOG.Write_Enter1("ClsFormat.CallExitMethod")

        Dim xmlDoc As New ConfigXmlDocument
        Dim node As XmlNode
        Dim mXmlFile As String
        Dim mXmlRoot As XmlElement
        Dim mDllName As String = ""
        Dim mClassName As String = ""
        Dim mDllAsm As Assembly = Nothing
        Dim mClassInstance As Object = Nothing
        Dim methodname As String = ""


        Try
            ' �t�H�[�}�b�g�敪���ݒ肳��Ă��Ȃ��ꍇ�́ANOP
            If FmtKubun = "" Then
                Return True
            End If

            ' ini�t�@�C����XML_FORMAT_FLD����`����Ă��Ȃ��ꍇ�́ANOP
            Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
            If xmlFolderPath = "err" Or xmlFolderPath = "" Then
                BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", "XML_FORMAT_FLD��`�Ȃ�")
                Return True
            End If

            'XML�p�X�쐬
            If xmlFolderPath.EndsWith("\") = False Then
                xmlFolderPath &= "\"
            End If
            mXmlFile = "XML_FORMAT_" & FmtKubun & ".xml"

            ' XML�t�@�C���������ꍇ�́ANOP
            If System.IO.File.Exists(xmlFolderPath & mXmlFile) = False Then
                BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", xmlFolderPath & mXmlFile & "�t�@�C���Ȃ�")
                Return True
            End If

            ' XML�t�H�[�}�b�g��root�I�u�W�F�N�g����
            xmlDoc.Load(xmlFolderPath & mXmlFile)
            mXmlRoot = xmlDoc.DocumentElement


            ' �o�^�o�����\�b�h�w�肪�����ꍇ�́ANOP
            node = mXmlRoot.SelectSingleNode("�o�^�o��/" & kindStr)
            If node Is Nothing OrElse node.InnerText.Trim = "" Then
                BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", "�u�o�^�o��/" & kindStr & "�v�w��Ȃ�")
                Return True
            End If

            methodname = node.InnerText.Trim


            ' ��DLL��
            node = mXmlRoot.SelectSingleNode("����/��DLL��")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v�^�O����`����Ă��܂���B")
            End If
            mDllName = node.InnerText.Trim
            ' ��DLL�����[�h
            If mDllName <> "" Then
                Try
                    mDllAsm = System.Reflection.Assembly.LoadFrom(mDllName & ".dll")
                Catch ex As Exception
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v�^�O�Ŏw�肳�ꂽ" & mDllName & ".dll" &
                                        "��������܂���B�i" & ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End Try
            Else
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
            End If

            ' �ʃN���X��
            node = mXmlRoot.SelectSingleNode("����/�ʃN���X��")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�ʃN���X���v�^�O����`����Ă��܂���B")
            End If
            mClassName = node.InnerText.Trim

            If mClassName <> "" Then
                ' �ʃN���X���C���X�^���X��
                Try
                    mClassInstance = mDllAsm.CreateInstance(mDllName & "." & mClassName)
                    If mClassInstance Is Nothing Then
                        Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�ʃN���X���v�^�O�Ŏw�肳�ꂽ�N���X��" &
                                    mDllName & ".dll" & "�ɂ���܂���B�i" & ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                    End If
                Catch ex As Exception
                    Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/�ʃN���X���v�^�O�Ŏw�肳�ꂽ�N���X��" &
                                    mDllName & ".dll" & "�ɂ���܂���B�i" & ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
                End Try
            Else
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u����/��DLL���v,�u����/�ʃN���X���v����`����Ă��܂���B")
            End If

            '----------------------------------------------------------------
            ' �o�^�o���ʃ��\�b�h�ďo��
            '----------------------------------------------------------------
            BLOG.Write_LEVEL3("ClsFormat.CallExitMethod", kindStr & "�ďo���F" & methodname)

            Dim methodInfo As MethodInfo = mClassInstance.GetType.GetMethod(methodname)
            If methodInfo Is Nothing Then
                Throw New Exception(mXmlFile & "��`�G���[�F" & "�u�o�^�o��/" & kindStr & "�v�^�O�Ŏw�肳�ꂽ" &
                        methodname & "��������܂���B�i" & ConfigurationErrorsException.GetLineNumber(node) & "�s�ځj")
            End If

            Dim methodParams() As Object = {toriCode, furiDate, Me}
            If CType(methodInfo.Invoke(mClassInstance, methodParams), Boolean) = False Then
                BLOG.Write_Err("ClsFormat.CallExitMethod", kindStr & "�G���[", methodname)
                Return False
            End If

            Return True

        Catch ex As Exception
            BLOG.Write_Err("ClsFormat.CallExitMethod", ex)
            Return False

        Finally
            BLOG.Write_Exit1(sw, "ClsFormat.CallExitMethod")

        End Try

    End Function

    '*** End Upd 2015/12/01 SO)�r�� for �o�^�o���Ή� ***

    ''' <summary>
    ''' �U�֓����x���␳�����l��Ԃ�
    ''' </summary>
    ''' <param name="furiDate">�␳���̐U�֓�</param>
    ''' <returns>�x���␳��̐U�֓�</returns>
    ''' <remarks>2017/12/12 �^�X�N�j����@�L���M��(RSV2�W����) added for ��K�͍\�z�Ή��i�U�֓��x���␳�Ή��j</remarks>
    Public Function HoseiFurikaeDate(ByVal furiDate As String) As String
        Dim HoseiFlag As Boolean = False
        Dim dFuriDate As Date = CASTCommon.ConvertDate(furiDate)

        If mInfoComm Is Nothing OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "" OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T Is Nothing Then

            Return dFuriDate.ToString("yyyyMMdd")
        End If

        '���U�̏ꍇ�A�ΏۊO
        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "1" Then
            Return dFuriDate.ToString("yyyyMMdd")
        End If

        If IsEigyobi(dFuriDate, HolidayList) = False Then
            '�x�Ɠ�
            If mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "0" Then
                '���c�Ɠ��ɃX���C�h
                Return GetEigyobiFmt(dFuriDate, 1).ToString("yyyyMMdd")
            Else
                '�O�c�Ɠ��ɃX���C�h
                Return GetEigyobiFmt(dFuriDate, -1).ToString("yyyyMMdd")
            End If
        End If

        Return dFuriDate.ToString("yyyyMMdd")

    End Function

    ''' <summary>
    ''' �U�֓����x���␳�����l��Ԃ�(�x�����X�g�Ď擾)
    ''' </summary>
    ''' <param name="furiDate">�␳���̐U�֓�</param>
    ''' <param name="db">DB</param>
    ''' <returns>�x���␳��̐U�֓�</returns>
    ''' <remarks>2017/12/12 �^�X�N�j����@�L���M��(RSV2�W����) added for ��K�͍\�z�Ή��i�U�֓��x���␳�Ή��j</remarks>
    Public Function HoseiFurikaeDate2(ByVal furiDate As String, ByVal db As CASTCommon.MyOracle) As String
        Dim HoseiFlag As Boolean = False
        Dim dFuriDate As Date = CASTCommon.ConvertDate(furiDate)

        If mInfoComm Is Nothing OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "" OrElse
           mInfoComm.INFOToriMast.FURI_KYU_CODE_T Is Nothing Then

            Return dFuriDate.ToString("yyyyMMdd")
        End If

        '�x�����X�g�Ď擾
        HolidayList.Clear()
        Dim SQL As New StringBuilder("SELECT YASUMI_DATE_Y,YASUMI_NAME_Y FROM YASUMIMAST ORDER BY YASUMI_DATE_Y")
        Dim OraReader As New CASTCommon.MyOracleReader(db)
        If OraReader.DataReader(SQL) = True Then
            Do Until OraReader.EOF = True
                HolidayList.Add(OraReader.GetValue(0).ToString)

                OraReader.NextRead()
            Loop
        End If
        OraReader.Close()

        '���U�̏ꍇ�A�ΏۊO
        If mInfoComm.INFOToriMast.FSYORI_KBN_T <> "1" Then
            Return dFuriDate.ToString("yyyyMMdd")
        End If

        If IsEigyobi(dFuriDate, HolidayList) = False Then
            '�x�Ɠ�
            If mInfoComm.INFOToriMast.FURI_KYU_CODE_T = "0" Then
                '���c�Ɠ��ɃX���C�h
                Return GetEigyobiFmt(dFuriDate, 1).ToString("yyyyMMdd")
            Else
                '�O�c�Ɠ��ɃX���C�h
                Return GetEigyobiFmt(dFuriDate, -1).ToString("yyyyMMdd")
            End If
        End If

        Return dFuriDate.ToString("yyyyMMdd")

    End Function

    ''' <summary>
    ''' �c�Ɠ����ǂ������肷��
    ''' </summary>
    ''' <param name="base">�Ώۂ̐U�֓�</param>
    ''' <param name="holiday">�j�����X�g</param>
    ''' <returns>True:�c�Ɠ��^False:�x�Ɠ�</returns>
    ''' <remarks>2017/12/12 �^�X�N�j����@�L���M��(RSV2�W����) added for ��K�͍\�z�Ή��i�U�֓��x���␳�Ή��j</remarks>
    Public Function IsEigyobi(ByVal base As Date, ByRef holiday As System.Collections.ArrayList) As Boolean
        If base.DayOfWeek() = DayOfWeek.Saturday OrElse
           base.DayOfWeek() = DayOfWeek.Sunday Then

            Return False
        Else
            '�x������
            If holiday.BinarySearch(base.ToString("yyyyMMdd")) >= 0 Then
                Return False
            End If
        End If

        Return True
    End Function

End Class
