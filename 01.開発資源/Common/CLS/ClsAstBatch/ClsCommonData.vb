Option Strict On
Option Explicit On

Imports System

' �o�b�`������p�N���X
Public Class CommData


    ' �����}�X�^ �擾�p�\����
    Public Structure stTORIMAST
        '�����}�X�^����擾
        Dim FSYORI_KBN_T As String              '�U�֏����敪
        Dim TORIS_CODE_T As String              '������R�[�h 
        Dim TORIF_CODE_T As String              '����敛�R�[�h
        Dim BAITAI_CODE_T As String             '�}�̃R�[�h
        Dim LABEL_KBN_T As String               '���x���敪
        Dim CODE_KBN_T As String                '�R�[�h�敪
        Dim ITAKU_KANRI_CODE_T As String        '��\�ϑ��҃R�[�h
        Dim FILE_NAME_T As String               '�t�@�C����
        Dim FMT_KBN_T As String                 '�t�H�[�}�b�g�敪 
        Dim MULTI_KBN_T As String               '�}���`�敪
        Dim NS_KBN_T As String                  '���o���敪
        Dim SYUBETU_T As String                 '���
        Dim ITAKU_CODE_T As String              '�ϑ��҃R�[�h
        Dim ITAKU_NNAME_T As String             '�ϑ��Җ��J�i
        Dim TKIN_NO_T As String                 '�戵���Z�@�փR�[�h    
        Dim TSIT_NO_T As String                 '�戵�x�X�R�[�h
        Dim KAMOKU_T As String                  '�Ȗ�
        Dim KOUZA_T As String                   '�����ԍ�
        Dim MOTIKOMI_KBN_T As String            '�����敪
        Dim SOUSIN_KBN_T As String              '���M�敪
        Dim TAKO_KBN_T As String                '���s�敪
        Dim JIFURICHK_KBN_T As String           '���U�_��쐬�敪
        Dim TEKIYOU_KBN_T As String             '�E�v�敪
        Dim KTEKIYOU_T As String                '�J�i�E�v
        Dim NTEKIYOU_T As String                '�����E�v
        Dim FURI_CODE_T As String               '�U�փR�[�h
        Dim KIGYO_CODE_T As String              '��ƃR�[�h
        Dim SYUMOKU_CODE_T As String            '��ڃR�[�h
        Dim FUKA_CODE_T As String               '�t���R�[�h
        Dim ITAKU_KNAME_T As String             '�ϑ��Җ�����
        Dim YUUBIN_T As String                  '�X�֔ԍ�
        Dim DENWA_T As String                   '�d�b�ԍ�
        Dim FAX As String                       '�e�`�w�ԍ�
        Dim KOKYAKU_NO_T As String              '�ڋq�ԍ�
        Dim KANREN_KIGYO_CODE_T As String       '�֘A��ƃR�[�h
        Dim ITAKU_NJYU_T As String              '�ϑ��ҏZ������
        Dim YUUBIN_KNAME_T As String            '�X����J�i
        Dim YUUBIN_NNAME_T As String            '�X���抿��
        Dim FURI_KYU_CODE_T As String           '�U�֋x���V�t�g
        Dim SFURI_FLG_T As String               '�ĐU�_��
        Dim SFURI_FCODE_T As String             '�ĐU���R�[�h
        Dim SFURI_DAY_T As String               '�ĐU��
        Dim SFURI_KIJITSU_T As String           '���t�敪(�ĐU)
        Dim SFURI_KYU_CODE_T As String          '�ĐU�x���V�t�g
        Dim KEIYAKU_DATE_T As String            '�_���
        Dim KAISI_DATE_T As String              '�J�n�N����
        Dim SYURYOU_DATE_T As String            '�I���N����
        Dim MOTIKOMI_KIJITSU_T As String        '��������
        Dim IRAISYO_YDATE_T As String           '�����^���(�˗���)
        Dim IRAISYO_KIJITSU_T As String         '���t�敪(�˗���)
        Dim IRAISYO_KYU_CODE_T As String        '�˗����x���V�t�g
        Dim IRAISYO_KBN_T As String             '�˗������
        Dim IRAISYO_SORT_T As String            '�˗����o�͏�
        Dim TEIGAKU_KBN_T As String             '��z�敪
        Dim UMEISAI_KBN_T As String             '��t���ו\ �o�͋敪 0:��ΏہC1:�X�ԃ\�[�g�C2:��\�[�g,3:�G���[���̂�
        Dim FUNOU_MEISAI_KBN_T As String        '�s�\���ʖ��ו\�o�͋敪
        Dim KEKKA_HENKYAKU_KBN_T As String      '���ʕԋp�v��
        Dim KEKKA_MEISAI_KBN_T As String        '���ʖ��׃f�[�^�쐬�敪
        Dim FKEKKA_TBL_T As String              '�U�֌��ʕϊ��e�[�u���h�c�s
        Dim KESSAI_KBN_T As String              '���ϋ敪
        Dim TORIMATOME_SIT_NO_T As String       '�Ƃ�܂ƂߓX
        Dim HONBU_KOUZA_T As String             '�{���ʒi�����ԍ�
        Dim KESSAI_DAY_T As String              '�����^���(����)
        Dim KESSAI_KIJITSU As String            '���t�敪(����)
        Dim KESSAI_KYU_CODE_T As String         '���ϋx���V�t�g
        Dim TUKEKIN_NO_T As String              '���ϋ��Z�@��
        Dim TUKESIT_NO_T As String              '���ώx�X
        Dim TUKEKAMOKU_T As String              '���ωȖ�
        Dim TUKE_KOUZA_T As String              '���ό����ԍ�
        Dim TUKEMEIGI_KNAME_T As String         '���ϖ��`�l(�J�i)
        Dim BIKOU1_T As String                  '���l�P
        Dim BIKOU2_T As String                  '���l�Q
        Dim TESUUTYO_SIT_T As String            '�萔�������x�X
        Dim TESUUTYO_KAMOKU_T As String         '�萔�������Ȗ�
        Dim TESUUTYO_KOUZA_T As String          '�萔�����������ԍ�
        Dim TESUUTYO_KBN_T As String            '�萔�������敪
        Dim TESUURYO_PATN_T As String           '�萔���������@
        Dim TESUUMAT_NO_T As String             '�萔���W�v����
        Dim TESUUTYO_DAY_T As Integer           '�����^���(�萔������)
        Dim TESUUTYO_KIJITSU_T As String        '���t�敪(�萔������)
        Dim TESUU_KYU_CODE_T As String          '�萔���x���V�t�g
        Dim SEIKYU_KBN_T As String              '�萔�������敪
        Dim KIHTESUU_T As String                '�U�֎萔���P��
        Dim SYOUHI_KBN_T As String              '����ŋ敪
        Dim SOURYO_T As String                  '����
        Dim KOTEI_TESUU1_T As String            '�Œ�萔���P
        Dim KOTEI_TESUU2_T As String            '�Œ�萔���Q
        Dim TESUUMAT_MONTH_T As String          '�W�v���
        Dim TESUUMAT_ENDDAY_T As String         '�W�v�I����
        Dim TESUUMAT_KIJYUN_T As String         '�W�v�
        Dim TESUUMAT_PATN_T As String           '�W�v���@
        Dim TESUU_GRP_T As String               '�W�v��Ƃf�q�o
        Dim TESUU_TABLE_T As String             '�U���萔����h�c
        Dim TESUU_A1_T As Integer               '��萔��A1
        Dim TESUU_A2_T As Integer               '��萔��A2
        Dim TESUU_A3_T As Integer               '��萔��A3
        Dim TESUU_B1_T As Integer               '��萔��B1
        Dim TESUU_B2_T As Integer               '��萔��B2
        Dim TESUU_B3_T As Integer               '��萔��B3
        Dim TESUU_C1_T As Integer               '��萔��C1
        Dim TESUU_C2_T As Integer               '��萔��C2
        Dim TESUU_C3_T As Integer               '��萔��C3
        Dim ENC_KBN_T As String                 '�Í��������敪
        Dim ENC_OPT1_T As String                '�`�d�r
        Dim ENC_KEY1_T As String                '�Í����L�[�P
        Dim ENC_KEY2_T As String                '�Í����L�[�Q
        Dim SAKUSEI_DATE_T As String            '�쐬��
        Dim KOUSIN_DATE_T As String             '�X�V��
        Dim YOBI1_T As String                   '�\���P
        Dim YOBI2_T As String                   '�\���Q
        Dim YOBI3_T As String                   '�\���R
        Dim YOBI4_T As String                   '�\���S
        Dim YOBI5_T As String                   '�\���T
        Dim YOBI6_T As String                   '�\���U
        Dim YOBI7_T As String                   '�\���V
        Dim YOBI8_T As String                   '�\���W
        Dim YOBI9_T As String                   '�\���X
        Dim YOBI10_T As String                  '�\���P�O

        '2017/12/04 �^�X�N�j���� ADD �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ START
        'TORIMAST_SUB�p�̍���
        Dim AITE_CNT_CODE_T As String           '�`������Z���^�[�R�[�h
        Dim TOHO_CNT_CODE_T As String           '�`�������Z���^�[�R�[�h
        Dim DENSO_FILE_ID_T As String           '�`���t�@�C��ID
        Dim HANSOU_KBN_T As String              '�������@
        Dim HANSOU_ROOT1_T As String            '�������[�g�P
        Dim HANSOU_ROOT2_T As String            '�������[�g�Q
        Dim HANSOU_ROOT3_T As String            '�������[�g�R
        Dim HENKYAKU_SIT_NO_T As String         '�ԋp�x�X
        Dim SYOUGOU_KBN_T As String             '�ƍ��v�ۋ敪
        Dim KEIYAKU_NO_T As String              '�_��ԍ�
        Dim MAE_SYORI_T As String               '�ʑO����
        Dim ATO_SYORI_T As String               '�ʌ㏈��  
        Dim TOKKIJIKOU1_T As String             '���L�����P
        Dim TOKKIJIKOU2_T As String             '���L�����Q
        Dim TOKKIJIKOU3_T As String             '���L�����R
        Dim TOKKIJIKOU4_T As String             '���L�����S
        Dim KYUUYO_KBN_T As String              '���^�K�p�敪�i���U�̂݁j
        Dim CUSTOM_NUM01_T As Long              '�\�����l�O�P
        Dim CUSTOM_NUM02_T As Long              '�\�����l�O�Q
        Dim CUSTOM_NUM03_T As Long              '�\�����l�O�R
        Dim CUSTOM_NUM04_T As Long              '�\�����l�O�S
        Dim CUSTOM_NUM05_T As Long              '�\�����l�O�T
        Dim CUSTOM_NUM06_T As Long              '�\�����l�O�U
        Dim CUSTOM_NUM07_T As Long              '�\�����l�O�V
        Dim CUSTOM_NUM08_T As Long              '�\�����l�O�W
        Dim CUSTOM_NUM09_T As Long              '�\�����l�O�X
        Dim CUSTOM_NUM10_T As Long              '�\�����l�P�O
        Dim CUSTOM_NUM11_T As Long              '�\�����l�P�P
        Dim CUSTOM_NUM12_T As Long              '�\�����l�P�Q
        Dim CUSTOM_NUM13_T As Long              '�\�����l�P�R
        Dim CUSTOM_NUM14_T As Long              '�\�����l�P�S
        Dim CUSTOM_NUM15_T As Long              '�\�����l�P�T
        Dim CUSTOM_NUM16_T As Long              '�\�����l�P�U
        Dim CUSTOM_NUM17_T As Long              '�\�����l�P�V
        Dim CUSTOM_NUM18_T As Long              '�\�����l�P�W
        Dim CUSTOM_NUM19_T As Long              '�\�����l�P�X
        Dim CUSTOM_NUM20_T As Long              '�\�����l�Q�O
        Dim CUSTOM_NUM21_T As Long              '�\�����l�Q�P
        Dim CUSTOM_NUM22_T As Long              '�\�����l�Q�Q
        Dim CUSTOM_NUM23_T As Long              '�\�����l�Q�R
        Dim CUSTOM_NUM24_T As Long              '�\�����l�Q�S
        Dim CUSTOM_NUM25_T As Long              '�\�����l�Q�T
        Dim CUSTOM_NUM26_T As Long              '�\�����l�Q�U
        Dim CUSTOM_NUM27_T As Long              '�\�����l�Q�V
        Dim CUSTOM_NUM28_T As Long              '�\�����l�Q�W
        Dim CUSTOM_NUM29_T As Long              '�\�����l�Q�X
        Dim CUSTOM_NUM30_T As Long              '�\�����l�R�O
        Dim CUSTOM_NUM31_T As Long              '�\�����l�R�P
        Dim CUSTOM_NUM32_T As Long              '�\�����l�R�Q
        Dim CUSTOM_NUM33_T As Long              '�\�����l�R�R
        Dim CUSTOM_NUM34_T As Long              '�\�����l�R�S
        Dim CUSTOM_NUM35_T As Long              '�\�����l�R�T
        Dim CUSTOM_NUM36_T As Long              '�\�����l�R�U
        Dim CUSTOM_NUM37_T As Long              '�\�����l�R�V
        Dim CUSTOM_NUM38_T As Long              '�\�����l�R�W
        Dim CUSTOM_NUM39_T As Long              '�\�����l�R�X
        Dim CUSTOM_NUM40_T As Long              '�\�����l�S�O
        Dim CUSTOM_NUM41_T As Long              '�\�����l�S�P
        Dim CUSTOM_NUM42_T As Long              '�\�����l�S�Q
        Dim CUSTOM_NUM43_T As Long              '�\�����l�S�R
        Dim CUSTOM_NUM44_T As Long              '�\�����l�S�S
        Dim CUSTOM_NUM45_T As Long              '�\�����l�S�T
        Dim CUSTOM_NUM46_T As Long              '�\�����l�S�U
        Dim CUSTOM_NUM47_T As Long              '�\�����l�S�V
        Dim CUSTOM_NUM48_T As Long              '�\�����l�S�W
        Dim CUSTOM_NUM49_T As Long              '�\�����l�S�X
        Dim CUSTOM_NUM50_T As Long              '�\�����l�T�O
        Dim CUSTOM_VCR01_T As String            '�\�������O�P
        Dim CUSTOM_VCR02_T As String            '�\�������O�Q
        Dim CUSTOM_VCR03_T As String            '�\�������O�R
        Dim CUSTOM_VCR04_T As String            '�\�������O�S
        Dim CUSTOM_VCR05_T As String            '�\�������O�T
        Dim CUSTOM_VCR06_T As String            '�\�������O�U
        Dim CUSTOM_VCR07_T As String            '�\�������O�V
        Dim CUSTOM_VCR08_T As String            '�\�������O�W
        Dim CUSTOM_VCR09_T As String            '�\�������O�X
        Dim CUSTOM_VCR10_T As String            '�\�������P�O
        Dim CUSTOM_VCR11_T As String            '�\�������P�P
        Dim CUSTOM_VCR12_T As String            '�\�������P�Q
        Dim CUSTOM_VCR13_T As String            '�\�������P�R
        Dim CUSTOM_VCR14_T As String            '�\�������P�S
        Dim CUSTOM_VCR15_T As String            '�\�������P�T
        Dim CUSTOM_VCR16_T As String            '�\�������P�U
        Dim CUSTOM_VCR17_T As String            '�\�������P�V
        Dim CUSTOM_VCR18_T As String            '�\�������P�W
        Dim CUSTOM_VCR19_T As String            '�\�������P�X
        Dim CUSTOM_VCR20_T As String            '�\�������Q�O
        Dim CUSTOM_VCR21_T As String            '�\�������Q�P
        Dim CUSTOM_VCR22_T As String            '�\�������Q�Q
        Dim CUSTOM_VCR23_T As String            '�\�������Q�R
        Dim CUSTOM_VCR24_T As String            '�\�������Q�S
        Dim CUSTOM_VCR25_T As String            '�\�������Q�T
        Dim CUSTOM_VCR26_T As String            '�\�������Q�U
        Dim CUSTOM_VCR27_T As String            '�\�������Q�V
        Dim CUSTOM_VCR28_T As String            '�\�������Q�W
        Dim CUSTOM_VCR29_T As String            '�\�������Q�X
        Dim CUSTOM_VCR30_T As String            '�\�������R�O
        Dim CUSTOM_VCR31_T As String            '�\�������R�P
        Dim CUSTOM_VCR32_T As String            '�\�������R�Q
        Dim CUSTOM_VCR33_T As String            '�\�������R�R
        Dim CUSTOM_VCR34_T As String            '�\�������R�S
        Dim CUSTOM_VCR35_T As String            '�\�������R�T
        Dim CUSTOM_VCR36_T As String            '�\�������R�U
        Dim CUSTOM_VCR37_T As String            '�\�������R�V
        Dim CUSTOM_VCR38_T As String            '�\�������R�W
        Dim CUSTOM_VCR39_T As String            '�\�������R�X
        Dim CUSTOM_VCR40_T As String            '�\�������S�O
        Dim CUSTOM_VCR41_T As String            '�\�������S�P
        Dim CUSTOM_VCR42_T As String            '�\�������S�Q
        Dim CUSTOM_VCR43_T As String            '�\�������S�R
        Dim CUSTOM_VCR44_T As String            '�\�������S�S
        Dim CUSTOM_VCR45_T As String            '�\�������S�T
        Dim CUSTOM_VCR46_T As String            '�\�������S�U
        Dim CUSTOM_VCR47_T As String            '�\�������S�V
        Dim CUSTOM_VCR48_T As String            '�\�������S�W
        Dim CUSTOM_VCR49_T As String            '�\�������S�X
        Dim CUSTOM_VCR50_T As String            '�\�������T�O
        '2017/12/04 �^�X�N�j���� ADD �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ END

        Dim TUKI_T() As String                  '���ʏ����t���O

        '����}�X�^�ɑ��݂��Ȃ�����
        Dim TKIN_NNAME_N As String              '�戵���Z�@�֖�
        Dim TSIT_NNAME_N As String              '�戵�x�X��
        Dim TKIN_KNAME_N As String              '�戵���Z�@�֖��J�i
        Dim TSIT_KNAME_N As String              '�戵�x�X���J�i
        Dim TORIMATOME_SIT_NNAME_N As String    '�Ƃ�܂ƂߓX��
        Dim CYCLE_T As String                   '�T�C�N���Ǘ� 0:�����񎝍��Ȃ��C1:�����񎝍�����
        Dim KIJITU_KANRI_T As String            '�����Ǘ��v�� 0:�����Ǘ��Ȃ��C1:�����Ǘ�����
        Dim TESUUTYO_NO_T As Integer            '�萔����������
        '2017/12/04 �^�X�N�j���� DEL �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ START
        'TORIMAST_SUB�Œǉ��ɂȂ����̂ŁA�������͍폜����
        'Dim DENSO_FILE_ID_T As String           '�`���t�@�C��ID
        '2017/12/04 �^�X�N�j���� DEL �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ END

        ReadOnly Property EOF() As Boolean
            Get
                If ITAKU_KNAME_T Is Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
    End Structure

    ' �A�g���p�\���́i�N���p�����[�^�̒l�j
    Public Structure stPARAMETER
        Dim FURI_DATE As String             '�U�֓�
        Dim CODE_KBN As String              '�R�[�h�敪
        Dim FMT_KBN As String               '�t�H�[�}�b�g�敪
        Dim BAITAI_CODE As String           '�}�̃R�[�h
        Dim LABEL_KBN As String             '���x���敪

        Dim RENKEI_FILENAME As String       '�A�g�t�@�C����
        Dim ENC_KBN As String               '�Í����敪
        Dim ENC_KEY1 As String              '�Í����L�[
        Dim ENC_KEY2 As String              '�Í���IV�L�[
        Dim ENC_OPT1 As String              '�`�d�r
        Dim CYCLENO As String               '�T�C�N����

        Dim JOBTUUBAN As Integer            '�W���u�ʔ�

        Dim FSYORI_KBN As String            '�U�֏����敪
        Dim TORIS_CODE As String            '������R�[�h
        Dim TORIF_CODE As String            '����敛�R�[�h

        Dim TIME_STAMP As String            '�^�C���X�^���v

        Dim MODE1 As String                 '�������[�h

        Dim RENKEI_KBN As String            '�A�g�敪

        ' �����R�[�h
        Public Property TORI_CODE() As String
            Get
                Return TORIS_CODE & TORIF_CODE
            End Get
            Set(ByVal Value As String)
                If Value.Length >= 11 Then
                    TORIS_CODE = Value.Substring(0, 10) '������R�[�h10��
                    TORIF_CODE = Value.Substring(10)    '����敛�R�[�h2��
                End If
            End Set
        End Property
    End Structure

    ' ���b�Z�[�W
    Public Message As String

    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- START
    Public Syorikekka_Bikou As String = ""      '�������ʊm�F�\�̔��l�ɏo�͂��郁�b�Z�[�W
    '2017/12/07 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���������Ή��j---------------------- END

    Public MainDB As CASTCommon.MyOracle

    ' �����}�X�^ ���
    Private mInfoTorimast As stTORIMAST
    Public Property INFOToriMast() As stTORIMAST
        Get
            Return mInfoTorimast
        End Get
        Set(ByVal Value As stTORIMAST)
            mInfoTorimast = Value
        End Set
    End Property

    ' �p�����[�^ ���
    Private mInfoParam As stPARAMETER
    Public Property INFOParameter() As stPARAMETER
        Get
            Return mInfoParam
        End Get
        Set(ByVal Value As stPARAMETER)
            mInfoParam = Value
        End Set
    End Property

    Sub New(ByVal db As CASTCommon.MyOracle)
        MainDB = db
    End Sub

    ' New
    Sub New(ByVal torimast As stTORIMAST, ByVal param As stPARAMETER)
        Message = ""

        ' �����}�X�^���
        mInfoTorimast = torimast

        ' �A�g���
        mInfoParam = param
    End Sub

    '
    ' �@�\�@ �F �����}�X�^��� �擾
    '
    ' �����@ �F ARG1 - ������R�[�h
    '           ARG2 - ����敛�R�[�h
    '
    ' �߂�l �F �����}�X�^���
    '
    ' ���l�@ �F 
    '
    Public Sub GetTORIMAST(ByVal toris As String, ByVal torif As String)
        '���U �����}�X�^������
        Call SelectTORIMAST("1", toris.Trim, torif.Trim)
        If mInfoTorimast.EOF = True Then
            '�U�� �����}�X�^������
            Call SelectTORIMAST("3", toris.Trim, torif.Trim)
        End If

        'Return mInfoTorimast
    End Sub

    '
    ' �@�\�@ �F �����}�X�^��� �擾
    '
    ' �����@ �F ARG1 - �U�֏����敪
    '           ARG2 - ������R�[�h
    '           ARG3 - ����敛�R�[�h
    '
    ' �߂�l �F �����}�X�^���
    '
    ' ���l�@ �F 
    '
    Public Function SelectTORIMAST(ByVal fsyorikbn As String, ByVal toris As String, ByVal torif As String) As stTORIMAST
        Dim SQL As New System.Text.StringBuilder(2048)
        Dim CloseFlag As Boolean = False

        If MainDB Is Nothing Then
            CloseFlag = True
            MainDB = New CASTCommon.MyOracle
        End If

        Try
            '�����}�X�^�擾
            SQL.Append("SELECT")
            SQL.Append(" FSYORI_KBN_T")
            SQL.Append(",TORIS_CODE_T")
            SQL.Append(",TORIF_CODE_T")
            SQL.Append(",ITAKU_KNAME_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",BAITAI_CODE_T")
            SQL.Append(",ITAKU_KANRI_CODE_T")
            SQL.Append(",FMT_KBN_T")
            SQL.Append(",CODE_KBN_T")
            SQL.Append(",SOUSIN_KBN_T")
            SQL.Append(",FILE_NAME_T")
            SQL.Append(",TKIN_NO_T")
            SQL.Append(",TSIT_NO_T")
            SQL.Append(",TORIMATOME_SIT_T")
            SQL.Append(",KAMOKU_T")
            SQL.Append(",KOUZA_T")
            SQL.Append(",MULTI_KBN_T")
            SQL.Append(",ITAKU_CODE_T")
            SQL.Append(",SYUBETU_T")
            SQL.Append(",TENMAST.KIN_NNAME_N TKIN_NNAME_N")
            SQL.Append(",TENMAST.SIT_NNAME_N TSIT_NNAME_N")
            SQL.Append(",TENMAST.KIN_KNAME_N TKIN_KNAME_N")
            SQL.Append(",TENMAST.SIT_KNAME_N TSIT_KNAME_N")
            SQL.Append(",TORI_TENMAST.SIT_NNAME_N TORIMATOME_SIT_NNAME_N")
            SQL.Append(",UMEISAI_KBN_T")
            SQL.Append(",MOTIKOMI_KIJITSU_T")
            SQL.Append(",TESUUTYO_KIJITSU_T")
            SQL.Append(",KESSAI_KYU_CODE_T")
            SQL.Append(",TESUUTYO_DAY_T")
            SQL.Append(",TESUUTYO_KBN_T")
            SQL.Append(",IRAISYO_KIJITSU_T")
            SQL.Append(",IRAISYO_YDATE_T")
            SQL.Append(",IRAISYO_KYU_CODE_T")
            SQL.Append(",TEIGAKU_KBN_T")
            SQL.Append(",KOKYAKU_NO_T")
            For i As Integer = 1 To 12
                SQL.Append(",TUKI" & i & "_T")
            Next
            SQL.Append(" ,ENC_KBN_T ")
            SQL.Append(" ,ENC_KEY1_T")
            SQL.Append(" ,ENC_KEY2_T")
            SQL.Append(" ,ENC_OPT1_T")
            SQL.Append(",TESUU_A1_T")
            SQL.Append(",TESUU_A2_T")
            SQL.Append(",TESUU_A3_T")
            SQL.Append(",TESUU_B1_T")
            SQL.Append(",TESUU_B2_T")
            SQL.Append(",TESUU_B3_T")
            SQL.Append(",TESUU_C1_T")
            SQL.Append(",TESUU_C2_T")
            SQL.Append(",TESUU_C3_T")

            '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- START
            SQL.Append(",FURI_KYU_CODE_T")
            '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- END
            '2018/01/16 saitou �L���M��(RSV2�W��) ADD �_����ԓ��擾 -------------------- START
            SQL.Append(",LABEL_KBN_T")
            SQL.Append(",KEIYAKU_DATE_T")
            SQL.Append(",KAISI_DATE_T")
            SQL.Append(",SYURYOU_DATE_T")
            '2018/01/16 saitou �L���M��(RSV2�W��) ADD ----------------------------------- END

            If fsyorikbn = "1" Then
                ' ���U �����}�X�^
                SQL.Append(",FURI_CODE_T")          '�U�փR�[�h
                SQL.Append(",KIGYO_CODE_T")         '��ƃR�[�h
                SQL.Append(",MOTIKOMI_KBN_T")       '�����敪
                SQL.Append(",TAKO_KBN_T")           '���s�敪
                SQL.Append(",TEKIYOU_KBN_T")        '�E�v�敪
                SQL.Append(",KTEKIYOU_T")           '�J�i�E�v
                SQL.Append(",NTEKIYOU_T")           '�����E�v
                SQL.Append(",NS_KBN_T")             '���o���敪
                SQL.Append(",'0' CYCLE_T")          '�����񎝍��Ȃ�
                SQL.Append(",'1' KIJITU_KANRI_T")   '�����Ǘ�����
                SQL.Append(",SFURI_FLG_T")          '�ĐU�_��
                SQL.Append(",SFURI_DAY_T")          '�ĐU���
                SQL.Append(",SFURI_KIJITSU_T")      '�ĐU�敪
                SQL.Append(",SFURI_KYU_CODE_T")     '�ĐU�x���R�[�h
                SQL.Append(",FUNOU_MEISAI_KBN_T")   '���ʖ��׃f�[�^�쐬�敪
                SQL.Append(",FKEKKA_TBL_T")         '�U�֌��ʕϊ��e�[�u���h�c�s
                SQL.Append(",SEIKYU_KBN_T")         '2009/09/12�@�ǉ�
                SQL.Append(",'00' SYUMOKU_CODE_T")  '��ڃR�[�h
                SQL.Append(",'000' FUKA_CODE_T")    '�t���R�[�h
                SQL.Append(",TESUU_TABLE_ID_T")

                SQL.Append(" FROM ")
                SQL.Append("  TORIMAST")
            Else
                ' �U�� �����}�X�^
                SQL.Append(",FURI_CODE_T")          '�U�փR�[�h
                SQL.Append(",KIGYO_CODE_T")         '��ƃR�[�h
                SQL.Append(",'0' MOTIKOMI_KBN_T")   '�����敪
                SQL.Append(",'1' TAKO_KBN_T")       '���s�敪
                SQL.Append(",TEKIYOU_KBN_T")        '�E�v�敪
                SQL.Append(",KTEKIYOU_T")           '�J�i�E�v
                SQL.Append(",NTEKIYOU_T")           '�����E�v
                SQL.Append(",NS_KBN_T")             '���o���敪
                SQL.Append(",CYCLE_T")              '�����񎝍��Ȃ�
                SQL.Append(",KIJITU_KANRI_T")       '�����Ǘ�����
                SQL.Append(",'0' SFURI_FLG_T")      '�ĐU�t���O
                SQL.Append(",NULL SFURI_DAY_T")     '�ĐU���
                SQL.Append(",NULL SFURI_KIJITSU_T") '�ĐU�敪
                SQL.Append(",NULL SFURI_KYU_CODE_T") '�ĐU�x���R�[�h
                SQL.Append(",'0' FUNOU_MEISAI_KBN_T") '���ʖ��׃f�[�^�쐬�敪
                SQL.Append(",'0' FKEKKA_TBL_T")     '�U�֌��ʕϊ��e�[�u���h�c�s
                SQL.Append(",NULL SEIKYU_KBN_T")
                SQL.Append(",SYUMOKU_CODE_T")  '��ڃR�[�h
                SQL.Append(",FUKA_CODE_T")    '�t���R�[�h
                SQL.Append(",TESUU_TABLE_ID_T")

                SQL.Append(" FROM ")
                SQL.Append("  S_TORIMAST")
            End If

            SQL.Append(" ,TENMAST TENMAST")
            SQL.Append(" ,TENMAST TORI_TENMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = '" & fsyorikbn & "'")
            SQL.Append(" AND TORIS_CODE_T = '" & toris & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & torif & "'")
            SQL.Append(" AND TKIN_NO_T = TENMAST.KIN_NO_N(+)")
            SQL.Append(" AND TSIT_NO_T = TENMAST.SIT_NO_N(+)")
            SQL.Append(" AND TKIN_NO_T = TORI_TENMAST.KIN_NO_N(+)")
            SQL.Append(" AND TORIMATOME_SIT_T = TORI_TENMAST.SIT_NO_N(+)")

            Dim SQLReader As New CASTCommon.MyOracleReader(MainDB)
            If SQLReader.DataReader(SQL) = True Then
                mInfoTorimast.FSYORI_KBN_T = SQLReader.GetString("FSYORI_KBN_T")
                mInfoTorimast.TORIS_CODE_T = SQLReader.GetString("TORIS_CODE_T")
                mInfoTorimast.TORIF_CODE_T = SQLReader.GetString("TORIF_CODE_T")
                mInfoTorimast.ITAKU_KNAME_T = SQLReader.GetString("ITAKU_KNAME_T")
                mInfoTorimast.ITAKU_NNAME_T = SQLReader.GetString("ITAKU_NNAME_T")
                mInfoTorimast.FURI_CODE_T = SQLReader.GetString("FURI_CODE_T")
                mInfoTorimast.KIGYO_CODE_T = SQLReader.GetString("KIGYO_CODE_T")
                mInfoTorimast.BAITAI_CODE_T = SQLReader.GetString("BAITAI_CODE_T").PadLeft(2, "0"c)
                mInfoTorimast.ITAKU_KANRI_CODE_T = SQLReader.GetString("ITAKU_KANRI_CODE_T")
                mInfoTorimast.FMT_KBN_T = SQLReader.GetString("FMT_KBN_T").PadLeft(2, "0"c)
                mInfoTorimast.MOTIKOMI_KBN_T = SQLReader.GetString("MOTIKOMI_KBN_T")
                mInfoTorimast.CODE_KBN_T = SQLReader.GetString("CODE_KBN_T")
                mInfoTorimast.SOUSIN_KBN_T = SQLReader.GetString("SOUSIN_KBN_T")
                mInfoTorimast.FILE_NAME_T = SQLReader.GetString("FILE_NAME_T")
                mInfoTorimast.TAKO_KBN_T = SQLReader.GetString("TAKO_KBN_T")
                mInfoTorimast.TKIN_NO_T = SQLReader.GetString("TKIN_NO_T")
                mInfoTorimast.TKIN_NNAME_N = SQLReader.GetString("TKIN_NNAME_N")
                mInfoTorimast.TSIT_NO_T = SQLReader.GetString("TSIT_NO_T")
                mInfoTorimast.TSIT_NNAME_N = SQLReader.GetString("TSIT_NNAME_N")
                mInfoTorimast.TKIN_KNAME_N = SQLReader.GetString("TKIN_KNAME_N")
                mInfoTorimast.TSIT_KNAME_N = SQLReader.GetString("TSIT_KNAME_N")
                mInfoTorimast.TORIMATOME_SIT_NO_T = SQLReader.GetString("TORIMATOME_SIT_T")
                mInfoTorimast.TORIMATOME_SIT_NNAME_N = SQLReader.GetString("TORIMATOME_SIT_NNAME_N")
                mInfoTorimast.KAMOKU_T = SQLReader.GetString("KAMOKU_T")
                mInfoTorimast.KOUZA_T = SQLReader.GetString("KOUZA_T")
                mInfoTorimast.MULTI_KBN_T = SQLReader.GetString("MULTI_KBN_T")
                mInfoTorimast.NS_KBN_T = SQLReader.GetString("NS_KBN_T")
                mInfoTorimast.ITAKU_CODE_T = SQLReader.GetString("ITAKU_CODE_T")
                mInfoTorimast.SYUBETU_T = SQLReader.GetString("SYUBETU_T")
                mInfoTorimast.TEKIYOU_KBN_T = SQLReader.GetString("TEKIYOU_KBN_T")
                mInfoTorimast.KTEKIYOU_T = SQLReader.GetString("KTEKIYOU_T")
                mInfoTorimast.NTEKIYOU_T = SQLReader.GetString("NTEKIYOU_T")
                mInfoTorimast.UMEISAI_KBN_T = SQLReader.GetString("UMEISAI_KBN_T")
                mInfoTorimast.MOTIKOMI_KIJITSU_T = SQLReader.GetString("MOTIKOMI_KIJITSU_T")
                mInfoTorimast.TESUUTYO_KIJITSU_T = SQLReader.GetString("TESUUTYO_KIJITSU_T")
                mInfoTorimast.KESSAI_KYU_CODE_T = SQLReader.GetString("KESSAI_KYU_CODE_T")
                mInfoTorimast.FUNOU_MEISAI_KBN_T = SQLReader.GetString("FUNOU_MEISAI_KBN_T")
                mInfoTorimast.TESUUTYO_DAY_T = SQLReader.GetInt("TESUUTYO_DAY_T")
                mInfoTorimast.TESUUTYO_KBN_T = SQLReader.GetString("TESUUTYO_KBN_T")
                mInfoTorimast.IRAISYO_KIJITSU_T = SQLReader.GetString("IRAISYO_KIJITSU_T")
                mInfoTorimast.IRAISYO_YDATE_T = SQLReader.GetString("IRAISYO_YDATE_T")
                mInfoTorimast.IRAISYO_KYU_CODE_T = SQLReader.GetString("IRAISYO_KYU_CODE_T")
                mInfoTorimast.SFURI_FLG_T = SQLReader.GetString("SFURI_FLG_T")
                mInfoTorimast.SFURI_DAY_T = SQLReader.GetString("SFURI_DAY_T")
                mInfoTorimast.SFURI_KIJITSU_T = SQLReader.GetString("SFURI_KIJITSU_T")
                mInfoTorimast.SFURI_KYU_CODE_T = SQLReader.GetString("SFURI_KYU_CODE_T")
                mInfoTorimast.SEIKYU_KBN_T = SQLReader.GetString("SEIKYU_KBN_T")
                mInfoTorimast.CYCLE_T = SQLReader.GetString("CYCLE_T")
                mInfoTorimast.KIJITU_KANRI_T = SQLReader.GetString("KIJITU_KANRI_T")
                mInfoTorimast.SYUMOKU_CODE_T = SQLReader.GetString("SYUMOKU_CODE_T")
                mInfoTorimast.FUKA_CODE_T = SQLReader.GetString("FUKA_CODE_T")
                mInfoTorimast.TEIGAKU_KBN_T = SQLReader.GetString("TEIGAKU_KBN_T")      '2010/02/08 ���ڒǉ�
                mInfoTorimast.KOKYAKU_NO_T = SQLReader.GetString("KOKYAKU_NO_T")        '2010/02/11 ���ڒǉ�

                mInfoTorimast.TUKI_T = New String(11) {}
                For i As Integer = 1 To 12
                    INFOToriMast.TUKI_T(i - 1) = SQLReader.GetString("TUKI" & i & "_T")
                Next i

                mInfoTorimast.ENC_KBN_T = SQLReader.GetString("ENC_KBN_T")
                mInfoTorimast.ENC_KEY1_T = SQLReader.GetString("ENC_KEY1_T")
                mInfoTorimast.ENC_KEY2_T = SQLReader.GetString("ENC_KEY2_T")
                mInfoTorimast.ENC_OPT1_T = SQLReader.GetString("ENC_OPT1_T")
                mInfoTorimast.FKEKKA_TBL_T = SQLReader.GetString("FKEKKA_TBL_T")
                mInfoTorimast.TESUU_A1_T = SQLReader.GetInt("TESUU_A1_T")
                mInfoTorimast.TESUU_A2_T = SQLReader.GetInt("TESUU_A2_T")
                mInfoTorimast.TESUU_A3_T = SQLReader.GetInt("TESUU_A3_T")
                mInfoTorimast.TESUU_B1_T = SQLReader.GetInt("TESUU_B1_T")
                mInfoTorimast.TESUU_B2_T = SQLReader.GetInt("TESUU_B2_T")
                mInfoTorimast.TESUU_B3_T = SQLReader.GetInt("TESUU_B3_T")
                mInfoTorimast.TESUU_C1_T = SQLReader.GetInt("TESUU_C1_T")
                mInfoTorimast.TESUU_C2_T = SQLReader.GetInt("TESUU_C2_T")
                mInfoTorimast.TESUU_C3_T = SQLReader.GetInt("TESUU_C3_T")
                mInfoTorimast.TESUU_TABLE_T = SQLReader.GetString("TESUU_TABLE_ID_T")

                '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- START
                mInfoTorimast.FURI_KYU_CODE_T = SQLReader.GetString("FURI_KYU_CODE_T")
                '2017/12/12 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- END
                '2018/01/16 saitou �L���M��(RSV2�W��) ADD �_����ԓ��擾 -------------------- START
                mInfoTorimast.LABEL_KBN_T = SQLReader.GetString("LABEL_KBN_T")
                mInfoTorimast.KEIYAKU_DATE_T = SQLReader.GetString("KEIYAKU_DATE_T")
                mInfoTorimast.KAISI_DATE_T = SQLReader.GetString("KAISI_DATE_T")
                mInfoTorimast.SYURYOU_DATE_T = SQLReader.GetString("SYURYOU_DATE_T")
                '2018/01/16 saitou �L���M��(RSV2�W��) ADD ----------------------------------- END
                '2017/12/04 �^�X�N�j���� ADD �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ START
                If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Call SelectTORIMAST_SUB(fsyorikbn, toris, torif)
                End If
                '2017/12/04 �^�X�N�j���� ADD �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ END
            Else
                mInfoTorimast = New stTORIMAST
            End If

            SQLReader.Close()
            SQLReader = Nothing
        Catch ex As Exception
            Message = ex.Message
        Finally
        End Try

        If CloseFlag = True Then
            MainDB.Close()
            MainDB = Nothing
        End If

        Return mInfoTorimast
    End Function

    '2017/12/04 �^�X�N�j���� ADD �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ START
    ''' <summary>
    ''' �����}�X�^���(�ŗL���) �擾
    ''' </summary>
    ''' <param name="fsyorikbn">�����敪</param>
    ''' <param name="toris">������R�[�h</param>
    ''' <param name="torif">����敛�R�[�h</param>
    ''' <remarks></remarks>
    Public Sub SelectTORIMAST_SUB(ByVal fsyorikbn As String, ByVal toris As String, ByVal torif As String)
        Dim SQL As New System.Text.StringBuilder(2048)
        Dim SQLReader As CASTCommon.MyOracleReader = Nothing

        Try
            '�����}�X�^(�ŗL���)�擾
            If fsyorikbn = "1" Then
                SQL.Append("SELECT * FROM TORIMAST_SUB")
            Else
                SQL.Append("SELECT * FROM S_TORIMAST_SUB")
            End If
            SQL.Append(" WHERE FSYORI_KBN_TSUB = '" & fsyorikbn & "'")
            SQL.Append(" AND TORIS_CODE_TSUB = '" & toris & "'")
            SQL.Append(" AND TORIF_CODE_TSUB = '" & torif & "'")

            SQLReader = New CASTCommon.MyOracleReader(MainDB)
            If SQLReader.DataReader(SQL) = True Then
                With mInfoTorimast
                    .AITE_CNT_CODE_T = SQLReader.GetString("AITE_CNT_CODE_T")
                    .TOHO_CNT_CODE_T = SQLReader.GetString("TOHO_CNT_CODE_T")
                    .DENSO_FILE_ID_T = SQLReader.GetString("DENSO_FILE_ID_T")
                    .HANSOU_KBN_T = SQLReader.GetString("HANSOU_KBN_T")
                    .HANSOU_ROOT1_T = SQLReader.GetString("HANSOU_ROOT1_T")
                    .HANSOU_ROOT2_T = SQLReader.GetString("HANSOU_ROOT2_T")
                    .HANSOU_ROOT3_T = SQLReader.GetString("HANSOU_ROOT3_T")
                    .HENKYAKU_SIT_NO_T = SQLReader.GetString("HENKYAKU_SIT_NO_T")
                    .SYOUGOU_KBN_T = SQLReader.GetString("SYOUGOU_KBN_T")
                    .KEIYAKU_NO_T = SQLReader.GetString("KEIYAKU_NO_T")
                    .MAE_SYORI_T = SQLReader.GetString("MAE_SYORI_T")
                    .ATO_SYORI_T = SQLReader.GetString("ATO_SYORI_T")
                    .TOKKIJIKOU1_T = SQLReader.GetString("TOKKIJIKOU1_T")
                    .TOKKIJIKOU2_T = SQLReader.GetString("TOKKIJIKOU2_T")
                    .TOKKIJIKOU3_T = SQLReader.GetString("TOKKIJIKOU3_T")
                    .TOKKIJIKOU4_T = SQLReader.GetString("TOKKIJIKOU4_T")
                    If fsyorikbn = "3" Then
                        .KYUUYO_KBN_T = SQLReader.GetString("KYUUYO_KBN_T")
                    End If
                    .CUSTOM_NUM01_T = SQLReader.GetInt64("CUSTOM_NUM01_T")
                    .CUSTOM_NUM02_T = SQLReader.GetInt64("CUSTOM_NUM02_T")
                    .CUSTOM_NUM03_T = SQLReader.GetInt64("CUSTOM_NUM03_T")
                    .CUSTOM_NUM04_T = SQLReader.GetInt64("CUSTOM_NUM04_T")
                    .CUSTOM_NUM05_T = SQLReader.GetInt64("CUSTOM_NUM05_T")
                    .CUSTOM_NUM06_T = SQLReader.GetInt64("CUSTOM_NUM06_T")
                    .CUSTOM_NUM07_T = SQLReader.GetInt64("CUSTOM_NUM07_T")
                    .CUSTOM_NUM08_T = SQLReader.GetInt64("CUSTOM_NUM08_T")
                    .CUSTOM_NUM09_T = SQLReader.GetInt64("CUSTOM_NUM09_T")
                    .CUSTOM_NUM10_T = SQLReader.GetInt64("CUSTOM_NUM10_T")
                    .CUSTOM_NUM11_T = SQLReader.GetInt64("CUSTOM_NUM11_T")
                    .CUSTOM_NUM12_T = SQLReader.GetInt64("CUSTOM_NUM12_T")
                    .CUSTOM_NUM13_T = SQLReader.GetInt64("CUSTOM_NUM13_T")
                    .CUSTOM_NUM14_T = SQLReader.GetInt64("CUSTOM_NUM14_T")
                    .CUSTOM_NUM15_T = SQLReader.GetInt64("CUSTOM_NUM15_T")
                    .CUSTOM_NUM16_T = SQLReader.GetInt64("CUSTOM_NUM16_T")
                    .CUSTOM_NUM17_T = SQLReader.GetInt64("CUSTOM_NUM17_T")
                    .CUSTOM_NUM18_T = SQLReader.GetInt64("CUSTOM_NUM18_T")
                    .CUSTOM_NUM19_T = SQLReader.GetInt64("CUSTOM_NUM19_T")
                    .CUSTOM_NUM20_T = SQLReader.GetInt64("CUSTOM_NUM20_T")
                    .CUSTOM_NUM21_T = SQLReader.GetInt64("CUSTOM_NUM21_T")
                    .CUSTOM_NUM22_T = SQLReader.GetInt64("CUSTOM_NUM22_T")
                    .CUSTOM_NUM23_T = SQLReader.GetInt64("CUSTOM_NUM23_T")
                    .CUSTOM_NUM24_T = SQLReader.GetInt64("CUSTOM_NUM24_T")
                    .CUSTOM_NUM25_T = SQLReader.GetInt64("CUSTOM_NUM25_T")
                    .CUSTOM_NUM26_T = SQLReader.GetInt64("CUSTOM_NUM26_T")
                    .CUSTOM_NUM27_T = SQLReader.GetInt64("CUSTOM_NUM27_T")
                    .CUSTOM_NUM28_T = SQLReader.GetInt64("CUSTOM_NUM28_T")
                    .CUSTOM_NUM29_T = SQLReader.GetInt64("CUSTOM_NUM29_T")
                    .CUSTOM_NUM30_T = SQLReader.GetInt64("CUSTOM_NUM30_T")
                    .CUSTOM_NUM31_T = SQLReader.GetInt64("CUSTOM_NUM31_T")
                    .CUSTOM_NUM32_T = SQLReader.GetInt64("CUSTOM_NUM32_T")
                    .CUSTOM_NUM33_T = SQLReader.GetInt64("CUSTOM_NUM33_T")
                    .CUSTOM_NUM34_T = SQLReader.GetInt64("CUSTOM_NUM34_T")
                    .CUSTOM_NUM35_T = SQLReader.GetInt64("CUSTOM_NUM35_T")
                    .CUSTOM_NUM36_T = SQLReader.GetInt64("CUSTOM_NUM36_T")
                    .CUSTOM_NUM37_T = SQLReader.GetInt64("CUSTOM_NUM37_T")
                    .CUSTOM_NUM38_T = SQLReader.GetInt64("CUSTOM_NUM38_T")
                    .CUSTOM_NUM39_T = SQLReader.GetInt64("CUSTOM_NUM39_T")
                    .CUSTOM_NUM40_T = SQLReader.GetInt64("CUSTOM_NUM40_T")
                    .CUSTOM_NUM41_T = SQLReader.GetInt64("CUSTOM_NUM41_T")
                    .CUSTOM_NUM42_T = SQLReader.GetInt64("CUSTOM_NUM42_T")
                    .CUSTOM_NUM43_T = SQLReader.GetInt64("CUSTOM_NUM43_T")
                    .CUSTOM_NUM44_T = SQLReader.GetInt64("CUSTOM_NUM44_T")
                    .CUSTOM_NUM45_T = SQLReader.GetInt64("CUSTOM_NUM45_T")
                    .CUSTOM_NUM46_T = SQLReader.GetInt64("CUSTOM_NUM46_T")
                    .CUSTOM_NUM47_T = SQLReader.GetInt64("CUSTOM_NUM47_T")
                    .CUSTOM_NUM48_T = SQLReader.GetInt64("CUSTOM_NUM48_T")
                    .CUSTOM_NUM49_T = SQLReader.GetInt64("CUSTOM_NUM49_T")
                    .CUSTOM_NUM50_T = SQLReader.GetInt64("CUSTOM_NUM50_T")
                    .CUSTOM_VCR01_T = SQLReader.GetString("CUSTOM_VCR01_T")
                    .CUSTOM_VCR02_T = SQLReader.GetString("CUSTOM_VCR02_T")
                    .CUSTOM_VCR03_T = SQLReader.GetString("CUSTOM_VCR03_T")
                    .CUSTOM_VCR04_T = SQLReader.GetString("CUSTOM_VCR04_T")
                    .CUSTOM_VCR05_T = SQLReader.GetString("CUSTOM_VCR05_T")
                    .CUSTOM_VCR06_T = SQLReader.GetString("CUSTOM_VCR06_T")
                    .CUSTOM_VCR07_T = SQLReader.GetString("CUSTOM_VCR07_T")
                    .CUSTOM_VCR08_T = SQLReader.GetString("CUSTOM_VCR08_T")
                    .CUSTOM_VCR09_T = SQLReader.GetString("CUSTOM_VCR09_T")
                    .CUSTOM_VCR10_T = SQLReader.GetString("CUSTOM_VCR10_T")
                    .CUSTOM_VCR11_T = SQLReader.GetString("CUSTOM_VCR11_T")
                    .CUSTOM_VCR12_T = SQLReader.GetString("CUSTOM_VCR12_T")
                    .CUSTOM_VCR13_T = SQLReader.GetString("CUSTOM_VCR13_T")
                    .CUSTOM_VCR14_T = SQLReader.GetString("CUSTOM_VCR14_T")
                    .CUSTOM_VCR15_T = SQLReader.GetString("CUSTOM_VCR15_T")
                    .CUSTOM_VCR16_T = SQLReader.GetString("CUSTOM_VCR16_T")
                    .CUSTOM_VCR17_T = SQLReader.GetString("CUSTOM_VCR17_T")
                    .CUSTOM_VCR18_T = SQLReader.GetString("CUSTOM_VCR18_T")
                    .CUSTOM_VCR19_T = SQLReader.GetString("CUSTOM_VCR19_T")
                    .CUSTOM_VCR20_T = SQLReader.GetString("CUSTOM_VCR20_T")
                    .CUSTOM_VCR21_T = SQLReader.GetString("CUSTOM_VCR21_T")
                    .CUSTOM_VCR22_T = SQLReader.GetString("CUSTOM_VCR22_T")
                    .CUSTOM_VCR23_T = SQLReader.GetString("CUSTOM_VCR23_T")
                    .CUSTOM_VCR24_T = SQLReader.GetString("CUSTOM_VCR24_T")
                    .CUSTOM_VCR25_T = SQLReader.GetString("CUSTOM_VCR25_T")
                    .CUSTOM_VCR26_T = SQLReader.GetString("CUSTOM_VCR26_T")
                    .CUSTOM_VCR27_T = SQLReader.GetString("CUSTOM_VCR27_T")
                    .CUSTOM_VCR28_T = SQLReader.GetString("CUSTOM_VCR28_T")
                    .CUSTOM_VCR29_T = SQLReader.GetString("CUSTOM_VCR29_T")
                    .CUSTOM_VCR30_T = SQLReader.GetString("CUSTOM_VCR30_T")
                    .CUSTOM_VCR31_T = SQLReader.GetString("CUSTOM_VCR31_T")
                    .CUSTOM_VCR32_T = SQLReader.GetString("CUSTOM_VCR32_T")
                    .CUSTOM_VCR33_T = SQLReader.GetString("CUSTOM_VCR33_T")
                    .CUSTOM_VCR34_T = SQLReader.GetString("CUSTOM_VCR34_T")
                    .CUSTOM_VCR35_T = SQLReader.GetString("CUSTOM_VCR35_T")
                    .CUSTOM_VCR36_T = SQLReader.GetString("CUSTOM_VCR36_T")
                    .CUSTOM_VCR37_T = SQLReader.GetString("CUSTOM_VCR37_T")
                    .CUSTOM_VCR38_T = SQLReader.GetString("CUSTOM_VCR38_T")
                    .CUSTOM_VCR39_T = SQLReader.GetString("CUSTOM_VCR39_T")
                    .CUSTOM_VCR40_T = SQLReader.GetString("CUSTOM_VCR40_T")
                    .CUSTOM_VCR41_T = SQLReader.GetString("CUSTOM_VCR41_T")
                    .CUSTOM_VCR42_T = SQLReader.GetString("CUSTOM_VCR42_T")
                    .CUSTOM_VCR43_T = SQLReader.GetString("CUSTOM_VCR43_T")
                    .CUSTOM_VCR44_T = SQLReader.GetString("CUSTOM_VCR44_T")
                    .CUSTOM_VCR45_T = SQLReader.GetString("CUSTOM_VCR45_T")
                    .CUSTOM_VCR46_T = SQLReader.GetString("CUSTOM_VCR46_T")
                    .CUSTOM_VCR47_T = SQLReader.GetString("CUSTOM_VCR47_T")
                    .CUSTOM_VCR48_T = SQLReader.GetString("CUSTOM_VCR48_T")
                    .CUSTOM_VCR49_T = SQLReader.GetString("CUSTOM_VCR49_T")
                    .CUSTOM_VCR50_T = SQLReader.GetString("CUSTOM_VCR50_T")
                End With
            End If
            Return
        Catch ex As Exception
            Message = ex.Message
            Return
        Finally
            If Not SQLReader Is Nothing Then
                SQLReader.Close()
                SQLReader = Nothing
            End If
        End Try

    End Sub
    '2017/12/04 �^�X�N�j���� ADD �W���ŏC���i�ƍ��@�\�ǉ��j------------------------------------ END

    ' �����}�X�^�����N���A����
    Public Sub ClearTORIMAST()
        mInfoTorimast = New stTORIMAST
    End Sub
End Class
