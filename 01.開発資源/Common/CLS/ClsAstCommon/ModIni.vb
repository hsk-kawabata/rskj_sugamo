Imports System
Imports System.Runtime.InteropServices
Imports System.Diagnostics
Imports System.Collections
'*** Str Add 2015/12/01 SO)荒木 for 簡易帳票印刷 ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)荒木 for 簡易帳票印刷 ***

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

    ' すべてのセクションの名前を取得します。
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

    ' 指定されたセクション内のすべてのキーと値を取得します。
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

    '*** Str Add 2015/12/01 SO)荒木 for 簡易帳票印刷 ***
    '
    ' 機能　 ： 指定iniファイルから指定キーの値一覧を取得する
    '
    ' 引数　 ： inifile  - iniファイルフルパス名
    ' 　　　 　 secssion - セクション名
    ' 　　　 　 key      - キー名
    '
    ' 戻り値 ： 値一覧
    '           iniファイルから取得失敗時は、Nothingが返る
    '
    Public Function GetIniFileValues(ByVal inifile As String, ByVal secssion As String, ByVal key As String) As String()

        Dim strResultList As New ArrayList()
        Dim strBuffer As String = New String(ControlChars.NullChar, 10240)

        ' iniファイル読込み（不要な空白、タブは取り除かれる）
        Dim rc As Integer = GetIniSection(secssion, strBuffer, strBuffer.Length, inifile)
        If rc = 0 Then
            Return Nothing
        End If

        Dim strData As String = strBuffer.TrimEnd(ControlChars.NullChar)
        Dim strSplits As String() = strData.Split(ControlChars.NullChar)
        For i As Integer = 0 To strSplits.Length - 1
            ' strSplits(i)は、"key=value" 形式となる
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
    ' 機能　 ： 指定iniファイルから指定キーの値一覧を取得する
    '
    ' 引数　 ： inifile  - iniファイルフルパス名
    ' 　　　 　 secssion - セクション名
    ' 　　　 　 key      - キー名
    '
    ' 戻り値 ： 値
    '           iniファイルから取得失敗時は、"err"が返る
    '
    Public Function GetIniFileValue(ByVal inifile As String, ByVal secssion As String, ByVal key As String) As String

        Dim Value As String = ""
        Dim sIDefault As String = "err"

        Call GetPrivateProfileString(secssion, key, sIDefault, Value, 1024, inifile)

        Return Value

    End Function
    '*** End Add 2015/12/01 SO)荒木 for 簡易帳票印刷 ***


    '
    ' 機能　 ： ＦＳＫＪ．ＩＮＩから値を取得する
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    '
    ' 戻り値 ： 値
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function GetFSKJIni(ByVal aAppName As String, ByVal aKeyName As String) As String
        Dim Value As String = ""

        Call GetFSKJIni(aAppName, aKeyName, Value)

        Return Value
    End Function

    ' 機能　 ： ＦＳＫＪ．ＩＮＩから値を取得する
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    '
    ' 戻り値 ： 値
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function GetFSKJIniNum(ByVal aAppName As String, ByVal aKeyName As String) As Long
        Return CAInt32(GetFSKJIni(aAppName, aKeyName))
    End Function

    '
    ' 機能　 ： ＦＳＫＪ．ＩＮＩから値を取得する
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    ' 　　　 　 ARG3 - 値（参照渡し）
    '
    ' 戻り値 ： 取得文字列の長さ
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function GetFSKJIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
        sIFileName = sIFileName & "\FSKJ.INI"
        Dim sIDefault As String = "err"

        Dim nRet As Integer = GetPrivateProfileString(aAppName, aKeyName, sIDefault, aValue, 1024, sIFileName)

        If aValue = "err" AndAlso aAppName <> "COMMON" AndAlso aAppName <> "GCOMMON" AndAlso aKeyName <> "LOG" Then
            '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("INIファイル取得", "Main")
            Dim LOG As New BatchLOG("INIファイル取得")
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'LOG.Write("INIファイル取得", "失敗", "失敗キー：" & aAppName & "-" & aKeyName)
            LOG.Write_Err("INIファイル取得", "失敗", "失敗キー：" & aAppName & "-" & aKeyName)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            LOG = Nothing
            '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
            'Call PutLogTrace()
            '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***
        End If

        Return nRet
    End Function

    ' 機能　 ： ＦＳＫＪ．ＩＮＩから,指定されたセクション内のすべてのキーと値を取得します。

    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    ' 　　　 　 ARG3 - 値（参照渡し）
    '
    ' 戻り値 ： 取得文字列の長さ
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function GetFSKJIniKeys(ByVal aAppName As String) As String()
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
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
    ' 機能　 ： ＦＳＫＪ．ＩＮＩから値を取得する
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    ' 　　　 　 ARG3 - 値（参照渡し）
    '
    ' 戻り値 ： 取得文字列の長さ
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function GetIni(ByVal INI As String, ByVal aAppName As String, ByVal aKeyName As String) As String
        Dim Value As String = ""
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
        sIFileName = System.IO.Path.Combine(sIFileName, INI)
        Dim sIDefault As String = "err"

        Call GetPrivateProfileString(aAppName, aKeyName, sIDefault, Value, 1024, sIFileName)

        Return Value
    End Function

    '
    ' 機能　 ： ＦＳＫＪ．ＩＮＩに値を書き込む
    '
    ' 引数　 ： ARG1 - セクション名
    ' 　　　 　 ARG2 - キー名
    ' 　　　 　 ARG3 - 値（参照渡し）
    '
    ' 戻り値 ： 取得文字列の長さ
    '
    ' 備考　 ：ＩＮＩファイル取得失敗時は，値に"err"が入る
    '
    Public Function PutFSKJIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
        sIFileName = sIFileName & "\FSKJ.INI"

        Return WritePrivateProfileString(aAppName, aKeyName, aValue, sIFileName)
    End Function

    '*** Str Del 2015/12/01 SO)荒木 for ログ強化 ***
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
    '    '2009.08.27 コンパイルを通すためコメントDim LOG As New BatchLOG("処理トレースログ", "Main")
    '    Dim LOG As New BatchLOG("処理トレースログ")
    '    LOG.Write("ログ", "トレース", Msg.ToString)
    'End Sub
    '*** End Del 2015/12/01 SO)荒木 for ログ強化 ***

    ' 2016/10/07 タスク）綾部 ADD 【ME】ME-99-99(RSV2機能拡張) -------------------- START
    Public Function GetRSKJIni(ByVal aAppName As String, ByVal aKeyName As String) As String
        Dim Value As String = ""

        Call GetRSKJIni(aAppName, aKeyName, Value)

        Return Value
    End Function

    Public Function GetRSKJIniNum(ByVal aAppName As String, ByVal aKeyName As String) As Long
        Return CAInt32(GetRSKJIni(aAppName, aKeyName))
    End Function

    Public Function GetRSKJIni(ByVal aAppName As String, ByVal aKeyName As String, ByRef aValue As String) As Integer
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
        sIFileName = sIFileName & "\RSKJ.INI"
        Dim sIDefault As String = "err"

        Dim nRet As Integer = GetPrivateProfileString(aAppName, aKeyName, sIDefault, aValue, 1024, sIFileName)

        If aValue = "err" AndAlso aAppName <> "COMMON" AndAlso aAppName <> "GCOMMON" AndAlso aKeyName <> "LOG" Then
            Dim LOG As New BatchLOG("INIファイル取得")
            LOG.Write_Err("INIファイル取得", "失敗", "失敗キー：" & aAppName & "-" & aKeyName)
            LOG = Nothing
        End If

        Return nRet
    End Function

    Public Function GetRSKJIniKeys(ByVal aAppName As String) As String()
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
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
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
        sIFileName = sIFileName & "\RSKJ.INI"

        Return WritePrivateProfileString(aAppName, aKeyName, aValue, sIFileName)
    End Function
    ' 2016/10/07 タスク）綾部 ADD 【ME】ME-99-99(RSV2機能拡張) -------------------- END
End Module
