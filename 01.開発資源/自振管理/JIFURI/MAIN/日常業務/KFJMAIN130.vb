Option Explicit On 
Option Strict On

Imports System
Imports System.IO
Imports System.Data.OracleClient
Imports System.Collections
Imports CASTCommon

Public Class KFJMAIN130

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Dim SortOrderFlag As Boolean = True
    Dim ClickedColumn As Integer

    Private MyOwnerForm As Form

    Private Structure CodeNameInf
        Dim Code As Integer
        Dim Name As String
    End Structure
    Private BAITAI_CODE_INF() As CodeNameInf
    Private SYUBETU_CODE_INF() As CodeNameInf
    Private CHECK_KBN_INF() As CodeNameInf
    Private CHECK_FLG_INF() As CodeNameInf

    Private Const msgTitle As String = "未照合状況画面(KFJMAIN130)"
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN130", "未照合状況画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private Structure MEDIA_ENTRY_TBL
        Dim FSYORI_KBN As String
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
        Dim ITAKU_CODE As String
        Dim ITAKU_NNAME As String
        Dim BAITAI_KANRI_CODE As String
        Dim IN_DATE As String
        Dim STATION_IN_NO As String
        Dim IN_COUNTER As String
        Dim ENTRY_DATE As String
        Dim STATION_ENTRY_NO As String
        Dim ENTRY_NO As String
        Dim BAITAI_CODE As String
        Dim SYUBETU_CODE As String
        Dim FURI_DATE As String
        Dim SYORI_KEN As String
        Dim SYORI_KIN As String
        Dim CHECK_KBN As String
        Dim CHECK_FLG As String
        Dim CREATE_DATE As String
        Dim UPDATE_DATE As String
    End Structure
    Private DT As MEDIA_ENTRY_TBL

    '画面起動イベント処理
    Private Sub KFJMAIN130_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '--------------------------------------------------
            'ログの書込に必要な情報の取得
            '--------------------------------------------------
            Me.LW.UserID = GCom.GetUserID
            Me.LW.ToriCode = "000000000000"
            Me.LW.FuriDate = "00000000"

            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)開始", "成功", "")

            '--------------------------------------------------
            'システム日付とユーザ名を表示
            '--------------------------------------------------
            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            Call GetCodeName(BAITAI_CODE_INF, "Common_媒体コード.TXT")
            Call GetCodeName(SYUBETU_CODE_INF, "KFJMAST010_種別.TXT")
            Call GetCodeName(CHECK_KBN_INF, "KFJMAST011_照合要否区分.TXT")
            Call GetCodeName(CHECK_FLG_INF, "KFJMAIN130_照合結果.TXT")

            Call MeForm_Load_Action(ListView1)
            Call MeForm_Load_Action(ListView2)

            ListView2.Size = New Size(732, 196)

            If ListView2.Items.Count > 0 Then
                Application.DoEvents()
                ListView2.Items(0).Selected = True
            End If

            If ListView1.Items.Count > 0 Then
                Application.DoEvents()
                ListView1.Items(0).Selected = True
            End If

            '休日情報取得
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.GroupBox1.Enabled = True
            Me.GroupBox2.Enabled = True
            Me.GroupBox3.Enabled = False
            Me.GroupBox1.Visible = True
            Me.GroupBox2.Visible = True
            Me.GroupBox3.Visible = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    '画面表示(再表示)イベント処理
    Private Sub KFJOTHER0910MG_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job2 = "未照合状況(媒体)画面"
    End Sub

    '戻るボタン処理
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
        Try
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)例外エラー", "失敗", ex.Message)
        Finally
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(終了)終了", "成功", "")
        End Try
    End Sub

    '
    ' 機　能 : 画面初期化
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - ListView
    '
    ' 備　考 : 一覧表示再描画
    '    
    Private Sub MeForm_Load_Action(ByVal ListView As ListView)
        Try
            With ListView
                .Clear()
                Me.SuspendLayout()

                Select Case ListView.Name.ToUpper
                    Case "ListView1".ToUpper
                        .Columns.Add("処理区分", 0, HorizontalAlignment.Left)
                        .Columns.Add("取引先主コード", 0, HorizontalAlignment.Left)
                        .Columns.Add("取引先副コード", 0, HorizontalAlignment.Left)
                        .Columns.Add("委託者コード", 90, HorizontalAlignment.Left)
                        .Columns.Add("委託者名", 125, HorizontalAlignment.Left)
                        .Columns.Add("媒体管理コード", 0, HorizontalAlignment.Left)
                        .Columns.Add("入庫日", 0, HorizontalAlignment.Center)
                        .Columns.Add("入庫端末", 0, HorizontalAlignment.Center)
                        .Columns.Add("入庫通番", 0, HorizontalAlignment.Right)
                        .Columns.Add("送付状入力日", 0, HorizontalAlignment.Center)
                        .Columns.Add("端末", 40, HorizontalAlignment.Center)
                        .Columns.Add("通番", 50, HorizontalAlignment.Right)
                        .Columns.Add("媒体CD", 0, HorizontalAlignment.Center)
                        .Columns.Add("媒体", 60, HorizontalAlignment.Left)
                        .Columns.Add("種別", 40, HorizontalAlignment.Center)
                        .Columns.Add("種別名称", 0, HorizontalAlignment.Left)
                        .Columns.Add("振替日", 80, HorizontalAlignment.Center)
                        .Columns.Add("依頼件数", 60, HorizontalAlignment.Right)
                        .Columns.Add("依頼金額", 110, HorizontalAlignment.Right)
                        .Columns.Add("照合KBN", 0, HorizontalAlignment.Center)
                        .Columns.Add("照合可否", 80, HorizontalAlignment.Left)
                        .Columns.Add("照合FLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("照合結果", 65, HorizontalAlignment.Left)
                        .Columns.Add("作成日", 0, HorizontalAlignment.Left)
                        .Columns.Add("更新日", 0, HorizontalAlignment.Left)
                        .Columns.Add("ｱｯﾌﾟﾛｰﾄﾞFLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("ｱｯﾌﾟﾛｰﾄﾞ日", 0, HorizontalAlignment.Center)
                    Case "ListView2".ToUpper
                        .Columns.Add("実行日付", 0, HorizontalAlignment.Left)
                        .Columns.Add("実行時間", 0, HorizontalAlignment.Left)
                        .Columns.Add("種別", 40, HorizontalAlignment.Center)
                        .Columns.Add("委託者コード", 80, HorizontalAlignment.Center)
                        .Columns.Add("委託者名", 125, HorizontalAlignment.Left)
                        .Columns.Add("取引先主コード", 0, HorizontalAlignment.Left)
                        .Columns.Add("取引先副コード", 0, HorizontalAlignment.Left)
                        .Columns.Add("振替日", 80, HorizontalAlignment.Center)
                        .Columns.Add("サイクル番号.", 0, HorizontalAlignment.Center)
                        .Columns.Add("落し込み日", 0, HorizontalAlignment.Center)
                        .Columns.Add("件数", 60, HorizontalAlignment.Right)
                        .Columns.Add("金額", 110, HorizontalAlignment.Right)
                        .Columns.Add("媒体", 60, HorizontalAlignment.Left)
                        .Columns.Add("エラーコード", 0, HorizontalAlignment.Right)
                        .Columns.Add("照合結果", 100, HorizontalAlignment.Left)
                        .Columns.Add("通番", 0, HorizontalAlignment.Right)
                        .Columns.Add("処理区分", 0, HorizontalAlignment.Left)
                        .Columns.Add("カナ委託者名", 0, HorizontalAlignment.Left)
                        .Columns.Add("受付FLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("照合FLG", 0, HorizontalAlignment.Center)
                        .Columns.Add("FMT", 0, HorizontalAlignment.Center)
                        .Columns.Add("BAITAI", 0, HorizontalAlignment.Center)
                        .Columns.Add("CODE", 0, HorizontalAlignment.Center)
                End Select
            End With

            Dim SQL As String
            Dim ROW As Integer
            Dim nRet As Integer
            Dim Temp As Decimal
            Dim onDate As DateTime
            Dim onText(2) As Integer
            Dim LineColor As Color
            Dim REC As OracleDataReader = Nothing

            Select Case ListView.Name.ToUpper
                Case "ListView1".ToUpper

                    '媒体入出庫実績TBL情報で上部領域を描画
                    SQL = "SELECT E.FSYORI_KBN_ME"
                    SQL &= ", E.TORIS_CODE_ME"
                    SQL &= ", E.TORIF_CODE_ME"
                    SQL &= ", E.ITAKU_CODE_ME"
                    SQL &= ", V.ITAKU_NNAME_T"
                    SQL &= ", E.ITAKU_KANRI_CODE_ME"
                    SQL &= ", E.IN_DATE_ME"
                    SQL &= ", E.STATION_IN_NO_ME"
                    SQL &= ", E.IN_COUNTER_ME"
                    SQL &= ", E.ENTRY_DATE_ME"
                    SQL &= ", E.STATION_ENTRY_NO_ME"
                    SQL &= ", E.ENTRY_NO_ME"
                    SQL &= ", E.BAITAI_CODE_ME"
                    SQL &= ", E.SYUBETU_CODE_ME"
                    SQL &= ", E.FURI_DATE_ME"
                    SQL &= ", E.SYORI_KEN_ME"
                    SQL &= ", E.SYORI_KIN_ME"
                    SQL &= ", E.CHECK_KBN_ME"
                    SQL &= ", E.CHECK_FLG_ME"
                    SQL &= ", E.CREATE_DATE_ME"
                    SQL &= ", E.UPDATE_DATE_ME"
                    SQL &= ", E.UPLOAD_FLG_ME"
                    SQL &= ", E.UPLOAD_DATE_ME"
                    SQL &= " FROM MEDIA_ENTRY_TBL"
                    SQL &= " E"
                    SQL &= ", (SELECT FSYORI_KBN_T"
                    SQL &= ", TORIS_CODE_T"
                    SQL &= ", TORIF_CODE_T"
                    SQL &= ", ITAKU_NNAME_T"
                    SQL &= ", ITAKU_KNAME_T"
                    SQL &= " FROM TORIMAST"
                    SQL &= ") V"
                    SQL &= " WHERE E.FSYORI_KBN_ME = V.FSYORI_KBN_T"
                    SQL &= " AND E.TORIS_CODE_ME = V.TORIS_CODE_T"
                    SQL &= " AND E.TORIF_CODE_ME = V.TORIF_CODE_T"
                    SQL &= " AND NVL(E.DELETE_FLG_ME, '0') = '0'"
                    SQL &= " AND NVL(E.CHECK_KBN_ME, '0') = '1'"
                    SQL &= " AND NOT NVL(E.CHECK_FLG_ME, '0') = '1'"
                    SQL &= " AND NVL(E.FURI_DATE_ME, '00000000') >= TO_CHAR(SYSDATE, 'yyyymmdd')"
                    SQL &= " ORDER BY V.ITAKU_KNAME_T ASC"
                    SQL &= ", E.TORIS_CODE_ME ASC"
                    SQL &= ", E.TORIF_CODE_ME ASC"
                    SQL &= ", E.FURI_DATE_ME ASC"

                    If GCom.SetDynaset(SQL, REC) Then

                        ROW = 0
                        Do While REC.Read
                            Dim Data(26) As String

                            Data(0) = GCom.NzDec(REC.Item("FSYORI_KBN_ME"), "")        '処理区分
                            Data(1) = GCom.NzDec(REC.Item("TORIS_CODE_ME"), "")        '取引先主コード
                            Data(2) = GCom.NzDec(REC.Item("TORIF_CODE_ME"), "")        '取引先副コード
                            Data(3) = GCom.NzStr(REC.Item("ITAKU_CODE_ME")).Trim       '委託者コード
                            Data(4) = GCom.NzStr(REC.Item("ITAKU_NNAME_T")).Trim       '委託者名
                            Data(5) = GCom.NzDec(REC.Item("ITAKU_KANRI_CODE_ME"), "") '媒体管理コード

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("IN_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(6) = String.Format("{0:yyyy.MM.dd}", onDate)   '入庫日
                            End If
                            Data(7) = GCom.NzDec(REC.Item("STATION_IN_NO_ME"), "")     '入庫端末
                            Data(8) = GCom.NzDec(REC.Item("IN_COUNTER_ME"), "")        '入庫通番

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("ENTRY_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(9) = String.Format("{0:yyyy.MM.dd}", onDate)   '送付状入力日
                            End If
                            Data(10) = GCom.NzDec(REC.Item("STATION_ENTRY_NO_ME"), "") '送付状入力端末
                            Data(11) = GCom.NzDec(REC.Item("ENTRY_NO_ME"), "")         '送付状入力通番

                            Data(12) = GCom.NzDec(REC.Item("BAITAI_CODE_ME"), "")      '媒体コード
                            Data(13) = GetCodeName(GCom.NzInt(Data(12)), BAITAI_CODE_INF)   '媒体名称

                            Data(14) = GCom.NzDec(REC.Item("SYUBETU_CODE_ME"), "")     '種別コード
                            Data(15) = GetCodeName(GCom.NzInt(Data(14)), SYUBETU_CODE_INF)  '種別名称

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("FURI_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(16) = String.Format("{0:yyyy.MM.dd}", onDate)  '振替日
                            End If

                            Temp = GCom.NzDec(REC.Item("SYORI_KEN_ME"))
                            Data(17) = String.Format("{0:#,##0}", Temp)             '依頼件数

                            Temp = GCom.NzDec(REC.Item("SYORI_KIN_ME"))
                            Data(18) = String.Format("{0:#,##0}", Temp)             '依頼金額

                            Data(19) = GCom.NzDec(REC.Item("CHECK_KBN_ME"), "")        '照合区分
                            Data(20) = GetCodeName(GCom.NzInt(Data(19)), CHECK_KBN_INF) '照合区分名称

                            Data(21) = GCom.NzDec(REC.Item("CHECK_FLG_ME"), "")        '照合結果
                            Data(22) = GetCodeName(GCom.NzInt(Data(21)), CHECK_FLG_INF) '照合結果名称

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("CREATE_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(23) = String.Format("{0:yyyy.MM.dd}", onDate)  '作成日
                            End If
                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("UPDATE_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(24) = String.Format("{0:yyyy.MM.dd}", onDate)  '更新日
                            End If

                            Data(25) = GCom.NzDec(REC.Item("UPLOAD_FLG_ME"), "")       'アップロードFLG
                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("UPLOAD_DATE_ME"), ""))
                            If Not onDate = Nothing Then
                                Data(26) = String.Format("{0:yyyy.MM.dd}", onDate)  'アップロード日
                            End If

                            If ROW Mod 2 = 0 Then
                                LineColor = Color.White
                            Else
                                LineColor = Color.WhiteSmoke
                            End If

                            Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                            ListView.Items.AddRange(New ListViewItem() {vLstItem})

                            ROW += 1
                        Loop
                    End If
                Case "ListView2".ToUpper
                    SQL = ""

                    '照合エラーTBL情報で下部領域を描画
                    SQL = "SELECT MATCHING_DATE"            '実行日付
                    SQL &= ", MATCHING_TIME"                '実行時間
                    SQL &= ", FSYORI_KBN"                   '処理区分
                    SQL &= ", TORIS_CODE"                   '取引先主コード
                    SQL &= ", TORIF_CODE"                   '取引先副コード
                    SQL &= ", FURI_DATE"                    '振替日
                    SQL &= ", CYCLE_NO"                     'サイクル番号
                    SQL &= ", IN_DATE"                      '落とし込み日
                    SQL &= ", IN_BAITAI_CODE"               '媒体コード
                    SQL &= ", IN_ITAKU_CODE"                '委託者コード
                    SQL &= ", IN_FURI_DATE"                 '振替日
                    SQL &= ", IN_KEN"                       '件数
                    SQL &= ", IN_KIN"                       '金額
                    SQL &= ", ERR_CODE"                     'エラーコード
                    SQL &= ", ERR_TEXT"                     'エラー文言
                    SQL &= ", SEQ"                          '通番
                    SQL &= ", ITAKU_NNAME"
                    SQL &= ", ITAKU_KNAME"
                    SQL &= ", SYUBETU_CODE"
                    SQL &= ", UKETUKE_FLG"
                    SQL &= ", SYOUGOU_FLG"
                    SQL &= ", FMT_KBN"
                    SQL &= ", CODE_KBN"

                    'スケジュールマスタからの未照合情報
                    SQL &= " FROM (SELECT '00000000' MATCHING_DATE" '実行日付
                    SQL &= ", '000000' MATCHING_TIME"               '実行時間
                    SQL &= ", E.FSYORI_KBN_S FSYORI_KBN"            '処理区分
                    SQL &= ", E.TORIS_CODE_S TORIS_CODE"            '取引先主コード
                    SQL &= ", E.TORIF_CODE_S TORIF_CODE"            '取引先副コード
                    SQL &= ", E.FURI_DATE_S FURI_DATE"              '振替日
                    SQL &= ", E.MOTIKOMI_SEQ_S CYCLE_NO"            'サイクル番号
                    SQL &= ", E.UKETUKE_DATE_S IN_DATE"             '落とし込み日
                    SQL &= ", E.BAITAI_CODE_S IN_BAITAI_CODE"       '媒体コード
                    SQL &= ", E.ITAKU_CODE_S IN_ITAKU_CODE"         '委託者コード
                    SQL &= ", E.FURI_DATE_S IN_FURI_DATE"           '振替日
                    SQL &= ", E.SYORI_KEN_S IN_KEN"                 '件数
                    SQL &= ", E.SYORI_KIN_S IN_KIN"                 '金額
                    SQL &= ", NULL ERR_CODE"                        'エラーコード
                    'エラー文言
                    SQL &= ", DECODE(SYOUGOU_FLG_S, '0', '未照合', '9', '合計票なし', ' ') ERR_TEXT"
                    SQL &= ", NULL SEQ"                             '通番
                    SQL &= ", V.ITAKU_NNAME_T ITAKU_NNAME"
                    SQL &= ", V.ITAKU_KNAME_T ITAKU_KNAME"
                    SQL &= ", V.SYUBETU_T SYUBETU_CODE"
                    SQL &= ", E.UKETUKE_FLG_S UKETUKE_FLG"
                    SQL &= ", E.SYOUGOU_FLG_S SYOUGOU_FLG"
                    SQL &= ", V.FMT_KBN_T FMT_KBN"
                    SQL &= ", V.CODE_KBN_T CODE_KBN"
                    SQL &= " FROM SCHMAST_VIEW E"
                    SQL &= ", (SELECT FSYORI_KBN_T"
                    SQL &= ", TORIS_CODE_T"
                    SQL &= ", TORIF_CODE_T"
                    SQL &= ", ITAKU_NNAME_T"
                    SQL &= ", ITAKU_KNAME_T"
                    SQL &= ", SYUBETU_T"
                    SQL &= ", FMT_KBN_T"
                    SQL &= ", CODE_KBN_T"
                    SQL &= " FROM TORIMAST_VIEW"
                    SQL &= " WHERE SYOUGOU_KBN_T = '1'"
                    SQL &= ") V"
                    SQL &= " WHERE E.FSYORI_KBN_S = V.FSYORI_KBN_T"
                    SQL &= " AND E.TORIS_CODE_S = V.TORIS_CODE_T"
                    SQL &= " AND E.TORIF_CODE_S = V.TORIF_CODE_T"
                    SQL &= " AND NVL(E.UKETUKE_FLG_S, '0') = '1'"
                    SQL &= " AND NVL(E.SYOUGOU_FLG_S, '0') IN ('0', '9')"
                    SQL &= " AND NVL(E.TYUUDAN_FLG_S, '0') = '0'  AND E.TOUROKU_FLG_S = '0'"
                    SQL &= ")"

                    '***修正 2009.02.24 振替日が今日より後のもののみ表示 2009.02.24 start
                    SQL &= " WHERE NVL(FURI_DATE, '00000000') >= TO_CHAR(SYSDATE, 'yyyymmdd')"
                    '***修正 2009.02.24 振替日が今日より後のもののみ表示 2009.02.24 end

                    SQL &= " ORDER BY ITAKU_KNAME ASC"
                    SQL &= ", FSYORI_KBN ASC"
                    SQL &= ", TORIS_CODE ASC"
                    SQL &= ", TORIF_CODE ASC"
                    SQL &= ", FURI_DATE ASC"
                    SQL &= ", CYCLE_NO ASC"

                    If GCom.SetDynaset(SQL, REC) Then

                        Dim CurrentString As String = ""
                        Dim PreviousString As String = ""

                        ROW = 0
                        Do While REC.Read
                            Dim Data(22) As String

                            Data(0) = GCom.NzDec(REC.Item("MATCHING_DATE"), "")     '実行日付
                            Data(1) = GCom.NzDec(REC.Item("MATCHING_TIME"), "")     '実行時間
                            Data(2) = GCom.NzDec(REC.Item("SYUBETU_CODE"), "")      '処理区分
                            Data(3) = GCom.NzDec(REC.Item("IN_ITAKU_CODE"), "")     '委託者コード
                            Data(4) = GCom.NzStr(REC.Item("ITAKU_NNAME")).Trim      '委託者名
                            Data(5) = GCom.NzDec(REC.Item("TORIS_CODE"), "")        '取引先主コード
                            Data(6) = GCom.NzDec(REC.Item("TORIF_CODE"), "")        '取引先副コード

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("IN_FURI_DATE"), ""))
                            If Not onDate = Nothing Then
                                Data(7) = String.Format("{0:yyyy.MM.dd}", onDate)   '振替日
                            End If

                            Data(8) = GCom.NzDec(REC.Item("CYCLE_NO"), "")          'サイクル番号

                            onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("IN_DATE"), ""))
                            If Not onDate = Nothing Then
                                Data(9) = String.Format("{0:yyyy.MM.dd}", onDate)   '落とし込み日
                            End If

                            Temp = GCom.NzDec(REC.Item("IN_KEN"), 0)
                            Data(10) = String.Format("{0:#,##0}", Temp)             '件数

                            Temp = GCom.NzDec(REC.Item("IN_KIN"), 0)
                            Data(11) = String.Format("{0:#,##0}", Temp)             '金額

                            nRet = GCom.NzInt(REC.Item("IN_BAITAI_CODE"))
                            Data(12) = GetCodeName(nRet, BAITAI_CODE_INF)           '媒体

                            Data(13) = GCom.NzStr(REC.Item("ERR_CODE")).Trim        'エラーコード
                            Data(14) = GCom.NzStr(REC.Item("ERR_TEXT")).Trim        '照合結果
                            Data(15) = GCom.NzDec(REC.Item("SEQ"), "")              '通番
                            Data(16) = GCom.NzDec(REC.Item("FSYORI_KBN"), "")       '処理区分
                            Data(17) = GCom.NzStr(REC.Item("ITAKU_KNAME")).Trim     'カナ委託者名
                            Data(18) = GCom.NzDec(REC.Item("UKETUKE_FLG"), "")      '受付FLG
                            Data(19) = GCom.NzDec(REC.Item("SYOUGOU_FLG"), "")      '照合FLG
                            Data(20) = GCom.NzDec(REC.Item("FMT_KBN"), "")          'FMT_KBN
                            Data(21) = nRet.ToString.PadLeft(2, "0"c)               '媒体コード
                            Data(22) = GCom.NzDec(REC.Item("CODE_KBN"), "")         'コード区分

                            '同一レコードの排除
                            CurrentString = Data(17)            'カナ委託者名
                            CurrentString &= "-" & Data(16)     '処理区分
                            CurrentString &= "-" & Data(5)      '取引先主コード
                            CurrentString &= "-" & Data(6)      '取引先副コード
                            CurrentString &= "-" & Data(7)      '振替日
                            CurrentString &= "-" & Data(8)      'サイクル番号

                            If Not CurrentString = PreviousString Then

                                If ROW Mod 2 = 0 Then
                                    LineColor = Color.White
                                Else
                                    LineColor = Color.WhiteSmoke
                                End If

                                Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                                ListView.Items.AddRange(New ListViewItem() {vLstItem})

                                ROW += 1
                                PreviousString = CurrentString
                            End If
                        Loop
                    End If
            End Select
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "一覧初期描画処理", "失敗", ex.Message)
        Finally
            Me.ResumeLayout()
        End Try
    End Sub

    '受付情報変更ボタン処理
    Private Sub CmdUkeUpdate_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUkeUpdate.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(受付情報変更)開始", "成功", "")

            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            'コントロールの使用可否
            Me.GroupBox1.Enabled = False
            Me.GroupBox2.Enabled = False
            Me.GroupBox3.Enabled = True
            Me.GroupBox2.Visible = False
            Me.GroupBox3.Visible = True

            '内容設定
            Me.UpdateInitializa()
            Me.txtFuriDateY.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.MainLOG.Write(Me.LW.UserID, Me.LW.ToriCode, Me.LW.FuriDate, "(受付情報変更)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(受付情報変更)終了", "成功", "")
        End Try
    End Sub

    '
    ' 機　能 : 媒体コード情報を蓄積する
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 蓄積配列変数
    ' 　　　   ARG2 - テキストファイル名
    '
    ' 備　考 : コンボボックス共通関数
    '    
    Private Sub GetCodeName(ByRef avCodeName() As CodeNameInf, ByVal avFileName As String)
        Dim FL As StreamReader = Nothing
        Try
            Dim Index As Integer = 0

            Dim FileName As String = GCom.SET_PATH(GCom.GetTXTFolder) & avFileName
            If System.IO.File.Exists(FileName) Then

                FL = New StreamReader(FileName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

                Dim LineData As String = FL.ReadLine
                Do While Not LineData Is Nothing

                    Dim Data() As String = LineData.Split(","c)
                    If Data.Length >= 2 Then

                        ReDim Preserve avCodeName(Index)
                        avCodeName(Index).Code = GCom.NzInt(Data(0))
                        avCodeName(Index).Name = GCom.NzStr(Data(1)).Trim

                        Index += 1
                    End If

                    LineData = FL.ReadLine
                Loop
            End If
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体コード情報蓄積処理", "失敗", ex.Message)
        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Sub

    '
    ' 機　能 : コード情報から名称を返す
    '
    ' 戻り値 : 対象名称値
    '
    ' 引き数 : ARG1 - Code値
    ' 　　　   ARG2 - 記憶配列
    '
    ' 備　考 : 蓄積配列の参照
    '    
    Private Function GetCodeName(ByVal avIntValue As Integer, ByVal avCodeName() As CodeNameInf) As String
        Try
            For Each Temp As CodeNameInf In avCodeName

                If avIntValue = Temp.Code Then

                    Return Temp.Name
                End If
            Next
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "名称返却処理", "失敗", ex.Message)
        End Try

        Return ""
    End Function

    '
    ' 機　能 : 名称情報からコードを返す
    '
    ' 戻り値 : コード値
    '
    ' 引き数 : ARG1 - 名称値
    ' 　　　   ARG2 - 記憶配列
    '
    ' 備　考 : 蓄積配列の参照
    '    
    Private Function GetCodeName(ByVal avStrValue As String, ByVal avCodeName() As CodeNameInf) As Integer
        Try
            For Each Temp As CodeNameInf In avCodeName

                If avStrValue = Temp.Name Then

                    Return Temp.Code
                End If
            Next
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード返却処理", ex.Message)
        End Try

        Return Nothing
    End Function

    '検証用CSV出力&表示
    Private Sub LetMonitor_ListView(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles ListView1.DoubleClick, ListView2.DoubleClick

        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    '一覧表示領域のソート
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
        Handles ListView1.ColumnClick, ListView2.ColumnClick

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
    End Sub

    '
    ' 機　能 : 対等処理画面への遷移判定
    '
    ' 戻り値 : DialogResult.OK = 遷移する, DialogResult.Cancel = 遷移しない
    '
    ' 引き数 : ARG1 - 独自メッセージ
    ' 　　　   ARG2 - 記憶配列(値)
    '
    ' 備　考 : 共通化
    '
    Private Function CheckMessage(ByVal avMSG As String, ByRef StrData() As String) As DialogResult
        Try
            Dim MSG As String = ""

            If ListView2.Items.Count <= 0 Then
                Return DialogResult.Cancel
            Else
                If GCom.GetListViewHasRow(Me.ListView2) <= 0 Then
                    MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return DialogResult.Cancel
                End If
            End If

            Dim StrName() As String = {"委託者名", "媒体", "種別", "委託者コード", _
                                       "取引先主コード", "取引先副コード", "振替日", "件数", "金額"}

            Dim BAITAI As String = GCom.NzStr(GCom.SelectedItem(ListView2, 12))
            BAITAI = "( " & GetCodeName(BAITAI, BAITAI_CODE_INF).ToString.PadLeft(2, "0"c) & " ) " & BAITAI

            Dim SYUBETU As String = GCom.NzDec(GCom.SelectedItem(ListView2, 2), "")
            SYUBETU = "( " & SYUBETU & " ) " & GetCodeName(GCom.NzInt(SYUBETU), SYUBETU_CODE_INF)

            Dim Data() As String = {GCom.NzStr(GCom.SelectedItem(ListView2, 4)).Trim, _
                        BAITAI, SYUBETU, _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 3), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 5), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 6), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 7), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 10), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView2, 11), "")}
            StrData = Data

            MSG = ""
            Dim Index As Integer = 0
            For Each Temp As String In StrName
                Do While Temp.Length < 7
                    Temp &= "　"
                Loop
                MSG &= Temp & ControlChars.Tab & StrData(Index) & Space(8) & ControlChars.Cr
                Index += 1
            Next
            MSG &= ControlChars.Cr & avMSG & Space(8)
            Return MessageBox.Show(MSG, GCom.GLog.Job1, _
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "該当処理画面への遷移判定", "失敗", ex.Message)
        End Try
    End Function


    Private Sub UpdateInitializa()
        With DT
            .FSYORI_KBN = GCom.SelectedItem(Me.ListView1, 0).ToString
            .TORIS_CODE = GCom.SelectedItem(Me.ListView1, 1).ToString
            .TORIF_CODE = GCom.SelectedItem(Me.ListView1, 2).ToString
            .ITAKU_CODE = GCom.SelectedItem(Me.ListView1, 3).ToString
            .ITAKU_NNAME = GCom.SelectedItem(Me.ListView1, 4).ToString
            .BAITAI_KANRI_CODE = GCom.SelectedItem(Me.ListView1, 5).ToString
            .IN_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 6), "")
            .STATION_IN_NO = GCom.SelectedItem(Me.ListView1, 7).ToString
            .IN_COUNTER = GCom.SelectedItem(Me.ListView1, 8).ToString
            .ENTRY_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 9), "")
            .STATION_ENTRY_NO = GCom.SelectedItem(Me.ListView1, 10).ToString
            .ENTRY_NO = GCom.SelectedItem(Me.ListView1, 11).ToString
            .BAITAI_CODE = GCom.SelectedItem(Me.ListView1, 12).ToString
            .SYUBETU_CODE = GCom.SelectedItem(Me.ListView1, 14).ToString
            .FURI_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 16), "")
            .SYORI_KEN = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 17), "")
            .SYORI_KIN = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 18), "")
            .CHECK_KBN = GCom.SelectedItem(Me.ListView1, 19).ToString
            .CHECK_FLG = GCom.SelectedItem(Me.ListView1, 21).ToString
            .CREATE_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 23), "")
            .UPDATE_DATE = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 24), "")
        End With

        '委託者コード／名
        Me.lblItakuCode.Text = DT.ITAKU_CODE
        Me.lblItakuName.Text = Space(1) & GCom.GetLimitString(DT.ITAKU_NNAME, 48)

        '種目(種別)コード／名称
        Select Case DT.SYUBETU_CODE
            Case "91" : Me.lblSyubetu.Text = "91：口振"
            Case "21" : Me.lblSyubetu.Text = "21：総振"
            Case "11" : Me.lblSyubetu.Text = "11：給与"
            Case "12" : Me.lblSyubetu.Text = "12：賞与"
            Case Else : Me.lblSyubetu.Text = DT.SYUBETU_CODE
        End Select

        '振替日
        Dim onDate As DateTime = GCom.SET_DATE(DT.FURI_DATE)
        If Not onDate = Nothing Then
            Me.txtFuriDateY.Text = onDate.Year.ToString.PadLeft(4, "0"c)
            Me.txtFuriDateM.Text = onDate.Month.ToString.PadLeft(2, "0"c)
            Me.txtFuriDateD.Text = onDate.Day.ToString.PadLeft(2, "0"c)
        Else
            Me.txtFuriDateY.Text = New String("0"c, 4)
            Me.txtFuriDateM.Text = New String("0"c, 2)
            Me.txtFuriDateD.Text = New String("0"c, 2)
        End If

        '件数
        Me.txtKen.Text = String.Format("{0:#,##0}", GCom.NzDec(DT.SYORI_KEN))

        '金額
        Me.txtKin.Text = String.Format("{0:#,##0}", GCom.NzDec(DT.SYORI_KIN))

        '通番
        Dim Temp As String = GCom.NzDec(GCom.SelectedItem(Me.ListView1, 9), "")
        Temp &= " - "
        Temp &= GCom.NzDec(GCom.SelectedItem(Me.ListView1, 10), "")
        Temp &= " - "
        Temp &= GCom.NzDec(GCom.SelectedItem(Me.ListView1, 11), "")
        Me.lblSEQ.Text = Temp

    End Sub

    ''' <summary>
    ''' 更新ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUpdate.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")

            '--------------------------------------------------
            'テキストボックスの入力チェック
            '--------------------------------------------------
            If Me.CheckTextBox() = False Then
                Return
            End If

            '--------------------------------------------------
            '更新チェック
            '--------------------------------------------------
            'Dim NEW_SYUBETU_CODE As String
            Dim NEW_FURI_DATE As String
            Dim NEW_SYORI_KEN As String
            Dim NEW_SYORI_KIN As String

            NEW_FURI_DATE = String.Concat(New String() {Me.txtFuriDateY.Text, Me.txtFuriDateM.Text, Me.txtFuriDateD.Text})
            NEW_SYORI_KEN = Me.txtKen.Text.Replace(",", "")
            NEW_SYORI_KIN = Me.txtKin.Text.Replace(",", "")

            If DT.FURI_DATE = NEW_FURI_DATE AndAlso _
                DT.SYORI_KEN = NEW_SYORI_KEN AndAlso _
                DT.SYORI_KIN = NEW_SYORI_KIN Then
                MessageBox.Show(MSG0040I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.txtFuriDateY.Focus()
                Return
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '--------------------------------------------------
            '更新処理
            '--------------------------------------------------
            Dim Ret As Integer
            Dim SQL As String
            Dim Temp As String
            Dim SQLCode As Integer

            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            SQL = "UPDATE MEDIA_ENTRY_TBL"
            '種別は更新しない
            'SQL &= " SET SYUBETU_CODE_ME = '" & String.Format("{0:00}", GCom.NzInt(NEW_SYUBETU_CODE)) & "'"
            SQL &= " SET FURI_DATE_ME = '" & NEW_FURI_DATE & "'"
            SQL &= ", SYORI_KEN_ME = " & NEW_SYORI_KEN
            SQL &= ", SYORI_KIN_ME = " & NEW_SYORI_KIN
            SQL &= ", UPDATE_OP_ME = '" & GCom.GetUserID & "'"
            SQL &= ", UPDATE_DATE_ME = TO_CHAR(SYSDATE, 'yyyymmddHH24MIss')"
            With DT
                SQL &= " WHERE FSYORI_KBN_ME = '" & .FSYORI_KBN & "'"
                SQL &= " AND TORIS_CODE_ME = '" & .TORIS_CODE & "'"
                SQL &= " AND TORIF_CODE_ME = '" & .TORIF_CODE & "'"
                SQL &= " AND FURI_DATE_ME = '" & .FURI_DATE & "'"
                SQL &= " AND ENTRY_DATE_ME = '" & .ENTRY_DATE & "'"
                SQL &= " AND STATION_ENTRY_NO_ME = '" & .STATION_ENTRY_NO & "'"
                SQL &= " AND BAITAI_CODE_ME = '" & .BAITAI_CODE & "'"
                SQL &= " AND ENTRY_NO_ME = " & .ENTRY_NO
            End With

            Try
                '媒体入出庫実績ＴＢＬの更新
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)

            Catch ex As Exception
                Ret = 0
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テーブル更新", "失敗", ex.Message)

            Finally
                Select Case SQLCode
                    Case 0
                        Try
                            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                            Temp = NEW_FURI_DATE.Substring(0, 4)
                            Temp &= "." & NEW_FURI_DATE.Substring(4, 2)
                            Temp &= "." & NEW_FURI_DATE.Substring(6)
                            Call GCom.SelectedItem(Me.ListView1, 16, Temp)                  '振替日
                            Temp = String.Format("{0:#,##0}", GCom.NzDec(NEW_SYORI_KEN))
                            Call GCom.SelectedItem(Me.ListView1, 17, Temp)                  '依頼件数
                            Temp = String.Format("{0:#,##0}", GCom.NzDec(NEW_SYORI_KIN))
                            Call GCom.SelectedItem(Me.ListView1, 18, Temp)                  '依頼金額

                            Me.CmdCancel.PerformClick()
                        Catch ex As Exception
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テーブルコミット", "失敗", ex.Message)
                        End Try

                    Case Else
                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                        MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End Try

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "更新", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' キャンセルボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdCancel.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(キャンセル)開始", "成功", "")

            'コントロールの使用可否
            Me.GroupBox1.Enabled = True
            Me.GroupBox2.Enabled = True
            Me.GroupBox3.Enabled = False
            Me.GroupBox2.Visible = True
            Me.GroupBox3.Visible = False

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "キャンセル", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(キャンセル)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' テキストボックスの入力チェックをします。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function CheckTextBox() As Boolean
        Try
            '振替日（年）チェック
            If Me.txtFuriDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '振替日（月）チェック
            If Me.txtFuriDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            If GCom.NzInt(Me.txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            '振替日（日）チェック
            If Me.txtFuriDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            If GCom.NzInt(Me.txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = Me.txtFuriDateY.Text & "/" & Me.txtFuriDateM.Text & "/" & Me.txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '過去日チェック
            WORK_DATE = String.Concat(New String() {Me.txtFuriDateY.Text, Me.txtFuriDateM.Text, Me.txtFuriDateD.Text})
            If WORK_DATE <= String.Format("{0:yyyyMMdd}", GCom.GetSysDate) Then
                MessageBox.Show(MSG0387W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '営業日チェック
            Dim KyuCode As Integer = 0
            Dim ChangeDate As String = String.Empty
            Dim bRet As Boolean = GCom.CheckDateModule(WORK_DATE, ChangeDate, KyuCode)
            If Not WORK_DATE = ChangeDate Then
                MessageBox.Show(MSG0093W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '期限チェック
            Select Case WORK_DATE
                Case Is > GCom.GetSysDate.AddMonths(1).AddDays(-1).ToString("yyyyMMdd")     '一ヶ月以上先チェック
                    If MessageBox.Show(MSG0090I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                        Me.txtFuriDateY.Focus()
                        Return False
                    End If

                Case Is < GCom.GetSysDate.AddMonths(-1).AddDays(1).ToString("yyyyMMdd")     '一ヶ月以上前チェック
                    If MessageBox.Show(MSG0090I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                        Me.txtFuriDateY.Focus()
                        Return False
                    End If
            End Select

            '件数チェック
            If Me.txtKen.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "依頼件数"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKen.Focus()
                Return False
            End If

            '金額チェック
            If Me.txtKin.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "依頼金額"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKin.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストボックス入力チェック", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' テキストボックスヴァリデイティングイベント（ゼロパディング）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストボックスヴァリデイティングイベント", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
