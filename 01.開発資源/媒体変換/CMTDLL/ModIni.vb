Imports System
Imports System.Runtime.InteropServices

' INIファイル処理 モジュール
Public Module ModIni
    ' INIファイル 情報取得
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

    ' INIファイル 情報取得
    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileString", CharSet:=CharSet.Auto)> _
    Friend Function GetIniString( _
        ByVal lpAppName As String, _
        ByVal lpKeyName As String, ByVal lpDefault As String, _
        ByVal lpReturnedString As System.Text.StringBuilder, ByVal nSize As Integer, _
        ByVal lpFileName As String) As Integer
    End Function

    ' INIファイル 情報取得
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
    ' 機能　 ： ＣＭＴ．ＩＮＩから値を取得する
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    '
    ' 戻り値 ： 値
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function GetCMTIni(ByVal aAppName As String, ByVal aKeyName As String) As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim Value As String = String.Empty
        'Dim Value As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Call GetCMTIni(aAppName, aKeyName, Value)

        Return Value
    End Function

    '
    ' 機能　 ： ＣＭＴ．ＩＮＩから値を取得する
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    ' 　　　 　 ARG3 - 値（参照渡し）
    '
    ' 戻り値 ： 取得文字列の長さ
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function GetCMTIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
    	'*** ASTAR.S.S 2008.05.26 EXEの場所からINIファイルを取得する		***
        'Dim sIFileName As String = CurDir()       'カレントディレクトリの取得
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath         'カレントディレクトリの取得
    	'**********************************************************************
        sIFileName = sIFileName & "\CMT.INI"
        Dim sIDefault As String = "err"

        Return GetPrivateProfileString(aAppName, aKeyName, sIDefault, aValue, 1024, sIFileName)
    End Function

    '
    ' 機能　 ： ＣＭＴ．ＩＮＩに値を書き込む
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    ' 　　　 　 ARG3 - 値（参照渡し）
    '
    ' 戻り値 ： 取得文字列の長さ
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function PutCMTIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
        sIFileName = sIFileName & "\CMT.INI"

        Return WritePrivateProfileString(aAppName, aKeyName, aValue, sIFileName)
    End Function

End Module
