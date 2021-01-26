Imports CAstFormKes.ClsFormKes

' ���G���^�f�[�^�t�H�[�}�b�g
Public Class ClsFormSikinFuri

    ' �Z�p���[�^
    Protected Shared SEPARATE As String = Microsoft.VisualBasic.ChrW(&H1E)

    ' �Œ蒷�\���̗p�C���^�[�t�F�[�X
    Protected Interface IFormat
        ' �f�[�^
        Sub Init()
        Property Data() As String
        Property DataSepaPlus() As String
    End Interface

#Region "04-099 �ʒi�x��"

    '---------------------------------------------------------
    '2018/01/11 FJH����
    '�N�Z���F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 352��351
    '---------------------------------------------------------

    Public Structure T_04099
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim KINGAKU As String           ' ���z
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim KIGYO_CODE As String        ' ��ƃR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim TORIATUKAI1 As String       ' �戵�ԍ��P
        Dim KENSU As String             ' ����
        Dim TEGATA_NO As String         ' ��`���؎�ԍ�
        Dim GENTEN_NO As String         ' ���X�ԍ�
        Dim KISANBI As String           ' �N�Z��
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "04"
            OPE_CODE = "099"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7), _
                    SubData(KINGAKU, 13), _
                    SubData(FURI_CODE, 3), _
                    SubData(KIGYO_CODE, 5), _
                    SubData(TEKIYOU, 16), _
                    SubData(TORIATUKAI1, 5), _
                    SubData(KENSU, 4), _
                    SubData(TEGATA_NO, 6), _
                    SubData(GENTEN_NO, 3), _
                    SubData(KISANBI, 7), _
                    SubData(YOBI1, 351) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                FURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                TEKIYOU = CuttingData(Value, 16)
                TORIATUKAI1 = CuttingData(Value, 5)
                KENSU = CuttingData(Value, 4)
                TEGATA_NO = CuttingData(Value, 6)
                GENTEN_NO = CuttingData(Value, 3)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'YOBI1 = CuttingData(Value, 352)
                YOBI1 = CuttingData(Value, 351)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 13).Trim, _
                    SEPARATE, _
                    SubData(FURI_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(KIGYO_CODE, 5).Trim, _
                    SEPARATE, _
                    SubData(TEKIYOU, 16).Trim, _
                    SEPARATE, _
                    SubData(TORIATUKAI1, 5).Trim, _
                    SEPARATE, _
                    SubData(KENSU, 4).Trim, _
                    SEPARATE, _
                    SubData(TEGATA_NO, 6).Trim, _
                    SEPARATE, _
                    SubData(GENTEN_NO, 3).Trim, _
                    SEPARATE, _
                    SubData(KISANBI, 7).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                FURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                TEKIYOU = CuttingData(Value, 16)
                TORIATUKAI1 = CuttingData(Value, 5)
                KENSU = CuttingData(Value, 4)
                TEGATA_NO = CuttingData(Value, 6)
                GENTEN_NO = CuttingData(Value, 3)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
            End Set
        End Property


    End Structure
#End Region

#Region "01-010 ��������"

    '---------------------------------------------------------
    '2008/4/30 
    '�E�v�C��
    '�����O�X  16 ��  13 �ɏC��(FJH)
    '
    '�\��1
    '�����O�X 168 �� 171 �ɏC��(FJH)
    '
    '2018/01/11 FJH����
    '�N�Z���A����t�\����F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 212��210
    '---------------------------------------------------------

    Public Structure T_01010
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h 
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim KINGAKU As String           ' ���z
        Dim SIKINKA_KBN As String       ' �������敪�R�[�h
        Dim TATEN_TEKIYOU As String     ' ���X���E�v
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim TESUU_KBN As String         ' �萔�������敪
        Dim TESUU_KINGAKU As String     ' �萔���z
        Dim KISANBI As String           ' �N�Z��
        Dim SAKIHIDUKE_YOTEI As String  ' ����t�\���
        Dim FURIKOMI_IRAI As String     ' �U���˗��l��
        Dim GENTEN_NO As String         ' ���X�ԍ�
        Dim KINGAKU1 As String          ' ���z�P
        Dim SIKINKA_KBN1 As String      ' �������敪�R�[�h�P
        Dim TATEN_TEKIYOU1 As String    ' ���X���E�v�P
        Dim KINGAKU2 As String          ' ���z�Q
        Dim SIKINKA_KBN2 As String      ' �������敪�R�[�h�Q
        Dim TATEN_TEKIYO2 As String     ' ���X���E�v�Q
        Dim KINGAKU3 As String          ' ���z�R
        Dim SIKINKA_KBN3 As String      ' �������敪�R�[�h�R
        Dim TATEN_TEKIYO3 As String     ' ���X���E�v�R
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "01"
            OPE_CODE = "010"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7), _
                    SubData(KINGAKU, 13), _
                    SubData(SIKINKA_KBN, 1), _
                    SubData(TATEN_TEKIYOU, 2), _
                    SubData(FURI_CODE, 3), _
                    SubData(TEKIYOU, 13), _
                    SubData(TESUU_KBN, 1), _
                    SubData(TESUU_KINGAKU, 5), _
                    SubData(KISANBI, 7), _
                    SubData(SAKIHIDUKE_YOTEI, 7), _
                    SubData(FURIKOMI_IRAI, 48), _
                    SubData(GENTEN_NO, 3), _
                    SubData(YOBI1, 210) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                SIKINKA_KBN = CuttingData(Value, 1)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                FURI_CODE = CuttingData(Value, 3)
                TEKIYOU = CuttingData(Value, 13)
                TESUU_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 5)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)
                FURIKOMI_IRAI = CuttingData(Value, 48)
                GENTEN_NO = CuttingData(Value, 3)
                'KINGAKU1 = CuttingData(Value, 10)
                'SIKINKA_KBN1 = CuttingData(Value, 1)
                'TATEN_TEKIYOU1 = CuttingData(Value, 2)
                'KINGAKU2 = CuttingData(Value, 10)
                'SIKINKA_KBN2 = CuttingData(Value, 1)
                'TATEN_TEKIYO2 = CuttingData(Value, 2)
                'KINGAKU3 = CuttingData(Value, 10)
                'SIKINKA_KBN3 = CuttingData(Value, 1)
                'TATEN_TEKIYO3 = CuttingData(Value, 2)
                'YOBI1 = CuttingData(Value, 212)
                YOBI1 = CuttingData(Value, 210)
            End Set
        End Property


        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 13).Trim, _
                    SEPARATE, _
                    SubData(SIKINKA_KBN, 1).Trim, _
                    SubData(TATEN_TEKIYOU, 2).Trim, _
                    SEPARATE, _
                    SubData(FURI_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(TEKIYOU, 13).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KBN, 1).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KINGAKU, 5).Trim, _
                    SEPARATE, _
                    SubData(KISANBI, 7).Trim, _
                    SEPARATE, _
                    SubData(SAKIHIDUKE_YOTEI, 7).Trim, _
                    SEPARATE, _
                    SubData(FURIKOMI_IRAI, 48).Trim, _
                    SEPARATE, _
                    SubData(GENTEN_NO, 3).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                SIKINKA_KBN = CuttingData(Value, 1)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                FURI_CODE = CuttingData(Value, 3)
                TEKIYOU = CuttingData(Value, 13)
                TESUU_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 5)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)
                FURIKOMI_IRAI = CuttingData(Value, 48)
                GENTEN_NO = CuttingData(Value, 3)
                'KINGAKU1 = CuttingData(Value, 10)
                'SIKINKA_KBN1 = CuttingData(Value, 1)
                'TATEN_TEKIYOU1 = CuttingData(Value, 2)
                'KINGAKU2 = CuttingData(Value, 10)
                'SIKINKA_KBN2 = CuttingData(Value, 1)
                'TATEN_TEKIYO2 = CuttingData(Value, 2)
                'KINGAKU3 = CuttingData(Value, 10)
                'SIKINKA_KBN3 = CuttingData(Value, 1)
                'TATEN_TEKIYO3 = CuttingData(Value, 2)
            End Set
        End Property

    End Structure
#End Region

#Region "02-019 ���ʓ����i�m�a�j"

    '---------------------------------------------------------
    '2008/4/30 
    '�E�v�C��
    '�����O�X  16 ��  13 �ɏC��(FJH)
    '
    '�\��1
    '�����O�X 166 �� 169 �ɏC��(FJH)
    '
    '2018/01/11 FJH����
    '�N�Z���A����t�\����F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 162��160
    '---------------------------------------------------------

    Public Structure T_02019
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim GYO As String               ' �s
        Dim KINGAKU As String           ' ���z
        Dim SIKINKA_KBN As String       ' �������敪�R�[�h
        Dim TATEN_TEKIYOU As String     ' ���X���E�v
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim TESUU_KBN As String         ' �萔�������敪
        Dim TESUU_KINGAKU As String     ' �萔���z
        Dim KISANBI As String           ' �N�Z��
        Dim SAKIHIDUKE_YOTEI As String  ' ����t�\���
        Dim FURIKOMI_IRAI As String     ' �U���˗��l��
        Dim GENTEN_NO As String         ' ���X�ԍ�      
        Dim KINGAKU1 As String          ' ���z�P
        Dim SIKINKA_KBN1 As String      ' �������敪�R�[�h�P
        Dim TATEN_TEKIYOU1 As String    ' ���X���E�v�P
        Dim KINGAKU2 As String          ' ���z�Q   
        Dim SIKINKA_KBN2 As String      ' �������敪�R�[�h�Q
        Dim TATEN_TEKIYO2 As String     ' ���X���E�v�Q
        Dim KINGAKU3 As String          ' ���z�R
        Dim SIKINKA_KBN3 As String      ' �������敪�R�[�h�R
        Dim TATEN_TEKIYO3 As String     ' ���X���E�v�R
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "02"
            OPE_CODE = "019"
            GYO = "01"
            FURI_CODE = "040"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7), _
                    SubData(GYO, 2), _
                    SubData(KINGAKU, 13), _
                    SubData(SIKINKA_KBN, 1), _
                    SubData(TATEN_TEKIYOU, 2), _
                    SubData(FURI_CODE, 3), _
                    SubData(TEKIYOU, 13), _
                    SubData(TESUU_KBN, 1), _
                    SubData(TESUU_KINGAKU, 5), _
                    SubData(KISANBI, 7), _
                    SubData(SAKIHIDUKE_YOTEI, 7), _
                    SubData(FURIKOMI_IRAI, 48), _
                    SubData(GENTEN_NO, 3), _
                    SubData(YOBI1, 160) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                GYO = CuttingData(Value, 2)
                KINGAKU = CuttingData(Value, 13)
                SIKINKA_KBN = CuttingData(Value, 1)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                FURI_CODE = CuttingData(Value, 3)
                TEKIYOU = CuttingData(Value, 13)
                TESUU_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 5)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)
                FURIKOMI_IRAI = CuttingData(Value, 48)
                GENTEN_NO = CuttingData(Value, 3)
                'KINGAKU1 = CuttingData(Value, 13)
                'SIKINKA_KBN1 = CuttingData(Value, 1)
                'TATEN_TEKIYOU1 = CuttingData(Value, 2)
                'KINGAKU2 = CuttingData(Value, 13)
                'SIKINKA_KBN2 = CuttingData(Value, 1)
                'TATEN_TEKIYO2 = CuttingData(Value, 2)
                'KINGAKU3 = CuttingData(Value, 13)
                'SIKINKA_KBN3 = CuttingData(Value, 1)
                'TATEN_TEKIYO3 = CuttingData(Value, 2)
                'YOBI1 = CuttingData(Value, 162)
                YOBI1 = CuttingData(Value, 160)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7).Trim, _
                    SEPARATE, _
                    SubData(GYO, 2).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 13).Trim, _
                    SEPARATE, _
                    SubData(SIKINKA_KBN, 1).Trim, _
                    SubData(TATEN_TEKIYOU, 2).Trim, _
                    SEPARATE, _
                    SubData(FURI_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(TEKIYOU, 13).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KBN, 1).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KINGAKU, 5).Trim, _
                    SEPARATE, _
                    SubData(KISANBI, 7).Trim, _
                    SEPARATE, _
                    SubData(SAKIHIDUKE_YOTEI, 7).Trim, _
                    SEPARATE, _
                    SubData(FURIKOMI_IRAI, 48).Trim, _
                    SEPARATE, _
                    SubData(GENTEN_NO, 3).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                GYO = CuttingData(Value, 2)
                KINGAKU = CuttingData(Value, 13)
                SIKINKA_KBN = CuttingData(Value, 1)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                FURI_CODE = CuttingData(Value, 3)
                TEKIYOU = CuttingData(Value, 13)
                TESUU_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 5)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)
                FURIKOMI_IRAI = CuttingData(Value, 48)
                GENTEN_NO = CuttingData(Value, 3)
                'KINGAKU1 = CuttingData(Value, 13)
                'SIKINKA_KBN1 = CuttingData(Value, 1)
                'TATEN_TEKIYOU1 = CuttingData(Value, 2)
                'KINGAKU2 = CuttingData(Value, 13)
                'SIKINKA_KBN2 = CuttingData(Value, 1)
                'TATEN_TEKIYO2 = CuttingData(Value, 2)
                'KINGAKU3 = CuttingData(Value, 13)
                'SIKINKA_KBN3 = CuttingData(Value, 1)
                'TATEN_TEKIYO3 = CuttingData(Value, 2)
            End Set
        End Property


    End Structure
#End Region

#Region "04-019 �ʒi����"

    '---------------------------------------------------------
    '2018/01/11 FJH����
    '�N�Z���A����t�\����F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 226��224
    '---------------------------------------------------------

    Public Structure T_04019
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim KINGAKU As String           ' ���z
        Dim SIKINKA_KBN As String       ' �������敪�R�[�h
        Dim TATEN_TEKIYOU As String     ' ���X���E�v
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim KIGYO_CODE As String        ' ��ƃR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim TORIATUKAI1 As String       ' �戵�ԍ��Q
        Dim KENSU As String             ' ����
        Dim MADOGUTI_KBN As String      ' �������[�敪
        Dim INSI_KEN As String          ' �󎆌���
        Dim TEGATA_NO As String         ' ��`���؎�ԍ�
        Dim HAKKOU_NO As String         ' ���s��ڋq�ԍ�
        Dim TESUU_KBN As String         ' �萔�������敪
        Dim TESUU_KINGAKU As String     ' �萔���z
        Dim KISANBI As String           ' �N�Z��
        Dim SAKIHIDUKE_YOTEI As String  ' ����t�\���
        Dim GENTEN_NO As String         ' ���X�ԍ�
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "04"
            OPE_CODE = "019"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7), _
                    SubData(KINGAKU, 13), _
                    SubData(SIKINKA_KBN, 1), _
                    SubData(TATEN_TEKIYOU, 2), _
                    SubData(FURI_CODE, 3), _
                    SubData(KIGYO_CODE, 5), _
                    SubData(TEKIYOU, 16), _
                    SubData(TORIATUKAI1, 5), _
                    SubData(KENSU, 4), _
                    SubData(MADOGUTI_KBN, 1), _
                    SubData(INSI_KEN, 3), _
                    SubData(TEGATA_NO, 6), _
                    SubData(HAKKOU_NO, 7), _
                    SubData(TESUU_KBN, 1), _
                    SubData(TESUU_KINGAKU, 5), _
                    SubData(KISANBI, 7), _
                    SubData(SAKIHIDUKE_YOTEI, 7), _
                    SubData(GENTEN_NO, 3), _
                    SubData(YOBI1, 224) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                SIKINKA_KBN = CuttingData(Value, 1)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                FURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                TEKIYOU = CuttingData(Value, 16)
                TORIATUKAI1 = CuttingData(Value, 5)
                KENSU = CuttingData(Value, 4)
                MADOGUTI_KBN = CuttingData(Value, 1)
                INSI_KEN = CuttingData(Value, 3)
                TEGATA_NO = CuttingData(Value, 6)
                HAKKOU_NO = CuttingData(Value, 7)
                TESUU_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 5)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)
                GENTEN_NO = CuttingData(Value, 3)
                'YOBI1 = CuttingData(Value, 226)
                YOBI1 = CuttingData(Value, 224)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 13).Trim, _
                    SEPARATE, _
                    SubData(SIKINKA_KBN, 1).Trim, _
                    SubData(TATEN_TEKIYOU, 2).Trim, _
                    SEPARATE, _
                    SubData(FURI_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(KIGYO_CODE, 5).Trim, _
                    SEPARATE, _
                    SubData(TEKIYOU, 16).Trim, _
                    SEPARATE, _
                    SubData(TORIATUKAI1, 5).Trim, _
                    SEPARATE, _
                    SubData(KENSU, 4).Trim, _
                    SEPARATE, _
                    SubData(MADOGUTI_KBN, 1).Trim, _
                    SEPARATE, _
                    SubData(INSI_KEN, 3).Trim, _
                    SEPARATE, _
                    SubData(TEGATA_NO, 6).Trim, _
                    SEPARATE, _
                    SubData(HAKKOU_NO, 7).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KBN, 1).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KINGAKU, 5).Trim, _
                    SEPARATE, _
                    SubData(KISANBI, 7).Trim, _
                    SEPARATE, _
                    SubData(SAKIHIDUKE_YOTEI, 7).Trim, _
                    SEPARATE, _
                    SubData(GENTEN_NO, 3).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                SIKINKA_KBN = CuttingData(Value, 1)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                FURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                TEKIYOU = CuttingData(Value, 16)
                TORIATUKAI1 = CuttingData(Value, 5)
                KENSU = CuttingData(Value, 4)
                MADOGUTI_KBN = CuttingData(Value, 1)
                INSI_KEN = CuttingData(Value, 3)
                TEGATA_NO = CuttingData(Value, 6)
                HAKKOU_NO = CuttingData(Value, 7)
                TESUU_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 5)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)
                GENTEN_NO = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
#End Region

#Region "99-011 ���������(NB)"

    '---------------------------------------------------------
    '2018/01/11 FJH����
    '�N�Z���F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 249��248
    '
    '2018/05/10 �^�X�N�֓�
    '�\���P�F�����O�X�ύX 248��332
    '---------------------------------------------------------

    Public Structure T_99011
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim UTIWAKE_CODE As String      ' ����R�[�h
        Dim KINGAKU As String           ' ���z
        Dim TATEN_TEKIYOU As String     ' ���X���E�v
        Dim KENSU As String             ' ����
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim TORIATUKAI1 As String       ' �戵�ԍ��P
        Dim JINKAKU_CODE As String      ' �l�i�R�[�h   
        Dim KAZEI_CODE As String        ' �ېŃR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim KISANBI As String           ' �N�Z��
        Dim GENTEN_NO As String         ' ���X�ԍ�
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "99"
            OPE_CODE = "011"
            KENSU = "1"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    {
                    SubData(KOUZA_NO, 7),
                    SubData(UTIWAKE_CODE, 2),
                    SubData(KINGAKU, 13),
                    SubData(TATEN_TEKIYOU, 2),
                    SubData(KENSU, 5),
                    SubData(FURI_CODE, 3),
                    SubData(TORIATUKAI1, 3),
                    SubData(JINKAKU_CODE, 2),
                    SubData(KAZEI_CODE, 1),
                    SubData(TEKIYOU, 20),
                    SubData(KISANBI, 7),
                    SubData(GENTEN_NO, 3),
                    SubData(YOBI1, 332)
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                UTIWAKE_CODE = CuttingData(Value, 2)
                KINGAKU = CuttingData(Value, 13)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                GENTEN_NO = CuttingData(Value, 3)
                'YOBI1 = CuttingData(Value, 249)
                'YOBI1 = CuttingData(Value, 248)
                YOBI1 = CuttingData(Value, 332)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    {
                    SubData(KOUZA_NO, 7).Trim,
                    SEPARATE,
                    SubData(UTIWAKE_CODE, 2).Trim,
                    SEPARATE,
                    SubData(KINGAKU, 13).Trim,
                    SEPARATE,
                    SubData(TATEN_TEKIYOU, 2).Trim,
                    SEPARATE,
                    SubData(KENSU, 5).Trim,
                    SEPARATE,
                    SubData(FURI_CODE, 3).Trim,
                    SEPARATE,
                    SubData(TORIATUKAI1, 3).Trim,
                    SEPARATE,
                    SubData(JINKAKU_CODE, 2).Trim,
                    SubData(KAZEI_CODE, 1).Trim,
                    SEPARATE,
                    SubData(TEKIYOU, 20).Trim,
                    SEPARATE,
                    SubData(KISANBI, 7).Trim,
                    SEPARATE,
                    SubData(GENTEN_NO, 3).Trim,
                    SEPARATE
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                UTIWAKE_CODE = CuttingData(Value, 2)
                KINGAKU = CuttingData(Value, 13)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                GENTEN_NO = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
#End Region

#Region "99-019 ���������"

    '---------------------------------------------------------
    '2018/01/11 FJH����
    '�N�Z���F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 235��234
    '---------------------------------------------------------

    Public Structure T_99019
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim GYO As String               ' �s
        Dim UTIWAKE_CODE As String      ' ����R�[�h
        Dim ZENZAN As String            ' �O�c
        Dim FUGOU_CODE As String        ' �����R�[�h
        Dim KINGAKU As String           ' ���z
        Dim TATEN_TEKIYOU As String     ' ���X���E�v
        Dim KENSU As String             ' ����
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim TORIATUKAI1 As String       ' �戵�ԍ��P
        Dim JINKAKU_CODE As String      ' �l�i�R�[�h   
        Dim KAZEI_CODE As String        ' �ېŃR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim KISANBI As String           ' �N�Z��
        Dim GENTEN_NO As String         ' ���X�ԍ�
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "99"
            OPE_CODE = "019"
            GYO = "01"
            ZENZAN = "0".PadRight(15, " "c)
            FUGOU_CODE = "1"
            KENSU = "1".PadLeft(5, " "c)
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7), _
                    SubData(GYO, 2), _
                    SubData(UTIWAKE_CODE, 2), _
                    SubData(ZENZAN, 15), _
                    SubData(FUGOU_CODE, 1), _
                    SubData(KINGAKU, 13), _
                    SubData(TATEN_TEKIYOU, 2), _
                    SubData(KENSU, 5), _
                    SubData(FURI_CODE, 3), _
                    SubData(TORIATUKAI1, 3), _
                    SubData(JINKAKU_CODE, 2), _
                    SubData(KAZEI_CODE, 1), _
                    SubData(TEKIYOU, 20), _
                    SubData(KISANBI, 7), _
                    SubData(GENTEN_NO, 3), _
                    SubData(YOBI1, 234) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                GYO = CuttingData(Value, 2)
                UTIWAKE_CODE = CuttingData(Value, 2)
                ZENZAN = CuttingData(Value, 15)
                FUGOU_CODE = CuttingData(Value, 1)
                KINGAKU = CuttingData(Value, 13)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                GENTEN_NO = CuttingData(Value, 3)
                'YOBI1 = CuttingData(Value, 235)
                YOBI1 = CuttingData(Value, 234)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7).Trim, _
                    SEPARATE, _
                    SubData(GYO, 2).Trim, _
                    SEPARATE, _
                    SubData(UTIWAKE_CODE, 2).Trim, _
                    SEPARATE, _
                    SubData(ZENZAN, 15).Trim, _
                    SEPARATE, _
                    SubData(FUGOU_CODE, 1).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 13).Trim, _
                    SEPARATE, _
                    SubData(TATEN_TEKIYOU, 2).Trim, _
                    SEPARATE, _
                    SubData(KENSU, 5).Trim, _
                    SEPARATE, _
                    SubData(FURI_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(TORIATUKAI1, 3).Trim, _
                    SEPARATE, _
                    SubData(JINKAKU_CODE, 2).Trim, _
                    SubData(KAZEI_CODE, 1).Trim, _
                    SEPARATE, _
                    SubData(TEKIYOU, 20).Trim, _
                    SEPARATE, _
                    SubData(KISANBI, 7).Trim, _
                    SEPARATE, _
                    SubData(GENTEN_NO, 3).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                GYO = CuttingData(Value, 2)
                UTIWAKE_CODE = CuttingData(Value, 2)
                ZENZAN = CuttingData(Value, 15)
                FUGOU_CODE = CuttingData(Value, 1)
                KINGAKU = CuttingData(Value, 13)
                TATEN_TEKIYOU = CuttingData(Value, 2)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                GENTEN_NO = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
#End Region

#Region "48-100 �ב֐U��"

    '---------------------------------------------------------
    '2008/4/30 
    '���M�X��
    '�����O�X 20 �� 15 �ɏC��(FJH)
    '
    '���l�����ԍ�
    '�����O�X 14 �� 15 �ɏC��(FJH)
    '
    '���z
    '�����O�X 15 �� 10 �ɏC��(FJH)
    '
    '�\��1
    '�����O�X 45 �� 54 �ɏC��(FJH)
    '---------------------------------------------------------

    Public Structure T_48100
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim TORIATUKAI As String        ' �戵��
        Dim SYUMOKU As String           ' ��ڃR�[�h
        Dim JUSIN_TEN As String         ' ��M�X��
        Dim FUKA_CODE As String         ' �t���R�[�h
        Dim HASSIN_TEN As String        ' ���M�X��
        Dim KINGAKU As String           ' ���z
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
        'Dim KESSAI_CNT As String        ' ���ω�
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
        Dim KINGAKU_FUGOU As String     ' ���z���L����
        Dim KOKYAKU_TESUU As String     ' �ڋq�萔��
        Dim UKETORI_KAMOKU As String    ' ���l�ȖڃR�[�h
        Dim UKETORI_KOUZA As String     ' ���l�����ԍ�
        Dim UKETORI_NAME As String      ' ���l��
        Dim IRAI_NAME As String         ' �˗��l��
        Dim EDI_INFO As String          ' EDI���
        Dim BIKOU1 As String            ' ���l�P
        Dim BIKOU2 As String            ' ���l�Q
        Dim YOBI1 As String             ' �\���P

        '20130920 maeda ���o�b�`�A�g�p
        Dim JUSIN_TEN_BUFF As String
        Dim HASSIN_TEN_BUFF As String
        Dim KINGAKU_BUFF As String
        Dim UKETORI_NAME_BUFF As String
        Dim BIKOU1_BUFF As String
        Dim BIKOU2_BUFF As String
        '20130920 maeda ���o�b�`�A�g�p

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "48"
            OPE_CODE = "100"
            FUKA_CODE = "000"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                '2011/08/24 saitou ��6���S��Ή� UPD ---------------------------------------->>>>
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8), _
                    SubData(SYUMOKU, 4), _
                    SubData(JUSIN_TEN, 30), _
                    SubData(FUKA_CODE, 3), _
                    SubData(HASSIN_TEN, 20), _
                    SubData(KINGAKU, 10), _
                    SubData(KINGAKU_FUGOU, 15), _
                    SubData(KOKYAKU_TESUU, 4), _
                    SubData(UKETORI_KAMOKU, 1), _
                    SubData(UKETORI_KOUZA, 15), _
                    SubData(UKETORI_NAME, 29), _
                    SubData(IRAI_NAME, 48), _
                    SubData(EDI_INFO, 20), _
                    SubData(BIKOU1, 29), _
                    SubData(BIKOU2, 29), _
                    SubData(YOBI1, 55) _
                    })
                'Return String.Concat(New String() _
                '    { _
                '    SubData(TORIATUKAI, 8), _
                '    SubData(SYUMOKU, 4), _
                '    SubData(JUSIN_TEN, 30), _
                '    SubData(FUKA_CODE, 3), _
                '    SubData(HASSIN_TEN, 15), _
                '    SubData(KINGAKU, 10), _
                '    SubData(KESSAI_CNT, 1), _
                '    SubData(KINGAKU_FUGOU, 15), _
                '    SubData(KOKYAKU_TESUU, 4), _
                '    SubData(UKETORI_KAMOKU, 1), _
                '    SubData(UKETORI_KOUZA, 15), _
                '    SubData(UKETORI_NAME, 29), _
                '    SubData(IRAI_NAME, 48), _
                '    SubData(EDI_INFO, 20), _
                '    SubData(BIKOU1, 29), _
                '    SubData(BIKOU2, 29), _
                '    SubData(YOBI1, 59) _
                '    })
                '2011/08/24 saitou ��6���S��Ή� UPD ----------------------------------------<<<<
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                KINGAKU = CuttingData(Value, 10)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                'KESSAI_CNT = CuttingData(Value, 1)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                KINGAKU_FUGOU = CuttingData(Value, 15)
                KOKYAKU_TESUU = CuttingData(Value, 4)
                UKETORI_KAMOKU = CuttingData(Value, 1)
                UKETORI_KOUZA = CuttingData(Value, 15)
                UKETORI_NAME = CuttingData(Value, 29)
                IRAI_NAME = CuttingData(Value, 48)
                EDI_INFO = CuttingData(Value, 20)
                BIKOU1 = CuttingData(Value, 29)
                BIKOU2 = CuttingData(Value, 29)
                '2011/08/24 saitou ��6���S��Ή� �\���P�����O�X�ύX UPD ---------------------------------------->>>>
                YOBI1 = CuttingData(Value, 55)
                'YOBI1 = CuttingData(Value, 59)
                '2011/08/24 saitou ��6���S��Ή� �\���P�����O�X�ύX UPD ----------------------------------------<<<<
            End Set
        End Property

        Public Property DataKinBatch() As String
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8), _
                    SubData(SYUMOKU, 4), _
                    SubData(JUSIN_TEN, 30), _
                    SubData(JUSIN_TEN_BUFF, 1), _
                    SubData(FUKA_CODE, 3), _
                    SubData(HASSIN_TEN, 20), _
                    SubData(HASSIN_TEN_BUFF, 11), _
                    SubData(KINGAKU_BUFF, 5), _
                    SubData(KINGAKU, 10), _
                    SubData(KINGAKU_FUGOU, 15), _
                    SubData(KOKYAKU_TESUU, 4), _
                    SubData(UKETORI_KAMOKU, 1), _
                    SubData(UKETORI_KOUZA, 15), _
                    SubData(UKETORI_NAME, 29), _
                    SubData(UKETORI_NAME_BUFF, 19), _
                    SubData(IRAI_NAME, 48), _
                    SubData(EDI_INFO, 20), _
                    SubData(BIKOU1, 29), _
                    SubData(BIKOU1_BUFF, 19), _
                    SubData(BIKOU2, 29), _
                    SubData(BIKOU2_BUFF, 19), _
                    SubData(YOBI1, 60) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                JUSIN_TEN_BUFF = CuttingData(Value, 1)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                HASSIN_TEN_BUFF = CuttingData(Value, 11)
                KINGAKU_BUFF = CuttingData(Value, 5)
                KINGAKU = CuttingData(Value, 10)
                KINGAKU_FUGOU = CuttingData(Value, 15)
                KOKYAKU_TESUU = CuttingData(Value, 4)
                UKETORI_KAMOKU = CuttingData(Value, 1)
                UKETORI_KOUZA = CuttingData(Value, 15)
                UKETORI_NAME = CuttingData(Value, 29)
                UKETORI_NAME_BUFF = CuttingData(Value, 19)
                IRAI_NAME = CuttingData(Value, 48)
                EDI_INFO = CuttingData(Value, 20)
                BIKOU1 = CuttingData(Value, 29)
                BIKOU1_BUFF = CuttingData(Value, 19)
                BIKOU2 = CuttingData(Value, 29)
                BIKOU2_BUFF = CuttingData(Value, 19)
                YOBI1 = CuttingData(Value, 60)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                '2011/08/24 saitou ��6���S��Ή� UPD ---------------------------------------->>>>
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8).Trim, _
                    SEPARATE, _
                    SubData(SYUMOKU, 4).Trim, _
                    SEPARATE, _
                    SubData(JUSIN_TEN, 30).Trim, _
                    SEPARATE, _
                    SubData(FUKA_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(HASSIN_TEN, 20).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 10).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU_FUGOU, 15).Trim, _
                    SEPARATE, _
                    SubData(KOKYAKU_TESUU, 4).Trim, _
                    SEPARATE, _
                    SubData(UKETORI_KAMOKU, 1).Trim, _
                    SEPARATE, _
                    SubData(UKETORI_KOUZA, 15).Trim, _
                    SEPARATE, _
                    SubData(UKETORI_NAME, 29).Trim, _
                    SEPARATE, _
                    SubData(IRAI_NAME, 48).Trim, _
                    SEPARATE, _
                    SubData(EDI_INFO, 20).Trim, _
                    SEPARATE, _
                    SubData(BIKOU1, 29).Trim, _
                    SEPARATE, _
                    SubData(BIKOU2, 29).Trim, _
                    SEPARATE _
                    })
                'Return String.Concat(New String() _
                '    { _
                '    SubData(TORIATUKAI, 8).Trim, _
                '    SEPARATE, _
                '    SubData(SYUMOKU, 4).Trim, _
                '    SEPARATE, _
                '    SubData(JUSIN_TEN, 30).Trim, _
                '    SEPARATE, _
                '    SubData(FUKA_CODE, 3).Trim, _
                '    SEPARATE, _
                '    SubData(HASSIN_TEN, 15).Trim, _
                '    SEPARATE, _
                '    SubData(KINGAKU, 10).Trim, _
                '    SEPARATE, _
                '    SubData(KESSAI_CNT, 1), _
                '    SEPARATE, _
                '    SubData(KINGAKU_FUGOU, 15).Trim, _
                '    SEPARATE, _
                '    SubData(KOKYAKU_TESUU, 4).Trim, _
                '    SEPARATE, _
                '    SubData(UKETORI_KAMOKU, 1).Trim, _
                '    SEPARATE, _
                '    SubData(UKETORI_KOUZA, 15).Trim, _
                '    SEPARATE, _
                '    SubData(UKETORI_NAME, 29).Trim, _
                '    SEPARATE, _
                '    SubData(IRAI_NAME, 48).Trim, _
                '    SEPARATE, _
                '    SubData(EDI_INFO, 20).Trim, _
                '    SEPARATE, _
                '    SubData(BIKOU1, 29).Trim, _
                '    SEPARATE, _
                '    SubData(BIKOU2, 29).Trim, _
                '    SEPARATE _
                '    })
                '2011/08/24 saitou ��6���S��Ή� UPD ----------------------------------------<<<<
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                KINGAKU = CuttingData(Value, 10)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                'KESSAI_CNT = CuttingData(Value, 1)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                KINGAKU_FUGOU = CuttingData(Value, 15)
                KOKYAKU_TESUU = CuttingData(Value, 4)
                UKETORI_KAMOKU = CuttingData(Value, 1)
                UKETORI_KOUZA = CuttingData(Value, 15)
                UKETORI_NAME = CuttingData(Value, 29)
                IRAI_NAME = CuttingData(Value, 48)
                EDI_INFO = CuttingData(Value, 20)
                BIKOU1 = CuttingData(Value, 29)
                BIKOU2 = CuttingData(Value, 29)
            End Set
        End Property
    End Structure
#End Region

#Region "48-500 �ב֕t��"

    '---------------------------------------------------------
    '2008/5/2
    '
    '���z 
    '�����O�X 15 �� 10 �ɏC��(FJH)
    '
    '���l      
    '�����O�X  1 ��  6 �ɏC��(FJH)
    '
    '---------------------------------------------------------

    Public Structure T_48500
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim TORIATUKAI As String        ' �戵��
        Dim SYUMOKU As String           ' ��ڃR�[�h
        Dim JUSIN_TEN As String         ' ��M�X��
        Dim FUKA_CODE As String         ' �t���R�[�h
        Dim HASSIN_TEN As String        ' ���M�X��
        Dim KINGAKU As String           ' ���z
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
        'Dim KESSAI_CNT As String        ' ���ω�
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
        Dim KINGAKU_FUGOU As String     ' ���z���L����
        Dim BANGOU As String            ' �ԍ�
        Dim SIKIN_JIYUU1 As String      ' �����t�֗��R�P
        Dim SIKIN_JIYUU2 As String      ' �����t�֗��R�Q
        Dim SIKIN_JIYUU3 As String      ' �����t�֗��R�R
        Dim SIKIN_JIYUU4 As String      ' �����t�֗��R�S
        Dim SYOKAI_NO As String         ' �Ɖ�ԍ�
        Dim YOBI1 As String             ' ���l
        '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ---------------------------------------->>>>
        Dim EDI_INFO As String          ' EDI���
        '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ----------------------------------------<<<<

        '20130920 maeda ���o�b�`�A�g�p
        Dim JUSIN_TEN_BUFF As String
        Dim HASSIN_TEN_BUFF As String
        Dim KINGAKU_BUFF As String           ' ���z
        '20130920 maeda ���o�b�`�A�g�p

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "48"
            OPE_CODE = "500"
            SYUMOKU = "4301"
            FUKA_CODE = "000"
            '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
            'KESSAI_CNT = " "
            '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                '2011/08/24 saitou ��6���S��Ή� UPD ---------------------------------------->>>>
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8), _
                    SubData(SYUMOKU, 4), _
                    SubData(JUSIN_TEN, 30), _
                    SubData(FUKA_CODE, 3), _
                    SubData(HASSIN_TEN, 20), _
                    SubData(KINGAKU, 10), _
                    SubData(KINGAKU_FUGOU, 15), _
                    SubData(BANGOU, 16), _
                    SubData(SIKIN_JIYUU1, 48), _
                    SubData(SIKIN_JIYUU2, 48), _
                    SubData(SIKIN_JIYUU3, 48), _
                    SubData(SIKIN_JIYUU4, 48), _
                    SubData(SYOKAI_NO, 15), _
                    SubData(EDI_INFO, 20), _
                    SubData(YOBI1, 6) _
                    })
                'Return String.Concat(New String() _
                '    { _
                '    SubData(TORIATUKAI, 8), _
                '    SubData(SYUMOKU, 4), _
                '    SubData(JUSIN_TEN, 30), _
                '    SubData(FUKA_CODE, 3), _
                '    SubData(HASSIN_TEN, 15), _
                '    SubData(KINGAKU, 10), _
                '    SubData(KESSAI_CNT, 1), _
                '    SubData(KINGAKU_FUGOU, 15), _
                '    SubData(BANGOU, 16), _
                '    SubData(SIKIN_JIYUU1, 48), _
                '    SubData(SIKIN_JIYUU2, 48), _
                '    SubData(SIKIN_JIYUU3, 48), _
                '    SubData(SIKIN_JIYUU4, 48), _
                '    SubData(SYOKAI_NO, 15), _
                '    SubData(YOBI1, 11) _
                '    })
                '2011/08/24 saitou ��6���S��Ή� UPD ----------------------------------------<<<<
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                KINGAKU = CuttingData(Value, 10)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                'KESSAI_CNT = CuttingData(Value, 1)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                KINGAKU_FUGOU = CuttingData(Value, 15)
                BANGOU = CuttingData(Value, 16)
                SIKIN_JIYUU1 = CuttingData(Value, 48)
                SIKIN_JIYUU2 = CuttingData(Value, 48)
                SIKIN_JIYUU3 = CuttingData(Value, 48)
                SIKIN_JIYUU4 = CuttingData(Value, 48)
                SYOKAI_NO = CuttingData(Value, 15)
                '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ---------------------------------------->>>>
                EDI_INFO = CuttingData(Value, 20)
                '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ----------------------------------------<<<<
                YOBI1 = CuttingData(Value, 6)
            End Set
        End Property

        Public Property DataKinBatch() As String
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8), _
                    SubData(SYUMOKU, 4), _
                    SubData(JUSIN_TEN, 30), _
                    SubData(JUSIN_TEN_BUFF, 1), _
                    SubData(FUKA_CODE, 3), _
                    SubData(HASSIN_TEN, 20), _
                    SubData(HASSIN_TEN_BUFF, 11), _
                    SubData(KINGAKU_BUFF, 5), _
                    SubData(KINGAKU, 10), _
                    SubData(KINGAKU_FUGOU, 15), _
                    SubData(BANGOU, 16), _
                    SubData(SIKIN_JIYUU1, 48), _
                    SubData(SIKIN_JIYUU2, 48), _
                    SubData(SIKIN_JIYUU3, 48), _
                    SubData(SIKIN_JIYUU4, 48), _
                    SubData(SYOKAI_NO, 15), _
                    SubData(EDI_INFO, 20), _
                    SubData(YOBI1, 11) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                JUSIN_TEN_BUFF = CuttingData(Value, 1)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                HASSIN_TEN_BUFF = CuttingData(Value, 11)
                KINGAKU_BUFF = CuttingData(Value, 5)
                KINGAKU = CuttingData(Value, 10)
                KINGAKU_FUGOU = CuttingData(Value, 15)
                BANGOU = CuttingData(Value, 16)
                SIKIN_JIYUU1 = CuttingData(Value, 48)
                SIKIN_JIYUU2 = CuttingData(Value, 48)
                SIKIN_JIYUU3 = CuttingData(Value, 48)
                SIKIN_JIYUU4 = CuttingData(Value, 48)
                SYOKAI_NO = CuttingData(Value, 15)
                EDI_INFO = CuttingData(Value, 20)
                YOBI1 = CuttingData(Value, 11)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                '2011/08/24 saitou ��6���S��Ή� UPD ---------------------------------------->>>>
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8).Trim, _
                    SEPARATE, _
                    SubData(SYUMOKU, 4).Trim, _
                    SEPARATE, _
                    SubData(JUSIN_TEN, 30).Trim, _
                    SEPARATE, _
                    SubData(FUKA_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(HASSIN_TEN, 20).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 10).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU_FUGOU, 15).Trim, _
                    SEPARATE, _
                    SubData(BANGOU, 16).Trim, _
                    SEPARATE, _
                    SubData(SIKIN_JIYUU1, 48).Trim, _
                    SEPARATE, _
                    SubData(SIKIN_JIYUU2, 48).Trim, _
                    SEPARATE, _
                    SubData(SIKIN_JIYUU3, 48).Trim, _
                    SEPARATE, _
                    SubData(SIKIN_JIYUU4, 48).Trim, _
                    SEPARATE, _
                    SubData(SYOKAI_NO, 15).Trim, _
                    SEPARATE, _
                    SubData(EDI_INFO, 20).Trim, _
                    SEPARATE _
                    })
                'Return String.Concat(New String() _
                '    { _
                '    SubData(TORIATUKAI, 8).Trim, _
                '    SEPARATE, _
                '    SubData(SYUMOKU, 4).Trim, _
                '    SEPARATE, _
                '    SubData(JUSIN_TEN, 30).Trim, _
                '    SEPARATE, _
                '    SubData(FUKA_CODE, 3).Trim, _
                '    SEPARATE, _
                '    SubData(HASSIN_TEN, 15).Trim, _
                '    SEPARATE, _
                '    SubData(KINGAKU, 10).Trim, _
                '    SEPARATE, _
                '    SubData(KESSAI_CNT, 1), _
                '    SEPARATE, _
                '    SubData(KINGAKU_FUGOU, 15).Trim, _
                '    SEPARATE, _
                '    SubData(BANGOU, 16).Trim, _
                '    SEPARATE, _
                '    SubData(SIKIN_JIYUU1, 48).Trim, _
                '    SEPARATE, _
                '    SubData(SIKIN_JIYUU2, 48).Trim, _
                '    SEPARATE, _
                '    SubData(SIKIN_JIYUU3, 48).Trim, _
                '    SEPARATE, _
                '    SubData(SIKIN_JIYUU4, 48).Trim, _
                '    SEPARATE, _
                '    SubData(SYOKAI_NO, 15).Trim, _
                '    SEPARATE _
                '    })
                '2011/08/24 saitou ��6���S��Ή� UPD ----------------------------------------<<<<
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                KINGAKU = CuttingData(Value, 10)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                'KESSAI_CNT = CuttingData(Value, 1)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                KINGAKU_FUGOU = CuttingData(Value, 15)
                BANGOU = CuttingData(Value, 16)
                SIKIN_JIYUU1 = CuttingData(Value, 48)
                SIKIN_JIYUU2 = CuttingData(Value, 48)
                SIKIN_JIYUU3 = CuttingData(Value, 48)
                SIKIN_JIYUU4 = CuttingData(Value, 48)
                SYOKAI_NO = CuttingData(Value, 15)
                '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ---------------------------------------->>>>
                EDI_INFO = CuttingData(Value, 20)
                '2011/08/24 saitou ��6���S��Ή� EDI���ǉ� ADD ----------------------------------------<<<<
            End Set
        End Property
    End Structure
#End Region

#Region "48-600 �ב֐���"


    '*** �C�� kakinoki 2008/05/06      ***
    '*** ���z  �����O�X 15 �� 10 �ɏC��***
    '*** YOBI1 �����O�X 39 �� 44       ***
    '*** 

    Public Structure T_48600
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim TORIATUKAI As String        ' �戵��
        Dim SYUMOKU As String           ' ��ڃR�[�h
        Dim JUSIN_TEN As String         ' ��M�X��
        Dim FUKA_CODE As String         ' �t���R�[�h
        Dim HASSIN_TEN As String        ' ���M�X��
        Dim KINGAKU As String           ' ���z
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
        'Dim KESSAI_CNT As String        ' ���ω�
        '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
        Dim KINGAKU_FUGOU As String     ' ���z���L����
        Dim BANGOU As String            ' �ԍ�
        Dim SIKIN_JIYUU1 As String      ' �����t�֗��R�P
        Dim SIKIN_JIYUU2 As String      ' �����t�֗��R�Q
        Dim BIKOU1 As String            ' ���l�P
        Dim BIKOU2 As String            ' ���l�Q
        Dim SYOKAI_NO As String         ' �Ɖ�ԍ�
        Dim YOBI1 As String             ' ���l

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "48"
            OPE_CODE = "600"
            SYUMOKU = "4701"
            FUKA_CODE = "000"
            '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
            'KESSAI_CNT = " "
            '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
            HASSIN_TEN = "� ���-"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                '2011/08/24 saitou ��6���S��Ή� UPD ---------------------------------------->>>>
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8), _
                    SubData(SYUMOKU, 4), _
                    SubData(JUSIN_TEN, 30), _
                    SubData(FUKA_CODE, 3), _
                    SubData(HASSIN_TEN, 20), _
                    SubData(KINGAKU, 10), _
                    SubData(KINGAKU_FUGOU, 15), _
                    SubData(BANGOU, 16), _
                    SubData(SIKIN_JIYUU1, 48), _
                    SubData(SIKIN_JIYUU2, 48), _
                    SubData(BIKOU1, 29), _
                    SubData(BIKOU2, 29), _
                    SubData(SYOKAI_NO, 15), _
                    SubData(YOBI1, 45) _
                    })
                'Return String.Concat(New String() _
                '    { _
                '    SubData(TORIATUKAI, 8), _
                '    SubData(SYUMOKU, 4), _
                '    SubData(JUSIN_TEN, 30), _
                '    SubData(FUKA_CODE, 3), _
                '    SubData(HASSIN_TEN, 15), _
                '    SubData(KINGAKU, 10), _
                '    SubData(KESSAI_CNT, 1), _
                '    SubData(KINGAKU_FUGOU, 15), _
                '    SubData(BANGOU, 16), _
                '    SubData(SIKIN_JIYUU1, 48), _
                '    SubData(SIKIN_JIYUU2, 48), _
                '    SubData(BIKOU1, 29), _
                '    SubData(BIKOU2, 29), _
                '    SubData(SYOKAI_NO, 15), _
                '    SubData(YOBI1, 49) _
                '    })
                '2011/08/24 saitou ��6���S��Ή� UPD ----------------------------------------<<<<
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                KINGAKU = CuttingData(Value, 10)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                'KESSAI_CNT = CuttingData(Value, 1)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                KINGAKU_FUGOU = CuttingData(Value, 15)
                BANGOU = CuttingData(Value, 16)
                SIKIN_JIYUU1 = CuttingData(Value, 48)
                SIKIN_JIYUU2 = CuttingData(Value, 48)
                BIKOU1 = CuttingData(Value, 29)
                BIKOU2 = CuttingData(Value, 29)
                SYOKAI_NO = CuttingData(Value, 15)
                '2011/08/24 saitou ��6���S��Ή� �\���P�����O�X�ύX UPD ---------------------------------------->>>>
                YOBI1 = CuttingData(Value, 45)
                'YOBI1 = CuttingData(Value, 49)
                '2011/08/24 saitou ��6���S��Ή� �\���P�����O�X�ύX UPD ----------------------------------------<<<<
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                '2011/08/24 saitou ��6���S��Ή� UPD ---------------------------------------->>>>
                Return String.Concat(New String() _
                    { _
                    SubData(TORIATUKAI, 8).Trim, _
                    SEPARATE, _
                    SubData(SYUMOKU, 4).Trim, _
                    SEPARATE, _
                    SubData(JUSIN_TEN, 30).Trim, _
                    SEPARATE, _
                    SubData(FUKA_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(HASSIN_TEN, 20).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 10).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU_FUGOU, 15).Trim, _
                    SEPARATE, _
                    SubData(BANGOU, 16).Trim, _
                    SEPARATE, _
                    SubData(SIKIN_JIYUU1, 48).Trim, _
                    SEPARATE, _
                    SubData(SIKIN_JIYUU2, 48).Trim, _
                    SEPARATE, _
                    SubData(BIKOU1, 29).Trim, _
                    SEPARATE, _
                    SubData(BIKOU2, 29).Trim, _
                    SEPARATE, _
                    SubData(SYOKAI_NO, 15).Trim, _
                    SEPARATE _
                    })
                'Return String.Concat(New String() _
                '    { _
                '    SubData(TORIATUKAI, 8).Trim, _
                '    SEPARATE, _
                '    SubData(SYUMOKU, 4).Trim, _
                '    SEPARATE, _
                '    SubData(JUSIN_TEN, 30).Trim, _
                '    SEPARATE, _
                '    SubData(FUKA_CODE, 3).Trim, _
                '    SEPARATE, _
                '    SubData(HASSIN_TEN, 15).Trim, _
                '    SEPARATE, _
                '    SubData(KINGAKU, 10).Trim, _
                '    SEPARATE, _
                '    SubData(KESSAI_CNT, 1), _
                '    SEPARATE, _
                '    SubData(KINGAKU_FUGOU, 15).Trim, _
                '    SEPARATE, _
                '    SubData(BANGOU, 16).Trim, _
                '    SEPARATE, _
                '    SubData(SIKIN_JIYUU1, 48).Trim, _
                '    SEPARATE, _
                '    SubData(SIKIN_JIYUU2, 48).Trim, _
                '    SEPARATE, _
                '    SubData(BIKOU1, 29).Trim, _
                '    SEPARATE, _
                '    SubData(BIKOU2, 29).Trim, _
                '    SEPARATE, _
                '    SubData(SYOKAI_NO, 15).Trim, _
                '    SEPARATE _
                '    })
                '2011/08/24 saitou ��6���S��Ή� UPD ----------------------------------------<<<<
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TORIATUKAI = CuttingData(Value, 8)
                SYUMOKU = CuttingData(Value, 4)
                JUSIN_TEN = CuttingData(Value, 30)
                FUKA_CODE = CuttingData(Value, 3)
                HASSIN_TEN = CuttingData(Value, 20)
                KINGAKU = CuttingData(Value, 10)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ---------------------------------------->>>>
                'KESSAI_CNT = CuttingData(Value, 1)
                '2011/08/24 saitou ��6���S��Ή� ���ω񐔍폜 DEL ----------------------------------------<<<<
                KINGAKU_FUGOU = CuttingData(Value, 15)
                BANGOU = CuttingData(Value, 16)
                SIKIN_JIYUU1 = CuttingData(Value, 48)
                SIKIN_JIYUU2 = CuttingData(Value, 48)
                BIKOU1 = CuttingData(Value, 29)
                BIKOU2 = CuttingData(Value, 29)
                SYOKAI_NO = CuttingData(Value, 15)
            End Set
        End Property
    End Structure
#End Region

    '�ύX�Ȃ��H
#Region "99-418 �萔�������i�A���j"

    '---------------------------------------------------------
    '2018/01/11 FJH����
    '�N�Z���F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 235��234
    '---------------------------------------------------------

    Public Structure T_99418
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim TESUU_SYUBETU As String     ' �萔����ʂb
        Dim TEUSU_UTIWAKE As String     ' �萔������b
        Dim UTIWAKE_CODE As String      ' �����X�� 
        Dim KAMOKU_KOBAN As String      ' �����ȖځE���� 
        Dim KOKYAKU_NO As String        ' �ڋq�ԍ� 
        Dim KAIIN_CODE As String        ' ����R�[�h
        Dim TESUUTYO_KBN As String      ' �萔�������敪
        Dim TESUU_KINGAKU As String     ' �萔���z 
        Dim TESUU_KEN As String         ' �萔������
        Dim CLKAMOKU_KOBAN As String    ' �b�k�Ȗڌ���
        Dim TEKIYO As String            ' �����薾�דE�v
        Dim KOUSYU_NO As String         ' ������הԍ� 
        Dim KISANBI As String           ' �N�Z��
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "99"
            OPE_CODE = "418"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(TESUU_SYUBETU, 3), _
                    SubData(TEUSU_UTIWAKE, 2), _
                    SubData(UTIWAKE_CODE, 3), _
                    SubData(KAMOKU_KOBAN, 9), _
                    SubData(KOKYAKU_NO, 10), _
                    SubData(KAIIN_CODE, 1), _
                    SubData(TESUUTYO_KBN, 1), _
                    SubData(TESUU_KINGAKU, 10), _
                    SubData(TESUU_KEN, 4), _
                    SubData(CLKAMOKU_KOBAN, 12), _
                    SubData(TEKIYO, 20), _
                    SubData(KOUSYU_NO, 4), _
                    SubData(KISANBI, 7), _
                    SubData(YOBI1, 234) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TESUU_SYUBETU = CuttingData(Value, 3)
                TEUSU_UTIWAKE = CuttingData(Value, 2)
                UTIWAKE_CODE = CuttingData(Value, 3)
                KAMOKU_KOBAN = CuttingData(Value, 9)
                KOKYAKU_NO = CuttingData(Value, 10)
                KAIIN_CODE = CuttingData(Value, 1)
                TESUUTYO_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 10)
                TESUU_KEN = CuttingData(Value, 4)
                CLKAMOKU_KOBAN = CuttingData(Value, 12)
                TEKIYO = CuttingData(Value, 20)
                KOUSYU_NO = CuttingData(Value, 4)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'YOBI1 = CuttingData(Value, 235)
                YOBI1 = CuttingData(Value, 234)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(TESUU_SYUBETU, 3).Trim, _
                    SEPARATE, _
                    SubData(TEUSU_UTIWAKE, 2).Trim, _
                    SEPARATE, _
                    SubData(UTIWAKE_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(KAMOKU_KOBAN, 9).Trim, _
                    SEPARATE, _
                    SubData(KOKYAKU_NO, 10).Trim, _
                    SEPARATE, _
                    SubData(KAIIN_CODE, 1).Trim, _
                    SEPARATE, _
                    SubData(TESUUTYO_KBN, 1).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KINGAKU, 10).Trim, _
                    SEPARATE, _
                    SubData(TESUU_KEN, 4).Trim, _
                    SEPARATE, _
                    SubData(CLKAMOKU_KOBAN, 12).Trim, _
                    SEPARATE, _
                    SubData(TEKIYO, 20).Trim, _
                    SEPARATE, _
                    SubData(KOUSYU_NO, 4).Trim, _
                    SEPARATE, _
                    SubData(KISANBI, 7).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                TESUU_SYUBETU = CuttingData(Value, 3)
                TEUSU_UTIWAKE = CuttingData(Value, 2)
                UTIWAKE_CODE = CuttingData(Value, 3)
                KAMOKU_KOBAN = CuttingData(Value, 9)
                KOKYAKU_NO = CuttingData(Value, 10)
                KAIIN_CODE = CuttingData(Value, 1)
                TESUUTYO_KBN = CuttingData(Value, 1)
                TESUU_KINGAKU = CuttingData(Value, 10)
                TESUU_KEN = CuttingData(Value, 4)
                CLKAMOKU_KOBAN = CuttingData(Value, 12)
                TEKIYO = CuttingData(Value, 20)
                KOUSYU_NO = CuttingData(Value, 4)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
            End Set
        End Property
    End Structure
#End Region

#Region "99-411 ������A������(NB)"

    '---------------------------------------------------------
    '2018/05/10 �^�X�N�֓�
    '�N�Z���F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 297��217
    '---------------------------------------------------------

    Public Structure T_99411
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        '2018/04/06 saitou �L���M��(RSV2�W��) DEL �t�H�[�}�b�g�C�� ---------------------------------------- START
        '�s�͕s�v
        'Dim GYO As String               ' �s
        '2018/04/06 saitou �L���M��(RSV2�W��) DEL --------------------------------------------------------- END
        Dim UTIWAKE_CODE As String      ' ����R�[�h
        Dim KINGAKU As String           ' ���z
        Dim KENSU As String             ' ����
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim TORIATUKAI1 As String       ' �戵�ԍ��P
        Dim JINKAKU_CODE As String      ' �l�i�R�[�h
        Dim KAZEI_CODE As String        ' �ېŃR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim RENDO_TEN As String         ' �A���X��
        Dim RENDO_KAMOKU As String      ' �A���Ȗڌ���
        Dim AITE_UTIWAKE As String      ' �������R�[�h
        Dim SOFT_NO As String           ' �\�t�g�@�@��
        Dim TORIATUKAI2 As String       ' �戵�ԍ�2
        Dim AITE_TEKIYOU As String      ' ����E�v
        Dim KISANBI As String           ' �N�Z��
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "99"
            OPE_CODE = "411"
            '2018/04/06 saitou �L���M��(RSV2�W��) DEL �t�H�[�}�b�g�C�� ---------------------------------------- START
            'GYO = "01"
            '2018/04/06 saitou �L���M��(RSV2�W��) DEL --------------------------------------------------------- END
            KENSU = "1".PadLeft(5, " "c)
            TEKIYOU = "ý��ֳ"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '�s�͕s�v
                Return String.Concat(New String() _
                    {
                    SubData(KOUZA_NO, 7),
                    SubData(UTIWAKE_CODE, 2),
                    SubData(KINGAKU, 13),
                    SubData(KENSU, 5),
                    SubData(FURI_CODE, 3),
                    SubData(TORIATUKAI1, 3),
                    SubData(JINKAKU_CODE, 2),
                    SubData(KAZEI_CODE, 1),
                    SubData(TEKIYOU, 20),
                    SubData(RENDO_TEN, 3),
                    SubData(RENDO_KAMOKU, 9),
                    SubData(AITE_UTIWAKE, 2),
                    SubData(SOFT_NO, 1),
                    SubData(TORIATUKAI2, 5),
                    SubData(AITE_TEKIYOU, 20),
                    SubData(KISANBI, 7),
                    SubData(YOBI1, 217)
                    })

                'Return String.Concat(New String() _
                '    {
                '    SubData(KOUZA_NO, 7), _
                '    SubData(GYO, 2), _
                '    SubData(UTIWAKE_CODE, 2), _
                '    SubData(KINGAKU, 13), _
                '    SubData(KENSU, 5), _
                '    SubData(FURI_CODE, 3), _
                '    SubData(TORIATUKAI1, 3), _
                '    SubData(JINKAKU_CODE, 2), _
                '    SubData(KAZEI_CODE, 1), _
                '    SubData(TEKIYOU, 20), _
                '    SubData(RENDO_TEN, 3), _
                '    SubData(RENDO_KAMOKU, 9), _
                '    SubData(AITE_UTIWAKE, 2), _
                '    SubData(SOFT_NO, 1), _
                '    SubData(TORIATUKAI2, 5), _
                '    SubData(AITE_TEKIYOU, 20), _
                '    SubData(KISANBI, 6), _
                '    SubData(YOBI1, 296) _
                '    })
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                '2018/04/06 saitou �L���M��(RSV2�W��) DEL �t�H�[�}�b�g�C�� ---------------------------------------- START
                'GYO = CuttingData(Value, 2)
                '2018/04/06 saitou �L���M��(RSV2�W��) DEL --------------------------------------------------------- END
                UTIWAKE_CODE = CuttingData(Value, 2)
                KINGAKU = CuttingData(Value, 13)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                RENDO_TEN = CuttingData(Value, 3)
                RENDO_KAMOKU = CuttingData(Value, 9)
                AITE_UTIWAKE = CuttingData(Value, 2)
                SOFT_NO = CuttingData(Value, 1)
                TORIATUKAI2 = CuttingData(Value, 5)
                AITE_TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                'YOBI1 = CuttingData(Value, 297)
                'YOBI1 = CuttingData(Value, 296)
                YOBI1 = CuttingData(Value, 217)
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '�s�͕s�v
                Return String.Concat(New String() _
                    {
                    SubData(KOUZA_NO, 7).Trim,
                    SEPARATE,
                    SubData(UTIWAKE_CODE, 2).Trim,
                    SEPARATE,
                    SubData(KINGAKU, 13).Trim,
                    SEPARATE,
                    SubData(KENSU, 5).Trim,
                    SEPARATE,
                    SubData(FURI_CODE, 3).Trim,
                    SEPARATE,
                    SubData(TORIATUKAI1, 3).Trim,
                    SEPARATE,
                    SubData(JINKAKU_CODE, 2).Trim,
                    SubData(KAZEI_CODE, 1).Trim,
                    SEPARATE,
                    SubData(TEKIYOU, 20).Trim,
                    SEPARATE,
                    SubData(RENDO_TEN, 3).Trim,
                    SEPARATE,
                    SubData(RENDO_KAMOKU, 9).Trim,
                    SEPARATE,
                    SubData(AITE_UTIWAKE, 2).Trim,
                    SEPARATE,
                    SubData(SOFT_NO, 1).Trim,
                    SEPARATE,
                    SubData(TORIATUKAI2, 5).Trim,
                    SEPARATE,
                    SubData(AITE_TEKIYOU, 20).Trim,
                    SEPARATE,
                    SubData(KISANBI, 7).Trim,
                    SEPARATE
                    })

                'Return String.Concat(New String() _
                '    { _
                '    SubData(KOUZA_NO, 7).Trim, _
                '    SEPARATE, _
                '    SubData(GYO, 2).Trim, _
                '    SEPARATE, _
                '    SubData(UTIWAKE_CODE, 2).Trim, _
                '    SEPARATE, _
                '    SubData(KINGAKU, 13).Trim, _
                '    SEPARATE, _
                '    SubData(KENSU, 5).Trim, _
                '    SEPARATE, _
                '    SubData(FURI_CODE, 3).Trim, _
                '    SEPARATE, _
                '    SubData(TORIATUKAI1, 3).Trim, _
                '    SEPARATE, _
                '    SubData(JINKAKU_CODE, 2).Trim, _
                '    SubData(KAZEI_CODE, 1).Trim, _
                '    SEPARATE, _
                '    SubData(TEKIYOU, 20).Trim, _
                '    SEPARATE, _
                '    SubData(RENDO_TEN, 3).Trim, _
                '    SEPARATE, _
                '    SubData(RENDO_KAMOKU, 9).Trim, _
                '    SEPARATE, _
                '    SubData(AITE_UTIWAKE, 2).Trim, _
                '    SEPARATE, _
                '    SubData(SOFT_NO, 1).Trim, _
                '    SEPARATE, _
                '    SubData(TORIATUKAI2, 5).Trim, _
                '    SEPARATE, _
                '    SubData(AITE_TEKIYOU, 20).Trim, _
                '    SEPARATE, _
                '    SubData(KISANBI, 6).Trim, _
                '    SEPARATE _
                '    })
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                '2018/04/06 saitou �L���M��(RSV2�W��) DEL �t�H�[�}�b�g�C�� ---------------------------------------- START
                'GYO = CuttingData(Value, 2)
                '2018/04/06 saitou �L���M��(RSV2�W��) DEL --------------------------------------------------------- END
                UTIWAKE_CODE = CuttingData(Value, 2)
                KINGAKU = CuttingData(Value, 13)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                RENDO_TEN = CuttingData(Value, 3)
                RENDO_KAMOKU = CuttingData(Value, 9)
                AITE_UTIWAKE = CuttingData(Value, 2)
                SOFT_NO = CuttingData(Value, 1)
                TORIATUKAI2 = CuttingData(Value, 5)
                AITE_TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
            End Set
        End Property
    End Structure
#End Region

#Region "99-419 ������A������"

    '---------------------------------------------------------
    '2018/01/11 FJH����
    '�N�Z���F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 200��199
    '---------------------------------------------------------

    Public Structure T_99419
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim GYO As String               ' �s
        Dim UTIWAKE_CODE As String      ' ����R�[�h
        Dim ZENZAN As String            ' �O�c      
        Dim FUGOU_CODE As String        ' �����R�[�h
        Dim KINGAKU As String           ' ���z
        Dim KENSU As String             ' ����
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim TORIATUKAI1 As String       ' �戵�ԍ��P
        Dim JINKAKU_CODE As String      ' �l�i�R�[�h
        Dim KAZEI_CODE As String        ' �ېŃR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim RENDO_TEN As String         ' �A���X��
        Dim RENDO_KAMOKU As String      ' �A���Ȗڌ���
        Dim AITE_UTIWAKE As String      ' �������R�[�h
        Dim SOFT_NO As String           ' �\�t�g�@�@��
        Dim TORIATUKAI2 As String       ' �戵�ԍ�2
        Dim AITE_TEKIYOU As String      ' ����E�v
        Dim KISANBI As String           ' �N�Z��
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "99"
            OPE_CODE = "419"
            GYO = "01"
            ZENZAN = "0".PadRight(15, " "c)
            KENSU = "1".PadLeft(5, " "c)
            TEKIYOU = "ý��ֳ"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7), _
                    SubData(GYO, 2), _
                    SubData(UTIWAKE_CODE, 2), _
                    SubData(ZENZAN, 15), _
                    SubData(FUGOU_CODE, 1), _
                    SubData(KINGAKU, 13), _
                    SubData(KENSU, 5), _
                    SubData(FURI_CODE, 3), _
                    SubData(TORIATUKAI1, 3), _
                    SubData(JINKAKU_CODE, 2), _
                    SubData(KAZEI_CODE, 1), _
                    SubData(TEKIYOU, 20), _
                    SubData(RENDO_TEN, 3), _
                    SubData(RENDO_KAMOKU, 9), _
                    SubData(AITE_UTIWAKE, 2), _
                    SubData(SOFT_NO, 1), _
                    SubData(TORIATUKAI2, 5), _
                    SubData(AITE_TEKIYOU, 20), _
                    SubData(KISANBI, 7), _
                    SubData(YOBI1, 199) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                GYO = CuttingData(Value, 2)
                UTIWAKE_CODE = CuttingData(Value, 2)
                ZENZAN = CuttingData(Value, 15)
                FUGOU_CODE = CuttingData(Value, 1)
                KINGAKU = CuttingData(Value, 13)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                RENDO_TEN = CuttingData(Value, 3)
                RENDO_KAMOKU = CuttingData(Value, 9)
                AITE_UTIWAKE = CuttingData(Value, 2)
                SOFT_NO = CuttingData(Value, 1)
                TORIATUKAI2 = CuttingData(Value, 5)
                AITE_TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'YOBI1 = CuttingData(Value, 200)
                YOBI1 = CuttingData(Value, 199)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KOUZA_NO, 7).Trim, _
                    SEPARATE, _
                    SubData(GYO, 2).Trim, _
                    SEPARATE, _
                    SubData(UTIWAKE_CODE, 2).Trim, _
                    SEPARATE, _
                    SubData(ZENZAN, 15).Trim, _
                    SEPARATE, _
                    SubData(FUGOU_CODE, 1).Trim, _
                    SEPARATE, _
                    SubData(KINGAKU, 13).Trim, _
                    SEPARATE, _
                    SubData(KENSU, 5).Trim, _
                    SEPARATE, _
                    SubData(FURI_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(TORIATUKAI1, 3).Trim, _
                    SEPARATE, _
                    SubData(JINKAKU_CODE, 2).Trim, _
                    SubData(KAZEI_CODE, 1).Trim, _
                    SEPARATE, _
                    SubData(TEKIYOU, 20).Trim, _
                    SEPARATE, _
                    SubData(RENDO_TEN, 3).Trim, _
                    SEPARATE, _
                    SubData(RENDO_KAMOKU, 9).Trim, _
                    SEPARATE, _
                    SubData(AITE_UTIWAKE, 2).Trim, _
                    SEPARATE, _
                    SubData(SOFT_NO, 1).Trim, _
                    SEPARATE, _
                    SubData(TORIATUKAI2, 5).Trim, _
                    SEPARATE, _
                    SubData(AITE_TEKIYOU, 20).Trim, _
                    SEPARATE, _
                    SubData(KISANBI, 7).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KOUZA_NO = CuttingData(Value, 7)
                GYO = CuttingData(Value, 2)
                UTIWAKE_CODE = CuttingData(Value, 2)
                ZENZAN = CuttingData(Value, 15)
                FUGOU_CODE = CuttingData(Value, 1)
                KINGAKU = CuttingData(Value, 13)
                KENSU = CuttingData(Value, 5)
                FURI_CODE = CuttingData(Value, 3)
                TORIATUKAI1 = CuttingData(Value, 3)
                JINKAKU_CODE = CuttingData(Value, 2)
                KAZEI_CODE = CuttingData(Value, 1)
                TEKIYOU = CuttingData(Value, 20)
                RENDO_TEN = CuttingData(Value, 3)
                RENDO_KAMOKU = CuttingData(Value, 9)
                AITE_UTIWAKE = CuttingData(Value, 2)
                SOFT_NO = CuttingData(Value, 1)
                TORIATUKAI2 = CuttingData(Value, 5)
                AITE_TEKIYOU = CuttingData(Value, 20)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
            End Set
        End Property
    End Structure
#End Region

    '�ύX�Ȃ��H
#Region "04-419 �U�֘A������"
    Public Structure T_04419

        '******  kakinoki 2008/5/9  ******
        '
        '�A���X��     �����O�X   6 ��   3
        '
        '�A���Ȗڌ��� �����O�X  12 ��   9
        '
        '�\��         �����O�X 216 �� 222
        '
        '******  kakinoki 2008/5/9  ******

        '---------------------------------------------------------
        '2018/05/10 �^�X�N�֓�
        '�N�Z���A����t�\����F�����O�X�ύX 6��7
        '�\���P�F�����O�X�ύX 293��291
        '---------------------------------------------------------

        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KOUZA_NO As String          ' �����ԍ�
        Dim KINGAKU As String           ' ���z
        Dim FURI_CODE As String         ' �U�փR�[�h
        Dim KIGYO_CODE As String        ' ��ƃR�[�h
        Dim TEKIYOU As String           ' �E�v
        Dim TORIATUKAI1 As String       ' �戵�ԍ��P
        Dim KENSU As String             ' ����
        Dim TEGATA_NO As String         ' ��`���؎�ԍ�
        Dim RENDO_TEN As String         ' �A���X��
        Dim RENDO_KAMOKU As String      ' �A���Ȗڌ���
        Dim TORIATUKAI2 As String       ' �戵�ԍ�2
        '2018/04/06 saitou �L���M��(RSV2�W��) ADD �t�H�[�}�b�g�C�� ---------------------------------------- START
        Dim NOUZEI_RENDO_FURIKAE_C As String    ' �[�ŘA���U�ւb
        '2018/04/06 saitou �L���M��(RSV2�W��) ADD --------------------------------------------------------- END
        Dim AITE_TEKIYOU As String      ' �A������E�v
        Dim KISANBI As String           ' �N�Z��
        Dim SAKIHIDUKE_YOTEI As String  ' ����t�\���
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "04"
            OPE_CODE = "419"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '���z���g���A��ƃR�[�h���g���A�[�ŘA���U�ւb�ǉ��A�\�����g��
                Return String.Concat(New String() _
                    {
                    SubData(KOUZA_NO, 7),
                    SubData(KINGAKU, 13),
                    SubData(FURI_CODE, 3),
                    SubData(KIGYO_CODE, 5),
                    SubData(TEKIYOU, 16),
                    SubData(TORIATUKAI1, 5),
                    SubData(KENSU, 4),
                    SubData(TEGATA_NO, 6),
                    SubData(RENDO_TEN, 3),
                    SubData(RENDO_KAMOKU, 9),
                    SubData(TORIATUKAI2, 5),
                    SubData(NOUZEI_RENDO_FURIKAE_C, 3),
                    SubData(AITE_TEKIYOU, 16),
                    SubData(KISANBI, 7),
                    SubData(SAKIHIDUKE_YOTEI, 7),
                    SubData(YOBI1, 291)
                    })

                'Return String.Concat(New String() _
                '    { _
                '    SubData(KOUZA_NO, 7), _
                '    SubData(KINGAKU, 10), _
                '    SubData(FURI_CODE, 3), _
                '    SubData(KIGYO_CODE, 3), _
                '    SubData(TEKIYOU, 16), _
                '    SubData(TORIATUKAI1, 5), _
                '    SubData(KENSU, 4), _
                '    SubData(TEGATA_NO, 6), _
                '    SubData(RENDO_TEN, 3), _
                '    SubData(RENDO_KAMOKU, 9), _
                '    SubData(TORIATUKAI2, 5), _
                '    SubData(AITE_TEKIYOU, 16), _
                '    SubData(KISANBI, 6), _
                '    SubData(SAKIHIDUKE_YOTEI, 6), _
                '    SubData(YOBI1, 227) _
                '    })
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '���z���g���A��ƃR�[�h���g���A�[�ŘA���U�ւb�ǉ��A�\�����g��
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                FURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                TEKIYOU = CuttingData(Value, 16)
                TORIATUKAI1 = CuttingData(Value, 5)
                KENSU = CuttingData(Value, 4)
                TEGATA_NO = CuttingData(Value, 6)
                RENDO_TEN = CuttingData(Value, 3)
                RENDO_KAMOKU = CuttingData(Value, 9)
                TORIATUKAI2 = CuttingData(Value, 5)
                NOUZEI_RENDO_FURIKAE_C = CuttingData(Value, 3)
                AITE_TEKIYOU = CuttingData(Value, 16)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)
                'YOBI1 = CuttingData(Value, 293)
                YOBI1 = CuttingData(Value, 291)

                'KOUZA_NO = CuttingData(Value, 7)
                'KINGAKU = CuttingData(Value, 10)
                'FURI_CODE = CuttingData(Value, 3)
                'KIGYO_CODE = CuttingData(Value, 3)
                'TEKIYOU = CuttingData(Value, 16)
                'TORIATUKAI1 = CuttingData(Value, 5)
                'KENSU = CuttingData(Value, 4)
                'TEGATA_NO = CuttingData(Value, 6)
                'RENDO_TEN = CuttingData(Value, 3)
                'RENDO_KAMOKU = CuttingData(Value, 9)
                'TORIATUKAI2 = CuttingData(Value, 5)
                'AITE_TEKIYOU = CuttingData(Value, 16)
                'KISANBI = CuttingData(Value, 6)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                'YOBI1 = CuttingData(Value, 227)
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Set
        End Property


        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '���z���g���A��ƃR�[�h���g���A�[�ŘA���U�ւb�ǉ�
                '�Z�p���[�^�łȂ̂ɃZ�p���[�^�������̂��C��
                Return String.Concat(New String() _
                    {
                    SubData(KOUZA_NO, 7).Trim,
                    SEPARATE,
                    SubData(KINGAKU, 13).Trim,
                    SEPARATE,
                    SubData(FURI_CODE, 3).Trim,
                    SEPARATE,
                    SubData(KIGYO_CODE, 5).Trim,
                    SEPARATE,
                    SubData(TEKIYOU, 16).Trim,
                    SEPARATE,
                    SubData(TORIATUKAI1, 5).Trim,
                    SEPARATE,
                    SubData(KENSU, 4).Trim,
                    SEPARATE,
                    SubData(TEGATA_NO, 6).Trim,
                    SEPARATE,
                    SubData(RENDO_TEN, 3).Trim,
                    SEPARATE,
                    SubData(RENDO_KAMOKU, 9).Trim,
                    SEPARATE,
                    SubData(TORIATUKAI2, 5).Trim,
                    SEPARATE,
                    SubData(NOUZEI_RENDO_FURIKAE_C, 3).Trim,
                    SEPARATE,
                    SubData(AITE_TEKIYOU, 16).Trim,
                    SEPARATE,
                    SubData(KISANBI, 7).Trim,
                    SEPARATE,
                    SubData(SAKIHIDUKE_YOTEI, 7).Trim,
                    SEPARATE
                    })

                'Return String.Concat(New String() _
                '    { _
                '    SubData(KOUZA_NO, 7).Trim, _
                '    SubData(KINGAKU, 10).Trim, _
                '    SubData(FURI_CODE, 3).Trim, _
                '    SubData(KIGYO_CODE, 3).Trim, _
                '    SubData(TEKIYOU, 16).Trim, _
                '    SubData(TORIATUKAI1, 5).Trim, _
                '    SubData(KENSU, 4).Trim, _
                '    SubData(TEGATA_NO, 6).Trim, _
                '    SubData(RENDO_TEN, 3).Trim, _
                '    SubData(RENDO_KAMOKU, 9).Trim, _
                '    SubData(TORIATUKAI2, 5).Trim, _
                '    SubData(AITE_TEKIYOU, 16).Trim, _
                '    SubData(KISANBI, 6).Trim, _
                '    SubData(SAKIHIDUKE_YOTEI, 6).Trim _
                '    })
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '���z���g���A��ƃR�[�h���g���A�[�ŘA���U�ւb�ǉ�
                KOUZA_NO = CuttingData(Value, 7)
                KINGAKU = CuttingData(Value, 13)
                FURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                TEKIYOU = CuttingData(Value, 16)
                TORIATUKAI1 = CuttingData(Value, 5)
                KENSU = CuttingData(Value, 4)
                TEGATA_NO = CuttingData(Value, 6)
                RENDO_TEN = CuttingData(Value, 3)
                RENDO_KAMOKU = CuttingData(Value, 9)
                TORIATUKAI2 = CuttingData(Value, 5)
                NOUZEI_RENDO_FURIKAE_C = CuttingData(Value, 3)
                AITE_TEKIYOU = CuttingData(Value, 16)
                'KISANBI = CuttingData(Value, 6)
                KISANBI = CuttingData(Value, 7)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                SAKIHIDUKE_YOTEI = CuttingData(Value, 7)

                'KOUZA_NO = CuttingData(Value, 7)
                'KINGAKU = CuttingData(Value, 10)
                'FURI_CODE = CuttingData(Value, 3)
                'KIGYO_CODE = CuttingData(Value, 3)
                'TEKIYOU = CuttingData(Value, 16)
                'TORIATUKAI1 = CuttingData(Value, 5)
                'KENSU = CuttingData(Value, 4)
                'TEGATA_NO = CuttingData(Value, 6)
                'RENDO_TEN = CuttingData(Value, 3)
                'RENDO_KAMOKU = CuttingData(Value, 9)
                'TORIATUKAI2 = CuttingData(Value, 5)
                'AITE_TEKIYOU = CuttingData(Value, 16)
                'KISANBI = CuttingData(Value, 6)
                'SAKIHIDUKE_YOTEI = CuttingData(Value, 6)
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Set
        End Property
    End Structure
#End Region

#Region "10-004 ���U�_��ݒ�ύX"

    '---------------------------------------------------------
    '2018/01/11 FJH����
    '�_����F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 291��290
    '---------------------------------------------------------

    Public Structure T_10004
        Implements ClsFormSikinFuri.IFormat

        Dim KAMOKU_CODE As String       ' �ȖڃR�[�h
        Dim OPE_CODE As String          ' �I�y�R�[�h
        Dim KAMOKU_KOUZA_NO As String   ' �Ȗڌ����ԍ�
        Dim GYO As String               ' �s
        Dim JIFURI_CODE As String       ' ���U�R�[�h
        Dim KIGYO_CODE As String        ' ��ƃR�[�h      
        Dim KEIYAKU_DATE As String      ' �_���
        Dim KOUFURIUKETUKE As String    ' ���U��t�T�[�r�X
        Dim GENTEN_NO As String         ' ���_�ԍ�
        Dim YOBI1 As String             ' �\���P

        ' ������
        Public Sub Init() Implements ClsFormSikinFuri.IFormat.Init
            KAMOKU_CODE = "10"
            OPE_CODE = "004"
            GYO = "01"
        End Sub

        Public Property Data() As String Implements ClsFormSikinFuri.IFormat.Data
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KAMOKU_KOUZA_NO, 9), _
                    SubData(GYO, 2), _
                    SubData(JIFURI_CODE, 3), _
                    SubData(KIGYO_CODE, 5), _
                    SubData(KEIYAKU_DATE, 7), _
                    SubData(KOUFURIUKETUKE, 1), _
                    SubData(GENTEN_NO, 3), _
                    SubData(YOBI1, 290) _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KAMOKU_KOUZA_NO = CuttingData(Value, 9)
                GYO = CuttingData(Value, 2)
                JIFURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                'KEIYAKU_DATE = CuttingData(Value, 6)
                KEIYAKU_DATE = CuttingData(Value, 7)
                KOUFURIUKETUKE = CuttingData(Value, 1)
                GENTEN_NO = CuttingData(Value, 3)
                'YOBI1 = CuttingData(Value, 291)
                YOBI1 = CuttingData(Value, 290)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements ClsFormSikinFuri.IFormat.DataSepaPlus
            ' �f�[�^�擾
            Get
                Return String.Concat(New String() _
                    { _
                    SubData(KAMOKU_KOUZA_NO, 9).Trim, _
                    SEPARATE, _
                    SubData(GYO, 2).Trim, _
                    SEPARATE, _
                    SubData(JIFURI_CODE, 3).Trim, _
                    SEPARATE, _
                    SubData(KIGYO_CODE, 5).Trim, _
                    SEPARATE, _
                    SubData(KEIYAKU_DATE, 7).Trim, _
                    SEPARATE, _
                    SubData(KOUFURIUKETUKE, 1).Trim, _
                    SEPARATE, _
                    SubData(GENTEN_NO, 3).Trim, _
                    SEPARATE _
                    })
            End Get

            ' �f�[�^�ݒ�
            Set(ByVal Value As String)
                KAMOKU_KOUZA_NO = CuttingData(Value, 9)
                GYO = CuttingData(Value, 2)
                JIFURI_CODE = CuttingData(Value, 3)
                KIGYO_CODE = CuttingData(Value, 5)
                'KEIYAKU_DATE = CuttingData(Value, 6)
                KEIYAKU_DATE = CuttingData(Value, 7)
                KOUFURIUKETUKE = CuttingData(Value, 1)
                GENTEN_NO = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
#End Region

#Region "02-499 �U�֎x���i�m�a�j"

    '---------------------------------------------------------
    '2018/05/10 �^�X�N�֓�
    '�N�Z���A����t�\����F�����O�X�ύX 6��7
    '---------------------------------------------------------

    Public Structure T_02499
        Implements ClsFormSikinFuri.IFormat

        ''' <summary> �ȖڃR�[�h </summary>
        Dim KAMOKU_CODE As String
        ''' <summary> �I�y�R�[�h </summary>
        Dim OPE_CODE As String
        ''' <summary> �����ԍ� </summary>
        Dim KOUZA_NO As String
        ''' <summary> �s </summary>
        Dim GYO As String
        ''' <summary> ���z </summary>
        Dim KINGAKU As String
        ''' <summary> �A���X�܃R�[�h </summary>
        Dim RENDO_TEN As String
        ''' <summary> �A���Ȗڌ����ԍ� </summary>
        Dim RENDO_KAMOKU_KOUZA As String
        ''' <summary> �U�փR�[�h </summary>
        Dim FURI_CODE As String
        ''' <summary> ��ƃR�[�h </summary>
        Dim KIGYO_CODE As String
        ''' <summary> �E�v </summary>
        Dim TEKIYO As String
        ''' <summary> �N�Z�� </summary>
        Dim KISANBI As String
        ''' <summary> ����t�\��� </summary>
        Dim SAKIHIDUKE_YOTEIBI As String

        Public Property Data() As String Implements IFormat.Data
            Get
                Return String.Concat(New String() _
                                     {
                                     SubData(KOUZA_NO, 7),
                                     SubData(GYO, 2),
                                     SubData(KINGAKU, 13),
                                     SubData(RENDO_TEN, 3),
                                     SubData(RENDO_KAMOKU_KOUZA, 9),
                                     SubData(FURI_CODE, 3),
                                     SubData(KIGYO_CODE, 5),
                                     SubData(TEKIYO, 13),
                                     SubData(KISANBI, 7),
                                     SubData(SAKIHIDUKE_YOTEIBI, 7)
                                     })
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                GYO = CuttingData(value, 2)
                KINGAKU = CuttingData(value, 13)
                RENDO_TEN = CuttingData(value, 3)
                RENDO_KAMOKU_KOUZA = CuttingData(value, 9)
                FURI_CODE = CuttingData(value, 3)
                KIGYO_CODE = CuttingData(value, 5)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements IFormat.DataSepaPlus
            Get
                Return String.Concat(New String() _
                                     {
                                     SubData(KOUZA_NO, 7).Trim,
                                     SEPARATE,
                                     SubData(GYO, 2).Trim,
                                     SEPARATE,
                                     SubData(KINGAKU, 13).Trim,
                                     SEPARATE,
                                     SubData(RENDO_TEN, 3).Trim,
                                     SEPARATE,
                                     SubData(RENDO_KAMOKU_KOUZA, 9).Trim,
                                     SEPARATE,
                                     SubData(FURI_CODE, 3).Trim,
                                     SEPARATE,
                                     SubData(KIGYO_CODE, 5).Trim,
                                     SEPARATE,
                                     SubData(TEKIYO, 13),
                                     SEPARATE,
                                     SubData(KISANBI, 7).Trim,
                                     SEPARATE,
                                     SubData(SAKIHIDUKE_YOTEIBI, 7).Trim,
                                     SEPARATE
                                     })
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                GYO = CuttingData(value, 2)
                KINGAKU = CuttingData(value, 13)
                RENDO_TEN = CuttingData(value, 3)
                RENDO_KAMOKU_KOUZA = CuttingData(value, 9)
                FURI_CODE = CuttingData(value, 3)
                KIGYO_CODE = CuttingData(value, 5)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
            End Set
        End Property

        Public Sub Init() Implements IFormat.Init
            KAMOKU_CODE = "02"
            OPE_CODE = "499"
            GYO = "01"
        End Sub
    End Structure
#End Region

#Region "01-490 �U�֘A���x��"

    '---------------------------------------------------------
    '2018/05/10 �^�X�N�֓�
    '�N�Z���A����t�\����F�����O�X�ύX 6��7
    '---------------------------------------------------------

    Public Structure T_01490
        Implements ClsFormSikinFuri.IFormat

        ''' <summary> �ȖڃR�[�h </summary>
        Dim KAMOKU_CODE As String
        ''' <summary> �I�y�R�[�h </summary>
        Dim OPE_CODE As String
        ''' <summary> �����ԍ� </summary>
        Dim KOUZA_NO As String
        ''' <summary> ��`�E���؎�ԍ� </summary>
        Dim TEGATA_KOGITTE_NO As String
        ''' <summary> ���z </summary>
        Dim KINGAKU As String
        ''' <summary> ��`���؎��� </summary>
        Dim TEGATA_KOGITTE_SYURUI As String
        ''' <summary> �A���X�܃R�[�h </summary>
        Dim RENDO_TEN As String
        ''' <summary> �U�֐�Ȗڌ����ԍ� </summary>
        Dim FURI_KAMOKU_KOUZA As String
        ''' <summary> �U�փR�[�h </summary>
        Dim FURI_CODE As String
        ''' <summary> ��ƃR�[�h </summary>
        Dim KIGYO_CODE As String
        ''' <summary> �E�v </summary>
        Dim TEKIYO As String
        ''' <summary> �N�Z�� </summary>
        Dim KISANBI As String
        ''' <summary> ����t�\��� </summary>
        Dim SAKIHIDUKE_YOTEIBI As String

        Public Property Data() As String Implements IFormat.Data
            Get
                Return String.Concat(New String() _
                                     {
                                     SubData(KOUZA_NO, 7),
                                     SubData(TEGATA_KOGITTE_NO, 6),
                                     SubData(KINGAKU, 13),
                                     SubData(TEGATA_KOGITTE_SYURUI, 1),
                                     SubData(RENDO_TEN, 3),
                                     SubData(FURI_KAMOKU_KOUZA, 9),
                                     SubData(FURI_CODE, 3),
                                     SubData(KIGYO_CODE, 5),
                                     SubData(TEKIYO, 13),
                                     SubData(KISANBI, 7),
                                     SubData(SAKIHIDUKE_YOTEIBI, 7)
                                     })
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                TEGATA_KOGITTE_NO = CuttingData(value, 6)
                KINGAKU = CuttingData(value, 13)
                TEGATA_KOGITTE_SYURUI = CuttingData(value, 1)
                RENDO_TEN = CuttingData(value, 3)
                FURI_KAMOKU_KOUZA = CuttingData(value, 9)
                FURI_CODE = CuttingData(value, 3)
                KIGYO_CODE = CuttingData(value, 5)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements IFormat.DataSepaPlus
            Get
                Return String.Concat(New String() _
                                     {
                                     SubData(KOUZA_NO, 7).Trim,
                                     SEPARATE,
                                     SubData(TEGATA_KOGITTE_NO, 6).Trim,
                                     SEPARATE,
                                     SubData(KINGAKU, 13).Trim,
                                     SEPARATE,
                                     SubData(TEGATA_KOGITTE_SYURUI, 1).Trim,
                                     SEPARATE,
                                     SubData(RENDO_TEN, 3).Trim,
                                     SEPARATE,
                                     SubData(FURI_KAMOKU_KOUZA, 9).Trim,
                                     SEPARATE,
                                     SubData(FURI_CODE, 3).Trim,
                                     SEPARATE,
                                     SubData(KIGYO_CODE, 5).Trim,
                                     SEPARATE,
                                     SubData(TEKIYO, 13),
                                     SEPARATE,
                                     SubData(KISANBI, 7).Trim,
                                     SEPARATE,
                                     SubData(SAKIHIDUKE_YOTEIBI, 7).Trim,
                                     SEPARATE
                                     })
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                TEGATA_KOGITTE_NO = CuttingData(value, 6)
                KINGAKU = CuttingData(value, 13)
                TEGATA_KOGITTE_SYURUI = CuttingData(value, 1)
                RENDO_TEN = CuttingData(value, 3)
                FURI_KAMOKU_KOUZA = CuttingData(value, 9)
                FURI_CODE = CuttingData(value, 3)
                KIGYO_CODE = CuttingData(value, 5)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
            End Set
        End Property

        Public Sub Init() Implements IFormat.Init
            KAMOKU_CODE = "01"
            OPE_CODE = "490"
        End Sub
    End Structure
#End Region

#Region "01-090 �����x��"

    '---------------------------------------------------------
    '2018/05/10 �^�X�N�֓�
    '�N�Z���A����t�\����F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 322��320
    '---------------------------------------------------------

    Public Structure T_01090
        Implements ClsFormSikinFuri.IFormat

        ''' <summary> �ȖڃR�[�h </summary>
        Dim KAMOKU_CODE As String
        ''' <summary> �I�y�R�[�h </summary>
        Dim OPE_CODE As String
        ''' <summary> �����ԍ� </summary>
        Dim KOUZA_NO As String
        ''' <summary> ��`�E���؎�ԍ� </summary>
        Dim TEGATA As String
        ''' <summary> ���z </summary>
        Dim KINGAKU As String
        ''' <summary> ��`���؎��� </summary>
        Dim TEGATA_S As String
        ''' <summary> �U�փR�[�h </summary>
        Dim FURI_CODE As String
        ''' <summary> �E�v </summary>
        Dim TEKIYO As String
        ''' <summary> �N�Z�� </summary>
        Dim KISANBI As String
        ''' <summary> ����t�\��� </summary>
        Dim SAKIHIDUKE_YOTEIBI As String
        ''' <summary> �d�q���ԍ� </summary>
        Dim DENSHISAIKEN As String
        ''' <summary> ���X�ԍ� </summary>
        Dim GENTEN_NO As String
        ''' <summary> �\���P </summary>
        Dim YOBI1 As String

        Public Property Data() As String Implements IFormat.Data
            Get
                Return String.Concat(New String() _
                                     {
                                     SubData(KOUZA_NO, 7),
                                     SubData(TEGATA, 6),
                                     SubData(KINGAKU, 13),
                                     SubData(TEGATA_S, 1),
                                     SubData(FURI_CODE, 3),
                                     SubData(TEKIYO, 13),
                                     SubData(KISANBI, 7),
                                     SubData(SAKIHIDUKE_YOTEIBI, 7),
                                     SubData(DENSHISAIKEN, 20),
                                     SubData(GENTEN_NO, 3),
                                     SubData(YOBI1, 320)
                                     })
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                TEGATA = CuttingData(value, 6)
                KINGAKU = CuttingData(value, 13)
                TEGATA_S = CuttingData(value, 1)
                FURI_CODE = CuttingData(value, 3)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
                DENSHISAIKEN = CuttingData(value, 20)
                GENTEN_NO = CuttingData(value, 3)
                'YOBI1 = CuttingData(value, 322)
                YOBI1 = CuttingData(value, 320)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements IFormat.DataSepaPlus
            Get
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '�\���͕s�v
                Return String.Concat(New String() _
                                     {
                                     SubData(KOUZA_NO, 7).Trim,
                                     SEPARATE,
                                     SubData(TEGATA, 6).Trim,
                                     SEPARATE,
                                     SubData(KINGAKU, 13).Trim,
                                     SEPARATE,
                                     SubData(TEGATA_S, 1).Trim,
                                     SEPARATE,
                                     SubData(FURI_CODE, 3).Trim,
                                     SEPARATE,
                                     SubData(TEKIYO, 13).Trim,
                                     SEPARATE,
                                     SubData(KISANBI, 7).Trim,
                                     SEPARATE,
                                     SubData(DENSHISAIKEN, 20).Trim,
                                     SEPARATE,
                                     SubData(SAKIHIDUKE_YOTEIBI, 7).Trim,
                                     SEPARATE,
                                     SubData(GENTEN_NO, 3).Trim,
                                     SEPARATE
                                     })

                'Return String.Concat(New String() _
                '                     { _
                '                     SubData(KOUZA_NO, 7).Trim, _
                '                     SEPARATE, _
                '                     SubData(TEGATA, 6).Trim, _
                '                     SEPARATE, _
                '                     SubData(KINGAKU, 13).Trim, _
                '                     SEPARATE, _
                '                     SubData(TEGATA_S, 1).Trim, _
                '                     SEPARATE, _
                '                     SubData(FURI_CODE, 3).Trim, _
                '                     SEPARATE, _
                '                     SubData(TEKIYO, 13), _
                '                     SEPARATE, _
                '                     SubData(KISANBI, 6).Trim, _
                '                     SEPARATE, _
                '                     SubData(SAKIHIDUKE_YOTEIBI, 6).Trim, _
                '                     SEPARATE, _
                '                     SubData(DENSHISAIKEN, 20).Trim, _
                '                     SEPARATE, _
                '                     SubData(GENTEN_NO, 3).Trim, _
                '                     SEPARATE, _
                '                     SubData(YOBI1, 322).Trim, _
                '                     SEPARATE _
                '                     })
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                TEGATA = CuttingData(value, 6)
                KINGAKU = CuttingData(value, 13)
                TEGATA_S = CuttingData(value, 1)
                FURI_CODE = CuttingData(value, 3)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
                DENSHISAIKEN = CuttingData(value, 20)
                GENTEN_NO = CuttingData(value, 3)
                YOBI1 = CuttingData(value, 320)
                '2018/04/06 saitou �L���M��(RSV2�W��) DEL �t�H�[�}�b�g�C�� ---------------------------------------- START
                'YOBI1 = CuttingData(value, 322)
                '2018/04/06 saitou �L���M��(RSV2�W��) DEL --------------------------------------------------------- END
            End Set
        End Property

        Public Sub Init() Implements IFormat.Init
            KAMOKU_CODE = "01"
            OPE_CODE = "090"
        End Sub
    End Structure


#End Region

#Region "02-099 ���ʎx��"

    '---------------------------------------------------------
    '2018/05/10 �^�X�N�֓�
    '�N�Z���A����t�\����F�����O�X�ύX 6��7
    '�\���P�F�����O�X�ύX 346��344
    '---------------------------------------------------------

    Public Structure T_02099
        Implements ClsFormSikinFuri.IFormat

        ''' <summary> �ȖڃR�[�h </summary>
        Dim KAMOKU_CODE As String
        ''' <summary> �I�y�R�[�h </summary>
        Dim OPE_CODE As String
        ''' <summary> �����ԍ� </summary>
        Dim KOUZA_NO As String
        ''' <summary> �s </summary>
        Dim GYO As String
        ''' <summary> ���z </summary>
        Dim KINGAKU As String
        ''' <summary> �U�փR�[�h </summary>
        Dim FURI_CODE As String
        ''' <summary> ���u�萔���s�v </summary>
        Dim SUEOKI As String
        ''' <summary> �E�v </summary>
        Dim TEKIYO As String
        ''' <summary> �N�Z�� </summary>
        Dim KISANBI As String
        ''' <summary> �d�q���ԍ� </summary>
        Dim DENSHISAIKEN As String
        ''' <summary> ����t�\��� </summary>
        Dim SAKIHIDUKE_YOTEIBI As String
        ''' <summary> ���X�ԍ� </summary>
        Dim GENTEN_NO As String
        ''' <summary> �\���P </summary>
        Dim YOBI1 As String

        Public Property Data() As String Implements IFormat.Data
            Get
                Return String.Concat(New String() _
                                     { _
                                     SubData(KOUZA_NO, 7), _
                                     SubData(GYO, 2), _
                                     SubData(KINGAKU, 13), _
                                     SubData(FURI_CODE, 3), _
                                     SubData(SUEOKI, 1), _
                                     SubData(TEKIYO, 13), _
                                     SubData(KISANBI, 7), _
                                     SubData(DENSHISAIKEN, 20), _
                                     SubData(SAKIHIDUKE_YOTEIBI, 7), _
                                     SubData(GENTEN_NO, 3), _
                                     SubData(YOBI1, 324) _
                                 })
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                GYO = CuttingData(value, 2)
                KINGAKU = CuttingData(value, 13)
                FURI_CODE = CuttingData(value, 3)
                SUEOKI = CuttingData(value, 1)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                DENSHISAIKEN = CuttingData(value, 20)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
                GENTEN_NO = CuttingData(value, 3)
                'YOBI1 = CuttingData(value, 346)
                YOBI1 = CuttingData(value, 324)
            End Set
        End Property

        Public Property DataSepaPlus() As String Implements IFormat.DataSepaPlus
            Get
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD �t�H�[�}�b�g�C�� ---------------------------------------- START
                '�\���͕s�v
                Return String.Concat(New String() _
                                     {
                                     SubData(KOUZA_NO, 7).Trim,
                                     SEPARATE,
                                     SubData(GYO, 2).Trim,
                                     SEPARATE,
                                     SubData(KINGAKU, 13).Trim,
                                     SEPARATE,
                                     SubData(FURI_CODE, 3).Trim,
                                     SEPARATE,
                                     SubData(SUEOKI, 1).Trim,
                                     SEPARATE,
                                     SubData(TEKIYO, 13).Trim,
                                     SEPARATE,
                                     SubData(KISANBI, 7).Trim,
                                     SEPARATE,
                                     SubData(DENSHISAIKEN, 20).Trim,
                                     SEPARATE,
                                     SubData(SAKIHIDUKE_YOTEIBI, 7).Trim,
                                     SEPARATE,
                                     SubData(GENTEN_NO, 3).Trim,
                                     SEPARATE
                                     })

                'Return String.Concat(New String() _
                '                     { _
                '                     SubData(KOUZA_NO, 7).Trim, _
                '                     SEPARATE, _
                '                     SubData(GYO, 2).Trim, _
                '                     SEPARATE, _
                '                     SubData(KINGAKU, 13).Trim, _
                '                     SEPARATE, _
                '                     SubData(FURI_CODE, 3).Trim, _
                '                     SEPARATE, _
                '                     SubData(SUEOKI, 1).Trim, _
                '                     SEPARATE, _
                '                     SubData(TEKIYO, 13), _
                '                     SEPARATE, _
                '                     SubData(KISANBI, 6).Trim, _
                '                     SEPARATE, _
                '                     SubData(DENSHISAIKEN, 20).Trim, _
                '                     SEPARATE, _
                '                     SubData(SAKIHIDUKE_YOTEIBI, 6).Trim, _
                '                     SEPARATE, _
                '                     SubData(GENTEN_NO, 3).Trim, _
                '                     SEPARATE, _
                '                     SubData(YOBI1, 326).Trim, _
                '                     SEPARATE _
                '                     })
                '2018/04/06 saitou �L���M��(RSV2�W��) UPD --------------------------------------------------------- END
            End Get
            Set(ByVal value As String)
                KOUZA_NO = CuttingData(value, 7)
                GYO = CuttingData(value, 2)
                KINGAKU = CuttingData(value, 13)
                FURI_CODE = CuttingData(value, 3)
                SUEOKI = CuttingData(value, 1)
                TEKIYO = CuttingData(value, 13)
                'KISANBI = CuttingData(value, 6)
                KISANBI = CuttingData(value, 7)
                DENSHISAIKEN = CuttingData(value, 20)
                'SAKIHIDUKE_YOTEIBI = CuttingData(value, 6)
                SAKIHIDUKE_YOTEIBI = CuttingData(value, 7)
                GENTEN_NO = CuttingData(value, 3)
                'YOBI1 = CuttingData(value, 326)
                YOBI1 = CuttingData(value, 324)
            End Set
        End Property

        Public Sub Init() Implements IFormat.Init
            KAMOKU_CODE = "02"
            OPE_CODE = "099"
        End Sub
    End Structure


#End Region

End Class
