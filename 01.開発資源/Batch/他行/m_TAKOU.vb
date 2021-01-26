Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CAstReports.ClsReportBase
Imports System.Data
Imports Microsoft.VisualBasic
Imports System.Windows.Forms
Imports CASTCommon

Module m_TAKOU

#Region "モジュール変数"

    Private clsFusion As New clsFUSION.clsMain

    'パラメータ用
    Public strTORI_CODE As String
    Public strTORIS_CODE As String
    Public strTORIF_CODE As String
    Public strFURI_DATE As String
    Public strTUUBAN As String

    ' ＯｒａｃｌｅＤＢ
    Private MainDB As CASTCommon.MyOracle

    'ヘッダーレコード用
    Public strHEDDA_FURI_DATA As String

    'ワークファイル名
    Public strWRK_FILE_NAME As String
    'ファイルのレコード長
    Public intREC_LENGTH As Integer
    '件数・金額計算用
    Private dblGOUKEI_KEN As Decimal
    Private dblGOUKEI_KIN As Decimal
    'データ作成が金融機関単位でループされるため、フラグで管理する
    Dim blnMakeMATOME_DATA As Boolean
    '****************************************************

    Dim strIBMP_FILE As String

    Public strParaFURI_DATE As String
    Public strParaTORI_CODE As String
    Public nJIFURI_SEQ As Long

    Private BLOG As New CASTCommon.BatchLOG("KFJ020", "他行データ作成")
    Private Const msgTitle As String = "他行データ作成(KFJ020)"

    '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
    Public vbDLL As New CMTCTRL.ClsFSKJ
    'Public vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

    Private gstrP_FILE As String

    Private JobMessage As String = ""

    Private GCOM As New MenteCommon.clsCommon

    Private Structure strcTakoumastInfo
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim TKIN_NO As String
        Dim TSIT_NO As String
        Dim ITAKU_CODE As String
        Dim KAMOKU As String
        Dim KOUZA As String
        Dim BAITAI_CODE As String
        Dim CODE_KBN As String
        Dim SFILE_NAME As String
        Dim RFILE_NAME As String

        Public Sub Init()
            TORIS_CODE = String.Empty
            TORIF_CODE = String.Empty
            ITAKU_CODE = String.Empty
            TKIN_NO = String.Empty
            TSIT_NO = String.Empty
            KAMOKU = String.Empty
            KOUZA = String.Empty
            BAITAI_CODE = String.Empty
            CODE_KBN = String.Empty
            SFILE_NAME = String.Empty
            RFILE_NAME = String.Empty
        End Sub

        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_V")
            TORIF_CODE = oraReader.GetString("TORIF_CODE_V")
            ITAKU_CODE = oraReader.GetString("ITAKU_CODE_V")
            TKIN_NO = oraReader.GetString("TKIN_NO_V")
            TSIT_NO = oraReader.GetString("TSIT_NO_V")
            KAMOKU = oraReader.GetString("KAMOKU_V")
            KOUZA = oraReader.GetString("KOUZA_V")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_V")
            CODE_KBN = oraReader.GetString("CODE_KBN_V")
            SFILE_NAME = oraReader.GetString("SFILE_NAME_V")
            RFILE_NAME = oraReader.GetString("RFILE_NAME_V")
        End Sub
    End Structure
    Private TakouInfo As strcTakoumastInfo

    Private Structure strcTorimastInfo
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim ITAKU_KNAME As String
        Dim ITAKU_NNAME As String
        Dim KIGYO_CODE As String
        Dim FURI_CODE As String
        Dim BAITAI_CODE As String
        Dim FMT_KBN As String
        Dim NS_KBN As String
        Dim LABEL_KBN As String
        Dim SYOUHI_KBN As String

        Public Sub Init()
            TORIS_CODE = String.Empty
            TORIF_CODE = String.Empty
            ITAKU_KNAME = String.Empty
            ITAKU_NNAME = String.Empty
            KIGYO_CODE = String.Empty
            FURI_CODE = String.Empty
            BAITAI_CODE = String.Empty
            FMT_KBN = String.Empty
            NS_KBN = String.Empty
            LABEL_KBN = String.Empty
            SYOUHI_KBN = String.Empty
        End Sub

        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_T")
            TORIF_CODE = oraReader.GetString("TORIF_CODE_T")
            ITAKU_KNAME = oraReader.GetString("ITAKU_KNAME_T")
            ITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
            KIGYO_CODE = oraReader.GetString("KIGYO_CODE_T")
            FURI_CODE = oraReader.GetString("FURI_CODE_T")
            BAITAI_CODE = oraReader.GetString("BAITAI_CODE_T")
            FMT_KBN = oraReader.GetString("FMT_KBN_T")
            NS_KBN = oraReader.GetString("NS_KBN_T")
            LABEL_KBN = oraReader.GetString("LABEL_KBN_T")
            SYOUHI_KBN = oraReader.GetString("SYOUHI_KBN_T")
        End Sub
    End Structure
    Private ToriInfo As strcTorimastInfo

    Private Structure strcIniInfo
        Dim DEN As String
        Dim TAK As String
        Dim KINKOCD As String
        Dim MT As String
        Dim CMT As String
        Dim TAKO_SEQ As String
        Dim NOUKYOMATOME As String
        Dim NOUKYOFROM As String
        Dim NOUKYOTO As String
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
        Dim RSV2_EDITION As String        ' RSV2機能設定
        Dim COMMON_BAITAIWRITE As String  ' 媒体書込用フォルダ
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
    End Structure
    Private IniInfo As strcIniInfo

    Private strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

#End Region

#Region "パブリックメソッド"

    Public Sub Main()
        '============================================================================
        'NAME           :Main
        'Parameter      :
        'Description    :他行データ作成処理主処理
        'Return         :
        'Create         :2004/08/19
        'Update         :
        '============================================================================

        '-----------------------------------------
        'パラメタチェック
        '-----------------------------------------
        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = BLOG.Write_Enter1("", "0000000000-00", "00000000", "他行データ作成処理主処理(開始)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            Dim strCommand As String = Microsoft.VisualBasic.Command()
            '2017/01/16 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            Dim SSSFlg As Boolean = False
            If strCommand.Split(","c).Length <> 3 AndAlso strCommand.Split(","c).Length <> 4 Then
                'If strCommand.Split(","c).Length <> 3 Then
                '2017/01/16 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                BLOG.Write("パラメタ取得", "失敗", "引数：" & strCommand)

                Exit Sub
            Else
                Dim arr() As String = strCommand.Split(","c)
                '2017/01/16 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                If arr.Length = 3 Then
                    strTORI_CODE = arr(0)                           '取引先コード
                    strTORIS_CODE = strTORI_CODE.Substring(0, 10)   '取引先主コード
                    strTORIF_CODE = strTORI_CODE.Substring(10, 2)   '取引先副コード
                    strFURI_DATE = arr(1)                           '振替日
                    strTUUBAN = arr(2)                              '通番
                ElseIf arr.Length = 4 Then
                    strTORI_CODE = arr(0)                           '取引先コード
                    strTORIS_CODE = strTORI_CODE.Substring(0, 10)   '取引先主コード
                    strTORIF_CODE = strTORI_CODE.Substring(10, 2)   '取引先副コード
                    strFURI_DATE = arr(1)                           '振替日
                    strTUUBAN = arr(3)                              '通番
                    SSSFlg = True
                End If
                'strTORI_CODE = arr(0)                           '取引先コード
                'strTORIS_CODE = strTORI_CODE.Substring(0, 10)   '取引先主コード
                'strTORIF_CODE = strTORI_CODE.Substring(10, 2)   '取引先副コード
                'strFURI_DATE = arr(1)                           '振替日
                'strTUUBAN = arr(2)                              '通番
                '2017/01/16 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END

                strParaTORI_CODE = strTORI_CODE
                strParaFURI_DATE = strFURI_DATE

                BLOG.Write("パラメタ取得", "成功")

            End If

            BLOG.ToriCode = strTORI_CODE
            BLOG.FuriDate = strFURI_DATE
            BLOG.JobTuuban = CASTCommon.CAInt32(strTUUBAN.Trim)

            '------------------------------------------
            'INIファイルの読み込み
            '------------------------------------------
            If fn_INI_READ() = False Then
                BLOG.UpdateJOBMASTbyErr("ディレクトリ取得失敗")
                Return
            End If

            '-----------------------------------------
            '他行データ作成メイン処理
            '-----------------------------------------
            '2017/01/16 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            If SSSFlg = False Then
                If fn_CREATE_DATA_MAIN() = False Then
                    BLOG.UpdateJOBMASTbyErr(JobMessage)
                    BLOG.Write("他行データ作成 異常終了", "失敗")

                Else
                    BLOG.UpdateJOBMASTbyOK(JobMessage)
                    BLOG.Write("他行データ作成 正常終了", "成功")

                End If
            Else
                Dim SSS As New ClsSSS
                SSS.FURI_DATE = strParaFURI_DATE
                If SSS.fn_CREATE_DATA_MAIN() = False Then
                    BLOG.Write("SSS他行データ作成 異常終了", "失敗")
                    '配信待ちフラグの戻し
                    SSS.ReturnFlg()
                Else
                    BLOG.Write("SSS他行データ作成 正常終了", "成功")
                    '配信待ちフラグの戻し
                    SSS.ReturnFlg()
                    BLOG.UpdateJOBMASTbyOK("")
                End If
            End If

            'If fn_CREATE_DATA_MAIN() = False Then
            '    BLOG.UpdateJOBMASTbyErr(JobMessage)
            '    BLOG.Write("他行データ作成 異常終了", "失敗")

            'Else
            '    BLOG.UpdateJOBMASTbyOK(JobMessage)
            '    BLOG.Write("他行データ作成 正常終了", "成功")

            'End If
            '2017/01/16 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END

            If Not MainDB Is Nothing Then
                MainDB.Close()
            End If

        Catch ex As Exception
            BLOG.Write("0000000000-00", "00000000", "他行データ作成処理主処理", "失敗", ex.Message)
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BLOG.Write_Exit1(sw, "", "0000000000-00", "00000000", "他行データ作成処理主処理(終了)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 他行データ作成メイン処理
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_CREATE_DATA_MAIN() As Boolean

        Dim TakouMeisaiList As New ArrayList(128)       '他行明細表印刷用
        Dim FurikaeDataInvoice As New ArrayList(128)    '口座振替請求データ送付票印刷用

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
        Dim oraSchReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Try
            BLOG.Write("(他行データ作成)開始", "成功")


            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                BLOG.Write_Err("他行データ作成処理", "失敗", "他行データ作成処理で実行待ちタイムアウト")
                JobMessage = "他行データ作成処理で実行待ちタイムアウト"
                Return False
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            '----------------------------------------
            'スケジュール検索
            '----------------------------------------
            If oraSchReader.DataReader(CreateGetSchmastSQL) = True Then

                '対象スケジュールが存在したら既存他行スケジュールの削除
                If DeleteTakoschmast() = False Then
                    BLOG.Write("他行データ作成", "失敗", "他行スケジュールマスタ削除失敗")
                    JobMessage = "他行スケジュールマスタ削除失敗　振替日：" & strFURI_DATE
                    Return False
                End If

                '自振シーケンスの取得
                nJIFURI_SEQ = GetMaxJifuriSEQPlusOne()

                '明細検索リーダー
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraSchReader.EOF = False

                    Dim strKEIYAKUKIN_OLD As String = ""

                    '----------------------------------------
                    'ログ用の取引先コード設定
                    '----------------------------------------
                    BLOG.ToriCode = oraSchReader.GetString("TORIS_CODE_S") & oraSchReader.GetString("TORIF_CODE_S")
                    BLOG.FuriDate = oraSchReader.GetString("FURI_DATE_S")

                    '----------------------------------------
                    'データ抽出(ヘッダー部)
                    '----------------------------------------
                    If oraMeiReader.DataReader(CreateGetMeimastHeaderRecord(oraSchReader)) = True Then
                        strHEDDA_FURI_DATA = oraMeiReader.GetItem("FURI_DATA_K")
                    Else
                        BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行データ作成", "失敗", "他行データヘッダレコード検索失敗")
                        JobMessage = "他行データヘッダレコード検索失敗"
                        Return False
                    End If

                    oraMeiReader.Close()

                    '----------------------------------------
                    'データ抽出(データ部)
                    '----------------------------------------
                    Dim intRECORD_COUNT As Integer = 0
                    If oraMeiReader.DataReader(CreateGetMeimastKeiyakuKin(oraSchReader)) = True Then
                        While oraMeiReader.EOF = False
                            intRECORD_COUNT += 1

                            '-------------------------------------
                            '他行マスタ検索
                            '-------------------------------------
                            TakouInfo.Init()
                            If GetTakoumast(oraSchReader.GetString("TORIS_CODE_S"), oraSchReader.GetString("TORIF_CODE_S"), oraMeiReader.GetString("KEIYAKU_KIN_K")) = False Then
                                '他行マスタ未登録
                                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行データ作成", "失敗", "他行マスタ検索失敗　金融機関コード：" & oraMeiReader.GetString("KEIYAKU_KIN_K"))
                                JobMessage = "他行マスタ検索失敗　取引先コード：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " 金融機関コード：" & oraMeiReader.GetString("KEIYAKU_KIN_K")
                            End If

                            '-------------------------------------
                            '金融機関マスタ検索
                            '-------------------------------------
                            If CheckTenmast(TakouInfo.TKIN_NO, TakouInfo.TSIT_NO) = False Then
                                '金融機関マスタ未登録
                                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行データ作成", "失敗", "金融機関マスタ検索失敗　金融機関コード：" & TakouInfo.TKIN_NO & "-" & TakouInfo.TSIT_NO)
                                JobMessage = "金融機関マスタ検索失敗　金融機関コード：" & TakouInfo.TKIN_NO & "-" & TakouInfo.TSIT_NO
                                Return False
                            End If

                            '--------------------------------------------------------
                            '農協向け対応（金融機関コードが5874～5939はまとめて農協扱い）
                            '--------------------------------------------------------
                            '2013/03/29 saitou ソース改善 DEL -------------------------------------------------->>>>
                            '用途不明なので削除
                            'If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                            '    strKIN_NNAME = "農協"
                            'End If
                            '2013/03/29 saitou ソース改善 DEL --------------------------------------------------<<<<

                            '-------------------------------------
                            '取引先マスタ検索
                            '-------------------------------------
                            ToriInfo.Init()
                            If GetTorimast(oraSchReader.GetString("TORIS_CODE_S"), oraSchReader.GetString("TORIF_CODE_S")) = False Then
                                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行データ作成", "失敗", "取引先コード：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S"))
                                JobMessage = "取引先マスタ検索失敗 取引先コード：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S")
                                Return False
                            End If

                            '-------------------------------------
                            'フォルダの存在確認と作成
                            '-------------------------------------
                            Dim strFOLDER As String
                            If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                                '*** 修正 mitsu 2008/08/11 農協代表コードでフォルダ作成 ***
                                strFOLDER = Path.Combine(IniInfo.TAK, IniInfo.NOUKYOMATOME)
                                '**********************************************************
                            Else
                                strFOLDER = Path.Combine(IniInfo.TAK, TakouInfo.TKIN_NO)
                            End If
                            strWRK_FILE_NAME = Path.Combine(strFOLDER, strFURI_DATE & "_" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & ".dat")
                            'フォルダの存在確認
                            If Directory.Exists(strFOLDER) = False Then
                                'フォルダの作成
                                Directory.CreateDirectory(strFOLDER)
                            End If

                            '-------------------------------------
                            'ファイルの作成
                            '-------------------------------------
                            ' 2016/01/22 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                            Select Case ToriInfo.FMT_KBN
                                Case "04", "05"
                                    ToriInfo.FMT_KBN = "00"
                                Case Else
                                    If CInt(ToriInfo.FMT_KBN.Trim) >= 50 Then
                                        ToriInfo.FMT_KBN = "00"
                                    End If
                            End Select
                            ' 2016/01/22 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                            Select Case ToriInfo.FMT_KBN
                                Case "00"   '全銀フォーマット
                                    Select Case TakouInfo.BAITAI_CODE
                                        Case "04", "09"
                                            If fn_CREATE_FILE_ZENGIN(False) = False Then
                                                Return False
                                            End If
                                        Case Else
                                            If fn_CREATE_FILE_ZENGIN(True) = False Then
                                                Return False
                                            End If
                                    End Select

                                Case "01"   'NHKフォーマット
                                    Select Case TakouInfo.BAITAI_CODE
                                        Case "04", "09"
                                            If fn_CREATE_FILE_NHK(False) = False Then
                                                Return False
                                            End If
                                        Case Else
                                            If fn_CREATE_FILE_NHK(True) = False Then
                                                Return False
                                            End If
                                    End Select

                                Case "02"

                            End Select

                            '-------------------------------------
                            '媒体書込み
                            '-------------------------------------
                            Select Case ToriInfo.FMT_KBN
                                Case "00", "01"
                                    Select Case TakouInfo.CODE_KBN
                                        Case "0" : gstrP_FILE = "120JIS→JIS.P"
                                        Case "1" : gstrP_FILE = "120JIS→JIS改.P"
                                        Case "2" : gstrP_FILE = "119JIS→JIS改.P"
                                        Case "3" : gstrP_FILE = "118JIS→JIS改.P"
                                        Case "4"
                                            gstrP_FILE = "120.P"
                                            strIBMP_FILE = "120IBM.P"
                                    End Select

                                    intREC_LENGTH = 120
                            End Select
                            '2016/02/05 タスク）斎藤 RSV2対応 UPD ---------------------------------------- START
                            '大規模版使用時はメッセージ出力しない
                            Select Case IniInfo.RSV2_EDITION
                                Case "2"
                                    'NOP
                                Case Else
                                    '2010/02/19 どこのMT/CMTを要求されているかわからないので通知
                                    Dim KIN_NAME As String = GCOM.GetBKBRName(TakouInfo.TKIN_NO, "")
                                    If (TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "0") Then
                                        MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "MT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                    ElseIf (TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "0") Then
                                        MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "CMT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                    End If
                                    '===========================================================

                                    If TakouInfo.BAITAI_CODE = "01" Then
                                        MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "FD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                    End If
                            End Select

                            ''2010/02/19 どこのMT/CMTを要求されているかわからないので通知
                            'Dim KIN_NAME As String = GCOM.GetBKBRName(TakouInfo.TKIN_NO, "")
                            'If (TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "0") Then
                            '    MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "MT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                            'ElseIf (TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "0") Then
                            '    MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "CMT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                            'End If
                            ''===========================================================

                            'If TakouInfo.BAITAI_CODE = "01" Then
                            '    MessageBox.Show(String.Format(MSG0072I, TakouInfo.TKIN_NO, KIN_NAME, "FD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                            'End If
                            '2016/02/05 タスク）斎藤 RSV2対応 UPD ---------------------------------------- END

                            If fn_BAITAI_WRITE() = False Then
                                Return False
                            End If

                            '2010/02/19 副書込み追加
                            ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
                            'Select Case TakouInfo.BAITAI_CODE
                            '    Case "01", "05", "06"
                            '        If TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "1" Then
                            '            Exit Select
                            '        ElseIf TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "1" Then
                            '            Exit Select
                            '        End If
                            '        If MessageBox.Show(MSG0061I, _
                            '                           msgTitle, _
                            '                           MessageBoxButtons.YesNo, _
                            '                           MessageBoxIcon.Information) = DialogResult.Yes Then

                            '            If fn_BAITAI_WRITE() = False Then
                            '                Exit Function
                            '            End If
                            '        End If
                            'End Select
                            If TakouInfo.BAITAI_CODE = "05" And IniInfo.MT = "1" Then
                                ' NOP
                            ElseIf TakouInfo.BAITAI_CODE = "06" And IniInfo.CMT = "1" Then
                                ' NOP
                            Else
                                Select Case IniInfo.RSV2_EDITION
                                    Case "2"
                                        ' NOP
                                    Case Else
                                        Select Case TakouInfo.BAITAI_CODE
                                            Case "01"
                                                If MessageBox.Show(MSG0061I, _
                                                                       msgTitle, _
                                                                       MessageBoxButtons.YesNo, _
                                                                       MessageBoxIcon.Information) = DialogResult.Yes Then

                                                    If fn_BAITAI_WRITE() = False Then
                                                        Exit Function
                                                    End If
                                                End If
                                        End Select
                                End Select
                            End If
                            ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
                            '==========================

                            '他行明細表用の配列に格納
                            If TakouInfo.TKIN_NO <> strKEIYAKUKIN_OLD Then
                                strKEIYAKUKIN_OLD = TakouInfo.TKIN_NO

                                Select Case TakouInfo.BAITAI_CODE
                                    Case "04"
                                        Dim KeyCode() As String = New String() {ToriInfo.TORIS_CODE, _
                                                                                ToriInfo.TORIF_CODE, _
                                                                                strFURI_DATE, _
                                                                                TakouInfo.TKIN_NO}
                                        '他行明細表
                                        TakouMeisaiList.Add(KeyCode)
                                End Select

                                Dim KeyCodeDataSoufu() As String = New String() {ToriInfo.TORIS_CODE, _
                                                                                 ToriInfo.TORIF_CODE, _
                                                                                 strFURI_DATE, _
                                                                                 TakouInfo.TKIN_NO}
                                '口座振替請求データ送付票
                                FurikaeDataInvoice.Add(KeyCodeDataSoufu)

                            End If


                            '-------------------------------------
                            '他行スケジュールの作成
                            '-------------------------------------
                            If InsertTakoschmast(oraSchReader) = False Then
                                Return False
                            End If

                            oraMeiReader.NextRead()
                        End While
                    End If

                    oraMeiReader.Close()


                    oraSchReader.NextRead()

                End While
            Else
                BLOG.Write("他行データ作成", "失敗", "作成対象スケジュールなし 振替日：" & strFURI_DATE)
                JobMessage = "他行データ作成対象スケジュールなし　振替日：" & strFURI_DATE
                Return False
            End If

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            'ここまで無傷ならコミット
            MainDB.Commit()
            MainDB = Nothing

            '' 他行宛てが紙の場合の処理
            For intN As Integer = 0 To TakouMeisaiList.Count - 1
                Dim KeyCodeList() As String = CType(TakouMeisaiList(intN), String())

                Dim strTorisCode As String = KeyCodeList(0)
                Dim strTorifCode As String = KeyCodeList(1)
                Dim strFuriDate As String = KeyCodeList(2)
                Dim strKeiyakuKin As String = KeyCodeList(3)

                BLOG.ToriCode = strTorisCode & strTorifCode
                BLOG.FuriDate = strFURI_DATE

                '他行明細表印刷
                BLOG.Write("他行明細表印刷(開始)", "成功", "金融機関コード：" & strKeiyakuKin)
                If PrnTakoumeisaiList(strTorisCode, strTorifCode, strFuriDate, strKeiyakuKin) = False Then
                    BLOG.Write("他行明細表印刷", "失敗", "金融機関コード：" & strKeiyakuKin)

                    JobMessage = "明細表印刷失敗 金融機関コード：" & strKeiyakuKin
                    Return False
                End If
                BLOG.Write("他行明細表印刷(終了)", "成功", "金融機関コード：" & strKeiyakuKin)
            Next

            For intM As Integer = 0 To FurikaeDataInvoice.Count - 1
                Dim KeyCodeList() As String = CType(FurikaeDataInvoice(intM), String())

                Dim strTorisCode As String = KeyCodeList(0)
                Dim strTorifCode As String = KeyCodeList(1)
                Dim strFuriDate As String = KeyCodeList(2)
                Dim strKeiyakuKin As String = KeyCodeList(3)

                BLOG.ToriCode = strTorisCode & strTorifCode
                BLOG.FuriDate = strFuriDate

                '口座振替請求データ送付票
                BLOG.Write("口座振替請求データ送付票(開始)", "成功", "金融機関コード：" & strKeiyakuKin)
                If PrnKouzafurikaeSeikyuDataInvoice(strTorisCode, strTorifCode, strFuriDate, strKeiyakuKin) = False Then
                    BLOG.Write("口座振替請求データ送付票印刷", "失敗", "金融機関コード：" & strKeiyakuKin)
                    JobMessage = "口座振替請求データ送付票印刷失敗 金融機関コード：" & strKeiyakuKin
                    Return False
                End If
                BLOG.Write("口座振替請求データ送付票(終了)", "成功", "金融機関コード：" & strKeiyakuKin)

            Next

            Return True

        Catch ex As Exception
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行データ作成", "失敗", ex.Message)
            JobMessage = "他行データ作成（メイン）失敗 取引先コード：" & BLOG.ToriCode
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

            '最後まで残っていたらロールバック
            If Not MainDB Is Nothing Then
                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If


        End Try

    End Function

    ''' <summary>
    ''' NHKフォーマットの他行データを作成します。(120バイト)
    ''' </summary>
    ''' <param name="createFlag">紙フラグ</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改対応にて改修</remarks>
    Private Function fn_CREATE_FILE_NHK(ByVal createFlag As Boolean) As Boolean
        fn_CREATE_FILE_NHK = False
        Dim intRECORD_COUNT As Integer

        '---------------------------------------------
        '作成対象金融機関のデータを抽出
        '---------------------------------------------
        Dim SQL As StringBuilder
        SQL = New StringBuilder(128)
        SQL.Append("SELECT * FROM MEIMAST")
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then    '農協対応１つのファイルにまとめて作成
            SQL.Append(" WHERE TORIS_CODE_K '" & ToriInfo.TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_K = '" & strFURI_DATE & "'")
            'SQL.Append(" AND KEIYAKU_KIN_K BETWEEN  '5874' AND '5939'")
            SQL.Append(" AND KEIYAKU_KIN_K BETWEEN  " & SQ(IniInfo.NOUKYOFROM) & " AND " & SQ(IniInfo.NOUKYOTO)) 'INIファイルの設定を反映
            SQL.Append(" AND DATA_KBN_K = '2'")

            '*** 修正 mitsu 2008/08/12 まとめデータ作成フラグ ***
            blnMakeMATOME_DATA = True
            '****************************************************
        Else
            SQL.Append(" WHERE TORIS_CODE_K = '" & ToriInfo.TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_K = '" & strFURI_DATE & "'")
            SQL.Append(" AND KEIYAKU_KIN_K = '" & TakouInfo.TKIN_NO & "'")
            SQL.Append(" AND DATA_KBN_K = '2'")
        End If
        '*** 修正 mitsu 2008/08/12 金融機関ソート追加 ***
        'SQL.Append(" ORDER BY KEIYAKU_SIT_K,RECORD_NO_K ASC")
        SQL.Append(" ORDER BY KEIYAKU_KIN_K, KEIYAKU_SIT_K, RECORD_NO_K")
        '************************************************

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
        Dim lngTKIN_KEN As Long = 0
        Dim lngTKIN_KIN As Long = 0
        '********************************************************
        Dim dblYUUKIN_KEN As Long = 0
        Dim dblZEROKIN_KEN As Long = 0

        '  紙はNHKファイルを作らない >>
        If createFlag = False Then
            dblGOUKEI_KEN = 0
            dblGOUKEI_KIN = 0

            '*** 修正 mitsu 2008/08/12 例外処理追加 ***
            Try
                BLOG.Write("明細の検索(開始)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
                '**************************************
                If OraReader.DataReader(SQL) = True Then
                    While (OraReader.EOF = False)
                        dblGOUKEI_KEN += 1
                        dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                        '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
                        If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                            lngTKIN_KEN += 1
                            lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                        End If
                        '********************************************************

                        OraReader.NextRead()
                    End While

                    '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        dblGOUKEI_KEN = lngTKIN_KEN
                        dblGOUKEI_KIN = lngTKIN_KIN
                    End If
                    '********************************************************

                    Return True
                End If

                '*** 修正 mitsu 2008/08/12 例外処理追加 ***
            Catch EX As Exception
                BLOG.Write("明細の検索", "失敗", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE & " " & EX.Message)
               JobMessage = "明細の検索 金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE
                Return False
            Finally
                OraReader.Close()
            End Try
            '**********************************************
            BLOG.Write("明細の検索(終了)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
            End If
        ' 2008.04.09 ADD <<

        intRECORD_COUNT = 1
        dblGOUKEI_KEN = 0
        dblGOUKEI_KIN = 0

        '--------------------------------------------
        'ファイルのオープン
        '--------------------------------------------
        If File.Exists(strWRK_FILE_NAME) = True Then
            File.Delete(strWRK_FILE_NAME)
        End If

        Dim oFusion As New clsFUSION.clsMain

        Dim nhkFormat As New CAstFormat.CFormatNHK
        Dim nhkStreamWriter As New StreamWriter(strWRK_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

        '--------------------------------------------
        'ヘッダーレコードの書込み
        '--------------------------------------------
        nhkFormat.NHK_REC1.NH01 = "1"                                   'データ区分
        Select Case ToriInfo.NS_KBN
            Case "1"
                nhkFormat.NHK_REC1.NH02 = "21"                          '種別コード
            Case "9"
                nhkFormat.NHK_REC1.NH02 = "91"
        End Select
        nhkFormat.NHK_REC1.NH03 = "1"                                   'コード区分
        nhkFormat.NHK_REC1.NH04 = TakouInfo.ITAKU_CODE                   '他行委託者コード
        nhkFormat.NHK_REC1.NH05 = strHEDDA_FURI_DATA.Substring(14, 40)  '委託者名
        nhkFormat.NHK_REC1.NH06 = strFURI_DATE.Substring(4, 4)          '振替日
        nhkFormat.NHK_REC1.NH07 = TakouInfo.TKIN_NO                        '金融機関コード
        nhkFormat.NHK_REC1.NH08 = ""
        nhkFormat.NHK_REC1.NH09 = ""
        nhkFormat.NHK_REC1.NH10 = ""
        nhkFormat.NHK_REC1.NH11 = ""
        nhkFormat.NHK_REC1.NH12 = ""
        nhkFormat.NHK_REC1.NH13 = ""
        nhkFormat.NHK_REC1.NH14 = TakouInfo.TKIN_NO                    '請求金融機関
        nhkFormat.NHK_REC1.NH15 = "*"

        nhkStreamWriter.Write(nhkFormat.NHK_REC1.Data)
        intRECORD_COUNT += 1

        Try
            BLOG.Write("データレコードの書込(開始)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
            If OraReader.DataReader(SQL) = True Then
                While (OraReader.EOF = False)
                    '--------------------------------------------
                    'データレコードの書込み
                    '--------------------------------------------
                    nhkFormat.RecordData = OraReader.GetItem("FURI_DATA_K")
                    dblGOUKEI_KEN += 1
                    dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                    '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
                    If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                        lngTKIN_KEN += 1
                        lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                    End If
                    '********************************************************
                    If GCOM.NzLong(OraReader.GetItem("FURIKIN_K")) = 0 Then
                        dblZEROKIN_KEN += 1
                    Else
                        dblYUUKIN_KEN += 1
                    End If

                    '    nhkFormat.RecordData = nhkFormat.RecordData.Substring(0, 112) & " " & nJIFURI_SEQ.ToString("00000000")
                    '
                    '    Try
                    '        SQL = New StringBuilder(128)
                    '        Sql.Append ("UPDATE MEIMAST SET KIGYO_SEQ_K = '" & nJIFURI_SEQ.ToString & "'")
                    '        Sql.Append (" WHERE TORIS_CODE_K ='" & strTORIS_CODE & "'")
                    '        Sql.Append ("   AND TORIF_CODE_K ='" & strTORIF_CODE & "'")
                    '        Sql.Append ("   AND FURI_DATE_K ='" & strFURI_DATE & "'")
                    '        Sql.Append ("   AND RECORD_NO_K = " & OraReader.GetItem("RECORD_NO_K"))
                    '
                    '        MainDB.ExecuteNonQuery (Sql)
                    '
                    '    Catch EX As Exception
                    '        '*** 修正 mitsu 2008/08/12 エラーメッセージ情報追加 ***
                    '        BLOG.Write("企業シーケンスの登録", "失敗", "金融機関コード：" & strKEIYAKU_KIN & " 取引先コード：" & strTORIS_CODE & "-" & strTORIF_CODE & " 振替日：" & strFURI_DATE & " " & EX.Message)
                    '        JobMessage = "企業シーケンスの登録失敗 金融機関コード：" & strKEIYAKU_KIN & " 取引先コード：" & strTORIS_CODE & "-" & strTORIF_CODE & " 振替日：" & strFURI_DATE
                    '        '******************************************************
                    '        Return False
                    '    End Try

                    '   nJIFURI_SEQ += 1

                    nhkStreamWriter.Write(nhkFormat.RecordData)
                    intRECORD_COUNT += 1

                    OraReader.NextRead()
                End While
            End If
            BLOG.Write("データレコードの書込(終了)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
            
        Catch EX As Exception
            '*** 修正 mitsu 2008/08/12 エラーメッセージ情報追加 ***
            BLOG.Write("ファイルの作成", "失敗", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE & " " & EX.Message)
            JobMessage = "ファイルの作成 金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE
            '******************************************************
            Return False
        Finally
            OraReader.Close()
        End Try

        '--------------------------------------------
        'トレーラレコードの書込み
        '--------------------------------------------
        nhkFormat.NHK_REC8.NH01 = "8"
        nhkFormat.NHK_REC8.NH02 = dblGOUKEI_KEN.ToString("000000")              '請求件数
        nhkFormat.NHK_REC8.NH03 = dblGOUKEI_KIN.ToString("000000000000")        '請求金額
        nhkFormat.NHK_REC8.NH04 = New String("0"c, 6)                           '振替済件数
        nhkFormat.NHK_REC8.NH05 = New String("0"c, 12)                          '振替済金額
        nhkFormat.NHK_REC8.NH06 = New String("0"c, 6)                           '振替不能件数
        nhkFormat.NHK_REC8.NH07 = New String("0"c, 12)                          '振替不能金額
        nhkFormat.NHK_REC8.NH08 = New String(" "c, 12)                          '振替手数料
        nhkFormat.NHK_REC8.NH09 = New String(" "c, 12)                          '入金額
        nhkFormat.NHK_REC8.NH10 = dblGOUKEI_KEN.ToString("000000")              'レコード数
        nhkFormat.NHK_REC8.NH11 = dblYUUKIN_KEN.ToString("000000")              '有金額
        nhkFormat.NHK_REC8.NH12 = dblZEROKIN_KEN.ToString("000000")             '金額「０」件数
        nhkFormat.NHK_REC8.NH13 = New String(" "c, 15)                          'ダミー
        nhkFormat.NHK_REC8.NH14 = New String("0"c, 3)                           'トレーラ識別
        nhkFormat.NHK_REC8.NH15 = New String(" "c, 4)                           '予備
        nhkFormat.NHK_REC8.NH16 = "*"
        nhkStreamWriter.Write(nhkFormat.NHK_REC8.Data)
        intRECORD_COUNT += 1

        '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
        'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
            dblGOUKEI_KEN = lngTKIN_KEN
            dblGOUKEI_KIN = lngTKIN_KIN
        End If
        '********************************************************

        '--------------------------------------------
        'エンドレコードの書込み
        '--------------------------------------------
        nhkFormat.NHK_REC9.NH01 = "9"
        nhkFormat.NHK_REC9.NH02 = New String(" "c, 111)
        nhkFormat.NHK_REC9.NH03 = New String(" "c, 3)
        nhkFormat.NHK_REC9.NH04 = TakouInfo.TKIN_NO
        nhkFormat.NHK_REC9.NH05 = "*"
        nhkStreamWriter.Write(nhkFormat.NHK_REC9.Data)
        nhkStreamWriter.Close()

        fn_CREATE_FILE_NHK = True
    End Function

    ''' <summary>
    ''' 全銀フォーマットの他行データを作成します。(120バイト)
    ''' </summary>
    ''' <param name="createFlag">紙フラグ</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改対応にて改修</remarks>
    Private Function fn_CREATE_FILE_ZENGIN(ByVal createFlag As Boolean) As Boolean
        '============================================================================
        'NAME           :fn_CREATE_FILE_120
        'Parameter      :
        'Description    :120バイトのファイル(JIS)作成
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/08/19
        'Update         :
        '============================================================================
        fn_CREATE_FILE_ZENGIN = False
        Dim intRECORD_COUNT As Integer

        '---------------------------------------------
        '作成対象金融機関のデータを抽出
        '---------------------------------------------
        Dim SQL As StringBuilder
        SQL = New StringBuilder(128)
        SQL.Append("SELECT * FROM MEIMAST")
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then    '農協対応１つのファイルにまとめて作成
            SQL.Append(" WHERE TORIS_CODE_K = '" & ToriInfo.TORIS_CODE & _
                                   "' AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & _
                                   "' AND FURI_DATE_K = '" & strFURI_DATE & _
                                   "' AND KEIYAKU_KIN_K BETWEEN  " & SQ(IniInfo.NOUKYOFROM) & " AND " & SQ(IniInfo.NOUKYOTO) & " AND DATA_KBN_K = '2'") 'INIファイルの設定を反映
            '*** 修正 mitsu 2008/08/12 まとめデータ作成フラグ ***
            blnMakeMATOME_DATA = True
            '****************************************************
        Else
            SQL.Append(" WHERE TORIS_CODE_K = '" & ToriInfo.TORIS_CODE & _
                                   "' AND TORIF_CODE_K = '" & ToriInfo.TORIF_CODE & _
                                   "' AND FURI_DATE_K = '" & strFURI_DATE & _
                                   "' AND KEIYAKU_KIN_K = '" & TakouInfo.TKIN_NO & _
                                   "' AND DATA_KBN_K = '2'")
        End If
        'If strParaTORI_CODE = New String("9", 7) & "88" And strTEIKEI_KBN <> "2" Then
        '    SQL.Append(" AND EXISTS (")
        '    SQL.Append(" SELECT BAITAI_CODE_V")
        '    SQL.Append("   FROM TAKOUMAST")
        '    SQL.Append("  WHERE TORIS_CODE_V = TORIS_CODE_K")
        '    SQL.Append("    AND TORIF_CODE_V = TORIF_CODE_K")
        '    If strTEIKEI_KBN = "1" Then
        '        ' 伝送
        '        SQL.Append(" AND BAITAI_CODE_V  = '00'")
        '    Else
        '        ' 伝送以外
        '        SQL.Append(" AND BAITAI_CODE_V  <> '00'")
        '    End If
        '    SQL.Append(" )")
        'End If
        '*** 修正 mitsu 2008/08/12 金融機関ソート追加 ***
        'SQL.Append(" ORDER BY KEIYAKU_SIT_K,RECORD_NO_K ASC")
        SQL.Append(" ORDER BY KEIYAKU_KIN_K, KEIYAKU_SIT_K, RECORD_NO_K")
        '************************************************

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
        Dim lngTKIN_KEN As Long = 0
        Dim lngTKIN_KIN As Long = 0
        '********************************************************

        ' 2008.04.09 ADD 紙は全銀ファイルを作らない >>
        If createFlag = False Then
            dblGOUKEI_KEN = 0
            dblGOUKEI_KIN = 0

            '*** 修正 mitsu 2008/08/12 例外処理追加 ***
            Try
                BLOG.Write("明細の検索(開始)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
                '**************************************
                If OraReader.DataReader(SQL) = True Then
                    While (OraReader.EOF = False)
                        dblGOUKEI_KEN += 1
                        dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                        '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
                        If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                            lngTKIN_KEN += 1
                            lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                        End If
                        '********************************************************

                        OraReader.NextRead()
                    End While

                    '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        dblGOUKEI_KEN = lngTKIN_KEN
                        dblGOUKEI_KIN = lngTKIN_KIN
                    End If
                    '********************************************************

                    Return True
                End If

                '*** 修正 mitsu 2008/08/12 例外処理追加 ***
            Catch EX As Exception
                BLOG.Write("明細の検索", "失敗", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE & " " & EX.Message)
                JobMessage = "明細の検索 金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE
                Return False
            Finally
                OraReader.Close()
            End Try
            '**********************************************
            BLOG.Write("明細の検索(終了)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
           End If
        ' 2008.04.09 ADD <<

        intRECORD_COUNT = 1
        dblGOUKEI_KEN = 0
        dblGOUKEI_KIN = 0

        '--------------------------------------------
        'ファイルのオープン
        '--------------------------------------------
        If File.Exists(strWRK_FILE_NAME) = True Then
            File.Delete(strWRK_FILE_NAME)
        End If

        Dim oFusion As New clsFUSION.clsMain

        ' 2008.01.17 >>
        'intFILE_NO_1 = FreeFile()
        'FileOpen(intFILE_NO_1, strWRK_FILE_NAME, OpenMode.Random, , , 120)    'ワークファイル
        Dim ZenFmt As New CAstFormat.CFormatZengin
        Dim ZenSWri As New StreamWriter(strWRK_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
        ' 2008.01.17 <<
        'FileOpen(intFILE_NO_1, strWRK_FILE_NAME, OpenMode.Random)    'ワークファイル

        '--------------------------------------------
        'ヘッダーレコードの書込み
        '--------------------------------------------
        ZenFmt.ZENGIN_REC1.ZG1 = "1"
        Select Case ToriInfo.NS_KBN
            Case "1"
                ZenFmt.ZENGIN_REC1.ZG2 = "21"
            Case "9"
                ZenFmt.ZENGIN_REC1.ZG2 = "91"
        End Select
        ZenFmt.ZENGIN_REC1.ZG3 = "1"
        ZenFmt.ZENGIN_REC1.ZG4 = TakouInfo.ITAKU_CODE
        ZenFmt.ZENGIN_REC1.ZG5 = strHEDDA_FURI_DATA.Substring(14, 40)
        ZenFmt.ZENGIN_REC1.ZG6 = strFURI_DATE.Substring(4, 4)
        ZenFmt.ZENGIN_REC1.ZG7 = TakouInfo.TKIN_NO
        '金融機関名取得
        ZenFmt.ZENGIN_REC1.ZG8 = GCOM.GetBKBRKName(TakouInfo.TKIN_NO, "").PadRight(30).Substring(0, 15)
        ZenFmt.ZENGIN_REC1.ZG10 = GCOM.GetBKBRKName(TakouInfo.TKIN_NO, TakouInfo.TSIT_NO).PadRight(30).Substring(0, 15)
        ZenFmt.ZENGIN_REC1.ZG9 = TakouInfo.TSIT_NO
        '2013/07/22 saitou 蒲郡信金 UPD -------------------------------------------------->>>>
        '通知預金追加＆呼び出す関数変更
        ZenFmt.ZENGIN_REC1.ZG11 = CASTCommon.ConvertKamoku2TO1(TakouInfo.KAMOKU)
        'ZenFmt.ZENGIN_REC1.ZG11 = oFusion.fn_CHG_KAMOKU2TO1(TakouInfo.KAMOKU)
        '2013/07/22 saitou 蒲郡信金 UPD --------------------------------------------------<<<<
        ZenFmt.ZENGIN_REC1.ZG12 = TakouInfo.KOUZA
        ZenFmt.ZENGIN_REC1.ZG13 = New String(" "c, 17)

        ZenSWri.Write(ZenFmt.ZENGIN_REC1.Data)

        intRECORD_COUNT += 1

        Try
            BLOG.Write("データレコードの書込(開始)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
             If OraReader.DataReader(SQL) = True Then
                While (OraReader.EOF = False)
                    '--------------------------------------------
                    'データレコードの書込み
                    '--------------------------------------------
                    'Dim strFURI_DATA As String
                    ZenFmt.RecordData = OraReader.GetItem("FURI_DATA_K")
                    dblGOUKEI_KEN += 1
                    dblGOUKEI_KIN += GCOM.NzDec(OraReader.GetItem("FURIKIN_K"))

                    ''2010/01/05 結果更新で矛盾がないようにするため、マスタの値を設定する > 不要(2010/01/12)
                    'ZenFmt.ZENGIN_REC2.Data = ZenFmt.RecordData
                    'ZenFmt.ZENGIN_REC2.ZG2 = OraReader.GetItem("KEIYAKU_KIN_K")                                     '金融機関コード
                    'ZenFmt.ZENGIN_REC2.ZG4 = OraReader.GetItem("KEIYAKU_SIT_K")                                     '支店コード
                    'ZenFmt.ZENGIN_REC2.ZG7 = CASTCommon.ConvertKamoku2TO1(OraReader.GetItem("KEIYAKU_KAMOKU_K"))    '科目
                    'ZenFmt.ZENGIN_REC2.ZG8 = OraReader.GetItem("KEIYAKU_KOUZA_K")                                   '口座番号
                    'ZenFmt.ZENGIN_REC2.ZG9 = GCOM.NzStr(OraReader.GetItem("KEIYAKU_KNAME_K"))                       '契約者名
                    'ZenFmt.ZENGIN_REC2.ZG10 = GCOM.NzStr(OraReader.GetItem("FURIKIN_K")).PadLeft(10, "0"c)          '振替金額
                    'ZenFmt.ZENGIN_REC2.ZG12 = Mid(GCOM.NzStr(OraReader.GetItem("JYUYOUKA_NO_K")), 1, 10)            '需要家番号
                    'ZenFmt.ZENGIN_REC2.ZG13 = Mid(GCOM.NzStr(OraReader.GetItem("JYUYOUKA_NO_K")), 11, 10)           '需要家番号
                    'ZenFmt.RecordData = ZenFmt.ZENGIN_REC2.Data
                    ''=================================================

                    '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
                    If OraReader.GetItem("KEIYAKU_KIN_K") = TakouInfo.TKIN_NO Then
                        lngTKIN_KEN += 1
                        lngTKIN_KIN += GCOM.NzLong(OraReader.GetItem("FURIKIN_K"))
                    End If
                    '********************************************************
                    '2010/01/04 企業シーケンスを入れるかをフラグで判定
                    If IniInfo.TAKO_SEQ = "1" Then

                        '結果更新で必要なため復活 ==================================
                        ZenFmt.RecordData = ZenFmt.RecordData.Substring(0, 112) & nJIFURI_SEQ.ToString("00000000")

                        Try
                            SQL = New StringBuilder(128)
                            SQL.Append("UPDATE MEIMAST SET KIGYO_SEQ_K = '" & nJIFURI_SEQ.ToString & "'")
                            SQL.Append(" WHERE TORIS_CODE_K ='" & ToriInfo.TORIS_CODE & "'")
                            SQL.Append("   AND TORIF_CODE_K ='" & ToriInfo.TORIF_CODE & "'")
                            SQL.Append("   AND FURI_DATE_K ='" & strFURI_DATE & "'")
                            SQL.Append("   AND RECORD_NO_K = " & OraReader.GetItem("RECORD_NO_K"))

                            MainDB.ExecuteNonQuery(SQL)

                        Catch EX As Exception
                            '*** 修正 mitsu 2008/08/12 エラーメッセージ情報追加 ***
                            BLOG.Write("企業シーケンスの登録", "失敗", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE & " " & EX.Message)
                            JobMessage = "企業シーケンスの登録失敗 金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE
                            '******************************************************
                            Return False
                        End Try

                        nJIFURI_SEQ += 1
                        '============================================================
                    End If
                    ' 2008.01.17 >>
                    'FilePut(intFILE_NO_1, gstrDATA, intRECORD_COUNT)
                    ZenSWri.Write(ZenFmt.RecordData)
                    ' 2008.01.17 <<
                    intRECORD_COUNT += 1

                    OraReader.NextRead()
                End While
            End If

            BLOG.Write("データレコードの書込(終了)", "成功", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE)
            
        Catch EX As Exception
            '*** 修正 mitsu 2008/08/12 エラーメッセージ情報追加 ***
            BLOG.Write("ファイルの作成", "失敗", "金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE & " " & EX.Message)
            JobMessage = "ファイルの作成 金融機関コード：" & TakouInfo.TKIN_NO & " 取引先コード：" & ToriInfo.TORIS_CODE & "-" & ToriInfo.TORIF_CODE & " 振替日：" & strFURI_DATE
            '******************************************************
            Return False
        Finally
            OraReader.Close()
        End Try

        '--------------------------------------------
        'トレーラレコードの書込み
        '--------------------------------------------
        ZenFmt.ZENGIN_REC8.ZG1 = "8"
        ZenFmt.ZENGIN_REC8.ZG2 = dblGOUKEI_KEN.ToString("000000")
        ZenFmt.ZENGIN_REC8.ZG3 = dblGOUKEI_KIN.ToString("000000000000")
        '*** 修正 mitsu 2008/08/11 振替済・不能欄も0埋めする ***
        'ZenFmt.ZENGIN_REC8.ZG4 = New String(" "c, 101)
        ZenFmt.ZENGIN_REC8.ZG4 = New String("0"c, 6)
        ZenFmt.ZENGIN_REC8.ZG5 = New String("0"c, 12)
        ZenFmt.ZENGIN_REC8.ZG6 = New String("0"c, 6)
        ZenFmt.ZENGIN_REC8.ZG7 = New String("0"c, 12)
        ZenFmt.ZENGIN_REC8.ZG8 = New String(" "c, 65)
        '*******************************************************
        ' 2008.01.17 >>
        'FilePut(intFILE_NO_1, gZENGIN_REC8, intRECORD_COUNT)
        ZenSWri.Write(ZenFmt.ZENGIN_REC8.Data)
        ' 2008.01.17 <<
        intRECORD_COUNT += 1

        '*** 修正 mitsu 2008/08/11 strKEIYAKU_KINの件数・金額 ***
        'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
        If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
            dblGOUKEI_KEN = lngTKIN_KEN
            dblGOUKEI_KIN = lngTKIN_KIN
        End If
        '********************************************************

        '--------------------------------------------
        'エンドレコードの書込み
        '--------------------------------------------
        ZenFmt.ZENGIN_REC9.ZG1 = "9"
        ZenFmt.ZENGIN_REC9.ZG2 = New String(" "c, 119)
        ' 2008.01.17 >>
        'FilePut(intFILE_NO_1, gZENGIN_REC9, intRECORD_COUNT)

        'FileClose(intFILE_NO_1)
        ZenSWri.Write(ZenFmt.ZENGIN_REC9.Data)
        ZenSWri.Close()
        ' 2008.01.17 <<

        fn_CREATE_FILE_ZENGIN = True
    End Function

    ''' <summary>
    ''' スケジュールマスタを参照するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks>蒲郡信金 ハード更改対応にて新規追加</remarks>
    Private Function CreateGetSchmastSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            '他行作成対象の集金代行以外を条件とする
            .Append("select * from SCHMAST")
            .Append(" inner join TORIMAST")
            .Append(" on TORIS_CODE_S = TORIS_CODE_T")
            .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            .Append(" where FURI_DATE_S = " & SQ(strFURI_DATE))
            .Append(" and TORIS_CODE_S = " & SQ(strTORIS_CODE))
            .Append(" and TORIF_CODE_S = " & SQ(strTORIF_CODE))
            .Append(" and TOUROKU_FLG_S = '1'")
            .Append(" and HAISIN_FLG_S = '0'")
            .Append(" and TYUUDAN_FLG_S = '0'")
            .Append(" and TAKO_KBN_T = '1'")
            '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            '集金代行は20(内)と21(内外)
            .Append(" and FMT_KBN_T not in ('20', '21')")
            '.Append(" and FMT_KBN_T <> '20'")
            '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 他行スケジュールマスタを削除します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function DeleteTakoschmast() As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("delete from TAKOSCHMAST")
            .Append(" where FURI_DATE_U = " & SQ(strFURI_DATE))
            .Append(" and TORIS_CODE_U = " & SQ(strTORIS_CODE))
            .Append(" and TORIF_CODE_U = " & SQ(strTORIF_CODE))
        End With

        Try
            BLOG.Write("(他行スケジュールマスタ削除)開始", "成功", "取引先コード：" & strTORI_CODE & " 振替日：" & strFURI_DATE)
            '対象が存在しなくてもいい、エラーにならなければいい
            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                BLOG.Write("他行スケジュールマスタ削除", "失敗", "取引先コード：" & strTORI_CODE & " 振替日：" & strFURI_DATE)
                Return False
            End If
            Return True
        Catch ex As Exception
            BLOG.Write("他行スケジュールマスタ削除", "失敗", "取引先コード：" & strTORI_CODE & " 振替日：" & strFURI_DATE & " " & ex.Message)
            Return False
        Finally
            BLOG.Write("(他行スケジュールマスタ削除)終了", "成功", "取引先コード：" & strTORI_CODE & " 振替日：" & strFURI_DATE)
             End Try
    End Function

    ''' <summary>
    ''' 他行スケジュールマスタを登録します。
    ''' </summary>
    ''' <param name="oraReader">スケジュールオラクルリーダー</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改対応にて新規追加</remarks>
    Private Function InsertTakoschmast(ByVal oraReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("insert into TAKOSCHMAST values (")
            .Append(" " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append("," & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append("," & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append("," & SQ(oraReader.GetString("FUNOU_YDATE_S")))
            .Append("," & SQ(oraReader.GetString("FMT_KBN_T")))
            .Append("," & SQ(TakouInfo.BAITAI_CODE))
            .Append("," & SQ(oraReader.GetString("LABEL_KBN_T")))
            .Append("," & SQ(TakouInfo.CODE_KBN))
            .Append("," & SQ(TakouInfo.TKIN_NO))
            .Append("," & SQ("0"))
            .Append("," & dblGOUKEI_KEN.ToString)
            .Append("," & dblGOUKEI_KIN.ToString)
            .Append(",0")
            .Append(",0")
            .Append(",0")
            .Append(",0")
            .Append("," & SQ(strDate))
            .Append(")")
        End With

        Dim DELSQL As New StringBuilder
        With DELSQL
            .Append("delete from TAKOSCHMAST")
            .Append(" where TORIS_CODE_U = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_U = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_U = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and TKIN_NO_U = " & SQ(TakouInfo.TKIN_NO))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(DELSQL)
            If nRet > 0 Then
                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行スケジュール作成", "成功", "先行レコードを削除")
            End If

            nRet = MainDB.ExecuteNonQuery(SQL)
            If nRet > 0 Then
                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行スケジュール作成", "成功")
            End If

            Return True

        Catch ex As Exception
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行スケジュール作成", "失敗", "金融機関コード：" & TakouInfo.TKIN_NO & " " & ex.Message)
            JobMessage = "他行スケジュール作成失敗 例外発生"
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 自振シーケンスの最大値＋１を取得します。
    ''' </summary>
    ''' <returns>最大自振シーケンス＋１</returns>
    ''' <remarks>蒲郡信金 ハード更改対応にて新規追加</remarks>
    Private Function GetMaxJifuriSEQPlusOne() As Long
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder
        With SQL
            .Append("select nvl(max(KIGYO_SEQ_K), 79999999) as JIFURI_SEQ_MAX")
            .Append(", count(KIGYO_SEQ_K) as JIFURI_SEQ_CNT")
            .Append(" from MEIMAST")
            .Append(" where FURI_DATE_K = " & SQ(strFURI_DATE))
            .Append(" and KIGYO_SEQ_K between '80000000' and '89999999'")
        End With

        Try
            If oraReader.DataReader(SQL) = True Then
                Return oraReader.GetInt64("JIFURI_SEQ_MAX") + 1
            Else
                Return 80000000
            End If
        Catch ex As Exception
            Return 80000000
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 明細マスタからヘッダレコードを取得するSQLを作成します。
    ''' </summary>
    ''' <param name="oraReader">スケジュールオラクルリーダー</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks>蒲郡信金 ハード更改対応にて新規追加</remarks>
    Private Function CreateGetMeimastHeaderRecord(ByVal oraReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select FURI_DATA_K from MEIMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and DATA_KBN_K = '1'")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 明細マスタから他行データ作成対象の金融機関を取得します。
    ''' </summary>
    ''' <param name="oraReader">スケジュールオラクルリーダー</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks>蒲郡信金 ハード更改対応にて新規追加</remarks>
    Private Function CreateGetMeimastKeiyakuKin(ByVal oraReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select distinct KEIYAKU_KIN_K from MEIMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraReader.GetString("FURI_DATE_S")))
            .Append(" and DATA_KBN_K = '2'")
            .Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
            .Append(" and FURIKETU_CODE_K = 0")
            .Append(" order by KEIYAKU_KIN_K")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 他行マスタを参照します。
    ''' </summary>
    ''' <param name="TORIS_CODE">取引先主コード</param>
    ''' <param name="TORIF_CODE">取引先副コード</param>
    ''' <param name="TKIN_NO">金融機関コード</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改にて新規追加</remarks>
    Private Function GetTakoumast(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, ByVal TKIN_NO As String) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            SQL.Append("SELECT * FROM TAKOUMAST")
            SQL.Append(" WHERE TORIS_CODE_V = " & SQ(TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_V = " & SQ(TORIF_CODE))
            SQL.Append(" AND TKIN_NO_V = " & SQ(TKIN_NO))

            If oraReader.DataReader(SQL) = False Then
                Return False
            Else
                TakouInfo.SetOraData(oraReader)
            End If
            Return True
        Catch ex As Exception
            BLOG.Write("他行マスタ検索", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 取引先マスタを参照します。
    ''' </summary>
    ''' <param name="TORIS_CODE">取引先主コード</param>
    ''' <param name="TORIF_CODE">取引先副コード</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改にて新規追加</remarks>
    Private Function GetTorimast(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF_CODE))

            If oraReader.DataReader(SQL) = False Then
                Return False
            Else
                ToriInfo.SetOraData(oraReader)
            End If
            Return True
        Catch ex As Exception
            BLOG.Write("取引先マスタ検索", "失敗", ex.Message)

            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 他行明細表を印刷します。
    ''' </summary>
    ''' <param name="strTorisCode">取引先主コード</param>
    ''' <param name="strTorifCode">取引先副コード</param>
    ''' <param name="strFuriDate">振替日</param>
    ''' <param name="strKeiyakuKin">金融機関コード</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改にて新規追加</remarks>
    Private Function PrnTakoumeisaiList(ByVal strTorisCode As String, _
                                        ByVal strTorifCode As String, _
                                        ByVal strFuriDate As String, _
                                        ByVal strKeiyakuKin As String) As Boolean
        Try
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim ParamRepo As String = ""
            Dim DQ As String = """"

            ParamRepo = strTorisCode & strTorifCode & _
                        "," & strFuriDate & _
                        "," & strKeiyakuKin
            BLOG.Write("他行明細表印刷(開始)", "成功", ParamRepo)

            Dim nRet As Integer = ExeRepo.ExecReport("KFJP006.EXE", ParamRepo)
            If nRet = 0 Then
                BLOG.Write("他行明細表印刷(終了)", "成功")
            Else
                BLOG.Write("他行明細表印刷(終了)", "失敗")
            End If

            Return True

        Catch ex As Exception
            BLOG.Write("他行明細表印刷", "失敗", ex.Message)
            JobMessage = "他行明細表印刷失敗"
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 口座振替請求データ送付票を印刷します。
    ''' </summary>
    ''' <param name="strTorisCode">取引先主コード</param>
    ''' <param name="strTorifCode">取引先副コード</param>
    ''' <param name="strFuriDate">振替日</param>
    ''' <param name="strKeiyakuKin">金融機関コード</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改にて新規追加</remarks>
    Private Function PrnKouzafurikaeSeikyuDataInvoice(ByVal strTorisCode As String, _
                                                      ByVal strTorifCode As String, _
                                                      ByVal strFuriDate As String, _
                                                      ByVal strKeiyakuKin As String) As Boolean
        Try
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim ParamRepo As String = ""
            Dim DQ As String = """"

            ParamRepo = strTorisCode & strTorifCode & _
                        "," & strFuriDate & _
                        "," & strKeiyakuKin
            BLOG.Write("口座振替請求データ送付票(開始)", "成功", ParamRepo)

            Dim nRet As Integer = ExeRepo.ExecReport("KFJP007.EXE", ParamRepo)
            If nRet = 0 Then
                BLOG.Write("口座振替請求データ送付票(終了)", "成功")
            Else
                BLOG.Write("口座振替請求データ送付票(終了)", "失敗")
            End If

            Return True

        Catch ex As Exception
            BLOG.Write("口座振替請求データ送付票印刷", "失敗", ex.Message)
            JobMessage = "口座振替請求データ送付票印刷失敗"
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 媒体書込みを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改にて改修</remarks>
    Private Function fn_BAITAI_WRITE() As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_WRITE
        'Parameter      :
        'Description    :媒体書込み処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/08/20
        'Update         :
        '============================================================================
        fn_BAITAI_WRITE = False
        Dim strTAKOU_SEND_FILE As String
        Dim intKEKKA As Integer
        Dim strKEKKA As String
        Dim strKAKUTYOUSI As String

        Select Case TakouInfo.BAITAI_CODE
            Case "00"        '伝送
                Select Case ToriInfo.FMT_KBN
                    Case "00"                   '全銀
                        strKAKUTYOUSI = ".120"
                    Case "01"                   'ＮＨＫ
                        strKAKUTYOUSI = ".120"
                    Case "02"
                        strKAKUTYOUSI = ".390"  '国税
                    Case "03"
                        strKAKUTYOUSI = ".130"  '年金
                    Case Else
                        strKAKUTYOUSI = ".120"
                End Select
                If TakouInfo.SFILE_NAME.Trim = "" Then  '他行マスタに送信ファイル名が設定されていない→一括送信ファイル作成対象
                    'ファイル名のセット
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        strTAKOU_SEND_FILE = Path.Combine(IniInfo.DEN & IniInfo.NOUKYOMATOME, "T" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & strKAKUTYOUSI)
                    Else
                        strTAKOU_SEND_FILE = Path.Combine(IniInfo.DEN & TakouInfo.TKIN_NO, "T" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & strKAKUTYOUSI)
                    End If
                    'フォルダの存在確認
                    Dim strFOLDER As String
                    'If 5874 <= CInt(strKEIYAKU_KIN) And CInt(strKEIYAKU_KIN) <= 5939 Then
                    If CInt(IniInfo.NOUKYOFROM) <= CInt(TakouInfo.TKIN_NO) And CInt(TakouInfo.TKIN_NO) <= CInt(IniInfo.NOUKYOTO) Then
                        strFOLDER = IniInfo.DEN & IniInfo.NOUKYOMATOME
                        If Not Directory.Exists(strFOLDER) Then
                            'フォルダ作成
                            Directory.CreateDirectory(strFOLDER)
                        End If
                    Else
                        strFOLDER = IniInfo.DEN & TakouInfo.TKIN_NO
                        If Not Directory.Exists(strFOLDER) Then
                            'フォルダ作成
                            Directory.CreateDirectory(strFOLDER)
                        End If
                    End If
                Else
                    strTAKOU_SEND_FILE = IniInfo.DEN & TakouInfo.SFILE_NAME     '出力ファイル
                End If
                intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                                                       strWRK_FILE_NAME, _
                                                       strTAKOU_SEND_FILE, _
                                                       intREC_LENGTH, _
                                                       TakouInfo.CODE_KBN, _
                                                       gstrP_FILE)

                Select Case intKEKKA
                    Case 0
                        fn_BAITAI_WRITE = True
                        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ファイル変換")
                    Case 100
                        fn_BAITAI_WRITE = False
                        'Return         :0=成功、100=コード変換失敗
                        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ファイル変換（コード変換）")
                        JobMessage = "ファイル変換（コード変換）失敗"
                        Return False
                End Select

            Case "01"    '３．５ＦＤ
                ' 2016/01/18 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                'strTAKOU_SEND_FILE = TakouInfo.SFILE_NAME.Trim
                'Select Case TakouInfo.CODE_KBN
                '    Case "0", "1", "2", "3"   'DOS形式
                '        intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                '                                              strWRK_FILE_NAME, _
                '                                              strTAKOU_SEND_FILE, _
                '                                              intREC_LENGTH, _
                '                                              TakouInfo.CODE_KBN, _
                '                                              gstrP_FILE, _
                '                                              True, msgTitle)
                '    Case "4"        'IBM形式
                '        intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                '                                              strWRK_FILE_NAME, _
                '                                              strTAKOU_SEND_FILE, _
                '                                              intREC_LENGTH, _
                '                                              TakouInfo.CODE_KBN, _
                '                                              strIBMP_FILE, _
                '                                              True, msgTitle)
                'End Select

                'Select Case intKEKKA
                '    Case 0
                '        fn_BAITAI_WRITE = True
                '        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ＦＤ書込み")
                '    Case 100
                '        fn_BAITAI_WRITE = False
                '        'Return         :0=成功、100=ＦＤ書込み失敗（IBM形式）、200=ＦＤ書込み失敗（DOS形式）
                '        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＦＤ書込み（IBM形式）")
                '        JobMessage = "ＦＤ書込み失敗（IBM形式）"
                '        Return False
                '    Case 200
                '        fn_BAITAI_WRITE = False
                '        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＦＤ書込み失敗（IBM形式）")
                '        JobMessage = "ＦＤ書込み失敗（DOS形式）"
                '        Return False
                'End Select
                Select Case IniInfo.RSV2_EDITION
                    Case "2"
                        '-------------------------------------------------
                        ' 出力ファイル名構築
                        '-------------------------------------------------
                        strTAKOU_SEND_FILE = IniInfo.COMMON_BAITAIWRITE & "TAK_FD_" & strTORI_CODE & "_" & strFURI_DATE & "_" & TakouInfo.TKIN_NO


                        '-------------------------------------------------
                        ' ファイル出力
                        '-------------------------------------------------
                        '2016/02/08 タスク）岩城 RSV2対応 MODIFY ------------------------------------- START
                        'コード変換すると媒体変換処理のコード変換でさらにコード変換してしまうため、返還データ作成ではコード変換を行わない。
                        If Dir(strTAKOU_SEND_FILE) <> "" Then
                            Kill(strTAKOU_SEND_FILE)
                        End If
                        File.Copy(strWRK_FILE_NAME, strTAKOU_SEND_FILE)
                        fn_BAITAI_WRITE = True
                        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ファイル変換")
                        'intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                        '                               strWRK_FILE_NAME, _
                        '                               strTAKOU_SEND_FILE, _
                        '                               intREC_LENGTH, _
                        '                               TakouInfo.CODE_KBN, _
                        '                               gstrP_FILE)

                        'Select Case intKEKKA
                        '    Case 0
                        '        fn_BAITAI_WRITE = True
                        '        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ファイル変換")
                        '    Case 100
                        '        fn_BAITAI_WRITE = False
                        '        BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ファイル変換（コード変換）")
                        '        JobMessage = "ファイル変換（コード変換）失敗"
                        '        Return False
                        'End Select
                        '2016/02/08 タスク）岩城 RSV2対応 MODIFY ------------------------------------- END

                    Case Else
                        strTAKOU_SEND_FILE = TakouInfo.SFILE_NAME.Trim
                        Select Case TakouInfo.CODE_KBN
                            Case "0", "1", "2", "3"   'DOS形式
                                intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                                                                      strWRK_FILE_NAME, _
                                                                      strTAKOU_SEND_FILE, _
                                                                      intREC_LENGTH, _
                                                                      TakouInfo.CODE_KBN, _
                                                                      gstrP_FILE, _
                                                                      True, msgTitle)
                            Case "4"        'IBM形式
                                intKEKKA = clsFusion.fn_DISK_CPYTO_FD(strTORI_CODE, _
                                                                      strWRK_FILE_NAME, _
                                                                      strTAKOU_SEND_FILE, _
                                                                      intREC_LENGTH, _
                                                                      TakouInfo.CODE_KBN, _
                                                                      strIBMP_FILE, _
                                                                      True, msgTitle)
                        End Select

                        Select Case intKEKKA
                            Case 0
                                fn_BAITAI_WRITE = True
                                BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ＦＤ書込み")
                            Case 100
                                fn_BAITAI_WRITE = False
                                'Return         :0=成功、100=ＦＤ書込み失敗（IBM形式）、200=ＦＤ書込み失敗（DOS形式）
                                BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＦＤ書込み（IBM形式）")
                                JobMessage = "ＦＤ書込み失敗（IBM形式）"
                                Return False
                            Case 200
                                fn_BAITAI_WRITE = False
                                BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＦＤ書込み失敗（IBM形式）")
                                JobMessage = "ＦＤ書込み失敗（DOS形式）"
                                Return False
                        End Select
                End Select
                ' 2016/01/18 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
            Case "04"        '依頼書
                '後処理で他行明細表印刷

            Case "05"        'ＭＴ
                Select Case IniInfo.MT
                    Case "0"     'ＭＴが直接自振サーバに接続している場合
                        Dim lngErrStatus As Long
                        Dim intBlkSize As Integer
                        Select Case intREC_LENGTH
                            Case 120
                                intBlkSize = 1800
                            Case 220
                                intBlkSize = 2200
                            Case 300
                                intBlkSize = 3000
                        End Select

                        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                        strKEKKA = vbDLL.mtCPYtoMT(intBlkSize, intREC_LENGTH, 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, lngErrStatus)
                        'strKEKKA = vbDLL.mtCPYtoMT(CShort(intBlkSize), CShort(intREC_LENGTH), 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, CInt(lngErrStatus))
                        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                        If strKEKKA <> "" Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＭＴ書込み")
                            JobMessage = "ＭＴ書込み失敗"
                            Return False
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ＭＴ書込み")
                        End If
                    Case "1"      'ＭＴが自振サーバに接続していない場合
                        '2010/01/25 DEN直下のTAKOUフォルダに金融機関コードで保存する
                        'If INI_COMMON_MTTAKOUFILE = "" Then
                        '    BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＭＴ書込み(ファイル名取得)失敗")
                        '    JobMessage = "ＭＴ書込み(ファイル名取得)失敗"
                        '    Return False
                        'End If
                        'If Dir(INI_COMMON_MTTAKOUFILE) <> "" Then
                        '    Kill(INI_COMMON_MTTAKOUFILE)
                        'End If
                        'FileCopy(strWRK_FILE_NAME, INI_COMMON_MTTAKOUFILE)
                        If Directory.Exists(Path.Combine(IniInfo.DEN, "TAKOU")) = False Then
                            Directory.CreateDirectory(Path.Combine(IniInfo.DEN, "TAKOU"))
                        End If
                        'FileCopy(strWRK_FILE_NAME, Path.Combine(Path.Combine(INI_COMMON_DEN, "TAKOU"), strKEIYAKU_KIN))
                        intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                                                               strWRK_FILE_NAME, _
                                                               Path.Combine(Path.Combine(IniInfo.DEN, "TAKOU"), TakouInfo.TKIN_NO), _
                                                               intREC_LENGTH, _
                                                               TakouInfo.CODE_KBN, _
                                                               gstrP_FILE)
                        '==========================================================
                        If Err.Number = 0 Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ＭＴ書込み(ファイルコピー)")
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ＭＴ書込み(ファイルコピー)失敗")
                            JobMessage = "ＭＴ書込み(ファイルコピー)失敗"
                            Return False
                        End If

                End Select
            Case "06"        'ＣＭＴ
                Select Case IniInfo.CMT
                    Case "0"    'ＣＭＴが直接自振サーバに接続している場合
                        Dim lngErrStatus As Long
                        Dim intBlkSize As Integer
                        Select Case intREC_LENGTH
                            Case 120
                                intBlkSize = 1800
                            Case 220
                                intBlkSize = 2200
                            Case 300
                                intBlkSize = 3000
                        End Select

                        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                        strKEKKA = vbDLL.cmtCPYtoMT(intBlkSize, intREC_LENGTH, 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, lngErrStatus)
                        'strKEKKA = vbDLL.cmtCPYtoMT(CShort(intBlkSize), CShort(intREC_LENGTH), 1, "SMLT", 1, 4, strWRK_FILE_NAME, " ", 0, CInt(lngErrStatus))
                        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                        If strKEKKA <> "" Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＣＭＴ書込み")
                            JobMessage = "ＣＭＴ書込み失敗"
                            Return False
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ＣＭＴ書込み")
                        End If
                    Case "1"    'ＣＭＴが自振サーバに接続していない場合
                        '2010/01/25 DEN直下のTAKOUフォルダに金融機関コードで保存する
                        'If INI_COMMON_CMTTAKOUFILE = "" Then
                        '    BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＣＭＴ書込み(ファイル名取得)失敗")
                        '    JobMessage = "ＣＭＴ書込み(ファイル名取得)失敗"
                        '    Return False
                        'End If
                        'If Dir(INI_COMMON_CMTTAKOUFILE) <> "" Then
                        '    Kill(INI_COMMON_CMTTAKOUFILE)
                        'End If
                        'FileCopy(strWRK_FILE_NAME, INI_COMMON_CMTTAKOUFILE)
                        If Directory.Exists(Path.Combine(IniInfo.DEN, "TAKOU")) = False Then
                            Directory.CreateDirectory(Path.Combine(IniInfo.DEN, "TAKOU"))
                        End If
                        'FileCopy(strWRK_FILE_NAME, Path.Combine(Path.Combine(INI_COMMON_DEN, "TAKOU"), strKEIYAKU_KIN))
                        intKEKKA = clsFusion.fn_DISK_CPYTO_DEN(strTORI_CODE, _
                                                               strWRK_FILE_NAME, _
                                                               Path.Combine(Path.Combine(IniInfo.DEN, "TAKOU"), TakouInfo.TKIN_NO), _
                                                               intREC_LENGTH, _
                                                               TakouInfo.CODE_KBN, _
                                                               gstrP_FILE)
                        '==========================================================
                        If Err.Number = 0 Then
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "成功", "ＣＭＴ書込み(ファイルコピー)")
                        Else
                            BLOG.Write(strTORI_CODE, strFURI_DATE, "他行データ作成", "失敗", "ＣＭＴ書込み(ファイルコピー)失敗")
                            JobMessage = "ＣＭＴ書込み(ファイルコピー)失敗"
                            Return False
                        End If
                End Select
        End Select

        fn_BAITAI_WRITE = True
    End Function

    ''' <summary>
    ''' 金融機関マスタに存在するかチェックします。
    ''' </summary>
    ''' <param name="strKinCd">金融機関コード</param>
    ''' <param name="strSitCd">支店コード</param>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改にて新規追加</remarks>
    Private Function CheckTenmast(ByVal strKinCd As String, _
                                  ByVal strSitCd As String) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        SQL.Append("select * from TENMAST where KIN_NO_N = " & SQ(strKinCd) & " and SIT_NO_N = " & SQ(strSitCd))

        Try
            Return oraReader.DataReader(SQL)
        Catch ex As Exception
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' INIファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>蒲郡信金 ハード更改にて改修</remarks>
    Private Function fn_INI_READ() As Boolean
        Try
            BLOG.Write("INIファイル読込(開始)", "成功")

            IniInfo.DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If IniInfo.DEN.ToUpper.Equals("ERR") = True OrElse IniInfo.DEN = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].DEN 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].DEN 設定なし")
                Return False
            End If

            IniInfo.TAK = CASTCommon.GetFSKJIni("COMMON", "TAK")
            If IniInfo.TAK.ToUpper.Equals("ERR") = True OrElse IniInfo.TAK = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].TAK 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].TAK 設定なし")
                Return False
            End If

            IniInfo.KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If IniInfo.KINKOCD.ToUpper.Equals("ERR") = True OrElse IniInfo.KINKOCD = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].KINKOCD 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].KINKOCD 設定なし")
                Return False
            End If

            IniInfo.MT = CASTCommon.GetFSKJIni("COMMON", "MT")
            If IniInfo.MT.ToUpper.Equals("ERR") = True OrElse IniInfo.MT = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].MT 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].MT 設定なし")
                Return False
            End If

            IniInfo.CMT = CASTCommon.GetFSKJIni("COMMON", "CMT")
            If IniInfo.CMT.ToUpper.Equals("ERR") = True OrElse IniInfo.CMT = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].CMT 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].CMT 設定なし")
                Return False
            End If

            IniInfo.TAKO_SEQ = CASTCommon.GetFSKJIni("JIFURI", "TAKO_SEQ")
            If IniInfo.TAKO_SEQ.ToUpper.Equals("ERR") = True OrElse IniInfo.TAKO_SEQ = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[JIFURI].TAKO_SEQ 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [JIFURI].TAKO_SEQ 設定なし")
                Return False
            End If

            IniInfo.NOUKYOMATOME = CASTCommon.GetFSKJIni("TAKO", "NOUKYOMATOME")
            If IniInfo.NOUKYOMATOME.ToUpper.Equals("ERR") = True OrElse IniInfo.NOUKYOMATOME = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[TAKO].NOUKYOMATOME 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [TAKO].NOUKYOMATOME 設定なし")
                Return False
            End If

            IniInfo.NOUKYOFROM = CASTCommon.GetFSKJIni("TAKO", "NOUKYOFROM")
            If IniInfo.NOUKYOFROM.ToUpper.Equals("ERR") = True OrElse IniInfo.NOUKYOFROM = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[TAKO].NOUKYOFROM 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [TAKO].NOUKYOFROM 設定なし")
                Return False
            End If

            IniInfo.NOUKYOTO = CASTCommon.GetFSKJIni("TAKO", "NOUKYOTO")
            If IniInfo.NOUKYOTO.ToUpper.Equals("ERR") = True OrElse IniInfo.NOUKYOTO = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[TAKO].NOUKYOTO 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [TAKO].NOUKYOTO 設定なし")
                Return False
            End If

            ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            IniInfo.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If IniInfo.RSV2_EDITION.ToUpper.Equals("ERR") = True OrElse IniInfo.RSV2_EDITION = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[RSV2_V1.0.0].EDITION 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [RSV2_V1.0.0].EDITION 設定なし")
                Return False
            End If

            IniInfo.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
            If IniInfo.COMMON_BAITAIWRITE.ToUpper.Equals("ERR") = True OrElse IniInfo.COMMON_BAITAIWRITE = Nothing Then
                BLOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].BAITAIWRITE 設定なし")
                BLOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].BAITAIWRITE 設定なし")
                Return False
            End If
            ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            BLOG.Write("INIファイル読込(終了)", "成功")

        Catch ex As Exception
            BLOG.Write("INIファイル読込", "失敗", ex.Message)
            Return False
        End Try

        '*** 修正 mitsu 2008/08/27 他行フォルダが存在しない場合 ***
        Try
            If Directory.Exists(IniInfo.TAK) = False Then
                Directory.CreateDirectory(IniInfo.TAK)
            End If
        Catch ex As Exception
            BLOG.Write("他行フォルダ作成", "失敗", ex.Message)
            Return False
        End Try
        '**********************************************************
        Return True
    End Function

#End Region

    Private Class mySortClass
        Implements IComparer

        ' Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare

            Dim strX() As String = CType(x, String())
            Dim strY() As String = CType(y, String())

            Dim splitX() As String = strX(0).Split("."c)     ' 金融機関,振替日
            Dim splitY() As String = strY(0).Split("."c)     ' 金融機関,振替日

            Dim nRet As Integer = 0
            ' 金融機関を先に比較
            nRet = String.Compare(splitX(0), splitY(0))
            If nRet = 0 Then
                ' 振替日を比較
                nRet = String.Compare(splitX(1), splitY(1))
            End If
            If nRet = 0 Then
                ' フォルダ名を比較
                Return New CaseInsensitiveComparer().Compare(strX(0), strY(0))
            End If
            Return nRet
        End Function 'IComparer.Compare

    End Class 'myReverserClass

End Module
