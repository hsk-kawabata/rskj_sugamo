Option Explicit On 
Option Strict On

Imports CASTCommon
Imports MenteCommon
Imports System.Data.OracleClient
Imports CMT


Module ModMain
    Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" _
        (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer

    Public GCom As MenteCommon.clsCommon

    Public CmtCom As clsCmtCommon       '���v���W�F�N�g���̋��ʋ@�\�֐��Q
    Public DB As clsDataBase            '���v���W�F�N�g���̃e�[�u���֘A���ʊ֐��Q
    Public GOwnerForm As Form           '�ėp

    ' �A��CMT�̍ő�X�^�b�J��
    Public Const MAXSTACKER As Integer = 1         '***�C�� ���� 10 �� 1

    'INI�t�@�C���ǂݍ��ݗp�ϐ�
    Public gintTEMP_LEN As Integer    'API�֐��擾������
    Public gstrIFileName As String
    Public gstrIAppName As String
    Public gstrIKeyName As String
    Public gstrIDefault As String

    ' �i�[�t�H���_��
    Public gstrCMTServerRead As String                          ' �T�[�o�̓Ǎ��p�p�X
    Public gstrCMTServerWrite As String                         ' �T�[�o�̏����p�p�X
    Public gstrCMTWriteFileName(MAXSTACKER) As String           ' ���[�J��PC��̏o�̓t�@�C����
    Public gstrCMTReadFileName(MAXSTACKER) As String            ' ���[�J��PC��̓��̓t�@�C����
    Public gstrCMTHeadLabelFileName(MAXSTACKER) As String       ' ���[�J��PC��̃��x���t�@�C����
    Public gstrCMTEndLabelFileName(MAXSTACKER) As String        ' ���[�J��PC���EOF���x���t�@�C����

    Private Const ThisModuleName As String = "ModMain.vb"

    Public Const GErrorString As String = "�\�����ʃG���["

    Public Sub Main()
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim MSG As String = String.Empty
        'Dim MSG As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        '���ʓI�Ȋ֐��𓋍�
        GCom = New MenteCommon.clsCommon
        With GCom.GLog
            .Job = "CMT�ϊ�"
            .Job1 = "�N������"
        End With
        Try
            '��Project��p�N���X������
            CmtCom = New clsCmtCommon           '���v���W�F�N�g���̋��ʋ@�\�֐��Q
            DB = New clsDataBase                '���v���W�F�N�g���̃e�[�u���֘A���ʊ֐��Q

            '��d�N���`�F�b�N
            '*** �C�� mitsu 2008/09/01 ���������� ***
            'If UBound(Diagnostics.Process.GetProcessesByName( _
            '                   Diagnostics.Process.GetCurrentProcess.ProcessName)) > 0 Then
            '    With GCom.GLog
            '        .Job2 = "��d�N���`�F�b�N"
            '        .Result = MenteCommon.clsCommon.NG
            '        .Discription = "�����I��"
            '    End With
            '    Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            '    Return
            'End If
            Dim mp As Process = Process.GetCurrentProcess
            Dim ps() As Process = Process.GetProcesses()
            For i As Integer = 0 To ps.Length - 1
                '�����v���Z�X���Ńv���Z�XID���Ⴄ�ꍇ�͓�d�N�����o
                If ps(i).ProcessName = mp.ProcessName AndAlso ps(i).Id <> mp.Id Then
                    With GCom.GLog
                        .Job2 = "��d�N���`�F�b�N"
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = "�����I��"
                    End With
                    Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                    Return
                End If
            Next
            '****************************************

            '�R�}���h���C���]��
            If Not GetEnvironment(MSG) Then
                With GCom.GLog
                    .Job2 = "�R�}���h���C���]��"
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = MSG
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

                GCom.GetUserID = "NoCommandLine"
                GCom.GetSysDate = #10/20/2007#
            End If


            'ini�t�@�C���ǂݍ���
            If fn_set_INIFILE() = False Then
                With GCom.GLog
                    .Job2 = "ini�t�@�C���擾"
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = ""
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                MessageBox.Show("ini�t�@�C���̎擾�Ɏ��s���܂���", "ini�t�@�C���擾", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' �@�ԃ`�F�b�N. �@�Ԃ̎擾�Ɏ��s�����ꍇ�͏I��
            If Not CmtCom.SetStationNo() Then
                Return
            End If

            '�N������LOG�o��
            With GCom.GLog
                .Job2 = "�V�X�e���N��"
                .Result = MenteCommon.clsCommon.OK
                .Discription = ""
            End With
            Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))


        Catch ex As Exception

        Finally
            '��ʋN��
            Dim onForm As New KFCMENU010
            With onForm
                onForm.Show()
                '2011/06/23 �W���ŏC�� ��ʂ�XP�X�^�C���ɕύX ------------------START
                Application.EnableVisualStyles()
                '2011/06/23 �W���ŏC�� ��ʂ�XP�X�^�C���ɕύX ------------------END

                Application.Run(onForm)

                '�I������LOG�o��
                With GCom.GLog
                    .Job2 = "�V�X�e���I��"
                    .Result = MenteCommon.clsCommon.OK
                    .Discription = ""
                End With
                Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

                GCom = Nothing
                .Dispose()
            End With
        End Try

    End Sub


    ' �@�\�@�@�@: �R�}���h���C����]������
    '
    ' �߂�l�@�@: ���� = True
    ' �@�@�@�@�@  �ُ� = False
    '
    ' �������@�@: ARG1 - �Ȃ�
    '
    ' ���l�@�@�@: ���p����
    '
    Private Function GetEnvironment(ByRef MSG As String) As Boolean
        Dim Cnt As Integer
        Dim CmdLine() As String

        MSG = "�p�����[�^�s���B"

        '�N���p�����[�^�L���^���e����
        If Not Environment.GetCommandLineArgs.Length = 2 Then

            For Cnt = 1 To Environment.GetCommandLineArgs.Length - 1 Step 1
                Select Case Cnt = 1
                    Case True : MSG &= "(" & Environment.GetCommandLineArgs(Cnt)
                    Case Else : MSG &= " " & Environment.GetCommandLineArgs(Cnt)
                End Select
            Next Cnt
            MSG &= ")"

            Return False
        Else
            CmdLine = Environment.GetCommandLineArgs(1).Split(","c)

            For Cnt = 0 To CmdLine.Length - 1 Step 1
                Select Case Cnt = 0
                    Case True : MSG &= "(" & CmdLine(Cnt)
                    Case Else : MSG &= "," & CmdLine(Cnt)
                End Select
            Next Cnt
            MSG &= ")"

            If Not CmdLine.Length = 2 Then Return False
        End If

        '�A�g���[�U�h�c �i�[
        GCom.GetUserID = CmdLine(0).Trim

        ' �A�g���t�����^�����`�F�b�N
        If Not CmdLine(1).Trim.Length = 8 OrElse Not GCom.IsNumber(CmdLine(1).Trim) Then

            Return False
        End If

        '�A�g���t����`�F�b�N & �i�[
        Dim sDate(2) As Integer
        With CmdLine(1).Trim
            sDate(0) = GCom.NzInt(.Substring(0, 4))
            sDate(1) = GCom.NzInt(.Substring(4, 2))
            sDate(2) = GCom.NzInt(.Substring(6))
        End With

        ' �A�g���t(yyyymmdd)���t�@�C�������ŗ��p���邽�߃O���[�o���ϐ��ɃR�s�[
        CmtCom.gstrSysDate = CmdLine(1)

        Return (GCom.SET_DATE(GCom.GetSysDate, sDate) < 0)
    End Function


    '
    ' �@�\�@ �F CMT�ϊ��֘A��INI�t�@�C���̏��擾
    '           �ǂݎ�茳: fskj.ini, cmt.ini
    '
    ' �����@ �F ARG1 - �Ȃ�
    '
    ' �߂�l �F ����I�� = True
    ' �@�@�@ �@ �ُ�I�� = False
    '
    ' ���l�@ �F 2007.11.13 Update By Astar
    '           2007.11.21 Update By Astar ���x���Ή�
    Function fn_set_INIFILE() As Boolean

        Dim Ret As Boolean = False
        Try
            ' FSKJ.INI����̓Ǎ�
            ' �Ǎ��t�@�C�����i�[����T�[�o��t�H���_���̎擾
            'FSKJ.ini����Ƃ邩CMT.ini����Ƃ邩�A�Ƃ肠����CMT.ini��
            gstrCMTServerRead = GetCMTIni("JIFURI", "DEN")

            ' �ԋp�t�@�C��(���ʃt�@�C��)���i�[����T�[�o��̃t�H���_���̎擾
            gstrCMTServerWrite = GetCMTIni("JIFURI", "DEN")

            ' CMT.INI����̓Ǎ�
            Dim strCMTWriteFileName As String = GetCMTIni("FILE-NAME", "WRITE")

            Dim strCMTReadFileName As String = GetCMTIni("FILE-NAME", "READ")
            Dim strCMTHeadLabelFileName As String = GetCMTIni("FILE-NAME", "HEAD")
            Dim strCMTEndLabelFileName As String = GetCMTIni("FILE-NAME", "END")

            For i As Integer = 1 To MAXSTACKER
                gstrCMTWriteFileName(i - 1) = GetCMTIni("WRITE-DIRECTORY", i.ToString) & "\" & strCMTWriteFileName
                gstrCMTReadFileName(i - 1) = GetCMTIni("READ-DIRECTORY", i.ToString) & "\" & strCMTReadFileName
                gstrCMTHeadLabelFileName(i - 1) = GetCMTIni("READ-DIRECTORY", i.ToString) & "\" & strCMTHeadLabelFileName
                gstrCMTEndLabelFileName(i - 1) = GetCMTIni("READ-DIRECTORY", i.ToString) & "\" & strCMTEndLabelFileName
                If ((gstrCMTWriteFileName(i - 1) = "err") Or (gstrCMTReadFileName(i) = "err")) Then
                    Throw New System.Exception("CMT.ini�t�@�C���ǎ掸�s")
                End If
            Next i

            ' �����ꂩ�ЂƂł��擾���s�����ꍇ�̓G���[�Ƃ݂Ȃ�
            If ((gstrCMTServerRead = "err") Or (gstrCMTServerWrite = "err")) Then
                Ret = False
            Else
                Ret = True
            End If
        Catch ex As Exception
            Ret = False
        End Try

        REM TODO ini�t�@�C���擾�t�@�C�����̌���

        Return Ret
    End Function
End Module
