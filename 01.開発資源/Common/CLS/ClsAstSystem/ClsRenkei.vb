Imports System
Imports System.IO
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch

' 各システムとの連携用クラス
Public Class ClsRenkei
    ' 入力ファイル名
    Public InFileName As String
    ' 出力ファイル名
    Public OutFileName As String
    ' メッセージ
    Public Message As String = ""

    ' パラメータ共通情報
    Private mInfoArgument As CommData
    Public ReadOnly Property InfoArg() As CommData
        Get
            Return mInfoArgument
        End Get
    End Property

    '' ＣＭＴ他シス フォルダ
    '   ＣＭＴ
    Public Shared ReadOnly CMTOTHERPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "CMTREAD")
    Public Shared ReadOnly CMTOTHERWRTPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "CMTWRITE")
    '   学校
    Public Shared ReadOnly SCHOOLOTHERPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "SCHOOLREAD")
    ' メディアコンバータ
    Public Shared ReadOnly MEDIAOTHERPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "MEDIAREAD")
    Public Shared ReadOnly MEDIAOTHERWRTPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "MEDIAWRITE")
    ' 汎用エントリ
    Public Shared ReadOnly HANREADPATH As String = CASTCommon.GetFSKJIni("OTHERSYS", "HANREAD")

    Public Shared ReadOnly TXTPATH As String = CASTCommon.GetFSKJIni("COMMON", "TXT")

    Public Shared ReadOnly OTHJIFFOLDER As String = "JIFURI\JFR"            ' 口振
    Public Shared ReadOnly OTHSOKFOLDER As String = "SOUKYUFURI\SFR"        ' 振込

    ' ベースフォルダ
    Private Shared BasePath As String = CASTCommon.GetFSKJIni("TOUROKU", "PATH")

    ' 連携区分 フォルダ名
    '' メディアコンバータ
    Public Shared MEDIAPATH As String = Path.Combine(BasePath, "I_MEDIA")
    '' 汎用エントリ
    Public Shared HENTRYPATH As String = Path.Combine(BasePath, "I_H-ENTRY")
    '' 学校
    Public Shared SCHOOLPATH As String = Path.Combine(BasePath, "I_SCHOOL")
    '' ＣＭＴ
    Public Shared CMTPATH As String = Path.Combine(BasePath, "I_CMT")
    '' 伝送
    Public Shared DENSOUPATH As String = Path.Combine(BasePath, "II_DENSOU")
    '' 個別
    Public Shared KOBETUPATH As String = Path.Combine(BasePath, "II_KOBETU")

    '' 金バッチ
    Public Shared KBPATH As String = Path.Combine(BasePath, "III_KINBATCH")

    '' ＳＳＣ
    Public Shared SSCPATH As String = Path.Combine(BasePath, "III_SSC")

    ' データ取得フォルダ
    Public Shared ReadOnly GETFOLDER As String = "GET"
    ' データ分解後フォルダ
    Public Shared ReadOnly HOLDFOLDER As String = "HOLD"
    ' データ保存正常フォルダ
    Public Shared ReadOnly NORFOLDER As String = "NORMAL"
    ' データ保存異常フォルダ
    Public Shared ReadOnly ERRFOLDER As String = "ERROR"
    ' データ返還フォルダ
    Public Shared ReadOnly SENDFOLDER As String = "SEND"

    ' 振替処理区分 フォルダ名
    Public Shared ReadOnly JIFFOLDER As String = "JIF"       ' 口振
    Public Shared ReadOnly SOKFOLDER As String = "SOK"       ' 振込
    Public Shared ReadOnly SSSFOLDER As String = "SKD"       ' ＳＳＳ

    Public Shared ReadOnly EncdJ As System.Text.Encoding = System.Text.Encoding.GetEncoding("SHIFT-JIS")   'JISエンコード
    Public Shared ReadOnly EncdE As System.Text.Encoding = System.Text.Encoding.GetEncoding("IBM290")  'EBCDICエンコード

    Private mRenkeiMode As Integer = 0

    ' ＣＯＭＰＬＯＣＫ暗号化情報
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
            Case 0                      ' 落し込み処理
                AppName = "TOUROKU"
            Case 1                      ' 結果更新処理
                AppName = "KEKKA"
            Case 2                      ' 返還処理
                AppName = "HENKAN"
            Case 3                      ' 資金決済結果更新処理
                AppName = "KESSAIKEKKA"
            Case 4                      ' 総給振結果更新処理
                AppName = "SOU_KEKKA"
            Case Else                   ' 落し込み処理
                AppName = "TOUROKU"
        End Select
        BasePath = CASTCommon.GetFSKJIni(AppName, "PATH")

        ' 連携区分 フォルダ名
        '' メディアコンバータ
        MEDIAPATH = Path.Combine(BasePath, "I_MEDIA")
        '' 汎用エントリ
        HENTRYPATH = Path.Combine(BasePath, "I_H-ENTRY")
        '' 学校
        SCHOOLPATH = Path.Combine(BasePath, "I_SCHOOL")
        '' ＣＭＴ
        CMTPATH = Path.Combine(BasePath, "I_CMT")
        '' 伝送
        DENSOUPATH = Path.Combine(BasePath, "II_DENSOU")
        '' 個別
        KOBETUPATH = Path.Combine(BasePath, "II_KOBETU")
        '' 金バッチ
        KBPATH = Path.Combine(BasePath, "III_KINBATCH")
    End Sub

    ' 機能　 ： New
    '
    ' 引数   ： ARG1 - 共通情報
    '           ARG2 - 連携モード（０：登録，配信，１：結果更新，２：返還）
    '
    ' 戻り値 ： エラー番号
    '
    ' 備考　 ： 0=成功、50=ファイルなし、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
    '        ： 300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
    '
    Sub New(ByVal comm As CommData, Optional ByVal RenkeiMode As Integer = 0)
        MyClass.New(RenkeiMode)

        Message = ""
        OutFileName = ""

        mInfoArgument = comm

        '2009/09/08.暫定対応　連携区分とりのぞく +++++++
        'If RenkeiMode = 1 AndAlso comm.INFOParameter.RENKEI_KBN = "99" Then
        '    ' 結果更新処理かつ，連携区分の上１桁が９の場合
        'Select Case comm.INFOParameter.FMT_KBN
        '    Case "MT"
        '        ' センタ不能データ
        '        InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DAT"), "FUNOU.DAT")
        '        Return
        '    Case "20", "21"
        '        ' ＳＳＳデータ
        '        InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DAT"), "FUNOU_SYUKINDAIKOU.DAT")
        '        Return
        '    Case Else
        '2009/09/14 仮コメント化======================================
        'InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DAT"), comm.INFOParameter.RENKEI_FILENAME)
        'Return
        '======================================
        'End Select
        'End If
        '+++++++++++++++++++++++++++++++++++++++++++++++
        '+++++++++++++++++++++++++++++++++++++++++++++++
        '2009/09/12 暫定的に不能結果ファイルを返すように変更
        If RenkeiMode = 1 Then
            ' 結果更新処理
            Select Case comm.INFOParameter.FMT_KBN
                Case "MT"
                    InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DEN"), comm.INFOParameter.RENKEI_FILENAME)
                    Return
                    '2017/01/18 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                Case "20", "21"
                    InFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DEN"), comm.INFOParameter.RENKEI_FILENAME)
                    Return
                    '2017/01/18 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
                Case Else
                    InFileName = ""
                    Return
            End Select
        End If
        '===================================================
        If RenkeiMode = 2 Then
            ' 返還処理 ファイル名
            InFileName = Path.Combine(GetFolderName(SENDFOLDER), "")
            Return
        End If

        If RenkeiMode >= 3 AndAlso RenkeiMode <= 6 Then
            ' 金バッチ結果ファイル名
            InFileName = Path.Combine(GetFolderName(GETFOLDER), comm.INFOParameter.RENKEI_FILENAME)
            Return
        End If

        '2009/09/08.暫定対応　連携区分とりのぞく +++++++
        'If (comm.INFOParameter.RENKEI_KBN = "99" Or comm.INFOParameter.RENKEI_KBN = "88") Then

        ' 落とし込み処理
        ' 入力ファイル名
        'Select Case mInfoArgument.INFOParameter.BAITAI_CODE
        '    Case "00"       ' 伝送の場合
        '        ' ファイル名設定
        '        InFileName = GetKobetuFileName("DAT")
        '    Case Else
        '        ' ファイル名設定
        '        InFileName = GetKobetuFileName("DAT")
        'End Select
        'Else
        If comm.INFOParameter.RENKEI_FILENAME.Trim <> "" Then
            '2009/09/08.暫定対応　連携区分とりのぞく +++++++
            'If ",02,00,07,20".IndexOf(comm.INFOParameter.RENKEI_KBN) >= 1 Then
            If comm.INFOParameter.BAITAI_CODE = "00" OrElse _
                comm.INFOParameter.BAITAI_CODE = "06" OrElse _
                comm.INFOParameter.BAITAI_CODE = "07" Then
                ' ＣＭＴ，伝送，学校
                InFileName = Path.Combine(GetFolderName(GETFOLDER), comm.INFOParameter.RENKEI_FILENAME.Trim)
            Else
                InFileName = Path.Combine(GetFolderName(HOLDFOLDER), comm.INFOParameter.RENKEI_FILENAME.Trim)
            End If
            '+++++++++++++++++++++++++++++++++++++++++++++++
        Else
            ' 入力ファイル名
            Select Case mInfoArgument.INFOParameter.BAITAI_CODE
                Case "00"       ' 伝送の場合
                    ' ファイル名設定
                    InFileName = GetKobetuFileName("DAT")
                Case Else
                    ' ファイル名設定
                    InFileName = GetKobetuFileName("DAT")
            End Select
        End If
        'End If
        '+++++++++++++++++++++++++++++++++++++++++++++++
    End Sub

    '
    ' 機能　 ： 依頼データをコピーする
    '
    ' 戻り値 ： エラー番号
    '
    ' 備考　 ： 0=成功、50=ファイルなし、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
    '        ： 300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
    '
    Public Function CopyToDisk(ByVal readfmt As CAstFormat.CFormat) As Integer
        Dim nRet As Integer = 0          '処理結果
        Dim sWorkFilename As String 'ワークファイル名

        If mInfoArgument.INFOParameter.RENKEI_FILENAME.Trim = "" Then

            '出力ファイル名
            OutFileName = Path.Combine(BasePath, String.Format("i{0}.dat", mInfoArgument.INFOParameter.TORI_CODE))

            '入力ファイル名
            'ファイルの存在チェック
            ' 2017/11/27 タスク）綾部 CHG 但馬信金(落込処理でﾌｧｲﾙなしエラーが発生(SMB2.0)<ME>) -------------------- START
            'If File.Exists(InFileName) = False Then
            '    Message = "ファイルが存在しません[ファイル名：" & InFileName & "]"
            '    Return 50
            'End If
            For i As Integer = 1 To 40 Step 1
                If File.Exists(InFileName) = True Then
                    Threading.Thread.Sleep(500)
                    Exit For
                Else
                    If i = 40 Then
                        Message = "ファイルが存在しません。[ファイル名：" & InFileName & "] (" & i & ")"
                        Return 50
                    End If
                    Threading.Thread.Sleep(500)
                End If
            Next
            ' 2017/11/27 タスク）綾部 CHG 但馬信金(落込処理でﾌｧｲﾙなしエラーが発生(SMB2.0)<ME>) -------------------- END

            'ローカルへコピーする
            sWorkFilename = CopyFileToWork(InFileName, OutFileName, _
                                            mInfoArgument.INFOParameter.JOBTUUBAN)
            If File.Exists(sWorkFilename) = False Then
                'コピー失敗
                Message = "コピー失敗[ファイル名：" & InFileName & ":" & OutFileName & "]"
                Return 400
            End If
        Else
            ' 2017/11/27 タスク）綾部 CHG 但馬信金(落込処理でﾌｧｲﾙなしエラーが発生(SMB2.0)<ME>) -------------------- START
            'If File.Exists(InFileName) = False Then
            '    Message = "ファイルが存在しません[ファイル名：" & InFileName & "]"
            '    Return 50
            'End If
            For i As Integer = 1 To 40 Step 1
                If File.Exists(InFileName) = True Then
                    Threading.Thread.Sleep(500)
                    Exit For
                Else
                    If i = 40 Then
                        Message = "ファイルが存在しません[ファイル名：" & InFileName & "] (" & i & ")"
                        Return 50
                    End If
                    Threading.Thread.Sleep(500)
                End If
            Next
            ' 2017/11/27 タスク）綾部 CHG 但馬信金(落込処理でﾌｧｲﾙなしエラーが発生(SMB2.0)<ME>) -------------------- END

            OutFileName = Path.Combine(Path.GetDirectoryName(InFileName), String.Format("{0}_{1}.dat", _
                                        mInfoArgument.INFOParameter.TORI_CODE, _
                                        Path.GetFileName(InFileName)) _
                                        )
            '不能ファイルの場合はファイルの作成先を変更
            If Path.GetFileName(InFileName) = "FUNOU.DAT" Then
                OutFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DENBK"), "FUNOU_JIS.DAT")
                '2017/01/18 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            ElseIf Path.GetFileName(InFileName) = "FUNOU_SYUKINDAIKOU.DAT" Then
                OutFileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DENBK"), "FUNOU_SYUKINDAIKOU_JIS.DAT")
                '2017/01/18 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
            End If
            sWorkFilename = CopyFileToWork(InFileName, OutFileName, mInfoArgument.INFOParameter.JOBTUUBAN)
            If File.Exists(sWorkFilename) = False Then
                'コピー失敗
                Message = "コピー失敗[ファイル名：" & InFileName & ":" & OutFileName & "]"
                Return 400
            End If
        End If

        'データ読み込み
        Try
            '出力
            If readfmt.FirstRead(sWorkFilename) = 0 Then
                Message = readfmt.Message
                Return 10
            End If
            If readfmt.IsEBCDIC = False Then
                '落とし込み
                If readfmt.CRLF = True Then

                    '2017/05/23 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- START
                    If mRenkeiMode = 0 AndAlso _
                       mInfoArgument.INFOParameter.CODE_KBN <> "1" AndAlso _
                       mInfoArgument.INFOParameter.CODE_KBN <> "2" AndAlso _
                       mInfoArgument.INFOParameter.CODE_KBN <> "3" Then
                        ''2009/09/18.Sakon　コード区分変更に伴う修正 +++++++++++++++++
                        'If mRenkeiMode = 0 AndAlso _
                        '   mInfoArgument.INFOParameter.CODE_KBN <> "1" AndAlso _
                        '   mInfoArgument.INFOParameter.CODE_KBN <> "2" Then
                        '    'mInfoArgument.INFOParameter.CODE_KBN <> "3" Then
                        '    'If mRenkeiMode = 0 AndAlso _
                        '    '    mInfoArgument.INFOParameter.CODE_KBN <> "2" Then
                        '    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        '2017/05/23 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- END

                        Return 200        'コード区分異常（JIS改行あり）
                    End If

                    ' 改行を取り除く
                    Dim fmtWrite As New CAstFormat.CFormat(OutFileName, readfmt.RecordLen)
                    fmtWrite.OpenWriteFile(OutFileName)
                    Do Until readfmt.EOF()
                        ' １行データを読み込む
                        readfmt.GetFileData()
                        ' 書き込む
                        fmtWrite.WriteData(readfmt.RecordData)
                    Loop
                    fmtWrite.Close()
                    fmtWrite.Dispose()
                    fmtWrite = Nothing
                    readfmt.Close()
                Else

                    '2009/09/18.Sakon　コード区分変更に伴う修正 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    If mRenkeiMode = 0 AndAlso _
                        mInfoArgument.INFOParameter.CODE_KBN <> "0" AndAlso _
                        mInfoArgument.INFOParameter.CODE_KBN <> "3" AndAlso _
                        mInfoArgument.INFOParameter.CODE_KBN <> "4" Then
                        'If mRenkeiMode = 0 AndAlso _
                        '    mInfoArgument.INFOParameter.CODE_KBN <> "0" AndAlso mInfoArgument.INFOParameter.CODE_KBN <> "3" Then
                        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                        Return 300        'コード区分異常（JIS改行なし）
                    End If

                    File.Copy(sWorkFilename, OutFileName, True)
                End If
                'Else
                '    ' 結果更新
                '    File.Copy(sWorkFilename, OutFileName, True)
                'End If
            Else
                ' プログラムで変換する場合は，こっち
                'File.Copy(sWorkFilename, OutFileName, True)

                ' FTRANを使用して変換する場合はこっち
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
    '2009/11/30 ファイルの文字コードチェック
    Public Function CheckCode(ByVal FileName As String, ByVal CodeKbn As String, ByRef ErrMessage As String) As Integer

        Dim Breader As New System.IO.FileStream(FileName, IO.FileMode.Open, IO.FileAccess.Read)
        Dim Bin As Integer
        Dim XBin As String = Nothing
        Dim I As Integer

        '1byte読込
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
                    ErrMessage = "文字コード異常"
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
            Message = "ファイルが存在しません[ファイル名：" & InFileName & "]"
            Return InFileName
        End If

        'Dim sWorkFilename As String = Path.Combine(GetFolderName(GETFOLDER), Path.GetFileName(InFileName))
        'If Directory.Exists(GetFolderName(GETFOLDER)) = False Then
        '    Directory.CreateDirectory(GetFolderName(GETFOLDER))
        'End If
        'File.Copy(InFileName, sWorkFilename, True)

        Dim sWorkFilename As String = CopyFileToWork(InFileName, Path.Combine(GetFolderName(GETFOLDER), ""), mInfoArgument.INFOParameter.JOBTUUBAN)
        If File.Exists(sWorkFilename) = False Then
            ' コピー失敗
            Message = "コピー失敗[ファイル名：" & InFileName & ":" & sWorkFilename & "]"
            Return InFileName
        End If

        Return sWorkFilename
    End Function

    '
    ' 機能　 ： 個別 入力ファイル名取得
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
    '
    Private Function GetKobetuFileName(ByVal pathIni As String) As String
        Dim nRet As Integer = 0
        ' 入力パス
        Dim sFilename As String = CASTCommon.GetFSKJIni("COMMON", pathIni)
        'If Not mInfoArgument.INFOToriMast.FILE_NAME_T Is Nothing AndAlso mInfoArgument.INFOToriMast.FILE_NAME_T.Trim <> "" Then
        '    ' 入力ファイル名
        '    sFilename &= mInfoArgument.INFOToriMast.FILE_NAME_T
        'Else

        sFilename &= String.Format("F{0}.DAT", mInfoArgument.INFOParameter.TORI_CODE)

        'やめ2009.10.23
        'Select Case mInfoArgument.INFOParameter.FMT_KBN
        '    Case "00", "20", "21"
        '        ' 全銀，ＳＳＳ
        '        sFilename &= String.Format("KD{0}.120", mInfoArgument.INFOParameter.TORI_CODE)
        '    Case "06"
        '        ' 地公体（３００）
        '        sFilename &= String.Format("KD{0}.300", mInfoArgument.INFOParameter.TORI_CODE)
        '    Case "01"
        '        ' 地公体（３５０）
        '        sFilename &= String.Format("KD{0}.350", mInfoArgument.INFOParameter.TORI_CODE)
        '    Case Else
        '        ' 
        '        sFilename &= String.Format("KD{0}.dat", mInfoArgument.INFOParameter.TORI_CODE)
        'End Select

        'If Not mInfoArgument.INFOToriMast.FILE_NAME_T Is Nothing AndAlso mInfoArgument.INFOToriMast.FILE_NAME_T.Equals("") = False Then
        '    If File.Exists(mInfoArgument.INFOToriMast.FILE_NAME_T) = False AndAlso _
        '        (mInfoArgument.INFOToriMast.MULTI_KBN_T = "1" Or mInfoArgument.INFOToriMast.MULTI_KBN_T = "3") Then
        '        ' 入力ファイルがない かつ 複数ヘッダの場合
        '        '副コード01のファイル名をstrIN_FILE_NAMEに設定する(及び他行以外のマルチであること)
        '        Dim aLength As Integer = mInfoArgument.INFOToriMast.FILE_NAME_T.Length
        '        sFilename = sFilename.Substring(0, aLength - 6) & "01" & sFilename.Substring(aLength - 4, 4)
        '    End If
        'End If

        'End If

        Return sFilename
    End Function

    '
    ' 機能　 ： 入力ファイルを出力ファイルのフォルダへコピーする
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
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
                ' コピー
                File.Copy(inname, sDestFileName, False)
                Exit For
            Catch ex As UnauthorizedAccessException
                Return ""                       '出力ファイル作成失敗
            Catch ex As ArgumentException
                Return ""                       '出力ファイル作成失敗
            Catch ex As PathTooLongException
                Return ""                       '出力ファイル作成失敗
            Catch ex As FileNotFoundException
                Return ""                       '出力ファイル作成失敗
            Catch ex As DirectoryNotFoundException
                Directory.CreateDirectory(Path.GetDirectoryName(outname))
            Catch ex As IOException
                ' ファイルがコピーできるまで繰り返す
                Threading.Thread.Sleep(128)
            Catch ex As Exception
                Return ""                       '出力ファイル作成失敗
            End Try
        Next nCounter

        If File.Exists(sDestFileName) = False Then
            Return ""
        End If

        Return sDestFileName
    End Function

    '
    ' 機能　 ： 入力ファイルを出力ファイルのフォルダへコピーする
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
    '
    Public Function FileAnalyze() As String()
        Dim HoldPath As String

        ' 連携区分,振替処理区分，フォーマット区分から保存フォルダ名を取得
        HoldPath = GetFolderName(HOLDFOLDER)

        '媒体読み込み
        'フォーマット　
        Dim oReadFMT As CAstFormat.CFormat = Nothing
        Try
            ' フォーマット区分から，フォーマットを特定する
            oReadFMT = CAstFormat.CFormat.GetFormat(mInfoArgument.INFOParameter)
            If oReadFMT Is Nothing Then
                ' 連携失敗
                Return New String() {}
            End If
        Catch ex As Exception
            Message = ex.Message
        End Try

        ' ファイル名一覧
        Dim retFiles As New ArrayList(100)

        Dim LineCount As Integer = 0

        ' ファイル内容読み込み
        Dim fmtWrite As CAstFormat.CFormat = Nothing
        If oReadFMT.FirstRead(InFileName) = 1 Then
            ' 正常
            Do Until oReadFMT.EOF
                oReadFMT.GetFileData()
                LineCount += 1
                If oReadFMT.IsHeaderRecord = True AndAlso fmtWrite Is Nothing Then
                    Call oReadFMT.CheckRecord1()
                    Dim FormatKbn As String = GetFormatKbn(oReadFMT.InfoMeisaiMast.SYUBETU_CODE, oReadFMT.InfoMeisaiMast.ITAKU_CODE)

                    ' ヘッダレコード で ， ファイルを作成
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

                ' 書き込む
                fmtWrite.WriteData(oReadFMT.RecordData)
                If oReadFMT.CRLF = True Then
                    fmtWrite.WriteCrLf()
                End If

                If Not fmtWrite Is Nothing AndAlso (oReadFMT.IsEndRecord = True OrElse oReadFMT.EOF = True) Then
                    ' エンドレコードで， ファイルを閉じる
                    fmtWrite.Close()
                    fmtWrite.Dispose()
                    fmtWrite = Nothing
                End If
            Loop
        Else
            ' 異常
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

    ' 機能　 ： 取引先マスタの振替処理区分を取得
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 振込＿取引先マスタを参照して，存在する場合は "2" を返す
    '           上記以外は，"1" を返す
    '
    Private Function GetFormatKbn(ByVal SyubetuCode As String, ByVal itakucode As String) As String
        Dim SQL As New System.Text.StringBuilder(128)
        Dim OraDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Try
            SQL.Append("SELECT FMT_KBN_T FROM TORIMAST")
            SQL.Append(" WHERE ITAKU_CODE_T = " & CASTCommon.SQ(itakucode))
            '2008/04/17 種別コード追加
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
            Message = "フォーマット区分が見つかりません"
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
    ' 機能　 ： 入力ファイルを出力ファイルのフォルダへコピーする
    '
    ' 引数   ： ARG1 - 入力ファイル名（Cxxxxx.a.bbbbbbb.cc.dat） a:振替処理区分，b:取引先主コード，c:取引先副コード
    '           ARG2 - 出力ファイル名
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
    '
    Public Function FileDecodeMove() As String
        Dim GetPath As String

        ' 連携区分,振替処理区分，フォーマット区分から保存フォルダ名を取得
        GetPath = GetFolderName(GETFOLDER)

        Dim Key() As String = InFileName.Split("."c)
        If Key.Length < 4 Then
            Message = "不正なファイル名です"
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

                ' 口振の取引先マスタが無い場合は，振込の取引先マスタを検索
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
                Message = "取引先マスタが取得できません"
                Return ""
            End If

            Complock.KEY = OraReader.GetString("ENC_KEY1_T").PadRight(64, " "c)
            Complock.IVKEY = OraReader.GetString("ENC_KEY2_T").PadRight(32, " "c)
            Complock.AES = OraReader.GetString("ENC_OPT1_T")
            Select Case Complock.AES
                Case "0"
                    ' ＡＥＳなし
                    Complock.KEY = Complock.KEY.Substring(0, 16)
                    Complock.IVKEY = Complock.IVKEY.Substring(0, 16)
                Case "1"
                    ' ＡＥＳ鍵長１２８ビット
                    Complock.KEY = Complock.KEY.Substring(0, 32)
                    Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
                Case "2"
                    ' ＡＥＳ鍵長１９２ビット
                    Complock.KEY = Complock.KEY.Substring(0, 48)
                    Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
                Case "3"
                    ' ＡＥＳ鍵長２５６ビット
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
                Message = "COMPLOCK II 複合エラー[" & nRet.ToString & "]"
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
    ' 機能　 ： 入力ファイルを出力ファイルのフォルダへコピーする
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
    '
    Public Function FileDecodeMove(ByVal recLen As String, ByVal aes As String, ByVal encodeKey As String, ByVal ivKey As String) As String
        Dim GetPath As String

        ' 連携区分,振替処理区分，フォーマット区分から保存フォルダ名を取得
        GetPath = GetFolderName(GETFOLDER)

        Dim Complock As stCOMPLOCK
        Complock.KEY = encodeKey.PadRight(32)
        Complock.IVKEY = ivKey.PadRight(64)
        Complock.AES = aes
        Select Case Complock.AES
            Case "0"
                ' ＡＥＳなし
                Complock.KEY = Complock.KEY.Substring(0, 16)
                Complock.IVKEY = Complock.IVKEY.Substring(0, 16)
            Case "1"
                ' ＡＥＳ鍵長１２８ビット
                Complock.KEY = Complock.KEY.Substring(0, 32)
                Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
            Case "2"
                ' ＡＥＳ鍵長１９２ビット
                Complock.KEY = Complock.KEY.Substring(0, 48)
                Complock.IVKEY = Complock.IVKEY.Substring(0, 32)
            Case "3"
                ' ＡＥＳ鍵長２５６ビット
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
            Message = "COMPLOCK II 複合エラー[" & nRet.ToString & "]"
            Return ""
        End If

        File.Delete(Path.ChangeExtension(OutFileName, ".DAT"))
        File.Move(OutFileName, Path.ChangeExtension(OutFileName, ".DAT"))
        OutFileName = Path.ChangeExtension(OutFileName, ".DAT")

        File.Delete(InFileName)

        Return OutFileName
    End Function

    '
    ' 機能　 ： 入力ファイルを出力ファイルのフォルダへコピーする
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
    '
    Public Function FileMove() As String
        Return FileCopy(False)
    End Function

    '
    ' 機能　 ： 入力ファイルを出力ファイルのフォルダへコピーする
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
    '
    Public Function FileCopy(Optional ByVal copyMode As Boolean = True) As String
        Dim GetPath As String

        ' 連携区分,振替処理区分，フォーマット区分から保存フォルダ名を取得
        GetPath = GetFolderName(GETFOLDER)

        ' ＣＭＴフォルダから，一括処理用フォルダへ移動
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
    ' 機能　 ： 汎用フォルダ内の対象ファイル一覧を取得する
    '
    ' 引数   ： ARG1 - 対象ファイル一覧
    '
    ' 戻り値 ： 対象ファイル一覧
    '
    ' 備考　 ： 
    '
    Public Function GetHanyouFiles(ByVal files() As String, ByVal ptn As String) As String()
        Dim HoldPath As String
        Dim ArrayFiles As New ArrayList(files)

        ' 連携区分,振替処理区分，フォーマット区分から保存フォルダ名を取得
        HoldPath = GetFolderName(HOLDFOLDER)

        ArrayFiles.AddRange(Directory.GetFiles(HoldPath & "\", ptn))

        Return CType(ArrayFiles.ToArray(GetType(String)), String())
    End Function

    '
    ' 機能　 ： 汎用フォルダ内の対象ファイル一覧を取得する
    '
    ' 引数   ： ARG1 - 対象ファイル一覧
    '
    ' 戻り値 ： 対象ファイル一覧
    '
    ' 備考　 ： 
    '
    Public Function GetHanyouOthreFiles(ByVal files() As String, ByVal ptn As String) As String()
        Dim HoldPath As String
        Dim ArrayFiles As New ArrayList(files)

        ' 連携区分,振替処理区分，フォーマット区分から保存フォルダ名を取得
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
    ' 機能　 ： FTRAN PLUS
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '           ARG3 - 変換定義ファイル
    '
    ' 戻り値 ： 
    '
    ' 備考　 ： 
    '
    Private Function ConvertFtranPlus(ByVal infile As String, ByVal outfile As String, ByVal teigi As String) As Integer
        Dim nRet As Integer

        ' EBCDIC 変換
        Dim strCMD As String
        Dim strTEIGI_FIEL As String

        Dim sFtranPPath As String = CASTCommon.GetFSKJIni("COMMON", "FTR")

        strTEIGI_FIEL = sFtranPPath & teigi

        '*** 修正 mitsu 2008/09/30 EBCDICデータ対応のため大幅修正 ***
        'strCMD = "FP /nwd/ cload " & sFtranPPath & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & infile & " " & outfile & " ++" & strTEIGI_FIEL
        '処理区分が自振かつ、連携区分が落し込みである場合
        '(センター直接持込フォーマットは専用のPファイルがあるためそちらで変換する(EBCDICデータは使わない))
        If mInfoArgument.INFOParameter.FSYORI_KBN = "1" AndAlso mRenkeiMode = 0 AndAlso Not mInfoArgument.INFOParameter.FMT_KBN = "TO" Then
            'フォーマット区分からレコード長取得
            Dim RecLen As Integer = GetFormatLength()

            'レコード前半部JIS、後半部EBCDICの倍固定長データに変換する
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
            ' コード変換失敗
            Return 100
        End If
        'End If

        Return 0
    End Function

    ' 機能　 ： FTRAN PLUS
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '           ARG3 - 変換定義ファイル
    '
    ' 戻り値 ： 
    '
    ' 備考　 ： 
    '
    Public Function ConvertToEBCDIC(ByVal infile As String, ByVal outfile As String, ByVal teigi As String) As Integer
        Dim nRet As Integer

        ' EBCDIC 変換
        Dim strCMD As String
        Dim strTEIGI_FIEL As String

        Dim sFtranPPath As String = CASTCommon.GetFSKJIni("COMMON", "FTR")

        strTEIGI_FIEL = sFtranPPath & teigi

        '*** 修正 mitsu 2008/09/30 FP不要 ***
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
            ' コード変換失敗
            Return 100
        End If

        Return 0
    End Function

    ' 機能　 ： FTRAN PLUS
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 出力ファイル名
    '           ARG3 - 変換定義ファイル
    '
    ' 戻り値 ： 
    '
    ' 備考　 ： 
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
            ' コード変換失敗
            Return 100
        End If

        Return 0
    End Function

    '
    ' 機能　 ： データ取得用フォルダを取得する
    '
    ' 引数   ： ARG1 - フォルダ名
    '
    ' 戻り値 ： フォルダ名
    '
    ' 備考　 ： 
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

        '2009/09/08.暫定対応　連携区分とりのぞく(個別のみ) +++++++
        '' 連携区分
        'Select Case mInfoArgument.INFOParameter.RENKEI_KBN
        '    Case "01"       ' メディアコンバータ
        '        sPath = Path.Combine(MEDIAPATH, foldername)
        '    Case "09"       ' 汎用エントリ
        '        sPath = Path.Combine(HENTRYPATH, foldername)
        '    Case "02"       ' ＣＭＴ
        '        sPath = Path.Combine(CMTPATH, foldername)
        '    Case "07"       ' 学校
        '        sPath = Path.Combine(SCHOOLPATH, foldername)
        '    Case "00"       ' 伝送
        '        sPath = Path.Combine(DENSOUPATH, foldername)
        '    Case "KB"       ' 金バッチ
        '        sPath = Path.Combine(KBPATH, foldername)
        '    Case "SC"       ' ＳＳＣ
        '        sPath = Path.Combine(SSCPATH, foldername)
        '    Case Else       ' 個別
        sPath = Path.Combine(KOBETUPATH, foldername)
        'End Select
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        ' 振替処理区分
        Select Case fSyoriKbn
            Case "1"            ' 口振
                sPath &= "\" & JIFFOLDER
            Case "3"            ' 振込
                sPath &= "\" & SOKFOLDER
            Case "2"            ' SSS
                sPath &= "\" & SSSFOLDER
            Case Else
                sPath &= "\" & JIFFOLDER
        End Select

        ' フォーマット区分
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
    ' 機能　 ： データ取得用フォルダを取得する
    '
    ' 引数   ： ARG1 - フォルダ名
    '
    ' 戻り値 ： フォルダ名
    '
    ' 備考　 ： 
    '
    Private Function GetFolderName(ByVal foldername As String) As String
        Return GetFolderName(foldername, mInfoArgument.INFOParameter.FSYORI_KBN)
    End Function

    ' 機能　 ： ファイルを正常フォルダへコピーする
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 備考　 ： 
    '
    Public Function CopyToNormal(ByVal filename As String) As String
        Return MoveFile(filename, NORFOLDER, True)
    End Function

    ' 機能　 ： ファイルを異常フォルダへコピーする
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 備考　 ： 
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

    ' 機能　 ： ファイルを異常フォルダへコピーする
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 備考　 ： 
    '
    Public Function CopyToError(ByVal filename As String) As String
        Return MoveFile(filename, ERRFOLDER, True)
    End Function

    ' 機能　 ： ファイルを正常フォルダへ移動する
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 備考　 ： 
    '
    Public Function MoveToNormal(ByVal filename As String) As String
        Return MoveFile(filename, NORFOLDER)
    End Function

    ' 機能　 ： ファイルを異常フォルダへ移動する
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 備考　 ： 
    '
    Public Function MoveToError(ByVal filename As String) As String
        Return MoveFile(filename, ERRFOLDER)
    End Function

    ' 機能　 ： ファイルを異常フォルダへコピーする
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 備考　 ： 
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

    ' 機能　 ： ファイルを正常フォルダへ移動する
    '
    ' 引数   ： ARG1 - ファイル名
    '
    ' 備考　 ： 
    '
    Public Function MoveToGet(ByVal filename As String) As String
        Return MoveFile(filename, GETFOLDER)
    End Function

    ' 機能　 ： ファイルを指定したフォルダへ移動する
    '
    ' 引数   ： ARG1 - ファイル名
    '           ARG2 - フォルダ名
    '
    ' 備考　 ： 
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
            '　ファイルコピー
            File.Copy(filename, DestFile, True)
        Else
            ' ファイル移動「
            File.Move(filename, DestFile)
        End If

        Return DestFile
    End Function

    ' 機能　 ： ファイルの拡張子を変更する
    '
    ' 引数   ： ARG1 - ファイル名
    '           ARG2 - 変更後拡張子
    '
    ' 備考　 ： 
    '
    Public Function ChangeExtension(ByVal filename As String, ByVal ext As String) As String
        Dim DestFile As String = Path.ChangeExtension(filename, ext)
        If File.Exists(DestFile) = True Then
            File.Move(DestFile, Path.ChangeExtension(DestFile, String.Format(".{0:HHmmss}.", Date.Now) & Path.GetExtension(DestFile)))
        End If
        Call File.Move(filename, DestFile)

        Return DestFile
    End Function

    ' 機能　 ： 取得ファイルの移動
    '
    ' 備考　 ： 
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

    ' 機能　 ： ＣＭＴフォルダ対象ファイルを取得
    '
    ' 備考　 ： 
    '
    Public Function GetCMTFiles() As String()
        Return GetFiles(GETFOLDER)
    End Function

    ' 機能　 ： 学校フォルダ対象ファイルを取得
    '
    ' 備考　 ： 
    '
    Public Function GetSchoolFiles() As String()
        Return GetFiles(GETFOLDER)
    End Function

    ' 機能　 ： 対象ファイルを取得
    '
    ' 備考　 ： 
    '
    Private Function GetFiles(ByVal foldername As String, Optional ByVal searchPattern As String = "*.DAT") As String()
        Dim GetPath As String

        GetPath = GetFolderName(foldername)

        If Directory.Exists(GetPath) = False Then
            Call Directory.CreateDirectory(GetPath)
        End If

        Return Directory.GetFiles(GetPath, searchPattern)
    End Function

    ' 機能　 ： ＣＭＴ媒体変換システムへの出力パスを取得
    '
    ' 備考　 ： 
    '
    Public Function GetCMTOtherWritePath() As String
        Dim GetPath As String

        ' ＣＭＴ書き込みフォルダ
        GetPath = CMTOTHERWRTPATH

        ' 振替処理区分
        Select Case mInfoArgument.INFOParameter.FSYORI_KBN
            Case "1"            ' 口振
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
            Case "3"            ' 振込
                GetPath = Path.Combine(GetPath, OTHSOKFOLDER)
            Case Else
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
        End Select

        ' フォーマット区分(CMT連携のフォルダは長さがフォルダ名になっている）
        GetPath &= GetFormatLength.ToString.PadLeft(3, "0"c) & "\"

        If Directory.Exists(GetPath) = False Then
            Call Directory.CreateDirectory(GetPath)
        End If

        Return GetPath
    End Function

    ' 機能　 ： ＣＭＴ媒体変換システムからの対象ファイルを取得
    '
    ' 備考　 ： 
    '
    Public Function GetCMTOtherFiles() As String()
        Dim GetPath As String

        GetPath = CMTOTHERPATH

        ' 振替処理区分
        Select Case mInfoArgument.INFOParameter.FSYORI_KBN
            Case "1"            ' 口振
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
            Case "3"            ' 振込
                GetPath = Path.Combine(GetPath, OTHSOKFOLDER)
            Case "10"
                ' 他行結果の場合
                GetPath = Path.Combine(GetPath, "TAKOU")
                Return Directory.GetFiles(GetPath)
            Case Else
                GetPath = Path.Combine(GetPath, OTHJIFFOLDER)
        End Select

        ' フォーマット区分(CMT連携のフォルダは長さがフォルダ名になっている）
        GetPath &= GetFormatLength.ToString.PadLeft(3, "0"c)

        If Directory.Exists(GetPath) = False Then
            Call Directory.CreateDirectory(GetPath)
        End If

        Return Directory.GetFiles(GetPath, "*.DAT")
    End Function

    ' 機能　 ： 学校自振システムからの対象ファイルを取得
    '
    ' 備考　 ： 
    '
    Public Function GetSCHOOLOtherFiles() As String()
        Dim GetPath As String

        GetPath = SCHOOLOTHERPATH

        Return Directory.GetFiles(GetPath, "*FD")
    End Function

    ' 機能　 ： メディアコンバータシステムへの出力パスを取得
    '
    ' 備考　 ： 
    '
    Public Function GetMEDIAOtherWritePath() As String
        Dim GetPath As String

        ' メディアコンバータ書き込みフォルダ
        GetPath = MEDIAOTHERWRTPATH

        Return GetPath
    End Function

    ' 機能　 ： メディアコンバータシステムからの対象ファイルを取得
    '
    ' 備考　 ： 
    '
    Public Function GetMEDIAOtherFiles() As String()
        Dim GetPath As String

        GetPath = MEDIAOTHERPATH

        If Directory.Exists(GetPath) = False Then
            Dim str() As String = {}
            Return str
        End If

        ' 振替処理区分
        Dim RetFiles() As String
        Dim FilesList As New ArrayList
        Dim WildFile As String
        Select Case mInfoArgument.INFOParameter.FSYORI_KBN
            Case "1"            ' 口振
                ' 自振ファイルの追加
                WildFile = CASTCommon.GetFSKJIni("TOUROKU", "JIF_FILE") & "*"
                RetFiles = Directory.GetFiles(GetPath, WildFile)
                For i As Integer = 0 To RetFiles.Length - 1
                    If Path.GetFileName(RetFiles(i)).EndsWith("000") = False Then
                        If File.Exists(RetFiles(i)) = True Then
                            FilesList.Add(RetFiles(i))
                        End If
                    End If
                Next i

                ' 集金代行ファイルの追加
                WildFile = CASTCommon.GetFSKJIni("TOUROKU", "SKD_FILE") & "*"
                RetFiles = Directory.GetFiles(GetPath, WildFile)
                For i As Integer = 0 To RetFiles.Length - 1
                    If Path.GetFileName(RetFiles(i)).EndsWith("000") = False Then
                        If File.Exists(RetFiles(i)) = True Then
                            FilesList.Add(RetFiles(i))
                        End If
                    End If
                Next i
            Case "3"            ' 振込
                ' 集金代行ファイルの追加
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

    ' 機能　 ： 汎用エントリシステムからの対象ファイルを取得
    '
    ' 備考　 ： 
    '
    Public Function GetHanyouOtherSystemFiles() As String()
        Dim GetPath As String

        ' 汎用エントリデータ読み取りフォルダ
        GetPath = HANREADPATH

        If Directory.Exists(GetPath) = False Then
            Dim str() As String = {}
            Return str
        End If

        ' 振替処理区分
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

    ' 機能　 ： ＣＯＭＰＬＯＣＫを使用してファイルを復号化
    '
    ' 備考　 ： 
    '
    Private Function DecodeComplock(ByVal complock As stCOMPLOCK, ByVal outfile As String) As Long
        Dim Arg As String

        Dim sComplockPath As String = CASTCommon.GetFSKJIni("COMMON", "COMPLOCK")
        If File.Exists(sComplockPath) = True Then
            Message = "COMPLOCKプログラムが見つかりません"
            Return -199
        End If

        ' 引数組み立て
        Dim DQ As String = """"
        Arg = " -I "
        Arg &= DQ & InFileName & DQ
        Arg &= " -O " & DQ & outfile & DQ ' 出力先
        Arg &= " -k " & DQ & complock.KEY & DQ                      ' 鍵
        Arg &= " -v " & DQ & complock.IVKEY & DQ                    ' IV
        Arg &= " -lf 0"

        'File.Copy(InFileName, outfile)
        'Return 0

        Dim ProcFT As New Process
        Try
            ' 復号化実行
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(sComplockPath, "decode.exe")
            ProcInfo.Arguments = Arg
            ProcInfo.WorkingDirectory = sComplockPath
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            If ProcFT.ExitCode = 19 Then
                ' DECODE-019 ヘッダーレコード長に誤りがあります。
                ' ヘッダ固定長にて，もう一度復号化を試みる
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

    ' 機能　 ： ＣＯＭＰＬＯＣＫを使用してファイルを暗号化
    '
    ' 備考　 ： 
    '
    Private Function EncodeComplock(ByVal complock As stCOMPLOCK, ByVal outfile As String) As Long
        Dim Arg As String

        Dim sComplockPath As String = CASTCommon.GetFSKJIni("COMMON", "COMPLOCK")
        If File.Exists(sComplockPath) = True Then
            Message = "COMPLOCKプログラムが見つかりません"
            Return -199
        End If

        ' 引数組み立て
        Dim DQ As String = """"
        Arg = " -I "
        Arg &= DQ & InFileName & DQ
        Arg &= " -l " & DQ & complock.RECLEN & DQ
        Arg &= " -O " & DQ & outfile & DQ ' 出力先
        If complock.AES = "0" Then
            ' ＡＥＳなし
            Arg &= " -a 5"
            Arg &= " -n 256"
        Else
            ' ＡＥＳ
            Arg &= " -a  8"             ' -rl を指定しない場合は, -a 6 となる
            Arg &= " -m  1 "
            Arg &= " -ak 1 "
            Arg &= " -p  1"
        End If
        Arg &= " -rl " & DQ & complock.RECLEN & DQ
        Arg &= " -k " & DQ & complock.KEY & DQ                      ' 鍵
        Arg &= " -v " & DQ & complock.IVKEY & DQ                    ' IV
        Arg &= " -t 0"
        Arg &= " -g 1"

        ' 暗号化実行
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

    ' 機能　 ： フォーマット区分からレコード長を取得
    '
    ' 備考　 ： 
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

    ' 機能　 ： ファイルを他シス連携フォルダへ移動
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - 振替処理区分
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
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

    ' 機能　 ： ファイルを暗号化する
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - キー（取引先主コード，取引先副コード，振替日）
    '           
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： スケジュールマスタからキー情報を取得
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
                Message = "スケジュールマスタが取得できません"
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

    ' 機能　 ： ファイルを暗号化する
    '
    ' 引数   ： ARG1 - 入力ファイル名
    '           ARG2 - キー（取引先主コード，取引先副コード）
    '           
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 取引先マスタからキー情報を取得
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
                Message = "取引先マスタが取得できません"
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

    ' 機能　 ： ファイルを暗号化する
    '
    ' 引数   ： ARG1 - ＣＯＭＰＬＯＣＫ情報
    '           
    '
    ' 戻り値 ： ファイル名
    '
    ' 備考　 ： 
    '
    Private Function FileEncode(ByVal complock As stCOMPLOCK) As String
        Select Case complock.AES
            Case "0"
                ' ＡＥＳなし
                complock.KEY = complock.KEY.Substring(0, 16)
                complock.IVKEY = complock.IVKEY.Substring(0, 16)
            Case "1"
                ' ＡＥＳ鍵長１２８ビット
                complock.KEY = complock.KEY.Substring(0, 32)
                complock.IVKEY = complock.IVKEY.Substring(0, 32)
            Case "2"
                ' ＡＥＳ鍵長１９２ビット
                complock.KEY = complock.KEY.Substring(0, 48)
                complock.IVKEY = complock.IVKEY.Substring(0, 32)
            Case "3"
                ' ＡＥＳ鍵長２５６ビット
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

        ' 暗号化
        Dim nRet As Long = EncodeComplock(complock, OutFileName)
        If nRet = -199 Then
            Return ""
        ElseIf nRet <> 0 Then
            Message = "COMPLOCK II 暗号エラー[" & nRet.ToString & "]"
            Return ""
        End If

        Return OutFileName
    End Function

    '読替リストに該当する委託者コードを書き換える(全銀フォーマット専用)
    Public Function ConvertItakuCode(ByVal filename As String, ByRef Log As CASTCommon.BatchLOG) As Boolean
        'ファイル存在チェック
        If File.Exists(filename) = False Then
            Return True
        End If

        If File.Exists(Path.Combine(TXTPATH, "委託者コード読替.txt")) = False Then
            Return True
        End If

        '委託者コード読替リスト
        Dim ItakuCodeList As Hashtable = New Hashtable

        '委託者コード読替一覧を取得する
        Dim sw As StreamReader = Nothing
        Try
            sw = New StreamReader(Path.Combine(TXTPATH, "委託者コード読替.txt"), EncdJ)
            '1行目無視
            sw.ReadLine()

            While sw.Peek > -1
                Dim line() As String = sw.ReadLine.Split(","c)
                '読替前種別コード・委託者コードをキーとする
                ItakuCodeList.Add(line(0) & line(1), New String() {line(2), line(3)})
            End While

        Catch ex As Exception
            Log.Write("委託者コード読替リスト取得", "失敗", ex.Message)
            Message = "委託者コード読替リスト取得失敗"
            Return False

        Finally
            If Not sw Is Nothing Then
                sw.Close()
            End If
        End Try

        '委託者コード読替メイン処理
        Dim fs As FileStream = Nothing
        Try
            'ファイルを読み書き両用で開く
            fs = New FileStream(filename, FileMode.Open, FileAccess.ReadWrite)

            Dim Rec(0) As Byte              'レコード区分格納用
            Dim Len As Integer = 0          '改行コードの長さ
            Dim Hed As Byte                 'ヘッダレコードのデータ区分
            Dim Enc As System.Text.Encoding 'エンコーディング

            'ファイル先頭1バイトを読み取りエンコード判定
            fs.Read(Rec, 0, 1)

            Select Case Rec(0)
                Case 49 'SJIS.GetBytes("1"c)(0)に該当
                    Hed = 49
                    Enc = EncdJ

                    '121バイト目までシーク
                    fs.Seek(120, SeekOrigin.Begin)

                    '改行コード判定(改行コード分のバイト長も判定)
                    While Len < 2
                        fs.Read(Rec, 0, 1) '1バイト読み取り

                        Select Case Rec(0)
                            Case 50, 56 'EncdJ.GetBytes("2"c)(0) EncdJ.GetBytes("8"c)(0)に該当
                                'データ・トレーラレコードの場合は終了
                                Exit While
                            Case Else
                                'データ・トレーラレコードでない場合は改行コード長増加
                                Len += 1
                        End Select
                    End While

                Case 241 'EncdE.GetBytes("1"c)(0)に該当
                    Hed = 241
                    Enc = EncdE

                Case Else
                    Log.Write("エンコード判定", "失敗", "ファイル名：" & filename)
                    Message = "エンコード判定失敗 ファイル名：" & filename
                    Return False
            End Select

            'ファイル先頭までシーク
            fs.Seek(0, SeekOrigin.Begin)

            'エンコード・改行コード長が分かったのでレコード読み取り
            While fs.Position < fs.Length
                '先頭1バイトを読み取り
                fs.Read(Rec, 0, 1)

                Select Case Rec(0)
                    Case Hed
                        'ヘッダレコードである場合
                        Dim Header(12) As Byte
                        '13バイト読み取りする
                        fs.Read(Header, 0, 13)

                        Dim Syubetu As String = Enc.GetString(Header, 0, 2)     '種別
                        Dim CodeKbn As String = Enc.GetString(Header, 2, 1)     'コード区分
                        Dim ItakuCode As String = Enc.GetString(Header, 3, 10)  '委託者コード

                        '委託者コードチェック
                        If ItakuCodeList.ContainsKey(Syubetu & ItakuCode) Then
                            Dim value() As String = DirectCast(ItakuCodeList.Item(Syubetu & ItakuCode), String())
                            '13バイト巻き戻して読替情報に書き換え
                            fs.Seek(-13, SeekOrigin.Current)
                            fs.Write(Enc.GetBytes(value(0) & CodeKbn & value(1)), 0, 13)

                            Log.Write("委託者コード読替", "成功", "委託者コード：" & Syubetu & "-" & ItakuCode _
                                & " -> " & value(0) & "-" & value(1) & " ファイル名：" & filename)
                        End If

                        '次のレコードへシーク
                        fs.Seek(106 + Len, SeekOrigin.Current)

                    Case Else
                        '次のレコードへシーク
                        fs.Seek(119 + Len, SeekOrigin.Current)
                End Select
            End While

        Catch ex As Exception
            Log.Write("委託者コード読替", "失敗", "ファイル名：" & filename & " " & ex.Message)
            Message = "委託者コード読替失敗 ファイル名：" & filename
            Return False

        Finally
            If Not fs Is Nothing Then
                fs.Close()
            End If
        End Try

        Return True
    End Function

End Class
