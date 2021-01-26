Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' ���ɓn���l�s���C�Z���^�[�t�H�[�}�b�g�i�W���j�N���X
Public Class CFormatTokCenter
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 640

    ' ------------------------------------------
    ' ���ɓn���l�s���C�Z���^�[�t�H�[�}�b�g
    ' ------------------------------------------
    ' �e���R�[�h���ʕ�
    Public Structure TCRECORD
        Implements CFormat.IFormat

        <VBFixedString(8)> Public TC1 As String     ' �U�֓��w��                
        <VBFixedString(4)> Public TC2 As String     ' ������ƃR�[�h
        <VBFixedString(2)> Public TC3 As String     ' �����Ǘ��ԍ�
        <VBFixedString(8)> Public TC4 As String     ' �V�[�P���XNO
        <VBFixedString(3)> Public TC5 As String     ' �U�փR�[�h
        <VBFixedString(5)> Public TC6 As String     ' ��ƃR�[�h
        <VBFixedString(2)> Public TC7 As String     ' ��Ǝ��� 
        <VBFixedString(4)> Public TC8 As String     ' ���ɃR�[�h
        <VBFixedString(8)> Public TC9 As String     ' �`�F�b�N�N����
        <VBFixedString(2)> Public TC10 As String    ' �����`�F�b�N���ʃR�[�h
        <VBFixedString(2)> Public TC11 As String    ' �w�b�_��ʃR�[�h
        <VBFixedString(1)> Public TC12 As String    ' ���Ɏ��A��MT�쐬�ςݕ\��
        <VBFixedString(1)> Public TC13 As String    ' ���c�Ɠ��Ԋҕ\��
        <VBFixedString(5)> Public TC14 As String    ' ���U��ƃR�[�h
        <VBFixedString(5)> Public TC15 As String    ' �\��

        <VBFixedString(2)> Public CO1 As String     ' �U���Ȗ�
        <VBFixedString(2)> Public CO2 As String     ' �ȖڃR�[�h�i�Z���^���j
        <VBFixedString(7)> Public CO3 As String     ' �����ԍ��@�i�Z���^���j
        <VBFixedString(1)> Public CO4 As String     ' �Ȗږ��̃R�[�h
        <VBFixedString(3)> Public CO5 As String     ' �X�ԁ@�@�@�i������j
        <VBFixedString(9)> Public CO6 As String     ' �ȖځE���ԁi������j
        <VBFixedString(4)> Public CO7 As String     ' �\��

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {TC1, TC2, TC3, TC4, TC5, TC6, TC7, _
                            TC8, TC9, TC10, TC11, TC12, TC13, TC14, TC15, _
                            CO1, CO2, CO3, CO4, CO5, CO6, CO7 _
                            })
            End Get
            Set(ByVal value As String)
                TC1 = CuttingData(value, 8)
                TC2 = CuttingData(value, 4)
                TC3 = CuttingData(value, 2)
                TC4 = CuttingData(value, 8)
                TC5 = CuttingData(value, 3)
                TC6 = CuttingData(value, 5)
                TC7 = CuttingData(value, 2)
                TC8 = CuttingData(value, 4)
                TC9 = CuttingData(value, 8)
                TC10 = CuttingData(value, 2)
                TC11 = CuttingData(value, 2)
                TC12 = CuttingData(value, 1)
                TC13 = CuttingData(value, 1)
                TC14 = CuttingData(value, 5)
                TC15 = CuttingData(value, 5)
                CO1 = CuttingData(value, 2)
                CO2 = CuttingData(value, 2)
                CO3 = CuttingData(value, 7)
                CO4 = CuttingData(value, 1)
                CO5 = CuttingData(value, 3)
                CO6 = CuttingData(value, 9)
                CO7 = CuttingData(value, 4)
            End Set
        End Property
    End Structure

    ' ------------------------------------------
    ' ���ɓn���l�s���C�Z���^�[�t�H�[�}�b�g
    ' ------------------------------------------
    ' �w�b�_���R�[�h
    Public Structure CORECORD1
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' �e���R�[�h���ʕ�

        Public R1 As CFormatZengin.ZGRECORD1        ' **��������S��f�[�^�`���ƂȂ�**
        <VBFixedString(432)> Public CO1 As String   ' �_�~�[

        Public N1 As CFormatNTTCMT.NTRECORD1        ' **��������m�s�s�����U�փf�[�^�`���ƂȂ�**
        <VBFixedString(372)> Public CO2 As String   ' �_�~�[

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' ����e�s
                    Return String.Concat(New String() _
                                {TC.Data, N1.Data, CO2})
                End If
                ' �ʏ�
                Return String.Concat(New String() _
                            {TC.Data, R1.Data, CO1})
            End Get
            Set(ByVal value As String)
                TC.Data = CuttingData(value, 88)
                If TC.TC5 = "014" And (TC.TC6 = "40140" Or TC.TC6 = "40141") Then
                    ' ����e�s
                    NTTFlag = True
                    N1.Data = CuttingData(value, 180)
                    CO2 = CuttingData(value, 372)
                Else
                    ' �ʏ�
                    NTTFlag = False
                    R1.Data = CuttingData(value, 120)
                    CO1 = CuttingData(value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC1 As CORECORD1

    ' �f�[�^���R�[�h
    Structure STRECORD2
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' �e���R�[�h���ʕ�

        Public R2 As CFormatZengin.ZGRECORD2        ' **��������S��f�[�^�`���ƂȂ�**
        <VBFixedString(432)> Public CO1 As String   ' �_�~�[

        Public N2 As CFormatNTTCMT.NTRECORD2        ' **��������m�s�s�����U�փf�[�^�`���ƂȂ�**
        <VBFixedString(372)> Public CO2 As String   ' �_�~�[

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' ����e�s
                    Return String.Concat(New String() _
                                {TC.Data, N2.Data, CO2})
                End If
                ' �ʏ�
                Return String.Concat(New String() _
                            {TC.Data, R2.Data, CO1})
            End Get
            Set(ByVal Value As String)
                TC.Data = CuttingData(Value, 88)
                If NTTFlag = True Then
                    ' ����e�s
                    N2.Data = CuttingData(Value, 180)
                    CO2 = CuttingData(Value, 372)
                Else
                    ' �ʏ�
                    R2.Data = CuttingData(Value, 120)
                    CO1 = CuttingData(Value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC2 As STRECORD2

    ' �g���[�����R�[�h
    Structure STRECORD8
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' �e���R�[�h���ʕ�
        Public R8 As CFormatZengin.ZGRECORD8        ' **��������S��f�[�^�`���ƂȂ�**
        <VBFixedString(432)> Public CO1 As String   ' �_�~�[

        Public N8 As CFormatNTTCMT.NTRECORD8        ' **��������m�s�s�����U�փf�[�^�`���ƂȂ�**
        <VBFixedString(372)> Public CO2 As String   ' �_�~�[


        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' ����e�s
                    Return String.Concat(New String() _
                                {TC.Data, N8.Data, CO2})
                End If
                ' �ʏ�
                Return String.Concat(New String() _
                            {TC.Data, R8.Data, CO1})
            End Get
            Set(ByVal Value As String)
                TC.Data = CuttingData(Value, 88)
                If NTTFlag = True Then
                    ' ����e�s
                    N8.Data = CuttingData(Value, 180)
                    CO2 = CuttingData(Value, 372)
                Else
                    ' �ʏ�
                    R8.Data = CuttingData(Value, 120)
                    CO1 = CuttingData(Value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC8 As STRECORD8

    ' �G���h���R�[�h
    Structure STRECORD9
        Implements CFormat.IFormat

        Public TC As TCRECORD                       ' �e���R�[�h���ʕ�
        Public R9 As CFormatZengin.ZGRECORD9        ' **��������S��f�[�^�`���ƂȂ�**
        <VBFixedString(432)> Public CO1 As String   ' �_�~�[

        Public N9 As CFormatNTTCMT.NTRECORD9        ' **��������m�s�s�����U�փf�[�^�`���ƂȂ�**
        <VBFixedString(372)> Public CO2 As String   ' �_�~�[

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                If NTTFlag = True Then
                    ' ����e�s
                    Return String.Concat(New String() _
                                {TC.Data, N9.Data, CO2})
                End If
                ' �ʏ�
                Return String.Concat(New String() _
                            {TC.Data, R9.Data, CO1})
            End Get
            Set(ByVal Value As String)
                TC.Data = CuttingData(Value, 88)
                If NTTFlag = True Then
                    ' ����e�s
                    N9.Data = CuttingData(Value, 180)
                    CO2 = CuttingData(Value, 372)
                Else
                    ' �ʏ�
                    R9.Data = CuttingData(Value, 120)
                    CO1 = CuttingData(Value, 432)
                End If
            End Set
        End Property
    End Structure
    Public TOKCENTER_REC9 As STRECORD9

    ' ����t�H�[�}�b�g����
    Private Shared NTTFlag As Boolean

    ' New
    Public Sub New()
        MyBase.New()

        NTTFlag = False

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "2", "8", "9"}
        
        FtranPfile = "640.P"

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
    '           �u���Ώە����́C�s�������ɂ͂Ȃ�Ȃ��͂�
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(DataInfo.LenOfOneRec)

        ' �S�p�����P�����𔼊p�󔒂Q�o�C�g�ɕϊ�
        For i As Integer = 0 To RecordData.Length - 1
            Dim s As String = RecordData.Substring(i, 1)
            If EncdJ.GetByteCount(s) = 2 Then
                buff.Append("  ")
            Else
                buff.Append(s)
            End If
        Next i
        If buff.Length = RecordLen OrElse buff.Length = DataInfo.LenOfOneRec Then
            mRecordData = buff.ToString(0, RecordLen)
        End If

        Select Case RecordData.Substring(88, 1)
            Case "1"        ' �w�b�_���R�[�h
            Case "2"        ' �f�[�^���R�[�h
            Case "8"        ' �g���[��
            Case "9"        ' �G���h
        End Select

        ' �S�p�`�F�b�N
        Return GetZenkakuPos(RecordData)
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

        Select Case RecordData.Substring(88, 1)
            Case "1"
                sRet = CheckRecord1()
            Case "2"
                sRet = CheckRecord2()
            Case "8"
                sRet = CheckRecord8()
            Case "9"
                sRet = CheckRecord9()
                '*** �C�� mitsu 2008/07/16 �󔒃��R�[�h��ʂ� ***
            Case " "
                sRet = "E"
                '************************************************
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

        Dim str As String = ""

        TOKCENTER_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC1.R1.ZG1
        InfoMeisaiMast.FURIKAE_DATE = TOKCENTER_REC1.TC.TC1
        InfoMeisaiMast.FURIKAE_DATE_MOTO = TOKCENTER_REC1.TC.TC1

        InfoMeisaiMast.FURI_CODE = TOKCENTER_REC1.TC.TC5
        InfoMeisaiMast.KIGYO_CODE = TOKCENTER_REC1.TC.TC6

        If NTTFlag = True Then                   '����e�s�̔���
            '�m�s�s�f�[�^�敪�Ή�
            InfoMeisaiMast.DATA_KBN = TOKCENTER_REC1.N1.NT1
            '�m�s�s�f�[�^�敪�Ή�
            InfoMeisaiMast.SYUBETU_CODE = TOKCENTER_REC1.N1.NT2
            InfoMeisaiMast.ITAKU_CODE = "9999999999"          '�ϑ��҃R�[�h�Œ�
            InfoMeisaiMast.ITAKU_KIN = TOKCENTER_REC1.N1.NT10
            InfoMeisaiMast.ITAKU_KAMOKU = TOKCENTER_REC1.N1.NT13
            InfoMeisaiMast.ITAKU_KOUZA = TOKCENTER_REC1.N1.NT14
            InfoMeisaiMast.ITAKU_KNAME = TOKCENTER_REC1.N1.NT9
        Else
            InfoMeisaiMast.SYUBETU_CODE = TOKCENTER_REC1.R1.ZG2
            InfoMeisaiMast.ITAKU_CODE = TOKCENTER_REC1.R1.ZG4
            InfoMeisaiMast.ITAKU_KIN = TOKCENTER_REC1.R1.ZG7
            InfoMeisaiMast.ITAKU_SIT = TOKCENTER_REC1.R1.ZG9
            InfoMeisaiMast.ITAKU_KAMOKU = TOKCENTER_REC1.R1.ZG11
            InfoMeisaiMast.ITAKU_KOUZA = TOKCENTER_REC1.R1.ZG12
            InfoMeisaiMast.ITAKU_KNAME = TOKCENTER_REC1.R1.ZG5
        End If

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        ' �f�[�^�`�F�b�N�i���Ɏ������ݓ��C�Z���^�[�p�j
        str = CheckHeaderRecordC()
        If str <> "" Then
            Return str
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
        TOKCENTER_REC2.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC2.R2.ZG1
        If NTTFlag = True Then '����e�s�̔���
            '�m�s�s�f�[�^�敪�Ή�
            InfoMeisaiMast.DATA_KBN = TOKCENTER_REC2.N2.NT1
            '�m�s�s�f�[�^�敪�Ή�
            InfoMeisaiMast.KEIYAKU_KIN = TOKCENTER_REC2.N2.NT10
            InfoMeisaiMast.KEIYAKU_SIT = TOKCENTER_REC2.N2.NT11
            InfoMeisaiMast.KEIYAKU_KAMOKU = TOKCENTER_REC2.N2.NT12.PadLeft(2, "0"c)
            ' 2008.01.18 Delete >>
            ''�Z���^�[����FMT�͋�s�Ȗڂŗ��邽�߁A�M���ȖڂɕԊҁj20050705
            ''Select Case InfoMeisaiMast.KEIYAKU_KAMOKU
            ''    Case "01"         '����
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "02"
            ''    Case "02"         '����
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "01"
            ''    Case "03"         '�[��
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "05"
            ''    Case "04"         '�E��
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "37"
            ''    Case Else
            ''End Select
            ' 2008.01.18 Delete <<
            InfoMeisaiMast.KEIYAKU_KOUZA = TOKCENTER_REC2.N2.NT13
            InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC2.TC.TC4
            InfoMeisaiMast.KEIYAKU_KNAME = Trim(TOKCENTER_REC2.N2.NT20)  '2007/10/12 20���ɕύX ��2007/10/24�@30���ɕύX
            InfoMeisaiMast.FURIKIN = CaDecNormal(TOKCENTER_REC2.N2.NT18)
            InfoMeisaiMast.FURIKIN_MOTO = TOKCENTER_REC2.N2.NT18
            InfoMeisaiMast.SINKI_CODE = "0"
            InfoMeisaiMast.KEIYAKU_NO = ""
            '2007/05/29:KG �Ð��M���p�@���v�Ɣԍ�(21�޲�)
            InfoMeisaiMast.JYUYOKA_NO = TOKCENTER_REC2.N2.NT2 & TOKCENTER_REC2.N2.NT3 & TOKCENTER_REC2.N2.NT4 & TOKCENTER_REC2.N2.NT5 & TOKCENTER_REC2.N2.NT6
        Else
            InfoMeisaiMast.KEIYAKU_KIN = TOKCENTER_REC2.R2.ZG2
            InfoMeisaiMast.KEIYAKU_SIT = TOKCENTER_REC2.R2.ZG4
            InfoMeisaiMast.KEIYAKU_KAMOKU = TOKCENTER_REC2.R2.ZG7.PadLeft(2, "0"c) '�f�[�^�̉Ȗڂ͂P���Ȃ̂ŁA�Q���ɕϊ�
            ' 2008.01.18 Delete >>
            ''�Z���^�[����FMT�͋�s�Ȗڂŗ��邽�߁A�M���ȖڂɕԊҁj20050705
            ''Select Case InfoMeisaiMast.KEIYAKU_KAMOKU
            ''    Case "01"         '����
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "02"
            ''    Case "02"         '����
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "01"
            ''    Case "03"         '�[��
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "05"
            ''    Case "04"         '�E��
            ''        InfoMeisaiMast.KEIYAKU_KAMOKU = "37"
            ''    Case Else
            ''End Select
            ' 2008.01.18 Delete <<
            InfoMeisaiMast.KEIYAKU_KOUZA = TOKCENTER_REC2.R2.ZG8
            InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC2.TC.TC4
            InfoMeisaiMast.KEIYAKU_KNAME = Trim(TOKCENTER_REC2.R2.ZG9)  '2007/10/12 20���ɕύX  ��2007/10/24 30���ɕύX
            InfoMeisaiMast.FURIKIN = CaDecNormal(TOKCENTER_REC2.R2.ZG10)
            InfoMeisaiMast.FURIKIN_MOTO = TOKCENTER_REC2.R2.ZG10
            InfoMeisaiMast.SINKI_CODE = TOKCENTER_REC2.R2.ZG11
            InfoMeisaiMast.KEIYAKU_NO = TOKCENTER_REC2.R2.ZG12
            InfoMeisaiMast.JYUYOKA_NO = TOKCENTER_REC2.R2.ZG12 & TOKCENTER_REC2.R2.ZG13
        End If

        If Not mInfoComm Is Nothing AndAlso mInfoComm.INFOToriMast.TEKIYOU_KBN_T = "2" Then
            InfoMeisaiMast.KTEKIYO = TOKCENTER_REC2.R2.ZG5.PadRight(15, " "c).Substring(0, 13).Trim
            InfoMeisaiMast.NTEKIYO = ""
        End If


        InfoMeisaiMast.FURIKETU_CODE = 0
        InfoMeisaiMast.FURIKETU_MOTO = "0"

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1


        ' �f�[�^�`�F�b�N
        '���z�`�F�b�N
        If IsDecimal(InfoMeisaiMast.FURIKIN_MOTO) = False Then
            Dim InError As INPUTERROR = Nothing

            InfoMeisaiMast.FURIKETU_CODE = 9
            InError.ERRINFO = Err.Name(Err.InputErrorType.Kingaku)
            InErrorArray.Add(InError)

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
        TOKCENTER_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC8.R8.ZG1
        InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC8.TC.TC4

        If NTTFlag = True Then '����e�s�̔���(��ڃR�[�h�h10�h�����U�A�h91�h�h21�h���ĐU)20050413
            '�m�s�s�f�[�^�敪�Ή�
            InfoMeisaiMast.DATA_KBN = TOKCENTER_REC8.N8.NT1
            '�m�s�s�f�[�^�敪�Ή�
            Select Case InfoMeisaiMast.SYUBETU_CODE
                Case "10"         '���U
                    InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT6)
                    InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT7)

                    InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = TOKCENTER_REC8.N8.NT6
                    InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = TOKCENTER_REC8.N8.NT7
                Case Else         '�ĐU
                    InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT12)
                    InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(TOKCENTER_REC8.N8.NT13)

                    InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = TOKCENTER_REC8.N8.NT12
                    InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = TOKCENTER_REC8.N8.NT13
            End Select
        Else
            InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(TOKCENTER_REC8.R8.ZG2)
            InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(TOKCENTER_REC8.R8.ZG3)

            InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = TOKCENTER_REC8.R8.ZG2
            InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = TOKCENTER_REC8.R8.ZG3
        End If
        InfoMeisaiMast.TOTAL_ZUMI_KEN = 0
        InfoMeisaiMast.TOTAL_ZUMI_KIN = 0
        InfoMeisaiMast.TOTAL_FUNO_KEN = 0
        InfoMeisaiMast.TOTAL_FUNO_KIN = 0

        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = "0"

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
        TOKCENTER_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = TOKCENTER_REC9.R9.ZG1
        InfoMeisaiMast.KIGYO_SEQ = TOKCENTER_REC9.TC.TC4

        Return "E"
    End Function

    '
    ' �@�\�@ �F �w�b�_���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' ���l�@ �F �e�t�H�[�}�b�g�`�F�b�N����C�Ă΂�鋤�ʊ֐�
    ' �@�@�@     �w�b�_��񂩂�C�����}�X�^���Q�Ƃ���
    '
    'Protected Overrides Function CheckHeaderRecord() As Boolean
    Private Function CheckHeaderRecordC() As String

        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader
        Dim ToriReader As CASTCommon.MyOracleReader
        Dim strRet As String = ""

        ' �c�a�ڑ������݂��Ȃ��ꍇ�C����l��Ԃ�
        If OraDB Is Nothing Then
            Return ""
        End If

        ' ���U
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(",TYUUDAN_FLG_S")
        SQL.Append(",TOUROKU_FLG_S")
        SQL.Append(",UKETUKE_FLG_S")
        SQL.Append(",BAITAI_CODE_T")
        SQL.Append(" FROM TORIMAST,SCHMAST")
        SQL.Append(" WHERE FURI_DATE_S  = " & SQ(InfoMeisaiMast.FURIKAE_DATE))
        SQL.Append("   AND FURI_CODE_S =  " & SQ(TOKCENTER_REC1.TC.TC5))
        SQL.Append("   AND KIGYO_CODE_S = " & SQ(TOKCENTER_REC1.TC.TC6))
        If ",91,71,10".IndexOf(InfoMeisaiMast.SYUBETU_CODE) >= 1 Then    '���o���敪
            SQL.Append("   AND NS_KBN_T = '9'")
        Else
            SQL.Append("   AND NS_KBN_T = '1'")
        End If
        SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
        SQL.Append("   AND '1'          = FSYORI_KBN_T")
        SQL.Append("   AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append("   AND TORIF_CODE_S = TORIF_CODE_T")
        SQL.Append("   AND KIGYO_CODE_S = KIGYO_CODE_T")
        SQL.Append("   AND FURI_CODE_S  = FURI_CODE_T ")

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then

            '**************
            '�����̂݌���
            '**************
            SQL = New StringBuilder(128)
            SQL.Append(" SELECT")
            SQL.Append(" TORIS_CODE_T,TORIF_CODE_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE FURI_CODE_T = " & SQ(TOKCENTER_REC1.TC.TC5))
            SQL.Append(" AND KIGYO_CODE_T = " & SQ(TOKCENTER_REC1.TC.TC6))
            ToriReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
            If ToriReader.DataReader(SQL) = False Then

                ' �����}�X�^�����N���A����
                Call mInfoComm.GetTORIMAST("", "")
                WriteBLog("�t�@�C���w�b�_����挟��", "���s", "�ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
                DataInfo.Message = "����挟�����s �ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " �U�փR�[�h�F" & TOKCENTER_REC1.TC.TC5 & " ��ƃR�[�h�F" & TOKCENTER_REC1.TC.TC6 & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
                strRet = "NT"
            Else

                ' �����}�X�^���擾
                Call mInfoComm.GetTORIMAST(ToriReader.GetItem("TORIS_CODE_T"), ToriReader.GetItem("TORIF_CODE_T"))
                WriteBLog("�X�P�W���[������", "���s", "�ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
                DataInfo.Message = "�X�P�W���[���������s �ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " �U�փR�[�h�F" & TOKCENTER_REC1.TC.TC5 & " ��ƃR�[�h�F" & TOKCENTER_REC1.TC.TC6 & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
                strRet = "NS"
            End If

            ToriReader.Close()

        Else
            ' �����}�X�^���擾
            Call mInfoComm.GetTORIMAST(OraReader.GetItem("TORIS_CODE_T"), OraReader.GetItem("TORIF_CODE_T"))
        End If

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.ITAKU_KIN = mInfoComm.INFOToriMast.TKIN_NO_T     ' �戵���Z�@�փR�[�h
        InfoMeisaiMast.ITAKU_SIT = mInfoComm.INFOToriMast.TSIT_NO_T     ' �戵�x�X�R�[�h
        InfoMeisaiMast.ITAKU_KAMOKU = mInfoComm.INFOToriMast.KAMOKU_T   ' �Ȗ�
        InfoMeisaiMast.ITAKU_KOUZA = mInfoComm.INFOToriMast.KOUZA_T     ' �����ԍ�

        BLOG.ToriCode = mInfoComm.INFOToriMast.TORIS_CODE_T & mInfoComm.INFOToriMast.TORIF_CODE_T

        ' 2007/07/04�@�t�H�[�}�b�g�敪�A�}�̃R�[�h�A���f�t���O�A�o�^�t���O�`�F�b�N
        If OraReader.EOF = False AndAlso OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
            WriteBLog("�X�P�W���[��:���f�t���O�ݒ�� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"), "���f")
            DataInfo.Message = "�X�P�W���[��:���f�t���O�ݒ�� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            strRet = "TS"
        End If

        If OraReader.EOF = False AndAlso OraReader.GetItem("TOUROKU_FLG_S") <> "0" Then
            WriteBLog("�X�P�W���[��:�������ݏ����� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"), "���f")
            DataInfo.Message = "�X�P�W���[��:�������ݏ����� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            strRet = "SS"
        End If

        ' �I���N��Reader�N���[�Y
        OraReader.Close()
        WriteBLog("�t�@�C���w�b�_����挟��", "����", "�����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)
        Return strRet
    End Function

    Public Overrides Function IsHeaderRecord() As Boolean
        For i As Integer = 0 To HeaderKubun.Length - 1
            If RecordData.Substring(88, 1) = HeaderKubun(i) Then
                Return True
            End If
        Next i
        Return False
    End Function

    Public Overrides Function IsDataRecord() As Boolean
        For i As Integer = 0 To DataKubun.Length - 1
            If RecordData.Substring(88, 1) = DataKubun(i) Then
                Return True
            End If
        Next i
        Return False
    End Function

    Public Overrides Function IsTrailerRecord() As Boolean
        For i As Integer = 0 To TrailerKubun.Length - 1
            If RecordData.Substring(88, 1) = TrailerKubun(i) Then
                Return True
            End If
        Next i
        Return False
    End Function

    Public Overrides Function IsEndRecord() As Boolean
        If RecordData.Substring(88, 1) = DataInfo.MinRecordCode(DataInfo.MinRecordCode.Length - 1) Then
            Return True
        End If
        Return False
    End Function

    Public Overrides Function UpdateSCHMAST() As Boolean

        Return MyBase.UpdateSCHMAST

    End Function

End Class
