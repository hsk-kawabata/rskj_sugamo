Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic


' ����Z���^�[���U�i�Q�Q�T�j �f�[�^�t�H�[�}�b�g�N���X
Public Class clsFormatFunou
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 225


    '------------------------------
    '���C�Z���^�[���U�t�H�[�}�b�g��`
    '------------------------------
    '----------------
    '�w�b�_�[���R�[�h
    '----------------
    Structure FUNOU225_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(4)> Public JF1 As String     '��Ǝ���
        <VBFixedString(8)> Public JF2 As String     '�f�[�^���
        <VBFixedString(2)> Public JF3 As String     '�T�C�N��
        <VBFixedString(2)> Public JF4 As String     '�\��
        <VBFixedString(1)> Public JF5 As String     '��Ƌ敪
        <VBFixedString(4)> Public JF6 As String     '���Z�@�փR�[�h
        <VBFixedString(3)> Public JF7 As String     '�x�X�R�[�h
        <VBFixedString(201)> Public JF8 As String   '�\��
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                {JF1, JF2, JF3, JF4, JF5, JF6, JF7, JF8})
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 8)
                JF3 = CuttingData(Value, 2)
                JF4 = CuttingData(Value, 2)
                JF5 = CuttingData(Value, 1)
                JF6 = CuttingData(Value, 4)
                JF7 = CuttingData(Value, 3)
                JF8 = CuttingData(Value, 201)
            End Set
        End Property
    End Structure
    Public FUNOU225_REC1 As FUNOU225_RECORD1

    ' �f�[�^���R�[�h
    Structure FUNOU225_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(8)> Public JF1 As String     '���U�w���
        <VBFixedString(3)> Public JF2 As String     '�Z���^�[�ꊇ�������
        <VBFixedString(4)> Public JF3 As String     '���Z�@�փR�[�h
        <VBFixedString(3)> Public JF4 As String     '�X�܃R�[�h
        <VBFixedString(3)> Public JF5 As String     '����D��R�[�h
        <VBFixedString(3)> Public JF6 As String     '��ƃR�[�h
        <VBFixedString(2)> Public JF7 As String     '�Ȗ�
        <VBFixedString(7)> Public JF8 As String     '����
        <VBFixedString(2)> Public JF9 As String     '�������D��R�[�h
        <VBFixedString(7)> Public JF10 As String    '�V�[�P���XNO
        <VBFixedString(10)> Public JF11 As String   '���z
        <VBFixedString(9)> Public JF12 As String    '�U�։ȖځE����
        <VBFixedString(1)> Public JF13 As String    '�o�^�敪
        <VBFixedString(4)> Public JF14 As String    '���ʕs�\STS
        <VBFixedString(4)> Public JF15 As String    '���ʎ��U����STS
        <VBFixedString(2)> Public JF16 As String    '�s�\���R�R�[�h
        <VBFixedString(3)> Public JF17 As String    '�U�փR�[�h
        <VBFixedString(5)> Public JF18 As String    '�\��
        <VBFixedString(1)> Public JF19 As String    '�d�b�敪�R�[�h
        <VBFixedString(1)> Public JF20 As String    '�s�\�A����R�[�h
        <VBFixedString(2)> Public JF21 As String    '�S���҃R�[�h
        <VBFixedString(13)> Public JF22 As String   '�����E�v/�J�i�E�v
        <VBFixedString(3)> Public JF23 As String    '�戵�X��
        <VBFixedString(7)> Public JF24 As String    '�ڋq�ԍ�
        <VBFixedString(13)> Public JF25 As String   '����
        <VBFixedString(10)> Public JF26 As String   '�x���\�c��
        <VBFixedString(3)> Public JF27 As String    '���Ԓ����p�X��
        <VBFixedString(9)> Public JF28 As String    '���Ԓ����p����
        <VBFixedString(9)> Public JF29 As String    '��������p�Ȗڌ���
        <VBFixedString(8)> Public JF30 As String    '�������U�w���
        <VBFixedString(24)> Public JF31 As String   '���v�Ɣԍ�
        <VBFixedString(2)> Public JF32 As String    '�����Ǘ�NO
        <VBFixedString(4)> Public JF33 As String    '�������Z�@�փR�[�h
        <VBFixedString(1)> Public JF34 As String    '�Ԋҋ敪
        <VBFixedString(8)> Public JF35 As String    '���X����������
        <VBFixedString(3)> Public JF36 As String    '�Z���^�[�J�b�g�����R�[�h
        <VBFixedString(7)> Public JF37 As String    '����ڋq�ԍ�
        <VBFixedString(10)> Public JF38 As String   '�d�b�ԍ�
        <VBFixedString(7)> Public JF39 As String    '�\��

        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                 JF8, JF9, JF10, JF11, JF12, JF13, JF14, _
                 JF15, JF16, JF17, JF18, JF19, JF20, JF21, _
                 JF22, JF23, JF24, JF25, JF26, JF27, JF28, _
                 JF29, JF30, JF31, JF32, JF33, JF34, JF35, _
                 JF36, JF37, JF38, JF39})
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 8)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 4)
                JF4 = CuttingData(Value, 3)
                JF5 = CuttingData(Value, 3)
                JF6 = CuttingData(Value, 3)
                JF7 = CuttingData(Value, 2)
                JF8 = CuttingData(Value, 7)
                JF9 = CuttingData(Value, 2)
                JF10 = CuttingData(Value, 7)
                JF11 = CuttingData(Value, 10)
                JF12 = CuttingData(Value, 9)
                JF13 = CuttingData(Value, 1)
                JF14 = CuttingData(Value, 4)
                JF15 = CuttingData(Value, 4)
                JF16 = CuttingData(Value, 2)
                JF17 = CuttingData(Value, 3)
                JF18 = CuttingData(Value, 5)
                JF19 = CuttingData(Value, 1)
                JF20 = CuttingData(Value, 1)
                JF21 = CuttingData(Value, 2)
                JF22 = CuttingData(Value, 13)
                JF23 = CuttingData(Value, 3)
                JF24 = CuttingData(Value, 7)
                JF25 = CuttingData(Value, 13)
                JF26 = CuttingData(Value, 10)
                JF27 = CuttingData(Value, 3)
                JF28 = CuttingData(Value, 9)
                JF29 = CuttingData(Value, 9)
                JF30 = CuttingData(Value, 8)
                JF31 = CuttingData(Value, 24)
                JF32 = CuttingData(Value, 2)
                JF33 = CuttingData(Value, 4)
                JF34 = CuttingData(Value, 1)
                JF35 = CuttingData(Value, 8)
                JF36 = CuttingData(Value, 3)
                JF37 = CuttingData(Value, 7)
                JF38 = CuttingData(Value, 10)
                JF39 = CuttingData(Value, 7)

            End Set
        End Property
    End Structure
    Public FUNOU225_REC2 As FUNOU225_RECORD2

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"H", ""}

        FtranPfile = "225.P"

        HeaderKubun = New String() {"H"}
        DataKubun = New String() {""}
    End Sub

    '
    ' �@�\   �F ���R�[�h�`�F�b�N
    '
    ' �߂�l �F �s�������̈ʒu
    '
    ' ���l   �F
    '
    Public Overrides Function CheckRegularString() As Long
        ' �`�F�b�N���Ȃ�
        Return 0
    End Function

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"" - �f�[�^
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F
    '
    Public Overrides Function CheckDataFormat() As String
        Dim sRet As String = MyBase.CheckDataFormat()

        If RecordData.Length = 0 Then
            DataInfo.Message = "�t�@�C���ُ�"
            mnErrorNumber = 1
            Return "ERR"
        End If

        Select Case RecordData.Substring(0, 1)
            Case "H"
                sRet = CheckRecord1()
            Case Else
                sRet = CheckRecord2()
        End Select

        Return sRet
    End Function

    '
    ' �@�\�@ �F �w�b�_���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_
    '           "ERR" - �G���[����
    ' ���l�@ �F
    '
    Public Overrides Function CheckRecord1() As String
        FUNOU225_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�

        'InfoMeisaiMast.  = FUNOU225_REC1.JF1
        InfoMeisaiMast.SYUBETU_CODE = FUNOU225_REC1.JF2
        'InfoMeisaiMast.  = FUNOU225_REC1.JF3
        'InfoMeisaiMast.  = FUNOU225_REC1.JF4
        'InfoMeisaiMast.  = FUNOU225_REC1.JF5
        InfoMeisaiMast.ITAKU_KIN = FUNOU225_REC1.JF6
        InfoMeisaiMast.ITAKU_SIT = FUNOU225_REC1.JF7
        'InfoMeisaiMast.  = FUNOU225_REC1.JF8

        ' �f�[�^�`�F�b�N
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        Return "H"
    End Function

    '
    ' �@�\�@ �F �f�[�^���R�[�h�`�F�b�N
    '
    ' �߂�l �F "" - �f�[�^
    '           "IJO" - �C���v�b�g�G���[
    ' ���l�@ �F
    '
    Protected Overridable Function CheckRecord2() As String
        FUNOU225_REC2.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        'InfoMeisaiMast. = funou225_REC2.JF1
        'InfoMeisaiMast. = funou255_rec2.jf2
        InfoMeisaiMast.KEIYAKU_KIN = FUNOU225_REC2.JF3
        InfoMeisaiMast.KEIYAKU_SIT = FUNOU225_REC2.JF4
        'InfoMeisaiMast. = funou225_REC2.JF5
        'InfoMeisaiMast. = funou225_REC2.JF6
        InfoMeisaiMast.KEIYAKU_KAMOKU = FUNOU225_REC2.JF7
        InfoMeisaiMast.KEIYAKU_KOUZA = FUNOU225_REC2.JF8
        'InfoMeisaiMast. = funou225_REC2.JF9
        'InfoMeisaiMast. = funou225_REC2.JF10
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(FUNOU225_REC2.JF11)
        'InfoMeisaiMast. = funou225_REC2.JF12
        'InfoMeisaiMast. = funou225_REC2.JF13
        'InfoMeisaiMast. = funou225_REC2.JF14
        'InfoMeisaiMast. = funou225_REC2.JF15
        'InfoMeisaiMast. = funou225_REC2.JF16
        'InfoMeisaiMast. = funou225_REC2.JF17
        'InfoMeisaiMast. = funou225_REC2.JF18
        'InfoMeisaiMast. = funou225_REC2.JF19
        'InfoMeisaiMast. = funou225_REC2.JF20
        'InfoMeisaiMast. = funou225_REC2.JF21
        'InfoMeisaiMast. = funou225_REC2.JF22
        'InfoMeisaiMast. = funou225_REC2.JF23
        'InfoMeisaiMast. = funou225_REC2.JF24
        InfoMeisaiMast.KEIYAKU_KNAME = FUNOU225_REC2.JF25
        'InfoMeisaiMast. = funou225_REC2.JF26
        InfoMeisaiMast.TEISEI_SIT = FUNOU225_REC2.JF27
        InfoMeisaiMast.TEISEI_KOUZA = FUNOU225_REC2.JF28
        'InfoMeisaiMast. = funou225_REC2.JF29
        'InfoMeisaiMast. = funou225_REC2.JF30
        'InfoMeisaiMast. = funou225_REC2.JF31
        'InfoMeisaiMast. = funou225_REC2.JF32
        'InfoMeisaiMast. = funou225_REC2.JF33
        'InfoMeisaiMast. = funou225_REC2.JF34
        'InfoMeisaiMast. = funou225_REC2.JF35
        'InfoMeisaiMast. = funou225_REC2.JF36
        'InfoMeisaiMast. = funou225_REC2.JF37
        'InfoMeisaiMast. = funou225_REC2.JF38
        'InfoMeisaiMast. = funou225_REC2.JF39

        ' �f�[�^�`�F�b�N
        If MyBase.CheckDataRecord() = False Then
            Return "IJO"
        End If

        Return ""
    End Function

End Class
