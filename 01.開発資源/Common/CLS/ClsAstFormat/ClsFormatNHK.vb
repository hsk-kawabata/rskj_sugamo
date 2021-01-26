Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' �m�g�j �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatNHK
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 120

    '------------------------------------------
    '�m�g�j�t�H�[�}�b�g
    '------------------------------------------
    '�w�b�_���R�[�h
    Public Structure NHKRECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    ' �f�[�^�敪(=1)
        <VBFixedString(2)> Public NH02 As String    ' ��ʃR�[�h
        <VBFixedString(1)> Public NH03 As String    ' �R�[�h�敪
        <VBFixedString(10)> Public NH04 As String   ' �ϑ��҃R�[�h
        <VBFixedString(40)> Public NH05 As String   ' �ϑ��Җ�
        <VBFixedString(4)> Public NH06 As String    ' �U�֌���
        <VBFixedString(4)> Public NH07 As String    ' ���Z�@�փR�[�h
        <VBFixedString(15)> Public NH08 As String   ' �_�~�[
        <VBFixedString(3)> Public NH09 As String    ' �_�~�[
        <VBFixedString(15)> Public NH10 As String   ' �_�~�[
        <VBFixedString(8)> Public NH11 As String    ' �_�~�[
        <VBFixedString(9)> Public NH12 As String    ' �_�~�[
        <VBFixedString(3)> Public NH13 As String    ' �_�~�[
        <VBFixedString(4)> Public NH14 As String    ' �����@�փR�[�h
        <VBFixedString(1)> Public NH15 As String    ' �A�X�^���X�N
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 2), _
                            SubData(NH03, 1), _
                            SubData(NH04, 10), _
                            SubData(NH05, 40), _
                            SubData(NH06, 4), _
                            SubData(NH07, 4), _
                            SubData(NH08, 15), _
                            SubData(NH09, 3), _
                            SubData(NH10, 15), _
                            SubData(NH11, 8), _
                            SubData(NH12, 9), _
                            SubData(NH13, 3), _
                            SubData(NH14, 4), _
                            SubData(NH15, 1) _
                            })
            End Get
            Set(ByVal value As String)
                NH01 = CuttingData(value, 1)
                NH02 = CuttingData(value, 2)
                NH03 = CuttingData(value, 1)
                NH04 = CuttingData(value, 10)
                NH05 = CuttingData(value, 40)
                NH06 = CuttingData(value, 4)
                NH07 = CuttingData(value, 4)
                NH08 = CuttingData(value, 15)
                NH09 = CuttingData(value, 3)
                NH10 = CuttingData(value, 15)
                NH11 = CuttingData(value, 8)
                NH12 = CuttingData(value, 9)
                NH13 = CuttingData(value, 3)
                NH14 = CuttingData(value, 4)
                NH15 = CuttingData(value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC1 As NHKRECORD1

    '�f�[�^���R�[�h
    Structure NHKRECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    ' �f�[�^�敪(=2)
        <VBFixedString(4)> Public NH02 As String    ' ������Z�@�փR�[�h
        <VBFixedString(15)> Public NH03 As String   ' �_�~�[�G���A
        <VBFixedString(3)> Public NH04 As String    ' ������Z�@�֓X�܃R�[�h
        <VBFixedString(1)> Public NH05 As String    ' �V�����ԍ�    �Ȗ�
        <VBFixedString(7)> Public NH06 As String    '               �ʒ��ԍ�
        <VBFixedString(11)> Public NH07 As String   ' �_�~�[
        <VBFixedString(1)> Public NH08 As String    ' �����ԍ�      �Ȗ�
        <VBFixedString(7)> Public NH09 As String    '               �ʒ��ԍ�
        <VBFixedString(10)> Public NH10 As String   ' �a���Җ�      �ʒ���
        <VBFixedString(5)> Public NH11 As String    '               �_�~�[
        <VBFixedString(15)> Public NH12 As String   ' �_��Җ�
        <VBFixedString(10)> Public NH13 As String   ' �������z
        <VBFixedString(1)> Public NH14 As String    ' �ύX����
        <VBFixedString(4)> Public NH15 As String    ' �ǁE�n��ԍ�  ��
        <VBFixedString(7)> Public NH16 As String    '               �n��ԍ�
        <VBFixedString(7)> Public NH17 As String    ' ���s�ԍ�
        <VBFixedString(2)> Public NH18 As String    ' �_�~�[
        <VBFixedString(1)> Public NH19 As String    ' �U�֕s�\���R
        <VBFixedString(3)> Public NH20 As String    ' �_�~�[
        <VBFixedString(4)> Public NH21 As String    ' �����@�փR�[�h
        <VBFixedString(1)> Public NH22 As String    ' �A�X�^���X�N
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 4), _
                            SubData(NH03, 15), _
                            SubData(NH04, 3), _
                            SubData(NH05, 1), _
                            SubData(NH06, 7), _
                            SubData(NH07, 11), _
                            SubData(NH08, 1), _
                            SubData(NH09, 7), _
                            SubData(NH10, 10), _
                            SubData(NH11, 5), _
                            SubData(NH12, 15), _
                            SubData(NH13, 10), _
                            SubData(NH14, 1), _
                            SubData(NH15, 4), _
                            SubData(NH16, 7), _
                            SubData(NH17, 7), _
                            SubData(NH18, 2), _
                            SubData(NH19, 1), _
                            SubData(NH20, 3), _
                            SubData(NH21, 4), _
                            SubData(NH22, 1) _
                            })
            End Get
            Set(ByVal Value As String)
                NH01 = CuttingData(Value, 1)
                NH02 = CuttingData(Value, 4)
                NH03 = CuttingData(Value, 15)
                NH04 = CuttingData(Value, 3)
                NH05 = CuttingData(Value, 1)
                NH06 = CuttingData(Value, 7)
                NH07 = CuttingData(Value, 11)
                NH08 = CuttingData(Value, 1)
                NH09 = CuttingData(Value, 7)
                NH10 = CuttingData(Value, 10)
                NH11 = CuttingData(Value, 5)
                NH12 = CuttingData(Value, 15)
                NH13 = CuttingData(Value, 10)
                NH14 = CuttingData(Value, 1)
                NH15 = CuttingData(Value, 4)
                NH16 = CuttingData(Value, 7)
                NH17 = CuttingData(Value, 7)
                NH18 = CuttingData(Value, 2)
                NH19 = CuttingData(Value, 1)
                NH20 = CuttingData(Value, 3)
                NH21 = CuttingData(Value, 4)
                NH22 = CuttingData(Value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC2 As NHKRECORD2

    '�g���[�����R�[�h
    Structure NHKRECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    ' �f�[�^�敪(=8)
        <VBFixedString(6)> Public NH02 As String    ' ��������
        <VBFixedString(12)> Public NH03 As String   ' �������z
        <VBFixedString(6)> Public NH04 As String    ' �U�֍ό���
        <VBFixedString(12)> Public NH05 As String   ' �U�֍ϋ��z
        <VBFixedString(6)> Public NH06 As String    ' �U�֕s�\����
        <VBFixedString(12)> Public NH07 As String   ' �U�֕s�\���z
        <VBFixedString(12)> Public NH08 As String   ' �U�֎萔��
        <VBFixedString(12)> Public NH09 As String   ' �����z
        <VBFixedString(6)> Public NH10 As String    ' ���R�[�h��
        <VBFixedString(6)> Public NH11 As String    ' �ύX���ʌ�������  �L���z
        <VBFixedString(6)> Public NH12 As String    '                   ���z�u0�v
        <VBFixedString(15)> Public NH13 As String   ' �_�~�[
        <VBFixedString(3)> Public NH14 As String    ' �g���[������
        <VBFixedString(4)> Public NH15 As String    ' �\��
        <VBFixedString(1)> Public NH16 As String    ' �A�X�^���X�N
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 6), _
                            SubData(NH03, 12), _
                            SubData(NH04, 6), _
                            SubData(NH05, 12), _
                            SubData(NH06, 6), _
                            SubData(NH07, 12), _
                            SubData(NH08, 12), _
                            SubData(NH09, 12), _
                            SubData(NH10, 6), _
                            SubData(NH11, 6), _
                            SubData(NH12, 6), _
                            SubData(NH13, 15), _
                            SubData(NH14, 3), _
                            SubData(NH15, 4), _
                            SubData(NH16, 1) _
                            })
            End Get
            Set(ByVal Value As String)
                NH01 = CuttingData(Value, 1)
                NH02 = CuttingData(Value, 6)
                NH03 = CuttingData(Value, 12)
                NH04 = CuttingData(Value, 6)
                NH05 = CuttingData(Value, 12)
                NH06 = CuttingData(Value, 6)
                NH07 = CuttingData(Value, 12)
                NH08 = CuttingData(Value, 12)
                NH09 = CuttingData(Value, 12)
                NH10 = CuttingData(Value, 6)
                NH11 = CuttingData(Value, 6)
                NH12 = CuttingData(Value, 6)
                NH13 = CuttingData(Value, 15)
                NH14 = CuttingData(Value, 3)
                NH15 = CuttingData(Value, 4)
                NH16 = CuttingData(Value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC8 As NHKRECORD8

    '�G���h���R�[�h
    Structure NHKRECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public NH01 As String    '�f�[�^�敪(=9)
        <VBFixedString(111)> Public NH02 As String  '�_�~�[�G���A
        <VBFixedString(3)> Public NH03 As String    '�_�~�[�G���A
        <VBFixedString(4)> Public NH04 As String    '�����@�փR�[�h
        <VBFixedString(1)> Public NH05 As String    '�A�X�^���X�N
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NH01, 1), _
                            SubData(NH02, 111), _
                            SubData(NH03, 3), _
                            SubData(NH04, 4), _
                            SubData(NH05, 1) _
                            })
            End Get
            Set(ByVal Value As String)
                NH01 = CuttingData(Value, 1)
                NH02 = CuttingData(Value, 111)
                NH03 = CuttingData(Value, 3)
                NH04 = CuttingData(Value, 4)
                NH05 = CuttingData(Value, 1)
            End Set
        End Property
    End Structure
    Public NHK_REC9 As NHKRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "120.P"
        FtranIBMPfile = "120IBM.P"
        FtranIBMBinaryPfile = "120READ.P"

        CMTBlockSize = 1800

        HeaderKubun = New String() {"1"}
        DataKubun = New String() {"2"}
        TrailerKubun = New String() {"8"}
    End Sub

    '
    ' �@�\�@ �F �K�蕶���`�F�b�N ���@�����u������
    '
    ' �߂�l �F �s�������̈ʒu
    '
    ' ���l�@ �F RepaceString()�֐��ɂĕ����u�������{
    '           �u���Ώە����́C�s�������ɂ͂Ȃ�Ȃ�
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(DataInfo.LenOfOneRec)
        Dim nRet As Long
        Dim RD() As Byte = EncdJ.GetBytes(RecordData)

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' �w�b�_���R�[�h
                buff.Append(EncdJ.GetString(RD, 0, 14))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 14, 40), -1))
                buff.Append(EncdJ.GetString(RD, 54, 66))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If
            Case "2"        ' �f�[�^���R�[�h
                buff.Append(EncdJ.GetString(RD, 0, 50))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30), -1))
                buff.Append(EncdJ.GetString(RD, 80, 40))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If

                nRet = CheckRegularStringVerA(RecordData, 50, 30)
                If nRet >= 0 Then
                    Return nRet
                End If

            Case "8"        ' �g���[��
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If
            Case "9"        ' �G���h
                buff.Append(RecordData.Substring(0))
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    mRecordData = buff.ToString(0, RecordLen)
                End If
        End Select

        nRet = MyBase.CheckRegularString()
        Return nRet

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
                '�G���h���R�[�h�����������Ă�OK
                If BeforeRecKbn <> "8" Then
                    If BeforeRecKbn <> "9" Then
                        DataInfo.Message = "�t�@�C�����R�[�h�i�G���h�敪�j�ُ�"
                        mnErrorNumber = 1
                        Return "ERR"
                    Else
                        sRet = CheckRecord9()
                        sRet = "99"
                    End If
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
        NHK_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = NHK_REC1.NH01             '�f�[�^�敪
        InfoMeisaiMast.SYUBETU_CODE = NHK_REC1.NH02         '��ʃR�[�h
        InfoMeisaiMast.CODE_KBN = NHK_REC1.NH03             '�R�[�h�敪
        InfoMeisaiMast.ITAKU_CODE = NHK_REC1.NH04           '�ϑ��҃R�[�h
        InfoMeisaiMast.ITAKU_KNAME = NHK_REC1.NH05          '�ϑ��Җ�
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(NHK_REC1.NH06, "yyyyMMdd")     '�U�֌���
        InfoMeisaiMast.FURIKAE_DATE_MOTO = NHK_REC1.NH06    '�U�֌����i���f�[�^�j
        InfoMeisaiMast.ITAKU_KIN = NHK_REC1.NH07            '���Z�@�փR�[�h

        Return "H"
    End Function

    Public Function CheckDBRecord1() As String
        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        '�f�[�^�`�F�b�N
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        '�S��t�H�[�}�b�g���o���̎��A�E�v�}�X�^���݃`�F�b�N
        '�E�v�敪=�f�[�^�E�v�̏ꍇ�̂ݎ擾 2007/08/08
        'If Not mInfoComm Is Nothing AndAlso mInfoComm.INFOToriMast.FMT_KBN_T = "00" AndAlso _
        '   mInfoComm.INFOToriMast.NS_KBN_T = "9" AndAlso mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
        '    mTekiyoData = GetTEKIYOUMAST(mInfoComm.INFOToriMast.TORIS_CODE_T, mInfoComm.INFOToriMast.TORIF_CODE_T)

        '    If mTekiyoData.Length = 0 Then
        '        WriteBLog("�E�v�}�X�^", "���o�^", "�ϑ��҃R�[�h�F" & InfoMeisaiMast.ITAKU_CODE)
        '        DataInfo.Message = "�E�v�}�X�^���o�^�@�����R�[�h:" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T
        '        Return "ERR"
        '    End If
        'End If

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
        NHK_REC2.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = NHK_REC2.NH01             '�f�[�^�敪
        InfoMeisaiMast.KEIYAKU_KIN = NHK_REC2.NH02          '���Z�@�փR�[�h
        InfoMeisaiMast.KEIYAKU_SIT = NHK_REC2.NH04          '�x�X�R�[�h
        InfoMeisaiMast.KEIYAKU_KAMOKU = NHK_REC2.NH08                               '�����ԍ� �Ȗ�
        InfoMeisaiMast.KEIYAKU_KOUZA = CStr(NHK_REC2.NH09).Trim.PadLeft(7, "0"c)    '�����ԍ� �ʒ��ԍ�
        InfoMeisaiMast.KEIYAKU_KNAME = NHK_REC2.NH10                                '�a���Җ�   �ʒ���
        InfoMeisaiMast.SINKI_CODE = NHK_REC2.NH14           '�ύX���ʁ@�V�K�R�[�h�H
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(NHK_REC2.NH13)              '�������z
        InfoMeisaiMast.FURIKIN_MOTO = NHK_REC2.NH13
        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(NHK_REC2.NH19)
        InfoMeisaiMast.FURIKETU_MOTO = NHK_REC2.NH19
        InfoMeisaiMast.JYUYOKA_NO = String.Concat(New String() {NHK_REC2.NH15, NHK_REC2.NH16, NHK_REC2.NH17, NHK_REC2.NH18})    '���v�Ɣԍ��H

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ""
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ""

        Return "D"
    End Function

    Private Function CheckDBRecord2() As String
        Dim CheckRet As Boolean

        '�f�[�^�`�F�b�N
        CheckRet = MyBase.CheckDataRecord()

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
                        'Case "2"
                        '    InfoMeisaiMast.KTEKIYO = NHK_REC2.ZG5.PadRight(13, " "c).Substring(0, 13).Trim
                        'Case "3"
                        '    InfoMeisaiMast.KTEKIYO = NHK_REC2.ZG15
                End Select
            End If
        Catch ex As Exception

        End Try

        If CheckRet = False Then
            Return "IJO"
        End If

        If mInfoComm Is Nothing Then
            Return "D"
        End If

        'If mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
        '    ' �E�v�敪���C�Q�F�ϓE�v�P�i�x�X���j�̏ꍇ
        '    Dim CheckTekiyou As String
        '    CheckTekiyou = ZENGIN_REC2.ZG5.TrimEnd

        '    ' ���׃}�X�^���ڐݒ� �J�i�E�v
        '    If CheckTekiyou.Length > 13 Then
        '        InfoMeisaiMast.KTEKIYO = CheckTekiyou.Substring(0, 13)
        '    Else
        '        InfoMeisaiMast.KTEKIYO = CheckTekiyou
        '    End If
        '    Dim bFoundFlag As Boolean = False
        '    For i As Integer = 0 To mTekiyoData.Length - 1
        '        If mTekiyoData(i).Length > 0 AndAlso CheckTekiyou.IndexOf(mTekiyoData(i)) >= 0 Then
        '            bFoundFlag = True
        '            Exit For
        '        End If
        '    Next i

        '    ' ���׃}�X�^���ڐݒ� �����E�v
        '    InfoMeisaiMast.NTEKIYO = ""
        '    If bFoundFlag = False Then
        '        ' �X�Ԑ��l�ُ�
        '        Dim InError As INPUTERROR
        '        InError.ERRINFO = "�E�v " & InfoMeisaiMast.KTEKIYO
        '        InErrorArray.Add(InError)
        '        Return "IJO"
        '    End If
        'End If

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
        NHK_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = NHK_REC8.NH01
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(NHK_REC8.NH02)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(NHK_REC8.NH03)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(NHK_REC8.NH04)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(NHK_REC8.NH05)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(NHK_REC8.NH06)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(NHK_REC8.NH07)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = NHK_REC8.NH02
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = NHK_REC8.NH03
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = NHK_REC8.NH04
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = NHK_REC8.NH05
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = NHK_REC8.NH06
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = NHK_REC8.NH07

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
    '
    ' �@�\�@ �F �G���h���R�[�h�`�F�b�N
    '
    ' �߂�l �F True - �����C False - ���s
    '
    ' ���l�@ �F
    '
    Protected Function CheckRecord9() As String
        NHK_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = NHK_REC9.NH01

        '�f�[�^�`�F�b�N
        If MyBase.CheckEndRecord() = False Then
            Return "ERR"
        End If

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

        NHK_REC2.Data = RecordData

        ' �ύX���ʂ��Z�b�g
        NHK_REC2.NH19 = InfoMeisaiMast.FURIKETU_KEKKA

        ' EBCDIC�f�[�^�Ή� 
        '�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, NHK_REC2.NH19)
        End If

        RecordData = NHK_REC2.Data

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
        '2010.03.02 start   'NHK�Ή�
        NHK_REC8.NH04 = InfoMeisaiMast.TOTAL_NORM_KEN2.ToString.PadLeft(6, "0"c)
        'NHK_REC8.NH04 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, "0"c)
        '2010.03.02 end
        NHK_REC8.NH05 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        ' �U�֕s�\�������Z�b�g
        NHK_REC8.NH06 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
        NHK_REC8.NH07 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)

        '2010/03/02 �U�֎萔���E�����z��ݒ肷��
        Dim TESUU As Double = 0
        Dim TANKA As Double
        Dim NHK As String = CASTCommon.GetFSKJIni("COMMON", "NHKTESUU").ToUpper
        Select Case NHK
            Case "ERR", Nothing
                TANKA = 10.5
            Case Else
                TANKA = CDbl(NHK)
        End Select
        TESUU = Math.Floor(TANKA * InfoMeisaiMast.TOTAL_NORM_KEN2)
        ' �U�֎萔�����Z�b�g
        NHK_REC8.NH08 = TESUU.ToString.PadLeft(12, "0"c)
        '�����z���Z�b�g
        If InfoMeisaiMast.TOTAL_NORM_KIN - TESUU >= 0 Then
            NHK_REC8.NH09 = (InfoMeisaiMast.TOTAL_NORM_KIN - TESUU).ToString.PadLeft(12, "0"c)
        Else
            '�萔�����U�֋��z�����傫���ꍇ�̉��Ή�
            NHK_REC8.NH09 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        End If
        '=============================================

        ' EBCDIC�f�[�^�Ή�
        '�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(19, NHK_REC8.NH04)
            ReadByteBin.Insert(25, NHK_REC8.NH05)
            ReadByteBin.Insert(37, NHK_REC8.NH06)
            ReadByteBin.Insert(43, NHK_REC8.NH07)
            '2010/03/02 �U�֎萔���E�����z��ݒ肷��
            ReadByteBin.Insert(55, NHK_REC8.NH08)
            ReadByteBin.Insert(67, NHK_REC8.NH09)
            '======================================
        End If

        RecordData = NHK_REC8.Data

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
End Class
