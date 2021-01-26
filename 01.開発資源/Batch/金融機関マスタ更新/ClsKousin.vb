Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CASTCommon
Imports CASTCommon.ModPublic

' 金融機関マスタ更新処理
Public Class ClsKousin

    ' ログ処理クラス
    Private MainLOG As New CASTCommon.BatchLOG("KFU010", "金融機関マスタ更新")

    ' 金融機関マスタ更新 構造体
    Structure KOBETUPARAM
        Dim SyoriDate As String
        Dim SyoriKbn As String
        Dim tuuban As Integer

        '固定長データ処理用プロパティ
        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)

                If para.Length = 3 Then
                    SyoriDate = para(0)                  ' 処理日付
                    SyoriKbn = para(1)              '処理区分
                    tuuban = Integer.Parse(para(2))   ' ジョブ通番
                End If
            End Set
        End Property
    End Structure
    Private mParam As KOBETUPARAM

    ' 金融機関ワークマスタ構造体
    Public Structure strTENVIEW
        Dim KIN_NO As String        '金融機関コード
        Dim KIN_FUKA As String      '金融機関付加コード
        Dim KIN_KNAME As String     '金融機関カナ名
        Dim KIN_NNAME As String     '金融機関漢字名
        Dim SIT_NO As String        '店舗コード
        Dim SIT_FUKA As String      '店舗付加コード
        Dim SIT_KNAME As String     '店舗カナ名
        Dim SIT_NNAME As String     '店舗漢字名
        Dim DENWA As String         '電話番号
        Dim YUUBIN As String        '郵便番号
        Dim KJYUSYO As String       '住所カナ
        Dim NJYUSYO As String       '住所漢字
        Dim KIN_IDO_CODE As String  '金融機関異動事由コード 
        Dim KIN_IDO_DATE As String  '金融機関異動日
        Dim SIT_IDO_CODE As String  '店舗異動事由コード
        Dim SIT_IDO_DATE As String  '店舗異動日
        Dim DEL_DATE As String      '店舗削除日
        Dim DEL_KIN_DATE As String  '金融機関削除日
        Dim TEN_ZOKUSEI As String   '店舗属性表示
        Dim SEIDOKU As String       '正読表示

        Public WriteOnly Property SetData() As MyOracleReader
            Set(ByVal OraReader As MyOracleReader)
                With OraReader
                    KIN_NO = .GetString("KIN_NO")
                    KIN_FUKA = .GetString("KIN_FUKA")
                    KIN_KNAME = .GetString("KIN_KNAME")
                    KIN_NNAME = .GetString("KIN_NNAME")
                    SIT_NO = .GetString("SIT_NO")
                    SIT_FUKA = .GetString("SIT_FUKA")
                    SIT_KNAME = .GetString("SIT_KNAME")
                    SIT_NNAME = .GetString("SIT_NNAME")
                    DENWA = .GetString("DENWA")
                    YUUBIN = .GetString("YUUBIN")
                    KJYUSYO = .GetString("KJYUSYO")
                    NJYUSYO = .GetString("NJYUSYO")
                    KIN_IDO_CODE = .GetString("KIN_IDO_CODE")
                    KIN_IDO_DATE = .GetString("KIN_IDO_DATE")
                    SIT_IDO_CODE = .GetString("SIT_IDO_CODE")
                    SIT_IDO_DATE = .GetString("SIT_IDO_DATE")
                    DEL_DATE = .GetString("DEL_DATE")
                    DEL_KIN_DATE = .GetString("DEL_KIN_DATE")
                    TEN_ZOKUSEI = .GetString("TEN_ZOKUSEI")
                    SEIDOKU = .GetString("SEIDOKU")
                End With
            End Set
        End Property
    End Structure
    Private TENVIEW As strTENVIEW

    Structure strcIni

        Dim DAT As String           'DATのパス
        Dim DEN As String           'DENのパス
        Dim KIN_KOUSIN As String    '更新区分 0:一括 1:差分
        Dim KIN_FILENAME As String  'リエンタファイル名
        Dim FTRANP As String        'ＦＴＲＡＮＰフォルダ
        Dim FTR As String           'ＦＴＲＡＮフォルダ
        Dim DATBK As String         'DATBKのパス

    End Structure
    Private ini_info As strcIni

    ' ジョブメッセージ
    Private JobMessage As String = ""

    ' 依頼データファイル名
    Private mDataFileName As String

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    ' New
    Public Sub New()
    End Sub

    ' 機能　 ： 金融機関マスタ更新処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Function Main(ByVal command As String) As Integer
        ' パラメータチェック

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            ' メイン引数設定
            mParam.Data = command

            ' ジョブ通番設定
            MainLOG.JobTuuban = mParam.tuuban

            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = MainLOG.Write_Enter1("", "0000000000-00", "00000000", "(メイン処理)開始", "成功")
            'MainLOG.Write("処理開始", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***


            ' オラクル
            MainDB = New MyOracle

            ' 金融機関マスタ更新処理
            Dim bRet As Boolean = KousinMain()

            If bRet = False Then
                MainDB.Rollback()
                MainDB.Close()

                MainLOG.UpdateJOBMASTbyErr(JobMessage)

                Return 2
            Else
                MainDB.Commit()
                MainDB.Close()

                MainLOG.UpdateJOBMASTbyOK(JobMessage)

                Return 0
            End If
        Catch ex As Exception
            MainLOG.Write("(メイン処理)", "失敗", ex.Message)

            Return -1
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            MainLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "(メイン処理)終了", "成功")
            'MainLOG.Write("処理終了", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        End Try

    End Function

    ' 金融機関マスタ更新
    Private Function KousinMain() As Boolean
        ' 入力ファイル名
        Dim InFileName As String
        ' 作業ファイル
        Dim WorkFileName As String

        Dim oraKinReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraSitReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            'iniファイル取得
            If Read_ini() = False Then
                Return False
            End If

            '念のためiniファイルの処理区分が等しいかチェック
            If mParam.SyoriKbn.Equals(ini_info.KIN_KOUSIN) = False Then
                JobMessage = "iniファイル　処理区分不一致"
                MainLOG.Write("金融機関マスタ更新", "失敗", JobMessage)
                Return False
            End If

            InFileName = Path.Combine(ini_info.DAT, ini_info.KIN_FILENAME)
            WorkFileName = Path.Combine(ini_info.DATBK, Path.GetFileNameWithoutExtension(ini_info.KIN_FILENAME) & "_work")

            '入力ファイルチェック
            If File.Exists(InFileName) = False Then
                JobMessage = "ファイルなし：" & InFileName
                MainLOG.Write("金融機関マスタ更新", "失敗", JobMessage)
                Return False
            End If

            'EBCDIC→JISコード変換し、ワークフォルダへコピー
            If ConvertData(InFileName, WorkFileName) = False Then
                MainLOG.Write("コード変換", "失敗", JobMessage)

                Return False
            End If

            '一括の場合は？履歴テーブル削除
            'If mParam.SyoriKbn = "0" Then
            If MainDB.ExecuteNonQuery("DELETE FROM WRK_KININFO") < 0 Then Return False
            If MainDB.ExecuteNonQuery("DELETE FROM WRK_SITINFO") < 0 Then Return False
            'End If

            'ワークテーブルにインサート
            If CreateWork(WorkFileName) = False Then
                Return False
            End If

            '一括と差分で処理分岐
            Select Case mParam.SyoriKbn

                Case "0" '一括

                    '金融機関マスタクリア
                    'If MainDB.ExecuteNonQuery("DELETE FROM TENMAST") < 0 Then Return False
                    If MainDB.ExecuteNonQuery("DELETE FROM KIN_INFOMAST") < 0 Then Return False
                    If MainDB.ExecuteNonQuery("DELETE FROM SITEN_INFOMAST") < 0 Then Return False

                    If GetKinReader(oraKinReader) = False Then
                        Return False
                    End If

                    '金融機関情報の更新
                    If RenewKinInfo(oraKinReader) = False Then
                        Return False
                    End If

                    oraKinReader.Close()

                    'ワーク支店情報の取得
                    If GetSItReader(oraSitReader) = False Then
                        Return False
                    End If

                    '支店情報の更新
                    If RenewSitInfo(oraSitReader) = False Then
                        Return False
                    End If

                    oraSitReader.Close()

                Case "1" '差分

                    '削除日到来レコード削除
                    If DeleteMast() = False Then
                        Return False
                    End If

                    'ワーク金融機関情報の取得
                    If GetKinReader(oraKinReader) = False Then
                        Return False
                    End If

                    '金融機関情報の更新
                    If RenewKinInfo(oraKinReader) = False Then
                        Return False
                    End If

                    oraKinReader.Close()

                    'ワーク支店情報の取得
                    If GetSitReader(oraSitReader) = False Then
                        Return False
                    End If

                    '支店情報の更新
                    If RenewSitInfo(oraSitReader) = False Then
                        Return False
                    End If

                    oraSitReader.Close()

            End Select

            ''支店ワークマスタをベースに金融機関ワークマスタの金融機関情報を結合する()
            ''金融機関ワークマスタに存在しなければ金融機関マスタと結合する()
            'sql.Length = 0
            'sql.Append("SELECT * FROM WRK_TENMAST_VIEW")
            ''削除日が処理日以降のもの()
            'sql.Append(" WHERE (DEL_DATE IS NULL")
            'sql.Append("     OR DEL_DATE = ' '")
            'sql.Append("     OR DEL_DATE >= '" & SYORIDATE & "')")
            ''姫路信金用 ゆうちょ銀行は店舗属性 = 2(出張所)のみ対象
            'sql.Append("   AND (KIN_NO   <> '9900' OR TEN_ZOKUSEI <> '2')")

            'ファイル移動
            Dim bkFileName As String
            bkFileName = Path.Combine(ini_info.DATBK, Path.GetFileNameWithoutExtension(InFileName) & "_" & _
                                      mParam.SyoriDate & Path.GetExtension(InFileName))
            File.Copy(InFileName, bkFileName, True)                 'ファイル上書きコピー
            If File.Exists(InFileName) Then File.Delete(InFileName) '元ファイル削除

            Return True

        Catch ex As Exception
            JobMessage = "ログ参照"
            MainLOG.Write("金融機関マスタ更新", "失敗", ex.Message)

            Return False
            If Not oraKinReader Is Nothing Then oraKinReader.Close()
            If Not oraSitReader Is Nothing Then oraSitReader.Close()
        End Try

        Return True
    End Function

    Private Function Read_ini() As Boolean

        ini_info.DAT = CASTCommon.GetFSKJIni("COMMON", "DAT")
        If ini_info.DAT = "err" OrElse ini_info.DAT = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            JobMessage = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
            Return False
        End If

        ini_info.DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
        If ini_info.DEN = "err" OrElse ini_info.DEN = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN")
            JobMessage = "設定ファイル取得失敗 項目名:DENフォルダ 分類:COMMON 項目:DEN"
            Return False
        End If

        ini_info.KIN_KOUSIN = CASTCommon.GetFSKJIni("COMMON", "KIN_KOUSIN")
        If ini_info.KIN_KOUSIN = "err" OrElse ini_info.KIN_KOUSIN = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:金融機関更新区分 分類:COMMON 項目:KIN_KOUSIN")
            JobMessage = "設定ファイル取得失敗 項目名:金融機関更新区分 分類:COMMON 項目:KIN_KOUSIN"
            Return False
        End If

        ini_info.KIN_FILENAME = CASTCommon.GetFSKJIni("COMMON", "KIN_FILENAME")
        If ini_info.KIN_FILENAME = "err" OrElse ini_info.KIN_FILENAME = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:金融機関ファイル名 分類:COMMON 項目:KIN_FILENAME")
            JobMessage = "設定ファイル取得失敗 項目名:金融機関ファイル名 分類:COMMON 項目:KIN_FILENAME"
            Return False
        End If

        ini_info.FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        If ini_info.FTRANP = "err" OrElse ini_info.FTRANP = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:FTRANPフォルダ 分類:COMMON 項目:FTRANP")
            JobMessage = "設定ファイル取得失敗 項目名:FTRANPフォルダ 分類:COMMON 項目:FTRANP"
            Return False
        End If

        ini_info.FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If ini_info.FTR = "err" OrElse ini_info.FTR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:FTRフォルダ 分類:COMMON 項目:FTR")
            JobMessage = "設定ファイル取得失敗 項目名:FTRフォルダ 分類:COMMON 項目:FTR"
            Return False
        End If

        ini_info.DATBK = CASTCommon.GetFSKJIni("COMMON", "DATBK")
        If ini_info.DATBK = "err" OrElse ini_info.DATBK = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK")
            JobMessage = "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK"
            Return False
        End If

        Return True
    End Function

    Private Function CreateWork(ByVal SyoriFile As String) As Boolean

        Dim StrmReader As FileStream = Nothing
        Dim OraReader As New MyOracleReader
        Dim SQL As StringBuilder = New StringBuilder(1024)

        '異動通知データをワークマスタに書き込む
        Dim RecCnt As Integer = 0       'レコード件数
        Dim KinCnt As Integer = 0       '金融機関レコード件数
        Dim SitCnt As Integer = 0       '店舗レコード件数

        Try
            'ワークファイルを使用して処理する
            StrmReader = New FileStream(SyoriFile, FileMode.Open, FileAccess.Read)

            Dim LineData(380 - 1) As Byte
            Dim Ch380(380 - 1) As Char
            Dim ReadLen As Integer
            Dim Encd As Encoding = Encoding.GetEncoding("SHIFT-JIS")

            ReadLen = StrmReader.Read(LineData, 0, LineData.Length)

            If ReadLen <> LineData.Length Then
                JobMessage = "レコード長不一致"
                MainLOG.Write("金融機関マスタ更新", "失敗", "レコード長不一致")

                Return False
            End If

            Console.WriteLine("履歴テーブル作成中")

            Do While ReadLen = LineData.Length
                RecCnt += 1

                Select Case Encd.GetString(LineData, 0, 2)
                    Case "21"
                        ' 金融機関データフォーマット
                        Dim KinkoData As New ClsFormatIdo.KinkoFormat
                        ' 金融機関データ
                        Call KinkoData.Init()
                        KinkoData.Data = LineData

                        'If KinkoData.KinCode = KinkoData.DaiKinCode Then
                        SQL.Length = 0
                        SQL.Append("INSERT INTO WRK_KININFO(")
                        SQL.Append(" DATA_KBN_N")
                        SQL.Append(",DATA_SYUBETU_N")
                        SQL.Append(",KIN_NO_N")
                        SQL.Append(",KIN_FUKA_N")
                        SQL.Append(",DAIHYO_NO_N")
                        SQL.Append(",KIN_KNAME_N")
                        SQL.Append(",KIN_NNAME_N")
                        SQL.Append(",RYAKU_KIN_KNAME_N")
                        SQL.Append(",RYAKU_KIN_NNAME_N")
                        SQL.Append(",JC1_N")
                        SQL.Append(",JC2_N")
                        SQL.Append(",JC3_N")
                        SQL.Append(",JC4_N")
                        SQL.Append(",JC5_N")
                        SQL.Append(",JC6_N")
                        SQL.Append(",JC7_N")
                        SQL.Append(",JC8_N")
                        SQL.Append(",JC9_N")
                        SQL.Append(",KIN_IDO_DATE_N")
                        SQL.Append(",KIN_IDO_CODE_N")
                        SQL.Append(",NEW_KIN_NO_N")
                        SQL.Append(",NEW_KIN_FUKA_N")
                        SQL.Append(",KIN_DEL_DATE_N")
                        SQL.Append(",SYORI_DATE_N")
                        SQL.Append(") VALUES (")
                        SQL.Append(" " & SQ(KinkoData.DataKubun))
                        SQL.Append("," & SQ(KinkoData.DataSyubetu))
                        SQL.Append("," & SQ(KinkoData.KinCode))
                        SQL.Append("," & SQ(KinkoData.KinFukaCode))
                        SQL.Append("," & SQ(KinkoData.DaiKinCode))
                        SQL.Append("," & SQ(KinkoData.SeiKinKana))
                        SQL.Append("," & SQ(KinkoData.SeiKinKanji))
                        SQL.Append("," & SQ(KinkoData.RyakuKinKana))
                        SQL.Append("," & SQ(KinkoData.RyakuKinKanji))
                        SQL.Append("," & SQ(KinkoData.JISIN(0)))
                        SQL.Append("," & SQ(KinkoData.JISIN(1)))
                        SQL.Append("," & SQ(KinkoData.JISIN(2)))
                        SQL.Append("," & SQ(KinkoData.JISIN(3)))
                        SQL.Append("," & SQ(KinkoData.JISIN(4)))
                        SQL.Append("," & SQ(KinkoData.JISIN(5)))
                        SQL.Append("," & SQ(KinkoData.JISIN(6)))
                        SQL.Append("," & SQ(KinkoData.JISIN(7)))
                        SQL.Append("," & SQ(KinkoData.JISIN(8)))
                        SQL.Append("," & SQ(KinkoData.IdoDate))
                        SQL.Append("," & SQ(KinkoData.IdoJiyuCode))
                        SQL.Append("," & SQ(KinkoData.NewKinCode))
                        SQL.Append("," & SQ(KinkoData.NewKinFukaCode))
                        SQL.Append("," & SQ(KinkoData.DeleteDate.Trim.PadLeft(8, "0"c)))
                        SQL.Append("," & SQ(System.DateTime.Now.ToString("yyyyMMddHHmmss")))
                        SQL.Append(")")

                        Dim ret As Integer = MainDB.ExecuteNonQuery(SQL)

                        If ret = 0 Then
                            JobMessage = "金融機関データ登録失敗"
                            MainLOG.Write("金融機関データ登録", "失敗", MainDB.Message)
                            Return False
                        End If

                        KinCnt += ret

                    Case "22"
                        ' 店舗データフォーマット
                        Dim TenpoData As New ClsFormatIdo.TenpoFormat

                        ' 店舗データ
                        Call TenpoData.Init()
                        TenpoData.Data = LineData

                        SQL.Length = 0
                        SQL.Append("INSERT INTO WRK_SITINFO(")
                        SQL.Append(" DATA_KBN_N")
                        SQL.Append(",DATA_SYBETU_N")
                        SQL.Append(",KIN_NO_N")
                        SQL.Append(",KIN_FUKA_N")
                        SQL.Append(",SIT_NO_N")
                        SQL.Append(",SIT_FUKA_N")
                        SQL.Append(",SIT_KNAME_N")
                        SQL.Append(",SIT_NNAME_N")
                        SQL.Append(",SEIDOKU_HYOUJI_N")
                        SQL.Append(",YUUBIN_N")
                        SQL.Append(",KJYU_N")
                        SQL.Append(",NJYU_N")
                        SQL.Append(",TKOUKAN_NO_N")
                        SQL.Append(",DENWA_N")
                        SQL.Append(",TENPO_ZOKUSEI_N")
                        SQL.Append(",JIKOU_HYOUJI_N")
                        SQL.Append(",FURI_HYOUJI_N")
                        SQL.Append(",SYUUTE_HYOUJI_N")
                        SQL.Append(",KAWASE_HYOUJI_N")
                        SQL.Append(",DAITE_HYOUJI_N")
                        SQL.Append(",JISIN_HYOUJI_N")
                        SQL.Append(",JC_CODE_N")
                        SQL.Append(",SIT_IDO_DATE_N")
                        SQL.Append(",SIT_IDO_CODE_N")
                        SQL.Append(",NEW_KIN_NO_N")
                        SQL.Append(",NEW_KIN_FUKA_N")
                        SQL.Append(",NEW_SIT_NO_N")
                        SQL.Append(",NEW_SIT_FUKA_N")
                        SQL.Append(",SIT_DEL_DATE_N")
                        SQL.Append(",TKOUKAN_NNAME_N")
                        SQL.Append(",SYORI_DATE_N")
                        SQL.Append(") VALUES (")
                        SQL.Append(" " & SQ(TenpoData.DataKubun))
                        SQL.Append("," & SQ(TenpoData.DataSyubetu))
                        SQL.Append("," & SQ(TenpoData.KinCode))
                        SQL.Append("," & SQ(TenpoData.KinFukaCode))
                        SQL.Append("," & SQ(TenpoData.TenCode))
                        SQL.Append("," & SQ(TenpoData.TenFukaCode))
                        SQL.Append("," & SQ(TenpoData.TenKana))
                        SQL.Append("," & SQ(TenpoData.TenKanji))
                        SQL.Append("," & SQ(TenpoData.SeiHyouji))
                        SQL.Append("," & SQ(TenpoData.Yubin))
                        SQL.Append("," & SQ(TenpoData.TenSyozaiKana))
                        SQL.Append("," & SQ(TenpoData.TenSyozaiKanji))
                        SQL.Append("," & SQ(TenpoData.TegataKoukan))
                        SQL.Append("," & SQ(TenpoData.TelNo))
                        SQL.Append("," & SQ(TenpoData.TenZokusei))
                        SQL.Append("," & SQ(TenpoData.JikoCenter))
                        SQL.Append("," & SQ(TenpoData.FuriCenter))
                        SQL.Append("," & SQ(TenpoData.SyuCenter))
                        SQL.Append("," & SQ(TenpoData.KawaseCenter))
                        SQL.Append("," & SQ(TenpoData.Daitegai))
                        SQL.Append("," & SQ(TenpoData.JisinHyoji))
                        SQL.Append("," & SQ(TenpoData.JCBango))
                        SQL.Append("," & SQ(TenpoData.IdoDate))
                        SQL.Append("," & SQ(TenpoData.IdoJiyuCode))
                        SQL.Append("," & SQ(TenpoData.NewKinCode))
                        SQL.Append("," & SQ(TenpoData.NewKinFukaCode))
                        SQL.Append("," & SQ(TenpoData.NewTenCode))
                        SQL.Append("," & SQ(TenpoData.NewTenFukaCode))
                        SQL.Append("," & SQ(TenpoData.DeleteDate.Trim.PadLeft(8, "0"c)))
                        SQL.Append("," & SQ(TenpoData.TegataKoukanKanji))
                        SQL.Append("," & SQ(System.DateTime.Now.ToString("yyyyMMddHHmmss")))
                        SQL.Append(")")

                        Dim ret As Integer = MainDB.ExecuteNonQuery(SQL)

                        If ret = 0 Then
                            JobMessage = "店舗データ登録失敗"
                            MainLOG.Write("店舗データ登録", "失敗", MainDB.Message)
                            Return False
                        End If

                        SitCnt += ret
                End Select

                If RecCnt Mod 1000 = 0 Then Console.Write("#")

                ReadLen = StrmReader.Read(LineData, 0, LineData.Length)
            Loop

            Console.WriteLine(" ")   '暫定
            MainLOG.Write("ワークマスタ登録", "成功", "レコード件数：" & RecCnt & "　金融機関レコード件数：" & KinCnt & "　店舗レコード件数：" & SitCnt)


            Return True

        Catch ex As Exception
            JobMessage = "ワークマスタ登録失敗"
            MainLOG.Write("ワークマスタ登録", "失敗", RecCnt & "レコード目　" & ex.Message)

            Return False

        Finally
            If Not StrmReader Is Nothing Then
                StrmReader.Close()
            End If
        End Try

    End Function

    'Private Function GetReader(ByRef oraReader As MyOracleReader) As Boolean

    '    Dim sql As New StringBuilder(256)

    '    Try
    '        sql.Append(" SELECT ")
    '        sql.Append(" WRK_KININFO.KIN_NO_N")
    '        sql.Append(",WRK_KININFO.KIN_FUKA_N")
    '        sql.Append(",WRK_KININFO.DAIHYO_NO_N")
    '        sql.Append(",WRK_KININFO.KIN_KNAME_N")
    '        sql.Append(",WRK_KININFO.KIN_NNAME_N")
    '        sql.Append(",WRK_KININFO.RYAKU_KIN_KNAME_N")
    '        sql.Append(",WRK_KININFO.RYAKU_KIN_NNAME_N")
    '        sql.Append(",WRK_KININFO.JC1_N")
    '        sql.Append(",WRK_KININFO.JC2_N")
    '        sql.Append(",WRK_KININFO.JC3_N")
    '        sql.Append(",WRK_KININFO.JC4_N")
    '        sql.Append(",WRK_KININFO.JC5_N")
    '        sql.Append(",WRK_KININFO.JC6_N")
    '        sql.Append(",WRK_KININFO.JC7_N")
    '        sql.Append(",WRK_KININFO.JC8_N")
    '        sql.Append(",WRK_KININFO.JC9_N")
    '        sql.Append(",WRK_KININFO.KIN_IDO_DATE_N")
    '        sql.Append(",WRK_KININFO.KIN_IDO_CODE_N")
    '        sql.Append(",WRK_KININFO.NEW_KIN_NO_N")
    '        sql.Append(",WRK_KININFO.NEW_KIN_FUKA_N")
    '        sql.Append(",WRK_KININFO.KIN_DEL_DATE_N")

    '        sql.Append(",WRK_SITINFO.SIT_NO_N")
    '        sql.Append(",WRK_SITINFO.SIT_FUKA_N")
    '        sql.Append(",WRK_SITINFO.SIT_KNAME_N")
    '        sql.Append(",WRK_SITINFO.SIT_NNAME_N")
    '        sql.Append(",WRK_SITINFO.SEIDOKU_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.YUUBIN_N")
    '        sql.Append(",WRK_SITINFO.KJYU_N")
    '        sql.Append(",WRK_SITINFO.NJYU_N")
    '        sql.Append(",WRK_SITINFO.TKOUKAN_NO_N")
    '        sql.Append(",WRK_SITINFO.DENWA_N")
    '        sql.Append(",WRK_SITINFO.TENPO_ZOKUSEI_N")
    '        sql.Append(",WRK_SITINFO.JIKOU_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.FURI_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.SYUUTE_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.KAWASE_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.DAITE_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.JISIN_HYOUJI_N")
    '        sql.Append(",WRK_SITINFO.JC_CODE_N")
    '        sql.Append(",WRK_SITINFO.SIT_IDO_DATE_N")
    '        sql.Append(",WRK_SITINFO.SIT_IDO_CODE_N")
    '        sql.Append(",WRK_SITINFO.NEW_KIN_NO_N AS SITEN_NEW_KIN_NO_N")
    '        sql.Append(",WRK_SITINFO.NEW_KIN_FUKA_N AS SITEN_NEW_KIN_FUKA_N")
    '        sql.Append(",WRK_SITINFO.NEW_SIT_NO_N")
    '        sql.Append(",WRK_SITINFO.NEW_SIT_FUKA_N")
    '        sql.Append(",WRK_SITINFO.SIT_DEL_DATE_N")
    '        sql.Append(",WRK_SITINFO.TKOUKAN_NNAME_N")
    '        sql.Append(" FROM ")
    '        sql.Append(" WRK_KININFO ")
    '        sql.Append(" ,WRK_SITINFO ")
    '        sql.Append(" WHERE ")
    '        sql.Append(" WRK_KININFO.KIN_NO_N = WRK_SITINFO.KIN_NO_N ")
    '        sql.Append(" AND WRK_KININFO.KIN_FUKA_N = WRK_SITINFO.KIN_FUKA_N ")
    '        sql.Append(" AND (WRK_KININFO.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_KININFO.KIN_DEL_DATE_N = '00000000')")
    '        sql.Append(" AND (WRK_SITINFO.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_SITINFO.SIT_DEL_DATE_N = '00000000')")
    '        sql.Append(" ORDER BY WRK_KININFO.KIN_NO_N, WRK_KININFO.KIN_FUKA_N, WRK_SITINFO.SIT_NO_N, WRK_SITINFO.SIT_FUKA_N, WRK_KININFO.KIN_DEL_DATE_N, WRK_SITINFO.SIT_DEL_DATE_N")

    '        If oraReader.DataReader(sql) = False Then
    '            JobMessage = "金融機関履歴マスタ検索失敗"
    '            MainLOG.Write("金融機関履歴マスタ検索", "失敗", "")
    '            Return False
    '        End If

    '        Return True

    '    Catch ex As Exception
    '        Throw
    '    Finally

    '    End Try

    'End Function

    Private Function GetSItReader(ByRef oraReader As MyOracleReader) As Boolean

        Dim sql As New StringBuilder(2048)

        Try
            sql.Append(" SELECT ")
            sql.Append(" WRK_SITINFO.KIN_NO_N")
            sql.Append(",WRK_SITINFO.KIN_FUKA_N")
            sql.Append(",WRK_SITINFO.SIT_NO_N")
            sql.Append(",WRK_SITINFO.SIT_FUKA_N")
            sql.Append(",WRK_SITINFO.SIT_KNAME_N")
            sql.Append(",WRK_SITINFO.SIT_NNAME_N")
            sql.Append(",WRK_SITINFO.SEIDOKU_HYOUJI_N")
            sql.Append(",WRK_SITINFO.YUUBIN_N")
            sql.Append(",WRK_SITINFO.KJYU_N")
            sql.Append(",WRK_SITINFO.NJYU_N")
            sql.Append(",WRK_SITINFO.TKOUKAN_NO_N")
            sql.Append(",WRK_SITINFO.DENWA_N")
            sql.Append(",WRK_SITINFO.TENPO_ZOKUSEI_N")
            sql.Append(",WRK_SITINFO.JIKOU_HYOUJI_N")
            sql.Append(",WRK_SITINFO.FURI_HYOUJI_N")
            sql.Append(",WRK_SITINFO.SYUUTE_HYOUJI_N")
            sql.Append(",WRK_SITINFO.KAWASE_HYOUJI_N")
            sql.Append(",WRK_SITINFO.DAITE_HYOUJI_N")
            sql.Append(",WRK_SITINFO.JISIN_HYOUJI_N")
            sql.Append(",WRK_SITINFO.JC_CODE_N")
            sql.Append(",WRK_SITINFO.SIT_IDO_DATE_N")
            sql.Append(",WRK_SITINFO.SIT_IDO_CODE_N")
            sql.Append(",WRK_SITINFO.NEW_KIN_NO_N")
            sql.Append(",WRK_SITINFO.NEW_KIN_FUKA_N")
            sql.Append(",WRK_SITINFO.NEW_SIT_NO_N")
            sql.Append(",WRK_SITINFO.NEW_SIT_FUKA_N")
            sql.Append(",WRK_SITINFO.SIT_DEL_DATE_N")
            sql.Append(",WRK_SITINFO.TKOUKAN_NNAME_N")
            sql.Append(",WRK_CNT,INF_CNT") '2011/03/30 レコード件数追加
            sql.Append(" FROM ")
            sql.Append(" WRK_SITINFO ")
            '2011/03/30 レコード件数集計クエリ ここから
            'WRK_SITINFOのレコード件数を集計する
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) WRK_CNT")
            sql.Append(",KIN_NO_N WRK_KIN_NO")
            sql.Append(",KIN_FUKA_N WRK_KIN_FUKA")
            sql.Append(",SIT_NO_N WRK_SIT_NO")
            sql.Append(",SIT_FUKA_N WRK_SIT_FUKA")
            sql.Append(" FROM ")
            sql.Append(" WRK_SITINFO ")
            sql.Append(" WHERE ")
            sql.Append("  (WRK_SITINFO.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_SITINFO.SIT_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N")
            sql.Append(",SIT_NO_N")
            sql.Append(",SIT_FUKA_N)")
            'SITEN_INFOMASTのレコード件数を集計する
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) INF_CNT")
            sql.Append(",KIN_NO_N INF_KIN_NO")
            sql.Append(",KIN_FUKA_N INF_KIN_FUKA")
            sql.Append(",SIT_NO_N INF_SIT_NO")
            sql.Append(",SIT_FUKA_N INF_SIT_FUKA")
            sql.Append(" FROM ")
            sql.Append(" SITEN_INFOMAST ")
            sql.Append(" WHERE ")
            sql.Append("  (SITEN_INFOMAST.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR SITEN_INFOMAST.SIT_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N")
            sql.Append(",SIT_NO_N")
            sql.Append(",SIT_FUKA_N)")
            '2011/03/30 レコード件数集計クエリ ここまで
            sql.Append(" WHERE ")
            sql.Append(" (WRK_SITINFO.SIT_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_SITINFO.SIT_DEL_DATE_N = '00000000')")
            '2011/03/30 結合条件追加 ここから
            sql.Append(" AND KIN_NO_N = WRK_KIN_NO")
            sql.Append(" AND KIN_FUKA_N = WRK_KIN_FUKA")
            sql.Append(" AND SIT_NO_N = WRK_SIT_NO")
            sql.Append(" AND SIT_FUKA_N = WRK_SIT_FUKA")
            sql.Append(" AND KIN_NO_N = INF_KIN_NO(+)")
            sql.Append(" AND KIN_FUKA_N = INF_KIN_FUKA(+)")
            sql.Append(" AND SIT_NO_N = INF_SIT_NO(+)")
            sql.Append(" AND SIT_FUKA_N = INF_SIT_FUKA(+)")
            '2011/03/30 結合条件追加 ここまで
            sql.Append(" ORDER BY WRK_SITINFO.KIN_NO_N, WRK_SITINFO.KIN_FUKA_N, WRK_SITINFO.SIT_NO_N, WRK_SITINFO.SIT_FUKA_N, WRK_SITINFO.SIT_DEL_DATE_N")

            If oraReader.DataReader(sql) = False Then
                'JobMessage = "支店情報ワークマスタ検索失敗"
                MainLOG.Write("支店情報ワークマスタ検索", "成功", "対象なし")


                'Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        Finally

        End Try

    End Function

    Private Function GetKinReader(ByRef oraReader As MyOracleReader) As Boolean

        Dim sql As New StringBuilder(2048)

        Try
            sql.Append(" SELECT ")
            sql.Append(" WRK_KININFO.KIN_NO_N")
            sql.Append(",WRK_KININFO.KIN_FUKA_N")
            sql.Append(",WRK_KININFO.DAIHYO_NO_N")
            sql.Append(",WRK_KININFO.KIN_KNAME_N")
            sql.Append(",WRK_KININFO.KIN_NNAME_N")
            sql.Append(",WRK_KININFO.RYAKU_KIN_KNAME_N")
            sql.Append(",WRK_KININFO.RYAKU_KIN_NNAME_N")
            sql.Append(",WRK_KININFO.JC1_N")
            sql.Append(",WRK_KININFO.JC2_N")
            sql.Append(",WRK_KININFO.JC3_N")
            sql.Append(",WRK_KININFO.JC4_N")
            sql.Append(",WRK_KININFO.JC5_N")
            sql.Append(",WRK_KININFO.JC6_N")
            sql.Append(",WRK_KININFO.JC7_N")
            sql.Append(",WRK_KININFO.JC8_N")
            sql.Append(",WRK_KININFO.JC9_N")
            sql.Append(",WRK_KININFO.KIN_IDO_DATE_N")
            sql.Append(",WRK_KININFO.KIN_IDO_CODE_N")
            sql.Append(",WRK_KININFO.NEW_KIN_NO_N")
            sql.Append(",WRK_KININFO.NEW_KIN_FUKA_N")
            sql.Append(",WRK_KININFO.KIN_DEL_DATE_N")
            sql.Append(",WRK_CNT,INF_CNT") '2011/03/30 レコード件数追加

            sql.Append(" FROM ")
            sql.Append(" WRK_KININFO ")
            '2011/03/30 レコード件数集計クエリ ここから
            'WRK_KININFOのレコード件数を集計する
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) WRK_CNT")
            sql.Append(",KIN_NO_N WRK_KIN_NO")
            sql.Append(",KIN_FUKA_N WRK_KIN_FUKA")
            sql.Append(" FROM ")
            sql.Append(" WRK_KININFO ")
            sql.Append(" WHERE ")
            sql.Append("  (WRK_KININFO.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_KININFO.KIN_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N)")
            'KIN_INFOMASTのレコード件数を集計する
            sql.Append(",(SELECT")
            sql.Append(" COUNT(*) INF_CNT")
            sql.Append(",KIN_NO_N INF_KIN_NO")
            sql.Append(",KIN_FUKA_N INF_KIN_FUKA")
            sql.Append(" FROM ")
            sql.Append(" KIN_INFOMAST ")
            sql.Append(" WHERE ")
            sql.Append("  (KIN_INFOMAST.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR KIN_INFOMAST.KIN_DEL_DATE_N = '00000000')")
            sql.Append(" GROUP BY ")
            sql.Append(" KIN_NO_N")
            sql.Append(",KIN_FUKA_N)")
            '2011/03/30 レコード件数集計クエリ ここまで

            sql.Append(" WHERE ")
            sql.Append("  (WRK_KININFO.KIN_DEL_DATE_N >= " & SQ(mParam.SyoriDate) & " OR WRK_KININFO.KIN_DEL_DATE_N = '00000000')")
            '2011/03/30 結合条件追加 ここから
            sql.Append(" AND KIN_NO_N = WRK_KIN_NO")
            sql.Append(" AND KIN_FUKA_N = WRK_KIN_FUKA")
            sql.Append(" AND KIN_NO_N = INF_KIN_NO(+)")
            sql.Append(" AND KIN_FUKA_N = INF_KIN_FUKA(+)")
            '2011/03/30 結合条件追加 ここまで
            sql.Append(" ORDER BY WRK_KININFO.KIN_NO_N, WRK_KININFO.KIN_FUKA_N,  WRK_KININFO.KIN_DEL_DATE_N")

            If oraReader.DataReader(sql) = False Then
                'JobMessage = "金融機関情報ワークマスタ検索失敗"
                MainLOG.Write("金融機関情報ワークマスタ検索", "成功", "対象なし")
                'Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        Finally

        End Try

    End Function

    Private Function RenewKinInfo(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean

        Dim sql As StringBuilder
        Dim iRet As Integer
        Dim RecCnt As Integer = 0

        Try

            Console.WriteLine("金融機関情報マスタ作成中")

            Do Until oraReader.EOF

                '2017/01/18 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                '金融機関情報マスタアップデートSQL作成時に取得していた提携区分を外出し
                'あらかじめ取得しておいて性能劣化を防ぐ
                Dim strTeikeiKbn As String = Me.GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))
                '2017/01/18 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

                '素直にアップデートインサートしてみる

                sql = New StringBuilder(2048)

                '2011/03/30 レコード集計結果によってクエリを分ける ここから
                If oraReader.GetInt("INF_CNT") >= 1 AndAlso oraReader.GetInt("WRK_CNT") = 1 Then
                    '金融機関情報マスタに1レコード以上、ワークマスタに1レコードの場合
                    '削除日なしのレコードを更新する
                    sql.Append("UPDATE KIN_INFOMAST SET")
                    sql.Append(" DAIHYO_NO_N = " & SQ(oraReader.GetString("DAIHYO_NO_N")))
                    sql.Append(",KIN_KNAME_N = " & SQ(oraReader.GetString("KIN_KNAME_N")))
                    sql.Append(",KIN_NNAME_N = " & SQ(oraReader.GetString("KIN_NNAME_N")))
                    sql.Append(",RYAKU_KIN_KNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
                    sql.Append(",RYAKU_KIN_NNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
                    sql.Append(",JC1_N = " & SQ(oraReader.GetString("JC1_N")))
                    sql.Append(",JC2_N = " & SQ(oraReader.GetString("JC2_N")))
                    sql.Append(",JC3_N = " & SQ(oraReader.GetString("JC3_N")))
                    sql.Append(",JC4_N = " & SQ(oraReader.GetString("JC4_N")))
                    sql.Append(",JC5_N = " & SQ(oraReader.GetString("JC5_N")))
                    sql.Append(",JC6_N = " & SQ(oraReader.GetString("JC6_N")))
                    sql.Append(",JC7_N = " & SQ(oraReader.GetString("JC7_N")))
                    sql.Append(",JC8_N = " & SQ(oraReader.GetString("JC8_N")))
                    sql.Append(",JC9_N = " & SQ(oraReader.GetString("JC9_N")))
                    sql.Append(",KIN_IDO_DATE_N = " & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
                    sql.Append(",KIN_IDO_CODE_N = " & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append(",KIN_DEL_DATE_N = " & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                    '提携区分、金融機関種類、地区コード、支払手数料の設定
                    sql.Append(",TEIKEI_KBN_N = " & SQ(strTeikeiKbn))
                    sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(0)))
                    sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(1)))
                    sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(2)))
                    'sql.Append(",TEIKEI_KBN_N = " & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
                    'sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
                    'sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
                    'sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))

                    sql.Append(" WHERE KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append(" AND KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append(" AND KIN_DEL_DATE_N = '00000000'")
                Else
                    '2011/03/30 レコード集計結果によってクエリを分ける ここまで
                    sql.Append("MERGE INTO KIN_INFOMAST")
                    sql.Append(" USING (SELECT")
                    sql.Append("  " & SQ(oraReader.GetString("KIN_NO_N")) & " KIN_NO")
                    sql.Append(", " & SQ(oraReader.GetString("KIN_FUKA_N")) & " KIN_FUKA")
                    sql.Append(", " & SQ(oraReader.GetString("KIN_DEL_DATE_N")) & " KIN_DEL_DATE")
                    sql.Append(" FROM DUAL)")
                    sql.Append(" ON (KIN_NO_N = KIN_NO")
                    sql.Append(" AND KIN_FUKA_N = KIN_FUKA")
                    sql.Append(" AND KIN_DEL_DATE_N = KIN_DEL_DATE)")
                    'UPDATE句
                    sql.Append(" WHEN MATCHED THEN")
                    sql.Append(" UPDATE SET")
                    'sql.Append(" KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    'sql.Append(",KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append(" DAIHYO_NO_N = " & SQ(oraReader.GetString("DAIHYO_NO_N")))
                    sql.Append(",KIN_KNAME_N = " & SQ(oraReader.GetString("KIN_KNAME_N")))
                    sql.Append(",KIN_NNAME_N = " & SQ(oraReader.GetString("KIN_NNAME_N")))
                    sql.Append(",RYAKU_KIN_KNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
                    sql.Append(",RYAKU_KIN_NNAME_N = " & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
                    sql.Append(",JC1_N = " & SQ(oraReader.GetString("JC1_N")))
                    sql.Append(",JC2_N = " & SQ(oraReader.GetString("JC2_N")))
                    sql.Append(",JC3_N = " & SQ(oraReader.GetString("JC3_N")))
                    sql.Append(",JC4_N = " & SQ(oraReader.GetString("JC4_N")))
                    sql.Append(",JC5_N = " & SQ(oraReader.GetString("JC5_N")))
                    sql.Append(",JC6_N = " & SQ(oraReader.GetString("JC6_N")))
                    sql.Append(",JC7_N = " & SQ(oraReader.GetString("JC7_N")))
                    sql.Append(",JC8_N = " & SQ(oraReader.GetString("JC8_N")))
                    sql.Append(",JC9_N = " & SQ(oraReader.GetString("JC9_N")))
                    sql.Append(",KIN_IDO_DATE_N = " & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
                    sql.Append(",KIN_IDO_CODE_N = " & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    'sql.Append(",KIN_DEL_DATE_N = " & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                    '提携区分、金融機関種類、地区コード、支払手数料の設定
                    sql.Append(",TEIKEI_KBN_N = " & SQ(strTeikeiKbn))
                    sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(0)))
                    sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(1)))
                    sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(2)))
                    'sql.Append(",TEIKEI_KBN_N = " & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
                    'sql.Append(",SYUBETU_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
                    'sql.Append(",TIKU_CODE_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
                    'sql.Append(",TESUU_TANKA_N = " & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    'sql.Append(",SAKUSEI_DATE_N = " & SQ(mParam.SyoriDate))
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))
                    'INSERT句
                    sql.Append(" WHEN NOT MATCHED THEN")
                    sql.Append(" INSERT VALUES(")
                    sql.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("DAIHYO_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_KNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_NNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC1_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC2_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC3_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC4_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC5_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC6_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC7_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC8_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC9_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                    '提携区分、金融機関種類、地区コード、支払手数料の設定
                    sql.Append("," & SQ(strTeikeiKbn))
                    sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(0)))
                    sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(1)))
                    sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), strTeikeiKbn)(2)))
                    'sql.Append("," & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
                    'sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
                    'sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
                    'sql.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append(")")
                End If '2011/03/30 If閉じる

                iRet = MainDB.ExecuteNonQuery(sql)

                If iRet <> 1 Then
                    JobMessage = "金融機関情報マスタ更新失敗"
                    MainLOG.Write("金融機関情報マスタ更新", "失敗", "更新件数不明:" & iRet)
                    Return False
                End If

                RecCnt += 1

                oraReader.NextRead()

                If RecCnt Mod 100 = 0 Then Console.Write("#")

            Loop

            Console.WriteLine(" ")

            JobMessage = "処理件数：" & RecCnt
            MainLOG.Write("金融機関情報マスタ更新", "成功", JobMessage)


            Return True

        Catch ex As Exception
            Throw
        End Try

    End Function


    Private Function RenewSitInfo(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean

        Dim sql As StringBuilder
        Dim iRet As Integer
        Dim RecCnt As Integer = 0

        Try

            Console.WriteLine("支店情報マスタ作成中")

            Do Until oraReader.EOF

                '素直にアップデートインサートしてみる

                sql = New StringBuilder(2048)

                '2011/03/30 レコード集計結果によってクエリを分ける ここから
                If oraReader.GetInt("INF_CNT") >= 1 AndAlso oraReader.GetInt("WRK_CNT") = 1 Then
                    '支店情報マスタに1レコード以上、ワークマスタに1レコードの場合
                    '削除日なしのレコードを更新する
                    sql.Append("UPDATE SITEN_INFOMAST SET")
                    sql.Append(" SIT_KNAME_N = " & SQ(oraReader.GetString("SIT_KNAME_N")))
                    sql.Append(",SIT_NNAME_N = " & SQ(oraReader.GetString("SIT_NNAME_N")))
                    sql.Append(",SEIDOKU_HYOUJI_N = " & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
                    sql.Append(",YUUBIN_N = " & SQ(oraReader.GetString("YUUBIN_N")))
                    sql.Append(",KJYU_N = " & SQ(oraReader.GetString("KJYU_N")))
                    sql.Append(",NJYU_N = " & SQ(oraReader.GetString("NJYU_N")))
                    sql.Append(",TKOUKAN_NO_N = " & SQ(oraReader.GetString("TKOUKAN_NO_N")))
                    sql.Append(",DENWA_N = " & SQ(oraReader.GetString("DENWA_N")))
                    sql.Append(",TENPO_ZOKUSEI_N = " & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
                    sql.Append(",JIKOU_HYOUJI_N = " & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
                    sql.Append(",FURI_HYOUJI_N = " & SQ(oraReader.GetString("FURI_HYOUJI_N")))
                    sql.Append(",SYUUTE_HYOUJI_N = " & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
                    sql.Append(",KAWASE_HYOUJI_N = " & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
                    sql.Append(",DAITE_HYOUJI_N = " & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
                    sql.Append(",JISIN_HYOUJI_N = " & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
                    sql.Append(",JC_CODE_N = " & SQ(oraReader.GetString("JC_CODE_N")))
                    sql.Append(",SIT_IDO_DATE_N = " & SQ(oraReader.GetString("SIT_IDO_DATE_N")))
                    sql.Append(",SIT_IDO_CODE_N = " & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append(",NEW_SIT_NO_N = " & SQ(oraReader.GetString("NEW_SIT_NO_N")))
                    sql.Append(",NEW_SIT_FUKA_N = " & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
                    sql.Append(",SIT_DEL_DATE_N = " & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
                    sql.Append(",TKOUKAN_NNAME_N = " & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))

                    sql.Append(" WHERE KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append(" AND KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append(" AND SIT_NO_N = " & SQ(oraReader.GetString("SIT_NO_N")))
                    sql.Append(" AND SIT_FUKA_N = " & SQ(oraReader.GetString("SIT_FUKA_N")))
                    sql.Append(" AND SIT_DEL_DATE_N = '00000000'")
                Else
                    '2011/03/30 レコード集計結果によってクエリを分ける ここまで
                    sql.Append("MERGE INTO SITEN_INFOMAST")
                    sql.Append(" USING (SELECT")
                    sql.Append("  " & SQ(oraReader.GetString("KIN_NO_N")) & " KIN_NO")
                    sql.Append(", " & SQ(oraReader.GetString("KIN_FUKA_N")) & " KIN_FUKA")
                    sql.Append(", " & SQ(oraReader.GetString("SIT_NO_N")) & " SIT_NO")
                    sql.Append(", " & SQ(oraReader.GetString("SIT_FUKA_N")) & " SIT_FUKA")
                    sql.Append(", " & SQ(oraReader.GetString("SIT_DEL_DATE_N")) & " SIT_DEL_DATE")
                    sql.Append(" FROM DUAL)")
                    sql.Append(" ON (KIN_NO_N = KIN_NO")
                    sql.Append(" AND KIN_FUKA_N = KIN_FUKA")
                    sql.Append(" AND SIT_NO_N = SIT_NO")
                    sql.Append(" AND SIT_FUKA_N = SIT_FUKA")
                    sql.Append(" AND SIT_DEL_DATE_N = SIT_DEL_DATE)")
                    'UPDATE句
                    sql.Append(" WHEN MATCHED THEN")
                    sql.Append(" UPDATE SET")
                    'sql.Append(" KIN_NO_N = " & SQ(oraReader.GetString("KIN_NO_N")))
                    'sql.Append(",KIN_FUKA_N = " & SQ(oraReader.GetString("KIN_FUKA_N")))
                    'sql.Append(",SIT_NO_N = " & SQ(oraReader.GetString("SIT_NO_N")))
                    'sql.Append(",SIT_FUKA_N = " & SQ(oraReader.GetString("SIT_FUKA_N")))
                    sql.Append(" SIT_KNAME_N = " & SQ(oraReader.GetString("SIT_KNAME_N")))
                    sql.Append(",SIT_NNAME_N = " & SQ(oraReader.GetString("SIT_NNAME_N")))
                    sql.Append(",SEIDOKU_HYOUJI_N = " & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
                    sql.Append(",YUUBIN_N = " & SQ(oraReader.GetString("YUUBIN_N")))
                    sql.Append(",KJYU_N = " & SQ(oraReader.GetString("KJYU_N")))
                    sql.Append(",NJYU_N = " & SQ(oraReader.GetString("NJYU_N")))
                    sql.Append(",TKOUKAN_NO_N = " & SQ(oraReader.GetString("TKOUKAN_NO_N")))
                    sql.Append(",DENWA_N = " & SQ(oraReader.GetString("DENWA_N")))
                    sql.Append(",TENPO_ZOKUSEI_N = " & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
                    sql.Append(",JIKOU_HYOUJI_N = " & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
                    sql.Append(",FURI_HYOUJI_N = " & SQ(oraReader.GetString("FURI_HYOUJI_N")))
                    sql.Append(",SYUUTE_HYOUJI_N = " & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
                    sql.Append(",KAWASE_HYOUJI_N = " & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
                    sql.Append(",DAITE_HYOUJI_N = " & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
                    sql.Append(",JISIN_HYOUJI_N = " & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
                    sql.Append(",JC_CODE_N = " & SQ(oraReader.GetString("JC_CODE_N")))
                    sql.Append(",SIT_IDO_DATE_N = " & SQ(oraReader.GetString("SIT_IDO_DATE_N")))
                    sql.Append(",SIT_IDO_CODE_N = " & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
                    sql.Append(",NEW_KIN_NO_N = " & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append(",NEW_KIN_FUKA_N = " & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append(",NEW_SIT_NO_N = " & SQ(oraReader.GetString("NEW_SIT_NO_N")))
                    sql.Append(",NEW_SIT_FUKA_N = " & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
                    'sql.Append(",SIT_DEL_DATE_N = " & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
                    sql.Append(",TKOUKAN_NNAME_N = " & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
                    sql.Append(",KOUSIN_DATE_N = " & SQ(mParam.SyoriDate))
                    'INSERT句
                    sql.Append(" WHEN NOT MATCHED THEN")
                    sql.Append(" INSERT VALUES(")
                    sql.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_KNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_NNAME_N")))
                    sql.Append("," & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("YUUBIN_N")))
                    sql.Append("," & SQ(oraReader.GetString("KJYU_N")))
                    sql.Append("," & SQ(oraReader.GetString("NJYU_N")))
                    sql.Append("," & SQ(oraReader.GetString("TKOUKAN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("DENWA_N")))
                    sql.Append("," & SQ(" "))
                    sql.Append("," & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
                    sql.Append("," & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("FURI_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
                    sql.Append("," & SQ(oraReader.GetString("JC_CODE_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_IDO_DATE_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_SIT_NO_N")))
                    sql.Append("," & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
                    sql.Append("," & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
                    sql.Append("," & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append("," & SQ(mParam.SyoriDate))
                    sql.Append(")")
                End If '2011/03/30 If閉じる

                iRet = MainDB.ExecuteNonQuery(sql)

                If iRet <> 1 Then
                    JobMessage = "支店情報マスタ更新失敗"
                    MainLOG.Write("支店情報マスタ更新", "失敗", "更新件数不明:" & iRet)

                    Return False
                End If

                oraReader.NextRead()

                RecCnt += 1

                If RecCnt Mod 1000 = 0 Then Console.Write("#")

            Loop


            Console.WriteLine(" ")

            JobMessage = "処理件数：" & RecCnt
            MainLOG.Write("支店情報マスタ更新", "成功", JobMessage)


            Return True

        Catch ex As Exception

            Throw

        End Try


        'Dim sql As StringBuilder

        'Try
        '    Dim ret As Integer
        '    Dim InsertCnt As Integer = 0        'INSERT件数
        '    Dim UpdateCnt As Integer = 0        'UPDATE件数
        '    RecCnt = 0

        '    oraReader = New MyOracleReader(MainDB)


        '    Do Until oraReader.EOF
        '        RecCnt += 1

        '        '異動事由コードで分岐
        '        Select Case oraReader.GetString("SIT_IDO_CODE")
        '            Case "00", "01"
        '                ' 初期登録，新設
        '                ret = 0

        '            Case "02", "03", "05", "09"
        '                ' 廃止，廃止継承，変更前，営業譲渡（変更前）

        '                '削除日、金融機関削除日、異動日、更新日をUPDATE
        '                sql.Length = 0
        '                sql.Append("UPDATE TENMAST SET ")
        '                sql.Append(" DEL_DATE_N     = '" & TENVIEW.DEL_DATE & "'")
        '                sql.Append(",DEL_KIN_DATE_N = '" & TENVIEW.DEL_KIN_DATE & "'")
        '                If TENVIEW.SIT_IDO_CODE = "02" Then
        '                    sql.Append(",YOBI5_N        = '" & TENVIEW.SIT_IDO_DATE & "'")
        '                End If
        '                sql.Append(",KOUSIN_DATE_N  = '" & SYORIDATE & "'")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                '未更新のレコードの最小の枝番を更新する
        '                sql.Append("   AND EDA_N    = (")
        '                sql.Append("SELECT MIN(EDA_N) FROM TENMAST")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                sql.Append("   AND YOBI1_N  = '" & TENVIEW.KIN_FUKA & "'")
        '                sql.Append("   AND YOBI2_N  = '" & TENVIEW.SIT_FUKA & "'")
        '                '未更新のレコードのみ対象
        '                sql.Append("   AND KOUSIN_DATE_N <> '" & SYORIDATE & "'")
        '                sql.Append(")")

        '                ret = MainDB.ExecuteNonQuery(sql)
        '                UpdateCnt += ret

        '                '0～1件しかありえないはず・・・
        '                If ret > 1 Then
        '                    JobMessage = "金融機関マスタUPDATE件数異常"
        '                    LOG.Write("金融機関マスタUPDATE", "失敗", "件数異常：" & sql.ToString)
        '                    Return False
        '                End If

        '            Case "04", "06", "10"
        '                ' 変更後，営業譲渡（変更後），その他変更

        '                'レコードの全情報をUPDATE
        '                sql.Length = 0
        '                sql.Append("UPDATE TENMAST SET ")
        '                sql.Append(" KIN_KNAME_N    = '" & TENVIEW.KIN_KNAME & "'")
        '                sql.Append(",KIN_NNAME_N    = '" & TENVIEW.KIN_NNAME & "'")
        '                sql.Append(",SIT_KNAME_N    = '" & TENVIEW.SIT_KNAME & "'")
        '                sql.Append(",SIT_NNAME_N    = '" & TENVIEW.SIT_NNAME & "'")
        '                sql.Append(",DENWA_N        = '" & TENVIEW.DENWA & "'")
        '                sql.Append(",YUUBIN_N       = '" & TENVIEW.YUUBIN & "'")
        '                sql.Append(",KJYU_N         = '" & TENVIEW.KJYUSYO & "'")
        '                sql.Append(",NJYU_N         = '" & TENVIEW.NJYUSYO & "'")
        '                sql.Append(",DEL_DATE_N     = '" & TENVIEW.DEL_DATE & "'")
        '                sql.Append(",DEL_KIN_DATE_N = '" & TENVIEW.DEL_KIN_DATE & "'")
        '                sql.Append(",KOUSIN_DATE_N  = '" & SYORIDATE & "'")
        '                sql.Append(",YOBI3_N        = '" & TENVIEW.TEN_ZOKUSEI & "'")
        '                sql.Append(",YOBI4_N        = '" & TENVIEW.SEIDOKU & "'")
        '                'SQL.Append(",YOBI5_N        = '" & TENVIEW.SIT_IDO_DATE & "'")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                '未更新のレコードの最小の枝番を更新する
        '                sql.Append("   AND EDA_N    = (")
        '                sql.Append("SELECT MIN(EDA_N) FROM TENMAST")
        '                sql.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
        '                sql.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "'")
        '                sql.Append("   AND YOBI1_N  = '" & TENVIEW.KIN_FUKA & "'")
        '                sql.Append("   AND YOBI2_N  = '" & TENVIEW.SIT_FUKA & "'")
        '                '未更新のレコードのみ対象
        '                sql.Append("   AND KOUSIN_DATE_N <> '" & SYORIDATE & "'")
        '                sql.Append(")")

        '                ret = MainDB.ExecuteNonQuery(sql)
        '                UpdateCnt += ret

        '                '0～1件しかありえないはず・・・
        '                If ret > 1 Then
        '                    JobMessage = "金融機関マスタUPDATE件数異常"
        '                    LOG.Write("金融機関マスタUPDATE", "失敗", "件数異常：" & sql.ToString)
        '                    Return False
        '                End If

        '            Case Else
        '                JobMessage = "店舗異動事由コード異常"
        '                LOG.Write("金融機関マスタ更新", "失敗", "店舗異動事由コード異常：" & TENVIEW.SIT_IDO_CODE)
        '                Return False
        '        End Select

        '        'UPDATE件数が0の場合はINSERTする
        '        If ret = 0 Then
        '            ret = InsertTENMAST()
        '            InsertCnt += ret
        '        End If

        '        '件数が-1の場合はエラー
        '        If ret = -1 Then
        '            Return False
        '        End If

        '    Loop While oraReader.NextRead = True

        '    '処理件数・・・ワークマスタのレコード件数
        '    '登録件数・・・・・INSERT件数
        '    '更新件数・・・・・UPDATE件数
        '    '削除件数・・・・・DELETE件数
        '    '※処理件数 = (登録件数 + 更新件数)の数が合わない場合は、何らかの原因で二重登録チェックに引っかかっている
        '    JobMessage = "処理件数：" & RecCnt & "　登録件数：" & InsertCnt & "　更新件数：" & UpdateCnt & "　削除件数：" & DelCnt
        '    LOG.Write("金融機関マスタ更新", "成功", JobMessage)

        'Catch ex As Exception
        '    JobMessage = "金融機関マスタ更新失敗"
        '    LOG.Write("金融機関マスタ更新", "失敗", ex.Message)
        '    Return False

        'Finally
        '    oraReader.Close()
        'End Try

    End Function


    '' 金融機関マスタにデータを登録する
    'Private Function InsertTENMAST(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean

    '    Dim iRet As Integer
    '    Dim SQL As StringBuilder
    '    Dim RecCnt As Integer = 0
    '    Dim KinOldKey As String = ""
    '    Dim KinNewKey As String = ""
    '    Try

    '        Console.WriteLine("金融機関マスタインサート中")

    '        Do Until oraReader.EOF

    '            ''同じ金融機関情報が存在する場合は登録しない(二重登録防止)
    '            'If CheckSameData(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"), _
    '            '                 oraReader.GetString("SIT_NO_N"), oraReader.GetString("SIT_FUKA_N")) = True Then

    '            SQL = New StringBuilder(128)

    '            SQL.Append("INSERT INTO TENMAST(")
    '            SQL.Append(" KIN_NO_N")
    '            SQL.Append(",KIN_FUKA_N")
    '            SQL.Append(",SIT_NO_N")
    '            SQL.Append(",SIT_FUKA_N")
    '            SQL.Append(",KIN_KNAME_N")
    '            SQL.Append(",KIN_NNAME_N")
    '            SQL.Append(",SIT_KNAME_N")
    '            SQL.Append(",SIT_NNAME_N")
    '            SQL.Append(",NEW_KIN_NO_N")
    '            SQL.Append(",NEW_KIN_FUKA_N")
    '            SQL.Append(",NEW_SIT_NO_N")
    '            SQL.Append(",NEW_SIT_FUKA_N")
    '            SQL.Append(",KIN_DEL_DATE_N")
    '            SQL.Append(",SIT_DEL_DATE_N")
    '            SQL.Append(",SAKUSEI_DATE_N")
    '            SQL.Append(",KOUSIN_DATE_N")
    '            SQL.Append(")")
    '            SQL.Append(" VALUES(")
    '            SQL.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_FUKA_N")))
    '            '枝番をカウントアップする
    '            'SQL.Append(",(SELECT")
    '            'SQL.Append(" TRIM(TO_CHAR(NVL(MAX(EDA_N), 0) + 1, '00'))")      ' EDA_N
    '            'SQL.Append(" FROM TENMAST")
    '            'SQL.Append(" WHERE KIN_NO_N = '" & TENVIEW.KIN_NO & "'")
    '            'SQL.Append("   AND SIT_NO_N = '" & TENVIEW.SIT_NO & "')")

    '            SQL.Append("," & SQ(oraReader.GetString("KIN_KNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_NNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_KNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append(")")

    '            'まとまるまでコメント　TENMAST　は未使用
    '            'iRet = MainDB.ExecuteNonQuery(SQL)

    '            'If iRet <= 0 Then
    '            '    JobMessage = "金融機関マスタINSERT失敗"
    '            '    MainLOG.Write("金融機関マスタINSERT", "失敗", "")
    '            '    Return False
    '            'End If

    '            '***************************************
    '            '金融機関情報マスタ
    '            '***************************************

    '            ''同じ金融機関情報が存在する場合は登録しない(二重登録防止)
    '            If CheckSameData(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"), _
    '                             oraReader.GetString("SIT_NO_N"), oraReader.GetString("SIT_FUKA_N")) = True Then

    '            Else
    '                '暫定どれくらいあるもんか数えてみる
    '                MainLOG.Write("金融機関マスタ登録", "失敗", "重複 金庫" _
    '                              & oraReader.GetString("KIN_NO_N") & "-" & oraReader.GetString("KIN_FUKA_N") _
    '                             & "支店" & oraReader.GetString("SIT_NO_N") & "-" & oraReader.GetString("SIT_FUKA_N") & " 金庫削除日:" & oraReader.GetString("KIN_DEL_DATE_N") & " 支店削除日:" & oraReader.GetString("SIT_DEL_DATE_N"))

    '            End If

    '            KinOldKey = String.Concat(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"))

    '            If KinOldKey <> KinNewKey Then

    '                SQL = New StringBuilder(128)

    '                SQL.Append("INSERT INTO KIN_INFOMAST(")
    '                SQL.Append(" KIN_NO_N")
    '                SQL.Append(",KIN_FUKA_N")
    '                SQL.Append(",DAIHYO_NO_N")
    '                SQL.Append(",KIN_KNAME_N")
    '                SQL.Append(",KIN_NNAME_N")
    '                SQL.Append(",RYAKU_KIN_KNAME_N")
    '                SQL.Append(",RYAKU_KIN_NNAME_N")
    '                SQL.Append(",JC1_N")
    '                SQL.Append(",JC2_N")
    '                SQL.Append(",JC3_N")
    '                SQL.Append(",JC4_N")
    '                SQL.Append(",JC5_N")
    '                SQL.Append(",JC6_N")
    '                SQL.Append(",JC7_N")
    '                SQL.Append(",JC8_N")
    '                SQL.Append(",JC9_N")
    '                SQL.Append(",KIN_IDO_DATE_N")
    '                SQL.Append(",KIN_IDO_CODE_N")
    '                SQL.Append(",NEW_KIN_NO_N")
    '                SQL.Append(",NEW_KIN_FUKA_N")
    '                SQL.Append(",KIN_DEL_DATE_N")
    '                SQL.Append(",TEIKEI_KBN_N")
    '                SQL.Append(",SYUBETU_N")
    '                SQL.Append(",TIKU_CODE_N")
    '                SQL.Append(",TESUU_TANKA_N")
    '                SQL.Append(",SAKUSEI_DATE_N")
    '                SQL.Append(",KOUSIN_DATE_N")
    '                SQL.Append(")")
    '                SQL.Append(" VALUES(")
    '                SQL.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("DAIHYO_NO_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_KNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_NNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("RYAKU_KIN_KNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("RYAKU_KIN_NNAME_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC1_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC2_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC3_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC4_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC5_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC6_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC7_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC8_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("JC9_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_IDO_DATE_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_IDO_CODE_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_NO_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("NEW_KIN_FUKA_N")))
    '                SQL.Append("," & SQ(oraReader.GetString("KIN_DEL_DATE_N")))
    '                SQL.Append("," & SQ(GetTeikeiKbn(oraReader.GetString("KIN_NO_N"))))
    '                SQL.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(0)))
    '                SQL.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(1)))
    '                SQL.Append("," & SQ(CreateSitenPlus(oraReader.GetString("KIN_NO_N"), "0")(2)))
    '                SQL.Append("," & SQ(mParam.SyoriDate))
    '                SQL.Append("," & SQ(mParam.SyoriDate))
    '                SQL.Append(")")

    '                iRet = MainDB.ExecuteNonQuery(SQL)

    '                If iRet <= 0 Then
    '                    JobMessage = "金融機関情報マスタINSERT失敗"
    '                    MainLOG.Write("金融機関情報マスタINSERT", "失敗", "")
    '                    Return False
    '                End If

    '            End If

    '            KinNewKey = String.Concat(oraReader.GetString("KIN_NO_N"), oraReader.GetString("KIN_FUKA_N"))

    '            '***************************************
    '            '支店情報マスタ
    '            '***************************************
    '            SQL = New StringBuilder(128)
    '            SQL.Append("INSERT INTO SITEN_INFOMAST(")
    '            SQL.Append(" KIN_NO_N")
    '            SQL.Append(",KIN_FUKA_N")
    '            SQL.Append(",SIT_NO_N")
    '            SQL.Append(",SIT_FUKA_N")
    '            SQL.Append(",SIT_KNAME_N")
    '            SQL.Append(",SIT_NNAME_N")
    '            SQL.Append(",SEIDOKU_HYOUJI_N")
    '            SQL.Append(",YUUBIN_N")
    '            SQL.Append(",KJYU_N")
    '            SQL.Append(",NJYU_N")
    '            SQL.Append(",TKOUKAN_NO_N")
    '            SQL.Append(",DENWA_N")
    '            SQL.Append(",TENPO_ZOKUSEI_N")
    '            SQL.Append(",JIKOU_HYOUJI_N")
    '            SQL.Append(",FURI_HYOUJI_N")
    '            SQL.Append(",SYUUTE_HYOUJI_N")
    '            SQL.Append(",KAWASE_HYOUJI_N")
    '            SQL.Append(",DAITE_HYOUJI_N")
    '            SQL.Append(",JISIN_HYOUJI_N")
    '            SQL.Append(",JC_CODE_N")
    '            SQL.Append(",SIT_IDOU_DATE_N")
    '            SQL.Append(",SIT_IDO_CODE_N")
    '            SQL.Append(",NEW_KIN_NO_N")
    '            SQL.Append(",NEW_KIN_FUKA_N")
    '            SQL.Append(",NEW_SIT_NO_N")
    '            SQL.Append(",NEW_SIT_FUKA_N")
    '            SQL.Append(",SIT_DEL_DATE_N")
    '            SQL.Append(",TKOUKAN_NNAME_N")
    '            SQL.Append(",SAKUSEI_DATE_N")
    '            SQL.Append(",KOUSIN_DATE_N")
    '            SQL.Append(")")
    '            SQL.Append(" VALUES(")
    '            SQL.Append(" " & SQ(oraReader.GetString("KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_KNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_NNAME_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SEIDOKU_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("YUUBIN_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KJYU_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NJYU_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("TKOUKAN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("DENWA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("TENPO_ZOKUSEI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("JIKOU_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("FURI_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SYUUTE_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("KAWASE_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("DAITE_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("JISIN_HYOUJI_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("JC_CODE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_IDOU_DATE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_IDO_CODE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SITEN_NEW_KIN_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SITEN_NEW_KIN_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_NO_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("NEW_SIT_FUKA_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("SIT_DEL_DATE_N")))
    '            SQL.Append("," & SQ(oraReader.GetString("TKOUKAN_NNAME_N")))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append("," & SQ(mParam.SyoriDate))
    '            SQL.Append(")")

    '            iRet = MainDB.ExecuteNonQuery(SQL)

    '            If iRet <= 0 Then
    '                JobMessage = "支店情報マスタINSERT失敗"
    '                MainLOG.Write("支店情報マスタINSERT", "失敗", "")
    '                Return False
    '            End If

    '            oraReader.NextRead()

    '            RecCnt += 1

    '            If RecCnt Mod 1000 = 0 Then Console.Write("#")

    '        Loop
    '        Return True

    '    Catch ex As Exception
    '        Throw
    '    Finally

    '    End Try
    'End Function

    ' 同じデータがないかをチェックする
    Private Function CheckSameData(ByVal Kin As String, ByVal kin_fuka As String, ByVal sit As String, ByVal sit_fuka As String) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New MyOracleReader(MainDB)

        Try

            SQL.Append("SELECT COUNT(*) AS COUNTER FROM KIN_INFOMAST,SITEN_INFOMAST")
            SQL.Append(" WHERE ")
            SQL.Append("  KIN_INFOMAST.KIN_NO_N = SITEN_INFOMAST.KIN_NO_N")
            SQL.Append(" AND KIN_INFOMAST.KIN_FUKA_N  = SITEN_INFOMAST.KIN_FUKA_N")
            SQL.Append(" AND KIN_INFOMAST.KIN_NO_N = '" & Kin & "'")
            SQL.Append(" AND KIN_INFOMAST.KIN_FUKA_N  = '" & kin_fuka & "'")
            SQL.Append(" AND SITEN_INFOMAST.SIT_NO_N = '" & sit & "'")
            SQL.Append(" AND SITEN_INFOMAST.SIT_FUKA_N  = '" & sit_fuka & "'")

            If OraReader.DataReader(SQL) = True Then
                If OraReader.GetInt("COUNTER") = 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    ' 他システム用コード変換
    Private Function ConvertData(ByVal inFile As String, ByVal outFile As String) As Boolean
        '変換用定義ファイル
        Dim pFile As String = ""
        Dim CmdLine As String = ""

        Try

            Select Case mParam.SyoriKbn
                Case "0" '一括
                    pFile = Path.Combine(ini_info.FTR, "SSC金融機関マスタJIS.p")

                    CmdLine = "/nwd/ cload "
                    CmdLine &= ini_info.FTR & "FUSION ; ank ebcdic ; kanji JIS getrand "
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'CmdLine &= inFile & " " & outFile & " ++" & pFile
                    CmdLine &= """" & inFile & """" & " " & """" & outFile & """" & " ++" & """" & pFile & """"
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Case "1" '差分

                    pFile = Path.Combine(ini_info.FTR, "SSC金融機関マスタJIS(差分更新).p") '2010/05/19

                    CmdLine = "/nwd/ cload "
                    CmdLine &= ini_info.FTR & "FUSION ; getrand "
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'CmdLine &= inFile & " " & outFile & " ++" & pFile
                    CmdLine &= """" & inFile & """" & " " & """" & outFile & """" & " ++" & """" & pFile & """"
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            End Select

            '------------------------------------------------------------------
            '銀行ファイル・コード変換
            '------------------------------------------------------------------

            Dim ExecProc As Process = Process.Start(Path.Combine(ini_info.FTRANP, "FP.exe"), CmdLine)
            If ExecProc.Id <> 0 Then
                '終了待機
                ExecProc.WaitForExit()
            Else
                JobMessage = "FTRANアプリケーションの起動に失敗しました。"
                Return False
            End If

            If ExecProc.ExitCode <> 0 Then
                JobMessage = "コード変換に失敗しました。"
                Return False
            End If

        Catch ex As Exception
            JobMessage = ex.Message
            Return False
        End Try

        Return True
    End Function

    Private Function GetTeikeiKbn(ByVal kin As String) As String

        Dim sql As New StringBuilder(64)
        Dim oraReader As New MyOracleReader(MainDB)


        Try

            sql.Append("SELECT TEIKEI_KBN_N FROM SSS_TKBNMAST")
            sql.Append(" WHERE ")
            sql.Append(" KIN_NO_N = '" & kin & "'")

            If oraReader.DataReader(sql) = True Then

                If oraReader.GetString("TEIKEI_KBN_N") = "" Then
                    Return "0"  '暫定
                End If

                Return oraReader.GetString("TEIKEI_KBN_N")

            Else
                Return "0"  '暫定
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

        '    '------------------------------------------------------------------
        '    '提携区分更新(2008.03.13 暫定コーディング By Astar)
        '    '------------------------------------------------------------------
        '    RecCnt = 0
        '    For i As Integer = 0 To 5 Step 1
        '        SQL.Length = 0
        '        SQL.Append("UPDATE TENMAST T1")
        '        SQL.Append(" SET TEIKEI_KBN_N = '1'")

        '        Select Case i
        '            Case 0
        '                'MSG = "銀行関係"
        '                SQL.Append(" WHERE KIN_NO_N IN")
        '                SQL.Append(" ('0001', '0005', '0009', '0010', '0016', '0017', '0149'")
        '                SQL.Append(", '0150', '0151', '0152', '0153', '0154', '0155', '0538'")
        '                SQL.Append(", '0541', '0542', '0543', '0544', '0546', '9900')") 'ゆうちょ銀行も追加
        '            Case 1
        '                'MSG = "信用金庫(京都CB,高知CB,以外)"
        '                SQL.Append(" WHERE KIN_NO_N BETWEEN '1000' AND '1999'")
        '                SQL.Append(" AND NOT KIN_NO_N IN ('1610', '1881')")
        '            Case 2
        '                'MSG = "農協(岐阜)"
        '                SQL.Append(" WHERE KIN_NO_N = '3020'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6129' AND '6313'")
        '            Case 3
        '                'MSG = "農協(静岡)"
        '                SQL.Append(" WHERE KIN_NO_N = '3021'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6328' AND '6426'")
        '            Case 4
        '                'MSG = "農協(愛知)"
        '                SQL.Append(" WHERE KIN_NO_N = '3022'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6430' AND '6618'")
        '            Case 5
        '                'MSG = "農協(三重)"
        '                SQL.Append(" WHERE KIN_NO_N = '3023'")
        '                SQL.Append(" OR KIN_NO_N BETWEEN '6631' AND '6770'")
        '        End Select

        '        '更新のあった金融機関のみ対象
        '        SQL.Append(" AND EXISTS (SELECT KIN_NO_N FROM TENMAST T2")
        '        SQL.Append(" WHERE T1.KIN_NO_N = T2.KIN_NO_N AND T2.KOUSIN_DATE_N = '" & SYORIDATE & "')")

        '        RecCnt += MainDB.ExecuteNonQuery(SQL)
        '    Next i

        '    LOG.Write("金融機関マスタ更新 提携区分更新", "成功", "更新件数：" & RecCnt)
    End Function




    Private Function CreateSitenPlus(ByVal kin As String, ByVal teikeikbn As String) As String()

        Dim intSYUBETU_CODE As Integer
        Dim intTESUURYOU As Integer
        Dim intTIKU_CODE As Integer
        Dim Str() As String = New String() {"", "", ""}
        'Dim YOBI1 As Integer '予備１　税区分(0:外税、1:内税)

        '提携内銀行==================
        '0001:みずほ銀行
        '0005:三菱東京ＵＦＪ銀行
        '0009:三井住友銀行
        '0010:りそな銀行
        '0016:みずほコーポレート銀行                  
        '0017:埼玉りそな銀行
        '0149:静岡銀行
        '0150:スルガ銀行
        '0151:清水銀行
        '0152:大垣共立銀行
        '0153:十六銀行
        '0154:三重銀行
        '0155:百五銀行
        '0538:静岡中央銀行
        '0541:岐阜銀行
        '0542:愛知銀行
        '0543:名古屋銀行
        '0544:中京銀行
        '0546:第三銀行
        '============================
        '提携内銀行は                                       種別コード0                 手数料は50
        '提携内信金は                                       種別コード1                 手数料は30
        '東海地区農協は                                     種別コード2                 手数料は50
        'ゆうちょ銀行は                                     種別コード3                 手数料は25(内税)
        'その他提携外金融機関は(京都信金、高知信金含む)     種別コード4                 手数料は85

        Dim intKinCode As Integer = Integer.Parse(kin)

        Select Case intKinCode
            Case Is < 1000 '銀行
                Select Case teikeikbn
                    Case "0" : intSYUBETU_CODE = 4
                    Case "1" : intSYUBETU_CODE = 0
                    Case Else : intSYUBETU_CODE = 4
                End Select

            Case Is < 2000 '信金
                Select Case teikeikbn
                    Case "0" : intSYUBETU_CODE = 4
                    Case "1" : intSYUBETU_CODE = 1
                    Case Else : intSYUBETU_CODE = 4
                End Select

            Case 9900 'ゆうちょ
                intSYUBETU_CODE = 3

            Case Else
                '農協
                If (intKinCode >= 3000 AndAlso intKinCode < 4000) OrElse _
                   (intKinCode >= 6000 AndAlso intKinCode < 9450) Then
                    Select Case teikeikbn
                        Case "0" : intSYUBETU_CODE = 4
                        Case "1" : intSYUBETU_CODE = 2
                        Case Else : intSYUBETU_CODE = 4
                    End Select

                Else 'それ以外
                    intSYUBETU_CODE = 4
                End If
        End Select

        '地区コード、手数料、税区分算出
        Select Case intSYUBETU_CODE
            Case 0 '提携内銀行
                intTIKU_CODE = 0
                intTESUURYOU = 50
                'YOBI1 = 0

            Case 1 '提携内信金
                intTIKU_CODE = GetTikuCode(intKinCode)
                intTESUURYOU = 30
                'YOBI1 = 0

            Case 2 '東海地区農協
                intTIKU_CODE = GetTikuCode(intKinCode)
                intTESUURYOU = 50
                'YOBI1 = 0

            Case 3 'ゆうちょ
                intTIKU_CODE = 0
                intTESUURYOU = 25
                'YOBI1 = 1

            Case 4 '提携外金融機関
                intTIKU_CODE = 0
                intTESUURYOU = 85
                'YOBI1 = 0
        End Select

        'Dim sb As StringBuilder = New StringBuilder
        Str(0) = intSYUBETU_CODE.ToString
        Str(1) = intTIKU_CODE.ToString
        Str(2) = intTESUURYOU.ToString

        Return Str
    End Function

    Private Function GetTikuCode(ByVal intKinCode As Integer) As Integer
        '愛知
        If intKinCode > 1549 And intKinCode < 1567 Or _
           intKinCode = 3022 Or _
           intKinCode > 6429 And intKinCode < 6619 Then
            Return 1
        End If

        '岐阜
        If intKinCode > 1529 And intKinCode < 1541 Or _
           intKinCode = 3020 Or _
           intKinCode > 6128 And intKinCode < 6314 Then
            Return 2
        End If

        '三重
        If intKinCode > 1579 And intKinCode < 1586 Or _
           intKinCode = 3023 Or _
           intKinCode > 6630 And intKinCode < 6771 Then
            Return 3
        End If

        '静岡
        If intKinCode > 1500 And intKinCode < 1518 Or _
           intKinCode = 3021 Or _
           intKinCode > 6327 And intKinCode < 6427 Then
            Return 4
        End If

        'その他
        Return 0
    End Function

    Private Function DeleteMast() As Boolean

        Dim sql As StringBuilder
        Dim iRet As Integer

        Try

            sql = New StringBuilder(128)
            sql.Append("DELETE FROM KIN_INFOMAST")
            'NULL、空白、0以外 で処理日より過去の削除日付を対象とする
            sql.Append(" WHERE KIN_DEL_DATE_N IS NOT NULL")
            sql.Append(" AND KIN_DEL_DATE_N NOT IN('        ', '00000000')")
            sql.Append(" AND KIN_DEL_DATE_N < '" & mParam.SyoriDate & "'")

            iRet = MainDB.ExecuteNonQuery(sql)

            If iRet < 0 Then
                JobMessage = "金融機関情報マスタ削除日到来レコード削除失敗"
                MainLOG.Write("金融機関情報マスタ削除", "失敗", JobMessage)

                Return False
            End If

            sql = New StringBuilder(128)
            sql.Append("DELETE FROM SITEN_INFOMAST")
            'NULL、空白、0以外 で処理日より過去の削除日付を対象とする
            sql.Append(" WHERE SIT_DEL_DATE_N IS NOT NULL")
            sql.Append(" AND SIT_DEL_DATE_N NOT IN('        ', '00000000')")
            sql.Append(" AND SIT_DEL_DATE_N < '" & mParam.SyoriDate & "'")

            iRet = MainDB.ExecuteNonQuery(sql)

            If iRet < 0 Then
                JobMessage = "支店情報マスタ削除日到来レコード削除失敗"
                MainLOG.Write("支店情報マスタ削除", "失敗", JobMessage)
                Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        End Try

    End Function



    'Private Class CreateTENMAST_S
    '    '----------------------------------------------------------
    '    'ＳＳＳ用金融機関マスタのＣＳＶを作成するクラス
    '    '2008/06/13 mitsu 色々修正
    '    '2008/06/24 mitsu 金融機関マスタ一括更新内に組み込み
    '    '----------------------------------------------------------
    '    Private CurDir As String
    '    Private CsvFile As String
    '    Private SysDate As String = DateTime.Today.ToString("yyyyMMdd")
    '    Private ENCD As Encoding = Encoding.GetEncoding(932)

    '    Sub New(ByVal LoadDataFolder As String, ByRef db As MyOracle)
    '        CurDir = LoadDataFolder
    '        CsvFile = CurDir & "SSS.CSV"

    '        Try
    '            'ＣＳＶファイルにShift JISで書き込む
    '            Dim sw As StreamWriter = New StreamWriter(CsvFile, False, ENCD)

    '            Dim strSQL As String = "SELECT DISTINCT KIN_NO_N, KIN_KNAME_N, KIN_NNAME_N,TEIKEI_KBN_N"
    '            strSQL &= " FROM TENMAST ORDER BY KIN_NO_N"

    '            Dim OraReader As MyOracleReader = New MyOracleReader(db)

    '            If OraReader.DataReader(strSQL) Then
    '                Do
    '                    sw.WriteLine(CreateCsvLine( _
    '                        OraReader.GetString("KIN_NO_N"), _
    '                        OraReader.GetString("KIN_KNAME_N"), _
    '                        OraReader.GetString("KIN_NNAME_N"), _
    '                        OraReader.GetString("TEIKEI_KBN_N") _
    '                    ))
    '                Loop While OraReader.NextRead = True
    '            End If

    '            OraReader.Close()
    '            sw.Close()

    '            'CSV作成成功時はTENMAST_Sの中身をクリア
    '            strSQL = "DELETE FROM TENMAST_S"
    '            Dim workDB As MyOracle = New MyOracle
    '            workDB.ExecuteNonQuery(strSQL)
    '            workDB.Commit()
    '            workDB.Close()

    '            'CTLファイル作成
    '            CreateCtlFile()

    '            'ローダー制御ファイルの実行コマンドを組み立てる
    '            Dim CmdLine As String = "KZAMAST/KZAMAST@FSKJ_LSNR"
    '            CmdLine &= " CONTROL = '" & CurDir & "SSS.CTL'"
    '            CmdLine &= " LOG = '" & CurDir & "SSS.LOG'"

    '            Dim PSI As New ProcessStartInfo
    '            With PSI
    '                .FileName = "SQLLDR"
    '                .CreateNoWindow = True
    '                .Arguments = CmdLine
    '            End With

    '            Dim Pro As Process = Process.Start(PSI)
    '            Pro.WaitForExit()

    '        Catch ex As Exception
    '            Try
    '                Dim sw As StreamWriter = New StreamWriter(CurDir & "err.log", False, Encoding.GetEncoding(932))
    '                sw.WriteLine(ex.Message)
    '                sw.Close()

    '            Catch ex2 As Exception
    '            End Try
    '        End Try
    '    End Sub




    '    Private Function CreateCtlFile() As Boolean
    '        Dim sw As StreamWriter = New StreamWriter(CurDir & "SSS.CTL", False, ENCD)
    '        sw.AutoFlush = False

    '        sw.WriteLine("LOAD DATA")
    '        sw.WriteLine("INFILE '" & CsvFile & "' ")
    '        sw.WriteLine("PRESERVE BLANKS")
    '        sw.WriteLine("INTO TABLE KZAMAST.TENMAST_S")
    '        sw.WriteLine("FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '""'")
    '        sw.WriteLine("(KIN_NO_N,")
    '        sw.WriteLine("KIN_KNAME_N,")
    '        sw.WriteLine("KIN_NNAME_N,")
    '        sw.WriteLine("SYUBETU_N,")
    '        sw.WriteLine("TIKU_CODE_N,")
    '        sw.WriteLine("TESUU_TANKA_N,")
    '        sw.WriteLine("SAKUSEI_DATE_N,")
    '        sw.WriteLine("KOUSIN_DATE_N,")
    '        sw.WriteLine("YOBI1_N,")
    '        sw.WriteLine("YOBI2_N,")
    '        sw.WriteLine("YOBI3_N,")
    '        sw.WriteLine("YOBI4_N,")
    '        sw.WriteLine("YOBI5_N")
    '        sw.WriteLine(")")
    '        sw.Close()

    '        Return True
    '    End Function
    'End Class

End Class

