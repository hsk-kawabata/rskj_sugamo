Imports clsFUSION.clsMain
Imports System.Drawing.Printing
Imports System.Text
Imports System.IO
Imports CASTCommon.ModPublic
Imports System.Data.OracleClient
Imports System.Runtime.InteropServices
Imports CASTCommon

Public Class KFJMAIN030
    Inherits System.Windows.Forms.Form

    Private clsFUSION As New clsFUSION.clsMain

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN030", "センターカットデータ作成画面")
    Private Const msgTitle As String = "センターカットデータ作成画面(KFJMAIN030)"
    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    'クリックした列の番号
    Dim ClickedColumn As Integer

    Public WriteOnly Property SetDayCounter() As Integer
        Set(ByVal Value As Integer)
            DayCounter = Value
        End Set
    End Property

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private DayCounter As Integer
    '*** 2008/6/24 kaki**********************
    '*** カウンターの最大数を変更         
    'Private Const MaxCounter As Integer = 60
    Private Const MaxCounter As Integer = 600
    '*** 2008/6/24 kaki**********************

    Private htTORI_LIST As Hashtable '2010/12/24 信組対応 企業持込の複数振替日制御

#Region "宣言"
    Public intSCH_COUNT As Integer
    Public strTORI_CODE(MaxCounter) As String
    Public strFURI_DATE(MaxCounter) As String
    Public strHAISIN_YDATE(MaxCounter) As String
    Public strSOUSIN_KBN(MaxCounter) As String
    Public strITAKU_NNAME(MaxCounter) As String
    Public strKIGYO_CODE(MaxCounter) As String
    Public strFURI_CODE(MaxCounter) As String
    ' 2008.01.16 >>
    Public strJIKOTAKO(MaxCounter) As String        ' 自行，他行，SSS
    ' 2008.01.16 <<
    ' 080207 ADD START
    Public strKENSU(MaxCounter) As String
    Public strTAKOU_KBN(MaxCounter) As String    ' 他行区分格納
    ' 080207 ADD END
    Public intMAXCNT As Integer
    Public intMAXLINE As Integer
    Public intPAGE_CNT As Integer
    Public P As Integer
    Public L As Integer

    Public intP_PAGE As Integer


    '帳票印刷用
    Private intGYO As Integer
    Private intMOJI_SIZE As Integer = 5
    Private intP As Integer
    Private intL As Integer

    Private mTakouDensoList As String()
    Private mTakouDensoGaiList As String()

    '2013/10/24 saitou 標準修正 ADD -------------------------------------------------->>>>
    ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- START
    'Private CENTER As String
    'Private KINKOCD As String
    ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- END

    ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- START
    Private INI_COMMON_DEN As String
    Private INI_COMMON_CENTER As String
    Private INI_COMMON_KINKOCD As String
    Private INI_JIFURI_FILEAM As String
    Private INI_JIFURI_FILEPM As String
    Private INI_JIFURI_KITEIKEN As String
    Private INI_JIFURI_BORDER_TIME As String
    Private INI_JIFURI_CCDATACHK As String
    Private INI_JIFURI_BORDERCHK As String
    Private INI_JIFURI_ENTRYCHK As String
    Private INI_JIFURI_CCFNAME As String
    ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- END
    '2017/06/08 タスク）西野 ADD 桑名信金対応（学校徴収金対応【標準版修正】）--------------------- START
    Private INI_JYOUKEN As String = ""
    '2017/06/08 タスク）西野 ADD 桑名信金対応（学校徴収金対応【標準版修正】）--------------------- END
#End Region

#Region "画面のロード"
    Protected Sub KFJMAIN030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim OraDB As New CASTCommon.MyOracle()
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQL As StringBuilder
        Dim aryCenter As New ArrayList
        Dim cData(10) As String
        Dim NextData As String = Nothing '2009/11/30 追加 次営業日

        ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
        ''2010.02.19 境界時間をiniより取得 start
        'Dim strBorder As String = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
        'If strBorder = "err" OrElse strBorder = "" Then
        '    strBorder = "1200"
        'End If
        ''2010.02.19 境界時間をiniより取得 end
        ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Me.Visible = False

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Select Case DayCounter
                Case 0
                    Me.Label3.Text = "＜センターカットデータ作成＞"
                    '                    Me.Label3.Location = New Point(232, 18)
                Case 1
                    Me.Label3.Text = "＜センターカット翌々日配信データ作成＞"
                    '                    Me.Label3.Location = New Point(222, 18)
                Case 2
                    'Me.Label3.Text = "＜センターカット翌日配信データ作成＞"
                    Me.Label3.Text = "＜自振ロギングデータ作成＞"
                    '                    Me.Label3.Location = New Point(262, 18)
            End Select

            txtCycle.Text = "1"

            'INIファイルの読み込み
            'gstrLOG_OPENDIR = GCom.GetLogFolder
            ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
            'CENTER = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

            '使用していない？2009.10.14
            'If FN_SET_INIFILE() = False Then
            '    MessageBox.Show("iniファイルの取得に失敗しました", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
            If GetIniInfo() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)ＩＮＩ情報取得", "失敗", "")
                Return
            End If
            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
            '-----------------------------------------------------------
            ' ＣＣデータ存在チェック
            '-----------------------------------------------------------
            Dim NowTimeStamp As String = System.DateTime.Now.ToString("HHmm")
            If INI_JIFURI_CCDATACHK = "YES" Then
                Dim GetFilesInfo() As String

                Select Case INI_COMMON_CENTER
                    Case "5"
                        '-----------------------------------------------
                        ' 大阪センターの場合(BORDER_TIMEにて切替)
                        '-----------------------------------------------
                        Select Case INI_JIFURI_CCFNAME
                            Case "1"
                                If NowTimeStamp < INI_JIFURI_BORDER_TIME Then
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM)
                                Else
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEPM)
                                End If
                            Case Else
                                If NowTimeStamp < INI_JIFURI_BORDER_TIME Then
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM & "*")
                                Else
                                    GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEPM & "*")
                                End If
                        End Select
                    Case Else
                        '-----------------------------------------------
                        ' 大阪センター以外の場合(FILEAM使用)
                        '-----------------------------------------------
                        Select Case INI_JIFURI_CCFNAME
                            Case "1"
                                GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM)
                            Case Else
                                GetFilesInfo = Directory.GetFiles(INI_COMMON_DEN, INI_JIFURI_FILEAM & "*")
                        End Select
                End Select

                If GetFilesInfo.Length > 0 Then
                    If MessageBox.Show(MSG0083I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                        ShowInTaskbar = False
                        Me.Close()
                        Exit Sub
                    End If
                End If
            End If
            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

            '休日マスタ取り込み
            Dim SubSQL As String = " WHERE SUBSTR(YASUMI_DATE_Y, 1, 6)"
            SubSQL &= " IN ('" & String.Format("{0:yyyyMM}", CASTCommon.Calendar.Now.AddMonths(-1)) & "'"
            SubSQL &= ", '" & String.Format("{0:yyyyMM}", CASTCommon.Calendar.Now) & "'"
            SubSQL &= ", '" & String.Format("{0:yyyyMM}", CASTCommon.Calendar.Now.AddMonths(1)) & "')"
            Dim BRet As Boolean = GCom.CheckDateModule(Nothing, CType(1, Short), SubSQL)

            '表示委託先の検索（スケジュールの検索）
            Dim KEY_HAISIN_YDATE As String = New String("0"c, 8)
            Dim HAISIN_YDATE_FROM As String = New String("0"c, 8)
            BRet = GCom.CheckDateModule(String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now), KEY_HAISIN_YDATE, DayCounter, 1)
            If DayCounter = 0 Then
                Dim nDay As Integer = GCom.NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN_OK"))
                If CASTCommon.GetFSKJIni("JIFURI", "HAISIN_OK") = "err" Then
                    nDay = GCom.NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN"))
                End If
                nDay -= GCom.NzInt(CASTCommon.GetFSKJIni("JIFURI", "HAISIN"))
                BRet = GCom.CheckDateModule(KEY_HAISIN_YDATE, HAISIN_YDATE_FROM, nDay, 0)
            Else
                '*** 修正 NISHIDA 2008/12/1 自振ロギングは配信予定日が作業日当日より前のものを対象とする。
                'HAISIN_YDATE_FROM = KEY_HAISIN_YDATE
                HAISIN_YDATE_FROM = String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now)
                '**********************************************************************************
            End If

            '2009/11/30 次営業日を取得する =======================
            BRet = GCom.CheckDateModule(String.Format("{0:yyyyMMdd}", CASTCommon.Calendar.Now), NextData, 1, 0)
            '=====================================================

            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
            '-----------------------------------------------------------
            ' ＣＣ境界時間チェック
            '-----------------------------------------------------------
            If INI_JIFURI_BORDERCHK = "YES" Then
                Dim CheckFuriDate As String = NextData
                Dim GetMessage As String = String.Empty
                If NowTimeStamp >= INI_JIFURI_BORDER_TIME Then
                    If BorderCheck(CheckFuriDate, GetMessage) = False Then
                        Return
                    Else
                        If GetMessage.Trim <> "" Then
                            MessageBox.Show(GetMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    End If
                End If
            End If

            '-----------------------------------------------------------
            ' ＣＣ時のエントリ未落込チェック
            '-----------------------------------------------------------
            If INI_JIFURI_ENTRYCHK = "YES" Then
                Dim GetMessage As String = String.Empty
                If EntryCheck(GetMessage) = False Then
                    Return
                Else
                    If GetMessage.Trim <> "" Then
                        MessageBox.Show(GetMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End If
            ' 2016/06/11 タスク）綾部 ADD 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

            SQL = New StringBuilder(128)

            SQL.Append("SELECT * FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_S = '1'")
            If DayCounter = 0 Then
                SQL.Append(" AND HAISIN_YDATE_S BETWEEN '" & KEY_HAISIN_YDATE & "' AND '" & HAISIN_YDATE_FROM & "'")
            Else
                '*** 修正 NISHIDA 2008/12/1 自振ロギングは配信予定日が作業日当日より前のもの、かつ、振替日が到来していないものを対象とする。
                SQL.Append(" AND HAISIN_YDATE_S > '" & KEY_HAISIN_YDATE & "'")
                SQL.Append(" AND HAISIN_YDATE_S < '" & HAISIN_YDATE_FROM & "'")
                '**********************************************************************************
            End If
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND FUNOU_FLG_S = '0'")
            SQL.Append(" AND (HAISIN_FLG_S = '0' OR HAISIN_FLG_S = '2')")        '配信フラグが0か2
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '2010/12/24 信組対応 送信区分を有効にする
            SQL.Append(" AND SOUSIN_KBN_S IN('0','1','2')")
            'SQL.Append(" AND SOUSIN_KBN_S IN('0')")
            SQL.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
            SQL.Append(" AND  SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
            '2009/11/30 時間によって翌営業日＝振替日分のデータを除く
            If Now.ToString("HHmm") >= INI_JIFURI_BORDER_TIME Then
                SQL.Append(" AND  FURI_DATE_S <> " & SQ(NextData))
            End If
            '=====================================================
            '2017/06/08 タスク）西野 ADD 桑名信金対応（学校徴収金対応【標準版修正】）--------------------- START
            '追加条件をINIファイルから取得する
            If INI_JYOUKEN.Trim <> "" Then
                SQL.Append(String.Format(" {0}", INI_JYOUKEN))
            End If
            '2017/06/08 タスク）西野 ADD 桑名信金対応（学校徴収金対応【標準版修正】）--------------------- END
            SQL.Append(" ORDER BY FURI_DATE_S ASC,SOUSIN_KBN_S ASC,TORIS_CODE_S ASC,TORIF_CODE_S ASC")

            htTORI_LIST = New Hashtable '2010/12/24 信組対応 企業持込の複数振替日制御

            intSCH_COUNT = 0
            'L = 1
            'P = 1
            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    '2010/12/24 信組対応 企業持込の複数振替日制御 ここから
                    Dim key As String = OraReader.GetString("TORIS_CODE_S") & OraReader.GetString("TORIF_CODE_S")

                    If htTORI_LIST.ContainsKey(key) = False Then
                        '2010/12/24 信組対応 企業持込の複数振替日制御 ここまで

                        strTORI_CODE(intSCH_COUNT) = OraReader.GetString("TORIS_CODE_S") & "-" & OraReader.GetString("TORIF_CODE_S")
                        strFURI_DATE(intSCH_COUNT) = OraReader.GetString("FURI_DATE_S")
                        strFURI_DATE(intSCH_COUNT) = strFURI_DATE(intSCH_COUNT).Substring(0, 4) & "/" & strFURI_DATE(intSCH_COUNT).Substring(4, 2) & "/" & strFURI_DATE(intSCH_COUNT).Substring(6, 2)
                        '2010/12/24 信組対応 配信予定日の項目欄を送信区分に使用する
                        If INI_COMMON_CENTER = "0" Then
                            Select Case OraReader.GetString("SOUSIN_KBN_T")
                                Case "0"
                                    strHAISIN_YDATE(intSCH_COUNT) = "組合持込"
                                Case "1"
                                    strHAISIN_YDATE(intSCH_COUNT) = "企業持込"
                                Case "2"
                                    strHAISIN_YDATE(intSCH_COUNT) = "地公体"
                                Case Else
                                    strHAISIN_YDATE(intSCH_COUNT) = ""
                            End Select
                        Else
                            strHAISIN_YDATE(intSCH_COUNT) = OraReader.GetString("HAISIN_YDATE_S")
                            strHAISIN_YDATE(intSCH_COUNT) = strHAISIN_YDATE(intSCH_COUNT).Substring(0, 4) & "/" & strHAISIN_YDATE(intSCH_COUNT).Substring(4, 2) & "/" & strHAISIN_YDATE(intSCH_COUNT).Substring(6, 2)
                        End If
                        strSOUSIN_KBN(intSCH_COUNT) = OraReader.GetString("SOUSIN_KBN_T")
                        strITAKU_NNAME(intSCH_COUNT) = OraReader.GetString("ITAKU_NNAME_T")
                        strKIGYO_CODE(intSCH_COUNT) = OraReader.GetString("KIGYO_CODE_T")
                        strFURI_CODE(intSCH_COUNT) = OraReader.GetString("FURI_CODE_T")
                        ' 080207 ADD START
                        strTAKOU_KBN(intSCH_COUNT) = "自行"
                        '080207 ADD END

                        ' 2008.01.16 >>
                        strJIKOTAKO(intSCH_COUNT) = "0" ' 自行分
                        ' 2008.01.16 <<

                        intSCH_COUNT += 1

                        '2010/12/24 信組対応 企業持込の複数振替日制御 ここから
                    End If

                    If INI_COMMON_CENTER = "0" Then
                        Select Case OraReader.GetString("SOUSIN_KBN_T")
                            Case "1", "2"
                                '同一取引先で複数振替日があった場合、最初の振替日のみ対象とする
                                If htTORI_LIST.ContainsKey(key) = False Then
                                    htTORI_LIST.Add(key, 0) '2011/01/04 再修正
                                End If
                                htTORI_LIST.Item(key) += 1 '2011/01/04 再修正
                        End Select
                    End If
                    '2010/12/24 信組対応 企業持込の複数振替日制御 ここまで

                    OraReader.NextRead()
                Loop

            End If
            OraReader.Close()


            'If DayCounter = 0 Then
            '    ' 2008.1.16 >>
            '    ' 他行対象データ抽出
            '    Dim OraDB As New CASTCommon.MyOracle(gdbcCONNECT)
            '    Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
            '    Dim SQL As StringBuilder
            '    SQL = New StringBuilder(128)
            '    SQL.Append("SELECT ")
            '    SQL.Append(" TORIS_CODE_S")
            '    SQL.Append(",TORIF_CODE_S")
            '    SQL.Append(",FURI_DATE_S")
            '    SQL.Append(",SOUSIN_KBN_T")
            '    SQL.Append(",ITAKU_NNAME_T")
            '    SQL.Append(",KIGYO_CODE_T")
            '    SQL.Append(",FURI_CODE_T")
            '    ' 080207 ADD START
            '    SQL.Append(",HAISIN_T1FLG_S")
            '    SQL.Append(",HAISIN_T1YDATE_S")
            '    SQL.Append(",HAISIN_T2FLG_S")
            '    SQL.Append(",HAISIN_T2YDATE_S")
            '    ' 080207 ADD END
            '    SQL.Append(" FROM SCHMAST, TORIMAST")
            '    SQL.Append(" WHERE")
            '    SQL.Append("     TOUROKU_FLG_S = '1'")
            '    SQL.Append(" AND (")
            '    SQL.Append("      (HAISIN_T1FLG_S = '0' AND HAISIN_T1YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "')")
            '    SQL.Append("     OR")
            '    SQL.Append("      (HAISIN_T2FLG_S = '0' AND HAISIN_T2YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "')")
            '    SQL.Append("     )")
            '    SQL.Append(" AND YUUKOU_FLG_S = '1'")
            '    SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '    SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '    SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '    SQL.Append(" AND TAKO_KBN_T = '1'")
            '    SQL.Append(" AND FMT_KBN_T <> '20' AND FMT_KBN_T <> '21'")
            '    SQL.Append(" ORDER BY TORIS_CODE_S ASC,TORIF_CODE_S ASC")
            '    If OraReader.DataReader(SQL) = True Then
            '        Do Until OraReader.EOF = True
            '            strTORI_CODE(P, L) = OraReader.GetItem("TORIS_CODE_S") & "-" & OraReader.GetItem("TORIF_CODE_S")
            '            strFURI_DATE(P, L) = OraReader.GetItem("FURI_DATE_S")
            '            strFURI_DATE(P, L) = strFURI_DATE(P, L).Substring(0, 4) & "/" & strFURI_DATE(P, L).Substring(4, 2) & "/" & strFURI_DATE(P, L).Substring(6, 2)
            '            strHAISIN_YDATE(P, L) = KEY_HAISIN_YDATE
            '            strHAISIN_YDATE(P, L) = strHAISIN_YDATE(P, L).Substring(0, 4) & "/" & strHAISIN_YDATE(P, L).Substring(4, 2) & "/" & strHAISIN_YDATE(P, L).Substring(6, 2)
            '            strSOUSIN_KBN(P, L) = OraReader.GetItem("SOUSIN_KBN_T")
            '            strITAKU_NNAME(P, L) = OraReader.GetItem("ITAKU_NNAME_T")
            '            strKIGYO_CODE(P, L) = OraReader.GetItem("KIGYO_CODE_T")
            '            strFURI_CODE(P, L) = OraReader.GetItem("FURI_CODE_T")
            '            ' 080207 ADD START
            '            If OraReader.GetItem("HAISIN_T1FLG_S") = "0" And OraReader.GetItem("HAISIN_T1YDATE_S") = Trim(KEY_HAISIN_YDATE) Then
            '                strTAKOU_KBN(P, L) = "他１"
            '            Else
            '                strTAKOU_KBN(P, L) = "他２"
            '            End If
            '            ' 080207 ADD END

            '            ' 2008.01.16 >>
            '            strJIKOTAKO(P, L) = "1" ' 他行分
            '            ' 2008.01.16 <<

            '            intSCH_COUNT += 1
            '            If L >= 15 Then
            '                L = 1
            '                P = P + 1
            '            Else
            '                L = L + 1
            '            End If

            '            OraReader.NextRead()
            '        Loop
            '    End If
            '    OraReader.Close()
            '    ' 2008.1.16 <<

            '    ' 2008.1.18 >>
            '    ' SSS他行対象データ抽出
            '    OraReader = New CASTCommon.MyOracleReader(OraDB)
            '    SQL = New StringBuilder(128)
            '    SQL.Append("SELECT")
            '    SQL.Append(" TORIS_CODE_S")
            '    SQL.Append(",TORIF_CODE_S")
            '    SQL.Append(",FURI_DATE_S")
            '    SQL.Append(",SOUSIN_KBN_T")
            '    SQL.Append(",ITAKU_NNAME_T")
            '    SQL.Append(",KIGYO_CODE_T")
            '    SQL.Append(",FURI_CODE_T")
            '    ' 080207 ADD START
            '    SQL.Append(",HAISIN_T1FLG_S")
            '    SQL.Append(",HAISIN_T1YDATE_S")
            '    SQL.Append(",HAISIN_T2FLG_S")
            '    SQL.Append(",HAISIN_T2YDATE_S")
            '    SQL.Append(",FMT_KBN_T")
            '    ' 080207 ADD END
            '    SQL.Append(" FROM SCHMAST, TORIMAST")
            '    SQL.Append(" WHERE")
            '    SQL.Append("     TOUROKU_FLG_S = '1'")
            '    SQL.Append(" AND (")
            '    SQL.Append("      (HAISIN_T1FLG_S = '0'")
            '    SQL.Append("       AND HAISIN_T1YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "'")
            '    SQL.Append("       AND FMT_KBN_T IN ('20','21')")
            '    SQL.Append("      )")
            '    SQL.Append("     OR")
            '    SQL.Append("      (HAISIN_T2FLG_S = '0'")
            '    SQL.Append("       AND HAISIN_T2YDATE_S = '" & Trim(KEY_HAISIN_YDATE) & "'")
            '    SQL.Append("       AND FMT_KBN_T = '21'")
            '    SQL.Append("      )")
            '    SQL.Append("     )")
            '    SQL.Append(" AND YUUKOU_FLG_S = '1'")
            '    SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '    SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '    SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '    SQL.Append(" ORDER BY TORIS_CODE_S ASC,TORIF_CODE_S ASC")

            '    If OraReader.DataReader(SQL) = True Then
            '        Do Until OraReader.EOF = True
            '            strTORI_CODE(P, L) = OraReader.GetItem("TORIS_CODE_S") & "-" & OraReader.GetItem("TORIF_CODE_S")
            '            strFURI_DATE(P, L) = OraReader.GetItem("FURI_DATE_S")
            '            strFURI_DATE(P, L) = strFURI_DATE(P, L).Substring(0, 4) & "/" & strFURI_DATE(P, L).Substring(4, 2) & "/" & strFURI_DATE(P, L).Substring(6, 2)
            '            strHAISIN_YDATE(P, L) = KEY_HAISIN_YDATE
            '            strHAISIN_YDATE(P, L) = strHAISIN_YDATE(P, L).Substring(0, 4) & "/" & strHAISIN_YDATE(P, L).Substring(4, 2) & "/" & strHAISIN_YDATE(P, L).Substring(6, 2)
            '            strSOUSIN_KBN(P, L) = OraReader.GetItem("SOUSIN_KBN_T")
            '            strITAKU_NNAME(P, L) = OraReader.GetItem("ITAKU_NNAME_T")
            '            strKIGYO_CODE(P, L) = OraReader.GetItem("KIGYO_CODE_T")
            '            strFURI_CODE(P, L) = OraReader.GetItem("FURI_CODE_T")
            '            ' 080207 ADD START
            '            If OraReader.GetItem("HAISIN_T1FLG_S") = "0" And _
            '               OraReader.GetItem("HAISIN_T1YDATE_S") = Trim(KEY_HAISIN_YDATE) And _
            '               (OraReader.GetItem("FMT_KBN_T") = "20" Or OraReader.GetItem("FMT_KBN_T") = "21") Then
            '                strTAKOU_KBN(P, L) = "提内"
            '            Else
            '                strTAKOU_KBN(P, L) = "提外"
            '            End If

            '            ' 2008.01.16 >>
            '            strJIKOTAKO(P, L) = "2" ' SSS分
            '            ' 2008.01.16 <<

            '            intSCH_COUNT += 1

            '            If L >= 15 Then
            '                L = 1
            '                P = P + 1
            '            Else
            '                L = L + 1
            '            End If

            '            OraReader.NextRead()
            '        Loop
            '    End If
            '    OraReader.Close()

            '    OraDB.Close()
            '    ' 2008.1.18 <<
            'End If

            If intSCH_COUNT = 0 Then
                'gdbcCONNECT.Close()
                If MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning) = DialogResult.OK Then
                    ShowInTaskbar = False
                    Me.Close()
                    Exit Sub
                End If
            End If
            'gdbcCONNECT.Close()


            'intMAXCNT = P
            'intMAXLINE = L - 1
            'If intMAXLINE = 0 Then
            '    intMAXCNT = intMAXCNT - 1
            '    intMAXLINE = 15
            'End If

            With Me.ListView1
                .Clear()
                .Columns.Add("対", 25, HorizontalAlignment.Left)
                .Columns.Add("項番", 40, HorizontalAlignment.Right)
                .Columns.Add("取引先名", 225, HorizontalAlignment.Left)
                .Columns.Add("取引先ｺｰﾄﾞ", 100, HorizontalAlignment.Center)
                .Columns.Add("振替日", 80, HorizontalAlignment.Center)
                .Columns.Add("振替ｺｰﾄﾞ", 64, HorizontalAlignment.Center)
                .Columns.Add("企業ｺｰﾄﾞ", 64, HorizontalAlignment.Center)
                '2010/12/24 信組対応 配信予定日の項目欄を送信区分に使用する
                If INI_COMMON_CENTER = "0" Then
                    .Columns.Add("送信区分", 80, HorizontalAlignment.Center)
                Else
                    .Columns.Add("配信予定日", 80, HorizontalAlignment.Center)
                End If
                .Columns.Add("件数", 92, HorizontalAlignment.Right)
                .Columns.Add("金額", 0, HorizontalAlignment.Right)
                '   .Columns.Add("配信先", 0, HorizontalAlignment.Center)
                .CheckBoxes = True
            End With

            ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- START
            'KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- END

            '2007/10/18
            'If gdbcCONNECT.State = ConnectionState.Closed Then
            '    gdbcCONNECT.ConnectionString = gstrDB_CONNECT
            '    gdbcCONNECT.Open()
            'End If

            mTakouDensoList = GetTakouList(True)
            mTakouDensoGaiList = GetTakouList(False)

            Dim LineColor As Color
            Dim CharColir As Color  '2009/11/30 追加(文字色)
            Dim ROW As Integer = 0
            For PG As Integer = 0 To intSCH_COUNT - 1 Step 1

                'Dim MaxLines As Integer
                'If PG = intMAXCNT Then

                '    MaxLines = intMAXLINE
                'Else
                '    MaxLines = 15
                'End If

                'For LN As Integer = 1 To MaxLines Step 1

                ' 処理対象件数 を カウントする
                Dim TaisyouKensu() As Decimal = GetTaisyouKensu(strTORI_CODE(PG), strFURI_DATE(PG), strTAKOU_KBN(PG), OraDB)

                strKENSU(PG) = TaisyouKensu(0).ToString("#,##0")

                '2010/12/24 信組対応 送信区分追加
                Dim Data() As String = New String(11) {strJIKOTAKO(PG), _
                    (ROW + 1).ToString, strITAKU_NNAME(PG).TrimEnd, strTORI_CODE(PG), _
                    strFURI_DATE(PG), strFURI_CODE(PG), strKIGYO_CODE(PG), _
                    strHAISIN_YDATE(PG), TaisyouKensu(0).ToString("#,##0"), TaisyouKensu(1).ToString("#,##0"), strTAKOU_KBN(PG), strSOUSIN_KBN(PG)}

                If ROW Mod 2 = 0 Then
                    LineColor = Color.White
                Else
                    LineColor = Color.WhiteSmoke
                End If

                '2009/11/30 翌営業日=振替日の場合赤文字化
                If NextData = strFURI_DATE(PG).Replace("/", "") Then
                    CharColir = Color.Red
                Else
                    CharColir = Color.Black
                End If
                'Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                Dim vLstItem As New ListViewItem(Data, -1, CharColir, LineColor, Nothing)
                '========================================
                ListView1.Items.AddRange(New ListViewItem() {vLstItem})
                vLstItem.Checked = True

                ROW += 1
                'Next LN
            Next PG

            'gdbcCONNECT.Close()

            'intPAGE_CNT = 1
            'txtPage.Text = Format(intPAGE_CNT, "00") & "/" & Format(intMAXCNT, "00")
            'If intMAXCNT = 0 Then
            '    btnPreGamen.Enabled = False
            '    btnNextGamen.Enabled = False
            'End If

            '2005/03/28 大阪センターの場合サイクル管理をせず、翌日振替の場合、翌々日以降振替でファイル名を管理する
            '2011/01/27 信組対応 信組の場合もサイクル管理しない
            If INI_COMMON_CENTER = "5" OrElse INI_COMMON_CENTER = "0" Then
                txtCycle.Text = "1"
                Label25.Enabled = False
                txtCycle.Enabled = False
            End If

            '蒲郡信金向け サイクル管理マスタからサイクル番号取得 2007/10/18
            'gstrSSQL = "SELECT MAX(CYCLE_NO_F) FROM FILE_CYCLE"
            'gstrSSQL = gstrSSQL & " WHERE SYORI_DATE_F = '" & Format(CASTCommon.Calendar.Now, "yyyyMMdd") & "'"
            'gstrSSQL = gstrSSQL & " ORDER BY CYCLE_NO_F DESC"

            '2007/10/18
            'If gdbcCONNECT.State = ConnectionState.Closed Then
            '    gdbcCONNECT.ConnectionString = gstrDB_CONNECT
            '    gdbcCONNECT.Open()
            'End If

            'gdbCOMMAND = New OracleClient.OracleCommand
            'gdbCOMMAND.CommandText = gstrSSQL
            'gdbCOMMAND.Connection = gdbcCONNECT

            'gdbrREADER = gdbCOMMAND.ExecuteReader '読込のみ
            'If gdbrREADER.Read = False Then
            '    txtSaikuru.Text = "1"
            'Else
            '    If CInt(clsFUSION.fn_chenge_null(gdbrREADER.Item(0), 0)) = 9 Then
            '        txtSaikuru.Text = "1"
            '    Else
            '        txtSaikuru.Text = CStr(CInt(clsFUSION.fn_chenge_null(gdbrREADER.Item(0), 0)) + 1)
            '    End If
            'End If

            'gdbcCONNECT.Close()

            Me.Visible = True

            txtCycle.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not OraDB Is Nothing Then OraDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region "終了ボタン"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region "印刷ボタン"
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        '============================================================================
        'NAME           :btnPrint_Click
        'Parameter      :
        'Description    :印刷ボタン押下時の処理
        'Return         :
        'Create         :2004/08/13
        'Update         :
        '============================================================================

        Dim mRet As Integer
        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFJP009


        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", "未検索")
                Return
            End If

            mRet = MessageBox.Show(MSG0013I.Replace("{0}", "センターカットデータ作成対象一覧"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If mRet <> DialogResult.OK Then
                Return
            End If

            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems

                CreateCSV.OutputCsvData(item.SubItems(1).Text)        '項番
                CreateCSV.OutputCsvData(SyoriDate)        '処理日
                CreateCSV.OutputCsvData(SyoriTime)        'タイムスタンプ
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(0, 10))        '取引先主コード
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(11, 2))        '取引先副コード
                CreateCSV.OutputCsvData(item.SubItems(2).Text)        '取引先名（漢字）
                CreateCSV.OutputCsvData(item.SubItems(5).Text)        '振替コード
                CreateCSV.OutputCsvData(item.SubItems(6).Text)        '企業コード
                '2010/12/24 信組対応 配信予定日の項目欄を送信区分に使用する
                If INI_COMMON_CENTER = "0" Then
                    CreateCSV.OutputCsvData(item.SubItems(7).Text)                     '送信区分
                Else
                    CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""))    '配信予定日
                End If
                CreateCSV.OutputCsvData(item.SubItems(4).Text.Replace("/", ""))        '振替日
                CreateCSV.OutputCsvData(item.SubItems(8).Text.Replace(",", ""))        '処理件数
                CreateCSV.OutputCsvData(item.SubItems(9).Text.Replace(",", ""))        '処理金額
                CreateCSV.OutputCsvData(item.SubItems(10).Text, False, True)        '配信先

            Next
            CreateCSV.CloseCsv()

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            param = GCom.GetUserID & "," & strCSVFileName
            iRet = ExeRepo.ExecReport("KFJP009.EXE", param)

            If iRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case iRet
                    Case -1
                        errMsg = MSG0226W.Replace("{0}", "センターカットデータ作成対象一覧")
                        MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        errMsg = MSG0004E.Replace("{0}", "センターカットデータ作成対象一覧")
                        MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Select

               MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "センターカットデータ作成対象一覧印刷", "失敗")
                Return
            Else
                '2009/12/17 印刷完了メッセージ追加
                MessageBox.Show(String.Format(MSG0014I, "センターカットデータ作成対象一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                '===============================
            End If

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
        End Try

    End Sub

#End Region

#Region "実行ボタン"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :作成ボタン押下時の処理
        'Return         :
        'Create         :2004/08/16
        'Update         :
        '=====================================================================================


        Dim MainDB As New CASTCommon.MyOracle()
        Dim SQL As StringBuilder
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)開始", "成功", "")

            '2005/03/28 大阪センターの場合サイクル管理をせず、翌日振替の場合、翌々日以降振替で
            'ファイル名を管理する
            If INI_COMMON_CENTER <> "5" Then
                '2011/01/27 信組対応 信組の場合もサイクル管理しない ここから
                If INI_COMMON_CENTER = "0" Then
                    If MessageBox.Show(MSG0051I.Replace("サイクル番号が{0}です", "センターカットデータを作成します"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                        txtCycle.Focus()
                        Exit Sub
                    End If
                Else
                    '2011/01/27 信組対応 信組の場合もサイクル管理しない ここまで

                    '------------------------------
                    'サイクル番号のチェック
                    '------------------------------
                    If txtCycle.Text.Length <> 0 Then
                        Select Case Integer.Parse(txtCycle.Text)
                            Case 1 To 9
                            Case Else
                                MessageBox.Show(MSG0329W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                txtCycle.Focus()
                                Exit Sub
                        End Select
                    Else
                        MessageBox.Show(MSG0330W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtCycle.Focus()
                        Exit Sub
                    End If

                    If MessageBox.Show(MSG0051I.Replace("{0}", txtCycle.Text), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                        txtCycle.Focus()
                        Exit Sub
                    End If
                End If '2011/01/27 信組対応 If閉じる

            Else    '大阪センター
                '2010.02.16 start

                '現在時刻の取得
                Dim strTime As String = CASTCommon.Calendar.Now.ToString("HHmm")
                Dim strFile As String
                Dim strErr As String = "err"

                ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
                ''2010.02.19 境界時間をiniより取得 start
                'Dim strBorder As String = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
                'If strBorder = "err" OrElse strBorder = "" Then
                '    strBorder = "1200"
                'End If
                ''2010.02.19 境界時間をiniより取得 end
                ' 2016/06/11 タスク）綾部 DEL 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

                ' 2016/06/11 タスク）綾部 CHG 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
                'If strTime < strBorder Then
                '    'INIファイル情報の取得
                '    '[JIFURI]FILEAM
                '    strFile = CASTCommon.GetFSKJIni("JIFURI", "FILEAM")
                '    If strFile = strErr Then
                '        MessageBox.Show(String.Format(MSG0001E, "センターカットファイル名", "JIFURI", "FILEAM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '        txtCycle.Focus()
                '        Return
                '    End If
                'Else
                '    'INIファイル情報の取得
                '    '[JIFURI]FILEPM
                '    strFile = CASTCommon.GetFSKJIni("JIFURI", "FILEPM")
                '    If strFile = strErr Then
                '        MessageBox.Show(String.Format(MSG0001E, "センターカットファイル名", "JIFURI", "FILEPM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '        txtCycle.Focus()
                '        Return
                '    End If
                'End If
                If strTime < INI_JIFURI_BORDER_TIME Then
                    strFile = INI_JIFURI_FILEAM
                Else
                    strFile = INI_JIFURI_FILEPM
                End If
                ' 2016/06/11 タスク）綾部 CHG 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

                If MessageBox.Show(String.Format(MSG0071I, strFile), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                    txtCycle.Focus()
                    Return
                End If
                '2010.02.16 end
            End If


            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '******************************************
            ' チェックボックスのチェック
            '******************************************
            'リストに１件も表示されていないとき
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", "未検索")
                Return
            Else
                'リストに１件以上表示されているが、チェックされていないとき
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", "未選択")
                    Return
                End If
            End If

            '------------------------------
            '作成確認メッセージ
            '------------------------------
            Dim sMessage As String
            sMessage = MSG0023I.Replace("{0}", "センターカットデータ作成") & vbCrLf
            Dim sJikoFuriDate As String = ""

            ' 2016/06/11 タスク）綾部 CHG 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- START
            '2010/09/09.Sakon　件数を取得、6万件以上でメッセージ出力 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'Dim GoukeiKensuu As Integer
            'Dim KiteiKensuu As String
            'For Each item As ListViewItem In nSelectItems
            '    '2010/12/24 信組対応 件数カウントは送信区分=0のみ
            '    'If item.SubItems(0).Text = "0" AndAlso item.SubItems(8).Text <> "" Then
            '    If item.SubItems(0).Text = "0" AndAlso item.SubItems(8).Text <> "" AndAlso item.SubItems(11).Text = "0" Then
            '        GoukeiKensuu += CInt(item.SubItems(8).Text)
            '    End If
            'Next
            'KiteiKensuu = CASTCommon.GetFSKJIni("JIFURI", "KITEIKEN")
            'If KiteiKensuu = "err" Then
            '    MessageBox.Show("ＩＮＩファイル取得失敗" & vbCrLf & "センターカットデータ送信規定件数が未設定です。", _
            '                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Else
            '    If GoukeiKensuu >= KiteiKensuu Then
            '        If MessageBox.Show("合計送信件数が規定値を超過しています。" & vbCrLf & _
            '                           "規定値ごとにファイルを分割して処理を続行しますか？" & vbCrLf & _
            '                           "規定件数：" & KiteiKensuu & " 件" & vbCrLf & _
            '                           "処理件数：" & GoukeiKensuu & " 件", msgTitle, _
            '                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
            '            Return
            '        End If
            '    End If
            'End If
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '-------------------------------------------------------
            ' センターカット送信件数カウント（送信区分=0 のみ）
            '-------------------------------------------------------
            Dim GoukeiKensuu As Integer
            For Each item As ListViewItem In nSelectItems
                If item.SubItems(0).Text = "0" AndAlso item.SubItems(8).Text <> "" AndAlso item.SubItems(11).Text = "0" Then
                    GoukeiKensuu += CInt(item.SubItems(8).Text)
                End If
            Next

            '-------------------------------------------------------
            ' センターカット送信件数チェック
            '-------------------------------------------------------
            If GoukeiKensuu >= CInt(INI_JIFURI_KITEIKEN.trim) Then
                If MessageBox.Show("合計送信件数が規定値を超過しています。" & vbCrLf & _
                                   "規定値ごとにファイルを分割して処理を続行しますか？" & vbCrLf & _
                                   "規定件数：" & CInt(INI_JIFURI_KITEIKEN.trim) & " 件" & vbCrLf & _
                                   "処理件数：" & GoukeiKensuu & " 件", msgTitle, _
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                    Return
                End If
            End If
            ' 2016/06/11 タスク）綾部 CHG 【PG】UI-03-02(RSV2対応<小浜信金>) -------------------- END

            For Each item As ListViewItem In nSelectItems
                If item.SubItems(0).Text = "0" Then
                    sJikoFuriDate = item.SubItems(4).Text.Replace("/", "")
                    Exit For
                End If
            Next
            If sJikoFuriDate <> "" Then
                sMessage &= "【自行】" & vbCrLf
                sMessage &= "振替日：" & sJikoFuriDate & vbCrLf
                sMessage &= "配信日：" & Format(System.DateTime.Today, "yyyy/MM/dd")
                sMessage &= vbCrLf & vbCrLf
            End If

            'For ii As Integer = 1 To intMAXCNT
            '    For jj As Integer = 1 To 15
            '        If strJIKOTAKO(ii, jj) = "0" Then
            '            sJikoFuriDate = strFURI_DATE(ii, jj)
            '            Exit For
            '        End If
            '    Next jj
            'Next ii
            'If sJikoFuriDate <> "" Then
            '    sMessage &= "【自行】" & vbCrLf
            '    sMessage &= "振替日：" & sJikoFuriDate & vbCrLf
            '    sMessage &= "配信日：" & Format(System.DateTime.Today, "yyyy/MM/dd")
            '    sMessage &= vbCrLf & vbCrLf
            'End If

            'Dim sTakoFuriDate As String = ""
            'For ii As Integer = 1 To intMAXCNT
            '    For jj As Integer = 1 To 15
            '        If strJIKOTAKO(ii, jj) = "1" Then
            '            sTakoFuriDate = strFURI_DATE(ii, jj)
            '            Exit For
            '        End If
            '    Next jj
            'Next ii
            'If sTakoFuriDate <> "" Then
            '    sMessage &= "【他行】" & vbCrLf
            '    sMessage &= "振替日：" & sTakoFuriDate & vbCrLf
            '    sMessage &= "配信日：" & Format(System.DateTime.Today, "yyyy/MM/dd")
            '    sMessage &= vbCrLf & vbCrLf
            'End If

            'Dim sSSSFuriDate As String = ""
            'For ii As Integer = 1 To intMAXCNT
            '    For jj As Integer = 1 To 15
            '        If strJIKOTAKO(ii, jj) = "2" Then
            '            sSSSFuriDate = strFURI_DATE(ii, jj)
            '            Exit For
            '        End If
            '    Next jj
            'Next ii
            'If sSSSFuriDate <> "" Then
            '    sMessage &= "【SSS】" & vbCrLf
            '    sMessage &= "振替日：" & sSSSFuriDate & vbCrLf
            '    sMessage &= "配信日：" & Format(System.DateTime.Today, "yyyy/MM/dd")
            '    sMessage &= vbCrLf & vbCrLf
            'End If
            'If MessageBox.Show(sMessage, "センターカットデータ作成", MessageBoxButtons.YesNo) = DialogResult.No Then
            '    Exit Sub
            'End If

            MainDB.BeginTrans()
            '********************************************
            ' スケジュールマスタの更新
            '********************************************
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems
                SQL = New StringBuilder(128)
                If item.SubItems(0).Text = "0" Then

                    If item.Checked = True Then
                        SQL.Append("UPDATE SCHMAST SET HAISIN_FLG_S = '2' ")
                    Else
                        SQL.Append("UPDATE SCHMAST SET HAISIN_FLG_S = '0' ")
                    End If

                    SQL.Append(" WHERE TORIS_CODE_S = '" & item.SubItems(3).Text.Substring(0, 10) & "' ")
                    SQL.Append(" AND TORIF_CODE_S = '" & item.SubItems(3).Text.Substring(11, 2) & "' ")
                    SQL.Append(" AND FURI_DATE_S = '" & item.SubItems(4).Text.Replace("/", "") & "' ")
                    Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                    If nRet = 0 Then
                        MainDB.Rollback()
                        Return
                    ElseIf nRet < 0 Then
                        Throw New Exception(String.Format(MSG0027E, "スケジュールマスタ", "更新"))
                    End If
                End If
            Next

            '2010/12/24 信組対応 送信区分毎にジョブ登録するようロジック変更 ここから
            Dim JikoFlg As Boolean = False
            Dim SounsinKbn As New ArrayList
            '送信区分をリストに追加
            For Each item As ListViewItem In nSelectItems
                If item.SubItems(0).Text <> "0" Then
                    Exit For
                End If

                If SounsinKbn.Contains(item.SubItems(11).Text) = False Then
                    SounsinKbn.Add(item.SubItems(11).Text)
                End If
            Next
            SounsinKbn.Sort() '値順にソート

            For i As Integer = 0 To SounsinKbn.Count - 1
                '2010/12/24 信組対応 送信区分毎にジョブ登録するようロジック変更 ここまで
                '------------------------------
                'JOBMASTに登録
                '------------------------------
                Dim strJOBID As String, strPARAMETA As String

                Dim minFuriDate As String = "99999999"
                Dim maxFuriDate As String = ""

                For Each item As ListViewItem In nSelectItems
                    If item.SubItems(0).Text <> "0" Then
                        Exit For
                    End If

                    ' 最小値

                    If String.Compare(minFuriDate, item.SubItems(4).Text.Replace("/", "")) >= 0 Then
                        minFuriDate = item.SubItems(4).Text.Replace("/", "")
                    End If

                    ' 最大値
                    If String.Compare(maxFuriDate, item.SubItems(4).Text.Replace("/", "")) <= 0 Then
                        maxFuriDate = item.SubItems(4).Text.Replace("/", "")
                    End If

                Next
                'For i = 1 To intMAXCNT
                '    For j = 1 To 15
                '        If strJIKOTAKO(i, j) <> "0" Then
                '            ' 自行以外は，処理を抜ける
                '            Exit For
                '        End If

                '        ' 最小値
                '        If String.Compare(minFuriDate, strFURI_DATE(i, j)) >= 0 Then
                '            minFuriDate = strFURI_DATE(i, j)
                '        End If

                '        ' 最大値
                '        If String.Compare(maxFuriDate, strFURI_DATE(i, j)) <= 0 Then
                '            maxFuriDate = strFURI_DATE(i, j)
                '        End If
                '    Next j
                'Next i

                Dim iRet As Integer
                '2010/12/24 信組対応 コメントアウト
                'Dim JikoFlg As Boolean = False

                If minFuriDate <> "99999999" AndAlso maxFuriDate <> "" Then
                    strJOBID = "J030"
                    strPARAMETA = minFuriDate.Replace("/", "")
                    strPARAMETA &= "," & maxFuriDate.Replace("/", "")
                    '2010/12/24 信組対応 正しい送信区分をパラメータ設定する
                    'strPARAMETA &= ",0," & txtSaikuru.Text
                    strPARAMETA &= "," & SounsinKbn(i) & "," & txtCycle.Text

                    iRet = MainLOG.SearchJOBMAST(strJOBID, strPARAMETA, MainDB)
                    If iRet = 1 Then
                        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        Return
                    ElseIf iRet = -1 Then
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainDB.Rollback()
                        Return
                    End If
                    If MainLOG.InsertJOBMAST(strJOBID, LW.UserID, strPARAMETA, MainDB) = False Then
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainDB.Rollback()
                        Return
                    End If

                    JikoFlg = True
                    'If clsFUSION.fn_INSERT_JOBMAST(strJOBID, gstrUSER_ID, strPARAMETA) = False Then
                    '    MessageBox.Show("センターカットデータ作成処理の起動に失敗しました。" & "エラー：" & Err.Description)
                    '    '*** 2008/6/24 kaki ************************************************
                    '    '*** 画面表示桁MAX(15)の場合jが16となり、ログ出力がエラーとなる
                    '    If j = 16 Then
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "起動パラメタの登録", "失敗", "送信区分：" & strSOUSIN_KBN(i - 1, j - 1) & " " & "サイクル番号：" & txtSaikuru.Text & "　" & Err.Description)
                    '    Else
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "起動パラメタの登録", "失敗", "送信区分：" & strSOUSIN_KBN(i, j) & " " & "サイクル番号：" & txtSaikuru.Text & "　" & Err.Description)
                    '    End If
                    '    '*** 2008/6/24 kaki ************************************************
                    '    Exit Sub
                    'Else
                    '    '*** 2008/6/24 kaki ************************************************
                    '    '*** 画面表示桁MAX(15)の場合jが16となり、ログ出力がエラーとなる
                    '    If j = 16 Then
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "起動パラメタの登録", "成功", "送信区分：" & strSOUSIN_KBN(i - 1, j - 1) & " " & "サイクル番号：" & txtSaikuru.Text & "　" & Err.Description)
                    '    Else
                    '        fn_LOG_WRITE(gstrUSER_ID, "", minFuriDate, gstrSYORI_R, "起動パラメタの登録", "成功", "送信区分：" & strSOUSIN_KBN(i, j) & " " & "サイクル番号：" & txtSaikuru.Text & "　" & Err.Description)
                    '    End If
                    '    '*** 2008/6/24 kaki ************************************************
                    'End If
                End If

            Next '2010/12/24 信組対応 ForEach閉じる

            ' 2008.01.16 >>
            Dim TakoFlag As Boolean = False
            Dim SSSFlag As Boolean = False

            MainDB.Commit()

            sMessage = MSG0021I.Replace("{0}", "センターカットデータ作成")
            sMessage &= vbCrLf
            If JikoFlg = True Then
                'sMessage &= vbCrLf & "  【センターカットデータ作成処理】"
                'MessageBox.Show("センターカットデータ作成処理の" & vbCrLf & "起動パラメータを登録しました", "センターカットデータ作成")
            End If
            If TakoFlag = True Then
                'sMessage &= vbCrLf & "  【他行データ作成処理】"
                'MessageBox.Show("他行データ作成処理の" & vbCrLf & "起動パラメータを登録しました", "他行データ作成")
            End If
            If SSSFlag = True Then
                'sMessage &= vbCrLf & "  【ＳＳＳデータ作成処理】"
                'MessageBox.Show("他行データ作成処理の" & vbCrLf & "起動パラメータを登録しました", "他行データ作成")
            End If
            '2010/12/24 信組対応 企業持込の複数振替日制御 ここから
            If htTORI_LIST.Count > 0 Then
                '2011/01/04 再修正 ここから
                For Each i As Integer In htTORI_LIST.Values
                    If i > 1 Then
                        sMessage &= vbCrLf
                        sMessage &= "※同一取引先で複数振替日の対象が存在するため、最小の振替日のみデータ作成します。"

                        Exit For
                    End If
                Next
                '2011/01/04 再修正 ここまで
            End If
            '2010/12/24 信組対応 企業持込の複数振替日制御 ここまで

            MessageBox.Show(sMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            btnAction.Enabled = False
            ' 2008.01.16 <<

        Catch ex As Exception
            MainDB.Rollback()
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)終了", "成功", "")
        End Try

    End Sub
#End Region

    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick

        Try

            With CType(sender, ListView)

                If ClickedColumn = e.Column Then
                    ' 同じ列をクリックした場合は，逆順にする 
                    SortOrderFlag = Not SortOrderFlag
                End If

                ' 列番号設定
                ClickedColumn = e.Column

                ' 列水平方向配置
                Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

                ' ソート
                .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

                ' ソート実行
                .Sort()

            End With
        Catch ex As Exception

        End Try
    End Sub

    Private Function GetTaisyouKensu(ByVal toriCode As String, ByVal furiData As String, ByVal takouKubun As String, ByVal db As CASTCommon.MyOracle) As Decimal()
        Dim SQL As New StringBuilder(128)

        Dim OraReader As New CASTCommon.MyOracleReader(db)

        Try
            SQL.Append("SELECT COUNT(*) KENSU,SUM(FURIKIN_K) AS KIN FROM MEIMAST")
            SQL.Append(" WHERE TORIS_CODE_K = '" & toriCode.Split("-")(0) & "'")
            SQL.Append("   AND TORIF_CODE_K = '" & toriCode.Split("-")(1) & "'")
            SQL.Append("   AND FURI_DATE_K  = '" & furiData.Replace("/", "") & "'")
            SQL.Append("   AND FURIKETU_CODE_K  = '0'")
            Select Case takouKubun
                Case "自行"
                    SQL.Append("   AND KEIYAKU_KIN_K  = '" & INI_COMMON_KINKOCD & "'")
                Case "他１"
                    '
                    If mTakouDensoList.Length > 0 Then
                        SQL.Append("   AND KEIYAKU_KIN_K  IN (" & String.Join(",", mTakouDensoList) & ")")
                    End If
                Case "他２"
                    '
                    If mTakouDensoList.Length > 0 Then
                        SQL.Append("   AND KEIYAKU_KIN_K  IN (" & String.Join(",", mTakouDensoGaiList) & ")")
                    End If
                Case "提内"
                    SQL.Append("   AND KEIYAKU_KIN_K  <> '" & INI_COMMON_KINKOCD & "'")
                    SQL.Append("   AND EXISTS (SELECT KIN_NO_N FROM TENMAST")
                    SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
                    SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
                    SQL.Append("      AND TEIKEI_KBN_N = '1')")
                Case "提外"
                    SQL.Append("   AND KEIYAKU_KIN_K  <> '" & INI_COMMON_KINKOCD & "'")
                    SQL.Append("   AND EXISTS (SELECT KIN_NO_N FROM TENMAST")
                    SQL.Append("    WHERE KIN_NO_N = KEIYAKU_KIN_K")
                    SQL.Append("      AND SIT_NO_N = KEIYAKU_SIT_K")
                    SQL.Append("      AND TEIKEI_KBN_N = '0')")
                Case Else
                    SQL.Append("   AND KEIYAKU_KIN_K  = '" & INI_COMMON_KINKOCD & "'")
            End Select

            Static ss As Integer = 1

            If OraReader.DataReader(SQL) = True Then
                Return New Decimal(1) {OraReader.GetInt64("KENSU"), OraReader.GetInt64("KIN")}
            Else
                Return New Decimal(1) {0, 0}
            End If
        Catch ex As Exception
            Throw
        Finally
            OraReader.Close()
        End Try

        Return New Decimal(1) {0, 0}
    End Function

    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- START
    Private Function GetIniInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "開始", "")

            '-------------------------------------------------------
            ' 伝送フォルダ [COMMON]-[DEN]
            '-------------------------------------------------------
            INI_COMMON_DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If INI_COMMON_DEN.ToUpper = "ERR" OrElse INI_COMMON_DEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "伝送フォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "伝送フォルダ [COMMON]-[DEN]")
                Return False
            End If

            '-------------------------------------------------------
            ' 指定センター [COMMON]-[CENTER]
            '-------------------------------------------------------
            INI_COMMON_CENTER = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If INI_COMMON_CENTER.ToUpper = "ERR" OrElse INI_COMMON_CENTER = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "指定センター", "COMMON", "CENTER"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "指定センター [COMMON]-[CENTER]")
                Return False
            End If

            '-------------------------------------------------------
            ' 自金庫コード [COMMON]-[KINKOCD]
            '-------------------------------------------------------
            INI_COMMON_KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If INI_COMMON_KINKOCD.ToUpper = "ERR" OrElse INI_COMMON_KINKOCD = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "自金庫コード [COMMON]-[KINKOCD]")
                Return False
            End If

            '-------------------------------------------------------
            ' ＣＣデータファイル名(ＡＭ) [JIFURI]-[FILEAM]
            '-------------------------------------------------------
            INI_JIFURI_FILEAM = CASTCommon.GetFSKJIni("JIFURI", "FILEAM")
            If INI_JIFURI_FILEAM.ToUpper = "ERR" OrElse INI_JIFURI_FILEAM = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "センターカットファイル名", "JIFURI", "FILEAM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "ＣＣデータファイル名(ＡＭ) [JIFURI]-[FILEAM]")
                Return False
            End If

            '-------------------------------------------------------
            ' ＣＣデータファイル名(ＰＭ) [JIFURI]-[FILEPM]
            '-------------------------------------------------------
            INI_JIFURI_FILEPM = CASTCommon.GetFSKJIni("JIFURI", "FILEPM")
            If INI_JIFURI_FILEPM.ToUpper = "ERR" OrElse INI_JIFURI_FILEPM = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "センターカットファイル名", "JIFURI", "FILEPM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "ＣＣデータファイル名(ＰＭ) [JIFURI]-[FILEPM]")
                Return False
            End If

            '-------------------------------------------------------
            ' センターカットデータ送信規定件数 [JIFURI]-[KITEIKEN]
            '-------------------------------------------------------
            INI_JIFURI_KITEIKEN = CASTCommon.GetFSKJIni("JIFURI", "KITEIKEN")
            If INI_JIFURI_KITEIKEN.ToUpper = "ERR" OrElse INI_JIFURI_KITEIKEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "センターカットデータ送信規定件数", "JIFURI", "KITEIKEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "センターカットデータ送信規定件数 [JIFURI]-[KITEIKEN]")
                Return False
            End If

            '-------------------------------------------------------
            ' ＣＣ境界時間 [JIFURI]-[BORDER_TIME]
            '-------------------------------------------------------
            INI_JIFURI_BORDER_TIME = CASTCommon.GetFSKJIni("JIFURI", "BORDER_TIME")
            If INI_JIFURI_BORDER_TIME.ToUpper = "ERR" OrElse INI_JIFURI_BORDER_TIME = Nothing OrElse INI_JIFURI_BORDER_TIME = "" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "ＣＣ境界時間 [JIFURI]-[BORDER_TIME]")
                INI_JIFURI_BORDER_TIME = "1200"
            End If

            '-------------------------------------------------------
            ' ＣＣデータ存在チェック [JIFURI]-[CCDATACHK]
            '-------------------------------------------------------
            INI_JIFURI_CCDATACHK = CASTCommon.GetFSKJIni("JIFURI", "CCDATACHK")
            If INI_JIFURI_CCDATACHK.ToUpper = "ERR" OrElse INI_JIFURI_CCDATACHK = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "ＣＣデータ存在チェック [JIFURI]-[CCDATACHK]")
            End If

            '-------------------------------------------------------
            ' ＣＣ境界時間チェック [JIFURI]-[BORDERCHK]
            '-------------------------------------------------------
            INI_JIFURI_BORDERCHK = CASTCommon.GetFSKJIni("JIFURI", "BORDERCHK")
            If INI_JIFURI_BORDERCHK.ToUpper = "ERR" OrElse INI_JIFURI_BORDERCHK = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "ＣＣ境界時間チェック [JIFURI]-[BORDERCHK]")
            End If

            '-------------------------------------------------------
            ' エントリ未落込チェック [JIFURI]-[ENTRYCHK]
            '-------------------------------------------------------
            INI_JIFURI_ENTRYCHK = CASTCommon.GetFSKJIni("JIFURI", "ENTRYCHK")
            If INI_JIFURI_ENTRYCHK.ToUpper = "ERR" OrElse INI_JIFURI_ENTRYCHK = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "エントリ未落込チェック [JIFURI]-[ENTRYCHK]")
            End If

            '-------------------------------------------------------
            ' センターカットファイル名設定 [JIFURI]-[CCFNAME]
            '-------------------------------------------------------
            INI_JIFURI_CCFNAME = CASTCommon.GetFSKJIni("JIFURI", "CCFNAME")
            If INI_JIFURI_CCFNAME.ToUpper = "ERR" OrElse INI_JIFURI_CCFNAME = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "センターカットファイル名設定 [JIFURI]-[CCFNAME]")
            End If

            '2017/06/08 タスク）西野 ADD 桑名信金対応（学校徴収金対応【標準版修正】）--------------------- START
            '-------------------------------------------------------
            ' センターカット対象条件追加 [FORM]-[KFJMAIN030_JYOUKEN]
            '-------------------------------------------------------
            INI_JYOUKEN = CASTCommon.GetRSKJIni("FORM", "KFJMAIN030_JYOUKEN")
            If INI_JYOUKEN.ToUpper = "ERR" OrElse INI_JYOUKEN = Nothing Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", "センターカット対象条件追加 [FORM]-[KFJMAIN030_JYOUKEN]")
                INI_JYOUKEN = ""
            End If
            '2017/06/08 タスク）西野 ADD 桑名信金対応（学校徴収金対応【標準版修正】）--------------------- END

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＩＮＩ情報取得", "終了", "")
        End Try

    End Function

    Private Function BorderCheck(ByVal CheckFuriDate As String, ByRef ReturnMessage As String) As Boolean

        Dim FunctionTitle As String = "ＣＣ境界時間チェック"

        Dim SQL As StringBuilder = Nothing
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "開始", "")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")
            SQL.Append("   , ITAKU_NNAME_T")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append("   , SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FSYORI_KBN_S   = '1'")
            SQL.Append(" AND TORIS_CODE_T   = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T   = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S    = '" & CheckFuriDate & "'")
            SQL.Append(" AND SOUSIN_KBN_S   = '0'")
            SQL.Append(" AND TOUROKU_FLG_S  = '1'")
            SQL.Append(" AND HAISIN_FLG_S   = '0'")
            SQL.Append(" AND FUNOU_FLG_S    = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S  = '0'")
            SQL.Append(" ORDER BY")
            SQL.Append("     SOUSIN_KBN_S")
            SQL.Append("   , TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim FirstFlg As Integer = 1

            If OraReader.DataReader(SQL) = True Then
                Do While OraReader.EOF = False
                    If FirstFlg = 1 Then
                        Dim MsgFuriDate As String = CheckFuriDate.Substring(0, 4) & "年" & _
                                       CheckFuriDate.Substring(4, 2) & "月" & _
                                       CheckFuriDate.Substring(6, 2) & "日"
                        ReturnMessage = "振替日:" & MsgFuriDate & "の振替データで送信できないものがあります。" & vbCrLf
                        FirstFlg = 0
                    End If

                    ReturnMessage += vbCrLf & _
                                                  "取引先コード:" & OraReader.GetString("TORIS_CODE_S") & "-" & OraReader.GetString("TORIF_CODE_S") & _
                                                  " 委託者名:" & OraReader.GetString("ITAKU_NNAME_T").Trim
                    OraReader.NextRead()
                Loop
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "終了", "")
        End Try

    End Function

    Private Function EntryCheck(ByRef ReturnMessage As String) As Boolean

        Dim FunctionTitle As String = "エントリ未落込チェック"

        Dim SQL As StringBuilder = Nothing
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "開始", "")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")
            SQL.Append("   , FURI_DATE_S")
            SQL.Append("   , ITAKU_NNAME_T")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append("   , SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FSYORI_KBN_S   = '1'")
            SQL.Append(" AND TORIS_CODE_T   = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T   = TORIF_CODE_S")
            SQL.Append(" AND BAITAI_CODE_T IN ('04', '09')")
            SQL.Append(" AND UKETUKE_FLG_S  = '1'")
            SQL.Append(" AND TOUROKU_FLG_S  = '0'")
            SQL.Append(" AND HAISIN_FLG_S   = '0'")
            SQL.Append(" AND FUNOU_FLG_S    = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S  = '0'")
            SQL.Append(" ORDER BY")
            SQL.Append("     FURI_DATE_S")
            SQL.Append("   , TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim FirstFlg As Integer = 1

            If OraReader.DataReader(SQL) = True Then


                Do While OraReader.EOF = False

                    If FirstFlg = 1 Then
                        ReturnMessage = "エントリデータ入力済で落込未実行のデータが存在します。" & vbCrLf
                        FirstFlg = 0
                    End If

                    Dim MsgFuriDate As String = OraReader.GetString("FURI_DATE_S").Substring(0, 4) & "年" & _
                                                OraReader.GetString("FURI_DATE_S").Substring(4, 2) & "月" & _
                                                OraReader.GetString("FURI_DATE_S").Substring(6, 2) & "日"

                    ReturnMessage += vbCrLf & _
                                     "振替日:" & MsgFuriDate & _
                                     " 取引先コード:" & OraReader.GetString("TORIS_CODE_S") & "-" & OraReader.GetString("TORIF_CODE_S") & _
                                     " 委託者名:" & OraReader.GetString("ITAKU_NNAME_T").Trim

                    OraReader.NextRead()
                Loop
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, FunctionTitle, "終了", "")
        End Try

    End Function
    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- START

    Private Function GetTakouList(ByVal denso As Boolean) As String()
        Dim Arr As New ArrayList

        Dim Apps() As String = GetPrivateProfileSectionNames(Path.Combine(Application.StartupPath, "FURIWAKE.INI"))

        For i As Integer = 0 To Apps.Length - 1
            If denso = True Then
                If CASTCommon.GetIni("FURIWAKE.INI", Apps(i), "Baitai") = "00" Then
                    '*** 修正 mitsu 2009/05/29 SQL用「'」で括る ***
                    'Arr.Add(Apps(i))
                    Arr.Add(SQ(Apps(i)))
                    '**********************************************
                End If
            Else
                Dim sRet As String
                sRet = CASTCommon.GetIni("FURIWAKE.INI", Apps(i), "Baitai")
                If sRet <> "00" AndAlso sRet <> "err" Then
                    '*** 修正 mitsu 2009/05/29 SQL用「'」で括る ***
                    'Arr.Add(Apps(i))
                    Arr.Add(SQ(Apps(i)))
                    '**********************************************
                End If
            End If
        Next i

        Return CType(Arr.ToArray(GetType(String)), String())
    End Function

    Private Sub btnAllOn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True '((CType(sender, Button).Name.ToUpper = "BTNALLON")) OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)終了", "成功", "")

        End Try
    End Sub

    Private Sub btnAllOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False 'OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)終了", "成功", "")

        End Try
    End Sub

    '数値評価するがZERO埋めしない
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtCycle.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox))
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("数値評価編集1", "失敗", ex.ToString)
        End Try
    End Sub

End Class
