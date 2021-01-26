Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CASTCommon.ModPublic
Imports System.Globalization
Imports CASTCommon

Public Class ClsJifkeiCreate

    Public MainLOG As New CASTCommon.BatchLOG("KFK090", "���U�_�񃊃G���^�쐬")

    Dim MainDB As CASTCommon.MyOracle

    'Private clsFUSION As New clsFUSION.clsMain

    Private strKEKKA As String              ' �f�[�^�쐬����

    Private jobMessage As String = ""          ' �W���u�Ď����b�Z�[�W

    ' �������t
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ''' <summary>
    ''' ini�t�@�C�����
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni

        Dim JIKINKO_CODE As String       '�����ɃR�[�h
        Dim JIKINKO_NAME As String       '�����ɖ�
        Dim HONBU_CODE As String         '�{���R�[�h
        Dim RIENTA_PATH As String        '���G���^�t�@�C���쐬��
        Dim DAT_PATH As String           'DAT�̃p�X
        Dim JIF_RIENTA_FILENAME As String   '���G���^�t�@�C����
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
        Dim RSV2_EDITION As String        ' RSV2�@�\�ݒ�
        Dim COMMON_BAITAIWRITE As String  ' �}�̏����p�t�H���_
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' ���U�_��}�X�^�̃L�[���ځ{���G���^�t�@�C����
    ''' </summary>
    ''' <remarks></remarks>
    Structure JifmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' ������
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private key As JifmastKey

    Dim paraSyoriDate As String        '�p�����[�^��������p����������

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' ���U�_�񃊃G���^�쐬��������
    ''' </summary>
    ''' <returns>����:True �ُ�:False</returns>
    ''' <remarks></remarks>
    Public Function JikeiInit(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try
            '�p�����[�^�̓Ǎ�
            param = CmdArgs(0).Split(","c)
            If param.Length = 2 Then

                '���O�����ݏ��̐ݒ�
                MainLOG.FuriDate = param(0)                     '�������Z�b�g
                MainLOG.JobTuuban = CType(param(1), Integer)
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(��������)�J�n", "����")


                paraSyoriDate = param(0)                       '���������Z�b�g

            Else
                MainLOG.Write("(��������)�J�n", "���s", "�R�}���h���C�������̃p�����[�^���s���ł�")

                Return False

            End If

            'ini�t�@�C���̓Ǎ�
            If IniRead() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("(��������)�J�n", "���s", ex.Message)

            Return False
        Finally
            MainLOG.Write("(��������)�I��", "����")
        End Try

    End Function

    Private Function IniRead() As Boolean

        ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")           '�����ɃR�[�h
        If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�����ɃR�[�h ����:COMMON ����:KINKOCD"
            Return False
        End If

        ini_info.JIKINKO_NAME = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")       '�����ɖ�
        If ini_info.JIKINKO_NAME = "err" OrElse ini_info.JIKINKO_NAME = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�����ɖ� ����:COMMON ����:KINKONAME")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�����ɖ� ����:COMMON ����:KINKONAME"
            Return False
        End If

        ini_info.HONBU_CODE = CASTCommon.GetFSKJIni("COMMON", "HONBUCD")         '�{���R�[�h
        If ini_info.HONBU_CODE = "err" OrElse ini_info.HONBU_CODE = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�{���R�[�h ����:COMMON ����:HONBUCD")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�{���R�[�h ����:COMMON ����:HONBUCD"
            Return False
        End If

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        '���G���^�t�@�C���쐬��
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���G���^�t�@�C���쐬�t�H���_ ����:COMMON ����:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DAT�̃p�X
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:DAT�t�H���_ ����:COMMON ����:DAT"
            Return False
        End If

        ini_info.JIF_RIENTA_FILENAME = CASTCommon.GetFSKJIni("COMMON", "JIKEI_RIENTAFILENAME")       '���G���^�t�@�C����
        If ini_info.JIF_RIENTA_FILENAME = "err" OrElse ini_info.JIF_RIENTA_FILENAME = "" OrElse ini_info.JIF_RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:���U�_�񃊃G���^�t�@�C���� ����:COMMON ����:JIKEI_RIENTAFILENAME")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:���U�_�񃊃G���^�t�@�C���� ����:COMMON ����:JIKEI_RIENTAFILENAME"
            Return False
        End If

        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
        ini_info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If ini_info.RSV2_EDITION = "err" OrElse ini_info.RSV2_EDITION = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:RSV2�@�\�ݒ� ����:RSV2_V1.0.0 ����:EDITION"
            Return False
        End If

        ini_info.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
        If ini_info.COMMON_BAITAIWRITE = "err" OrElse ini_info.COMMON_BAITAIWRITE = "" Then
            MainLOG.Write("INI�t�@�C���擾", "���s", "�ݒ�t�@�C���擾���s ���ږ�:�}�̏����p�t�H���_ ����:COMMON ����:BAITAIWRITE")
            jobMessage = "�ݒ�t�@�C���擾���s ���ږ�:�}�̏����p�t�H���_ ����:COMMON ����:BAITAIWRITE"
            Return False
        End If
        ' 2016/01/18 �^�X�N�j���� ADD �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- END

        Return True

    End Function

    ' �@�\�@ �F ���U�_�񃊃G���^�쐬���� ���C������
    '
    ' �����@ �F ARG1 - �N���p�����[�^
    '
    ' �߂�l �F True - ����CFalse - �ُ�
    '
    ' ���l�@ �F 
    '
    Public Function Main(ByVal command As String) As Integer

        MainDB = New CASTCommon.MyOracle
        Dim bRet As Boolean = True
        Dim iRet As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        ' �p�����[�^�`�F�b�N
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "���U�_�񃊃G���^�쐬����(�J�n)", "����")
            'MainLOG.Write("(�又��)�J�n", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

            '*******************************
            ' �񎟂��擾
            '*******************************
            If GetKaiji() = False Then
                MainLOG.Write("(�又��)", "���s", "�񎟂̎擾�Ɏ��s���܂���")

                Return -1
            End If

            '*****************************************
            ' ���U�_��f�[�^���i�[�ƃX�P�W���[���̍X�V
            '*****************************************
            Dim aryJikei As New ArrayList      '���U�_��f�[�^�i�[�p
            iRet = MakeJikeiData(aryJikei)
            Select Case iRet
                Case 0          ' �f�[�^�i�[����
                    bRet = True
                Case 1          ' �Ώۃf�[�^�O��
                    bRet = True
                Case Else       ' �f�[�^�i�[���s
                    bRet = False
            End Select

            '********************************************
            '���U�_��e�[�u�����폜
            '********************************************
            If iRet = 0 Then bRet = DELETE_STORE_JIFKEIYAKU()

            '***********************
            ' ���G���^FD�쐬
            '***********************
            Dim totalRow As Integer = aryJikei.Count()
            Dim msgtitle As String = "���U�_�񃊃G���^�쐬(KFJ090)"

            If iRet = 0 AndAlso aryJikei.Count() > 0 Then

                If MakeRientaFD(aryJikei) = False Then
                    jobMessage = "���U�_�񃊃G���^�쐬���s"
                    iRet = -1
                Else
                    ' 2016/01/18 �^�X�N�j���� CHG �yPG�zUI_B-14-01(RSV2�Ή�) -------------------- START
                    'Do
                    '    Try
                    '        '2012/01/13 saitou �W���C�� MODIFY ------------------------------------->>>>
                    '        '���G���^�쐬���INI�t�@�C���ŊǗ�����
                    '        Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                    '        'Dim DirInfo As New DirectoryInfo("A:\")
                    '        '2012/01/13 saitou �W���C�� MODIFY -------------------------------------<<<<
                    '        Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                    '        iRet = 0
                    '        Exit Do

                    '    Catch ex As Exception
                    '        '2012/01/13 saitou �W���C�� MODIFY ------------------------------------->>>>
                    '        '���G���^�쐬���INI�t�@�C���ŊǗ�����
                    '        If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                    '            iRet = -1
                    '            jobMessage = "FD�}�����L�����Z������܂����B"
                    '            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                    '            MainLOG.Write_LEVEL1("", "FD�}�����L�����Z��")
                    '            'MainLOG.Write("", "FD�}�����L�����Z��")
                    '            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

                    '            Exit Do
                    '        End If
                    '        'If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot("A:\")), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                    '        '    iRet = -1
                    '        '    jobMessage = "FD�}�����L�����Z������܂����B"
                    '        '    MainLOG.Write("", "FD�}�����L�����Z��")
                    '        '    Exit Do
                    '        'End If
                    '        '2012/01/13 saitou �W���C�� MODIFY -------------------------------------<<<<
                    '    End Try
                    'Loop

                    'Select Case iRet
                    '    Case 0          ' �f�[�^�i�[����
                    '        '2012/01/13 saitou �W���C�� MODIFY ------------------------------------->>>>
                    '        '���G���^�쐬���INI�t�@�C���ŊǗ�����
                    '        If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME)) Then
                    '            '2014/05/01 saitou �W���ŏC�� MODIFY ----------------------------------------------->>>>
                    '            '���b�Z�[�W�{�b�N�X���őO�ʂɏo��
                    '            If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                    '                jobMessage = "�t���b�s�[���t�@�C���폜�L�����Z��"
                    '                iRet = -1
                    '            End If
                    '            'If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
                    '            '    jobMessage = "�t���b�s�[���t�@�C���폜�L�����Z��"
                    '            '    iRet = -1
                    '            'End If
                    '            '2014/05/01 saitou �W���ŏC�� MODIFY -----------------------------------------------<<<<
                    '        End If
                    '        'If File.Exists(Path.Combine("A:\", ini_info.JIF_RIENTA_FILENAME)) Then
                    '        '    If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
                    '        '        jobMessage = "�t���b�s�[���t�@�C���폜�L�����Z��"
                    '        '        iRet = -1
                    '        '    End If
                    '        'End If
                    '        '2012/01/13 saitou �W���C�� MODIFY -------------------------------------<<<<

                    '        If iRet = 0 Then
                    '            '2012/01/13 saitou �W���C�� MODIFY ------------------------------------->>>>
                    '            '���G���^�쐬���INI�t�@�C���ŊǗ�����
                    '            File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME), True)
                    '            'File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine("A:\", ini_info.JIF_RIENTA_FILENAME), True)
                    '            '2012/01/13 saitou �W���C�� MODIFY -------------------------------------<<<<
                    '        End If
                    'End Select
                    Select Case ini_info.RSV2_EDITION
                        Case "2"
                            '---------------------------------------------------------------
                            ' �t�@�C�����\�z
                            '  [�t�@�C����] RNT_JR_yyyyMMdd_HHmmss_1(1�Œ�)
                            '---------------------------------------------------------------
                            Dim RientFileName As String = "RNT_JR_" & strDate & "_" & strTime & "_1"

                            '---------------------------------------------------------------
                            ' �t�@�C���R�s�[
                            '---------------------------------------------------------------
                            If File.Exists(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName)) Then
                                File.Delete(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName))
                            End If
                            File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName), True)
                        Case Else
                            Do
                                Try
                                    Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                                    Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                                    iRet = 0
                                    Exit Do

                                Catch ex As Exception
                                    If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                                        iRet = -1
                                        jobMessage = "FD�}�����L�����Z������܂����B"
                                        '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
                                        MainLOG.Write_LEVEL1("", "FD�}�����L�����Z��")
                                        'MainLOG.Write("", "FD�}�����L�����Z��")
                                        '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

                                        Exit Do
                                    End If
                                End Try
                            Loop

                            Select Case iRet
                                Case 0          ' �f�[�^�i�[����
                                    If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME)) Then
                                        If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                            jobMessage = "�t���b�s�[���t�@�C���폜�L�����Z��"
                                            iRet = -1
                                        End If
                                    End If

                                    If iRet = 0 Then
                                        File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME), True)
                                    End If
                            End Select

                    End Select
                End If

            End If

            If iRet <> 0 Then
                bRet = False
            End If

            '*******************************
            ' ���[�o��
            '*******************************
            ' ���U�_��f�[�^���P���ȏ㑶�݂���ꍇ�A���[�o��
            If iRet = 0 Then
                ' ���[�o��
                'LOG.Write("���[�o�͊J�n", "����")

                '' �������ʊm�F�\
                Dim PrnSyoKekka As ClsKFJP043 = Nothing
                Dim intPrnRet As Integer

                PrnSyoKekka = New ClsKFJP043


                PrnSyoKekka.OraDB = MainDB
                ' ���U�_�񏈗����ʊm�F�\�@�^�C�g���s�o��
                PrnSyoKekka.CreateCsvFile()

                ' ���U�_�񏈗����ʊm�F�\�@���׍s�o��
                intPrnRet = PrnSyoKekka.OutputCSVKekka(aryJikei, ini_info.JIKINKO_CODE, strDate, strTime)

                If intPrnRet <> 0 Then
                    bRet = False
                    MainLOG.Write("�������ʊm�F�\(���U�_��)�o��", "���s", "�������ʊm�F�\(���U�_��)�b�r�u�o�͂Ɏ��s���܂����B")
                End If

                ' ���U�_�񏈗����ʊm�F�\
                If Not PrnSyoKekka Is Nothing And intPrnRet = 0 Then
                    PrnSyoKekka.CloseCsv()

                    '����o�b�`�Ăяo��
                    Dim ExeRepo As New CAstReports.ClsExecute
                    Dim param As String = ""
                    Dim nret As Integer

                    '�p�����[�^�ݒ�F���O�C�����A�b�r�u�t�@�C����
                    param = MainLOG.UserID & "," & PrnSyoKekka.FileName

                    nret = ExeRepo.ExecReport("KFJP043.EXE", param)

                    If nret <> 0 Then
                        '������s�F�߂�l�ɑΉ������G���[���b�Z�[�W��\������
                        Select Case nret
                            Case -1
                                jobMessage = "�������ʊm�F�\(���U�_��)����ΏۂO���B"

                            Case Else

                                jobMessage = "�������ʊm�F�\(���U�_��)������s�B�G���[�R�[�h�F" & nret
                        End Select
                        MainLOG.Write("�������ʊm�F�\(���U�_��)���", "���s", jobMessage)
                        bRet = False
                    End If
                End If
            End If

            If bRet = False Then

                If jobMessage = "" Then
                    Call MainLOG.UpdateJOBMASTbyErr("���O�Q��")
                Else
                    Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                End If

                ' ���[���o�b�N
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "�Ώۃf�[�^�O��"
                End If

                Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                ' �R�~�b�g
                MainDB.Commit()
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            MainLOG.Write("(�又��)", "���s", ex.ToString)
            Return 1
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG�����Ή� ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "���U�_�񃊃G���^�쐬����(�I��)", "����")
            'MainLOG.Write("(�又��)�I��", "����")
            '*** end Add 2015/12/26 sys)mori for LOG�����Ή� ***

        End Try

        Return 0

    End Function

    ' �@�\�@ �F ���U�_��f�[�^�쐬����
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '

    Private Function MakeJikeiData(ByRef aryJifKei As ArrayList) As Integer

        Dim OraReader As CASTCommon.MyOracleReader
        Dim SQL As StringBuilder
        Dim Jdata As CAstFormKes.ClsFormKes.JifkeiData
        Dim fmt10004 As CAstFormKes.ClsFormSikinFuri.T_10004

        Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
        culture.DateTimeFormat.Calendar = New JapaneseCalendar()
        Dim target As DateTime
        Dim result As String
        Dim Cnt As Integer = 0

        OraReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)


        Try
            MainLOG.Write("(���U�_��f�[�^�i�[)�J�n", "����")

            '***********************
            ' �_�����a���
            '***********************
            target = Date.Parse(Format(CInt(strDate), "0000/00/00"))
            result = target.ToString("yyMMdd", culture)

            SQL.Append("SELECT")
            SQL.Append(" KEIYAKU_SIT_JK")
            SQL.Append(",KEIYAKU_KAMOKU_JK")
            SQL.Append(",KEIYAKU_KOUZA_JK")
            SQL.Append(",KEIYAKU_KNAME_JK")
            SQL.Append(",FURI_CODE_JK")
            SQL.Append(",KIGYO_CODE_JK")
            SQL.Append(",TORIS_CODE_JK")
            SQL.Append(",TORIF_CODE_JK")
            SQL.Append(",FURI_DATE_JK")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",RECORD_NO_JK")
            SQL.Append(" FROM STORE_JIFKEIYAKU,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" TORIS_CODE_JK = TORIS_CODE_T ")
            SQL.Append(" AND TORIF_CODE_JK = TORIF_CODE_T ")
            SQL.Append(" ORDER BY FURI_CODE_JK,KIGYO_CODE_JK,KEIYAKU_SIT_JK,KEIYAKU_KAMOKU_JK,KEIYAKU_KOUZA_JK ")

            If OraReader.DataReader(SQL) = True Then

                Do Until OraReader.EOF

                    Cnt += 1

                    Jdata = New CAstFormKes.ClsFormKes.JifkeiData
                    fmt10004 = New CAstFormKes.ClsFormSikinFuri.T_10004
                    Jdata.Init()
                    fmt10004.Init()

                    ' �f�[�^�ݒ�
                    With fmt10004
                        .KAMOKU_KOUZA_NO = String.Concat(OraReader.GetString("KEIYAKU_KAMOKU_JK"), OraReader.GetString("KEIYAKU_KOUZA_JK"))        ' �Ȗڌ����ԍ�
                        .GYO = "01"                                           ' �s
                        .JIFURI_CODE = OraReader.GetString("FURI_CODE_JK")    ' �U�փR�[�h
                        .KIGYO_CODE = OraReader.GetString("KIGYO_CODE_JK")    ' ��ƃR�[�h
                        .KEIYAKU_DATE = result                                ' �_���(�a��)
                        .KOUFURIUKETUKE = "".PadLeft(1, " "c)                 ' ���U��t�T�[�r�X
                        '****20120709 mubuchi �A�n�M���C��******************************************>>>>
                        '�{���R�[�h�ƌ_��Ҏx�X�R�[�h�������������ꍇ��[���_�ԍ�]�ɂ͋󔒂��Z�b�g����B
                        If ini_info.HONBU_CODE = OraReader.GetString("KEIYAKU_SIT_JK") Then
                            .GENTEN_NO = "".PadLeft(3, " "c)    ' ���_�ԍ�
                        Else
                            .GENTEN_NO = OraReader.GetString("KEIYAKU_SIT_JK")    ' ���_�ԍ�
                        End If
                        '****20120709 mubuchi �A�n�M���C��******************************************<<<<<
                        .YOBI1 = ""                                           ' �\���P
                    End With

                    ' �f�[�^�ݒ�
                    With Jdata
                        .SyoriDate = paraSyoriDate                            '���ϓ�
                        .TorisCode = OraReader.GetString("TORIS_CODE_JK")     '������R�[�h
                        .TorifCode = OraReader.GetString("TORIF_CODE_JK")     '����敛�R�[�h
                        .FuriDate = OraReader.GetString("FURI_DATE_JK")    '�U�֓�
                        .ToriNName = OraReader.GetString("ITAKU_NNAME_T")     '����於
                        .FuriCode = OraReader.GetString("FURI_CODE_JK")       '�U�փR�[�h
                        .KigyoCode = OraReader.GetString("KIGYO_CODE_JK")     '��ƃR�[�h
                        .KeiyakuKname = OraReader.GetString("KEIYAKU_KNAME_JK") '�_��Җ��J�i
                        .MeiRecordNo = OraReader.GetString("RECORD_NO_JK").ToString
                    End With

                    Jdata.record320 = fmt10004.Data
                    Jdata.OpeCode = String.Concat(fmt10004.KAMOKU_CODE, fmt10004.OPE_CODE)


                    ' �Œ蒷�ɕϊ�����
                    Jdata.Data = Jdata.Data

                    aryJifKei.Add(Jdata)

                    OraReader.NextRead()
                Loop

            End If

            If aryJifKei.Count = 0 Then
                MainLOG.Write("(���U�_��f�[�^�i�[)", "���s", "�����O��")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(���U�_��f�[�^�i�[)", "���s", ex.Message)

            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            MainLOG.Write("(���U�_��f�[�^�i�[)�I��", "����")

        End Try

        Return 0

    End Function


    ' �@�\�@ �F ���U�_��}�X�^�o�^
    '
    ' �߂�l �F TRUE - ���� �C FALSE - �ُ�
    '
    ' ���l�@ �F 
    '
    Private Function InsertJifMast(ByVal JData As CAstFormKes.ClsFormKes.JifkeiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Dim fmt10004 As New CAstFormKes.ClsFormSikinFuri.T_10004


        Try
            MainLOG.Write("(���U�_��}�X�^�o�^)�J�n", "����")


            fmt10004.Init()
            fmt10004.Data = JData.record320

            SQL.Append("INSERT INTO JIKEIMAST(")
            SQL.Append(" SYORI_DATE_JR")
            SQL.Append(",TIME_STAMP_JR")
            SQL.Append(",KAIJI_JR")
            SQL.Append(",RECORD_NO_JR")
            SQL.Append(",FILE_NAME_JR")
            SQL.Append(",TORIS_CODE_JR")
            SQL.Append(",TORIF_CODE_JR")
            SQL.Append(",FURI_DATE_JR")
            SQL.Append(",MEI_RECORD_NO_JR")
            SQL.Append(",FURI_CODE_JR")
            SQL.Append(",KIGYO_CODE_JR")
            SQL.Append(",TSIT_NO_JR")
            SQL.Append(",KAMOKU_JR")
            SQL.Append(",KOUZA_JR")
            SQL.Append(",KAMOKU_CODE_JR")
            SQL.Append(",OPE_CODE_JR")
            SQL.Append(",DENBUN_ALL_JR")
            SQL.Append(",ERR_CODE_JR")
            SQL.Append(",ERR_MSG_JR")
            SQL.Append(",SAKUSEI_DATE_JR")
            SQL.Append(",KOUSIN_DATE_JR")
            SQL.Append(") VALUES (")
            SQL.Append(" " & SQ(strDate))                                   ' ������
            SQL.Append("," & SQ(String.Concat(strDate, strTime)))           ' �^�C���X�^���v
            SQL.Append("," & SQ(key.Kaiji))                              ' ��
            SQL.Append("," & SQ(key.RecordNo))                           ' �ʔ�
            SQL.Append("," & SQ(ini_info.JIF_RIENTA_FILENAME))                     ' ���G���^�t�@�C����
            SQL.Append("," & SQ(JData.TorisCode))                        ' ������R�[�h
            SQL.Append("," & SQ(JData.TorifCode))                        ' ����敛�R�[�h
            SQL.Append("," & SQ(JData.FuriDate))                         ' �U�֓�
            SQL.Append("," & JData.MeiRecordNo.Trim)
            SQL.Append("," & SQ(JData.FuriCode))
            SQL.Append("," & SQ(JData.KigyoCode))
            SQL.Append("," & SQ(fmt10004.GENTEN_NO))
            SQL.Append("," & SQ(fmt10004.KAMOKU_KOUZA_NO.Substring(0, 2)))
            SQL.Append("," & SQ(fmt10004.KAMOKU_KOUZA_NO.Substring(2, 7)))
            SQL.Append("," & SQ(JData.OpeCode.Substring(0, 2)))           ' �ȖڃR�[�h
            SQL.Append("," & SQ(JData.OpeCode.Substring(2, 3)))           ' �I�y�R�[�h
            SQL.Append("," & SQ(JData.record320))                           ' �ʃf�[�^
            SQL.Append("," & SQ(""))                                        ' ���ʃR�[�h
            SQL.Append("," & SQ(""))                                        ' �G���[���b�Z�[�W
            SQL.Append("," & SQ(strDate))                                   ' �쐬��
            SQL.Append("," & SQ(strDate))                                   ' �X�V��
            SQL.Append(")")

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then Return False

        Catch ex As Exception
            MainLOG.Write("(���U�_��}�X�^�o�^)", "���s", ex.Message)

            Return False
        Finally

            MainLOG.Write("(���U�_��}�X�^�o�^)�J�n", "����")

        End Try

        Return True

    End Function

    Private Function DELETE_STORE_JIFKEIYAKU() As Boolean

        Dim SQL As New StringBuilder(128)

        Try
            MainLOG.Write("(���U�_��e�[�u���폜)�J�n", "����")


            SQL.Append("DELETE FROM STORE_JIFKEIYAKU")

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then Return False

        Catch ex As Exception
            MainLOG.Write("(���U�_��e�[�u���폜)", "���s", ex.Message)
            Return False
        Finally
            MainLOG.Write("(���U�_��e�[�u���폜)�J�n", "����")
        End Try

        Return True

    End Function

    Private Function MakeRientaFD(ByVal ary As ArrayList) As Boolean

        Dim T_RIENT77 As New CAstFormKes.ClsT_RIENT77
        Dim T_RIENT10 As New CAstFormKes.ClsT_RIENT10

        Dim Jdata As CAstFormKes.ClsFormKes.JifkeiData
        Dim EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")

        Dim T_10004 As New CAstFormKes.ClsFormSikinFuri.T_10004

        Dim StrmWrite As FileStream = Nothing

        Try

            ' �^���L���O�w�b�_  
            ' �����f�[�^�ݒ�
            Call T_RIENT77.TANKING_HEAD.Init()
            ' �s���t
            T_RIENT77.TANKING_HEAD.strT_HIDUKE = CASTCommon.Calendar.Now.ToString("yyyyMMdd")

            ' �X�܏�񃌃R�[�h�i�P�X�܏��j
            ' �����f�[�^�ݒ�
            Call T_RIENT77.TENPO_INFOREC(0).Init()
            ' ���ɃR�[�h
            T_RIENT77.TENPO_INFOREC(0).strKINKO_CD = ini_info.JIKINKO_CODE
            ' �X�܃R�[�h
            T_RIENT77.TENPO_INFOREC(0).strSIT_CD = ini_info.HONBU_CODE

            ' �X�܏�񃌃R�[�h�i�Q�`�R�Q�X�܏��j
            ' �����f�[�^�ݒ�
            For i As Integer = 1 To T_RIENT77.TENPO_INFOREC.Length - 1
                ' �Q�`�R�Q�̏����f�[�^�ݒ�
                Call T_RIENT77.TENPO_INFOREC(i).Init2_32()
            Next i

            ' �\���R
            ' ������
            Call T_RIENT77.DATA_SIKIBETU.Init()

            ' ���G���^�t�@�C�� �I�[�v��

            If File.Exists(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME)) = True Then
                ' ���ɑ��݂���ꍇ�́C�폜
                File.Delete(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME))
            End If

            StrmWrite = New FileStream(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), FileMode.OpenOrCreate, FileAccess.ReadWrite)

            Dim Bytes As Byte() = EncdJ.GetBytes(ini_info.JIF_RIENTA_FILENAME.PadRight(12, Nothing).PadRight(16, " "c))
            StrmWrite.Write(Bytes, 0, 16)

            ' �^���L���O�w�b�_����
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.Data, 0, 28)

            ' �X�܏�񃌃R�[�h����
            For i As Integer = 0 To T_RIENT77.TENPO_INFOREC.Length - 1
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(i).Data, 0, 28)
            Next i

            ' �\���R����
            StrmWrite.Write(T_RIENT77.DATA_SIKIBETU.Data, 0, 84)

            Dim WriteCount As Integer = 0           ' ��������

            Dim RecLen As Integer

            ' �^���L���O�f�[�^����

            Dim cnt As Integer = ary.Count - 1 '���[�v��

            For i As Integer = 0 To cnt

                ' �^���L���O�f�[�^
                Jdata = CType(ary.Item(i), CAstFormKes.ClsFormKes.JifkeiData)

                Select Case Jdata.OpeCode
                    Case "10004"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_10004.DataSepaPlus = Jdata.record320
                        RecLen = T_10004.DataSepaPlus.Replace(" ", "").Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Jdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Jdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Jdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_10004.DataSepaPlus
                        T_10004 = Nothing
                    Case Else
                        '�G���[
                End Select

                WriteCount += 1

                ' ���z�Z�p���[�^
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)

                ' �^���L���O�A��
                T_RIENT10.TANKING_DATA.bytTANKING_NO(0) = CType(WriteCount \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytTANKING_NO(1) = CType(WriteCount Mod 256, Byte)

                ' ���f�[�^�A�h���X
                If cnt + 1 = WriteCount Then
                    ' �ŏI�s
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = 255
                Else
                    Dim NextAddr As Integer = 1024 + (WriteCount * 256)

                    Dim NextAddr0 As Integer = CType(NextAddr \ 16777216, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = CType(NextAddr0, Byte)

                    Dim NextAddr1 As Integer = CType((NextAddr Mod 16777216) \ 65536, Integer)
                    Dim Amari1 As Integer = CType((NextAddr Mod 16777216) Mod 65536, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = CType(NextAddr1, Byte)

                    Dim NextAddr2 As Integer = CType(Amari1 \ 256, Integer)
                    Dim Amari2 As Integer = CType(Amari1 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = CType(NextAddr2, Byte)

                    Dim NextAddr3 As Integer = CType(Amari2 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = CType(NextAddr3, Byte)
                End If

                ' ���X�R�[�h
                T_RIENT10.TANKING_DATA.strKINTEN_CD = T_RIENT77.TENPO_INFOREC(0).strKINKO_CD & T_RIENT77.TENPO_INFOREC(0).strSIT_CD

                '�X�y�[�X��""�ɒu���Ȃ���
                T_RIENT10.TANKING_DATA.strTANKING_DATA = T_RIENT10.TANKING_DATA.strTANKING_DATA.Replace(" ", "")


                '���z�Z�p���[�^���Čv�Z���ăZ�b�g
                'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)
                '*****************************************

                StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_10, 0, 256)

                key.RecordNo = i + 1

                If InsertJifMast(Jdata) = False Then
                    MainLOG.Write("���U�_��}�X�^�o�^", "���s", "")

                    Return False
                End If

            Next

            ' �ŏI���R�[�h
            T_RIENT10.TANKING_LAST.Init()
            StrmWrite.Write(T_RIENT10.TANKING_LAST.Data, 0, 512)

            ' �^���L���O�w�b�_ �������� �ď���
            ' �S�X�܂s����
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(20 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN, 0, 2)

            ' �X�܂s����
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(36 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN, 0, 2)

            ' �X�܂s�I���e�c�A�h���X
            '***�C���@�O�c�@20080930****************************************
            'WriteCount��1�ȊO�Ȃ�A�I���A�h���X�𐳂����ݒ肷��
            If WriteCount <> 1 Then

                '***�C�� maeda 2001006*******************************************
                '��������-1�̒l�ŏI���A�h���X���v�Z����(���m�ɂ͍ŏI���R�[�h�̊J�n�A�h���X�̂���)
                Dim FinishAddr As Integer = 1024 + ((WriteCount - 1) * 256)
                'Dim FinishAddr As Integer = 1024 + (WriteCount * 256)
                '****************************************************************

                Dim FinishAddr0 As Integer = CType(FinishAddr \ 16777216, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(0) = CType(FinishAddr0, Byte)

                Dim FinishAddr1 As Integer = CType((FinishAddr Mod 16777216) \ 65536, Integer)
                Dim FinishAmari1 As Integer = CType((FinishAddr Mod 16777216) Mod 65536, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(1) = CType(FinishAddr1, Byte)

                Dim FinishAddr2 As Integer = CType(FinishAmari1 \ 256, Integer)
                Dim FinishAmari2 As Integer = CType(FinishAmari1 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(2) = CType(FinishAddr2, Byte)

                Dim FinishAddr3 As Integer = CType(FinishAmari2 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(3) = CType(FinishAddr3, Byte)
            End If

            StrmWrite.Seek(48 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD, 0, 4)

            StrmWrite.Close()

        Catch ex As Exception
            MainLOG.Write("���G���^�t�@�C���쐬", "���s", ex.Message)

            Return False
        Finally
            If Not StrmWrite Is Nothing Then StrmWrite.Close()
        End Try
        Return True
    End Function

    Private Function GetKaiji() As Boolean

        Dim sql As New StringBuilder(64)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            MainLOG.Write("(�񎟎擾)�J�n", "����", "")

            sql.Append("SELECT NVL(MAX(KAIJI_JR),0) AS MAX_KAIJI FROM JIKEIMAST")
            sql.Append(" WHERE SYORI_DATE_JR = " & SQ(strDate))

            If OraReader.DataReader(sql) = True Then
                key.Kaiji = CType(OraReader.GetInt64("MAX_KAIJI"), Integer) + 1
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("(�񎟎擾)", "���s", ex.ToString)

            Return False
        Finally
            MainLOG.Write("(�񎟎擾)�I��", "����", "")

            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return True

    End Function


    Private Function GetTENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(MainDB)

        Try
            KIN_NNAME = ""
            SIT_NNAME = ""
            KIN_KNAME = ""
            SIT_KNAME = ""

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & KIN_NO.Trim & "' AND SIT_NO_N = '" & SIT_NO.Trim & "'")

            If orareader.DataReader(sql) = True Then
                KIN_NNAME = orareader.GetString("KIN_NNAME_N")
                SIT_NNAME = orareader.GetString("SIT_NNAME_N")
                KIN_KNAME = orareader.GetString("KIN_KNAME_N")
                SIT_KNAME = orareader.GetString("SIT_KNAME_N")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
