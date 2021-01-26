Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' ���񂫂�t�H�[�}�b�g�i���[�N�j�N���X
Public Class CFormatShinkinWork580
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 580

    ' ------------------------------------------
    ' ���񂫂�t�H�[�}�b�g
    ' ------------------------------------------
    '----------------
    '�w�b�_�[���R�[�h
    '----------------
    Structure JF_Record1
        Implements CFormat.IFormat

        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(12)> Public TC As String      '�����R�[�h
        '<VBFixedString(9)> Public TC As String      '�����R�[�h
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        <VBFixedString(28)> Public FKN As String    '�U�֊�Ɩ�     '2007/10/03 mitsu 28���ɕύX
        <VBFixedString(15)> Public KNM As String    '�_��Ҏ���
        <VBFixedString(4)> Public JF1 As String     '���ɃR�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(4)> Public JF4 As String     '�����݋��ɃR�[�h
        <VBFixedString(1)> Public JF5 As String     '�Ԋҋ敪
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(123)> Public JF6 As String   '�\��
        <VBFixedString(389)> Public JF400 As String '�\��
        '<VBFixedString(115)> Public JF6 As String   '�\��           '2007/10/03 mitsu 115���ɕύX
        '<VBFixedString(400)> Public JF400 As String   '�\��
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                Return String.Concat(New String() _
                                   { _
                                    SubData(TC, 12), _
                                    SubData(FKN, 28), _
                                    SubData(KNM, 15), _
                                    SubData(JF1, 4), _
                                    SubData(JF2, 3), _
                                    SubData(JF3, 1), _
                                    SubData(JF4, 4), _
                                    SubData(JF5, 1), _
                                    SubData(JF6, 123), _
                                    SubData(JF400, 389) _
                                   })
                'Return String.Concat(New String() _
                '                   { _
                '                    SubData(TC, 9), _
                '                    SubData(FKN, 28), _
                '                    SubData(KNM, 15), _
                '                    SubData(JF1, 4), _
                '                    SubData(JF2, 3), _
                '                    SubData(JF3, 1), _
                '                    SubData(JF4, 4), _
                '                    SubData(JF5, 1), _
                '                    SubData(JF6, 115), _
                '                    SubData(JF400, 400) _
                '                   })
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            End Get
            Set(ByVal value As String)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                TC = CuttingData(value, 12)
                'TC = CuttingData(value, 9)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
                FKN = CuttingData(value, 28)
                KNM = CuttingData(value, 15)
                JF1 = CuttingData(value, 4)
                JF2 = CuttingData(value, 3)
                JF3 = CuttingData(value, 1)
                JF4 = CuttingData(value, 4)
                JF5 = CuttingData(value, 1)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                JF6 = CuttingData(value, 123)
                JF400 = CuttingData(value, 389)
                'JF6 = CuttingData(value, 115)
                'JF400 = CuttingData(value, 400)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            End Set
        End Property
    End Structure
    Public JF_DATA1 As JF_Record1

    '--------------
    '�f�[�^���R�[�h
    '--------------
    Structure JF_Record2
        Implements CFormat.IFormat

        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(12)> Public TC As String      '�����R�[�h
        '<VBFixedString(9)> Public TC As String      '�����R�[�h
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        <VBFixedString(30)> Public FKN As String    '�U�֊�Ɩ�
        <VBFixedString(15)> Public KNM As String    '�_��Ҏ���
        <VBFixedString(4)> Public JF1 As String     '���ɃR�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(2)> Public JF4 As String     '�ȖڃR�[�h
        <VBFixedString(7)> Public JF5 As String     '�����ԍ�
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(8)> Public JF6 As String     '���U�w���
        <VBFixedString(13)> Public JF7 As String    '���z
        '<VBFixedString(6)> Public JF6 As String     '���U�w���
        '<VBFixedString(10)> Public JF7 As String    '���z
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        <VBFixedString(1)> Public JF8 As String     '���o���敪
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(5)> Public JF9 As String     '��ƃR�[�h
        <VBFixedString(8)> Public JF10 As String    '��ƃV�[�P���X
        '<VBFixedString(4)> Public JF9 As String     '��ƃR�[�h
        '<VBFixedString(7)> Public JF10 As String    '��ƃV�[�P���X
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        <VBFixedString(2)> Public JF11 As String    '�������D��R�[�h
        <VBFixedString(3)> Public JF12 As String    '�U�փR�[�h
        <VBFixedString(2)> Public JF13 As String    '�U�֑���Ȗ�
        <VBFixedString(7)> Public JF14 As String    '�U�֑������
        <VBFixedString(4)> Public JF15 As String    '�J�n�N��
        <VBFixedString(4)> Public JF16 As String    '�I���N��
        <VBFixedString(1)> Public JF17 As String    '�E�v�ݒ�敪
        <VBFixedString(13)> Public JF18 As String   '�J�i�E�v
        <VBFixedString(12)> Public JF19 As String   '�����E�v
        <VBFixedString(24)> Public JF20 As String   '���v�Ɣԍ�
        <VBFixedString(7)> Public JF21 As String    '����ڋq�ԍ�
        <VBFixedString(1)> Public JF22 As String    '�x���w��R�[�h
        <VBFixedString(1)> Public JF23 As String    '�\��
        <VBFixedString(2)> Public FMT As String     '�t�H�[�}�b�g�敪
        <VBFixedString(3)> Public YOBI1 As String   '�����}�X�^ �\���P�i�E�v�R�[�h�j
        <VBFixedString(3)> Public YOBI2 As String   '�����}�X�^ �\���Q�i����R�[�h�j
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(6)> Public RECORDNO As String   '���R�[�h�ԍ�
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(376)> Public JF400 As String '�\��
        '<VBFixedString(392)> Public JF400 As String '�\��
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                Return String.Concat(New String() _
                                      { _
                                          SubData(TC, 12), _
                                          SubData(FKN, 30), _
                                          SubData(KNM, 15), _
                                          SubData(JF1, 4), _
                                          SubData(JF2, 3), _
                                          SubData(JF3, 1), _
                                          SubData(JF4, 2), _
                                          SubData(JF5, 7), _
                                          SubData(JF6, 8), _
                                          SubData(JF7, 13), _
                                          SubData(JF8, 1), _
                                          SubData(JF9, 5), _
                                          SubData(JF10, 8), _
                                          SubData(JF11, 2), _
                                          SubData(JF12, 3), _
                                          SubData(JF13, 2), _
                                          SubData(JF14, 7), _
                                          SubData(JF15, 4), _
                                          SubData(JF16, 4), _
                                          SubData(JF17, 1), _
                                          SubData(JF18, 13), _
                                          SubData(JF19, 12), _
                                          SubData(JF20, 24), _
                                          SubData(JF21, 7), _
                                          SubData(JF22, 1), _
                                          SubData(JF23, 1), _
                                          SubData(FMT, 2), _
                                          SubData(YOBI1, 3), _
                                          SubData(YOBI2, 3), _
                                          SubData(RECORDNO, 6), _
                                          SubData(JF400, 376) _
                                      })
                'Return String.Concat(New String() _
                '                      { _
                '                          SubData(TC, 9), _
                '                          SubData(FKN, 30), _
                '                          SubData(KNM, 15), _
                '                          SubData(JF1, 4), _
                '                          SubData(JF2, 3), _
                '                          SubData(JF3, 1), _
                '                          SubData(JF4, 2), _
                '                          SubData(JF5, 7), _
                '                          SubData(JF6, 6), _
                '                          SubData(JF7, 10), _
                '                          SubData(JF8, 1), _
                '                          SubData(JF9, 4), _
                '                          SubData(JF10, 7), _
                '                          SubData(JF11, 2), _
                '                          SubData(JF12, 3), _
                '                          SubData(JF13, 2), _
                '                          SubData(JF14, 7), _
                '                          SubData(JF15, 4), _
                '                          SubData(JF16, 4), _
                '                          SubData(JF17, 1), _
                '                          SubData(JF18, 13), _
                '                          SubData(JF19, 12), _
                '                          SubData(JF20, 24), _
                '                          SubData(JF21, 7), _
                '                          SubData(JF22, 1), _
                '                          SubData(JF23, 1), _
                '                          SubData(FMT, 2), _
                '                          SubData(YOBI1, 3), _
                '                          SubData(YOBI2, 3), _
                '                          SubData(JF400, 392) _
                '                      })
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            End Get
            Set(ByVal Value As String)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                TC = CuttingData(Value, 12)
                'TC = CuttingData(Value, 9)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
                FKN = CuttingData(Value, 30)
                KNM = CuttingData(Value, 15)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 2)
                JF5 = CuttingData(Value, 7)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                JF6 = CuttingData(Value, 8)
                JF7 = CuttingData(Value, 13)
                'JF6 = CuttingData(Value, 6)
                'JF7 = CuttingData(Value, 10)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
                JF8 = CuttingData(Value, 1)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                JF9 = CuttingData(Value, 5)
                JF10 = CuttingData(Value, 8)
                'JF9 = CuttingData(Value, 4)
                'JF10 = CuttingData(Value, 7)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
                JF11 = CuttingData(Value, 2)
                JF12 = CuttingData(Value, 3)
                JF13 = CuttingData(Value, 2)
                JF14 = CuttingData(Value, 7)
                JF15 = CuttingData(Value, 4)
                JF16 = CuttingData(Value, 4)
                JF17 = CuttingData(Value, 1)
                JF18 = CuttingData(Value, 13)
                JF19 = CuttingData(Value, 12)
                JF20 = CuttingData(Value, 24)
                JF21 = CuttingData(Value, 7)
                JF22 = CuttingData(Value, 1)
                JF23 = CuttingData(Value, 1)
                FMT = CuttingData(Value, 2)
                YOBI1 = CuttingData(Value, 3)
                YOBI2 = CuttingData(Value, 3)
                '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                RECORDNO = CuttingData(Value, 6)
                '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                JF400 = CuttingData(Value, 376)
                'JF400 = CuttingData(Value, 392)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            End Set
        End Property
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        Public Sub INIT()
            TC = ""
            FKN = ""
            KNM = ""    '�_��Ҏ���
            JF1 = ""     '���ɃR�[�h
            JF2 = ""     '�X�܃R�[�h
            JF3 = ""     '���R�[�h���
            JF4 = ""     '�ȖڃR�[�h
            JF5 = ""     '�����ԍ�
            JF6 = ""     '���U�w���
            JF7 = ""    '���z
            JF8 = ""     '���o���敪
            JF9 = ""     '��ƃR�[�h
            JF10 = ""    '��ƃV�[�P���X
            JF11 = ""    '�������D��R�[�h
            JF12 = ""    '�U�փR�[�h
            JF13 = ""    '�U�֑���Ȗ�
            JF14 = ""    '�U�֑������
            JF15 = ""    '�J�n�N��
            JF16 = ""    '�I���N��
            JF17 = ""    '�E�v�ݒ�敪
            JF18 = ""   '�J�i�E�v
            JF19 = ""   '�����E�v
            JF20 = ""   '���v�Ɣԍ�
            JF21 = ""    '����ڋq�ԍ�
            JF22 = ""    '�x���w��R�[�h
            JF23 = ""    '�\��
            FMT = ""     '�t�H�[�}�b�g�敪
            YOBI1 = ""   '�����}�X�^ �\���P�i�E�v�R�[�h�j
            YOBI2 = ""   '�����}�X�^ �\���Q�i����R�[�h�j
            RECORDNO = ""   '���R�[�h�ԍ�
            JF400 = "" '�\��
        End Sub
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
    End Structure
    Public JF_DATA2 As JF_Record2

    '--------------
    '�G���h���R�[�h
    '--------------
    Structure JF_Record3
        Implements CFormat.IFormat

        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(12)> Public TC As String      '�����R�[�h
        '<VBFixedString(9)> Public TC As String      '�����R�[�h
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        <VBFixedString(30)> Public FKN As String    '�U�֊�Ɩ�
        <VBFixedString(15)> Public KNM As String    '�_��Ҏ���
        <VBFixedString(4)> Public JF1 As String     '���ɃR�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(118)> Public JF4 As String   '�\��
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        <VBFixedString(397)> Public JF400 As String '�\��
        '<VBFixedString(400)> Public JF400 As String '�\��
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                Return String.Concat(New String() _
                                { _
                                    SubData(TC, 12), _
                                    SubData(FKN, 30), _
                                    SubData(KNM, 15), _
                                    SubData(JF1, 4), _
                                    SubData(JF2, 3), _
                                    SubData(JF3, 1), _
                                    SubData(JF4, 118), _
                                    SubData(JF400, 397) _
                                })
                'Return String.Concat(New String() _
                '                { _
                '                    SubData(TC, 9), _
                '                    SubData(FKN, 30), _
                '                    SubData(KNM, 15), _
                '                    SubData(JF1, 4), _
                '                    SubData(JF2, 3), _
                '                    SubData(JF3, 1), _
                '                    SubData(JF4, 118), _
                '                    SubData(JF400, 400) _
                '                })
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            End Get
            Set(ByVal Value As String)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                TC = CuttingData(Value, 12)
                'TC = CuttingData(Value, 9)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
                FKN = CuttingData(Value, 30)
                KNM = CuttingData(Value, 15)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 118)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
                JF400 = CuttingData(Value, 397)
                'JF400 = CuttingData(Value, 400)
                '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
            End Set
        End Property
    End Structure
    Public JF_DATA3 As JF_Record3

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"0", "9"}

        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        FtranPfile = "136.P"    'P�t�@�C���͌��s�ɂ����݂��Ȃ��̂Ŗ��g�p���H
        'FtranPfile = "128.P"
        '2018/02/01 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END

        HeaderKubun = New String() {"0"}
        DataKubun = New String() {"1", "2", "3", "4"}
        TrailerKubun = New String() {"9"}
    End Sub

End Class
