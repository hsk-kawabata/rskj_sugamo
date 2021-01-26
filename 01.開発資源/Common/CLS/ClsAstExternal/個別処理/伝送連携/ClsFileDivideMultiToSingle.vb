'========================================================================
'ClsFileDivideMultiToSingle
'�˗��t�@�C�������c�[���N���X
'
'�˗��f�[�^�t�@�C���i128128128�E�E�E9�`���j��ǂݍ��݁A�����R�[�h�敪
'�iJIS/JIS������/EBCDIC�j�𔻒�̏�A�V���O���`���t�@�C���ɕ������܂��B
'�����P�ʂ́u�w�b�_���R�[�h�`�g���[�����R�[�h�v�Ƃ��܂��B
'
'���^�p���[����
'���ǂݍ��ރt�@�C���̕����R�[�h��SHIFT-JIS�ASHIFT-JIS�i���s����j�AEBCDIC���T�|�[�g���܂��B
'���ǂݍ��݃t�@�C���͔C�ӂ̃f�B���N�g������I�т܂��B��x�ɓǂݍ��߂�t�@�C�����͂ЂƂ݂̂ł��B
'��������̃t�@�C���́A���̂܂ܑ����U�̈ꊇ�������ݗp�t�H���_�iini�t�@�C���ŁAENTRI-SOUENTRYREAD�j�֊i�[����܂��B
'
'�쐬���F2010/04/15
'�쐬�ҁFm-fukuoka
'
'���l�F
'2010/04/15 �V�K�쐬
'========================================================================
Imports System
Imports System.IO
Imports System.Text
Imports System.Diagnostics.StackTrace
Imports System.Globalization

Public Class ClsFileDivideMultiToSingle

#Region "�N���X�ϐ���`"

    Private Const LOG_RES_STRING_OK As String = "����"
    Private Const LOG_RES_STRING_ERR As String = "�G���["
    Private Const LOG_RES_STRING_NG As String = "���s"

    '�����R�[�h�敪
    Private iCharCdKbn As Integer
    '�V�X�e�����t
    Private strSysDate As String = DateTime.Today.ToString("yyyyMMdd")
    '�V�X�e������
    Private strSysTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")
    '�G���h���R�[�h�ۊǗp�iNothing�������j
    Private bufEndRecord As Byte() = Nothing
    '�o�̓t�H���_�p�X
    Private strOutDirPath As String = String.Empty

    '���O�o�̓N���X�C���X�^���X
    Private MainLOG As New CASTCommon.BatchLOG("KFD011", "�t�@�C������")

#End Region

#Region "�N���X�萔��`"

    '�����R�[�h�敪�iSJIS�^SJIS���^EBCDIC�j
    Private Const CHARCD_KBN_SJIS As Integer = 1
    Private Const CHARCD_KBN_SJISKAI As Integer = 2
    Private Const CHARCD_KBN_EBCDIC As Integer = 3
    Private Const CHARCD_KBN_OTHER As Integer = 9

    '���R�[�h�敪�i�w�b�_�^�f�[�^�^�g���[���^�G���h�j
    Private Const REC_KBN_H As Integer = 1
    Private Const REC_KBN_D As Integer = 2
    Private Const REC_KBN_T As Integer = 8
    Private Const REC_KBN_E As Integer = 9
    Private Const REC_KBN_NG As Integer = 0

    '���O�o�͗p�\����
    Dim clsUserID As String            '���[�UID
    Dim clsToriCode As String          '�����啛�R�[�h
    Dim clsFuriDate As String          '�U�֓�

#End Region

#Region "�v���p�e�B�ϐ�"

    '���̓t�@�C���p�X
    Public WriteOnly Property InFilePath() As String
        Set(ByVal Value As String)
            Me.strInFilePath = Value
        End Set
    End Property
    Private strInFilePath As String

    '���R�[�h���i�����l�͋K�背�R�[�h���A��Ƀv���O�������ŉ��s�R�[�h�������������j
    Public WriteOnly Property RecordLength() As Integer
        Set(ByVal Value As Integer)
            Me.iRecordLength = Value
        End Set
    End Property
    Private iRecordLength As Integer

    '�t�@�C����
    Public Property FileCount() As Integer
        Get
            Return Me.iFileCount
        End Get
        Set(ByVal Value As Integer)
            Me.iFileCount = Value
        End Set
    End Property
    Private iFileCount As Integer = 0

    '���[�U�h�c
    Public WriteOnly Property UserId() As String
        Set(ByVal Value As String)
            Me.strUserId = Value
        End Set
    End Property
    Private strUserId As String

    '�����R�[�h
    Public WriteOnly Property ToriCode() As String
        Set(ByVal Value As String)
            Me.strToriCode = Value
        End Set
    End Property
    Private strToriCode As String

    '�U�֓�
    Public WriteOnly Property FuriDate() As String
        Set(ByVal Value As String)
            Me.strFuriDate = Value
        End Set
    End Property
    Private strFuriDate As String

#End Region

#Region "���C�����\�b�h"

    '=======================================================================
    'FileDivideMain
    '
    '���T�v��
    '�@�t�@�C�������c�[�����C�����\�b�h�ł��B
    '
    '���p�����[�^��
    '�@�Ȃ�
    '
    '���߂�l��
    '�@1�F�t�@�C���Ȃ�
    '�@2�F�t�@�C�����e�擾�G���[
    '  3�F�s���ȕ����R�[�h
    '  4�F�s���ȃ��R�[�h�敪
    '  5�F�t�@�C���o�̓G���[
    '  6�Fini�t�@�C���擾�G���[
    '�@9�F�v���I�G���[
    '�@0�F����I��
    '=======================================================================
    Public Function DivideFileMain(ByRef SplitFileName As ArrayList) As Integer

        Try

            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���C��", "�J�n", "")

            '=======================================================
            '�t�@�C�����݊m�F
            '=======================================================
            If File.Exists(Me.strInFilePath) = False Then
                '�t�@�C���Ȃ��G���[
                Return 1
            End If

            '=======================================================
            'ini�t�@�C���ǂݍ���
            '=======================================================
            If Me.GetIniData() = False Then
                'ini�t�@�C���擾�G���[
                Return 6
            End If

            '=======================================================
            '�t�@�C���I�[�v���E�����l�擾
            '=======================================================
            '�t�@�C���I�[�v���A�o�C�g�z��擾
            SplitFileName = New ArrayList
            SplitFileName.Clear()
            Dim bArray As Byte() = Me.OpenFileAndGetByte()
            If bArray Is Nothing Then
                '�t�@�C�����e�擾�G���[
                Return 2
            End If

            '=======================================================
            '���R�[�h�؂�o��
            '=======================================================
            '�؂�o���J�n�I�t�Z�b�g
            Dim iOffset As Integer = 0
            '�o�͗p�o�b�t�@���X�g�i���R�[�h�P�ʊi�[�j
            Dim alBufArraySJIS As New ArrayList

            '�����R�[�h�`�F�b�N�iSJIS or EBCDIC�j
            Dim blRetValue As Boolean = Me.CheckCharCdKbn(bArray(0), Me.iCharCdKbn)
            If Me.iCharCdKbn = CHARCD_KBN_OTHER Then
                '�����R�[�h�敪���z��O
                Return 3
            End If

            '�����R�[�h��SHIFT_JIS�̏ꍇ�́A���s�̑��݃`�F�b�N
            'EBCDIC�̏ꍇ�͉��s������ӎ����Ȃ�
            If Me.iCharCdKbn = CHARCD_KBN_SJIS Then
                If Me.CheckIsIncludeCRLF(bArray, Me.iRecordLength) = True Then
                    'S-JIS������ �����R�[�h�敪�X�V
                    iCharCdKbn = CHARCD_KBN_SJISKAI
                    '���s����2�o�C�g�����R�[�h���ɒǉ�
                    Me.iRecordLength += 2
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���C��", "����", "�����R�[�h�FSJIS���s����")
                End If
            End If


            '�G���h���R�[�h�̒��o�E�ێ�
            If Me.GetEndRecord(bArray) = False Then
                '�G���h���R�[�h���o���ʁA�G���h���R�[�h�ł͂Ȃ�����
                Return 4
            End If


            '�t�@�C���̍ŏ�����Ō�܂ő�����
            While iOffset < bArray.Length

                '���R�[�h�����o�C�g�z�񂩂�؂�o�����s��
                Dim bTgtArray As Byte() = Me.CutToRecord(bArray, iOffset)

                '=======================================================
                '���R�[�h����E�t�@�C���o��
                '=======================================================
                '�؂�o�������R�[�h�̃��R�[�h�敪����
                Dim iRecKbn As Integer = Me.CheckRecordKbn(bTgtArray(0))
                If iRecKbn = REC_KBN_NG Then
                    '�s���ȃ��R�[�h�敪
                    Return 4
                End If

                '�G���h���R�[�h�Ȃ�Ώ������΂�
                If iRecKbn = REC_KBN_E Then
                    GoTo NEXT_RECORD
                End If

                '���R�[�h���X�g�ɒǉ�
                alBufArraySJIS.Add(bTgtArray)

                If iRecKbn = REC_KBN_T Then
                    '�g���[�����R�[�h�������ꍇ�A�G���h���R�[�h��t������
                    '�t�@�C���o�͂��s��

                    alBufArraySJIS.Add(Me.bufEndRecord)

                    If alBufArraySJIS.Count > 0 Then
                        If Me.CreateFile(alBufArraySJIS, CHARCD_KBN_SJIS, SplitFileName) = False Then
                            Return 5
                        End If
                        '�o�b�t�@�N���A
                        alBufArraySJIS.Clear()
                    End If
                End If

NEXT_RECORD:
                '�I�t�Z�b�g���Z�i���R�[�h����������j
                iOffset += Me.iRecordLength
            End While

            '����I��
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���C��", "����", "")
            Return 0

        Catch ex As Exception
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���C��", "���s", ex.Message)
            Return 9
        Finally
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���C��", "�I��", "")
        End Try

    End Function

    '=======================================================================
    'GetIniData
    '���T�v��
    '�@�ݒ�t�@�C����������擾���A�O���[�o���ϐ��֊i�[���܂��B
    '
    '���p�����[�^��
    '�@�Ȃ�
    '
    '���߂�l��
    '�@�Ȃ�
    '
    '�����l��
    '=======================================================================
    Private Function GetIniData() As Boolean

        Try

            Me.strOutDirPath = CASTCommon.GetFSKJIni("COMMON", "DENBK")
            If Me.strOutDirPath = "err" OrElse Me.strOutDirPath = "" Then
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ini�t�@�C�����擾", "���s", "[COMMON]-[DENBK]")
                Return False
            Else
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ini�t�@�C�����擾", "����", Me.strOutDirPath)
            End If

            Return True

        Catch ex As Exception
            '�ُ탍�O
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "ini�t�@�C�����擾", "���s", ex.Message)
            Return False
        End Try

    End Function

    '=======================================================================
    'OpenFileAndGetByte
    '
    '���T�v��
    '�@�t�@�C�����J���A���e���o�C�g�z��ŕԂ��܂��B
    '
    '���p�����[�^��
    '�@�Ȃ�
    '
    '���߂�l��
    '�@�t�@�C�����e�o�C�g�z��
    '=======================================================================
    Private Function OpenFileAndGetByte() As Byte()

        Dim fs As FileStream = Nothing

        Try
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�t�@�C���Ǎ�", "����", "")

            '�t�@�C���I�[�v��
            fs = New FileStream(Me.strInFilePath, FileMode.Open, FileAccess.Read)
            If fs.Length = 0 Then
                '�t�@�C�����e�Ȃ��G���[
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�t�@�C���Ǎ�", "���s", "��t�@�C��")
                Return Nothing
            End If

            '�t�@�C�����e���o�C�g�z��Ɋi�[
            Dim byteArray(fs.Length - 1) As Byte
            fs.Read(byteArray, 0, fs.Length)

            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�t�@�C���Ǎ�", "����", "")
            Return byteArray

        Catch ex As Exception
            '��O����Nothing��Ԃ�
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�t�@�C���Ǎ�", "���s", ex.Message)
            Return Nothing
        Finally
            If Not fs Is Nothing Then
                '�t�@�C���N���[�Y
                fs.Close()
            End If
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�t�@�C���Ǎ�", "�I��", "")
        End Try

    End Function

    '=======================================================================
    'CheckCharCd
    '
    '���T�v��
    '�@�����R�[�h���`�F�b�N���A���ʂ�Ԃ��܂��B�n����镶���̓��R�[�h�敪���Ӗ�����
    '�@�u1/2/8/9�v�ł���K�v������܂��B
    '
    '���p�����[�^��
    '�@byteCheckTgt�F�`�F�b�N�Ώە����i�o�C�g�^�j
    '�@iRetValue�F�����R�[�h�敪�i1�FSHIFT_JIS�^3�FEBCDIC�^9�F����ΏۊO�j
    '
    '���߂�l��
    '�@TRUE�i�w�b�_���R�[�h�ł���j or FALSE�i�w�b�_���R�[�h�łȂ��@�܂��́@�G���[�j
    '=======================================================================
    Private Function CheckCharCdKbn(ByVal byteCheckTgt As Byte, _
                                    ByRef iRetValue As Integer) As Boolean

        Try
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���R�[�h���擾", "����", "�����R�[�h�FSJIS")

            Select Case Hex(byteCheckTgt)
                Case "31"
                    '�w�b�_���R�[�h�EJIS
                    iRetValue = CHARCD_KBN_SJIS
                    '���O�o��
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���R�[�h���擾", "����", "�����R�[�h�FSJIS")
                    Return True
                Case "F1"
                    '�w�b�_���R�[�h�EEBCDIC
                    iRetValue = CHARCD_KBN_EBCDIC
                    '���O�o��
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���R�[�h���擾", "����", "�����R�[�h�FEBCDIC")
                    Return True
                Case "32", "38", "39"
                    '�w�b�_���R�[�h�łȂ��@JIS
                    iRetValue = CHARCD_KBN_SJIS
                    Return False
                Case "F2", "F8", "F9"
                    '�w�b�_���R�[�h�łȂ��@EBCDIC
                    iRetValue = CHARCD_KBN_EBCDIC
                    Return False
                Case Else
                    '���R�[�h�敪�ł͂Ȃ�
                    iRetValue = CHARCD_KBN_OTHER
                    MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���R�[�h���擾", "���s", "�����R�[�h�F�s��")
                    Return False
            End Select

        Catch ex As Exception
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "���R�[�h���擾", "���s", ex.Message)
            Return False
        End Try

    End Function

    '=======================================================================
    'CheckIsIncludeCRLF
    '
    '���T�v��
    '�@���s�̗L���𔻒肵�܂��B�n���ꂽ�o�C�g�z�������̃��R�[�h���i���s�t���̏ꍇ��
    '�@���s���܂߂Ȃ������j�Ŋ���؂�邩�ǂ����Ŕ��f���܂��B
    '
    '���p�����[�^��
    '�@bArray�F�`�F�b�N�Ώە�����i�o�C�g�z��j
    '�@iRecLength�F�t�H�[�}�b�g�̋K�蒷�i�S��˗��t�@�C���̏ꍇ��120�j
    '
    '���߂�l��
    '�@TRUE�i���s����j or FALSE�i���s�Ȃ��j
    '=======================================================================
    Private Function CheckIsIncludeCRLF(ByVal bArray As Byte(), _
                                        ByVal iRecLength As Integer) As Boolean

        Dim iArrayLength As Integer = bArray.Length

        '�K�蕶�����ł�����؂�A���K�蕶�����{�Q�i���s���j�ł�����؂��
        '�܂�A���s���肩�Ȃ��������񒷂Ŕ���ł��Ȃ��ꍇ
        If iArrayLength Mod iRecLength = 0 And _
            iArrayLength Mod (iRecLength + 2) = 0 Then

            '���s�R�[�h������ׂ��ꏊ�̕������擾
            Dim strCheckString As String = Me.GetCrlfString(bArray, iRecLength)
            '���s�R�[�h���ۂ����`�F�b�N
            Return Me.CheckIsCRLF(strCheckString, Me.iCharCdKbn)
        End If

        '�K�蕶�����Ŋ���؂�遁���s�Ȃ�
        If iArrayLength Mod iRecLength = 0 Then
            Return False
        End If

        '�K�蕶�����{�Q�Ŋ���؂�遁���s����
        If iArrayLength Mod (iRecLength + 2) = 0 Then
            Return True
        End If

        Return False

    End Function

    '=======================================================================
    'GetCrlfString
    '
    '���T�v��
    '�@���s�R�[�h�����݂���ʒu�̒l��Ԃ��܂��B
    '
    '���p�����[�^��
    '�@byteArray�F�`�F�b�N�Ώە����i�o�C�g�z��j
    '�@iRecLength�F�t�H�[�}�b�g�̋K�蒷�i�S��˗��t�@�C���̏ꍇ��120�j
    '
    '���߂�l��
    '�@���s�R�[�h�ʒu�̕���
    '=======================================================================
    Private Function GetCrlfString(ByVal bArray As Byte(), _
                                   ByVal iRecLength As Integer) As String

        'CRLF����������ʒu�̒l�擾
        Dim sb As New StringBuilder
        With sb
            .Length = 0
            .Append(Hex(bArray(iRecLength)))
            .Append(Hex(bArray(iRecLength + 1)))
        End With

        Return sb.ToString

    End Function

    '=======================================================================
    'CheckIsCRLF
    '
    '���T�v��
    '�@�n���ꂽ���������s�R�[�h���ۂ��̔�����s���܂��B
    '
    '���p�����[�^��
    '�@strCheckString�F�`�F�b�N�Ώە���
    '�@iCharCd�F�����R�[�h�敪
    '
    '���߂�l��
    '�@���s�R�[�h�ʒu�̕���
    '=======================================================================
    Private Function CheckIsCRLF(ByVal strCheckString As String, _
                                 ByVal iCharCd As Integer) _
                                 As Boolean

        Select Case iCharCd
            Case 1
                'SHIFT_JIS
                If strCheckString <> "DA" Then
                    Return False
                End If
            Case 3
                'EBCDIC
                If strCheckString <> "D25" Then
                    Return False
                End If
            Case Else
                Return False
        End Select

        Return True

    End Function

    '=======================================================================
    'CutToRecord
    '
    '���T�v��
    '�@�n���ꂽ�o�C�g�z��̊J�n�ʒu����A���R�[�h�����̕�������o�C�g�z�񂩂�
    '�@�؂�o���܂��B
    '
    '���p�����[�^��
    '�@bArray�F������i�o�C�g�z��j
    '�@iOffset�F�؂�o���J�n�ʒu
    '
    '���߂�l��
    '�@�؂�o�����o�C�g�z��i���R�[�h�P�ʁj
    '=======================================================================
    Private Function CutToRecord(ByVal bArray As Byte(), _
                                 ByVal iOffset As Integer)

        '���R�[�h���Ŕz�񏉊���
        Dim bRetArray(Me.iRecordLength - 1) As Byte

        For index As Integer = 0 To Me.iRecordLength - 1 Step 1
            '�擾���z����A�V�K�z��֊i�[
            bRetArray(index) = bArray(index + iOffset)
        Next

        Return bRetArray

    End Function

    '=======================================================================
    'CheckRecordKbn
    '
    '���T�v��
    '�@���R�[�h�敪�𔻒肵�܂��B�n���ꂽ�o�C�g�����Ŕ��肵�܂��B
    '
    '���p�����[�^��
    '�@bCheckTgt�F�`�F�b�N�Ώە����i�o�C�g�^�j
    '
    '���߂�l��
    '�@���R�[�h�敪�i1/2/8/9/0�i�s���j�j
    '=======================================================================
    Private Function CheckRecordKbn(ByVal bCheckTgt As Byte) As Integer

        Try
            Dim str As String = Hex(bCheckTgt)

            Select Case Me.iCharCdKbn
                Case CHARCD_KBN_SJIS, CHARCD_KBN_SJISKAI
                    'SJIS�܂���SJIS������
                    Select Case str
                        Case "31"
                            '�w�b�_
                            Return REC_KBN_H
                        Case "32"
                            '�f�[�^
                            Return REC_KBN_D
                        Case "38"
                            '�g���[��
                            Return REC_KBN_T
                        Case "39"
                            '�G���h
                            Return REC_KBN_E
                        Case Else
                            '�s��
                            Return REC_KBN_NG
                    End Select

                Case CHARCD_KBN_EBCDIC
                    'EBCDIC
                    Select Case str
                        Case "F1"
                            '�w�b�_
                            Return REC_KBN_H
                        Case "F2"
                            '�f�[�^
                            Return REC_KBN_D
                        Case "F8"
                            '�g���[��
                            Return REC_KBN_T
                        Case "F9"
                            '�G���h
                            Return REC_KBN_E
                        Case Else
                            '�s��
                            Return REC_KBN_NG
                    End Select

                Case Else
                    '�s��
                    Return REC_KBN_NG

            End Select

        Catch ex As Exception
            '�s��
            Return REC_KBN_NG
        End Try

    End Function

    '=======================================================================
    'CheckRecordKbn
    '
    '���T�v��
    '�@���R�[�h�敪�𔻒肵�܂��B�n���ꂽ�o�C�g�����Ŕ��肵�܂��B
    '
    '���p�����[�^��
    '�@alBufferList�F���R�[�h�P�ʂɃo�C�g�z����i�[����ArrayList
    '�@iMode�F�t�@�C���쐬���[�h�i�����R�[�h�敪�Ɠ����j
    '
    '���߂�l��
    '=======================================================================
    Private Function CreateFile(ByVal alBufferList As ArrayList, _
                                ByVal iMode As Integer, _
                                ByRef SplitFileName As ArrayList) As Boolean

        '�o�͗p�o�C�g�z��
        Dim bOutputArray As Byte() = Nothing
        '�t�@�C���J�E���g���Z
        Me.iFileCount += 1

        Try
            '======================================================
            '�o�͗p�o�C�g�z��쐬
            '======================================================
            For i As Integer = 0 To alBufferList.Count - 1 Step 1

                'ArrayList���z�񒊏o
                Dim bCurArray As Byte() = alBufferList(i)

                '�o�͗p�z��̗v�f��ǉ�
                ReDim Preserve bOutputArray((i + 1) * Me.iRecordLength - 1)

                'ArrayList�̔z�񂩂�A�v�f�P�ʂŏo�͗p�o�C�g�z��֊i�[
                Dim index As Integer = 0
                For Each bt As Byte In bCurArray
                    bOutputArray((i * Me.iRecordLength) + index) = bt
                    index += 1
                Next

            Next

            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�o�͗p�o�b�t�@�쐬", "����", "")

        Catch ex As Exception
            '�ُ탍�O
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�o�͗p�o�b�t�@�쐬", "���s", ex.Message)
            Return False
        End Try

        '======================================================
        '�o�͐�w��
        '======================================================
        '�T�t�B�b�N�X�����i���[�h���V�X�e������ & �t�@�C���A�ԁi9999�j
        Dim strMode As String = String.Empty
        Select Case iMode
            Case CHARCD_KBN_SJIS
                strMode = "SJIS"
            Case CHARCD_KBN_SJISKAI
                strMode = "SJISK"
            Case CHARCD_KBN_EBCDIC
                strMode = "EBC"
        End Select

        Dim strAddStr As String = "_" & Me.iFileCount.ToString.PadLeft(3, "0"c)
        '�o�̓t�@�C�����i���̓t�@�C���̃t�@�C���������ɏ�L�̃T�t�B�b�N�X������ǉ��j
        Dim strOutFileName As String = _
            Path.GetFileNameWithoutExtension(Me.strInFilePath) & strAddStr & ".DAT"

        '�f�B���N�g�����Ȃ��ꍇ�͍쐬
        If Directory.Exists(Me.strOutDirPath) = False Then
            Directory.CreateDirectory(Me.strOutDirPath)
        End If

        '�o�̓p�X
        Dim strOutputFilePath As String = Path.Combine(Me.strOutDirPath, strOutFileName)

        Dim fs As FileStream = Nothing
        Try
            '�t�@�C���o�̓X�g���[��
            fs = New FileStream(strOutputFilePath, FileMode.Create, FileAccess.Write)
            '�t�@�C����������
            fs.Write(bOutputArray, 0, bOutputArray.Length)
            SplitFileName.Add(strOutputFilePath)

            '���탍�O
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�t�@�C���o��", "����", "")
            Return True

        Catch ex As Exception
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�t�@�C���o��", "���s", ex.Message)
            Return False
        Finally
            '�t�@�C���N���[�Y
            If Not fs Is Nothing Then
                '�t�@�C���N���[�Y
                fs.Close()
            End If
        End Try

    End Function

    '=======================================================================
    'GetEndRecord
    '
    '���T�v��
    '�@�t�@�C���̍ŏI���R�[�h�i�G���h���R�[�h�j���o�C�g�z��Ŏ擾���܂��B
    '
    '���p�����[�^��
    '�@bArray�F�t�@�C�����e���i�[�����o�C�g�z��
    '�@iMode�F�t�@�C���쐬���[�h�i�����R�[�h�敪�Ɠ����j
    '
    '���߂�l��
    '=======================================================================
    Private Function GetEndRecord(ByVal bArray As Byte()) As Boolean

        Try
            '�G���h���R�[�h�i�[�p�z��
            Dim bEndRecArray(Me.iRecordLength - 1) As Byte
            Dim j As Integer = 0

            '�t�@�C���̃o�C�g�z����A�ŏI�̃��R�[�h�𒊏o
            For i As Integer = bArray.Length - Me.iRecordLength To bArray.Length - 1 Step 1
                bEndRecArray(j) = bArray(i)
                j += 1
            Next

            If Me.CheckRecordKbn(bEndRecArray(0)) = REC_KBN_E Then
                '���o���ʂ��G���h���R�[�h��������A����I��
                '�O���[�o���֊i�[
                Me.bufEndRecord = bEndRecArray

                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�G���h���R�[�h�擾", "����", "")
                Return True
            Else
                '���o���ʂ��G���h���R�[�h�ł͂Ȃ����߁A�ُ�I��
                MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�G���h���R�[�h�擾", "���s", "�f�[�^���ŏI���R�[�h�v�m�F")
                Return False
            End If

        Catch ex As Exception
            '�ُ탍�O
            MainLOG.Write(Me.strUserId, Me.strToriCode, Me.strFuriDate, "�G���h���R�[�h�擾", "���s", ex.Message)
            Return False
        End Try

    End Function

#End Region

End Class
