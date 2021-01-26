Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' ���񂫂�t�H�[�}�b�g�i���R�[�h���T�R�O�j�N���X
Public Class CFormatShinkin136
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 136

    ' ------------------------------------------
    ' ���񂫂�t�H�[�}�b�g
    ' ------------------------------------------
    ' �e���R�[�h���ʕ�
    Public Structure SINKIN136_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '�g���R�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(4)> Public JF4 As String     '�����g���R�[�h
        <VBFixedString(1)> Public JF5 As String     '�Ԋҋ敪�R�[�h
        <VBFixedString(123)> Public JF6 As String   '�\��

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                           { _
                            SubData(JF1, 4), _
                            SubData(JF2, 3), _
                            SubData(JF3, 1), _
                            SubData(JF4, 4), _
                            SubData(JF5, 1), _
                            SubData(JF6, 123) _
                           })
            End Get
            Set(ByVal value As String)
                JF1 = CuttingData(value, 4)
                JF2 = CuttingData(value, 3)
                JF3 = CuttingData(value, 1)
                JF4 = CuttingData(value, 4)
                JF5 = CuttingData(value, 1)
                JF6 = CuttingData(value, 123)
            End Set
        End Property
    End Structure
    Public SINKIN136_DATA1 As SINKIN136_RECORD1

    ' �f�[�^���R�[�h
    Structure SINKIN136_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '�g���R�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(2)> Public JF4 As String     '�ȖڃR�[�h
        <VBFixedString(7)> Public JF5 As String     '�����ԍ�
        <VBFixedString(6)> Public JF6 As String     '���U�w���
        <VBFixedString(13)> Public JF7 As String    '���z
        <VBFixedString(1)> Public JF8 As String     '���o���敪
        <VBFixedString(5)> Public JF9 As String     '��ƃR�[�h
        <VBFixedString(8)> Public JF10 As String    '��ƃV�[�P���X
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
        <VBFixedString(7)> Public JF22 As String    '�\��

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 4), _
                            SubData(JF2, 3), _
                            SubData(JF3, 1), _
                            SubData(JF4, 2), _
                            SubData(JF5, 7), _
                            SubData(JF6, 6), _
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
                            SubData(JF22, 7) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 2)
                JF5 = CuttingData(Value, 7)
                JF6 = CuttingData(Value, 6)
                JF7 = CuttingData(Value, 10)
                JF8 = CuttingData(Value, 1)
                JF9 = CuttingData(Value, 3)
                JF10 = CuttingData(Value, 7)
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
                JF22 = CuttingData(Value, 7)
            End Set
        End Property
    End Structure
    Public SINKIN136_DATA2 As SINKIN136_RECORD2

    ' �G���h���R�[�h
    Structure SINKIN136_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '�g���R�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(128)> Public JF4 As String   '�\��

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 4), _
                            SubData(JF2, 3), _
                            SubData(JF3, 1), _
                            SubData(JF4, 128) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 128)
            End Set
        End Property
    End Structure
    Public SINKIN136_DATA9 As SINKIN136_RECORD9


    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"0", "9"}

        FtranPfile = "530.P"

        HeaderKubun = New String() {"0"}
        DataKubun = New String() {"1", "2", "3", "4"}
        TrailerKubun = New String() {"9"}
    End Sub

End Class
