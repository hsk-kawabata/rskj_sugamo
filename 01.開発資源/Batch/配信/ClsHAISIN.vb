Imports System
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' 配信データ作成メインクラス
''' </summary>
''' <remarks></remarks>
Public Class ClsHAISIN

#Region "クラス定数"
    ' 処理日付
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")
#End Region

#Region "クラス変数"
    Private HAISIN_DATE As String
    Private FURI_DATE As String
    Private SOUSIN_KBN As String
    Private CYCLE As String

    Private Structure strcIniInfo
        Dim CENTER As String
        Dim DEN As String
        Dim DAT As String
        Dim DATBK As String
        Dim KINKOCD As String
        Dim PRTHAISIN As String
        Dim SITEIKINKOCD As String
        Dim FILEAM As String
        Dim FILEPM As String
        Dim KINKONAME As String
        Dim KINKOTANTO As String
        Dim KINKOTEL As String
        Dim KINKOBUSYO As String
        Dim CENTER_MOTIKOMI As String
        Dim SOUSINRENRAKU As String
        Dim KITEIKEN As String
        Dim BORDER_TIME As String
        Dim FTR As String
        Dim FTRANP As String
        ' 2015/12/14 タスク）綾部 ADD 【PG】UI_B-14-04(RSV2対応) -------------------- START
        Dim KUMIAI_SOUFU As String
        Dim KUMIAI_SOUFU1 As String
        Dim KUMIAI_SOUFU2 As String
        Dim KUMIAI_SOUFU3 As String
        Dim KIGYO_SOUFU As String
        Dim KIGYO_SOUFU1 As String
        Dim KIGYO_SOUFU2 As String
        Dim KIGYO_SOUFU3 As String
        ' 2015/12/14 タスク）綾部 ADD 【PG】UI_B-14-04(RSV2対応) -------------------- END
        ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- START
        Dim SYORIKEKKA_HAISIN As String
        ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- END
        ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
        Dim JIFURI_CCFNAME As String
        ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END
    End Structure
    Private IniInfo As strcIniInfo

    Private Structure strcSchmastInfo
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim FURI_DATE As String
        Dim FMT_KBN As String
        Dim TAKO_KBN As String
        Dim KIGYO_CODE As String
        Dim FURI_CODE As String
        Dim ITAKU_KANRI_CODE As String
        Dim ITAKU_CODE As String
        Dim ITAKU_NNAME As String
        Dim NS_KBN As String
        Dim MULTI_KBN As String
        Dim FURI_KYU_CODE As String
        Dim FILE_SEQ As String

        Dim SYORI_KEN As String
        Dim SYORI_KIN As String

        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            FURI_DATE = ""
            FMT_KBN = ""
            TAKO_KBN = ""
            KIGYO_CODE = ""
            FURI_CODE = ""
            ITAKU_KANRI_CODE = ""
            ITAKU_CODE = ""
            ITAKU_NNAME = ""
            NS_KBN = ""
            MULTI_KBN = ""
            FURI_KYU_CODE = ""
            FILE_SEQ = ""

            SYORI_KEN = ""
            SYORI_KIN = ""
        End Sub

        Friend Sub SetOraDataHaisin(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_T")
            TORIF_CODE = oraReader.GetString("TORIF_CODE_T")
            FURI_DATE = oraReader.GetString("FURI_DATE_S")
            FMT_KBN = oraReader.GetString("FMT_KBN_T")
            TAKO_KBN = oraReader.GetString("TAKO_KBN_T")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_T")
            FURI_CODE = oraReader.GetString("FURI_CODE_T")
            ITAKU_KANRI_CODE = oraReader.GetString("ITAKU_KANRI_CODE_T")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
            NS_KBN = oraReader.GetString("NS_KBN_T")
            MULTI_KBN = oraReader.GetString("MULTI_KBN_T")
            FURI_KYU_CODE = oraReader.GetString("FURI_KYU_CODE_T")
            FILE_SEQ = oraReader.GetString("FILE_SEQ_S")

            SYORI_KEN = oraReader.GetString("SYORI_KEN_S")
            SYORI_KIN = oraReader.GetString("SYORI_KIN_S")
        End Sub
    End Structure

    '指定金庫格納用
    Private strSITEIKINKO_CODE(50) As String
    Private intSITEIKINKO_KEN As Integer

    Private strWRK_FILE As String
    Private intRECORD_CNT As Integer
    Private strSEND_FILE As String
    Private strJIFURI_FILE As String

    '企業シーケンス用
    '*** 修正 mitsu 2009/09/17 企業シーケンス用 ***
    Private intJIFURI_SEQ As Integer
    Private htJIFURI_SEQ As New Hashtable
    '**********************************************

    '金額0円のデータ、振替結果が0以外のデータの合計
    Private dblFUNOU_KEN As Double
    Private dblFUNOU_KIN As Double
    'センターに送信分の件数、金額の合計
    Private dblSOUSIN_KEN As Double
    Private dblSOUSIN_KIN As Double
    '全体の件数、金額の合計
    Private dblALL_KEN As Double
    Private dblALL_KIN As Double

    '蒲郡信金向け　4万件対応 2007/08/17
    Private lngCYCLE As Long = 0
    Private strWRK_FILE_B As String
    Private intRECORD_CNT_B As Integer

    Private MainDB As CASTCommon.MyOracle

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 初期処理を行います。
    ''' </summary>
    ''' <param name="CmdArgs">コマンドライン引数</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function FirstInit(ByVal CmdArgs() As String) As Boolean
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


        Try
            '-----------------------------------------
            'パラメータチェック
            '-----------------------------------------
            Dim param() As String
            param = CmdArgs(0).Split(","c)
            If param.Length = 5 Then

                '------------------------------------------
                'ログ書込み情報の設定
                '------------------------------------------
                MainLOG.FuriDate = param(1)
                MainLOG.JobTuuban = CType(param(4), Integer)
                MainLOG.ToriCode = "000000000000" '*** Str Add 2015/12/01 sys)mori for LOG強化対応 ***

                MainLOG.Write("(初期処理)開始", "成功")

                HAISIN_DATE = param(0)
                FURI_DATE = param(1)
                SOUSIN_KBN = param(2)
                CYCLE = param(3)

                MainLOG.Write("パラメータ取得", "成功", "サイクル番号：" & CYCLE)

                If HAISIN_DATE < strDate Then
                    MainLOG.Write("パラメータチェック", "失敗", "送信日が過ぎている。")
                    MainLOG.UpdateJOBMASTbyErr("送信日が過ぎています。サイクル番号：" & CYCLE)
                    Return False
                End If

                '------------------------------------------
                'INIファイルの読み込み
                '------------------------------------------
                Dim ErrMsg As String = String.Empty
                If fn_INI_READ(ErrMsg) = False Then
                    MainLOG.UpdateJOBMASTbyErr(ErrMsg)
                    Exit Function
                End If

            Else
                MainLOG.Write("(初期処理)開始", "失敗", "コマンドライン引数のパラメータが不正")
                MainLOG.UpdateJOBMASTbyErr("コマンドライン引数のパラメータが不正[" & CmdArgs(0) & "]")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("初期処理", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("パラメータ取得失敗[" & CmdArgs(0) & "]" & ex.Message)
            Return False
        Finally
            MainLOG.Write("(初期処理)終了", "成功")
        End Try

        Return True

    End Function

    Public Function Main(ByVal command As String) As Integer

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 60
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME1")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 60
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

        MainDB = New CASTCommon.MyOracle

        Try
            MainLOG.Write("(主処理)開始", "成功")

            MainDB.BeginTrans()     ' トランザクション開始

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("主処理", "失敗", "センターカットデータ作成処理で実行待ちタイムアウト")
                MainLOG.UpdateJOBMASTbyErr("センターカットデータ作成処理で実行待ちタイムアウト")
                Return -1
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            '-----------------------------------------
            '送信区分により分岐
            '-----------------------------------------
            Select Case SOUSIN_KBN
                Case "0"        '金庫、組合持込フォーマット
                    If fn_MAIN_00() = False Then
                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                        ' ジョブ実行アンロック
                        dblock.Job_UnLock(MainDB)
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                        MainDB.Rollback()
                        Return -1
                    End If

                Case "1"        '全銀フォーマット
                    If fn_MAIN_01() = False Then
                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                        ' ジョブ実行アンロック
                        dblock.Job_UnLock(MainDB)
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                        MainDB.Rollback()
                        Return -1
                    End If

                Case Else
                    MainLOG.Write("主処理", "失敗", "送信区分異常：" & SOUSIN_KBN)
                    MainLOG.UpdateJOBMASTbyErr("送信区分異常：" & SOUSIN_KBN)
                    Return -1
            End Select

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            MainDB.Commit()

            '-----------------------------------------
            '口座振替明細表の印刷
            '-----------------------------------------
            If IniInfo.PRTHAISIN > 0 Then

                Dim ExeRepo As New CAstReports.ClsExecute ' レポエージェント印刷
                Dim Param As String = ""
                Dim DQ As String = """"
                Param &= MainLOG.UserID & ",000000000000," & strDate & strTime
                MainLOG.Write("口座振替明細表出力開始", "成功", "パラメータ" & Param)

                Dim nRet As Integer = ExeRepo.ExecReport("KFJP010.EXE", Param)
                If nRet = 0 Then
                    MainLOG.Write("口座振替明細表出力", "成功", "パラメータ" & Param)
                Else
                    MainLOG.Write("口座振替明細表出力", "失敗", "パラメータ" & Param)
                End If
                MainLOG.Write("口座振替明細表出力終了", "成功", "")

            End If

            ' 2017/01/16 タスク）綾部 ADD 【IT】UI_99-99(RSV2 伝送システム連携機能対応) -------------------- START
            '===============================================
            ' 伝送システム宛　データ送信処理
            '===============================================
            If CASTCommon.GetRSKJIni("RSV2_V1.0.0", "USE_DENSO_CC") = "1" Then
                Dim DENSO_EXE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "DENSO_EXE")

                Dim Proc As New Process
                Dim ProcInfo As New ProcessStartInfo
                ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), DENSO_EXE)
                ProcInfo.WorkingDirectory = ""
                ProcInfo.Arguments = strJIFURI_FILE
                Proc = Process.Start(ProcInfo)
                Proc.WaitForExit()

                If Proc.ExitCode = 0 Then
                    ' 連携成功
                    MainLOG.Write("主処理", "成功", "センターカットデータ伝送送信処理 Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                    Call MainLOG.UpdateJOBMASTbyOK("伝送完了")
                Else
                    ' 連携失敗
                    MainLOG.Write("主処理", "失敗", "センターカットデータ伝送送信処理 Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                End If
            End If
            ' 2017/01/16 タスク）綾部 ADD 【IT】UI_99-99(RSV2 伝送システム連携機能対応) -------------------- END

            '-----------------------------------------
            'ジョブマスタの更新
            '-----------------------------------------
            MainLOG.UpdateJOBMASTbyOK("")

        Catch ex As Exception
            MainLOG.Write("主処理", "失敗", ex.Message)
            Return 1
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            If Not MainDB Is Nothing Then
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

                ' ロールバック
                MainDB.Rollback()
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write("(主処理)終了", "成功")

        End Try

        Return 0

    End Function


#End Region

#Region "プライベートメソッド"

    Private Function fn_MAIN_00() As Boolean

        Dim oraSchReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Dim arySyoriKekka As New ArrayList
        Dim intSCH_CNT As Integer = 0

        Dim WrkFmt As New CAstFormat.CFormatShinkinWork200

        '--------------------------------------------------------------------------
        'センターカットデータを分割できるように別ファイルにも同時書き込み
        '--------------------------------------------------------------------------
        Dim WriteA As StreamWriter = Nothing
        Dim WriteB As StreamWriter = Nothing

        '*** 修正 mitsu 2008/09/02 ループの外へ移動 ***
        '和暦変換
        Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
        culture.DateTimeFormat.Calendar = New JapaneseCalendar
        '**********************************************

        Try
            '-----------------------------------------
            '自振ヘッダーレコード設定
            '-----------------------------------------
            Call SetHeaderRecordWorkFile(WrkFmt)

            '-----------------------------------------
            '自振エンドレコード設定
            '-----------------------------------------
            Call SetEndRecordWorkFile(WrkFmt)

            '-----------------------------------------
            '対象スケジュール検索
            '-----------------------------------------
            Dim Key As strcSchMastInfo = Nothing

            If oraSchReader.DataReader(CreateGetSchmastSQL("00")) = True Then

                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraSchReader.EOF = False
                    intSCH_CNT += 1

                    ' キー初期化
                    Key.Init()
                    ' キー設定
                    Call Key.SetOraDataHaisin(oraSchReader)

                    MainLOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                    MainLOG.FuriDate = Key.FURI_DATE

                    '*** 修正 mitsu 2008/09/17 振替日単位で企業シーケンスを設定する ***
                    If htJIFURI_SEQ.ContainsKey(Key.FURI_DATE) Then
                        '振替日の企業シーケンスが存在する場合
                        intJIFURI_SEQ = htJIFURI_SEQ.Item(Key.FURI_DATE)
                    Else
                        '振替日の企業シーケンスが存在しない場合は明細マスタを検索する
                        intJIFURI_SEQ = GetJifuriSEQ(Key)
                    End If
                    '******************************************************************

                    '*** 修正 mitsu 2008/09/02 ループの外へ移動 ***
                    Dim NHKflag As Boolean = True
                    ' NHK判定
                    '*** 修正 mitsu 2008/08/19 処理高速化 ***
                    'If gdbrREADER.Item("KIGYO_CODE_S") <> "100" And _
                    '    gdbrREADER.Item("FURI_CODE_S") <> "010" Then
                    If Key.KIGYO_CODE <> "40100" AndAlso _
                      Key.FURI_DATE <> "010" Then
                        '************************************
                        ' 2008.04.22 間違い訂正 MODIFY
                        'NHKflag = True     
                        NHKflag = False
                    End If
                    '**********************************************

                    '*** 修正 mitsu 2008/09/02 ループの外へ移動 ***
                    '和暦変換
                    Dim strWORK_DATE As String
                    strWORK_DATE = Key.FURI_DATE.Trim
                    Dim target As DateTime = New DateTime(strWORK_DATE.Substring(0, 4), strWORK_DATE.Substring(4, 2), strWORK_DATE.Substring(6, 2))
                    Dim strWarekiFuriDate As String = target.ToString("yyMMdd", culture)
                    '**********************************************

                    If intRECORD_CNT = 0 Then
                        '------------------------------------------------
                        '中間ファイルのオープン（D:\RSKJ\DATBK\WRK001.DAT）
                        '------------------------------------------------
                        strWRK_FILE = Path.Combine(IniInfo.DATBK, "WRK00" & CYCLE & ".DAT")
                        If Dir(strWRK_FILE) <> "" Then
                            Kill(strWRK_FILE)
                        End If

                        '--------------------------------------------------------------------------
                        'センターカットデータを分割できるように別ファイルにも同時書き込み
                        '--------------------------------------------------------------------------
                        lngCYCLE = CInt(CYCLE)  '初期サイクル番号取得
                        strWRK_FILE_B = Path.Combine(IniInfo.DATBK, "WRK00" & lngCYCLE & "_B.DAT")
                        If Dir(strWRK_FILE_B) <> "" Then
                            Kill(strWRK_FILE_B)
                        End If

                        Try
                            WriteA = New StreamWriter(strWRK_FILE, False, Encoding.GetEncoding("SHIFT-JIS"))
                            intRECORD_CNT = 1
                            '--------------------------------
                            'ヘッダーレコードの書込み
                            '--------------------------------
                            WriteA.Write(WrkFmt.JF_DATA1.Data)

                            '--------------------------------------------------------------------------
                            'センターカットデータを分割できるように別ファイルにも同時書き込み
                            '--------------------------------------------------------------------------
                            intRECORD_CNT_B = 1
                            '--------------------------------
                            'ヘッダーレコードの書込み
                            '--------------------------------
                            WriteB = New StreamWriter(strWRK_FILE_B, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
                            WriteB.Write(WrkFmt.JF_DATA1.Data)

                        Catch ex As Exception
                            MainLOG.Write("配信ワークファイル作成", "失敗", ex.Message)
                            MainLOG.UpdateJOBMASTbyErr("配信ワークファイル作成失敗 例外発生")
                            If Not WriteA Is Nothing Then
                                WriteA.Close()
                                WriteA = Nothing
                            End If
                            If Not WriteB Is Nothing Then
                                WriteB.Close()
                                WriteB = Nothing
                            End If
                            Return False
                        End Try
                    End If

                    '-----------------------------------------
                    '明細マスタの検索（MEIMAST）
                    '-----------------------------------------
                    If oraMeiReader.DataReader(CreateGetMeimastSQL(Key, "00")) = True Then

                        While oraMeiReader.EOF = False
                            If (NHKflag = True AndAlso Not oraMeiReader.GetInt("FURIKETU_CODE_K") = 0) OrElse _
                                (NHKflag = False AndAlso (oraMeiReader.GetInt64("FURIKIN_K") = 0 OrElse Not oraMeiReader.GetInt("FURIKETU_CODE_K") = 0)) Then
                                'センターに送信しない。件数、金額を計算する。
                                dblFUNOU_KEN += 1
                                dblFUNOU_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                            Else
                                If SetDataRecordWorkFile(WrkFmt, Key, oraMeiReader, strWarekiFuriDate) = False Then
                                    Return False
                                End If

                                '企業シーケンスをMEIMASTに更新する
                                If UpdateJifuriSEQ(Key, oraMeiReader) = False Then
                                    Return False
                                End If

                                '--------------------------------
                                'データレコードの書込み
                                '--------------------------------
                                WriteA.Write(WrkFmt.JF_DATA2.Data)

                                '--------------------------------------------------------------------------
                                'センターカットデータを分割できるように別ファイルにも同時書き込み
                                '--------------------------------------------------------------------------
                                intRECORD_CNT_B += 1
                                '--------------------------------
                                'データレコードの書込み
                                '--------------------------------
                                WriteB.Write(WrkFmt.JF_DATA2.Data)

                                '2010/09/14.Sakon　分割件数をＩＮＩファイルから取得する +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                If intRECORD_CNT_B = CInt(IniInfo.KITEIKEN) - 1 Then
                                    '------------------------------------
                                    'エンドレコードの書込み
                                    '------------------------------------
                                    intRECORD_CNT_B += 1

                                    WriteB.Write(WrkFmt.JF_DATA3.Data)
                                    WriteB.Close()

                                    lngCYCLE = lngCYCLE + 1 'サイクル番号カウントアップ
                                    strWRK_FILE_B = Path.Combine(IniInfo.DATBK, "WRK00" & lngCYCLE & "_B.DAT")
                                    If Dir(strWRK_FILE_B) <> "" Then
                                        Kill(strWRK_FILE_B)
                                    End If

                                    WriteB = New StreamWriter(strWRK_FILE_B, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

                                    intRECORD_CNT_B = 1
                                    '--------------------------------
                                    'ヘッダーレコードの書込み
                                    '---------------------------------
                                    WriteB.Write(WrkFmt.JF_DATA1.Data)
                                End If
                                '149999件(ヘッダ含む)でファイル分割を行う
                                '*** 修正 nishida 2008/07/28 共同加盟後はＭＡＸ150000件で分割/先行稼動時はファイル分割なし ***
                                'If intRECORD_CNT_B = 149999 Then
                                '*** 修正 mitsu 2008/09/17 処理高速化 ***
                                'If Mode <> "530" And intRECORD_CNT_B = 149999 Then
                                'If intRECORD_CNT_B = 39999 Then
                                '    '************************************
                                '    '------------------------------------
                                '    'エンドレコードの書込み
                                '    '------------------------------------
                                '    intRECORD_CNT_B += 1

                                '    WriteB.Write(WrkFmt.JF_DATA3.Data)
                                '    WriteB.Close()

                                '    lngCYCLE = lngCYCLE + 1 'サイクル番号カウントアップ
                                '    strWRK_FILE_B = gstrDATBK_OPENDIR & "WRK00" & lngCYCLE & "_B.DAT"
                                '    If Dir(strWRK_FILE_B) <> "" Then
                                '        Kill(strWRK_FILE_B)
                                '    End If

                                '    WriteB = New StreamWriter(strWRK_FILE_B, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

                                '    intRECORD_CNT_B = 1
                                '    '--------------------------------
                                '    'ヘッダーレコードの書込み
                                '    '---------------------------------
                                '    WriteB.Write(WrkFmt.JF_DATA1.Data)
                                'End If
                                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                                intRECORD_CNT += 1
                                dblSOUSIN_KEN += 1
                                dblSOUSIN_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                                dblALL_KEN += 1
                                dblALL_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                                '*** 修正 mitsu 2008/09/17 strJIFURI_SEQ → intJIFURI_SEQ ***
                                intJIFURI_SEQ += 1
                                '************************************************************
                                '*** 修正 mitsu 2008/09/17 振替日の企業シーケンス設定 ***
                                htJIFURI_SEQ.Item(Key.FURI_DATE) = intJIFURI_SEQ
                                '********************************************************
                            End If

                            oraMeiReader.NextRead()
                        End While
                    End If

                    oraMeiReader.Close()

                    '************************************
                    ' 処理結果確認表
                    '************************************
                    Dim SK(14) As String

                    SK(0) = strDate                         '処理日
                    SK(1) = strTime                         'タイムスタンプ
                    SK(2) = Key.FURI_DATE                   '振替日
                    SK(3) = Key.TORIS_CODE                  '取引先種コード
                    SK(4) = Key.TORIF_CODE                  '取引先副コード
                    SK(5) = Key.ITAKU_NNAME                 '委託者名
                    SK(6) = Key.ITAKU_CODE                  '委託者コード
                    SK(7) = Key.NS_KBN                      '入出金区分
                    SK(8) = Key.FURI_CODE                   '振替コード
                    SK(9) = Key.KIGYO_CODE                  '企業コード
                    SK(10) = Key.SYORI_KEN                  '依頼件数
                    SK(11) = Key.SYORI_KIN                  '依頼金額
                    SK(12) = dblSOUSIN_KEN.ToString         '処理件数
                    SK(13) = dblSOUSIN_KIN.ToString         '処理金額
                    SK(14) = ""                             '備考
                    '2010/12/24 信組対応 信組の場合は送信区分を追加する
                    If IniInfo.CENTER = "0" Then
                        ' 2015/12/14 タスク）綾部 CHG 【PG】UI_B-14-04(RSV2対応) -------------------- START
                        'ReDim Preserve SK(15)
                        'SK(15) = "組合持込"                 '送信区分
                        ReDim Preserve SK(18)
                        SK(15) = "組合持込"                 '送信区分
                        If IniInfo.KUMIAI_SOUFU = "1" Then
                            SK(16) = IniInfo.KUMIAI_SOUFU1
                            SK(17) = IniInfo.KUMIAI_SOUFU2
                            SK(18) = IniInfo.KUMIAI_SOUFU3
                        Else
                            SK(16) = ""
                            SK(17) = ""
                            SK(18) = ""
                        End If
                        ' 2015/12/14 タスク）綾部 CHG 【PG】UI_B-14-04(RSV2対応) -------------------- END
                    End If

                    arySyoriKekka.Add(SK)

                    dblSOUSIN_KEN = 0
                    dblSOUSIN_KIN = 0
                    '-----------------------------------------
                    'スケジュールの更新
                    '-----------------------------------------
                    If fn_SCHMAST_UPDATE(Key, intSCH_CNT) = False Then
                        MainLOG.UpdateJOBMASTbyErr("スケジュール更新失敗")
                        Return False
                    End If

                    oraSchReader.NextRead()
                End While
            Else
                '対象スケジュールが存在しない場合
                MainLOG.Write("スケジュール検索", "失敗", "対象スケジュールが存在しない")
                MainLOG.UpdateJOBMASTbyErr("対象スケジュールなし")
                Return False
            End If

            '------------------------------------
            'エンドレコードの書込み
            '------------------------------------
            intRECORD_CNT += 1
            WriteA.Write(WrkFmt.JF_DATA3.Data)
            WriteA.Close()

            '--------------------------------------------------------------------------
            'センターカットデータを分割できるように別ファイルにも同時書き込み
            '--------------------------------------------------------------------------
            '------------------------------------
            'エンドレコードの書込み
            '------------------------------------
            intRECORD_CNT_B += 1
            WriteB.Write(WrkFmt.JF_DATA3.Data)
            WriteB.Close()

            '蒲郡信金向け　分割ファイル分コード変換を行う 2007/08/17
            Dim lngCYCLE_CNT As Long = 0
            Dim lngFILE_CNT As Long = 0

            For lngCYCLE_CNT = CLng(CYCLE) To lngCYCLE            '初期値サイクルから分割したサイクル番号までループ
                lngFILE_CNT += 1

                'strWRK_FILE置き換え
                strWRK_FILE = Path.Combine(IniInfo.DATBK, "WRK00" & lngCYCLE_CNT & "_B.DAT")

                '-------------------------------------
                '送信ファイルの作成　センター毎に分岐
                '-------------------------------------
                strSEND_FILE = Path.Combine(IniInfo.DAT, "SENDWRK" & lngCYCLE_CNT & ".DAT")
                If Dir(strSEND_FILE) <> "" Then
                    Kill(strSEND_FILE)
                End If

                Select Case IniInfo.CENTER
                    Case "0"  'ＳＫＣ
                        Call fn_CREATE_FILE_SKC()
                    Case "1"  '北海道センター

                    Case "2", "3", "4", "5", "6"  '東北センター,東海センター,大阪センター,中国センター,東京センター(2010/09/10追加)
                        Call fn_CREATE_FILE_SINKIN136()
                    Case "7"  '九州センター

                End Select

                '--------------------------------------------
                '送信ファイルをEBCDICにコード変換（FTRAN+使用）
                '--------------------------------------------
                '大阪センターは午前、午後でファイル名を変える。
                ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
                'Select Case IniInfo.CENTER
                '    Case "5"
                '        If strTime < IniInfo.BORDER_TIME Then    '2010.02.19 境界時間をiniより取得
                '            strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEAM & lngCYCLE_CNT & ".DAT")
                '        Else
                '            strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEPM & lngCYCLE_CNT & ".DAT")
                '        End If

                '    Case Else
                '        strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEAM & lngCYCLE_CNT & ".DAT")
                'End Select
                Select Case IniInfo.CENTER
                    Case "5"
                        '-----------------------------------------------
                        ' 大阪センター
                        '-----------------------------------------------
                        Select Case IniInfo.JIFURI_CCFNAME
                            Case "1"
                                '---------------------------------------
                                ' ファイル名固定
                                '---------------------------------------
                                If strTime < IniInfo.BORDER_TIME Then
                                    strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEAM)
                                Else
                                    strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEPM)
                                End If
                            Case Else
                                '---------------------------------------
                                ' ファイル名CYCLE付与
                                '---------------------------------------
                                If strTime < IniInfo.BORDER_TIME Then
                                    strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEAM & lngCYCLE_CNT & ".DAT")
                                Else
                                    strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEPM & lngCYCLE_CNT & ".DAT")
                                End If
                        End Select

                    Case Else
                        '-----------------------------------------------
                        ' 大阪センター以外
                        '-----------------------------------------------
                        Select Case IniInfo.JIFURI_CCFNAME
                            Case "1"
                                '---------------------------------------
                                ' ファイル名固定
                                '---------------------------------------
                                strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEAM)
                            Case Else
                                '---------------------------------------
                                ' ファイル名CYCLE付与
                                '---------------------------------------
                                strJIFURI_FILE = Path.Combine(IniInfo.DEN, IniInfo.FILEAM & lngCYCLE_CNT & ".DAT")
                        End Select
                End Select
                ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

                If File.Exists(strJIFURI_FILE) = True Then File.Delete(strJIFURI_FILE)

                If dblALL_KEN > 0 Then  '明細が０件以上だったら
                    If fn_CODE_CHANGE() = False Then
                        MainLOG.UpdateJOBMASTbyErr("コード変換失敗")
                        Return False
                    End If

                    If File.Exists(strJIFURI_FILE) = False Then
                        MainLOG.UpdateJOBMASTbyErr("配信ファイル作成失敗")
                        Return False
                    End If
                Else
                    MainLOG.Write("配信ファイル作成", "成功", "明細０件")
                End If
            Next

            '-------------------------------------
            '処理結果確認表印刷
            '-------------------------------------
            If PrnSyoriKekka(arySyoriKekka) = False Then
                Return False
            End If

            '-------------------------------------
            '地区伝送データ送信連絡票
            '-------------------------------------
            '2010/09/10.Sakon　If文追加：送信連絡票を印刷するか否かをＩＮＩファイルで設定
            If IniInfo.SOUSINRENRAKU = "1" Then
                If PrnChikuSoufu() = False Then
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("配信ファイル作成(金庫組合フォーマット)", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("配信ファイル作成(金庫組合フォーマット)失敗 例外発生")
            Return False
        Finally
            If Not oraSchReader Is Nothing Then
                oraSchReader.Close()
                oraSchReader = Nothing
            End If
        End Try
        If Not oraMeiReader Is Nothing Then
            oraMeiReader.Close()
            oraMeiReader = Nothing
        End If
    End Function

    Private Function fn_MAIN_01() As Boolean

        Dim oraSchReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Dim arySyoriKekka As New ArrayList
        Dim intSCH_CNT As Integer = 0

        Dim strMAE_ITAKU_KANRI_CODE As String = String.Empty

        Dim swZenFile As StreamWriter = Nothing
        Dim fmtZen As New CAstFormat.CFormatZengin

        Try
            '-----------------------------------------
            '対象スケジュール検索
            '-----------------------------------------
            Dim Key As strcSchmastInfo = Nothing

            If oraSchReader.DataReader(CreateGetSchmastSQL("01")) = True Then

                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraSchReader.EOF = False
                    intSCH_CNT += 1

                    ' キー初期化
                    Key.Init()
                    ' 最初のキー設定
                    Call Key.SetOraDataHaisin(oraSchReader)

                    MainLOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                    MainLOG.FuriDate = Key.FURI_DATE
                    MainLOG.Write("スケジュール検索", "成功", intSCH_CNT.ToString & "件目")

                    If Key.FILE_SEQ = "1" Then
                        If intSCH_CNT <> 1 And Key.ITAKU_KANRI_CODE <> strMAE_ITAKU_KANRI_CODE Then
                            If Not swZenFile Is Nothing Then swZenFile.Close()

                            '--------------------------------------------
                            '送信ファイルをEBCDICにコード変換（FTRAN+使用）
                            '--------------------------------------------
                            If fn_CODE_CHANGE() = False Then
                                MainLOG.UpdateJOBMASTbyErr("コード変換失敗")
                                Return False
                            End If
                        End If
                        '---------------------------------------------------
                        '中間ファイルのオープン（D:\RSKJ\DATBK\S000000000.WRK）
                        '---------------------------------------------------
                        If Key.MULTI_KBN = "1" Then
                            strWRK_FILE = Path.Combine(IniInfo.DATBK, "S" & Key.ITAKU_KANRI_CODE & ".WRK")
                            strJIFURI_FILE = Path.Combine(IniInfo.DEN, "S" & Key.ITAKU_KANRI_CODE & ".DAT")
                        Else
                            strWRK_FILE = Path.Combine(IniInfo.DATBK, "S" & Key.TORIS_CODE & Key.TORIF_CODE & ".WRK")
                            strJIFURI_FILE = Path.Combine(IniInfo.DEN, "S" & Key.TORIS_CODE & Key.TORIF_CODE & ".DAT")
                        End If
                        If Dir(strWRK_FILE) <> "" Then
                            Kill(strWRK_FILE)
                        End If

                        swZenFile = New StreamWriter(strWRK_FILE, False, Encoding.GetEncoding("SHIFT-JIS"))
                    End If

                    '---------------------------------------------------
                    '明細マスタの検索（MEIMAST）
                    '---------------------------------------------------
                    If oraMeiReader.DataReader(CreateGetMeimastSQL(Key, "01")) = True Then
                        While oraMeiReader.EOF = False
                            Select Case oraMeiReader.GetString("DATA_KBN_K")
                                Case "1"
                                    dblSOUSIN_KEN = 0
                                    dblSOUSIN_KIN = 0
                                    intRECORD_CNT += 1
                                    fmtZen.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                    swZenFile.Write(fmtZen.ZENGIN_REC1.Data)

                                Case "2"
                                    If oraMeiReader.GetString("KEIYAKU_KIN_K") <> IniInfo.KINKOCD OrElse _
                                        oraMeiReader.GetInt64("FURIKIN_K") = 0 OrElse _
                                        oraMeiReader.GetInt("FURIKETU_CODE_K") <> 0 Then
                                        'センターに送信しない。件数、金額を計算する。
                                        dblFUNOU_KEN += 1
                                        dblFUNOU_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                                    Else
                                        intRECORD_CNT += 1
                                        fmtZen.ZENGIN_REC2.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                        fmtZen.ZENGIN_REC2.ZG2 = oraMeiReader.GetString("KEIYAKU_KIN_K")
                                        fmtZen.ZENGIN_REC2.ZG4 = oraMeiReader.GetString("KEIYAKU_SIT_K")
                                        fmtZen.ZENGIN_REC2.ZG8 = oraMeiReader.GetString("KEIYAKU_KOUZA_K")
                                        swZenFile.Write(fmtZen.ZENGIN_REC2.Data)
                                        dblSOUSIN_KEN += 1
                                        dblSOUSIN_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                                        dblALL_KEN += 1
                                        dblALL_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                                    End If

                                Case "8"
                                    intRECORD_CNT += 1
                                    fmtZen.ZENGIN_REC8.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                    fmtZen.ZENGIN_REC8.ZG2 = dblSOUSIN_KEN.ToString.PadLeft(6, "0"c)
                                    fmtZen.ZENGIN_REC8.ZG3 = dblSOUSIN_KIN.ToString.PadLeft(12, "0"c)
                                    swZenFile.Write(fmtZen.ZENGIN_REC8.Data)

                                    '************************************
                                    ' 処理結果確認表
                                    '************************************
                                    ' 2015/12/14 タスク）綾部 CHG 【PG】UI_B-14-04(RSV2対応) -------------------- START
                                    'Dim SK(15) As String
                                    Dim SK(18) As String
                                    ' 2015/12/14 タスク）綾部 CHG 【PG】UI_B-14-04(RSV2対応) -------------------- END

                                    SK(0) = strDate                         '処理日
                                    SK(1) = strTime                         'タイムスタンプ
                                    SK(2) = Key.FURI_DATE                   '振替日
                                    SK(3) = Key.TORIS_CODE                  '取引先種コード
                                    SK(4) = Key.TORIF_CODE                  '取引先副コード
                                    SK(5) = Key.ITAKU_NNAME                 '委託者名
                                    SK(6) = Key.ITAKU_CODE                  '委託者コード
                                    SK(7) = Key.NS_KBN                      '入出金区分
                                    SK(8) = Key.FURI_CODE                   '振替コード
                                    SK(9) = Key.KIGYO_CODE                  '企業コード
                                    SK(10) = Key.SYORI_KEN                  '依頼件数
                                    SK(11) = Key.SYORI_KIN                  '依頼金額
                                    SK(12) = dblSOUSIN_KEN.ToString         '処理件数
                                    SK(13) = dblSOUSIN_KIN.ToString         '処理金額
                                    SK(14) = ""                             '備考
                                    SK(15) = "企業持込"                     '送信区分
                                    ' 2015/12/14 タスク）綾部 ADD 【PG】UI_B-14-04(RSV2対応) -------------------- START
                                    If IniInfo.KIGYO_SOUFU = "1" Then
                                        SK(16) = IniInfo.KIGYO_SOUFU1
                                        SK(17) = IniInfo.KIGYO_SOUFU2
                                        SK(18) = IniInfo.KIGYO_SOUFU3
                                    Else
                                        SK(16) = ""
                                        SK(17) = ""
                                        SK(18) = ""
                                    End If
                                    ' 2015/12/14 タスク）綾部 ADD 【PG】UI_B-14-04(RSV2対応) -------------------- END

                                    arySyoriKekka.Add(SK)

                                    '-------------------------------------
                                    'スケジュール更新
                                    '-------------------------------------
                                    If fn_SCHMAST_UPDATE(Key, intSCH_CNT) = False Then
                                        MainLOG.UpdateJOBMASTbyErr("スケジュール更新失敗")
                                        Return False
                                    End If

                                Case "9"
                                    intRECORD_CNT += 1
                                    fmtZen.ZENGIN_REC9.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                    swZenFile.Write(fmtZen.ZENGIN_REC9.Data)
                            End Select

                            oraMeiReader.NextRead()
                        End While
                    End If

                    oraMeiReader.Close()

                    strMAE_ITAKU_KANRI_CODE = Key.ITAKU_KANRI_CODE

                    oraSchReader.NextRead()
                End While
            Else
                MainLOG.Write("スケジュール検索", "失敗", "対象スケジュールが存在しない")
                MainLOG.UpdateJOBMASTbyErr("対象スケジュールなし")
                Return False
            End If

            If Not swZenFile Is Nothing Then swZenFile.Close()
            If Not oraSchReader Is Nothing Then oraSchReader.Close()
            If Not oraMeiReader Is Nothing Then oraMeiReader.Close()

            '-------------------------------------
            '処理結果確認表印刷
            '-------------------------------------
            If PrnSyoriKekka(arySyoriKekka) = False Then
                Return False
            End If

            '--------------------------------------------
            '送信ファイルをEBCDICにコード変換（FTRAN+使用）
            '--------------------------------------------
            If fn_CODE_CHANGE() = False Then
                MainLOG.UpdateJOBMASTbyErr("コード変換失敗")
                Return False
            End If

            Return True

        Catch ex As Exception

            '対象スケジュールが存在しない場合
            MainLOG.Write("配信ファイル作成(全銀フォーマット)", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("配信ファイル作成(全銀フォーマット)失敗 例外発生")
            Return False
        Finally
            If Not oraSchReader Is Nothing Then
                oraSchReader.Close()
                oraSchReader = Nothing
            End If
            If Not oraMeiReader Is Nothing Then
                oraMeiReader.Close()
                oraMeiReader = Nothing
            End If
        End Try
    End Function

    Private Function fn_CREATE_FILE_SKC() As Boolean
        '============================================================================
        'NAME           :fn_CREATE_FILE_SKC
        'Parameter      :
        'Description    :センターカットデータファイル作成（ＳＫＣ）
        '               :200バイトのワークファイルから送信用の160バイトのファイルを作成する
        'Return         :
        'Create         :2004/08/18
        'Update         :
        '============================================================================

        Dim strDATA_KBN As String
        Dim intSND_RECORD_COUNT As Integer
        Dim dblGOUKEI_KEN As Double, dblGOUKEI_KIN As Double
        Dim strMAE_TORI_CODE As String = ""

        Try

            Dim intWRK_RECORD_COUNT As Integer = 0
            intSND_RECORD_COUNT = 0
            dblGOUKEI_KEN = 0
            dblGOUKEI_KIN = 0

            Dim FmtWork As New CAstFormat.CFormatShinkinWork200
            Dim FmtSKC As New CAstFormat.CFormatSKC160

            Call FmtWork.FirstRead(strWRK_FILE)

            Dim SendFile As New StreamWriter(strSEND_FILE, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
            '*** Str Add 2015/12/26 sys)mori for 信組手数料決済表示対応 ***
            Dim strSHINKUMI_TESUURYOU As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SHINKUMI_TESUURYOU")
            '*** end Add 2015/12/26 sys)mori for 信組手数料決済表示対応 ***

            Do Until FmtWork.EOF
                Call FmtWork.GetFileData()
                Select Case FmtWork.RecordData.Substring(0, 12)
                    Case "000000000000"  'ヘッダー部
                        strDATA_KBN = "1"
                        FmtWork.JF_DATA1.Data = FmtWork.RecordData
                        With FmtSKC.SKC160_DATA1
                            .JF1 = "1"
                            .JF2 = "0000"
                            .JF3 = "000"
                            .JF4 = "0"
                            .JF5 = IniInfo.KINKOCD
                            .JF6 = "9"
                            .JF7 = "0"
                            '*** Str Add 2015/12/26 sys)mori for 信組手数料決済表示対応 ***
                            If strSHINKUMI_TESUURYOU = "1" Then
                                .JF8 = "1" & Space(144)
                            Else
                                .JF8 = Space(145)
                            End If
                            '.JF8 = Space(145)
                            '*** end Add 2015/12/26 sys)mori for 信組手数料決済表示対応 ***
                        End With
                        strMAE_TORI_CODE = FmtWork.JF_DATA1.TC
                        '--------------------------
                        'ヘッダーレコードの書込み
                        '--------------------------
                        SendFile.Write(FmtSKC.SKC160_DATA1.Data)
                        intSND_RECORD_COUNT += 1
                    Case "999999999999"  'エンド部
                        strDATA_KBN = "9"
                        FmtWork.JF_DATA3.Data = FmtWork.RecordData
                        With FmtSKC.SKC160_DATA8
                            .JF1 = "8"
                            .JF2 = "0000"
                            .JF3 = "000"
                            .JF4 = "8"
                            .JF5 = dblGOUKEI_KEN.ToString("000000")
                            .JF6 = dblGOUKEI_KIN.ToString("000000000000")
                            .JF7 = Space(1)
                            .JF8 = "000000"
                            .JF9 = Space(1)
                            .JF10 = "000000000000"
                            .JF11 = "000000"
                            .JF12 = "000000000000"
                            .JF13 = Space(95)
                        End With
                        '--------------------------
                        'トレーラレコードの書込み
                        '--------------------------
                        SendFile.Write(FmtSKC.SKC160_DATA8.Data)
                        intSND_RECORD_COUNT += 1
                        With FmtSKC.SKC160_DATA9
                            .JF1 = "9"
                            .JF2 = "0000"
                            .JF3 = "000"
                            .JF4 = "9"
                            .JF5 = Space(151)
                        End With
                        '--------------------------
                        'エンドレコードの書込み
                        '--------------------------
                        SendFile.Write(FmtSKC.SKC160_DATA9.Data)
                        intSND_RECORD_COUNT += 1
                        dblGOUKEI_KEN = 0
                        dblGOUKEI_KIN = 0
                    Case Else         'データ部
                        strDATA_KBN = "2"
                        FmtWork.JF_DATA2.Data = FmtWork.RecordData
                        '----------------------------------------------------------------
                        '取引先コードが変わったらトレーラ、エンド、ヘッダーレコードを書き込む
                        '----------------------------------------------------------------
                        If strMAE_TORI_CODE <> FmtWork.JF_DATA2.TC And strMAE_TORI_CODE <> "000000000000" Then
                            With FmtSKC.SKC160_DATA8
                                .JF1 = "8"
                                .JF2 = "0000"
                                .JF3 = "000"
                                .JF4 = "8"
                                .JF5 = dblGOUKEI_KEN.ToString("000000")
                                .JF6 = dblGOUKEI_KIN.ToString("000000000000")
                                .JF7 = Space(1)
                                .JF8 = "000000"
                                .JF9 = Space(1)
                                .JF10 = "000000000000"
                                .JF11 = "000000"
                                .JF12 = "000000000000"
                                .JF13 = Space(95)
                            End With
                            '--------------------------
                            'トレーラレコードの書込み
                            '--------------------------
                            SendFile.Write(FmtSKC.SKC160_DATA8.Data)
                            intSND_RECORD_COUNT += 1
                            With FmtSKC.SKC160_DATA9
                                .JF1 = "9"
                                .JF2 = "0000"
                                .JF3 = "000"
                                .JF4 = "9"
                                .JF5 = Space(151)
                            End With
                            '--------------------------
                            'エンドレコードの書込み
                            '--------------------------
                            SendFile.Write(FmtSKC.SKC160_DATA9.Data)
                            intSND_RECORD_COUNT += 1
                            dblGOUKEI_KEN = 0
                            dblGOUKEI_KIN = 0
                            With FmtSKC.SKC160_DATA1
                                .JF1 = "1"
                                .JF2 = "0000"
                                .JF3 = "000"
                                .JF4 = "0"
                                .JF5 = IniInfo.KINKOCD
                                .JF6 = "9"
                                .JF7 = "0"
                                .JF8 = Space(145)
                            End With
                            strMAE_TORI_CODE = FmtWork.JF_DATA1.TC
                            '--------------------------
                            'ヘッダーレコードの書込み
                            '--------------------------
                            SendFile.Write(FmtSKC.SKC160_DATA1.Data)
                            intSND_RECORD_COUNT += 1
                        End If
                        With FmtSKC.SKC160_DATA2
                            .JF1 = "2"
                            .JF2 = FmtWork.JF_DATA2.JF1
                            .JF3 = FmtWork.JF_DATA2.JF2
                            .JF4 = "2"
                            .JF5 = FmtWork.JF_DATA2.JF4
                            .JF6 = FmtWork.JF_DATA2.JF5
                            '西暦変換
                            ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
                            'Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
                            'culture.DateTimeFormat.Calendar = New JapaneseCalendar
                            'Dim target As String = FmtWork.JF_DATA2.JF6
                            '何故かエラーになるので年と月の間に空白を入れる
                            'Dim result As DateTime = DateTime.ParseExact(target.Insert(2, " "), "yy MMdd", culture)

                            Dim target As String = FmtWork.JF_DATA2.JF6
                            Dim result As DateTime = DateTime.ParseExact(ConvertYear(target.Substring(0, 2)) & target.Substring(2, 4), "yyyyMMdd", Nothing)
                            ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END
                            .JF7 = Format(result, "yyyyMMdd")
                            .JF8 = CLng(FmtWork.JF_DATA2.JF7).ToString("0000000000")
                            .JF9 = FmtWork.JF_DATA2.JF8
                            .JF10 = FmtWork.JF_DATA2.JF9.Substring(0, 4) '企業コードは4桁
                            .JF11 = FmtWork.JF_DATA2.JF10.Substring(0, 7) '企業シーケンスは7桁
                            .JF12 = "00"
                            .JF13 = FmtWork.JF_DATA2.JF12
                            .JF14 = "00"
                            .JF15 = "0000000"
                            .JF16 = "000000"
                            .JF17 = "000000"
                            .JF18 = FmtWork.JF_DATA2.JF17
                            .JF19 = FmtWork.JF_DATA2.JF18
                            .JF20 = FmtWork.JF_DATA2.JF19
                            .JF21 = FmtWork.JF_DATA2.JF20
                            .JF22 = FmtWork.JF_DATA2.JF21
                            .JF23 = "0"
                            .JF24 = "00000000000" '2011/01/27 10→11に修正
                            .JF25 = Space(10)
                            .JF26 = FmtWork.JF_DATA2.JF22
                            .JF27 = Space(5) '2011/01/27 5→4に修正
                            .JF28 = "00"
                        End With
                        strMAE_TORI_CODE = FmtWork.JF_DATA2.TC
                        '--------------------------
                        'データレコードの書込み
                        '--------------------------
                        SendFile.Write(FmtSKC.SKC160_DATA2.Data)
                        intSND_RECORD_COUNT += 1
                        dblGOUKEI_KEN += 1
                        dblGOUKEI_KIN += Val(FmtWork.JF_DATA2.JF7)
                End Select
                intWRK_RECORD_COUNT += 1
            Loop
            FmtWork.Close()
            FmtSKC.Close()
            SendFile.Close()

            MainLOG.Write("送信ファイルSKC（JIS）作成", "成功", Err.Description)
            intRECORD_CNT = intSND_RECORD_COUNT

        Catch ex As Exception
        Finally
        End Try

        Return True

    End Function

    Private Function fn_CREATE_FILE_SINKIN136() As Boolean
        '============================================================================
        'NAME           :fn_CREATE_FILE_SINKIN136
        'Parameter      :
        'Description    :センターカットデータファイル作成（東海センター）
        '               :200バイトのワークファイルから送信用の136バイトのファイルを作成する
        'Return         :
        'Create         :2004/08/18
        'Update         :
        '============================================================================

        Dim strDATA_KBN As String
        Dim intSND_RECORD_COUNT As Integer
        Dim dblGOUKEI_KEN As Double, dblGOUKEI_KIN As Double

        Try

            Dim intWRK_RECORD_COUNT As Integer = 1
            intSND_RECORD_COUNT = 1
            dblGOUKEI_KEN = 0
            dblGOUKEI_KIN = 0
            Dim FmtWork As New CAstFormat.CFormatShinkinWork200
            Dim FmtSink As New CAstFormat.CFormatShinkin136
            Call FmtWork.FirstRead(strWRK_FILE)

            Dim SendFile As New StreamWriter(strSEND_FILE, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
            Do Until FmtWork.EOF
                Call FmtWork.GetFileData()
                Select Case FmtWork.RecordData.Substring(0, 12)
                    Case "000000000000"  'ヘッダー部
                        strDATA_KBN = "1"
                        FmtWork.JF_DATA1.Data = FmtWork.RecordData
                        With FmtSink.SINKIN136_DATA1
                            .JF1 = FmtWork.JF_DATA1.JF1
                            .JF2 = FmtWork.JF_DATA1.JF2
                            .JF3 = FmtWork.JF_DATA1.JF3
                            .JF4 = FmtWork.JF_DATA1.JF4
                            .JF5 = FmtWork.JF_DATA1.JF5
                            .JF6 = FmtWork.JF_DATA1.JF6
                        End With
                        '--------------------------
                        'ヘッダーレコードの書込み
                        '--------------------------
                        SendFile.Write(FmtSink.SINKIN136_DATA1.Data)
                        intSND_RECORD_COUNT += 1
                    Case "999999999999"  'エンド部
                        strDATA_KBN = "9"
                        FmtWork.JF_DATA3.Data = FmtWork.RecordData
                        With FmtSink.SINKIN136_DATA9
                            .JF1 = FmtWork.JF_DATA3.JF1
                            .JF2 = FmtWork.JF_DATA3.JF2
                            .JF3 = FmtWork.JF_DATA3.JF3
                            .JF4 = ""
                            .JF4 = .JF4.PadLeft(128, "0")
                        End With
                        '--------------------------
                        'エンドレコードの書込み
                        '--------------------------
                        SendFile.Write(FmtSink.SINKIN136_DATA9.Data)
                        intSND_RECORD_COUNT += 1
                    Case Else         'データ部
                        strDATA_KBN = "2"
                        FmtWork.JF_DATA2.Data = FmtWork.RecordData
                        With FmtSink.SINKIN136_DATA2
                            .JF1 = FmtWork.JF_DATA2.JF1
                            .JF2 = FmtWork.JF_DATA2.JF2
                            .JF3 = FmtWork.JF_DATA2.JF3
                            .JF4 = FmtWork.JF_DATA2.JF4
                            .JF5 = FmtWork.JF_DATA2.JF5
                            .JF6 = FmtWork.JF_DATA2.JF6
                            .JF7 = FmtWork.JF_DATA2.JF7
                            .JF8 = FmtWork.JF_DATA2.JF8
                            .JF9 = Microsoft.VisualBasic.Left(FmtWork.JF_DATA2.JF9.Trim, 5)
                            .JF10 = FmtWork.JF_DATA2.JF10
                            .JF11 = FmtWork.JF_DATA2.JF11
                            .JF12 = FmtWork.JF_DATA2.JF12
                            .JF13 = FmtWork.JF_DATA2.JF13
                            .JF14 = FmtWork.JF_DATA2.JF14
                            .JF15 = FmtWork.JF_DATA2.JF15
                            .JF16 = FmtWork.JF_DATA2.JF16
                            .JF17 = FmtWork.JF_DATA2.JF17
                            .JF18 = FmtWork.JF_DATA2.JF18
                            .JF19 = FmtWork.JF_DATA2.JF19
                            .JF20 = FmtWork.JF_DATA2.JF20
                            .JF21 = FmtWork.JF_DATA2.JF21
                            .JF22 = "0000000"

                        End With
                        '--------------------------
                        'データレコードの書込み
                        '--------------------------
                        SendFile.Write(FmtSink.SINKIN136_DATA2.Data)
                        intSND_RECORD_COUNT += 1
                        dblGOUKEI_KEN += 1
                        dblGOUKEI_KIN += Val(FmtWork.JF_DATA2.JF7)

                End Select
                intWRK_RECORD_COUNT += 1
            Loop
            FmtWork.Close()
            FmtSink.Close()
            SendFile.Close()

            MainLOG.Write("送信ファイル（JIS）作成", "成功", Err.Description)

        Catch ex As Exception
        Finally
        End Try

        Return True

    End Function

    Private Function fn_CODE_CHANGE() As Boolean
        '============================================================================
        'NAME           :fn_CODE_CHENGE
        'Parameter      :
        'Description    :送信ファイル（JIS）をEBCDICコードに変換する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/08/18
        'Update         :
        '===========================================================================

        Try

            fn_CODE_CHANGE = False

            Dim strP_FILE As String = ""        '2011/06/17 警告削除
            Dim intBYTE As Integer
            Select Case SOUSIN_KBN
                Case "0"    '金庫、組合持込フォーマット
                    Select Case IniInfo.CENTER
                        Case "0"  'ＳＫＣ
                            intBYTE = 160
                            strP_FILE = "HAISIN_SKC.P"
                        Case "1"  '北海道センター

                        Case "2", "3", "4", "5", "6" '東北センター,東海センター,大阪センター,中国センター,東京センター（2010/09/14追加）
                            intBYTE = 136
                            strP_FILE = "HAISIN_SINKIN136.P"
                        Case "7"  '九州センター

                    End Select
                Case "1"  '全銀フォーマット
                    intBYTE = 120
                    strP_FILE = "120.P"
                    strSEND_FILE = strWRK_FILE
                Case "2"  '地公体フォーマット
                    intBYTE = 220
                    strP_FILE = "220.P"
                    strSEND_FILE = strWRK_FILE
                Case Else
            End Select

            Dim intKEKKA As Integer
            'Dim strCODE_KBN As String = "1"
            Dim strCODE_KBN As String = "4" 'EBCDIC(1→4)
            '2013/03/22 saitou 蒲郡信金 ソース改善 UPD -------------------------------------------------->>>>
            intKEKKA = ConvertFileFtranP("PUTRAND", strSEND_FILE, strJIFURI_FILE, Path.Combine(IniInfo.FTR, strP_FILE))
            'intKEKKA = clsFUSION.fn_DISK_CPYTO_DEN(strTORI_CODE, strSEND_FILE, strJIFURI_FILE, _
            '                                       intBYTE, strCODE_KBN, strP_FILE)
            '2013/03/22 saitou 蒲郡信金 ソース改善 UPD --------------------------------------------------<<<<
            Select Case intKEKKA
                Case 0
                    fn_CODE_CHANGE = True
                Case 100
                    fn_CODE_CHANGE = False
                    Exit Function
            End Select

            fn_CODE_CHANGE = True

        Catch ex As Exception
            MainLOG.Write("コード変換", "失敗", ex.Message)
        End Try

    End Function

    ''' <summary>
    ''' FTRANPを用いてコード変換を行います。
    ''' </summary>
    ''' <param name="strGetOrPut">GETRAND,GETDATA,PUTRAND,PUTDATA</param>
    ''' <param name="strInFileName">入力ファイルパス</param>
    ''' <param name="strOutFileName">出力ファイルパス</param>
    ''' <param name="strPFileName">FTRANPパラメータファイル名</param>
    ''' <returns>0:正常 100:異常</returns>
    ''' <remarks></remarks>
    Private Function ConvertFileFtranP(ByVal strGetOrPut As String, _
                                       ByVal strInFileName As String, _
                                       ByVal strOutFileName As String, _
                                       ByVal strPFileName As String) As Integer
        Try
            '変換コマンド組み立て
            Dim Command As New StringBuilder
            With Command
                .Append(" /nwd/ cload ")
                .Append("""" & IniInfo.FTR & "FUSION" & """")
                .Append(" ; kanji 83_jis")
                .Append(" " & strGetOrPut & " ")
                .Append("""" & strInFileName & """" & " ")
                .Append("""" & strOutFileName & """" & " ")
                .Append(" ++" & """" & strPFileName & """")
            End With

            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(IniInfo.FTRANP, "FP.EXE")
            ProcInfo.WorkingDirectory = IniInfo.FTRANP
            ProcInfo.Arguments = Command.ToString
            Proc = Process.Start(ProcInfo)
            Proc.WaitForExit()
            If Proc.ExitCode = 0 Then
                MainLOG.Write("(FTRANPコード変換)", "成功", "終了コード：" & Proc.ExitCode)
                Return 0
            Else
                MainLOG.Write("(FTRANPコード変換)", "失敗", "終了コード：" & Proc.ExitCode)
                Return 100
            End If
        Catch ex As Exception
            MainLOG.Write("(FTRANPコード変換)", "失敗", ex.Message)
            Return 100
        End Try
    End Function

    ''' <summary>
    ''' スケジュールを更新します。
    ''' </summary>
    ''' <param name="Key">スケジュール情報</param>
    ''' <param name="intSCH_CNT">更新対象スケジュール</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_SCHMAST_UPDATE(ByVal Key As strcSchmastInfo, ByVal intSCH_CNT As Integer) As Boolean
        fn_SCHMAST_UPDATE = False

        Dim SQL As New StringBuilder
        With SQL
            .Append("update SCHMAST set ")
            .Append(" HAISIN_DATE_S = " & SQ(strDate))
            .Append(",JIFURI_TIME_STAMP_S = " & SQ(strDate & strTime))
            .Append(",HAISIN_FLG_S = '1'")
            .Append(" where TORIS_CODE_S = " & SQ(Key.TORIS_CODE))
            .Append(" and TORIF_CODE_S = " & SQ(Key.TORIF_CODE))
            .Append(" and FURI_DATE_S = " & SQ(Key.FURI_DATE))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
            '*** 修正 mitsu 2008/08/19 ログ内容修正 ***
            'LOG.Write("スケジュール更新", "成功", nRet.ToString)
            MainLOG.Write("スケジュール更新", "成功", intSCH_CNT & "件目")

            '******************************************
        Catch ex As Exception
            MainLOG.Write("スケジュール更新", "失敗", ex.Message)
            Exit Function
        End Try
        fn_SCHMAST_UPDATE = True
    End Function

    ''' <summary>
    ''' 処理結果確認表を出力します。
    ''' </summary>
    ''' <param name="arSyoriKekka">処理結果配列</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function PrnSyoriKekka(ByVal arSyoriKekka As ArrayList) As Boolean

        Dim ListClass As ClsPrnSyorikekka = New ClsPrnSyorikekka(IniInfo.CENTER)
        Dim SKFileName As String = ListClass.CreateCsvFile

        Try

            MainLOG.Write("処理結果確認表(配信データ作成)開始", "成功")

            If ListClass.OutputCSVKekka(arSyoriKekka, intRECORD_CNT) = True Then
                ListClass.CloseCsv()
                ListClass = Nothing
            Else
                MainLOG.UpdateJOBMASTbyErr("処理結果確認表(配信データ作成)CSV出力失敗")
                Return False
            End If

            ' 2016/03/09 タスク）綾部 CHG 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- START
            ' 処理結果確認表印刷要否が "0" の場合は、帳票印刷しない
            If IniInfo.SYORIKEKKA_HAISIN = "0" Then
                Return True
            End If
            ' 2016/03/09 タスク）綾部 CHG 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- END

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String = ""
            Dim errMessage As String
            Dim nret As Integer

            'パラメータ設定：ログイン名、ＣＳＶファイル名
            param = MainLOG.UserID & "," & SKFileName

            nret = ExeRepo.ExecReport("KFJP008.EXE", param)

            If nret <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case nret
                    Case -1
                        errMessage = "処理結果確認表(配信データ作成)の印刷対象が0件です。"
                    Case Else
                        errMessage = "処理結果確認表(配信データ作成)の印刷に失敗しました。"
                End Select

                MainLOG.UpdateJOBMASTbyErr(errMessage)
                MainLOG.Write("処理結果確認表(配信データ作成)印刷", "失敗", errMessage)

                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("処理結果確認表(配信データ作成)", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("処理結果確認表(配信データ作成)出力失敗 例外発生")
            Return False
        Finally
            If Not ListClass Is Nothing Then
                ListClass.CloseCsv()
                ListClass = Nothing
            End If
            MainLOG.Write("処理結果確認表(配信データ作成)終了", "成功")
        End Try
    End Function

    ''' <summary>
    ''' 地区伝送データ送信連絡票を出力します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function PrnChikuSoufu() As Boolean

        Dim ListClass As ClsPrnChikuSoufu = New ClsPrnChikuSoufu
        Dim SKFileName As String = ListClass.CreateCsvFile

        Try
            MainLOG.Write("地区伝送データ送信連絡票開始", "成功")

            If dblALL_KEN > 0 Then  '明細が０件以上だったら

                If ListClass.OutputCSVKekka(intRECORD_CNT, strTime, IniInfo.KINKOBUSYO, IniInfo.KINKOTANTO, IniInfo.KINKOTEL, IniInfo.KINKONAME) = True Then
                    ListClass.CloseCsv()
                    ListClass = Nothing
                Else
                    MainLOG.UpdateJOBMASTbyErr("地区伝送データ送信連絡票CSV出力失敗")
                    Return False
                End If

                Dim ExeRepo As New CAstReports.ClsExecute
                Dim param As String = ""
                Dim errMessage As String
                Dim nret As Integer

                'パラメータ設定：ログイン名、ＣＳＶファイル名
                param = MainLOG.UserID & "," & SKFileName

                nret = ExeRepo.ExecReport("KFJP051.EXE", param)

                If nret <> 0 Then
                    '印刷失敗：戻り値に対応したエラーメッセージを表示する
                    Select Case nret
                        Case -1
                            errMessage = "地区伝送データ送信連絡票の印刷対象が0件です。"
                        Case Else
                            errMessage = "地区伝送データ送信連絡票の印刷に失敗しました。"
                    End Select

                    MainLOG.UpdateJOBMASTbyErr(errMessage)
                    MainLOG.Write("地区伝送データ送信連絡票印刷", "失敗", errMessage)

                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("地区伝送データ送信連絡票", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("地区伝送データ送信連絡票出力失敗 例外発生")
            Return False

        Finally
            If Not ListClass Is Nothing Then
                ListClass.CloseCsv()
                ListClass = Nothing
            End If
            MainLOG.Write("地区伝送データ送信連絡票印刷終了", "成功")
        End Try
    End Function

    ''' <summary>
    ''' 設定ファイルを読み込みます。
    ''' </summary>
    ''' <param name="ErrMsg">エラーメッセージ</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_INI_READ(ByRef ErrMsg As String) As Boolean
        Try
            IniInfo.CENTER = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If IniInfo.CENTER = "err" OrElse IniInfo.CENTER = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:センターコード 分類:COMMON 項目:CENTER")
                ErrMsg = "設定ファイル取得失敗 項目名:センターコード 分類:COMMON 項目:CENTER"
                Return False
            End If

            IniInfo.DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If IniInfo.DEN = "err" OrElse IniInfo.DEN = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN")
                ErrMsg = "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN"
                Return False
            End If

            IniInfo.DAT = CASTCommon.GetFSKJIni("COMMON", "DAT")
            If IniInfo.DAT = "err" OrElse IniInfo.DAT = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
                ErrMsg = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
                Return False
            End If

            IniInfo.DATBK = CASTCommon.GetFSKJIni("COMMON", "DATBK")
            If IniInfo.DATBK = "err" OrElse IniInfo.DATBK = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK")
                ErrMsg = "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK"
                Return False
            End If

            IniInfo.KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If IniInfo.KINKOCD = "err" OrElse IniInfo.KINKOCD = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                ErrMsg = "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD"
                Return False
            End If

            IniInfo.PRTHAISIN = CASTCommon.GetFSKJIni("PRINT", "PRTHAISIN")
            If IniInfo.PRTHAISIN = "err" OrElse IniInfo.PRTHAISIN = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:配信時受付明細表印刷フラグ 分類:PRINT 項目:PRTHAISIN")
                ErrMsg = "設定ファイル取得失敗 項目名:配信時受付明細表印刷フラグ 分類:PRINT 項目:PRTHAISIN"
                Return False
            End If

            IniInfo.SITEIKINKOCD = CASTCommon.GetFSKJIni("COMMON", "SITEIKINKOCD")
            If IniInfo.SITEIKINKOCD = "err" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:SITEIKINKOCD 分類:COMMON 項目:strSITEIKINKO")
                ErrMsg = "設定ファイル取得失敗 項目名:SITEIKINKOCD 分類:COMMON 項目:strSITEIKINKO"
                Return False
            End If
            Dim strSiteikinkoCd() As String
            strSiteikinkoCd = IniInfo.SITEIKINKOCD.Split(","c)
            intSITEIKINKO_KEN = strSiteikinkoCd.Length
            For i As Integer = 0 To strSiteikinkoCd.Length - 1
                strSITEIKINKO_CODE(i) = strSiteikinkoCd(i)
            Next

            IniInfo.FILEAM = CASTCommon.GetFSKJIni("JIFURI", "FILEAM")
            If IniInfo.FILEAM = "err" OrElse IniInfo.FILEAM = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:配信ファイル名(午前) 分類:JIFURI 項目:FILEAM")
                ErrMsg = "設定ファイル取得失敗 項目名:配信ファイル名(午前) 分類:JIFURI 項目:FILEAM"
                Return False
            End If

            IniInfo.FILEPM = CASTCommon.GetFSKJIni("JIFURI", "FILEPM")
            If IniInfo.FILEPM = "err" OrElse IniInfo.FILEPM = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:配信ファイル名(午後) 分類:JIFURI 項目:FILEPM")
                ErrMsg = "設定ファイル取得失敗 項目名:配信ファイル名(午後) 分類:JIFURI 項目:FILEPM"
                Return False
            End If

            IniInfo.KINKONAME = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
            If IniInfo.KINKONAME = "err" OrElse IniInfo.KINKONAME = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:金庫名 分類:PRINT 項目:KINKONAME")
                ErrMsg = "設定ファイル取得失敗 項目名:金庫名 分類:PRINT 項目:KINKONAME"
                Return False
            End If

            IniInfo.KINKOTANTO = CASTCommon.GetFSKJIni("PRINT", "KINKOTANTO")
            If IniInfo.KINKOTANTO = "err" OrElse IniInfo.KINKOTANTO = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:担当名 分類:PRINT 項目:KINKOTANTO")
                ErrMsg = "設定ファイル取得失敗 項目名:担当名 分類:PRINT 項目:KINKOTANTO"
                Return False
            End If

            IniInfo.KINKOTEL = CASTCommon.GetFSKJIni("PRINT", "KINKOTEL")
            If IniInfo.KINKOTEL = "err" OrElse IniInfo.KINKOTEL = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:電話番号 分類:PRINT 項目:KINKOTEL")
                ErrMsg = "設定ファイル取得失敗 項目名:電話番号 分類:PRINT 項目:KINKOTEL"
                Return False
            End If

            '2010/01/20 追加 ======================
            IniInfo.KINKOBUSYO = CASTCommon.GetFSKJIni("PRINT", "KINKOBUSYO")
            If IniInfo.KINKOBUSYO = "err" OrElse IniInfo.KINKOBUSYO = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:部署名 分類:PRINT 項目:KINKOBUSYO")
                ErrMsg = "設定ファイル取得失敗 項目名:部署名 分類:PRINT 項目:KINKOBUSYO"
                Return False
            End If
            '======================================

            IniInfo.CENTER_MOTIKOMI = CASTCommon.GetFSKJIni("JIFURI", "CENTER_MOTIKOMI")
            If IniInfo.CENTER_MOTIKOMI = "err" OrElse IniInfo.CENTER_MOTIKOMI = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:センター持込フォーマット区分 分類:JIFURI 項目:CENTER_MOTIKOMI")
                ErrMsg = "設定ファイル取得失敗 項目名:センターコード持込フォーマット区分 分類:JIFURI 項目:CENTER_MOTIKOMI"
                Return False
            End If

            '2010/09/10.Sakon　追加 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            IniInfo.SOUSINRENRAKU = CASTCommon.GetFSKJIni("PRINT", "SOUSINRENRAKU")
            If IniInfo.SOUSINRENRAKU = "err" OrElse IniInfo.SOUSINRENRAKU = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:送信連絡票区分 分類:PRINT 項目:SOUSINRENRAKU")
                ErrMsg = "設定ファイル取得失敗 項目名:送信連絡票区分 分類:PRINT 項目:SOUSINRENRAKU"
                Return False
            End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            '2010/09/14.Sakon　センターカットデータ分割件数をＩＮＩファイルから取得 +++++++++++++++++++++++++++++++++++++++++++++++++++++
            IniInfo.KITEIKEN = CASTCommon.GetFSKJIni("JIFURI", "KITEIKEN")
            If IniInfo.KITEIKEN = "err" OrElse IniInfo.KITEIKEN = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:センターカットデータ分割件数 分類:JIFURI 項目:KITEIKEN")
                ErrMsg = "設定ファイル取得失敗 項目名:センターカットデータ分割件数 分類:JIFURI 項目:KITEIKEN"
                Return False
            End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            IniInfo.BORDER_TIME = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
            If IniInfo.BORDER_TIME = "err" OrElse IniInfo.BORDER_TIME = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:センターカットデータ配信境界時間 分類:JIFURI 項目:BORDER_TIME")
                ErrMsg = "設定ファイル取得失敗 項目名:センターカットデータ配信境界時間 分類:JIFURI 項目:BORDER_TIME"
                Return False
            End If

            IniInfo.FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")
            If IniInfo.FTR = "err" OrElse IniInfo.FTR = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:FTRフォルダ 分類:COMMON 項目:FTR")
                ErrMsg = "設定ファイル取得失敗 項目名:FTRフォルダ 分類:COMMON 項目:FTR"
                Return False
            End If

            IniInfo.FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If IniInfo.FTRANP = "err" OrElse IniInfo.FTRANP = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:FTRANPフォルダ 分類:COMMON 項目:FTRANP")
                ErrMsg = "設定ファイル取得失敗 項目名:FTRANPフォルダ 分類:COMMON 項目:FTRANP"
                Return False
            End If

            ' 2015/12/14 タスク）綾部 ADD 【PG】UI_B-14-04(RSV2対応) -------------------- START
            IniInfo.KUMIAI_SOUFU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KUMIAI_SOUFU")
            If IniInfo.KUMIAI_SOUFU = "err" OrElse IniInfo.KUMIAI_SOUFU = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:組合持込情報印字 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU")
                ErrMsg = "設定ファイル取得失敗 項目名:組合持込情報印字 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU"
                Return False
            End If

            IniInfo.KUMIAI_SOUFU1 = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KUMIAI_SOUFU1")
            If IniInfo.KUMIAI_SOUFU1 = "err" OrElse IniInfo.KUMIAI_SOUFU1 = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:組合持込情報上段 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU1")
                ErrMsg = "設定ファイル取得失敗 項目名:組合持込情報上段 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU1"
                Return False
            End If

            IniInfo.KUMIAI_SOUFU2 = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KUMIAI_SOUFU2")
            If IniInfo.KUMIAI_SOUFU2 = "err" OrElse IniInfo.KUMIAI_SOUFU2 = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:組合持込情報中段 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU2")
                ErrMsg = "設定ファイル取得失敗 項目名:組合持込情報中段 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU2"
                Return False
            End If

            IniInfo.KUMIAI_SOUFU3 = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KUMIAI_SOUFU3")
            If IniInfo.KUMIAI_SOUFU3 = "err" OrElse IniInfo.KUMIAI_SOUFU3 = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:組合持込情報下段 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU3")
                ErrMsg = "設定ファイル取得失敗 項目名:組合持込情報下段 分類:RSV2_V1.0.0 項目:KUMIAI_SOUFU3"
                Return False
            End If

            IniInfo.KIGYO_SOUFU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KIGYO_SOUFU")
            If IniInfo.KIGYO_SOUFU = "err" OrElse IniInfo.KIGYO_SOUFU = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:企業持込情報印字 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU")
                ErrMsg = "設定ファイル取得失敗 項目名:企業持込情報印字 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU"
                Return False
            End If

            IniInfo.KIGYO_SOUFU1 = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KIGYO_SOUFU1")
            If IniInfo.KIGYO_SOUFU1 = "err" OrElse IniInfo.KIGYO_SOUFU1 = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:企業持込情報上段 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU1")
                ErrMsg = "設定ファイル取得失敗 項目名:企業持込情報上段 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU1"
                Return False
            End If

            IniInfo.KIGYO_SOUFU2 = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KIGYO_SOUFU2")
            If IniInfo.KIGYO_SOUFU2 = "err" OrElse IniInfo.KIGYO_SOUFU2 = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:企業持込情報中段 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU2")
                ErrMsg = "設定ファイル取得失敗 項目名:企業持込情報中段 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU2"
                Return False
            End If

            IniInfo.KIGYO_SOUFU3 = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KIGYO_SOUFU3")
            If IniInfo.KIGYO_SOUFU3 = "err" OrElse IniInfo.KIGYO_SOUFU3 = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:企業持込情報下段 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU3")
                ErrMsg = "設定ファイル取得失敗 項目名:企業持込情報下段 分類:RSV2_V1.0.0 項目:KIGYO_SOUFU3"
                Return False
            End If
            ' 2015/12/14 タスク）綾部 ADD 【PG】UI_B-14-04(RSV2対応) -------------------- END

            ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- START
            IniInfo.SYORIKEKKA_HAISIN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYORIKEKKA_HAISIN")
            If IniInfo.SYORIKEKKA_HAISIN = "err" OrElse IniInfo.SYORIKEKKA_HAISIN = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:処理結果確認表印刷要否 分類:RSV2_V1.0.0 項目:SYORIKEKKA_HAISIN")
                ErrMsg = "設定ファイル取得失敗 項目名:処理結果確認表印刷要否 分類:RSV2_V1.0.0 項目:SYORIKEKKA_HAISIN"
                Return False
            End If
            ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- END

            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
            IniInfo.JIFURI_CCFNAME = CASTCommon.GetFSKJIni("JIFURI", "CCFNAME")
            If IniInfo.JIFURI_CCFNAME = "err" OrElse IniInfo.JIFURI_CCFNAME = "" Then
                MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:センターカットファイル名指定 分類:JIFURI 項目:CCFNAME")
                ErrMsg = "設定ファイル取得失敗 項目名:センターカットファイル名指定 分類:JIFURI 項目:CCFNAME"
                Return False
            End If
            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

            Return True

        Catch ex As Exception
            MainLOG.Write("設定ファイル取得", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("設定ファイル取得失敗 例外発生")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 自振シーケンスを取得します。
    ''' </summary>
    ''' <param name="Key">スケジュール情報</param>
    ''' <returns>最大自振シーケンス＋1</returns>
    ''' <remarks></remarks>
    Private Function GetJifuriSEQ(ByVal Key As strcSchmastInfo) As Integer
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder
        Dim intJifuriSEQ As Integer = 0

        '2010/12/24 信組対応 信組の場合は企業シーケンスを7桁とする
        If IniInfo.CENTER = "0" Then
            SQL.Append("SELECT NVL(MAX(KIGYO_SEQ_K),1999999) AS JIFURI_SEQ_MAX")
            SQL.Append(",COUNT(KIGYO_SEQ_K) AS JIFURI_SEQ_CNT")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & Key.FURI_DATE & "'")
            SQL.Append(" AND KIGYO_SEQ_K BETWEEN '2000000' AND '2999999'")
        Else
            SQL.Append("SELECT NVL(MAX(KIGYO_SEQ_K),19999999) AS JIFURI_SEQ_MAX")
            SQL.Append(",COUNT(KIGYO_SEQ_K) AS JIFURI_SEQ_CNT")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K = '" & Key.FURI_DATE & "'")
            SQL.Append(" AND KIGYO_SEQ_K BETWEEN '20000000' AND '29999999'")
        End If

        Try
            If oraReader.DataReader(SQL) = True Then
                intJifuriSEQ = oraReader.GetInt("JIFURI_SEQ_MAX") + 1
            End If
        Catch ex As Exception
            If IniInfo.CENTER = "0" Then
                intJifuriSEQ = 2000000
            Else
                intJifuriSEQ = 20000000
            End If

        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

        '振替日の企業シーケンス設定
        '2014/05/01 saitou 標準版修正 MODIFY ----------------------------------------------->>>>
        '変数設定ミス
        htJIFURI_SEQ.Add(Key.FURI_DATE, intJifuriSEQ)
        'htJIFURI_SEQ.Add(Key.FURI_DATE, intJIFURI_SEQ)
        '2014/05/01 saitou 標準版修正 MODIFY -----------------------------------------------<<<<

        Return intJifuriSEQ
    End Function

    ''' <summary>
    ''' 企業シーケンスを更新します。
    ''' </summary>
    ''' <param name="Key">スケジュール情報</param>
    ''' <param name="oraReader">明細オラクルリーダー</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function UpdateJifuriSEQ(ByVal Key As strcSchmastInfo, ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("update MEIMAST set KIGYO_SEQ_K = " & SQ(intJIFURI_SEQ))
            .Append(" where TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
            .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
            .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
            .Append(" and RECORD_NO_K = " & oraReader.GetInt("RECORD_NO_K"))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If nRet <> 1 Then
                MainLOG.Write("企業シーケンス更新", "失敗")
                MainLOG.UpdateJOBMASTbyErr("企業シーケンス更新失敗")
                Return False
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("企業シーケンス更新", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("企業シーケンス更新失敗 例外発生")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 配信データ作成対象のスケジュールを取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strSyoriKbn">処理区分(00:金庫,組合フォーマット　01:全銀フォーマット)</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetSchmastSQL(ByVal strSyoriKbn As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from TORIMAST, SCHMAST")
            .Append(" where FURI_DATE_S between " & SQ(HAISIN_DATE) & " and " & SQ(FURI_DATE))
            .Append(" and TOUROKU_FLG_S = '1'")
            .Append(" and HAISIN_FLG_S = '2'")
            .Append(" and TYUUDAN_FLG_S = '0'")
            .Append(" and SOUSIN_KBN_S = " & SQ(SOUSIN_KBN))
            .Append(" and TORIS_CODE_S = TORIS_CODE_T")
            .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            Select Case strSyoriKbn
                Case "00"       '金庫、組合持込フォーマット
                    .Append(" and FSYORI_KBN_T = '1'")
                    If IniInfo.CENTER_MOTIKOMI = "1" Then
                        .Append(" and MOTIKOMI_KBN_T = '0'")
                    End If
                    .Append(" order by FURI_DATE_S, SOUSIN_KBN_S, TORIS_CODE_S, TORIF_CODE_S")

                Case "01"       '全銀フォーマット
                    .Append(" order by FURI_DATE_S, ITAKU_KANRI_CODE_T, FILE_SEQ_S")
            End Select
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 配信データ作成対象の明細を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="Key">スケジュール情報</param>
    ''' <param name="strSyoriKbn">処理区分(00:金庫,組合フォーマット　01:全銀フォーマット)</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetMeimastSQL(ByVal Key As strcSchmastInfo, ByVal strSyoriKbn As String) As StringBuilder

        Dim strDATA_KBN As String
        Select Case Key.FMT_KBN
            Case "02"        '国税
                strDATA_KBN = "3"
            Case Else
                strDATA_KBN = "2"
        End Select

        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from MEIMAST")
            .Append(" where FSYORI_KBN_K = '1'")
            .Append(" and TORIS_CODE_K = " & SQ(Key.TORIS_CODE))
            .Append(" and TORIF_CODE_K = " & SQ(Key.TORIF_CODE))
            .Append(" and FURI_DATE_K = " & SQ(Key.FURI_DATE))
            Select Case strSyoriKbn
                Case "00"       '金庫、組合持込フォーマット
                    .Append(" and DATA_KBN_K = " & SQ(strDATA_KBN))
                    .Append(" and KEIYAKU_KIN_K in (" & SQ(IniInfo.KINKOCD))
                    Select Case IniInfo.CENTER
                        Case "3"        '東海センターのみ指定の金庫を自金庫分と同様にセンターに持ち込む
                            If intSITEIKINKO_KEN = 0 Then
                                .Append(")")
                            Else
                                For i As Integer = 0 To intSITEIKINKO_KEN - 1
                                    .Append("," & SQ(strSITEIKINKO_CODE(i)))
                                Next
                                .Append(")")
                            End If

                        Case Else
                            .Append(")")
                    End Select
                    '2013/03/22 saitou ソース改善 UPD -------------------------------------------------->>>>
                    '取引先コードと振替日はキーなので、ソート順に含めない(含める意味が無い)
                    .Append(" order by RECORD_NO_K")
                    '.Append(" order by TORIS_CODE_K, TORIF_CODE_K, FURI_DATE_K, RECORD_NO_K")
                    '2013/03/22 saitou ソース改善 UPD --------------------------------------------------<<<<

                Case "01"       '全銀フォーマット
                    .Append(" order by RECORD_NO_K")
            End Select
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 自振ヘッダレコード設定
    ''' </summary>
    ''' <param name="WORKFORMAT">ワークファイル（参照渡し）</param>
    ''' <remarks></remarks>
    Private Sub SetHeaderRecordWorkFile(ByRef WORKFORMAT As CAstFormat.CFormatShinkinWork200)
        With WORKFORMAT.JF_DATA1
            .TC = "000000000000"
            .FKN = Space(30)
            .KNM = Space(15)
            .JF1 = "0000"
            .JF2 = "000"
            .JF3 = "0"
            .JF4 = IniInfo.KINKOCD
            ' 2016/11/23 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 標準機能拡張) -------------------- START
            '2005/03/28　導入時に設定値を顧客に確認すること！
            'Select Case IniInfo.CENTER
            '    Case "0"
            '        'SKCの場合、返還区分「9」固定　SKC伝送返還でのマルチ返還
            '        '値を修正するときはfn_CREATE_FILE_SKCも修正対象
            '        .JF5 = "9"
            '    Case Else
            '        '2005/03/28返還区分「2」固定、「1」はMT返還の場合
            '        '.JF5 = "1"
            '        .JF5 = "2"
            'End Select
            '----------------------------------
            ' 返還区分設定
            '----------------------------------
            Dim HenkanKbn As String = CASTCommon.GetFSKJIni("JIFURI", "CC_HENKANKBN")
            Select Case IniInfo.CENTER
                Case "0"
                    '----------------------------------
                    ' ＳＫＣ
                    '----------------------------------
                    Select Case HenkanKbn
                        Case "err", ""
                            .JF5 = "9"
                        Case Else
                            .JF5 = HenkanKbn
                    End Select
                Case Else
                    '----------------------------------
                    ' ＳＫＣ以外
                    '----------------------------------
                    Select Case HenkanKbn
                        Case "err", ""
                            .JF5 = "2"
                        Case Else
                            .JF5 = HenkanKbn
                    End Select
            End Select
            ' 2016/11/23 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 標準機能拡張) -------------------- END
            .JF6 = ""
            .JF6 = .JF6.PadLeft(130, "0"c)
        End With
    End Sub

    ''' <summary>
    ''' 自振データレコード設定
    ''' </summary>
    ''' <param name="WORKFORMAT">ワークファイル（参照渡し）</param>
    ''' <param name="Key">スケジュール情報</param>
    ''' <param name="oraReader">明細オラクルリーダー</param>
    ''' <param name="strWarekiFuriDate">和暦振替日</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function SetDataRecordWorkFile(ByRef WORKFORMAT As CAstFormat.CFormatShinkinWork200, _
                                           ByVal Key As strcSchmastInfo, _
                                           ByVal oraReader As CASTCommon.MyOracleReader, _
                                           ByVal strWarekiFuriDate As String) As Boolean
        With WORKFORMAT.JF_DATA2
            .TC = Key.TORIS_CODE & Key.TORIF_CODE
            .FKN = Key.ITAKU_NNAME.PadRight(30, " "c).Substring(0, 15)
            .KNM = oraReader.GetString("KEIYAKU_KNAME_K")
            .JF1 = oraReader.GetString("KEIYAKU_KIN_K")
            .JF2 = oraReader.GetString("KEIYAKU_SIT_K")
            .JF3 = "2"
            .JF4 = oraReader.GetString("KEIYAKU_KAMOKU_K")
            .JF5 = oraReader.GetString("KEIYAKU_KOUZA_K")
            .JF6 = strWarekiFuriDate
            .JF7 = oraReader.GetInt64("FURIKIN_K").ToString.PadLeft(13, "0"c)
            .JF8 = Key.NS_KBN
            .JF9 = Key.KIGYO_CODE
            '2010/12/24 信組対応 信組の場合は企業シーケンスを7桁とする
            If IniInfo.CENTER = "0" Then
                .JF10 = intJIFURI_SEQ.ToString("0000000")
            Else
                .JF10 = intJIFURI_SEQ.ToString("00000000")
            End If
            .JF11 = "00"
            .JF12 = Key.FURI_CODE
            .JF13 = "00"
            .JF14 = "0000000"
            .JF15 = "0000"
            .JF16 = "0000"

            Dim strRet As String = String.Empty '追加 2007/05/29
            Select Case oraReader.GetString("TEKIYO_KBN_K")
                Case "0"
                    .JF17 = "0"
                    '追加 2007/05/29
                    If fn_CHANGE_KANA_SPASE(oraReader.GetString("KTEKIYO_K"), strRet) = False Then
                        MainLOG.UpdateJOBMASTbyErr("摘要大文字変換失敗")
                        Return False
                    Else
                        .JF18 = strRet
                    End If

                    '*** 修正 mitsu 2008/09/02 処理高速化 ***
                    'WrkFmt.JF_DATA2.JF19 = Space(12)
                    .JF19 = New String(" "c, 12)
                    '****************************************

                Case "1"
                    .JF17 = "1"
                    '*** 修正 mitsu 2008/09/02 処理高速化 ***
                    'WrkFmt.JF_DATA2.JF18 = Space(13)
                    .JF18 = New String(" "c, 13)
                    '****************************************
                    .JF19 = oraReader.GetString("NTEKIYO_K")

                Case "2"
                    .JF17 = "0"
                    '追加 2007/05/29
                    If fn_CHANGE_KANA_SPASE(oraReader.GetString("KTEKIYO_K"), strRet) = False Then
                        MainLOG.UpdateJOBMASTbyErr("摘要大文字変換失敗")
                        Return False
                    Else
                        .JF18 = strRet
                    End If

                    .JF19 = New String(" "c, 12)

                Case "3"
                    .JF17 = "0"
                    '追加 2007/05/29
                    If fn_CHANGE_KANA_SPASE(oraReader.GetString("KTEKIYO_K"), strRet) = False Then
                        MainLOG.UpdateJOBMASTbyErr("摘要大文字変換失敗")
                        Return False
                    Else
                        .JF18 = strRet
                    End If

                    .JF19 = New String(" "c, 12)

            End Select

            '2010/01/19 需要家番号を予備4から取得する
            .JF20 = oraReader.GetString("YOBI4_K")
            .JF21 = "0000000"
            Select Case Key.FURI_KYU_CODE
                Case "0"
                    .JF22 = "1"
                Case "1"
                    .JF22 = "2"
            End Select
            .JF23 = Space(1)
        End With
        Return True
    End Function

    ''' <summary>
    ''' 自振エンドレコード設定
    ''' </summary>
    ''' <param name="WORKFORMAT">ワークファイル（参照渡し）</param>
    ''' <remarks></remarks>
    Private Sub SetEndRecordWorkFile(ByRef WORKFORMAT As CAstFormat.CFormatShinkinWork200)
        With WORKFORMAT.JF_DATA3
            .TC = "999999999999"
            .FKN = Space(30)
            .KNM = Space(15)
            .JF1 = "0000"
            .JF2 = "000"
            .JF3 = "9"
            .JF4 = ""
            .JF4 = .JF4.PadLeft(135, "0")
        End With
    End Sub

    ''' <summary>
    ''' カナ大文字変換
    ''' </summary>
    ''' <param name="strReadString">対象文字列</param>
    ''' <param name="strRepString">変換後文字列</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_CHANGE_KANA_SPASE(ByVal strReadString As String, _
                                          ByRef strRepString As String) As Boolean
        Dim sbReturn As New StringBuilder("", strReadString.Length)

        Try
            For i As Integer = 0 To strReadString.Length - 1
                Select Case strReadString.Chars(i)
                    Case "ｧ" : sbReturn.Append("ｱ")
                    Case "ｨ" : sbReturn.Append("ｲ")
                    Case "ｩ" : sbReturn.Append("ｳ")
                    Case "ｪ" : sbReturn.Append("ｴ")
                    Case "ｫ" : sbReturn.Append("ｵ")
                    Case "ｬ" : sbReturn.Append("ﾔ")
                    Case "ｭ" : sbReturn.Append("ﾕ")
                    Case "ｮ" : sbReturn.Append("ﾖ")
                    Case "ｯ" : sbReturn.Append("ﾂ")
                    Case "ｰ" : sbReturn.Append("-")
                    Case "A" To "Z", "ｱ" To "ﾝ", "0" To "9"
                        sbReturn.Append(strReadString.Chars(i))
                    Case "ﾞ", "ﾟ"
                        sbReturn.Append(strReadString.Chars(i))
                        '*&$は規定外文字
                    Case "\", ",", ".", "｢", "｣", "-", "/", "(", ")"
                        sbReturn.Append(strReadString.Chars(i))
                    Case " " : sbReturn.Append(strReadString.Chars(i))
                    Case Else : sbReturn.Append(" ")
                End Select
            Next

            strRepString = sbReturn.ToString
            Return True

        Catch ex As Exception
            MainLOG.Write("カナ大文字変換", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Class
