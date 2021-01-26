Imports System
Imports System.IO
Imports CASTCommon
Imports System.Runtime.InteropServices

REM #Const DEBUGCMT = True

Public Class ClsCMTCTRL

    <VBFixedString(8)> Dim strMemMitsudo As String    ' 記録密度
    <VBFixedString(8)> Dim strProtect As String    ' ログ用プロテクト判定
    Private LOG As New CASTCommon.BatchLOG("CMT読書制御", "CMT")
    Private strLogwrite As String    ' ログ詳細
    Private bit_status As DEVICE_status    ' バイト変換した復帰値

    '***ASTAR ブロックサイズ対応 >>
    Private mnBlockSize As Integer = -1
    Public Property BlockSize() As Integer
        Get
            Return mnBlockSize
        End Get
        Set(ByVal Value As Integer)
            mnBlockSize = Value
        End Set
    End Property
    '***ASTAR ブロックサイズ対応 <<

    '-------------------------------------------------------------------------------------------
    '----- ＣＭＴアクセス用関数定義（ＪＰＣ社ＤＬＬ用）-----------------------------------------
    '-------------------------------------------------------------------------------------------

    ' 新モジュール用定義
    Private Declare Function mtinit Lib "mtdll53.dll" (ByRef lpmtinf As MTINFBLOCK) As Integer
    '装置のステータス読み取り
    Private Declare Function mtstat Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'ＢＯＴまで巻き戻し
    Private Declare Function mtrewind Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '１ブロック読み込み
    Private Declare Function mtrblock Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef buff As Byte, ByRef blklen_ As Integer, ByVal bufflen_ As Long) As Integer
    '１ブロック書き込み
    Private Declare Function mtwblock Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef buff As Byte, ByVal blklen_ As Integer) As Integer
    '１ブロック前進
    Private Declare Function mtfblock Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '１ブロック後退
    Private Declare Function mtbblock Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'テープマークを検出するまで前進
    Private Declare Function mtffile Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'テープマークを検出するまで後退
    Private Declare Function mtbfile Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'テープマーク１個ライト
    Private Declare Function mtwtmk Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'テープマーク２個ライト
    Private Declare Function mtwmtmk Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'マルチテープマークの検出
    Private Declare Function mtsmtmk Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'テープのアンロード
    Private Declare Function mtunload Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    '装置のオンライン
    Private Declare Function mtonline Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    'ＥＢＣＤＩＣコード変換
    Private Declare Function mtebc Lib "mtdll53.dll" (ByVal code_ As Byte) As Integer
    '磁気テープごとにＥＢＣＤＩＣコード変換
    Private Declare Function mtebcex Lib "mtdll53.dll" (ByVal code_ As Byte) As Integer
    'ＭＴのイレーズ
    Private Declare Function mters Lib "mtdll53.dll" (ByVal mtid_ As Byte) As Integer
    ' トラック数、記録密度変更 
    Private Declare Function mtdensity Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByVal code_ As Byte) As Integer
    ' ＣＳＬからテープをロードする 
    'Private Declare Function mtloadcsl Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef slot_no_ As Byte, ByVal ctrlflag_ As Byte) As Integer
    ' ＣＳＬからテープをロードする 
    'Private Declare Function mtstatcsl Lib "mtdll53.dll" (ByVal mtid_ As Byte, ByRef CSLDATA As CSLDATA) As Integer

    '------------------------------------------
    ' ＭＴ制御用
    '------------------------------------------

    ' ＤＬＬを呼び出す為に必要な宣言
    Structure MTINFBLOCK
        Public UnitNo As Byte
        Public HostNo As Byte
        Public TargetNo As Byte
        <MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.ByValTStr, SizeConst:=8), VBFixedArray(8)> Public Vender() As Char
        <MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.ByValTStr, SizeConst:=16), VBFixedArray(16)> Public Product() As Char
        <MarshalAs(UnmanagedType.ByValArray, ArraySubType:=UnmanagedType.ByValTStr, SizeConst:=4), VBFixedArray(4)> Public VERSION() As Char
        Public Reserve As Byte
    End Structure

    ' ＣＳＬ情報構造体
    'Structure CSLDATA
    '    Public CslModeCode As Byte
    '    Public Drive As Byte
    '    Public SlotPosition As Byte
    '    Public Slot1 As Byte
    '    Public Slot2 As Byte
    '    Public Slot3 As Byte
    '    Public Slot4 As Byte
    '    Public Slot5 As Byte
    '    Public Slot6 As Byte
    '    Public Slot7 As Byte
    '    Public Slot8 As Byte
    '    Public Slot9 As Byte
    '    Public Slot10 As Byte
    'End Structure

    ' 構造体の宣言
    Private CMTINFO As New MTINFBLOCK
    'Private LOADERDATA As New CSLDATA

    ' ＣＭＴ関数復帰値バイト格納
    Structure DEVICE_status
        <VBFixedString(1)> Public BIT_TMK As String
        <VBFixedString(1)> Public BIT_EOT As String
        <VBFixedString(1)> Public BIT_BOT As String
        <VBFixedString(1)> Public BIT_DEN0 As String
        <VBFixedString(1)> Public BIT_DEN1 As String
        <VBFixedString(1)> Public BIT_CSL As String
        <VBFixedString(1)> Public BIT_PRO As String
        <VBFixedString(1)> Public BIT_FIL1 As String
        <VBFixedString(1)> Public BIT_DTE As String
        <VBFixedString(1)> Public BIT_HDE As String
        <VBFixedString(1)> Public BIT_NRDY As String
        <VBFixedString(1)> Public BIT_ILC As String
        <VBFixedString(1)> Public BIT_SCE As String
        <VBFixedString(1)> Public BIT_UDC As String
        <VBFixedString(1)> Public BIT_TIM As String
        <VBFixedString(1)> Public BIT_CHC As String
        <VBFixedString(1)> Public BIT_FIL2 As String
        <VBFixedString(1)> Public BIT_FIL3 As String
        <VBFixedString(1)> Public BIT_FIL4 As String
        <VBFixedString(1)> Public BIT_FIL5 As String
        <VBFixedString(1)> Public BIT_ROB As String
        <VBFixedString(1)> Public BIT_FIL6 As String
        <VBFixedString(1)> Public BIT_FIL7 As String
        <VBFixedString(1)> Public BIT_FIL8 As String
        <VBFixedString(1)> Public BIT_FIL9 As String
        <VBFixedString(1)> Public BIT_FIL10 As String
        <VBFixedString(1)> Public BIT_FIL11 As String
        <VBFixedString(1)> Public BIT_FIL12 As String
        <VBFixedString(1)> Public BIT_FIL13 As String
        <VBFixedString(1)> Public BIT_FIL14 As String
        <VBFixedString(1)> Public BIT_FIL15 As String
        <VBFixedString(1)> Public BIT_FIL16 As String

    End Structure

#Region "public CmtCtrl()"
    '
    ' 機　能 : ＣＭＴ読書制御関数
    '          引数に応じて呼び出す関数を制御する。
    '
    ' 戻り値 : 0 - 正常  0以外 - 異常
    '
    ' 引  数 : ARG1 - 11=暗号化なし読取、12=暗号化あり読取、 22=暗号化あり書込
    '
    ' 備　考 : 
    ' 
    Public Function CmtCtrl(ByVal read As Integer) As Integer
        Dim nRtn As Integer   ' 関数復帰値     
        Dim nSlotSum As Integer    ' ＣＭＴスロット格納数
        Dim bSlotNo As Byte    ' 現在読込んでいるＣＭＴテープのスロット番号

        'LOG.SyoriMei = "CmtCtrl"

        ' 構造体初期化
        Call CmtInfoInit()
        'Call CslInfoInit()

        ' ＣＭＴ装置初期処理
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return nRtn
        End If

        Select Case read
            Case 11    ' オートローダー読込処理
                nRtn = ReadAutoLoaderCmt()

            Case 12    ' 暗号あり読込
                nRtn = ChkCmtStat(bSlotNo, nSlotSum)
                If nRtn = 1 And bSlotNo = 0 And nSlotSum = 1 Then
                    ' 手差し暗号あり読込
                    If ReadCmt(1, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                ElseIf nRtn = 1 And bSlotNo <> 0 And nSlotSum = 1 Then
                    ' マガジンに一つ格納暗号あり読込
                    If ReadCmt(bSlotNo, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                Else
                    nRtn = 1
                End If
                'Case 21    ' 暗号なし書込
                '    nRtn = NotEncCmtWrite()
            Case 22    ' 暗号あり書込
                nRtn = ChkCmtStat(bSlotNo, nSlotSum)
                If nRtn = 1 And bSlotNo = 0 And nSlotSum = 1 Then
                    ' 手差し暗号あり書込
                    If WriteCmt(1, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                ElseIf nRtn = 1 And bSlotNo <> 0 And nSlotSum = 1 Then
                    ' マガジンに一つ格納暗号あり書込
                    If WriteCmt(bSlotNo, False) Then
                        nRtn = 0
                    Else
                        nRtn = 1
                    End If
                Else
                    nRtn = 1
                End If
            Case Else
                strLogwrite = "パラメータが正しくありません" & _
                                "パラメータ：" & read.ToString
                LOG.Write("起動パラメータ判定", "失敗", strLogwrite)
                nRtn = 1
        End Select
        Return nRtn
    End Function
#End Region
#Region "public SelectCmt()"
    '
    ' 機　能 : ＣＭＴロード関数
    '
    ' 戻り値 : TRUE - 正常  FALSE - 異常
    '
    ' 引  数 : ARG1 - 1-10: ロードするスロット番号
    '                 21  : マガジンのロード
    '                 22  : マガジンのイジェクト
    '
    ' 備　考 : 
    ' 
    Public Function SelectCmt(ByVal slotno As Byte) As Boolean
        Dim nRtn As Integer = 0
        Dim nMtRtn As Integer
        Dim cflg As Byte = 0

        'LOG.SyoriMei = "SelectCmt"

        ' 構造体初期化
        Call CmtInfoInit()

        If Not (slotno >= 1 And slotno <= 10 Or slotno = 21 Or slotno = 22) Then
            LOG.Write("パラメータ異常", "slotno：" & slotno.ToString)
            Return False
        End If

        ' ＣＭＴ装置初期処理
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

#If DEBUGCMT Then
        LOADERDATA.Slot1 = 1
        '        LOADERDATA.Slot2 = 1
#End If

        nRtn = MtCHGStatus(nMtRtn, bit_status)

        If bit_status.BIT_CSL = "0" Then
            Return True
        Else
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("SelectCmt", "CSLエラーが起きました", strLogwrite)
            Return False
        End If
    End Function
#End Region
#Region "public UnloadCmt()"
    '
    ' 機　能 : ＣＭＴアンロード関数
    '
    ' 戻り値 : TRUE - 正常  FALSE - 異常
    '
    ' 引  数 : なし
    '
    ' 備　考 : ＣＭＴ装置内に格納中のＣＭＴテープををアンロードする
    ' 
    Public Function UnloadCmt() As Boolean
        Dim nRtn As Integer = 0
        Dim bRtn As Boolean
        Dim nMtRtn As Integer
        'LOG.SyoriMei = "UnloadCmt"

        ' 構造体初期化
        Call CmtInfoInit()
        'Call CslInfoInit()

        ' ＣＭＴ装置初期処理
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

        ' ＣＭＴステータス
        nRtn = FuncMtStat()
        If nRtn <> 0 Then
            bRtn = False
        End If

        '------------------------------------------
        'ＣＭＴアンロード
        '------------------------------------------
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)
        If bit_status.BIT_NRDY = "0" Then
            nMtRtn = mtunload(CMTINFO.UnitNo)
        End If

        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)
        If bit_status.BIT_NRDY = "1" Then
            bRtn = True
        Else
            nRtn = MtCHGStatus(nMtRtn, bit_status)
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("UnloadCmt", nMtRtn.ToString, strLogwrite)
            bRtn = False
        End If
        Return bRtn
    End Function
#End Region
#Region "public ReadCmt()"
    '
    ' 機　能 : ＣＭＴテープ読取関数
    '          指定したＣＭＴテープからファイルを読込む
    '
    ' 戻り値 : TRUE - 正常  FALSE - 異常
    '
    ' 引  数 : ARG1 - ＣＭＴテープ格納元スロット番号
    '          ARG2 - TURE - 暗号なし読込 FALSE - 暗号あり読込
    '
    ' 備　考 : 
    '
    Public Function ReadCmt(ByVal siteislotno As Byte) As Boolean
        ' オーバーライド
        Return ReadCmt(siteislotno, True)
    End Function

    Public Function ReadCmt(ByVal siteislotno As Byte, ByVal codeflg As Boolean) As Boolean
        Dim nRtn As Integer    ' 関数復帰値     
        Dim bHeadchk As Boolean    ' ＣＭＴヘッダーラベル有無判定 TRUE：ファイル有 FALSE：ファイルなし
        Dim bFilechk As Boolean    ' ＣＭＴファイル有無判定 TRUE：ファイル有 FALSE：ファイルなし
        Dim bEndchk As Boolean    ' ＣＭＴエンドラベル有無判定 TRUE：ファイル有 FALSE：ファイルなし
        Dim strPath As String    ' パス格納
        Dim nMtRtn As Integer    ' ＣＭＴ関数復帰値
        Dim len As Integer = 0    ' 読込バッファ長
        Dim buff(&HF000 - 1) As Byte    ' 読込ストリーム
        Dim fl As FileStream    ' ファイルストリーム

        ' 初期処理
        'LOG.SyoriMei = "ReadCmt"
        bHeadchk = False
        bFilechk = False
        bEndchk = False

        If siteislotno > 10 Or siteislotno < 1 Then
            LOG.Write("指定スロット番号が正しくありません", "siteislotno:" & siteislotno.ToString)
            Return False
        End If

        ' ＣＭＴ装置初期処理
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

        ' ＣＭＴ装置ステータス処理
        nRtn = FuncMtStat()
        If nRtn <> 0 Then
            Return False
        End If

        ' ＣＭＴ装置オンライン処理
        nRtn = FuncMtOnline()
        If nRtn <> 0 Then
            Return False
        End If

        ' 暗号化ありかなしかフラグ
        If codeflg Then
            ' 引数と実際格納スロットとの正誤比較
            If Not ChkSlot(siteislotno) Then
                Return False
            End If
        End If

        ' CMT.INI設定
        PutCMTIni("READ-RESULT", siteislotno.ToString, "1")
        PutCMTIni("LABEL-EXIST", siteislotno.ToString, "0")

        ' パス格納
        strPath = GetCMTIni("READ-DIRECTORY", siteislotno.ToString)

        'ファイル出力先の先行判定
        If Not ChkReadDrFl(strPath, siteislotno) Then
            Return False
        End If

        ' 記録密度判定
        If Not ChkKirokumitsudo() Then
            PutCMTIni("READ-RESULT", siteislotno.ToString, "4")
            Return False
        End If

        '------------------------------------------
        'コード変換モード設定
        '------------------------------------------
        'If codeflg Then
        '    nMtRtn = mtebc(7)
        'Else
        '    nMtRtn = mtebc(0)
        'End If
        nMtRtn = mtebc(0)

#If DEBUGCMT Then
        File.Copy(strPath & "\..\" & GetCMTIni("FILE-NAME", "READ"), strPath & "\" & GetCMTIni("FILE-NAME", "READ"), True)
        bFilechk = True
        If File.Exists(strPath & "\..\" & GetCMTIni("FILE-NAME", "HEAD")) = True Then
            File.Copy(strPath & "\..\" & GetCMTIni("FILE-NAME", "HEAD"), strPath & "\" & GetCMTIni("FILE-NAME", "HEAD"), True)
            bHeadchk = True
        End If
        If File.Exists(strPath & "\..\" & GetCMTIni("FILE-NAME", "END")) = True Then
            File.Copy(strPath & "\..\" & GetCMTIni("FILE-NAME", "END"), strPath & "\" & GetCMTIni("FILE-NAME", "END"), True)
            bEndchk = True
        End If
        bit_status.BIT_TMK = 1
#Else
        '------------------------------------------
        'ファイル読込処理
        '------------------------------------------
        Try
            fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "HEAD"), FileMode.CreateNew)
            ' ヘッダーラベル読込
            Do While True
                ' バッファ領域クリア
                Array.Clear(buff, 0, buff.Length)
                ' 一ブロック読込み
                nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                'LOG.Write("mtrblock", "ヘッドラベル読込", "読込バッファ長:" & len.ToString)
                'デバイスステータス取得
                nRtn = MtCHGStatus(nMtRtn, bit_status)
                ' エラー判定
                If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                    ' ログ出力
                    strLogwrite = "ＣＭＴ読込エラー（ヘッダーラベル）が起きました" & vbCrLf & _
                                " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                " エラーコード：" & nMtRtn.ToString
                    LOG.Write("ヘッダーラベル読込処理", "失敗", strLogwrite)
                    Call LogCmtStat()
                    PutCMTIni("READ-RESULT", siteislotno.ToString, "4")

                    fl.Close()
                    Exit Do    ' ヘッドラベル読込ループ脱出
                End If
#If DEBUGCMT Then
                bit_status.BIT_TMK = 1
#End If
                'テープマーク検出
                If bit_status.BIT_TMK = 1 Then
                    fl.Close()
                    Exit Do    ' ヘッドラベル読込ループ脱出
                End If
                ' HEADLABELに書込
                fl.Write(buff, 0, len)
                ' ヘッダーラベル読込フラグ
                bHeadchk = True
            Loop
        Catch ex As Exception
            LOG.Write("ヘッダーラベル読込", "例外", ex.Message)
        End Try

        Try
            fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "READ"), FileMode.CreateNew)
            ' ファイル読込
            Do While True
                ' バッファ領域クリア
                Array.Clear(buff, 0, buff.Length)
                ' 一ブロック読込み
                nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                'LOG.Write("mtrblock", "ファイル読込", "読込バッファ長:" & len.ToString)
                'デバイスステータス取得
                nRtn = MtCHGStatus(nMtRtn, bit_status)
                ' エラー判定
                If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                    ' ログ出力
                    strLogwrite = "ＣＭＴ読込エラー（ファイル）が起きました" & vbCrLf & _
                                " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                " エラーコード：" & nMtRtn.ToString
                    LOG.Write("ファイル読込処理", "失敗", strLogwrite)
                    Call LogCmtStat()
                    PutCMTIni("READ-RESULT", siteislotno.ToString, "4")

                    fl.Close()
                    Exit Do    ' ファイル読込ループ脱出
                End If
                ' テープマーク検出
                If bit_status.BIT_TMK = 1 Then
                    fl.Close()
                    Exit Do    ' ファイル読込ループ脱出
                End If
                ' INPUTに書込
                fl.Write(buff, 0, len)

                ' ファイル読込フラグ
                bFilechk = True
            Loop
        Catch ex As Exception
            LOG.Write("ファイル読込", "例外", ex.Message)
        End Try

        Try
            ' ヘッダーラベル、ファイルを読込んだときのみ実行
            If bHeadchk = True And bFilechk = True Then
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "END"), FileMode.CreateNew)
                ' エンドラベル読込
                Do While True
                    ' バッファ領域クリア
                    Array.Clear(buff, 0, buff.Length)
                    ' 一ブロック読込
                    nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                    'LOG.Write("mtrblock", "エンドラベル読込", "読込バッファ長:" & len.ToString)
                    'デバイスステータス取得
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    ' エラー判定
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ログ出力
                        strLogwrite = "ＣＭＴ読込エラー（エンド）が起きました" & vbCrLf & _
                                    " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                    " エラーコード：" & nMtRtn.ToString
                        LOG.Write("読込処理", "失敗", strLogwrite)
                        Call LogCmtStat()
                        PutCMTIni("READ-RESULT", siteislotno.ToString, "4")

                        fl.Close()
                        Exit Do    ' エンドラベル読込ループ脱出
                    End If
                    ' テープマーク検出
                    If bit_status.BIT_TMK = 1 Then
                        fl.Close()
                        Exit Do    ' エンドラベル読込ループ脱出
                    End If
                    ' ENDLABELに書込
                    fl.Write(buff, 0, len)

                    ' エンドラベル読込フラグ
                    bEndchk = True
                Loop
            End If

        Catch ex As Exception
            LOG.Write("エンドラベル読込", "例外", ex.Message)
        End Try
#End If

        ' 読込ＣＭＴラベル有無判定
        nRtn = ChkLabel(strPath, nMtRtn, siteislotno, bHeadchk, bFilechk, bEndchk)
        If nRtn <> 0 Then
            LOG.Write("ファイル読み込み異常です", "HEADLABEL:" & bHeadchk & " INPUT:" & bFilechk & " ENDLABEL:" & bEndchk)
        End If

        ' ＣＭＴ装置巻き戻し処理
        If FuncMtrewind() <> 0 Then
            Return False
        End If
        Call LogCmtStat()

        ' 読取結果判定
        If Not "0" = GetCMTIni("READ-RESULT", siteislotno.ToString) Then
            Return False
        End If
        Return True
    End Function
#End Region
#Region "public WriteCmt()"

    '
    ' 機　能 : ＣＭＴテープ書込関数
    '
    ' 戻り値 : TRUE - 正常  FALSE - 異常
    '
    ' 引  数 : ARG1 - ＣＭＴテープ格納元スロット番号
    '          ARG2 - TRUE - EBCDIC変換する FALSE -EBCDIC変換しない 
    '
    ' 備　考 : 
    '   
    Public Function WriteCmt(ByVal siteislotno As Byte) As Boolean
        ' オーバーライド
        Return WriteCmt(siteislotno, True)
    End Function
    Public Function WriteCmt(ByVal siteislotno As Byte, ByVal codeflg As Boolean) As Boolean
        Dim strPath As String    ' パス格納
        Dim nRtn As Integer    ' 関数復帰値     
        Dim nMtRtn As Integer    ' CMT復帰値
        Dim buff(&HF000 - 1) As Byte ' 書込バイトストリーム
        Dim count As Integer    ' ファイル読み込み時の文字列長
        Dim nBlocklen As Integer   ' ブロック長
        Dim bHeadchk As Boolean    ' 書込ヘッダーラベル有無判定 TRUE：ファイルあり FALSE：ファイルなし
        Dim bEndchk As Boolean    ' 書込エンドラベル有無判定 TRUE：ファイルあり FALSE：ファイルなし
        Dim bFilechk As Boolean    ' 書込ファイル有無判定 TRUE：ファイルあり FALSE：ファイルなし
        Dim bDrive As Boolean    ' ＣＭＴ読込有無判定 TRUE：読込状態 FALSE：非読込状態
        Dim nCnt As Integer    ' 汎用カウンタ
        Dim fl As FileStream


        ' 初期処理
        'LOG.SyoriMei = "WriteCmt"
        bHeadchk = False
        bFilechk = False
        bEndchk = False
        bDrive = False

        If siteislotno > 10 Or siteislotno < 1 Then
            LOG.Write("指定スロット番号が正しくありません", "siteislotno:" & siteislotno.ToString)
            Return False
        End If

        ' ＣＭＴ装置初期処理
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            Return False
        End If

        ' ＣＭＴ装置ステータス処理
        nRtn = FuncMtStat()
        If nRtn <> 0 Then
            Return False
        End If

        ' ＣＭＴ装置オンライン処理
        nRtn = FuncMtOnline()
        If nRtn <> 0 Then
            Return False
        Else
            bDrive = True
        End If

        ' 暗号化ありなしフラグ
        If codeflg Then
            ' 引数と実際格納スロットとの正誤比較
            If Not ChkSlot(siteislotno) Then
                Return False
            End If
        End If

        ' CMT.INI設定
        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "1")

        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        '記録密度判定
        If Not ChkKirokumitsudo() Then
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
            Return False
        End If

        'プロテクト判定
        Select Case bit_status.BIT_PRO
            Case 0
                strProtect = "書込可能"
            Case 1
                strProtect = "書込禁止"
                strLogwrite = "書込禁止です  プロテクトを解除してください" & vbCrLf & _
                "ステータス：" & bit_status.BIT_PRO & bit_status.BIT_DEN1
                LOG.Write("プロテクト判定", "失敗", strLogwrite)
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            Case Else
        End Select

        ' パス格納
        strPath = GetCMTIni("WRITE-DIRECTORY", siteislotno.ToString)

        If Not ChkWriteDrFl(strPath, siteislotno, bDrive, bHeadchk, bFilechk, bEndchk) Then
            Return False
        End If

        '------------------------------------------
        'コード変換モード設定
        '------------------------------------------
        'If codeflg Then
        '    nMtRtn = mtebc(7)
        'Else
        '    nMtRtn = mtebc(0)
        'End If

        nMtRtn = mtebc(0)

        '------------------------------------------
        'ファイル書込処理
        '------------------------------------------
        LOG.Write("head:" & bHeadchk.ToString, "file:" & bFilechk.ToString, "end:" & bEndchk.ToString)

        If bHeadchk = True And bFilechk = True And bEndchk = True Then
            ' ラベルあり返却ファイルの場合
            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "HEAD"))) Then
                    LOG.Write("書込処理", "失敗", Path.Combine(strPath, GetCMTIni("FILE-NAME", "HEAD")) & "がありません")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' ヘッダーラベル書込
                fl = New FileStream(Path.Combine(strPath, GetCMTIni("FILE-NAME", "HEAD")), FileMode.Open)
                ' バッファ領域クリア
                Array.Clear(buff, 0, buff.Length)
                ' カウントを0に
                nCnt = 0
                ' ヘッダーラベル読込
                count = fl.Read(buff, 0, 80)
                Do
                    ' ヘッダーラベル２に格納しているブロック長を取得する
                    If nCnt = 2 Then
                        Try
                            nBlocklen = Integer.Parse("0" & System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(buff, 5, 5).Trim)
                            LOG.Write("ラベルありヘッダー書込", "nCnt:" & nCnt.ToString, "ブロック長：" & nBlocklen.ToString)
                        Catch ex As Exception
                            '***ASTAR SUSUKI 2008.06.13             ***
                            'EBCDIC対応                             ***
                            nBlocklen = Integer.Parse("0" & System.Text.Encoding.GetEncoding("IBM290").GetString(buff, 5, 5).Trim)
                            LOG.Write("ラベルありヘッダー書込", "nCnt:" & nCnt.ToString, "ブロック長：" & nBlocklen.ToString)
                            '******************************************
                        End Try
                    End If
                    nCnt = nCnt + 1
                    ' 一ブロック書込
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    LOG.Write("mtwblock", "ヘッダーラベル書込", "書込バッファ長:" & count.ToString)
                    ' デバイスステータス取得
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    'エラー判定
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ログ出力
                        strLogwrite = "ヘッダーラベル書込異常が起きました" & vbCrLf & _
                                    " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                    " エラーコード：" & nMtRtn.ToString
                        LOG.Write("ヘッダーラベル書込処理", "失敗", strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Return False
                    End If
                    ' バッファ領域クリア
                    Array.Clear(buff, 0, buff.Length)
                    ' 返却ヘッダーラベルをバッファに読込
                    count = fl.Read(buff, 0, 80)
                Loop While count > 0    ' ファイルから読取ったバッファ長が0になるまで繰り返す
                fl.Close()
            Catch ex As Exception
                LOG.Write("書込処理例外", "ヘッダーラベル", ex.Message)
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try
            ' テープマーク書込
            nMtRtn = mtwtmk(CMTINFO.UnitNo)
            LOG.Write("ヘッダーラベル書込", "mtwtmk", nMtRtn.ToString)

            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE"))) Then
                    LOG.Write("書込処理", "失敗", Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE")) & "ありません")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' ファイル書込
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "WRITE"), FileMode.Open)
                ' バッファ領域クリア
                Array.Clear(buff, 0, buff.Length)
                ' ヘッダーラベル書込時に取得したブロック長分バッファに読込む
                count = fl.Read(buff, 0, nBlocklen)
                Do
                    ' 一ブロック書込
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    '*** 修正 mitsu 2008/08/29 コメントアウト ***
                    'LOG.Write("mtwblock", "返却ファイル書込", "書込バッファ長:" & count.ToString)
                    '********************************************
                    'デバイスステータス取得
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    'エラー判定
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ログ出力
                        strLogwrite = "返却ファイル書込異常が起きました" & vbCrLf & _
                                    " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                    " エラーコード：" & nMtRtn.ToString
                        LOG.Write("返却ファイル書込処理", "失敗", strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Return False
                    End If
                    ' バッファ領域クリア
                    Array.Clear(buff, 0, buff.Length)
                    ' ヘッダーラベル書込時に取得したブロック長分バッファに読込む
                    count = fl.Read(buff, 0, nBlocklen)
                Loop While count > 0
                fl.Close()
            Catch ex As Exception
                LOG.Write("書込処理例外", "返却ファイル", ex.Message)
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try
            nMtRtn = mtwtmk(CMTINFO.UnitNo)
            LOG.Write("返却ファイル書込", "mtwtmk", nMtRtn.ToString)

            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "END"))) Then
                    LOG.Write("書込処理", "失敗", Path.Combine(strPath, GetCMTIni("FILE-NAME", "END")) & "ありません")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' エンドラベル書込
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "END"), FileMode.Open)
                ' バッファ領域クリア
                Array.Clear(buff, 0, buff.Length)
                ' エンドラベル読込
                count = fl.Read(buff, 0, 80)
                Do
                    ' 一ブロック書込
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    LOG.Write("mtwblock", "エンドラベル書込", "書込バッファ長:" & count.ToString)
                    'デバイスステータス取得
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    'エラー判定
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        ' ログ出力
                        strLogwrite = "エンドラベル書込異常が起きました" & vbCrLf & _
                                    " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                    " エラーコード：" & nMtRtn.ToString
                        LOG.Write("書込処理", "失敗", strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Return False
                    End If
                    ' バッファ領域クリア
                    Array.Clear(buff, 0, buff.Length)
                    ' 返却エンドラベルをバッファに読込む 
                    count = fl.Read(buff, 0, 80)
                Loop While count > 0
                fl.Close()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "0")
            Catch ex As Exception
                LOG.Write("書込処理例外", "エンドレコード", ex.Message)
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try
            nMtRtn = mtwmtmk(CMTINFO.UnitNo)
            LOG.Write("返却ファイル書込", "mtwmtmk", nMtRtn.ToString)
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "0")

        ElseIf bHeadchk = False And bFilechk = True And bEndchk = False Then
            ' ラベルなし返却ファイルの場合
            Try
                If Not File.Exists(Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE"))) Then
                    LOG.Write("書込処理", "失敗", Path.Combine(strPath, GetCMTIni("FILE-NAME", "WRITE")) & "ありません")
                    PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                    Return False
                End If

                ' テープマークありフォーマットの場合
                If GetCMTIni("LABEL-EXIST", siteislotno.ToString) = "2" Then
                    nMtRtn = mtwtmk(CMTINFO.UnitNo)
                    LOG.Write("テープマークありフォーマット", "mtwtmk", nMtRtn.ToString)
                End If

                '***ASTAR 2008.08.07 ブロック指定対応 >>
                Dim nBuffLen As Integer
                If Me.BlockSize = -1 Then
                    'ブロックサイズ指定なし
                    nBuffLen = buff.Length
                Else
                    'ブロックサイズ指定あり
                    nBuffLen = Me.BlockSize
                End If
                '***ASTAR 2008.08.07 ブロック指定対応 <<

                ' ファイル書込
                fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "WRITE"), FileMode.Open)
                ' バッファ領域をクリア
                Array.Clear(buff, 0, buff.Length)
                ' 返却ファイルを読込む
                '***ASTAR 2008.08.07 ブロック指定対応 >>
                'count = fl.Read(buff, 0, buff.Length)
                count = fl.Read(buff, 0, nBuffLen)
                '***ASTAR 2008.08.07 ブロック指定対応 <<
                Do While count > 0
                    ' 一ブロック書込
                    nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
                    '*** 修正 mitsu 2008/08/29 コメントアウト ***
                    'LOG.Write("mtwblock", "返却ファイル書込", "書込バッファ長:" & count.ToString)
                    '********************************************
                    'デバイスステータス取得
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                    'エラー判定
                    If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                        'ログ出力
                        LOG.Write("ノンラベル返却ファイル書込異常が起きました", _
                                "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & _
                                " DTE:" & bit_status.BIT_DTE & " HDE:" & bit_status.BIT_HDE, strLogwrite)
                        PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                        Call LogCmtStat()
                        fl.Close()
                        Exit Do
                    End If
                    ' バッファ領域をクリア
                    Array.Clear(buff, 0, buff.Length)
                    '返却ファイルをバッファに読込む
                    '***ASTAR 2008.08.07 ブロック指定対応 >>
                    'count = fl.Read(buff, 0, buff.Length)
                    count = fl.Read(buff, 0, nBuffLen)
                    '***ASTAR 2008.08.07 ブロック指定対応 <<
                Loop
                fl.Close()
            Catch ex As Exception
                If GetCMTIni("LABEL-EXIST", siteislotno.ToString) = "0" Then
                    LOG.Write("書込処理例外", "ノンラベル返却ファイル", ex.Message)
                ElseIf GetCMTIni("LABEL-EXIST", siteislotno.ToString) = "2" Then
                    LOG.Write("書込処理例外", "テープマークあり返却ファイル", ex.Message)
                End If
                Call LogCmtStat()
                PutCMTIni("WRITE-RESULT", siteislotno.ToString, "4")
                Return False
            End Try

            mtwmtmk(CMTINFO.UnitNo)
            LOG.Write("返却ファイル書込", "mtwmtmk", nMtRtn.ToString)
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "0")

        ElseIf bHeadchk = False And bFilechk = False And bEndchk = False Then
            LOG.Write("書込ファイルがありません", "HEAD:" & bHeadchk & " FILE:" & bFilechk & "END" & bEndchk)
            PutCMTIni("WRITE-RESULT", siteislotno.ToString, "2")
        Else
            LOG.Write("書込ファイルが足りません", "HEAD:" & bHeadchk & " FILE:" & bFilechk & "END" & bEndchk)
        End If

        ' 巻き戻し
        If FuncMtrewind() <> 0 Then
            Return False
        End If
        Call LogCmtStat()

        ' 書込結果判定
        If Not "0" = GetCMTIni("WRITE-RESULT", siteislotno.ToString) Then
            Return False
        End If
        Return True
    End Function
#End Region
#Region "public ChkCmtStat()"
    '
    ' 機　能 : ＣＭＴ情報取得関数
    '
    ' 戻り値 : 0 - ＣＭＴテープがＣＭＴ装置にない 
    '          1 - ＣＭＴテープがＣＭＴ装置にある
    '          4 - ＣＭＴ装置エラー
    '
    ' 引  数 : ARG1 - ＣＭＴ格納元スロット
    '          ARG2 - ＣＭＴテープ合計
    '
    ' 備　考 : 
    ' 
    Public Function ChkCmtStat(ByRef slotno As Byte, ByRef slotsum As Integer) As Integer
        Dim nRtn As Integer  ' 関数復帰値

        slotno = 0
        slotsum = 0

        'mtinit
        nRtn = FuncMtinit()
        If nRtn <> 0 Then
            slotno = 0
            Return 4
        End If

        'mtstat
        nRtn = FuncMtStat()
        'If nRtn <> 0 Then
        '    slotno = 0
        '    Return 4
        'End If
        slotsum = 1
    End Function
#End Region
#Region "private ReadAutoLoaderCmt()"
    '
    ' 機　能 : オートローダーＣＭＴ読取関数
    '
    ' 戻り値 : 0 - 正常  1 - 異常
    '
    ' 引  数 : なし
    '
    ' 備　考 : 
    '
    Private Function ReadAutoLoaderCmt() As Integer
        Dim nRtn As Integer    ' 関数復帰値     
        Dim nMtRtn As Integer    ' ＣＭＴ関数復帰値
        Dim len As Integer = 0    '読込バッファ長
        Dim strPath As String    ' パス
        Dim buff(&HF000 - 1) As Byte    ' 読込バッファ
        Dim bSlotNo As Byte    ' ＣＭＴスロットナンバー（ループに使用）
        Dim cflg As Byte = 1    ' ＣＳＬステータスフラグ
        Dim bMemNo As Byte = 0    ' ＣＭＴテープ格納中で最大スロット番号
        Dim fl As FileStream    ' ファイルストリーム
        Dim nErrCnt As Integer    ' 最終エラー数カウント
        Dim bHandFlg As Boolean = False  ' 手差しフラグ
        Dim bFstFlg As Boolean = False    ' 一回処理する
        Dim bHeadchk As Boolean    ' ＣＭＴヘッダーラベル有無判定 TRUE：ファイル有 FALSE：ファイルなし
        Dim bFilechk As Boolean    ' ＣＭＴファイル有無判定 TRUE：ファイル有 FALSE：ファイルなし
        Dim bEndchk As Boolean    ' ＣＭＴエンドラベル有無判定 TRUE：ファイル有 FALSE：ファイルなし

        'LOG.SyoriMei = "ReadAutoLoaderCmt"

        'CMT.INIにステータスを格納
        For bSlotNo = 1 To 10 Step 1
            PutCMTIni("READ-RESULT", bSlotNo.ToString, "1")
            PutCMTIni("LABEL-EXIST", bSlotNo.ToString, "0")
        Next

        For bSlotNo = 1 To 10 Step 1
            ' 初期処理
            bHeadchk = False
            bFilechk = False
            bEndchk = False

            ' エラー時スキップ用Do
            Do
                ' 手差しフラグを立てる
                bHandFlg = True

                ' ＣＭＴ装置ステータス判定
                nRtn = FuncMtStat()
                If nRtn <> 0 Then
                    PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                    Exit Do
                End If

                ' ＣＭＴ装置オンライン処理
                nRtn = FuncMtOnline()
                If nRtn <> 0 Then
                    PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                    Exit Do
                End If

                ' パス格納
                strPath = GetCMTIni("READ-DIRECTORY", bSlotNo.ToString)

                'ファイル出力先先行判定
                If Not ChkReadDrFl(strPath, bSlotNo) Then
                    Exit Do
                End If

                ' 記録密度判定
                If Not ChkKirokumitsudo() Then
                    PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                    Return False
                End If

                '------------------------------------------
                'コード変換モード設定
                '------------------------------------------
                nMtRtn = mtebc(0)

                '------------------------------------------
                'ファイル読込処理
                '------------------------------------------
                Try
                    fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "HEAD"), FileMode.CreateNew)
                    ' ヘッダーラベル読込 
                    Do While True
                        ' バッファ領域クリア
                        Array.Clear(buff, 0, buff.Length)
                        ' 一ブロック読込
                        nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)

                        'LOG.Write("mtrblock", "ヘッドラベル読込", "読込バッファ長:" & len.ToString)
                        'デバイスステータス取得
                        nRtn = MtCHGStatus(nMtRtn, bit_status)

                        ' エラー判定
                        If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                            ' ログ出力
                            strLogwrite = "ＣＭＴ読込エラー（ヘッダーラベル）が起きました" & vbCrLf & _
                                        " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                        " エラーコード：" & nMtRtn.ToString
                            LOG.Write("ヘッダーラベル読込処理", "失敗", strLogwrite)
                            Call LogCmtStat()

                            PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                            fl.Close()
                            Exit Do    ' ヘッドラベル読込ループ脱出
                        End If
                        'テープマーク検出
                        If bit_status.BIT_TMK = 1 Then
                            fl.Close()
                            Exit Do    ' ヘッドラベル読込ループ脱出
                        End If

                        ' HEADLABELに書込
                        fl.Write(buff, 0, len)
                        ' ヘッダーラベル読込フラグ
                        bHeadchk = True
                    Loop
                Catch ex As Exception
                    LOG.Write("ヘッドラベル読込", "例外", ex.Message)
                    Exit Do    ' エラー脱出
                End Try

                Try

                    fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "READ"), FileMode.CreateNew)
                    ' ファイル読込
                    Do While True
                        ' バッファ領域クリア
                        Array.Clear(buff, 0, buff.Length)
                        ' 一ブロック読込み
                        nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)
                        'LOG.Write("mtrblock", "ファイル読込", "読込バッファ長:" & len.ToString)
                        'デバイスステータス取得
                        nRtn = MtCHGStatus(nMtRtn, bit_status)
                        ' エラー判定
                        If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                            ' ログ出力
                            strLogwrite = "ＣＭＴ読込エラー（ファイル）が起きました" & vbCrLf & _
                                        " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                        " エラーコード：" & nMtRtn.ToString
                            LOG.Write("読込処理", "失敗", strLogwrite)
                            Call LogCmtStat()
                            PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")

                            fl.Close()
                            Exit Do    ' ファイル読込ループ脱出
                        End If
                        ' テープマーク検出
                        If bit_status.BIT_TMK = 1 Then
                            fl.Close()
                            Exit Do    ' ファイル読込ループ脱出
                        End If
                        ' INPUTに書込
                        fl.Write(buff, 0, len)

                        ' ファイル読込フラグ
                        bFilechk = True
                    Loop

                Catch ex As Exception
                    LOG.Write("ファイル読込", "例外", ex.Message)
                    Exit Do
                End Try

                Try
                    ' ヘッダーラベル、ファイルを読込んだときのみ実行
                    If bHeadchk = True And bFilechk = True Then
                        fl = New FileStream(strPath & "\" & GetCMTIni("FILE-NAME", "END"), FileMode.CreateNew)
                        ' エンドラベル読込
                        Do While True
                            ' 一ブロック読込
                            nMtRtn = mtrblock(CMTINFO.UnitNo, buff(0), len, buff.Length)
                            'LOG.Write("mtrblock", "エンドラベル読込", "読込バッファ長:" & len.ToString)
                            'デバイスステータス取得
                            nRtn = MtCHGStatus(nMtRtn, bit_status)
                            ' エラー判定
                            If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
                                strLogwrite = "ＣＭＴ読込エラー（エンド）が起きました" & vbCrLf & _
                                            " ＤＴＥ：" & bit_status.BIT_DTE & " ＨＤＥ：" & bit_status.BIT_HDE & _
                                            " エラーコード：" & nMtRtn.ToString
                                LOG.Write("読込処理", "失敗", strLogwrite)
                                PutCMTIni("READ-RESULT", bSlotNo.ToString, "4")
                                fl.Close()
                                Exit Do    ' エンドラベル読込ループ脱出
                            End If
                            ' テープマーク検出
                            If bit_status.BIT_TMK = 1 Then
                                fl.Close()
                                Exit Do    ' エンドラベル読込ループ脱出
                            End If
                            ' ENDLABELに書込
                            fl.Write(buff, 0, len)
                            ' エンドラベル読込フラグ
                            bEndchk = True
                        Loop
                    End If

                Catch ex As Exception
                    LOG.Write("エンドラベル読込", "例外", ex.Message)
                    Exit Do
                End Try

                ' 読込ＣＭＴラベル有無判定
                nRtn = ChkLabel(strPath, nMtRtn, bSlotNo, bHeadchk, bFilechk, bEndchk)
                If nRtn <> 0 Then
                    LOG.Write("ファイル読み込み異常です", "HEADLABEL:" & bHeadchk & " INPUT:" & bFilechk & " ENDLABEL:" & bEndchk)
                    Exit Do
                End If

            Loop While False
            If bHandFlg Then
                Exit For    '手差し読込ならループ脱出
            End If

            If bSlotNo = bMemNo Then
                '------------------------------------------
                'ＣＭＴアンロード
                '------------------------------------------
                nMtRtn = mtstat(CMTINFO.UnitNo)
                nRtn = MtCHGStatus(nMtRtn, bit_status)
                If bit_status.BIT_NRDY = "0" Then
                    nMtRtn = mtunload(CMTINFO.UnitNo)
                    nRtn = MtCHGStatus(nMtRtn, bit_status)
                End If
                Exit For
            End If
        Next bSlotNo
        ' 読取結果判定
        If bHandFlg Then
            If Not "0" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Then
                Return 1
            End If
        Else
            nErrCnt = 0
            For bSlotNo = 1 To 10 Step 1
                ' 先行ファイルあり、読み込み異常がひとつでもあった場合は異常復帰する
                If "3" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Or "4" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Then
                    Return 1
                End If
                ' 全てのスロットにＣＭＴが入っていない、
                ' ＣＭＴにファイルが入っていなかった場合はエラーカウントアップする
                If "1" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Or "2" = GetCMTIni("READ-RESULT", bSlotNo.ToString) Then
                    nErrCnt = nErrCnt + 1
                End If
            Next
            LOG.Write("ErrCnt", nErrCnt.ToString)
            ' すべてエラーだった場合
            If nErrCnt = 10 Then
                Return 1
            End If
        End If
        Return 0
    End Function
#End Region
#Region "private FuncMtinit()"
    '
    ' 機　能 : ＣＭＴ装置初期関数
    '
    ' 戻り値 : 0 - 正常  1 - 異常
    '
    ' 引  数 : なし
    '
    ' 備　考 : 最初に呼び出す必要あり
    ' 
    Private Function FuncMtinit() As Integer
        Dim nMtRtn, nRtn As Integer    ' MT関数の復帰値

        '*** ASTAR.S.S 2008.05.28 ＰＣ起動直後ＣＭＴ動作不具合対応		***
        nMtRtn = mtstat(0)
        nMtRtn = mtstat(1)
        '******************************************************************

        ' mtinit処理
        nMtRtn = mtinit(CMTINFO)
        '*** ASTAR.S.S 2008.05.28 ＰＣ起動直後ＣＭＴ動作不具合対応		***
        Call mtonline(CMTINFO.UnitNo)
        '******************************************************************
        nRtn = MtCHGStatus(nMtRtn, bit_status)
#If Not DEBUGCMT Then
        If nMtRtn = 0 Or nMtRtn = &HFFFFFFFF Then
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("ＣＭＴ装置の初期処理", "失敗", strLogwrite)
            Return 1
        End If
#End If

        Return 0
    End Function
#End Region
#Region "private FuncMtStat()"
    '
    ' 機　能 : ＣＭＴステータス取得関数
    '
    ' 戻り値 : 0 - 正常  0以外 - 異常
    '
    ' 引  数 : なし
    '
    ' 備　考 : ＣＭＴテープを操作する前に呼び出す必要あり
    ' 
    Private Function FuncMtStat() As Integer
        Dim nMtRtn, nRtn As Integer    ' MT関数の復帰値

        ' mtstat処理(二回行う)
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        If nRtn <> 0 Then
            If bit_status.BIT_PRO = "1" OrElse _
                bit_status.BIT_BOT = "1" OrElse _
                bit_status.BIT_EOT = "1" OrElse _
                bit_status.BIT_TMK = "1" Then
                ' プロテクト、ＢＯＴ検出、ＥＯＴ検出、テープマーク検出時 リターン正常
                Return 0
            End If
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("ＣＭＴ装置のステータス取得", "失敗", strLogwrite)
            Return nRtn
        End If
        Return nRtn
    End Function
#End Region
#Region "private FuncMtOnline()"
    '
    ' 機　能 : ＣＭＴテープオンライン関数
    '
    ' 戻り値 : 0 - 正常  0以外 - 異常
    '
    ' 引  数 : なし
    '
    ' 備　考 : 
    ' 
    Private Function FuncMtOnline() As Integer

        Dim nMtRtn, nRtn As Integer    ' MT関数の復帰値

        ' mtonline処理
        nMtRtn = mtonline(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        If nMtRtn <> 0 Then
            strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
            LOG.Write("ＣＭＴ装置のオンライン処理", "失敗", strLogwrite)
            Return nMtRtn
        End If
        Return nMtRtn
    End Function
#End Region
#Region "private FuncMtrewind()"
    '
    ' 機　能 : ＣＭＴテープ巻き戻し関数
    '
    ' 戻り値 : 0 - 正常  0以外 - 異常
    '
    ' 引  数 : なし
    '
    ' 備　考 : すでにＣＭＴテープがロード状態の場合は、ＢＯＴ位置まで巻き戻す
    ' 
    Private Function FuncMtrewind() As Integer
        Dim nMtRtn, nRtn As Integer

        nMtRtn = mtrewind(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)
        If nMtRtn <> 0 Then
            strLogwrite = Setbit_statusLog(nMtRtn, nRtn)
            LOG.Write("ＣＭＴテープ巻き戻し処理", "失敗", strLogwrite)
            Return nMtRtn
        End If
        Return nMtRtn
    End Function
#End Region
#Region "private CslInfoInit()"
    '
    ' 機　能 : CSLDATA構造体の初期化
    '
    ' 戻り値 : 0 - 正常
    '
    ' 引  数 : なし
    '
    ' 備　考 : 
    ' 
    'Private Function CslInfoInit() As Integer
    '    LOADERDATA.CslModeCode = 2
    '    LOADERDATA.Drive = 0
    '    'LOADERDATA.SlotPosition = 0	'***修正 瀬戸 2008.11.13 書き込みが動かないので修正
    '    LOADERDATA.SlotPosition = 1		'***修正 瀬戸 2008.11.13 書き込みが動かないので修正
    '    LOADERDATA.Slot1 = 0
    '    LOADERDATA.Slot2 = 0
    '    LOADERDATA.Slot3 = 0
    '    LOADERDATA.Slot4 = 0
    '    LOADERDATA.Slot5 = 0
    '    LOADERDATA.Slot6 = 0
    '    LOADERDATA.Slot7 = 0
    '    LOADERDATA.Slot8 = 0
    '    LOADERDATA.Slot9 = 0
    '    LOADERDATA.Slot10 = 0
    '    Return 0
    'End Function
#End Region
#Region "private CmtInfoInit()"
    '
    ' 機　能 : MTINFOBLOCK構造体の初期化
    '
    ' 戻り値 : 0 - 正常
    '
    ' 引  数 : なし
    '
    ' 備　考 : 
    '   
    Private Function CmtInfoInit() As Integer
        CMTINFO.UnitNo = 0
        CMTINFO.HostNo = 0
        CMTINFO.TargetNo = 0
        CMTINFO.Vender = Array.CreateInstance(GetType(Char), 8)
        CMTINFO.Product = Array.CreateInstance(GetType(Char), 16)
        CMTINFO.VERSION = Array.CreateInstance(GetType(Char), 4)
        CMTINFO.Reserve = 0
        Return 0
    End Function
#End Region
#Region "private MtCHGStatus()"
    '
    ' 機　能 : ＣＭＴアクセス関数復帰値変換
    '          ビット判定の関数復帰値をバイト判定に変換する
    '
    ' 戻り値 : 0 - 正常  0以外 - 異常
    '
    ' 引  数 : ARG1 - ＣＭＴアクセス関数復帰値 ARG2 - 変換後バイト判定構造体
    '
    ' 備　考 : 
    '
    Private Function MtCHGStatus(ByVal Fskj_mt_rtn As Integer, ByRef Fskj_mt_BITrtn As DEVICE_status) As Integer

        Dim STATUS_BIT(31) As Byte    ' バイトデータ一時保存
        Dim count As Integer    ' カウンター

        ' エラークリア
        Err.Clear()

        MtCHGStatus = -1
        For count = 0 To 31
            'STATUS_BIT(count) = (Fskj_mt_rtn And (1L << count)) >> count
            STATUS_BIT(count) = Fskj_mt_rtn And &H1
            Fskj_mt_rtn = Fskj_mt_rtn >> 1

        Next
        Fskj_mt_BITrtn.BIT_TMK = CStr(Math.Abs(STATUS_BIT(0)))
        Fskj_mt_BITrtn.BIT_EOT = CStr(Math.Abs(STATUS_BIT(1)))
        Fskj_mt_BITrtn.BIT_BOT = CStr(Math.Abs(STATUS_BIT(2)))
        Fskj_mt_BITrtn.BIT_DEN0 = CStr(Math.Abs(STATUS_BIT(3)))
        Fskj_mt_BITrtn.BIT_DEN1 = CStr(Math.Abs(STATUS_BIT(4)))
        Fskj_mt_BITrtn.BIT_CSL = CStr(Math.Abs(STATUS_BIT(5)))
        Fskj_mt_BITrtn.BIT_PRO = CStr(Math.Abs(STATUS_BIT(6)))
        Fskj_mt_BITrtn.BIT_FIL1 = CStr(Math.Abs(STATUS_BIT(7)))
        Fskj_mt_BITrtn.BIT_DTE = CStr(Math.Abs(STATUS_BIT(8)))
        Fskj_mt_BITrtn.BIT_HDE = CStr(Math.Abs(STATUS_BIT(9)))
        Fskj_mt_BITrtn.BIT_NRDY = CStr(Math.Abs(STATUS_BIT(10)))
        Fskj_mt_BITrtn.BIT_ILC = CStr(Math.Abs(STATUS_BIT(11)))
        Fskj_mt_BITrtn.BIT_SCE = CStr(Math.Abs(STATUS_BIT(12)))
        Fskj_mt_BITrtn.BIT_UDC = CStr(Math.Abs(STATUS_BIT(13)))
        Fskj_mt_BITrtn.BIT_TIM = CStr(Math.Abs(STATUS_BIT(14)))
        Fskj_mt_BITrtn.BIT_CHC = CStr(Math.Abs(STATUS_BIT(15)))
        Fskj_mt_BITrtn.BIT_FIL2 = CStr(Math.Abs(STATUS_BIT(16)))
        Fskj_mt_BITrtn.BIT_FIL3 = CStr(Math.Abs(STATUS_BIT(17)))
        Fskj_mt_BITrtn.BIT_FIL4 = CStr(Math.Abs(STATUS_BIT(18)))
        Fskj_mt_BITrtn.BIT_FIL5 = CStr(Math.Abs(STATUS_BIT(19)))
        Fskj_mt_BITrtn.BIT_ROB = CStr(Math.Abs(STATUS_BIT(20)))
        Fskj_mt_BITrtn.BIT_FIL6 = CStr(Math.Abs(STATUS_BIT(21)))
        Fskj_mt_BITrtn.BIT_FIL7 = CStr(Math.Abs(STATUS_BIT(22)))
        Fskj_mt_BITrtn.BIT_FIL8 = CStr(Math.Abs(STATUS_BIT(23)))
        Fskj_mt_BITrtn.BIT_FIL9 = CStr(Math.Abs(STATUS_BIT(24)))
        Fskj_mt_BITrtn.BIT_FIL10 = CStr(Math.Abs(STATUS_BIT(25)))
        Fskj_mt_BITrtn.BIT_FIL11 = CStr(Math.Abs(STATUS_BIT(26)))
        Fskj_mt_BITrtn.BIT_FIL12 = CStr(Math.Abs(STATUS_BIT(27)))
        Fskj_mt_BITrtn.BIT_FIL13 = CStr(Math.Abs(STATUS_BIT(28)))
        Fskj_mt_BITrtn.BIT_FIL14 = CStr(Math.Abs(STATUS_BIT(29)))
        Fskj_mt_BITrtn.BIT_FIL15 = CStr(Math.Abs(STATUS_BIT(30)))
        Fskj_mt_BITrtn.BIT_FIL16 = CStr(Math.Abs(STATUS_BIT(31)))

        ' エラー判定
        If Err.Number <> 0 Then
            Return Err.Number
        Else
            Return 0
        End If
    End Function
#End Region
#Region "private LoaderMax()"
    '
    ' 機　能 : ＣＭＴマガジン内の最も番号が高いスロットを返却する
    '
    ' 戻り値 : 1-10 - 格納中の最大のスロット番号 1-10以外 - 異常、マガジンからロードしていない
    '
    ' 引  数 : なし
    '
    ' 備　考 : 
    '
    Private Function LoaderMax() As Byte
        Dim nMtRtn As Integer
        Dim slotno As Byte
        Dim slotsum As Integer

        nMtRtn = ChkCmtStat(slotno, slotsum)

        ' ＣＭＴテープがロードしていなかった
        If slotsum = 0 Then
            Return 0
        End If

        ' ＣＭＴテープが一つ読込まれていた
        If slotsum = 1 Then
            Return slotno
        End If

        Return 0
    End Function
#End Region
#Region "private ChkSlot()"
    '
    ' 機　能 : スロットポジション正誤判定
    '
    ' 戻り値 : true - 正常 false - 異常
    '
    ' 引  数 : ARG1 - スロット番号
    '
    ' 備　考 : 
    '
    Private Function ChkSlot(ByVal siteislotno As Byte) As Boolean
        Dim nRtn As Integer    ' 復帰情報
        Dim bSlotNo As Byte    ' スロット番号
        Dim nSlotSum As Integer    ' ＣＭＴテープ合計

        nRtn = ChkCmtStat(bSlotNo, nSlotSum)

        ' 手差しなら正誤確認はしない
        If nRtn = 1 And bSlotNo = 0 And nSlotSum = 1 Then
            Return True
        End If

        Return True
    End Function
#End Region
#Region "private ChkLabel()"
    '
    ' 機　能 : 読込ＣＭＴラベル有無判定
    '
    ' 戻り値 : 0 - 正常 0以外 - 異常
    '
    ' 引  数 : ARG1 - 判定先ディレクトリ
    '          ARG2 - ヘッダーラベルフラグ ARG3 - 読込ファイルフラグ ARG4 - エンドラベルフラグ
    '
    ' 備　考 : 
    '
    Private Function ChkLabel(ByVal pathread As String, ByVal mtrtn As Integer, ByVal slotno As Integer, ByVal headchk As Boolean, ByVal filechk As Boolean, ByVal endchk As Boolean) As Integer
        Dim Bbuff(256 - 1) As Byte ' ラベル判定読込バイトストリーム
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim sBlock As String = String.Empty
        'Dim sBlock As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim fl As FileStream

        ' ラベルありを読込んだ場合
        If headchk = True And filechk = True And endchk = True Then
            LOG.Write("三ブロック読込", "")
            PutCMTIni("READ-RESULT", slotno.ToString, "0")
            PutCMTIni("LABEL-EXIST", slotno.ToString, "1")
            Return 0
        End If

        ' ラベルなしを読込んだ場合
        If headchk = True And filechk = False And endchk = False Then
            Try
                ' 読込ファイルのチェック
                fl = New FileStream(Path.Combine(pathread, GetCMTIni("FILE-NAME", "HEAD")), FileMode.Open)
                Array.Clear(Bbuff, 0, Bbuff.Length)
                fl.Read(Bbuff, 0, 3)
                sBlock = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(Bbuff, 0, 3).Trim
                LOG.Write("strblocklen:", sBlock)
                '***ASTAR SUSUKI 2008.06.13             ***
                'EBCDIC対応                             ***
                If sBlock <> "VOL" Then
                    sBlock = System.Text.Encoding.GetEncoding("IBM290").GetString(Bbuff, 0, 3).Trim
                    LOG.Write("strblocklen:", sBlock)
                End If
                '******************************************
                fl.Close()
                'ヘッダーラベルが格納されていたら削除
                If sBlock = "VOL" Then
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
                    PutCMTIni("READ-RESULT", slotno.ToString, "2")
                    PutCMTIni("LABEL-EXIST", slotno.ToString, "0")
                    LOG.Write("ChkLabel", "ファイルがありませんでした")
                    Return 1
                End If
            Catch ex As Exception
                LOG.Write("strblocklen", sBlock)
                LOG.Write("chkLabel", "例外", ex.Message)
            End Try
            ' 正しい中身が格納されていたらリネームする
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "END"))
            Rename(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"), pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
            PutCMTIni("READ-RESULT", slotno.ToString, "0")
            PutCMTIni("LABEL-EXIST", slotno.ToString, "0")
            Return 0
        End If

        ' テープマークありを読込んだ場合
        If headchk = False And filechk = True And endchk = False Then
            Try
                ' 読込ファイルのチェック
                fl = New FileStream(Path.Combine(pathread, GetCMTIni("FILE-NAME", "READ")), FileMode.Open)
                Array.Clear(Bbuff, 0, Bbuff.Length)
                fl.Read(Bbuff, 0, 3)
                sBlock = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(Bbuff, 0, 3).Trim
                LOG.Write("strblocklen:", sBlock)
                '***ASTAR SUSUKI 2008.06.13             ***
                'EBCDIC対応                             ***
                If sBlock <> "VOL" Then
                    sBlock = System.Text.Encoding.GetEncoding("IBM290").GetString(Bbuff, 0, 3).Trim
                    LOG.Write("strblocklen:", sBlock)
                End If
                '******************************************
                fl.Close()
                'ヘッダーラベルが格納されていたら削除
                If sBlock = "VOL" Then
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
                    File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
                    PutCMTIni("READ-RESULT", slotno.ToString, "2")
                    PutCMTIni("LABEL-EXIST", slotno.ToString, "0")
                    LOG.Write("ChkLabel", "フォーマット異常です")
                    Return 0
                End If
            Catch ex As Exception
                LOG.Write("strblocklen", sBlock)
                LOG.Write("chkLabel", "例外", ex.Message)
            End Try
            ' 正しい中身が格納されていた
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "END"))
            PutCMTIni("READ-RESULT", slotno.ToString, "0")
            PutCMTIni("LABEL-EXIST", slotno.ToString, "2")
            Return 0
        End If

        ' 一ブロックも読込まなかった場合
        ' 空のファイルを削除する
        If headchk = False And filechk = False And endchk = False Then
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "HEAD"))
            File.Delete(pathread & "\" & GetCMTIni("FILE-NAME", "READ"))
            strLogwrite = "ＣＭＴ内にファイルはありませんでした" & vbCrLf & _
            "エラーコード：" & mtrtn.ToString
            LOG.Write("ＣＭＴ読取処理", "ファイルなし", strLogwrite)
            PutCMTIni("READ-RESULT", slotno.ToString, "2")
            Return 1
        End If

        If headchk = True And filechk = True And endchk = False Then    ' 二ブロック読込んだ場合
            strLogwrite = "エンドボリュームがありません" & vbCrLf & _
            "エラーコード：" & mtrtn.ToString
            LOG.Write("ＣＭＴ読取処理", "ファイル破損", strLogwrite)
            PutCMTIni("READ-RESULT", slotno.ToString, "4")
            Return 1
        End If
        Return 1
    End Function
#End Region
#Region "private ChkWriteDrFl()"
    '
    ' 機　能 : ＣＭＴテープロード、書込ファイル判定
    '
    ' 戻り値 : true - 正常 false - 異常
    '
    ' 引  数 : ARG1 - 判定先ディレクトリ ARG2 - スロット番号 ARG3 - ドライバーフラグ
    '          ARG4 - ヘッダーラベルフラグ ARG5 - 書込ファイルフラグ ARG6 - エンドラベルフラグ
    '
    ' 備　考 : 
    '
    Private Function ChkWriteDrFl(ByVal path As String, ByVal slotno As Byte, ByVal drive As Boolean, ByRef headlabel As Boolean, ByRef writefile As Boolean, ByRef endlabel As Boolean) As Boolean
        ' 対象ディレクトリ判定
        If Not Directory.Exists(path) Then
            strLogwrite = "ディレクトリなし" & vbCrLf & _
            "ディレクトリ：" & path
            LOG.Write("対象ディレクトリ判定", "失敗", strLogwrite)
            Return False
        End If

        ' 書込ヘッダーラベル判定
        headlabel = File.Exists(path & "\" & GetCMTIni("FILE-NAME", "HEAD"))

        ' 書込ファイル判定
        writefile = File.Exists(path & "\" & GetCMTIni("FILE-NAME", "WRITE"))

        ' 書込エンドラベル判定
        endlabel = File.Exists(path & "\" & GetCMTIni("FILE-NAME", "END"))

        LOG.Write("head:" & headlabel.ToString, "file:" & writefile.ToString, "end:" & endlabel.ToString)

        If drive Then
            ' ＣＭＴ装置にテープがロードされている
            If headlabel = True And writefile = True And endlabel = True Then
                ' ヘッダーラベルあり、ファイルあり、エンドラベルあり
                Return True
            ElseIf headlabel = False And writefile = True And endlabel = False Then
                'ファイルのみあり
                Return True
            Else
                ' それ以外はエラー
                If Not headlabel Then
                    strLogwrite = "書込ヘッダーラベルなし" & vbCrLf & _
                                "ファイル：" & path & "\" & GetCMTIni("FILE-NAME", "HEAD")
                    LOG.Write("書込ファイル判定", "失敗", strLogwrite.ToString)
                End If
                If Not writefile Then
                    strLogwrite = "書込ファイルなし" & vbCrLf & _
                                "ファイル：" & path & "\" & GetCMTIni("FILE-NAME", "WRITE")
                    LOG.Write("書込ファイル判定", "失敗", strLogwrite.ToString)
                End If
                If Not endlabel Then
                    strLogwrite = "書込エンドラベルなし" & vbCrLf & _
                    "ファイル：" & path & "\" & GetCMTIni("FILE-NAME", "END")
                    LOG.Write("書込ファイル判定", "失敗", strLogwrite.ToString)
                End If
                PutCMTIni("WRITE-RESULT", slotno.ToString, "2")
                Return False
            End If
        Else
            ' ＣＭＴテープをロードしていない
            LOG.Write("書込ファイル判定", "失敗", "ＣＭＴテープがロードされていない")

            '  ヘッダーラベルなし
            If Not headlabel Then
                strLogwrite = "書込ヘッダーラベルなし" & vbCrLf & _
                            "ファイル：" & path & "\" & GetCMTIni("FILE-NAME", "HEAD")
                LOG.Write("書込ファイル判定", "失敗", strLogwrite.ToString)
            End If
            ' ファイルなし
            If Not writefile Then
                strLogwrite = "書込ファイルなし" & vbCrLf & _
                            "ファイル：" & path & "\" & GetCMTIni("FILE-NAME", "WRITE")
                LOG.Write("書込ファイル判定", "失敗", strLogwrite.ToString)
            End If
            ' エンドラベルなし
            If Not endlabel Then
                strLogwrite = "書込エンドラベルなし" & vbCrLf & _
                "ファイル：" & path & "\" & GetCMTIni("FILE-NAME", "END")
                LOG.Write("書込ファイル判定", "失敗", strLogwrite.ToString)
            End If
            Return False
        End If
    End Function
#End Region
#Region "private ChkReadDrFl()"
    '
    ' 機　能 : 先行ファイル有無判定
    '
    ' 戻り値 : true - 正常 false - 異常
    '
    ' 引  数 : ARG1 - 判定先ディレクトリ ARG2 - ヘッダーラベルフラグ
    '          ARG3 - 読込ファイルフラグ ARG4 - エンドラベルフラグ
    '
    ' 備　考 : 
    '
    Private Function ChkReadDrFl(ByVal path As String, ByVal slotno As Byte) As Boolean
        ' 対象ディレクトリ判定
        If Not Directory.Exists(path) Then
            strLogwrite = "ディレクトリなし" & vbCrLf & _
            "ディレクトリ：" & path
            LOG.Write("対象ディレクトリ判定", "失敗", strLogwrite)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If

        ' 先行ファイル判定
        If File.Exists(path & "\" & GetCMTIni("FILE-NAME", "READ")) Then
            strLogwrite = "先行ファイル有り" & vbCrLf & _
            "ファイル：" & path
            LOG.Write("先行ファイル判定", "失敗", strLogwrite.ToString)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If

        ' 先行ヘッダーラベル判定
        If File.Exists(path & "\" & GetCMTIni("FILE-NAME", "HEAD")) Then
            strLogwrite = "先行ヘッダーラベル有り" & vbCrLf & _
            "ファイル：" & path
            LOG.Write("先行ファイル判定", "失敗", strLogwrite.ToString)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If

        ' 先行エンドラベル判定
        If File.Exists(path & "\" & GetCMTIni("FILE-NAME", "END")) Then
            strLogwrite = "先行エンドラベル有り" & vbCrLf & _
            "ファイル：" & path
            LOG.Write("先行ファイル判定", "失敗", strLogwrite.ToString)
            PutCMTIni("READ-RESULT", slotno.ToString, "3")
            Return False
        End If
        Return True
    End Function
#End Region
#Region "private ChkKirokumitsudo()"
    '
    ' ＣＭＴテープの記録密度を測定する
    '
    Private Function ChkKirokumitsudo() As Boolean
        Dim nMtRtn, nRtn As Integer
        nMtRtn = mtstat(CMTINFO.UnitNo)
        nRtn = MtCHGStatus(nMtRtn, bit_status)

        '記録密度判定
        Select Case bit_status.BIT_DEN0
            Case 1
                If bit_status.BIT_DEN1 = 1 Then
                    strMemMitsudo = "6250BPI"
                Else
                    strMemMitsudo = "1600BPI"
                End If
            Case 0
                If bit_status.BIT_DEN1 = 1 Then
                    strMemMitsudo = "3200BPI"
                Else
                    strMemMitsudo = "800BPI"
                End If
            Case Else
                strLogwrite = "ステータス変化エラー(記録密度判定)" & vbCrLf & _
                "ステータス：" & bit_status.BIT_DEN0 & bit_status.BIT_DEN1 & _
                "エラーコード：" & nMtRtn.ToString
                LOG.Write("記録密度測定", "失敗", strLogwrite)
                strLogwrite = Setbit_statusLog(nMtRtn, nRtn)
                LOG.Write(strLogwrite, "")
                Return False
        End Select
        Return True
    End Function
#End Region
#Region "private LogCmtStat()"
    ' 現在のＣＭＴステータスをログに出す
    Private Function LogCmtStat() As Integer
        Dim nRtn As Integer = 0
        Dim nMtRtn As Integer
        Dim cflg As Byte = 0

        Call CmtInfoInit()
        'Call CslInfoInit()

        'mtinit
        nMtRtn = FuncMtinit()
        If nMtRtn <> 0 Then
            nRtn = nMtRtn
        End If

        'mtstat
        nMtRtn = FuncMtStat()
        If nMtRtn <> 0 Then
            nRtn = nMtRtn
        End If

        nRtn = MtCHGStatus(nMtRtn, bit_status)

        strLogwrite = SetCmtInfoLog(nMtRtn, nRtn)
        LOG.Write("LogCmtStat()", "CMTINFO", strLogwrite)

        strLogwrite = Setbit_statusLog(nMtRtn, nRtn)
        LOG.Write("LogCmtStat()", "bit_status", strLogwrite)

        Return nMtRtn
    End Function
#End Region
#Region "private SetCmtInfoLog()"
    ' CMTINFOのログを作成
    Private Function SetCmtInfoLog(ByVal nMtRtn As Integer, ByVal nRtn As Integer) As String
        Return "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & " nRtn:" & String.Format("{0}:{0:X}", nRtn) & " UnitNo:" & CMTINFO.UnitNo & _
            " HostNo:" & CMTINFO.HostNo & " TargetNo:" & CMTINFO.TargetNo & _
            " Vender:" & CMTINFO.Vender & " Product:" & CMTINFO.Product & _
            " VERSION:" & CMTINFO.VERSION & " reserve:" & CMTINFO.Reserve
    End Function
#End Region
#Region "private SetCslDataLog()"
    ' LOADERDATAのログを作成
    'Private Function SetCslDataLog(ByVal nMtRtn As Integer, ByVal nRtn As Integer) As String
    '    Return "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & " nRtn:" & String.Format("{0}:{0:X}", nRtn) & _
    '        " CslModeCode:" & LOADERDATA.CslModeCode & " Drive:" & LOADERDATA.Drive & " SlotPosition:" & LOADERDATA.SlotPosition & _
    '        " Slot1:" & LOADERDATA.Slot1 & " Slot2:" & LOADERDATA.Slot2 & " Slot3:" & LOADERDATA.Slot3 & " Slot4:" & LOADERDATA.Slot4 & _
    '        " Slot5:" & LOADERDATA.Slot5 & " Slot6:" & LOADERDATA.Slot6 & " Slot7:" & LOADERDATA.Slot7 & " Slot8:" & LOADERDATA.Slot8 & _
    '        " Slot9:" & LOADERDATA.Slot9 & " Slot10:" & LOADERDATA.Slot10
    'End Function
#End Region
#Region "private Setbit_statusLog()"
    ' bit_statusのログを作成
    Private Function Setbit_statusLog(ByVal nMtRtn As Integer, ByVal nRtn As Integer) As String
        Return "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & " nRtn:" & String.Format("{0}:{0:X}", nRtn) & _
            " TMK:" & bit_status.BIT_TMK & " EOT:" & bit_status.BIT_EOT & " BOT:" & bit_status.BIT_BOT & _
            " DEN0:" & bit_status.BIT_DEN0 & " DEN1:" & bit_status.BIT_DEN1 & " CSL:" & bit_status.BIT_CSL & _
            " PRO:" & bit_status.BIT_PRO & " FIL1:" & bit_status.BIT_FIL1 & " DTE:" & bit_status.BIT_DTE & _
            " HDE:" & bit_status.BIT_HDE & " NRDY:" & bit_status.BIT_NRDY & " ILC:" & bit_status.BIT_ILC & _
            " SCE:" & bit_status.BIT_SCE & " UDC:" & bit_status.BIT_UDC & " TIM:" & bit_status.BIT_TIM & _
            " CHC:" & bit_status.BIT_CHC & " FIL2:" & bit_status.BIT_FIL2 & " FIL3:" & bit_status.BIT_FIL3 & _
            " FIL4:" & bit_status.BIT_FIL4 & " FIL5:" & bit_status.BIT_FIL5 & " ROB:" & bit_status.BIT_ROB & _
            " FIL6:" & bit_status.BIT_FIL6 & " FIL7:" & bit_status.BIT_FIL7 & " FIL8:" & bit_status.BIT_FIL8 & _
            " FIL9:" & bit_status.BIT_FIL9 & " FIL10:" & bit_status.BIT_FIL10 & " FIL11:" & bit_status.BIT_FIL11 & _
            " FIL12:" & bit_status.BIT_FIL12 & " FIL13:" & bit_status.BIT_FIL13 & " FIL14:" & bit_status.BIT_FIL14 & _
            " FIL15:" & bit_status.BIT_FIL15 & " FIL16:" & bit_status.BIT_FIL16
    End Function
#End Region

    '
    ' 機　能 : ＣＭＴ装置の接続状態を取得
    '
    ' 戻り値 : TRUE - 正常  FALSE - 異常
    '
    ' 引  数 : なし
    '
    ' 備　考 : 最初に呼び出す必要あり
    ' 
    Public Function ChkLoader() As Boolean
        Dim nMtRtn As Integer    ' ＣＭＴ関数の復帰値

        'LOG.SyoriMei = "ChkLoader"

        ' 接続確認
        nMtRtn = mtinit(CMTINFO)
#If Not DEBUGCMT Then
        If nMtRtn = 0 Or nMtRtn = &HFFFFFFFF Then
            LOG.Write("ＣＭＴ装置が接続状態にありません", "mtinit:" & nMtRtn.ToString)
            Return False
        End If
#End If
        Return True
    End Function

    '#Region "単体書込"

    '    ' 
    '    ' 単体テープマーク書込
    '    '
    '    ' 復帰値： デバイスステータス
    '    ' 

    '    Public Function writeTMK() As Integer
    '        Dim nRtn As Integer    ' 関数復帰値
    '        Dim nMtRtn As Integer    ' ＣＭＴ関数復帰値


    '        ' ＣＭＴ装置初期処理
    '        nRtn = FuncMtinit()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        ' ＣＭＴ装置ステータス処理
    '        nRtn = FuncMtStat()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        nRtn = FuncMtOnline()
    '        If nRtn <> 0 Then
    '            Return 1

    '        End If

    '        nMtRtn = mtwtmk(CMTINFO.UnitNo)

    '        Call LogCmtStat()

    '        Return nMtRtn

    '    End Function

    '    ' 
    '    ' 単体ダブルテープマーク書込
    '    '
    '    ' 復帰値： デバイスステータス
    '    ' 

    '    Public Function writeWTMK() As Integer
    '        Dim nRtn As Integer    ' 関数復帰値
    '        Dim nMtRtn As Integer    ' ＣＭＴ関数復帰値


    '        ' ＣＭＴ装置初期処理
    '        nRtn = FuncMtinit()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        ' ＣＭＴ装置ステータス処理
    '        nRtn = FuncMtStat()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        nMtRtn = mtwmtmk(CMTINFO.UnitNo)

    '        Call LogCmtStat()

    '        Return nMtRtn

    '    End Function


    '    ' 
    '    ' 単体ダブルテープマーク書込
    '    '
    '    ' 復帰値： デバイスステータス
    '    ' 

    '    Public Function writeBlock(ByVal slotno As Byte, ByVal filename As String) As Integer
    '        Dim nRtn As Integer    ' 関数復帰値
    '        Dim nMtRtn As Integer    ' ＣＭＴ関数復帰値
    '        Dim sPath As String
    '        Dim fl As FileStream
    '        Dim buff(&HF000 - 1) As Byte ' 書込バイトストリーム
    '        Dim count As Integer    ' ファイル読み込み時の文字列長


    '        sPath = GetCMTIni("WRITE-DIRECTORY", slotno.ToString)


    '        ' ＣＭＴ装置初期処理
    '        nRtn = FuncMtinit()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        ' ＣＭＴ装置ステータス処理
    '        nRtn = FuncMtStat()
    '        If nRtn <> 0 Then
    '            Return 1
    '        End If

    '        Try
    '            If Not File.Exists(Path.Combine(sPath, filename)) Then
    '                LOG.Write("書込処理", "失敗", Path.Combine(sPath, filename) & "がありません")
    '                PutCMTIni("WRITE-RESULT", slotno.ToString, "4")
    '                Return 1
    '            End If

    '            ' ヘッダーラベル書込
    '            fl = New FileStream(Path.Combine(sPath, filename), FileMode.Open)
    '            ' バッファ領域クリア
    '            Array.Clear(buff, 0, buff.Length)
    '            ' 書込ファイルを読込む
    '            count = fl.Read(buff, 0, buff.Length)
    '            Do While count > 0
    '                ' 一ブロック書込
    '                LOG.Write("", "通過")
    '                nMtRtn = mtwblock(CMTINFO.UnitNo, buff(0), count)
    '                LOG.Write("書込", "nMtRtn:" & nMtRtn.ToString)

    '                LOG.Write("mtwblock", "返却ファイル書込", "書込バッファ長:" & count.ToString)
    '                'デバイスステータス取得
    '                nRtn = MtCHGStatus(nMtRtn, bit_status)
    '                'エラー判定
    '                If bit_status.BIT_DTE = "1" Or bit_status.BIT_HDE = "1" Then
    '                    'ログ出力
    '                    LOG.Write("ノンラベル返却ファイル書込異常が起きました", _
    '                            "nMtRtn:" & String.Format("{0}:{0:X}", nMtRtn) & _
    '                            " DTE:" & bit_status.BIT_DTE & " HDE:" & bit_status.BIT_HDE, strLogwrite)
    '                    PutCMTIni("WRITE-RESULT", slotno.ToString, "4")
    '                    Call LogCmtStat()
    '                    fl.Close()
    '                    Exit Do
    '                End If
    '                ' バッファ領域をクリア
    '                Array.Clear(buff, 0, buff.Length)
    '                '返却ファイルをバッファに読込む
    '                count = fl.Read(buff, 0, buff.Length)
    '            Loop
    '            fl.Close()

    '        Catch ex As Exception
    '            LOG.Write("書込処理例外", "ヘッダーラベル", ex.Message)
    '            Call LogCmtStat()
    '            PutCMTIni("WRITE-RESULT", slotno.ToString, "4")
    '            Return False
    '        End Try

    '        Call LogCmtStat()

    '        Return nMtRtn

    '    End Function
    '#End Region

    Public Sub New()
        mnBlockSize = -1
    End Sub
End Class