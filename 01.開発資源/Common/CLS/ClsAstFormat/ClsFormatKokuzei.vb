Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' ���� �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatKokuzei
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 390

    '------------------------------------------
    '���Ńt�H�[�}�b�g
    '------------------------------------------
    '�t�@�C���擪���R�[�h�i�`�j
    Structure KOKUZEI_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=1)
        <VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪
        <VBFixedString(19)> Public KZ3 As String        '�_�~�[
        <VBFixedString(3)> Public KZ4 As String         '�ȖڃR�[�h
        <VBFixedString(2)> Public KZ5 As String         '�N�x
        <VBFixedString(2)> Public KZ6 As String         '�ېŔN��
        <VBFixedString(1)> Public KZ7 As String         '�[���敪
        <VBFixedString(7)> Public KZ8 As String         '�[���J�i����
        <VBFixedString(2)> Public KZ9 As String         '����敪
        <VBFixedString(6)> Public KZ10 As String        '�����N����
        <VBFixedString(6)> Public KZ11 As String        '�U�֓�
        <VBFixedString(6)> Public KZ12 As String        '�ېŊ��ԁi���j
        <VBFixedString(6)> Public KZ13 As String        '�ېŊ��ԁi���j
        <VBFixedString(325)> Public KZ14 As String      '�_�~�[
        <VBFixedString(2)> Public KZ15 As String        '�˗��t�@�C���m�n
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 19), _
                            SubData(KZ4, 3), _
                            SubData(KZ5, 2), _
                            SubData(KZ6, 2), _
                            SubData(KZ7, 1), _
                            SubData(KZ8, 7), _
                            SubData(KZ9, 2), _
                            SubData(KZ10, 6), _
                            SubData(KZ11, 6), _
                            SubData(KZ12, 6), _
                            SubData(KZ13, 6), _
                            SubData(KZ14, 325), _
                            SubData(KZ15, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 19)
                KZ4 = CuttingData(Value, 3)
                KZ5 = CuttingData(Value, 2)
                KZ6 = CuttingData(Value, 2)
                KZ7 = CuttingData(Value, 1)
                KZ8 = CuttingData(Value, 7)
                KZ9 = CuttingData(Value, 2)
                KZ10 = CuttingData(Value, 6)
                KZ11 = CuttingData(Value, 6).Replace(" "c, "0")
                KZ12 = CuttingData(Value, 6)
                KZ13 = CuttingData(Value, 6)
                KZ14 = CuttingData(Value, 325)
                KZ15 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure

    Public KOKUZEI_REC1 As KOKUZEI_RECORD1

    '���ʋ��Z�@�֓X�ܕʖ��̃��R�[�h�i�a�j
    '���ʋ��Z�@�֓X�ܕʃg�[�^�����R�[�h�i�c�j
    '���ʋ��Z�@�֕ʃg�[�^�����R�[�h�i�d�j
    Structure KOKUZEI_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=2)
        <VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪
        <VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
        <VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
        <VBFixedString(9)> Public KZ5 As String         '�_�~�[
        <VBFixedString(5)> Public KZ6 As String         '����R�[�h
        <VBFixedString(10)> Public KZ7 As String        '�Ŗ�����
        <VBFixedString(7)> Public KZ8 As String         '�_�~�[
        <VBFixedString(5)> Public KZ9 As String         '�Ŗ����X�֔ԍ�
        <VBFixedString(7)> Public KZ10 As String        '�戵���Z�@�֔ԍ�
        <VBFixedString(5)> Public KZ11 As String        '���Z�@�֗X�֔ԍ�
        <VBFixedString(7)> Public KZ12 As String        '�_�~�[
        <VBFixedString(6)> Public KZ13 As String        '���t������
        <VBFixedString(12)> Public KZ14 As String       '���t�����v���z
        <VBFixedString(6)> Public KZ15 As String        '�U�֔[�t�s�\����
        <VBFixedString(12)> Public KZ16 As String       '�U�֔[�t�s�\���v
        <VBFixedString(6)> Public KZ17 As String        '�U�֔[�t����
        <VBFixedString(12)> Public KZ18 As String       '�U�֔[�t���v���z
        <VBFixedString(5)> Public KZ19 As String        '�_�~�[
        <VBFixedString(8)> Public KZ20 As String        '�Ŗ����d�b�ԍ�
        <VBFixedString(8)> Public KZ21 As String        '���Z�@�֓d�b��
        <VBFixedString(27)> Public KZ22 As String       '�_�~�[
        <VBFixedString(30)> Public KZ23 As String       '�s�s�於
        <VBFixedString(30)> Public KZ24 As String       '���ݒn�T
        <VBFixedString(30)> Public KZ25 As String       '���ݒn�U
        <VBFixedString(30)> Public KZ26 As String       '����
        <VBFixedString(30)> Public KZ27 As String       '���Z�@�֖��̇T
        <VBFixedString(30)> Public KZ28 As String       '���Z�@�֖��̇U
        <VBFixedString(30)> Public KZ29 As String       '�X�ܖ���
        <VBFixedString(1)> Public KZ30 As String        '��[�L��
        <VBFixedString(5)> Public KZ31 As String        '�_�~�[
        <VBFixedString(2)> Public KZ32 As String        '�˗��t�@�C���m�n
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 9), _
                            SubData(KZ6, 5), _
                            SubData(KZ7, 10), _
                            SubData(KZ8, 7), _
                            SubData(KZ9, 5), _
                            SubData(KZ10, 7), _
                            SubData(KZ11, 5), _
                            SubData(KZ12, 7), _
                            SubData(KZ13, 6), _
                            SubData(KZ14, 12), _
                            SubData(KZ15, 6), _
                            SubData(KZ16, 12), _
                            SubData(KZ17, 6), _
                            SubData(KZ18, 12), _
                            SubData(KZ19, 5), _
                            SubData(KZ20, 8), _
                            SubData(KZ21, 8), _
                            SubData(KZ22, 27), _
                            SubData(KZ23, 30), _
                            SubData(KZ24, 30), _
                            SubData(KZ25, 30), _
                            SubData(KZ26, 30), _
                            SubData(KZ27, 30), _
                            SubData(KZ28, 30), _
                            SubData(KZ29, 30), _
                            SubData(KZ30, 1), _
                            SubData(KZ31, 5), _
                            SubData(KZ32, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 9)
                KZ6 = CuttingData(Value, 5)
                KZ7 = CuttingData(Value, 10)
                KZ8 = CuttingData(Value, 7)
                KZ9 = CuttingData(Value, 5)
                KZ10 = CuttingData(Value, 7)
                KZ11 = CuttingData(Value, 5)
                KZ12 = CuttingData(Value, 7)
                KZ13 = CuttingData(Value, 6)
                KZ14 = CuttingData(Value, 12)
                KZ15 = CuttingData(Value, 6)
                KZ16 = CuttingData(Value, 12)
                KZ17 = CuttingData(Value, 6)
                KZ18 = CuttingData(Value, 12)
                KZ19 = CuttingData(Value, 5)
                KZ20 = CuttingData(Value, 8)
                KZ21 = CuttingData(Value, 8)
                KZ22 = CuttingData(Value, 27)
                KZ23 = CuttingData(Value, 30)
                KZ24 = CuttingData(Value, 30)
                KZ25 = CuttingData(Value, 30)
                KZ26 = CuttingData(Value, 30)
                KZ27 = CuttingData(Value, 30)
                KZ28 = CuttingData(Value, 30)
                KZ29 = CuttingData(Value, 30)
                KZ30 = CuttingData(Value, 1)
                KZ31 = CuttingData(Value, 5)
                KZ32 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC2 As KOKUZEI_RECORD2

    '�ʖ��׃��R�[�h�i�b�j
    Structure KOKUZEI_RECORD3
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=3)
        <VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪(=91)
        <VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
        <VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
        <VBFixedString(7)> Public KZ5 As String         '�[�ŎҔԍ�
        <VBFixedString(1)> Public KZ6 As String         '�p���敪
        <VBFixedString(1)> Public KZ7 As String         '�⊮�\���敪
        <VBFixedString(1)> Public KZ8 As String         '�U�֌��ʃR�[�h
        <VBFixedString(10)> Public KZ9 As String        '�[�t�Ŋz
        <VBFixedString(9)> Public KZ10 As String        '�����q��
        <VBFixedString(1)> Public KZ11 As String        '�a�����
        <VBFixedString(7)> Public KZ12 As String        '�����ԍ�
        <VBFixedString(8)> Public KZ13 As String        '�����ԍ�
        <VBFixedString(69)> Public KZ14 As String       '�_�~�[
        <VBFixedString(7)> Public KZ15 As String        '�X�֔ԍ��i7���j
        <VBFixedString(5)> Public KZ16 As String        '�X�֔ԍ��i5���j
        <VBFixedString(1)> Public KZ17 As String        '�⊮�\��
        <VBFixedString(7)> Public KZ18 As String        '�戵���Z�@�֔ԍ�
        <VBFixedString(7)> Public KZ19 As String        '�_�~�[
        <VBFixedString(6)> Public KZ20 As String        '�s�O�ǔ�(�[�Ŏ�)
        <VBFixedString(8)> Public KZ21 As String        '�d�b�ԍ�(�[�Ŏ�)
        <VBFixedString(2)> Public KZ22 As String        '�_�~�[
        <VBFixedString(23)> Public KZ23 As String       '�s�s�敪
        <VBFixedString(23)> Public KZ24 As String       '�Z���T
        <VBFixedString(23)> Public KZ25 As String       '�Z���U
        <VBFixedString(23)> Public KZ26 As String       '�Z���V
        <VBFixedString(23)> Public KZ27 As String       '�����T
        <VBFixedString(23)> Public KZ28 As String       '�����U
        <VBFixedString(23)> Public KZ29 As String       '�����V
        <VBFixedString(23)> Public KZ30 As String       '�[�ŎҖ��T
        <VBFixedString(23)> Public KZ31 As String       '�[�ŎҖ��U
        <VBFixedString(5)> Public KZ32 As String        '�[���ԍ�
        <VBFixedString(3)> Public KZ33 As String        '�����ԍ�
        <VBFixedString(1)> Public KZ34 As String        '�p���敪
        <VBFixedString(2)> Public KZ35 As String        '�˗��t�@�C���m�n
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 7), _
                            SubData(KZ6, 1), _
                            SubData(KZ7, 1), _
                            SubData(KZ8, 1), _
                            SubData(KZ9, 10), _
                            SubData(KZ10, 9), _
                            SubData(KZ11, 1), _
                            SubData(KZ12, 7), _
                            SubData(KZ13, 8), _
                            SubData(KZ14, 69), _
                            SubData(KZ15, 7), _
                            SubData(KZ16, 5), _
                            SubData(KZ17, 1), _
                            SubData(KZ18, 7), _
                            SubData(KZ19, 7), _
                            SubData(KZ20, 6), _
                            SubData(KZ21, 8), _
                            SubData(KZ22, 2), _
                            SubData(KZ23, 23), _
                            SubData(KZ24, 23), _
                            SubData(KZ25, 23), _
                            SubData(KZ26, 23), _
                            SubData(KZ27, 23), _
                            SubData(KZ28, 23), _
                            SubData(KZ29, 23), _
                            SubData(KZ30, 23), _
                            SubData(KZ31, 23), _
                            SubData(KZ32, 5), _
                            SubData(KZ33, 3), _
                            SubData(KZ34, 1), _
                            SubData(KZ35, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 7)
                KZ6 = CuttingData(Value, 1)
                KZ7 = CuttingData(Value, 1)
                KZ8 = CuttingData(Value, 1)
                KZ9 = CuttingData(Value, 10)
                KZ10 = CuttingData(Value, 9)
                KZ11 = CuttingData(Value, 1)
                KZ12 = CuttingData(Value, 7)
                KZ13 = CuttingData(Value, 8)
                KZ14 = CuttingData(Value, 69)
                KZ15 = CuttingData(Value, 7)
                KZ16 = CuttingData(Value, 5)
                KZ17 = CuttingData(Value, 1)
                KZ18 = CuttingData(Value, 7)
                KZ19 = CuttingData(Value, 7)
                KZ20 = CuttingData(Value, 6)
                KZ21 = CuttingData(Value, 8)
                KZ22 = CuttingData(Value, 2)
                KZ23 = CuttingData(Value, 23)
                KZ24 = CuttingData(Value, 23)
                KZ25 = CuttingData(Value, 23)
                KZ26 = CuttingData(Value, 23)
                KZ27 = CuttingData(Value, 23)
                KZ28 = CuttingData(Value, 23)
                KZ29 = CuttingData(Value, 23)
                KZ30 = CuttingData(Value, 23)
                KZ31 = CuttingData(Value, 23)
                KZ32 = CuttingData(Value, 5)
                KZ33 = CuttingData(Value, 3)
                KZ34 = CuttingData(Value, 1)
                KZ35 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC3 As KOKUZEI_RECORD3

    '�t�@�C�����v���R�[�h�i�e�j
    Structure KOKUZEI_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=8)
        <VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪(=91)
        <VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
        <VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
        <VBFixedString(10)> Public KZ5 As String        '�_�~�[
        <VBFixedString(45)> Public KZ6 As String        '�_�~�[
        <VBFixedString(6)> Public KZ7 As String         '���t������
        <VBFixedString(12)> Public KZ8 As String        '���t�����v���z
        <VBFixedString(6)> Public KZ9 As String         '�U�֔[�t�s�\����
        <VBFixedString(12)> Public KZ10 As String       '�U�֔[�t�s�\���v���z
        <VBFixedString(6)> Public KZ11 As String        '�U�֔[�t����
        <VBFixedString(12)> Public KZ12 As String       '�U�֔[�t���v���z
        <VBFixedString(264)> Public KZ13 As String      '�_�~�[
        <VBFixedString(2)> Public KZ14 As String        '�˗��t�@�C���m�n
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 10), _
                            SubData(KZ6, 45), _
                            SubData(KZ7, 6), _
                            SubData(KZ8, 12), _
                            SubData(KZ9, 6), _
                            SubData(KZ10, 12), _
                            SubData(KZ11, 6), _
                            SubData(KZ12, 12), _
                            SubData(KZ13, 264), _
                            SubData(KZ14, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 10)
                KZ6 = CuttingData(Value, 45)
                KZ7 = CuttingData(Value, 6)
                KZ8 = CuttingData(Value, 12)
                KZ9 = CuttingData(Value, 6)
                KZ10 = CuttingData(Value, 12)
                KZ11 = CuttingData(Value, 6)
                KZ12 = CuttingData(Value, 12)
                KZ13 = CuttingData(Value, 264)
                KZ14 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC8 As KOKUZEI_RECORD8

    '�t�@�C���G���h���R�[�h�i�f�j
    Structure KOKUZEI_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public KZ1 As String         '�f�[�^�敪(=9)
        <VBFixedString(2)> Public KZ2 As String         '�t�@�C���敪(=91)
        <VBFixedString(5)> Public KZ3 As String         '�Ǐ��ԍ�
        <VBFixedString(7)> Public KZ4 As String         '�S�⋦����R�[�h
        <VBFixedString(10)> Public KZ5 As String        '�_�~�[
        <VBFixedString(45)> Public KZ6 As String        '�_�~�[
        <VBFixedString(6)> Public KZ7 As String         '���t������
        <VBFixedString(12)> Public KZ8 As String        '���t�����v���z
        <VBFixedString(6)> Public KZ9 As String         '�U�֔[�t�s�\����
        <VBFixedString(12)> Public KZ10 As String       '�U�֔[�t�s�\���z
        <VBFixedString(6)> Public KZ11 As String        '�U�֔[�t����
        <VBFixedString(12)> Public KZ12 As String       '�U�֔[�t���z
        <VBFixedString(264)> Public KZ13 As String      '�_�~�[
        <VBFixedString(2)> Public KZ14 As String        '�˗��t�@�C���m�n
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(KZ1, 1), _
                            SubData(KZ2, 2), _
                            SubData(KZ3, 5), _
                            SubData(KZ4, 7), _
                            SubData(KZ5, 10), _
                            SubData(KZ6, 45), _
                            SubData(KZ7, 6), _
                            SubData(KZ8, 12), _
                            SubData(KZ9, 6), _
                            SubData(KZ10, 12), _
                            SubData(KZ11, 6), _
                            SubData(KZ12, 12), _
                            SubData(KZ13, 264), _
                            SubData(KZ14, 2) _
                            })
            End Get
            Set(ByVal Value As String)
                KZ1 = CuttingData(Value, 1)
                KZ2 = CuttingData(Value, 2)
                KZ3 = CuttingData(Value, 5)
                KZ4 = CuttingData(Value, 7)
                KZ5 = CuttingData(Value, 10)
                KZ6 = CuttingData(Value, 45)
                KZ7 = CuttingData(Value, 6)
                KZ8 = CuttingData(Value, 12)
                KZ9 = CuttingData(Value, 6)
                KZ10 = CuttingData(Value, 12)
                KZ11 = CuttingData(Value, 6)
                KZ12 = CuttingData(Value, 12)
                KZ13 = CuttingData(Value, 264)
                KZ14 = CuttingData(Value, 2)
            End Set
        End Property
    End Structure
    Public KOKUZEI_REC9 As KOKUZEI_RECORD9

    Private TOTAL_SIT_NORM_KEN As Decimal = 0           ' ���Z�@�֓X�ܕ� �U�֔[�t����
    Private TOTAL_SIT_NORM_KIN As Decimal = 0           ' ���Z�@�֓X�ܕ� �U�֔[�t���z
    Private TOTAL_SIT_IJO_KEN As Decimal = 0            ' ���Z�@�֓X�ܕ� �U�֔[�t�s�\����
    Private TOTAL_SIT_IJO_KIN As Decimal = 0            ' ���Z�@�֓X�ܕ� �U�֔[�t�s�\���z
    Private TOTAL_KIN_NORM_KEN As Decimal = 0           ' ���Z�@�֕� �U�֔[�t����
    Private TOTAL_KIN_NORM_KIN As Decimal = 0           ' ���Z�@�֕� �U�֔[�t���z
    Private TOTAL_KIN_IJO_KEN As Decimal = 0            ' ���Z�@�֕� �U�֔[�t�s�\����
    Private TOTAL_KIN_IJO_KIN As Decimal = 0            ' ���Z�@�֕� �U�֔[�t�s�\���z

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "2", "3", "4", "5", "8", "9"}

        FtranPfile = "390.P"
        FtranIBMPfile = "390IBM.P"
        FtranIBMBinaryPfile = "390READ.P"

        CMTBlockSize = 3900

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2", "3", "4", "5"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' �@�\�@ �F ���R�[�h�`�F�b�N
    '
    ' �߂�l �F �s�������̈ʒu
    '
    ' ���l�@ �F
    '
    Public Overrides Function CheckRegularString() As Long
        Return MyBase.CheckRegularString()
    End Function

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F
    '
    Public Overrides Function CheckDataFormat() As String
        Dim ErrString As String = ""

        ' ��{�N���X �`�F�b�N
        Dim sRet As String = MyBase.CheckDataFormat()
        If sRet = "ERR" Then
            '' �K��O��������
            'Return "ERR"
            ErrString = DataInfo.Message
            If ErrString = "" Then
                ErrString = "ERR"
            End If
        End If

        If RecordData.Length = 0 Then
            DataInfo.Message = "�t�@�C���ُ�"
            mnErrorNumber = 1
            Return "ERR"
        End If

        Select Case RecordData.Substring(0, 1)
            Case "1"
                If BeforeRecKbn <> "" And BeforeRecKbn <> "8" And BeforeRecKbn <> "9" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�w�b�_�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord1()
                    '*** �C�� mitsu 2008/05/21 �w�b�_�Ɉϑ��ҏ�񂪂Ȃ����߃`�F�b�N���Ȃ� ***
                    'If sRet <> "ERR" Then
                    '    sRet = CheckDBRecord1()
                    'End If
                    '************************************************************************
                End If
            Case "2"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "4" And BeforeRecKbn <> "5" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�f�[�^�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord2()
                End If
            Case "3"
                If BeforeRecKbn <> "2" And BeforeRecKbn <> "3" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�f�[�^�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord3()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord3()
                    End If
                End If
            Case "4"
                If BeforeRecKbn <> "3" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�f�[�^�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord4()
                End If
            Case "5"
                If BeforeRecKbn <> "4" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�f�[�^�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord5()
                End If
            Case "8"
                If BeforeRecKbn <> "5" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�g���[���敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord8()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord8()
                    End If
                End If
            Case "9"
                If BeforeRecKbn <> "8" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�G���h�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord9()
                End If
            Case Else
                DataInfo.Message = "���R�[�h�敪�ُ�i" & RecordData.Substring(0, 1) & "�j�ُ�"
                mnErrorNumber = 1
                Return "ERR"
        End Select

        ' �e�t�H�[�}�b�g�@���ʌ㏈��
        MyBase.CheckDataFormatAfter()

        If ErrString <> "" Then
            ' �K��O��������
            Return ErrString
        End If

        Return sRet
    End Function

    '
    ' �@�\�@ �F �w�b�_���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Public Overrides Function CheckRecord1() As String
        KOKUZEI_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC1.KZ1
        InfoMeisaiMast.SYUBETU_CODE = KOKUZEI_REC1.KZ2
        InfoMeisaiMast.ITAKU_CODE = ""

        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(KOKUZEI_REC1.KZ11, "yyyyMMdd")
        If InfoMeisaiMast.FURIKAE_DATE = "" Then
            InfoMeisaiMast.FURIKAE_DATE = New String("0"c, 8)
        End If
        InfoMeisaiMast.FURIKAE_DATE_MOTO = KOKUZEI_REC1.KZ11

        InfoMeisaiMast.ITAKU_KIN = ""
        InfoMeisaiMast.ITAKU_SIT = ""
        InfoMeisaiMast.ITAKU_KAMOKU = ""
        InfoMeisaiMast.ITAKU_KOUZA = ""
        InfoMeisaiMast.ITAKU_KNAME = ""

        InfoMeisaiMast.I_SIT_NNAME = ""

        '***
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader
        Dim strRet As String = ""

        ' �c�a�ڑ������݂��Ȃ��ꍇ�C����l��Ԃ�
        If OraDB Is Nothing Then
            Return ""
        End If

        ' ���U
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T,TORIF_CODE_T,TYUUDAN_FLG_S,TOUROKU_FLG_S")
        SQL.Append(" FROM TORIMAST,SCHMAST")
        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
        SQL.Append("   AND TORIS_CODE_S =  " & SQ(mInfoComm.INFOToriMast.TORIS_CODE_T))
        SQL.Append("   AND TORIF_CODE_S = " & SQ(mInfoComm.INFOToriMast.TORIF_CODE_T))
        SQL.Append("   AND '1'          = FSYORI_KBN_T")
        SQL.Append("   AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append("   AND TORIF_CODE_S = TORIF_CODE_T")

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then

            WriteBLog("�����܂��̓X�P�W���[������", "���s", "�ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
            DataInfo.Message = "�����܂��ͽ��ޭ�ٌ������s"

            OraReader.Close()

            ' �����}�X�^�����N���A����
            Call mInfoComm.GetTORIMAST("", "")

            Return "ERR"

        Else
            ' �����}�X�^���擾
            Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))

        End If

        If OraReader.EOF = False AndAlso OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
            WriteBLog("�X�P�W���[��:�������ݏ����� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"), "���f")
            DataInfo.Message = "���f�t���O�ݒ�� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            OraReader.Close()
            Return "ERR"
        End If

        If OraReader.EOF = False AndAlso OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
            WriteBLog("�X�P�W���[��:�������ݏ����� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"), "���f")
            DataInfo.Message = "�������ݏ����� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            OraReader.Close()
            Return "ERR"
        End If

        ' �I���N��Reader�N���[�Y
        OraReader.Close()
        WriteBLog("����挟��", "����", "�����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
        Return "H"

    End Function

    Private Function CheckDBRecord1() As String

        '�f�[�^�`�F�b�N
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        Return "H"
    End Function

    '
    ' �@�\�@ �F �f�[�^���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Protected Overridable Function CheckRecord2() As String
        KOKUZEI_REC2.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC2.KZ1
        InfoMeisaiMast.ITAKU_KIN = KOKUZEI_REC2.KZ4.Substring(0, 4)
        InfoMeisaiMast.ITAKU_SIT = KOKUZEI_REC2.KZ4.Substring(4, 3)

        '*** �C�� maeda 2008/05/12************************************************
        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = KOKUZEI_REC2.KZ27
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = KOKUZEI_REC2.KZ28
        '*************************************************************************


        Return "D"
    End Function

    '
    ' �@�\�@ �F �f�[�^���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Protected Overridable Function CheckRecord3() As String
        KOKUZEI_REC3.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC3.KZ1
        InfoMeisaiMast.KEIYAKU_KIN = KOKUZEI_REC3.KZ4.Substring(0, 4)
        InfoMeisaiMast.KEIYAKU_SIT = KOKUZEI_REC3.KZ4.Substring(4, 3)
        InfoMeisaiMast.KEIYAKU_KAMOKU = KOKUZEI_REC3.KZ11
        InfoMeisaiMast.KEIYAKU_KOUZA = KOKUZEI_REC3.KZ12.Trim.PadLeft(7, "0"c)
        InfoMeisaiMast.KEIYAKU_KNAME = KOKUZEI_REC3.KZ30.Trim & KOKUZEI_REC3.KZ31.Trim
        If InfoMeisaiMast.KEIYAKU_KNAME.Length > 40 Then
            InfoMeisaiMast.KEIYAKU_KNAME = InfoMeisaiMast.KEIYAKU_KNAME.Substring(0, 40)
        End If
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(KOKUZEI_REC3.KZ9.TrimStart)
        InfoMeisaiMast.FURIKIN_MOTO = KOKUZEI_REC3.KZ9.TrimStart
        InfoMeisaiMast.SINKI_CODE = "0"
        InfoMeisaiMast.KEIYAKU_NO = KOKUZEI_REC3.KZ5
        InfoMeisaiMast.JYUYOKA_NO = KOKUZEI_REC3.KZ20 & KOKUZEI_REC3.KZ21
        '2011/06/16 �W���ŏC�� ���ł̓E�v��MEIMAST�ɔ��f ------------------START

        ' �E�v
        InfoMeisaiMast.NTEKIYO = ""
        InfoMeisaiMast.KTEKIYO = ""
        Try
            If (Not mInfoComm Is Nothing) Then
                Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                    Case "0"
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case Else
                        Return "ERR"
                End Select
            End If
        Catch ex As Exception
        End Try
        '2011/06/16 �W���ŏC�� ���ł̓E�v��MEIMAST�ɔ��f ------------------END

        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(KOKUZEI_REC3.KZ8)
        InfoMeisaiMast.FURIKETU_MOTO = KOKUZEI_REC3.KZ8

        '*** �C�� maeda 2008/05/12************************************************
        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""
        '*************************************************************************


        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        Return "D"
    End Function

    Private Function CheckDBRecord3() As String

        '�f�[�^�`�F�b�N
        If MyBase.CheckDataRecord() = False Then
            Return "IJO"
        End If

        Return "D"
    End Function

    Protected Overridable Function CheckRecord4() As String
        KOKUZEI_REC2.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC2.KZ1

        '*** �C�� maeda 2008/05/12************************************************
        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""
        '*************************************************************************


        Return "D"
    End Function

    Protected Overridable Function CheckRecord5() As String
        KOKUZEI_REC2.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC2.KZ1

        '*** �C�� maeda 2008/05/12************************************************
        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""
        '*************************************************************************

        Return "D"
    End Function

    '
    ' �@�\�@ �F �g���[���[���R�[�h�`�F�b�N
    '
    ' �߂�l �F True - �����C False - ���s
    '
    ' ���l�@ �F
    '
    Protected Function CheckRecord8() As String
        KOKUZEI_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC8.KZ1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ7.TrimStart)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ8.TrimStart)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ11.TrimStart)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ12.TrimStart)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ9.TrimStart)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(KOKUZEI_REC8.KZ10.TrimStart)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = KOKUZEI_REC8.KZ7.TrimStart
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = KOKUZEI_REC8.KZ8.TrimStart
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = KOKUZEI_REC8.KZ11.TrimStart
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = KOKUZEI_REC8.KZ12.TrimStart
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = KOKUZEI_REC8.KZ9.TrimStart
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = KOKUZEI_REC8.KZ10.TrimStart

        Return "T"
    End Function

    Private Function CheckDBRecord8() As String

        '�f�[�^�`�F�b�N
        If MyBase.CheckTrailerRecord() = False Then
            Return "ERR"
        End If

        Return "T"
    End Function

    '
    ' �@�\�@ �F �G���h���R�[�h�`�F�b�N
    '
    ' �߂�l �F True - �����C False - ���s
    '
    ' ���l�@ �F
    '
    Protected Function CheckRecord9() As String
        KOKUZEI_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = KOKUZEI_REC9.KZ1

        Return "E"
    End Function

    ' �@�\�@ �F �Ԋ҃f�[�^���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overrides Sub GetHenkanDataRecord()
        If mRecordData.StartsWith("3") = False Then
            ' �ʖ��׃��R�[�h�ȊO�̏ꍇ�́C�������Ȃ�
            Return
        End If

        KOKUZEI_REC3.Data = RecordData

        ' �U�֌��ʂ��Z�b�g
        KOKUZEI_REC3.KZ8 = InfoMeisaiMast.FURIKETU_KEKKA

        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
        '�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(24, KOKUZEI_REC3.KZ8)
        End If
        '**********************************************

        RecordData = KOKUZEI_REC3.Data

        ' ���R�[�h�f�[�^�𕪐�
        Call CheckRecord3()

        '�@���ʋ��Z�@�֓X�ܕʁC���ʋ��Z�@�֕� �W�v
        Try
            If InfoMeisaiMast.FURIKETU_CODE = 0 Then
                ' �U�֍ς݌����C�U�֍ς݋��z 
                TOTAL_SIT_NORM_KEN += InfoMeisaiMast.FURIKEN
                TOTAL_KIN_NORM_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    TOTAL_SIT_NORM_KIN += InfoMeisaiMast.FURIKIN
                    TOTAL_KIN_NORM_KIN += InfoMeisaiMast.FURIKIN
                End If
            Else
                ' �ُ팏���C�ُ���z
                TOTAL_SIT_IJO_KEN += InfoMeisaiMast.FURIKEN
                TOTAL_KIN_IJO_KEN += InfoMeisaiMast.FURIKEN
                If InfoMeisaiMast.FURIKEN = 1 Then
                    TOTAL_SIT_IJO_KIN += InfoMeisaiMast.FURIKIN
                    TOTAL_KIN_IJO_KIN += InfoMeisaiMast.FURIKIN
                End If
            End If

            ' ���R�[�h�敪��ۑ�
            BeforeRecKbn = RecordData.Substring(0, 1)
        Catch ex As Exception

        End Try

        Call MyBase.GetHenkanDataRecord()
    End Sub

    ' �@�\�@ �F �Ԋ҃g���[�����R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overrides Sub GetHenkanTrailerRecord()
        ' ���R�[�h�f�[�^�𕪐�
        If mRecordData.StartsWith("4") = True Then
            ' ���ʋ��Z�@�֓X�ܕʃg�[�^�����R�[�h
            Call CheckRecord4()

            ' �U�֕s�\�������Z�b�g
            KOKUZEI_REC2.KZ15 = TOTAL_SIT_IJO_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ16 = TOTAL_SIT_IJO_KIN.ToString.PadLeft(12, " "c)
            ' �U�֍ς݌������Z�b�g
            KOKUZEI_REC2.KZ17 = TOTAL_SIT_NORM_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ18 = TOTAL_SIT_NORM_KIN.ToString.PadLeft(12, " "c)

            '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
            '�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Insert(88, KOKUZEI_REC2.KZ15)
                ReadByteBin.Insert(94, KOKUZEI_REC2.KZ16)
                ReadByteBin.Insert(106, KOKUZEI_REC2.KZ17)
                ReadByteBin.Insert(112, KOKUZEI_REC2.KZ18)
            End If
            '**********************************************

            RecordData = KOKUZEI_REC2.Data

            TOTAL_SIT_IJO_KEN = 0
            TOTAL_SIT_IJO_KIN = 0
            TOTAL_SIT_NORM_KEN = 0
            TOTAL_SIT_NORM_KIN = 0
        ElseIf mRecordData.StartsWith("5") = True Then
            ' ���ʋ��Z�@�֕ʃg�[�^�����R�[�h
            Call CheckRecord5()

            ' �U�֕s�\�������Z�b�g
            KOKUZEI_REC2.KZ15 = TOTAL_KIN_IJO_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ16 = TOTAL_KIN_IJO_KIN.ToString.PadLeft(12, " "c)
            ' �U�֍ς݌������Z�b�g
            KOKUZEI_REC2.KZ17 = TOTAL_KIN_NORM_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC2.KZ18 = TOTAL_KIN_NORM_KIN.ToString.PadLeft(12, " "c)

            '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
            '�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Insert(88, KOKUZEI_REC2.KZ15)
                ReadByteBin.Insert(94, KOKUZEI_REC2.KZ16)
                ReadByteBin.Insert(106, KOKUZEI_REC2.KZ17)
                ReadByteBin.Insert(112, KOKUZEI_REC2.KZ18)
            End If
            '**********************************************

            RecordData = KOKUZEI_REC2.Data

            TOTAL_KIN_IJO_KEN = 0
            TOTAL_KIN_IJO_KIN = 0
            TOTAL_KIN_NORM_KEN = 0
            TOTAL_KIN_NORM_KIN = 0
        ElseIf mRecordData.StartsWith("8") = True Then
            ' �t�@�C�����v���R�[�h
            Call CheckRecord8()

            ' �U�֍ς݌������Z�b�g
            KOKUZEI_REC8.KZ11 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC8.KZ12 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, " "c)
            ' �U�֕s�\�������Z�b�g
            KOKUZEI_REC8.KZ9 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, " "c)
            KOKUZEI_REC8.KZ10 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, " "c)

            '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
            '�o�C�i���f�[�^�����݂���ꍇ�͏���������
            If Not ReadByteBin Is Nothing Then
                ReadByteBin.Insert(88, KOKUZEI_REC8.KZ9)
                ReadByteBin.Insert(94, KOKUZEI_REC8.KZ10)
                ReadByteBin.Insert(106, KOKUZEI_REC8.KZ11)
                ReadByteBin.Insert(112, KOKUZEI_REC8.KZ12)
            End If
            '**********************************************

            RecordData = KOKUZEI_REC8.Data
        End If

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
End Class
