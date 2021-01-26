Imports System
Imports System.IO
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch

' �e�V�X�e���Ƃ̘A�g�p�N���X
Public Class ClsRenkei
    ' ���̓t�@�C����
    Public InFileName As String
    ' �o�̓t�@�C����
    Public OutFileName As String
    ' ���b�Z�[�W
    Public Message As String = ""

    ' �p�����[�^���ʏ��
    Private mInfoArgument As CommData
    Public ReadOnly Property InfoArg() As CommData
        Get
            Return mInfoArgument
        End Get
    End Property

    '' �b�l�s���V�X �t�H���_
    '   �b�l�s
    Public Shared ReadOnly CMTOTHERPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "CMTREAD")
    Public Shared ReadOnly CMTOTHERWRTPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "CMTWRITE")
    '   �w�Z
    Public Shared ReadOnly SCHOOLOTHERPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "SCHOOLREAD")
    ' ���f�B�A�R���o�[�^
    Public Shared ReadOnly MEDIAOTHERPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "MEDIAREAD")
    Public Shared ReadOnly MEDIAOTHERWRTPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "MEDIAWRITE")
    ' �ėp�G���g��
    Public Shared ReadOnly HANREADPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "HANREAD")

    Public Shared ReadOnly TXTPATH As String = CASTCommon.GetFSKJIni("COMMON", "TXT")

    Public Shared ReadOnly OTHJIFFOLDER As String = "JIFURI\JFR"            ' ���U
    Public Shared ReadOnly OTHSOKFOLDER As String = "SOUKYUFURI\SFR"        ' �U��

    ' �x�[�X�t�H���_
    Private Shared BasePath As String = CASTCommon.GetFSKJIni("TOUROKU", "PATH")

    ' �A�g�敪 �t�H���_��
    '' ���f�B�A�R���o�[�^
    Public Shared MEDIAPATH As String = Path.Combine(BasePath, "I_MEDIA")
    '' �ėp�G���g��
    Public Shared HENTRYPATH As String = Path.Combine(BasePath, "I_H-ENTRY")
    '' �w�Z
    Public Shared SCHOOLPATH As String = Path.Combine(BasePath, "I_SCHOOL")
    '' �b�l�s
    Public Shared CMTPATH As String = Path.Combine(BasePath, "I_CMT")
    '' �`��
    Public Shared DENSOUPATH As String = Path.Combine(BasePath, "II_DENSOU")
    '' ��
    Public Shared KOBETUPATH As String = Path.Combine(BasePath, "II_KOBETU")

    '' ���o�b�`
    Public Shared KBPATH As String = Path.Combine(BasePath, "III_KINBATCH")

    '' �r�r�b
    Public Shared SSCPATH As String = Path.Combine(BasePath, "III_SSC")

    ' �f�[�^�擾�t�H���_
    Public Shared ReadOnly GETFOLDER As String = "GET"
    ' �f�[�^������t�H���_
    Public Shared ReadOnly HOLDFOLDER As String = "HOLD"
    ' �f�[�^�ۑ�����t�H���_
    Public Shared ReadOnly NORFOLDER As String = "NORMAL"
    ' �f�[�^�ۑ��ُ�t�H���_
    Public Shared ReadOnly ERRFOLDER As String = "ERROR"
    ' �f�[�^�Ԋ҃t�H���_
    Public Shared ReadOnly SENDFOLDER As String = "SEND"

    ' �U�֏����敪 �t�H���_��
    Public Shared ReadOnly JIFFOLDER As String = "JIF"       ' ���U
    Public Shared ReadOnly SOKFOLDER As String = "SOK"       ' �U��
    Public Shared ReadOnly SSSFOLDER As String = "SKD"       ' �r�r�r

    Public Shared ReadOnly EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")   'JIS�G���R�[�h
    Public Shared ReadOnly EncdE As System.Text.Encoding = System.Text.Encoding.GetEncoding("IBM290")  'EBCDIC�G���R�[�h

    Private mRenkeiMode As Integer = 0

    ' �b�n�l�o�k�n�b�j�Í������
    Private Structure stCOMPLOCK
        Dim AES As String
        Dim KEY As String
        Dim IVKEY As String
        Dim RECLEN As String
    End Structure

    Sub New(Optional ByVal renkeiMode As Integer = 0)
        Dim AppName As String

        mRenkeiMode = renkeiMode
        Select Case renkeiMode
            Case 0                      ' �������ݏ���
                AppName = "TOUROKU"
            Case 1                      ' ���ʍX�V����
                AppName = "KEKKA"
            Case 2                      ' �Ԋҏ���
                AppName = "HENKAN"
            Case 3                      ' �������ό��ʍX�V����
                AppName = "KESSAIKEKKA"
            Case 4                      ' �����U���ʍX�V����
                AppName = "SOU_KEKKA"
            Case Else                   ' �������ݏ���
                AppName = "TOUROKU"
        End Select
        BasePath = CASTCommon.GetFSKJIni(AppName, "PATH")

        ' �A�g�敪 �t�H���_��
        '' ���f�B�A�R���o�[�^
        MEDIAPATH = Path.Combine(BasePath, "I_MEDIA")
        '' �ėp�G���g��
        HENTRYPATH = Path.Combine(BasePath, "I_H-ENTRY")
        '' �w�Z
        SCHOOLPATH = Path.Combine(BasePath, "I_SCHOOL")
        '' �b�l�s
        CMTPATH = Path.Combine(BasePath, "I_CMT")
        '' �`��
        DENSOUPATH = Path.Combine(BasePath, "II_DENSOU")
        '' ��
        KOBETUPATH = Path.Combine(BasePath, "II_KOBETU")
        '' ���o�b�`
        KBPATH = Path.Combine(BasePath, "III_KINBATCH")
    End Sub

    ' �@�\�@ �F New
    '
    ' ����   �F ARG1 - ���ʏ��
    '           ARG2 - �A�g���[�h�i�O�F�o�^�C�z�M�C�P�F���ʍX�V�C�Q�F�Ԋҁj
    '
    ' �߂�l �F �G���[�ԍ�
    '
    ' ���l�@ �F 0=�����A50=�t�@�C���Ȃ��A100=�R�[�h�ϊ����s�A200=�R�[�h�敪�ُ�iJIS���s����j�A
    '        �F 300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
    '
    Sub New(ByVal comm As CommData, Optional ByVal RenkeiMode As Integer = 0)
        MyClass.New(RenkeiMode)

        Message = ""
        OutFileName = ""

        mInfoArgument = comm

        '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
        'If RenkeiMode = 1 AndAlso comm.INFOParameter.RENKEI_KBN = "99" Then
        '    ' ���ʍX�V�������C�A�g�敪�̏�P�����X�̏ꍇ
        'Select Case comm.INFOParameter.FMT_KBN
        '    Case "MT"
        '        ' �Z���^�s�\�f�[�^
        '        InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DAT"), "FUNOU.DAT")
        '        Return
        '    Case "20", "21"
        '        ' �r�r�r�f�[�^
        '        InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DAT"), "FUNOU_SYUKINDAIKOU.DAT")
        '        Return
        '    Case Else
        '2009/09/14 ���R�����g��======================================
        'InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DAT"), comm.INFOParameter.RENKEI_FILENAME)
        'Return
        '======================================
        'End Select
        'End If
        '+++++++++++++++++++++++++++++++++++++++++++++++
        '+++++++++++++++++++++++++++++++++++++++++++++++
        '2009/09/12 �b��I�ɕs�\���ʃt�@�C����Ԃ��悤�ɕύX
        If RenkeiMode = 1 Then
            ' ���ʍX�V����
            Select Case comm.INFOParameter.FMT_KBN
                Case "MT"
                    InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DEN"), comm.INFOParameter.RENKEI_FILENAME)
                    Return
                    '2017/01/18 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
                Case "20", "21"
                    InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DEN"), comm.INFOParameter.RENKEI_FILENAME)
                    Return
                    '2017/01/18 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
                Case Else
                    InFileName = ""
                    Return
            End Select
        End If
        '===================================================
        If RenkeiMode = 2 Then
            ' �Ԋҏ��� �t�@�C����
            InFileName = Path.Combine(GetFolderName(SENDFOLDER), "")
            Return
        End If

        If RenkeiMode >= 3 AndAlso RenkeiMode <= 6 Then
            ' ���o�b�`���ʃt�@�C����
            InFileName = Path.Combine(GetFolderName(GETFOLDER), comm.INFOParameter.RENKEI_FILENAME)
            Return
        End If

        '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
        'If (comm.INFOParameter.RENKEI_KBN = "99" Or comm.INFOParameter.RENKEI_KBN = "88") Then

        ' ���Ƃ����ݏ���
        ' ���̓t�@�C����
        'Select Case mInfoArgument.INFOParameter.BAITAI_CODE
        '    Case "00"       ' �`���̏ꍇ
        '        ' �t�@�C�����ݒ�
        '        InFileName = GetKobetuFileName("DAT")
        '    Case Else
        '        ' �t�@�C�����ݒ�
        '        InFileName = GetKobetuFileName("DAT")
        'End Select
        'Else
        If comm.INFOParameter.RENKEI_FILENAME.Trim <> "" Then
            '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂��� +++++++
            'If ",02,00,07,20".IndexOf(comm.INFOParameter.RENKEI_KBN) >= 1 Then
            If comm.INFOParameter.BAITAI_CODE = "00" OrElse _
                comm.INFOParameter.BAITAI_CODE = "06" OrElse _
                comm.INFOParameter.BAITAI_CODE = "07" Then
                ' �b�l�s�C�`���C�w�Z
                InFileName = Path.Combine(GetFolderName(GETFOLDER), comm.INFOParameter.RENKEI_FILENAME.Trim)
            Else
                InFileName = Path.Combine(GetFolderName(HOLDFOLDER), comm.INFOParameter.RENKEI_FILENAME.Trim)
            End If
            '+++++++++++++++++++++++++++++++++++++++++++++++
        Else
            ' ���̓t�@�C����
            Select Case mInfoArgument.INFOParameter.BAITAI_CODE
                Case "00"       ' �`���̏ꍇ
                    ' �t�@�C�����ݒ�
                    InFileName = GetKobetuFileName("DAT")
                Case Else
                    ' �t�@�C�����ݒ�
                    InFileName = GetKobetuFileName("DAT")
            End Select
        End If
        'End If
        '+++++++++++++++++++++++++++++++++++++++++++++++
    End Sub

    '
    ' �@�\�@ �F �˗��f�[�^���R�s�[����
    '
    ' �߂�l �F �G���[�ԍ�
    '
    ' ���l�@ �F 0=�����A50=�t�@�C���Ȃ��A100=�R�[�h�ϊ����s�A200=�R�[�h�敪�ُ�iJIS���s����j�A
    '        �F 300=�R�[�h�敪�ُ�iJIS���s�Ȃ��j�A400=�o�̓t�@�C���쐬���s
    '
    Public Function CopyToDisk(ByVal readfmt As CAstFormat.CFormat) As Integer
        Dim nRet As Integer = 0          '��������
        Dim sWorkFilename As String '���[�N�t�@�C����

        If mInfoArgument.INFOParameter.RENKEI_FILENAME.Trim = "" Then

            '�o�̓t�@�C����
            OutFileName = Path.Combine(BasePath, String.Format("i{0}.dat", mInfoArgument.INFOParameter.TORI_CODE))

            '���̓t�@�C����
            '�t�@�C���̑��݃`�F�b�N
            ' 2017/11/27 �^�X�N�j���� CHG �A�n�M��(����������̧�قȂ��G���[������(SMB2.0)<ME>) -------------------- START
            'If File.Exists(InFileName) = False Then
            '    Message = "�t�@�C�������݂��܂���[�t�@�C�����F" & InFileName & "]"
            '    Return 50
            'End If
            For i As Integer = 1 To 40 Step 1
                If File.Exists(InFileName) = True Then
                    Threading.Thread.Sleep(500)
                    Exit For
                Else
                    If i = 40 Then
                        Message = "�t�@�C�������݂��܂���B[�t�@�C�����F" & InFileName & "] (" & i & ")"
                        Return 50
                    End If
                    Threading.Thread.Sleep(500)
                End If
            Next
            ' 2017/11/27 �^�X�N�j���� CHG �A�n�M��(����������̧�قȂ��G���[������(SMB2.0)<ME>) -------------------- END

            '���[�J���փR�s�[����
            sWorkFilename = CopyFileToWork(InFileName, OutFileName, _
                                            mInfoArgument.INFOParameter.JOBTUUBAN)
            If File.Exists(sWorkFilename) = False Then
                '�R�s�[���s
                Message = "�R�s�[���s[�t�@�C�����F" & InFileName & ":" & OutFileName & "]"
                Return 400
            End If
        Else
            ' 2017/11/27 �^�X�N�j���� CHG �A�n�M��(����������̧�قȂ��G���[������(SMB2.0)<ME>) -------------------- START
            'If File.Exists(InFileName) = False Then
            '    Message = "�t�@�C�������݂��܂���[�t�@�C�����F" & InFileName & "]"
            '    Return 50
            'End If
            For i As Integer = 1 To 40 Step 1
                If File.Exists(InFileName) = True Then
                    Threading.Thread.Sleep(500)
                    Exit For
                Else
                    If i = 40 Then
                        Message = "�t�@�C�������݂��܂���[�t�@�C�����F" & InFileName & "] (" & i & ")"
                        Return 50
                    End If
                    Threading.Thread.Sleep(500)
                End If
            Next
            ' 2017/11/27 �^�X�N�j���� CHG �A�n�M��(����������̧�قȂ��G���[������(SMB2.0)<ME>) -------------------- END

            OutFileName = Path.Combine(Path.GetDirectoryName(InFileName), String.Format("{0}_{1}.dat", _
                                        mInfoArgument.INFOParameter.TORI_CODE, _
                                        Path.GetFileName(InFileName)) _
                                        )
            '�s�\�t�@�C���̏ꍇ�̓t�@�C���̍쐬���ύX
            If Path.GetFileName(InFileName) = "FUNOU.DAT" Then
                OutFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DENBK"), "FUNOU_JIS.DAT")
                '2017/01/18 saitou ���t�M��(RSV2�W��) ADD �X���[�G�X�Ή� ---------------------------------------- START
            ElseIf Path.GetFileName(InFileName) = "FUNOU_SYUKINDAIKOU.DAT" Then
                OutFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DENBK"), "FUNOU_SYUKINDAIKOU_JIS.DAT")
                '2017/01/18 saitou ���t�M��(RSV2�W��) ADD ------------------------------------------------------- END
            End If
            sWorkFilename = CopyFileToWork(InFileName, OutFileName, mInfoArgument.INFOParameter.JOBTUUBAN)
            If File.Exists(sWorkFilename) = False Then
                '�R�s�[���s
                Message = "�R�s�[���s[�t�@�C�����F" & InFileName & ":" & OutFileName & "]"
                Return 400
            End If
        End If

        '�f�[�^�ǂݍ���
        Try
            '�o��
            If readfmt.FirstRead(sWorkFilename) = 0 Then
                Message = readfmt.Message
                Return 10
            End If
            If readfmt.IsEBCDIC = False Then
                '���Ƃ�����
                If readfmt.CRLF = True Then

                    '2017/05/23 �^�X�N�j���� CHG �W���ŏC���iJIS118,119���Ή��j-------------------------- START
                    If mRenkeiMode = 0 AndAlso _
                       mInfoArgument.INFOParameter.CODE_KBN <> "1" AndAlso _
                       mInfoArgument.INFOParameter.CODE_KBN <> "2" AndAlso _
                       mInfoArgument.INFOParameter.CODE_KBN <> "3" Then
                        ''2009/09/18.Sakon�@�R�[�h�敪�ύX�ɔ����C�� +++++++++++++++++
                        'If mRenkeiMode = 0 AndAlso _
                        '   mInfoArgument.INFOParameter.CODE_KBN <> "1" AndAlso _
                        '   mInfoArgument.INFOParameter.CODE_KBN <> "2" Then
                        '    'mInfoArgument.INFOParameter.CODE_KBN <> "3" Then
                        '    'If mRenkeiMode = 0 AndAlso _
                        '    '    mInfoArgument.INFOParameter.CODE_KBN <> "2" Then
                        '    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        '2017/05/23 �^�X�N�j���� CHG �W���ŏC���iJIS118,119���Ή��j-------------------------- END

                        Return 200        '�R�[�h�敪�ُ�iJIS���s����j
                    End If

                    ' ���s����菜��
                    Dim fmtWrite As New CAstFormat.CFormat(OutFileName, readfmt.RecordLen)
                    fmtWrite.OpenWriteFile(OutFileName)
                    Do Until readfmt.EOF()
                        ' �P�s�f�[�^��ǂݍ���
                        readfmt.GetFileData()
                        ' ��������
                        fmtWrite.WriteData(readfmt.RecordData)
                    Loop
                    fmtWrite.Close()
                    fmtWrite.Dispose()
                    fmtWrite = Nothing
                    readfmt.Close()
                Else

                    '2009/09/18.Sakon�@�R�[�h�敪�ύX�ɔ����C�� ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    If mRenkeiMode = 0 AndAlso _
                        mInfoArgument.INFOParameter.CODE_KBN <> "0" AndAlso _
                        mInfoArgument.INFOParameter.CODE_KBN <> "3" AndAlso _
                        mInfoArgument.INFOParameter.CODE_KBN <> "4" Then
                        'If mRenkeiMode = 0 AndAlso _
                        '    mInfoArgument.INFOParameter.CODE_KBN <> "0" AndAlso mInfoArgument.INFOParameter.CODE_KBN <> "3" Then
                        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                        Return 300        '�R�[�h�敪�ُ�iJIS���s�Ȃ��j
                    End If

                    File.Copy(sWorkFilename, OutFileName, True)
                End If
                'Else
                '    ' ���ʍX�V
                '    File.Copy(sWorkFilename, OutFileName, True)
                'End If
            Else
                ' �v���O�����ŕϊ�����ꍇ�́C������
                'File.Copy(sWorkFilename, OutFileName, True)

                ' FTRAN���g�p���ĕϊ�����ꍇ�͂�����
                If ConvertFtranPlus(sWorkFilename, OutFileName, readfmt.FTRANP) <> 0 Then
                    Return 100
                End If
            End If
            readfmt.Close()

            If File.Exists(sWorkFilename) = True Then
                File.Delete(sWorkFilename)
            End If

            If readfmt.Message.Equals("") = False Then
                Message = readfmt.Message
            End If
        Catch ex As Exception
            Message = ex.Message
            Return -1
        Finally
            If Not readfmt Is Nothing Then
                readfmt.Close()
                readfmt.Dispose()
            End If

            If File.Exists(sWorkFilename) = True Then
                File.Delete(sWorkFilename)
            End If
        End Try

    End Function
    '2009/11/30 �t�@�C���̕����R�[�h�`�F�b�N
    Public Function CheckCode(ByVal FileName As String, ByVal CodeKbn As String, ByRef ErrMessage As String) As Integer

        Dim Breader As New System.IO.FileStream(FileName, IO.FileMode.Open, IO.FileAccess.Read)
        Dim Bin As Integer
        Dim XBin As String = Nothing
        Dim I As Integer

        '1byte�Ǎ�
        Try
            For I = 0 To 0
                Bin = Breader.ReadByte()
                XBin = Bin.ToString("x")
            Next

            Select Case XBin
                Case "31" 'Shift-jis
                    Select Case CodeKbn
                        Case "0", "1", "2", "3"
                        Case Else
                            Return 10
                    End Select
                Case "f1" 'EBCDIC
                    Select Case CodeKbn
                        Case "4"
                        Case Else
                            Return 20
                    End Select
                Case Else
                    ErrMessage = "�����R�[�h�ُ�"
                    Return -1
            End Select
            Return 0
        Catch ex As Exception
            ErrMessage = ex.Message
            Return -1
        Finally
            Breader.Close()
        End Try

    End Function

    '======================================

    Public Function CopyToWork() As String
        If File.Exists(InFileName) = False Then
            Message = "�t�@�C�������݂��܂���[�t�@�C�����F" & InFileName & "]"
            Return InFileName
        End If

        'Dim sWorkFilename As String = Path.Combine(GetFolderName(GETFOLDER), Path.GetFileName(InFileName))
        'If Directory.Exists(GetFolderName(GETFOLDER)) = False Then
        '    Directory.CreateDirectory(GetFolderName(GETFOLDER))
        'End If
        'File.Copy(InFileName, sWorkFilename, True)

        Dim sWorkFilename As String = CopyFileToWork(InFileName, Path.Combine(GetFolderName(GETFOLDER), ""), mInfoArgument.INFOParameter.JOBTUUBAN)
        If File.Exists(sWorkFilename) = False Then
            ' �R�s�[���s
            Message = "�R�s�[���s[�t�@�C�����F" & InFileName & ":" & sWorkFilename & "]"
            Return InFileName
        End If

        Return sWorkFilename
    End Function

    '
    ' �@�\�@ �F �� ���̓t�@�C�����擾
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Private Function GetKobetuFileName(ByVal pathIni As String) As String
        Dim nRet As Integer = 0
        ' ���̓p�X
        Dim sFilename As String = CASTCommon.GetFSKJIni("COMMON", pathIni)
        'If Not mInfoArgument.INFOToriMast.FILE_NAME_T Is Nothing AndAlso mInfoArgument.INFOToriMast.FILE_NAME_T.Trim <> "" Then
        '    ' ���̓t�@�C����
        '    sFilename &= mInfoArgument.INFOToriMast.FILE_NAME_T
        'Else

        sFilename &= String.Format("F{0}.DAT", mInfoArgument.INFOParameter.TORI_CODE)

        '���2009.10.23
        'Select Case mInfoArgument.INFOParameter.FMT_KBN
        '    Case "00", "20", "21"
        '        ' �S��C�r�r�r
        '        sFilename &= String.Format("KD{0}.120", mInfoArgument.INFOParameter.TORI_CODE)
        '    Case "06"
        '        ' �n���́i�R�O�O�j
        '        sFilename &= String.Format("KD{0}.300", mInfoArgument.INFOParameter.TORI_CODE)
        '    Case "01"
        '        ' �n���́i�R�T�O�j
        '        sFilename &= String.Format("KD{0}.350", mInfoArgument.INFOParameter.TORI_CODE)
        '    Case Else
        '        ' 
        '        sFilename &= String.Format("KD{0}.dat", mInfoArgument.INFOParameter.TORI_CODE)
        'End Select

        'If Not mInfoArgument.INFOToriMast.FILE_NAME_T Is Nothing AndAlso mInfoArgument.INFOToriMast.FILE_NAME_T.Equals("") = False Then
        '    If File.Exists(mInfoArgument.INFOToriMast.FILE_NAME_T) = False AndAlso _
        '        (mInfoArgument.INFOToriMast.MULTI_KBN_T = "1" Or mInfoArgument.INFOToriMast.MULTI_KBN_T = "3") Then
        '        ' ���̓t�@�C�����Ȃ� ���� �����w�b�_�̏ꍇ
        '        '���R�[�h01�̃t�@�C������strIN_FILE_NAME�ɐݒ肷��(�y�ё��s�ȊO�̃}���`�ł��邱��)
        '        Dim aLength As Integer = mInfoArgument.INFOToriMast.FILE_NAME_T.Length
        '        sFilename = sFilename.Substring(0, aLength - 6) & "01" & sFilename.Substring(aLength - 4, 4)
        '    End If
        'End If

        'End If

        Return sFilename
    End Function

    '
    ' �@�\�@ �F ���̓t�@�C�����o�̓t�@�C���̃t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function CopyFileToWork(ByVal inname As String, ByVal outname As String, ByVal tuuban As Long) As String
        Dim sDestFileName As String = ""

        For nCounter As Integer = 1 To 100
            sDestFileName = Path.GetDirectoryName(outname) & "\"
            sDestFileName &= CASTCommon.Calendar.Now.ToString("yyMMdd")
            If tuuban <> 0 Then
                sDestFileName &= "."
                sDestFileName &= tuuban.ToString("000")
            End If
            sDestFileName &= "."
            'sDestFileName &= CASTCommon.Calendar.Now.ToString("HHmmssfffffff")
            sDestFileName &= CASTCommon.Calendar.Now.ToString("HHmmssff")
            sDestFileName &= "_"
            sDestFileName &= Path.GetFileName(inname)
            Try
                ' �R�s�[
                File.Copy(inname, sDestFileName, False)
                Exit For
            Catch ex As UnauthorizedAccessException
                Return ""                       '�o�̓t�@�C���쐬���s
            Catch ex As ArgumentException
                Return ""                       '�o�̓t�@�C���쐬���s
            Catch ex As PathTooLongException
                Return ""                       '�o�̓t�@�C���쐬���s
            Catch ex As FileNotFoundException
                Return ""                       '�o�̓t�@�C���쐬���s
            Catch ex As DirectoryNotFoundException
                Directory.CreateDirectory(Path.GetDirectoryName(outname))
            Catch ex As IOException
                ' �t�@�C�����R�s�[�ł���܂ŌJ��Ԃ�
                Threading.Thread.Sleep(128)
            Catch ex As Exception
                Return ""                       '�o�̓t�@�C���쐬���s
            End Try
        Next nCounter

        If File.Exists(sDestFileName) = False Then
            Return ""
        End If

        Return sDestFileName
    End Function

    '
    ' �@�\�@ �F ���̓t�@�C�����o�̓t�@�C���̃t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function FileAnalyze() As String()
        Dim HoldPath As String

        ' �A�g�敪,�U�֏����敪�C�t�H�[�}�b�g�敪����ۑ��t�H���_�����擾
        HoldPath = GetFolderName(HOLDFOLDER)

        '�}�̓ǂݍ���
        '�t�H�[�}�b�g�@
        Dim oReadFMT As CAstFormat.CFormat = Nothing
        Try
            ' �t�H�[�}�b�g�敪����C�t�H�[�}�b�g����肷��
            oReadFMT = CAstFormat.CFormat.GetFormat(mInfoArgument.INFOParameter)
            If oReadFMT Is Nothing Then
                ' �A�g���s
                Return New String() {}
            End If
        Catch ex As Exception
            Message = ex.Message
        End Try

        ' �t�@�C�����ꗗ
        Dim retFiles As New ArrayList(100)

        Dim LineCount As Integer = 0

        ' �t�@�C�����e�ǂݍ���
        Dim fmtWrite As CAstFormat.CFormat = Nothing
        If oReadFMT.FirstRead(InFileName) = 1 Then
            ' ����
            Do Until oReadFMT.EOF
                oReadFMT.GetFileData()
                LineCount += 1
                If oReadFMT.IsHeaderRecord = True AndAlso fmtWrite Is Nothing Then
                    Call oReadFMT.CheckRecord1()
                    Dim FormatKbn As String = GetFormatKbn(oReadFMT.InfoMeisaiMast.SYUBETU_CODE, oReadFMT.InfoMeisaiMast.ITAKU_CODE)

                    ' �w�b�_���R�[�h �� �C �t�@�C�����쐬
                    OutFileName = Path.Combine(Path.Combine(Directory.GetParent(HoldPath).ToString, FormatKbn), _
                                CASTCommon.Calendar.Now.ToString("yyMMdd.") & _
                                mInfoArgument.INFOParameter.JOBTUUBAN.ToString("000") & _
                                "_" & _
                                Path.GetFileName(InFileName) & _
                                "_" & _
                                (LineCount).ToString("00000") & ".tmp")
                    If Directory.Exists(Path.GetDirectoryName(OutFileName)) = False Then
                        Directory.CreateDirectory(Path.GetDirectoryName(OutFileName))
                    End If
                    fmtWrite = New CAstFormat.CFormat(OutFileName, oReadFMT.RecordLen)
                    If fmtWrite.OpenWriteFile(OutFileName) = 0 Then
                        Return New String() {}
                    End If
                    retFiles.Add(OutFileName)
                End If

                ' ��������
                fmtWrite.WriteData(oReadFMT.RecordData)
                If oReadFMT.CRLF = True Then
                    fmtWrite.WriteCrLf()
                End If

                If Not fmtWrite Is Nothing AndAlso (oReadFMT.IsEndRecord = True OrElse oReadFMT.EOF = True) Then
                    ' �G���h���R�[�h�ŁC �t�@�C�������
                    fmtWrite.Close()
                    fmtWrite.Dispose()
                    fmtWrite = Nothing
                End If
            Loop
        Else
            ' �ُ�
            OutFileName = Path.Combine(Path.Combine(Directory.GetParent(HoldPath).ToString, mInfoArgument.INFOParameter.FMT_KBN), _
                        CASTCommon.Calendar.Now.ToString("yyMMdd.") & _
                        mInfoArgument.INFOParameter.JOBTUUBAN.ToString("000") & _
                        "_" & _
                        Path.GetFileName(InFileName) & _
                        "_" & _
                        (LineCount).ToString("00000") & ".tmp")
            If Directory.Exists(Path.GetDirectoryName(OutFileName)) = False Then
                Directory.CreateDirectory(Path.GetDirectoryName(OutFileName))
            End If
            File.Copy(InFileName, OutFileName, True)
            retFiles.Add(OutFileName)
        End If

        oReadFMT.Close()
        oReadFMT = Nothing

        For i As Integer = 0 To retFiles.Count - 1
            File.Delete(Path.ChangeExtension(retFiles.Item(i).ToString, ".DAT"))
            File.Move(retFiles.Item(i).ToString, Path.ChangeExtension(retFiles.Item(i).ToString, ".DAT"))
            retFiles.Item(i) = Path.ChangeExtension(retFiles.Item(i).ToString, ".DAT")
        Next i

        Return CType(retFiles.ToArray(GetType(String)), String())
    End Function

    ' �@�\�@ �F �����}�X�^�̐U�֏����敪���擾
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F �U���Q�����}�X�^���Q�Ƃ��āC���݂���ꍇ�� "2" ��Ԃ�
    '           ��L�ȊO�́C"1" ��Ԃ�
    '
    Private Function GetFormatKbn(ByVal SyubetuCode As String, ByVal itakucode As String) As String
        Dim SQL As New System.Text.StringBuilder(128)
        Dim OraDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Try
            SQL.Append("SELECT FMT_KBN_T FROM TORIMAST")
            SQL.Append(" WHERE ITAKU_CODE_T = " & CASTCommon.SQ(itakucode))
            '2008/04/17 ��ʃR�[�h�ǉ�
            SQL.Append("   AND SYUBETU_T =" & CASTCommon.SQ(SyubetuCode))
            If OraReader.DataReader(SQL) = True Then
                Return OraReader.GetValue(0)
            Else
                If OraReader.DataReader(SQL.Replace("TORIMAST", "S_TORIMAST")) = True Then
                    Return OraReader.GetValue(0)
                Else
                    Return "00"
                End If
            End If
        Catch ex As Exception
            Message = "�t�H�[�}�b�g�敪��������܂���"
            Return "00"
        Finally
            OraReader.Close()
            OraReader = Nothing

            If Not OraDB Is Nothing Then
                OraDB.Close()
            End If
        End Try
    End Function

    '
    ' �@�\�@ �F ���̓t�@�C�����o�̓t�@�C���̃t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - ���̓t�@�C�����iCxxxxx.a.bbbbbbb.cc.dat�j a:�U�֏����敪�Cb:������R�[�h�Cc:����敛�R�[�h
    '           ARG2 - �o�̓t�@�C����
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function FileDecodeMove() As String
        Dim GetPath As String

        ' �A�g�敪,�U�֏����敪�C�t�H�[�}�b�g�敪����ۑ��t�H���_�����擾
        GetPath = GetFolderName(GETFOLDER)

        Dim Key() As String = InFileName.Split("."c)
        If Key.Length < 4 Then
            Message = "�s���ȃt�@�C�����ł�"
            Return ""
        End If
        Dim Complock As stCOMPLOCK

        Dim OraDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQL As New System.Text.StringBuilder(128)
        Try
            SQL.Append("SELECT ")
            SQL.Append(" ENC_KEY1_T")
            SQL.Append(",ENC_KEY2_T")
            SQL.Append(",ENC_OPT1_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = " & CASTCommon.SQ(Key(1)))
            SQL.Append("   AND TORIS_CODE_T = " & CASTCommon.SQ(Key(2)))
            SQL.Append("   AND TORIF_CODE_T = " & CASTCommon.SQ(Key(3)))
            If OraReader.DataReader(SQL) = False Then

                ' ���U�̎����}�X�^�������ꍇ�́C�U���̎����}�X�^������
                SQL = New System.Text.StringBuilder(128)
                SQL.Append("SELECT ")
                SQL.Append(" ENC_KEY1_T")
                SQL.Append(",ENC_KEY2_T")
                SQL.Append(",ENC_OPT1_T")
                SQL.Append(" FROM S_TORIMAST")
                SQL.Append(" WHERE FSYORI_KBN_T = '3'")
                SQL.Append("   AND TORIS_CODE_T = " & CASTCommon.SQ(Key(2)))
                SQL.Append("   AND TORIF_CODE_T = " & CASTCommon.SQ(Key(3)))
                OraReader.DataReader(SQL)
            End If
            If OraReader.EOF = True Then
                OraReader.Close()
                Message = "�����}�X�^���擾�ł��܂���"
                Return ""
            End If

            Complock.KEY = OraReader.GetString("ENC_KEY1_T").PadRight(64, " "c)
            Complock.IVKEY = OraReader.GetString("ENC_KEY2_T").PadRight(32, " "c)
            Complock.AES = OraReader.GetString("ENC_OPT1_T")
            Select Case Complock.AES
                Case "0"
                    ' �`�d�r�Ȃ�
                    Complock.KEY = Complock.KEY.Substring(0, 16)
                    Complock.IVKEY = Complock.IVKEY.Substring(0, 16)
                Case "1"
                    ' �`�d�r�����P�Q�W�r�b�g
                    Complock.KEY = Complock.KEY.Substring(0, 32)
                    Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
                Case "2"
                    ' �`�d�r�����P�X�Q�r�b�g
                    Complock.KEY = Complock.KEY.Substring(0, 48)
                    Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
                Case "3"
                    ' �`�d�r�����Q�T�U�r�b�g
                    Complock.KEY = Complock.KEY.Substring(0, 64)
                    Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
            End Select
            Complock.RECLEN = GetFormatLength().ToString

            OraReader.Close()
            OraDB.Close()

            OraReader = Nothing
            OraDB = Nothing

            OutFileName = Path.Combine(GetPath, _
                        CASTCommon.Calendar.Now.ToString("yyMMdd.") & _
                        mInfoArgument.INFOParameter.JOBTUUBAN.ToString("000") & _
                        "_" & _
                        Path.GetFileName(InFileName) & _
                        ".tmp")
            If Directory.Exists(Path.GetDirectoryName(OutFileName)) = False Then
                Directory.CreateDirectory(Path.GetDirectoryName(OutFileName))
            End If
            File.Delete(OutFileName)

            Dim nRet As Long = DecodeComplock(Complock, OutFileName)
            If nRet = -199 Then
                Return ""
            ElseIf nRet <> 0 Then
                Message = "COMPLOCK II �����G���[[" & nRet.ToString & "]"
                Return ""
            End If

            File.Delete(Path.ChangeExtension(OutFileName, ".DAT"))
            File.Move(OutFileName, Path.ChangeExtension(OutFileName, ".DAT"))
            OutFileName = Path.ChangeExtension(OutFileName, ".DAT")

            File.Delete(InFileName)
        Catch ex As Exception
            Message = ex.Message
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            If Not OraDB Is Nothing Then
                OraDB.Close()
            End If
        End Try

        Return OutFileName
    End Function

    '
    ' �@�\�@ �F ���̓t�@�C�����o�̓t�@�C���̃t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function FileDecodeMove(ByVal recLen As String, ByVal aes As String, ByVal encodeKey As String, ByVal ivKey As String) As String
        Dim GetPath As String

        ' �A�g�敪,�U�֏����敪�C�t�H�[�}�b�g�敪����ۑ��t�H���_�����擾
        GetPath = GetFolderName(GETFOLDER)

        Dim Complock As stCOMPLOCK
        Complock.KEY = encodeKey.PadRight(32)
        Complock.IVKEY = ivKey.PadRight(64)
        Complock.AES = aes
        Select Case Complock.AES
            Case "0"
                ' �`�d�r�Ȃ�
                Complock.KEY = Complock.KEY.Substring(0, 16)
                Complock.IVKEY = Complock.IVKEY.Substring(0, 16)
            Case "1"
                ' �`�d�r�����P�Q�W�r�b�g
                Complock.KEY = Complock.KEY.Substring(0, 32)
                Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
            Case "2"
                ' �`�d�r�����P�X�Q�r�b�g
                Complock.KEY = Complock.KEY.Substring(0, 48)
                Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
            Case "3"
                ' �`�d�r�����Q�T�U�r�b�g
                Complock.KEY = Complock.KEY.Substring(0, 64)
                Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
        End Select

        Complock.RECLEN = recLen

        OutFileName = Path.Combine(GetPath, Path.GetFileName(InFileName) & ".tmp")
        If Directory.Exists(Path.GetDirectoryName(OutFileName)) = False Then
            Directory.CreateDirectory(Path.GetDirectoryName(OutFileName))
        End If
        File.Delete(OutFileName)

        Dim nRet As Long = DecodeComplock(Complock, OutFileName)
        If nRet <> 0 Then
            Message = "COMPLOCK II �����G���[[" & nRet.ToString & "]"
            Return ""
        End If

        File.Delete(Path.ChangeExtension(OutFileName, ".DAT"))
        File.Move(OutFileName, Path.ChangeExtension(OutFileName, ".DAT"))
        OutFileName = Path.ChangeExtension(OutFileName, ".DAT")

        File.Delete(InFileName)

        Return OutFileName
    End Function

    '
    ' �@�\�@ �F ���̓t�@�C�����o�̓t�@�C���̃t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function FileMove() As String
        Return FileCopy(False)
    End Function

    '
    ' �@�\�@ �F ���̓t�@�C�����o�̓t�@�C���̃t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function FileCopy(Optional ByVal copyMode As Boolean = True) As String
        Dim GetPath As String

        ' �A�g�敪,�U�֏����敪�C�t�H�[�}�b�g�敪����ۑ��t�H���_�����擾
        GetPath = GetFolderName(GETFOLDER)

        ' �b�l�s�t�H���_����C�ꊇ�����p�t�H���_�ֈړ�
        OutFileName = Path.Combine(GetPath, _
                    CASTCommon.Calendar.Now.ToString("yyMMdd.") & _
                    mInfoArgument.INFOParameter.JOBTUUBAN.ToString("000") & _
                    "_" & _
                    Path.GetFileName(InFileName) & _
                    ".tmp")
        If Directory.Exists(Path.GetDirectoryName(OutFileName)) = False Then
            Directory.CreateDirectory(Path.GetDirectoryName(OutFileName))
        End If
        File.Delete(OutFileName)
        If copyMode = True Then
            File.Copy(InFileName, OutFileName)
        Else
            File.Move(InFileName, OutFileName)
        End If

        File.Delete(Path.ChangeExtension(OutFileName, ".DAT"))
        File.Move(OutFileName, Path.ChangeExtension(OutFileName, ".DAT"))
        OutFileName = Path.ChangeExtension(OutFileName, ".DAT")

        Return OutFileName
    End Function

    '
    ' �@�\�@ �F �ėp�t�H���_���̑Ώۃt�@�C���ꗗ���擾����
    '
    ' ����   �F ARG1 - �Ώۃt�@�C���ꗗ
    '
    ' �߂�l �F �Ώۃt�@�C���ꗗ
    '
    ' ���l�@ �F 
    '
    Public Function GetHanyouFiles(ByVal files() As String, ByVal ptn As String) As String()
        Dim HoldPath As String
        Dim ArrayFiles As New ArrayList(files)

        ' �A�g�敪,�U�֏����敪�C�t�H�[�}�b�g�敪����ۑ��t�H���_�����擾
        HoldPath = GetFolderName(HOLDFOLDER)

        ArrayFiles.AddRange(Directory.GetFiles(HoldPath & "\", ptn))

        Return CType(ArrayFiles.ToArray(GetType(String)), String())
    End Function

    '
    ' �@�\�@ �F �ėp�t�H���_���̑Ώۃt�@�C���ꗗ���擾����
    '
    ' ����   �F ARG1 - �Ώۃt�@�C���ꗗ
    '
    ' �߂�l �F �Ώۃt�@�C���ꗗ
    '
    ' ���l�@ �F 
    '
    Public Function GetHanyouOthreFiles(ByVal files() As String, ByVal ptn As String) As String()
        Dim HoldPath As String
        Dim ArrayFiles As New ArrayList(files)

        ' �A�g�敪,�U�֏����敪�C�t�H�[�}�b�g�敪����ۑ��t�H���_�����擾
        If mInfoArgument.INFOParameter.FSYORI_KBN = "1" Then
            HoldPath = GetFolderName(HOLDFOLDER, "3")

            If Directory.Exists(HoldPath) = False Then
                Directory.CreateDirectory(HoldPath)
            End If

            ArrayFiles.AddRange(Directory.GetFiles(HoldPath & "\", ptn.Replace(mInfoArgument.INFOParameter.FSYORI_KBN, "3")))
        Else
            HoldPath = GetFolderName(HOLDFOLDER, "1")

            If Directory.Exists(HoldPath) = False Then
                Directory.CreateDirectory(HoldPath)
            End If

            ArrayFiles.AddRange(Directory.GetFiles(HoldPath & "\", ptn.Replace(mInfoArgument.INFOParameter.FSYORI_KBN, "1")))
        End If

        Return CType(ArrayFiles.ToArray(GetType(String)), String())
    End Function

    '
    ' �@�\�@ �F FTRAN PLUS
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '           ARG3 - �ϊ���`�t�@�C��
    '
    ' �߂�l �F 
    '
    ' ���l�@ �F 
    '
    Private Function ConvertFtranPlus(ByVal infile As String, ByVal outfile As String, ByVal teigi As String) As Integer
        Dim nRet As Integer

        ' EBCDIC �ϊ�
        Dim strCMD As String
        Dim strTEIGI_FIEL As String

        Dim sFtranPPath As String = CASTCommon.GetFSKJIni("COMMON", "FTR")

        strTEIGI_FIEL = sFtranPPath & teigi

        '*** �C�� mitsu 2008/09/30 EBCDIC�f�[�^�Ή��̂��ߑ啝�C�� ***
        'strCMD = "FP /nwd/ cload " & sFtranPPath & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & infile & " " & outfile & " ++" & strTEIGI_FIEL
        '�����敪�����U���A�A�g�敪���������݂ł���ꍇ
        '(�Z���^�[���ڎ����t�H�[�}�b�g�͐�p��P�t�@�C�������邽�߂�����ŕϊ�����(EBCDIC�f�[�^�͎g��Ȃ�))
        If mInfoArgument.INFOParameter.FSYORI_KBN = "1" AndAlso mRenkeiMode = 0 AndAlso Not mInfoArgument.INFOParameter.FMT_KBN = "TO" Then
            '�t�H�[�}�b�g�敪���烌�R�[�h���擾
            Dim RecLen As Integer = GetFormatLength()

            '���R�[�h�O����JIS�A�㔼��EBCDIC�̔{�Œ蒷�f�[�^�ɕϊ�����
            strCMD = String.Format("/nwd/ cload {0}FUSION ; kanji 83_jis getrand {1} {2} /isize {3}" & _
                " /size $*2 /map @0 ank {3}:{3} , @0 binary {3}:{3}", sFtranPPath, infile, outfile, RecLen)
        Else
            strCMD = "/nwd/ cload " & sFtranPPath & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & infile & " " & outfile & " ++" & strTEIGI_FIEL
        End If
        '************************************************************

        Dim ProcFT As Process
        Dim ProcInfo As New ProcessStartInfo
        ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "FTRANP"), "FP.EXE")
        ProcInfo.Arguments = strCMD
        ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        ProcFT = Process.Start(ProcInfo)
        ProcFT.WaitForExit()
        nRet = ProcFT.ExitCode

        If nRet <> 0 Then
            ' �R�[�h�ϊ����s
            Return 100
        End If
        'End If

        Return 0
    End Function

    ' �@�\�@ �F FTRAN PLUS
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '           ARG3 - �ϊ���`�t�@�C��
    '
    ' �߂�l �F 
    '
    ' ���l�@ �F 
    '
    Public Function ConvertToEBCDIC(ByVal infile As String, ByVal outfile As String, ByVal teigi As String) As Integer
        Dim nRet As Integer

        ' EBCDIC �ϊ�
        Dim strCMD As String
        Dim strTEIGI_FIEL As String

        Dim sFtranPPath As String = CASTCommon.GetFSKJIni("COMMON", "FTR")

        strTEIGI_FIEL = sFtranPPath & teigi

        '*** �C�� mitsu 2008/09/30 FP�s�v ***
        'strCMD = "FP /nwd/ cload " & sFtranPPath & "FUSION ; ebcdic ; kanji 83_jis putrand """ & infile & """ """ & outfile & """ ++" & strTEIGI_FIEL
        strCMD = "/nwd/ cload " & sFtranPPath & "FUSION ; ebcdic ; kanji 83_jis putrand """ & infile & """ """ & outfile & """ ++" & strTEIGI_FIEL
        '************************************

        Dim ProcFT As Process
        Dim ProcInfo As New ProcessStartInfo
        ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "FTRANP"), "FP.EXE")
        ProcInfo.Arguments = strCMD
        ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        ProcFT = Process.Start(ProcInfo)
        ProcFT.WaitForExit()
        nRet = ProcFT.ExitCode

        If nRet <> 0 Then
            ' �R�[�h�ϊ����s
            Return 100
        End If

        Return 0
    End Function

    ' �@�\�@ �F FTRAN PLUS
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �o�̓t�@�C����
    '           ARG3 - �ϊ���`�t�@�C��
    '
    ' �߂�l �F 
    '
    ' ���l�@ �F 
    '
    Public Function ConvertToSJIS(ByVal infile As String, ByVal outfile As String, ByVal teigi As String) As Integer
        Dim nRet As Integer
        Dim strCMD As String
        Dim strTEIGI_FIEL As String

        Dim sFtranPPath As String = CASTCommon.GetFSKJIni("COMMON", "FTR")

        strTEIGI_FIEL = sFtranPPath & teigi

        strCMD = "/nwd/ cload " & sFtranPPath & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & infile & " " & outfile & " ++" & strTEIGI_FIEL

        Dim ProcFT As Process
        Dim ProcInfo As New ProcessStartInfo
        ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "FTRANP"), "FP.EXE")
        ProcInfo.Arguments = strCMD
        ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        ProcFT = Process.Start(ProcInfo)
        ProcFT.WaitForExit()
        nRet = ProcFT.ExitCode

        If nRet <> 0 Then
            ' �R�[�h�ϊ����s
            Return 100
        End If

        Return 0
    End Function

    '
    ' �@�\�@ �F �f�[�^�擾�p�t�H���_���擾����
    '
    ' ����   �F ARG1 - �t�H���_��
    '
    ' �߂�l �F �t�H���_��
    '
    ' ���l�@ �F 
    '
    Private Function GetFolderName(ByVal foldername As String, ByVal fSyoriKbn As String) As String
        Dim sPath As String

        If mInfoArgument.INFOParameter.FMT_KBN = "SC" Then
            sPath = Path.Combine(SSCPATH, foldername)

            Try
                If Directory.Exists(sPath) = False Then
                    Directory.CreateDirectory(sPath)
                End If
            Catch ex As Exception
                Message = ex.Message
            End Try

            Return sPath
        End If

        '2009/09/08.�b��Ή��@�A�g�敪�Ƃ�̂���(�ʂ̂�) +++++++
        '' �A�g�敪
        'Select Case mInfoArgument.INFOParameter.RENKEI_KBN
        '    Case "01"       ' ���f�B�A�R���o�[�^
        '        sPath = Path.Combine(MEDIAPATH, foldername)
        '    Case "09"       ' �ėp�G���g��
        '        sPath = Path.Combine(HENTRYPATH, foldername)
        '    Case "02"       ' �b�l�s
        '        sPath = Path.Combine(CMTPATH, foldername)
        '    Case "07"       ' �w�Z
        '        sPath = Path.Combine(SCHOOLPATH, foldername)
        '    Case "00"       ' �`��
        '        sPath = Path.Combine(DENSOUPATH, foldername)
        '    Case "KB"       ' ���o�b�`
        '        sPath = Path.Combine(KBPATH, foldername)
        '    Case "SC"       ' �r�r�b
        '        sPath = Path.Combine(SSCPATH, foldername)
        '    Case Else       ' ��
        sPath = Path.Combine(KOBETUPATH, foldername)
        'End Select
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        ' �U�֏����敪
        Select Case fSyoriKbn
            Case "1"            ' ���U
                sPath &= "\" & JIFFOLDER
            Case "3"            ' �U��
                sPath &= "\" & SOKFOLDER
            Case "2"            ' SSS
                sPath &= "\" & SSSFOLDER
            Case Else
                sPath &= "\" & JIFFOLDER
        End Select

        ' �t�H�[�}�b�g�敪
        sPath &= "\" & mInfoArgument.INFOParameter.FMT_KBN

        Try
            If Directory.Exists(sPath) = False Then
                Directory.CreateDirectory(sPath)
            End If
        Catch ex As Exception
            Message = ex.Message
        End Try

        Return sPath
    End Function

    '
    ' �@�\�@ �F �f�[�^�擾�p�t�H���_���擾����
    '
    ' ����   �F ARG1 - �t�H���_��
    '
    ' �߂�l �F �t�H���_��
    '
    ' ���l�@ �F 
    '
    Private Function GetFolderName(ByVal foldername As String) As String
        Return GetFolderName(foldername, mInfoArgument.INFOParameter.FSYORI_KBN)
    End Function

    ' �@�\�@ �F �t�@�C���𐳏�t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function CopyToNormal(ByVal filename As String) As String
        Return MoveFile(filename, NORFOLDER, True)
    End Function

    ' �@�\�@ �F �t�@�C�����ُ�t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function CopyToError(ByVal filename As String, ByVal destFileName As String) As String
        'If destFileName = "" Then
        '    If File.Exists(filename) = True Then
        '        File.Copy(filename, Path.Combine(Path.GetDirectoryName(InFileName), destFileName))
        '    End If
        '    Call MoveFile(Path.Combine(Path.GetDirectoryName(InFileName), destFileName), ERRFOLDER, False)

        '    Return destFileName
        'Else
        Return CopyToError(filename)
        'End If
    End Function

    ' �@�\�@ �F �t�@�C�����ُ�t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function CopyToError(ByVal filename As String) As String
        Return MoveFile(filename, ERRFOLDER, True)
    End Function

    ' �@�\�@ �F �t�@�C���𐳏�t�H���_�ֈړ�����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function MoveToNormal(ByVal filename As String) As String
        Return MoveFile(filename, NORFOLDER)
    End Function

    ' �@�\�@ �F �t�@�C�����ُ�t�H���_�ֈړ�����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function MoveToError(ByVal filename As String) As String
        Return MoveFile(filename, ERRFOLDER)
    End Function

    ' �@�\�@ �F �t�@�C�����ُ�t�H���_�փR�s�[����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function MoveToError(ByVal filename As String, ByVal destFileName As String) As String
        If destFileName = "" Then
            If File.Exists(filename) = True Then
                File.Copy(filename, Path.Combine(Path.GetDirectoryName(InFileName), destFileName))
            End If
            Call MoveFile(Path.Combine(Path.GetDirectoryName(InFileName), destFileName), ERRFOLDER, False)

            Return destFileName
        Else
            Return MoveToError(filename)
        End If
    End Function

    ' �@�\�@ �F �t�@�C���𐳏�t�H���_�ֈړ�����
    '
    ' ����   �F ARG1 - �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function MoveToGet(ByVal filename As String) As String
        Return MoveFile(filename, GETFOLDER)
    End Function

    ' �@�\�@ �F �t�@�C�����w�肵���t�H���_�ֈړ�����
    '
    ' ����   �F ARG1 - �t�@�C����
    '           ARG2 - �t�H���_��
    '
    ' ���l�@ �F 
    '
    Private Function MoveFile(ByVal filename As String, ByVal tofolder As String, Optional ByVal copy As Boolean = False) As String
        Dim DestFile As String

        If Directory.Exists(GetFolderName(tofolder)) = False Then
            Call Directory.CreateDirectory(GetFolderName(tofolder))
        End If
        DestFile = Path.Combine(GetFolderName(tofolder), Path.GetFileName(filename))
        If filename.Trim = DestFile.TrimEnd Then
            Return DestFile
        End If
        If File.Exists(DestFile) = True Then
            File.Delete(DestFile)
            'File.Move(DestFile, Path.ChangeExtension(DestFile, String.Format(".{0:HHmmss}.DAT", Date.Now)))
        End If
        If copy = True Then
            '�@�t�@�C���R�s�[
            File.Copy(filename, DestFile, True)
        Else
            ' �t�@�C���ړ��u
            File.Move(filename, DestFile)
        End If

        Return DestFile
    End Function

    ' �@�\�@ �F �t�@�C���̊g���q��ύX����
    '
    ' ����   �F ARG1 - �t�@�C����
    '           ARG2 - �ύX��g���q
    '
    ' ���l�@ �F 
    '
    Public Function ChangeExtension(ByVal filename As String, ByVal ext As String) As String
        Dim DestFile As String = Path.ChangeExtension(filename, ext)
        If File.Exists(DestFile) = True Then
            File.Move(DestFile, Path.ChangeExtension(DestFile, String.Format(".{0:HHmmss}.", Date.Now) & Path.GetExtension(DestFile)))
        End If
        Call File.Move(filename, DestFile)

        Return DestFile
    End Function

    ' �@�\�@ �F �擾�t�@�C���̈ړ�
    '
    ' ���l�@ �F 
    '
    Public Sub RemoveInFile()
        Dim Prefix As String = String.Format("{0:yyMMdd}.{1:000}_", CASTCommon.Calendar.Now, mInfoArgument.INFOParameter.JOBTUUBAN)
        Dim DestFile As String = Path.Combine(Path.GetDirectoryName(InFileName), Prefix & Path.GetFileName(InFileName))
        If File.Exists(DestFile) = True Then
            File.Delete(DestFile)
        End If
        File.Move(InFileName, DestFile)
        MoveToNormal(DestFile)
    End Sub

    ' �@�\�@ �F �b�l�s�t�H���_�Ώۃt�@�C�����擾
    '
    ' ���l�@ �F 
    '
    Public Function GetCMTFiles() As String()
        Return GetFiles(GETFOLDER)
    End Function

    ' �@�\�@ �F �w�Z�t�H���_�Ώۃt�@�C�����擾
    '
    ' ���l�@ �F 
    '
    Public Function GetSchoolFiles() As String()
        Return GetFiles(GETFOLDER)
    End Function

    ' �@�\�@ �F �Ώۃt�@�C�����擾
    '
    ' ���l�@ �F 
    '
    Private Function GetFiles(ByVal foldername As String, Optional ByVal searchPattern As String = "*.DAT") As String()
        Dim GetPath As String

        GetPath = GetFolderName(foldername)

        If Directory.Exists(GetPath) = False Then
            Call Directory.CreateDirectory(GetPath)
        End If

        Return Directory.GetFiles(GetPath, searchPattern)
    End Function

    ' �@�\�@ �F �b�l�s�}�̕ϊ��V�X�e���ւ̏o�̓p�X���擾
    '
    ' ���l�@ �F 
    '
    Public Function GetCMTOtherWritePath() As String
        Dim GetPath As String

        ' �b�l�s�������݃t�H���_
        GetPath = CMTOTHERWRTPATH

        ' �U�֏����敪
        Select Case mInfoArgument.INFOParameter.FSYORI_KBN
            Case "1"            ' ���U
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
            Case "3"            ' �U��
                GetPath = Path.Combine(GetPath, OTHSOKFOLDER)
            Case Else
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
        End Select

        ' �t�H�[�}�b�g�敪(CMT�A�g�̃t�H���_�͒������t�H���_���ɂȂ��Ă���j
        GetPath &= GetFormatLength.ToString.PadLeft(3, "0"c) & "\"

        If Directory.Exists(GetPath) = False Then
            Call Directory.CreateDirectory(GetPath)
        End If

        Return GetPath
    End Function

    ' �@�\�@ �F �b�l�s�}�̕ϊ��V�X�e������̑Ώۃt�@�C�����擾
    '
    ' ���l�@ �F 
    '
    Public Function GetCMTOtherFiles() As String()
        Dim GetPath As String

        GetPath = CMTOTHERPATH

        ' �U�֏����敪
        Select Case mInfoArgument.INFOParameter.FSYORI_KBN
            Case "1"            ' ���U
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
            Case "3"            ' �U��
                GetPath = Path.Combine(GetPath, OTHSOKFOLDER)
            Case "10"
                ' ���s���ʂ̏ꍇ
                GetPath = Path.Combine(GetPath, "TAKOU")
                Return Directory.GetFiles(GetPath)
            Case Else
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
        End Select

        ' �t�H�[�}�b�g�敪(CMT�A�g�̃t�H���_�͒������t�H���_���ɂȂ��Ă���j
        GetPath &= GetFormatLength.ToString.PadLeft(3, "0"c)

        If Directory.Exists(GetPath) = False Then
            Call Directory.CreateDirectory(GetPath)
        End If

        Return Directory.GetFiles(GetPath, "*.DAT")
    End Function

    ' �@�\�@ �F �w�Z���U�V�X�e������̑Ώۃt�@�C�����擾
    '
    ' ���l�@ �F 
    '
    Public Function GetSCHOOLOtherFiles() As String()
        Dim GetPath As String

        GetPath = SCHOOLOTHERPATH

        Return Directory.GetFiles(GetPath, "*FD")
    End Function

    ' �@�\�@ �F ���f�B�A�R���o�[�^�V�X�e���ւ̏o�̓p�X���擾
    '
    ' ���l�@ �F 
    '
    Public Function GetMEDIAOtherWritePath() As String
        Dim GetPath As String

        ' ���f�B�A�R���o�[�^�������݃t�H���_
        GetPath = MEDIAOTHERWRTPATH

        Return GetPath
    End Function

    ' �@�\�@ �F ���f�B�A�R���o�[�^�V�X�e������̑Ώۃt�@�C�����擾
    '
    ' ���l�@ �F 
    '
    Public Function GetMEDIAOtherFiles() As String()
        Dim GetPath As String

        GetPath = MEDIAOTHERPATH

        If Directory.Exists(GetPath) = False Then
            Dim str() As String = {}
            Return str
        End If

        ' �U�֏����敪
        Dim RetFiles() As String
        Dim FilesList As New ArrayList
        Dim WildFile As String
        Select Case mInfoArgument.INFOParameter.FSYORI_KBN
            Case "1"            ' ���U
                ' ���U�t�@�C���̒ǉ�
                WildFile = CASTCommon.GetFSKJIni("TOUROKU", "JIF_FILE") & "*"
                RetFiles = Directory.GetFiles(GetPath, WildFile)
                For i As Integer = 0 To RetFiles.Length - 1
                    If Path.GetFileName(RetFiles(i)).EndsWith("000") = False Then
                        If File.Exists(RetFiles(i)) = True Then
                            FilesList.Add(RetFiles(i))
                        End If
                    End If
                Next i

                ' �W����s�t�@�C���̒ǉ�
                WildFile = CASTCommon.GetFSKJIni("TOUROKU", "SKD_FILE") & "*"
                RetFiles = Directory.GetFiles(GetPath, WildFile)
                For i As Integer = 0 To RetFiles.Length - 1
                    If Path.GetFileName(RetFiles(i)).EndsWith("000") = False Then
                        If File.Exists(RetFiles(i)) = True Then
                            FilesList.Add(RetFiles(i))
                        End If
                    End If
                Next i
            Case "3"            ' �U��
                ' �W����s�t�@�C���̒ǉ�
                WildFile = CASTCommon.GetFSKJIni("TOUROKU", "SOK_FILE") & "*"
                RetFiles = Directory.GetFiles(GetPath, WildFile)
                For i As Integer = 0 To RetFiles.Length - 1
                    If Path.GetFileName(RetFiles(i)).EndsWith("000") = False Then
                        If File.Exists(RetFiles(i)) = True Then
                            FilesList.Add(RetFiles(i))
                        End If
                    End If
                Next i
        End Select

        Dim MediaList() As String = FilesList.ToArray(GetType(String))

        Return MediaList
    End Function

    ' �@�\�@ �F �ėp�G���g���V�X�e������̑Ώۃt�@�C�����擾
    '
    ' ���l�@ �F 
    '
    Public Function GetHanyouOtherSystemFiles() As String()
        Dim GetPath As String

        ' �ėp�G���g���f�[�^�ǂݎ��t�H���_
        GetPath = HANREADPATH

        If Directory.Exists(GetPath) = False Then
            Dim str() As String = {}
            Return str
        End If

        ' �U�֏����敪
        Dim RetFiles() As String
        Dim FilesList As New ArrayList
        Dim WildFile As String
        WildFile = CASTCommon.GetFSKJIni("TOUROKU", "H-ENTRY_FILE")
        RetFiles = Directory.GetFiles(GetPath, WildFile)
        For i As Integer = 0 To RetFiles.Length - 1
            If Path.GetFileName(RetFiles(i)) = WildFile Then
                If File.Exists(RetFiles(i)) = True Then
                    FilesList.Add(RetFiles(i))
                End If
            End If
        Next i

        Dim HanyouList() As String = FilesList.ToArray(GetType(String))

        Return HanyouList
    End Function

    ' �@�\�@ �F �b�n�l�o�k�n�b�j���g�p���ăt�@�C���𕜍���
    '
    ' ���l�@ �F 
    '
    Private Function DecodeComplock(ByVal complock As stCOMPLOCK, ByVal outfile As String) As Long
        Dim Arg As String

        Dim sComplockPath As String = CASTCommon.GetFSKJIni("COMMON", "COMPLOCK")
        If File.Exists(sComplockPath) = True Then
            Message = "COMPLOCK�v���O������������܂���"
            Return -199
        End If

        ' �����g�ݗ���
        Dim DQ As String = """"
        Arg = " -I "
        Arg &= DQ & InFileName & DQ
        Arg &= " -O " & DQ & outfile & DQ ' �o�͐�
        Arg &= " -k " & DQ & complock.KEY & DQ                      ' ��
        Arg &= " -v " & DQ & complock.IVKEY & DQ                    ' IV
        Arg &= " -lf 0"

        'File.Copy(InFileName, outfile)
        'Return 0

        Dim ProcFT As New Process
        Try
            ' ���������s
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(sComplockPath, "decode.exe")
            ProcInfo.Arguments = Arg
            ProcInfo.WorkingDirectory = sComplockPath
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            If ProcFT.ExitCode = 19 Then
                ' DECODE-019 �w�b�_�[���R�[�h���Ɍ�肪����܂��B
                ' �w�b�_�Œ蒷�ɂāC������x�����������݂�
                ProcFT.Close()
                Arg &= " -rl " & DQ & complock.RECLEN & DQ
                ProcInfo.Arguments = Arg
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
            End If
            If ProcFT.ExitCode <> 0 Then
                Dim LogPath As String = CASTCommon.GetFSKJIni("COMMON", "LOG")
                Dim fs As New StreamWriter(Path.Combine(LogPath, "COMPLOCK" & Path.GetFileName(Path.ChangeExtension(InFileName, ".LOG"))), True, EncdJ)
                fs.WriteLine(ProcInfo.FileName & " ")
                fs.WriteLine(Arg)
                fs.Write(ProcFT.StandardOutput.ReadToEnd())
                fs.Close()
                fs = Nothing
            End If
        Catch ex As Exception
            Return -1
        End Try

        Return ProcFT.ExitCode

    End Function

    ' �@�\�@ �F �b�n�l�o�k�n�b�j���g�p���ăt�@�C�����Í���
    '
    ' ���l�@ �F 
    '
    Private Function EncodeComplock(ByVal complock As stCOMPLOCK, ByVal outfile As String) As Long
        Dim Arg As String

        Dim sComplockPath As String = CASTCommon.GetFSKJIni("COMMON", "COMPLOCK")
        If File.Exists(sComplockPath) = True Then
            Message = "COMPLOCK�v���O������������܂���"
            Return -199
        End If

        ' �����g�ݗ���
        Dim DQ As String = """"
        Arg = " -I "
        Arg &= DQ & InFileName & DQ
        Arg &= " -l " & DQ & complock.RECLEN & DQ
        Arg &= " -O " & DQ & outfile & DQ ' �o�͐�
        If complock.AES = "0" Then
            ' �`�d�r�Ȃ�
            Arg &= " -a 5"
            Arg &= " -n 256"
        Else
            ' �`�d�r
            Arg &= " -a  8"             ' -rl ���w�肵�Ȃ��ꍇ��, -a 6 �ƂȂ�
            Arg &= " -m  1 "
            Arg &= " -ak 1 "
            Arg &= " -p  1"
        End If
        Arg &= " -rl " & DQ & complock.RECLEN & DQ
        Arg &= " -k " & DQ & complock.KEY & DQ                      ' ��
        Arg &= " -v " & DQ & complock.IVKEY & DQ                    ' IV
        Arg &= " -t 0"
        Arg &= " -g 1"

        ' �Í������s
        Dim ProcFT As New Process
        Try
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(sComplockPath, "encode.exe")
            ProcInfo.Arguments = Arg
            ProcInfo.WorkingDirectory = sComplockPath
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            If ProcFT.ExitCode <> 0 Then
                Dim LogPath As String = CASTCommon.GetFSKJIni("COMMON", "LOG")
                Dim fs As New StreamWriter(Path.Combine(LogPath, "COMPLOCK" & Path.GetFileName(Path.ChangeExtension(InFileName, ".LOG"))), True, EncdJ)
                fs.WriteLine(ProcInfo.FileName & " ")
                fs.WriteLine(Arg)
                fs.Write(ProcFT.StandardOutput.ReadToEnd())
                fs.Close()
                fs = Nothing
            End If
        Catch ex As Exception
            Return -1
        End Try

        Return ProcFT.ExitCode

    End Function

    ' �@�\�@ �F �t�H�[�}�b�g�敪���烌�R�[�h�����擾
    '
    ' ���l�@ �F 
    '
    Private Function GetFormatLength() As Integer
        Dim para As CAstBatch.CommData.stPARAMETER = Nothing
        para.FMT_KBN = mInfoArgument.INFOParameter.FMT_KBN
        para.FSYORI_KBN = mInfoArgument.INFOParameter.FSYORI_KBN
        Try
            Dim fmt As CAstFormat.CFormat = CAstFormat.CFormat.GetFormat(para)
            Return fmt.RecordLen
        Catch ex As Exception
            Return 120
        Finally
        End Try
    End Function

    ' �@�\�@ �F �t�@�C���𑼃V�X�A�g�t�H���_�ֈړ�
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �U�֏����敪
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Public Function MoveToOther(ByVal fileName As String, ByVal fSyoriKbn As String) As String
        Dim ToFolder As String

        ToFolder = GetFolderName(HOLDFOLDER, fSyoriKbn)

        If Directory.Exists(ToFolder) = False Then
            Directory.CreateDirectory(ToFolder)
        End If

        File.Copy(fileName, Path.Combine(ToFolder, Path.GetFileName(fileName)), True)
        File.Delete(fileName)

        Return Path.Combine(ToFolder, Path.GetFileName(fileName))
    End Function

    ' �@�\�@ �F �t�@�C�����Í�������
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �L�[�i������R�[�h�C����敛�R�[�h�C�U�֓��j
    '           
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F �X�P�W���[���}�X�^����L�[�����擾
    '
    Public Function FileEncodeBySchMast(ByVal toriSCode As String, ByVal toriFCode As String, ByVal furiDate As String) As String
        Dim Complock As stCOMPLOCK = Nothing

        Dim OraDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQL As New System.Text.StringBuilder(128)

        Try
            SQL.Append("SELECT ")
            SQL.Append(" ENC_KEY1_S")
            SQL.Append(",ENC_KEY2_S")
            SQL.Append(",ENC_OPT1_S")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = " & CASTCommon.SQ(toriSCode))
            SQL.Append("   AND TORIF_CODE_S = " & CASTCommon.SQ(toriFCode))
            SQL.Append("   AND FURI_DATE_S  = " & CASTCommon.SQ(furiDate))
            If OraReader.DataReader(SQL) = False Then
                OraReader.Close()
                Message = "�X�P�W���[���}�X�^���擾�ł��܂���"
                Return ""
            End If
            Complock.KEY = OraReader.GetString("ENC_KEY1_S").PadRight(64, " "c)
            Complock.IVKEY = OraReader.GetString("ENC_KEY2_S").PadRight(32, " "c)
            Complock.AES = OraReader.GetString("ENC_OPT1_S")
        Catch ex As Exception
            Message = ex.Message
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            If Not OraDB Is Nothing Then
                OraDB.Close()
            End If
        End Try

        Return FileEncode(Complock)
    End Function

    ' �@�\�@ �F �t�@�C�����Í�������
    '
    ' ����   �F ARG1 - ���̓t�@�C����
    '           ARG2 - �L�[�i������R�[�h�C����敛�R�[�h�j
    '           
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F �����}�X�^����L�[�����擾
    '
    Public Function FileEncodeByToriMast(ByVal toriSCode As String, ByVal toriFCode As String) As String
        Dim Complock As stCOMPLOCK = Nothing

        Dim OraDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQL As New System.Text.StringBuilder(128)

        Try
            SQL.Append("SELECT ")
            SQL.Append(" ENC_KEY1_T")
            SQL.Append(",ENC_KEY2_T")
            SQL.Append(",ENC_OPT1_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = '1'")
            SQL.Append("   AND TORIS_CODE_T = " & CASTCommon.SQ(toriSCode))
            SQL.Append("   AND TORIF_CODE_T = " & CASTCommon.SQ(toriFCode))
            If OraReader.DataReader(SQL) = False Then
                OraReader.Close()
                Message = "�����}�X�^���擾�ł��܂���"
                Return ""
            End If
            Complock.KEY = OraReader.GetString("ENC_KEY1_T").PadRight(64, " "c)
            Complock.IVKEY = OraReader.GetString("ENC_KEY2_T").PadRight(32, " "c)
            Complock.AES = OraReader.GetString("ENC_OPT1_T")
        Catch ex As Exception
            Message = ex.Message
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            If Not OraDB Is Nothing Then
                OraDB.Close()
            End If
        End Try

        Return FileEncode(Complock)
    End Function

    ' �@�\�@ �F �t�@�C�����Í�������
    '
    ' ����   �F ARG1 - �b�n�l�o�k�n�b�j���
    '           
    '
    ' �߂�l �F �t�@�C����
    '
    ' ���l�@ �F 
    '
    Private Function FileEncode(ByVal complock As stCOMPLOCK) As String
        Select Case complock.AES
            Case "0"
                ' �`�d�r�Ȃ�
                complock.KEY = complock.KEY.Substring(0, 16)
                complock.IVKEY = complock.IVKEY.Substring(0, 16)
            Case "1"
                ' �`�d�r�����P�Q�W�r�b�g
                complock.KEY = complock.KEY.Substring(0, 32)
                complock.IVKEY = complock.IVKEY.Substring(0, 32)
            Case "2"
                ' �`�d�r�����P�X�Q�r�b�g
                complock.KEY = complock.KEY.Substring(0, 48)
                complock.IVKEY = complock.IVKEY.Substring(0, 32)
            Case "3"
                ' �`�d�r�����Q�T�U�r�b�g
                complock.KEY = complock.KEY.Substring(0, 64)
                complock.IVKEY = complock.IVKEY.Substring(0, 32)
        End Select
        complock.RECLEN = GetFormatLength().ToString

        OutFileName = InFileName & ".complock"
        If Directory.Exists(Path.GetDirectoryName(OutFileName)) = False Then
            Directory.CreateDirectory(Path.GetDirectoryName(OutFileName))
        End If
        If File.Exists(OutFileName) = True Then
            File.Delete(OutFileName)
        End If

        ' �Í���
        Dim nRet As Long = EncodeComplock(complock, OutFileName)
        If nRet = -199 Then
            Return ""
        ElseIf nRet <> 0 Then
            Message = "COMPLOCK II �Í��G���[[" & nRet.ToString & "]"
            Return ""
        End If

        Return OutFileName
    End Function

    '�Ǒփ��X�g�ɊY������ϑ��҃R�[�h������������(�S��t�H�[�}�b�g��p)
    Public Function ConvertItakuCode(ByVal filename As String, ByRef Log As CASTCommon.BatchLOG) As Boolean
        '�t�@�C�����݃`�F�b�N
        If File.Exists(filename) = False Then
            Return True
        End If

        If File.Exists(Path.Combine(TXTPATH, "�ϑ��҃R�[�h�Ǒ�.txt")) = False Then
            Return True
        End If

        '�ϑ��҃R�[�h�Ǒփ��X�g
        Dim ItakuCodeList As Hashtable = New Hashtable

        '�ϑ��҃R�[�h�Ǒֈꗗ���擾����
        Dim sw As StreamReader = Nothing
        Try
            sw = New StreamReader(Path.Combine(TXTPATH, "�ϑ��҃R�[�h�Ǒ�.txt"), EncdJ)
            '1�s�ږ���
            sw.ReadLine()

            While sw.Peek > -1
                Dim line() As String = sw.ReadLine.Split(","c)
                '�Ǒ֑O��ʃR�[�h�E�ϑ��҃R�[�h���L�[�Ƃ���
                ItakuCodeList.Add(line(0) & line(1), New String() {line(2), line(3)})
            End While

        Catch ex As Exception
            Log.Write("�ϑ��҃R�[�h�Ǒփ��X�g�擾", "���s", ex.Message)
            Message = "�ϑ��҃R�[�h�Ǒփ��X�g�擾���s"
            Return False

        Finally
            If Not sw Is Nothing Then
                sw.Close()
            End If
        End Try

        '�ϑ��҃R�[�h�Ǒփ��C������
        Dim fs As FileStream = Nothing
        Try
            '�t�@�C����ǂݏ������p�ŊJ��
            fs = New FileStream(filename, FileMode.Open, FileAccess.ReadWrite)

            Dim Rec(0) As Byte              '���R�[�h�敪�i�[�p
            Dim Len As Integer = 0          '���s�R�[�h�̒���
            Dim Hed As Byte                 '�w�b�_���R�[�h�̃f�[�^�敪
            Dim Enc As System.Text.Encoding '�G���R�[�f�B���O

            '�t�@�C���擪1�o�C�g��ǂݎ��G���R�[�h����
            fs.Read(Rec, 0, 1)

            Select Case Rec(0)
                Case 49 'SJIS.GetBytes("1"c)(0)�ɊY��
                    Hed = 49
                    Enc = EncdJ

                    '121�o�C�g�ڂ܂ŃV�[�N
                    fs.Seek(120, SeekOrigin.Begin)

                    '���s�R�[�h����(���s�R�[�h���̃o�C�g��������)
                    While Len < 2
                        fs.Read(Rec, 0, 1) '1�o�C�g�ǂݎ��

                        Select Case Rec(0)
                            Case 50, 56 'EncdJ.GetBytes("2"c)(0) EncdJ.GetBytes("8"c)(0)�ɊY��
                                '�f�[�^�E�g���[�����R�[�h�̏ꍇ�͏I��
                                Exit While
                            Case Else
                                '�f�[�^�E�g���[�����R�[�h�łȂ��ꍇ�͉��s�R�[�h������
                                Len += 1
                        End Select
                    End While

                Case 241 'EncdE.GetBytes("1"c)(0)�ɊY��
                    Hed = 241
                    Enc = EncdE

                Case Else
                    Log.Write("�G���R�[�h����", "���s", "�t�@�C�����F" & filename)
                    Message = "�G���R�[�h���莸�s �t�@�C�����F" & filename
                    Return False
            End Select

            '�t�@�C���擪�܂ŃV�[�N
            fs.Seek(0, SeekOrigin.Begin)

            '�G���R�[�h�E���s�R�[�h�������������̂Ń��R�[�h�ǂݎ��
            While fs.Position < fs.Length
                '�擪1�o�C�g��ǂݎ��
                fs.Read(Rec, 0, 1)

                Select Case Rec(0)
                    Case Hed
                        '�w�b�_���R�[�h�ł���ꍇ
                        Dim Header(12) As Byte
                        '13�o�C�g�ǂݎ�肷��
                        fs.Read(Header, 0, 13)

                        Dim Syubetu As String = Enc.GetString(Header, 0, 2)     '���
                        Dim CodeKbn As String = Enc.GetString(Header, 2, 1)     '�R�[�h�敪
                        Dim ItakuCode As String = Enc.GetString(Header, 3, 10)  '�ϑ��҃R�[�h

                        '�ϑ��҃R�[�h�`�F�b�N
                        If ItakuCodeList.ContainsKey(Syubetu & ItakuCode) Then
                            Dim value() As String = DirectCast(ItakuCodeList.Item(Syubetu & ItakuCode), String())
                            '13�o�C�g�����߂��ēǑ֏��ɏ�������
                            fs.Seek(-13, SeekOrigin.Current)
                            fs.Write(Enc.GetBytes(value(0) & CodeKbn & value(1)), 0, 13)

                            Log.Write("�ϑ��҃R�[�h�Ǒ�", "����", "�ϑ��҃R�[�h�F" & Syubetu & "-" & ItakuCode _
                                & " -> " & value(0) & "-" & value(1) & " �t�@�C�����F" & filename)
                        End If

                        '���̃��R�[�h�փV�[�N
                        fs.Seek(106 + Len, SeekOrigin.Current)

                    Case Else
                        '���̃��R�[�h�փV�[�N
                        fs.Seek(119 + Len, SeekOrigin.Current)
                End Select
            End While

        Catch ex As Exception
            Log.Write("�ϑ��҃R�[�h�Ǒ�", "���s", "�t�@�C�����F" & filename & " " & ex.Message)
            Message = "�ϑ��҃R�[�h�Ǒ֎��s �t�@�C�����F" & filename
            Return False

        Finally
            If Not fs Is Nothing Then
                fs.Close()
            End If
        End Try

        Return True
    End Function

End Class
