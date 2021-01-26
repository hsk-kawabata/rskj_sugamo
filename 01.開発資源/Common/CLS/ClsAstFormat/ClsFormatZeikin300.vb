Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' �n���́i�R�O�O�j �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatZeikin300
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 300

    ' ------------------------------------------
    ' 300�o�C�g�t�H�[�}�b�g(����s��)
    ' ------------------------------------------
    ' �w�b�_�[���R�[�h
    Structure SHIZEI_RECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public SZ1 As String     ' �f�[�^�敪(=1)
        <VBFixedString(2)> Public SZ2 As String     ' ��ʃR�[�h
        <VBFixedString(1)> Public SZ3 As String     ' �R�[�h�敪
        <VBFixedString(10)> Public SZ4 As String    ' �ϑ��҃R�[�h
        <VBFixedString(40)> Public SZ5 As String    ' �ϑ��Җ�
        <VBFixedString(6)> Public SZ6 As String     ' �U�֓�(�a��)
        <VBFixedString(4)> Public SZ7 As String     ' �����s�ԍ�
        <VBFixedString(15)> Public SZ8 As String    ' �����s��
        <VBFixedString(3)> Public SZ9 As String     ' ����x�X�ԍ�
        <VBFixedString(15)> Public SZ10 As String   ' ����x�X��
        <VBFixedString(1)> Public SZ11 As String    ' �a�����
        <VBFixedString(7)> Public SZ12 As String    ' �����ԍ�
        <VBFixedString(195)> Public SZ13 As String  ' �_�~�[
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SZ1, 1), _
                            SubData(SZ2, 2), _
                            SubData(SZ3, 1), _
                            SubData(SZ4, 10), _
                            SubData(SZ5, 40), _
                            SubData(SZ6, 6), _
                            SubData(SZ7, 4), _
                            SubData(SZ8, 15), _
                            SubData(SZ9, 3), _
                            SubData(SZ10, 15), _
                            SubData(SZ11, 1), _
                            SubData(SZ12, 7), _
                            SubData(SZ13, 195) _
                            })
            End Get
            Set(ByVal Value As String)
                SZ1 = CuttingData(Value, 1)
                SZ2 = CuttingData(Value, 2)
                SZ3 = CuttingData(Value, 1)
                SZ4 = CuttingData(Value, 10)
                SZ5 = CuttingData(Value, 40)
                SZ6 = CuttingData(Value, 6)
                SZ7 = CuttingData(Value, 4)
                SZ8 = CuttingData(Value, 15)
                SZ9 = CuttingData(Value, 3)
                SZ10 = CuttingData(Value, 15)
                SZ11 = CuttingData(Value, 1)
                SZ12 = CuttingData(Value, 7)
                SZ13 = CuttingData(Value, 195)
            End Set
        End Property
    End Structure
    Public SHIZEI_REC1 As SHIZEI_RECORD1

    ' �f�[�^���R�[�h
    Structure SHIZEI_RECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public SZ1 As String     ' �f�[�^�敪(=2)
        <VBFixedString(4)> Public SZ2 As String     ' ������s�ԍ�
        <VBFixedString(15)> Public SZ3 As String    ' ������s��
        <VBFixedString(3)> Public SZ4 As String     ' �����x�X�ԍ�
        <VBFixedString(15)> Public SZ5 As String    ' �����x�X��
        <VBFixedString(4)> Public SZ6 As String     ' �_�~�[
        <VBFixedString(1)> Public SZ7 As String     ' �a�����
        <VBFixedString(7)> Public SZ8 As String     ' �a�������ԍ�
        <VBFixedString(30)> Public SZ9 As String    ' �a���Җ�
        <VBFixedString(10)> Public SZ10 As String   ' �������z
        <VBFixedString(1)> Public SZ11 As String    ' �V�K�R�[�h
        <VBFixedString(1)> Public SZ12 As String    ' �U�֌��ʃR�[�h
        <VBFixedString(8)> Public SZ13 As String    ' �_�~�[
        <VBFixedString(20)> Public SZ14 As String   ' ���v�Ɣԍ�(�V)
        <VBFixedString(20)> Public SZ15 As String   ' ���v�Ɣԍ�(��)
        <VBFixedString(8)> Public SZ16 As String    ' ����v�Z�N��
        <VBFixedString(6)> Public SZ17 As String    ' �g�p����(�`)
        <VBFixedString(6)> Public SZ18 As String    ' �g�p����(�a)
        <VBFixedString(6)> Public SZ19 As String    ' ���̑�
        <VBFixedString(8)> Public SZ20 As String    ' �ʐ���g�p����
        <VBFixedString(8)> Public SZ21 As String    ' �㐅������
        <VBFixedString(8)> Public SZ22 As String    ' ����������
        <VBFixedString(8)> Public SZ23 As String    ' ���̑�����
        <VBFixedString(30)> Public SZ24 As String   ' ���v�Ɩ�
        <VBFixedString(5)> Public SZ25 As String    ' �X�֔ԍ�
        <VBFixedString(53)> Public SZ26 As String   ' �Z��(�s��23���{����22��+�Ԓn8��)
        <VBFixedString(14)> Public SZ27 As String   ' �_�~�[
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SZ1, 1), _
                            SubData(SZ2, 4), _
                            SubData(SZ3, 15), _
                            SubData(SZ4, 3), _
                            SubData(SZ5, 15), _
                            SubData(SZ6, 4), _
                            SubData(SZ7, 1), _
                            SubData(SZ8, 7), _
                            SubData(SZ9, 30), _
                            SubData(SZ10, 10), _
                            SubData(SZ11, 1), _
                            SubData(SZ12, 1), _
                            SubData(SZ13, 8), _
                            SubData(SZ14, 20), _
                            SubData(SZ15, 20), _
                            SubData(SZ16, 8), _
                            SubData(SZ17, 6), _
                            SubData(SZ18, 6), _
                            SubData(SZ19, 6), _
                            SubData(SZ20, 8), _
                            SubData(SZ21, 8), _
                            SubData(SZ22, 8), _
                            SubData(SZ23, 8), _
                            SubData(SZ24, 30), _
                            SubData(SZ25, 5), _
                            SubData(SZ26, 53), _
                            SubData(SZ27, 14) _
                            })
            End Get
            Set(ByVal Value As String)
                SZ1 = CuttingData(Value, 1)
                SZ2 = CuttingData(Value, 4)
                SZ3 = CuttingData(Value, 15)
                SZ4 = CuttingData(Value, 3)
                SZ5 = CuttingData(Value, 15)
                SZ6 = CuttingData(Value, 4)
                SZ7 = CuttingData(Value, 1)
                SZ8 = CuttingData(Value, 7)
                SZ9 = CuttingData(Value, 30)
                SZ10 = CuttingData(Value, 10)
                SZ11 = CuttingData(Value, 1)
                SZ12 = CuttingData(Value, 1)
                SZ13 = CuttingData(Value, 8)
                SZ14 = CuttingData(Value, 20)
                SZ15 = CuttingData(Value, 20)
                SZ16 = CuttingData(Value, 8)
                SZ17 = CuttingData(Value, 6)
                SZ18 = CuttingData(Value, 6)
                SZ19 = CuttingData(Value, 6)
                SZ20 = CuttingData(Value, 8)
                SZ21 = CuttingData(Value, 8)
                SZ22 = CuttingData(Value, 8)
                SZ23 = CuttingData(Value, 8)
                SZ24 = CuttingData(Value, 30)
                SZ25 = CuttingData(Value, 5)
                SZ26 = CuttingData(Value, 53)
                SZ27 = CuttingData(Value, 14)
            End Set
        End Property
    End Structure
    Public SHIZEI_REC2 As SHIZEI_RECORD2

    ' �g���[�����R�[�h
    Structure SHIZEI_RECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public SZ1 As String   ' �f�[�^�敪(=8)
        <VBFixedString(6)> Public SZ2 As String   ' ���v����
        <VBFixedString(12)> Public SZ3 As String  ' ���v���z
        <VBFixedString(6)> Public SZ4 As String   ' �U�֍ό���
        <VBFixedString(12)> Public SZ5 As String  ' �U�֍ϋ��z
        <VBFixedString(6)> Public SZ6 As String   ' �U�֕s�\����
        <VBFixedString(12)> Public SZ7 As String  ' �U�֕s�\���z
        <VBFixedString(245)> Public SZ8 As String ' �_�~�[
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SZ1, 1), _
                            SubData(SZ2, 6), _
                            SubData(SZ3, 12), _
                            SubData(SZ4, 6), _
                            SubData(SZ5, 12), _
                            SubData(SZ6, 6), _
                            SubData(SZ7, 12), _
                            SubData(SZ8, 245) _
                            })
            End Get
            Set(ByVal Value As String)
                SZ1 = CuttingData(Value, 1)
                SZ2 = CuttingData(Value, 6)
                SZ3 = CuttingData(Value, 12)
                SZ4 = CuttingData(Value, 6)
                SZ5 = CuttingData(Value, 12)
                SZ6 = CuttingData(Value, 6)
                SZ7 = CuttingData(Value, 12)
                SZ8 = CuttingData(Value, 245)
            End Set
        End Property
    End Structure
    Public SHIZEI_REC8 As SHIZEI_RECORD8

    Structure SHIZEI_RECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public SZ1 As String   ' �f�[�^�敪(=9)
        <VBFixedString(299)> Public SZ2 As String   ' �_�~�[
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(SZ1, 1), _
                            SubData(SZ2, 299) _
                            })
            End Get
            Set(ByVal Value As String)
                SZ1 = CuttingData(Value, 1)
                SZ2 = CuttingData(Value, 299)
            End Set
        End Property
    End Structure
    Public SHIZEI_REC9 As SHIZEI_RECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "8", "9"}

        FtranPfile = "300.P"
        FtranIBMPfile = "300IBM.P"

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
        '*** �C�� mitsu 2008/06/03 ���������� ***
        'Select Case RecordData.Substring(0, 1)
        '    Case "1"        ' �w�b�_
        '        buff.Append(EncdJ.GetString(RD, 0, 14))
        '        buff.Append(ReplaceString(EncdJ.GetString(RD, 14, 40)))
        '        buff.Append(EncdJ.GetString(RD, 54, 246))
        '        If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
        '            mRecordData = buff.ToString(0, RecordLen)
        '        End If
        '    Case "2"        ' �f�[�^
        '        buff.Append(EncdJ.GetString(RD, 0, 50))
        '        buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30)))
        '        buff.Append(EncdJ.GetString(RD, 80, 220))
        '        If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
        '            mRecordData = buff.ToString(0, RecordLen)
        '        End If

        '        ' �K�蕶���`�F�b�N
        '        nRet = CheckRegularStringVerB(RecordData, 0, 198)
        '        If nRet >= 0 Then
        '            Return nRet
        '        End If
        '    Case "8"        ' �g���[��
        '        buff.Append(RecordData.Substring(0))
        '        If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
        '            mRecordData = buff.ToString(0, RecordLen)
        '        End If
        '    Case "9"        ' �G���h
        '        buff.Append(RecordData.Substring(0))
        '        If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
        '            mRecordData = buff.ToString(0, RecordLen)
        '        End If
        'End Select

        'Return MyBase.CheckRegularString()

        Select Case RecordData.Substring(0, 1)
            Case "1"        ' �w�b�_
                buff.Append(EncdJ.GetString(RD, 0, 14))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 14, 40), -1)) '�ϑ��Җ�
                buff.Append(EncdJ.GetString(RD, 54, 10))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 64, 15), -1)) '��s��
                buff.Append(EncdJ.GetString(RD, 79, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 82, 15), -1)) '�x�X��
                buff.Append(EncdJ.GetString(RD, 97, 203))

            Case "2"        ' �f�[�^
                buff.Append(EncdJ.GetString(RD, 0, 5))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 5, 15), -1)) '���Z�@�֖�
                buff.Append(EncdJ.GetString(RD, 20, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 23, 15), -1)) '�x�X��
                buff.Append(EncdJ.GetString(RD, 38, 12))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30), -1)) '�a���Җ�
                buff.Append(EncdJ.GetString(RD, 80, 220))

            Case "8"        ' �g���[��
                buff.Append(ReplaceString(EncdJ.GetString(RD, 0, 300), -1))

            Case "9"        ' �G���h
                buff.Append(ReplaceString(EncdJ.GetString(RD, 0, 300), -1))
        End Select

        mRecordData = buff.ToString

        Return -1
        '****************************************
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
        SHIZEI_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = SHIZEI_REC1.SZ1
        InfoMeisaiMast.SYUBETU_CODE = SHIZEI_REC1.SZ2
        InfoMeisaiMast.ITAKU_CODE = SHIZEI_REC1.SZ4
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(SHIZEI_REC1.SZ6.Substring(2, 4), "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = SHIZEI_REC1.SZ6.Substring(2, 4)
        InfoMeisaiMast.ITAKU_KIN = SHIZEI_REC1.SZ7
        InfoMeisaiMast.ITAKU_SIT = SHIZEI_REC1.SZ9
        InfoMeisaiMast.ITAKU_KAMOKU = SHIZEI_REC1.SZ11
        InfoMeisaiMast.ITAKU_KOUZA = SHIZEI_REC1.SZ12
        InfoMeisaiMast.ITAKU_KNAME = SHIZEI_REC1.SZ5

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
        SHIZEI_REC2.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = SHIZEI_REC2.SZ1
        InfoMeisaiMast.KEIYAKU_KIN = SHIZEI_REC2.SZ2
        InfoMeisaiMast.KEIYAKU_SIT = SHIZEI_REC2.SZ4
        InfoMeisaiMast.KEIYAKU_KAMOKU = SHIZEI_REC2.SZ7
        InfoMeisaiMast.KEIYAKU_KOUZA = SHIZEI_REC2.SZ8
        InfoMeisaiMast.KEIYAKU_KNAME = SHIZEI_REC2.SZ9.Trim
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(SHIZEI_REC2.SZ10)
        InfoMeisaiMast.FURIKIN_MOTO = SHIZEI_REC2.SZ10
        InfoMeisaiMast.SINKI_CODE = SHIZEI_REC2.SZ11
        If SHIZEI_REC2.SZ14.Trim.Length > 10 Then
            InfoMeisaiMast.KEIYAKU_NO = SHIZEI_REC2.SZ14.Substring(0, 10) ' �ڋq�ԍ�(�V)�̏�10���̂݃Z�b�g(�b��Ή�)
        Else
            InfoMeisaiMast.KEIYAKU_NO = SHIZEI_REC2.SZ14 ' �ڋq�ԍ�(�V)�Z�b�g(�b��Ή�)
        End If
        InfoMeisaiMast.JYUYOKA_NO = SHIZEI_REC2.SZ14 ' �ڋq�ԍ�(�V)�Z�b�g

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
                    InfoMeisaiMast.KTEKIYO = SHIZEI_REC2.SZ5.PadRight(13, " "c).Substring(0, 13).Trim
                Case "3"
                    InfoMeisaiMast.KTEKIYO = SHIZEI_REC2.SZ5
            End Select
        End If

        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(SHIZEI_REC2.SZ12)
        InfoMeisaiMast.FURIKETU_MOTO = SHIZEI_REC2.SZ12

        '*** �C�� maeda 2008/05/12************************************************
        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = SHIZEI_REC2.SZ3
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = SHIZEI_REC2.SZ5
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
        SHIZEI_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = SHIZEI_REC8.SZ1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(SHIZEI_REC8.SZ2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(SHIZEI_REC8.SZ3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(SHIZEI_REC8.SZ4)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(SHIZEI_REC8.SZ5)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(SHIZEI_REC8.SZ6)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(SHIZEI_REC8.SZ7)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = SHIZEI_REC8.SZ2
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = SHIZEI_REC8.SZ3
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = SHIZEI_REC8.SZ4
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = SHIZEI_REC8.SZ5
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = SHIZEI_REC8.SZ6
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = SHIZEI_REC8.SZ7

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
        SHIZEI_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = SHIZEI_REC9.SZ1

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

        SHIZEI_REC2.Data = RecordData

        ' �U�֌��ʂ��Z�b�g
        SHIZEI_REC2.SZ12 = InfoMeisaiMast.FURIKETU_KEKKA

        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
        '�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(91, SHIZEI_REC2.SZ12)
        End If
        '**********************************************

        RecordData = SHIZEI_REC2.Data

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
        SHIZEI_REC8.SZ4 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, "0"c)
        SHIZEI_REC8.SZ5 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        ' �U�֕s�\�������Z�b�g
        SHIZEI_REC8.SZ6 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
        SHIZEI_REC8.SZ7 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)

        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή� ***
        '�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(19, SHIZEI_REC8.SZ4)
            ReadByteBin.Insert(25, SHIZEI_REC8.SZ5)
            ReadByteBin.Insert(37, SHIZEI_REC8.SZ6)
            ReadByteBin.Insert(43, SHIZEI_REC8.SZ7)
        End If
        '**********************************************

        RecordData = SHIZEI_REC8.Data

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
End Class
