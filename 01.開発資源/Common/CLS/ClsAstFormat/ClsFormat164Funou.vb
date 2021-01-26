Imports System
Imports System.IO
Imports System.Text
Imports System.Attribute
Imports CASTCommon.ModPublic

' ���U�s�\���ɕԊ҂l�s ���k�E�����E���E�����Z���^�[�t�H�[�}�b�g�i�W���j�N���X
Public Class FUNOU_164_DATA
    ' �f�[�^�t�H�[�}�b�g��{�N���X
    Inherits CFormat

    ' �f�[�^��
    Private Shadows ReadOnly RecordLen As Integer = 164

    '------------------------------------------------------------
    '���k�E�����E���E�����Z���^�[���U�t�H�[�}�b�g��`
    '------------------------------------------------------------

    '----------------
    '�f�[�^���R�[�h
    '----------------
    Structure FUNOU_164_Record
        Implements CFormat.IFormat

        Public JF1 As String     '�����݋��Z�@�փR�[�h
        Public JF2 As String     '�Ԋҋ敪
        Public JF3 As String     '�o�^�敪
        Public JF4 As String     '���Z�@�փR�[�h
        Public JF5 As String     '�X�܃R�[�h
        Public JF6 As String     '���R�[�h���
        Public JF7 As String     '�ȖڃR�[�h
        Public JF8 As String     '�����ԍ�
        Public JF9 As String     '�U�֓�
        Public JF10 As String    '���z
        Public JF11 As String    '���o���敪
        Public JF12 As String    '��ƃR�[�h
        Public JF13 As String    '��ƃV�[�P���X
        Public JF14 As String    '�������D��
        Public JF15 As String    '�U�փR�[�h
        Public JF16 As String    '�U�֑���Ȗ�
        Public JF17 As String    '�U�֑������
        Public JF18 As String    '�E�v�ݒ�敪
        Public JF19 As String    '�J�i�E�v
        Public JF20 As String    '�����E�v
        Public JF21 As String    '���v�Ɣԍ�
        Public JF22 As String    '�U�֌��ʃR�[�h
        Public JF23 As String    '�݂Ȃ�����
        Public JF24 As String    '������X��
        Public JF25 As String    '������Ȗ�
        Public JF26 As String    '���������
        Public JF27 As String    '�����㑊��Ȗ�
        Public JF28 As String    '�����㑊�����
        Public JF29 As String    '����ڋq�ԍ�
        Public JF30 As String    '�\��

        ' �Œ蒷�f�[�^�����p�v���p�e�B
        Public Property Data() As String Implements CFormat.IFormat.Data
            Get
                Return String.Concat(New String() _
                            {JF1, JF2, JF3, JF4, JF5, JF6, JF7, _
                             JF8, JF9, JF10, JF11, JF12, JF13, JF14, _
                             JF15, JF16, JF17, JF18, JF19, JF20, JF21, _
                             JF22, JF23, JF24, JF25, JF26, JF27, JF28, _
                             JF29, JF30 _
                             })
            End Get
            Set(ByVal Value As String)
                JF1 = CuttingData(Value, 4)
                JF2 = CuttingData(Value, 1)
                JF3 = CuttingData(Value, 1)
                JF4 = CuttingData(Value, 4)
                JF5 = CuttingData(Value, 3)
                JF6 = CuttingData(Value, 1)
                JF7 = CuttingData(Value, 2)
                JF8 = CuttingData(Value, 7)
                JF9 = CuttingData(Value, 6)
                JF10 = CuttingData(Value, 13)
                JF11 = CuttingData(Value, 1)
                JF12 = CuttingData(Value, 5)
                JF13 = CuttingData(Value, 8)
                JF14 = CuttingData(Value, 2)
                JF15 = CuttingData(Value, 3)
                JF16 = CuttingData(Value, 2)
                JF17 = CuttingData(Value, 7)
                JF18 = CuttingData(Value, 1)
                JF19 = CuttingData(Value, 13)
                JF20 = CuttingData(Value, 12)
                JF21 = CuttingData(Value, 24)
                JF22 = CuttingData(Value, 2)
                JF23 = CuttingData(Value, 1)
                JF24 = CuttingData(Value, 3)
                JF25 = CuttingData(Value, 2)
                JF26 = CuttingData(Value, 7)
                JF27 = CuttingData(Value, 2)
                JF28 = CuttingData(Value, 7)
                JF29 = CuttingData(Value, 7)
                JF30 = CuttingData(Value, 13)
            End Set
        End Property
    End Structure
    Public FUNOU_164_DATA As FUNOU_164_Record

    Private KEKKATXT As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "�U�֌��ʃR�[�h.TXT")

    ' New
    Public Sub New()
        MyBase.New()

        ' ���R�[�h���w��
        DataInfo.RecoedLen = RecordLen

        ' ���R�[�h�敪
        DataInfo.MinRecordCode = New String() {"D"}

        FtranPfile = "FUNOU_164.P"

        '�s�\�t�@�C����EBCDIC
        DataInfo.Encoding = EncodingType.EBCDIC

        HeaderKubun = New String() {""}
        DataKubun = New String() {"D"}
        TrailerKubun = New String() {""}
    End Sub

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

        sRet = CheckRecord2()

        Return sRet
    End Function

    '
    ' �@�\�@ �F �f�[�^���R�[�h�`�F�b�N
    '
    ' �߂�l �F "H" - �w�b�_�C"D" - �f�[�^�C"T" - �g���[���C"E" - �G���h
    '           "ERR" - �G���[����C"IJO" - �C���v�b�g�G���[

    ' ���l�@ �F
    '
    Protected Overridable Function CheckRecord2() As String
        FUNOU_164_DATA.Data = RecordData

        ' ���׃}�X�^���ڐݒ�
        Call InfoMeisaiMast.InitData()
        InfoMeisaiMast.DATA_KBN = "D"

        InfoMeisaiMast.FURIKAE_DATE = CASTCommon.ConvertDate(FUNOU_164_DATA.JF9, "yyyyMMdd")
        InfoMeisaiMast.FURIKAE_DATE_MOTO = FUNOU_164_DATA.JF9
        InfoMeisaiMast.KIGYO_CODE = FUNOU_164_DATA.JF12
        InfoMeisaiMast.KIGYO_SEQ = FUNOU_164_DATA.JF13
        InfoMeisaiMast.KEIYAKU_KIN = FUNOU_164_DATA.JF4
        InfoMeisaiMast.KEIYAKU_SIT = FUNOU_164_DATA.JF5
        InfoMeisaiMast.KEIYAKU_KAMOKU = FUNOU_164_DATA.JF7
        InfoMeisaiMast.KEIYAKU_KOUZA = FUNOU_164_DATA.JF8
        InfoMeisaiMast.FURIKIN_MOTO = FUNOU_164_DATA.JF10
        InfoMeisaiMast.FURIKIN = CaDecNormal(FUNOU_164_DATA.JF10)
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- START
        InfoMeisaiMast.NS_KBN = FUNOU_164_DATA.JF11         ' ���o���敪
        '2018/02/01 �^�X�N�j���� ADD �W���ŏC���F�L���M���Ή��i���U���M���O���G���^���̕s�\���ʍX�V�@�\�̒ǉ��j-- END
        InfoMeisaiMast.JYUYOKA_NO = FUNOU_164_DATA.JF21
        InfoMeisaiMast.FURIKETU_CENTERCODE = FUNOU_164_DATA.JF22
        ' 2008.04.02 �����}�X�^�̐U�֌��ʕϊ��e�[�u���h�c�s �ɂĔ��肷�邽��
        '            ���ۂɂt�o�c�`�s�d�����O�ŁC�U�֌��ʕԊ҂��s��
        'InfoMeisaiMast.FURIKETU_CODE = fn_FUNOU_KEKKA_YOMIKAE_2TO1(CASTCommon.CAInt32(FUNOU_164_DATA.JF16))

        '2009/12/29 �݂Ȃ������t���O�`�F�b�N�ǉ� FJH Append Start -------------------------------------------------->
        InfoMeisaiMast.MINASI = FUNOU_164_DATA.JF23
        '2009/12/29 �݂Ȃ������t���O�`�F�b�N�ǉ� FJH Append Start --------------------------------------------------<

        InfoMeisaiMast.TEISEI_SIT = FUNOU_164_DATA.JF24      ' �����p�x�X�R�[�h
        InfoMeisaiMast.TEISEI_KAMOKU = FUNOU_164_DATA.JF25   ' �����p�Ȗ�
        InfoMeisaiMast.TEISEI_KOUZA = FUNOU_164_DATA.JF26    ' �����p����
        InfoMeisaiMast.TEISEI_AKAMOKU = FUNOU_164_DATA.JF27  ' ��������p�Ȗ�
        InfoMeisaiMast.TEISEI_AKOUZA = FUNOU_164_DATA.JF28   ' ��������p����

        ' �˗������C�˗����z �J�E���g�Ώۃ��R�[�h
        InfoMeisaiMast.FURIKEN = 1

        '�s�\�t�@�C���̓f�[�^�`�F�b�N���Ȃ� ***
        '' �f�[�^�`�F�b�N
        'If MyBase.CheckDataRecord() = False Then
        '    Return "IJO"
        'End If
        '****************************************************************

        '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------START
        '�Ԋҋ敪�ǉ�
        InfoMeisaiMast.HENKANKBN = FUNOU_164_DATA.JF2           '�Ԋҋ敪
        InfoMeisaiMast.FURI_CODE = FUNOU_164_DATA.JF15          '�U�փR�[�h
        '2011/06/16 �W���ŏC�� �}�X�^�ɑ��݂��Ȃ����ׂ��l�� ------------------END
        Return "D"
    End Function

    Public Overrides Function IsDataRecord() As Boolean
        'If RecordData.StartsWith(HeaderKubun(0)) = False Then
        '    ' �w�b�_�łȂ���΁C���׃��R�[�h
            Return True
        'End If

        'Return False
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
