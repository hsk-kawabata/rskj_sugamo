Imports System.Runtime.InteropServices

''' <summary>
''' ＣＭＴ操作モジュール（旧FSKJDLL）
''' </summary>
''' <remarks>
''' 2018/02/16 saitou 広島信金(RSV2標準) added for サーバー処理対応(64ビット対応)
''' ・VB6時代のソースを解析し、VB.NETにマイグレーションする。</remarks>
Module ModFSKJ

    'ＭＴアクセス用関数定義（ＪＰＣ社ＤＬＬ用）

    '装置の初期化
    Public Declare Function mtinit Lib "MTDLL53.DLL" (ByRef Mt_info As MtINFO) As Long
    '装置のステータス読み取り
    Public Declare Function mtstat Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'ＢＯＴまで巻き戻し
    Public Declare Function mtrewind Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    '１ブロック読み込み
    Public Declare Function mtrblock Lib "MTDLL53.DLL" (ByVal MtId As Integer, ByVal buff As String, count As Integer, ByVal bufflen As Long) As Long
    '１ブロック書き込み
    Public Declare Function mtwblock Lib "MTDLL53.DLL" (ByVal MtId As Integer, ByVal buff As String, ByVal Blksize As Integer) As Long
    '１ブロック前進
    Public Declare Function mtfblock Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    '１ブロック後退
    Public Declare Function mtbblock Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'テープマークを検出するまで前進
    Public Declare Function mtffile Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'テープマークを検出するまで後退
    Public Declare Function mtbfile Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'テープマーク１個ライト
    Public Declare Function mtwtmk Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'テープマーク２個ライト
    Public Declare Function mtwmtmk Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'マルチテープマークの検出
    Public Declare Function mtsmtmk Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'テープのアンロード
    Public Declare Function mtunload Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    '装置のオンライン
    Public Declare Function mtonline Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    'ＥＢＣＤＩＣコード変換
    Public Declare Function mtebc Lib "MTDLL53.DLL" (ByVal CodeNo As Integer) As Long
    'ＭＴのイレーズ
    Public Declare Function mters Lib "MTDLL53.DLL" (ByVal MtId As Integer) As Long
    '記録密度の設定
    Public Declare Function mtdensity Lib "MTDLL53.DLL" (ByVal MtId As Integer, ByVal Code As Integer) As Long

    'ＤＬＬ内でＪＰＣ関数を使用するときの引数
    Public mt_mtid As Integer                   'unsigned char  mtid     : MT番号
    Public mt_buffer As String                  'const char     *buff    : ﾒﾓﾘｱﾄﾞﾚｽ
    Public mt_bufflen As Long                   'unsigned long  bufflen  : ﾃﾞｰﾀﾊﾞｯﾌｧｻｲｽﾞ
    Public mt_count As Integer                  'unsigned short *count   : ﾘｰﾄﾞﾊﾞｲﾄ数
    Public mt_Blksize As Integer                'unsigned short blklen   : ﾌﾞﾛｯｸ長
    Public mt_codeno As Integer                 'unsigned char  codeno   : ｺｰﾄﾞ番号
    Public mt_rtn As Long                       'unsigned long  status   : ﾃﾞﾊﾞｲｽｽﾃｰﾀｽ
    Public Const MT_LABEL_SIZE As Long = 80

    Public Structure MtINFO
        Dim UnitNo As Byte
        Dim HostNo As Byte
        Dim TargetNo As Byte
        Dim Vender() As Byte
        Dim Product() As Byte
        Dim Version() As Byte
        Dim Reserve As Byte
    End Structure
    Public T_Mtinfo(8) As MtINFO

    'デバイスステータス
    Public Structure DEVICE_status
        Dim BIT_TMK As Byte
        Dim BIT_EOT As Byte
        Dim BIT_BOT As Byte
        Dim BIT_DEN0 As Byte
        Dim BIT_DEN1 As Byte
        Dim BIT_FIL1 As Byte
        Dim BIT_PRO As Byte
        Dim BIT_FIL2 As Byte
        Dim BIT_DTE As Byte
        Dim BIT_HDE As Byte
        Dim BIT_NRDY As Byte
        Dim BIT_ILC As Byte
        Dim BIT_SCE As Byte
        Dim BIT_UDC As Byte
        Dim BIT_FIL3 As Byte
        Dim BIT_CHG As Byte
    End Structure

#Region "INIファイル処理"
    Friend Function GetPrivateProfileString(ByVal lpAppName As String, _
                                            ByVal lpKeyName As String, _
                                            ByVal lpDefault As String, _
                                            ByRef lpReturnedString As String, _
                                            ByVal nSize As Integer, _
                                            ByVal lpFileName As String) As Integer
        Dim str As New System.Text.StringBuilder(1024)
        Dim nRet As Integer = GetIniString(lpAppName, lpKeyName, lpDefault, str, str.Capacity, lpFileName)
        lpReturnedString = str.ToString
        Return nRet
    End Function

    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileString", CharSet:=CharSet.Auto)> _
    Friend Function GetIniString(ByVal lpAppName As String, _
                                 ByVal lpKeyName As String, _
                                 ByVal lpDefault As String, _
                                 ByVal lpReturnedString As System.Text.StringBuilder, _
                                 ByVal nSize As Integer, _
                                 ByVal lpFileName As String) As Integer
    End Function

    <DllImport("KERNEL32.DLL", CharSet:=CharSet.Auto)> _
    Friend Function GetPrivateProfileInt(ByVal lpAppName As String, _
                                         ByVal lpKeyName As String, _
                                         ByVal nDefault As Integer, _
                                         ByVal lpFileName As String) As Integer
    End Function

    <DllImport("KERNEL32.DLL")> _
    Friend Function WritePrivateProfileString(ByVal lpAppName As String, _
                                              ByVal lpKeyName As String, _
                                              ByVal lpString As String, _
                                              ByVal lpFileName As String) As Integer
    End Function

    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileSectionNames", CharSet:=CharSet.Auto)> _
    Friend Function GetIniSectionNames(ByVal lpszReturnBuffer As String, _
                                       ByVal nSize As Integer, _
                                       ByVal lpFileName As String) As Integer
    End Function

    <DllImport("KERNEL32.DLL", EntryPoint:="GetPrivateProfileSection", CharSet:=CharSet.Auto)> _
    Friend Function GetIniSection(ByVal lpAppName As String, _
                                  ByVal lpszReturnBuffer As String, _
                                  ByVal nSize As Integer, _
                                  ByVal lpFileName As String) As Integer
    End Function

    Public Function GetIni(ByVal INI As String, _
                           ByVal aAppName As String, _
                           ByVal aKeyName As String) As String
        Dim Value As String = ""
        Dim sIFileName As String = System.Windows.Forms.Application.StartupPath        'カレントディレクトリの取得
        sIFileName = System.IO.Path.Combine(sIFileName, INI)
        Dim sIDefault As String = "err"

        Call GetPrivateProfileString(aAppName, aKeyName, sIDefault, Value, 1024, sIFileName)

        Return Value
    End Function
#End Region

End Module
