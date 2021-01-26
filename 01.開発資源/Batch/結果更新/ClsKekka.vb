Option Strict On
Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic
Imports clsFUSION.clsMain
Imports System.Windows.Forms
Imports CASTCommon.ModMessage
' センタ不能結果更新処理
Public Class ClsKekka

    ' センタ不能起動パラメータ 構造体
    Structure KOBETUPARAM
        Dim CP As CAstBatch.CommData.stPARAMETER
        Dim TKIN_CODE As String    '他行用金融機関コード
        Dim MOTIKOMI_KBN As String '持ち込み区分
        Dim KOUSIN_KBN As String   '更新区分(0:通常更新 1:強制更新)
        '                                                                       
        '固定長データ処理用プロパティ
        Public WriteOnly Property Data() As String
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                Try
                    Select Case para.Length
                        Case 4 '金庫/組合持込分
                            CP.FURI_DATE = para(0)
                            MOTIKOMI_KBN = para(1)
                            If MOTIKOMI_KBN <> "0" Then
                                Throw New Exception("引数異常")
                            End If
                            CP.MODE1 = para(2)
                            CP.JOBTUUBAN = Integer.Parse(para(3))      ' ジョブ通番
                            CP.RENKEI_FILENAME = "FUNOU.DAT"
                            ' フォーマット区分
                            CP.FMT_KBN = "MT"   '不能結果更新
                        Case 5 '企業持込分
                            CP.FURI_DATE = para(0)
                            MOTIKOMI_KBN = para(1)
                            '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                            Select Case MOTIKOMI_KBN
                                Case "1"
                                    CP.MODE1 = para(2)
                                    CP.TORI_CODE = para(3)
                                    CP.JOBTUUBAN = Integer.Parse(para(4))
                                    ' フォーマット区分
                                    CP.FMT_KBN = "00"   '全銀フォーマット

                                Case "4"
                                    CP.MODE1 = para(2)
                                    CP.FMT_KBN = para(3)
                                    CP.JOBTUUBAN = Integer.Parse(para(4))
                                    CP.RENKEI_FILENAME = "FUNOU_SYUKINDAIKOU.DAT"

                                Case Else
                                    Throw New Exception("引数異常")
                            End Select

                            'If MOTIKOMI_KBN <> "1" Then
                            '    Throw New Exception("引数異常")
                            'End If
                            'CP.MODE1 = para(2)
                            'CP.TORI_CODE = para(3)
                            'CP.JOBTUUBAN = Integer.Parse(para(4))
                            '' フォーマット区分
                            'CP.FMT_KBN = "00"   '全銀フォーマット
                            '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                        Case 7 '他行分
                            CP.FURI_DATE = para(0)
                            MOTIKOMI_KBN = para(1)
                            If MOTIKOMI_KBN <> "2" Then
                                Throw New Exception("引数異常")
                            End If
                            CP.MODE1 = para(2)
                            CP.TORI_CODE = para(3)
                            TKIN_CODE = para(4)
                            KOUSIN_KBN = para(5)
                            CP.JOBTUUBAN = Integer.Parse(para(6))
                            ' フォーマット区分
                            CP.FMT_KBN = "00"   '全銀フォーマット
                        Case Else '引数異常
                            Throw New Exception("引数異常")
                    End Select
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                End Try
            End Set
        End Property
    End Structure
    Private mKoParam As KOBETUPARAM

    ' ジョブメッセージ
    Dim mJobMessage As String = ""

    ' 起動パラメータ 共通情報
    Private mArgumentData As CommData

    ' 依頼データファイル名
    Private mDataFileName As String

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    'ＳＳＳ他行スケ更新不具合対応
    Private Jikinko As String

    '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
    Private vbDLL As New CMTCTRL.ClsFSKJ
    'Private vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

    '企業持込・他行振分設定用変数
    'ファイル名
    Private strIN_FILE_NAME As String
    Private strOUT_FILE_NAME As String
    '格納フォルダ
    Private DAT_Folder As String
    Private DEN_Folder As String
    Private TAK_Folder As String
    Private DENBK_Folder As String
    Private DATBK_Folder As String
    'MT/CMT読込用
    Private gstrMT As String
    Private strMT_FUNOU_FILE As String
    Private gstrCMT As String
    Private strCMT_FUNOU_FILE As String
    '他行マスタ検索用
    Private strTORIS_CODE As String
    Private strTORIF_CODE As String
    Private strTAKOU_KIN As String
    Private strTAKOU_ITAKU_CODE As String
    Private strTAKOU_ITAKU_KIN As String
    Private strTAKOU_ITAKU_SIT As String
    Private strTAKOU_ITAKU_KAMOKU As String
    Private strTAKOU_ITAKU_KOUZA As String
    Private strTAKO_BAITAI_CODE As String
    Private strTAKOU_S_FILE_NAME As String
    Private strR_FILE_NAME As String
    Private strCODE_KBN As String
    '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
    Private strLABEL_CODE As Integer
    'Private strLABEL_CODE As Short
    '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END
    Private TAKO_SEQ As String
    'FTRAN+定義ファイル 
    Public strP_FILE As String
    'フォーマット
    '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
    Private intREC_LENGTH As Integer
    Private intBLK_SIZE As Integer
    'Private intREC_LENGTH As Short
    'Private intBLK_SIZE As Short
    '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END
    'バックアップファイル名
    Private strBKUP_FILE As String
    'センター区分
    Private CENTER As String
    'ClsFusion
    Private clsFUSION As New clsFUSION.clsMain

    '農協コード
    Private MatomeNoukyo As String
    Private Noukyo_From As String
    Private Noukyo_To As String

    Private CountMinasi As Long = 0 'みなし更新件数追加
    '振替結果コード変換用内部クラス
    Protected Class FkekkaTbl
        Private FkekkaTblList As Hashtable      '振替結果テーブルリスト
        Private ToriCodeList As Hashtable       '取引先コードリスト

        '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
        'Public Sub New()
        Public Sub New(ByVal MainDB As CASTCommon.MyOracle)
        '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            '振替結果テーブルリスト作成 ここから
            FkekkaTblList = New Hashtable(10)

            '全振替結果テキストファイル取得
            Dim KekkaTxt() As String = Directory.GetFiles(CASTCommon.GetFSKJIni("COMMON", "TXT"), "振替結果コード*.txt")

            '各振替結果テキストファイル毎に内容取得
            For Each filename As String In KekkaTxt
                Dim sr As StreamReader = New StreamReader(filename)
                Dim ht As Hashtable = New Hashtable(30) '振替結果格納用

                While sr.Peek > -1
                    Dim s() As String = sr.ReadLine().Split("="c)
                    '=で分割できる行のみ格納
                    If s.Length = 2 Then
                        ht.Add(s(0), s(1))
                    End If
                End While
                sr.Close()

                'デフォルトファイル名の場合はテーブルID = 0
                If Path.GetFileName(filename).StartsWith("振替結果コード.") Then
                    FkekkaTblList.Add("0", ht)
                Else
                    'ファイル名からID取得
                    Dim id As String = Path.GetFileNameWithoutExtension(filename).Remove(0, 8)
                    FkekkaTblList.Add(id, ht)
                End If
            Next
            'ここまで

            '取引先コードリスト作成 ここから
            ToriCodeList = New Hashtable(3000)

            Dim SQL As StringBuilder = New StringBuilder(100)
            '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            'Dim oraReader As New CASTCommon.MyOracleReader
            Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_T || TORIF_CODE_T TORI_CODE")
            SQL.Append(",NVL(FKEKKA_TBL_T, '0') FKEKKA_TBL")
            SQL.Append(" FROM TORIMAST")

            With oraReader
                .DataReader(SQL)

                While .NextRead
                    ToriCodeList.Add(.GetString("TORI_CODE"), .GetString("FKEKKA_TBL"))
                End While

                .Close()
            End With
            'ここまで
        End Sub

        '振替結果テーブルリストのキーを取得する
        Private Function GetFkekkaTblListKey(ByVal TORI_CODE As String) As String
            If ToriCodeList.ContainsKey(TORI_CODE) Then
                Return ToriCodeList.Item(TORI_CODE).ToString
            End If

            '取引先コードが存在しない場合は標準の振替結果テーブルのキーを返す
            Return "0"
        End Function

        '振替結果テーブルリストから振替結果テーブルを取得する
        Private Function GetFkekkaTbl(ByVal id As String) As Hashtable
            If FkekkaTblList.ContainsKey(id) Then
                Return DirectCast(FkekkaTblList.Item(id), Hashtable)
            End If

            '振替結果テーブルが存在しない場合は標準の振替結果テーブルを返す
            Return DirectCast(FkekkaTblList.Item("0"), Hashtable)
        End Function

        '振替結果テーブルリストから振替結果コードを取得する
        Public Function GetKekkaCode(ByVal TORI_CODE As String, ByVal FURIKETU_CODE As String) As Integer
            Dim id As String = GetFkekkaTblListKey(TORI_CODE)
            Dim FkekkaTbl As Hashtable = GetFkekkaTbl(id)
            Dim code As String = CAInt32(FURIKETU_CODE).ToString

            If FkekkaTbl.ContainsKey(code) Then
                Return Convert.ToInt32(FkekkaTbl.Item(code))
            End If

            '振替結果コードが存在しない場合はELSE値を返す
            Return Convert.ToInt32(FkekkaTbl.Item("ELSE"))
        End Function
    End Class
    Private FKEKKA_TBL As FkekkaTbl

    ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- START
    Private INI_RSV2_SYORIKEKKA_FUNOU As String
    ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- END

    ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- START
    Private INI_RSV2_EDITION As String
    Private UnmatchRecordNum As Integer = 0
    ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- END
    
    '2017/03/13 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
    '預金口座振替変更通知書が０件で出力される不具合を修正
    Private HenkoutuutiCount As Integer = 0
    '2017/03/13 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END

    ' New
    Public Sub New()
    End Sub

    ' 機能　 ： センタ不能結果更新処理 メイン処理
    '
    ' 引数　 ： ARG1 - 起動パラメータ
    '
    ' 戻り値 ： True - 正常，False - 異常
    '
    ' 備考　 ： 
    '
    Function Main(ByVal command As String) As Integer
        ' パラメータチェック
        Try
            mKoParam.Data = command

            ' ジョブ通番設定
            MainLOG.JobTuuban = mKoParam.CP.JOBTUUBAN
            MainLOG.ToriCode = mKoParam.CP.TORI_CODE
            MainLOG.FuriDate = mKoParam.CP.FURI_DATE
            If MainLOG.UserID.Trim = "" Then
                MainLOG.Write("ログイン名取得", "失敗", "")
                MainLOG.UpdateJOBMASTbyErr("ログイン名取得失敗")
                Return -200
            End If
            LW.UserID = MainLOG.UserID
            LW.FuriDate = mKoParam.CP.FURI_DATE
            If SetIniFIle() = False Then
                MainLOG.UpdateJOBMASTbyErr("設定ファイル取得失敗")
                Return -300
            End If
        Catch ex As Exception
            MainLOG.Write("パラメタ設定", "失敗", "引数[" & command & "] " & ex.Message)
            If mKoParam.CP.JOBTUUBAN <> 0 Then
                MainLOG.UpdateJOBMASTbyErr("パラメータ設定失敗")
            End If
            Return -100
        End Try

        Try
            ' オラクル
            MainDB = New CASTCommon.MyOracle

            ' 起動パラメータ共通情報
            mArgumentData = New CommData(MainDB)

            ' パラメータ情報を設定
            Dim InfoParam As CommData.stPARAMETER = Nothing
            InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
            InfoParam.RENKEI_FILENAME = mKoParam.CP.RENKEI_FILENAME
            InfoParam.JOBTUUBAN = mKoParam.CP.JOBTUUBAN
            InfoParam.FURI_DATE = mKoParam.CP.FURI_DATE
            InfoParam.TORI_CODE = mKoParam.CP.TORI_CODE
            mArgumentData.INFOParameter = InfoParam
            If (mKoParam.CP.TORI_CODE).Trim <> "" Then
                LW.ToriCode = mKoParam.CP.TORI_CODE
                mArgumentData.GetTORIMAST(Mid(mKoParam.CP.TORI_CODE, 1, 10), Mid(mKoParam.CP.TORI_CODE, 11, 2))
                ' 2016/02/01 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(影響調査漏れ)) -------------------- START
                ''2009/12/18 他行の結果更新の場合、フォーマット区分を設定
                ''2011/01/04 信組対応 企業持込の場合、フォーマット区分を設定
                ''If mKoParam.MOTIKOMI_KBN = "2" Then
                'If mKoParam.MOTIKOMI_KBN = "2" OrElse mKoParam.MOTIKOMI_KBN = "1" Then
                '    mKoParam.CP.FMT_KBN = mArgumentData.INFOToriMast.FMT_KBN_T
                '    InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
                '    mArgumentData.INFOParameter = InfoParam
                'End If
                ''=======================================================
                '2017/05/26 タスク）西野 CHG 標準版修正（他行分の結果更新処理修正）-------------------------- START
                '他行分はフォーマット区分を変更しない(NEWしたときに「00:全銀」を指定している )
                If mKoParam.MOTIKOMI_KBN = "1" Then
                    'If mKoParam.MOTIKOMI_KBN = "2" OrElse mKoParam.MOTIKOMI_KBN = "1" Then
                    '2017/05/26 タスク）西野 CHG 標準版修正（他行分の結果更新処理修正）-------------------------- END
                    If CInt(mArgumentData.INFOToriMast.FMT_KBN_T.Trim) < 50 Then
                        mKoParam.CP.FMT_KBN = mArgumentData.INFOToriMast.FMT_KBN_T
                        InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
                        mArgumentData.INFOParameter = InfoParam
                    End If
                End If
                ' 2016/02/01 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(影響調査漏れ)) -------------------- END
            End If

            '2017/03/13 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
            '預金口座振替変更通知書が０件で出力される不具合を修正
            HenkoutuutiCount = 0
            '2017/03/13 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END

            Dim bRet As Boolean
            ' 結果更新処理
            bRet = TourokuMain()
            If bRet = False Then
            Else
                Dim oRenkei As New ClsRenkei(mArgumentData, 1)
                Dim DestFile As String = ""
                Try
                    If mKoParam.MOTIKOMI_KBN = "2" Then
                        If strIN_FILE_NAME.Trim <> "" AndAlso strTAKO_BAITAI_CODE = "00" Then
                            oRenkei.InFileName = strIN_FILE_NAME
                        End If
                    End If
                    '2010/12/24 信組対応 企業持込の場合のファイル退避追加
                    If mKoParam.MOTIKOMI_KBN = "1" Then
                        oRenkei.InFileName = strIN_FILE_NAME
                    End If
                    If oRenkei.InFileName <> "" Then
                        DestFile = oRenkei.InFileName
                        '履歴を保持するためリネーム
                        Dim ToFile As String = Path.Combine(DENBK_Folder, Now.ToString("dd") & "_" & Path.GetFileName(DestFile))
                        If File.Exists(ToFile) Then File.Delete(ToFile)
                        File.Move(DestFile, ToFile)
                        DestFile = ToFile
                        MainLOG.Write("入力ファイル正常フォルダへ移動", "成功", oRenkei.InFileName & " -> " & DestFile)
                    End If
                Catch ex As Exception
                    MainLOG.Write("入力ファイル正常フォルダへ移動", "失敗", oRenkei.InFileName & " -> " & DestFile & " " & ex.Message)
                End Try
            End If

            MainDB.Close()

            MainLOG.Write("登録終了", "成功", bRet.ToString)


            If bRet = False Then
                Return 2
            End If

        Catch ex As Exception
            Return 3
        End Try
        Return 0
    End Function
#Region " 設定ファイル取得"
    Private Function SetIniFIle() As Boolean
        Try
            '自金庫コード
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" OrElse Jikinko = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return False
            End If
            'センター区分
            Center = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If CENTER.ToUpper = "ERR" OrElse CENTER = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:センター区分 分類:COMMON 項目:CENTER")
                Return False
            End If
            'DAT格納フォルダ
            DAT_Folder = CASTCommon.GetFSKJIni("COMMON", "DAT")
            If DAT_Folder.ToUpper = "ERR" OrElse DAT_Folder = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:DAT格納フォルダ 分類:COMMON 項目:DAT")
                Return False
            End If
            '伝送格納フォルダ
            DEN_Folder = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If DEN_Folder.ToUpper = "ERR" OrElse DEN_Folder = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:伝送格納フォルダ 分類:COMMON 項目:DEN")
                Return False
            End If
            '他行格納ディレクトリ
            TAK_Folder = CASTCommon.GetFSKJIni("COMMON", "TAK")
            If TAK_Folder.ToUpper = "ERR" OrElse TAK_Folder = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:他行格納ディレクトリ 分類:COMMON 項目:TAK")
                Return False
            End If
            '伝送バックアップ格納フォルダ
            DENBK_Folder = CASTCommon.GetFSKJIni("COMMON", "DENBK")
            If DENBK_Folder.ToUpper = "ERR" OrElse DENBK_Folder = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:伝送バックアップ格納フォルダ 分類:COMMON 項目:DENBK")
                Return False
            End If
            '伝送バックアップ格納フォルダ
            DATBK_Folder = CASTCommon.GetFSKJIni("COMMON", "DATBK")
            If DATBK_Folder.ToUpper = "ERR" OrElse DATBK_Folder = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:DATバックアップ格納フォルダ 分類:COMMON 項目DATBK")
                Return False
            End If

            'MT接続情報
            gstrMT = CASTCommon.GetFSKJIni("COMMON", "MT")
            If gstrMT.ToUpper = "ERR" OrElse gstrMT = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:MT接続情報 分類:COMMON 項目:MT")
                Return False
            End If
            'MT不能ファイル名
            strMT_FUNOU_FILE = CASTCommon.GetFSKJIni("COMMON", "MTFUNOUFILE")
            If strMT_FUNOU_FILE.ToUpper = "ERR" OrElse strMT_FUNOU_FILE = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:MT不能ファイル名 分類:COMMON 項目:MTFUNOUFILE")
                Return False
            End If
            'CMT接続情報
            gstrCMT = CASTCommon.GetFSKJIni("COMMON", "CMT")
            If gstrCMT.ToUpper = "ERR" OrElse gstrCMT = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:CMT接続情報 分類:COMMON 項目:CMT")
                Return False
            End If
            'CMT不能ファイル名
            strCMT_FUNOU_FILE = CASTCommon.GetFSKJIni("COMMON", "CMTFUNOUFILE")
            If strCMT_FUNOU_FILE.ToUpper = "ERR" OrElse strCMT_FUNOU_FILE = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:CMT不能ファイル名 分類:COMMON 項目:CMTFUNOUFILE")
                Return False
            End If

            '他行企業シーケンス
            TAKO_SEQ = CASTCommon.GetFSKJIni("JIFURI", "TAKO_SEQ")
            If TAKO_SEQ.ToUpper = "ERR" OrElse TAKO_SEQ = Nothing Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:他行企業シーケンス 分類:JIFURI 項目:TAKO_SEQ")
                Return False
            End If

            '農協関連
            MatomeNoukyo = CASTCommon.GetFSKJIni("TAKO", "NOUKYOMATOME")
            If MatomeNoukyo.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:代表農協コード 分類:TAKO 項目:NOUKYOMATOME")
                Return False
            End If

            Noukyo_From = CASTCommon.GetFSKJIni("TAKO", "NOUKYOFROM")
            If Noukyo_From.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:農協コード(FROM) 分類:TAKO 項目:NOUKYOFROM")
                Return False
            End If

            Noukyo_To = CASTCommon.GetFSKJIni("TAKO", "NOUKYOTO")
            If Noukyo_To.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:農協コード(TO) 分類:TAKO 項目:NOUKYOTO")
                Return False
            End If

            ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- START
            INI_RSV2_SYORIKEKKA_FUNOU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYORIKEKKA_FUNOU")
            If INI_RSV2_SYORIKEKKA_FUNOU.ToUpper = "ERR" Then
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:処理結果確認表印刷要否 分類:RSV2_V1.0.0 項目:SYORIKEKKA_FUNOU")
                Return False
            End If
            ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- END

            ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- START
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If INI_RSV2_EDITION.ToUpper = "ERR" Or INI_RSV2_EDITION = "" Then
                INI_RSV2_EDITION = "1"
            End If
            ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- END

            Return True
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(設定ファイル取得)", "失敗", ex.ToString)

        End Try
    End Function
#End Region
#Region " 不能結果更新メイン処理"
    ' 機能　 ： 不能結果更新処理（センタ，他行，企業持込）
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function TourokuMain() As Boolean
        MainLOG.Write("登録処理開始", "成功", "")


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

        Try
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                MainLOG.Write_Err("登録処理", "失敗", "不能結果更新処理で実行待ちタイムアウト")
                MainLOG.UpdateJOBMASTbyErr("不能結果更新処理で実行待ちタイムアウト")
                Return False
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            ' パラメータ情報を設定
            Dim InfoParam As CommData.stPARAMETER = mArgumentData.INFOParameter
            InfoParam.FSYORI_KBN = mArgumentData.INFOToriMast.FSYORI_KBN_T
            mArgumentData.INFOParameter = InfoParam

            '媒体読み込み
            'フォーマット　
            Dim oReadFMT As CAstFormat.CFormat
            Try
                ' フォーマット区分から，フォーマットを特定する
                oReadFMT = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
                If oReadFMT Is Nothing Then
                    MainLOG.Write("フォーマット取得（規定外フォーマット）", "失敗")
                    MainLOG.UpdateJOBMASTbyErr("フォーマット取得（規定外フォーマット）")
                    Return False
                End If
                MainLOG.Write("フォーマット取得", "成功")
            Catch ex As Exception
                MainLOG.Write("フォーマット取得", "失敗", ex.Message)
                MainLOG.UpdateJOBMASTbyErr("フォーマット取得")
                Return False
            End Try

            Dim sReadFile As String = ""
            Try
                sReadFile = ReadMediaData(oReadFMT)
                ' 2016/06/14 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(標準バグ対応)) -------------------- START
                ' ソースが解析しづらい為変更
                ''他行かつ更新区分が強制更新の場合、ファイルなし
                ''他行かつ、媒体が依頼書の場合はファイルなし
                'If Not ((mKoParam.MOTIKOMI_KBN = "2" AndAlso mKoParam.KOUSIN_KBN = "1") OrElse _
                '        (mKoParam.MOTIKOMI_KBN = "2" AndAlso strTAKO_BAITAI_CODE = "04")) Then
                '    If sReadFile = "" Then  'ファイル取得失敗時はエラー
                '        oReadFMT.Close()
                '        MainLOG.Write("媒体読み込み", "失敗", oReadFMT.Message)
                '        MainLOG.UpdateJOBMASTbyErr("媒体読み込み失敗:" & oReadFMT.Message)
                '        Return False
                '    End If
                '    Call oReadFMT.FirstRead(sReadFile)
                'End If
                'MainLOG.Write("媒体読み込み", "成功")
                Select Case mKoParam.MOTIKOMI_KBN
                    Case "2"
                        '--------------------------------------------------------------
                        ' 不能結果（他行）
                        '--------------------------------------------------------------
                        If mKoParam.KOUSIN_KBN = "1" Then
                            '----------------------------------------------------------
                            ' 強制更新はファイル無し [sReadFile]  = ""
                            '----------------------------------------------------------
                        ElseIf strTAKO_BAITAI_CODE = "04" Then
                            '----------------------------------------------------------
                            ' 依頼書はファイル無し   [sReadFile]  = ""
                            '----------------------------------------------------------
                        Else
                            '----------------------------------------------------------
                            ' 他行の強制更新・依頼書以外は、ファイル名取得していない
                            ' 場合、エラーとする。
                            '----------------------------------------------------------
                            If sReadFile = "" Then  'ファイル取得失敗時はエラー
                                oReadFMT.Close()
                                MainLOG.Write("媒体読み込み", "失敗", oReadFMT.Message)
                                MainLOG.UpdateJOBMASTbyErr("媒体読み込み失敗:" & oReadFMT.Message)
                                Return False
                            End If
                            Call oReadFMT.FirstRead(sReadFile)
                        End If
                    Case Else
                        '--------------------------------------------------------------
                        ' 不能結果（他行以外）
                        ' ファイルが存在しない場合は、エラーとする。
                        ' ※金庫持込でデータがない場合は、画面で０バイトファイル作成済
                        '--------------------------------------------------------------
                        If sReadFile = "" Then  'ファイル取得失敗時はエラー
                            oReadFMT.Close()
                            MainLOG.Write("媒体読み込み", "失敗", oReadFMT.Message)
                            MainLOG.UpdateJOBMASTbyErr("媒体読み込み失敗:" & oReadFMT.Message)
                            Return False
                        End If
                        Call oReadFMT.FirstRead(sReadFile)
                End Select

                MainLOG.Write("媒体読み込み", "成功")

                ' 2016/06/14 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(標準バグ対応)) -------------------- END
            Catch ex As Exception
                MainLOG.Write("媒体読み込み", "失敗", ex.Message)
            End Try

            Select Case mKoParam.MOTIKOMI_KBN
                Case "0"    '金庫/組合持込
                    MainLOG.Write("金庫/組合持込開始", "成功", "")

                    ' センタ不能結果
                    ' 明細マスタ登録処理
                    Select Case CENTER
                        Case "0" 'SKC
                            'SKC不能フォーマット
                            If UpdateMeiMast(CType(oReadFMT, CAstFormat.CFormatSKCFunou)) = False Then
                                oReadFMT.Close()
                                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                                ' ジョブ実行アンロック
                                dblock.Job_UnLock(MainDB)
                                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                                ' ロールバック
                                MainDB.Rollback()
                                Return False
                            End If
                        Case "1" '北海道センター
                        Case "2", "3", "5", "6"  '東北センター,東京センター,大阪センター,中国センター
                            If UpdateMeiMast(CType(oReadFMT, CAstFormat.FUNOU_164_DATA)) = False Then
                                oReadFMT.Close()
                                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                                ' ジョブ実行アンロック
                                dblock.Job_UnLock(MainDB)
                                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                                ' ロールバック
                                MainDB.Rollback()
                                Return False
                            End If
                        Case "4" '東海センター
                            If UpdateMeiMast(CType(oReadFMT, CAstFormat.CFormatTokFunou)) = False Then
                                oReadFMT.Close()
                                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                                ' ジョブ実行アンロック
                                dblock.Job_UnLock(MainDB)
                                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                                ' ロールバック
                                MainDB.Rollback()
                                Return False
                            End If
                        Case "7"  '九州センター
                        Case Else

                    End Select
                Case "1" '企業持込不能結果
                    MainLOG.Write("企業持込登録開始", "成功", "")
                    '明細マスタ登録処理
                    If UpdateMeiMastKigyo(CType(oReadFMT, CAstFormat.CFormatZengin)) = False Then
                        oReadFMT.Close()
                        If File.Exists(strOUT_FILE_NAME) = True Then
                            File.Delete(strOUT_FILE_NAME)
                        End If

                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                        ' ジョブ実行アンロック
                        dblock.Job_UnLock(MainDB)
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                        ' ロールバック
                        MainDB.Rollback()
                        Return False
                    End If
                Case "2"
                    ' 他行不能結果
                    MainLOG.Write("他行振分登録開始", "成功", "")
                    ' 明細マスタ登録処理
                    If mKoParam.CP.FMT_KBN = "00" AndAlso _
                       UpdateMeiMastTako(CType(oReadFMT, CAstFormat.CFormatZengin)) = False Then    '条件追加
                        oReadFMT.Close()
                        If File.Exists(strOUT_FILE_NAME) = True Then
                            File.Delete(strOUT_FILE_NAME)
                        End If

                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                        ' ジョブ実行アンロック
                        dblock.Job_UnLock(MainDB)
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                        ' ロールバック
                        MainDB.Rollback()
                        Return False
                    End If
                    '2017/01/18 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                Case "4"
                    ' スリーエス不能結果
                    MainLOG.Write("ＳＳＳ持込登録開始", "成功", "")
                    ' 明細マスタ登録処理
                    If UpdateMeiMastSSS(CType(oReadFMT, CAstFormat.CFormatZengin)) = False Then
                        oReadFMT.Close()
                        If File.Exists(strOUT_FILE_NAME) = True Then
                            File.Delete(strOUT_FILE_NAME)
                        End If

                        ' ロールバック
                        MainDB.Rollback()
                        Return False
                    End If
                    '2017/01/18 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
            End Select

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応＆トラン明確化 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)

            ' ＤＢコミット（UpdateMeiMastメソッドではなく呼出し元でコミットするように変更）
            MainDB.Commit()

            '2017/03/13 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
            '預金口座振替変更通知書が０件で出力される不具合を修正
            If HenkoutuutiCount > 0 Then

                Dim HENKOU_PRINT_FLG As String = "0"
                HENKOU_PRINT_FLG = CASTCommon.GetFSKJIni("PRINT", "HENKOUTUCHI_PRINT")
                If HENKOU_PRINT_FLG = "err" OrElse HENKOU_PRINT_FLG = "" Then
                    MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:変更通知書印刷フラグ 分類:PRINT 項目:HENKOUTUCHI_PRINT")
                    HENKOU_PRINT_FLG = "0"          'iniファイルの取得失敗した場合は、印刷しない。
                End If
                If HENKOU_PRINT_FLG = "1" Then
                    ''------------------------------------------
                    '' レポエージェント印刷
                    ''------------------------------------------
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ' 預金口座振替解約・変更通知書
                    Dim Param As String = LW.UserID & "," & mKoParam.CP.FURI_DATE
                    MainLOG.Write("預金口座振替変更通知書印刷バッチ呼出", "成功", Param)
                    Dim nRet As Integer = ExeRepo.ExecReport("KFJP012.EXE", Param)
                    Select Case nRet
                        Case 0
                            MainLOG.Write("預金口座振替変更通知書印刷", "成功", "")
                        Case -1
                            MainLOG.Write("預金口座振替変更通知書印刷", "失敗", "印刷対象なし")
                        Case Else
                            MainLOG.Write("預金口座振替変更通知書印刷", "失敗", "")
                            MainLOG.UpdateJOBMASTbyErr("預金口座振替変更通知書印刷 失敗")
                            Return False
                    End Select
                End If
            End If
            '2017/03/13 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END

            ' トランザクション開始
            ' 現状、暗黙のトランであるため明示的コミットはNOPとなりコネクションクロース時にコミットされるので
            ' 明示的にトランを開始する
            MainDB.BeginTrans()
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応＆トラン明確化 ***

            '手数料計算をジョブ登録
            Dim jobid As String
            Dim para As String
            Try
                'ジョブマスタに登録
                jobid = "J070"
                'パラメータ(振替日)
                para = mKoParam.CP.FURI_DATE
                'job検索
                Dim iRet As Integer
                iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                If iRet = 1 Then    'ジョブ登録済
                    MainLOG.Write("手数料計算登録", "失敗", "ジョブ登録済み")
                ElseIf iRet = -1 Then 'ジョブ検索失敗
                    MainLOG.Write("手数料計算登録", "失敗")
                Else
                    'job登録
                    If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料計算登録", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                    End If
                End If
            Catch ex As Exception
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料計算登録", "失敗", ex.ToString)
            End Try

            If sReadFile = oReadFMT.FileName Then
                oReadFMT.Close()
            Else
                oReadFMT.Close()
                File.Delete(sReadFile)
            End If

            ' JOBマスタ更新
            If MainLOG.UpdateJOBMASTbyOK(mJobMessage) = False Then
                ' ロールバック
                MainDB.Rollback()
                Return False
            End If
            MainLOG.Write("ジョブマスタ更新", "成功")


            ' ＤＢコミット
            MainDB.Commit()

            Return True

        Finally
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)

            ' ロールバック
            MainDB.Rollback()
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        End Try
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

    End Function
    ' 機能　 ： 依頼データを読み込む
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Public Function ReadMediaData(ByVal readfmt As CAstFormat.CFormat) As String
        Try
            ' 媒体読み込み結果
            Dim nRetRead As Integer

            ' 連携
            Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData, 1)

            If mKoParam.CP.FMT_KBN = "MT" Then
                ' センター不能結果が０バイトの場合の処理

                Dim InfileInfo As New FileInfo(oRenkei.InFileName)
                If InfileInfo.Exists() = True Then
                    If InfileInfo.Length = 0 Then
                        ' 実際０バイトでした
                        oRenkei.OutFileName = oRenkei.InFileName

                        Return oRenkei.OutFileName
                    End If
                End If
            End If

            Select Case mKoParam.MOTIKOMI_KBN
                Case "0" '金庫/信組
                    nRetRead = oRenkei.CopyToDisk(readfmt)
                    'Return         :0=成功、50=ファイルなし、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
                    '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
                    Select Case nRetRead
                        Case 0
                            mDataFileName = oRenkei.OutFileName
                            MainLOG.Write("ファイル取込", "成功", "ファイル名：" & oRenkei.InFileName)
                        Case 10
                            MainLOG.Write("ファイル取込", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル形式異常、ファイル名：" & oRenkei.InFileName)
                            Return ""
                        Case 50
                            MainLOG.Write("ファイル検索", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイルなし、ファイル名：" & oRenkei.InFileName)
                            Return ""
                        Case 100
                            MainLOG.Write("ファイル取込（コード変換）", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（コード変換）失敗")
                            Return ""
                        Case 200
                            MainLOG.Write("ファイル取込（コード区分異常（JIS改行あり））", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（コード区分異常（JIS改行あり））失敗")
                            Return ""
                        Case 300
                            MainLOG.Write("ファイル取込（コード区分異常（JIS改行なし））", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（コード区分異常（JIS改行なし））失敗")
                            Return ""
                        Case 400
                            MainLOG.Write("ファイル取込（出力ファイル作成）", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（出力ファイル作成）失敗")
                            Return ""
                    End Select
                Case "1" '企業持込
                    If fn_BAITAI_READ() Then
                        oRenkei.OutFileName = strOUT_FILE_NAME
                    Else
                        oRenkei.OutFileName = ""
                    End If
                Case "2" '他行振分
                    ' 2016/06/14 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(標準バグ対応)) -------------------- START
                    ' ソースが解析しづらい＆但馬信金でバグ発生の為変更
                    ''他行情報を取得する
                    ''媒体からファイルを取り込む(依頼書または更新区分が1:強制更新の場合はファイルなし)
                    'If fn_TAKO_BAITAI_READ() OrElse (strTAKO_BAITAI_CODE <> "04" AndAlso mKoParam.KOUSIN_KBN <> "1") Then
                    '    oRenkei.OutFileName = strOUT_FILE_NAME
                    'Else
                    '    oRenkei.OutFileName = ""
                    'End If
                    If fn_TAKO_BAITAI_READ() = True Then
                        If mKoParam.KOUSIN_KBN = "1" Then
                            '----------------------------------------------
                            ' 強制更新     [sReadFile]を ""(空白) とする
                            '----------------------------------------------
                            oRenkei.OutFileName = ""
                        ElseIf strTAKO_BAITAI_CODE = "04" Then
                            '----------------------------------------------
                            ' 依頼書       [sReadFile]を ""(空白) とする
                            '----------------------------------------------
                            oRenkei.OutFileName = ""
                        Else
                            '----------------------------------------------
                            ' 正常時       [sReadFile]にファイル名を返す
                            '----------------------------------------------
                            oRenkei.OutFileName = strOUT_FILE_NAME
                        End If
                    Else
                        '--------------------------------------------------
                        ' エラー時         [sReadFile]を ""(空白) とする
                        '--------------------------------------------------
                        oRenkei.OutFileName = ""
                    End If
                    ' 2016/06/14 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(標準バグ対応)) -------------------- END
                    '2017/01/18 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                Case "4"    'ＳＳＳ持込
                    nRetRead = oRenkei.CopyToDisk(readfmt)
                    'Return         :0=成功、50=ファイルなし、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
                    '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
                    Select Case nRetRead
                        Case 0
                            mDataFileName = oRenkei.OutFileName
                            MainLOG.Write("ファイル取込", "成功", "ファイル名：" & oRenkei.InFileName)
                        Case 10
                            MainLOG.Write("ファイル取込", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル形式異常、ファイル名：" & oRenkei.InFileName)
                            Return ""
                        Case 50
                            MainLOG.Write("ファイル検索", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイルなし、ファイル名：" & oRenkei.InFileName)
                            Return ""
                        Case 100
                            MainLOG.Write("ファイル取込（コード変換）", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（コード変換）失敗")
                            Return ""
                        Case 200
                            MainLOG.Write("ファイル取込（コード区分異常（JIS改行あり））", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（コード区分異常（JIS改行あり））失敗")
                            Return ""
                        Case 300
                            MainLOG.Write("ファイル取込（コード区分異常（JIS改行なし））", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（コード区分異常（JIS改行なし））失敗")
                            Return ""
                        Case 400
                            MainLOG.Write("ファイル取込（出力ファイル作成）", "失敗", oRenkei.Message)
                            MainLOG.UpdateJOBMASTbyErr("ファイル取込（出力ファイル作成）失敗")
                            Return ""
                    End Select
                    '2017/01/18 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
            End Select

            Return oRenkei.OutFileName

        Catch ex As Exception
            MainLOG.Write("依頼データ読込", "失敗", ex.ToString)
            Return ""
        Finally

        End Try

    End Function

#End Region

#Region "東海センター用明細更新"
    ' 機能　 ： 明細マスタ登録処理(東海センター用)
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateMeiMast(ByVal aReadFmt As CAstFormat.CFormatTokFunou) As Boolean
        Dim dblRECORD_COUNT As Integer = 0
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal


        Dim dblKIGYO_RECORD_COUNT As Integer = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' チェック処理結果

        Dim SQL As StringBuilder = New StringBuilder(512)

        Dim UpdateKeys As New ArrayList(1000)

        Dim strTODAY As String = DateTime.Today.ToString("yyyyMMdd")

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        ' 全データチェック
        Try
            'パラメータの振替日が無ければシステム日付の１営業日前を振替日とする
            If mKoParam.CP.FURI_DATE Is Nothing OrElse mKoParam.CP.FURI_DATE.Trim = "" Then
                Dim oFmt As New CAstFormat.CFormat
                oFmt.Oracle = MainDB
                Dim ZenDay As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, -1, oFmt.HolidayList)
                oFmt = Nothing
                mKoParam.CP.FURI_DATE = ZenDay.ToString("yyyyMMdd")
            End If

            ' 起動パラメータ共通情報
            aReadFmt.ToriData = mArgumentData

            Dim InfileInfo As New FileInfo(aReadFmt.FileName)
            If InfileInfo.Exists() = True AndAlso InfileInfo.Length = 0 Then
                ' 実際０バイトでした
                MainLOG.Write("不能結果件数０件処理", "成功")
                Dim Key(0) As String
                '東海センター複数振替日対応 ***
                Key(0) = mKoParam.CP.FURI_DATE
                '********************************************************
                UpdateKeys.Add(Key)
            Else
                ' 不能結果ファイル読み込み開始
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("ファイルオープン", "失敗", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("ファイルオープン失敗")
                    Return False
                End If
            End If

            Dim stMei As CAstFormat.CFormat.MEISAI      ' 明細マスタ情報

            '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            'Dim FKEKKA_TBL As New FkekkaTbl
            Dim FKEKKA_TBL As New FkekkaTbl(MainDB)
            '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

            Do Until aReadFmt.EOF
                ' データを読み込んで，フォーマットチェックを行う
                sCheckRet = aReadFmt.CheckKekkaFormat()

                dblRECORD_COUNT += 1

                If aReadFmt.IsHeaderRecord = True Then
                    ' ヘッダ
                ElseIf aReadFmt.IsDataRecord = True Then
                    'ヘッダチェックはしない
                    stMei = aReadFmt.InfoMeisaiMast
                    MainLOG.FuriDate = stMei.FURIKAE_DATE

                    '東海センター複数振替日対応 振替日以外の明細は無視する
                    If stMei.FURIKAE_DATE = mKoParam.CP.FURI_DATE Then
                        dblALL_KEN += 1
                        dblALL_KINGAKU += stMei.FURIKIN

                        Dim intCOUNT As Integer = 0
                        '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                        OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                        SQL.Length = 0
                        SQL.Append("SELECT COUNT(*)")
                        SQL.Append(", MAX(TORIS_CODE_K || TORIF_CODE_K)")
                        SQL.Append(" FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                        SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                        If mKoParam.CP.MODE1 = "0" Then
                            ' 更新キーが企業シーケンス
                            SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                        Else
                            ' 更新キーが口座情報
                            SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                            SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                            SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                            SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                            SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                            '2010/01/19 カナ変換後の需要家番号をYOBI4_Kに設定
                            'SQL.Append("   AND JYUYOUKA_NO_K = " & SQ(stMei.JYUYOKA_NO))
                            ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                            'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            Select Case mKoParam.CP.MODE1
                                Case "9"
                                    ' MODE1="9"は、需要家番号を条件からはずし更新する
                                Case Else
                                    SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            End Select
                            ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                            '================================================
                        End If
                        If OraMeiReader.DataReader(SQL) = True Then
                            intCOUNT = OraMeiReader.GetValueInt(0)
                        End If

                        Select Case stMei.KIGYO_SEQ.Chars(0)
                            Case "2"c, "5"c, "6"c
                                If intCOUNT = 0 Then
                                    '検索結果が0件の場合、メッセージを表示
                                    MainLOG.Write("明細検索", "失敗", "振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                    '明細が見つからなくても異常終了としない
                                    'LOG.UpdateJOBMASTbyErr("明細検索失敗 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                    'Return False
                                    GoTo NEXT_RECORD
                                    '********************************************************************
                                ElseIf intCOUNT > 1 Then
                                    MainLOG.Write("明細検索", "失敗", "同一条件明細複数存在　振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                    '異常終了にしない
                                    'LOG.UpdateJOBMASTbyErr("明細検索同一条件明細複数存在 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                    mJobMessage = "明細検索同一条件明細複数存在 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ
                                    MainLOG.UpdateJOBMAST(mJobMessage)
                                    '**********************************************
                                End If

                                '' 振替結果コード変換
                                Dim ToriCode As String = OraMeiReader.GetValue(1).ToString
                                stMei.FURIKETU_CODE = FKEKKA_TBL.GetKekkaCode(ToriCode, aReadFmt.InfoMeisaiMast.FURIKETU_CENTERCODE)
                                '************************************************************

                                '更新
                                SQL = New StringBuilder(128)
                                SQL.Append("UPDATE MEIMAST")
                                SQL.Append(" SET")
                                SQL.Append("  FURIKETU_CODE_K = " & stMei.FURIKETU_CODE.ToString)
                                SQL.Append(", FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_CENTERCODE.ToString)
                                SQL.Append(", MINASI_K = " & SQ(stMei.MINASI))
                                '訂正情報を格納
                                SQL.Append(", TEISEI_SIT_K     = " & SQ(stMei.TEISEI_SIT))
                                SQL.Append(", TEISEI_KAMOKU_K  = " & SQ(stMei.TEISEI_KAMOKU))
                                SQL.Append(", TEISEI_KOUZA_K   = " & SQ(stMei.TEISEI_KOUZA))
                                SQL.Append(", TEISEI_AKAMOKU_K = " & SQ(stMei.TEISEI_AKAMOKU))
                                SQL.Append(", TEISEI_AKOUZA_K  = " & SQ(stMei.TEISEI_AKOUZA))
                                SQL.Append(", KOUSIN_DATE_K = " & SQ(strTODAY))

                                SQL.Append("WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                                SQL.Append("  AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                                If mKoParam.CP.MODE1 = "0" Then
                                    ' 更新キーが企業シーケンス
                                    SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                                Else
                                    ' 更新キーが口座情報
                                    '複数見つかった時は見つかった企業SEQの最小のもののみ更新する
                                    SQL.Append("   AND KIGYO_SEQ_K = ")
                                    SQL.Append("   (SELECT MIN(KIGYO_SEQ_K) FROM MEIMAST WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                                    SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                                    SQL.Append("   AND FURIKETU_CODE_K = '0' ")
                                    SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                                    SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                                    SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                                    SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                                    SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                                    ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                                    'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                    Select Case mKoParam.CP.MODE1
                                        Case "9"
                                            ' MODE1="9"は、需要家番号を条件からはずし更新する
                                        Case Else
                                            SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                    End Select
                                    ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                                    SQL.Append("   )")
                                End If

                                Try
                                    MainDB.ExecuteNonQuery(SQL)
                                    dblKOUSIN_KEN += 1
                                    If aReadFmt.InfoMeisaiMast.MINASI = "1" Then
                                        CountMinasi += 1
                                    End If
                                Catch ex As Exception
                                    MainLOG.Write("明細更新", "失敗", "振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                                    MainLOG.UpdateJOBMASTbyErr("明細更新 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                    Return False
                                End Try

                                ' スケジュールキー情報保存
                                Dim Key(0) As String
                                Key(0) = stMei.FURIKAE_DATE

                                Dim oSearch As New mySearchClass
                                If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                                    '東海センター不能データ仕様 振替日は２日分入ってくるため異常としない
                                    'If mKoParam.CP.RENKEI_KBN = "99" AndAlso mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                                    '    LOG.Write("振替日不一致", "失敗", "入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                                    '    LOG.UpdateJOBMASTbyErr("振替日不一致 入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                                    '    Return False
                                    'End If
                                    '*************************************************************************************************
                                    UpdateKeys.Add(Key)
                                End If

                                OraMeiReader.Close()

                            Case Else
                                '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                                OraMeiReader.Close()
                                '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                                '金庫持込以外
                                dblKIGYO_RECORD_COUNT += 1

                        End Select
                    End If
                End If
NEXT_RECORD:
            Loop

            '東海センター複数振替日対応
            '指定振替日の更新明細がなくとも、必ず指定振替日の結果更新を行う
            Dim opSearch As New mySearchClass
            Dim ParamKey(0) As String
            ParamKey(0) = mKoParam.CP.FURI_DATE
            If UpdateKeys.BinarySearch(ParamKey, opSearch) < 0 Then
                UpdateKeys.Add(ParamKey)
            End If
            '********************************************************
            For i As Integer = 0 To UpdateKeys.Count - 1
                ' スケジュール更新
                Dim Keys() As String = CType(UpdateKeys(i), String())
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

            'エラーメッセージがない場合
            If mJobMessage = "" Then
                '件数表示 レコード件数追加
                'レコード件数・・・全レコード件数(ヘッダがある場合は含まれる)
                '読込件数・・・・・指定振替日のみのデータレコード件数
                '更新件数・・・・・実際に更新されたレコード件数
                '※読込件数・更新件数が０件の場合は、振替日が違う可能性有り
                mJobMessage = "レコード件数：" & dblRECORD_COUNT & " 読込件数：" & dblALL_KEN & " 更新件数：" & dblKOUSIN_KEN & " みなし更新件数:" & CountMinasi
                MainLOG.Write("結果更新", "成功", mJobMessage)
            End If

            '2017/03/13 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '預金口座振替変更通知書が０件で出力される不具合を修正
            'TourokuMainのコミット後に印刷する
            HenkoutuutiCount = UpdateKeys.Count
            'If UpdateKeys.Count > 0 Then
            '    '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------START
            '    Dim HENKOU_PRINT_FLG As String = "0"
            '    HENKOU_PRINT_FLG = CASTCommon.GetFSKJIni("PRINT", "HENKOUTUCHI_PRINT")
            '    If HENKOU_PRINT_FLG = "err" OrElse HENKOU_PRINT_FLG = "" Then
            '        MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:変更通知書印刷フラグ 分類:PRINT 項目:HENKOUTUCHI_PRINT")
            '        HENKOU_PRINT_FLG = "0"          'iniファイルの取得失敗した場合は、印刷しない。
            '    End If
            '    If HENKOU_PRINT_FLG = "1" Then
            '        '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------END

            '        ''------------------------------------------
            '        '' レポエージェント印刷
            '        ''------------------------------------------
            '        Dim ExeRepo As New CAstReports.ClsExecute
            '        ' 預金口座振替解約・変更通知書
            '        Dim Keys() As String = CType(UpdateKeys(0), String())
            '        Dim Param As String = LW.UserID & "," & Keys(0).TrimEnd
            '        MainLOG.Write("預金口座振替変更通知書印刷バッチ呼出", "成功", Param)
            '        Dim nRet As Integer = ExeRepo.ExecReport("KFJP012.EXE", Param)
            '        Select Case nRet
            '            Case 0
            '                MainLOG.Write("預金口座振替変更通知書印刷", "成功", "")
            '            Case -1
            '                MainLOG.Write("預金口座振替変更通知書印刷", "失敗", "印刷対象なし")
            '            Case Else
            '                MainLOG.Write("預金口座振替変更通知書印刷", "失敗", "")
            '                MainLOG.UpdateJOBMASTbyErr("預金口座振替変更通知書印刷 失敗")
            '                Return False
            '        End Select
            '        '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------START
            '    End If
            '    '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------END
            'End If
            '2017/03/13 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END

        Catch ex As Exception
            MainLOG.Write("明細マスタ登録処理", "失敗", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("システムエラー（ログ参照）")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        End Try
        Return True
    End Function

#End Region

#Region "東北・東京・大阪・中国センター用明細更新"
    ' 機能　 ： 明細マスタ登録処理(東北・東京・大阪・中国センター)
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateMeiMast(ByVal aReadFmt As CAstFormat.FUNOU_164_DATA) As Boolean
        Dim dblRECORD_COUNT As Integer = 0
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Integer = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' チェック処理結果

        Dim SQL As StringBuilder = New StringBuilder(512)

        Dim UpdateKeys As New ArrayList(1000)

        Dim strTODAY As String = DateTime.Today.ToString("yyyyMMdd")

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***


        ' 全データチェック
        Try
            ' 起動パラメータ共通情報
            aReadFmt.ToriData = mArgumentData
            Dim InfileInfo As New FileInfo(aReadFmt.FileName)
            If InfileInfo.Exists() = True AndAlso InfileInfo.Length = 0 Then
                ' 実際０バイトでした
                MainLOG.Write("不能結果件数０件処理", "成功")

                Dim Key(0) As String
                If mKoParam.CP.FURI_DATE Is Nothing OrElse mKoParam.CP.FURI_DATE.Trim = "" Then
                    ' 前営業日
                    Dim oFmt As New CAstFormat.CFormat
                    oFmt.Oracle = MainDB
                    Dim ZenDay As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, -1, oFmt.HolidayList)
                    oFmt = Nothing
                    Key(0) = ZenDay.ToString("yyyyMMdd")
                Else
                    Key(0) = mKoParam.CP.FURI_DATE
                End If
                UpdateKeys.Add(Key)
            Else
                ' 不能結果ファイル読み込み開始
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("ファイルオープン", "失敗", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("ファイルオープン失敗")
                    Return False
                End If
            End If

            Dim stMei As CAstFormat.CFormat.MEISAI      ' 明細マスタ情報
            '振替結果テーブルロード 

            '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            'Dim FKEKKA_TBL As New FkekkaTbl
            Dim FKEKKA_TBL As New FkekkaTbl(MainDB)
            '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

            Do Until aReadFmt.EOF
                ' データを読み込んで，フォーマットチェックを行う
                sCheckRet = aReadFmt.CheckKekkaFormat()

                dblRECORD_COUNT += 1

                If aReadFmt.IsDataRecord = True Then

                    stMei = aReadFmt.InfoMeisaiMast

                    MainLOG.FuriDate = stMei.FURIKAE_DATE

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    Dim intCOUNT As Integer = 0
                    '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                    'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                    OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                    SQL.Length = 0
                    SQL.Append("SELECT COUNT(*)")
                    SQL.Append(", MAX(TORIS_CODE_K || TORIF_CODE_K)")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                    SQL.Append("   AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                    If mKoParam.CP.MODE1 = "0" Then
                        ' 更新キーが企業シーケンス
                        SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                    Else
                        ' 更新キーが口座情報
                        SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                        SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        '振替金額数値化
                        SQL.Append("   AND FURIKIN_K = " & CInt(stMei.FURIKIN))
                        '2010/01/19 カナ変換後の需要家番号をYOBI4_Kに設定
                        ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                        ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                        'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                        Select Case mKoParam.CP.MODE1
                            Case "9"
                                ' MODE1="9"は、需要家番号を条件からはずし更新する
                            Case Else
                                SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                        End Select
                        ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                        '================================================
                    End If
                    If OraMeiReader.DataReader(SQL) = True Then
                        intCOUNT = OraMeiReader.GetValueInt(0)
                    End If

                    Select Case stMei.KIGYO_SEQ.Chars(0)
                        Case "2"c, "5"c, "6"c
                            If intCOUNT = 0 Then
                                '検索結果が0件の場合、メッセージを表示
                                ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- START
                                'With stMei
                                '    If MessageBox.Show(String.Format(MSG0074I, .HENKANKBN, _
                                '                                               .FURIKAE_DATE, _
                                '                                               .KIGYO_CODE, _
                                '                                               .KEIYAKU_KIN, .KEIYAKU_SIT, _
                                '                                               .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA, _
                                '                                               .KIGYO_SEQ), _
                                '                        msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                                '        GoTo NEXT_RECORD
                                '    Else
                                '        MainLOG.Write("明細検索", "失敗", "振替日：" & .FURIKAE_DATE & " 企業コード：" & .KIGYO_CODE & " 企業シーケンス：" & .KIGYO_SEQ)
                                '        MainLOG.UpdateJOBMASTbyErr("明細検索失敗 振替日：" & .FURIKAE_DATE & " 企業コード：" & .KIGYO_CODE & " 企業シーケンス：" & .KIGYO_SEQ)
                                '        Return False
                                '    End If
                                '    MainLOG.Write("明細検索", "失敗", "振替日：" & .FURIKAE_DATE & " 企業コード：" & .KIGYO_CODE & " 企業シーケンス：" & .KIGYO_SEQ)

                                'End With
                                Select Case INI_RSV2_EDITION
                                    Case "2"
                                        With stMei
                                            MainLOG.Write("明細検索", "失敗", "振替日：" & .FURIKAE_DATE & " 企業コード：" & .KIGYO_CODE & " 企業シーケンス：" & .KIGYO_SEQ)
                                            UnmatchRecordNum += 1
                                        End With
                                        GoTo NEXT_RECORD
                                    Case Else
                                        With stMei
                                            If MessageBox.Show(String.Format(MSG0074I, .HENKANKBN, _
                                                                                       .FURIKAE_DATE, _
                                                                                       .KIGYO_CODE, _
                                                                                       .KEIYAKU_KIN, .KEIYAKU_SIT, _
                                                                                       .KEIYAKU_KAMOKU, .KEIYAKU_KOUZA, _
                                                                                       .KIGYO_SEQ), _
                                                                msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                                                GoTo NEXT_RECORD
                                            Else
                                                MainLOG.Write("明細検索", "失敗", "振替日：" & .FURIKAE_DATE & " 企業コード：" & .KIGYO_CODE & " 企業シーケンス：" & .KIGYO_SEQ)
                                                MainLOG.UpdateJOBMASTbyErr("明細検索失敗 振替日：" & .FURIKAE_DATE & " 企業コード：" & .KIGYO_CODE & " 企業シーケンス：" & .KIGYO_SEQ)
                                                Return False
                                            End If
                                        End With
                                End Select
                                ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- END
                                'MainLOG.Write("明細検索", "失敗", "振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                'MainLOG.UpdateJOBMASTbyErr("明細検索失敗 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                'Return False
                                '2011/06/16 標準版修正 マスタに存在しない明細を考慮 ------------------END

                            ElseIf intCOUNT > 1 Then
                                MainLOG.Write("明細検索", "失敗", "同一条件明細複数存在　振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                ' 異常終了にしない
                                'LOG.UpdateJOBMASTbyErr("明細検索同一条件明細複数存在 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                mJobMessage = "明細検索同一条件明細複数存在 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ
                                MainLOG.UpdateJOBMAST(mJobMessage)
                            End If

                            Dim ToriCode As String = OraMeiReader.GetValue(1).ToString

                            'みなし完了フラグが"1"の場合、振替結果コード0に更新する。※センター結果コードがそのまま
                            Select Case aReadFmt.InfoMeisaiMast.MINASI
                                Case "1"
                                    stMei.FURIKETU_CODE = FKEKKA_TBL.GetKekkaCode(ToriCode, "00")
                                    CountMinasi += 1
                                Case Else
                                    stMei.FURIKETU_CODE = FKEKKA_TBL.GetKekkaCode(ToriCode, aReadFmt.InfoMeisaiMast.FURIKETU_CENTERCODE)
                            End Select

                            '更新
                            SQL = New StringBuilder(128)
                            SQL.Append("UPDATE MEIMAST")
                            SQL.Append(" SET")
                            SQL.Append("  FURIKETU_CODE_K = " & stMei.FURIKETU_CODE.ToString)
                            SQL.Append(", FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_CENTERCODE.ToString)
                            SQL.Append(", MINASI_K = " & SQ(stMei.MINASI))
                            '訂正情報を格納
                            SQL.Append(", TEISEI_SIT_K     = " & SQ(stMei.TEISEI_SIT))
                            SQL.Append(", TEISEI_KAMOKU_K  = " & SQ(stMei.TEISEI_KAMOKU))
                            SQL.Append(", TEISEI_KOUZA_K   = " & SQ(stMei.TEISEI_KOUZA))
                            SQL.Append(", TEISEI_AKAMOKU_K = " & SQ(stMei.TEISEI_AKAMOKU))
                            SQL.Append(", TEISEI_AKOUZA_K  = " & SQ(stMei.TEISEI_AKOUZA))
                            SQL.Append(", KOUSIN_DATE_K = " & SQ(strTODAY))
                            SQL.Append("WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                            SQL.Append("  AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                            If mKoParam.CP.MODE1 = "0" Then
                                ' 更新キーが企業シーケンス
                                SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                            Else
                                ' 更新キーが口座情報
                                '複数見つかった時は見つかった企業SEQの最小のもののみ更新する
                                SQL.Append("   AND KIGYO_SEQ_K = ")
                                SQL.Append("   (SELECT MIN(KIGYO_SEQ_K) FROM MEIMAST WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                                SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                                SQL.Append("   AND FURIKETU_CODE_K = '0' ")
                                SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                                SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                                SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                                SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                                SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                                '2010/01/19 カナ変換後の需要家番号をYOBI4_Kに設定
                                ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                                ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                                'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                Select Case mKoParam.CP.MODE1
                                    Case "9"
                                        ' MODE1="9"は、需要家番号を条件からはずし更新する
                                    Case Else
                                        SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                                End Select
                                ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                                '================================================
                                SQL.Append("   )")

                            End If

                            Try
                                MainDB.ExecuteNonQuery(SQL)
                                dblKOUSIN_KEN += 1
                            Catch ex As Exception
                                MainLOG.Write("明細更新", "失敗", "振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("明細更新 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                                Return False
                            End Try

                            ' スケジュールキー情報保存
                            Dim Key(0) As String
                            Key(0) = stMei.FURIKAE_DATE

                            Dim oSearch As New mySearchClass
                            If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                                If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                                    MainLOG.Write("振替日不一致", "失敗", "入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                                    MainLOG.UpdateJOBMASTbyErr("振替日不一致 入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                                    Return False
                                End If

                                UpdateKeys.Add(Key)
                            End If
                        Case Else
                            '金庫持込以外のデータ
                            dblKIGYO_RECORD_COUNT += 1
                    End Select

                    '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                    OraMeiReader.Close()
                    '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                End If
NEXT_RECORD:
            Loop

            For i As Integer = 0 To UpdateKeys.Count - 1

                Dim Keys() As String = CType(UpdateKeys(i), String())


                ' スケジュール更新
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

            'エラーメッセージがない場合件数表示
            If mJobMessage = "" Then
                ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- START
                'mJobMessage = "読込件数：" & dblALL_KEN & " 更新件数：" & dblKOUSIN_KEN & " みなし更新件数:" & CountMinasi
                If UnmatchRecordNum > 0 Then
                    mJobMessage = "● 未更新件数：" & UnmatchRecordNum & " 読込件数：" & dblALL_KEN & " 更新件数：" & dblKOUSIN_KEN & " みなし更新件数:" & CountMinasi
                Else
                    mJobMessage = "読込件数：" & dblALL_KEN & " 更新件数：" & dblKOUSIN_KEN & " みなし更新件数:" & CountMinasi
                End If
                ' 2017/01/26 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応(追加カスタマイズ)) -------------------- END
                MainLOG.Write("結果更新", "成功", mJobMessage)
            End If

            '2017/03/13 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '預金口座振替変更通知書が０件で出力される不具合を修正
            'TourokuMainのコミット後に印刷する
            HenkoutuutiCount = UpdateKeys.Count
            'If UpdateKeys.Count > 0 Then
            '    '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------START
            '    Dim HENKOU_PRINT_FLG As String = "0"
            '    HENKOU_PRINT_FLG = CASTCommon.GetFSKJIni("PRINT", "HENKOUTUCHI_PRINT")
            '    If HENKOU_PRINT_FLG = "err" OrElse HENKOU_PRINT_FLG = "" Then
            '        MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:変更通知書印刷フラグ 分類:PRINT 項目:HENKOUTUCHI_PRINT")
            '        HENKOU_PRINT_FLG = "0"          'iniファイルの取得失敗した場合は、印刷しない。
            '    End If
            '    If HENKOU_PRINT_FLG = "1" Then
            '        '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------END

            '        ''------------------------------------------
            '        '' レポエージェント印刷
            '        ''------------------------------------------
            '        Dim ExeRepo As New CAstReports.ClsExecute
            '        ' 預金口座振替解約・変更通知書
            '        Dim Keys() As String = CType(UpdateKeys(0), String())
            '        Dim Param As String = LW.UserID & "," & Keys(0).TrimEnd
            '        MainLOG.Write("預金口座振替変更通知書印刷バッチ呼出", "成功", Param)
            '        Dim nRet As Integer = ExeRepo.ExecReport("KFJP012.EXE", Param)
            '        Select Case nRet
            '            Case 0
            '                MainLOG.Write("預金口座振替変更通知書印刷", "成功", "")
            '            Case -1
            '                MainLOG.Write("預金口座振替変更通知書印刷", "失敗", "印刷対象なし")
            '            Case Else
            '                MainLOG.Write("預金口座振替変更通知書印刷", "失敗", "")
            '                MainLOG.UpdateJOBMASTbyErr("預金口座振替変更通知書印刷 失敗")
            '                Return False
            '        End Select
            '        '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------START
            '    End If
            '    '2011/06/29 標準版修正 預金口座振替変更通知書印刷選択 ------------------END
            'End If
            '2017/03/13 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
        Catch ex As Exception
            MainLOG.Write("明細マスタ登録処理", "失敗", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("システムエラー（ログ参照）")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            If Not aReadFmt Is Nothing Then
                aReadFmt.Close()
                aReadFmt.Dispose()
            End If
        End Try

        Return True
    End Function

#End Region

#Region "SKC用明細更新"
    ' 機能　 ： 明細マスタ登録処理(SKC)
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateMeiMast(ByVal aReadFmt As CAstFormat.CFormatSKCFunou) As Boolean
        Dim dblRECORD_COUNT As Integer = 0
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Integer = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' チェック処理結果

        Dim SQL As StringBuilder = New StringBuilder(512)

        Dim UpdateKeys As New ArrayList(1000)

        Dim strTODAY As String = DateTime.Today.ToString("yyyyMMdd")

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***


        ' 全データチェック
        Try
            ' 起動パラメータ共通情報
            aReadFmt.ToriData = mArgumentData
            Dim InfileInfo As New FileInfo(aReadFmt.FileName)
            If InfileInfo.Exists() = True AndAlso InfileInfo.Length = 0 Then
                ' 実際０バイトでした
                MainLOG.Write("不能結果件数０件処理", "成功")

                Dim Key(0) As String
                If mKoParam.CP.FURI_DATE Is Nothing OrElse mKoParam.CP.FURI_DATE.Trim = "" Then
                    ' 前営業日
                    Dim oFmt As New CAstFormat.CFormat
                    oFmt.Oracle = MainDB
                    Dim ZenDay As Date = CASTCommon.GetEigyobi(CASTCommon.Calendar.Now, -1, oFmt.HolidayList)
                    oFmt = Nothing
                    Key(0) = ZenDay.ToString("yyyyMMdd")
                Else
                    Key(0) = mKoParam.CP.FURI_DATE
                End If
                UpdateKeys.Add(Key)
            Else
                ' 不能結果ファイル読み込み開始
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("ファイルオープン", "失敗", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("ファイルオープン失敗")
                    Return False
                End If
            End If

            Dim stMei As CAstFormat.CFormat.MEISAI      ' 明細マスタ情報

            Do Until aReadFmt.EOF
                ' データを読み込んで，フォーマットチェックを行う
                sCheckRet = aReadFmt.CheckKekkaFormat()

                dblRECORD_COUNT += 1

                If aReadFmt.IsDataRecord = True Then

                    stMei = aReadFmt.InfoMeisaiMast

                    MainLOG.FuriDate = stMei.FURIKAE_DATE

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    '不能コード0以外を更新する
                    If stMei.FURIKETU_CODE <> 0 Then

                        Dim intCOUNT As Integer = 0
                        '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                        OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                        SQL.Length = 0
                        SQL.Append("SELECT COUNT(*)")
                        SQL.Append(", MAX(TORIS_CODE_K || TORIF_CODE_K)")
                        SQL.Append(" FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                        SQL.Append("   AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                        If mKoParam.CP.MODE1 = "0" Then
                            ' 更新キーが企業シーケンス
                            SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                        Else
                            ' 更新キーが口座情報
                            SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                            SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                            SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                            SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                            '振替金額数値化
                            SQL.Append("   AND FURIKIN_K = " & CInt(stMei.FURIKIN))
                            '2010/01/19 カナ変換後の需要家番号をYOBI4_Kに設定
                            ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                            ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                            'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            Select Case mKoParam.CP.MODE1
                                Case "9"
                                    ' MODE1="9"は、需要家番号を条件からはずし更新する
                                Case Else
                                    SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            End Select
                            ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                            '================================================
                        End If
                        If OraMeiReader.DataReader(SQL) = True Then
                            intCOUNT = OraMeiReader.GetValueInt(0)
                        End If

                        If intCOUNT = 0 Then
                            '検索結果が0件の場合、メッセージを表示
                            MainLOG.Write("明細検索", "失敗", "振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            MainLOG.UpdateJOBMASTbyErr("明細検索失敗 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            Return False
                        ElseIf intCOUNT > 1 Then
                            MainLOG.Write("明細検索", "失敗", "同一条件明細複数存在　振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            ' 異常終了にしない
                            'LOG.UpdateJOBMASTbyErr("明細検索同一条件明細複数存在 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            mJobMessage = "明細検索同一条件明細複数存在 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ
                            MainLOG.UpdateJOBMAST(mJobMessage)
                        End If

                        '更新
                        SQL = New StringBuilder(128)
                        SQL.Append("UPDATE MEIMAST")
                        SQL.Append(" SET")
                        SQL.Append("  FURIKETU_CODE_K = " & stMei.FURIKETU_CODE.ToString)
                        SQL.Append(", FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_CENTERCODE.ToString)
                        SQL.Append(", KOUSIN_DATE_K = " & SQ(strTODAY))
                        SQL.Append("WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                        SQL.Append("  AND TRIM(KIGYO_CODE_K) = " & SQ(stMei.KIGYO_CODE))
                        If mKoParam.CP.MODE1 = "0" Then
                            ' 更新キーが企業シーケンス
                            SQL.Append("   AND KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
                        Else
                            ' 更新キーが口座情報
                            '複数見つかった時は見つかった企業SEQの最小のもののみ更新する
                            SQL.Append("   AND KIGYO_SEQ_K = ")
                            SQL.Append("   (SELECT MIN(KIGYO_SEQ_K) FROM MEIMAST WHERE FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                            SQL.Append("   AND KIGYO_CODE_K = " & SQ(stMei.KIGYO_CODE))
                            SQL.Append("   AND FURIKETU_CODE_K = '0' ")
                            SQL.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                            SQL.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                            SQL.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                            SQL.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                            SQL.Append("   AND FURIKIN_K = " & stMei.FURIKIN)
                            '2010/01/19 カナ変換後の需要家番号をYOBI4_Kに設定
                            ' SQL.Append("   AND TRIM(JYUYOUKA_NO_K) = " & SQ(stMei.JYUYOKA_NO.Trim))
                            ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                            'SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            Select Case mKoParam.CP.MODE1
                                Case "9"
                                    ' MODE1="9"は、需要家番号を条件からはずし更新する
                                Case Else
                                    SQL.Append("   AND YOBI4_K = " & SQ(stMei.JYUYOKA_NO))
                            End Select
                            ' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                            '================================================
                            SQL.Append("   )")

                        End If

                        Try
                            MainDB.ExecuteNonQuery(SQL)
                            dblKOUSIN_KEN += 1
                        Catch ex As Exception
                            MainLOG.Write("明細更新", "失敗", "振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                            MainLOG.UpdateJOBMASTbyErr("明細更新 振替日：" & stMei.FURIKAE_DATE & " 企業コード：" & stMei.KIGYO_CODE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            Return False
                        End Try

                        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        OraMeiReader.Close()
                        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                    End If

                    ' スケジュールキー情報保存
                    Dim Key(0) As String
                    Key(0) = stMei.FURIKAE_DATE

                    Dim oSearch As New mySearchClass
                    If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                        If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                            MainLOG.Write("振替日不一致", "失敗", "入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                            MainLOG.UpdateJOBMASTbyErr("振替日不一致 入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                            Return False
                        End If

                        UpdateKeys.Add(Key)
                    End If
                End If
            Loop

            For i As Integer = 0 To UpdateKeys.Count - 1

                Dim Keys() As String = CType(UpdateKeys(i), String())

                ' スケジュール更新
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If

            Next i

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

            'エラーメッセージがない場合件数表示
            If mJobMessage = "" Then
                mJobMessage = "読込件数：" & dblALL_KEN & " 更新件数：" & dblKOUSIN_KEN & " みなし更新件数:" & CountMinasi
                MainLOG.Write("結果更新", "成功", mJobMessage)
            End If

        Catch ex As Exception
            MainLOG.Write("明細マスタ登録処理", "失敗", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("システムエラー（ログ参照）")
            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            If Not aReadFmt Is Nothing Then
                aReadFmt.Close()
                aReadFmt.Dispose()
            End If
        End Try

        Return True
    End Function

#End Region

#Region " 明細マスタ登録処理（ＳＳＳ）"
    ' 機能　 ： 明細マスタ登録処理（ＳＳＳ）
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateMeiMastSSS(ByVal aReadFmt As CAstFormat.CFormatZengin) As Boolean
        Dim dblRECORD_COUNT As Integer
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Double = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' チェック処理結果

        Dim SQL As StringBuilder

        Dim UpdateKeys As New ArrayList(1000)

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***


        ' 全データチェック
        Try
            ' 起動パラメータ共通情報
            aReadFmt.ToriData = mArgumentData
            If aReadFmt.FirstRead() = 0 Then
                MainLOG.Write("ファイルオープン", "失敗", aReadFmt.Message)
                MainLOG.UpdateJOBMASTbyErr("ファイルオープン失敗")
                Return False
            End If

            Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
            Dim stMei As CAstFormat.CFormat.MEISAI      ' 明細マスタ情報
            Do Until aReadFmt.EOF
                ' データを読み込んで，フォーマットチェックを行う
                sCheckRet = aReadFmt.CheckKekkaFormatSSS()
                Select Case sCheckRet
                    Case "ERR"
                        ' フォーマットエラー
                        MainLOG.Write("フォーマットエラー", "失敗", (dblRECORD_COUNT + 1).ToString & "行目 " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr((dblRECORD_COUNT + 1).ToString & "行目 " & aReadFmt.Message)
                        Return False
                        Exit Do
                    Case "IJO"
                        MainLOG.Write("フォーマットエラー", "失敗", aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr(aReadFmt.Message)
                        Return False
                    Case "H"
                        MainLOG.Write("ファイルヘッダデータセット", "成功")
                    Case "D"
                    Case "T"
                        MainLOG.Write("ファイルトレーラデータセット", "成功")
                    Case ""
                        Exit Do
                End Select

                dblRECORD_COUNT += 1

                stMei = aReadFmt.InfoMeisaiMast
                If aReadFmt.IsHeaderRecord = True Then
                    ' ヘッダ

                    If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                        MainLOG.Write("振替日不一致", "失敗", "入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                        MainLOG.UpdateJOBMASTbyErr("振替日不一致 入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                        Return False
                    End If

                    If aReadFmt.GetTorimastFromItakuCodeSSS(MainDB) = False Then
                        MainLOG.Write("取引先マスタ検索", "失敗", "委託者コード:" & stMei.ITAKU_CODE & " " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr("取引先マスタ検索失敗 委託者コード:" & stMei.ITAKU_CODE)
                        Return False
                    End If
                ElseIf aReadFmt.IsDataRecord = True Then
                    stTori = aReadFmt.ToriData.INFOToriMast

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    Dim intCOUNT As Integer = 0
                    '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                    'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                    OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(*)")
                    SQL.Append(", MAX(TORIS_CODE_K)")
                    SQL.Append(", MAX(TORIF_CODE_K)")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                    SQL.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                    SQL.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                    SQL.Append("   AND DATA_KBN_K   =  '2'")
                    If mKoParam.CP.MODE1 = "0" Then
                        ' 更新キーが企業シーケンス
                        SQL.Append(" AND KIGYO_SEQ_K = " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                    Else
                        ' 更新キーが口座情報
                        SQL.Append(" AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQL.Append(" AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- START
                        SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        'SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- END
                        SQL.Append(" AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQL.Append(" AND FURIKIN_K = " & stMei.FURIKIN)
                        ' 2017/11/10 タスク）西野 DEL (標準版不具合対応(№168)) -------------------- START
                        'SSSでも使用していないので条件に入れない
                        '' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                        ''SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'Select Case mKoParam.CP.MODE1
                        '    Case "9"
                        '        ' MODE1="9"は、需要家番号を条件からはずし更新する
                        '    Case Else
                        '        SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'End Select
                        '' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                        ' 2017/11/10 タスク）西野 DEL (標準版不具合対応(№168)) -------------------- END
                    End If

                    If OraMeiReader.DataReader(SQL) = True Then
                        intCOUNT = OraMeiReader.GetValueInt(0)
                    End If

                    If intCOUNT = 0 Then
                        '検索結果が0件の場合、メッセージを表示
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- START
                        'ログ出力見直し
                        If mKoParam.CP.MODE1 = "0" Then

                            MainLOG.Write("明細検索(企業シーケンス)", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                          " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            MainLOG.UpdateJOBMASTbyErr("明細検索失敗(企業シーケンス) 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                                       " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        Else
                            MainLOG.Write("明細検索(口座情報)", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                          " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                          " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN)
                            MainLOG.UpdateJOBMASTbyErr("明細検索(口座情報) 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                          " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                          " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN)
                        End If
                        'MainLOG.Write("明細検索", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        'MainLOG.UpdateJOBMASTbyErr("明細検索失敗 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- END

                        Return False
                    ElseIf intCOUNT > 1 Then
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- START
                        'ログ出力見直し
                        If mKoParam.CP.MODE1 = "0" Then
                            MainLOG.Write("明細検索(企業シーケンス)", "失敗", "同一条件明細複数存在　取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                          " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            MainLOG.UpdateJOBMASTbyErr("明細検索(企業シーケンス) 同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                                       " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        Else
                            MainLOG.Write("明細検索(口座情報)", "失敗", "同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                          " 振替日：" & stMei.FURIKAE_DATE & " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & _
                                          " 科目：" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN)
                            MainLOG.UpdateJOBMASTbyErr("明細検索(口座情報) 同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & _
                                                       " 振替日：" & stMei.FURIKAE_DATE & " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & _
                                                       " 科目：" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN)
                        End If
                        'MainLOG.Write("明細検索", "失敗", "同一条件明細複数存在　取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        'MainLOG.UpdateJOBMASTbyErr("明細検索同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- END

                    End If

                    '更新
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE MEIMAST ")
                    SQL.Append(" SET FURIKETU_CODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",KOUSIN_DATE_K = " & SQ(DateTime.Today.ToString("yyyyMMdd")))
                    SQL.Append(" WHERE FURI_DATE_K  = " & SQ(stMei.FURIKAE_DATE))
                    SQL.Append("   AND TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
                    SQL.Append("   AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
                    If mKoParam.CP.MODE1 = "0" Then
                        ' 更新キーが企業シーケンス
                        SQL.Append(" AND KIGYO_SEQ_K = " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                    Else
                        ' 更新キーが口座情報
                        SQL.Append(" AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQL.Append(" AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- START
                        SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        'SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(stMei.KEIYAKU_KAMOKU))
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- END
                        SQL.Append(" AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQL.Append(" AND FURIKIN_K = " & stMei.FURIKIN)
                        ' 2017/11/10 タスク）西野 DEL (標準版不具合対応(№168)) -------------------- START
                        'SSSでも使用していないので条件に入れない
                        '' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- START
                        ''SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'Select Case mKoParam.CP.MODE1
                        '    Case "9"
                        '        ' MODE1="9"は、需要家番号を条件からはずし更新する
                        '    Case Else
                        '        SQL.Append(" AND SUBSTR(JYUYOUKA_NO_K,1,11) = " & SQ(stMei.JYUYOKA_NO.Substring(9, 11)))
                        'End Select
                        '' 2017/01/26 タスク）綾部 CHG 【OT】UI_B-99(RSV2対応(標準版機能追加)) -------------------- END
                        ' 2017/11/10 タスク）西野 DEL (標準版不具合対応(№168)) -------------------- END
                    End If

                    Try
                        MainDB.ExecuteNonQuery(SQL)
                        dblKOUSIN_KEN += 1
                    Catch ex As Exception
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- START
                        'ログ出力見直し
                        If mKoParam.CP.MODE1 = "0" Then
                            MainLOG.Write("明細更新(企業シーケンス)", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                          " 企業シーケンス：" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                            MainLOG.UpdateJOBMASTbyErr("明細更新(企業シーケンス) 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                                       " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        Else
                            MainLOG.Write("明細更新(口座情報)", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                          " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                          " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN)
                            MainLOG.UpdateJOBMASTbyErr("明細更新(口座情報) 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                                       " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU) & _
                                                       " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN)
                        End If
                        'MainLOG.Write("明細更新", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                        'MainLOG.UpdateJOBMASTbyErr("明細更新 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№168)) -------------------- END
                        Return False
                    End Try

                    OraMeiReader.Close()

                ElseIf aReadFmt.IsTrailerRecord = True Then
                    ' スケジュールキー情報保存
                    stTori = aReadFmt.ToriData.INFOToriMast
                    Dim Key(4) As String
                    Key(0) = stTori.TORIS_CODE_T
                    Key(1) = stTori.TORIF_CODE_T
                    Key(2) = stMei.FURIKAE_DATE
                    Key(3) = Jikinko
                    Key(4) = stMei.ITAKU_CODE.PadRight(10, " "c).Substring(9, 1)

                    Dim oSearch As New mySearchClass
                    If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                        UpdateKeys.Add(Key)
                    End If

                End If
NEXT_RECORD:
            Loop

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' 他行スケジュール更新
                If UpdateTAKOSCHMAST(Keys) = False Then
                    Return False
                End If
            Next i

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' スケジュール更新
                If UpdateSCHMAST(Keys, False) = False Then
                    Return False
                End If
            Next i

            '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            '処理結果確認表はスケジュール更新で印刷せず、個別に印刷する。
            '（不能結果更新先数分印刷されるため）
            If Me.PrintSyoriKekkaKakuninhyoForSSS(UpdateKeys) = False Then
                Return False
            End If
            '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

            ' 手数料計算
            '取引先でなく振替日キーで手数料計算する
            '2017/01/18 saitou 東春信金(RSV2標準) DEL スリーエス対応 ---------------------------------------- START
            'この手数料計算は使えない
            'Dim ht As Hashtable = New Hashtable
            'For i As Integer = 0 To UpdateKeys.Count - 1
            '    Dim Keys() As String = CType(UpdateKeys(i), String())

            '    '取引先でなく振替日キーで手数料計算する
            '    If ht.ContainsKey(Keys(2)) = False Then
            '        Call CalcTesuuExecute(Keys(2))
            '        ht.Add(Keys(2), Nothing)
            '    End If
            'Next i
            '2017/01/18 saitou 東春信金(RSV2標準) DEL ------------------------------------------------------- END

        Catch ex As Exception
            MainLOG.Write("明細マスタ登録処理(SSS)", "失敗", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("システムエラー（ログ参照）")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        End Try

        Return True
    End Function
#End Region

#Region " 明細マスタ登録処理（企業持込）"
    ' 機能　 ： 明細マスタ登録処理（企業持込）
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateMeiMastKigyo(ByVal aReadFmt As CAstFormat.CFormatZengin) As Boolean
        Dim dblRECORD_COUNT As Integer
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Double = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' チェック処理結果

        Dim SQL As StringBuilder

        Dim UpdateKeys As New ArrayList(1000)
        Dim RecordNo As String = "0"
        Dim RecordHash As New Hashtable

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***


        ' 全データチェック
        Try

            ' 起動パラメータ共通情報
            aReadFmt.ToriData = mArgumentData
            If aReadFmt.FirstRead() = 0 Then
                MainLOG.Write("ファイルオープン", "失敗", aReadFmt.Message)
                MainLOG.UpdateJOBMASTbyErr("ファイルオープン失敗")
                Return False
            End If
            Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
            Dim stMei As CAstFormat.CFormat.MEISAI      ' 明細マスタ情報
            Do Until aReadFmt.EOF
                ' データを読み込んで，フォーマットチェックを行う
                sCheckRet = aReadFmt.CheckKekkaFormat()
                Select Case sCheckRet
                    Case "ERR"
                        ' フォーマットエラー
                        MainLOG.Write("フォーマットエラー", "失敗", (dblRECORD_COUNT + 1).ToString & "行目 " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr((dblRECORD_COUNT + 1).ToString & "行目 " & aReadFmt.Message)
                        Return False
                        Exit Do
                    Case "IJO"
                        MainLOG.Write("フォーマットエラー", "失敗", aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr(aReadFmt.Message)
                        Return False
                    Case "H"
                        MainLOG.Write("ファイルヘッダデータセット", "成功")
                    Case "D"
                    Case "T"
                        MainLOG.Write("ファイルトレーラデータセット", "成功")
                    Case ""
                        Exit Do
                End Select

                dblRECORD_COUNT += 1

                Dim SQLWhere As New StringBuilder(128)

                stMei = aReadFmt.InfoMeisaiMast
                If aReadFmt.IsHeaderRecord = True Then
                    ' ヘッダ
                    If aReadFmt.GetTorimastFromItakuCodeKigyo(MainDB) = False Then
                        MainLOG.Write("取引先マスタ検索", "失敗", "委託者コード:" & stMei.ITAKU_CODE & " " & aReadFmt.Message)
                        MainLOG.UpdateJOBMASTbyErr("取引先マスタ検索失敗 委託者コード:" & stMei.ITAKU_CODE)
                        Return False
                    End If

                    ' スケジュールキー情報保存
                    stTori = aReadFmt.ToriData.INFOToriMast
                    Dim Key(2) As String
                    Key(0) = stTori.TORIS_CODE_T
                    Key(1) = stTori.TORIF_CODE_T
                    Key(2) = stMei.FURIKAE_DATE

                    Dim oSearch As New mySearchClass
                    If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                        If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                            MainLOG.Write("振替日不一致", "失敗", "入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                            MainLOG.UpdateJOBMASTbyErr("振替日不一致 入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                            Return False
                        End If

                        UpdateKeys.Add(Key)
                    End If

                ElseIf aReadFmt.IsDataRecord = True Then
                    stTori = aReadFmt.ToriData.INFOToriMast

                    dblALL_KEN += 1
                    dblALL_KINGAKU += stMei.FURIKIN

                    Dim intCOUNT As Integer = 0
                    '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                    'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                    OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(*)")
                    SQL.Append(", MAX(TORIS_CODE_K)")
                    SQL.Append(", MAX(TORIF_CODE_K)")
                    SQL.Append(", MIN(RECORD_NO_K) RECORD_NO")  '2009/12/18 追加
                    SQL.Append(" FROM MEIMAST")

                    SQLWhere = New StringBuilder(128)
                    SQLWhere.Append(" WHERE TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
                    If stTori.BAITAI_CODE_T = "07" Then
                        ' 学校自振の場合，副コードをキーに含まない
                    Else
                        SQLWhere.Append("   AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
                    End If
                    SQLWhere.Append("   AND FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                    SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                    SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                    SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                    SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                    SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                    '需要家番号はFURI_DATAと比較する
                    SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                    SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                    Dim sSQL As String = SQL.ToString & SQLWhere.ToString
                    If OraMeiReader.DataReader(sSQL) = True Then
                        intCOUNT = CASTCommon.CAInt32(OraMeiReader.GetValue(0).ToString)
                    End If

                    If RecordHash.Contains(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T) Then
                        RecordNo = RecordHash.Item(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T).ToString
                    Else
                        RecordNo = "0"
                    End If

                    If intCOUNT > 1 Then
                        OraMeiReader.Close()
                        ' 複数件数有り，別のキーでアクセス
                        SQLWhere = New StringBuilder(128)
                        SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                        SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                        SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                        SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                        SQLWhere.Append("   AND RECORD_NO_K  =  (SELECT MIN(RECORD_NO_K) ")
                        SQLWhere.Append(" FROM MEIMAST")
                        SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                        SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                        SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                        SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                        '需要家番号はFURI_DATAと比較する
                        SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                        SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                        SQLWhere.Append("   AND RECORD_NO_K NOT IN(" & RecordNo & ")")
                        SQLWhere.Append("   )")
                        sSQL = SQL.ToString & SQLWhere.ToString
                        If OraMeiReader.DataReader(sSQL) = True Then
                            intCOUNT = OraMeiReader.GetValueInt(0)
                        End If
                    End If

                    If intCOUNT = 0 Then
                        '検索結果が0件の場合、メッセージを表示
                        MainLOG.Write("明細検索", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                      " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                      " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                        MainLOG.UpdateJOBMASTbyErr("明細検索 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                      " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                      " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                        Return False
                    ElseIf intCOUNT > 1 Then
                        MainLOG.Write("明細検索", "失敗", "同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                                                                        " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                                                                        " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                        MainLOG.UpdateJOBMASTbyErr("明細検索同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                      " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                      " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                        Return False
                    End If

                    '更新
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE MEIMAST ")
                    SQL.Append(" SET FURIKETU_CODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_MOTO)
                    SQL.Append(",KOUSIN_DATE_K = " & SQ(DateTime.Today.ToString("yyyyMMdd")))
                    RecordNo &= "," & OraMeiReader.GetInt("RECORD_NO")
                    If RecordHash.Contains(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T) Then
                        RecordHash.Item(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T) = RecordNo
                    Else
                        RecordHash.Add(stTori.TORIS_CODE_T & stTori.TORIF_CODE_T, RecordNo)
                    End If
                    Try
                        MainDB.ExecuteNonQuery(SQL.Append(SQLWhere))
                        dblKOUSIN_KEN += 1
                    Catch ex As Exception
                        MainLOG.Write("明細更新", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                                                  " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                                                  " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO & " " & ex.Message & ":" & ex.StackTrace)
                        MainLOG.UpdateJOBMASTbyErr("明細更新 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                      " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                      " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                        Return False
                    End Try

                    OraMeiReader.Close()
                End If
NEXT_RECORD:
            Loop


            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' スケジュール更新
                If UpdateSCHMAST(Keys, True) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

        Catch ex As Exception
            MainLOG.Write("明細マスタ登録処理", "失敗", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("システムエラー（ログ参照）")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        End Try

        Return True
    End Function

#End Region

#Region " 明細マスタ登録処理（他行）"
    ' 機能　 ： 明細マスタ登録処理（他行）
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateMeiMastTako(ByVal aReadFmt As CAstFormat.CFormatZengin) As Boolean
        Dim dblRECORD_COUNT As Integer
        Dim dblALL_KEN As Decimal
        Dim dblALL_KINGAKU As Decimal
        Dim dblKOUSIN_KEN As Decimal

        Dim dblKIGYO_RECORD_COUNT As Double = 0
        Dim intMINASI_COUNT As Integer = 0

        Dim sCheckRet As String                         ' チェック処理結果

        Dim SQL As StringBuilder

        Dim UpdateKeys As New ArrayList(1000)
        Dim RecordNo As String = "0"

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***


        ' 全データチェック
        Try
            '強制更新の場合または媒体が依頼書の場合はスケジュール更新のみ
            If mKoParam.KOUSIN_KBN = "1" OrElse strTAKO_BAITAI_CODE = "04" Then
                Dim Key(3) As String
                '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                'Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                OraReader = New CASTCommon.MyOracleReader(MainDB)
                '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                Key(0) = mKoParam.CP.TORIS_CODE
                Key(1) = mKoParam.CP.TORIF_CODE
                Key(2) = mKoParam.CP.FURI_DATE
                Key(3) = mKoParam.TKIN_CODE

                'キー項目がマスタ登録されている場合、マスタ更新を行う
                SQL = New StringBuilder(128)
                SQL.Append(" SELECT COUNT(*) COUNTER")
                SQL.Append(" FROM   TAKOSCHMAST ")
                SQL.Append(" WHERE  TORIS_CODE_U = " & SQ(Key(0)))
                SQL.Append(" AND    TORIF_CODE_U = " & SQ(Key(1)))
                SQL.Append(" AND    FURI_DATE_U  = " & SQ(Key(2)))
                SQL.Append(" AND    TKIN_NO_U    = " & SQ(Key(3)))

                If OraReader.DataReader(SQL) = True Then
                    If OraReader.GetInt("COUNTER") = 0 Then
                        MainLOG.Write("他行スケジュール更新", "失敗", " 他行スケジュールマスタ無し： 取引先 " & Key(0) & "-" & Key(1) & _
                                                                             " 振替日 " & ConvertDate(Key(2), "yyyy年MM月dd日") & " 金融機関" & Key(3))

                        MainLOG.UpdateJOBMASTbyErr("他行スケジュール更新失敗： 取引先 " & Key(0) & "-" & Key(1) & _
                                                   " 振替日 " & ConvertDate(Key(2), "yyyy年MM月dd日") & " 金融機関" & Key(3))
                        Return False
                    End If

                Else
                    If mKoParam.KOUSIN_KBN = "1" Then
                        MainLOG.Write("他行スケジュール更新更新", "失敗", " 他行スケジュールマスタ無し： 取引先 " & Key(0) & "-" & Key(1) & _
                                      " 振替日 " & ConvertDate(Key(2), "yyyy年MM月dd日") & " 金融機関" & Key(3))
                        MainLOG.UpdateJOBMASTbyErr("強制更新失敗： 取引先 " & Key(0) & "-" & Key(1) & _
                                                   " 振替日 " & ConvertDate(Key(2), "yyyy年MM月dd日") & " 金融機関" & Key(3))
                    Else
                        MainLOG.Write("他行スケジュール更新更新", "失敗", " 他行スケジュールマスタ無し： 取引先 " & Key(0) & "-" & Key(1) & _
                                                      " 振替日 " & ConvertDate(Key(2), "yyyy年MM月dd日") & " 金融機関" & Key(3))
                        MainLOG.UpdateJOBMASTbyErr("依頼書更新失敗： 取引先 " & Key(0) & "-" & Key(1) & _
                                                   " 振替日 " & ConvertDate(Key(2), "yyyy年MM月dd日") & " 金融機関" & Key(3))
                    End If
                    Return False
                End If

                UpdateKeys.Add(Key)

                '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                OraReader.Close()
                '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            Else
                ' 起動パラメータ共通情報
                aReadFmt.ToriData = mArgumentData
                If aReadFmt.FirstRead() = 0 Then
                    MainLOG.Write("ファイルオープン", "失敗", aReadFmt.Message)
                    MainLOG.UpdateJOBMASTbyErr("ファイルオープン失敗")
                    Return False
                End If
                Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
                Dim stMei As CAstFormat.CFormat.MEISAI      ' 明細マスタ情報
                Do Until aReadFmt.EOF
                    ' データを読み込んで，フォーマットチェックを行う
                    sCheckRet = aReadFmt.CheckKekkaFormat()
                    Select Case sCheckRet
                        Case "ERR"
                            ' フォーマットエラー
                            MainLOG.Write("フォーマットエラー", "失敗", (dblRECORD_COUNT + 1).ToString & "行目 " & aReadFmt.Message)
                            MainLOG.UpdateJOBMASTbyErr((dblRECORD_COUNT + 1).ToString & "行目 " & aReadFmt.Message)
                            Return False
                            Exit Do
                        Case "IJO"
                            MainLOG.Write("フォーマットエラー", "失敗", aReadFmt.Message)
                            MainLOG.UpdateJOBMASTbyErr(aReadFmt.Message)
                            Return False
                        Case "H"
                            MainLOG.Write("ファイルヘッダデータセット", "成功")
                        Case "D"
                        Case "T"
                            MainLOG.Write("ファイルトレーラデータセット", "成功")
                        Case ""
                            Exit Do
                    End Select

                    dblRECORD_COUNT += 1

                    Dim SQLWhere As New StringBuilder(128)

                    stMei = aReadFmt.InfoMeisaiMast
                    If aReadFmt.IsHeaderRecord = True Then
                        ' ヘッダ
                        If aReadFmt.GetTorimastFromItakuCodeTAKO(MainDB) = False Then
                            MainLOG.Write("他行マスタ検索", "失敗", "委託者コード:" & stMei.ITAKU_CODE & " " & aReadFmt.Message)
                            MainLOG.UpdateJOBMASTbyErr("他行マスタ検索失敗 委託者コード:" & stMei.ITAKU_CODE)
                            Return False
                        End If

                        ' スケジュールキー情報保存
                        stTori = aReadFmt.ToriData.INFOToriMast
                        If mKoParam.CP.FURI_DATE <> stMei.FURIKAE_DATE Then
                            MainLOG.Write("振替日不一致", "失敗", "入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                            MainLOG.UpdateJOBMASTbyErr("振替日不一致 入力振替日：" & ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " データ振替日：" & ConvertDate(stMei.FURIKAE_DATE, "yyyy年MM月dd日"))
                            Return False
                        End If

                    ElseIf aReadFmt.IsDataRecord = True Then
                        stTori = aReadFmt.ToriData.INFOToriMast

                        dblALL_KEN += 1
                        dblALL_KINGAKU += stMei.FURIKIN

                        Dim intCOUNT As Integer = 0
                        '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
                        OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT COUNT(*)")
                        SQL.Append(", MAX(TORIS_CODE_K)")
                        SQL.Append(", MAX(TORIF_CODE_K)")
                        SQL.Append(", MIN(RECORD_NO_K) RECORD_NO")  '2009/12/18 追加
                        SQL.Append(" FROM MEIMAST")

                        SQLWhere = New StringBuilder(128)
                        SQLWhere.Append(" WHERE TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
                        If stTori.BAITAI_CODE_T = "07" Then
                            ' 学校自振の場合，副コードをキーに含まない
                        Else
                            SQLWhere.Append("   AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
                        End If
                        SQLWhere.Append("   AND FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
                        SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                        SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                        SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                        SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                        SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                        '需要家番号はFURI_DATAと比較する
                        SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                        SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                        If TAKO_SEQ <> "0" Then
                            SQLWhere.Append("   AND KIGYO_SEQ_K  =  " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                        End If
                        Dim sSQL As String = SQL.ToString & SQLWhere.ToString
                        If OraMeiReader.DataReader(sSQL) = True Then
                            intCOUNT = CASTCommon.CAInt32(OraMeiReader.GetValue(0).ToString)
                        End If


                        If intCOUNT > 1 Then
                            OraMeiReader.Close()
                            ' 複数件数有り，別のキーでアクセス
                            SQLWhere = New StringBuilder(128)
                            SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                            SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                            SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                            SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                            If TAKO_SEQ <> "0" Then
                                SQLWhere.Append("   AND KIGYO_SEQ_K  =  " & SQ(CADec(aReadFmt.ZENGIN_REC2.ZG15).ToString))
                            Else
                                SQLWhere.Append("   AND RECORD_NO_K  =  (SELECT MIN(RECORD_NO_K) ")
                                SQLWhere.Append(" FROM MEIMAST")
                                SQLWhere.Append(" WHERE TORIS_CODE_K =  " & SQ(stTori.TORIS_CODE_T))
                                SQLWhere.Append("   AND TORIF_CODE_K =  " & SQ(stTori.TORIF_CODE_T))
                                SQLWhere.Append("   AND FURI_DATE_K  =  " & SQ(stMei.FURIKAE_DATE))
                                SQLWhere.Append("   AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
                                SQLWhere.Append("   AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
                                SQLWhere.Append("   AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
                                SQLWhere.Append("   AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
                                SQLWhere.Append("   AND FURIKIN_K = " & stMei.FURIKIN_MOTO)
                                '需要家番号はFURI_DATAと比較する
                                SQLWhere.Append("   AND SUBSTR(FURI_DATA_K,92,20) = " & SQ(stMei.JYUYOKA_NO))
                                SQLWhere.Append("   AND DATA_KBN_K   =  '2'")
                                SQLWhere.Append("   AND RECORD_NO_K NOT IN(" & RecordNo & ")")
                                SQLWhere.Append("   )")
                            End If
                            sSQL = SQL.ToString & SQLWhere.ToString
                            If OraMeiReader.DataReader(sSQL) = True Then
                                intCOUNT = OraMeiReader.GetValueInt(0)
                            End If
                        End If

                        If intCOUNT = 0 Then
                            '検索結果が0件の場合、メッセージを表示
                            If TAKO_SEQ = "0" Then
                                MainLOG.Write("明細検索", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                                                          " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                                                          " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                                MainLOG.UpdateJOBMASTbyErr("明細検索 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                              " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                              " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                            Else
                                MainLOG.Write("明細検索", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & aReadFmt.ZENGIN_REC2.ZG15)
                                MainLOG.UpdateJOBMASTbyErr("明細検索失敗 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & aReadFmt.ZENGIN_REC2.ZG15)
                            End If
                            Return False
                        ElseIf intCOUNT > 1 Then
                            If TAKO_SEQ = "0" Then
                                MainLOG.Write("明細検索", "失敗", "同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                                                     " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                                                     " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                                MainLOG.UpdateJOBMASTbyErr("明細検索同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                              " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                              " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                                Return False
                            Else
                                MainLOG.Write("明細検索", "失敗", "同一条件明細複数存在　取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & aReadFmt.ZENGIN_REC2.ZG15)
                                MainLOG.UpdateJOBMASTbyErr("明細検索同一条件明細複数存在 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & aReadFmt.ZENGIN_REC2.ZG15)
                            End If
                        End If

                        '更新
                        SQL = New StringBuilder(128)
                        SQL.Append("UPDATE MEIMAST ")
                        SQL.Append(" SET FURIKETU_CODE_K = " & stMei.FURIKETU_MOTO)
                        SQL.Append(",FURIKETU_CENTERCODE_K = " & stMei.FURIKETU_MOTO)
                        SQL.Append(",KOUSIN_DATE_K = " & SQ(DateTime.Today.ToString("yyyyMMdd")))
                        If TAKO_SEQ = "0" Then
                            RecordNo &= "," & OraMeiReader.GetInt("RECORD_NO")
                        End If

                        Try
                            MainDB.ExecuteNonQuery(SQL.Append(SQLWhere))
                            dblKOUSIN_KEN += 1
                        Catch ex As Exception
                            If TAKO_SEQ = "0" Then
                                MainLOG.Write("明細更新", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                              " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                              " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("明細更新 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & _
                                              " 金融機関：" & stMei.KEIYAKU_KIN & " 支店：" & stMei.KEIYAKU_SIT & " 科目：" & stMei.KEIYAKU_KAMOKU & _
                                              " 口座番号：" & stMei.KEIYAKU_KOUZA & " 振替金額：" & stMei.FURIKIN_MOTO)
                            Else
                                MainLOG.Write("明細更新", "失敗", "取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("明細更新 取引先コード：" & stTori.TORIS_CODE_T & stTori.TORIF_CODE_T & " 振替日：" & stMei.FURIKAE_DATE & " 企業シーケンス：" & stMei.KIGYO_SEQ)
                            End If
                            Return False
                        End Try

                        ' スケジュールキー情報保存
                        stTori = aReadFmt.ToriData.INFOToriMast
                        Dim Key(3) As String
                        Key(0) = stTori.TORIS_CODE_T
                        Key(1) = stTori.TORIF_CODE_T
                        Key(2) = stMei.FURIKAE_DATE
                        Key(3) = stMei.KEIYAKU_KIN

                        Dim oSearch As New mySearchClass
                        If UpdateKeys.BinarySearch(Key, oSearch) < 0 Then
                            UpdateKeys.Add(Key)
                        End If

                        OraMeiReader.Close()
                    End If
NEXT_RECORD:
                Loop
            End If


            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                ' 他行スケジュール更新
                If UpdateTAKOSCHMAST(Keys) = False Then
                    Return False
                End If
            Next i

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                '複数の農協でまとめた場合、明細の金融機関の数だけ更新を行うため
                '2回目以降を不要とする
                If mKoParam.TKIN_CODE = MatomeNoukyo AndAlso i > 0 Then
                    Exit For
                End If
                ' スケジュール更新
                If UpdateSCHMAST(Keys, False) = False Then
                    Return False
                End If
            Next i

            '*** Str Del 2015/12/01 SO)荒木 for 多重実行対応 ***
            'MainDB.Commit()
            '*** End Del 2015/12/01 SO)荒木 for 多重実行対応 ***

        Catch ex As Exception
            MainLOG.Write("明細マスタ登録処理", "失敗", ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("システムエラー（ログ参照）")

            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        End Try

        Return True
    End Function

#End Region

#Region " スケジュールマスタ更新"
    ' 機能　 ： スケジュールマスタ更新
    '
    ' 引数   ： ARG1 - 更新キー
    '           ARG2 - モード TRUE-自行結果更新，FALSE-他行結果更新
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateSCHMAST(ByVal Keys() As String, ByVal mode As Boolean) As Boolean
        '*** Str Del 2015/12/01 SO)荒木 for 不要MyOracleReader削除 ***
        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
        '*** End Del 2015/12/01 SO)荒木 for 不要MyOracleReader削除 ***
        Dim SQL As New StringBuilder(128)
        Dim nUpdate As Integer

        Dim KeyToriSCode As String = ""
        Dim KeyToriFCode As String = ""
        Dim KeyFuriDate As String = ""

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim OraCntReader As CASTCommon.MyOracleReader = Nothing

        Try
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            If Keys.Length = 1 Then
                KeyFuriDate = Keys(0)
            Else
                KeyToriSCode = Keys(0)
                KeyToriFCode = Keys(1)
                KeyFuriDate = Keys(2)

                MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
                MainLOG.FuriDate = KeyFuriDate
            End If

            If mode = False Then
                ' 他行結果更新
                ' スケジュールマスタ更新
                ' 他行未処理なし（伝送)
                'Try
                '    SQL = New StringBuilder(128)
                '    SQL.Append("UPDATE SCHMAST")
                '    SQL.Append(" SET")
                '    'SQL.Append(" FUNOU_T1DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '    SQL.Append(" FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '    SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '    'SQL.Append("   AND HAISIN_T1FLG_S= '1'")
                '    SQL.Append("   AND HAISIN_FLG_S= '1'")
                '    SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '    SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '    If Keys.Length > 1 Then
                '        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '    End If
                '    SQL.Append("   AND NOT EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' 不能処理が未完了
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U = '00'")
                '    SQL.Append(")")
                '    SQL.Append("   AND EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' 不能処理が完了
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U = '00'")
                '    SQL.Append(")")

                '    nUpdate = MainDB.ExecuteNonQuery(SQL)

                '    MainLOG.Write("スケジュール更新（他行未処理あり（伝送)）", "成功", "件数：" & nUpdate.ToString)
                'Catch ex As Exception
                '    MainLOG.FuriDate = KeyFuriDate
                '    MainLOG.Write("スケジュール更新（他行未処理あり（伝送)）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '    MainLOG.UpdateJOBMASTbyErr("スケジュール更新（他行未処理あり（伝送)） 振替日：" & KeyFuriDate)
                '    Return False
                'End Try

                '' 他行未処理なし（伝送以外)
                'Try
                '    SQL = New StringBuilder(128)
                '    SQL.Append("UPDATE SCHMAST")
                '    SQL.Append(" SET")
                '    SQL.Append(" FUNOU_T2DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '    SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '    'SQL.Append("   AND HAISIN_T2FLG_S= '1'")
                '    SQL.Append("   AND HAISIN_FLG_S= '1'")
                '    SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '    SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '    If Keys.Length > 1 Then
                '        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '    End If
                '    SQL.Append("   AND NOT EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' 不能処理が未完了
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U <> '00'")
                '    SQL.Append(")")
                '    SQL.Append("   AND EXISTS")
                '    SQL.Append("(SELECT TORIS_CODE_U ")
                '    SQL.Append("  FROM TAKOSCHMAST")
                '    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '    SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' 不能処理が完了
                '    SQL.Append("   AND SYORI_KEN_U  <> 0")
                '    SQL.Append("   AND SYORI_KIN_U  <> 0")
                '    'SQL.Append("   AND TEIKEI_KBN_U = '0'")
                '    SQL.Append("   AND BAITAI_CODE_U <> '00'")
                '    SQL.Append(")")

                '    nUpdate = MainDB.ExecuteNonQuery(SQL)

                '    MainLOG.Write("スケジュール更新（他行未処理あり（伝送以外)）", "成功", "件数：" & nUpdate.ToString)
                'Catch ex As Exception
                '    MainLOG.FuriDate = KeyFuriDate
                '    MainLOG.Write("スケジュール更新（他行未処理あり（伝送以外)）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '    MainLOG.UpdateJOBMASTbyErr("スケジュール更新（他行未処理あり（伝送以外)） 振替日：" & KeyFuriDate)
                '    Return False
                'End Try

                '================================
                'SSSのためコメント化
                '================================
                '    ' ＳＳＳ未処理なし（提携内)
                '    Try
                '        SQL = New StringBuilder(128)
                '        SQL.Append("UPDATE SCHMAST")
                '        SQL.Append(" SET")
                '        SQL.Append(" FUNOU_T1DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '        SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '        SQL.Append("   AND HAISIN_T1FLG_S= '1'")
                '        SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '        SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '        If Keys.Length > 1 Then
                '            SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '            SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '        End If
                '        SQL.Append("   AND NOT EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' 不能処理が未完了
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '1'")
                '        SQL.Append(")")
                '        SQL.Append("   AND EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' 不能処理が完了
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '1'")
                '        SQL.Append(")")

                '        nUpdate = MainDB.ExecuteNonQuery(SQL)

                '        MainLOG.Write("スケジュール更新（ＳＳＳ未処理なし（提携内)）", "成功", "件数：" & nUpdate.ToString)
                '    Catch ex As Exception
                '        MainLOG.FuriDate = KeyFuriDate
                '        MainLOG.Write("スケジュール更新（ＳＳＳ未処理なし（提携内)）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '        MainLOG.UpdateJOBMASTbyErr("スケジュール更新（ＳＳＳ未処理なし（提携内)） 振替日：" & KeyFuriDate)
                '        Return False
                '    End Try

                '    ' ＳＳＳ未処理なし（提携外)
                '    Try
                '        SQL = New StringBuilder(128)
                '        SQL.Append("UPDATE SCHMAST")
                '        SQL.Append(" SET")
                '        SQL.Append(" FUNOU_T2DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                '        SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                '        SQL.Append("   AND HAISIN_T2FLG_S= '1'")
                '        SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '        SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                '        If Keys.Length > 1 Then
                '            SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                '            SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                '        End If
                '        SQL.Append("   AND NOT EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' 不能処理が未完了
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '2'")
                '        SQL.Append(")")
                '        SQL.Append("   AND EXISTS")
                '        SQL.Append("(SELECT TORIS_CODE_U ")
                '        SQL.Append("  FROM TAKOSCHMAST")
                '        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                '        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                '        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                '        SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' 不能処理が完了
                '        SQL.Append("   AND SYORI_KEN_U  <> 0")
                '        SQL.Append("   AND SYORI_KIN_U  <> 0")
                '        SQL.Append("   AND TEIKEI_KBN_U = '2'")
                '        SQL.Append(")")

                '        nUpdate = MainDB.ExecuteNonQuery(SQL)

                '        MainLOG.Write("スケジュール更新（ＳＳＳ未処理なし（提携外)）", "成功", "件数：" & nUpdate.ToString)
                '    Catch ex As Exception
                '        MainLOG.FuriDate = KeyFuriDate
                '        MainLOG.Write("スケジュール更新（ＳＳＳ未処理なし（提携外)）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                '        MainLOG.UpdateJOBMASTbyErr("スケジュール更新（ＳＳＳ未処理なし（提携外)） 振替日：" & KeyFuriDate)
                '        Return False
                '    End Try
            End If

            '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            'スリーエスで依頼が全て自行の場合、他行データ作成を行っても配信データに含まれないため、
            '他行スケジュールマスタの処理件数と金額を見て他行スケジュールマスタの不能フラグを更新する。
            Try
                SQL = New StringBuilder(128)
                SQL.Append("UPDATE TAKOSCHMAST")
                SQL.Append(" SET")
                SQL.Append(" FUNOU_FLG_U = '1'")
                SQL.Append(" WHERE FURI_DATE_U = " & SQ(KeyFuriDate))
                SQL.Append(" AND SYORI_KEN_U = 0")
                SQL.Append(" AND SYORI_KIN_U = 0")
                SQL.Append(" AND TKIN_NO_U = " & SQ(Jikinko))
                SQL.Append(" AND EXISTS(")
                SQL.Append(" SELECT * FROM SCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append(" AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append(" AND FURI_DATE_U = FURI_DATE_S")
                SQL.Append(" AND TAKOU_FLG_S = '1'")
                SQL.Append(" )")

                nUpdate = MainDB.ExecuteNonQuery(SQL)
                MainLOG.Write("他行スケジュール更新（ＳＳＳ他行依頼無し結果更新）", "成功", "件数：" & nUpdate.ToString)

            Catch ex As Exception
                MainLOG.FuriDate = KeyFuriDate
                MainLOG.Write("他行スケジュール更新（ＳＳＳ他行依頼無し結果更新）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                MainLOG.UpdateJOBMASTbyErr("他行スケジュール更新（ＳＳＳ他行依頼無し結果更新） 振替日：" & KeyFuriDate)
                Return False
            End Try
            '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

            ' 全て完了の場合
            Try
                SQL = New StringBuilder(128)
                SQL.Append("UPDATE SCHMAST")
                SQL.Append(" SET")
                SQL.Append(" FUNOU_FLG_S = '1'")
                SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                SQL.Append("   AND HAISIN_FLG_S= '1'")
                SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                '東海以外はセンター直接持込は更新しない
                If CENTER <> "4" Then
                    SQL.Append("   AND MOTIKOMI_KBN_S = '0'")
                End If
                If mode = False Then
                    ' 他行結果更新の場合，不能フラグ＝２の時のみ 
                    SQL.Append("   AND FUNOU_FLG_S = '2'")
                Else
                    SQL.Append("   AND FUNOU_FLG_S IN ('0', '2')")
                End If
                '信組対応 組合持込の場合は送信区分も条件に入れる
                If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "0" Then
                    SQL.Append("   AND SOUSIN_KBN_S = '0'")
                End If
                If Keys.Length > 1 Then
                    If mArgumentData.INFOToriMast.SOUSIN_KBN_T = "1" AndAlso mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                        '企業持込(マルチ)
                        '2010/12/24 クエリ修正 ここから
                        'SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(" AND EXISTS(")
                        SQL.Append(" SELECT * FROM TORIMAST")
                        SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(")")
                        '2010/12/24 クエリ修正 ここまで
                        SQL.Append(" AND SOUSIN_KBN_S = '1'")
                    Else
                        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                    End If
                End If
                SQL.Append("   AND ((NOT EXISTS")
                SQL.Append("(SELECT TORIS_CODE_U ")
                SQL.Append("  FROM TAKOSCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                SQL.Append("   AND SYORI_KEN_U  <> 0")
                SQL.Append("   AND SYORI_KIN_U  <> 0")
                SQL.Append(") AND TAKOU_FLG_S = '0')")
                SQL.Append(" OR ")
                SQL.Append("   (EXISTS")
                SQL.Append("(SELECT TORIS_CODE_U ")
                SQL.Append("  FROM TAKOSCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                SQL.Append("   AND SYORI_KEN_U  <> 0")
                SQL.Append("   AND SYORI_KIN_U  <> 0")
                '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                '元請との区別を図るため、金融機関コードが自行以外の条件を追加
                SQL.Append("   AND TKIN_NO_U <> " & SQ(Jikinko))
                '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
                '2017/01/20 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                'スリーエスの条件追加
                'SQL.Append(") AND TAKOU_FLG_S = '1'))")
                SQL.Append(") AND TAKOU_FLG_S = '1')")
                SQL.Append(" OR ")
                SQL.Append("   (EXISTS")
                SQL.Append("(SELECT TORIS_CODE_U ")
                SQL.Append("  FROM TAKOSCHMAST")
                SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                'SQL.Append("   AND SYORI_KEN_U  <> 0")
                'SQL.Append("   AND SYORI_KIN_U  <> 0")
                SQL.Append("   AND TKIN_NO_U = " & SQ(Jikinko))
                SQL.Append("   AND FUNOU_FLG_U = '1'")
                SQL.Append(") AND TAKOU_FLG_S = '1'))")
                '2017/01/20 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                nUpdate = MainDB.ExecuteNonQuery(SQL)
                MainLOG.Write("スケジュール更新（結果更新）", "成功", "件数：" & nUpdate.ToString)
            Catch ex As Exception
                MainLOG.FuriDate = KeyFuriDate
                MainLOG.Write("スケジュール更新（結果更新）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                MainLOG.UpdateJOBMASTbyErr("スケジュール更新（自行のみ） 振替日：" & KeyFuriDate)
                Return False
            End Try

            ' 結果返却不要の場合
            Try
                SQL = New StringBuilder(128)
                SQL.Append("UPDATE SCHMAST")
                SQL.Append(" SET")
                SQL.Append(" HENKAN_FLG_S = '1'")
                SQL.Append(",HENKAN_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                SQL.Append("   AND HAISIN_FLG_S= '1'")
                SQL.Append("   AND FUNOU_FLG_S = '1'")
                SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                SQL.Append("   AND HENKAN_FLG_S = '0'")
                '東海以外はセンター直接持込は更新しない
                If CENTER <> "4" Then
                    SQL.Append("   AND MOTIKOMI_KBN_S = '0'")
                End If
                '信組対応 組合持込の場合は送信区分も条件に入れる
                If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "0" Then
                    SQL.Append("   AND SOUSIN_KBN_S = '0'")
                End If
                If Keys.Length > 1 Then
                    If mArgumentData.INFOToriMast.SOUSIN_KBN_T = "1" AndAlso mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                        '企業持込(マルチ)
                        '2010/12/24 クエリ修正 ここから
                        'SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(" AND EXISTS(")
                        SQL.Append(" SELECT * FROM TORIMAST")
                        SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(")")
                        '2010/12/24 クエリ修正 ここまで
                        SQL.Append(" AND SOUSIN_KBN_S = '1'")
                    Else
                        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                    End If
                End If
                SQL.Append("   AND EXISTS")
                SQL.Append("(SELECT TORIS_CODE_T ")
                SQL.Append("  FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_S")
                SQL.Append("   AND TORIF_CODE_T = TORIF_CODE_S")
                SQL.Append("   AND KEKKA_HENKYAKU_KBN_T = '0'")         ' 結果返却不要
                SQL.Append(")")

                nUpdate = MainDB.ExecuteNonQuery(SQL)
                MainLOG.Write("スケジュール更新（自行のみ）", "成功", "件数：" & nUpdate.ToString)

            Catch ex As Exception
                MainLOG.FuriDate = KeyFuriDate
                MainLOG.Write("スケジュール更新（自行のみ）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                MainLOG.UpdateJOBMASTbyErr("スケジュール更新（自行のみ） 振替日：" & KeyFuriDate)
                Return False
            End Try

            If Keys.Length = 1 Then
                ' 他行，ＳＳＳ未処理あり
                Try
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE SCHMAST")
                    SQL.Append(" SET")
                    SQL.Append(" FUNOU_FLG_S = '2'")
                    SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    SQL.Append(" WHERE FURI_DATE_S = " & SQ(KeyFuriDate))
                    SQL.Append("   AND HAISIN_FLG_S= '1'")
                    SQL.Append("   AND TYUUDAN_FLG_S = '0'")
                    SQL.Append("   AND FUNOU_FLG_S   IN ( '0', '2' )")
                    SQL.Append("   AND EXISTS")
                    SQL.Append("(SELECT TORIS_CODE_U ")
                    SQL.Append("  FROM TAKOSCHMAST")
                    SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
                    SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
                    SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
                    SQL.Append("   AND FUNOU_FLG_U  = '0'")
                    SQL.Append("   AND SYORI_KEN_U  <> 0")
                    SQL.Append("   AND SYORI_KIN_U  <> 0")
                    SQL.Append(")")

                    nUpdate = MainDB.ExecuteNonQuery(SQL)
                    MainLOG.Write("スケジュール更新（他行未処理あり）", "成功", "件数：" & nUpdate.ToString)
                Catch ex As Exception
                    MainLOG.FuriDate = KeyFuriDate
                    MainLOG.Write("スケジュール更新（他行未処理あり）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                    MainLOG.UpdateJOBMASTbyErr("スケジュール更新（他行未処理あり） 振替日：" & KeyFuriDate)
                    Return False
                End Try
            End If

            ' 合計
            '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            'Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
            'Dim OraCntReader As New CASTCommon.MyOracleReader(MainDB)
            OraSchReader = New CASTCommon.MyOracleReader(MainDB)
            OraCntReader = New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            '処理結果確認表(不能結果更新)
            Dim CreateCSV As New KFJP013
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")
            Dim strCSV_FILE_NAME As String = CreateCSV.CreateCsvFile()
            '------------------------------------
            '更新対象のスケジュール検索
            '------------------------------------
            SQL = New StringBuilder(128)
            SQL.Append("SELECT ")
            SQL.Append(" TORIS_CODE_S")
            SQL.Append(",TORIF_CODE_S")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(",KIGYO_CODE_S")
            SQL.Append(",FURI_CODE_S")
            SQL.Append(",FMT_KBN_T")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",ITAKU_CODE_T")
            SQL.Append(",SYORI_KEN_S")
            SQL.Append(",SYORI_KIN_S")
            SQL.Append(",FUNOU_FLG_S")
            SQL.Append(",FUNOU_KEN_S")
            SQL.Append(",FUNOU_KIN_S")
            SQL.Append(",MOTIKOMI_KBN_S")
            SQL.Append(" FROM SCHMAST ")
            SQL.Append("     ,TORIMAST ")
            SQL.Append(" WHERE FURI_DATE_S   = " & SQ(KeyFuriDate))
            SQL.Append("   AND FUNOU_FLG_S IN ('1','2')")
            SQL.Append("   AND TYUUDAN_FLG_S = '0'")
            SQL.Append("   AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append("   AND TORIF_CODE_T = TORIF_CODE_S")
            '信組対応 組合持込の場合は送信区分も条件に入れる
            If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "0" Then
                SQL.Append("   AND SOUSIN_KBN_S = '0'")
            End If
            Select Case mKoParam.MOTIKOMI_KBN
                Case "0" '金庫持込
                    '東海以外はセンター直接持込は更新しない
                    If CENTER <> "4" Then
                        SQL.Append("   AND MOTIKOMI_KBN_S = '0'")
                    End If
                Case "1" '企業持込
                    If mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                        '企業持込(マルチ)
                        '2010/12/24 クエリ修正 ここから
                        'SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(" AND EXISTS(")
                        SQL.Append(" SELECT * FROM TORIMAST")
                        SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                        SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                        SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        SQL.Append(")")
                        '2010/12/24 クエリ修正 ここまで
                        SQL.Append(" AND SOUSIN_KBN_S = '1'")
                    Else
                        SQL.Append(" AND TORIS_CODE_S = " & SQ(KeyToriSCode))
                        SQL.Append(" AND TORIF_CODE_S = " & SQ(KeyToriFCode))
                    End If
                Case "2" '他行振分
                    SQL.Append("   AND TORIS_CODE_T = " & SQ(KeyToriSCode))
                    SQL.Append("   AND TORIF_CODE_T = " & SQ(KeyToriFCode))
                    '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                Case "4"
                    SQL.Append("   AND TORIS_CODE_T = " & SQ(KeyToriSCode))
                    SQL.Append("   AND TORIF_CODE_T = " & SQ(KeyToriFCode))
                    '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
            End Select
            SQL.Append(" ORDER BY TORIS_CODE_S,TORIF_CODE_S")

            Dim intCOUNT As Integer
            If OraSchReader.DataReader(SQL) = True Then
                intCOUNT = 0
                While OraSchReader.EOF = False
                    intCOUNT += 1

                    '----------------------
                    '不能件数、金額取得
                    '----------------------
                    Dim dblFUNOU_KEN As Decimal = 0
                    Dim dblFUNOU_KIN As Decimal = 0
                    Dim dblFURI_KEN As Decimal = 0
                    Dim dblFURI_KIN As Decimal = 0

                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
                    If OraSchReader.GetString("FMT_KBN_T") = "02" Then
                        ' 国税の場合
                        SQL.Append("   AND DATA_KBN_K      = '3'")
                    Else
                        SQL.Append("   AND DATA_KBN_K      = '2'")
                    End If
                    SQL.Append("   AND FURIKETU_CODE_K <> 0")
                    '振替金額が０円のものは含まない
                    SQL.Append("  AND FURIKIN_K > 0")
                    SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))

                    If OraCntReader.DataReader(SQL) = True Then
                        dblFUNOU_KEN = OraCntReader.GetInt64("CNT1")
                        dblFUNOU_KIN = OraCntReader.GetInt64("CNT2")
                    End If
                    OraCntReader.Close()

                    '----------------------
                    '振替済件数、金額取得
                    '----------------------
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
                    If OraSchReader.GetString("FMT_KBN_T") = "02" Then
                        ' 国税の場合
                        SQL.Append("   AND DATA_KBN_K      = '3'")
                    Else
                        SQL.Append("   AND DATA_KBN_K      = '2'")
                    End If
                    SQL.Append("   AND FURIKETU_CODE_K =  0")
                    '' 2008.03.14 振替金額が０円のものは含まない
                    SQL.Append("  AND FURIKIN_K > 0 ")
                    SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    If OraCntReader.DataReader(SQL) = True Then
                        dblFURI_KEN = OraCntReader.GetInt64("CNT1")
                        dblFURI_KIN = OraCntReader.GetInt64("CNT2")
                    End If
                    OraCntReader.Close()

                    '-------------------------------------------
                    'スケジュールマスタの更新
                    '-------------------------------------------
                    SQL = New StringBuilder(128)
                    SQL.Append("UPDATE SCHMAST SET")
                    SQL.Append(" FURI_KEN_S = " & dblFURI_KEN.ToString)
                    SQL.Append(",FURI_KIN_S = " & dblFURI_KIN.ToString)
                    SQL.Append(",FUNOU_KEN_S = " & dblFUNOU_KEN.ToString)
                    SQL.Append(",FUNOU_KIN_S =" & dblFUNOU_KIN.ToString)
                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("  AND TORIF_CODE_S = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    SQL.Append("  AND FURI_DATE_S  = " & SQ(KeyFuriDate))

                    Try
                        MainDB.ExecuteNonQuery(SQL)
                    Catch ex As Exception
                        MainLOG.Write("スケジュール更新（振替済み）", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
                        MainLOG.UpdateJOBMASTbyErr("スケジュール更新（振替済み） 振替日：" & KeyFuriDate)
                        Return False
                    End Try

                    '帳票印刷用
                    CreateCSV.OutputCsvData(Today, True)                                                'システム日付
                    CreateCSV.OutputCsvData(NowTime, True)                                              'タイムスタンプ
                    CreateCSV.OutputCsvData(KeyFuriDate, True)                                          '振替日
                    CreateCSV.OutputCsvData(OraSchReader.GetString("TORIS_CODE_S"), True)               '取引先主コード
                    CreateCSV.OutputCsvData(OraSchReader.GetString("TORIF_CODE_S"), True)               '取引先副コード
                    CreateCSV.OutputCsvData(OraSchReader.GetString("ITAKU_NNAME_T"), True)              '取引先名
                    CreateCSV.OutputCsvData(OraSchReader.GetString("ITAKU_CODE_T"), True)               '委託者コード
                    CreateCSV.OutputCsvData(OraSchReader.GetString("SYORI_KEN_S"), True)                '依頼件数
                    CreateCSV.OutputCsvData(OraSchReader.GetString("SYORI_KIN_S"), True)                '依頼金額
                    CreateCSV.OutputCsvData(dblFUNOU_KEN.ToString, True)                                '不能件数
                    CreateCSV.OutputCsvData(dblFUNOU_KIN.ToString, True)                                '不能金額
                    Select Case OraSchReader.GetString("FUNOU_FLG_S")                                   '備考
                        Case "1"
                            CreateCSV.OutputCsvData("不能結果更新完了", True)
                        Case "2"
                            CreateCSV.OutputCsvData("未処理あり", True)
                    End Select
                    CreateCSV.OutputCsvData(OraSchReader.GetString("FURI_CODE_S"), True)                '振替コード
                    CreateCSV.OutputCsvData(OraSchReader.GetString("KIGYO_CODE_S"), True, True)         '企業コード

                    OraSchReader.NextRead()
                End While
            End If
            CreateCSV.CloseCsv()

            ' 学校更新
            If UpdateG_SCHMAST(Keys) = False Then
                Return False
            End If

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String = ""
            Dim errMessage As String
            Dim nret As Integer

            If intCOUNT = 0 Then
                errMessage = "スケジュール更新件数が0件です。"
                MainLOG.JobMessage = "スケジュール更新件数が0件のため処理を中断します。"
                If mKoParam.MOTIKOMI_KBN = "2" Then
                    errMessage &= "自行分の不能結果更新未完了"
                    MainLOG.JobMessage &= "自行分の更新を先に行ってください。"
                End If
                MainLOG.UpdateJOBMASTbyErr(MainLOG.JobMessage)
                MainLOG.Write("スケジュール更新", "失敗", errMessage)
                Return False
            End If

            '信組対応 企業持込の場合は帳票印刷しない
            If CENTER = "0" AndAlso mKoParam.MOTIKOMI_KBN = "1" Then
                Return True
            End If

            ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- START
            ' 処理結果確認表印刷要否が "0" の場合は、帳票印刷しない
            If INI_RSV2_SYORIKEKKA_FUNOU = "0" Then
                Return True
            End If
            ' 2016/03/09 タスク）綾部 ADD 【OT】UI_B-14-99(RSV2対応(追加カスタマイズ)) -------------------- END

            '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            'スリーエスの場合はここで印刷しない
            If mKoParam.MOTIKOMI_KBN = "4" Then
                Return True
            End If
            '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

            'パラメータ設定：ログイン名、ＣＳＶファイル名
            param = MainLOG.UserID & "," & strCSV_FILE_NAME

            nret = ExeRepo.ExecReport("KFJP013.EXE", param)

            If nret <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case nret
                    Case -1
                        errMessage = "処理結果確認表(不能結果更新)の印刷対象が0件です。"
                    Case Else
                        errMessage = "処理結果確認表(不能結果更新)の印刷に失敗しました。"
                        MainLOG.JobMessage = "処理結果確認表(不能結果更新)印刷失敗"
                        MainLOG.UpdateJOBMASTbyErr(MainLOG.JobMessage)
                End Select
                MainLOG.Write("処理結果確認表(不能結果更新)印刷", "失敗", errMessage)
                Return False
            Else
                MainLOG.Write("処理結果確認表(不能結果更新)印刷", "成功")
                Return True
            End If

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Finally
            If Not OraSchReader Is Nothing Then
                OraSchReader.Close()
            End If

            If Not OraCntReader Is Nothing Then
                OraCntReader.Close()
            End If
        End Try
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

    End Function

    ''' <summary>
    ''' スケジュールマスタ更新（ＳＳＳ）
    ''' </summary>
    ''' <param name="Keys">更新キー</param>
    ''' <param name="mode">モード(True:自行結果更新 False:他行結果更新)</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function UpdateSCHMASTSSS(ByVal Keys() As String, _
                                      ByVal mode As Boolean) As Boolean
        Dim SQL As New StringBuilder
        Dim nUpdate As Integer

        Dim KeyToriSCode As String = ""
        Dim KeyToriFCode As String = ""
        Dim KeyFuriDate As String = ""

        If Keys.Length = 1 Then
            KeyFuriDate = Keys(0)
        Else
            KeyToriSCode = Keys(0)
            KeyToriFCode = Keys(1)
            KeyFuriDate = Keys(2)

            MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
            MainLOG.FuriDate = KeyFuriDate
        End If

        '-------------------------------------------
        'スケジュールマスタの更新
        '-------------------------------------------
        'スケジュールマスタの件数と金額が更新されないため、ここで明細を集計して、正しい値に更新する。
        'なお、スリーエスの結果更新では手数料計算は既に自行分で計算済みなので、次の手数料計算の処理で計算されないことを忘れてはならない。
        Dim oraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        With SQL
            .Length = 0
            If Keys.Length = 1 Then
                .Append("select TORIS_CODE_S, TORIF_CODE_S ")
                .Append(" from SCHMAST, TORIMAST")
                .Append(" where FURI_DATE_S = " & SQ(KeyFuriDate))
                .Append(" and HAISIN_FLG_S = '1'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FMT_KBN_T in ('20', '21')")
                .Append(" and TAKOU_FLG_S = '1'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            Else
                .Append("select TORIS_CODE_S, TORIF_CODE_S ")
                .Append(" from SCHMAST, TORIMAST")
                .Append(" where FURI_DATE_S = " & SQ(KeyFuriDate))
                .Append(" and TORIS_CODE_S = " & SQ(KeyToriSCode))
                .Append(" and TORIF_CODE_S = " & SQ(KeyToriFCode))
                .Append(" and HAISIN_FLG_S = '1'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FMT_KBN_T in ('20', '21')")
                .Append(" and TAKOU_FLG_S = '1'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            End If
        End With

        Try
            oraSchReader = New CASTCommon.MyOracleReader(MainDB)
            If oraSchReader.DataReader(SQL) = True Then
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)
                While oraSchReader.EOF = False
                    With SQL
                        .Length = 0
                        .Append("select ")
                        .Append(" count(FURIKIN_K) as SYORIKEN")
                        .Append(",sum(FURIKIN_K) as SYORIKIN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, 1, 0)) as FURIKEN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, FURIKIN_K, 0)) as FURIKIN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, 1)) as FUNOUKEN")
                        .Append(",sum(decode(FURIKETU_CODE_K, 0, 0, FURIKIN_K)) as FUNOUKIN")
                        .Append(" from MEIMAST")
                        .Append(" where TORIS_CODE_K = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
                        .Append(" and TORIF_CODE_K = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
                        .Append(" and FURI_DATE_K = " & SQ(KeyFuriDate))
                        .Append(" and DATA_KBN_K = '2'")
                    End With

                    If oraMeiReader.DataReader(SQL) = True Then
                        '件数と金額が取得できたらスケジュールマスタ更新
                        With SQL
                            .Length = 0
                            .Append("update SCHMAST set ")
                            .Append(" FURI_KEN_S = " & oraMeiReader.GetInt("FURIKEN"))
                            .Append(",FURI_KIN_S = " & oraMeiReader.GetInt64("FURIKIN"))
                            .Append(",FUNOU_KEN_S = " & oraMeiReader.GetInt("FUNOUKEN"))
                            .Append(",FUNOU_KIN_S = " & oraMeiReader.GetInt64("FUNOUKIN"))
                            .Append(",TAKOU_FLG_S = '2'")
                            .Append(" where TORIS_CODE_S = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
                            .Append(" and TORIF_CODE_S = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
                            .Append(" and FURI_DATE_S = " & SQ(KeyFuriDate))
                        End With

                        nUpdate = MainDB.ExecuteNonQuery(SQL)
                        If nUpdate < 0 Then
                            '異常終了
                            MainLOG.Write("スケジュール更新(SSS結果更新)", "失敗", MainDB.Message)
                            MainLOG.UpdateJOBMASTbyErr("スケジュール更新(SSS結果更新)失敗 取引先：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " 振替日：" & KeyFuriDate)
                            Return False
                        End If

                        MainLOG.Write("スケジュール更新(SSS結果更新)", "成功", "取引先：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " 振替日：" & KeyFuriDate)
                    Else
                        '異常終了
                        MainLOG.Write("スケジュール更新(SSS結果更新)", "失敗", "明細マスタから件数と金額の集計に失敗")
                        MainLOG.UpdateJOBMASTbyErr("スケジュール更新(SSS結果更新)失敗 取引先：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIF_CODE_S") & " 振替日：" & KeyFuriDate)
                        Return False
                    End If

                    oraMeiReader.Close()

                    oraSchReader.NextRead()
                End While
            Else
                'スケジュール無し
                MainLOG.Write("スケジュール更新(SSS結果更新)", "失敗", "対象スケジュール無し 振替日：" & KeyFuriDate)
                MainLOG.UpdateJOBMASTbyErr("スケジュール更新失敗(SSS結果更新) 対象スケジュール無し")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("スケジュール更新(SSS結果更新)", "失敗", "振替日：" & KeyFuriDate & " " & ex.Message & ":" & ex.StackTrace)
            MainLOG.UpdateJOBMASTbyErr("スケジュール更新(SSS) 例外発生 振替日：" & KeyFuriDate)
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

        Return True

    End Function

#End Region

#Region " 他行スケジュールマスタ更新"
    ' 機能　 ： 他行スケジュールマスタ更新
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateTAKOSCHMAST(ByVal Keys() As String) As Boolean
        '*** Str Del 2015/12/01 SO)荒木 for 不要MyOracleReader削除 ***
        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
        '*** End Del 2015/12/01 SO)荒木 for 不要MyOracleReader削除 ***
        Dim SQL As New StringBuilder(128)

        Dim KeyToriSCode As String = ""
        Dim KeyToriFCode As String = ""
        Dim KeyFuriDate As String = ""
        Dim KeyKeiyakuKin As String = ""
        Dim KeyTeikeiKubun As String = "0"

        KeyToriSCode = Keys(0)
        KeyToriFCode = Keys(1)
        KeyFuriDate = Keys(2)
        KeyKeiyakuKin = Keys(3)

        MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
        MainLOG.FuriDate = KeyFuriDate

        If Keys.Length = 5 Then
            KeyTeikeiKubun = Keys(4)
        End If

        ' 他行マスタレコードロック
        Dim OraLockReader As New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)
        SQL.Append("SELECT TORIS_CODE_S, TORIF_CODE_S")
        SQL.Append(" FROM SCHMAST")
        SQL.Append(" WHERE TORIS_CODE_S = " & SQ(KeyToriSCode))
        SQL.Append("   AND TORIF_CODE_S = " & SQ(KeyToriFCode))
        SQL.Append("   AND FURI_DATE_S  = " & SQ(KeyFuriDate))
        SQL.Append(" FOR UPDATE")
        OraLockReader.DataReader(SQL)
        OraLockReader.Close()

        '----------------------
        '不能件数、金額取得
        '----------------------
        Dim nFUNOU_KEN As Decimal = 0
        Dim nFUNOU_KIN As Decimal = 0
        Dim nFURI_KEN As Decimal = 0
        Dim nFURI_KIN As Decimal = 0

        Dim OraCntReader As New CASTCommon.MyOracleReader(MainDB)
        SQL = New StringBuilder(128)
        '2017/01/20 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
        'コメントアウトされていた処理を復活させる。
        If KeyKeiyakuKin = Jikinko Then
            ' ＳＳＳ処理
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K    = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K    = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            '自行分は対象外とする
            SQL.Append("   AND KEIYAKU_KIN_K <> " & SQ(Jikinko))
            SQL.Append("   AND FURIKETU_CODE_K <> 0")
            '提携内、提携外の条件は変更する
            '標準版(スリーエス決済無し)は提携内のみ対象
            SQL.Append("   AND EXISTS (")
            SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            SQL.Append("      AND TEIKEI_KBN_N = '1'")
            SQL.Append("   )")
            'If KeyTeikeiKubun = "1" Then
            '    ' 提携内
            '    SQL.Append("   AND EXISTS (")
            'Else
            '    ' 提携外
            '    SQL.Append("   AND NOT EXISTS (")
            'End If
            'SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            'SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            'SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            'SQL.Append("      AND EDA_N = '01'")
            'SQL.Append("      AND TEIKEI_KBN_N = '1'")
            'SQL.Append("   )")
        Else
            ' 他行処理
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K    = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K    = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            SQL.Append("   AND FURIKETU_CODE_K <> 0")
            SQL.Append("   AND KEIYAKU_KIN_K   = " & SQ(KeyKeiyakuKin))
        End If
        '2017/01/20 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
        If OraCntReader.DataReader(SQL) = True Then
            nFUNOU_KEN = OraCntReader.GetInt64("CNT1")
            nFUNOU_KIN = OraCntReader.GetInt64("CNT2")
        End If
        OraCntReader.Close()

        '----------------------
        '振替済件数、金額取得
        '----------------------
        SQL = New StringBuilder(128)
        '2017/01/20 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
        'コメントアウトされていた処理を復活させる。
        If KeyKeiyakuKin = Jikinko Then
            ' ＳＳＳ処理
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K    = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K    = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            '自行分は対象外とする
            SQL.Append("   AND KEIYAKU_KIN_K <> " & SQ(Jikinko))
            SQL.Append("   AND FURIKETU_CODE_K = 0")
            '提携内、提携外の条件は変更する
            '標準版(スリーエス決済無し)は提携内のみ対象
            SQL.Append("   AND EXISTS (")
            SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            SQL.Append("      AND TEIKEI_KBN_N = '1'")
            SQL.Append("   )")
            'If KeyTeikeiKubun = "1" Then
            '    ' 提携内
            '    SQL.Append("   AND EXISTS (")
            'Else
            '    ' 提携外
            '    SQL.Append("   AND NOT EXISTS (")
            'End If
            'SQL.Append("   SELECT TEIKEI_KBN_N FROM TENMAST ")
            'SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
            'SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
            'SQL.Append("      AND EDA_N = '01'")
            'SQL.Append("      AND TEIKEI_KBN_N = '1'")
            'SQL.Append("   )")
        Else
            ' 他行処理
            SQL.Append("SELECT COUNT(FURIKIN_K) CNT1,SUM(FURIKIN_K) CNT2")
            SQL.Append(" FROM MEIMAST")
            SQL.Append(" WHERE FURI_DATE_K     = " & SQ(KeyFuriDate))
            SQL.Append("   AND TORIS_CODE_K = " & SQ(KeyToriSCode))
            SQL.Append("   AND TORIF_CODE_K = " & SQ(KeyToriFCode))
            SQL.Append("   AND DATA_KBN_K      = '2'")
            SQL.Append("   AND FURIKETU_CODE_K =  0")
            SQL.Append("   AND KEIYAKU_KIN_K   = " & SQ(KeyKeiyakuKin))
        End If
        '2017/01/20 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
        If OraCntReader.DataReader(SQL) = True Then
            nFURI_KEN = OraCntReader.GetInt64("CNT1")
            nFURI_KIN = OraCntReader.GetInt64("CNT2")
        End If
        OraCntReader.Close()

        SQL = New StringBuilder(128)
        SQL.Append("UPDATE TAKOSCHMAST")
        SQL.Append(" SET")
        SQL.Append(" FUNOU_FLG_U = '1'")
        SQL.Append(",FURI_KEN_U  = " & nFURI_KEN.ToString)
        SQL.Append(",FURI_KIN_U  = " & nFURI_KIN.ToString)
        SQL.Append(",FUNOU_KEN_U = " & nFUNOU_KEN.ToString)
        SQL.Append(",FUNOU_KIN_U = " & nFUNOU_KIN.ToString)
        SQL.Append(" WHERE TORIS_CODE_U = " & SQ(KeyToriSCode))
        SQL.Append("   AND TORIF_CODE_U = " & SQ(KeyToriFCode))
        SQL.Append("   AND FURI_DATE_U  = " & SQ(KeyFuriDate))
        SQL.Append("   AND TKIN_NO_U    = " & SQ(KeyKeiyakuKin))
        'SQL.Append("   AND TEIKEI_KBN_U = " & SQ(KeyTeikeiKubun))

        Try
            Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("他行スケジュール更新", "成功", "金融機関コード：" & KeyKeiyakuKin & " 更新件数:" & nCount.ToString)
        Catch ex As Exception
            MainLOG.Write("他行スケジュール更新", "失敗", "金融機関コード：" & KeyKeiyakuKin)
            MainLOG.UpdateJOBMASTbyErr("他行スケジュール更新失敗 金融機関コード：" & KeyKeiyakuKin)
            Return False
        End Try

        '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
        'スリーエスの場合、標準版(スリーエス決済無し)は他行フラグの更新不要
        If KeyKeiyakuKin = Jikinko Then
            Return True
        End If
        '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

        ' 他行フラグ更新
        SQL = New StringBuilder(128)
        SQL.Append("UPDATE SCHMAST")
        SQL.Append(" SET")
        SQL.Append(" TAKOU_FLG_S = '1'")
        SQL.Append(" WHERE TORIS_CODE_S = " & SQ(KeyToriSCode))
        SQL.Append("   AND TORIF_CODE_S = " & SQ(KeyToriFCode))
        SQL.Append("   AND FURI_DATE_S  = " & SQ(KeyFuriDate))
        SQL.Append("   AND NOT EXISTS")
        SQL.Append("(SELECT TORIS_CODE_U ")
        SQL.Append("  FROM TAKOSCHMAST")
        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
        SQL.Append("   AND FUNOU_FLG_U  = '0'")             ' 不能処理が未完了
        SQL.Append("   AND SYORI_KEN_U  <> 0")
        SQL.Append("   AND SYORI_KIN_U  <> 0")
        SQL.Append(")")
        SQL.Append("   AND EXISTS")
        SQL.Append("(SELECT TORIS_CODE_U ")
        SQL.Append("  FROM TAKOSCHMAST")
        SQL.Append(" WHERE TORIS_CODE_U = TORIS_CODE_S")
        SQL.Append("   AND TORIF_CODE_U = TORIF_CODE_S")
        SQL.Append("   AND FURI_DATE_U  = FURI_DATE_S")
        SQL.Append("   AND FUNOU_FLG_U  = '1'")             ' 不能処理が未完了
        SQL.Append("   AND SYORI_KEN_U  <> 0")
        SQL.Append("   AND SYORI_KIN_U  <> 0")
        SQL.Append(")")

        Try
            Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
            MainLOG.Write("スケジュール 他行フラグ更新", "成功", "更新件数:" & nCount.ToString)
        Catch ex As Exception
            MainLOG.Write("スケジュール 他行フラグ更新", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("他行スケジュール更新失敗")
            Return False
        End Try

        Return True
    End Function

#End Region

#Region " 学校スケジュールマスタ更新"
    ' 機能　 ： 学校スケジュールマスタ更新
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function UpdateG_SCHMAST(ByVal keys() As String) As Boolean
        Dim SQL As New StringBuilder(128)

        '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        'Dim OraMeiReader As New CASTCommon.MyOracleReader(MainDB)
        'Dim OraSchReader As New CASTCommon.MyOracleReader(MainDB)
        'Dim OraCount As New CASTCommon.MyOracleReader(MainDB)
        Dim OraMeiReader As CASTCommon.MyOracleReader = Nothing
        Dim OraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim OraCount As CASTCommon.MyOracleReader = Nothing
        '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Try
            OraMeiReader = New CASTCommon.MyOracleReader(MainDB)
            OraSchReader = New CASTCommon.MyOracleReader(MainDB)
            OraCount = New CASTCommon.MyOracleReader(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            Dim KeyToriSCode As String = ""
            Dim KeyToriFCode As String = ""
            Dim KeyFuriDate As String = ""

            If keys.Length = 1 Then
                KeyFuriDate = keys(0)
            Else
                KeyToriSCode = keys(0)
                KeyToriFCode = keys(1)
                KeyFuriDate = keys(2)

                MainLOG.ToriCode = KeyToriSCode & KeyToriFCode
                MainLOG.FuriDate = KeyFuriDate
            End If

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append(" TORIS_CODE_S")
            SQL.Append(",TORIF_CODE_S")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE FURI_DATE_S  = " & SQ(KeyFuriDate))
            SQL.Append("   AND HAISIN_FLG_S= '1'")
            SQL.Append("   AND TYUUDAN_FLG_S= '0'")
            SQL.Append("   AND BAITAI_CODE_S= '07' ")
            If OraSchReader.DataReader(SQL) = True Then
                Do While OraSchReader.EOF = False

                    ' 学校明細マスタ更新
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT ")
                    SQL.Append(" TORIS_CODE_K")
                    SQL.Append(",TORIF_CODE_K")
                    SQL.Append(",FURI_DATE_K")
                    SQL.Append(",JYUYOUKA_NO_K")
                    SQL.Append(",KEIYAKU_KAMOKU_K")
                    SQL.Append(",KEIYAKU_KIN_K")
                    SQL.Append(",KEIYAKU_SIT_K")
                    SQL.Append(",KEIYAKU_KOUZA_K")
                    SQL.Append(",FURIKETU_CODE_K")
                    SQL.Append(" FROM MEIMAST")
                    SQL.Append(" WHERE FURIKETU_CODE_K <> 0 ")
                    SQL.Append("   AND DATA_KBN_K   = '2'")
                    SQL.Append("   AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    SQL.Append("   AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    SQL.Append("   AND FURI_DATE_K  = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                    If OraMeiReader.DataReader(SQL) = True Then

                        Do While OraMeiReader.EOF = False

                            '----------------------------
                            '学校自振明細マスタから検索
                            '----------------------------
                            Dim strJYUYOUKA_NO As String, strKAMOKU As String

                            If OraMeiReader.GetString("JYUYOUKA_NO_K").TrimEnd.Length >= 20 Then
                                strJYUYOUKA_NO = OraMeiReader.GetString("JYUYOUKA_NO_K").Substring(0, 20)
                            Else
                                strJYUYOUKA_NO = OraMeiReader.GetString("JYUYOUKA_NO_K")
                            End If
                            strKAMOKU = OraMeiReader.GetString("KEIYAKU_KAMOKU_K")

                            SQL = New StringBuilder(128)
                            SQL.Append("UPDATE G_MEIMAST")
                            SQL.Append(" SET FURIKETU_CODE_M = " & OraMeiReader.GetInt("FURIKETU_CODE_K").ToString)
                            SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(OraMeiReader.GetString("TORIS_CODE_K")))
                            SQL.Append("   AND FURI_DATE_M   = " & SQ(OraMeiReader.GetString("FURI_DATE_K")))
                            SQL.Append("   AND TKIN_NO_M     = " & SQ(OraMeiReader.GetString("KEIYAKU_KIN_K")))
                            SQL.Append("   AND TSIT_NO_M     = " & SQ(OraMeiReader.GetString("KEIYAKU_SIT_K")))
                            SQL.Append("   AND TKAMOKU_M     = " & SQ(strKAMOKU))
                            SQL.Append("   AND TKOUZA_M      = " & SQ(OraMeiReader.GetString("KEIYAKU_KOUZA_K")))
                            SQL.Append("   AND JUYOUKA_NO_M  = " & SQ(strJYUYOUKA_NO))

                            Try
                                Dim nCount As Integer
                                nCount = MainDB.ExecuteNonQuery(SQL)
                                If nCount = 0 Then
                                    MainLOG.Write("学校明細検索", "失敗", "学校コード：" & OraMeiReader.GetString("TORIS_CODE_K") _
                                                & " 振替日：" & OraMeiReader.GetString("FURI_DATE_K") _
                                                & " 需要家番号：" & strJYUYOUKA_NO _
                                                & " 金融機関：" & OraMeiReader.GetString("KEIYAKU_KIN_K") _
                                                & " 支店：" & OraMeiReader.GetString("KEIYAKU_SIT_K") _
                                                & " 科目：" & strKAMOKU _
                                                & " 口座番号：" & OraMeiReader.GetString("KEIYAKU_KOUZA_K"))

                                    MainLOG.UpdateJOBMASTbyErr("学校明細検索 学校コード：" & OraMeiReader.GetString("TORIS_CODE_K") _
                                        & " 振替日：" & OraMeiReader.GetString("FURI_DATE_K") _
                                        & " 需要家番号：" & strJYUYOUKA_NO)
                                    Return False
                                End If

                            Catch ex As Exception
                                MainLOG.Write("学校明細更新", "失敗", "学校コード：" & OraMeiReader.GetString("TORIS_CODE_K") _
                                            & " 振替日：" & OraMeiReader.GetString("FURI_DATE_K") _
                                            & " 需要家番号：" & strJYUYOUKA_NO _
                                            & " 金融機関：" & OraMeiReader.GetString("KEIYAKU_KIN_K") _
                                            & " 支店：" & OraMeiReader.GetString("KEIYAKU_SIT_K") _
                                            & " 科目：" & strKAMOKU _
                                            & " 口座番号：" & OraMeiReader.GetString("KEIYAKU_KOUZA_K") _
                                            & " " & ex.Message & ":" & ex.StackTrace)
                                MainLOG.UpdateJOBMASTbyErr("学校明細更新 学校コード：" & OraMeiReader.GetString("TORIS_CODE_K") _
                                        & " 振替日：" & OraMeiReader.GetString("FURI_DATE_K") _
                                        & " 需要家番号：" & strJYUYOUKA_NO)
                                Return False
                            End Try

                            OraMeiReader.NextRead()
                        Loop
                    Else
                        MainLOG.Write("学校明細更新 明細不能明細なし", "成功", "取引先コード：" _
                                    & OraSchReader.GetString("TORIS_CODE_S") _
                                    & "-" & OraSchReader.GetString("TORIF_CODE_S") _
                                    & " 振替日：" & OraSchReader.GetString("FURI_DATE_S"))

                    End If
                    OraMeiReader.Close()

                    ' 学校スケジュール更新
                    '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                    '特別振替日対応
                    '特別スケジュールは学年と再振日を変えることで、同一振替日に複数レコード作成することができるため、
                    '学年に紐付く学校スケジュールマスタを更新するように修正する

                    '--------------------------------------------------
                    '学校スケジュールマスタのレコード件数抽出
                    '--------------------------------------------------
                    Dim MultiGScheduleFlg As Boolean = True

                    With SQL
                        .Length = 0
                        .Append("select count(*) as COUNTER from G_SCHMAST")
                        .Append(" where GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        .Append(" and FURI_DATE_S = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                        .Append(" and FURI_KBN_S = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))
                        .Append(" and TYUUDAN_FLG_S = '0'")
                    End With

                    If OraCount.DataReader(SQL) = True Then
                        'スケジュールが1レコード
                        If OraCount.GetInt("COUNTER") = 1 Then
                            MultiGScheduleFlg = False
                        End If
                    End If

                    OraCount.Close()

                    '学校スケジュールマスタが同一振替日に複数レコード存在しない場合は、既存の処理を行う
                    If MultiGScheduleFlg = False Then

                        '-----------------------------------------------
                        '学校自振振替済み件数、金額、不能件数、金額の取得
                        '-----------------------------------------------
                        Dim dblGFURI_KEN As Decimal = 0
                        Dim dblGFURI_KIN As Decimal = 0
                        Dim dblGFUNOU_KEN As Decimal = 0
                        Dim dblGFUNOU_KIN As Decimal = 0
                        dblGFURI_KEN = 0
                        dblGFURI_KIN = 0
                        dblGFUNOU_KEN = 0
                        dblGFUNOU_KIN = 0
                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                        SQL.Append("  AND FURIKETU_CODE_K = 0")
                        SQL.Append("  AND DATA_KBN_K = '2'")
                        SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                        If OraCount.DataReader(SQL) = True Then
                            dblGFURI_KEN = OraCount.GetInt64("CNT1")
                            dblGFURI_KIN = OraCount.GetInt64("CNT2")
                        End If
                        OraCount.Close()

                        SQL = New StringBuilder(128)
                        SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                        SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                        SQL.Append("   AND FURIKETU_CODE_K <> 0")
                        SQL.Append("   AND DATA_KBN_K = '2'")
                        SQL.Append("   AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        SQL.Append("   AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                        If OraCount.DataReader(SQL) = True Then
                            dblGFUNOU_KEN = OraCount.GetInt64("CNT1")
                            dblGFUNOU_KIN = OraCount.GetInt64("CNT2")
                        End If
                        OraCount.Close()

                        '-------------------------------------------
                        '学校自振スケジュールマスタの更新
                        '-------------------------------------------
                        SQL = New StringBuilder(128)
                        SQL.Append("UPDATE G_SCHMAST SET")
                        SQL.Append(" FUNOU_FLG_S = '1'")
                        SQL.Append(",FURI_KEN_S  = " & dblGFURI_KEN.ToString)
                        SQL.Append(",FURI_KIN_S = " & dblGFURI_KIN.ToString)
                        SQL.Append(",FUNOU_KEN_S = " & dblGFUNOU_KEN.ToString)
                        SQL.Append(",FUNOU_KIN_S = " & dblGFUNOU_KIN)
                        SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                        SQL.Append(" WHERE ")
                        SQL.Append("     FURI_DATE_S   = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                        SQL.Append(" AND GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                        SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                        SQL.Append(" AND FURI_KBN_S    = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))

                        Try
                            Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
                        Catch ex As Exception
                            MainLOG.Write("学校スケジュール更新", "失敗", SQL.ToString & " " & ex.Message & ":" & ex.StackTrace)
                            MainLOG.UpdateJOBMASTbyErr("学校スケジュール更新 学校コード：" & OraSchReader.GetString("TORIS_CODE_S") _
                                    & " 振替日：" & OraSchReader.GetString("TORIS_CODE_S"))
                            Return False
                        End Try


                    Else
                        '============================================================
                        '同一振替日に複数レコード存在する場合
                        '============================================================

                        '--------------------------------------------------
                        '学校スケジュールマスタ抽出
                        '--------------------------------------------------
                        With SQL
                            .Length = 0
                            .Append("select ")
                            .Append(" GAKKOU_CODE_S")
                            .Append(",FURI_DATE_S")
                            .Append(",FURI_KBN_S")
                            .Append(",GAKUNEN1_FLG_S")
                            .Append(",GAKUNEN2_FLG_S")
                            .Append(",GAKUNEN3_FLG_S")
                            .Append(",GAKUNEN4_FLG_S")
                            .Append(",GAKUNEN5_FLG_S")
                            .Append(",GAKUNEN6_FLG_S")
                            .Append(",GAKUNEN7_FLG_S")
                            .Append(",GAKUNEN8_FLG_S")
                            .Append(",GAKUNEN9_FLG_S")
                            .Append(" from G_SCHMAST")
                            .Append(" where GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                            .Append(" and FURI_DATE_S = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                            .Append(" and FURI_KBN_S = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))
                            .Append(" and TYUUDAN_FLG_S = '0'")
                        End With

                        If OraCount.DataReader(SQL) = True Then
                            While OraCount.EOF = False

                                Dim dblGFURI_KEN As Decimal = 0
                                Dim dblGFURI_KIN As Decimal = 0
                                Dim dblGFUNOU_KEN As Decimal = 0
                                Dim dblGFUNOU_KIN As Decimal = 0

                                '--------------------------------------------------
                                '学年フラグが立っている明細に対して集計を行う
                                '--------------------------------------------------
                                For i As Integer = 1 To 9
                                    If OraCount.GetString("GAKUNEN" & i.ToString & "_FLG_S") = "1" Then
                                        With SQL
                                            .Length = 0
                                            .Append("select ")
                                            .Append(" sum(decode(FURIKETU_CODE_M, 0, 1, 0)) as FURI_KEN")
                                            .Append(",sum(decode(FURIKETU_CODE_M, 0, SEIKYU_KIN_M)) as FURI_KIN")
                                            .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, 1)) as FUNO_KEN")
                                            .Append(",sum(decode(FURIKETU_CODE_M, 0, 0, SEIKYU_KIN_M)) as FUNO_KIN")
                                            .Append(" from G_MEIMAST")
                                            .Append(" where GAKKOU_CODE_M = " & SQ(OraCount.GetString("GAKKOU_CODE_S")))
                                            .Append(" and FURI_DATE_M = " & SQ(OraCount.GetString("FURI_DATE_S")))
                                            .Append(" and FURI_KBN_M = " & SQ(OraCount.GetString("FURI_KBN_S")))
                                            .Append(" and GAKUNEN_CODE_M = " & i.ToString)
                                        End With

                                        OraMeiReader.Close()
                                        If OraMeiReader.DataReader(SQL) = True Then
                                            dblGFURI_KEN += OraMeiReader.GetInt("FURI_KEN")
                                            dblGFURI_KIN += OraMeiReader.GetInt64("FURI_KIN")
                                            dblGFUNOU_KEN += OraMeiReader.GetInt("FUNO_KEN")
                                            dblGFUNOU_KIN += OraMeiReader.GetInt64("FUNO_KIN")
                                        End If
                                    End If
                                Next

                                '--------------------------------------------------
                                '学校スケジュールマスタ更新
                                '--------------------------------------------------
                                With SQL
                                    .Length = 0
                                    .Append("update G_SCHMAST set")
                                    .Append(" FUNOU_FLG_S = '1'")
                                    .Append(",FURI_KEN_S = " & dblGFURI_KEN.ToString)
                                    .Append(",FURI_KIN_S = " & dblGFURI_KIN.ToString)
                                    .Append(",FUNOU_KEN_S = " & dblGFUNOU_KEN.ToString)
                                    .Append(",FUNOU_KIN_S = " & dblGFUNOU_KIN.ToString)
                                    .Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                                    .Append(" where GAKKOU_CODE_S = " & SQ(OraCount.GetString("GAKKOU_CODE_S")))
                                    .Append(" and FURI_DATE_S = " & SQ(OraCount.GetString("FURI_DATE_S")))
                                    .Append(" and FURI_KBN_S = " & SQ(OraCount.GetString("FURI_KBN_S")))
                                    .Append(" and GAKUNEN1_FLG_S = " & SQ(OraCount.GetString("GAKUNEN1_FLG_S")))
                                    .Append(" and GAKUNEN2_FLG_S = " & SQ(OraCount.GetString("GAKUNEN2_FLG_S")))
                                    .Append(" and GAKUNEN3_FLG_S = " & SQ(OraCount.GetString("GAKUNEN3_FLG_S")))
                                    .Append(" and GAKUNEN4_FLG_S = " & SQ(OraCount.GetString("GAKUNEN4_FLG_S")))
                                    .Append(" and GAKUNEN5_FLG_S = " & SQ(OraCount.GetString("GAKUNEN5_FLG_S")))
                                    .Append(" and GAKUNEN6_FLG_S = " & SQ(OraCount.GetString("GAKUNEN6_FLG_S")))
                                    .Append(" and GAKUNEN7_FLG_S = " & SQ(OraCount.GetString("GAKUNEN7_FLG_S")))
                                    .Append(" and GAKUNEN8_FLG_S = " & SQ(OraCount.GetString("GAKUNEN8_FLG_S")))
                                    .Append(" and GAKUNEN9_FLG_S = " & SQ(OraCount.GetString("GAKUNEN9_FLG_S")))
                                End With

                                Try
                                    Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
                                Catch ex As Exception
                                    MainLOG.Write("学校スケジュール更新", "失敗", SQL.ToString & " " & ex.Message & ":" & ex.StackTrace)
                                    MainLOG.UpdateJOBMASTbyErr("学校スケジュール更新 学校コード：" & OraSchReader.GetString("TORIS_CODE_S") _
                                            & " 振替日：" & OraSchReader.GetString("TORIS_CODE_S"))
                                    Return False
                                End Try

                                OraCount.NextRead()
                            End While
                        End If

                        OraCount.Close()

                    End If

                    ''-----------------------------------------------
                    ''学校自振振替済み件数、金額、不能件数、金額の取得
                    ''-----------------------------------------------
                    'Dim dblGFURI_KEN As Decimal = 0
                    'Dim dblGFURI_KIN As Decimal = 0
                    'Dim dblGFUNOU_KEN As Decimal = 0
                    'Dim dblGFUNOU_KIN As Decimal = 0
                    'dblGFURI_KEN = 0
                    'dblGFURI_KIN = 0
                    'dblGFUNOU_KEN = 0
                    'dblGFUNOU_KIN = 0
                    'SQL = New StringBuilder(128)
                    'SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                    'SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                    'SQL.Append("  AND FURIKETU_CODE_K = 0")
                    'SQL.Append("  AND DATA_KBN_K = '2'")
                    'SQL.Append("  AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    'SQL.Append("  AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    'If OraCount.DataReader(SQL) = True Then
                    '    dblGFURI_KEN = OraCount.GetInt64("CNT1")
                    '    dblGFURI_KIN = OraCount.GetInt64("CNT2")
                    'End If
                    'OraCount.Close()

                    'SQL = New StringBuilder(128)
                    'SQL.Append("SELECT COUNT(KEIYAKU_KNAME_K) CNT1,SUM(FURIKIN_K) CNT2 FROM MEIMAST")
                    'SQL.Append(" WHERE FURI_DATE_K = " & SQ(KeyFuriDate))
                    'SQL.Append("   AND FURIKETU_CODE_K <> 0")
                    'SQL.Append("   AND DATA_KBN_K = '2'")
                    'SQL.Append("   AND TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    'SQL.Append("   AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                    'If OraCount.DataReader(SQL) = True Then
                    '    dblGFUNOU_KEN = OraCount.GetInt64("CNT1")
                    '    dblGFUNOU_KIN = OraCount.GetInt64("CNT2")
                    'End If
                    'OraCount.Close()

                    ''-------------------------------------------
                    ''学校自振スケジュールマスタの更新
                    ''-------------------------------------------
                    'SQL = New StringBuilder(128)
                    'SQL.Append("UPDATE G_SCHMAST SET")
                    'SQL.Append(" FUNOU_FLG_S = '1'")
                    'SQL.Append(",FURI_KEN_S  = " & dblGFURI_KEN.ToString)
                    'SQL.Append(",FURI_KIN_S = " & dblGFURI_KIN.ToString)
                    'SQL.Append(",FUNOU_KEN_S = " & dblGFUNOU_KEN.ToString)
                    'SQL.Append(",FUNOU_KIN_S = " & dblGFUNOU_KIN)
                    'SQL.Append(",FUNOU_DATE_S = TO_CHAR(SYSDATE,'YYYYMMDD')")
                    'SQL.Append(" WHERE ")
                    'SQL.Append("     FURI_DATE_S   = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                    'SQL.Append(" AND GAKKOU_CODE_S = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                    'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                    'SQL.Append(" AND FURI_KBN_S    = " & SQ((CAInt32(OraSchReader.GetString("TORIF_CODE_S")) - 1).ToString))

                    'Try
                    '    Dim nCount As Integer = MainDB.ExecuteNonQuery(SQL)
                    'Catch ex As Exception
                    '    MainLOG.Write("学校スケジュール更新", "失敗", SQL.ToString & " " & ex.Message & ":" & ex.StackTrace)
                    '    MainLOG.UpdateJOBMASTbyErr("学校スケジュール更新 学校コード：" & OraSchReader.GetString("TORIS_CODE_S") _
                    '            & " 振替日：" & OraSchReader.GetString("TORIS_CODE_S"))
                    '    Return False
                    'End Try
                    '2017/03/14 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END

                    OraSchReader.NextRead()
                Loop

                OraSchReader.Close()

            Else
                MainLOG.Write("学校スケジュール対象レコードなし", "成功", "配信済なし，フォーマット区分：07，振替日：" & KeyFuriDate)
            End If

            Return True

            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Finally
            If Not OraMeiReader Is Nothing Then
                OraMeiReader.Close()
            End If

            If Not OraSchReader Is Nothing Then
                OraSchReader.Close()
            End If

            If Not OraCount Is Nothing Then
                OraCount.Close()
            End If
        End Try
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

    End Function

#End Region

#Region " 媒体読み込み"
    Public Function fn_BAITAI_READ() As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_READ
        'Parameter      :
        'Description    :企業持込媒体読み込み処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/12
        'Update         :
        '============================================================================
        fn_BAITAI_READ = False
        Try

            Dim Set_Code As String = ""
            If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                Set_Code = mArgumentData.INFOToriMast.TORIS_CODE_T + mArgumentData.INFOToriMast.TORIF_CODE_T
                strIN_FILE_NAME = DEN_Folder & "R" & Set_Code & ".DAT"
                strOUT_FILE_NAME = DAT_Folder & "R" & Set_Code & "_JIS.DAT"
            Else
                Set_Code = mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T
                strIN_FILE_NAME = DEN_Folder & "R" & Set_Code & ".DAT"
                strOUT_FILE_NAME = DAT_Folder & "R" & Set_Code & "_JIS.DAT"
            End If

            '媒体読込
            strP_FILE = "120.P"
            intREC_LENGTH = 120

            '伝送のみ
            Select Case clsFUSION.fn_DISK_CPYTO_DEN(MainLOG.ToriCode, strIN_FILE_NAME, strOUT_FILE_NAME, _
                                               intREC_LENGTH, "0", strP_FILE)
                'Return         :0=成功、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
                '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
                Case 0
                    fn_BAITAI_READ = True
                    MainLOG.Write("(伝送ファイル取込)", "成功", "ファイル名:" & strOUT_FILE_NAME)
                Case 100
                    fn_BAITAI_READ = False
                    MainLOG.Write("(伝送ファイル取込)", "失敗", "コード変換失敗:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
                Case 200
                    fn_BAITAI_READ = False
                    MainLOG.Write("(伝送ファイル取込)", "失敗", "コード区分異常:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
                Case 300
                    fn_BAITAI_READ = False
                    MainLOG.Write("(伝送ファイル取込)", "失敗", "コード区分異常（JIS改行なし）:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
                Case 400
                    fn_BAITAI_READ = False
                    MainLOG.Write("(伝送ファイル取込)", "失敗", "出力ファイル作成:" & Microsoft.VisualBasic.Err.Description)
                    Exit Function
            End Select
            fn_BAITAI_READ = True
        Catch ex As Exception

        End Try
    End Function

    Public Function fn_TAKO_BAITAI_READ() As Boolean
        '============================================================================
        'NAME           :fn_BAITAI_READ
        'Parameter      :
        'Description    :媒体読み込み処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/12
        'Update         :
        '============================================================================
        fn_TAKO_BAITAI_READ = False

        Try
            '検索用パラメータ設定
            strTORIS_CODE = Mid(mKoParam.CP.TORI_CODE, 1, 10)
            strTORIF_CODE = Mid(mKoParam.CP.TORI_CODE, 11, 2)
            strTAKOU_KIN = mKoParam.TKIN_CODE

            Dim intKEKKA As Integer
            Dim strKEKKA As String
            Dim SQL As New StringBuilder(128)
            '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            'Dim oraReader As New CASTCommon.MyOracleReader
            Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
            Dim strFMT_KBN As String
            SQL.Append("SELECT * ")
            SQL.Append(" FROM TAKOSCHMAST,TAKOUMAST")
            SQL.Append(" WHERE TORIS_CODE_U = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_U = " & SQ(strTORIF_CODE))
            SQL.Append(" AND FURI_DATE_U = " & SQ(mKoParam.CP.FURI_DATE))
            If Trim(strTAKOU_KIN) = MatomeNoukyo Then
                SQL.Append(" AND TKIN_NO_U BETWEEN " & SQ(Noukyo_From) & " AND " & SQ(Noukyo_To))
            Else
                SQL.Append(" AND TKIN_NO_U = " & SQ(strTAKOU_KIN))
            End If
            SQL.Append(" AND TORIS_CODE_U = TORIS_CODE_V")
            SQL.Append(" AND TORIF_CODE_U = TORIF_CODE_V")
            SQL.Append(" AND TKIN_NO_U = TKIN_NO_V")
            If oraReader.DataReader(SQL) Then
                strLABEL_CODE = CType(oraReader.GetString("LABEL_CODE_U"), Short)
                strTAKO_BAITAI_CODE = oraReader.GetString("BAITAI_CODE_U")
                strFMT_KBN = oraReader.GetString("FMT_KBN_U")
                strCODE_KBN = oraReader.GetString("CODE_KBN_U")
                strR_FILE_NAME = oraReader.GetString("RFILE_NAME_V")
                oraReader.Close()
            Else
                '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                oraReader.Close()
                '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                '他行マスタ未登録
                MainLOG.Write("(他行スケジュールマスタ取得)", "失敗", "他行スケジュールマスタ未登録:" & strTAKOU_KIN)
            End If

            Select Case mArgumentData.INFOParameter.FMT_KBN
                Case "00" '全銀
                    intREC_LENGTH = 120
                    intBLK_SIZE = 1800
                    strP_FILE = "120.P"
                Case Else   '2009/12/18 追加
                    MainLOG.Write("フォーマット区分判定", "失敗", "フォーマット区分異常:" & mArgumentData.INFOToriMast.FMT_KBN_T)
                    Exit Function
            End Select

            '2017/05/25 タスク）西野 CHG 標準版修正（潜在バグ修正）-------------------------- START
            If mKoParam.KOUSIN_KBN = "1" OrElse strTAKO_BAITAI_CODE = "04" Then
                'ファイル名を初期化する
                strIN_FILE_NAME = ""
                Return True '強制更新/依頼書はこのまま正常終了
            End If
            'If mKoParam.KOUSIN_KBN = "1" OrElse strTAKO_BAITAI_CODE = "04" Then Return True '強制更新/依頼書はこのまま正常終了
            '2017/05/25 タスク）西野 CHG 標準版修正（潜在バグ修正）-------------------------- END

            strIN_FILE_NAME = DEN_Folder & strR_FILE_NAME.Trim     '入力ファイル
            If Not Directory.Exists(TAK_Folder & strTAKOU_KIN) Then
                Directory.CreateDirectory(TAK_Folder & strTAKOU_KIN)
            End If
            strOUT_FILE_NAME = TAK_Folder & strTAKOU_KIN & "\F" & strTAKOU_KIN & strTORIS_CODE & strTORIF_CODE & System.DateTime.Today.ToString("yyyyMMddhhmmdd") & ".dat"   '出力ファイル

            strBKUP_FILE = DENBK_Folder & strR_FILE_NAME.Trim.PadLeft(1, " "c)  'バックアップファイル

            Dim Baitai As String = ""
            If (strTAKO_BAITAI_CODE = "05" AndAlso gstrMT = "1") OrElse _
               (strTAKO_BAITAI_CODE = "06" AndAlso gstrCMT = "1") Then
                strIN_FILE_NAME = Path.Combine(DEN_Folder, "TAKOU\" & strTAKOU_KIN)
                Baitai = "00"
            Else
                Baitai = strTAKO_BAITAI_CODE
            End If
            Select Case Baitai
                Case "00"        '伝送
                    intKEKKA = clsFUSION.fn_DEN_CPYTO_DISK(LW.ToriCode, strIN_FILE_NAME, strOUT_FILE_NAME, _
                                                           intREC_LENGTH, strCODE_KBN, strP_FILE, msgTitle)
                    Select Case intKEKKA
                        'Return         :0=成功、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
                        '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
                        Case 0
                            fn_TAKO_BAITAI_READ = True
                            MainLOG.Write("(伝送ファイル取込)", "成功", "ファイル名:" & strOUT_FILE_NAME)
                        Case 50
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(伝送ファイル取込)", "失敗", "ファイルなし:" & Err.Description)
                            Exit Function
                        Case 100
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(伝送ファイル取込)", "失敗", Err.Description)
                            Exit Function
                        Case 200
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(伝送ファイル取込)", "失敗", "コード変換失敗:" & Err.Description)
                            Exit Function
                        Case 300
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(伝送ファイル取込)", "失敗", "コード区分異常（JIS改行なし）:" & Err.Description)
                            Exit Function
                        Case 400
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(伝送ファイル取込)", "失敗", "出力ファイル作成:" & Err.Description)
                            Exit Function
                    End Select

                Case "01"        'ＦＤ３．５
                    strIN_FILE_NAME = strR_FILE_NAME.Trim
                    intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(LW.ToriCode, strIN_FILE_NAME, strOUT_FILE_NAME, _
                                                          intREC_LENGTH, strCODE_KBN, strP_FILE, msgTitle)
                    Select Case intKEKKA
                        Case 0
                            fn_TAKO_BAITAI_READ = True
                            MainLOG.Write("(ＦＤ取込)", "成功", "ファイル名:" & strOUT_FILE_NAME)
                        Case 100
                            fn_TAKO_BAITAI_READ = False
                            'Return         :0=成功、100=ファイル読み込み失敗、、200=コード区分異常（JIS改行あり）、
                            '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
                            MainLOG.Write("(ＦＤ取込)", "失敗", "コード変換:" & Err.Description)
                            Exit Function
                        Case 200
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(ＦＤ取込)", "失敗", "コード区分異常（JIS改行あり）:" & Err.Description)
                            Exit Function
                        Case 300
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(ＦＤ取込)", "失敗", "コード区分異常（JIS改行なし）:" & Err.Description)
                            Exit Function
                        Case 400
                            fn_TAKO_BAITAI_READ = False
                            MainLOG.Write("(ＦＤ取込)", "失敗", "出力ファイル作成:" & Err.Description)
                            Exit Function
                    End Select
                Case "05"        'ＭＴ
                    Select Case gstrMT
                        Case "0"     'ＭＴが直接自振サーバに接続している場合
                            strIN_FILE_NAME = " "
                            '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                            Dim lngErrStatus As Long
                            strKEKKA = vbDLL.mtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            'Dim lngErrStatus As Integer
                            'strKEKKA = vbDLL.mtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END
                            If strKEKKA <> "" Then
                                MainLOG.Write("(MT取込)", "失敗", Err.Description)
                                Exit Function
                            Else
                                MainLOG.Write("(MT取込)", "成功", "ファイル名:" & strOUT_FILE_NAME)
                            End If
                        Case "1"      'ＭＴが自振サーバに接続していない場合
                            If Dir(strMT_FUNOU_FILE) = "" Then
                                MainLOG.Write("(MT取込)", "失敗", "ファイルなし:" & strMT_FUNOU_FILE)
                                Exit Function
                            End If
                            File.Copy(strMT_FUNOU_FILE, strOUT_FILE_NAME)
                            If Err.Number <> 0 Then
                                MainLOG.Write("(MT取込)", "失敗", "ファイルコピー:" & Err.Description)
                                Exit Function
                            End If
                    End Select

                Case "06"        'ＣＭＴ
                    Select Case gstrCMT
                        Case "0"    'ＣＭＴが直接自振サーバに接続している場合
                            strIN_FILE_NAME = " "
                            '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                            Dim lngErrStatus As Long
                            strKEKKA = vbDLL.cmtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            'Dim lngErrStatus As Integer
                            'strKEKKA = vbDLL.cmtCPYtoDISK(intBLK_SIZE, intREC_LENGTH, strLABEL_CODE, "SMLT", 1, 4, strIN_FILE_NAME, strOUT_FILE_NAME, 0, lngErrStatus)
                            '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END
                            If strKEKKA <> "" Then
                                MainLOG.Write("(CMT取込)", "失敗", Err.Description)
                                Exit Function
                            Else
                                MainLOG.Write("(CMT取込)", "成功", "ファイル名:" & strOUT_FILE_NAME)
                            End If
                        Case "1"    'ＣＭＴが自振サーバに接続していない場合
                            If Dir(strCMT_FUNOU_FILE) = "" Then
                                MainLOG.Write("(CMT取込)", "失敗", "ファイル検索:" & strCMT_FUNOU_FILE)
                                Exit Function
                            End If
                            File.Copy(strCMT_FUNOU_FILE, strOUT_FILE_NAME)
                            If Err.Number <> 0 Then
                                MainLOG.Write("(MT取込)", "失敗", "ファイルコピー:" & Err.Description)
                                Exit Function
                            End If
                    End Select
                Case "07"        '学校自振
                Case "09"        '伝票
                    fn_TAKO_BAITAI_READ = True
                Case Else
            End Select
        Catch ex As Exception
            strOUT_FILE_NAME = ""
            MainLOG.Write("(他行媒体取込)", "失敗", ex.ToString)

        End Try
        fn_TAKO_BAITAI_READ = True

    End Function
#End Region

    ''' <summary>
    ''' 処理結果確認表(不能結果更新)の印刷処理を行います。（スリーエス不能結果更新用）
    ''' </summary>
    ''' <param name="UpdateKeys"></param>
    ''' <returns></returns>
    ''' <remarks>2017/01/20 saitou 東春信金(RSV2標準) added for スリーエス対応</remarks>
    Private Function PrintSyoriKekkaKakuninhyoForSSS(ByVal UpdateKeys As ArrayList) As Boolean
        Dim dbReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim CreateCSV As New KFJP013
        Dim strCSV_FILE_NAME As String = String.Empty
        Dim strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        Dim strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

        Try
            Dim strTorisCode As String
            Dim strTorifCode As String
            Dim strFuriDate As String

            '----------------------------------------
            '印刷判定
            '----------------------------------------
            If INI_RSV2_SYORIKEKKA_FUNOU = "0" Then
                Return True
            End If

            '----------------------------------------
            '印刷処理前に引数のチェック
            '----------------------------------------
            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                If Keys.Length = 5 Then
                    'OK
                Else
                    'スリーエスの場合、スケジュール更新キーの個数5なので、異常とする
                    MainLOG.Write("処理結果確認表(不能結果更新)印刷", "失敗", "印刷キー異常")
                    Return False
                End If
            Next

            '----------------------------------------
            '印刷用CSV作成
            '----------------------------------------
            strCSV_FILE_NAME = CreateCSV.CreateCsvFile()
            dbReader = New CASTCommon.MyOracleReader(MainDB)

            For i As Integer = 0 To UpdateKeys.Count - 1
                Dim Keys() As String = CType(UpdateKeys(i), String())
                strTorisCode = Keys(0)
                strTorifCode = Keys(1)
                strFuriDate = Keys(2)

                With SQL
                    .Length = 0
                    .Append("select")
                    .Append("     TORIS_CODE_S")
                    .Append("    ,TORIF_CODE_S")
                    .Append("    ,FURI_DATE_S")
                    .Append("    ,KIGYO_CODE_S")
                    .Append("    ,FURI_CODE_S")
                    .Append("    ,ITAKU_NNAME_T")
                    .Append("    ,ITAKU_CODE_T")
                    .Append("    ,FUNOU_FLG_S")
                    .Append("    ,SYORI_KEN_S")
                    .Append("    ,SYORI_KIN_S")
                    .Append("    ,FUNOU_KEN_S")
                    .Append("    ,FUNOU_KIN_S")
                    .Append(" from")
                    .Append("     SCHMAST")
                    .Append(" inner join")
                    .Append("     TORIMAST")
                    .Append(" on  TORIS_CODE_S = TORIS_CODE_T")
                    .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                    .Append(" where")
                    .Append("     FURI_DATE_S = " & SQ(strFuriDate))
                    .Append(" and FUNOU_FLG_S in ('1', '2')")
                    .Append(" and TYUUDAN_FLG_S = '0'")
                    .Append(" and TORIS_CODE_S = " & SQ(strTorisCode))
                    .Append(" and TORIF_CODE_S = " & SQ(strTorifCode))
                    .Append(" order by")
                    .Append("     TORIS_CODE_S")
                    .Append("    ,TORIF_CODE_S")
                End With

                If dbReader.DataReader(SQL) = True Then
                    While dbReader.EOF = False
                        With CreateCSV
                            .OutputCsvData(strDate, True)                                       'システム日付
                            .OutputCsvData(strTime, True)                                       'タイムスタンプ
                            .OutputCsvData(dbReader.GetString("FURI_DATE_S"), True)             '振替日
                            .OutputCsvData(dbReader.GetString("TORIS_CODE_S"), True)            '取引先主コード
                            .OutputCsvData(dbReader.GetString("TORIF_CODE_S"), True)            '取引先副コード
                            .OutputCsvData(dbReader.GetString("ITAKU_NNAME_T"), True)           '委託者名
                            .OutputCsvData(dbReader.GetString("ITAKU_CODE_T"), True)            '委託者コード
                            .OutputCsvData(dbReader.GetInt("SYORI_KEN_S").ToString)             '依頼件数
                            .OutputCsvData(dbReader.GetInt64("SYORI_KIN_S").ToString)           '依頼金額
                            .OutputCsvData(dbReader.GetInt("FUNOU_KEN_S").ToString)             '不能件数
                            .OutputCsvData(dbReader.GetInt64("FUNOU_KIN_S").ToString)           '不能金額
                            Select Case dbReader.GetString("FUNOU_FLG_S")                       '備考
                                Case "1"
                                    .OutputCsvData("不能結果更新完了", True)
                                Case "2"
                                    .OutputCsvData("未処理あり", True)
                            End Select
                            .OutputCsvData(dbReader.GetString("FURI_CODE_S"), True)             '振替コード
                            .OutputCsvData(dbReader.GetString("KIGYO_CODE_S"), True, True)      '企業コード
                        End With

                        dbReader.NextRead()
                    End While
                End If

                dbReader.Close()
            Next

            CreateCSV.CloseCsv()

            '----------------------------------------
            '印刷バッチ呼び出し
            '----------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim execName As String = "KFJP013.EXE"
            Dim CmdArg As String = MainLOG.UserID & "," & strCSV_FILE_NAME
            Dim Ret As Integer
            Ret = ExeRepo.ExecReport(execName, CmdArg)
            Select Case Ret
                Case 0
                    MainLOG.Write("処理結果確認表(不能結果更新)印刷", "成功")
                    Return True
                Case -1
                    MainLOG.Write("処理結果確認表(不能結果更新)印刷", "失敗", "印刷対象が0件")
                    Return False
                Case Else
                    MainLOG.Write("処理結果確認表(不能結果更新)印刷", "失敗", "印刷失敗")
                    MainLOG.UpdateJOBMASTbyErr("処理結果確認表(不能結果更新)印刷失敗")
                    Return False
            End Select

        Catch ex As Exception
            MainLOG.Write("処理結果確認表(不能結果更新)印刷", "失敗", ex.Message)
            MainLOG.UpdateJOBMASTbyErr("処理結果確認表(不能結果更新)印刷失敗")
            Return False

        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If
        End Try
    End Function

    Private Function CompressArray(ByVal arr As ArrayList) As String()
        Dim WorkArray As ArrayList = New ArrayList(arr.Count)

        Dim OldKey As String = ""
        For i As Integer = 0 To arr.Count - 1
            If OldKey <> CType(arr(i), String) Then
                WorkArray.Add(arr(i))
            End If
        Next i

        Return CType(WorkArray.ToArray(GetType(String)), String())
    End Function

    Public Class mySearchClass
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
            Implements IComparer.Compare

            Dim xA() As String = CType(x, String())
            Dim yA() As String = CType(y, String())

            For i As Integer = 0 To xA.Length - 1
                If xA(i) <> yA(i) Then
                    Return -1
                End If
            Next i

            Return 0
        End Function 'IComparer.Compare
    End Class 'myReverserClass

    Public Class mySearchClass2
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
            Implements IComparer.Compare

            Dim xA As String = CType(x, String)
            Dim yA As String = CType(y, String)

            Return String.Compare(xA, yA)
        End Function 'IComparer.Compare
    End Class 'myReverserClass

    Public Shared Sub PrintIndexAndValues(ByVal myList As IEnumerable)
        Dim i As Integer = 0
        Dim myEnumerator As System.Collections.IEnumerator = myList.GetEnumerator()
        While myEnumerator.MoveNext()
            Console.WriteLine(Microsoft.VisualBasic.ControlChars.Tab + "[{0}]:" + Microsoft.VisualBasic.ControlChars.Tab + "{1}", i, myEnumerator.Current)
            i = i + 1
        End While
        Console.WriteLine()
    End Sub 'PrintIndexAndValues
End Class
