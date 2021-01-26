Option Strict On

Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CASTCommon.ModPublic
Imports System.Globalization
Imports CASTCommon

Public Class ClsJifkeiCreate

    Public MainLOG As New CASTCommon.BatchLOG("KFK090", "自振契約リエンタ作成")

    Dim MainDB As CASTCommon.MyOracle

    'Private clsFUSION As New clsFUSION.clsMain

    Private strKEKKA As String              ' データ作成結果

    Private jobMessage As String = ""          ' ジョブ監視メッセージ

    ' 処理日付
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ''' <summary>
    ''' iniファイル情報
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni

        Dim JIKINKO_CODE As String       '自金庫コード
        Dim JIKINKO_NAME As String       '自金庫名
        Dim HONBU_CODE As String         '本部コード
        Dim RIENTA_PATH As String        'リエンタファイル作成先
        Dim DAT_PATH As String           'DATのパス
        Dim JIF_RIENTA_FILENAME As String   'リエンタファイル名
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
        Dim RSV2_EDITION As String        ' RSV2機能設定
        Dim COMMON_BAITAIWRITE As String  ' 媒体書込用フォルダ
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' 自振契約マスタのキー項目＋リエンタファイル名
    ''' </summary>
    ''' <remarks></remarks>
    Structure JifmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' 初期化
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private key As JifmastKey

    Dim paraSyoriDate As String        'パラメータから引き継いだ処理日

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 自振契約リエンタ作成初期処理
    ''' </summary>
    ''' <returns>正常:True 異常:False</returns>
    ''' <remarks></remarks>
    Public Function JikeiInit(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try
            'パラメータの読込
            param = CmdArgs(0).Split(","c)
            If param.Length = 2 Then

                'ログ書込み情報の設定
                MainLOG.FuriDate = param(0)                     '処理日セット
                MainLOG.JobTuuban = CType(param(1), Integer)
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(初期処理)開始", "成功")


                paraSyoriDate = param(0)                       '処理日をセット

            Else
                MainLOG.Write("(初期処理)開始", "失敗", "コマンドライン引数のパラメータが不正です")

                Return False

            End If

            'iniファイルの読込
            If IniRead() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("(初期処理)開始", "失敗", ex.Message)

            Return False
        Finally
            MainLOG.Write("(初期処理)終了", "成功")
        End Try

    End Function

    Private Function IniRead() As Boolean

        ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")           '自金庫コード
        If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
            jobMessage = "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD"
            Return False
        End If

        ini_info.JIKINKO_NAME = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")       '自金庫名
        If ini_info.JIKINKO_NAME = "err" OrElse ini_info.JIKINKO_NAME = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME")
            jobMessage = "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME"
            Return False
        End If

        ini_info.HONBU_CODE = CASTCommon.GetFSKJIni("COMMON", "HONBUCD")         '本部コード
        If ini_info.HONBU_CODE = "err" OrElse ini_info.HONBU_CODE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD")
            jobMessage = "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD"
            Return False
        End If

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        'リエンタファイル作成先
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR")
            jobMessage = "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DATのパス
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            jobMessage = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
            Return False
        End If

        ini_info.JIF_RIENTA_FILENAME = CASTCommon.GetFSKJIni("COMMON", "JIKEI_RIENTAFILENAME")       'リエンタファイル名
        If ini_info.JIF_RIENTA_FILENAME = "err" OrElse ini_info.JIF_RIENTA_FILENAME = "" OrElse ini_info.JIF_RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自振契約リエンタファイル名 分類:COMMON 項目:JIKEI_RIENTAFILENAME")
            jobMessage = "設定ファイル取得失敗 項目名:自振契約リエンタファイル名 分類:COMMON 項目:JIKEI_RIENTAFILENAME"
            Return False
        End If

        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
        ini_info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If ini_info.RSV2_EDITION = "err" OrElse ini_info.RSV2_EDITION = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
            jobMessage = "設定ファイル取得失敗 項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION"
            Return False
        End If

        ini_info.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
        If ini_info.COMMON_BAITAIWRITE = "err" OrElse ini_info.COMMON_BAITAIWRITE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:媒体書込用フォルダ 分類:COMMON 項目:BAITAIWRITE")
            jobMessage = "設定ファイル取得失敗 項目名:媒体書込用フォルダ 分類:COMMON 項目:BAITAIWRITE"
            Return False
        End If
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

        Return True

    End Function

    ' 機能　 ： 自振契約リエンタ作成処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Public Function Main(ByVal command As String) As Integer

        MainDB = New CASTCommon.MyOracle
        Dim bRet As Boolean = True
        Dim iRet As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        ' パラメータチェック
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "自振契約リエンタ作成処理(開始)", "成功")
            'MainLOG.Write("(主処理)開始", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            '*******************************
            ' 回次を取得
            '*******************************
            If GetKaiji() = False Then
                MainLOG.Write("(主処理)", "失敗", "回次の取得に失敗しました")

                Return -1
            End If

            '*****************************************
            ' 自振契約データを格納とスケジュールの更新
            '*****************************************
            Dim aryJikei As New ArrayList      '自振契約データ格納用
            iRet = MakeJikeiData(aryJikei)
            Select Case iRet
                Case 0          ' データ格納成功
                    bRet = True
                Case 1          ' 対象データ０件
                    bRet = True
                Case Else       ' データ格納失敗
                    bRet = False
            End Select

            '********************************************
            '自振契約テーブルを削除
            '********************************************
            If iRet = 0 Then bRet = DELETE_STORE_JIFKEIYAKU()

            '***********************
            ' リエンタFD作成
            '***********************
            Dim totalRow As Integer = aryJikei.Count()
            Dim msgtitle As String = "自振契約リエンタ作成(KFJ090)"

            If iRet = 0 AndAlso aryJikei.Count() > 0 Then

                If MakeRientaFD(aryJikei) = False Then
                    jobMessage = "自振契約リエンタ作成失敗"
                    iRet = -1
                Else
                    ' 2016/01/18 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                    'Do
                    '    Try
                    '        '2012/01/13 saitou 標準修正 MODIFY ------------------------------------->>>>
                    '        'リエンタ作成先をINIファイルで管理する
                    '        Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                    '        'Dim DirInfo As New DirectoryInfo("A:\")
                    '        '2012/01/13 saitou 標準修正 MODIFY -------------------------------------<<<<
                    '        Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                    '        iRet = 0
                    '        Exit Do

                    '    Catch ex As Exception
                    '        '2012/01/13 saitou 標準修正 MODIFY ------------------------------------->>>>
                    '        'リエンタ作成先をINIファイルで管理する
                    '        If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                    '            iRet = -1
                    '            jobMessage = "FD挿入がキャンセルされました。"
                    '            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                    '            MainLOG.Write_LEVEL1("", "FD挿入がキャンセル")
                    '            'MainLOG.Write("", "FD挿入がキャンセル")
                    '            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

                    '            Exit Do
                    '        End If
                    '        'If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot("A:\")), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                    '        '    iRet = -1
                    '        '    jobMessage = "FD挿入がキャンセルされました。"
                    '        '    MainLOG.Write("", "FD挿入がキャンセル")
                    '        '    Exit Do
                    '        'End If
                    '        '2012/01/13 saitou 標準修正 MODIFY -------------------------------------<<<<
                    '    End Try
                    'Loop

                    'Select Case iRet
                    '    Case 0          ' データ格納成功
                    '        '2012/01/13 saitou 標準修正 MODIFY ------------------------------------->>>>
                    '        'リエンタ作成先をINIファイルで管理する
                    '        If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME)) Then
                    '            '2014/05/01 saitou 標準版修正 MODIFY ----------------------------------------------->>>>
                    '            'メッセージボックスを最前面に出す
                    '            If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                    '                jobMessage = "フロッピー内ファイル削除キャンセル"
                    '                iRet = -1
                    '            End If
                    '            'If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
                    '            '    jobMessage = "フロッピー内ファイル削除キャンセル"
                    '            '    iRet = -1
                    '            'End If
                    '            '2014/05/01 saitou 標準版修正 MODIFY -----------------------------------------------<<<<
                    '        End If
                    '        'If File.Exists(Path.Combine("A:\", ini_info.JIF_RIENTA_FILENAME)) Then
                    '        '    If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
                    '        '        jobMessage = "フロッピー内ファイル削除キャンセル"
                    '        '        iRet = -1
                    '        '    End If
                    '        'End If
                    '        '2012/01/13 saitou 標準修正 MODIFY -------------------------------------<<<<

                    '        If iRet = 0 Then
                    '            '2012/01/13 saitou 標準修正 MODIFY ------------------------------------->>>>
                    '            'リエンタ作成先をINIファイルで管理する
                    '            File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME), True)
                    '            'File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine("A:\", ini_info.JIF_RIENTA_FILENAME), True)
                    '            '2012/01/13 saitou 標準修正 MODIFY -------------------------------------<<<<
                    '        End If
                    'End Select
                    Select Case ini_info.RSV2_EDITION
                        Case "2"
                            '---------------------------------------------------------------
                            ' ファイル名構築
                            '  [ファイル名] RNT_JR_yyyyMMdd_HHmmss_1(1固定)
                            '---------------------------------------------------------------
                            Dim RientFileName As String = "RNT_JR_" & strDate & "_" & strTime & "_1"

                            '---------------------------------------------------------------
                            ' ファイルコピー
                            '---------------------------------------------------------------
                            If File.Exists(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName)) Then
                                File.Delete(Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName))
                            End If
                            File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine(ini_info.COMMON_BAITAIWRITE, RientFileName), True)
                        Case Else
                            Do
                                Try
                                    Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                                    Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                                    iRet = 0
                                    Exit Do

                                Catch ex As Exception
                                    If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                                        iRet = -1
                                        jobMessage = "FD挿入がキャンセルされました。"
                                        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                                        MainLOG.Write_LEVEL1("", "FD挿入がキャンセル")
                                        'MainLOG.Write("", "FD挿入がキャンセル")
                                        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

                                        Exit Do
                                    End If
                                End Try
                            Loop

                            Select Case iRet
                                Case 0          ' データ格納成功
                                    If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME)) Then
                                        If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                            jobMessage = "フロッピー内ファイル削除キャンセル"
                                            iRet = -1
                                        End If
                                    End If

                                    If iRet = 0 Then
                                        File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), Path.Combine(ini_info.RIENTA_PATH, ini_info.JIF_RIENTA_FILENAME), True)
                                    End If
                            End Select

                    End Select
                End If

            End If

            If iRet <> 0 Then
                bRet = False
            End If

            '*******************************
            ' 帳票出力
            '*******************************
            ' 自振契約データが１件以上存在する場合、帳票出力
            If iRet = 0 Then
                ' 帳票出力
                'LOG.Write("帳票出力開始", "成功")

                '' 処理結果確認表
                Dim PrnSyoKekka As ClsKFJP043 = Nothing
                Dim intPrnRet As Integer

                PrnSyoKekka = New ClsKFJP043


                PrnSyoKekka.OraDB = MainDB
                ' 自振契約処理結果確認表　タイトル行出力
                PrnSyoKekka.CreateCsvFile()

                ' 自振契約処理結果確認表　明細行出力
                intPrnRet = PrnSyoKekka.OutputCSVKekka(aryJikei, ini_info.JIKINKO_CODE, strDate, strTime)

                If intPrnRet <> 0 Then
                    bRet = False
                    MainLOG.Write("処理結果確認表(自振契約)出力", "失敗", "処理結果確認表(自振契約)ＣＳＶ出力に失敗しました。")
                End If

                ' 自振契約処理結果確認表
                If Not PrnSyoKekka Is Nothing And intPrnRet = 0 Then
                    PrnSyoKekka.CloseCsv()

                    '印刷バッチ呼び出し
                    Dim ExeRepo As New CAstReports.ClsExecute
                    Dim param As String = ""
                    Dim nret As Integer

                    'パラメータ設定：ログイン名、ＣＳＶファイル名
                    param = MainLOG.UserID & "," & PrnSyoKekka.FileName

                    nret = ExeRepo.ExecReport("KFJP043.EXE", param)

                    If nret <> 0 Then
                        '印刷失敗：戻り値に対応したエラーメッセージを表示する
                        Select Case nret
                            Case -1
                                jobMessage = "処理結果確認表(自振契約)印刷対象０件。"

                            Case Else

                                jobMessage = "処理結果確認表(自振契約)印刷失敗。エラーコード：" & nret
                        End Select
                        MainLOG.Write("処理結果確認表(自振契約)印刷", "失敗", jobMessage)
                        bRet = False
                    End If
                End If
            End If

            If bRet = False Then

                If jobMessage = "" Then
                    Call MainLOG.UpdateJOBMASTbyErr("ログ参照")
                Else
                    Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                End If

                ' ロールバック
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "対象データ０件"
                End If

                Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                ' コミット
                MainDB.Commit()
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            MainLOG.Write("(主処理)", "失敗", ex.ToString)
            Return 1
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "自振契約リエンタ作成処理(終了)", "成功")
            'MainLOG.Write("(主処理)終了", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        End Try

        Return 0

    End Function

    ' 機能　 ： 自振契約データ作成処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '

    Private Function MakeJikeiData(ByRef aryJifKei As ArrayList) As Integer

        Dim OraReader As CASTCommon.MyOracleReader
        Dim SQL As StringBuilder
        Dim Jdata As CAstFormKes.ClsFormKes.JifkeiData
        Dim fmt10004 As CAstFormKes.ClsFormSikinFuri.T_10004

        Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
        culture.DateTimeFormat.Calendar = New JapaneseCalendar()
        Dim target As DateTime
        Dim result As String
        Dim Cnt As Integer = 0

        OraReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)


        Try
            MainLOG.Write("(自振契約データ格納)開始", "成功")

            '***********************
            ' 契約日を和暦に
            '***********************
            target = Date.Parse(Format(CInt(strDate), "0000/00/00"))
            result = target.ToString("yyMMdd", culture)

            SQL.Append("SELECT")
            SQL.Append(" KEIYAKU_SIT_JK")
            SQL.Append(",KEIYAKU_KAMOKU_JK")
            SQL.Append(",KEIYAKU_KOUZA_JK")
            SQL.Append(",KEIYAKU_KNAME_JK")
            SQL.Append(",FURI_CODE_JK")
            SQL.Append(",KIGYO_CODE_JK")
            SQL.Append(",TORIS_CODE_JK")
            SQL.Append(",TORIF_CODE_JK")
            SQL.Append(",FURI_DATE_JK")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",RECORD_NO_JK")
            SQL.Append(" FROM STORE_JIFKEIYAKU,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" TORIS_CODE_JK = TORIS_CODE_T ")
            SQL.Append(" AND TORIF_CODE_JK = TORIF_CODE_T ")
            SQL.Append(" ORDER BY FURI_CODE_JK,KIGYO_CODE_JK,KEIYAKU_SIT_JK,KEIYAKU_KAMOKU_JK,KEIYAKU_KOUZA_JK ")

            If OraReader.DataReader(SQL) = True Then

                Do Until OraReader.EOF

                    Cnt += 1

                    Jdata = New CAstFormKes.ClsFormKes.JifkeiData
                    fmt10004 = New CAstFormKes.ClsFormSikinFuri.T_10004
                    Jdata.Init()
                    fmt10004.Init()

                    ' データ設定
                    With fmt10004
                        .KAMOKU_KOUZA_NO = String.Concat(OraReader.GetString("KEIYAKU_KAMOKU_JK"), OraReader.GetString("KEIYAKU_KOUZA_JK"))        ' 科目口座番号
                        .GYO = "01"                                           ' 行
                        .JIFURI_CODE = OraReader.GetString("FURI_CODE_JK")    ' 振替コード
                        .KIGYO_CODE = OraReader.GetString("KIGYO_CODE_JK")    ' 企業コード
                        .KEIYAKU_DATE = result                                ' 契約日(和暦)
                        .KOUFURIUKETUKE = "".PadLeft(1, " "c)                 ' 口振受付サービス
                        '****20120709 mubuchi 但馬信金修正******************************************>>>>
                        '本部コードと契約者支店コードが同じだった場合は[原点番号]には空白をセットする。
                        If ini_info.HONBU_CODE = OraReader.GetString("KEIYAKU_SIT_JK") Then
                            .GENTEN_NO = "".PadLeft(3, " "c)    ' 原点番号
                        Else
                            .GENTEN_NO = OraReader.GetString("KEIYAKU_SIT_JK")    ' 原点番号
                        End If
                        '****20120709 mubuchi 但馬信金修正******************************************<<<<<
                        .YOBI1 = ""                                           ' 予備１
                    End With

                    ' データ設定
                    With Jdata
                        .SyoriDate = paraSyoriDate                            '決済日
                        .TorisCode = OraReader.GetString("TORIS_CODE_JK")     '取引先主コード
                        .TorifCode = OraReader.GetString("TORIF_CODE_JK")     '取引先副コード
                        .FuriDate = OraReader.GetString("FURI_DATE_JK")    '振替日
                        .ToriNName = OraReader.GetString("ITAKU_NNAME_T")     '取引先名
                        .FuriCode = OraReader.GetString("FURI_CODE_JK")       '振替コード
                        .KigyoCode = OraReader.GetString("KIGYO_CODE_JK")     '企業コード
                        .KeiyakuKname = OraReader.GetString("KEIYAKU_KNAME_JK") '契約者名カナ
                        .MeiRecordNo = OraReader.GetString("RECORD_NO_JK").ToString
                    End With

                    Jdata.record320 = fmt10004.Data
                    Jdata.OpeCode = String.Concat(fmt10004.KAMOKU_CODE, fmt10004.OPE_CODE)


                    ' 固定長に変換する
                    Jdata.Data = Jdata.Data

                    aryJifKei.Add(Jdata)

                    OraReader.NextRead()
                Loop

            End If

            If aryJifKei.Count = 0 Then
                MainLOG.Write("(自振契約データ格納)", "失敗", "件数０件")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(自振契約データ格納)", "失敗", ex.Message)

            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            MainLOG.Write("(自振契約データ格納)終了", "成功")

        End Try

        Return 0

    End Function


    ' 機能　 ： 自振契約マスタ登録
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertJifMast(ByVal JData As CAstFormKes.ClsFormKes.JifkeiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Dim fmt10004 As New CAstFormKes.ClsFormSikinFuri.T_10004


        Try
            MainLOG.Write("(自振契約マスタ登録)開始", "成功")


            fmt10004.Init()
            fmt10004.Data = JData.record320

            SQL.Append("INSERT INTO JIKEIMAST(")
            SQL.Append(" SYORI_DATE_JR")
            SQL.Append(",TIME_STAMP_JR")
            SQL.Append(",KAIJI_JR")
            SQL.Append(",RECORD_NO_JR")
            SQL.Append(",FILE_NAME_JR")
            SQL.Append(",TORIS_CODE_JR")
            SQL.Append(",TORIF_CODE_JR")
            SQL.Append(",FURI_DATE_JR")
            SQL.Append(",MEI_RECORD_NO_JR")
            SQL.Append(",FURI_CODE_JR")
            SQL.Append(",KIGYO_CODE_JR")
            SQL.Append(",TSIT_NO_JR")
            SQL.Append(",KAMOKU_JR")
            SQL.Append(",KOUZA_JR")
            SQL.Append(",KAMOKU_CODE_JR")
            SQL.Append(",OPE_CODE_JR")
            SQL.Append(",DENBUN_ALL_JR")
            SQL.Append(",ERR_CODE_JR")
            SQL.Append(",ERR_MSG_JR")
            SQL.Append(",SAKUSEI_DATE_JR")
            SQL.Append(",KOUSIN_DATE_JR")
            SQL.Append(") VALUES (")
            SQL.Append(" " & SQ(strDate))                                   ' 処理日
            SQL.Append("," & SQ(String.Concat(strDate, strTime)))           ' タイムスタンプ
            SQL.Append("," & SQ(key.Kaiji))                              ' 回次
            SQL.Append("," & SQ(key.RecordNo))                           ' 通番
            SQL.Append("," & SQ(ini_info.JIF_RIENTA_FILENAME))                     ' リエンタファイル名
            SQL.Append("," & SQ(JData.TorisCode))                        ' 取引先主コード
            SQL.Append("," & SQ(JData.TorifCode))                        ' 取引先副コード
            SQL.Append("," & SQ(JData.FuriDate))                         ' 振替日
            SQL.Append("," & JData.MeiRecordNo.Trim)
            SQL.Append("," & SQ(JData.FuriCode))
            SQL.Append("," & SQ(JData.KigyoCode))
            SQL.Append("," & SQ(fmt10004.GENTEN_NO))
            SQL.Append("," & SQ(fmt10004.KAMOKU_KOUZA_NO.Substring(0, 2)))
            SQL.Append("," & SQ(fmt10004.KAMOKU_KOUZA_NO.Substring(2, 7)))
            SQL.Append("," & SQ(JData.OpeCode.Substring(0, 2)))           ' 科目コード
            SQL.Append("," & SQ(JData.OpeCode.Substring(2, 3)))           ' オペコード
            SQL.Append("," & SQ(JData.record320))                           ' 個別データ
            SQL.Append("," & SQ(""))                                        ' 結果コード
            SQL.Append("," & SQ(""))                                        ' エラーメッセージ
            SQL.Append("," & SQ(strDate))                                   ' 作成日
            SQL.Append("," & SQ(strDate))                                   ' 更新日
            SQL.Append(")")

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then Return False

        Catch ex As Exception
            MainLOG.Write("(自振契約マスタ登録)", "失敗", ex.Message)

            Return False
        Finally

            MainLOG.Write("(自振契約マスタ登録)開始", "成功")

        End Try

        Return True

    End Function

    Private Function DELETE_STORE_JIFKEIYAKU() As Boolean

        Dim SQL As New StringBuilder(128)

        Try
            MainLOG.Write("(自振契約テーブル削除)開始", "成功")


            SQL.Append("DELETE FROM STORE_JIFKEIYAKU")

            If MainDB.ExecuteNonQuery(SQL) <= 0 Then Return False

        Catch ex As Exception
            MainLOG.Write("(自振契約テーブル削除)", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write("(自振契約テーブル削除)開始", "成功")
        End Try

        Return True

    End Function

    Private Function MakeRientaFD(ByVal ary As ArrayList) As Boolean

        Dim T_RIENT77 As New CAstFormKes.ClsT_RIENT77
        Dim T_RIENT10 As New CAstFormKes.ClsT_RIENT10

        Dim Jdata As CAstFormKes.ClsFormKes.JifkeiData
        Dim EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")

        Dim T_10004 As New CAstFormKes.ClsFormSikinFuri.T_10004

        Dim StrmWrite As FileStream = Nothing

        Try

            ' タンキングヘッダ  
            ' 初期データ設定
            Call T_RIENT77.TANKING_HEAD.Init()
            ' Ｔ日付
            T_RIENT77.TANKING_HEAD.strT_HIDUKE = CASTCommon.Calendar.Now.ToString("yyyyMMdd")

            ' 店舗情報レコード（１店舗情報）
            ' 初期データ設定
            Call T_RIENT77.TENPO_INFOREC(0).Init()
            ' 金庫コード
            T_RIENT77.TENPO_INFOREC(0).strKINKO_CD = ini_info.JIKINKO_CODE
            ' 店舗コード
            T_RIENT77.TENPO_INFOREC(0).strSIT_CD = ini_info.HONBU_CODE

            ' 店舗情報レコード（２～３２店舗情報）
            ' 初期データ設定
            For i As Integer = 1 To T_RIENT77.TENPO_INFOREC.Length - 1
                ' ２～３２の初期データ設定
                Call T_RIENT77.TENPO_INFOREC(i).Init2_32()
            Next i

            ' 予備３
            ' 初期化
            Call T_RIENT77.DATA_SIKIBETU.Init()

            ' リエンタファイル オープン

            If File.Exists(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME)) = True Then
                ' 既に存在する場合は，削除
                File.Delete(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME))
            End If

            StrmWrite = New FileStream(Path.Combine(ini_info.DAT_PATH, ini_info.JIF_RIENTA_FILENAME), FileMode.OpenOrCreate, FileAccess.ReadWrite)

            Dim Bytes As Byte() = EncdJ.GetBytes(ini_info.JIF_RIENTA_FILENAME.PadRight(12, Nothing).PadRight(16, " "c))
            StrmWrite.Write(Bytes, 0, 16)

            ' タンキングヘッダ書込
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.Data, 0, 28)

            ' 店舗情報レコード書込
            For i As Integer = 0 To T_RIENT77.TENPO_INFOREC.Length - 1
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(i).Data, 0, 28)
            Next i

            ' 予備３書込
            StrmWrite.Write(T_RIENT77.DATA_SIKIBETU.Data, 0, 84)

            Dim WriteCount As Integer = 0           ' 書込件数

            Dim RecLen As Integer

            ' タンキングデータ書込

            Dim cnt As Integer = ary.Count - 1 'ループ回数

            For i As Integer = 0 To cnt

                ' タンキングデータ
                Jdata = CType(ary.Item(i), CAstFormKes.ClsFormKes.JifkeiData)

                Select Case Jdata.OpeCode
                    Case "10004"
                        T_RIENT10.TANKING_DATA.Init_10()
                        T_10004.DataSepaPlus = Jdata.record320
                        RecLen = T_10004.DataSepaPlus.Replace(" ", "").Length + 26
                        T_RIENT10.TANKING_DATA.strOPE_CODE = Jdata.OpeCode
                        T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Jdata.OpeCode.Substring(0, 2)
                        T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Jdata.OpeCode.Substring(2, 3)
                        T_RIENT10.TANKING_DATA.strTANKING_DATA = T_10004.DataSepaPlus
                        T_10004 = Nothing
                    Case Else
                        'エラー
                End Select

                WriteCount += 1

                ' 金額セパレータ
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)

                ' タンキング連番
                T_RIENT10.TANKING_DATA.bytTANKING_NO(0) = CType(WriteCount \ 256, Byte)
                T_RIENT10.TANKING_DATA.bytTANKING_NO(1) = CType(WriteCount Mod 256, Byte)

                ' 次データアドレス
                If cnt + 1 = WriteCount Then
                    ' 最終行
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = 255
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = 255
                Else
                    Dim NextAddr As Integer = 1024 + (WriteCount * 256)

                    Dim NextAddr0 As Integer = CType(NextAddr \ 16777216, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = CType(NextAddr0, Byte)

                    Dim NextAddr1 As Integer = CType((NextAddr Mod 16777216) \ 65536, Integer)
                    Dim Amari1 As Integer = CType((NextAddr Mod 16777216) Mod 65536, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = CType(NextAddr1, Byte)

                    Dim NextAddr2 As Integer = CType(Amari1 \ 256, Integer)
                    Dim Amari2 As Integer = CType(Amari1 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = CType(NextAddr2, Byte)

                    Dim NextAddr3 As Integer = CType(Amari2 Mod 256, Integer)
                    T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = CType(NextAddr3, Byte)
                End If

                ' 金店コード
                T_RIENT10.TANKING_DATA.strKINTEN_CD = T_RIENT77.TENPO_INFOREC(0).strKINKO_CD & T_RIENT77.TENPO_INFOREC(0).strSIT_CD

                'スペースを""に置きなおす
                T_RIENT10.TANKING_DATA.strTANKING_DATA = T_RIENT10.TANKING_DATA.strTANKING_DATA.Replace(" ", "")


                '金額セパレータを再計算してセット
                'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)
                '*****************************************

                StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_10, 0, 256)

                key.RecordNo = i + 1

                If InsertJifMast(Jdata) = False Then
                    MainLOG.Write("自振契約マスタ登録", "失敗", "")

                    Return False
                End If

            Next

            ' 最終レコード
            T_RIENT10.TANKING_LAST.Init()
            StrmWrite.Write(T_RIENT10.TANKING_LAST.Data, 0, 512)

            ' タンキングヘッダ 書込件数 再書込
            ' 全店舗Ｔ件数
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(20 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN, 0, 2)

            ' 店舗Ｔ件数
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)
            T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
            StrmWrite.Seek(36 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN, 0, 2)

            ' 店舗Ｔ終了ＦＤアドレス
            '***修正　前田　20080930****************************************
            'WriteCountが1以外なら、終了アドレスを正しく設定する
            If WriteCount <> 1 Then

                '***修正 maeda 2001006*******************************************
                '書込件数-1の値で終了アドレスを計算する(正確には最終レコードの開始アドレスのため)
                Dim FinishAddr As Integer = 1024 + ((WriteCount - 1) * 256)
                'Dim FinishAddr As Integer = 1024 + (WriteCount * 256)
                '****************************************************************

                Dim FinishAddr0 As Integer = CType(FinishAddr \ 16777216, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(0) = CType(FinishAddr0, Byte)

                Dim FinishAddr1 As Integer = CType((FinishAddr Mod 16777216) \ 65536, Integer)
                Dim FinishAmari1 As Integer = CType((FinishAddr Mod 16777216) Mod 65536, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(1) = CType(FinishAddr1, Byte)

                Dim FinishAddr2 As Integer = CType(FinishAmari1 \ 256, Integer)
                Dim FinishAmari2 As Integer = CType(FinishAmari1 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(2) = CType(FinishAddr2, Byte)

                Dim FinishAddr3 As Integer = CType(FinishAmari2 Mod 256, Integer)
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(3) = CType(FinishAddr3, Byte)
            End If

            StrmWrite.Seek(48 + 16, SeekOrigin.Begin)
            StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD, 0, 4)

            StrmWrite.Close()

        Catch ex As Exception
            MainLOG.Write("リエンタファイル作成", "失敗", ex.Message)

            Return False
        Finally
            If Not StrmWrite Is Nothing Then StrmWrite.Close()
        End Try
        Return True
    End Function

    Private Function GetKaiji() As Boolean

        Dim sql As New StringBuilder(64)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            MainLOG.Write("(回次取得)開始", "成功", "")

            sql.Append("SELECT NVL(MAX(KAIJI_JR),0) AS MAX_KAIJI FROM JIKEIMAST")
            sql.Append(" WHERE SYORI_DATE_JR = " & SQ(strDate))

            If OraReader.DataReader(sql) = True Then
                key.Kaiji = CType(OraReader.GetInt64("MAX_KAIJI"), Integer) + 1
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("(回次取得)", "失敗", ex.ToString)

            Return False
        Finally
            MainLOG.Write("(回次取得)終了", "成功", "")

            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return True

    End Function


    Private Function GetTENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New CASTCommon.MyOracleReader(MainDB)

        Try
            KIN_NNAME = ""
            SIT_NNAME = ""
            KIN_KNAME = ""
            SIT_KNAME = ""

            sql.Append("SELECT * FROM TENMAST WHERE KIN_NO_N = '" & KIN_NO.Trim & "' AND SIT_NO_N = '" & SIT_NO.Trim & "'")

            If orareader.DataReader(sql) = True Then
                KIN_NNAME = orareader.GetString("KIN_NNAME_N")
                SIT_NNAME = orareader.GetString("SIT_NNAME_N")
                KIN_KNAME = orareader.GetString("KIN_KNAME_N")
                SIT_KNAME = orareader.GetString("SIT_KNAME_N")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not orareader Is Nothing Then orareader.Close()
        End Try

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
