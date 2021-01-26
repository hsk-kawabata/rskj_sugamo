Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstFormat

Module ModMain
    ' ログ処理クラス
    Private ELog As New CASTCommon.ClsEventLOG
    Private MainLOG As New CASTCommon.BatchLOG("KFJ050", "日報集計処理バッチ")
    Private Const msgTitle As String = "日報集計処理バッチ(KFJ050)"
    ' パブリックＤＢ
    Private MainDB As New CASTCommon.MyOracle
    '*** Str Del 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
    'Private OraReader As CASTCommon.MyOracleReader
    '*** Str Del 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

    ' システム連携
    Private clsFUSION As New clsFUSION.clsMain
    Private Renkei As New CAstSystem.ClsRenkei

    'スケジュール選択画面
    Private KFJMAIN051 As New KFJMAIN051

    ' パラメータ
    Private FuriDate As String = ""
    Private JobTuuban As Integer = 0
    Private JobMessage As String = ""
    Private FileKbn As String
    'INIファイルより
    Private DEN_DIR, DENBK_DIR, DATBK_DIR, NTC, NIPPOU_HONBU, CSV_DIR As String
    '2017/04/10 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ START
    '手数料再計算区分
    Private TESUU_RECALC_KBN As String = ""
    '手数料再計算待機時間
    Private TESUU_RECALC_SLEEPTIME As Integer = 3000
    '2017/04/10 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ END
    '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- START
    Private CENTER_CODE As String
    '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- END

    '共通変数
    Private FURI_DATE As String = ""
    Private RECORD_SYUBETU As String = ""
    Private FURI_CODE As String = ""
    Private KIGYO_CODE As String = ""
    Private FURI_SYUBETU As String = ""
    Private NS_KBN As String = ""

    Private TORIS_CODE As String = ""
    Private TORIF_CODE As String = ""
    Private ITAKU_NAME As String = ""
    Private HONBU_CODE As String = ""
    Private MOTIKOMI_KBN As String = ""
    Private KEKKA_HENKYAKU_KBN As String = ""   '2009.12.18 追加

    Private FURI_KEN As Long = 0
    Private FURI_KIN As Long = 0
    Private FUNOU_KEN As Long = 0
    Private FUNOU_KIN As Long = 0

    Private RECORD_COUNT As Integer = 0

    '取引先エラーリスト用
    Private Structure TORI_ERR
        Public FURI As String
        Public KIGYO As String
        Public FURI_DATE As String
    End Structure
    Private LIST_TORI_ERR As New List(Of TORI_ERR)

    'スケジュールエラーリスト用
    Private Structure SCH_ERR
        Public FURI As String
        Public KIGYO As String
        Public FURI_DATE As String
        Public TORI_CODE As String
        Public TORI_NAME As String
        Public MSG As String
    End Structure
    Private LIST_SCH_ERR As New List(Of SCH_ERR)

    Public GCom As MenteCommon.clsCommon

    Public Function Main(ByVal CmdArgs() As String) As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw1 As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw1 = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "日報集計(開始)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            ELog.Write("開始")

            '初期処理
            If NippouInit(CmdArgs) = False Then
                Return -1
            End If

            '主処理
            Dim ret As Boolean = NippouMain()

            '終了処理
            If NippouEnd(ret) = False Then
                Return -1
            Else
                Return 0
            End If

        Catch ex As Exception
            ELog.Write(ex.Message)
            Return -1
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw1, "", "0000000000-00", "00000000", "日報集計(終了)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            ELog.Write("終了")


        End Try
    End Function

    Private Function NippouInit(ByVal CmdArgs() As String) As Boolean

        Try
            MainLOG.Write("(初期処理)開始", "成功")

            'パラメータチェック
            If CmdArgs.Length = 0 Then
                MainLOG.Write("パラメータチェック", "失敗", "コマンドライン引数異常")
                Return False
            End If

            Dim param() As String = CmdArgs(0).Split(","c)
            If param.Length = 4 Then
                FuriDate = param(0)
                FileKbn = param(2)  '2009/09/17 ファイル区分追加
                If FileKbn <> "1" AndAlso FileKbn <> "2" Then
                    MainLOG.Write("パラメータチェック", "失敗", "ファイル区分異常：" & CmdArgs(0))
                    Return False
                End If
                JobTuuban = CInt(param(3))

                'ログの初期設定
                MainLOG.JobTuuban = JobTuuban
                MainLOG.UserID = param(1)
                MainLOG.FuriDate = FuriDate
            Else
                MainLOG.Write("パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("初期処理", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write("(初期処理)終了", "成功")
        End Try

        Return True
    End Function

    Private Function NippouMain() As Boolean

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 300
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME2")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 300
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

        Try
            MainLOG.Write("(主処理)開始", "成功")

            If IniRead() = False Then
                Return False
            End If

            '日報ファイル
            Dim InFile As String = ""
            Dim OutFile As String = ""
            'ファイル区分の考慮追加
            Select Case FileKbn
                Case "1"
                    InFile = Path.Combine(DEN_DIR, "NIPPOU.DAT")
                    OutFile = Path.Combine(DATBK_DIR, "NIPPOU_JIS.DAT")
                Case "2"
                    InFile = Path.Combine(DEN_DIR, "NIPPOU2.DAT")
                    OutFile = Path.Combine(DATBK_DIR, "NIPPOU2_JIS.DAT")
            End Select

            '日報ファイルの存在チェック
            If File.Exists(InFile) = False Then
                MainLOG.Write("日報ファイル存在チェック", "失敗", "日報ファイル取得失敗 日報ファイル名:" & InFile)
                JobMessage = "日報ファイルが存在しません。"
                Return False
            End If

            '日報ファイルのコード変換
            If NTC = "1" Then   'NTC接続の場合コード変換を行わない
                File.Copy(InFile, OutFile, True)
            Else
                Dim pFile As String = "NIPPOU.P"
                Dim ret As Integer

                ret = clsFUSION.fn_DEN_CPYTO_DISK("00000000", InFile, OutFile, CFormatNippou.RecordLen, "4", pFile, msgTitle)
                Select Case ret
                    Case 0
                        MainLOG.Write("コード変換", "成功")
                    Case 100
                        MainLOG.Write("コード変換", "失敗", "コード変換失敗　ファイルパス:" & InFile)
                        JobMessage = "コード変換に失敗しました。"
                        Return False
                    Case 200
                        MainLOG.Write("コード変換", "失敗", "コード区分異常(JIS改行あり)　ファイルパス:" & InFile)
                        JobMessage = "コード変換に失敗しました。"
                        Return False
                    Case 300
                        MainLOG.Write("コード変換", "失敗", "コード区分異常(JIS改行なし)　ファイルパス:" & InFile)
                        JobMessage = "コード変換に失敗しました。"
                        Return False
                    Case 400
                        MainLOG.Write("コード変換", "失敗", "出力ファイル作成失敗　ファイルパス:" & OutFile)
                        JobMessage = "コード変換に失敗しました。"
                        Return False
                End Select
            End If

            'ファイルソート
            Dim SortFile As String = Path.Combine(DATBK_DIR, "NIPPOU_JIS.SRT")

            Dim intDispMessage As Boolean = False
            Dim intDisposalNumber As Integer = 0
            Dim intFieldDefinition As Integer = 1
            '新フォーマット対応
            Dim strKeyCmdStr As String = "7.2asca,68.8asca,9.3asca,12.5asca"
            Dim strInputFiles As String = OutFile
            Dim intInputFileType As Integer = 1
            Dim strOutputFile As String = SortFile
            Dim intOutputFileType As Integer = 1
            '2016/10/06 タスク）西野 CHG 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- START
            '改行文字考慮(東京センタ)
            Dim intMaxRecordLength As Integer
            If CENTER_CODE = "3" Then
                intMaxRecordLength = 315
            Else
                intMaxRecordLength = 313
            End If
            'Dim intMaxRecordLength As Integer = 313
            '2016/10/06 タスク）西野 CHG 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- END

            If clsFUSION.fn_POWER_SORT(intDisposalNumber, intFieldDefinition, strKeyCmdStr, strInputFiles, _
                             intInputFileType, strOutputFile, intOutputFileType, intMaxRecordLength) = False Then
                MainLOG.Write("ファイルソート", "失敗")
                JobMessage = "ファイルのソートに失敗しました。"
                Return False
            End If

            'トランザクション開始
            MainDB.BeginTrans()

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("日報集計処理", "失敗", "日報集計処理で実行待ちタイムアウト")
                JobMessage = "日報集計処理で実行待ちタイムアウト"
                Return False
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            'ファイル読込
            If ReadFile(SortFile) = False Then
                Return False
            End If

            'スケジュール対象なしのエラーリスト印刷
            If LIST_SCH_ERR.Count > 0 Then
                Dim csvname As String = Path.Combine(CSV_DIR, "KFJP014_" & _
                                                     DateTime.Now.ToString("yyyyMMddHHmmss") & "_" & Process.GetCurrentProcess.Id & ".CSV")
                Dim sw As StreamWriter = New StreamWriter(csvname, False, Encoding.GetEncoding(932))

                For Each SCH_ERR In LIST_SCH_ERR
                    Dim s(9) As String
                    s(0) = DQ("自振集計日報")
                    s(1) = DQ(DateTime.Today.ToString("yyyyMMdd"))
                    s(2) = DQ(DateTime.Now.ToString("HHmmss"))
                    s(3) = DQ(SCH_ERR.TORI_CODE.Split("-"c)(0))
                    s(4) = DQ(SCH_ERR.TORI_CODE.Split("-"c)(1))
                    s(5) = DQ(SCH_ERR.TORI_NAME)
                    s(6) = DQ(SCH_ERR.FURI)
                    s(7) = DQ(SCH_ERR.KIGYO)
                    s(8) = DQ(SCH_ERR.FURI_DATE)
                    s(9) = DQ(SCH_ERR.MSG)

                    sw.WriteLine(String.Join(",", s))
                Next

                sw.Close()

                Dim ExeRepo As New CAstReports.ClsExecute
                Dim ret As Integer = ExeRepo.ExecReport("KFJP014", MainLOG.UserID & "," & csvname)
                If ret <> 0 Then
                    MainLOG.Write("スケジュールマスタエラーリスト印刷", "失敗", "リターンコード:" & ret)
                    JobMessage = "スケジュールマスタエラーリストの印刷に失敗しました。"
                    Return False
                Else
                    MainLOG.Write("スケジュールマスタエラーリスト印刷", "成功", "")
                End If
            End If
            '取引先マスタ登録なしのエラーリスト印刷
            If LIST_TORI_ERR.Count > 0 Then
                Dim csvname As String = Path.Combine(CSV_DIR, "KFJP015_" & _
                                                     DateTime.Now.ToString("yyyyMMddHHmmss") & "_" & Process.GetCurrentProcess.Id & ".CSV")
                Dim sw As StreamWriter = New StreamWriter(csvname, False, Encoding.GetEncoding(932))

                For Each TORI_ERR In LIST_TORI_ERR
                    Dim s(5) As String
                    s(0) = DQ("自振集計日報")
                    s(1) = DQ(DateTime.Today.ToString("yyyyMMdd"))
                    s(2) = DQ(DateTime.Now.ToString("HHmmss"))
                    s(3) = DQ(TORI_ERR.FURI)
                    s(4) = DQ(TORI_ERR.KIGYO)
                    s(5) = DQ(TORI_ERR.FURI_DATE)

                    sw.WriteLine(String.Join(",", s))
                Next

                sw.Close()

                Dim ExeRepo As New CAstReports.ClsExecute
                Dim ret As Integer = ExeRepo.ExecReport("KFJP015", MainLOG.UserID & "," & csvname)
                If ret <> 0 Then
                    MainLOG.Write("取引先マスタエラーリスト印刷", "失敗", "リターンコード:" & ret)
                    JobMessage = "取引先マスタエラーリストの印刷に失敗しました。"
                    Return False
                Else
                    MainLOG.Write("取引先マスタエラーリスト印刷", "成功", "")

                End If
            End If

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            'コミットを行う
            MainDB.Commit()

            '2009/12/03 ファイルの移動を行う =======================================
            Dim ToFile As String = Path.Combine(DENBK_DIR, Now.ToString("dd") & "_" & Path.GetFileName(InFile))
            If File.Exists(ToFile) Then
                Kill(ToFile)
            End If
            File.Move(InFile, ToFile)
            MainLOG.Write("入力ファイル正常フォルダへ移動", "成功", InFile & " -> " & ToFile)
            '========================================================================

            ''手数料計算を呼び出す
            'Dim p As Process = Process.Start("KFJ070", FuriDate)
            'p.WaitForExit()
            'If p.Id <> 0 Then
            '    MainLOG.Write("手数料計算", "失敗", "リターンコード:" & p.Id)
            '    JobMessage = "手数料計算に失敗しました。"
            '    Return False
            'End If
            '手数料計算をジョブ登録
            Dim jobid As String
            Dim para As String
            Try
                '*** Str Add 2015/12/01 SO)荒木 for トラン明確化 ***
                'トランザクション開始（明示的に開始するようにする）
                MainDB.BeginTrans()
                '*** End Add 2015/12/01 SO)荒木 for トラン明確化 ***

                'ジョブマスタに登録
                jobid = "J070"                      '..\Batch\手数料計算\
                'パラメータ(振替日)
                para = FuriDate
                'job検索
                Dim iRet As Integer
                iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                If iRet = 1 Then    'ジョブ登録済
                    MainLOG.Write("手数料計算登録", "失敗", "ジョブ登録済み")
                ElseIf iRet = -1 Then 'ジョブ検索失敗
                    MainLOG.Write("手数料計算登録", "失敗")

                Else
                    'job登録
                    If MainLOG.InsertJOBMAST(jobid, MainLOG.UserID, para, MainDB) = False Then 'ジョブ登録失敗(MSG????E)
                        MainLOG.Write("手数料計算登録", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                    End If
                End If

                '*** Str Add 2015/12/01 SO)荒木 for トラン明確化 ***
                'コミットを行う（明示的にコミットするようにする）
                MainDB.Commit()
                '*** End Add 2015/12/01 SO)荒木 for トラン明確化 ***
            Catch ex As Exception
                MainLOG.Write("手数料計算登録", "失敗", ex.ToString)

            End Try

            '2017/04/10 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ START
            ' 振替ｺｰﾄﾞ、企業ｺｰﾄﾞが同一でかつ別取引先の金庫持込が存在する場合について、
            ' 日報・不能結果の処理順が逆転（共同センターの障害発生にて日報が不能より遅延）した場合に、
            ' 振替済、不能が日報情報で上書きされるため、手数料計算では明細マスタから再計算を行うよう、対応を実施する。

            ' INIファイルの「日報取込手数料再計算区分」が「1:手数料再計算する」の場合に処理する
            If TESUU_RECALC_KBN = "1" Then
                '------------------------------------------------
                ' 手数料再計算をジョブ登録
                '------------------------------------------------
                Try
                    'トランザクション開始（明示的に開始するようにする）
                    MainDB.BeginTrans()

                    'ジョブマスタに登録
                    jobid = "J070"                      '..\Batch\手数料計算\
                    'パラメータ(振替日)
                    para = FuriDate & ",1"

                    Threading.Thread.Sleep(TESUU_RECALC_SLEEPTIME)

                    'job検索
                    Dim iRet As Integer
                    iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                    If iRet = 1 Then    'ジョブ登録済
                        MainLOG.Write("手数料再計算登録", "失敗", "ジョブ登録済み")
                    ElseIf iRet = -1 Then 'ジョブ検索失敗
                        MainLOG.Write("手数料再計算登録", "失敗")

                    Else
                        'job登録
                        If MainLOG.InsertJOBMAST(jobid, MainLOG.UserID, para, MainDB) = False Then
                            MainLOG.Write("手数料再計算登録", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                        End If
                    End If

                    'コミットを行う（明示的にコミットするようにする）
                    MainDB.Commit()
                Catch ex As Exception
                    MainLOG.Write("手数料再計算登録", "失敗", ex.ToString)

                End Try
            End If
            '2017/04/10 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ END

        Catch ex As Exception
            MainLOG.Write("主処理", "失敗", ex.Message)
            JobMessage = ex.Message
            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for トラン明確化 ***
            ' 現状、ジョブ登録に対する明示的なコミット、ロールバックを行ってなく、MainDBのFinalizeで
            ' ロールバックが呼ばれている
            ' このロールバックは、明示的にトランを開始していないためNOPとなり、DBコネクションのクローズ
            ' でコミットされているが、本来、コミット、ロールバックは明確に呼出すべきである

            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)

            'ロールバック（明示的にロールバックするようにする）
            MainDB.Rollback()

            'ロールバック（明示的にクローズするようにする）
            MainDB.Close()
            '*** End Add 2015/12/01 SO)荒木 for トラン明確化 ***
            MainLOG.Write("(主処理)終了", "成功")

        End Try

        Return True
    End Function

    Private Function NippouEnd(ByVal ret As Boolean) As Boolean

        Try
            MainLOG.Write("(終了処理)開始", "成功")

            If ret = False Then
                MainLOG.UpdateJOBMASTbyErr(JobMessage)
                Return False
            Else
                MainLOG.UpdateJOBMASTbyOK("")
            End If

        Catch ex As Exception
            MainLOG.Write("終了処理", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write("(終了処理)終了", "成功")
        End Try

        Return True
    End Function

    Private Function IniRead() As Boolean
        'DENフォルダチェック
        DEN_DIR = CASTCommon.GetFSKJIni("COMMON", "DEN")
        If DEN_DIR = "err" OrElse DEN_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN")
            JobMessage = "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN"
            Return False
        End If

        'DENBKフォルダチェック
        DENBK_DIR = CASTCommon.GetFSKJIni("COMMON", "DENBK")
        If DENBK_DIR = "err" OrElse DENBK_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DENBKフォルダ 分類:COMMON 項目:DENBK")
            JobMessage = "設定ファイル取得失敗 項目名:DENBKフォルダ 分類:COMMON 項目:DENBK"
            Return False
        End If

        'DATBKフォルダチェック
        DATBK_DIR = CASTCommon.GetFSKJIni("COMMON", "DATBK")
        If DATBK_DIR = "err" OrElse DATBK_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK")
            JobMessage = "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK"
            Return False
        End If

        '接続方法チェック
        NTC = CASTCommon.GetFSKJIni("NIPPOU", "NTC")
        If NTC = "err" OrElse NTC = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:NTC接続方法 分類:NIPPOU 項目:NTC")
            JobMessage = "設定ファイル取得失敗 項目名:NTC接続方法 分類:NIPPOU 項目:NTC"
            Return False
        End If

        '日報本部コードチェック
        NIPPOU_HONBU = CASTCommon.GetFSKJIni("NIPPOU", "HONBUCD")
        If NIPPOU_HONBU = "err" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:日報本部コード 分類:NIPPOU 項目:HONBUCD")
            JobMessage = "設定ファイル取得失敗 項目名:日報本部コード 分類:NIPPOU 項目:HONBUCD"
            Return False
        End If

        'CSVフォルダチェック
        CSV_DIR = CASTCommon.GetFSKJIni("COMMON", "PRT")
        If CSV_DIR = "err" OrElse CSV_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:CSVフォルダ 分類:COMMON 項目:CSV")
            JobMessage = "設定ファイル取得失敗 項目名:CSVフォルダ 分類:COMMON 項目:CSV"
            Return False
        End If

        '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- START
        'センターコード追加
        CENTER_CODE = CASTCommon.GetFSKJIni("COMMON", "CENTER")
        If CSV_DIR = "err" OrElse CSV_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:センターコード 分類:COMMON 項目:CENTER")
            JobMessage = "設定ファイル取得失敗 項目名:センターコード 分類:COMMON 項目:CENTER"
            Return False
        End If
        '2016/10/06 タスク）西野 ADD 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- END

        '2017/04/10 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ START
        '日報取込手数料再計算区分
        TESUU_RECALC_KBN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TESUU_RECALC_KBN")
        If TESUU_RECALC_KBN = "err" OrElse TESUU_RECALC_KBN = "" Then
            TESUU_RECALC_KBN = "0"
        End If
        '手数料再計算待機時間
        Dim wkini As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TESUU_RECALC_SLEEPTIME")
        If wkini = "err" OrElse wkini = "" Then
            TESUU_RECALC_SLEEPTIME = 3000
        Else
            TESUU_RECALC_SLEEPTIME = CInt(wkini)
        End If
        '2017/04/10 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ END

        Return True
    End Function

    Private Function ReadFile(ByVal FileName As String) As Boolean
        Dim Nippou As New CFormatNippou
        Dim NIPPO_DATA As CFormatNippou.NIPPO_DATA_RECORD

        Try
            'ファイルオープン
            Select Case Nippou.FirstRead(FileName)
                Case 0
                    MainLOG.Write("ファイル読込", "失敗", "ファイルパス:" & FileName)
                    JobMessage = "ファイル読込に失敗しました。"
                    Return False
                Case 1
                    MainLOG.Write("ファイル読込", "成功", "ファイルパス:" & FileName)
            End Select

            '次レコードとの比較の初期値を取得するため、1行目を読み込む
            Nippou.CheckDataFormat()
            NIPPO_DATA = Nippou.NIPPO_DATA

            RECORD_SYUBETU = NIPPO_DATA.NI3
            FURI_DATE = NIPPO_DATA.NI9
            FURI_CODE = NIPPO_DATA.NI4
            KIGYO_CODE = NIPPO_DATA.NI5
            FURI_SYUBETU = NIPPO_DATA.NI6
            NS_KBN = GetNS_KBN(FURI_SYUBETU)

            '1レコード目に戻る
            Nippou.FirstRead(FileName)

            While Nippou.EOF = False
                RECORD_COUNT += 1
                Nippou.CheckDataFormat()
                NIPPO_DATA = Nippou.NIPPO_DATA

                '指定日翌営業日分は読み飛ばす
                If RECORD_SYUBETU <> "03" Then
                    If RECORD_SYUBETU = NIPPO_DATA.NI3 AndAlso FURI_DATE = NIPPO_DATA.NI9 AndAlso FURI_CODE = NIPPO_DATA.NI4 AndAlso KIGYO_CODE = NIPPO_DATA.NI5 Then
                        FURI_KEN += CLng(NIPPO_DATA.NI10) + CLng(NIPPO_DATA.NI12) + CLng(NIPPO_DATA.NI14) + CLng(NIPPO_DATA.NI16) + CLng(NIPPO_DATA.NI18)
                        FURI_KIN += CLng(NIPPO_DATA.NI11) + CLng(NIPPO_DATA.NI13) + CLng(NIPPO_DATA.NI15) + CLng(NIPPO_DATA.NI17) + CLng(NIPPO_DATA.NI19)
                        FUNOU_KEN += CLng(NIPPO_DATA.NI20) + CLng(NIPPO_DATA.NI22) + CLng(NIPPO_DATA.NI24)
                        FUNOU_KIN += CLng(NIPPO_DATA.NI21) + CLng(NIPPO_DATA.NI23) + CLng(NIPPO_DATA.NI25)
                    Else
                        '-----------------------------------------------
                        'スケジュールの検索、更新、日報マスタへのインサート
                        '-----------------------------------------------
                        If SearchKIGYO() = False Then
                            Return False
                        End If

                        RECORD_SYUBETU = NIPPO_DATA.NI3
                        FURI_DATE = NIPPO_DATA.NI9
                        FURI_CODE = NIPPO_DATA.NI4
                        KIGYO_CODE = NIPPO_DATA.NI5
                        FURI_KEN = CLng(NIPPO_DATA.NI10) + CLng(NIPPO_DATA.NI12) + CLng(NIPPO_DATA.NI14) + CLng(NIPPO_DATA.NI16) + CLng(NIPPO_DATA.NI18)
                        FURI_KIN = CLng(NIPPO_DATA.NI11) + CLng(NIPPO_DATA.NI13) + CLng(NIPPO_DATA.NI15) + CLng(NIPPO_DATA.NI17) + CLng(NIPPO_DATA.NI19)
                        FUNOU_KEN = CLng(NIPPO_DATA.NI20) + CLng(NIPPO_DATA.NI22) + CLng(NIPPO_DATA.NI24)
                        FUNOU_KIN = CLng(NIPPO_DATA.NI21) + CLng(NIPPO_DATA.NI23) + CLng(NIPPO_DATA.NI25)
                    End If
                End If

                RECORD_SYUBETU = NIPPO_DATA.NI3
                FURI_DATE = NIPPO_DATA.NI9
                FURI_CODE = NIPPO_DATA.NI4
                KIGYO_CODE = NIPPO_DATA.NI5
                FURI_SYUBETU = NIPPO_DATA.NI6
                NS_KBN = GetNS_KBN(FURI_SYUBETU)
            End While
            '2010/03/08 初振(種別=01)のみのデータの場合、最終行がインサートされずに終了する
            '最後の種別が初振の場合のときのみインサート処理を行う必要あり
            If RECORD_SYUBETU = "01" Then
                If SearchKIGYO() = False Then
                    Return False
                End If
            End If
            '============================================================================
            Nippou.Close()

        Catch ex As Exception
            Throw
        Finally
        End Try

        Return True
    End Function

    Private Function SearchKIGYO() As Boolean

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        Try
            '変数宣言
            Dim NIPPOU_FLG As String = ""
            Select Case RECORD_SYUBETU
                Case "01"     '初振分
                    NIPPOU_FLG = "'0','1'"
                Case "02"     '指定日当日
                    NIPPOU_FLG = "'0','1','2'"
            End Select

            Dim LIST_TORIS_CODE As New List(Of String)
            Dim LIST_TORIF_CODE As New List(Of String)
            Dim LIST_HONBU_CODE As New List(Of String)
            Dim LIST_MOTIKOMI As New List(Of String)

            'リスト初期化
            KFJMAIN051.lstSCHLIST.Rows.Clear()

            'スケジュール検索(振替日、振替コード、企業コード)
            Dim SQL As StringBuilder = New StringBuilder
            SQL.AppendLine("SELECT * FROM SCHMAST, TORIMAST WHERE ")
            SQL.AppendLine("FURI_DATE_S = '" & FURI_DATE & "' AND ")
            SQL.AppendLine("FURI_CODE_S = '" & FURI_CODE & "' AND ")
            SQL.AppendLine("KIGYO_CODE_S = '" & KIGYO_CODE & "' AND ")
            SQL.AppendLine("NS_KBN_T = '" & NS_KBN & "' AND ")
            SQL.AppendLine("HENKAN_FLG_S = '0' AND ")
            SQL.AppendLine("NOT (MOTIKOMI_KBN_S = '0' AND HAISIN_FLG_S = '0') AND ")
            SQL.AppendLine("NIPPO_FLG_S IN (" & NIPPOU_FLG & ") AND ")
            SQL.AppendLine("TYUUDAN_FLG_S = '0' AND ")
            SQL.AppendLine("TORIS_CODE_S = TORIS_CODE_T AND ")
            SQL.AppendLine("TORIF_CODE_S = TORIF_CODE_T AND ")
            SQL.AppendLine("FURI_CODE_S = FURI_CODE_T AND ")
            SQL.AppendLine("KIGYO_CODE_S = KIGYO_CODE_T ")
            SQL.AppendLine("ORDER BY TORIS_CODE_T, TORIF_CODE_T")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            '-----------------------------------------
            'リストに追加する
            '-----------------------------------------
            Dim intCOUNT_SCH As Integer = 0

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    intCOUNT_SCH += 1
                    TORIS_CODE = OraReader.GetString("TORIS_CODE_S")
                    TORIF_CODE = OraReader.GetString("TORIF_CODE_S")
                    ITAKU_NAME = OraReader.GetString("ITAKU_NNAME_T")
                    MOTIKOMI_KBN = OraReader.GetString("MOTIKOMI_KBN_T")
                    KEKKA_HENKYAKU_KBN = OraReader.GetString("KEKKA_HENKYAKU_KBN_T")    '結果要否追加 2009.12.18
                    LIST_TORIS_CODE.Add(TORIS_CODE)
                    LIST_TORIF_CODE.Add(TORIF_CODE)
                    LIST_HONBU_CODE.Add(OraReader.GetString("TSIT_NO_T"))
                    LIST_MOTIKOMI.Add(MOTIKOMI_KBN)

                    KFJMAIN051.lstSCHLIST.Rows.Add(TORIS_CODE & "-" & TORIF_CODE, ITAKU_NAME, FURI_CODE, KIGYO_CODE, FURI_DATE)

                    OraReader.NextRead()
                End While
            Else
                '中断フラグが０のものが存在しない場合は、中断フラグを除いたものを検索する
                OraReader.Close()

                SQL.Length = 0
                SQL.AppendLine("SELECT * FROM SCHMAST, TORIMAST WHERE ")
                SQL.AppendLine("FURI_DATE_S = '" & FURI_DATE.Trim & "' AND ")
                SQL.AppendLine("FURI_CODE_S = '" & FURI_CODE.Trim & "' AND ")
                SQL.AppendLine("KIGYO_CODE_S = '" & KIGYO_CODE.Trim & "' AND ")
                SQL.AppendLine("NS_KBN_T = '" & NS_KBN & "' AND ")
                SQL.AppendLine("HENKAN_FLG_S = '0' AND ")
                SQL.AppendLine("NOT (MOTIKOMI_KBN_S = '0' AND HAISIN_FLG_S = '0') AND ")
                SQL.AppendLine("NIPPO_FLG_S IN (" & NIPPOU_FLG & ") AND ")
                SQL.AppendLine("TORIS_CODE_S = TORIS_CODE_T AND ")
                SQL.AppendLine("TORIF_CODE_S = TORIF_CODE_T AND ")
                SQL.AppendLine("FURI_CODE_S = FURI_CODE_T AND ")
                SQL.AppendLine("KIGYO_CODE_S = KIGYO_CODE_T ")
                SQL.AppendLine("ORDER BY TORIS_CODE_T, TORIF_CODE_T")

                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        intCOUNT_SCH += 1
                        TORIS_CODE = OraReader.GetString("TORIS_CODE_S")
                        TORIF_CODE = OraReader.GetString("TORIF_CODE_S")
                        ITAKU_NAME = OraReader.GetString("ITAKU_NNAME_T")
                        MOTIKOMI_KBN = OraReader.GetString("MOTIKOMI_KBN_T")
                        KEKKA_HENKYAKU_KBN = OraReader.GetString("KEKKA_HENKYAKU_KBN_T")    '結果要否追加 2009.12.18
                        LIST_TORIS_CODE.Add(TORIS_CODE)
                        LIST_TORIF_CODE.Add(TORIF_CODE)
                        LIST_HONBU_CODE.Add(OraReader.GetString("TSIT_NO_T"))
                        LIST_MOTIKOMI.Add(MOTIKOMI_KBN)

                        KFJMAIN051.lstSCHLIST.Rows.Add(TORIS_CODE & "-" & TORIF_CODE, ITAKU_NAME, FURI_CODE, KIGYO_CODE, FURI_DATE)

                        OraReader.NextRead()
                    End While
                End If
            End If

            If intCOUNT_SCH = 1 OrElse intCOUNT_SCH = 0 Then
                KFJMAIN051.lstSCHLIST.Rows.Clear()
            End If

            '検索したスケジュールが０件の場合、取引先マスタを検索し、存在する場合はSCHMASTエラー、存在しない場合はTORIMASTエラー
            If intCOUNT_SCH = 0 Then
                OraReader.Close()

                SQL.Length = 0
                SQL.AppendLine("SELECT * FROM TORIMAST WHERE ")
                SQL.AppendLine("FURI_CODE_T = '" & FURI_CODE & "' AND ")
                SQL.AppendLine("KIGYO_CODE_T = '" & KIGYO_CODE & "' ")
                SQL.AppendLine(" AND NS_KBN_T = '" & NS_KBN & "' ")     '20010.01.26 入出金区分も条件に。
                SQL.AppendLine("ORDER BY TORIS_CODE_T, TORIF_CODE_T")

                If OraReader.DataReader(SQL) = True Then

                    While OraReader.EOF = False
                        TORIS_CODE = OraReader.GetString("TORIS_CODE_T")
                        TORIF_CODE = OraReader.GetString("TORIF_CODE_T")
                        ITAKU_NAME = OraReader.GetString("ITAKU_NNAME_T")
                        MOTIKOMI_KBN = OraReader.GetString("MOTIKOMI_KBN_T")
                        KEKKA_HENKYAKU_KBN = OraReader.GetString("KEKKA_HENKYAKU_KBN_T")    '結果要否追加 2009.12.18

                        OraReader.NextRead()

                    End While

                    Dim SCH_ERR As SCH_ERR
                    SCH_ERR.FURI = FURI_CODE
                    SCH_ERR.KIGYO = KIGYO_CODE
                    SCH_ERR.FURI_DATE = FURI_DATE
                    SCH_ERR.TORI_CODE = TORIS_CODE & "-" & TORIF_CODE
                    SCH_ERR.TORI_NAME = ITAKU_NAME

                    If MOTIKOMI_KBN = "1" Then

                        '**************************************
                        '2009.12.18 無い場合はスケジュール作成
                        '持込区分がセンター直接持込であれば
                        '**************************************
                        If InsertSchmast(TORIS_CODE, TORIF_CODE, FURI_DATE) = False Then
                            Return False
                        End If

                        SCH_ERR.MSG = "スケジュール自動作成"

                        'スケジュールを作ったので、スケジュールマスタ更新
                        If UpdateSCHMAST() = False Then
                            Return False
                        End If

                    Else
                        SCH_ERR.MSG = "未配信またはスケジュール未作成"
                    End If

                    LIST_SCH_ERR.Add(SCH_ERR)
                Else
                    Dim TORI_ERR As TORI_ERR
                    TORI_ERR.FURI = FURI_CODE
                    TORI_ERR.KIGYO = KIGYO_CODE
                    TORI_ERR.FURI_DATE = FURI_DATE

                    LIST_TORI_ERR.Add(TORI_ERR)
                End If
            Else
                '検索したスケジュールが1件以上の場合、スケジュール選択画面を表示する
                If intCOUNT_SCH > 1 Then
                    KFJMAIN051.ShowDialog()

                    TORIS_CODE = LIST_TORIS_CODE(KFJMAIN051.lstSCHLIST.CurrentRow.Index)
                    TORIF_CODE = LIST_TORIF_CODE(KFJMAIN051.lstSCHLIST.CurrentRow.Index)
                    HONBU_CODE = LIST_HONBU_CODE(KFJMAIN051.lstSCHLIST.CurrentRow.Index)
                    MOTIKOMI_KBN = LIST_MOTIKOMI(KFJMAIN051.lstSCHLIST.CurrentRow.Index)
                End If
                If intCOUNT_SCH = 1 Then
                    TORIS_CODE = LIST_TORIS_CODE(0)
                    TORIF_CODE = LIST_TORIF_CODE(0)
                    'HONBU_CODE = HONBU_CODE(0) '暫定コメント化
                    HONBU_CODE = LIST_HONBU_CODE(0)
                    MOTIKOMI_KBN = LIST_MOTIKOMI(0)
                End If

                'スケジュールマスタ更新
                If UpdateSCHMAST() = False Then
                    Return False
                End If
            End If

            '日報マスタへの登録
            Return MergeNIPPOUMAST()

        Catch ex As Exception
            MainLOG.Write("スケジュール検索", "失敗", ex.Message)
            JobMessage = ex.Message
            Return False

        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        End Try
    End Function

    Private Function MergeNIPPOUMAST() As Boolean
        Try
            '振替済件数・金額、不能件数・金額から処理件数・金額を算出する
            Dim SYORI_KEN As Long = FURI_KEN + FUNOU_KEN
            Dim SYORI_KIN As Long = FURI_KIN + FUNOU_KIN

            Dim SQL As StringBuilder = New StringBuilder(512)
            SQL.AppendLine("MERGE INTO NIPPOUMAST")
            '日報マスタの検索
            SQL.AppendLine(" USING (SELECT")
            SQL.AppendLine(" " & SQ(FURI_DATE) & " FURI_DATE")
            SQL.AppendLine(", " & SQ(FURI_CODE) & " FURI_CODE")
            SQL.AppendLine(", " & SQ(KIGYO_CODE) & " KIGYO_CODE")
            SQL.AppendLine(" FROM DUAL)")
            SQL.AppendLine(" ON (FURI_DATE_D = FURI_DATE")
            SQL.AppendLine(" AND FURI_CODE_D = FURI_CODE")
            SQL.AppendLine(" AND KIGYO_CODE_D = KIGYO_CODE)")
            'UPDATE句
            SQL.AppendLine(" WHEN MATCHED THEN")
            SQL.AppendLine(" UPDATE SET")
            SQL.AppendLine("REC_KBN_D = " & SQ(RECORD_SYUBETU) & ",")
            SQL.AppendLine("SYORI_KEN_D = " & SYORI_KEN & ",")
            SQL.AppendLine("SYORI_KIN_D = " & SYORI_KIN & ",")
            SQL.AppendLine("FURI_KEN_D = " & FURI_KEN & ",")
            SQL.AppendLine("FURI_KIN_D = " & FURI_KIN & ",")
            SQL.AppendLine("FUNOU_KEN_D = " & FUNOU_KEN & ",")
            SQL.AppendLine("FUNOU_KIN_D = " & FUNOU_KIN & ",")
            SQL.AppendLine("KOUSIN_DATE_D = " & SQ(DateTime.Today.ToString("yyyyMMdd")))
            'INSERT句
            SQL.AppendLine(" WHEN NOT MATCHED THEN")
            SQL.AppendLine(" INSERT VALUES(")
            SQL.AppendLine(SQ(RECORD_SYUBETU) & ",")
            SQL.AppendLine(SQ(FURI_DATE) & ",")
            SQL.AppendLine(SQ(FURI_CODE) & ",")
            SQL.AppendLine(SQ(KIGYO_CODE) & ",")
            SQL.AppendLine(SYORI_KEN & ",")
            SQL.AppendLine(SYORI_KIN & ",")
            SQL.AppendLine(FURI_KEN & ",")
            SQL.AppendLine(FURI_KIN & ",")
            SQL.AppendLine(FUNOU_KEN & ",")
            SQL.AppendLine(FUNOU_KIN & ",")
            SQL.AppendLine(SQ(DateTime.Today.ToString("yyyyMMdd")) & ",")
            SQL.AppendLine("'        ')")

            MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("日報マスタ更新", "失敗", ex.Message)
            JobMessage = "日報マスタの更新処理に失敗しました。"

            '*** Str Add 2015/12/01 SO)荒木 for 日報マスタ更新例外時に正常終了しているのを修正（潜在） ***
            Return False
            '*** Str Add 2015/12/01 SO)荒木 for 日報マスタ更新例外時の正常終了しているのを修正（潜在） ***

        End Try

        Return True
    End Function

    Private Function UpdateSCHMAST() As Boolean
        Try
            'INIファイルに本部コードが設定されていなかった場合、全支店更新対象
            'INIファイルに本部コードが設定されていた場合、本部のみ更新対象
            If (NIPPOU_HONBU = "") OrElse NIPPOU_HONBU = HONBU_CODE Then
                '振替済件数・金額、不能件数・金額から処理件数・金額を算出する
                Dim SYORI_KEN As Long = FURI_KEN + FUNOU_KEN
                Dim SYORI_KIN As Long = FURI_KIN + FUNOU_KIN

                Dim SQL As StringBuilder = New StringBuilder(128)
                SQL.AppendLine("UPDATE SCHMAST SET ")
                SQL.AppendLine("FURI_KEN_S = " & FURI_KEN & ",")
                SQL.AppendLine("FURI_KIN_S = " & FURI_KIN & ",")
                SQL.AppendLine("FUNOU_KEN_S = " & FUNOU_KEN & ",")
                SQL.AppendLine("FUNOU_KIN_S = " & FUNOU_KIN & ",")
                If MOTIKOMI_KBN <> "0" Then
                    SQL.AppendLine("SYORI_KEN_S = " & SYORI_KEN & ",")
                    SQL.AppendLine("SYORI_KIN_S = " & SYORI_KIN & ",")
                End If
                SQL.AppendLine("NIPPO_FLG_S = " & CInt(RECORD_SYUBETU))
                SQL.AppendLine(",TOUROKU_FLG_S = '1'")  '追加
                SQL.AppendLine(",HAISIN_FLG_S = '1'")  '追加
                SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(TORIS_CODE))
                SQL.AppendLine(" AND TORIF_CODE_S = " & SQ(TORIF_CODE))
                SQL.AppendLine(" AND FURI_DATE_S = " & SQ(FURI_DATE))
                SQL.AppendLine(" AND FURI_CODE_S = " & SQ(FURI_CODE))
                SQL.AppendLine(" AND KIGYO_CODE_S = " & SQ(KIGYO_CODE))

                MainDB.ExecuteNonQuery(SQL)

                '***************************************
                '2009.12.18 不能フラグを更新
                '（センター直持でレコード種別が０２のとき）
                '***************************************
                '2016/10/06 タスク）西野 CHG 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- START
                '東京センタ：翌日指定分は01
                If (CENTER_CODE = "3" AndAlso MOTIKOMI_KBN = "1" AndAlso RECORD_SYUBETU = "01") OrElse _
                   (CENTER_CODE <> "3" AndAlso MOTIKOMI_KBN = "1" AndAlso RECORD_SYUBETU = "02") Then
                    'If MOTIKOMI_KBN = "1" AndAlso RECORD_SYUBETU = "02" Then
                    '2016/10/06 タスク）西野 CHG 【PG】青梅信金 カスタマイズ対応(UI_12-13) -------------------- END
                    SQL.Length = 0

                    SQL.AppendLine("UPDATE SCHMAST SET ")
                    SQL.AppendLine("FUNOU_FLG_S = '1'")
                    'If KEKKA_HENKYAKU_KBN = "0" Then
                    '    SQL.AppendLine(",HENKAN_FLG_S = '1'")
                    '    SQL.AppendLine(",HENKAN_DATE_S = '" & DateTime.Today.ToString("yyyyMMdd") & "'")
                    'End If
                    SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(TORIS_CODE))
                    SQL.AppendLine(" AND TORIF_CODE_S = " & SQ(TORIF_CODE))
                    SQL.AppendLine(" AND FURI_DATE_S = " & SQ(FURI_DATE))
                    SQL.AppendLine(" AND FURI_CODE_S = " & SQ(FURI_CODE))
                    SQL.AppendLine(" AND KIGYO_CODE_S = " & SQ(KIGYO_CODE))

                    MainDB.ExecuteNonQuery(SQL)
                End If

            End If

        Catch ex As Exception
            MainLOG.Write("スケジュールマスタ更新", "失敗", ex.Message)
            JobMessage = "スケジュールの更新処理に失敗しました。"
            Return False
        End Try

        Return True
    End Function

    Private Function GetNS_KBN(ByVal FuriSyubetu As String) As String
        If NTC = "1" Then   'NTC接続の場合
            If FuriSyubetu = "111" Then    '振替種別=111（出金）、110（入金）
                Return "9"
            Else
                Return "1"
            End If
        Else
            If FuriSyubetu = "011" Then    '振替種別=011（出金）、010（入金）
                Return "9"
            Else
                Return "1"
            End If
        End If
    End Function

    Private Function DQ(ByVal value As String) As String
        Return """" & value & """"
    End Function

    Private Function InsertSchmast(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String) As Boolean

        Dim CLS As New ClsSchduleMaintenanceClass

        Dim SCHData As ClsSchduleMaintenanceClass.SCHMAST_Data

        Try

            GCom = New MenteCommon.clsCommon
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            GCom.Oracle = MainDB
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            '休日情報の蓄積
            Call CLS.SetKyuzituInformation()

            'SCHMAST項目名の蓄積
            Call CLS.SetSchMastInformation()

            '取引先マスタに取引先コードが存在することを確認
            Dim BRet As Boolean
            BRet = CLS.GET_SELECT_TORIMAST(Nothing, _
                        TorisCode, TorifCode, ClsSchduleMaintenanceClass.OPT.OptionNothing)
            If Not BRet Then
                Return False
            End If

            CLS.SCH.KFURI_DATE = FuriDate
            CLS.SCH.FURI_DATE = FuriDate

            If CLS.INSERT_NEW_SCHMAST(0, False, True) = True Then '各種予定日計算のみ
                SCHData = CLS.SCH
            Else
                Return False
            End If

            Dim sql As New StringBuilder(1024)

            sql.Append("INSERT INTO SCHMAST (")
            sql.Append("  FSYORI_KBN_S")
            sql.Append(", TORIS_CODE_S")
            sql.Append(", TORIF_CODE_S")
            sql.Append(", FURI_DATE_S")
            sql.Append(", KFURI_DATE_S")
            sql.Append(", SAIFURI_DATE_S")
            sql.Append(", KSAIFURI_DATE_S")
            sql.Append(", FURI_CODE_S")
            sql.Append(", KIGYO_CODE_S")
            sql.Append(", ITAKU_CODE_S")
            sql.Append(", TKIN_NO_S")
            sql.Append(", TSIT_NO_S")
            sql.Append(", SOUSIN_KBN_S")
            sql.Append(", MOTIKOMI_KBN_S")
            sql.Append(", BAITAI_CODE_S")
            sql.Append(", MOTIKOMI_SEQ_S")
            sql.Append(", FILE_SEQ_S")
            sql.Append(", TESUU_KBN_S")
            sql.Append(", IRAISYO_DATE_S")
            sql.Append(", IRAISYOK_YDATE_S")
            sql.Append(", MOTIKOMI_DATE_S")
            sql.Append(", UKETUKE_DATE_S")
            sql.Append(", TOUROKU_DATE_S")
            sql.Append(", HAISIN_YDATE_S")
            sql.Append(", HAISIN_DATE_S")
            sql.Append(", SOUSIN_YDATE_S")
            sql.Append(", SOUSIN_DATE_S")
            sql.Append(", FUNOU_YDATE_S")
            sql.Append(", FUNOU_DATE_S")
            sql.Append(", KESSAI_YDATE_S")
            sql.Append(", KESSAI_DATE_S")
            sql.Append(", TESUU_YDATE_S")
            sql.Append(", TESUU_DATE_S")
            sql.Append(", HENKAN_YDATE_S")
            sql.Append(", HENKAN_DATE_S")
            sql.Append(", UKETORI_DATE_S")
            sql.Append(", UKETUKE_FLG_S")
            sql.Append(", TOUROKU_FLG_S")
            sql.Append(", HAISIN_FLG_S")
            sql.Append(", SAIFURI_FLG_S")
            sql.Append(", SOUSIN_FLG_S")
            sql.Append(", FUNOU_FLG_S")
            sql.Append(", TESUUKEI_FLG_S")
            sql.Append(", TESUUTYO_FLG_S")
            sql.Append(", KESSAI_FLG_S")
            sql.Append(", HENKAN_FLG_S")
            sql.Append(", TYUUDAN_FLG_S")
            sql.Append(", TAKOU_FLG_S")
            sql.Append(", NIPPO_FLG_S")
            sql.Append(", ERROR_INF_S")
            sql.Append(", SYORI_KEN_S")
            sql.Append(", SYORI_KIN_S")
            sql.Append(", ERR_KEN_S")
            sql.Append(", ERR_KIN_S")
            sql.Append(", TESUU_KIN_S")
            sql.Append(", TESUU_KIN1_S")
            sql.Append(", TESUU_KIN2_S")
            sql.Append(", TESUU_KIN3_S")
            sql.Append(", FURI_KEN_S")
            sql.Append(", FURI_KIN_S")
            sql.Append(", FUNOU_KEN_S")
            sql.Append(", FUNOU_KIN_S")
            sql.Append(", UFILE_NAME_S")
            sql.Append(", SFILE_NAME_S")
            sql.Append(", SAKUSEI_DATE_S")
            sql.Append(", JIFURI_TIME_STAMP_S")
            sql.Append(", KESSAI_TIME_STAMP_S")
            sql.Append(", TESUU_TIME_STAMP_S")
            sql.Append(", YOBI1_S")
            sql.Append(", YOBI2_S")
            sql.Append(", YOBI3_S")
            sql.Append(", YOBI4_S")
            sql.Append(", YOBI5_S")
            sql.Append(", YOBI6_S")
            sql.Append(", YOBI7_S")
            sql.Append(", YOBI8_S")
            sql.Append(", YOBI9_S")
            sql.Append(", YOBI10_S")
            sql.Append(") ")
            sql.Append("VALUES (")
            sql.Append("  '1'")                                         'FSYORI_KBN_S
            sql.Append(", '" & TorisCode & "'")                         'TORIS_CODE_S
            sql.Append(", '" & TorifCode & "'")                         'TORIF_CODE_S
            sql.Append(", '" & FuriDate & "'")                          'FURI_DATE_S
            sql.Append(", '" & FuriDate & "'")                          'KFURI_DATE_S
            sql.Append(", '00000000'")                                  'SAIFURI_DATE_S
            sql.Append(", '00000000'")                                  'KSAIFURI_DATE_S
            sql.Append(", '" & CLS.TR(0).FURI_CODE & "'")               'FURI_CODE_S
            sql.Append(", '" & CLS.TR(0).KIGYO_CODE & "'")              'KIGYO_CODE_S
            sql.Append(", '" & CLS.TR(0).ITAKU_CODE & "'")              'ITAKU_CODE_S
            sql.Append(", '" & CLS.TR(0).TKIN_NO & "'")                 'TKIN_NO_S
            sql.Append(", '" & CLS.TR(0).TSIT_NO & "'")                 'TSIT_NO_S
            sql.Append(", '" & CLS.TR(0).SOUSIN_KBN & "'")              'SOUSIN_KBN_S
            sql.Append(", '" & CLS.TR(0).MOTIKOMI_KBN & "'")            'MOTIKOMI_KBN_S
            sql.Append(", '" & CLS.TR(0).BAITAI_CODE & "'")             'BAITAI_CODE_S
            sql.Append(", " & SCHData.MOTIKOMI_SEQ)                     'MOTIKOMI_SEQ_S
            sql.Append(", 0")                                           'FILE_SEQ_S
            Select Case CLS.TR(0).TESUUTYO_KBN
                Case 0
                    '手数料徴求区分 = 都度徴求
                    sql.Append(", '1'")                                 'TESUU_KBN_S
                Case 1, 3
                    '手数料徴求区分 = 一括徴求
                    Select Case CLS.TR(0).MONTH_FLG
                        Case 1, 3
                            sql.Append(", '2'")                         'TESUU_KBN_S
                        Case Else
                            sql.Append(", '3'")                         'TESUU_KBN_S
                    End Select
                Case Else
                    '手数料徴求区分 = (2)特別免除, (3)別途徴求
                    sql.Append(", '4'")                                 'TESUU_KBN_S
            End Select
            sql.Append(", '00000000'")                                  'IRAISYO_DATE_S
            sql.Append(", '" & SCHData.IRAISYOK_YDATE & "'")            'IRAISYOK_YDATE_S
            sql.Append(", '" & SCHData.MOTIKOMI_DATE & "'")             'MOTIKOMI_DATE_S
            sql.Append(", '00000000'")                                  'UKETUKE_DATE_S
            sql.Append(", '00000000'")                                  'TOUROKU_DATE_S
            sql.Append(", '" & SCHData.HAISIN_YDATE & "'")              'HAISIN_YDATE_S
            sql.Append(", '00000000'")                                  'HAISIN_DATE_S
            sql.Append(", '" & SCHData.HAISIN_YDATE & "'")              'SOUSIN_YDATE_S
            sql.Append(", '00000000'")                                  'SOUSIN_DATE_S
            sql.Append(", '" & SCHData.FUNOU_YDATE & "'")               'FUNOU_YDATE_S
            sql.Append(", '00000000'")                                  'FUNOU_DATE_S
            sql.Append(", '" & SCHData.KESSAI_YDATE & "'")              'KESSAI_YDATE_S
            sql.Append(", '00000000'")                                  'KESSAI_DATE_S
            sql.Append(", '" & SCHData.TESUU_YDATE & "'")               'TESUU_YDATE_S
            sql.Append(", '00000000'")                                  'TESUU_DATE_S
            sql.Append(", '" & SCHData.HENKAN_YDATE & "'")              'HENKAN_YDATE_S
            sql.Append(", '00000000'")                                  'HENKAN_DATE_S
            sql.Append(", '00000000'")                                  'UKETORI_DATE_S
            sql.Append(", '0'")                                         'UKETUKE_FLG_S
            sql.Append(", '0'")                                         'TOUROKU_FLG_S
            sql.Append(", '0'")                                         'HAISIN_FLG_S
            sql.Append(", '0'")                                         'SAIFURI_FLG_S
            sql.Append(", '0'")                                         'SOUSIN_FLG_S
            sql.Append(", '0'")                                         'FUNOU_FLG_S
            sql.Append(", '0'")                                         'TESUUKEI_FLG_S
            sql.Append(", '0'")                                         'TESUUTYO_FLG_S
            sql.Append(", '0'")                                         'KESSAI_FLG_S
            sql.Append(", '0'")                                         'HENKAN_FLG_S
            sql.Append(", '0'")                                         'TYUUDAN_FLG_S
            sql.Append(", '0'")                                         'TAKOU_FLG_S
            sql.Append(", '0'")                                         'NIPPO_FLG_S
            sql.Append(", NULL")                                        'ERROR_INF_S
            sql.Append(", 0")                                           'SYORI_KEN_S
            sql.Append(", 0")                                           'SYORI_KIN_S
            sql.Append(", NULL")                                        'ERR_KEN_S
            sql.Append(", NULL")                                        'ERR_KIN_S
            sql.Append(", 0")                                           'TESUU_KIN_S
            sql.Append(", 0")                                           'TESUU_KIN1_S
            sql.Append(", 0")                                           'TESUU_KIN2_S
            sql.Append(", 0")                                           'TESUU_KIN3_S
            sql.Append(", 0")                                           'FURI_KEN_S
            sql.Append(", 0")                                           'FURI_KIN_S
            sql.Append(", 0")                                           'FUNOU_KEN_S
            sql.Append(", 0")                                           'FUNOU_KIN_S
            sql.Append(", NULL")                                        'UFILE_NAME_S
            sql.Append(", NULL")                                        'SFILE_NAME_S
            sql.Append(", TO_CHAR(SYSDATE,'yyyymmdd')")                 'SAKUSEI_DATE_S
            sql.Append(", '00000000000000'")                            'JIFURI_TIME_STAMP_S
            sql.Append(", '00000000000000'")                            'KESSAI_TIME_STAMP_S
            sql.Append(", '00000000000000'")                            'TESUU_TIME_STAMP_S
            sql.Append(", NULL")                                        'YOBI1_S
            sql.Append(", NULL")                                        'YOBI2_S
            sql.Append(", NULL")                                        'YOBI3_S
            sql.Append(", NULL")                                        'YOBI4_S
            sql.Append(", NULL")                                        'YOBI5_S
            sql.Append(", NULL")                                        'YOBI6_S
            sql.Append(", NULL")                                        'YOBI7_S
            sql.Append(", NULL")                                        'YOBI8_S
            sql.Append(", NULL")                                        'YOBI9_S
            sql.Append(", NULL")                                        'YOBI10_S
            sql.Append(")")

            MainDB.ExecuteNonQuery(sql)

            If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                Dim ReturnMessage As String = String.Empty
                Dim SubMastInsert_Ret As Integer = 0
                Call CAstExternal.ModExternal.Ex_InsertSchmastSub("1", _
                                                                  TorisCode, _
                                                                  TorifCode, _
                                                                  FuriDate, _
                                                                  SCHData.MOTIKOMI_SEQ, _
                                                                  ReturnMessage, _
                                                                  MainDB)
                MainLOG.Write("スケジュールマスタ(追加情報)", "登録", "結果メッセージ：" & ReturnMessage)
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("スケジュールマスタ登録", "失敗", ex.Message)
            JobMessage = "スケジュールの新規登録処理に失敗しました。"
            Return False
        End Try
    End Function

End Module
