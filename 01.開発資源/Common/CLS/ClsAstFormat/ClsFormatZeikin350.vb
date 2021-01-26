Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' �n���́i�R�T�O�j �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatZeikin350
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 350

    ' ------------------------------------------
    ' �n���̃t�H�[�}�b�g�i�R�T�O�o�C�g�j
    ' ------------------------------------------
    ' �w�b�_�[���R�[�h
    Structure ZEIKIN350_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String    ' �f�[�^�敪(=1)
        <VBFixedString(2)> Public ZK2 As String    ' ��ʃR�[�h
        <VBFixedString(1)> Public ZK3 As String    ' �R�[�h�敪
        <VBFixedString(8)> Public ZK4 As String    ' �ϑ��҃R�[�h
        <VBFixedString(2)> Public ZK5 As String    ' �ȖڃR�[�h
        <VBFixedString(5)> Public ZK6 As String    ' �Ȗږ�
        <VBFixedString(35)> Public ZK7 As String   ' �ϑ��Җ�
        <VBFixedString(2)> Public ZK8 As String    ' �U�֔N
        <VBFixedString(2)> Public ZK9 As String    ' �U�֌�
        <VBFixedString(2)> Public ZK10 As String   ' �U�֓�
        <VBFixedString(4)> Public ZK11 As String   ' ��s�R�[�h
        <VBFixedString(15)> Public ZK12 As String  ' ��s��
        <VBFixedString(3)> Public ZK13 As String   ' �x�X�R�[�h
        <VBFixedString(15)> Public ZK14 As String  ' �x�X��
        <VBFixedString(1)> Public ZK15 As String   ' �����a�����
        <VBFixedString(7)> Public ZK16 As String   ' ���������ԍ�
        <VBFixedString(2)> Public ZK17 As String   ' �N�x
        <VBFixedString(243)> Public ZK18 As String ' �\��
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 2), _
                            SubData(ZK3, 1), _
                            SubData(ZK4, 8), _
                            SubData(ZK5, 2), _
                            SubData(ZK6, 5), _
                            SubData(ZK7, 35), _
                            SubData(ZK8, 2), _
                            SubData(ZK9, 2), _
                            SubData(ZK10, 2), _
                            SubData(ZK11, 4), _
                            SubData(ZK12, 15), _
                            SubData(ZK13, 3), _
                            SubData(ZK14, 15), _
                            SubData(ZK15, 1), _
                            SubData(ZK16, 7), _
                            SubData(ZK17, 2), _
                            SubData(ZK18, 243) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 2)
                ZK3 = CuttingData(Value, 1)
                ZK4 = CuttingData(Value, 8)
                ZK5 = CuttingData(Value, 2)
                ZK6 = CuttingData(Value, 5)
                ZK7 = CuttingData(Value, 35)
                ZK8 = CuttingData(Value, 2)
                ZK9 = CuttingData(Value, 2)
                ZK10 = CuttingData(Value, 2)
                ZK11 = CuttingData(Value, 4)
                ZK12 = CuttingData(Value, 15)
                ZK13 = CuttingData(Value, 3)
                ZK14 = CuttingData(Value, 15)
                ZK15 = CuttingData(Value, 1)
                ZK16 = CuttingData(Value, 7)
                ZK17 = CuttingData(Value, 2)
                ZK18 = CuttingData(Value, 243)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC1 As ZEIKIN350_RECORD1

    ' �f�[�^���R�[�h
    Structure ZEIKIN350_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   ' �f�[�^�敪(=2)
        <VBFixedString(4)> Public ZK2 As String   ' ������s�ԍ�
        <VBFixedString(15)> Public ZK3 As String  ' ������s��
        <VBFixedString(3)> Public ZK4 As String   ' �����x�X�ԍ�
        <VBFixedString(15)> Public ZK5 As String  ' �����x�X��
        <VBFixedString(4)> Public ZK6 As String   ' �\��
        <VBFixedString(1)> Public ZK7 As String   ' �a�����
        <VBFixedString(7)> Public ZK8 As String   ' �����ԍ�
        <VBFixedString(30)> Public ZK9 As String  ' �������`�l
        <VBFixedString(10)> Public ZK10 As String ' �������z
        <VBFixedString(1)> Public ZK11 As String  ' �V�K�R�[�h
        <VBFixedString(1)> Public ZK12 As String  ' �U�֌��ʃR�[�h
        <VBFixedString(10)> Public ZK13 As String ' �\��(�X�y�[�X�{��s�g�p��)
        <VBFixedString(18)> Public ZK14 As String ' �i�V�j�`���Ҕԍ�
        <VBFixedString(2)> Public ZK15 As String  ' �\��
        <VBFixedString(18)> Public ZK16 As String ' �i���j�`���Ҕԍ�
        <VBFixedString(2)> Public ZK17 As String  ' ��ڃR�[�h
        <VBFixedString(5)> Public ZK18 As String  ' ��ږ�
        <VBFixedString(2)> Public ZK19 As String  ' �N�x
        <VBFixedString(5)> Public ZK20 As String  ' ����
        <VBFixedString(11)> Public ZK21 As String ' �����ԍ�
        <VBFixedString(26)> Public ZK22 As String ' ���[�ԍ�
        <VBFixedString(7)> Public ZK23 As String  ' �\��
        <VBFixedString(30)> Public ZK24 As String ' �[�Łi�t�j�Җ�����
        <VBFixedString(5)> Public ZK25 As String  ' �X�֔ԍ�
        <VBFixedString(22)> Public ZK26 As String ' �Z���i�s�����j
        <VBFixedString(22)> Public ZK27 As String ' �Z���i�����j
        <VBFixedString(22)> Public ZK28 As String ' �Z���i�Ԓn�j
        <VBFixedString(22)> Public ZK29 As String ' �Z���i�����j
        <VBFixedString(15)> Public ZK30 As String ' �Ȗ�
        <VBFixedString(14)> Public ZK31 As String ' �����ԍ�
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 4), _
                            SubData(ZK3, 15), _
                            SubData(ZK4, 3), _
                            SubData(ZK5, 15), _
                            SubData(ZK6, 4), _
                            SubData(ZK7, 1), _
                            SubData(ZK8, 7), _
                            SubData(ZK9, 30), _
                            SubData(ZK10, 10), _
                            SubData(ZK11, 1), _
                            SubData(ZK12, 1), _
                            SubData(ZK13, 10), _
                            SubData(ZK14, 18), _
                            SubData(ZK15, 2), _
                            SubData(ZK16, 18), _
                            SubData(ZK17, 2), _
                            SubData(ZK18, 5), _
                            SubData(ZK19, 2), _
                            SubData(ZK20, 5), _
                            SubData(ZK21, 11), _
                            SubData(ZK22, 26), _
                            SubData(ZK23, 7), _
                            SubData(ZK24, 30), _
                            SubData(ZK25, 5), _
                            SubData(ZK26, 22), _
                            SubData(ZK27, 22), _
                            SubData(ZK28, 22), _
                            SubData(ZK29, 22), _
                            SubData(ZK30, 15), _
                            SubData(ZK31, 14) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 4)
                ZK3 = CuttingData(Value, 15)
                ZK4 = CuttingData(Value, 3)
                ZK5 = CuttingData(Value, 15)
                ZK6 = CuttingData(Value, 4)
                ZK7 = CuttingData(Value, 1)
                ZK8 = CuttingData(Value, 7)
                ZK9 = CuttingData(Value, 30)
                ZK10 = CuttingData(Value, 10)
                ZK11 = CuttingData(Value, 1)
                ZK12 = CuttingData(Value, 1)
                ZK13 = CuttingData(Value, 10)
                ZK14 = CuttingData(Value, 18)
                ZK15 = CuttingData(Value, 2)
                ZK16 = CuttingData(Value, 18)
                ZK17 = CuttingData(Value, 2)
                ZK18 = CuttingData(Value, 5)
                ZK19 = CuttingData(Value, 2)
                ZK20 = CuttingData(Value, 5)
                ZK21 = CuttingData(Value, 11)
                ZK22 = CuttingData(Value, 26)
                ZK23 = CuttingData(Value, 7)
                ZK24 = CuttingData(Value, 30)
                ZK25 = CuttingData(Value, 5)
                ZK26 = CuttingData(Value, 22)
                ZK27 = CuttingData(Value, 22)
                ZK28 = CuttingData(Value, 22)
                ZK29 = CuttingData(Value, 22)
                ZK30 = CuttingData(Value, 15)
                ZK31 = CuttingData(Value, 14)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC2 As ZEIKIN350_RECORD2

    ' �g���[�����R�[�h
    Structure ZEIKIN350_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String   ' �f�[�^�敪(=8)
        <VBFixedString(6)> Public ZK2 As String   ' ���v����
        <VBFixedString(12)> Public ZK3 As String  ' ���v���z
        <VBFixedString(6)> Public ZK4 As String   ' �U�֍ό���
        <VBFixedString(12)> Public ZK5 As String  ' �U�֍ϋ��z
        <VBFixedString(6)> Public ZK6 As String   ' �U�֕s�\����
        <VBFixedString(12)> Public ZK7 As String  ' �U�֕s�\���z
        <VBFixedString(295)> Public ZK8 As String ' �_�~�[
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 6), _
                            SubData(ZK3, 12), _
                            SubData(ZK4, 6), _
                            SubData(ZK5, 12), _
                            SubData(ZK6, 6), _
                            SubData(ZK7, 12), _
                            SubData(ZK8, 295) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 6)
                ZK3 = CuttingData(Value, 12)
                ZK4 = CuttingData(Value, 6)
                ZK5 = CuttingData(Value, 12)
                ZK6 = CuttingData(Value, 6)
                ZK7 = CuttingData(Value, 12)
                ZK8 = CuttingData(Value, 295)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC8 As ZEIKIN350_RECORD8

    Structure ZEIKIN350_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZK1 As String     ' �f�[�^�敪(=9)
        <VBFixedString(349)> Public ZK2 As String   ' �_�~�[
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(ZK1, 1), _
                            SubData(ZK2, 349) _
                            })
            End Get
            Set(ByVal Value As String)
                ZK1 = CuttingData(Value, 1)
                ZK2 = CuttingData(Value, 349)
            End Set
        End Property
    End Structure
    Public ZEIKIN350_REC9 As ZEIKIN350_RECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "350.P"
        FtranIBMPfile = "350IBM.P"

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

        Dim RD() As Byte = EncdJ.GetBytes(RecordData)

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' �w�b�_
                buff.Append(EncdJ.GetString(RD, 0, 19))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 19, 35), -1)) '�ϑ��Җ�
                buff.Append(EncdJ.GetString(RD, 54, 10))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 64, 15), -1)) '��s��
                buff.Append(EncdJ.GetString(RD, 79, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 82, 15), -1)) '�x�X��
                buff.Append(EncdJ.GetString(RD, 97, 253))

            Case "2"        ' �f�[�^
                buff.Append(EncdJ.GetString(RD, 0, 5))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 5, 15), -1)) '��s��
                buff.Append(EncdJ.GetString(RD, 20, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 23, 15), -1)) '�x�X��
                buff.Append(EncdJ.GetString(RD, 38, 12))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30), -1)) '�������`�l
                buff.Append(EncdJ.GetString(RD, 80, 270))

            Case "8"        ' �g���[��
                buff.Append(ReplaceString(EncdJ.GetString(RD, 0, 350), -1))

            Case "9"        ' �G���h
                buff.Append(ReplaceString(EncdJ.GetString(RD, 0, 350), -1))
        End Select

        mRecordData = buff.ToString

        Return -1
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
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord1()
                    End If
                End If
            Case "2"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
                    DataInfo.Message = "�t�@�C�����R�[�h�i�f�[�^�敪�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else

                    sRet = CheckRecord2()
                    If sRet <> "ERR" Then
                        sRet = CheckDBRecord2()
                    End If
                End If
            Case "8"
                If BeforeRecKbn <> "1" And BeforeRecKbn <> "2" Then
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
        ZEIKIN350_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC1.ZK1
        InfoMeisaiMast.SYUBETU_CODE = ZEIKIN350_REC1.ZK2
        InfoMeisaiMast.ITAKU_CODE = ZEIKIN350_REC1.ZK4 & ZEIKIN350_REC1.ZK5
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZEIKIN350_REC1.ZK9 & ZEIKIN350_REC1.ZK10, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = ZEIKIN350_REC1.ZK9 & ZEIKIN350_REC1.ZK10
        InfoMeisaiMast.ITAKU_KIN = ZEIKIN350_REC1.ZK11
        InfoMeisaiMast.ITAKU_SIT = ZEIKIN350_REC1.ZK13
        InfoMeisaiMast.ITAKU_KAMOKU = ZEIKIN350_REC1.ZK15
        InfoMeisaiMast.ITAKU_KOUZA = ZEIKIN350_REC1.ZK16
        InfoMeisaiMast.ITAKU_KNAME = ZEIKIN350_REC1.ZK7

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        Return "H"
    End Function

    Private Function CheckDBRecord1() As String
        ' �f�[�^�`�F�b�N
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
        ZEIKIN350_REC2.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC2.ZK1
        InfoMeisaiMast.KEIYAKU_KIN = ZEIKIN350_REC2.ZK2
        InfoMeisaiMast.KEIYAKU_SIT = ZEIKIN350_REC2.ZK4
        InfoMeisaiMast.KEIYAKU_KAMOKU = ZEIKIN350_REC2.ZK7
        InfoMeisaiMast.KEIYAKU_KOUZA = ZEIKIN350_REC2.ZK8
        InfoMeisaiMast.KEIYAKU_KNAME = ZEIKIN350_REC2.ZK9.Trim
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(ZEIKIN350_REC2.ZK10)
        InfoMeisaiMast.FURIKIN_MOTO = ZEIKIN350_REC2.ZK10
        InfoMeisaiMast.SINKI_CODE = ZEIKIN350_REC2.ZK11
        '*** �C�� mitsu 2008/06/26 �K��O�����Ή� ***
        'InfoMeisaiMast.KEIYAKU_NO = ZEIKIN350_REC2.ZK21.Substring(0, 10)
        '*** �C�� mitsu 2008/08/05 ZK21�̒l��ێ����� ***
        'InfoMeisaiMast.KEIYAKU_NO = CASTCommon.Cutting(ZEIKIN350_REC2.ZK21, 10)
        Dim zk21 As String = ZEIKIN350_REC2.ZK21
        ZEIKIN350_REC2.ZK21 = CASTCommon.Cutting(zk21, 11)
        '*** �C�� mitsu 2008/08/26 10byte�ɂȂ�悤�C�� ***
        zk21 = ZEIKIN350_REC2.ZK21
        InfoMeisaiMast.KEIYAKU_NO = CASTCommon.Cutting(zk21, 10)
        '**************************************************
        '************************************************
        '********************************************
        InfoMeisaiMast.YOBI1 = ZEIKIN350_REC2.ZK21
        InfoMeisaiMast.JYUYOKA_NO = ZEIKIN350_REC2.ZK16

        InfoMeisaiMast.KTEKIYO = ""
        InfoMeisaiMast.NTEKIYO = ""

        ' �E�v
        If (Not mInfoComm Is Nothing) Then
            Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                Case "0"
                    InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                    InfoMeisaiMast.NTEKIYO = ""
                Case "1"
                    InfoMeisaiMast.KTEKIYO = ""
                    InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                Case "2"
                    InfoMeisaiMast.KTEKIYO = ZEIKIN350_REC2.ZK5.PadRight(13, " "c).Substring(0, 13).Trim
                Case "3"
                    InfoMeisaiMast.KTEKIYO = ZEIKIN350_REC2.ZK5
            End Select
        End If

        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(ZEIKIN350_REC2.ZK12)
        InfoMeisaiMast.FURIKETU_MOTO = ZEIKIN350_REC2.ZK12

        '*** �C�� maeda 2008/05/12************************************************
        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ZEIKIN350_REC2.ZK3
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ZEIKIN350_REC2.ZK5
        '*************************************************************************

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        Return "D"
    End Function

    Private Function CheckDBRecord2() As String
        ' �f�[�^�`�F�b�N
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
        ZEIKIN350_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC8.ZK1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK4)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK5)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK6)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(ZEIKIN350_REC8.ZK7)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = ZEIKIN350_REC8.ZK2
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = ZEIKIN350_REC8.ZK3
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = ZEIKIN350_REC8.ZK4
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = ZEIKIN350_REC8.ZK5
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = ZEIKIN350_REC8.ZK6
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = ZEIKIN350_REC8.ZK7

        Return "T"
    End Function

    Private Function CheckDBRecord8() As String

        ' �f�[�^�`�F�b�N
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
        ZEIKIN350_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZEIKIN350_REC9.ZK1

        Return "E"
    End Function

    ' �@�\�@ �F �Ԋ҃f�[�^���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overrides Sub GetHenkanDataRecord()
        If IsDataRecord() = False Then
            Return
        End If

        ZEIKIN350_REC2.Data = RecordData

        ' �U�֌��ʂ��Z�b�g
        ZEIKIN350_REC2.ZK12 = InfoMeisaiMast.FURIKETU_KEKKA

        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
        '�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(91, ZEIKIN350_REC2.ZK12)
        End If
        '**********************************************

        RecordData = ZEIKIN350_REC2.Data

        ' ���R�[�h�f�[�^�𕪐�
        Call CheckRecord2()

        Call MyBase.GetHenkanDataRecord()
    End Sub

    ' �@�\�@ �F �Ԋ҃g���[�����R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overrides Sub GetHenkanTrailerRecord()
        If IsTrailerRecord() = False Then
            Return
        End If

        ' ���R�[�h�f�[�^�𕪐�
        Call CheckRecord8()

        ' �U�֍ς݌������Z�b�g
        ZEIKIN350_REC8.ZK4 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, "0"c)
        ZEIKIN350_REC8.ZK5 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        ' �U�֕s�\�������Z�b�g
        ZEIKIN350_REC8.ZK6 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
        ZEIKIN350_REC8.ZK7 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)

        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
        '�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(19, ZEIKIN350_REC8.ZK4)
            ReadByteBin.Insert(25, ZEIKIN350_REC8.ZK5)
            ReadByteBin.Insert(37, ZEIKIN350_REC8.ZK6)
            ReadByteBin.Insert(43, ZEIKIN350_REC8.ZK7)
        End If
        '**********************************************

        RecordData = ZEIKIN350_REC8.Data

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
End Class
