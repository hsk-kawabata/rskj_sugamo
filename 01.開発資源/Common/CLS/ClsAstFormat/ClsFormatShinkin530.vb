Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' ���񂫂�t�H�[�}�b�g�i���R�[�h���T�R�O�j�N���X
Public Class CFormatShinkin530
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 530

    ' ------------------------------------------
    ' ���񂫂�t�H�[�}�b�g
    ' ------------------------------------------
    ' �e���R�[�h���ʕ�
    Public Structure SINKIN128_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '�g���R�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(4)> Public JF4 As String     '�����g���R�[�h
        <VBFixedString(1)> Public JF5 As String     '�Ԋҋ敪�R�[�h
        <VBFixedString(115)> Public JF6 As String   '�\��

        <VBFixedString(402)> Public JF7 As String   '�\��

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
                            SubData(JF6, 115), _
 _
                            SubData(JF7, ModeLen(0)) _
                           })
            End Get
            Set(ByVal value As String)
                JF1 = CuttingData(value, 4)
                JF2 = CuttingData(value, 3)
                JF3 = CuttingData(value, 1)
                JF4 = CuttingData(value, 4)
                JF5 = CuttingData(value, 1)
                JF6 = CuttingData(value, 115)

                JF7 = CuttingData(value, 402)
            End Set
        End Property
    End Structure
    Public SINKIN128_DATA1 As SINKIN128_RECORD1

    ' �f�[�^���R�[�h
    Structure SINKIN128_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '�g���R�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(2)> Public JF4 As String     '�ȖڃR�[�h
        <VBFixedString(7)> Public JF5 As String     '�����ԍ�
        <VBFixedString(6)> Public JF6 As String     '���U�w���
        <VBFixedString(10)> Public JF7 As String    '���z
        <VBFixedString(1)> Public JF8 As String     '���o���敪
        <VBFixedString(3)> Public JF9 As String     '��ƃR�[�h
        <VBFixedString(7)> Public JF10 As String    '��ƃV�[�P���X
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
        <VBFixedString(5)> Public JF22 As String    '�\��

        <VBFixedString(2)> Public JF23 As String    '�t�H�[�}�b�g�敪
        <VBFixedString(394)> Public JF24 As String  '�\��
        <VBFixedString(3)> Public JF25 As String    '�U�փR�[�h
        <VBFixedString(3)> Public JF26 As String    '��ƃR�[�h
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
                            SubData(JF7, 10), _
                            SubData(JF8, 1), _
                            SubData(JF9, 3), _
                            SubData(JF10, 7), _
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
                            SubData(JF22, 5), _
 _
                            SubData(JF23, ModeLen(1)), _
                            SubData(JF24, ModeLen(2)), _
                            SubData(JF25, ModeLen(3)), _
                            SubData(JF26, ModeLen(4)) _
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
                JF22 = CuttingData(Value, 5)

                JF23 = CuttingData(Value, 2)
                JF24 = CuttingData(Value, 394)
                JF25 = CuttingData(Value, 3)
                JF26 = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
    Public SINKIN128_DATA2 As SINKIN128_RECORD2

    ' �G���h���R�[�h
    Structure SINKIN128_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '�g���R�[�h
        <VBFixedString(3)> Public JF2 As String     '�X�܃R�[�h
        <VBFixedString(1)> Public JF3 As String     '���R�[�h���
        <VBFixedString(120)> Public JF4 As String   '�\��

        <VBFixedString(402)> Public JF5 As String   '�\��
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                        { _
                            SubData(JF1, 4), _
                            SubData(JF2, 3), _
                            SubData(JF3, 1), _
                            SubData(JF4, 120), _
 _
                            SubData(JF5, ModeLen(0)) _
                        })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 120)

                JF5 = CuttingData(Value, 402)
            End Set
        End Property
    End Structure
    Public SINKIN128_DATA9 As SINKIN128_RECORD9

    Private Mode As String = CASTCommon.GetFSKJIni("OTHERSYS", "JIFURILEN")
    Private Shared ModeLen(4) As Integer

    ' New
    Public Sub New()
        MyBase.New()

        If Mode = "530" Then
            ModeLen(0) = 402
            ModeLen(1) = 2
            ModeLen(2) = 394
            ModeLen(3) = 3
            ModeLen(4) = 3
        End If

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
