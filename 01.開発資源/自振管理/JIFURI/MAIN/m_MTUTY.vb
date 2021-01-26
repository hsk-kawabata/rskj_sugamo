Imports System
Imports System.Runtime.InteropServices
Module m_MTUTY
    '-------------------------------------------------------------------------------------------
    '----- ＭＴアクセス用関数定義（ＪＰＣ社ＤＬＬ用）----------------------------------------------
    '-------------------------------------------------------------------------------------------

    ' 新モジュール用定義
    Public Declare Function mtinit Lib "MTDLL53.DLL" _
           (ByRef Mt_info As Integer) As Long
    '(ByRef Mt_info As Any) As Long
    '装置のステータス読み取り
    Public Declare Function mtstat Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'ＢＯＴまで巻き戻し
    Public Declare Function mtrewind Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '１ブロック読み込み
    'Public Declare Function mtrblock Lib "MTDLL53.DLL" _
    '(ByVal MtId As Integer, ByVal buff As String, ByVal count As Integer, ByVal bufflen As Long) As Long
    Public Declare Function mtrblock Lib "MTDLL53.DLL" _
       (ByVal MtId As Integer, ByRef buff As String, ByRef count As Integer, ByRef bufflen As Long) As Long
    '１ブロック書き込み
    Public Declare Function mtwblock Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer, ByVal buff As String, ByVal blklen As Integer) As Long
    '１ブロック前進
    Public Declare Function mtfblock Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '１ブロック後退
    Public Declare Function mtbblock Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'テープマークを検出するまで前進
    Public Declare Function mtffile Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'テープマークを検出するまで後退
    Public Declare Function mtbfile Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'テープマーク１個ライト
    Public Declare Function mtwtmk Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'テープマーク２個ライト
    Public Declare Function mtwmtmk Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'マルチテープマークの検出
    Public Declare Function mtsmtmk Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'テープのアンロード
    Public Declare Function mtunload Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '装置のオンライン
    Public Declare Function mtonline Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    'ＥＢＣＤＩＣコード変換
    Public Declare Function mtebc Lib "MTDLL53.DLL" _
        (ByVal codeno As Integer) As Long
    'ＭＴのイレーズ
    Public Declare Function mters Lib "MTDLL53.DLL" _
        (ByVal MtId As Integer) As Long
    '----------------------------------------------------------------------------------------
    '----------------------------------------------------------------------------------------
    'ＤＬＬ内でＪＰＣ関数を使用するときの引数
    Public mt_mtid As Integer
    <VBFixedString(32000)> Public mt_buffer As String
    Public mt_bufflen As Long
    Public mt_count As Integer
    Public mt_blklen As Integer
    Public mt_codeno As Integer
    Public mt_rtn As Long

    Structure MtINFO
        Public UnitNo As Byte
        Public HostNo As Byte
        Public TargetNo As Byte
        'Public Vender(0 To 7)   As Byte
        'Public Product(0 To 15) As Byte
        'Public Version(0 To 3)  As Byte
        Public Vender1 As Byte
        Public Vender2 As Byte
        Public Vender3 As Byte
        Public Vender4 As Byte
        Public Vender5 As Byte
        Public Vender6 As Byte
        Public Vender7 As Byte
        Public Vender8 As Byte
        Public Product1 As Byte
        Public Product2 As Byte
        Public Product3 As Byte
        Public Product4 As Byte
        Public Product5 As Byte
        Public Product6 As Byte
        Public Product7 As Byte
        Public Product8 As Byte
        Public Product9 As Byte
        Public Product10 As Byte
        Public Product11 As Byte
        Public Product12 As Byte
        Public Product13 As Byte
        Public Product14 As Byte
        Public Product15 As Byte
        Public Product16 As Byte
        Public Version1 As Byte
        Public Version2 As Byte
        Public Version3 As Byte
        Public Version4 As Byte
        '<VBFixedArray(7), MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> Public Vender() As Byte
        '<VBFixedArray(15), MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> Public Product() As Byte
        '<VBFixedArray(3), MarshalAs(UnmanagedType.ByValArray, SizeConst:=1)> Public Version() As Byte
        Public Reserve As Byte
    End Structure
    'Public T_Mtinfo(1 To 8) As MtINFO
    Public T_Mtinfo() As MtINFO
    '<VBFixedArray(7), MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)> Public T_Mtinfo As MtINFO
    'スタンダードラベル定義
    '--------------------------------------
    ' 初期化時 BOT + VOL1 + HDR1 + TM + TM
    '--------------------------------------
    'ボリュームラベル（VOL1)
    <VBFixedString(3)> Public Const VOL1_ラベル識別名 As String = "VOL"
    <VBFixedString(1)> Public Const VOL1_ラベル番号 As String = "1"
    <VBFixedString(6)> Public VOL1_ボリューム識別名 As String
    'Public Const VOL1_アクセス条件      As String * 1 = " "
    <VBFixedString(31)> Public Const 予備1 As String = " "
    <VBFixedString(10)> Public VOL1_所有者識別 As String
    <VBFixedString(29)> Public VOL1_予備2 As String
    'ファイルラベル（HDR1)
    <VBFixedString(3)> Public Const HDR1_ラベル識別名 As String = "HDR"
    <VBFixedString(1)> Public Const HDR1_ラベル番号 As String = "1"
    <VBFixedString(17)> Public HDR1_ファイル識別名 As String
    <VBFixedString(6)> Public HDR1_ファイルセット識別名 As String
    <VBFixedString(4)> Public Const HDR1_ファイル分割番号 As String = "0001"
    <VBFixedString(4)> Public Const HDR1_ファイル順序番号 As String = "0001"
    <VBFixedString(4)> Public Const HDR1_世代番号 As String = "0001"
    <VBFixedString(2)> Public Const HDR1_世代更新番号 As String = "00"
    <VBFixedString(6)> Public Const HDR1_作成日付 As String = " 00000"
    <VBFixedString(6)> Public Const HDR1_満了日付 As String = " 00000"
    <VBFixedString(1)> Public Const HDR1_アクセス条件 As String = "0"
    <VBFixedString(6)> Public Const HDR1_ブロック数 As String = "000000"
    <VBFixedString(13)> Public Const HDR1_システム識別名 As String = " "
    <VBFixedString(7)> Public Const HDR1_予備1 As String = " "
    'ファイルラベル（HDR2)
    <VBFixedString(3)> Public Const HDR2_ラベル識別名 As String = "HDR"
    <VBFixedString(1)> Public Const HDR2_ラベル番号 As String = "2"
    <VBFixedString(1)> Public Const HDR2_レコード番号 As String = "F"        '固定長
    <VBFixedString(5)> Public HDR2_ブロック長 As String
    <VBFixedString(5)> Public HDR2_レコード長 As String
    <VBFixedString(1)> Public Const HDR2_記録密度 As String = " "
    <VBFixedString(1)> Public Const HDR2_ボリューム状態 As String = "0"      '単ボリューム
    <VBFixedString(21)> Public Const HDR2_予備1 As String = " "
    <VBFixedString(1)> Public HDR2_ブロック属性 As String
    <VBFixedString(41)> Public Const HDR2_予備2 As String = " "
    'ファイルラベル（EOF1)
    <VBFixedString(3)> Public Const EOF1_ラベル識別名 As String = "EOF"
    <VBFixedString(1)> Public Const EOF1_ラベル番号 As String = "1"
    <VBFixedString(17)> Public EOF1_ファイル識別名 As String
    <VBFixedString(6)> Public EOF1_ファイルセット識別名 As String
    <VBFixedString(4)> Public Const EOF1_ファイル分割番号 As String = "0001"
    <VBFixedString(4)> Public Const EOF1_ファイル順序番号 As String = "0001"
    <VBFixedString(4)> Public Const EOF1_世代番号 As String = "0001"
    <VBFixedString(2)> Public Const EOF1_世代更新番号 As String = "00"
    <VBFixedString(6)> Public Const EOF1_作成日付 As String = " 00000"
    <VBFixedString(6)> Public Const EOF1_満了日付 As String = " 00000"
    <VBFixedString(1)> Public Const EOF1_アクセス条件 As String = "0"
    <VBFixedString(6)> Public Const EOF1_ブロック数 As String = "000000"
    <VBFixedString(13)> Public Const EOF1_システム識別名 As String = " "
    <VBFixedString(7)> Public Const EOF1_予備1 As String = " "
    'ファイルラベル（EOF2)
    <VBFixedString(3)> Public Const EOF2_ラベル識別名 As String = "EOF"
    <VBFixedString(1)> Public Const EOF2_ラベル番号 As String = "2"
    <VBFixedString(1)> Public Const EOF2_レコード番号 As String = "F"        '固定長
    <VBFixedString(5)> Public EOF2_ブロック長 As String
    <VBFixedString(5)> Public EOF2_レコード長 As String
    <VBFixedString(1)> Public Const EOF2_記録密度 As String = " "
    <VBFixedString(1)> Public Const EOF2_ボリューム状態 As String = "0"     '単ボリューム
    <VBFixedString(21)> Public Const EOF2_予備1 As String = " "
    <VBFixedString(1)> Public EOF2_ブロック属性 As String
    <VBFixedString(41)> Public Const EOF2_予備2 As String = " "
    'ファイルラベル（EOV1)
    <VBFixedString(3)> Public Const EOV1_ラベル識別名 As String = "EOV"
    <VBFixedString(1)> Public Const EOV1_ラベル番号 As String = "1"
    <VBFixedString(17)> Public EOV1_ファイル識別名 As String
    <VBFixedString(6)> Public EOV1_ファイルセット識別名 As String
    <VBFixedString(4)> Public Const EOV1_ファイル分割番号 As String = "0001"
    <VBFixedString(4)> Public Const EOV1_ファイル順序番号 As String = "0001"
    <VBFixedString(4)> Public Const EOV1_世代番号 As String = "0001"
    <VBFixedString(2)> Public Const EOV1_世代更新番号 As String = "00"
    <VBFixedString(6)> Public Const EOV1_作成日付 As String = " 00000"
    <VBFixedString(6)> Public Const EOV1_満了日付 As String = " 00000"
    <VBFixedString(1)> Public Const EOV1_アクセス条件 As String = "0"
    <VBFixedString(6)> Public Const EOV1_ブロック数 As String = "000000"
    <VBFixedString(13)> Public Const EOV1_システム識別名 As String = " "
    <VBFixedString(7)> Public Const EOV1_予備1 As String = " "
    'ファイルラベル（EOV2)
    <VBFixedString(3)> Public Const EOV2_ラベル識別名 As String = "EOV"
    <VBFixedString(1)> Public Const EOV2_ラベル番号 As String = "2"
    <VBFixedString(1)> Public Const EOV2_レコード番号 As String = "F"       '固定長
    <VBFixedString(5)> Public EOV2_ブロック長 As String
    <VBFixedString(5)> Public EOV2_レコード長 As String
    <VBFixedString(1)> Public Const EOV2_記録密度 As String = " "
    <VBFixedString(1)> Public Const EOV2_ボリューム状態 As String = "0"     '単ボリューム
    <VBFixedString(21)> Public Const EOV2_予備1 As String = " "
    <VBFixedString(1)> Public EOV2_ブロック属性 As String
    <VBFixedString(41)> Public Const EOV2_予備2 As String = " "


    Structure DEVICE_status
        <VBFixedString(1)> Public BIT_TMK As String
        <VBFixedString(1)> Public BIT_EOT As String
        <VBFixedString(1)> Public BIT_BOT As String
        <VBFixedString(1)> Public BIT_DEN0 As String
        <VBFixedString(1)> Public BIT_DEN1 As String
        <VBFixedString(1)> Public BIT_FIL1 As String
        <VBFixedString(1)> Public BIT_PRO As String
        <VBFixedString(1)> Public BIT_FIL2 As String
        <VBFixedString(1)> Public BIT_DTE As String
        <VBFixedString(1)> Public BIT_HDE As String
        <VBFixedString(1)> Public BIT_NRDY As String
        <VBFixedString(1)> Public BIT_ILC As String
        <VBFixedString(1)> Public BIT_SCE As String
        <VBFixedString(1)> Public BIT_UDC As String
        <VBFixedString(1)> Public BIT_FIL3 As String
        <VBFixedString(1)> Public BIT_CHG As String
    End Structure

    '取扱いデータレコード定義
    <VBFixedString(120)> Public KZFDT120 As String   '全銀レコード
    <VBFixedString(180)> Public KZFDT180 As String   'NTT電話料/一括処理レコード
    <VBFixedString(220)> Public KZFDT220 As String   '地公体（富山県）
    <VBFixedString(350)> Public KZFDT350 As String   '地公体（三重県）
    <VBFixedString(390)> Public KZFDT390 As String   '国税


    Public Function mtCHGStatus(ByVal Fskj_mt_rtn As Long, ByRef Fskj_mt_BITrtn As DEVICE_status) As Integer
        Dim STATUS_BIT(16) As Integer
        Dim count As Integer

        mtCHGStatus = -1

        If Fskj_mt_rtn = 0 Then
            Fskj_mt_BITrtn.BIT_TMK = 0
            Fskj_mt_BITrtn.BIT_EOT = 0
            Fskj_mt_BITrtn.BIT_BOT = 0
            Fskj_mt_BITrtn.BIT_DEN0 = 0
            Fskj_mt_BITrtn.BIT_DEN1 = 0
            Fskj_mt_BITrtn.BIT_FIL1 = 0
            Fskj_mt_BITrtn.BIT_PRO = 0
            Fskj_mt_BITrtn.BIT_FIL2 = 0
            Fskj_mt_BITrtn.BIT_DTE = 0
            Fskj_mt_BITrtn.BIT_HDE = 0
            Fskj_mt_BITrtn.BIT_NRDY = 0
            Fskj_mt_BITrtn.BIT_ILC = 0
            Fskj_mt_BITrtn.BIT_SCE = 0
            Fskj_mt_BITrtn.BIT_UDC = 0
            Fskj_mt_BITrtn.BIT_FIL3 = 0
            Fskj_mt_BITrtn.BIT_CHG = 0
            'Fskj_mt_BITrtn = "0000000000000000"
            GoTo 処理終了
        End If
        If Fskj_mt_rtn = -1 Then
            Fskj_mt_BITrtn.BIT_TMK = 1
            Fskj_mt_BITrtn.BIT_EOT = 1
            Fskj_mt_BITrtn.BIT_BOT = 1
            Fskj_mt_BITrtn.BIT_DEN0 = 1
            Fskj_mt_BITrtn.BIT_DEN1 = 1
            Fskj_mt_BITrtn.BIT_FIL1 = 1
            Fskj_mt_BITrtn.BIT_PRO = 1
            Fskj_mt_BITrtn.BIT_FIL2 = 1
            Fskj_mt_BITrtn.BIT_DTE = 1
            Fskj_mt_BITrtn.BIT_HDE = 1
            Fskj_mt_BITrtn.BIT_NRDY = 1
            Fskj_mt_BITrtn.BIT_ILC = 1
            Fskj_mt_BITrtn.BIT_SCE = 1
            Fskj_mt_BITrtn.BIT_UDC = 1
            Fskj_mt_BITrtn.BIT_FIL3 = 1
            Fskj_mt_BITrtn.BIT_CHG = 1
            'Fskj_mt_BITrtn = "1111111111111111"
            GoTo 処理終了
        End If
        'ビット --> バイト変換
        'Fskj_mt_BITrtn = Null
        For count = 0 To 15
            STATUS_BIT(count) = Fskj_mt_rtn Mod 2
            'Fskj_mt_BITrtn = Fskj_mt_BITrtn & CStr(Abs(STATUS_BIT(count)))
            Fskj_mt_rtn = Fskj_mt_rtn \ 2
        Next
        Fskj_mt_BITrtn.BIT_TMK = CStr(Math.Abs(STATUS_BIT(0)))
        Fskj_mt_BITrtn.BIT_EOT = CStr(Math.Abs(STATUS_BIT(1)))
        Fskj_mt_BITrtn.BIT_BOT = CStr(Math.Abs(STATUS_BIT(2)))
        Fskj_mt_BITrtn.BIT_DEN0 = CStr(Math.Abs(STATUS_BIT(3)))
        Fskj_mt_BITrtn.BIT_DEN1 = CStr(Math.Abs(STATUS_BIT(4)))
        Fskj_mt_BITrtn.BIT_FIL1 = CStr(Math.Abs(STATUS_BIT(5)))
        Fskj_mt_BITrtn.BIT_PRO = CStr(Math.Abs(STATUS_BIT(6)))
        Fskj_mt_BITrtn.BIT_FIL2 = CStr(Math.Abs(STATUS_BIT(7)))
        Fskj_mt_BITrtn.BIT_DTE = CStr(Math.Abs(STATUS_BIT(8)))
        Fskj_mt_BITrtn.BIT_HDE = CStr(Math.Abs(STATUS_BIT(9)))
        Fskj_mt_BITrtn.BIT_NRDY = CStr(Math.Abs(STATUS_BIT(10)))
        Fskj_mt_BITrtn.BIT_ILC = CStr(Math.Abs(STATUS_BIT(11)))
        Fskj_mt_BITrtn.BIT_SCE = CStr(Math.Abs(STATUS_BIT(12)))
        Fskj_mt_BITrtn.BIT_UDC = CStr(Math.Abs(STATUS_BIT(13)))
        Fskj_mt_BITrtn.BIT_FIL3 = CStr(Math.Abs(STATUS_BIT(14)))
        Fskj_mt_BITrtn.BIT_CHG = CStr(Math.Abs(STATUS_BIT(15)))

処理終了:
        If Err.Number <> 0 Then
            mtCHGStatus = Err.Number
        Else
            mtCHGStatus = 0
        End If
    End Function




End Module
