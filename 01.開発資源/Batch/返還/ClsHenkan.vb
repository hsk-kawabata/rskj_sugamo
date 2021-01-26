Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CASTCommon
Imports System.Data
Imports Microsoft.VisualBasic
Imports System.Windows.Forms
Imports System.Configuration
Imports System.Reflection
Imports System.Xml
Imports CAstExtendPrint

Public Class ClsHenkan
    ' ログ処理クラス
    Private LOG As New CASTCommon.BatchLOG("KFJ060", "返還データ作成")
    Private Const msgTitle As String = "返還データ作成(KFJP060)"

    Public clsFUSION As New clsFUSION.clsMain()
    Private vbDLL As New CMTCTRL.ClsFSKJ

    Private mLockWaitTime3 As Integer = 600
    Private mLockWaitTime4 As Integer = 30

    ' 個別起動パラメータ構造体
    Friend Structure KOBETUPARAM
        Dim strTORIS_CODE As String         '取引先主コード
        Dim strTORIF_CODE As String         '取引先副コード
        Dim strFURI_DATE As String          '振替日
        Dim strMOTIKOMI_SEQ As String       '持込シーケンス
        Dim strJOB_TUUBAN As String         'ジョブ通番

        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                Select Case para.Length
                    Case 4
                        strTORIS_CODE = para(0).Substring(0, 10)
                        strTORIF_CODE = para(0).Substring(10, 2)
                        strFURI_DATE = para(1)
                        strMOTIKOMI_SEQ = para(2)
                        strJOB_TUUBAN = para(3)
                End Select
            End Set
        End Property
    End Structure
    Private strcKOBETUPARAM As KOBETUPARAM

    Friend Structure INI_PARAM
        Dim DEN_FOLDER As String                    '伝送フォルダ
        Dim CSV_FOLDER As String                    'CSVフォルダ
        Dim DAT_FOLDER As String                    'DATフォルダ
        Dim DATBK_FOLDER As String                  'データバックアップフォルダ
        Dim LOG_FOLDER As String                    'LOGフォルダ
        Dim MT_FLG As String                        'MT フラグ
        Dim MT_INFILE As String                     'MT 入力ファイル
        Dim MT_OUTFILE As String                    'MT 出力ファイル
        Dim MT_TAKOUFILE As String                  'MT 他行ファイル
        Dim MT_FUNOUFILE As String                  'MT 不能ファイル
        Dim CMT_FLG As String                       'CMT フラグ
        Dim CMT_INFILE As String                    'CMT 入力ファイル
        Dim CMT_OUTFILE As String                   'CMT 出力ファイル
        Dim CMT_TAKOUFILE As String                 'CMT 他行ファイル
        Dim CMT_FUNOUFILE As String                 'CMT 不能ファイル
        Dim WEB_SED_FOLDER As String                'WEB伝送フォルダ(送信)  '2012/06/30 標準版　WEB伝送対応
        Dim RSV2_EDITION As String                  'RSV2機能設定
        Dim COMMON_BAITAIWRITE As String            '媒体書込用フォルダ(返還データ出力フォルダ)
        Dim RSV2_HENKAN_PRINT As String             '帳票個別指定
        Dim RSV2_HENKAN_PRINT_TXT As String         '帳票個別指定 外部ファイル
        Dim RSV2_HENKAN_PRINT_SORT_TXT As String    '帳票個別指定 外部ファイル（帳票ソート用）
        Dim PRNT_KIN_NAME As String                 '金庫名
        Dim ATENA_UMU As String                     '処理結果確認表の郵送先・金庫名出力有無
        Dim HENKAN_WRITE_CHKTYPE As String          '返還時媒体検証種別
        Dim RSV2_USE_DENSO As String                '伝送システム利用区分

    End Structure
    Private strcINI_PARAM As INI_PARAM

    ' 各ヘッダごとの依頼データ情報
    Protected mInfoComm As CAstBatch.CommData
    Public Property ToriData() As CAstBatch.CommData
        Get
            Return mInfoComm
        End Get
        Set(ByVal Value As CAstBatch.CommData)
            mInfoComm = Value
        End Set
    End Property

    Friend Structure TORI_PARAM
        Dim FMT_KBN As String               'フォーマット区分
        Dim ITAKU_CODE As String            '委託者コード
        Dim CODE_KBN As String              'コード区分
        Dim BAITAI_CODE As String           '媒体コード
        Dim FILE_NAME As String             '返還ファイル名
        Dim LABEL_KBN As String             'ラベル区分
        Dim MULTI_KBN As String             'マルチ区分
        Dim ITAKU_KANRI_CODE As String
        Dim KEKKA_HENKYAKU_KBN As String    '結果返却区分
        Dim PRTNUM As Integer
    End Structure
    Private strcTORI_PARAM As TORI_PARAM

    Public strCHECK_FILE_NAME As String     'チェックファイル

    Private strTORIF_CODE As String

    Private intRECORD_COUNT As Integer

    '合計
    Private dblFURI_KEN As Double
    Private dblFURI_KIN As Double
    Private dblFUNOU_KEN As Double
    Private dblFUNOU_KIN As Double
    Private dblALL_KEN As Double
    Private dblALL_KIN As Double
    Private dblALL_FURI_KEN As Double
    Private dblALL_FURI_KIN As Double
    Private dblALL_FUNOU_KEN As Double
    Private dblALL_FUNOU_KIN As Double

    '国税用
    Dim dblSIT_TOTAL_FURI_KEN As Double
    Dim dblSIT_TOTAL_FURI_KIN As Double
    Dim dblSIT_TOTAL_FUNOU_KEN As Double
    Dim dblSIT_TOTAL_FUNOU_KIN As Double
    Dim dblZEIMUSYO_TOTAL_FURI_KEN As Double
    Dim dblZEIMUSYO_TOTAL_FURI_KIN As Double
    Dim dblZEIMUSYO_TOTAL_FUNOU_KEN As Double
    Dim dblZEIMUSYO_TOTAL_FUNOU_KIN As Double

    'フォーマット
    Private intREC_LENGTH As Integer
    Private intBLK_SIZE As Integer

    'ファイル名
    Private strIN_FILE_NAME As String
    Private strOUT_FILE_NAME As String
    Private strOUT_END_FILE_NAME As String  '2012/06/30 標準版　WEB連携対応

    'FTRAN+定義ファイル 
    Public gstrP_FILE As String
    Public strIBMP_FILE As String

    Structure gstrFURI_DATA_118
        <VBFixedString(118)> Public strDATA As String
    End Structure
    Public gstrDATA_118 As gstrFURI_DATA_118

    Structure gstrFURI_DATA_119
        <VBFixedString(119)> Public strDATA As String
    End Structure
    Public gstrDATA_119 As gstrFURI_DATA_119

    Structure gstrFURI_DATA_120
        <VBFixedString(120)> Public strDATA As String
    End Structure
    Public gstrDATA_120 As gstrFURI_DATA_120

    Structure gstrFURI_DATA_130
        <VBFixedString(130)> Public strDATA As String
    End Structure
    Public gstrDATA_130 As gstrFURI_DATA_130

    Structure gstrFURI_DATA_220
        <VBFixedString(220)> Public strDATA As String
    End Structure
    Public gstrDATA_220 As gstrFURI_DATA_220

    Structure gstrFURI_DATA_390
        <VBFixedString(390)> Public strDATA As String
    End Structure
    Public gstrDATA_390 As gstrFURI_DATA_390

    '全銀
    Public gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1
    Public gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2
    Public gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8
    '国税
    Public gKOKUZEI_REC1 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD1
    Public gKOKUZEI_REC2 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD2
    Public gKOKUZEI_REC3 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD3
    Public gKOKUZEI_REC8 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD8
    'ＮＨＫ
    Public gNHK_REC1 As CAstFormat.CFormatNHK.NHKRECORD1
    Public gNHK_REC2 As CAstFormat.CFormatNHK.NHKRECORD2
    Public gNHK_REC8 As CAstFormat.CFormatNHK.NHKRECORD8
    Public gNHK_REC9 As CAstFormat.CFormatNHK.NHKRECORD9

    ' XMLファイル名
    Private mXmlFile As String
    ' XMLフォーマットrootオブジェクト
    Private mXmlRoot As XmlElement

    ' 起動パラメータ 共通情報
    Private mArgumentData As CommData

    ' 依頼データファイル名
    Private mDataFileName As String

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle
    Private SubDB As CASTCommon.MyOracle

    '***返還データ再作成されたものだけを処理対象とする 
    ' 返還対象フラグ        
    Private HenkanFlag As String = "0"      ' 再作成時は，"5"になる

    Private CENTER_MOTIKOMI As String = CASTCommon.GetFSKJIni("JIFURI", "CENTER_MOTIKOMI")

    Public Structure KeyInfo
        Dim TORIS_CODE As String            ' 取引先主コード
        Dim TORIF_CODE As String            ' 取引先副コード
        Dim DENSO_CNT_CODE As String        ' 伝送相手センター確認コード
        Dim TOHO_CNT_CODE As String         ' 伝送当方センター確認コード
        Dim ZENGIN_F_NAME As String         ' 全銀ファイル名
        Dim DENSO_F_NAME As String          ' 伝送ファイル名
        Dim FURI_DATE As String             ' 振替日
        Dim TORIMATOME_SIT_NO As String     ' 取りまとめ店
        Dim FUNOU_MEISAI_KBN As String      ' 不能結果明細表出力
        Dim KEKKA_MEISAI_KBN As String
        Dim CODE_KBN As String              ' コード区分
        Dim FMT_KBN As String               ' フォーマット区分
        Dim RENKEI_KBN As String            ' 連携区分
        Dim ENC_KBN_T As String             ' 暗号化区分（取引先マスタ）
        Dim ENC_KBN_S As String             ' 暗号化区分（スケジュールマスタ）
        Dim ATO_SYORI As String             ' 個別後処理
        Dim ITAKU_CODE As String            ' 委託者コード
        Dim ITAKU_KNAME As String           ' 委託者名
        Dim MULTI_KBN As String
        Dim SYORI_KEN As Double
        Dim SYORI_KIN As Double
        Dim FURI_KEN As Double
        Dim FURI_KIN As Double
        Dim FUNOU_KEN As Double
        Dim FUNOU_KIN As Double
        Dim MOTIKOMI_SEQ As String
        Dim ITAKU_KANRI_CODE As String

        Dim MESSAGE As String
        Dim ErrorDetail As String           ' エラー詳細
        ' 初期化
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            DENSO_CNT_CODE = ""
            TOHO_CNT_CODE = ""
            ZENGIN_F_NAME = ""
            DENSO_F_NAME = ""
            FURI_DATE = ""
            TORIMATOME_SIT_NO = ""
            FUNOU_MEISAI_KBN = ""
            CODE_KBN = ""
            FMT_KBN = ""
            RENKEI_KBN = ""
            ENC_KBN_S = ""
            ENC_KBN_T = ""
            ATO_SYORI = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            MULTI_KBN = ""
            SYORI_KEN = 0
            SYORI_KIN = 0
            FURI_KEN = 0
            FURI_KIN = 0
            FUNOU_KEN = 0
            FUNOU_KIN = 0
            ITAKU_KANRI_CODE = ""
            MESSAGE = ""
            ErrorDetail = ""
        End Sub

        ' キー比較
        '        戻り値
        '           2 つの比較対照値の構文上の関係を示す 32 ビット符号付き整数。
        '
        '       値 意味 
        '           0 より小さい値 strA が strB より小さい。 
        '           0 strA と strB は等しい。 
        '           0 より大きい値 strA が strB より大きい。
        Public Function Matching(ByVal compareKey As KeyInfo) As Integer
            Dim nRet As Integer

            ' 持込シーケンス
            nRet = String.Compare(MOTIKOMI_SEQ, compareKey.MOTIKOMI_SEQ)
            If Not nRet = 0 Then
                Return nRet
            End If

            Return nRet
        End Function

        ' DBからの値を設定
        Friend Sub SetOracleData(ByVal OraReader As CASTCommon.MyOracleReader)
            Dim GCOM As New MenteCommon.clsCommon

            FURI_DATE = GCOM.NzStr(OraReader.GetString("FURI_DATE_S")).PadRight(8)              '振替日
            TORIS_CODE = GCOM.NzStr(OraReader.GetString("TORIS_CODE_S")).PadRight(7)            '取引先主コード
            TORIF_CODE = GCOM.NzStr(OraReader.GetString("TORIF_CODE_S")).PadRight(2)            '取引先副コード
            ITAKU_KNAME = GCOM.NzStr(OraReader.GetString("ITAKU_KNAME_T"))
            ITAKU_CODE = GCOM.NzStr(OraReader.GetString("ITAKU_CODE_T"))
            SYORI_KEN = GCOM.NzLong(OraReader.GetString("SYORI_KEN_S"))
            SYORI_KIN = GCOM.NzLong(OraReader.GetString("SYORI_KIN_S"))
            FURI_KEN = GCOM.NzLong(OraReader.GetString("FURI_KEN_S"))
            FURI_KIN = GCOM.NzLong(OraReader.GetString("FURI_KIN_S"))
            FUNOU_KEN = GCOM.NzLong(OraReader.GetString("FUNOU_KEN_S"))
            FUNOU_KIN = GCOM.NzLong(OraReader.GetString("FUNOU_KIN_S"))
            FUNOU_MEISAI_KBN = GCOM.NzStr(OraReader.GetString("FUNOU_MEISAI_KBN_T"))
            KEKKA_MEISAI_KBN = GCOM.NzStr(OraReader.GetString("KEKKA_MEISAI_KBN_T"))

            CODE_KBN = GCOM.NzStr(OraReader.GetString("CODE_KBN_T"))
            FMT_KBN = GCOM.NzStr(OraReader.GetString("FMT_KBN_T"))
            MULTI_KBN = GCOM.NzStr(OraReader.GetString("MULTI_KBN_T"))
            ENC_KBN_T = GCOM.NzStr(OraReader.GetString("ENC_KBN_T"))

            ITAKU_KANRI_CODE = GCOM.NzStr(OraReader.GetString("ITAKU_KANRI_CODE_T"))
            MOTIKOMI_SEQ = GCOM.NzStr(OraReader.GetString("MOTIKOMI_SEQ_S"))
            MESSAGE = ""
            ErrorDetail = ""

        End Sub

    End Structure

    Private Class SitCompareClass
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
            Implements IComparer.Compare

            Dim kx As KeyInfo = DirectCast(x, KeyInfo)
            Dim ky As KeyInfo = DirectCast(y, KeyInfo)
            Dim ret As Integer = String.Compare(kx.TORIMATOME_SIT_NO, ky.TORIMATOME_SIT_NO)

            Select Case ret
                Case 0
                    Return String.Compare(kx.DENSO_F_NAME, ky.DENSO_F_NAME)
                Case Else
                    Return ret
            End Select
        End Function
    End Class
    Private SitCompare As New SitCompareClass

    ' New
    Public Sub New()
    End Sub

    ' 機能　 ： 返還データ作成処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Public Function Main(ByVal command As String) As Integer
        ' パラメータチェック

        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 600
        Dim sWorkTime As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWorkTime) Then
            LockWaitTime = CInt(sWorkTime)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If

        Try
            ' メイン引数設定
            strcKOBETUPARAM.Data = command

            ' ジョブ通番設定
            LOG.ToriCode = strcKOBETUPARAM.strTORIS_CODE & "-" & strcKOBETUPARAM.strTORIF_CODE
            LOG.FuriDate = strcKOBETUPARAM.strFURI_DATE
            LOG.JobTuuban = CInt(strcKOBETUPARAM.strJOB_TUUBAN)

        Catch ex As Exception
            LOG.Write("返還データ作成処理開始", "失敗", "パラメタ取得失敗[" & command & "]" & ex.Message)
            LOG.UpdateJOBMASTbyErr("パラメタ取得失敗[" & command & "]" & ex.Message)
            Return 1
        End Try

        Try
            'とりあえず一番頭で開ける
            MainDB = New CASTCommon.MyOracle
            SubDB = New CASTCommon.MyOracle

            'iniファイル取得
            Dim ErrMsg As String = ""
            If Not fn_GetIni(ErrMsg) Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "設定ファイル取得", "失敗", ErrMsg)
                LOG.UpdateJOBMASTbyErr(ErrMsg)
                Return 1
            End If

            '取引先情報取得
            If Not fn_GetToriParam(strcKOBETUPARAM, strcTORI_PARAM) Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ対象検索", "失敗", "スケジュールマスタ検索失敗")
                LOG.UpdateJOBMASTbyErr("スケジュールマスタ検索失敗")
                Return 1
            Else
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "スケジュールマスタ検索(終了)", "成功")
            End If

            Dim InfoParam As New CommData.stPARAMETER
            Dim blnRet As Boolean = False
            Dim blnPrnRet As Boolean
            Dim HenkyakuArray As New ArrayList(128)
            Dim UnmatchArray As New ArrayList(128)

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還ファイル名設定(開始)", "成功")

            '返還ファイル名
            If strcTORI_PARAM.FILE_NAME <> "" Then
                ' 2016/02/08 タスク）岩城 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                Select Case strcINI_PARAM.RSV2_EDITION
                    Case "2"
                        If strcTORI_PARAM.MULTI_KBN = "1" Then
                            strIN_FILE_NAME = Path.Combine(strcINI_PARAM.DAT_FOLDER,
                                                           "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & ".dat")
                        Else
                            strIN_FILE_NAME = Path.Combine(strcINI_PARAM.DAT_FOLDER,
                                                           "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & ".dat")
                        End If
                    Case Else
                        strIN_FILE_NAME = Path.Combine(strcINI_PARAM.DAT_FOLDER, strcTORI_PARAM.FILE_NAME)
                End Select
                'strIN_FILE_NAME = Path.Combine(strcINI_PARAM.DAT_FOLDER, strcTORI_PARAM.FILE_NAME)
                ' 2016/01/08 タスク）岩城 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
            Else
                If strcTORI_PARAM.MULTI_KBN = "1" Then
                    strIN_FILE_NAME = Path.Combine(strcINI_PARAM.DAT_FOLDER,
                                                   "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & ".dat")
                Else
                    strIN_FILE_NAME = Path.Combine(strcINI_PARAM.DAT_FOLDER,
                                                   "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & ".dat")
                End If
            End If

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還ファイル名設定(終了)", "成功", "返還ファイル名：" & strIN_FILE_NAME)

            '返還ファイルが存在していたら削除
            If File.Exists(strIN_FILE_NAME) = True Then
                Kill(strIN_FILE_NAME)
            End If

            InfoParam.TORIS_CODE = strcKOBETUPARAM.strTORIS_CODE        '取引先主コード
            InfoParam.TORIF_CODE = strcKOBETUPARAM.strTORIF_CODE        '取引先副コード
            InfoParam.BAITAI_CODE = ""
            InfoParam.FMT_KBN = strcTORI_PARAM.FMT_KBN                  'フォーマット区分
            InfoParam.FURI_DATE = strcKOBETUPARAM.strFURI_DATE          '振替日
            InfoParam.CODE_KBN = ""
            InfoParam.FSYORI_KBN = "1"                                  '処理区分
            InfoParam.JOBTUUBAN = CInt(strcKOBETUPARAM.strJOB_TUUBAN)   'ジョブ通番
            InfoParam.ENC_KBN = ""
            InfoParam.ENC_KEY1 = ""
            InfoParam.ENC_KEY2 = ""
            InfoParam.ENC_OPT1 = ""
            InfoParam.CYCLENO = ""
            'mArgumentData.INFOParameter = InfoParam

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成(開始)", "成功", "フォーマット区分：" & strcTORI_PARAM.FMT_KBN)

            ' ジョブ実行ロックの位置変更
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                LOG.Write_Err("主処理", "失敗", "返還データ作成処理で実行待ちタイムアウト")
                LOG.UpdateJOBMASTbyErr("返還データ作成処理で実行待ちタイムアウト")
                Return -1
            End If

            '返還データ作成
            Select Case strcTORI_PARAM.FMT_KBN
                '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                Case "00", "20", "21"
                    'Case "00"
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    '全銀フォーマット(120バイト)
                    blnRet = MakeHenkanData_ZENGIN(HenkyakuArray,
                                                   UnmatchArray)
                Case "01"
                    'NHKフォーマット(120バイト)
                    blnRet = MakeHenkanData_NHK(HenkyakuArray,
                                                UnmatchArray)
                Case "02"
                    '国税フォーマット(390バイト)
                    dblSIT_TOTAL_FURI_KEN = 0
                    dblSIT_TOTAL_FURI_KEN = 0
                    dblSIT_TOTAL_FUNOU_KEN = 0
                    dblSIT_TOTAL_FUNOU_KIN = 0
                    dblZEIMUSYO_TOTAL_FURI_KEN = 0
                    dblZEIMUSYO_TOTAL_FURI_KIN = 0
                    dblZEIMUSYO_TOTAL_FUNOU_KEN = 0
                    dblZEIMUSYO_TOTAL_FUNOU_KIN = 0

                    blnRet = MakeHenkanData_KOKUZEI(HenkyakuArray,
                                                    UnmatchArray)

                Case "03"
                    '年金フォーマット(130バイト)
                    blnRet = True

                    LOG.UpdateJOBMASTbyOK(Space(1))
                    Exit Function

                Case "04", "05"
                    '依頼書フォーマット

                    intRECORD_COUNT = 0
                    dblALL_KEN = 0
                    dblALL_KIN = 0
                    dblALL_FURI_KEN = 0
                    dblALL_FURI_KIN = 0
                    dblALL_FUNOU_KEN = 0
                    dblALL_FUNOU_KIN = 0

                    blnRet = MakeHenkanData_IRAISYO(HenkyakuArray)
                Case "06"
                    '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                Case Else
                    If IsNumeric(strcTORI_PARAM.FMT_KBN) Then
                        Dim nFmtKbn As Integer = CInt(strcTORI_PARAM.FMT_KBN)
                        'フォーマット区分が50～99の場合
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            blnRet = MakeHenkanData_XML(HenkyakuArray, UnmatchArray)
                            If blnRet = False Then
                                ' ロールバック
                                MainDB.Rollback()
                                Return 1
                            End If
                        End If
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
            End Select

            If UnmatchArray.Count > 0 Then
                Dim UnmatchKey As KeyInfo = DirectCast(UnmatchArray.Item(UnmatchArray.Count - 1), KeyInfo)

                LOG.UpdateJOBMASTbyErr(UnmatchKey.MESSAGE)
                If Not MainDB Is Nothing Then
                    MainDB.Rollback()
                End If

                LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成(終了)", "失敗", UnmatchKey.MESSAGE)

                Return 1
            End If

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成(終了)", "成功", "フォーマット区分：" & strcTORI_PARAM.FMT_KBN)

            Select Case strcTORI_PARAM.FMT_KBN
                '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                Case "00", "20", "21"
                    'Case "00"
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    Select Case strcTORI_PARAM.CODE_KBN

                        Case "0"
                            intREC_LENGTH = 120
                            intBLK_SIZE = 1800
                            gstrP_FILE = "120JIS→JIS.P"        'JIS→JIS改
                            strIBMP_FILE = "120JIS→JIS.P"
                        Case "1"
                            intREC_LENGTH = 120
                            intBLK_SIZE = 1800
                            gstrP_FILE = "120JIS→JIS改.P"      'JIS→JIS改
                            strIBMP_FILE = "120JIS→JIS改.P"
                        Case "2"
                            intREC_LENGTH = 119
                            intBLK_SIZE = 1800
                            gstrP_FILE = "119JIS→JIS改.P"
                            strIBMP_FILE = "119JIS→JIS改.P"
                        Case "3"
                            intREC_LENGTH = 118
                            intBLK_SIZE = 1800
                            gstrP_FILE = "118JIS→JIS改.P"
                            strIBMP_FILE = "118JIS→JIS改.P"
                        Case "4"
                            intREC_LENGTH = 120
                            intBLK_SIZE = 1800

                            gstrP_FILE = "120.P"                'JIS→EBCDIC
                            strIBMP_FILE = "120IBM.P"
                    End Select
                Case "01"   'NHK
                    Select Case strcTORI_PARAM.CODE_KBN
                        Case "0"
                            intREC_LENGTH = 120
                            intBLK_SIZE = 1800
                            gstrP_FILE = "120JIS→JIS.P"        'JIS→JIS改
                            strIBMP_FILE = "120JIS→JIS.P"
                        Case "1"
                            intREC_LENGTH = 120
                            intBLK_SIZE = 1800
                            gstrP_FILE = "120JIS→JIS改.P"      'JIS→JIS改
                            strIBMP_FILE = "120JIS→JIS改.P"
                        Case "2"
                        Case "4"
                            intREC_LENGTH = 120
                            intBLK_SIZE = 1800
                            gstrP_FILE = "120.P"                'JIS→EBCDIC
                            strIBMP_FILE = "120IBM.P"
                    End Select
                Case "02"
                    intREC_LENGTH = 390
                    intBLK_SIZE = 3900
                    gstrP_FILE = "390.P"
                    Select Case strcTORI_PARAM.CODE_KBN
                        Case "0"
                            gstrP_FILE = "390JIS→JIS.P"        'JIS→JIS改
                            strIBMP_FILE = "390JIS→JIS.P"
                        Case "1"
                            gstrP_FILE = "390JIS→JIS改.P"      'JIS→JIS改
                            strIBMP_FILE = "390JIS→JIS改.P"
                        Case "4"
                            gstrP_FILE = "390.P"                'JIS→EBCDIC
                            strIBMP_FILE = "390IBM.P"
                    End Select
                Case Else
                    If IsNumeric(strcTORI_PARAM.FMT_KBN) Then
                        Dim nFmtKbn As Integer = CInt(strcTORI_PARAM.FMT_KBN)
                        'フォーマット区分が50～99の場合
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            Dim node As XmlNode
                            Dim attribute As XmlAttribute
                            Dim sWork As String

                            ' ブロックサイズ
                            node = mXmlRoot.SelectSingleNode("共通/CMTブロックサイズ")
                            If node Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「共通/CMTブロックサイズ」タグが定義されていません。")
                            End If
                            sWork = node.InnerText.Trim
                            If sWork <> "" Then
                                If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                                    Throw New Exception(mXmlFile & "定義エラー：" & "「共通/CMTブロックサイズ」タグの値（" & sWork & "）が不当です。（" &
                                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                                End If
                                intBLK_SIZE = CInt(sWork)
                            Else
                                intBLK_SIZE = 0
                            End If

                            ' 返還ファイルを伝送ファイルにコピーする際のレコード長
                            node = mXmlRoot.SelectSingleNode("返還/コピー設定一覧/コピー設定[@コード区分='" & strcTORI_PARAM.CODE_KBN & "']")
                            If node Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定[@コード区分='" & strcTORI_PARAM.CODE_KBN & "']」タグが定義されていません。")
                            End If

                            attribute = node.Attributes.ItemOf("データ長")
                            If attribute Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性が定義されていません。（" &
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            sWork = attribute.Value.Trim
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性の値（" & sWork & "）が不当です。（" &
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If

                            intREC_LENGTH = CInt(sWork)

                            ' 返還ファイルを伝送ファイルにコピーする際のパラメータファイル名
                            attribute = node.Attributes.ItemOf("パラメータファイル")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「パラメータファイル」属性が定義されていません。（" &
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            gstrP_FILE = attribute.Value.Trim

                            ' 返還ファイルをＦＤ３．５にコピーする際のパラメータファイル名
                            attribute = node.Attributes.ItemOf("IBMパラメータファイル")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「IBMパラメータファイル」属性が定義されていません。（" &
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            strIBMP_FILE = attribute.Value.Trim
                        End If
                    End If
            End Select

            '2010/01/12 結果返却区分が帳票のみの場合は媒体書き込みを行わないように修正
            If strcTORI_PARAM.KEKKA_HENKYAKU_KBN <> "3" Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体１本目の作成(開始)", "成功")

                If fn_BAITAI_WRITE() = False Then
                    If Not MainDB Is Nothing Then
                        MainDB.Rollback()
                    End If
                    Exit Function
                End If
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体１本目の作成(終了)", "成功")

                If strcINI_PARAM.HENKAN_WRITE_CHKTYPE = "1" Then
                    '--------------------------------------------
                    '媒体１本目書込み後検証
                    '--------------------------------------------
                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体１本目書込み後検証(開始)", "成功")

                    Select Case strcTORI_PARAM.BAITAI_CODE
                        Case "01", "05", "06", "11", "12", "13", "14", "15"
                            If strcTORI_PARAM.BAITAI_CODE = "05" And strcINI_PARAM.MT_FLG = "1" Then
                                Exit Select
                            ElseIf strcTORI_PARAM.BAITAI_CODE = "06" And strcINI_PARAM.CMT_FLG = "1" Then
                                Exit Select
                            End If
                            MessageBox.Show(MSG0062I,
                                            msgTitle,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information,
                                            MessageBoxDefaultButton.Button1,
                                            MessageBoxOptions.DefaultDesktopOnly)
                            If fn_BAITAI_WRITE_CHK() = False Then
                                LOG.UpdateJOBMASTbyErr("検証不一致")
                                If Not MainDB Is Nothing Then
                                    MainDB.Rollback()
                                End If
                                Exit Function
                            End If
                    End Select

                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体１本目書込み後検証(終了)", "成功")

                    '--------------------------------------------
                    '媒体２本目の作成
                    '--------------------------------------------
                    If strcTORI_PARAM.BAITAI_CODE = "05" And strcINI_PARAM.MT_FLG = "1" Then
                        '媒体=MTかつ外付け
                        '何もしない
                    ElseIf strcTORI_PARAM.BAITAI_CODE = "06" And strcINI_PARAM.CMT_FLG = "1" Then
                        '媒体=CMTかつ外付け
                        '何もしない
                    Else
                        'それ以外の媒体
                        Select Case strcTORI_PARAM.BAITAI_CODE
                            Case "01", "05", "06", "11", "12", "13", "14", "15"
                                Select Case strcINI_PARAM.RSV2_EDITION
                                    Case "2"
                                        ' NOP
                                    Case Else

                                        If MessageBox.Show(MSG0061I,
                                                           msgTitle,
                                                           MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Information,
                                                           MessageBoxDefaultButton.Button1,
                                                           MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Yes Then

                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目の作成(開始)", "成功")

                                            If fn_BAITAI_WRITE() = False Then
                                                If Not MainDB Is Nothing Then
                                                    MainDB.Rollback()
                                                End If
                                                Exit Function
                                            End If

                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目の作成(終了)", "成功")

                                            '--------------------------------------------
                                            '媒体２本目書込み後検証
                                            '--------------------------------------------
                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目書込み後検証(開始)", "成功")

                                            MessageBox.Show(MSG0062I,
                                                           msgTitle,
                                                           MessageBoxButtons.OK,
                                                           MessageBoxIcon.Information,
                                                           MessageBoxDefaultButton.Button1,
                                                           MessageBoxOptions.DefaultDesktopOnly)
                                            If fn_BAITAI_WRITE_CHK() = False Then
                                                LOG.UpdateJOBMASTbyErr("検証不一致")
                                                If Not MainDB Is Nothing Then
                                                    MainDB.Rollback()
                                                End If
                                                Exit Function
                                            End If
                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目書込み後検証(終了)", "成功")
                                        Else
                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目の作成", "キャンセル")
                                        End If
                                End Select
                        End Select
                    End If
                Else

                    If strcTORI_PARAM.BAITAI_CODE = "05" And strcINI_PARAM.MT_FLG = "1" Then
                        '媒体=MTかつ外付け
                        '何もしない
                    ElseIf strcTORI_PARAM.BAITAI_CODE = "06" And strcINI_PARAM.CMT_FLG = "1" Then
                        '媒体=CMTかつ外付け
                        '何もしない
                    Else
                        'それ以外の媒体
                        Select Case strcTORI_PARAM.BAITAI_CODE
                            Case "01", "05", "06", "11", "12", "13", "14", "15"
                                Select Case strcINI_PARAM.RSV2_EDITION
                                    Case "2"
                                        ' NOP
                                    Case Else
                                        '--------------------------------------------
                                        '媒体２本目の作成
                                        '--------------------------------------------
                                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目の作成(開始)", "成功")

                                        If MessageBox.Show(MSG0061I,
                                                           msgTitle,
                                                           MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Information,
                                                           MessageBoxDefaultButton.Button1,
                                                           MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Yes Then
                                            If fn_BAITAI_WRITE() = False Then
                                                If Not MainDB Is Nothing Then
                                                    MainDB.Rollback()
                                                End If
                                                Exit Function
                                            End If
                                        End If
                                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目の作成(終了)", "成功")

                                        '--------------------------------------------
                                        '媒体２本目書込み後検証
                                        '--------------------------------------------
                                        If strcINI_PARAM.HENKAN_WRITE_CHKTYPE = "1" Then
                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目書込み後検証(開始)", "成功")
                                        Else
                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証(開始)", "成功")
                                        End If

                                        MessageBox.Show(MSG0062I,
                                                       msgTitle,
                                                       MessageBoxButtons.OK,
                                                       MessageBoxIcon.Information,
                                                       MessageBoxDefaultButton.Button1,
                                                       MessageBoxOptions.DefaultDesktopOnly)
                                        If fn_BAITAI_WRITE_CHK() = False Then
                                            LOG.UpdateJOBMASTbyErr("検証不一致")
                                            If Not MainDB Is Nothing Then
                                                MainDB.Rollback()
                                            End If
                                            Exit Function
                                        End If
                                        If strcINI_PARAM.HENKAN_WRITE_CHKTYPE = "1" Then
                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体２本目書込み後検証(終了)", "成功")
                                        Else
                                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証(終了)", "成功")
                                        End If
                                End Select
                        End Select
                    End If
                End If
            End If

            If blnRet = True AndAlso Not HenkyakuArray Is Nothing Then

                ' 帳票出力
                LOG.Write("帳票出力開始", "成功")

                Dim OldKey As New KeyInfo
                Call OldKey.Init()
                For i As Integer = 0 To HenkyakuArray.Count - 1
                    Dim Key As KeyInfo = DirectCast(HenkyakuArray.Item(i), KeyInfo)

                    LOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                    LOG.FuriDate = Key.FURI_DATE

                    If Key.Matching(OldKey) <> 0 AndAlso i = 0 Then

                        Dim strSYORI_KEKKA_PRINT As String
                        strSYORI_KEKKA_PRINT = CASTCommon.GetFSKJIni("HENKAN", "SYORI_KEKKA_PRINT")
                        If strSYORI_KEKKA_PRINT = "1" Then

                            '処理結果確認表
                            blnPrnRet = PrintSyoriKekkaKakuninhyo(Key)
                        End If
                    End If

                    '2010/01/12 結果返却区分が媒体のみの場合は帳票を行わないように修正
                    'If Key.MESSAGE.Trim = "" Then
                    If Key.MESSAGE.Trim = "" AndAlso strcTORI_PARAM.KEKKA_HENKYAKU_KBN <> "2" Then
                        ' エラーがなければ，帳票を出力

                        '======================================================================================================
                        ' 【返還処理】返却帳票印刷
                        '    INIファイル[RSV2_V1.0.0]-[HENKAN_PRINT] の指定により「標準帳票印刷」または「特殊パターン印刷」か
                        '    切り分けを行う。
                        '     [HENKAN_PRINT] 1(標準帳票印刷) : 振替結果、不能明細、不能集計、店別集計をマスタ設定情報で印刷
                        '                    2(特殊ﾊﾟﾀｰﾝ印刷): 外部テキストファイルに記載した内容で印刷
                        '======================================================================================================
                        Select Case strcINI_PARAM.RSV2_HENKAN_PRINT
                            Case "1"
                                '======================================================================================================
                                ' 【不能結果明細表出力区分】　※標準設定内容
                                '------------------------------------------------------------------------------------------------------
                                '   1:振替明細 ＋ 店別集計 ＋ 不能集計      
                                '   2:不能明細 ＋ 店別集計 ＋ 不能集計
                                '   3:振替明細 ＋ 店別集計
                                '   4:不能明細 ＋ 店別集計
                                '   5:店別集計 ＋ 不能集計
                                '   6:店別集計
                                '   7:振替明細表
                                '   8:不能明細表
                                '   9:不能集計表
                                '   0:未出力
                                '======================================================================================================

                                ' レポエージェント印刷
                                Dim ExeRepo As New CAstReports.ClsExecute

                                '======================================================================================================
                                ' 【振替結果明細表】印刷
                                '   <パラメータ> [取引先コード],[振替日],[帳票種類],[空白](,[WEB伝送フラグ])
                                '                ※帳票種類  　 ：0(結果明細)
                                '                  WEB伝送フラグ：1(媒体がWEB伝送の場合にパラメータ追加
                                '======================================================================================================
                                Select Case Key.FUNOU_MEISAI_KBN
                                    Case "1", "3", "7"
                                        '---------------------------------------------------------
                                        ' パラメータ設定
                                        '---------------------------------------------------------
                                        Dim Param As String = Key.TORIS_CODE & Key.TORIF_CODE & "," & Key.FURI_DATE & ",0"
                                        Dim DQ As String = """"
                                        Param &= "," & DQ & DQ

                                        '---------------------------------------------------------
                                        ' WEB伝送時パラメータ
                                        '---------------------------------------------------------
                                        If strcTORI_PARAM.BAITAI_CODE = "10" Then
                                            Param &= ",1"
                                        End If

                                        LOG.Write("振替結果明細表出力開始", "成功", Param)

                                        '---------------------------------------------------------
                                        ' 印刷処理(部数分繰返)
                                        '---------------------------------------------------------
                                        Dim nRet As Integer = 0
                                        If strcTORI_PARAM.PRTNUM = 0 Then
                                            strcTORI_PARAM.PRTNUM = 1
                                        End If
                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                            nRet = ExeRepo.ExecReport("KFJP018.EXE", Param)
                                            If nRet <> 0 Then
                                                Exit For
                                            End If
                                        Next

                                        If nRet = 0 Then
                                            LOG.Write("振替結果明細表出力", "成功")
                                        Else
                                            LOG.Write("振替結果明細表出力", "失敗")

                                            Key.MESSAGE = "振替結果明細表出力 失敗"
                                            HenkyakuArray.Item(i) = Key
                                        End If
                                End Select

                                '======================================================================================================
                                ' 【振替不能明細表】印刷
                                '   <パラメータ> [取引先コード],[振替日],[帳票種類],[空白](,[WEB伝送フラグ])
                                '                ※帳票種類  　 ：1(不能明細)
                                '                  WEB伝送フラグ：1(媒体がWEB伝送の場合にパラメータ追加
                                '======================================================================================================
                                Select Case Key.FUNOU_MEISAI_KBN
                                    Case "2", "4", "8"
                                        '---------------------------------------------------------
                                        ' パラメータ設定
                                        '---------------------------------------------------------
                                        Dim Param As String = Key.TORIS_CODE & Key.TORIF_CODE & "," & Key.FURI_DATE & ",1"
                                        Dim DQ As String = """"
                                        Param &= "," & DQ & DQ

                                        '---------------------------------------------------------
                                        ' WEB伝送時パラメータ
                                        '---------------------------------------------------------
                                        If strcTORI_PARAM.BAITAI_CODE = "10" Then
                                            Param &= ",1"   '返還処理からﾊﾟﾗﾒﾀを発行したことをﾊﾟﾗﾒﾀ数で区分けする
                                        End If

                                        LOG.Write("振替不能明細表出力開始", "成功", Param)

                                        '---------------------------------------------------------
                                        ' 印刷処理(部数分繰返)
                                        '---------------------------------------------------------
                                        Dim nRet As Integer = 0
                                        If strcTORI_PARAM.PRTNUM = 0 Then
                                            strcTORI_PARAM.PRTNUM = 1
                                        End If
                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                            nRet = ExeRepo.ExecReport("KFJP017.EXE", Param)
                                            If nRet <> 0 Then
                                                Exit For
                                            End If
                                        Next

                                        If nRet = 0 Then
                                            LOG.Write("振替不能明細表出力", "成功")
                                        Else
                                            LOG.Write("振替不能明細表出力", "失敗")
                                        End If

                                End Select

                                '======================================================================================================
                                ' 【振替不能事由別集計表】印刷
                                '   <パラメータ> [取引先コード],[振替日],[空白](,[WEB伝送フラグ])
                                '                ※WEB伝送フラグ：1(媒体がWEB伝送の場合にパラメータ追加
                                '======================================================================================================
                                Select Case Key.FUNOU_MEISAI_KBN
                                    Case "1", "2", "5", "9"
                                        '---------------------------------------------------------
                                        ' パラメータ設定
                                        '---------------------------------------------------------
                                        Dim Param As String = Key.TORIS_CODE & Key.TORIF_CODE & "," & Key.FURI_DATE
                                        Dim DQ As String = """"
                                        Param &= "," & DQ & DQ

                                        '---------------------------------------------------------
                                        ' WEB伝送時パラメータ
                                        '---------------------------------------------------------
                                        If strcTORI_PARAM.BAITAI_CODE = "10" Then
                                            Param &= ",1"   '返還処理からﾊﾟﾗﾒﾀを発行したことをﾊﾟﾗﾒﾀ数で区分けする
                                        End If

                                        LOG.Write("振替不能事由別集計表出力開始", "成功", Param)

                                        '---------------------------------------------------------
                                        ' 印刷処理(部数分繰返)
                                        '---------------------------------------------------------
                                        Dim nRet As Integer = 0
                                        If strcTORI_PARAM.PRTNUM = 0 Then
                                            strcTORI_PARAM.PRTNUM = 1
                                        End If
                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                            nRet = ExeRepo.ExecReport("KFJP052.EXE", Param)
                                            If nRet <> 0 Then
                                                Exit For
                                            End If
                                        Next

                                        If nRet = 0 Then
                                            LOG.Write("振替不能事由別集計表出力", "成功")
                                        Else
                                            LOG.Write("振替不能事由別集計表出力", "失敗")
                                        End If
                                End Select

                                '======================================================================================================
                                ' 【口座振替店別集計表】印刷
                                '   <パラメータ> [取引先コード],[振替日],[返還フラグ],[空白](,[WEB伝送フラグ])
                                '                ※返還フラグ　 ：0(落込処理時)、1(返還処理時)
                                '                  WEB伝送フラグ：1(媒体がWEB伝送の場合にパラメータ追加
                                '======================================================================================================
                                Select Case Key.FUNOU_MEISAI_KBN
                                    Case "1", "2", "3", "4", "5", "6"
                                        '---------------------------------------------------------
                                        ' パラメータ設定
                                        '---------------------------------------------------------
                                        Dim Param As String = Key.TORIS_CODE & Key.TORIF_CODE & "," & Key.FURI_DATE & ",1"
                                        Dim DQ As String = """"
                                        Param &= "," & DQ & DQ

                                        '---------------------------------------------------------
                                        ' WEB伝送時パラメータ
                                        '---------------------------------------------------------
                                        If strcTORI_PARAM.BAITAI_CODE = "10" Then
                                            Param &= ",1"   '返還処理からﾊﾟﾗﾒﾀを発行したことをﾊﾟﾗﾒﾀ数で区分けする
                                        End If

                                        LOG.Write("口座振替店別集計表出力開始", "成功", Param)

                                        '---------------------------------------------------------
                                        ' 印刷処理(部数分繰返)
                                        '---------------------------------------------------------
                                        Dim nRet As Integer = 0
                                        If strcTORI_PARAM.PRTNUM = 0 Then
                                            strcTORI_PARAM.PRTNUM = 1
                                        End If
                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                            nRet = ExeRepo.ExecReport("KFJP019.EXE", Param)
                                            If nRet <> 0 Then
                                                Exit For
                                            End If
                                        Next

                                        If nRet = 0 Then
                                            LOG.Write("口座振替店別集計表出力", "成功")
                                        Else
                                            LOG.Write("口座振替店別集計表出力", "失敗")
                                            Key.MESSAGE = "口座振替店別集計表出力 失敗"
                                            HenkyakuArray.Item(i) = Key
                                        End If
                                End Select
                            Case "2"
                                '======================================================================================================
                                ' 【特殊パターン印刷】
                                '======================================================================================================
                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷", "開始")
                                Dim ExtendPrint As New CAstExtendPrint.CExtendPrint
                                Dim nRet As Integer = 0

                                Dim ExternalPrintArray As New ArrayList
                                Dim External_Param As String() = Nothing
                                Dim Param_prtID As String = ""
                                Dim Param_prtName As String = ""
                                Dim Param_replaceArray As String() = Nothing
                                Dim Param_printerArray As String() = Nothing
                                Dim Param_dllName As String = ""
                                Dim Param_className As String = ""
                                Dim Param_methodName As String = ""

                                ExternalPrintArray.Clear()
                                GetTextFileInfo(strcINI_PARAM.RSV2_HENKAN_PRINT_TXT, ExternalPrintArray)

                                '---------------------------------------------------------
                                ' 印刷処理(部数分繰返)
                                '---------------------------------------------------------
                                If strcTORI_PARAM.PRTNUM = 0 Then
                                    strcTORI_PARAM.PRTNUM = 1
                                End If

                                For TextCount As Integer = 0 To ExternalPrintArray.Count - 1 Step 1
                                    External_Param = ExternalPrintArray(TextCount).ToString.Split(CChar(","))
                                    Param_prtID = External_Param(2)
                                    Param_prtName = External_Param(3)

                                    If Key.FUNOU_MEISAI_KBN = External_Param(0) Then
                                        nRet = 0

                                        Select Case External_Param(1)
                                            Case "EXE"
                                                '---------------------------------------------------------
                                                ' パラメータ構築
                                                '---------------------------------------------------------
                                                ReDim Param_replaceArray(0)
                                                Param_replaceArray(0) = ""
                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replaceArray(0)) = False Then
                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(EXE)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                    Key.MESSAGE = "特殊パターン(EXE)印刷(パラメータ構築) 失敗"
                                                    HenkyakuArray.Item(i) = Key
                                                End If

                                                Param_dllName = External_Param(4)
                                                Param_className = External_Param(5)
                                                Param_methodName = External_Param(6)

                                                '---------------------------------------------------------
                                                ' 拡張印刷実行（印刷対象チェック後）
                                                '---------------------------------------------------------
                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                    For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                    Next
                                                End If
                                            Case "DLL"
                                                '---------------------------------------------------------
                                                ' パラメータ構築
                                                '---------------------------------------------------------
                                                Dim Param_replace As String = ""
                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                    Key.MESSAGE = "特殊パターン(DLL)印刷(パラメータ構築) 失敗"
                                                    HenkyakuArray.Item(i) = Key
                                                End If
                                                Param_replaceArray = Param_replace.Split(CChar(","))

                                                '---------------------------------------------------------
                                                ' 印刷情報取得
                                                '---------------------------------------------------------
                                                If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                    Key.MESSAGE = "特殊パターン(DLL)印刷(印刷情報取得) 失敗"
                                                    HenkyakuArray.Item(i) = Key
                                                End If

                                                '---------------------------------------------------------
                                                ' 拡張印刷実行（印刷対象チェック後）
                                                '---------------------------------------------------------
                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                    For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                    Next
                                                End If
                                            Case "PRT"
                                                '---------------------------------------------------------
                                                ' パラメータ構築
                                                '---------------------------------------------------------
                                                Dim Param_replace As String = ""
                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                    Key.MESSAGE = "特殊パターン(PRT)印刷(パラメータ構築) 失敗"
                                                    HenkyakuArray.Item(i) = Key
                                                End If
                                                Param_replaceArray = Param_replace.Split(CChar(","))

                                                '---------------------------------------------------------
                                                ' 印刷情報取得
                                                '---------------------------------------------------------
                                                If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                    Key.MESSAGE = "特殊パターン(PRT)印刷(印刷情報取得) 失敗"
                                                    HenkyakuArray.Item(i) = Key
                                                End If

                                                '---------------------------------------------------------
                                                ' 拡張印刷実行（印刷対象チェック後）
                                                '---------------------------------------------------------
                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                    For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                    Next
                                                End If
                                            Case Else
                                                '---------------------------------------------------------
                                                ' グループ単位帳票
                                                '---------------------------------------------------------
                                                If Key.Matching(OldKey) <> 0 AndAlso i = 0 Then
                                                    Select Case External_Param(1).Substring(0, 3)
                                                        Case "EXE"
                                                            '---------------------------------------------------------
                                                            ' パラメータ構築
                                                            '---------------------------------------------------------
                                                            ReDim Param_replaceArray(0)
                                                            Param_replaceArray(0) = ""
                                                            If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replaceArray(0)) = False Then
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(EXE)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                Key.MESSAGE = "特殊パターン(EXE)印刷(パラメータ構築) 失敗"
                                                                HenkyakuArray.Item(i) = Key
                                                            End If

                                                            Param_dllName = External_Param(4)
                                                            Param_className = External_Param(5)
                                                            Param_methodName = External_Param(6)

                                                            '---------------------------------------------------------
                                                            ' 拡張印刷実行（印刷対象チェック後）
                                                            '---------------------------------------------------------
                                                            If PrintExeCheck(External_Param(8), Key) = True Then
                                                                Select Case External_Param(1).Substring(3, 1).Trim
                                                                    Case "1", "2", "3", "4", "5", "6", "7", "8", "9"
                                                                        For intPrintNumber As Integer = 0 To CInt(External_Param(1).Substring(3, 1).Trim) - 1
                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                        Next
                                                                    Case Else
                                                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                        Next
                                                                End Select
                                                            End If
                                                        Case "DLL"
                                                            '---------------------------------------------------------
                                                            ' パラメータ構築
                                                            '---------------------------------------------------------
                                                            Dim Param_replace As String = ""
                                                            If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                Key.MESSAGE = "特殊パターン(DLL)印刷(パラメータ構築) 失敗"
                                                                HenkyakuArray.Item(i) = Key
                                                            End If
                                                            Param_replaceArray = Param_replace.Split(CChar(","))

                                                            '---------------------------------------------------------
                                                            ' 印刷情報取得
                                                            '---------------------------------------------------------
                                                            If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                Key.MESSAGE = "特殊パターン(DLL)印刷(印刷情報取得) 失敗"
                                                                HenkyakuArray.Item(i) = Key
                                                            End If

                                                            '---------------------------------------------------------
                                                            ' 拡張印刷実行（印刷対象チェック後）
                                                            '---------------------------------------------------------
                                                            If PrintExeCheck(External_Param(8), Key) = True Then
                                                                Select Case External_Param(1).Substring(3, 1).Trim
                                                                    Case "1", "2", "3", "4", "5", "6", "7", "8", "9"
                                                                        For intPrintNumber As Integer = 0 To CInt(External_Param(1).Substring(3, 1).Trim) - 1
                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                        Next
                                                                    Case Else
                                                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                        Next
                                                                End Select
                                                            End If
                                                        Case "PRT"
                                                            '---------------------------------------------------------
                                                            ' パラメータ構築
                                                            '---------------------------------------------------------
                                                            Dim Param_replace As String = ""
                                                            If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                Key.MESSAGE = "特殊パターン(PRT)印刷(パラメータ構築) 失敗"
                                                                HenkyakuArray.Item(i) = Key
                                                            End If
                                                            Param_replaceArray = Param_replace.Split(CChar(","))

                                                            '---------------------------------------------------------
                                                            ' 印刷情報取得
                                                            '---------------------------------------------------------
                                                            If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                Key.MESSAGE = "特殊パターン(PRT)印刷(印刷情報取得) 失敗"
                                                                HenkyakuArray.Item(i) = Key
                                                            End If

                                                            '---------------------------------------------------------
                                                            ' 拡張印刷実行（印刷対象チェック後）
                                                            '---------------------------------------------------------
                                                            If PrintExeCheck(External_Param(8), Key) = True Then
                                                                Select Case External_Param(1).Substring(3, 1).Trim
                                                                    Case "1", "2", "3", "4", "5", "6", "7", "8", "9"
                                                                        For intPrintNumber As Integer = 0 To CInt(External_Param(1).Substring(3, 1).Trim) - 1
                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                                        Next
                                                                    Case Else
                                                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                                        Next
                                                                End Select
                                                            End If
                                                    End Select
                                                End If
                                        End Select

                                        Select Case nRet
                                            Case 0
                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "成功", "0件")
                                            Case -1
                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "失敗", "")
                                                Key.MESSAGE = "特殊パターン印刷(" & Param_prtName & ") 失敗"
                                                HenkyakuArray.Item(i) = Key
                                                Exit For
                                            Case Else
                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "成功", nRet & "件")
                                        End Select
                                    End If
                                Next
                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷", "終了")
                        End Select
                    End If

                    OldKey = Key
                Next i

                Select Case strcINI_PARAM.RSV2_HENKAN_PRINT
                    Case "3", "4"
                        '======================================================================================================
                        ' 【特殊パターン印刷（帳票印刷順ソート）】
                        '======================================================================================================
                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷（帳票印刷順ソート）", "開始")
                        Dim ExternalPrintSortArray As New ArrayList
                        ExternalPrintSortArray.Clear()
                        GetTextFileInfo(strcINI_PARAM.RSV2_HENKAN_PRINT_SORT_TXT, ExternalPrintSortArray)

                        For PrintSortCount As Integer = 0 To ExternalPrintSortArray.Count - 1
                            Dim Sort_PrintID As String = ExternalPrintSortArray(PrintSortCount).ToString.Split(CChar(","))(0)

                            OldKey = New KeyInfo
                            Call OldKey.Init()

                            Select Case strcINI_PARAM.RSV2_HENKAN_PRINT
                                Case "3"
                                    '======================================================================================================
                                    ' 【特殊パターン印刷（帳票印刷順ソート　指定[3]）】
                                    '======================================================================================================
                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷（帳票印刷順ソート　指定[3]）", "開始")

                                    For i As Integer = 0 To HenkyakuArray.Count - 1
                                        Dim Key As KeyInfo = DirectCast(HenkyakuArray.Item(i), KeyInfo)

                                        LOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                                        LOG.FuriDate = Key.FURI_DATE

                                        If Key.MESSAGE.Trim = "" AndAlso strcTORI_PARAM.KEKKA_HENKYAKU_KBN <> "2" Then
                                            Dim ExtendPrint As New CAstExtendPrint.CExtendPrint
                                            Dim nRet As Integer = 0

                                            Dim ExternalPrintArray As New ArrayList
                                            Dim External_Param As String() = Nothing
                                            Dim Param_prtID As String = ""
                                            Dim Param_prtName As String = ""
                                            Dim Param_replaceArray As String() = Nothing
                                            Dim Param_printerArray As String() = Nothing
                                            Dim Param_dllName As String = ""
                                            Dim Param_className As String = ""
                                            Dim Param_methodName As String = ""

                                            ExternalPrintArray.Clear()
                                            GetTextFileInfo(strcINI_PARAM.RSV2_HENKAN_PRINT_TXT, ExternalPrintArray)

                                            '---------------------------------------------------------
                                            ' 印刷処理(部数分繰返)
                                            '---------------------------------------------------------
                                            If strcTORI_PARAM.PRTNUM = 0 Then
                                                strcTORI_PARAM.PRTNUM = 1
                                            End If

                                            For TextCount As Integer = 0 To ExternalPrintArray.Count - 1 Step 1
                                                External_Param = ExternalPrintArray(TextCount).ToString.Split(CChar(","))
                                                Param_prtID = External_Param(2)
                                                Param_prtName = External_Param(3)

                                                If Param_prtID = Sort_PrintID Then

                                                    If Key.FUNOU_MEISAI_KBN = External_Param(0) Then
                                                        nRet = 0

                                                        Select Case External_Param(1)
                                                            Case "EXE"
                                                                '---------------------------------------------------------
                                                                ' パラメータ構築
                                                                '---------------------------------------------------------
                                                                ReDim Param_replaceArray(0)
                                                                Param_replaceArray(0) = ""
                                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replaceArray(0)) = False Then
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(EXE)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                    Key.MESSAGE = "特殊パターン(EXE)印刷(パラメータ構築) 失敗"
                                                                    HenkyakuArray.Item(i) = Key
                                                                End If

                                                                Param_dllName = External_Param(4)
                                                                Param_className = External_Param(5)
                                                                Param_methodName = External_Param(6)

                                                                '---------------------------------------------------------
                                                                ' 拡張印刷実行（印刷対象チェック後）
                                                                '---------------------------------------------------------
                                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                                    For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                    Next
                                                                End If
                                                            Case "DLL"
                                                                '---------------------------------------------------------
                                                                ' パラメータ構築
                                                                '---------------------------------------------------------
                                                                Dim Param_replace As String = ""
                                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                    Key.MESSAGE = "特殊パターン(DLL)印刷(パラメータ構築) 失敗"
                                                                    HenkyakuArray.Item(i) = Key
                                                                End If
                                                                Param_replaceArray = Param_replace.Split(CChar(","))

                                                                '---------------------------------------------------------
                                                                ' 印刷情報取得
                                                                '---------------------------------------------------------
                                                                If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                    Key.MESSAGE = "特殊パターン(DLL)印刷(印刷情報取得) 失敗"
                                                                    HenkyakuArray.Item(i) = Key
                                                                End If

                                                                '---------------------------------------------------------
                                                                ' 拡張印刷実行（印刷対象チェック後）
                                                                '---------------------------------------------------------
                                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                                    For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                    Next
                                                                End If
                                                            Case "PRT"
                                                                '---------------------------------------------------------
                                                                ' パラメータ構築
                                                                '---------------------------------------------------------
                                                                Dim Param_replace As String = ""
                                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                    Key.MESSAGE = "特殊パターン(PRT)印刷(パラメータ構築) 失敗"
                                                                    HenkyakuArray.Item(i) = Key
                                                                End If
                                                                Param_replaceArray = Param_replace.Split(CChar(","))

                                                                '---------------------------------------------------------
                                                                ' 印刷情報取得
                                                                '---------------------------------------------------------
                                                                If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                    Key.MESSAGE = "特殊パターン(PRT)印刷(印刷情報取得) 失敗"
                                                                    HenkyakuArray.Item(i) = Key
                                                                End If

                                                                '---------------------------------------------------------
                                                                ' 拡張印刷実行（印刷対象チェック後）
                                                                '---------------------------------------------------------
                                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                                    For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                                    Next
                                                                End If
                                                            Case Else
                                                                '---------------------------------------------------------
                                                                ' グループ単位帳票
                                                                '---------------------------------------------------------
                                                                If Key.Matching(OldKey) <> 0 AndAlso i = 0 Then
                                                                    Select Case External_Param(1).Substring(0, 3)
                                                                        Case "EXE"
                                                                            '---------------------------------------------------------
                                                                            ' パラメータ構築
                                                                            '---------------------------------------------------------
                                                                            ReDim Param_replaceArray(0)
                                                                            Param_replaceArray(0) = ""
                                                                            If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replaceArray(0)) = False Then
                                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(EXE)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                                Key.MESSAGE = "特殊パターン(EXE)印刷(パラメータ構築) 失敗"
                                                                                HenkyakuArray.Item(i) = Key
                                                                            End If

                                                                            Param_dllName = External_Param(4)
                                                                            Param_className = External_Param(5)
                                                                            Param_methodName = External_Param(6)

                                                                            '---------------------------------------------------------
                                                                            ' 拡張印刷実行（印刷対象チェック後）
                                                                            '---------------------------------------------------------
                                                                            If PrintExeCheck(External_Param(8), Key) = True Then
                                                                                Select Case External_Param(1).Substring(3, 1).Trim
                                                                                    Case "1", "2", "3", "4", "5", "6", "7", "8", "9"
                                                                                        For intPrintNumber As Integer = 0 To CInt(External_Param(1).Substring(3, 1).Trim) - 1
                                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                                        Next
                                                                                    Case Else
                                                                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                                        Next
                                                                                End Select
                                                                            End If
                                                                        Case "DLL"
                                                                            '---------------------------------------------------------
                                                                            ' パラメータ構築
                                                                            '---------------------------------------------------------
                                                                            Dim Param_replace As String = ""
                                                                            If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                                Key.MESSAGE = "特殊パターン(DLL)印刷(パラメータ構築) 失敗"
                                                                                HenkyakuArray.Item(i) = Key
                                                                            End If
                                                                            Param_replaceArray = Param_replace.Split(CChar(","))

                                                                            '---------------------------------------------------------
                                                                            ' 印刷情報取得
                                                                            '---------------------------------------------------------
                                                                            If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                                Key.MESSAGE = "特殊パターン(DLL)印刷(印刷情報取得) 失敗"
                                                                                HenkyakuArray.Item(i) = Key
                                                                            End If

                                                                            '---------------------------------------------------------
                                                                            ' 拡張印刷実行（印刷対象チェック後）
                                                                            '---------------------------------------------------------
                                                                            If PrintExeCheck(External_Param(8), Key) = True Then
                                                                                Select Case External_Param(1).Substring(3, 1).Trim
                                                                                    Case "1", "2", "3", "4", "5", "6", "7", "8", "9"
                                                                                        For intPrintNumber As Integer = 0 To CInt(External_Param(1).Substring(3, 1).Trim) - 1
                                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                                        Next
                                                                                    Case Else
                                                                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                                        Next
                                                                                End Select
                                                                            End If
                                                                        Case "PRT"
                                                                            '---------------------------------------------------------
                                                                            ' パラメータ構築
                                                                            '---------------------------------------------------------
                                                                            Dim Param_replace As String = ""
                                                                            If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                                Key.MESSAGE = "特殊パターン(PRT)印刷(パラメータ構築) 失敗"
                                                                                HenkyakuArray.Item(i) = Key
                                                                            End If
                                                                            Param_replaceArray = Param_replace.Split(CChar(","))

                                                                            '---------------------------------------------------------
                                                                            ' 印刷情報取得
                                                                            '---------------------------------------------------------
                                                                            If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                                Key.MESSAGE = "特殊パターン(PRT)印刷(印刷情報取得) 失敗"
                                                                                HenkyakuArray.Item(i) = Key
                                                                            End If

                                                                            '---------------------------------------------------------
                                                                            ' 拡張印刷実行（印刷対象チェック後）
                                                                            '---------------------------------------------------------
                                                                            If PrintExeCheck(External_Param(8), Key) = True Then
                                                                                Select Case External_Param(1).Substring(3, 1).Trim
                                                                                    Case "1", "2", "3", "4", "5", "6", "7", "8", "9"
                                                                                        For intPrintNumber As Integer = 0 To CInt(External_Param(1).Substring(3, 1).Trim) - 1
                                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                                                        Next
                                                                                    Case Else
                                                                                        For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                                                                            nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                                                        Next
                                                                                End Select
                                                                            End If
                                                                    End Select
                                                                End If
                                                        End Select

                                                        Select Case nRet
                                                            Case 0
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "成功", "0件")
                                                            Case -1
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "失敗", "")
                                                                Key.MESSAGE = "特殊パターン印刷(" & Param_prtName & ") 失敗"
                                                                HenkyakuArray.Item(i) = Key
                                                                Exit For
                                                            Case Else
                                                                LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "成功", nRet & "件")
                                                        End Select
                                                    End If
                                                End If
                                            Next
                                        End If

                                        OldKey = Key
                                    Next i
                                Case "4"
                                    '======================================================================================================
                                    ' 【特殊パターン印刷（帳票印刷順ソート　指定[4]）】
                                    '======================================================================================================
                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷（帳票印刷順ソート　指定[4]）", "開始")

                                    For intPrintNumber As Integer = 0 To strcTORI_PARAM.PRTNUM - 1
                                        For i As Integer = 0 To HenkyakuArray.Count - 1
                                            Dim Key As KeyInfo = DirectCast(HenkyakuArray.Item(i), KeyInfo)

                                            LOG.ToriCode = Key.TORIS_CODE & Key.TORIF_CODE
                                            LOG.FuriDate = Key.FURI_DATE

                                            If Key.MESSAGE.Trim = "" AndAlso strcTORI_PARAM.KEKKA_HENKYAKU_KBN <> "2" Then
                                                Dim ExtendPrint As New CAstExtendPrint.CExtendPrint
                                                Dim nRet As Integer = 0

                                                Dim ExternalPrintArray As New ArrayList
                                                Dim External_Param As String() = Nothing
                                                Dim Param_prtID As String = ""
                                                Dim Param_prtName As String = ""
                                                Dim Param_replaceArray As String() = Nothing
                                                Dim Param_printerArray As String() = Nothing
                                                Dim Param_dllName As String = ""
                                                Dim Param_className As String = ""
                                                Dim Param_methodName As String = ""

                                                ExternalPrintArray.Clear()
                                                GetTextFileInfo(strcINI_PARAM.RSV2_HENKAN_PRINT_TXT, ExternalPrintArray)

                                                '---------------------------------------------------------
                                                ' 印刷処理(部数分繰返)
                                                '---------------------------------------------------------
                                                If strcTORI_PARAM.PRTNUM = 0 Then
                                                    strcTORI_PARAM.PRTNUM = 1
                                                End If

                                                For TextCount As Integer = 0 To ExternalPrintArray.Count - 1 Step 1
                                                    External_Param = ExternalPrintArray(TextCount).ToString.Split(CChar(","))
                                                    Param_prtID = External_Param(2)
                                                    Param_prtName = External_Param(3)

                                                    If Param_prtID = Sort_PrintID Then

                                                        If Key.FUNOU_MEISAI_KBN = External_Param(0) Then
                                                            nRet = 0

                                                            Select Case External_Param(1)
                                                                Case "EXE"
                                                                    '---------------------------------------------------------
                                                                    ' パラメータ構築
                                                                    '---------------------------------------------------------
                                                                    ReDim Param_replaceArray(0)
                                                                    Param_replaceArray(0) = ""
                                                                    If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replaceArray(0)) = False Then
                                                                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(EXE)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                        Key.MESSAGE = "特殊パターン(EXE)印刷(パラメータ構築) 失敗"
                                                                        HenkyakuArray.Item(i) = Key
                                                                    End If

                                                                    Param_dllName = External_Param(4)
                                                                    Param_className = External_Param(5)
                                                                    Param_methodName = External_Param(6)

                                                                    '---------------------------------------------------------
                                                                    ' 拡張印刷実行（印刷対象チェック後）
                                                                    '---------------------------------------------------------
                                                                    If PrintExeCheck(External_Param(8), Key) = True Then
                                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                    End If
                                                                Case "DLL"
                                                                    '---------------------------------------------------------
                                                                    ' パラメータ構築
                                                                    '---------------------------------------------------------
                                                                    Dim Param_replace As String = ""
                                                                    If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                        Key.MESSAGE = "特殊パターン(DLL)印刷(パラメータ構築) 失敗"
                                                                        HenkyakuArray.Item(i) = Key
                                                                    End If
                                                                    Param_replaceArray = Param_replace.Split(CChar(","))

                                                                    '---------------------------------------------------------
                                                                    ' 印刷情報取得
                                                                    '---------------------------------------------------------
                                                                    If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                        Key.MESSAGE = "特殊パターン(DLL)印刷(印刷情報取得) 失敗"
                                                                        HenkyakuArray.Item(i) = Key
                                                                    End If

                                                                    '---------------------------------------------------------
                                                                    ' 拡張印刷実行（印刷対象チェック後）
                                                                    '---------------------------------------------------------
                                                                    If PrintExeCheck(External_Param(8), Key) = True Then
                                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                    End If
                                                                Case "PRT"
                                                                    '---------------------------------------------------------
                                                                    ' パラメータ構築
                                                                    '---------------------------------------------------------
                                                                    Dim Param_replace As String = ""
                                                                    If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                        Key.MESSAGE = "特殊パターン(PRT)印刷(パラメータ構築) 失敗"
                                                                        HenkyakuArray.Item(i) = Key
                                                                    End If
                                                                    Param_replaceArray = Param_replace.Split(CChar(","))

                                                                    '---------------------------------------------------------
                                                                    ' 印刷情報取得
                                                                    '---------------------------------------------------------
                                                                    If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                        Key.MESSAGE = "特殊パターン(PRT)印刷(印刷情報取得) 失敗"
                                                                        HenkyakuArray.Item(i) = Key
                                                                    End If

                                                                    '---------------------------------------------------------
                                                                    ' 拡張印刷実行（印刷対象チェック後）
                                                                    '---------------------------------------------------------
                                                                    If PrintExeCheck(External_Param(8), Key) = True Then
                                                                        nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                                    End If
                                                                Case Else
                                                                    '---------------------------------------------------------
                                                                    ' グループ単位帳票
                                                                    '---------------------------------------------------------
                                                                    If Key.Matching(OldKey) <> 0 AndAlso i = 0 Then
                                                                        Select Case External_Param(1).Substring(0, 3)
                                                                            Case "EXE"
                                                                                '---------------------------------------------------------
                                                                                ' パラメータ構築
                                                                                '---------------------------------------------------------
                                                                                ReDim Param_replaceArray(0)
                                                                                Param_replaceArray(0) = ""
                                                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replaceArray(0)) = False Then
                                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(EXE)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                                    Key.MESSAGE = "特殊パターン(EXE)印刷(パラメータ構築) 失敗"
                                                                                    HenkyakuArray.Item(i) = Key
                                                                                End If

                                                                                Param_dllName = External_Param(4)
                                                                                Param_className = External_Param(5)
                                                                                Param_methodName = External_Param(6)

                                                                                '---------------------------------------------------------
                                                                                ' 拡張印刷実行（印刷対象チェック後）
                                                                                '---------------------------------------------------------
                                                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                                                    nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                                End If
                                                                            Case "DLL"
                                                                                '---------------------------------------------------------
                                                                                ' パラメータ構築
                                                                                '---------------------------------------------------------
                                                                                Dim Param_replace As String = ""
                                                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                                    Key.MESSAGE = "特殊パターン(DLL)印刷(パラメータ構築) 失敗"
                                                                                    HenkyakuArray.Item(i) = Key
                                                                                End If
                                                                                Param_replaceArray = Param_replace.Split(CChar(","))

                                                                                '---------------------------------------------------------
                                                                                ' 印刷情報取得
                                                                                '---------------------------------------------------------
                                                                                If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(DLL)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                                    Key.MESSAGE = "特殊パターン(DLL)印刷(印刷情報取得) 失敗"
                                                                                    HenkyakuArray.Item(i) = Key
                                                                                End If

                                                                                '---------------------------------------------------------
                                                                                ' 拡張印刷実行（印刷対象チェック後）
                                                                                '---------------------------------------------------------
                                                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                                                    nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False, Param_dllName, Param_className, Param_methodName)
                                                                                End If
                                                                            Case "PRT"
                                                                                '---------------------------------------------------------
                                                                                ' パラメータ構築
                                                                                '---------------------------------------------------------
                                                                                Dim Param_replace As String = ""
                                                                                If MakePrintParam(Param_prtID, External_Param(7), Key, Param_replace) = False Then
                                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(パラメータ構築)", "失敗", "パラメータ構築:" & External_Param(7))
                                                                                    Key.MESSAGE = "特殊パターン(PRT)印刷(パラメータ構築) 失敗"
                                                                                    HenkyakuArray.Item(i) = Key
                                                                                End If
                                                                                Param_replaceArray = Param_replace.Split(CChar(","))

                                                                                '---------------------------------------------------------
                                                                                ' 印刷情報取得
                                                                                '---------------------------------------------------------
                                                                                If GetPrintInfo(Param_prtID, Param_prtName, Param_printerArray, Param_dllName, Param_className, Param_methodName) = False Then
                                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン(PRT)印刷(印刷情報取得)", "失敗", "帳票ＩＤ:" & Param_prtID)
                                                                                    Key.MESSAGE = "特殊パターン(PRT)印刷(印刷情報取得) 失敗"
                                                                                    HenkyakuArray.Item(i) = Key
                                                                                End If

                                                                                '---------------------------------------------------------
                                                                                ' 拡張印刷実行（印刷対象チェック後）
                                                                                '---------------------------------------------------------
                                                                                If PrintExeCheck(External_Param(8), Key) = True Then
                                                                                    nRet = ExtendPrint.ExtendPrint4Exe(Param_prtID, Param_prtName, Param_replaceArray, Param_printerArray, False)
                                                                                End If
                                                                        End Select
                                                                    End If
                                                            End Select

                                                            Select Case nRet
                                                                Case 0
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "成功", "0件")
                                                                Case -1
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "失敗", "")
                                                                    Key.MESSAGE = "特殊パターン印刷(" & Param_prtName & ") 失敗"
                                                                    HenkyakuArray.Item(i) = Key
                                                                    Exit For
                                                                Case Else
                                                                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷(" & Param_prtName & ")", "成功", nRet & "件")
                                                            End Select
                                                        End If
                                                    End If
                                                Next
                                            End If

                                            OldKey = Key
                                        Next i
                                    Next intPrintNumber
                            End Select
                        Next PrintSortCount
                        LOG.Write(LOG.ToriCode, LOG.FuriDate, "特殊パターン印刷", "終了")
                End Select
            End If

            If blnRet = False Then
                If Not HenkyakuArray Is Nothing AndAlso HenkyakuArray.Count = 0 Then
                    Call LOG.UpdateJOBMASTbyErr("ログ参照")
                End If

                ' ロールバック
                MainDB.Rollback()
            Else
                Dim Message As String = ""
                Dim ErrorDetail As String = ""
                Dim ErrMiFlag As Boolean = False        ' 未処理ありフラグ
                For i As Integer = 0 To HenkyakuArray.Count - 1
                    Dim Key As KeyInfo = DirectCast(HenkyakuArray.Item(i), KeyInfo)

                    If Key.MESSAGE.Trim <> "" Then
                        Message = Key.MESSAGE.Trim & "データあり"
                        ErrorDetail = Key.ErrorDetail
                        ErrMiFlag = True
                        Exit For
                    End If
                Next i
                If ErrMiFlag = False Then
                    Call LOG.UpdateJOBMASTbyOK(Message)

                    '2012/06/30 標準版　WEB伝送対応---------------------------->
                    If strcTORI_PARAM.BAITAI_CODE = "10" Then
                        File.Create(strOUT_END_FILE_NAME)   'ENDファイル作成
                    End If
                    '----------------------------------------------------------<

                Else
                    Call LOG.UpdateJOBMASTbyErr(Message & "【" & ErrorDetail & "】")
                End If

                ' コミット
                MainDB.Commit()

                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)

                Dim ToriCode(HenkyakuArray.Count - 1) As String
                Dim FuriDate As String = ""
                For i As Integer = 0 To HenkyakuArray.Count - 1
                    Dim Key As KeyInfo = DirectCast(HenkyakuArray.Item(i), KeyInfo)
                    FuriDate = Key.FURI_DATE
                    If Key.MESSAGE.Trim = "" Then
                        ToriCode(i) = Key.TORIS_CODE & Key.TORIF_CODE
                    End If
                Next i

                Dim para As New CAstBatch.CommData.stPARAMETER
                para.FSYORI_KBN = "1"
                para.FMT_KBN = strcTORI_PARAM.FMT_KBN
                Dim HenkanFMT As CAstFormat.CFormat = CAstFormat.CFormat.GetFormat(para)

                If HenkanFMT.CallHenkanExit(ToriCode, FuriDate) = False Then
                    LOG.Write("返還用登録出口メソッド処理", "失敗", "")
                End If

                '===============================================
                ' 伝送システム宛　データ送信処理
                '===============================================
                If strcINI_PARAM.RSV2_USE_DENSO = "1" Then
                    Dim DENSO_EXE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "DENSO_EXE")
                    Dim DENSO_BAITAI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "DENSO_BAITAI")
                    Dim DENSO_SAIFURI_HENKAN As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "DENSO_SAIFURI_HENKAN")

                    Dim DensoCheck As String = "0"
                    If DENSO_BAITAI.IndexOf(strcTORI_PARAM.BAITAI_CODE) >= 0 Then
                        ' 伝送システムの送信対象媒体の場合
                        Select Case DENSO_SAIFURI_HENKAN
                            Case "YES"
                                ' 再振の返還も送信する場合
                                DensoCheck = "1"
                            Case "NO"
                                ' 再振の返還は送信しない場合 (再振の取引先の場合Trueとなる)
                                If Check_Saifuri(strcKOBETUPARAM.strTORIS_CODE, strcKOBETUPARAM.strTORIF_CODE) = False Then
                                    DensoCheck = "1"
                                End If
                        End Select
                    End If

                    Select Case strcTORI_PARAM.KEKKA_HENKYAKU_KBN
                        Case "0", "3"
                            DensoCheck = "0"
                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成処理", "成功", "返却不要 結果返却要否:" & strcTORI_PARAM.KEKKA_HENKYAKU_KBN)
                    End Select

                    If DensoCheck = "1" Then
                        Dim Proc As New Process
                        Dim ProcInfo As New ProcessStartInfo
                        ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), DENSO_EXE)
                        ProcInfo.WorkingDirectory = ""
                        ProcInfo.Arguments = strOUT_FILE_NAME
                        Proc = Process.Start(ProcInfo)
                        Proc.WaitForExit()

                        If Proc.ExitCode = 0 Then
                            ' 連携成功
                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成処理", "成功", "伝送送信処理 Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                            Call LOG.UpdateJOBMASTbyOK("伝送完了")
                        Else
                            ' 連携失敗
                            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成処理", "失敗", "伝送送信処理 Return:" & Proc.ExitCode.ToString & " " & ProcInfo.FileName & " " & ProcInfo.Arguments)
                        End If
                    End If
                End If
            End If

            MainDB.Close()
            MainDB = Nothing

            SubDB.Close()
            SubDB = Nothing

            If blnRet = False Then
                Return 2
            End If
            Return 0

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成処理", "失敗", ex.Message)
            Return 1
        Finally
            If Not MainDB Is Nothing Then
                '媒体検証エラー時はdblockが生成されていない
                If Not dblock Is Nothing Then
                    dblock.Job_UnLock(MainDB)
                End If
                '最後まで生きてたら元に戻す。
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            If Not SubDB Is Nothing Then
                SubDB.Close()
                SubDB = Nothing
            End If
        End Try
    End Function

    ' 機能　 ： 媒体書込
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function fn_BAITAI_WRITE() As Boolean

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体書込処理(開始)", "成功")

            fn_BAITAI_WRITE = False

            Dim strCHECK_SEND_FILE As String
            Dim intKEKKA As Integer
            Dim strKEKKA As String
            Dim lngKEKKA As Long

            strCHECK_SEND_FILE = strcINI_PARAM.DATBK_FOLDER &
                                "A" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & ".dat"
            If File.Exists(strCHECK_SEND_FILE) = True Then
                Kill(strCHECK_SEND_FILE)
            End If

            ' 媒体がMT/CMTかつ外部取り付けの場合は伝送と同じ処理を行う
            Dim WriteBaitai As String
            If strcTORI_PARAM.BAITAI_CODE = "05" AndAlso strcINI_PARAM.MT_FLG = "1" Then
                WriteBaitai = "00"
            ElseIf strcTORI_PARAM.BAITAI_CODE = "06" AndAlso strcINI_PARAM.CMT_FLG = "1" Then
                WriteBaitai = "00"
            Else
                WriteBaitai = strcTORI_PARAM.BAITAI_CODE
            End If

            '--------------------------------------------
            ' 振替結果データ(中間ファイル)の退避
            '--------------------------------------------
            Dim BKUP_CHECK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_CHECK_OUT")
            If BKUP_CHECK <> "err" Then
                If BKUP_CHECK = "1" Then
                    Dim BKUP_BAITAI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_BAITAI_OUT")

                    If BKUP_BAITAI.IndexOf(WriteBaitai) >= 0 Then
                        Dim BKUP_FOLDER As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_FOLDER_OUT")
                        Dim BKUP_FILENAME As String = Path.Combine(BKUP_FOLDER, "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & "_" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & ".DAT")

                        Try
                            File.Copy(strIN_FILE_NAME, BKUP_FILENAME, True)
                        Catch ex As Exception
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "結果データ退避", "失敗 ", ex.Message)
                        End Try
                    End If
                End If
            End If

            Select Case WriteBaitai
                Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"        '伝送
                    Select Case strcINI_PARAM.RSV2_USE_DENSO
                        Case "1"
                            '-----------------------------------
                            ' 伝送システム使用
                            '-----------------------------------
                            If strcTORI_PARAM.MULTI_KBN = "1" Then
                                strOUT_FILE_NAME = strcINI_PARAM.DEN_FOLDER &
                                                   "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & "_" &
                                                   strcKOBETUPARAM.strFURI_DATE & ".dat"   '出力ファイル
                            Else
                                strOUT_FILE_NAME = strcINI_PARAM.DEN_FOLDER &
                                                   "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & "_" &
                                                   strcKOBETUPARAM.strFURI_DATE & ".dat"     '出力ファイル
                            End If
                        Case Else
                            '-----------------------------------
                            ' 伝送システム未使用
                            '-----------------------------------
                            If strcTORI_PARAM.MULTI_KBN = "1" Then
                                strOUT_FILE_NAME = strcINI_PARAM.DEN_FOLDER & "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & ".dat"     '出力ファイル
                            Else
                                strOUT_FILE_NAME = strcINI_PARAM.DEN_FOLDER & "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & ".dat"     '出力ファイル
                            End If
                    End Select

                    Select Case strcTORI_PARAM.CODE_KBN
                        Case "0", "1", "4"
                            intKEKKA = clsFUSION.fn_DISK_CPYTO_DEN(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                                   strIN_FILE_NAME,
                                                                   strOUT_FILE_NAME,
                                                                   intREC_LENGTH,
                                                                   strcTORI_PARAM.CODE_KBN,
                                                                   gstrP_FILE)
                            Select Case intKEKKA
                                Case 0
                                    fn_BAITAI_WRITE = True
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "成功 ", "ファイル変換（コード変換）")
                                Case 100
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "失敗 ", "ファイル変換（コード変換）")
                                    LOG.UpdateJOBMASTbyErr("ファイル変換（コード変換）失敗")
                                    Exit Function
                            End Select
                        Case Else
                            If Dir(strOUT_FILE_NAME) <> "" Then
                                Kill(strOUT_FILE_NAME)
                            End If
                            File.Copy(strIN_FILE_NAME, strOUT_FILE_NAME)
                            fn_BAITAI_WRITE = True
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "成功 ", "ファイル変換（コード変換）")
                    End Select


                Case "01"       'FD書込み
                    Select Case strcINI_PARAM.RSV2_EDITION
                        Case "2"
                            '-----------------------------------
                            ' 出力ファイル名設定
                            '-----------------------------------
                            If strcTORI_PARAM.MULTI_KBN = "1" Then
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            Else
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            End If

                            '-----------------------------------
                            ' ファイル出力処理
                            '-----------------------------------
                            'コード変換すると媒体変換処理のコード変換でさらにコード変換してしまうため、返還データ作成ではコード変換を行わない。
                            If Dir(strOUT_FILE_NAME) <> "" Then
                                Kill(strOUT_FILE_NAME)
                            End If
                            File.Copy(strIN_FILE_NAME, strOUT_FILE_NAME)
                            fn_BAITAI_WRITE = True
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "成功 ", "ファイル変換（コード変換）")
                        Case Else
                            strOUT_FILE_NAME = strcTORI_PARAM.FILE_NAME.Trim

                            intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                                 strOUT_FILE_NAME,
                                                                 strCHECK_SEND_FILE,
                                                                 intREC_LENGTH,
                                                                 strcTORI_PARAM.CODE_KBN,
                                                                 gstrP_FILE, msgTitle)
                            Select Case intKEKKA
                                Case 0

                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "成功 ", "ＦＤ取込（コード変換）")

                                Case 100
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（コード変換）")
                                    LOG.UpdateJOBMASTbyErr("ＦＤ読み込み失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 200
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（コード区分異常（JIS改行あり））")
                                    LOG.UpdateJOBMASTbyErr("ＦＤ取込（コード区分異常（JIS改行あり））失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 300
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（コード区分異常（JIS改行なし））")
                                    LOG.UpdateJOBMASTbyErr("ＦＤ取込（コード区分異常（JIS改行なし））失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 400
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（出力ファイル作成）失敗")
                                    LOG.UpdateJOBMASTbyErr("ＦＤ取込（出力ファイル作成）失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                            End Select

                            '------------------------------------
                            '媒体内のファイルのチェック
                            '------------------------------------
                            If fn_CHK_BAITAI(strCHECK_SEND_FILE, strIN_FILE_NAME, intREC_LENGTH) = False Then
                                fn_BAITAI_WRITE = False
                                Exit Function
                            End If

                            '------------------------------------
                            '媒体に書き込み
                            '------------------------------------
                            If strcTORI_PARAM.BAITAI_CODE = "01" Then     '３．５ＦＤ（1枚組）
                                intKEKKA = clsFUSION.fn_DISK_CPYTO_FD(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                                      strIN_FILE_NAME,
                                                                      strOUT_FILE_NAME,
                                                                      intREC_LENGTH,
                                                                      strcTORI_PARAM.CODE_KBN, strIBMP_FILE, False, msgTitle)
                            End If
                            Select Case intKEKKA
                                Case 0
                                    fn_BAITAI_WRITE = True
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_FD", "成功 ", "ＦＤ書込み")
                                Case 100
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_FD", "失敗 ", "ＦＤ書込み失敗（IBM形式）")
                                    LOG.UpdateJOBMASTbyErr("ＦＤ書込み失敗（IBM形式）")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 200
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_FD", "失敗 ", "ＦＤ書込み失敗（DOS形式）")
                                    LOG.UpdateJOBMASTbyErr("ＦＤ書込み失敗（DOS形式）")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                            End Select
                    End Select
                Case "04"        '依頼書

                Case "05"        'ＭＴ
                    '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- START
                    Select Case strcINI_PARAM.RSV2_EDITION
                        Case "2"
                            '-----------------------------------
                            ' 出力ファイル名設定
                            '-----------------------------------
                            If strcTORI_PARAM.MULTI_KBN = "1" Then
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            Else
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            End If

                            '-----------------------------------
                            ' ファイル出力処理
                            '-----------------------------------
                            If Dir(strOUT_FILE_NAME) <> "" Then
                                Kill(strOUT_FILE_NAME)
                            End If
                            File.Copy(strIN_FILE_NAME, strOUT_FILE_NAME)
                            fn_BAITAI_WRITE = True

                        Case Else
                            Select Case strcINI_PARAM.MT_FLG
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
                                        Case 390
                                            intBlkSize = 3900
                                    End Select
                                    '---------------------------------
                                    'ＭＴ内ファイルの読み込み
                                    '----------------------------------
                                    strKEKKA = vbDLL.mtCPYtoDISK_CHK(intBlkSize,
                                                             intREC_LENGTH,
                                                             CInt(strcTORI_PARAM.LABEL_KBN),
                                                             "SLMT", 1, 4, " ", strCHECK_SEND_FILE, 0,
                                                             lngErrStatus)

                                    If strKEKKA <> "" Then
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoDISK_CHK", "失敗 ", "ＭＴ読み込み:" & strKEKKA)
                                        LOG.UpdateJOBMASTbyErr("ＭＴ読み込み:" & strKEKKA)
                                        Exit Function
                                    Else
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoDISK_CHK", "成功 ", "ＭＴ読み込み")
                                    End If

                                    '------------------------------------
                                    '媒体内のファイルのチェック
                                    '------------------------------------
                                    If fn_CHK_BAITAI(strCHECK_SEND_FILE, strIN_FILE_NAME, intREC_LENGTH) = False Then
                                        lngKEKKA = vbDLL.mt_UNLOAD
                                        If lngKEKKA <> 0 Then
                                            MessageBox.Show(String.Format(MSG0027E, "ＭＴ装置", "アンロード"),
                                                    msgTitle,
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                        End If
                                        fn_BAITAI_WRITE = False
                                        Exit Function
                                    End If

                                    '------------------------------------
                                    '媒体に書き込み
                                    '------------------------------------
                                    strKEKKA = vbDLL.mtCPYtoMT_CHK(intBlkSize,
                                                           intREC_LENGTH,
                                                           CInt(strcTORI_PARAM.LABEL_KBN), "SLMT", 1, 4,
                                                           strIN_FILE_NAME, " ", 0, lngErrStatus)

                                    If strKEKKA <> "" Then
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "失敗 ", "ＭＴ書込み:" & strKEKKA)
                                        LOG.UpdateJOBMASTbyErr("ＭＴ書込み:" & strKEKKA)
                                        Exit Function
                                    Else
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "成功 ", "ＭＴ書込み")
                                    End If
                                Case "1"      'ＭＴが自振サーバに接続していない場合
                                    If strcINI_PARAM.MT_OUTFILE = "" Then
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "失敗 ", "ＭＴ書込み(ファイル名取得)：" & Err.Description)
                                        LOG.UpdateJOBMASTbyErr("ＭＴ書込み(ファイル名取得)失敗")
                                        Exit Function
                                    End If
                                    If Dir(strcINI_PARAM.MT_OUTFILE) <> "" Then
                                        Kill(strcINI_PARAM.MT_OUTFILE)
                                    End If
                                    FileCopy(strIN_FILE_NAME, strcINI_PARAM.MT_OUTFILE)
                                    If Err.Number = 0 Then
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "成功 ", "ＭＴ書込み(ファイルコピー)")
                                    Else
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "失敗 ", "ＭＴ書込み(ファイルコピー)失敗")
                                        LOG.UpdateJOBMASTbyErr("ＭＴ書込み(ファイルコピー)失敗")
                                        Exit Function
                                    End If
                            End Select
                    End Select

                'Select Case strcINI_PARAM.MT_FLG
                '    Case "0"     'ＭＴが直接自振サーバに接続している場合
                '        Dim lngErrStatus As Long
                '        Dim intBlkSize As Integer
                '        Select Case intREC_LENGTH
                '            Case 120
                '                intBlkSize = 1800
                '            Case 220
                '                intBlkSize = 2200
                '            Case 300
                '                intBlkSize = 3000
                '            Case 390
                '                intBlkSize = 3900
                '        End Select
                '        '---------------------------------
                '        'ＭＴ内ファイルの読み込み
                '        '----------------------------------
                '        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                '        strKEKKA = vbDLL.mtCPYtoDISK_CHK(intBlkSize,
                '                                         intREC_LENGTH,
                '                                         CInt(strcTORI_PARAM.LABEL_KBN),
                '                                         "SLMT", 1, 4, " ", strCHECK_SEND_FILE, 0,
                '                                         lngErrStatus)
                '        'strKEKKA = vbDLL.mtCPYtoDISK_CHK(CShort(intBlkSize),
                '        '                                 CShort(intREC_LENGTH),
                '        '                                 CShort(strcTORI_PARAM.LABEL_KBN),
                '        '                                 "SLMT", 1, 4, " ", strCHECK_SEND_FILE, 0,
                '        '                                 CInt(lngErrStatus))
                '        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                '        If strKEKKA <> "" Then
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoDISK_CHK", "失敗 ", "ＭＴ読み込み:" & strKEKKA)
                '            LOG.UpdateJOBMASTbyErr("ＭＴ読み込み:" & strKEKKA)
                '            Exit Function
                '        Else
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoDISK_CHK", "成功 ", "ＭＴ読み込み")
                '        End If

                '        '------------------------------------
                '        '媒体内のファイルのチェック
                '        '------------------------------------
                '        If fn_CHK_BAITAI(strCHECK_SEND_FILE, strIN_FILE_NAME, intREC_LENGTH) = False Then
                '            lngKEKKA = vbDLL.mt_UNLOAD
                '            If lngKEKKA <> 0 Then
                '                MessageBox.Show(String.Format(MSG0027E, "ＭＴ装置", "アンロード"),
                '                                msgTitle,
                '                                MessageBoxButtons.OK,
                '                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                '            End If
                '            fn_BAITAI_WRITE = False
                '            Exit Function
                '        End If

                '        '------------------------------------
                '        '媒体に書き込み
                '        '------------------------------------
                '        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                '        strKEKKA = vbDLL.mtCPYtoMT_CHK(intBlkSize,
                '                                       intREC_LENGTH,
                '                                       CInt(strcTORI_PARAM.LABEL_KBN), "SLMT", 1, 4,
                '                                       strIN_FILE_NAME, " ", 0, lngErrStatus)
                '        'strKEKKA = vbDLL.mtCPYtoMT_CHK(CShort(intBlkSize),
                '        '                               CShort(intREC_LENGTH),
                '        '                               CShort(strcTORI_PARAM.LABEL_KBN), "SLMT", 1, 4,
                '        '                               strIN_FILE_NAME, " ", 0, CInt(lngErrStatus))
                '        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                '        If strKEKKA <> "" Then
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "失敗 ", "ＭＴ書込み:" & strKEKKA)
                '            LOG.UpdateJOBMASTbyErr("ＭＴ書込み:" & strKEKKA)
                '            Exit Function
                '        Else
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "成功 ", "ＭＴ書込み")
                '        End If
                '    Case "1"      'ＭＴが自振サーバに接続していない場合
                '        If strcINI_PARAM.MT_OUTFILE = "" Then
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "失敗 ", "ＭＴ書込み(ファイル名取得)：" & Err.Description)
                '            LOG.UpdateJOBMASTbyErr("ＭＴ書込み(ファイル名取得)失敗")
                '            Exit Function
                '        End If
                '        If Dir(strcINI_PARAM.MT_OUTFILE) <> "" Then
                '            Kill(strcINI_PARAM.MT_OUTFILE)
                '        End If
                '        FileCopy(strIN_FILE_NAME, strcINI_PARAM.MT_OUTFILE)
                '        If Err.Number = 0 Then
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "成功 ", "ＭＴ書込み(ファイルコピー)")
                '        Else
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoMT_CHK", "失敗 ", "ＭＴ書込み(ファイルコピー)失敗")
                '            LOG.UpdateJOBMASTbyErr("ＭＴ書込み(ファイルコピー)失敗")
                '            Exit Function
                '        End If
                'End Select
                '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- END
                Case "06"        'ＣＭＴ
                    '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- START
                    Select Case strcINI_PARAM.RSV2_EDITION
                        Case "2"
                            '-----------------------------------
                            ' 出力ファイル名設定
                            '-----------------------------------
                            If strcTORI_PARAM.MULTI_KBN = "1" Then
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            Else
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            End If

                            '-----------------------------------
                            ' ファイル出力処理
                            '-----------------------------------
                            If Dir(strOUT_FILE_NAME) <> "" Then
                                Kill(strOUT_FILE_NAME)
                            End If
                            File.Copy(strIN_FILE_NAME, strOUT_FILE_NAME)
                            fn_BAITAI_WRITE = True

                        Case Else
                            Select Case strcINI_PARAM.CMT_FLG
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
                                        Case 390
                                            intBlkSize = 3900
                                    End Select

                                    '2010.03.04 NHKは事前検証をしない start
                                    If strcTORI_PARAM.FMT_KBN <> "01" Then

                                        '---------------------------------
                                        'ＣＭＴ内ファイルの読み込み
                                        '----------------------------------
                                        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                                        strKEKKA = vbDLL.cmtCPYtoDISK_CHK(intBlkSize,
                                                                  intREC_LENGTH,
                                                                  CInt(strcTORI_PARAM.LABEL_KBN),
                                                                  "SLMT", 1, 4, " ", strCHECK_SEND_FILE, 0, lngErrStatus)
                                        'strKEKKA = vbDLL.cmtCPYtoDISK_CHK(CShort(intBlkSize),
                                        '                                  CShort(intREC_LENGTH),
                                        '                                  CShort(strcTORI_PARAM.LABEL_KBN),
                                        '                                  "SLMT", 1, 4, " ", strCHECK_SEND_FILE, 0, CInt(lngErrStatus))
                                        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                                        If strKEKKA <> "" Then
                                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoDISK_CHK", "失敗 ", "ＣＭＴ読み込み:" & strKEKKA)
                                            LOG.UpdateJOBMASTbyErr("ＣＭＴ読み込み:" & strKEKKA)
                                            Exit Function
                                        Else
                                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoDISK_CHK", "成功 ", "ＣＭＴ読み込み")
                                        End If
                                        '------------------------------------
                                        '媒体内のファイルのチェック
                                        '------------------------------------
                                        If fn_CHK_BAITAI(strCHECK_SEND_FILE, strIN_FILE_NAME, intREC_LENGTH) = False Then
                                            lngKEKKA = vbDLL.cmt_UNLOAD
                                            If lngKEKKA <> 0 Then

                                                MessageBox.Show(String.Format(MSG0027E, "ＭＴ装置", "アンロード"),
                                                        msgTitle,
                                                        MessageBoxButtons.OK,
                                                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                                            End If
                                            fn_BAITAI_WRITE = False
                                            Exit Function
                                        End If

                                    End If
                                    '2010.03.04 NHKは事前検証をしない end
                                    '------------------------------------
                                    '媒体に書き込み
                                    '------------------------------------
                                    '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                                    strKEKKA = vbDLL.cmtCPYtoCMT_CHK(intBlkSize,
                                                             intREC_LENGTH,
                                                             CInt(strcTORI_PARAM.LABEL_KBN),
                                                             "SLMT", 1, 4, strIN_FILE_NAME, " ", 0, lngErrStatus)
                                    'strKEKKA = vbDLL.cmtCPYtoCMT_CHK(CShort(intBlkSize),
                                    '                                 CShort(intREC_LENGTH),
                                    '                                 CShort(strcTORI_PARAM.LABEL_KBN),
                                    '                                 "SLMT", 1, 4, strIN_FILE_NAME, " ", 0, CInt(lngErrStatus))
                                    '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                                    If strKEKKA <> "" Then
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "失敗 ", "ＣＭＴ書込み:" & strKEKKA)
                                        LOG.UpdateJOBMASTbyErr("ＣＭＴ書込み:" & strKEKKA)
                                        Exit Function
                                    Else
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み")
                                    End If
                                Case "1"    'ＣＭＴが自振サーバに接続していない場合
                                    If strcINI_PARAM.CMT_OUTFILE = "" Then
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み(ファイル名取得)失敗")
                                        LOG.UpdateJOBMASTbyErr("ＣＭＴ書込み(ファイル名取得)失敗")
                                        Exit Function
                                    End If
                                    If Dir(strcINI_PARAM.CMT_OUTFILE) <> "" Then
                                        Kill(strcINI_PARAM.CMT_OUTFILE)
                                    End If
                                    FileCopy(strIN_FILE_NAME, strcINI_PARAM.CMT_OUTFILE)
                                    If Err.Number = 0 Then
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み(ファイルコピー)")
                                    Else
                                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み(ファイルコピー)失敗")
                                        LOG.UpdateJOBMASTbyErr("ＣＭＴ書込み(ファイルコピー)失敗")
                                        Exit Function
                                    End If
                            End Select
                    End Select

                'Select Case strcINI_PARAM.CMT_FLG
                '    Case "0"    'ＣＭＴが直接自振サーバに接続している場合
                '        Dim lngErrStatus As Long
                '        Dim intBlkSize As Integer
                '        Select Case intREC_LENGTH
                '            Case 120
                '                intBlkSize = 1800
                '            Case 220
                '                intBlkSize = 2200
                '            Case 300
                '                intBlkSize = 3000
                '            Case 390
                '                intBlkSize = 3900
                '        End Select

                '        '2010.03.04 NHKは事前検証をしない start
                '        If strcTORI_PARAM.FMT_KBN <> "01" Then

                '            '---------------------------------
                '            'ＣＭＴ内ファイルの読み込み
                '            '----------------------------------
                '            '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                '            strKEKKA = vbDLL.cmtCPYtoDISK_CHK(intBlkSize,
                '                                              intREC_LENGTH,
                '                                              CInt(strcTORI_PARAM.LABEL_KBN),
                '                                              "SLMT", 1, 4, " ", strCHECK_SEND_FILE, 0, lngErrStatus)
                '            'strKEKKA = vbDLL.cmtCPYtoDISK_CHK(CShort(intBlkSize),
                '            '                                  CShort(intREC_LENGTH),
                '            '                                  CShort(strcTORI_PARAM.LABEL_KBN),
                '            '                                  "SLMT", 1, 4, " ", strCHECK_SEND_FILE, 0, CInt(lngErrStatus))
                '            '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                '            If strKEKKA <> "" Then
                '                LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoDISK_CHK", "失敗 ", "ＣＭＴ読み込み:" & strKEKKA)
                '                LOG.UpdateJOBMASTbyErr("ＣＭＴ読み込み:" & strKEKKA)
                '                Exit Function
                '            Else
                '                LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoDISK_CHK", "成功 ", "ＣＭＴ読み込み")
                '            End If
                '            '------------------------------------
                '            '媒体内のファイルのチェック
                '            '------------------------------------
                '            If fn_CHK_BAITAI(strCHECK_SEND_FILE, strIN_FILE_NAME, intREC_LENGTH) = False Then
                '                lngKEKKA = vbDLL.cmt_UNLOAD
                '                If lngKEKKA <> 0 Then

                '                    MessageBox.Show(String.Format(MSG0027E, "ＭＴ装置", "アンロード"),
                '                                    msgTitle,
                '                                    MessageBoxButtons.OK,
                '                                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                '                End If
                '                fn_BAITAI_WRITE = False
                '                Exit Function
                '            End If

                '        End If
                '        '2010.03.04 NHKは事前検証をしない end
                '        '------------------------------------
                '        '媒体に書き込み
                '        '------------------------------------
                '        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                '        strKEKKA = vbDLL.cmtCPYtoCMT_CHK(intBlkSize,
                '                                         intREC_LENGTH,
                '                                         CInt(strcTORI_PARAM.LABEL_KBN),
                '                                         "SLMT", 1, 4, strIN_FILE_NAME, " ", 0, lngErrStatus)
                '        'strKEKKA = vbDLL.cmtCPYtoCMT_CHK(CShort(intBlkSize),
                '        '                                 CShort(intREC_LENGTH),
                '        '                                 CShort(strcTORI_PARAM.LABEL_KBN),
                '        '                                 "SLMT", 1, 4, strIN_FILE_NAME, " ", 0, CInt(lngErrStatus))
                '        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                '        If strKEKKA <> "" Then
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "失敗 ", "ＣＭＴ書込み:" & strKEKKA)
                '            LOG.UpdateJOBMASTbyErr("ＣＭＴ書込み:" & strKEKKA)
                '            Exit Function
                '        Else
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み")
                '        End If
                '    Case "1"    'ＣＭＴが自振サーバに接続していない場合
                '        If strcINI_PARAM.CMT_OUTFILE = "" Then
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み(ファイル名取得)失敗")
                '            LOG.UpdateJOBMASTbyErr("ＣＭＴ書込み(ファイル名取得)失敗")
                '            Exit Function
                '        End If
                '        If Dir(strcINI_PARAM.CMT_OUTFILE) <> "" Then
                '            Kill(strcINI_PARAM.CMT_OUTFILE)
                '        End If
                '        FileCopy(strIN_FILE_NAME, strcINI_PARAM.CMT_OUTFILE)
                '        If Err.Number = 0 Then
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み(ファイルコピー)")
                '        Else
                '            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoCMT_CHK", "成功 ", "ＣＭＴ書込み(ファイルコピー)失敗")
                '            LOG.UpdateJOBMASTbyErr("ＣＭＴ書込み(ファイルコピー)失敗")
                '            Exit Function
                '        End If
                'End Select
                '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- END
                Case "07"       '学校
                    '2012/06/30 標準版　WEB伝送対応
                Case "10"       'WEB伝送
                    Dim SQL As String

                    SQL = "SELECT * "
                    SQL += " FROM WEB_RIREKIMAST"
                    SQL += " WHERE TORIS_CODE_W = '" & strcKOBETUPARAM.strTORIS_CODE & "'"
                    SQL += " AND TORIF_CODE_W = '" & strcKOBETUPARAM.strTORIF_CODE & "'"
                    SQL += " AND FURI_DATE_W = '" & strcKOBETUPARAM.strFURI_DATE & "'"
                    SQL += " AND FSYORI_KBN_W = '1'"

                    Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                    If OraReader.DataReader(SQL) = False Then
                        OraReader.Close()

                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "WEB履歴マスタ検索", "失敗 ", "")
                        LOG.UpdateJOBMASTbyErr("WEB履歴マスタ検索　失敗")
                        Return False
                    End If

                    strOUT_END_FILE_NAME = strcINI_PARAM.WEB_SED_FOLDER & "END_" & OraReader.GetString("USER_ID_W") & "_" & OraReader.GetString("FILE_NAME_W") & "_" & strcKOBETUPARAM.strFURI_DATE

                    strOUT_FILE_NAME = strcINI_PARAM.WEB_SED_FOLDER & OraReader.GetString("USER_ID_W") & "_" & OraReader.GetString("FILE_NAME_W") & "_" & strcKOBETUPARAM.strFURI_DATE     '出力ファイル

                    Select Case strcTORI_PARAM.CODE_KBN
                        Case "0", "1", "4"
                            intKEKKA = clsFUSION.fn_DISK_CPYTO_DEN(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                                   strIN_FILE_NAME,
                                                                   strOUT_FILE_NAME,
                                                                   intREC_LENGTH,
                                                                   strcTORI_PARAM.CODE_KBN,
                                                                   gstrP_FILE)
                            Select Case intKEKKA
                                Case 0
                                    fn_BAITAI_WRITE = True
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "成功 ", "ファイル変換（コード変換）")
                                Case 100
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "失敗 ", "ファイル変換（コード変換）")
                                    LOG.UpdateJOBMASTbyErr("ファイル変換（コード変換）失敗")
                                    Exit Function
                            End Select
                        Case Else
                            If Dir(strOUT_FILE_NAME) <> "" Then
                                Kill(strOUT_FILE_NAME)
                            End If
                            File.Copy(strIN_FILE_NAME, strOUT_FILE_NAME)
                            fn_BAITAI_WRITE = True
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "成功 ", "ファイル変換（コード変換）")
                    End Select

                    If InsertWEB_RIREKIMAST(OraReader.GetString("TORIS_CODE_W"), OraReader.GetString("TORIF_CODE_W"), OraReader.GetString("FURI_DATE_W")) = False Then
                        OraReader.Close()
                        Return False
                    End If
                    OraReader.Close()

                Case "11", "12", "13", "14", "15"       'DVD書込み
                    Select Case strcINI_PARAM.RSV2_EDITION
                        Case "2"
                            '-----------------------------------
                            ' 出力ファイル名設定
                            '-----------------------------------
                            If strcTORI_PARAM.MULTI_KBN = "1" Then
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcTORI_PARAM.ITAKU_KANRI_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            Else
                                strOUT_FILE_NAME = strcINI_PARAM.COMMON_BAITAIWRITE & "O" & strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & "_" & strcKOBETUPARAM.strFURI_DATE & ".dat"
                            End If

                            '-----------------------------------
                            ' ファイル出力処理
                            '-----------------------------------
                            If Dir(strOUT_FILE_NAME) <> "" Then
                                Kill(strOUT_FILE_NAME)
                            End If
                            File.Copy(strIN_FILE_NAME, strOUT_FILE_NAME)
                            fn_BAITAI_WRITE = True
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_DEN", "成功 ", "ファイル変換（コード変換）")

                        Case Else
                            strOUT_FILE_NAME = strcTORI_PARAM.FILE_NAME.Trim

                            intKEKKA = clsFUSION.fn_DVD_CPYTO_DISK(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                   strOUT_FILE_NAME,
                                                   strCHECK_SEND_FILE,
                                                   intREC_LENGTH,
                                                   strcTORI_PARAM.CODE_KBN,
                                                   gstrP_FILE, msgTitle,
                                                   WriteBaitai)

                            Select Case intKEKKA
                                Case 0
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "成功 ", "外部媒体取込（コード変換）")

                                Case 100
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "外部媒体取込（コード変換）")
                                    LOG.UpdateJOBMASTbyErr("外部媒体読み込み失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 200
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "外部媒体取込（コード区分異常（JIS改行あり））")
                                    LOG.UpdateJOBMASTbyErr("外部媒体取込（コード区分異常（JIS改行あり））失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 300
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "外部媒体取込（コード区分異常（JIS改行なし））")
                                    LOG.UpdateJOBMASTbyErr("外部媒体取込（コード区分異常（JIS改行なし））失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 400
                                    fn_BAITAI_WRITE = False
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "外部媒体取込（出力ファイル作成）失敗")
                                    LOG.UpdateJOBMASTbyErr("外部媒体取込（出力ファイル作成）失敗")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                            End Select

                            '------------------------------------
                            '媒体内のファイルのチェック
                            '------------------------------------
                            If fn_CHK_BAITAI(strCHECK_SEND_FILE, strIN_FILE_NAME, intREC_LENGTH) = False Then
                                fn_BAITAI_WRITE = False
                                Exit Function
                            End If

                            '------------------------------------
                            '媒体に書き込み
                            '------------------------------------
                            intKEKKA = clsFUSION.fn_DISK_CPYTO_DVD(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                   strIN_FILE_NAME,
                                                   strOUT_FILE_NAME,
                                                   intREC_LENGTH,
                                                   strcTORI_PARAM.CODE_KBN, gstrP_FILE, False, msgTitle,
                                                   strcTORI_PARAM.BAITAI_CODE)
                            Select Case intKEKKA
                                Case 0
                                    fn_BAITAI_WRITE = True
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_FD", "成功 ", "外部媒体書込み")

                                Case 100
                                    fn_BAITAI_WRITE = False
                                    'Return         :0=成功、100=ＦＤ書込み失敗（IBM形式）、200=ＦＤ書込み失敗（DOS形式）
                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_FD", "失敗 ", "外部媒体書込み失敗（EBCDIC）")
                                    LOG.UpdateJOBMASTbyErr("外部媒体書込み失敗（EBCDIC）")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                                Case 200
                                    fn_BAITAI_WRITE = False

                                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DISK_CPYTO_FD", "失敗 ", "外部媒体書込み失敗(JIS）")

                                    LOG.UpdateJOBMASTbyErr("外部媒体書込み失敗（JIS）")
                                    If Dir(strCHECK_SEND_FILE) <> "" Then
                                        Kill(strCHECK_SEND_FILE)
                                    End If
                                    Exit Function
                            End Select

                    End Select
            End Select

            If Dir(strCHECK_SEND_FILE) <> "" Then
                Kill(strCHECK_SEND_FILE)
            End If

            fn_BAITAI_WRITE = True

        Catch ex As Exception
            LOG.Write("媒体書込処理", "失敗", ex)
            LOG.UpdateJOBMASTbyErr("媒体書込処理 失敗")
            Return False
        Finally
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "媒体書込処理(終了)", "成功")
        End Try

    End Function

    ' 機能　 ： 返還データ作成（依頼書・伝票）
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function MakeHenkanData_IRAISYO(ByRef Henkyakuarray As ArrayList) As Boolean

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 依頼書・伝票フォーマット(開始)", "成功")

            'スケジュールマスタの検索を行う
            Dim SQL As New StringBuilder
            SQL.Append("SELECT *")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(", SCHMAST")
            SQL.Append(" WHERE TORIMAST.TORIS_CODE_T = SCHMAST.TORIS_CODE_S")
            SQL.Append(" AND TORIMAST.TORIF_CODE_T = SCHMAST.TORIF_CODE_S")
            Select Case strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE
                Case "111111111111"
                    SQL.Append(" AND SCHMAST.BAITAI_CODE_S = '04'")
                Case "222222222222"
                    SQL.Append(" AND SCHMAST.BAITAI_CODE_S = '09'")
                Case Else
                    SQL.Append(" AND SCHMAST.TORIS_CODE_S = '" & Trim(strcKOBETUPARAM.strTORIS_CODE) & "'")
                    SQL.Append(" AND SCHMAST.TORIF_CODE_S = '" & Trim(strcKOBETUPARAM.strTORIF_CODE) & "'")
                    SQL.Append(" AND SCHMAST.MOTIKOMI_SEQ_S = '" & strcKOBETUPARAM.strMOTIKOMI_SEQ & "'")
            End Select
            SQL.Append(" AND SCHMAST.FURI_DATE_S = '" & strcKOBETUPARAM.strFURI_DATE & "'")
            SQL.Append(" AND SCHMAST.FUNOU_FLG_S = '1'")
            SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TORIMAST.KEKKA_HENKYAKU_KBN_T <> '0'")
            SQL.Append(" ORDER BY SCHMAST.TORIS_CODE_S,SCHMAST.TORIF_CODE_S")
            SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime3)

            intRECORD_COUNT = 0
            dblALL_KEN = 0
            dblALL_KIN = 0

            Dim SchReader As New CASTCommon.MyOracleReader(MainDB)

            If Not SchReader.DataReader(SQL) Then
                If SchReader.Message <> "" Then
                    Dim errmsg As String
                    If SchReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "返還データ作成（依頼書）実行待ちタイムアウト"
                    Else
                        errmsg = "返還データ作成（依頼書）処理ロック異常"
                    End If

                    SchReader.Close()
                    SchReader = Nothing
                    LOG.Write_Err("返還データ作成（依頼書）処理", "失敗", errmsg)

                    LOG.UpdateJOBMASTbyErr(errmsg)
                    Return False
                End If

                SchReader.Close()
                SchReader = Nothing

                LOG.Write("返還データ作成（依頼書）処理", "失敗", "スケジュール検索失敗")
                LOG.UpdateJOBMASTbyErr("返還データ作成（依頼書）処理 失敗")
                Return False
            End If

            Dim Key As New KeyInfo

            While SchReader.EOF = False
                Key.Init()
                Call Key.SetOracleData(SchReader)

                If fn_SCHMAST_UPDATE(Key) = False Then
                    Exit Function
                End If

                Henkyakuarray.Add(Key)

                SchReader.NextRead()
            End While

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 依頼書・伝票フォーマット(終了)", "成功")
            Return True

        Catch ex As Exception
            LOG.Write("返還データ作成（依頼書）処理", "失敗", ex.Message)
            LOG.Write("トレース", "成功", CASTCommon.MakeLogTrace)
            LOG.UpdateJOBMASTbyErr("返還データ作成（依頼書）処理 失敗")
            Return False
        End Try
    End Function

    ' 機能　 ： 返還データ作成（国税）390バイト
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Function MakeHenkanData_KOKUZEI(ByRef HenkyakuArray As ArrayList,
                                            ByRef UnmatchArray As ArrayList) As Boolean


        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 国税フォーマット(開始)", "成功")

            Dim Key As New KeyInfo

            'スケジュールマスタの検索を行う
            Dim SQL As New StringBuilder
            SQL.Append("SELECT *")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(", SCHMAST")
            SQL.Append(" WHERE TORIMAST.ITAKU_KANRI_CODE_T = '" & strcTORI_PARAM.ITAKU_KANRI_CODE & "'")
            SQL.Append(" AND TORIMAST.TORIS_CODE_T = SCHMAST.TORIS_CODE_S")
            SQL.Append(" AND TORIMAST.TORIF_CODE_T = SCHMAST.TORIF_CODE_S")
            SQL.Append(" AND SCHMAST.FURI_DATE_S = '" & strcKOBETUPARAM.strFURI_DATE & "'")
            SQL.Append(" AND SCHMAST.FUNOU_FLG_S = '1'")
            SQL.Append(" AND SCHMAST.MOTIKOMI_SEQ_S = '" & strcKOBETUPARAM.strMOTIKOMI_SEQ & "'")
            SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
            SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime3)


            Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
            Dim blnSQL As Boolean = False

            Dim intSch_COUNT As Integer = 0
            Dim intFILE_NO_1 As Integer
            Dim strFURI_DATA_390 As String = ""
            intFILE_NO_1 = FreeFile()
            intRECORD_COUNT = 0
            dblALL_KEN = 0
            dblALL_KIN = 0

            blnSQL = OraSchReader.DataReader(SQL)

            If blnSQL = True Then
                Do
                    Key.Init()
                    Call Key.SetOracleData(OraSchReader)

                    intSch_COUNT += 1
                    If intSch_COUNT = 1 Then
                        FileOpen(intFILE_NO_1, strIN_FILE_NAME, OpenMode.Random, , , 390)
                    End If

                    SQL.Length = 0
                    SQL.Append("SELECT *")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE TORIS_CODE_K = '" & OraSchReader.GetString("TORIS_CODE_S") & "'")
                    SQL.Append(" AND TORIF_CODE_K = '" & OraSchReader.GetString("TORIF_CODE_S") & "'")
                    SQL.Append(" AND FURI_DATE_K = '" & OraSchReader.GetString("FURI_DATE_S") & "'")
                    SQL.Append(" ORDER BY RECORD_NO_K ASC")

                    Dim MeiReader As New CASTCommon.MyOracleReader(MainDB)

                    If Not MeiReader.DataReader(SQL) Then
                        '明細無しでも処理なし？？？
                        '取り合えず
                        LOG.Write("返還データ作成（国税）処理", "失敗", "明細マスタ取得失敗")

                        LOG.UpdateJOBMASTbyErr("返還データ作成（国税）処理 失敗")
                        FileClose(intFILE_NO_1)
                        Return False
                    End If

                    While MeiReader.EOF = False

                        intRECORD_COUNT += 1

                        Select Case MeiReader.GetItem("DATA_KBN_K")

                            Case "1"
                                dblFURI_KEN = 0
                                dblFURI_KIN = 0
                                dblFUNOU_KEN = 0
                                dblFURI_KIN = 0
                                gstrDATA_390.strDATA = MeiReader.GetItem("FURI_DATA_K")
                                FilePut(intFILE_NO_1, gstrDATA_390, intRECORD_COUNT)
                            Case "2"
                                gstrDATA_390.strDATA = MeiReader.GetItem("FURI_DATA_K")
                                FilePut(intFILE_NO_1, gstrDATA_390, intRECORD_COUNT)
                            Case "3"         'データ部
                                If MeiReader.GetString("FURIKETU_CODE_K") <> "0" Then
                                    '不能
                                    dblFUNOU_KEN += 1
                                    dblFUNOU_KIN += MeiReader.GetInt64("FURIKIN_K")
                                    dblALL_KEN += 1
                                    dblALL_KIN += MeiReader.GetInt64("FURIKIN_K")
                                    dblSIT_TOTAL_FUNOU_KEN += 1
                                    dblSIT_TOTAL_FUNOU_KIN += MeiReader.GetInt64("FURIKIN_K")
                                    dblZEIMUSYO_TOTAL_FUNOU_KEN += 1
                                    dblZEIMUSYO_TOTAL_FUNOU_KIN += MeiReader.GetInt64("FURIKIN_K")
                                Else
                                    '振替済み
                                    dblFURI_KEN += 1
                                    dblFURI_KIN += MeiReader.GetInt64("FURIKIN_K")
                                    dblALL_KEN += 1
                                    dblALL_KIN += MeiReader.GetInt64("FURIKIN_K")
                                    dblSIT_TOTAL_FURI_KEN += 1
                                    dblSIT_TOTAL_FURI_KIN += MeiReader.GetInt64("FURIKIN_K")
                                    dblZEIMUSYO_TOTAL_FURI_KEN += 1
                                    dblZEIMUSYO_TOTAL_FURI_KIN += MeiReader.GetInt64("FURIKIN_K")
                                End If

                                gKOKUZEI_REC3.Data = MeiReader.GetItem("FURI_DATA_K")
                                '振替結果コード設定
                                gKOKUZEI_REC3.KZ8 = MeiReader.GetString("FURIKETU_CODE_K")
                                gstrDATA_390.strDATA = gKOKUZEI_REC3.Data
                                FilePut(intFILE_NO_1, gstrDATA_390, intRECORD_COUNT)

                            Case "4", "5"   '4:支店トータル、5:税務署トータル
                                gKOKUZEI_REC2.Data = MeiReader.GetItem("FURI_DATA_K")
                                gKOKUZEI_REC2.KZ15 = Format(dblSIT_TOTAL_FUNOU_KEN, "#####0").PadLeft(6, " "c)
                                gKOKUZEI_REC2.KZ16 = Format(dblSIT_TOTAL_FUNOU_KIN, "###########0").PadLeft(12, " "c)
                                gKOKUZEI_REC2.KZ17 = Format(dblSIT_TOTAL_FURI_KEN, "#####0").PadLeft(6, " "c)
                                gKOKUZEI_REC2.KZ18 = Format(dblSIT_TOTAL_FURI_KIN, "###########0").PadLeft(12, " "c)
                                gstrDATA_390.strDATA = gKOKUZEI_REC2.Data

                                FilePut(intFILE_NO_1, gstrDATA_390, intRECORD_COUNT)

                                If MeiReader.GetString("DATA_KBN_K") = "4" Then
                                    '4:支店トータル初期化
                                    dblSIT_TOTAL_FURI_KEN = 0
                                    dblSIT_TOTAL_FURI_KIN = 0
                                    dblSIT_TOTAL_FUNOU_KEN = 0
                                    dblSIT_TOTAL_FUNOU_KIN = 0
                                Else
                                    '5:税務署トータル初期化
                                    dblZEIMUSYO_TOTAL_FURI_KEN = 0
                                    dblZEIMUSYO_TOTAL_FURI_KIN = 0
                                    dblZEIMUSYO_TOTAL_FUNOU_KEN = 0
                                    dblZEIMUSYO_TOTAL_FUNOU_KIN = 0
                                End If

                            Case "8"   'トレーラ部
                                If OraSchReader.GetInt64("SYORI_KEN_S") <> dblFURI_KEN + dblFUNOU_KEN Then
                                    LOG.Write("トレーラチェック", "成功", "処理件数 不一致")
                                    FileClose(intFILE_NO_1)
                                    UnmatchArray.Add(Key)
                                    Return False
                                End If
                                If OraSchReader.GetInt64("SYORI_KIN_S") <> dblFURI_KIN + dblFUNOU_KIN Then
                                    LOG.Write("トレーラチェック", "成功", "処理金額 不一致")
                                    FileClose(intFILE_NO_1)
                                    UnmatchArray.Add(Key)
                                    Return False
                                End If
                                gKOKUZEI_REC8.Data = MeiReader.GetItem("FURI_DATA_K")
                                gKOKUZEI_REC8.KZ9 = Format(dblFUNOU_KEN, "#####0").PadLeft(6, " "c)
                                gKOKUZEI_REC8.KZ10 = Format(dblFUNOU_KIN, "###########0").PadLeft(12, " "c)
                                gKOKUZEI_REC8.KZ11 = Format(dblFURI_KEN, "#####0").PadLeft(6, " "c)
                                gKOKUZEI_REC8.KZ12 = Format(dblFURI_KIN, "###########0").PadLeft(12, " "c)
                                gstrDATA_390.strDATA = gKOKUZEI_REC8.Data

                                FilePut(intFILE_NO_1, gstrDATA_390, intRECORD_COUNT)

                                If UnmatchArray.Count > 0 Then
                                    FileClose(intFILE_NO_1)
                                    Exit Function
                                Else
                                    If fn_SCHMAST_UPDATE(Key) = False Then
                                        FileClose(intFILE_NO_1)
                                        Exit Function
                                    End If
                                End If

                                HenkyakuArray.Add(Key)
                            Case "9"
                                gstrDATA_390.strDATA = MeiReader.GetItem("FURI_DATA_K")
                                FilePut(intFILE_NO_1, gstrDATA_390, intRECORD_COUNT)
                        End Select

                        MeiReader.NextRead()
                    End While

                    OraSchReader.NextRead()
                Loop Until OraSchReader.EOF = True

            Else
                If OraSchReader.Message <> "" Then
                    Dim errmsg As String
                    If OraSchReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "返還データ作成（国税フォーマット）実行待ちタイムアウト"
                    Else
                        errmsg = "返還データ作成（国税フォーマット）処理ロック異常"
                    End If

                    FileClose(intFILE_NO_1)
                    OraSchReader.Close()
                    OraSchReader = Nothing
                    LOG.Write_Err("返還データ作成（国税フォーマット）処理", "失敗", errmsg)

                    LOG.UpdateJOBMASTbyErr(errmsg)
                    Return False
                End If

            End If

            FileClose(intFILE_NO_1)

            OraSchReader.Close()

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 国税フォーマット(終了)", "成功")
            Return True

        Catch ex As Exception
            LOG.Write("返還データ作成（国税）処理", "失敗", ex.Message)
            LOG.Write("トレース", "成功", CASTCommon.MakeLogTrace)
            LOG.UpdateJOBMASTbyErr("返還データ作成（国税）処理 失敗")
            Return False
        End Try

    End Function

    ' 機能　 ： 返還データ作成（ＮＨＫ）
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    Private Function MakeHenkanData_NHK(ByRef HenkyakuArray As ArrayList,
                                        ByRef UnmatchArray As ArrayList) As Boolean

        Try

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 ＮＨＫフォーマット(開始)", "成功")

            Dim Key As New KeyInfo
            Dim HenkanFMT As New CAstFormat.CFormat
            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = strcTORI_PARAM.FMT_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)

            Dim Comm As New CAstBatch.CommData(MainDB)
            HenkanFMT.Oracle = MainDB
            Dim HenkanStream As StreamWriter = Nothing
            HenkanStream = New StreamWriter(strIN_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim EndRecord As String = ""

            Dim NoBinaryData As Boolean = False
            Dim NewKey As New KeyInfo
            Dim OldKey As New KeyInfo
            NewKey.Init()
            OldKey.Init()
            Dim intWriteLength As Integer = 0

            Dim SQL As New StringBuilder
            SQL.Append("SELECT *")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(", SCHMAST")
            SQL.Append(" WHERE TORIMAST.ITAKU_KANRI_CODE_T = '" & strcTORI_PARAM.ITAKU_KANRI_CODE & "'")
            SQL.Append(" AND TORIMAST.TORIS_CODE_T = SCHMAST.TORIS_CODE_S")
            SQL.Append(" AND TORIMAST.TORIF_CODE_T = SCHMAST.TORIF_CODE_S")
            SQL.Append(" AND SCHMAST.FURI_DATE_S = '" & strcKOBETUPARAM.strFURI_DATE & "'")
            SQL.Append(" AND SCHMAST.FUNOU_FLG_S = '1'")
            SQL.Append(" AND SCHMAST.MOTIKOMI_SEQ_S = '" & strcKOBETUPARAM.strMOTIKOMI_SEQ & "'")
            SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
            SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime3)

            Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
            Dim blnSQL As Boolean = False

            blnSQL = OraSchReader.DataReader(SQL)

            If blnSQL = True Then
                Do
                    para.TORIS_CODE = OraSchReader.GetString("TORIS_CODE_S")
                    para.TORIF_CODE = OraSchReader.GetString("TORIF_CODE_S")
                    para.FURI_DATE = OraSchReader.GetString("FURI_DATE_S")

                    Comm.INFOParameter = para

                    ' 取引先マスタ取得
                    Call Comm.GetTORIMAST(para.TORIS_CODE,
                                          para.TORIF_CODE)
                    HenkanFMT.ToriData = Comm

                    If HenkanFMT.FirstRead_Henkan() = 0 Then
                        LOG.Write("明細マスタ", "失敗")
                        Return Nothing
                    End If

                    Dim sRet As String = ""
                    Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
                    stTori = Comm.INFOToriMast

                    Do Until HenkanFMT.EOF(1)

                        Key.Init()
                        NewKey.Init()
                        Call Key.SetOracleData(OraSchReader)
                        Call NewKey.SetOracleData(OraSchReader)
                        Select Case NewKey.CODE_KBN
                            Case "0", "1"
                                intWriteLength = 120
                            Case "2"
                                intWriteLength = 119
                            Case "3"
                                intWriteLength = 118
                            Case Else
                                intWriteLength = 120
                        End Select

                        sRet = HenkanFMT.CheckHenkanFormat()
                        'ヘッダ
                        If HenkanFMT.IsHeaderRecord Then

                            Call HenkanFMT.CheckRecord1()

                        ElseIf HenkanFMT.IsTrailerRecord Then
                            ' トレーラ
                            ' トレーラ件数金額更新
                            Call HenkanFMT.GetHenkanTrailerRecord()

                            If HenkanFMT.InfoMeisaiMast.TOTAL_KEN <> OraSchReader.GetInt64("SYORI_KEN_S") And
                            HenkanFMT.InfoMeisaiMast.TOTAL_KEN2 <> OraSchReader.GetInt64("SYORI_KEN_S") Then
                                LOG.Write("トレーラチェック", "成功", "請求件数 不一致")
                                Key.MESSAGE = "請求件数 不一致"
                                UnmatchArray.Add(Key)
                            ElseIf HenkanFMT.InfoMeisaiMast.TOTAL_KIN <> OraSchReader.GetInt64("SYORI_KIN_S") Then
                                LOG.Write("トレーラチェック", "成功", "請求金額 不一致")
                                Key.MESSAGE = "請求金額 不一致"
                                UnmatchArray.Add(Key)
                            End If

                            If HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KEN <> OraSchReader.GetInt64("FUNOU_KEN_S") Then
                                LOG.Write("トレーラチェック", "成功", "不能件数 不一致")
                                Key.MESSAGE = "不能件数 不一致"
                                UnmatchArray.Add(Key)
                            ElseIf HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KIN <> OraSchReader.GetInt64("FUNOU_KIN_S") Then
                                LOG.Write("トレーラチェック", "成功", "不能金額 不一致")
                                Key.MESSAGE = "不能金額 不一致"
                                UnmatchArray.Add(Key)
                            End If

                            dblFURI_KEN = HenkanFMT.InfoMeisaiMast.TOTAL_NORM_KEN2
                            dblFURI_KIN = HenkanFMT.InfoMeisaiMast.TOTAL_NORM_KIN
                            dblFUNOU_KEN = HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KEN
                            dblFUNOU_KIN = HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KIN

                            If UnmatchArray.Count > 0 Then
                                HenkanStream.Close()
                                HenkanStream = Nothing
                                OraSchReader.Close()
                                Exit Function
                            Else
                                If fn_SCHMAST_UPDATE(Key) = False Then
                                    HenkanStream.Close()
                                    HenkanStream = Nothing
                                    OraSchReader.Close()
                                    Exit Function
                                End If
                            End If

                            HenkyakuArray.Add(Key)

                        ElseIf HenkanFMT.IsEndRecord Then
                            ' エンド

                        ElseIf HenkanFMT.IsDataRecord Then
                            'データレコード
                            Call HenkanFMT.GetHenkanDataRecord()

                            If Key.KEKKA_MEISAI_KBN = "1" AndAlso
                               HenkanFMT.InfoMeisaiMast.FURIKETU_CODE = 0 Then

                                HenkanFMT.RecordData = ""
                            Else
                            End If
                        End If

                        If HenkanFMT.RecordData.Trim <> "" Then
                            HenkanStream.Write(HenkanFMT.RecordData.Substring(0, intWriteLength))
                        End If

                    Loop

                    OraSchReader.NextRead()

                Loop Until OraSchReader.EOF = True

            Else
                If OraSchReader.Message <> "" Then
                    Dim errmsg As String
                    If OraSchReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "返還データ作成（ＮＨＫフォーマット）実行待ちタイムアウト"
                    Else
                        errmsg = "返還データ作成（ＮＨＫフォーマット）処理ロック異常"
                    End If

                    HenkanStream.Close()
                    HenkanStream = Nothing
                    OraSchReader.Close()
                    OraSchReader = Nothing

                    LOG.Write_Err("返還データ作成（ＮＨＫフォーマット）処理", "失敗", errmsg)

                    LOG.UpdateJOBMASTbyErr(errmsg)
                    Return False
                End If

            End If

            HenkanStream.Close()
            HenkanStream = Nothing

            OraSchReader.Close()

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 ＮＨＫフォーマット(終了)", "成功")

            Return True

        Catch ex As Exception
            LOG.Write("返還データ作成（ＮＨＫ）処理", "失敗", ex.Message)
            LOG.Write("トレース", "成功", CASTCommon.MakeLogTrace)
            LOG.UpdateJOBMASTbyErr("返還データ作成（ＮＨＫ）処理 失敗")
            Return False
        End Try
    End Function

    ' 機能　 ： 返還データ作成（全銀）
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    Private Function MakeHenkanData_ZENGIN(ByRef HenkyakuArray As ArrayList,
                                           ByRef UnmatchArray As ArrayList) As Boolean

        Try

            Dim Key As New KeyInfo
            Dim HenkanFMT As New CAstFormat.CFormat
            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = strcTORI_PARAM.FMT_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)

            Dim Comm As New CAstBatch.CommData(MainDB)
            HenkanFMT.Oracle = MainDB
            Dim HenkanStream As StreamWriter = Nothing
            HenkanStream = New StreamWriter(strIN_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim EndRecord As String = ""

            Dim NoBinaryData As Boolean = False
            Dim NewKey As New KeyInfo
            Dim OldKey As New KeyInfo
            NewKey.Init()
            OldKey.Init()
            Dim intWriteLength As Integer = 0

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 全銀フォーマット(開始)", "成功")

            Dim SQL As New StringBuilder
            SQL.Append("SELECT *")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(", SCHMAST")
            SQL.Append(" WHERE TORIMAST.ITAKU_KANRI_CODE_T = '" & strcTORI_PARAM.ITAKU_KANRI_CODE & "'")
            SQL.Append(" AND TORIMAST.TORIS_CODE_T = SCHMAST.TORIS_CODE_S")
            SQL.Append(" AND TORIMAST.TORIF_CODE_T = SCHMAST.TORIF_CODE_S")
            SQL.Append(" AND SCHMAST.FURI_DATE_S = '" & strcKOBETUPARAM.strFURI_DATE & "'")
            SQL.Append(" AND SCHMAST.FUNOU_FLG_S = '1'")
            SQL.Append(" AND SCHMAST.MOTIKOMI_SEQ_S = " & CInt(strcKOBETUPARAM.strMOTIKOMI_SEQ))
            SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
            SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime3)

            Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
            Dim blnSQL As Boolean = False

            blnSQL = OraSchReader.DataReader(SQL)

            If blnSQL = True Then
                Do
                    para.TORIS_CODE = OraSchReader.GetString("TORIS_CODE_S")
                    para.TORIF_CODE = OraSchReader.GetString("TORIF_CODE_S")
                    para.FURI_DATE = OraSchReader.GetString("FURI_DATE_S")

                    Comm.INFOParameter = para

                    ' 取引先マスタ取得
                    Call Comm.GetTORIMAST(para.TORIS_CODE,
                                          para.TORIF_CODE)
                    HenkanFMT.ToriData = Comm

                    If HenkanFMT.FirstRead_Henkan() = 0 Then
                        LOG.Write("明細マスタ", "失敗")
                        Return Nothing
                    End If

                    Dim sRet As String = ""
                    Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
                    stTori = Comm.INFOToriMast

                    Do Until HenkanFMT.EOF(1)

                        Key.Init()
                        NewKey.Init()
                        Call Key.SetOracleData(OraSchReader)
                        Call NewKey.SetOracleData(OraSchReader)
                        Select Case NewKey.CODE_KBN
                            Case "0", "1"
                                intWriteLength = 120
                            Case "2"
                                intWriteLength = 119
                            Case "3"
                                intWriteLength = 118
                            Case Else
                                intWriteLength = 120
                        End Select

                        sRet = HenkanFMT.CheckHenkanFormat()
                        'ヘッダ
                        If HenkanFMT.IsHeaderRecord Then

                            Call HenkanFMT.CheckRecord1()

                        ElseIf HenkanFMT.IsTrailerRecord Then
                            ' トレーラ
                            ' トレーラ件数金額更新
                            Call HenkanFMT.GetHenkanTrailerRecord()

                            If HenkanFMT.InfoMeisaiMast.TOTAL_KEN <> OraSchReader.GetInt64("SYORI_KEN_S") And
                                HenkanFMT.InfoMeisaiMast.TOTAL_KEN2 <> OraSchReader.GetInt64("SYORI_KEN_S") Then
                                LOG.Write("トレーラチェック", "成功", "請求件数 不一致")
                                Key.MESSAGE = "請求件数 不一致"
                                UnmatchArray.Add(Key)
                            ElseIf HenkanFMT.InfoMeisaiMast.TOTAL_KIN <> OraSchReader.GetInt64("SYORI_KIN_S") Then
                                LOG.Write("トレーラチェック", "成功", "請求金額 不一致")
                                Key.MESSAGE = "請求金額 不一致"
                                UnmatchArray.Add(Key)
                            End If

                            If HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KEN <> OraSchReader.GetInt64("FUNOU_KEN_S") Then
                                LOG.Write("トレーラチェック", "成功", "不能件数 不一致")
                                Key.MESSAGE = "不能件数 不一致"
                                UnmatchArray.Add(Key)
                            ElseIf HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KIN <> OraSchReader.GetInt64("FUNOU_KIN_S") Then
                                LOG.Write("トレーラチェック", "成功", "不能金額 不一致")
                                Key.MESSAGE = "不能金額 不一致"
                                UnmatchArray.Add(Key)
                            End If

                            dblFURI_KEN = HenkanFMT.InfoMeisaiMast.TOTAL_NORM_KEN
                            dblFURI_KIN = HenkanFMT.InfoMeisaiMast.TOTAL_NORM_KIN
                            dblFUNOU_KEN = HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KEN
                            dblFUNOU_KIN = HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KIN

                            If UnmatchArray.Count > 0 Then
                                HenkanStream.Close()
                                HenkanStream = Nothing
                                OraSchReader.Close()
                                Exit Function
                            Else
                                If fn_SCHMAST_UPDATE(Key) = False Then
                                    HenkanStream.Close()
                                    HenkanStream = Nothing
                                    OraSchReader.Close()
                                    Exit Function
                                End If

                                HenkyakuArray.Add(Key)
                            End If

                        ElseIf HenkanFMT.IsEndRecord Then
                            ' エンド

                        ElseIf HenkanFMT.IsDataRecord Then
                            'データレコード
                            Call HenkanFMT.GetHenkanDataRecord()

                            If Key.KEKKA_MEISAI_KBN = "1" AndAlso
                                HenkanFMT.InfoMeisaiMast.FURIKETU_CODE = 0 Then

                                HenkanFMT.RecordData = ""
                            Else
                            End If
                        End If

                        If HenkanFMT.RecordData.Trim <> "" Then
                            Select Case NewKey.CODE_KBN
                                Case "2"
                                    HenkanStream.Write(HenkanFMT.RecordData.Substring(0, intWriteLength) & vbCr)
                                Case "3"
                                    HenkanStream.Write(HenkanFMT.RecordData.Substring(0, intWriteLength) & vbCrLf)
                                Case Else
                                    HenkanStream.Write(HenkanFMT.RecordData)
                            End Select
                        End If

                    Loop

                    OraSchReader.NextRead()

                Loop Until OraSchReader.EOF = True

            Else
                If OraSchReader.Message <> "" Then
                    Dim errmsg As String
                    If OraSchReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "返還データ作成（全銀フォーマット）実行待ちタイムアウト"
                    Else
                        errmsg = "返還データ作成（全銀フォーマット）処理ロック異常"
                    End If

                    HenkanStream.Close()
                    HenkanStream = Nothing
                    OraSchReader.Close()
                    OraSchReader = Nothing

                    LOG.Write_Err("返還データ作成（全銀フォーマット）処理", "失敗", errmsg)

                    LOG.UpdateJOBMASTbyErr(errmsg)
                    Return False
                End If

            End If

            HenkanStream.Close()
            HenkanStream = Nothing

            OraSchReader.Close()

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "返還データ作成 全銀フォーマット(終了)", "成功")
            Return True

        Catch ex As Exception
            LOG.Write("返還データ作成（全銀）処理", "失敗", ex.Message)
            LOG.Write("トレース", "成功", CASTCommon.MakeLogTrace)
            LOG.UpdateJOBMASTbyErr("返還データ作成（全銀）処理 失敗")
            Return False

        End Try
    End Function

    '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
    ' 機能　 ： 返還データ作成（XMLフォーマット）
    '
    ' 引数　 ： HenkyakuArray - 処理結果一覧
    '           UnmatchArray  - 不一致一覧
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    Private Function MakeHenkanData_XML(ByRef HenkyakuArray As ArrayList,
                                           ByRef UnmatchArray As ArrayList) As Boolean

        Try
            Dim sw As System.Diagnostics.Stopwatch = Nothing
            sw = LOG.Write_Enter1(LOG.ToriCode, LOG.FuriDate, "返還データ作成（XMLフォーマット）", "")

            Dim Key As New KeyInfo
            Dim HenkanFMT As New CAstFormat.CFormat
            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = strcTORI_PARAM.FMT_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)
            HenkanFMT.LOG = LOG

            Dim Comm As New CAstBatch.CommData(MainDB)
            HenkanFMT.Oracle = MainDB
            Dim HenkanStream As StreamWriter = Nothing
            HenkanStream = New StreamWriter(strIN_FILE_NAME, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim EndRecord As String = ""

            Dim NoBinaryData As Boolean = False
            'V01L09(QA/要望管理[OT-017]) 2019-06-19 DEL 九州農協オンライン対応 -------------------- START
            '不要な変数を削除
            'Dim NewKey As New KeyInfo
            'Dim OldKey As New KeyInfo
            'NewKey.Init()
            'OldKey.Init()
            'V01L09(QA/要望管理[OT-017]) 2019-06-19 DEL 九州農協オンライン対応 -------------------- END
            Dim intWriteLength As Integer = 0

            Dim xmlDoc As New ConfigXmlDocument
            Dim node As XmlNode

            'XMLパス作成
            Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
            If xmlFolderPath = "err" Or xmlFolderPath = "" Then
                Throw New Exception("fskj.iniでXML_FORMAT_FLDが定義されていません。")
            End If
            If xmlFolderPath.EndsWith("\") = False Then
                xmlFolderPath &= "\"
            End If
            mXmlFile = "XML_FORMAT_" & strcTORI_PARAM.FMT_KBN & ".xml"

            ' XMLフォーマットのrootオブジェクト生成
            xmlDoc.Load(xmlFolderPath & mXmlFile)
            mXmlRoot = xmlDoc.DocumentElement

            ' 返還/含0円データ（必須）
            Dim Include0Yen As Integer = 1    ' 1: 0円データを含める
            node = mXmlRoot.SelectSingleNode("返還/含0円データ")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/含0円データ」タグが定義されていません。")
            End If

            Dim sWork As String = node.InnerText.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/含0円データ」タグの値（" & sWork & "）が不当です。（" &
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            Include0Yen = CInt(sWork)

            ' 返還/超過データ書き込み（必須）
            Dim WriteLongRecord As Integer = 1   ' 1: 超過分のデータを書き込む
            node = mXmlRoot.SelectSingleNode("返還/超過データ書き込み")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/超過データ書き込み」タグが定義されていません。")
            End If

            sWork = node.InnerText.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 1 Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/超過データ書き込み」タグの値（" & sWork & "）が不当です。（" &
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            WriteLongRecord = CInt(sWork)

            ' 文字埋め（必須）
            Dim Padding As Integer = 0   ' 0: 文字埋めしない
            node = mXmlRoot.SelectSingleNode("返還/文字埋め")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/文字埋め」タグが定義されていません。")
            End If

            sWork = node.InnerText.Trim
            If IsNumeric(sWork) = False OrElse CInt(sWork) < 0 OrElse CInt(sWork) > 2 Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/文字埋め」タグの値（" & sWork & "）が不当です。（" &
                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
            End If
            Padding = CInt(sWork)

            ' 使用文字（値省略可）
            Dim PaddingChar As Char = " "c  ' デフォルト： 半角空白
            node = mXmlRoot.SelectSingleNode("返還/使用文字")
            If node Is Nothing Then
                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/使用文字」タグが定義されていません。")
            End If

            sWork = node.InnerText.Trim
            If sWork <> "" Then
                If System.Text.Encoding.GetEncoding(932).GetByteCount(sWork) <> 1 Then
                    Throw New Exception(mXmlFile & "定義エラー：" & "「返還/使用文字」タグの値（" & sWork & "）が不当です。（" &
                                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                End If
                PaddingChar = CChar(sWork)
            End If

            Dim SQL As New StringBuilder
            SQL.Append("SELECT *")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(", SCHMAST")
            SQL.Append(" WHERE TORIMAST.ITAKU_KANRI_CODE_T = '" & strcTORI_PARAM.ITAKU_KANRI_CODE & "'")
            SQL.Append(" AND TORIMAST.TORIS_CODE_T = SCHMAST.TORIS_CODE_S")
            SQL.Append(" AND TORIMAST.TORIF_CODE_T = SCHMAST.TORIF_CODE_S")
            SQL.Append(" AND SCHMAST.FURI_DATE_S = '" & strcKOBETUPARAM.strFURI_DATE & "'")
            SQL.Append(" AND SCHMAST.FUNOU_FLG_S = '1'")
            SQL.Append(" AND SCHMAST.MOTIKOMI_SEQ_S = " & CInt(strcKOBETUPARAM.strMOTIKOMI_SEQ))
            SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            SQL.Append(" ORDER BY SCHMAST.FILE_SEQ_S")
            SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime3)

            Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
            Dim blnSQL As Boolean = False

            blnSQL = OraSchReader.DataReader(SQL)

            If blnSQL = True Then
                Do
                    para.TORIS_CODE = OraSchReader.GetString("TORIS_CODE_S")
                    para.TORIF_CODE = OraSchReader.GetString("TORIF_CODE_S")
                    para.FURI_DATE = OraSchReader.GetString("FURI_DATE_S")

                    Comm.INFOParameter = para

                    ' 取引先マスタ取得
                    Call Comm.GetTORIMAST(para.TORIS_CODE,
                                          para.TORIF_CODE)
                    HenkanFMT.ToriData = Comm

                    If HenkanFMT.FirstRead_Henkan() = 0 Then
                        LOG.Write_Err("明細マスタ", "失敗")
                        LOG.UpdateJOBMASTbyErr("返還データ作成（XMLフォーマット）処理 失敗")
                        Return False
                    End If

                    Dim sRet As String = ""
                    Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
                    stTori = Comm.INFOToriMast

                    Do Until HenkanFMT.EOF(1)

                        'V01L09(QA/要望管理[OT-017]) 2019-06-19 DEL 九州農協オンライン対応 -------------------- START
                        'ヘッダレコード読み込み時に取得するように処理を移動
                        'Key.Init()
                        'NewKey.Init()
                        'Call Key.SetOracleData(OraSchReader)
                        'Call NewKey.SetOracleData(OraSchReader)
                        'node = mXmlRoot.SelectSingleNode("返還/コピー設定一覧/コピー設定[@コード区分='" & NewKey.CODE_KBN & "']")
                        'If node Is Nothing Then
                        '    Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定[@コード区分='" & NewKey.CODE_KBN & "']」タグが定義されていません。")
                        'End If

                        'Dim attribute As XmlAttribute = node.Attributes.ItemOf("データ長")
                        'If attribute Is Nothing Then
                        '    Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性が定義されていません。（" &
                        '                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                        'End If
                        'sWork = attribute.Value.Trim
                        'If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                        '    Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性の値（" & sWork & "）が不当です。（" &
                        '                        ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                        'End If

                        'intWriteLength = CInt(sWork)
                        'V01L09(QA/要望管理[OT-017]) 2019-06-19 DEL 九州農協オンライン対応 -------------------- END

                        sRet = HenkanFMT.CheckHenkanFormat()
                        'ヘッダ
                        If HenkanFMT.IsHeaderRecord Then

                            'V01L09(QA/要望管理[OT-017]) 2019-06-19 ADD 九州農協オンライン対応 -------------------- START
                            '上記より処理移動
                            Key.Init()
                            Call Key.SetOracleData(OraSchReader)
                            node = mXmlRoot.SelectSingleNode("返還/コピー設定一覧/コピー設定[@コード区分='" & Key.CODE_KBN & "']")
                            If node Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定[@コード区分='" & Key.CODE_KBN & "']」タグが定義されていません。")
                            End If

                            Dim attribute As XmlAttribute = node.Attributes.ItemOf("データ長")
                            If attribute Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性が定義されていません。（" &
                                                ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            sWork = attribute.Value.Trim
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性の値（" & sWork & "）が不当です。（" &
                                                ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If

                            intWriteLength = CInt(sWork)
                            'V01L09(QA/要望管理[OT-017]) 2019-06-19 ADD 九州農協オンライン対応 -------------------- END

                            Call HenkanFMT.GetHenkanHeaderRecord()

                        ElseIf HenkanFMT.IsTrailerRecord Then
                            ' トレーラ
                            ' トレーラ件数金額更新
                            Call HenkanFMT.GetHenkanTrailerRecord()

                            If HenkanFMT.InfoMeisaiMast.TOTAL_KEN <> OraSchReader.GetInt64("SYORI_KEN_S") And
                                HenkanFMT.InfoMeisaiMast.TOTAL_KEN2 <> OraSchReader.GetInt64("SYORI_KEN_S") Then
                                LOG.Write_LEVEL1("トレーラチェック", "成功", "請求件数 不一致")
                                Key.MESSAGE = "請求件数 不一致"
                                UnmatchArray.Add(Key)
                            ElseIf HenkanFMT.InfoMeisaiMast.TOTAL_KIN <> OraSchReader.GetInt64("SYORI_KIN_S") Then
                                LOG.Write_LEVEL1("トレーラチェック", "成功", "請求金額 不一致")
                                Key.MESSAGE = "請求金額 不一致"
                                UnmatchArray.Add(Key)
                            End If

                            If HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KEN <> OraSchReader.GetInt64("FUNOU_KEN_S") Then
                                LOG.Write_LEVEL1("トレーラチェック", "成功", "不能件数 不一致")
                                Key.MESSAGE = "不能件数 不一致"
                                UnmatchArray.Add(Key)
                            ElseIf HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KIN <> OraSchReader.GetInt64("FUNOU_KIN_S") Then
                                LOG.Write_LEVEL1("トレーラチェック", "成功", "不能金額 不一致")
                                Key.MESSAGE = "不能金額 不一致"
                                UnmatchArray.Add(Key)
                            End If

                            If Include0Yen <> 0 Then
                                '合計正常件数 計算値に、0円データを含める場合
                                dblFURI_KEN = HenkanFMT.InfoMeisaiMast.TOTAL_NORM_KEN
                            Else
                                '合計正常件数 計算値に、0円データを含めない場合
                                dblFURI_KEN = HenkanFMT.InfoMeisaiMast.TOTAL_NORM_KEN2
                            End If
                            dblFURI_KIN = HenkanFMT.InfoMeisaiMast.TOTAL_NORM_KIN
                            dblFUNOU_KEN = HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KEN
                            dblFUNOU_KIN = HenkanFMT.InfoMeisaiMast.TOTAL_IJO_KIN

                            If UnmatchArray.Count > 0 Then
                                HenkanStream.Close()
                                HenkanStream = Nothing
                                OraSchReader.Close()
                                Exit Function
                            Else
                                If fn_SCHMAST_UPDATE(Key) = False Then
                                    HenkanStream.Close()
                                    HenkanStream = Nothing
                                    OraSchReader.Close()
                                    Exit Function
                                End If

                                HenkyakuArray.Add(Key)
                            End If

                        ElseIf HenkanFMT.IsEndRecord Then
                            ' エンド

                        ElseIf HenkanFMT.IsDataRecord Then
                            'データレコード
                            Call HenkanFMT.GetHenkanDataRecord()

                            If Key.KEKKA_MEISAI_KBN = "1" AndAlso
                                HenkanFMT.InfoMeisaiMast.FURIKETU_CODE = 0 Then

                                HenkanFMT.RecordData = ""
                            Else
                            End If
                        End If

                        If HenkanFMT.RecordData.Trim <> "" Then
                            Dim tmpData As String = String.Copy(HenkanFMT.RecordData)
                            If WriteLongRecord = 0 Then
                                '変換ファイルにレコード長以上のデータを書き込まない場合
                                tmpData = tmpData.Substring(0, intWriteLength)
                            End If

                            If Padding = 1 Then
                                'レコードデータの先頭の文字詰めを行う場合
                                tmpData = tmpData.PadLeft(intWriteLength, PaddingChar)
                            ElseIf Padding = 2 Then
                                'レコードデータの末尾の文字詰めを行う場合
                                tmpData = tmpData.PadRight(intWriteLength, PaddingChar)
                            End If

                            HenkanStream.Write(tmpData)
                        End If

                    Loop

                    OraSchReader.NextRead()

                Loop Until OraSchReader.EOF = True

            Else
                If OraSchReader.Message <> "" Then
                    Dim errmsg As String
                    If OraSchReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "返還データ作成（XMLフォーマット）実行待ちタイムアウト"
                    Else
                        errmsg = "返還データ作成（XMLフォーマット）処理ロック異常"
                    End If

                    HenkanStream.Close()
                    HenkanStream = Nothing
                    OraSchReader.Close()
                    OraSchReader = Nothing

                    LOG.Write_Err("返還データ作成（XMLフォーマット）処理", "失敗", errmsg)
                    LOG.UpdateJOBMASTbyErr(errmsg)
                    Return False
                End If

            End If

            HenkanStream.Close()
            HenkanStream = Nothing

            OraSchReader.Close()

            LOG.Write_Exit1(sw, LOG.ToriCode, LOG.FuriDate, "返還データ作成（XMLフォーマット）", "")
            Return True

        Catch ex As Exception

            LOG.Write_Err("返還データ作成（XMLフォーマット）", "失敗", ex)
            LOG.UpdateJOBMASTbyErr("返還データ作成（XMLフォーマット）処理 失敗")
            Return False

        End Try
    End Function

    '処理結果確認表
    Private Function PrintSyoriKekkaKakuninhyo(ByVal Key As KeyInfo) As Boolean

        Dim SQL As New StringBuilder(128)
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

        Dim PrnSyoriKekkaKakuninhyo As New ClsPrnSyorikekkaKakuninhyo
        Dim bSQL As Boolean
        Dim name As String = ""

        Try

            PrnSyoriKekkaKakuninhyo.strATENA_UMU = strcINI_PARAM.ATENA_UMU
            Dim strYUUSOU_NAME As String = ""
            Dim strPRNT_KIN_NAME As String = ""

            SQL.Append("SELECT ")
            SQL.Append(" ITAKU_NNAME_T")
            SQL.Append(", ITAKU_CODE_T")
            SQL.Append(", YUUBIN_NNAME_T")
            SQL.Append(", SYORI_KEN_S")
            SQL.Append(", SYORI_KIN_S")
            SQL.Append(", FURI_KEN_S")
            SQL.Append(", FURI_KIN_S")
            SQL.Append(", FUNOU_KEN_S")
            SQL.Append(", FUNOU_KIN_S")
            SQL.Append(", TORIS_CODE_T")
            SQL.Append(", TORIF_CODE_T")
            SQL.Append(", FURI_CODE_T")
            SQL.Append(", KIGYO_CODE_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(", SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND FURI_DATE_S  = " & SQ(Key.FURI_DATE))
            SQL.Append(" AND FUNOU_FLG_S = '1'")
            Select Case strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE
                Case "111111111111" '依頼書
                    SQL.Append(" AND BAITAI_CODE_T = '04'")
                    SQL.Append(" ORDER BY TORIS_CODE_S,TORIF_CODE_S")
                Case "222222222222" '伝票
                    SQL.Append(" AND BAITAI_CODE_T = '09'")
                    SQL.Append(" ORDER BY TORIS_CODE_S,TORIF_CODE_S")
                Case Else
                    SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(Key.ITAKU_KANRI_CODE))
                    SQL.Append(" AND MOTIKOMI_SEQ_S = " & SQ(Key.MOTIKOMI_SEQ))
                    SQL.Append(" ORDER BY FILE_SEQ_S")
            End Select

            bSQL = OraReader.DataReader(SQL)

            If bSQL = True Then
                ' CSVを作成する
                name = PrnSyoriKekkaKakuninhyo.CreateCsvFile()
            End If

            Do
                PrnSyoriKekkaKakuninhyo.OutputCsvData(CASTCommon.Calendar.Now.ToString("yyyyMMdd"))     '処理日
                PrnSyoriKekkaKakuninhyo.OutputCsvData(CASTCommon.Calendar.Now.ToString("HHmmss"))       'タイムスタンプ
                PrnSyoriKekkaKakuninhyo.OutputCsvData(Key.FURI_DATE)                                    '振替日
                If strcINI_PARAM.ATENA_UMU <> "err" Then                                                '小浜信金以降のVerのみ項目追加
                    If strcINI_PARAM.ATENA_UMU = "1" Then                                               '宛名出力あり
                        If OraReader.GetItem("YUUBIN_NNAME_T").Trim <> "" Then                          '郵送先漢字に入力あり
                            strYUUSOU_NAME = OraReader.GetItem("YUUBIN_NNAME_T")
                        Else                                                                            '入力なし
                            strYUUSOU_NAME = OraReader.GetItem("ITAKU_NNAME_T")
                        End If
                        strPRNT_KIN_NAME = strcINI_PARAM.PRNT_KIN_NAME

                    Else                                                                                '宛名出力なし（０またはスペース指定）
                        strYUUSOU_NAME = ""                                                             '郵送先空白
                        strPRNT_KIN_NAME = ""                                                           '金庫名空白
                    End If
                    PrnSyoriKekkaKakuninhyo.OutputCsvData(strYUUSOU_NAME)                               '郵送先（なければ委託者名)設定
                    PrnSyoriKekkaKakuninhyo.OutputCsvData(strPRNT_KIN_NAME)                             '金庫名（iniファイル)設定
                End If

                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("TORIS_CODE_T"))                '取引先主コード
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("TORIF_CODE_T"))                '取引先副コード
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("ITAKU_NNAME_T"))               '取引先名
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("ITAKU_CODE_T"))                '委託者コード
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("SYORI_KEN_S"))                 '請求件数
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("SYORI_KIN_S"))                 '請求金額
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("FURI_KEN_S"))                  '振替件数
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("FURI_KIN_S"))                  '振替金額
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("FUNOU_KEN_S"))                 '不能件数
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("FUNOU_KIN_S"))                 '不能金額
                PrnSyoriKekkaKakuninhyo.OutputCsvData(Key.MESSAGE)                                      '備考
                PrnSyoriKekkaKakuninhyo.OutputCsvData("0")                                              '区分
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("FURI_CODE_T"))                 '振替コード
                PrnSyoriKekkaKakuninhyo.OutputCsvData(OraReader.GetItem("KIGYO_CODE_T"), False, True)   '企業コード

                OraReader.NextRead()
            Loop Until OraReader.EOF
            OraReader.Close()

            If Not PrnSyoriKekkaKakuninhyo Is Nothing Then

                PrnSyoriKekkaKakuninhyo.CloseCsv()

                If PrnSyoriKekkaKakuninhyo.ReportExecute() = True Then
                    LOG.Write("処理結果確認表（返還データ作成）出力", "成功")
                Else
                    LOG.Write("処理結果確認表（返還データ作成）出力", "失敗", PrnSyoriKekkaKakuninhyo.ReportMessage)
                End If
            End If

            Return True

        Catch ex As Exception
            LOG.Write("処理結果確認表出力（返還データ作成）", "失敗", PrnSyoriKekkaKakuninhyo.ReportMessage)
            Return False
        End Try

    End Function

    Private Function fn_CHK_BAITAI(ByVal astrCHK_SEND_FILE As String,
                           ByVal astrIN_FILE_NAME As String,
                           ByVal aintREC_LENGTH As Integer) As Boolean
        '============================================================================
        'NAME           :fn_CHK_BAITAI
        'Parameter      :astrCHK_SEND_FILE：チェックするファイル／astrIN_FILE_NAME：比較対象ファイル
        '               :aintREC_LENGTH：ファイルのレコード長
        'Description    :媒体データチェック処理（書き込み前）
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/11/30
        'Update         :
        '============================================================================
        fn_CHK_BAITAI = False

        Dim intFILE_NO_1 As Integer
        Dim intFILE_NO_2 As Integer
        Dim strFURI_DATA_1 As String = ""
        Dim strFURI_DATA_2 As String = ""
        '2011/06/15 標準版修正 再振は媒体チェックしない---------------START
        Dim SFURI_FLG As String = "0"
        Dim SQL As New StringBuilder(128)
        Dim SAI_Reader As New CASTCommon.MyOracleReader(MainDB)
        Try
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック(開始)", "成功", "")

            SQL.Append("SELECT ")
            SQL.Append(" SFURI_FLG_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = '" & strcKOBETUPARAM.strTORIS_CODE & "'")
            SQL.Append(" AND ITAKU_CODE_T = '" & strcTORI_PARAM.ITAKU_CODE & "'")
            SQL.Append(" AND SFURI_FCODE_T = '" & strcKOBETUPARAM.strTORIF_CODE & "'")
            SQL.Append(" AND SFURI_FLG_T = '1'")

            If SAI_Reader.DataReader(SQL) = True Then
                SFURI_FLG = "1"
            End If
            SAI_Reader.Close()
            If SFURI_FLG = "1" Then
                fn_CHK_BAITAI = True
                Exit Function
            End If

        Catch ex As Exception
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック", "失敗 ", Err.Description)
            SAI_Reader.Close()
        Finally
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック(終了)", "成功", "")
            SAI_Reader.Close()
        End Try
        '2011/06/15 標準版修正 再振は媒体チェックしない---------------END

        intFILE_NO_1 = FreeFile()
        FileOpen(intFILE_NO_1, astrCHK_SEND_FILE, OpenMode.Random, , , aintREC_LENGTH)
        If Err.Number = 0 Then
        Else
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体チェック", "失敗 ", "媒体ファイルオープン ファイル名：" & astrCHK_SEND_FILE & Err.Description)
            LOG.UpdateJOBMASTbyErr("媒体チェック失敗")
            FileClose(intFILE_NO_1)
            Exit Function
        End If

        intFILE_NO_2 = FreeFile()
        FileOpen(intFILE_NO_2, astrIN_FILE_NAME, OpenMode.Random, , , aintREC_LENGTH)
        If Err.Number = 0 Then
        Else
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体チェック", "失敗 ", "媒体ファイルオープン ファイル名：" & astrIN_FILE_NAME & Err.Description)
            LOG.UpdateJOBMASTbyErr("媒体チェック失敗")
            FileClose(intFILE_NO_1)
            FileClose(intFILE_NO_2)
            Exit Function
        End If

        Dim sysValueType As System.ValueType

        Try
            Select Case aintREC_LENGTH
                Case 118
                    sysValueType = DirectCast(gstrDATA_118, ValueType)
                    FileGet(intFILE_NO_1, sysValueType, 1)
                    gstrDATA_118 = DirectCast(sysValueType, gstrFURI_DATA_118)
                    strFURI_DATA_1 = gstrDATA_118.strDATA

                    sysValueType = DirectCast(gstrDATA_118, ValueType)
                    FileGet(intFILE_NO_2, sysValueType, 1)
                    gstrDATA_118 = DirectCast(sysValueType, gstrFURI_DATA_118)
                    strFURI_DATA_2 = gstrDATA_118.strDATA
                Case 119
                    sysValueType = DirectCast(gstrDATA_119, ValueType)
                    FileGet(intFILE_NO_1, sysValueType, 1)
                    gstrDATA_119 = DirectCast(sysValueType, gstrFURI_DATA_119)
                    strFURI_DATA_1 = gstrDATA_119.strDATA

                    sysValueType = DirectCast(gstrDATA_119, ValueType)
                    FileGet(intFILE_NO_2, sysValueType, 1)
                    gstrDATA_119 = DirectCast(sysValueType, gstrFURI_DATA_119)
                    strFURI_DATA_2 = gstrDATA_119.strDATA
                Case 120
                    sysValueType = DirectCast(gstrDATA_120, ValueType)
                    FileGet(intFILE_NO_1, sysValueType, 1)
                    gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                    strFURI_DATA_1 = gstrDATA_120.strDATA

                    sysValueType = DirectCast(gstrDATA_120, ValueType)
                    FileGet(intFILE_NO_2, sysValueType, 1)
                    gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                    strFURI_DATA_2 = gstrDATA_120.strDATA
            End Select

        Catch EX As Exception

            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体チェック", "失敗 ", "ファイル読み込み レコードNO：" & intRECORD_COUNT & Err.Description)
            LOG.UpdateJOBMASTbyErr("媒体チェック失敗")
            FileClose(intFILE_NO_1)
            FileClose(intFILE_NO_2)
            Exit Function
        End Try
        If strFURI_DATA_1 <> strFURI_DATA_2 Then
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体チェック", "失敗 ", "媒体不一致" & Err.Description)
            LOG.UpdateJOBMASTbyErr("媒体不一致")
            fn_CHK_BAITAI = False
            FileClose(intFILE_NO_1)
            FileClose(intFILE_NO_2)
            Exit Function
        End If
        fn_CHK_BAITAI = True
        FileClose(intFILE_NO_1)
        FileClose(intFILE_NO_2)

    End Function

    Private Function fn_BAITAI_WRITE_CHK() As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_WRITE_CHK
        'Parameter      :
        'Description    :媒体書込み処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/08/20
        'Update         :
        '============================================================================
        fn_BAITAI_WRITE_CHK = False
        Dim strCHK_SEND_FILE As String
        Dim intKEKKA As Integer
        Dim strKEKKA As String

        strCHK_SEND_FILE = strcINI_PARAM.DATBK_FOLDER & "A" &
                           strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE & ".dat" 'チェックファイル
        If Dir(strCHK_SEND_FILE) <> "" Then
            Kill(strCHK_SEND_FILE)
        End If

        Select Case strcTORI_PARAM.BAITAI_CODE
            Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"        '伝送

            Case "01"
                strOUT_FILE_NAME = strcTORI_PARAM.FILE_NAME.Trim

                intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                      strOUT_FILE_NAME, strCHK_SEND_FILE,
                                                      intREC_LENGTH, strcTORI_PARAM.CODE_KBN, gstrP_FILE, msgTitle)
                Select Case intKEKKA
                    Case 0
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "成功 ", "ＦＤ取込")
                    Case 100
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（コード変換）失敗")
                        LOG.UpdateJOBMASTbyErr("ＦＤ読み込み失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                    Case 200
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（コード区分異常（JIS改行あり））")
                        LOG.UpdateJOBMASTbyErr("ＦＤ取込（コード区分異常（JIS改行あり））失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                    Case 300
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（コード区分異常（JIS改行なし））")
                        LOG.UpdateJOBMASTbyErr("ＦＤ取込（コード区分異常（JIS改行なし））失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                    Case 400
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_FD_CPYTO_DISK", "失敗 ", "ＦＤ取込（出力ファイル作成）失敗")
                        LOG.UpdateJOBMASTbyErr("ＦＤ取込（出力ファイル作成）失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                End Select

                '------------------------------------
                '媒体内のファイルのチェック
                '------------------------------------
                If IsNumeric(strcTORI_PARAM.FMT_KBN) Then
                    Dim nFmtKbn As Integer = CInt(strcTORI_PARAM.FMT_KBN)
                    If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                        If fn_BAITAI_FILE_CHK_XML(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    Else
                        If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    End If
                Else
                    If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                        fn_BAITAI_WRITE_CHK = False
                        Exit Function
                    End If
                End If

            Case "04"        '紙

            Case "05"        'ＭＴ
                Dim lngErrStatus As Long
                '---------------------------------
                'ＭＴ内ファイルの読み込み
                '----------------------------------
                '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                strKEKKA = vbDLL.mtCPYtoDISK(intBLK_SIZE,
                                             intREC_LENGTH,
                                             CInt(strcTORI_PARAM.LABEL_KBN), "SLMT", 1, 4, " ", strCHK_SEND_FILE, 0, lngErrStatus)
                'strKEKKA = vbDLL.mtCPYtoDISK(CShort(intBLK_SIZE),
                '                                 CShort(intREC_LENGTH),
                '                                 CShort(strcTORI_PARAM.LABEL_KBN), "SLMT", 1, 4, " ", strCHK_SEND_FILE, 0, CInt(lngErrStatus))
                '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                If strKEKKA <> "" Then
                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoDISK", "失敗 ", "ＭＴ読み込み:" & strKEKKA)
                    LOG.UpdateJOBMASTbyErr("ＭＴ読み込み:" & strKEKKA)
                    fn_BAITAI_WRITE_CHK = False
                    Exit Function
                Else
                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "mtCPYtoDISK", "成功 ", "ＭＴ読み込み")
                End If

                '------------------------------------
                '媒体内のファイルのチェック
                '------------------------------------
                If IsNumeric(strcTORI_PARAM.FMT_KBN) Then
                    Dim nFmtKbn As Integer = CInt(strcTORI_PARAM.FMT_KBN)
                    If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                        If fn_BAITAI_FILE_CHK_XML(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    Else
                        If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    End If
                Else
                    If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                        fn_BAITAI_WRITE_CHK = False
                        Exit Function
                    End If
                End If

            Case "06"        'ＣＭＴ
                Dim lngErrStatus As Long

                '---------------------------------
                'ＣＭＴ内ファイルの読み込み
                '----------------------------------
                '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                strKEKKA = vbDLL.cmtCPYtoDISK(intBLK_SIZE,
                                              intREC_LENGTH,
                                              CInt(strcTORI_PARAM.LABEL_KBN), "SLMT", 1, 4, " ", strCHK_SEND_FILE, 0, lngErrStatus)
                'strKEKKA = vbDLL.cmtCPYtoDISK(CShort(intBLK_SIZE),
                '                                  CShort(intREC_LENGTH),
                '                                  CShort(strcTORI_PARAM.LABEL_KBN), "SLMT", 1, 4, " ", strCHK_SEND_FILE, 0, CInt(lngErrStatus))
                '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                If strKEKKA <> "" Then
                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoDISK", "失敗", "ＣＭＴ読み込み:" & strKEKKA)
                    LOG.UpdateJOBMASTbyErr("ＣＭＴ読み込み:" & strKEKKA)
                    fn_BAITAI_WRITE_CHK = False
                    Exit Function
                Else
                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "cmtCPYtoDISK", "成功", "ＣＭＴ読み込み")
                End If
                '------------------------------------
                '媒体内のファイルのチェック
                '------------------------------------
                If IsNumeric(strcTORI_PARAM.FMT_KBN) Then
                    Dim nFmtKbn As Integer = CInt(strcTORI_PARAM.FMT_KBN)
                    If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                        If fn_BAITAI_FILE_CHK_XML(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    Else
                        If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    End If
                Else
                    If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                        fn_BAITAI_WRITE_CHK = False
                        Exit Function
                    End If
                End If

            Case "11", "12", "13", "14", "15"
                '------------------------------------
                '媒体内のファイルのチェック
                '------------------------------------
                strOUT_FILE_NAME = strcTORI_PARAM.FILE_NAME.Trim

                intKEKKA = clsFUSION.fn_DVD_CPYTO_DISK(strcKOBETUPARAM.strTORIS_CODE & strcKOBETUPARAM.strTORIF_CODE,
                                                       strOUT_FILE_NAME, strCHK_SEND_FILE,
                                                       intREC_LENGTH, strcTORI_PARAM.CODE_KBN, gstrP_FILE, msgTitle, strcTORI_PARAM.BAITAI_CODE)

                '-------------------------------------------------------
                ' intKEKKA      : 0   = 成功
                '               : 100 = ファイル読み込み失敗
                '               : 200 = コード区分異常（JIS改行あり）
                '               : 300 = コード区分異常（JIS改行なし）
                '               : 400 = 出力ファイル作成失敗
                '-------------------------------------------------------
                Select Case intKEKKA
                    Case 0
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "成功 ", "媒体書込み後検証 外部媒体取込（コード変換）")
                    Case 100
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "媒体書込み後検証 外部媒体取込（コード変換）")
                        LOG.UpdateJOBMASTbyErr("媒体書込み後検証 外部媒体読み込み失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                    Case 200
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "媒体書込み後検証 外部媒体取込（コード区分異常（JIS改行あり））")
                        LOG.UpdateJOBMASTbyErr("媒体書込み後検証 外部媒体取込（コード区分異常（JIS改行あり））失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                    Case 300
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "媒体書込み後検証 外部媒体取込（コード区分異常（JIS改行なし））")
                        LOG.UpdateJOBMASTbyErr("媒体書込み後検証 外部媒体取込（コード区分異常（JIS改行なし））失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                    Case 400
                        fn_BAITAI_WRITE_CHK = False
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "fn_DVD_CPYTO_DISK", "失敗 ", "媒体書込み後検証 外部媒体取込（出力ファイル作成）失敗")
                        LOG.UpdateJOBMASTbyErr("媒体書込み後検証 外部媒体取込（出力ファイル作成）失敗")
                        If Dir(strCHK_SEND_FILE) <> "" Then
                            Kill(strCHK_SEND_FILE)
                        End If
                        Exit Function
                End Select

                '------------------------------------
                '媒体内のファイルのチェック
                '------------------------------------
                If IsNumeric(strcTORI_PARAM.FMT_KBN) Then
                    Dim nFmtKbn As Integer = CInt(strcTORI_PARAM.FMT_KBN)
                    If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                        If fn_BAITAI_FILE_CHK_XML(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    Else
                        If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                            fn_BAITAI_WRITE_CHK = False
                            Exit Function
                        End If
                    End If
                Else
                    If fn_BAITAI_FILE_CHK(strCHK_SEND_FILE, intREC_LENGTH) = False Then
                        fn_BAITAI_WRITE_CHK = False
                        Exit Function
                    End If
                End If
        End Select

        If Dir(strCHK_SEND_FILE) <> "" Then
            Kill(strCHK_SEND_FILE)
        End If
        fn_BAITAI_WRITE_CHK = True
    End Function


    Function fn_BAITAI_FILE_CHK(ByVal astrCHK_SEND_FILE As String, ByVal aintREC_LENGTH As Integer) As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_FILE_CHK
        'Parameter      :astrCHK_SEND_FILE：チェックするファイル
        '               :aintREC_LENGTH：ファイルのレコード長
        'Description    :媒体データチェック処理（書き込み前）
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/11/30
        'Update         :
        '============================================================================
        fn_BAITAI_FILE_CHK = False

        Dim intFILE_NO_1 As Integer
        Dim strFURI_DATA_1 As String
        Dim dblRECORD_COUNT As Double
        Dim strFILE_ITAKU_CODE As String = ""
        Dim dblSCH_SYORI_KEN As Double
        Dim dblSCH_SYORI_KIN As Double
        Dim dblSCH_FURI_KEN As Double
        Dim dblSCH_FURI_KIN As Double
        Dim dblSCH_FUNOU_KEN As Double
        Dim dblSCH_FUNOU_KIN As Double
        Dim dblFILE_SYORI_KEN As Double
        Dim dblFILE_SYORI_KIN As Double
        Dim dblFILE_FURI_KEN As Double
        Dim dblFILE_FURI_KIN As Double
        Dim dblFILE_FUNOU_KEN As Double
        Dim dblFILE_FUNOU_KIN As Double

        dblRECORD_COUNT = 1

        intFILE_NO_1 = FreeFile()
        Select Case aintREC_LENGTH
            Case 118, 119, 120
                FileOpen(intFILE_NO_1, astrCHK_SEND_FILE, OpenMode.Random, , , 120)
            Case Else
                FileOpen(intFILE_NO_1, astrCHK_SEND_FILE, OpenMode.Random, , , aintREC_LENGTH)
        End Select
        If Err.Number = 0 Then
        Else
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "書き込みファイルオープン", "失敗", "ファイル名：" & astrCHK_SEND_FILE & Err.Description)
            FileClose(intFILE_NO_1)
            Exit Function
        End If

        Dim sysValueType As ValueType

        Try
            Do Until EOF(intFILE_NO_1)
                strFURI_DATA_1 = ""
                Select Case aintREC_LENGTH
                    '2017/05/16 saitou RSV2標準 MODIFY JIS118,JIS119の処理不良改修 ------------------------------------- START
                    'JIS118,JIS119も120バイトの構造体を使用する
                    Case 118, 119, 120
                        sysValueType = DirectCast(gstrDATA_120, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                        strFURI_DATA_1 = gstrDATA_120.strDATA
                        'Case 118
                        '    sysValueType = DirectCast(gstrDATA_118, ValueType)
                        '    FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        '    gstrDATA_118 = DirectCast(sysValueType, gstrFURI_DATA_118)
                        '    strFURI_DATA_1 = gstrDATA_118.strDATA
                        'Case 119
                        '    sysValueType = DirectCast(gstrDATA_119, ValueType)
                        '    FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        '    gstrDATA_119 = DirectCast(sysValueType, gstrFURI_DATA_119)
                        '    strFURI_DATA_1 = gstrDATA_119.strDATA
                        'Case 120
                        '    sysValueType = DirectCast(gstrDATA_120, ValueType)
                        '    FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        '    gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                        '    strFURI_DATA_1 = gstrDATA_120.strDATA
                        '2017/05/16 saitou RSV2標準 MODIFY ----------------------------------------------------------------- END
                    Case 220
                        '   FileGet(intFILE_NO_1, gstrDATA_220, dblRECORD_COUNT)
                        '   strFURI_DATA_1 = gstrDATA_220.strDATA
                    Case 390
                        sysValueType = DirectCast(gstrDATA_390, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_390 = DirectCast(sysValueType, gstrFURI_DATA_390)
                        strFURI_DATA_1 = gstrDATA_390.strDATA
                End Select
                If strFURI_DATA_1 = "" Then
                    LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT)
                    FileClose(intFILE_NO_1)
                    Return False
                End If
                Select Case strFURI_DATA_1.Substring(0, 1)
                    Case "1"
                        '------------------------------------
                        '取引先コード　&　スケジュールの検索
                        '------------------------------------

                        Select Case aintREC_LENGTH
                            Case 118, 119, 120
                                Select Case strcTORI_PARAM.FMT_KBN
                                    '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                                    Case "00", "20", "21"
                                        'Case "00"
                                        '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                                        sysValueType = DirectCast(gZENGIN_REC1, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gZENGIN_REC1 = DirectCast(sysValueType, CAstFormat.CFormatZengin.ZGRECORD1)
                                        strFILE_ITAKU_CODE = gZENGIN_REC1.ZG4
                                    Case "01"       'ＮＨＫ
                                        sysValueType = DirectCast(gNHK_REC1, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gNHK_REC1 = DirectCast(sysValueType, CAstFormat.CFormatNHK.NHKRECORD1)
                                        strFILE_ITAKU_CODE = gNHK_REC1.NH04
                                End Select
                            Case 220

                            Case 390
                                sysValueType = DirectCast(gKOKUZEI_REC1, ValueType)
                                FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                gKOKUZEI_REC1 = DirectCast(sysValueType, CAstFormat.CFormatKokuzei.KOKUZEI_RECORD1)
                                strFILE_ITAKU_CODE = ""
                            Case Else
                                strFILE_ITAKU_CODE = ""
                        End Select
                        '--------------------------------------------------------------
                        '取引先コード検索（マルチの場合も考慮する）、委託者コードのチェック
                        '--------------------------------------------------------------
                        '   If strcTORI_PARAM.MULTI_KBN = "0" Then      'シングル (取引先主コード、副コードで検索)
                        '       gstrSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strcKOBETUPARAM.strTORIS_CODE) & _
                        '                 "' AND TORIF_CODE_T = '" & Trim(strcKOBETUPARAM.strTORIF_CODE) & "'"
                        '   Else                              'マルチ（取引先主コード、委託者コードで検索）
                        '       gstrSQL = "SELECT * FROM TORIMAST WHERE TORIS_CODE_T = '" & Trim(strcKOBETUPARAM.strTORIS_CODE) & _
                        '                 "' AND ITAKU_CODE_T = '" & Trim(strcTORI_PARAM.ITAKU_CODE) & "'"
                        '   End If
                        Dim SQL As String = ""
                        If strFILE_ITAKU_CODE = "" Then
                        Else
                            SQL = "SELECT * FROM TORIMAST"
                            SQL &= " WHERE ITAKU_CODE_T = '" & Trim(strFILE_ITAKU_CODE) & "'"
                        End If
                        SQL &= " AND FSYORI_KBN_T = '1'"

                        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

                        If OraReader.DataReader(SQL) = False Then            '取引先コードが存在しない
                            OraReader.Close()
                            OraReader = Nothing
                            Exit Function
                        Else
                            strTORIF_CODE = clsFUSION.fn_chenge_null_value(OraReader.GetItem("TORIF_CODE_T"))
                            OraReader.Close()
                            OraReader = Nothing
                        End If

                        '------------------------------------
                        'スケジュールの検索
                        '------------------------------------
                        '   gstrSQL = "SELECT * FROM SCHMAST WHERE TORIS_CODE_S = '" & Trim(strcKOBETUPARAM.strTORIS_CODE) & "' AND "
                        '   gstrSQL = gstrSQL & "TORIF_CODE_S = '" & Trim(strTORIF_CODE) & "' AND "
                        '2010/02/04 国税の場合は委託者コードを条件からはずす
                        If strcTORI_PARAM.FMT_KBN <> "02" Then
                            SQL = "SELECT * FROM SCHMAST WHERE ITAKU_CODE_S = '" & Trim(strFILE_ITAKU_CODE) & "' AND "
                            SQL = SQL & "FURI_DATE_S = '" & Trim(strcKOBETUPARAM.strFURI_DATE) & "' AND "
                            SQL = SQL & "FSYORI_KBN_S = '1'"
                            '2010/02/03 同一委託者コード/振替日対応(マルチ対応は不要)
                            SQL = SQL & " AND SCHMAST.FUNOU_FLG_S = '1'"
                            If strcTORI_PARAM.MULTI_KBN = "0" Then  'シングル
                                SQL = SQL & " AND TORIS_CODE_S = " & SQ(strcKOBETUPARAM.strTORIS_CODE)
                                SQL = SQL & " AND TORIF_CODE_S = " & SQ(strcKOBETUPARAM.strTORIF_CODE)
                            End If
                        Else
                            SQL = "SELECT * FROM SCHMAST WHERE FURI_DATE_S = '" & Trim(strcKOBETUPARAM.strFURI_DATE) & "' AND "
                            SQL = SQL & "FSYORI_KBN_S = '1'"
                            SQL = SQL & " AND TORIS_CODE_S = " & SQ(strcKOBETUPARAM.strTORIS_CODE)
                            SQL = SQL & " AND TORIF_CODE_S = " & SQ(strcKOBETUPARAM.strTORIF_CODE)
                        End If
                        '======================================

                        OraReader = New CASTCommon.MyOracleReader(MainDB)

                        If OraReader.DataReader(SQL) = False Then            '取引先コードが存在しない
                            OraReader.Close()
                            OraReader = Nothing
                            Exit Function
                        Else
                            dblSCH_SYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                            dblSCH_SYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")
                            dblSCH_FURI_KEN = OraReader.GetInt64("FURI_KEN_S")
                            dblSCH_FURI_KIN = OraReader.GetInt64("FURI_KIN_S")
                            dblSCH_FUNOU_KEN = OraReader.GetInt64("FUNOU_KEN_S")
                            dblSCH_FUNOU_KIN = OraReader.GetInt64("FUNOU_KIN_S")

                            OraReader.Close()
                            OraReader = Nothing
                        End If
                    Case "2", "3", "4", "5"
                    Case "8"
                        Select Case aintREC_LENGTH
                            Case 118, 119, 120
                                Select Case strcTORI_PARAM.FMT_KBN
                                    '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                                    Case "00", "20", "21"
                                        'Case "00"
                                        '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                                        sysValueType = DirectCast(gZENGIN_REC8, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gZENGIN_REC8 = DirectCast(sysValueType, CAstFormat.CFormatZengin.ZGRECORD8)
                                        dblFILE_SYORI_KEN = Val(gZENGIN_REC8.ZG2)
                                        dblFILE_SYORI_KIN = Val(gZENGIN_REC8.ZG3)
                                        dblFILE_FURI_KEN = Val(gZENGIN_REC8.ZG4.Substring(0, 6))
                                        dblFILE_FURI_KIN = Val(gZENGIN_REC8.ZG5.Substring(0, 12))
                                        dblFILE_FUNOU_KEN = Val(gZENGIN_REC8.ZG6.Substring(0, 6))
                                        dblFILE_FUNOU_KIN = Val(gZENGIN_REC8.ZG7.Substring(0, 12))
                                    Case "01"
                                        sysValueType = DirectCast(gNHK_REC8, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gNHK_REC8 = DirectCast(sysValueType, CAstFormat.CFormatNHK.NHKRECORD8)
                                        dblFILE_SYORI_KEN = Val(gNHK_REC8.NH02)
                                        dblFILE_SYORI_KIN = Val(gNHK_REC8.NH03)
                                        dblFILE_FURI_KEN = Val(gNHK_REC8.NH04.Substring(0, 6))
                                        dblFILE_FURI_KIN = Val(gNHK_REC8.NH05.Substring(0, 12))
                                        dblFILE_FUNOU_KEN = Val(gNHK_REC8.NH06.Substring(0, 6))
                                        dblFILE_FUNOU_KIN = Val(gNHK_REC8.NH07.Substring(0, 12))
                                End Select
                            Case 220

                            Case 390
                                sysValueType = DirectCast(gKOKUZEI_REC8, ValueType)
                                FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                gKOKUZEI_REC8 = DirectCast(sysValueType, CAstFormat.CFormatKokuzei.KOKUZEI_RECORD8)
                                dblFILE_SYORI_KEN = Val(gKOKUZEI_REC8.KZ7.Trim)
                                dblFILE_SYORI_KIN = Val(gKOKUZEI_REC8.KZ8.Trim)
                                dblFILE_FURI_KEN = Val(gKOKUZEI_REC8.KZ11.Trim)
                                dblFILE_FURI_KIN = Val(gKOKUZEI_REC8.KZ12.Trim)
                                dblFILE_FUNOU_KEN = Val(gKOKUZEI_REC8.KZ9.Trim)
                                dblFILE_FUNOU_KIN = Val(gKOKUZEI_REC8.KZ10.Trim)
                        End Select
                        If dblSCH_SYORI_KEN <> dblFILE_SYORI_KEN Then
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "トレーラチェック", "失敗", "処理件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            FileClose(intFILE_NO_1)
                            Exit Function
                        End If
                        If dblSCH_SYORI_KIN <> dblFILE_SYORI_KIN Then
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "トレーラチェック", "失敗", "処理金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            FileClose(intFILE_NO_1)
                            Exit Function
                        End If

                        If dblSCH_FUNOU_KEN <> dblFILE_FUNOU_KEN Then
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "トレーラチェック", "失敗", "不能件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            FileClose(intFILE_NO_1)
                            Exit Function
                        End If
                        If dblSCH_FUNOU_KIN <> dblFILE_FUNOU_KIN Then
                            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "トレーラチェック", "失敗", "不能金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            FileClose(intFILE_NO_1)
                            Exit Function
                        End If
                    Case "9"

                End Select
                dblRECORD_COUNT += 1
            Loop
        Catch EX As Exception
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "ファイル読み込み", "失敗", "レコードNO：" & dblRECORD_COUNT & Err.Description)
            FileClose(intFILE_NO_1)
            Exit Function
        End Try
        fn_BAITAI_FILE_CHK = True
        FileClose(intFILE_NO_1)

    End Function

    Private Function fn_SCHMAST_UPDATE(ByVal Key As KeyInfo) As Boolean
        '============================================================================
        'NAME           :fn_SCHMAST_UPDATE
        'Parameter      :
        'Description    :スケジュールマスタ（SCHMAST）の更新
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/06
        'Update         :
        '============================================================================
        Try
            fn_SCHMAST_UPDATE = False

            Dim SQL As New StringBuilder
            SQL.Append("UPDATE SCHMAST SET")
            SQL.Append(" HENKAN_FLG_S = '1'")
            SQL.Append(", HENKAN_DATE_S = '" & Format(System.DateTime.Today, "yyyyMMdd") & "'")
            SQL.Append(" WHERE TORIS_CODE_S = '" & Key.TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_S= '" & Key.TORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_S= '" & Key.FURI_DATE & "'")

            If MainDB.ExecuteNonQuery(SQL) = -1 Then
                LOG.Write(LOG.UserID, Key.TORIS_CODE & "-" & Key.TORIF_CODE, Key.FURI_DATE, "スケジュール更新", "失敗", MainDB.Message)
                LOG.UpdateJOBMASTbyErr("スケジュール更新失敗")
                Exit Function
            End If

        Catch ex As Exception
            LOG.Write(LOG.UserID, Key.TORIS_CODE & "-" & Key.TORIF_CODE, Key.FURI_DATE, "スケジュール更新", "失敗", Err.Description)
            LOG.UpdateJOBMASTbyErr("スケジュール更新失敗")
            Exit Function
        End Try

        LOG.Write(LOG.UserID, Key.TORIS_CODE & "-" & Key.TORIF_CODE, Key.FURI_DATE, "返還データ作成", "成功", "スケジュール更新")

        fn_SCHMAST_UPDATE = True
    End Function
    '
    ' 関数名 ： fn_GetIni
    '
    ' 機能　 ： iniファイル読込
    '
    ' 引数   ： ByRef ErrMsg As String
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： エラー情報を返して呼び出し側関数でログ出力
    '
    Private Function fn_GetIni(ByRef ErrMsg As String) As Boolean
        '設定ファイルより取得
        strcINI_PARAM.DEN_FOLDER = CASTCommon.GetFSKJIni("COMMON", "DEN")               '伝送フォルダ
        If strcINI_PARAM.DEN_FOLDER = "err" Then
            ErrMsg = "DENフォルダ設定なし"
            Return False
        End If

        strcINI_PARAM.CSV_FOLDER = CASTCommon.GetFSKJIni("COMMON", "CSV")               'CSVフォルダ
        If strcINI_PARAM.CSV_FOLDER = "err" Then
            ErrMsg = "CSVフォルダ設定なし"
            Return False
        End If

        strcINI_PARAM.DAT_FOLDER = CASTCommon.GetFSKJIni("COMMON", "DAT")               'DATフォルダ
        If strcINI_PARAM.DAT_FOLDER = "err" Then
            ErrMsg = "DATフォルダ設定なし"
            Return False
        End If

        strcINI_PARAM.DATBK_FOLDER = CASTCommon.GetFSKJIni("COMMON", "DATBK")           'DATBKフォルダ
        If strcINI_PARAM.DATBK_FOLDER = "err" Then
            ErrMsg = "DATBKフォルダ設定なし"
            Return False
        End If

        strcINI_PARAM.LOG_FOLDER = CASTCommon.GetFSKJIni("COMMON", "LOG")           'LOGフォルダ
        If strcINI_PARAM.LOG_FOLDER = "err" Then
            ErrMsg = "LOGフォルダ設定なし"
            Return False
        End If

        strcINI_PARAM.MT_FLG = CASTCommon.GetFSKJIni("COMMON", "MT")           'MT設定
        If strcINI_PARAM.MT_FLG = "err" Then
            ErrMsg = "MT設定なし"
            Return False
        End If

        strcINI_PARAM.MT_INFILE = CASTCommon.GetFSKJIni("COMMON", "MTINFILE")           'MT設定
        If strcINI_PARAM.MT_INFILE = "err" Then
            ErrMsg = "MT_INFILE設定なし"
            Return False
        End If

        strcINI_PARAM.MT_OUTFILE = CASTCommon.GetFSKJIni("COMMON", "MTOUTFILE")
        If strcINI_PARAM.MT_OUTFILE = "err" Then
            ErrMsg = "MT_OUTFILE設定なし"
            Return False
        End If

        strcINI_PARAM.MT_TAKOUFILE = CASTCommon.GetFSKJIni("COMMON", "MTTAKOUFILE")
        If strcINI_PARAM.MT_TAKOUFILE = "err" Then
            ErrMsg = "MT_TAKOUFILE設定なし"
            Return False
        End If

        strcINI_PARAM.MT_FUNOUFILE = CASTCommon.GetFSKJIni("COMMON", "MTFUNOUFILE")
        If strcINI_PARAM.MT_FUNOUFILE = "err" Then
            ErrMsg = "MT_FUNOUFILE設定なし"
            Return False
        End If

        strcINI_PARAM.CMT_FLG = CASTCommon.GetFSKJIni("COMMON", "CMT")         'CMT設定
        If strcINI_PARAM.CMT_FLG = "err" Then
            ErrMsg = "CMT設定なし"
            Return False
        End If

        strcINI_PARAM.CMT_INFILE = CASTCommon.GetFSKJIni("COMMON", "CMTINFILE")         'CMT設定
        If strcINI_PARAM.CMT_INFILE = "err" Then
            ErrMsg = "CMT_INFILE設定なし"
            Return False
        End If

        strcINI_PARAM.CMT_OUTFILE = CASTCommon.GetFSKJIni("COMMON", "CMTOUTFILE")
        If strcINI_PARAM.CMT_OUTFILE = "err" Then
            ErrMsg = "CMT_OUTFILE設定なし"
            Return False
        End If

        strcINI_PARAM.CMT_TAKOUFILE = CASTCommon.GetFSKJIni("COMMON", "CMTTAKOUFILE")
        If strcINI_PARAM.CMT_TAKOUFILE = "err" Then
            ErrMsg = "CMT_TAKOUFILE設定なし"
            Return False
        End If

        strcINI_PARAM.CMT_FUNOUFILE = CASTCommon.GetFSKJIni("COMMON", "CMTFUNOUFILE")
        If strcINI_PARAM.CMT_FUNOUFILE = "err" Then
            ErrMsg = "CMT_FUNOUFILE設定なし"
            Return False
        End If

        '2012/06/30 標準版　WEB伝送対応
        strcINI_PARAM.WEB_SED_FOLDER = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_SED")
        If strcINI_PARAM.WEB_SED_FOLDER = "err" Then
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "設定ファイル取得", "失敗", "WEB_SED設定なし")
            LOG.UpdateJOBMASTbyErr("WEB_SED設定なし")
            Return False
        End If

        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            mLockWaitTime3 = CInt(sWork)
            If mLockWaitTime3 <= 0 Then
                mLockWaitTime3 = 600
            End If
        End If

        sWork = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            mLockWaitTime4 = CInt(sWork)
            If mLockWaitTime4 <= 0 Then
                mLockWaitTime4 = 30
            End If
        End If

        strcINI_PARAM.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If strcINI_PARAM.RSV2_EDITION = "err" Then
            ErrMsg = "EDITION設定なし"
            Return False
        End If

        strcINI_PARAM.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
        If strcINI_PARAM.COMMON_BAITAIWRITE = "err" Then
            ErrMsg = "BAITAIWRITE設定なし"
            Return False
        End If

        strcINI_PARAM.RSV2_HENKAN_PRINT = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HENKAN_PRINT")
        If strcINI_PARAM.RSV2_HENKAN_PRINT = "err" Then
            ErrMsg = "HENKAN_PRINT設定なし"
            Return False
        End If

        strcINI_PARAM.RSV2_HENKAN_PRINT_TXT = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HENKAN_PRINT_TXT")
        If strcINI_PARAM.RSV2_HENKAN_PRINT_TXT = "err" Then
            ErrMsg = "HENKAN_PRINT_TXT設定なし"
            Return False
        End If

        strcINI_PARAM.RSV2_HENKAN_PRINT_SORT_TXT = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HENKAN_PRINT_SORT_TXT")
        If strcINI_PARAM.RSV2_HENKAN_PRINT_SORT_TXT = "err" Then
            ErrMsg = "HENKAN_PRINT_SORT_TXT設定なし"
            Return False
        End If

        strcINI_PARAM.ATENA_UMU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "ATENA_UMU")

        strcINI_PARAM.PRNT_KIN_NAME = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KINKONAME")
        If strcINI_PARAM.ATENA_UMU <> "err" Then
            If strcINI_PARAM.PRNT_KIN_NAME = "err" Then
                ErrMsg = "KINKONAME設定なし"
                Return False
            End If
        End If

        strcINI_PARAM.HENKAN_WRITE_CHKTYPE = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "HENKAN_WRITE_CHKTYPE")
        If strcINI_PARAM.HENKAN_WRITE_CHKTYPE = "err" Then
            strcINI_PARAM.HENKAN_WRITE_CHKTYPE = "0"
        End If

        strcINI_PARAM.RSV2_USE_DENSO = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "USE_DENSO")
        If strcINI_PARAM.RSV2_USE_DENSO = "err" Or strcINI_PARAM.RSV2_USE_DENSO = "" Then
            strcINI_PARAM.RSV2_USE_DENSO = "0"
        End If

        Return True

    End Function
    '
    ' 関数名 ： fn_GetToriParam
    '
    ' 機能　 ： TORIMAST,SCHMAST情報取得
    '
    ' 引数   ： ByVal KobetuParam As ClsHenkan.KOBATUPARAM, ByRef ToriParam As ClsHenkan.TORI_PARAM
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： パラメータから取得
    '
    Private Function fn_GetToriParam(ByVal KobetuParam As ClsHenkan.KOBETUPARAM, ByRef ToriParam As ClsHenkan.TORI_PARAM) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try

            Dim SQL As String = ""
            SQL = "SELECT *"
            SQL &= " FROM TORIMAST"
            SQL &= ", SCHMAST"
            SQL &= " WHERE TORIMAST.TORIS_CODE_T = SCHMAST.TORIS_CODE_S"
            SQL &= " AND TORIMAST.TORIF_CODE_T = SCHMAST.TORIF_CODE_S"
            Select Case KobetuParam.strTORIS_CODE & KobetuParam.strTORIF_CODE
                Case "111111111111"
                    SQL &= " AND SCHMAST.BAITAI_CODE_S = '04'"
                Case "222222222222"
                    SQL &= " AND SCHMAST.BAITAI_CODE_S = '09'"
                Case Else
                    SQL &= " AND SCHMAST.TORIS_CODE_S = '" & Trim(KobetuParam.strTORIS_CODE) & "'"
                    SQL &= " AND SCHMAST.TORIF_CODE_S = '" & Trim(KobetuParam.strTORIF_CODE) & "'"
                    SQL &= " AND SCHMAST.MOTIKOMI_SEQ_S = '" & KobetuParam.strMOTIKOMI_SEQ & "'"
            End Select
            SQL &= " AND SCHMAST.FURI_DATE_S = '" & strcKOBETUPARAM.strFURI_DATE & "'"
            SQL &= " AND SCHMAST.FUNOU_FLG_S = '1'"
            SQL &= " AND SCHMAST.TYUUDAN_FLG_S = '0'"
            SQL &= " AND TORIMAST.KEKKA_HENKYAKU_KBN_T <> '0'"

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "スケジュールマスタ検索(開始)", "成功")

            '接続文字列設定
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = False Then

                OraReader.Close()
                OraReader = Nothing

                Return False
            Else
                ToriParam.FMT_KBN = OraReader.GetString("FMT_KBN_T")
                ToriParam.ITAKU_CODE = OraReader.GetString("ITAKU_CODE_T")
                ToriParam.CODE_KBN = OraReader.GetString("CODE_KBN_T")
                ToriParam.BAITAI_CODE = OraReader.GetString("BAITAI_CODE_T")
                ToriParam.FILE_NAME = OraReader.GetString("FILE_NAME_T")
                ToriParam.LABEL_KBN = OraReader.GetString("LABEL_KBN_T")
                ToriParam.MULTI_KBN = OraReader.GetString("MULTI_KBN_T")
                ToriParam.KEKKA_HENKYAKU_KBN = OraReader.GetString("KEKKA_HENKYAKU_KBN_T")
                ToriParam.ITAKU_KANRI_CODE = OraReader.GetString("ITAKU_KANRI_CODE_T")
                ToriParam.PRTNUM = OraReader.GetInt("PRTNUM_T")

                OraReader.Close()
                OraReader = Nothing

                Return True
            End If

        Catch ex As Exception
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return False

    End Function

    '2012/06/30 標準版　WEB伝送対応
    Public Function InsertWEB_RIREKIMAST(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, ByVal FURIDATE As String) As Boolean

        Dim dblock As CASTCommon.CDBLock = Nothing

        SubDB.BeginTrans()

        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = New MyOracleReader(SubDB)
        Dim SEQ As Integer = 0
        Dim ELog As New CASTCommon.ClsEventLOG

        Try

            ' WEB_RIREKIMAST登録ロック
            dblock = New CASTCommon.CDBLock
            If dblock.InsertWEB_RIREKIMAST_Lock(SubDB, mLockWaitTime4) = False Then
                ELog.Write("InsertWEB_RIREKIMAST処理でタイムアウト", )
                LOG.Write_Err("WEB_RIREKIMAST登録", "失敗", "InsertWEB_RIREKIMAST処理でタイムアウト")
                LOG.UpdateJOBMASTbyErr("InsertWEB_RIREKIMAST処理でタイムアウト")
                Return False
            End If

            SQL.Append(" SELECT * ")
            SQL.Append(" FROM WEB_RIREKIMAST ")
            SQL.Append(" WHERE TORIS_CODE_W = '" & TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_W = '" & TORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_W = '" & FURIDATE & "'")
            SQL.Append(" ORDER BY SEQ_NO_W DESC")

            If OraReader.DataReader(SQL) Then
                SEQ = OraReader.GetInt("SEQ_NO_W") + 1
            Else
                ELog.Write("InsertWEB_RIREKIMAST　検索失敗　", )
                Return False
            End If

            SQL = New StringBuilder(128)
            SQL.Append("INSERT INTO WEB_RIREKIMAST(")
            SQL.Append(" FSYORI_KBN_W")
            SQL.Append(",TORIS_CODE_W")
            SQL.Append(",TORIF_CODE_W")
            SQL.Append(",FURI_DATE_W")
            SQL.Append(",SEQ_NO_W")
            SQL.Append(",ITAKU_KANRI_CODE_W")
            SQL.Append(",USER_ID_W")
            SQL.Append(",FILE_NAME_W")
            SQL.Append(",SAKUSEI_DATE_W")
            SQL.Append(",SAKUSEI_TIME_W")
            SQL.Append(",STATUS_KBN_W")
            SQL.Append(",END_KEN_W")
            SQL.Append(",END_KIN_W")
            SQL.Append(",YOBI1_W")
            SQL.Append(",YOBI2_W")
            SQL.Append(",YOBI3_W")
            SQL.Append(",YOBI4_W")
            SQL.Append(",YOBI5_W")
            SQL.Append(",YOBI6_W")
            SQL.Append(",YOBI7_W")
            SQL.Append(",YOBI8_W")
            SQL.Append(",YOBI9_W")
            SQL.Append(",YOBI10_W")
            SQL.Append(") VALUES (")
            SQL.Append("'" & "1" & "'")                                         ' FSYORI_KBN_W
            SQL.Append("," & SQ(TORIS_CODE))                                    ' TORIS_CODE_W
            SQL.Append("," & SQ(TORIF_CODE))                                    ' TORIF_CODE_W
            SQL.Append("," & SQ(FURIDATE))                                      ' FURI_DATE_W
            SQL.Append("," & SQ(SEQ))                                           ' SEQ_NO_W
            SQL.Append("," & SQ(OraReader.GetString("ITAKU_KANRI_CODE_W")))     ' ITAKU_KANRI_CODE_W
            SQL.Append("," & SQ(OraReader.GetString("USER_ID_W")))              ' USER_ID_W
            SQL.Append("," & SQ(OraReader.GetString("FILE_NAME_W")))            ' FILE_NAME_W
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' SAKUSEI_DATE_W
            SQL.Append("," & SQ(Now.ToString("HHmmss")))                        ' SAKUSEI_TIME_W
            SQL.Append("," & SQ("2"))                                           ' STATUS_KBN_W (2:返還データ作成済)
            SQL.Append("," & "0")                                               ' END_KEN_W
            SQL.Append("," & "0")                                               ' END_KIN_W
            SQL.Append("," & SQ(""))                                            ' YOBI1_W
            SQL.Append("," & SQ(""))                                            ' YOBI2_W
            SQL.Append("," & SQ(""))                                            ' YOBI3_W
            SQL.Append("," & SQ(""))                                            ' YOBI4_W
            SQL.Append("," & SQ(""))                                            ' YOBI5_W
            SQL.Append("," & SQ(""))                                            ' YOBI6_W
            SQL.Append("," & SQ(""))                                            ' YOBI7_W
            SQL.Append("," & SQ(""))                                            ' YOBI8_W
            SQL.Append("," & SQ(""))                                            ' YOBI9_W
            SQL.Append("," & SQ(""))                                            ' YOBI10_W
            SQL.Append(")")

            If SubDB.ExecuteNonQuery(SQL) <= 0 Then
                ELog.Write("InsertWEB_RIREKIMAST　失敗　", Diagnostics.EventLogEntryType.Error)
                Return False
            End If

            OraReader.Close()

            dblock.InsertWEB_RIREKIMAST_UnLock(SubDB)
            SubDB.Commit()

            Return True

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "InsertWEB_RIREKIMAST", "", ex)

        Finally
            If Not dblock Is Nothing Then
                ' WEB_RIREKIMAST登録アンロック
                dblock.InsertWEB_RIREKIMAST_UnLock(SubDB)
            End If
            SubDB.Rollback()
        End Try

    End Function

    Private Function GetTextFileInfo(ByVal TextFileName As String, ByRef ArrayInfo As ArrayList) As Boolean

        Dim sr As StreamReader = Nothing

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "テキストファイル読込", "開始", TextFileName)

            sr = New StreamReader(TextFileName, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData As String = sr.ReadLine()
                ArrayInfo.Add(strLineData)
            End While

            Return True

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "テキストファイル読込", "", ex.Message)
            Return False
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "テキストファイル読込", "終了", "")
        End Try

    End Function

    Private Function MakePrintParam(ByVal PrintID As String, ByVal TextParam As String, ByVal Key As KeyInfo, ByRef PrintParam As String) As Boolean

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷パラメータ構築", "開始", "取引先コード:" & Key.TORIS_CODE & Key.TORIF_CODE & " / テキスト:" & TextParam)

            PrintParam = TextParam
            '---------------------------------------------------------
            ' パラメータ文字列置替
            '  <取引先マスタ系項目>
            '---------------------------------------------------------
            PrintParam = PrintParam.Replace("取引先コード", Key.TORIS_CODE & Key.TORIF_CODE)
            PrintParam = PrintParam.Replace("取引先主コード", Key.TORIS_CODE)
            PrintParam = PrintParam.Replace("取引先副コード", Key.TORIF_CODE)
            PrintParam = PrintParam.Replace("フォーマット区分", Key.FMT_KBN)
            PrintParam = PrintParam.Replace("コード区分", Key.CODE_KBN)
            PrintParam = PrintParam.Replace("マルチ区分", Key.MULTI_KBN)
            PrintParam = PrintParam.Replace("代表委託者コード", Key.ITAKU_KANRI_CODE)
            PrintParam = PrintParam.Replace("委託者コード", Key.ITAKU_CODE)
            PrintParam = PrintParam.Replace("媒体コード", strcTORI_PARAM.BAITAI_CODE)

            '---------------------------------------------------------
            ' パラメータ文字列置替
            '  <スケジュールマスタ系項目>
            '---------------------------------------------------------
            PrintParam = PrintParam.Replace("振替日", Key.FURI_DATE)

            '---------------------------------------------------------
            ' パラメータ文字列置替
            '  <その他項目>
            '---------------------------------------------------------
            Dim DQ As String = """"
            PrintParam = PrintParam.Replace("空白", DQ & DQ)

            '---------------------------------------------------------
            ' パラメータ文字列追加
            '  <媒体がWEB伝送時に通常印刷帳票のみに適用>
            '---------------------------------------------------------
            Select Case PrintID
                Case "KFJP018", "KFJP017", "KFJP019", "KFJP052"
                    If strcTORI_PARAM.BAITAI_CODE = "10" Then
                        PrintParam &= ",1"
                    End If
            End Select

            PrintParam = PrintParam.Replace("|", ",")

            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷パラメータ構築", "成功", "構築パラメータ:" & PrintParam)

            Return True

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷パラメータ構築", "", ex.Message)
            Return False
        Finally
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷パラメータ構築", "終了", "")
        End Try

    End Function

    Private Function PrintExeCheck(ByVal TextParam As String, ByVal Key As KeyInfo) As Boolean

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷条件チェック", "開始", TextParam)

            If TextParam.Split(CChar("_")).Length <> 3 Then
                Return True
            End If

            Dim CheckParam() As String = TextParam.Split(CChar("_"))

            Select Case CheckParam(1)
                Case "EQ"
                    Select Case CheckParam(0)
                        Case "取引先コード"
                            If CheckParam(2).IndexOf(Key.TORIS_CODE & Key.TORIF_CODE) < 0 Then
                                Return False
                            End If
                        Case "取引先主コード"
                            If CheckParam(2).IndexOf(Key.TORIS_CODE) < 0 Then
                                Return False
                            End If
                        Case "取引先副コード"
                            If CheckParam(2).IndexOf(Key.TORIF_CODE) < 0 Then
                                Return False
                            End If
                        Case "委託者コード"
                            If CheckParam(2).IndexOf(Key.ITAKU_CODE) < 0 Then
                                Return False
                            End If
                        Case "代表委託者コード"
                            If CheckParam(2).IndexOf(Key.ITAKU_KANRI_CODE) < 0 Then
                                Return False
                            End If
                        Case "フォーマット区分"
                            If CheckParam(2).IndexOf(Key.FMT_KBN) < 0 Then
                                Return False
                            End If
                        Case "媒体コード"
                            If CheckParam(2).IndexOf(strcTORI_PARAM.BAITAI_CODE) < 0 Then
                                Return False
                            End If
                    End Select
                Case "NOT"
                    Select Case CheckParam(0)
                        Case "取引先コード"
                            If CheckParam(2).IndexOf(Key.TORIS_CODE & Key.TORIF_CODE) >= 0 Then
                                Return False
                            End If
                        Case "取引先主コード"
                            If CheckParam(2).IndexOf(Key.TORIS_CODE) >= 0 Then
                                Return False
                            End If
                        Case "取引先副コード"
                            If CheckParam(2).IndexOf(Key.TORIF_CODE) >= 0 Then
                                Return False
                            End If
                        Case "委託者コード"
                            If CheckParam(2).IndexOf(Key.ITAKU_CODE) >= 0 Then
                                Return False
                            End If
                        Case "代表委託者コード"
                            If CheckParam(2).IndexOf(Key.ITAKU_KANRI_CODE) >= 0 Then
                                Return False
                            End If
                        Case "フォーマット区分"
                            If CheckParam(2).IndexOf(Key.FMT_KBN) >= 0 Then
                                Return False
                            End If
                        Case "媒体コード"
                            If CheckParam(2).IndexOf(strcTORI_PARAM.BAITAI_CODE) >= 0 Then
                                Return False
                            End If
                    End Select
            End Select

            Return True

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷条件チェック", "", ex.Message)
            Return False
        Finally
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷条件チェック", "終了", "")
        End Try

    End Function

    Private Function GetPrintInfo(ByVal PrtID As String, ByVal PrtName As String, ByRef PrtDevNameArray As String(), ByRef PrtDLL As String, ByRef PrtClass As String, ByRef PrtMethod As String) As Boolean

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷情報取得", "開始", PrtID & "_" & PrtName)

            'フォルダの末尾\チェック（なかったらつける）
            Dim filepath As String = CASTCommon.GetFSKJIni("COMMON", "PRT_FLD")
            If filepath.EndsWith("\") = False Then
                filepath = filepath & "\"
            End If

            '帳票印刷画面定義ファイル名の設定
            Dim PrtFileName As String = filepath & PrtID & "_" & PrtName & "\" & PrtID & "_" & PrtName & "_DSP.ini"

            'ファイル存在確認
            If Not System.IO.File.Exists(PrtFileName) Then
                LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷情報取得", "失敗", "印刷情報定義なし:" & PrtFileName)
                Return False
            End If

            'プリンタ名取得
            Dim prtrsplits As String() = CASTCommon.GetIniFileValues(PrtFileName, "PRINTERS", "PRINTER")
            If prtrsplits Is Nothing Then
                PrtDevNameArray = Nothing
            Else
                PrtDevNameArray = New String(prtrsplits.Length - 1) {}
                Dim i As Integer = 0
                For Each prtrvalue As String In prtrsplits
                    PrtDevNameArray(i) = prtrvalue.Trim
                    i += 1
                Next
            End If

            '業務固有印刷メソッド情報取得
            PrtDLL = CASTCommon.GetIniFileValue(PrtFileName, "EXTERNAL", "DLL")
            PrtClass = CASTCommon.GetIniFileValue(PrtFileName, "EXTERNAL", "CLASS")
            PrtMethod = CASTCommon.GetIniFileValue(PrtFileName, "EXTERNAL", "METHOD")

            If PrtDLL = "err" AndAlso
               PrtClass = "err" AndAlso
               PrtMethod = "err" Then
            Else
                If PrtDLL <> "err" AndAlso
                   PrtClass <> "err" AndAlso
                   PrtMethod <> "err" Then
                    PrtDLL = PrtDLL.Trim
                    PrtClass = PrtClass.Trim
                    PrtMethod = PrtMethod.Trim
                Else
                    '[EXTERNAL]セクションの定義が揃っていないため、エラー
                    LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷情報取得", "失敗", "[EXTERNAL]セクションの定義誤り:" & PrtFileName)
                    Return False
                End If
            End If

            Return True
        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷情報取得", "", ex.Message)
            Return False
        Finally
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "印刷情報取得", "終了", PrtID & "_" & PrtName)
        End Try

    End Function

    Private Function fn_BAITAI_FILE_CHK_XML(ByVal astrCHK_SEND_FILE As String, ByVal aintREC_LENGTH As Integer) As Boolean

        Dim HenkanFMT As New CAstFormat.CFormat
        Dim sRet As String = ""

        Dim SQL As StringBuilder = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Dim dblSCH_SYORI_KEN As Double
        Dim dblSCH_SYORI_KIN As Double
        Dim dblSCH_FURI_KEN As Double
        Dim dblSCH_FURI_KIN As Double
        Dim dblSCH_FUNOU_KEN As Double
        Dim dblSCH_FUNOU_KIN As Double
        Dim dblFILE_SYORI_KEN As Double
        Dim dblFILE_SYORI_KIN As Double
        Dim dblFILE_FURI_KEN As Double
        Dim dblFILE_FURI_KIN As Double
        Dim dblFILE_FUNOU_KEN As Double
        Dim dblFILE_FUNOU_KIN As Double
        Dim dblRECORD_COUNT As Double

        Try
            Dim sw As System.Diagnostics.Stopwatch = Nothing
            sw = LOG.Write_Enter1(LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "")

            dblRECORD_COUNT = 0

            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = strcTORI_PARAM.FMT_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)
            HenkanFMT.LOG = LOG

            If HenkanFMT.FirstRead(astrCHK_SEND_FILE) = 0 Then
                LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", "ファイル読込 対象ファイル:" & astrCHK_SEND_FILE)
                Return False
            End If

            Do Until HenkanFMT.EOF()
                dblRECORD_COUNT += 1
                sRet = HenkanFMT.CheckDataFormat()

                If HenkanFMT.IsHeaderRecord Then
                    '=========================================
                    ' ヘッダレコード
                    '=========================================
                    '-----------------------------------------
                    ' 取引先コード検索
                    '-----------------------------------------
                    SQL = New StringBuilder
                    SQL.Length = 0
                    If HenkanFMT.InfoMeisaiMast.ITAKU_CODE = "" Then
                        Return False
                    Else
                        SQL.Append("SELECT")
                        SQL.Append("     *")
                        SQL.Append(" FROM")
                        SQL.Append("     TORIMAST")
                        SQL.Append(" WHERE")
                        SQL.Append("     FSYORI_KBN_T = '1'")
                        SQL.Append(" AND ITAKU_CODE_T = '" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE) & "'")
                    End If

                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    If OraReader.DataReader(SQL) = False Then            '取引先コードが存在しない
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", "取引先該当なし　委託者コード:" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE))
                        OraReader.Close()
                        OraReader = Nothing
                        Return False
                    Else
                        strTORIF_CODE = clsFUSION.fn_chenge_null_value(OraReader.GetItem("TORIF_CODE_T"))
                        OraReader.Close()
                        OraReader = Nothing
                    End If

                    '------------------------------------
                    'スケジュール検索
                    '------------------------------------
                    SQL.Length = 0
                    SQL.Append("SELECT")
                    SQL.Append("     *")
                    SQL.Append(" FROM")
                    SQL.Append("     SCHMAST")
                    SQL.Append(" WHERE")
                    SQL.Append("     FSYORI_KBN_S     = '1'")
                    SQL.Append(" AND ITAKU_CODE_S     = '" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE) & "'")
                    SQL.Append(" AND FURI_DATE_S      = '" & Trim(strcKOBETUPARAM.strFURI_DATE) & "'")
                    SQL.Append(" AND FUNOU_FLG_S      = '1'")
                    If strcTORI_PARAM.MULTI_KBN = "0" Then  'シングル
                        SQL.Append(" AND TORIS_CODE_S = '" & Trim(strcKOBETUPARAM.strTORIS_CODE) & "'")
                        SQL.Append(" AND TORIF_CODE_S = '" & Trim(strcKOBETUPARAM.strTORIF_CODE) & "'")
                    End If

                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    If OraReader.DataReader(SQL) = False Then
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", "スケジュール該当なし")
                        OraReader.Close()
                        OraReader = Nothing
                        Return False
                    Else
                        dblSCH_SYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                        dblSCH_SYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")
                        dblSCH_FURI_KEN = OraReader.GetInt64("FURI_KEN_S")
                        dblSCH_FURI_KIN = OraReader.GetInt64("FURI_KIN_S")
                        dblSCH_FUNOU_KEN = OraReader.GetInt64("FUNOU_KEN_S")
                        dblSCH_FUNOU_KIN = OraReader.GetInt64("FUNOU_KIN_S")

                        OraReader.Close()
                        OraReader = Nothing
                    End If

                ElseIf HenkanFMT.IsTrailerRecord Then
                    '=========================================
                    ' トレーラレコード
                    '=========================================
                    dblFILE_SYORI_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO.Trim)
                    dblFILE_SYORI_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO.Trim)
                    dblFILE_FURI_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO.Trim)
                    dblFILE_FURI_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO.Trim)
                    dblFILE_FUNOU_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO.Trim)
                    dblFILE_FUNOU_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO.Trim)

                    '-----------------------------------------
                    ' 件数・金額チェック
                    '-----------------------------------------
                    If dblSCH_SYORI_KEN <> dblFILE_SYORI_KEN Then
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", "処理件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_SYORI_KIN <> dblFILE_SYORI_KIN Then
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", "処理金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_FUNOU_KEN <> dblFILE_FUNOU_KEN Then
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", "不能件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_FUNOU_KIN <> dblFILE_FUNOU_KIN Then
                        LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", "不能金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                End If
            Loop

            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "成功", "")

            Return True

        Catch ex As Exception
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "失敗", ex.Message)
            Return False
        Finally
            If Not HenkanFMT Is Nothing Then
                HenkanFMT.Close()
                HenkanFMT = Nothing
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "媒体書込み後検証", "終了", "")
        End Try

    End Function

    Private Function Check_Saifuri(ByVal TorisCode As String,
                                   ByVal TorifCode As String) As Boolean


        Dim SQL As New StringBuilder(128)
        Dim SAI_Reader As New CASTCommon.MyOracleReader(MainDB)

        Try
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック", "開始", "")

            SQL.Append("SELECT ")
            SQL.Append("    TORIS_CODE_T")
            SQL.Append(" FROM")
            SQL.Append("    TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T  = '" & TorisCode & "'")
            SQL.Append(" AND SFURI_FCODE_T = '" & TorifCode & "'")
            SQL.Append(" AND SFURI_FLG_T   = '1'")

            If SAI_Reader.DataReader(SQL) = True Then
                LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック", "成功", "再振取引先")
                Return True
            Else
                LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック", "成功", "初振取引先")
                Return False
            End If

        Catch ex As Exception
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック", "失敗 ", ex.Message)
        Finally
            If Not SAI_Reader Is Nothing Then
                SAI_Reader.Close()
                SAI_Reader = Nothing
            End If
            LOG.Write(LOG.UserID, LOG.ToriCode, LOG.FuriDate, "再振チェック", "終了", "")
        End Try

    End Function

End Class

Public Class myCompareClass
    Implements IComparer

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
        Implements IComparer.Compare

        Dim xA As ClsHenkan.KeyInfo = DirectCast(x, ClsHenkan.KeyInfo)
        Dim yA As ClsHenkan.KeyInfo = DirectCast(y, ClsHenkan.KeyInfo)

        Return xA.Matching(yA)
    End Function 'IComparer.Compare
End Class 'myReverserClass
