Imports System.Globalization
Imports System.Text
Imports System.Windows.Forms
Imports CASTCommon


''' <summary>
''' 振込発信CSVリエンタ作成　メインクラス
''' </summary>
''' <remarks></remarks>
Public Class ClsFurikomiDataCreate

#Region "クラス定数"

    Private Const FD_COUNT_LIMIT As Integer = 2000
    Private Const msgTitle As String = "振込発信CSVリエンタ作成(KFS120)"


#End Region

#Region "クラス変数"

    Public MainLOG As New CASTCommon.BatchLOG("KFS120", "振込発信CSVリエンタ作成")
    Private MainDB As CASTCommon.MyOracle

    ' パブリックフォーマット
    Private FmtComm As New CAstFormat.CFormat

    Private jobMessage As String = ""          ' ジョブ監視メッセージ

    ' 処理日付
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    ''' <summary>
    ''' iniファイル情報
    ''' </summary>
    ''' <remarks></remarks>
    Structure strcIni
        Dim JIKINKO_CODE As String              ' 自金庫コード
        Dim JIKINKO_NAME As String              ' 自金庫名
        Dim HONBU_CODE As String                ' 本部コード
        Dim RIENTA_PATH As String               ' リエンタファイル作成先
        Dim CSV_PATH As String                  ' CSVフォルダパス

        Dim RSV2_S_SYORIKEKKA_CSVRNT As String  ' 処理結果確認表出力判定
        Dim RSV2_S_MEISAI_CSVRNT As String      ' 総合振込明細表出力判定
        Dim RSV2_S_FBFURITEN_CSVRNT As String   ' FBタンキングデータ振込店別集計表出力判定

        Dim RSV2_HDR_GRP As String              ' ヘッダグループ

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

    Private HONBU_KNAME As String = ""

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
        Dim TUKEKIN_NNAME As String         ' 金融機関名漢字
        Dim TUKESIT_NNAME As String         ' 支店名漢字
        Dim BIKOU1 As String                ' 備考１
        Dim BIKOU2 As String                ' 備考２

        Dim KAMOKU As String                ' 科目
        Dim KOUZA As String                 ' 口座番号

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

        Dim IraiKen As Long                 ' 明細の合計依頼件数
        Dim IraiKin As Long                 ' 明細の合計依頼金額

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
            TUKEKIN_NNAME = ""
            TUKESIT_NNAME = ""
            BIKOU1 = ""
            BIKOU2 = ""

            KAMOKU = ""
            KOUZA = ""


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

            IraiKen = 0
            IraiKin = 0

            MESSAGE = ""

        End Sub

        ' ＤＢからの値を設定（振込発信リエンタ作成用）
        Friend Sub SetOraDataKessai(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S")
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")
            FURI_DATE = oraReader.GetString("FURI_DATE_S")
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
            TUKEKIN_NNAME = oraReader.GetString("KIN_NNAME_N")
            TUKESIT_NNAME = oraReader.GetString("SIT_NNAME_N")
            BIKOU1 = oraReader.GetString("BIKOU1_T")
            BIKOU2 = oraReader.GetString("BIKOU2_T")

            KAMOKU = oraReader.GetString("KAMOKU_T")
            KOUZA = oraReader.GetString("KOUZA_T")

        End Sub

    End Structure

    Private Structure ArrayListItem
        Dim EachKEN As Decimal
        Dim EachKIN As Decimal
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
    End Structure

    Private RecInfo As New ArrayList
    Private FDTempDataDirectory As String
    Private MaxFiles As Integer
    Private LimitKen As Integer

    Private alPrintList As ArrayList

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

            '--------------------------------------------------
            '帳票印刷用リストを初期化
            '--------------------------------------------------
            alPrintList = New ArrayList

            Return True

        Catch ex As Exception
            MainLOG.Write("(初期処理)開始", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write("(初期処理)終了", "成功")
        End Try

    End Function

    ''' <summary>
    ''' 振込発信CSVリエンタのメイン処理を行います。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Main() As Integer

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
        FmtComm.Oracle = MainDB

        Dim bRet As Boolean = True
        Dim iRet As Integer

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "振込発信CSVリエンタ作成処理(開始)", "成功")
            'MainLOG.Write("(主処理)開始", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


            MainDB.BeginTrans()     ' トランザクション開始

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("(主処理)", "失敗", "振込発信CSVリエンタ作成処理で実行待ちタイムアウト")
                MainLOG.UpdateJOBMASTbyErr("振込発信CSVリエンタ作成処理で実行待ちタイムアウト")
                Return -1
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            '--------------------------------------------------
            ' 回次を取得
            '--------------------------------------------------
            If GetKaiji() = False Then
                MainLOG.Write("(主処理)", "失敗", "回次の取得に失敗しました")
                Return -1
            End If

            '--------------------------------------------------
            ' 本部支店名を取得
            '--------------------------------------------------
            HONBU_KNAME = GetTenmast()

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
            ' リエンタＦＤ作成
            '--------------------------------------------------
            If iRet = 0 Then
                If Not LetFDWriteAction(MaxFiles, RecInfo) Then
                    iRet = -1
                    jobMessage = "媒体書き込み失敗"
                End If
            End If

            If iRet <> 0 Then
                bRet = False
            End If

            '--------------------------------------------------
            ' 帳票出力
            '--------------------------------------------------
            ' 振込発信データが１件以上存在する場合、帳票出力
            If iRet = 0 Then
                Dim ExeRepo As New CAstReports.ClsExecute
                bRet = True

                '処理結果確認表　印刷
                If bRet = True Then
                    bRet = Me.PrintSyoriKekkaList(ExeRepo)
                End If

                '総合振込明細表　印刷
                If bRet = True Then
                    bRet = Me.PrintSoufuriMeisai(ExeRepo)
                End If

                'FBタンキングデータ振込店別集計表　印刷
                If bRet = True Then
                    bRet = Me.PrintFBTenbetuList(ExeRepo)
                End If
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
            MainLOG.Write("(主処理)", "失敗", ex.ToString)
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
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "振込発信リエンタ作成処理(終了)", "成功")
            'MainLOG.Write("(主処理)終了", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
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
    ''' 振込発信CSVリエンタの作成処理を行います。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MakeFurikomiData() As Integer

        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write("(振込発信データ格納)開始", "成功")

            '--------------------------------------------------
            '対象明細数
            '--------------------------------------------------
            MaxFiles = GetFileCount()
            If MaxFiles = 0 Then
                MainLOG.Write("(振込発信データ格納)", "失敗", "件数０件")
                Return 1
            End If

            '--------------------------------------------------
            'ＦＤ用ファイルの作業領域をお掃除
            '--------------------------------------------------
            FDTempDataDirectory = System.IO.Path.Combine(ini_info.CSV_PATH, "CSVRNT_WORK")
            If System.IO.Directory.Exists(FDTempDataDirectory) Then
                For Each FL As String In System.IO.Directory.GetFiles(FDTempDataDirectory)
                    System.IO.File.Delete(FL)
                Next
            Else
                System.IO.Directory.CreateDirectory(FDTempDataDirectory)
            End If

            '--------------------------------------------------
            '該当する取引先を抽出
            '--------------------------------------------------
            OraSchReader = New CASTCommon.MyOracleReader(MainDB)

            RecInfo = New ArrayList
            LimitKen = FD_COUNT_LIMIT

            Dim Key As FKeyInfo = Nothing

            If OraSchReader.DataReader(GetScheduleSQL) = True Then
                While OraSchReader.EOF = False
                    ' キー初期化
                    Key.Init()

                    ' 最初のキー設定
                    Call Key.SetOraDataKessai(OraSchReader)

                    MainLOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                    MainLOG.FuriDate = Key.FURI_DATE

                    ' 明細マスタから発信マスタを作成する
                    If GetFurikomiData(Key) = False Then
                        Return -1
                    End If

                    ' スケジュールマスタの更新処理 
                    If UpdateSchMast(Key) = False Then
                        Return -1
                    End If

                    ' スケジュールを更新したキー情報をリストに追加
                    Me.alPrintList.Add(Key)

                    OraSchReader.NextRead()
                End While

            End If

            If haskey.RecordNo = 0 Then
                MainLOG.Write("(振込発信データ格納)", "失敗", "件数０件")
                Return 1
            End If

        Catch ex As Exception
            MainLOG.Write("(振込発信データ格納)", "失敗", ex.Message)
            Return -1
        Finally
            If Not OraSchReader Is Nothing Then OraSchReader.Close()
            MainLOG.Write("(振込発信データ格納)終了", "成功")
        End Try

        Return 0

    End Function

    ''' <summary>
    ''' 振込発信CSVリエンタの明細情報を設定します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFurikomiData(ByRef Key As FKeyInfo) As Boolean
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing       ' 明細マスタ

        ' 振込日
        Dim FuriDate As Date = CASTCommon.ConvertDate(Key.FURI_DATE)

        ' 振込日の１営業日前，２営業日前
        Dim Zen1Day As String = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
        Dim Zen2Day As String = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

        ' 振込発信CSVリエンタフォーマット
        Dim SoufuriFormat As New CAstFormat.ClsFormatCSVRienta
        Dim SoufuriDataRecordList As New ArrayList

        Try
            Dim EachKEN As Integer = 0
            Dim EachKIN As Double = 0

            OraMeiReader = New CASTCommon.MyOracleReader(MainDB)

            If OraMeiReader.DataReader(GetMeisaiSQL(Key)) = True Then
                While OraMeiReader.EOF = False

                    If OraMeiReader.GetInt("FURIKETU_CODE_K") = 0 Then
                        If OraMeiReader.GetInt64("FURIKIN_K") > 0 Then
                            haskey.RecordNo += 1
                            Key.TotalKen += 1
                            Key.TotalKin += OraMeiReader.GetInt64("FURIKIN_K")
                            '2018/03/15 saitou 広島信金(RSV2標準) ADD スケジュールマスタの手数料更新 -------------------- START
                            Key.TesuuKin += OraMeiReader.GetInt64("TESUU_KIN_K")
                            '2018/03/15 saitou 広島信金(RSV2標準) ADD --------------------------------------------------- END
                            Key.IraiKen += 1
                            Key.IraiKin += OraMeiReader.GetInt64("FURIKIN_K")

                            '--------------------------------------------------
                            '明細部
                            '--------------------------------------------------
                            Select Case PARAMETER.HASSIN_DATE
                                Case Key.FURI_DATE
                                    ' 当日
                                    SoufuriFormat.CSVRNT_DTREC.CR1 = "48-100"
                                    SoufuriFormat.CSVRNT_DTREC.CR4 = "1022"
                                Case Zen1Day
                                    ' １営業日前
                                    SoufuriFormat.CSVRNT_DTREC.CR1 = "48-110"
                                    SoufuriFormat.CSVRNT_DTREC.CR4 = "1122"
                                Case Is <= Zen2Day
                                    ' ２営業日前以前
                                    Select Case Key.SYUBETU
                                        Case "21"
                                            ' 総振
                                            SoufuriFormat.CSVRNT_DTREC.CR1 = "48-110"
                                            SoufuriFormat.CSVRNT_DTREC.CR4 = "1122"
                                        Case "11"
                                            ' 給与
                                            SoufuriFormat.CSVRNT_DTREC.CR1 = "48-120"
                                            SoufuriFormat.CSVRNT_DTREC.CR4 = "1211"
                                        Case "12"
                                            ' 賞与
                                            SoufuriFormat.CSVRNT_DTREC.CR1 = "48-120"
                                            SoufuriFormat.CSVRNT_DTREC.CR4 = "1212"
                                    End Select
                            End Select

                            SoufuriFormat.CSVRNT_DTREC.CR2 = "0"
                            SoufuriFormat.CSVRNT_DTREC.CR3 = Key.FURI_DATE
                            SoufuriFormat.CSVRNT_DTREC.CR5 = "000"
                            SoufuriFormat.CSVRNT_DTREC.CR6 = OraMeiReader.GetString("KIN_KNAME_N")
                            SoufuriFormat.CSVRNT_DTREC.CR7 = OraMeiReader.GetString("SIT_KNAME_N")
                            SoufuriFormat.CSVRNT_DTREC.CR8 = OraMeiReader.GetInt64("FURIKIN_K")
                            SoufuriFormat.CSVRNT_DTREC.CR9 = HONBU_KNAME
                            SoufuriFormat.CSVRNT_DTREC.CR10 = OraMeiReader.GetString("KEIYAKU_KAMOKU_K")
                            SoufuriFormat.CSVRNT_DTREC.CR11 = OraMeiReader.GetString("KEIYAKU_KOUZA_K")
                            SoufuriFormat.CSVRNT_DTREC.CR12 = ""
                            SoufuriFormat.CSVRNT_DTREC.CR13 = "0"
                            SoufuriFormat.CSVRNT_DTREC.CR14 = OraMeiReader.GetString("KEIYAKU_KNAME_K")
                            SoufuriFormat.CSVRNT_DTREC.CR15 = Key.ITAKU_KNAME
                            SoufuriFormat.CSVRNT_DTREC.CR16 = Key.TUKESIT_KNAME & "ｱﾂｶｲ"
                            SoufuriFormat.CSVRNT_DTREC.CR17 = ""
                            SoufuriFormat.CSVRNT_DTREC.CR18 = ""

                            'レコード保存
                            SoufuriDataRecordList.Add(SoufuriFormat.CSVRNT_DTREC.Data)

                            ' 発信マスタへInsert
                            If InsertHassinMast(Key, SoufuriFormat.CSVRNT_DTREC.CR1.Split("-"c)(0), SoufuriFormat.CSVRNT_DTREC.CR1.Split("-"c)(1), SoufuriFormat.CSVRNT_DTREC.Data) = False Then
                                Return False
                            End If

                            EachKEN += 1
                            EachKIN += OraMeiReader.GetInt64("FURIKIN_K")

                            If EachKEN = LimitKen Then

                                Dim NewItem As New ArrayListItem
                                With NewItem
                                    .EachKEN = EachKEN
                                    .EachKIN = EachKIN
                                    .TORIS_CODE = Key.TORIS_CODE
                                    .TORIF_CODE = Key.TORIF_CODE
                                End With
                                RecInfo.Add(NewItem)

                                'ファイルに出力する
                                If Not MakeEachCsvFile(MaxFiles, RecInfo, SoufuriDataRecordList) Then
                                    Return False
                                End If

                                EachKEN = 0
                                EachKIN = 0
                                LimitKen = FD_COUNT_LIMIT

                                SoufuriDataRecordList.Clear()

                                haskey.Kaiji += 1
                            End If
                        Else
                            '振込金額が0のレコードはカウントするが、データは作成しない
                            Key.IraiKen += 1
                            Key.IraiKin += OraMeiReader.GetInt64("FURIKIN_K")

                        End If
                    Else
                        '依頼返却により返却するレコード
                        Key.IraiKen += 1
                        Key.IraiKin += OraMeiReader.GetInt64("FURIKIN_K")

                    End If

                    OraMeiReader.NextRead()
                End While

                'ファイルに出力する
                If EachKEN > 0 Then

                    Dim NewItem As New ArrayListItem
                    With NewItem
                        .EachKEN = EachKEN
                        .EachKIN = EachKIN
                        .TORIS_CODE = Key.TORIS_CODE
                        .TORIF_CODE = Key.TORIF_CODE
                    End With
                    RecInfo.Add(NewItem)

                    If Not MakeEachCsvFile(MaxFiles, RecInfo, SoufuriDataRecordList) Then
                        Return False
                    End If

                    If EachKEN Mod 100 > 0 Then
                        EachKEN = (EachKEN \ 100 + 1) * 100
                    End If

                    If EachKEN < FD_COUNT_LIMIT Then
                        LimitKen = (FD_COUNT_LIMIT - EachKEN)
                    End If
                End If
            End If

            OraMeiReader.Close()

            Return True

        Catch ex As Exception
            MainLOG.Write("明細マスタ取得", "失敗", ex.Message)
            Return False
        Finally
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
                OraMeiReader = Nothing
            End If
        End Try

    End Function

    Private Function LetFDWriteAction(ByVal avMaxFiles As Integer, ByVal avRecInfo As ArrayList) As Boolean
        Dim MSG As String
        Dim FDCnt As Integer = 0
        Dim RecordCounter As Integer = 0
        Try
            'ＦＤ数計算
            For Idx As Integer = 0 To avRecInfo.Count - 1 Step 1

                RecordCounter += avRecInfo(Idx).EachKEN
                Select Case RecordCounter
                    Case Is = FD_COUNT_LIMIT
                        FDCnt += 1
                        RecordCounter = 0
                    Case Is > FD_COUNT_LIMIT
                        FDCnt += 1
                        RecordCounter = avRecInfo(Idx).EachKEN
                End Select
            Next Idx
            If RecordCounter > 0 Then
                FDCnt += 1
            End If

            'ＦＤ複数枚時のメッセージ
            If FDCnt = 1 Then
                MSG = String.Format("空の媒体をドライブ{0}に挿入してください。", ini_info.RIENTA_PATH)
            Else
                MSG = String.Format("空の媒体を{0}枚用意して1枚目の媒体を挿入してください。", FDCnt.ToString)
            End If

            MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Information, _
                            MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
            '指定枚数出力
            RecordCounter = 0

            Dim FDCounter As Integer = 1

            For Index As Integer = 0 To avRecInfo.Count - 1 Step 1

                If RecordCounter = 0 Then

                    If FDCounter > 1 Then
                        MSG = String.Format("{0}枚目の媒体を挿入してください。", FDCounter.ToString)
                        MessageBox.Show(MSG, msgTitle, _
                                        MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                    End If

                    If Not SubLetFDWriterAction(FDCounter, ini_info.RIENTA_PATH) Then
                        Exit Try
                    End If
                End If

                '複写元のファイル
                Dim FileName As String = ""
                Select Case avMaxFiles
                    Case Is = 1
                        FileName = "kawase" & String.Format("{0:00}", Index) & ".csv"
                    Case Else
                        FileName = "kawase" & String.Format("{0:00}", Index + 1) & ".csv"
                End Select
                Dim SourceFileName As String = System.IO.Path.Combine(FDTempDataDirectory, FileName)
                Dim DestFileName As String = System.IO.Path.Combine(ini_info.RIENTA_PATH, FileName)

                RecordCounter += avRecInfo(Index).EachKEN

                Select Case RecordCounter
                    Case Is < FD_COUNT_LIMIT
                        '該当ファイルを複写する

                        System.IO.File.Copy(SourceFileName, DestFileName, True)

                    Case Is = FD_COUNT_LIMIT
                        '該当ファイルを複写する

                        System.IO.File.Copy(SourceFileName, DestFileName, True)

                        RecordCounter = 0
                        FDCounter += 1
                    Case Is > FD_COUNT_LIMIT
                        Index -= 1
                        RecordCounter = 0
                        FDCounter += 1
                End Select
            Next Index

            '2017/11/22 タスク）西野 CHG (標準版修正(№175)) -------------------- START
            MainLOG.Write("タンキング・リエンタＣＳＶ作成", "成功", "")
            'MainLOG.Write("ＦＤタンキング・リエンタＣＳＶ作成", "成功", "")
            '2017/11/22 タスク）西野 CHG (標準版修正(№175)) -------------------- END

            Return True

        Catch ex As Exception
            '2017/11/22 タスク）西野 CHG (標準版修正(№175)) -------------------- START
            MainLOG.Write("タンキング・リエンタＣＳＶ作成", "失敗", ex.Message)
            'MainLOG.Write("ＦＤタンキング・リエンタＣＳＶ作成", "失敗", ex.Message)
            '2017/11/22 タスク）西野 CHG (標準版修正(№175)) -------------------- END

        End Try

        MSG = "作成した媒体は無効となります。再度最初からやり直してください。"
        '2017/11/22 タスク）西野 CHG (標準版修正(№175)) -------------------- START
        MainLOG.Write("タンキング・リエンタＣＳＶ作成", "失敗", MSG)
        'MainLOG.Write("ＦＤタンキング・リエンタＣＳＶ作成", "失敗", MSG)
        '2017/11/22 タスク）西野 CHG (標準版修正(№175)) -------------------- END
        MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, _
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)

        Return False

    End Function

    Private Function SubLetFDWriterAction( _
                                         ByVal avFDCounter As Integer, Optional ByVal avPath As String = "A:\") As Boolean
        Dim MSG As String
        Try
            Do
                Try
                    Dim onFiles() As String = System.IO.Directory.GetFiles(avPath)
                    If onFiles.Length = 0 Then
                        Exit Do
                    End If
                Catch ex As Exception

                End Try

                MSG = "空の媒体を用意してください。"
                If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, _
                                   MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    Return False
                End If
            Loop

            Return True
        Catch ex As Exception
            Return False
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
        SQL.AppendLine(",KAMOKU_T")
        SQL.AppendLine(",KOUZA_T")

        SQL.AppendLine(",KIN_KNAME_N")
        SQL.AppendLine(",SIT_KNAME_N")
        SQL.AppendLine(",KIN_NNAME_N")
        SQL.AppendLine(",SIT_NNAME_N")

        SQL.AppendLine(" FROM S_TORIMAST")
        SQL.AppendLine("     ,S_SCHMAST")
        SQL.AppendLine("     ,TENMAST")

        SQL.AppendLine(" WHERE TORIS_CODE_S = TORIS_CODE_T")
        SQL.AppendLine("   AND TORIF_CODE_S = TORIF_CODE_T")
        SQL.AppendLine("   AND '" & ini_info.JIKINKO_CODE & "' = KIN_NO_N(+)")
        SQL.AppendLine("   AND TORIMATOME_SIT_T = SIT_NO_N(+)")
        ' 給与振込、賞与振込
        SQL.AppendLine("   AND ((SYUBETU_T IN ('11', '12') AND FURI_DATE_S = " & SQ(PARAMETER.KYUFURI_DATE) & ")")
        SQL.AppendLine("    OR  (SYUBETU_T = '21'          AND FURI_DATE_S = " & SQ(PARAMETER.SOUFURI_DATE) & "))")

        SQL.AppendLine("   AND HASSIN_FLG_S = '2'")
        SQL.AppendLine("   AND TYUUDAN_FLG_S = '0'")

        SQL.AppendLine(" ORDER BY FURI_DATE_S, SOUSIN_KBN_S, TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")

        Return SQL
    End Function

    ''' <summary>
    ''' 対象の明細を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetMeisaiSQL(ByVal Key As FKeyInfo) As StringBuilder
        Dim SQL As New StringBuilder
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
        SQL.AppendLine(",FURIKETU_CODE_K")
        'EDI情報用 識別表示
        SQL.AppendLine(",SUBSTRB(FURI_DATA_K, 113, 1) SIKIBETU")

        SQL.AppendLine(",KIN_KNAME_N")
        SQL.AppendLine(",SIT_KNAME_N")
        SQL.AppendLine(",KIN_NNAME_N")
        SQL.AppendLine(",SIT_NNAME_N")

        SQL.AppendLine(" FROM S_MEIMAST")
        SQL.AppendLine("     ,TENMAST")

        SQL.AppendLine(" WHERE TORIS_CODE_K   = " & SQ(Key.TORIS_CODE))
        SQL.AppendLine("   AND TORIF_CODE_K   = " & SQ(Key.TORIF_CODE))
        SQL.AppendLine("   AND FURI_DATE_K    = " & SQ(Key.FURI_DATE))
        SQL.AppendLine("   AND MOTIKOMI_SEQ_K = " & Key.MOTIKOMI_SEQ)
        SQL.AppendLine("   AND DATA_KBN_K   = '2'")
        'SQL.AppendLine("   AND FURIKETU_CODE_K = 0")
        'SQL.AppendLine("   AND FURIKIN_K >= 0")

        SQL.AppendLine("   AND KEIYAKU_KIN_K = KIN_NO_N(+)")
        SQL.AppendLine("   AND KEIYAKU_SIT_K = SIT_NO_N(+)")
        SQL.AppendLine(" ORDER BY RECORD_NO_K")

        Return SQL

    End Function

    ''' <summary>
    ''' 振込発信マスタの登録を行います。
    ''' </summary>
    ''' <param name="Key"></param>
    ''' <param name="KamokuCode"></param>
    ''' <param name="OpeCode"></param>
    ''' <param name="RecordData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertHassinMast(ByVal Key As FKeyInfo, ByVal KamokuCode As String, ByVal OpeCode As String, ByVal RecordData As String) As Boolean
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
            SQL.AppendLine("," & SQ(""))                                        ' リエンタファイル名
            SQL.AppendLine("," & SQ(Key.TORIS_CODE))                            ' 取引先主コード
            SQL.AppendLine("," & SQ(Key.TORIF_CODE))                            ' 取引先副コード
            SQL.AppendLine("," & SQ(Key.FURI_DATE))                             ' 振込日
            SQL.AppendLine("," & SQ(Key.MOTIKOMI_SEQ))                          ' 持込SEQ
            SQL.AppendLine("," & SQ(KamokuCode))                                ' 科目コード
            SQL.AppendLine("," & SQ(OpeCode))                                   ' オペコード
            SQL.AppendLine("," & SQ(RecordData))                                ' 個別データ
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
    ''' 設定ファイルの読み込みを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function IniRead() As Boolean

        ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
            jobMessage = "設定ファイル取得失敗 項目名:自金庫コード 分類:COMMON 項目:KINKOCD"
            Return False
        End If

        ini_info.JIKINKO_NAME = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")
        If ini_info.JIKINKO_NAME = "err" OrElse ini_info.JIKINKO_NAME = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME")
            jobMessage = "設定ファイル取得失敗 項目名:自金庫名 分類:COMMON 項目:KINKONAME"
            Return False
        End If

        ini_info.HONBU_CODE = CASTCommon.GetFSKJIni("COMMON", "HONBUCD")
        If ini_info.HONBU_CODE = "err" OrElse ini_info.HONBU_CODE = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD")
            jobMessage = "設定ファイル取得失敗 項目名:本部コード 分類:COMMON 項目:HONBUCD"
            Return False
        End If

        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR")
            jobMessage = "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR"
            Return False
        End If

        ini_info.CSV_PATH = CASTCommon.GetFSKJIni("COMMON", "CSV")
        If ini_info.CSV_PATH = "err" OrElse ini_info.CSV_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:CSVフォルダ 分類:COMMON 項目:CSV")
            jobMessage = "設定ファイル取得失敗 項目名:CSVフォルダ 分類:COMMON 項目:CSV"
            Return False
        End If

        ini_info.RSV2_S_SYORIKEKKA_CSVRNT = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_SYORIKEKKA_CSVRNT")
        If ini_info.RSV2_S_SYORIKEKKA_CSVRNT = "err" OrElse ini_info.RSV2_S_SYORIKEKKA_CSVRNT = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:処理結果確認表印刷要否 分類:RSV2_V1.0.0 項目:S_SYORIKEKKA_CSVRNT")
            jobMessage = "設定ファイル取得失敗 項目名:処理結果確認表印刷要否 分類:RSV2_V1.0.0 項目:S_SYORIKEKKA_CSVRNT"
            Return False
        End If

        ini_info.RSV2_S_MEISAI_CSVRNT = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_MEISAI_CSVRNT")
        If ini_info.RSV2_S_MEISAI_CSVRNT = "err" OrElse ini_info.RSV2_S_MEISAI_CSVRNT = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:総合振込明細表印刷要否 分類:RSV2_V1.0.0 項目:S_MEISAI_CSVRNT")
            jobMessage = "設定ファイル取得失敗 項目名:総合振込明細表印刷要否 分類:RSV2_V1.0.0 項目:S_MEISAI_CSVRNT"
            Return False
        End If

        ini_info.RSV2_S_FBFURITEN_CSVRNT = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_FBFURITEN_CSVRNT")
        If ini_info.RSV2_S_FBFURITEN_CSVRNT = "err" OrElse ini_info.RSV2_S_FBFURITEN_CSVRNT = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:FBタンキングデータ振込店別集計表印刷要否 分類:RSV2_V1.0.0 項目:S_FBFURITEN_CSVRNT")
            jobMessage = "設定ファイル取得失敗 項目名:FBタンキングデータ振込店別集計表印刷要否 分類:RSV2_V1.0.0 項目:S_FBFURITEN_CSVRNT"
            Return False
        End If

        ini_info.RSV2_HDR_GRP = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HDR_GRP")
        If ini_info.RSV2_HDR_GRP = "err" OrElse ini_info.RSV2_HDR_GRP = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:CSVリエンタヘッダグループ 分類:RSV2_V1.0.0 項目:HDR_GRP")
            jobMessage = "設定ファイル取得失敗 項目名:CSVリエンタヘッダグループ 分類:RSV2_V1.0.0 項目:HDR_GRP"
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' ファイル数の取得を行います。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFileCount() As Integer
        Dim OraReader1 As CASTCommon.MyOracleReader = Nothing
        Dim OraReader2 As CASTCommon.MyOracleReader = Nothing
        Dim Key As FKeyInfo = Nothing

        Try
            Dim Ret As Integer = 0

            OraReader1 = New CASTCommon.MyOracleReader(MainDB)
            If OraReader1.DataReader(GetScheduleSQL) = True Then
                OraReader2 = New CASTCommon.MyOracleReader(MainDB)
                While OraReader1.EOF = False

                    Dim Counter As Integer = 0
                    Key.Init()
                    Call Key.SetOraDataKessai(OraReader1)

                    If OraReader2.DataReader(GetMeisaiSQL(Key)) = True Then
                        While OraReader2.EOF = False
                            Counter += 1
                            OraReader2.NextRead()
                        End While
                    End If

                    Ret += (Counter \ FD_COUNT_LIMIT)
                    If Counter Mod FD_COUNT_LIMIT > 0 Then
                        Ret += 1
                    End If

                    OraReader2.Close()

                    OraReader1.NextRead()
                End While
            End If

            Return Ret

        Catch ex As Exception
            MainLOG.Write("明細数取得", "失敗", ex.Message)
            Return 0

        Finally
            If Not OraReader1 Is Nothing Then
                OraReader1.Close()
                OraReader1 = Nothing
            End If

            If Not OraReader2 Is Nothing Then
                OraReader2.Close()
                OraReader2 = Nothing
            End If
        End Try
    End Function

    Private Function MakeEachCsvFile(ByVal avMaxFiles As Integer, ByVal avRecInfo As ArrayList, _
                                     ByVal avDataRecInfo As ArrayList) As Boolean
        Dim FS As System.IO.FileStream = Nothing
        Dim FW As System.IO.StreamWriter = Nothing
        Dim MaxIndex As Integer = (avRecInfo.Count - 1)
        Try
            'ファイル名
            Dim FileName As String = System.IO.Path.Combine(FDTempDataDirectory, "kawase")
            Select Case avMaxFiles
                Case Is = 1
                    FileName &= "00.csv"
                Case Else
                    FileName &= String.Format("{0:00}", avRecInfo.Count) & ".csv"
            End Select

            FS = New System.IO.FileStream(FileName, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
            FW = New System.IO.StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            '振込発信CSVリエンタフォーマット
            Dim SoufuriFormat As New CAstFormat.ClsFormatCSVRienta

            '--------------------------------------------------
            'ファイルヘッダ部１行目
            '--------------------------------------------------
            SoufuriFormat.CSVRNT_HDREC1.CR1 = "TANCSVDAT"
            SoufuriFormat.CSVRNT_HDREC1.CR2 = String.Format("{0:MMdd}", Date.Now) & _
                String.Format("{0:0000}", haskey.Kaiji)
            FW.WriteLine(SoufuriFormat.CSVRNT_HDREC1.Data)

            '--------------------------------------------------
            'ファイルヘッダ部２行目
            '--------------------------------------------------
            SoufuriFormat.CSVRNT_HDREC2.CR1 = ini_info.RSV2_HDR_GRP & _
                StrConv(String.Format("{0:MMdd}", Date.Now), VbStrConv.Wide) & _
                StrConv(String.Format("{0:0000}", haskey.Kaiji), VbStrConv.Wide)
            SoufuriFormat.CSVRNT_HDREC2.CR2 = "0"
            SoufuriFormat.CSVRNT_HDREC2.CR3 = avRecInfo.Item(MaxIndex).EachKEN.ToString
            SoufuriFormat.CSVRNT_HDREC2.CR4 = GetWAREKI(PARAMETER.HASSIN_DATE, "yyMMdd")
            SoufuriFormat.CSVRNT_HDREC2.CR5 = "0"
            SoufuriFormat.CSVRNT_HDREC2.CR6 = "0"
            FW.WriteLine(SoufuriFormat.CSVRNT_HDREC2.Data)

            '--------------------------------------------------
            '明細部
            '--------------------------------------------------
            For Each RecordData As String In avDataRecInfo
                FW.WriteLine(RecordData)
            Next

            Return True
        Catch ex As Exception
            MainLOG.Write("FDタンキング・リエンタCSV作成","失敗",ex.Message)
            Return False
        Finally
            If Not FW Is Nothing Then
                FW.Close()
                FW = Nothing
            End If
            If Not FS Is Nothing Then
                FS.Close()
                FS = Nothing
            End If
        End Try
    End Function

    ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- START
    '''' <summary>
    '''' 和暦変換
    '''' </summary>
    '''' <param name="strDate">変換する日付</param>
    '''' <param name="strFormat">変換するフォーマット</param>
    '''' <returns>変換後の日付</returns>
    '''' <remarks></remarks>
    'Private Function GetWAREKI(ByVal strDate As String, ByVal strFormat As String) As String
    '    Dim culture As CultureInfo = New CultureInfo("ja-JP", True)
    '    culture.DateTimeFormat.Calendar = New JapaneseCalendar
    '    Dim target As DateTime = New DateTime(strDate.Substring(0, 4), strDate.Substring(4, 2), strDate.Substring(6, 2))
    '    Return target.ToString(strFormat, culture)
    'End Function
    ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- END

    ''' <summary>
    ''' 処理結果確認表の印刷処理を行います。
    ''' </summary>
    ''' <param name="ExeRepo"></param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function PrintSyoriKekkaList(ByVal ExeRepo As CAstReports.ClsExecute) As Boolean
        Dim iRet As Integer
        Dim bRet As Boolean = True

        Dim List As New ClsPrnSyoriKekkaList
        List.CreateCsvFile()

        ' 振込日
        Dim FuriDate As Date

        ' 振込日の１営業日前，２営業日前
        Dim Zen1Day As String
        Dim Zen2Day As String

        Try
            For i As Integer = 0 To Me.alPrintList.Count - 1
                Dim Key As FKeyInfo = DirectCast(Me.alPrintList.Item(i), FKeyInfo)

                'CSV作成
                With List.CSVObject
                    .Output(strDate, True)
                    .Output(strTime, True)
                    .Output(Key.TORIS_CODE, True)
                    .Output(Key.TORIF_CODE, True)
                    .Output(Key.FURI_DATE, True)
                    .Output(Key.MOTIKOMI_SEQ.ToString)
                    .Output(Key.ITAKU_NNAME, True)
                    .Output(Key.TUKESIT_NNAME, True)

                    '種別の設定
                    FuriDate = CASTCommon.ConvertDate(Key.FURI_DATE)
                    Zen1Day = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
                    Zen2Day = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

                    Select Case PARAMETER.HASSIN_DATE
                        Case Key.FURI_DATE
                            ' 当日
                            .Output("ﾌﾘｺﾐ", True)
                        Case Zen1Day
                            ' １営業日前
                            .Output("ｻｷﾌﾘ", True)
                        Case Is <= Zen2Day
                            ' ２営業日前以前
                            Select Case Key.SYUBETU
                                Case "21"
                                    ' 総振
                                    .Output("ｻｷﾌﾘ", True)
                                Case "11"
                                    ' 給与
                                    .Output("ｷﾕｳﾖ", True)
                                Case "12"
                                    ' 賞与
                                    .Output("ｼﾖｳﾖ", True)
                            End Select
                    End Select

                    Dim SyubetuText As CAstFormat.ClsText = New CAstFormat.ClsText("KFSMAST010_種別.TXT")
                    .Output(SyubetuText.GetBaitaiCode(Key.SYUBETU), True)
                    .Output(Key.IraiKen.ToString)
                    .Output(Key.IraiKin.ToString)
                    .Output(Key.TotalKen.ToString)
                    .Output(Key.TotalKin.ToString, False, True)
                End With
            Next

        Catch ex As Exception
            bRet = False
            MainLOG.Write("処理結果確認表出力", "失敗", ex.Message)

        End Try

        If Not List Is Nothing AndAlso bRet = True Then
            List.CloseCsv()

            '印刷バッチ呼出し
            If ini_info.RSV2_S_SYORIKEKKA_CSVRNT = "0" Then
                MainLOG.Write("処理結果確認表印刷", "成功", "印刷不要")
                Return bRet
            End If

            Dim param As String = MainLOG.UserID & "," & List.FileName

            iRet = ExeRepo.ExecReport("KFSP032.EXE", param)

            If iRet <> 0 Then
                '印刷失敗
                If iRet = -1 Then
                    jobMessage = "処理結果確認表 印刷対象0件"
                Else
                    jobMessage = "処理結果確認表 印刷失敗 エラーコード：" & iRet.ToString
                End If

                MainLOG.Write("処理結果確認表印刷", "失敗", jobMessage)
                bRet = False
            End If
        End If

        Return bRet

    End Function

    ''' <summary>
    ''' 総合振込明細表の印刷処理を行います。
    ''' </summary>
    ''' <param name="ExeRepo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function PrintSoufuriMeisai(ByVal ExeRepo As CAstReports.ClsExecute) As Boolean
        Dim iRet As Integer
        Dim bRet As Boolean = True

        Dim List As New ClsPrnSoufuriMeisai
        List.CreateCsvFile()

        Dim dbReader As CASTCommon.MyOracleReader = Nothing

        ' 振込日
        Dim FuriDate As Date

        ' 振込日の１営業日前，２営業日前
        Dim Zen1Day As String
        Dim Zen2Day As String

        Try
            dbReader = New CASTCommon.MyOracleReader(MainDB)

            Dim KamokuText As New CAstFormat.ClsText("Common_総振_科目.TXT")

            For i As Integer = 0 To Me.alPrintList.Count - 1
                Dim Key As FKeyInfo = DirectCast(Me.alPrintList.Item(i), FKeyInfo)

                '種別の設定
                Dim Syubetu As String = String.Empty
                FuriDate = CASTCommon.ConvertDate(Key.FURI_DATE)
                Zen1Day = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
                Zen2Day = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

                Select Case PARAMETER.HASSIN_DATE
                    Case Key.FURI_DATE
                        ' 当日
                        Syubetu = "ﾌﾘｺﾐ"
                    Case Zen1Day
                        ' １営業日前
                        Syubetu = "ｻｷﾌﾘ"
                    Case Is <= Zen2Day
                        ' ２営業日前以前
                        Select Case Key.SYUBETU
                            Case "21"
                                ' 総振
                                Syubetu = "ｻｷﾌﾘ"
                            Case "11"
                                ' 給与
                                Syubetu = "ｷﾕｳﾖ"
                            Case "12"
                                ' 賞与
                                Syubetu = "ｼﾖｳﾖ"
                        End Select
                End Select

                If dbReader.DataReader(GetMeisaiSQL(Key)) = True Then
                    While dbReader.EOF = False
                        If dbReader.GetInt("FURIKETU_CODE_K") = 0 Then
                            With List.CSVObject
                                'ヘッダ部
                                .Output(strDate, True)
                                .Output(strTime, True)
                                .Output(Key.TORIS_CODE, True)
                                .Output(Key.TORIF_CODE, True)
                                .Output(Key.ITAKU_NNAME, True)
                                .Output(Key.FURI_DATE, True)
                                .Output(Key.MOTIKOMI_SEQ.ToString)

                                .Output(Key.TORIMATOME_SIT, True)
                                .Output(Key.TUKESIT_NNAME, True)
                                .Output(KamokuText.GetBaitaiCode(Key.KAMOKU), True)
                                .Output(Key.KOUZA, True)

                                .Output(ini_info.JIKINKO_CODE)

                                '明細部
                                .Output(dbReader.GetString("KEIYAKU_KNAME_K"), True)
                                .Output(dbReader.GetString("KEIYAKU_KIN_K"), True)
                                .Output(dbReader.GetString("KEIYAKU_SIT_K"), True)
                                .Output(dbReader.GetString("KIN_NNAME_N"), True)
                                .Output(dbReader.GetString("SIT_NNAME_N"), True)
                                Select Case dbReader.GetString("KEIYAKU_KAMOKU_K")
                                    Case "1" : .Output("ﾌ", True)
                                    Case "2" : .Output("ﾄ", True)
                                    Case "9" : .Output("ｿ", True)
                                    Case Else : .Output("ｿ", True)
                                End Select
                                .Output(dbReader.GetString("KEIYAKU_KOUZA_K"), True)
                                .Output(dbReader.GetString("JYUYOUKA_NO_K"), True)
                                .Output(dbReader.GetInt64("FURIKIN_K").ToString)
                                .Output(Syubetu, True, True)
                            End With

                        End If

                        dbReader.NextRead()
                    End While
                End If

                dbReader.Close()

            Next


        Catch ex As Exception
            bRet = False
            MainLOG.Write("総合振込明細表出力", "失敗", ex.Message)

        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If
        End Try

        If Not List Is Nothing AndAlso bRet = True Then
            List.CloseCsv()

            '印刷バッチ呼出し
            If ini_info.RSV2_S_MEISAI_CSVRNT = "0" Then
                MainLOG.Write("総合振込明細表印刷", "成功", "印刷不要")
                Return bRet
            End If

            Dim param As String = MainLOG.UserID & "," & List.FileName

            iRet = ExeRepo.ExecReport("KFSP033.EXE", param)

            If iRet <> 0 Then
                '印刷失敗
                If iRet = -1 Then
                    jobMessage = "総合振込明細表 印刷対象0件"
                Else
                    jobMessage = "総合振込明細表 印刷失敗 エラーコード：" & iRet.ToString
                End If

                MainLOG.Write("総合振込明細表印刷", "失敗", jobMessage)
                bRet = False
            End If
        End If

        Return bRet

    End Function

    ''' <summary>
    ''' FBタンキングデータ振込店別集計表の印刷処理を行います。
    ''' </summary>
    ''' <param name="ExeRepo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function PrintFBTenbetuList(ByVal ExeRepo As CAstReports.ClsExecute) As Boolean
        Dim iRet As Integer
        Dim bRet As Boolean = True

        Dim List As New ClsPrnFBTenbetuList
        List.CreateCsvFile()

        Dim dbReader As CASTCommon.MyOracleReader = Nothing

        ' 振込日
        Dim FuriDate As Date

        ' 振込日の１営業日前，２営業日前
        Dim Zen1Day As String
        Dim Zen2Day As String

        Try
            Dim SQL As New StringBuilder
            With SQL
                .Append("SELECT ")
                .Append(" TORIS_CODE_S")
                .Append(",TORIF_CODE_S")
                .Append(",FURI_DATE_S")
                .Append(",MOTIKOMI_SEQ_S")
                .Append(",FURI_KEN_S")
                .Append(",FURI_KIN_S")
                .Append(",ITAKU_NNAME_T")
                .Append(",SYUBETU_T")
                .Append(",TKIN_NO_T")
                .Append(",TSIT_NO_T")
                .Append(",KIN_NNAME_N")
                .Append(",SIT_NNAME_N")

                .Append(" FROM S_TORIMAST")
                .Append(" INNER JOIN S_SCHMAST")
                .Append(" ON TORIS_CODE_T = TORIS_CODE_S")
                .Append(" AND TORIF_CODE_T = TORIF_CODE_S")
                .Append(" LEFT OUTER JOIN TENMAST")
                .Append(" ON TKIN_NO_T = KIN_NO_N")
                .Append(" AND TSIT_NO_T = SIT_NO_N")

                .Append(" WHERE HASSIN_FLG_S = '1'")
                .Append(" AND HASSIN_DATE_S = '" & Me.strDate & "'")
                .Append(" AND HASSIN_TIME_STAMP_S = '" & String.Concat(Me.strDate, Me.strTime) & "'")

                .Append(" ORDER BY ")
                .Append(" TKIN_NO_T")
                .Append(",TSIT_NO_T")
                .Append(",TORIS_CODE_S")
                .Append(",TORIF_CODE_S")
                .Append(",FURI_DATE_S")
                .Append(",MOTIKOMI_SEQ_S")
            End With

            dbReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SyubetuText As New CAstFormat.ClsText("KFSMAST010_種別.TXT")

            If dbReader.DataReader(SQL) = True Then
                While dbReader.EOF = False
                    With List.CSVObject
                        .Output(Me.strDate, True)
                        .Output(Me.strTime, True)
                        .Output(dbReader.GetString("TKIN_NO_T"), True)
                        .Output(dbReader.GetString("TSIT_NO_T"), True)
                        .Output(dbReader.GetString("KIN_NNAME_N"), True)
                        .Output(dbReader.GetString("SIT_NNAME_N"), True)
                        .Output(dbReader.GetString("TORIS_CODE_S"), True)
                        .Output(dbReader.GetString("TORIF_CODE_S"), True)
                        .Output(dbReader.GetString("ITAKU_NNAME_T"), True)
                        .Output(dbReader.GetString("FURI_DATE_S"), True)
                        .Output(dbReader.GetInt("MOTIKOMI_SEQ_S").ToString)
                        .Output(SyubetuText.GetBaitaiCode(dbReader.GetString("SYUBETU_T")), True)

                        '種別の設定
                        Dim Syubetu As String = String.Empty
                        FuriDate = CASTCommon.ConvertDate(dbReader.GetString("FURI_DATE_S"))
                        Zen1Day = CASTCommon.GetEigyobi(FuriDate, -1, FmtComm.HolidayList).ToString("yyyyMMdd")
                        Zen2Day = CASTCommon.GetEigyobi(FuriDate, -2, FmtComm.HolidayList).ToString("yyyyMMdd")

                        Select Case PARAMETER.HASSIN_DATE
                            Case dbReader.GetString("FURI_DATE_S")
                                ' 当日
                                Syubetu = "ﾌﾘｺﾐ"
                            Case Zen1Day
                                ' １営業日前
                                Syubetu = "ｻｷﾌﾘ"
                            Case Is <= Zen2Day
                                ' ２営業日前以前
                                Select Case dbReader.GetString("SYUBETU_T")
                                    Case "21"
                                        ' 総振
                                        Syubetu = "ｻｷﾌﾘ"
                                    Case "11"
                                        ' 給与
                                        Syubetu = "ｷﾕｳﾖ"
                                    Case "12"
                                        ' 賞与
                                        Syubetu = "ｼﾖｳﾖ"
                                End Select
                        End Select

                        .Output(Syubetu, True)
                        .Output(dbReader.GetInt64("FURI_KEN_S").ToString)
                        .Output(dbReader.GetInt64("FURI_KIN_S").ToString, False, True)

                    End With

                    dbReader.NextRead()
                End While

            End If

        Catch ex As Exception
            bRet = False
            MainLOG.Write("FBタンキングデータ振込店別集計表出力", "失敗", ex.Message)

        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If
        End Try

        If Not List Is Nothing AndAlso bRet = True Then
            List.CloseCsv()

            '印刷バッチ呼出し
            If ini_info.RSV2_S_FBFURITEN_CSVRNT = "0" Then
                MainLOG.Write("FBタンキングデータ振込店別集計表印刷", "成功", "印刷不要")
                Return bRet
            End If

            Dim param As String = MainLOG.UserID & "," & List.FileName

            iRet = ExeRepo.ExecReport("KFSP034.EXE", param)

            If iRet <> 0 Then
                '印刷失敗
                If iRet = -1 Then
                    jobMessage = "FBタンキングデータ振込店別集計表 印刷対象0件"
                Else
                    jobMessage = "FBタンキングデータ振込店別集計表 印刷失敗 エラーコード：" & iRet.ToString
                End If

                MainLOG.Write("FBタンキングデータ振込店別集計表印刷", "失敗", jobMessage)
                bRet = False
            End If
        End If

        Return bRet

    End Function

#End Region

End Class
