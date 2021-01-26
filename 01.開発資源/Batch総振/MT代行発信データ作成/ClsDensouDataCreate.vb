Imports System
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' MT代行発信データ作成　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class ClsDensouDataCreate

#Region "クラス変数"

    Public MainLOG As New CASTCommon.BatchLOG("KFS110", "MT代行発信データ作成")
    Private MainDB As CASTCommon.MyOracle

    ' パブリックフォーマット
    Private FmtComm As New CAstFormat.CFormat

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
        Dim JIKINKO_CODE As String           '自金庫コード
        Dim JIKINKO_NAME As String           '自金庫名
        Dim HONBU_CODE As String             '本部コード
        Dim DAT_PATH As String               'DATのパス
        Dim RIENTA_FILENAME As String        'リエンタファイル名
        Dim DEN_PATH As String               'DENのパス
        Dim FTR As String                    'FTRのパス
        Dim FTRANP As String                 'FTRANPのパス
        Dim RSV2_S_KAWASE_HONSITEN As String ' 為替振込明細票(本支店為替)印刷要否
        Dim RSV2_S_KAWASE_TAKOU As String    ' 為替振込明細票(他行為替)印刷要否
        Dim RSV2_S_KAWASE_LOGGING As String  ' 為替振込明細票(ロギング)印刷要否
    End Structure
    Private ini_info As strcIni

    ''' <summary>
    ''' 発信マスタのキー項目＋リエンタファイル名
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure hasmastKey

        Dim Kaiji As Integer
        Dim RecordNo As Integer

        ' 初期化
        Public Sub Init()
            Kaiji = 0
            RecordNo = 0
        End Sub
    End Structure
    Private haskey As hasmastKey

    Private Structure strcParam
        Dim HASSIN_DATE As String           'パラメータから引き継いだ発信日
        Dim KYUFURI_DATE As String          'パラメータから引き継いだ給振可能日
        Dim SOUFURI_DATE As String          'パラメータから引き継いだ総振可能日
    End Structure
    Private PARAMETER As strcParam

    Private HassinList As New List(Of CAstFormat.ClsFormSOU.PrnSoufuriData) '振込発信データ格納用
    Private HONBU_KNAME As String = ""
    Private JIKINKO_KNAME As String = ""
    Private HONBU_NNAME As String = ""
    Private JIKINKO_NNAME As String = ""

    Private JIKOU_KEN As Long = 0         '自行件数
    Private TAKOU_KEN As Long = 0         '他行分件数
    Private JIFURI_KEN As Long = 0        '自振ロギング件数

    Private plngJyunKen As Long = 0             ' 順電文合計件数  'Shintani.T
    Private plngJyunKin As Long = 0             ' 順電文合計金額    'Shintani.T
    Private plngSakiKen As Long = 0             ' 先電文合計件数  'Shintani.T
    Private plngSakiKin As Long = 0             ' 先電文合計金額    'Shintani.T

    Private strWRK_FILE_NAME As String = "" '伝送ワークファイル名
    Private strFILE_NAME As String = ""     '伝送ファイル名

    Private Structure FKeyInfo
        '発信データ作成＆帳票ＣＳＶデータ作成兼用
        Dim TORIS_CODE As String            ' 取引先主コード
        Dim TORIF_CODE As String            ' 取引先副コード
        Dim BAITAI_CODE As String           ' 媒体コード
        Dim FURI_DATE As String             ' 振込日
        Dim MOTIKOMI_SEQ As Integer         ' 持込SEQ
        Dim SOUSIN_KBN As String            ' 送信区分
        Dim SYUBETU As String               ' 種別
        Dim SYUMOKU_CODE As String          ' 種目コード
        Dim FUKA_CODE As String             ' 付加コード
        Dim FURI_CODE As String             ' 振替コード
        Dim KIGYO_CODE As String            ' 企業コード
        Dim ITAKU_CODE As String            ' 委託者コード
        Dim ITAKU_KNAME As String           ' 委託者名カナ
        Dim ITAKU_NNAME As String           ' 委託者名漢字
        Dim TORIMATOME_SIT As String        ' 取りまとめ店
        Dim TUKEKIN_KNAME As String         ' 金融機関名
        Dim TUKESIT_KNAME As String         ' 支店名
        Dim BIKOU1 As String                ' 備考１
        Dim BIKOU2 As String                ' 備考２

        Dim TEKIYOU_SYUBETU As String       ' 適用種別
        Dim FURI_NAME As String             ' 振替名称

        Dim TotalKen As Long                ' 明細の合計件数
        Dim TotalKin As Long                ' 明細の合計金額
        Dim JikouKen As Long                ' 明細の自行合計件数
        Dim JikouKin As Long                ' 明細の自行合計金額
        Dim TakouKen As Long                ' 明細の他行合計件数
        Dim TakouKin As Long                ' 明細の他行合計金額
        Dim JifuriKen As Long               ' 明細の自振ロギング合計件数
        Dim JifuriKin As Long               ' 明細の自振ロギング合計金額
        '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
        Dim TesuuKin As Long                ' 明細の手数料合計
        '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END

        Dim MESSAGE As String

        ' 初期化
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            BAITAI_CODE = ""
            FURI_DATE = ""
            MOTIKOMI_SEQ = 0
            SOUSIN_KBN = ""
            SYUBETU = ""
            SYUMOKU_CODE = ""
            FUKA_CODE = ""
            FURI_CODE = ""
            KIGYO_CODE = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            ITAKU_NNAME = ""
            TORIMATOME_SIT = ""
            TUKEKIN_KNAME = ""
            TUKESIT_KNAME = ""
            BIKOU1 = ""
            BIKOU2 = ""

            TEKIYOU_SYUBETU = ""
            FURI_NAME = ""

            TotalKen = 0
            TotalKin = 0
            JikouKen = 0
            JikouKin = 0
            TakouKen = 0
            TakouKin = 0
            JifuriKen = 0
            JifuriKin = 0
            '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
            TesuuKin = 0
            '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END

            MESSAGE = ""

        End Sub

        ' ＤＢからの値を設定（振込発信データ作成用）
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(10)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            MOTIKOMI_SEQ = oraReader.GetInt("MOTIKOMI_SEQ_S")
            SOUSIN_KBN = oraReader.GetString("SOUSIN_KBN_S")
            SYUBETU = oraReader.GetString("SYUBETU_S")
            SYUMOKU_CODE = oraReader.GetString("SYUMOKU_CODE_T")
            FUKA_CODE = oraReader.GetString("FUKA_CODE_T")
            FURI_CODE = oraReader.GetString("FURI_CODE_S")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_S")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_S")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
            TORIMATOME_SIT = oraReader.GetString("TORIMATOME_SIT_T")
            TUKEKIN_KNAME = oraReader.GetString("KIN_KNAME_N")
            TUKESIT_KNAME = oraReader.GetString("SIT_KNAME_N")
            BIKOU1 = oraReader.GetString("BIKOU1_T")
            BIKOU2 = oraReader.GetString("BIKOU2_T")
        End Sub

    End Structure

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' 初期処理を行います。
    ''' </summary>
    ''' <param name="CmdArgs"></param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Public Function Init(ByVal CmdArgs() As String) As Boolean

        Dim param() As String

        Try

            '--------------------------------------------------
            'パラメータのチェック
            '--------------------------------------------------
            param = CmdArgs(0).Split(","c)
            If param.Length = 4 Then

                'ログ書込み情報の設定
                MainLOG.FuriDate = "00000000"
                MainLOG.JobTuuban = CInt(param(3))
                MainLOG.ToriCode = "000000000000"

                MainLOG.Write("(初期処理)開始", "成功")

                PARAMETER.HASSIN_DATE = param(0)                       '発信日をセット
                PARAMETER.KYUFURI_DATE = param(1)                       '給振可能日をセット
                PARAMETER.SOUFURI_DATE = param(2)                       '総振可能日をセット

            Else

                MainLOG.Write("(初期処理)開始", "失敗", "コマンドライン引数のパラメータが不正です")
                Return False

            End If

            '--------------------------------------------------
            '設定ファイルの読み込み
            '--------------------------------------------------
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

    ''' <summary>
    ''' MT代行発信データのメイン処理を行います。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Main() As Integer

        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If

        MainDB = New CASTCommon.MyOracle
        FmtComm.Oracle = MainDB
        Dim bRet As Boolean = True
        Dim iRet As Integer

        Dim sw As System.Diagnostics.Stopwatch = Nothing

        Try
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "MT代行発信リエンタ作成処理(開始)", "成功")

            MainDB.BeginTrans()     ' トランザクション開始

            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("(主処理)", "失敗", "MT代行発信データ作成処理で実行待ちタイムアウト")
                MainLOG.UpdateJOBMASTbyErr("MT代行発信データ作成処理で実行待ちタイムアウト")
                Return -1
            End If

            '--------------------------------------------------
            ' 回次を取得
            '--------------------------------------------------
            If GetKaiji() = False Then

                MainLOG.Write("(主処理)", "失敗", "回次の取得に失敗しました")
                Return -1
            End If

            '--------------------------------------------------
            ' レコード番号を取得
            '--------------------------------------------------
            If GetRecordNo() = False Then

                MainLOG.Write("(主処理)", "失敗", "レコード番号の取得に失敗しました")
                Return -1
            End If

            '--------------------------------------------------
            ' 振込発信データの格納とスケジュールの更新
            '--------------------------------------------------
            iRet = MakeFurikomiData()
            Select Case iRet
                Case 0          ' データ格納成功
                    bRet = True
                Case 1          ' 対象データ０件
                    bRet = True
                Case Else       ' データ格納失敗
                    bRet = False
            End Select

            '--------------------------------------------------
            ' コード変換
            '--------------------------------------------------
            strFILE_NAME = Path.Combine(ini_info.DEN_PATH, ini_info.RIENTA_FILENAME)
            If File.Exists(strFILE_NAME) = True Then File.Delete(strFILE_NAME)
            If iRet = 0 Then
                If fn_CODE_CHANGE() = False Then
                    jobMessage = "コード変換失敗"
                    bRet = False
                    iRet = 9
                End If
                If File.Exists(strFILE_NAME) = False Then
                    jobMessage = "配信ファイル作成失敗"
                    bRet = False
                    iRet = 9
                End If

            End If

            '--------------------------------------------------
            ' 帳票出力
            '--------------------------------------------------
            ' 振込発信データが１件以上存在する場合、帳票出力
            If iRet = 0 Then
                Dim ExeRepo As New CAstReports.ClsExecute
                bRet = True

                '為替振込明細表(本支店為替)
                If bRet = True AndAlso JIKOU_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP010", "為替振込明細表(本支店為替)(MT代行)")
                End If

                '為替振込明細表(他行為替)
                If bRet = True AndAlso TAKOU_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP011", "為替振込明細表(他行為替)(MT代行)")
                End If

                '為替振込明細表(自振ロギング登録)
                If bRet = True AndAlso JIFURI_KEN > 0 Then
                    bRet = PrintKawaseFurikomiMeisai(ExeRepo, "KFSP012", "為替振込明細表(自振ロギング登録)(MT代行)")
                End If

            End If

            If bRet = False Then
                If jobMessage = "" Then
                    Call MainLOG.UpdateJOBMASTbyErr("ログ参照")
                Else
                    Call MainLOG.UpdateJOBMASTbyErr(jobMessage)
                End If

                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

                ' ロールバック
                MainDB.Rollback()
            Else

                If iRet = 1 Then
                    jobMessage = "対象データ０件"
                End If

                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

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
            If Not MainDB Is Nothing Then
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

                ' ロールバック
                MainDB.Rollback()
            End If

            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "MT代行発信データ作成処理(終了)", "成功")
        End Try

        Return 0

    End Function

    ''' <summary>
    ''' 発信待ちになっているスケジュールの発信フラグを更新します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReturnFlg() As Integer
        Dim Ret As Integer = 0
        Dim OraReader As CASTCommon.MyOracleReader
        Dim SQL As New StringBuilder

        Try
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(GetScheduleSQL()) = True Then
                While OraReader.EOF = False
                    With SQL
                        .Length = 0
                        .Append("update S_SCHMAST set")
                        .Append(" HASSIN_FLG_S = '0'")
                        .Append(" where TORIS_CODE_S = '" & OraReader.GetString("TORIS_CODE_S") & "'")
                        .Append("   and TORIF_CODE_S = '" & OraReader.GetString("TORIF_CODE_S") & "'")
                        .Append("   and FURI_DATE_S = '" & OraReader.GetString("FURI_DATE_S") & "'")
                        .Append("   and MOTIKOMI_SEQ_S = " & OraReader.GetString("MOTIKOMI_SEQ_S"))
                    End With

                    Ret += MainDB.ExecuteNonQuery(SQL)

                    OraReader.NextRead()
                End While
            Else
                Ret = 0
            End If

            MainLOG.Write("発信待ち取消", "成功", Ret & "件")

            MainDB.Commit()

            Return 0

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write("発信待ち取消", "失敗", ex.ToString)
            Return -1
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 設定ファイルの読み込みを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
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

        ini_info.DAT_PATH = CASTCommon.GetFSKJIni("COMMON", "DAT")           'DATのパス
        If ini_info.DAT_PATH = "err" OrElse ini_info.DAT_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            jobMessage = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
            Return False
        End If

        ini_info.DEN_PATH = CASTCommon.GetFSKJIni("COMMON", "DEN")           'DENのパス
        If ini_info.DEN_PATH = "err" OrElse ini_info.DEN_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN")
            jobMessage = "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN"
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KAWASE", "RIENTANAME")       'リエンタファイル名
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル名 分類:KAWASE 項目:RIENTANAME")
            jobMessage = "設定ファイル取得失敗 項目名:リエンタファイル名 分類:KAWASE 項目:RIENTANAME"
            Return False
        End If

        ini_info.FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")         'FTRのパス
        If ini_info.FTR = "err" OrElse ini_info.FTR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:FTRフォルダ 分類:COMMON 項目:FTR")
            jobMessage = "設定ファイル取得失敗 項目名:FTRフォルダ 分類:COMMON 項目:FTR"
            Return False
        End If

        ini_info.FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")         'FTRANPのパス
        If ini_info.FTRANP = "err" OrElse ini_info.FTRANP = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:FTRANPフォルダ 分類:COMMON 項目:FTRANP")
            jobMessage = "設定ファイル取得失敗 項目名:FTRANPフォルダ 分類:COMMON 項目:FTRANP"
            Return False
        End If

        ini_info.RSV2_S_KAWASE_HONSITEN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_HONSITEN")
        If ini_info.RSV2_S_KAWASE_HONSITEN = "err" OrElse ini_info.RSV2_S_KAWASE_HONSITEN = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替振込明細表(本支店為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_HONSITEN")
            jobMessage = "設定ファイル取得失敗 項目名:為替振込明細表(本支店為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_HONSITEN"
            Return False
        End If

        ini_info.RSV2_S_KAWASE_TAKOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_TAKOU")
        If ini_info.RSV2_S_KAWASE_TAKOU = "err" OrElse ini_info.RSV2_S_KAWASE_TAKOU = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替振込明細表(他行為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_TAKOU")
            jobMessage = "設定ファイル取得失敗 項目名:為替振込明細表(他行為替)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_TAKOU"
            Return False
        End If

        ini_info.RSV2_S_KAWASE_LOGGING = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_KAWASE_LOGGING")
        If ini_info.RSV2_S_KAWASE_LOGGING = "err" OrElse ini_info.RSV2_S_KAWASE_LOGGING = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:為替振込明細表(ロギング)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_LOGGING")
            jobMessage = "設定ファイル取得失敗 項目名:為替振込明細表(ロギング)印刷要否 分類:RSV2_V1.0.0 項目:S_KAWASE_LOGGING"
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' MT代行発信データの作成処理を行います。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MakeFurikomiData() As Integer

        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        'Dim SQL As StringBuilder = New StringBuilder(256)

        strWRK_FILE_NAME = Path.Combine(ini_info.DAT_PATH, "WRK_" & ini_info.RIENTA_FILENAME)
        Dim lngRecordcount As Long
        Try
            MainLOG.Write("(振込発信データ格納)開始", "成功")

            OraSchReader = New CASTCommon.MyOracleReader(MainDB)

            Dim Key As FKeyInfo = Nothing

            'フォルダの存在確認
            If Directory.Exists(ini_info.DEN_PATH) = False Then
                'フォルダの作成
                Directory.CreateDirectory(ini_info.DEN_PATH)
            End If
            'ファイルのオープン
            If File.Exists(strWRK_FILE_NAME) = True Then
                File.Delete(strWRK_FILE_NAME)
            End If

            'ファイルフォーマットの定義
            Dim SoufuriFormat As New CAstFormat.CFormatSoufuriDen
            Dim SoufuriStreamWriter As New StreamWriter(strWRK_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            If GetTenmast(ini_info.JIKINKO_CODE, ini_info.HONBU_CODE, JIKINKO_NNAME, HONBU_NNAME, JIKINKO_KNAME, HONBU_KNAME) = False Then
                MainLOG.Write("(契約金融機関名取得)", "失敗", "金融機関コード:" & ini_info.JIKINKO_CODE & _
                              "本部支店コード:" & ini_info.HONBU_CODE)
                Return -1
            End If
            lngRecordcount = haskey.RecordNo
            Try
                '--------------------------------------------
                'ヘッダーレコードの書込み
                '--------------------------------------------
                SoufuriFormat.SOUDEN_REC1.SD1 = "00000000"                              'ブロック番号
                SoufuriFormat.SOUDEN_REC1.SD2 = PARAMETER.HASSIN_DATE                          '発信指定日
                SoufuriFormat.SOUDEN_REC1.SD3 = ini_info.JIKINKO_CODE & Format(haskey.Kaiji, "00")    'MT番号
                SoufuriFormat.SOUDEN_REC1.SD4 = fn_NULL_CHG(JIKINKO_KNAME, 15, " ")                           '依頼組合名
                SoufuriFormat.SOUDEN_REC1.SD5 = ini_info.JIKINKO_CODE                   '依頼組合コード
                SoufuriFormat.SOUDEN_REC1.SD6 = Space(359)                              'ダミー

                SoufuriStreamWriter.Write(SoufuriFormat.SOUDEN_REC1.Data)

                If OraSchReader.DataReader(GetScheduleSQL) = True Then

                    Do While OraSchReader.EOF = False
                        ' キー初期化
                        Key.Init()

                        ' 最初のキー設定
                        Call Key.SetOraDataKessai(OraSchReader)

                        MainLOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                        MainLOG.FuriDate = Key.FURI_DATE

                        ' 明細マスタから，振込データを作成する
                        If GetFurikomiData(Key, SoufuriFormat, SoufuriStreamWriter) = False Then
                            Return -1
                        End If

                        ' スケジュールマスタの更新処理 
                        If UpdateSchMast(Key) = False Then
                            Return -1
                        End If

                        ' 対象データの次レコードを読込む
                        MainLOG.Write("(振込発信データ格納)", "成功", "取引先コード：" & MainLOG.ToriCode & "　振込日:" & MainLOG.FuriDate)
                        OraSchReader.NextRead()
                        If OraSchReader.EOF = False Then
                            ' キー初期化
                            Key.Init()

                            ' キーの再設定
                            Call Key.SetOraDataKessai(OraSchReader)

                        End If
                    Loop
                    '--------------------------------------------
                    'エンドレコードの書込み
                    '--------------------------------------------
                    SoufuriFormat.SOUDEN_REC9.SD1 = "EOF" & Space(5)                                 'ブロック番号
                    SoufuriFormat.SOUDEN_REC9.SD2 = Format(plngJyunKen, "000000")               '順電文数
                    SoufuriFormat.SOUDEN_REC9.SD3 = Format(plngJyunKin, "000000000000")         '順電文合計金額
                    SoufuriFormat.SOUDEN_REC9.SD4 = Format(plngSakiKen, "000000")               '先振電文数
                    SoufuriFormat.SOUDEN_REC9.SD5 = Format(plngSakiKin, "000000000000")         '先振電文合計金額
                    SoufuriFormat.SOUDEN_REC9.SD6 = "000000"                                    '逆電文数
                    SoufuriFormat.SOUDEN_REC9.SD7 = "000000000000"                              '逆電文合計金額
                    SoufuriFormat.SOUDEN_REC9.SD8 = "000000"                                    '取立\0電文数
                    SoufuriFormat.SOUDEN_REC9.SD9 = "000000"                                    '通信電文数
                    SoufuriFormat.SOUDEN_REC9.SD10 = Format(plngJyunKen + plngSakiKen, "000000") '合計電文数
                    SoufuriFormat.SOUDEN_REC9.SD11 = Space(320)                                 '予備

                    SoufuriStreamWriter.Write(SoufuriFormat.SOUDEN_REC9.Data)
                End If

                If lngRecordcount = haskey.RecordNo Then
                    MainLOG.Write("(振込発信データ格納)", "失敗", "件数０件")
                    Return 1
                End If
            Catch ex As Exception
            Finally
                SoufuriStreamWriter.Close()
            End Try

            MainLOG.Write("(振込発信データ格納)終了", "成功")
            Return 0
        Catch ex As Exception
            MainLOG.Write("(振込発信データ格納)", "失敗", ex.Message)
            Return -1
        Finally
            If Not OraSchReader Is Nothing Then OraSchReader.Close()

        End Try
    End Function

    ''' <summary>
    ''' 対象のスケジュールを取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetScheduleSQL() As StringBuilder
        Dim SQL As New StringBuilder
        SQL.AppendLine("SELECT")
        SQL.AppendLine(" TORIS_CODE_S")
        SQL.AppendLine(",TORIF_CODE_S")
        SQL.AppendLine(",BAITAI_CODE_S")
        SQL.AppendLine(",FURI_DATE_S")
        SQL.AppendLine(",MOTIKOMI_SEQ_S")
        SQL.AppendLine(",SYUBETU_S")
        SQL.AppendLine(",FURI_CODE_S")
        SQL.AppendLine(",KIGYO_CODE_S")
        SQL.AppendLine(",ITAKU_CODE_S")
        SQL.AppendLine(",SOUSIN_KBN_S")

        SQL.AppendLine(",ITAKU_KNAME_T")
        SQL.AppendLine(",ITAKU_NNAME_T")
        SQL.AppendLine(",SYUMOKU_CODE_T")
        SQL.AppendLine(",FUKA_CODE_T")
        SQL.AppendLine(",TORIMATOME_SIT_T")
        SQL.AppendLine(",BIKOU1_T")
        SQL.AppendLine(",BIKOU2_T")

        SQL.AppendLine(",KIN_KNAME_N")
        SQL.AppendLine(",SIT_KNAME_N")

        SQL.AppendLine(" FROM S_TORIMAST")
        SQL.AppendLine("     ,S_SCHMAST")
        SQL.AppendLine("     ,TENMAST")

        SQL.AppendLine(" WHERE TORIS_CODE_S = TORIS_CODE_T")
        SQL.AppendLine("   AND TORIF_CODE_S = TORIF_CODE_T")
        SQL.AppendLine(" AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
        SQL.AppendLine(" AND TORIMATOME_SIT_T = SIT_NO_N(+)")
        SQL.AppendLine("   AND FURI_DATE_S >= " & SQ(PARAMETER.HASSIN_DATE))

        ' 総合振込
        SQL.AppendLine("   AND ((SYUBETU_T = '21' AND FURI_DATE_S BETWEEN " & SQ(PARAMETER.HASSIN_DATE) & " AND " & SQ(PARAMETER.SOUFURI_DATE) & ")")
        ' 給与振込，賞与振込
        SQL.AppendLine("    OR (SYUBETU_T <> '21' AND FURI_DATE_S BETWEEN " & SQ(PARAMETER.HASSIN_DATE) & " AND " & SQ(PARAMETER.KYUFURI_DATE) & "))")

        SQL.AppendLine("   AND TOUROKU_FLG_S = '1'")
        SQL.AppendLine("   AND HASSIN_FLG_S = '2'")
        SQL.AppendLine("   AND TYUUDAN_FLG_S = '0'")

        SQL.AppendLine(" ORDER BY FURI_DATE_S, SOUSIN_KBN_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")

        Return SQL
    End Function

    ''' <summary>
    ''' MT代行発信データの明細情報を設定します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="SoufuriFormat"></param>
    ''' <param name="SoufuriStreamWriter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFurikomiData(ByRef Key As FKeyInfo, ByRef SoufuriFormat As CAstFormat.CFormatSoufuriDen, ByVal SoufuriStreamWriter As StreamWriter) As Boolean

        Dim SQL As StringBuilder = New StringBuilder(256)
        Dim OraMeiReader As CASTCommon.MyOracleReader       ' 明細マスタ
        OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
        ' 振込日
        Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

        ' 振込日の１営業日前，２営業日前
        Dim Zen1Day As String = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")
        Try
            SQL.AppendLine("SELECT ")
            SQL.AppendLine(" FURIKIN_K")
            SQL.AppendLine(",TESUU_KIN_K")
            SQL.AppendLine(",KEIYAKU_KIN_K")
            SQL.AppendLine(",KEIYAKU_SIT_K")
            '科目はFURI_DATA_Kの値を設定する
            SQL.AppendLine(",SUBSTRB(FURI_DATA_K, 43, 1) KEIYAKU_KAMOKU_K")
            SQL.AppendLine(",KEIYAKU_KOUZA_K")
            SQL.AppendLine(",KEIYAKU_KNAME_K")
            SQL.AppendLine(",SINKI_CODE_K")
            SQL.AppendLine(",TEKIYO_KBN_K")
            SQL.AppendLine(",KTEKIYO_K")
            SQL.AppendLine(",NTEKIYO_K")
            SQL.AppendLine(",JYUYOUKA_NO_K")
            'EDI情報用 識別表示
            SQL.AppendLine(",SUBSTRB(FURI_DATA_K, 113, 1) SIKIBETU")

            SQL.AppendLine(",KIN_KNAME_N")
            SQL.AppendLine(",SIT_KNAME_N")

            SQL.AppendLine(" FROM S_MEIMAST")
            SQL.AppendLine("     ,TENMAST")

            SQL.AppendLine(" WHERE TORIS_CODE_K   = " & SQ(Key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_K   = " & SQ(Key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_K    = " & SQ(Key.FURI_DATE))
            SQL.AppendLine("   AND MOTIKOMI_SEQ_K = " & Key.MOTIKOMI_SEQ)
            SQL.AppendLine("   AND DATA_KBN_K   = '2'")
            SQL.AppendLine("   AND FURIKETU_CODE_K = 0")
            SQL.AppendLine("   AND FURIKIN_K >= 0")

            SQL.AppendLine("   AND KEIYAKU_KIN_K = KIN_NO_N(+)")
            SQL.AppendLine("   AND KEIYAKU_SIT_K = SIT_NO_N(+)")
            SQL.AppendLine(" ORDER BY RECORD_NO_K")



            If OraMeiReader.DataReader(SQL) = True Then

                Dim KData As New CAstFormat.ClsFormSOU.PrnSoufuriData
                Dim strKeiyakuKinKname As String = ""
                Dim strKeiyakuKinNname As String = ""
                Dim strKeiyakuSitKname As String = ""
                Dim strKeiyakuSitNname As String = ""
                Dim strSyumokuCode As String = ""
                Dim intJyunKen As Integer = 0
                Dim strSyubetu As String = ""
                Do While OraMeiReader.EOF = False
                    strKeiyakuKinKname = ""
                    strKeiyakuSitKname = ""
                    strSyubetu = ""
                    '帳票印刷用構造体初期化
                    KData.Init()
                    If OraMeiReader.GetInt64("FURIKIN_K") = 0 Then
                        Key.TotalKen += 1
                    Else

                        '--------------------------------------------
                        'データレコードの書込み
                        '--------------------------------------------
                        SoufuriFormat.SOUDEN_REC2.SD1 = "00" & Format(haskey.RecordNo, "000000")     'ブロック番号
                        SoufuriFormat.SOUDEN_REC2.SD2 = Space(1)                                '決済回数
                        SoufuriFormat.SOUDEN_REC2.SD3 = Space(1)                                '予備1
                        SoufuriFormat.SOUDEN_REC2.SD4 = Key.FURI_DATE                           '取扱日

                        '適用種別設定
                        Select Case PARAMETER.HASSIN_DATE
                            Case Key.FURI_DATE
                                ' 当日
                                Key.TEKIYOU_SYUBETU = "ﾌﾘｺﾐ"
                                plngJyunKen += 1
                                plngJyunKin += OraMeiReader.GetInt64("FURIKIN_K")
                            Case Zen1Day
                                ' １営業日前
                                Key.TEKIYOU_SYUBETU = "ｻｷﾌﾘ"
                                plngSakiKen += 1
                                plngSakiKin += OraMeiReader.GetInt64("FURIKIN_K")
                            Case Is <= Zen2Day
                                ' ２営業日前以前
                                Select Case Key.SYUBETU
                                    Case "21"
                                        ' 総振
                                        Key.TEKIYOU_SYUBETU = "ｻｷﾌﾘ"
                                    Case "11"
                                        ' 給与
                                        Key.TEKIYOU_SYUBETU = "ｷﾕｳﾖ"
                                    Case "12"
                                        ' 賞与
                                        Key.TEKIYOU_SYUBETU = "ｼﾖｳﾖ"
                                End Select
                                plngSakiKen += 1
                                plngSakiKin += OraMeiReader.GetInt64("FURIKIN_K")
                        End Select
                        '取扱通信種目コード
                        Select Case Key.TEKIYOU_SYUBETU
                            Case "ﾌﾘｺﾐ"
                                Select Case Key.SYUMOKU_CODE
                                    Case "02", "03", "04"
                                        '振込公金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1074"
                                    Case "01"
                                        '振込国庫金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1054"
                                    Case Else
                                        '振込一般
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1022"
                                End Select

                            Case "ｻｷﾌﾘ"
                                Select Case Key.SYUMOKU_CODE
                                    Case "02", "03", "04"
                                        '先振公金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1174"
                                    Case "01"
                                        '先振国庫金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1154"
                                    Case Else
                                        '先振一般
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1122"
                                End Select

                            Case "ｷﾕｳﾖ"
                                Select Case Key.SYUMOKU_CODE
                                    Case "02", "03", "04"
                                        '給与公金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1271"
                                    Case "01"
                                        '給与国庫金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1251"
                                    Case Else
                                        '給与一般
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1211"
                                End Select

                            Case "ｼﾖｳﾖ"
                                Select Case Key.SYUMOKU_CODE
                                    Case "02", "03", "04"
                                        '賞与公金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1272"
                                    Case "01"
                                        ''賞与国庫金
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1252"
                                    Case Else
                                        '賞与一般
                                        SoufuriFormat.SOUDEN_REC2.SD5 = "1212"
                                End Select
                        End Select
                        SoufuriFormat.SOUDEN_REC2.SD6 = Key.FUKA_CODE                         '付加コード
                        '契約金融機関名取得
                        If GetTenmast(OraMeiReader.GetString("KEIYAKU_KIN_K"), OraMeiReader.GetString("KEIYAKU_SIT_K"), strKeiyakuKinNname, _
                                      strKeiyakuSitNname, strKeiyakuKinKname, strKeiyakuSitKname) = False Then
                            MainLOG.Write("(契約金融機関名取得)", "失敗", "金融機関コード:" & OraMeiReader.GetString("KEIYAKU_KIN_K") & _
                                          "支店コード:" & OraMeiReader.GetString("KEIYAKU_SIT_K"))
                            Return False
                        End If
                        SoufuriFormat.SOUDEN_REC2.SD7 = fn_NULL_CHG(strKeiyakuKinKname, 15, " ")        '受信金融機関名
                        SoufuriFormat.SOUDEN_REC2.SD8 = Space(1)                                        '区切1
                        SoufuriFormat.SOUDEN_REC2.SD9 = fn_NULL_CHG(strKeiyakuSitKname, 15, " ")        '受信店舗名
                        SoufuriFormat.SOUDEN_REC2.SD10 = Format(OraMeiReader.GetInt64("FURIKIN_K"), "000000000000000")  '金額
                        SoufuriFormat.SOUDEN_REC2.SD11 = fn_NULL_CHG(JIKINKO_KNAME, 15, " ")            '発信組合
                        SoufuriFormat.SOUDEN_REC2.SD12 = Space(1)                                       '区切2
                        SoufuriFormat.SOUDEN_REC2.SD13 = fn_NULL_CHG(Key.TUKESIT_KNAME, 15, " ")              '本部店名
                        SoufuriFormat.SOUDEN_REC2.SD14 = Space(7)                                      '銀行間手数料
                        SoufuriFormat.SOUDEN_REC2.SD15 = OraMeiReader.GetString("KEIYAKU_KAMOKU_K") _
                            & OraMeiReader.GetString("KEIYAKU_KOUZA_K") & Space(8)                      '番号欄
                        SoufuriFormat.SOUDEN_REC2.SD16 = Space(20)  'EDI情報 
                        '2017/04/05 saitou 近畿産業信組(RSV2標準) MODIFY ------------------------------------- START
                        '契約者名の開始がスペースの場合にSKCでフォーマットエラーになるため、前後空白を削除する
                        SoufuriFormat.SOUDEN_REC2.SD17 = fn_NULL_CHG(Trim(OraMeiReader.GetString("KEIYAKU_KNAME_K")), 48, " ")  '契約者名
                        'SoufuriFormat.SOUDEN_REC2.SD17 = fn_NULL_CHG(OraMeiReader.GetString("KEIYAKU_KNAME_K"), 48, " ")  '契約者名
                        '2017/04/05 saitou 近畿産業信組(RSV2標準) MODIFY ------------------------------------- END
                        SoufuriFormat.SOUDEN_REC2.SD18 = fn_NULL_CHG(Key.ITAKU_KNAME, 48, " ")          '委託者名
                        SoufuriFormat.SOUDEN_REC2.SD19 = fn_NULL_CHG(Key.BIKOU1, 48, " ")        '備考１
                        SoufuriFormat.SOUDEN_REC2.SD20 = fn_NULL_CHG(Key.BIKOU2, 48, " ")        '備考２
                        SoufuriFormat.SOUDEN_REC2.SD21 = Space(15)         '発信番号
                        SoufuriFormat.SOUDEN_REC2.SD22 = Space(15)         '照会番号
                        SoufuriFormat.SOUDEN_REC2.SD23 = Space(33)         '予備２

                        SoufuriStreamWriter.Write(SoufuriFormat.SOUDEN_REC2.Data)

                        haskey.RecordNo += 1

                        Key.TotalKen += 1
                        Key.TotalKin += OraMeiReader.GetInt64("FURIKIN_K")
                        '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
                        Key.TesuuKin += OraMeiReader.GetInt64("TESUU_KIN_K")
                        '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END


                        Select Case Key.SOUSIN_KBN
                            Case "2"
                                'CSVリエンタ(信組用)
                                If OraMeiReader.GetString("KEIYAKU_KIN_K") = ini_info.JIKINKO_CODE Then
                                    Key.JikouKen += 1
                                    Key.JikouKin += OraMeiReader.GetInt64("FURIKIN_K")
                                    JIKOU_KEN += 1
                                Else
                                    Key.TakouKen += 1
                                    TAKOU_KEN += 1
                                    Key.TakouKin += OraMeiReader.GetInt64("FURIKIN_K")
                                End If
                            Case Else
                                '送信区分異常
                                jobMessage = "送信区分異常：" & Key.SOUSIN_KBN
                                Return False
                        End Select

                        ' 発信マスタへInsert
                        If InsertHassinMast(Key, SoufuriFormat.SOUDEN_REC2.Data) = False Then
                            Return False
                        End If
                    End If
                    ' 振込発信データのリスト作成
                    KData.HassinDate = PARAMETER.HASSIN_DATE
                    KData.TorisCode = Key.TORIS_CODE
                    KData.TorifCode = Key.TORIF_CODE
                    KData.ToriNName = Key.ITAKU_NNAME
                    KData.FuriDate = Key.FURI_DATE
                    KData.Baitai = Key.BAITAI_CODE
                    KData.Syubetu = SoufuriFormat.SOUDEN_REC2.SD5
                    KData.TekiyouSyubetu = Key.TEKIYOU_SYUBETU
                    KData.HonbuTenCode = Key.TORIMATOME_SIT
                    KData.HonbuTenName = Key.TUKESIT_KNAME
                    KData.KeiyakuName = SoufuriFormat.SOUDEN_REC2.SD17
                    KData.FurikomiKinCode = OraMeiReader.GetString("KEIYAKU_KIN_K")
                    KData.FurikomiKinName = SoufuriFormat.SOUDEN_REC2.SD7
                    KData.FurikomiSitCode = OraMeiReader.GetString("KEIYAKU_SIT_K")
                    KData.FurikomiSitName = SoufuriFormat.SOUDEN_REC2.SD9
                    KData.Kamoku = OraMeiReader.GetString("KEIYAKU_KAMOKU_K")
                    KData.KouzaNo = OraMeiReader.GetString("KEIYAKU_KOUZA_K")
                    KData.FurikomiKIN = SoufuriFormat.SOUDEN_REC2.SD10
                    KData.Bikou1 = SoufuriFormat.SOUDEN_REC2.SD19
                    KData.Bikou2 = SoufuriFormat.SOUDEN_REC2.SD20
                    KData.TukekaeKinName = Key.TUKEKIN_KNAME
                    KData.TukekaeSitName = Key.TUKESIT_KNAME

                    HassinList.Add(KData)

                    OraMeiReader.NextRead()
                Loop

            End If
            OraMeiReader.Close()

            Return True

        Catch ex As Exception
            MainLOG.Write("(データレコード作成)", "失敗", "回次:" & haskey.Kaiji & "レコードNo:" & haskey.RecordNo)
            Return False
        Finally
            If Not OraMeiReader Is Nothing Then OraMeiReader.Close()
        End Try
    End Function

    ''' <summary>
    ''' 金融機関マスタより本部名の取得を行います。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' 振込発信マスタの登録を行います。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="StrData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertHassinMast(ByVal Key As FKeyInfo, ByVal StrData As String) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine("INSERT INTO HASSINMAST(")
            SQL.AppendLine(" SYORI_DATE_FH")
            SQL.AppendLine(",TIME_STAMP_FH")
            SQL.AppendLine(",KAIJI_FH")
            SQL.AppendLine(",RECORD_NO_FH")
            SQL.AppendLine(",FILE_NAME_FH")
            SQL.AppendLine(",TORIS_CODE_FH")
            SQL.AppendLine(",TORIF_CODE_FH")
            SQL.AppendLine(",FURI_DATE_FH")
            SQL.AppendLine(",MOTIKOMI_SEQ_FH")
            SQL.AppendLine(",KAMOKU_CODE_FH")
            SQL.AppendLine(",OPE_CODE_FH")
            SQL.AppendLine(",DENBUN_ALL_FH")
            SQL.AppendLine(",ERR_CODE_FH")
            SQL.AppendLine(",ERR_MSG_FH")
            SQL.AppendLine(",SAKUSEI_DATE_FH")
            SQL.AppendLine(",KOUSIN_DATE_FH")
            SQL.AppendLine(") VALUES (")
            SQL.AppendLine(" " & SQ(strDate))                                   ' 処理日
            SQL.AppendLine("," & SQ(String.Concat(strDate, strTime)))           ' タイムスタンプ
            SQL.AppendLine("," & SQ(haskey.Kaiji))                              ' 回次
            SQL.AppendLine("," & SQ(haskey.RecordNo))                           ' 通番
            SQL.AppendLine("," & SQ(ini_info.RIENTA_FILENAME))                  ' リエンタファイル名
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' 取引先主コード
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' 取引先副コード
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' 振込日
            SQL.AppendLine("," & SQ(Key.MOTIKOMI_SEQ))                          ' 持込SEQ
            SQL.AppendLine(",'00' ")                                            ' 科目コード
            SQL.AppendLine(",'000' ")                                           ' オペコード
            SQL.AppendLine("," & SQ(StrData))                                   ' 個別データ
            SQL.AppendLine("," & SQ(""))                                        ' 結果コード
            SQL.AppendLine("," & SQ(""))                                        ' エラーメッセージ
            SQL.AppendLine("," & SQ(strDate))                                   ' 作成日
            SQL.AppendLine("," & SQ("00000000"))                                ' 更新日
            SQL.AppendLine(")")

            Call MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MainLOG.Write("(振込発信マスタ登録)", "失敗", ex.Message)
            Return False
        Finally
        End Try

        Return True

    End Function

    ''' <summary>
    ''' スケジュールマスタの更新を行います。
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateSchMast(ByVal key As FKeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)

        Try
            SQL.AppendLine(" UPDATE S_SCHMAST SET")
            SQL.AppendLine(" HASSIN_DATE_S = " & SQ(strDate))
            SQL.AppendLine(",FURI_KEN_S    = " & key.TotalKen)
            SQL.AppendLine(",FURI_KIN_S    = " & key.TotalKin)
            SQL.AppendLine(",HASSIN_FLG_S  = '1'")
            SQL.AppendLine(",HASSIN_TIME_STAMP_S = " & SQ(strDate & strTime))
            '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
            SQL.AppendLine(",TESUU_KIN_S   = " & key.TesuuKin)
            '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END
            SQL.AppendLine(" WHERE TORIS_CODE_S = " & SQ(key.TORIS_CODE))
            SQL.AppendLine("   AND TORIF_CODE_S = " & SQ(key.TORIF_CODE))
            SQL.AppendLine("   AND FURI_DATE_S  = " & SQ(key.FURI_DATE))
            SQL.AppendLine("   AND MOTIKOMI_SEQ_S = " & SQ(key.MOTIKOMI_SEQ))

            If MainDB.ExecuteNonQuery(SQL) = 0 Then
                MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & _
                              " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE & " 持込回数：" & key.MOTIKOMI_SEQ)
            Else
                MainLOG.Write("スケジュールマスタ更新", "成功", "取引先主コード：" & key.TORIS_CODE & _
                              " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE & " 持込回数：" & key.MOTIKOMI_SEQ)
            End If

        Catch ex As Exception
            MainLOG.Write("スケジュールマスタ更新", "失敗", "取引先主コード：" & key.TORIS_CODE & _
                          " 取引先副コード：" & key.TORIF_CODE & " 振込日：" & key.FURI_DATE & " 持込回数：" & key.MOTIKOMI_SEQ & " " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' 回次を取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetKaiji() As Boolean

        Dim SQL As New StringBuilder()
        Dim OraReader As MyOracleReader = Nothing

        Try
            MainLOG.Write("(回次取得)開始", "成功", "")

            SQL.Append("SELECT NVL(MAX(KAIJI_FH),0) AS MAX_KAIJI FROM HASSINMAST")
            SQL.Append(" WHERE SYORI_DATE_FH = " & SQ(strDate))

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                haskey.Kaiji = OraReader.GetInt("MAX_KAIJI") + 1
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

    ''' <summary>
    ''' レコード番号を取得します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetRecordNo() As Boolean

        Dim SQL As New StringBuilder()
        Dim OraReader As MyOracleReader = Nothing

        Try
            MainLOG.Write("(レコード番号取得)開始", "成功", "")

            SQL.Append("SELECT NVL(MAX(RECORD_NO_FH),0) AS MAX_RECORD_NO FROM HASSINMAST")
            SQL.Append(" WHERE SYORI_DATE_FH = " & SQ(strDate))

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                haskey.RecordNo = OraReader.GetInt("MAX_RECORD_NO") + 1
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("(レコード番号取得)", "失敗", ex.ToString)
            Return False
        Finally
            MainLOG.Write("(レコード番号取得)終了", "成功", "")
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

    ''' <summary>
    ''' 為替振込明細表の印刷処理を行います。
    ''' </summary>
    ''' <param name="ExeRepo"></param>
    ''' <param name="ReportID"></param>
    ''' <param name="ReportName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function PrintKawaseFurikomiMeisai(ByVal ExeRepo As CAstReports.ClsExecute, ByVal ReportID As String, ByVal ReportName As String) As Boolean
        Dim iRet As Integer
        Dim bRet As Boolean = True

        Dim PrnMeisai As New ClsPrnKawaseFurikomiMeisai(ReportID, ReportName)

        PrnMeisai.OraDB = MainDB
        PrnMeisai.CreateCsvFile()

        '明細行出力
        iRet = PrnMeisai.OutputCSVKekka(HassinList, ini_info.JIKINKO_CODE, PARAMETER.HASSIN_DATE, strDate, strTime)

        If iRet <> 0 Then
            bRet = False
            MainLOG.Write(ReportName & "出力", "失敗", ReportName & "ＣＳＶ出力に失敗しました。")
        End If

        If Not PrnMeisai Is Nothing And iRet = 0 Then
            PrnMeisai.CloseCsv()

            Select Case ReportID
                Case "KFSP010"
                    '振込日・取引先主コード・取引先副コード・金融機関コード・支店コード・レコード番号順でソート
                    PrnMeisai.SortFile("7.8sjia 4.10sjia 5.2sjia 14.4sjia 15.3sjia 0.6sjia")

                    If ini_info.RSV2_S_KAWASE_HONSITEN = "0" Then
                        MainLOG.Write(ReportName & "印刷", "終了", "印刷不要")
                        Return bRet
                    End If

                Case "KFSP011"
                    '振込日・取引先主コード・取引先副コード・レコード番号順でソート
                    PrnMeisai.SortFile("7.8sjia 4.10sjia 5.2sjia 0.6sjia")

                    If ini_info.RSV2_S_KAWASE_TAKOU = "0" Then
                        MainLOG.Write(ReportName & "印刷", "終了", "印刷不要")
                        Return bRet
                    End If

                Case "KFSP012"

                    If ini_info.RSV2_S_KAWASE_LOGGING = "0" Then
                        MainLOG.Write(ReportName & "印刷", "終了", "印刷不要")
                        Return bRet
                    End If
            End Select

            '印刷バッチ呼び出し
            Dim param As String = ""

            'パラメータ設定：ログイン名、ＣＳＶファイル名
            param = MainLOG.UserID & "," & PrnMeisai.FileName & ",2"

            iRet = ExeRepo.ExecReport(ReportID & ".EXE", param)

            If iRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case iRet
                    Case -1
                        jobMessage = ReportName & "印刷対象０件。"
                    Case Else
                        jobMessage = ReportName & "印刷失敗。エラーコード：" & iRet
                End Select

                MainLOG.Write(ReportName & "印刷", "失敗", jobMessage)
                bRet = False
            End If

        End If

        Return bRet
    End Function

    Public Function fn_NULL_CHG(ByVal astrOBJ As Object, ByVal aintKETA As Integer, ByVal astrATAI As String) As String
        '============================================================================
        'NAME           :fn_NULL_CHG
        'Parameter      :astrOBJ：オブジェクト／aintKETA：桁数／astrATAI：値
        'Description    :astrOBJの値がNULL値だった場合、aintKETA桁分astrATAIで置き換える
        'Return         :置き換えた結果
        'Create         :2004/08/17
        'Update         :2008/08/19
        '============================================================================
        If astrOBJ Is Nothing OrElse astrOBJ Is DBNull.Value Then
            fn_NULL_CHG = New String(CChar(astrATAI), aintKETA)
            '************************************
        Else
            fn_NULL_CHG = CStr(astrOBJ)
        End If

    End Function

    ''' <summary>
    ''' コード変換を行います。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function fn_CODE_CHANGE() As Boolean
        Dim intKEKKA As Integer
        Dim strCODE_KBN As String = "4"
        intKEKKA = ConvertFileFtranP("PUTRAND", strWRK_FILE_NAME, strFILE_NAME, Path.Combine(ini_info.FTR, "400.P"))

        Select Case intKEKKA
            Case 0
                Return True
            Case Else
                Return False
        End Select
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
                .Append("""" & ini_info.FTR & "FUSION" & """")
                .Append(" ; kanji 83_jis")
                .Append(" " & strGetOrPut & " ")
                .Append("""" & strInFileName & """" & " ")
                .Append("""" & strOutFileName & """" & " ")
                .Append(" ++" & """" & strPFileName & """")
            End With

            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(ini_info.FTRANP, "FP.EXE")
            ProcInfo.WorkingDirectory = ini_info.FTRANP
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

#End Region

End Class
