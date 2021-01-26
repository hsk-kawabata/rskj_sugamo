Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports CASTCommon.ModPublic

' ���U�s�\���ɕԊ҂l�s���C�Z���^�[�t�H�[�}�b�g�i�W���j�N���X
Public Class CFormatTokFunou
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 237

    '------------------------------
    '���C�Z���^�[���U�t�H�[�}�b�g��`
    '------------------------------
    '----------------
    '�w�b�_�[���R�[�h
    '----------------
    Structure TOKAI_FUNOU_Record1
        Implements CFormat.IFormat

        Public JF1 As String        ' ��Ǝ���
        Public JF2 As String        ' �f�[�^���
        Public JF3 As String        ' �T�C�N��
        Public JF4 As String        ' �\��
        Public JF5 As String        ' ��Ƌ敪
        Public JF6 As String        ' ���Z�@�փR�[�h
        Public JF7 As String        ' �x�X�R�[�h
        Public JF8 As String        ' �\��
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8})
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
    Public TOKAI_FUNOU_DATA1 As TOKAI_FUNOU_Record1

    '----------------
    '�f�[�^���R�[�h
    '----------------
    Structure TOKAI_FUNOU_Record2
        Implements CFormat.IFormat

        Public JF1 As String        ' ���U�w���
        Public JF2 As String        ' �Z���^�[�ꊇ�������
        Public JF3 As String        ' ���Z�@�փR�[�h
        Public JF4 As String        ' �X�܃R�[�h
        Public JF5 As String        ' ����D��R�[�h
        Public JF6 As String        ' ��ƃR�[�h
        Public JF7 As String        ' �Ȗ�
        Public JF8 As String        ' ����
        Public JF9 As String        ' �������D��R�[�h
        Public JF10 As String       ' �V�[�P���XNO
        Public JF11 As String       ' ���z
        Public JF12 As String       ' �U�։ȖځE����
        Public JF13 As String       ' �o�^�敪
        Public JF14 As String       ' ���ʕs�\STS
        Public JF15 As String       ' ���ʎ��U����STS
        Public JF16 As String       ' �s�\���R�R�[�h
        Public JF17 As String       ' �U�փR�[�h
        Public JF18 As String       ' �\��
        Public JF19 As String       ' �d�b�敪�R�[�h
        Public JF20 As String       ' �s�\�A����R�[�h
        Public JF21 As String       ' �S���҃R�[�h
        Public JF22 As String       ' �����E�v/�J�i�E�v
        Public JF23 As String       ' �戵�X��
        Public JF24 As String       ' �ڋq�ԍ�
        Public JF25 As String       ' ����
        Public JF26 As String       ' �x���\�c��
        Public JF27 As String       ' ���Ԓ����p�X��
        Public JF28 As String       ' ���Ԓ����p����
        Public JF29 As String       ' ��������p�Ȗڌ���
        Public JF30 As String       ' �������U�w���
        Public JF31 As String       ' ���v�Ɣԍ�
        Public JF32 As String       ' �����Ǘ�NO
        Public JF33 As String       ' �������Z�@�փR�[�h
        Public JF34 As String       ' �Ԋҋ敪
        Public JF35 As String       ' ���X����������
        Public JF36 As String       ' �Z���^�[�J�b�g�����R�[�h
        Public JF37 As String       ' ����ڋq�ԍ�
        Public JF38 As String       ' �d�b�ԍ�
        Public JF39 As String       ' �\��
        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8, JF9, JF10, JF11, JF12, JF13, JF14, _
                             JF15, JF16, JF17, JF18, JF19, JF20, JF21, _
                             JF22, JF23, JF24, JF25, JF26, JF27, JF28, _
                             JF29, JF30, JF31, JF32, JF33, JF34, JF35, _
                             JF36, JF37, JF38, JF39 _
                             })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 8)
                JF2 = CuttingData(Value, 3)
                JF3 = CuttingData(Value, 4)
                JF4 = CuttingData(Value, 3)
                JF5 = CuttingData(Value, 3)
                JF6 = CuttingData(Value, 5)
                JF7 = CuttingData(Value, 2)
                JF8 = CuttingData(Value, 7)
                JF9 = CuttingData(Value, 2)
                JF10 = CuttingData(Value, 8)
                JF11 = CuttingData(Value, 13)
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
                JF26 = CuttingData(Value, 13)
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
                JF38 = CuttingData(Value, 13)
                JF39 = CuttingData(Value, 8)
            End Set
        End Property
    End Structure
    Public TOKAI_FUNOU_DATA2 As TOKAI_FUNOU_Record2

    Private KEKKATXT As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�U�֌��ʃR�[�h.TXT")

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"H"}

        FtranPfile = "FUNOU_TOKAI.P"

        '*** �C�� mitsu 2008/07/14 �s�\�t�@�C����EBCDIC ***
        DataInfo.Encoding = EncodingType.EBCDIC
        '**************************************************

        HeaderKubun = New String() {"H"}
        DataKubun = New String() {"D"}
        TrailerKubun = New String() {""}
    End Sub

    '
    ' �@�\�@ �F ���R�[�h�`�F�b�N
    '
    ' �߂�l �F �s�������̈ʒu
    '
    ' ���l�@ �F
    '
    Public Overrides Function CheckRegularString() As Long
        Dim buff As New StringBuilder(RecordLen)

        Select Case RecordData.Substring(0, 1)
            Case "H"        ' �w�b�_���R�[�h
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
    Public Overrides Function CheckKekkaFormat() As String
        ' ��{�N���X �`�F�b�N
        Dim sRet As String = MyBase.CheckKekkaFormat()

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
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Public Overrides Function CheckRecord1() As String
        TOKAI_FUNOU_DATA1.Data = RecordData

        ' ���׃}�X�^��񏉊���
        Call InfoMeisaiMast.Init()

        ' ���׃}�X�^���ڐݒ�
        InfoMeisaiMast.DATA_KBN = "H"

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
        TOKAI_FUNOU_DATA2.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "D"

        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(TOKAI_FUNOU_DATA2.JF1, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = TOKAI_FUNOU_DATA2.JF1
        InfoMeisaiMast.KIGYO_CODE = TOKAI_FUNOU_DATA2.JF6
        InfoMeisaiMast.KIGYO_SEQ = TOKAI_FUNOU_DATA2.JF10
        InfoMeisaiMast.KEIYAKU_KIN = TOKAI_FUNOU_DATA2.JF3
        InfoMeisaiMast.KEIYAKU_SIT = TOKAI_FUNOU_DATA2.JF4
        InfoMeisaiMast.KEIYAKU_KAMOKU = TOKAI_FUNOU_DATA2.JF7
        InfoMeisaiMast.KEIYAKU_KOUZA = TOKAI_FUNOU_DATA2.JF8
        InfoMeisaiMast.FURIKIN_MOTO = TOKAI_FUNOU_DATA2.JF11
        InfoMeisaiMast.FURIKIN = CaDecNormal(TOKAI_FUNOU_DATA2.JF11)
        InfoMeisaiMast.JYUYOKA_NO = TOKAI_FUNOU_DATA2.JF31
        InfoMeisaiMast.FURIKETU_CENTERCODE = TOKAI_FUNOU_DATA2.JF16
        InfoMeisaiMast.FURI_CODE = TOKAI_FUNOU_DATA2.JF17   '2010/04/28 �U�փR�[�h�擾
        ' 2008.04.02 �����}�X�^�̐U�֌��ʕϊ��e�[�u���h�c�s �ɂĔ��肷�邽��
        '            ���ۂɂt�o�c�`�s�d�����O�ŁC�U�֌��ʕԊ҂��s��
        'InfoMeisaiMast.FURIKETU_CODE = fn_FUNOU_KEKKA_YOMIKAE_2TO1(CASTCommon.CAInt32(TOKAI_FUNOU_DATA2.JF16))

        Dim ���ʎ��U����STS As String
        Dim L_���ʎ��U����STS As Long
        Dim S_���ʎ��U����STS As String
        Dim ���ʎ��U����STS_1 As String
        Dim ���ʎ��U����STS_2 As String
        Dim ���ʎ��U����STS_3 As String
        Dim ���ʎ��U����STS_4 As String

        L_���ʎ��U����STS = 0
        S_���ʎ��U����STS = ""

        ���ʎ��U����STS = TOKAI_FUNOU_DATA2.JF15            '16�i���\�L
        '16�i���\�L��2�i���\�L�ɕϊ�����
        ���ʎ��U����STS_1 = ���ʎ��U����STS.Substring(3, 1)
        ���ʎ��U����STS_2 = ���ʎ��U����STS.Substring(2, 1)
        ���ʎ��U����STS_3 = ���ʎ��U����STS.Substring(1, 1)
        ���ʎ��U����STS_4 = ���ʎ��U����STS.Substring(0, 1)

        ���ʎ��U����STS_1 = fn_Change_10(���ʎ��U����STS_1)
        ���ʎ��U����STS_2 = fn_Change_10(���ʎ��U����STS_2)
        ���ʎ��U����STS_3 = fn_Change_10(���ʎ��U����STS_3)
        ���ʎ��U����STS_4 = fn_Change_10(���ʎ��U����STS_4)
        L_���ʎ��U����STS = CAInt32(���ʎ��U����STS_1) * 1 _
                        + CAInt32(���ʎ��U����STS_2) * 16 _
                        + CAInt32(���ʎ��U����STS_3) * 256 _
                        + CAInt32(���ʎ��U����STS_4) * 4096 '10�i�\�L
        Do
            S_���ʎ��U����STS = CStr(L_���ʎ��U����STS Mod 2) & S_���ʎ��U����STS
            L_���ʎ��U����STS = CType((L_���ʎ��U����STS / 2), Integer)

        Loop While L_���ʎ��U����STS > 0

        S_���ʎ��U����STS = CALng(S_���ʎ��U����STS).ToString("0000000000000000")
        InfoMeisaiMast.MINASI = S_���ʎ��U����STS.Substring(7, 1)
        ''1998/04/01 �݂Ȃ������� "1" �̏ꍇ�U�֌��ʃR�[�h�� 00 �ɕ␳����
        If InfoMeisaiMast.MINASI = "1" Then
            InfoMeisaiMast.FURIKETU_CODE = 0
        End If

        InfoMeisaiMast.TEISEI_SIT = TOKAI_FUNOU_DATA2.JF27
        InfoMeisaiMast.TEISEI_KAMOKU = TOKAI_FUNOU_DATA2.JF28.Substring(0, 2)   ' �����p�Ȗ�
        InfoMeisaiMast.TEISEI_KOUZA = TOKAI_FUNOU_DATA2.JF28.Substring(2, 7)    ' �����p����
        InfoMeisaiMast.TEISEI_AKAMOKU = TOKAI_FUNOU_DATA2.JF29.Substring(0, 2)  ' ��������p�Ȗ�
        InfoMeisaiMast.TEISEI_AKOUZA = TOKAI_FUNOU_DATA2.JF29.Substring(2, 7)   ' ��������p����

        '*** �C�� mitsu 2008/11/07 �������̎擾 ***
        If InfoMeisaiMast.TEISEI_SIT <> "000" Then
            InfoMeisaiMast.TEISEI_SIT = TOKAI_FUNOU_DATA2.JF4       'JF27,28�F�Ǒ֑O���(�˗�)�AJF4,7,8�F�Ǒ֌���
        End If
        If InfoMeisaiMast.TEISEI_KAMOKU <> "00" Then
            InfoMeisaiMast.TEISEI_KAMOKU = TOKAI_FUNOU_DATA2.JF7
        End If
        If InfoMeisaiMast.TEISEI_KOUZA <> "0000000" Then
            InfoMeisaiMast.TEISEI_KOUZA = TOKAI_FUNOU_DATA2.JF8
        End If
        '********************************************

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        '*** �C�� mitsu 2008/07/15 �s�\�t�@�C���̓f�[�^�`�F�b�N���Ȃ� ***
        '' �f�[�^�`�F�b�N
        'If MyBase.CheckDataRecord() = False Then
        '    Return "IJO"
        'End If
        '****************************************************************

        Return "D"
    End Function

    Public Overrides Function IsDataRecord() As Boolean
        If RecordData.StartsWith(HeaderKubun(0)) = False Then
            ' �w�b�_�łȂ���΁C���׃��R�[�h
            Return True
        End If

        Return False
    End Function

    '============================================================================
    'NAME           :fn_Change_10
    'Parameter      :
    'Description    :16�i����10�i���ɕϊ�
    'Return         :16�i����10�i���ɕϊ������l
    'Create         :2004/08/23
    'Update         :
    '============================================================================
    Private Function fn_Change_10(ByVal In_DATA As String) As String
        Select Case In_DATA
            Case "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
                Return In_DATA
            Case "A", "a"
                Return "10"
            Case "B", "b"
                Return "11"
            Case "C", "c"
                Return "12"
            Case "D", "d"
                Return "13"
            Case "E", "e"
                Return "14"
            Case "F", "f"
                Return "15"
            Case Else
                Return ""
        End Select
    End Function

    ' �@�\�@ �F �U�֌��ʃR�[�h�Ԋ�
    '
    ' ����   �F ARG1 - �U�֌��ʕϊ��e�[�u���h�c�s
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[
    '
    ' ���l�@ �F
    '
    Public Function SetFuriKetu(ByVal fKekkaTbl As String) As Integer
        If fKekkaTbl = "0" Then
            KEKKATXT = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�U�֌��ʃR�[�h.TXT")
        Else
            KEKKATXT = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�U�֌��ʃR�[�h_" & fKekkaTbl & ".TXT")
        End If

        Return fn_FUNOU_KEKKA_YOMIKAE_2TO1(CASTCommon.CAInt32(InfoMeisaiMast.FURIKETU_CENTERCODE))
    End Function

    ' �@�\�@ �F ���ʃR�[�h�Ԋ�
    '
    ' ����   �F ARG1 - �Z���^�[�U�֌��ʃR�[�h
    '
    ' �߂�l �F �U�֌��ʃR�[�h
    '
    ' ���l�@ �F
    '
    Private Function fn_FUNOU_KEKKA_YOMIKAE_2TO1(ByVal aFURIKETU_CODE_2 As Integer) As Integer
        Dim sRet As String = CASTCommon.GetIni(KEKKATXT, "KEKKA_CODE", aFURIKETU_CODE_2.ToString.Trim)
        If sRet <> "err" Then
            Return CASTCommon.CAInt32(sRet)
        End If

        sRet = CASTCommon.GetIni(KEKKATXT, "KEKKA_CODE", "ELSE")
        If sRet = "err" Then
            ' �G���[�̏ꍇ�C�W���̐U�֌��ʃR�[�h.TXT�ōă`�������W
            Dim sKekka As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�U�֌��ʃR�[�h.TXT")
            sRet = CASTCommon.GetIni(sKekka, "KEKKA_CODE", aFURIKETU_CODE_2.ToString.Trim)
            If sRet <> "err" Then
                Return CASTCommon.CAInt32(sRet)
            End If
            sRet = CASTCommon.GetIni(sKekka, "KEKKA_CODE", "ELSE")
            If sRet = "err" Then
                Return CASTCommon.CAInt32(sRet)
            End If

            Return 9
        End If

        Return CASTCommon.CAInt32(sRet)
    End Function
End Class
