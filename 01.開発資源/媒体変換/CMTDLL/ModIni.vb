Imports System
Imports System.Runtime.InteropServices

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

    '
    ' �@�\�@ �F �b�l�s�D�h�m�h����l���擾����
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    '
    ' �߂�l �F �l
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function GetCMTIni(ByVal aAppName As String, ByVal aKeyName As String) As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ---------------------------->>>>
        Dim Value As String = String.Empty
        'Dim Value As String
        '2012/01/13 saitou �W���C�� �x����� MODIFY ----------------------------<<<<

        Call GetCMTIni(aAppName, aKeyName, Value)

        Return Value
    End Function

    '
    ' �@�\�@ �F �b�l�s�D�h�m�h����l���擾����
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    ' �@�@�@ �@ ARG3 - �l�i�Q�Ɠn���j
    '
    ' �߂�l �F �擾������̒���
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function GetCMTIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
    	'*** ASTAR.S.S 2008.05.26 EXE�̏ꏊ����INI�t�@�C�����擾����		***
        'Dim sIFileName As String = CurDir()       '�J�����g�f�B���N�g���̎擾
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath         '�J�����g�f�B���N�g���̎擾
    	'**********************************************************************
        sIFileName = sIFileName & "\CMT.INI"
        Dim sIDefault As String = "err"

        Return GetPrivateProfileString(aAppName, aKeyName, sIDefault, aValue, 1024, sIFileName)
    End Function

    '
    ' �@�\�@ �F �b�l�s�D�h�m�h�ɒl����������
    '
    ' �����@ �F ARG1 - �Z�N�V������
    ' �@�@�@ �@ ARG2 - �L�[��
    ' �@�@�@ �@ ARG3 - �l�i�Q�Ɠn���j
    '
    ' �߂�l �F �擾������̒���
    '
    ' ���l�@ �F�h�m�h�t�@�C���擾���s���́C�l��"err"������
    '
    Public Function PutCMTIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        '�J�����g�f�B���N�g���̎擾
        sIFileName = sIFileName & "\CMT.INI"

        Return WritePrivateProfileString(aAppName, aKeyName, aValue, sIFileName)
    End Function

End Module
