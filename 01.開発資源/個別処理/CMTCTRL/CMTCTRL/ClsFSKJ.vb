Imports System
Imports System.Windows.Forms

''' <summary>
''' ＣＭＴ操作クラス（旧FSKJDLL）
''' </summary>
''' <remarks>
''' 2018/02/16 saitou 広島信金(RSV2標準) added for サーバー処理対応(64ビット対応)
''' ・VB6時代のソースを解析し、VB.NETにマイグレーションする。</remarks>
Public Class ClsFSKJ

    Public Function cmt_UNLOAD() As Integer
        cmt_UNLOAD = 0
        Dim In_mt_mtid As Integer
        '----------------------------------------
        '初期化ファイル読み込み
        '----------------------------------------
        Dim TEMP As String
        'Dim TEMP As String * 50
        TEMP = GetIni("DEV_INFO.INI", "CMT", "UNIT_NO")
        If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
            MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:CMTユニット番号" & vbCrLf & "分類:CMT" & vbCrLf & "項目:UNIT_NO",
                            "ＣＭＴアンロード", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            In_mt_mtid = TEMP
        End If

        'ﾘﾜｲﾝﾄﾞ
        cmt_UNLOAD = mtunload(mt_mtid)

    End Function

    Public Function cmtCPYtoCMT_CHK(ByRef Blksize As Integer,
                                    ByRef Lrecl As Integer,
                                    ByRef Label As Integer,
                                    ByRef OutVOL As String,
                                    ByRef OutFileSeq As Integer,
                                    ByRef CodeNo As Integer,
                                    ByRef InFileName As String,
                                    ByRef OutFileName As String,
                                    ByRef InCH13 As Integer,
                                    ByRef ErrStatus As Long) As String

        'Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        Dim HDR1_REC_SAVE As String
        Dim HDR2_REC_SAVE As String
        'Dim HDR1_REC_SAVE  As String * MT_LABEL_SIZE
        'Dim HDR2_REC_SAVE  As String * MT_LABEL_SIZE
        'Dim ErrStatus As Long             'MTDLL-52 関数戻り値
        Dim MTRET As Long             'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード      As String * 2
        Dim In_mt_mtid As Integer
        Dim VOL通番 As String
        'Dim VOL通番        As String * 6
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 OrElse Label = 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(OutFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(OutFileSeq)" & OutFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If InFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "CMT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:CMTユニット番号" & vbCrLf & "分類:CMT" & vbCrLf & "項目:UNIT_NO", _
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            mt_mtid = In_mt_mtid

            '----------------------------------------
            'ＣＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 1       'read-write
            MTRET = cmtONL_CHK(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    '　ブロック長チェック(80バイト)
                    '　ラベル識別(VOL1)チェック
                    '　ボリューム通番チェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If OutVOL = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = OutVOL Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    End If

                    'VOLUME通番退避
                    VOL通番 = Mid(mt_buffer, 5, 6)

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)／ファイル名書込み
                    '　HDR1はリライトする
                    Dim HDR1_buff As String
                    Dim H1_ファイル識別名 As String
                    Dim H1_ファイルセット識別名 As String
                    Dim H1_ボリューム順序番号 As String
                    Dim H1_ファイル順序番号 As String
                    Dim H1_世代番号 As String
                    Dim H1_世代更新番号 As String
                    Dim H1_引継情報 As String
                    'Dim HDR1_buff                As String * MT_LABEL_SIZE
                    'Dim H1_ファイル識別名         As String * 17
                    'Dim H1_ファイルセット識別名   As String * 6
                    'Dim H1_ボリューム順序番号     As String * 4
                    'Dim H1_ファイル順序番号       As String * 4
                    'Dim H1_世代番号               As String * 4
                    'Dim H1_世代更新番号           As String * 2
                    'Dim H1_引継情報              As String * 39

                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = 80 Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If OutFileName = " " Then
                                    OutFileName = Mid(mt_buffer, 5, 17)
                                End If
                                H1_ファイル識別名 = Mid(OutFileName, 1, 17)
                                '        H1_ファイルセット識別名 = Format(OutFileSeq, "000000")
                                H1_ファイルセット識別名 = VOL通番
                                H1_ボリューム順序番号 = "0001"
                                H1_ファイル順序番号 = "0001"
                                H1_世代番号 = "0001"
                                H1_世代更新番号 = "00"
                                H1_引継情報 = Mid(mt_buffer, 42, 39)
                                HDR1_buff = "HDR1" & H1_ファイル識別名 & H1_ファイルセット識別名 & H1_ボリューム順序番号 _
                                           & H1_ファイル順序番号 & H1_世代番号 & H1_世代更新番号 & H1_引継情報
                                HDR1_REC_SAVE = HDR1_buff
                                ErrStatus = mtbblock(mt_mtid)
                                ErrStatus = mtwblock(mt_mtid, HDR1_buff, mt_bufflen)
                                If ErrStatus = 0 Then
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "WRITE_ERR(HDR1)" & ErrStatus
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)" & ErrStatus
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)" & ErrStatus
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベル書込み(HDR2)
                    Dim HDR2_BUFF As String
                    Dim H2_レコード形式 As String
                    Dim H2_ブロック長 As String
                    Dim H2_レコード長 As String
                    Dim H2_記録密度 As String
                    Dim H2_制御文字 As String
                    Dim H2_予備1 As String
                    Dim H2_ブロック属性 As String
                    Dim H2_予備2 As String
                    'Dim H2_レコード形式  As String * 1
                    'Dim H2_ブロック長    As String * 5
                    'Dim H2_レコード長    As String * 5
                    'Dim H2_記録密度      As String * 1
                    'Dim H2_制御文字      As String * 1
                    'Dim H2_予備1        As String * 21
                    'Dim H2_ブロック属性  As String * 1
                    'Dim H2_予備2        As String * 41

                    mt_bufflen = MT_LABEL_SIZE
                    H2_レコード形式 = "F"
                    H2_ブロック長 = Format(Blksize, "00000")
                    H2_レコード長 = Format(Lrecl, "00000")
                    H2_記録密度 = " "
                    H2_制御文字 = "0"
                    H2_予備1 = Space(21)
                    H2_ブロック属性 = "B"
                    H2_予備2 = Space(41)
                    HDR2_BUFF = "HDR2" & H2_レコード形式 _
                              & H2_ブロック長 & H2_レコード長 & H2_記録密度 & H2_制御文字 & H2_予備1 _
                              & H2_ブロック属性 & H2_予備2
                    HDR2_REC_SAVE = HDR2_BUFF
                    ErrStatus = mtwblock(mt_mtid, HDR2_BUFF, mt_bufflen)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(HDR2)"
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)" & ErrStatus
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ファイルラベル(EOF1)
                    'ファイルラベル(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    Dim EOF1_BUFF As String
                    Dim EOF2_BUFF As String
                    'Dim EOF1_BUFF As String * MT_LABEL_SIZE
                    'Dim EOF2_BUFF As String * MT_LABEL_SIZE
                    ' EOF1
                    EOF1_BUFF = "EOF1" & Mid(HDR1_REC_SAVE, 5, 50) & Format(ブロック数, "000000") _
                        & Mid(HDR1_REC_SAVE, 61, 20)
                    ' EOF2
                    EOF2_BUFF = "EOF2" & Mid(HDR2_REC_SAVE, 5, 76)
                    ErrStatus = mtwblock(mt_mtid, EOF1_BUFF, mt_bufflen)
                    ErrStatus = mtwblock(mt_mtid, EOF2_BUFF, mt_bufflen)
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(EOF)"
                    End If

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        OutFileSeq = OutFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)"
                    End If

                    'テープマーク（２個）書込み
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try
    End Function

    Public Function cmtCPYtoDISK(ByRef Blksize As Integer,
                                 ByRef Lrecl As Integer,
                                 ByRef Label As Integer,
                                 ByRef InVol As String,
                                 ByRef InFileSeq As Integer,
                                 ByRef CodeNo As Integer,
                                 ByRef InFileName As String,
                                 ByRef OutFileName As String,
                                 ByRef OutCH13 As Integer,
                                 ByRef ErrStatus As Long) As String

        Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        Dim MTRET As Long         'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード As String * 2
        Dim In_mt_mtid As Integer
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 OrElse Label = 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(InFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileSeq)" & InFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If OutFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(OutFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "CMT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:CMTユニット番号" & vbCrLf & "分類:CMT" & vbCrLf & "項目:UNIT_NO",
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            '----------------------------------------
            'ＣＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 0       'read-only
            MTRET = cmtONL(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If InVol = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = InVol Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(VOL1)" & ErrStatus
                    End If

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If InFileName = " " Then
                                Else
                                    If InVol = "SLMT" Then
                                    Else
                                        If Mid(mt_buffer, 5, 17) = InFileName Then
                                        Else
                                            ErrStatus = mtunload(mt_mtid)
                                            Return "FILE NOT FOUND : " & Mid(mt_buffer, 5, 17)
                                        End If
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベルチェック(HDR2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                    If mtBitRtn.BIT_TMK = 1 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:FILE NOT FOUND(TMK))"
                    End If
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2:BLKSIZE) HDR:" & Mid(mt_buffer, 6, 5) & "PARA:" & Blksize
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR2:LENGTH=" & mt_count & ")"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:LENGTH=" & ErrStatus & ")"
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'cmtCPYtoDISK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    If mtBitRtn.BIT_TMK = 1 Then
                        'ラベルチェック(EOF1)
                        mt_bufflen = MT_LABEL_SIZE
                        ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                        If ErrStatus = 0 Then
                            If mt_count = MT_LABEL_SIZE Then
                                If Mid(mt_buffer, 1, 4) = "EOF1" Then
                                    If Mid(mt_buffer, 55, 6) = ブロック数 Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "READ-ERR(BLKCNT)"
                                    End If
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "LABEL-ERR(EOF1)"
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF1)"
                            End If
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ-ERR(DATA)"
                    End If

                    'ラベルチェック(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "EOF2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2:BLKSIZE)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(EOF2)"
                        End If
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'cmtCPYtoDISK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    'アンロード
                    ErrStatus = mtunload(mt_mtid)

                    Return ""

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        InFileSeq = InFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    'アンロード
                    ErrStatus = mtunload(mt_mtid)

                    Return ""

                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try
    End Function

    Public Function cmtCPYtoDISK_CHK(ByRef Blksize As Integer, _
                                     ByRef Lrecl As Integer, _
                                     ByRef Label As Integer, _
                                     ByRef InVol As String, _
                                     ByRef InFileSeq As Integer, _
                                     ByRef CodeNo As Integer, _
                                     ByRef InFileName As String, _
                                     ByRef OutFileName As String, _
                                     ByRef OutCH13 As Integer, _
                                     ByRef ErrStatus As Long) As String

        Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        Dim MTRET As Long         'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード As String * 2
        Dim In_mt_mtid As Integer
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 OrElse Label = 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(InFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileSeq)" & InFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If OutFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(OutFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "CMT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:CMTユニット番号" & vbCrLf & "分類:CMT" & vbCrLf & "項目:UNIT_NO", _
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            '----------------------------------------
            'ＣＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 0       'read-only
            MTRET = cmtONL(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If InVol = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = InVol Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(VOL1)" & ErrStatus
                    End If

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If InFileName = " " Then
                                Else
                                    If InVol = "SLMT" Then
                                    Else
                                        If Mid(mt_buffer, 5, 17) = InFileName Then
                                        Else
                                            ErrStatus = mtunload(mt_mtid)
                                            Return "FILE NOT FOUND : " & Mid(mt_buffer, 5, 17)
                                        End If
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベルチェック(HDR2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                    If mtBitRtn.BIT_TMK = 1 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:FILE NOT FOUND(TMK))"
                    End If
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2:BLKSIZE) HDR:" & Mid(mt_buffer, 6, 5) & "PARA:" & Blksize
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR2:LENGTH=" & mt_count & ")"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:LENGTH=" & ErrStatus & ")"
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'cmtCPYtoDISK_CHK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    If mtBitRtn.BIT_TMK = 1 Then
                        'ラベルチェック(EOF1)
                        mt_bufflen = MT_LABEL_SIZE
                        ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                        If ErrStatus = 0 Then
                            If mt_count = MT_LABEL_SIZE Then
                                If Mid(mt_buffer, 1, 4) = "EOF1" Then
                                    If Mid(mt_buffer, 55, 6) = ブロック数 Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "READ-ERR(BLKCNT)"
                                    End If
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "LABEL-ERR(EOF1)"
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF1)"
                            End If
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ-ERR(DATA)"
                    End If

                    'ラベルチェック(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "EOF2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2:BLKSIZE)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(EOF2)"
                        End If
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'cmtCPYtoDISK_CHK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    Return ""

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        InFileSeq = InFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    Return ""

                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try

    End Function

    Public Function cmtCPYtoMT(ByRef Blksize As Integer, _
                               ByRef Lrecl As Integer, _
                               ByRef Label As Integer, _
                               ByRef OutVOL As String, _
                               ByRef OutFileSeq As Integer, _
                               ByRef CodeNo As Integer, _
                               ByRef InFileName As String, _
                               ByRef OutFileName As String, _
                               ByRef InCH13 As Integer, _
                               ByRef ErrStatus As Long) As String

        'Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        Dim HDR1_REC_SAVE As String
        Dim HDR2_REC_SAVE  As String 
        'Dim HDR1_REC_SAVE  As String * MT_LABEL_SIZE
        'Dim HDR2_REC_SAVE  As String * MT_LABEL_SIZE
        Dim MTRET As Long             'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード      As String * 2
        Dim In_mt_mtid As Integer
        Dim VOL通番 As String
        'Dim VOL通番        As String * 6
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 Or 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(OutFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(0utFileSeq)" & OutFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If InFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "CMT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:CMTユニット番号" & vbCrLf & "分類:CMT" & vbCrLf & "項目:UNIT_NO", _
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            '----------------------------------------
            'ＣＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 1       'read-write
            MTRET = cmtONL(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    '　ブロック長チェック(80バイト)
                    '　ラベル識別(VOL1)チェック
                    '　ボリューム通番チェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If OutVOL = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = OutVOL Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    End If
                    'VOLUME通番退避
                    VOL通番 = Mid(mt_buffer, 5, 6)

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)／ファイル名書込み
                    '　HDR1はリライトする
                    Dim HDR1_buff As String
                    Dim H1_ファイル識別名 As String
                    Dim H1_ファイルセット識別名 As String
                    Dim H1_ボリューム順序番号 As String
                    Dim H1_ファイル順序番号 As String
                    Dim H1_世代番号 As String
                    Dim H1_世代更新番号 As String
                    Dim H1_引継情報 As String
                    'Dim HDR1_buff                As String * MT_LABEL_SIZE
                    'Dim H1_ファイル識別名         As String * 17
                    'Dim H1_ファイルセット識別名   As String * 6
                    'Dim H1_ボリューム順序番号     As String * 4
                    'Dim H1_ファイル順序番号       As String * 4
                    'Dim H1_世代番号               As String * 4
                    'Dim H1_世代更新番号           As String * 2
                    'Dim H1_引継情報              As String * 39

                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = 80 Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If OutFileName = " " Then
                                    OutFileName = Mid(mt_buffer, 5, 17)
                                End If
                                H1_ファイル識別名 = Mid(OutFileName, 1, 17)
                                '        H1_ファイルセット識別名 = Format(OutFileSeq, "000000")
                                H1_ファイルセット識別名 = VOL通番
                                H1_ボリューム順序番号 = "0001"
                                H1_ファイル順序番号 = "0001"
                                H1_世代番号 = "0001"
                                H1_世代更新番号 = "00"
                                H1_引継情報 = Mid(mt_buffer, 42, 39)
                                HDR1_buff = "HDR1" & H1_ファイル識別名 & H1_ファイルセット識別名 & H1_ボリューム順序番号 _
                                           & H1_ファイル順序番号 & H1_世代番号 & H1_世代更新番号 & H1_引継情報
                                HDR1_REC_SAVE = HDR1_buff
                                ErrStatus = mtbblock(mt_mtid)
                                ErrStatus = mtwblock(mt_mtid, HDR1_buff, mt_bufflen)
                                If ErrStatus = 0 Then
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "WRITE_ERR(HDR1)" & ErrStatus
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)" & ErrStatus
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)" & ErrStatus
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベル書込み(HDR2)
                    Dim HDR2_BUFF As String
                    Dim H2_レコード形式 As String
                    Dim H2_ブロック長 As String
                    Dim H2_レコード長 As String
                    Dim H2_記録密度 As String
                    Dim H2_制御文字 As String
                    Dim H2_予備1 As String
                    Dim H2_ブロック属性 As String
                    Dim H2_予備2 As String
                    'Dim H2_レコード形式  As String * 1
                    'Dim H2_ブロック長    As String * 5
                    'Dim H2_レコード長    As String * 5
                    'Dim H2_記録密度      As String * 1
                    'Dim H2_制御文字      As String * 1
                    'Dim H2_予備1        As String * 21
                    'Dim H2_ブロック属性  As String * 1
                    'Dim H2_予備2        As String * 41

                    mt_bufflen = MT_LABEL_SIZE
                    H2_レコード形式 = "F"
                    H2_ブロック長 = Format(Blksize, "00000")
                    H2_レコード長 = Format(Lrecl, "00000")
                    H2_記録密度 = " "
                    H2_制御文字 = "0"
                    H2_予備1 = Space(21)
                    H2_ブロック属性 = "B"
                    H2_予備2 = Space(41)
                    HDR2_BUFF = "HDR2" & H2_レコード形式 _
                              & H2_ブロック長 & H2_レコード長 & H2_記録密度 & H2_制御文字 & H2_予備1 _
                              & H2_ブロック属性 & H2_予備2
                    HDR2_REC_SAVE = HDR2_BUFF
                    ErrStatus = mtwblock(mt_mtid, HDR2_BUFF, mt_bufflen)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(HDR2)"
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)" & ErrStatus
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ファイルラベル(EOF1)
                    'ファイルラベル(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    Dim EOF1_BUFF As String
                    Dim EOF2_BUFF As String
                    'Dim EOF1_BUFF As String * MT_LABEL_SIZE
                    'Dim EOF2_BUFF As String * MT_LABEL_SIZE
                    ' EOF1
                    EOF1_BUFF = "EOF1" & Mid(HDR1_REC_SAVE, 5, 50) & Format(ブロック数, "000000") _
                        & Mid(HDR1_REC_SAVE, 61, 20)
                    ' EOF2
                    EOF2_BUFF = "EOF2" & Mid(HDR2_REC_SAVE, 5, 76)
                    ErrStatus = mtwblock(mt_mtid, EOF1_BUFF, mt_bufflen)
                    ErrStatus = mtwblock(mt_mtid, EOF2_BUFF, mt_bufflen)
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(EOF)"
                    End If

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        OutFileSeq = OutFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)"
                    End If

                    'テープマーク（２個）書込み
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try
    End Function

    Public Function cmtONL(ByRef In_mt_mtid As Integer, _
                           ByRef In_Mode As Integer) As Long

        Dim mtBitRtn As DEVICE_status
        Dim MTRET As Long
        'Dim 改行 As String
        ''Dim 改行            As String * 2
        '改行 = Chr(13) & Chr(10)
        Dim 記録密度 As String
        'Dim 記録密度        As String * 10
        Dim 設定記録密度 As Integer
        Dim プロテクト As String
        'Dim プロテクト      As String * 10
        Dim Msg As String
        Dim Msg_Buttons As Integer
        Dim Msg_Title As String
        'Dim Reply As Integer
        Dim 接続装置 As Byte
        Dim 装置情報 As String
        Dim MT装置認識 As Boolean

        Msg_Title = "ＣＭＴオンライン処理"
        Msg_Buttons = vbOKOnly
        MT装置認識 = False

        Try
            '----------------------------------------
            'ＣＭＴオンライン処理
            '----------------------------------------
            Do
                '----------------------------------------
                '装置ステータス判定
                '----------------------------------------
                '一回目はダミー実行する（MTDLL-52の仕様）
                mt_rtn = mtstat(In_mt_mtid)
                mt_rtn = 0

                '----------------------------------------
                'ＣＭＴ装置の初期化
                '(戻り値には接続されている装置台数がセットされる)
                '----------------------------------------
                mt_rtn = mtinit(T_Mtinfo(1))
                If mt_rtn = 0 OrElse mt_rtn = 255 Then
                    Msg = "ＣＭＴ装置が接続されていません。" & vbCrLf & _
                        "エラーコード：" & mt_rtn
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                Else
                    接続装置 = 1
                End If

                Do Until MT装置認識 = True Or 接続装置 > mt_rtn

                    If T_Mtinfo(接続装置).UnitNo = In_mt_mtid Then
                        MT装置認識 = True
                    End If

                    装置情報 = Chr(T_Mtinfo(接続装置).Vender(0)) & Chr(T_Mtinfo(接続装置).Vender(1)) & Chr(T_Mtinfo(接続装置).Vender(2)) _
                             & Chr(T_Mtinfo(接続装置).Vender(3)) & Chr(T_Mtinfo(接続装置).Vender(4)) & Chr(T_Mtinfo(接続装置).Vender(5)) _
                             & Chr(T_Mtinfo(接続装置).Vender(6)) & Chr(T_Mtinfo(接続装置).Vender(7))

                    装置情報 = 装置情報 & Chr(T_Mtinfo(接続装置).Product(0)) & Chr(T_Mtinfo(接続装置).Product(1)) & Chr(T_Mtinfo(接続装置).Product(2)) _
                             & Chr(T_Mtinfo(接続装置).Product(3)) & Chr(T_Mtinfo(接続装置).Product(4)) & Chr(T_Mtinfo(接続装置).Product(5)) _
                             & Chr(T_Mtinfo(接続装置).Product(6)) & Chr(T_Mtinfo(接続装置).Product(7))

                    接続装置 = 接続装置 + 1

                Loop

                If MT装置認識 <> True Then
                    Msg = "ＣＭＴのアクセスに失敗しました。（装置初期化）" & vbCrLf & _
                        "ユニットＩＤ：" & In_mt_mtid
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                End If

                'start 2008/07/29追加 by mori
                mt_rtn = mtinit(T_Mtinfo(1))
                mt_rtn = mtstat(In_mt_mtid)
                mt_rtn = 0
                'end by mori

                '----------------------------------------
                'ＣＭＴのマウント要求メッセージ出力
                '----------------------------------------
                Msg = "ＣＭＴをセットしてください。"
                If MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                    mt_mtid = In_mt_mtid
                Else
                    Return -1
                End If

                '----------------------------------------
                '装置のオンライン
                '----------------------------------------

                'start 2008/07/29追加 by mori
                mt_rtn = mtinit(T_Mtinfo(1))
                mt_rtn = mtstat(In_mt_mtid)
                mt_rtn = 0
                'end by mori

                mt_rtn = mtonline(mt_mtid)
                If mt_rtn = 0 Then
                    Exit Do
                Else
                    Msg = "ＣＭＴのオンラインに失敗しました。" & vbCrLf & _
                        "エラーコード：" & mt_rtn & vbCrLf & _
                        "処理を継続しますか？"
                    If MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then

                    Else
                        Return -1
                    End If
                End If
            Loop

            '----------------------------------------
            '装置ステータス判定
            '----------------------------------------
            '一回目はダミー実行する（MTDLL-52の仕様）
            mt_rtn = mtstat(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                    Else
                        記録密度 = "1600BPI"
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                    Else
                        記録密度 = "800BPI"
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select

            'プロテクト判定
            If In_Mode = 0 Then         'read-only
            Else
                Select Case mtBitRtn.BIT_PRO
                    Case 0
                        プロテクト = "書込可能"
                    Case 1
                        プロテクト = "書込禁止"
                        Msg = "書込禁止です。プロテクトを解除してください。"
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return -1
                    Case Else
                        Msg = "ステータス変換エラーが発生しました。（プロテクト）" & vbCrLf & _
                            "ステータス：" & mtBitRtn.BIT_PRO
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return -1
                End Select
            End If

            '記録密度設定
            mt_rtn = mtfblock(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                        設定記録密度 = 3
                    Else
                        記録密度 = "1600BPI"
                        '設定記録密度 = 1
                        設定記録密度 = 7
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                        設定記録密度 = 2
                    Else
                        記録密度 = "800BPI"
                        '設定記録密度 = 0
                        設定記録密度 = 6
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select
            mt_rtn = mtonline(mt_mtid)
            mt_rtn = mtdensity(mt_mtid, 設定記録密度)

            Return 0

        Catch ex As Exception
            Msg = "例外が発生しました。" & vbCrLf & ex.ToString
            MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1
        End Try
    End Function

    Public Function cmtONL_CHK(ByRef In_mt_mtid As Integer, _
                               ByRef In_Mode As Integer) As Long

        Dim mtBitRtn As DEVICE_status
        Dim MTRET As Long
        'Dim 改行 As String
        ''Dim 改行            As String * 2
        '改行 = Chr(13) & Chr(10)
        Dim 記録密度 As String
        'Dim 記録密度        As String * 10
        Dim 設定記録密度 As Integer
        Dim プロテクト As String
        'Dim プロテクト      As String * 10
        Dim Msg As String
        Dim Msg_Buttons As Integer
        Dim Msg_Title As String
        'Dim Reply As Integer
        Dim 接続装置 As Byte
        Dim 装置情報 As String
        Dim MT装置認識 As Boolean

        Msg_Title = "ＣＭＴオンライン処理"
        Msg_Buttons = vbOKOnly
        MT装置認識 = False

        Try
            '----------------------------------------
            'ＣＭＴオンライン処理
            '----------------------------------------
            Do
                '----------------------------------------
                '装置ステータス判定
                '----------------------------------------
                '一回目はダミー実行する（MTDLL-52の仕様）
                mt_rtn = mtstat(In_mt_mtid)
                mt_rtn = 0

                '----------------------------------------
                'ＣＭＴ装置の初期化
                '(戻り値には接続されている装置台数がセットされる)
                '----------------------------------------
                mt_rtn = mtinit(T_Mtinfo(1))
                If mt_rtn = 0 OrElse mt_rtn = 255 Then
                    Msg = "ＣＭＴ装置が接続されていません。" & vbCrLf & _
                        "エラーコード：" & mt_rtn
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                Else
                    接続装置 = 1
                End If

                Do Until MT装置認識 = True Or 接続装置 > mt_rtn

                    If T_Mtinfo(接続装置).UnitNo = In_mt_mtid Then
                        MT装置認識 = True
                    End If

                    装置情報 = Chr(T_Mtinfo(接続装置).Vender(0)) & Chr(T_Mtinfo(接続装置).Vender(1)) & Chr(T_Mtinfo(接続装置).Vender(2)) _
                             & Chr(T_Mtinfo(接続装置).Vender(3)) & Chr(T_Mtinfo(接続装置).Vender(4)) & Chr(T_Mtinfo(接続装置).Vender(5)) _
                             & Chr(T_Mtinfo(接続装置).Vender(6)) & Chr(T_Mtinfo(接続装置).Vender(7))

                    装置情報 = 装置情報 & Chr(T_Mtinfo(接続装置).Product(0)) & Chr(T_Mtinfo(接続装置).Product(1)) & Chr(T_Mtinfo(接続装置).Product(2)) _
                             & Chr(T_Mtinfo(接続装置).Product(3)) & Chr(T_Mtinfo(接続装置).Product(4)) & Chr(T_Mtinfo(接続装置).Product(5)) _
                             & Chr(T_Mtinfo(接続装置).Product(6)) & Chr(T_Mtinfo(接続装置).Product(7))

                    接続装置 = 接続装置 + 1

                Loop

                If MT装置認識 <> True Then
                    Msg = "ＣＭＴのアクセスに失敗しました。（装置初期化）" & vbCrLf & _
                        "ユニットＩＤ：" & In_mt_mtid
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                End If

                '----------------------------------------
                '装置のオンライン
                '----------------------------------------
                mt_rtn = 0
                If mt_rtn = 0 Then
                    Exit Do
                Else
                    Msg = "ＣＭＴのオンラインに失敗しました。" & vbCrLf & _
                        "エラーコード：" & mt_rtn & vbCrLf & _
                        "処理を継続しますか？"
                    If MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then

                    Else
                        Return -1
                    End If
                End If
            Loop

            '----------------------------------------
            '装置ステータス判定
            '----------------------------------------
            '一回目はダミー実行する（MTDLL-52の仕様）
            mt_rtn = mtstat(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                    Else
                        記録密度 = "1600BPI"
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                    Else
                        記録密度 = "800BPI"
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select

            'プロテクト判定
            If In_Mode = 0 Then         'read-only
            Else
                Select Case mtBitRtn.BIT_PRO
                    Case 0
                        プロテクト = "書込可能"
                    Case 1
                        プロテクト = "書込禁止"
                        Msg = "書込禁止です。プロテクトを解除してください。"
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return -1
                    Case Else
                        Msg = "ステータス変換エラーが発生しました。（プロテクト）" & vbCrLf & _
                            "ステータス：" & mtBitRtn.BIT_PRO
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return -1
                End Select
            End If

            '記録密度設定
            mt_rtn = mtfblock(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                        設定記録密度 = 3
                    Else
                        記録密度 = "1600BPI"
                        '設定記録密度 = 1
                        設定記録密度 = 7
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                        設定記録密度 = 2
                    Else
                        記録密度 = "800BPI"
                        '設定記録密度 = 0
                        設定記録密度 = 6
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select
            mt_rtn = mtonline(mt_mtid)
            mt_rtn = mtdensity(mt_mtid, 設定記録密度)

            Return 0

        Catch ex As Exception
            Msg = "例外が発生しました。" & vbCrLf & ex.ToString
            MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1
        End Try

    End Function

    Public Function mt_UNLOAD() As Integer
        mt_UNLOAD = 0
        Dim In_mt_mtid As Integer
        '----------------------------------------
        '初期化ファイル読み込み
        '----------------------------------------
        Dim TEMP As String
        'Dim TEMP As String * 50
        TEMP = GetIni("DEV_INFO.INI", "MT", "UNIT_NO")
        If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
            MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:MTユニット番号" & vbCrLf & "分類:MT" & vbCrLf & "項目:UNIT_NO",
                            "ＭＴアンロード", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            In_mt_mtid = TEMP
        End If

        'ﾘﾜｲﾝﾄﾞ
        mt_UNLOAD = mtunload(mt_mtid)

    End Function

    Private Function mtCHGStatus(ByRef Fskj_mt_rtn As Long, _
                                 ByRef Fskj_mt_BITrtn As DEVICE_status) As Integer

        Dim STATUS_BIT(16) As Integer
        Dim count As Integer

        Try
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
                Return 0
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
                Return 0
            End If
            'ビット --> バイト変換
            For count = 0 To 15
                STATUS_BIT(count) = Fskj_mt_rtn Mod 2
                Fskj_mt_rtn = Fskj_mt_rtn \ 2
            Next

            Fskj_mt_BITrtn.BIT_TMK = STATUS_BIT(0)
            Fskj_mt_BITrtn.BIT_EOT = STATUS_BIT(1)
            Fskj_mt_BITrtn.BIT_BOT = STATUS_BIT(2)
            Fskj_mt_BITrtn.BIT_DEN0 = STATUS_BIT(3)
            Fskj_mt_BITrtn.BIT_DEN1 = STATUS_BIT(4)
            Fskj_mt_BITrtn.BIT_FIL1 = STATUS_BIT(5)
            Fskj_mt_BITrtn.BIT_PRO = STATUS_BIT(6)
            Fskj_mt_BITrtn.BIT_FIL2 = STATUS_BIT(7)
            Fskj_mt_BITrtn.BIT_DTE = STATUS_BIT(8)
            Fskj_mt_BITrtn.BIT_HDE = STATUS_BIT(9)
            Fskj_mt_BITrtn.BIT_NRDY = STATUS_BIT(10)
            Fskj_mt_BITrtn.BIT_ILC = STATUS_BIT(11)
            Fskj_mt_BITrtn.BIT_SCE = STATUS_BIT(12)
            Fskj_mt_BITrtn.BIT_UDC = STATUS_BIT(13)
            Fskj_mt_BITrtn.BIT_FIL3 = STATUS_BIT(14)
            Fskj_mt_BITrtn.BIT_CHG = STATUS_BIT(15)

            Return 0

        Catch ex As Exception
            Return -1
        End Try
    End Function

    Public Function mtCPYtoDISK(ByRef Blksize As Integer, _
                                ByRef Lrecl As Integer, _
                                ByRef Label As Integer, _
                                ByRef InVol As String, _
                                ByRef InFileSeq As Integer, _
                                ByRef CodeNo As Integer, _
                                ByRef InFileName As String, _
                                ByRef OutFileName As String, _
                                ByRef OutCH13 As Integer, _
                                ByRef ErrStatus As Long) As String

        Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        'Dim ErrStatus As Long         'MTDLL-52 関数戻り値
        Dim MTRET As Long         'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード As String * 2
        Dim In_mt_mtid As Integer
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 OrElse Label = 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(InFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileSeq)" & InFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If OutFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(OutFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "MT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:MTユニット番号" & vbCrLf & "分類:MT" & vbCrLf & "項目:UNIT_NO", _
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            '----------------------------------------
            'ＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 0       'read-only
            MTRET = mtONL(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If InVol = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = InVol Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(VOL1)" & ErrStatus
                    End If

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If InFileName = " " Then
                                Else
                                    If InVol = "SLMT" Then
                                    Else
                                        If Mid(mt_buffer, 5, 17) = InFileName Then
                                        Else
                                            ErrStatus = mtunload(mt_mtid)
                                            Return "FILE NOT FOUND : " & Mid(mt_buffer, 5, 17)
                                        End If
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベルチェック(HDR2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                    If mtBitRtn.BIT_TMK = 1 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:FILE NOT FOUND(TMK))"
                    End If
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2:BLKSIZE) HDR:" & Mid(mt_buffer, 6, 5) & "PARA:" & Blksize
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR2:LENGTH=" & mt_count & ")"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:LENGTH=" & ErrStatus & ")"
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'mtCPYtoDISK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    If mtBitRtn.BIT_TMK = 1 Then
                        'ラベルチェック(EOF1)
                        mt_bufflen = MT_LABEL_SIZE
                        ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                        If ErrStatus = 0 Then
                            If mt_count = MT_LABEL_SIZE Then
                                If Mid(mt_buffer, 1, 4) = "EOF1" Then
                                    If Mid(mt_buffer, 55, 6) = ブロック数 Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "READ-ERR(BLKCNT)"
                                    End If
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "LABEL-ERR(EOF1)"
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF1)"
                            End If
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ-ERR(DATA)"
                    End If

                    'ラベルチェック(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "EOF2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2:BLKSIZE)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(EOF2)"
                        End If
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'mtCPYtoDISK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    'アンロード
                    ErrStatus = mtunload(mt_mtid)

                    Return ""

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        InFileSeq = InFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ﾌｧｲﾙまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    'アンロード
                    ErrStatus = mtunload(mt_mtid)

                    Return ""

                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try
    End Function

    Public Function mtCPYtoDISK_CHK(ByRef Blksize As Integer, _
                                    ByRef Lrecl As Integer, _
                                    ByRef Label As Integer, _
                                    ByRef InVol As String, _
                                    ByRef InFileSeq As Integer, _
                                    ByRef CodeNo As Integer, _
                                    ByRef InFileName As String, _
                                    ByRef OutFileName As String, _
                                    ByRef OutCH13 As Integer, _
                                    ByRef ErrStatus As Long) As String

        Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        'Dim ErrStatus As Long         'MTDLL-52 関数戻り値
        Dim MTRET As Long         'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード As String * 2
        Dim In_mt_mtid As Integer
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 OrElse Label = 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(InFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileSeq)" & InFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If OutFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(OutFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "MT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:MTユニット番号" & vbCrLf & "分類:MT" & vbCrLf & "項目:UNIT_NO", _
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            '----------------------------------------
            'ＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 0       'read-only
            MTRET = mtONL(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If InVol = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = InVol Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(VOL1)" & ErrStatus
                    End If

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If InFileName = " " Then
                                Else
                                    If InVol = "SLMT" Then
                                    Else
                                        If Mid(mt_buffer, 5, 17) = InFileName Then
                                        Else
                                            ErrStatus = mtunload(mt_mtid)
                                            Return "FILE NOT FOUND : " & Mid(mt_buffer, 5, 17)
                                        End If
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベルチェック(HDR2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                    If mtBitRtn.BIT_TMK = 1 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:FILE NOT FOUND(TMK))"
                    End If
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "HDR2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR2:BLKSIZE) HDR:" & Mid(mt_buffer, 6, 5) & "PARA:" & Blksize
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR2:LENGTH=" & mt_count & ")"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR2:LENGTH=" & ErrStatus & ")"
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'mtCPYtoDISK_CHK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    If mtBitRtn.BIT_TMK = 1 Then
                        'ラベルチェック(EOF1)
                        mt_bufflen = MT_LABEL_SIZE
                        ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                        If ErrStatus = 0 Then
                            If mt_count = MT_LABEL_SIZE Then
                                If Mid(mt_buffer, 1, 4) = "EOF1" Then
                                    If Mid(mt_buffer, 55, 6) = ブロック数 Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "READ-ERR(BLKCNT)"
                                    End If
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "LABEL-ERR(EOF1)"
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF1)"
                            End If
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ-ERR(DATA)"
                    End If

                    'ラベルチェック(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "EOF2" Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2)"
                            End If
                            If Mid(mt_buffer, 6, 5) = Blksize Then
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(EOF2:BLKSIZE)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(EOF2)"
                        End If
                    End If

                    'テープマークチェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        'コメントアウト　これはエラーじゃないよね？
                        'mtCPYtoDISK_CHK = "READ_ERR(TMK)"
                    Else
                        MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                        If MTRET = 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                        If mtBitRtn.BIT_TMK = 1 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ-ERR(TMK)"
                        End If
                    End If

                    Return ""

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        InFileSeq = InFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ﾌｧｲﾙまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (InFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & InFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'データ読み込み、ディスク書き出し
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoDISK(Blksize, Lrecl, OutFileName, OutCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 > 0 Then
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "READ_ERR(NODATA)"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "READ_ERR(DATA)"
                    End If

                    Return ""

                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try

    End Function

    Public Function mtCPYtoMT(ByRef Blksize As Integer, _
                              ByRef Lrecl As Integer, _
                              ByRef Label As Integer, _
                              ByRef OutVOL As String, _
                              ByRef OutFileSeq As Integer, _
                              ByRef CodeNo As Integer, _
                              ByRef InFileName As String, _
                              ByRef OutFileName As String, _
                              ByRef InCH13 As Integer, _
                              ByRef ErrStatus As Long) As String

        'Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        Dim HDR1_REC_SAVE As String
        Dim HDR2_REC_SAVE As String
        'Dim HDR1_REC_SAVE  As String * MT_LABEL_SIZE
        'Dim HDR2_REC_SAVE  As String * MT_LABEL_SIZE
        Dim MTRET As Long             'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード      As String * 2
        Dim In_mt_mtid As Integer
        Dim VOL通番 As String
        'Dim VOL通番        As String * 6
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 Or 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(OutFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(0utFileSeq)" & OutFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If InFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "MT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:MTユニット番号" & vbCrLf & "分類:MT" & vbCrLf & "項目:UNIT_NO", _
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            '----------------------------------------
            'ＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 1       'read-write
            MTRET = mtONL(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    '　ブロック長チェック(80バイト)
                    '　ラベル識別(VOL1)チェック
                    '　ボリューム通番チェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If OutVOL = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = OutVOL Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    End If
                    'VOLUME通番退避
                    VOL通番 = Mid(mt_buffer, 5, 6)

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)／ファイル名書込み
                    '　HDR1はリライトする
                    Dim HDR1_buff As String
                    Dim H1_ファイル識別名 As String
                    Dim H1_ファイルセット識別名 As String
                    Dim H1_ボリューム順序番号 As String
                    Dim H1_ファイル順序番号 As String
                    Dim H1_世代番号 As String
                    Dim H1_世代更新番号 As String
                    Dim H1_引継情報 As String
                    'Dim HDR1_buff                As String * MT_LABEL_SIZE
                    'Dim H1_ファイル識別名         As String * 17
                    'Dim H1_ファイルセット識別名   As String * 6
                    'Dim H1_ボリューム順序番号     As String * 4
                    'Dim H1_ファイル順序番号       As String * 4
                    'Dim H1_世代番号               As String * 4
                    'Dim H1_世代更新番号           As String * 2
                    'Dim H1_引継情報              As String * 39

                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = 80 Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If OutFileName = " " Then
                                    OutFileName = Mid(mt_buffer, 5, 17)
                                End If
                                H1_ファイル識別名 = Mid(OutFileName, 1, 17)
                                '        H1_ファイルセット識別名 = Format(OutFileSeq, "000000")
                                H1_ファイルセット識別名 = VOL通番
                                H1_ボリューム順序番号 = "0001"
                                H1_ファイル順序番号 = "0001"
                                H1_世代番号 = "0001"
                                H1_世代更新番号 = "00"
                                H1_引継情報 = Mid(mt_buffer, 42, 39)
                                HDR1_buff = "HDR1" & H1_ファイル識別名 & H1_ファイルセット識別名 & H1_ボリューム順序番号 _
                                           & H1_ファイル順序番号 & H1_世代番号 & H1_世代更新番号 & H1_引継情報
                                HDR1_REC_SAVE = HDR1_buff
                                ErrStatus = mtbblock(mt_mtid)
                                ErrStatus = mtwblock(mt_mtid, HDR1_buff, mt_bufflen)
                                If ErrStatus = 0 Then
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "WRITE_ERR(HDR1)" & ErrStatus
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)" & ErrStatus
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)" & ErrStatus
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベル書込み(HDR2)
                    Dim HDR2_BUFF As String
                    Dim H2_レコード形式 As String
                    Dim H2_ブロック長 As String
                    Dim H2_レコード長 As String
                    Dim H2_記録密度 As String
                    Dim H2_制御文字 As String
                    Dim H2_予備1 As String
                    Dim H2_ブロック属性 As String
                    Dim H2_予備2 As String
                    'Dim H2_レコード形式  As String * 1
                    'Dim H2_ブロック長    As String * 5
                    'Dim H2_レコード長    As String * 5
                    'Dim H2_記録密度      As String * 1
                    'Dim H2_制御文字      As String * 1
                    'Dim H2_予備1        As String * 21
                    'Dim H2_ブロック属性  As String * 1
                    'Dim H2_予備2        As String * 41

                    mt_bufflen = MT_LABEL_SIZE
                    H2_レコード形式 = "F"
                    H2_ブロック長 = Format(Blksize, "00000")
                    H2_レコード長 = Format(Lrecl, "00000")
                    H2_記録密度 = " "
                    H2_制御文字 = "0"
                    H2_予備1 = Space(21)
                    H2_ブロック属性 = "B"
                    H2_予備2 = Space(41)
                    HDR2_BUFF = "HDR2" & H2_レコード形式 _
                              & H2_ブロック長 & H2_レコード長 & H2_記録密度 & H2_制御文字 & H2_予備1 _
                              & H2_ブロック属性 & H2_予備2
                    HDR2_REC_SAVE = HDR2_BUFF
                    ErrStatus = mtwblock(mt_mtid, HDR2_BUFF, mt_bufflen)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(HDR2)"
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)" & ErrStatus
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ファイルラベル(EOF1)
                    'ファイルラベル(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    Dim EOF1_BUFF As String
                    Dim EOF2_BUFF As String
                    'Dim EOF1_BUFF As String * MT_LABEL_SIZE
                    'Dim EOF2_BUFF As String * MT_LABEL_SIZE
                    ' EOF1
                    EOF1_BUFF = "EOF1" & Mid(HDR1_REC_SAVE, 5, 50) & Format(ブロック数, "000000") _
                        & Mid(HDR1_REC_SAVE, 61, 20)
                    ' EOF2
                    EOF2_BUFF = "EOF2" & Mid(HDR2_REC_SAVE, 5, 76)
                    ErrStatus = mtwblock(mt_mtid, EOF1_BUFF, mt_bufflen)
                    ErrStatus = mtwblock(mt_mtid, EOF2_BUFF, mt_bufflen)
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(EOF)"
                    End If

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        OutFileSeq = OutFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)"
                    End If

                    'テープマーク（２個）書込み
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try
    End Function

    Public Function mtCPYtoMT_CHK(ByRef Blksize As Integer, _
                                  ByRef Lrecl As Integer, _
                                  ByRef Label As Integer, _
                                  ByRef OutVOL As String, _
                                  ByRef OutFileSeq As Integer, _
                                  ByRef CodeNo As Integer, _
                                  ByRef InFileName As String, _
                                  ByRef OutFileName As String, _
                                  ByRef InCH13 As Integer, _
                                  ByRef ErrStatus As Long) As String

        'Dim mtBitRtn As DEVICE_status
        Dim mt_codeno As Integer
        Dim HDR1_REC_SAVE As String
        Dim HDR2_REC_SAVE As String
        'Dim HDR1_REC_SAVE  As String * MT_LABEL_SIZE
        'Dim HDR2_REC_SAVE  As String * MT_LABEL_SIZE
        Dim MTRET As Long             'FSKJDLL  関数戻り値
        Dim FILE_CNT As Integer
        'Dim TMK_CNT As Integer
        'Dim WRITE_REC As String
        Dim Mode As Integer
        Dim 改行コード As String
        'Dim 改行コード      As String * 2
        Dim In_mt_mtid As Integer
        Dim VOL通番 As String
        'Dim VOL通番        As String * 6
        改行コード = Chr(13) & Chr(10)

        Try
            '----------------------------------------
            'パラメータチェック
            '----------------------------------------
            If IsNumeric(Blksize) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Blksize)" & Blksize
            End If
            If IsNumeric(Lrecl) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Lrecl)" & Lrecl
            End If
            If Label = 0 Or 1 Then
            Else
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(Label)" & Label
            End If
            If IsNumeric(OutFileSeq) = False Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(0utFileSeq)" & OutFileSeq
            End If
            Select Case CodeNo
                Case 0 To 5
                    mt_codeno = CodeNo
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "PARAMETA_ERR"
            End Select
            If InFileName.Trim = String.Empty Then
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(InFileName)"
            End If

            '----------------------------------------
            '初期化ファイル読み込み
            '----------------------------------------
            Dim TEMP As String
            'Dim TEMP As String * 50
            TEMP = GetIni("DEV_INFO.INI", "MT", "UNIT_NO")
            If TEMP = "err" OrElse TEMP = Nothing OrElse IsNumeric(TEMP) = False Then
                MessageBox.Show("イニシャルファイルからのデータ取得に失敗しました。" & vbCrLf & "項目名:MTユニット番号" & vbCrLf & "分類:MT" & vbCrLf & "項目:UNIT_NO", _
                                "ファイル複写", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ErrStatus = mtunload(mt_mtid)
                Return "PARAMETA_ERR(DEV_INFO.ini)"
            Else
                In_mt_mtid = TEMP
            End If

            mt_mtid = In_mt_mtid
            '----------------------------------------
            'ＭＴ装置オンライン処理
            '----------------------------------------
            Mode = 1       'read-write
            MTRET = mtONL_CHK(In_mt_mtid, Mode)
            If MTRET <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "ONLINE-ERR" & MTRET
            End If

            '----------------------------------------
            'コード変換モード設定
            '----------------------------------------
            ErrStatus = mtebc(mt_codeno)
            If ErrStatus <> 0 Then
                ErrStatus = mtunload(mt_mtid)
                Return "MTCODE-ERR" & ErrStatus
            End If

            Select Case Label
                Case 0    'SL
                    '----------------------------------------
                    'スタンダードラベル処理
                    '----------------------------------------
                    'ディスクへの複写処理

                    'ラベルチェック(VOL1)
                    '　ブロック長チェック(80バイト)
                    '　ラベル識別(VOL1)チェック
                    '　ボリューム通番チェック
                    mt_bufflen = Blksize
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = MT_LABEL_SIZE Then
                            If Mid(mt_buffer, 1, 4) = "VOL1" Then
                                If OutVOL = "SLMT" Then
                                Else
                                    If Mid(mt_buffer, 5, 6) = OutVOL Then
                                    Else
                                        ErrStatus = mtunload(mt_mtid)
                                        Return "VOLUME IS INVALID :VOL=" & Mid(mt_buffer, 5, 6)
                                    End If
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(VOL1)"
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(VOL1)"
                        End If
                    End If
                    'VOLUME通番退避
                    VOL通番 = Mid(mt_buffer, 5, 6)

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= 3 * (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ラベルチェック(HDR1)／ファイル名書込み
                    '　HDR1はリライトする
                    Dim HDR1_buff As String
                    Dim H1_ファイル識別名 As String
                    Dim H1_ファイルセット識別名 As String
                    Dim H1_ボリューム順序番号 As String
                    Dim H1_ファイル順序番号 As String
                    Dim H1_世代番号 As String
                    Dim H1_世代更新番号 As String
                    Dim H1_引継情報 As String
                    'Dim HDR1_buff                As String * MT_LABEL_SIZE
                    'Dim H1_ファイル識別名         As String * 17
                    'Dim H1_ファイルセット識別名   As String * 6
                    'Dim H1_ボリューム順序番号     As String * 4
                    'Dim H1_ファイル順序番号       As String * 4
                    'Dim H1_世代番号               As String * 4
                    'Dim H1_世代更新番号           As String * 2
                    'Dim H1_引継情報              As String * 39

                    mt_bufflen = MT_LABEL_SIZE
                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    If ErrStatus = 0 Then
                        If mt_count = 80 Then
                            If Mid(mt_buffer, 1, 4) = "HDR1" Then
                                If OutFileName = " " Then
                                    OutFileName = Mid(mt_buffer, 5, 17)
                                End If
                                H1_ファイル識別名 = Mid(OutFileName, 1, 17)
                                '        H1_ファイルセット識別名 = Format(OutFileSeq, "000000")
                                H1_ファイルセット識別名 = VOL通番
                                H1_ボリューム順序番号 = "0001"
                                H1_ファイル順序番号 = "0001"
                                H1_世代番号 = "0001"
                                H1_世代更新番号 = "00"
                                H1_引継情報 = Mid(mt_buffer, 42, 39)
                                HDR1_buff = "HDR1" & H1_ファイル識別名 & H1_ファイルセット識別名 & H1_ボリューム順序番号 _
                                           & H1_ファイル順序番号 & H1_世代番号 & H1_世代更新番号 & H1_引継情報
                                HDR1_REC_SAVE = HDR1_buff
                                ErrStatus = mtbblock(mt_mtid)
                                ErrStatus = mtwblock(mt_mtid, HDR1_buff, mt_bufflen)
                                If ErrStatus = 0 Then
                                Else
                                    ErrStatus = mtunload(mt_mtid)
                                    Return "WRITE_ERR(HDR1)" & ErrStatus
                                End If
                            Else
                                ErrStatus = mtunload(mt_mtid)
                                Return "LABEL-ERR(HDR1)" & ErrStatus
                            End If
                        Else
                            ErrStatus = mtunload(mt_mtid)
                            Return "LABEL-ERR(HDR1)" & ErrStatus
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "LABEL-ERR(HDR1)" & ErrStatus
                    End If

                    'ラベル書込み(HDR2)
                    Dim HDR2_BUFF As String
                    Dim H2_レコード形式 As String
                    Dim H2_ブロック長 As String
                    Dim H2_レコード長 As String
                    Dim H2_記録密度 As String
                    Dim H2_制御文字 As String
                    Dim H2_予備1 As String
                    Dim H2_ブロック属性 As String
                    Dim H2_予備2 As String
                    'Dim H2_レコード形式  As String * 1
                    'Dim H2_ブロック長    As String * 5
                    'Dim H2_レコード長    As String * 5
                    'Dim H2_記録密度      As String * 1
                    'Dim H2_制御文字      As String * 1
                    'Dim H2_予備1        As String * 21
                    'Dim H2_ブロック属性  As String * 1
                    'Dim H2_予備2        As String * 41

                    mt_bufflen = MT_LABEL_SIZE
                    H2_レコード形式 = "F"
                    H2_ブロック長 = Format(Blksize, "00000")
                    H2_レコード長 = Format(Lrecl, "00000")
                    H2_記録密度 = " "
                    H2_制御文字 = "0"
                    H2_予備1 = Space(21)
                    H2_ブロック属性 = "B"
                    H2_予備2 = Space(41)
                    HDR2_BUFF = "HDR2" & H2_レコード形式 _
                              & H2_ブロック長 & H2_レコード長 & H2_記録密度 & H2_制御文字 & H2_予備1 _
                              & H2_ブロック属性 & H2_予備2
                    HDR2_REC_SAVE = HDR2_BUFF
                    ErrStatus = mtwblock(mt_mtid, HDR2_BUFF, mt_bufflen)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(HDR2)"
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)" & ErrStatus
                    End If

                    'テープマーク（１個）書込み
                    ErrStatus = mtwtmk(mt_mtid)
                    If ErrStatus = 0 Then
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If

                    'ファイルラベル(EOF1)
                    'ファイルラベル(EOF2)
                    mt_bufflen = MT_LABEL_SIZE
                    Dim EOF1_BUFF As String
                    Dim EOF2_BUFF As String
                    'Dim EOF1_BUFF As String * MT_LABEL_SIZE
                    'Dim EOF2_BUFF As String * MT_LABEL_SIZE
                    ' EOF1
                    EOF1_BUFF = "EOF1" & Mid(HDR1_REC_SAVE, 5, 50) & Format(ブロック数, "000000") _
                        & Mid(HDR1_REC_SAVE, 61, 20)
                    ' EOF2
                    EOF2_BUFF = "EOF2" & Mid(HDR2_REC_SAVE, 5, 76)
                    ErrStatus = mtwblock(mt_mtid, EOF1_BUFF, mt_bufflen)
                    ErrStatus = mtwblock(mt_mtid, EOF2_BUFF, mt_bufflen)
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(EOF)"
                    End If

                Case 1, 2 'NL
                    '----------------------------------------
                    'ノンラベル処理
                    '----------------------------------------
                    If Label = 2 Then
                        'NL(日立形式)
                        OutFileSeq = OutFileSeq + 1
                    End If
                    'ディスクへの複写処理

                    '指定ファイルまで読み飛ばし
                    FILE_CNT = 1

                    Do Until FILE_CNT >= (OutFileSeq - 1) + 1
                        ErrStatus = mtffile(mt_mtid)
                        If ErrStatus <> 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "FILESEQ-ERR(FILE_NOTFOUND) :" & OutFileSeq
                        End If
                        FILE_CNT = FILE_CNT + 1
                    Loop

                    'ブロック化／書込み
                    Dim ブロック数 As Integer
                    ErrStatus = 0
                    ブロック数 = mtWRITEtoMT(Blksize, Lrecl, InFileName, InCH13, ErrStatus)
                    If ErrStatus = 0 Then
                        If ブロック数 = 0 Then
                            ErrStatus = mtunload(mt_mtid)
                            Return "WRITE-ERR(DATA)" & "WRITE NO_REC"
                        End If
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE-ERR(DATA)"
                    End If

                    'テープマーク（２個）書込み
                    ErrStatus = mtwmtmk(mt_mtid)
                    If ErrStatus = 0 Then
                        ErrStatus = mtunload(mt_mtid)
                        Return ""
                    Else
                        ErrStatus = mtunload(mt_mtid)
                        Return "WRITE_ERR(TMK)"
                    End If
                Case Else
                    ErrStatus = mtunload(mt_mtid)
                    Return "LABEL-ERR : " & Label
            End Select

        Catch ex As Exception
            ErrStatus = mtunload(mt_mtid)
            Return "Exception" & ex.ToString
        End Try
    End Function

    Public Function mtOFFL(ByRef Blksize As Integer,
                           ByRef Lrecl As Integer,
                           ByRef Label As Integer,
                           ByRef InVol As String,
                           ByRef InFileSeq As Integer,
                           ByRef CodeNo As Integer,
                           ByRef InFileName As String,
                           ByRef OutFileName As String,
                           ByRef OutCH13 As Integer,
                           ByRef ErrStatus As Long) As String

        'Dim mtBitRtn As DEVICE_status
        'Dim MTRET As Long
        'Dim 改行 As String
        'Dim 改行            As String * 2
        '改行 = Chr(13) & Chr(10)
        'Dim 記録密度 As String
        'Dim 記録密度        As String * 10
        'Dim 設定記録密度 As Integer
        'Dim プロテクト As String
        'Dim プロテクト      As String * 10
        'Dim Msg As String
        'Dim Msg_Buttons As Integer
        'Dim Msg_Title As String
        'Dim Reply As Integer
        'Dim 接続装置 As Byte
        'Dim 装置情報 As String
        'Dim MT装置認識 As Boolean

        'Msg_Title = "ＭＴオフライン処理"
        'Msg_Buttons = vbOKOnly
        'MT装置認識 = False

        '----------------------------------------
        'ＭＴオフライン処理
        '----------------------------------------
        ErrStatus = mtunload(mt_mtid)

        Return ""

    End Function

    Public Function mtONL(ByRef In_mt_mtid As Integer, _
                          ByRef In_Mode As Integer) As Long

        Dim mtBitRtn As DEVICE_status
        Dim MTRET As Long
        'Dim 改行 As String
        ''Dim 改行            As String * 2
        '改行 = Chr(13) & Chr(10)
        Dim 記録密度 As String
        'Dim 記録密度        As String * 10
        Dim 設定記録密度 As Integer
        Dim プロテクト As String
        'Dim プロテクト      As String * 10
        Dim Msg As String
        Dim Msg_Buttons As Integer
        Dim Msg_Title As String
        'Dim Reply As Integer
        Dim 接続装置 As Byte
        Dim 装置情報 As String
        Dim MT装置認識 As Boolean

        Msg_Title = "ＭＴオンライン処理"
        Msg_Buttons = vbOKOnly
        MT装置認識 = False

        Try
            '----------------------------------------
            'ＭＴオンライン処理
            '----------------------------------------
            Do
                '----------------------------------------
                '装置ステータス判定
                '----------------------------------------
                '一回目はダミー実行する（MTDLL-52の仕様）
                mt_rtn = mtstat(In_mt_mtid)
                mt_rtn = 0

                '----------------------------------------
                'ＭＴ装置の初期化
                '(戻り値には接続されている装置台数がセットされる)
                '----------------------------------------
                mt_rtn = mtinit(T_Mtinfo(1))
                If mt_rtn = 0 OrElse mt_rtn = 255 Then
                    Msg = "ＭＴ装置が接続されていません。" & vbCrLf & _
                        "エラーコード：" & mt_rtn
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                Else
                    接続装置 = 1
                End If

                Do Until MT装置認識 = True Or 接続装置 > mt_rtn

                    If T_Mtinfo(接続装置).UnitNo = In_mt_mtid Then
                        MT装置認識 = True
                    End If

                    装置情報 = Chr(T_Mtinfo(接続装置).Vender(0)) & Chr(T_Mtinfo(接続装置).Vender(1)) & Chr(T_Mtinfo(接続装置).Vender(2)) _
                             & Chr(T_Mtinfo(接続装置).Vender(3)) & Chr(T_Mtinfo(接続装置).Vender(4)) & Chr(T_Mtinfo(接続装置).Vender(5)) _
                             & Chr(T_Mtinfo(接続装置).Vender(6)) & Chr(T_Mtinfo(接続装置).Vender(7))

                    装置情報 = 装置情報 & Chr(T_Mtinfo(接続装置).Product(0)) & Chr(T_Mtinfo(接続装置).Product(1)) & Chr(T_Mtinfo(接続装置).Product(2)) _
                             & Chr(T_Mtinfo(接続装置).Product(3)) & Chr(T_Mtinfo(接続装置).Product(4)) & Chr(T_Mtinfo(接続装置).Product(5)) _
                             & Chr(T_Mtinfo(接続装置).Product(6)) & Chr(T_Mtinfo(接続装置).Product(7))

                    接続装置 = 接続装置 + 1

                Loop

                If MT装置認識 <> True Then
                    Msg = "ＭＴのアクセスに失敗しました。（装置初期化）" & vbCrLf & _
                        "ユニットＩＤ：" & In_mt_mtid
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                End If

                '----------------------------------------
                'ＭＴのマウント要求メッセージ出力
                '----------------------------------------
                Msg = "ＭＴをセットしてください。"
                If MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                    mt_mtid = In_mt_mtid
                Else
                    Return -1
                End If

                '----------------------------------------
                '装置のオンライン
                '----------------------------------------
                mt_rtn = mtonline(mt_mtid)
                If mt_rtn = 0 Then
                    Exit Do
                Else
                    Msg = "ＭＴのオンラインに失敗しました。" & vbCrLf & _
                        "エラーコード：" & mt_rtn & vbCrLf & _
                        "処理を継続しますか？"
                    If MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then

                    Else
                        Return -1
                    End If
                End If
            Loop

            '----------------------------------------
            '装置ステータス判定
            '----------------------------------------
            '一回目はダミー実行する（MTDLL-52の仕様）
            mt_rtn = mtstat(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                    Else
                        記録密度 = "1600BPI"
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                    Else
                        記録密度 = "800BPI"
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select

            'プロテクト判定
            If In_Mode = 0 Then         'read-only
            Else
                Select Case mtBitRtn.BIT_PRO
                    Case 0
                        プロテクト = "書込可能"
                    Case 1
                        プロテクト = "書込禁止"
                        Msg = "書込禁止です。プロテクトを解除してください。"
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return -1
                    Case Else
                        Msg = "ステータス変換エラーが発生しました。（プロテクト）" & vbCrLf & _
                            "ステータス：" & mtBitRtn.BIT_PRO
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return -1
                End Select
            End If

            '記録密度設定
            mt_rtn = mtfblock(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                        設定記録密度 = 3
                    Else
                        記録密度 = "1600BPI"
                        設定記録密度 = 1
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                        設定記録密度 = 2
                    Else
                        記録密度 = "800BPI"
                        設定記録密度 = 0
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select
            mt_rtn = mtonline(mt_mtid)
            mt_rtn = mtdensity(mt_mtid, 設定記録密度)

            Return 0

        Catch ex As Exception
            Msg = "例外が発生しました。" & vbCrLf & ex.ToString
            MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1
        End Try
    End Function

    Public Function mtONL_CHK(ByRef In_mt_mtid As Integer, _
                              ByRef In_Mode As Integer) As Long

        Dim mtBitRtn As DEVICE_status
        Dim MTRET As Long
        'Dim 改行 As String
        ''Dim 改行            As String * 2
        '改行 = Chr(13) & Chr(10)
        Dim 記録密度 As String
        'Dim 記録密度        As String * 10
        Dim 設定記録密度 As Integer
        Dim プロテクト As String
        'Dim プロテクト      As String * 10
        Dim Msg As String
        Dim Msg_Buttons As Integer
        Dim Msg_Title As String
        'Dim Reply As Integer
        Dim 接続装置 As Byte
        Dim 装置情報 As String
        Dim MT装置認識 As Boolean

        Msg_Title = "ＭＴオンライン処理"
        Msg_Buttons = vbOKOnly
        MT装置認識 = False

        Try
            '----------------------------------------
            'ＭＴオンライン処理
            '----------------------------------------
            Do
                '----------------------------------------
                '装置ステータス判定
                '----------------------------------------
                '一回目はダミー実行する（MTDLL-52の仕様）
                mt_rtn = mtstat(In_mt_mtid)
                mt_rtn = 0

                '----------------------------------------
                'ＭＴ装置の初期化
                '(戻り値には接続されている装置台数がセットされる)
                '----------------------------------------
                mt_rtn = mtinit(T_Mtinfo(1))
                If mt_rtn = 0 OrElse mt_rtn = 255 Then
                    Msg = "ＭＴ装置が接続されていません。" & vbCrLf & _
                        "エラーコード：" & mt_rtn
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                Else
                    接続装置 = 1
                End If

                Do Until MT装置認識 = True Or 接続装置 > mt_rtn

                    If T_Mtinfo(接続装置).UnitNo = In_mt_mtid Then
                        MT装置認識 = True
                    End If

                    装置情報 = Chr(T_Mtinfo(接続装置).Vender(0)) & Chr(T_Mtinfo(接続装置).Vender(1)) & Chr(T_Mtinfo(接続装置).Vender(2)) _
                             & Chr(T_Mtinfo(接続装置).Vender(3)) & Chr(T_Mtinfo(接続装置).Vender(4)) & Chr(T_Mtinfo(接続装置).Vender(5)) _
                             & Chr(T_Mtinfo(接続装置).Vender(6)) & Chr(T_Mtinfo(接続装置).Vender(7))

                    装置情報 = 装置情報 & Chr(T_Mtinfo(接続装置).Product(0)) & Chr(T_Mtinfo(接続装置).Product(1)) & Chr(T_Mtinfo(接続装置).Product(2)) _
                             & Chr(T_Mtinfo(接続装置).Product(3)) & Chr(T_Mtinfo(接続装置).Product(4)) & Chr(T_Mtinfo(接続装置).Product(5)) _
                             & Chr(T_Mtinfo(接続装置).Product(6)) & Chr(T_Mtinfo(接続装置).Product(7))

                    接続装置 = 接続装置 + 1

                Loop

                If MT装置認識 <> True Then
                    Msg = "ＭＴのアクセスに失敗しました。（装置初期化）" & vbCrLf & _
                        "ユニットＩＤ：" & In_mt_mtid
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
                End If

                ''----------------------------------------
                ''ＭＴのマウント要求メッセージ出力
                ''----------------------------------------
                'Msg = "ＭＴをセットしてください。"
                'If MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                '    mt_mtid = In_mt_mtid
                'Else
                '    Return -1
                'End If

                '----------------------------------------
                '装置のオンライン
                '----------------------------------------
                mt_rtn = 0
                'mt_rtn = mtonline(mt_mtid)
                If mt_rtn = 0 Then
                    Exit Do
                Else
                    Msg = "ＭＴのオンラインに失敗しました。" & vbCrLf & _
                        "エラーコード：" & mt_rtn & vbCrLf & _
                        "処理を継続しますか？"
                    If MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then

                    Else
                        Return -1
                    End If
                End If
            Loop

            '----------------------------------------
            '装置ステータス判定
            '----------------------------------------
            '一回目はダミー実行する（MTDLL-52の仕様）
            mt_rtn = mtstat(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                    Else
                        記録密度 = "1600BPI"
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                    Else
                        記録密度 = "800BPI"
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select

            'プロテクト判定
            If In_Mode = 0 Then         'read-only
            Else
                Select Case mtBitRtn.BIT_PRO
                    Case 0
                        プロテクト = "書込可能"
                    Case 1
                        プロテクト = "書込禁止"
                        Msg = "書込禁止です。プロテクトを解除してください。"
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return -1
                    Case Else
                        Msg = "ステータス変換エラーが発生しました。（プロテクト）" & vbCrLf & _
                            "ステータス：" & mtBitRtn.BIT_PRO
                        MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return -1
                End Select
            End If

            '記録密度設定
            mt_rtn = mtfblock(mt_mtid)
            mt_rtn = mtstat(mt_mtid)
            MTRET = mtCHGStatus(mt_rtn, mtBitRtn)
            If MTRET <> 0 Then
                Msg = "ステータス変換エラーが発生しました。" & vbCrLf & _
                    "エラーコード：" & MTRET
                MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return -1
            End If

            '記録密度判定
            Select Case mtBitRtn.BIT_DEN0
                Case 1
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "6250BPI"
                        設定記録密度 = 3
                    Else
                        記録密度 = "1600BPI"
                        設定記録密度 = 1
                    End If
                Case 0
                    If mtBitRtn.BIT_DEN1 = 1 Then
                        記録密度 = "3200BPI"
                        設定記録密度 = 2
                    Else
                        記録密度 = "800BPI"
                        設定記録密度 = 0
                    End If
                Case Else
                    Msg = "ステータス変換エラーが発生しました。（記録密度）" & vbCrLf & _
                        "ステータス：" & mtBitRtn.BIT_DEN0 & mtBitRtn.BIT_DEN1
                    MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return -1
            End Select
            mt_rtn = mtonline(mt_mtid)
            mt_rtn = mtdensity(mt_mtid, 設定記録密度)

            Return 0

        Catch ex As Exception
            Msg = "例外が発生しました。" & vbCrLf & ex.ToString
            MessageBox.Show(Msg, Msg_Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1
        End Try
    End Function

    Public Function mtWRITEtoDISK(ByRef Blksize As Integer, _
                                  ByRef Lrecl As Integer, _
                                  ByRef OutFileName As String, _
                                  ByRef OutCH13 As Integer, _
                                  ByRef ErrStatus As Long) As Long

        Dim mtBitRtn As DEVICE_status
        Dim I As Integer
        Dim MTRET As Integer
        Dim WRITE_REC As String
        'Dim READ_BLOCK As String
        'Dim 書込みブロック数 As Long
        Dim レコード番号 As Long
        Dim ブロック化係数 As Integer
        Dim 改行コード As String

        改行コード = Chr(13)
        レコード番号 = 1      '一件目の読込みは１バイト目

        Dim intFILE_NO_1 As Integer

        Try
            '----------------------------------------
            'ディスクファイル存在チェック
            '----------------------------------------
            '出力ファイル削除
            If System.IO.File.Exists(OutFileName) Then
                System.IO.File.Delete(OutFileName)
            End If
            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, OutFileName, OpenMode.Binary)

            '----------------------------------------
            'アンブロック化／書込み
            '----------------------------------------
            mt_bufflen = Blksize
            Dim 終了フラグ As Integer
            Dim 読込みブロック数 As Long
            終了フラグ = 0
            読込みブロック数 = 0

            'MTブロックリード
            ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
            MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
            If mtBitRtn.BIT_TMK = 0 Then
                If mtBitRtn.BIT_DTE = 0 Then
                    If mtBitRtn.BIT_HDE = 0 Then
                        読込みブロック数 = 読込みブロック数 + 1
                    Else
                        終了フラグ = 1
                    End If
                Else
                    終了フラグ = 1
                End If
            Else
                終了フラグ = 1
            End If

            '----------------------------------------
            'OutCH13 = 0 '改行コードなし処理
            'OutCH13 = 1 '改行コード付加処理
            '----------------------------------------
            If OutCH13 = 0 Then
                '入出力レコード領域確保(入力ﾚｺｰﾄﾞ長 )*******************
                WRITE_REC = Space(Lrecl)
                Do Until 終了フラグ = 1
                    ブロック化係数 = (mt_count \ Lrecl)
                    レコード番号 = 1
                    For I = 1 To ブロック化係数
                        WRITE_REC = Mid(mt_buffer, (レコード番号 - 1) * Lrecl + 1, Lrecl)
                        FilePut(intFILE_NO_1, WRITE_REC)
                        レコード番号 = レコード番号 + 1
                    Next

                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                    If mtBitRtn.BIT_TMK = 0 Then
                        If mtBitRtn.BIT_DTE = 0 Then
                            If mtBitRtn.BIT_HDE = 0 Then
                                読込みブロック数 = 読込みブロック数 + 1
                            Else
                                終了フラグ = 1
                            End If
                        Else
                            終了フラグ = 1
                        End If
                    Else
                        終了フラグ = 1
                    End If
                Loop
            Else
                '入出力レコード領域確保(入力ﾚｺｰﾄﾞ長 + 改行ｺｰﾄﾞ)************
                WRITE_REC = Space(Lrecl + 1)
                Do Until 終了フラグ = 1
                    ブロック化係数 = (mt_count \ Lrecl)
                    レコード番号 = 1
                    For I = 1 To ブロック化係数
                        WRITE_REC = Mid(mt_buffer, (レコード番号 - 1) * Lrecl + 1, Lrecl) & 改行コード
                        FilePut(intFILE_NO_1, WRITE_REC)
                        レコード番号 = レコード番号 + 1
                    Next

                    ErrStatus = mtrblock(mt_mtid, mt_buffer, mt_count, mt_bufflen)
                    MTRET = mtCHGStatus(ErrStatus, mtBitRtn)
                    If mtBitRtn.BIT_TMK = 0 Then
                        If mtBitRtn.BIT_DTE = 0 Then
                            If mtBitRtn.BIT_HDE = 0 Then
                                読込みブロック数 = 読込みブロック数 + 1
                            Else
                                終了フラグ = 1
                            End If
                        Else
                            終了フラグ = 1
                        End If
                    Else
                        終了フラグ = 1
                    End If
                Loop
            End If

            FileClose(intFILE_NO_1)

            Return 読込みブロック数

        Catch ex As Exception
            Return 0
        Finally
            FileClose(intFILE_NO_1)
        End Try
    End Function

    Public Function mtWRITEtoMT(ByRef Blksize As Integer, _
                                ByRef Lrecl As Integer, _
                                ByRef InFileName As String, _
                                ByRef InCH13 As Integer, _
                                ByRef ErrStatus As Long) As Integer

        'Dim mtBitRtn As DEVICE_status
        Dim I As Integer
        Dim WRITE_REC As String = ""
        Dim WRITE_BLOCK As String
        Dim READ_REC As String
        Dim 書込みブロック数 As Long
        Dim レコード番号 As Long
        Dim ブロック化係数 As Integer
        Dim 改行コード As String
        'Dim 改行コード        As String * 1
        Dim LAST_REC_LEN As Integer

        改行コード = Chr(13)
        ブロック化係数 = Blksize \ Lrecl
        レコード番号 = 1      '一件目の読込みは１バイト目

        Dim intFILE_NO_1 As Integer

        Try
            '----------------------------------------
            'ディスクファイル存在チェック
            '----------------------------------------
            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, InFileName, OpenMode.Binary)
            If LOF(intFILE_NO_1) >= Lrecl Then
            Else
                If LOF(intFILE_NO_1) = 0 Then
                Else

                End If
            End If

            '----------------------------------------
            'ブロック化／書込み
            '----------------------------------------
            '入出力レコード領域確保
            READ_REC = Space(Lrecl)
            WRITE_BLOCK = Space(Blksize)

            '----------------------------------------
            'OutCH13 = 0 '改行コードなし処理
            'OutCH13 = 1 '改行コード付加処理
            '----------------------------------------
            If InCH13 = 0 Then
                Do Until EOF(intFILE_NO_1) = True
                    For I = 0 To (ブロック化係数 - 1)
                        FileGet(intFILE_NO_1, READ_REC, ((レコード番号 - 1) * (Lrecl) + 1))

                        If EOF(intFILE_NO_1) = True Then
                            If I = 0 Then
                                FileClose(intFILE_NO_1)
                                Return 書込みブロック数
                            Else
                                WRITE_BLOCK = WRITE_REC
                                '最終ブロック書込み
                                LAST_REC_LEN = Len(WRITE_BLOCK)
                                WRITE_BLOCK = WRITE_BLOCK & Space(Blksize - Len(WRITE_BLOCK))
                                ErrStatus = mtwblock(mt_mtid, WRITE_BLOCK, LAST_REC_LEN)
                                If ErrStatus = 0 Then
                                    書込みブロック数 = 書込みブロック数 + 1
                                    WRITE_REC = ""
                                    WRITE_BLOCK = ""
                                Else
                                    FileClose(intFILE_NO_1)
                                    Return 書込みブロック数
                                End If
                            End If
                        Else
                            WRITE_REC = WRITE_REC & READ_REC
                        End If
                        レコード番号 = レコード番号 + 1
                    Next
                    WRITE_BLOCK = WRITE_REC
                    ErrStatus = mtwblock(mt_mtid, WRITE_BLOCK, Blksize)
                    If ErrStatus = 0 Then
                        書込みブロック数 = 書込みブロック数 + 1
                        WRITE_REC = ""
                        WRITE_BLOCK = ""
                    Else
                        FileClose(intFILE_NO_1)
                        Return 書込みブロック数
                    End If
                Loop
            Else
                Do Until EOF(intFILE_NO_1) = True
                    For I = 0 To (ブロック化係数 - 1)
                        FileGet(intFILE_NO_1, READ_REC, ((レコード番号 - 1) * (Lrecl + 2) + 1))

                        If EOF(intFILE_NO_1) = True Then
                            If I = 0 Then
                                FileClose(intFILE_NO_1)
                                Return 書込みブロック数
                            Else
                                WRITE_BLOCK = WRITE_REC
                                '最終ブロック書込み
                                LAST_REC_LEN = Len(WRITE_BLOCK)
                                WRITE_BLOCK = WRITE_BLOCK & Space(Blksize - Len(WRITE_BLOCK))
                                ErrStatus = mtwblock(mt_mtid, WRITE_BLOCK, LAST_REC_LEN)
                                If ErrStatus = 0 Then
                                    書込みブロック数 = 書込みブロック数 + 1
                                    WRITE_REC = ""
                                    WRITE_BLOCK = ""
                                Else
                                    FileClose(intFILE_NO_1)
                                    Return 書込みブロック数
                                End If
                            End If
                        Else
                            WRITE_REC = WRITE_REC & READ_REC
                        End If
                        レコード番号 = レコード番号 + 1
                    Next
                    WRITE_BLOCK = WRITE_REC
                    ErrStatus = mtwblock(mt_mtid, WRITE_BLOCK, Blksize)
                    If ErrStatus = 0 Then
                        書込みブロック数 = 書込みブロック数 + 1
                        WRITE_REC = ""
                        WRITE_BLOCK = ""
                    Else
                        FileClose(intFILE_NO_1)
                        Return 書込みブロック数
                    End If
                Loop
            End If

            FileClose(intFILE_NO_1)
            Return 書込みブロック数

        Catch ex As Exception
            Return 0
        Finally
            FileClose(intFILE_NO_1)
        End Try
    End Function
End Class
