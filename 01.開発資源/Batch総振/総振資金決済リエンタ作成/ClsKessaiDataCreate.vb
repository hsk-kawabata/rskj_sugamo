Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Collections.Generic
Imports CASTCommon

Public Class ClsKessaiDataCreate

    Private Const FD_COUNT_LIMIT As Integer = 5000 'リエンタＦＤ１枚あたりの最大件数をセットする

    Public MainLOG As New CASTCommon.BatchLOG("KFS060", "為替請求リエンタ作成")

    Dim MainDB As CASTCommon.MyOracle

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
        Dim RIENTA_FILENAME As String   'リエンタファイル名

    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' 総振決済マスタのキー項目＋リエンタファイル名
    ''' </summary>
    ''' <remarks></remarks>
    Structure kesmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' 初期化
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private keskey As kesmastKey

    Private ParaKessaiDate As String        'パラメータから引き継いだ決済日
    Private KessaiList As New List(Of CAstFormKes.ClsFormKes.KessaiData) '資金決済データ格納用
    Private HONBU_KNAME As String = ""

    Private Structure KeyInfo
        '発信データ作成＆帳票ＣＳＶデータ作成兼用
        Dim TORIS_CODE As String            ' 取引先主コード
        Dim TORIF_CODE As String            ' 取引先副コード
        Dim FURI_DATE As String             ' 振込日
        Dim ITAKU_CODE As String            ' 委託者コード
        Dim ITAKU_KNAME As String           ' 委託者名カナ
        Dim TORIMATOME_SIT As String        ' 取りまとめ店
        Dim SIT_KNAME As String             ' 取りまとめ店名
        Dim KESSAI_KBN As String            ' 決済区分
        Dim KESSAI_PATN As String           ' 資金確保方法
        Dim BAITAI_CODE As String           ' 媒体コード

        Dim FURI_KEN As String              ' 振込済件数
        Dim FURI_KIN As String              ' 振込済金額

        Dim MESSAGE As String

        ' 初期化
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            FURI_DATE = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            TORIMATOME_SIT = ""
            SIT_KNAME = ""
            KESSAI_KBN = ""
            KESSAI_PATN = ""
            BAITAI_CODE = ""

            FURI_KEN = ""
            FURI_KIN = ""

            MESSAGE = ""
        End Sub

        ' ＤＢからの値を設定（為替請求リエンタ作成用）
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_S")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            SIT_KNAME = oraReader.GetString("SIT_KNAME_N")
            KESSAI_KBN = oraReader.GetString("KESSAI_KBN_T")
            KESSAI_PATN = oraReader.GetString("KESSAI_PATN_T")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")

            FURI_KEN = oraReader.GetString("FURI_KEN_S")
            FURI_KIN = oraReader.GetString("FURI_KIN_S")
        End Sub

    End Structure

    ' New
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 為替請求リエンタ作成初期処理
    ''' </summary>
    ''' <returns>正常:True 異常:False</returns>
    ''' <remarks></remarks>
    Public Function KessaiInit(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try
            'パラメータの読込
            param = CmdArgs(0).Split(","c)
            If param.Length = 2 Then

                'ログ書込み情報の設定
                MainLOG.FuriDate = param(0)                     '決済日セット
                MainLOG.JobTuuban = CType(param(1), Integer)
                MainLOG.ToriCode = "000000000000"
                '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                MainLOG.Write_LEVEL1("(初期処理)開始", "成功")
                'MainLOG.Write("(初期処理)開始", "成功")
                '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


                ParaKessaiDate = param(0)                       '決済日をセット

            Else
                '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                MainLOG.Write_LEVEL1("(初期処理)開始", "失敗", "コマンドライン引数のパラメータが不正です")
                'MainLOG.Write("(初期処理)開始", "失敗", "コマンドライン引数のパラメータが不正です")
                '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
                Return False

            End If

            'iniファイルの読込
            If IniRead() = False Then
                Return False
            End If

            Return True

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("(初期処理)開始", "失敗", ex.Message)
            'MainLOG.Write("(初期処理)開始", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            Return False
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("(初期処理)終了", "成功")
            'MainLOG.Write("(初期処理)終了", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        End Try

    End Function

    Private Function IniRead() As Boolean

        ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")           '自金庫コード
        If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
            'MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            jobMessage = "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD"
            Return False
        End If

        ini_info.JIKINKO_NAME = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")       '自金庫名
        If ini_info.JIKINKO_NAME = "err" OrElse ini_info.JIKINKO_NAME = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME")
            'MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            jobMessage = "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME"
            Return False
        End If

        ini_info.HONBU_CODE = CASTCommon.GetFSKJIni("COMMON", "HONBUCD")         '本部コード
        If ini_info.HONBU_CODE = "err" OrElse ini_info.HONBU_CODE = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD")
            'MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            jobMessage = "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD"
            Return False
        End If

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        'リエンタファイル作成先
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR")
            'MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            jobMessage = "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR"
            Return False
        End If

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DATのパス
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            'MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            jobMessage = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")       'リエンタファイル名
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:資金決済リエンタファイル名 分類:KESSAI 項目:RIENTANAME")
            'MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:資金決済リエンタファイル名 分類:KESSAI 項目:RIENTANAME")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            jobMessage = "設定ファイル取得失敗 項目名:資金決済リエンタファイル名 分類:KESSAI 項目:RIENTANAME"
            Return False
        End If

        Return True

    End Function

    ' 機能　 ： 為替請求リエンタ作成処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Public Function Main(ByVal command As String) As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

        MainDB = New CASTCommon.MyOracle
        Dim bRet As Boolean = True
        Dim iRet As Integer

        ' パラメータチェック
        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "為替請求リエンタ作成処理(開始)", "成功")
            'MainLOG.Write("(主処理)開始", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


            MainDB.BeginTrans()     ' トランザクション開始

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("(主処理)", "失敗", "為替請求リエンタ作成処理で実行待ちタイムアウト")
                MainLOG.UpdateJOBMASTbyErr("為替請求リエンタ作成処理で実行待ちタイムアウト")
                Return -1
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            '*******************************
            ' 回次を取得
            '*******************************
            If GetKaiji() = False Then
                '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                MainLOG.Write_LEVEL1("(主処理)", "失敗", "回次の取得に失敗しました")
                'MainLOG.Write("(主処理)", "失敗", "回次の取得に失敗しました")
                '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

                Return -1
            End If

            '*****************************************
            ' 資金決済データの格納とスケジュールの更新
            '*****************************************
            iRet = MakeKessaiData()
            Select Case iRet
                Case 0          ' データ格納成功
                    bRet = True
                Case 1          ' 対象データ０件
                    bRet = True
                Case Else       ' データ格納失敗
                    bRet = False
            End Select

            '***********************
            ' リエンタFD作成
            '***********************
            Dim msgtitle As String = "為替請求リエンタ作成(KFS060)"
            Dim FDCnt As Integer = 1 'FD枚数

            If iRet = 0 AndAlso KessaiList.Count > 0 Then

                If MakeRientaFD(FDCnt) = False Then
                    jobMessage = "為替請求リエンタ作成失敗"
                    iRet = -1
                Else

                    For i As Integer = 1 To FDCnt
                        If i > 1 Then
                            MessageBox.Show(String.Format(MSG0500I, FD_COUNT_LIMIT, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, _
                                            Windows.Forms.MessageBoxDefaultButton.Button1, Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
                        End If

                        Do
                            Try

                                Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                                Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()

                                iRet = 0
                                Exit Do

                            Catch ex As Exception
                                If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                    iRet = -1
                                    jobMessage = "FD挿入がキャンセルされました。"
                                    '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                                    MainLOG.Write_LEVEL1("FD要求", "失敗", "FD挿入がキャンセル")
                                    'MainLOG.Write("FD要求", "失敗", "FD挿入がキャンセル")
                                    '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

                                    Exit Do
                                End If
                            End Try
                        Loop

                        Select Case iRet
                            Case 0          ' データ格納成功
                                If File.Exists(Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME)) Then
                                    If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) <> DialogResult.OK Then
                                        jobMessage = "フロッピー内ファイル削除キャンセル"
                                        iRet = -1
                                    End If
                                End If

                                If iRet = 0 Then
                                    File.Copy(Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME & i), Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME), True)
                                End If
                        End Select
                    Next

                End If

            End If

            If iRet <> 0 Then
                bRet = False
            End If

            '*******************************
            ' 帳票出力
            '*******************************
            ' 資金決済データが１件以上存在する場合、帳票出力
            If iRet = 0 Then
            End If

            If bRet = False Then
                If jobMessage = "" Then
                    Call MainLOG.UpdateJOBMASTbyErr("ログ参照")
                Else
                    Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                End If

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                ' ロールバック
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "対象データ０件"
                End If

                Call MainLOG.UpdateJOBMASTbyOK(jobMessage)

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                ' コミット
                MainDB.Commit()
            End If

            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("(主処理)", "失敗", ex.ToString)
            'MainLOG.Write("(主処理)", "失敗", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

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
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "為替請求リエンタ作成処理(終了)", "成功")
            'MainLOG.Write("(主処理)終了", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        End Try

        Return 0

    End Function

    ' 機能　 ： 資金決済データ作成処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '

    Private Function MakeKessaiData() As Integer

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = New StringBuilder(256)

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("(資金決済データ格納)開始", "成功")
            'MainLOG.Write("(資金決済データ格納)開始", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.AppendLine("SELECT")
            SQL.AppendLine(" MAX(TORIS_CODE_S) TORIS_CODE_S")
            SQL.AppendLine(",MAX(TORIF_CODE_S) TORIF_CODE_S")
            SQL.AppendLine(",MAX(FURI_DATE_S) FURI_DATE_S")
            SQL.AppendLine(",MAX(SYUBETU_S) SYUBETU_S")
            SQL.AppendLine(",MAX(ITAKU_CODE_S) ITAKU_CODE_S")
            SQL.AppendLine(",SUM(FURI_KEN_S) FURI_KEN_S")
            SQL.AppendLine(",SUM(FURI_KIN_S) FURI_KIN_S")

            SQL.AppendLine(",MAX(ITAKU_KNAME_T) ITAKU_KNAME_T")
            SQL.AppendLine(",MAX(TORIMATOME_SIT_T) TORIMATOME_SIT_T")
            SQL.AppendLine(",MAX(KESSAI_KBN_T) KESSAI_KBN_T")
            SQL.AppendLine(",MAX(KESSAI_PATN_T) KESSAI_PATN_T")

            SQL.AppendLine(",MAX(KIN_KNAME_N) KIN_KNAME_N")
            SQL.AppendLine(",MAX(SIT_KNAME_N) SIT_KNAME_N")
            SQL.AppendLine(",MAX(BAITAI_CODE_S) BAITAI_CODE_S")
            SQL.AppendLine(" FROM S_TORIMAST")
            SQL.AppendLine("     ,S_SCHMAST")
            SQL.AppendLine("     ,TENMAST")

            SQL.AppendLine(" WHERE KESSAI_YDATE_S = " & SQ(ParaKessaiDate))
            SQL.AppendLine(" AND KESSAI_FLG_S = '0'")
            SQL.AppendLine(" AND HASSIN_FLG_S = '1'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            SQL.AppendLine(" AND FURI_KIN_S > 0")
            SQL.AppendLine(" AND KESSAI_KBN_T <> '99'")
            SQL.AppendLine(" AND TORIS_CODE_S   = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S   = TORIF_CODE_T")
            SQL.AppendLine(" AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
            SQL.AppendLine(" AND TORIMATOME_SIT_T = SIT_NO_N(+)")
            SQL.AppendLine(" GROUP BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")
            SQL.AppendLine(" ORDER BY TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")

            Dim Key As KeyInfo = Nothing
            Dim test As String = SQL.ToString

            If OraReader.DataReader(SQL) = True Then

                Dim lstKessaiData As New List(Of CAstFormKes.ClsFormKes.KessaiData)

                ' キー初期化
                Key.Init()

                ' 本部店名取得
                HONBU_KNAME = GetTenmast()

                ' 最初のキー設定
                Call Key.SetOraDataKessai(OraReader)

                Do While OraReader.EOF = False
                    lstKessaiData.Clear()

                    ' 資金決済データ取得処理(総合振込用)
                    If fn_GetKessaiData(Key, lstKessaiData) = False Then
                        Return -1
                    End If

                    If Not (lstKessaiData Is Nothing OrElse lstKessaiData.Count = 0) Then
                        ' 取得した資金決済データを基に、総振決済マスタ登録を行う
                        For i As Integer = 0 To lstKessaiData.Count - 1
                            Dim KData As CAstFormKes.ClsFormKes.KessaiData = lstKessaiData.Item(i)

                            ' 総振決済マスタの登録処理
                            If InsertKessaiMast(Key, KData) = False Then
                                jobMessage = "総振決済マスタ登録失敗 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & _
                                             " 振込日：" & Key.FURI_DATE
                                Return -1
                            End If
                        Next
                    End If

                    ' スケジュールマスタの更新処理 
                    If UpdateSchMast(Key) = False Then
                        jobMessage = "スケジュールマスタ更新失敗 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & _
                                     " 振込日：" & Key.FURI_DATE
                        Return -1
                    End If

                    ' 対象データの次レコードを読込む
                    OraReader.NextRead()

                    If OraReader.EOF = False Then
                        ' キー設定
                        Call Key.SetOraDataKessai(OraReader)
                    End If

                    KessaiList.AddRange(lstKessaiData)
                Loop
            End If

            If KessaiList.Count = 0 Then
                '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                MainLOG.Write_LEVEL1("(資金決済データ格納)", "失敗", "件数０件")
                'MainLOG.Write("(資金決済データ格納)", "失敗", "件数０件")
                '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

                Return 1
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("(資金決済データ格納)", "失敗", ex.Message)
            'MainLOG.Write("(資金決済データ格納)", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            Return -1
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("(資金決済データ格納)終了", "成功")
            'MainLOG.Write("(資金決済データ格納)終了", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try

        Return 0

    End Function


    ' 機能　 ： 資金決済データ取得処理(総合振込用)
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_GetKessaiData(ByRef Key As KeyInfo, ByRef lstKessaiData As List(Of CAstFormKes.ClsFormKes.KessaiData)) As Boolean

        Dim errFlg As Boolean = False
        Dim errMsg As String = "決済情報に誤りがあります。"
        Dim KData As CAstFormKes.ClsFormKes.KessaiData = Nothing

        strKEKKA = ""

        Try
            '為替請求のみ
            Select Case Key.KESSAI_KBN
                Case "00"
                    ' 為替請求のデータ作成
                    If errFlg = False AndAlso fn_KAWASE_SEIKYU(Key, KData) >= 0 AndAlso fn_KessaiData(Key, KData) = True Then
                        ' 資金決済データのリスト作成
                        lstKessaiData.Add(KData)
                    Else
                        errFlg = True
                    End If
                Case "99"

                Case Else
                    errFlg = True
                    errMsg &= "(決済区分)"
            End Select

            If errFlg = True Then
                jobMessage = errMsg & " 取引先主コード：" & Key.TORIS_CODE & " 取引先副コード：" & Key.TORIF_CODE & " 振込日：" & Key.FURI_DATE & _
                    " 決済区分：" & Key.KESSAI_KBN
                '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                MainLOG.Write_LEVEL1("資金決済データ取得処理(総合振込用)", "失敗", jobMessage)
                'MainLOG.Write("資金決済データ取得処理(総合振込用)", "失敗", jobMessage)
                '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

                Return False
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("資金決済データ取得処理(総合振込用)", "失敗", ex.Message)
            'MainLOG.Write("資金決済データ取得処理(総合振込用)", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 資金決済データ作成処理
    '
    ' 引数　 ： 
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_KessaiData(ByRef Key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean

        Try
            ' 通番のカウントアップ
            keskey.RecordNo += 1
         
        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("資金決済データ作成", "失敗", ex.Message)
            'MainLOG.Write("資金決済データ作成", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 為替請求データ作成処理
    '
    ' 引数　 ：
    '
    ' 戻り値 ： 0 - 正常，-1 - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_KAWASE_SEIKYU(ByRef key As KeyInfo, ByRef KData As CAstFormKes.ClsFormKes.KessaiData) As Integer

        Dim KawaseSeikyuInFmt As New CAstFormKes.ClsFormSikinFuri.T_48600
        Dim strKINFUKKI_FUGOU As String = ""    ' 金額複記符号

        Try
            ' 初期化
            KawaseSeikyuInFmt.Init()

            ' 金額複記符号の取得
            If fn_FUGO_SETTEI(CASTCommon.CADec(key.FURI_KIN).ToString("#,##0"), strKINFUKKI_FUGOU) = False Then
                '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
                MainLOG.Write_LEVEL1("為替請求データ作成", "失敗", "複記符号設定処理エラー。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE)
                'MainLOG.Write("為替請求データ作成", "失敗", "複記符号設定処理エラー。取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE)
                '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
                Return -1
            End If

            'データ設定
            With KawaseSeikyuInFmt
                .TORIATUKAI = ParaKessaiDate
                .SYUMOKU = "4701"                                   ' 種目コード
                .JUSIN_TEN = "ﾟ " & key.SIT_KNAME                   ' 受信店名
                .FUKA_CODE = "000"                                  ' 付加コード
                .HASSIN_TEN = "ﾟ " & HONBU_KNAME                    ' 発信店名
                .KINGAKU = key.FURI_KIN.ToString.PadLeft(10)        ' 金額
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ---------------------------------------->>>>
                '.KESSAI_CNT = " "                                   ' 決済回数
                '2011/08/24 saitou 第6次全銀対応 決済回数削除 DEL ----------------------------------------<<<<
                .KINGAKU_FUGOU = strKINFUKKI_FUGOU.PadRight(15, " "c) ' 金額複記符号
                .BANGOU = ""                                        ' 番号
                .SIKIN_JIYUU1 = "ｲﾗｲﾆﾝ" & key.ITAKU_KNAME.Trim & "ﾌﾞﾝ"  ' 資金付替理由
                .SIKIN_JIYUU2 = key.FURI_KEN.Trim & "ｹﾝ"            ' 資金付替理由２
                .BIKOU1 = ""                                        ' 備考１
                .BIKOU2 = ""                                        ' 備考２
                .SYOKAI_NO = ""                                     ' 照会番号
                .YOBI1 = ""                                         ' 予備１
            End With

            ' 資金決済データにオペコード毎の個別データを設定
            KData.record320 = KawaseSeikyuInFmt.Data
            KData.OpeCode = String.Concat(KawaseSeikyuInFmt.KAMOKU_CODE, KawaseSeikyuInFmt.OPE_CODE)
            KData.TorimatomeSit = key.TORIMATOME_SIT

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("為替請求データ作成", "失敗", ex.Message)
            'MainLOG.Write("為替請求データ作成", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            Return -1
        End Try

        Return 0

    End Function

    ' 機能　 ： 複記符号設定処理
    '
    ' 引数　 ： astrKEY1:変換前金額（カンマ編集済み）
    '           astrKEY2
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： パラメタでわたされた金額をもとに１５ケタの複記符号を返す
    '
    Private Function fn_FUGO_SETTEI(ByVal astrKEY1 As String, ByRef astrKEY2 As String) As Boolean
        Dim intCount As Integer     '文字数
        Dim strASSYUKU As String    '圧縮
        Dim strFUGO(14) As String   '符号
        Dim I As Integer

        Try
            astrKEY2 = ""
            strASSYUKU = "Y"

            For intCount = 0 To astrKEY1.Length - 1

                strFUGO(intCount) = " "

                Select Case astrKEY1.Substring(intCount, 1)
                    Case "0"
                        If strASSYUKU = "Y" Then
                            strFUGO(intCount) = " "
                        Else
                            strFUGO(intCount) = "ﾄ"
                        End If
                    Case "1"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾋ"
                    Case "2"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾌ"
                    Case "3"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾐ"
                    Case "4"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾖ"
                    Case "5"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ｲ"
                    Case "6"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾙ"
                    Case "7"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾅ"
                    Case "8"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ﾔ"
                    Case "9"
                        strASSYUKU = "N"
                        strFUGO(intCount) = "ｺ"
                    Case ","
                        strFUGO(intCount) = " "
                End Select
            Next

            For I = 0 To strFUGO.Length - 1
                astrKEY2 = astrKEY2 & strFUGO(I)
            Next

            astrKEY2 = astrKEY2.Trim

        Catch ex As Exception

            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("複記符号設定処理", "失敗", ex.Message)
            'MainLOG.Write("複記符号設定処理", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            Return False
        End Try

        Return True

    End Function

    ' 機能　 ： 総振決済マスタ登録
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertKessaiMast(ByVal Key As KeyInfo, ByVal KData As CAstFormKes.ClsFormKes.KessaiData) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("INSERT INTO S_KESSAIMAST(")
            SQL.AppendLine(" SYORI_DATE_KR")
            SQL.AppendLine(",TIME_STAMP_KR")
            SQL.AppendLine(",KAIJI_KR")
            SQL.AppendLine(",RECORD_NO_KR")
            SQL.AppendLine(",FILE_NAME_KR")
            SQL.AppendLine(",TORIS_CODE_KR")
            SQL.AppendLine(",TORIF_CODE_KR")
            SQL.AppendLine(",FURI_DATE_KR")
            SQL.AppendLine(",MOTIKOMI_SEQ_KR")
            SQL.AppendLine(",KAMOKU_CODE_KR")
            SQL.AppendLine(",OPE_CODE_KR")
            SQL.AppendLine(",DENBUN_ALL_KR")
            SQL.AppendLine(",ERR_CODE_KR")
            SQL.AppendLine(",ERR_MSG_KR")
            SQL.AppendLine(",SAKUSEI_DATE_KR")
            SQL.AppendLine(",KOUSIN_DATE_KR")
            SQL.AppendLine(") VALUES (")
            SQL.AppendLine(" " & SQ(strDate))                                   ' 処理日
            SQL.AppendLine("," & SQ(String.Concat(strDate, strTime)))           ' タイムスタンプ
            SQL.AppendLine("," & SQ(keskey.Kaiji))                              ' 回次
            SQL.AppendLine("," & SQ(keskey.RecordNo))                           ' 通番
            SQL.AppendLine("," & SQ(ini_info.RIENTA_FILENAME))                  ' リエンタファイル名
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' 取引先主コード
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' 取引先副コード
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' 振込日
            SQL.AppendLine(",1")                                                ' 持込SEQ
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(0, 2)))             ' 科目コード
            SQL.AppendLine("," & SQ(KData.OpeCode.Substring(2, 3)))             ' オペコード
            SQL.AppendLine("," & SQ(KData.record320))                           ' 個別データ
            SQL.AppendLine("," & SQ(""))                                        ' 結果コード
            SQL.AppendLine("," & SQ(""))                                        ' エラーメッセージ
            SQL.AppendLine("," & SQ(strDate))                                   ' 作成日
            SQL.AppendLine("," & SQ("00000000"))                                ' 更新日
            SQL.AppendLine(")")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("(総振決済マスタ登録)", "失敗", ex.Message)
            'MainLOG.Write("(総振決済マスタ登録)", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            Return False
        Finally
        End Try

        Return True

    End Function

    ' 機能　 ： スケジュールマスタ更新
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateSchMast(ByVal key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("UPDATE S_SCHMAST")
            SQL.AppendLine(" SET")
            SQL.AppendLine(" KESSAI_FLG_S = '1'")
            SQL.AppendLine(",KESSAI_DATE_S = " & SQ(ParaKessaiDate))
            SQL.AppendLine(",KESSAI_TIME_STAMP_S = " & SQ(strDate & strTime))
            SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & _
                          " 振込日：" & key.FURI_DATE & " " & ex.Message)
            'MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & " 取引先副コード：" & key.TORIF_CODE & _
            '              " 振込日：" & key.FURI_DATE & " " & ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            
            Return False
        End Try

        Return True

    End Function

    Private Function MakeRientaFD(ByRef FDCnt As Integer) As Boolean
        Dim T_RIENT77 As New CAstFormKes.ClsT_RIENT77
        Dim T_RIENT10 As New CAstFormKes.ClsT_RIENT10

        Dim Kdata As CAstFormKes.ClsFormKes.KessaiData
        Dim EncdJ As Encoding = Encoding.GetEncoding("SHIFT-JIS")

        Dim T_48100 As New CAstFormKes.ClsFormSikinFuri.T_48100
        Dim T_48600 As New CAstFormKes.ClsFormSikinFuri.T_48600

        Dim StrmWrite As FileStream = Nothing

        Dim iLoop As Integer = 0 '次のFDでも初期化させないためのループ用カウンタ

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

            '複数枚用ループ
            While iLoop < KessaiList.Count - 1 OrElse iLoop = 0
                ' リエンタファイル オープン
                Dim filename As String = Path.Combine(ini_info.DAT_PATH, ini_info.RIENTA_FILENAME & FDCnt)

                If File.Exists(filename) = True Then
                    ' 既に存在する場合は，削除
                    File.Delete(filename)
                End If

                StrmWrite = New FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite)

                Dim Bytes As Byte() = EncdJ.GetBytes(ini_info.RIENTA_FILENAME.PadRight(12, Nothing).PadRight(16, " "c))
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

                Dim cnt As Integer = KessaiList.Count - 1 'ループ回数

                For i As Integer = iLoop To cnt

                    ' タンキングデータ
                    Kdata = KessaiList.Item(i)

                    Select Case Kdata.OpeCode
                        Case "48100"
                            T_RIENT10.TANKING_DATA.Init_48()
                            T_48100.DataSepaPlus = Kdata.record320
                            'RecLen = T_48100.DataSepaPlus.Replace(" ", "").Length + 32
                            RecLen = T_48100.DataSepaPlus.Length + 32
                            T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                            T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                            T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                            T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48100.DataSepaPlus
                            T_48100 = Nothing
                        Case "48600"
                            T_RIENT10.TANKING_DATA.Init_48()
                            T_48600.DataSepaPlus = Kdata.record320
                            'RecLen = T_48600.DataSepaPlus.Replace(" ", "").Length + 32
                            RecLen = T_48600.DataSepaPlus.Length + 32
                            T_RIENT10.TANKING_DATA.strOPE_CODE = Kdata.OpeCode
                            T_RIENT10.TANKING_DATA.strKAMOKU_CODE = Kdata.OpeCode.Substring(0, 2)
                            T_RIENT10.TANKING_DATA.strTORIHIKI_CODE = Kdata.OpeCode.Substring(2, 3)
                            T_RIENT10.TANKING_DATA.strTANKING_DATA = T_48600.DataSepaPlus
                            T_48600 = Nothing
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

                        Dim NextAddr0 As Integer = CType(NextAddr \ 16777216, Integer)      '2010.05.08 /　→　\
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(0) = CType(NextAddr0, Byte)

                        Dim NextAddr1 As Integer = CType((NextAddr Mod 16777216) \ 65536, Integer)  '2010.05.08 /　→　\
                        Dim Amari1 As Integer = CType((NextAddr Mod 16777216) Mod 65536, Integer)
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(1) = CType(NextAddr1, Byte)

                        Dim NextAddr2 As Integer = CType(Amari1 \ 256, Integer)     '2010.05.08 /　→　\
                        Dim Amari2 As Integer = CType(Amari1 Mod 256, Integer)
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(2) = CType(NextAddr2, Byte)

                        Dim NextAddr3 As Integer = CType(Amari2 Mod 256, Integer)
                        T_RIENT10.TANKING_DATA.bytNEXT_DATA_ADD(3) = CType(NextAddr3, Byte)
                    End If

                    ' 金店コード
                    T_RIENT10.TANKING_DATA.strKINTEN_CD = T_RIENT77.TENPO_INFOREC(0).strKINKO_CD & T_RIENT77.TENPO_INFOREC(0).strSIT_CD

                    'スペースを""に置きなおす
                    'T_RIENT10.TANKING_DATA.strTANKING_DATA = T_RIENT10.TANKING_DATA.strTANKING_DATA.Replace(" ", "")


                    '金額セパレータを再計算してセット
                    'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(0) = CType(RecLen \ 256, Byte)
                    'T_RIENT10.TANKING_DATA.bytKINGAKU_SEPA(1) = CType(RecLen Mod 256, Byte)
                    '*****************************************
                    Select Case Kdata.OpeCode.Substring(0, 2)
                        Case "48"
                            StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_48, 0, 256)
                        Case Else
                            StrmWrite.Write(T_RIENT10.TANKING_DATA.Data_10, 0, 256)
                    End Select

                    iLoop += 1
                    '書込み件数が最大件数になった場合は書込みを止める
                    If iLoop >= FD_COUNT_LIMIT AndAlso iLoop Mod FD_COUNT_LIMIT = 0 Then
                        '次のレコードが存在する場合はFD枚数カウントアップ
                        If iLoop < KessaiList.Count - 1 Then
                            FDCnt += 1
                        End If

                        Exit For
                    End If
                Next

                ' 最終レコード
                T_RIENT10.TANKING_LAST.Init()
                StrmWrite.Write(T_RIENT10.TANKING_LAST.Data, 0, 512)

                ' タンキングヘッダ 書込件数 再書込
                ' 全店舗Ｔ件数
                T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)  '2010.05.08 /　→　\
                T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
                StrmWrite.Seek(20 + 16, SeekOrigin.Begin)
                StrmWrite.Write(T_RIENT77.TANKING_HEAD.bytZENTENPO_TKEN, 0, 2)

                ' 店舗Ｔ件数
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(0) = CType(WriteCount \ 256, Byte)   '2010.05.08 /　→　\
                T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN(1) = CType(WriteCount Mod 256, Byte)
                StrmWrite.Seek(36 + 16, SeekOrigin.Begin)
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TKEN, 0, 2)

                ' 店舗Ｔ終了ＦＤアドレス
                'WriteCountが1以外なら、終了アドレスを正しく設定する
                If WriteCount <> 1 Then

                    '書込件数-1の値で終了アドレスを計算する(正確には最終レコードの開始アドレスのため)
                    Dim FinishAddr As Integer = 1024 + ((WriteCount - 1) * 256)
                    'Dim FinishAddr As Integer = 1024 + (WriteCount * 256)

                    Dim FinishAddr0 As Integer = CType(FinishAddr \ 16777216, Integer)     '2010.05.08 /　→　\
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(0) = CType(FinishAddr0, Byte)

                    Dim FinishAddr1 As Integer = CType((FinishAddr Mod 16777216) \ 65536, Integer)     '2010.05.08 /　→　\
                    Dim FinishAmari1 As Integer = CType((FinishAddr Mod 16777216) Mod 65536, Integer)
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(1) = CType(FinishAddr1, Byte)

                    Dim FinishAddr2 As Integer = CType(FinishAmari1 \ 256, Integer)        '2010.05.08 /　→　\
                    Dim FinishAmari2 As Integer = CType(FinishAmari1 Mod 256, Integer)
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(2) = CType(FinishAddr2, Byte)

                    Dim FinishAddr3 As Integer = CType(FinishAmari2 Mod 256, Integer)
                    T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD(3) = CType(FinishAddr3, Byte)
                End If

                StrmWrite.Seek(48 + 16, SeekOrigin.Begin)
                StrmWrite.Write(T_RIENT77.TENPO_INFOREC(0).bytTENPO_TFINISH_FDADD, 0, 4)

                StrmWrite.Close()

            End While

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("リエンタファイル作成", "失敗", ex.Message)
            'MainLOG.Write("リエンタファイル作成", "失敗", ex.Message)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
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
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("(回次取得)開始", "成功", "")
            'MainLOG.Write("(回次取得)開始", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


            sql.Append("SELECT NVL(MAX(KAIJI_KR),0) AS MAX_KAIJI FROM S_KESSAIMAST")
            sql.Append(" WHERE SYORI_DATE_KR = " & SQ(strDate))

            If OraReader.DataReader(sql) = True Then
                keskey.Kaiji = CType(OraReader.GetInt64("MAX_KAIJI"), Integer) + 1
            Else
                Return False
            End If

        Catch ex As Exception
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Err("(回次取得)", "失敗", ex.ToString)
            'MainLOG.Write("(回次取得)", "失敗", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
            Return False
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_LEVEL1("(回次取得)終了", "成功", "")
            'MainLOG.Write("(回次取得)終了", "成功", "")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        Return True

    End Function

    Private Function GetTenmast() As String
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append("SELECT SIT_KNAME_N FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(ini_info.JIKINKO_CODE))
            SQL.Append("   AND SIT_NO_N = " & SQ(ini_info.HONBU_CODE))
            If OraReader.DataReader(SQL) = True Then
                Return OraReader.GetString("SIT_KNAME_N")
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
