Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' �n���� �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatZeikin220
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 220

    '------------------------------------------
    '�n���̃t�H�[�}�b�g
    '------------------------------------------
    '�w�b�_�[���R�[�h
    Structure ZEIKIN_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String    '�f�[�^�敪(=1)
        <VBFixedString(2)> Public ZK2 As String    '��ʃR�[�h
        <VBFixedString(1)> Public ZK3 As String    '�R�[�h�敪
        <VBFixedString(10)> Public ZK4 As String   '�ϑ��҃R�[�h
        <VBFixedString(40)> Public ZK5 As String   '�ϑ��Җ�
        <VBFixedString(4)> Public ZK6 As String    '�U�֓�
        <VBFixedString(4)> Public ZK7 As String    '�����s�ԍ�
        <VBFixedString(15)> Public ZK8 As String   '�����s��
        <VBFixedString(3)> Public ZK9 As String    '����x�X�ԍ�
        <VBFixedString(15)> Public ZK10 As String  '����x�X��
        <VBFixedString(1)> Public ZK11 As String   '�a�����
        <VBFixedString(7)> Public ZK12 As String   '�����ԍ�
        <VBFixedString(17)> Public ZK13 As String  '�_�~�[
        <VBFixedString(20)> Public ZK14 As String   '���[���
        <VBFixedString(80)> Public ZK15 As String  '�_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2, ZK3, ZK4, ZK5, ZK6, ZK7, _
                             ZK8, ZK9, ZK10, ZK11, ZK12, ZK13, ZK14, _
                             ZK15})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 2)
                ZK3 = CuttingData(Value, 1)
                ZK4 = CuttingData(Value, 10)
                ZK5 = CuttingData(Value, 40)
                ZK6 = CuttingData(Value, 4)
                ZK7 = CuttingData(Value, 4)
                ZK8 = CuttingData(Value, 15)
                ZK9 = CuttingData(Value, 3)
                ZK10 = CuttingData(Value, 15)
                ZK11 = CuttingData(Value, 1)
                ZK12 = CuttingData(Value, 7)
                ZK13 = CuttingData(Value, 17)
                ZK14 = CuttingData(Value, 20)
                ZK15 = CuttingData(Value, 80)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC1 As ZEIKIN_RECORD1

    '�f�[�^���R�[�h
    Structure ZEIKIN_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   '�f�[�^�敪(=2)
        <VBFixedString(4)> Public ZK2 As String   '������s�ԍ�
        <VBFixedString(15)> Public ZK3 As String  '������s��
        <VBFixedString(3)> Public ZK4 As String   '�����x�X�ԍ�
        <VBFixedString(15)> Public ZK5 As String  '�����x�X��
        <VBFixedString(1)> Public ZK6 As String   '�a�����
        <VBFixedString(7)> Public ZK7 As String   '�����ԍ�
        <VBFixedString(30)> Public ZK8 As String  '�������`
        <VBFixedString(10)> Public ZK9 As String  '�������z
        <VBFixedString(10)> Public ZK10 As String '�ی����z(��)
        <VBFixedString(10)> Public ZK11 As String '�ی����z(�t)
        <VBFixedString(20)> Public ZK12 As String '�_�~�[
        <VBFixedString(10)> Public ZK13 As String '�O�[�񏧋�
        <VBFixedString(1)> Public ZK14 As String  '�V�K�R�[�h
        <VBFixedString(20)> Public ZK15 As String '�ڋq�ԍ�
        <VBFixedString(30)> Public ZK16 As String '��ی��Җ�
        <VBFixedString(1)> Public ZK17 As String  '�U�֌��ʃR�[�h
        <VBFixedString(10)> Public ZK18 As String '�ʒ�����
        <VBFixedString(2)> Public ZK19 As String  '�N�x
        <VBFixedString(20)> Public ZK20 As String '�_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2, ZK3, ZK4, ZK5, ZK6, ZK7, _
                             ZK8, ZK9, ZK10, ZK11, ZK12, ZK13, ZK14, _
                             ZK15, ZK16, ZK17, ZK18, ZK19, ZK20})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 4)
                ZK3 = CuttingData(Value, 15)
                ZK4 = CuttingData(Value, 3)
                ZK5 = CuttingData(Value, 15)
                ZK6 = CuttingData(Value, 1)
                ZK7 = CuttingData(Value, 7)
                ZK8 = CuttingData(Value, 30)
                ZK9 = CuttingData(Value, 10)
                ZK10 = CuttingData(Value, 10)
                ZK11 = CuttingData(Value, 10)
                ZK12 = CuttingData(Value, 20)
                ZK13 = CuttingData(Value, 10)
                ZK14 = CuttingData(Value, 1)
                ZK15 = CuttingData(Value, 20)
                ZK16 = CuttingData(Value, 30)
                ZK17 = CuttingData(Value, 1)
                ZK18 = CuttingData(Value, 10)
                ZK19 = CuttingData(Value, 2)
                ZK20 = CuttingData(Value, 20)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC2 As ZEIKIN_RECORD2

    '�g���[�����R�[�h
    Structure ZEIKIN_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   '�f�[�^�敪(=8)
        <VBFixedString(6)> Public ZK2 As String   '���v����
        <VBFixedString(12)> Public ZK3 As String  '���v���z
        <VBFixedString(6)> Public ZK4 As String   '�U�֍ό���
        <VBFixedString(12)> Public ZK5 As String  '�U�֍ϋ��z
        <VBFixedString(6)> Public ZK6 As String   '�U�֕s�\����
        <VBFixedString(12)> Public ZK7 As String  '�U�֕s�\���z
        <VBFixedString(165)> Public ZK8 As String '�_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2, ZK3, ZK4, ZK5, ZK6, ZK7, _
                             ZK8})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 6)
                ZK3 = CuttingData(Value, 12)
                ZK4 = CuttingData(Value, 6)
                ZK5 = CuttingData(Value, 12)
                ZK6 = CuttingData(Value, 6)
                ZK7 = CuttingData(Value, 12)
                ZK8 = CuttingData(Value, 165)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC8 As ZEIKIN_RECORD8

    Structure ZEIKIN_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   '�f�[�^�敪(=9)
        <VBFixedString(219)> Public ZK2 As String   '�_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {ZK1, ZK2})
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 219)
            End Set
        End Property
    End Structure
    Public ZEIKIN_REC9 As ZEIKIN_RECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "220.P"

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
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
        Dim buff As New StringBuilder(DataInfo.LenOfOneRec)
        Dim nRet As Long

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' �w�b�_
                buff.Append(RecordData, 0, 14)
                buff.Append(ReplaceString(RecordData.Substring(14, 40)))
                buff.Append(RecordData.Substring(54))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If
            Case "2"        ' �f�[�^
                buff.Append(RecordData, 0, 46)
                buff.Append(ReplaceString(RecordData.Substring(46, 30)))
                buff.Append(RecordData.Substring(76))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If

                ' �K�蕶���`�F�b�N
                nRet = CheckRegularStringVerB(RecordData, 0, 198)
                If nRet >= 0 Then
                    Return nRet
                End If
            Case "8"        ' �g���[��
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If
            Case "9"        ' �G���h
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    RecordData = buff.ToString(0, RecordLen)
                End If
        End Select

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
        ' ��{�N���X �`�F�b�N
        Dim sRet As String = MyBase.CheckDataFormat()
        If sRet = "ERR" Then
            ' �K��O��������
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
                End If
            Case "2"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�f�[�^�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord2()
                End If
            Case "8"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�g���[���敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord8()
                End If
            Case "9"
                If BeforeRecKbn <> "8" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�G���h�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord9()
                End If
        End Select

        ' �e�t�H�[�}�b�g�@���ʌ㏈��
        MyBase.CheckDataFormatAfter()

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
        ZEIKIN_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC1.ZK1
        InfoMeisaiMast.SYUBETU_CODE = ZEIKIN_REC1.ZK2
        InfoMeisaiMast.CODE_KBN = ZEIKIN_REC1.ZK3
        InfoMeisaiMast.ITAKU_CODE = ZEIKIN_REC1.ZK4
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZEIKIN_REC1.ZK6, "yyyyMMdd")
        InfoMeisaiMast.ITAKU_KIN = ZEIKIN_REC1.ZK7
        InfoMeisaiMast.ITAKU_SIT = ZEIKIN_REC1.ZK9
        InfoMeisaiMast.ITAKU_KAMOKU = ZEIKIN_REC1.ZK11
        InfoMeisaiMast.ITAKU_KOUZA = ZEIKIN_REC1.ZK12
        InfoMeisaiMast.ITAKU_KNAME = ZEIKIN_REC1.ZK5

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
        OraReader.Close()

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
        ZEIKIN_REC2.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC2.ZK1
        InfoMeisaiMast.KEIYAKU_KIN = ZEIKIN_REC2.ZK2
        InfoMeisaiMast.KEIYAKU_SIT = ZEIKIN_REC2.ZK4
        InfoMeisaiMast.KEIYAKU_KAMOKU = ZEIKIN_REC2.ZK6
        InfoMeisaiMast.KEIYAKU_KOUZA = ZEIKIN_REC2.ZK7
        InfoMeisaiMast.KEIYAKU_KNAME = ZEIKIN_REC2.ZK8.Trim
        InfoMeisaiMast.FURIKIN = CASTCommon.CADec(ZEIKIN_REC2.ZK9)
        InfoMeisaiMast.FURIKIN_MOTO = ZEIKIN_REC2.ZK9
        InfoMeisaiMast.SINKI_CODE = ZEIKIN_REC2.ZK14
        InfoMeisaiMast.KEIYAKU_NO = ZEIKIN_REC2.ZK15.Substring(0, 10)
        InfoMeisaiMast.JYUYOKA_NO = ZEIKIN_REC2.ZK15
        If mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
            InfoMeisaiMast.KTEKIYO = ZEIKIN_REC2.ZK5.Trim
            InfoMeisaiMast.NTEKIYO = ""
        Else
            InfoMeisaiMast.KTEKIYO = ""
            InfoMeisaiMast.NTEKIYO = ""
        End If

        InfoMeisaiMast.FURIKETU_CODE = 0
        InfoMeisaiMast.FURIKETU_MOTO = "0"

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        '�f�[�^�`�F�b�N
        If MyBase.CheckDataRecord() = False Then
            Return "IJO"
        End If

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
        ZEIKIN_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC8.ZK1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CADec(ZEIKIN_REC8.ZK2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CADec(ZEIKIN_REC8.ZK3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CADec(ZEIKIN_REC8.ZK4)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CADec(ZEIKIN_REC8.ZK5)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CADec(ZEIKIN_REC8.ZK6)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CADec(ZEIKIN_REC8.ZK7)

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
        ZEIKIN_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN_REC9.ZK1

        Return "E"
    End Function

End Class
