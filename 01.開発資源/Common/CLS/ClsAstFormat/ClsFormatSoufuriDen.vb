Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

''' <summary>
''' MT��s�f�[�^�@�t�H�[�}�b�g��`�N���X
''' </summary>
''' <remarks>
''' 2016/10/19 saitou RSV2 added for �M�g�Ή�
''' �}���M�g�ɓ�������Ă���MT��s�t�H�[�}�b�g�N���X��RSV2�ɈڐA
''' </remarks>
Public Class CFormatSoufuriDen
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 400

    '------------------------------------------
    '���U�`���f�[�^�t�H�[�}�b�g
    '------------------------------------------
    '�w�b�_���R�[�h
    Public Structure SDRECORD1
        Implements CFormat.IFormat

        <VBFixedString(8)> Public SD1 As String     ' �u���b�N�ԍ�
        <VBFixedString(8)> Public SD2 As String     ' ���M�w���
        <VBFixedString(6)> Public SD3 As String     ' MT�ԍ�             �����ɃR�[�h�@���@�i����ԍ�+1�j
        <VBFixedString(15)> Public SD4 As String    ' �˗��g����         �g����
        <VBFixedString(4)> Public SD5 As String     ' �˗��g���R�[�h     �����ɃR�[�h
        <VBFixedString(359)> Public SD6 As String   ' �\��

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SD1, 8), _
                            SubData(SD2, 8), _
                            SubData(SD3, 6), _
                            SubData(SD4, 15), _
                            SubData(SD5, 4), _
                            SubData(SD6, 359) _
                            })
            End Get
            Set(ByVal value As String)
                SD1 = CuttingData(value, 8)
                SD2 = CuttingData(value, 8)
                SD3 = CuttingData(value, 6)
                SD4 = CuttingData(value, 15)
                SD5 = CuttingData(value, 4)
                SD6 = CuttingData(value, 359)
            End Set
        End Property
    End Structure
    Public SOUDEN_REC1 As SDRECORD1

    '�f�[�^���R�[�h
    Structure SDRECORD2
        Implements CFormat.IFormat

        <VBFixedString(8)> Public SD1 As String     ' �u���b�N�ԍ�
        <VBFixedString(1)> Public SD2 As String     ' ���ω�
        <VBFixedString(1)> Public SD3 As String     ' �\��1
        <VBFixedString(8)> Public SD4 As String     ' �戵��
        <VBFixedString(4)> Public SD5 As String     ' �戵���ʐM��ڃR�[�h
        <VBFixedString(3)> Public SD6 As String     ' �t���R�[�h
        <VBFixedString(15)> Public SD7 As String    ' ��M���Z�@�֖�
        <VBFixedString(1)> Public SD8 As String     ' ���1
        <VBFixedString(15)> Public SD9 As String    ' ��M�X�ܖ�
        <VBFixedString(15)> Public SD10 As String   ' ���z
        <VBFixedString(15)> Public SD11 As String   ' ���M�g����
        <VBFixedString(1)> Public SD12 As String    ' ���2
        <VBFixedString(15)> Public SD13 As String   ' ���M�X�ܖ�
        <VBFixedString(7)> Public SD14 As String    ' ��s�Ԏ萔��
        <VBFixedString(16)> Public SD15 As String   ' �ԍ���
        <VBFixedString(20)> Public SD16 As String   ' EDI���
        <VBFixedString(48)> Public SD17 As String   ' ���l��
        <VBFixedString(48)> Public SD18 As String   ' �˗��l��
        <VBFixedString(48)> Public SD19 As String   ' ���l1
        <VBFixedString(48)> Public SD20 As String   ' ���l2
        <VBFixedString(15)> Public SD21 As String   ' ���M�ԍ�
        <VBFixedString(15)> Public SD22 As String   ' �Ɖ�ԍ�
        <VBFixedString(33)> Public SD23 As String   ' �\��2

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SD1, 8), _
                            SubData(SD2, 1), _
                            SubData(SD3, 1), _
                            SubData(SD4, 8), _
                            SubData(SD5, 4), _
                            SubData(SD6, 3), _
                            SubData(SD7, 15), _
                            SubData(SD8, 1), _
                            SubData(SD9, 15), _
                            SubData(SD10, 15), _
                            SubData(SD11, 15), _
                            SubData(SD12, 1), _
                            SubData(SD13, 15), _
                            SubData(SD14, 7), _
                            SubData(SD15, 16), _
                            SubData(SD16, 20), _
                            SubData(SD17, 48), _
                            SubData(SD18, 48), _
                            SubData(SD19, 48), _
                            SubData(SD20, 48), _
                            SubData(SD21, 15), _
                            SubData(SD22, 15), _
                            SubData(SD23, 33) _
                            })
            End Get
            Set(ByVal Value As String)
                SD1 = CuttingData(Value, 8)
                SD2 = CuttingData(Value, 1)
                SD3 = CuttingData(Value, 1)
                SD4 = CuttingData(Value, 8)
                SD5 = CuttingData(Value, 4)
                SD6 = CuttingData(Value, 3)
                SD7 = CuttingData(Value, 15)
                SD8 = CuttingData(Value, 1)
                SD9 = CuttingData(Value, 15)
                SD10 = CuttingData(Value, 15)
                SD11 = CuttingData(Value, 15)
                SD12 = CuttingData(Value, 1)
                SD13 = CuttingData(Value, 15)
                SD14 = CuttingData(Value, 7)
                SD15 = CuttingData(Value, 16)
                SD16 = CuttingData(Value, 20)
                SD17 = CuttingData(Value, 48)
                SD18 = CuttingData(Value, 48)
                SD19 = CuttingData(Value, 48)
                SD20 = CuttingData(Value, 48)
                SD21 = CuttingData(Value, 15)
                SD22 = CuttingData(Value, 15)
                SD23 = CuttingData(Value, 33)
            End Set
        End Property
    End Structure
    Public SOUDEN_REC2 As SDRECORD2

    '�t�b�^�[���R�[�h
    Public Structure SDRECORD9
        Implements CFormat.IFormat

        <VBFixedString(8)> Public SD1 As String     ' �u���b�N�ԍ�
        <VBFixedString(6)> Public SD2 As String     ' ���d����
        <VBFixedString(12)> Public SD3 As String    ' ���d�����v���z
        <VBFixedString(6)> Public SD4 As String     ' ��U�d����
        <VBFixedString(12)> Public SD5 As String    ' ��U�d�����v���z
        <VBFixedString(6)> Public SD6 As String     ' �t�d����
        <VBFixedString(12)> Public SD7 As String    ' �t�d�����v���z
        <VBFixedString(6)> Public SD8 As String     ' ��藧��\0�d����
        <VBFixedString(6)> Public SD9 As String     ' �ʐM�d����
        <VBFixedString(6)> Public SD10 As String    ' ���v�d����
        <VBFixedString(320)> Public SD11 As String  ' �\��

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SD1, 8), _
                            SubData(SD2, 6), _
                            SubData(SD3, 12), _
                            SubData(SD4, 6), _
                            SubData(SD5, 12), _
                            SubData(SD6, 6), _
                            SubData(SD7, 12), _
                            SubData(SD8, 6), _
                            SubData(SD9, 6), _
                            SubData(SD10, 6), _
                            SubData(SD11, 320) _
                            })
            End Get
            Set(ByVal Value As String)
                SD1 = CuttingData(Value, 8)
                SD2 = CuttingData(Value, 6)
                SD3 = CuttingData(Value, 12)
                SD4 = CuttingData(Value, 6)
                SD5 = CuttingData(Value, 12)
                SD6 = CuttingData(Value, 6)
                SD7 = CuttingData(Value, 12)
                SD8 = CuttingData(Value, 6)
                SD9 = CuttingData(Value, 6)
                SD10 = CuttingData(Value, 6)
                SD11 = CuttingData(Value, 320)
            End Set
        End Property
    End Structure
    Public SOUDEN_REC9 As SDRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        FtranPfile = "400.P"

    End Sub
End Class

' �������σf�[�^�t�H�[�}�b�g
Public Class ClsFormSOU

    ' �Œ蒷�\���̗p�C���^�[�t�F�[�X
    Protected Interface IFormat
        ' �f�[�^
        Sub Init()
        Property Data() As String
    End Interface

    ' SHIT-JIS�G���R�[�f�B���O
    Protected Friend Shared EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")

#Region "���U�`���ב֖��חp"
    Public Structure PrnSoufuriData
        Implements ClsFormSOU.IFormat

        '--------�w�b�_--------
        Dim HassinDate As String        '���M��
        '--------�O���[�v------
        Dim TorisCode As String         '������R�[�h
        Dim TorifCode As String         '����敛�R�[�h
        Dim ToriNName As String         '����於(���{��)
        Dim FuriDate As String          '�U����
        Dim Baitai As String            '�}��
        Dim Syubetu As String           '���
        Dim TekiyouSyubetu As String    '�K�p���
        Dim HonbuTenCode As String      '�{���x�X�R�[�h
        Dim HonbuTenName As String      '�{���x�X��
        Dim KeiyakuName As String       '���l��
        Dim FurikomiKinCode As String   '�U�����Z�@�փR�[�h
        Dim FurikomiKinName As String   '�U�����Z�@�֖�
        Dim FurikomiSitCode As String   '�U���x�X�R�[�h
        Dim FurikomiSitName As String   '�U���x�X��
        Dim Kamoku As String            '�Ȗ�
        Dim KouzaNo As String           '�����ԍ�
        Dim FurikomiKIN As String       '�U����
        Dim Bikou1 As String            '���l�P
        Dim Bikou2 As String            '���l�Q
        Dim TukekaeKinCode As String    '�t�֋��Z�@�փR�[�h
        Dim TukekaeKinName As String    '�t�֋��Z�@�֖�
        Dim TukekaeSitCode As String    '�t�֎x�X�R�[�h
        Dim TukekaeSitName As String    '�t�֎x�X��

        Public Sub Init() Implements IFormat.Init
            HassinDate = ""         '���M��

            TorisCode = ""          '������R�[�h
            TorifCode = ""          '����敛�R�[�h
            ToriNName = ""          '����於(���{��)
            FuriDate = ""           '�U����
            Baitai = ""             '�}��
            Syubetu = ""            '���
            TekiyouSyubetu = ""     '�K�p���
            HonbuTenCode = ""       '�{���x�X�R�[�h
            HonbuTenName = ""       '�{���x�X��
            KeiyakuName = ""        '���l��
            FurikomiKinCode = ""    '�U�����Z�@�փR�[�h
            FurikomiKinName = ""    '�U�����Z�@�֖�
            FurikomiSitCode = ""    '�U���x�X�R�[�h
            FurikomiSitName = ""    '�U���x�X��
            Kamoku = ""             '�Ȗ�
            KouzaNo = ""            '�����ԍ�
            FurikomiKIN = ""        '�U����
            Bikou1 = ""             '���l�P
            Bikou2 = ""             '���l�Q
            TukekaeKinCode = ""     '�t�֋��Z�@�փR�[�h
            TukekaeKinName = ""     '�t�֋��Z�@�֖�
            TukekaeSitCode = ""     '�t�֎x�X�R�[�h
            TukekaeSitName = ""     '�t�֎x�X��

        End Sub

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements IFormat.Data
            Get
                Dim record As String = ""

                record = String.Concat(New String() _
                            { _
                            SubData(HassinDate, 8), _
                            SubData(TorisCode, 10), _
                            SubData(TorifCode, 2), _
                            SubData(ToriNName, 50), _
                            SubData(FuriDate, 8), _
                            SubData(Baitai, 2), _
                            SubData(Syubetu, 4), _
                            SubData(TekiyouSyubetu, 8), _
                            SubData(HonbuTenCode, 3), _
                            SubData(HonbuTenName, 15), _
                            SubData(KeiyakuName, 40), _
                            SubData(FurikomiKinCode, 4), _
                            SubData(FurikomiKinName, 15), _
                            SubData(FurikomiSitCode, 3), _
                            SubData(FurikomiSitName, 15), _
                            SubData(Kamoku, 1), _
                            SubData(KouzaNo, 7), _
                            SubData(FurikomiKIN, 15), _
                            SubData(Bikou1, 48), _
                            SubData(Bikou2, 48), _
                            SubData(TukekaeKinCode, 4), _
                            SubData(TukekaeKinName, 15), _
                            SubData(TukekaeSitCode, 4), _
                            SubData(TukekaeSitName, 15) _
                            })
                Return record
            End Get
            Set(ByVal value As String)
                HassinDate = CuttingData(value, 8)
                TorisCode = CuttingData(value, 10)
                TorifCode = CuttingData(value, 2)
                ToriNName = CuttingData(value, 50)
                FuriDate = CuttingData(value, 8)
                Baitai = CuttingData(value, 2)
                Syubetu = CuttingData(value, 4)
                TekiyouSyubetu = CuttingData(value, 8)
                HonbuTenCode = CuttingData(value, 3)
                HonbuTenname = CuttingData(value, 15)
                KeiyakuName = CuttingData(value, 40)
                FurikomiKinCode = CuttingData(value, 4)
                FurikomiKinName = CuttingData(value, 15)
                FurikomiSitCode = CuttingData(value, 3)
                FurikomiSitName = CuttingData(value, 15)
                Kamoku = CuttingData(value, 1)
                KouzaNo = CuttingData(value, 7)
                FurikomiKIN = CuttingData(value, 15)
                Bikou1 = CuttingData(value, 48)
                Bikou2 = CuttingData(value, 48)
                TukekaeKinCode = CuttingData(value, 4)
                TukekaeKinName = CuttingData(value, 15)
                TukekaeSitCode = CuttingData(value, 4)
                TukekaeSitName = CuttingData(value, 15)

            End Set

        End Property
    End Structure
#End Region
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
    Protected Friend Shared Function SubData(ByVal value As String, ByVal len As Integer, _
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
            Dim ret As String
            Dim bt() As Byte = EncdJ.GetBytes(value)
            ret = EncdJ.GetString(bt, 0, len)
            ' �؂�������̎c��̕�����
            value = value.Substring(ret.Length())
            Return ret
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class


