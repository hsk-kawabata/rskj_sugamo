Imports System
Imports System.Runtime.InteropServices
Imports System.Diagnostics
Imports System.Collections
'*** Str Add 2015/12/01 SO)�r�� for �ȈՒ��[��� ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)�r�� for �ȈՒ��[��� ***

' INI�t�@�C������ ���W���[��
Public Module ModIni
    ' INI�t�@�C�� ���擾
    Friend Function GetPrivateProfileString( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByRef lpReturnedString As String, ByVal nSize As Integer, _
        ByVal lpFileName As String) As Integer
        Dim str As New System.Text.StringBuilder(1024)
        Dim nRet As Integer = GetIniString(lpAppName, lpKeyName, lpDefault, str, str.Capacity, lpFileName)
        lpReturnedString = str.ToString
        Return nRet
    End Function

    ' INI�t�@�C�� ���擾
    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileString", CharSet:=CharSet.Auto)> _
    Friend Function GetIniString( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByVal lpReturnedString As System.Text.StringBuilder, ByVal nSize As Integer, _
        ByVal lpFileName As String) As Integer
    End Function

    ' INI�t�@�C�� ���擾
    <DllImport("KERNEL32.DLL", CharSet:=CharSet.Auto)> _
    Friend Function GetPrivateProfileInt( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, ByVal nDefault As Integer, _
        ByVal lpFileName As String) As Integer
    End Function

    <DllImport("KERNEL32.DLL")> _
    Friend Function WritePrivateProfileString( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, _
        ByVal lpString As String, _
        ByVal lpFileName As String) As Integer
    End Function

    ' ���ׂẴZ�N�V�����̖��O���擾���܂��B
    Friend Function GetPrivateProfileSectionNames( _
       ByVal lpFileName As String) As String()
        Dim str As String = New String(" ", 10240)
        Dim len As Integer = 10240
        Dim nRet As Integer = GetIniSectionNames(str, len, lpFileName)

        Return str.Split(Microsoft.VisualBasic.Chr(0))
    End Function

    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileSectionNames", CharSet:=CharSet.Auto)> _
    Friend Function GetIniSectionNames( _
            ByVal lpszReturnBuffer As String, _
            ByVal nSize As Integer, _
            ByVal lpFileName As String) As Integer
    End Function

    ' �w�肳�ꂽ�Z�N�V�������̂��ׂẴL�[�ƒl���擾���܂��B
    Friend Function GetPrivateProfileSection( _
        ByVal lpAppName As String, _
        ByVal lpFileName As String) As String()
        Dim str As String = New String(" ", 10240)
        Dim len As Integer = 10240
        Dim nRet As Integer = GetIniSection(lpAppName, str, len, lpFileName)

        Return str.Split(Microsoft.VisualBasic.Chr(0))
    End Function

    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileSection", CharSet:=CharSet.Auto)> _
    Friend Function GetIniSection( _
            ByVal lpAppName As String, _
            ByVal lpszReturnBuffer As String, _
            ByVal nSize As Integer, _
            ByVal lpFileName As String) As Integer
    End Function

    '*** Str Add 2015/12/01 SO)�r�� for �ȈՒ��[��� ***
    '
    ' �@�\�@ �F �w��ini�t�@�C������w��L�[�̒l�ꗗ���擾����
    '
    ' �����@ �F inifile  - ini�t�@�C���t���p�X��
    ' �@�@�@ �@ secssion - �Z�N�V������
    ' �@�@�@ �@ key      - �L�[��
    '
    ' �߂�l �F �l�ꗗ
    '           ini�t�@�C������擾���s���́ANothing���Ԃ�
    '
    Public Function GetIniFileValues(ByVal inifile As String, ByVal secssion As String, ByVal key As String) As String()

        Dim strResultList As New ArrayList()
        Dim strBuffer As String = New String(ControlChars.NullChar, 10240)

        ' ini�t�@�C���Ǎ��݁i�s�v�ȋ󔒁A�^�u�͎�菜�����j
        Dim rc As Integer = GetIniSection(secssion, strBuffer, strBuffer.Length, inifile)
        If rc = 0 Then
            Return Nothing
        End If

        Dim strData As String = strBuffer.TrimEnd(ControlChars.NullChar)
        Dim strSplits As String() = strData.Split(ControlChars.NullChar)
        For i As Integer = 0 To strSplits.Length - 1
            ' strSplits(i)�́A"key=value" �`���ƂȂ�
            Dim pos As Integer = strSplits(i).IndexOf("=")
            If pos > 0 AndAlso strSplits(i).Substring(0, pos).ToUpper = key.ToUpper Then
                strResultList.Add(strSplits(i).Substring(pos + 1))
            End If
        Next

        If strResultList.Count = 0 Then
            Return Nothing
        End If

        Return  DirectCast(strResultList.ToArray(GetType(String)), String())

    End Function

    '
    ' �@�\�@ �F �w��ini�t�@�C������w��L�[�̒l�ꗗ���擾����
    '
    ' �����@ �F inifile  - ini�t�@�C���t���p�X��
    ' �@�@�@ �@ secssion - �Z�N�V������
    ' �@�@�@ �@ key      - �L�[��
    '
    ' �߂�l �F �l
    '           ini�t�@�C������擾���s���́A"err"���Ԃ�
    '
    Public Function GetIniFileValue(ByVal inifile As String, ByVal secssion As String, ByVal key As String) As String

        Dim Value As String = ""
        Dim sIDefault As String = "err"

        Call GetPrivateProfileString(secssion, key, sIDefault, Value, 1024, inifile)

        Return Value

    End Function
    '*** End Add 2015/12/01 SO)�r�� for �ȈՒ��[��� ***


    '
    ' �@�\�@ �F �e�r�j�i�D�h�m�h����l���擾����
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    '
    ' �߂�l �F �l
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function GetFSKJIni(ByVal aAppName As String, ByVal aKeyName As String) As String
        Dim Value As String = ""

        Call GetFSKJIni(aAppName, aKeyName, Value)

        Return Value
    End Function

    ' �@�\�@ �F �e�r�j�i�D�h�m�h����l���擾����
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    '
    ' �߂�l �F �l
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function GetFSKJIniNum(ByVal aAppName As String, ByVal aKeyName As String) As Long
        Return CAInt32(GetFSKJIni(aAppName, aKeyName))
    End Function

    '
    ' �@�\�@ �F �e�r�j�i�D�h�m�h����l���擾����
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    ' �@�@�@ �@ ARG3 - �l�i�Q�Ɠn���j
    '
    ' �߂�l �F �擾������̒���
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function GetFSKJIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = sIFileName & "\FSKJ.INI"
        Dim sIDefault As String = "err"

        Dim nRet As Integer = GetPrivateProfileString(aAppName, aKeyName, sIDefault, aValue, 1024, sIFileName)

        If aValue = "err" AndAlso aAppName <> "COMMON" AndAlso aAppName <> "GCOMMON" AndAlso aKeyName <> "LOG" Then
            '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("INI�t�@�C���擾", "Main")
            Dim LOG As New BatchLOG("INI�t�@�C���擾")
            '*** Str Upd 2015/12/01 SO)�r�� for ���O���� ***
            'LOG.Write("INI�t�@�C���擾", "���s", "���s�L�[�F" & aAppName & "-" & aKeyName)
            LOG.Write_Err("INI�t�@�C���擾", "���s", "���s�L�[�F" & aAppName & "-" & aKeyName)
            '*** End Upd 2015/12/01 SO)�r�� for ���O���� ***
            LOG = Nothing
            '*** Str Del 2015/12/01 SO)�r�� for ���O���� ***
            'Call PutLogTrace()
            '*** End Del 2015/12/01 SO)�r�� for ���O���� ***
        End If

        Return nRet
    End Function

    ' �@�\�@ �F �e�r�j�i�D�h�m�h����,�w�肳�ꂽ�Z�N�V�������̂��ׂẴL�[�ƒl���擾���܂��B

    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    ' �@�@�@ �@ ARG3 - �l�i�Q�Ɠn���j
    '
    ' �߂�l �F �擾������̒���
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function GetFSKJIniKeys(ByVal aAppName As String) As String()
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = sIFileName & "\FSKJ.INI"
        Dim sIDefault As String = "err"

        Dim Keys() As String = GetPrivateProfileSection(aAppName, sIFileName)
        Dim Arr As New ArrayList
        For i As Integer = 0 To Keys.Length - 1
            If Keys(i).ToString.Trim <> "" Then
                Arr.Add(Keys(i).ToString)
            End If
        Next i

        Return CType(Arr.ToArray(GetType(String)), String())
    End Function


    '
    ' �@�\�@ �F �e�r�j�i�D�h�m�h����l���擾����
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    ' �@�@�@ �@ ARG3 - �l�i�Q�Ɠn���j
    '
    ' �߂�l �F �擾������̒���
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function GetIni(ByVal INI As String, ByVal aAppName As String, ByVal aKeyName As String) As String
        Dim Value As String = ""
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = System.IO.Path.Combine(sIFileName, INI)
        Dim sIDefault As String = "err"

        Call GetPrivateProfileString(aAppName, aKeyName, sIDefault, Value, 1024, sIFileName)

        Return Value
    End Function

    '
    ' �@�\�@ �F �e�r�j�i�D�h�m�h�ɒl����������
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    ' �@�@�@ �@ ARG3 - �l�i�Q�Ɠn���j
    '
    ' �߂�l �F �擾������̒���
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function PutFSKJIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = sIFileName & "\FSKJ.INI"

        Return WritePrivateProfileString(aAppName, aKeyName, aValue, sIFileName)
    End Function

    '*** Str Del 2015/12/01 SO)�r�� for ���O���� ***
    'Private Sub PutLogTrace()
    '    Dim strace As New StackTrace(True)
    '    Dim count As Integer
    '    Dim Msg As New System.Text.StringBuilder(128)
	'
    '    ' High up the call stack, there is only one stack frame
    '    While count < strace.FrameCount
    '        Dim frame As New StackFrame
    '        frame = strace.GetFrame(count)
    '        Msg.Append(" : ")
    '        Msg.Append(frame.GetMethod())
    '        Msg.Append(" Line:")
    '        Msg.Append(frame.GetFileLineNumber().ToString)
    '        count += 1
    '    End While
    '    '2009.08.27 �R���p�C����ʂ����߃R�����gDim LOG As New BatchLOG("�����g���[�X���O", "Main")
    '    Dim LOG As New BatchLOG("�����g���[�X���O")
    '    LOG.Write("���O", "�g���[�X", Msg.ToString)
    'End Sub
    '*** End Del 2015/12/01 SO)�r�� for ���O���� ***

    ' 2016/10/07 �^�X�N�j���� ADD �yME�zME-99-99(RSV2�@�\�g��) -------------------- START
    Public Function GetRSKJIni(ByVal aAppName As String, ByVal aKeyName As String) As String
        Dim Value As String = ""

        Call GetRSKJIni(aAppName, aKeyName, Value)

        Return Value
    End Function

    Public Function GetRSKJIniNum(ByVal aAppName As String, ByVal aKeyName As String) As Long
        Return CAInt32(GetRSKJIni(aAppName, aKeyName))
    End Function

    Public Function GetRSKJIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = sIFileName & "\RSKJ.INI"
        Dim sIDefault As String = "err"

        Dim nRet As Integer = GetPrivateProfileString(aAppName, aKeyName, sIDefault, aValue, 1024, sIFileName)

        If aValue = "err" AndAlso aAppName <> "COMMON" AndAlso aAppName <> "GCOMMON" AndAlso aKeyName <> "LOG" Then
            Dim LOG As New BatchLOG("INI�t�@�C���擾")
            LOG.Write_Err("INI�t�@�C���擾", "���s", "���s�L�[�F" & aAppName & "-" & aKeyName)
            LOG = Nothing
        End If

        Return nRet
    End Function

    Public Function GetRSKJIniKeys(ByVal aAppName As String) As String()
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = sIFileName & "\RSKJ.INI"
        Dim sIDefault As String = "err"

        Dim Keys() As String = GetPrivateProfileSection(aAppName, sIFileName)
        Dim Arr As New ArrayList
        For i As Integer = 0 To Keys.Length - 1
            If Keys(i).ToString.Trim <> "" Then
                Arr.Add(Keys(i).ToString)
            End If
        Next i

        Return CType(Arr.ToArray(GetType(String)), String())
    End Function

    Public Function PutRSKJIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = sIFileName & "\RSKJ.INI"

        Return WritePrivateProfileString(aAppName, aKeyName, aValue, sIFileName)
    End Function
    ' 2016/10/07 �^�X�N�j���� ADD �yME�zME-99-99(RSV2�@�\�g��) -------------------- END
End Module
