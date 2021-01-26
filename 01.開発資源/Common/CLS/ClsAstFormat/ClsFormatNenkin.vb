Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports Microsoft.VisualBasic
Imports CASTCommon.ModPublic

' �N���U�� �f�[�^�t�H�[�}�b�g�N���X
Public Class CFormatNenkin
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 130

    '�U���x�X�ϊ��p����
    Private ReadOnly SIT_CODE As String = CASTCommon.GetFSKJIni("COMMON", "NENKIN_SIT") '�Ǒ֎��s���̃f�t�H���g�x�X
    Private ReadOnly SIT_CODE_TABLE As New CAstFormat.ClsTextEqual("�x�X�R�[�h.TXT")    '�U���x�X�Ǒփe�[�u��

    '------------------------------------------
    '�N���U���t�H�[�}�b�g
    '------------------------------------------
    '�w�b�_���R�[�h
    '   Public Structure MKRECORD1  2009/09/30 kakiwaki
    Public Structure NKRECORD1
        Implements CFormat.IFormat

        <VBFixedString(2)> Public NK1 As String     '�f�[�^�敪
        <VBFixedString(2)> Public NK2 As String     '�N�����
        <VBFixedString(1)> Public NK3 As String     '�R�[�h�敪
        <VBFixedString(10)> Public NK4 As String    '���{��s�R�[�h
        <VBFixedString(20)> Public NK5 As String    '���{��s��
        <VBFixedString(2)> Public NK6 As String     '���{��s�U���˗��X�R�[�h
        <VBFixedString(18)> Public NK7 As String    '���{��s�U���˗��X��
        <VBFixedString(6)> Public NK8 As String     '�U���˗���
        <VBFixedString(4)> Public NK9 As String     '�˗�����Z�@�փR�[�h
        <VBFixedString(14)> Public NK10 As String   '�˗�����Z�@�֖�
        <VBFixedString(14)> Public NK11 As String   '�_�~�[
        <VBFixedString(21)> Public NK12 As String   '�˗�����Z�@�֓X�ܖ�
        <VBFixedString(10)> Public NK13 As String   '�U������������
        <VBFixedString(6)> Public NK14 As String    '�_�~�[

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 2), _
                            SubData(NK3, 1), _
                            SubData(NK4, 10), _
                            SubData(NK5, 20), _
                            SubData(NK6, 2), _
                            SubData(NK7, 18), _
                            SubData(NK8, 6), _
                            SubData(NK9, 4), _
                            SubData(NK10, 14), _
                            SubData(NK11, 14), _
                            SubData(NK12, 21), _
                            SubData(NK13, 10), _
                            SubData(NK14, 6) _
                            })
            End Get
            Set(ByVal value As String)
                NK1 = CuttingData(value, 2)
                NK2 = CuttingData(value, 2)
                NK3 = CuttingData(value, 1)
                NK4 = CuttingData(value, 10)
                NK5 = CuttingData(value, 20)
                NK6 = CuttingData(value, 2)
                NK7 = CuttingData(value, 18)
                NK8 = CuttingData(value, 6)
                NK9 = CuttingData(value, 4)
                NK10 = CuttingData(value, 14)
                NK11 = CuttingData(value, 14)
                NK12 = CuttingData(value, 21)
                NK13 = CuttingData(value, 10)
                NK14 = CuttingData(value, 6)
            End Set
        End Property
    End Structure
    '   Public NENKIN_REC1 As MKRECORD1     2009/09/30 kakiwaki
    Public NENKIN_REC1 As NKRECORD1

    '�f�[�^���R�[�h
    Structure NKRECORD2
        Implements CFormat.IFormat
        <VBFixedString(2)> Public NK1 As String     '�f�[�^�敪
        <VBFixedString(7)> Public NK2 As String     '�����ԍ�
        <VBFixedString(6)> Public NK3 As String     '�_�~�[
        <VBFixedString(4)> Public NK4 As String     '�U������Z�@�փR�[�h
        <VBFixedString(14)> Public NK5 As String    '�U������Z�@�֖�
        <VBFixedString(11)> Public NK6 As String    '�_�~�[
        <VBFixedString(3)> Public NK7 As String     '�U���X��
        <VBFixedString(21)> Public NK8 As String    '�U����X�ܖ�
        <VBFixedString(1)> Public NK9 As String     '�U���Ȗ�
        <VBFixedString(10)> Public NK10 As String   '�U��������ԍ�
        <VBFixedString(25)> Public NK11 As String   '���l����
        <VBFixedString(8)> Public NK12 As String    '���z
        <VBFixedString(15)> Public NK13 As String   '�N���؏��ԍ�
        <VBFixedString(3)> Public NK14 As String    '�_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 7), _
                            SubData(NK3, 6), _
                            SubData(NK4, 4), _
                            SubData(NK5, 14), _
                            SubData(NK6, 11), _
                            SubData(NK7, 3), _
                            SubData(NK8, 21), _
                            SubData(NK9, 1), _
                            SubData(NK10, 010), _
                            SubData(NK11, 025), _
                            SubData(NK12, 08), _
                            SubData(NK13, 015), _
                            SubData(NK14, 03) _
                            })
            End Get
            Set(ByVal Value As String)
                NK1 = CuttingData(Value, 2)
                NK2 = CuttingData(Value, 7)
                NK3 = CuttingData(Value, 6)
                NK4 = CuttingData(Value, 4)
                NK5 = CuttingData(Value, 14)
                NK6 = CuttingData(Value, 11)
                NK7 = CuttingData(Value, 3)
                NK8 = CuttingData(Value, 21)
                NK9 = CuttingData(Value, 1)
                NK10 = CuttingData(Value, 10)
                NK11 = CuttingData(Value, 25)
                NK12 = CuttingData(Value, 8)
                NK13 = CuttingData(Value, 15)
                NK14 = CuttingData(Value, 3)
            End Set
        End Property
    End Structure
    Public NENKIN_REC2 As NKRECORD2

    '�g���[�����R�[�h
    Structure NKRECORD8
        Implements CFormat.IFormat
        <VBFixedString(2)> Public NK1 As String     '�f�[�^�敪
        <VBFixedString(8)> Public NK2 As String     '���v����
        <VBFixedString(13)> Public NK3 As String    '���v���z
        <VBFixedString(107)> Public NK4 As String   '�_�~�[
        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 8), _
                            SubData(NK3, 13), _
                            SubData(NK4, 107) _
                            })
            End Get
            Set(ByVal Value As String)
                NK1 = CuttingData(Value, 2)
                NK2 = CuttingData(Value, 8)
                NK3 = CuttingData(Value, 13)
                NK4 = CuttingData(Value, 107)
            End Set
        End Property
    End Structure
    Public NENKIN_REC8 As NKRECORD8

    '�G���h���R�[�h
    Structure NKRECORD9
        Implements CFormat.IFormat
        <VBFixedString(2)> Public NK1 As String     '�f�[�^�敪
        <VBFixedString(8)> Public NK2 As String     '�����v����
        <VBFixedString(13)> Public NK3 As String    '�����v���z
        <VBFixedString(107)> Public NK4 As String   '�_�~�[

        '�Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            { _
                            SubData(NK1, 2), _
                            SubData(NK2, 8), _
                            SubData(NK3, 13), _
                            SubData(NK4, 107) _
                            })
            End Get
            Set(ByVal Value As String)
                NK1 = CuttingData(Value, 2)
                NK2 = CuttingData(Value, 8)
                NK3 = CuttingData(Value, 13)
                NK4 = CuttingData(Value, 107)
            End Set
        End Property
    End Structure
    Public NENKIN_REC9 As NKRECORD9

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"1", "2", "8", "9"}

        FtranPfile = "130.P"
        FtranIBMPfile = "130IBM.P"
        FtranIBMBinaryPfile = "130READ.P"

        CMTBlockSize = 1300

        HeaderKubun = New String() {"10", "11"}
        DataKubun = New String() {"23"}
        TrailerKubun = New String() {"83"}
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
        Dim buff As New StringBuilder(RecordLen)

        Select Case RecordData.Substring(0, 1)
            Case "10", "11"         ' �w�b�_���R�[�h
            Case "23"               ' �f�[�^���R�[�h
            Case "81", "82", "83"   ' �g���[��
            Case "90"               ' �G���h
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

        Select Case RecordData.Substring(0, 2)
            Case "10", "11"
                sRet = CheckRecord1()
            Case "23"
                sRet = CheckRecord2()
            Case "81", "82"
                sRet = CheckRecord81_82()
            Case "83"
                sRet = CheckRecord8()
            Case Else
                Select Case RecordData.Substring(0, 1)
                    Case "9"
                        sRet = CheckRecord9()
                    Case Else
                        DataInfo.Message = "���R�[�h�敪�ُ�i" & RecordData.Substring(0, 2) & "�j�ُ�"
                        mnErrorNumber = 1
                        Return "ERR"
                End Select
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
        NENKIN_REC1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = "1"
        InfoMeisaiMast.SYUBETU_CODE = NENKIN_REC1.NK2
        InfoMeisaiMast.CODE_KBN = NENKIN_REC1.NK3
        InfoMeisaiMast.ITAKU_CODE = mInfoComm.INFOToriMast.ITAKU_CODE_T
        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(NENKIN_REC1.NK8, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = NENKIN_REC1.NK8

        Dim OraReader As CASTCommon.MyOracleReader = GetTENMAST(InfoMeisaiMast.ITAKU_KIN, InfoMeisaiMast.ITAKU_SIT)
        If Not OraReader Is Nothing Then
            InfoMeisaiMast.I_SIT_NNAME = OraReader.GetItem("SIT_NNAME_N")
            OraReader.Close()
        End If

        Dim ToriCode As String = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & InfoMeisaiMast.SYUBETU_CODE)
        If ToriCode <> "err" Then
            Call GetTorimastFromToriCode(ToriCode, OraDB)
        End If

        '�f�[�^�`�F�b�N�i�N���U���ݗp�j
        If CheckHeaderRecord() = False Then
            Return "ERR"
        End If

        InfoMeisaiMast.ITAKU_KNAME = ""

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
        NENKIN_REC2.Data = RecordData

        '���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "2"
        InfoMeisaiMast.KEIYAKU_KIN = NENKIN_REC2.NK4
        InfoMeisaiMast.KEIYAKU_SIT = NENKIN_REC2.NK7
        InfoMeisaiMast.KEIYAKU_KAMOKU = NENKIN_REC2.NK9
        If InfoMeisaiMast.KEIYAKU_KAMOKU.Trim = "" Then
            InfoMeisaiMast.KEIYAKU_KAMOKU = "1"
        End If
        InfoMeisaiMast.KEIYAKU_KOUZA = NENKIN_REC2.NK10.Substring(3, 7)
        InfoMeisaiMast.KEIYAKU_KNAME = NENKIN_REC2.NK11
        InfoMeisaiMast.FURIKIN = CASTCommon.CaDecNormal(NENKIN_REC2.NK12)
        InfoMeisaiMast.FURIKIN_MOTO = NENKIN_REC2.NK12
        InfoMeisaiMast.SINKI_CODE = "0"
        InfoMeisaiMast.KEIYAKU_NO = New String("0"c, 10)
        InfoMeisaiMast.JYUYOKA_NO = (InfoMeisaiMast.SYUBETU_CODE & NENKIN_REC2.NK13).PadRight(20, " "c)

        InfoMeisaiMast.FURIKETU_CODE = 0
        InfoMeisaiMast.FURIKETU_MOTO = "0"

        InfoMeisaiMast.YOBI1 = Cutting(NENKIN_REC2.NK8, 15)
        NENKIN_REC2.NK8 = InfoMeisaiMast.YOBI1
        InfoMeisaiMast.YOBI2 = NENKIN_REC2.NK2
        InfoMeisaiMast.YOBI3 = NENKIN_REC2.NK13.Trim

        Try
            If (Not mInfoComm Is Nothing) Then
                Select Case mInfoComm.INFOToriMast.TEKIYOU_KBN_T
                    Case "0"
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                    Case "1"
                        InfoMeisaiMast.KTEKIYO = ""
                        InfoMeisaiMast.NTEKIYO = mInfoComm.INFOToriMast.NTEKIYOU_T
                    Case Else
                        InfoMeisaiMast.KTEKIYO = mInfoComm.INFOToriMast.KTEKIYOU_T
                        InfoMeisaiMast.NTEKIYO = ""
                End Select
            End If
        Catch ex As Exception
        End Try

        '���[�o�͍��ڗp�ɋ��Z�@�֖��A�X�ܖ���ǉ�
        Call InfoMeisaiMast2.Init()
        InfoMeisaiMast2.KEIYAKU_KIN_KNAME = NENKIN_REC2.NK5
        InfoMeisaiMast2.KEIYAKU_SIT_KNAME = NENKIN_REC2.NK8

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        '�x�X�R�[�h�������ꍇ�͎x�X�����画�ʂ���
        If InfoMeisaiMast.KEIYAKU_SIT.Trim = "" Then
            InfoMeisaiMast.KEIYAKU_SIT = SIT_CODE_TABLE.GetText(InfoMeisaiMast.YOBI1.Trim)
            If InfoMeisaiMast.KEIYAKU_SIT.Trim = "" Then
                '�ϊ��ł��Ȃ��ꍇ
                InfoMeisaiMast.KEIYAKU_SIT = SIT_CODE
            End If
        End If

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
    Protected Function CheckRecord81_82() As String
        NENKIN_REC8.Data = RecordData

        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "8"

        Return "T0"
    End Function

    '
    ' �@�\�@ �F �g���[���[���R�[�h�`�F�b�N
    '
    ' �߂�l �F True - �����C False - ���s
    '
    ' ���l�@ �F
    '
    Protected Function CheckRecord8() As String
        NENKIN_REC8.Data = RecordData

        ' ���׃}�X�^���ڐݒ� 
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "8"
        InfoMeisaiMast.TOTAL_IRAI_KEN = CASTCommon.CaDecNormal(NENKIN_REC8.NK2)
        InfoMeisaiMast.TOTAL_IRAI_KIN = CASTCommon.CaDecNormal(NENKIN_REC8.NK3)
        InfoMeisaiMast.TOTAL_ZUMI_KEN = 0
        InfoMeisaiMast.TOTAL_ZUMI_KIN = 0
        InfoMeisaiMast.TOTAL_FUNO_KEN = 0
        InfoMeisaiMast.TOTAL_FUNO_KIN = 0

        InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO = NENKIN_REC8.NK2
        InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO = NENKIN_REC8.NK3
        InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO = "0"
        InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO = "0"

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
        NENKIN_REC9.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "9"

        Return "E"
    End Function

    '
    ' �@�\�@ �F �w�b�_���R�[�h��ǂݍ���Ń`�F�b�N
    '
    ' ���l�@ �F �e�t�H�[�}�b�g�`�F�b�N����C�Ă΂�鋤�ʊ֐�
    '�@�@�@     �w�b�_��񂩂�C�����}�X�^���Q�Ƃ���
    '
    Protected Overrides Function CheckHeaderRecord(Optional ByVal mode As Integer = 0) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader

        ' �c�a�ڑ������݂��Ȃ��ꍇ�C����l��Ԃ�
        If OraDB Is Nothing Then
            Return True
        End If

        ' ���U
        '�����R�[�h�����i�}���`�̏ꍇ���l������j�A�ϑ��҃R�[�h�̃`�F�b�N
        SQL.Append("SELECT")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(",TYUUDAN_FLG_S")
        SQL.Append(",TOUROKU_FLG_S")
        SQL.Append(",UKETUKE_FLG_S")
        SQL.Append(",BAITAI_CODE_T")
        SQL.Append(" FROM TORIMAST,SCHMAST")
        SQL.Append(" WHERE ")

        Dim ToriCode As String
        ' �N����ʂ��画�f 61:�������N��,62:���D���N��,63:�������N��,64:�J�ДN��,65:�V�����N���E�����N��,66:�V�D���N��,67:�������N���Z��
        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & NENKIN_REC1.NK2)
        If ToriCode <> "err" Then
            SQL.Append("     TORIS_CODE_T = " & SQ(ToriCode.PadRight(12).Substring(0, 10)))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(ToriCode.PadRight(12).Substring(10, 2)))
        Else
            WriteBLog("�N�� �����R�[�h�擾", "���s", "��ڃR�[�h�F" & NENKIN_REC1.NK2 & " �����R�[�h�F" & ToriCode & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
            DataInfo.Message = "�N�� �����R�[�h�擾 ��ڃR�[�h�F" & NENKIN_REC1.NK2 & " �����R�[�h�F" & ToriCode & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            ' �����}�X�^�����N���A����
            Call mInfoComm.GetTORIMAST("", "")
            Return False
        End If

        'SQL.Append("   AND ITAKU_CODE_T = " & SQ(InfoMeisaiMast.ITAKU_CODE))

        SQL.Append("   AND FURI_DATE_S = " & SQ(mInfoComm.INFOParameter.FURI_DATE))
        SQL.Append("   AND '1'          = FSYORI_KBN_T")
        SQL.Append("   AND TORIS_CODE_S = TORIS_CODE_T")
        SQL.Append("   AND TORIF_CODE_S = TORIF_CODE_T")

        OraReader = New CASTCommon.MyOracleReader(OraDB.OracleConnection, OraDB.OracleTransaction)
        If OraReader.DataReader(SQL) = False Then
            WriteBLog("�t�@�C���w�b�_����斔�̓X�P�W���[������", "���s", "��ڃR�[�h�F" & NENKIN_REC1.NK2 & " �����R�[�h�F" & ToriCode & " �ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"))
            DataInfo.Message = "����斔�̓X�P�W���[���������s ��ڃR�[�h�F" & NENKIN_REC1.NK2 & " �����R�[�h�F" & ToriCode & " �ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T & " �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            ' �����}�X�^�����N���A����
            Call mInfoComm.GetTORIMAST("", "")
            OraReader.Close()
            Return False
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

        '2007/07/04�@�t�H�[�}�b�g�敪�A�}�̃R�[�h�A���f�t���O�A�o�^�t���O�`�F�b�N
        '�t�H�[�}�b�g�敪�̃`�F�b�N
        If mInfoComm.INFOToriMast.FMT_KBN_T <> mInfoComm.INFOToriMast.FMT_KBN_T Then
            WriteBLog("�t�H�[�}�b�g�敪�ُ�", "�G���[����")
            DataInfo.Message = " �����}�X�^�o�^�ُ�F�t�H�[�}�b�g�敪"
            Return False
        End If

        If OraReader.GetItem("TYUUDAN_FLG_S") <> "0" Then
            WriteBLog("�X�P�W���[��:���f�t���O�ݒ�� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��"), "���f")
            DataInfo.Message = "�X�P�W���[��:���f�t���O�ݒ�� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            Return False
        End If
        If OraReader.GetItem("UKETUKE_FLG_S") <> "0" Then
            WriteBLog("�X�P�W���[��:�������ݏ����� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��") & "��ڃR�[�h�F" & NENKIN_REC1.NK2 & " �����R�[�h�F" & ToriCode & " �ϑ��҃R�[�h�F" & mInfoComm.INFOToriMast.ITAKU_CODE_T, "���f")
            DataInfo.Message = "�X�P�W���[��:�������ݏ����� �U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "yyyy�NMM��dd��")
            Return False
        End If

        ' �I���N��Reader�N���[�Y
        OraReader.Close()

        WriteBLog("�t�@�C���w�b�_����挟��", "����", "�����R�[�h�F" & mInfoComm.INFOToriMast.TORIS_CODE_T & "-" & mInfoComm.INFOToriMast.TORIF_CODE_T)

        If InfoMeisaiMast.FURIKAE_DATE <> mInfoComm.INFOParameter.FURI_DATE Then
            '�U�֓��s��v
            '�U�֓�������Ă���ƈُ�I������ꍇ
            WriteBLog("�t�@�C���w�b�_�U�֓�", "�s��v", "�U�֓��F" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "MM��dd��"))
            DataInfo.Message = "�t�@�C���w�b�_�U�֓��s��v:" & ConvertDate(InfoMeisaiMast.FURIKAE_DATE, "MM��dd��")
            Return False
        End If

        Return True
    End Function
End Class
