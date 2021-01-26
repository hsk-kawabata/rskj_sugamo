Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic

' �S�� �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatZengin
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 120

    '------------------------------------------
    '�S��t�H�[�}�b�g
    '------------------------------------------
    '�w�b�_���R�[�h
    Public Structure ZGRECORD1
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    ' �f�[�^�敪(=1)
        <VBFixedString(2)> Public ZG2 As String    ' ��ʃR�[�h
        <VBFixedString(1)> Public ZG3 As String    ' �R�[�h�敪
        <VBFixedString(10)> Public ZG4 As String   ' ��ЃR�[�h            �U���˗��l�R�[�h
        <VBFixedString(40)> Public ZG5 As String   ' �˗��l��              �U���˗��l��
        <VBFixedString(4)> Public ZG6 As String    ' �U�֎w���(�����j     �戵��
        <VBFixedString(4)> Public ZG7 As String    ' ������Z�@�փR�[�h    �d����s����
        <VBFixedString(15)> Public ZG8 As String   ' ������Z�@�֖�        �d����s��
        <VBFixedString(3)> Public ZG9 As String    ' ����X�܃R�[�h        �d���x�X����
        <VBFixedString(15)> Public ZG10 As String  ' ����X�ܖ�            �d���x�X��
        <VBFixedString(1)> Public ZG11 As String   ' ����X�ܖ����        �a�����
        <VBFixedString(7)> Public ZG12 As String   ' �˗��l�����ԍ�        �����ԍ�
        <VBFixedString(17)> Public ZG13 As String  ' �_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 2),
                            SubData(ZG3, 1),
                            SubData(ZG4, 10),
                            SubData(ZG5, 40),
                            SubData(ZG6, 4),
                            SubData(ZG7, 4),
                            SubData(ZG8, 15),
                            SubData(ZG9, 3),
                            SubData(ZG10, 15),
                            SubData(ZG11, 1),
                            SubData(ZG12, 7),
                            SubData(ZG13, 17)
                            })
            End Get
            Set(ByVal value As String)
                ZG1 = CuttingData(value, 1)
                ZG2 = CuttingData(value, 2)
                '***�C�� maeda 2008/05/21******************************
                '��ʃR�[�h��41,43,44,45��21�ɕύX����
                Select Case ZG2
                    Case "41", "43", "44", "45"
                        ZG2 = "21"
                    Case Else
                End Select
                '******************************************************
                ZG3 = CuttingData(value, 1)
                ZG4 = CuttingData(value, 10)
                ZG5 = CuttingData(value, 40)
                ZG6 = CuttingData(value, 4)
                ZG7 = CuttingData(value, 4)
                ZG8 = CuttingData(value, 15)
                ZG9 = CuttingData(value, 3)
                ZG10 = CuttingData(value, 15)
                ZG11 = CuttingData(value, 1)
                ZG12 = CuttingData(value, 7)
                ZG13 = CuttingData(value, 17)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC1 As ZGRECORD1

    '�f�[�^���R�[�h
    Structure ZGRECORD2
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    ' �f�[�^�敪(=2)
        <VBFixedString(4)> Public ZG2 As String    ' �������Z�@�փR�[�h		��d����s�ԍ�
        <VBFixedString(15)> Public ZG3 As String   ' �������Z�@�֖�         ��d����s���@
        <VBFixedString(3)> Public ZG4 As String    ' �����X�܃R�[�h         ��d���x�X�ԍ�
        <VBFixedString(15)> Public ZG5 As String   ' �����X�ܖ�             ��d���x�X��
        <VBFixedString(4)> Public ZG6 As String    ' �_�~�[�G���A           ��`�������ԍ�
        <VBFixedString(1)> Public ZG7 As String    ' �a�����               �a�����
        <VBFixedString(7)> Public ZG8 As String    ' �����ԍ�               �����ԍ�
        <VBFixedString(30)> Public ZG9 As String   ' �a���Ҏ���             ���l
        <VBFixedString(10)> Public ZG10 As String  ' �������z               �U�����z
        <VBFixedString(1)> Public ZG11 As String   ' �V�K�R�[�h             �V�K�R�[�h
        <VBFixedString(10)> Public ZG12 As String  ' �ڋq�ԍ�               �ڋq�R�[�h�P
        <VBFixedString(10)> Public ZG13 As String  '				        �ڋq�R�[�h�Q
        <VBFixedString(1)> Public ZG14 As String   ' �U�֌��ʃR�[�h         �U���w��敪
        <VBFixedString(8)> Public ZG15 As String   ' �_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 4),
                            SubData(ZG3, 15),
                            SubData(ZG4, 3),
                            SubData(ZG5, 15),
                            SubData(ZG6, 4),
                            SubData(ZG7, 1),
                            SubData(ZG8, 7),
                            SubData(ZG9, 30),
                            SubData(ZG10, 10),
                            SubData(ZG11, 1),
                            SubData(ZG12, 10),
                            SubData(ZG13, 10),
                            SubData(ZG14, 1),
                            SubData(ZG15, 8)
                            })
            End Get
            Set(ByVal Value As String)
                ZG1 = CuttingData(Value, 1)
                ZG2 = CuttingData(Value, 4)
                ZG3 = CuttingData(Value, 15)
                ZG4 = CuttingData(Value, 3)
                ZG5 = CuttingData(Value, 15)
                ZG6 = CuttingData(Value, 4)
                ZG7 = CuttingData(Value, 1)
                ZG8 = CuttingData(Value, 7)
                ZG9 = CuttingData(Value, 30)
                ZG10 = CuttingData(Value, 10)
                ZG11 = CuttingData(Value, 1)
                ZG12 = CuttingData(Value, 10)
                ZG13 = CuttingData(Value, 10)
                ZG14 = CuttingData(Value, 1)
                ZG15 = CuttingData(Value, 8)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC2 As ZGRECORD2

    '�g���[�����R�[�h
    Structure ZGRECORD8
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    '�f�[�^�敪(=8)
        <VBFixedString(6)> Public ZG2 As String    ' ���v����
        <VBFixedString(12)> Public ZG3 As String   ' ���v���z
        <VBFixedString(6)> Public ZG4 As String    ' �U�֍ό���
        <VBFixedString(12)> Public ZG5 As String   ' �U�֍ϋ��z
        <VBFixedString(6)> Public ZG6 As String    ' �U�֕s�\����
        <VBFixedString(12)> Public ZG7 As String   ' �U�֕s�\���z
        <VBFixedString(65)> Public ZG8 As String    '�_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 6),
                            SubData(ZG3, 12),
                            SubData(ZG4, 6),
                            SubData(ZG5, 12),
                            SubData(ZG6, 6),
                            SubData(ZG7, 12),
                            SubData(ZG8, 65)
                            })
            End Get
            Set(ByVal Value As String)
                ZG1 = CuttingData(Value, 1)
                ZG2 = CuttingData(Value, 6)
                ZG3 = CuttingData(Value, 12)
                ZG4 = CuttingData(Value, 6)
                ZG5 = CuttingData(Value, 12)
                ZG6 = CuttingData(Value, 6)
                ZG7 = CuttingData(Value, 12)
                ZG8 = CuttingData(Value, 65)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC8 As ZGRECORD8

    '�G���h���R�[�h
    Structure ZGRECORD9
        Implements CFormat.IFormat

        <VBFixedString(1)> Public ZG1 As String    '�f�[�^�敪(=9)
        <VBFixedString(119)> Public ZG2 As String  '�_�~�[��
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {
                            SubData(ZG1, 1),
                            SubData(ZG2, 119)
                            })
            End Get
            Set(ByVal Value As String)
                ZG1 = CuttingData(Value, 1)
                ZG2 = CuttingData(Value, 119)
            End Set
        End Property
    End Structure
    Public ZENGIN_REC9 As ZGRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- START
        DataInfo.WorkLen = 120
        '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- END

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

    '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- START
    Public Sub New(ByVal len As Integer)
        '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
        Me.New()
        'MyBase.New()
        '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END

        ' ���R�[�h���w��
        DataInfo.RecoedLen = len

    End Sub
    '2017/05/24 �^�X�N�j���� ADD �W���ŏC���iJIS118,119���Ή��j-------------------------- END


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

        '2008/06/12 �l���M���p�@TXT�Ǘ��̕����ȊO�ُ͈�Ɣ��肷��B�i�v�����j================================
        Select Case RecordData.Substring(0, 1)
            Case "1"        ' �w�b�_���R�[�h
                buff.Append(EncdJ.GetString(RD, 0, 14))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 14, 40), -1))
                buff.Append(EncdJ.GetString(RD, 54, 8))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 62, 15), -1))
                buff.Append(EncdJ.GetString(RD, 77, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 80, 15), -1))
                '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
                Dim intS As Integer = RecordLen - DataInfo.LenOfOneRec
                buff.Append(EncdJ.GetString(RD, 95, 25 - intS))
                'buff.Append(EncdJ.GetString(RD, 95, 25))
                '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
                    mRecordData = buff.ToString(0, DataInfo.LenOfOneRec)
                    'mRecordData = buff.ToString(0, RecordLen)
                    '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END
                End If
            Case "2"        ' �f�[�^���R�[�h
                buff.Append(EncdJ.GetString(RD, 0, 5))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 5, 15), -1))
                buff.Append(EncdJ.GetString(RD, 20, 3))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 23, 15), -1))
                buff.Append(EncdJ.GetString(RD, 38, 12))
                buff.Append(ReplaceString(EncdJ.GetString(RD, 50, 30), -1))
                buff.Append(EncdJ.GetString(RD, 80, 11))    '�������Ƃ����z�A�V�K�R�[�h
                buff.Append(ReplaceString(EncdJ.GetString(RD, 91, 20), -1)) '�ڋq�R�[�h
                '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
                Dim intS As Integer = RecordLen - DataInfo.LenOfOneRec
                buff.Append(EncdJ.GetString(RD, 111, 9 - intS)) '�U�֌��ʃR�[�h�A�_�~�[
                'buff.Append(EncdJ.GetString(RD, 111, 9)) '�U�֌��ʃR�[�h�A�_�~�[
                '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END
                If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
                    '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ START
                    mRecordData = buff.ToString(0, DataInfo.LenOfOneRec)
                    'mRecordData = buff.ToString(0, RecordLen)
                    '2018/03/05 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�s��C���j------------------------ END
                End If

                nRet = CheckRegularStringVerA(RecordData, 5, 15)
                If nRet >= 0 Then
                    Return nRet
                End If
                nRet = CheckRegularStringVerA(RecordData, 23, 15)
                If nRet >= 0 Then
                    Return nRet
                End If
                nRet = CheckRegularStringVerA(RecordData, 50, 30)
                If nRet >= 0 Then
                    Return nRet
                End If
                nRet = CheckRegularStringVerA(RecordData, 91, 20)
                If nRet >= 0 Then
                    Return nRet
                End If
                If nRet >= 0 Then

                    If nRet = 118 AndAlso RecordLen = 120 Then
                        If mRecordData.Substring(118) = Environment.NewLine Then
                            Return -1
                        End If

                    ElseIf nRet = 119 AndAlso RecordLen = 120 Then
                        If mRecordData.Substring(119) = vbCr OrElse
                           mRecordData.Substring(119) = vbLf Then
                            Return -1
                        End If

                    End If
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
        If nRet = 118 AndAlso RecordLen = 120 Then
            If mRecordData.Substring(118) = Environment.NewLine Then
                Return -1
            End If
        ElseIf nRet = 119 AndAlso RecordLen = 120 Then
            If mRecordData.Substring(119) = vbCr OrElse
               mRecordData.Substring(119) = vbLf Then
                Return -1
            End If
        End If
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

            Case ChrW(&H1A) '2010.01.19�@1A �ǉ� start
                If BeforeRecKbn <> "9" Then
                    DataInfo.Message = "���R�[�h�敪�ُ�i1A�j�ُ�"
                    mnErrorNumber = 1
                    Return "ERR"
                Else
                    sRet = "1A"
                End If
            '2010.01.19�@1A �ǉ� end
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
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F
    '
    Public Overrides Function CheckKekkaFormat() As String
        ' ��{�N���X �`�F�b�N
        Dim sRet As String = MyBase.CheckKekkaFormat()

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
                    If sRet <> "ERR" Then
                        If CheckTrailerRecordFunou() = False Then
                            sRet = "ERR"
                        End If
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
        Call MyBase.CheckDataFormatAfterFunou()

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
        ZENGIN_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC1.ZG1
        InfoMeisaiMast.SYUBETU_CODE = ZENGIN_REC1.ZG2
        InfoMeisaiMast.CODE_KBN = ZENGIN_REC1.ZG3
        InfoMeisaiMast.ITAKU_CODE = ZENGIN_REC1.ZG4
        InfoMeisaiMast.ITAKU_KNAME = ZENGIN_REC1.ZG5
        '2017/12/12 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- START
        If INI_IRAI_KYUJITU_HOSEI = "1" Then
            '�x���␳���s��
            InfoMeisaiMast.FURIKAE_DATE = HoseiFurikaeDate(ZENGIN_REC1.ZG6)
        Else
            InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZENGIN_REC1.ZG6, "yyyyMMdd")
        End If
        'InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(ZENGIN_REC1.ZG6, "yyyyMMdd")
        '2017/12/12 �^�X�N�j���� CHG �W���ŏC���F�L���M���Ή��i�U�֓��x���␳�Ή��j---------------- END
        InfoMeisaiMast.FURIKAE_DATE_MOTO = ZENGIN_REC1.ZG6
        InfoMeisaiMast.ITAKU_KIN = ZENGIN_REC1.ZG7
        InfoMeisaiMast.ITAKU_SIT = ZENGIN_REC1.ZG9
        InfoMeisaiMast.ITAKU_KAMOKU = ZENGIN_REC1.ZG11
        InfoMeisaiMast.ITAKU_KOUZA = ZENGIN_REC1.ZG12

        Return "H"
    End Function

    Public Function CheckDBRecord1() As String
        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        '2018/10/07 saitou �L���M��(RSV2�W��) ADD �i�w�b�_�������`�F�b�N�Ή��j ------------------ START
        '�w�b�_�`�F�b�N���s���ƁA�w�b�_�̖��׏�񂪎������ŏ㏑�������̂ŁA���炩���ߕϐ��Ɋi�[����
        Dim strTSIT_NO As String = InfoMeisaiMast.ITAKU_SIT
        Dim strKAMOKU As String = InfoMeisaiMast.ITAKU_KAMOKU
        Dim strKOUZA As String = InfoMeisaiMast.ITAKU_KOUZA
        '2018/10/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END

        '�f�[�^�`�F�b�N
        If MyBase.CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        '2018/10/07 saitou �L���M��(RSV2�W��) ADD �i�w�b�_�������`�F�b�N�Ή��j ------------------ START
        If OraDB Is Nothing Then
        Else
            If INI_S_KDBMAST_CHK = "1" AndAlso mInfoComm.INFOToriMast.FSYORI_KBN_T = "3" Then
                '2018/10/07 saitou �L���M��(RSV2�W��) UPD �i�w�b�_�������`�F�b�N�Ή��j ------------------ START
                Dim bRet As Boolean = ChkKDBMAST(strTSIT_NO, strKAMOKU, strKOUZA)
                If bRet = False Then
                    Dim MSG As String = String.Format("�x�X�R�[�h�F{0} �ȖځF{1} �����F{2}", strTSIT_NO, strKAMOKU, strKOUZA)
                    WriteBLog("�w�b�_�������`�F�b�N", "�����Ȃ�", MSG)
                    DataInfo.Message = "�w�b�_�������`�F�b�N�s��v " & MSG

                    Dim InError As INPUTERROR = Nothing
                    InError.ERRINFO = "�������Ȃ�(�w�b�_�[)"
                    InErrorArray.Add(InError)

                    Return "IJO"
                End If
                'Dim bRet As Boolean = ChkKDBMAST(InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
                'If bRet = False Then
                '    Dim MSG As String = String.Format("�x�X�R�[�h�F{0} �ȖځF{1} �����F{2}", InfoMeisaiMast.ITAKU_SIT, InfoMeisaiMast.ITAKU_KAMOKU, InfoMeisaiMast.ITAKU_KOUZA)
                '    WriteBLog("�w�b�_�������`�F�b�N", "�����Ȃ�", MSG)
                '    DataInfo.Message = "�w�b�_�������`�F�b�N�s��v " & MSG

                '    Dim InError As INPUTERROR = Nothing
                '    InError.ERRINFO = "�������Ȃ�(�w�b�_�[)"
                '    InErrorArray.Add(InError)

                '    Return "IJO"
                'End If
                '2018/10/07 saitou �L���M��(RSV2�W��) UPD ------------------------------------------------- END
            End If
        End If
        '2018/10/07 saitou �L���M��(RSV2�W��) ADD ------------------------------------------------- END

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
        ZENGIN_REC2.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC2.ZG1
        InfoMeisaiMast.KEIYAKU_KIN = ZENGIN_REC2.ZG2
        InfoMeisaiMast.KEIYAKU_SIT = ZENGIN_REC2.ZG4
        InfoMeisaiMast.KEIYAKU_KAMOKU = ZENGIN_REC2.ZG7


        '�S��t�H�[�}�b�g�̌_������ԍ��͑O��󔒂̏ꍇ�A0���߂���
        InfoMeisaiMast.KEIYAKU_KOUZA = CStr(ZENGIN_REC2.ZG8).Trim.PadLeft(7, "0"c)
        InfoMeisaiMast.KEIYAKU_KNAME = ZENGIN_REC2.ZG9
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(ZENGIN_REC2.ZG10.TrimStart)
        InfoMeisaiMast.FURIKIN_MOTO = ZENGIN_REC2.ZG10.TrimStart
        InfoMeisaiMast.SINKI_CODE = ZENGIN_REC2.ZG11
        InfoMeisaiMast.KEIYAKU_NO = ZENGIN_REC2.ZG12
        InfoMeisaiMast.JYUYOKA_NO = ZENGIN_REC2.ZG12 & ZENGIN_REC2.ZG13

        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ����擾
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = ZENGIN_REC2.ZG3
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = ZENGIN_REC2.ZG5
        InfoMeisaiMast.FURIKETU_CODE = CASTCommon.CAInt32(ZENGIN_REC2.ZG14)
        InfoMeisaiMast.FURIKETU_MOTO = ZENGIN_REC2.ZG14

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        Return "D"
    End Function

    Private Function CheckDBRecord2() As String
        Dim CheckRet As Boolean

        '�f�[�^�`�F�b�N
        CheckRet = CheckDataRecord()

        ' �E�v
        InfoMeisaiMast.NTEKIYO = ""
        InfoMeisaiMast.KTEKIYO = ""
        Try
            If (Not mInfoComm Is Nothing) Then
                Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                    Case "0"
                        '2017/01/16 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                        '�X���[�G�X�̏ꍇ�͓E�v��ҏW����B
                        Select Case mInfoComm.INFOToriMast.FMT_KBN_T
                            Case "20", "21"
                                If mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Length > 10 Then
                                    InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim.Substring(0, 10)
                                Else
                                    InfoMeisaiMast.KTEKIYO = "SK(" & mInfoComm.INFOToriMast.KTEKIYOU_T.Trim
                                End If
                                InfoMeisaiMast.NTEKIYO = ""
                            Case Else
                                InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                                InfoMeisaiMast.NTEKIYO = ""
                        End Select
                    'InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                    'InfoMeisaiMast.NTEKIYO = ""
                    '2017/01/16 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case "2"
                        '2017/01/16 saitou ���t�M��(RSV2�W��) UPD �X���[�G�X�Ή� ---------------------------------------- START
                        '�X���[�G�X�̏ꍇ�͓E�v��ҏW����B
                        Select Case mInfoComm.INFOToriMast.FMT_KBN_T
                            Case "20", "21"
                                '�˗��f�[�^��SK(���t���Ă��邩�킩��Ȃ��̂ŁA�˗��f�[�^��SK(�͎�菜���A�ʂ�SK(��t������
                                InfoMeisaiMast.KTEKIYO = "SK(" & ZENGIN_REC2.ZG5.Replace("SK(", "").PadRight(10, " "c).Substring(0, 10).Trim
                            Case Else
                                InfoMeisaiMast.KTEKIYO = ZENGIN_REC2.ZG5.PadRight(13, " "c).Substring(0, 13).Trim
                        End Select
                    'InfoMeisaiMast.KTEKIYO = ZENGIN_REC2.ZG5.PadRight(13, " "c).Substring(0, 13).Trim
                    '2017/01/16 saitou ���t�M��(RSV2�W��) UPD ------------------------------------------------------- END
                    Case "3"
                        InfoMeisaiMast.KTEKIYO = ZENGIN_REC2.ZG15
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
        ZENGIN_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC8.ZG1
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG4)
        InfoMeisaiMast.TOTAL_ZUMI_KIN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG5)
        InfoMeisaiMast.TOTAL_FUNO_KEN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG6)
        InfoMeisaiMast.TOTAL_FUNO_KIN = CASTCommon.CaDecNormal(ZENGIN_REC8.ZG7)

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = ZENGIN_REC8.ZG2
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = ZENGIN_REC8.ZG3
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = ZENGIN_REC8.ZG4
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = ZENGIN_REC8.ZG5
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = ZENGIN_REC8.ZG6
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = ZENGIN_REC8.ZG7

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
        ZENGIN_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = ZENGIN_REC9.ZG1

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

        ZENGIN_REC2.Data = RecordData

        If Not ToriData Is Nothing Then
            Dim ToriCode As String = ToriData.INFOToriMast.TORIS_CODE_T & ToriData.INFOToriMast.TORIF_CODE_T

            '���������Ή�
            If ("," & KokuminNenkinTori).IndexOf(ToriCode) >= 1 Then

                '�����N���ی����̏ꍇ
                '���Z�@�֖��ɒ����p�X�ԁC�����p���Ԃ�X�Ԃƒ����p�X�Ԃ���v���Ȃ��ꍇ�ɂ̂݃Z�b�g����
                If InfoMeisaiMast.TEISEI_SIT <> "000" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> "" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> InfoMeisaiMast.KEIYAKU_SIT Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(4, 3).Insert(4, InfoMeisaiMast.TEISEI_SIT)

                    'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(9, InfoMeisaiMast.TEISEI_SIT)
                    End If
                End If

                If InfoMeisaiMast.TEISEI_KOUZA <> "0000000" AndAlso InfoMeisaiMast.TEISEI_KOUZA <> "" Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(8, 7).Insert(8, InfoMeisaiMast.TEISEI_KOUZA)

                    'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(13, InfoMeisaiMast.TEISEI_KOUZA)
                    End If
                End If
            End If
        End If

        '�U�֌��ʂ��Z�b�g
        ZENGIN_REC2.ZG14 = InfoMeisaiMast.FURIKETU_KEKKA

        'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, ZENGIN_REC2.ZG14)
        End If

        RecordData = ZENGIN_REC2.Data

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
        ZENGIN_REC8.ZG4 = InfoMeisaiMast.TOTAL_NORM_KEN.ToString.PadLeft(6, "0"c)
        ZENGIN_REC8.ZG5 = InfoMeisaiMast.TOTAL_NORM_KIN.ToString.PadLeft(12, "0"c)
        ' �U�֕s�\�������Z�b�g
        ZENGIN_REC8.ZG6 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
        ZENGIN_REC8.ZG7 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)

        'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(19, ZENGIN_REC8.ZG4)
            ReadByteBin.Insert(25, ZENGIN_REC8.ZG5)
            ReadByteBin.Insert(37, ZENGIN_REC8.ZG6)
            ReadByteBin.Insert(43, ZENGIN_REC8.ZG7)
        End If

        RecordData = ZENGIN_REC8.Data

        Call MyBase.GetHenkanTrailerRecord()
    End Sub
    ' �@�\�@ �F �ĐU�w�b�_���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overrides Sub GetSaifuriHeaderRecord(ByVal SAIFURI_DATE As String)
        If IsHeaderRecord() = False Then
            Return
        End If

        ' ���R�[�h�f�[�^�𕪐�
        Call CheckRecord1()

        '�ĐU�����Z�b�g
        ZENGIN_REC1.ZG6 = SAIFURI_DATE
        'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(54, ZENGIN_REC1.ZG6)
        End If

        RecordData = ZENGIN_REC1.Data

        Call MyBase.GetSaifuriHeaderRecord(SAIFURI_DATE)
    End Sub
    ' �@�\�@ �F �ĐU�f�[�^���R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overrides Sub GetSaifuriDataRecord()
        If IsDataRecord() = False Then
            Return
        End If

        ZENGIN_REC2.Data = RecordData

        If Not ToriData Is Nothing Then
            Dim ToriCode As String = ToriData.INFOToriMast.TORIS_CODE_T & ToriData.INFOToriMast.TORIF_CODE_T

            '���������Ή�
            If ("," & KokuminNenkinTori).IndexOf(ToriCode) >= 1 Then

                '�����N���ی����̏ꍇ
                '���Z�@�֖��ɒ����p�X�ԁC�����p���Ԃ�X�Ԃƒ����p�X�Ԃ���v���Ȃ��ꍇ�ɂ̂݃Z�b�g����
                If InfoMeisaiMast.TEISEI_SIT <> "000" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> "" AndAlso
                    InfoMeisaiMast.TEISEI_SIT <> InfoMeisaiMast.KEIYAKU_SIT Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(4, 3).Insert(4, InfoMeisaiMast.TEISEI_SIT)

                    'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(9, InfoMeisaiMast.TEISEI_SIT)
                    End If
                End If

                If InfoMeisaiMast.TEISEI_KOUZA <> "0000000" AndAlso InfoMeisaiMast.TEISEI_KOUZA <> "" Then
                    ZENGIN_REC2.ZG3 = ZENGIN_REC2.ZG3.Remove(8, 7).Insert(8, InfoMeisaiMast.TEISEI_KOUZA)

                    'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
                    If Not ReadByteBin Is Nothing Then
                        ReadByteBin.Insert(13, InfoMeisaiMast.TEISEI_KOUZA)
                    End If
                End If
            End If
        End If

        '�U�֌��ʂ��Z�b�g
        ZENGIN_REC2.ZG14 = InfoMeisaiMast.FURIKETU_KEKKA

        'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, ZENGIN_REC2.ZG14)
        End If
        RecordData = ZENGIN_REC2.Data

        ' ���R�[�h�f�[�^�𕪐�
        Call CheckRecord2()
        Call MyBase.GetSaifuriDataRecord()

        '�f�[�^���R�[�h�̐U�֌��ʂ�0�ɂ���
        '�U�֌��ʂ��Z�b�g
        ZENGIN_REC2.ZG14 = "0"

        'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(111, ZENGIN_REC2.ZG14)
        End If
        RecordData = ZENGIN_REC2.Data
    End Sub

    ' �@�\�@ �F �ĐU�g���[�����R�[�h���擾����
    '
    ' �߂�l �F �P���R�[�h���̃f�[�^
    '
    ' ���l�@ �F
    '
    Public Overrides Sub GetSaifuriTrailerRecord(Optional ByVal SyoriKen As Long = 0, Optional ByVal SyoriKin As Long = 0,
                                                       Optional ByVal Write As Boolean = False)
        If IsTrailerRecord() = False Then
            Return
        End If

        ' ���R�[�h�f�[�^�𕪐�
        Call CheckRecord8()

        ' �U�֕s�\�������Z�b�g
        If Write = True Then
            ZENGIN_REC8.ZG2 = SyoriKen.ToString.PadLeft(6, "0"c)
            ZENGIN_REC8.ZG3 = SyoriKin.ToString.PadLeft(12, "0"c)
        Else
            ZENGIN_REC8.ZG2 = InfoMeisaiMast.TOTAL_IJO_KEN.ToString.PadLeft(6, "0"c)
            ZENGIN_REC8.ZG3 = InfoMeisaiMast.TOTAL_IJO_KIN.ToString.PadLeft(12, "0"c)
        End If

        ' 0���Z�b�g
        ZENGIN_REC8.ZG4 = "0".PadLeft(6, "0"c)
        ZENGIN_REC8.ZG5 = "0".PadLeft(12, "0"c)
        ZENGIN_REC8.ZG6 = "0".PadLeft(6, "0"c)
        ZENGIN_REC8.ZG7 = "0".PadLeft(12, "0"c)

        'EBCDIC�f�[�^�Ή��F�o�C�i���f�[�^�����݂���ꍇ�͏���������
        If Not ReadByteBin Is Nothing Then
            ReadByteBin.Insert(1, ZENGIN_REC8.ZG2)
            ReadByteBin.Insert(7, ZENGIN_REC8.ZG3)
            ReadByteBin.Insert(19, ZENGIN_REC8.ZG4)
            ReadByteBin.Insert(25, ZENGIN_REC8.ZG5)
            ReadByteBin.Insert(37, ZENGIN_REC8.ZG6)
            ReadByteBin.Insert(43, ZENGIN_REC8.ZG7)
        End If

        RecordData = ZENGIN_REC8.Data

        Call MyBase.GetSaifuriTrailerRecord(SyoriKin, SyoriKin, Write)
    End Sub

    '
    ' �@�\�@ �F ���R�[�h��ǂݍ���Ń`�F�b�N�i�X���[�G�X�p�j
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����
    '
    ' ���l�@ �F 2017/01/16 saitou ���t�M��(RSV2�W��) added for �X���[�G�X�Ή�
    '
    Public Overrides Function CheckKekkaFormatSSS() As String
        ' ��{�N���X �`�F�b�N
        Dim sRet As String = MyBase.CheckKekkaFormatSSS()

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
                    If sRet <> "ERR" Then
                        '�r�r�r�̕s�\�f�[�^�͕s�\�������Ԃ��Ă��Ȃ����߁A�f�[�^�ƃg���[���̌����͍���Ȃ�
                        'If CheckTrailerRecordFunou() = False Then
                        '    sRet = "ERR"
                        'End If
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
        Call MyBase.CheckDataFormatAfterFunou()

        Return sRet
    End Function

    ''' <summary>
    ''' �������}�X�^�Ɏw����������݂��邩�`�F�b�N����
    ''' </summary>
    ''' <param name="astrSIT_NO">�x�X�R�[�h</param>
    ''' <param name="astrKAMOKU">�ȖڃR�[�h</param>
    ''' <param name="astrKOUZA">�����ԍ�</param>
    ''' <returns></returns>
    ''' <remarks>2018/10/07 saitou �L���M��(RSV2�W��) added for �w�b�_�����`�F�b�N�Ή�</remarks>
    Private Function ChkKDBMAST(ByVal astrSIT_NO As String, ByVal astrKAMOKU As String, ByVal astrKOUZA As String) As Boolean
        Dim SQL As New System.Text.StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            OraReader = New CASTCommon.MyOracleReader(OraDB)

            astrKAMOKU = CASTCommon.ConvertKamoku1TO2(astrKAMOKU)

            SQL.Append("SELECT TSIT_NO_D, KOUZA_D, IDOU_DATE_D FROM KDBMAST")
            SQL.Append(" WHERE TSIT_NO_D = '" & astrSIT_NO & "'")
            SQL.Append(" AND KAMOKU_D = '" & astrKAMOKU & "'")
            SQL.Append(" AND KOUZA_D = '" & astrKOUZA & "'")

            Return OraReader.DataReader(SQL)

        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

    End Function

End Class
